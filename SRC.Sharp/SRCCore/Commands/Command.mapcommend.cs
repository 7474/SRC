// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using Newtonsoft.Json;
using SRCCore.VB;
using System;
using System.Linq;

namespace SRCCore.Commands
{
    public partial class Command
    {
        // マップコマンド実行
        public void MapCommand(UiCommand command)
        {
            LogDebug();

            CommandState = "ユニット選択";
            switch (command.Id)
            {
                case EndTurnCmdID: // ターン終了
                    if (ViewMode)
                    {
                        ViewMode = false;
                        return;
                    }
                    EndTurnCommand();
                    break;

                //case DumpCmdID: // 中断
                //    DumpCommand();
                //    break;

                //case UnitListCmdID: // 部隊表
                //    UnitListCommand();
                //    break;

                //case SearchSpecialPowerCmdID: // スペシャルパワー検索
                //    SearchSpecialPowerCommand();
                //    break;

                //case GlobalMapCmdID: // 全体マップ
                //    GlobalMapCommand();
                //    break;

                case OperationObjectCmdID: // 作戦目的
                    GUI.LockGUI();
                    Event.HandleEvent("勝利条件");
                    GUI.RedrawScreen();
                    GUI.UnlockGUI();
                    break;

                case MapCommandCmdID: // マップコマンド
                    GUI.LockGUI();
                    Event.HandleEvent("" + command.LabelData.EventDataId);
                    GUI.UnlockGUI();
                    break;

                //case AutoDefenseCmdID: // 自動反撃モード
                //GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = !GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked;
                //if (GUI.MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked)
                //{
                //    GeneralLib.WriteIni("Option", "AutoDefense", "On");
                //}
                //else
                //{
                //    GeneralLib.WriteIni("Option", "AutoDefense", "Off");
                //}
                //break;

                case ConfigurationCmdID: // 設定変更
                    GUI.Configure();
                    break;

                //case RestartCmdID: // リスタート
                //    RestartCommand();
                //    break;

                //case QuickLoadCmdID: // クイックロード
                //    QuickLoadCommand();
                //    break;

                //case QuickSaveCmdID: // クイックセーブ
                //    QuickSaveCommand();
                //    break;

                default:
                    throw new NotSupportedException(JsonConvert.SerializeObject(command));
            }

            SRC.IsScenarioFinished = false;
        }

        // 「ターン終了」コマンド
        private void EndTurnCommand()
        {
            // 行動していない味方ユニットの数を数える
            var stillActionUnits = SRC.UList.Items.Where(u => u.Party == "味方" && u.CanAction).ToList();

            // 行動していないユニットがいれば警告
            if (stillActionUnits.Any())
            {
                var ret = GUI.Confirm(
                    "行動していないユニットが" + SrcFormatter.Format(stillActionUnits.Count) + "体あります"
                    + Environment.NewLine
                    + "このターンを終了しますか？",
                    "",
                    GuiConfirmOption.OkCancel | GuiConfirmOption.Question
                    );
                if (ret == GuiDialogResult.Cancel)
                {
                    return;
                }
            }

            // 行動終了していないユニットに対して行動終了イベントを実施
            foreach (var currentSelectedUnit in stillActionUnits)
            {
                SelectedUnit = currentSelectedUnit;
                Event.HandleEvent("行動終了", currentSelectedUnit.MainPilot().ID);
                if (SRC.IsScenarioFinished)
                {
                    SRC.IsScenarioFinished = false;
                    return;
                }
            }

            // 各陣営のフェイズに移行
            SRC.StartTurn("敵");
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            SRC.StartTurn("中立");
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            SRC.StartTurn("ＮＰＣ");
            if (SRC.IsScenarioFinished)
            {
                SRC.IsScenarioFinished = false;
                return;
            }

            // 味方フェイズに戻る
            SRC.StartTurn("味方");
            SRC.IsScenarioFinished = false;
        }

        //// ユニット一覧の表示
        //// MOD START MARGE
        //// Public Sub UnitListCommand()
        //private void UnitListCommand()
        //{
        //    // MOD END MARGE
        //    string[] list;
        //    string[] id_list;
        //    int j, i, ret;
        //    string uparty, sort_mode;
        //    string[] mode_list;
        //    int[] key_list;
        //    string[] strkey_list;
        //    int max_item;
        //    int max_value;
        //    string max_str;
        //    string buf;
        //    Unit u;
        //    var pilot_status_mode = default(bool);
        //    GUI.LockGUI();
        //    GUI.TopItem = 1;
        //    GUI.EnlargeListBoxHeight();
        //    GUI.ReduceListBoxWidth();

        //    // デフォルトのソート方法
        //    uparty = "味方";
        //    sort_mode = "レベル";
        //Beginning:
        //    ;


