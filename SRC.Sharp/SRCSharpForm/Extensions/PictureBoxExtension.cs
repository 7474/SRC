using System.Drawing;
using System.Windows.Forms;

namespace SRCSharpForm.Extensions
{
    public static class PictureBoxExtension
    {
        public static void NewImageIfNull(this PictureBox pic)
        {
            if (pic.Image == null)
            {
                pic.Image = new Bitmap(pic.ClientSize.Width, pic.ClientSize.Height);
            }
        }
    }
}
