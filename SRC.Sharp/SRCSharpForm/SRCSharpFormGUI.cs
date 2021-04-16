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
                //        LoadUnitBitmap(lu, frmMessage.picUnit1, 0, 0, true, fname: "");
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
                        buf = GeneralLib.LeftPaddedString(SrcFormatter.Format(lu.HP), GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxHP)), 5));
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
                        buf = GeneralLib.LeftPaddedString(SrcFormatter.Format(lu.EN), GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxEN)), 3));
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
                //        LoadUnitBitmap(ru, frmMessage.picUnit2, 0, 0, true, fname: "");
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
                        buf = GeneralLib.LeftPaddedString(SrcFormatter.Format(ru.HP), GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxHP)), 5));
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
                        buf = GeneralLib.LeftPaddedString(SrcFormatter.Format(ru.EN), GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxEN)), 3));
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
                                buf = GeneralLib.LeftPaddedString(SrcFormatter.Format(tmp), GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxHP)), 5));
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
                                buf = GeneralLib.LeftPaddedString(SrcFormatter.Format(tmp), GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(lu.MaxEN)), 3));
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
                                buf = GeneralLib.LeftPaddedString(SrcFormatter.Format(tmp), GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxHP)), 5));
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
                                buf = GeneralLib.LeftPaddedString(SrcFormatter.Format(tmp), GeneralLib.MinLng(Strings.Len(SrcFormatter.Format(ru.MaxEN)), 3));
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
            Expression.FormatMessage(ref tmpMsg);

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
                if (SRC.PList.IsDefined(pname))
                {
                    var p = SRC.PList.Item(pname);
                    pnickname = p.get_Nickname(false);
                    fname = p.get_Bitmap(false);
                }
                else if (SRC.PDList.IsDefined(pname))
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
                        if (DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "メッセージ " + msg_mode))
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
                            //if (SRC.PList.IsDefined(pname))
                            //{
                            //    {
                            //        var withBlock = SRC.PList.Item(pname);
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

        private Font defaultFont = new Font("ＭＳ Ｐ明朝", 12, FontStyle.Regular, GraphicsUnit.Point);
        private Brush defaultFontColor = Brushes.Black;
        public void PrintMessage(string msg, bool is_sys_msg)
        {
            using var g = frmMessage.picMessage.NewImageIfNull().CreateGraphics();
            var currentFont = defaultFont;
            var currentFontColor = defaultFontColor;
            var currentPoint = new PointF(1, 1);
            var max_y = 1f;

            string cname;
            var in_tag = false;
            var escape_depth = 0;
            var head = 1;
            var loopTo = Strings.Len(msg);
            for (var i = 1; i <= loopTo; i++)
            {
                var ch = Strings.Mid(msg, i, 1);

                // システムメッセージの時のみエスケープシーケンスの処理を行う
                if (is_sys_msg)
                {
                    switch (ch ?? "")
                    {
                        case "[":
                            {
                                escape_depth = (escape_depth + 1);
                                if (escape_depth == 1)
                                {
                                    // エスケープシーケンス開始
                                    // それまでの文字列を出力
                                    var m = Strings.Mid(msg, head, i - head);
                                    g.DrawString(m, currentFont, currentFontColor, currentPoint);
                                    currentPoint = currentPoint.AddX(MessageLen(m, g, currentFont).Width);
                                    head = (i + 1);
                                    goto NextChar;
                                }

                                break;
                            }

                        case "]":
                            {
                                escape_depth = (escape_depth - 1);
                                if (escape_depth == 0)
                                {
                                    // エスケープシーケンス終了
                                    // エスケープシーケンスを出力
                                    var m = Strings.Mid(msg, head, i - head);
                                    g.DrawString(m, currentFont, currentFontColor, currentPoint);
                                    currentPoint = currentPoint.AddX(MessageLen(m, g, currentFont).Width);
                                    head = (i + 1);
                                    goto NextChar;
                                }

                                break;
                            }
                    }
                }

                // タグの処理
                switch (ch ?? "")
                {
                    case "<":
                        {
                            if (!in_tag & escape_depth == 0)
                            {
                                // タグ開始
                                in_tag = true;
                                // それまでの文字列を出力
                                var m = Strings.Mid(msg, head, i - head);
                                g.DrawString(m, currentFont, currentFontColor, currentPoint);
                                currentPoint = currentPoint.AddX(MessageLen(m, g, currentFont).Width);
                                head = (i + 1);
                                goto NextChar;
                            }

                            break;
                        }

                    case ">":
                        {
                            if (in_tag)
                            {
                                // タグ終了
                                in_tag = false;

                                // タグの切り出し
                                var tag = Strings.LCase(Strings.Mid(msg, head, i - head));

                                // タグに合わせて各種処理を行う
                                switch (tag ?? "")
                                {
                                    case "b":
                                        {
                                            currentFont = currentFont.Bold();
                                            break;
                                        }

                                    case "/b":
                                        {
                                            currentFont = currentFont.UnBold();
                                            break;
                                        }

                                    case "i":
                                        {
                                            currentFont = currentFont.Italic();
                                            break;
                                        }

                                    case "/i":
                                        {
                                            currentFont = currentFont.UnItalic();
                                            break;
                                        }

                                    case "big":
                                        {
                                            currentFont = currentFont.ReSize(currentFont.SizeInPoints + 2f);
                                            if (currentPoint.Y + currentFont.GetHeight(g) > max_y)
                                            {
                                                max_y = currentPoint.Y + currentFont.GetHeight(g);
                                            }
                                            break;
                                        }

                                    case "/big":
                                        {
                                            currentFont = currentFont.ReSize(currentFont.SizeInPoints - 2f);
                                            break;
                                        }

                                    case "small":
                                        {
                                            currentFont = currentFont.ReSize(currentFont.SizeInPoints - 2f);
                                            if (currentPoint.Y + currentFont.GetHeight(g) > max_y)
                                            {
                                                max_y = currentPoint.Y + currentFont.GetHeight(g);
                                            }
                                            break;
                                        }

                                    case "/small":
                                        {
                                            currentFont = currentFont.ReSize(currentFont.SizeInPoints + 2f);
                                            break;
                                        }

                                    case "/color":
                                        {
                                            currentFontColor = defaultFontColor;
                                            break;
                                        }

                                    case "/size":
                                        {
                                            currentFont = currentFont.ReSize(defaultFont.SizeInPoints);
                                            break;
                                        }

                                    case "lt":
                                        {
                                            g.DrawString("<", currentFont, currentFontColor, currentPoint);
                                            currentPoint = currentPoint.AddX(MessageLen("<", g, currentFont).Width);
                                            break;
                                        }

                                    case "gt":
                                        {
                                            g.DrawString(">", currentFont, currentFontColor, currentPoint);
                                            currentPoint = currentPoint.AddX(MessageLen(">", g, currentFont).Width);
                                            break;
                                        }

                                    default:
                                        {
                                            if (Strings.InStr(tag, "color=") == 1)
                                            {
                                                // 色設定
                                                cname = Expression.GetValueAsString(Strings.Mid(tag, 7));
                                                switch (cname ?? "")
                                                {
                                                    case "black":
                                                        currentFontColor = Brushes.Black;
                                                        break;

                                                    case "gray":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0x80, 0x80, 0x80));
                                                        break;

                                                    case "silver":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0xC0, 0xC0, 0xC0));
                                                        break;

                                                    case "white":
                                                        currentFontColor = Brushes.White;
                                                        break;

                                                    case "red":
                                                        currentFontColor = Brushes.Red;
                                                        break;

                                                    case "yellow":
                                                        currentFontColor = Brushes.Yellow;
                                                        break;

                                                    case "lime":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0x0, 0xFF, 0x0));
                                                        break;

                                                    case "aqua":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0x0, 0xFF, 0xFF));
                                                        break;

                                                    case "blue":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0x0, 0x0, 0xFF));
                                                        break;

                                                    case "fuchsia":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0xFF, 0x0, 0xFF));
                                                        break;

                                                    case "maroon":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0x80, 0x0, 0x0));
                                                        break;

                                                    case "olive":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0x80, 0x80, 0x0));
                                                        break;

                                                    case "green":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0x0, 0x80, 0x0));
                                                        break;

                                                    case "teal":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0x0, 0x80, 0x80));
                                                        break;

                                                    case "navy":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0x0, 0x0, 0x80));
                                                        break;

                                                    case "purple":
                                                        currentFontColor = new SolidBrush(Color.FromArgb(0x80, 0x0, 0x80));
                                                        break;

                                                        // TODO Impl color code
                                                        //default:
                                                        //    {
                                                        //        if (Strings.Asc(cname) == 35) // #
                                                        //        {
                                                        //            buf = new string(Conversions.ToChar(Constants.vbNullChar), 8);
                                                        //            StringType.MidStmtStr(ref buf, 1, 2, "&H");
                                                        //            var midTmp = Strings.Mid(cname, 6, 2);
                                                        //            StringType.MidStmtStr(ref buf, 3, 2, midTmp);
                                                        //            var midTmp1 = Strings.Mid(cname, 4, 2);
                                                        //            StringType.MidStmtStr(ref buf, 5, 2, midTmp1);
                                                        //            var midTmp2 = Strings.Mid(cname, 2, 2);
                                                        //            StringType.MidStmtStr(ref buf, 7, 2, midTmp2);
                                                        //            if (Information.IsNumeric(buf))
                                                        //            {
                                                        //                p.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(buf));
                                                        //            }
                                                        //        }

                                                        //        break;
                                                        //    }
                                                }
                                            }
                                            else if (Strings.InStr(tag, "size=") == 1)
                                            {
                                                // サイズ設定
                                                if (Information.IsNumeric(Strings.Mid(tag, 6)))
                                                {
                                                    currentFont = currentFont.ReSize(Conversions.ToInteger(Strings.Mid(tag, 6)));
                                                    if (currentPoint.Y + currentFont.GetHeight(g) > max_y)
                                                    {
                                                        max_y = currentPoint.Y + currentFont.GetHeight(g);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // タグではないのでそのまま書き出す
                                                var m = Strings.Mid(msg, head - 1, i - head + 2);
                                                g.DrawString(m, currentFont, currentFontColor, currentPoint);
                                                currentPoint = currentPoint.AddX(MessageLen(m, g, currentFont).Width);
                                            }

                                            break;
                                        }
                                }

                                head = (i + 1);
                                goto NextChar;
                            }

                            break;
                        }
                }

            NextChar:
                ;
            }

            // 終了していないタグ、もしくはエスケープシーケンスはただの文字列と見なす
            if (in_tag | escape_depth > 0)
            {
                head = (head - 1);
            }

            // 未出力の文字列を出力する
            if (head <= Strings.Len(msg))
            {
                if (Strings.Right(msg, 1) == "」")
                {
                    // 最後の括弧の位置は一番大きなサイズの文字に合わせる
                    {
                        var m = Strings.Mid(msg, head, Strings.Len(msg) - head);
                        g.DrawString(m, currentFont, currentFontColor, currentPoint);
                        currentPoint = currentPoint.AddX(MessageLen(m, g, currentFont).Width);
                    }
                    {
                        var m = Strings.Mid(msg, head);
                        g.DrawString(m, currentFont, currentFontColor, currentPoint.X, max_y - currentFont.GetHeight(g));
                        currentPoint = currentPoint.AddX(MessageLen(m, g, currentFont).Width);
                    }
                }
                else
                {
                    var m = Strings.Mid(msg, head);
                    g.DrawString(m, currentFont, currentFontColor, currentPoint);
                    currentPoint = currentPoint.AddX(MessageLen(m, g, currentFont).Width);
                }
            }
            else
            {
                // 未出力の文字列がない場合は改行のみ
                currentPoint = currentPoint.AddX(currentFont.GetHeight(g));
            }

            // 改行後の位置は一番大きなサイズの文字に合わせる
            if (max_y > currentPoint.Y)
            {
                currentPoint.Y = max_y + 1;
            }
            else
            {
                currentPoint.Y = currentPoint.Y + 1;
            }
            currentPoint.X = 1;
        }

        // メッセージ幅を計算(タグを無視して)
        private SizeF MessageLen(string msg, Graphics g, Font font)
        {
            // タグが存在する？
            var ret = Strings.InStr(msg, "<");
            if (ret == 0)
            {
                return g.MeasureString(msg, font);
            }

            var buf = "";
            // タグを除いたメッセージを作成
            while (ret > 0)
            {
                buf = buf + Strings.Left(msg, ret - 1);
                msg = Strings.Mid(msg, ret + 1);
                ret = Strings.InStr(msg, ">");
                if (ret > 0)
                {
                    msg = Strings.Mid(msg, ret + 1);
                }
                else
                {
                    msg = "";
                }

                ret = Strings.InStr(msg, "<");
            }

            buf = buf + msg;

            // タグ抜きメッセージのピクセル幅を計算
            return g.MeasureString(buf, font);
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
            throw new NotImplementedException();
        }

        public void RemovePartsOnListBox()
        {
            throw new NotImplementedException();
        }

        public UnitWeapon WeaponListBox(Unit u, UnitWeaponList weapons, string caption_msg, string lb_mode, string BGM)
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
            //        wpower[i] = withBlock.WeaponPower(i, "");
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
            //                if (!withBlock1.IsWeaponAvailable(w, "ステータス"))
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

            //                    if (withBlock1.IsWeaponClassifiedAs(w, "合"))
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

            //                if (withBlock1.IsWeaponClassifiedAs(w, "合"))
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
            //            if (GeneralLib.InStrNotNest(wclass, "|") > 0)
            //            {
            //                wclass = Strings.Left(wclass, GeneralLib.InStrNotNest(wclass, "|") - 1);
            //            }

            //            list[Information.UBound(list)] = list[Information.UBound(list)] + " " + wclass;
            //        }

            //    NextLoop1:
            //        ;
            //    }

            //    if (lb_mode == "移動前" | lb_mode == "移動後")
            //    {
            //        if (u.LookForSupportAttack(null) is object)
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
            //    ret = ListBox(caption_msg, list, "名称                       攻撃 射程  命 弾  " + Expression.Term(argtname, u, 2) + "  " + Expression.Term(argtname1, u, 2) + " 適応 分類", "表示のみ");
            //    if (SRC.AutoMoveCursor)
            //    {
            //        if (lb_mode != "一覧")
            //        {
            //            MoveCursorPos("武器選択");
            //        }
            //        else
            //        {
            //            MoveCursorPos("ダイアログ");
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

            //            Commands.SelectedItem = ListBox(caption_msg, list, "名称                       攻撃 射程  命 弾  " + Expression.Term(argtname2, u, 2) + "  " + Expression.Term(argtname3, u, 2) + " 適応 分類", "表示のみ");
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
            //            if (withBlock3.IsWeaponClassifiedAs(w, "合"))
            //            {
            //                if (!withBlock3.IsCombinationAttackAvailable(w, true))
            //                {
            //                    goto NextLoop2;
            //                }
            //            }

            //            if (!withBlock3.IsWeaponAvailable(w, "移動前"))
            //            {
            //                // この武器は使用不能
            //                ListItemFlag[Information.UBound(list) + 1] = true;
            //            }
            //            else if (!withBlock3.IsTargetWithinRange(w, Commands.SelectedUnit))
            //            {
            //                // ターゲットが射程外
            //                ListItemFlag[Information.UBound(list) + 1] = true;
            //            }
            //            else if (withBlock3.IsWeaponClassifiedAs(w, "Ｍ"))
            //            {
            //                // マップ攻撃は武器選定外
            //                ListItemFlag[Information.UBound(list) + 1] = true;
            //            }
            //            else if (withBlock3.IsWeaponClassifiedAs(w, "合"))
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
            //            if (!Expression.IsOptionDefined("予測命中率非表示"))
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
            //            if (!Expression.IsOptionDefined("予測命中率非表示"))
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
            //            if (GeneralLib.InStrNotNest(wclass, "|") > 0)
            //            {
            //                wclass = Strings.Left(wclass, GeneralLib.InStrNotNest(wclass, "|") - 1);
            //            }

            //            list[Information.UBound(list)] = list[Information.UBound(list)] + " " + wclass;
            //        }

            //    NextLoop2:
            //        ;
            //    }

            //    // リストボックスを表示
            //    TopItem = -1;
            //    ret = ListBox(caption_msg, list, "名称                         攻撃 命中 " + Expression.Term("CT", u, 2) + "   弾  " + Expression.Term(argtname5, u, 2) + " 適応 分類", "連続表示,カーソル移動");
            //    WeaponListBoxRet = wlist[ret];
            //}

            TopItem = -1;
            var list = weapons.Items.Select(x => new ListBoxItem()
            {
                Text = $"{x.Weapon.Name}" +
                    $" {x.Weapon.WeaponPower("")}" +
                    $" {x.Weapon.WeaponPrecision()}" +
                    $" {x.Weapon.WeaponCritical()}" +
                    $" {x.Weapon.WeaponENConsumption()}" +
                    $" {x.Weapon.UpdatedWeaponData.Adaption}" +
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
            // TODO Impl
            TopItem = -1;
            var list = abilities.Items.Select(x => new ListBoxItem()
            {
                Text = $"{x.Ability.Data.Name}",
                ListItemComment = "",
                ListItemFlag = !x.CanUse,
                ListItemID = "",
            }).ToList();
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

            var buttons = option.HasFlag(GuiConfirmOption.OkCancel)
                ? MessageBoxButtons.OKCancel
                : MessageBoxButtons.OK;
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

        public void Configure()
        {
            using (var dialog = new frmConfiguration())
            {
                dialog.SRC = SRC;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ShowDialog(MainForm);
            }
        }
    }
}