        //    // ユニット一覧のリストを作成
        //    list = new string[2];
        //    id_list = new string[2];
        //    list[1] = "▽陣営変更・並べ替え▽";
        //    foreach (Unit currentU in SRC.UList)
        //    {
        //        u = currentU;
        //        {
        //            var withBlock = u;
        //            if ((withBlock.Party0 ?? "") == (uparty ?? "") & (withBlock.Status_Renamed == "出撃" | withBlock.Status_Renamed == "格納"))
        //            {
        //                // 未確認のユニットは表示しない
        //                if (Expression.IsOptionDefined("ユニット情報隠蔽") & !withBlock.IsConditionSatisfied("識別済み") & (withBlock.Party0 == "敵" | withBlock.Party0 == "中立") | withBlock.IsConditionSatisfied("ユニット情報隠蔽"))
        //                {
        //                    goto NextUnit;
        //                }

        //                Array.Resize(list, Information.UBound(list) + 1 + 1);
        //                Array.Resize(id_list, Information.UBound(list) + 1);
        //                Array.Resize(GUI.ListItemFlag, Information.UBound(list) + 1);
        //                if (!withBlock.IsFeatureAvailable("ダミーユニット"))
        //                {
        //                    // 通常のユニット表示
        //                    if (Expression.IsOptionDefined("等身大基準"))
        //                    {
        //                        // 等身大基準を使った場合のユニット表示
        //                        string localRightPaddedString() { string argbuf = withBlock.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 33); withBlock.Nickname0 = argbuf; return ret; }

        //                        string localLeftPaddedString() { string argbuf = SrcFormatter.Format(withBlock.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

        //                        list[Information.UBound(list)] = localRightPaddedString() + localLeftPaddedString() + " ";
        //                    }
        //                    else
        //                    {
        //                        list[Information.UBound(list)] = GeneralLib.RightPaddedString(withBlock.Nickname0, 23);
        //                        withBlock.Nickname0 = argbuf;
        //                        if (withBlock.MainPilot().Nickname0 == "パイロット不在")
        //                        {
        //                            // パイロットが乗っていない場合
        //                            list[Information.UBound(list)] = GeneralLib.RightPaddedString(list[Information.UBound(list)] + "", 34) + GeneralLib.LeftPaddedString("", 2);
        //                        }
        //                        else
        //                        {
        //                            string localLeftPaddedString1() { string argbuf = SrcFormatter.Format(withBlock.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

        //                            list[Information.UBound(list)] = GeneralLib.RightPaddedString(list[Information.UBound(list)] + withBlock.MainPilot().get_Nickname(false), 34) + localLeftPaddedString1();
        //                        }

        //                        list[Information.UBound(list)] = GeneralLib.RightPaddedString(list[Information.UBound(list)], 37);
        //                    }

        //                    if (withBlock.IsConditionSatisfied("データ不明"))
        //                    {
        //                        list[Information.UBound(list)] = list[Information.UBound(list)] + "?????/????? ???/???";
        //                    }
        //                    else
        //                    {
        //                        string localLeftPaddedString2() { string argbuf = SrcFormatter.Format(withBlock.HP); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //                        string localLeftPaddedString3() { string argbuf = SrcFormatter.Format(withBlock.MaxHP); var ret = GeneralLib.LeftPaddedString(argbuf, 5); return ret; }

        //                        string localLeftPaddedString4() { string argbuf = SrcFormatter.Format(withBlock.EN); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

        //                        string localLeftPaddedString5() { string argbuf = SrcFormatter.Format(withBlock.MaxEN); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

        //                        list[Information.UBound(list)] = list[Information.UBound(list)] + localLeftPaddedString2() + "/" + localLeftPaddedString3() + " " + localLeftPaddedString4() + "/" + localLeftPaddedString5();
        //                    }
        //                }
        //                else
        //                {
        //                    // パイロットステータス表示時
        //                    pilot_status_mode = true;
        //                    {
        //                        var withBlock1 = withBlock.MainPilot();
        //                        string localRightPaddedString1() { string argbuf = withBlock1.get_Nickname(false); var ret = GeneralLib.RightPaddedString(argbuf, 21); withBlock1.get_Nickname(false) = argbuf; return ret; }

        //                        string localLeftPaddedString6() { string argbuf = SrcFormatter.Format(withBlock1.Level); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

        //                        string localLeftPaddedString7() { string argbuf = SrcFormatter.Format(withBlock1.SP) + "/" + SrcFormatter.Format(withBlock1.MaxSP); var ret = GeneralLib.LeftPaddedString(argbuf, 9); return ret; }

        //                        list[Information.UBound(list)] = localRightPaddedString1() + localLeftPaddedString6() + localLeftPaddedString7() + "  ";
        //                        // 使用可能なスペシャルパワー一覧
        //                        var loopTo = withBlock1.CountSpecialPower;
        //                        for (i = 1; i <= loopTo; i++)
        //                        {
        //                            int localSpecialPowerCost() { string argsname = withBlock1.get_SpecialPower(i); var ret = withBlock1.SpecialPowerCost(argsname); withBlock1.get_SpecialPower(i) = argsname; return ret; }

