// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using Newtonsoft.Json;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Pilots;
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
            GUI.LockGUI();

            // イベント専用のコマンドを除いた全スペシャルパワーのリストを作成
            var allSpList = SRC.SPDList.Items
                .Where(x => x.ShortName != "非表示")
                .OrderBy(x => x.KanaName)
                .ToList();

            // 個々のスペシャルパワーに対して、そのスペシャルパワーを使用可能なパイロットが
            // いるかどうか判定
            var pilotMap = allSpList.Select(x => new
            {
                spName = x.Name,
                pilots = SRC.PList.Items
                        .Where(p => p.Party == "味方"
                            && p.Unit != null
                            && p.Unit.Status == "出撃"
                            && !p.Unit.IsConditionSatisfied("憑依")
                            && p.IsSpecialPowerAvailable(x.Name)
                            // 本当に乗っている？
                            && p.Unit.AllPilots.Any(up => up == p))
                        .ToList(),
            }).ToDictionary(x => x.spName, x => x.pilots);

            while (true)
            {
                var list = allSpList.Select(x => new ListBoxItem(x.Name, x.Name)
                {
                    // 選択出来ないスペシャルパワーをマスク
                    ListItemFlag = !pilotMap[x.Name].Any(),
                    // スペシャルパワーの解説を設定
                    ListItemComment = x.Comment,
                }).ToList();
                var listArgs = new ListBoxArgs
                {
                    lb_caption = Expression.Term("スペシャルパワー") + "検索",
                    Items = list,
                    HasFlag = true,
                };
                // 検索するスペシャルパワーを選択
                GUI.TopItem = 1;
                // TODO Impl MultiColumnListBox
                //ret = GUI.MultiColumnListBox(, list, true);
                var ret = GUI.ListBox(listArgs);
                if (ret == 0)
                {
                    CancelCommand();
                    GUI.UnlockGUI();
                    return;
                }

                var sel = list[ret - 1].ListItemID;
                SelectedSpecialPower = sel;

                // 選択されたスペシャルパワーを使用できるパイロットの一覧を作成
                var pilots = pilotMap.ContainsKey(sel) ? pilotMap[sel] : new List<Pilot>();
                var list2 = pilots.Select(p =>
                {
                    var msg = GeneralLib.RightPaddedString(p.get_Nickname(false), 19)
                        + GeneralLib.RightPaddedString(SrcFormatter.Format(p.SP) + "/" + SrcFormatter.Format(p.MaxSP), 10);

                    msg += GeneralLib.RightPaddedString(string.Join("", p.SpecialPowerNames
                            .Where(x => p.IsSpecialPowerAvailable(x))
                            .Select(x => SRC.SPDList.Item(x).ShortName)), 12);

                    if (p.SP < p.SpecialPowerCost(SelectedSpecialPower))
                    {
                        msg += " " + Expression.Term("ＳＰ", p.Unit) + "不足";
                    }

                    if (p.Unit.Action == 0)
                    {
                        msg += " 行動済";
                    }
                    return new ListBoxItem(msg, p.ID)
                    {
                        // XXX ここはフラグ立たない？
                    };
                }).ToList();

                SelectedSpecialPower = "";

                // 検索をかけるパイロットの選択
                GUI.TopItem = 1;
                GUI.EnlargeListBoxHeight();
                string caption2;
                string info2;
                if (Expression.IsOptionDefined("等身大基準"))
                {
                    caption2 = "ユニット選択";
                    info2 = "ユニット           " + Expression.Term("SP", null, 2) + "/Max" + Expression.Term("SP", null, 2) + "  " + Expression.Term("スペシャルパワー");
                }
                else
                {
                    caption2 = "パイロット選択";
                    info2 = "パイロット         " + Expression.Term("SP", null, 2) + "/Max" + Expression.Term("SP", null, 2) + "  " + Expression.Term("スペシャルパワー");
                }
                var ret2 = GUI.ListBox(new ListBoxArgs
                {
                    lb_caption = caption2,
                    lb_info = info2,
                    Items = list2,
                });

                GUI.ReduceListBoxHeight();

                // パイロットの乗るユニットを画面中央に表示
                if (ret2 > 0)
                {
                    var p = SRC.PList.Item(list2[ret2 - 1].ListItemID);
                    GUI.Center(p.Unit.x, p.Unit.y);
                    GUI.RefreshScreen();
                    Status.DisplayUnitStatus(p.Unit);

                    // カーソル自動移動
                    if (SRC.AutoMoveCursor)
                    {
                        GUI.MoveCursorPos("ユニット選択", p.Unit);
                    }

                    GUI.UnlockGUI();
                    return;
                }
            }
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
