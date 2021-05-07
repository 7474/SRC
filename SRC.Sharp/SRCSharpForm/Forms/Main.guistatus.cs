using Newtonsoft.Json;
using SRCCore;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using SRCSharpForm.Extensions;
using SRCSharpForm.Lib;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;

namespace SRCSharpForm
{
    // TODO インタフェースの切り方見直す
    // ステータスをいつ出すかはGUIの側で制御できたほうがよさそう
    // そのうえで表示内容が変化する契機を通知するのがよいだろうか
    internal partial class frmMain : IGUIStatus
    {
        public Unit DisplayedUnit { get; set; }
        public Pilot DisplayedPilot { get; set; }

        // XXX
        // ステータス画面の更新を一時停止するかどうか
        public static bool IsStatusWindowDisabled;
        // ステータス画面の背景色
        public Color StatusWindowBackBolor;
        // ステータス画面の枠色
        public Color StatusWindowFrameColor;
        // ステータス画面の枠幅
        public int StatusWindowFrameWidth;
        // ステータス画面 能力名のフォントカラー
        public Color StatusFontColorAbilityName = Color.FromArgb(0, 0, 150);
        // ステータス画面 有効な能力のフォントカラー
        public Color StatusFontColorAbilityEnable = Color.FromArgb(0, 0, 200);
        // ステータス画面 無効な能力のフォントカラー
        public Color StatusFontColorAbilityDisable = Color.FromArgb(150, 0, 0);
        // ステータス画面 その他通常描画のフォントカラー
        public Color StatusFontColorNormalString = Color.FromArgb(0, 0, 0);

        // XXX
        private TextRenderingHint StatusTextRenderingHint = TextRenderingHint.SystemDefault;
        //private Font StatusFont = new Font("Yu Gothic UI", 10f);
        //private Font StatusFont = new Font("Meiryo UI", 10f);
        private Font StatusFont = new Font("ＭＳ 明朝", 10f);
        // XXX 10.5より大きい感じする
        private Font HeadingFont = new Font("ＭＳ 明朝", 10.5f);
        private Font StatusSmallFont = new Font("ＭＳ 明朝", 9f);
        private Brush StatusNormalStringBrush = Brushes.Black;
        private Brush StatusAbilityNameBrush = new SolidBrush(Color.FromArgb(0, 0, 150));
        private Brush StatusAbilityEnableBrush = new SolidBrush(Color.FromArgb(0, 0, 200));
        private Brush StatusAbilityDisableBrush = new SolidBrush(Color.FromArgb(150, 0, 0));

        public void DisplayGlobalStatus()
        {
            ClearUnitStatus();

            var xx = GUI.PixelToMapX((int)GUI.MouseX);
            var yy = GUI.PixelToMapY((int)GUI.MouseY);
            var mapCell = Map.CellAtPoint(xx, yy);

            var sb = new StringBuilder();
            sb.AppendLine("ターン数 " + SrcFormatter.Format(SRC.Turn));
            sb.AppendLine(Expression.Term("資金", null, 8) + " " + SrcFormatter.Format(SRC.Money));

            // マップ外をクリックした時はここで終了
            if (mapCell == null)
            {
                picUnitStatus.NewImageIfNull();
                using (var g = Graphics.FromImage(picUnitStatus.Image))
                {
                    g.TextRenderingHint = StatusTextRenderingHint;
                    g.DrawString(sb.ToString(), StatusFont, StatusNormalStringBrush, 0, 0);
                }
                return;
            }

            // 地形情報の表示
            var td = mapCell.Terrain;
            var sbTerrain = new StringBuilder();
            // 地形名称
            sbTerrain.Append($"({mapCell.X},{mapCell.Y}) ");
            if (Strings.InStr(td.Name, "(") > 0)
            {
                sbTerrain.AppendLine(Strings.Left(td.Name, Strings.InStr(td.Name, "(") - 1));
            }
            else
            {
                sbTerrain.AppendLine(td.Name);
            }

            // 命中修正
            if (td.HitMod >= 0)
            {
                sbTerrain.Append("回避 +" + SrcFormatter.Format(td.HitMod) + "%");
            }
            else
            {
                sbTerrain.Append("回避 " + SrcFormatter.Format(td.HitMod) + "%");
            }

            // ダメージ修正
            if (td.DamageMod >= 0)
            {
                sbTerrain.AppendLine("  防御 +" + SrcFormatter.Format(td.DamageMod) + "%");
            }
            else
            {
                sbTerrain.AppendLine("  防御 " + SrcFormatter.Format(td.DamageMod) + "%");
            }

            // ＨＰ回復率
            if (td.EffectForHPRecover() > 0)
            {
                sbTerrain.Append(Expression.Term("ＨＰ", u: null) + " +" + SrcFormatter.Format(td.EffectForHPRecover()) + "%  ");
            }

            // ＥＮ回復率
            if (td.EffectForENRecover() > 0)
            {
                sbTerrain.Append(Expression.Term("ＥＮ", u: null) + " +" + SrcFormatter.Format(td.EffectForENRecover()) + "%");
            }

            if (td.EffectForHPRecover() > 0 || td.EffectForENRecover() > 0)
            {
                sbTerrain.AppendLine();
            }

            // ＨＰ＆ＥＮ減少
            if (td.IsFeatureAvailable("ＨＰ減少"))
            {
                sbTerrain.Append(Expression.Term("ＨＰ", u: null) + " -" + SrcFormatter.Format(10d * td.FeatureLevel("ＨＰ減少")) + "% (" + td.Feature("ＨＰ減少").Data + ")  ");
            }

            if (td.IsFeatureAvailable("ＥＮ減少"))
            {
                sbTerrain.Append(Expression.Term("ＥＮ", u: null) + " -" + SrcFormatter.Format(10d * td.FeatureLevel("ＥＮ減少")) + "% (" + td.Feature("ＥＮ減少").Data + ")  ");
            }

            if (td.IsFeatureAvailable("ＨＰ減少") || td.IsFeatureAvailable("ＥＮ減少"))
            {
                sbTerrain.AppendLine();
            }

            // ＨＰ＆ＥＮ増加
            if (td.IsFeatureAvailable("ＨＰ増加"))
            {
                sbTerrain.Append(Expression.Term("ＨＰ", u: null) + " +" + SrcFormatter.Format(1000d * td.FeatureLevel("ＨＰ増加")) + "  ");
            }

            if (td.IsFeatureAvailable("ＥＮ増加"))
            {
                sbTerrain.Append(Expression.Term("ＥＮ", u: null) + " +" + SrcFormatter.Format(10d * td.FeatureLevel("ＥＮ増加")) + "  ");
            }

            if (td.IsFeatureAvailable("ＨＰ増加") || td.IsFeatureAvailable("ＥＮ増加"))
            {
                sbTerrain.AppendLine();
            }

            // ＨＰ＆ＥＮ低下
            if (td.IsFeatureAvailable("ＨＰ低下"))
            {
                sbTerrain.Append(Expression.Term("ＨＰ", u: null) + " -" + SrcFormatter.Format(1000d * td.FeatureLevel("ＨＰ低下")) + "  ");
            }

            if (td.IsFeatureAvailable("ＥＮ低下"))
            {
                sbTerrain.Append(Expression.Term("ＥＮ", u: null) + " -" + SrcFormatter.Format(10d * td.FeatureLevel("ＥＮ低下")) + "  ");
            }

            if (td.IsFeatureAvailable("ＨＰ低下") || td.IsFeatureAvailable("ＥＮ低下"))
            {
                sbTerrain.AppendLine();
            }

            // 摩擦
            if (td.IsFeatureAvailable("摩擦"))
            {
                sbTerrain.AppendLine("摩擦Lv" + SrcFormatter.Format(td.FeatureLevel("摩擦")));
            }
            // 状態異常付加
            if (td.IsFeatureAvailable("状態付加"))
            {
                sbTerrain.AppendLine(td.Feature("状態付加").Data + "状態付加");
            }

            picUnitStatus.NewImageIfNull();
            using (var g = Graphics.FromImage(picUnitStatus.Image))
            {
                g.TextRenderingHint = StatusTextRenderingHint;
                g.DrawString(sb.ToString(), StatusFont, StatusNormalStringBrush, 0, 0);

                // マップ画像表示
                g.DrawImage(picBack.Image, 0, 48,
                    new Rectangle((xx - 1) * MapCellPx, (yy - 1) * MapCellPx, MapCellPx, MapCellPx),
                    GraphicsUnit.Pixel);

                g.DrawString(sbTerrain.ToString(), StatusFont, StatusNormalStringBrush, MapCellPx + 5, 48);
            }
        }

