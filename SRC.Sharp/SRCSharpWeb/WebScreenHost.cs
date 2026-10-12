using SkiaSharp;
using SRCRender;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace SRCSharpWeb
{
    /// <summary>
    /// ScreenGUI の画面を canvas へ渡す。
    /// Present はゲームスレッドから呼ばれるため、画素を RGBA に変換して預けておき、
    /// ブラウザの描画のたび（requestAnimationFrame）に UI スレッドで canvas へ書き込む。
    /// </summary>
    public sealed class WebScreenHost : IScreenHost
    {
        private static readonly SKImageInfo FrameInfo
            = new SKImageInfo(ScreenLayout.Width, ScreenLayout.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul);

        // UI スレッドを止めないよう、ロックを使わずに受け渡す
        private byte[] pending;
        private byte[] spare;
        private string title;

        /// <summary>シナリオが終了した</summary>
        public bool Terminated { get; private set; }

        public void Present(SKBitmap frame)
        {
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
        }
    }
}
