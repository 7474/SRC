using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System.Drawing;

namespace SRCSharpForm.Tests
{
    [TestClass()]
    public class SRCSharpFormGUITests
    {
        [TestMethod()]
        public void MessageLenTest()
        {
            int width = 1000;
            string buf = "  test.";
            Graphics g = Graphics.FromImage(new Bitmap(1024, 768));
            Font font = new Font(FontFamily.GenericSerif, 12f);


            var nonStyle = g.MeasureString(buf, font);
            var gDefault = g.MeasureString(buf, font, width, StringFormat.GenericDefault);
            var gTypographic = g.MeasureString(buf, font, width, StringFormat.GenericTypographic);
            var gMyFormat = g.MeasureString(buf, font, width, new StringFormat(
                    StringFormatFlags.NoClip | StringFormatFlags.FitBlackBox | StringFormatFlags.LineLimit
                )
            {
                Trimming = StringTrimming.None,
            });
            System.Console.WriteLine(JsonConvert.SerializeObject(nonStyle));
            System.Console.WriteLine(JsonConvert.SerializeObject(gDefault));
            System.Console.WriteLine(JsonConvert.SerializeObject(gTypographic));
            System.Console.WriteLine(JsonConvert.SerializeObject(gMyFormat));
        }
    }
}