using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;
using SRCCore;
using SRCRender;
using System;
using System.Runtime.InteropServices;

namespace SRCSharpAvalonia
{
    /// <summary>
    /// ScreenGUI が描画した画面を表示し、ポインタ・キー入力を ScreenGUI へ渡す。
    /// </summary>
    public class ScreenView : Control
    {
        public const double DisplayScale = 2.0;

        private readonly WriteableBitmap bitmap;
        private readonly object pixelLock = new object();
        private byte[] pendingPixels;

        public ScreenView()
        {
            bitmap = new WriteableBitmap(
                new PixelSize(ScreenLayout.Width, ScreenLayout.Height),
                new Vector(96, 96),
                PixelFormat.Bgra8888,
                AlphaFormat.Premul);
            Width = ScreenLayout.Width * DisplayScale;
            Height = ScreenLayout.Height * DisplayScale;
            Focusable = true;
            ClipToBounds = true;
        }

        public ScreenGUI GUI { get; set; }

        /// <summary>表示中の画面（テスト用）</summary>
        public WriteableBitmap Bitmap => bitmap;

        /// <summary>
        /// 描画済みの画面を受け取る。任意のスレッドから呼べる。
        /// </summary>
        public void SetFrame(SKBitmap frame)
        {
            var pixels = new byte[frame.ByteCount];
            Marshal.Copy(frame.GetPixels(), pixels, 0, pixels.Length);
            lock (pixelLock)
            {
                pendingPixels = pixels;
            }
        }

        /// <summary>
        /// 受け取った画面を表示へ反映する。UI スレッドで呼ぶ。
        /// </summary>
        public void Flush()
        {
            byte[] pixels;
            lock (pixelLock)
            {
                pixels = pendingPixels;
                pendingPixels = null;
            }
            if (pixels == null)
            {
                return;
            }
            using (var fb = bitmap.Lock())
            {
                var length = Math.Min(pixels.Length, fb.RowBytes * fb.Size.Height);
                Marshal.Copy(pixels, 0, fb.Address, length);
            }
            InvalidateVisual();
        }

        public override void Render(DrawingContext context)
        {
            var scaling = TopLevel.GetTopLevel(this)?.RenderScaling ?? 1.0;
            using (context.PushRenderOptions(new RenderOptions { BitmapInterpolationMode = InterpolationModeFor(scaling) }))
            {
                context.DrawImage(bitmap, new Rect(Bounds.Size));
            }
        }

        /// <summary>
        /// 画面の拡大の方法。実際の画素で 1 倍以上に拡大して表示するときは補間せず、
        /// 縮小するときは補間する（補間しないと文字の線が抜ける）。7474/SRC#1148
        /// </summary>
        /// <param name="renderScaling">OS の表示倍率（TopLevel.RenderScaling）</param>
        public static BitmapInterpolationMode InterpolationModeFor(double renderScaling)
        {
            return renderScaling * DisplayScale >= 1.0 ? BitmapInterpolationMode.None : BitmapInterpolationMode.HighQuality;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);
            Focus();
            var point = e.GetCurrentPoint(this);
            GUI?.PointerPressed((float)(point.Position.X / DisplayScale), (float)(point.Position.Y / DisplayScale),
                point.Properties.IsRightButtonPressed ? GuiButton.Right : GuiButton.Left, e.ClickCount);
        }

        protected override void OnPointerReleased(PointerReleasedEventArgs e)
        {
            base.OnPointerReleased(e);
            var point = e.GetCurrentPoint(this);
            GUI?.PointerReleased((float)(point.Position.X / DisplayScale), (float)(point.Position.Y / DisplayScale),
                e.InitialPressMouseButton == MouseButton.Right ? GuiButton.Right : GuiButton.Left);
        }

        protected override void OnPointerMoved(PointerEventArgs e)
        {
            base.OnPointerMoved(e);
            var point = e.GetPosition(this);
            GUI?.PointerMoved((float)(point.X / DisplayScale), (float)(point.Y / DisplayScale));
        }

        protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
        {
            base.OnPointerWheelChanged(e);
            var point = e.GetPosition(this);
            var delta = Math.Sign(e.Delta.Y);
            if (delta != 0)
            {
                GUI?.PointerWheel((float)(point.X / DisplayScale), (float)(point.Y / DisplayScale), delta);
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);
            var vk = VirtualKeys.FromKey(e.Key);
            if (vk != 0)
            {
                GUI?.KeyStateChanged(vk, false);
            }
        }

        protected override void OnLostFocus(FocusChangedEventArgs e)
        {
            base.OnLostFocus(e);
            GUI?.ReleaseAllKeys();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            var vk = VirtualKeys.FromKey(e.Key);
            if (vk != 0)
            {
                GUI?.KeyStateChanged(vk, true);
            }
            ScreenKey? key = e.Key switch
            {
                Key.Enter => ScreenKey.Enter,
                Key.Space => ScreenKey.Enter,
                Key.Escape => ScreenKey.Escape,
                Key.Back => ScreenKey.Escape,
                Key.Delete => ScreenKey.Escape,
                Key.Left => ScreenKey.Left,
                Key.Right => ScreenKey.Right,
                Key.Up => ScreenKey.Up,
                Key.Down => ScreenKey.Down,
                _ => null,
            };
            if (key.HasValue)
            {
                GUI?.KeyPressed(key.Value);
                e.Handled = true;
            }
        }
    }
}
