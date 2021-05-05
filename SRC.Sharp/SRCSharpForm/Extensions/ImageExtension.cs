using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCSharpForm.Extensions
{
    public static class ImageExtension
    {
        // https://dobon.net/vb/dotnet/graphics/grayscale.html
        // http://www.graficaobscura.com/matrix/index.html
        private static readonly ColorMatrix monochromeMatrix = new ColorMatrix(new float[][]{
            new float[]{0.3086f, 0.3086f, 0.3086f, 0 ,0},
            new float[]{0.6094f, 0.6094f, 0.6094f, 0, 0},
            new float[]{0.0820f, 0.0820f, 0.0820f, 0, 0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{0, 0, 0, 0, 1}
        });

        public static Image Monotone(this Image image)
        {
            using (var orgImage = new Bitmap(image))
            using (var g = Graphics.FromImage(image))
            using (var ia = new ImageAttributes())
            {
                ia.SetColorMatrix(monochromeMatrix);
                g.Clear(Color.Transparent);
                g.DrawImage(orgImage,
                    new Rectangle(0, 0, image.Width, image.Height),
                    0, 0, image.Width, image.Height,
                    GraphicsUnit.Pixel,
                    ia);
                return image;
            }
        }

        public static Image Rotate(this Image image, float angle, Color background)
        {
            using (var orgImage = new Bitmap(image))
            using (var g = Graphics.FromImage(image))
            {
                // XXX 90度の倍数なら軽量に回転させてもいい
                g.Clear(background);
                g.ResetTransform();
                g.TranslateTransform(image.Width / 2f, image.Height / 2f);
                g.RotateTransform(angle % 360);
                g.DrawImage(orgImage, -image.Width / 2f, -image.Height / 2f);
                g.ResetTransform();
                return image;
            }
        }
    }
}
