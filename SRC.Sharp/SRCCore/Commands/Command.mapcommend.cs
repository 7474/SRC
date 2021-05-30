// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using Newtonsoft.Json;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.IO;
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

                case DumpCmdID: // 中断
                    DumpCommand();
                    break;

                case UnitListCmdID: // 部隊表
                    UnitListCommand();
                    break;

                case SearchSpecialPowerCmdID: // スペシャルパワー検索
                    SearchSpecialPowerCommand();
                    break;

                case GlobalMapCmdID: // 全体マップ
                    GlobalMapCommand();
                    break;

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

                case AutoDefenseCmdID: // 自動反撃モード
                    SystemConfig.AutoDefense = !SystemConfig.AutoDefense;
                    SystemConfig.Save();
                    break;

                case ConfigurationCmdID: // 設定変更
                    GUI.Configure();
                    break;

                case RestartCmdID: // リスタート
                    RestartCommand();
                    break;

                case QuickLoadCmdID: // クイックロード
                    QuickLoadCommand();
                    break;

                case QuickSaveCmdID: // クイックセーブ
                    QuickSaveCommand();
                    break;

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
                    "終了",
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

        // ユニット一覧の表示
        private void UnitListCommand()
        {
            GUI.LockGUI();
            GUI.TopItem = 1;
            GUI.EnlargeListBoxHeight();
            GUI.ReduceListBoxWidth();

            // デフォルトのソート方法
            var uparty = "味方";
            var sort_mode = "レベル";
            var pilot_status_mode = false;
        Beginning:
            ;

            // ユニット一覧のリストを作成
            var list = new List<ListBoxItem>();
            list.Add(new ListBoxItem("▽陣営変更・並べ替え▽", "▽陣営変更・並べ替え▽"));
            var units = SRC.UList.Items
                .Where(u => u.Party0 == uparty && (u.Status == "出撃" || u.Status == "格納"))
                .Where(u => !(Expression.IsOptionDefined("ユニット情報隠蔽")
                        && !u.IsConditionSatisfied("識別済み")
                        && (u.Party0 == "敵" || u.Party0 == "中立")
                    || u.IsConditionSatisfied("ユニット情報隠蔽")))
                .Select(u =>
                {
                    var msg = "";
                    if (!u.IsFeatureAvailable("ダミーユニット"))
                    {
                        // 通常のユニット表示
                        if (Expression.IsOptionDefined("等身大基準"))
                        {
                            // 等身大基準を使った場合のユニット表示
                            msg = GeneralLib.RightPaddedString(u.Nickname0, 33)
                            + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MainPilot().Level), 3)
                            + " ";
                        }
                        else
                        {
                            msg = GeneralLib.RightPaddedString(u.Nickname0, 23);
                            if (u.MainPilot().Nickname0 == "パイロット不在")
                            {
                                // パイロットが乗っていない場合
                                msg = GeneralLib.RightPaddedString(msg + "", 34) + GeneralLib.LeftPaddedString("", 2);
                            }
                            else
                            {
                                msg = GeneralLib.RightPaddedString(msg + u.MainPilot().get_Nickname(false), 34)
                                    + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MainPilot().Level), 2);
                            }

                            msg = GeneralLib.RightPaddedString(msg, 37);
                        }

                        if (u.IsConditionSatisfied("データ不明"))
                        {
                            msg = msg + "?????/????? ???/???";
                        }
                        else
                        {
                            msg = msg + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.HP), 5) + "/"
                                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxHP), 5) + " "
                                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.EN), 3) + "/"
                                + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxEN), 3);
                        }
                    }
                    else
                    {
                        // パイロットステータス表示時
                        pilot_status_mode = true;
                        {
                            var p = u.MainPilot();
                            msg = GeneralLib.RightPaddedString(p.get_Nickname(false), 21)
                                + GeneralLib.LeftPaddedString(SrcFormatter.Format(p.Level), 3)
                                + GeneralLib.LeftPaddedString(SrcFormatter.Format(p.SP) + "/" + SrcFormatter.Format(p.MaxSP), 9);
                            // 使用可能なスペシャルパワー一覧
                            msg += string.Join("", p.SpecialPowerNames
                                    .Where(x => p.SP > p.SpecialPowerCost(x))
                                    .Select(x => SRC.SPDList.Item(x).ShortName));
                        }
                    }

                    if (u.Action == 0)
                    {
                        msg = msg + "済";
                    }

                    if (u.Status == "格納")
                    {
                        msg = msg + "格";
                    }

                    return new ListBoxItem(msg, u.ID);
                })
                .ToList();

        SortList:
            ;

            // ソート
            if (Strings.InStr(sort_mode, "名称") == 0)
            {
                // 数値を使ったソート
                units = units.OrderByDescending(x =>
                {
                    var u = SRC.UList.Item(x.ListItemID);
                    switch (sort_mode)
                    {
                        case "ＨＰ": return u.MaxHP;
                        case "ＥＮ": return u.MaxEN;
                        case "レベル":
                        case "パイロットレベル": return u.MainPilot()?.TotalExp ?? 0;
                        default: return 0;
                    }
                }).ToList();
            }
            else
            {
                // 数値以外を使ったソート
                units = units.OrderBy(x =>
                {
                    var u = SRC.UList.Item(x.ListItemID);
                    switch (sort_mode)
                    {
                        case "名称":
                        case "ユニット名称":
                            return u.KanaName;
                        case "パイロット名称":
                            return u.MainPilot()?.KanaName ?? "";
                        default: return "";
                    }
                }).ToList();
            }

            // リストを表示
            string caption;
            string info;
            if (pilot_status_mode)
            {
                caption = uparty + "パイロット一覧";
                info = "パイロット名       レベル    " + Expression.Term("ＳＰ", null, 4) + "  " + Expression.Term("スペシャルパワー");
            }
            else if (Expression.IsOptionDefined("等身大基準"))
            {
                caption = uparty + "ユニット一覧";
                info = "ユニット名                        Lv     " + Expression.Term("ＨＰ", null, 8) + Expression.Term("ＥＮ"); ;
            }
            else
            {
                caption = uparty + "ユニット一覧";
                info = "ユニット               パイロット Lv     " + Expression.Term("ＨＰ", null, 8) + Expression.Term("ＥＮ"); ;
            }
            var ret = GUI.ListBox(new ListBoxArgs
            {
                HasFlag = true,
                Items = list.AppendRange(units).ToList(),
                lb_caption = caption,
                lb_info = info,
                lb_mode = "連続表示",
            });

            switch (ret)
            {
                case 0:
                    {
                        // キャンセル
                        GUI.CloseListBox();
                        GUI.ReduceListBoxHeight();
                        GUI.EnlargeListBoxWidth();
                        GUI.UnlockGUI();
                        return;
                    }

                case 1:
                    {
                        // 表示する陣営
                        IList<ListBoxItem> sortItems = new List<ListBoxItem>
                        {
                                new ListBoxItem("味方一覧", "味方一覧"),
                                new ListBoxItem("ＮＰＣ一覧", "ＮＰＣ一覧"),
                                new ListBoxItem("敵一覧", "敵一覧"),
                                new ListBoxItem("中立一覧", "中立一覧"),
                        };
                        if (pilot_status_mode)
                        {
                            sortItems = sortItems.AppendRange(new ListBoxItem[]{
                                new ListBoxItem("パイロット名称で並べ替え", "パイロット名称で並べ替え"),
                                new ListBoxItem("レベルで並べ替え", "レベルで並べ替え"),
                                new ListBoxItem(Expression.Term("ＳＰ") + "で並べ替え", "ＳＰで並べ替え"),
                            }).ToList();
                        }
                        else if (Expression.IsOptionDefined("等身大基準"))
                        {
                            sortItems = sortItems.AppendRange(new ListBoxItem[]{
                                new ListBoxItem("名称で並べ替え", "名称で並べ替え"),
                                new ListBoxItem("レベルで並べ替え", "レベルで並べ替え"),
                                new ListBoxItem(Expression.Term("ＨＰ") + "で並べ替え", "ＨＰで並べ替え"),
                                new ListBoxItem(Expression.Term("スペシャルパワー") + "で並べ替え", "スペシャルパワーで並べ替え"),
                            }).ToList();
                        }
                        else
                        {
                            sortItems = sortItems.AppendRange(new ListBoxItem[]{
                                new ListBoxItem(Expression.Term("ＨＰ") + "で並べ替え", "ＨＰで並べ替え"),
                                new ListBoxItem(Expression.Term("ＥＮ") + "で並べ替え", "ＥＮで並べ替え"),
                                new ListBoxItem(Expression.Term("パイロットレベルで並べ替え"), "パイロットレベルで並べ替え"),
                                new ListBoxItem(Expression.Term("ユニット名称で並べ替え"), "ユニット名称で並べ替え"),
                                new ListBoxItem(Expression.Term("ユニット名称で並べ替え"), "ユニット名称で並べ替え"),
                            }).ToList();
                        }
                        var sortRes = GUI.ListBox(new ListBoxArgs
                        {
                            HasFlag = false,
                            Items = sortItems,
                            lb_caption = "選択",
                            lb_info = "一覧表示方法",
                            lb_mode = "連続表示",
                        });

                        // 陣営を変更して再表示
                        if (sortRes > 0)
                        {
                            var sel = sortItems[sortRes - 1].ListItemID;
                            if (Strings.Right(sel, 2) == "一覧")
                            {
                                uparty = Strings.Left(sel, Strings.Len(sel) - 2);
                                goto Beginning;
                            }
                            else if (Strings.Right(sel, 5) == "で並べ替え")
                            {
                                sort_mode = Strings.Left(sel, Strings.Len(sel) - 5);
                                goto SortList;
                            }
                        }

                        goto SortList;
                    }
            }

            GUI.CloseListBox();
            GUI.ReduceListBoxHeight();
            GUI.EnlargeListBoxWidth();

            // 選択されたユニットを画面中央に表示
            var uid = units[ret - 2].ListItemID;
            var u = SRC.UList.Item(uid);
            GUI.Center(u.x, u.y);
            GUI.RefreshScreen();
            Status.DisplayUnitStatus(u);

            // カーソル自動移動
            if (SRC.AutoMoveCursor)
            {
                GUI.MoveCursorPos("ユニット選択", u);
            }
            GUI.UnlockGUI();
        }

        // スペシャルパワー検索コマンド
        private void SearchSpecialPowerCommand()
        {
            // TODO Impl
            //int j, i, ret;
            //string[] list;
            //string[] list2;
            //bool[] flist;
            //string[] pid;
            //string buf;
            //Pilot p;
            //string[] id_list;
            //var strkey_list = default(string[]);
            //int max_item;
            //string max_str;
            //bool found;
            //GUI.LockGUI();

            //// イベント専用のコマンドを除いた全スペシャルパワーのリストを作成
            //list = new string[1];
            //var loopTo = SRC.SPDList.Count();
            //for (i = 1; i <= loopTo; i++)
            //{
            //    {
            //        var withBlock = SRC.SPDList.Item(i);
            //        if (withBlock.intName != "非表示")
            //        {
            //            Array.Resize(list, Information.UBound(list) + 1 + 1);
            //            Array.Resize(strkey_list, Information.UBound(list) + 1);
            //            list[Information.UBound(list)] = withBlock.Name;
            //            strkey_list[Information.UBound(list)] = withBlock.KanaName;
            //        }
            //    }
            //}

            //// ソート
            //var loopTo1 = (Information.UBound(strkey_list) - 1);
            //for (i = 1; i <= loopTo1; i++)
            //{
            //    max_item = i;
            //    max_str = strkey_list[i];
            //    var loopTo2 = Information.UBound(strkey_list);
            //    for (j = (i + 1); j <= loopTo2; j++)
            //    {
            //        if (Strings.StrComp(strkey_list[j], max_str, (CompareMethod)1) == -1)
            //        {
            //            max_item = j;
            //            max_str = strkey_list[j];
            //        }
            //    }

            //    if (max_item != i)
            //    {
            //        buf = list[i];
            //        list[i] = list[max_item];
            //        list[max_item] = buf;
            //        buf = strkey_list[i];
            //        strkey_list[i] = max_str;
            //        strkey_list[max_item] = buf;
            //    }
            //}

            //// 個々のスペシャルパワーに対して、そのスペシャルパワーを使用可能なパイロットが
            //// いるかどうか判定
            //flist = new bool[Information.UBound(list) + 1];
            //var loopTo3 = Information.UBound(list);
            //for (i = 1; i <= loopTo3; i++)
            //{
            //    flist[i] = true;
            //    foreach (Pilot currentP in SRC.PList)
            //    {
            //        p = currentP;
            //        if (p.Party == "味方")
            //        {
            //            if (p.Unit is object)
            //            {
            //                if (p.Unit.Status == "出撃" && !p.Unit.IsConditionSatisfied("憑依"))
            //                {
            //                    // 本当に乗っている？
            //                    found = false;
            //                    {
            //                        var withBlock1 = p.Unit;
            //                        if (ReferenceEquals(p, withBlock1.MainPilot()))
            //                        {
            //                            found = true;
            //                        }
            //                        else
            //                        {
            //                            var loopTo4 = withBlock1.CountPilot();
            //                            for (j = 2; j <= loopTo4; j++)
            //                            {
            //                                Pilot localPilot() { object argIndex1 = j; var ret = withBlock1.Pilot(argIndex1); return ret; }

            //                                if (ReferenceEquals(p, localPilot()))
            //                                {
            //                                    found = true;
            //                                    break;
            //                                }
            //                            }

            //                            var loopTo5 = withBlock1.CountSupport();
            //                            for (j = 1; j <= loopTo5; j++)
            //                            {
            //                                Pilot localSupport() { object argIndex1 = j; var ret = withBlock1.Support(argIndex1); return ret; }

            //                                if (ReferenceEquals(p, localSupport()))
            //                                {
            //                                    found = true;
            //                                    break;
            //                                }
            //                            }

            //                            if (ReferenceEquals(p, withBlock1.AdditionalSupport()))
            //                            {
            //                                found = true;
            //                            }
            //                        }
            //                    }

            //                    if (found)
            //                    {
            //                        if (p.IsSpecialPowerAvailable(list[i]))
            //                        {
            //                            flist[i] = false;
            //                            break;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            //while (true)
            //{
            //    GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
            //    GUI.ListItemComment = new string[Information.UBound(list) + 1];
            //    id_list = new string[Information.UBound(list) + 1];
            //    strkey_list = new string[Information.UBound(list) + 1];

            //    // 選択出来ないスペシャルパワーをマスク
            //    var loopTo6 = Information.UBound(GUI.ListItemFlag);
            //    for (i = 1; i <= loopTo6; i++)
            //        GUI.ListItemFlag[i] = flist[i];

            //    // スペシャルパワーの解説を設定
            //    var loopTo7 = Information.UBound(GUI.ListItemComment);
            //    for (i = 1; i <= loopTo7; i++)
            //    {
            //        SpecialPowerData localItem() { var tmp = list; object argIndex1 = tmp[i]; var ret = SRC.SPDList.Item(argIndex1); return ret; }

            //        GUI.ListItemComment[i] = localItem().Comment;
            //    }

            //    // 検索するスペシャルパワーを選択
            //    GUI.TopItem = 1;
            //    ret = GUI.MultiColumnListBox(Expression.Term("スペシャルパワー", u: null) + "検索", list, true);
            //    if (ret == 0)
            //    {
            //        CancelCommand();
            //        GUI.UnlockGUI();
            //        return;
            //    }

            //    SelectedSpecialPower = list[ret];

            //    // 選択されたスペシャルパワーを使用できるパイロットの一覧を作成
            //    list2 = new string[1];
            //    GUI.ListItemFlag = new bool[1];
            //    id_list = new string[1];
            //    pid = new string[1];
            //    foreach (Pilot currentP1 in SRC.PList)
            //    {
            //        p = currentP1;
            //        // 選択したスペシャルパワーを使用できるパイロットかどうか判定
            //        if (p.Party != "味方")
            //        {
            //            goto NextLoop;
            //        }

            //        if (p.Unit is null)
            //        {
            //            goto NextLoop;
            //        }

            //        if (p.Unit.Status != "出撃")
            //        {
            //            goto NextLoop;
            //        }

            //        if (p.Unit.CountPilot() > 0)
            //        {
            //            if ((p.ID ?? "") == (p.Unit.Pilot(1).ID ?? "") && (p.ID ?? "") != (p.Unit.MainPilot().ID ?? ""))
            //            {
            //                // 追加パイロットのため、使用されていない
            //                goto NextLoop;
            //            }
            //        }

            //        if (!p.IsSpecialPowerAvailable(SelectedSpecialPower))
            //        {
            //            goto NextLoop;
            //        }

            //        // パイロットをリストに追加
            //        Array.Resize(list2, Information.UBound(list2) + 1 + 1);
            //        Array.Resize(GUI.ListItemFlag, Information.UBound(list2) + 1);
            //        Array.Resize(id_list, Information.UBound(list2) + 1);
            //        Array.Resize(pid, Information.UBound(list2) + 1);
            //        GUI.ListItemFlag[Information.UBound(list2)] = false;
            //        id_list[Information.UBound(list2)] = p.Unit.ID;
            //        pid[Information.UBound(list2)] = p.ID;
            //        string localRightPaddedString() { string argbuf = p.get_Nickname(false); var ret = GeneralLib.RightPaddedString(argbuf, 19); p.get_Nickname(false) = argbuf; return ret; }

            //        string localRightPaddedString1() { string argbuf = SrcFormatter.Format(p.SP) + "/" + SrcFormatter.Format(p.MaxSP); var ret = GeneralLib.RightPaddedString(argbuf, 10); return ret; }

            //        list2[Information.UBound(list2)] = localRightPaddedString() + localRightPaddedString1();
            //        buf = "";
            //        var loopTo8 = p.CountSpecialPower;
            //        for (j = 1; j <= loopTo8; j++)
            //        {
            //            SpecialPowerData localItem1() { object argIndex1 = p.get_SpecialPower(j); var ret = SRC.SPDList.Item(argIndex1); p.get_SpecialPower(j) = Conversions.ToString(argIndex1); return ret; }

            //            buf = buf + localItem1().intName;
            //        }

            //        list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + GeneralLib.RightPaddedString(buf, 12);
            //        if (p.SP < p.SpecialPowerCost(SelectedSpecialPower))
            //        {
            //            list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + " " + Expression.Term("ＳＰ", p.Unit) + "不足";
            //        }

            //        if (p.Unit.Action == 0)
            //        {
            //            list2[Information.UBound(list2)] = list2[Information.UBound(list2)] + " 行動済";
            //        }

            //    NextLoop:
            //        ;
            //    }

            //    SelectedSpecialPower = "";

            //    // 検索をかけるパイロットの選択
            //    GUI.TopItem = 1;
            //    GUI.EnlargeListBoxHeight();
            //    if (Expression.IsOptionDefined("等身大基準"))
            //    {
            //        ret = GUI.ListBox("ユニット選択", list2, "ユニット           " + Expression.Term("SP", null, 2) + "/Max" + Expression.Term("SP", argu2, 2) + "  " + Expression.Term("スペシャルパワー", u: null), lb_mode: "");
            //    }
            //    else
            //    {
            //        ret = GUI.ListBox("パイロット選択", list2, "パイロット         " + Expression.Term(argtname5, argu4, 2) + "/Max" + Expression.Term("SP", argu5, 2) + "  " + Expression.Term("スペシャルパワー", u: null), lb_mode: "");
            //    }

            //    GUI.ReduceListBoxHeight();

            //    // パイロットの乗るユニットを画面中央に表示
            //    if (ret > 0)
            //    {
            //        var tmp = pid;
            //        {
            //            var withBlock2 = SRC.PList.Item(tmp[ret]);
            //            GUI.Center(withBlock2.Unit.x, withBlock2.Unit.y);
            //            GUI.RefreshScreen();
            //            Status.DisplayUnitStatus(withBlock2.Unit);

            //            // カーソル自動移動
            //            if (SRC.AutoMoveCursor)
            //            {
            //                GUI.MoveCursorPos("ユニット選択", withBlock2.Unit);
            //            }
            //        }

            //        id_list = new string[1];
            //        GUI.UnlockGUI();
            //        return;
            //    }
            //}
        }

        // リスタートコマンド
        private void RestartCommand()
        {
            int ret;

            // リスタートを行うか確認
            var suspendRes = GUI.Confirm("リスタートしますか？", "リスタート", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
            if (suspendRes != GuiDialogResult.Ok)
            {
                return;
            }

            GUI.LockGUI();
            SRC.RestoreData(Path.Combine(SRC.ScenarioPath, "_リスタート.srcq"), SRCSaveKind.Restart);
            GUI.UnlockGUI();
        }

        // クイックロードコマンド
        private void QuickLoadCommand()
        {
            // ロードを行うか確認
            var suspendRes = GUI.Confirm("データをロードしますか？", "クイックロード", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
            if (suspendRes != GuiDialogResult.Ok)
            {
                return;
            }

            GUI.LockGUI();
            SRC.RestoreData(Path.Combine(SRC.ScenarioPath, "_クイックセーブ.srcq"), SRCSaveKind.Quik);

            // 画面を書き直してステータスを表示
            GUI.RedrawScreen();
            Status.DisplayGlobalStatus();
            GUI.UnlockGUI();
        }

        // クイックセーブコマンド
        private void QuickSaveCommand()
        {
            GUI.LockGUI();

            // マウスカーソルを砂時計に
            GUI.ChangeStatus(GuiStatus.WaitCursor);

            // 中断データをセーブ
            SRC.DumpData(Path.Combine(SRC.ScenarioPath, "_クイックセーブ.srcq"), SRCSaveKind.Quik);
            GUI.UnlockGUI();

            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);
        }


        // プレイを中断し、中断用データをセーブする
        private void DumpCommand()
        {
            // プレイを中断するか確認
            var suspendRes = GUI.Confirm("プレイを中断しますか？", "中断", GuiConfirmOption.OkCancel | GuiConfirmOption.Question);
            if (suspendRes != GuiDialogResult.Ok)
            {
                return;
            }

            // 中断データをセーブするファイル名を決定
            var fname = Path.GetFileNameWithoutExtension(SRC.ScenarioFileName) + "を中断.srcq";
            // XXX Streamを取った結果ファイル目が消え失せてしまった
            using (var saveStream = GUI.SelectSaveStream(SRCSaveKind.Suspend, fname))
            {
                if (saveStream == null)
                {
                    // キャンセル
                    return;
                }

                // マウスカーソルを砂時計に
                GUI.ChangeStatus(GuiStatus.WaitCursor);
                GUI.LockGUI();

                // 中断データをセーブ
                SRC.DumpData(saveStream, SRCSaveKind.Suspend);
            }
            // マウスカーソルを元に戻す
            GUI.ChangeStatus(GuiStatus.Default);
            GUI.MainFormHide();

            // ゲームを終了
            SRC.ExitGame();
        }

        // 全体マップの表示
        private void GlobalMapCommand()
        {
            GUI.LockGUI();
            GUI.DisplayGlobalMap();
            GUI.UnlockGUI();
        }
    }
}
