using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SRCAuthoring.Protocol;
using SRCCore.Logging;
using SRCLogging;
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
        private readonly LogHost logHost;
        private readonly ILogger log;
        private int seq;
        private JsonNode launchArgs;
        private ScreenScenarioRunner runner;
        private EventDebugger debugger;
        private InteractiveScreen interactive;
        /// <summary>自動で進めるときにフレームだけを送る</summary>
        private InteractiveScreen frames;
        private HotReloader reloader;
        private readonly Dictionary<string, List<int>> pendingBreakpoints = new Dictionary<string, List<int>>(StringComparer.OrdinalIgnoreCase);
        private Task session;
        private volatile bool disconnected;
        private volatile bool sessionEnded;

        /// <param name="logHost">ログの構成。ログは output イベントでデバッグコンソールにも送る（警告以上は stderr）</param>
        public DapServer(Stream input, Stream output, LogHost logHost = null)
        {
            stream = new MessageStream(input, output);
            this.logHost = logHost;
            if (logHost != null)
            {
                logHost.LoggerFactory.AddProvider(new DelegateLoggerProvider((category, level, message, ex) =>
                    Output($"[{DelegateLoggerProvider.ShortLevel(level)}] {category}: {message}" + (ex == null ? "" : "\n" + ex),
                        level >= LogLevel.Warning ? "stderr" : "console")));
                log = logHost.LoggerFactory.CreateLogger("SRCSharp.Dap");
            }
            else
            {
                log = NullLogger.Instance;
            }
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
                    log.LogError(ex, "{Command} を処理できません", command);
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
                case InteractiveScreen.InputCommand:
                    RequireInteractive().Input(args);
                    return null;
                case HotReloader.ReloadCommand:
                    return (reloader ?? throw new InvalidOperationException("シナリオを実行していません")).Request(args);
                case InteractiveScreen.PromptResultCommand:
                    RequireInteractive().PromptResult(args);
                    return null;
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
            // SRC 本体のフォルダ（Lib・Data\System・Bitmap などの共通の素材）。無ければシナリオのフォルダの親から探し、それも無ければシナリオのフォルダ
            var scenarioRoot = SRCAuthoring.Analysis.ScenarioAnalyzer.FindScenarioRoot(program);
            var appPath = launchArgs["appPath"]?.GetValue<string>()
                ?? SRCAuthoring.Analysis.ScenarioAnalyzer.FindAppPath(scenarioRoot)
                ?? scenarioRoot;
            var stopOnEntry = launchArgs["stopOnEntry"]?.GetValue<bool>() ?? false;
            // script が無く interactive でもないときの自動プレイ：選択肢は先頭、マップは方針（policy）で動かし、ターンを終える
            var policy = launchArgs["policy"]?.GetValue<string>() ?? "smart";
            var turns = launchArgs["turns"]?.GetValue<int>() ?? 1000;
            // ウェイト（Wait・メッセージの表示待ちなど）を飛ばす。画面は送り続けるので、早送りで眺められる
            var skipWait = launchArgs["skipWait"]?.GetValue<bool>() ?? false;
            // .play スクリプトがあればそれで動かし、タブの操作は受けない
            var isInteractive = (launchArgs["interactive"]?.GetValue<bool>() ?? false) && string.IsNullOrEmpty(script);
            if (!SRCRender.Compat.SkiaFonts.HasJapaneseFont())
            {
                // 画面の日本語が豆腐になる。important は VS Code が通知で出す
                Output($"日本語のフォントが見つからないため、画面の日本語が表示されません。日本語のフォントを入れるか（Ubuntu・Debian: sudo apt install fonts-noto-cjk）、環境変数 {SRCRender.Compat.SkiaFonts.FontsEnvironmentVariable} にフォントのファイルかフォルダを指定してください。", "important");
            }

            // ログのレベル（launch.json の logLevel、VS Code 拡張の srcsharp.logLevel）
            var logLevel = LogSettings.ParseLevel(launchArgs["logLevel"]?.GetValue<string>());
            if (logLevel.HasValue)
            {
                logHost?.OverrideLevel(logLevel);
            }
            runner = new ScreenScenarioRunner(appPath, logHost?.LoggerFactory);
            logHost?.ApplyConfig(runner.Session.SRC.SystemConfig);
            log.LogInformation("デバッグを開始 {Program} SRC のフォルダ: {AppPath}", program, appPath);
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
            reloader = new HotReloader(runner, SendEventAsync);
            if (isInteractive)
            {
                interactive = new InteractiveScreen(runner, SendEventAsync, () => debugger.IsPaused, log);
                interactive.Start();
            }
            else
            {
                // 自動で進めるときも、表示したフレームを送る（タブの操作は受けない）
                frames = new InteractiveScreen(runner, SendEventAsync, () => debugger.IsPaused, log, acceptInput: false);
                frames.Start();
            }
            SRCCore.Lib.GeneralLib.SkipWait = skipWait;

            session = Task.Run(() =>
            {
                try
                {
                    runner.Start(program);
                    if (interactive != null)
                    {
                        // タブの操作で進める。シナリオが終わるか、デバッグを止めるまで待つ
                        while (!runner.IsFinished && !disconnected)
                        {
                            Thread.Sleep(50);
                        }
                    }
                    else if (!string.IsNullOrEmpty(script))
                    {
                        var play = new PlayScript(runner) { Trace = line => Output(line) };
                        play.Run(File.ReadAllLines(script));
                    }
                    else
                    {
                        var play = new PlayScript(runner) { Trace = line => Output(line) };
                        play.Run(new[]
                        {
                            "policy " + policy,
                            "choose コマンド 次のステージへ",
                            "turns " + turns,
                        });
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
                    interactive?.Dispose();
                    frames?.Dispose();
                    reloader?.Dispose();
                    SRCCore.Lib.GeneralLib.SkipWait = false;
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

        private InteractiveScreen RequireInteractive()
        {
            return interactive ?? throw new InvalidOperationException("interactive で起動した時だけ使えます");
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
            // 答えを待っている文字入力などを打ち切る
            interactive?.Dispose();
            frames?.Dispose();
            reloader?.Dispose();
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
