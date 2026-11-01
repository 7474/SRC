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
        /// <summary>枠の内側の大きさ（VB の ScaleWidth / ScaleHeight に当たる）</summary>
        public virtual Size ClientSize => new Size(Width, Height);
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

        /// <summary>画像を枠の内側の大きさに拡大縮小して表示するか（PictureBoxSizeMode.StretchImage）</summary>
        public bool StretchImage { get; set; }

        /// <summary>立体枠（へこんだ枠）の太さ</summary>
        public const int Border3DPx = 2;

        /// <summary>
        /// へこんだ立体枠を付けるか。元の SRC の PictureBox / Image の BorderStyle = 1（実線）、Appearance = 1（3D）に当たる。
        /// 枠は Width × Height の内側に描き、画像（Image）は枠の内側（ClientSize）に表示する。7474/SRC#1424
        /// </summary>
        public bool Border3D { get; set; }

        /// <summary>枠の太さ</summary>
        public int BorderPx => Border3D ? Border3DPx : 0;

        public override Size ClientSize => new Size(System.Math.Max(0, Width - BorderPx * 2), System.Math.Max(0, Height - BorderPx * 2));

        public PictureBox NewImageIfNull()
        {
            var size = ClientSize;
            if (Image == null || Image.Width != size.Width || Image.Height != size.Height)
            {
                Image = new SKBitmap(size.Width, size.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
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

        /// <summary>
        /// ゲージを描く。元の SRC（GUI.bas の .picHP1.Line (0, 0)-((.picHP1.width - 4) * lu.HP \ lu.MaxHP - 1, 4), , BF）と同じく、
        /// 枠の内側を背景色で塗り、内側の幅に割合を掛けて切り捨てた幅を塗る。
        /// </summary>
        public void DrawBar(float ratio, Brush back, Brush fore)
        {
            NewImageIfNull();
            using (var g = Graphics.FromImage(Image))
            {
                g.FillRectangle(back, 0, 0, Image.Width, Image.Height);
                var width = BarWidth(Image.Width, ratio);
                if (width > 0)
                {
                    g.FillRectangle(fore, 0, 0, width, Image.Height);
                }
            }
        }

        /// <summary>ゲージの塗る幅（整数の割り算と同じく切り捨て）</summary>
        public static int BarWidth(int width, float ratio)
        {
            // float の割合の誤差で、ちょうど割り切れる値が 1 小さくならないようにする
            return System.Math.Max(0, System.Math.Min(width, (int)System.Math.Floor(width * (double)ratio + 1e-4)));
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
