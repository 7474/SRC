using Microsoft.Extensions.Logging;
using SRCCore;
using SRCCore.Commands;
using SRCCore.Lib;
using SRCCore.Units;
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
            // TODO Impl AddPartsToListBox
            //throw new NotImplementedException();
        }

        public void RemovePartsOnListBox()
        {
            // TODO Impl AddPartsToListBox
            //throw new NotImplementedException();
        }

        public UnitWeapon WeaponListBox(Unit u, UnitWeaponList weapons, string caption_msg, string lb_mode, string BGM)
        {
            // TODO Impl WeaponListBox
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
            //                else if (withBlock.Weapon((i - j)).ENConsumption == 0 && withBlock.Weapon(warray[i - j]).Bullet == 0)
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
            //if (lb_mode == "移動前" || lb_mode == "移動後" || lb_mode == "一覧")
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

            //    if (lb_mode == "移動前" || lb_mode == "移動後")
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
            //            if ((GetAsyncKeyState(RButtonID) && 0x8000) == 0)
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
            //            else if (!withBlock3.IsNormalWeapon(w) && withBlock3.CriticalProbability(w, Commands.SelectedUnit) > 0)
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
            // TODO Impl AbilityListBox
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
            if (!ignore_message_wait && MessageWait == 0)
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
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X && PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock.Left) + SrcFormatter.PixelsToTwipsX(withBlock.Width)) / (long)SrcFormatter.TwipsPerPixelX() && (long)SrcFormatter.PixelsToTwipsY(withBlock.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y && PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock.Top) + SrcFormatter.PixelsToTwipsY(withBlock.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GetAsyncKeyState(RButtonID) && 0x8000) != 0)
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
            //        if ((long)SrcFormatter.PixelsToTwipsX(withBlock1.Left) / (long)SrcFormatter.TwipsPerPixelX() <= PT.X && PT.X <= (long)(SrcFormatter.PixelsToTwipsX(withBlock1.Left) + SrcFormatter.PixelsToTwipsX(withBlock1.Width)) / (long)SrcFormatter.TwipsPerPixelX() && (long)SrcFormatter.PixelsToTwipsY(withBlock1.Top) / (long)SrcFormatter.TwipsPerPixelY() <= PT.Y && PT.Y <= (long)(SrcFormatter.PixelsToTwipsY(withBlock1.Top) + SrcFormatter.PixelsToTwipsY(withBlock1.Height)) / (long)SrcFormatter.TwipsPerPixelY())
            //        {
            //            if ((GetAsyncKeyState(RButtonID) && 0x8000) != 0)
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

        public Stream SelectSaveStream(SRCSaveKind saveKind, string defaultName)
        {
            // TODO Impl データセーブ
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
            //return File.OpenWrite(fname);
        }

        public Stream OpenQuikSaveStream(FileAccess fileAccess)
        {
            throw new NotImplementedException();
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
    }
}
