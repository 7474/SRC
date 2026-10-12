using SRCCore;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCRender
{
    // 撃墜・爆発アニメーション。WinForms 版（SRCSharpFormGUI.cs）の処理を、描画コマンドの上に移植したもの。
    public partial class ScreenGUI
    {
        public void DieAnimation(Unit u)
        {
            EraseUnitBitmap(u.x, u.y, false);
            if (!u.IsHero())
            {
                ExplodeAnimation(u.Size, u.x, u.y);
                return;
            }
            if (IsRButtonPressed())
            {
                return;
            }

            switch (u.Area ?? "")
            {
                case "地上":
                    src.Sound.PlayWave("FallDown.wav");
                    break;
                case "空中":
                    if (MessageWait > 0)
                    {
                        src.Sound.PlayWave("Bomb.wav");
                        Sleep(500);
                    }
                    var terrainClass = Map.Terrain(u.x, u.y).Class;
                    src.Sound.PlayWave(terrainClass == "水" || terrainClass == "深海" ? "Splash.wav" : "FallDown.wav");
                    break;
            }
            if (MessageWait == 0)
            {
                return;
            }

            string fname;
            switch (u.Party0 ?? "")
            {
                case "味方":
                case "ＮＰＣ":
                    fname = @"Bitmap\Anime\Common\EFFECT_Tile(Ally)";
                    break;
                case "敵":
                    fname = @"Bitmap\Anime\Common\EFFECT_Tile(Enemy)";
                    break;
                case "中立":
                    fname = @"Bitmap\Anime\Common\EFFECT_Tile(Neutral)";
                    break;
                default:
                    return;
            }
            fname = src.FileSystem.FileExists(src.FileSystem.PathCombine(src.ScenarioPath, fname + ".bmp"))
                ? src.FileSystem.PathCombine(src.ScenarioPath, fname)
                : src.FileSystem.PathCombine(src.AppPath, fname);
            if (!src.FileSystem.FileExists(fname + "01.bmp"))
            {
                return;
            }

            var drawMode = Map.MapDrawMode == "夜" ? "暗" : Map.MapDrawMode;
            for (var i = 1; i <= 6; i++)
            {
                DrawPicture(fname + ".bmp", MapToPixelX(u.x), MapToPixelY(u.y), 32, 32, 0, 0, 0, 0, drawMode);
                DrawPicture(@"Unit\" + u.get_Bitmap(false), MapToPixelX(u.x), MapToPixelY(u.y), 32, 32, 0, 0, 0, 0, "透過 " + drawMode);
                DrawPicture(fname + "0" + SrcFormatter.Format(i) + ".bmp", MapToPixelX(u.x), MapToPixelY(u.y), 32, 32, 0, 0, 0, 0, "透過 " + drawMode);
                UpdateScreen();
                Sleep(50);
            }
            ClearPicture();
            UpdateScreen();
        }

        public void ExplodeAnimation(string tsize, int tx, int ty)
        {
            if (IsRButtonPressed())
            {
                return;
            }
            switch (tsize ?? "")
            {
                case "XL":
                case "LL":
                    src.Sound.PlayWave("Explode(Far).wav");
                    break;
                case "L":
                case "M":
                case "S":
                case "SS":
                    src.Sound.PlayWave("Explode.wav");
                    break;
            }
            if (MessageWait == 0)
            {
                return;
            }

            var path = new[]
            {
                src.FileSystem.PathCombine(src.ScenarioPath, @"Bitmap\Anime\Explode\EFFECT_Explode"),
                src.FileSystem.PathCombine(src.ScenarioPath, @"Bitmap\Event\Explode"),
                src.FileSystem.PathCombine(src.AppPath, @"Bitmap\Anime\Explode\EFFECT_Explode"),
                src.FileSystem.PathCombine(src.AppPath, @"Bitmap\Event\Explode"),
            }.FirstOrDefault(x => src.FileSystem.FileExists(x + "01.bmp"));
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            var count = 0;
            while (src.FileSystem.FileExists(path + (count + 1).ToString("00") + ".bmp"))
            {
                count++;
            }

            // サイズごとの（表示位置のずれ, 大きさ, 1コマの時間）。汎用戦闘アニメの画像と旧来の画像で異なる
            var isAnime = Strings.InStr(path, "Anime") > 0;
            int offset, size, wait;
            switch (tsize ?? "")
            {
                case "XL": (offset, size, wait) = isAnime ? (64, 160, 130) : (64, 160, 130); break;
                case "LL": (offset, size, wait) = isAnime ? (56, 144, 100) : (48, 128, 100); break;
                case "L": (offset, size, wait) = isAnime ? (48, 128, 70) : (32, 96, 70); break;
                case "M": (offset, size, wait) = isAnime ? (40, 112, 50) : (16, 64, 50); break;
                case "S": (offset, size, wait) = isAnime ? (24, 80, 40) : (8, 48, 40); break;
                case "SS": (offset, size, wait) = isAnime ? (8, 48, 40) : (0, 32, 40); break;
                default: return;
            }
            for (var i = 1; i <= count; i++)
            {
                if (isAnime)
                {
                    ClearPicture();
                }
                DrawPicture(path + i.ToString("00") + ".bmp", MapToPixelX(tx) - offset, MapToPixelY(ty) - offset, size, size, 0, 0, 0, 0, "透過");
                UpdateScreen();
                Sleep(wait);
            }
            ClearPicture();
            UpdateScreen();
        }

        #region カーソル

        // マウスカーソルを動かす処理は OS に依存するため行わない

        public void SaveCursorPos()
        {
        }

        public void MoveCursorPos(string cursor_mode, Unit t = null)
        {
        }

        public void RestoreCursorPos()
        {
        }

        #endregion
    }
}
