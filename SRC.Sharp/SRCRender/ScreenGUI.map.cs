using SRCCore;
using SRCCore.Pilots;
using SRCCore.Units;
using System.Collections.Generic;
using System.Drawing;

namespace SRCRender
{
    public partial class ScreenGUI
    {
        #region IGUIMap

        public void InitMapSize(int w, int h)
        {
        }

        public void SetMapSize(int w, int h)
        {
            background?.Dispose();
            background = null;
        }

        #endregion

        public void ClearScrean()
        {
            State.ShowMap = false;
            Present();
            ScreenIsSaved = true;
        }

        public void SetupBackground(string draw_mode = "", string draw_option = "", Color filter_color = default, double filter_trans_par = 0d)
        {
            Map.MapDrawMode = draw_mode;
            Map.MapDrawFilterColor = filter_color;
            Map.MapDrawFilterTransPercent = filter_trans_par;
            Map.IsMapDirty = false;
            IsPictureVisible = false;
            IsCursorVisible = false;

            background?.Dispose();
            background = renderer.BuildBackground(src, Images);
            if (draw_option != "ステータス")
            {
                MapX = (MainWidth / 2 + 1);
                MapY = (MainHeight / 2 + 1);
            }
            if (!Map.IsStatusView && string.IsNullOrEmpty(draw_option))
            {
                RefreshScreen();
            }
        }

        public void RedrawScreen(bool late_refresh = false)
        {
            ScreenIsMasked = false;
            RefreshScreen(false, late_refresh);
        }

        public void MaskScreen()
        {
            ScreenIsMasked = true;
            RefreshScreen();
        }

        public void RefreshScreen(bool without_refresh = false, bool delay_refresh = false)
        {
            if (background == null && Map?.MapData != null && Map.MapWidth > 0)
            {
                background = renderer.BuildBackground(src, Images);
            }
            State.ShowMap = true;
            if (!without_refresh)
            {
                IsPictureVisible = false;
                IsCursorVisible = false;
            }
            Present();
            ScreenIsSaved = false;
        }

        public void TransionScrean(TransionPattern pattern, Color fillColor, int frame, int frameMillis)
        {
            // TODO トランジション効果
            Sleep(frame * frameMillis);
        }

        public void Center(int new_x, int new_y)
        {
            if (Map.IsStatusView)
            {
                return;
            }
            MapX = System.Math.Max(1, System.Math.Min(Map.MapWidth, new_x));
            MapY = System.Math.Max(1, System.Math.Min(Map.MapHeight, new_y));
        }

        public int MapToPixelX(int X)
        {
            return ScreenLayout.CellPx * ((MainWidth + 1) / 2 - 1 - (MapX - X));
        }

        public int MapToPixelY(int Y)
        {
            return ScreenLayout.CellPx * ((MainHeight + 1) / 2 - 1 - (MapY - Y));
        }

        public int PixelToMapX(int X)
        {
            X = System.Math.Max(0, System.Math.Min(MainPWidth - 1, X));
            return X / ScreenLayout.CellPx + 1 + MapX - (MainWidth + 1) / 2;
        }

        public int PixelToMapY(int Y)
        {
            Y = System.Math.Max(0, System.Math.Min(MainPHeight - 1, Y));
            return Y / ScreenLayout.CellPx + 1 + MapY - (MainHeight + 1) / 2;
        }

        public void PaintUnitBitmap(Unit u, string smode = "")
        {
            Present();
        }

        public void EraseUnitBitmap(int X, int Y, bool do_refresh = true)
        {
            if (do_refresh)
            {
                Present();
            }
        }

        public void MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division = 2)
        {
            // TODO 移動アニメーション
            Present();
        }

        public void MoveUnitBitmap2(Unit u, int wait_time0, int division = 2)
        {
            Present();
        }

        public void DieAnimation(Unit u)
        {
            Present();
        }

        public void ExplodeAnimation(string tsize, int tx, int ty)
        {
            Present();
        }

        public void SaveCursorPos()
        {
        }

        public void MoveCursorPos(string cursor_mode, Unit t = null)
        {
        }

        public void RestoreCursorPos()
        {
        }

        #region 画像・文字列描画（Paint 系コマンド）

        // TODO 画像・文字列・図形の描画コマンド。現状は描画しない

        public PaintFont CurrentPaintFont { get; private set; } = new PaintFont { Name = "ＭＳ Ｐ明朝", Size = 16, Bold = true };
        public Color CurrentPaintColor { get; private set; } = Color.White;

        public bool DrawPicture(string fname, int dx, int dy, int dw, int dh, int sx, int sy, int sw, int sh, string draw_option)
        {
            return true;
        }

        public void DrawString(string msg, int X, int Y, bool without_cr = false)
        {
        }

        public void DrawSysString(int X, int Y, string msg, bool without_refresh = false)
        {
        }

        public SizeF MeasureString(string msg)
        {
            var size = renderer.MeasureText(msg);
            return new SizeF(size.Width, size.Height);
        }

        public void ResetDrawString()
        {
            CurrentPaintFont = new PaintFont { Name = "ＭＳ Ｐ明朝", Size = 16, Bold = true };
            CurrentPaintColor = Color.White;
        }

        public void SetDrawString(DrawStringMode mode)
        {
        }

        public void SetDrawFont(DrawFontOption option)
        {
            CurrentPaintFont = new PaintFont
            {
                Name = option.FontFamily,
                Size = option.Size,
                Bold = option.Bold,
                Italic = option.Italic,
            };
            CurrentPaintColor = option.Color;
        }

        public void UpdateBaseX(int newX)
        {
        }

        public void UpdateBaseY(int newY)
        {
        }

        public void SaveScreen()
        {
            ScreenIsSaved = true;
        }

        public void ClearPicture()
        {
        }

        public void ClearPicture2(int x1, int y1, int x2, int y2)
        {
        }

        public void ArcCmd(ScreanDrawOption option, int x1, int y1, int rad, float start_angle, float end_angle)
        {
        }

        public void CircleCmd(ScreanDrawOption option, int x1, int y1, int rad)
        {
        }

        public void OvalCmd(ScreanDrawOption option, int x1, int y1, int rad, float oval_ratio)
        {
        }

        public void LineCmd(ScreanDrawOption option, int x1, int y1, int x2, int y2)
        {
        }

        public void BoxCmd(ScreanDrawOption option, int x1, int y1, int x2, int y2)
        {
        }

        public void PSetCmd(ScreanDrawOption option, int x1, int y1)
        {
        }

        public void PolygonCmd(ScreanDrawOption option, Point[] points)
        {
        }

        #endregion

        #region IGUIStatus

        public Unit DisplayedUnit { get; set; }
        public Pilot DisplayedPilot { get; set; }

        public void DisplayGlobalStatus()
        {
            State.StatusLines = new List<string>
            {
                "ターン " + src.Turn,
                "資金 " + src.Money,
            };
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
            State.StatusLines = lines;
            Present();
        }

        public void DisplayPilotStatus(Pilot p)
        {
            DisplayedPilot = p;
            State.StatusLines = new List<string>
            {
                p.get_Nickname(false),
                "レベル " + p.Level,
                "気力 " + p.Morale,
            };
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
            State.StatusLines = new List<string>();
            Present();
        }

        #endregion
    }
}
