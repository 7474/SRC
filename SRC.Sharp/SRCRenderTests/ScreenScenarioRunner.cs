using SkiaSharp;
using SRCCore;
using SRCCore.TestLib;
using System;
using System.IO;
using System.Threading;

namespace SRCRender.Tests
{
    /// <summary>
    /// ScreenGUI でシナリオを実行し、画面の状態を見ながら入力を与える。
    /// </summary>
    public sealed class ScreenScenarioRunner : IScreenHost, IDisposable
    {
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(20);

        private readonly ManualResetEventSlim finished = new ManualResetEventSlim();
        private volatile bool terminated;
        private int frameCount;

        public ScreenScenarioRunner(string appPath)
        {
            Session = new ScreenSession(this, appPath);
            Session.Game.UnhandledException += (s, e) =>
            {
                Error = e.Exception;
                finished.Set();
            };
        }

        public ScreenSession Session { get; }
        public ScreenGUI GUI => Session.GUI;
        public ScreenState State => Session.GUI.State;
        public Exception Error { get; private set; }
        public bool IsTerminated => terminated;
        public int FrameCount => frameCount;

        public void Start(string eve)
        {
            Session.Start(eve);
        }

        /// <summary>シナリオが終了（Quit など）するまで待つ。</summary>
        public void WaitForTermination()
        {
            if (!finished.Wait(Timeout))
            {
                throw new TimeoutException("scenario did not terminate");
            }
            if (!(Error is HeadlessTerminatedException))
            {
                throw new InvalidOperationException("scenario failed", Error);
            }
        }

        public void WaitFor(Func<ScreenState, bool> condition, string description)
        {
            var deadline = DateTime.UtcNow + Timeout;
            while (!condition(State))
            {
                if (finished.IsSet)
                {
                    throw new InvalidOperationException("scenario finished while waiting for " + description, Error);
                }
                if (DateTime.UtcNow > deadline)
                {
                    throw new TimeoutException("timeout waiting for " + description);
                }
                Thread.Sleep(10);
            }
        }

        /// <summary>メッセージのクリック待ちになるまで待ち、表示中の行を返す。</summary>
        public string WaitForMessage()
        {
            WaitFor(s => s.Message.Visible && s.Message.Waiting, "message");
            return string.Join("\n", State.Message.Lines);
        }

        /// <summary>メッセージ送り（画面下部をクリック）</summary>
        public void ClickMessage()
        {
            var area = ScreenLayout.MessageArea;
            GUI.PointerPressed(area.MidX, area.MidY, GuiButton.Left);
            GUI.PointerReleased(area.MidX, area.MidY, GuiButton.Left);
            WaitFor(s => !s.Message.Waiting, "message closed");
        }

        public ListState WaitForList()
        {
            WaitFor(s => s.List != null, "list");
            return State.List;
        }

        /// <summary>選択肢の index 番目（0 始まり）をクリックする。</summary>
        public void ChooseListItem(int index)
        {
            var list = WaitForList();
            var layout = GUI.Renderer.LayoutList(list);
            var rect = layout.ItemRect(index - layout.Top);
            GUI.PointerPressed(rect.MidX, rect.MidY, GuiButton.Left);
            GUI.PointerReleased(rect.MidX, rect.MidY, GuiButton.Left);
            WaitFor(s => s.List == null, "list closed");
        }

        public byte[] SnapshotPng()
        {
            using (var stream = new MemoryStream())
            {
                GUI.SnapshotPng(stream);
                return stream.ToArray();
            }
        }

        #region IScreenHost

        void IScreenHost.Present(SKBitmap frame)
        {
            if (terminated)
            {
                // Terminate 後はシナリオの実行を打ち切る
                throw new HeadlessTerminatedException();
            }
            Interlocked.Increment(ref frameCount);
        }

        void IScreenHost.SetTitle(string title)
        {
        }

        bool IScreenHost.Terminate()
        {
            terminated = true;
            // false を返して Environment.Exit させない
            return false;
        }

        #endregion

        public void Dispose()
        {
            Session.Dispose();
            Session.Game.Join(TimeSpan.FromSeconds(5));
            finished.Dispose();
        }
    }
}
