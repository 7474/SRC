using SRCCore;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using SRCSharpForm.Extensions;
using SRCSharpForm.Lib;
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace SRCSharpForm
{
    public partial class SRCSharpFormGUI
    {
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
                // XXX 
                //// メインウィンドウが表示されていない場合は画面中央に表示
                //frmMessage.Left = SrcFormatter.TwipsToPixelsX((SrcFormatter.PixelsToTwipsX(Screen.PrimaryScreen.Bounds.Width) - SrcFormatter.PixelsToTwipsX(frmMessage.Width)) / 2d);
                //frmMessage.Top = SrcFormatter.TwipsToPixelsY((SrcFormatter.PixelsToTwipsY(Screen.PrimaryScreen.Bounds.Height) - SrcFormatter.PixelsToTwipsY(frmMessage.Height)) / 2d);
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
                frmMessage.picMessage.Top = 7;
                frmMessage.picMessage.Left = 84;
            }
            else if (u2 is null)
            {
                // ユニット表示１体のみ
                if (u1.Party == "味方" || u1.Party == "ＮＰＣ")
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
                frmMessage.picMessage.Top = 41;
                frmMessage.picMessage.Left = 84;
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
                if (!frmMessage.picUnit1.Visible && !frmMessage.picUnit2.Visible)
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
                if (u1.Party == "味方" || u1.Party == "ＮＰＣ")
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
            else if ((ReferenceEquals(u2, LeftUnit) || ReferenceEquals(u1, RightUnit)) && !ReferenceEquals(LeftUnit, RightUnit))
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
                //if (ru.HP != RightUnitHPRatio || ru.EN != RightUnitENRatio)
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
                        var tmp = ((lu.MaxHP * LeftUnitHPRatio * (num - i) + lu.HP * i) / num);
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
                        var tmp = ((lu.MaxEN * LeftUnitENRatio * (num - i) + lu.EN * i) / num);
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
                        var tmp = ((long)(ru.MaxHP * RightUnitHPRatio * (num - i) + ru.HP * i) / num);
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
                        var tmp = ((ru.MaxEN * RightUnitENRatio * (num - i) + ru.EN * i) / num);
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
            // TODO Impl SaveMessageFormStatus
        }

        public void KeepMessageFormStatus()
        {
            // TODO Impl KeepMessageFormStatus
        }

        public void DisplayMessage(string pname, string msg_org, string msg_mode)
        {
            var msg = msg_org;
            ResetMessage();
            try
            {
                string pnickname;
                string left_margin;
                DisplayMessagePilot(pname, msg_mode, out pnickname, out left_margin);

                // メッセージ中の式置換を処理
                Expression.FormatMessage(ref msg);
                msg = Strings.Trim(msg);

                // 末尾に強制改行が入っている場合は取り除く
                while (Strings.Right(msg, 1) == ";")
                {
                    msg = Strings.Left(msg, Strings.Len(msg) - 1);
                }

                // メッセージが空の場合はキャラ表示の描き換えのみ行う
                if (string.IsNullOrEmpty(msg))
                {
                    return;
                }

                var is_character_message = false;
                switch (pname ?? "")
                {
                    // そのまま使用
                    case "システム":
                        {
                            break;
                        }

                    case var @case when @case == "":
                        {
                            // 基本的にはそのまま使用するが、せりふ表示の代用の場合は
                            // せりふ表示用の処理を行う
                            var i = 0;
                            if (Strings.InStr(msg, "「") > 0 && Strings.Right(msg, 1) == "」")
                            {
                                i = Strings.InStr(msg, "「");
                            }
                            else if (Strings.InStr(msg, "『") > 0 && Strings.Right(msg, 1) == "』")
                            {
                                i = Strings.InStr(msg, "『");
                            }
                            else if (Strings.InStr(msg, "(") > 0 && Strings.Right(msg, 1) == ")")
                            {
                                i = Strings.InStr(msg, "(");
                            }
                            else if (Strings.InStr(msg, "（") > 0 && Strings.Right(msg, 1) == "）")
                            {
                                i = Strings.InStr(msg, "（");
                            }

                            if (i > 1)
                            {
                                var pcheck = Strings.Trim(Strings.Left(msg, i - 1));
                                if (i < 8 || SRC.PDList.IsDefined(pcheck) || SRC.NPDList.IsDefined(pcheck))
                                {
                                    is_character_message = true;
                                    if (!GeneralLib.IsSpace(Strings.Mid(msg, i - 1, 1)))
                                    {
                                        // "「"の前に半角スペースを挿入
                                        msg = Strings.Left(msg, i - 1) + " " + Strings.Mid(msg, i);
                                    }
                                }
                            }
                            break;
                        }

                    default:
                        {
                            is_character_message = true;
                            if ((Strings.Left(msg, 1) == "(" || Strings.Left(msg, 1) == "（") && (Strings.Right(msg, 1) == ")" || Strings.Right(msg, 1) == "）"))
                            {
                                // モノローグ
                                msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                                msg = string.Join("",
                                    pnickname,
                                    Expression.IsOptionDefined("会話パイロット名改行") ? ";" : " ",
                                    "（", msg, "）"
                                );
                            }
                            else if (Strings.Left(msg, 1) == "『" && Strings.Right(msg, 1) == "』")
                            {
                                msg = Strings.Mid(msg, 2, Strings.Len(msg) - 2);
                                msg = string.Join("",
                                    pnickname,
                                    Expression.IsOptionDefined("会話パイロット名改行") ? ";" : " ",
                                    "『", msg, "』"
                                );
                            }
                            else
                            {
                                // せりふ
                                msg = string.Join("",
                                    pnickname,
                                    Expression.IsOptionDefined("会話パイロット名改行") ? ";" : " ",
                                    "「", msg, "」"
                                );
                            }
                            break;
                        }
                }

                // 強制改行の位置を設定
                var mesConfig = new MessageConfig();
                if (Expression.IsOptionDefined("改行時余白短縮"))
                {
                    mesConfig.ConfigureShortSpace();
                }

                // メッセージを分割
                var messageParts = msg.Split(':').ToList();
                var messages = Enumerable.Range(1, messageParts.Count)
                    .Select(x => string.Join("", messageParts.Take(x)))
                    .ToList();

                // XXX 使ってないよなぁ？
                //// メッセージ長判定のため、元のメッセージを再構築
                //msg = string.Join("", messages);

                // メッセージの表示
                {
                    var msg_head = 1;
                    var prev_lnum = 0;
                    var i = -1;
                    while (++i < messages.Count)
                    {
                        var buf = messages[i];
                        var lnum = 0;
                        var line_head = msg_head;
                        var in_tag = false;

                        ResetMessageArea();
                        if (msg_head == 1)
                        {
                            ResetFont();
                        }
                        // メッセージの途中から表示
                        else if (is_character_message)
                        {
                            PrintString("  ");
                        }

                        var counter = msg_head;
                        var loopTo2 = Strings.Len(buf);
                        for (var j = counter; j <= loopTo2; j++)
                        {
                            var ch = Strings.Mid(buf, j, 1);

                            // ";"では必ず改行
                            if (ch == ";")
                            {
                                if (j != line_head)
                                {
                                    PrintMessage(Strings.Mid(buf, line_head, j - line_head), false);
                                    lnum = (lnum + 1);
                                    if (is_character_message && (lnum > 1 && Expression.IsOptionDefined("会話パイロット名改行") || lnum > 0 && !Expression.IsOptionDefined("会話パイロット名改行")))
                                    {
                                        PrintString(left_margin);
                                    }
                                }

                                line_head = (j + 1);
                                goto NextLoop;
                            }

                            // タグ内では改行しない
                            if (ch == "<")
                            {
                                in_tag = true;
                                goto NextLoop;
                            }
                            else if (ch == ">")
                            {
                                in_tag = false;
                            }
                            else if (in_tag)
                            {
                                goto NextLoop;
                            }

                            // メッセージが途切れてしまう場合は必ず改行
                            if (MessageLen(Strings.Mid(buf, line_head, j - line_head)).Width > MessageConfig.BrThresholdLimit * messageArea.Width)
                            {
                                PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), false);
                                lnum = (lnum + 1);
                                if (is_character_message && (lnum > 1 && Expression.IsOptionDefined("会話パイロット名改行") || lnum > 0 && !Expression.IsOptionDefined("会話パイロット名改行")))
                                {
                                    PrintString(left_margin);
                                }

                                line_head = (j + 1);
                                goto NextLoop;
                            }

                            // 禁則処理
                            switch (Strings.Mid(buf, j + 1, 1) ?? "")
                            {
                                case "。":
                                case "、":
                                case "…":
                                case "‥":
                                case "・":
                                case "･":
                                case "～":
                                case "ー":
                                case "－":
                                case "！":
                                case "？":
                                case "」":
                                case "』":
                                case "）":
                                case ")":
                                case " ":
                                case ";":
                                    {
                                        goto NextLoop;
                                    }
                            }

                            switch (Strings.Mid(buf, j + 2, 1) ?? "")
                            {
                                case "。":
                                case "、":
                                case "…":
                                case "‥":
                                case "・":
                                case "･":
                                case "～":
                                case "ー":
                                case "－":
                                case "！":
                                case "？":
                                case "」":
                                case "』":
                                case "）":
                                case ")":
                                case " ":
                                case ";":
                                    {
                                        goto NextLoop;
                                    }
                            }

                            if (Strings.Mid(buf, j + 3, 1) == ";")
                            {
                                goto NextLoop;
                            }

                            // 改行の判定
                            if (MessageLen(Strings.Mid(messages[i], line_head)).Width < MessageConfig.BrThresholdLimit * messageArea.Width)
                            {
                                // 全体が一行に収まる場合
                                goto NextLoop;
                            }

                            switch (ch ?? "")
                            {
                                case "。":
                                    {
                                        if (MessageLen(Strings.Mid(buf, line_head, j - line_head)).Width > mesConfig.BrThresholdKuten * messageArea.Width)
                                        {
                                            PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), false);
                                            lnum = (lnum + 1);
                                            if (is_character_message && (lnum > 1 && Expression.IsOptionDefined("会話パイロット名改行") || lnum > 0 && !Expression.IsOptionDefined("会話パイロット名改行")))
                                            {
                                                PrintString(left_margin);
                                            }

                                            line_head = (j + 1);
                                        }

                                        break;
                                    }

                                case "、":
                                    {
                                        if (MessageLen(Strings.Mid(buf, line_head, j - line_head)).Width > mesConfig.BrThresholdTouten * messageArea.Width)
                                        {
                                            PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), false);
                                            lnum = (lnum + 1);
                                            if (is_character_message && (lnum > 1 && Expression.IsOptionDefined("会話パイロット名改行") || lnum > 0 && !Expression.IsOptionDefined("会話パイロット名改行")))
                                            {
                                                PrintString(left_margin);
                                            }

                                            line_head = (j + 1);
                                        }

                                        break;
                                    }

                                case " ":
                                    {
                                        ch = Strings.Mid(buf, j - 1, 1);
                                        // スペースが文の区切りに使われているかどうか判定
                                        if (pname != "システム" && (ch == "！" || ch == "？" || ch == "…" || ch == "‥" || ch == "・" || ch == "･" || ch == "～"))
                                        {
                                            // 文の区切り
                                            if (MessageLen(Strings.Mid(buf, line_head, j - line_head)).Width > mesConfig.BrThresholdKuten * messageArea.Width)
                                            {
                                                PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), false);
                                                lnum = (lnum + 1);
                                                if (is_character_message && (lnum > 1 && Expression.IsOptionDefined("会話パイロット名改行") || lnum > 0 && !Expression.IsOptionDefined("会話パイロット名改行")))
                                                {
                                                    PrintString(left_margin);
                                                }

                                                line_head = (j + 1);
                                            }
                                        }
                                        // 単なる空白
                                        else if (MessageLen(Strings.Mid(buf, line_head, j - line_head)).Width > mesConfig.BrThresholdOver * messageArea.Width)
                                        {
                                            PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), false);
                                            lnum = (lnum + 1);
                                            if (is_character_message && (lnum > 1 && Expression.IsOptionDefined("会話パイロット名改行") || lnum > 0 && !Expression.IsOptionDefined("会話パイロット名改行")))
                                            {
                                                PrintString(left_margin);
                                            }

                                            line_head = (j + 1);
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        if (MessageLen(Strings.Mid(buf, line_head, j - line_head)).Width > mesConfig.BrThresholdOver * messageArea.Width)
                                        {
                                            PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), false);
                                            lnum = (lnum + 1);
                                            if (is_character_message && (lnum > 1 && Expression.IsOptionDefined("会話パイロット名改行") || lnum > 0 && !Expression.IsOptionDefined("会話パイロット名改行")))
                                            {
                                                PrintString(left_margin);
                                            }

                                            line_head = (j + 1);
                                        }

                                        break;
                                    }
                            }

                        NextLoop:
                            ;
                            if (lnum == 4)
                            {
                                if (j < Strings.Len(buf))
                                {
                                    msg_head = line_head;
                                    i = (i - 1);
                                    break;
                                }
                            }
                        }
                        // 残りの部分を表示
                        if (lnum < 4)
                        {
                            if (Strings.Len(buf) >= line_head)
                            {
                                PrintMessage(Strings.Mid(buf, line_head), false);
                            }
                        }
                        RefreshMessage();

                        Application.DoEvents();
                        if (MessageWait > 10000)
                        {
                            AutoMessageMode = false;
                        }

                        // ウィンドウのキャプションを設定
                        frmMessage.SetMessageModeCaption(AutoMessageMode);

                        // 次のメッセージ表示までの時間を設定(自動メッセージ送り用)
                        var start_time = GeneralLib.timeGetTime();
                        var wait_time = (lnum - prev_lnum + 2) * (MessageWait + 250);

                        // 次のメッセージ待ち
                        IsFormClicked = false;
                        var is_automode = AutoMessageMode;
                        while (!IsFormClicked)
                        {
                            if (AutoMessageMode)
                            {
                                if (start_time + wait_time < GeneralLib.timeGetTime())
                                {
                                    break;
                                }
                            }

                            if (IsRButtonPressed(true))
                            {
                                break;
                            }
                            Thread.Sleep(100);
                            Application.DoEvents();

                            // 自動送りモードが切り替えられた場合
                            if (is_automode != AutoMessageMode)
                            {
                                IsFormClicked = false;
                                is_automode = AutoMessageMode;
                                frmMessage.SetMessageModeCaption(AutoMessageMode);
                            }
                        }

                        // ウェイト計算用に既に表示した行数を記録
                        if (lnum < 4)
                        {
                            prev_lnum = lnum;
                        }
                        else
                        {
                            prev_lnum = 0;
                        }
                    }
                }
                // XXX 画像が見つからなかった時のエラー処理
            }
            finally
            {
                ResetMessage();
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
            else if (!string.IsNullOrEmpty(pname) && pname != "-")
            {
                // XXX DisplayMessage 時は "-" 条件ないけれど問題ないやろ
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

                // TODO out left_margin
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
        private Brush defaultBgColor = Brushes.White;
        private Brush defaultFontColor = Brushes.Black;

        private Font currentMessageFont;
        private Brush currentMessageFontColor;
        private PointF currentMessagePoint;

        private Size messageArea;

        private void ResetFont()
        {
            currentMessageFont = defaultFont;
            currentMessageFontColor = defaultFontColor;
        }

        private void ResetMessageArea()
        {
            messageArea = frmMessage.picMessage.Size;
            frmMessage.picMessage.ClearImage(defaultBgColor);
            currentMessagePoint = new PointF(0, 0);
        }

        private void ResetMessage()
        {
            ResetMessageArea();
            ResetFont();
        }

        private void CRMessage()
        {
            currentMessagePoint = new PointF(0, currentMessagePoint.Y);
        }

        private void RefreshMessage()
        {
            frmMessage.picMessage.Refresh();
        }

        public void PrintMessage(string msg, bool is_sys_msg)
        {
            using var g = Graphics.FromImage(frmMessage.picMessage.NewImageIfNull().Image);
            var max_y = currentMessagePoint.Y + currentMessageFont.GetHeight(g);

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
                                    PrintString(Strings.Mid(msg, head, i - head), g);
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
                                    PrintString(Strings.Mid(msg, head, i - head), g);
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
                            if (!in_tag && escape_depth == 0)
                            {
                                // タグ開始
                                in_tag = true;
                                // それまでの文字列を出力
                                PrintString(Strings.Mid(msg, head, i - head), g);
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
                                            currentMessageFont = currentMessageFont.Bold();
                                            break;
                                        }

                                    case "/b":
                                        {
                                            currentMessageFont = currentMessageFont.UnBold();
                                            break;
                                        }

                                    case "i":
                                        {
                                            currentMessageFont = currentMessageFont.Italic();
                                            break;
                                        }

                                    case "/i":
                                        {
                                            currentMessageFont = currentMessageFont.UnItalic();
                                            break;
                                        }

                                    case "big":
                                        {
                                            currentMessageFont = currentMessageFont.ReSize(currentMessageFont.SizeInPoints + 2f);
                                            if (currentMessagePoint.Y + currentMessageFont.GetHeight(g) > max_y)
                                            {
                                                max_y = currentMessagePoint.Y + currentMessageFont.GetHeight(g);
                                            }
                                            break;
                                        }

                                    case "/big":
                                        {
                                            currentMessageFont = currentMessageFont.ReSize(currentMessageFont.SizeInPoints - 2f);
                                            break;
                                        }

                                    case "small":
                                        {
                                            currentMessageFont = currentMessageFont.ReSize(currentMessageFont.SizeInPoints - 2f);
                                            if (currentMessagePoint.Y + currentMessageFont.GetHeight(g) > max_y)
                                            {
                                                max_y = currentMessagePoint.Y + currentMessageFont.GetHeight(g);
                                            }
                                            break;
                                        }

                                    case "/small":
                                        {
                                            currentMessageFont = currentMessageFont.ReSize(currentMessageFont.SizeInPoints + 2f);
                                            break;
                                        }

                                    case "/color":
                                        {
                                            currentMessageFontColor = defaultFontColor;
                                            break;
                                        }

                                    case "/size":
                                        {
                                            currentMessageFont = currentMessageFont.ReSize(defaultFont.SizeInPoints);
                                            break;
                                        }

                                    case "lt":
                                        {
                                            PrintString("<", g);
                                            break;
                                        }

                                    case "gt":
                                        {
                                            PrintString(">", g);
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
                                                        currentMessageFontColor = Brushes.Black;
                                                        break;

                                                    case "gray":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0x80, 0x80, 0x80));
                                                        break;

                                                    case "silver":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0xC0, 0xC0, 0xC0));
                                                        break;

                                                    case "white":
                                                        currentMessageFontColor = Brushes.White;
                                                        break;

                                                    case "red":
                                                        currentMessageFontColor = Brushes.Red;
                                                        break;

                                                    case "yellow":
                                                        currentMessageFontColor = Brushes.Yellow;
                                                        break;

                                                    case "lime":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0x0, 0xFF, 0x0));
                                                        break;

                                                    case "aqua":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0x0, 0xFF, 0xFF));
                                                        break;

                                                    case "blue":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0x0, 0x0, 0xFF));
                                                        break;

                                                    case "fuchsia":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0xFF, 0x0, 0xFF));
                                                        break;

                                                    case "maroon":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0x80, 0x0, 0x0));
                                                        break;

                                                    case "olive":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0x80, 0x80, 0x0));
                                                        break;

                                                    case "green":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0x0, 0x80, 0x0));
                                                        break;

                                                    case "teal":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0x0, 0x80, 0x80));
                                                        break;

                                                    case "navy":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0x0, 0x0, 0x80));
                                                        break;

                                                    case "purple":
                                                        currentMessageFontColor = new SolidBrush(Color.FromArgb(0x80, 0x0, 0x80));
                                                        break;

                                                    default:
                                                        {
                                                            if (Strings.Asc(cname) == 35 && cname.Length == 7) // #
                                                            {
                                                                Color color;
                                                                if (!ColorExtension.TryFromHexString(cname, out color))
                                                                {
                                                                    throw new Exception($"色の指定が不正です。[{cname}]");
                                                                }
                                                                currentMessageFontColor = new SolidBrush(color);
                                                            }
                                                            break;
                                                        }
                                                }
                                            }
                                            else if (Strings.InStr(tag, "size=") == 1)
                                            {
                                                // サイズ設定
                                                if (Information.IsNumeric(Strings.Mid(tag, 6)))
                                                {
                                                    currentMessageFont = currentMessageFont.ReSize(Conversions.ToInteger(Strings.Mid(tag, 6)));
                                                    if (currentMessagePoint.Y + currentMessageFont.GetHeight(g) > max_y)
                                                    {
                                                        max_y = currentMessagePoint.Y + currentMessageFont.GetHeight(g);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                // タグではないのでそのまま書き出す
                                                PrintString(Strings.Mid(msg, head - 1, i - head + 2), g);
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
            if (in_tag || escape_depth > 0)
            {
                head = (head - 1);
            }

            // 未出力の文字列を出力する
            if (head <= Strings.Len(msg))
            {
                if (Strings.Right(msg, 1) == "」")
                {
                    // 最後の括弧の位置は一番大きなサイズの文字に合わせる
                    PrintString(Strings.Mid(msg, head, Strings.Len(msg) - head), g);
                    currentMessagePoint = new PointF(currentMessagePoint.X, max_y - currentMessageFont.GetHeight(g));
                    PrintString(Strings.Right(msg, 1), g);
                    currentMessagePoint = currentMessagePoint.AddY(currentMessageFont.GetHeight(g));
                }
                else
                {
                    PrintString(Strings.Mid(msg, head), g);
                    currentMessagePoint = currentMessagePoint.AddY(currentMessageFont.GetHeight(g));
                }
            }
            else
            {
                // 未出力の文字列がない場合は改行のみ
                currentMessagePoint = currentMessagePoint.AddY(currentMessageFont.GetHeight(g));
            }

            // 改行後の位置は一番大きなサイズの文字に合わせる
            if (max_y > currentMessagePoint.Y)
            {
                currentMessagePoint.Y = max_y + 1;
            }
            else
            {
                currentMessagePoint.Y = currentMessagePoint.Y + 1;
            }
            CRMessage();
        }

        private void PrintString(string msg)
        {
            using var g = Graphics.FromImage(frmMessage.picMessage.NewImageIfNull().Image);
            PrintString(msg, g);
        }

        private void PrintString(string msg, Graphics g)
        {
            g.DrawString(msg, currentMessageFont, currentMessageFontColor, currentMessagePoint);
            currentMessagePoint = currentMessagePoint.AddX(MessageLen(msg, g, currentMessageFont).Width);
        }

        private SizeF MessageLen(string msg)
        {
            // XXX Graphics の取り回し
            using var g = Graphics.FromImage(frmMessage.picMessage.NewImageIfNull().Image);
            return MessageLen(msg, g, currentMessageFont);
        }

        // メッセージ幅を計算(タグを無視して)
        private SizeF MessageLen(string msg, Graphics g, Font font)
        {
            // タグが存在する？
            var ret = Strings.InStr(msg, "<");
            if (ret == 0)
            {
                return g.MeasureStringWithoutRightMargin(msg, font);
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
            return g.MeasureStringWithoutRightMargin(buf, font);
        }

        public void DisplayBattleMessage(string pname, string msg, string msg_mode)
        {
            ResetMessage();
            try
            {
                // メッセージウィンドウが閉じられていれば表示しない
                // XXX 元はWindowState見てたけれどどうかな
                if (!MessageFormVisible)
                {
                    return;
                }

                // ウィンドウのキャプションを設定
                frmMessage.SetMessageModeCaption(false);

                // キャラ表示の描き換え
                string pnickname;
                DisplayMessagePilot(pname, msg_mode, out pnickname, out _);

                // メッセージが空なら表示は止める
                if (string.IsNullOrEmpty(msg))
                {
                    return;
                }

                // 強制改行の位置を設定
                var mesConfig = new MessageConfig();
                if (Expression.IsOptionDefined("改行時余白短縮"))
                {
                    mesConfig.ConfigureShortSpace();
                }
                // メッセージウィンドウの状態を記録
                SaveMessageFormStatus();

                // メッセージを分割
                var messages = msg.Split(";").SelectMany(m =>
                {
                    var messageParts = m.Split(':').ToList();
                    return Enumerable.Range(1, messageParts.Count)
                        .Select(x => string.Join("", messageParts.Take(x))
                            + (x != messageParts.Count ? ":" : ""));
                }).ToList();

                var wait_time = Constants.DEFAULT_LEVEL;

                // 各メッセージを表示
                var need_refresh = false;
                var start_time = 0;
                var lnum = 0;
                var prev_lnum = 0;
                string fsuffix = default, fname0 = default, fpath = default;
                int first_id, last_id = default;
                int wait_time2;
                bool with_footer;
                foreach (var message in messages)
                {
                    var buf = message;

                    // メッセージ内の式置換を処理
                    Event.SaveBasePoint();
                    Expression.FormatMessage(ref buf);
                    Event.RestoreBasePoint();

                    // 特殊効果
                    switch (Strings.LCase(Strings.Right(GeneralLib.LIndex(buf, 1), 4)) ?? "")
                    {
                        case ".bmp":
                        case ".jpg":
                        case ".gif":
                        case ".png":
                            {

                                // 右ボタンを押されていたらスキップ
                                if (IsRButtonPressed())
                                {
                                    goto NextMessage;
                                }

                                var clear_every_time = false;
                                // カットインの表示
                                var fname = GeneralLib.LIndex(buf, 1);

                                // アニメ指定かどうか判定
                                var j = Strings.InStr(fname, "[");
                                if (j > 0 && Strings.InStr(fname, "].") == Strings.Len(fname) - 4)
                                {
                                    fname0 = Strings.Left(fname, j - 1);
                                    fsuffix = Strings.Right(fname, 4);
                                    var buf2 = Strings.Mid(fname, j + 1, Strings.Len(fname) - j - 5);
                                    j = Strings.InStr(buf2, "-");
                                    first_id = Conversions.ToInteger(Strings.Left(buf2, j - 1));
                                    last_id = Conversions.ToInteger(Strings.Mid(buf2, j + 1));
                                }
                                else
                                {
                                    first_id = -1;
                                }

                                // 画像表示のオプション
                                var options = "";
                                var n = GeneralLib.LLength(buf);
                                j = 2;
                                var opt_n = 2;
                                while (j <= n)
                                {
                                    var buf2 = GeneralLib.LIndex(buf, j);
                                    switch (buf2 ?? "")
                                    {
                                        case "透過":
                                        case "背景":
                                        case "白黒":
                                        case "セピア":
                                        case "明":
                                        case "暗":
                                        case "上下反転":
                                        case "左右反転":
                                        case "上半分":
                                        case "下半分":
                                        case "右半分":
                                        case "左半分":
                                        case "右上":
                                        case "左上":
                                        case "右下":
                                        case "左下":
                                        case "ネガポジ反転":
                                        case "シルエット":
                                        case "夕焼け":
                                        case "水中":
                                        case "保持":
                                            {
                                                options = options + buf2 + " ";
                                                break;
                                            }

                                        case "消去":
                                            {
                                                clear_every_time = true;
                                                break;
                                            }

                                        case "右回転":
                                            {
                                                j = (j + 1);
                                                options = options + "右回転 " + GeneralLib.LIndex(buf, j) + " ";
                                                break;
                                            }

                                        case "左回転":
                                            {
                                                j = (j + 1);
                                                options = options + "左回転 " + GeneralLib.LIndex(buf, j) + " ";
                                                break;
                                            }

                                        case "-":
                                            {
                                                // スキップ
                                                opt_n = (j + 1);
                                                break;
                                            }

                                        default:
                                            {
                                                if (Strings.Asc(buf2) == 35 && Strings.Len(buf2) == 7)
                                                {
                                                    // 透過色設定
                                                    var transparentColor = ColorExtension.FromHexString(buf2);
                                                    if (!transparentColor.IsEmpty)
                                                    {
                                                        options = options + transparentColor.ToHexString() + " ";
                                                    }
                                                }
                                                else if (Information.IsNumeric(buf2))
                                                {
                                                    // スキップ
                                                    opt_n = (j + 1);
                                                }

                                                break;
                                            }
                                    }

                                    j = (j + 1);
                                }

                                if (Strings.Asc(fname) == 64) // @
                                {
                                    // パイロット画像切り替えの場合

                                    if (first_id == -1)
                                    {
                                        fname = Strings.Mid(fname, 2);
                                    }
                                    else
                                    {
                                        fname0 = Strings.Mid(fname0, 2);
                                        fname = fname0 + SrcFormatter.Format(first_id, "00") + fsuffix;
                                    }

                                    // ウィンドウが表示されていなければ表示
                                    if (!MessageFormVisible)
                                    {
                                        OpenMessageForm(u1: null, u2: null);
                                    }

                                    if (wait_time > 0)
                                    {
                                        start_time = GeneralLib.timeGetTime();
                                    }

                                    // 画像表示のオプション
                                    options = options + " メッセージ";
                                    switch (Map.MapDrawMode ?? "")
                                    {
                                        case "セピア":
                                        case "白黒":
                                            {
                                                options = options + " " + Map.MapDrawMode;
                                                break;
                                            }
                                    }

                                    if (first_id == -1)
                                    {
                                        // １枚画像の場合
                                        DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, options);
                                        // XXX picFace
                                        //My.MyProject.Forms.frmMessage.picFace.Refresh();
                                        if (wait_time > 0)
                                        {
                                            while (start_time + wait_time > GeneralLib.timeGetTime())
                                                Sleep(20);
                                        }
                                    }
                                    else
                                    {
                                        // アニメーションの場合
                                        var loopTo2 = last_id;
                                        for (j = first_id; j <= loopTo2; j++)
                                        {
                                            fname = fpath + fname0 + SrcFormatter.Format(j, "00") + fsuffix;
                                            DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, options);
                                            // XXX picFace
                                            //My.MyProject.Forms.frmMessage.picFace.Refresh();
                                            if (wait_time > 0)
                                            {
                                                wait_time2 = wait_time * (j - first_id + 1) / (last_id - first_id);
                                                var cur_time = GeneralLib.timeGetTime();
                                                if (cur_time < start_time + wait_time2)
                                                {
                                                    Sleep(start_time + wait_time2 - cur_time);
                                                }
                                            }
                                        }
                                    }

                                    wait_time = Constants.DEFAULT_LEVEL;
                                    DisplayedPilot = fname;
                                    goto NextMessage;
                                }

                                string dw, dh;
                                string dx, dy;
                                // 表示画像のサイズ
                                if (opt_n > 2)
                                {
                                    var buf2 = GeneralLib.LIndex(buf, 2);
                                    if (buf2 == "-")
                                    {
                                        dw = Constants.DEFAULT_LEVEL.ToString();
                                    }
                                    else
                                    {
                                        dw = GeneralLib.StrToLng(buf2).ToString();
                                    }

                                    buf2 = GeneralLib.LIndex(buf, 3);
                                    if (buf2 == "-")
                                    {
                                        dh = Constants.DEFAULT_LEVEL.ToString();
                                    }
                                    else
                                    {
                                        dh = GeneralLib.StrToLng(buf2).ToString();
                                    }
                                }
                                else
                                {
                                    dw = Constants.DEFAULT_LEVEL.ToString();
                                    dh = Constants.DEFAULT_LEVEL.ToString();
                                }

                                // 表示画像の位置
                                if (opt_n > 4)
                                {
                                    var buf2 = GeneralLib.LIndex(buf, 4);
                                    if (buf2 == "-")
                                    {
                                        dx = Constants.DEFAULT_LEVEL.ToString();
                                    }
                                    else
                                    {
                                        dx = GeneralLib.StrToLng(buf2).ToString();
                                    }

                                    buf2 = GeneralLib.LIndex(buf, 5);
                                    if (buf2 == "-")
                                    {
                                        dy = Constants.DEFAULT_LEVEL.ToString();
                                    }
                                    else
                                    {
                                        dy = GeneralLib.StrToLng(buf2).ToString();
                                    }
                                }
                                else
                                {
                                    dx = Constants.DEFAULT_LEVEL.ToString();
                                    dy = Constants.DEFAULT_LEVEL.ToString();
                                }

                                if (wait_time > 0)
                                {
                                    start_time = GeneralLib.timeGetTime();
                                }

                                if (first_id == -1)
                                {
                                    // １枚絵の場合
                                    if (clear_every_time)
                                    {
                                        ClearPicture();
                                    }

                                    DrawPicture(fname, Conversions.ToInteger(dx), Conversions.ToInteger(dy), Conversions.ToInteger(dw), Conversions.ToInteger(dh), 0, 0, 0, 0, options);
                                    need_refresh = true;
                                    if (wait_time > 0)
                                    {
                                        RefreshScreen();
                                        need_refresh = false;
                                        var cur_time = GeneralLib.timeGetTime();
                                        if (cur_time < start_time + wait_time)
                                        {
                                            Sleep(start_time + wait_time - cur_time);
                                        }

                                        wait_time = Constants.DEFAULT_LEVEL;
                                    }
                                }
                                else
                                {
                                    // アニメーションの場合
                                    var loopTo3 = last_id;
                                    for (j = first_id; j <= loopTo3; j++)
                                    {
                                        fname = fname0 + SrcFormatter.Format(j, "00") + fsuffix;
                                        if (clear_every_time)
                                        {
                                            ClearPicture();
                                        }

                                        DrawPicture(fname, Conversions.ToInteger(dx), Conversions.ToInteger(dy), Conversions.ToInteger(dw), Conversions.ToInteger(dh), 0, 0, 0, 0, options);

                                        RefreshScreen();
                                        if (wait_time > 0)
                                        {
                                            wait_time2 = wait_time * (j - first_id + 1) / (last_id - first_id);
                                            var cur_time = GeneralLib.timeGetTime();
                                            if (cur_time < start_time + wait_time2)
                                            {
                                                Sleep(start_time + wait_time2 - cur_time);
                                            }
                                        }
                                    }

                                    wait_time = Constants.DEFAULT_LEVEL;
                                }

                                goto NextMessage;
                            }

                        case ".wav":
                        case ".mp3":
                            {
                                // 右ボタンを押されていたらスキップ
                                if (IsRButtonPressed())
                                {
                                    goto NextMessage;
                                }

                                // 効果音の演奏
                                Sound.PlayWave(buf);
                                if (wait_time > 0)
                                {
                                    if (need_refresh)
                                    {
                                        RefreshScreen();
                                        need_refresh = false;
                                    }

                                    Sleep(wait_time);
                                    wait_time = Constants.DEFAULT_LEVEL;
                                }

                                goto NextMessage;
                            }
                    }

                    // 戦闘アニメ呼び出し
                    if (Strings.Left(buf, 1) == "@")
                    {
                        Effect.ShowAnimation(Strings.Mid(buf, 2));
                        goto NextMessage;
                    }

                    // 特殊コマンド
                    switch (Strings.LCase(GeneralLib.LIndex(buf, 1)) ?? "")
                    {
                        case "clear":
                            {
                                // カットインの消去
                                ClearPicture();
                                need_refresh = true;
                                goto NextMessage;
                            }

                        case "keep":
                            {
                                // カットインの保存
                                IsPictureDrawn = false;
                                goto NextMessage;
                            }
                    }

                    // ウェイト
                    if (Information.IsNumeric(buf))
                    {
                        wait_time = (int)(100d * Conversions.ToDouble(buf));
                        goto NextMessage;
                    }

                    // これよりメッセージの表示

                    // 空メッセージの場合は表示しない
                    if (string.IsNullOrEmpty(buf))
                    {
                        goto NextMessage;
                    }

                    // メッセージウィンドウの状態が変化している場合は復元
                    KeepMessageFormStatus();
                    // ウィンドウをクリア
                    ResetMessageArea();

                    // フォント設定を初期化
                    ResetFont();

                    // 話者名と括弧の表示処理
                    var is_char_message = false;
                    if (pname != "システム" && (!string.IsNullOrEmpty(pname) && pname != "-" || Strings.Left(buf, 1) == "「" && Strings.Right(buf, 1) == "」" || Strings.Left(buf, 1) == "『" && Strings.Right(buf, 1) == "』"))
                    {
                        is_char_message = true;

                        // 話者のグラフィックを表示
                        if (pname == "-" && Commands.SelectedUnit is object)
                        {
                            if (Commands.SelectedUnit.CountPilot() > 0)
                            {
                                var fname = Commands.SelectedUnit.MainPilot().get_Bitmap(false);
                                if (DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "メッセージ " + msg_mode))
                                {
                                    // XXX picFace
                                    //My.MyProject.Forms.frmMessage.picFace.Refresh();
                                    DisplayedPilot = fname;
                                    DisplayMode = msg_mode;
                                }
                            }
                        }

                        // 話者名を表示
                        if (string.IsNullOrEmpty(pnickname) && pname == "-" && Commands.SelectedUnit is object)
                        {
                            if (Commands.SelectedUnit.CountPilot() > 0)
                            {
                                PrintString(Commands.SelectedUnit.MainPilot().get_Nickname(false));
                            }
                        }
                        else if (!string.IsNullOrEmpty(pnickname))
                        {
                            PrintString(pnickname);
                        }

                        // メッセージが途中で終わっているか判定
                        if (Strings.Right(buf, 1) != ":")
                        {
                            with_footer = true;
                        }
                        else
                        {
                            with_footer = false;
                            prev_lnum = lnum;
                            buf = Strings.Left(buf, Strings.Len(buf) - 1);
                        }

                        // 括弧を付加
                        if ((Strings.Left(buf, 1) == "(" || Strings.Left(buf, 1) == "（") && (!with_footer || Strings.Right(buf, 1) == ")" || Strings.Right(buf, 1) == "）"))
                        {
                            // モノローグ
                            if (with_footer)
                            {
                                buf = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                                buf = "（" + buf + "）";
                            }
                            else
                            {
                                buf = Strings.Mid(buf, 2);
                                buf = "（" + buf;
                            }
                        }
                        else if (Strings.Left(buf, 1) == "「" && (!with_footer || Strings.Right(buf, 1) == "」"))
                        {
                        }
                        // 「」の括弧が既にあるので変更しない
                        else if (Strings.Left(buf, 1) == "『" && (!with_footer || Strings.Right(buf, 1) == "』"))
                        {
                        }
                        // 『』の括弧が既にあるので変更しない
                        else if (with_footer)
                        {
                            buf = "「" + buf + "」";
                        }
                        else
                        {
                            buf = "「" + buf;
                        }
                    }
                    // メッセージが途中で終わっているか判定
                    else if (Strings.Right(buf, 1) == ":")
                    {
                        prev_lnum = lnum;
                        buf = Strings.Left(buf, Strings.Len(buf) - 1);
                    }

                    prev_lnum = GeneralLib.MaxLng(prev_lnum, 1);
                    lnum = 0;
                    var line_head = 1;
                    var in_tag = false;
                    var loopTo4 = Strings.Len(buf);
                    for (var j = 1; j <= loopTo4; j++)
                    {
                        var ch = Strings.Mid(buf, j, 1);

                        // 「.」の場合は必ず改行
                        if (ch == ".")
                        {
                            if (j != line_head)
                            {
                                PrintMessage(Strings.Mid(buf, line_head, j - line_head), !is_char_message);
                                lnum = (lnum + 1);
                                if (is_char_message)
                                {
                                    PrintString("  ");
                                }
                            }

                            line_head = (j + 1);
                            goto NextLoop;
                        }

                        // タグ内では改行しない
                        if (ch == "<")
                        {
                            in_tag = true;
                            goto NextLoop;
                        }
                        else if (ch == ">")
                        {
                            in_tag = false;
                        }
                        else if (in_tag)
                        {
                            goto NextLoop;
                        }

                        // メッセージが途切れてしまう場合は必ず改行
                        if (MessageLen(Strings.Mid(buf, line_head, j - line_head)).Width > MessageConfig.BrThresholdLimit * messageArea.Width)
                        {
                            PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), !is_char_message);
                            lnum = (lnum + 1);
                            if (is_char_message)
                            {
                                PrintString("  ");
                            }

                            line_head = (j + 1);
                            goto NextLoop;
                        }

                        // 禁則処理
                        switch (Strings.Mid(buf, j + 1, 1) ?? "")
                        {
                            case "。":
                            case "、":
                            case "…":
                            case "‥":
                            case "・":
                            case "･":
                            case "～":
                            case "ー":
                            case "－":
                            case "！":
                            case "？":
                            case "」":
                            case "』":
                            case "）":
                            case ")":
                            case " ":
                            case ".":
                                {
                                    goto NextLoop;
                                }
                        }

                        switch (Strings.Mid(buf, j + 2, 1) ?? "")
                        {
                            case "。":
                            case "、":
                            case "…":
                            case "‥":
                            case "・":
                            case "･":
                            case "～":
                            case "ー":
                            case "－":
                            case "！":
                            case "？":
                            case "」":
                            case "』":
                            case "）":
                            case ")":
                            case " ":
                            case ".":
                                {
                                    goto NextLoop;
                                }
                        }

                        if (Strings.Mid(buf, j + 3, 1) == ".")
                        {
                            goto NextLoop;
                        }

                        // 改行の判定
                        if (MessageLen(Strings.Mid(message, line_head)).Width < MessageConfig.BrThresholdLimit * messageArea.Width)
                        {
                            // 全体が一行に収まる場合
                            goto NextLoop;
                        }

                        switch (ch ?? "")
                        {
                            case "。":
                                {
                                    if (MessageLen(Strings.Mid(buf, line_head, j - line_head)).Width > mesConfig.BrThresholdKuten * messageArea.Width)
                                    {
                                        PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), !is_char_message);
                                        lnum = (lnum + 1);
                                        if (is_char_message)
                                        {
                                            PrintString("  ");
                                        }

                                        line_head = (j + 1);
                                    }

                                    break;
                                }

                            case "、":
                                {
                                    if (MessageLen(Strings.Mid(buf, line_head, j - line_head)).Width > mesConfig.BrThresholdTouten * messageArea.Width)
                                    {
                                        PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), !is_char_message);
                                        lnum = (lnum + 1);
                                        if (is_char_message)
                                        {
                                            PrintString("  ");
                                        }

                                        line_head = (j + 1);
                                    }

                                    break;
                                }

                            case " ":
                                {
                                    PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), !is_char_message);
                                    lnum = (lnum + 1);
                                    if (is_char_message)
                                    {
                                        PrintString("  ");
                                    }

                                    line_head = (j + 1);

                                    break;
                                }

                            default:
                                {
                                    if (MessageLen(Strings.Mid(buf, line_head, j - line_head)).Width > mesConfig.BrThresholdOver * messageArea.Width)
                                    {
                                        PrintMessage(Strings.Mid(buf, line_head, j - line_head + 1), !is_char_message);
                                        lnum = (lnum + 1);
                                        if (is_char_message)
                                        {
                                            PrintString("  ");
                                        }

                                        line_head = (j + 1);
                                    }

                                    break;
                                }
                        }

                    NextLoop:
                        ;
                    }
                    // メッセージの残りを表示しておく
                    if (Strings.Len(buf) >= line_head)
                    {
                        PrintMessage(Strings.Mid(buf, line_head), !is_char_message);
                        lnum = (lnum + 1);
                    }
                    RefreshMessage();

                    // フォント設定を元に戻す
                    ResetFont();

                    // デフォルトのウェイト
                    if (wait_time == Constants.DEFAULT_LEVEL)
                    {
                        wait_time = (lnum - prev_lnum + 1) * MessageWait;
                        if (msg_mode == "高速")
                        {
                            wait_time = wait_time / 2;
                        }
                    }

                    // 画面を更新
                    if (need_refresh)
                    {
                        UpdateScreen();
                        need_refresh = false;
                    }

                    Application.DoEvents();

                    // 待ち時間が切れるまで待機
                    start_time = GeneralLib.timeGetTime();
                    IsFormClicked = false;
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

                    wait_time = Constants.DEFAULT_LEVEL;
                NextMessage:
                    ;
                }

                // 戦闘アニメデータのカットイン表示？
                if (pname == "-")
                {
                    return;
                }

                // 画面を更新
                if (need_refresh)
                {
                    UpdateScreen();
                    need_refresh = false;
                }

                // メッセージデータの最後にウェイトの指定が行われていた場合
                if (wait_time > 0)
                {
                    start_time = GeneralLib.timeGetTime();
                    IsFormClicked = false;
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

                // メッセージウィンドウの状態が変化している場合は復元
                KeepMessageFormStatus();
                Application.DoEvents();
            }
            finally
            {
                ResetMessage();
            }
        }

        public void DisplaySysMessage(string msg, bool short_wait)
        {
            ResetMessage();
            try
            {
                // メッセージウィンドウが閉じられていれば表示しない
                // XXX 元はWindowState見てたけれどどうかな
                if (!MessageFormVisible)
                {
                    return;
                }

                // ウィンドウのキャプションを設定
                frmMessage.SetMessageModeCaption(false);

                // メッセージ内の式を置換
                Expression.FormatMessage(ref msg);

                // メッセージを表示
                var in_tag = false;
                var lnum = 0;
                var line_head = 1;
                var loopTo = Strings.Len(msg);
                for (var i = 1; i <= loopTo; i++)
                {
                    var ch = Strings.Mid(msg, i, 1);

                    // 「;」の場合は必ず改行
                    if (ch == ";")
                    {
                        if (line_head != i)
                        {
                            var buf = Strings.Mid(msg, line_head, i - line_head);
                            PrintMessage(buf, true);
                            lnum = (lnum + 1);
                        }

                        line_head = (i + 1);
                        goto NextLoop;
                    }

                    // タグ内では改行しない
                    if (ch == "<")
                    {
                        in_tag = true;
                        goto NextLoop;
                    }
                    else if (ch == ">")
                    {
                        in_tag = false;
                    }
                    else if (in_tag)
                    {
                        goto NextLoop;
                    }

                    // 禁則処理
                    if (ch == "。" || ch == "、")
                    {
                        goto NextLoop;
                    }

                    if (i < Strings.Len(msg))
                    {
                        if (Strings.Mid(msg, i + 1, 1) == "。" || Strings.Mid(msg, i + 1, 1) == "、")
                        {
                            goto NextLoop;
                        }
                    }

                    if (MessageLen(Strings.Mid(msg, line_head)).Width < MessageConfig.BrThresholdLimit * messageArea.Width)
                    {
                        // 全体が一行に収まる場合
                        goto NextLoop;
                    }

                    if (GeneralLib.IsSpace(ch) && MessageLen(Strings.Mid(msg, line_head, i - line_head)).Width > MessageConfig.BrThresholdMinimum * messageArea.Width)
                    {
                        var buf = Strings.Mid(msg, line_head, i - line_head);
                        PrintMessage(buf, true);
                        lnum = (lnum + 1);
                        line_head = (i + 1);
                    }
                    else if (MessageLen(Strings.Mid(msg, line_head, i - line_head + 1)).Width > MessageConfig.BrThresholdLimit * messageArea.Width)
                    {
                        var buf = Strings.Mid(msg, line_head, i - line_head + 1);
                        PrintMessage(buf, true);
                        lnum = (lnum + 1);
                        line_head = (i + 1);
                    }
                    else if (ch == "[")
                    {
                        // []で囲まれた文字列内では改行しない
                        var loopTo1 = Strings.Len(msg);
                        var j = 1;
                        for (j = i; j <= loopTo1; j++)
                        {
                            if (Strings.Mid(msg, j, 1) == "]")
                            {
                                break;
                            }
                        }

                        if (MessageLen(Strings.Mid(msg, line_head, j - line_head)).Width > MessageConfig.BrThresholdLimit * messageArea.Width)
                        {
                            var buf = Strings.Mid(msg, line_head, i - line_head);
                            PrintMessage(buf, true);
                            lnum = (lnum + 1);
                            line_head = i;
                        }
                    }

                NextLoop:
                    ;
                }
                {
                    var buf = Strings.Mid(msg, line_head);
                    PrintMessage(buf, true);
                    lnum = (lnum + 1);
                }
                // フォント設定を元に戻す
                ResetFont();

                // ウェイトを計算
                var wait_time = (int)((0.8d + 0.5d * lnum) * MessageWait);
                if (short_wait)
                {
                    wait_time = wait_time / 2;
                }

                RefreshMessage();
                Application.DoEvents();

                // 待ち時間が切れるまで待機
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
            finally
            {
                ResetMessage();
            }
        }
    }
}
