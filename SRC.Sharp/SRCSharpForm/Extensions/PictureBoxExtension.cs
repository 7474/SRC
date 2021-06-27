using System.Drawing;
using System.Windows.Forms;

namespace SRCSharpForm.Extensions
{
    public static class PictureBoxExtension
    {
        public static PictureBox NewImageIfNull(this PictureBox pic)
        {
            if (pic.Image == null)
            {
                pic.Image = new Bitmap(pic.ClientSize.Width, pic.ClientSize.Height);
            }
            return pic;
        }

        public static PictureBox ClearImage(this PictureBox pic, Brush brush)
        {
            NewImageIfNull(pic);
            using var g = Graphics.FromImage(pic.Image);
            g.FillRectangle(brush, g.VisibleClipBounds);
            return pic;
        }

        public static void DrawBar(this PictureBox pic, float ratio, Brush back, Brush fore)
        {
            NewImageIfNull(pic);

            using (var g = Graphics.FromImage(pic.Image))
            {
                g.DrawBar(g.VisibleClipBounds, ratio, back, fore);
            }
        }
    }
}
