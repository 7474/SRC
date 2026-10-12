using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Threading;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SRCCore.Threading.Tests
{
    /// <summary>
    /// CoreThreading（ゲームスレッドと UI スレッドへのマーシャリング）のユニットテスト
    /// </summary>
    [TestClass]
    public class CoreThreadingTests
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

        private UIThreadSimulator ui;
        private CoreThreading threading;

        [TestInitialize]
        public void Initialize()
        {
            ui = new UIThreadSimulator();
            // UI スレッド上で生成する
            threading = ui.Invoke(() => new CoreThreading(ui.Context));
            threading.Start();
        }

        [TestCleanup]
        public void Cleanup()
        {
            threading.Dispose();
            threading.Game.Join(Timeout);
            ui.Dispose();
        }

        /// <summary>ゲームスレッドで処理を実行し、完了を待つ</summary>
        private void RunOnGame(Action action)
        {
            var tcs = new TaskCompletionSource<bool>();
            threading.Post(() =>
            {
                try
                {
                    action();
                    tcs.SetResult(true);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            Assert.IsTrue(tcs.Task.Wait(Timeout), "game thread timeout");
        }

        [TestMethod]
        public void Post_RunsOnGameThread()
        {
            Thread executed = null;
            RunOnGame(() => executed = Thread.CurrentThread);
            Assert.IsNotNull(executed);
            Assert.AreNotEqual(ui.Thread, executed);
            Assert.AreNotEqual(Thread.CurrentThread, executed);
        }

        [TestMethod]
        public void GUICall_FromGameThread_RunsOnUIThread()
        {
            var gui = new MockGUI();
            Thread executed = null;
            gui.ClearScreanHandler = () => executed = Thread.CurrentThread;
            var proxy = threading.Wrap<IGUI>(gui);

            RunOnGame(() => proxy.ClearScrean());

            Assert.AreEqual(ui.Thread, executed);
        }

        [TestMethod]
        public void GUIProperty_FromGameThread_RoundTrips()
        {
            var gui = new MockGUI();
            var proxy = threading.Wrap<IGUI>(gui);
            int mapX = 0;

            RunOnGame(() =>
            {
                proxy.MapX = 12;
                mapX = proxy.MapX;
            });

            Assert.AreEqual(12, mapX);
            Assert.AreEqual(12, gui.MapX);
        }

        [TestMethod]
        public void GUICall_ReturnValueAndOutParameter_AreReturned()
        {
            var gui = new MockGUI();
            gui.InputHandler = (message, title, defaultValue) => (GuiDialogResult.Ok, defaultValue + "!");
            var proxy = threading.Wrap<IGUI>(gui);
            GuiDialogResult result = default;
            string inputValue = null;

            RunOnGame(() => result = proxy.Input("msg", "title", "abc", out inputValue));

            Assert.AreEqual(GuiDialogResult.Ok, result);
            Assert.AreEqual("abc!", inputValue);
        }

        [TestMethod]
        public void GUICall_Exception_IsRethrownOnGameThreadWithOriginalType()
        {
            var gui = new MockGUI();
            var proxy = threading.Wrap<IGUI>(gui);
            Exception caught = null;

            // MockGUI はハンドラ未設定のメソッドで GUINotImplementedException を投げる
            RunOnGame(() =>
            {
                try
                {
                    proxy.ClearScrean();
                }
                catch (Exception ex)
                {
                    caught = ex;
                }
            });

            Assert.IsInstanceOfType(caught, typeof(GUINotImplementedException));
        }

        [TestMethod]
        public void GUICall_FromUIThread_RunsDirectly()
        {
            var gui = new MockGUI();
            Thread executed = null;
            gui.ClearScreanHandler = () => executed = Thread.CurrentThread;
            var proxy = threading.Wrap<IGUI>(gui);

            ui.Invoke(() => proxy.ClearScrean());

            Assert.AreEqual(ui.Thread, executed);
        }

        [TestMethod]
        public void GUICall_ReenteringProxyOnUIThread_DoesNotDeadlock()
        {
            var gui = new MockGUI();
            IGUI proxy = null;
            Thread inner = null;
            gui.ClearScreanHandler = () => proxy.ClearPicture();
            gui.ClearPictureHandler = () => inner = Thread.CurrentThread;
            proxy = threading.Wrap<IGUI>(gui);

            RunOnGame(() => proxy.ClearScrean());

            Assert.AreEqual(ui.Thread, inner);
        }

        [TestMethod]
        public void DoEvents_OnGameThread_RunsPostedActionsOnGameThread()
        {
            var gui = new MockGUI();
            gui.DoEventsHandler = () => Assert.Fail("DoEvents must not be forwarded to the GUI");
            var proxy = threading.Wrap<IGUI>(gui);
            var log = new List<string>();
            Thread postedThread = null;

            RunOnGame(() =>
            {
                log.Add("before");
                // UI スレッドのイベントハンドラから処理が投入された想定
                ui.Invoke(() => threading.Post(() =>
                {
                    postedThread = Thread.CurrentThread;
                    log.Add("posted");
                }));
                proxy.DoEvents();
                log.Add("after");
            });

            CollectionAssert.AreEqual(new[] { "before", "posted", "after" }, log);
            Assert.IsTrue(threading.Game.IsRunning);
            Assert.AreNotEqual(ui.Thread, postedThread);
        }

        [TestMethod]
        public void Sleep_WithEvents_RunsPostedActionsAndWaits()
        {
            var gui = new MockGUI();
            gui.SleepHandler = (ms, withEvents) => Assert.Fail("Sleep must not be forwarded to the GUI");
            var proxy = threading.Wrap<IGUI>(gui);
            var posted = false;
            TimeSpan elapsed = default;

            RunOnGame(() =>
            {
                threading.Post(() => posted = true);
                var sw = System.Diagnostics.Stopwatch.StartNew();
                proxy.Sleep(50, true);
                elapsed = sw.Elapsed;
            });

            Assert.IsTrue(posted);
            Assert.IsTrue(elapsed >= TimeSpan.FromMilliseconds(40), elapsed.ToString());
        }

        [TestMethod]
        public void Sleep_WithoutEvents_DoesNotRunPostedActions()
        {
            var gui = new MockGUI();
            var proxy = threading.Wrap<IGUI>(gui);
            var postedDuringSleep = false;

            RunOnGame(() =>
            {
                var posted = false;
                threading.Post(() => posted = true);
                proxy.Sleep(0, false);
                postedDuringSleep = posted;
            });

            Assert.IsFalse(postedDuringSleep);
        }

        [TestMethod]
        public void DoEvents_FromUIThread_IsForwardedToGUI()
        {
            var gui = new MockGUI();
            var forwarded = false;
            gui.DoEventsHandler = () => forwarded = true;
            var proxy = threading.Wrap<IGUI>(gui);

            ui.Invoke(() => proxy.DoEvents());

            Assert.IsTrue(forwarded);
        }

        [TestMethod]
        public void Wrap_AlreadyWrapped_ReturnsSameInstance()
        {
            var proxy = threading.Wrap<IGUI>(new MockGUI());
            Assert.AreSame(proxy, threading.Wrap(proxy));
        }

        [TestMethod]
        public void Wrap_Null_ReturnsNull()
        {
            Assert.IsNull(threading.Wrap<IGUI>(null));
        }

        [TestMethod]
        public void Install_WrapsAllGUIInterfaces()
        {
            var headless = new HeadlessGUI();
            var src = new SRC();
            headless.Install(src);

            threading.Install(src);

            Assert.IsInstanceOfType(src.GUI, typeof(CoreThreadProxy));
            Assert.IsInstanceOfType(src.GUIMap, typeof(CoreThreadProxy));
            Assert.IsInstanceOfType(src.GUIStatus, typeof(CoreThreadProxy));
            Assert.IsInstanceOfType(src.GUIScrean, typeof(CoreThreadProxy));
            Assert.IsInstanceOfType(src.Sound.Player, typeof(CoreThreadProxy));
        }

        [TestMethod]
        public void UnhandledException_IsReportedAndThreadKeepsRunning()
        {
            Exception reported = null;
            using (var reportedEvent = new ManualResetEventSlim())
            {
                threading.Game.UnhandledException += (s, e) =>
                {
                    reported = e.Exception;
                    reportedEvent.Set();
                };
                threading.Post(() => throw new InvalidOperationException("boom"));
                Assert.IsTrue(reportedEvent.Wait(Timeout));
            }

            Assert.IsInstanceOfType(reported, typeof(InvalidOperationException));
            var ran = false;
            RunOnGame(() => ran = true);
            Assert.IsTrue(ran);
        }

        [TestMethod]
        public void ProcessPending_OutsideGameThread_Throws()
        {
            Assert.ThrowsExactly<InvalidOperationException>(() => threading.Game.ProcessPending());
        }
    }
}
