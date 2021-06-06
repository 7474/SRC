using SRCCore;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.VB;
using SRCSharpForm.Extensions;
using SRCSharpForm.Lib;
using System;
using System.Collections.Generic;
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
        public Color StatusFontColorWarning = Color.FromArgb(190, 0, 0);

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

        private const float UnitStatusGridColumnWidth = 112f;

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
                g.DrawImage(BackgroundBuffer, 0, 48,
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

            // 情報を更新
            u.Update();

            // 未確認ユニットかどうか判定しておく
            var is_unknown = Expression.IsOptionDefined("ユニット情報隠蔽") && !u.IsConditionSatisfied("識別済み") && (u.Party0 == "敵" || u.Party0 == "中立")
                || u.IsConditionSatisfied("ユニット情報隠蔽");

            try
            {
                using (var pilotG = Graphics.FromImage(picPilotStatus.NewImageIfNull().Image))
                using (var unitG = Graphics.FromImage(picUnitStatus.NewImageIfNull().Image))
                {
                    pilotG.TextRenderingHint = StatusTextRenderingHint;
                    unitG.TextRenderingHint = StatusTextRenderingHint;
                    var headingMargin = HeadingFont.Height;
                    var textMargin = StatusFont.Height;
                    var smallMargin = StatusSmallFont.Height;

                    var ppic = new Printer(pilotG, StatusFont, StatusFontColorNormalString, UnitStatusGridColumnWidth);
                    var upic = new Printer(unitG, StatusFont, StatusFontColorNormalString, UnitStatusGridColumnWidth);

                    // パイロットが乗っていない？
                    if (u.CountPilot() == 0)
                    {
                        // キャラ画面をクリア
                        picFace.Image = null;
                        DrawPilotLabel(u, pilotG, unitG, headingMargin);
                        goto UnitStatus;
                    }

                    // 表示するパイロットを選択
                    // 表示対象の解決は外に求めてる
                    // 指定がなければメインパイロット
                    if (p == null)
                    {
                        p = u.MainPilot();
                    }
                    // 情報を更新
                    p.UpdateSupportMod();

                    {
                        // パイロット画像を表示
                        var fname = SRC.FileSystem.PathCombine("Pilot", p.get_Bitmap(false));
                        // TODO Impl ザコ＆汎用パイロットが乗るユニットの出撃選択時はパイロット画像の
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
                        var image = imageBuffer.Get(fname);
                        picFace.Image = image;
                    }
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
                        ppic.SetColor(StatusFontColorAbilityName);
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
                    upic.PushGrid();

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
                    upic.PushGrid();

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
                    upic.PushGrid();

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
                    upic.PushGrid();

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
                    upic.PushGrid();

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
                    upic.PushGrid();

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
                        upic.PushGrid();
                    }

                    upic.ClearGrid();

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
                            upic.PushGrid();
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
                            upic.PushGrid();
                        }
                    }
                    upic.ClearGrid();

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
                        // 表示するパイロット能力のリストを作成
                        var name_list = p.Skills.Select(x => x.Name).ToList();
                        // 付加されたパイロット特殊能力
                        foreach (var cond in u.Conditions.Where(x => x.IsEnable))
                        {
                            switch (Strings.Right(cond.Name, 3) ?? "")
                            {
                                case "付加２":
                                case "強化２":
                                    {
                                        switch (GeneralLib.LIndex(cond.StrData, 1) ?? "")
                                        {
                                            // 非表示の能力
                                            case "非表示":
                                            case "解説":
                                                {
                                                    break;
                                                }

                                            default:
                                                {
                                                    var stype = Strings.Left(cond.Name, Strings.Len(cond.Name) - 3);
                                                    switch (stype ?? "")
                                                    {
                                                        case "ハンター":
                                                        case "ＳＰ消費減少":
                                                        case "スペシャルパワー自動発動":
                                                            {
                                                                // 重複可能な能力
                                                                name_list.Add(stype);
                                                                break;
                                                            }

                                                        default:
                                                            {
                                                                // 既に所有している能力であればスキップ
                                                                if (!name_list.Contains(stype))
                                                                {
                                                                    name_list.Add(stype);
                                                                }
                                                                break;
                                                            }
                                                    }

                                                    break;
                                                }
                                        }

                                        break;
                                    }
                            }
                        }

                        // パイロット能力を表示
                        var n = 0;
                        var i = 0;
                        foreach (var stype in name_list)
                        {
                            i++;
                            string sname;
                            string slevel;
                            // 文字色をリセット
                            upic.SetColor(StatusFontColorNormalString);
                            if (i <= p.CountSkill())
                            {
                                sname = p.SkillName(i);
                                slevel = p.SkillLevel(i).ToString();
                            }
                            else
                            {
                                sname = p.SkillName(stype);
                                slevel = p.SkillLevel(stype).ToString();
                            }

                            if (Strings.InStr(sname, "非表示") > 0)
                            {
                                goto NextSkill;
                            }

                            switch (stype ?? "")
                            {
                                case "オーラ":
                                    {
                                        if (p.IsMainPilot)
                                        {
                                            if (u.AuraLevel(true) < u.AuraLevel() && !string.IsNullOrEmpty(Map.MapFileName))
                                            {
                                                upic.SetColor(StatusFontColorAbilityDisable);
                                            }

                                            if (u.AuraLevel(true) > Conversions.ToDouble(slevel))
                                            {
                                                sname = sname + "+" + SrcFormatter.Format(u.AuraLevel(true) - Conversions.ToDouble(slevel));
                                            }
                                        }

                                        break;
                                    }

                                case "超能力":
                                    {
                                        if (p.IsMainPilot)
                                        {
                                            if (u.PsychicLevel(true) < u.PsychicLevel() && !string.IsNullOrEmpty(Map.MapFileName))
                                            {
                                                upic.SetColor(StatusFontColorAbilityDisable);
                                            }

                                            if (u.PsychicLevel(true) > Conversions.ToDouble(slevel))
                                            {
                                                sname = sname + "+" + SrcFormatter.Format(u.PsychicLevel(true) - Conversions.ToDouble(slevel));
                                            }
                                        }

                                        break;
                                    }

                                case "底力":
                                case "超底力":
                                case "覚悟":
                                    {
                                        if (u.HP <= u.MaxHP / 4)
                                        {
                                            upic.SetColor(StatusFontColorAbilityEnable);
                                        }

                                        break;
                                    }

                                case "不屈":
                                    {
                                        if (u.HP <= u.MaxHP / 2)
                                        {
                                            upic.SetColor(StatusFontColorAbilityEnable);
                                        }

                                        break;
                                    }

                                case "潜在力開放":
                                    {
                                        if (p.Morale >= 130)
                                        {
                                            upic.SetColor(StatusFontColorAbilityEnable);
                                        }

                                        break;
                                    }

                                case "スペシャルパワー自動発動":
                                    {
                                        if (i <= p.CountSkill())
                                        {
                                            if (p.Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(p.SkillData(i), 3)))
                                            {
                                                upic.SetColor(StatusFontColorAbilityEnable);
                                            }
                                        }
                                        else
                                        {
                                            if (p.Morale >= GeneralLib.StrToLng(GeneralLib.LIndex(p.SkillData(i), 3)))
                                            {
                                                upic.SetColor(StatusFontColorAbilityEnable);
                                            }
                                        }

                                        break;
                                    }

                                case "Ｓ防御":
                                    {
                                        if (!u.IsFeatureAvailable("シールド")
                                            && !u.IsFeatureAvailable("大型シールド")
                                            && !u.IsFeatureAvailable("小型シールド")
                                            && !u.IsFeatureAvailable("エネルギーシールド")
                                            && !u.IsFeatureAvailable("アクティブシールド")
                                            && !u.IsFeatureAvailable("盾")
                                            && !u.IsFeatureAvailable("バリアシールド")
                                            && !u.IsFeatureAvailable("アクティブフィールド")
                                            && !u.IsFeatureAvailable("アクティブプロテクション")
                                            && Strings.InStr(u.FeatureData("阻止"), "Ｓ防御") == 0
                                            && Strings.InStr(u.FeatureData("広域阻止"), "Ｓ防御") == 0
                                            && Strings.InStr(u.FeatureData("反射"), "Ｓ防御") == 0
                                            && Strings.InStr(u.FeatureData("当て身技"), "Ｓ防御") == 0
                                            && Strings.InStr(u.FeatureData("自動反撃"), "Ｓ防御") == 0
                                            && !Map.IsStatusView)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        break;
                                    }

                                case "切り払い":
                                    {
                                        if (!u.IsFeatureAvailable("格闘武器")
                                            && !u.Weapons.Any(uw => uw.IsWeaponClassifiedAs("武"))
                                            && Strings.InStr(u.FeatureData("阻止"), "切り払い") == 0
                                            && Strings.InStr(u.FeatureData("広域阻止"), "切り払い") == 0
                                            && Strings.InStr(u.FeatureData("反射"), "切り払い") == 0
                                            && Strings.InStr(u.FeatureData("当て身技"), "切り払い") == 0
                                            && Strings.InStr(u.FeatureData("自動反撃"), "切り払い") == 0
                                            && !Map.IsStatusView)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        break;
                                    }

                                case "迎撃":
                                    {
                                        if (!u.IsFeatureAvailable("迎撃武器")
                                            && !u.Weapons.Any(uw => uw.CanUseIntercept())
                                            && Strings.InStr(u.FeatureData("阻止"), "迎撃") == 0
                                            && Strings.InStr(u.FeatureData("広域阻止"), "迎撃") == 0
                                            && Strings.InStr(u.FeatureData("反射"), "迎撃") == 0
                                            && Strings.InStr(u.FeatureData("当て身技"), "迎撃") == 0
                                            && Strings.InStr(u.FeatureData("自動反撃"), "迎撃") == 0
                                            && !Map.IsStatusView)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        break;
                                    }

                                case "浄化":
                                    {
                                        if (!u.Weapons.Any(uw => uw.IsWeaponClassifiedAs("浄"))
                                            && !Map.IsStatusView)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        break;
                                    }

                                case "援護":
                                    {
                                        if (!string.IsNullOrEmpty(Map.MapFileName))
                                        {
                                            int ret;
                                            if ((u.Party ?? "") == (SRC.Stage ?? ""))
                                            {
                                                ret = GeneralLib.MaxLng(u.MaxSupportAttack() - u.UsedSupportAttack, 0);
                                            }
                                            else
                                            {
                                                if (u.IsUnderSpecialPowerEffect("サポートガード不能"))
                                                {
                                                    // MOD START 240a
                                                    // upic.ForeColor = rgb(150, 0, 0)
                                                    upic.SetColor(StatusFontColorAbilityDisable);
                                                }

                                                ret = GeneralLib.MaxLng(u.MaxSupportGuard() - u.UsedSupportGuard, 0);
                                            }

                                            if (ret == 0)
                                            {
                                                upic.SetColor(StatusFontColorAbilityDisable);
                                            }

                                            sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
                                        }

                                        break;
                                    }

                                case "援護攻撃":
                                    {
                                        if (!string.IsNullOrEmpty(Map.MapFileName))
                                        {
                                            var ret = GeneralLib.MaxLng(u.MaxSupportAttack() - u.UsedSupportAttack, 0);
                                            if (ret == 0)
                                            {
                                                upic.SetColor(StatusFontColorAbilityDisable);
                                            }

                                            sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
                                        }

                                        break;
                                    }

                                case "援護防御":
                                    {
                                        if (!string.IsNullOrEmpty(Map.MapFileName))
                                        {
                                            var ret = GeneralLib.MaxLng(u.MaxSupportGuard() - u.UsedSupportGuard, 0);
                                            if (ret == 0 || u.IsUnderSpecialPowerEffect("サポートガード不能"))
                                            {
                                                upic.SetColor(StatusFontColorAbilityDisable);
                                            }

                                            sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
                                        }

                                        break;
                                    }

                                case "統率":
                                    {
                                        if (!string.IsNullOrEmpty(Map.MapFileName))
                                        {
                                            var ret = GeneralLib.MaxLng(u.MaxSyncAttack() - u.UsedSyncAttack, 0);
                                            if (ret == 0)
                                            {
                                                upic.SetColor(StatusFontColorAbilityDisable);
                                            }

                                            sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
                                        }

                                        break;
                                    }

                                case "カウンター":
                                    {
                                        if (!string.IsNullOrEmpty(Map.MapFileName))
                                        {
                                            var ret = GeneralLib.MaxLng(u.MaxCounterAttack() - u.UsedCounterAttack, 0);
                                            if (ret > 100)
                                            {
                                                sname = sname + " (残り∞回)";
                                            }
                                            else if (ret > 0)
                                            {
                                                sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
                                            }
                                            else
                                            {
                                                upic.SetColor(StatusFontColorAbilityDisable);
                                                sname = sname + " (残り0回)";
                                            }
                                        }

                                        break;
                                    }

                                case "先手必勝":
                                    {
                                        if (u.MaxCounterAttack() > 100)
                                        {
                                            upic.SetColor(StatusFontColorAbilityEnable);
                                        }

                                        break;
                                    }

                                case "耐久":
                                    {
                                        if (Expression.IsOptionDefined("防御力成長") || Expression.IsOptionDefined("防御力レベルアップ"))
                                        {
                                            goto NextSkill;
                                        }

                                        break;
                                    }

                                case "霊力":
                                case "同調率":
                                case "得意技":
                                case "不得手":
                                    goto NextSkill;
                            }

                            // 特殊能力名を表示
                            if (Strings.LenB(sname) > 19)
                            {
                                if (n > 0)
                                {
                                    upic.Print();
                                    //// ADD START 240a
                                    //if (GUI.NewGUIMode)
                                    //{
                                    //    upic.CurrentX = 5;
                                    //}
                                    //// ADD  END  240a
                                }
                                upic.Print(sname);
                                n = 2;
                            }
                            else
                            {
                                upic.Print(GeneralLib.RightPaddedString(sname, 19));
                                n = (n + 1);
                            }

                            upic.SetColor(StatusFontColorNormalString);

                            // 必要に応じて改行
                            if (n > 1)
                            {
                                upic.Print();
                                //// ADD START 240a
                                //if (GUI.NewGUIMode)
                                //{
                                //    upic.CurrentX = 5;
                                //}
                                //// ADD  END  240a
                                n = 0;
                            }

                        NextSkill:
                            ;
                        }

                        if (n > 0)
                        {
                            upic.Print();
                        }

                        upic.CurrentY = upic.CurrentY + 8;
                    }
                UnitStatus:
                    ;

                    // パイロットステータス表示用のダミーユニットの場合はここで表示を終了
                    if (u.IsFeatureAvailable("ダミーユニット"))
                    {
                        goto UpdateStatusWindow;
                    }

                    // ここからはユニットに関する情報
                    upic.ClearGrid();
                    upic.Print();
                    var mapCell = Map.CellAtPoint(u.x, u.y);
                    var td = mapCell.Terrain;
                    string buf;

                    // ユニット愛称
                    var offset = new Point();
                    offset.Y = textMargin * 8;
                    upic.SetFont(HeadingFont);
                    upic.Print(u.Nickname0);
                    upic.Print();

                    upic.SetFont(StatusSmallFont);
                    if (u.Status == "出撃" && !string.IsNullOrEmpty(Map.MapFileName))
                    {
                        // 地形情報の表示
                        // ユニットの位置を地形名称
                        if (Strings.InStr(td.Name, "(") > 0)
                        {
                            upic.Print(u.Area + " (" + Strings.Left(td.Name, Strings.InStr(td.Name, "(") - 1));
                        }
                        else
                        {
                            upic.Print(u.Area + " (" + td.Name);
                        }

                        // 回避＆防御修正
                        if (td.HitMod == td.DamageMod)
                        {
                            if (td.HitMod >= 0)
                            {
                                upic.Print(" 回＆防+" + SrcFormatter.Format(td.HitMod) + "%");
                            }
                            else
                            {
                                upic.Print(" 回＆防" + SrcFormatter.Format(td.HitMod) + "%");
                            }
                        }
                        else
                        {
                            if (td.HitMod >= 0)
                            {
                                upic.Print(" 回+" + SrcFormatter.Format(td.HitMod) + "%");
                            }
                            else
                            {
                                upic.Print(" 回" + SrcFormatter.Format(td.HitMod) + "%");
                            }

                            if (td.DamageMod >= 0)
                            {
                                upic.Print(" 防+" + SrcFormatter.Format(td.DamageMod) + "%");
                            }
                            else
                            {
                                upic.Print(" 防" + SrcFormatter.Format(td.DamageMod) + "%");
                            }
                        }

                        // ＨＰ＆ＥＮ回復
                        if (td.EffectForHPRecover() > 0)
                        {
                            upic.Print(" " + Strings.Left(Expression.Term("ＨＰ", u: null), 1) + "+" + SrcFormatter.Format(td.EffectForHPRecover()) + "%");
                        }

                        if (td.EffectForENRecover() > 0)
                        {
                            upic.Print(" " + Strings.Left(Expression.Term("ＥＮ", u: null), 1) + "+" + SrcFormatter.Format(td.EffectForENRecover()) + "%");
                        }

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
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print("ランク ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print(SrcFormatter.Format((object)u.Rank));
                    }
                    upic.ClearGrid();

                    // 未確認ユニット？
                    if (is_unknown)
                    {
                        // ＨＰ
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print(Expression.Term("ＨＰ", null, 6) + " ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print("?????/?????");
                        upic.PushGrid();

                        // ＥＮ
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print(Expression.Term("ＥＮ", null, 6) + " ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print("???/???");
                        upic.PushGrid();

                        // 装甲
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print(Expression.Term("装甲", null, 6) + " ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print(GeneralLib.RightPaddedString("？", 12));
                        upic.PushGrid();

                        // 運動性
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print(Expression.Term("運動性", null, 6) + " ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print("？");
                        upic.PushGrid();

                        // 移動タイプ
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print(Expression.Term("タイプ", null, 6) + " ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print(GeneralLib.RightPaddedString("？", 12));
                        upic.PushGrid();

                        // 移動力
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print(Expression.Term("移動力", null, 6) + " ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print("？");
                        upic.PushGrid();

                        // 地形適応
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print("適応   ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print(GeneralLib.RightPaddedString("？", 12));
                        upic.PushGrid();

                        // ユニットサイズ
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print(Expression.Term("サイズ", null, 6) + " ");
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print("？");
                        upic.PushGrid();

                        // サポートアタックを得られるかどうかのみ表示
                        if ((Commands.CommandState == "ターゲット選択" || Commands.CommandState == "移動後ターゲット選択") && (Commands.SelectedCommand == "攻撃" || Commands.SelectedCommand == "マップ攻撃") && Commands.SelectedUnit is object)
                        {
                            if (u.Party == "敵" || u.Party == "中立" || u.IsConditionSatisfied("暴走") || u.IsConditionSatisfied("魅了") || u.IsConditionSatisfied("憑依"))
                            {
                                upic.Print();

                                // 攻撃手段
                                upic.SetColor(StatusFontColorAbilityName);
                                upic.Print("攻撃     ");
                                upic.SetColor(StatusFontColorNormalString);
                                upic.Print(Commands.SelectedUnit.Weapon(Commands.SelectedWeapon).WeaponNickname());
                                // サポートアタックを得られる？
                                if (!Commands.SelectedUnit.Weapon(Commands.SelectedWeapon).IsWeaponClassifiedAs("合")
                                    && !Commands.SelectedUnit.Weapon(Commands.SelectedWeapon).IsWeaponClassifiedAs("Ｍ"))
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
                    if (u.Party == "ＮＰＣ"
                        && !u.IsConditionSatisfied("混乱")
                        && !u.IsConditionSatisfied("恐怖")
                        && !u.IsConditionSatisfied("暴走")
                        && !u.IsConditionSatisfied("狂戦士"))
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

                        if (buf == "通常")
                        {
                            upic.Print("自由行動中");
                        }
                        else if (SRC.PList.IsDefined(buf))
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
                    upic.ClearGrid();

                    // ユニットにかかっている特殊ステータス
                    {
                        // TODO Impl ユニットにかかっている特殊ステータス
                        //name_list = new string[1];
                        //var loopTo8 = u.CountCondition();
                        //for (i = 1; i <= loopTo8; i++)
                        //{
                        //    // 時間切れ？
                        //    if (u.ConditionLifetime(i) == 0)
                        //    {
                        //        goto NextCondition;
                        //    }

                        //    // 非表示？
                        //    string localConditionData1() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //    if (Strings.InStr(localConditionData1(), "非表示") > 0)
                        //    {
                        //        goto NextCondition;
                        //    }

                        //    // 解説？
                        //    string localConditionData2() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //    if (GeneralLib.LIndex(localConditionData2(), 1) == "解説")
                        //    {
                        //        goto NextCondition;
                        //    }
                        //    // ADD START 240a
                        //    if (GUI.NewGUIMode)
                        //    {
                        //        upic.CurrentX = 5;
                        //    }
                        //    // ADD  END  240a
                        //    switch (u.Condition(i) ?? "")
                        //    {
                        //        case "データ不明":
                        //        case "形態固定":
                        //        case "機体固定":
                        //        case "不死身":
                        //        case "無敵":
                        //        case "識別済み":
                        //        case "非操作":
                        //        case "破壊キャンセル":
                        //        case "盾ダメージ":
                        //        case "能力コピー":
                        //        case "メッセージ付加":
                        //        case "ノーマルモード付加":
                        //        case "追加パイロット付加":
                        //        case "追加サポート付加":
                        //        case "パイロット愛称付加":
                        //        case "パイロット画像付加":
                        //        case "性格変更付加":
                        //        case "性別付加":
                        //        case "ＢＧＭ付加":
                        //        case "愛称変更付加":
                        //        case "スペシャルパワー無効化":
                        //        case "精神コマンド無効化":
                        //        case "ユニット画像付加":
                        //        case var case12 when case12 == "メッセージ付加":
                        //            {
                        //                break;
                        //            }
                        //        // 非表示
                        //        case "残り時間":
                        //            {
                        //                int localConditionLifetime1() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime2() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime1() && localConditionLifetime2() < 100)
                        //                {
                        //                    int localConditionLifetime() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print("残り時間" + SrcFormatter.Format(localConditionLifetime()) + "ターン");
                        //                }

                        //                break;
                        //            }

                        //        case "無効化付加":
                        //        case "耐性付加":
                        //        case "吸収付加":
                        //        case "弱点付加":
                        //            {
                        //                string localConditionData3() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                upic.Print(localConditionData3() + cond.Name);
                        //                int localConditionLifetime4() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime5() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime4() && localConditionLifetime5() < 100)
                        //                {
                        //                    int localConditionLifetime3() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" " + SrcFormatter.Format(localConditionLifetime3()) + "T");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "特殊効果無効化付加":
                        //            {
                        //                string localConditionData4() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                upic.Print(localConditionData4() + "無効化付加");
                        //                int localConditionLifetime7() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime8() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime7() && localConditionLifetime8() < 100)
                        //                {
                        //                    int localConditionLifetime6() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime6()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "攻撃属性付加":
                        //            {
                        //                string localConditionData5() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                string localLIndex2() { string arglist = hs755742c2c238431abd43e11d0920ad14(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                        //                upic.Print(localLIndex2() + "属性付加");
                        //                int localConditionLifetime10() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime11() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime10() && localConditionLifetime11() < 100)
                        //                {
                        //                    int localConditionLifetime9() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime9()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "武器強化付加":
                        //            {
                        //                double localConditionLevel() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                        //                upic.Print("武器強化Lv" + localConditionLevel() + "付加");
                        //                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                        //                {
                        //                    string localConditionData6() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                    upic.Print("(" + localConditionData6() + ")");
                        //                }

                        //                int localConditionLifetime13() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime14() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime13() && localConditionLifetime14() < 100)
                        //                {
                        //                    int localConditionLifetime12() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime12()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "命中率強化付加":
                        //            {
                        //                double localConditionLevel1() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                        //                upic.Print("命中率強化Lv" + localConditionLevel1() + "付加");
                        //                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                        //                {
                        //                    string localConditionData7() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                    upic.Print("(" + localConditionData7() + ")");
                        //                }

                        //                int localConditionLifetime16() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime17() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime16() && localConditionLifetime17() < 100)
                        //                {
                        //                    int localConditionLifetime15() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime15()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "ＣＴ率強化付加":
                        //            {
                        //                double localConditionLevel2() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                        //                upic.Print("ＣＴ率強化Lv" + localConditionLevel2() + "付加");
                        //                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                        //                {
                        //                    string localConditionData8() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                    upic.Print("(" + localConditionData8() + ")");
                        //                }

                        //                int localConditionLifetime19() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime20() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime19() && localConditionLifetime20() < 100)
                        //                {
                        //                    int localConditionLifetime18() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime18()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "特殊効果発動率強化付加":
                        //            {
                        //                double localConditionLevel3() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                        //                upic.Print("特殊効果発動率強化Lv" + localConditionLevel3() + "付加");
                        //                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                        //                {
                        //                    string localConditionData9() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                    upic.Print("(" + localConditionData9() + ")");
                        //                }

                        //                int localConditionLifetime22() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime23() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime22() && localConditionLifetime23() < 100)
                        //                {
                        //                    int localConditionLifetime21() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime21()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "地形適応変更付加":
                        //            {
                        //                upic.Print("地形適応変更付加");
                        //                int localConditionLifetime25() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime26() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime25() && localConditionLifetime26() < 100)
                        //                {
                        //                    int localConditionLifetime24() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime24()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "盾付加":
                        //            {
                        //                string localConditionData10() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                string localLIndex3() { string arglist = hsba8faef602a144028d0b911086dca487(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                        //                upic.Print(localLIndex3() + "付加");
                        //                double localConditionLevel4() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                        //                upic.Print("(" + SrcFormatter.Format(localConditionLevel4()) + ")");
                        //                int localConditionLifetime28() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime29() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime28() && localConditionLifetime29() < 100)
                        //                {
                        //                    int localConditionLifetime27() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime27()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "ダミー破壊":
                        //            {
                        //                buf = u.FeatureName("ダミー");
                        //                if (Strings.InStr(buf, "Lv") > 0)
                        //                {
                        //                    buf = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
                        //                }
                        //                double localConditionLevel5() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                        //                upic.Print(buf + Strings.StrConv(SrcFormatter.Format(localConditionLevel5()), VbStrConv.Wide) + "体破壊");
                        //                break;
                        //            }

                        //        case "ダミー付加":
                        //            {
                        //                double localConditionLevel6() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                        //                upic.Print(u.FeatureName("ダミー") + "残り" + Strings.StrConv(SrcFormatter.Format(localConditionLevel6()), VbStrConv.Wide) + "体");
                        //                int localConditionLifetime31() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime32() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime31() && localConditionLifetime32() < 100)
                        //                {
                        //                    int localConditionLifetime30() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime30()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "バリア発動":
                        //            {
                        //                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                        //                {
                        //                    string localConditionData11() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                    upic.Print(localConditionData11());
                        //                }
                        //                else
                        //                {
                        //                    upic.Print("バリア発動");
                        //                }
                        //                int localConditionLifetime33() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime33()) + "ターン");
                        //                break;
                        //            }

                        //        case "フィールド発動":
                        //            {
                        //                if (!string.IsNullOrEmpty(u.ConditionData(i)))
                        //                {
                        //                    string localConditionData12() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                    upic.Print(localConditionData12());
                        //                }
                        //                else
                        //                {
                        //                    upic.Print("フィールド発動");
                        //                }
                        //                int localConditionLifetime34() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime34()) + "ターン");
                        //                break;
                        //            }

                        //        case "装甲劣化":
                        //            {
                        //                upic.Print(Expression.Term("装甲", u) + "劣化");
                        //                int localConditionLifetime36() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime37() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime36() && localConditionLifetime37() < 20)
                        //                {
                        //                    int localConditionLifetime35() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime35()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "運動性ＵＰ":
                        //            {
                        //                upic.Print(Expression.Term("運動性", u) + "ＵＰ");
                        //                int localConditionLifetime39() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime40() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime39() && localConditionLifetime40() < 20)
                        //                {
                        //                    int localConditionLifetime38() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime38()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "運動性ＤＯＷＮ":
                        //            {
                        //                upic.Print(Expression.Term("運動性", u) + "ＤＯＷＮ");
                        //                int localConditionLifetime42() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime43() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime42() && localConditionLifetime43() < 20)
                        //                {
                        //                    int localConditionLifetime41() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime41()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "移動力ＵＰ":
                        //            {
                        //                upic.Print(Expression.Term("移動力", u) + "ＵＰ");
                        //                int localConditionLifetime45() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime46() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime45() && localConditionLifetime46() < 20)
                        //                {
                        //                    int localConditionLifetime44() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime44()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        case "移動力ＤＯＷＮ":
                        //            {
                        //                upic.Print(Expression.Term("移動力", u) + "ＤＯＷＮ");
                        //                int localConditionLifetime48() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime49() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime48() && localConditionLifetime49() < 20)
                        //                {
                        //                    int localConditionLifetime47() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime47()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }

                        //        default:
                        //            {
                        //                // 充填中？
                        //                string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                if (Strings.Right(cond.Name, 3) == "充填中")
                        //                {
                        //                    if (u.IsHero())
                        //                    {
                        //                        string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                        string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                        upic.Print(Strings.Left(cond.Name, Strings.Len(cond.Name) - 3));
                        //                        upic.Print("準備中");
                        //                    }
                        //                    else
                        //                    {
                        //                        string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                        upic.Print(cond.Name);
                        //                    }
                        //                    int localConditionLifetime50() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime50()) + "ターン");
                        //                    goto NextCondition;
                        //                }

                        //                // パイロット特殊能力付加＆強化による状態は表示しない
                        //                string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                if (Strings.Right(cond.Name, 3) == "付加２" || Strings.Right(cond.Name, 3) == "強化２")
                        //                {
                        //                    goto NextCondition;
                        //                }

                        //                string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                string localConditionData15() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                string localConditionData16() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                double localConditionLevel9() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                        //                if (Strings.Right(cond.Name, 2) == "付加" && !string.IsNullOrEmpty(localConditionData15()))
                        //                {
                        //                    string localConditionData13() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                    buf = GeneralLib.LIndex(localConditionData13(), 1) + "付加";
                        //                }
                        //                else if (Strings.Right(cond.Name, 2) == "強化" && !string.IsNullOrEmpty(localConditionData16()))
                        //                {
                        //                    // 強化アビリティ
                        //                    string localConditionData14() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

                        //                    string localLIndex4() { string arglist = hs68f2e5d1358d41f987f02a7379e2b562(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

                        //                    double localConditionLevel7() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                        //                    buf = localLIndex4() + "強化Lv" + localConditionLevel7();
                        //                }
                        //                else if (localConditionLevel9() > 0d)
                        //                {
                        //                    // 付加アビリティ(レベル指定あり)
                        //                    string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                    string cond.Name { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

                        //                    double localConditionLevel8() { object argIndex1 = i; var ret = u.ConditionLevel(argIndex1); return ret; }

                        //                    buf = Strings.Left(cond.Name, Strings.Len(cond.Name) - 2) + "Lv" + SrcFormatter.Format(localConditionLevel8()) + "付加";
                        //                }
                        //                else
                        //                {
                        //                    // 付加アビリティ(レベル指定なし)
                        //                    buf = u.Condition(i);
                        //                }

                        //                // エリアスされた特殊能力の付加表示がたぶらないように
                        //                var loopTo9 = Information.UBound(name_list);
                        //                for (j = 1; j <= loopTo9; j++)
                        //                {
                        //                    if ((buf ?? "") == (name_list[j] ?? ""))
                        //                    {
                        //                        break;
                        //                    }
                        //                }

                        //                if (j <= Information.UBound(name_list))
                        //                {
                        //                    goto NextCondition;
                        //                }

                        //                Array.Resize(name_list, Information.UBound(name_list) + 1 + 1);
                        //                name_list[Information.UBound(name_list)] = buf;

                        //                upic.Print(buf);
                        //                int localConditionLifetime52() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                int localConditionLifetime53() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                if (0 < localConditionLifetime52() && localConditionLifetime53() < 20)
                        //                {
                        //                    int localConditionLifetime51() { object argIndex1 = i; var ret = u.ConditionLifetime(argIndex1); return ret; }

                        //                    upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime51()) + "ターン");
                        //                }
                        //                upic.Print("");
                        //                break;
                        //            }
                        //    }

                        //NextCondition:
                        //    ;
                        //}

                    }

                    // ＨＰ
                    // TODO Impl HP Bar
                    //cx = upic.CurrentX;
                    //cy = upic.CurrentY;
                    //upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //upic.Line(117, cy + 8); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //upic.Line(118 + GUI.GauageWidth, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //if (u.HP > 0)
                    //{
                    //    upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //}
                    //upic.CurrentX = cx;
                    //upic.CurrentY = cy;
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("ＨＰ", u, 6) + " ");
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
                    upic.PushGrid();

                    // ＥＮ
                    // TODO Impl EN Bar
                    //cx = upic.CurrentX;
                    //cy = upic.CurrentY;
                    //upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //upic.Line(117, cy + 8); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //upic.Line(118 + GUI.GauageWidth, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //if (u.EN > 0)
                    //{
                    //    upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
                    //}
                    //upic.CurrentX = cx;
                    //upic.CurrentY = cy;
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("ＥＮ", u, 6) + " ");
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
                    upic.PushGrid();

                    // 装甲
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("装甲", u, 6) + " ");
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
                    upic.PushGrid();

                    // 運動性
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("運動性", u, 6) + " ");
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
                    upic.PushGrid();

                    // 移動タイプ
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("タイプ", u, 6) + " ");
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(GeneralLib.RightPaddedString(u.Transportation, 12));
                    upic.PushGrid();

                    // 移動力
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("移動力", u, 6) + " ");
                    upic.SetColor(StatusFontColorNormalString);
                    if (u.IsFeatureAvailable("テレポート") && (u.Data.Speed == 0 || GeneralLib.LIndex(u.FeatureData("テレポート"), 2) == "0"))
                    {
                        upic.Print(SrcFormatter.Format(u.Speed + u.FeatureLevel("テレポート")));
                    }
                    else
                    {
                        upic.Print(SrcFormatter.Format(u.Speed));
                    }
                    upic.PushGrid();

                    // 地形適応
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print("適応   ");
                    upic.SetColor(StatusFontColorNormalString);
                    for (var i = 1; i <= 4; i++)
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
                    upic.PushGrid();

                    // ユニットサイズ
                    upic.SetColor(StatusFontColorAbilityName);
                    upic.Print(Expression.Term("サイズ", u, 6) + " ");
                    upic.SetColor(StatusFontColorNormalString);
                    upic.Print(Strings.StrConv(u.Size, VbStrConv.Wide));
                    upic.PushGrid();

                    upic.ClearGrid();
                    {
                        // XXX n の処理
                        var n = 0;
                        // 防御属性の表示
                        {
                            // 吸収
                            if (Strings.Len(u.strAbsorb) > 0 && Strings.InStr(u.strAbsorb, "非表示") == 0)
                            {
                                if (Strings.Len(u.strAbsorb) > 5)
                                {
                                    if (n > 0)
                                    {
                                        upic.Print();
                                    }

                                    n = 2;
                                }
                                upic.SetColor(StatusFontColorAbilityName);
                                upic.Print("吸収   ");
                                upic.SetColor(StatusFontColorNormalString);
                                upic.Print(GeneralLib.RightPaddedString(u.strAbsorb, 12));
                                n = (n + 1);
                                if (n > 1)
                                {
                                    upic.Print();
                                    n = 0;
                                }
                            }

                            // 無効化
                            if (Strings.Len(u.strImmune) > 0 && Strings.InStr(u.strImmune, "非表示") == 0)
                            {
                                if (Strings.Len(u.strImmune) > 5)
                                {
                                    if (n > 0)
                                    {
                                        upic.Print();
                                    }

                                    n = 2;
                                }
                                upic.SetColor(StatusFontColorAbilityName);
                                upic.Print("無効化 ");
                                upic.SetColor(StatusFontColorNormalString);
                                upic.Print(GeneralLib.RightPaddedString(u.strImmune, 12));
                                n = (n + 1);
                                if (n > 1)
                                {
                                    upic.Print();
                                    n = 0;
                                }
                            }

                            // 耐性
                            if (Strings.Len(u.strResist) > 0 && Strings.InStr(u.strResist, "非表示") == 0)
                            {
                                if (Strings.Len(u.strResist) > 5)
                                {
                                    if (n > 0)
                                    {
                                        upic.Print();
                                    }

                                    n = 2;
                                }

                                upic.SetColor(StatusFontColorAbilityName);
                                upic.Print("耐性   ");
                                // MOD START 240a
                                // upic.ForeColor = rgb(0, 0, 0)
                                upic.SetColor(StatusFontColorNormalString);
                                upic.Print(GeneralLib.RightPaddedString(u.strResist, 12));
                                n = (n + 1);
                                if (n > 1)
                                {
                                    upic.Print();
                                    n = 0;
                                }
                            }

                            // 弱点
                            if (Strings.Len(u.strWeakness) > 0 && Strings.InStr(u.strWeakness, "非表示") == 0)
                            {
                                if (Strings.Len(u.strWeakness) > 5)
                                {
                                    if (n > 0)
                                    {
                                        upic.Print();
                                    }

                                    n = 2;
                                }

                                upic.SetColor(StatusFontColorAbilityName);
                                upic.Print("弱点   ");
                                upic.SetColor(StatusFontColorNormalString);
                                upic.Print(GeneralLib.RightPaddedString(u.strWeakness, 12));
                                n = (n + 1);
                                if (n > 1)
                                {
                                    upic.Print();
                                    n = 0;
                                }
                            }

                            // 有効
                            if (Strings.Len(u.strEffective) > 0 && Strings.InStr(u.strEffective, "非表示") == 0)
                            {
                                if (Strings.Len(u.strEffective) > 5)
                                {
                                    if (n > 0)
                                    {
                                        upic.Print();
                                    }

                                    n = 2;
                                }

                                upic.SetColor(StatusFontColorAbilityName);
                                upic.Print("有効   ");
                                upic.SetColor(StatusFontColorNormalString);
                                upic.Print(GeneralLib.RightPaddedString(u.strEffective, 12));
                                n = (n + 1);
                                if (n > 1)
                                {
                                    upic.Print();
                                    n = 0;
                                }
                            }

                            // 特殊効果無効化
                            if (Strings.Len(u.strSpecialEffectImmune) > 0 && Strings.InStr(u.strSpecialEffectImmune, "非表示") == 0)
                            {
                                if (Strings.Len(u.strSpecialEffectImmune) > 5)
                                {
                                    if (n > 0)
                                    {
                                        upic.Print();
                                    }

                                    n = 2;
                                }

                                upic.SetColor(StatusFontColorAbilityName);
                                upic.Print("特無効 ");
                                upic.SetColor(StatusFontColorNormalString);
                                upic.Print(GeneralLib.RightPaddedString(u.strSpecialEffectImmune, 12));
                                n = (n + 1);
                                if (n > 1)
                                {
                                    upic.Print();
                                    n = 0;
                                }
                            }

                            // 必要に応じて改行
                            if (n > 0)
                            {
                                upic.Print();
                            }
                        }

                        // 武器・防具クラス
                        var flist = new List<string>();
                        if (Expression.IsOptionDefined("アイテム交換"))
                        {
                            if (u.IsFeatureAvailable("武器クラス") || u.IsFeatureAvailable("防具クラス"))
                            {
                                upic.Print(GeneralLib.RightPaddedString("武器・防具クラス", 19));
                                flist.Add("武器・防具クラス");
                                n = (n + 1);
                            }
                        }

                        // 特殊能力一覧を表示する前に必要気力判定のためメインパイロットの気力を参照
                        int pmorale;
                        if (u.CountPilot() > 0)
                        {
                            pmorale = u.MainPilot().Morale;
                        }
                        else
                        {
                            pmorale = 150;
                        }

                        // 特殊能力一覧
                        foreach (var fd in u.AllFeatures)
                        {
                            var fname = fd.Name;

                            // ユニットステータスコマンド時は通常は非表示のパーツ合体、
                            // ノーマルモード、換装も表示
                            if (string.IsNullOrEmpty(fname))
                            {
                                if (string.IsNullOrEmpty(Map.MapFileName))
                                {
                                    switch (fd.Name)
                                    {
                                        case "パーツ合体":
                                        case "ノーマルモード":
                                            {
                                                upic.Print(GeneralLib.RightPaddedString(fd.Name, 19));
                                                n = (n + 1);
                                                if (n > 1)
                                                {
                                                    upic.Print();
                                                    n = 0;
                                                }

                                                break;
                                            }

                                        case "換装":
                                            {
                                                // エリアスで換装の名称が変更されている？
                                                fname = SRC.ALDList.RefName("換装");
                                                upic.Print(GeneralLib.RightPaddedString(fname, 19));
                                                n = (n + 1);
                                                if (n > 1)
                                                {
                                                    upic.Print();
                                                    n = 0;
                                                }

                                                break;
                                            }
                                    }
                                }

                                goto NextFeature;
                            }

                            // 既に表示しているかを判定
                            if (flist.Contains(fname))
                            {
                                goto NextFeature;
                            }
                            flist.Add(fname);

                            // 使用可否によって表示色を変える
                            var fdata = fd.Data;
                            switch (fd.Name)
                            {
                                case "合体":
                                    {
                                        if (!SRC.UList.IsDefined(fd.DataL[1]))
                                        {
                                            goto NextFeature;
                                        }
                                        if (SRC.UList.Item(fd.DataL[1]).IsConditionSatisfied("行動不能"))
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }
                                        break;
                                    }

                                case "分離":
                                    {
                                        // XXX 分離の可否どっかにまとまっていて欲しい
                                        var k = 0;
                                        var loopTo13 = GeneralLib.LLength(fdata);
                                        for (var j = 2; j <= loopTo13; j++)
                                        {
                                            var uname = GeneralLib.LIndex(fdata, j);
                                            if (!SRC.UList.IsDefined(uname))
                                            {
                                                goto NextFeature;
                                            }
                                            var ud = SRC.UList.Item(uname).Data;
                                            if (ud.IsFeatureAvailable("召喚ユニット"))
                                            {
                                                k = (k + Math.Abs(ud.PilotNum));
                                            }
                                        }
                                        if (u.CountPilot() < k)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        break;
                                    }

                                case "ハイパーモード":
                                    {
                                        if (pmorale < (10d * fd.Level) + 100 && u.HP > u.MaxHP / 4)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }
                                        else if (u.IsConditionSatisfied("ノーマルモード付加"))
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        break;
                                    }

                                case "修理装置":
                                case "補給装置":
                                    {
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                                        {
                                            if (u.EN < Conversions.ToInteger(GeneralLib.LIndex(fdata, 2)))
                                            {
                                                upic.SetColor(StatusFontColorAbilityDisable);
                                            }
                                        }

                                        break;
                                    }

                                case "テレポート":
                                    {
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                                        {
                                            if (u.EN < Conversions.ToInteger(GeneralLib.LIndex(fdata, 2)))
                                            {
                                                upic.SetColor(StatusFontColorAbilityDisable);
                                            }
                                        }
                                        else if (u.EN < 40)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        break;
                                    }

                                case "分身":
                                    {
                                        if (pmorale < 130)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        break;
                                    }

                                case "超回避":
                                    {
                                        int ecost;
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                                        {
                                            ecost = Conversions.ToInteger(GeneralLib.LIndex(fdata, 2));
                                        }
                                        else
                                        {
                                            ecost = 0;
                                        }
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 3));
                                        }
                                        else
                                        {
                                            nmorale = 0;
                                        }

                                        if (u.EN < ecost || pmorale < nmorale)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        break;
                                    }

                                case "緊急テレポート":
                                    {
                                        int ecost;
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                        {
                                            ecost = Conversions.ToInteger(GeneralLib.LIndex(fdata, 3));
                                        }
                                        else
                                        {
                                            ecost = 0;
                                        }
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 4));
                                        }
                                        else
                                        {
                                            nmorale = 0;
                                        }

                                        if (u.EN < ecost || pmorale < nmorale)
                                        {
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
                                        int ecost;
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                        {
                                            ecost = Conversions.ToInteger(GeneralLib.LIndex(fdata, 3));
                                        }
                                        else
                                        {
                                            ecost = 10;
                                        }

                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 4));
                                        }
                                        else
                                        {
                                            nmorale = 0;
                                        }

                                        if (u.EN < ecost
                                            || pmorale < nmorale
                                            || u.IsConditionSatisfied("バリア無効化") && Strings.InStr(fdata, "バリア無効化無効") == 0)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }
                                        else if (Strings.InStr(fdata, "能力必要") > 0)
                                        {
                                            var loopTo14 = GeneralLib.LLength(fdata);
                                            for (var j = 5; j <= loopTo14; j++)
                                            {
                                                var opt = GeneralLib.LIndex(fdata, j);
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
                                        int ecost;
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                        {
                                            ecost = Conversions.ToInteger(GeneralLib.LIndex(fdata, 3));
                                        }
                                        else
                                        {
                                            ecost = 0;
                                        }

                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 4));
                                        }
                                        else
                                        {
                                            nmorale = 0;
                                        }

                                        if (u.EN < ecost || pmorale < nmorale || u.IsConditionSatisfied("バリア無効化") && Strings.InStr(fdata, "バリア無効化無効") == 0)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }
                                        else if (Strings.InStr(fdata, "能力必要") > 0)
                                        {
                                            var loopTo15 = GeneralLib.LLength(fdata);
                                            for (var j = 5; j <= loopTo15; j++)
                                            {
                                                var opt = GeneralLib.LIndex(fdata, j);
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
                                        int ecost;
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                                        {
                                            ecost = Conversions.ToInteger(GeneralLib.LIndex(fdata, 4));
                                        }
                                        else if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
                                        {
                                            ecost = (20 * Conversions.ToInteger(GeneralLib.LIndex(fdata, 2)));
                                        }
                                        else
                                        {
                                            ecost = 0;
                                        }

                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 5));
                                        }
                                        else
                                        {
                                            nmorale = 0;
                                        }

                                        if (u.EN < ecost || pmorale < nmorale || u.IsConditionSatisfied("バリア無効化") && Strings.InStr(fdata, "バリア無効化無効") == 0)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        fname = fname + "(範囲" + GeneralLib.LIndex(fdata, 2) + "マス)";
                                        break;
                                    }

                                case "アーマー":
                                case "レジスト":
                                    {
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 3));
                                        }
                                        else
                                        {
                                            nmorale = 0;
                                        }

                                        if (pmorale < nmorale)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }
                                        else if (Strings.InStr(fdata, "能力必要") > 0)
                                        {
                                            var loopTo16 = GeneralLib.LLength(fdata);
                                            for (var j = 4; j <= loopTo16; j++)
                                            {
                                                var opt = GeneralLib.LIndex(fdata, j);
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
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 3));
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
                                        int ecost;
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
                                        {
                                            ecost = Conversions.ToInteger(GeneralLib.LIndex(fdata, 4));
                                        }
                                        else
                                        {
                                            ecost = 0;
                                        }

                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 5));
                                        }
                                        else
                                        {
                                            nmorale = 0;
                                        }

                                        if (u.EN < ecost || pmorale < nmorale)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }
                                        else if (Strings.InStr(fdata, "能力必要") > 0)
                                        {
                                            var loopTo17 = GeneralLib.LLength(fdata);
                                            for (var j = 6; j <= loopTo17; j++)
                                            {
                                                var opt = GeneralLib.LIndex(fdata, j);
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
                                        int ecost;
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
                                        {
                                            ecost = Conversions.ToInteger(GeneralLib.LIndex(fdata, 5));
                                        }
                                        else
                                        {
                                            ecost = 0;
                                        }

                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 6));
                                        }
                                        else
                                        {
                                            nmorale = 0;
                                        }

                                        if (u.EN < ecost || pmorale < nmorale)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        fname = fname + "(範囲" + GeneralLib.LIndex(fdata, 2) + "マス)";
                                        break;
                                    }

                                case "当て身技":
                                case "自動反撃":
                                    {
                                        int ecost;
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
                                        {
                                            ecost = Conversions.ToInteger(GeneralLib.LIndex(fdata, 5));
                                        }
                                        else
                                        {
                                            ecost = 0;
                                        }

                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 6));
                                        }
                                        else
                                        {
                                            nmorale = 0;
                                        }

                                        if (u.EN < ecost || pmorale < nmorale)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }
                                        else if (Strings.InStr(fdata, "能力必要") > 0)
                                        {
                                            var loopTo18 = GeneralLib.LLength(fdata);
                                            for (var j = 7; j <= loopTo18; j++)
                                            {
                                                var opt = GeneralLib.LIndex(fdata, j);
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
                                            upic.SetColor(StatusFontColorAbilityEnable);
                                        }

                                        break;
                                    }

                                case "盾":
                                    {
                                        if ((u.Condition("盾ダメージ")?.Level ?? 0) >= fd.Level)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        fname = fname + "(" + SrcFormatter.Format(GeneralLib.MaxLng((int)(fd.Level - (u.Condition("盾ダメージ")?.Level ?? 0)), 0)) + "/" + SrcFormatter.Format(fd.Level) + ")";
                                        break;
                                    }

                                case "ＨＰ回復":
                                case "ＥＮ回復":
                                    {
                                        if (u.IsConditionSatisfied("回復不能") || u.IsSpecialPowerInEffect("回復不能"))
                                        {
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
                                                upic.SetColor(StatusFontColorAbilityEnable);
                                            }
                                        }

                                        break;
                                    }

                                case "ＺＯＣ":
                                    {
                                        int j;
                                        if (GeneralLib.LLength(fdata) < 2)
                                        {
                                            j = 1;
                                        }
                                        else
                                        {
                                            j = Conversions.ToInteger(GeneralLib.LIndex(fdata, 2));
                                        }

                                        if (j >= 1)
                                        {
                                            fdata = fdata.Replace(Constants.vbTab, " ");
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
                                        int ecost;
                                        int nmorale;
                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
                                        {
                                            ecost = Conversions.ToInteger(GeneralLib.LIndex(fdata, 5));
                                        }
                                        else
                                        {
                                            ecost = 0;
                                        }

                                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
                                        {
                                            nmorale = Conversions.ToInteger(GeneralLib.LIndex(fdata, 6));
                                        }
                                        else
                                        {
                                            nmorale = 0;
                                        }

                                        if (u.EN < ecost || pmorale < nmorale)
                                        {
                                            upic.SetColor(StatusFontColorAbilityDisable);
                                        }

                                        break;
                                    }
                            }

                            // 必要条件を満たさない特殊能力は赤色で表示
                            if (!u.IsFeatureActivated(fd))
                            {
                                upic.SetColor(StatusFontColorAbilityDisable);
                            }

                            // 特殊能力名を表示
                            if (GeneralLib.StrWidth(fname) > 19)
                            {
                                if (n > 0)
                                {
                                    upic.Print();
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
                                n = 0;
                            }

                            // 表示色を戻しておく
                            upic.SetColor(StatusFontColorNormalString);
                        NextFeature:
                            ;
                        }

                        if (n > 0)
                        {
                            upic.Print();
                        }
                    }

                    upic.ClearGrid();
                    // アイテム一覧
                    if (u.CountItem() > 0)
                    {
                        var j = 0;
                        var loopTo19 = u.CountItem();
                        for (var i = 1; i <= loopTo19; i++)
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
                                    j = 0;
                                }
                            }

                        NextItem:
                            ;
                        }

                        if (j > 0)
                        {
                            upic.Print();
                        }
                    }

                    // ターゲット選択時の攻撃結果予想表示
                    {
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

                        upic.ClearGrid();
                        upic.Print();
                        upic.SetGridCellWidth(55f);

                        // 攻撃手段
                        var uw = Commands.SelectedUnit.Weapon(Commands.SelectedWeapon);
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print("攻撃     ");
                        upic.PushGrid();
                        upic.SetColor(StatusFontColorNormalString);
                        upic.Print(uw.WeaponNickname());
                        // サポートアタックを得られる？
                        if (!uw.IsWeaponClassifiedAs("合")
                            && !uw.IsWeaponClassifiedAs("Ｍ")
                            && Commands.UseSupportAttack)
                        {
                            if (Commands.SelectedUnit.LookForSupportAttack(u) is object)
                            {
                                upic.Print(" [援]");
                            }
                        }
                        upic.ClearGrid();

                        // 反撃を受ける？
                        UnitWeapon cw;
                        int tw;
                        if (u.MaxAction() == 0
                            || uw.IsWeaponClassifiedAs("Ｍ")
                            || uw.IsWeaponClassifiedAs("間"))
                        {
                            tw = 0;
                        }
                        else
                        {
                            tw = SRC.COM.SelectWeapon(u, Commands.SelectedUnit, "反撃", out _, out _);
                        }

                        // 敵の防御行動を設定
                        var def_mode = SRC.COM.SelectDefense(Commands.SelectedUnit, Commands.SelectedWeapon, u, tw);
                        if (!string.IsNullOrEmpty(def_mode))
                        {
                            tw = 0;
                        }

                        // 予測ダメージ
                        if (!Expression.IsOptionDefined("予測ダメージ非表示"))
                        {
                            upic.SetColor(StatusFontColorAbilityName);
                            upic.Print("ダメージ ");
                            upic.PushGrid();
                            var dmg = uw.Damage(u, true);
                            if (def_mode == "防御")
                            {
                                dmg = dmg / 2;
                            }

                            if (dmg >= u.HP && !u.IsConditionSatisfied("データ不明"))
                            {
                                upic.SetColor(StatusFontColorWarning);
                            }
                            else
                            {
                                upic.SetColor(StatusFontColorNormalString);
                            }
                            upic.Print(SrcFormatter.Format(dmg));
                            upic.ClearGrid();
                        }

                        // 予測命中率
                        if (!Expression.IsOptionDefined("予測命中率非表示"))
                        {
                            upic.SetColor(StatusFontColorAbilityName);
                            upic.Print("命中率   ");
                            upic.PushGrid();
                            upic.SetColor(StatusFontColorNormalString);
                            var prob = uw.HitProbability(u, true);
                            if (def_mode == "回避")
                            {
                                prob = (prob / 2);
                            }

                            var cprob = uw.CriticalProbability(u, def_mode);
                            upic.Print(GeneralLib.MinLng(prob, 100) + "％（" + cprob + "％）");
                            upic.SetColor(StatusFontColorNormalString);
                            upic.ClearGrid();
                        }

                        if (tw > 0)
                        {
                            var tuw = u.Weapon(tw);
                            // 反撃手段
                            upic.SetColor(StatusFontColorAbilityName);
                            upic.Print("反撃     ");
                            upic.PushGrid();
                            upic.SetColor(StatusFontColorNormalString);
                            upic.Print(tuw.WeaponNickname());
                            // サポートガードを受けられる？
                            if (u.LookForSupportGuard(Commands.SelectedUnit, uw) is object)
                            {
                                upic.Print(" [援]");
                            }
                            upic.ClearGrid();

                            // 予測ダメージ
                            if (!Expression.IsOptionDefined("予測ダメージ非表示"))
                            {
                                upic.SetColor(StatusFontColorAbilityName);
                                upic.Print("ダメージ ");
                                upic.PushGrid();
                                var dmg = tuw.Damage(Commands.SelectedUnit, true);
                                if (dmg >= Commands.SelectedUnit.HP)
                                {
                                    upic.SetColor(StatusFontColorWarning);
                                }
                                else
                                {
                                    upic.SetColor(StatusFontColorNormalString);
                                }
                                upic.Print(SrcFormatter.Format(dmg));
                                upic.ClearGrid();
                            }

                            // 予測命中率
                            if (!Expression.IsOptionDefined("予測命中率非表示"))
                            {
                                upic.SetColor(StatusFontColorAbilityName);
                                upic.Print("命中率   ");
                                upic.PushGrid();
                                upic.SetColor(StatusFontColorNormalString);
                                var prob = tuw.HitProbability(Commands.SelectedUnit, true);
                                var cprob = tuw.CriticalProbability(Commands.SelectedUnit);
                                upic.Print(SrcFormatter.Format(GeneralLib.MinLng(prob, 100)) + "％（" + cprob + "％）");
                                upic.ClearGrid();
                            }
                        }
                        else
                        {
                            // 相手は反撃できない
                            upic.SetColor(StatusFontColorAbilityName);
                            if (!string.IsNullOrEmpty(def_mode))
                            {
                                upic.Print(def_mode);
                            }
                            else
                            {
                                upic.Print("反撃不能");
                            }
                            upic.SetColor(StatusFontColorNormalString);
                            // サポートガードを受けられる？
                            if (u.LookForSupportGuard(Commands.SelectedUnit, uw) is object)
                            {
                                upic.Print(" [援]");
                            }
                            upic.ClearGrid();
                        }
                    }

                SkipAttackExpResult:
                    ;

                    // 武器一覧
                    {
                        upic.ClearGrid();
                        upic.Print();
                        upic.SetColor(StatusFontColorAbilityName);
                        upic.Print(Strings.Space(25) + "攻撃 射程");
                        upic.SetColor(StatusFontColorNormalString);

                        // 攻撃力, 消費EN, 残弾数でソート
                        var weapons = u.Weapons
                            .OrderBy(x => x.WeaponPower("") * 1000 * 100
                                + x.WeaponENConsumption() * 100
                                + x.Bullet())
                            .ToList();
                        // 個々の武器を表示
                        foreach (var uw in weapons)
                        {
                            // XXX 枠の外なら無視
                            //if (upic.CurrentY > 420)
                            //{
                            //    break;
                            //}

                            if (!uw.IsWeaponAvailable("ステータス"))
                            {
                                // 習得していない技は表示しない
                                if (!uw.IsWeaponMastered())
                                {
                                    goto NextWeapon;
                                }
                                // Disableコマンドで使用不可になった武器も同様
                                if (u.IsDisabled(uw.Name))
                                {
                                    goto NextWeapon;
                                }
                                // フォーメーションを満たしていない合体技も
                                if (uw.IsWeaponClassifiedAs("合"))
                                {
                                    if (!uw.IsCombinationAttackAvailable(true))
                                    {
                                        goto NextWeapon;
                                    }
                                }
                                upic.SetColor(StatusFontColorAbilityDisable);
                            }

                            // 武器の表示
                            if (uw.WeaponPower("") < 10000)
                            {
                                buf = GeneralLib.RightPaddedString(SrcFormatter.Format(uw.WeaponNickname()), 25);
                                buf = buf + GeneralLib.LeftPaddedString(SrcFormatter.Format(uw.WeaponPower("")), 4);
                            }
                            else
                            {
                                buf = GeneralLib.RightPaddedString(SrcFormatter.Format(uw.WeaponNickname()), 24);
                                buf = buf + GeneralLib.LeftPaddedString(SrcFormatter.Format(uw.WeaponPower("")), 5);
                            }

                            // 武器が特殊効果を持つ場合は略称で表記
                            if (uw.WeaponMaxRange() > 1)
                            {
                                var range = SrcFormatter.Format(uw.WeaponMinRange()) + "-" + SrcFormatter.Format(uw.WeaponMaxRange());
                                buf = buf + GeneralLib.LeftPaddedString(range, 34 - GeneralLib.StrWidth(buf));
                                // 移動後攻撃可能
                                if (uw.IsWeaponClassifiedAs("Ｐ"))
                                {
                                    buf = buf + "P";
                                }
                            }
                            else
                            {
                                buf = buf + GeneralLib.LeftPaddedString("1", 34 - GeneralLib.StrWidth(buf));
                                // 移動後攻撃不可
                                if (uw.IsWeaponClassifiedAs("Ｑ"))
                                {
                                    buf = buf + "Q";
                                }
                            }
                            // マップ攻撃
                            if (uw.IsWeaponClassifiedAs("Ｍ"))
                            {
                                buf = buf + "M";
                            }
                            // 特殊効果
                            var wclass = uw.WeaponClass();
                            buf += Strings.StrDup("+", uw.CountWeaponEffect());
                            upic.Print(buf);
                            upic.Print();
                            upic.SetColor(StatusFontColorNormalString);
                        NextWeapon:
                            ;
                        }
                    }

                    // アビリティ一覧
                    {
                        upic.ClearGrid();
                        var abilities = u.Abilities;
                        foreach (var ua in abilities)
                        {
                            // XXX 枠の外なら無視
                            //if (upic.CurrentY > 420)
                            //{
                            //    break;
                            //}

                            if (!ua.IsAbilityAvailable("ステータス"))
                            {
                                // 習得していない技は表示しない
                                if (!ua.IsAbilityMastered())
                                {
                                    goto NextAbility;
                                }
                                // Disableコマンドで使用不可になった武器も同様
                                if (u.IsDisabled(ua.Data.Name))
                                {
                                    goto NextAbility;
                                }
                                // フォーメーションを満たしていない合体技も
                                if (ua.IsAbilityClassifiedAs("合"))
                                {
                                    if (!ua.IsCombinationAbilityAvailable(true))
                                    {
                                        goto NextAbility;
                                    }
                                }
                                upic.SetColor(StatusFontColorAbilityDisable);
                            }

                            // アビリティの表示
                            // XXX 終端空白無視されてそう。どうすっかな。
                            upic.Print(GeneralLib.RightPaddedString(SrcFormatter.Format(ua.AbilityNickname()), 29));
                            if (ua.AbilityMaxRange() > 1)
                            {
                                upic.Print(GeneralLib.LeftPaddedString(SrcFormatter.Format(ua.AbilityMinRange()) + "-" + SrcFormatter.Format(ua.AbilityMaxRange()), 5));
                                if (ua.IsAbilityClassifiedAs("Ｐ"))
                                {
                                    upic.Print("P");
                                }

                                if (ua.IsAbilityClassifiedAs("Ｍ"))
                                {
                                    upic.Print("M");
                                }
                                upic.Print();
                            }
                            else if (ua.AbilityMaxRange() == 1)
                            {
                                upic.Print("    1");
                                if (ua.IsAbilityClassifiedAs("Ｑ"))
                                {
                                    upic.Print("Q");
                                }
                                if (ua.IsAbilityClassifiedAs("Ｍ"))
                                {
                                    upic.Print("M");
                                }
                                upic.Print();
                            }
                            else
                            {
                                upic.Print("    -");
                            }
                            upic.SetColor(StatusFontColorNormalString);
                        NextAbility:
                            ;
                        }
                    }

                UpdateStatusWindow:
                    ;

                    //// ステータスウィンドウをリフレッシュ
                    //GUI.MainForm.picFace.Refresh();
                    //ppic.Refresh();
                    //upic.Refresh();
                }
            }
            catch (Exception ex)
            {
                SRC.LogWarn(ex);
                //    GUI.ErrorMessage("パイロット用画像ファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "画像ファイルが壊れていないか確認して下さい。");
            }
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
