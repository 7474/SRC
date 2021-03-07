// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace SRCCore
{
    // インターミッションに関する処理を行うモジュール
    public class InterMission
    {
        private SRC SRC { get; }
        private IGUI GUI => SRC.GUI;
        private Maps.Map Map => SRC.Map;
        private Events.Event Event => SRC.Event;
        private Expressions.Expression Expression => SRC.Expression;

        public InterMission(SRC src)
        {
            SRC = src;
        }

        // TODO Impl
        // インターミッション
        public void InterMissionCommand(bool skip_update = false)
        {
            //string[] cmd_list;
            //string[] name_list;
            //int j, i, ret;
            //string buf;
            //Unit u;
            //string fname, save_path = default;
            SRC.Stage = "インターミッション";
            SRC.IsSubStage = false;

            //// ＢＧＭを変更
            //Sound.KeepBGM = false;
            //Sound.BossBGM = false;
            //string argbgm_name2 = "Intermission";
            //if (Strings.InStr(Sound.BGMFileName, @"\" + Sound.BGMName(argbgm_name2)) == 0)
            //{
            //    Sound.StopBGM();
            //    string argbgm_name = "Intermission";
            //    string argbgm_name1 = Sound.BGMName(argbgm_name);
            //    Sound.StartBGM(argbgm_name1);
            //}

            //// マップをクリア
            //var loopTo = Map.MapWidth;
            //for (i = 1; i <= loopTo; i++)
            //{
            //    var loopTo1 = Map.MapHeight;
            //    for (j = 1; j <= loopTo1; j++)
            //        // UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //        Map.MapDataForUnit[i, j] = null;
            //}

            //// 各種データをアップデート
            //if (!skip_update)
            //{
            //    SRC.UList.Update();
            //    SRC.PList.Update();
            //    SRC.IList.Update();
            //}

            //Event.ClearEventData();
            //Map.ClearMap();

            // 選択用ダイアログを拡大
            GUI.EnlargeListBoxHeight();
            while (true)
            {
                // 利用可能なインターミッションコマンドを選択
                var cmd_list = new List<string>();
                cmd_list.Add("キャンセル");

                // 「次のステージへ」コマンド
                if (!string.IsNullOrEmpty(Expression.GetValueAsString("次ステージ")))
                {
                    cmd_list.Add("次ステージ");
                }

                // データセーブコマンド
                if (!Expression.IsOptionDefined("データセーブ不可") || Expression.IsOptionDefined("デバッグ"))
                {
                    cmd_list.Add("データセーブ");
                }

                // 機体改造コマンド
                if (!Expression.IsOptionDefined("改造不可"))
                {
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        cmd_list.Add("ユニットの強化");
                    }
                    else
                    {
                        if (SRC.UList.Items.Any(u => u.Party0 == "味方" && u.Status == "待機" && Strings.Left(u.Class, 1) == "("))
                        {
                            cmd_list.Add("ユニットの強化");
                        }
                        else
                        {
                            cmd_list.Add("機体改造");
                        }
                    }
                }

                // 乗り換えコマンド
                if (Expression.IsOptionDefined("乗り換え"))
                {
                    cmd_list.Add("乗り換え");
                }

                // アイテム交換コマンド
                string argoname5 = ;
                if (Expression.IsOptionDefined("アイテム交換"))
                {
                    cmd_list.Add("アイテム交換");
                }

                //換装コマンド
                //foreach (Unit currentU1 in SRC.UList)
                //{
                //    u = currentU1;
                //    if (u.Party0 == "味方" & u.Status_Renamed == "待機")
                //    {
                //        string argfname = "換装";
                //        if (u.IsFeatureAvailable(argfname))
                //        {
                //            object argIndex1 = "換装";
                //            string arglist = u.FeatureData(argIndex1);
                //            var loopTo2 = GeneralLib.LLength(arglist);
                //            for (i = 1; i <= loopTo2; i++)
                //            {
                //                string localLIndex() { object argIndex1 = "換装"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, i); return ret; }

                //                Unit localOtherForm() { object argIndex1 = (object)hs2400bc4ce27a4e82838570740e7e2b83(); var ret = u.OtherForm(argIndex1); return ret; }

                //                if (localOtherForm().IsAvailable())
                //                {
                //                    break;
                //                }
                //            }

                //            int localLLength() { object argIndex1 = "換装"; string arglist = u.FeatureData(argIndex1); var ret = GeneralLib.LLength(arglist); return ret; }

                //            if (i <= localLLength())
                //            {
                //                Array.Resize(cmd_list, Information.UBound(cmd_list) + 1 + 1);
                //                Array.Resize(GUI.ListItemFlag, Information.UBound(cmd_list) + 1);
                //                cmd_list[Information.UBound(cmd_list)] = "換装";
                //                break;
                //            }
                //        }
                //    }
                //}

                // パイロットステータスコマンド
                if (!Expression.IsOptionDefined("等身大基準"))
                {
                    cmd_list.Add("パイロットステータス");
                }

                // ユニットステータスコマンド
                cmd_list.Add("ユニットステータス");

                //// ユーザー定義のインターミッションコマンド
                //foreach (VarData var in Event_Renamed.GlobalVariableList)
                //{
                //    if (Strings.InStr(var.Name, "IntermissionCommand(") == 1)
                //    {
                //        ret = Strings.Len("IntermissionCommand(");
                //        buf = Strings.Mid(var.Name, ret + 1, Strings.Len(var.Name) - ret - 1);
                //        buf = Expression.GetValueAsString(buf);
                //        Expression.FormatMessage(buf);
                //        Array.Resize(cmd_list, Information.UBound(cmd_list) + 1 + 1);
                //        Array.Resize(GUI.ListItemFlag, Information.UBound(cmd_list) + 1);
                //        Array.Resize(GUI.ListItemID, Information.UBound(cmd_list) + 1);
                //        cmd_list[Information.UBound(cmd_list)] = buf;
                //        GUI.ListItemID[Information.UBound(cmd_list)] = var.Name;
                //    }
                //}

                // 終了コマンド
                cmd_list.Add("SRCを終了");

                // インターミッションのコマンド名称にエリアスを適用
                var name_list = cmd_list.Select(x => new ListBoxItem()
                {
                    Text = x,
                }).ToList();
                //name_list = new string[Information.UBound(cmd_list) + 1];
                //var loopTo3 = Information.UBound(name_list);
                //for (i = 1; i <= loopTo3; i++)
                //{
                //    name_list[i] = cmd_list[i];
                //    {
                //        var withBlock = SRC.ALDList;
                //        var loopTo4 = withBlock.Count();
                //        for (j = 1; j <= loopTo4; j++)
                //        {
                //            object argIndex2 = j;
                //            {
                //                var withBlock1 = withBlock.Item(argIndex2);
                //                if ((withBlock1.get_AliasType(1) ?? "") == (cmd_list[i] ?? ""))
                //                {
                //                    name_list[i] = withBlock1.Name;
                //                    break;
                //                }
                //            }
                //        }
                //    }
                //}

                // プレイヤーによるコマンド選択
                GUI.TopItem = 1;
                string arglb_caption = "インターミッション： 総ターン数"
                    + SrcFormatter.Format(SRC.TotalTurn) + " "
                    //+ Expression.Term("資金", u: argu)
                    + SrcFormatter.Format(SRC.Money);
                string arglb_info = "コマンド";
                string arglb_mode = "連続表示";
                var ret = GUI.ListBox(arglb_caption, name_list, arglb_info, arglb_mode);

                // 選択されたインターミッションコマンドを実行
                switch (cmd_list[ret] ?? "")
                {
                    case "次のステージへ":
                        {
                            if (Interaction.MsgBox("次のステージへ進みますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "次ステージ") == 1)
                            {
                                SRC.UList.Update(); // 追加パイロットを消去
                                My.MyProject.Forms.frmListBox.Hide();
                                GUI.ReduceListBoxHeight();
                                Sound.StopBGM();
                                return;
                            }

                            break;
                        }

                    //case "データセーブ":
                    //    {
                    //        // 一旦「常に手前に表示」を解除
                    //        if (My.MyProject.Forms.frmListBox.Visible)
                    //        {
                    //            ret = GUI.SetWindowPos(My.MyProject.Forms.frmListBox.Handle.ToInt32(), -2, 0, 0, 0, 0, 0x3);
                    //        }

                    //        string argdtitle = "データセーブ";
                    //        string argexpr1 = "セーブデータファイル名";
                    //        string argdefault_file = Expression.GetValueAsString(argexpr1);
                    //        string argftype = "ｾｰﾌﾞﾃﾞｰﾀ";
                    //        string argfsuffix = "src";
                    //        string argftype2 = "";
                    //        string argfsuffix2 = "";
                    //        string argftype3 = "";
                    //        string argfsuffix3 = "";
                    //        fname = FileDialog.SaveFileDialog(argdtitle, SRC.ScenarioPath, argdefault_file, 2, argftype, argfsuffix, ftype2: argftype2, fsuffix2: argfsuffix2, ftype3: argftype3, fsuffix3: argfsuffix3);

                    //        // 再び「常に手前に表示」
                    //        if (My.MyProject.Forms.frmListBox.Visible)
                    //        {
                    //            ret = GUI.SetWindowPos(My.MyProject.Forms.frmListBox.Handle.ToInt32(), -1, 0, 0, 0, 0, 0x3);
                    //        }

                    //        // キャンセル？
                    //        if (string.IsNullOrEmpty(fname))
                    //        {
                    //            goto NextLoop;
                    //        }

                    //        // セーブ先はシナリオフォルダ？
                    //        if (Strings.InStr(fname, @"\") > 0)
                    //        {
                    //            string argstr2 = @"\";
                    //            save_path = Strings.Left(fname, GeneralLib.InStr2(fname, argstr2));
                    //        }
                    //        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                    //        if ((FileSystem.Dir(save_path) ?? "") != (FileSystem.Dir(SRC.ScenarioPath) ?? ""))
                    //        {
                    //            if (Interaction.MsgBox("セーブファイルはシナリオフォルダにないと読み込めません。" + Constants.vbCr + Constants.vbLf + "このままセーブしますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question)) != 1)
                    //            {
                    //                goto NextLoop;
                    //            }
                    //        }

                    //        if (!string.IsNullOrEmpty(fname))
                    //        {
                    //            SRC.UList.Update(); // 追加パイロットを消去
                    //            SRC.SaveData(fname);
                    //        }

                    //        break;
                    //    }

                    //case "機体改造":
                    //case "ユニットの強化":
                    //    {
                    //        RankUpCommand();
                    //        break;
                    //    }

                    //case "乗り換え":
                    //    {
                    //        ExchangeUnitCommand();
                    //        break;
                    //    }

                    //case "アイテム交換":
                    //    {
                    //        Unit argselected_unit = null;
                    //        string argselected_part = "";
                    //        ExchangeItemCommand(selected_unit: argselected_unit, selected_part: argselected_part);
                    //        break;
                    //    }

                    //case "換装":
                    //    {
                    //        ExchangeFormCommand();
                    //        break;
                    //    }

                    case "SRCを終了":
                        {
                            if (Interaction.MsgBox("SRCを終了しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "終了") == 1)
                            {
                                My.MyProject.Forms.frmListBox.Hide();
                                GUI.ReduceListBoxHeight();
                                SRC.ExitGame();
                            }

                            break;
                        }

                    //case "パイロットステータス":
                    //    {
                    //        My.MyProject.Forms.frmListBox.Hide();
                    //        GUI.ReduceListBoxHeight();
                    //        SRC.IsSubStage = true;
                    //        bool localFileExists() { string argfname = SRC.ExtDataPath + @"Lib\パイロットステータス表示.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    //        bool localFileExists1() { string argfname = SRC.ExtDataPath2 + @"Lib\パイロットステータス表示.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    //        string argfname1 = SRC.ScenarioPath + @"Lib\パイロットステータス表示.eve";
                    //        if (GeneralLib.FileExists(argfname1))
                    //        {
                    //            SRC.StartScenario(SRC.ScenarioPath + @"Lib\パイロットステータス表示.eve");
                    //        }
                    //        else if (localFileExists())
                    //        {
                    //            SRC.StartScenario(SRC.ExtDataPath + @"Lib\パイロットステータス表示.eve");
                    //        }
                    //        else if (localFileExists1())
                    //        {
                    //            SRC.StartScenario(SRC.ExtDataPath2 + @"Lib\パイロットステータス表示.eve");
                    //        }
                    //        else
                    //        {
                    //            SRC.StartScenario(SRC.AppPath + @"Lib\パイロットステータス表示.eve");
                    //        }
                    //        // サブステージを通常のステージとして実行
                    //        SRC.IsSubStage = true;
                    //        return;
                    //    }

                    //case "ユニットステータス":
                    //    {
                    //        My.MyProject.Forms.frmListBox.Hide();
                    //        GUI.ReduceListBoxHeight();
                    //        SRC.IsSubStage = true;
                    //        bool localFileExists2() { string argfname = SRC.ExtDataPath + @"Lib\ユニットステータス表示.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    //        bool localFileExists3() { string argfname = SRC.ExtDataPath2 + @"Lib\ユニットステータス表示.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    //        string argfname2 = SRC.ScenarioPath + @"Lib\ユニットステータス表示.eve";
                    //        if (GeneralLib.FileExists(argfname2))
                    //        {
                    //            SRC.StartScenario(SRC.ScenarioPath + @"Lib\ユニットステータス表示.eve");
                    //        }
                    //        else if (localFileExists2())
                    //        {
                    //            SRC.StartScenario(SRC.ExtDataPath + @"Lib\ユニットステータス表示.eve");
                    //        }
                    //        else if (localFileExists3())
                    //        {
                    //            SRC.StartScenario(SRC.ExtDataPath2 + @"Lib\ユニットステータス表示.eve");
                    //        }
                    //        else
                    //        {
                    //            SRC.StartScenario(SRC.AppPath + @"Lib\ユニットステータス表示.eve");
                    //        }
                    //        // サブステージを通常のステージとして実行
                    //        SRC.IsSubStage = true;
                    //        return;
                    //    }
                    // キャンセル
                    case "キャンセル":
                        {
                            break;
                        }

                    // ユーザー定義のインターミッションコマンド
                    default:
                        {
                            My.MyProject.Forms.frmListBox.Hide();
                            GUI.ReduceListBoxHeight();
                            SRC.IsSubStage = true;
                            SRC.StartScenario(Expression.GetValueAsString(GUI.ListItemID[ret]));
                            if (SRC.IsSubStage)
                            {
                                // インターミッションを再開
                                Sound.KeepBGM = false;
                                Sound.BossBGM = false;
                                string argbgm_name3 = "Intermission";
                                string argbgm_name4 = Sound.BGMName(argbgm_name3);
                                Sound.ChangeBGM(argbgm_name4);
                                SRC.UList.Update();
                                SRC.PList.Update();
                                SRC.IList.Update();
                                Event_Renamed.ClearEventData();
                                if (Map.MapWidth > 1)
                                {
                                    Map.ClearMap();
                                }

                                SRC.IsSubStage = false;
                                GUI.EnlargeListBoxHeight();
                            }
                            else
                            {
                                // サブステージを通常のステージとして実行
                                SRC.IsSubStage = true;
                                return;
                            }

                            break;
                        }
                }
            }
        }

        //// 機体改造コマンド
        //public void RankUpCommand()
        //{
        //    int k, i, j, urank;
        //    string[] list;
        //    string[] id_list;
        //    string sort_mode;
        //    var sort_mode_type = new string[8];
        //    var sort_mode_list = new string[8];
        //    bool[] item_flag_backup;
        //    string[] item_comment_backup;
        //    int[] key_list;
        //    string[] strkey_list;
        //    int max_item;
        //    int max_value;
        //    string max_str;
        //    Unit u;
        //    int cost;
        //    string buf;
        //    int ret;
        //    bool b;
        //    var use_max_rank = default(bool);
        //    int name_width;
        //    GUI.TopItem = 1;

        //    // デフォルトのソート方法
        //    string argoname = "等身大基準";
        //    if (Expression.IsOptionDefined(argoname))
        //    {
        //        sort_mode = "レベル";
        //    }
        //    else
        //    {
        //        sort_mode = "ＨＰ";
        //    }

        //    // 最大改造数がユニット毎に変更されているかをあらかじめチェック
        //    foreach (Unit currentU in SRC.UList)
        //    {
        //        u = currentU;
        //        string argfname = "最大改造数";
        //        if (u.IsFeatureAvailable(argfname))
        //        {
        //            use_max_rank = true;
        //            break;
        //        }
        //    }

        //    // ユニット名の項の文字数を設定
        //    name_width = 33;
        //    if (use_max_rank)
        //    {
        //        name_width = (name_width - 2);
        //    }

        //    string argoname1 = "等身大基準";
        //    if (Expression.IsOptionDefined(argoname1))
        //    {
        //        name_width = (name_width + 8);
        //    }

        //    // ユニットのリストを作成
        //    list = new string[2];
        //    id_list = new string[2];
        //    GUI.ListItemFlag = new bool[2];
        //    GUI.ListItemComment = new string[2];
        //    list[1] = "▽並べ替え▽";
        //    foreach (Unit currentU1 in SRC.UList)
        //    {
        //        u = currentU1;
        //        {
        //            var withBlock = u;
        //            if (withBlock.Party0 != "味方" | withBlock.Status_Renamed != "待機")
        //            {
        //                goto NextLoop;
        //            }

        //            Array.Resize(list, Information.UBound(list) + 1 + 1);
        //            Array.Resize(id_list, Information.UBound(list) + 1);
        //            Array.Resize(GUI.ListItemFlag, Information.UBound(list) + 1);
        //            Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

        //            // 改造が可能？
        //            cost = RankUpCost(u);
        //            if (cost > SRC.Money | cost > 10000000)
        //            {
        //                GUI.ListItemFlag[Information.UBound(list)] = true;
        //            }

        //            // ユニットランク
        //            if (use_max_rank)
        //            {
        //                string localRightPaddedString() { string argbuf = withBlock.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock.Nickname0 = argbuf; return ret; }

        //                string localLeftPaddedString() { string argbuf = SrcFormatter.Format(withBlock.Rank); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

        //                list[Information.UBound(list)] = localRightPaddedString() + localLeftPaddedString() + "/";
        //                if (MaxRank(u) > 0)
        //                {
        //                    string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(MaxRank(u)); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString1();
        //                }
        //                else
        //                {
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + "--";
        //                }
        //            }
        //            else if (withBlock.Rank < 10)
        //            {
        //                string localRightPaddedString1() { string argbuf = withBlock.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock.Nickname0 = argbuf; return ret; }

        //                list[Information.UBound(list)] = localRightPaddedString1() + Strings.StrConv(SrcFormatter.Format(withBlock.Rank), VbStrConv.Wide);
        //            }
        //            else
        //            {
        //                string localRightPaddedString2() { string argbuf = withBlock.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock.Nickname0 = argbuf; return ret; }

        //                list[Information.UBound(list)] = localRightPaddedString2() + SrcFormatter.Format(withBlock.Rank);
        //            }

        //            // 改造に必要な資金
        //            if (cost < 10000000)
        //            {
        //                string localLeftPaddedString2() { string argbuf = SrcFormatter.Format(cost); var ret = GeneralLib.LeftPaddedString(argbuf, 7); return ret; }

        //                list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString2();
        //            }
        //            else
        //            {
        //                string argbuf = "----";
        //                list[Information.UBound(list)] = list[Information.UBound(list)] + GeneralLib.LeftPaddedString(argbuf, 7);
        //            }

        //            // 等身大基準の場合はパイロットレベルも表示
        //            string argoname2 = "等身大基準";
        //            if (Expression.IsOptionDefined(argoname2))
        //            {
        //                if (withBlock.CountPilot() > 0)
        //                {
        //                    string localLeftPaddedString3() { string argbuf = SrcFormatter.Format(withBlock.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString3();
        //                }
        //            }

        //            // ユニットに関する情報
        //            string localLeftPaddedString4() { string argbuf = SrcFormatter.Format(withBlock.MaxHP); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

        //            string localLeftPaddedString5() { string argbuf = SrcFormatter.Format(withBlock.MaxEN); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

        //            string localLeftPaddedString6() { string argbuf = SrcFormatter.Format(withBlock.get_Armor("")); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

        //            string localLeftPaddedString7() { string argbuf = SrcFormatter.Format(withBlock.get_Mobility("")); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

        //            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString4() + localLeftPaddedString5() + localLeftPaddedString6() + localLeftPaddedString7();

        //            // 等身大基準でない場合はパイロット名を表示
        //            string argoname3 = "等身大基準";
        //            if (!Expression.IsOptionDefined(argoname3))
        //            {
        //                if (withBlock.CountPilot() > 0)
        //                {
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + "  " + withBlock.MainPilot().get_Nickname(false);
        //                }
        //            }

        //            // 装備しているアイテムをコメント欄に列記
        //            var loopTo = withBlock.CountItem();
        //            for (k = 1; k <= loopTo; k++)
        //            {
        //                object argIndex1 = k;
        //                {
        //                    var withBlock1 = withBlock.Item(argIndex1);
        //                    string argfname1 = "非表示";
        //                    if ((withBlock1.Class_Renamed() != "固定" | !withBlock1.IsFeatureAvailable(argfname1)) & withBlock1.Part() != "非表示")
        //                    {
        //                        GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + withBlock1.Nickname() + " ";
        //                    }
        //                }
        //            }

        //            // ユニットＩＤを記録しておく
        //            id_list[Information.UBound(list)] = withBlock.ID;
        //        }

        //    NextLoop:
        //        ;
        //    }

        //Beginning:
        //    ;


        //    // ソート
        //    if (Strings.InStr(sort_mode, "名称") == 0)
        //    {
        //        // 数値を使ったソート

        //        // まず並べ替えに使うキーのリストを作成
        //        key_list = new int[Information.UBound(list) + 1];
        //        {
        //            var withBlock2 = SRC.UList;
        //            switch (sort_mode ?? "")
        //            {
        //                case "ＨＰ":
        //                    {
        //                        var loopTo1 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo1; i++)
        //                        {
        //                            Unit localItem() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem().MaxHP;
        //                        }

        //                        break;
        //                    }

        //                case "ＥＮ":
        //                    {
        //                        var loopTo2 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo2; i++)
        //                        {
        //                            Unit localItem1() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem1().MaxEN;
        //                        }

        //                        break;
        //                    }

        //                case "装甲":
        //                    {
        //                        var loopTo3 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo3; i++)
        //                        {
        //                            Unit localItem2() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem2().get_Armor("");
        //                        }

        //                        break;
        //                    }

        //                case "運動性":
        //                    {
        //                        var loopTo4 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo4; i++)
        //                        {
        //                            Unit localItem3() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem3().get_Mobility("");
        //                        }

        //                        break;
        //                    }

        //                case "ユニットランク":
        //                    {
        //                        var loopTo5 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo5; i++)
        //                        {
        //                            Unit localItem4() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem4().Rank;
        //                        }

        //                        break;
        //                    }

        //                case "レベル":
        //                    {
        //                        var loopTo6 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo6; i++)
        //                        {
        //                            var tmp = id_list;
        //                            object argIndex2 = tmp[i];
        //                            {
        //                                var withBlock3 = withBlock2.Item(argIndex2);
        //                                if (withBlock3.CountPilot() > 0)
        //                                {
        //                                    {
        //                                        var withBlock4 = withBlock3.MainPilot();
        //                                        key_list[i] = 500 * withBlock4.Level + withBlock4.Exp;
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        break;
        //                    }
        //            }
        //        }

        //        // キーを使って並べ換え
        //        var loopTo7 = (Information.UBound(list) - 1);
        //        for (i = 2; i <= loopTo7; i++)
        //        {
        //            max_item = i;
        //            max_value = key_list[i];
        //            var loopTo8 = Information.UBound(list);
        //            for (j = (i + 1); j <= loopTo8; j++)
        //            {
        //                if (key_list[j] > max_value)
        //                {
        //                    max_item = j;
        //                    max_value = key_list[j];
        //                }
        //            }

        //            if (max_item != i)
        //            {
        //                buf = list[i];
        //                list[i] = list[max_item];
        //                list[max_item] = buf;
        //                buf = id_list[i];
        //                id_list[i] = id_list[max_item];
        //                id_list[max_item] = buf;
        //                b = GUI.ListItemFlag[i];
        //                GUI.ListItemFlag[i] = GUI.ListItemFlag[max_item];
        //                GUI.ListItemFlag[max_item] = b;
        //                buf = GUI.ListItemComment[i];
        //                GUI.ListItemComment[i] = GUI.ListItemComment[max_item];
        //                GUI.ListItemComment[max_item] = buf;
        //                key_list[max_item] = key_list[i];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // 数値を使ったソート

        //        // まず並べ替えに使うキーのリストを作成
        //        strkey_list = new string[Information.UBound(list) + 1];
        //        {
        //            var withBlock5 = SRC.UList;
        //            switch (sort_mode ?? "")
        //            {
        //                case "名称":
        //                case "ユニット名称":
        //                    {
        //                        var loopTo9 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo9; i++)
        //                        {
        //                            Unit localItem5() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock5.Item(argIndex1); return ret; }

        //                            strkey_list[i] = localItem5().KanaName;
        //                        }

        //                        break;
        //                    }

        //                case "パイロット名称":
        //                    {
        //                        var loopTo10 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo10; i++)
        //                        {
        //                            var tmp1 = id_list;
        //                            object argIndex3 = tmp1[i];
        //                            {
        //                                var withBlock6 = withBlock5.Item(argIndex3);
        //                                if (withBlock6.CountPilot() > 0)
        //                                {
        //                                    strkey_list[i] = withBlock6.MainPilot().KanaName;
        //                                }
        //                            }
        //                        }

        //                        break;
        //                    }
        //            }
        //        }

        //        // キーを使って並べ換え
        //        var loopTo11 = (Information.UBound(strkey_list) - 1);
        //        for (i = 2; i <= loopTo11; i++)
        //        {
        //            max_item = i;
        //            max_str = strkey_list[i];
        //            var loopTo12 = Information.UBound(strkey_list);
        //            for (j = (i + 1); j <= loopTo12; j++)
        //            {
        //                if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
        //                {
        //                    max_item = j;
        //                    max_str = strkey_list[j];
        //                }
        //            }

        //            if (max_item != i)
        //            {
        //                buf = list[i];
        //                list[i] = list[max_item];
        //                list[max_item] = buf;
        //                buf = id_list[i];
        //                id_list[i] = id_list[max_item];
        //                id_list[max_item] = buf;
        //                b = GUI.ListItemFlag[i];
        //                GUI.ListItemFlag[i] = GUI.ListItemFlag[max_item];
        //                GUI.ListItemFlag[max_item] = b;
        //                buf = GUI.ListItemComment[i];
        //                GUI.ListItemComment[i] = GUI.ListItemComment[max_item];
        //                GUI.ListItemComment[max_item] = buf;
        //                strkey_list[max_item] = strkey_list[i];
        //            }
        //        }
        //    }

        //    // 改造するユニットを選択
        //    string argoname4 = "等身大基準";
        //    if (Expression.IsOptionDefined(argoname4))
        //    {
        //        if (use_max_rank)
        //        {
        //            string argtname = "資金";
        //            Unit argu = null;
        //            string arglb_caption = "ユニット選択： " + Expression.Term(argtname, u: argu) + SrcFormatter.Format(SRC.Money);
        //            string argtname1 = "ランク";
        //            Unit argu1 = null;
        //            string argtname2 = "ＨＰ";
        //            Unit argu2 = null;
        //            string argtname3 = "ＥＮ";
        //            Unit argu3 = null;
        //            string argtname4 = "装甲";
        //            Unit argu4 = null;
        //            string argtname5 = "運動";
        //            Unit argu5 = null;
        //            string arglb_info = "ユニット                               " + Expression.Term(argtname1, argu1, 6) + "  費用 Lv  " + Expression.Term(argtname2, argu2, 4) + " " + Expression.Term(argtname3, argu3, 4) + " " + Expression.Term(argtname4, argu4, 4) + " " + Expression.Term(argtname5, u: argu5);
        //            string arglb_mode = "連続表示,コメント";
        //            ret = GUI.ListBox(arglb_caption, list, arglb_info, arglb_mode);
        //        }
        //        else
        //        {
        //            string argtname6 = "資金";
        //            Unit argu6 = null;
        //            string arglb_caption1 = "ユニット選択： " + Expression.Term(argtname6, u: argu6) + SrcFormatter.Format(SRC.Money);
        //            string argtname7 = "ランク";
        //            Unit argu7 = null;
        //            string argtname8 = "ＨＰ";
        //            Unit argu8 = null;
        //            string argtname9 = "ＥＮ";
        //            Unit argu9 = null;
        //            string argtname10 = "装甲";
        //            Unit argu10 = null;
        //            string argtname11 = "運動";
        //            Unit argu11 = null;
        //            string arglb_info1 = "ユニット                             " + Expression.Term(argtname7, argu7, 6) + "   費用 Lv  " + Expression.Term(argtname8, argu8, 4) + " " + Expression.Term(argtname9, argu9, 4) + " " + Expression.Term(argtname10, argu10, 4) + " " + Expression.Term(argtname11, u: argu11);
        //            string arglb_mode1 = "連続表示,コメント";
        //            ret = GUI.ListBox(arglb_caption1, list, arglb_info1, arglb_mode1);
        //        }
        //    }
        //    else if (use_max_rank)
        //    {
        //        string argtname12 = "資金";
        //        Unit argu12 = null;
        //        string arglb_caption2 = "ユニット選択： " + Expression.Term(argtname12, u: argu12) + SrcFormatter.Format(SRC.Money);
        //        string argtname13 = "ランク";
        //        Unit argu13 = null;
        //        string argtname14 = "ＨＰ";
        //        Unit argu14 = null;
        //        string argtname15 = "ＥＮ";
        //        Unit argu15 = null;
        //        string argtname16 = "装甲";
        //        Unit argu16 = null;
        //        string argtname17 = "運動";
        //        Unit argu17 = null;
        //        string arglb_info2 = "ユニット                       " + Expression.Term(argtname13, argu13, 6) + "  費用  " + Expression.Term(argtname14, argu14, 4) + " " + Expression.Term(argtname15, argu15, 4) + " " + Expression.Term(argtname16, argu16, 4) + " " + Expression.Term(argtname17, argu17, 4) + " パイロット";
        //        string arglb_mode2 = "連続表示,コメント";
        //        ret = GUI.ListBox(arglb_caption2, list, arglb_info2, arglb_mode2);
        //    }
        //    else
        //    {
        //        string argtname18 = "資金";
        //        Unit argu18 = null;
        //        string arglb_caption3 = "ユニット選択： " + Expression.Term(argtname18, u: argu18) + SrcFormatter.Format(SRC.Money);
        //        string argtname19 = "ランク";
        //        Unit argu19 = null;
        //        string argtname20 = "ＨＰ";
        //        Unit argu20 = null;
        //        string argtname21 = "ＥＮ";
        //        Unit argu21 = null;
        //        string argtname22 = "装甲";
        //        Unit argu22 = null;
        //        string argtname23 = "運動";
        //        Unit argu23 = null;
        //        string arglb_info3 = "ユニット                     " + Expression.Term(argtname19, argu19, 6) + "   費用  " + Expression.Term(argtname20, argu20, 4) + " " + Expression.Term(argtname21, argu21, 4) + " " + Expression.Term(argtname22, argu22, 4) + " " + Expression.Term(argtname23, argu23, 4) + " パイロット";
        //        string arglb_mode3 = "連続表示,コメント";
        //        ret = GUI.ListBox(arglb_caption3, list, arglb_info3, arglb_mode3);
        //    }

        //    switch (ret)
        //    {
        //        case 0:
        //            {
        //                // キャンセル
        //                return;
        //            }

        //        case 1:
        //            {
        //                // ソート方法を選択
        //                string argoname5 = "等身大基準";
        //                if (Expression.IsOptionDefined(argoname5))
        //                {
        //                    sort_mode_type[1] = "名称";
        //                    sort_mode_list[1] = "名称";
        //                    sort_mode_type[2] = "レベル";
        //                    sort_mode_list[2] = "レベル";
        //                    sort_mode_type[3] = "ＨＰ";
        //                    string argtname24 = "ＨＰ";
        //                    Unit argu24 = null;
        //                    sort_mode_list[3] = Expression.Term(argtname24, u: argu24);
        //                    sort_mode_type[4] = "ＥＮ";
        //                    string argtname25 = "ＥＮ";
        //                    Unit argu25 = null;
        //                    sort_mode_list[4] = Expression.Term(argtname25, u: argu25);
        //                    sort_mode_type[5] = "装甲";
        //                    string argtname26 = "装甲";
        //                    Unit argu26 = null;
        //                    sort_mode_list[5] = Expression.Term(argtname26, u: argu26);
        //                    sort_mode_type[6] = "運動性";
        //                    string argtname27 = "運動性";
        //                    Unit argu27 = null;
        //                    sort_mode_list[6] = Expression.Term(argtname27, u: argu27);
        //                    sort_mode_type[7] = "ユニットランク";
        //                    string argtname28 = "ランク";
        //                    Unit argu28 = null;
        //                    sort_mode_list[7] = Expression.Term(argtname28, u: argu28);
        //                }
        //                else
        //                {
        //                    sort_mode_type[1] = "ＨＰ";
        //                    string argtname29 = "ＨＰ";
        //                    Unit argu29 = null;
        //                    sort_mode_list[1] = Expression.Term(argtname29, u: argu29);
        //                    sort_mode_type[2] = "ＥＮ";
        //                    string argtname30 = "ＥＮ";
        //                    Unit argu30 = null;
        //                    sort_mode_list[2] = Expression.Term(argtname30, u: argu30);
        //                    sort_mode_type[3] = "装甲";
        //                    string argtname31 = "装甲";
        //                    Unit argu31 = null;
        //                    sort_mode_list[3] = Expression.Term(argtname31, u: argu31);
        //                    sort_mode_type[4] = "運動性";
        //                    string argtname32 = "運動性";
        //                    Unit argu32 = null;
        //                    sort_mode_list[4] = Expression.Term(argtname32, u: argu32);
        //                    sort_mode_type[5] = "ユニットランク";
        //                    string argtname33 = "ランク";
        //                    Unit argu33 = null;
        //                    sort_mode_list[5] = Expression.Term(argtname33, u: argu33);
        //                    sort_mode_type[6] = "ユニット名称";
        //                    sort_mode_list[6] = "ユニット名称";
        //                    sort_mode_type[7] = "パイロット名称";
        //                    sort_mode_list[7] = "パイロット名称";
        //                }

        //                item_flag_backup = new bool[Information.UBound(list) + 1];
        //                item_comment_backup = new string[Information.UBound(list) + 1];
        //                var loopTo13 = Information.UBound(list);
        //                for (i = 2; i <= loopTo13; i++)
        //                {
        //                    item_flag_backup[i] = GUI.ListItemFlag[i];
        //                    item_comment_backup[i] = GUI.ListItemComment[i];
        //                }

        //                GUI.ListItemComment = new string[Information.UBound(sort_mode_list) + 1];
        //                GUI.ListItemFlag = new bool[Information.UBound(sort_mode_list) + 1];
        //                string arglb_caption4 = "どれで並べ替えますか？";
        //                string arglb_info4 = "並べ替えの方法";
        //                string arglb_mode4 = "連続表示,コメント";
        //                ret = GUI.ListBox(arglb_caption4, sort_mode_list, arglb_info4, arglb_mode4);
        //                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
        //                GUI.ListItemComment = new string[Information.UBound(list) + 1];
        //                var loopTo14 = Information.UBound(list);
        //                for (i = 2; i <= loopTo14; i++)
        //                {
        //                    GUI.ListItemFlag[i] = item_flag_backup[i];
        //                    GUI.ListItemComment[i] = item_comment_backup[i];
        //                }

        //                // ソート方法を変更して再表示
        //                if (ret > 0)
        //                {
        //                    sort_mode = sort_mode_type[ret];
        //                }

        //                goto Beginning;
        //                break;
        //            }
        //    }

        //    // 改造するユニットを検索
        //    var tmp2 = id_list;
        //    object argIndex4 = tmp2[ret];
        //    u = SRC.UList.Item(argIndex4);

        //    // 改造するか確認
        //    if (u.IsHero())
        //    {
        //        if (Interaction.MsgBox(u.Nickname0 + "をパワーアップさせますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "パワーアップ") != 1)
        //        {
        //            goto Beginning;
        //        }
        //    }
        //    else if (Interaction.MsgBox(u.Nickname0 + "を改造しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "改造") != 1)
        //    {
        //        goto Beginning;
        //    }

        //    // 資金を減らす
        //    SRC.IncrMoney(-RankUpCost(u));

        //    // ユニットランクを一段階上げる
        //    {
        //        var withBlock7 = u;
        //        withBlock7.Rank = (withBlock7.Rank + 1);
        //        withBlock7.HP = withBlock7.MaxHP;
        //        withBlock7.EN = withBlock7.MaxEN;

        //        // 他形態のランクも上げておく
        //        var loopTo15 = withBlock7.CountOtherForm();
        //        for (i = 1; i <= loopTo15; i++)
        //        {
        //            Unit localOtherForm() { object argIndex1 = i; var ret = withBlock7.OtherForm(argIndex1); return ret; }

        //            localOtherForm().Rank = withBlock7.Rank;
        //            Unit localOtherForm1() { object argIndex1 = i; var ret = withBlock7.OtherForm(argIndex1); return ret; }

        //            Unit localOtherForm2() { object argIndex1 = i; var ret = withBlock7.OtherForm(argIndex1); return ret; }

        //            localOtherForm1().HP = localOtherForm2().MaxHP;
        //            Unit localOtherForm3() { object argIndex1 = i; var ret = withBlock7.OtherForm(argIndex1); return ret; }

        //            Unit localOtherForm4() { object argIndex1 = i; var ret = withBlock7.OtherForm(argIndex1); return ret; }

        //            localOtherForm3().EN = localOtherForm4().MaxEN;
        //        }

        //        // 合体形態が主形態の分離形態が改造された場合は他の分離形態のユニットの
        //        // ランクも上げる
        //        string argfname4 = "合体";
        //        if (withBlock7.IsFeatureAvailable(argfname4))
        //        {
        //            var loopTo16 = withBlock7.CountFeature();
        //            for (i = 1; i <= loopTo16; i++)
        //            {
        //                object argIndex7 = i;
        //                if (withBlock7.Feature(argIndex7) == "合体")
        //                {
        //                    string localFeatureData() { object argIndex1 = i; var ret = withBlock7.FeatureData(argIndex1); return ret; }

        //                    string arglist = localFeatureData();
        //                    buf = GeneralLib.LIndex(arglist, 2);
        //                    string localFeatureData1() { object argIndex1 = i; var ret = withBlock7.FeatureData(argIndex1); return ret; }

        //                    string arglist1 = localFeatureData1();
        //                    if (GeneralLib.LLength(arglist1) == 3)
        //                    {
        //                        object argIndex5 = buf;
        //                        if (SRC.UDList.IsDefined(argIndex5))
        //                        {
        //                            UnitData localItem6() { object argIndex1 = buf; var ret = SRC.UDList.Item(argIndex1); return ret; }

        //                            string argfname2 = "主形態";
        //                            if (localItem6().IsFeatureAvailable(argfname2))
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                    else
        //                    {
        //                        object argIndex6 = buf;
        //                        if (SRC.UDList.IsDefined(argIndex6))
        //                        {
        //                            UnitData localItem7() { object argIndex1 = buf; var ret = SRC.UDList.Item(argIndex1); return ret; }

        //                            string argfname3 = "制限時間";
        //                            if (!localItem7().IsFeatureAvailable(argfname3))
        //                            {
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //            }

        //            if (i <= withBlock7.CountFeature())
        //            {
        //                urank = withBlock7.Rank;
        //                string localFeatureData2() { object argIndex1 = i; var ret = withBlock7.FeatureData(argIndex1); return ret; }

        //                string localLIndex() { string arglist = hsba79967d549343448297e0bcf57b2982(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

        //                UnitData localItem8() { object argIndex1 = (object)hsd871da3601884bc086fad5045542bc83(); var ret = SRC.UDList.Item(argIndex1); return ret; }

        //                object argIndex8 = "分離";
        //                buf = localItem8().FeatureData(argIndex8);
        //                var loopTo17 = GeneralLib.LLength(buf);
        //                for (i = 2; i <= loopTo17; i++)
        //                {
        //                    bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(buf, i); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

        //                    if (!localIsDefined())
        //                    {
        //                        goto NextForm;
        //                    }

        //                    object argIndex9 = GeneralLib.LIndex(buf, i);
        //                    {
        //                        var withBlock8 = SRC.UList.Item(argIndex9);
        //                        withBlock8.Rank = GeneralLib.MaxLng(urank, withBlock8.Rank);
        //                        withBlock8.HP = withBlock8.MaxHP;
        //                        withBlock8.EN = withBlock8.MaxEN;
        //                        var loopTo18 = withBlock8.CountOtherForm();
        //                        for (j = 1; j <= loopTo18; j++)
        //                        {
        //                            Unit localOtherForm5() { object argIndex1 = j; var ret = withBlock8.OtherForm(argIndex1); return ret; }

        //                            localOtherForm5().Rank = withBlock8.Rank;
        //                            Unit localOtherForm6() { object argIndex1 = j; var ret = withBlock8.OtherForm(argIndex1); return ret; }

        //                            Unit localOtherForm7() { object argIndex1 = j; var ret = withBlock8.OtherForm(argIndex1); return ret; }

        //                            localOtherForm6().HP = localOtherForm7().MaxHP;
        //                            Unit localOtherForm8() { object argIndex1 = j; var ret = withBlock8.OtherForm(argIndex1); return ret; }

        //                            Unit localOtherForm9() { object argIndex1 = j; var ret = withBlock8.OtherForm(argIndex1); return ret; }

        //                            localOtherForm8().EN = localOtherForm9().MaxEN;
        //                        }

        //                        var loopTo19 = Information.UBound(id_list);
        //                        for (j = 1; j <= loopTo19; j++)
        //                        {
        //                            if ((withBlock8.CurrentForm().ID ?? "") == (id_list[j] ?? ""))
        //                            {
        //                                break;
        //                            }
        //                        }

        //                        if (j > Information.UBound(id_list))
        //                        {
        //                            goto NextForm;
        //                        }

        //                        if (use_max_rank)
        //                        {
        //                            string localRightPaddedString3() { string argbuf = withBlock8.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock8.Nickname0 = argbuf; return ret; }

        //                            string localLeftPaddedString8() { string argbuf = SrcFormatter.Format(withBlock8.Rank); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

        //                            list[j] = localRightPaddedString3() + localLeftPaddedString8() + "/";
        //                            if (MaxRank(u) > 0)
        //                            {
        //                                string localLeftPaddedString9() { string argbuf = SrcFormatter.Format(MaxRank(u)); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

        //                                list[j] = list[j] + localLeftPaddedString9();
        //                            }
        //                            else
        //                            {
        //                                list[j] = list[j] + "--";
        //                            }
        //                        }
        //                        else if (withBlock8.Rank < 10)
        //                        {
        //                            string localRightPaddedString4() { string argbuf = withBlock8.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock8.Nickname0 = argbuf; return ret; }

        //                            list[j] = localRightPaddedString4() + Strings.StrConv(SrcFormatter.Format(withBlock8.Rank), VbStrConv.Wide);
        //                        }
        //                        else
        //                        {
        //                            string localRightPaddedString5() { string argbuf = withBlock8.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock8.Nickname0 = argbuf; return ret; }

        //                            list[j] = localRightPaddedString5() + SrcFormatter.Format(withBlock8.Rank);
        //                        }

        //                        if (RankUpCost(u) < 1000000)
        //                        {
        //                            string localLeftPaddedString10() { string argbuf = SrcFormatter.Format(RankUpCost(u)); var ret = GeneralLib.LeftPaddedString(argbuf, 7); return ret; }

        //                            list[j] = list[j] + localLeftPaddedString10();
        //                        }
        //                        else
        //                        {
        //                            string argbuf1 = "----";
        //                            list[j] = list[j] + GeneralLib.LeftPaddedString(argbuf1, 7);
        //                        }

        //                        string argoname6 = "等身大基準";
        //                        if (Expression.IsOptionDefined(argoname6))
        //                        {
        //                            if (withBlock8.CountPilot() > 0)
        //                            {
        //                                string localLeftPaddedString11() { string argbuf = SrcFormatter.Format(withBlock8.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

        //                                list[j] = list[j] + localLeftPaddedString11();
        //                            }
        //                        }

        //                        string localLeftPaddedString12() { string argbuf = SrcFormatter.Format(withBlock8.MaxHP); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

        //                        string localLeftPaddedString13() { string argbuf = SrcFormatter.Format(withBlock8.MaxEN); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

        //                        string localLeftPaddedString14() { string argbuf = SrcFormatter.Format(withBlock8.get_Armor("")); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

        //                        string localLeftPaddedString15() { string argbuf = SrcFormatter.Format(withBlock8.get_Mobility("")); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

        //                        list[j] = list[j] + localLeftPaddedString12() + localLeftPaddedString13() + localLeftPaddedString14() + localLeftPaddedString15();
        //                        string argoname7 = "等身大基準";
        //                        if (!Expression.IsOptionDefined(argoname7))
        //                        {
        //                            if (withBlock8.CountPilot() > 0)
        //                            {
        //                                list[j] = list[j] + "  " + withBlock8.MainPilot().get_Nickname(false);
        //                            }
        //                        }
        //                    }

        //                NextForm:
        //                    ;
        //                }
        //            }
        //        }

        //        // 合体ユニットの場合は分離形態のユニットのランクも上げる
        //        string argfname5 = "分離";
        //        if (withBlock7.IsFeatureAvailable(argfname5))
        //        {
        //            urank = withBlock7.Rank;
        //            object argIndex10 = "分離";
        //            buf = withBlock7.FeatureData(argIndex10);
        //            var loopTo20 = GeneralLib.LLength(buf);
        //            for (i = 2; i <= loopTo20; i++)
        //            {
        //                object argIndex12 = GeneralLib.LIndex(buf, i);
        //                if (SRC.UList.IsDefined(argIndex12))
        //                {
        //                    object argIndex11 = GeneralLib.LIndex(buf, i);
        //                    {
        //                        var withBlock9 = SRC.UList.Item(argIndex11);
        //                        withBlock9.Rank = GeneralLib.MaxLng(urank, withBlock9.Rank);
        //                        withBlock9.HP = withBlock9.MaxHP;
        //                        withBlock9.EN = withBlock9.MaxEN;
        //                        var loopTo21 = withBlock9.CountOtherForm();
        //                        for (j = 1; j <= loopTo21; j++)
        //                        {
        //                            Unit localOtherForm10() { object argIndex1 = j; var ret = withBlock9.OtherForm(argIndex1); return ret; }

        //                            localOtherForm10().Rank = withBlock9.Rank;
        //                            Unit localOtherForm11() { object argIndex1 = j; var ret = withBlock9.OtherForm(argIndex1); return ret; }

        //                            Unit localOtherForm12() { object argIndex1 = j; var ret = withBlock9.OtherForm(argIndex1); return ret; }

        //                            localOtherForm11().HP = localOtherForm12().MaxHP;
        //                            Unit localOtherForm13() { object argIndex1 = j; var ret = withBlock9.OtherForm(argIndex1); return ret; }

        //                            Unit localOtherForm14() { object argIndex1 = j; var ret = withBlock9.OtherForm(argIndex1); return ret; }

        //                            localOtherForm13().EN = localOtherForm14().MaxEN;
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        // ユニットリストの表示内容を更新

        //        if (use_max_rank)
        //        {
        //            string localRightPaddedString6() { string argbuf = withBlock7.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock7.Nickname0 = argbuf; return ret; }

        //            string localLeftPaddedString16() { string argbuf = SrcFormatter.Format(withBlock7.Rank); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

        //            list[ret] = localRightPaddedString6() + localLeftPaddedString16() + "/";
        //            if (MaxRank(u) > 0)
        //            {
        //                string localLeftPaddedString17() { string argbuf = SrcFormatter.Format(MaxRank(u)); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

        //                list[ret] = list[ret] + localLeftPaddedString17();
        //            }
        //            else
        //            {
        //                list[ret] = list[ret] + "--";
        //            }
        //        }
        //        else if (withBlock7.Rank < 10)
        //        {
        //            string localRightPaddedString7() { string argbuf = withBlock7.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock7.Nickname0 = argbuf; return ret; }

        //            list[ret] = localRightPaddedString7() + Strings.StrConv(SrcFormatter.Format(withBlock7.Rank), VbStrConv.Wide);
        //        }
        //        else
        //        {
        //            string localRightPaddedString8() { string argbuf = withBlock7.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock7.Nickname0 = argbuf; return ret; }

        //            list[ret] = localRightPaddedString8() + SrcFormatter.Format(withBlock7.Rank);
        //        }

        //        if (RankUpCost(u) < 10000000)
        //        {
        //            string localLeftPaddedString18() { string argbuf = SrcFormatter.Format(RankUpCost(u)); var ret = GeneralLib.LeftPaddedString(argbuf, 7); return ret; }

        //            list[ret] = list[ret] + localLeftPaddedString18();
        //        }
        //        else
        //        {
        //            string argbuf2 = "----";
        //            list[ret] = list[ret] + GeneralLib.LeftPaddedString(argbuf2, 7);
        //        }

        //        string argoname8 = "等身大基準";
        //        if (Expression.IsOptionDefined(argoname8))
        //        {
        //            if (withBlock7.CountPilot() > 0)
        //            {
        //                string localLeftPaddedString19() { string argbuf = SrcFormatter.Format(withBlock7.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

        //                list[ret] = list[ret] + localLeftPaddedString19();
        //            }
        //        }

        //        string localLeftPaddedString20() { string argbuf = SrcFormatter.Format(withBlock7.MaxHP); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

        //        string localLeftPaddedString21() { string argbuf = SrcFormatter.Format(withBlock7.MaxEN); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

        //        string localLeftPaddedString22() { string argbuf = SrcFormatter.Format(withBlock7.get_Armor("")); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

        //        string localLeftPaddedString23() { string argbuf = SrcFormatter.Format(withBlock7.get_Mobility("")); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

        //        list[ret] = list[ret] + localLeftPaddedString20() + localLeftPaddedString21() + localLeftPaddedString22() + localLeftPaddedString23();
        //        string argoname9 = "等身大基準";
        //        if (!Expression.IsOptionDefined(argoname9))
        //        {
        //            if (withBlock7.CountPilot() > 0)
        //            {
        //                list[ret] = list[ret] + "  " + withBlock7.MainPilot().get_Nickname(false);
        //            }
        //        }
        //    }

        //    // 改めて資金と改造費を調べ、各ユニットが改造可能かチェックする
        //    var loopTo22 = Information.UBound(list);
        //    for (i = 2; i <= loopTo22; i++)
        //    {
        //        Unit localItem9() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = SRC.UList.Item(argIndex1); return ret; }

        //        var argu34 = localItem9();
        //        cost = RankUpCost(argu34);
        //        if (cost > SRC.Money | cost > 10000000)
        //        {
        //            GUI.ListItemFlag[i] = true;
        //        }
        //        else
        //        {
        //            GUI.ListItemFlag[i] = false;
        //        }
        //    }

        //    goto Beginning;
        //}

        //// ユニットランクを上げるためのコストを算出
        //public int RankUpCost(Unit u)
        //{
        //    int RankUpCostRet = default;
        //    {
        //        var withBlock = u;
        //        // これ以上改造できない？
        //        if (withBlock.Rank >= MaxRank(u))
        //        {
        //            RankUpCostRet = 999999999;
        //            return RankUpCostRet;
        //        }

        //        // 合体状態にある場合はそれが主形態でない限り改造不可
        //        string argfname2 = "分離";
        //        if (withBlock.IsFeatureAvailable(argfname2))
        //        {
        //            int localLLength() { object argIndex1 = "分離"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LLength(arglist); return ret; }

        //            string argfname = "主形態";
        //            string argfname1 = "制限時間";
        //            if (localLLength() == 3 & !withBlock.IsFeatureAvailable(argfname) | withBlock.IsFeatureAvailable(argfname1))
        //            {
        //                RankUpCostRet = 999999999;
        //                return RankUpCostRet;
        //            }
        //        }

        //        string argoname = "低改造費";
        //        string argoname1 = "１５段階改造";
        //        if (Expression.IsOptionDefined(argoname))
        //        {
        //            // 低改造費の場合
        //            switch (withBlock.Rank)
        //            {
        //                case 0:
        //                    {
        //                        RankUpCostRet = 10000;
        //                        break;
        //                    }

        //                case 1:
        //                    {
        //                        RankUpCostRet = 15000;
        //                        break;
        //                    }

        //                case 2:
        //                    {
        //                        RankUpCostRet = 20000;
        //                        break;
        //                    }

        //                case 3:
        //                    {
        //                        RankUpCostRet = 30000;
        //                        break;
        //                    }

        //                case 4:
        //                    {
        //                        RankUpCostRet = 40000;
        //                        break;
        //                    }

        //                case 5:
        //                    {
        //                        RankUpCostRet = 50000;
        //                        break;
        //                    }

        //                case 6:
        //                    {
        //                        RankUpCostRet = 60000;
        //                        break;
        //                    }

        //                case 7:
        //                    {
        //                        RankUpCostRet = 70000;
        //                        break;
        //                    }

        //                case 8:
        //                    {
        //                        RankUpCostRet = 80000;
        //                        break;
        //                    }

        //                case 9:
        //                    {
        //                        RankUpCostRet = 100000;
        //                        break;
        //                    }

        //                case 10:
        //                    {
        //                        RankUpCostRet = 120000;
        //                        break;
        //                    }

        //                case 11:
        //                    {
        //                        RankUpCostRet = 140000;
        //                        break;
        //                    }

        //                case 12:
        //                    {
        //                        RankUpCostRet = 160000;
        //                        break;
        //                    }

        //                case 13:
        //                    {
        //                        RankUpCostRet = 180000;
        //                        break;
        //                    }

        //                case 14:
        //                    {
        //                        RankUpCostRet = 200000;
        //                        break;
        //                    }

        //                default:
        //                    {
        //                        RankUpCostRet = 999999999;
        //                        return RankUpCostRet;
        //                    }
        //            }
        //        }
        //        else if (Expression.IsOptionDefined(argoname1))
        //        {
        //            // 通常の１５段改造
        //            // (１０段改造時よりお求め安い価格になっております……)
        //            switch (withBlock.Rank)
        //            {
        //                case 0:
        //                    {
        //                        RankUpCostRet = 10000;
        //                        break;
        //                    }

        //                case 1:
        //                    {
        //                        RankUpCostRet = 15000;
        //                        break;
        //                    }

        //                case 2:
        //                    {
        //                        RankUpCostRet = 20000;
        //                        break;
        //                    }

        //                case 3:
        //                    {
        //                        RankUpCostRet = 40000;
        //                        break;
        //                    }

        //                case 4:
        //                    {
        //                        RankUpCostRet = 80000;
        //                        break;
        //                    }

        //                case 5:
        //                    {
        //                        RankUpCostRet = 120000;
        //                        break;
        //                    }

        //                case 6:
        //                    {
        //                        RankUpCostRet = 160000;
        //                        break;
        //                    }

        //                case 7:
        //                    {
        //                        RankUpCostRet = 200000;
        //                        break;
        //                    }

        //                case 8:
        //                    {
        //                        RankUpCostRet = 250000;
        //                        break;
        //                    }

        //                case 9:
        //                    {
        //                        RankUpCostRet = 300000;
        //                        break;
        //                    }

        //                case 10:
        //                    {
        //                        RankUpCostRet = 350000;
        //                        break;
        //                    }

        //                case 11:
        //                    {
        //                        RankUpCostRet = 400000;
        //                        break;
        //                    }

        //                case 12:
        //                    {
        //                        RankUpCostRet = 450000;
        //                        break;
        //                    }

        //                case 13:
        //                    {
        //                        RankUpCostRet = 500000;
        //                        break;
        //                    }

        //                case 14:
        //                    {
        //                        RankUpCostRet = 550000;
        //                        break;
        //                    }

        //                default:
        //                    {
        //                        RankUpCostRet = 999999999;
        //                        return RankUpCostRet;
        //                    }
        //            }
        //        }
        //        else
        //        {
        //            // 通常の１０段改造
        //            switch (withBlock.Rank)
        //            {
        //                case 0:
        //                    {
        //                        RankUpCostRet = 10000;
        //                        break;
        //                    }

        //                case 1:
        //                    {
        //                        RankUpCostRet = 15000;
        //                        break;
        //                    }

        //                case 2:
        //                    {
        //                        RankUpCostRet = 20000;
        //                        break;
        //                    }

        //                case 3:
        //                    {
        //                        RankUpCostRet = 40000;
        //                        break;
        //                    }

        //                case 4:
        //                    {
        //                        RankUpCostRet = 80000;
        //                        break;
        //                    }

        //                case 5:
        //                    {
        //                        RankUpCostRet = 150000;
        //                        break;
        //                    }

        //                case 6:
        //                    {
        //                        RankUpCostRet = 200000;
        //                        break;
        //                    }

        //                case 7:
        //                    {
        //                        RankUpCostRet = 300000;
        //                        break;
        //                    }

        //                case 8:
        //                    {
        //                        RankUpCostRet = 400000;
        //                        break;
        //                    }

        //                case 9:
        //                    {
        //                        RankUpCostRet = 500000;
        //                        break;
        //                    }

        //                default:
        //                    {
        //                        RankUpCostRet = 999999999;
        //                        return RankUpCostRet;
        //                    }
        //            }
        //        }

        //        // ユニット用特殊能力「改造費修正」による修正
        //        string argfname3 = "改造費修正";
        //        if (withBlock.IsFeatureAvailable(argfname3))
        //        {
        //            object argIndex1 = "改造費修正";
        //            RankUpCostRet = (RankUpCostRet * (1d + withBlock.FeatureLevel(argIndex1) / 10d));
        //        }
        //    }

        //    return RankUpCostRet;
        //}

        //// ユニットの最大改造数を算出
        //public int MaxRank(Unit u)
        //{
        //    int MaxRankRet = default;
        //    string argoname = "５段階改造";
        //    string argoname1 = "１５段階改造";
        //    if (Expression.IsOptionDefined(argoname))
        //    {
        //        // ５段階改造までしか出来ない
        //        MaxRankRet = 5;
        //    }
        //    else if (Expression.IsOptionDefined(argoname1))
        //    {
        //        // １５段階改造まで可能
        //        MaxRankRet = 15;
        //    }
        //    else
        //    {
        //        // デフォルトは１０段階まで
        //        MaxRankRet = 10;
        //    }
        //    // Disableコマンドで改造不可にされている？
        //    string argvname = "Disable(" + u.Name + ",改造)";
        //    if (Expression.IsGlobalVariableDefined(argvname))
        //    {
        //        MaxRankRet = 0;
        //        return MaxRankRet;
        //    }

        //    // 最大改造数が設定されている？
        //    string argfname = "最大改造数";
        //    if (u.IsFeatureAvailable(argfname))
        //    {
        //        object argIndex1 = "最大改造数";
        //        MaxRankRet = GeneralLib.MinLng(MaxRankRet, u.FeatureLevel(argIndex1));
        //    }

        //    return MaxRankRet;
        //}

        //// 乗り換えコマンド
        //public void ExchangeUnitCommand()
        //{
        //    int j, i, k;
        //    string[] list;
        //    string[] id_list;
        //    string sort_mode, sort_mode2;
        //    string[] sort_mode_type;
        //    string[] sort_mode_list;
        //    bool[] item_flag_backup;
        //    string[] item_comment_backup;
        //    int[] key_list;
        //    string[] strkey_list;
        //    int max_item;
        //    int max_value;
        //    string max_str;
        //    Unit u;
        //    Pilot p;
        //    string pname;
        //    string buf;
        //    int ret;
        //    bool b;
        //    bool is_support;
        //    string caption_str;
        //    int top_item;
        //    top_item = 1;

        //    // デフォルトのソート方法
        //    sort_mode = "レベル";
        //    sort_mode2 = "名称";
        //Beginning:
        //    ;


        //    // 乗り換えるパイロットの一覧を作成
        //    list = new string[2];
        //    id_list = new string[2];
        //    GUI.ListItemComment = new string[2];
        //    list[1] = "▽並べ替え▽";
        //    foreach (Pilot currentP in SRC.PList)
        //    {
        //        p = currentP;
        //        {
        //            var withBlock = p;
        //            bool localIsGlobalVariableDefined() { string argvname = "Fix(" + withBlock.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

        //            if (withBlock.Party != "味方" | withBlock.Away | localIsGlobalVariableDefined())
        //            {
        //                goto NextLoop;
        //            }

        //            // 追加パイロット＆サポートは乗り換え不可
        //            if (withBlock.IsAdditionalPilot | withBlock.IsAdditionalSupport)
        //            {
        //                goto NextLoop;
        //            }

        //            is_support = false;
        //            if (withBlock.Unit_Renamed is object)
        //            {
        //                // サポートが複数乗っている場合は乗り降り不可
        //                if (withBlock.Unit_Renamed.CountSupport() > 1)
        //                {
        //                    goto NextLoop;
        //                }

        //                // サポートパイロットとして乗り込んでいるかを判定
        //                if (withBlock.Unit_Renamed.CountSupport() == 1)
        //                {
        //                    object argIndex1 = 1;
        //                    if ((withBlock.ID ?? "") == (withBlock.Unit_Renamed.Support(argIndex1).ID ?? ""))
        //                    {
        //                        is_support = true;
        //                    }
        //                }

        //                // 通常のパイロットの場合
        //                if (!is_support)
        //                {
        //                    // ３人乗り以上は乗り降り不可
        //                    if (withBlock.Unit_Renamed.Data.PilotNum != 1 & Math.Abs(withBlock.Unit_Renamed.Data.PilotNum) != 2)
        //                    {
        //                        goto NextLoop;
        //                    }
        //                }
        //            }

        //            if (is_support)
        //            {
        //                // サポートパイロットの場合
        //                Array.Resize(list, Information.UBound(list) + 1 + 1);
        //                Array.Resize(id_list, Information.UBound(list) + 1);
        //                Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

        //                // パイロットのステータス
        //                string localLeftPaddedString() { string argbuf = Strings.StrConv(SrcFormatter.Format(withBlock.Level), VbStrConv.Wide); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

        //                string argbuf = "*" + withBlock.get_Nickname(false);
        //                list[Information.UBound(list)] = GeneralLib.RightPaddedString(argbuf, 25) + localLeftPaddedString();
        //                if (withBlock.Unit_Renamed is object)
        //                {
        //                    {
        //                        var withBlock1 = withBlock.Unit_Renamed;
        //                        // ユニットのステータス
        //                        string localRightPaddedString() { string argbuf = withBlock1.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 29); withBlock1.Nickname0 = argbuf; return ret; }

        //                        list[Information.UBound(list)] = list[Information.UBound(list)] + "  " + localRightPaddedString() + "(" + withBlock1.MainPilot().get_Nickname(false) + ")";

        //                        // ユニットが装備しているアイテム一覧
        //                        var loopTo = withBlock1.CountItem();
        //                        for (k = 1; k <= loopTo; k++)
        //                        {
        //                            object argIndex2 = k;
        //                            {
        //                                var withBlock2 = withBlock1.Item(argIndex2);
        //                                string argfname = "非表示";
        //                                if ((withBlock2.Class_Renamed() != "固定" | !withBlock2.IsFeatureAvailable(argfname)) & withBlock2.Part() != "非表示")
        //                                {
        //                                    GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + withBlock2.Nickname() + " ";
        //                                }
        //                            }
        //                        }
        //                    }
        //                }

        //                // パイロットＩＤを記録しておく
        //                id_list[Information.UBound(list)] = withBlock.ID;
        //            }
        //            else if (withBlock.Unit_Renamed is null)
        //            {
        //                // ユニットに乗っていないパイロットの場合
        //                Array.Resize(list, Information.UBound(list) + 1 + 1);
        //                Array.Resize(id_list, Information.UBound(list) + 1);
        //                Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

        //                // パイロットのステータス
        //                string localLeftPaddedString1() { string argbuf = Strings.StrConv(SrcFormatter.Format(withBlock.Level), VbStrConv.Wide); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

        //                string argbuf1 = " " + withBlock.get_Nickname(false);
        //                list[Information.UBound(list)] = GeneralLib.RightPaddedString(argbuf1, 25) + localLeftPaddedString1();

        //                // パイロットＩＤを記録しておく
        //                id_list[Information.UBound(list)] = withBlock.ID;
        //            }
        //            else if (withBlock.Unit_Renamed.CountPilot() <= 2)
        //            {
        //                // 複数乗りのユニットに乗っているパイロットの場合
        //                Array.Resize(list, Information.UBound(list) + 1 + 1);
        //                Array.Resize(id_list, Information.UBound(list) + 1);
        //                Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

        //                // パイロットが足りない？
        //                if (withBlock.Unit_Renamed.CountPilot() < Math.Abs(withBlock.Unit_Renamed.Data.PilotNum))
        //                {
        //                    list[Information.UBound(list)] = "-";
        //                }
        //                else
        //                {
        //                    list[Information.UBound(list)] = " ";
        //                }

        //                string argfname1 = "追加パイロット";
        //                if (withBlock.Unit_Renamed.IsFeatureAvailable(argfname1))
        //                {
        //                    pname = withBlock.Unit_Renamed.MainPilot().get_Nickname(false);
        //                }
        //                else
        //                {
        //                    pname = withBlock.get_Nickname(false);
        //                }

        //                // 複数乗りの場合は何番目のパイロットか表示
        //                if (Math.Abs(withBlock.Unit_Renamed.Data.PilotNum) > 1)
        //                {
        //                    var loopTo1 = withBlock.Unit_Renamed.CountPilot();
        //                    for (k = 1; k <= loopTo1; k++)
        //                    {
        //                        object argIndex3 = k;
        //                        if (ReferenceEquals(withBlock.Unit_Renamed.Pilot(argIndex3), p))
        //                        {
        //                            pname = pname + "(" + SrcFormatter.Format(k) + ")";
        //                        }
        //                    }
        //                }

        //                // パイロット＆ユニットのステータス
        //                string localLeftPaddedString2() { string argbuf = Strings.StrConv(SrcFormatter.Format(withBlock.Level), VbStrConv.Wide); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

        //                string localRightPaddedString1() { string argbuf = withBlock.Unit_Renamed.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 29); withBlock.Unit_Renamed.Nickname0 = argbuf; return ret; }

        //                list[Information.UBound(list)] = list[Information.UBound(list)] + GeneralLib.RightPaddedString(pname, 24) + localLeftPaddedString2() + "  " + localRightPaddedString1();
        //                if (withBlock.Unit_Renamed.CountSupport() > 0)
        //                {
        //                    object argIndex4 = 1;
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + "(" + withBlock.Unit_Renamed.Support(argIndex4).get_Nickname(false) + ")";
        //                }

        //                // ユニットが装備しているアイテム一覧
        //                {
        //                    var withBlock3 = withBlock.Unit_Renamed;
        //                    var loopTo2 = withBlock3.CountItem();
        //                    for (k = 1; k <= loopTo2; k++)
        //                    {
        //                        object argIndex5 = k;
        //                        {
        //                            var withBlock4 = withBlock3.Item(argIndex5);
        //                            string argfname2 = "非表示";
        //                            if ((withBlock4.Class_Renamed() != "固定" | !withBlock4.IsFeatureAvailable(argfname2)) & withBlock4.Part() != "非表示")
        //                            {
        //                                GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + withBlock4.Nickname() + " ";
        //                            }
        //                        }
        //                    }
        //                }

        //                // パイロットＩＤを記録しておく
        //                id_list[Information.UBound(list)] = withBlock.ID;
        //            }
        //        }

        //    NextLoop:
        //        ;
        //    }

        //    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
        //SortAgain:
        //    ;


        //    // ソート
        //    if (sort_mode == "レベル")
        //    {
        //        // レベルによるソート

        //        // まずレベルのリストを作成
        //        key_list = new int[Information.UBound(list) + 1];
        //        {
        //            var withBlock5 = SRC.PList;
        //            var loopTo3 = Information.UBound(list);
        //            for (i = 2; i <= loopTo3; i++)
        //            {
        //                var tmp = id_list;
        //                object argIndex6 = tmp[i];
        //                {
        //                    var withBlock6 = withBlock5.Item(argIndex6);
        //                    key_list[i] = 500 * withBlock6.Level + withBlock6.Exp;
        //                }
        //            }
        //        }

        //        // レベルを使って並べ換え
        //        var loopTo4 = (Information.UBound(list) - 1);
        //        for (i = 2; i <= loopTo4; i++)
        //        {
        //            max_item = i;
        //            max_value = key_list[i];
        //            var loopTo5 = Information.UBound(list);
        //            for (j = (i + 1); j <= loopTo5; j++)
        //            {
        //                if (key_list[j] > max_value)
        //                {
        //                    max_item = j;
        //                    max_value = key_list[j];
        //                }
        //            }

        //            if (max_item != i)
        //            {
        //                buf = list[i];
        //                list[i] = list[max_item];
        //                list[max_item] = buf;
        //                buf = id_list[i];
        //                id_list[i] = id_list[max_item];
        //                id_list[max_item] = buf;
        //                buf = GUI.ListItemComment[i];
        //                GUI.ListItemComment[i] = GUI.ListItemComment[max_item];
        //                GUI.ListItemComment[max_item] = buf;
        //                key_list[max_item] = key_list[i];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // 読み仮名によるソート

        //        // まず読み仮名のリストを作成
        //        strkey_list = new string[Information.UBound(list) + 1];
        //        {
        //            var withBlock7 = SRC.PList;
        //            var loopTo6 = Information.UBound(list);
        //            for (i = 2; i <= loopTo6; i++)
        //            {
        //                Pilot localItem() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock7.Item(argIndex1); return ret; }

        //                strkey_list[i] = localItem().KanaName;
        //            }
        //        }

        //        // 読み仮名を使って並べ替え
        //        var loopTo7 = (Information.UBound(strkey_list) - 1);
        //        for (i = 2; i <= loopTo7; i++)
        //        {
        //            max_item = i;
        //            max_str = strkey_list[i];
        //            var loopTo8 = Information.UBound(strkey_list);
        //            for (j = (i + 1); j <= loopTo8; j++)
        //            {
        //                if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
        //                {
        //                    max_item = j;
        //                    max_str = strkey_list[j];
        //                }
        //            }

        //            if (max_item != i)
        //            {
        //                buf = list[i];
        //                list[i] = list[max_item];
        //                list[max_item] = buf;
        //                buf = id_list[i];
        //                id_list[i] = id_list[max_item];
        //                id_list[max_item] = buf;
        //                buf = GUI.ListItemComment[i];
        //                GUI.ListItemComment[i] = GUI.ListItemComment[max_item];
        //                GUI.ListItemComment[max_item] = buf;
        //                strkey_list[max_item] = strkey_list[i];
        //            }
        //        }
        //    }

        //    // パイロットを選択
        //    GUI.TopItem = top_item;
        //    string argoname = "等身大基準";
        //    if (Expression.IsOptionDefined(argoname))
        //    {
        //        caption_str = " キャラクター          レベル  ユニット";
        //        string arglb_caption = "キャラクター選択";
        //        string arglb_mode = "連続表示,コメント";
        //        ret = GUI.ListBox(arglb_caption, list, caption_str, arglb_mode);
        //    }
        //    else
        //    {
        //        caption_str = " パイロット            レベル  ユニット";
        //        string arglb_caption1 = "パイロット選択";
        //        string arglb_mode1 = "連続表示,コメント";
        //        ret = GUI.ListBox(arglb_caption1, list, caption_str, arglb_mode1);
        //    }

        //    top_item = GUI.TopItem;
        //    switch (ret)
        //    {
        //        case 0:
        //            {
        //                // キャンセル
        //                return;
        //            }

        //        case 1:
        //            {
        //                // ソート方法を選択
        //                sort_mode_list = new string[3];
        //                sort_mode_list[1] = "レベル";
        //                sort_mode_list[2] = "名称";
        //                item_flag_backup = new bool[Information.UBound(list) + 1];
        //                item_comment_backup = new string[Information.UBound(list) + 1];
        //                var loopTo9 = Information.UBound(list);
        //                for (i = 2; i <= loopTo9; i++)
        //                {
        //                    item_flag_backup[i] = GUI.ListItemFlag[i];
        //                    item_comment_backup[i] = GUI.ListItemComment[i];
        //                }

        //                GUI.ListItemComment = new string[Information.UBound(sort_mode_list) + 1];
        //                GUI.ListItemFlag = new bool[Information.UBound(sort_mode_list) + 1];
        //                string arglb_caption2 = "どれで並べ替えますか？";
        //                string arglb_info = "並べ替えの方法";
        //                string arglb_mode2 = "連続表示,コメント";
        //                ret = GUI.ListBox(arglb_caption2, sort_mode_list, arglb_info, arglb_mode2);
        //                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
        //                GUI.ListItemComment = new string[Information.UBound(list) + 1];
        //                var loopTo10 = Information.UBound(list);
        //                for (i = 2; i <= loopTo10; i++)
        //                {
        //                    GUI.ListItemFlag[i] = item_flag_backup[i];
        //                    GUI.ListItemComment[i] = item_comment_backup[i];
        //                }

        //                // ソート方法を変更して再表示
        //                if (ret > 0)
        //                {
        //                    sort_mode = sort_mode_list[ret];
        //                }

        //                goto SortAgain;
        //                break;
        //            }
        //    }

        //    // 乗り換えさせるパイロット
        //    var tmp1 = id_list;
        //    object argIndex7 = tmp1[ret];
        //    p = SRC.PList.Item(argIndex7);

        //    // 乗り換え先ユニット一覧作成
        //    list = new string[2];
        //    id_list = new string[2];
        //    GUI.ListItemComment = new string[2];
        //    list[1] = "▽並べ替え▽";
        //    foreach (Unit currentU in SRC.UList)
        //    {
        //        u = currentU;
        //        {
        //            var withBlock8 = u;
        //            if (withBlock8.Party0 != "味方" | withBlock8.Status_Renamed != "待機")
        //            {
        //                goto NextUnit;
        //            }

        //            if (withBlock8.CountSupport() > 1)
        //            {
        //                if (Strings.InStr(p.Class_Renamed, "専属サポート") == 0)
        //                {
        //                    goto NextUnit;
        //                }
        //            }

        //            if (ReferenceEquals(u, p.Unit_Renamed))
        //            {
        //                goto NextUnit;
        //            }

        //            if (!p.IsAbleToRide(u))
        //            {
        //                goto NextUnit;
        //            }

        //            // サポートキャラでなければ乗り換えられるパイロット数に制限がある
        //            if (!p.IsSupport(u))
        //            {
        //                if (withBlock8.Data.PilotNum != 1 & Math.Abs(withBlock8.Data.PilotNum) != 2)
        //                {
        //                    goto NextUnit;
        //                }
        //            }

        //            if (withBlock8.CountPilot() > 0)
        //            {
        //                object argIndex8 = 1;
        //                string argvname = "Fix(" + withBlock8.Pilot(argIndex8).Name + ")";
        //                if (Expression.IsGlobalVariableDefined(argvname) & !p.IsSupport(u))
        //                {
        //                    // Fixコマンドでパイロットが固定されたユニットはサポートでない
        //                    // 限り乗り換え不可
        //                    goto NextUnit;
        //                }

        //                Array.Resize(list, Information.UBound(list) + 1 + 1);
        //                Array.Resize(id_list, Information.UBound(list) + 1);
        //                Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

        //                // パイロットが足りている？
        //                if (withBlock8.CountPilot() < Math.Abs(withBlock8.Data.PilotNum))
        //                {
        //                    list[Information.UBound(list)] = "-";
        //                }
        //                else
        //                {
        //                    list[Information.UBound(list)] = " ";
        //                }

        //                string localRightPaddedString2() { string argbuf = withBlock8.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 35); withBlock8.Nickname0 = argbuf; return ret; }

        //                string localRightPaddedString3() { string argbuf = withBlock8.MainPilot().get_Nickname(false); var ret = GeneralLib.RightPaddedString(argbuf, 21); withBlock8.MainPilot().get_Nickname(false) = argbuf; return ret; }

        //                list[Information.UBound(list)] = list[Information.UBound(list)] + localRightPaddedString2() + localRightPaddedString3();
        //                if (withBlock8.Rank < 10)
        //                {
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + " " + Strings.StrConv(SrcFormatter.Format(withBlock8.Rank), VbStrConv.Wide);
        //                }
        //                else
        //                {
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + " " + SrcFormatter.Format(withBlock8.Rank);
        //                }

        //                if (withBlock8.CountSupport() > 0)
        //                {
        //                    object argIndex9 = 1;
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + " (" + withBlock8.Support(argIndex9).get_Nickname(false) + ")";
        //                }

        //                // ユニットに装備されているアイテムをコメント欄に列記
        //                var loopTo11 = withBlock8.CountItem();
        //                for (j = 1; j <= loopTo11; j++)
        //                {
        //                    object argIndex10 = j;
        //                    {
        //                        var withBlock9 = withBlock8.Item(argIndex10);
        //                        string argfname3 = "非表示";
        //                        if ((withBlock9.Class_Renamed() != "固定" | !withBlock9.IsFeatureAvailable(argfname3)) & withBlock9.Part() != "非表示")
        //                        {
        //                            GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + withBlock9.Nickname() + " ";
        //                        }
        //                    }
        //                }

        //                // ユニットＩＤを記録しておく
        //                id_list[Information.UBound(list)] = withBlock8.ID;
        //            }
        //            else if (!p.IsSupport(u))
        //            {
        //                // 誰も乗ってないユニットに乗れるのは通常パイロットのみ

        //                Array.Resize(list, Information.UBound(list) + 1 + 1);
        //                Array.Resize(id_list, Information.UBound(list) + 1);
        //                Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);
        //                string localRightPaddedString4() { string argbuf = withBlock8.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 35); withBlock8.Nickname0 = argbuf; return ret; }

        //                list[Information.UBound(list)] = " " + localRightPaddedString4() + Strings.Space(21);
        //                if (withBlock8.Rank < 10)
        //                {
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + " " + Strings.StrConv(SrcFormatter.Format(withBlock8.Rank), VbStrConv.Wide);
        //                }
        //                else
        //                {
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + " " + SrcFormatter.Format(withBlock8.Rank);
        //                }

        //                // ユニットに装備されているアイテムをコメント欄に列記
        //                var loopTo12 = withBlock8.CountItem();
        //                for (j = 1; j <= loopTo12; j++)
        //                {
        //                    object argIndex11 = j;
        //                    {
        //                        var withBlock10 = withBlock8.Item(argIndex11);
        //                        string argfname4 = "非表示";
        //                        if ((withBlock10.Class_Renamed() != "固定" | !withBlock10.IsFeatureAvailable(argfname4)) & withBlock10.Part() != "非表示")
        //                        {
        //                            GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + withBlock10.Nickname() + " ";
        //                        }
        //                    }
        //                }

        //                // ユニットＩＤを記録しておく
        //                id_list[Information.UBound(list)] = withBlock8.ID;
        //            }
        //        }

        //    NextUnit:
        //        ;
        //    }

        //    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
        //SortAgain2:
        //    ;


        //    // ソート
        //    if (Strings.InStr(sort_mode2, "名称") == 0)
        //    {
        //        // 数値によるソート

        //        // まずキーのリストを作成
        //        key_list = new int[Information.UBound(list) + 1];
        //        {
        //            var withBlock11 = SRC.UList;
        //            switch (sort_mode2 ?? "")
        //            {
        //                case "ＨＰ":
        //                    {
        //                        var loopTo13 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo13; i++)
        //                        {
        //                            Unit localItem1() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock11.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem1().MaxHP;
        //                        }

        //                        break;
        //                    }

        //                case "ＥＮ":
        //                    {
        //                        var loopTo14 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo14; i++)
        //                        {
        //                            Unit localItem2() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock11.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem2().MaxEN;
        //                        }

        //                        break;
        //                    }

        //                case "装甲":
        //                    {
        //                        var loopTo15 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo15; i++)
        //                        {
        //                            Unit localItem3() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock11.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem3().get_Armor("");
        //                        }

        //                        break;
        //                    }

        //                case "運動性":
        //                    {
        //                        var loopTo16 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo16; i++)
        //                        {
        //                            Unit localItem4() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock11.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem4().get_Mobility("");
        //                        }

        //                        break;
        //                    }

        //                case "ユニットランク":
        //                    {
        //                        var loopTo17 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo17; i++)
        //                        {
        //                            Unit localItem5() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock11.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem5().Rank;
        //                        }

        //                        break;
        //                    }
        //            }
        //        }

        //        // キーを使って並べ替え
        //        var loopTo18 = (Information.UBound(list) - 1);
        //        for (i = 2; i <= loopTo18; i++)
        //        {
        //            max_item = i;
        //            max_value = key_list[i];
        //            var loopTo19 = Information.UBound(list);
        //            for (j = (i + 1); j <= loopTo19; j++)
        //            {
        //                if (key_list[j] > max_value)
        //                {
        //                    max_item = j;
        //                    max_value = key_list[j];
        //                }
        //            }

        //            if (max_item != i)
        //            {
        //                buf = list[i];
        //                list[i] = list[max_item];
        //                list[max_item] = buf;
        //                buf = id_list[i];
        //                id_list[i] = id_list[max_item];
        //                id_list[max_item] = buf;
        //                b = GUI.ListItemFlag[i];
        //                GUI.ListItemFlag[i] = GUI.ListItemFlag[max_item];
        //                GUI.ListItemFlag[max_item] = b;
        //                buf = GUI.ListItemComment[i];
        //                GUI.ListItemComment[i] = GUI.ListItemComment[max_item];
        //                GUI.ListItemComment[max_item] = buf;
        //                key_list[max_item] = key_list[i];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // 読み仮名によるソート

        //        // まず読み仮名のリストを作成
        //        strkey_list = new string[Information.UBound(list) + 1];
        //        {
        //            var withBlock12 = SRC.UList;
        //            var loopTo20 = Information.UBound(list);
        //            for (i = 2; i <= loopTo20; i++)
        //            {
        //                Unit localItem6() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock12.Item(argIndex1); return ret; }

        //                strkey_list[i] = localItem6().KanaName;
        //            }
        //        }

        //        // 読み仮名を使って並べ替え
        //        var loopTo21 = (Information.UBound(strkey_list) - 1);
        //        for (i = 2; i <= loopTo21; i++)
        //        {
        //            max_item = i;
        //            max_str = strkey_list[i];
        //            var loopTo22 = Information.UBound(strkey_list);
        //            for (j = (i + 1); j <= loopTo22; j++)
        //            {
        //                if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
        //                {
        //                    max_item = j;
        //                    max_str = strkey_list[j];
        //                }
        //            }

        //            if (max_item != i)
        //            {
        //                buf = list[i];
        //                list[i] = list[max_item];
        //                list[max_item] = buf;
        //                buf = id_list[i];
        //                id_list[i] = id_list[max_item];
        //                id_list[max_item] = buf;
        //                b = GUI.ListItemFlag[i];
        //                GUI.ListItemFlag[i] = GUI.ListItemFlag[max_item];
        //                GUI.ListItemFlag[max_item] = b;
        //                buf = GUI.ListItemComment[i];
        //                GUI.ListItemComment[i] = GUI.ListItemComment[max_item];
        //                GUI.ListItemComment[max_item] = buf;
        //                strkey_list[max_item] = strkey_list[i];
        //            }
        //        }
        //    }

        //    // 乗り換え先を選択
        //    GUI.TopItem = 1;
        //    u = p.Unit_Renamed;
        //    string argoname1 = "等身大基準";
        //    if (Expression.IsOptionDefined(argoname1))
        //    {
        //        string argtname = "ランク";
        //        Unit argu = null;
        //        caption_str = " ユニット                           キャラクター       " + Expression.Term(argtname, u: argu);
        //    }
        //    else
        //    {
        //        string argtname1 = "ランク";
        //        Unit argu1 = null;
        //        caption_str = " ユニット                           パイロット         " + Expression.Term(argtname1, u: argu1);
        //    }

        //    if (u is object)
        //    {
        //        string argfname5 = "追加パイロット";
        //        if (u.IsFeatureAvailable(argfname5))
        //        {
        //            string arglb_caption3 = "乗り換え先選択 ： " + u.MainPilot().get_Nickname(false) + " (" + u.Nickname + ")";
        //            string arglb_mode3 = "連続表示,コメント";
        //            ret = GUI.ListBox(arglb_caption3, list, caption_str, arglb_mode3);
        //        }
        //        else
        //        {
        //            string arglb_caption4 = "乗り換え先選択 ： " + p.get_Nickname(false) + " (" + u.Nickname + ")";
        //            string arglb_mode4 = "連続表示,コメント";
        //            ret = GUI.ListBox(arglb_caption4, list, caption_str, arglb_mode4);
        //        }
        //    }
        //    else
        //    {
        //        string arglb_caption5 = "乗り換え先選択 ： " + p.get_Nickname(false);
        //        string arglb_mode5 = "連続表示,コメント";
        //        ret = GUI.ListBox(arglb_caption5, list, caption_str, arglb_mode5);
        //    }

        //    switch (ret)
        //    {
        //        case 0:
        //            {
        //                // キャンセル
        //                return;
        //            }

        //        case 1:
        //            {
        //                // ソート方法を選択
        //                sort_mode_type = new string[7];
        //                sort_mode_list = new string[7];
        //                string argoname2 = "等身大基準";
        //                if (Expression.IsOptionDefined(argoname2))
        //                {
        //                    sort_mode_type[1] = "名称";
        //                    sort_mode_list[1] = "名称";
        //                    sort_mode_type[2] = "ＨＰ";
        //                    string argtname2 = "ＨＰ";
        //                    Unit argu2 = null;
        //                    sort_mode_list[2] = Expression.Term(argtname2, u: argu2);
        //                    sort_mode_type[3] = "ＥＮ";
        //                    string argtname3 = "ＥＮ";
        //                    Unit argu3 = null;
        //                    sort_mode_list[3] = Expression.Term(argtname3, u: argu3);
        //                    sort_mode_type[4] = "装甲";
        //                    string argtname4 = "装甲";
        //                    Unit argu4 = null;
        //                    sort_mode_list[4] = Expression.Term(argtname4, u: argu4);
        //                    sort_mode_type[5] = "運動性";
        //                    string argtname5 = "運動性";
        //                    Unit argu5 = null;
        //                    sort_mode_list[5] = Expression.Term(argtname5, u: argu5);
        //                    sort_mode_type[6] = "ユニットランク";
        //                    string argtname6 = "ランク";
        //                    Unit argu6 = null;
        //                    sort_mode_list[6] = Expression.Term(argtname6, u: argu6);
        //                }
        //                else
        //                {
        //                    sort_mode_type[1] = "ＨＰ";
        //                    string argtname7 = "ＨＰ";
        //                    Unit argu7 = null;
        //                    sort_mode_list[1] = Expression.Term(argtname7, u: argu7);
        //                    sort_mode_type[2] = "ＥＮ";
        //                    string argtname8 = "ＥＮ";
        //                    Unit argu8 = null;
        //                    sort_mode_list[2] = Expression.Term(argtname8, u: argu8);
        //                    sort_mode_type[3] = "装甲";
        //                    string argtname9 = "装甲";
        //                    Unit argu9 = null;
        //                    sort_mode_list[3] = Expression.Term(argtname9, u: argu9);
        //                    sort_mode_type[4] = "運動性";
        //                    string argtname10 = "運動性";
        //                    Unit argu10 = null;
        //                    sort_mode_list[4] = Expression.Term(argtname10, u: argu10);
        //                    sort_mode_type[5] = "ユニットランク";
        //                    string argtname11 = "ランク";
        //                    Unit argu11 = null;
        //                    sort_mode_list[5] = Expression.Term(argtname11, u: argu11);
        //                    sort_mode_type[6] = "ユニット名称";
        //                    sort_mode_list[6] = "ユニット名称";
        //                }

        //                item_flag_backup = new bool[Information.UBound(list) + 1];
        //                item_comment_backup = new string[Information.UBound(list) + 1];
        //                var loopTo23 = Information.UBound(list);
        //                for (i = 2; i <= loopTo23; i++)
        //                {
        //                    item_flag_backup[i] = GUI.ListItemFlag[i];
        //                    item_comment_backup[i] = GUI.ListItemComment[i];
        //                }

        //                GUI.ListItemComment = new string[Information.UBound(sort_mode_list) + 1];
        //                GUI.ListItemFlag = new bool[Information.UBound(sort_mode_list) + 1];
        //                GUI.TopItem = 1;
        //                string arglb_caption6 = "どれで並べ替えますか？";
        //                string arglb_info1 = "並べ替えの方法";
        //                string arglb_mode6 = "連続表示,コメント";
        //                ret = GUI.ListBox(arglb_caption6, sort_mode_list, arglb_info1, arglb_mode6);
        //                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
        //                GUI.ListItemComment = new string[Information.UBound(list) + 1];
        //                var loopTo24 = Information.UBound(list);
        //                for (i = 2; i <= loopTo24; i++)
        //                {
        //                    GUI.ListItemFlag[i] = item_flag_backup[i];
        //                    GUI.ListItemComment[i] = item_comment_backup[i];
        //                }

        //                // ソート方法を変更して再表示
        //                if (ret > 0)
        //                {
        //                    sort_mode2 = sort_mode_type[ret];
        //                }

        //                goto SortAgain2;
        //                break;
        //            }
        //    }

        //    // キャンセル？
        //    if (ret == 0)
        //    {
        //        goto Beginning;
        //    }

        //    var tmp2 = id_list;
        //    object argIndex12 = tmp2[ret];
        //    u = SRC.UList.Item(argIndex12);

        //    // 元のユニットから降ろす
        //    p.GetOff();

        //    // 乗り換え
        //    {
        //        var withBlock13 = u;
        //        if (!p.IsSupport(u))
        //        {
        //            // 通常のパイロット
        //            if (withBlock13.CountPilot() == withBlock13.Data.PilotNum)
        //            {
        //                object argIndex13 = 1;
        //                withBlock13.Pilot(argIndex13).GetOff();
        //            }
        //        }
        //        else
        //        {
        //            // サポートパイロット
        //            var loopTo25 = withBlock13.CountSupport();
        //            for (i = 1; i <= loopTo25; i++)
        //            {
        //                object argIndex14 = 1;
        //                withBlock13.Support(argIndex14).GetOff();
        //            }
        //        }
        //    }

        //    Unit localItem7() { var tmp = id_list; object argIndex1 = tmp[ret]; var ret = SRC.UList.Item(argIndex1); return ret; }

        //    var argu12 = localItem7();
        //    p.Ride(argu12);
        //    goto Beginning;
        //}

        //// アイテム交換コマンド
        //public void ExchangeItemCommand([Optional, DefaultParameterValue(null)] Unit selected_unit, [Optional, DefaultParameterValue("")] string selected_part)
        //{
        //    int j, i, k;
        //    int inum, inum2;
        //    string[] list;
        //    string[] id_list;
        //    string iid;
        //    string sort_mode;
        //    var sort_mode_type = new string[8];
        //    var sort_mode_list = new string[8];
        //    bool[] item_flag_backup;
        //    string[] item_comment_backup;
        //    int[] key_list;
        //    string[] strkey_list;
        //    int max_item;
        //    int max_value;
        //    string max_str;
        //    string caption_str;
        //    Unit u;
        //    Item it;
        //    string iname;
        //    string buf;
        //    int ret;
        //    string[] part_list;
        //    string[] part_item;
        //    int arm_point = default, shoulder_point = default;
        //    string ipart;
        //    var empty_slot = default;
        //    var is_right_hand_available = default(bool);
        //    var is_left_hand_available = default(bool);
        //    string[] item_list;
        //    int top_item1, top_item2;
        //    top_item1 = 1;
        //    top_item2 = 1;

        //    // デフォルトのソート方法
        //    string argoname = "等身大基準";
        //    if (Expression.IsOptionDefined(argoname))
        //    {
        //        sort_mode = "レベル";
        //    }
        //    else
        //    {
        //        sort_mode = "ＨＰ";
        //    }

        //    // ユニットがあらかじめ選択されている場合
        //    // (ユニットステータスからのアイテム交換時)
        //    if (selected_unit is object)
        //    {
        //        GUI.EnlargeListBoxHeight();
        //        GUI.ReduceListBoxWidth();
        //        u = selected_unit;
        //        if (GUI.MainForm.Visible)
        //        {
        //            if (!ReferenceEquals(u, Status.DisplayedUnit))
        //            {
        //                Status.DisplayUnitStatus(u);
        //            }
        //        }

        //        goto MakeEquipedItemList;
        //    }

        //Beginning:
        //    ;


        //    // ユニット一覧の作成
        //    list = new string[2];
        //    id_list = new string[2];
        //    GUI.ListItemComment = new string[2];
        //    list[1] = "▽並べ替え▽";
        //    foreach (Unit currentU in SRC.UList)
        //    {
        //        u = currentU;
        //        {
        //            var withBlock = u;
        //            if (withBlock.Party0 != "味方" | withBlock.Status_Renamed != "待機")
        //            {
        //                goto NextUnit;
        //            }

        //            Array.Resize(list, Information.UBound(list) + 1 + 1);
        //            Array.Resize(id_list, Information.UBound(list) + 1);
        //            Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

        //            // 装備しているアイテムの数を数える
        //            inum = 0;
        //            inum2 = 0;
        //            var loopTo = withBlock.CountItem();
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                object argIndex1 = i;
        //                {
        //                    var withBlock1 = withBlock.Item(argIndex1);
        //                    string argfname = "非表示";
        //                    if ((withBlock1.Class_Renamed() != "固定" | !withBlock1.IsFeatureAvailable(argfname)) & withBlock1.Part() != "非表示")
        //                    {
        //                        GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + withBlock1.Nickname() + " ";
        //                        if (withBlock1.Part() == "強化パーツ" | withBlock1.Part() == "アイテム")
        //                        {
        //                            inum = (inum + withBlock1.Size());
        //                        }
        //                        else
        //                        {
        //                            inum2 = (inum2 + withBlock1.Size());
        //                        }
        //                    }
        //                }
        //            }

        //            // リストを作成
        //            string argoname1 = "等身大基準";
        //            if (Expression.IsOptionDefined(argoname1))
        //            {
        //                string argbuf = withBlock.Nickname0;
        //                list[Information.UBound(list)] = GeneralLib.RightPaddedString(argbuf, 39);
        //                withBlock.Nickname0 = argbuf;
        //            }
        //            else
        //            {
        //                string argbuf1 = withBlock.Nickname0;
        //                list[Information.UBound(list)] = GeneralLib.RightPaddedString(argbuf1, 31);
        //                withBlock.Nickname0 = argbuf1;
        //            }

        //            list[Information.UBound(list)] = list[Information.UBound(list)] + SrcFormatter.Format(inum) + "/" + SrcFormatter.Format(withBlock.MaxItemNum());
        //            if (inum2 > 0)
        //            {
        //                list[Information.UBound(list)] = list[Information.UBound(list)] + "(" + SrcFormatter.Format(inum2) + ")   ";
        //            }
        //            else
        //            {
        //                list[Information.UBound(list)] = list[Information.UBound(list)] + "      ";
        //            }

        //            if (withBlock.Rank < 10)
        //            {
        //                list[Information.UBound(list)] = list[Information.UBound(list)] + Strings.StrConv(SrcFormatter.Format(withBlock.Rank), VbStrConv.Wide);
        //            }
        //            else
        //            {
        //                list[Information.UBound(list)] = list[Information.UBound(list)] + SrcFormatter.Format(withBlock.Rank);
        //            }

        //            string argoname2 = "等身大基準";
        //            if (Expression.IsOptionDefined(argoname2))
        //            {
        //                if (withBlock.CountPilot() > 0)
        //                {
        //                    string localLeftPaddedString() { string argbuf = SrcFormatter.Format(withBlock.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString();
        //                }
        //            }

        //            string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(withBlock.MaxHP); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

        //            string localLeftPaddedString2() { string argbuf = SrcFormatter.Format(withBlock.MaxEN); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

        //            string localLeftPaddedString3() { string argbuf = SrcFormatter.Format(withBlock.get_Armor("")); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //            string localLeftPaddedString4() { string argbuf = SrcFormatter.Format(withBlock.get_Mobility("")); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString1() + localLeftPaddedString2() + localLeftPaddedString3() + localLeftPaddedString4();
        //            string argoname3 = "等身大基準";
        //            if (!Expression.IsOptionDefined(argoname3))
        //            {
        //                if (withBlock.CountPilot() > 0)
        //                {
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + " " + withBlock.MainPilot().get_Nickname(false);
        //                }
        //            }

        //            // ユニットＩＤを記録しておく
        //            id_list[Information.UBound(list)] = withBlock.ID;
        //        }

        //    NextUnit:
        //        ;
        //    }

        //    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
        //SortAgain:
        //    ;


        //    // ソート
        //    if (Strings.InStr(sort_mode, "名称") == 0)
        //    {
        //        // 数値によるソート

        //        // まずキーのリストを作成
        //        key_list = new int[Information.UBound(list) + 1];
        //        {
        //            var withBlock2 = SRC.UList;
        //            switch (sort_mode ?? "")
        //            {
        //                case "ＨＰ":
        //                    {
        //                        var loopTo1 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo1; i++)
        //                        {
        //                            Unit localItem() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem().MaxHP;
        //                        }

        //                        break;
        //                    }

        //                case "ＥＮ":
        //                    {
        //                        var loopTo2 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo2; i++)
        //                        {
        //                            Unit localItem1() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem1().MaxEN;
        //                        }

        //                        break;
        //                    }

        //                case "装甲":
        //                    {
        //                        var loopTo3 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo3; i++)
        //                        {
        //                            Unit localItem2() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem2().get_Armor("");
        //                        }

        //                        break;
        //                    }

        //                case "運動性":
        //                    {
        //                        var loopTo4 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo4; i++)
        //                        {
        //                            Unit localItem3() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem3().get_Mobility("");
        //                        }

        //                        break;
        //                    }

        //                case "ユニットランク":
        //                    {
        //                        var loopTo5 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo5; i++)
        //                        {
        //                            Unit localItem4() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem4().Rank;
        //                        }

        //                        break;
        //                    }

        //                case "レベル":
        //                    {
        //                        var loopTo6 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo6; i++)
        //                        {
        //                            var tmp = id_list;
        //                            object argIndex2 = tmp[i];
        //                            {
        //                                var withBlock3 = withBlock2.Item(argIndex2);
        //                                if (withBlock3.CountPilot() > 0)
        //                                {
        //                                    {
        //                                        var withBlock4 = withBlock3.MainPilot();
        //                                        key_list[i] = 500 * withBlock4.Level + withBlock4.Exp;
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        break;
        //                    }
        //            }
        //        }

        //        // キーを使って並べ替え
        //        var loopTo7 = (Information.UBound(list) - 1);
        //        for (i = 2; i <= loopTo7; i++)
        //        {
        //            max_item = i;
        //            max_value = key_list[i];
        //            var loopTo8 = Information.UBound(list);
        //            for (j = (i + 1); j <= loopTo8; j++)
        //            {
        //                if (key_list[j] > max_value)
        //                {
        //                    max_item = j;
        //                    max_value = key_list[j];
        //                }
        //            }

        //            if (max_item != i)
        //            {
        //                buf = list[i];
        //                list[i] = list[max_item];
        //                list[max_item] = buf;
        //                buf = id_list[i];
        //                id_list[i] = id_list[max_item];
        //                id_list[max_item] = buf;
        //                buf = GUI.ListItemComment[i];
        //                GUI.ListItemComment[i] = GUI.ListItemComment[max_item];
        //                GUI.ListItemComment[max_item] = buf;
        //                key_list[max_item] = key_list[i];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // 文字列によるソート

        //        // まずはキーのリストを作成
        //        strkey_list = new string[Information.UBound(list) + 1];
        //        {
        //            var withBlock5 = SRC.UList;
        //            switch (sort_mode ?? "")
        //            {
        //                case "名称":
        //                case "ユニット名称":
        //                    {
        //                        var loopTo9 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo9; i++)
        //                        {
        //                            Unit localItem5() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock5.Item(argIndex1); return ret; }

        //                            strkey_list[i] = localItem5().KanaName;
        //                        }

        //                        break;
        //                    }

        //                case "パイロット名称":
        //                    {
        //                        var loopTo10 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo10; i++)
        //                        {
        //                            var tmp1 = id_list;
        //                            object argIndex3 = tmp1[i];
        //                            {
        //                                var withBlock6 = withBlock5.Item(argIndex3);
        //                                if (withBlock6.CountPilot() > 0)
        //                                {
        //                                    strkey_list[i] = withBlock6.MainPilot().KanaName;
        //                                }
        //                            }
        //                        }

        //                        break;
        //                    }
        //            }
        //        }

        //        // キーを使って並べ替え
        //        var loopTo11 = (Information.UBound(strkey_list) - 1);
        //        for (i = 2; i <= loopTo11; i++)
        //        {
        //            max_item = i;
        //            max_str = strkey_list[i];
        //            var loopTo12 = Information.UBound(strkey_list);
        //            for (j = (i + 1); j <= loopTo12; j++)
        //            {
        //                if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
        //                {
        //                    max_item = j;
        //                    max_str = strkey_list[j];
        //                }
        //            }

        //            if (max_item != i)
        //            {
        //                buf = list[i];
        //                list[i] = list[max_item];
        //                list[max_item] = buf;
        //                buf = id_list[i];
        //                id_list[i] = id_list[max_item];
        //                id_list[max_item] = buf;
        //                buf = GUI.ListItemComment[i];
        //                GUI.ListItemComment[i] = GUI.ListItemComment[max_item];
        //                GUI.ListItemComment[max_item] = buf;
        //                strkey_list[max_item] = strkey_list[i];
        //            }
        //        }
        //    }

        //    // アイテムを交換するユニットを選択
        //    GUI.TopItem = top_item1;
        //    string argoname4 = "等身大基準";
        //    if (Expression.IsOptionDefined(argoname4))
        //    {
        //        string arglb_caption = "アイテムを交換するユニットを選択";
        //        string argtname = "RK";
        //        Unit argu = null;
        //        string argtname1 = "ＨＰ";
        //        Unit argu1 = null;
        //        string argtname2 = "ＥＮ";
        //        Unit argu2 = null;
        //        string argtname3 = "装甲";
        //        Unit argu3 = null;
        //        string argtname4 = "運動";
        //        Unit argu4 = null;
        //        string arglb_info = "ユニット                               アイテム " + Expression.Term(argtname, argu, 2) + " Lv  " + Expression.Term(argtname1, argu1, 4) + " " + Expression.Term(argtname2, argu2, 4) + " " + Expression.Term(argtname3, argu3, 4) + " " + Expression.Term(argtname4, u: argu4);
        //        string arglb_mode = "連続表示,コメント";
        //        ret = GUI.ListBox(arglb_caption, list, arglb_info, arglb_mode);
        //    }
        //    else
        //    {
        //        string arglb_caption1 = "アイテムを交換するユニットを選択";
        //        string argtname5 = "RK";
        //        Unit argu5 = null;
        //        string argtname6 = "ＨＰ";
        //        Unit argu6 = null;
        //        string argtname7 = "ＥＮ";
        //        Unit argu7 = null;
        //        string argtname8 = "装甲";
        //        Unit argu8 = null;
        //        string argtname9 = "運動";
        //        Unit argu9 = null;
        //        string arglb_info1 = "ユニット                       アイテム " + Expression.Term(argtname5, argu5, 2) + "  " + Expression.Term(argtname6, argu6, 4) + " " + Expression.Term(argtname7, argu7, 4) + " " + Expression.Term(argtname8, argu8, 4) + " " + Expression.Term(argtname9, argu9, 4) + " パイロット";
        //        string arglb_mode1 = "連続表示,コメント";
        //        ret = GUI.ListBox(arglb_caption1, list, arglb_info1, arglb_mode1);
        //    }

        //    top_item1 = GUI.TopItem;
        //    switch (ret)
        //    {
        //        case 0:
        //            {
        //                // キャンセル
        //                return;
        //            }

        //        case 1:
        //            {
        //                // ソート方法を選択
        //                string argoname5 = "等身大基準";
        //                if (Expression.IsOptionDefined(argoname5))
        //                {
        //                    sort_mode_type[1] = "名称";
        //                    sort_mode_list[1] = "名称";
        //                    sort_mode_type[2] = "レベル";
        //                    sort_mode_list[2] = "レベル";
        //                    sort_mode_type[3] = "ＨＰ";
        //                    string argtname10 = "ＨＰ";
        //                    Unit argu10 = null;
        //                    sort_mode_list[3] = Expression.Term(argtname10, u: argu10);
        //                    sort_mode_type[4] = "ＥＮ";
        //                    string argtname11 = "ＥＮ";
        //                    Unit argu11 = null;
        //                    sort_mode_list[4] = Expression.Term(argtname11, u: argu11);
        //                    sort_mode_type[5] = "装甲";
        //                    string argtname12 = "装甲";
        //                    Unit argu12 = null;
        //                    sort_mode_list[5] = Expression.Term(argtname12, u: argu12);
        //                    sort_mode_type[6] = "運動性";
        //                    string argtname13 = "運動性";
        //                    Unit argu13 = null;
        //                    sort_mode_list[6] = Expression.Term(argtname13, u: argu13);
        //                    sort_mode_type[7] = "ユニットランク";
        //                    string argtname14 = "ランク";
        //                    Unit argu14 = null;
        //                    sort_mode_list[7] = Expression.Term(argtname14, u: argu14);
        //                }
        //                else
        //                {
        //                    sort_mode_type[1] = "ＨＰ";
        //                    string argtname15 = "ＨＰ";
        //                    Unit argu15 = null;
        //                    sort_mode_list[1] = Expression.Term(argtname15, u: argu15);
        //                    sort_mode_type[2] = "ＥＮ";
        //                    string argtname16 = "ＥＮ";
        //                    Unit argu16 = null;
        //                    sort_mode_list[2] = Expression.Term(argtname16, u: argu16);
        //                    sort_mode_type[3] = "装甲";
        //                    string argtname17 = "装甲";
        //                    Unit argu17 = null;
        //                    sort_mode_list[3] = Expression.Term(argtname17, u: argu17);
        //                    sort_mode_type[4] = "運動性";
        //                    string argtname18 = "運動性";
        //                    Unit argu18 = null;
        //                    sort_mode_list[4] = Expression.Term(argtname18, u: argu18);
        //                    sort_mode_type[5] = "ユニットランク";
        //                    string argtname19 = "ランク";
        //                    Unit argu19 = null;
        //                    sort_mode_list[5] = Expression.Term(argtname19, u: argu19);
        //                    sort_mode_type[6] = "ユニット名称";
        //                    sort_mode_list[6] = "ユニット名称";
        //                    sort_mode_type[7] = "パイロット名称";
        //                    sort_mode_list[7] = "パイロット名称";
        //                }

        //                item_flag_backup = new bool[Information.UBound(list) + 1];
        //                item_comment_backup = new string[Information.UBound(list) + 1];
        //                var loopTo13 = Information.UBound(list);
        //                for (i = 2; i <= loopTo13; i++)
        //                {
        //                    item_flag_backup[i] = GUI.ListItemFlag[i];
        //                    item_comment_backup[i] = GUI.ListItemComment[i];
        //                }

        //                GUI.ListItemComment = new string[Information.UBound(sort_mode_list) + 1];
        //                GUI.ListItemFlag = new bool[Information.UBound(sort_mode_list) + 1];
        //                GUI.TopItem = 1;
        //                string arglb_caption2 = "どれで並べ替えますか？";
        //                string arglb_info2 = "並べ替えの方法";
        //                string arglb_mode2 = "連続表示,コメント";
        //                ret = GUI.ListBox(arglb_caption2, sort_mode_list, arglb_info2, arglb_mode2);
        //                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
        //                GUI.ListItemComment = new string[Information.UBound(list) + 1];
        //                var loopTo14 = Information.UBound(list);
        //                for (i = 2; i <= loopTo14; i++)
        //                {
        //                    GUI.ListItemFlag[i] = item_flag_backup[i];
        //                    GUI.ListItemComment[i] = item_comment_backup[i];
        //                }

        //                // ソート方法を変更して再表示
        //                if (ret > 0)
        //                {
        //                    sort_mode = sort_mode_type[ret];
        //                }

        //                goto SortAgain;
        //                break;
        //            }
        //    }

        //    // ユニットを選択
        //    var tmp2 = id_list;
        //    object argIndex4 = tmp2[ret];
        //    u = SRC.UList.Item(argIndex4);
        //MakeEquipedItemList:
        //    ;


        //    // 選択されたユニットが装備しているアイテム一覧の作成
        //    string[] tmp_part_list;
        //    {
        //        var withBlock7 = u;
        //        while (true)
        //        {
        //            // アイテムの装備個所一覧を作成
        //            part_list = new string[1];
        //            string argfname1 = "装備個所";
        //            if (withBlock7.IsFeatureAvailable(argfname1))
        //            {
        //                object argIndex5 = "装備個所";
        //                buf = withBlock7.FeatureData(argIndex5);
        //                if (Strings.InStr(buf, "腕") > 0)
        //                {
        //                    arm_point = (Information.UBound(part_list) + 1);
        //                    Array.Resize(part_list, Information.UBound(part_list) + 2 + 1);
        //                    part_list[1] = "右手";
        //                    part_list[2] = "左手";
        //                }

        //                if (Strings.InStr(buf, "肩") > 0)
        //                {
        //                    shoulder_point = (Information.UBound(part_list) + 1);
        //                    Array.Resize(part_list, Information.UBound(part_list) + 2 + 1);
        //                    part_list[Information.UBound(part_list) - 1] = "右肩";
        //                    part_list[Information.UBound(part_list)] = "左肩";
        //                }

        //                if (Strings.InStr(buf, "体") > 0)
        //                {
        //                    Array.Resize(part_list, Information.UBound(part_list) + 1 + 1);
        //                    part_list[Information.UBound(part_list)] = "体";
        //                }

        //                if (Strings.InStr(buf, "頭") > 0)
        //                {
        //                    Array.Resize(part_list, Information.UBound(part_list) + 1 + 1);
        //                    part_list[Information.UBound(part_list)] = "頭";
        //                }
        //            }

        //            var loopTo15 = withBlock7.CountFeature();
        //            for (i = 1; i <= loopTo15; i++)
        //            {
        //                object argIndex7 = i;
        //                if (withBlock7.Feature(argIndex7) == "ハードポイント")
        //                {
        //                    object argIndex6 = i;
        //                    ipart = withBlock7.FeatureData(argIndex6);
        //                    switch (ipart ?? "")
        //                    {
        //                        // 表示しない
        //                        case "強化パーツ":
        //                        case "アイテム":
        //                        case "非表示":
        //                            {
        //                                break;
        //                            }

        //                        default:
        //                            {
        //                                var loopTo16 = Information.UBound(part_list);
        //                                for (j = 1; j <= loopTo16; j++)
        //                                {
        //                                    if ((part_list[j] ?? "") == (ipart ?? ""))
        //                                    {
        //                                        break;
        //                                    }
        //                                }

        //                                if (j > Information.UBound(part_list))
        //                                {
        //                                    Array.Resize(part_list, Information.UBound(part_list) + withBlock7.ItemSlotSize(ipart) + 1);
        //                                    var loopTo17 = Information.UBound(part_list);
        //                                    for (j = (Information.UBound(part_list) - withBlock7.ItemSlotSize(ipart) + 1); j <= loopTo17; j++)
        //                                        part_list[j] = ipart;
        //                                }

        //                                break;
        //                            }
        //                    }
        //                }
        //            }

        //            Array.Resize(part_list, Information.UBound(part_list) + withBlock7.MaxItemNum() + 1);
        //            if (withBlock7.IsHero())
        //            {
        //                var loopTo18 = Information.UBound(part_list);
        //                for (i = (Information.UBound(part_list) - withBlock7.MaxItemNum() + 1); i <= loopTo18; i++)
        //                    part_list[i] = "アイテム";
        //            }
        //            else
        //            {
        //                var loopTo19 = Information.UBound(part_list);
        //                for (i = (Information.UBound(part_list) - withBlock7.MaxItemNum() + 1); i <= loopTo19; i++)
        //                    part_list[i] = "強化パーツ";
        //            }

        //            // 特定の装備個所のアイテムのみを交換する？
        //            if (!string.IsNullOrEmpty(selected_part))
        //            {
        //                tmp_part_list = new string[Information.UBound(part_list) + 1];
        //                var loopTo20 = Information.UBound(part_list);
        //                for (i = 1; i <= loopTo20; i++)
        //                    tmp_part_list[i] = part_list[i];
        //                part_list = new string[1];
        //                arm_point = 0;
        //                shoulder_point = 0;
        //                var loopTo21 = Information.UBound(tmp_part_list);
        //                for (i = 1; i <= loopTo21; i++)
        //                {
        //                    if ((tmp_part_list[i] ?? "") == (selected_part ?? "") | (selected_part == "片手" | selected_part == "両手" | selected_part == "盾") & (tmp_part_list[i] == "右手" | tmp_part_list[i] == "左手") | (selected_part == "肩" | selected_part == "両肩") & (tmp_part_list[i] == "右肩" | tmp_part_list[i] == "左肩") | (selected_part == "アイテム" | selected_part == "強化パーツ") & (tmp_part_list[i] == "アイテム" | tmp_part_list[i] == "強化パーツ"))
        //                    {
        //                        Array.Resize(part_list, Information.UBound(part_list) + 1 + 1);
        //                        part_list[Information.UBound(part_list)] = tmp_part_list[i];
        //                        switch (part_list[Information.UBound(part_list)] ?? "")
        //                        {
        //                            case "右手":
        //                                {
        //                                    arm_point = Information.UBound(part_list);
        //                                    break;
        //                                }

        //                            case "右肩":
        //                                {
        //                                    shoulder_point = Information.UBound(part_list);
        //                                    break;
        //                                }
        //                        }
        //                    }
        //                }
        //            }

        //            part_item = new string[Information.UBound(part_list) + 1];

        //            // 装備個所に現在装備しているアイテムを割り当て
        //            var loopTo22 = withBlock7.CountItem();
        //            for (i = 1; i <= loopTo22; i++)
        //            {
        //                object argIndex8 = i;
        //                {
        //                    var withBlock8 = withBlock7.Item(argIndex8);
        //                    string argfname2 = "非表示";
        //                    if (withBlock8.Class_Renamed() == "固定" & withBlock8.IsFeatureAvailable(argfname2))
        //                    {
        //                        goto NextEquipedItem;
        //                    }

        //                    switch (withBlock8.Part() ?? "")
        //                    {
        //                        case "両手":
        //                            {
        //                                if (arm_point == 0)
        //                                {
        //                                    goto NextEquipedItem;
        //                                }

        //                                part_item[arm_point] = withBlock8.ID;
        //                                part_item[arm_point + 1] = ":";
        //                                break;
        //                            }

        //                        case "片手":
        //                            {
        //                                if (arm_point == 0)
        //                                {
        //                                    goto NextEquipedItem;
        //                                }

        //                                if (string.IsNullOrEmpty(part_item[arm_point]))
        //                                {
        //                                    part_item[arm_point] = withBlock8.ID;
        //                                }
        //                                else
        //                                {
        //                                    part_item[arm_point + 1] = withBlock8.ID;
        //                                }

        //                                break;
        //                            }

        //                        case "盾":
        //                            {
        //                                if (arm_point == 0)
        //                                {
        //                                    goto NextEquipedItem;
        //                                }

        //                                part_item[arm_point + 1] = withBlock8.ID;
        //                                break;
        //                            }

        //                        case "両肩":
        //                            {
        //                                if (shoulder_point == 0)
        //                                {
        //                                    goto NextEquipedItem;
        //                                }

        //                                part_item[shoulder_point] = withBlock8.ID;
        //                                break;
        //                            }

        //                        case "肩":
        //                            {
        //                                if (shoulder_point == 0)
        //                                {
        //                                    goto NextEquipedItem;
        //                                }

        //                                if (string.IsNullOrEmpty(part_item[shoulder_point]))
        //                                {
        //                                    part_item[shoulder_point] = withBlock8.ID;
        //                                }
        //                                else
        //                                {
        //                                    part_item[shoulder_point + 1] = withBlock8.ID;
        //                                }

        //                                break;
        //                            }
        //                        // 無視
        //                        case "非表示":
        //                            {
        //                                break;
        //                            }

        //                        default:
        //                            {
        //                                if (withBlock8.Part() == "強化パーツ" | withBlock8.Part() == "アイテム")
        //                                {
        //                                    var loopTo23 = Information.UBound(part_list);
        //                                    for (j = 1; j <= loopTo23; j++)
        //                                    {
        //                                        if ((part_list[j] == "強化パーツ" | part_list[j] == "アイテム") & string.IsNullOrEmpty(part_item[j]))
        //                                        {
        //                                            part_item[j] = withBlock8.ID;
        //                                            var loopTo24 = (j + withBlock8.Size() - 1);
        //                                            for (k = (j + 1); k <= loopTo24; k++)
        //                                            {
        //                                                if (k > Information.UBound(part_item))
        //                                                {
        //                                                    break;
        //                                                }

        //                                                part_item[k] = ":";
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }
        //                                else
        //                                {
        //                                    var loopTo25 = Information.UBound(part_list);
        //                                    for (j = 1; j <= loopTo25; j++)
        //                                    {
        //                                        if ((part_list[j] ?? "") == (withBlock8.Part() ?? "") & string.IsNullOrEmpty(part_item[j]))
        //                                        {
        //                                            part_item[j] = withBlock8.ID;
        //                                            var loopTo26 = (j + withBlock8.Size() - 1);
        //                                            for (k = (j + 1); k <= loopTo26; k++)
        //                                            {
        //                                                if (k > Information.UBound(part_item))
        //                                                {
        //                                                    break;
        //                                                }

        //                                                part_item[k] = ":";
        //                                            }

        //                                            break;
        //                                        }
        //                                    }
        //                                }

        //                                if (j > Information.UBound(part_list) & string.IsNullOrEmpty(selected_part))
        //                                {
        //                                    Array.Resize(part_list, Information.UBound(part_list) + 1 + 1);
        //                                    Array.Resize(part_item, Information.UBound(part_list) + 1);
        //                                    part_list[Information.UBound(part_list)] = withBlock8.Part();
        //                                    part_item[Information.UBound(part_list)] = withBlock8.ID;
        //                                }

        //                                break;
        //                            }
        //                    }
        //                }

        //            NextEquipedItem:
        //                ;
        //            }

        //            list = new string[Information.UBound(part_list) + 1 + 1];
        //            id_list = new string[Information.UBound(list) + 1];
        //            GUI.ListItemComment = new string[Information.UBound(list) + 1];
        //            GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

        //            // リストを構築
        //            var loopTo27 = Information.UBound(part_item);
        //            for (i = 1; i <= loopTo27; i++)
        //            {
        //                switch (part_item[i] ?? "")
        //                {
        //                    case var @case when @case == "":
        //                        {
        //                            string argbuf2 = "----";
        //                            list[i] = GeneralLib.RightPaddedString(argbuf2, 23) + part_list[i];
        //                            break;
        //                        }

        //                    case ":":
        //                        {
        //                            string argbuf3 = " :  ";
        //                            list[i] = GeneralLib.RightPaddedString(argbuf3, 23) + part_list[i];
        //                            GUI.ListItemComment[i] = GUI.ListItemComment[i - 1];
        //                            GUI.ListItemFlag[i] = GUI.ListItemFlag[i - 1];
        //                            break;
        //                        }

        //                    default:
        //                        {
        //                            var tmp3 = part_item;
        //                            object argIndex9 = tmp3[i];
        //                            {
        //                                var withBlock9 = SRC.IList.Item(argIndex9);
        //                                list[i] = GeneralLib.RightPaddedString(withBlock9.Nickname(), 22) + " " + part_list[i];
        //                                GUI.ListItemComment[i] = withBlock9.Data.Comment;
        //                                id_list[i] = withBlock9.ID;
        //                                var loopTo28 = (i + withBlock9.Size() - 1);
        //                                for (j = (i + 1); j <= loopTo28; j++)
        //                                {
        //                                    if (j > Information.UBound(part_item))
        //                                    {
        //                                        break;
        //                                    }

        //                                    id_list[j] = withBlock9.ID;
        //                                }

        //                                bool localIsGlobalVariableDefined() { string argvname = "Fix(" + withBlock9.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

        //                                string argfname3 = "呪い";
        //                                if (localIsGlobalVariableDefined() | withBlock9.Class_Renamed() == "固定" | withBlock9.IsFeatureAvailable(argfname3))
        //                                {
        //                                    GUI.ListItemFlag[i] = true;
        //                                    var loopTo29 = (i + withBlock9.Size() - 1);
        //                                    for (j = (i + 1); j <= loopTo29; j++)
        //                                    {
        //                                        if (j > Information.UBound(part_item))
        //                                        {
        //                                            break;
        //                                        }

        //                                        GUI.ListItemFlag[j] = true;
        //                                    }
        //                                }
        //                            }

        //                            break;
        //                        }
        //                }
        //            }

        //            list[Information.UBound(list)] = "▽装備解除▽";

        //            // 交換するアイテムを選択
        //            caption_str = "装備個所を選択 ： " + withBlock7.Nickname;
        //            string argoname6 = "等身大基準";
        //            if (withBlock7.CountPilot() > 0 & !Expression.IsOptionDefined(argoname6))
        //            {
        //                caption_str = caption_str + " (" + withBlock7.MainPilot().get_Nickname(false) + ")";
        //            }

        //            string argtname20 = "ＨＰ";
        //            string argtname21 = "ＥＮ";
        //            string argtname22 = "装甲";
        //            string argtname23 = "運動性";
        //            string argtname24 = "移動力";
        //            caption_str = caption_str + "  " + Expression.Term(argtname20, u) + "=" + SrcFormatter.Format(withBlock7.MaxHP) + " " + Expression.Term(argtname21, u) + "=" + SrcFormatter.Format(withBlock7.MaxEN) + " " + Expression.Term(argtname22, u) + "=" + SrcFormatter.Format(withBlock7.get_Armor("")) + " " + Expression.Term(argtname23, u) + "=" + SrcFormatter.Format(withBlock7.get_Mobility("")) + " " + Expression.Term(argtname24, u) + "=" + SrcFormatter.Format(withBlock7.Speed);
        //            GUI.TopItem = top_item2;
        //            string arglb_info3 = "アイテム               分類";
        //            string arglb_mode3 = "連続表示,コメント";
        //            ret = GUI.ListBox(caption_str, list, arglb_info3, arglb_mode3);
        //            top_item2 = GUI.TopItem;
        //            if (ret == 0)
        //            {
        //                break;
        //            }

        //            // 装備を解除する場合
        //            if (ret == Information.UBound(list))
        //            {
        //                list[Information.UBound(list)] = "▽全て外す▽";
        //                caption_str = "外すアイテムを選択 ： " + withBlock7.Nickname;
        //                string argoname7 = "等身大基準";
        //                if (withBlock7.CountPilot() > 0 & !Expression.IsOptionDefined(argoname7))
        //                {
        //                    caption_str = caption_str + " (" + withBlock7.MainPilot().get_Nickname(false) + ")";
        //                }

        //                string argtname25 = "ＨＰ";
        //                string argtname26 = "ＥＮ";
        //                string argtname27 = "装甲";
        //                string argtname28 = "運動性";
        //                string argtname29 = "移動力";
        //                caption_str = caption_str + "  " + Expression.Term(argtname25, u) + "=" + SrcFormatter.Format(withBlock7.MaxHP) + " " + Expression.Term(argtname26, u) + "=" + SrcFormatter.Format(withBlock7.MaxEN) + " " + Expression.Term(argtname27, u) + "=" + SrcFormatter.Format(withBlock7.get_Armor("")) + " " + Expression.Term(argtname28, u) + "=" + SrcFormatter.Format(withBlock7.get_Mobility("")) + " " + Expression.Term(argtname29, u) + "=" + SrcFormatter.Format(withBlock7.Speed);
        //                string arglb_info4 = "アイテム               分類";
        //                string arglb_mode4 = "連続表示,コメント";
        //                ret = GUI.ListBox(caption_str, list, arglb_info4, arglb_mode4);
        //                if (ret != 0)
        //                {
        //                    if (ret < Information.UBound(list))
        //                    {
        //                        // 指定されたアイテムを外す
        //                        if (!string.IsNullOrEmpty(id_list[ret]))
        //                        {
        //                            var tmp4 = id_list;
        //                            object argIndex10 = tmp4[ret];
        //                            withBlock7.DeleteItem(argIndex10, false);
        //                        }
        //                        else if (GeneralLib.LIndex(list[ret], 1) == ":")
        //                        {
        //                            var tmp5 = id_list;
        //                            object argIndex11 = tmp5[ret - 1];
        //                            withBlock7.DeleteItem(argIndex11, false);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        // 全てのアイテムを外す
        //                        var loopTo30 = (Information.UBound(list) - 1);
        //                        for (i = 1; i <= loopTo30; i++)
        //                        {
        //                            if (!GUI.ListItemFlag[i] & !string.IsNullOrEmpty(id_list[i]))
        //                            {
        //                                var tmp6 = id_list;
        //                                object argIndex12 = tmp6[i];
        //                                withBlock7.DeleteItem(argIndex12, false);
        //                            }
        //                        }
        //                    }

        //                    if (string.IsNullOrEmpty(Map.MapFileName))
        //                    {
        //                        withBlock7.FullRecover();
        //                    }

        //                    if (GUI.MainForm.Visible)
        //                    {
        //                        Status.DisplayUnitStatus(u);
        //                    }
        //                }

        //                goto NextLoop2;
        //            }

        //            // 交換するアイテムの装備個所
        //            iid = id_list[ret];
        //            if (!string.IsNullOrEmpty(iid))
        //            {
        //                Item localItem6() { object argIndex1 = iid; var ret = SRC.IList.Item(argIndex1); return ret; }

        //                ipart = localItem6().Part();
        //            }
        //            else
        //            {
        //                ipart = GeneralLib.LIndex(list[ret], 2);
        //            }

        //            // 空きスロットを調べておく
        //            switch (ipart ?? "")
        //            {
        //                case "右手":
        //                case "左手":
        //                case "片手":
        //                case "両手":
        //                case "盾":
        //                    {
        //                        is_right_hand_available = true;
        //                        is_left_hand_available = true;
        //                        var loopTo31 = withBlock7.CountItem();
        //                        for (i = 1; i <= loopTo31; i++)
        //                        {
        //                            object argIndex13 = i;
        //                            {
        //                                var withBlock10 = withBlock7.Item(argIndex13);
        //                                if (withBlock10.Part() == "片手")
        //                                {
        //                                    bool localIsGlobalVariableDefined1() { string argvname = "Fix(" + withBlock10.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

        //                                    string argfname4 = "呪い";
        //                                    if (localIsGlobalVariableDefined1() | withBlock10.Class_Renamed() == "固定" | withBlock10.IsFeatureAvailable(argfname4))
        //                                    {
        //                                        if (is_right_hand_available)
        //                                        {
        //                                            is_right_hand_available = false;
        //                                        }
        //                                        else
        //                                        {
        //                                            is_left_hand_available = false;
        //                                        }
        //                                    }
        //                                }
        //                                else if (withBlock10.Part() == "盾")
        //                                {
        //                                    bool localIsGlobalVariableDefined2() { string argvname = "Fix(" + withBlock10.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

        //                                    string argfname5 = "呪い";
        //                                    if (localIsGlobalVariableDefined2() | withBlock10.Class_Renamed() == "固定" | withBlock10.IsFeatureAvailable(argfname5))
        //                                    {
        //                                        is_left_hand_available = false;
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        break;
        //                    }

        //                case "右肩":
        //                case "左肩":
        //                case "肩":
        //                    {
        //                        empty_slot = 2;
        //                        var loopTo32 = withBlock7.CountItem();
        //                        for (i = 1; i <= loopTo32; i++)
        //                        {
        //                            object argIndex14 = i;
        //                            {
        //                                var withBlock11 = withBlock7.Item(argIndex14);
        //                                if (withBlock11.Part() == "肩")
        //                                {
        //                                    bool localIsGlobalVariableDefined3() { string argvname = "Fix(" + withBlock11.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

        //                                    string argfname6 = "呪い";
        //                                    if (localIsGlobalVariableDefined3() | withBlock11.Class_Renamed() == "固定" | withBlock11.IsFeatureAvailable(argfname6))
        //                                    {
        //                                        empty_slot = (empty_slot - 1);
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        break;
        //                    }

        //                case "強化パーツ":
        //                case "アイテム":
        //                    {
        //                        empty_slot = withBlock7.MaxItemNum();
        //                        var loopTo33 = withBlock7.CountItem();
        //                        for (i = 1; i <= loopTo33; i++)
        //                        {
        //                            object argIndex15 = i;
        //                            {
        //                                var withBlock12 = withBlock7.Item(argIndex15);
        //                                if (withBlock12.Part() == "強化パーツ" | withBlock12.Part() == "アイテム")
        //                                {
        //                                    bool localIsGlobalVariableDefined4() { string argvname = "Fix(" + withBlock12.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

        //                                    string argfname7 = "呪い";
        //                                    if (localIsGlobalVariableDefined4() | withBlock12.Class_Renamed() == "固定" | withBlock12.IsFeatureAvailable(argfname7))
        //                                    {
        //                                        empty_slot = (empty_slot - withBlock12.Size());
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        break;
        //                    }

        //                default:
        //                    {
        //                        empty_slot = 0;
        //                        var loopTo34 = withBlock7.CountFeature();
        //                        for (i = 1; i <= loopTo34; i++)
        //                        {
        //                            string localFeature() { object argIndex1 = i; var ret = withBlock7.Feature(argIndex1); return ret; }

        //                            string localFeatureData() { object argIndex1 = i; var ret = withBlock7.FeatureData(argIndex1); return ret; }

        //                            if (localFeature() == "ハードポイント" & (localFeatureData() ?? "") == (ipart ?? ""))
        //                            {
        //                                double localFeatureLevel() { object argIndex1 = i; var ret = withBlock7.FeatureLevel(argIndex1); return ret; }

        //                                empty_slot = (empty_slot + localFeatureLevel());
        //                            }
        //                        }

        //                        if (empty_slot == 0)
        //                        {
        //                            empty_slot = 1;
        //                        }

        //                        var loopTo35 = withBlock7.CountItem();
        //                        for (i = 1; i <= loopTo35; i++)
        //                        {
        //                            object argIndex16 = i;
        //                            {
        //                                var withBlock13 = withBlock7.Item(argIndex16);
        //                                if ((withBlock13.Part() ?? "") == (ipart ?? ""))
        //                                {
        //                                    bool localIsGlobalVariableDefined5() { string argvname = "Fix(" + withBlock13.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

        //                                    string argfname8 = "呪い";
        //                                    if (localIsGlobalVariableDefined5() | withBlock13.Class_Renamed() == "固定" | withBlock13.IsFeatureAvailable(argfname8))
        //                                    {
        //                                        empty_slot = (empty_slot - withBlock13.Size());
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        break;
        //                    }
        //            }

        //            while (true)
        //            {
        //                // 装備可能なアイテムを調べる
        //                item_list = new string[1];
        //                foreach (Item currentIt in SRC.IList)
        //                {
        //                    it = currentIt;
        //                    {
        //                        var withBlock14 = it;
        //                        if (!withBlock14.Exist)
        //                        {
        //                            goto NextItem;
        //                        }

        //                        // 装備スロットが空いている？
        //                        switch (ipart ?? "")
        //                        {
        //                            case "右手":
        //                            case "左手":
        //                            case "片手":
        //                            case "両手":
        //                                {
        //                                    switch (withBlock14.Part() ?? "")
        //                                    {
        //                                        case "両手":
        //                                            {
        //                                                if (!is_right_hand_available | !is_left_hand_available)
        //                                                {
        //                                                    goto NextItem;
        //                                                }

        //                                                break;
        //                                            }

        //                                        case "片手":
        //                                            {
        //                                                string argfname9 = "両手持ち";
        //                                                if (u.IsFeatureAvailable(argfname9))
        //                                                {
        //                                                    if (!is_right_hand_available & !is_left_hand_available)
        //                                                    {
        //                                                        goto NextItem;
        //                                                    }
        //                                                }
        //                                                else if (!is_right_hand_available)
        //                                                {
        //                                                    goto NextItem;
        //                                                }

        //                                                break;
        //                                            }

        //                                        case "盾":
        //                                            {
        //                                                if (!is_left_hand_available)
        //                                                {
        //                                                    goto NextItem;
        //                                                }

        //                                                break;
        //                                            }

        //                                        default:
        //                                            {
        //                                                goto NextItem;
        //                                                break;
        //                                            }
        //                                    }

        //                                    break;
        //                                }

        //                            case "盾":
        //                                {
        //                                    switch (withBlock14.Part() ?? "")
        //                                    {
        //                                        case "両手":
        //                                            {
        //                                                if (!is_right_hand_available | !is_left_hand_available)
        //                                                {
        //                                                    goto NextItem;
        //                                                }

        //                                                break;
        //                                            }

        //                                        case "片手":
        //                                            {
        //                                                string argfname10 = "両手持ち";
        //                                                if (u.IsFeatureAvailable(argfname10))
        //                                                {
        //                                                    if (!is_right_hand_available & !is_left_hand_available)
        //                                                    {
        //                                                        goto NextItem;
        //                                                    }
        //                                                }
        //                                                else
        //                                                {
        //                                                    goto NextItem;
        //                                                }

        //                                                break;
        //                                            }

        //                                        case "盾":
        //                                            {
        //                                                if (!is_left_hand_available)
        //                                                {
        //                                                    goto NextItem;
        //                                                }

        //                                                break;
        //                                            }

        //                                        default:
        //                                            {
        //                                                goto NextItem;
        //                                                break;
        //                                            }
        //                                    }

        //                                    break;
        //                                }

        //                            case "右肩":
        //                            case "左肩":
        //                            case "肩":
        //                                {
        //                                    if (withBlock14.Part() != "両肩" & withBlock14.Part() != "肩")
        //                                    {
        //                                        goto NextItem;
        //                                    }

        //                                    if (withBlock14.Part() == "両肩")
        //                                    {
        //                                        if (empty_slot < 2)
        //                                        {
        //                                            goto NextItem;
        //                                        }
        //                                    }

        //                                    break;
        //                                }

        //                            case "強化パーツ":
        //                            case "アイテム":
        //                                {
        //                                    if (withBlock14.Part() != "強化パーツ" & withBlock14.Part() != "アイテム")
        //                                    {
        //                                        goto NextItem;
        //                                    }

        //                                    if (empty_slot < withBlock14.Size())
        //                                    {
        //                                        goto NextItem;
        //                                    }

        //                                    break;
        //                                }

        //                            default:
        //                                {
        //                                    if ((withBlock14.Part() ?? "") != (ipart ?? ""))
        //                                    {
        //                                        goto NextItem;
        //                                    }

        //                                    if (empty_slot < withBlock14.Size())
        //                                    {
        //                                        goto NextItem;
        //                                    }

        //                                    break;
        //                                }
        //                        }

        //                        if (withBlock14.Unit_Renamed is object)
        //                        {
        //                            {
        //                                var withBlock15 = withBlock14.Unit_Renamed.CurrentForm();
        //                                // 離脱したユニットが装備している
        //                                if (withBlock15.Status_Renamed == "離脱")
        //                                {
        //                                    goto NextItem;
        //                                }

        //                                // 敵ユニットが装備している
        //                                if (withBlock15.Party != "味方")
        //                                {
        //                                    goto NextItem;
        //                                }
        //                            }

        //                            // 呪われているので外せない……
        //                            string argfname11 = "呪い";
        //                            if (withBlock14.IsFeatureAvailable(argfname11))
        //                            {
        //                                goto NextItem;
        //                            }
        //                        }

        //                        // 既に登録済み？
        //                        var loopTo36 = Information.UBound(item_list);
        //                        for (i = 1; i <= loopTo36; i++)
        //                        {
        //                            if ((item_list[i] ?? "") == (withBlock14.Name ?? ""))
        //                            {
        //                                goto NextItem;
        //                            }
        //                        }
        //                    }

        //                    // 装備可能？
        //                    if (!withBlock7.IsAbleToEquip(it))
        //                    {
        //                        goto NextItem;
        //                    }

        //                    Array.Resize(item_list, Information.UBound(item_list) + 1 + 1);
        //                    item_list[Information.UBound(item_list)] = it.Name;
        //                NextItem:
        //                    ;
        //                }

        //                // 装備可能なアイテムの一覧を表示
        //                list = new string[Information.UBound(item_list) + 1];
        //                strkey_list = new string[Information.UBound(item_list) + 1];
        //                id_list = new string[Information.UBound(item_list) + 1];
        //                GUI.ListItemFlag = new bool[Information.UBound(item_list) + 1];
        //                GUI.ListItemComment = new string[Information.UBound(item_list) + 1];
        //                var loopTo37 = Information.UBound(item_list);
        //                for (i = 1; i <= loopTo37; i++)
        //                {
        //                    iname = item_list[i];
        //                    object argIndex17 = iname;
        //                    {
        //                        var withBlock16 = SRC.IDList.Item(argIndex17);
        //                        string localRightPaddedString() { string argbuf = withBlock16.Nickname; var ret = GeneralLib.RightPaddedString(argbuf, 22); withBlock16.Nickname = argbuf; return ret; }

        //                        list[i] = localRightPaddedString() + " ";
        //                        string argfname12 = "大型アイテム";
        //                        if (withBlock16.IsFeatureAvailable(argfname12))
        //                        {
        //                            string localRightPaddedString1() { string argbuf = withBlock16.Part + "[" + SrcFormatter.Format(withBlock16.Size()) + "]"; var ret = GeneralLib.RightPaddedString(argbuf, 15); return ret; }

        //                            list[i] = list[i] + localRightPaddedString1();
        //                        }
        //                        else
        //                        {
        //                            list[i] = list[i] + GeneralLib.RightPaddedString(withBlock16.Part, 15);
        //                        }

        //                        // アイテムの数をカウント
        //                        inum = 0;
        //                        inum2 = 0;
        //                        foreach (Item currentIt1 in SRC.IList)
        //                        {
        //                            it = currentIt1;
        //                            {
        //                                var withBlock17 = it;
        //                                if ((withBlock17.Name ?? "") == (iname ?? ""))
        //                                {
        //                                    if (withBlock17.Exist)
        //                                    {
        //                                        if (withBlock17.Unit_Renamed is null)
        //                                        {
        //                                            inum = (inum + 1);
        //                                            inum2 = (inum2 + 1);
        //                                        }
        //                                        else if (withBlock17.Unit_Renamed.CurrentForm().Status_Renamed != "離脱")
        //                                        {
        //                                            string argfname13 = "呪い";
        //                                            if (!withBlock17.IsFeatureAvailable(argfname13))
        //                                            {
        //                                                inum = (inum + 1);
        //                                            }
        //                                        }
        //                                    }
        //                                }
        //                            }
        //                        }

        //                        string localLeftPaddedString5() { string argbuf = SrcFormatter.Format(inum2); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

        //                        string localLeftPaddedString6() { string argbuf = SrcFormatter.Format(inum); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

        //                        list[i] = list[i] + localLeftPaddedString5() + "/" + localLeftPaddedString6();
        //                        id_list[i] = withBlock16.Name;
        //                        strkey_list[i] = withBlock16.KanaName;
        //                        GUI.ListItemComment[i] = withBlock16.Comment;
        //                    }
        //                }

        //                // アイテムを名前順にソート
        //                var loopTo38 = (Information.UBound(strkey_list) - 1);
        //                for (i = 1; i <= loopTo38; i++)
        //                {
        //                    max_item = i;
        //                    max_str = strkey_list[i];
        //                    var loopTo39 = Information.UBound(strkey_list);
        //                    for (j = (i + 1); j <= loopTo39; j++)
        //                    {
        //                        if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
        //                        {
        //                            max_item = j;
        //                            max_str = strkey_list[j];
        //                        }
        //                    }

        //                    if (max_item != i)
        //                    {
        //                        buf = list[i];
        //                        list[i] = list[max_item];
        //                        list[max_item] = buf;
        //                        buf = id_list[i];
        //                        id_list[i] = id_list[max_item];
        //                        id_list[max_item] = buf;
        //                        buf = GUI.ListItemComment[i];
        //                        GUI.ListItemComment[i] = GUI.ListItemComment[max_item];
        //                        GUI.ListItemComment[max_item] = buf;
        //                        strkey_list[max_item] = strkey_list[i];
        //                    }
        //                }

        //                // 装備するアイテムの種類を選択
        //                caption_str = "装備するアイテムを選択 ： " + withBlock7.Nickname;
        //                string argoname8 = "等身大基準";
        //                if (withBlock7.CountPilot() > 0 & !Expression.IsOptionDefined(argoname8))
        //                {
        //                    caption_str = caption_str + " (" + withBlock7.MainPilot().get_Nickname(false) + ")";
        //                }

        //                string argtname30 = "ＨＰ";
        //                string argtname31 = "ＥＮ";
        //                string argtname32 = "装甲";
        //                string argtname33 = "運動性";
        //                string argtname34 = "移動力";
        //                caption_str = caption_str + "  " + Expression.Term(argtname30, u) + "=" + SrcFormatter.Format(withBlock7.MaxHP) + " " + Expression.Term(argtname31, u) + "=" + SrcFormatter.Format(withBlock7.MaxEN) + " " + Expression.Term(argtname32, u) + "=" + SrcFormatter.Format(withBlock7.get_Armor("")) + " " + Expression.Term(argtname33, u) + "=" + SrcFormatter.Format(withBlock7.get_Mobility("")) + " " + Expression.Term(argtname34, u) + "=" + SrcFormatter.Format(withBlock7.Speed);
        //                string arglb_info5 = "アイテム               分類            数量";
        //                string arglb_mode5 = "連続表示,コメント";
        //                ret = GUI.ListBox(caption_str, list, arglb_info5, arglb_mode5);

        //                // キャンセルされた？
        //                if (ret == 0)
        //                {
        //                    break;
        //                }

        //                iname = id_list[ret];

        //                // 未装備のアイテムがあるかどうか探す
        //                foreach (Item currentIt2 in SRC.IList)
        //                {
        //                    it = currentIt2;
        //                    {
        //                        var withBlock18 = it;
        //                        if ((withBlock18.Name ?? "") == (iname ?? "") & withBlock18.Exist)
        //                        {
        //                            if (withBlock18.Unit_Renamed is null)
        //                            {
        //                                // 未装備の装備が見つかったのでそれを装備
        //                                if (!string.IsNullOrEmpty(iid))
        //                                {
        //                                    object argIndex18 = iid;
        //                                    u.DeleteItem(argIndex18);
        //                                }
        //                                // 呪いのアイテムを装備……
        //                                string argfname14 = "呪い";
        //                                if (withBlock18.IsFeatureAvailable(argfname14))
        //                                {
        //                                    Interaction.MsgBox(withBlock18.Nickname() + "は呪われていた！");
        //                                }

        //                                u.AddItem(it);
        //                                if (string.IsNullOrEmpty(Map.MapFileName))
        //                                {
        //                                    u.FullRecover();
        //                                }

        //                                if (GUI.MainForm.Visible)
        //                                {
        //                                    Status.DisplayUnitStatus(u);
        //                                }

        //                                break;
        //                            }
        //                        }
        //                    }
        //                }

        //                // 選択されたアイテムを列挙
        //                list = new string[1];
        //                id_list = new string[1];
        //                GUI.ListItemComment = new string[1];
        //                inum = 0;
        //                ItemData localItem7() { object argIndex1 = iname; var ret = SRC.IDList.Item(argIndex1); return ret; }

        //                string argfname16 = "呪い";
        //                if (!localItem7().IsFeatureAvailable(argfname16))
        //                {
        //                    foreach (Item currentIt3 in SRC.IList)
        //                    {
        //                        it = currentIt3;
        //                        if ((it.Name ?? "") != (iname ?? "") | !it.Exist)
        //                        {
        //                            goto NextItem2;
        //                        }

        //                        if (it.Unit_Renamed is null)
        //                        {
        //                            goto NextItem2;
        //                        }

        //                        {
        //                            var withBlock19 = it.Unit_Renamed.CurrentForm();
        //                            if (withBlock19.Status_Renamed == "離脱")
        //                            {
        //                                goto NextItem2;
        //                            }

        //                            if (withBlock19.Party != "味方")
        //                            {
        //                                goto NextItem2;
        //                            }

        //                            Array.Resize(list, Information.UBound(list) + 1 + 1);
        //                            Array.Resize(id_list, Information.UBound(list) + 1);
        //                            Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);
        //                            string argoname9 = "等身大基準";
        //                            if (!Expression.IsOptionDefined(argoname9) & withBlock19.CountPilot() > 0)
        //                            {
        //                                string localRightPaddedString2() { string argbuf = withBlock19.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 36); withBlock19.Nickname0 = argbuf; return ret; }

        //                                list[Information.UBound(list)] = localRightPaddedString2() + " " + withBlock19.MainPilot().get_Nickname(false);
        //                            }
        //                            else
        //                            {
        //                                list[Information.UBound(list)] = withBlock19.Nickname0;
        //                            }

        //                            id_list[Information.UBound(list)] = it.ID;
        //                            var loopTo40 = withBlock19.CountItem();
        //                            for (i = 1; i <= loopTo40; i++)
        //                            {
        //                                object argIndex19 = i;
        //                                {
        //                                    var withBlock20 = withBlock19.Item(argIndex19);
        //                                    string argfname15 = "非表示";
        //                                    if ((withBlock20.Class_Renamed() != "固定" | !withBlock20.IsFeatureAvailable(argfname15)) & withBlock20.Part() != "非表示")
        //                                    {
        //                                        GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + withBlock20.Nickname() + " ";
        //                                    }
        //                                }
        //                            }

        //                            inum = (inum + 1);
        //                        }

        //                    NextItem2:
        //                        ;
        //                    }
        //                }

        //                GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
        //                Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

        //                // どのアイテムを装備するか選択
        //                Item localItem8() { var tmp = id_list; object argIndex1 = tmp[1]; var ret = SRC.IList.Item(argIndex1); return ret; }

        //                caption_str = localItem8().Nickname() + "の入手先を選択 ： " + withBlock7.Nickname;
        //                string argoname10 = "等身大基準";
        //                if (withBlock7.CountPilot() > 0 & !Expression.IsOptionDefined(argoname10))
        //                {
        //                    caption_str = caption_str + " (" + withBlock7.MainPilot().get_Nickname(false) + ")";
        //                }

        //                string argtname35 = "ＨＰ";
        //                string argtname36 = "ＥＮ";
        //                string argtname37 = "装甲";
        //                string argtname38 = "運動性";
        //                string argtname39 = "移動力";
        //                caption_str = caption_str + "  " + Expression.Term(argtname35, u) + "=" + SrcFormatter.Format(withBlock7.MaxHP) + " " + Expression.Term(argtname36, u) + "=" + SrcFormatter.Format(withBlock7.MaxEN) + " " + Expression.Term(argtname37, u) + "=" + SrcFormatter.Format(withBlock7.get_Armor("")) + " " + Expression.Term(argtname38, u) + "=" + SrcFormatter.Format(withBlock7.get_Mobility("")) + " " + Expression.Term(argtname39, u) + "=" + SrcFormatter.Format(withBlock7.Speed);
        //                GUI.TopItem = 1;
        //                string argoname11 = "等身大基準";
        //                if (Expression.IsOptionDefined(argoname11))
        //                {
        //                    string arglb_info6 = "ユニット";
        //                    string arglb_mode6 = "連続表示,コメント";
        //                    ret = GUI.ListBox(caption_str, list, arglb_info6, arglb_mode6);
        //                }
        //                else
        //                {
        //                    string arglb_info7 = "ユニット                             パイロット";
        //                    string arglb_mode7 = "連続表示,コメント";
        //                    ret = GUI.ListBox(caption_str, list, arglb_info7, arglb_mode7);
        //                }

        //                // アイテムを交換
        //                if (ret > 0)
        //                {
        //                    if (!string.IsNullOrEmpty(iid))
        //                    {
        //                        object argIndex20 = iid;
        //                        withBlock7.DeleteItem(argIndex20);
        //                    }

        //                    var tmp7 = id_list;
        //                    object argIndex22 = tmp7[ret];
        //                    {
        //                        var withBlock21 = SRC.IList.Item(argIndex22);
        //                        if (withBlock21.Unit_Renamed is object)
        //                        {
        //                            object argIndex21 = withBlock21.ID;
        //                            withBlock21.Unit_Renamed.DeleteItem(argIndex21);
        //                        }

        //                        // 呪いのアイテムを装備……
        //                        string argfname17 = "呪い";
        //                        if (withBlock21.IsFeatureAvailable(argfname17))
        //                        {
        //                            Interaction.MsgBox(withBlock21.Nickname() + "は呪われていた！");
        //                        }
        //                    }

        //                    Item localItem9() { var tmp = id_list; object argIndex1 = tmp[ret]; var ret = SRC.IList.Item(argIndex1); return ret; }

        //                    var argitm = localItem9();
        //                    withBlock7.AddItem(argitm);
        //                    if (string.IsNullOrEmpty(Map.MapFileName))
        //                    {
        //                        withBlock7.FullRecover();
        //                    }

        //                    if (GUI.MainForm.Visible)
        //                    {
        //                        Status.DisplayUnitStatus(u);
        //                    }

        //                    break;
        //                }

        //            NextLoop:
        //                ;
        //            }

        //        NextLoop2:
        //            ;
        //        }
        //    }

        //    // ユニットがあらかじめ選択されている場合
        //    // (ユニットステータスからのアイテム交換時)
        //    if (selected_unit is object)
        //    {
        //        {
        //            var withBlock22 = My.MyProject.Forms.frmListBox;
        //            withBlock22.Hide();
        //            if (withBlock22.txtComment.Enabled)
        //            {
        //                withBlock22.txtComment.Enabled = false;
        //                withBlock22.txtComment.Visible = false;
        //                withBlock22.Height = SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock22.Height) - 600d);
        //            }
        //        }

        //        GUI.ReduceListBoxHeight();
        //        GUI.EnlargeListBoxWidth();
        //        return;
        //    }

        //    goto Beginning;
        //}

        //// 換装コマンド
        //// MOD START MARGE
        //// Public Sub ExchangeFormCommand()
        //private void ExchangeFormCommand()
        //{
        //    // MOD END MARGE
        //    int j, i, k;
        //    string[] list;
        //    string[] id_list;
        //    int[] key_list;
        //    string[] list2;
        //    string[] id_list2;
        //    int max_item, max_value;
        //    Unit u;
        //    string buf;
        //    int ret;
        //    int top_item;
        //    string[] farray;
        //Beginning:
        //    ;
        //    top_item = 1;

        //    // 換装可能なユニットのリストを作成
        //    list = new string[1];
        //    id_list = new string[1];
        //    GUI.ListItemComment = new string[1];
        //    foreach (Unit currentU in SRC.UList)
        //    {
        //        u = currentU;
        //        {
        //            var withBlock = u;
        //            // 待機中の味方ユニット？
        //            if (withBlock.Party0 != "味方" | withBlock.Status_Renamed != "待機")
        //            {
        //                goto NextLoop;
        //            }

        //            // 換装能力を持っている？
        //            string argfname = "換装";
        //            if (!withBlock.IsFeatureAvailable(argfname))
        //            {
        //                goto NextLoop;
        //            }

        //            // いずれかの形態に換装可能？
        //            object argIndex1 = "換装";
        //            string arglist = withBlock.FeatureData(argIndex1);
        //            var loopTo = GeneralLib.LLength(arglist);
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                string localLIndex() { object argIndex1 = "換装"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LIndex(arglist, i); return ret; }

        //                Unit localOtherForm() { object argIndex1 = (object)hsfe39adde656b471593385f8a950d6e8a(); var ret = withBlock.OtherForm(argIndex1); return ret; }

        //                if (localOtherForm().IsAvailable())
        //                {
        //                    break;
        //                }
        //            }

        //            int localLLength() { object argIndex1 = "換装"; string arglist = withBlock.FeatureData(argIndex1); var ret = GeneralLib.LLength(arglist); return ret; }

        //            if (i > localLLength())
        //            {
        //                goto NextLoop;
        //            }

        //            Array.Resize(list, Information.UBound(list) + 1 + 1);
        //            Array.Resize(id_list, Information.UBound(list) + 1);
        //            Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

        //            // ユニットのステータスを表示
        //            string argoname = "等身大基準";
        //            if (Expression.IsOptionDefined(argoname))
        //            {
        //                if (withBlock.Rank < 10)
        //                {
        //                    string localRightPaddedString() { string argbuf = withBlock.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 37); withBlock.Nickname0 = argbuf; return ret; }

        //                    list[Information.UBound(list)] = localRightPaddedString() + Strings.StrConv(SrcFormatter.Format(withBlock.Rank), VbStrConv.Wide);
        //                }
        //                else
        //                {
        //                    string localRightPaddedString1() { string argbuf = withBlock.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 37); withBlock.Nickname0 = argbuf; return ret; }

        //                    list[Information.UBound(list)] = localRightPaddedString1() + SrcFormatter.Format(withBlock.Rank);
        //                }
        //            }
        //            else if (withBlock.Rank < 10)
        //            {
        //                string localRightPaddedString2() { string argbuf = withBlock.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 33); withBlock.Nickname0 = argbuf; return ret; }

        //                list[Information.UBound(list)] = localRightPaddedString2() + Strings.StrConv(SrcFormatter.Format(withBlock.Rank), VbStrConv.Wide);
        //            }
        //            else
        //            {
        //                string localRightPaddedString3() { string argbuf = withBlock.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 33); withBlock.Nickname0 = argbuf; return ret; }

        //                list[Information.UBound(list)] = localRightPaddedString3() + SrcFormatter.Format(withBlock.Rank);
        //            }

        //            string localLeftPaddedString() { string argbuf = SrcFormatter.Format(withBlock.MaxHP); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

        //            string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(withBlock.MaxEN); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //            string localLeftPaddedString2() { string argbuf = SrcFormatter.Format(withBlock.get_Armor("")); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //            string localLeftPaddedString3() { string argbuf = SrcFormatter.Format(withBlock.get_Mobility("")); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //            list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString() + localLeftPaddedString1() + localLeftPaddedString2() + localLeftPaddedString3();
        //            if (withBlock.CountPilot() > 0)
        //            {
        //                string argoname1 = "等身大基準";
        //                if (Expression.IsOptionDefined(argoname1))
        //                {
        //                    string localLeftPaddedString4() { string argbuf = SrcFormatter.Format(withBlock.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + "  " + localLeftPaddedString4();
        //                }
        //                else
        //                {
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + "  " + withBlock.MainPilot().get_Nickname(false);
        //                }
        //            }

        //            // ユニットに装備されているアイテムをコメント欄に列記
        //            var loopTo1 = withBlock.CountItem();
        //            for (k = 1; k <= loopTo1; k++)
        //            {
        //                object argIndex2 = k;
        //                {
        //                    var withBlock1 = withBlock.Item(argIndex2);
        //                    string argfname1 = "非表示";
        //                    if ((withBlock1.Class_Renamed() != "固定" | !withBlock1.IsFeatureAvailable(argfname1)) & withBlock1.Part() != "非表示")
        //                    {
        //                        GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + withBlock1.Nickname() + " ";
        //                    }
        //                }
        //            }

        //            // ユニットＩＤを記録しておく
        //            id_list[Information.UBound(list)] = withBlock.ID;
        //        }

        //    NextLoop:
        //        ;
        //    }

        //    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];

        //    // リストをユニットのＨＰでソート
        //    key_list = new int[Information.UBound(list) + 1];
        //    {
        //        var withBlock2 = SRC.UList;
        //        var loopTo2 = Information.UBound(list);
        //        for (i = 1; i <= loopTo2; i++)
        //        {
        //            Unit localItem() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //            key_list[i] = localItem().MaxHP;
        //        }
        //    }

        //    var loopTo3 = (Information.UBound(list) - 1);
        //    for (i = 1; i <= loopTo3; i++)
        //    {
        //        max_item = i;
        //        max_value = key_list[i];
        //        var loopTo4 = Information.UBound(list);
        //        for (j = (i + 1); j <= loopTo4; j++)
        //        {
        //            if (key_list[j] > max_value)
        //            {
        //                max_item = j;
        //                max_value = key_list[j];
        //            }
        //        }

        //        if (max_item != i)
        //        {
        //            buf = list[i];
        //            list[i] = list[max_item];
        //            list[max_item] = buf;
        //            buf = id_list[i];
        //            id_list[i] = id_list[max_item];
        //            id_list[max_item] = buf;
        //            buf = GUI.ListItemComment[i];
        //            GUI.ListItemComment[i] = GUI.ListItemComment[max_item];
        //            GUI.ListItemComment[max_item] = buf;
        //            key_list[max_item] = key_list[i];
        //        }
        //    }

        //    // 換装するユニットを選択
        //    GUI.TopItem = top_item;
        //    string argoname2 = "等身大基準";
        //    if (Expression.IsOptionDefined(argoname2))
        //    {
        //        string arglb_caption = "ユニット選択";
        //        string argtname = "ランク";
        //        Unit argu = null;
        //        string argtname1 = "ＨＰ";
        //        Unit argu1 = null;
        //        string argtname2 = "ＥＮ";
        //        Unit argu2 = null;
        //        string argtname3 = "装甲";
        //        Unit argu3 = null;
        //        string argtname4 = "運動";
        //        Unit argu4 = null;
        //        string arglb_info = "ユニット                         " + Expression.Term(argtname, argu, 6) + "   " + Expression.Term(argtname1, argu1, 4) + " " + Expression.Term(argtname2, argu2, 4) + " " + Expression.Term(argtname3, argu3, 4) + " " + Expression.Term(argtname4, argu4, 4) + " レベル";
        //        string arglb_mode = "連続表示,コメント";
        //        ret = GUI.ListBox(arglb_caption, list, arglb_info, arglb_mode);
        //    }
        //    else
        //    {
        //        string arglb_caption1 = "ユニット選択";
        //        string argtname5 = "ランク";
        //        Unit argu5 = null;
        //        string argtname6 = "ＨＰ";
        //        Unit argu6 = null;
        //        string argtname7 = "ＥＮ";
        //        Unit argu7 = null;
        //        string argtname8 = "装甲";
        //        Unit argu8 = null;
        //        string argtname9 = "運動";
        //        Unit argu9 = null;
        //        string arglb_info1 = "ユニット                     " + Expression.Term(argtname5, argu5, 6) + "   " + Expression.Term(argtname6, argu6, 4) + " " + Expression.Term(argtname7, argu7, 4) + " " + Expression.Term(argtname8, argu8, 4) + " " + Expression.Term(argtname9, argu9, 4) + " パイロット";
        //        string arglb_mode1 = "連続表示,コメント";
        //        ret = GUI.ListBox(arglb_caption1, list, arglb_info1, arglb_mode1);
        //    }

        //    top_item = GUI.TopItem;

        //    // キャンセル？
        //    if (ret == 0)
        //    {
        //        return;
        //    }

        //    // 選択されたユニットを検索
        //    var tmp = id_list;
        //    object argIndex3 = tmp[ret];
        //    u = SRC.UList.Item(argIndex3);

        //    // 換装可能な形態のリストを作成
        //    {
        //        var withBlock3 = u;
        //        object argIndex4 = "換装";
        //        buf = withBlock3.FeatureData(argIndex4);
        //        list2 = new string[1];
        //        id_list2 = new string[1];
        //        GUI.ListItemComment = new string[1];
        //        var loopTo5 = GeneralLib.LLength(buf);
        //        for (i = 1; i <= loopTo5; i++)
        //        {
        //            object argIndex8 = GeneralLib.LIndex(buf, i);
        //            {
        //                var withBlock4 = withBlock3.OtherForm(argIndex8);
        //                if (withBlock4.IsAvailable())
        //                {
        //                    Array.Resize(list2, Information.UBound(list2) + 1 + 1);
        //                    Array.Resize(id_list2, Information.UBound(list2) + 1);
        //                    Array.Resize(GUI.ListItemComment, Information.UBound(list2) + 1);

        //                    // ユニットランクを合わせる
        //                    withBlock4.Rank = u.Rank;
        //                    withBlock4.BossRank = u.BossRank;
        //                    withBlock4.Update();

        //                    // 換装先のリストを作成
        //                    id_list2[Information.UBound(list2)] = withBlock4.Name;
        //                    if ((u.Nickname0 ?? "") == (withBlock4.Nickname ?? ""))
        //                    {
        //                        string argbuf = withBlock4.Name;
        //                        list2[Information.UBound(list2)] = GeneralLib.RightPaddedString(argbuf, 27);
        //                        withBlock4.Name = argbuf;
        //                    }
        //                    else
        //                    {
        //                        string argbuf1 = withBlock4.Nickname0;
        //                        list2[Information.UBound(list2)] = GeneralLib.RightPaddedString(argbuf1, 27);
        //                        withBlock4.Nickname0 = argbuf1;
        //                    }

        //                    string localLeftPaddedString5() { string argbuf = SrcFormatter.Format(withBlock4.MaxHP); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

        //                    string localLeftPaddedString6() { string argbuf = SrcFormatter.Format(withBlock4.MaxEN); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //                    string localLeftPaddedString7() { string argbuf = SrcFormatter.Format(withBlock4.get_Armor("")); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //                    string localLeftPaddedString8() { string argbuf = SrcFormatter.Format(withBlock4.get_Mobility("")); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //                    list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + localLeftPaddedString5() + localLeftPaddedString6() + localLeftPaddedString7() + localLeftPaddedString8() + " " + withBlock4.Data.Adaption;

        //                    // 最大攻撃力
        //                    max_value = 0;
        //                    var loopTo6 = withBlock4.CountWeapon();
        //                    for (j = 1; j <= loopTo6; j++)
        //                    {
        //                        string argattr = "合";
        //                        if (withBlock4.IsWeaponMastered(j) & !withBlock4.IsDisabled(withBlock4.Weapon(j).Name) & !withBlock4.IsWeaponClassifiedAs(j, argattr))
        //                        {
        //                            string argtarea1 = "";
        //                            if (withBlock4.WeaponPower(j, argtarea1) > max_value)
        //                            {
        //                                string argtarea = "";
        //                                max_value = withBlock4.WeaponPower(j, argtarea);
        //                            }
        //                        }
        //                    }

        //                    string localLeftPaddedString9() { string argbuf = SrcFormatter.Format(max_value); var ret = GeneralLib.LeftPaddedString(argbuf, 7); return ret; }

        //                    list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + localLeftPaddedString9();

        //                    // 最大射程
        //                    max_value = 0;
        //                    var loopTo7 = withBlock4.CountWeapon();
        //                    for (j = 1; j <= loopTo7; j++)
        //                    {
        //                        string argattr1 = "合";
        //                        if (withBlock4.IsWeaponMastered(j) & !withBlock4.IsDisabled(withBlock4.Weapon(j).Name) & !withBlock4.IsWeaponClassifiedAs(j, argattr1))
        //                        {
        //                            if (withBlock4.WeaponMaxRange(j) > max_value)
        //                            {
        //                                max_value = withBlock4.WeaponMaxRange(j);
        //                            }
        //                        }
        //                    }

        //                    string localLeftPaddedString10() { string argbuf = SrcFormatter.Format(max_value); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //                    list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + localLeftPaddedString10();

        //                    // 換装先が持つ特殊能力一覧
        //                    farray = new string[1];
        //                    var loopTo8 = withBlock4.CountFeature();
        //                    for (j = 1; j <= loopTo8; j++)
        //                    {
        //                        object argIndex7 = j;
        //                        if (!string.IsNullOrEmpty(withBlock4.FeatureName(argIndex7)))
        //                        {
        //                            // 重複する特殊能力は表示しないようチェック
        //                            var loopTo9 = Information.UBound(farray);
        //                            for (k = 1; k <= loopTo9; k++)
        //                            {
        //                                object argIndex5 = j;
        //                                if ((withBlock4.FeatureName(argIndex5) ?? "") == (farray[k] ?? ""))
        //                                {
        //                                    break;
        //                                }
        //                            }

        //                            if (k > Information.UBound(farray))
        //                            {
        //                                string localFeatureName() { object argIndex1 = j; var ret = withBlock4.FeatureName(argIndex1); return ret; }

        //                                GUI.ListItemComment[Information.UBound(list2)] = GUI.ListItemComment[Information.UBound(list2)] + localFeatureName() + " ";
        //                                Array.Resize(farray, Information.UBound(farray) + 1 + 1);
        //                                object argIndex6 = j;
        //                                farray[Information.UBound(farray)] = withBlock4.FeatureName(argIndex6);
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        GUI.ListItemFlag = new bool[Information.UBound(list2) + 1];

        //        // 換装先の形態を選択
        //        GUI.TopItem = 1;
        //        string arglb_caption2 = "変更先選択";
        //        string argtname10 = "ＨＰ";
        //        string argtname11 = "ＥＮ";
        //        string argtname12 = "装甲";
        //        string argtname13 = "運動";
        //        string arglb_info2 = "ユニット                     " + Expression.Term(argtname10, u, 4) + " " + Expression.Term(argtname11, u, 4) + " " + Expression.Term(argtname12, u, 4) + " " + Expression.Term(argtname13, u, 4) + " 適応 攻撃力 射程";
        //        string arglb_mode2 = "連続表示,コメント";
        //        ret = GUI.ListBox(arglb_caption2, list2, arglb_info2, arglb_mode2);

        //        // キャンセル？
        //        if (ret == 0)
        //        {
        //            goto Beginning;
        //        }

        //        // 換装を実施
        //        withBlock3.Transform(id_list2[ret]);
        //    }

        //    goto Beginning;
        //}

        //// ステータスコマンド中かどうかを返す
        //public bool InStatusCommand()
        //{
        //    bool InStatusCommandRet = default;
        //    if (string.IsNullOrEmpty(Map.MapFileName))
        //    {
        //        if (Strings.InStr(SRC.ScenarioFileName, @"\ユニットステータス表示.eve") > 0 | Strings.InStr(SRC.ScenarioFileName, @"\パイロットステータス表示.eve") > 0 | SRC.IsSubStage)
        //        {
        //            InStatusCommandRet = true;
        //        }
        //    }

        //    return InStatusCommandRet;
        //}
    }
}