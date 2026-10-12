using SkiaSharp;
using SRCCore;
using SRCCore.Pilots;
using SRCCore.Units;
using System.Collections.Generic;

namespace SRCRender
{
    // ステータスウィンドウ。描画先は statusBuffer（ScreenLayout.StatusArea の大きさ）。
    public partial class ScreenGUI
    {
        private readonly SKBitmap statusBuffer = NewTransparentBuffer(ScreenLayout.StatusArea.Width, ScreenLayout.StatusArea.Height);
        private readonly SKBitmap faceBuffer = NewTransparentBuffer(ScreenLayout.FaceSize, ScreenLayout.FaceSize);

        private static SKBitmap NewTransparentBuffer(int width, int height)
        {
            var bitmap = new SKBitmap(width, height, SKColorType.Bgra8888, SKAlphaType.Premul);
            bitmap.Erase(SKColors.Transparent);
            return bitmap;
        }

        public Unit DisplayedUnit { get; set; }
        public Pilot DisplayedPilot { get; set; }

        private void DrawStatusLines(IList<string> lines)
        {
            statusBuffer.Erase(SKColors.Transparent);
            using (var canvas = new SKCanvas(statusBuffer))
            using (var font = CreateFont(new PaintFont { Name = "Noto Sans CJK JP", Size = 11 }))
            using (var paint = new SKPaint { Color = SKColors.White, IsAntialias = true })
            {
                var y = 8f;
                foreach (var line in lines)
                {
                    DrawTextTop(canvas, line ?? "", 12, y, font, paint);
                    y += font.Spacing + 2;
                }
            }
        }

        public void DisplayGlobalStatus()
        {
            DisplayedUnit = null;
            DisplayedPilot = null;
            DrawStatusLines(new List<string>
            {
                "ターン " + src.Turn,
                "資金 " + src.Money,
            });
            Present();
        }

        public void DisplayUnitStatus(Unit u, Pilot p = null)
        {
            DisplayedUnit = u;
            DisplayedPilot = p;
            var lines = new List<string>();
            try
            {
                lines.Add(u.Nickname0);
                lines.Add("HP " + u.HP + "/" + u.MaxHP);
                lines.Add("EN " + u.EN + "/" + u.MaxEN);
                var pilot = p ?? u.MainPilot();
                if (pilot != null)
                {
                    lines.Add("");
                    lines.Add(pilot.get_Nickname(false));
                    lines.Add("レベル " + pilot.Level);
                    lines.Add("気力 " + pilot.Morale);
                }
            }
            catch
            {
                // データ不備時は取得できた分だけ表示する
            }
            DrawStatusLines(lines);
            Present();
        }

        public void DisplayPilotStatus(Pilot p)
        {
            DisplayedPilot = p;
            DrawStatusLines(new List<string>
            {
                p.get_Nickname(false),
                "レベル " + p.Level,
                "気力 " + p.Morale,
            });
            Present();
        }

        public void InstantUnitStatusDisplay(int X, int Y)
        {
            var u = Map.UnitAtPoint(X, Y);
            if (u != null)
            {
                DisplayUnitStatus(u);
            }
        }

        public void ClearUnitStatus()
        {
            DisplayedUnit = null;
            DisplayedPilot = null;
            statusBuffer.Erase(SKColors.Transparent);
            Present();
        }
    }
}