        //                            if (withBlock1.SP >= localSpecialPowerCost())
        //                            {
        //                                SpecialPowerData localItem() { object argIndex1 = withBlock1.get_SpecialPower(i); var ret = SRC.SPDList.Item(argIndex1); withBlock1.get_SpecialPower(i) = Conversions.ToString(argIndex1); return ret; }

        //                                list[Information.UBound(list)] = list[Information.UBound(list)] + localItem().intName;
        //                            }
        //                        }
        //                    }
        //                }

        //                if (withBlock.Action == 0)
        //                {
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + "済";
        //                }

        //                if (withBlock.Status_Renamed == "格納")
        //                {
        //                    list[Information.UBound(list)] = list[Information.UBound(list)] + "格";
        //                }

        //                id_list[Information.UBound(list)] = withBlock.ID;
        //                GUI.ListItemFlag[Information.UBound(list)] = false;
        //            }
        //        }

        //    NextUnit:
        //        ;
        //    }

        //SortList:
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
        //                            Unit localItem1() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem1().HP;
        //                        }

        //                        break;
        //                    }

        //                case "ＥＮ":
        //                    {
        //                        var loopTo2 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo2; i++)
        //                        {
        //                            Unit localItem2() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock2.Item(argIndex1); return ret; }

        //                            key_list[i] = localItem2().EN;
        //                        }

        //                        break;
        //                    }

        //                case "レベル":
        //                case "パイロットレベル":
        //                    {
        //                        var loopTo3 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo3; i++)
        //                        {
        //                            var tmp = id_list;
        //                            {
        //                                var withBlock3 = withBlock2.Item(tmp[i]);
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
        //                key_list[max_item] = key_list[i];
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // 文字列を使ったソート

        //        // まず並べ替えに使うキーのリストを作成
        //        strkey_list = new string[Information.UBound(list) + 1];
        //        {
        //            var withBlock5 = SRC.UList;
        //            switch (sort_mode ?? "")
        //            {
        //                case "名称":
        //                case "ユニット名称":
        //                    {
        //                        var loopTo6 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo6; i++)
        //                        {
        //                            Unit localItem3() { var tmp = id_list; object argIndex1 = tmp[i]; var ret = withBlock5.Item(argIndex1); return ret; }

        //                            strkey_list[i] = localItem3().KanaName;
        //                        }

        //                        break;
        //                    }

        //                case "パイロット名称":
        //                    {
        //                        var loopTo7 = Information.UBound(list);
        //                        for (i = 2; i <= loopTo7; i++)
        //                        {
        //                            var tmp1 = id_list;
        //                            {
        //                                var withBlock6 = withBlock5.Item(tmp1[i]);
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
        //        var loopTo8 = (Information.UBound(strkey_list) - 1);
        //        for (i = 2; i <= loopTo8; i++)
        //        {
        //            max_item = i;
        //            max_str = strkey_list[i];
        //            var loopTo9 = Information.UBound(strkey_list);
        //            for (j = (i + 1); j <= loopTo9; j++)
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
        //                strkey_list[max_item] = strkey_list[i];
        //            }
        //        }
        //    }

        //    GUI.ListItemFlag = new bool[1];
        //    GUI.ListItemID = new string[Information.UBound(list) + 1];
        //    var loopTo10 = Information.UBound(list);
        //    for (i = 1; i <= loopTo10; i++)
        //        GUI.ListItemID[i] = id_list[i];

        //    // リストを表示
        //    if (pilot_status_mode)
        //    {
        //        ret = GUI.ListBox(uparty + "パイロット一覧", list, "パイロット名       レベル    " + Expression.Term(argtname, argu, 4) + "  " + Expression.Term(argtname1, u: argu1), "連続表示");
        //    }
        //    else if (Expression.IsOptionDefined("等身大基準"))
        //    {
        //        ret = GUI.ListBox(uparty + "ユニット一覧", list, "ユニット名                        Lv     " + Expression.Term(argtname4, argu4, 8) + Expression.Term(argtname5, u: argu5), "連続表示");
        //    }
        //    else
        //    {
        //        ret = GUI.ListBox(uparty + "ユニット一覧", list, "ユニット               パイロット Lv     " + Expression.Term(argtname2, argu2, 8) + Expression.Term(argtname3, u: argu3), "連続表示");
        //    }

        //    switch (ret)
        //    {
        //        case 0:
        //            {
        //                // キャンセル
        //                My.MyProject.Forms.frmListBox.Hide();
        //                GUI.ReduceListBoxHeight();
        //                GUI.EnlargeListBoxWidth();
        //                GUI.ListItemID = new string[1];
        //                GUI.UnlockGUI();
        //                return;
        //            }

