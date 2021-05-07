using System.Drawing;

namespace SRCSharpForm.Lib
{
    public class Printer
    {
        private Graphics g;
        private Font currentFont;
        private Brush currentBrush;
        private PointF currentPoint;

        public float CurrentX
        {
            get => currentPoint.X;
            set => currentPoint.X = value;
        }
        public float CurrentY
        {
            get => currentPoint.Y;
            set => currentPoint.Y = value;
        }

        public Printer(Graphics g, Font font, Color color)
        {
            this.g = g;
            currentPoint.X = 0;
            currentPoint.Y = 0;
            Configure(font, color);
        }

        public void Configure(Font font, Color color)
        {
            SetFont(font);
            SetColor(color);
        }

        public void SetFont(Font font)
        {
            currentFont = font;
        }

        public void SetColor(Color color)
        {
            currentBrush = new SolidBrush(color);
        }

        public void Print()
        {
            currentPoint.X = 0;
            currentPoint.Y += currentFont.GetHeight(g);
        }

        public void Print(string str)
        {
            g.DrawString(str, currentFont, currentBrush, currentPoint);
            currentPoint.X += g.MeasureString(str, currentFont).Width;
        }
    }
}
