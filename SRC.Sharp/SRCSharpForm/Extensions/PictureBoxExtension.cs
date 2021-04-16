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

        public static void DrawBar(this PictureBox pic, float ratio, Brush back, Brush fore)
        {
            NewImageIfNull(pic);

            using (var g = Graphics.FromImage(pic.Image))
            {
                g.FillRectangle(back, g.VisibleClipBounds);
                g.FillRectangle(fore, 0f, 0f, g.VisibleClipBounds.Width * ratio, g.VisibleClipBounds.Height);
            }
        }
    }
}