        //        case 1:
        //            {
        //                // 表示する陣営
        //                mode_list = new string[5];
        //                mode_list[1] = "味方一覧";
        //                mode_list[2] = "ＮＰＣ一覧";
        //                mode_list[3] = "敵一覧";
        //                mode_list[4] = "中立一覧";

        //                // ソート方法を選択
        //                if (pilot_status_mode)
        //                {
        //                    Array.Resize(mode_list, 8);
        //                    mode_list[5] = "パイロット名称で並べ替え";
        //                    mode_list[6] = "レベルで並べ替え";
        //                    mode_list[7] = Expression.Term("ＳＰ"6, u: null6) + "で並べ替え";
        //                }
        //                else if (Expression.IsOptionDefined("等身大基準"))
        //                {
        //                    Array.Resize(mode_list, 9);
        //                    mode_list[5] = "名称で並べ替え";
        //                    mode_list[6] = "レベルで並べ替え";
        //                    mode_list[7] = Expression.Term("ＨＰ", u: null) + "で並べ替え";
        //                    mode_list[8] = Expression.Term("スペシャルパワー"0, u: null) + "で並べ替え";
        //                }
        //                else
        //                {
        //                    Array.Resize(mode_list, 10);
        //                    mode_list[5] = Expression.Term("ＨＰ", u: null) + "で並べ替え";
        //                    mode_list[6] = Expression.Term("ＥＮ", u: null) + "で並べ替え";
        //                    mode_list[7] = "パイロットレベルで並べ替え";
        //                    mode_list[8] = "ユニット名称で並べ替え";
        //                    mode_list[9] = "パイロット名称で並べ替え";
        //                }

        //                GUI.ListItemID = new string[Information.UBound(mode_list) + 1];
        //                GUI.ListItemFlag = new bool[Information.UBound(mode_list) + 1];
        //                ret = GUI.ListBox("選択", mode_list, "一覧表示方法", "連続表示");

        //                // 陣営を変更して再表示
        //                if (ret > 0)
        //                {
        //                    if (Strings.Right(mode_list[ret], 2) == "一覧")
        //                    {
        //                        uparty = Strings.Left(mode_list[ret], Strings.Len(mode_list[ret]) - 2);
        //                        goto Beginning;
        //                    }
        //                    else if (Strings.Right(mode_list[ret], 5) == "で並べ替え")
        //                    {
        //                        sort_mode = Strings.Left(mode_list[ret], Strings.Len(mode_list[ret]) - 5);
        //                        goto SortList;
        //                    }
        //                }

        //                goto SortList;
        //                break;
        //            }
        //    }

        //    My.MyProject.Forms.frmListBox.Hide();
        //    GUI.ReduceListBoxHeight();
        //    GUI.EnlargeListBoxWidth();

        //    // 選択されたユニットを画面中央に表示
        //    var tmp2 = GUI.ListItemID;
        //    u = SRC.UList.Item(tmp2[ret]);
        //    GUI.Center(u.x, u.y);
        //    GUI.RefreshScreen();
        //    Status.DisplayUnitStatus(u);

        //    // カーソル自動移動
        //    if (SRC.AutoMoveCursor)
        //    {
        //        GUI.MoveCursorPos("ユニット選択", u);
        //    }

        //    GUI.ListItemID = new string[1];
        //    GUI.UnlockGUI();
        //}

        //// スペシャルパワー検索コマンド
        //// MOD START MARGE
        //// Public Sub SearchSpecialPowerCommand()
        //private void SearchSpecialPowerCommand()
        //{
        //    // MOD END MARGE
        //    int j, i, ret;
        //    string[] list;
        //    string[] list2;
        //    bool[] flist;
        //    string[] pid;
        //    string buf;
        //    Pilot p;
        //    string[] id_list;
        //    var strkey_list = default(string[]);
        //    int max_item;
        //    string max_str;
        //    bool found;
        //    GUI.LockGUI();

        //    // イベント専用のコマンドを除いた全スペシャルパワーのリストを作成
        //    list = new string[1];
        //    var loopTo = SRC.SPDList.Count();
        //    for (i = 1; i <= loopTo; i++)
        //    {
        //        {
        //            var withBlock = SRC.SPDList.Item(i);
        //            if (withBlock.intName != "非表示")
        //            {
        //                Array.Resize(list, Information.UBound(list) + 1 + 1);
        //                Array.Resize(strkey_list, Information.UBound(list) + 1);
        //                list[Information.UBound(list)] = withBlock.Name;
        //                strkey_list[Information.UBound(list)] = withBlock.KanaName;
        //            }
        //        }
        //    }

        //    // ソート
        //    var loopTo1 = (Information.UBound(strkey_list) - 1);
        //    for (i = 1; i <= loopTo1; i++)
        //    {
        //        max_item = i;
        //        max_str = strkey_list[i];
        //        var loopTo2 = Information.UBound(strkey_list);
        //        for (j = (i + 1); j <= loopTo2; j++)
        //        {
        //            if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
        //            {
        //                max_item = j;
        //                max_str = strkey_list[j];
        //            }
        //        }

