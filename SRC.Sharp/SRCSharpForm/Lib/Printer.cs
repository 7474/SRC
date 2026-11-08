using SRCSharpForm.Extensions;
using System.Drawing;

namespace SRCSharpForm.Lib
{
    public class Printer
    {
        private Graphics g;
        private Font currentFont;
        private Brush currentBrush;
        private Color currentColor;
        private PointF currentPoint;
        private float gridCellWidth;

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

        public float CellWidth => gridCellWidth;

        /// <summary>文字列を描いたときに、その位置（左上）・文字列・色を知らせる（テストで表示を確かめるため）</summary>
        public System.Action<float, float, string, Color> Printed { get; set; }
        public Graphics Graphics => g;

        public Printer(Graphics g, Font font, Color color, float gridCellWidth)
        {
            this.g = g;
            this.g.TextRenderingHint = RenderingConfig.TextHint;
            currentPoint.X = 0;
            currentPoint.Y = 0;
            Configure(font, color);
            this.gridCellWidth = gridCellWidth;
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
            currentColor = color;
            currentBrush = new SolidBrush(color);
        }

        public void SetGridCellWidth(float gridCellWidth)
        {
            this.gridCellWidth = gridCellWidth;
        }

        public void PushGrid()
        {
            // XXX 3カラム以上対応
            if (CurrentX <= gridCellWidth)
            {
                CurrentX = gridCellWidth;
            }
            else
            {
                Print();
            }
        }

        public void ClearGrid()
        {
            if (CurrentX == 0)
            {
                // Ignore
            }
            else
            {
                Print();
            }
        }

        public void Print()
        {
            currentPoint.X = 0;
            currentPoint.Y += currentFont.GetHeight(g);
        }

        public void Print(string str)
        {
            var textSize = g.MeasureStringWithoutRightMargin(str, currentFont);
            // 元の SRC（PictureBox.Print）は右端で折り返さず、はみ出した分は見えないだけ
            g.DrawStringWithoutMargin(str, currentFont, currentBrush, currentPoint);
            Printed?.Invoke(currentPoint.X, currentPoint.Y, str, currentColor);
            currentPoint.X += textSize.Width;
        }
    }
}
