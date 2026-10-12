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
    public sealed class ScreenScenarioRunner : IScreenHost, IScreenDialogHost, IDisposable
    {
        public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(20);

        private readonly ManualResetEventSlim finished = new ManualResetEventSlim();
        private volatile bool terminated;
        private int frameCount;

        public ScreenScenarioRunner(string appPath, Microsoft.Extensions.Logging.ILoggerFactory loggerFactory = null, SRCCore.IPlaySound soundPlayer = null)
        {
            Session = new ScreenSession(this, appPath, soundPlayer, loggerFactory);
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
        public bool IsFinished => finished.IsSet;
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

        /// <summary>
        /// それまでに UI からゲームスレッドへ渡した処理（描画を含む）が終わるまで待つ。
        /// </summary>
        public void Sync()
        {
            using (var done = new ManualResetEventSlim())
            {
                Session.Game.Post(done.Set);
                if (!done.Wait(Timeout))
                {
                    throw new TimeoutException("timeout waiting for game thread");
                }
            }
        }

        public byte[] SnapshotPng()
        {
            // UI から渡した処理の描画を反映してから撮る（シナリオが終わっていれば待たない）
            if (!IsFinished && !Session.Game.IsStopping)
            {
                using (var done = new ManualResetEventSlim())
                {
                    Session.Game.Post(done.Set);
                    done.Wait(TimeSpan.FromSeconds(2));
                }
            }
            using (var stream = new MemoryStream())
            {
                GUI.SnapshotPng(stream);
                return stream.ToArray();
            }
        }

        #region IScreenHost

        /// <summary>
        /// 表示されたフレームを保存する（検証用）。every フレームごとに directory へ PNG で書き出す。
        /// </summary>
        public void RecordFrames(string directory, int every)
        {
            Directory.CreateDirectory(directory);
            recordDirectory = directory;
            recordEvery = Math.Max(1, every);
        }

        private string recordDirectory;
        private int recordEvery;

        void IScreenHost.Present(SKBitmap frame)
        {
            if (terminated)
            {
                // Terminate 後はシナリオの実行を打ち切る
                throw new HeadlessTerminatedException();
            }
            var count = Interlocked.Increment(ref frameCount);
            if (recordDirectory != null && count % recordEvery == 0)
            {
                using (var image = SKImage.FromBitmap(frame))
                using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
                using (var stream = File.Create(Path.Combine(recordDirectory, $"frame-{count:00000}.png")))
                {
                    data.SaveTo(stream);
                }
            }
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

        /// <summary>文字入力への応答（title, message, defaultValue → 入力値、null でキャンセル）。既定は既定値で確定。</summary>
        public Func<string, string, string, string> InputHandler { get; set; } = (title, message, defaultValue) => defaultValue;

        /// <summary>ファイル選択への応答。既定はキャンセル。</summary>
        public Func<FileDialogRequest, string> OpenFileHandler { get; set; } = request => null;
        public Func<FileDialogRequest, string> SaveFileHandler { get; set; } = request => null;

        public System.Threading.Tasks.Task<string> InputTextAsync(string title, string message, string defaultValue)
        {
            return System.Threading.Tasks.Task.Run(() => InputHandler(title, message, defaultValue));
        }

        public System.Threading.Tasks.Task<string> OpenFileAsync(FileDialogRequest request)
        {
            return System.Threading.Tasks.Task.Run(() => OpenFileHandler(request));
        }

        public System.Threading.Tasks.Task<string> SaveFileAsync(FileDialogRequest request)
        {
            return System.Threading.Tasks.Task.Run(() => SaveFileHandler(request));
        }

        public void Dispose()
        {
            Session.Dispose();
            Session.Game.Join(TimeSpan.FromSeconds(5));
            finished.Dispose();
        }
    }
}
