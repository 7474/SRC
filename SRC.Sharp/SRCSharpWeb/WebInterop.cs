using SRCCore;
using SRCRender;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace SRCSharpWeb
{
    /// <summary>
    /// canvas との受け渡し（wwwroot/js/srcweb.js）。
    /// マルチスレッドの WebAssembly では .NET はブラウザの UI スレッドとは別のスレッドで動き、
    /// UI スレッドからは非同期（Task を返す）の呼び出ししかできないため、公開するメソッドは Task を返す。
    /// 入力は専用のスレッドで ScreenGUI へ渡す（JS との受け渡しを担うスレッドでロックを待たないため）。
    /// </summary>
    public static partial class WebInterop
    {
        public const string AppTitle = "SRC#Web";
        public const string ModuleName = "srcweb";

        private static WebScreenHost host;
        private static ScreenGUI gui;
        private static SRCCore.SRC src;
        private static readonly BlockingCollection<Action<ScreenGUI>> inputs = new BlockingCollection<Action<ScreenGUI>>();

        // バイト列は Span で渡す（JS 側は呼び出しの間に複写する）。
        // ArraySegment で渡すと配列ごとに GCHandle が JS に渡り、JS の GC（FinalizationRegistry）で解放される。
        // 同じ配列（画面の受け渡し用に使い回す配列）を何度も渡すと、マルチスレッドではその解放が二重になり、
        // ReleaseJSOwnedObjectByGCHandle の ArgumentNullException でランタイムが落ちる（dotnet/runtime#107486）。
        // 解放されるまで配列が固定・保持され続けることもない。
        [JSImport("drawFrame", ModuleName)]
        internal static partial void DrawFrame([JSMarshalAs<JSType.MemoryView>] Span<byte> pixels, int width, int height);

        [JSImport("setTitle", ModuleName)]
        internal static partial void SetDocumentTitle(string title);

        [JSImport("reportError", ModuleName)]
        internal static partial void ReportError(string type, string message, string stack);

        [JSImport("showInput", ModuleName)]
        internal static partial Task<string> ShowInput(string title, string message, string value, string ok, string cancel);

        [JSImport("showChoice", ModuleName)]
        internal static partial Task<string> ShowChoice(string title, [JSMarshalAs<JSType.Array<JSType.String>>] string[] items, string cancel);

        [JSImport("soundPlayEncoded", ModuleName)]
        internal static partial void SoundPlayEncoded(int channel, [JSMarshalAs<JSType.MemoryView>] Span<byte> data, bool loop);

        [JSImport("soundQueuePcm", ModuleName)]
        internal static partial void SoundQueuePcm(int channel, [JSMarshalAs<JSType.MemoryView>] Span<byte> samples, int channels, int sampleRate);

        [JSImport("soundAhead", ModuleName)]
        internal static partial double SoundAhead(int channel);

        [JSImport("soundStop", ModuleName)]
        internal static partial void SoundStop(int channel);

        [JSImport("soundVolume", ModuleName)]
        internal static partial void SoundVolume(double volume);

        [JSImport("bindCanvas", ModuleName)]
        internal static partial void BindCanvas(string id);

        /// <summary>画面と入力の受け渡しを始める。UI スレッドで呼ぶ。</summary>
        public static void Attach(string canvasId, WebScreenHost screenHost, ScreenSession session)
        {
            host = screenHost;
            gui = session.GUI;
            src = session.SRC;
            var pump = new Thread(() =>
            {
                foreach (var input in inputs.GetConsumingEnumerable())
                {
                    try
                    {
                        input(gui);
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex);
                    }
                }
            })
            {
                IsBackground = true,
                Name = "SRC#Web input",
            };
            pump.Start();
            BindCanvas(canvasId);
        }

        /// <summary>シナリオが終了したか</summary>
        public static bool Terminated => host?.Terminated ?? false;

        [JSExport]
        public static Task Tick()
        {
            host?.Flush();
            return Task.CompletedTask;
        }

        /// <summary>
        /// 画面とイベントの状態（動作確認・E2E テスト用）。JSON で返す。
        /// line: 実行中のイベントの行、list: 表示中のリスト、dialog: 表示中のダイアログ、message: クリック待ちのメッセージ
        /// </summary>
        private static readonly System.Text.Json.JsonSerializerOptions JsonOptions = new System.Text.Json.JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        [JSExport]
        public static Task<string> DebugState()
        {
            var json = new JsonObject
            {
                ["present"] = host?.PresentCount ?? 0,
                ["flush"] = host?.FlushCount ?? 0,
                ["terminated"] = Terminated,
            };
            var e = src?.Event;
            if (e != null)
            {
                var n = e.CurrentLineNum;
                if (e.EventData != null && n >= 0 && n < e.EventData.Count)
                {
                    var data = e.EventData[n];
                    json["line"] = $"{data.File}:{data.LineNum} {data.Data}";
                }
            }
            try
            {
                AddScreenState(json);
            }
            catch (Exception ex)
            {
                // ゲームスレッドが更新している最中に読んだ場合
                json["error"] = ex.Message;
            }
            return Task.FromResult(json.ToJsonString(JsonOptions));
        }

        private static void AddScreenState(JsonObject json)
        {
            if (gui != null)
            {
                // マップの表示位置（中央のマス）と、マップ操作を受け付けるか（ドラッグのテストなどで使う）
                json["mapX"] = gui.MapX;
                json["mapY"] = gui.MapY;
                json["locked"] = gui.IsGUILocked;
            }
            var state = gui?.State;
            var list = state?.List;
            if (list != null)
            {
                var items = new JsonArray();
                foreach (var item in list.Items)
                {
                    items.Add(item.Text);
                }
                json["list"] = new JsonObject
                {
                    ["caption"] = list.Caption,
                    ["info"] = list.Info,
                    ["highlight"] = list.Highlight,
                    ["items"] = items,
                };
            }
            if (state?.Dialog != null)
            {
                json["dialog"] = state.Dialog.Title + ": " + state.Dialog.Text;
            }
            if (state != null && state.Message.Visible && state.Message.Waiting)
            {
                json["message"] = string.Join("\n", state.Message.Lines);
            }
        }

        /// <summary>
        /// タッチ向けの選択画面（srcweb.js）に出す、表示中のリスト・メニュー・ダイアログ。JSON で返す（無いものは含めない）。
        /// ゲームの画面に描く小さなリスト・メニュー・ダイアログを、タッチの端末では大きなボタンで選べるようにする
        /// </summary>
        [JSExport]
        public static Task<string> TouchUi()
        {
            var json = new JsonObject();
            try
            {
                var state = gui?.State;
                var list = state?.List;
                if (list != null && gui.IsWaitingList)
                {
                    var items = new JsonArray();
                    foreach (var item in list.Items)
                    {
                        items.Add(new JsonObject { ["text"] = item.Text, ["disabled"] = item.Disabled });
                    }
                    json["list"] = new JsonObject { ["caption"] = list.Caption, ["info"] = list.Info, ["items"] = items };
                }
                var menu = state?.Menu;
                if (menu != null)
                {
                    var items = new JsonArray();
                    foreach (var item in menu.Items)
                    {
                        items.Add(item);
                    }
                    json["menu"] = new JsonObject { ["items"] = items };
                }
                var dialog = state?.Dialog;
                if (dialog != null)
                {
                    var buttons = new JsonArray();
                    foreach (var button in dialog.Buttons)
                    {
                        buttons.Add(button);
                    }
                    json["dialog"] = new JsonObject { ["title"] = dialog.Title, ["text"] = dialog.Text, ["buttons"] = buttons };
                }
            }
            catch (Exception ex)
            {
                // ゲームスレッドが更新している最中に読んだ場合（次の呼び出しで読み直す）
                json["error"] = ex.Message;
            }
            return Task.FromResult(json.ToJsonString(JsonOptions));
        }

        /// <summary>
        /// タッチ向けの選択画面で選んだものをゲームに渡す。自動プレイ（SRCPlay の PlayScript）と同じ操作にする。
        /// kind: 0 リストの項目（カーソルを合わせて Enter）、1 メニューの項目（項目をクリック）、2 ダイアログのボタン（ボタンをクリック）、3 キャンセル（右クリック）
        /// </summary>
        [JSExport]
        public static Task TouchChoose(int kind, int index)
        {
            inputs.Add(g =>
            {
                var state = g.State;
                switch (kind)
                {
                    case 0:
                        var list = state.List;
                        if (list != null && g.IsWaitingList && index >= 0 && index < list.Items.Count)
                        {
                            list.Highlight = index;
                            g.KeyPressed(ScreenKey.Enter);
                        }
                        break;
                    case 1:
                        var menu = state.Menu;
                        if (menu != null && index >= 0 && index < menu.Items.Count)
                        {
                            var p = g.Renderer.MenuItemCenter(menu, index);
                            Click(g, p.X, p.Y, GuiButton.Left);
                        }
                        break;
                    case 2:
                        var dialog = state.Dialog;
                        if (dialog != null && index >= 0 && index < dialog.Buttons.Count)
                        {
                            var rect = g.Renderer.LayoutDialog(dialog).ButtonRect(index);
                            Click(g, rect.MidX, rect.MidY, GuiButton.Left);
                        }
                        break;
                    case 3:
                        Click(g, ScreenLayout.MapWidthPx / 2f, ScreenLayout.MapHeightPx / 2f, GuiButton.Right);
                        break;
                }
            });
            return Task.CompletedTask;
        }

        private static void Click(ScreenGUI g, float x, float y, GuiButton button)
        {
            g.PointerMoved(x, y);
            g.PointerPressed(x, y, button, 1);
            g.PointerReleased(x, y, button);
        }

        /// <summary>kind: 0 押した、1 離した、2 移動</summary>
        [JSExport]
        public static Task Pointer(int kind, double x, double y, int button, int clicks)
        {
            var b = button == 2 ? GuiButton.Right : GuiButton.Left;
            var fx = (float)x;
            var fy = (float)y;
            switch (kind)
            {
                case 0:
                    inputs.Add(g => g.PointerPressed(fx, fy, b, Math.Max(1, clicks)));
                    break;
                case 1:
                    inputs.Add(g => g.PointerReleased(fx, fy, b));
                    break;
                case 3:
                    // タッチで指を滑らせた: クリックにせずマップのドラッグを始める
                    inputs.Add(g => g.BeginMapDrag(fx, fy));
                    break;
                case 4:
                    inputs.Add(g => g.EndMapDrag());
                    break;
                default:
                    inputs.Add(g => g.PointerMoved(fx, fy));
                    break;
            }
            return Task.CompletedTask;
        }

        [JSExport]
        public static Task Wheel(double x, double y, int delta)
        {
            inputs.Add(g => g.PointerWheel((float)x, (float)y, delta));
            return Task.CompletedTask;
        }

        /// <summary>kind: 0 押した、1 離した、2 フォーカスを失った。keyCode は KeyboardEvent.keyCode（Windows の仮想キーコードと同じ）</summary>
        [JSExport]
        public static Task Key(int kind, int keyCode)
        {
            if (kind == 2)
            {
                inputs.Add(g => g.ReleaseAllKeys());
                return Task.CompletedTask;
            }
            var pressed = kind == 0;
            inputs.Add(g => g.KeyStateChanged(keyCode, pressed));
            if (!pressed)
            {
                return Task.CompletedTask;
            }
            ScreenKey? key = keyCode switch
            {
                13 => ScreenKey.Enter,
                32 => ScreenKey.Enter,
                27 => ScreenKey.Escape,
                8 => ScreenKey.Escape,
                46 => ScreenKey.Escape,
                37 => ScreenKey.Left,
                38 => ScreenKey.Up,
                39 => ScreenKey.Right,
                40 => ScreenKey.Down,
                _ => null,
            };
            if (key.HasValue)
            {
                var k = key.Value;
                inputs.Add(g => g.KeyPressed(k));
            }
            return Task.CompletedTask;
        }
    }
}
