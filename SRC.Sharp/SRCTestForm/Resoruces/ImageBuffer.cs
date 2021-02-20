using SRCCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCTestForm.Resoruces
{
    // TODO LRUキャッシュ
    public class ImageBuffer
    {
        private IDictionary<string, Image> buffer = new Dictionary<string, Image>();
        private SRC SRC;

        public ImageBuffer(SRC src)
        {
            SRC = src;
        }

        public bool Load(string type, string name)
        {
            var path = SearchFile(type, name);
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }
            var image = NormalizeImage(path, Image.FromFile(path));
            buffer[ToKey(type, name)] = image;
            return true;
        }

        private Image NormalizeImage(string path, Image image)
        {
            // bmpなら真っ白を抜く
            //if (path.ToLower().EndsWith("bmp"))
            if (!image.PixelFormat.HasFlag(PixelFormat.Alpha))
            {
                var bitmap = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(bitmap))
                {
                    g.DrawImage(image, 0, 0);
                }
                // XXX 色を入れ替えとかできそうな気はする
                var white = Color.White.ToArgb();
                for (var x = 0; x < bitmap.Width; x++)
                {
                    for (var y = 0; y < bitmap.Width; y++)
                    {
                        if (bitmap.GetPixel(x, y).ToArgb() == white)
                        {
                            bitmap.SetPixel(x, y, Color.Transparent);
                        }
                    }
                }
                return bitmap;
            }

            return image;
        }

        public Image Get(string type, string name)
        {
            var key = ToKey(type, name);
            if (buffer.ContainsKey(key))
            {
                return buffer[key];
            }
            if (Load(type, name))
            {
                return buffer[key];
            }
            else
            {
                return null;
            }
        }

        public string ToKey(string type, string name)
        {
            return Path.Combine(type, name);
        }

        public string SearchFile(string type, string name)
        {
            return ImageFilders()
                .Select(x => Path.Combine(x, type, name))
                .FirstOrDefault(x => File.Exists(x));
        }

        private IEnumerable<string> ImageFilders()
        {
            return new string[]
            {
                SRC.ScenarioPath,
                SRC.ExtDataPath,
                SRC.ExtDataPath2,
                SRC.AppPath,
            }.Select(x => Path.Combine(x, "Bitmap"));
        }
    }
}
