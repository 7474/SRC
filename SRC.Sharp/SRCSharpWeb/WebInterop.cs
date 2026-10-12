using SRCCore;
using SRCRender;
using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;
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
        private static readonly BlockingCollection<Action<ScreenGUI>> inputs = new BlockingCollection<Action<ScreenGUI>>();

        [JSImport("drawFrame", ModuleName)]
        internal static partial void DrawFrame([JSMarshalAs<JSType.MemoryView>] ArraySegment<byte> pixels, int width, int height);

        [JSImport("setTitle", ModuleName)]
        internal static partial void SetDocumentTitle(string title);

        [JSImport("bindCanvas", ModuleName)]
        internal static partial void BindCanvas(string id);

        /// <summary>画面と入力の受け渡しを始める。UI スレッドで呼ぶ。</summary>
        public static void Attach(string canvasId, WebScreenHost screenHost, ScreenGUI screenGui)
        {
            host = screenHost;
            gui = screenGui;
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
