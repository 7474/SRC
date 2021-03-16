//using System;
//using System.Drawing;
//using System.Windows.Forms;
//using Microsoft.VisualBasic;
//using Microsoft.VisualBasic.CompilerServices;

//namespace Project1
//{
//    static class Status
//    {

//        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
//        // 本プログラムはフリーソフトであり、無保証です。
//        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
//        // 再頒布または改変することができます。

//        // ステータスウィンドウへのステータス表示を行うモジュール
//        // ppicとupicに分かれているが、ppicにはアイコンと同じ行のデータが書き込まれる。
//        // 決してパイロットステータスを書くのがppicではないことを留意しておく

//        // ステータス画面に表示されているユニット
//        public static Unit DisplayedUnit;
//        public static short DisplayedPilotInd;

//        // ステータス画面の更新を一時停止するかどうか
//        public static bool IsStatusWindowDisabled;
//        // ADD START 240a
//        // ステータス画面の背景色
//        public static int StatusWindowBackBolor;
//        // ステータス画面の枠色
//        public static int StatusWindowFrameColor;
//        // ステータス画面の枠幅
//        public static int StatusWindowFrameWidth;
//        // ステータス画面 能力名のフォントカラー
//        public static int StatusFontColorAbilityName;
//        // ステータス画面 有効な能力のフォントカラー
//        public static int StatusFontColorAbilityEnable;
//        // ステータス画面 無効な能力のフォントカラー
//        public static int StatusFontColorAbilityDisable;
//        // ステータス画面 その他通常描画のフォントカラー
//        public static int StatusFontColorNormalString;
//        // ADD  END

//        // 現在の状況をステータスウィンドウに表示
//        public static void DisplayGlobalStatus()
//        {
//            short X, Y;
//            PictureBox pic;
//            TerrainData td;
//            // ADD START 240a
//            string fname;
//            var wHeight = default(short);
//            int lineStart, ret, color, lineEnd;
//            // ADD  END  240a

//            // ステータスウィンドウを消去
//            ClearUnitStatus();

//            pic = GUI.MainForm.picUnitStatus;
//            pic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeSize(pic.Font, 12f);

//            // ADD START 240a
//            // マウスカーソルの位置は？
//            X = GUI.PixelToMapX((short)GUI.MouseX);
//            Y = GUI.PixelToMapY((short)GUI.MouseY);
//            if (GUI.NewGUIMode)
//            {
//                // Global変数が宣言されていれば、ステータス画面用変数の同期を取る
//                GlobalVariableLoad();
//                pic.BackColor = ColorTranslator.FromOle(StatusWindowBackBolor);
//                pic.DrawWidth = StatusWindowFrameWidth;
//                color = StatusWindowFrameColor;
//                lineStart = (int)((StatusWindowFrameWidth - 1) / 2d);
//                lineEnd = (int)((StatusWindowFrameWidth + 1) / 2d);
//                pic.FillStyle = vbFSTransparent;
//                // 一旦高さを最大にする
//                pic.Width = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(235d);
//                pic.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(GUI.MapPHeight - 20);
//                wHeight = (short)GetGlobalStatusSize(ref X, ref Y);
//                // 枠線を引く
//                pic.Line(lineStart, lineStart); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                pic.FillStyle = Event_Renamed.ObjFillStyle;
//                // 高さを設定する
//                pic.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(wHeight);
//                pic.CurrentX = 5;
//                pic.CurrentY = 5;
//                // 文字色をリセット
//                pic.ForeColor = ColorTranslator.FromOle(StatusFontColorNormalString);
//            }
//            // ADD  END  240a
//            pic.Print("ターン数 " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SRC.Turn));
//            // ADD START 240a
//            if (GUI.NewGUIMode)
//            {
//                pic.CurrentX = 5;
//            }
//            // ADD  END  240a
//            string argtname = "資金";
//            Unit argu = null;
//            pic.Print(Expression.Term(ref argtname, ref argu, 8) + " " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SRC.Money));

//            // MOV START 240a ↑に移動
//            // 'マウスカーソルの位置は？
//            // X = PixelToMapX(MouseX)
//            // Y = PixelToMapY(MouseY)
//            // MOV  END  240a

//            // マップ外をクリックした時はここで終了
//            if (X < 1 | Map.MapWidth < X | Y < 1 | Map.MapHeight < Y)
//            {
//                pic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeSize(pic.Font, 9f);
//                if (GUI.NewGUIMode)
//                {
//                    // 高さを設定する
//                    pic.Height = (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(wHeight);
//                }

//                return;
//            }

//            // 地形情報の表示
//            pic.Print();

//            // 地形名称
//            // ADD START 240a
//            // マップ画像表示
//            if (GUI.NewGUIMode)
//            {
//                ret = GUI.BitBlt(pic.hDC, 5, 48, 32, 32, GUI.MainForm.picBack.hDC, (X - 1) * 32, (Y - 1) * 32, GUI.SRCCOPY);
//            }
//            else
//            {
//                ret = GUI.BitBlt(pic.hDC, 0, 48, 32, 32, GUI.MainForm.picBack.hDC, (X - 1) * 32, (Y - 1) * 32, GUI.SRCCOPY);
//            }
//            pic.CurrentX = 37;
//            pic.CurrentY = 65;
//            // ADD  END  240a
//            if (Strings.InStr(Map.TerrainName(X, Y), "(") > 0)
//            {
//                pic.Print("(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(X) + "," + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Y) + ") " + Strings.Left(Map.TerrainName(X, Y), Strings.InStr(Map.TerrainName(X, Y), "(") - 1));
//            }
//            else
//            {
//                pic.Print("(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(X) + "," + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Y) + ") " + Map.TerrainName(X, Y));
//            }

//            // ADD START 240a
//            if (GUI.NewGUIMode)
//            {
//                pic.CurrentX = 5;
//            }
//            // ADD  END  240a
//            // 命中修正
//            if (Map.TerrainEffectForHit(X, Y) >= 0)
//            {
//                pic.Print("回避 +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForHit(X, Y)) + "%");
//            }
//            else
//            {
//                pic.Print("回避 " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForHit(X, Y)) + "%");
//            }

//            // ダメージ修正
//            if (Map.TerrainEffectForDamage(X, Y) >= 0)
//            {
//                pic.Print("  防御 +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForDamage(X, Y)) + "%");
//            }
//            else
//            {
//                pic.Print("  防御 " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForDamage(X, Y)) + "%");
//            }

//            // ADD START 240a
//            if (GUI.NewGUIMode)
//            {
//                pic.CurrentX = 5;
//            }
//            // ADD  END  240a
//            // ＨＰ回復率
//            if (Map.TerrainEffectForHPRecover(X, Y) > 0)
//            {
//                string argtname1 = "ＨＰ";
//                Unit argu1 = null;
//                pic.Print(Expression.Term(ref argtname1, u: ref argu1) + " +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForHPRecover(X, Y)) + "%  ");
//            }

//            // ＥＮ回復率
//            if (Map.TerrainEffectForENRecover(X, Y) > 0)
//            {
//                string argtname2 = "ＥＮ";
//                Unit argu2 = null;
//                pic.Print(Expression.Term(ref argtname2, u: ref argu2) + " +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForENRecover(X, Y)) + "%");
//            }

//            if (Map.TerrainEffectForHPRecover(X, Y) > 0 | Map.TerrainEffectForENRecover(X, Y) > 0)
//            {
//                pic.Print();
//            }

//            // MOD START 240a
//            // Set td = TDList.Item(MapData(X, Y, 0))
//            // マスのタイプに応じて参照先を変更
//            switch (Map.MapData[X, Y, Map.MapDataIndex.BoxType])
//            {
//                case (short)Map.BoxTypes.Under:
//                case (short)Map.BoxTypes.UpperBmpOnly:
//                    {
//                        td = SRC.TDList.Item(Map.MapData[X, Y, Map.MapDataIndex.TerrainType]);
//                        break;
//                    }

//                default:
//                    {
//                        td = SRC.TDList.Item(Map.MapData[X, Y, Map.MapDataIndex.LayerType]);
//                        break;
//                    }
//            }
//            // MOD  END

//            // ADD START 240a
//            if (GUI.NewGUIMode)
//            {
//                pic.CurrentX = 5;
//            }
//            // ADD  END  240a
//            // ＨＰ＆ＥＮ減少
//            string argfname = "ＨＰ減少";
//            if (td.IsFeatureAvailable(ref argfname))
//            {
//                string argtname3 = "ＨＰ";
//                Unit argu3 = null;
//                object argIndex1 = "ＨＰ減少";
//                object argIndex2 = "ＨＰ減少";
//                pic.Print(Expression.Term(ref argtname3, u: ref argu3) + " -" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * td.FeatureLevel(ref argIndex1)) + "% (" + td.FeatureData(ref argIndex2) + ")  ");
//            }

//            string argfname1 = "ＥＮ減少";
//            if (td.IsFeatureAvailable(ref argfname1))
//            {
//                string argtname4 = "ＥＮ";
//                Unit argu4 = null;
//                object argIndex3 = "ＥＮ減少";
//                object argIndex4 = "ＥＮ減少";
//                pic.Print(Expression.Term(ref argtname4, u: ref argu4) + " -" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * td.FeatureLevel(ref argIndex3)) + "% (" + td.FeatureData(ref argIndex4) + ")  ");
//            }

//            string argfname2 = "ＨＰ減少";
//            string argfname3 = "ＥＮ減少";
//            if (td.IsFeatureAvailable(ref argfname2) | td.IsFeatureAvailable(ref argfname3))
//            {
//                pic.Print();
//            }

//            // ADD START 240a
//            if (GUI.NewGUIMode)
//            {
//                pic.CurrentX = 5;
//            }
//            // ADD  END  240a
//            // ＨＰ＆ＥＮ増加
//            string argfname4 = "ＨＰ増加";
//            if (td.IsFeatureAvailable(ref argfname4))
//            {
//                string argtname5 = "ＨＰ";
//                Unit argu5 = null;
//                object argIndex5 = "ＨＰ増加";
//                pic.Print(Expression.Term(ref argtname5, u: ref argu5) + " +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(1000d * td.FeatureLevel(ref argIndex5)) + "  ");
//            }

//            string argfname5 = "ＥＮ増加";
//            if (td.IsFeatureAvailable(ref argfname5))
//            {
//                string argtname6 = "ＥＮ";
//                Unit argu6 = null;
//                object argIndex6 = "ＥＮ増加";
//                pic.Print(Expression.Term(ref argtname6, u: ref argu6) + " +" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * td.FeatureLevel(ref argIndex6)) + "  ");
//            }

//            string argfname6 = "ＨＰ増加";
//            string argfname7 = "ＥＮ増加";
//            if (td.IsFeatureAvailable(ref argfname6) | td.IsFeatureAvailable(ref argfname7))
//            {
//                pic.Print();
//            }
//            // MOD  END

//            // ADD START 240a
//            if (GUI.NewGUIMode)
//            {
//                pic.CurrentX = 5;
//            }
//            // ADD  END  240a
//            // ＨＰ＆ＥＮ低下
//            string argfname8 = "ＨＰ低下";
//            if (td.IsFeatureAvailable(ref argfname8))
//            {
//                string argtname7 = "ＨＰ";
//                Unit argu7 = null;
//                object argIndex7 = "ＨＰ低下";
//                pic.Print(Expression.Term(ref argtname7, u: ref argu7) + " -" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(1000d * td.FeatureLevel(ref argIndex7)) + "  ");
//            }

//            string argfname9 = "ＥＮ低下";
//            if (td.IsFeatureAvailable(ref argfname9))
//            {
//                string argtname8 = "ＥＮ";
//                Unit argu8 = null;
//                object argIndex8 = "ＥＮ低下";
//                pic.Print(Expression.Term(ref argtname8, u: ref argu8) + " -" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * td.FeatureLevel(ref argIndex8)) + "  ");
//            }

//            string argfname10 = "ＨＰ低下";
//            string argfname11 = "ＥＮ低下";
//            if (td.IsFeatureAvailable(ref argfname10) | td.IsFeatureAvailable(ref argfname11))
//            {
//                pic.Print();
//            }

//            // ADD START 240a
//            if (GUI.NewGUIMode)
//            {
//                pic.CurrentX = 5;
//            }
//            // ADD  END  240a
//            // 摩擦
//            string argfname12 = "摩擦";
//            if (td.IsFeatureAvailable(ref argfname12))
//            {
//                object argIndex9 = "摩擦";
//                pic.Print("摩擦Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(td.FeatureLevel(ref argIndex9)));
//            }
//            // ADD START MARGE
//            // 状態異常付加
//            string argfname13 = "状態付加";
//            if (td.IsFeatureAvailable(ref argfname13))
//            {
//                object argIndex10 = "状態付加";
//                pic.Print(td.FeatureData(ref argIndex10) + "状態付加");
//            }
//            // ADD END MARGE

//            // フォントサイズを元に戻しておく
//            pic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeSize(pic.Font, 9f);
//        }

//        // ユニットステータスを表示
//        // pindexはステータス表示に使うパイロットを指定
//        public static void DisplayUnitStatus(ref Unit u, short pindex = 0)
//        {
//            Pilot p;
//            short k, i, j, n;
//            int ret;
//            string buf;
//            string fdata, fname, opt;
//            string stype, sname, slevel;
//            short cx, cy;
//            short[] warray;
//            int[] wpower;
//            PictureBox ppic, upic;
//            short nmorale, ecost, pmorale;
//            string[] flist;
//            var is_unknown = default(bool);
//            short prob, w, cprob;
//            int dmg;
//            string def_mode;
//            string[] name_list;
//            // ADD START 240a
//            int lineStart, color, lineEnd;
//            bool isNoSp;
//            isNoSp = false;
//            // ADD  END  240a
//            // ステータス画面の更新が一時停止されている場合はそのまま終了
//            if (IsStatusWindowDisabled)
//            {
//                return;
//            }

//            // 破壊、破棄されたユニットは表示しない
//            if (u.Status_Renamed == "破壊" | u.Status_Renamed == "破棄")
//            {
//                return;
//            }

//            DisplayedUnit = u;
//            DisplayedPilotInd = pindex;

//            // MOD START MARGE
//            // If MainWidth = 15 Then
//            if (!GUI.NewGUIMode)
//            {
//                // MOD  END  MARGE
//                ppic = GUI.MainForm.picPilotStatus;
//                upic = GUI.MainForm.picUnitStatus;
//                ppic.Cls();
//                upic.Cls();
//            }
//            else
//            {
//                ppic = GUI.MainForm.picUnitStatus;
//                upic = GUI.MainForm.picUnitStatus;
//                upic.Cls();
//                // ADD START 240a
//                // global変数とステータス描画用の変数を同期
//                GlobalVariableLoad();
//                // 新ＧＵＩでは地形表示したときにサイズを変えているので元に戻す
//                upic.SetBounds((int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(GUI.MainPWidth - 240), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(10d), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(235d), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(GUI.MainPHeight - 20));
//                upic.BackColor = ColorTranslator.FromOle(StatusWindowBackBolor);
//                upic.DrawWidth = StatusWindowFrameWidth;
//                color = StatusWindowFrameColor;
//                lineStart = (int)((StatusWindowFrameWidth - 1) / 2d);
//                lineEnd = (int)((StatusWindowFrameWidth + 1) / 2d);
//                upic.FillStyle = vbFSTransparent;
//                // 枠線を引く
//                upic.Line(lineStart, lineStart); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                upic.FillStyle = Event_Renamed.ObjFillStyle;
//                upic.CurrentX = 5;
//                upic.CurrentY = 5;
//                // 文字色をリセット
//                upic.ForeColor = ColorTranslator.FromOle(StatusFontColorNormalString);
//                // ADD  END
//            }

//            TerrainData td;
//            string wclass;
//            {
//                var withBlock = u;
//                // 情報を更新
//                withBlock.Update();

//                // 未確認ユニットかどうか判定しておく
//                string argoname = "ユニット情報隠蔽";
//                object argIndex1 = "識別済み";
//                object argIndex2 = "ユニット情報隠蔽";
//                if (Expression.IsOptionDefined(ref argoname) & !withBlock.IsConditionSatisfied(ref argIndex1) & (withBlock.Party0 == "敵" | withBlock.Party0 == "中立") | withBlock.IsConditionSatisfied(ref argIndex2))
//                {
//                    is_unknown = true;
//                }

//                // パイロットが乗っていない？
//                if (withBlock.CountPilot() == 0)
//                {
//                    // キャラ画面をクリア
//                    if (GUI.MainWidth == 15)
//                    {
//                        GUI.MainForm.picFace = Image.FromFile("");
//                    }
//                    else
//                    {
//                        string argfname = "white.bmp";
//                        string argdraw_option = "ステータス";
//                        GUI.DrawPicture(ref argfname, 2, 2, 64, 64, 0, 0, 0, 0, ref argdraw_option);
//                    }
//                    // MOD START 240a
//                    // ppic.ForeColor = rgb(0, 0, 150)
//                    ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname = "レベル";
//                    ppic.Print(Expression.Term(ref argtname, ref u));
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname1 = "気力";
//                    ppic.Print(Expression.Term(ref argtname1, ref u));
//                    // MOD START 240a
//                    // ppic.ForeColor = rgb(0, 0, 0)
//                    ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a

//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname2 = "格闘";
//                    string argtname3 = "射撃";
//                    upic.Print(Expression.Term(ref argtname2, ref u, 4) + "               " + Expression.Term(ref argtname3, ref u));
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname4 = "命中";
//                    string argtname5 = "回避";
//                    upic.Print(Expression.Term(ref argtname4, ref u, 4) + "               " + Expression.Term(ref argtname5, ref u));
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname6 = "技量";
//                    string argtname7 = "反応";
//                    upic.Print(Expression.Term(ref argtname6, ref u, 4) + "               " + Expression.Term(ref argtname7, ref u));
//                    upic.Print();
//                    upic.Print();
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a

//                    goto UnitStatus;
//                }

