using Microsoft.Extensions.Logging;
using SRCCore;
using SRCCore.Commands;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using SRCSharpForm.Extensions;
using SRCSharpForm.Forms;
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

        public void Sleep(int dwMilliseconds, bool withEvents = true)
        {
            if (withEvents)
            {
                Application.DoEvents();
            }
            Thread.Sleep(dwMilliseconds);
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
        public int BGColor { get; set; }
        int IGUI.TopItem { get; set; }

        private frmNowLoading frmNowLoading;
        private frmTitle frmTitle;

        private frmMessage frmMessage;
        private frmMain MainForm;
        private frmListBox frmListBox;

        private ImageBuffer imageBuffer;


        private SRCCore.SRC SRC;
        private SRCCore.Expressions.Expression Expression => SRC.Expression;
        private SRCCore.Maps.Map Map => SRC.Map;
        private SRCCore.Commands.Command Commands => SRC.Commands;

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
            MainForm.Init(imageBuffer);

            frmListBox = new frmListBox()
            {
                SRC = SRC,
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
            //string argini_section1 = "Option";
            //string argini_entry1 = "NewGUI";
            //switch (Strings.LCase(GeneralLib.ReadIni(argini_section1, argini_entry1)) ?? "")
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
            //            string argini_section = "Option";
            //            string argini_entry = "NewGUI";
            //            string argini_data = "Off";
            //            GeneralLib.WriteIni(argini_section, argini_entry, argini_data);
            //            break;
            //        }
            //}
            //// ADD START MARGE
            //// Optionで定義されていればそちらを優先する
            //string argoname = "新ＧＵＩ";
            //if (Expression.IsOptionDefined(argoname))
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
            throw new NotImplementedException();
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

        private string DisplayedPilot;
        private string DisplayMode;
        private Unit RightUnit;
        private Unit LeftUnit;
        private double RightUnitHPRatio;
        private double LeftUnitHPRatio;
        private double RightUnitENRatio;
        private double LeftUnitENRatio;

        public void OpenMessageForm(Unit u1, Unit u2)
        {
            // ユニット表示を伴う場合はキャプションから「(自動送り)」を削除
            if (u1 is object)
            {
                if (frmMessage.Text == "メッセージ (自動送り)")
                {
                    frmMessage.Text = "メッセージ";
                }
            }

            // メッセージウィンドウを強制的に最小化解除
            if (frmMessage.WindowState != FormWindowState.Normal)
            {
                frmMessage.WindowState = FormWindowState.Normal;
                frmMessage.Show(MainForm);
                frmMessage.Activate();
            }

            SetMessageWindowUnit(u1, u2);

            // メッセージウィンドウの位置設定
            if (MainForm.Visible && MainForm.WindowState != FormWindowState.Minimized)
            {
                // メインウィンドウが表示されていればメインウィンドウの下端に合わせて表示
                if (!frmMessage.Visible)
                {
                    if (MainWidth == 15)
                    {
                        frmMessage.Left = MainForm.Left;
                    }
                    else
                    {
                        frmMessage.Left = MainForm.Left - (MainForm.Width - frmMessage.Width) / 2;
                    }

                    if (MessageWindowIsOut)
                    {
                        frmMessage.Top = MainForm.Top + MainForm.Height;// - 350;
                    }
                    else
                    {
                        frmMessage.Top = MainForm.Top + MainForm.Height - frmMessage.Height;
                    }
                }
            }
            else
            {
                //// メインウィンドウが表示されていない場合は画面中央に表示
                //frmMessage.Left = (int)SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(frmMessage.Width)) / 2d);
                //frmMessage.Top = (int)SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(frmMessage.Height)) / 2d);
            }

            // ウィンドウをクリアしておく
            ClearMessageForm();

            // ウィンドウを表示
            if (!frmMessage.Visible)
            {
                frmMessage.Show(MainForm);
            }

            // 常に手前に表示する
            frmMessage.TopMost = true;

            Application.DoEvents();
        }

        private void SetMessageWindowUnit(Unit u1, Unit u2)
        {
            if (u1 is null)
            {
                // ユニット表示なし
                frmMessage.labHP1.Visible = false;
                frmMessage.labHP2.Visible = false;
                frmMessage.labEN1.Visible = false;
                frmMessage.labEN2.Visible = false;
                frmMessage.picHP1.Visible = false;
                frmMessage.picHP2.Visible = false;
                frmMessage.picEN1.Visible = false;
                frmMessage.picEN2.Visible = false;
                frmMessage.txtHP1.Visible = false;
                frmMessage.txtHP2.Visible = false;
                frmMessage.txtEN1.Visible = false;
                frmMessage.txtEN2.Visible = false;
                frmMessage.picUnit1.Visible = false;
                frmMessage.picUnit2.Visible = false;
                // XXX 余白
                frmMessage.Width = frmMessage.Width - frmMessage.ClientRectangle.Width + 508;
                frmMessage.Height = frmMessage.Height - frmMessage.ClientRectangle.Height + 84;
                frmMessage.picFace.Top = 8;
                frmMessage.picFace.Left = 8;
                frmMessage.labKariText.Top = 7;
                frmMessage.labKariText.Left = 84;
            }
            else if (u2 is null)
            {
                // ユニット表示１体のみ
                if (u1.Party == "味方" | u1.Party == "ＮＰＣ")
                {
                    frmMessage.labHP1.Visible = false;
                    frmMessage.labEN1.Visible = false;
                    frmMessage.picHP1.Visible = false;
                    frmMessage.picEN1.Visible = false;
                    frmMessage.txtHP1.Visible = false;
                    frmMessage.txtEN1.Visible = false;
                    frmMessage.picUnit1.Visible = false;
                    frmMessage.labHP2.Visible = true;
                    frmMessage.labEN2.Visible = true;
                    frmMessage.picHP2.Visible = true;
                    frmMessage.picEN2.Visible = true;
                    frmMessage.txtHP2.Visible = true;
                    frmMessage.txtEN2.Visible = true;
                    frmMessage.picUnit2.Visible = true;
                }
                else
                {
                    frmMessage.labHP1.Visible = true;
                    frmMessage.labEN1.Visible = true;
                    frmMessage.picHP1.Visible = true;
                    frmMessage.picEN1.Visible = true;
                    frmMessage.txtHP1.Visible = true;
                    frmMessage.txtEN1.Visible = true;
                    frmMessage.picUnit1.Visible = true;
                    frmMessage.labHP2.Visible = false;
                    frmMessage.labEN2.Visible = false;
                    frmMessage.picHP2.Visible = false;
                    frmMessage.picEN2.Visible = false;
                    frmMessage.txtHP2.Visible = false;
                    frmMessage.txtEN2.Visible = false;
                    frmMessage.picUnit2.Visible = false;
                }

                UpdateMessageForm(u1, null);
                // XXX 余白
                frmMessage.Width = frmMessage.Width - frmMessage.ClientRectangle.Width + 508;
                frmMessage.Height = frmMessage.Height - frmMessage.ClientRectangle.Height + 118;
                frmMessage.picFace.Top = 42;
                frmMessage.picFace.Left = 8;
                frmMessage.labKariText.Top = 41;
                frmMessage.labKariText.Left = 84;
            }
            else
            {
                // ユニットを２体表示
                frmMessage.labHP1.Visible = true;
                frmMessage.labHP2.Visible = true;
                frmMessage.labEN1.Visible = true;
                frmMessage.labEN2.Visible = true;
                frmMessage.picHP1.Visible = true;
                frmMessage.picHP2.Visible = true;
                frmMessage.picEN1.Visible = true;
                frmMessage.picEN2.Visible = true;
                frmMessage.txtHP1.Visible = true;
                frmMessage.txtHP2.Visible = true;
                frmMessage.txtEN1.Visible = true;
                frmMessage.txtEN2.Visible = true;
                frmMessage.picUnit1.Visible = true;
                frmMessage.picUnit2.Visible = true;
                UpdateMessageForm(u1, u2);
                // XXX 余白
                frmMessage.Width = frmMessage.Width - frmMessage.ClientRectangle.Width + 508;
                frmMessage.Height = frmMessage.Height - frmMessage.ClientRectangle.Height + 118;
                frmMessage.picFace.Top = 42;
                frmMessage.picFace.Left = 8;
                frmMessage.picMessage.Top = 41;
                frmMessage.picMessage.Left = 84;
                frmMessage.labKariText.Top = 41;
                frmMessage.labKariText.Left = 84;
            }
        }

        public void CloseMessageForm()
        {
            frmMessage.Hide();
            Application.DoEvents();
        }

        public void ClearMessageForm()
        {
            DisplayedPilot = "";
            RightUnit = null;
            LeftUnit = null;

            frmMessage.ClearForm();
            Application.DoEvents();
        }


        private Brush BarBackBrush = new SolidBrush(Color.FromArgb(0xc0, 0, 0));
        private Brush BarForeBrush = new SolidBrush(Color.FromArgb(0, 0xc0, 0));
        public void UpdateMessageForm(Unit u1, Unit u2)
        {
            Unit lu, ru;
            // ウィンドウにユニット情報が表示されていない場合はそのまま終了
            if (frmMessage.Visible)
            {
                if (!frmMessage.picUnit1.Visible & !frmMessage.picUnit2.Visible)
                {
                    return;
                }
            }

            // luを左に表示するユニット、ruを右に表示するユニットに設定
            // XXX IsNothing と Null の差分とか考慮してねー。裏技的だが null, null 指定でそうすることができなくもない。
            //if (Information.IsNothing(u2))
            if (u2 is null)
            {
                // １体のユニットのみ表示
                if (u1.Party == "味方" | u1.Party == "ＮＰＣ")
                {
                    lu = null;
                    ru = u1;
                }
                else
                {
                    lu = u1;
                    ru = null;
                }
            }
            else if (u2 is null)
            {
                // 反射攻撃
                // 前回表示されたユニットをそのまま使用
                lu = LeftUnit;
                ru = RightUnit;
            }
            else if ((ReferenceEquals(u2, LeftUnit) | ReferenceEquals(u1, RightUnit)) & !ReferenceEquals(LeftUnit, RightUnit))
            {
                lu = u2;
                ru = u1;
            }
            else
            {
                lu = u1;
                ru = u2;
            }

            // 現在表示されている順番に応じてユニットの入れ替え
            if (ReferenceEquals(lu, RightUnit) && ReferenceEquals(ru, LeftUnit) && !ReferenceEquals(LeftUnit, RightUnit))
            {
                lu = LeftUnit;
                ru = RightUnit;
            }

            // 表示するユニットのＧＵＩ部品を表示
            if (lu != null)
            {
                if (!frmMessage.labHP1.Visible)
                {
                    frmMessage.labHP1.Visible = true;
                    frmMessage.labEN1.Visible = true;
                    frmMessage.picHP1.Visible = true;
                    frmMessage.picEN1.Visible = true;
                    frmMessage.txtHP1.Visible = true;
                    frmMessage.txtEN1.Visible = true;
                    frmMessage.picUnit1.Visible = true;
                }
            }

            if (ru != null)
            {
                if (!frmMessage.labHP2.Visible)
                {
                    frmMessage.labHP2.Visible = true;
                    frmMessage.labEN2.Visible = true;
                    frmMessage.picHP2.Visible = true;
                    frmMessage.picEN2.Visible = true;
                    frmMessage.txtHP2.Visible = true;
                    frmMessage.txtEN2.Visible = true;
                    frmMessage.picUnit2.Visible = true;
                }
            }

            string buf;
            // 未表示のユニットを表示する
            if (lu != null && !ReferenceEquals(lu, LeftUnit))
            {
                // 左のユニットが未表示なので表示する

                // ユニット画像
                frmMessage.picUnit1.NewImageIfNull();
                using (var g = Graphics.FromImage(frmMessage.picUnit1.Image))
                {
                    MainForm.DrawUnit(g, Map.CellAtPoint(lu.x, lu.y), lu, new Rectangle(0, 0, frmMessage.picUnit1.Width, frmMessage.picUnit1.Height));
                }
                // TODO BitmapID
                //if (lu.BitmapID > 0)
                //{

                //    if (string.IsNullOrEmpty(Map.MapDrawMode))
                //    {
                //        ret = BitBlt(frmMessage.picUnit1.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * ((int)lu.BitmapID % 15), 96 * ((int)lu.BitmapID / 15), SRCCOPY);
                //    }
                //    else
                //    {
                //        var argpic = frmMessage.picUnit1;
                //        string argfname = "";
                //        LoadUnitBitmap(lu, argpic, 0, 0, true, fname: argfname);
                //        frmMessage.picUnit1 = argpic;
                //    }
                //}
                //else
                //{
                //    // 非表示のユニットの場合はユニットのいる地形タイルを表示
                //    ret = BitBlt(frmMessage.picUnit1.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * ((int)lu.x - 1), 32 * ((int)lu.y - 1), SRCCOPY);
                //}

                frmMessage.picUnit1.Refresh();

                // ＨＰ名称
                if (lu.IsConditionSatisfied("データ不明"))
                {
                    frmMessage.labHP1.Text = Expression.Term("HP", null);
                }
                else
                {
                    frmMessage.labHP1.Text = Expression.Term("HP", lu);
                }

                // ＨＰ数値
                if (lu.IsConditionSatisfied("データ不明"))
                {
                    frmMessage.txtHP1.Text = "?????/?????";
                }
                else
                {
                    if (lu.HP < 100000)
                    {
                        string argbuf = SrcFormatter.Format(lu.HP);
                        buf = GeneralLib.LeftPaddedString(argbuf, GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxHP)), 5));
                    }
                    else
                    {
                        buf = "?????";
                    }

                    if (lu.MaxHP < 100000)
                    {
                        buf = buf + "/" + SrcFormatter.Format(lu.MaxHP);
                    }
                    else
                    {
                        buf = buf + "/?????";
                    }

                    frmMessage.txtHP1.Text = buf;
                }

                // ＨＰゲージ
                frmMessage.picHP1.DrawBar((float)lu.HP / lu.MaxHP, BarBackBrush, BarForeBrush);

                // ＥＮ名称
                if (lu.IsConditionSatisfied("データ不明"))
                {
                    frmMessage.labEN1.Text = Expression.Term("EN", null);
                }
                else
                {
                    frmMessage.labEN1.Text = Expression.Term("EN", lu);
                }

                // ＥＮ数値
                if (lu.IsConditionSatisfied("データ不明"))
                {
                    frmMessage.txtEN1.Text = "???/???";
                }
                else
                {
                    if (lu.EN < 1000)
                    {
                        string argbuf1 = SrcFormatter.Format(lu.EN);
                        buf = GeneralLib.LeftPaddedString(argbuf1, GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxEN)), 3));
                    }
                    else
                    {
                        buf = "???";
                    }

                    if (lu.MaxEN < 1000)
                    {
                        buf = buf + "/" + SrcFormatter.Format(lu.MaxEN);
                    }
                    else
                    {
                        buf = buf + "/???";
                    }

                    frmMessage.txtEN1.Text = buf;
                }

                // ＥＮゲージ
                frmMessage.picEN1.DrawBar((float)lu.EN / lu.MaxEN, BarBackBrush, BarForeBrush);

                // 表示内容を記録
                LeftUnit = lu;
                LeftUnitHPRatio = lu.HP / (double)lu.MaxHP;
                LeftUnitENRatio = lu.EN / (double)lu.MaxEN;
            }

            if (ru != null && !ReferenceEquals(RightUnit, ru))
            {
                // 右のユニットが未表示なので表示する

                // ユニット画像
                frmMessage.picUnit2.NewImageIfNull();
                using (var g = Graphics.FromImage(frmMessage.picUnit2.Image))
                {
                    MainForm.DrawUnit(g, Map.CellAtPoint(ru.x, ru.y), ru, new Rectangle(0, 0, frmMessage.picUnit2.Width, frmMessage.picUnit2.Height));
                }
                // TODO BitmapID
                //if (ru.BitmapID > 0)
                //{
                //    if (string.IsNullOrEmpty(Map.MapDrawMode))
                //    {
                //        ret = BitBlt(frmMessage.picUnit2.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * ((int)ru.BitmapID % 15), 96 * ((int)ru.BitmapID / 15), SRCCOPY);
                //    }
                //    else
                //    {
                //        var argpic1 = frmMessage.picUnit2;
                //        string argfname1 = "";
                //        LoadUnitBitmap(ru, argpic1, 0, 0, true, fname: argfname1);
                //        frmMessage.picUnit2 = argpic1;
                //    }
                //}
                //else
                //{
                //    // 非表示のユニットの場合はユニットのいる地形タイルを表示
                //    ret = BitBlt(frmMessage.picUnit2.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * ((int)ru.x - 1), 32 * ((int)ru.y - 1), SRCCOPY);
                //}

                frmMessage.picUnit2.Refresh();

                // ＨＰ数値
                if (ru.IsConditionSatisfied("データ不明"))
                {
                    frmMessage.labHP2.Text = Expression.Term("HP", null);
                }
                else
                {
                    frmMessage.labHP2.Text = Expression.Term("HP", ru);
                }

                // ＨＰ数値
                if (ru.IsConditionSatisfied("データ不明"))
                {
                    frmMessage.txtHP2.Text = "?????/?????";
                }
                else
                {
                    if (ru.HP < 100000)
                    {
                        string argbuf2 = SrcFormatter.Format(ru.HP);
                        buf = GeneralLib.LeftPaddedString(argbuf2, GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxHP)), 5));
                    }
                    else
                    {
                        buf = "?????";
                    }

                    if (ru.MaxHP < 100000)
                    {
                        buf = buf + "/" + SrcFormatter.Format(ru.MaxHP);
                    }
                    else
                    {
                        buf = buf + "/?????";
                    }

                    frmMessage.txtHP2.Text = buf;
                }

                // ＨＰゲージ
                frmMessage.picHP2.DrawBar((float)ru.HP / ru.MaxHP, BarBackBrush, BarForeBrush);

                // ＥＮ名称
                if (ru.IsConditionSatisfied("データ不明"))
                {
                    frmMessage.labEN2.Text = Expression.Term("EN", null);
                }
                else
                {
                    frmMessage.labEN2.Text = Expression.Term("EN", ru);
                }

                // ＥＮ数値
                if (ru.IsConditionSatisfied("データ不明"))
                {
                    frmMessage.txtEN2.Text = "???/???";
                }
                else
                {
                    if (ru.EN < 1000)
                    {
                        string argbuf3 = SrcFormatter.Format(ru.EN);
                        buf = GeneralLib.LeftPaddedString(argbuf3, GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxEN)), 3));
                    }
                    else
                    {
                        buf = "???";
                    }

                    if (ru.MaxEN < 1000)
                    {
                        buf = buf + "/" + SrcFormatter.Format(ru.MaxEN);
                    }
                    else
                    {
                        buf = buf + "/???";
                    }

                    frmMessage.txtEN2.Text = buf;
                }

                // ＥＮゲージ
                frmMessage.picEN2.DrawBar((float)ru.EN / ru.MaxEN, BarBackBrush, BarForeBrush);

                // 表示内容を記録
                RightUnit = ru;
                RightUnitHPRatio = ru.HP / (double)ru.MaxHP;
                RightUnitENRatio = ru.EN / (double)ru.MaxEN;
            }

            // 前回の表示からのＨＰ、ＥＮの変化をアニメ表示

            // 変化がない場合はアニメ表示の必要がないのでチェックしておく
            var num = 0;
            if (lu != null)
            {
                if (lu.HP / (double)lu.MaxHP != LeftUnitHPRatio || lu.EN / (double)lu.MaxEN != LeftUnitENRatio)
                {
                    num = 8;
                }
            }

            if (ru != null)
            {
                // XXX これ常に真になるんじゃないか？　それによって常にバーアニメしてた？
                //if (ru.HP != RightUnitHPRatio | ru.EN != RightUnitENRatio)
                if (ru.HP / (double)ru.MaxHP != RightUnitHPRatio || ru.EN / (double)ru.MaxEN != RightUnitENRatio)
                {
                    num = 8;
                }
            }

            // 右ボタンが押されている場合はアニメーション表示を短縮化
            if (num > 0)
            {
                if (IsRButtonPressed())
                {
                    num = 2;
                }
            }

            for (var i = 1; i <= num; i++)
            {
                // 左側のユニット
                if (lu != null)
                {
                    // ＨＰ
                    if (lu.HP / (double)lu.MaxHP != LeftUnitHPRatio)
                    {
                        var tmp = (int)((lu.MaxHP * LeftUnitHPRatio * (num - i) + lu.HP * i) / num);
                        if (lu.IsConditionSatisfied("データ不明"))
                        {
                            frmMessage.txtHP1.Text = "?????/?????";
                        }
                        else
                        {
                            if (lu.HP < 100000)
                            {
                                string argbuf4 = SrcFormatter.Format(tmp);
                                buf = GeneralLib.LeftPaddedString(argbuf4, GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxHP)), 5));
                            }
                            else
                            {
                                buf = "?????";
                            }

                            if (lu.MaxHP < 100000)
                            {
                                buf = buf + "/" + SrcFormatter.Format(lu.MaxHP);
                            }
                            else
                            {
                                buf = buf + "/?????";
                            }

                            frmMessage.txtHP1.Text = buf;
                        }

                        frmMessage.picHP1.DrawBar((float)tmp / lu.MaxHP, BarBackBrush, BarForeBrush);
                    }

                    // ＥＮ
                    if (lu.EN / (double)lu.MaxEN != LeftUnitENRatio)
                    {
                        var tmp = (int)((lu.MaxEN * LeftUnitENRatio * (num - i) + lu.EN * i) / num);
                        if (lu.IsConditionSatisfied("データ不明"))
                        {
                            frmMessage.txtEN1.Text = "???/???";
                        }
                        else
                        {
                            if (lu.EN < 1000)
                            {
                                string argbuf5 = SrcFormatter.Format(tmp);
                                buf = GeneralLib.LeftPaddedString(argbuf5, GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxEN)), 3));
                            }
                            else
                            {
                                buf = "???";
                            }

                            if (lu.MaxEN < 1000)
                            {
                                buf = buf + "/" + SrcFormatter.Format(lu.MaxEN);
                            }
                            else
                            {
                                buf = buf + "/???";
                            }

                            frmMessage.txtEN1.Text = buf;
                        }

                        frmMessage.picEN1.DrawBar((float)tmp / lu.MaxEN, BarBackBrush, BarForeBrush);
                    }
                }

                // 右側のユニット
                if (ru != null)
                {
                    // ＨＰ
                    if (ru.HP / (double)ru.MaxHP != RightUnitHPRatio)
                    {
                        var tmp = (int)((long)(ru.MaxHP * RightUnitHPRatio * (num - i) + ru.HP * i) / num);
                        if (ru.IsConditionSatisfied("データ不明"))
                        {
                            frmMessage.txtHP2.Text = "?????/?????";
                        }
                        else
                        {
                            if (ru.HP < 100000)
                            {
                                string argbuf6 = SrcFormatter.Format(tmp);
                                buf = GeneralLib.LeftPaddedString(argbuf6, GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxHP)), 5));
                            }
                            else
                            {
                                buf = "?????";
                            }

                            if (ru.MaxHP < 100000)
                            {
                                buf = buf + "/" + SrcFormatter.Format(ru.MaxHP);
                            }
                            else
                            {
                                buf = buf + "/?????";
                            }

                            frmMessage.txtHP2.Text = buf;
                        }

                        frmMessage.picHP2.DrawBar((float)tmp / ru.MaxHP, BarBackBrush, BarForeBrush);
                    }

                    // ＥＮ
                    if (ru.EN / (double)ru.MaxEN != RightUnitENRatio)
                    {
                        var tmp = (int)((ru.MaxEN * RightUnitENRatio * (num - i) + ru.EN * i) / num);
                        if (ru.IsConditionSatisfied("データ不明"))
                        {
                            frmMessage.txtEN2.Text = "???/???";
                        }
                        else
                        {
                            if (ru.EN < 1000)
                            {
                                string argbuf7 = SrcFormatter.Format(tmp);
                                buf = GeneralLib.LeftPaddedString(argbuf7, GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxEN)), 3));
                            }
                            else
                            {
                                buf = "???";
                            }

                            if (ru.MaxEN < 1000)
                            {
                                buf = buf + "/" + SrcFormatter.Format(ru.MaxEN);
                            }
                            else
                            {
                                buf = buf + "/???";
                            }

                            frmMessage.txtEN2.Text = buf;
                        }

                        frmMessage.picEN2.DrawBar((float)tmp / ru.MaxEN, BarBackBrush, BarForeBrush);
                    }
                }

                // リフレッシュ
                if (lu != null)
                {
                    if (lu.HP / (double)lu.MaxHP != LeftUnitHPRatio)
                    {
                        frmMessage.picHP1.Refresh();
                        frmMessage.txtHP1.Refresh();
                    }

                    if (lu.EN / (double)lu.MaxEN != LeftUnitENRatio)
                    {
                        frmMessage.picEN1.Refresh();
                        frmMessage.txtEN1.Refresh();
                    }
                }

                if (ru != null)
                {
                    if (ru.HP / (double)ru.MaxHP != RightUnitHPRatio)
                    {
                        frmMessage.picHP2.Refresh();
                        frmMessage.txtHP2.Refresh();
                    }

                    if (ru.EN / (double)ru.MaxEN != RightUnitENRatio)
                    {
                        frmMessage.picEN2.Refresh();
                        frmMessage.txtEN2.Refresh();
                    }
                }

                Sleep(20);
            }

            // 表示内容を記録
            if (lu != null)
            {
                LeftUnitHPRatio = lu.HP / (double)lu.MaxHP;
                LeftUnitENRatio = lu.EN / (double)lu.MaxEN;
            }

            if (ru != null)
            {
                RightUnitHPRatio = ru.HP / (double)ru.MaxHP;
                RightUnitENRatio = ru.EN / (double)ru.MaxEN;
            }
            Application.DoEvents();
        }

        public void SaveMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        public void KeepMessageFormStatus()
        {
            throw new NotImplementedException();
        }

        public void DisplayMessage(string pname, string msg, string msg_mode)
        {
            // TODO 完全に仮実装
            string pnickname;
            string left_margin;
            DisplayMessagePilot(pname, msg_mode, out pnickname, out left_margin);

            var tmpMsg = msg;
            Expression.ReplaceSubExpression(ref tmpMsg);

            frmMessage.SetMessage(tmpMsg);
            Application.DoEvents();

            // 次のメッセージ待ち
            IsFormClicked = false;
            while (!IsFormClicked)
            {
                if (IsRButtonPressed(true))
                {
                    break;
                }
                Thread.Sleep(100);
                Application.DoEvents();
            }
        }

        private void DisplayMessagePilot(string pname, string msg_mode, out string pnickname, out string left_margin)
        {
            pnickname = "";
            left_margin = "";
            // キャラ表示の描き換え
            if (pname == "システム")
            {
                // 「システム」
                frmMessage.picFace.Image = null;
                frmMessage.picFace.Refresh();
                DisplayedPilot = "";
                left_margin = "";
            }
            else if (!string.IsNullOrEmpty(pname))
            {
                // どのキャラ画像を使うか？
                var fname = "-.bmp";
                // TODO
                //if (SRC.PList.IsDefined(pname))
                //{
                //    Pilot localItem() { object argIndex1 = pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                //    pnickname = localItem().get_Nickname(false);
                //    Pilot localItem1() { object argIndex1 = pname; var ret = SRC.PList.Item(argIndex1); return ret; }

                //    fname = localItem1().get_Bitmap(false);
                //}
                //else
                if (SRC.PDList.IsDefined(pname))
                {
                    var pd = SRC.PDList.Item(pname);
                    pnickname = pd.Nickname;
                    fname = pd.Bitmap;
                }
                else if (SRC.NPDList.IsDefined(pname))
                {
                    var pd = SRC.NPDList.Item(pname);
                    pnickname = pd.Nickname;
                    fname = pd.Bitmap;
                }

                // キャラ画像の表示
                if (fname != "-.bmp")
                {
                    fname = SRC.FileSystem.PathCombine("Pilot", fname);
                    if ((DisplayedPilot ?? "") != (fname ?? "") || (DisplayMode ?? "") != (msg_mode ?? ""))
                    {
                        string argdraw_option = "メッセージ " + msg_mode;
                        if (DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, argdraw_option))
                        {
                            frmMessage.picFace.Refresh();
                            DisplayedPilot = fname;
                            DisplayMode = msg_mode;
                        }
                        else
                        {
                            frmMessage.picFace.Image = null;
                            frmMessage.picFace.Refresh();
                            DisplayedPilot = "";
                            DisplayMode = "";

                            // TODO
                            //// パイロット画像が存在しないことを記録しておく
                            //object argIndex3 = pname;
                            //if (SRC.PList.IsDefined(pname))
                            //{
                            //    object argIndex2 = pname;
                            //    {
                            //        var withBlock = SRC.PList.Item(argIndex2);
                            //        if ((withBlock.get_Bitmap(false) ?? "") == (withBlock.Data.Bitmap ?? ""))
                            //        {
                            //            withBlock.Data.IsBitmapMissing = true;
                            //        }
                            //    }
                            //}
                            //else if (SRC.PDList.IsDefined(pname))
                            //{
                            //    PilotData localItem6() { object argIndex1 = pname; var ret = SRC.PDList.Item(argIndex1); return ret; }

                            //    localItem6().IsBitmapMissing = true;
                            //}
                            //else if (SRC.NPDList.IsDefined(pname))
                            //{
                            //    NonPilotData localItem7() { object argIndex1 = pname; var ret = SRC.NPDList.Item(argIndex1); return ret; }

                            //    localItem7().IsBitmapMissing = true;
                            //}
                        }
                    }
                }
                else
                {
                    frmMessage.picFace.Image = null;
                    frmMessage.picFace.Refresh();
                    DisplayedPilot = "";
                    DisplayMode = "";
                }

                // TODO
                left_margin = "  ";
                //if (Expression.IsOptionDefined("会話パイロット名改行"))
                //{
                //    left_margin = " ";
                //}
                //else
                //{
                //    left_margin = "  ";
                //}
            }
        }

        public void PrintMessage(string msg, bool is_sys_msg)
        {
            throw new NotImplementedException();
        }

        public int MessageLen(string msg)
        {
            throw new NotImplementedException();
        }

        public void DisplayBattleMessage(string pname, string msg, string msg_mode)
        {
            // XXX バイパスしてあるだけ
            DisplayMessage(pname, msg, msg_mode);
        }

        public void DisplaySysMessage(string msg, bool short_wait)
        {
            // TODO Impl
            MessageWait = 700;
            string pnickname;
            string left_margin;
            DisplayMessagePilot("システム", "", out pnickname, out left_margin);

            frmMessage.SetMessage(msg);
            Application.DoEvents();

            var lnum = msg.Length;
            var wait_time = (int)((0.8d + 0.5d * lnum) * MessageWait);
            if (short_wait)
            {
                wait_time = wait_time / 2;
            }
            IsFormClicked = false;
            var start_time = GeneralLib.timeGetTime();
            while (start_time + wait_time > GeneralLib.timeGetTime())
            {
                // 左ボタンが押されたらメッセージ送り
                if (IsFormClicked)
                {
                    break;
                }

                // 右ボタンを押されていたら早送り
                if (IsRButtonPressed())
                {
                    break;
                }

                Sleep(20);
                Application.DoEvents();
            }
        }

        public void ClearScrean()
        {
            MainForm.ClearScrean();
        }

        public void SetupBackground(string draw_mode, string draw_option, int filter_color, double filter_trans_par)
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

        public int MakeUnitBitmap(Unit u)
        {
            throw new NotImplementedException();
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
            }
        }

        public void EnlargeListBoxHeight()
        {
            throw new NotImplementedException();
        }

        public void ReduceListBoxHeight()
        {
            throw new NotImplementedException();
        }

        public void EnlargeListBoxWidth()
        {
            throw new NotImplementedException();
        }

        public void ReduceListBoxWidth()
        {
            throw new NotImplementedException();
        }

        public void AddPartsToListBox()
        {
            throw new NotImplementedException();
        }

        public void RemovePartsOnListBox()
        {
            throw new NotImplementedException();
        }

        public int WeaponListBox(Unit u, IList<UnitWeapon> weapons, string caption_msg, string lb_mode, string BGM)
        {
            // TODO Impl
            //short WeaponListBoxRet = default;
            //short ret, j, i, k, w;
            //string[] list;
            //short[] wlist;
            //short[] warray;
            //int[] wpower;
            //string wclass;
            //var is_rbutton_released = default(bool);
            //string buf;
            //{
            //    var withBlock = u;
            //    warray = new short[(withBlock.CountWeapon() + 1)];
            //    wpower = new int[(withBlock.CountWeapon() + 1)];
            //    ListItemFlag = new bool[(withBlock.CountWeapon() + 1)];
            //    var ToolTips = new object[(withBlock.CountWeapon() + 1)];
            //    var loopTo = withBlock.CountWeapon();
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        string argtarea = "";
            //        wpower[i] = withBlock.WeaponPower(i, argtarea);
            //    }

            //    // 攻撃力でソート
            //    var loopTo1 = withBlock.CountWeapon();
            //    for (i = 1; i <= loopTo1; i++)
            //    {
            //        var loopTo2 = (i - 1);
            //        for (j = 1; j <= loopTo2; j++)
            //        {
            //            if (wpower[i] > wpower[warray[i - j]])
            //            {
            //                break;
            //            }
            //            else if (wpower[i] == wpower[warray[i - j]])
            //            {
            //                if (withBlock.Weapon(i).ENConsumption > 0)
            //                {
            //                    if (withBlock.Weapon(i).ENConsumption >= withBlock.Weapon(warray[i - j]).ENConsumption)
            //                    {
            //                        break;
            //                    }
            //                }
            //                else if (withBlock.Weapon(i).Bullet > 0)
            //                {
            //                    if (withBlock.Weapon(i).Bullet <= withBlock.Weapon(warray[i - j]).Bullet)
            //                    {
            //                        break;
            //                    }
            //                }
            //                else if (withBlock.Weapon((i - j)).ENConsumption == 0 & withBlock.Weapon(warray[i - j]).Bullet == 0)
            //                {
            //                    break;
            //                }
            //            }
            //        }

            //        var loopTo3 = (j - 1);
            //        for (k = 1; k <= loopTo3; k++)
            //            warray[i - k + 1] = warray[i - k];
            //        warray[i - j + 1] = i;
            //    }
            //}

            //list = new string[1];
            //wlist = new short[1];
            //if (lb_mode == "移動前" | lb_mode == "移動後" | lb_mode == "一覧")
            //{
            //    // 通常の武器選択時の表示
            //    var loopTo4 = u.CountWeapon();
            //    for (i = 1; i <= loopTo4; i++)
            //    {
            //        w = warray[i];
            //        {
            //            var withBlock1 = u;
            //            if (lb_mode == "一覧")
            //            {
            //                string argref_mode = "ステータス";
            //                if (!withBlock1.IsWeaponAvailable(w, argref_mode))
            //                {
            //                    // Disableコマンドで使用不可にされた武器と使用できない合体技
            //                    // は表示しない
            //                    if (withBlock1.IsDisabled(withBlock1.Weapon(w).Name))
            //                    {
            //                        goto NextLoop1;
            //                    }

            //                    if (!withBlock1.IsWeaponMastered(w))
            //                    {
            //                        goto NextLoop1;
            //                    }

            //                    string argattr = "合";
            //                    if (withBlock1.IsWeaponClassifiedAs(w, argattr))
            //                    {
            //                        if (!withBlock1.IsCombinationAttackAvailable(w, true))
            //                        {
            //                            goto NextLoop1;
            //                        }
            //                    }
            //                }

            //                ListItemFlag[Information.UBound(list) + 1] = false;
            //            }
            //            else if (withBlock1.IsWeaponUseful(w, lb_mode))
            //            {
            //                ListItemFlag[Information.UBound(list) + 1] = false;
            //            }
            //            else
            //            {
            //                // Disableコマンドで使用不可にされた武器と使用できない合体技
            //                // は表示しない
            //                if (withBlock1.IsDisabled(withBlock1.Weapon(w).Name))
            //                {
            //                    goto NextLoop1;
            //                }

            //                if (!withBlock1.IsWeaponMastered(w))
            //                {
            //                    goto NextLoop1;
            //                }

            //                string argattr1 = "合";
            //                if (withBlock1.IsWeaponClassifiedAs(w, argattr1))
            //                {
            //                    if (!withBlock1.IsCombinationAttackAvailable(w, true))
            //                    {
            //                        goto NextLoop1;
            //                    }
            //                }

            //                ListItemFlag[Information.UBound(list) + 1] = true;
            //            }
            //        }

            //        Array.Resize(list, Information.UBound(list) + 1 + 1);
            //        Array.Resize(wlist, Information.UBound(list) + 1);
            //        wlist[Information.UBound(list)] = w;

            //        // 各武器の表示内容の設定
            //        {
            //            var withBlock2 = u.Weapon(w);
            //            // 攻撃力
            //            if (wpower[w] < 10000)
            //            {
            //                string localLeftPaddedString() { string argbuf = SrcFormatter.Format(wpower[w]); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //                list[Information.UBound(list)] = GeneralLib.RightPaddedString(withBlock2.Nickname(), 27) + localLeftPaddedString();
            //            }
            //            else
            //            {
            //                string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(wpower[w]); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                list[Information.UBound(list)] = GeneralLib.RightPaddedString(withBlock2.Nickname(), 26) + localLeftPaddedString1();
            //            }

            //            // 最大射程
            //            if (u.WeaponMaxRange(w) > 1)
            //            {
            //                buf = SrcFormatter.Format(withBlock2.MinRange) + "-" + SrcFormatter.Format(u.WeaponMaxRange(w));
            //                list[Information.UBound(list)] = list[Information.UBound(list)] + GeneralLib.LeftPaddedString(buf, 5);
            //            }
            //            else
            //            {
            //                list[Information.UBound(list)] = list[Information.UBound(list)] + "    1";
            //            }

            //            // 命中率修正
            //            if (u.WeaponPrecision(w) >= 0)
            //            {
            //                string localLeftPaddedString2() { string argbuf = "+" + SrcFormatter.Format(u.WeaponPrecision(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString2();
            //            }
            //            else
            //            {
            //                string localLeftPaddedString3() { string argbuf = SrcFormatter.Format(u.WeaponPrecision(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString3();
            //            }

            //            // 残り弾数
            //            if (withBlock2.Bullet > 0)
            //            {
            //                string localLeftPaddedString4() { string argbuf = SrcFormatter.Format(u.Bullet(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString4();
            //            }
            //            else
            //            {
            //                list[Information.UBound(list)] = list[Information.UBound(list)] + "  -";
            //            }

            //            // ＥＮ消費量
            //            if (withBlock2.ENConsumption > 0)
            //            {
            //                string localLeftPaddedString5() { string argbuf = SrcFormatter.Format(u.WeaponENConsumption(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString5();
            //            }
            //            else
            //            {
            //                list[Information.UBound(list)] = list[Information.UBound(list)] + "   -";
            //            }

            //            // クリティカル率修正
            //            if (u.WeaponCritical(w) >= 0)
            //            {
            //                string localLeftPaddedString6() { string argbuf = "+" + SrcFormatter.Format(u.WeaponCritical(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString6();
            //            }
            //            else
            //            {
            //                string localLeftPaddedString7() { string argbuf = SrcFormatter.Format(u.WeaponCritical(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString7();
            //            }

            //            // 地形適応
            //            list[Information.UBound(list)] = list[Information.UBound(list)] + " " + withBlock2.Adaption;

            //            // 必要気力
            //            if (withBlock2.NecessaryMorale > 0)
            //            {
            //                list[Information.UBound(list)] = list[Information.UBound(list)] + " 気" + withBlock2.NecessaryMorale;
            //            }

            //            // 属性
            //            wclass = u.WeaponClass(w);
            //            string argstring21 = "|";
            //            if (GeneralLib.InStrNotNest(wclass, argstring21) > 0)
            //            {
            //                string argstring2 = "|";
            //                wclass = Strings.Left(wclass, GeneralLib.InStrNotNest(wclass, argstring2) - 1);
            //            }

            //            list[Information.UBound(list)] = list[Information.UBound(list)] + " " + wclass;
            //        }

            //    NextLoop1:
            //        ;
            //    }

            //    if (lb_mode == "移動前" | lb_mode == "移動後")
            //    {
            //        Unit argt = null;
            //        Unit argt1 = null;
            //        if (u.LookForSupportAttack(argt1) is object)
            //        {
            //            // 援護攻撃を使うかどうか選択
            //            Commands.UseSupportAttack = true;
            //            Array.Resize(list, Information.UBound(list) + 1 + 1);
            //            Array.Resize(ListItemFlag, Information.UBound(list) + 1);
            //            list[Information.UBound(list)] = "援護攻撃：使用する";
            //        }
            //    }

            //    // リストボックスを表示
            //    TopItem = -1;
            //    string argtname = "EN";
            //    string argtname1 = "CT";
            //    string arglb_info = "名称                       攻撃 射程  命 弾  " + Expression.Term(argtname, u, 2) + "  " + Expression.Term(argtname1, u, 2) + " 適応 分類";
            //    string arglb_mode = "表示のみ";
            //    ret = ListBox(caption_msg, list, arglb_info, arglb_mode);
            //    if (SRC.AutoMoveCursor)
            //    {
            //        if (lb_mode != "一覧")
            //        {
            //            string argcursor_mode = "武器選択";
            //            MoveCursorPos(argcursor_mode);
            //        }
            //        else
            //        {
            //            string argcursor_mode1 = "ダイアログ";
            //            MoveCursorPos(argcursor_mode1);
            //        }
            //    }

            if (!string.IsNullOrEmpty(BGM))
            {
                SRC.Sound.ChangeBGM(BGM);
            }

            //    while (true)
            //    {
            //        while (!IsFormClicked)
            //        {
            //            Application.DoEvents();
            //            // 右ボタンでのダブルクリックの実現
            //            if ((GetAsyncKeyState(RButtonID) & 0x8000) == 0)
            //            {
            //                is_rbutton_released = true;
            //            }
            //            else if (is_rbutton_released)
            //            {
            //                IsFormClicked = true;
            //            }
            //        }

            //        if (Commands.SelectedItem <= Information.UBound(wlist))
            //        {
            //            break;
            //        }
            //        else
            //        {
            //            // 援護攻撃のオン/オフ切り替え
            //            Commands.UseSupportAttack = !Commands.UseSupportAttack;
            //            if (Commands.UseSupportAttack)
            //            {
            //                list[Information.UBound(list)] = "援護攻撃：使用する";
            //            }
            //            else
            //            {
            //                list[Information.UBound(list)] = "援護攻撃：使用しない";
            //            }

            //            string argtname2 = "EN";
            //            string argtname3 = "CT";
            //            string arglb_info1 = "名称                       攻撃 射程  命 弾  " + Expression.Term(argtname2, u, 2) + "  " + Expression.Term(argtname3, u, 2) + " 適応 分類";
            //            string arglb_mode1 = "表示のみ";
            //            Commands.SelectedItem = ListBox(caption_msg, list, arglb_info1, arglb_mode1);
            //        }
            //    }

            //    if (lb_mode != "一覧")
            //    {
            //        My.MyProject.Forms.frmListBox.Hide();
            //    }

            //    ListItemComment = new string[1];
            //    WeaponListBoxRet = wlist[Commands.SelectedItem];
            //}
            //else if (lb_mode == "反撃")
            //{
            //    // 反撃武器選択時の表示

            //    var loopTo5 = u.CountWeapon();
            //    for (i = 1; i <= loopTo5; i++)
            //    {
            //        w = warray[i];
            //        {
            //            var withBlock3 = u;
            //            // Disableコマンドで使用不可にされた武器は表示しない
            //            if (withBlock3.IsDisabled(withBlock3.Weapon(w).Name))
            //            {
            //                goto NextLoop2;
            //            }

            //            // 必要技能を満たさない武器は表示しない
            //            if (!withBlock3.IsWeaponMastered(w))
            //            {
            //                goto NextLoop2;
            //            }

            //            // 使用できない合体技は表示しない
            //            string argattr2 = "合";
            //            if (withBlock3.IsWeaponClassifiedAs(w, argattr2))
            //            {
            //                if (!withBlock3.IsCombinationAttackAvailable(w, true))
            //                {
            //                    goto NextLoop2;
            //                }
            //            }

            //            string argref_mode1 = "移動前";
            //            string argattr3 = "Ｍ";
            //            string argattr4 = "合";
            //            if (!withBlock3.IsWeaponAvailable(w, argref_mode1))
            //            {
            //                // この武器は使用不能
            //                ListItemFlag[Information.UBound(list) + 1] = true;
            //            }
            //            else if (!withBlock3.IsTargetWithinRange(w, Commands.SelectedUnit))
            //            {
            //                // ターゲットが射程外
            //                ListItemFlag[Information.UBound(list) + 1] = true;
            //            }
            //            else if (withBlock3.IsWeaponClassifiedAs(w, argattr3))
            //            {
            //                // マップ攻撃は武器選定外
            //                ListItemFlag[Information.UBound(list) + 1] = true;
            //            }
            //            else if (withBlock3.IsWeaponClassifiedAs(w, argattr4))
            //            {
            //                // 合体技は自分から攻撃をかける場合にのみ使用
            //                ListItemFlag[Information.UBound(list) + 1] = true;
            //            }
            //            else if (withBlock3.Damage(w, Commands.SelectedUnit, true) > 0)
            //            {
            //                // ダメージを与えられる
            //                ListItemFlag[Information.UBound(list) + 1] = false;
            //            }
            //            else if (!withBlock3.IsNormalWeapon(w) & withBlock3.CriticalProbability(w, Commands.SelectedUnit) > 0)
            //            {
            //                // 特殊効果を与えられる
            //                ListItemFlag[Information.UBound(list) + 1] = false;
            //            }
            //            else
            //            {
            //                // この武器は効果が無い
            //                ListItemFlag[Information.UBound(list) + 1] = true;
            //            }
            //        }

            //        Array.Resize(list, Information.UBound(list) + 1 + 1);
            //        Array.Resize(wlist, Information.UBound(list) + 1);
            //        wlist[Information.UBound(list)] = w;

            //        // 各武器の表示内容の設定
            //        {
            //            var withBlock4 = u.Weapon(w);
            //            // 攻撃力
            //            string localLeftPaddedString8() { string argbuf = SrcFormatter.Format(wpower[w]); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //            list[Information.UBound(list)] = GeneralLib.RightPaddedString(withBlock4.Nickname(), 29) + localLeftPaddedString8();

            //            // 命中率
            //            string argoname = "予測命中率非表示";
            //            if (!Expression.IsOptionDefined(argoname))
            //            {
            //                buf = SrcFormatter.Format(GeneralLib.MinLng(u.HitProbability(w, Commands.SelectedUnit, true), 100)) + "%";
            //                list[Information.UBound(list)] = list[Information.UBound(list)] + GeneralLib.LeftPaddedString(buf, 5);
            //            }
            //            else if (u.WeaponPrecision(w) >= 0)
            //            {
            //                string localLeftPaddedString10() { string argbuf = "+" + SrcFormatter.Format(u.WeaponPrecision(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString10();
            //            }
            //            else
            //            {
            //                string localLeftPaddedString9() { string argbuf = SrcFormatter.Format(u.WeaponPrecision(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString9();
            //            }


            //            // クリティカル率
            //            string argoname1 = "予測命中率非表示";
            //            if (!Expression.IsOptionDefined(argoname1))
            //            {
            //                buf = SrcFormatter.Format(GeneralLib.MinLng(u.CriticalProbability(w, Commands.SelectedUnit), 100)) + "%";
            //                list[Information.UBound(list)] = list[Information.UBound(list)] + GeneralLib.LeftPaddedString(buf, 5);
            //            }
            //            else if (u.WeaponCritical(w) >= 0)
            //            {
            //                string localLeftPaddedString12() { string argbuf = "+" + SrcFormatter.Format(u.WeaponCritical(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString12();
            //            }
            //            else
            //            {
            //                string localLeftPaddedString11() { string argbuf = SrcFormatter.Format(u.WeaponCritical(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString11();
            //            }

            //            // 残り弾数
            //            if (withBlock4.Bullet > 0)
            //            {
            //                string localLeftPaddedString13() { string argbuf = SrcFormatter.Format(u.Bullet(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString13();
            //            }
            //            else
            //            {
            //                list[Information.UBound(list)] = list[Information.UBound(list)] + "  -";
            //            }

            //            // ＥＮ消費量
            //            if (withBlock4.ENConsumption > 0)
            //            {
            //                string localLeftPaddedString14() { string argbuf = SrcFormatter.Format(u.WeaponENConsumption(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString14();
            //            }
            //            else
            //            {
            //                list[Information.UBound(list)] = list[Information.UBound(list)] + "   -";
            //            }

            //            // 地形適応
            //            list[Information.UBound(list)] = list[Information.UBound(list)] + " " + withBlock4.Adaption;

            //            // 必要気力
            //            if (withBlock4.NecessaryMorale > 0)
            //            {
            //                list[Information.UBound(list)] = list[Information.UBound(list)] + " 気" + withBlock4.NecessaryMorale;
            //            }

            //            // 属性
            //            wclass = u.WeaponClass(w);
            //            string argstring23 = "|";
            //            if (GeneralLib.InStrNotNest(wclass, argstring23) > 0)
            //            {
            //                string argstring22 = "|";
            //                wclass = Strings.Left(wclass, GeneralLib.InStrNotNest(wclass, argstring22) - 1);
            //            }

            //            list[Information.UBound(list)] = list[Information.UBound(list)] + " " + wclass;
            //        }

            //    NextLoop2:
            //        ;
            //    }

            //    // リストボックスを表示
            //    TopItem = -1;
            //    string argtname4 = "CT";
            //    string argtname5 = "EN";
            //    string arglb_info2 = "名称                         攻撃 命中 " + Expression.Term(argtname4, u, 2) + "   弾  " + Expression.Term(argtname5, u, 2) + " 適応 分類";
            //    string arglb_mode2 = "連続表示,カーソル移動";
            //    ret = ListBox(caption_msg, list, arglb_info2, arglb_mode2);
            //    WeaponListBoxRet = wlist[ret];
            //}

            TopItem = -1;
            var list = weapons.Select(x => new ListBoxItem()
            {
                Text = $"{x.Name}",
                ListItemComment = "",
                ListItemFlag = !x.IsWeaponAvailable(lb_mode),
                ListItemID = "",
            }).ToList();
            var ret = ListBox(new ListBoxArgs
            {
                Items = list,
                HasFlag = true,
                lb_caption = caption_msg,
                //"名称                         攻撃 命中 " + Expression.Term(argtname4, u, 2) + "   弾  " + Expression.Term(argtname5, u, 2) + " 適応 分類",
                lb_info = "名称                         攻撃 命中 CT   弾  EN 適応 分類",
                lb_mode = "",
            });
            //var WeaponListBoxRet = wlist[ret];
            var WeaponListBoxRet = ret;
            Application.DoEvents();
            return WeaponListBoxRet;
        }

        public int AbilityListBox(Unit u, string caption_msg, string lb_mode, bool is_item)
        {
            throw new NotImplementedException();
        }

        public int LIPS(string lb_caption, string[] list, string lb_info, int time_limit)
        {
            throw new NotImplementedException();
        }

        public int MultiColumnListBox(string lb_caption, string[] list, bool is_center)
        {
            throw new NotImplementedException();
        }

        public int MultiSelectListBox(string lb_caption, string[] list, string lb_info, int max_num)
        {
            throw new NotImplementedException();
        }

        public void MakePicBuf()
        {
            throw new NotImplementedException();
        }

        public void DrawSysString(int X, int Y, string msg, bool without_refresh)
        {
            throw new NotImplementedException();
        }

        public void SaveScreen()
        {
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

        public void SaveCursorPos()
        {
            throw new NotImplementedException();
        }

        public void MoveCursorPos(string cursor_mode, Unit t)
        {
            throw new NotImplementedException();
        }

        public void RestoreCursorPos()
        {
            throw new NotImplementedException();
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
            frmNowLoading.Progress();
            Application.DoEvents();
        }

        public void SetLoadImageSize(int new_size)
        {
            frmNowLoading.Value = 0;
            frmNowLoading.Max = new_size;
        }

        public void ChangeDisplaySize(int w, int h)
        {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        public bool IsRButtonPressed(bool ignore_message_wait = false)
        {
            // メッセージがウエイト無しならスキップ
            if (!ignore_message_wait & MessageWait == 0)
            {
                return true;
            }

            // TODO Impl ネイティブAPIでシビアに取る？
            if (Control.MouseButtons.HasFlag(MouseButtons.Right))
            {
                return true;
            }
            //// メインウインドウ上でマウスボタンを押した場合
            //if (MainForm.Handle.ToInt32() == GetForegroundWindow())
            //{
            //    GetCursorPos(PT);
            //    {
            //        var withBlock = MainForm;
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X & PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX() & (long)SrcFormatter.PixelsToTwipsY(withBlock.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y & PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GetAsyncKeyState(RButtonID) & 0x8000) != 0)
            //            {
            //                // 右ボタンでスキップ
            //                return true;
            //            }
            //        }
            //    }
            //}
            //// メッセージウインドウ上でマウスボタンを押した場合
            //else if (My.MyProject.Forms.frmMessage.Handle.ToInt32() == GetForegroundWindow())
            //{
            //    GetCursorPos(PT);
            //    {
            //        var withBlock1 = My.MyProject.Forms.frmMessage;
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock1.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X & PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX() & (long)SrcFormatter.PixelsToTwipsY(withBlock1.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y & PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock1.Top) + SrcFormatter.PixelsToTwipsY(withBlock1.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GetAsyncKeyState(RButtonID) & 0x8000) != 0)
            //            {
            //                // 右ボタンでスキップ
            //                return true;
            //            }
            //        }
            //    }
            //}

            return false;
        }

        public void DisplayTelop(string msg)
        {
            Console.WriteLine("DisplayTelop: " + msg);
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

            var buttons = MessageBoxButtons.OKCancel;
            MessageBoxIcon? icon = option.HasFlag(GuiConfirmOption.Question) ? MessageBoxIcon.Question : null;

            DialogResult res;
            if (icon.HasValue)
            {
                res = MessageBox.Show(message, title, buttons, icon.Value);

            }
            else
            {
                res = MessageBox.Show(message, title, buttons);
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
    }
}