        //        if (max_item != i)
        //        {
        //            buf = list[i];
        //            list[i] = list[max_item];
        //            list[max_item] = buf;
        //            buf = strkey_list[i];
        //            strkey_list[i] = max_str;
        //            strkey_list[max_item] = buf;
        //        }
        //    }

        //    // 個々のスペシャルパワーに対して、そのスペシャルパワーを使用可能なパイロットが
        //    // いるかどうか判定
        //    flist = new bool[Information.UBound(list) + 1];
        //    var loopTo3 = Information.UBound(list);
        //    for (i = 1; i <= loopTo3; i++)
        //    {
        //        flist[i] = true;
        //        foreach (Pilot currentP in SRC.PList)
        //        {
        //            p = currentP;
        //            if (p.Party == "味方")
        //            {
        //                if (p.Unit_Renamed is object)
        //                {
        //                    if (p.Unit_Renamed.Status_Renamed == "出撃" & !p.Unit_Renamed.IsConditionSatisfied("憑依"))
        //                    {
        //                        // 本当に乗っている？
        //                        found = false;
        //                        {
        //                            var withBlock1 = p.Unit_Renamed;
        //                            if (ReferenceEquals(p, withBlock1.MainPilot()))
        //                            {
        //                                found = true;
        //                            }
        //                            else
        //                            {
        //                                var loopTo4 = withBlock1.CountPilot();
        //                                for (j = 2; j <= loopTo4; j++)
        //                                {
        //                                    Pilot localPilot() { object argIndex1 = j; var ret = withBlock1.Pilot(argIndex1); return ret; }

        //                                    if (ReferenceEquals(p, localPilot()))
        //                                    {
        //                                        found = true;
        //                                        break;
        //                                    }
        //                                }

        //                                var loopTo5 = withBlock1.CountSupport();
        //                                for (j = 1; j <= loopTo5; j++)
        //                                {
        //                                    Pilot localSupport() { object argIndex1 = j; var ret = withBlock1.Support(argIndex1); return ret; }

        //                                    if (ReferenceEquals(p, localSupport()))
        //                                    {
        //                                        found = true;
        //                                        break;
        //                                    }
        //                                }

        //                                if (ReferenceEquals(p, withBlock1.AdditionalSupport()))
        //                                {
        //                                    found = true;
        //                                }
        //                            }
        //                        }

        //                        if (found)
        //                        {
        //                            if (p.IsSpecialPowerAvailable(list[i]))
        //                            {
        //                                flist[i] = false;
        //                                break;
        //                            }
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    while (true)
        //    {
        //        GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
        //        GUI.ListItemComment = new string[Information.UBound(list) + 1];
        //        id_list = new string[Information.UBound(list) + 1];
        //        strkey_list = new string[Information.UBound(list) + 1];

        //        // 選択出来ないスペシャルパワーをマスク
        //        var loopTo6 = Information.UBound(GUI.ListItemFlag);
        //        for (i = 1; i <= loopTo6; i++)
        //            GUI.ListItemFlag[i] = flist[i];

        //        // スペシャルパワーの解説を設定
        //        var loopTo7 = Information.UBound(GUI.ListItemComment);
        //        for (i = 1; i <= loopTo7; i++)
        //        {
        //            SpecialPowerData localItem() { var tmp = list; object argIndex1 = tmp[i]; var ret = SRC.SPDList.Item(argIndex1); return ret; }

        //            GUI.ListItemComment[i] = localItem().Comment;
        //        }

        //        // 検索するスペシャルパワーを選択
        //        GUI.TopItem = 1;
        //        ret = GUI.MultiColumnListBox(Expression.Term("スペシャルパワー", u: null) + "検索", list, true);
        //        if (ret == 0)
        //        {
        //            CancelCommand();
        //            GUI.UnlockGUI();
        //            return;
        //        }

        //        SelectedSpecialPower = list[ret];

        //        // 選択されたスペシャルパワーを使用できるパイロットの一覧を作成
        //        list2 = new string[1];
        //        GUI.ListItemFlag = new bool[1];
        //        id_list = new string[1];
        //        pid = new string[1];
        //        foreach (Pilot currentP1 in SRC.PList)
        //        {
        //            p = currentP1;
        //            // 選択したスペシャルパワーを使用できるパイロットかどうか判定
        //            if (p.Party != "味方")
        //            {
        //                goto NextLoop;
        //            }

        //            if (p.Unit_Renamed is null)
        //            {
        //                goto NextLoop;
        //            }

        //            if (p.Unit_Renamed.Status_Renamed != "出撃")
        //            {
        //                goto NextLoop;
        //            }