//                // 表示するパイロットを選択
//                if (pindex == 0)
//                {
//                    // メインパイロット
//                    p = withBlock.MainPilot();
//                    object argIndex3 = 1;
//                    if ((withBlock.MainPilot().get_Nickname(false) ?? "") == (withBlock.Pilot(ref argIndex3).get_Nickname(false) ?? "") | withBlock.Data.PilotNum == 1)
//                    {
//                        DisplayedPilotInd = 1;
//                    }
//                }
//                else if (pindex == 1)
//                {
//                    // メインパイロットまたは１番目のパイロット
//                    object argIndex5 = 1;
//                    if ((withBlock.MainPilot().get_Nickname(false) ?? "") != (withBlock.Pilot(ref argIndex5).get_Nickname(false) ?? "") & withBlock.Data.PilotNum != 1)
//                    {
//                        object argIndex4 = 1;
//                        p = withBlock.Pilot(ref argIndex4);
//                    }
//                    else
//                    {
//                        p = withBlock.MainPilot();
//                    }
//                }
//                else if (pindex <= withBlock.CountPilot())
//                {
//                    // サブパイロット
//                    object argIndex6 = pindex;
//                    p = withBlock.Pilot(ref argIndex6);
//                }
//                else if (pindex <= (short)(withBlock.CountPilot() + withBlock.CountSupport()))
//                {
//                    // サポートパイロット
//                    object argIndex7 = pindex - withBlock.CountPilot();
//                    p = withBlock.Support(ref argIndex7);
//                }
//                else
//                {
//                    // 追加サポート
//                    p = withBlock.AdditionalSupport();
//                }
//                // 情報を更新
//                p.UpdateSupportMod();

//                // パイロット画像を表示
//                fname = @"\Bitmap\Pilot\" + p.get_Bitmap(false);
//                if (My.MyProject.Forms.frmMultiSelectListBox.Visible)
//                {
//                    // ザコ＆汎用パイロットが乗るユニットの出撃選択時はパイロット画像の
//                    // 代わりにユニット画像を表示
//                    if (Strings.InStr(p.Name, "(ザコ)") > 0 | Strings.InStr(p.Name, "(汎用)") > 0)
//                    {
//                        fname = @"\Bitmap\Unit\" + u.get_Bitmap(false);
//                    }
//                }

//                // 画像ファイルを検索
//                bool localFileExists() { string argfname = SRC.ScenarioPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

//                bool localFileExists1() { string argfname = SRC.ExtDataPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

//                bool localFileExists2() { string argfname = SRC.ExtDataPath2 + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

//                bool localFileExists3() { string argfname = SRC.AppPath + fname; var ret = GeneralLib.FileExists(ref argfname); return ret; }

//                if (Strings.InStr(fname, @"\-.bmp") > 0)
//                {
//                    fname = "";
//                }
//                else if (localFileExists())
//                {
//                    fname = SRC.ScenarioPath + fname;
//                }
//                else if (localFileExists1())
//                {
//                    fname = SRC.ExtDataPath + fname;
//                }
//                else if (localFileExists2())
//                {
//                    fname = SRC.ExtDataPath2 + fname;
//                }
//                else if (localFileExists3())
//                {
//                    fname = SRC.AppPath + fname;
//                }
//                else
//                {
//                    // 画像が見つからなかったことを記録
//                    if (Strings.InStr(fname, @"\Pilot\") > 0)
//                    {
//                        if ((p.get_Bitmap(false) ?? "") == (p.Data.Bitmap ?? ""))
//                        {
//                            p.Data.IsBitmapMissing = true;
//                        }
//                    }

//                    fname = "";
//                }

//                // 画像ファイルを読み込んで表示
//                if (GUI.MainWidth == 15)
//                {
//                    if (!string.IsNullOrEmpty(fname))
//                    {
//                        ;
//                        GUI.MainForm.picTmp = Image.FromFile(fname);
//                        ;
//                        GUI.MainForm.picFace.PaintPicture(GUI.MainForm.picTmp.Picture, 0, 0, 64, 64);
//                    }
//                    else
//                    {
//                        // 画像ファイルが見つからなかった場合はキャラ画面をクリア
//                        GUI.MainForm.picFace = Image.FromFile("");
//                    }
//                }
//                else if (!string.IsNullOrEmpty(fname))
//                {
//                    string argdraw_option1 = "ステータス";
//                    GUI.DrawPicture(ref fname, 2, 2, 64, 64, 0, 0, 0, 0, ref argdraw_option1);
//                }
//                else
//                {
//                    // 画像ファイルが見つからなかった場合はキャラ画面をクリア
//                    string argfname1 = "white.bmp";
//                    string argdraw_option2 = "ステータス";
//                    GUI.DrawPicture(ref argfname1, 2, 2, 64, 64, 0, 0, 0, 0, ref argdraw_option2);
//                }

//                // パイロット愛称
//                ppic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeSize(ppic.Font, 10.5f);
//                ppic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeBold(ppic.Font, false);
//                // MOD START 240a
//                // If MainWidth <> 15 Then
//                if (GUI.NewGUIMode)
//                {
//                    // MOD  END
//                    ppic.CurrentX = 68;
//                }
//                ppic.Print(p.get_Nickname(false));
//                ppic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeBold(ppic.Font, false);
//                ppic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeSize(ppic.Font, 10f);

//                // ダミーパイロット？
//                if (p.Nickname0 == "パイロット不在")
//                {
//                    // MOD START 240a
//                    // ppic.ForeColor = rgb(0, 0, 150)
//                    ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname8 = "レベル";
//                    ppic.Print(Expression.Term(ref argtname8, ref u));
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname9 = "気力";
//                    ppic.Print(Expression.Term(ref argtname9, ref u));
//                    // MOD START 240a
//                    // ppic.ForeColor = rgb(0, 0, 0)
//                    ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a

//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname10 = "格闘";
//                    string argtname11 = "射撃";
//                    upic.Print(Expression.Term(ref argtname10, ref u, 4) + "               " + Expression.Term(ref argtname11, ref u));
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname12 = "命中";
//                    string argtname13 = "回避";
//                    upic.Print(Expression.Term(ref argtname12, ref u, 4) + "               " + Expression.Term(ref argtname13, ref u));
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname14 = "技量";
//                    string argtname15 = "反応";
//                    upic.Print(Expression.Term(ref argtname14, ref u, 4) + "               " + Expression.Term(ref argtname15, ref u));
//                    upic.Print();
//                    upic.Print();
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a

//                    goto UnitStatus;
//                }
//                // レベル、経験値、行動回数
//                // MOD START 240a
//                // ppic.ForeColor = rgb(0, 0, 150)
//                ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                // MOD START 240a
//                // If MainWidth <> 15 Then
//                if (GUI.NewGUIMode)
//                {
//                    // MOD  END  240a
//                    ppic.CurrentX = 68;
//                }
//                string argtname16 = "レベル";
//                ppic.Print(Expression.Term(ref argtname16, ref u) + " ");
//                // MOD START 240a
//                // ppic.ForeColor = rgb(0, 0, 0)
//                ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                if (p.Party == "味方")
//                {
//                    ppic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Level) + " (" + p.Exp + ")");
//                    switch (u.Action)
//                    {
//                        case 2:
//                            {
//                                // MOD START 240a
//                                // ppic.ForeColor = rgb(0, 0, 200)
//                                ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                                // MOD  END  240a
//                                ppic.Print(" Ｗ");
//                                // MOD START 240a
//                                // ppic.ForeColor = rgb(0, 0, 0)
//                                ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                                break;
//                            }
//                        // MOD  END  240a
//                        case 3:
//                            {
//                                // MOD START 240a
//                                // ppic.ForeColor = rgb(0, 0, 200)
//                                ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                                // MOD  END  240a
//                                ppic.Print(" Ｔ");
//                                // MOD START 240a
//                                // ppic.ForeColor = rgb(0, 0, 0)
//                                ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                                break;
//                            }
//                            // MOD  END  240a
//                    }
//                }
//                else if (!is_unknown)
//                {
//                    ppic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Level));
//                    if (u.Action == 2)
//                    {
//                        // MOD START 240a
//                        // ppic.ForeColor = rgb(0, 0, 200)
//                        ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                        // MOD  END  240a
//                        ppic.Print(" Ｗ");
//                        // MOD START 240a
//                        // ppic.ForeColor = rgb(0, 0, 0)
//                        ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                        // MOD  END  240a
//                    }
//                }
//                else
//                {
//                    ppic.Print("？");
//                }
//                ppic.Print();

//                // 気力
//                // MOD START 240a
//                // ppic.ForeColor = rgb(0, 0, 150)
//                ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                if (GUI.MainWidth != 15)
//                {
//                    ppic.CurrentX = 68;
//                }
//                string argtname17 = "気力";
//                ppic.Print(Expression.Term(ref argtname17, ref u) + " ");
//                // MOD START 240a
//                // ppic.ForeColor = rgb(0, 0, 0)
//                ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                if (!is_unknown)
//                {
//                    if (p.MoraleMod > 0)
//                    {
//                        ppic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Morale) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MoraleMod) + " (" + p.Personality + ")");
//                    }
//                    else
//                    {
//                        ppic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Morale) + " (" + p.Personality + ")");
//                    }
//                }
//                else
//                {
//                    ppic.Print("？");
//                }

//                // ＳＰ
//                if (p.MaxSP > 0)
//                {
//                    // MOD START 240a
//                    // ppic.ForeColor = rgb(0, 0, 150)
//                    ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    if (GUI.MainWidth != 15)
//                    {
//                        ppic.CurrentX = 68;
//                    }
//                    string argtname18 = "ＳＰ";
//                    ppic.Print(Expression.Term(ref argtname18, ref u) + " ");
//                    // MOD START 240a
//                    // ppic.ForeColor = rgb(0, 0, 0)
//                    ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    if (!is_unknown)
//                    {
//                        ppic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.SP) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MaxSP));
//                    }
//                    else
//                    {
//                        ppic.Print("？");
//                    }
//                }
//                else
//                {
//                    isNoSp = true;
//                }

//                // 使用中のスペシャルパワー一覧
//                if (!is_unknown)
//                {
//                    // MOD START 240a
//                    // ppic.ForeColor = rgb(0, 0, 0)
//                    ppic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    // MOD START 240a
//                    // If MainWidth <> 15 Then
//                    if (GUI.NewGUIMode)
//                    {
//                        // MOD  END
//                        ppic.CurrentX = 68;
//                    }
//                    ppic.Print(u.SpecialPowerInEffect());
//                }
//                // ADD START 240a
//                else if (GUI.NewGUIMode)
//                {
//                    ppic.Print(" ");
//                    // ADD  END  240a
//                }
//                // ADD START 240a
//                if (isNoSp)
//                {
//                    ppic.Print(" ");
//                }

//                // upicを明示的に初期化
//                upic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeBold(upic.Font, false);
//                upic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeSize(upic.Font, 9f);

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 格闘
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname19 = "格闘";
//                upic.Print(Expression.Term(ref argtname19, ref u, 4) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                if (is_unknown)
//                {
//                    string argbuf = "？";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf, 4) + Strings.Space(10));
//                }
//                else if (p.Data.Infight > 1)
//                {
//                    switch ((short)(p.InfightMod + p.InfightMod2))
//                    {
//                        case var @case when @case > 0:
//                            {
//                                string localLeftPaddedString() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.InfightBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString() { string argbuf = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.InfightMod + p.InfightMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 9); return ret; }

//                                upic.Print(localLeftPaddedString() + localRightPaddedString());
//                                break;
//                            }

//                        case var case1 when case1 < 0:
//                            {
//                                string localLeftPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.InfightBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString1() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.InfightMod + p.InfightMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 9); return ret; }

//                                upic.Print(localLeftPaddedString1() + localRightPaddedString1());
//                                break;
//                            }

//                        case 0:
//                            {
//                                string localLeftPaddedString2() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Infight); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                upic.Print(localLeftPaddedString2() + Strings.Space(9));
//                                break;
//                            }
//                    }
//                }
//                else
//                {
//                    string argbuf1 = "--";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf1, 5) + Strings.Space(9));
//                }

//                // 射撃
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                if (!p.HasMana())
//                {
//                    string argtname20 = "射撃";
//                    upic.Print(Expression.Term(ref argtname20, ref u, 4) + " ");
//                }
//                else
//                {
//                    string argtname21 = "魔力";
//                    upic.Print(Expression.Term(ref argtname21, ref u, 4) + " ");
//                }
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                if (is_unknown)
//                {
//                    string argbuf2 = "？";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf2, 4));
//                }
//                else if (p.Data.Shooting > 1)
//                {
//                    switch ((short)(p.ShootingMod + p.ShootingMod2))
//                    {
//                        case var case2 when case2 > 0:
//                            {
//                                string localLeftPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.ShootingBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString2() { string argbuf = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.ShootingMod + p.ShootingMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 5); return ret; }

//                                upic.Print(localLeftPaddedString3() + localRightPaddedString2());
//                                break;
//                            }

//                        case var case3 when case3 < 0:
//                            {
//                                string localLeftPaddedString4() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.ShootingBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString3() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.ShootingMod + p.ShootingMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 5); return ret; }

//                                upic.Print(localLeftPaddedString4() + localRightPaddedString3());
//                                break;
//                            }

//                        case 0:
//                            {
//                                string localLeftPaddedString5() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Shooting); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                upic.Print(localLeftPaddedString5() + Strings.Space(5));
//                                break;
//                            }
//                    }
//                }
//                else
//                {
//                    string argbuf3 = "--";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf3, 5) + Strings.Space(5));
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 命中
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname22 = "命中";
//                upic.Print(Expression.Term(ref argtname22, ref u, 4) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                if (is_unknown)
//                {
//                    string argbuf4 = "？";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf4, 4) + Strings.Space(10));
//                }
//                else if (p.Data.Hit > 1)
//                {
//                    switch ((short)(p.HitMod + p.HitMod2))
//                    {
//                        case var case4 when case4 > 0:
//                            {
//                                string localLeftPaddedString6() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.HitBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString4() { string argbuf = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.HitMod + p.HitMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 9); return ret; }

//                                upic.Print(localLeftPaddedString6() + localRightPaddedString4());
//                                break;
//                            }

//                        case var case5 when case5 < 0:
//                            {
//                                string localLeftPaddedString7() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.HitBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString5() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.HitMod + p.HitMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 9); return ret; }

//                                upic.Print(localLeftPaddedString7() + localRightPaddedString5());
//                                break;
//                            }

//                        case 0:
//                            {
//                                string localLeftPaddedString8() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Hit); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                upic.Print(localLeftPaddedString8() + Strings.Space(9));
//                                break;
//                            }
//                    }
//                }
//                else
//                {
//                    string argbuf5 = "--";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf5, 5) + Strings.Space(9));
//                }

//                // 回避
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname23 = "回避";
//                upic.Print(Expression.Term(ref argtname23, ref u, 4) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                if (is_unknown)
//                {
//                    string argbuf6 = "？";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf6, 4));
//                }
//                else if (p.Data.Dodge > 1)
//                {
//                    switch ((short)(p.DodgeMod + p.DodgeMod2))
//                    {
//                        case var case6 when case6 > 0:
//                            {
//                                string localLeftPaddedString9() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.DodgeBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString6() { string argbuf = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.DodgeMod + p.DodgeMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 9); return ret; }

//                                upic.Print(localLeftPaddedString9() + localRightPaddedString6());
//                                break;
//                            }

//                        case var case7 when case7 < 0:
//                            {
//                                string localLeftPaddedString10() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.DodgeBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString7() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.DodgeMod + p.DodgeMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 9); return ret; }

//                                upic.Print(localLeftPaddedString10() + localRightPaddedString7());
//                                break;
//                            }

//                        case 0:
//                            {
//                                string localLeftPaddedString11() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Dodge); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                upic.Print(localLeftPaddedString11() + Strings.Space(9));
//                                break;
//                            }
//                    }
//                }
//                else
//                {
//                    string argbuf7 = "--";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf7, 5) + Strings.Space(9));
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 技量
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname24 = "技量";
//                upic.Print(Expression.Term(ref argtname24, ref u, 4) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                if (is_unknown)
//                {
//                    string argbuf8 = "？";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf8, 4) + Strings.Space(10));
//                }
//                else if (p.Data.Technique > 1)
//                {
//                    switch ((short)(p.TechniqueMod + p.TechniqueMod2))
//                    {
//                        case var case8 when case8 > 0:
//                            {
//                                string localLeftPaddedString12() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.TechniqueBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString8() { string argbuf = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.TechniqueMod + p.TechniqueMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 9); return ret; }

//                                upic.Print(localLeftPaddedString12() + localRightPaddedString8());
//                                break;
//                            }

//                        case var case9 when case9 < 0:
//                            {
//                                string localLeftPaddedString13() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.TechniqueBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString9() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.TechniqueMod + p.TechniqueMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 9); return ret; }

//                                upic.Print(localLeftPaddedString13() + localRightPaddedString9());
//                                break;
//                            }

//                        case 0:
//                            {
//                                string localLeftPaddedString14() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Technique); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                upic.Print(localLeftPaddedString14() + Strings.Space(9));
//                                break;
//                            }
//                    }
//                }
//                else
//                {
//                    string argbuf9 = "--";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf9, 5) + Strings.Space(9));
//                }

//                // 反応
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname25 = "反応";
//                upic.Print(Expression.Term(ref argtname25, ref u, 4) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                if (is_unknown)
//                {
//                    string argbuf10 = "？";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf10, 4));
//                }
//                else if (p.Data.Intuition > 1)
//                {
//                    switch ((short)(p.IntuitionMod + p.IntuitionMod2))
//                    {
//                        case var case10 when case10 > 0:
//                            {
//                                string localLeftPaddedString15() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.IntuitionBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString10() { string argbuf = "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.IntuitionMod + p.IntuitionMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 9); return ret; }

//                                upic.Print(localLeftPaddedString15() + localRightPaddedString10());
//                                break;
//                            }

//                        case var case11 when case11 < 0:
//                            {
//                                string localLeftPaddedString16() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.IntuitionBase); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                string localRightPaddedString11() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.IntuitionMod + p.IntuitionMod2); var ret = GeneralLib.RightPaddedString(ref argbuf, 9); return ret; }

//                                upic.Print(localLeftPaddedString16() + localRightPaddedString11());
//                                break;
//                            }

//                        case 0:
//                            {
//                                string localLeftPaddedString17() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Intuition); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                                upic.Print(localLeftPaddedString17() + Strings.Space(9));
//                                break;
//                            }
//                    }
//                }
//                else
//                {
//                    string argbuf11 = "--";
//                    upic.Print(GeneralLib.LeftPaddedString(ref argbuf11, 5) + Strings.Space(9));
//                }

