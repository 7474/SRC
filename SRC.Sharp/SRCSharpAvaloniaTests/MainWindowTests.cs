using Avalonia;
using Avalonia.Headless;
using Avalonia.Input;
using Avalonia.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkiaSharp;
using SRCCore.TestLib;
using SRCRender;
using SRCRender.Tests;
using System;
using System.IO;
using System.Threading;

namespace SRCSharpAvalonia.Tests
{
    /// <summary>
    /// SRC#Avalonia のウィンドウを headless で動かし、マウス入力でシナリオを進める。
    /// </summary>
    [TestClass]
    public class MainWindowTests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(20);

        /// <summary>
        /// MainWindow へ委譲し、Terminate 後はシナリオの実行を打ち切る IScreenHost。
        /// </summary>
        private class TestHost : IScreenHost
        {
            private readonly MainWindow window;

            public TestHost(MainWindow window)
            {
                this.window = window;
            }

            public volatile bool Terminated;
            public int PresentCount;

            public void Present(SKBitmap frame)
            {
                if (Terminated)
                {
                    throw new HeadlessTerminatedException();
                }
                window.Present(frame);
                Interlocked.Increment(ref PresentCount);
            }

            public void SetTitle(string title)
            {
                window.SetTitle(title);
            }

            public bool Terminate()
            {
                Terminated = true;
                return false;
            }
        }

        private static void PumpUntil(Func<bool> condition, string description)
        {
            var deadline = DateTime.UtcNow + Timeout;
            while (!condition())
            {
                if (DateTime.UtcNow > deadline)
                {
                    throw new TimeoutException("timeout waiting for " + description);
                }
                Dispatcher.UIThread.RunJobs();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                Thread.Sleep(10);
            }
            Dispatcher.UIThread.RunJobs();
            AvaloniaHeadlessPlatform.ForceRenderTimerTick();
        }

        /// <summary>
        /// 新しいフレームが届かなくなるまで待ち、ウィンドウへ反映させる。
        /// </summary>
        private static void PumpUntilIdle(TestHost host)
        {
            var last = -1;
            var stable = 0;
            while (stable < 5)
            {
                var current = Volatile.Read(ref host.PresentCount);
                stable = current == last ? stable + 1 : 0;
                last = current;
                Dispatcher.UIThread.RunJobs();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick();
                Thread.Sleep(10);
            }
        }

        private static void Click(MainWindow window, Point point, MouseButton button = MouseButton.Left)
        {
            window.MouseDown(point, button);
            window.MouseUp(point, button);
        }

        private static string SaveFrame(MainWindow window, string name)
        {
            var frame = window.CaptureRenderedFrame();
            Assert.IsNotNull(frame, "frame");
            var dir = Path.Combine(AppContext.BaseDirectory, "TestResults", "avalonia");
            Directory.CreateDirectory(dir);
            var path = Path.Combine(dir, name + ".png");
            frame.Save(path);
            return path;
        }

        [TestMethod]
        public void PlayScenario_WithMouseInput()
        {
            using (var workspace = new ScenarioWorkspace())
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                workspace.WriteMap("test.map", 20, 20, (x, y) => x <= 10 ? 0 : 1);
                var eve = workspace.Write("test.eve",
                    "プロローグ:",
                    "ChangeMap test.map",
                    "Talk システム",
                    "ヘッドレステスト開始",
                    "End",
                    "Ask 選択してください",
                    "はい",
                    "いいえ",
                    "End",
                    "If 選択 = 1 Then",
                    "Talk システム",
                    "はいが選ばれた",
                    "End",
                    "EndIf",
                    "Quit",
                    "");

                session.Dispatch(() =>
                {
                    var window = new MainWindow();
                    window.Show();
                    var host = new TestHost(window);
                    using (var game = new ScreenSession(host, workspace.AppPath))
                    {
                        Exception error = null;
                        game.Game.UnhandledException += (s, e) => error = e.Exception;
                        window.View.GUI = game.GUI;
                        game.Start(eve);
                        var state = game.GUI.State;

                        // メッセージ表示
                        PumpUntil(() => state.Message.Waiting, "message");
                        PumpUntilIdle(host);
                        Assert.AreEqual("ヘッドレステスト開始", state.Message.Lines[0]);
                        var framePath = SaveFrame(window, "01-message");
                        using (var frame = SKBitmap.Decode(framePath))
                        {
                            // マップ左上は平地（緑）、ウィンドウ経由で描画されている
                            var pixel = frame.GetPixel(16, 16);
                            Assert.IsTrue(pixel.Green > pixel.Red && pixel.Green > pixel.Blue, pixel.ToString());
                        }
                        Click(window, new Point(ScreenLayout.MessageArea.MidX, ScreenLayout.MessageArea.MidY));

                        // 選択肢
                        PumpUntil(() => state.List != null, "list");
                        PumpUntilIdle(host);
                        SaveFrame(window, "02-list");
                        var layout = game.GUI.Renderer.LayoutList(state.List);
                        var item = layout.ItemRect(0);
                        Click(window, new Point(item.MidX, item.MidY));

                        // 選択結果
                        PumpUntil(() => state.Message.Waiting && state.Message.Lines.Count > 0 && state.Message.Lines[0] == "はいが選ばれた", "result message");
                        PumpUntilIdle(host);
                        SaveFrame(window, "03-after-choice");
                        Click(window, new Point(ScreenLayout.MessageArea.MidX, ScreenLayout.MessageArea.MidY));

                        PumpUntil(() => error != null, "termination");
                        Assert.IsTrue(host.Terminated);
                        Assert.IsInstanceOfType(error, typeof(HeadlessTerminatedException), error.ToString());
                    }
                    window.Close();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }

        [TestMethod]
        public void InputText_ReturnsTypedTextOrNullOnCancel()
        {
            using (var session = HeadlessUnitTestSession.StartNew(typeof(TestApp)))
            {
                session.Dispatch(() =>
                {
                    var window = new MainWindow();
                    window.Show();

                    // 入力して OK
                    var task = window.InputTextAsync("SRC", "名前を入力してください", "名無し");
                    PumpUntil(() => window.OwnedWindows.Count > 0, "input window");
                    var input = (InputWindow)window.OwnedWindows[0];
                    Assert.AreEqual("名無し", input.TextBox.Text);
                    input.TextBox.Text = "";
                    input.TextBox.Focus();
                    input.KeyTextInput("ジェイド");
                    input.KeyPressQwerty(PhysicalKey.Enter, RawInputModifiers.None);
                    PumpUntil(() => task.IsCompleted, "input result");
                    Assert.AreEqual("ジェイド", task.Result);

                    // Escape でキャンセル
                    task = window.InputTextAsync("SRC", "名前を入力してください", "名無し");
                    PumpUntil(() => window.OwnedWindows.Count > 0, "input window");
                    input = (InputWindow)window.OwnedWindows[0];
                    input.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);
                    PumpUntil(() => task.IsCompleted, "cancel result");
                    Assert.IsNull(task.Result);

                    window.Close();
                    return true;
                }, CancellationToken.None).GetAwaiter().GetResult();
            }
        }
    }
}