        //            if (p.Unit_Renamed.CountPilot() > 0)
        //            {
        //                if ((p.ID ?? "") == (p.Unit_Renamed.Pilot(1).ID ?? "") & (p.ID ?? "") != (p.Unit_Renamed.MainPilot().ID ?? ""))
        //                {
        //                    // 追加パイロットのため、使用されていない
        //                    goto NextLoop;
        //                }
        //            }

        //            if (!p.IsSpecialPowerAvailable(SelectedSpecialPower))
        //            {
        //                goto NextLoop;
        //            }

        //            // パイロットをリストに追加
        //            Array.Resize(list2, Information.UBound(list2) + 1 + 1);
        //            Array.Resize(GUI.ListItemFlag, Information.UBound(list2) + 1);
        //            Array.Resize(id_list, Information.UBound(list2) + 1);
        //            Array.Resize(pid, Information.UBound(list2) + 1);
        //            GUI.ListItemFlag[Information.UBound(list2)] = false;
        //            id_list[Information.UBound(list2)] = p.Unit_Renamed.ID;
        //            pid[Information.UBound(list2)] = p.ID;
        //            string localRightPaddedString() { string argbuf = p.get_Nickname(false); var ret = GeneralLib.RightPaddedString(argbuf, 19); p.get_Nickname(false) = argbuf; return ret; }

        //            string localRightPaddedString1() { string argbuf = SrcFormatter.Format(p.SP) + "/" + SrcFormatter.Format(p.MaxSP); var ret = GeneralLib.RightPaddedString(argbuf, 10); return ret; }

        //            list2[Information.UBound(list2)] = localRightPaddedString() + localRightPaddedString1();
        //            buf = "";
        //            var loopTo8 = p.CountSpecialPower;
        //            for (j = 1; j <= loopTo8; j++)
        //            {
        //                SpecialPowerData localItem1() { object argIndex1 = p.get_SpecialPower(j); var ret = SRC.SPDList.Item(argIndex1); p.get_SpecialPower(j) = Conversions.ToString(argIndex1); return ret; }

        //                buf = buf + localItem1().intName;
        //            }

        //            list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + GeneralLib.RightPaddedString(buf, 12);
        //            if (p.SP < p.SpecialPowerCost(SelectedSpecialPower))
        //            {
        //                list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + " " + Expression.Term("ＳＰ", p.Unit_Renamed) + "不足";
        //            }

        //            if (p.Unit_Renamed.Action == 0)
        //            {
        //                list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + " 行動済";
        //            }

        //        NextLoop:
        //            ;
        //        }

        //        SelectedSpecialPower = "";

        //        // 検索をかけるパイロットの選択
        //        GUI.TopItem = 1;
        //        GUI.EnlargeListBoxHeight();
        //        if (Expression.IsOptionDefined("等身大基準"))
        //        {
        //            ret = GUI.ListBox("ユニット選択", list2, "ユニット           " + Expression.Term("SP", null, 2) + "/Max" + Expression.Term("SP", argu2, 2) + "  " + Expression.Term("スペシャルパワー", u: null), lb_mode: "");
        //        }
        //        else
        //        {
        //            ret = GUI.ListBox("パイロット選択", list2, "パイロット         " + Expression.Term(argtname5, argu4, 2) + "/Max" + Expression.Term("SP", argu5, 2) + "  " + Expression.Term("スペシャルパワー", u: null), lb_mode: "");
        //        }

        //        GUI.ReduceListBoxHeight();

        //        // パイロットの乗るユニットを画面中央に表示
        //        if (ret > 0)
        //        {
        //            var tmp = pid;
        //            {
        //                var withBlock2 = SRC.PList.Item(tmp[ret]);
        //                GUI.Center(withBlock2.Unit_Renamed.x, withBlock2.Unit_Renamed.y);
        //                GUI.RefreshScreen();
        //                Status.DisplayUnitStatus(withBlock2.Unit_Renamed);

        //                // カーソル自動移動
        //                if (SRC.AutoMoveCursor)
        //                {
        //                    GUI.MoveCursorPos("ユニット選択", withBlock2.Unit_Renamed);
        //                }
        //            }

        //            id_list = new string[1];
        //            GUI.UnlockGUI();
        //            return;
        //        }
        //    }
        //}

        //// リスタートコマンド
        //// MOD START MARGE
        //// Public Sub RestartCommand()
        //private void RestartCommand()
        //{
        //    // MOD END MARGE
        //    int ret;

        //    // リスタートを行うか確認
        //    ret = Interaction.MsgBox("リスタートしますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "リスタート");
        //    if (ret == MsgBoxResult.Cancel)
        //    {
        //        return;
        //    }

        //    GUI.LockGUI();
        //    SRC.RestoreData(SRC.ScenarioPath + "_リスタート.src", true);
        //    GUI.UnlockGUI();
        //}

        //// クイックロードコマンド
        //// MOD START MARGE
        //// Public Sub QuickLoadCommand()
        //private void QuickLoadCommand()
        //{
        //    // MOD END MARGE
        //    int ret;

