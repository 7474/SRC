using Newtonsoft.Json;
using SRCCore;
using SRCCore.Maps;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCSharpForm.Extensions;
using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
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
        // ステータス画面の背景色
        public Color StatusWindowBackBolor;
        // ステータス画面の枠色
        public Color StatusWindowFrameColor;
        // ステータス画面の枠幅
        public int StatusWindowFrameWidth;
        // ステータス画面 能力名のフォントカラー
        public Color StatusFontColorAbilityName;
        // ステータス画面 有効な能力のフォントカラー
        public Color StatusFontColorAbilityEnable;
        // ステータス画面 無効な能力のフォントカラー
        public Color StatusFontColorAbilityDisable;
        // ステータス画面 その他通常描画のフォントカラー
        public Color StatusFontColorNormalString;

        // XXX
        private TextRenderingHint StatusTextRenderingHint = TextRenderingHint.AntiAliasGridFit;
        //private Font StatusFont = new Font("Yu Gothic UI", 10f);
        //private Font StatusFont = new Font("Meiryo UI", 10f);
        private Font StatusFont = new Font("ＭＳ ゴシック", 10f);
        private Brush StatusNormalStringBrush = Brushes.Black;
        private Brush StatusAbilityNameBrush = Brushes.Blue;
        private Brush StatusAbilityEnableBrush = Brushes.Red;
        private Brush StatusAbilityDisableBrush = Brushes.Red;

        public void DisplayGlobalStatus()
        {
            // ステータスウィンドウを消去
            ClearUnitStatus();

            var sb = new StringBuilder();
            sb.AppendLine($"ターン数 {SRC.Turn}");
            sb.AppendLine($"資金 {SRC.Money}");

            var xx = GUI.PixelToMapX((int)GUI.MouseX);
            var yy = GUI.PixelToMapY((int)GUI.MouseY);
            var mapCell = Map.CellAtPoint(xx, yy);

            if (mapCell != null)
            {
                sb.AppendLine(JsonConvert.SerializeObject(mapCell.Terrain, Formatting.Indented));
            }

            picUnitStatus.NewImageIfNull();
            using (var g = Graphics.FromImage(picUnitStatus.Image))
            {
                g.TextRenderingHint = StatusTextRenderingHint;
                g.DrawString(sb.ToString(), StatusFont, StatusNormalStringBrush, 0, 0);
            }

            //var pic = picUnitStatus;
            //pic.Font = SrcFormatter.FontChangeSize(pic.Font, 12f);

            //// ADD START 240a
            //// マウスカーソルの位置は？
            //X = GUI.PixelToMapX(GUI.MouseX);
            //Y = GUI.PixelToMapY(GUI.MouseY);
            //if (GUI.NewGUIMode)
            //{
            //    // Global変数が宣言されていれば、ステータス画面用変数の同期を取る
            //    GlobalVariableLoad();
            //    pic.BackColor = ColorTranslator.FromOle(StatusWindowBackBolor);
            //    pic.DrawWidth = StatusWindowFrameWidth;
            //    color = StatusWindowFrameColor;
            //    lineStart = ((StatusWindowFrameWidth - 1) / 2d);
            //    lineEnd = ((StatusWindowFrameWidth + 1) / 2d);
            //    pic.FillStyle = vbFSTransparent;
            //    // 一旦高さを最大にする
            //    pic.Width = SrcFormatter.TwipsToPixelsX(235d);
            //    pic.Height = SrcFormatter.TwipsToPixelsY(GUI.MapPHeight - 20);
            //    wHeight = GetGlobalStatusSize(X, Y);
            //    // 枠線を引く
            //    pic.Line(lineStart, lineStart); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //    pic.FillStyle = Event_Renamed.ObjFillStyle;
            //    // 高さを設定する
            //    pic.Height = SrcFormatter.TwipsToPixelsY(wHeight);
            //    pic.CurrentX = 5;
            //    pic.CurrentY = 5;
            //    // 文字色をリセット
            //    pic.ForeColor = ColorTranslator.FromOle(StatusFontColorNormalString);
            //}
            //// ADD  END  240a
            //pic.Print("ターン数 " + SrcFormatter.Format(SRC.Turn));
            //// ADD START 240a
            //if (GUI.NewGUIMode)
            //{
            //    pic.CurrentX = 5;
            //}
            //// ADD  END  240a
            //string argtname = "資金";
            //Unit argu = null;
            //pic.Print(Expression.Term(argtname, argu, 8) + " " + SrcFormatter.Format(SRC.Money));

            //// MOV START 240a ↑に移動
            //// 'マウスカーソルの位置は？
            //// X = PixelToMapX(MouseX)
            //// Y = PixelToMapY(MouseY)
            //// MOV  END  240a

            //// マップ外をクリックした時はここで終了
            //if (X < 1 | Map.MapWidth < X | Y < 1 | Map.MapHeight < Y)
            //{
            //    pic.Font = SrcFormatter.FontChangeSize(pic.Font, 9f);
            //    if (GUI.NewGUIMode)
            //    {
            //        // 高さを設定する
            //        pic.Height = SrcFormatter.TwipsToPixelsY(wHeight);
            //    }

            //    return;
            //}

            //// 地形情報の表示
            //pic.Print();

            //// 地形名称
            //// ADD START 240a
            //// マップ画像表示
            //if (GUI.NewGUIMode)
            //{
            //    ret = GUI.BitBlt(pic.hDC, 5, 48, 32, 32, GUI.MainForm.picBack.hDC, (X - 1) * 32, (Y - 1) * 32, GUI.SRCCOPY);
            //}
            //else
            //{
            //    ret = GUI.BitBlt(pic.hDC, 0, 48, 32, 32, GUI.MainForm.picBack.hDC, (X - 1) * 32, (Y - 1) * 32, GUI.SRCCOPY);
            //}
            //pic.CurrentX = 37;
            //pic.CurrentY = 65;
            //// ADD  END  240a
            //if (Strings.InStr(Map.TerrainName(X, Y), "(") > 0)
            //{
            //    pic.Print("(" + SrcFormatter.Format(X) + "," + SrcFormatter.Format(Y) + ") " + Strings.Left(Map.TerrainName(X, Y), Strings.InStr(Map.TerrainName(X, Y), "(") - 1));
            //}
            //else
            //{
            //    pic.Print("(" + SrcFormatter.Format(X) + "," + SrcFormatter.Format(Y) + ") " + Map.TerrainName(X, Y));
            //}

            //// ADD START 240a
            //if (GUI.NewGUIMode)
            //{
            //    pic.CurrentX = 5;
            //}
            //// ADD  END  240a
            //// 命中修正
            //if (Map.TerrainEffectForHit(X, Y) >= 0)
            //{
            //    pic.Print("回避 +" + SrcFormatter.Format(Map.TerrainEffectForHit(X, Y)) + "%");
            //}
            //else
            //{
            //    pic.Print("回避 " + SrcFormatter.Format(Map.TerrainEffectForHit(X, Y)) + "%");
            //}

            //// ダメージ修正
            //if (Map.TerrainEffectForDamage(X, Y) >= 0)
            //{
            //    pic.Print("  防御 +" + SrcFormatter.Format(Map.TerrainEffectForDamage(X, Y)) + "%");
            //}
            //else
            //{
            //    pic.Print("  防御 " + SrcFormatter.Format(Map.TerrainEffectForDamage(X, Y)) + "%");
            //}

            //// ADD START 240a
            //if (GUI.NewGUIMode)
            //{
            //    pic.CurrentX = 5;
            //}
            //// ADD  END  240a
            //// ＨＰ回復率
            //if (Map.TerrainEffectForHPRecover(X, Y) > 0)
            //{
            //    string argtname1 = "ＨＰ";
            //    Unit argu1 = null;
            //    pic.Print(Expression.Term(argtname1, u: argu1) + " +" + SrcFormatter.Format(Map.TerrainEffectForHPRecover(X, Y)) + "%  ");
            //}

            //// ＥＮ回復率
            //if (Map.TerrainEffectForENRecover(X, Y) > 0)
            //{
            //    string argtname2 = "ＥＮ";
            //    Unit argu2 = null;
            //    pic.Print(Expression.Term(argtname2, u: argu2) + " +" + SrcFormatter.Format(Map.TerrainEffectForENRecover(X, Y)) + "%");
            //}

            //if (Map.TerrainEffectForHPRecover(X, Y) > 0 | Map.TerrainEffectForENRecover(X, Y) > 0)
            //{
            //    pic.Print();
            //}

            //// MOD START 240a
            //// Set td = TDList.Item(MapData(X, Y, 0))
            //// マスのタイプに応じて参照先を変更
            //switch (Map.MapData[X, Y, Map.MapDataIndex.BoxType])
            //{
            //    case Map.BoxTypes.Under:
            //    case Map.BoxTypes.UpperBmpOnly:
            //        {
            //            td = SRC.TDList.Item(Map.MapData[X, Y, Map.MapDataIndex.TerrainType]);
            //            break;
            //        }

            //    default:
            //        {
            //            td = SRC.TDList.Item(Map.MapData[X, Y, Map.MapDataIndex.LayerType]);
            //            break;
            //        }
            //}
            //// MOD  END

            //// ADD START 240a
            //if (GUI.NewGUIMode)
            //{
            //    pic.CurrentX = 5;
            //}
            //// ADD  END  240a
            //// ＨＰ＆ＥＮ減少
            //string argfname = "ＨＰ減少";
            //if (td.IsFeatureAvailable(argfname))
            //{
            //    string argtname3 = "ＨＰ";
            //    Unit argu3 = null;
            //    object argIndex1 = "ＨＰ減少";
            //    object argIndex2 = "ＨＰ減少";
            //    pic.Print(Expression.Term(argtname3, u: argu3) + " -" + SrcFormatter.Format(10d * td.FeatureLevel(argIndex1)) + "% (" + td.FeatureData(argIndex2) + ")  ");
            //}

            //string argfname1 = "ＥＮ減少";
            //if (td.IsFeatureAvailable(argfname1))
            //{
            //    string argtname4 = "ＥＮ";
            //    Unit argu4 = null;
            //    object argIndex3 = "ＥＮ減少";
            //    object argIndex4 = "ＥＮ減少";
            //    pic.Print(Expression.Term(argtname4, u: argu4) + " -" + SrcFormatter.Format(10d * td.FeatureLevel(argIndex3)) + "% (" + td.FeatureData(argIndex4) + ")  ");
            //}

            //string argfname2 = "ＨＰ減少";
            //string argfname3 = "ＥＮ減少";
            //if (td.IsFeatureAvailable(argfname2) | td.IsFeatureAvailable(argfname3))
            //{
            //    pic.Print();
            //}

            //// ADD START 240a
            //if (GUI.NewGUIMode)
            //{
            //    pic.CurrentX = 5;
            //}
            //// ADD  END  240a
            //// ＨＰ＆ＥＮ増加
            //string argfname4 = "ＨＰ増加";
            //if (td.IsFeatureAvailable(argfname4))
            //{
            //    string argtname5 = "ＨＰ";
            //    Unit argu5 = null;
            //    object argIndex5 = "ＨＰ増加";
            //    pic.Print(Expression.Term(argtname5, u: argu5) + " +" + SrcFormatter.Format(1000d * td.FeatureLevel(argIndex5)) + "  ");
            //}

            //string argfname5 = "ＥＮ増加";
            //if (td.IsFeatureAvailable(argfname5))
            //{
            //    string argtname6 = "ＥＮ";
            //    Unit argu6 = null;
            //    object argIndex6 = "ＥＮ増加";
            //    pic.Print(Expression.Term(argtname6, u: argu6) + " +" + SrcFormatter.Format(10d * td.FeatureLevel(argIndex6)) + "  ");
            //}

            //string argfname6 = "ＨＰ増加";
            //string argfname7 = "ＥＮ増加";
            //if (td.IsFeatureAvailable(argfname6) | td.IsFeatureAvailable(argfname7))
            //{
            //    pic.Print();
            //}
            //// MOD  END

            //// ADD START 240a
            //if (GUI.NewGUIMode)
            //{
            //    pic.CurrentX = 5;
            //}
            //// ADD  END  240a
            //// ＨＰ＆ＥＮ低下
            //string argfname8 = "ＨＰ低下";
            //if (td.IsFeatureAvailable(argfname8))
            //{
            //    string argtname7 = "ＨＰ";
            //    Unit argu7 = null;
            //    object argIndex7 = "ＨＰ低下";
            //    pic.Print(Expression.Term(argtname7, u: argu7) + " -" + SrcFormatter.Format(1000d * td.FeatureLevel(argIndex7)) + "  ");
            //}

            //string argfname9 = "ＥＮ低下";
            //if (td.IsFeatureAvailable(argfname9))
            //{
            //    string argtname8 = "ＥＮ";
            //    Unit argu8 = null;
            //    object argIndex8 = "ＥＮ低下";
            //    pic.Print(Expression.Term(argtname8, u: argu8) + " -" + SrcFormatter.Format(10d * td.FeatureLevel(argIndex8)) + "  ");
            //}

            //string argfname10 = "ＨＰ低下";
            //string argfname11 = "ＥＮ低下";
            //if (td.IsFeatureAvailable(argfname10) | td.IsFeatureAvailable(argfname11))
            //{
            //    pic.Print();
            //}

            //// ADD START 240a
            //if (GUI.NewGUIMode)
            //{
            //    pic.CurrentX = 5;
            //}
            //// ADD  END  240a
            //// 摩擦
            //string argfname12 = "摩擦";
            //if (td.IsFeatureAvailable(argfname12))
            //{
            //    object argIndex9 = "摩擦";
            //    pic.Print("摩擦Lv" + SrcFormatter.Format(td.FeatureLevel(argIndex9)));
            //}
            //// ADD START MARGE
            //// 状態異常付加
            //string argfname13 = "状態付加";
            //if (td.IsFeatureAvailable(argfname13))
            //{
            //    object argIndex10 = "状態付加";
            //    pic.Print(td.FeatureData(argIndex10) + "状態付加");
            //}
            //// ADD END MARGE
        }

        public void DisplayUnitStatus(Unit u, Pilot p = null)
        {
            // ステータスウィンドウを消去
            ClearUnitStatus();

            p = p ?? (u.CountPilot() > 0 ? u.MainPilot() : null);
            if (p != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine(JsonConvert.SerializeObject(p.Data, Formatting.Indented));
                picPilotStatus.NewImageIfNull();
                using (var g = Graphics.FromImage(picPilotStatus.Image))
                {
                    g.TextRenderingHint = StatusTextRenderingHint;
                    g.DrawString(sb.ToString(), StatusFont, StatusNormalStringBrush, 0, 0);
                }
                // XXX get_Bitmap
                picFace.Image = imageBuffer.Get(Path.Combine("Pilot", p.Data.Bitmap));
            }
            if (u != null)
            {
                var sb = new StringBuilder();
                sb.AppendLine(JsonConvert.SerializeObject(u.Data, Formatting.Indented));
                picUnitStatus.NewImageIfNull();
                using (var g = Graphics.FromImage(picUnitStatus.Image))
                {
                    g.TextRenderingHint = StatusTextRenderingHint;
                    var uStatus = $"HP:{u.HP}/{u.MaxHP} EN:{u.EN}/{u.MaxEN}";
                    g.DrawString(
                        uStatus +
                        Environment.NewLine +
                        sb.ToString(),
                        StatusFont,
                        StatusNormalStringBrush,
                        0,
                        0
                    );
                }
            }

            //    Pilot p;
            //    int k, i, j, n;
            //    int ret;
            //    string buf;
            //    string fdata, fname, opt;
            //    string stype, sname, slevel;
            //    int cx, cy;
            //    int[] warray;
            //    int[] wpower;
            //    PictureBox ppic, upic;
            //    int nmorale, ecost, pmorale;
            //    string[] flist;
            //    var is_unknown = default(bool);
            //    int prob, w, cprob;
            //    int dmg;
            //    string def_mode;
            //    string[] name_list;
            //    // ADD START 240a
            //    int lineStart, color, lineEnd;
            //    bool isNoSp;
            //    isNoSp = false;
            //    // ADD  END  240a
            //    // ステータス画面の更新が一時停止されている場合はそのまま終了
            //    if (IsStatusWindowDisabled)
            //    {
            //        return;
            //    }

            //    // 破壊、破棄されたユニットは表示しない
            //    if (u.Status_Renamed == "破壊" | u.Status_Renamed == "破棄")
            //    {
            //        return;
            //    }

            //    DisplayedUnit = u;
            //    DisplayedPilotInd = pindex;

            //    // MOD START MARGE
            //    // If MainWidth = 15 Then
            //    if (!GUI.NewGUIMode)
            //    {
            //        // MOD  END  MARGE
            //        ppic = GUI.MainForm.picPilotStatus;
            //        upic = GUI.MainForm.picUnitStatus;
            //        ppic.Cls();
            //        upic.Cls();
            //    }
            //    else
            //    {
            //        ppic = GUI.MainForm.picUnitStatus;
            //        upic = GUI.MainForm.picUnitStatus;
            //        upic.Cls();
            //        // ADD START 240a
            //        // global変数とステータス描画用の変数を同期
            //        GlobalVariableLoad();
            //        // 新ＧＵＩでは地形表示したときにサイズを変えているので元に戻す
            //        upic.SetBounds(SrcFormatter.TwipsToPixelsX(GUI.MainPWidth - 240), SrcFormatter.TwipsToPixelsY(10d), SrcFormatter.TwipsToPixelsX(235d), SrcFormatter.TwipsToPixelsY(GUI.MainPHeight - 20));
            //        upic.BackColor = ColorTranslator.FromOle(StatusWindowBackBolor);
            //        upic.DrawWidth = StatusWindowFrameWidth;
            //        color = StatusWindowFrameColor;
            //        lineStart = ((StatusWindowFrameWidth - 1) / 2d);
            //        lineEnd = ((StatusWindowFrameWidth + 1) / 2d);
            //        upic.FillStyle = vbFSTransparent;
            //        // 枠線を引く
            //        upic.Line(lineStart, lineStart); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        upic.FillStyle = Event_Renamed.ObjFillStyle;
            //        upic.CurrentX = 5;
            //        upic.CurrentY = 5;
            //        // 文字色をリセット
            //        upic.ForeColor = ColorTranslator.FromOle(StatusFontColorNormalString);
            //        // ADD  END
            //    }

            //    TerrainData td;
            //    string wclass;
            //    {
            //        var withBlock = u;
            //        // 情報を更新
            //        withBlock.Update();

            //        // 未確認ユニットかどうか判定しておく
            //        string argoname = "ユニット情報隠蔽";
            //        object argIndex1 = "識別済み";
            //        object argIndex2 = "ユニット情報隠蔽";
            //        if (Expression.IsOptionDefined(argoname) & !withBlock.IsConditionSatisfied(argIndex1) & (withBlock.Party0 == "敵" | withBlock.Party0 == "中立") | withBlock.IsConditionSatisfied(argIndex2))
            //        {
            //            is_unknown = true;
            //        }

            //        // パイロットが乗っていない？
            //        if (withBlock.CountPilot() == 0)
            //        {
            //            // キャラ画面をクリア
            //            if (GUI.MainWidth == 15)
            //            {
            //                GUI.MainForm.picFace = Image.FromFile("");
            //            }
            //            else
            //            {
            //                string argfname = "white.bmp";
            //                string argdraw_option = "ステータス";
            //                GUI.DrawPicture(argfname, 2, 2, 64, 64, 0, 0, 0, 0, argdraw_option);
            //            }
            //            // MOD START 240a
            //            // ppic.ForeColor = rgb(0, 0, 150)
            //            ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname = "レベル";
            //            ppic.Print(Expression.Term(argtname, u));
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname1 = "気力";
            //            ppic.Print(Expression.Term(argtname1, u));
            //            // MOD START 240a
            //            // ppic.ForeColor = rgb(0, 0, 0)
            //            ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a

            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname2 = "格闘";
            //            string argtname3 = "射撃";
            //            upic.Print(Expression.Term(argtname2, u, 4) + "               " + Expression.Term(argtname3, u));
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname4 = "命中";
            //            string argtname5 = "回避";
            //            upic.Print(Expression.Term(argtname4, u, 4) + "               " + Expression.Term(argtname5, u));
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname6 = "技量";
            //            string argtname7 = "反応";
            //            upic.Print(Expression.Term(argtname6, u, 4) + "               " + Expression.Term(argtname7, u));
            //            upic.Print();
            //            upic.Print();
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a

            //            goto UnitStatus;
            //        }

            //        // 表示するパイロットを選択
            //        if (pindex == 0)
            //        {
            //            // メインパイロット
            //            p = withBlock.MainPilot();
            //            object argIndex3 = 1;
            //            if ((withBlock.MainPilot().get_Nickname(false) ?? "") == (withBlock.Pilot(argIndex3).get_Nickname(false) ?? "") | withBlock.Data.PilotNum == 1)
            //            {
            //                DisplayedPilotInd = 1;
            //            }
            //        }
            //        else if (pindex == 1)
            //        {
            //            // メインパイロットまたは１番目のパイロット
            //            object argIndex5 = 1;
            //            if ((withBlock.MainPilot().get_Nickname(false) ?? "") != (withBlock.Pilot(argIndex5).get_Nickname(false) ?? "") & withBlock.Data.PilotNum != 1)
            //            {
            //                object argIndex4 = 1;
            //                p = withBlock.Pilot(argIndex4);
            //            }
            //            else
            //            {
            //                p = withBlock.MainPilot();
            //            }
            //        }
            //        else if (pindex <= withBlock.CountPilot())
            //        {
            //            // サブパイロット
            //            object argIndex6 = pindex;
            //            p = withBlock.Pilot(argIndex6);
            //        }
            //        else if (pindex <= (withBlock.CountPilot() + withBlock.CountSupport()))
            //        {
            //            // サポートパイロット
            //            object argIndex7 = pindex - withBlock.CountPilot();
            //            p = withBlock.Support(argIndex7);
            //        }
            //        else
            //        {
            //            // 追加サポート
            //            p = withBlock.AdditionalSupport();
            //        }
            //        // 情報を更新
            //        p.UpdateSupportMod();

            //        // パイロット画像を表示
            //        fname = @"\Bitmap\Pilot\" + p.get_Bitmap(false);
            //        if (My.MyProject.Forms.frmMultiSelectListBox.Visible)
            //        {
            //            // ザコ＆汎用パイロットが乗るユニットの出撃選択時はパイロット画像の
            //            // 代わりにユニット画像を表示
            //            if (Strings.InStr(p.Name, "(ザコ)") > 0 | Strings.InStr(p.Name, "(汎用)") > 0)
            //            {
            //                fname = @"\Bitmap\Unit\" + u.get_Bitmap(false);
            //            }
            //        }

            //        // 画像ファイルを検索
            //        bool localFileExists() { string argfname = SRC.ScenarioPath + fname; var ret = GeneralLib.FileExists(argfname); return ret; }

            //        bool localFileExists1() { string argfname = SRC.ExtDataPath + fname; var ret = GeneralLib.FileExists(argfname); return ret; }

            //        bool localFileExists2() { string argfname = SRC.ExtDataPath2 + fname; var ret = GeneralLib.FileExists(argfname); return ret; }

            //        bool localFileExists3() { string argfname = SRC.AppPath + fname; var ret = GeneralLib.FileExists(argfname); return ret; }

            //        if (Strings.InStr(fname, @"\-.bmp") > 0)
            //        {
            //            fname = "";
            //        }
            //        else if (localFileExists())
            //        {
            //            fname = SRC.ScenarioPath + fname;
            //        }
            //        else if (localFileExists1())
            //        {
            //            fname = SRC.ExtDataPath + fname;
            //        }
            //        else if (localFileExists2())
            //        {
            //            fname = SRC.ExtDataPath2 + fname;
            //        }
            //        else if (localFileExists3())
            //        {
            //            fname = SRC.AppPath + fname;
            //        }
            //        else
            //        {
            //            // 画像が見つからなかったことを記録
            //            if (Strings.InStr(fname, @"\Pilot\") > 0)
            //            {
            //                if ((p.get_Bitmap(false) ?? "") == (p.Data.Bitmap ?? ""))
            //                {
            //                    p.Data.IsBitmapMissing = true;
            //                }
            //            }

            //            fname = "";
            //        }

            //        // 画像ファイルを読み込んで表示
            //        if (GUI.MainWidth == 15)
            //        {
            //            if (!string.IsNullOrEmpty(fname))
            //            {
            //                ;
            //                GUI.MainForm.picTmp = Image.FromFile(fname);
            //                ;
            //                GUI.MainForm.picFace.PaintPicture(GUI.MainForm.picTmp.Picture, 0, 0, 64, 64);
            //            }
            //            else
            //            {
            //                // 画像ファイルが見つからなかった場合はキャラ画面をクリア
            //                GUI.MainForm.picFace = Image.FromFile("");
            //            }
            //        }
            //        else if (!string.IsNullOrEmpty(fname))
            //        {
            //            string argdraw_option1 = "ステータス";
            //            GUI.DrawPicture(fname, 2, 2, 64, 64, 0, 0, 0, 0, argdraw_option1);
            //        }
            //        else
            //        {
            //            // 画像ファイルが見つからなかった場合はキャラ画面をクリア
            //            string argfname1 = "white.bmp";
            //            string argdraw_option2 = "ステータス";
            //            GUI.DrawPicture(argfname1, 2, 2, 64, 64, 0, 0, 0, 0, argdraw_option2);
            //        }

            //        // パイロット愛称
            //        ppic.Font = SrcFormatter.FontChangeSize(ppic.Font, 10.5f);
            //        ppic.Font = SrcFormatter.FontChangeBold(ppic.Font, false);
            //        // MOD START 240a
            //        // If MainWidth <> 15 Then
            //        if (GUI.NewGUIMode)
            //        {
            //            // MOD  END
            //            ppic.CurrentX = 68;
            //        }
            //        ppic.Print(p.get_Nickname(false));
            //        ppic.Font = SrcFormatter.FontChangeBold(ppic.Font, false);
            //        ppic.Font = SrcFormatter.FontChangeSize(ppic.Font, 10f);

            //        // ダミーパイロット？
            //        if (p.Nickname0 == "パイロット不在")
            //        {
            //            // MOD START 240a
            //            // ppic.ForeColor = rgb(0, 0, 150)
            //            ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname8 = "レベル";
            //            ppic.Print(Expression.Term(argtname8, u));
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname9 = "気力";
            //            ppic.Print(Expression.Term(argtname9, u));
            //            // MOD START 240a
            //            // ppic.ForeColor = rgb(0, 0, 0)
            //            ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a

            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname10 = "格闘";
            //            string argtname11 = "射撃";
            //            upic.Print(Expression.Term(argtname10, u, 4) + "               " + Expression.Term(argtname11, u));
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname12 = "命中";
            //            string argtname13 = "回避";
            //            upic.Print(Expression.Term(argtname12, u, 4) + "               " + Expression.Term(argtname13, u));
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname14 = "技量";
            //            string argtname15 = "反応";
            //            upic.Print(Expression.Term(argtname14, u, 4) + "               " + Expression.Term(argtname15, u));
            //            upic.Print();
            //            upic.Print();
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a

            //            goto UnitStatus;
            //        }
            //        // レベル、経験値、行動回数
            //        // MOD START 240a
            //        // ppic.ForeColor = rgb(0, 0, 150)
            //        ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        // MOD START 240a
            //        // If MainWidth <> 15 Then
            //        if (GUI.NewGUIMode)
            //        {
            //            // MOD  END  240a
            //            ppic.CurrentX = 68;
            //        }
            //        string argtname16 = "レベル";
            //        ppic.Print(Expression.Term(argtname16, u) + " ");
            //        // MOD START 240a
            //        // ppic.ForeColor = rgb(0, 0, 0)
            //        ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        if (p.Party == "味方")
            //        {
            //            ppic.Print(SrcFormatter.Format(p.Level) + " (" + p.Exp + ")");
            //            switch (u.Action)
            //            {
            //                case 2:
            //                    {
            //                        // MOD START 240a
            //                        // ppic.ForeColor = rgb(0, 0, 200)
            //                        ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                        // MOD  END  240a
            //                        ppic.Print(" Ｗ");
            //                        // MOD START 240a
            //                        // ppic.ForeColor = rgb(0, 0, 0)
            //                        ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                        break;
            //                    }
            //                // MOD  END  240a
            //                case 3:
            //                    {
            //                        // MOD START 240a
            //                        // ppic.ForeColor = rgb(0, 0, 200)
            //                        ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                        // MOD  END  240a
            //                        ppic.Print(" Ｔ");
            //                        // MOD START 240a
            //                        // ppic.ForeColor = rgb(0, 0, 0)
            //                        ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                        break;
            //                    }
            //                    // MOD  END  240a
            //            }
            //        }
            //        else if (!is_unknown)
            //        {
            //            ppic.Print(SrcFormatter.Format(p.Level));
            //            if (u.Action == 2)
            //            {
            //                // MOD START 240a
            //                // ppic.ForeColor = rgb(0, 0, 200)
            //                ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                // MOD  END  240a
            //                ppic.Print(" Ｗ");
            //                // MOD START 240a
            //                // ppic.ForeColor = rgb(0, 0, 0)
            //                ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                // MOD  END  240a
            //            }
            //        }
            //        else
            //        {
            //            ppic.Print("？");
            //        }
            //        ppic.Print();

            //        // 気力
            //        // MOD START 240a
            //        // ppic.ForeColor = rgb(0, 0, 150)
            //        ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        if (GUI.MainWidth != 15)
            //        {
            //            ppic.CurrentX = 68;
            //        }
            //        string argtname17 = "気力";
            //        ppic.Print(Expression.Term(argtname17, u) + " ");
            //        // MOD START 240a
            //        // ppic.ForeColor = rgb(0, 0, 0)
            //        ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        if (!is_unknown)
            //        {
            //            if (p.MoraleMod > 0)
            //            {
            //                ppic.Print(SrcFormatter.Format(p.Morale) + "+" + SrcFormatter.Format(p.MoraleMod) + " (" + p.Personality + ")");
            //            }
            //            else
            //            {
            //                ppic.Print(SrcFormatter.Format(p.Morale) + " (" + p.Personality + ")");
            //            }
            //        }
            //        else
            //        {
            //            ppic.Print("？");
            //        }

            //        // ＳＰ
            //        if (p.MaxSP > 0)
            //        {
            //            // MOD START 240a
            //            // ppic.ForeColor = rgb(0, 0, 150)
            //            ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            if (GUI.MainWidth != 15)
            //            {
            //                ppic.CurrentX = 68;
            //            }
            //            string argtname18 = "ＳＰ";
            //            ppic.Print(Expression.Term(argtname18, u) + " ");
            //            // MOD START 240a
            //            // ppic.ForeColor = rgb(0, 0, 0)
            //            ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            if (!is_unknown)
            //            {
            //                ppic.Print(SrcFormatter.Format(p.SP) + "/" + SrcFormatter.Format(p.MaxSP));
            //            }
            //            else
            //            {
            //                ppic.Print("？");
            //            }
            //        }
            //        else
            //        {
            //            isNoSp = true;
            //        }

            //        // 使用中のスペシャルパワー一覧
            //        if (!is_unknown)
            //        {
            //            // MOD START 240a
            //            // ppic.ForeColor = rgb(0, 0, 0)
            //            ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            // MOD START 240a
            //            // If MainWidth <> 15 Then
            //            if (GUI.NewGUIMode)
            //            {
            //                // MOD  END
            //                ppic.CurrentX = 68;
            //            }
            //            ppic.Print(u.SpecialPowerInEffect());
            //        }
            //        // ADD START 240a
            //        else if (GUI.NewGUIMode)
            //        {
            //            ppic.Print(" ");
            //            // ADD  END  240a
            //        }
            //        // ADD START 240a
            //        if (isNoSp)
            //        {
            //            ppic.Print(" ");
            //        }

            //        // upicを明示的に初期化
            //        upic.Font = SrcFormatter.FontChangeBold(upic.Font, false);
            //        upic.Font = SrcFormatter.FontChangeSize(upic.Font, 9f);

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 格闘
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname19 = "格闘";
            //        upic.Print(Expression.Term(argtname19, u, 4) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        if (is_unknown)
            //        {
            //            string argbuf = "？";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf, 4) + Strings.Space(10));
            //        }
            //        else if (p.Data.Infight > 1)
            //        {
            //            switch ((p.InfightMod + p.InfightMod2))
            //            {
            //                case var @case when @case > 0:
            //                    {
            //                        string localLeftPaddedString() { string argbuf = SrcFormatter.Format(p.InfightBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString() { string argbuf = "+" + SrcFormatter.Format(p.InfightMod + p.InfightMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

            //                        upic.Print(localLeftPaddedString() + localRightPaddedString());
            //                        break;
            //                    }

            //                case var case1 when case1 < 0:
            //                    {
            //                        string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(p.InfightBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString1() { string argbuf = SrcFormatter.Format(p.InfightMod + p.InfightMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

            //                        upic.Print(localLeftPaddedString1() + localRightPaddedString1());
            //                        break;
            //                    }

            //                case 0:
            //                    {
            //                        string localLeftPaddedString2() { string argbuf = SrcFormatter.Format(p.Infight); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        upic.Print(localLeftPaddedString2() + Strings.Space(9));
            //                        break;
            //                    }
            //            }
            //        }
            //        else
            //        {
            //            string argbuf1 = "--";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf1, 5) + Strings.Space(9));
            //        }

            //        // 射撃
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        if (!p.HasMana())
            //        {
            //            string argtname20 = "射撃";
            //            upic.Print(Expression.Term(argtname20, u, 4) + " ");
            //        }
            //        else
            //        {
            //            string argtname21 = "魔力";
            //            upic.Print(Expression.Term(argtname21, u, 4) + " ");
            //        }
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        if (is_unknown)
            //        {
            //            string argbuf2 = "？";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf2, 4));
            //        }
            //        else if (p.Data.Shooting > 1)
            //        {
            //            switch ((p.ShootingMod + p.ShootingMod2))
            //            {
            //                case var case2 when case2 > 0:
            //                    {
            //                        string localLeftPaddedString3() { string argbuf = SrcFormatter.Format(p.ShootingBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString2() { string argbuf = "+" + SrcFormatter.Format(p.ShootingMod + p.ShootingMod2); var ret = GeneralLib.RightPaddedString(argbuf, 5); return ret; }

            //                        upic.Print(localLeftPaddedString3() + localRightPaddedString2());
            //                        break;
            //                    }

            //                case var case3 when case3 < 0:
            //                    {
            //                        string localLeftPaddedString4() { string argbuf = SrcFormatter.Format(p.ShootingBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString3() { string argbuf = SrcFormatter.Format(p.ShootingMod + p.ShootingMod2); var ret = GeneralLib.RightPaddedString(argbuf, 5); return ret; }

            //                        upic.Print(localLeftPaddedString4() + localRightPaddedString3());
            //                        break;
            //                    }

            //                case 0:
            //                    {
            //                        string localLeftPaddedString5() { string argbuf = SrcFormatter.Format(p.Shooting); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        upic.Print(localLeftPaddedString5() + Strings.Space(5));
            //                        break;
            //                    }
            //            }
            //        }
            //        else
            //        {
            //            string argbuf3 = "--";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf3, 5) + Strings.Space(5));
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 命中
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname22 = "命中";
            //        upic.Print(Expression.Term(argtname22, u, 4) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        if (is_unknown)
            //        {
            //            string argbuf4 = "？";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf4, 4) + Strings.Space(10));
            //        }
            //        else if (p.Data.Hit > 1)
            //        {
            //            switch ((p.HitMod + p.HitMod2))
            //            {
            //                case var case4 when case4 > 0:
            //                    {
            //                        string localLeftPaddedString6() { string argbuf = SrcFormatter.Format(p.HitBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString4() { string argbuf = "+" + SrcFormatter.Format(p.HitMod + p.HitMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

            //                        upic.Print(localLeftPaddedString6() + localRightPaddedString4());
            //                        break;
            //                    }

            //                case var case5 when case5 < 0:
            //                    {
            //                        string localLeftPaddedString7() { string argbuf = SrcFormatter.Format(p.HitBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString5() { string argbuf = SrcFormatter.Format(p.HitMod + p.HitMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

            //                        upic.Print(localLeftPaddedString7() + localRightPaddedString5());
            //                        break;
            //                    }

            //                case 0:
            //                    {
            //                        string localLeftPaddedString8() { string argbuf = SrcFormatter.Format(p.Hit); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        upic.Print(localLeftPaddedString8() + Strings.Space(9));
            //                        break;
            //                    }
            //            }
            //        }
            //        else
            //        {
            //            string argbuf5 = "--";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf5, 5) + Strings.Space(9));
            //        }

            //        // 回避
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname23 = "回避";
            //        upic.Print(Expression.Term(argtname23, u, 4) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        if (is_unknown)
            //        {
            //            string argbuf6 = "？";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf6, 4));
            //        }
            //        else if (p.Data.Dodge > 1)
            //        {
            //            switch ((p.DodgeMod + p.DodgeMod2))
            //            {
            //                case var case6 when case6 > 0:
            //                    {
            //                        string localLeftPaddedString9() { string argbuf = SrcFormatter.Format(p.DodgeBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString6() { string argbuf = "+" + SrcFormatter.Format(p.DodgeMod + p.DodgeMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

            //                        upic.Print(localLeftPaddedString9() + localRightPaddedString6());
            //                        break;
            //                    }

            //                case var case7 when case7 < 0:
            //                    {
            //                        string localLeftPaddedString10() { string argbuf = SrcFormatter.Format(p.DodgeBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString7() { string argbuf = SrcFormatter.Format(p.DodgeMod + p.DodgeMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

            //                        upic.Print(localLeftPaddedString10() + localRightPaddedString7());
            //                        break;
            //                    }

            //                case 0:
            //                    {
            //                        string localLeftPaddedString11() { string argbuf = SrcFormatter.Format(p.Dodge); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        upic.Print(localLeftPaddedString11() + Strings.Space(9));
            //                        break;
            //                    }
            //            }
            //        }
            //        else
            //        {
            //            string argbuf7 = "--";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf7, 5) + Strings.Space(9));
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 技量
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname24 = "技量";
            //        upic.Print(Expression.Term(argtname24, u, 4) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        if (is_unknown)
            //        {
            //            string argbuf8 = "？";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf8, 4) + Strings.Space(10));
            //        }
            //        else if (p.Data.Technique > 1)
            //        {
            //            switch ((p.TechniqueMod + p.TechniqueMod2))
            //            {
            //                case var case8 when case8 > 0:
            //                    {
            //                        string localLeftPaddedString12() { string argbuf = SrcFormatter.Format(p.TechniqueBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString8() { string argbuf = "+" + SrcFormatter.Format(p.TechniqueMod + p.TechniqueMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

            //                        upic.Print(localLeftPaddedString12() + localRightPaddedString8());
            //                        break;
            //                    }

            //                case var case9 when case9 < 0:
            //                    {
            //                        string localLeftPaddedString13() { string argbuf = SrcFormatter.Format(p.TechniqueBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString9() { string argbuf = SrcFormatter.Format(p.TechniqueMod + p.TechniqueMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

            //                        upic.Print(localLeftPaddedString13() + localRightPaddedString9());
            //                        break;
            //                    }

            //                case 0:
            //                    {
            //                        string localLeftPaddedString14() { string argbuf = SrcFormatter.Format(p.Technique); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        upic.Print(localLeftPaddedString14() + Strings.Space(9));
            //                        break;
            //                    }
            //            }
            //        }
            //        else
            //        {
            //            string argbuf9 = "--";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf9, 5) + Strings.Space(9));
            //        }

            //        // 反応
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname25 = "反応";
            //        upic.Print(Expression.Term(argtname25, u, 4) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        if (is_unknown)
            //        {
            //            string argbuf10 = "？";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf10, 4));
            //        }
            //        else if (p.Data.Intuition > 1)
            //        {
            //            switch ((p.IntuitionMod + p.IntuitionMod2))
            //            {
            //                case var case10 when case10 > 0:
            //                    {
            //                        string localLeftPaddedString15() { string argbuf = SrcFormatter.Format(p.IntuitionBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString10() { string argbuf = "+" + SrcFormatter.Format(p.IntuitionMod + p.IntuitionMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

            //                        upic.Print(localLeftPaddedString15() + localRightPaddedString10());
            //                        break;
            //                    }

            //                case var case11 when case11 < 0:
            //                    {
            //                        string localLeftPaddedString16() { string argbuf = SrcFormatter.Format(p.IntuitionBase); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        string localRightPaddedString11() { string argbuf = SrcFormatter.Format(p.IntuitionMod + p.IntuitionMod2); var ret = GeneralLib.RightPaddedString(argbuf, 9); return ret; }

            //                        upic.Print(localLeftPaddedString16() + localRightPaddedString11());
            //                        break;
            //                    }

            //                case 0:
            //                    {
            //                        string localLeftPaddedString17() { string argbuf = SrcFormatter.Format(p.Intuition); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                        upic.Print(localLeftPaddedString17() + Strings.Space(9));
            //                        break;
            //                    }
            //            }
            //        }
            //        else
            //        {
            //            string argbuf11 = "--";
            //            upic.Print(GeneralLib.LeftPaddedString(argbuf11, 5) + Strings.Space(9));
            //        }

            //        string argoname1 = "防御力成長";
            //        string argoname2 = "防御力レベルアップ";
            //        if (Expression.IsOptionDefined(argoname1) | Expression.IsOptionDefined(argoname2))
            //        {
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // 防御
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            string argtname26 = "防御";
            //            upic.Print(Expression.Term(argtname26, u) + " ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            if (is_unknown)
            //            {
            //                string argbuf12 = "？";
            //                upic.Print(GeneralLib.LeftPaddedString(argbuf12, 4));
            //            }
            //            else if (!p.IsSupport(u))
            //            {
            //                string localLeftPaddedString18() { string argbuf = SrcFormatter.Format(p.Defense); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                upic.Print(localLeftPaddedString18());
            //            }
            //            else
            //            {
            //                string argbuf13 = "--";
            //                upic.Print(GeneralLib.LeftPaddedString(argbuf13, 5));
            //            }
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 所有するスペシャルパワー一覧
            //        if (p.CountSpecialPower > 0)
            //        {
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            string argtname27 = "スペシャルパワー";
            //            upic.Print(Expression.Term(argtname27, u, 18) + " ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            if (!is_unknown)
            //            {
            //                var loopTo = p.CountSpecialPower;
            //                for (i = 1; i <= loopTo; i++)
            //                {
            //                    int localSpecialPowerCost() { string argsname = p.get_SpecialPower(i); var ret = p.SpecialPowerCost(argsname); p.get_SpecialPower(i) = argsname; return ret; }

            //                    if (p.SP < localSpecialPowerCost())
            //                    {
            //                        // MOD START 240a
            //                        // upic.ForeColor = rgb(150, 0, 0)
            //                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                        // MOD  END  240a

            //                    }
            //                    SpecialPowerData localItem() { object argIndex1 = p.get_SpecialPower(i); var ret = SRC.SPDList.Item(argIndex1); p.get_SpecialPower(i) = Conversions.ToString(argIndex1); return ret; }

            //                    upic.Print(localItem().intName);
            //                    // MOD START 240a
            //                    // upic.ForeColor = rgb(0, 0, 0)
            //                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                    // MOD  END  240a
            //                }
            //            }
            //            else
            //            {
            //                upic.Print("？");
            //            }
            //            upic.Print();
            //        }

            //        // 未識別のユニットはこれ以降の情報を表示しない
            //        if (is_unknown)
            //        {
            //            upic.CurrentY = upic.CurrentY + 8;
            //            goto UnitStatus;
            //        }

            //        // パイロット用特殊能力一覧
            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 霊力
            //        if (p.MaxPlana() > 0)
            //        {
            //            string argsname = "霊力";
            //            if (p.IsSkillAvailable(argsname))
            //            {
            //                object argIndex8 = "霊力";
            //                sname = p.SkillName(argIndex8);
            //            }
            //            else
            //            {
            //                // 追加パイロットは第１パイロットの霊力を代わりに使うので
            //                object argIndex9 = 1;
            //                object argIndex10 = "霊力";
            //                sname = u.Pilot(argIndex9).SkillName(argIndex10);
            //            }

            //            if (Strings.InStr(sname, "非表示") == 0)
            //            {
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(0, 0, 150)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //                // MOD  END  240a
            //                upic.Print(sname + " ");
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(0, 0, 0)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                // MOD  END  240a
            //                if (u.PlanaLevel() < p.Plana)
            //                {
            //                    // MOD START 240a
            //                    // upic.ForeColor = rgb(150, 0, 0)
            //                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                    // MOD  END  240a
            //                }
            //                upic.Print(SrcFormatter.Format(p.Plana) + "/" + SrcFormatter.Format(p.MaxPlana()));
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(0, 0, 0)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                // MOD  END  240a
            //            }
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 同調率
            //        if (p.SynchroRate() > 0)
            //        {
            //            object argIndex12 = "同調率";
            //            if (Strings.InStr(p.SkillName(argIndex12), "非表示") == 0)
            //            {
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(0, 0, 150)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //                // MOD  END  240a
            //                object argIndex11 = "同調率";
            //                upic.Print(p.SkillName(argIndex11) + " ");
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(0, 0, 0)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                // MOD  END  240a
            //                if (u.SyncLevel() < p.SynchroRate())
            //                {
            //                    // MOD START 240a
            //                    // upic.ForeColor = rgb(150, 0, 0)
            //                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                    // MOD  END  240a
            //                }
            //                upic.Print(SrcFormatter.Format(p.SynchroRate()) + "%");
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(0, 0, 0)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                // MOD  END  240a
            //            }
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 得意技＆不得手
            //        n = 0;
            //        string argsname1 = "得意技";
            //        if (p.IsSkillAvailable(argsname1))
            //        {
            //            n = (n + 1);
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("得意技 ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            string localRightPaddedString12() { object argIndex1 = "得意技"; string argbuf = p.SkillData(argIndex1); var ret = GeneralLib.RightPaddedString(argbuf, 12); return ret; }

            //            upic.Print(localRightPaddedString12());
            //        }

            //        string argsname2 = "不得手";
            //        if (p.IsSkillAvailable(argsname2))
            //        {
            //            n = (n + 1);
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("不得手 ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            object argIndex13 = "不得手";
            //            upic.Print(p.SkillData(argIndex13));
            //        }

            //        if (n > 0)
            //        {
            //            upic.Print();
            //        }

            //        // 表示するパイロット能力のリストを作成
            //        name_list = new string[(p.CountSkill() + 1)];
            //        var loopTo1 = p.CountSkill();
            //        for (i = 1; i <= loopTo1; i++)
            //        {
            //            object argIndex14 = i;
            //            name_list[i] = p.Skill(argIndex14);
            //        }
            //        // 付加されたパイロット特殊能力
            //        var loopTo2 = u.CountCondition();
            //        for (i = 1; i <= loopTo2; i++)
            //        {
            //            object argIndex15 = i;
            //            if (u.ConditionLifetime(argIndex15) != 0)
            //            {
            //                string localCondition2() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

            //                switch (Strings.Right(localCondition2(), 3) ?? "")
            //                {
            //                    case "付加２":
            //                    case "強化２":
            //                        {
            //                            string localConditionData() { object argIndex1 = i; var ret = u.ConditionData(argIndex1); return ret; }

            //                            string arglist = localConditionData();
            //                            switch (GeneralLib.LIndex(arglist, 1) ?? "")
            //                            {
            //                                // 非表示の能力
            //                                case "非表示":
            //                                case "解説":
            //                                    {
            //                                        break;
            //                                    }

            //                                default:
            //                                    {
            //                                        string localCondition() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

            //                                        string localCondition1() { object argIndex1 = i; var ret = u.Condition(argIndex1); return ret; }

            //                                        stype = Strings.Left(localCondition(), Strings.Len(localCondition1()) - 3);
            //                                        switch (stype ?? "")
            //                                        {
            //                                            case "ハンター":
            //                                            case "ＳＰ消費減少":
            //                                            case "スペシャルパワー自動発動":
            //                                                {
            //                                                    // 重複可能な能力
            //                                                    Array.Resize(name_list, Information.UBound(name_list) + 1 + 1);
            //                                                    name_list[Information.UBound(name_list)] = stype;
            //                                                    break;
            //                                                }

            //                                            default:
            //                                                {
            //                                                    // 既に所有している能力であればスキップ
            //                                                    var loopTo3 = Information.UBound(name_list);
            //                                                    for (j = 1; j <= loopTo3; j++)
            //                                                    {
            //                                                        if ((stype ?? "") == (name_list[j] ?? ""))
            //                                                        {
            //                                                            break;
            //                                                        }
            //                                                    }

            //                                                    if (j > Information.UBound(name_list))
            //                                                    {
            //                                                        Array.Resize(name_list, Information.UBound(name_list) + 1 + 1);
            //                                                        name_list[Information.UBound(name_list)] = stype;
            //                                                    }

            //                                                    break;
            //                                                }
            //                                        }

            //                                        break;
            //                                    }
            //                            }

            //                            break;
            //                        }
            //                }
            //            }
            //        }

            //        // パイロット能力を表示
            //        n = 0;
            //        var loopTo4 = Information.UBound(name_list);
            //        for (i = 1; i <= loopTo4; i++)
            //        {
            //            // ADD START 240a
            //            // 文字色をリセット
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // ADD  END  240a
            //            stype = name_list[i];
            //            if (i <= p.CountSkill())
            //            {
            //                object argIndex16 = i;
            //                sname = p.SkillName(argIndex16);
            //                double localSkillLevel() { object argIndex1 = i; string argref_mode = ""; var ret = p.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                double localSkillLevel1() { object argIndex1 = i; string argref_mode = ""; var ret = p.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                slevel = localSkillLevel1().ToString();
            //            }
            //            else
            //            {
            //                object argIndex17 = stype;
            //                sname = p.SkillName(argIndex17);
            //                double localSkillLevel2() { object argIndex1 = stype; string argref_mode = ""; var ret = p.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                double localSkillLevel3() { object argIndex1 = stype; string argref_mode = ""; var ret = p.SkillLevel(argIndex1, ref_mode: argref_mode); return ret; }

            //                slevel = localSkillLevel3().ToString();
            //            }

            //            if (Strings.InStr(sname, "非表示") > 0)
            //            {
            //                goto NextSkill;
            //            }

            //            switch (stype ?? "")
            //            {
            //                case "オーラ":
            //                    {
            //                        if (DisplayedPilotInd == 1)
            //                        {
            //                            if (u.AuraLevel(true) < u.AuraLevel() & !string.IsNullOrEmpty(Map.MapFileName))
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = rgb(150, 0, 0)
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                                // MOD  END  240a
            //                            }

            //                            if (u.AuraLevel(true) > Conversions.ToDouble(slevel))
            //                            {
            //                                sname = sname + "+" + SrcFormatter.Format(u.AuraLevel(true) - Conversions.ToDouble(slevel));
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "超能力":
            //                    {
            //                        if (DisplayedPilotInd == 1)
            //                        {
            //                            if (u.PsychicLevel(true) < u.PsychicLevel() & !string.IsNullOrEmpty(Map.MapFileName))
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = rgb(150, 0, 0)
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                                // MOD  END  240a
            //                            }

            //                            if (u.PsychicLevel(true) > Conversions.ToDouble(slevel))
            //                            {
            //                                sname = sname + "+" + SrcFormatter.Format(u.PsychicLevel(true) - Conversions.ToDouble(slevel));
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "底力":
            //                case "超底力":
            //                case "覚悟":
            //                    {
            //                        if (u.HP <= u.MaxHP / 4)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = vbBlue
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "不屈":
            //                    {
            //                        if (u.HP <= u.MaxHP / 2)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = vbBlue
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "潜在力開放":
            //                    {
            //                        if (p.Morale >= 130)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = vbBlue
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "スペシャルパワー自動発動":
            //                    {
            //                        if (i <= p.CountSkill())
            //                        {
            //                            string localSkillData() { object argIndex1 = i; var ret = p.SkillData(argIndex1); return ret; }

            //                            string localLIndex() { string arglist = hs4c6b0a8a8b874cdb8c87b94121d03278(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //                            int localStrToLng() { string argexpr = hs23c31b1421414ea0be207e3875c77613(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                            if (p.Morale >= localStrToLng())
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = vbBlue
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                                // MOD  END  240a
            //                            }
            //                        }
            //                        else
            //                        {
            //                            string localSkillData1() { object argIndex1 = stype; var ret = p.SkillData(argIndex1); return ret; }

            //                            string localLIndex1() { string arglist = hs5a6fb85244614ee5b61690cdbec6b0f9(); var ret = GeneralLib.LIndex(arglist, 3); return ret; }

            //                            int localStrToLng1() { string argexpr = hsa5a03d2c426146729603becc658cd7bc(); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                            if (p.Morale >= localStrToLng1())
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = vbBlue
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                                // MOD  END  240a
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "Ｓ防御":
            //                    {
            //                        string argfname2 = "シールド";
            //                        string argfname3 = "大型シールド";
            //                        string argfname4 = "小型シールド";
            //                        string argfname5 = "エネルギーシールド";
            //                        string argfname6 = "アクティブシールド";
            //                        string argfname7 = "盾";
            //                        string argfname8 = "バリアシールド";
            //                        string argfname9 = "アクティブフィールド";
            //                        string argfname10 = "アクティブプロテクション";
            //                        object argIndex18 = "阻止";
            //                        object argIndex19 = "広域阻止";
            //                        object argIndex20 = "反射";
            //                        object argIndex21 = "当て身技";
            //                        object argIndex22 = "自動反撃";
            //                        if (!u.IsFeatureAvailable(argfname2) & !u.IsFeatureAvailable(argfname3) & !u.IsFeatureAvailable(argfname4) & !u.IsFeatureAvailable(argfname5) & !u.IsFeatureAvailable(argfname6) & !u.IsFeatureAvailable(argfname7) & !u.IsFeatureAvailable(argfname8) & !u.IsFeatureAvailable(argfname9) & !u.IsFeatureAvailable(argfname10) & Strings.InStr(u.FeatureData(argIndex18), "Ｓ防御") == 0 & Strings.InStr(u.FeatureData(argIndex19), "Ｓ防御") == 0 & Strings.InStr(u.FeatureData(argIndex20), "Ｓ防御") == 0 & Strings.InStr(u.FeatureData(argIndex21), "Ｓ防御") == 0 & Strings.InStr(u.FeatureData(argIndex22), "Ｓ防御") == 0 & !string.IsNullOrEmpty(Map.MapFileName))
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "切り払い":
            //                    {
            //                        var loopTo5 = u.CountWeapon();
            //                        for (j = 1; j <= loopTo5; j++)
            //                        {
            //                            string argattr = "武";
            //                            if (u.IsWeaponClassifiedAs(j, argattr))
            //                            {
            //                                if (!u.IsDisabled(u.Weapon(j).Name))
            //                                {
            //                                    break;
            //                                }
            //                            }
            //                        }

            //                        string argfname11 = "格闘武器";
            //                        if (u.IsFeatureAvailable(argfname11))
            //                        {
            //                            j = 0;
            //                        }

            //                        object argIndex23 = "阻止";
            //                        object argIndex24 = "広域阻止";
            //                        object argIndex25 = "反射";
            //                        object argIndex26 = "当て身技";
            //                        object argIndex27 = "自動反撃";
            //                        if (j > u.CountWeapon() & Strings.InStr(u.FeatureData(argIndex23), "切り払い") == 0 & Strings.InStr(u.FeatureData(argIndex24), "切り払い") == 0 & Strings.InStr(u.FeatureData(argIndex25), "切り払い") == 0 & Strings.InStr(u.FeatureData(argIndex26), "切り払い") == 0 & Strings.InStr(u.FeatureData(argIndex27), "切り払い") == 0 & !string.IsNullOrEmpty(Map.MapFileName))
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "迎撃":
            //                    {
            //                        var loopTo6 = u.CountWeapon();
            //                        for (j = 1; j <= loopTo6; j++)
            //                        {
            //                            string argref_mode = "移動後";
            //                            string argattr1 = "射撃系";
            //                            if (u.IsWeaponAvailable(j, argref_mode) & u.IsWeaponClassifiedAs(j, argattr1) & (u.Weapon(j).Bullet >= 10 | u.Weapon(j).Bullet == 0 & u.Weapon(j).ENConsumption <= 5))
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        string argfname12 = "迎撃武器";
            //                        if (u.IsFeatureAvailable(argfname12))
            //                        {
            //                            j = 0;
            //                        }

            //                        object argIndex28 = "阻止";
            //                        object argIndex29 = "広域阻止";
            //                        object argIndex30 = "反射";
            //                        object argIndex31 = "当て身技";
            //                        object argIndex32 = "自動反撃";
            //                        if (j > u.CountWeapon() & Strings.InStr(u.FeatureData(argIndex28), "迎撃") == 0 & Strings.InStr(u.FeatureData(argIndex29), "迎撃") == 0 & Strings.InStr(u.FeatureData(argIndex30), "迎撃") == 0 & Strings.InStr(u.FeatureData(argIndex31), "迎撃") == 0 & Strings.InStr(u.FeatureData(argIndex32), "迎撃") == 0 & !string.IsNullOrEmpty(Map.MapFileName))
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "浄化":
            //                    {
            //                        var loopTo7 = u.CountWeapon();
            //                        for (j = 1; j <= loopTo7; j++)
            //                        {
            //                            string argattr2 = "浄";
            //                            if (u.IsWeaponClassifiedAs(j, argattr2))
            //                            {
            //                                if (!u.IsDisabled(u.Weapon(j).Name))
            //                                {
            //                                    break;
            //                                }
            //                            }
            //                        }

            //                        if (j > u.CountWeapon() & !string.IsNullOrEmpty(Map.MapFileName))
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "援護":
            //                    {
            //                        if (!string.IsNullOrEmpty(Map.MapFileName))
            //                        {
            //                            if ((u.Party ?? "") == (SRC.Stage ?? ""))
            //                            {
            //                                ret = GeneralLib.MaxLng(u.MaxSupportAttack() - u.UsedSupportAttack, 0);
            //                            }
            //                            else
            //                            {
            //                                string argsptype = "サポートガード不能";
            //                                if (u.IsUnderSpecialPowerEffect(argsptype))
            //                                {
            //                                    // MOD START 240a
            //                                    // upic.ForeColor = rgb(150, 0, 0)
            //                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                                    // MOD  END  240a
            //                                }

            //                                ret = GeneralLib.MaxLng(u.MaxSupportGuard() - u.UsedSupportGuard, 0);
            //                            }

            //                            if (ret == 0)
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = rgb(150, 0, 0)
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                                // MOD  END  240a
            //                            }

            //                            sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
            //                        }

            //                        break;
            //                    }

            //                case "援護攻撃":
            //                    {
            //                        if (!string.IsNullOrEmpty(Map.MapFileName))
            //                        {
            //                            ret = GeneralLib.MaxLng(u.MaxSupportAttack() - u.UsedSupportAttack, 0);
            //                            if (ret == 0)
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = rgb(150, 0, 0)
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                                // MOD  END  240a
            //                            }

            //                            sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
            //                        }

            //                        break;
            //                    }

            //                case "援護防御":
            //                    {
            //                        if (!string.IsNullOrEmpty(Map.MapFileName))
            //                        {
            //                            ret = GeneralLib.MaxLng(u.MaxSupportGuard() - u.UsedSupportGuard, 0);
            //                            string argsptype1 = "サポートガード不能";
            //                            if (ret == 0 | u.IsUnderSpecialPowerEffect(argsptype1))
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = rgb(150, 0, 0)
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                                // MOD  END  240a
            //                            }

            //                            sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
            //                        }

            //                        break;
            //                    }

            //                case "統率":
            //                    {
            //                        if (!string.IsNullOrEmpty(Map.MapFileName))
            //                        {
            //                            ret = GeneralLib.MaxLng(u.MaxSyncAttack() - u.UsedSyncAttack, 0);
            //                            if (ret == 0)
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = rgb(150, 0, 0)
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                                // MOD  END  240a
            //                            }

            //                            sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
            //                        }

            //                        break;
            //                    }

            //                case "カウンター":
            //                    {
            //                        if (!string.IsNullOrEmpty(Map.MapFileName))
            //                        {
            //                            ret = GeneralLib.MaxLng(u.MaxCounterAttack() - u.UsedCounterAttack, 0);
            //                            if (ret > 100)
            //                            {
            //                                sname = sname + " (残り∞回)";
            //                            }
            //                            else if (ret > 0)
            //                            {
            //                                sname = sname + " (残り" + SrcFormatter.Format(ret) + "回)";
            //                            }
            //                            else
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = rgb(150, 0, 0)
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                                // MOD  END  240a
            //                                sname = sname + " (残り0回)";
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "先手必勝":
            //                    {
            //                        if (u.MaxCounterAttack() > 100)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = vbBlue
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "耐久":
            //                    {
            //                        string argoname3 = "防御力成長";
            //                        string argoname4 = "防御力レベルアップ";
            //                        if (Expression.IsOptionDefined(argoname3) | Expression.IsOptionDefined(argoname4))
            //                        {
            //                            goto NextSkill;
            //                        }

            //                        break;
            //                    }

            //                case "霊力":
            //                case "同調率":
            //                case "得意技":
            //                case "不得手":
            //                    {
            //                        goto NextSkill;
            //                        break;
            //                    }
            //            }

            //            // 特殊能力名を表示
            //            if (LenB(Strings.StrConv(sname, vbFromUnicode)) > 19)
            //            {
            //                if (n > 0)
            //                {
            //                    upic.Print();
            //                    // ADD START 240a
            //                    if (GUI.NewGUIMode)
            //                    {
            //                        upic.CurrentX = 5;
            //                    }
            //                    // ADD  END  240a
            //                }
            //                upic.Print(sname);
            //                n = 2;
            //            }
            //            else
            //            {
            //                upic.Print(GeneralLib.RightPaddedString(sname, 19));
            //                n = (n + 1);
            //            }

            //            upic.ForeColor = Color.Black;

            //            // 必要に応じて改行
            //            if (n > 1)
            //            {
            //                upic.Print();
            //                // ADD START 240a
            //                if (GUI.NewGUIMode)
            //                {
            //                    upic.CurrentX = 5;
            //                }
            //                // ADD  END  240a
            //                n = 0;
            //            }

            //        NextSkill:
            //            ;
            //        }

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

            //        upic.CurrentY = upic.CurrentY + 8;
            //    UnitStatus:
            //        ;


            //        // パイロットステータス表示用のダミーユニットの場合はここで表示を終了
            //        string argfname13 = "ダミーユニット";
            //        if (withBlock.IsFeatureAvailable(argfname13))
            //        {
            //            goto UpdateStatusWindow;
            //        }

            //        // ここからはユニットに関する情報

            //        // ユニット愛称
            //        upic.Font = SrcFormatter.FontChangeSize(upic.Font, 10.5f);
            //        upic.Font = SrcFormatter.FontChangeBold(upic.Font, false);
            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //            // 文字色をリセット
            //            upic.ForeColor = ColorTranslator.FromOle(StatusFontColorNormalString);
            //        }
            //        // ADD  END  240a
            //        upic.Print(withBlock.Nickname0);
            //        upic.Font = SrcFormatter.FontChangeBold(upic.Font, false);
            //        upic.Font = SrcFormatter.FontChangeSize(upic.Font, 9f);
            //        if (withBlock.Status_Renamed == "出撃" & !string.IsNullOrEmpty(Map.MapFileName))
            //        {

            //            // 地形情報の表示

            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            // ユニットの位置を地形名称
            //            if (Strings.InStr(Map.TerrainName(withBlock.x, withBlock.y), "(") > 0)
            //            {
            //                upic.Print(withBlock.Area + " (" + Strings.Left(Map.TerrainName(withBlock.x, withBlock.y), Strings.InStr(Map.TerrainName(withBlock.x, withBlock.y), "(") - 1));
            //            }
            //            else
            //            {
            //                upic.Print(withBlock.Area + " (" + Map.TerrainName(withBlock.x, withBlock.y));
            //            }

            //            // 回避＆防御修正
            //            if (Map.TerrainEffectForHit(withBlock.x, withBlock.y) == Map.TerrainEffectForDamage(withBlock.x, withBlock.y))
            //            {
            //                if (Map.TerrainEffectForHit(withBlock.x, withBlock.y) >= 0)
            //                {
            //                    upic.Print(" 回＆防+" + SrcFormatter.Format(Map.TerrainEffectForHit(withBlock.x, withBlock.y)) + "%");
            //                }
            //                else
            //                {
            //                    upic.Print(" 回＆防" + SrcFormatter.Format(Map.TerrainEffectForHit(withBlock.x, withBlock.y)) + "%");
            //                }
            //            }
            //            else
            //            {
            //                if (Map.TerrainEffectForHit(withBlock.x, withBlock.y) >= 0)
            //                {
            //                    upic.Print(" 回+" + SrcFormatter.Format(Map.TerrainEffectForHit(withBlock.x, withBlock.y)) + "%");
            //                }
            //                else
            //                {
            //                    upic.Print(" 回" + SrcFormatter.Format(Map.TerrainEffectForHit(withBlock.x, withBlock.y)) + "%");
            //                }

            //                if (Map.TerrainEffectForDamage(withBlock.x, withBlock.y) >= 0)
            //                {
            //                    upic.Print(" 防+" + SrcFormatter.Format(Map.TerrainEffectForDamage(withBlock.x, withBlock.y)) + "%");
            //                }
            //                else
            //                {
            //                    upic.Print(" 防" + SrcFormatter.Format(Map.TerrainEffectForDamage(withBlock.x, withBlock.y)) + "%");
            //                }
            //            }

            //            // ＨＰ＆ＥＮ回復
            //            if (Map.TerrainEffectForHPRecover(withBlock.x, withBlock.y) > 0)
            //            {
            //                string argtname28 = "ＨＰ";
            //                Unit argu = null;
            //                upic.Print(" " + Strings.Left(Expression.Term(argtname28, u: argu), 1) + "+" + SrcFormatter.Format(Map.TerrainEffectForHPRecover(withBlock.x, withBlock.y)) + "%");
            //            }

            //            if (Map.TerrainEffectForENRecover(withBlock.x, withBlock.y) > 0)
            //            {
            //                string argtname29 = "ＥＮ";
            //                Unit argu1 = null;
            //                upic.Print(" " + Strings.Left(Expression.Term(argtname29, u: argu1), 1) + "+" + SrcFormatter.Format(Map.TerrainEffectForENRecover(withBlock.x, withBlock.y)) + "%");
            //            }

            //            // MOD START 240a
            //            // Set td = TDList.Item(MapData(.X, .Y, 0))
            //            // マスのタイプに応じて参照先を変更
            //            switch (Map.MapData[withBlock.x, withBlock.y, Map.MapDataIndex.BoxType])
            //            {
            //                case Map.BoxTypes.Under:
            //                case Map.BoxTypes.UpperBmpOnly:
            //                    {
            //                        td = SRC.TDList.Item(Map.MapData[withBlock.x, withBlock.y, Map.MapDataIndex.TerrainType]);
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        td = SRC.TDList.Item(Map.MapData[withBlock.x, withBlock.y, Map.MapDataIndex.LayerType]);
            //                        break;
            //                    }
            //            }
            //            // MOD START 240a
            //            // ＨＰ＆ＥＮ減少
            //            string argfname14 = "ＨＰ減少";
            //            if (td.IsFeatureAvailable(argfname14))
            //            {
            //                string argtname30 = "ＨＰ";
            //                Unit argu2 = null;
            //                object argIndex33 = "ＨＰ減少";
            //                upic.Print(" " + Strings.Left(Expression.Term(argtname30, u: argu2), 1) + "-" + SrcFormatter.Format(10d * td.FeatureLevel(argIndex33)) + "%");
            //            }

            //            string argfname15 = "ＥＮ減少";
            //            if (td.IsFeatureAvailable(argfname15))
            //            {
            //                string argtname31 = "ＥＮ";
            //                Unit argu3 = null;
            //                object argIndex34 = "ＥＮ減少";
            //                upic.Print(" " + Strings.Left(Expression.Term(argtname31, u: argu3), 1) + "-" + SrcFormatter.Format(10d * td.FeatureLevel(argIndex34)) + "%");
            //            }

            //            // ＨＰ＆ＥＮ増加
            //            string argfname16 = "ＨＰ増加";
            //            if (td.IsFeatureAvailable(argfname16))
            //            {
            //                string argtname32 = "ＨＰ";
            //                Unit argu4 = null;
            //                object argIndex35 = "ＨＰ増加";
            //                upic.Print(" " + Strings.Left(Expression.Term(argtname32, u: argu4), 1) + "+" + SrcFormatter.Format(1000d * td.FeatureLevel(argIndex35)));
            //            }

            //            string argfname17 = "ＥＮ増加";
            //            if (td.IsFeatureAvailable(argfname17))
            //            {
            //                string argtname33 = "ＥＮ";
            //                Unit argu5 = null;
            //                object argIndex36 = "ＥＮ増加";
            //                upic.Print(" " + Strings.Left(Expression.Term(argtname33, u: argu5), 1) + "+" + SrcFormatter.Format(10d * td.FeatureLevel(argIndex36)));
            //            }

            //            // ＨＰ＆ＥＮ低下
            //            string argfname18 = "ＨＰ低下";
            //            if (td.IsFeatureAvailable(argfname18))
            //            {
            //                string argtname34 = "ＨＰ";
            //                Unit argu6 = null;
            //                object argIndex37 = "ＨＰ低下";
            //                upic.Print(" " + Strings.Left(Expression.Term(argtname34, u: argu6), 1) + "-" + SrcFormatter.Format(1000d * td.FeatureLevel(argIndex37)));
            //            }

            //            string argfname19 = "ＥＮ低下";
            //            if (td.IsFeatureAvailable(argfname19))
            //            {
            //                string argtname35 = "ＥＮ";
            //                Unit argu7 = null;
            //                object argIndex38 = "ＥＮ低下";
            //                upic.Print(" " + Strings.Left(Expression.Term(argtname35, u: argu7), 1) + "-" + SrcFormatter.Format(10d * td.FeatureLevel(argIndex38)));
            //            }

            //            // 摩擦
            //            string argfname20 = "摩擦";
            //            if (td.IsFeatureAvailable(argfname20))
            //            {
            //                object argIndex39 = "摩擦";
            //                upic.Print(" 摩L" + SrcFormatter.Format(td.FeatureLevel(argIndex39)));
            //            }

            //            upic.Print(")");
            //        }
            //        else
            //        {
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("ランク ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print(SrcFormatter.Format(withBlock.Rank));
            //        }

            //        // 未確認ユニット？
            //        if (is_unknown)
            //        {
            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            // ＨＰ
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            string argtname36 = "ＨＰ";
            //            Unit argu8 = null;
            //            upic.Print(Expression.Term(argtname36, argu8, 6) + " ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print("?????/?????");

            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            // ＥＮ
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            string argtname37 = "ＥＮ";
            //            Unit argu9 = null;
            //            upic.Print(Expression.Term(argtname37, argu9, 6) + " ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print("???/???");

            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            // 装甲
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            string argtname38 = "装甲";
            //            Unit argu10 = null;
            //            upic.Print(Expression.Term(argtname38, argu10, 6) + " ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            string argbuf14 = "？";
            //            upic.Print(GeneralLib.RightPaddedString(argbuf14, 12));

            //            // 運動性
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            string argtname39 = "運動性";
            //            Unit argu11 = null;
            //            upic.Print(Expression.Term(argtname39, argu11, 6) + " ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print("？");

            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            // 移動タイプ
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            string argtname40 = "タイプ";
            //            Unit argu12 = null;
            //            upic.Print(Expression.Term(argtname40, argu12, 6) + " ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            string argbuf15 = "？";
            //            upic.Print(GeneralLib.RightPaddedString(argbuf15, 12));

            //            // 移動力
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            string argtname41 = "移動力";
            //            Unit argu13 = null;
            //            upic.Print(Expression.Term(argtname41, argu13, 6) + " ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print("？");

            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            // 地形適応
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("適応   ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            string argbuf16 = "？";
            //            upic.Print(GeneralLib.RightPaddedString(argbuf16, 12));

            //            // ユニットサイズ
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            string argtname42 = "サイズ";
            //            Unit argu14 = null;
            //            upic.Print(Expression.Term(argtname42, argu14, 6) + " ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print("？");

            //            // サポートアタックを得られるかどうかのみ表示
            //            if ((Commands.CommandState == "ターゲット選択" | Commands.CommandState == "移動後ターゲット選択") & (Commands.SelectedCommand == "攻撃" | Commands.SelectedCommand == "マップ攻撃") & Commands.SelectedUnit is object)
            //            {
            //                object argIndex40 = "暴走";
            //                object argIndex41 = "魅了";
            //                object argIndex42 = "憑依";
            //                if (withBlock.Party == "敵" | withBlock.Party == "中立" | withBlock.IsConditionSatisfied(argIndex40) | withBlock.IsConditionSatisfied(argIndex41) | withBlock.IsConditionSatisfied(argIndex42))
            //                {
            //                    upic.Print();

            //                    // 攻撃手段
            //                    // MOD START 240a
            //                    // upic.ForeColor = rgb(0, 0, 150)
            //                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //                    // MOD  END  240a
            //                    upic.Print("攻撃     ");
            //                    // MOD START 240a
            //                    // upic.ForeColor = rgb(0, 0, 0)
            //                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                    // MOD  END  240a
            //                    upic.Print(Commands.SelectedUnit.WeaponNickname(Commands.SelectedWeapon));
            //                    // サポートアタックを得られる？
            //                    string argattr3 = "合";
            //                    string argattr4 = "Ｍ";
            //                    if (!Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, argattr3) & !Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, argattr4))
            //                    {
            //                        if (Commands.SelectedUnit.LookForSupportAttack(u) is object)
            //                        {
            //                            upic.Print(" [援]");
            //                        }
            //                    }
            //                }
            //            }

            //            goto UpdateStatusWindow;
            //        }

            //        // 実行中の命令
            //        object argIndex46 = "混乱";
            //        object argIndex47 = "恐怖";
            //        object argIndex48 = "暴走";
            //        object argIndex49 = "狂戦士";
            //        if (withBlock.Party == "ＮＰＣ" & !withBlock.IsConditionSatisfied(argIndex46) & !withBlock.IsConditionSatisfied(argIndex47) & !withBlock.IsConditionSatisfied(argIndex48) & !withBlock.IsConditionSatisfied(argIndex49))
            //        {
            //            // 思考モードを見れば実行している命令が分かるので……
            //            buf = "";
            //            object argIndex43 = "魅了";
            //            if (withBlock.IsConditionSatisfied(argIndex43))
            //            {
            //                if (withBlock.Master is object)
            //                {
            //                    if (withBlock.Master.Party == "味方")
            //                    {
            //                        buf = withBlock.Mode;
            //                    }
            //                }
            //            }

            //            string argfname21 = "召喚ユニット";
            //            object argIndex44 = "魅了";
            //            if (withBlock.IsFeatureAvailable(argfname21) & !withBlock.IsConditionSatisfied(argIndex44))
            //            {
            //                if (withBlock.Summoner is object)
            //                {
            //                    if (withBlock.Summoner.Party == "味方")
            //                    {
            //                        buf = withBlock.Mode;
            //                    }
            //                }
            //            }

            //            bool localIsDefined() { object argIndex1 = buf; var ret = SRC.PList.IsDefined(argIndex1); return ret; }

            //            if (buf == "通常")
            //            {
            //                upic.Print("自由行動中");
            //            }
            //            else if (localIsDefined())
            //            {
            //                // 思考モードにパイロット名が指定されている場合
            //                object argIndex45 = buf;
            //                {
            //                    var withBlock1 = SRC.PList.Item(argIndex45);
            //                    if (withBlock1.Unit_Renamed is object)
            //                    {
            //                        {
            //                            var withBlock2 = withBlock1.Unit_Renamed;
            //                            if (withBlock2.Status_Renamed == "出撃")
            //                            {
            //                                upic.Print(withBlock2.Nickname + "(" + SrcFormatter.Format(withBlock2.x) + "," + SrcFormatter.Format(withBlock2.y) + ")を");
            //                                if (withBlock2.Party == "味方")
            //                                {
            //                                    upic.Print("護衛中");
            //                                }
            //                                else
            //                                {
            //                                    upic.Print("追跡中");
            //                                }
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //            else if (GeneralLib.LLength(buf) == 2)
            //            {
            //                // 思考モードに座標が指定されている場合
            //                upic.Print("(" + GeneralLib.LIndex(buf, 1) + "," + GeneralLib.LIndex(buf, 2) + ")に移動中");
            //            }
            //        }

            //        // ユニットにかかっている特殊ステータス
            //        name_list = new string[1];
            //        var loopTo8 = withBlock.CountCondition();
            //        for (i = 1; i <= loopTo8; i++)
            //        {
            //            // 時間切れ？
            //            object argIndex50 = i;
            //            if (withBlock.ConditionLifetime(argIndex50) == 0)
            //            {
            //                goto NextCondition;
            //            }

            //            // 非表示？
            //            string localConditionData1() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //            if (Strings.InStr(localConditionData1(), "非表示") > 0)
            //            {
            //                goto NextCondition;
            //            }

            //            // 解説？
            //            string localConditionData2() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //            string arglist1 = localConditionData2();
            //            if (GeneralLib.LIndex(arglist1, 1) == "解説")
            //            {
            //                goto NextCondition;
            //            }
            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            object argIndex60 = i;
            //            switch (withBlock.Condition(argIndex60) ?? "")
            //            {
            //                case "データ不明":
            //                case "形態固定":
            //                case "機体固定":
            //                case "不死身":
            //                case "無敵":
            //                case "識別済み":
            //                case "非操作":
            //                case "破壊キャンセル":
            //                case "盾ダメージ":
            //                case "能力コピー":
            //                case "メッセージ付加":
            //                case "ノーマルモード付加":
            //                case "追加パイロット付加":
            //                case "追加サポート付加":
            //                case "パイロット愛称付加":
            //                case "パイロット画像付加":
            //                case "性格変更付加":
            //                case "性別付加":
            //                case "ＢＧＭ付加":
            //                case "愛称変更付加":
            //                case "スペシャルパワー無効化":
            //                case "精神コマンド無効化":
            //                case "ユニット画像付加":
            //                case var case12 when case12 == "メッセージ付加":
            //                    {
            //                        break;
            //                    }
            //                // 非表示
            //                case "残り時間":
            //                    {
            //                        int localConditionLifetime1() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime2() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime1() & localConditionLifetime2() < 100)
            //                        {
            //                            int localConditionLifetime() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print("残り時間" + SrcFormatter.Format(localConditionLifetime()) + "ターン");
            //                        }

            //                        break;
            //                    }

            //                case "無効化付加":
            //                case "耐性付加":
            //                case "吸収付加":
            //                case "弱点付加":
            //                    {
            //                        string localConditionData3() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                        string localCondition3() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                        upic.Print(localConditionData3() + localCondition3());
            //                        int localConditionLifetime4() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime5() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime4() & localConditionLifetime5() < 100)
            //                        {
            //                            int localConditionLifetime3() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" " + SrcFormatter.Format(localConditionLifetime3()) + "T");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "特殊効果無効化付加":
            //                    {
            //                        string localConditionData4() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                        upic.Print(localConditionData4() + "無効化付加");
            //                        int localConditionLifetime7() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime8() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime7() & localConditionLifetime8() < 100)
            //                        {
            //                            int localConditionLifetime6() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime6()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "攻撃属性付加":
            //                    {
            //                        string localConditionData5() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                        string localLIndex2() { string arglist = hs755742c2c238431abd43e11d0920ad14(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                        upic.Print(localLIndex2() + "属性付加");
            //                        int localConditionLifetime10() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime11() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime10() & localConditionLifetime11() < 100)
            //                        {
            //                            int localConditionLifetime9() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime9()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "武器強化付加":
            //                    {
            //                        double localConditionLevel() { object argIndex1 = i; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                        upic.Print("武器強化Lv" + localConditionLevel() + "付加");
            //                        object argIndex51 = i;
            //                        if (!string.IsNullOrEmpty(withBlock.ConditionData(argIndex51)))
            //                        {
            //                            string localConditionData6() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                            upic.Print("(" + localConditionData6() + ")");
            //                        }

            //                        int localConditionLifetime13() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime14() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime13() & localConditionLifetime14() < 100)
            //                        {
            //                            int localConditionLifetime12() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime12()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "命中率強化付加":
            //                    {
            //                        double localConditionLevel1() { object argIndex1 = i; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                        upic.Print("命中率強化Lv" + localConditionLevel1() + "付加");
            //                        object argIndex52 = i;
            //                        if (!string.IsNullOrEmpty(withBlock.ConditionData(argIndex52)))
            //                        {
            //                            string localConditionData7() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                            upic.Print("(" + localConditionData7() + ")");
            //                        }

            //                        int localConditionLifetime16() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime17() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime16() & localConditionLifetime17() < 100)
            //                        {
            //                            int localConditionLifetime15() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime15()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "ＣＴ率強化付加":
            //                    {
            //                        double localConditionLevel2() { object argIndex1 = i; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                        upic.Print("ＣＴ率強化Lv" + localConditionLevel2() + "付加");
            //                        object argIndex53 = i;
            //                        if (!string.IsNullOrEmpty(withBlock.ConditionData(argIndex53)))
            //                        {
            //                            string localConditionData8() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                            upic.Print("(" + localConditionData8() + ")");
            //                        }

            //                        int localConditionLifetime19() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime20() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime19() & localConditionLifetime20() < 100)
            //                        {
            //                            int localConditionLifetime18() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime18()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "特殊効果発動率強化付加":
            //                    {
            //                        double localConditionLevel3() { object argIndex1 = i; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                        upic.Print("特殊効果発動率強化Lv" + localConditionLevel3() + "付加");
            //                        object argIndex54 = i;
            //                        if (!string.IsNullOrEmpty(withBlock.ConditionData(argIndex54)))
            //                        {
            //                            string localConditionData9() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                            upic.Print("(" + localConditionData9() + ")");
            //                        }

            //                        int localConditionLifetime22() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime23() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime22() & localConditionLifetime23() < 100)
            //                        {
            //                            int localConditionLifetime21() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime21()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "地形適応変更付加":
            //                    {
            //                        upic.Print("地形適応変更付加");
            //                        int localConditionLifetime25() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime26() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime25() & localConditionLifetime26() < 100)
            //                        {
            //                            int localConditionLifetime24() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime24()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "盾付加":
            //                    {
            //                        string localConditionData10() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                        string localLIndex3() { string arglist = hsba8faef602a144028d0b911086dca487(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                        upic.Print(localLIndex3() + "付加");
            //                        double localConditionLevel4() { object argIndex1 = i; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                        upic.Print("(" + SrcFormatter.Format(localConditionLevel4()) + ")");
            //                        int localConditionLifetime28() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime29() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime28() & localConditionLifetime29() < 100)
            //                        {
            //                            int localConditionLifetime27() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime27()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "ダミー破壊":
            //                    {
            //                        object argIndex55 = "ダミー";
            //                        buf = withBlock.FeatureName(argIndex55);
            //                        if (Strings.InStr(buf, "Lv") > 0)
            //                        {
            //                            buf = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
            //                        }
            //                        double localConditionLevel5() { object argIndex1 = i; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                        upic.Print(buf + Strings.StrConv(SrcFormatter.Format(localConditionLevel5()), VbStrConv.Wide) + "体破壊");
            //                        break;
            //                    }

            //                case "ダミー付加":
            //                    {
            //                        double localConditionLevel6() { object argIndex1 = i; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                        object argIndex56 = "ダミー";
            //                        upic.Print(withBlock.FeatureName(argIndex56) + "残り" + Strings.StrConv(SrcFormatter.Format(localConditionLevel6()), VbStrConv.Wide) + "体");
            //                        int localConditionLifetime31() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime32() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime31() & localConditionLifetime32() < 100)
            //                        {
            //                            int localConditionLifetime30() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime30()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "バリア発動":
            //                    {
            //                        object argIndex57 = i;
            //                        if (!string.IsNullOrEmpty(withBlock.ConditionData(argIndex57)))
            //                        {
            //                            string localConditionData11() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                            upic.Print(localConditionData11());
            //                        }
            //                        else
            //                        {
            //                            upic.Print("バリア発動");
            //                        }
            //                        int localConditionLifetime33() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime33()) + "ターン");
            //                        break;
            //                    }

            //                case "フィールド発動":
            //                    {
            //                        object argIndex58 = i;
            //                        if (!string.IsNullOrEmpty(withBlock.ConditionData(argIndex58)))
            //                        {
            //                            string localConditionData12() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                            upic.Print(localConditionData12());
            //                        }
            //                        else
            //                        {
            //                            upic.Print("フィールド発動");
            //                        }
            //                        int localConditionLifetime34() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime34()) + "ターン");
            //                        break;
            //                    }

            //                case "装甲劣化":
            //                    {
            //                        string argtname43 = "装甲";
            //                        upic.Print(Expression.Term(argtname43, u) + "劣化");
            //                        int localConditionLifetime36() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime37() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime36() & localConditionLifetime37() < 20)
            //                        {
            //                            int localConditionLifetime35() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime35()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "運動性ＵＰ":
            //                    {
            //                        string argtname44 = "運動性";
            //                        upic.Print(Expression.Term(argtname44, u) + "ＵＰ");
            //                        int localConditionLifetime39() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime40() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime39() & localConditionLifetime40() < 20)
            //                        {
            //                            int localConditionLifetime38() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime38()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "運動性ＤＯＷＮ":
            //                    {
            //                        string argtname45 = "運動性";
            //                        upic.Print(Expression.Term(argtname45, u) + "ＤＯＷＮ");
            //                        int localConditionLifetime42() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime43() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime42() & localConditionLifetime43() < 20)
            //                        {
            //                            int localConditionLifetime41() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime41()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "移動力ＵＰ":
            //                    {
            //                        string argtname46 = "移動力";
            //                        upic.Print(Expression.Term(argtname46, u) + "ＵＰ");
            //                        int localConditionLifetime45() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime46() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime45() & localConditionLifetime46() < 20)
            //                        {
            //                            int localConditionLifetime44() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime44()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                case "移動力ＤＯＷＮ":
            //                    {
            //                        string argtname47 = "移動力";
            //                        upic.Print(Expression.Term(argtname47, u) + "ＤＯＷＮ");
            //                        int localConditionLifetime48() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime49() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime48() & localConditionLifetime49() < 20)
            //                        {
            //                            int localConditionLifetime47() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime47()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        // 充填中？
            //                        string localCondition7() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                        if (Strings.Right(localCondition7(), 3) == "充填中")
            //                        {
            //                            if (withBlock.IsHero())
            //                            {
            //                                string localCondition4() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                                string localCondition5() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                                upic.Print(Strings.Left(localCondition4(), Strings.Len(localCondition5()) - 3));
            //                                upic.Print("準備中");
            //                            }
            //                            else
            //                            {
            //                                string localCondition6() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                                upic.Print(localCondition6());
            //                            }
            //                            int localConditionLifetime50() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime50()) + "ターン");
            //                            goto NextCondition;
            //                        }

            //                        // パイロット特殊能力付加＆強化による状態は表示しない
            //                        string localCondition8() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                        string localCondition9() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                        if (Strings.Right(localCondition8(), 3) == "付加２" | Strings.Right(localCondition9(), 3) == "強化２")
            //                        {
            //                            goto NextCondition;
            //                        }

            //                        string localCondition12() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                        string localConditionData15() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                        string localCondition13() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                        string localConditionData16() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                        double localConditionLevel9() { object argIndex1 = i; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                        if (Strings.Right(localCondition12(), 2) == "付加" & !string.IsNullOrEmpty(localConditionData15()))
            //                        {
            //                            string localConditionData13() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                            string arglist2 = localConditionData13();
            //                            buf = GeneralLib.LIndex(arglist2, 1) + "付加";
            //                        }
            //                        else if (Strings.Right(localCondition13(), 2) == "強化" & !string.IsNullOrEmpty(localConditionData16()))
            //                        {
            //                            // 強化アビリティ
            //                            string localConditionData14() { object argIndex1 = i; var ret = withBlock.ConditionData(argIndex1); return ret; }

            //                            string localLIndex4() { string arglist = hs68f2e5d1358d41f987f02a7379e2b562(); var ret = GeneralLib.LIndex(arglist, 1); return ret; }

            //                            double localConditionLevel7() { object argIndex1 = i; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                            buf = localLIndex4() + "強化Lv" + localConditionLevel7();
            //                        }
            //                        else if (localConditionLevel9() > 0d)
            //                        {
            //                            // 付加アビリティ(レベル指定あり)
            //                            string localCondition10() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                            string localCondition11() { object argIndex1 = i; var ret = withBlock.Condition(argIndex1); return ret; }

            //                            double localConditionLevel8() { object argIndex1 = i; var ret = withBlock.ConditionLevel(argIndex1); return ret; }

            //                            buf = Strings.Left(localCondition10(), Strings.Len(localCondition11()) - 2) + "Lv" + SrcFormatter.Format(localConditionLevel8()) + "付加";
            //                        }
            //                        else
            //                        {
            //                            // 付加アビリティ(レベル指定なし)
            //                            object argIndex59 = i;
            //                            buf = withBlock.Condition(argIndex59);
            //                        }

            //                        // エリアスされた特殊能力の付加表示がたぶらないように
            //                        var loopTo9 = Information.UBound(name_list);
            //                        for (j = 1; j <= loopTo9; j++)
            //                        {
            //                            if ((buf ?? "") == (name_list[j] ?? ""))
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        if (j <= Information.UBound(name_list))
            //                        {
            //                            goto NextCondition;
            //                        }

            //                        Array.Resize(name_list, Information.UBound(name_list) + 1 + 1);
            //                        name_list[Information.UBound(name_list)] = buf;

            //                        upic.Print(buf);
            //                        int localConditionLifetime52() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        int localConditionLifetime53() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                        if (0 < localConditionLifetime52() & localConditionLifetime53() < 20)
            //                        {
            //                            int localConditionLifetime51() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(argIndex1); return ret; }

            //                            upic.Print(" 残り" + SrcFormatter.Format(localConditionLifetime51()) + "ターン");
            //                        }
            //                        upic.Print("");
            //                        break;
            //                    }
            //            }

            //        NextCondition:
            //            ;
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // ＨＰ
            //        cx = upic.CurrentX;
            //        cy = upic.CurrentY;
            //        upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        upic.Line(117, cy + 8); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        upic.Line(118 + GUI.GauageWidth, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        if (withBlock.HP > 0)
            //        {
            //            upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        }
            //        upic.CurrentX = cx;
            //        upic.CurrentY = cy;
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname48 = "ＨＰ";
            //        upic.Print(Expression.Term(argtname48, u, 6) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        object argIndex61 = "データ不明";
            //        if (withBlock.IsConditionSatisfied(argIndex61))
            //        {
            //            upic.Print("?????/?????");
            //        }
            //        else
            //        {
            //            if (withBlock.HP < 100000)
            //            {
            //                upic.Print(SrcFormatter.Format(withBlock.HP));
            //            }
            //            else
            //            {
            //                upic.Print("?????");
            //            }
            //            upic.Print("/");
            //            if (withBlock.MaxHP < 100000)
            //            {
            //                upic.Print(SrcFormatter.Format(withBlock.MaxHP));
            //            }
            //            else
            //            {
            //                upic.Print("?????");
            //            }
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // ＥＮ
            //        cx = upic.CurrentX;
            //        cy = upic.CurrentY;
            //        upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        upic.Line(117, cy + 8); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        upic.Line(118 + GUI.GauageWidth, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        if (withBlock.EN > 0)
            //        {
            //            upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
            //        }
            //        upic.CurrentX = cx;
            //        upic.CurrentY = cy;
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname49 = "ＥＮ";
            //        upic.Print(Expression.Term(argtname49, u, 6) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        object argIndex62 = "データ不明";
            //        if (withBlock.IsConditionSatisfied(argIndex62))
            //        {
            //            upic.Print("???/???");
            //        }
            //        else
            //        {
            //            if (withBlock.EN < 1000)
            //            {
            //                upic.Print(SrcFormatter.Format(withBlock.EN));
            //            }
            //            else
            //            {
            //                upic.Print("???");
            //            }
            //            upic.Print("/");
            //            if (withBlock.MaxEN < 1000)
            //            {
            //                upic.Print(SrcFormatter.Format(withBlock.MaxEN));
            //            }
            //            else
            //            {
            //                upic.Print("???");
            //            }
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 装甲
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname50 = "装甲";
            //        upic.Print(Expression.Term(argtname50, u, 6) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        switch (withBlock.get_Armor("修正値"))
            //        {
            //            case var case13 when case13 > 0:
            //                {
            //                    string localRightPaddedString13() { string argbuf = SrcFormatter.Format(withBlock.get_Armor("基本値")) + "+" + SrcFormatter.Format(withBlock.get_Armor("修正値")); var ret = GeneralLib.RightPaddedString(argbuf, 12); return ret; }

            //                    upic.Print(localRightPaddedString13());
            //                    break;
            //                }

            //            case var case14 when case14 < 0:
            //                {
            //                    string localRightPaddedString14() { string argbuf = SrcFormatter.Format(withBlock.get_Armor("基本値")) + SrcFormatter.Format(withBlock.get_Armor("修正値")); var ret = GeneralLib.RightPaddedString(argbuf, 12); return ret; }

            //                    upic.Print(localRightPaddedString14());
            //                    break;
            //                }

            //            case 0:
            //                {
            //                    string localRightPaddedString15() { string argbuf = SrcFormatter.Format(withBlock.get_Armor("")); var ret = GeneralLib.RightPaddedString(argbuf, 12); return ret; }

            //                    upic.Print(localRightPaddedString15());
            //                    break;
            //                }
            //        }

            //        // 運動性
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname51 = "運動性";
            //        upic.Print(Expression.Term(argtname51, u, 6) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        switch (withBlock.get_Mobility("修正値"))
            //        {
            //            case var case15 when case15 > 0:
            //                {
            //                    upic.Print(SrcFormatter.Format(withBlock.get_Mobility("基本値")) + "+" + SrcFormatter.Format(withBlock.get_Mobility("修正値")));
            //                    break;
            //                }

            //            case var case16 when case16 < 0:
            //                {
            //                    upic.Print(SrcFormatter.Format(withBlock.get_Mobility("基本値")) + SrcFormatter.Format(withBlock.get_Mobility("修正値")));
            //                    break;
            //                }

            //            case 0:
            //                {
            //                    upic.Print(SrcFormatter.Format(withBlock.get_Mobility("")));
            //                    break;
            //                }
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 移動タイプ
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname52 = "タイプ";
            //        upic.Print(Expression.Term(argtname52, u, 6) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        string localRightPaddedString16() { string argbuf = withBlock.Transportation; var ret = GeneralLib.RightPaddedString(argbuf, 12); withBlock.Transportation = argbuf; return ret; }

            //        upic.Print(localRightPaddedString16());

            //        // 移動力
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname53 = "移動力";
            //        upic.Print(Expression.Term(argtname53, u, 6) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        string localLIndex5() { object argIndex1 = "テレポート"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

            //        string argfname22 = "テレポート";
            //        if (withBlock.IsFeatureAvailable(argfname22) & (withBlock.Data.Speed == 0 | localLIndex5() == "0"))
            //        {
            //            object argIndex63 = "テレポート";
            //            upic.Print(SrcFormatter.Format(withBlock.Speed + withBlock.FeatureLevel(argIndex63)));
            //        }
            //        else
            //        {
            //            upic.Print(SrcFormatter.Format(withBlock.Speed));
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 地形適応
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        upic.Print("適応   ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        for (i = 1; i <= 4; i++)
            //        {
            //            switch (withBlock.get_Adaption(i))
            //            {
            //                case 5:
            //                    {
            //                        upic.Print("S");
            //                        break;
            //                    }

            //                case 4:
            //                    {
            //                        upic.Print("A");
            //                        break;
            //                    }

            //                case 3:
            //                    {
            //                        upic.Print("B");
            //                        break;
            //                    }

            //                case 2:
            //                    {
            //                        upic.Print("C");
            //                        break;
            //                    }

            //                case 1:
            //                    {
            //                        upic.Print("D");
            //                        break;
            //                    }

            //                default:
            //                    {
            //                        upic.Print("E");
            //                        break;
            //                    }
            //            }
            //        }
            //        upic.Print(Strings.Space(8));

            //        // ユニットサイズ
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        string argtname54 = "サイズ";
            //        upic.Print(Expression.Term(argtname54, u, 6) + " ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        upic.Print(Strings.StrConv(withBlock.Size, VbStrConv.Wide));

            //        // 防御属性の表示
            //        n = 0;

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 吸収
            //        if (Strings.Len(withBlock.strAbsorb) > 0 & Strings.InStr(withBlock.strAbsorb, "非表示") == 0)
            //        {
            //            if (Strings.Len(withBlock.strAbsorb) > 5)
            //            {
            //                if (n > 0)
            //                {
            //                    upic.Print();
            //                }

            //                n = 2;
            //            }
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("吸収   ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print(GeneralLib.RightPaddedString(withBlock.strAbsorb, 12));
            //            n = (n + 1);
            //            if (n > 1)
            //            {
            //                upic.Print();
            //                // ADD START 240a
            //                if (GUI.NewGUIMode)
            //                {
            //                    upic.CurrentX = 5;
            //                }
            //                // ADD  END  240a
            //                n = 0;
            //            }
            //        }

            //        // 無効化
            //        if (Strings.Len(withBlock.strImmune) > 0 & Strings.InStr(withBlock.strImmune, "非表示") == 0)
            //        {
            //            if (Strings.Len(withBlock.strImmune) > 5)
            //            {
            //                if (n > 0)
            //                {
            //                    upic.Print();
            //                    // ADD START 240a
            //                    if (GUI.NewGUIMode)
            //                    {
            //                        upic.CurrentX = 5;
            //                    }
            //                    // ADD  END  240a
            //                }

            //                n = 2;
            //            }
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("無効化 ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print(GeneralLib.RightPaddedString(withBlock.strImmune, 12));
            //            n = (n + 1);
            //            if (n > 1)
            //            {
            //                upic.Print();
            //                // ADD START 240a
            //                if (GUI.NewGUIMode)
            //                {
            //                    upic.CurrentX = 5;
            //                }
            //                // ADD  END  240a
            //                n = 0;
            //            }
            //        }

            //        // 耐性
            //        if (Strings.Len(withBlock.strResist) > 0 & Strings.InStr(withBlock.strResist, "非表示") == 0)
            //        {
            //            if (Strings.Len(withBlock.strResist) > 5)
            //            {
            //                if (n > 0)
            //                {
            //                    upic.Print();
            //                    // ADD START 240a
            //                    if (GUI.NewGUIMode)
            //                    {
            //                        upic.CurrentX = 5;
            //                    }
            //                    // ADD  END  240a
            //                }

            //                n = 2;
            //            }

            //            if (n == 0 & GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("耐性   ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print(GeneralLib.RightPaddedString(withBlock.strResist, 12));
            //            n = (n + 1);
            //            if (n > 1)
            //            {
            //                upic.Print();
            //                // ADD START 240a
            //                if (GUI.NewGUIMode)
            //                {
            //                    upic.CurrentX = 5;
            //                }
            //                // ADD  END  240a
            //                n = 0;
            //            }
            //        }

            //        // 弱点
            //        if (Strings.Len(withBlock.strWeakness) > 0 & Strings.InStr(withBlock.strWeakness, "非表示") == 0)
            //        {
            //            if (Strings.Len(withBlock.strWeakness) > 5)
            //            {
            //                if (n > 0)
            //                {
            //                    upic.Print();
            //                    // ADD START 240a
            //                    if (GUI.NewGUIMode)
            //                    {
            //                        upic.CurrentX = 5;
            //                    }
            //                    // ADD  END  240a
            //                }

            //                n = 2;
            //            }

            //            if (n == 0 & GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("弱点   ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print(GeneralLib.RightPaddedString(withBlock.strWeakness, 12));
            //            n = (n + 1);
            //            if (n > 1)
            //            {
            //                upic.Print();
            //                // ADD START 240a
            //                if (GUI.NewGUIMode)
            //                {
            //                    upic.CurrentX = 5;
            //                }
            //                // ADD  END  240a
            //                n = 0;
            //            }
            //        }

            //        // 有効
            //        if (Strings.Len(withBlock.strEffective) > 0 & Strings.InStr(withBlock.strEffective, "非表示") == 0)
            //        {
            //            if (Strings.Len(withBlock.strEffective) > 5)
            //            {
            //                if (n > 0)
            //                {
            //                    upic.Print();
            //                    // ADD START 240a
            //                    if (GUI.NewGUIMode)
            //                    {
            //                        upic.CurrentX = 5;
            //                    }
            //                    // ADD  END  240a
            //                }

            //                n = 2;
            //            }

            //            if (n == 0 & GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("有効   ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print(GeneralLib.RightPaddedString(withBlock.strEffective, 12));
            //            n = (n + 1);
            //            if (n > 1)
            //            {
            //                upic.Print();
            //                // ADD START 240a
            //                if (GUI.NewGUIMode)
            //                {
            //                    upic.CurrentX = 5;
            //                }
            //                // ADD  END  240a
            //                n = 0;
            //            }
            //        }

            //        // 特殊効果無効化
            //        if (Strings.Len(withBlock.strSpecialEffectImmune) > 0 & Strings.InStr(withBlock.strSpecialEffectImmune, "非表示") == 0)
            //        {
            //            if (Strings.Len(withBlock.strSpecialEffectImmune) > 5)
            //            {
            //                if (n > 0)
            //                {
            //                    upic.Print();
            //                    // ADD START 240a
            //                    if (GUI.NewGUIMode)
            //                    {
            //                        upic.CurrentX = 5;
            //                    }
            //                    // ADD  END  240a
            //                }

            //                n = 2;
            //            }

            //            if (n == 0 & GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("特無効 ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print(GeneralLib.RightPaddedString(withBlock.strSpecialEffectImmune, 12));
            //            n = (n + 1);
            //            if (n > 1)
            //            {
            //                upic.Print();
            //                // ADD START 240a
            //                if (GUI.NewGUIMode)
            //                {
            //                    upic.CurrentX = 5;
            //                }
            //                // ADD  END  240a
            //                n = 0;
            //            }
            //        }

            //        // 必要に応じて改行
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

            //        n = 0;

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 武器・防具クラス
            //        flist = new string[1];
            //        string argoname5 = "アイテム交換";
            //        if (Expression.IsOptionDefined(argoname5))
            //        {
            //            string argfname23 = "武器クラス";
            //            string argfname24 = "防具クラス";
            //            if (withBlock.IsFeatureAvailable(argfname23) | withBlock.IsFeatureAvailable(argfname24))
            //            {
            //                if (GUI.NewGUIMode)
            //                {
            //                    upic.CurrentX = 5;
            //                }
            //                string argbuf17 = "武器・防具クラス";
            //                upic.Print(GeneralLib.RightPaddedString(argbuf17, 19));
            //                Array.Resize(flist, 2);
            //                flist[1] = "武器・防具クラス";
            //                n = (n + 1);
            //            }
            //        }

            //        // 特殊能力一覧を表示する前に必要気力判定のためメインパイロットの気力を参照
            //        if (withBlock.CountPilot() > 0)
            //        {
            //            pmorale = withBlock.MainPilot().Morale;
            //        }
            //        else
            //        {
            //            pmorale = 150;
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 特殊能力一覧
            //        var loopTo10 = withBlock.CountAllFeature();
            //        for (i = (withBlock.AdditionalFeaturesNum + 1); i <= loopTo10; i++)
            //        {
            //            object argIndex64 = i;
            //            fname = withBlock.AllFeatureName(argIndex64);

            //            // ユニットステータスコマンド時は通常は非表示のパーツ合体、
            //            // ノーマルモード、換装も表示
            //            if (string.IsNullOrEmpty(fname))
            //            {
            //                if (string.IsNullOrEmpty(Map.MapFileName))
            //                {
            //                    object argIndex66 = i;
            //                    switch (withBlock.AllFeature(argIndex66) ?? "")
            //                    {
            //                        case "パーツ合体":
            //                        case "ノーマルモード":
            //                            {
            //                                string localAllFeature() { object argIndex1 = i; var ret = withBlock.AllFeature(argIndex1); return ret; }

            //                                string localRightPaddedString17() { string argbuf = hs5fe6f1588051411f97aaada3678aab3c(); var ret = GeneralLib.RightPaddedString(argbuf, 19); return ret; }

            //                                upic.Print(localRightPaddedString17());
            //                                n = (n + 1);
            //                                if (n > 1)
            //                                {
            //                                    upic.Print();
            //                                    // ADD START 240a
            //                                    if (GUI.NewGUIMode)
            //                                    {
            //                                        upic.CurrentX = 5;
            //                                    }
            //                                    // ADD  END  240a
            //                                    n = 0;
            //                                }

            //                                break;
            //                            }

            //                        case "換装":
            //                            {
            //                                fname = "換装";

            //                                // エリアスで換装の名称が変更されている？
            //                                {
            //                                    var withBlock3 = SRC.ALDList;
            //                                    var loopTo11 = withBlock3.Count();
            //                                    for (j = 1; j <= loopTo11; j++)
            //                                    {
            //                                        object argIndex65 = j;
            //                                        {
            //                                            var withBlock4 = withBlock3.Item(argIndex65);
            //                                            if (withBlock4.get_AliasType(1) == "換装")
            //                                            {
            //                                                fname = withBlock4.Name;
            //                                                break;
            //                                            }
            //                                        }
            //                                    }
            //                                }

            //                                upic.Print(GeneralLib.RightPaddedString(fname, 19));
            //                                n = (n + 1);
            //                                if (n > 1)
            //                                {
            //                                    upic.Print();
            //                                    // ADD START 240a
            //                                    if (GUI.NewGUIMode)
            //                                    {
            //                                        upic.CurrentX = 5;
            //                                    }
            //                                    // ADD  END  240a
            //                                    n = 0;
            //                                }

            //                                break;
            //                            }
            //                    }
            //                }

            //                goto NextFeature;
            //            }

            //            // 既に表示しているかを判定
            //            var loopTo12 = Information.UBound(flist);
            //            for (j = 1; j <= loopTo12; j++)
            //            {
            //                if ((fname ?? "") == (flist[j] ?? ""))
            //                {
            //                    goto NextFeature;
            //                }
            //            }

            //            Array.Resize(flist, Information.UBound(flist) + 1 + 1);
            //            flist[Information.UBound(flist)] = fname;

            //            // 使用可否によって表示色を変える
            //            object argIndex67 = i;
            //            fdata = withBlock.AllFeatureData(argIndex67);
            //            object argIndex79 = i;
            //            switch (withBlock.AllFeature(argIndex79) ?? "")
            //            {
            //                case "合体":
            //                    {
            //                        bool localIsDefined1() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                        if (!localIsDefined1())
            //                        {
            //                            goto NextFeature;
            //                        }

            //                        Unit localItem1() { object argIndex1 = GeneralLib.LIndex(fdata, 2); var ret = SRC.UList.Item(argIndex1); return ret; }

            //                        object argIndex68 = "行動不能";
            //                        if (localItem1().IsConditionSatisfied(argIndex68))
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "分離":
            //                    {
            //                        k = 0;
            //                        var loopTo13 = GeneralLib.LLength(fdata);
            //                        for (j = 2; j <= loopTo13; j++)
            //                        {
            //                            bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(fdata, j); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

            //                            if (!localIsDefined2())
            //                            {
            //                                goto NextFeature;
            //                            }

            //                            Unit localItem2() { object argIndex1 = GeneralLib.LIndex(fdata, j); var ret = SRC.UList.Item(argIndex1); return ret; }

            //                            {
            //                                var withBlock5 = localItem2().Data;
            //                                string argfname25 = "召喚ユニット";
            //                                if (withBlock5.IsFeatureAvailable(argfname25))
            //                                {
            //                                    k = (k + Math.Abs(withBlock5.PilotNum));
            //                                }
            //                            }
            //                        }

            //                        if (withBlock.CountPilot() < k)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "ハイパーモード":
            //                    {
            //                        double localFeatureLevel() { object argIndex1 = i; var ret = withBlock.FeatureLevel(argIndex1); return ret; }

            //                        double localFeatureLevel1() { object argIndex1 = i; var ret = withBlock.FeatureLevel(argIndex1); return ret; }
            //                        // MOD  END  240a
            //                        object argIndex69 = "ノーマルモード付加";
            //                        if (pmorale < (10d * localFeatureLevel1()) + 100 & withBlock.HP > withBlock.MaxHP / 4)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                        }
            //                        else if (withBlock.IsConditionSatisfied(argIndex69))
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "修理装置":
            //                case "補給装置":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
            //                        {
            //                            if (withBlock.EN < Conversions.Toint(GeneralLib.LIndex(fdata, 2)))
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = rgb(150, 0, 0)
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                                // MOD  END  240a
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "テレポート":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
            //                        {
            //                            if (withBlock.EN < Conversions.Toint(GeneralLib.LIndex(fdata, 2)))
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = rgb(150, 0, 0)
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                                // MOD  END  240a
            //                            }
            //                        }
            //                        else if (withBlock.EN < 40)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "分身":
            //                    {
            //                        if (pmorale < 130)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "超回避":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 2));
            //                        }
            //                        else
            //                        {
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        if (withBlock.EN < ecost | pmorale < nmorale)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "緊急テレポート":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //                        }
            //                        else
            //                        {
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        if (withBlock.EN < ecost | pmorale < nmorale)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "エネルギーシールド":
            //                    {
            //                        if (withBlock.EN < 5)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "バリア":
            //                case "バリアシールド":
            //                case "プロテクション":
            //                case "アクティブプロテクション":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //                        }
            //                        else
            //                        {
            //                            ecost = 10;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        object argIndex70 = "バリア無効化";
            //                        if (withBlock.EN < ecost | pmorale < nmorale | withBlock.IsConditionSatisfied(argIndex70) & Strings.InStr(fdata, "バリア無効化無効") == 0)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                        }
            //                        // MOD  END  240a
            //                        else if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            var loopTo14 = GeneralLib.LLength(fdata);
            //                            for (j = 5; j <= loopTo14; j++)
            //                            {
            //                                opt = GeneralLib.LIndex(fdata, j);
            //                                if (Strings.InStr(opt, "*") > 0)
            //                                {
            //                                    opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
            //                                }

            //                                switch (opt ?? "")
            //                                {
            //                                    case "相殺":
            //                                    case "中和":
            //                                    case "近接無効":
            //                                    case "手動":
            //                                    case "能力必要":
            //                                        {
            //                                            break;
            //                                        }
            //                                    // スキップ
            //                                    case "同調率":
            //                                        {
            //                                            if (withBlock.SyncLevel() == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "霊力":
            //                                        {
            //                                            if (withBlock.PlanaLevel() == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }

            //                                    default:
            //                                        {
            //                                            if (withBlock.SkillLevel(opt) == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "フィールド":
            //                case "アクティブフィールド":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //                        }
            //                        else
            //                        {
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        object argIndex71 = "バリア無効化";
            //                        if (withBlock.EN < ecost | pmorale < nmorale | withBlock.IsConditionSatisfied(argIndex71) & Strings.InStr(fdata, "バリア無効化無効") == 0)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                        }
            //                        // MOD  END  240a
            //                        else if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            var loopTo15 = GeneralLib.LLength(fdata);
            //                            for (j = 5; j <= loopTo15; j++)
            //                            {
            //                                opt = GeneralLib.LIndex(fdata, j);
            //                                if (Strings.InStr(opt, "*") > 0)
            //                                {
            //                                    opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
            //                                }

            //                                switch (opt ?? "")
            //                                {
            //                                    case "相殺":
            //                                    case "中和":
            //                                    case "近接無効":
            //                                    case "手動":
            //                                    case "能力必要":
            //                                        {
            //                                            break;
            //                                        }
            //                                    // スキップ
            //                                    case "同調率":
            //                                        {
            //                                            if (withBlock.SyncLevel() == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "霊力":
            //                                        {
            //                                            if (withBlock.PlanaLevel() == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }

            //                                    default:
            //                                        {
            //                                            if (withBlock.SkillLevel(opt) == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "広域バリア":
            //                case "広域フィールド":
            //                case "広域プロテクション":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //                        }
            //                        else if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
            //                        {
            //                            ecost = (20 * Conversions.Toint(GeneralLib.LIndex(fdata, 2)));
            //                        }
            //                        else
            //                        {
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        object argIndex72 = "バリア無効化";
            //                        if (withBlock.EN < ecost | pmorale < nmorale | withBlock.IsConditionSatisfied(argIndex72) & Strings.InStr(fdata, "バリア無効化無効") == 0)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        fname = fname + "(範囲" + GeneralLib.LIndex(fdata, 2) + "マス)";
            //                        break;
            //                    }

            //                case "アーマー":
            //                case "レジスト":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        if (pmorale < nmorale)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                        }
            //                        // MOD  END  240a
            //                        else if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            var loopTo16 = GeneralLib.LLength(fdata);
            //                            for (j = 4; j <= loopTo16; j++)
            //                            {
            //                                opt = GeneralLib.LIndex(fdata, j);
            //                                if (Strings.InStr(opt, "*") > 0)
            //                                {
            //                                    opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
            //                                }

            //                                switch (opt ?? "")
            //                                {
            //                                    case "同調率":
            //                                        {
            //                                            if (withBlock.SyncLevel() == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "霊力":
            //                                        {
            //                                            if (withBlock.PlanaLevel() == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }

            //                                    default:
            //                                        {
            //                                            if (withBlock.SkillLevel(opt) == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "攻撃回避":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 3)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 3));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        if (pmorale < nmorale)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "反射":
            //                case "阻止":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 4)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 4));
            //                        }
            //                        else
            //                        {
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        if (withBlock.EN < ecost | pmorale < nmorale)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                        }
            //                        // MOD  END  240a
            //                        else if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            var loopTo17 = GeneralLib.LLength(fdata);
            //                            for (j = 6; j <= loopTo17; j++)
            //                            {
            //                                opt = GeneralLib.LIndex(fdata, j);
            //                                if (Strings.InStr(opt, "*") > 0)
            //                                {
            //                                    opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
            //                                }

            //                                switch (opt ?? "")
            //                                {
            //                                    case "相殺":
            //                                    case "中和":
            //                                    case "近接無効":
            //                                    case "手動":
            //                                    case "能力必要":
            //                                        {
            //                                            break;
            //                                        }
            //                                    // スキップ
            //                                    case "同調率":
            //                                        {
            //                                            if (withBlock.SyncLevel() == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "霊力":
            //                                        {
            //                                            if (withBlock.PlanaLevel() == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }

            //                                    default:
            //                                        {
            //                                            if (withBlock.SkillLevel(opt) == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "広域阻止":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //                        }
            //                        else
            //                        {
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 6));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        if (withBlock.EN < ecost | pmorale < nmorale)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        fname = fname + "(範囲" + GeneralLib.LIndex(fdata, 2) + "マス)";
            //                        break;
            //                    }

            //                case "当て身技":
            //                case "自動反撃":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //                        }
            //                        else
            //                        {
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 6));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        if (withBlock.EN < ecost | pmorale < nmorale)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                        }
            //                        // MOD  END  240a
            //                        else if (Strings.InStr(fdata, "能力必要") > 0)
            //                        {
            //                            var loopTo18 = GeneralLib.LLength(fdata);
            //                            for (j = 7; j <= loopTo18; j++)
            //                            {
            //                                opt = GeneralLib.LIndex(fdata, j);
            //                                if (Strings.InStr(opt, "*") > 0)
            //                                {
            //                                    opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
            //                                }

            //                                switch (opt ?? "")
            //                                {
            //                                    case "相殺":
            //                                    case "中和":
            //                                    case "近接無効":
            //                                    case "手動":
            //                                    case "能力必要":
            //                                        {
            //                                            break;
            //                                        }
            //                                    // スキップ
            //                                    case "同調率":
            //                                        {
            //                                            if (withBlock.SyncLevel() == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }

            //                                    case "霊力":
            //                                        {
            //                                            if (withBlock.PlanaLevel() == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }

            //                                    default:
            //                                        {
            //                                            if (withBlock.SkillLevel(opt) == 0d)
            //                                            {
            //                                                goto NextFeature;
            //                                            }

            //                                            break;
            //                                        }
            //                                }
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "ブースト":
            //                    {
            //                        if (pmorale >= 130)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = vbBlue
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "盾":
            //                    {
            //                        object argIndex73 = "盾ダメージ";
            //                        object argIndex74 = "盾";
            //                        if (withBlock.ConditionLevel(argIndex73) >= withBlock.AllFeatureLevel(argIndex74))
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        object argIndex75 = "盾";
            //                        object argIndex76 = "盾ダメージ";
            //                        object argIndex77 = "盾";
            //                        fname = fname + "(" + SrcFormatter.Format(GeneralLib.MaxLng((withBlock.AllFeatureLevel(argIndex75) - withBlock.ConditionLevel(argIndex76)), 0)) + "/" + SrcFormatter.Format(withBlock.AllFeatureLevel(argIndex77)) + ")";
            //                        break;
            //                    }

            //                case "ＨＰ回復":
            //                case "ＥＮ回復":
            //                    {
            //                        // MOD START MARGE
            //                        // If .IsConditionSatisfied("回復不能") Then
            //                        object argIndex78 = "回復不能";
            //                        string argsname3 = "回復不能";
            //                        if (withBlock.IsConditionSatisfied(argIndex78) | withBlock.IsSpecialPowerInEffect(argsname3))
            //                        {
            //                            // MOD END MARGE
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }

            //                case "格闘強化":
            //                case "射撃強化":
            //                case "命中強化":
            //                case "回避強化":
            //                case "技量強化":
            //                case "反応強化":
            //                case "ＨＰ強化":
            //                case "ＥＮ強化":
            //                case "装甲強化":
            //                case "運動性強化":
            //                case "移動力強化":
            //                case "ＨＰ割合強化":
            //                case "ＥＮ割合強化":
            //                case "装甲割合強化":
            //                case "運動性割合強化":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 2)))
            //                        {
            //                            int localStrToLng2() { string argexpr = GeneralLib.LIndex(fdata, 2); var ret = GeneralLib.StrToLng(argexpr); return ret; }

            //                            if (pmorale >= localStrToLng2())
            //                            {
            //                                // MOD START 240a
            //                                // upic.ForeColor = vbBlue
            //                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
            //                                // MOD  END  240a
            //                            }
            //                        }

            //                        break;
            //                    }

            //                case "ＺＯＣ":
            //                    {
            //                        if (GeneralLib.LLength(fdata) < 2)
            //                        {
            //                            j = 1;
            //                        }
            //                        else
            //                        {
            //                            j = Conversions.Toint(GeneralLib.LIndex(fdata, 2));
            //                        }

            //                        if (j >= 1)
            //                        {
            //                            string args3 = " ";
            //                            GeneralLib.ReplaceString(fdata, Constants.vbTab, args3);
            //                            if (Strings.InStr(fdata, " 直線") > 0 | Strings.InStr(fdata, " 垂直") > 0 & Strings.InStr(fdata, " 水平") > 0)
            //                            {
            //                                buf = "直線";
            //                            }
            //                            else if (Strings.InStr(fdata, " 垂直") > 0)
            //                            {
            //                                buf = "上下";
            //                            }
            //                            else if (Strings.InStr(fdata, " 水平") > 0)
            //                            {
            //                                buf = "左右";
            //                            }
            //                            else
            //                            {
            //                                buf = "範囲";
            //                            }

            //                            fname = fname + "(" + buf + SrcFormatter.Format(j) + "マス)";
            //                        }

            //                        break;
            //                    }

            //                case "広域ＺＯＣ無効化":
            //                    {
            //                        fname = fname + "(範囲" + GeneralLib.LIndex(fdata, 2) + "マス)";
            //                        break;
            //                    }

            //                case "追加攻撃":
            //                    {
            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 5)))
            //                        {
            //                            ecost = Conversions.Toint(GeneralLib.LIndex(fdata, 5));
            //                        }
            //                        else
            //                        {
            //                            ecost = 0;
            //                        }

            //                        if (Information.IsNumeric(GeneralLib.LIndex(fdata, 6)))
            //                        {
            //                            nmorale = Conversions.Toint(GeneralLib.LIndex(fdata, 6));
            //                        }
            //                        else
            //                        {
            //                            nmorale = 0;
            //                        }

            //                        if (withBlock.EN < ecost | pmorale < nmorale)
            //                        {
            //                            // MOD START 240a
            //                            // upic.ForeColor = rgb(150, 0, 0)
            //                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                            // MOD  END  240a
            //                        }

            //                        break;
            //                    }
            //            }

            //            // 必要条件を満たさない特殊能力は赤色で表示
            //            bool localIsFeatureActivated() { object argIndex1 = i; var ret = withBlock.IsFeatureActivated(argIndex1); return ret; }

            //            if (!localIsFeatureActivated())
            //            {
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(150, 0, 0)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                // MOD  END  240a
            //            }

            //            // 特殊能力名を表示
            //            if (LenB(Strings.StrConv(fname, vbFromUnicode)) > 19)
            //            {
            //                if (n > 0)
            //                {
            //                    upic.Print();
            //                    // ADD START 240a
            //                    if (GUI.NewGUIMode)
            //                    {
            //                        upic.CurrentX = 5;
            //                    }
            //                    // ADD  END  240a
            //                }
            //                upic.Print(fname);
            //                n = 2;
            //            }
            //            else
            //            {
            //                upic.Print(GeneralLib.RightPaddedString(fname, 19));
            //                n = (n + 1);
            //            }

            //            // 必要に応じて改行
            //            if (n > 1)
            //            {
            //                upic.Print();
            //                // ADD START 240a
            //                if (GUI.NewGUIMode)
            //                {
            //                    upic.CurrentX = 5;
            //                }
            //                // ADD  END  240a
            //                n = 0;
            //            }

            //            // 表示色を戻しておく
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        NextFeature:
            //            ;
            //        }

            //        if (n > 0)
            //        {
            //            upic.Print();
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // アイテム一覧
            //        if (withBlock.CountItem() > 0)
            //        {
            //            j = 0;
            //            var loopTo19 = withBlock.CountItem();
            //            for (i = 1; i <= loopTo19; i++)
            //            {
            //                object argIndex80 = i;
            //                {
            //                    var withBlock6 = withBlock.Item(argIndex80);
            //                    // 表示指定を持つアイテムのみ表示する
            //                    string argfname26 = "表示";
            //                    if (!withBlock6.IsFeatureAvailable(argfname26))
            //                    {
            //                        goto NextItem;
            //                    }

            //                    // アイテム名を表示
            //                    if (Strings.Len(withBlock6.Nickname()) > 9)
            //                    {
            //                        if (j == 1)
            //                        {
            //                            upic.Print();
            //                            // ADD START 240a
            //                            if (GUI.NewGUIMode)
            //                            {
            //                                upic.CurrentX = 5;
            //                            }
            //                            // ADD  END  240a
            //                        }
            //                        upic.Print(withBlock6.Nickname());
            //                        j = 2;
            //                    }
            //                    else
            //                    {
            //                        upic.Print(GeneralLib.RightPaddedString(withBlock6.Nickname(), 19));
            //                        j = (j + 1);
            //                    }

            //                    if (j == 2)
            //                    {
            //                        upic.Print();
            //                        // ADD START 240a
            //                        if (GUI.NewGUIMode)
            //                        {
            //                            upic.CurrentX = 5;
            //                        }
            //                        // ADD  END  240a
            //                        j = 0;
            //                    }
            //                }

            //            NextItem:
            //                ;
            //            }

            //            if (j > 0)
            //            {
            //                upic.Print();
            //                // ADD START 240a
            //                if (GUI.NewGUIMode)
            //                {
            //                    upic.CurrentX = 5;
            //                }
            //                // ADD  END  240a
            //            }
            //        }

            //        // ターゲット選択時の攻撃結果予想表示

            //        // 攻撃時にのみ表示
            //        if ((Commands.CommandState == "ターゲット選択" | Commands.CommandState == "移動後ターゲット選択") & (Commands.SelectedCommand == "攻撃" | Commands.SelectedCommand == "マップ攻撃") & Commands.SelectedUnit is object & Commands.SelectedWeapon > 0 & SRC.Stage != "プロローグ" & SRC.Stage != "エピローグ")
            //        {
            //        }
            //        // 攻撃時と判定
            //        else
            //        {
            //            goto SkipAttackExpResult;
            //        }

            //        // 相手が敵の場合にのみ表示
            //        object argIndex81 = "暴走";
            //        object argIndex82 = "魅了";
            //        object argIndex83 = "憑依";
            //        object argIndex84 = "混乱";
            //        object argIndex85 = "睡眠";
            //        if (withBlock.Party != "敵" & withBlock.Party != "中立" & !withBlock.IsConditionSatisfied(argIndex81) & !withBlock.IsConditionSatisfied(argIndex82) & !withBlock.IsConditionSatisfied(argIndex83) & !withBlock.IsConditionSatisfied(argIndex84) & !withBlock.IsConditionSatisfied(argIndex85))
            //        {
            //            goto SkipAttackExpResult;
            //        }

            //        upic.Print();

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 攻撃手段
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        upic.Print("攻撃     ");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        upic.Print(Commands.SelectedUnit.WeaponNickname(Commands.SelectedWeapon));
            //        // サポートアタックを得られる？
            //        string argattr5 = "合";
            //        string argattr6 = "Ｍ";
            //        if (!Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, argattr5) & !Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, argattr6) & Commands.UseSupportAttack)
            //        {
            //            if (Commands.SelectedUnit.LookForSupportAttack(u) is object)
            //            {
            //                upic.Print(" [援]");
            //            }
            //            else
            //            {
            //                upic.Print();
            //            }
            //        }
            //        else
            //        {
            //            upic.Print();
            //        }

            //        // 反撃を受ける？
            //        string argattr7 = "Ｍ";
            //        string argattr8 = "間";
            //        if (withBlock.MaxAction() == 0 | Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, argattr7) | Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, argattr8))
            //        {
            //            w = 0;
            //        }
            //        else
            //        {
            //            string argamode = "反撃";
            //            int argmax_prob = 0;
            //            int argmax_dmg = 0;
            //            w = COM.SelectWeapon(u, Commands.SelectedUnit, argamode, max_prob: argmax_prob, max_dmg: argmax_dmg);
            //        }

            //        // 敵の防御行動を設定
            //        // UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            //        def_mode = Conversions.ToString(COM.SelectDefense(Commands.SelectedUnit, Commands.SelectedWeapon, u, w));
            //        if (!string.IsNullOrEmpty(def_mode))
            //        {
            //            w = 0;
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 予測ダメージ
            //        string argoname6 = "予測ダメージ非表示";
            //        if (!Expression.IsOptionDefined(argoname6))
            //        {
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("ダメージ ");
            //            dmg = Commands.SelectedUnit.Damage(Commands.SelectedWeapon, u, true);
            //            if (def_mode == "防御")
            //            {
            //                dmg = dmg / 2;
            //            }

            //            object argIndex86 = "データ不明";
            //            if (dmg >= withBlock.HP & !withBlock.IsConditionSatisfied(argIndex86))
            //            {
            //                upic.ForeColor = ColorTranslator.FromOle(Information.RGB(190, 0, 0));
            //            }
            //            else
            //            {
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(0, 0, 0)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                // MOD  END  240a
            //            }
            //            upic.Print(SrcFormatter.Format(dmg));
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 予測命中率
            //        string argoname7 = "予測命中率非表示";
            //        if (!Expression.IsOptionDefined(argoname7))
            //        {
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("命中率   ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            prob = Commands.SelectedUnit.HitProbability(Commands.SelectedWeapon, u, true);
            //            if (def_mode == "回避")
            //            {
            //                prob = (prob / 2);
            //            }

            //            cprob = Commands.SelectedUnit.CriticalProbability(Commands.SelectedWeapon, u, def_mode);
            //            upic.Print(GeneralLib.MinLng(prob, 100) + "％（" + cprob + "％）");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //        }

            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        if (w > 0)
            //        {
            //            // 反撃手段
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            upic.Print("反撃     ");
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            upic.Print(withBlock.WeaponNickname(w));
            //            // サポートガードを受けられる？
            //            if (u.LookForSupportGuard(Commands.SelectedUnit, Commands.SelectedWeapon) is object)
            //            {
            //                upic.Print(" [援]");
            //            }
            //            else
            //            {
            //                upic.Print();
            //            }

            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            // 予測ダメージ
            //            string argoname8 = "予測ダメージ非表示";
            //            if (!Expression.IsOptionDefined(argoname8))
            //            {
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(0, 0, 150)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //                // MOD  END  240a
            //                upic.Print("ダメージ ");
            //                dmg = withBlock.Damage(w, Commands.SelectedUnit, true);
            //                if (dmg >= Commands.SelectedUnit.HP)
            //                {
            //                    upic.ForeColor = ColorTranslator.FromOle(Information.RGB(190, 0, 0));
            //                }
            //                else
            //                {
            //                    // MOD START 240a
            //                    // upic.ForeColor = rgb(0, 0, 0)
            //                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                    // MOD  END  240a
            //                }
            //                upic.Print(SrcFormatter.Format(dmg));
            //            }

            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            // 予測命中率
            //            string argoname9 = "予測命中率非表示";
            //            if (!Expression.IsOptionDefined(argoname9))
            //            {
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(0, 0, 150)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //                // MOD  END  240a
            //                upic.Print("命中率   ");
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(0, 0, 0)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //                // MOD  END  240a
            //                prob = withBlock.HitProbability(w, Commands.SelectedUnit, true);
            //                cprob = withBlock.CriticalProbability(w, Commands.SelectedUnit);
            //                upic.Print(SrcFormatter.Format(GeneralLib.MinLng(prob, 100)) + "％（" + cprob + "％）");
            //            }
            //        }
            //        else
            //        {
            //            // 相手は反撃できない
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 150)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //            // MOD  END  240a
            //            if (!string.IsNullOrEmpty(def_mode))
            //            {
            //                upic.Print(def_mode);
            //            }
            //            else
            //            {
            //                upic.Print("反撃不能");
            //            }
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //            // MOD  END  240a
            //            // サポートガードを受けられる？
            //            if (u.LookForSupportGuard(Commands.SelectedUnit, Commands.SelectedWeapon) is object)
            //            {
            //                upic.Print(" [援]");
            //            }
            //            else
            //            {
            //                upic.Print();
            //            }
            //        }

            //    SkipAttackExpResult:
            //        ;


            //        // ADD START 240a
            //        if (GUI.NewGUIMode)
            //        {
            //            upic.CurrentX = 5;
            //        }
            //        // ADD  END  240a
            //        // 武器一覧
            //        upic.CurrentY = upic.CurrentY + 8;
            //        upic.Print(Strings.Space(25));
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 150)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
            //        // MOD  END  240a
            //        upic.Print("攻撃 射程");
            //        // MOD START 240a
            //        // upic.ForeColor = rgb(0, 0, 0)
            //        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a

            //        warray = new int[(withBlock.CountWeapon() + 1)];
            //        wpower = new int[(withBlock.CountWeapon() + 1)];
            //        var loopTo20 = withBlock.CountWeapon();
            //        for (i = 1; i <= loopTo20; i++)
            //        {
            //            string argtarea = "";
            //            wpower[i] = withBlock.WeaponPower(i, argtarea);
            //        }

            //        // 攻撃力でソート
            //        var loopTo21 = withBlock.CountWeapon();
            //        for (i = 1; i <= loopTo21; i++)
            //        {
            //            var loopTo22 = (i - 1);
            //            for (j = 1; j <= loopTo22; j++)
            //            {
            //                if (wpower[i] > wpower[warray[i - j]])
            //                {
            //                    break;
            //                }
            //                else if (wpower[i] == wpower[warray[i - j]])
            //                {
            //                    if (withBlock.Weapon(i).ENConsumption > 0)
            //                    {
            //                        if (withBlock.Weapon(i).ENConsumption >= withBlock.Weapon(warray[i - j]).ENConsumption)
            //                        {
            //                            break;
            //                        }
            //                    }
            //                    else if (withBlock.Weapon(i).Bullet > 0)
            //                    {
            //                        if (withBlock.Weapon(i).Bullet <= withBlock.Weapon(warray[i - j]).Bullet)
            //                        {
            //                            break;
            //                        }
            //                    }
            //                    else if (withBlock.Weapon((i - j)).ENConsumption == 0 & withBlock.Weapon(warray[i - j]).Bullet == 0)
            //                    {
            //                        break;
            //                    }
            //                }
            //            }

            //            var loopTo23 = (j - 1);
            //            for (k = 1; k <= loopTo23; k++)
            //                warray[i - k + 1] = warray[i - k];
            //            warray[i - j + 1] = i;
            //        }

            //        // 個々の武器を表示
            //        var loopTo24 = withBlock.CountWeapon();
            //        for (i = 1; i <= loopTo24; i++)
            //        {
            //            if (upic.CurrentY > 420)
            //            {
            //                break;
            //            }

            //            w = warray[i];
            //            string argref_mode1 = "ステータス";
            //            if (!withBlock.IsWeaponAvailable(w, argref_mode1))
            //            {
            //                // 習得していない技は表示しない
            //                if (!withBlock.IsWeaponMastered(w))
            //                {
            //                    goto NextWeapon;
            //                }
            //                // Disableコマンドで使用不可になった武器も同様
            //                if (withBlock.IsDisabled(withBlock.Weapon(w).Name))
            //                {
            //                    goto NextWeapon;
            //                }
            //                // フォーメーションを満たしていない合体技も
            //                string argattr9 = "合";
            //                if (withBlock.IsWeaponClassifiedAs(w, argattr9))
            //                {
            //                    if (!withBlock.IsCombinationAttackAvailable(w, true))
            //                    {
            //                        goto NextWeapon;
            //                    }
            //                }
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(150, 0, 0)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                // MOD  END  240a
            //            }

            //            // 武器の表示
            //            string argtarea1 = "";
            //            if (withBlock.WeaponPower(w, argtarea1) < 10000)
            //            {
            //                string argbuf18 = SrcFormatter.Format(withBlock.WeaponNickname(w));
            //                buf = GeneralLib.RightPaddedString(argbuf18, 25);
            //                string localLeftPaddedString19() { string argtarea = ""; string argbuf = SrcFormatter.Format(withBlock.WeaponPower(w, argtarea)); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //                buf = buf + localLeftPaddedString19();
            //            }
            //            else
            //            {
            //                string argbuf19 = SrcFormatter.Format(withBlock.WeaponNickname(w));
            //                buf = GeneralLib.RightPaddedString(argbuf19, 24);
            //                string localLeftPaddedString20() { string argtarea = ""; string argbuf = SrcFormatter.Format(withBlock.WeaponPower(w, argtarea)); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                buf = buf + localLeftPaddedString20();
            //            }

            //            // 武器が特殊効果を持つ場合は略称で表記
            //            if (withBlock.WeaponMaxRange(w) > 1)
            //            {
            //                string localLeftPaddedString21() { string argbuf = SrcFormatter.Format(withBlock.Weapon(w).MinRange) + "-" + SrcFormatter.Format(withBlock.WeaponMaxRange(w)); var ret = GeneralLib.LeftPaddedString(argbuf, 34 - LenB(Strings.StrConv(buf, vbFromUnicode))); return ret; }

            //                buf = buf + localLeftPaddedString21();
            //                // 移動後攻撃可能
            //                string argattr10 = "Ｐ";
            //                if (withBlock.IsWeaponClassifiedAs(w, argattr10))
            //                {
            //                    buf = buf + "P";
            //                }
            //            }
            //            else
            //            {
            //                string argbuf20 = "1";
            //                buf = buf + GeneralLib.LeftPaddedString(argbuf20, 34 - LenB(Strings.StrConv(buf, vbFromUnicode)));
            //                // ADD START MARGE
            //                // 移動後攻撃不可
            //                string argattr11 = "Ｑ";
            //                if (withBlock.IsWeaponClassifiedAs(w, argattr11))
            //                {
            //                    buf = buf + "Q";
            //                }
            //                // ADD END MARGE
            //            }
            //            // マップ攻撃
            //            string argattr12 = "Ｍ";
            //            if (withBlock.IsWeaponClassifiedAs(w, argattr12))
            //            {
            //                buf = buf + "M";
            //            }
            //            // 特殊効果
            //            wclass = withBlock.Weapon(w).Class_Renamed;
            //            var loopTo25 = withBlock.CountWeaponEffect(w);
            //            for (j = 1; j <= loopTo25; j++)
            //                buf = buf + "+";
            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            upic.Print(buf);
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        NextWeapon:
            //            ;
            //        }

            //        // アビリティ一覧
            //        var loopTo26 = withBlock.CountAbility();
            //        for (i = 1; i <= loopTo26; i++)
            //        {
            //            if (upic.CurrentY > 420)
            //            {
            //                break;
            //            }

            //            string argref_mode2 = "ステータス";
            //            if (!withBlock.IsAbilityAvailable(i, argref_mode2))
            //            {
            //                // 習得していない技は表示しない
            //                if (!withBlock.IsAbilityMastered(i))
            //                {
            //                    goto NextAbility;
            //                }
            //                // Disableコマンドで使用不可になった武器も同様
            //                if (withBlock.IsDisabled(withBlock.Ability(i).Name))
            //                {
            //                    goto NextAbility;
            //                }
            //                // フォーメーションを満たしていない合体技も
            //                string argattr13 = "合";
            //                if (withBlock.IsAbilityClassifiedAs(i, argattr13))
            //                {
            //                    if (!withBlock.IsCombinationAbilityAvailable(i, true))
            //                    {
            //                        goto NextAbility;
            //                    }
            //                }
            //                // MOD START 240a
            //                // upic.ForeColor = rgb(150, 0, 0)
            //                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
            //                // MOD  END  240a
            //            }

            //            // ADD START 240a
            //            if (GUI.NewGUIMode)
            //            {
            //                upic.CurrentX = 5;
            //            }
            //            // ADD  END  240a
            //            // アビリティの表示
            //            string localRightPaddedString18() { string argbuf = SrcFormatter.Format(withBlock.AbilityNickname(i)); var ret = GeneralLib.RightPaddedString(argbuf, 29); return ret; }

            //            upic.Print(localRightPaddedString18());
            //            if (withBlock.AbilityMaxRange(i) > 1)
            //            {
            //                string localLeftPaddedString22() { string argbuf = SrcFormatter.Format(withBlock.AbilityMinRange(i)) + "-" + SrcFormatter.Format(withBlock.AbilityMaxRange(i)); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

            //                upic.Print(localLeftPaddedString22());
            //                string argattr14 = "Ｐ";
            //                if (withBlock.IsAbilityClassifiedAs(i, argattr14))
            //                {
            //                    upic.Print("P");
            //                }

            //                string argattr15 = "Ｍ";
            //                if (withBlock.IsAbilityClassifiedAs(i, argattr15))
            //                {
            //                    upic.Print("M");
            //                }
            //                upic.Print();
            //            }
            //            else if (withBlock.AbilityMaxRange(i) == 1)
            //            {
            //                upic.Print("    1");
            //                // ADD START MARGE
            //                string argattr16 = "Ｑ";
            //                if (withBlock.IsAbilityClassifiedAs(i, argattr16))
            //                {
            //                    upic.Print("Q");
            //                }
            //                // ADD END MARGE
            //                string argattr17 = "Ｍ";
            //                if (withBlock.IsAbilityClassifiedAs(i, argattr17))
            //                {
            //                    upic.Print("M");
            //                }
            //                upic.Print();
            //            }
            //            else
            //            {
            //                upic.Print("    -");
            //            }
            //            // MOD START 240a
            //            // upic.ForeColor = rgb(0, 0, 0)
            //            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
            //        // MOD  END  240a
            //        NextAbility:
            //            ;
            //        }
            //    }

            //UpdateStatusWindow:
            //    ;


            //    // MOD START 240a
            //    // If MainWidth = 15 Then
            //    if (!GUI.NewGUIMode)
            //    {
            //        // MOD  END
            //        // ステータスウィンドウをリフレッシュ
            //        GUI.MainForm.picFace.Refresh();
            //        ppic.Refresh();
            //        upic.Refresh();
            //    }
            //    else
            //    {
            //        if (GUI.MouseX < GUI.MainPWidth / 2)
            //        {
            //            // MOD START 240a
            //            // upic.Move MainPWidth - 230 - 5, 10
            //            // 画面左側にカーソルがある場合
            //            upic.SetBounds(SrcFormatter.TwipsToPixelsX(GUI.MainPWidth - 240), SrcFormatter.TwipsToPixelsY(10d), 0, 0, BoundsSpecified.X | BoundsSpecified.Y);
            //        }
            //        // MOD  END
            //        else
            //        {
            //            upic.SetBounds(SrcFormatter.TwipsToPixelsX(5d), SrcFormatter.TwipsToPixelsY(10d), 0, 0, BoundsSpecified.X | BoundsSpecified.Y);
            //        }

            //        if (upic.Visible)
            //        {
            //            upic.Refresh();
            //        }
            //        else
            //        {
            //            upic.Visible = true;
            //        }
            //    }

            //    return;
            //ErrorHandler:
            //    ;
            //    string argmsg = "パイロット用画像ファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "画像ファイルが壊れていないか確認して下さい。";
            //    GUI.ErrorMessage(argmsg);
            //}
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
            if (Commands.CommandState == "ターゲット選択" & Commands.SelectedCommand == "発進")
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
            //if (GUI.MainWidth == 15)
            //{
            picFace.Image = null;
            picPilotStatus.Image = null;
            picUnitStatus.Image = null;
            DisplayedUnit = null;
            //}
            //else
            //{
            //    GUI.MainForm.picUnitStatus.Visible = false;
            //    GUI.MainForm.picUnitStatus.Cls();
            //    IsStatusWindowDisabled = true;
            //    Application.DoEvents();
            //    IsStatusWindowDisabled = false;
            //    // ADD
            //    // UPGRADE_NOTE: オブジェクト DisplayedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    DisplayedUnit = null;
            //}
        }
    }
}
