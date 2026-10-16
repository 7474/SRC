using SRCAuthoring.Protocol;
using SRCPlay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SRCDebugAdapter
{
    /// <summary>
    /// イベントのデバッガ（Debug Adapter Protocol）。シナリオを画面なしで動かし、
    /// 入力は .play スクリプト（無ければ自動で既定の選択）で与える。
    /// https://microsoft.github.io/debug-adapter-protocol/specification
    /// </summary>
    public sealed class DapServer
    {
        private const int ThreadId = 1;
        private const int LocalsReference = 1;
        private const int GlobalsReference = 2;

        /// <summary>
        /// 画面（最後に表示したフレーム）を PNG で返す独自のリクエスト。シナリオが終わる時は同じ名前のイベントで最後の画面を送る。
        /// VS Code 拡張の画面のビューが使う
        /// </summary>
        public const string ScreenCommand = "srcsharpScreen";

        private readonly MessageStream stream;
        private int seq;
        private JsonNode launchArgs;
        private ScreenScenarioRunner runner;
        private EventDebugger debugger;
        private readonly Dictionary<string, List<int>> pendingBreakpoints = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
        private Task session;
        private volatile bool disconnected;
        private volatile bool sessionEnded;

        public DapServer(Stream input, Stream output)
        {
            stream = new MessageStream(input, output);
        }

        public async Task<int> RunAsync(CancellationToken cancellationToken = default)
        {
            while (!disconnected && !cancellationToken.IsCancellationRequested)
            {
                var message = await stream.ReadAsync(cancellationToken);
                if (message == null)
                {
                    break;
                }
                if (message["type"]?.GetValue<string>() != "request")
                {
                    continue;
                }
                var command = message["command"]?.GetValue<string>();
                var requestSeq = message["seq"]?.GetValue<int>() ?? 0;
                try
                {
                    var body = await HandleAsync(command, message["arguments"]);
                    await RespondAsync(requestSeq, command, true, body);
                    if (command == "initialize")
                    {
                        await SendEventAsync("initialized", null);
                    }
                }
                catch (Exception ex)
                {
                    await RespondAsync(requestSeq, command, false, null, ex.Message);
                }
            }
            Shutdown();
            return 0;
        }

        private Task RespondAsync(int requestSeq, string command, bool success, JsonNode body, string message = null)
        {
            var response = new JsonObject
            {
                ["seq"] = Interlocked.Increment(ref seq),
                ["type"] = "response",
                ["request_seq"] = requestSeq,
                ["success"] = success,
                ["command"] = command,
            };
            if (body != null)
            {
                response["body"] = body;
            }
            if (message != null)
            {
                response["message"] = message;
            }
            return stream.WriteAsync(response);
        }

        private Task SendEventAsync(string name, JsonNode body)
        {
            var ev = new JsonObject
            {
                ["seq"] = Interlocked.Increment(ref seq),
                ["type"] = "event",
                ["event"] = name,
            };
            if (body != null)
            {
                ev["body"] = body;
            }
            return stream.WriteAsync(ev);
        }

        private void Output(string text, string category = "console")
        {
            _ = SendEventAsync("output", new JsonObject { ["category"] = category, ["output"] = text + "\n" });
        }

        private async Task<JsonNode> HandleAsync(string command, JsonNode args)
        {
            switch (command)
            {
                case "initialize":
                    return new JsonObject
                    {
                        ["supportsConfigurationDoneRequest"] = true,
                        ["supportsTerminateRequest"] = true,
                        ["supportsEvaluateForHovers"] = true,
                    };
                case "launch":
                    launchArgs = args?.DeepClone() ?? new JsonObject();
                    if (string.IsNullOrEmpty(launchArgs["program"]?.GetValue<string>()))
                    {
                        throw new ArgumentException("program（実行するイベントファイル）を指定してください");
                    }
                    return null;
                case "setBreakpoints":
                    {
                        var path = args["source"]?["path"]?.GetValue<string>();
                        var lines = (args["breakpoints"] as JsonArray ?? new JsonArray())
                            .Select(x => x["line"].GetValue<int>())
                            .ToList();
                        if (!string.IsNullOrEmpty(path))
                        {
                            pendingBreakpoints[path] = lines;
                            debugger?.SetBreakpoints(path, lines);
                        }
                        var result = new JsonArray();
                        foreach (var line in lines)
                        {
                            result.Add(new JsonObject { ["verified"] = true, ["line"] = line });
                        }
                        return new JsonObject { ["breakpoints"] = result };
                    }
                case "setExceptionBreakpoints":
                    return new JsonObject { ["breakpoints"] = new JsonArray() };
                case "configurationDone":
                    StartSession();
                    return null;
                case "threads":
                    return new JsonObject
                    {
                        ["threads"] = new JsonArray { new JsonObject { ["id"] = ThreadId, ["name"] = "イベント" } },
                    };
                case "stackTrace":
                    return StackTrace();
                case "scopes":
                    return new JsonObject
                    {
                        ["scopes"] = new JsonArray
                        {
                            new JsonObject { ["name"] = "ローカル", ["variablesReference"] = LocalsReference, ["expensive"] = false },
                            new JsonObject { ["name"] = "グローバル", ["variablesReference"] = GlobalsReference, ["expensive"] = true },
                        },
                    };
                case "variables":
                    return Variables(args["variablesReference"].GetValue<int>());
                case "continue":
                    debugger?.Continue();
                    return new JsonObject { ["allThreadsContinued"] = true };
                case "next":
                    debugger?.Next();
                    return null;
                case "stepIn":
                    debugger?.StepIn();
                    return null;
                case "stepOut":
                    debugger?.StepOut();
                    return null;
                case "pause":
                    debugger?.Pause();
                    return null;
                case "evaluate":
                    return Evaluate(args["expression"]?.GetValue<string>());
                case ScreenCommand:
                    return Screen();
                case "terminate":
                case "disconnect":
                    disconnected = true;
                    Shutdown();
                    if (session != null)
                    {
                        await Task.WhenAny(session, Task.Delay(TimeSpan.FromSeconds(5)));
                    }
                    return null;
                default:
                    throw new NotSupportedException(command);
            }
        }

        private void StartSession()
        {
            var program = Path.GetFullPath(launchArgs["program"].GetValue<string>());
            var script = launchArgs["script"]?.GetValue<string>();
            var appPath = launchArgs["appPath"]?.GetValue<string>() ?? SRCAuthoring.Analysis.ScenarioAnalyzer.FindScenarioRoot(program);
            var stopOnEntry = launchArgs["stopOnEntry"]?.GetValue<bool>() ?? false;
            var maxSteps = launchArgs["maxSteps"]?.GetValue<int>() ?? 100000;

            runner = new ScreenScenarioRunner(appPath);
            debugger = new EventDebugger(runner.Session.SRC);
            foreach (var kv in pendingBreakpoints)
            {
                debugger.SetBreakpoints(kv.Key, kv.Value);
            }
            if (stopOnEntry)
            {
                debugger.StopOnEntry();
            }
            debugger.Stopped = info =>
            {
                runner.IsSuspended = true;
                var body = new JsonObject
                {
                    ["reason"] = info.Reason,
                    ["threadId"] = ThreadId,
                    ["allThreadsStopped"] = true,
                };
                if (!string.IsNullOrEmpty(info.Text))
                {
                    body["text"] = info.Text;
                    body["description"] = info.Text;
                    Output($"{info.Line?.File}({info.Line?.LineNum}): {info.Text}", "stderr");
                }
                _ = SendEventAsync("stopped", body);
            };
            debugger.Resumed = () => runner.IsSuspended = false;
            runner.Session.SRC.Event.Debugger = debugger;

            session = Task.Run(() =>
            {
                try
                {
                    runner.Start(program);
                    if (!string.IsNullOrEmpty(script))
                    {
                        var play = new PlayScript(runner) { Trace = line => Output(line) };
                        play.Run(File.ReadAllLines(script));
                    }
                    else
                    {
                        var auto = new AutoPlayer(runner) { Trace = line => { if (line.StartsWith("stall")) { Output(line); } } };
                        auto.Run(maxSteps);
                    }
                    if (runner.Error != null && !(runner.Error is SRCCore.TestLib.HeadlessTerminatedException))
                    {
                        Output(runner.Error.ToString(), "stderr");
                    }
                }
                catch (Exception ex)
                {
                    Output(ex.Message, "stderr");
                }
                finally
                {
                    // 終わった後はリクエストで画面を取れないため、最後の画面をイベントで送る
                    try
                    {
                        SendEventAsync(ScreenCommand, ScreenBody(false)).Wait();
                    }
                    catch (Exception ex)
                    {
                        Output(ex.Message, "stderr");
                    }
                    sessionEnded = true;
                    _ = SendEventAsync("terminated", null);
                    runner.Dispose();
                }
            });
        }

        private JsonNode StackTrace()
        {
            var frames = new JsonArray();
            var id = 0;
            foreach (var line in debugger?.StackLines() ?? new List<SRCCore.Events.EventDataLine>())
            {
                frames.Add(new JsonObject
                {
                    ["id"] = id++,
                    ["name"] = string.IsNullOrEmpty(line.Data) ? "(空行)" : line.Data,
                    ["source"] = new JsonObject
                    {
                        ["name"] = Path.GetFileName(line.File ?? ""),
                        ["path"] = string.IsNullOrEmpty(line.File) ? null : Path.GetFullPath(line.File),
                    },
                    ["line"] = line.LineNum,
                    ["column"] = 1,
                });
            }
            return new JsonObject { ["stackFrames"] = frames, ["totalFrames"] = frames.Count };
        }

        private JsonNode Variables(int reference)
        {
            var list = debugger == null
                ? new List<(string Name, string Value)>()
                : reference == LocalsReference ? debugger.LocalVariables() : debugger.GlobalVariables();
            var result = new JsonArray();
            foreach (var (name, value) in list)
            {
                result.Add(new JsonObject { ["name"] = name, ["value"] = value, ["variablesReference"] = 0 });
            }
            return new JsonObject { ["variables"] = result };
        }

        private JsonNode Evaluate(string expression)
        {
            if (debugger == null || !debugger.IsPaused)
            {
                throw new InvalidOperationException("止まっている時だけ式を評価できます");
            }
            var value = runner.Session.SRC.Expression.GetValueAsString(expression ?? "");
            return new JsonObject { ["result"] = value, ["variablesReference"] = 0 };
        }

        private JsonNode Screen()
        {
            return ScreenBody(runner != null && !sessionEnded);
        }

        /// <summary>画面の応答・イベントの本体。png は最後に表示したフレーム（base64）</summary>
        private JsonObject ScreenBody(bool running)
        {
            var result = new JsonObject
            {
                ["running"] = running,
                ["paused"] = debugger?.IsPaused ?? false,
            };
            if (runner == null || sessionEnded)
            {
                return result;
            }
            // 止まっている間はゲームスレッドが待っているため、描画の反映を待たずに最後のフレームを撮る
            using (var png = new MemoryStream())
            {
                try
                {
                    runner.GUI.SnapshotPng(png);
                }
                catch (ObjectDisposedException)
                {
                    // 終わった直後
                    return result;
                }
                if (png.Length > 0)
                {
                    result["png"] = Convert.ToBase64String(png.ToArray());
                }
            }
            return result;
        }

        private void Shutdown()
        {
            debugger?.Terminate();
            if (runner != null)
            {
                runner.IsSuspended = false;
                // シナリオを打ち切る
                runner.Session.GUI.Terminate();
            }
        }
    }
}