        //    // ロードを行うか確認
        //    ret = Interaction.MsgBox("データをロードしますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "クイックロード");
        //    if (ret == MsgBoxResult.Cancel)
        //    {
        //        return;
        //    }

        //    GUI.LockGUI();
        //    SRC.RestoreData(SRC.ScenarioPath + "_クイックセーブ.src", true);

        //    // 画面を書き直してステータスを表示
        //    GUI.RedrawScreen();
        //    Status.DisplayGlobalStatus();
        //    GUI.UnlockGUI();
        //}

        //// クイックセーブコマンド
        //// MOD START MARGE
        //// Public Sub QuickSaveCommand()
        //private void QuickSaveCommand()
        //{
        //    // MOD END MARGE

        //    GUI.LockGUI();

        //    // マウスカーソルを砂時計に
        //    // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
        //    Cursor.Current = Cursors.WaitCursor;

        //    // 中断データをセーブ
        //    SRC.DumpData(SRC.ScenarioPath + "_クイックセーブ.src");
        //    GUI.UnlockGUI();

        //    // マウスカーソルを元に戻す
        //    // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
        //    Cursor.Current = Cursors.Default;
        //}


        //// プレイを中断し、中断用データをセーブする
        //// MOD START MARGE
        //// Public Sub DumpCommand()
        //private void DumpCommand()
        //{
        //    // MOD END MARGE
        //    string fname, save_path = default;
        //    int ret, i;

        //    // プレイを中断するか確認
        //    ret = Interaction.MsgBox("プレイを中断しますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question), "中断");
        //    if (ret == MsgBoxResult.Cancel)
        //    {
        //        return;
        //    }

        //    // 中断データをセーブするファイル名を決定
        //    var loopTo = Strings.Len(SRC.ScenarioFileName);
        //    for (i = 1; i <= loopTo; i++)
        //    {
        //        if (Strings.Mid(SRC.ScenarioFileName, Strings.Len(SRC.ScenarioFileName) - i + 1, 1) == @"\")
        //        {
        //            break;
        //        }
        //    }

        //    fname = Strings.Mid(SRC.ScenarioFileName, Strings.Len(SRC.ScenarioFileName) - i + 2, i - 5);
        //    fname = fname + "を中断.src";
        //    fname = FileDialog.SaveFileDialog("データセーブ", SRC.ScenarioPath, fname, 2, "ｾｰﾌﾞﾃﾞｰﾀ", "src", ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
        //    if (string.IsNullOrEmpty(fname))
        //    {
        //        // キャンセル
        //        return;
        //    }

        //    // セーブ先はシナリオフォルダ？
        //    if (Strings.InStr(fname, @"\") > 0)
        //    {
        //        save_path = Strings.Left(fname, GeneralLib.InStr2(fname, @"\"));
        //    }
        //    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
        //    if ((FileSystem.Dir(save_path) ?? "") != (FileSystem.Dir(SRC.ScenarioPath) ?? ""))
        //    {
        //        if (Interaction.MsgBox("セーブファイルはシナリオフォルダにないと読み込めません。" + Constants.vbCr + Constants.vbLf + "このままセーブしますか？", (MsgBoxStyle)(MsgBoxStyle.OkCancel + MsgBoxStyle.Question)) != 1)
        //        {
        //            return;
        //        }
        //    }

        //    // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
        //    Cursor.Current = Cursors.WaitCursor; // マウスカーソルを砂時計に
        //    GUI.LockGUI();

        //    // 中断データをセーブ
        //    SRC.DumpData(fname);

        //    // マウスカーソルを元に戻す
        //    // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
        //    Cursor.Current = Cursors.Default;
        //    GUI.MainForm.Hide();

        //    // ゲームを終了
        //    SRC.ExitGame();
        //}


        //// 全体マップの表示
        //// MOD START MARGE
        //// Public Sub GlobalMapCommand()
        //private void GlobalMapCommand()
        //{
        //    // MOD END MARGE
        //    PictureBox pic;
        //    int xx, yy;
        //    int num = default, num2 = default;
        //    int mwidth, mheight;
        //    int ret, smode;
        //    Unit u;
        //    int i, j;
        //    bool prev_mode;
        //    GUI.LockGUI();
        //    {
        //        var withBlock = GUI.MainForm;
        //        // 見やすいように背景を設定
        //        withBlock.picMain(0).BackColor = 0xC0C0C0;
        //        withBlock.picMain(0) = Image.FromFile("");

        //        // マップの縦横の比率を元に縮小マップの大きさを決める
        //        if (Map.MapWidth > Map.MapHeight)
        //        {
        //            mwidth = 300;
        //            mheight = 300 * Map.MapHeight / Map.MapWidth;
        //        }
        //        else
        //        {
        //            mheight = 300;
        //            mwidth = 300 * Map.MapWidth / Map.MapHeight;
        //        }