//                string argoname1 = "防御力成長";
//                string argoname2 = "防御力レベルアップ";
//                if (Expression.IsOptionDefined(ref argoname1) | Expression.IsOptionDefined(ref argoname2))
//                {
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // 防御
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    string argtname26 = "防御";
//                    upic.Print(Expression.Term(ref argtname26, ref u) + " ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    if (is_unknown)
//                    {
//                        string argbuf12 = "？";
//                        upic.Print(GeneralLib.LeftPaddedString(ref argbuf12, 4));
//                    }
//                    else if (!p.IsSupport(ref u))
//                    {
//                        string localLeftPaddedString18() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Defense); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                        upic.Print(localLeftPaddedString18());
//                    }
//                    else
//                    {
//                        string argbuf13 = "--";
//                        upic.Print(GeneralLib.LeftPaddedString(ref argbuf13, 5));
//                    }
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 所有するスペシャルパワー一覧
//                if (p.CountSpecialPower > 0)
//                {
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    string argtname27 = "スペシャルパワー";
//                    upic.Print(Expression.Term(ref argtname27, ref u, 18) + " ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    if (!is_unknown)
//                    {
//                        var loopTo = p.CountSpecialPower;
//                        for (i = 1; i <= loopTo; i++)
//                        {
//                            short localSpecialPowerCost() { string argsname = p.get_SpecialPower(i); var ret = p.SpecialPowerCost(ref argsname); p.get_SpecialPower(i) = argsname; return ret; }

//                            if (p.SP < localSpecialPowerCost())
//                            {
//                                // MOD START 240a
//                                // upic.ForeColor = rgb(150, 0, 0)
//                                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                // MOD  END  240a

//                            }
//                            SpecialPowerData localItem() { object argIndex1 = p.get_SpecialPower(i); var ret = SRC.SPDList.Item(ref argIndex1); p.get_SpecialPower(i) = Conversions.ToString(argIndex1); return ret; }

//                            upic.Print(localItem().ShortName);
//                            // MOD START 240a
//                            // upic.ForeColor = rgb(0, 0, 0)
//                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                            // MOD  END  240a
//                        }
//                    }
//                    else
//                    {
//                        upic.Print("？");
//                    }
//                    upic.Print();
//                }

//                // 未識別のユニットはこれ以降の情報を表示しない
//                if (is_unknown)
//                {
//                    upic.CurrentY = upic.CurrentY + 8;
//                    goto UnitStatus;
//                }

//                // パイロット用特殊能力一覧
//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 霊力
//                if (p.MaxPlana() > 0)
//                {
//                    string argsname = "霊力";
//                    if (p.IsSkillAvailable(ref argsname))
//                    {
//                        object argIndex8 = "霊力";
//                        sname = p.SkillName(ref argIndex8);
//                    }
//                    else
//                    {
//                        // 追加パイロットは第１パイロットの霊力を代わりに使うので
//                        object argIndex9 = 1;
//                        object argIndex10 = "霊力";
//                        sname = u.Pilot(ref argIndex9).SkillName(ref argIndex10);
//                    }

