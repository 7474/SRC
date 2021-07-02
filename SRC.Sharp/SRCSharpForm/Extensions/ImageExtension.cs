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
        // XXX 夜って意味合いだと単純に各色50%より青系強めにしたほうがいい気はする
        private static readonly ColorMatrix darkMatrix = new ColorMatrix(new float[][]{
            new float[]{0.5f, 0, 0, 0 ,0},
            new float[]{0, 0.5f, 0, 0 ,0},
            new float[]{0, 0, 0.5f, 0 ,0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{0, 0, 0, 0, 1}
        });
        private static readonly ColorMatrix sepiaMatrix = new ColorMatrix(new float[][]{
            new float[]{ 0.299f * 1.1f, 0.299f * 0.9f, 0.299f * 0.7f, 0 ,0},
            new float[]{ 0.587f * 1.1f, 0.587f * 0.9f, 0.587f * 0.7f, 0 ,0},
            new float[]{ 0.114f * 1.1f, 0.114f * 0.9f, 0.114f * 0.7f, 0 ,0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{0, 0, 0, 0, 1}
        });
        // https://dobon.net/vb/dotnet/graphics/grayscale.html
        // http://www.graficaobscura.com/matrix/index.html
        private static readonly ColorMatrix monochromeMatrix = new ColorMatrix(new float[][]{
            new float[]{0.3086f, 0.3086f, 0.3086f, 0 ,0},
            new float[]{0.6094f, 0.6094f, 0.6094f, 0, 0},
            new float[]{0.0820f, 0.0820f, 0.0820f, 0, 0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{0, 0, 0, 0, 1}
        });
        // XXX 色味未調整
        private static readonly ColorMatrix waterMatrix = new ColorMatrix(new float[][]{
            new float[]{ 0.299f * 0.6f, 0.299f * 0.8f, 0.299f, 0 ,0},
            new float[]{ 0.587f * 0.6f, 0.587f * 0.8f, 0.587f, 0 ,0},
            new float[]{ 0.114f * 0.6f, 0.114f * 0.8f, 0.114f, 0 ,0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{0, 0, 0, 0, 1}
        });
        // XXX 色味未調整
        private static readonly ColorMatrix sunsetMatrix = new ColorMatrix(new float[][]{
            new float[]{ 0.299f * 1.3f + 0.2f, 0.299f * 0.4f, 0.299f * 0.2f, 0 ,0},
            new float[]{ 0.587f * 1.3f, 0.587f * 0.4f + 0.2f, 0.587f * 0.2f, 0 ,0},
            new float[]{ 0.114f * 1.3f, 0.114f * 0.4f, 0.114f * 0.2f + 0.2f, 0 ,0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{0, 0, 0, 0, 1}
        });
        // 真っ白以外が真っ黒になるようにする
        private static readonly ColorMatrix silhouetteMatrix = new ColorMatrix(new float[][]{
            new float[]{255, 255, 255, 0, 0},
            new float[]{255, 255, 255, 0, 0},
            new float[]{255, 255, 255, 0, 0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{-764, -764, -764, 0, 1}
        });
        private static readonly ColorMatrix negPosReverseMatrix = new ColorMatrix(new float[][]{
            new float[]{-1, 0, 0, 0, 0},
            new float[]{0, -1, 0, 0, 0},
            new float[]{0, 0, -1, 0, 0},
            new float[]{0, 0, 0, 1, 0},
            new float[]{1, 1, 1, 0, 1}
        });

        public static Image ApplyColorMatrix(this Image image, ColorMatrix matrix)
        {
            using (var orgImage = new Bitmap(image))
            using (var g = Graphics.FromImage(image))
            using (var ia = new ImageAttributes())
            {
                ia.SetColorMatrix(matrix);
                g.Clear(Color.Transparent);
                g.DrawImage(orgImage,
                    new Rectangle(0, 0, image.Width, image.Height),
                    0, 0, image.Width, image.Height,
                    GraphicsUnit.Pixel,
                    ia);
                return image;
            }
        }

        public static Image Dark(this Image image)
        {
            return image.ApplyColorMatrix(darkMatrix);
        }

        public static Image Sepia(this Image image)
        {
            return image.ApplyColorMatrix(sepiaMatrix);
        }

        public static Image Monotone(this Image image)
        {
            return image.ApplyColorMatrix(monochromeMatrix);
        }

        public static Image Sunset(this Image image)
        {
            return image.ApplyColorMatrix(sunsetMatrix);
        }

        public static Image Water(this Image image)
        {
            return image.ApplyColorMatrix(waterMatrix);
        }

        public static Image Silhouette(this Image image)
        {
            return image.ApplyColorMatrix(silhouetteMatrix);
        }

        public static Image NegPosReverse(this Image image)
        {
            return image.ApplyColorMatrix(negPosReverseMatrix);
        }

        public static Image ColorFilter(this Image image, Color color, float alpha)
        {
            return image.ApplyColorMatrix(new ColorMatrix(new float[][]{
                new float[]{1f - alpha, 0, 0, 0, 0},
                new float[]{0, 1f - alpha, 0, 0, 0},
                new float[]{0, 0, 1f - alpha, 0, 0},
                new float[]{0, 0, 0, 1, 0},
                new float[]{color.R * alpha / 255f, color.G * alpha / 255f, color.B * alpha / 255f, 0, 1}
            }));
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

        public static Image RemoveAlpha(this Image image, Color background)
        {
            using (var orgImage = new Bitmap(image))
            using (var g = Graphics.FromImage(image))
            {
                g.Clear(background);
                g.DrawImage(orgImage, 0, 0);
                return image;
            }
        }
    }
}