        //        // マップの全体画像を作成
        //        pic = withBlock.picTmp;
        //        pic.Image = Image.FromFile("");
        //        pic.Width = SrcFormatter.TwipsToPixelsX(GUI.MapPWidth);
        //        pic.Height = SrcFormatter.TwipsToPixelsY(GUI.MapPHeight);
        //        ret = GUI.BitBlt(pic.hDC, 0, 0, GUI.MapPWidth, GUI.MapPHeight, withBlock.picBack.hDC, 0, 0, GUI.SRCCOPY);
        //        var loopTo = Map.MapWidth;
        //        for (i = 1; i <= loopTo; i++)
        //        {
        //            xx = (32 * (i - 1));
        //            var loopTo1 = Map.MapHeight;
        //            for (j = 1; j <= loopTo1; j++)
        //            {
        //                yy = (32 * (j - 1));
        //                u = Map.MapDataForUnit[i, j];
        //                if (u is object)
        //                {
        //                    if (u.BitmapID > 0)
        //                    {
        //                        if (u.Action > 0 | u.IsFeatureAvailable("地形ユニット"))
        //                        {
        //                            // ユニット
        //                            ret = GUI.BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * (u.BitmapID % 15), 96 * (u.BitmapID / 15), GUI.SRCCOPY);
        //                        }
        //                        else
        //                        {
        //                            // 行動済のユニット
        //                            ret = GUI.BitBlt(pic.hDC, xx, yy, 32, 32, withBlock.picUnitBitmap.hDC, 32 * (u.BitmapID % 15), 96 * (u.BitmapID / 15) + 32, GUI.SRCCOPY);
        //                        }

        //                        // ユニットのいる場所に合わせて表示を変更
        //                        switch (u.Area ?? "")
        //                        {
        //                            case "空中":
        //                                {
        //                                    pic.Line(xx, yy + 28); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
        //                                    break;
        //                                }

        //                            case "水中":
        //                                {
        //                                    pic.Line(xx, yy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
        //                                    break;
        //                                }

        //                            case "地中":
        //                                {
        //                                    pic.Line(xx, yy + 28); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
        //                                    pic.Line(xx, yy + 3); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
        //                                    break;
        //                                }

        //                            case "宇宙":
        //                                {
        //                                    if (Map.TerrainClass(i, j) == "月面")
        //                                    {
        //                                        pic.Line(xx, yy + 28); /* TODO ERROR: Skipped SkippedTokensTrivia *//* TODO ERROR: Skipped SkippedTokensTrivia */
        //                                    }

        //                                    break;
        //                                }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        // マップ全体を縮小して描画
        //        smode = GUI.GetStretchBltMode(withBlock.picMain(0).hDC);
        //        ret = GUI.SetStretchBltMode(withBlock.picMain(0).hDC, GUI.STRETCH_DELETESCANS);
        //        ret = GUI.StretchBlt(withBlock.picMain(0).hDC, (GUI.MainPWidth - mwidth) / 2, (GUI.MainPHeight - mheight) / 2, mwidth, mheight, pic.hDC, 0, 0, GUI.MapPWidth, GUI.MapPHeight, GUI.SRCCOPY);
        //        ret = GUI.SetStretchBltMode(withBlock.picMain(0).hDC, smode);

        //        // マップ全体画像を破棄
        //        pic.Image = Image.FromFile("");
        //        pic.Width = SrcFormatter.TwipsToPixelsX(32d);
        //        pic.Height = SrcFormatter.TwipsToPixelsY(32d);

        //        // 画面を更新
        //        withBlock.picMain(0).Refresh();
        //    }

        //    // 味方ユニット数、敵ユニット数のカウント
        //    foreach (Unit currentU in SRC.UList)
        //    {
        //        u = currentU;
        //        if (u.Status_Renamed == "出撃" | u.Status_Renamed == "格納")
        //        {
        //            if (u.Party0 == "味方" | u.Party0 == "ＮＰＣ")
        //            {
        //                num = (num + 1);
        //            }
        //            else
        //            {
        //                num2 = (num2 + 1);
        //            }
        //        }
        //    }

        //    // 各ユニット数の表示
        //    prev_mode = GUI.AutoMessageMode;
        //    GUI.AutoMessageMode = false;
        //    GUI.OpenMessageForm(u1: null, u2: null);
        //    GUI.DisplayMessage("システム", "味方ユニット： " + SrcFormatter.Format(num) + ";" + "敵ユニット  ： " + SrcFormatter.Format(num2));
        //    GUI.CloseMessageForm();
        //    GUI.AutoMessageMode = prev_mode;

        //    // 画面を元に戻す
        //    GUI.MainForm.picMain(0).BackColor = 0xFFFFFF;
        //    GUI.RefreshScreen();
        //    GUI.UnlockGUI();
        //}

    }
}