//                    if (Strings.InStr(sname, "非表示") == 0)
//                    {
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(0, 0, 150)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                        // MOD  END  240a
//                        upic.Print(sname + " ");
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(0, 0, 0)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                        // MOD  END  240a
//                        if (u.PlanaLevel() < p.Plana)
//                        {
//                            // MOD START 240a
//                            // upic.ForeColor = rgb(150, 0, 0)
//                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                            // MOD  END  240a
//                        }
//                        upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.Plana) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.MaxPlana()));
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(0, 0, 0)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                        // MOD  END  240a
//                    }
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 同調率
//                if (p.SynchroRate() > 0)
//                {
//                    object argIndex12 = "同調率";
//                    if (Strings.InStr(p.SkillName(ref argIndex12), "非表示") == 0)
//                    {
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(0, 0, 150)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                        // MOD  END  240a
//                        object argIndex11 = "同調率";
//                        upic.Print(p.SkillName(ref argIndex11) + " ");
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(0, 0, 0)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                        // MOD  END  240a
//                        if (u.SyncLevel() < p.SynchroRate())
//                        {
//                            // MOD START 240a
//                            // upic.ForeColor = rgb(150, 0, 0)
//                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                            // MOD  END  240a
//                        }
//                        upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(p.SynchroRate()) + "%");
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(0, 0, 0)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                        // MOD  END  240a
//                    }
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 得意技＆不得手
//                n = 0;
//                string argsname1 = "得意技";
//                if (p.IsSkillAvailable(ref argsname1))
//                {
//                    n = (short)(n + 1);
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("得意技 ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    string localRightPaddedString12() { object argIndex1 = "得意技"; string argbuf = p.SkillData(ref argIndex1); var ret = GeneralLib.RightPaddedString(ref argbuf, 12); return ret; }

//                    upic.Print(localRightPaddedString12());
//                }

//                string argsname2 = "不得手";
//                if (p.IsSkillAvailable(ref argsname2))
//                {
//                    n = (short)(n + 1);
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("不得手 ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    object argIndex13 = "不得手";
//                    upic.Print(p.SkillData(ref argIndex13));
//                }

//                if (n > 0)
//                {
//                    upic.Print();
//                }

//                // 表示するパイロット能力のリストを作成
//                name_list = new string[(p.CountSkill() + 1)];
//                var loopTo1 = p.CountSkill();
//                for (i = 1; i <= loopTo1; i++)
//                {
//                    object argIndex14 = i;
//                    name_list[i] = p.Skill(ref argIndex14);
//                }
//                // 付加されたパイロット特殊能力
//                var loopTo2 = u.CountCondition();
//                for (i = 1; i <= loopTo2; i++)
//                {
//                    object argIndex15 = i;
//                    if (u.ConditionLifetime(ref argIndex15) != 0)
//                    {
//                        string localCondition2() { object argIndex1 = i; var ret = u.Condition(ref argIndex1); return ret; }

//                        switch (Strings.Right(localCondition2(), 3) ?? "")
//                        {
//                            case "付加２":
//                            case "強化２":
//                                {
//                                    string localConditionData() { object argIndex1 = i; var ret = u.ConditionData(ref argIndex1); return ret; }

//                                    string arglist = localConditionData();
//                                    switch (GeneralLib.LIndex(ref arglist, 1) ?? "")
//                                    {
//                                        // 非表示の能力
//                                        case "非表示":
//                                        case "解説":
//                                            {
//                                                break;
//                                            }

//                                        default:
//                                            {
//                                                string localCondition() { object argIndex1 = i; var ret = u.Condition(ref argIndex1); return ret; }

//                                                string localCondition1() { object argIndex1 = i; var ret = u.Condition(ref argIndex1); return ret; }

//                                                stype = Strings.Left(localCondition(), Strings.Len(localCondition1()) - 3);
//                                                switch (stype ?? "")
//                                                {
//                                                    case "ハンター":
//                                                    case "ＳＰ消費減少":
//                                                    case "スペシャルパワー自動発動":
//                                                        {
//                                                            // 重複可能な能力
//                                                            Array.Resize(ref name_list, Information.UBound(name_list) + 1 + 1);
//                                                            name_list[Information.UBound(name_list)] = stype;
//                                                            break;
//                                                        }

//                                                    default:
//                                                        {
//                                                            // 既に所有している能力であればスキップ
//                                                            var loopTo3 = (short)Information.UBound(name_list);
//                                                            for (j = 1; j <= loopTo3; j++)
//                                                            {
//                                                                if ((stype ?? "") == (name_list[j] ?? ""))
//                                                                {
//                                                                    break;
//                                                                }
//                                                            }

//                                                            if (j > Information.UBound(name_list))
//                                                            {
//                                                                Array.Resize(ref name_list, Information.UBound(name_list) + 1 + 1);
//                                                                name_list[Information.UBound(name_list)] = stype;
//                                                            }

//                                                            break;
//                                                        }
//                                                }

//                                                break;
//                                            }
//                                    }

//                                    break;
//                                }
//                        }
//                    }
//                }

//                // パイロット能力を表示
//                n = 0;
//                var loopTo4 = (short)Information.UBound(name_list);
//                for (i = 1; i <= loopTo4; i++)
//                {
//                    // ADD START 240a
//                    // 文字色をリセット
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // ADD  END  240a
//                    stype = name_list[i];
//                    if (i <= p.CountSkill())
//                    {
//                        object argIndex16 = i;
//                        sname = p.SkillName(ref argIndex16);
//                        double localSkillLevel() { object argIndex1 = i; string argref_mode = ""; var ret = p.SkillLevel(ref argIndex1, ref_mode: ref argref_mode); return ret; }

//                        double localSkillLevel1() { object argIndex1 = i; string argref_mode = ""; var ret = p.SkillLevel(ref argIndex1, ref_mode: ref argref_mode); return ret; }

//                        slevel = localSkillLevel1().ToString();
//                    }
//                    else
//                    {
//                        object argIndex17 = stype;
//                        sname = p.SkillName(ref argIndex17);
//                        double localSkillLevel2() { object argIndex1 = stype; string argref_mode = ""; var ret = p.SkillLevel(ref argIndex1, ref_mode: ref argref_mode); return ret; }

//                        double localSkillLevel3() { object argIndex1 = stype; string argref_mode = ""; var ret = p.SkillLevel(ref argIndex1, ref_mode: ref argref_mode); return ret; }

//                        slevel = localSkillLevel3().ToString();
//                    }

//                    if (Strings.InStr(sname, "非表示") > 0)
//                    {
//                        goto NextSkill;
//                    }

//                    switch (stype ?? "")
//                    {
//                        case "オーラ":
//                            {
//                                if (DisplayedPilotInd == 1)
//                                {
//                                    if (u.AuraLevel(true) < u.AuraLevel() & !string.IsNullOrEmpty(Map.MapFileName))
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = rgb(150, 0, 0)
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                        // MOD  END  240a
//                                    }

//                                    if (u.AuraLevel(true) > Conversions.ToDouble(slevel))
//                                    {
//                                        sname = sname + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(u.AuraLevel(true) - Conversions.ToDouble(slevel));
//                                    }
//                                }

//                                break;
//                            }

//                        case "超能力":
//                            {
//                                if (DisplayedPilotInd == 1)
//                                {
//                                    if (u.PsychicLevel(true) < u.PsychicLevel() & !string.IsNullOrEmpty(Map.MapFileName))
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = rgb(150, 0, 0)
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                        // MOD  END  240a
//                                    }

//                                    if (u.PsychicLevel(true) > Conversions.ToDouble(slevel))
//                                    {
//                                        sname = sname + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(u.PsychicLevel(true) - Conversions.ToDouble(slevel));
//                                    }
//                                }

//                                break;
//                            }

//                        case "底力":
//                        case "超底力":
//                        case "覚悟":
//                            {
//                                if (u.HP <= u.MaxHP / 4)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = vbBlue
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "不屈":
//                            {
//                                if (u.HP <= u.MaxHP / 2)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = vbBlue
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "潜在力開放":
//                            {
//                                if (p.Morale >= 130)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = vbBlue
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "スペシャルパワー自動発動":
//                            {
//                                if (i <= p.CountSkill())
//                                {
//                                    string localSkillData() { object argIndex1 = i; var ret = p.SkillData(ref argIndex1); return ret; }

//                                    string localLIndex() { string arglist = hs4c6b0a8a8b874cdb8c87b94121d03278(); var ret = GeneralLib.LIndex(ref arglist, 3); return ret; }

//                                    int localStrToLng() { string argexpr = hs23c31b1421414ea0be207e3875c77613(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                                    if (p.Morale >= localStrToLng())
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = vbBlue
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                                        // MOD  END  240a
//                                    }
//                                }
//                                else
//                                {
//                                    string localSkillData1() { object argIndex1 = stype; var ret = p.SkillData(ref argIndex1); return ret; }

//                                    string localLIndex1() { string arglist = hs5a6fb85244614ee5b61690cdbec6b0f9(); var ret = GeneralLib.LIndex(ref arglist, 3); return ret; }

//                                    int localStrToLng1() { string argexpr = hsa5a03d2c426146729603becc658cd7bc(); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                                    if (p.Morale >= localStrToLng1())
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = vbBlue
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                                        // MOD  END  240a
//                                    }
//                                }

//                                break;
//                            }

//                        case "Ｓ防御":
//                            {
//                                string argfname2 = "シールド";
//                                string argfname3 = "大型シールド";
//                                string argfname4 = "小型シールド";
//                                string argfname5 = "エネルギーシールド";
//                                string argfname6 = "アクティブシールド";
//                                string argfname7 = "盾";
//                                string argfname8 = "バリアシールド";
//                                string argfname9 = "アクティブフィールド";
//                                string argfname10 = "アクティブプロテクション";
//                                object argIndex18 = "阻止";
//                                object argIndex19 = "広域阻止";
//                                object argIndex20 = "反射";
//                                object argIndex21 = "当て身技";
//                                object argIndex22 = "自動反撃";
//                                if (!u.IsFeatureAvailable(ref argfname2) & !u.IsFeatureAvailable(ref argfname3) & !u.IsFeatureAvailable(ref argfname4) & !u.IsFeatureAvailable(ref argfname5) & !u.IsFeatureAvailable(ref argfname6) & !u.IsFeatureAvailable(ref argfname7) & !u.IsFeatureAvailable(ref argfname8) & !u.IsFeatureAvailable(ref argfname9) & !u.IsFeatureAvailable(ref argfname10) & Strings.InStr(u.FeatureData(ref argIndex18), "Ｓ防御") == 0 & Strings.InStr(u.FeatureData(ref argIndex19), "Ｓ防御") == 0 & Strings.InStr(u.FeatureData(ref argIndex20), "Ｓ防御") == 0 & Strings.InStr(u.FeatureData(ref argIndex21), "Ｓ防御") == 0 & Strings.InStr(u.FeatureData(ref argIndex22), "Ｓ防御") == 0 & !string.IsNullOrEmpty(Map.MapFileName))
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "切り払い":
//                            {
//                                var loopTo5 = u.CountWeapon();
//                                for (j = 1; j <= loopTo5; j++)
//                                {
//                                    string argattr = "武";
//                                    if (u.IsWeaponClassifiedAs(j, ref argattr))
//                                    {
//                                        if (!u.IsDisabled(ref u.Weapon(j).Name))
//                                        {
//                                            break;
//                                        }
//                                    }
//                                }

//                                string argfname11 = "格闘武器";
//                                if (u.IsFeatureAvailable(ref argfname11))
//                                {
//                                    j = 0;
//                                }

//                                object argIndex23 = "阻止";
//                                object argIndex24 = "広域阻止";
//                                object argIndex25 = "反射";
//                                object argIndex26 = "当て身技";
//                                object argIndex27 = "自動反撃";
//                                if (j > u.CountWeapon() & Strings.InStr(u.FeatureData(ref argIndex23), "切り払い") == 0 & Strings.InStr(u.FeatureData(ref argIndex24), "切り払い") == 0 & Strings.InStr(u.FeatureData(ref argIndex25), "切り払い") == 0 & Strings.InStr(u.FeatureData(ref argIndex26), "切り払い") == 0 & Strings.InStr(u.FeatureData(ref argIndex27), "切り払い") == 0 & !string.IsNullOrEmpty(Map.MapFileName))
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "迎撃":
//                            {
//                                var loopTo6 = u.CountWeapon();
//                                for (j = 1; j <= loopTo6; j++)
//                                {
//                                    string argref_mode = "移動後";
//                                    string argattr1 = "射撃系";
//                                    if (u.IsWeaponAvailable(j, ref argref_mode) & u.IsWeaponClassifiedAs(j, ref argattr1) & (u.Weapon(j).Bullet >= 10 | u.Weapon(j).Bullet == 0 & u.Weapon(j).ENConsumption <= 5))
//                                    {
//                                        break;
//                                    }
//                                }

//                                string argfname12 = "迎撃武器";
//                                if (u.IsFeatureAvailable(ref argfname12))
//                                {
//                                    j = 0;
//                                }

//                                object argIndex28 = "阻止";
//                                object argIndex29 = "広域阻止";
//                                object argIndex30 = "反射";
//                                object argIndex31 = "当て身技";
//                                object argIndex32 = "自動反撃";
//                                if (j > u.CountWeapon() & Strings.InStr(u.FeatureData(ref argIndex28), "迎撃") == 0 & Strings.InStr(u.FeatureData(ref argIndex29), "迎撃") == 0 & Strings.InStr(u.FeatureData(ref argIndex30), "迎撃") == 0 & Strings.InStr(u.FeatureData(ref argIndex31), "迎撃") == 0 & Strings.InStr(u.FeatureData(ref argIndex32), "迎撃") == 0 & !string.IsNullOrEmpty(Map.MapFileName))
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "浄化":
//                            {
//                                var loopTo7 = u.CountWeapon();
//                                for (j = 1; j <= loopTo7; j++)
//                                {
//                                    string argattr2 = "浄";
//                                    if (u.IsWeaponClassifiedAs(j, ref argattr2))
//                                    {
//                                        if (!u.IsDisabled(ref u.Weapon(j).Name))
//                                        {
//                                            break;
//                                        }
//                                    }
//                                }

//                                if (j > u.CountWeapon() & !string.IsNullOrEmpty(Map.MapFileName))
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "援護":
//                            {
//                                if (!string.IsNullOrEmpty(Map.MapFileName))
//                                {
//                                    if ((u.Party ?? "") == (SRC.Stage ?? ""))
//                                    {
//                                        ret = GeneralLib.MaxLng(u.MaxSupportAttack() - u.UsedSupportAttack, 0);
//                                    }
//                                    else
//                                    {
//                                        string argsptype = "サポートガード不能";
//                                        if (u.IsUnderSpecialPowerEffect(ref argsptype))
//                                        {
//                                            // MOD START 240a
//                                            // upic.ForeColor = rgb(150, 0, 0)
//                                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                            // MOD  END  240a
//                                        }

//                                        ret = GeneralLib.MaxLng(u.MaxSupportGuard() - u.UsedSupportGuard, 0);
//                                    }

//                                    if (ret == 0)
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = rgb(150, 0, 0)
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                        // MOD  END  240a
//                                    }

//                                    sname = sname + " (残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(ret) + "回)";
//                                }

//                                break;
//                            }

//                        case "援護攻撃":
//                            {
//                                if (!string.IsNullOrEmpty(Map.MapFileName))
//                                {
//                                    ret = GeneralLib.MaxLng(u.MaxSupportAttack() - u.UsedSupportAttack, 0);
//                                    if (ret == 0)
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = rgb(150, 0, 0)
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                        // MOD  END  240a
//                                    }

//                                    sname = sname + " (残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(ret) + "回)";
//                                }

//                                break;
//                            }

//                        case "援護防御":
//                            {
//                                if (!string.IsNullOrEmpty(Map.MapFileName))
//                                {
//                                    ret = GeneralLib.MaxLng(u.MaxSupportGuard() - u.UsedSupportGuard, 0);
//                                    string argsptype1 = "サポートガード不能";
//                                    if (ret == 0 | u.IsUnderSpecialPowerEffect(ref argsptype1))
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = rgb(150, 0, 0)
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                        // MOD  END  240a
//                                    }

//                                    sname = sname + " (残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(ret) + "回)";
//                                }

//                                break;
//                            }

//                        case "統率":
//                            {
//                                if (!string.IsNullOrEmpty(Map.MapFileName))
//                                {
//                                    ret = GeneralLib.MaxLng(u.MaxSyncAttack() - u.UsedSyncAttack, 0);
//                                    if (ret == 0)
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = rgb(150, 0, 0)
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                        // MOD  END  240a
//                                    }

//                                    sname = sname + " (残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(ret) + "回)";
//                                }

//                                break;
//                            }

//                        case "カウンター":
//                            {
//                                if (!string.IsNullOrEmpty(Map.MapFileName))
//                                {
//                                    ret = GeneralLib.MaxLng(u.MaxCounterAttack() - u.UsedCounterAttack, 0);
//                                    if (ret > 100)
//                                    {
//                                        sname = sname + " (残り∞回)";
//                                    }
//                                    else if (ret > 0)
//                                    {
//                                        sname = sname + " (残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(ret) + "回)";
//                                    }
//                                    else
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = rgb(150, 0, 0)
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                        // MOD  END  240a
//                                        sname = sname + " (残り0回)";
//                                    }
//                                }

//                                break;
//                            }

//                        case "先手必勝":
//                            {
//                                if (u.MaxCounterAttack() > 100)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = vbBlue
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "耐久":
//                            {
//                                string argoname3 = "防御力成長";
//                                string argoname4 = "防御力レベルアップ";
//                                if (Expression.IsOptionDefined(ref argoname3) | Expression.IsOptionDefined(ref argoname4))
//                                {
//                                    goto NextSkill;
//                                }

//                                break;
//                            }

//                        case "霊力":
//                        case "同調率":
//                        case "得意技":
//                        case "不得手":
//                            {
//                                goto NextSkill;
//                                break;
//                            }
//                    }

//                    // 特殊能力名を表示
//                    if (LenB(Strings.StrConv(sname, vbFromUnicode)) > 19)
//                    {
//                        if (n > 0)
//                        {
//                            upic.Print();
//                            // ADD START 240a
//                            if (GUI.NewGUIMode)
//                            {
//                                upic.CurrentX = 5;
//                            }
//                            // ADD  END  240a
//                        }
//                        upic.Print(sname);
//                        n = 2;
//                    }
//                    else
//                    {
//                        upic.Print(GeneralLib.RightPaddedString(ref sname, 19));
//                        n = (short)(n + 1);
//                    }

//                    upic.ForeColor = Color.Black;

//                    // 必要に応じて改行
//                    if (n > 1)
//                    {
//                        upic.Print();
//                        // ADD START 240a
//                        if (GUI.NewGUIMode)
//                        {
//                            upic.CurrentX = 5;
//                        }
//                        // ADD  END  240a
//                        n = 0;
//                    }

//                    NextSkill:
//                    ;
//                }

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

//                upic.CurrentY = upic.CurrentY + 8;
//                UnitStatus:
//                ;


//                // パイロットステータス表示用のダミーユニットの場合はここで表示を終了
//                string argfname13 = "ダミーユニット";
//                if (withBlock.IsFeatureAvailable(ref argfname13))
//                {
//                    goto UpdateStatusWindow;
//                }

//                // ここからはユニットに関する情報

//                // ユニット愛称
//                upic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeSize(upic.Font, 10.5f);
//                upic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeBold(upic.Font, false);
//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                    // 文字色をリセット
//                    upic.ForeColor = ColorTranslator.FromOle(StatusFontColorNormalString);
//                }
//                // ADD  END  240a
//                upic.Print(withBlock.Nickname0);
//                upic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeBold(upic.Font, false);
//                upic.Font = Microsoft.VisualBasic.Compatibility.VB6.Support.FontChangeSize(upic.Font, 9f);
//                if (withBlock.Status_Renamed == "出撃" & !string.IsNullOrEmpty(Map.MapFileName))
//                {

//                    // 地形情報の表示

//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    // ユニットの位置を地形名称
//                    if (Strings.InStr(Map.TerrainName(withBlock.x, withBlock.y), "(") > 0)
//                    {
//                        upic.Print(withBlock.Area + " (" + Strings.Left(Map.TerrainName(withBlock.x, withBlock.y), Strings.InStr(Map.TerrainName(withBlock.x, withBlock.y), "(") - 1));
//                    }
//                    else
//                    {
//                        upic.Print(withBlock.Area + " (" + Map.TerrainName(withBlock.x, withBlock.y));
//                    }

//                    // 回避＆防御修正
//                    if (Map.TerrainEffectForHit(withBlock.x, withBlock.y) == Map.TerrainEffectForDamage(withBlock.x, withBlock.y))
//                    {
//                        if (Map.TerrainEffectForHit(withBlock.x, withBlock.y) >= 0)
//                        {
//                            upic.Print(" 回＆防+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForHit(withBlock.x, withBlock.y)) + "%");
//                        }
//                        else
//                        {
//                            upic.Print(" 回＆防" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForHit(withBlock.x, withBlock.y)) + "%");
//                        }
//                    }
//                    else
//                    {
//                        if (Map.TerrainEffectForHit(withBlock.x, withBlock.y) >= 0)
//                        {
//                            upic.Print(" 回+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForHit(withBlock.x, withBlock.y)) + "%");
//                        }
//                        else
//                        {
//                            upic.Print(" 回" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForHit(withBlock.x, withBlock.y)) + "%");
//                        }

//                        if (Map.TerrainEffectForDamage(withBlock.x, withBlock.y) >= 0)
//                        {
//                            upic.Print(" 防+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForDamage(withBlock.x, withBlock.y)) + "%");
//                        }
//                        else
//                        {
//                            upic.Print(" 防" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForDamage(withBlock.x, withBlock.y)) + "%");
//                        }
//                    }

//                    // ＨＰ＆ＥＮ回復
//                    if (Map.TerrainEffectForHPRecover(withBlock.x, withBlock.y) > 0)
//                    {
//                        string argtname28 = "ＨＰ";
//                        Unit argu = null;
//                        upic.Print(" " + Strings.Left(Expression.Term(ref argtname28, u: ref argu), 1) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForHPRecover(withBlock.x, withBlock.y)) + "%");
//                    }

//                    if (Map.TerrainEffectForENRecover(withBlock.x, withBlock.y) > 0)
//                    {
//                        string argtname29 = "ＥＮ";
//                        Unit argu1 = null;
//                        upic.Print(" " + Strings.Left(Expression.Term(ref argtname29, u: ref argu1), 1) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Map.TerrainEffectForENRecover(withBlock.x, withBlock.y)) + "%");
//                    }

//                    // MOD START 240a
//                    // Set td = TDList.Item(MapData(.X, .Y, 0))
//                    // マスのタイプに応じて参照先を変更
//                    switch (Map.MapData[withBlock.x, withBlock.y, Map.MapDataIndex.BoxType])
//                    {
//                        case (short)Map.BoxTypes.Under:
//                        case (short)Map.BoxTypes.UpperBmpOnly:
//                            {
//                                td = SRC.TDList.Item(Map.MapData[withBlock.x, withBlock.y, Map.MapDataIndex.TerrainType]);
//                                break;
//                            }

//                        default:
//                            {
//                                td = SRC.TDList.Item(Map.MapData[withBlock.x, withBlock.y, Map.MapDataIndex.LayerType]);
//                                break;
//                            }
//                    }
//                    // MOD START 240a
//                    // ＨＰ＆ＥＮ減少
//                    string argfname14 = "ＨＰ減少";
//                    if (td.IsFeatureAvailable(ref argfname14))
//                    {
//                        string argtname30 = "ＨＰ";
//                        Unit argu2 = null;
//                        object argIndex33 = "ＨＰ減少";
//                        upic.Print(" " + Strings.Left(Expression.Term(ref argtname30, u: ref argu2), 1) + "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * td.FeatureLevel(ref argIndex33)) + "%");
//                    }

//                    string argfname15 = "ＥＮ減少";
//                    if (td.IsFeatureAvailable(ref argfname15))
//                    {
//                        string argtname31 = "ＥＮ";
//                        Unit argu3 = null;
//                        object argIndex34 = "ＥＮ減少";
//                        upic.Print(" " + Strings.Left(Expression.Term(ref argtname31, u: ref argu3), 1) + "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * td.FeatureLevel(ref argIndex34)) + "%");
//                    }

//                    // ＨＰ＆ＥＮ増加
//                    string argfname16 = "ＨＰ増加";
//                    if (td.IsFeatureAvailable(ref argfname16))
//                    {
//                        string argtname32 = "ＨＰ";
//                        Unit argu4 = null;
//                        object argIndex35 = "ＨＰ増加";
//                        upic.Print(" " + Strings.Left(Expression.Term(ref argtname32, u: ref argu4), 1) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(1000d * td.FeatureLevel(ref argIndex35)));
//                    }

//                    string argfname17 = "ＥＮ増加";
//                    if (td.IsFeatureAvailable(ref argfname17))
//                    {
//                        string argtname33 = "ＥＮ";
//                        Unit argu5 = null;
//                        object argIndex36 = "ＥＮ増加";
//                        upic.Print(" " + Strings.Left(Expression.Term(ref argtname33, u: ref argu5), 1) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * td.FeatureLevel(ref argIndex36)));
//                    }

//                    // ＨＰ＆ＥＮ低下
//                    string argfname18 = "ＨＰ低下";
//                    if (td.IsFeatureAvailable(ref argfname18))
//                    {
//                        string argtname34 = "ＨＰ";
//                        Unit argu6 = null;
//                        object argIndex37 = "ＨＰ低下";
//                        upic.Print(" " + Strings.Left(Expression.Term(ref argtname34, u: ref argu6), 1) + "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(1000d * td.FeatureLevel(ref argIndex37)));
//                    }

//                    string argfname19 = "ＥＮ低下";
//                    if (td.IsFeatureAvailable(ref argfname19))
//                    {
//                        string argtname35 = "ＥＮ";
//                        Unit argu7 = null;
//                        object argIndex38 = "ＥＮ低下";
//                        upic.Print(" " + Strings.Left(Expression.Term(ref argtname35, u: ref argu7), 1) + "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(10d * td.FeatureLevel(ref argIndex38)));
//                    }

//                    // 摩擦
//                    string argfname20 = "摩擦";
//                    if (td.IsFeatureAvailable(ref argfname20))
//                    {
//                        object argIndex39 = "摩擦";
//                        upic.Print(" 摩L" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(td.FeatureLevel(ref argIndex39)));
//                    }

//                    upic.Print(")");
//                }
//                else
//                {
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("ランク ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.Rank));
//                }

//                // 未確認ユニット？
//                if (is_unknown)
//                {
//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    // ＨＰ
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    string argtname36 = "ＨＰ";
//                    Unit argu8 = null;
//                    upic.Print(Expression.Term(ref argtname36, ref argu8, 6) + " ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print("?????/?????");

//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    // ＥＮ
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    string argtname37 = "ＥＮ";
//                    Unit argu9 = null;
//                    upic.Print(Expression.Term(ref argtname37, ref argu9, 6) + " ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print("???/???");

//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    // 装甲
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    string argtname38 = "装甲";
//                    Unit argu10 = null;
//                    upic.Print(Expression.Term(ref argtname38, ref argu10, 6) + " ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    string argbuf14 = "？";
//                    upic.Print(GeneralLib.RightPaddedString(ref argbuf14, 12));

//                    // 運動性
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    string argtname39 = "運動性";
//                    Unit argu11 = null;
//                    upic.Print(Expression.Term(ref argtname39, ref argu11, 6) + " ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print("？");

//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    // 移動タイプ
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    string argtname40 = "タイプ";
//                    Unit argu12 = null;
//                    upic.Print(Expression.Term(ref argtname40, ref argu12, 6) + " ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    string argbuf15 = "？";
//                    upic.Print(GeneralLib.RightPaddedString(ref argbuf15, 12));

//                    // 移動力
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    string argtname41 = "移動力";
//                    Unit argu13 = null;
//                    upic.Print(Expression.Term(ref argtname41, ref argu13, 6) + " ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print("？");

//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    // 地形適応
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("適応   ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    string argbuf16 = "？";
//                    upic.Print(GeneralLib.RightPaddedString(ref argbuf16, 12));

//                    // ユニットサイズ
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    string argtname42 = "サイズ";
//                    Unit argu14 = null;
//                    upic.Print(Expression.Term(ref argtname42, ref argu14, 6) + " ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print("？");

//                    // サポートアタックを得られるかどうかのみ表示
//                    if ((Commands.CommandState == "ターゲット選択" | Commands.CommandState == "移動後ターゲット選択") & (Commands.SelectedCommand == "攻撃" | Commands.SelectedCommand == "マップ攻撃") & Commands.SelectedUnit is object)
//                    {
//                        object argIndex40 = "暴走";
//                        object argIndex41 = "魅了";
//                        object argIndex42 = "憑依";
//                        if (withBlock.Party == "敵" | withBlock.Party == "中立" | withBlock.IsConditionSatisfied(ref argIndex40) | withBlock.IsConditionSatisfied(ref argIndex41) | withBlock.IsConditionSatisfied(ref argIndex42))
//                        {
//                            upic.Print();

//                            // 攻撃手段
//                            // MOD START 240a
//                            // upic.ForeColor = rgb(0, 0, 150)
//                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                            // MOD  END  240a
//                            upic.Print("攻撃     ");
//                            // MOD START 240a
//                            // upic.ForeColor = rgb(0, 0, 0)
//                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                            // MOD  END  240a
//                            upic.Print(Commands.SelectedUnit.WeaponNickname(Commands.SelectedWeapon));
//                            // サポートアタックを得られる？
//                            string argattr3 = "合";
//                            string argattr4 = "Ｍ";
//                            if (!Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, ref argattr3) & !Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, ref argattr4))
//                            {
//                                if (Commands.SelectedUnit.LookForSupportAttack(ref u) is object)
//                                {
//                                    upic.Print(" [援]");
//                                }
//                            }
//                        }
//                    }

//                    goto UpdateStatusWindow;
//                }

//                // 実行中の命令
//                object argIndex46 = "混乱";
//                object argIndex47 = "恐怖";
//                object argIndex48 = "暴走";
//                object argIndex49 = "狂戦士";
//                if (withBlock.Party == "ＮＰＣ" & !withBlock.IsConditionSatisfied(ref argIndex46) & !withBlock.IsConditionSatisfied(ref argIndex47) & !withBlock.IsConditionSatisfied(ref argIndex48) & !withBlock.IsConditionSatisfied(ref argIndex49))
//                {
//                    // 思考モードを見れば実行している命令が分かるので……
//                    buf = "";
//                    object argIndex43 = "魅了";
//                    if (withBlock.IsConditionSatisfied(ref argIndex43))
//                    {
//                        if (withBlock.Master is object)
//                        {
//                            if (withBlock.Master.Party == "味方")
//                            {
//                                buf = withBlock.Mode;
//                            }
//                        }
//                    }

//                    string argfname21 = "召喚ユニット";
//                    object argIndex44 = "魅了";
//                    if (withBlock.IsFeatureAvailable(ref argfname21) & !withBlock.IsConditionSatisfied(ref argIndex44))
//                    {
//                        if (withBlock.Summoner is object)
//                        {
//                            if (withBlock.Summoner.Party == "味方")
//                            {
//                                buf = withBlock.Mode;
//                            }
//                        }
//                    }

//                    bool localIsDefined() { object argIndex1 = buf; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

//                    if (buf == "通常")
//                    {
//                        upic.Print("自由行動中");
//                    }
//                    else if (localIsDefined())
//                    {
//                        // 思考モードにパイロット名が指定されている場合
//                        object argIndex45 = buf;
//                        {
//                            var withBlock1 = SRC.PList.Item(ref argIndex45);
//                            if (withBlock1.Unit_Renamed is object)
//                            {
//                                {
//                                    var withBlock2 = withBlock1.Unit_Renamed;
//                                    if (withBlock2.Status_Renamed == "出撃")
//                                    {
//                                        upic.Print(withBlock2.Nickname + "(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock2.x) + "," + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock2.y) + ")を");
//                                        if (withBlock2.Party == "味方")
//                                        {
//                                            upic.Print("護衛中");
//                                        }
//                                        else
//                                        {
//                                            upic.Print("追跡中");
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                    else if (GeneralLib.LLength(ref buf) == 2)
//                    {
//                        // 思考モードに座標が指定されている場合
//                        upic.Print("(" + GeneralLib.LIndex(ref buf, 1) + "," + GeneralLib.LIndex(ref buf, 2) + ")に移動中");
//                    }
//                }

//                // ユニットにかかっている特殊ステータス
//                name_list = new string[1];
//                var loopTo8 = withBlock.CountCondition();
//                for (i = 1; i <= loopTo8; i++)
//                {
//                    // 時間切れ？
//                    object argIndex50 = i;
//                    if (withBlock.ConditionLifetime(ref argIndex50) == 0)
//                    {
//                        goto NextCondition;
//                    }

//                    // 非表示？
//                    string localConditionData1() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                    if (Strings.InStr(localConditionData1(), "非表示") > 0)
//                    {
//                        goto NextCondition;
//                    }

//                    // 解説？
//                    string localConditionData2() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                    string arglist1 = localConditionData2();
//                    if (GeneralLib.LIndex(ref arglist1, 1) == "解説")
//                    {
//                        goto NextCondition;
//                    }
//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    object argIndex60 = i;
//                    switch (withBlock.Condition(ref argIndex60) ?? "")
//                    {
//                        case "データ不明":
//                        case "形態固定":
//                        case "機体固定":
//                        case "不死身":
//                        case "無敵":
//                        case "識別済み":
//                        case "非操作":
//                        case "破壊キャンセル":
//                        case "盾ダメージ":
//                        case "能力コピー":
//                        case "メッセージ付加":
//                        case "ノーマルモード付加":
//                        case "追加パイロット付加":
//                        case "追加サポート付加":
//                        case "パイロット愛称付加":
//                        case "パイロット画像付加":
//                        case "性格変更付加":
//                        case "性別付加":
//                        case "ＢＧＭ付加":
//                        case "愛称変更付加":
//                        case "スペシャルパワー無効化":
//                        case "精神コマンド無効化":
//                        case "ユニット画像付加":
//                        case var case12 when case12 == "メッセージ付加":
//                            {
//                                break;
//                            }
//                        // 非表示
//                        case "残り時間":
//                            {
//                                short localConditionLifetime1() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime2() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime1() & localConditionLifetime2() < 100)
//                                {
//                                    short localConditionLifetime() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print("残り時間" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime()) + "ターン");
//                                }

//                                break;
//                            }

//                        case "無効化付加":
//                        case "耐性付加":
//                        case "吸収付加":
//                        case "弱点付加":
//                            {
//                                string localConditionData3() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                string localCondition3() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                upic.Print(localConditionData3() + localCondition3());
//                                short localConditionLifetime4() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime5() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime4() & localConditionLifetime5() < 100)
//                                {
//                                    short localConditionLifetime3() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime3()) + "T");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "特殊効果無効化付加":
//                            {
//                                string localConditionData4() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                upic.Print(localConditionData4() + "無効化付加");
//                                short localConditionLifetime7() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime8() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime7() & localConditionLifetime8() < 100)
//                                {
//                                    short localConditionLifetime6() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime6()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "攻撃属性付加":
//                            {
//                                string localConditionData5() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                string localLIndex2() { string arglist = hs755742c2c238431abd43e11d0920ad14(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

//                                upic.Print(localLIndex2() + "属性付加");
//                                short localConditionLifetime10() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime11() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime10() & localConditionLifetime11() < 100)
//                                {
//                                    short localConditionLifetime9() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime9()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "武器強化付加":
//                            {
//                                double localConditionLevel() { object argIndex1 = i; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

//                                upic.Print("武器強化Lv" + localConditionLevel() + "付加");
//                                object argIndex51 = i;
//                                if (!string.IsNullOrEmpty(withBlock.ConditionData(ref argIndex51)))
//                                {
//                                    string localConditionData6() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                    upic.Print("(" + localConditionData6() + ")");
//                                }

//                                short localConditionLifetime13() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime14() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime13() & localConditionLifetime14() < 100)
//                                {
//                                    short localConditionLifetime12() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime12()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "命中率強化付加":
//                            {
//                                double localConditionLevel1() { object argIndex1 = i; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

//                                upic.Print("命中率強化Lv" + localConditionLevel1() + "付加");
//                                object argIndex52 = i;
//                                if (!string.IsNullOrEmpty(withBlock.ConditionData(ref argIndex52)))
//                                {
//                                    string localConditionData7() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                    upic.Print("(" + localConditionData7() + ")");
//                                }

//                                short localConditionLifetime16() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime17() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime16() & localConditionLifetime17() < 100)
//                                {
//                                    short localConditionLifetime15() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime15()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "ＣＴ率強化付加":
//                            {
//                                double localConditionLevel2() { object argIndex1 = i; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

//                                upic.Print("ＣＴ率強化Lv" + localConditionLevel2() + "付加");
//                                object argIndex53 = i;
//                                if (!string.IsNullOrEmpty(withBlock.ConditionData(ref argIndex53)))
//                                {
//                                    string localConditionData8() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                    upic.Print("(" + localConditionData8() + ")");
//                                }

//                                short localConditionLifetime19() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime20() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime19() & localConditionLifetime20() < 100)
//                                {
//                                    short localConditionLifetime18() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime18()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "特殊効果発動率強化付加":
//                            {
//                                double localConditionLevel3() { object argIndex1 = i; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

//                                upic.Print("特殊効果発動率強化Lv" + localConditionLevel3() + "付加");
//                                object argIndex54 = i;
//                                if (!string.IsNullOrEmpty(withBlock.ConditionData(ref argIndex54)))
//                                {
//                                    string localConditionData9() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                    upic.Print("(" + localConditionData9() + ")");
//                                }

//                                short localConditionLifetime22() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime23() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime22() & localConditionLifetime23() < 100)
//                                {
//                                    short localConditionLifetime21() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime21()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "地形適応変更付加":
//                            {
//                                upic.Print("地形適応変更付加");
//                                short localConditionLifetime25() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime26() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime25() & localConditionLifetime26() < 100)
//                                {
//                                    short localConditionLifetime24() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime24()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "盾付加":
//                            {
//                                string localConditionData10() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                string localLIndex3() { string arglist = hsba8faef602a144028d0b911086dca487(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

//                                upic.Print(localLIndex3() + "付加");
//                                double localConditionLevel4() { object argIndex1 = i; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

//                                upic.Print("(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel4()) + ")");
//                                short localConditionLifetime28() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime29() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime28() & localConditionLifetime29() < 100)
//                                {
//                                    short localConditionLifetime27() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime27()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "ダミー破壊":
//                            {
//                                object argIndex55 = "ダミー";
//                                buf = withBlock.FeatureName(ref argIndex55);
//                                if (Strings.InStr(buf, "Lv") > 0)
//                                {
//                                    buf = Strings.Left(buf, Strings.InStr(buf, "Lv") - 1);
//                                }
//                                double localConditionLevel5() { object argIndex1 = i; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

//                                upic.Print(buf + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel5()), VbStrConv.Wide) + "体破壊");
//                                break;
//                            }

//                        case "ダミー付加":
//                            {
//                                double localConditionLevel6() { object argIndex1 = i; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

//                                object argIndex56 = "ダミー";
//                                upic.Print(withBlock.FeatureName(ref argIndex56) + "残り" + Strings.StrConv(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel6()), VbStrConv.Wide) + "体");
//                                short localConditionLifetime31() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime32() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime31() & localConditionLifetime32() < 100)
//                                {
//                                    short localConditionLifetime30() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime30()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "バリア発動":
//                            {
//                                object argIndex57 = i;
//                                if (!string.IsNullOrEmpty(withBlock.ConditionData(ref argIndex57)))
//                                {
//                                    string localConditionData11() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                    upic.Print(localConditionData11());
//                                }
//                                else
//                                {
//                                    upic.Print("バリア発動");
//                                }
//                                short localConditionLifetime33() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime33()) + "ターン");
//                                break;
//                            }

//                        case "フィールド発動":
//                            {
//                                object argIndex58 = i;
//                                if (!string.IsNullOrEmpty(withBlock.ConditionData(ref argIndex58)))
//                                {
//                                    string localConditionData12() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                    upic.Print(localConditionData12());
//                                }
//                                else
//                                {
//                                    upic.Print("フィールド発動");
//                                }
//                                short localConditionLifetime34() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime34()) + "ターン");
//                                break;
//                            }

//                        case "装甲劣化":
//                            {
//                                string argtname43 = "装甲";
//                                upic.Print(Expression.Term(ref argtname43, ref u) + "劣化");
//                                short localConditionLifetime36() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime37() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime36() & localConditionLifetime37() < 20)
//                                {
//                                    short localConditionLifetime35() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime35()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "運動性ＵＰ":
//                            {
//                                string argtname44 = "運動性";
//                                upic.Print(Expression.Term(ref argtname44, ref u) + "ＵＰ");
//                                short localConditionLifetime39() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime40() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime39() & localConditionLifetime40() < 20)
//                                {
//                                    short localConditionLifetime38() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime38()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "運動性ＤＯＷＮ":
//                            {
//                                string argtname45 = "運動性";
//                                upic.Print(Expression.Term(ref argtname45, ref u) + "ＤＯＷＮ");
//                                short localConditionLifetime42() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime43() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime42() & localConditionLifetime43() < 20)
//                                {
//                                    short localConditionLifetime41() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime41()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "移動力ＵＰ":
//                            {
//                                string argtname46 = "移動力";
//                                upic.Print(Expression.Term(ref argtname46, ref u) + "ＵＰ");
//                                short localConditionLifetime45() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime46() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime45() & localConditionLifetime46() < 20)
//                                {
//                                    short localConditionLifetime44() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime44()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        case "移動力ＤＯＷＮ":
//                            {
//                                string argtname47 = "移動力";
//                                upic.Print(Expression.Term(ref argtname47, ref u) + "ＤＯＷＮ");
//                                short localConditionLifetime48() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime49() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime48() & localConditionLifetime49() < 20)
//                                {
//                                    short localConditionLifetime47() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime47()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }

//                        default:
//                            {
//                                // 充填中？
//                                string localCondition7() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                if (Strings.Right(localCondition7(), 3) == "充填中")
//                                {
//                                    if (withBlock.IsHero())
//                                    {
//                                        string localCondition4() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                        string localCondition5() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                        upic.Print(Strings.Left(localCondition4(), Strings.Len(localCondition5()) - 3));
//                                        upic.Print("準備中");
//                                    }
//                                    else
//                                    {
//                                        string localCondition6() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                        upic.Print(localCondition6());
//                                    }
//                                    short localConditionLifetime50() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime50()) + "ターン");
//                                    goto NextCondition;
//                                }

//                                // パイロット特殊能力付加＆強化による状態は表示しない
//                                string localCondition8() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                string localCondition9() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                if (Strings.Right(localCondition8(), 3) == "付加２" | Strings.Right(localCondition9(), 3) == "強化２")
//                                {
//                                    goto NextCondition;
//                                }

//                                string localCondition12() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                string localConditionData15() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                string localCondition13() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                string localConditionData16() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                double localConditionLevel9() { object argIndex1 = i; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

//                                if (Strings.Right(localCondition12(), 2) == "付加" & !string.IsNullOrEmpty(localConditionData15()))
//                                {
//                                    string localConditionData13() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                    string arglist2 = localConditionData13();
//                                    buf = GeneralLib.LIndex(ref arglist2, 1) + "付加";
//                                }
//                                else if (Strings.Right(localCondition13(), 2) == "強化" & !string.IsNullOrEmpty(localConditionData16()))
//                                {
//                                    // 強化アビリティ
//                                    string localConditionData14() { object argIndex1 = i; var ret = withBlock.ConditionData(ref argIndex1); return ret; }

//                                    string localLIndex4() { string arglist = hs68f2e5d1358d41f987f02a7379e2b562(); var ret = GeneralLib.LIndex(ref arglist, 1); return ret; }

//                                    double localConditionLevel7() { object argIndex1 = i; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

//                                    buf = localLIndex4() + "強化Lv" + localConditionLevel7();
//                                }
//                                else if (localConditionLevel9() > 0d)
//                                {
//                                    // 付加アビリティ(レベル指定あり)
//                                    string localCondition10() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                    string localCondition11() { object argIndex1 = i; var ret = withBlock.Condition(ref argIndex1); return ret; }

//                                    double localConditionLevel8() { object argIndex1 = i; var ret = withBlock.ConditionLevel(ref argIndex1); return ret; }

//                                    buf = Strings.Left(localCondition10(), Strings.Len(localCondition11()) - 2) + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLevel8()) + "付加";
//                                }
//                                else
//                                {
//                                    // 付加アビリティ(レベル指定なし)
//                                    object argIndex59 = i;
//                                    buf = withBlock.Condition(ref argIndex59);
//                                }

//                                // エリアスされた特殊能力の付加表示がたぶらないように
//                                var loopTo9 = (short)Information.UBound(name_list);
//                                for (j = 1; j <= loopTo9; j++)
//                                {
//                                    if ((buf ?? "") == (name_list[j] ?? ""))
//                                    {
//                                        break;
//                                    }
//                                }

//                                if (j <= Information.UBound(name_list))
//                                {
//                                    goto NextCondition;
//                                }

//                                Array.Resize(ref name_list, Information.UBound(name_list) + 1 + 1);
//                                name_list[Information.UBound(name_list)] = buf;

//                                upic.Print(buf);
//                                short localConditionLifetime52() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                short localConditionLifetime53() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                if (0 < localConditionLifetime52() & localConditionLifetime53() < 20)
//                                {
//                                    short localConditionLifetime51() { object argIndex1 = i; var ret = withBlock.ConditionLifetime(ref argIndex1); return ret; }

//                                    upic.Print(" 残り" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(localConditionLifetime51()) + "ターン");
//                                }
//                                upic.Print("");
//                                break;
//                            }
//                    }

//                    NextCondition:
//                    ;
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // ＨＰ
//                cx = upic.CurrentX;
//                cy = upic.CurrentY;
//                upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                upic.Line(117, cy + 8); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                upic.Line(118 + GUI.GauageWidth, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                if (withBlock.HP > 0)
//                {
//                    upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                }
//                upic.CurrentX = cx;
//                upic.CurrentY = cy;
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname48 = "ＨＰ";
//                upic.Print(Expression.Term(ref argtname48, ref u, 6) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                object argIndex61 = "データ不明";
//                if (withBlock.IsConditionSatisfied(ref argIndex61))
//                {
//                    upic.Print("?????/?????");
//                }
//                else
//                {
//                    if (withBlock.HP < 100000)
//                    {
//                        upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.HP));
//                    }
//                    else
//                    {
//                        upic.Print("?????");
//                    }
//                    upic.Print("/");
//                    if (withBlock.MaxHP < 100000)
//                    {
//                        upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.MaxHP));
//                    }
//                    else
//                    {
//                        upic.Print("?????");
//                    }
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // ＥＮ
//                cx = upic.CurrentX;
//                cy = upic.CurrentY;
//                upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                upic.Line(116, cy + 2); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                upic.Line(117, cy + 8); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                upic.Line(118 + GUI.GauageWidth, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                if (withBlock.EN > 0)
//                {
//                    upic.Line(117, cy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
//                }
//                upic.CurrentX = cx;
//                upic.CurrentY = cy;
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname49 = "ＥＮ";
//                upic.Print(Expression.Term(ref argtname49, ref u, 6) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                object argIndex62 = "データ不明";
//                if (withBlock.IsConditionSatisfied(ref argIndex62))
//                {
//                    upic.Print("???/???");
//                }
//                else
//                {
//                    if (withBlock.EN < 1000)
//                    {
//                        upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.EN));
//                    }
//                    else
//                    {
//                        upic.Print("???");
//                    }
//                    upic.Print("/");
//                    if (withBlock.MaxEN < 1000)
//                    {
//                        upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.MaxEN));
//                    }
//                    else
//                    {
//                        upic.Print("???");
//                    }
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 装甲
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname50 = "装甲";
//                upic.Print(Expression.Term(ref argtname50, ref u, 6) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                switch (withBlock.get_Armor("修正値"))
//                {
//                    case var case13 when case13 > 0:
//                        {
//                            string localRightPaddedString13() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.get_Armor("基本値")) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.get_Armor("修正値")); var ret = GeneralLib.RightPaddedString(ref argbuf, 12); return ret; }

//                            upic.Print(localRightPaddedString13());
//                            break;
//                        }

//                    case var case14 when case14 < 0:
//                        {
//                            string localRightPaddedString14() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.get_Armor("基本値")) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.get_Armor("修正値")); var ret = GeneralLib.RightPaddedString(ref argbuf, 12); return ret; }

//                            upic.Print(localRightPaddedString14());
//                            break;
//                        }

//                    case 0:
//                        {
//                            string localRightPaddedString15() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.get_Armor("")); var ret = GeneralLib.RightPaddedString(ref argbuf, 12); return ret; }

//                            upic.Print(localRightPaddedString15());
//                            break;
//                        }
//                }

//                // 運動性
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname51 = "運動性";
//                upic.Print(Expression.Term(ref argtname51, ref u, 6) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                switch (withBlock.get_Mobility("修正値"))
//                {
//                    case var case15 when case15 > 0:
//                        {
//                            upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.get_Mobility("基本値")) + "+" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.get_Mobility("修正値")));
//                            break;
//                        }

//                    case var case16 when case16 < 0:
//                        {
//                            upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.get_Mobility("基本値")) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.get_Mobility("修正値")));
//                            break;
//                        }

//                    case 0:
//                        {
//                            upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.get_Mobility("")));
//                            break;
//                        }
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 移動タイプ
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname52 = "タイプ";
//                upic.Print(Expression.Term(ref argtname52, ref u, 6) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                string localRightPaddedString16() { string argbuf = withBlock.Transportation; var ret = GeneralLib.RightPaddedString(ref argbuf, 12); withBlock.Transportation = argbuf; return ret; }

//                upic.Print(localRightPaddedString16());

//                // 移動力
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname53 = "移動力";
//                upic.Print(Expression.Term(ref argtname53, ref u, 6) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                string localLIndex5() { object argIndex1 = "テレポート"; string arglist = withBlock.FeatureData(ref argIndex1); var ret = GeneralLib.LIndex(ref arglist, 2); return ret; }

//                string argfname22 = "テレポート";
//                if (withBlock.IsFeatureAvailable(ref argfname22) & (withBlock.Data.Speed == 0 | localLIndex5() == "0"))
//                {
//                    object argIndex63 = "テレポート";
//                    upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.Speed + withBlock.FeatureLevel(ref argIndex63)));
//                }
//                else
//                {
//                    upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.Speed));
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 地形適応
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                upic.Print("適応   ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                for (i = 1; i <= 4; i++)
//                {
//                    switch (withBlock.get_Adaption(i))
//                    {
//                        case 5:
//                            {
//                                upic.Print("S");
//                                break;
//                            }

//                        case 4:
//                            {
//                                upic.Print("A");
//                                break;
//                            }

//                        case 3:
//                            {
//                                upic.Print("B");
//                                break;
//                            }

//                        case 2:
//                            {
//                                upic.Print("C");
//                                break;
//                            }

//                        case 1:
//                            {
//                                upic.Print("D");
//                                break;
//                            }

//                        default:
//                            {
//                                upic.Print("E");
//                                break;
//                            }
//                    }
//                }
//                upic.Print(Strings.Space(8));

//                // ユニットサイズ
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                string argtname54 = "サイズ";
//                upic.Print(Expression.Term(ref argtname54, ref u, 6) + " ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                upic.Print(Strings.StrConv(withBlock.Size, VbStrConv.Wide));

//                // 防御属性の表示
//                n = 0;

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 吸収
//                if (Strings.Len(withBlock.strAbsorb) > 0 & Strings.InStr(withBlock.strAbsorb, "非表示") == 0)
//                {
//                    if (Strings.Len(withBlock.strAbsorb) > 5)
//                    {
//                        if (n > 0)
//                        {
//                            upic.Print();
//                        }

//                        n = 2;
//                    }
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("吸収   ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print(GeneralLib.RightPaddedString(ref withBlock.strAbsorb, 12));
//                    n = (short)(n + 1);
//                    if (n > 1)
//                    {
//                        upic.Print();
//                        // ADD START 240a
//                        if (GUI.NewGUIMode)
//                        {
//                            upic.CurrentX = 5;
//                        }
//                        // ADD  END  240a
//                        n = 0;
//                    }
//                }

//                // 無効化
//                if (Strings.Len(withBlock.strImmune) > 0 & Strings.InStr(withBlock.strImmune, "非表示") == 0)
//                {
//                    if (Strings.Len(withBlock.strImmune) > 5)
//                    {
//                        if (n > 0)
//                        {
//                            upic.Print();
//                            // ADD START 240a
//                            if (GUI.NewGUIMode)
//                            {
//                                upic.CurrentX = 5;
//                            }
//                            // ADD  END  240a
//                        }

//                        n = 2;
//                    }
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("無効化 ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print(GeneralLib.RightPaddedString(ref withBlock.strImmune, 12));
//                    n = (short)(n + 1);
//                    if (n > 1)
//                    {
//                        upic.Print();
//                        // ADD START 240a
//                        if (GUI.NewGUIMode)
//                        {
//                            upic.CurrentX = 5;
//                        }
//                        // ADD  END  240a
//                        n = 0;
//                    }
//                }

//                // 耐性
//                if (Strings.Len(withBlock.strResist) > 0 & Strings.InStr(withBlock.strResist, "非表示") == 0)
//                {
//                    if (Strings.Len(withBlock.strResist) > 5)
//                    {
//                        if (n > 0)
//                        {
//                            upic.Print();
//                            // ADD START 240a
//                            if (GUI.NewGUIMode)
//                            {
//                                upic.CurrentX = 5;
//                            }
//                            // ADD  END  240a
//                        }

//                        n = 2;
//                    }

//                    if (n == 0 & GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("耐性   ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print(GeneralLib.RightPaddedString(ref withBlock.strResist, 12));
//                    n = (short)(n + 1);
//                    if (n > 1)
//                    {
//                        upic.Print();
//                        // ADD START 240a
//                        if (GUI.NewGUIMode)
//                        {
//                            upic.CurrentX = 5;
//                        }
//                        // ADD  END  240a
//                        n = 0;
//                    }
//                }

//                // 弱点
//                if (Strings.Len(withBlock.strWeakness) > 0 & Strings.InStr(withBlock.strWeakness, "非表示") == 0)
//                {
//                    if (Strings.Len(withBlock.strWeakness) > 5)
//                    {
//                        if (n > 0)
//                        {
//                            upic.Print();
//                            // ADD START 240a
//                            if (GUI.NewGUIMode)
//                            {
//                                upic.CurrentX = 5;
//                            }
//                            // ADD  END  240a
//                        }

//                        n = 2;
//                    }

//                    if (n == 0 & GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("弱点   ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print(GeneralLib.RightPaddedString(ref withBlock.strWeakness, 12));
//                    n = (short)(n + 1);
//                    if (n > 1)
//                    {
//                        upic.Print();
//                        // ADD START 240a
//                        if (GUI.NewGUIMode)
//                        {
//                            upic.CurrentX = 5;
//                        }
//                        // ADD  END  240a
//                        n = 0;
//                    }
//                }

//                // 有効
//                if (Strings.Len(withBlock.strEffective) > 0 & Strings.InStr(withBlock.strEffective, "非表示") == 0)
//                {
//                    if (Strings.Len(withBlock.strEffective) > 5)
//                    {
//                        if (n > 0)
//                        {
//                            upic.Print();
//                            // ADD START 240a
//                            if (GUI.NewGUIMode)
//                            {
//                                upic.CurrentX = 5;
//                            }
//                            // ADD  END  240a
//                        }

//                        n = 2;
//                    }

//                    if (n == 0 & GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("有効   ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print(GeneralLib.RightPaddedString(ref withBlock.strEffective, 12));
//                    n = (short)(n + 1);
//                    if (n > 1)
//                    {
//                        upic.Print();
//                        // ADD START 240a
//                        if (GUI.NewGUIMode)
//                        {
//                            upic.CurrentX = 5;
//                        }
//                        // ADD  END  240a
//                        n = 0;
//                    }
//                }

//                // 特殊効果無効化
//                if (Strings.Len(withBlock.strSpecialEffectImmune) > 0 & Strings.InStr(withBlock.strSpecialEffectImmune, "非表示") == 0)
//                {
//                    if (Strings.Len(withBlock.strSpecialEffectImmune) > 5)
//                    {
//                        if (n > 0)
//                        {
//                            upic.Print();
//                            // ADD START 240a
//                            if (GUI.NewGUIMode)
//                            {
//                                upic.CurrentX = 5;
//                            }
//                            // ADD  END  240a
//                        }

//                        n = 2;
//                    }

//                    if (n == 0 & GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("特無効 ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print(GeneralLib.RightPaddedString(ref withBlock.strSpecialEffectImmune, 12));
//                    n = (short)(n + 1);
//                    if (n > 1)
//                    {
//                        upic.Print();
//                        // ADD START 240a
//                        if (GUI.NewGUIMode)
//                        {
//                            upic.CurrentX = 5;
//                        }
//                        // ADD  END  240a
//                        n = 0;
//                    }
//                }

//                // 必要に応じて改行
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

//                n = 0;

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 武器・防具クラス
//                flist = new string[1];
//                string argoname5 = "アイテム交換";
//                if (Expression.IsOptionDefined(ref argoname5))
//                {
//                    string argfname23 = "武器クラス";
//                    string argfname24 = "防具クラス";
//                    if (withBlock.IsFeatureAvailable(ref argfname23) | withBlock.IsFeatureAvailable(ref argfname24))
//                    {
//                        if (GUI.NewGUIMode)
//                        {
//                            upic.CurrentX = 5;
//                        }
//                        string argbuf17 = "武器・防具クラス";
//                        upic.Print(GeneralLib.RightPaddedString(ref argbuf17, 19));
//                        Array.Resize(ref flist, 2);
//                        flist[1] = "武器・防具クラス";
//                        n = (short)(n + 1);
//                    }
//                }

//                // 特殊能力一覧を表示する前に必要気力判定のためメインパイロットの気力を参照
//                if (withBlock.CountPilot() > 0)
//                {
//                    pmorale = withBlock.MainPilot().Morale;
//                }
//                else
//                {
//                    pmorale = 150;
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 特殊能力一覧
//                var loopTo10 = withBlock.CountAllFeature();
//                for (i = (short)(withBlock.AdditionalFeaturesNum + 1); i <= loopTo10; i++)
//                {
//                    object argIndex64 = i;
//                    fname = withBlock.AllFeatureName(ref argIndex64);

//                    // ユニットステータスコマンド時は通常は非表示のパーツ合体、
//                    // ノーマルモード、換装も表示
//                    if (string.IsNullOrEmpty(fname))
//                    {
//                        if (string.IsNullOrEmpty(Map.MapFileName))
//                        {
//                            object argIndex66 = i;
//                            switch (withBlock.AllFeature(ref argIndex66) ?? "")
//                            {
//                                case "パーツ合体":
//                                case "ノーマルモード":
//                                    {
//                                        string localAllFeature() { object argIndex1 = i; var ret = withBlock.AllFeature(ref argIndex1); return ret; }

//                                        string localRightPaddedString17() { string argbuf = hs5fe6f1588051411f97aaada3678aab3c(); var ret = GeneralLib.RightPaddedString(ref argbuf, 19); return ret; }

//                                        upic.Print(localRightPaddedString17());
//                                        n = (short)(n + 1);
//                                        if (n > 1)
//                                        {
//                                            upic.Print();
//                                            // ADD START 240a
//                                            if (GUI.NewGUIMode)
//                                            {
//                                                upic.CurrentX = 5;
//                                            }
//                                            // ADD  END  240a
//                                            n = 0;
//                                        }

//                                        break;
//                                    }

//                                case "換装":
//                                    {
//                                        fname = "換装";

//                                        // エリアスで換装の名称が変更されている？
//                                        {
//                                            var withBlock3 = SRC.ALDList;
//                                            var loopTo11 = withBlock3.Count();
//                                            for (j = 1; j <= loopTo11; j++)
//                                            {
//                                                object argIndex65 = j;
//                                                {
//                                                    var withBlock4 = withBlock3.Item(ref argIndex65);
//                                                    if (withBlock4.get_AliasType(1) == "換装")
//                                                    {
//                                                        fname = withBlock4.Name;
//                                                        break;
//                                                    }
//                                                }
//                                            }
//                                        }

//                                        upic.Print(GeneralLib.RightPaddedString(ref fname, 19));
//                                        n = (short)(n + 1);
//                                        if (n > 1)
//                                        {
//                                            upic.Print();
//                                            // ADD START 240a
//                                            if (GUI.NewGUIMode)
//                                            {
//                                                upic.CurrentX = 5;
//                                            }
//                                            // ADD  END  240a
//                                            n = 0;
//                                        }

//                                        break;
//                                    }
//                            }
//                        }

//                        goto NextFeature;
//                    }

//                    // 既に表示しているかを判定
//                    var loopTo12 = (short)Information.UBound(flist);
//                    for (j = 1; j <= loopTo12; j++)
//                    {
//                        if ((fname ?? "") == (flist[j] ?? ""))
//                        {
//                            goto NextFeature;
//                        }
//                    }

//                    Array.Resize(ref flist, Information.UBound(flist) + 1 + 1);
//                    flist[Information.UBound(flist)] = fname;

//                    // 使用可否によって表示色を変える
//                    object argIndex67 = i;
//                    fdata = withBlock.AllFeatureData(ref argIndex67);
//                    object argIndex79 = i;
//                    switch (withBlock.AllFeature(ref argIndex79) ?? "")
//                    {
//                        case "合体":
//                            {
//                                bool localIsDefined1() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

//                                if (!localIsDefined1())
//                                {
//                                    goto NextFeature;
//                                }

//                                Unit localItem1() { object argIndex1 = GeneralLib.LIndex(ref fdata, 2); var ret = SRC.UList.Item(ref argIndex1); return ret; }

//                                object argIndex68 = "行動不能";
//                                if (localItem1().IsConditionSatisfied(ref argIndex68))
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "分離":
//                            {
//                                k = 0;
//                                var loopTo13 = GeneralLib.LLength(ref fdata);
//                                for (j = 2; j <= loopTo13; j++)
//                                {
//                                    bool localIsDefined2() { object argIndex1 = GeneralLib.LIndex(ref fdata, j); var ret = SRC.UList.IsDefined(ref argIndex1); return ret; }

//                                    if (!localIsDefined2())
//                                    {
//                                        goto NextFeature;
//                                    }

//                                    Unit localItem2() { object argIndex1 = GeneralLib.LIndex(ref fdata, j); var ret = SRC.UList.Item(ref argIndex1); return ret; }

//                                    {
//                                        var withBlock5 = localItem2().Data;
//                                        string argfname25 = "召喚ユニット";
//                                        if (withBlock5.IsFeatureAvailable(ref argfname25))
//                                        {
//                                            k = (short)(k + Math.Abs(withBlock5.PilotNum));
//                                        }
//                                    }
//                                }

//                                if (withBlock.CountPilot() < k)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "ハイパーモード":
//                            {
//                                double localFeatureLevel() { object argIndex1 = i; var ret = withBlock.FeatureLevel(ref argIndex1); return ret; }

//                                double localFeatureLevel1() { object argIndex1 = i; var ret = withBlock.FeatureLevel(ref argIndex1); return ret; }
//                                // MOD  END  240a
//                                object argIndex69 = "ノーマルモード付加";
//                                if (pmorale < (short)(10d * localFeatureLevel1()) + 100 & withBlock.HP > withBlock.MaxHP / 4)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                }
//                                else if (withBlock.IsConditionSatisfied(ref argIndex69))
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "修理装置":
//                        case "補給装置":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                                {
//                                    if (withBlock.EN < Conversions.ToShort(GeneralLib.LIndex(ref fdata, 2)))
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = rgb(150, 0, 0)
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                        // MOD  END  240a
//                                    }
//                                }

//                                break;
//                            }

//                        case "テレポート":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                                {
//                                    if (withBlock.EN < Conversions.ToShort(GeneralLib.LIndex(ref fdata, 2)))
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = rgb(150, 0, 0)
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                        // MOD  END  240a
//                                    }
//                                }
//                                else if (withBlock.EN < 40)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "分身":
//                            {
//                                if (pmorale < 130)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "超回避":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                                {
//                                    ecost = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 2));
//                                }
//                                else
//                                {
//                                    ecost = 0;
//                                }

//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 3));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                if (withBlock.EN < ecost | pmorale < nmorale)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "緊急テレポート":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                                {
//                                    ecost = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 3));
//                                }
//                                else
//                                {
//                                    ecost = 0;
//                                }

//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 4)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 4));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                if (withBlock.EN < ecost | pmorale < nmorale)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "エネルギーシールド":
//                            {
//                                if (withBlock.EN < 5)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "バリア":
//                        case "バリアシールド":
//                        case "プロテクション":
//                        case "アクティブプロテクション":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                                {
//                                    ecost = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 3));
//                                }
//                                else
//                                {
//                                    ecost = 10;
//                                }

//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 4)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 4));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                object argIndex70 = "バリア無効化";
//                                if (withBlock.EN < ecost | pmorale < nmorale | withBlock.IsConditionSatisfied(ref argIndex70) & Strings.InStr(fdata, "バリア無効化無効") == 0)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                }
//                                // MOD  END  240a
//                                else if (Strings.InStr(fdata, "能力必要") > 0)
//                                {
//                                    var loopTo14 = GeneralLib.LLength(ref fdata);
//                                    for (j = 5; j <= loopTo14; j++)
//                                    {
//                                        opt = GeneralLib.LIndex(ref fdata, j);
//                                        if (Strings.InStr(opt, "*") > 0)
//                                        {
//                                            opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
//                                        }

//                                        switch (opt ?? "")
//                                        {
//                                            case "相殺":
//                                            case "中和":
//                                            case "近接無効":
//                                            case "手動":
//                                            case "能力必要":
//                                                {
//                                                    break;
//                                                }
//                                            // スキップ
//                                            case "同調率":
//                                                {
//                                                    if (withBlock.SyncLevel() == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }

//                                            case "霊力":
//                                                {
//                                                    if (withBlock.PlanaLevel() == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }

//                                            default:
//                                                {
//                                                    if (withBlock.SkillLevel(opt) == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }
//                                        }
//                                    }
//                                }

//                                break;
//                            }

//                        case "フィールド":
//                        case "アクティブフィールド":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                                {
//                                    ecost = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 3));
//                                }
//                                else
//                                {
//                                    ecost = 0;
//                                }

//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 4)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 4));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                object argIndex71 = "バリア無効化";
//                                if (withBlock.EN < ecost | pmorale < nmorale | withBlock.IsConditionSatisfied(ref argIndex71) & Strings.InStr(fdata, "バリア無効化無効") == 0)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                }
//                                // MOD  END  240a
//                                else if (Strings.InStr(fdata, "能力必要") > 0)
//                                {
//                                    var loopTo15 = GeneralLib.LLength(ref fdata);
//                                    for (j = 5; j <= loopTo15; j++)
//                                    {
//                                        opt = GeneralLib.LIndex(ref fdata, j);
//                                        if (Strings.InStr(opt, "*") > 0)
//                                        {
//                                            opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
//                                        }

//                                        switch (opt ?? "")
//                                        {
//                                            case "相殺":
//                                            case "中和":
//                                            case "近接無効":
//                                            case "手動":
//                                            case "能力必要":
//                                                {
//                                                    break;
//                                                }
//                                            // スキップ
//                                            case "同調率":
//                                                {
//                                                    if (withBlock.SyncLevel() == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }

//                                            case "霊力":
//                                                {
//                                                    if (withBlock.PlanaLevel() == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }

//                                            default:
//                                                {
//                                                    if (withBlock.SkillLevel(opt) == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }
//                                        }
//                                    }
//                                }

//                                break;
//                            }

//                        case "広域バリア":
//                        case "広域フィールド":
//                        case "広域プロテクション":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 4)))
//                                {
//                                    ecost = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 4));
//                                }
//                                else if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                                {
//                                    ecost = (short)(20 * Conversions.ToShort(GeneralLib.LIndex(ref fdata, 2)));
//                                }
//                                else
//                                {
//                                    ecost = 0;
//                                }

//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 5)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 5));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                object argIndex72 = "バリア無効化";
//                                if (withBlock.EN < ecost | pmorale < nmorale | withBlock.IsConditionSatisfied(ref argIndex72) & Strings.InStr(fdata, "バリア無効化無効") == 0)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                fname = fname + "(範囲" + GeneralLib.LIndex(ref fdata, 2) + "マス)";
//                                break;
//                            }

//                        case "アーマー":
//                        case "レジスト":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 3));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                if (pmorale < nmorale)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                }
//                                // MOD  END  240a
//                                else if (Strings.InStr(fdata, "能力必要") > 0)
//                                {
//                                    var loopTo16 = GeneralLib.LLength(ref fdata);
//                                    for (j = 4; j <= loopTo16; j++)
//                                    {
//                                        opt = GeneralLib.LIndex(ref fdata, j);
//                                        if (Strings.InStr(opt, "*") > 0)
//                                        {
//                                            opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
//                                        }

//                                        switch (opt ?? "")
//                                        {
//                                            case "同調率":
//                                                {
//                                                    if (withBlock.SyncLevel() == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }

//                                            case "霊力":
//                                                {
//                                                    if (withBlock.PlanaLevel() == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }

//                                            default:
//                                                {
//                                                    if (withBlock.SkillLevel(opt) == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }
//                                        }
//                                    }
//                                }

//                                break;
//                            }

//                        case "攻撃回避":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 3)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 3));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                if (pmorale < nmorale)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "反射":
//                        case "阻止":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 4)))
//                                {
//                                    ecost = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 4));
//                                }
//                                else
//                                {
//                                    ecost = 0;
//                                }

//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 5)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 5));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                if (withBlock.EN < ecost | pmorale < nmorale)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                }
//                                // MOD  END  240a
//                                else if (Strings.InStr(fdata, "能力必要") > 0)
//                                {
//                                    var loopTo17 = GeneralLib.LLength(ref fdata);
//                                    for (j = 6; j <= loopTo17; j++)
//                                    {
//                                        opt = GeneralLib.LIndex(ref fdata, j);
//                                        if (Strings.InStr(opt, "*") > 0)
//                                        {
//                                            opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
//                                        }

//                                        switch (opt ?? "")
//                                        {
//                                            case "相殺":
//                                            case "中和":
//                                            case "近接無効":
//                                            case "手動":
//                                            case "能力必要":
//                                                {
//                                                    break;
//                                                }
//                                            // スキップ
//                                            case "同調率":
//                                                {
//                                                    if (withBlock.SyncLevel() == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }

//                                            case "霊力":
//                                                {
//                                                    if (withBlock.PlanaLevel() == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }

//                                            default:
//                                                {
//                                                    if (withBlock.SkillLevel(opt) == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }
//                                        }
//                                    }
//                                }

//                                break;
//                            }

//                        case "広域阻止":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 5)))
//                                {
//                                    ecost = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 5));
//                                }
//                                else
//                                {
//                                    ecost = 0;
//                                }

//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 6)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 6));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                if (withBlock.EN < ecost | pmorale < nmorale)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                fname = fname + "(範囲" + GeneralLib.LIndex(ref fdata, 2) + "マス)";
//                                break;
//                            }

//                        case "当て身技":
//                        case "自動反撃":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 5)))
//                                {
//                                    ecost = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 5));
//                                }
//                                else
//                                {
//                                    ecost = 0;
//                                }

//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 6)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 6));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                if (withBlock.EN < ecost | pmorale < nmorale)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                }
//                                // MOD  END  240a
//                                else if (Strings.InStr(fdata, "能力必要") > 0)
//                                {
//                                    var loopTo18 = GeneralLib.LLength(ref fdata);
//                                    for (j = 7; j <= loopTo18; j++)
//                                    {
//                                        opt = GeneralLib.LIndex(ref fdata, j);
//                                        if (Strings.InStr(opt, "*") > 0)
//                                        {
//                                            opt = Strings.Left(opt, Strings.InStr(opt, "*") - 1);
//                                        }

//                                        switch (opt ?? "")
//                                        {
//                                            case "相殺":
//                                            case "中和":
//                                            case "近接無効":
//                                            case "手動":
//                                            case "能力必要":
//                                                {
//                                                    break;
//                                                }
//                                            // スキップ
//                                            case "同調率":
//                                                {
//                                                    if (withBlock.SyncLevel() == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }

//                                            case "霊力":
//                                                {
//                                                    if (withBlock.PlanaLevel() == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }

//                                            default:
//                                                {
//                                                    if (withBlock.SkillLevel(opt) == 0d)
//                                                    {
//                                                        goto NextFeature;
//                                                    }

//                                                    break;
//                                                }
//                                        }
//                                    }
//                                }

//                                break;
//                            }

//                        case "ブースト":
//                            {
//                                if (pmorale >= 130)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = vbBlue
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "盾":
//                            {
//                                object argIndex73 = "盾ダメージ";
//                                object argIndex74 = "盾";
//                                if (withBlock.ConditionLevel(ref argIndex73) >= withBlock.AllFeatureLevel(ref argIndex74))
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                object argIndex75 = "盾";
//                                object argIndex76 = "盾ダメージ";
//                                object argIndex77 = "盾";
//                                fname = fname + "(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GeneralLib.MaxLng((int)(withBlock.AllFeatureLevel(ref argIndex75) - withBlock.ConditionLevel(ref argIndex76)), 0)) + "/" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.AllFeatureLevel(ref argIndex77)) + ")";
//                                break;
//                            }

//                        case "ＨＰ回復":
//                        case "ＥＮ回復":
//                            {
//                                // MOD START MARGE
//                                // If .IsConditionSatisfied("回復不能") Then
//                                object argIndex78 = "回復不能";
//                                string argsname3 = "回復不能";
//                                if (withBlock.IsConditionSatisfied(ref argIndex78) | withBlock.IsSpecialPowerInEffect(ref argsname3))
//                                {
//                                    // MOD END MARGE
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }

//                        case "格闘強化":
//                        case "射撃強化":
//                        case "命中強化":
//                        case "回避強化":
//                        case "技量強化":
//                        case "反応強化":
//                        case "ＨＰ強化":
//                        case "ＥＮ強化":
//                        case "装甲強化":
//                        case "運動性強化":
//                        case "移動力強化":
//                        case "ＨＰ割合強化":
//                        case "ＥＮ割合強化":
//                        case "装甲割合強化":
//                        case "運動性割合強化":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 2)))
//                                {
//                                    int localStrToLng2() { string argexpr = GeneralLib.LIndex(ref fdata, 2); var ret = GeneralLib.StrToLng(ref argexpr); return ret; }

//                                    if (pmorale >= localStrToLng2())
//                                    {
//                                        // MOD START 240a
//                                        // upic.ForeColor = vbBlue
//                                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityEnable, ColorTranslator.ToOle(Color.Blue))));
//                                        // MOD  END  240a
//                                    }
//                                }

//                                break;
//                            }

//                        case "ＺＯＣ":
//                            {
//                                if (GeneralLib.LLength(ref fdata) < 2)
//                                {
//                                    j = 1;
//                                }
//                                else
//                                {
//                                    j = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 2));
//                                }

//                                if (j >= 1)
//                                {
//                                    string args3 = " ";
//                                    GeneralLib.ReplaceString(ref fdata, ref Constants.vbTab, ref args3);
//                                    if (Strings.InStr(fdata, " 直線") > 0 | Strings.InStr(fdata, " 垂直") > 0 & Strings.InStr(fdata, " 水平") > 0)
//                                    {
//                                        buf = "直線";
//                                    }
//                                    else if (Strings.InStr(fdata, " 垂直") > 0)
//                                    {
//                                        buf = "上下";
//                                    }
//                                    else if (Strings.InStr(fdata, " 水平") > 0)
//                                    {
//                                        buf = "左右";
//                                    }
//                                    else
//                                    {
//                                        buf = "範囲";
//                                    }

//                                    fname = fname + "(" + buf + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(j) + "マス)";
//                                }

//                                break;
//                            }

//                        case "広域ＺＯＣ無効化":
//                            {
//                                fname = fname + "(範囲" + GeneralLib.LIndex(ref fdata, 2) + "マス)";
//                                break;
//                            }

//                        case "追加攻撃":
//                            {
//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 5)))
//                                {
//                                    ecost = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 5));
//                                }
//                                else
//                                {
//                                    ecost = 0;
//                                }

//                                if (Information.IsNumeric(GeneralLib.LIndex(ref fdata, 6)))
//                                {
//                                    nmorale = Conversions.ToShort(GeneralLib.LIndex(ref fdata, 6));
//                                }
//                                else
//                                {
//                                    nmorale = 0;
//                                }

//                                if (withBlock.EN < ecost | pmorale < nmorale)
//                                {
//                                    // MOD START 240a
//                                    // upic.ForeColor = rgb(150, 0, 0)
//                                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                                    // MOD  END  240a
//                                }

//                                break;
//                            }
//                    }

//                    // 必要条件を満たさない特殊能力は赤色で表示
//                    bool localIsFeatureActivated() { object argIndex1 = i; var ret = withBlock.IsFeatureActivated(ref argIndex1); return ret; }

//                    if (!localIsFeatureActivated())
//                    {
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(150, 0, 0)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                        // MOD  END  240a
//                    }

//                    // 特殊能力名を表示
//                    if (LenB(Strings.StrConv(fname, vbFromUnicode)) > 19)
//                    {
//                        if (n > 0)
//                        {
//                            upic.Print();
//                            // ADD START 240a
//                            if (GUI.NewGUIMode)
//                            {
//                                upic.CurrentX = 5;
//                            }
//                            // ADD  END  240a
//                        }
//                        upic.Print(fname);
//                        n = 2;
//                    }
//                    else
//                    {
//                        upic.Print(GeneralLib.RightPaddedString(ref fname, 19));
//                        n = (short)(n + 1);
//                    }

//                    // 必要に応じて改行
//                    if (n > 1)
//                    {
//                        upic.Print();
//                        // ADD START 240a
//                        if (GUI.NewGUIMode)
//                        {
//                            upic.CurrentX = 5;
//                        }
//                        // ADD  END  240a
//                        n = 0;
//                    }

//                    // 表示色を戻しておく
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    NextFeature:
//                    ;
//                }

//                if (n > 0)
//                {
//                    upic.Print();
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // アイテム一覧
//                if (withBlock.CountItem() > 0)
//                {
//                    j = 0;
//                    var loopTo19 = withBlock.CountItem();
//                    for (i = 1; i <= loopTo19; i++)
//                    {
//                        object argIndex80 = i;
//                        {
//                            var withBlock6 = withBlock.Item(ref argIndex80);
//                            // 表示指定を持つアイテムのみ表示する
//                            string argfname26 = "表示";
//                            if (!withBlock6.IsFeatureAvailable(ref argfname26))
//                            {
//                                goto NextItem;
//                            }

//                            // アイテム名を表示
//                            if (Strings.Len(withBlock6.Nickname()) > 9)
//                            {
//                                if (j == 1)
//                                {
//                                    upic.Print();
//                                    // ADD START 240a
//                                    if (GUI.NewGUIMode)
//                                    {
//                                        upic.CurrentX = 5;
//                                    }
//                                    // ADD  END  240a
//                                }
//                                upic.Print(withBlock6.Nickname());
//                                j = 2;
//                            }
//                            else
//                            {
//                                upic.Print(GeneralLib.RightPaddedString(ref withBlock6.Nickname(), 19));
//                                j = (short)(j + 1);
//                            }

//                            if (j == 2)
//                            {
//                                upic.Print();
//                                // ADD START 240a
//                                if (GUI.NewGUIMode)
//                                {
//                                    upic.CurrentX = 5;
//                                }
//                                // ADD  END  240a
//                                j = 0;
//                            }
//                        }

//                        NextItem:
//                        ;
//                    }

//                    if (j > 0)
//                    {
//                        upic.Print();
//                        // ADD START 240a
//                        if (GUI.NewGUIMode)
//                        {
//                            upic.CurrentX = 5;
//                        }
//                        // ADD  END  240a
//                    }
//                }

//                // ターゲット選択時の攻撃結果予想表示

//                // 攻撃時にのみ表示
//                if ((Commands.CommandState == "ターゲット選択" | Commands.CommandState == "移動後ターゲット選択") & (Commands.SelectedCommand == "攻撃" | Commands.SelectedCommand == "マップ攻撃") & Commands.SelectedUnit is object & Commands.SelectedWeapon > 0 & SRC.Stage != "プロローグ" & SRC.Stage != "エピローグ")
//                {
//                }
//                // 攻撃時と判定
//                else
//                {
//                    goto SkipAttackExpResult;
//                }

//                // 相手が敵の場合にのみ表示
//                object argIndex81 = "暴走";
//                object argIndex82 = "魅了";
//                object argIndex83 = "憑依";
//                object argIndex84 = "混乱";
//                object argIndex85 = "睡眠";
//                if (withBlock.Party != "敵" & withBlock.Party != "中立" & !withBlock.IsConditionSatisfied(ref argIndex81) & !withBlock.IsConditionSatisfied(ref argIndex82) & !withBlock.IsConditionSatisfied(ref argIndex83) & !withBlock.IsConditionSatisfied(ref argIndex84) & !withBlock.IsConditionSatisfied(ref argIndex85))
//                {
//                    goto SkipAttackExpResult;
//                }

//                upic.Print();

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 攻撃手段
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                upic.Print("攻撃     ");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a
//                upic.Print(Commands.SelectedUnit.WeaponNickname(Commands.SelectedWeapon));
//                // サポートアタックを得られる？
//                string argattr5 = "合";
//                string argattr6 = "Ｍ";
//                if (!Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, ref argattr5) & !Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, ref argattr6) & Commands.UseSupportAttack)
//                {
//                    if (Commands.SelectedUnit.LookForSupportAttack(ref u) is object)
//                    {
//                        upic.Print(" [援]");
//                    }
//                    else
//                    {
//                        upic.Print();
//                    }
//                }
//                else
//                {
//                    upic.Print();
//                }

//                // 反撃を受ける？
//                string argattr7 = "Ｍ";
//                string argattr8 = "間";
//                if (withBlock.MaxAction() == 0 | Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, ref argattr7) | Commands.SelectedUnit.IsWeaponClassifiedAs(Commands.SelectedWeapon, ref argattr8))
//                {
//                    w = 0;
//                }
//                else
//                {
//                    string argamode = "反撃";
//                    int argmax_prob = 0;
//                    int argmax_dmg = 0;
//                    w = COM.SelectWeapon(ref u, ref Commands.SelectedUnit, ref argamode, max_prob: ref argmax_prob, max_dmg: ref argmax_dmg);
//                }

//                // 敵の防御行動を設定
//                // UPGRADE_WARNING: オブジェクト SelectDefense() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
//                def_mode = Conversions.ToString(COM.SelectDefense(ref Commands.SelectedUnit, ref Commands.SelectedWeapon, ref u, ref w));
//                if (!string.IsNullOrEmpty(def_mode))
//                {
//                    w = 0;
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 予測ダメージ
//                string argoname6 = "予測ダメージ非表示";
//                if (!Expression.IsOptionDefined(ref argoname6))
//                {
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("ダメージ ");
//                    dmg = Commands.SelectedUnit.Damage(Commands.SelectedWeapon, ref u, true);
//                    if (def_mode == "防御")
//                    {
//                        dmg = dmg / 2;
//                    }

//                    object argIndex86 = "データ不明";
//                    if (dmg >= withBlock.HP & !withBlock.IsConditionSatisfied(ref argIndex86))
//                    {
//                        upic.ForeColor = ColorTranslator.FromOle(Information.RGB(190, 0, 0));
//                    }
//                    else
//                    {
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(0, 0, 0)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                        // MOD  END  240a
//                    }
//                    upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(dmg));
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 予測命中率
//                string argoname7 = "予測命中率非表示";
//                if (!Expression.IsOptionDefined(ref argoname7))
//                {
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("命中率   ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    prob = Commands.SelectedUnit.HitProbability(Commands.SelectedWeapon, ref u, true);
//                    if (def_mode == "回避")
//                    {
//                        prob = (short)(prob / 2);
//                    }

//                    cprob = Commands.SelectedUnit.CriticalProbability(Commands.SelectedWeapon, ref u, def_mode);
//                    upic.Print(GeneralLib.MinLng(prob, 100) + "％（" + cprob + "％）");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                }

//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                if (w > 0)
//                {
//                    // 反撃手段
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    upic.Print("反撃     ");
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    upic.Print(withBlock.WeaponNickname(w));
//                    // サポートガードを受けられる？
//                    if (u.LookForSupportGuard(ref Commands.SelectedUnit, Commands.SelectedWeapon) is object)
//                    {
//                        upic.Print(" [援]");
//                    }
//                    else
//                    {
//                        upic.Print();
//                    }

//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    // 予測ダメージ
//                    string argoname8 = "予測ダメージ非表示";
//                    if (!Expression.IsOptionDefined(ref argoname8))
//                    {
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(0, 0, 150)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                        // MOD  END  240a
//                        upic.Print("ダメージ ");
//                        dmg = withBlock.Damage(w, ref Commands.SelectedUnit, true);
//                        if (dmg >= Commands.SelectedUnit.HP)
//                        {
//                            upic.ForeColor = ColorTranslator.FromOle(Information.RGB(190, 0, 0));
//                        }
//                        else
//                        {
//                            // MOD START 240a
//                            // upic.ForeColor = rgb(0, 0, 0)
//                            upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                            // MOD  END  240a
//                        }
//                        upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(dmg));
//                    }

//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    // 予測命中率
//                    string argoname9 = "予測命中率非表示";
//                    if (!Expression.IsOptionDefined(ref argoname9))
//                    {
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(0, 0, 150)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                        // MOD  END  240a
//                        upic.Print("命中率   ");
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(0, 0, 0)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                        // MOD  END  240a
//                        prob = withBlock.HitProbability(w, ref Commands.SelectedUnit, true);
//                        cprob = withBlock.CriticalProbability(w, ref Commands.SelectedUnit);
//                        upic.Print(Microsoft.VisualBasic.Compatibility.VB6.Support.Format(GeneralLib.MinLng(prob, 100)) + "％（" + cprob + "％）");
//                    }
//                }
//                else
//                {
//                    // 相手は反撃できない
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 150)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                    // MOD  END  240a
//                    if (!string.IsNullOrEmpty(def_mode))
//                    {
//                        upic.Print(def_mode);
//                    }
//                    else
//                    {
//                        upic.Print("反撃不能");
//                    }
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    // サポートガードを受けられる？
//                    if (u.LookForSupportGuard(ref Commands.SelectedUnit, Commands.SelectedWeapon) is object)
//                    {
//                        upic.Print(" [援]");
//                    }
//                    else
//                    {
//                        upic.Print();
//                    }
//                }

//                SkipAttackExpResult:
//                ;


//                // ADD START 240a
//                if (GUI.NewGUIMode)
//                {
//                    upic.CurrentX = 5;
//                }
//                // ADD  END  240a
//                // 武器一覧
//                upic.CurrentY = upic.CurrentY + 8;
//                upic.Print(Strings.Space(25));
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 150)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityName, Information.RGB(0, 0, 150))));
//                // MOD  END  240a
//                upic.Print("攻撃 射程");
//                // MOD START 240a
//                // upic.ForeColor = rgb(0, 0, 0)
//                upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                // MOD  END  240a

//                warray = new short[(withBlock.CountWeapon() + 1)];
//                wpower = new int[(withBlock.CountWeapon() + 1)];
//                var loopTo20 = withBlock.CountWeapon();
//                for (i = 1; i <= loopTo20; i++)
//                {
//                    string argtarea = "";
//                    wpower[i] = withBlock.WeaponPower(i, ref argtarea);
//                }

//                // 攻撃力でソート
//                var loopTo21 = withBlock.CountWeapon();
//                for (i = 1; i <= loopTo21; i++)
//                {
//                    var loopTo22 = (short)(i - 1);
//                    for (j = 1; j <= loopTo22; j++)
//                    {
//                        if (wpower[i] > wpower[warray[i - j]])
//                        {
//                            break;
//                        }
//                        else if (wpower[i] == wpower[warray[i - j]])
//                        {
//                            if (withBlock.Weapon(i).ENConsumption > 0)
//                            {
//                                if (withBlock.Weapon(i).ENConsumption >= withBlock.Weapon(warray[i - j]).ENConsumption)
//                                {
//                                    break;
//                                }
//                            }
//                            else if (withBlock.Weapon(i).Bullet > 0)
//                            {
//                                if (withBlock.Weapon(i).Bullet <= withBlock.Weapon(warray[i - j]).Bullet)
//                                {
//                                    break;
//                                }
//                            }
//                            else if (withBlock.Weapon((short)(i - j)).ENConsumption == 0 & withBlock.Weapon(warray[i - j]).Bullet == 0)
//                            {
//                                break;
//                            }
//                        }
//                    }

//                    var loopTo23 = (short)(j - 1);
//                    for (k = 1; k <= loopTo23; k++)
//                        warray[i - k + 1] = warray[i - k];
//                    warray[i - j + 1] = i;
//                }

//                // 個々の武器を表示
//                var loopTo24 = withBlock.CountWeapon();
//                for (i = 1; i <= loopTo24; i++)
//                {
//                    if (upic.CurrentY > 420)
//                    {
//                        break;
//                    }

//                    w = warray[i];
//                    string argref_mode1 = "ステータス";
//                    if (!withBlock.IsWeaponAvailable(w, ref argref_mode1))
//                    {
//                        // 習得していない技は表示しない
//                        if (!withBlock.IsWeaponMastered(w))
//                        {
//                            goto NextWeapon;
//                        }
//                        // Disableコマンドで使用不可になった武器も同様
//                        if (withBlock.IsDisabled(ref withBlock.Weapon(w).Name))
//                        {
//                            goto NextWeapon;
//                        }
//                        // フォーメーションを満たしていない合体技も
//                        string argattr9 = "合";
//                        if (withBlock.IsWeaponClassifiedAs(w, ref argattr9))
//                        {
//                            if (!withBlock.IsCombinationAttackAvailable(w, true))
//                            {
//                                goto NextWeapon;
//                            }
//                        }
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(150, 0, 0)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                        // MOD  END  240a
//                    }

//                    // 武器の表示
//                    string argtarea1 = "";
//                    if (withBlock.WeaponPower(w, ref argtarea1) < 10000)
//                    {
//                        string argbuf18 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.WeaponNickname(w));
//                        buf = GeneralLib.RightPaddedString(ref argbuf18, 25);
//                        string localLeftPaddedString19() { string argtarea = ""; string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.WeaponPower(w, ref argtarea)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 4); return ret; }

//                        buf = buf + localLeftPaddedString19();
//                    }
//                    else
//                    {
//                        string argbuf19 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.WeaponNickname(w));
//                        buf = GeneralLib.RightPaddedString(ref argbuf19, 24);
//                        string localLeftPaddedString20() { string argtarea = ""; string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.WeaponPower(w, ref argtarea)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                        buf = buf + localLeftPaddedString20();
//                    }

//                    // 武器が特殊効果を持つ場合は略称で表記
//                    if (withBlock.WeaponMaxRange(w) > 1)
//                    {
//                        string localLeftPaddedString21() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.Weapon(w).MinRange) + "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.WeaponMaxRange(w)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 34 - LenB(Strings.StrConv(buf, vbFromUnicode))); return ret; }

//                        buf = buf + localLeftPaddedString21();
//                        // 移動後攻撃可能
//                        string argattr10 = "Ｐ";
//                        if (withBlock.IsWeaponClassifiedAs(w, ref argattr10))
//                        {
//                            buf = buf + "P";
//                        }
//                    }
//                    else
//                    {
//                        string argbuf20 = "1";
//                        buf = buf + GeneralLib.LeftPaddedString(ref argbuf20, 34 - LenB(Strings.StrConv(buf, vbFromUnicode)));
//                        // ADD START MARGE
//                        // 移動後攻撃不可
//                        string argattr11 = "Ｑ";
//                        if (withBlock.IsWeaponClassifiedAs(w, ref argattr11))
//                        {
//                            buf = buf + "Q";
//                        }
//                        // ADD END MARGE
//                    }
//                    // マップ攻撃
//                    string argattr12 = "Ｍ";
//                    if (withBlock.IsWeaponClassifiedAs(w, ref argattr12))
//                    {
//                        buf = buf + "M";
//                    }
//                    // 特殊効果
//                    wclass = withBlock.Weapon(w).Class_Renamed;
//                    var loopTo25 = withBlock.CountWeaponEffect(w);
//                    for (j = 1; j <= loopTo25; j++)
//                        buf = buf + "+";
//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    upic.Print(buf);
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    NextWeapon:
//                    ;
//                }

//                // アビリティ一覧
//                var loopTo26 = withBlock.CountAbility();
//                for (i = 1; i <= loopTo26; i++)
//                {
//                    if (upic.CurrentY > 420)
//                    {
//                        break;
//                    }

//                    string argref_mode2 = "ステータス";
//                    if (!withBlock.IsAbilityAvailable(i, ref argref_mode2))
//                    {
//                        // 習得していない技は表示しない
//                        if (!withBlock.IsAbilityMastered(i))
//                        {
//                            goto NextAbility;
//                        }
//                        // Disableコマンドで使用不可になった武器も同様
//                        if (withBlock.IsDisabled(ref withBlock.Ability(i).Name))
//                        {
//                            goto NextAbility;
//                        }
//                        // フォーメーションを満たしていない合体技も
//                        string argattr13 = "合";
//                        if (withBlock.IsAbilityClassifiedAs(i, ref argattr13))
//                        {
//                            if (!withBlock.IsCombinationAbilityAvailable(i, true))
//                            {
//                                goto NextAbility;
//                            }
//                        }
//                        // MOD START 240a
//                        // upic.ForeColor = rgb(150, 0, 0)
//                        upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorAbilityDisable, Information.RGB(150, 0, 0))));
//                        // MOD  END  240a
//                    }

//                    // ADD START 240a
//                    if (GUI.NewGUIMode)
//                    {
//                        upic.CurrentX = 5;
//                    }
//                    // ADD  END  240a
//                    // アビリティの表示
//                    string localRightPaddedString18() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.AbilityNickname(i)); var ret = GeneralLib.RightPaddedString(ref argbuf, 29); return ret; }

//                    upic.Print(localRightPaddedString18());
//                    if (withBlock.AbilityMaxRange(i) > 1)
//                    {
//                        string localLeftPaddedString22() { string argbuf = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.AbilityMinRange(i)) + "-" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock.AbilityMaxRange(i)); var ret = GeneralLib.LeftPaddedString(ref argbuf, 5); return ret; }

//                        upic.Print(localLeftPaddedString22());
//                        string argattr14 = "Ｐ";
//                        if (withBlock.IsAbilityClassifiedAs(i, ref argattr14))
//                        {
//                            upic.Print("P");
//                        }

//                        string argattr15 = "Ｍ";
//                        if (withBlock.IsAbilityClassifiedAs(i, ref argattr15))
//                        {
//                            upic.Print("M");
//                        }
//                        upic.Print();
//                    }
//                    else if (withBlock.AbilityMaxRange(i) == 1)
//                    {
//                        upic.Print("    1");
//                        // ADD START MARGE
//                        string argattr16 = "Ｑ";
//                        if (withBlock.IsAbilityClassifiedAs(i, ref argattr16))
//                        {
//                            upic.Print("Q");
//                        }
//                        // ADD END MARGE
//                        string argattr17 = "Ｍ";
//                        if (withBlock.IsAbilityClassifiedAs(i, ref argattr17))
//                        {
//                            upic.Print("M");
//                        }
//                        upic.Print();
//                    }
//                    else
//                    {
//                        upic.Print("    -");
//                    }
//                    // MOD START 240a
//                    // upic.ForeColor = rgb(0, 0, 0)
//                    upic.ForeColor = ColorTranslator.FromOle(Conversions.ToInteger(Interaction.IIf(GUI.NewGUIMode, StatusFontColorNormalString, Information.RGB(0, 0, 0))));
//                    // MOD  END  240a
//                    NextAbility:
//                    ;
//                }
//            }

//            UpdateStatusWindow:
//            ;


//            // MOD START 240a
//            // If MainWidth = 15 Then
//            if (!GUI.NewGUIMode)
//            {
//                // MOD  END
//                // ステータスウィンドウをリフレッシュ
//                GUI.MainForm.picFace.Refresh();
//                ppic.Refresh();
//                upic.Refresh();
//            }
//            else
//            {
//                if (GUI.MouseX < GUI.MainPWidth / 2)
//                {
//                    // MOD START 240a
//                    // upic.Move MainPWidth - 230 - 5, 10
//                    // 画面左側にカーソルがある場合
//                    upic.SetBounds((int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(GUI.MainPWidth - 240), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(10d), 0, 0, BoundsSpecified.X | BoundsSpecified.Y);
//                }
//                // MOD  END
//                else
//                {
//                    upic.SetBounds((int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsX(5d), (int)Microsoft.VisualBasic.Compatibility.VB6.Support.TwipsToPixelsY(10d), 0, 0, BoundsSpecified.X | BoundsSpecified.Y);
//                }

//                if (upic.Visible)
//                {
//                    upic.Refresh();
//                }
//                else
//                {
//                    upic.Visible = true;
//                }
//            }

//            return;
//            ErrorHandler:
//            ;
//            string argmsg = "パイロット用画像ファイル" + Constants.vbCr + Constants.vbLf + fname + Constants.vbCr + Constants.vbLf + "の読み込み中にエラーが発生しました。" + Constants.vbCr + Constants.vbLf + "画像ファイルが壊れていないか確認して下さい。";
//            GUI.ErrorMessage(ref argmsg);
//        }

//        // 指定されたパイロットのステータスをステータスウィンドウに表示
//        public static void DisplayPilotStatus(Pilot p)
//        {
//            short i;
//            DisplayedUnit = p.Unit_Renamed;
//            {
//                var withBlock = DisplayedUnit;
//                if (ReferenceEquals(p, withBlock.MainPilot()))
//                {
//                    // メインパイロット
//                    DisplayUnitStatus(ref DisplayedUnit, 0);
//                }
//                else
//                {
//                    // サブパイロット
//                    var loopTo = withBlock.CountPilot();
//                    for (i = 1; i <= loopTo; i++)
//                    {
//                        Pilot localPilot() { object argIndex1 = i; var ret = withBlock.Pilot(ref argIndex1); return ret; }

//                        if (ReferenceEquals(p, localPilot()))
//                        {
//                            DisplayUnitStatus(ref DisplayedUnit, i);
//                            return;
//                        }
//                    }

//                    // サポートパイロット
//                    var loopTo1 = withBlock.CountSupport();
//                    for (i = 1; i <= loopTo1; i++)
//                    {
//                        Pilot localSupport() { object argIndex1 = i; var ret = withBlock.Support(ref argIndex1); return ret; }

//                        if (ReferenceEquals(p, localSupport()))
//                        {
//                            DisplayUnitStatus(ref DisplayedUnit, (short)(i + withBlock.CountPilot()));
//                            return;
//                        }
//                    }

//                    // 追加サポート
//                    string argfname = "追加サポート";
//                    if (withBlock.IsFeatureAvailable(ref argfname))
//                    {
//                        DisplayUnitStatus(ref DisplayedUnit, (short)(withBlock.CountPilot() + withBlock.CountSupport() + 1));
//                    }
//                }
//            }
//        }

//        // 指定したマップ座標にいるユニットのステータスをステータスウィンドウに表示
//        public static void InstantUnitStatusDisplay(short X, short Y)
//        {
//            Unit u;

//            // 指定された座標にいるユニットを収得
//            u = Map.MapDataForUnit[X, Y];

//            // 発進コマンドの場合は母艦ではなく発進するユニットを使う
//            if (Commands.CommandState == "ターゲット選択" & Commands.SelectedCommand == "発進")
//            {
//                if (ReferenceEquals(u, Commands.SelectedUnit))
//                {
//                    u = Commands.SelectedTarget;
//                    if (u is null)
//                    {
//                        return;
//                    }
//                }
//            }

//            if (DisplayedUnit is null)
//            {
//            }
//            // ステータスウィンドウに何も表示されていなければ無条件で表示
//            // 同じユニットが表示されていればスキップ
//            else if (ReferenceEquals(u, DisplayedUnit))
//            {
//                return;
//            }

//            DisplayUnitStatus(ref u);
//        }

//        // ステータスウィンドウをクリア
//        public static void ClearUnitStatus()
//        {
//            if (GUI.MainWidth == 15)
//            {
//                GUI.MainForm.picFace = Image.FromFile("");
//                GUI.MainForm.picPilotStatus.Cls();
//                GUI.MainForm.picUnitStatus.Cls();
//                // UPGRADE_NOTE: オブジェクト DisplayedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                DisplayedUnit = null;
//            }
//            else
//            {
//                GUI.MainForm.picUnitStatus.Visible = false;
//                GUI.MainForm.picUnitStatus.Cls();
//                IsStatusWindowDisabled = true;
//                Application.DoEvents();
//                IsStatusWindowDisabled = false;
//                // ADD
//                // UPGRADE_NOTE: オブジェクト DisplayedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
//                DisplayedUnit = null;
//            }
//        }

//        // ADD START 240a
//        // 新ＧＵＩ時のグローバルステータスウインドウのサイズを取得する
//        private static int GetGlobalStatusSize(ref short X, ref short Y)
//        {
//            int GetGlobalStatusSizeRet = default;
//            int ret;
//            ret = 42;
//            if (!(X < 1 | Map.MapWidth < X | Y < 1 | Map.MapHeight < Y))
//            {
//                // 地形情報の表示が確定
//                ret = 106;
//                // ＨＰ・ＥＮ回復が記述される場合
//                if (Map.TerrainEffectForHPRecover(X, Y) > 0 | Map.TerrainEffectForENRecover(X, Y) > 0)
//                {
//                    ret = ret + 16;
//                }
//                // ＨＰ・ＥＮ減少が記述される場合
//                string argFeature = "ＨＰ減少";
//                string argFeature1 = "ＥＮ減少";
//                if (Map.TerrainHasFeature(X, Y, ref argFeature) | Map.TerrainHasFeature(X, Y, ref argFeature1))
//                {
//                    ret = ret + 16;
//                }
//                // ＨＰ・ＥＮ増加が記述される場合
//                string argFeature2 = "ＨＰ増加";
//                string argFeature3 = "ＥＮ増加";
//                if (Map.TerrainHasFeature(X, Y, ref argFeature2) | Map.TerrainHasFeature(X, Y, ref argFeature3))
//                {
//                    ret = ret + 16;
//                }
//                // ＨＰ・ＥＮ低下が記述される場合
//                string argFeature4 = "ＨＰ低下";
//                string argFeature5 = "ＥＮ低下";
//                if (Map.TerrainHasFeature(X, Y, ref argFeature4) | Map.TerrainHasFeature(X, Y, ref argFeature5))
//                {
//                    ret = ret + 16;
//                }
//                // 摩擦・状態付加が記述される場合
//                string argFeature6 = "摩擦";
//                string argFeature7 = "状態付加";
//                if (Map.TerrainHasFeature(X, Y, ref argFeature6) | Map.TerrainHasFeature(X, Y, ref argFeature7))
//                {
//                    ret = ret + 16;
//                }
//            }

//            GetGlobalStatusSizeRet = ret;
//            return GetGlobalStatusSizeRet;
//        }

//        // Global変数とステータス描画系変数の同期。
//        private static void GlobalVariableLoad()
//        {
//            // 背景色
//            string argvname = "StatusWindow(BackBolor)";
//            if (Expression.IsGlobalVariableDefined(ref argvname))
//            {
//                string argexpr1 = "StatusWindow(BackBolor)";
//                if (!(StatusWindowBackBolor == Expression.GetValueAsLong(ref argexpr1)))
//                {
//                    string argexpr = "StatusWindow(BackBolor)";
//                    StatusWindowBackBolor = Expression.GetValueAsLong(ref argexpr);
//                }
//            }
//            // 枠の色
//            string argvname1 = "StatusWindow(FrameColor)";
//            if (Expression.IsGlobalVariableDefined(ref argvname1))
//            {
//                string argexpr3 = "StatusWindow(FrameColor)";
//                if (!(StatusWindowFrameColor == Expression.GetValueAsLong(ref argexpr3)))
//                {
//                    string argexpr2 = "StatusWindow(FrameColor)";
//                    StatusWindowFrameColor = Expression.GetValueAsLong(ref argexpr2);
//                }
//            }
//            // 枠の太さ
//            string argvname2 = "StatusWindow(FrameWidth)";
//            if (Expression.IsGlobalVariableDefined(ref argvname2))
//            {
//                string argexpr5 = "StatusWindow(FrameWidth)";
//                if (!(StatusWindowFrameWidth == Expression.GetValueAsLong(ref argexpr5)))
//                {
//                    string argexpr4 = "StatusWindow(FrameWidth)";
//                    StatusWindowFrameWidth = Expression.GetValueAsLong(ref argexpr4);
//                }
//            }
//            // 能力名の色
//            string argvname3 = "StatusWindow(ANameColor)";
//            if (Expression.IsGlobalVariableDefined(ref argvname3))
//            {
//                string argexpr7 = "StatusWindow(ANameColor)";
//                if (!(StatusFontColorAbilityName == Expression.GetValueAsLong(ref argexpr7)))
//                {
//                    string argexpr6 = "StatusWindow(ANameColor)";
//                    StatusFontColorAbilityName = Expression.GetValueAsLong(ref argexpr6);
//                }
//            }
//            // 有効な能力の色
//            string argvname4 = "StatusWindow(EnableColor)";
//            if (Expression.IsGlobalVariableDefined(ref argvname4))
//            {
//                string argexpr9 = "StatusWindow(EnableColor)";
//                if (!(StatusFontColorAbilityEnable == Expression.GetValueAsLong(ref argexpr9)))
//                {
//                    string argexpr8 = "StatusWindow(EnableColor)";
//                    StatusFontColorAbilityEnable = Expression.GetValueAsLong(ref argexpr8);
//                }
//            }
//            // 無効な能力の色
//            string argvname5 = "StatusWindow(DisableColor)";
//            if (Expression.IsGlobalVariableDefined(ref argvname5))
//            {
//                string argexpr11 = "StatusWindow(DisableColor)";
//                if (!(StatusFontColorAbilityDisable == Expression.GetValueAsLong(ref argexpr11)))
//                {
//                    string argexpr10 = "StatusWindow(DisableColor)";
//                    StatusFontColorAbilityDisable = Expression.GetValueAsLong(ref argexpr10);
//                }
//            }
//            // 通常文字の色
//            string argvname6 = "StatusWindow(StringColor)";
//            if (Expression.IsGlobalVariableDefined(ref argvname6))
//            {
//                string argexpr13 = "StatusWindow(StringColor)";
//                if (!(StatusFontColorNormalString == Expression.GetValueAsLong(ref argexpr13)))
//                {
//                    string argexpr12 = "StatusWindow(StringColor)";
//                    StatusFontColorNormalString = Expression.GetValueAsLong(ref argexpr12);
//                }
//            }
//        }
//        // ADD  END  240a
//    }
//}