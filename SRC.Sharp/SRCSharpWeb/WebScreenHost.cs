using SkiaSharp;
using SRCRender;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace SRCSharpWeb
{
    /// <summary>
    /// ScreenGUI の画面を canvas へ渡す。
    /// Present はゲームスレッドから呼ばれるため、画素を RGBA に変換して預けておき、
    /// ブラウザの描画のたび（requestAnimationFrame）に UI スレッドで canvas へ書き込む。
    /// 文字入力・ファイル選択はページ上のモーダルで行う（JS を呼べるのは UI 側のスレッドだけのため、描画と同じ機会に始める）。
    /// </summary>
    public sealed class WebScreenHost : IScreenHost, IScreenDialogHost
    {
        public WebScreenHost(string scenarioRoot)
        {
            ScenarioRoot = scenarioRoot;
        }

        /// <summary>シナリオを展開したフォルダ。ファイル選択はこの中に限る</summary>
        public string ScenarioRoot { get; }

        private readonly ConcurrentQueue<Action> uiActions = new ConcurrentQueue<Action>();

        /// <summary>UI 側のスレッドで行う処理を頼む（JS を呼ぶ処理）。任意のスレッドから呼べる</summary>
        public void Post(Action action)
        {
            uiActions.Enqueue(action);
        }

        /// <summary>描画の機会ごとに UI 側のスレッドで呼ばれる（音声の送り出しなど）</summary>
        public event Action Flushed;
        private static readonly SKImageInfo FrameInfo
            = new SKImageInfo(ScreenLayout.Width, ScreenLayout.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul);

        // UI スレッドを止めないよう、ロックを使わずに受け渡す
        private byte[] pending;
        private byte[] spare;
        private string title;

        /// <summary>シナリオが終了した</summary>
        public bool Terminated { get; private set; }

        /// <summary>受け取った画面の数・canvas へ書き込んだ数（動作確認用）</summary>
        public int PresentCount => presentCount;
        public int FlushCount => flushCount;
        private int presentCount;
        private int flushCount;

        public void Present(SKBitmap frame)
        {
            Interlocked.Increment(ref presentCount);
            var buffer = Interlocked.Exchange(ref spare, null) ?? new byte[FrameInfo.BytesSize];
            var handle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                using (var pixmap = frame.PeekPixels())
                {
                    pixmap.ReadPixels(FrameInfo, handle.AddrOfPinnedObject(), FrameInfo.RowBytes);
                }
            }
            finally
            {
                handle.Free();
            }
            var old = Interlocked.Exchange(ref pending, buffer);
            if (old != null)
            {
                Interlocked.CompareExchange(ref spare, old, null);
            }
        }

        public void SetTitle(string title)
        {
            Volatile.Write(ref this.title, string.IsNullOrEmpty(title) ? WebInterop.AppTitle : title + " - " + WebInterop.AppTitle);
        }

        public bool Terminate()
        {
            Terminated = true;
            // Environment.Exit は呼ばせない
            return false;
        }

        /// <summary>UI スレッドで呼ぶ。新しい画面があれば canvas へ書き込む。</summary>
        public void Flush()
        {
            while (uiActions.TryDequeue(out var action))
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                }
            }
            try
            {
                Flushed?.Invoke();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
            }
            var t = Interlocked.Exchange(ref title, null);
            if (t != null)
            {
                WebInterop.SetDocumentTitle(t);
            }
            var frame = Interlocked.Exchange(ref pending, null);
            if (frame == null)
            {
                return;
            }
            WebInterop.DrawFrame(new ArraySegment<byte>(frame), FrameInfo.Width, FrameInfo.Height);
            Interlocked.CompareExchange(ref spare, frame, null);
            flushCount++;
        }
    
        private Task<string> OnUi(Func<Task<string>> show)
        {
            var result = new TaskCompletionSource<string>(TaskCreationOptions.RunContinuationsAsynchronously);
            uiActions.Enqueue(() =>
            {
                try
                {
                    show().ContinueWith(t => result.TrySetResult(t.IsCompletedSuccessfully ? t.Result : null));
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine(ex);
                    result.TrySetResult(null);
                }
            });
            return result.Task;
        }

        public Task<string> InputTextAsync(string title, string message, string defaultValue)
        {
            return OnUi(() => WebInterop.ShowInput(title ?? "", message ?? "", defaultValue ?? ""));
        }

        private string Directory(FileDialogRequest request)
        {
            var dir = request.InitialDirectory;
            var root = Path.GetFullPath(ScenarioRoot);
            return !string.IsNullOrEmpty(dir) && Path.GetFullPath(dir).StartsWith(root, StringComparison.Ordinal) && System.IO.Directory.Exists(dir)
                ? dir
                : root;
        }

        public Task<string> OpenFileAsync(FileDialogRequest request)
        {
            var dir = Directory(request);
            var pattern = string.IsNullOrEmpty(request.FileExtension) ? "*" : "*." + request.FileExtension;
            var names = System.IO.Directory.GetFiles(dir, pattern)
                .Select(Path.GetFileName)
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToArray();
            if (names.Length == 0)
            {
                return Task.FromResult<string>(null);
            }
            return OnUi(() => WebInterop.ShowChoice(request.Title ?? "ファイルを開く", names))
                .ContinueWith(t => t.Result == null ? null : Path.Combine(dir, t.Result), TaskScheduler.Default);
        }

        /// <summary>保存のダイアログでフォルダが選ばれた（セーブデータの同期の対象にする）</summary>
        public Action<string> SaveDirectoryChosen { get; set; }

        public Task<string> SaveFileAsync(FileDialogRequest request)
        {
            var dir = Directory(request);
            var ext = string.IsNullOrEmpty(request.FileExtension) ? "" : "." + request.FileExtension;
            var name = string.IsNullOrEmpty(request.InitialFile) ? "_save" + ext : Path.GetFileName(request.InitialFile);
            return OnUi(() => WebInterop.ShowInput(request.Title ?? "保存", "保存するファイル名", name))
                .ContinueWith(t =>
                {
                    var file = Path.GetFileName(t.Result ?? "");
                    if (string.IsNullOrWhiteSpace(file))
                    {
                        return null;
                    }
                    if (ext.Length > 0 && !file.EndsWith(ext, StringComparison.OrdinalIgnoreCase))
                    {
                        file += ext;
                    }
                    SaveDirectoryChosen?.Invoke(dir);
                    return Path.Combine(dir, file);
                }, TaskScheduler.Default);
        }
    }
}
