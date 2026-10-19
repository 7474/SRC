using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SkiaSharp;
using SRCCore;
using SRCPlay;
using SRCRender;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace SRCDebugAdapter
{
    /// <summary>
    /// デバッグ中のシナリオを VS Code のタブで遊ぶための中継（launch の interactive）。
    /// 表示したフレームを独自のイベントで送り、タブのクリック・キーを独自のリクエストで受けて ScreenGUI に渡す。
    /// 文字入力・ファイル選択は独自のイベントで尋ね、独自のリクエストで答えを受ける。
    /// acceptInput が false なら（自動で進めるとき）フレームを送るだけで、操作も尋ねることもしない。
    /// </summary>
    public sealed class InteractiveScreen : IDisposable
    {
        /// <summary>フレームのイベント（body: format, data（base64）, width, height）</summary>
        public const string FrameEvent = "srcsharpFrame";
        /// <summary>入力のリクエスト（arguments: type = pointer・wheel・key と、その値）</summary>
        public const string InputCommand = "srcsharpInput";
        /// <summary>文字入力・ファイル選択を尋ねるイベント（body: id, kind = input・open・save と、その値）</summary>
        public const string PromptEvent = "srcsharpPrompt";
        /// <summary>尋ねたことへの答えのリクエスト（arguments: id, value（キャンセルは null））</summary>
        public const string PromptResultCommand = "srcsharpPromptResult";

        /// <summary>フレームを送る最短の間隔（約 15 枚／秒）</summary>
        private static readonly TimeSpan FrameInterval = TimeSpan.FromMilliseconds(66);
        private const int FrameQuality = 85;

        private readonly ScreenScenarioRunner runner;
        private readonly Func<string, JsonNode, Task> sendEvent;
        private readonly Func<bool> isPaused;
        private readonly ILogger log;
        private readonly BlockingCollection<Action<ScreenGUI>> inputs = new BlockingCollection<Action<ScreenGUI>>();
        private readonly ConcurrentDictionary<int, TaskCompletionSource<string>> prompts = new ConcurrentDictionary<int, TaskCompletionSource<string>>();
        private readonly CancellationTokenSource stop = new CancellationTokenSource();
        private readonly bool acceptInput;
        private int promptId;
        private int sentFrame = -1;

        public InteractiveScreen(ScreenScenarioRunner runner, Func<string, JsonNode, Task> sendEvent, Func<bool> isPaused, ILogger log = null, bool acceptInput = true)
        {
            this.log = log ?? NullLogger.Instance;
            this.runner = runner;
            this.sendEvent = sendEvent;
            this.isPaused = isPaused;
            this.acceptInput = acceptInput;
            if (!acceptInput)
            {
                return;
            }
            runner.InputHandler = (title, message, defaultValue) => Ask(new JsonObject
            {
                ["kind"] = "input",
                ["title"] = title,
                ["message"] = message,
                ["value"] = defaultValue,
            });
            runner.OpenFileHandler = request => Ask(FileRequest("open", request));
            runner.SaveFileHandler = request => Ask(FileRequest("save", request));
        }

        /// <summary>入力を渡すスレッドと、フレームを送るスレッドを動かす</summary>
        public void Start()
        {
            if (acceptInput)
            {
                new Thread(PumpInputs) { IsBackground = true, Name = "SRC# DAP input" }.Start();
            }
            new Thread(PumpFrames) { IsBackground = true, Name = "SRC# DAP frame" }.Start();
        }

        private void PumpInputs()
        {
            try
            {
                foreach (var input in inputs.GetConsumingEnumerable(stop.Token))
                {
                    try
                    {
                        input(runner.GUI);
                    }
                    catch (Exception ex)
                    {
                        log.LogError(ex, "画面の操作を渡せません");
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        private void PumpFrames()
        {
            while (!stop.IsCancellationRequested)
            {
                try
                {
                    SendFrameIfChanged();
                }
                catch (Exception ex)
                {
                    // 送れなくなった（クライアントが切れたなど）。このスレッドの例外はプロセスを落とすため、ここで止める
                    log.LogWarning(ex, "画面を送れません");
                    return;
                }
                stop.Token.WaitHandle.WaitOne(FrameInterval);
            }
        }

        /// <summary>前に送ってから表示が変わっていれば、今のフレームを送る</summary>
        public void SendFrameIfChanged()
        {
            var count = runner.FrameCount;
            if (count == sentFrame)
            {
                return;
            }
            sentFrame = count;
            try
            {
                using (var stream = new MemoryStream())
                {
                    // 文字がにじまないよう、劣化の少ない WebP にする（Webview は Chromium のため表示できる）
                    runner.GUI.Snapshot(stream, SKEncodedImageFormat.Webp, FrameQuality);
                    if (stream.Length == 0)
                    {
                        return;
                    }
                    sendEvent(FrameEvent, new JsonObject
                    {
                        ["format"] = "webp",
                        ["data"] = Convert.ToBase64String(stream.ToArray()),
                    }).Wait();
                }
            }
            catch (ObjectDisposedException)
            {
                // 終わった直後
            }
            catch (AggregateException ex) when (ex.InnerException is ObjectDisposedException || ex.InnerException is IOException)
            {
                // 送る先のパイプが閉じた直後
            }
        }

        /// <summary>入力のリクエストを ScreenGUI の操作にする。止まっている間の押下は捨てる（再開した時に古い操作が効かないように）</summary>
        public void Input(JsonNode args)
        {
            var type = args?["type"]?.GetValue<string>();
            var x = (float)(args?["x"]?.GetValue<double>() ?? 0);
            var y = (float)(args?["y"]?.GetValue<double>() ?? 0);
            var paused = isPaused();
            switch (type)
            {
                case "pointer":
                    {
                        // kind: down・up・move。button: 0 左、2 右（MouseEvent.button）
                        var kind = args["kind"]?.GetValue<string>();
                        var button = args["button"]?.GetValue<int>() == 2 ? GuiButton.Right : GuiButton.Left;
                        var clicks = Math.Max(1, args["clicks"]?.GetValue<int>() ?? 1);
                        if (kind == "down" && !paused)
                        {
                            inputs.Add(g => g.PointerPressed(x, y, button, clicks));
                        }
                        else if (kind == "up" && !paused)
                        {
                            inputs.Add(g => g.PointerReleased(x, y, button));
                        }
                        else if (kind == "move")
                        {
                            inputs.Add(g => g.PointerMoved(x, y));
                        }
                        break;
                    }
                case "wheel":
                    {
                        var delta = args["delta"]?.GetValue<int>() ?? 0;
                        if (!paused)
                        {
                            inputs.Add(g => g.PointerWheel(x, y, delta));
                        }
                        break;
                    }
                case "key":
                    {
                        // kind: down・up・blur（フォーカスを失った）。keyCode は KeyboardEvent.keyCode（Windows の仮想キーコードと同じ）
                        var kind = args["kind"]?.GetValue<string>();
                        var keyCode = args["keyCode"]?.GetValue<int>() ?? 0;
                        if (kind == "blur")
                        {
                            inputs.Add(g => g.ReleaseAllKeys());
                        }
                        else if (kind == "up")
                        {
                            inputs.Add(g => g.KeyStateChanged(keyCode, false));
                        }
                        else if (kind == "down" && !paused)
                        {
                            inputs.Add(g => g.KeyStateChanged(keyCode, true));
                            var key = ScreenKeyOf(keyCode);
                            if (key.HasValue)
                            {
                                var k = key.Value;
                                inputs.Add(g => g.KeyPressed(k));
                            }
                        }
                        break;
                    }
                default:
                    throw new ArgumentException("type は pointer・wheel・key のどれかです");
            }
        }

        /// <summary>キーの操作（SRC#Web と同じ割り当て）</summary>
        public static ScreenKey? ScreenKeyOf(int keyCode)
        {
            switch (keyCode)
            {
                case 13:
                case 32:
                    return ScreenKey.Enter;
                case 27:
                case 8:
                case 46:
                    return ScreenKey.Escape;
                case 37:
                    return ScreenKey.Left;
                case 38:
                    return ScreenKey.Up;
                case 39:
                    return ScreenKey.Right;
                case 40:
                    return ScreenKey.Down;
                default:
                    return null;
            }
        }

        private static JsonObject FileRequest(string kind, FileDialogRequest request)
        {
            return new JsonObject
            {
                ["kind"] = kind,
                ["title"] = request.Title,
                ["initialDirectory"] = request.InitialDirectory,
                ["value"] = request.InitialFile,
                ["fileType"] = request.FileType,
                ["extension"] = request.FileExtension,
            };
        }

        /// <summary>タブの側に尋ね、答えを待つ（ゲームのスレッドとは別のスレッドで呼ばれる）</summary>
        private string Ask(JsonObject body)
        {
            var id = Interlocked.Increment(ref promptId);
            var answer = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            prompts[id] = answer;
            body["id"] = id;
            try
            {
                sendEvent(PromptEvent, body).Wait();
                using (stop.Token.Register(() => answer.TrySetResult(null)))
                {
                    return answer.Task.Result;
                }
            }
            finally
            {
                prompts.TryRemove(id, out _);
            }
        }

        /// <summary>尋ねたことへの答え</summary>
        public void PromptResult(JsonNode args)
        {
            var id = args?["id"]?.GetValue<int>() ?? 0;
            if (prompts.TryGetValue(id, out var answer))
            {
                answer.TrySetResult(args["value"]?.GetValue<string>());
            }
        }

        public void Dispose()
        {
            stop.Cancel();
            foreach (var answer in prompts.Values)
            {
                answer.TrySetResult(null);
            }
        }
    }
}
