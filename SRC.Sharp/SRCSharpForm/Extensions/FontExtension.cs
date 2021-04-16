using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCSharpForm.Extensions
{
    public static class FontExtension
    {
        public static Font ReSize(this Font font, float emSize)
        {
            return new Font(font.FontFamily, emSize, font.Unit);
        }
        public static Font Bold(this Font font)
        {
            return new Font(font, font.Style | FontStyle.Bold);
        }
        public static Font UnBold(this Font font)
        {
            return new Font(font, font.Style & ~FontStyle.Bold);
        }
        public static Font Italic(this Font font)
        {
            return new Font(font, font.Style | FontStyle.Italic);
        }
        public static Font UnItalic(this Font font)
        {
            return new Font(font, font.Style & ~FontStyle.Italic);
        }
    }
}
