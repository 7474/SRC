using Microsoft.Extensions.Logging;
using SRCCore;
using SRCCore.Commands;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using SRCSharpForm.Forms;
using SRCSharpForm.Lib;
using SRCSharpForm.Resoruces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SRCSharpForm
{
    public partial class SRCSharpFormGUI : IGUI
    {
        public bool Terminate()
        {
            return true;
        }

        public void DoEvents()
        {
            Sleep(0, true);
        }

        public void Sleep(int dwMilliseconds, bool withEvents = true)
        {
            if (withEvents)
            {
                Application.DoEvents();
            }
            if (dwMilliseconds > 0)
            {
                Thread.Sleep(dwMilliseconds);
            }
        }

        public bool IsGUILocked { get; set; }
        public short TopItem { get; set; }
        public bool MessageWindowIsOut { get; set; }
        public bool IsFormClicked { get; set; }
        public bool IsMordal { get; set; }
        public int MessageWait { get; set; }
        public bool AutoMessageMode { get; set; }
        public bool HCentering { get; set; }
        public bool VCentering { get; set; }
        public bool PermanentStringMode { get; set; }
        public bool KeepStringMode { get; set; }
        public int MainWidth { get; set; }
        public int MainHeight { get; set; }
        public int MainPWidth
        {
            get => MainForm.MainPWidth;
            set => throw new NotSupportedException();
        }
        public int MainPHeight
        {
            get => MainForm.MainPHeight;
            set => throw new NotSupportedException();
        }
        public int MapPWidth
        {
            get => MainForm.MapPWidth;
            set => throw new NotSupportedException();
        }
        public int MapPHeight
        {
            get => MainForm.MapPHeight;
            set => throw new NotSupportedException();
        }
        public bool ScreenIsMasked { get; set; }
        public bool ScreenIsSaved { get; set; }
        public int MapX { get; set; }
        public int MapY { get; set; }
        public int PrevMapX { get; set; }
        public int PrevMapY { get; set; }
        public GuiButton MouseButton { get; set; }
        public double MouseX { get; set; }
        public double MouseY { get; set; }
        public double PrevMouseX { get; set; }
        public double PrevMouseY { get; set; }
        public int PrevUnitX { get; set; }
        public int PrevUnitY { get; set; }
        public string PrevUnitArea { get; set; }
        public string PrevCommand { get; set; }
        public bool IsPictureDrawn { get; set; }
        public bool IsPictureVisible { get; set; }
        public int PaintedAreaX1 { get; set; }
        public int PaintedAreaY1 { get; set; }
        public int PaintedAreaX2 { get; set; }
        public int PaintedAreaY2 { get; set; }
        public bool IsCursorVisible { get; set; }
        public Color BGColor { get; set; }
        public Font CurrentPaintFont => currentDrawFont;
        // XXX Fontと画像のForeカラー同じでいいのか？
        public Brush CurrentPaintBrush => currentDrawFontColor;
        // XXX そもそもBrushでなくForeColor持っていたほうがいいかもしれない
        public Color CurrentPaintColor => (currentDrawFontColor as SolidBrush).Color;
        int IGUI.TopItem { get; set; }

        private frmNowLoading frmNowLoading;
        private frmTitle frmTitle;

        private frmMessage frmMessage;
        private frmMain MainForm;
        private frmListBox frmListBox;

        private ImageBuffer imageBuffer;

        private SRCCore.SRC SRC;
        private SRCCore.Events.Event Event => SRC.Event;
        private SRCCore.Expressions.Expression Expression => SRC.Expression;
        private SRCCore.Maps.Map Map => SRC.Map;
        private Command Commands => SRC.Commands;
        private Sound Sound => SRC.Sound;
        private Effect Effect => SRC.Effect;

        public SRCSharpFormGUI(SRC src)
        {
            SRC = src;
        }

        public void LoadMainFormAndRegisterFlash()
        {
            imageBuffer = new ImageBuffer(SRC);

            MainForm = new frmMain()
            {
                SRC = SRC,
            };
            SRC.GUIMap = MainForm;
            SRC.GUIStatus = MainForm;
            SRC.GUIScrean = MainForm;
            MainForm.Init(imageBuffer);

            frmListBox = new frmListBox()
            {
                SRC = SRC,
                MainForm = MainForm,
            };

            Program.Log.LogDebug("LoadMainFormAndRegisterFlash");
        }

        public void LoadForms()
        {
            Console.WriteLine("LoadForms");

            //short X, Y;

            //Load(frmToolTip);
            frmMessage = new frmMessage()
            {
                SRC = SRC
            };
            //Load(frmListBox);
            LockGUI();
            //Commands.CommandState = "ユニット選択";

            //// マップ画面に表示できるマップのサイズ
            //switch (Strings.LCase(GeneralLib.ReadIni("Option", "NewGUI")) ?? "")
            //{
            //    case "on":
            //        {
            //            // MOD START MARGE
            //            NewGUIMode = true;
            //            // MOD END MARGE
            //            MainWidth = 20;
            //            break;
            //        }

            //    case "off":
            //        {
            //            MainWidth = 15;
            //            break;
            //        }

            //    default:
            //        {
            //            MainWidth = 15;
            //            SystemConfig.SetItem("Option", "NewGUI", "Off");
            //            break;
            //        }
            //}
            //// ADD START MARGE
            //// Optionで定義されていればそちらを優先する
            //if (Expression.IsOptionDefined("新ＧＵＩ"))
            //{
            //    NewGUIMode = true;
            //    MainWidth = 20;
            //}
            //// ADD END MARGE
            //MainHeight = 15;
            MainWidth = 15;
            MainHeight = 15;

            MainForm.InitMapSize(MainWidth, MainHeight);
            MainForm.InitStatus();
        }

        public void SetNewGUIMode()
        {
            throw new NotSupportedException();
        }
        public void MainFormShow()
        {
            if (!MainFormVisible)
            {
                MainForm.Show();
            }
        }
        public void MainFormHide()
        {
            if (MainFormVisible)
            {
                MainForm.Hide();
            }
        }

        public bool MessageFormVisible => frmMessage.Visible;

        public bool MainFormVisible => MainForm.Visible;
        public string MainFormText { get => MainForm.Text; set => MainForm.Text = value; }

        private string DisplayedPilot;
        private string DisplayMode;
        private Unit RightUnit;
        private Unit LeftUnit;
        private double RightUnitHPRatio;
        private double LeftUnitHPRatio;
        private double RightUnitENRatio;
        private double LeftUnitENRatio;
        private bool _savedMessageFormVisible;
        private Unit _savedLeftUnit;
        private Unit _savedRightUnit;

        public void TransionScrean(TransionPattern pattern, Color fillColor, int frame, int frameMillis)
        {
            var start_time = GeneralLib.timeGetTime();
            using (var copyBuffer = new Bitmap(MainForm.MainBuffer))
            using (var g = Graphics.FromImage(MainForm.MainBuffer))
            {
                for (int i = 0; i <= frame; i++)
                {
                    if (IsRButtonPressed())
                    {
                        i = frame + 1;
                    }
                    int fillOpacity = pattern == TransionPattern.FadeIn
                        ? 0xff * (frame - i) / frame
                        : 0xff * i / frame;

                    var fillBrush = new SolidBrush(Color.FromArgb(Math.Max(0, Math.Min(0xff, fillOpacity)), fillColor));

                    g.DrawImage(copyBuffer, 0, 0);
                    g.FillRectangle(fillBrush, g.VisibleClipBounds);
                    UpdateScreen();

                    if (frame != i)
                    {
                        var cur_time = GeneralLib.timeGetTime();
                        while (cur_time < start_time + frameMillis * (i + 1))
                        {
                            Application.DoEvents();
                            cur_time = GeneralLib.timeGetTime();
                        }
                    }
                }
                // XXX バッファ戻しておく
                g.DrawImage(copyBuffer, 0, 0);
            }
        }

        public void ClearScrean()
        {
            MainForm.ClearScrean();
        }

        public void SetupBackground(string draw_mode, string draw_option, Color filter_color, double filter_trans_par)
        {
            MainForm.SetupBackground(draw_mode, draw_option, filter_color, filter_trans_par);
        }

        public void RedrawScreen(bool late_refresh)
        {
            ScreenIsMasked = false;

            // 画面を更新
            RefreshScreen(false, late_refresh);

            //'カーソルを再描画
            //GetCursorPos PT
            //ret = SetCursorPos(PT.X, PT.Y)
        }

        public void MaskScreen()
        {
            ScreenIsMasked = true;

            // 画面を更新
            RefreshScreen();
        }

        public void RefreshScreen(bool without_refresh = false, bool delay_refresh = false)
        {
            // マップデータが設定されていなければ画面書き換えを行わない
            if (Map.MapWidth == 1)
            {
                return;
            }

            // 表示位置がマップ外にある場合はマップ内に合わせる
            if (MapX < 1)
            {
                MapX = 1;
            }
            else if (MapX > Map.MapWidth)
            {
                MapX = Map.MapWidth;
            }

            if (MapY < 1)
            {
                MapY = 1;
            }
            else if (MapY > Map.MapHeight)
            {
                MapY = Map.MapHeight;
            }

            MainForm.RefreshScreen(MapX, MapY, without_refresh, delay_refresh);
        }

        public void Center(int new_x, int new_y)
        {
            if (Map.IsStatusView)
            {
                return;
            }

            // XXX スクロールバーのMax見るでいいの？
            MapX = new_x;
            if (MapX < 1)
            {
                MapX = 1;
            }
            else if (MapX > MainForm.HScrollBar.Maximum)
            {
                MapX = MainForm.HScrollBar.Maximum;
            }

            MapY = new_y;
            if (MapY < 1)
            {
                MapY = 1;
            }
            else if (MapY > MainForm.VScrollBar.Maximum)
            {
                MapY = MainForm.VScrollBar.Maximum;
            }
        }

        public int MapToPixelX(int X)
        {
            return frmMain.MapCellPx * ((MainWidth + 1) / 2 - 1 - (MapX - X));
        }

        public int MapToPixelY(int Y)
        {
            return frmMain.MapCellPx * ((MainHeight + 1) / 2 - 1 - (MapY - Y));
        }

        public int PixelToMapX(int X)
        {
            if (X < 0)
            {
                X = 0;
            }
            else if (X >= MainPWidth)
            {
                X = MainPWidth - 1;
            }

            return X / frmMain.MapCellPx + 1 + MapX - (MainWidth + 1) / 2;
        }

        public int PixelToMapY(int Y)
        {
            if (Y < 0)
            {
                Y = 0;
            }
            else if (Y >= MainPHeight)
            {
                Y = MainPHeight - 1;
            }

            return Y / frmMain.MapCellPx + 1 + MapY - (MainHeight + 1) / 2;
        }

        public void PaintUnitBitmap(Unit u, string smode)
        {
            MainForm.PaintUnitBitmap(u, smode);
        }

        public void EraseUnitBitmap(int X, int Y, bool do_refresh)
        {
            MainForm.EraseUnitBitmap(X, Y, do_refresh);
        }

        public void MoveUnitBitmap(Unit u, int x1, int y1, int x2, int y2, int wait_time0, int division)
        {
            MainForm.MoveUnitBitmap(u, x1, y1, x2, y2, wait_time0, division);
        }

        public void MoveUnitBitmap2(Unit u, int wait_time0, int division)
        {
            MainForm.MoveUnitBitmap2(u, wait_time0, division);
        }

        public void DieAnimation(Unit u)
        {
            EraseUnitBitmap(u.x, u.y, false);

            // 人間ユニットでない場合は爆発を表示
            if (!u.IsHero())
            {
                ExplodeAnimation(u.Size, u.x, u.y);
                return;
            }

            // 右クリック中はスキップ
            if (IsRButtonPressed())
            {
                return;
            }

            #region XXX 右ボタン参照
            //GUI.GetCursorPos(PT);

            //// メッセージウインドウ上でマウスボタンを押した場合
            //if (ReferenceEquals(Form.ActiveForm, My.MyProject.Forms.m_frmMessage))
            //{
            //    {
            //        var withBlock = My.MyProject.Forms.frmMessage;
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X
            //&& PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX()
            //&& (long)SrcFormatter.PixelsToTwipsY(withBlock.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y
            //&& PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GUI.GetAsyncKeyState(GUI.RButtonID)
            //&& 0x8000) != 0)
            //            {
            //                // 右ボタンで爆発スキップ
            //                return;
            //            }
            //        }
            //    }
            //}

            //// メインウインドウ上でマウスボタンを押した場合
            //if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
            //{
            //    {
            //        var withBlock1 = GUI.MainForm;
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock1.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X
            //&& PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX()
            //&& (long)SrcFormatter.PixelsToTwipsY(withBlock1.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y
            //&& PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock1.Top) + SrcFormatter.PixelsToTwipsY(withBlock1.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GUI.GetAsyncKeyState(GUI.RButtonID)
            //&& 0x8000) != 0)
            //            {
            //                // 右ボタンで爆発スキップ
            //                return;
            //            }
            //        }
            //    }
            //}
            #endregion

            // 倒れる音
            switch (u.Area ?? "")
            {
                case "地上":
                    {
                        Sound.PlayWave("FallDown.wav");
                        break;
                    }

                case "空中":
                    {
                        if (MessageWait > 0)
                        {
                            Sound.PlayWave("Bomb.wav");
                            Sleep(500);
                        }

                        if (Map.Terrain(u.x, u.y).Class == "水"
                            || Map.Terrain(u.x, u.y).Class == "深海")
                        {
                            Sound.PlayWave("Splash.wav");
                        }
                        else
                        {
                            Sound.PlayWave("FallDown.wav");
                        }

                        break;
                    }
            }

            // ユニット消滅のアニメーション

            // メッセージがウエイト無しならアニメーションもスキップ
            if (MessageWait == 0)
            {
                return;
            }

            string fname;
            switch (u.Party0 ?? "")
            {
                case "味方":
                case "ＮＰＣ":
                    {
                        fname = @"Bitmap\Anime\Common\EFFECT_Tile(Ally)";
                        break;
                    }

                case "敵":
                    {
                        fname = @"Bitmap\Anime\Common\EFFECT_Tile(Enemy)";
                        break;
                    }

                case "中立":
                    {
                        fname = @"Bitmap\Anime\Common\EFFECT_Tile(Neutral)";
                        break;
                    }
                default: throw new NotSupportedException(u.Party0);
            }

            if (SRC.FileSystem.FileExists(SRC.ScenarioPath + fname + ".bmp"))
            {
                fname = SRC.ScenarioPath + fname;
            }
            else
            {
                fname = SRC.AppPath + fname;
            }

            if (!SRC.FileSystem.FileExists(fname + "01.bmp"))
            {
                return;
            }

            string draw_mode;
            switch (Map.MapDrawMode ?? "")
            {
                case "夜":
                    {
                        draw_mode = "暗";
                        break;
                    }

                default:
                    {
                        draw_mode = Map.MapDrawMode;
                        break;
                    }
            }

            for (var i = 1; i <= 6; i++)
            {
                DrawPicture(fname + ".bmp", MapToPixelX(u.x), MapToPixelY(u.y), 32, 32, 0, 0, 0, 0, draw_mode);
                DrawPicture(@"Unit\" + u.get_Bitmap(false), MapToPixelX(u.x), MapToPixelY(u.y), 32, 32, 0, 0, 0, 0, "透過 " + draw_mode);
                DrawPicture(fname + "0" + SrcFormatter.Format(i) + ".bmp", MapToPixelX(u.x), MapToPixelY(u.y), 32, 32, 0, 0, 0, 0, "透過 " + draw_mode);
                UpdateScreen();
                Sleep(50);
            }

            ClearPicture();
            UpdateScreen();
        }

        public void ExplodeAnimation(string tsize, int tx, int ty)
        {
            // 右クリック中はスキップ
            if (IsRButtonPressed())
            {
                return;
            }

            #region XXX 右ボタン参照
            //GUI.GetCursorPos(PT);

            //// メッセージウインドウ上でマウスボタンを押した場合
            //if (ReferenceEquals(Form.ActiveForm, My.MyProject.Forms.m_frmMessage))
            //{
            //    {
            //        var withBlock = My.MyProject.Forms.frmMessage;
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X
            //&& PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX()
            //&& (long)SrcFormatter.PixelsToTwipsY(withBlock.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y
            //&& PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GUI.GetAsyncKeyState(GUI.RButtonID)
            //&& 0x8000) != 0)
            //            {
            //                // 右ボタンで爆発スキップ
            //                return;
            //            }
            //        }
            //    }
            //}

            //// メインウインドウ上でマウスボタンを押した場合
            //if (ReferenceEquals(Form.ActiveForm, GUI.MainForm))
            //{
            //    {
            //        var withBlock1 = GUI.MainForm;
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock1.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X
            //&& PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX()
            //&& (long)SrcFormatter.PixelsToTwipsY(withBlock1.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y
            //&& PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock1.Top) + SrcFormatter.PixelsToTwipsY(withBlock1.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GUI.GetAsyncKeyState(GUI.RButtonID)
            //&& 0x8000) != 0)
            //            {
            //                // 右ボタンで爆発スキップ
            //                return;
            //            }
            //        }
            //    }
            //}
            #endregion


            // 爆発音
            switch (tsize ?? "")
            {
                case "XL":
                case "LL":
                    {
                        Sound.PlayWave("Explode(Far).wav");
                        break;
                    }

                case "L":
                case "M":
                case "S":
                case "SS":
                    {
                        Sound.PlayWave("Explode.wav");
                        break;
                    }
            }

            // メッセージがウエイト無しなら爆発もスキップ
            if (MessageWait == 0)
            {
                return;
            }

            // 爆発用画像のパス
            var explode_image_pathes = new string[]
            {
                SRC.FileSystem.PathCombine(SRC.ScenarioPath, @"Bitmap\Anime\Explode\EFFECT_Explode"),
                SRC.FileSystem.PathCombine(SRC.ScenarioPath , @"Bitmap\Event\Explode"),
                SRC.FileSystem.PathCombine(SRC.AppPath , @"Bitmap\Anime\Explode\EFFECT_Explode"),
                SRC.FileSystem.PathCombine(SRC.AppPath , @"Bitmap\Event\Explode"),
            };
            string explode_image_path = explode_image_pathes.FirstOrDefault(x => SRC.FileSystem.FileExists(x + "01.bmp"));

            if (string.IsNullOrEmpty(explode_image_path)) { return; }
            int explode_image_num = 0;
            while (imageBuffer.Get(explode_image_path + (explode_image_num + 1).ToString("00") + ".bmp") != null)
            {
                explode_image_num++;
            }

            // 爆発の表示
            if (Strings.InStr(explode_image_path, @"\Anime\") > 0)
            {
                // 戦闘アニメ版の画像を使用
                switch (tsize ?? "")
                {
                    case "XL":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                ClearPicture();
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 64, MapToPixelY(ty) - 64, 160, 160, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(130);
                            }

                            break;
                        }

                    case "LL":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                ClearPicture();
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 56, MapToPixelY(ty) - 56, 144, 144, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(100);
                            }

                            break;
                        }

                    case "L":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                ClearPicture();
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 48, MapToPixelY(ty) - 48, 128, 128, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(70);
                            }

                            break;
                        }

                    case "M":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                ClearPicture();
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 40, MapToPixelY(ty) - 40, 112, 112, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(50);
                            }

                            break;
                        }

                    case "S":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                ClearPicture();
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 24, MapToPixelY(ty) - 24, 80, 80, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(40);
                            }

                            break;
                        }

                    case "SS":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                ClearPicture();
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 8, MapToPixelY(ty) - 8, 48, 48, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(40);
                            }

                            break;
                        }
                }

                ClearPicture();
                UpdateScreen();
            }
            else
            {
                // 汎用イベント画像版の画像を使用
                switch (tsize ?? "")
                {
                    case "XL":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 64, MapToPixelY(ty) - 64, 160, 160, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(130);
                            }

                            break;
                        }

                    case "LL":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 48, MapToPixelY(ty) - 48, 128, 128, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(100);
                            }

                            break;
                        }

                    case "L":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 32, MapToPixelY(ty) - 32, 96, 96, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(70);
                            }

                            break;
                        }

                    case "M":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 16, MapToPixelY(ty) - 16, 64, 64, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(50);
                            }

                            break;
                        }

                    case "S":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx) - 8, MapToPixelY(ty) - 8, 48, 48, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(40);
                            }

                            break;
                        }

                    case "SS":
                        {
                            for (var i = 1; i <= explode_image_num; i++)
                            {
                                DrawPicture(explode_image_path + i.ToString("00") + ".bmp", MapToPixelX(tx), MapToPixelY(ty), 32, 32, 0, 0, 0, 0, "透過");
                                UpdateScreen();
                                Sleep(40);
                            }

                            break;
                        }
                }

                ClearPicture();
                UpdateScreen();
            }
        }


        public int ListBox(ListBoxArgs args)
        {
            frmListBox.ShowItems(MainForm, args);
            var ListBoxRet = Commands.SelectedItem;
            Application.DoEvents();

            return ListBoxRet;
        }

        public void CloseListBox()
        {
            if (frmListBox.Visible)
            {
                frmListBox.Hide();
                // ShowItems に設定しているので要らんかも
                if (frmListBox.txtComment.Enabled)
                {
                    frmListBox.txtComment.Visible = false;
                    frmListBox.txtComment.Enabled = false;
                    frmListBox.Height = frmListBox.Height - 40;
                }
            }
        }

        public void EnlargeListBoxHeight()
        {
            frmListBox.EnlargeListBoxHeight();
        }

        public void ReduceListBoxHeight()
        {
            frmListBox.ReduceListBoxHeight();
        }

        public void EnlargeListBoxWidth()
        {
            frmListBox.EnlargeListBoxWidth();
        }

        public void ReduceListBoxWidth()
        {
            frmListBox.ReduceListBoxWidth();
        }

        public void AddPartsToListBox()
        {
            frmListBox.AddPartsToListBox();
        }

        public void RemovePartsOnListBox()
        {
            frmListBox.RemovePartsOnListBox();
        }

        public UnitWeapon WeaponListBox(Unit u, UnitWeaponList weapons, string caption_msg, string lb_mode, string BGM)
        {
            TopItem = -1;
            var list = weapons.Items.Select(x => new ListBoxItem()
            {
                Text = $"{ GeneralLib.RightPaddedString(x.Weapon.Name, 28)}" +
                    $" { GeneralLib.LeftPaddedString(x.Weapon.WeaponPower("") + "", 4)}" +
                    $" { GeneralLib.LeftPaddedString(x.Weapon.WeaponPrecision() + "", 3)}" +
                    $" { GeneralLib.LeftPaddedString(x.Weapon.WeaponCritical() + "", 3)}" +
                    $" { GeneralLib.LeftPaddedString(x.Weapon.Bullet() + "", 3)}" +
                    $" { GeneralLib.LeftPaddedString(x.Weapon.WeaponENConsumption() + "", 4)}" +
                    $" {x.Weapon.WeaponData.Adaption}" +
                    $" {x.Weapon.WeaponClass()}",
                ListItemComment = "",
                ListItemFlag = !x.CanUse,
                ListItemID = "",
            }).ToList();
            var ret = ListBox(new ListBoxArgs
            {
                Items = list,
                HasFlag = true,
                lb_caption = caption_msg,
                //"名称                         攻撃 命中 " + Expression.Term("EN"4, u, 2) + "   弾  " + Expression.Term("EN"5, u, 2) + " 適応 分類",
                lb_info = "名称                         攻撃 命中 CT   弾  EN 適応 分類",
                lb_mode = "",
            });
            //var WeaponListBoxRet = wlist[ret];
            Application.DoEvents();
            return ret > 0 ? weapons.Items[ret - 1].Weapon : null;
        }

        public UnitAbility AbilityListBox(Unit u, UnitAbilityList abilities, string caption_msg, string lb_mode, bool is_item = false)
        {
            TopItem = -1;
            var list = abilities.Items.Select(x => new ListBoxItem()
            {
                Text = $"{x.Ability.Data.Name}",
                ListItemComment = "",
                ListItemFlag = !x.CanUse,
                ListItemID = "",
            }).ToList();
            if (list.Count == 1 && lb_mode != "一覧" && !is_item)
            {
                return abilities.Items[0].Ability;
            }
            var ret = ListBox(new ListBoxArgs
            {
                Items = list,
                HasFlag = true,
                lb_caption = caption_msg,
                lb_info = "名称",
                lb_mode = "",
            });
            Application.DoEvents();
            return ret > 0 ? abilities.Items[ret - 1].Ability : null;
        }

        public int LIPS(ListBoxArgs args, int time_limit)
        {
            throw new NotImplementedException();
        }

        public int MultiColumnListBox(ListBoxArgs args, bool is_center)
        {
            using (var box = new frmMultiColumnListBox())
            {
                box.Init(SRC, args);

                // 表示位置を設定
                box.Left = MainForm.Left;
                if (MainForm.Visible == true && MainForm.WindowState != FormWindowState.Minimized && !is_center)
                {
                    box.Top = MainForm.Top + MainForm.Height - box.Height;
                }
                else
                {
                    box.Top = MainForm.Top + (MainForm.Height - box.Height) / 2;
                }
                Commands.SelectedItem = 0;
                Sleep(0, true);
                IsFormClicked = false;

                // リストボックスを表示
                IsMordal = false;
                box.Show();
                while (!IsFormClicked)
                {
                    Sleep(0, true);
                }

                return Commands.SelectedItem;
            }

        }

        public bool IsInDeploymentSelection { get; private set; }

        public int MultiSelectListBox(ListBoxArgs args, int max_num)
        {
            // ステータスウィンドウに攻撃の命中率などを表示させないようにする
            Commands.CommandState = "ユニット選択";

            // リストボックスを作成して表示
            using (var box = new frmMultiSelectListBox())
            {
                box.Init(SRC, args, max_num);
                box.Left = MainForm.Left;
                box.Top = MainForm.Top + (MainForm.Height - box.Height) / 2;
                IsInDeploymentSelection = true;
                try
                {
                    box.ShowDialog();
                }
                finally
                {
                    IsInDeploymentSelection = false;
                }

                // 選択された項目数を返す
                return box.SelectedItemNum;
            }
        }

        public void SaveScreen()
        {
            SRC.LogTrace(ScreenIsSaved + "");
            if (!ScreenIsSaved)
            {
                // XXX 何で半端にMainFormに追い出してあるんだ。
                // 画像をpicMain(1)に保存
                MainForm.SaveScreen();

                ScreenIsSaved = true;
            }
        }

        public void ClearPicture()
        {
            SRC.LogTrace(ScreenIsSaved + "");
            if (!ScreenIsSaved)
            {
                return;
            }

            IsPictureVisible = false;
            IsCursorVisible = false;

            // XXX 全体クリアしておく
            using (var g = Graphics.FromImage(MainForm.MainBuffer))
            {
                g.DrawImage(MainForm.MainBufferBack, 0, 0);
            }
        }

        public void ClearPicture2(int x1, int y1, int x2, int y2)
        {
            if (!ScreenIsSaved)
            {
                return;
            }

            using (var g = Graphics.FromImage(MainForm.MainBuffer))
            {
                var rect = new Rectangle(x1, y1, x2 - x1, y2 - y1);
                g.DrawImage(MainForm.MainBufferBack, rect, rect, GraphicsUnit.Pixel);
            }
        }

        public void LockGUI()
        {
            IsGUILocked = true;
            MainForm.VScrollBar.Enabled = false;
            MainForm.HScrollBar.Enabled = false;
        }

        public void UnlockGUI()
        {
            IsGUILocked = false;
            MainForm.VScrollBar.Enabled = true;
            MainForm.HScrollBar.Enabled = true;
        }

        // カーソル位置自動変更前のマウスカーソルの座標
        private int PrevCursorX;
        private int PrevCursorY;
        // カーソル位置自動変更後のマウスカーソルの座標
        private int NewCursorX;
        private int NewCursorY;

        public void SaveCursorPos()
        {
            var pos = Control.MousePosition;
            PrevCursorX = pos.X;
            PrevCursorY = pos.Y;
            NewCursorX = 0;
            NewCursorY = 0;
        }

        public void MoveCursorPos(string cursor_mode, Unit t)
        {
            // マウスカーソルの位置を収得
            var pos = Control.MousePosition;

            // 現在の位置を記録しておく
            if (PrevCursorX == 0 & cursor_mode != "メッセージウィンドウ")
            {
                SaveCursorPos();
            }

            // カーソル自動移動
            int tx, ty;
            if (t is null)
            {
                if (cursor_mode == "メッセージウィンドウ")
                {
                    // メッセージウィンドウまで移動
                    {
                        if (pos.X < frmMessage.Left + 0.05d * frmMessage.Width)
                        {
                            tx = (int)(frmMessage.Left + 0.05d * frmMessage.Width);
                        }
                        else if (pos.X > frmMessage.Left + 0.95d * frmMessage.Width)
                        {
                            tx = (int)(frmMessage.Left + 0.95d * frmMessage.Width);
                        }
                        else
                        {
                            tx = pos.X;
                        }

                        if (pos.Y < frmMessage.Top + frmMessage.Height - frmMessage.ClientRectangle.Height + frmMessage.picMessage.Top)
                        {
                            ty = frmMessage.Top + frmMessage.Height - frmMessage.ClientRectangle.Height + frmMessage.picMessage.Top;
                        }
                        else if (pos.Y > frmMessage.Top + 0.9d * frmMessage.Height)
                        {
                            ty = (int)(frmMessage.Top + 0.9d * frmMessage.Height);
                        }
                        else
                        {
                            ty = pos.Y;
                        }
                    }
                }
                else
                {
                    // リストボックスまで移動
                    {
                        if (pos.X < frmListBox.Left + 0.1d * frmListBox.Width)
                        {
                            tx = (int)(frmListBox.Left + 0.1d * frmListBox.Width);
                        }
                        else if (pos.X > frmListBox.Left + 0.9d * frmListBox.Width)
                        {
                            tx = (int)(frmListBox.Left + 0.9d * frmListBox.Width);
                        }
                        else
                        {
                            tx = pos.X;
                        }

                        // 選択するアイテム
                        int i;
                        if (cursor_mode == "武器選択")
                        {
                            // 武器選択の場合は選択可能な最後のアイテムに
                            // XXX 援護攻撃：は厳しい。
                            var item = frmListBox.Items.Last(x => !x.ListItemFlag && !x.Text.Contains("援護攻撃："));
                            i = item != null ? frmListBox.Items.IndexOf(item) + 1 : 1;
                        }
                        else
                        {
                            // そうでなければ最初のアイテムに
                            i = TopItem;
                        }

                        // XXX これでいい感じが維持されるかは分からん
                        ty = frmListBox.Top + frmListBox.Height - frmListBox.ClientRectangle.Height
                            + frmListBox.ListBox.Top + 16 * (i - frmListBox.ListBox.TopIndex) - 8;
                    }
                }
            }
            else
            {
                // ユニット上まで移動
                {
                    //var withBlock2 = MainForm;
                    //if (NewGUIMode)
                    //{
                    //    tx = (int)((long)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsX(withBlock2.Left) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelX() + 32 * (t.x - (MapX - MainWidth / 2)) + 4L);
                    //    ty = (int)((long)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock2.Top) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelY() + (long)Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock2.Height) / (long)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsPerPixelY() - Microsoft.VisualBasic.Compatibility.VB6.Support.PixelsToTwipsY(withBlock2.ClientRectangle.Height) + 32 * (t.y - (MapY - MainHeight / 2)) + 16d);
                    //}
                    //else
                    {
                        tx = MainForm.Left + 32 * (t.x - (MapX - MainWidth / 2)) + 24;
                        ty = MainForm.Top + MainForm.Height - MainForm.ClientRectangle.Height + 32 * (t.y - (MapY - MainHeight / 2)) + 20;
                    }
                }
            }

            // 何回に分けて移動するか計算
            var num = (int)((long)Math.Sqrt(Math.Pow(tx - pos.X, 2d) + Math.Pow(ty - pos.Y, 2d)) / 25L + 1L);

            // カーソルを移動
            var prev_lock = IsGUILocked;
            IsGUILocked = true;
            // XXX
            //Status.IsStatusWindowDisabled = true;
            for (var i = 1; i <= num; i++)
            {
                Cursor.Position = new Point(
                    (tx * i + pos.X * (num - i)) / num,
                    (ty * i + pos.Y * (num - i)) / num
                );
                Sleep(10, true);
            }
            //Status.IsStatusWindowDisabled = false;
            IsGUILocked = prev_lock;

            // 新しいカーソル位置を記録
            if (NewCursorX == 0)
            {
                NewCursorX = tx;
                NewCursorY = ty;
            }
        }

        public void RestoreCursorPos()
        {
            // ユニットが選択されていればその場所まで戻す
            if (Commands.SelectedUnit is object)
            {
                if (Commands.SelectedUnit.Status == "出撃")
                {
                    MoveCursorPos("ユニット選択", Commands.SelectedUnit);
                    return;
                }
            }

            // 戻るべき位置が設定されていない？
            if (PrevCursorX == 0 && PrevCursorY == 0)
            {
                return;
            }

            // 現在のカーソル位置収得
            var pos = Control.MousePosition;

            // 以前の位置までカーソル自動移動
            var tx = PrevCursorX;
            var ty = PrevCursorY;
            var num = (short)((long)Math.Sqrt(Math.Pow(tx - pos.X, 2d) + Math.Pow(ty - pos.Y, 2d)) / 50L + 1L);
            for (var i = 1; i <= num; i++)
            {
                Cursor.Position = new Point(
                    (tx * i + pos.X * (num - i)) / num,
                    (ty * i + pos.Y * (num - i)) / num
                );
                Sleep(10, true);
            }

            // 戻り位置を初期化
            PrevCursorX = 0;
            PrevCursorY = 0;
        }

        public void OpenTitleForm()
        {
            frmTitle = new frmTitle();
            frmTitle.Show();
            Application.DoEvents();
        }

        public void CloseTitleForm()
        {
            if (frmTitle != null)
            {
                frmTitle.Close();
                frmTitle.Dispose();
                frmTitle = null;
            }
        }

        public void OpenNowLoadingForm()
        {
            frmNowLoading = new frmNowLoading();
            frmNowLoading.Show(MainForm);
        }

        public void CloseNowLoadingForm()
        {
            frmNowLoading.Close();
            frmNowLoading.Dispose();
            frmNowLoading = null;
            // XXX シナリオのパスが決まってるタイミングでという意味でファイルシステムの状態を再処理してる。カス。
            imageBuffer.InitFileSystemInfo();
        }

        public void DisplayLoadingProgress()
        {
            if (frmNowLoading != null)
            {
                frmNowLoading.Progress();
                Application.DoEvents();
            }
        }

        public void SetLoadImageSize(int new_size)
        {
            frmNowLoading.Value = 0;
            frmNowLoading.Max = new_size;
        }

        public void ChangeDisplaySize(int w, int h)
        {
            // 当面全画面はサポートしない
            throw new NotSupportedException();
        }

        public void ErrorMessage(string msg)
        {
            LogError(msg);

            // SRCは非モーダルだが余計なことを考えたくないのでモーダルにしておく。
            using (var form = new frmErrorMessage())
            {
                form.SetErrorMessage(msg);
                form.ShowDialog();
            }
        }

        public void DataErrorMessage(string msg, string fname, int line_num, string line_buf, string dname)
        {
            string err_msg;

            // エラーが発生したファイル名と行番号
            err_msg = fname + "：" + line_num + "行目" + Constants.vbCr + Constants.vbLf;

            // エラーが発生したデータ名
            if (Strings.Len(dname) > 0)
            {
                err_msg = err_msg + dname + "のデータが不正です。" + Constants.vbCr + Constants.vbLf;
            }

            // エラーの原因
            if (Strings.Len(msg) > 0)
            {
                err_msg = err_msg + msg + Constants.vbCr + Constants.vbLf;
            }

            // なにも指定されていない？
            if (string.IsNullOrEmpty(dname) && string.IsNullOrEmpty(msg))
            {
                err_msg = err_msg + "データが不正です。" + Constants.vbCr + Constants.vbLf;
            }

            // エラーが発生したデータ行
            err_msg = err_msg + line_buf;

            // エラーメッセージを表示
            ErrorMessage(err_msg);
        }

        public bool IsRButtonPressed(bool ignore_message_wait = false)
        {
            // メッセージがウエイト無しならスキップ
            if (!ignore_message_wait && MessageWait == 0)
            {
                return true;
            }

            if (Control.MouseButtons.HasFlag(MouseButtons.Right))
            {
                return true;
            }
            return false;
        }

        public void DisplayTelop(string msg)
        {
            Console.WriteLine("DisplayTelop: " + msg);
            using (var telop = new frmTelop())
            {
                Expression.FormatMessage(ref msg);
                if (Strings.InStr(msg, ".") > 0)
                {
                    msg = msg.Replace(".", Environment.NewLine);
                    telop.Height = 78;
                }
                else
                {
                    telop.Height = 53;
                }

                if (MainForm.Visible == true && !(MainForm.WindowState == FormWindowState.Minimized))
                {
                    telop.Left = MainForm.Left + (MainForm.picMain.Width * MainForm.Width / MainForm.ClientRectangle.Width - telop.Width) / 2;
                    telop.Top = MainForm.Top + (MainForm.Height - telop.Height) / 2;
                }
                else
                {
                    telop.Left = (int)((Screen.PrimaryScreen.Bounds.Width - telop.Width) / 2d);
                    telop.Top = (int)((Screen.PrimaryScreen.Bounds.Height - telop.Height) / 2d);
                }

                telop.Label1.Text = msg;
                telop.Show();
                telop.Refresh();
                if (!IsRButtonPressed())
                {
                    Sleep(1000);
                }
                telop.Close();
            }


        }

        public void SetTitle(string title)
        {
            MainForm.Name = title;
        }

        public void LogDebug(string message)
        {
            Program.Log.LogDebug(message);
        }

        public void LogInfo(string message)
        {
            Program.Log.LogInformation(message);
        }
        public void LogError(string message)
        {
            Program.Log.LogError(message);
        }

        public void ChangeStatus(GuiStatus status)
        {
            switch (status)
            {
                case GuiStatus.WaitCursor:
                    Cursor.Current = Cursors.WaitCursor;
                    break;
                case GuiStatus.IBeam:
                    Cursor.Current = Cursors.IBeam;
                    break;
                default:
                    Cursor.Current = Cursors.Default;
                    break;
            }
        }

        public void ShowUnitCommandMenu(IList<UiCommand> commands)
        {
            MainForm.ShowUnitCommandMenu(commands);
        }

        public void ShowMapCommandMenu(IList<UiCommand> commands)
        {
            MainForm.ShowMapCommandMenu(commands);
        }

        public void UpdateScreen()
        {
            MainForm.UpdateScreen();
        }

        public GuiDialogResult Confirm(string message, string title, GuiConfirmOption option)
        {
            IWin32Window owner = null;

            if (MainForm.Visible)
            {
                owner = MainForm;
            }
            else if (frmMessage.Visible)
            {
                owner = frmMessage;
            }

            var buttons = option.HasFlag(GuiConfirmOption.OkCancel)
                ? MsgBoxButtons.OKCancel
                : MsgBoxButtons.OK;
            MsgBoxIcon? icon = option.HasFlag(GuiConfirmOption.Question) ? MsgBoxIcon.Question : null;

            DialogResult res;
            if (icon.HasValue)
            {
                res = MsgBox.Show(owner, message, title, buttons, icon.Value);

            }
            else
            {
                res = MsgBox.Show(owner, message, title, buttons);
            }
            return res == DialogResult.OK ? GuiDialogResult.Ok : GuiDialogResult.Cancel;
        }

        public GuiDialogResult Input(string message, string title, string defaultValue, out string value)
        {
            using (var dialog = new InputForm())
            {
                dialog.Text = title;
                dialog.InputText = defaultValue;
                var res = dialog.ShowDialog();
                value = dialog.InputText;
                return res == DialogResult.OK ? GuiDialogResult.Ok : GuiDialogResult.Cancel;
            }
        }

        public void Configure()
        {
            using (var dialog = new frmConfiguration())
            {
                dialog.SRC = SRC;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ShowDialog(MainForm);
            }
        }

        public Stream SelectSaveStream(SRCSaveKind saveKind, string defaultName)
        {
            //// 一旦「常に手前に表示」を解除
            //if (My.MyProject.Forms.frmListBox.Visible)
            //{
            //    ret = GUI.SetWindowPos(My.MyProject.Forms.frmListBox.Handle.ToInt32(), -2, 0, 0, 0, 0, 0x3);
            //}
            
            var ext = saveKind == SRCSaveKind.Normal ? "srcs" : "srcq";
            string fname;
            using (var fsd = new SaveFileDialog())
            {
                fsd.Filter = $"save files (*.{ext})|*.{ext}";
                fsd.InitialDirectory = SRC.ScenarioPath;
                fsd.FileName = string.IsNullOrEmpty(defaultName)
                    ? Expression.GetValueAsString("セーブデータファイル名")
                    : defaultName;

                var res = fsd.ShowDialog();
                if (res == DialogResult.OK)
                {
                    fname = fsd.FileName;
                }
                else
                {
                    return null;
                }
            }

            //// 再び「常に手前に表示」
            //if (My.MyProject.Forms.frmListBox.Visible)
            //{
            //    ret = GUI.SetWindowPos(My.MyProject.Forms.frmListBox.Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
            //}

            // キャンセル？
            if (string.IsNullOrEmpty(fname))
            {
                return null;
            }

            // セーブ先はシナリオフォルダ？
            var save_path = Path.GetDirectoryName(fname);
            if (FileSystem.Dir(save_path, FileAttribute.Directory) != FileSystem.Dir(SRC.ScenarioPath, FileAttribute.Directory))
            {
                if (Confirm("セーブファイルはシナリオフォルダにないと読み込めません。" + Constants.vbCr + Constants.vbLf + "このままセーブしますか？",
                   "セーブ",
                    GuiConfirmOption.OkCancel | GuiConfirmOption.Question) != GuiDialogResult.Ok)
                {
                    return null;
                }
            }
            return new FileInfo(fname).Open(FileMode.Create);
        }

        public Stream OpenQuikSaveStream(FileAccess fileAccess)
        {
            if (fileAccess == FileAccess.Read)
            {
                return new FileInfo(Path.Combine(SRC.ScenarioPath, "_クイックセーブ.srcq")).Open(FileMode.Open);
            }
            else
            {
                return new FileInfo(Path.Combine(SRC.ScenarioPath, "_クイックセーブ.srcq")).Open(FileMode.Create);
            }
        }

        public string SelectLoadFile(string title, string initialDirectory, string fileType, string fileExtension)
        {
            using (var ofd = new OpenFileDialog())
            {
                ofd.Title = title;
                var effectiveDir = new[] { initialDirectory, SRC.ScenarioPath }
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x) && Directory.Exists(x));
                if (effectiveDir != null) ofd.InitialDirectory = effectiveDir;
                if (!string.IsNullOrEmpty(fileExtension) && !string.IsNullOrEmpty(fileType))
                {
                    ofd.Filter = $"{fileType} (*.{fileExtension})|*.{fileExtension}|すべてのファイル (*.*)|*.*";
                }
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    return ofd.FileName;
                }
            }
            return "";
        }

        public string SelectSaveFile(string title, string initialDirectory, string initialFile, string fileType, string fileExtension)
        {
            using (var sfd = new SaveFileDialog())
            {
                sfd.Title = title;
                var effectiveDir = new[] { initialDirectory, SRC.ScenarioPath }
                    .FirstOrDefault(x => !string.IsNullOrEmpty(x) && Directory.Exists(x));
                if (effectiveDir != null) sfd.InitialDirectory = effectiveDir;
                sfd.FileName = initialFile ?? "";
                if (!string.IsNullOrEmpty(fileExtension) && !string.IsNullOrEmpty(fileType))
                {
                    sfd.Filter = $"{fileType} (*.{fileExtension})|*.{fileExtension}|すべてのファイル (*.*)|*.*";
                }
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    return sfd.FileName;
                }
            }
            return "";
        }

        public void DisplayGlobalMap()
        {
            MainForm.DisplayGlobalMap();

            // 味方ユニット数、敵ユニット数のカウント
            var num = 0;
            var num2 = 0;
            foreach (Unit u in SRC.UList.Items)
            {
                if (u.Status == "出撃" || u.Status == "格納")
                {
                    if (u.Party0 == "味方" || u.Party0 == "ＮＰＣ")
                    {
                        num = (num + 1);
                    }
                    else
                    {
                        num2 = (num2 + 1);
                    }
                }
            }

            // 各ユニット数の表示
            var prev_mode = AutoMessageMode;
            AutoMessageMode = false;
            OpenMessageForm(u1: null, u2: null);
            DisplayMessage("システム", "味方ユニット： " + num + ";" + "敵ユニット  ： " + num2, "");
            CloseMessageForm();
            AutoMessageMode = prev_mode;

            // 画面を元に戻す
            RefreshScreen();
        }

        public void UpdateHotPoint()
        {
            MainForm.UpdateHotPointTooltip();
        }

        public bool GetKeyState(int i)
        {
            var key = (Keys)i;
            // 左利き設定に対応（システムでマウスボタンが入れ替えられている場合）
            if (SystemInformation.MouseButtonsSwapped)
            {
                switch (key)
                {
                    case Keys.LButton:
                        key = Keys.RButton;
                        break;
                    case Keys.RButton:
                        key = Keys.LButton;
                        break;
                }
            }

            var in_window = false;
            if (key == Keys.LButton || key == Keys.RButton)
            {
                // マウスカーソルの位置を参照
                var pt = Control.MousePosition;

                // メインウインドウ上でマウスボタンを押している？
                if (Form.ActiveForm == MainForm)
                {
                    // picMainはMainFormに内接している前提
                    var clientRect = MainForm.RectangleToScreen(MainForm.picMain.ClientRectangle);
                    var x1 = clientRect.Left;
                    var y1 = clientRect.Top;
                    var x2 = clientRect.Left + clientRect.Width;
                    var y2 = clientRect.Top + clientRect.Height;
                    if (x1 <= pt.X && pt.X <= x2 && y1 <= pt.Y && pt.Y <= y2)
                    {
                        in_window = true;
                    }
                }
            }
            // メインウィンドウがアクティブになっている？
            else
            if (ReferenceEquals(Form.ActiveForm, MainForm))
            {
                in_window = true;
            }

            // ウィンドウが選択されていない場合は常に0を返す
            if (!in_window)
            {
                return false;
            }

            // キーの状態を参照
            return SRCSharpForm.Lib.Windows.GetKeyState((int)key) < 0;
        }
    }
}