        public void DisplayUnitStatus(Unit u, Pilot p = null)
        {
            // ステータス画面の更新が一時停止されている場合はそのまま終了
            if (IsStatusWindowDisabled)
            {
                return;
            }

            // 破壊、破棄されたユニットは表示しない
            if (u.Status == "破壊" || u.Status == "破棄")
            {
                return;
            }

            ClearUnitStatus();

            DisplayedUnit = u;
            DisplayedPilot = p;

            TerrainData td;
            string wclass;
            // 情報を更新
            u.Update();

            // 未確認ユニットかどうか判定しておく
            var is_unknown = Expression.IsOptionDefined("ユニット情報隠蔽") && !u.IsConditionSatisfied("識別済み") && (u.Party0 == "敵" || u.Party0 == "中立")
                || u.IsConditionSatisfied("ユニット情報隠蔽");

            using (var pilotG = Graphics.FromImage(picPilotStatus.NewImageIfNull().Image))
            using (var unitG = Graphics.FromImage(picUnitStatus.NewImageIfNull().Image))
            {
                pilotG.TextRenderingHint = StatusTextRenderingHint;
                unitG.TextRenderingHint = StatusTextRenderingHint;
                var headingMargin = HeadingFont.Height;
                var textMargin = StatusFont.Height;
                var smallMargin = StatusSmallFont.Height;

                var ppic = new Printer(pilotG, StatusFont, StatusFontColorNormalString);
                var upic = new Printer(unitG, StatusFont, StatusFontColorNormalString);

                // パイロットが乗っていない？
                if (u.CountPilot() == 0)
                {
                    // キャラ画面をクリア
                    picFace.Image = null;
                    DrawPilotLabel(u, pilotG, unitG, headingMargin);
                    goto UnitStatus;
                }

                // XXX 表示対象の解決は外に求めてる
                //// 表示するパイロットを選択
                //if (pindex == 0)
                //{
                //    // メインパイロット
                //    p = u.MainPilot();
                //    if ((u.MainPilot().get_Nickname(false) ?? "") == (u.Pilot(1).get_Nickname(false) ?? "") || u.Data.PilotNum == 1)
                //    {
                //        DisplayedPilotInd = 1;
                //    }
                //}
                //else if (pindex == 1)
                //{
                //    // メインパイロットまたは１番目のパイロット
                //    if ((u.MainPilot().get_Nickname(false) ?? "") != (u.Pilot(1).get_Nickname(false) ?? "") && u.Data.PilotNum != 1)
                //    {
                //        p = u.Pilot(1);
                //    }
                //    else
                //    {
                //        p = u.MainPilot();
                //    }
                //}
                //else if (pindex <= u.CountPilot())
                //{
                //    // サブパイロット
                //    p = u.Pilot(pindex);
                //}
                //else if (pindex <= (u.CountPilot() + u.CountSupport()))
                //{
                //    // サポートパイロット
                //    p = u.Support(pindex - u.CountPilot());
                //}
                //else
                //{
                //    // 追加サポート
                //    p = u.AdditionalSupport();
                //}
                // 指定がなければメインパイロット
                if (p == null)
                {
                    p = u.MainPilot();
                }
                // 情報を更新
                p.UpdateSupportMod();

                // パイロット画像を表示
                var fname = SRC.FileSystem.PathCombine("Pilot", p.get_Bitmap(false));
                //if (My.MyProject.Forms.frmMultiSelectListBox.Visible)
                //{
                //    // ザコ＆汎用パイロットが乗るユニットの出撃選択時はパイロット画像の
                //    // 代わりにユニット画像を表示
                //    if (Strings.InStr(p.Name, "(ザコ)") > 0 || Strings.InStr(p.Name, "(汎用)") > 0)
                //    {
                //        fname = @"\Bitmap\Unit\" + u.get_Bitmap(false);
                //    }
                //}
                // 画像ファイルを読み込んで表示
                var image = imageBuffer.GetTransparent(fname);
                picFace.Image = image;

                // パイロット愛称
                ppic.SetFont(StatusFont);
                ppic.SetColor(StatusFontColorNormalString);
                ppic.Print(p.get_Nickname(false));
                ppic.Print();
                // ダミーパイロット？
                if (p.Nickname0 == "パイロット不在")
                {
                    DrawPilotLabel(u, pilotG, unitG, headingMargin);
                    goto UnitStatus;
                }
                // レベル、経験値、行動回数
                var lvExpText = "";
                var actionText = "";
                if (p.Party == "味方")
                {
                    lvExpText = SrcFormatter.Format(p.Level) + " (" + p.Exp + ")";
                    switch (u.Action)
                    {
                        case 2:
                            actionText = "Ｗ";
                            break;

                        case 3:
                            actionText = " Ｔ";
                            break;
                    }
                }
                else if (!is_unknown)
                {
                    lvExpText = SrcFormatter.Format(p.Level);
                    if (u.Action == 2)
                    {
                        actionText = "Ｗ";
                    }
                }
                else
                {
                    lvExpText = "？";
                }
                ppic.SetFont(StatusFont);
                ppic.SetColor(StatusFontColorAbilityName);
                ppic.Print("レベル ");
                ppic.SetColor(StatusFontColorNormalString);
                ppic.Print(lvExpText);
                ppic.SetColor(StatusFontColorAbilityEnable);
                ppic.Print(actionText);
                ppic.Print();

                // 気力
                var moraleText = "";
                if (!is_unknown)
                {
                    if (p.MoraleMod > 0)
                    {
                        moraleText = SrcFormatter.Format(p.Morale) + "+" + SrcFormatter.Format(p.MoraleMod) + " (" + p.Personality + ")";
                    }
                    else
                    {
                        moraleText = SrcFormatter.Format(p.Morale) + " (" + p.Personality + ")";
                    }
                }
                else
                {
                    moraleText = "？";
                }
                ppic.SetColor(StatusFontColorAbilityName);
                ppic.Print("気力 ");
                ppic.SetColor(StatusFontColorNormalString);
                ppic.Print(moraleText);
                ppic.Print();

                // ＳＰ
                var isNoSp = false;
                if (p.MaxSP > 0)
                {
                    ppic.Print(Expression.Term("ＳＰ", u) + " ");
                    ppic.SetColor(StatusFontColorNormalString);
                    if (!is_unknown)
                    {
                        ppic.Print(SrcFormatter.Format(p.SP) + "/" + SrcFormatter.Format(p.MaxSP));
                    }
                    else
                    {
                        ppic.Print("？");
                    }
                }
                else
                {
                    isNoSp = true;
                }

                // 使用中のスペシャルパワー一覧
                if (!is_unknown)
                {
                    ppic.SetColor(StatusFontColorNormalString);
                    ppic.Print(u.SpecialPowerInEffect());
                }
                if (isNoSp)
                {
                    ppic.Print(" ");
                }

                // upicを明示的に初期化
                upic.SetFont(StatusSmallFont);

                // 格闘
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("格闘", u, 4) + " ");
                upic.SetColor(StatusFontColorNormalString);
                if (is_unknown)
                {
                    upic.Print(GeneralLib.LeftPaddedString("？", 4) + Strings.Space(10));
                }
                else if (p.Data.Infight > 1)
                {
                    switch ((p.InfightMod + p.InfightMod2))
                    {
                        case var @case when @case > 0:
                            {
                                string localLeftPaddedString() { string argbuf = SrcFormatter.Format(p.InfightBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString() { string argbuf = "+" + SrcFormatter.Format(p.InfightMod + p.InfightMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

                                upic.Print(localLeftPaddedString() + localRightPaddedString());
                                break;
                            }

                        case var case1 when case1 < 0:
                            {
                                string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(p.InfightBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString1() { string argbuf = SrcFormatter.Format(p.InfightMod + p.InfightMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

                                upic.Print(localLeftPaddedString1() + localRightPaddedString1());
                                break;
                            }

                        case 0:
                            {
                                string localLeftPaddedString2() { string argbuf = SrcFormatter.Format(p.Infight); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                upic.Print(localLeftPaddedString2() + Strings.Space(9));
                                break;
                            }
                    }
                }
                else
                {
                    upic.Print(GeneralLib.LeftPaddedString("--", 5) + Strings.Space(9));
                }

                // 射撃
                upic.SetColor(StatusFontColorAbilityName);
                if (!p.HasMana())
                {
                    upic.Print(Expression.Term("射撃", u, 4) + " ");
                }
                else
                {
                    upic.Print(Expression.Term("魔力", u, 4) + " ");
                }
                upic.SetColor(StatusFontColorNormalString);
                if (is_unknown)
                {
                    upic.Print(GeneralLib.LeftPaddedString("？", 4));
                }
                else if (p.Data.Shooting > 1)
                {
                    switch ((p.ShootingMod + p.ShootingMod2))
                    {
                        case var case2 when case2 > 0:
                            {
                                string localLeftPaddedString3() { string argbuf = SrcFormatter.Format(p.ShootingBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString2() { string argbuf = "+" + SrcFormatter.Format(p.ShootingMod + p.ShootingMod2); var ret = GeneralLib.RightPaddedString(argbuf, 5); return ret; }

                                upic.Print(localLeftPaddedString3() + localRightPaddedString2());
                                break;
                            }

                        case var case3 when case3 < 0:
                            {
                                string localLeftPaddedString4() { string argbuf = SrcFormatter.Format(p.ShootingBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString3() { string argbuf = SrcFormatter.Format(p.ShootingMod + p.ShootingMod2); var ret = GeneralLib.RightPaddedString(argbuf, 5); return ret; }

                                upic.Print(localLeftPaddedString4() + localRightPaddedString3());
                                break;
                            }

                        case 0:
                            {
                                string localLeftPaddedString5() { string argbuf = SrcFormatter.Format(p.Shooting); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                upic.Print(localLeftPaddedString5() + Strings.Space(5));
                                break;
                            }
                    }
                }
                else
                {
                    upic.Print(GeneralLib.LeftPaddedString("--", 5) + Strings.Space(5));
                }

                // 命中
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("命中", u, 4) + " ");
                upic.SetColor(StatusFontColorNormalString);
                if (is_unknown)
                {
                    upic.Print(GeneralLib.LeftPaddedString("？", 4) + Strings.Space(10));
                }
                else if (p.Data.Hit > 1)
                {
                    switch ((p.HitMod + p.HitMod2))
                    {
                        case var case4 when case4 > 0:
                            {
                                string localLeftPaddedString6() { string argbuf = SrcFormatter.Format(p.HitBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString4() { string argbuf = "+" + SrcFormatter.Format(p.HitMod + p.HitMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

                                upic.Print(localLeftPaddedString6() + localRightPaddedString4());
                                break;
                            }

                        case var case5 when case5 < 0:
                            {
                                string localLeftPaddedString7() { string argbuf = SrcFormatter.Format(p.HitBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString5() { string argbuf = SrcFormatter.Format(p.HitMod + p.HitMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

                                upic.Print(localLeftPaddedString7() + localRightPaddedString5());
                                break;
                            }

                        case 0:
                            {
                                string localLeftPaddedString8() { string argbuf = SrcFormatter.Format(p.Hit); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                upic.Print(localLeftPaddedString8() + Strings.Space(9));
                                break;
                            }
                    }
                }
                else
                {
                    upic.Print(GeneralLib.LeftPaddedString("--", 5) + Strings.Space(9));
                }

                // 回避
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("回避", u, 4) + " ");
                upic.SetColor(StatusFontColorNormalString);
                if (is_unknown)
                {
                    upic.Print(GeneralLib.LeftPaddedString("？", 4));
                }
                else if (p.Data.Dodge > 1)
                {
                    switch ((p.DodgeMod + p.DodgeMod2))
                    {
                        case var case6 when case6 > 0:
                            {
                                string localLeftPaddedString9() { string argbuf = SrcFormatter.Format(p.DodgeBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString6() { string argbuf = "+" + SrcFormatter.Format(p.DodgeMod + p.DodgeMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

                                upic.Print(localLeftPaddedString9() + localRightPaddedString6());
                                break;
                            }

                        case var case7 when case7 < 0:
                            {
                                string localLeftPaddedString10() { string argbuf = SrcFormatter.Format(p.DodgeBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString7() { string argbuf = SrcFormatter.Format(p.DodgeMod + p.DodgeMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

                                upic.Print(localLeftPaddedString10() + localRightPaddedString7());
                                break;
                            }

                        case 0:
                            {
                                string localLeftPaddedString11() { string argbuf = SrcFormatter.Format(p.Dodge); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                upic.Print(localLeftPaddedString11() + Strings.Space(9));
                                break;
                            }
                    }
                }
                else
                {
                    upic.Print(GeneralLib.LeftPaddedString("--", 5) + Strings.Space(9));
                }

                // 技量
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("技量", u, 4) + " ");
                upic.SetColor(StatusFontColorNormalString);
                if (is_unknown)
                {
                    upic.Print(GeneralLib.LeftPaddedString("？", 4) + Strings.Space(10));
                }
                else if (p.Data.Technique > 1)
                {
                    switch ((p.TechniqueMod + p.TechniqueMod2))
                    {
                        case var case8 when case8 > 0:
                            {
                                string localLeftPaddedString12() { string argbuf = SrcFormatter.Format(p.TechniqueBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString8() { string argbuf = "+" + SrcFormatter.Format(p.TechniqueMod + p.TechniqueMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

                                upic.Print(localLeftPaddedString12() + localRightPaddedString8());
                                break;
                            }

                        case var case9 when case9 < 0:
                            {
                                string localLeftPaddedString13() { string argbuf = SrcFormatter.Format(p.TechniqueBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString9() { string argbuf = SrcFormatter.Format(p.TechniqueMod + p.TechniqueMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

                                upic.Print(localLeftPaddedString13() + localRightPaddedString9());
                                break;
                            }

                        case 0:
                            {
                                string localLeftPaddedString14() { string argbuf = SrcFormatter.Format(p.Technique); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                upic.Print(localLeftPaddedString14() + Strings.Space(9));
                                break;
                            }
                    }
                }
                else
                {
                    upic.Print(GeneralLib.LeftPaddedString("--", 5) + Strings.Space(9));
                }

                // 反応
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("反応", u, 4) + " ");
                upic.SetColor(StatusFontColorNormalString);
                if (is_unknown)
                {
                    upic.Print(GeneralLib.LeftPaddedString("？", 4));
                }
                else if (p.Data.Intuition > 1)
                {
                    switch ((p.IntuitionMod + p.IntuitionMod2))
                    {
                        case var case10 when case10 > 0:
                            {
                                string localLeftPaddedString15() { string argbuf = SrcFormatter.Format(p.IntuitionBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString10() { string argbuf = "+" + SrcFormatter.Format(p.IntuitionMod + p.IntuitionMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

                                upic.Print(localLeftPaddedString15() + localRightPaddedString10());
                                break;
                            }

                        case var case11 when case11 < 0:
                            {
                                string localLeftPaddedString16() { string argbuf = SrcFormatter.Format(p.IntuitionBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                string localRightPaddedString11() { string argbuf = SrcFormatter.Format(p.IntuitionMod + p.IntuitionMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

                                upic.Print(localLeftPaddedString16() + localRightPaddedString11());
                                break;
                            }

                        case 0:
                            {
                                string localLeftPaddedString17() { string argbuf = SrcFormatter.Format(p.Intuition); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                                upic.Print(localLeftPaddedString17() + Strings.Space(9));
                                break;
                            }
                    }
                }
                else
                {
                    upic.Print(GeneralLib.LeftPaddedString("--", 5) + Strings.Space(9));
                }

                if (Expression.IsOptionDefined("防御力成長") || Expression.IsOptionDefined("防御力レベルアップ"))
                {
                    // 防御
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("防御", u) + " ");
                    upic.SetColor(StatusFontColorNormalString);
                    if (is_unknown)
                    {
                        upic.Print(GeneralLib.LeftPaddedString("？", 4));
                    }
                    else if (!p.IsSupport(u))
                    {
                        string localLeftPaddedString18() { string argbuf = SrcFormatter.Format(p.Defense); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                        upic.Print(localLeftPaddedString18());
                    }
                    else
                    {
                        upic.Print(GeneralLib.LeftPaddedString("--", 5));
                    }
                }

                // 所有するスペシャルパワー一覧
                if (p.CountSpecialPower > 0)
                {
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("スペシャルパワー", u, 18) + " ");
                    upic.SetColor(StatusFontColorNormalString);
                    if (!is_unknown)
                    {
                        var loopTo = p.CountSpecialPower;
                        for (var i = 1; i <= loopTo; i++)
                        {
                            if (p.SP < p.SpecialPowerCost(p.get_SpecialPower(i)))
                            {
                                upic.SetColor(StatusFontColorAbilityDisable);
                            }
                            upic.Print(SRC.SPDList.Item(p.get_SpecialPower(i)).ShortName);
                            upic.SetColor(StatusFontColorNormalString);
                        }
                    }
                    else
                    {
                        upic.Print("？");
                    }
                    upic.Print();
                }

                // 未識別のユニットはこれ以降の情報を表示しない
                if (is_unknown)
                {
                    upic.CurrentY = upic.CurrentY + 8;
                    goto UnitStatus;
                }

                // パイロット用特殊能力一覧
                // 霊力
                if (p.MaxPlana() > 0)
                {
                    string sname;
                    if (p.IsSkillAvailable("霊力"))
                    {
                        sname = p.SkillName("霊力");
                    }
                    else
                    {
                        // 追加パイロットは第１パイロットの霊力を代わりに使うので
                        sname = u.Pilots.First().SkillName("霊力");
                    }

                    if (Strings.InStr(sname, "非表示") == 0)
                    {
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print(sname + " ");
                        upic.SetColor(StatusFontColorNormalString);
                        if (u.PlanaLevel() < p.Plana)
                        {
                            upic.SetColor(StatusFontColorAbilityDisable);
                        }
                        upic.Print(SrcFormatter.Format(p.Plana) + "/" + SrcFormatter.Format(p.MaxPlana()));
                        upic.SetColor(StatusFontColorNormalString);
                    }
                }

                // 同調率
                if (p.SynchroRate() > 0)
                {
                    if (Strings.InStr(p.SkillName("同調率"), "非表示") == 0)
                    {
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print(p.SkillName("同調率") + " ");
                        upic.SetColor(StatusFontColorNormalString);
                        if (u.SyncLevel() < p.SynchroRate())
                        {
                            upic.SetColor(StatusFontColorAbilityDisable);
                        }
                        upic.Print(SrcFormatter.Format(p.SynchroRate()) + "%");
                        upic.SetColor(StatusFontColorNormalString);
                    }
                }

                // 得意技＆不得手
                {
                    var n = 0;
                    if (p.IsSkillAvailable("得意技"))
                    {
                        n = (n + 1);
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print("得意技 ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print(GeneralLib.RightPaddedString(p.SkillData("得意技"), 12));
                    }

                    if (p.IsSkillAvailable("不得手"))
                    {
                        n = (n + 1);
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print("不得手 ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print(p.SkillData("不得手"));
                    }

                    if (n > 0)
                    {
                        upic.Print();
                    }
                }

                {
                    // TODO Impl
                    //// 表示するパイロット能力のリストを作成
                    //var name_list = p.Skills.Select(x => x.Name).ToList();
                    //// 付加されたパイロット特殊能力
                    //var loopTo2 = u.CountCondition();
                    //for (var i = 1; i <= loopTo2; i++)
                    //{
                    //    if (u.ConditionLifetime(i) != 0)
                    //    {
                    //        string localCondition2() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                    //        switch (Strings.Right(localCondition2(), 3) ?? "")
                    //        {
                    //            case "付加２":
                    //            case "強化２":
                    //                {
                    //                    string localConditionData() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                    //                    switch (GeneralLib.LIndex(localConditionData(), 1) ?? "")
                    //                    {
                    //                        // 非表示の能力
                    //                        case "非表示":
                    //                        case "解説":
                    //                            {
                    //                                break;
                    //                            }

                    //                        default:
                    //                            {
                    //                                string localCondition() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                    //                                string localCondition1() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                    //                                stype = Strings.Left(localCondition(), Strings.Len(localCondition1()) - 3);
                    //                                switch (stype ?? "")
                    //                                {
                    //                                    case "ハンター":
                    //                                    case "ＳＰ消費減少":
                    //                                    case "スペシャルパワー自動発動":
                    //                                        {
                    //                                            // 重複可能な能力
                    //                                            Array.Resize(name_list, Information.UBound(name_list) + 1 + 1);
                    //                                            name_list[Information.UBound(name_list)] = stype;
                    //                                            break;
                    //                                        }

                    //                                    default:
                    //                                        {
                    //                                            // 既に所有している能力であればスキップ
                    //                                            var loopTo3 = Information.UBound(name_list);
                    //                                            for (j = 1; j <= loopTo3; j++)
                    //                                            {
                    //                                                if ((stype ?? "") == (name_list[j] ?? ""))
                    //                                                {
                    //                                                    break;
                    //                                                }
                    //                                            }

                    //                                            if (j > Information.UBound(name_list))
                    //                                            {
                    //                                                Array.Resize(name_list, Information.UBound(name_list) + 1 + 1);
                    //                                                name_list[Information.UBound(name_list)] = stype;
                    //                                            }

                    //                                            break;
                    //                                        }
                    //                                }

                    //                                break;
                    //                            }
                    //                    }

                    //                    break;
                    //                }
                    //        }
                    //    }
                    //}

                    //// パイロット能力を表示
                    //var n = 0;
                    //var loopTo4 = Information.UBound(name_list);
                    //for (i = 1; i <= loopTo4; i++)
                    //{
                    //    // ADD START 240a
                    //    // 文字色をリセット
                    //    upic.SetColor(StatusFontColorNormalString);                    // ADD  END  240a
                    //    stype = name_list[i];
                    //    if (i <= p.CountSkill())
                    //    {
                    //        sname = p.SkillName(i);
                    //        double localSkillLevel() { object argIndex1 = i; string argref_mode = ""; var ret = p.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

                    //        double localSkillLevel1() { object argIndex1 = i; string argref_mode = ""; var ret = p.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

                    //        slevel = localSkillLevel1().ToString();
                    //    }
                    //    else
                    //    {
                    //        sname = p.SkillName(stype);
                    //        double localSkillLevel2() { object argIndex1 = stype; string argref_mode = ""; var ret = p.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

                    //        double localSkillLevel3() { object argIndex1 = stype; string argref_mode = ""; var ret = p.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

                    //        slevel = localSkillLevel3().ToString();
                    //    }

                    //    if (Strings.InStr(sname, "非表示") > 0)
                    //    {
                    //        goto NextSkill;
                    //    }

                    //    switch (stype ?? "")
                    //    {
                    //        case "オーラ":
                    //            {
                    //                if (DisplayedPilotInd == 1)
                    //                {
                    //                    if (u.AuraLevel(true) < u.AuraLevel() && !string.IsNullOrEmpty(Map.MapFileName))
                    //                    {
                    //                        // MOD START 240a
                    //                        // upic.ForeColor = rgb(150, 0, 0)
                    //                        upic.SetColor(StatusFontColorAbilityDisable);
                    //                    }

                    //                    if (u.AuraLevel(true) > Conversions.ToDouble(slevel))
                    //                    {
                    //                        sname = sname + "+" + SrcFormatter.Format(u.AuraLevel(true) - Conversions.ToDouble(slevel));
                    //                    }
                    //                }

                    //                break;
                    //            }

                    //        case "超能力":
                    //            {
                    //                if (DisplayedPilotInd == 1)
                    //                {
                    //                    if (u.PsychicLevel(true) < u.PsychicLevel() && !string.IsNullOrEmpty(Map.MapFileName))
                    //                    {
                    //                        // MOD START 240a
                    //                        // upic.ForeColor = rgb(150, 0, 0)
                    //                        upic.SetColor(StatusFontColorAbilityDisable);
                    //                    }

                    //                    if (u.PsychicLevel(true) > Conversions.ToDouble(slevel))
                    //                    {
                    //                        sname = sname + "+" + SrcFormatter.Format(u.PsychicLevel(true) - Conversions.ToDouble(slevel));
                    //                    }
                    //                }

                    //                break;
                    //            }

                    //        case "底力":
                    //        case "超底力":
                    //        case "覚悟":
                    //            {
                    //                if (u.HP <= u.MaxHP / 4)
                    //                {
                    //                    // MOD START 240a
                    //                    // upic.ForeColor = vbBlue
                    //                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
                    //                    // MOD  END  240a
                    //                }

                    //                break;
                    //            }

                    //        case "不屈":
                    //            {
                    //                if (u.HP <= u.MaxHP / 2)
                    //                {
                    //                    // MOD START 240a
                    //                    // upic.ForeColor = vbBlue
                    //                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
                    //                    // MOD  END  240a
                    //                }

                    //                break;
                    //            }

                    //        case "潜在力開放":
                    //            {
                    //                if (p.Morale >= 130)
                    //                {
                    //                    // MOD START 240a
                    //                    // upic.ForeColor = vbBlue
                    //                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
                    //                    // MOD  END  240a
                    //                }

                    //                break;
                    //            }

                    //        case "スペシャルパワー自動発動":
                    //            {
                    //                if (i <= p.CountSkill())
                    //                {
                    //                    string localSkillData() { object argIndex1 = i; var ret = p.SkillData(argIndex1); return ret; }

                    //                    string localLIndex() { string arglist = hs4c6b0a8a8b874cdb8c87b94121d03278(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

                    //                    int localStrToLng() { string argexpr = hs23c31b1421414ea0be207e3875c77613(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                    //                    if (p.Morale >= localStrToLng())
                    //                    {
                    //                        // MOD START 240a
                    //                        // upic.ForeColor = vbBlue
                    //                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
                    //                        // MOD  END  240a
                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    string localSkillData1() { object argIndex1 = stype; var ret = p.SkillData(argIndex1); return ret; }

                    //                    string localLIndex1() { string arglist = hs5a6fb85244614ee5b61690cdbec6b0f9(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

                    //                    int localStrToLng1() { string argexpr = hsa5a03d2c426146729603becc658cd7bc(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                    //                    if (p.Morale >= localStrToLng1())
                    //                    {
                    //                        // MOD START 240a
                    //                        // upic.ForeColor = vbBlue
                    //                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
                    //                        // MOD  END  240a
                    //                    }
                    //                }

                    //                break;
                    //            }

                    //        case "Ｓ防御":
                    //            {
                    //                if (!u.IsFeatureAvailable("シールド") && !u.IsFeatureAvailable("大型シールド") && !u.IsFeatureAvailable("小型シールド") && !u.IsFeatureAvailable("エネルギーシールド") && !u.IsFeatureAvailable("アクティブシールド") && !u.IsFeatureAvailable("盾") && !u.IsFeatureAvailable("バリアシールド") && !u.IsFeatureAvailable("アクティブフィールド") && !u.IsFeatureAvailable("アクティブプロテクション") && Strings.InStr(u.FeatureData("阻止"), "Ｓ防御") == 0 && Strings.InStr(u.FeatureData("広域阻止"), "Ｓ防御") == 0 && Strings.InStr(u.FeatureData("反射"), "Ｓ防御") == 0 && Strings.InStr(u.FeatureData("当て身技"), "Ｓ防御") == 0 && Strings.InStr(u.FeatureData("自動反撃"), "Ｓ防御") == 0 && !string.IsNullOrEmpty(Map.MapFileName))
                    //                {
                    //                    // MOD START 240a
                    //                    // upic.ForeColor = rgb(150, 0, 0)
                    //                    upic.SetColor(StatusFontColorAbilityDisable);
                    //                }

                    //                break;
                    //            }

                    //        case "切り払い":
                    //            {
                    //                var loopTo5 = u.CountWeapon();
                    //                for (j = 1; j <= loopTo5; j++)
                    //                {
                    //                    if (u.IsWeaponClassifiedAs(j, "武"))
                    //                    {
                    //                        if (!u.IsDisabled(u.Weapon(j).Name))
                    //                        {
                    //                            break;
                    //                        }
                    //                    }
                    //                }

                    //                if (u.IsFeatureAvailable("格闘武器"))
                    //                {
                    //                    j = 0;
                    //                }

                    //                if (j > u.CountWeapon() && Strings.InStr(u.FeatureData("阻止"), "切り払い") == 0 && Strings.InStr(u.FeatureData("広域阻止"), "切り払い") == 0 && Strings.InStr(u.FeatureData("反射"), "切り払い") == 0 && Strings.InStr(u.FeatureData("当て身技"), "切り払い") == 0 && Strings.InStr(u.FeatureData("自動反撃"), "切り払い") == 0 && !string.IsNullOrEmpty(Map.MapFileName))
                    //                {
                    //                    // MOD START 240a
                    //                    // upic.ForeColor = rgb(150, 0, 0)
                    //                    upic.SetColor(StatusFontColorAbilityDisable);
                    //                }

                    //                break;
                    //            }

                    //        case "迎撃":
                    //            {
                    //                var loopTo6 = u.CountWeapon();
                    //                for (j = 1; j <= loopTo6; j++)
                    //                {
                    //                    if (u.IsWeaponAvailable(j, "移動後") && u.IsWeaponClassifiedAs(j, "射撃系") && (u.Weapon(j).Bullet >= 10 || u.Weapon(j).Bullet == 0 && u.Weapon(j).ENConsumption <= 5))
                    //                    {
                    //                        break;
                    //                    }
                    //                }

                    //                if (u.IsFeatureAvailable("迎撃武器"))
                    //                {
                    //                    j = 0;
                    //                }

                    //                if (j > u.CountWeapon() && Strings.InStr(u.FeatureData("阻止"), "迎撃") == 0 && Strings.InStr(u.FeatureData("広域阻止"), "迎撃") == 0 && Strings.InStr(u.FeatureData("反射"), "迎撃") == 0 && Strings.InStr(u.FeatureData("当て身技"), "迎撃") == 0 && Strings.InStr(u.FeatureData("自動反撃"), "迎撃") == 0 && !string.IsNullOrEmpty(Map.MapFileName))
                    //                {
                    //                    // MOD START 240a
                    //                    // upic.ForeColor = rgb(150, 0, 0)
                    //                    upic.SetColor(StatusFontColorAbilityDisable);
                    //                }

                    //                break;
                    //            }

                    //        case "浄化":
                    //            {
                    //                var loopTo7 = u.CountWeapon();
                    //                for (j = 1; j <= loopTo7; j++)
                    //                {
                    //                    if (u.IsWeaponClassifiedAs(j, "浄"))
                    //                    {
                    //                        if (!u.IsDisabled(u.Weapon(j).Name))
                    //                        {
                    //                            break;
                    //                        }
                    //                    }
                    //                }

                    //                if (j > u.CountWeapon() && !string.IsNullOrEmpty(Map.MapFileName))
                    //                {
                    //                    // MOD START 240a
                    //                    // upic.ForeColor = rgb(150, 0, 0)
                    //                    upic.SetColor(StatusFontColorAbilityDisable);
                    //                }

                    //                break;
                    //            }

                    //        case "援護":
                    //            {
                    //                if (!string.IsNullOrEmpty(Map.MapFileName))
                    //                {
                    //                    if ((u.Party ?? "") == (SRC.Stage ?? ""))
                    //                    {
                    //                        ret = GeneralLib.MaxLng(u.MaxSupportAttack() - u.UsedSupportAttack, 0);
                    //                    }
                    //                    else
                    //                    {
                    //                        if (u.IsUnderSpecialPowerEffect("サポートガード不能"))
                    //                        {
                    //                            // MOD START 240a
                    //                            // upic.ForeColor = rgb(150, 0, 0)
                    //                            upic.SetColor(StatusFontColorAbilityDisable);
                    //                        }

                    //                        ret = GeneralLib.MaxLng(u.MaxSupportGuard() - u.UsedSupportGuard, 0);
                    //                    }

                    //                    if (ret == 0)
                    //                    {
                    //                        // MOD START 240a
                    //                        // upic.ForeColor = rgb(150, 0, 0)
                    //                        upic.SetColor(StatusFontColorAbilityDisable);
                    //                    }

                    //                    sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
                    //                }

                    //                break;
                    //            }

                    //        case "援護攻撃":
                    //            {
                    //                if (!string.IsNullOrEmpty(Map.MapFileName))
                    //                {
                    //                    ret = GeneralLib.MaxLng(u.MaxSupportAttack() - u.UsedSupportAttack, 0);
                    //                    if (ret == 0)
                    //                    {
                    //                        // MOD START 240a
                    //                        // upic.ForeColor = rgb(150, 0, 0)
                    //                        upic.SetColor(StatusFontColorAbilityDisable);
                    //                    }

                    //                    sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
                    //                }

                    //                break;
                    //            }

                    //        case "援護防御":
                    //            {
                    //                if (!string.IsNullOrEmpty(Map.MapFileName))
                    //                {
                    //                    ret = GeneralLib.MaxLng(u.MaxSupportGuard() - u.UsedSupportGuard, 0);
                    //                    if (ret == 0 || u.IsUnderSpecialPowerEffect("サポートガード不能"))
                    //                    {
                    //                        // MOD START 240a
                    //                        // upic.ForeColor = rgb(150, 0, 0)
                    //                        upic.SetColor(StatusFontColorAbilityDisable);
                    //                    }

                    //                    sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
                    //                }

                    //                break;
                    //            }

                    //        case "統率":
                    //            {
                    //                if (!string.IsNullOrEmpty(Map.MapFileName))
                    //                {
                    //                    ret = GeneralLib.MaxLng(u.MaxSyncAttack() - u.UsedSyncAttack, 0);
                    //                    if (ret == 0)
                    //                    {
                    //                        // MOD START 240a
                    //                        // upic.ForeColor = rgb(150, 0, 0)
                    //                        upic.SetColor(StatusFontColorAbilityDisable);
                    //                    }

                    //                    sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
                    //                }

                    //                break;
                    //            }

                    //        case "カウンター":
                    //            {
                    //                if (!string.IsNullOrEmpty(Map.MapFileName))
                    //                {
                    //                    ret = GeneralLib.MaxLng(u.MaxCounterAttack() - u.UsedCounterAttack, 0);
                    //                    if (ret > 100)
                    //                    {
                    //                        sname = sname + " (残り∞回)";
                    //                    }
                    //                    else if (ret > 0)
                    //                    {
                    //                        sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
                    //                    }
                    //                    else
                    //                    {
                    //                        // MOD START 240a
                    //                        // upic.ForeColor = rgb(150, 0, 0)
                    //                        upic.SetColor(StatusFontColorAbilityDisable);
                    //                        sname = sname + " (残り0回)";
                    //                    }
                    //                }

                    //                break;
                    //            }

                    //        case "先手必勝":
                    //            {
                    //                if (u.MaxCounterAttack() > 100)
                    //                {
                    //                    // MOD START 240a
                    //                    // upic.ForeColor = vbBlue
                    //                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
                    //                    // MOD  END  240a
                    //                }

                    //                break;
                    //            }

                    //        case "耐久":
                    //            {
                    //                if (Expression.IsOptionDefined("防御力成長") || Expression.IsOptionDefined("防御力レベルアップ"))
                    //                {
                    //                    goto NextSkill;
                    //                }

                    //                break;
                    //            }

                    //        case "霊力":
                    //        case "同調率":
                    //        case "得意技":
                    //        case "不得手":
                    //            {
                    //                goto NextSkill;
                    //                break;
                    //            }
                    //    }

                    //    // 特殊能力名を表示
                    //    if (LenB(Strings.StrConv(sname, vbFromUnicode)) > 19)
                    //    {
                    //        if (n > 0)
                    //        {
                    //            upic.Print();
                    //            // ADD START 240a
                    //            if (GUI.NewGUIMode)
                    //            {
                    //                upic.CurrentX = 5;
                    //            }
                    //            // ADD  END  240a
                    //        }
                    //        upic.Print(sname);
                    //        n = 2;
                    //    }
                    //    else
                    //    {
                    //        upic.Print(GeneralLib.RightPaddedString(sname, 19));
                    //        n = (n + 1);
                    //    }

                    //    upic.ForeColor = Color.Black;

                    //    // 必要に応じて改行
                    //    if (n > 1)
                    //    {
                    //        upic.Print();
                    //        // ADD START 240a
                    //        if (GUI.NewGUIMode)
                    //        {
                    //            upic.CurrentX = 5;
                    //        }
                    //        // ADD  END  240a
                    //        n = 0;
                    //    }

                    //NextSkill:
                    //    ;
                    //}

                    //if (n > 0)
                    //{
                    //    upic.Print();
                    //}

                    //upic.CurrentY = upic.CurrentY + 8;
                }
            UnitStatus:
                ;

                // パイロットステータス表示用のダミーユニットの場合はここで表示を終了
                if (u.IsFeatureAvailable("ダミーユニット"))
                {
                    goto UpdateStatusWindow;
                }

                // ここからはユニットに関する情報

                // ユニット愛称
                var offset = new Point();
                offset.Y = textMargin * 8;
                upic.SetFont(HeadingFont);
                upic.Print(p.get_Nickname(false));
                upic.SetFont(StatusSmallFont);
                if (u.Status == "出撃" && !string.IsNullOrEmpty(Map.MapFileName))
                {
                    // 地形情報の表示
                    // ユニットの位置を地形名称
                    if (Strings.InStr(Map.TerrainName(u.x, u.y), "(") > 0)
                    {
                        upic.Print(u.Area + " (" + Strings.Left(Map.TerrainName(u.x, u.y), Strings.InStr(Map.TerrainName(u.x, u.y), "(") - 1));
                    }
                    else
                    {
                        upic.Print(u.Area + " (" + Map.TerrainName(u.x, u.y));
                    }

                    // 回避＆防御修正
                    if (Map.TerrainEffectForHit(u.x, u.y) == Map.TerrainEffectForDamage(u.x, u.y))
                    {
                        if (Map.TerrainEffectForHit(u.x, u.y) >= 0)
                        {
                            upic.Print(" 回＆防+" + SrcFormatter.Format(Map.TerrainEffectForHit(u.x, u.y)) + "%");
                        }
                        else
                        {
                            upic.Print(" 回＆防" + SrcFormatter.Format(Map.TerrainEffectForHit(u.x, u.y)) + "%");
                        }
                    }
                    else
                    {
                        if (Map.TerrainEffectForHit(u.x, u.y) >= 0)
                        {
                            upic.Print(" 回+" + SrcFormatter.Format(Map.TerrainEffectForHit(u.x, u.y)) + "%");
                        }
                        else
                        {
                            upic.Print(" 回" + SrcFormatter.Format(Map.TerrainEffectForHit(u.x, u.y)) + "%");
                        }

                        if (Map.TerrainEffectForDamage(u.x, u.y) >= 0)
                        {
                            upic.Print(" 防+" + SrcFormatter.Format(Map.TerrainEffectForDamage(u.x, u.y)) + "%");
                        }
                        else
                        {
                            upic.Print(" 防" + SrcFormatter.Format(Map.TerrainEffectForDamage(u.x, u.y)) + "%");
                        }
                    }

                    // ＨＰ＆ＥＮ回復
                    if (Map.TerrainEffectForHPRecover(u.x, u.y) > 0)
                    {
                        upic.Print(" " + Strings.Left(Expression.Term("ＨＰ", u: null), 1) + "+" + SrcFormatter.Format(Map.TerrainEffectForHPRecover(u.x, u.y)) + "%");
                    }

                    if (Map.TerrainEffectForENRecover(u.x, u.y) > 0)
                    {
                        upic.Print(" " + Strings.Left(Expression.Term("ＥＮ", u: null), 1) + "+" + SrcFormatter.Format(Map.TerrainEffectForENRecover(u.x, u.y)) + "%");
                    }

                    // MOD START 240a
                    // Set td = TDList.Item(MapData(.X, .Y, 0))
                    // マスのタイプに応じて参照先を変更
                    switch (Map.MapData[u.x, u.y, Map.MapDataIndex.BoxType])
                    {
                        case Map.BoxTypes.Under:
                        case Map.BoxTypes.UpperBmpOnly:
                            {
                                td = SRC.TDList.Item(Map.MapData[u.x, u.y, Map.MapDataIndex.TerrainType]);
                                break;
                            }

                        default:
                            {
                                td = SRC.TDList.Item(Map.MapData[u.x, u.y, Map.MapDataIndex.LayerType]);
                                break;
                            }
                    }
                    // MOD START 240a
                    // ＨＰ＆ＥＮ減少
                    if (td.IsFeatureAvailable("ＨＰ減少"))
                    {
                        upic.Print(" " + Strings.Left(Expression.Term("ＨＰ", u: null), 1) + "-" + SrcFormatter.Format(10d * td.FeatureLevel("ＨＰ減少")) + "%");
                    }

                    if (td.IsFeatureAvailable("ＥＮ減少"))
                    {
                        upic.Print(" " + Strings.Left(Expression.Term("ＥＮ", u: null), 1) + "-" + SrcFormatter.Format(10d * td.FeatureLevel("ＥＮ減少")) + "%");
                    }

                    // ＨＰ＆ＥＮ増加
                    if (td.IsFeatureAvailable("ＨＰ増加"))
                    {
                        upic.Print(" " + Strings.Left(Expression.Term("ＨＰ", u: null), 1) + "+" + SrcFormatter.Format(1000d * td.FeatureLevel("ＨＰ増加")));
                    }

                    if (td.IsFeatureAvailable("ＥＮ増加"))
                    {
                        upic.Print(" " + Strings.Left(Expression.Term("ＥＮ", u: null), 1) + "+" + SrcFormatter.Format(10d * td.FeatureLevel("ＥＮ増加")));
                    }

                    // ＨＰ＆ＥＮ低下
                    if (td.IsFeatureAvailable("ＨＰ低下"))
                    {
                        upic.Print(" " + Strings.Left(Expression.Term("ＨＰ", u: null), 1) + "-" + SrcFormatter.Format(1000d * td.FeatureLevel("ＨＰ低下")));
                    }

                    if (td.IsFeatureAvailable("ＥＮ低下"))
                    {
                        upic.Print(" " + Strings.Left(Expression.Term("ＥＮ", u: null), 1) + "-" + SrcFormatter.Format(10d * td.FeatureLevel("ＥＮ低下")));
                    }

                    // 摩擦
                    if (td.IsFeatureAvailable("摩擦"))
                    {
                        upic.Print(" 摩L" + SrcFormatter.Format(td.FeatureLevel("摩擦")));
                    }

                    upic.Print(")");
                }
                else
                {
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("ランク ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(SrcFormatter.Format((object)u.Rank));
                }

                // 未確認ユニット？
                if (is_unknown)
                {
                    // ＨＰ
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("ＨＰ", null, 6) + " ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print("?????/?????");

                    // ＥＮ
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("ＥＮ", null, 6) + " ");
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print("???/???");

                    // 装甲
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("装甲", null, 6) + " ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(GeneralLib.RightPaddedString("？", 12));

                    // 運動性
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("運動性", null, 6) + " ");
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print("？");

                    if (GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // 移動タイプ
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("タイプ", null, 6) + " ");
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(GeneralLib.RightPaddedString("？", 12));

                    // 移動力
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("移動力", null, 6) + " ");
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print("？");

                    // 地形適応
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("適応   ");
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(GeneralLib.RightPaddedString("？", 12));

                    // ユニットサイズ
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("サイズ", null, 6) + " ");
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print("？");

                    // サポートアタックを得られるかどうかのみ表示
                    if ((Commands.CommandState == "ターゲット選択" || Commands.CommandState == "移動後ターゲット選択") && (Commands.SelectedCommand == "攻撃" || Commands.SelectedCommand == "マップ攻撃") && Commands.SelectedUnit is object)
                    {
                        if (u.Party == "敵" || u.Party == "中立" || u.IsConditionSatisfied("暴走") || u.IsConditionSatisfied("魅了") || u.IsConditionSatisfied("憑依"))
                        {
                            upic.Print();

                            // 攻撃手段
                            // MOD START 240a
                            // upic.ForeColor = rgb(0, 0, 150)
                            upic.SetColor(StatusFontColorAbilityName);
                            upic.Print("攻撃     ");
                            // MOD START 240a
                            // upic.ForeColor = rgb(0, 0, 0)
                            upic.SetColor(StatusFontColorNormalString);
                            upic.Print(Commands.SelectedUnit.WeaponNickname(Commands.SelectedWeapon));
                            // サポートアタックを得られる？
                            if (!Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, "合") && !Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, "Ｍ"))
                            {
                                if (Commands.SelectedUnit.LookForSupportAttack(u) is object)
                                {
                                    upic.Print(" [援]");
                                }
                            }
                        }
                    }

                    goto UpdateStatusWindow;
                }

                // 実行中の命令
                if (u.Party == "ＮＰＣ" && !u.IsConditionSatisfied("混乱") && !u.IsConditionSatisfied("恐怖") && !u.IsConditionSatisfied("暴走") && !u.IsConditionSatisfied("狂戦士"))
                {
                    // 思考モードを見れば実行している命令が分かるので……
                    buf = "";
                    if (u.IsConditionSatisfied("魅了"))
                    {
                        if (u.Master is object)
                        {
                            if (u.Master.Party == "味方")
                            {
                                buf = u.Mode;
                            }
                        }
                    }

                    if (u.IsFeatureAvailable("召喚ユニット") && !u.IsConditionSatisfied("魅了"))
                    {
                        if (u.Summoner is object)
                        {
                            if (u.Summoner.Party == "味方")
                            {
                                buf = u.Mode;
                            }
                        }
                    }

                    bool localIsDefined() { object argIndex1 = buf; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

                    if (buf == "通常")
                    {
                        upic.Print("自由行動中");
                    }
                    else if (localIsDefined())
                    {
                        // 思考モードにパイロット名が指定されている場合
                        {
                            var withBlock1 = SRC.PList.Item(buf);
                            if (withBlock1.Unit is object)
                            {
                                {
                                    var withBlock2 = withBlock1.Unit;
                                    if (withBlock2.Status == "出撃")
                                    {
                                        upic.Print(withBlock2.Nickname + "(" + SrcFormatter.Format(withBlock2.x) + "," + SrcFormatter.Format(withBlock2.y) + ")を");
                                        if (withBlock2.Party == "味方")
                                        {
                                            upic.Print("護衛中");
                                        }
                                        else
                                        {
                                            upic.Print("追跡中");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else if (GeneralLib.LLength(buf) == 2)
                    {
                        // 思考モードに座標が指定されている場合
                        upic.Print("(" + GeneralLib.LIndex(buf, 1) + "," + GeneralLib.LIndex(buf, 2) + ")に移動中");
                    }
                }

                // ユニットにかかっている特殊ステータス
                name_list = new string[1];
                var loopTo8 = u.CountCondition();
                for (i = 1; i <= loopTo8; i++)
                {
                    // 時間切れ？
                    if (u.ConditionLifetime(i) == 0)
                    {
                        goto NextCondition;
                    }

                    // 非表示？
                    string localConditionData1() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                    if (Strings.InStr(localConditionData1(), "非表示") > 0)
                    {
                        goto NextCondition;
                    }

                    // 解説？
                    string localConditionData2() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                    if (GeneralLib.LIndex(localConditionData2(), 1) == "解説")
                    {
                        goto NextCondition;
                    }
                    // ADD START 240a
                    if (GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // ADD  END  240a
                    switch (u.Condition(i) ?? "")
                    {
                        case "データ不明":
                        case "形態固定":
                        case "機体固定":
                        case "不死身":
                        case "無敵":
                        case "識別済み":
                        case "非操作":
                        case "破壊キャンセル":
                        case "盾ダメージ":
                        case "能力コピー":
                        case "メッセージ付加":
                        case "ノーマルモード付加":
                        case "追加パイロット付加":
                        case "追加サポート付加":
                        case "パイロット愛称付加":
                        case "パイロット画像付加":
                        case "性格変更付加":
                        case "性別付加":
                        case "ＢＧＭ付加":
                        case "愛称変更付加":
                        case "スペシャルパワー無効化":
                        case "精神コマンド無効化":
                        case "ユニット画像付加":
                        case var case12 when case12 == "メッセージ付加":
                            {
                                break;
                            }
                        // 非表示
                        case "残り時間":
                            {
                                int localConditionLifetime1() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime2() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime1() && localConditionLifetime2() < 100)
                                {
                                    int localConditionLifetime() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print("残り時間" + SrcFormatter.Format(localConditionLifetime()) + "ターン");
                                }

                                break;
                            }

                        case "無効化付加":
                        case "耐性付加":
                        case "吸収付加":
                        case "弱点付加":
                            {
                                string localConditionData3() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                string localCondition3() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                upic.Print(localConditionData3() + localCondition3());
                                int localConditionLifetime4() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime5() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime4() && localConditionLifetime5() < 100)
                                {
                                    int localConditionLifetime3() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" " + SrcFormatter.Format(localConditionLifetime3()) + "T");
                                }
                                upic.Print("");
                                break;
                            }

                        case "特殊効果無効化付加":
                            {
                                string localConditionData4() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                upic.Print(localConditionData4() + "無効化付加");
                                int localConditionLifetime7() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime8() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime7() && localConditionLifetime8() < 100)
                                {
                                    int localConditionLifetime6() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime6()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "攻撃属性付加":
                            {
                                string localConditionData5() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                string localLIndex2() { string arglist = hs755742c2c238431abd43e11d0920ad14(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                                upic.Print(localLIndex2() + "属性付加");
                                int localConditionLifetime10() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime11() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime10() && localConditionLifetime11() < 100)
                                {
                                    int localConditionLifetime9() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime9()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "武器強化付加":
                            {
                                double localConditionLevel() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                                upic.Print("武器強化Lv" + localConditionLevel() + "付加");
                                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                                {
                                    string localConditionData6() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                    upic.Print("(" + localConditionData6() + ")");
                                }

                                int localConditionLifetime13() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime14() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime13() && localConditionLifetime14() < 100)
                                {
                                    int localConditionLifetime12() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime12()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "命中率強化付加":
                            {
                                double localConditionLevel1() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                                upic.Print("命中率強化Lv" + localConditionLevel1() + "付加");
                                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                                {
                                    string localConditionData7() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                    upic.Print("(" + localConditionData7() + ")");
                                }

                                int localConditionLifetime16() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime17() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime16() && localConditionLifetime17() < 100)
                                {
                                    int localConditionLifetime15() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime15()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "ＣＴ率強化付加":
                            {
                                double localConditionLevel2() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                                upic.Print("ＣＴ率強化Lv" + localConditionLevel2() + "付加");
                                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                                {
                                    string localConditionData8() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                    upic.Print("(" + localConditionData8() + ")");
                                }

                                int localConditionLifetime19() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime20() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime19() && localConditionLifetime20() < 100)
                                {
                                    int localConditionLifetime18() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime18()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "特殊効果発動率強化付加":
                            {
                                double localConditionLevel3() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                                upic.Print("特殊効果発動率強化Lv" + localConditionLevel3() + "付加");
                                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                                {
                                    string localConditionData9() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                    upic.Print("(" + localConditionData9() + ")");
                                }

                                int localConditionLifetime22() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime23() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime22() && localConditionLifetime23() < 100)
                                {
                                    int localConditionLifetime21() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime21()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "地形適応変更付加":
                            {
                                upic.Print("地形適応変更付加");
                                int localConditionLifetime25() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime26() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime25() && localConditionLifetime26() < 100)
                                {
                                    int localConditionLifetime24() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime24()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "盾付加":
                            {
                                string localConditionData10() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                string localLIndex3() { string arglist = hsba8faef602a144028d0b911086dca487(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                                upic.Print(localLIndex3() + "付加");
                                double localConditionLevel4() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                                upic.Print("(" + SrcFormatter.Format(localConditionLevel4()) + ")");
                                int localConditionLifetime28() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime29() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime28() && localConditionLifetime29() < 100)
                                {
                                    int localConditionLifetime27() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime27()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "ダミー破壊":
                            {
                                buf = u.FeatureName("ダミー");
                                if (Strings.InStr(buf, "Lv") > 0)
                                {
                                    buf = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
                                }
                                double localConditionLevel5() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                                upic.Print(buf + Strings.StrConv(SrcFormatter.Format(localConditionLevel5()), VbStrConv.Wide) + "体破壊");
                                break;
                            }

                        case "ダミー付加":
                            {
                                double localConditionLevel6() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                                upic.Print(u.FeatureName("ダミー") + "残り" + Strings.StrConv(SrcFormatter.Format(localConditionLevel6()), VbStrConv.Wide) + "体");
                                int localConditionLifetime31() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime32() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime31() && localConditionLifetime32() < 100)
                                {
                                    int localConditionLifetime30() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime30()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "バリア発動":
                            {
                                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                                {
                                    string localConditionData11() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                    upic.Print(localConditionData11());
                                }
                                else
                                {
                                    upic.Print("バリア発動");
                                }
                                int localConditionLifetime33() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime33()) + "ターン");
                                break;
                            }

                        case "フィールド発動":
                            {
                                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                                {
                                    string localConditionData12() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                    upic.Print(localConditionData12());
                                }
                                else
                                {
                                    upic.Print("フィールド発動");
                                }
                                int localConditionLifetime34() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime34()) + "ターン");
                                break;
                            }

                        case "装甲劣化":
                            {
                                upic.Print(Expression.Term("装甲", u) + "劣化");
                                int localConditionLifetime36() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime37() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime36() && localConditionLifetime37() < 20)
                                {
                                    int localConditionLifetime35() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime35()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "運動性ＵＰ":
                            {
                                upic.Print(Expression.Term("運動性", u) + "ＵＰ");
                                int localConditionLifetime39() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime40() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime39() && localConditionLifetime40() < 20)
                                {
                                    int localConditionLifetime38() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime38()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "運動性ＤＯＷＮ":
                            {
                                upic.Print(Expression.Term("運動性", u) + "ＤＯＷＮ");
                                int localConditionLifetime42() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime43() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime42() && localConditionLifetime43() < 20)
                                {
                                    int localConditionLifetime41() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime41()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "移動力ＵＰ":
                            {
                                upic.Print(Expression.Term("移動力", u) + "ＵＰ");
                                int localConditionLifetime45() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime46() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime45() && localConditionLifetime46() < 20)
                                {
                                    int localConditionLifetime44() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime44()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        case "移動力ＤＯＷＮ":
                            {
                                upic.Print(Expression.Term("移動力", u) + "ＤＯＷＮ");
                                int localConditionLifetime48() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime49() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime48() && localConditionLifetime49() < 20)
                                {
                                    int localConditionLifetime47() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime47()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }

                        default:
                            {
                                // 充填中？
                                string localCondition7() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                if (Strings.Right(localCondition7(), 3) == "充填中")
                                {
                                    if (u.IsHero())
                                    {
                                        string localCondition4() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                        string localCondition5() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                        upic.Print(Strings.Left(localCondition4(), Strings.Len(localCondition5()) - 3));
                                        upic.Print("準備中");
                                    }
                                    else
                                    {
                                        string localCondition6() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                        upic.Print(localCondition6());
                                    }
                                    int localConditionLifetime50() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime50()) + "ターン");
                                    goto NextCondition;
                                }

                                // パイロット特殊能力付加＆強化による状態は表示しない
                                string localCondition8() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                string localCondition9() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                if (Strings.Right(localCondition8(), 3) == "付加２" || Strings.Right(localCondition9(), 3) == "強化２")
                                {
                                    goto NextCondition;
                                }

                                string localCondition12() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                string localConditionData15() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                string localCondition13() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                string localConditionData16() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                double localConditionLevel9() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                                if (Strings.Right(localCondition12(), 2) == "付加" && !string.IsNullOrEmpty(localConditionData15()))
                                {
                                    string localConditionData13() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                    buf = GeneralLib.LIndex(localConditionData13(), 1) + "付加";
                                }
                                else if (Strings.Right(localCondition13(), 2) == "強化" && !string.IsNullOrEmpty(localConditionData16()))
                                {
                                    // 強化アビリティ
                                    string localConditionData14() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                                    string localLIndex4() { string arglist = hs68f2e5d1358d41f987f02a7379e2b562(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                                    double localConditionLevel7() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                                    buf = localLIndex4() + "強化Lv" + localConditionLevel7();
                                }
                                else if (localConditionLevel9() > 0d)
                                {
                                    // 付加アビリティ(レベル指定あり)
                                    string localCondition10() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                    string localCondition11() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                                    double localConditionLevel8() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                                    buf = Strings.Left(localCondition10(), Strings.Len(localCondition11()) - 2) + "Lv" + SrcFormatter.Format(localConditionLevel8()) + "付加";
                                }
                                else
                                {
                                    // 付加アビリティ(レベル指定なし)
                                    buf = u.Condition(i);
                                }

                                // エリアスされた特殊能力の付加表示がたぶらないように
                                var loopTo9 = Information.UBound(name_list);
                                for (j = 1; j <= loopTo9; j++)
                                {
                                    if ((buf ?? "") == (name_list[j] ?? ""))
                                    {
                                        break;
                                    }
                                }

                                if (j <= Information.UBound(name_list))
                                {
                                    goto NextCondition;
                                }

                                Array.Resize(name_list, Information.UBound(name_list) + 1 + 1);
                                name_list[Information.UBound(name_list)] = buf;

                                upic.Print(buf);
                                int localConditionLifetime52() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                int localConditionLifetime53() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                if (0 < localConditionLifetime52() && localConditionLifetime53() < 20)
                                {
                                    int localConditionLifetime51() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime51()) + "ターン");
                                }
                                upic.Print("");
                                break;
                            }
                    }

                NextCondition:
                    ;
                }

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // ＨＰ
                cx = upic.CurrentX;
                cy = upic.CurrentY;
                upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                upic.Line(117, cy + 8); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                upic.Line(118 + GUI.GauageWidth, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                if (u.HP > 0)
                {
                    upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                }
                upic.CurrentX = cx;
                upic.CurrentY = cy;
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 150)
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("ＨＰ", u, 6) + " ");
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 0)
                upic.SetColor(StatusFontColorNormalString);
                if (u.IsConditionSatisfied("データ不明"))
                {
                    upic.Print("?????/?????");
                }
                else
                {
                    if (u.HP < 100000)
                    {
                        upic.Print(SrcFormatter.Format(u.HP));
                    }
                    else
                    {
                        upic.Print("?????");
                    }
                    upic.Print("/");
                    if (u.MaxHP < 100000)
                    {
                        upic.Print(SrcFormatter.Format(u.MaxHP));
                    }
                    else
                    {
                        upic.Print("?????");
                    }
                }

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // ＥＮ
                cx = upic.CurrentX;
                cy = upic.CurrentY;
                upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                upic.Line(117, cy + 8); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                upic.Line(118 + GUI.GauageWidth, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                if (u.EN > 0)
                {
                    upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                }
                upic.CurrentX = cx;
                upic.CurrentY = cy;
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 150)
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("ＥＮ", u, 6) + " ");
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 0)
                upic.SetColor(StatusFontColorNormalString);
                if (u.IsConditionSatisfied("データ不明"))
                {
                    upic.Print("???/???");
                }
                else
                {
                    if (u.EN < 1000)
                    {
                        upic.Print(SrcFormatter.Format(u.EN));
                    }
                    else
                    {
                        upic.Print("???");
                    }
                    upic.Print("/");
                    if (u.MaxEN < 1000)
                    {
                        upic.Print(SrcFormatter.Format(u.MaxEN));
                    }
                    else
                    {
                        upic.Print("???");
                    }
                }

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // 装甲
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 150)
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("装甲", u, 6) + " ");
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 0)
                upic.SetColor(StatusFontColorNormalString);
                switch (u.get_Armor("修正値"))
                {
                    case var case13 when case13 > 0:
                        {
                            string localRightPaddedString13() { string argbuf = SrcFormatter.Format(u.get_Armor("基本値")) + "+" + SrcFormatter.Format(u.get_Armor("修正値")); var ret = GeneralLib.RightPaddedString(argbuf, 12); return ret; }

                            upic.Print(localRightPaddedString13());
                            break;
                        }

                    case var case14 when case14 < 0:
                        {
                            string localRightPaddedString14() { string argbuf = SrcFormatter.Format(u.get_Armor("基本値")) + SrcFormatter.Format(u.get_Armor("修正値")); var ret = GeneralLib.RightPaddedString(argbuf, 12); return ret; }

                            upic.Print(localRightPaddedString14());
                            break;
                        }

                    case 0:
                        {
                            string localRightPaddedString15() { string argbuf = SrcFormatter.Format(u.get_Armor("")); var ret = GeneralLib.RightPaddedString(argbuf, 12); return ret; }

                            upic.Print(localRightPaddedString15());
                            break;
                        }
                }

                // 運動性
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 150)
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("運動性", u, 6) + " ");
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 0)
                upic.SetColor(StatusFontColorNormalString);
                switch (u.get_Mobility("修正値"))
                {
                    case var case15 when case15 > 0:
                        {
                            upic.Print(SrcFormatter.Format(u.get_Mobility("基本値")) + "+" + SrcFormatter.Format(u.get_Mobility("修正値")));
                            break;
                        }

                    case var case16 when case16 < 0:
                        {
                            upic.Print(SrcFormatter.Format(u.get_Mobility("基本値")) + SrcFormatter.Format(u.get_Mobility("修正値")));
                            break;
                        }

                    case 0:
                        {
                            upic.Print(SrcFormatter.Format(u.get_Mobility("")));
                            break;
                        }
                }

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // 移動タイプ
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 150)
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("タイプ", u, 6) + " ");
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 0)
                upic.SetColor(StatusFontColorNormalString);
                string localRightPaddedString16() { string argbuf = u.Transportation; var ret = GeneralLib.RightPaddedString(argbuf, 12); u.Transportation = argbuf; return ret; }

                upic.Print(localRightPaddedString16());

                // 移動力
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 150)
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("移動力", u, 6) + " ");
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 0)
                upic.SetColor(StatusFontColorNormalString);
                string localLIndex5() { object argIndex1 = "テレポート"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                if (u.IsFeatureAvailable("テレポート") && (u.Data.Speed == 0 || localLIndex5() == "0"))
                {
                    upic.Print(SrcFormatter.Format(u.Speed + u.FeatureLevel("テレポート")));
                }
                else
                {
                    upic.Print(SrcFormatter.Format(u.Speed));
                }

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // 地形適応
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 150)
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print("適応   ");
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 0)
                upic.SetColor(StatusFontColorNormalString);
                for (i = 1; i <= 4; i++)
                {
                    switch (u.get_Adaption(i))
                    {
                        case 5:
                            {
                                upic.Print("S");
                                break;
                            }

                        case 4:
                            {
                                upic.Print("A");
                                break;
                            }

                        case 3:
                            {
                                upic.Print("B");
                                break;
                            }

                        case 2:
                            {
                                upic.Print("C");
                                break;
                            }

                        case 1:
                            {
                                upic.Print("D");
                                break;
                            }

                        default:
                            {
                                upic.Print("E");
                                break;
                            }
                    }
                }
                upic.Print(Strings.Space(8));

                // ユニットサイズ
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 150)
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print(Expression.Term("サイズ", u, 6) + " ");
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 0)
                upic.SetColor(StatusFontColorNormalString);
                upic.Print(Strings.StrConv((string)u.Size, VbStrConv.Wide));

                // 防御属性の表示
                n = 0;

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // 吸収
                if (Strings.Len((string)u.strAbsorb) > 0 && Strings.InStr((string)u.strAbsorb, "非表示") == 0)
                {
                    if (Strings.Len((string)u.strAbsorb) > 5)
                    {
                        if (n > 0)
                        {
                            upic.Print();
                        }

                        n = 2;
                    }
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("吸収   ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(GeneralLib.RightPaddedString((string)u.strAbsorb, 12));
                    n = (n + 1);
                    if (n > 1)
                    {
                        upic.Print();
                        // ADD START 240a
                        if (GUI.NewGUIMode)
                        {
                            upic.CurrentX = 5;
                        }
                        // ADD  END  240a
                        n = 0;
                    }
                }

                // 無効化
                if (Strings.Len((string)u.strImmune) > 0 && Strings.InStr((string)u.strImmune, "非表示") == 0)
                {
                    if (Strings.Len((string)u.strImmune) > 5)
                    {
                        if (n > 0)
                        {
                            upic.Print();
                            // ADD START 240a
                            if (GUI.NewGUIMode)
                            {
                                upic.CurrentX = 5;
                            }
                            // ADD  END  240a
                        }

                        n = 2;
                    }
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("無効化 ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(GeneralLib.RightPaddedString((string)u.strImmune, 12));
                    n = (n + 1);
                    if (n > 1)
                    {
                        upic.Print();
                        // ADD START 240a
                        if (GUI.NewGUIMode)
                        {
                            upic.CurrentX = 5;
                        }
                        // ADD  END  240a
                        n = 0;
                    }
                }

                // 耐性
                if (Strings.Len((string)u.strResist) > 0 && Strings.InStr((string)u.strResist, "非表示") == 0)
                {
                    if (Strings.Len((string)u.strResist) > 5)
                    {
                        if (n > 0)
                        {
                            upic.Print();
                            // ADD START 240a
                            if (GUI.NewGUIMode)
                            {
                                upic.CurrentX = 5;
                            }
                            // ADD  END  240a
                        }

                        n = 2;
                    }

                    if (n == 0 && GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("耐性   ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(GeneralLib.RightPaddedString((string)u.strResist, 12));
                    n = (n + 1);
                    if (n > 1)
                    {
                        upic.Print();
                        // ADD START 240a
                        if (GUI.NewGUIMode)
                        {
                            upic.CurrentX = 5;
                        }
                        // ADD  END  240a
                        n = 0;
                    }
                }

                // 弱点
                if (Strings.Len((string)u.strWeakness) > 0 && Strings.InStr((string)u.strWeakness, "非表示") == 0)
                {
                    if (Strings.Len((string)u.strWeakness) > 5)
                    {
                        if (n > 0)
                        {
                            upic.Print();
                            // ADD START 240a
                            if (GUI.NewGUIMode)
                            {
                                upic.CurrentX = 5;
                            }
                            // ADD  END  240a
                        }

                        n = 2;
                    }

                    if (n == 0 && GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("弱点   ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(GeneralLib.RightPaddedString((string)u.strWeakness, 12));
                    n = (n + 1);
                    if (n > 1)
                    {
                        upic.Print();
                        // ADD START 240a
                        if (GUI.NewGUIMode)
                        {
                            upic.CurrentX = 5;
                        }
                        // ADD  END  240a
                        n = 0;
                    }
                }

                // 有効
                if (Strings.Len((string)u.strEffective) > 0 && Strings.InStr((string)u.strEffective, "非表示") == 0)
                {
                    if (Strings.Len((string)u.strEffective) > 5)
                    {
                        if (n > 0)
                        {
                            upic.Print();
                            // ADD START 240a
                            if (GUI.NewGUIMode)
                            {
                                upic.CurrentX = 5;
                            }
                            // ADD  END  240a
                        }

                        n = 2;
                    }

                    if (n == 0 && GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("有効   ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(GeneralLib.RightPaddedString((string)u.strEffective, 12));
                    n = (n + 1);
                    if (n > 1)
                    {
                        upic.Print();
                        // ADD START 240a
                        if (GUI.NewGUIMode)
                        {
                            upic.CurrentX = 5;
                        }
                        // ADD  END  240a
                        n = 0;
                    }
                }

                // 特殊効果無効化
                if (Strings.Len((string)u.strSpecialEffectImmune) > 0 && Strings.InStr((string)u.strSpecialEffectImmune, "非表示") == 0)
                {
                    if (Strings.Len((string)u.strSpecialEffectImmune) > 5)
                    {
                        if (n > 0)
                        {
                            upic.Print();
                            // ADD START 240a
                            if (GUI.NewGUIMode)
                            {
                                upic.CurrentX = 5;
                            }
                            // ADD  END  240a
                        }

                        n = 2;
                    }

                    if (n == 0 && GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("特無効 ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(GeneralLib.RightPaddedString((string)u.strSpecialEffectImmune, 12));
                    n = (n + 1);
                    if (n > 1)
                    {
                        upic.Print();
                        // ADD START 240a
                        if (GUI.NewGUIMode)
                        {
                            upic.CurrentX = 5;
                        }
                        // ADD  END  240a
                        n = 0;
                    }
                }

                // 必要に応じて改行
                if (n > 0)
                {
                    upic.Print();
                    // ADD START 240a
                    if (GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // ADD  END  240a
                }

                n = 0;

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // 武器・防具クラス
                flist = new string[1];
                if (Expression.IsOptionDefined("アイテム交換"))
                {
                    if (u.IsFeatureAvailable("武器クラス") || u.IsFeatureAvailable("防具クラス"))
                    {
                        if (GUI.NewGUIMode)
                        {
                            upic.CurrentX = 5;
                        }
                        upic.Print(GeneralLib.RightPaddedString("武器・防具クラス", 19));
                        Array.Resize(flist, 2);
                        flist[1] = "武器・防具クラス";
                        n = (n + 1);
                    }
                }

                // 特殊能力一覧を表示する前に必要気力判定のためメインパイロットの気力を参照
                if (u.CountPilot() > 0)
                {
                    pmorale = u.MainPilot().Morale;
                }
                else
                {
                    pmorale = 150;
                }

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // 特殊能力一覧
                var loopTo10 = u.CountAllFeature();
                for (i = (u.AdditionalFeaturesNum + 1); i <= loopTo10; i++)
                {
                    fname = u.AllFeatureName(i);

                    // ユニットステータスコマンド時は通常は非表示のパーツ合体、
                    // ノーマルモード、換装も表示
                    if (string.IsNullOrEmpty(fname))
                    {
                        if (string.IsNullOrEmpty(Map.MapFileName))
                        {
                            switch (u.AllFeature(i) ?? "")
                            {
                                case "パーツ合体":
                                case "ノーマルモード":
                                    {
                                        string localAllFeature() { object argIndex1 = i; var ret = u.AllFeature(argIndex1); return ret; }

                                        string localRightPaddedString17() { string argbuf = hs5fe6f1588051411f97aaada3678aab3c(); var ret = GeneralLib.RightPaddedString(argbuf, 19); return ret; }

                                        upic.Print(localRightPaddedString17());
                                        n = (n + 1);
                                        if (n > 1)
                                        {
                                            upic.Print();
                                            // ADD START 240a
                                            if (GUI.NewGUIMode)
                                            {
                                                upic.CurrentX = 5;
                                            }
                                            // ADD  END  240a
                                            n = 0;
                                        }

                                        break;
                                    }

                                case "換装":
                                    {
                                        fname = "換装";

                                        // エリアスで換装の名称が変更されている？
                                        {
                                            var withBlock3 = SRC.ALDList;
                                            var loopTo11 = withBlock3.Count();
                                            for (j = 1; j <= loopTo11; j++)
                                            {
                                                {
                                                    var withBlock4 = withBlock3.Item(j);
                                                    if (withBlock4.get_AliasType(1) == "換装")
                                                    {
                                                        fname = withBlock4.Name;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        upic.Print(GeneralLib.RightPaddedString(fname, 19));
                                        n = (n + 1);
                                        if (n > 1)
                                        {
                                            upic.Print();
                                            // ADD START 240a
                                            if (GUI.NewGUIMode)
                                            {
                                                upic.CurrentX = 5;
                                            }
                                            // ADD  END  240a
                                            n = 0;
                                        }

                                        break;
                                    }
                            }
                        }

                        goto NextFeature;
                    }

                    // 既に表示しているかを判定
                    var loopTo12 = Information.UBound(flist);
                    for (j = 1; j <= loopTo12; j++)
                    {
                        if ((fname ?? "") == (flist[j] ?? ""))
                        {
                            goto NextFeature;
                        }
                    }

                    Array.Resize(flist, Information.UBound(flist) + 1 + 1);
                    flist[Information.UBound(flist)] = fname;

                    // 使用可否によって表示色を変える
                    fdata = u.AllFeatureData(i);
                    switch (u.AllFeature(i) ?? "")
                    {
                        case "合体":
                            {
                                bool localIsDefined1() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                                if (!localIsDefined1())
                                {
                                    goto NextFeature;
                                }

                                Unit localItem1() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UList.Item(argIndex1); return ret; }

                                if (localItem1().IsConditionSatisfied("行動不能"))
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }

                        case "分離":
                            {
                                k = 0;
                                var loopTo13 = GeneralLib.LLength(fdata);
                                for (j = 2; j <= loopTo13; j++)
                                {
                                    bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(fdata, j); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                                    if (!localIsDefined2())
                                    {
                                        goto NextFeature;
                                    }

                                    Unit localItem2() { object argIndex1 = GeneralLib.LIndex(fdata, j); var ret = SRC.UList.Item(argIndex1); return ret; }

                                    {
                                        var withBlock5 = localItem2().Data;
                                        if (withBlock5.IsFeatureAvailable("召喚ユニット"))
                                        {
                                            k = (k + Math.Abs(withBlock5.PilotNum));
                                        }
                                    }
                                }

                                if (u.CountPilot() < k)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }

                        case "ハイパーモード":
                            {
                                double localFeatureLevel() { object argIndex1 = i; var ret = u.FeatureLevel(argIndex1); return ret; }

                                double localFeatureLevel1() { object argIndex1 = i; var ret = u.FeatureLevel(argIndex1); return ret; }
                                // MOD  END  240a
                                if (pmorale < (10d * localFeatureLevel1()) + 100 && u.HP > u.MaxHP / 4)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }
                                else if (u.IsConditionSatisfied("ノーマルモード付加"))
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }

                        case "修理装置":
                        case "補給装置":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                                {
                                    if (u.EN < Conversions.Toint(GeneralLib.LIndex(fdata, 2)))
                                    {
                                        // MOD START 240a
                                        // upic.ForeColor = rgb(150, 0, 0)
                                        upic.SetColor(StatusFontColorAbilityDisable);
                                    }
                                }

                                break;
                            }

                        case "テレポート":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                                {
                                    if (u.EN < Conversions.Toint(GeneralLib.LIndex(fdata, 2)))
                                    {
                                        // MOD START 240a
                                        // upic.ForeColor = rgb(150, 0, 0)
                                        upic.SetColor(StatusFontColorAbilityDisable);
                                    }
                                }
                                else if (u.EN < 40)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }

                        case "分身":
                            {
                                if (pmorale < 130)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }

                        case "超回避":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                                {
                                    ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 2));
                                }
                                else
                                {
                                    ecost = 0;
                                }

                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (u.EN < ecost || pmorale < nmorale)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }

                        case "緊急テレポート":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                {
                                    ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
                                }
                                else
                                {
                                    ecost = 0;
                                }

                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (u.EN < ecost || pmorale < nmorale)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }

                        case "エネルギーシールド":
                            {
                                if (u.EN < 5)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }

                        case "バリア":
                        case "バリアシールド":
                        case "プロテクション":
                        case "アクティブプロテクション":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                {
                                    ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
                                }
                                else
                                {
                                    ecost = 10;
                                }

                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (u.EN < ecost || pmorale < nmorale || u.IsConditionSatisfied("バリア無効化") && Strings.InStr(fdata, "バリア無効化無効") == 0)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }
                                // MOD  END  240a
                                else if (Strings.InStr(fdata, "能力必要") > 0)
                                {
                                    var loopTo14 = GeneralLib.LLength(fdata);
                                    for (j = 5; j <= loopTo14; j++)
                                    {
                                        opt = GeneralLib.LIndex(fdata, j);
                                        if (Strings.InStr(opt, "*") > 0)
                                        {
                                            opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
                                        }

                                        switch (opt ?? "")
                                        {
                                            case "相殺":
                                            case "中和":
                                            case "近接無効":
                                            case "手動":
                                            case "能力必要":
                                                {
                                                    break;
                                                }
                                            // スキップ
                                            case "同調率":
                                                {
                                                    if (u.SyncLevel() == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }

                                            case "霊力":
                                                {
                                                    if (u.PlanaLevel() == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }

                                            default:
                                                {
                                                    if (u.SkillLevel(opt) == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }
                                        }
                                    }
                                }

                                break;
                            }

                        case "フィールド":
                        case "アクティブフィールド":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                {
                                    ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
                                }
                                else
                                {
                                    ecost = 0;
                                }

                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (u.EN < ecost || pmorale < nmorale || u.IsConditionSatisfied("バリア無効化") && Strings.InStr(fdata, "バリア無効化無効") == 0)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }
                                // MOD  END  240a
                                else if (Strings.InStr(fdata, "能力必要") > 0)
                                {
                                    var loopTo15 = GeneralLib.LLength(fdata);
                                    for (j = 5; j <= loopTo15; j++)
                                    {
                                        opt = GeneralLib.LIndex(fdata, j);
                                        if (Strings.InStr(opt, "*") > 0)
                                        {
                                            opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
                                        }

                                        switch (opt ?? "")
                                        {
                                            case "相殺":
                                            case "中和":
                                            case "近接無効":
                                            case "手動":
                                            case "能力必要":
                                                {
                                                    break;
                                                }
                                            // スキップ
                                            case "同調率":
                                                {
                                                    if (u.SyncLevel() == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }

                                            case "霊力":
                                                {
                                                    if (u.PlanaLevel() == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }

                                            default:
                                                {
                                                    if (u.SkillLevel(opt) == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }
                                        }
                                    }
                                }

                                break;
                            }

                        case "広域バリア":
                        case "広域フィールド":
                        case "広域プロテクション":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                                {
                                    ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
                                }
                                else if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                                {
                                    ecost = (20 * Conversions.Toint(GeneralLib.LIndex(fdata, 2)));
                                }
                                else
                                {
                                    ecost = 0;
                                }

                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (u.EN < ecost || pmorale < nmorale || u.IsConditionSatisfied("バリア無効化") && Strings.InStr(fdata, "バリア無効化無効") == 0)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                fname = fname + "(範囲" + GeneralLib.LIndex(fdata, 2) + "マス)";
                                break;
                            }

                        case "アーマー":
                        case "レジスト":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (pmorale < nmorale)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }
                                // MOD  END  240a
                                else if (Strings.InStr(fdata, "能力必要") > 0)
                                {
                                    var loopTo16 = GeneralLib.LLength(fdata);
                                    for (j = 4; j <= loopTo16; j++)
                                    {
                                        opt = GeneralLib.LIndex(fdata, j);
                                        if (Strings.InStr(opt, "*") > 0)
                                        {
                                            opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
                                        }

                                        switch (opt ?? "")
                                        {
                                            case "同調率":
                                                {
                                                    if (u.SyncLevel() == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }

                                            case "霊力":
                                                {
                                                    if (u.PlanaLevel() == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }

                                            default:
                                                {
                                                    if (u.SkillLevel(opt) == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }
                                        }
                                    }
                                }

                                break;
                            }

                        case "攻撃回避":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (pmorale < nmorale)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }

                        case "反射":
                        case "阻止":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                                {
                                    ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
                                }
                                else
                                {
                                    ecost = 0;
                                }

                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (u.EN < ecost || pmorale < nmorale)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }
                                // MOD  END  240a
                                else if (Strings.InStr(fdata, "能力必要") > 0)
                                {
                                    var loopTo17 = GeneralLib.LLength(fdata);
                                    for (j = 6; j <= loopTo17; j++)
                                    {
                                        opt = GeneralLib.LIndex(fdata, j);
                                        if (Strings.InStr(opt, "*") > 0)
                                        {
                                            opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
                                        }

                                        switch (opt ?? "")
                                        {
                                            case "相殺":
                                            case "中和":
                                            case "近接無効":
                                            case "手動":
                                            case "能力必要":
                                                {
                                                    break;
                                                }
                                            // スキップ
                                            case "同調率":
                                                {
                                                    if (u.SyncLevel() == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }

                                            case "霊力":
                                                {
                                                    if (u.PlanaLevel() == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }

                                            default:
                                                {
                                                    if (u.SkillLevel(opt) == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }
                                        }
                                    }
                                }

                                break;
                            }

                        case "広域阻止":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
                                {
                                    ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
                                }
                                else
                                {
                                    ecost = 0;
                                }

                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 6));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (u.EN < ecost || pmorale < nmorale)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                fname = fname + "(範囲" + GeneralLib.LIndex(fdata, 2) + "マス)";
                                break;
                            }

                        case "当て身技":
                        case "自動反撃":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
                                {
                                    ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
                                }
                                else
                                {
                                    ecost = 0;
                                }

                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 6));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (u.EN < ecost || pmorale < nmorale)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }
                                // MOD  END  240a
                                else if (Strings.InStr(fdata, "能力必要") > 0)
                                {
                                    var loopTo18 = GeneralLib.LLength(fdata);
                                    for (j = 7; j <= loopTo18; j++)
                                    {
                                        opt = GeneralLib.LIndex(fdata, j);
                                        if (Strings.InStr(opt, "*") > 0)
                                        {
                                            opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
                                        }

                                        switch (opt ?? "")
                                        {
                                            case "相殺":
                                            case "中和":
                                            case "近接無効":
                                            case "手動":
                                            case "能力必要":
                                                {
                                                    break;
                                                }
                                            // スキップ
                                            case "同調率":
                                                {
                                                    if (u.SyncLevel() == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }

                                            case "霊力":
                                                {
                                                    if (u.PlanaLevel() == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }

                                            default:
                                                {
                                                    if (u.SkillLevel(opt) == 0d)
                                                    {
                                                        goto NextFeature;
                                                    }

                                                    break;
                                                }
                                        }
                                    }
                                }

                                break;
                            }

                        case "ブースト":
                            {
                                if (pmorale >= 130)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = vbBlue
                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
                                    // MOD  END  240a
                                }

                                break;
                            }

                        case "盾":
                            {
                                if (u.ConditionLevel("盾ダメージ") >= u.AllFeatureLevel("盾"))
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                fname = fname + "(" + SrcFormatter.Format(GeneralLib.MaxLng((u.AllFeatureLevel("盾") - u.ConditionLevel("盾ダメージ")), 0)) + "/" + SrcFormatter.Format(u.AllFeatureLevel("盾")) + ")";
                                break;
                            }

                        case "ＨＰ回復":
                        case "ＥＮ回復":
                            {
                                // MOD START MARGE
                                // If .IsConditionSatisfied("回復不能") Then
                                if (u.IsConditionSatisfied("回復不能") || u.IsSpecialPowerInEffect("回復不能"))
                                {
                                    // MOD END MARGE
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }

                        case "格闘強化":
                        case "射撃強化":
                        case "命中強化":
                        case "回避強化":
                        case "技量強化":
                        case "反応強化":
                        case "ＨＰ強化":
                        case "ＥＮ強化":
                        case "装甲強化":
                        case "運動性強化":
                        case "移動力強化":
                        case "ＨＰ割合強化":
                        case "ＥＮ割合強化":
                        case "装甲割合強化":
                        case "運動性割合強化":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                                {
                                    int localStrToLng2() { string argexpr = GeneralLib.LIndex(fdata, 2); var ret = GeneralLib.StrToLng(argexpr); return ret; }

                                    if (pmorale >= localStrToLng2())
                                    {
                                        // MOD START 240a
                                        // upic.ForeColor = vbBlue
                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
                                        // MOD  END  240a
                                    }
                                }

                                break;
                            }

                        case "ＺＯＣ":
                            {
                                if (GeneralLib.LLength(fdata) < 2)
                                {
                                    j = 1;
                                }
                                else
                                {
                                    j = Conversions.Toint(GeneralLib.LIndex(fdata, 2));
                                }

                                if (j >= 1)
                                {
                                    GeneralLib.ReplaceString(fdata, Constants.vbTab, " ");
                                    if (Strings.InStr(fdata, " 直線") > 0 || Strings.InStr(fdata, " 垂直") > 0 && Strings.InStr(fdata, " 水平") > 0)
                                    {
                                        buf = "直線";
                                    }
                                    else if (Strings.InStr(fdata, " 垂直") > 0)
                                    {
                                        buf = "上下";
                                    }
                                    else if (Strings.InStr(fdata, " 水平") > 0)
                                    {
                                        buf = "左右";
                                    }
                                    else
                                    {
                                        buf = "範囲";
                                    }

                                    fname = fname + "(" + buf + SrcFormatter.Format(j) + "マス)";
                                }

                                break;
                            }

                        case "広域ＺＯＣ無効化":
                            {
                                fname = fname + "(範囲" + GeneralLib.LIndex(fdata, 2) + "マス)";
                                break;
                            }

                        case "追加攻撃":
                            {
                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
                                {
                                    ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
                                }
                                else
                                {
                                    ecost = 0;
                                }

                                if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
                                {
                                    nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 6));
                                }
                                else
                                {
                                    nmorale = 0;
                                }

                                if (u.EN < ecost || pmorale < nmorale)
                                {
                                    // MOD START 240a
                                    // upic.ForeColor = rgb(150, 0, 0)
                                    upic.SetColor(StatusFontColorAbilityDisable);
                                }

                                break;
                            }
                    }

                    // 必要条件を満たさない特殊能力は赤色で表示
                    bool localIsFeatureActivated() { object argIndex1 = i; var ret = u.IsFeatureActivated(argIndex1); return ret; }

                    if (!localIsFeatureActivated())
                    {
                        // MOD START 240a
                        // upic.ForeColor = rgb(150, 0, 0)
                        upic.SetColor(StatusFontColorAbilityDisable);
                    }

                    // 特殊能力名を表示
                    if (LenB(Strings.StrConv(fname, vbFromUnicode)) > 19)
                    {
                        if (n > 0)
                        {
                            upic.Print();
                            // ADD START 240a
                            if (GUI.NewGUIMode)
                            {
                                upic.CurrentX = 5;
                            }
                            // ADD  END  240a
                        }
                        upic.Print(fname);
                        n = 2;
                    }
                    else
                    {
                        upic.Print(GeneralLib.RightPaddedString(fname, 19));
                        n = (n + 1);
                    }

                    // 必要に応じて改行
                    if (n > 1)
                    {
                        upic.Print();
                        // ADD START 240a
                        if (GUI.NewGUIMode)
                        {
                            upic.CurrentX = 5;
                        }
                        // ADD  END  240a
                        n = 0;
                    }

                    // 表示色を戻しておく
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                NextFeature:
                    ;
                }

                if (n > 0)
                {
                    upic.Print();
                }

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // アイテム一覧
                if (u.CountItem() > 0)
                {
                    j = 0;
                    var loopTo19 = u.CountItem();
                    for (i = 1; i <= loopTo19; i++)
                    {
                        {
                            var withBlock6 = u.Item(i);
                            // 表示指定を持つアイテムのみ表示する
                            if (!withBlock6.IsFeatureAvailable("表示"))
                            {
                                goto NextItem;
                            }

                            // アイテム名を表示
                            if (Strings.Len(withBlock6.Nickname()) > 9)
                            {
                                if (j == 1)
                                {
                                    upic.Print();
                                    // ADD START 240a
                                    if (GUI.NewGUIMode)
                                    {
                                        upic.CurrentX = 5;
                                    }
                                    // ADD  END  240a
                                }
                                upic.Print(withBlock6.Nickname());
                                j = 2;
                            }
                            else
                            {
                                upic.Print(GeneralLib.RightPaddedString(withBlock6.Nickname(), 19));
                                j = (j + 1);
                            }

                            if (j == 2)
                            {
                                upic.Print();
                                // ADD START 240a
                                if (GUI.NewGUIMode)
                                {
                                    upic.CurrentX = 5;
                                }
                                // ADD  END  240a
                                j = 0;
                            }
                        }

                    NextItem:
                        ;
                    }

                    if (j > 0)
                    {
                        upic.Print();
                        // ADD START 240a
                        if (GUI.NewGUIMode)
                        {
                            upic.CurrentX = 5;
                        }
                        // ADD  END  240a
                    }
                }

                // ターゲット選択時の攻撃結果予想表示

                // 攻撃時にのみ表示
                if ((Commands.CommandState == "ターゲット選択" || Commands.CommandState == "移動後ターゲット選択") && (Commands.SelectedCommand == "攻撃" || Commands.SelectedCommand == "マップ攻撃") && Commands.SelectedUnit is object && Commands.SelectedWeapon > 0 && SRC.Stage != "プロローグ" && SRC.Stage != "エピローグ")
                {
                }
                // 攻撃時と判定
                else
                {
                    goto SkipAttackExpResult;
                }

                // 相手が敵の場合にのみ表示
                if (u.Party != "敵" && u.Party != "中立" && !u.IsConditionSatisfied("暴走") && !u.IsConditionSatisfied("魅了") && !u.IsConditionSatisfied("憑依") && !u.IsConditionSatisfied("混乱") && !u.IsConditionSatisfied("睡眠"))
                {
                    goto SkipAttackExpResult;
                }

                upic.Print();

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // 攻撃手段
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 150)
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print("攻撃     ");
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 0)
                upic.SetColor(StatusFontColorNormalString);
                upic.Print(Commands.SelectedUnit.WeaponNickname(Commands.SelectedWeapon));
                // サポートアタックを得られる？
                if (!Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, "合") && !Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, "Ｍ") && Commands.UseSupportAttack)
                {
                    if (Commands.SelectedUnit.LookForSupportAttack(u) is object)
                    {
                        upic.Print(" [援]");
                    }
                    else
                    {
                        upic.Print();
                    }
                }
                else
                {
                    upic.Print();
                }

                // 反撃を受ける？
                if (u.MaxAction() == 0 || Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, "Ｍ") || Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, "間"))
                {
                    w = 0;
                }
                else
                {
                    w = COM.SelectWeapon(u, Commands.SelectedUnit, "反撃", max_prob: 0, max_dmg: 0);
                }

                // 敵の防御行動を設定
                // UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                def_mode = Conversions.ToString(COM.SelectDefense(Commands.SelectedUnit, Commands.SelectedWeapon, u, w));
                if (!string.IsNullOrEmpty(def_mode))
                {
                    w = 0;
                }

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // 予測ダメージ
                if (!Expression.IsOptionDefined("予測ダメージ非表示"))
                {
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("ダメージ ");
                    dmg = Commands.SelectedUnit.Damage(Commands.SelectedWeapon, u, true);
                    if (def_mode == "防御")
                    {
                        dmg = dmg / 2;
                    }

                    if (dmg >= u.HP && !u.IsConditionSatisfied("データ不明"))
                    {
                        upic.ForeColor = ColorTranslator.FromOle(Information.RGB(190, 0, 0));
                    }
                    else
                    {
                        // MOD START 240a
                        // upic.ForeColor = rgb(0, 0, 0)
                        upic.SetColor(StatusFontColorNormalString);
                    }
                    upic.Print(SrcFormatter.Format(dmg));
                }

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // 予測命中率
                if (!Expression.IsOptionDefined("予測命中率非表示"))
                {
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("命中率   ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    prob = Commands.SelectedUnit.HitProbability(Commands.SelectedWeapon, u, true);
                    if (def_mode == "回避")
                    {
                        prob = (prob / 2);
                    }

                    cprob = Commands.SelectedUnit.CriticalProbability(Commands.SelectedWeapon, u, def_mode);
                    upic.Print(GeneralLib.MinLng(prob, 100) + "％（" + cprob + "％）");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                }

                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                if (w > 0)
                {
                    // 反撃手段
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("反撃     ");
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(u.WeaponNickname(w));
                    // サポートガードを受けられる？
                    if (u.LookForSupportGuard(Commands.SelectedUnit, Commands.SelectedWeapon) is object)
                    {
                        upic.Print(" [援]");
                    }
                    else
                    {
                        upic.Print();
                    }

                    // ADD START 240a
                    if (GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // ADD  END  240a
                    // 予測ダメージ
                    if (!Expression.IsOptionDefined("予測ダメージ非表示"))
                    {
                        // MOD START 240a
                        // upic.ForeColor = rgb(0, 0, 150)
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print("ダメージ ");
                        dmg = u.Damage(w, Commands.SelectedUnit, true);
                        if (dmg >= Commands.SelectedUnit.HP)
                        {
                            upic.ForeColor = ColorTranslator.FromOle(Information.RGB(190, 0, 0));
                        }
                        else
                        {
                            // MOD START 240a
                            // upic.ForeColor = rgb(0, 0, 0)
                            upic.SetColor(StatusFontColorNormalString);
                        }
                        upic.Print(SrcFormatter.Format(dmg));
                    }

                    // ADD START 240a
                    if (GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // ADD  END  240a
                    // 予測命中率
                    if (!Expression.IsOptionDefined("予測命中率非表示"))
                    {
                        // MOD START 240a
                        // upic.ForeColor = rgb(0, 0, 150)
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print("命中率   ");
                        // MOD START 240a
                        // upic.ForeColor = rgb(0, 0, 0)
                        upic.SetColor(StatusFontColorNormalString);
                        prob = u.HitProbability(w, Commands.SelectedUnit, true);
                        cprob = u.CriticalProbability(w, Commands.SelectedUnit);
                        upic.Print(SrcFormatter.Format(GeneralLib.MinLng(prob, 100)) + "％（" + cprob + "％）");
                    }
                }
                else
                {
                    // 相手は反撃できない
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 150)
                    upic.SetColor(StatusFontColorAbilityName);
                    if (!string.IsNullOrEmpty(def_mode))
                    {
                        upic.Print(def_mode);
                    }
                    else
                    {
                        upic.Print("反撃不能");
                    }
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                    // サポートガードを受けられる？
                    if (u.LookForSupportGuard(Commands.SelectedUnit, Commands.SelectedWeapon) is object)
                    {
                        upic.Print(" [援]");
                    }
                    else
                    {
                        upic.Print();
                    }
                }

            SkipAttackExpResult:
                ;


                // ADD START 240a
                if (GUI.NewGUIMode)
                {
                    upic.CurrentX = 5;
                }
                // ADD  END  240a
                // 武器一覧
                upic.CurrentY = upic.CurrentY + 8;
                upic.Print(Strings.Space(25));
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 150)
                upic.SetColor(StatusFontColorAbilityName);
                upic.Print("攻撃 射程");
                // MOD START 240a
                // upic.ForeColor = rgb(0, 0, 0)
                upic.SetColor(StatusFontColorNormalString);

                warray = new int[(u.CountWeapon() + 1)];
                wpower = new int[(u.CountWeapon() + 1)];
                var loopTo20 = u.CountWeapon();
                for (i = 1; i <= loopTo20; i++)
                {
                    wpower[i] = u.WeaponPower(i, "");
                }

                // 攻撃力でソート
                var loopTo21 = u.CountWeapon();
                for (i = 1; i <= loopTo21; i++)
                {
                    var loopTo22 = (i - 1);
                    for (j = 1; j <= loopTo22; j++)
                    {
                        if (wpower[i] > wpower[warray[i - j]])
                        {
                            break;
                        }
                        else if (wpower[i] == wpower[warray[i - j]])
                        {
                            if (u.Weapon(i).ENConsumption > 0)
                            {
                                if (u.Weapon(i).ENConsumption >= u.Weapon(warray[i - j]).ENConsumption)
                                {
                                    break;
                                }
                            }
                            else if (u.Weapon(i).Bullet > 0)
                            {
                                if (u.Weapon(i).Bullet <= u.Weapon(warray[i - j]).Bullet)
                                {
                                    break;
                                }
                            }
                            else if (u.Weapon((i - j)).ENConsumption == 0 && u.Weapon(warray[i - j]).Bullet == 0)
                            {
                                break;
                            }
                        }
                    }

                    var loopTo23 = (j - 1);
                    for (k = 1; k <= loopTo23; k++)
                        warray[i - k + 1] = warray[i - k];
                    warray[i - j + 1] = i;
                }

                // 個々の武器を表示
                var loopTo24 = u.CountWeapon();
                for (i = 1; i <= loopTo24; i++)
                {
                    if (upic.CurrentY > 420)
                    {
                        break;
                    }

                    w = warray[i];
                    if (!u.IsWeaponAvailable(w, "ステータス"))
                    {
                        // 習得していない技は表示しない
                        if (!u.IsWeaponMastered(w))
                        {
                            goto NextWeapon;
                        }
                        // Disableコマンドで使用不可になった武器も同様
                        if (u.IsDisabled(u.Weapon(w).Name))
                        {
                            goto NextWeapon;
                        }
                        // フォーメーションを満たしていない合体技も
                        if (u.IsWeaponClassifiedAs(w, "合"))
                        {
                            if (!u.IsCombinationAttackAvailable(w, true))
                            {
                                goto NextWeapon;
                            }
                        }
                        // MOD START 240a
                        // upic.ForeColor = rgb(150, 0, 0)
                        upic.SetColor(StatusFontColorAbilityDisable);
                    }

                    // 武器の表示
                    if (u.WeaponPower(w, "") < 10000)
                    {
                        buf = GeneralLib.RightPaddedString(SrcFormatter.Format(u.WeaponNickname(w)), 25);
                        string localLeftPaddedString19() { string argtarea = ""; string argbuf = SrcFormatter.Format(u.WeaponPower(w, argtarea)); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

                        buf = buf + localLeftPaddedString19();
                    }
                    else
                    {
                        buf = GeneralLib.RightPaddedString(SrcFormatter.Format(u.WeaponNickname(w)), 24);
                        string localLeftPaddedString20() { string argtarea = ""; string argbuf = SrcFormatter.Format(u.WeaponPower(w, argtarea)); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                        buf = buf + localLeftPaddedString20();
                    }

                    // 武器が特殊効果を持つ場合は略称で表記
                    if (u.WeaponMaxRange(w) > 1)
                    {
                        string localLeftPaddedString21() { string argbuf = SrcFormatter.Format(u.Weapon(w).MinRange) + "-" + SrcFormatter.Format(u.WeaponMaxRange(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 34 - LenB(Strings.StrConv(buf, vbFromUnicode))); return ret; }

                        buf = buf + localLeftPaddedString21();
                        // 移動後攻撃可能
                        if (u.IsWeaponClassifiedAs(w, "Ｐ"))
                        {
                            buf = buf + "P";
                        }
                    }
                    else
                    {
                        buf = buf + GeneralLib.LeftPaddedString("1", 34 - LenB(Strings.StrConv(buf, vbFromUnicode)));
                        // ADD START MARGE
                        // 移動後攻撃不可
                        if (u.IsWeaponClassifiedAs(w, "Ｑ"))
                        {
                            buf = buf + "Q";
                        }
                        // ADD END MARGE
                    }
                    // マップ攻撃
                    if (u.IsWeaponClassifiedAs(w, "Ｍ"))
                    {
                        buf = buf + "M";
                    }
                    // 特殊効果
                    wclass = u.Weapon(w).Class;
                    var loopTo25 = u.CountWeaponEffect(w);
                    for (j = 1; j <= loopTo25; j++)
                        buf = buf + "+";
                    // ADD START 240a
                    if (GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // ADD  END  240a
                    upic.Print(buf);
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                NextWeapon:
                    ;
                }

                // アビリティ一覧
                var loopTo26 = u.CountAbility();
                for (i = 1; i <= loopTo26; i++)
                {
                    if (upic.CurrentY > 420)
                    {
                        break;
                    }

                    if (!u.IsAbilityAvailable(i, "ステータス"))
                    {
                        // 習得していない技は表示しない
                        if (!u.IsAbilityMastered(i))
                        {
                            goto NextAbility;
                        }
                        // Disableコマンドで使用不可になった武器も同様
                        if (u.IsDisabled(u.Ability(i).Name))
                        {
                            goto NextAbility;
                        }
                        // フォーメーションを満たしていない合体技も
                        if (u.IsAbilityClassifiedAs(i, "合"))
                        {
                            if (!u.IsCombinationAbilityAvailable(i, true))
                            {
                                goto NextAbility;
                            }
                        }
                        // MOD START 240a
                        // upic.ForeColor = rgb(150, 0, 0)
                        upic.SetColor(StatusFontColorAbilityDisable);
                    }

                    // ADD START 240a
                    if (GUI.NewGUIMode)
                    {
                        upic.CurrentX = 5;
                    }
                    // ADD  END  240a
                    // アビリティの表示
                    string localRightPaddedString18() { string argbuf = SrcFormatter.Format(u.AbilityNickname(i)); var ret = GeneralLib.RightPaddedString(argbuf, 29); return ret; }

                    upic.Print(localRightPaddedString18());
                    if (u.AbilityMaxRange(i) > 1)
                    {
                        string localLeftPaddedString22() { string argbuf = SrcFormatter.Format(u.AbilityMinRange(i)) + "-" + SrcFormatter.Format(u.AbilityMaxRange(i)); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

                        upic.Print(localLeftPaddedString22());
                        if (u.IsAbilityClassifiedAs(i, "Ｐ"))
                        {
                            upic.Print("P");
                        }

                        if (u.IsAbilityClassifiedAs(i, "Ｍ"))
                        {
                            upic.Print("M");
                        }
                        upic.Print();
                    }
                    else if (u.AbilityMaxRange(i) == 1)
                    {
                        upic.Print("    1");
                        // ADD START MARGE
                        if (u.IsAbilityClassifiedAs(i, "Ｑ"))
                        {
                            upic.Print("Q");
                        }
                        // ADD END MARGE
                        if (u.IsAbilityClassifiedAs(i, "Ｍ"))
                        {
                            upic.Print("M");
                        }
                        upic.Print();
                    }
                    else
                    {
                        upic.Print("    -");
                    }
                    // MOD START 240a
                    // upic.ForeColor = rgb(0, 0, 0)
                    upic.SetColor(StatusFontColorNormalString);
                NextAbility:
                    ;
                }


            UpdateStatusWindow:
                ;

                // MOD START 240a
                // If MainWidth = 15 Then
                if (!GUI.NewGUIMode)
                {
                    // MOD  END
                    // ステータスウィンドウをリフレッシュ
                    GUI.MainForm.picFace.Refresh();
                    ppic.Refresh();
                    upic.Refresh();
                }
                else
                {
                    if (GUI.MouseX < GUI.MainPWidth / 2)
                    {
                        // MOD START 240a
                        // upic.Move MainPWidth - 230 - 5, 10
                        // 画面左側にカーソルがある場合
                        upic.SetBounds(SrcFormatter.TwipsToPixelsX(GUI.MainPWidth - 240), SrcFormatter.TwipsToPixelsY(10d), 0, 0, BoundsSpecified.X || BoundsSpecified.Y);
                    }
                    // MOD  END
                    else
                    {
                        upic.SetBounds(SrcFormatter.TwipsToPixelsX(5d), SrcFormatter.TwipsToPixelsY(10d), 0, 0, BoundsSpecified.X || BoundsSpecified.Y);
                    }

                    if (upic.Visible)
                    {
                        upic.Refresh();
                    }
                    else
                    {
                        upic.Visible = true;
                    }
                }
            }
            //    return;
            //ErrorHandler:
            //    ;
            //    GUI.ErrorMessage("パイロット用画像ファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "画像ファイルが壊れていないか確認して下さい。");
        }

        private void DrawPilotLabel(Unit u, Graphics pilotG, Graphics unitG, float headingMargin)
        {
            // XXX ニックネームのフォントサイズ分マージンを入れる
            pilotG.DrawString(
@$"{Expression.Term("レベル", u)}
{Expression.Term("気力", u)}", StatusFont, StatusAbilityNameBrush, 0, headingMargin);

            unitG.DrawString(
@$"{Expression.Term("格闘", u, 4) + "               " + Expression.Term("射撃", u)}
{Expression.Term("命中", u, 4) + "               " + Expression.Term("回避", u)}
{Expression.Term("技量", u, 4) + "               " + Expression.Term("反応", u)}", StatusFont, StatusAbilityNameBrush, 0, 0);
        }

        public void DisplayPilotStatus(Pilot p)
        {
            DisplayedUnit = p.Unit;

            // メインパイロット、サブパイロット、サポートパイロット、追加サポート
            // の場合がある
            DisplayUnitStatus(DisplayedUnit, p);
        }

        public void InstantUnitStatusDisplay(int X, int Y)
        {
            // 指定された座標にいるユニットを収得
            Unit u = Map.UnitAtPoint(X, Y);

            // 発進コマンドの場合は母艦ではなく発進するユニットを使う
            if (Commands.CommandState == "ターゲット選択" && Commands.SelectedCommand == "発進")
            {
                if (ReferenceEquals(u, Commands.SelectedUnit))
                {
                    u = Commands.SelectedTarget;
                    if (u is null)
                    {
                        return;
                    }
                }
            }

            if (DisplayedUnit is null)
            {
            }
            // ステータスウィンドウに何も表示されていなければ無条件で表示
            // 同じユニットが表示されていればスキップ
            else if (ReferenceEquals(u, DisplayedUnit))
            {
                return;
            }

            DisplayUnitStatus(u);
        }

        public void ClearUnitStatus()
        {
            // XXX ステータスの枠自体を消すならここで
            picFace.Image = null;
            picPilotStatus.Image = null;
            picUnitStatus.Image = null;
            DisplayedUnit = null;
            DisplayedPilot = null;
            IsStatusWindowDisabled = false;
        }
    }
}
