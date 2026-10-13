using SkiaSharp;
using System.Drawing;

namespace SRCRender.Compat
{
    /// <summary>
    /// WinForms の Control 相当（位置・大きさ・表示状態）。描画は ScreenRenderer が行う。
    /// </summary>
    public class Control
    {
        public bool Visible { get; set; } = true;
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public Size ClientSize => new Size(Width, Height);
        public Size Size => new Size(Width, Height);
        public Color BackColor { get; set; } = Color.FromArgb(192, 192, 192);
        public Color ForeColor { get; set; } = Color.Black;

        /// <summary>Refresh で呼ばれる処理（画面の再描画）</summary>
        public System.Action Refreshed { get; set; }

        public void Refresh()
        {
            Refreshed?.Invoke();
        }
    }

    /// <summary>
    /// PictureBox 相当。Image に描画した内容を表示する。
    /// </summary>
    public class PictureBox : Control
    {
        public SKBitmap Image { get; set; }

        /// <summary>画像を枠の大きさに拡大縮小して表示するか（PictureBoxSizeMode.StretchImage）</summary>
        public bool StretchImage { get; set; }

        public PictureBox NewImageIfNull()
        {
            if (Image == null || Image.Width != Width || Image.Height != Height)
            {
                Image = new SKBitmap(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul);
                Image.Erase(SKColors.Transparent);
            }
            return this;
        }

        public PictureBox ClearImage(Brush brush)
        {
            NewImageIfNull();
            using (var g = Graphics.FromImage(Image))
            {
                g.FillRectangle(brush, g.VisibleClipBounds);
            }
            return this;
        }

        public void DrawBar(float ratio, Brush back, Brush fore)
        {
            NewImageIfNull();
            using (var g = Graphics.FromImage(Image))
            {
                g.DrawBar(g.VisibleClipBounds, ratio, back, fore);
            }
        }
    }

    /// <summary>
    /// Label / TextBox 相当。
    /// </summary>
    public class Label : Control
    {
        public string Text { get; set; } = "";
        public Font Font { get; set; }
    }
}
