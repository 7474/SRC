// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
        private Sound Sound => SRC.Sound;

        public InterMission(SRC src)
        {
            SRC = src;
        }

        // インターミッション
        public void InterMissionCommand(bool skip_update = false)
        {
            SRC.Stage = "インターミッション";
            SRC.IsSubStage = false;

            // ＢＧＭを変更
            Sound.KeepBGM = false;
            Sound.BossBGM = false;
            Sound.ChangeBGM(Sound.BGMName("Intermission"));

            // マップをクリア
            // XXX Map.ClearMap(); していれば要らんのでは
            var loopTo = Map.MapWidth;
            for (var i = 1; i <= loopTo; i++)
            {
                var loopTo1 = Map.MapHeight;
                for (var j = 1; j <= loopTo1; j++)
                    Map.MapDataForUnit[i, j] = null;
            }

            // 各種データをアップデート
            if (!skip_update)
            {
                SRC.UList.Update();
                SRC.PList.Update();
                SRC.IList.Update();
            }

            Event.ClearEventData();
            Map.ClearMap();

            // 選択用ダイアログを拡大
            GUI.EnlargeListBoxHeight();
            while (true)
            {
                // 利用可能なインターミッションコマンドを選択
                var cmd_list = new List<ListBoxItem>();

                // 「次のステージへ」コマンド
                if (!string.IsNullOrEmpty(Expression.GetValueAsString("次ステージ")))
                {
                    cmd_list.Add(new ListBoxItem("次のステージへ"));
                }

                // データセーブコマンド
                if (!Expression.IsOptionDefined("データセーブ不可") || Expression.IsOptionDefined("デバッグ"))
                {
                    cmd_list.Add(new ListBoxItem("データセーブ"));
                }

                // 機体改造コマンド
                if (!Expression.IsOptionDefined("改造不可"))
                {
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        cmd_list.Add(new ListBoxItem("ユニットの強化"));
                    }
                    else
                    {
                        if (SRC.UList.Items.Any(u => u.Party0 == "味方" && u.Status == "待機" && Strings.Left(u.Class, 1) == "("))
                        {
                            cmd_list.Add(new ListBoxItem("ユニットの強化"));
                        }
                        else
                        {
                            cmd_list.Add(new ListBoxItem("機体改造"));
                        }
                    }
                }

                // 乗り換えコマンド
                if (Expression.IsOptionDefined("乗り換え"))
                {
                    cmd_list.Add(new ListBoxItem("乗り換え"));
                }

                // アイテム交換コマンド
                if (Expression.IsOptionDefined("アイテム交換"))
                {
                    cmd_list.Add(new ListBoxItem("アイテム交換"));
                }

                // 換装コマンド
                foreach (var u in SRC.UList.Items)
                {
                    if (u.Party0 == "味方" && u.Status == "待機")
                    {
                        if (u.IsFeatureAvailable("換装"))
                        {
                            var fd = u.Feature("換装");
                            if (fd.DataL.Any(x => u.OtherForm(x).IsAvailable()))
                            {
                                cmd_list.Add(new ListBoxItem("換装"));
                                break;
                            }
                        }
                    }
                }

                // パイロットステータスコマンド
                if (!Expression.IsOptionDefined("等身大基準"))
                {
                    cmd_list.Add(new ListBoxItem("パイロットステータス"));
                }

                // ユニットステータスコマンド
                cmd_list.Add(new ListBoxItem("ユニットステータス"));

                // ユーザー定義のインターミッションコマンド
                foreach (var var in Event.GlobalVariableList.Values)
                {
                    if (Strings.InStr(var.Name, "IntermissionCommand(") == 1)
                    {
                        var pos = Strings.Len("IntermissionCommand(");
                        var buf = Strings.Mid(var.Name, pos + 1, Strings.Len(var.Name) - pos - 1);
                        buf = Expression.GetValueAsString(buf);
                        Expression.FormatMessage(ref buf);
                        cmd_list.Add(new ListBoxItem(buf, var.Name));
                    }
                }

                // 終了コマンド
                cmd_list.Add(new ListBoxItem("SRCを終了"));

                // インターミッションのコマンド名称にエリアスを適用
                var name_list = cmd_list.Select(x =>
                {
                    x.Text = SRC.ALDList.Items.FirstOrDefault(
                        a => a.Elements.FirstOrDefault()?.strAliasType == x.Text
                    )?.Name ?? x.Text;
                    return x;
                }).ToList();

                // プレイヤーによるコマンド選択
                GUI.TopItem = 1;
                string arglb_caption = "インターミッション： 総ターン数"
                    + SrcFormatter.Format(SRC.TotalTurn) + " "
                    + Expression.Term("資金")
                    + SrcFormatter.Format(SRC.Money);
                var ret = GUI.ListBox(new ListBoxArgs()
                {
                    Items = name_list,
                    HasFlag = false,
                    lb_caption = "コマンド",
                    lb_mode = "連続表示",
                });
                if (ret == 0) { continue; }

                var selectedItem = cmd_list[ret - 1];
                // 選択されたインターミッションコマンドを実行
                switch (selectedItem.ListItemID ?? "")
                {
                    case "次のステージへ":
                        if (GUI.Confirm("次のステージへ進みますか？",
                            "次ステージ",
                            GuiConfirmOption.OkCancel | GuiConfirmOption.Question) == GuiDialogResult.Ok)
                        {
                            SRC.UList.Update(); // 追加パイロットを消去
                            GUI.CloseListBox();
                            GUI.ReduceListBoxHeight();
                            Sound.StopBGM();
                            return;
                        }
                        break;

                    case "データセーブ":
                        using (var saveStream = GUI.SelectSaveStream(SRCSaveKind.Normal))
                        {
                            if (saveStream != null)
                            {
                                SRC.UList.Update(); // 追加パイロットを消去
                                SRC.SaveData(saveStream);
                            }
                        }
                        break;

                    case "機体改造":
                    case "ユニットの強化":
                        {
                            RankUpCommand();
                            break;
                        }

                    case "乗り換え":
                        {
                            ExchangeUnitCommand();
                            break;
                        }

                    case "アイテム交換":
                        {
                            ExchangeItemCommand(selected_unit: null, selected_part: "");
                            break;
                        }

                    case "換装":
                        {
                            ExchangeFormCommand();
                            break;
                        }

                    case "SRCを終了":
                        if (GUI.Confirm("SRCを終了しますか？",
                            "終了",
                            GuiConfirmOption.OkCancel | GuiConfirmOption.Question) == GuiDialogResult.Ok)
                        {
                            GUI.CloseListBox();
                            GUI.ReduceListBoxHeight();
                            SRC.ExitGame();
                        }

                        break;

                    case "パイロットステータス":
                        {
                            GUI.CloseListBox();
                            GUI.ReduceListBoxHeight();
                            SRC.IsSubStage = true;

                            // TODO FileSystemに逃がす
                            var eveFile = new string[]
                            {
                                SRC.ScenarioPath,
                                SRC.ExtDataPath,
                                SRC.ExtDataPath2,
                                SRC.AppPath,
                            }.Where(x => Directory.Exists(x))
                                .Select(x => Path.Combine(x, "Lib", "パイロットステータス表示.eve"))
                                .Where(x => SRC.FileSystem.FileExists(x))
                                .FirstOrDefault();
                            if (!string.IsNullOrEmpty(eveFile))
                            {
                                SRC.StartScenario(eveFile);
                            }
                            // サブステージを通常のステージとして実行
                            SRC.IsSubStage = true;
                            return;
                        }

                    case "ユニットステータス":
                        {
                            GUI.CloseListBox();
                            GUI.ReduceListBoxHeight();
                            SRC.IsSubStage = true;

                            // TODO FileSystemに逃がす
                            var eveFile = new string[]
                            {
                                SRC.ScenarioPath,
                                SRC.ExtDataPath,
                                SRC.ExtDataPath2,
                                SRC.AppPath,
                            }.Where(x => Directory.Exists(x))
                                .Select(x => Path.Combine(x, "Lib", "ユニットステータス表示.eve"))
                                .Where(x => SRC.FileSystem.FileExists(x))
                                .FirstOrDefault();
                            if (!string.IsNullOrEmpty(eveFile))
                            {
                                SRC.StartScenario(eveFile);
                            }
                            // サブステージを通常のステージとして実行
                            SRC.IsSubStage = true;
                            return;
                        }

                    // ユーザー定義のインターミッションコマンド
                    default:
                        {
                            GUI.CloseListBox();
                            GUI.ReduceListBoxHeight();
                            SRC.IsSubStage = true;
                            SRC.StartScenario(Expression.GetValueAsString(selectedItem.ListItemID));
                            if (SRC.IsSubStage)
                            {
                                // インターミッションを再開
                                Sound.KeepBGM = false;
                                Sound.BossBGM = false;
                                Sound.ChangeBGM(Sound.BGMName("Intermission"));
                                SRC.UList.Update();
                                SRC.PList.Update();
                                SRC.IList.Update();
                                Event.ClearEventData();
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

        // 機体改造コマンド
        public void RankUpCommand()
        {
            GUI.TopItem = 1;
            string sort_mode;

            // デフォルトのソート方法
            if (Expression.IsOptionDefined("等身大基準"))
            {
                sort_mode = "レベル";
            }
            else
            {
                sort_mode = "ＨＰ";
            }

            // 最大改造数がユニット毎に変更されているかをあらかじめチェック
            var use_max_rank = SRC.UList.Items.Any(u => u.IsFeatureAvailable("最大改造数"));

            // ユニット名の項の文字数を設定
            var name_width = 33;
            if (use_max_rank)
            {
                name_width = (name_width - 2);
            }

            if (Expression.IsOptionDefined("等身大基準"))
            {
                name_width = (name_width + 8);
            }

            // ユニットのリストを作成
            var list = new List<ListBoxItem>();
            list.Add(new ListBoxItem("▽並べ替え▽", "▽並べ替え▽"));


        Beginning:
            ;

            var units = SRC.UList.Items
                .Where(u => !(u.Party0 != "味方" || u.Status != "待機"))
                .Select(u =>
                {
                    // 改造が可能？
                    var cost = RankUpCost(u);
                    // ユニットランク
                    string msg = GeneralLib.RightPaddedString(u.Nickname0, name_width)
                              + GeneralLib.LeftPaddedString("" + u.Rank, 2);
                    if (use_max_rank)
                    {
                        if (MaxRank(u) > 0)
                        {
                            msg += "/" + GeneralLib.LeftPaddedString("" + MaxRank(u), 2);
                        }
                        else
                        {
                            msg += "/--";
                        }
                    }
                    // 改造に必要な資金
                    if (cost < 10000000)
                    {
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(cost), 7);
                    }
                    else
                    {
                        msg += GeneralLib.LeftPaddedString("----", 7);
                    }
                    // 等身大基準の場合はパイロットレベルも表示
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        if (u.CountPilot() > 0)
                        {
                            msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MainPilot().Level), 3);
                        }
                    }
                    // ユニットに関する情報
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxHP), 6);
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxEN), 5);
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Armor("")), 5);
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Mobility("")), 5);
                    // 等身大基準でない場合はパイロット名を表示
                    if (!Expression.IsOptionDefined("等身大基準"))
                    {
                        if (u.CountPilot() > 0)
                        {
                            msg += "  " + u.MainPilot().get_Nickname(false);
                        }
                    }

                    // 装備しているアイテムをコメント欄に列記
                    var comment = string.Join(" ", u.ItemList
                        .Where(itm => (itm.Class() != "固定" || !itm.IsFeatureAvailable("非表示")) && itm.Part() != "非表示")
                        .Select(itm => itm.Nickname()));

                    return new ListBoxItem(msg, u.ID)
                    {
                        ListItemFlag = cost > SRC.Money || cost > 10000000,
                    };
                })
                .ToList();

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
                        case "装甲": return u.get_Armor("");
                        case "運動性": return u.get_Mobility("");
                        case "ユニットランク": return u.Rank;
                        case "レベル": return u.MainPilot()?.TotalExp ?? 0;
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

            // 改造するユニットを選択
            string caption = "ユニット選択： " + Expression.Term("資金", u: null) + SrcFormatter.Format(SRC.Money);
            string info;
            if (Expression.IsOptionDefined("等身大基準"))
            {
                if (use_max_rank)
                {
                    info = "ユニット                               "
                        + Expression.Term("ランク", null, 6) + "  費用 Lv  "
                        + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                        + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", u: null);
                }
                else
                {
                    info = "ユニット                             "
                        + Expression.Term("ランク", null, 6) + "  費用 Lv  "
                        + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                        + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", u: null);
                }
            }
            else
            {
                if (use_max_rank)
                {
                    info = "ユニット                       "
                        + Expression.Term("ランク", null, 6) + "  費用  "
                        + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                        + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", u: null) + " パイロット";
                }
                else
                {
                    info = "ユニット                     "
                        + Expression.Term("ランク", null, 6) + "  費用  "
                        + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                        + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", u: null) + " パイロット";
                }
            }
            var ret = GUI.ListBox(new ListBoxArgs
            {
                HasFlag = true,
                Items = list.AppendRange(units).ToList(),
                lb_caption = caption,
                lb_info = info,
                lb_mode = "連続表示,コメント",
            });

            switch (ret)
            {
                case 0:
                    // キャンセル
                    return;

                case 1:
                    // ソート方法を選択
                    IList<ListBoxItem> sortItems;
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        sortItems = new List<ListBoxItem>
                            {
                                new ListBoxItem("名称", "名称"),
                                new ListBoxItem("レベル", "レベル"),
                                new ListBoxItem(Expression.Term("ＨＰ"), "ＨＰ"),
                                new ListBoxItem(Expression.Term("ＥＮ"), "ＥＮ"),
                                new ListBoxItem(Expression.Term("装甲"), "装甲"),
                                new ListBoxItem(Expression.Term("運動性"), "運動性"),
                                new ListBoxItem(Expression.Term("ランク"), "ユニットランク"),
                            };
                    }
                    else
                    {
                        sortItems = new List<ListBoxItem>
                            {
                                new ListBoxItem(Expression.Term("ＨＰ"), "ＨＰ"),
                                new ListBoxItem(Expression.Term("ＥＮ"), "ＥＮ"),
                                new ListBoxItem(Expression.Term("装甲"), "装甲"),
                                new ListBoxItem(Expression.Term("運動性"), "運動性"),
                                new ListBoxItem(Expression.Term("ランク"), "ユニットランク"),
                                new ListBoxItem(Expression.Term("ユニット名称"), "ユニット名称"),
                                new ListBoxItem(Expression.Term("パイロット名称"), "パイロット名称"),
                            };
                    }
                    var sortRes = GUI.ListBox(new ListBoxArgs
                    {
                        HasFlag = false,
                        Items = sortItems,
                        lb_caption = "どれで並べ替えますか？",
                        lb_info = "並べ替えの方法",
                        lb_mode = "連続表示,コメント",
                    });

                    // ソート方法を変更して再表示
                    if (sortRes > 0)
                    {
                        sort_mode = sortItems[sortRes - 1].ListItemID;
                    }
                    goto Beginning;
            }

            // 改造するユニットを検索
            var u = SRC.UList.Item(units[ret - 2].ListItemID);

            // 改造するか確認
            if (u.IsHero())
            {
                if (GUI.Confirm(u.Nickname0 + "をパワーアップさせますか？",
                    "パワーアップ",
                    GuiConfirmOption.OkCancel | GuiConfirmOption.Question) != GuiDialogResult.Ok)
                {
                    goto Beginning;
                }
            }
            else
            {
                if (GUI.Confirm(u.Nickname0 + "を改造しますか？",
                    "改造",
                    GuiConfirmOption.OkCancel | GuiConfirmOption.Question) != GuiDialogResult.Ok)
                {
                    goto Beginning;
                }
            }

            // 資金を減らす
            SRC.IncrMoney(-RankUpCost(u));

            // ユニットランクを一段階上げる
            RankUp(u);

            goto Beginning;
        }

        private void RankUp(Unit u)
        {
            // TODO 適当に Unit#update
            u.Rank = u.Rank + 1;

            // 他形態のランクも上げておく
            foreach (var of in u.OtherForms)
            {
                of.Rank = u.Rank;
            }
            // 合体形態が主形態の分離形態が改造された場合は他の分離形態のユニットの
            // ランクも上げる
            if (u.IsFeatureAvailable("合体"))
            {
                var mu = u.Features.Where(fd => fd.Name == "合体")
                    .Select(fd => new
                    {
                        u = SRC.UDList.Item(GeneralLib.LIndex(fd.Data, 2)),
                        fd = fd,
                    })
                    .Where(x => x.u != null)
                    .Where(x => (GeneralLib.LLength(x.fd.Data) == 3 && x.u.IsFeatureAvailable("主形態"))
                        || (GeneralLib.LLength(x.fd.Data) != 3 && !x.u.IsFeatureAvailable("制限時間")))
                    .Select(x => x.u)
                    .FirstOrDefault();
                if (mu != null)
                {
                    if (mu.IsFeatureAvailable("分離"))
                    {
                        var buf = mu.FeatureData("分離");
                        foreach (var pu in GeneralLib.ToList(buf).Skip(1).Select(x => SRC.UList.Item(x)).Where(x => x != null))
                        {
                            pu.Rank = GeneralLib.MaxLng(u.Rank, pu.Rank);
                            foreach (var of in pu.OtherForms)
                            {
                                of.Rank = pu.Rank;
                            }
                        }
                    }
                }
                // XXX 分離ユニット選択肢に加えるの？
                {
                    //if (i <= u.CountFeature())
                    //{
                    //    urank = u.Rank;
                    //    string localFeatureData2() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

                    //    string localLIndex() { string arglist = hsba79967d549343448297e0bcf57b2982(); var ret = GeneralLib.LIndex(arglist, 2); return ret; }

                    //    UnitData localItem8() { object argIndex1 = (object)hsd871da3601884bc086fad5045542bc83(); var ret = SRC.UDList.Item(argIndex1); return ret; }

                    //    buf = localItem8().FeatureData("分離");
                    //    var loopTo17 = GeneralLib.LLength(buf);
                    //    for (i = 2; i <= loopTo17; i++)
                    //    {
                    //        bool localIsDefined() { object argIndex1 = GeneralLib.LIndex(buf, i); var ret = SRC.UList.IsDefined(argIndex1); return ret; }

                    //        if (!localIsDefined())
                    //        {
                    //            goto NextForm;
                    //        }

                    //        {
                    //            var withBlock8 = SRC.UList.Item(GeneralLib.LIndex(buf, i));
                    //            withBlock8.Rank = GeneralLib.MaxLng(urank, withBlock8.Rank);
                    //            withBlock8.HP = withBlock8.MaxHP;
                    //            withBlock8.EN = withBlock8.MaxEN;
                    //            var loopTo18 = withBlock8.CountOtherForm();
                    //            for (j = 1; j <= loopTo18; j++)
                    //            {
                    //                Unit localOtherForm5() { object argIndex1 = j; var ret = withBlock8.OtherForm(argIndex1); return ret; }

                    //                localOtherForm5().Rank = withBlock8.Rank;
                    //                Unit localOtherForm6() { object argIndex1 = j; var ret = withBlock8.OtherForm(argIndex1); return ret; }

                    //                Unit localOtherForm7() { object argIndex1 = j; var ret = withBlock8.OtherForm(argIndex1); return ret; }

                    //                localOtherForm6().HP = localOtherForm7().MaxHP;
                    //                Unit localOtherForm8() { object argIndex1 = j; var ret = withBlock8.OtherForm(argIndex1); return ret; }

                    //                Unit localOtherForm9() { object argIndex1 = j; var ret = withBlock8.OtherForm(argIndex1); return ret; }

                    //                localOtherForm8().EN = localOtherForm9().MaxEN;
                    //            }

                    //            var loopTo19 = Information.UBound(id_list);
                    //            for (j = 1; j <= loopTo19; j++)
                    //            {
                    //                if ((withBlock8.CurrentForm().ID ?? "") == (id_list[j] ?? ""))
                    //                {
                    //                    break;
                    //                }
                    //            }

                    //            if (j > Information.UBound(id_list))
                    //            {
                    //                goto NextForm;
                    //            }

                    //            if (use_max_rank)
                    //            {
                    //                string localRightPaddedString3() { string argbuf = withBlock8.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock8.Nickname0 = argbuf; return ret; }

                    //                string localLeftPaddedString8() { string argbuf = SrcFormatter.Format(withBlock8.Rank); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

                    //                list[j] = localRightPaddedString3() + localLeftPaddedString8() + "/";
                    //                if (MaxRank(u) > 0)
                    //                {
                    //                    string localLeftPaddedString9() { string argbuf = SrcFormatter.Format(MaxRank(u)); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

                    //                    list[j] = list[j] + localLeftPaddedString9();
                    //                }
                    //                else
                    //                {
                    //                    list[j] = list[j] + "--";
                    //                }
                    //            }
                    //            else if (withBlock8.Rank < 10)
                    //            {
                    //                string localRightPaddedString4() { string argbuf = withBlock8.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock8.Nickname0 = argbuf; return ret; }

                    //                list[j] = localRightPaddedString4() + Strings.StrConv(SrcFormatter.Format(withBlock8.Rank), VbStrConv.Wide);
                    //            }
                    //            else
                    //            {
                    //                string localRightPaddedString5() { string argbuf = withBlock8.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, name_width); withBlock8.Nickname0 = argbuf; return ret; }

                    //                list[j] = localRightPaddedString5() + SrcFormatter.Format(withBlock8.Rank);
                    //            }

                    //            if (RankUpCost(u) < 1000000)
                    //            {
                    //                string localLeftPaddedString10() { string argbuf = SrcFormatter.Format(RankUpCost(u)); var ret = GeneralLib.LeftPaddedString(argbuf, 7); return ret; }

                    //                list[j] = list[j] + localLeftPaddedString10();
                    //            }
                    //            else
                    //            {
                    //                list[j] = list[j] + GeneralLib.LeftPaddedString("----", 7);
                    //            }

                    //            if (Expression.IsOptionDefined("等身大基準"))
                    //            {
                    //                if (withBlock8.CountPilot() > 0)
                    //                {
                    //                    string localLeftPaddedString11() { string argbuf = SrcFormatter.Format(withBlock8.MainPilot().Level); var ret = GeneralLib.LeftPaddedString(argbuf, 3); return ret; }

                    //                    list[j] = list[j] + localLeftPaddedString11();
                    //                }
                    //            }

                    //            string localLeftPaddedString12() { string argbuf = SrcFormatter.Format(withBlock8.MaxHP); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

                    //            string localLeftPaddedString13() { string argbuf = SrcFormatter.Format(withBlock8.MaxEN); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

                    //            string localLeftPaddedString14() { string argbuf = SrcFormatter.Format(withBlock8.get_Armor("")); var ret = GeneralLib.LeftPaddedString(argbuf, 6); return ret; }

                    //            string localLeftPaddedString15() { string argbuf = SrcFormatter.Format(withBlock8.get_Mobility("")); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

                    //            list[j] = list[j] + localLeftPaddedString12() + localLeftPaddedString13() + localLeftPaddedString14() + localLeftPaddedString15();
                    //            if (!Expression.IsOptionDefined("等身大基準"))
                    //            {
                    //                if (withBlock8.CountPilot() > 0)
                    //                {
                    //                    list[j] = list[j] + "  " + withBlock8.MainPilot().get_Nickname(false);
                    //                }
                    //            }
                    //        }

                    //    NextForm:
                    //        ;
                    //    }
                    //}
                }
            }

            // 合体ユニットの場合は分離形態のユニットのランクも上げる
            if (u.IsFeatureAvailable("分離"))
            {
                var buf = u.FeatureData("分離");
                foreach (var pu in GeneralLib.ToList(buf).Skip(1).Select(x => SRC.UList.Item(x)).Where(x => x != null))
                {
                    pu.Rank = GeneralLib.MaxLng(u.Rank, pu.Rank);
                    foreach (var of in pu.OtherForms)
                    {
                        of.Rank = pu.Rank;
                    }
                }
            }
        }

        // ユニットランクを上げるためのコストを算出
        public int RankUpCost(Unit u)
        {
            int RankUpCostRet = default;
            {
                // これ以上改造できない？
                if (u.Rank >= MaxRank(u))
                {
                    RankUpCostRet = 999999999;
                    return RankUpCostRet;
                }

                // 合体状態にある場合はそれが主形態でない限り改造不可
                if (u.IsFeatureAvailable("分離"))
                {
                    if (GeneralLib.LLength(u.FeatureData("分離")) == 3
                        && !u.IsFeatureAvailable("主形態") || u.IsFeatureAvailable("制限時間"))
                    {
                        RankUpCostRet = 999999999;
                        return RankUpCostRet;
                    }
                }

                if (Expression.IsOptionDefined("低改造費"))
                {
                    // 低改造費の場合
                    switch (u.Rank)
                    {
                        case 0:
                            {
                                RankUpCostRet = 10000;
                                break;
                            }

                        case 1:
                            {
                                RankUpCostRet = 15000;
                                break;
                            }

                        case 2:
                            {
                                RankUpCostRet = 20000;
                                break;
                            }

                        case 3:
                            {
                                RankUpCostRet = 30000;
                                break;
                            }

                        case 4:
                            {
                                RankUpCostRet = 40000;
                                break;
                            }

                        case 5:
                            {
                                RankUpCostRet = 50000;
                                break;
                            }

                        case 6:
                            {
                                RankUpCostRet = 60000;
                                break;
                            }

                        case 7:
                            {
                                RankUpCostRet = 70000;
                                break;
                            }

                        case 8:
                            {
                                RankUpCostRet = 80000;
                                break;
                            }

                        case 9:
                            {
                                RankUpCostRet = 100000;
                                break;
                            }

                        case 10:
                            {
                                RankUpCostRet = 120000;
                                break;
                            }

                        case 11:
                            {
                                RankUpCostRet = 140000;
                                break;
                            }

                        case 12:
                            {
                                RankUpCostRet = 160000;
                                break;
                            }

                        case 13:
                            {
                                RankUpCostRet = 180000;
                                break;
                            }

                        case 14:
                            {
                                RankUpCostRet = 200000;
                                break;
                            }

                        default:
                            {
                                RankUpCostRet = 999999999;
                                return RankUpCostRet;
                            }
                    }
                }
                else if (Expression.IsOptionDefined("１５段階改造"))
                {
                    // 通常の１５段改造
                    // (１０段改造時よりお求め安い価格になっております……)
                    switch (u.Rank)
                    {
                        case 0:
                            {
                                RankUpCostRet = 10000;
                                break;
                            }

                        case 1:
                            {
                                RankUpCostRet = 15000;
                                break;
                            }

                        case 2:
                            {
                                RankUpCostRet = 20000;
                                break;
                            }

                        case 3:
                            {
                                RankUpCostRet = 40000;
                                break;
                            }

                        case 4:
                            {
                                RankUpCostRet = 80000;
                                break;
                            }

                        case 5:
                            {
                                RankUpCostRet = 120000;
                                break;
                            }

                        case 6:
                            {
                                RankUpCostRet = 160000;
                                break;
                            }

                        case 7:
                            {
                                RankUpCostRet = 200000;
                                break;
                            }

                        case 8:
                            {
                                RankUpCostRet = 250000;
                                break;
                            }

                        case 9:
                            {
                                RankUpCostRet = 300000;
                                break;
                            }

                        case 10:
                            {
                                RankUpCostRet = 350000;
                                break;
                            }

                        case 11:
                            {
                                RankUpCostRet = 400000;
                                break;
                            }

                        case 12:
                            {
                                RankUpCostRet = 450000;
                                break;
                            }

                        case 13:
                            {
                                RankUpCostRet = 500000;
                                break;
                            }

                        case 14:
                            {
                                RankUpCostRet = 550000;
                                break;
                            }

                        default:
                            {
                                RankUpCostRet = 999999999;
                                return RankUpCostRet;
                            }
                    }
                }
                else
                {
                    // 通常の１０段改造
                    switch (u.Rank)
                    {
                        case 0:
                            {
                                RankUpCostRet = 10000;
                                break;
                            }

                        case 1:
                            {
                                RankUpCostRet = 15000;
                                break;
                            }

                        case 2:
                            {
                                RankUpCostRet = 20000;
                                break;
                            }

                        case 3:
                            {
                                RankUpCostRet = 40000;
                                break;
                            }

                        case 4:
                            {
                                RankUpCostRet = 80000;
                                break;
                            }

                        case 5:
                            {
                                RankUpCostRet = 150000;
                                break;
                            }

                        case 6:
                            {
                                RankUpCostRet = 200000;
                                break;
                            }

                        case 7:
                            {
                                RankUpCostRet = 300000;
                                break;
                            }

                        case 8:
                            {
                                RankUpCostRet = 400000;
                                break;
                            }

                        case 9:
                            {
                                RankUpCostRet = 500000;
                                break;
                            }

                        default:
                            {
                                RankUpCostRet = 999999999;
                                return RankUpCostRet;
                            }
                    }
                }

                // ユニット用特殊能力「改造費修正」による修正
                if (u.IsFeatureAvailable("改造費修正"))
                {
                    RankUpCostRet = (int)(RankUpCostRet * (1d + u.FeatureLevel("改造費修正") / 10d));
                }
            }

            return RankUpCostRet;
        }

        // ユニットの最大改造数を算出
        public int MaxRank(Unit u)
        {

            int MaxRankRet = default;
            if (Expression.IsOptionDefined("５段階改造"))
            {
                // ５段階改造までしか出来ない
                MaxRankRet = 5;
            }
            else if (Expression.IsOptionDefined("１５段階改造"))
            {
                // １５段階改造まで可能
                MaxRankRet = 15;
            }
            else
            {
                // デフォルトは１０段階まで
                MaxRankRet = 10;
            }
            // Disableコマンドで改造不可にされている？
            if (Expression.IsGlobalVariableDefined("Disable(" + u.Name + ",改造)"))
            {
                MaxRankRet = 0;
                return MaxRankRet;
            }

            // 最大改造数が設定されている？
            if (u.IsFeatureAvailable("最大改造数"))
            {
                MaxRankRet = GeneralLib.MinLng(MaxRankRet, (int)u.FeatureLevel("最大改造数"));
            }

            return MaxRankRet;
        }

        // 乗り換えコマンド
        public void ExchangeUnitCommand()
        {
            throw new NotImplementedException();
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

            //            if (withBlock.Party != "味方" || withBlock.Away || localIsGlobalVariableDefined())
            //            {
            //                goto NextLoop;
            //            }

            //            // 追加パイロット＆サポートは乗り換え不可
            //            if (withBlock.IsAdditionalPilot || withBlock.IsAdditionalSupport)
            //            {
            //                goto NextLoop;
            //            }

            //            is_support = false;
            //            if (withBlock.Unit is object)
            //            {
            //                // サポートが複数乗っている場合は乗り降り不可
            //                if (withBlock.Unit.CountSupport() > 1)
            //                {
            //                    goto NextLoop;
            //                }

            //                // サポートパイロットとして乗り込んでいるかを判定
            //                if (withBlock.Unit.CountSupport() == 1)
            //                {
            //                    if ((withBlock.ID ?? "") == (withBlock.Unit.Support(1).ID ?? ""))
            //                    {
            //                        is_support = true;
            //                    }
            //                }

            //                // 通常のパイロットの場合
            //                if (!is_support)
            //                {
            //                    // ３人乗り以上は乗り降り不可
            //                    if (withBlock.Unit.Data.PilotNum != 1 && Math.Abs(withBlock.Unit.Data.PilotNum) != 2)
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

            //                list[Information.UBound(list)] = GeneralLib.RightPaddedString("*" + withBlock.get_Nickname(false), 25) + localLeftPaddedString();
            //                if (withBlock.Unit is object)
            //                {
            //                    {
            //                        var withBlock1 = withBlock.Unit;
            //                        // ユニットのステータス
            //                        string localRightPaddedString() { string argbuf = withBlock1.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 29); withBlock1.Nickname0 = argbuf; return ret; }

            //                        list[Information.UBound(list)] = list[Information.UBound(list)] + "  " + localRightPaddedString() + "(" + withBlock1.MainPilot().get_Nickname(false) + ")";

            //                        // ユニットが装備しているアイテム一覧
            //                        var loopTo = withBlock1.CountItem();
            //                        for (k = 1; k <= loopTo; k++)
            //                        {
            //                            {
            //                                var withBlock2 = withBlock1.Item(k);
            //                                if ((withBlock2.Class() != "固定" || !withBlock2.IsFeatureAvailable("非表示")) && withBlock2.Part() != "非表示")
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
            //            else if (withBlock.Unit is null)
            //            {
            //                // ユニットに乗っていないパイロットの場合
            //                Array.Resize(list, Information.UBound(list) + 1 + 1);
            //                Array.Resize(id_list, Information.UBound(list) + 1);
            //                Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

            //                // パイロットのステータス
            //                string localLeftPaddedString1() { string argbuf = Strings.StrConv(SrcFormatter.Format(withBlock.Level), VbStrConv.Wide); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //                list[Information.UBound(list)] = GeneralLib.RightPaddedString(" " + withBlock.get_Nickname(false), 25) + localLeftPaddedString1();

            //                // パイロットＩＤを記録しておく
            //                id_list[Information.UBound(list)] = withBlock.ID;
            //            }
            //            else if (withBlock.Unit.CountPilot() <= 2)
            //            {
            //                // 複数乗りのユニットに乗っているパイロットの場合
            //                Array.Resize(list, Information.UBound(list) + 1 + 1);
            //                Array.Resize(id_list, Information.UBound(list) + 1);
            //                Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

            //                // パイロットが足りない？
            //                if (withBlock.Unit.CountPilot() < Math.Abs(withBlock.Unit.Data.PilotNum))
            //                {
            //                    list[Information.UBound(list)] = "-";
            //                }
            //                else
            //                {
            //                    list[Information.UBound(list)] = " ";
            //                }

            //                if (withBlock.Unit.IsFeatureAvailable("追加パイロット"))
            //                {
            //                    pname = withBlock.Unit.MainPilot().get_Nickname(false);
            //                }
            //                else
            //                {
            //                    pname = withBlock.get_Nickname(false);
            //                }

            //                // 複数乗りの場合は何番目のパイロットか表示
            //                if (Math.Abs(withBlock.Unit.Data.PilotNum) > 1)
            //                {
            //                    var loopTo1 = withBlock.Unit.CountPilot();
            //                    for (k = 1; k <= loopTo1; k++)
            //                    {
            //                        if (ReferenceEquals(withBlock.Unit.Pilot(k), p))
            //                        {
            //                            pname = pname + "(" + SrcFormatter.Format(k) + ")";
            //                        }
            //                    }
            //                }

            //                // パイロット＆ユニットのステータス
            //                string localLeftPaddedString2() { string argbuf = Strings.StrConv(SrcFormatter.Format(withBlock.Level), VbStrConv.Wide); var ret = GeneralLib.LeftPaddedString(argbuf, 4); return ret; }

            //                string localRightPaddedString1() { string argbuf = withBlock.Unit.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 29); withBlock.Unit.Nickname0 = argbuf; return ret; }

            //                list[Information.UBound(list)] = list[Information.UBound(list)] + GeneralLib.RightPaddedString(pname, 24) + localLeftPaddedString2() + "  " + localRightPaddedString1();
            //                if (withBlock.Unit.CountSupport() > 0)
            //                {
            //                    list[Information.UBound(list)] = list[Information.UBound(list)] + "(" + withBlock.Unit.Support(1).get_Nickname(false) + ")";
            //                }

            //                // ユニットが装備しているアイテム一覧
            //                {
            //                    var withBlock3 = withBlock.Unit;
            //                    var loopTo2 = withBlock3.CountItem();
            //                    for (k = 1; k <= loopTo2; k++)
            //                    {
            //                        {
            //                            var withBlock4 = withBlock3.Item(k);
            //                            if ((withBlock4.Class() != "固定" || !withBlock4.IsFeatureAvailable("非表示")) && withBlock4.Part() != "非表示")
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
            //                {
            //                    var withBlock6 = withBlock5.Item(tmp[i]);
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
            //    if (Expression.IsOptionDefined("等身大基準"))
            //    {
            //        caption_str = " キャラクター          レベル  ユニット";
            //        ret = GUI.ListBox("キャラクター選択", list, caption_str, "連続表示,コメント");
            //    }
            //    else
            //    {
            //        caption_str = " パイロット            レベル  ユニット";
            //        ret = GUI.ListBox("パイロット選択", list, caption_str, "連続表示,コメント");
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
            //                ret = GUI.ListBox("どれで並べ替えますか？", sort_mode_list, "並べ替えの方法", "連続表示,コメント");
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
            //    p = SRC.PList.Item(tmp1[ret]);

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
            //            if (withBlock8.Party0 != "味方" || withBlock8.Status != "待機")
            //            {
            //                goto NextUnit;
            //            }

            //            if (withBlock8.CountSupport() > 1)
            //            {
            //                if (Strings.InStr(p.Class, "専属サポート") == 0)
            //                {
            //                    goto NextUnit;
            //                }
            //            }

            //            if (ReferenceEquals(u, p.Unit))
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
            //                if (withBlock8.Data.PilotNum != 1 && Math.Abs(withBlock8.Data.PilotNum) != 2)
            //                {
            //                    goto NextUnit;
            //                }
            //            }

            //            if (withBlock8.CountPilot() > 0)
            //            {
            //                if (Expression.IsGlobalVariableDefined("Fix(" + withBlock8.Pilot(1).Name + ")") && !p.IsSupport(u))
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
            //                    list[Information.UBound(list)] = list[Information.UBound(list)] + " (" + withBlock8.Support(1).get_Nickname(false) + ")";
            //                }

            //                // ユニットに装備されているアイテムをコメント欄に列記
            //                var loopTo11 = withBlock8.CountItem();
            //                for (j = 1; j <= loopTo11; j++)
            //                {
            //                    {
            //                        var withBlock9 = withBlock8.Item(j);
            //                        if ((withBlock9.Class() != "固定" || !withBlock9.IsFeatureAvailable("非表示")) && withBlock9.Part() != "非表示")
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
            //                    {
            //                        var withBlock10 = withBlock8.Item(j);
            //                        if ((withBlock10.Class() != "固定" || !withBlock10.IsFeatureAvailable("非表示")) && withBlock10.Part() != "非表示")
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
            //    u = p.Unit;
            //    if (Expression.IsOptionDefined("等身大基準"))
            //    {
            //        caption_str = " ユニット                           キャラクター       " + Expression.Term("ランク", u: null);
            //    }
            //    else
            //    {
            //        caption_str = " ユニット                           パイロット         " + Expression.Term("ランク", u: null);
            //    }

            //    if (u is object)
            //    {
            //        if (u.IsFeatureAvailable("追加パイロット"))
            //        {
            //            ret = GUI.ListBox("乗り換え先選択 ： " + u.MainPilot().get_Nickname(false) + " (" + u.Nickname + ")", list, caption_str, "連続表示,コメント");
            //        }
            //        else
            //        {
            //            ret = GUI.ListBox("乗り換え先選択 ： " + p.get_Nickname(false) + " (" + u.Nickname + ")", list, caption_str, "連続表示,コメント");
            //        }
            //    }
            //    else
            //    {
            //        ret = GUI.ListBox("乗り換え先選択 ： " + p.get_Nickname(false), list, caption_str, "連続表示,コメント");
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
            //                if (Expression.IsOptionDefined("等身大基準"))
            //                {
            //                    sort_mode_type[1] = "名称";
            //                    sort_mode_list[1] = "名称";
            //                    sort_mode_type[2] = "ＨＰ";
            //                    sort_mode_list[2] = Expression.Term("ＨＰ", u: null);
            //                    sort_mode_type[3] = "ＥＮ";
            //                    sort_mode_list[3] = Expression.Term("ＥＮ", u: null);
            //                    sort_mode_type[4] = "装甲";
            //                    sort_mode_list[4] = Expression.Term("装甲", u: null);
            //                    sort_mode_type[5] = "運動性";
            //                    sort_mode_list[5] = Expression.Term("運動性", u: null);
            //                    sort_mode_type[6] = "ユニットランク";
            //                    sort_mode_list[6] = Expression.Term("ランク", u: null);
            //                }
            //                else
            //                {
            //                    sort_mode_type[1] = "ＨＰ";
            //                    sort_mode_list[1] = Expression.Term("ＨＰ", u: null);
            //                    sort_mode_type[2] = "ＥＮ";
            //                    sort_mode_list[2] = Expression.Term("ＥＮ", u: null);
            //                    sort_mode_type[3] = "装甲";
            //                    sort_mode_list[3] = Expression.Term("装甲", u: null);
            //                    sort_mode_type[4] = "運動性";
            //                    sort_mode_list[4] = Expression.Term("運動性", u: null);
            //                    sort_mode_type[5] = "ユニットランク";
            //                    sort_mode_list[5] = Expression.Term("ランク", u: null);
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
            //                ret = GUI.ListBox("どれで並べ替えますか？", sort_mode_list, "並べ替えの方法", "連続表示,コメント");
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
            //    u = SRC.UList.Item(tmp2[ret]);

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
            //                withBlock13.Pilot(1).GetOff();
            //            }
            //        }
            //        else
            //        {
            //            // サポートパイロット
            //            var loopTo25 = withBlock13.CountSupport();
            //            for (i = 1; i <= loopTo25; i++)
            //            {
            //                withBlock13.Support(1).GetOff();
            //            }
            //        }
            //    }

            //    Unit localItem7() { var tmp = id_list; object argIndex1 = tmp[ret]; var ret = SRC.UList.Item(argIndex1); return ret; }

            //    p.Ride(localItem7());
            //    goto Beginning;
        }

        // アイテム交換コマンド
        public void ExchangeItemCommand(Unit selected_unit = null, string selected_part = "")
        {
            throw new NotImplementedException();
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
            //    if (Expression.IsOptionDefined("等身大基準"))
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
            //            if (withBlock.Party0 != "味方" || withBlock.Status != "待機")
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
            //                {
            //                    var withBlock1 = withBlock.Item(i);
            //                    if ((withBlock1.Class() != "固定" || !withBlock1.IsFeatureAvailable("非表示")) && withBlock1.Part() != "非表示")
            //                    {
            //                        GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + withBlock1.Nickname() + " ";
            //                        if (withBlock1.Part() == "強化パーツ" || withBlock1.Part() == "アイテム")
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
            //            if (Expression.IsOptionDefined("等身大基準"))
            //            {
            //                list[Information.UBound(list)] = GeneralLib.RightPaddedString(withBlock.Nickname0, 39);
            //                withBlock.Nickname0 = argbuf;
            //            }
            //            else
            //            {
            //                list[Information.UBound(list)] = GeneralLib.RightPaddedString(withBlock.Nickname0, 31);
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

            //            if (Expression.IsOptionDefined("等身大基準"))
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
            //            if (!Expression.IsOptionDefined("等身大基準"))
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
            //    if (Expression.IsOptionDefined("等身大基準"))
            //    {
            //        ret = GUI.ListBox("アイテムを交換するユニットを選択", list, "ユニット                               アイテム " + Expression.Term("RK", null, 2) + " Lv  " + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " " + Expression.Term("装甲", null3, 4) + " " + Expression.Term("RK"4, u: null), "連続表示,コメント");
            //    }
            //    else
            //    {
            //        ret = GUI.ListBox("アイテムを交換するユニットを選択", list, "ユニット                       アイテム " + Expression.Term("RK", null, 2) + "  " + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " " + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", null, 4) + " パイロット", "連続表示,コメント");
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
            //                if (Expression.IsOptionDefined("等身大基準"))
            //                {
            //                    sort_mode_type[1] = "名称";
            //                    sort_mode_list[1] = "名称";
            //                    sort_mode_type[2] = "レベル";
            //                    sort_mode_list[2] = "レベル";
            //                    sort_mode_type[3] = "ＨＰ";
            //                    sort_mode_list[3] = Expression.Term("ＨＰ", u: null);
            //                    sort_mode_type[4] = "ＥＮ";
            //                    sort_mode_list[4] = Expression.Term("ＥＮ", u: null);
            //                    sort_mode_type[5] = "装甲";
            //                    sort_mode_list[5] = Expression.Term("装甲", u: null);
            //                    sort_mode_type[6] = "運動性";
            //                    sort_mode_list[6] = Expression.Term("運動性", u: null);
            //                    sort_mode_type[7] = "ユニットランク";
            //                    sort_mode_list[7] = Expression.Term("ランク", u: null);
            //                }
            //                else
            //                {
            //                    sort_mode_type[1] = "ＨＰ";
            //                    sort_mode_list[1] = Expression.Term("ＨＰ", u: null);
            //                    sort_mode_type[2] = "ＥＮ";
            //                    sort_mode_list[2] = Expression.Term("ＥＮ", u: null);
            //                    sort_mode_type[3] = "装甲";
            //                    sort_mode_list[3] = Expression.Term("装甲", u: null);
            //                    sort_mode_type[4] = "運動性";
            //                    sort_mode_list[4] = Expression.Term("運動性", u: null);
            //                    sort_mode_type[5] = "ユニットランク";
            //                    sort_mode_list[5] = Expression.Term("ランク", u: null);
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
            //                ret = GUI.ListBox("どれで並べ替えますか？", sort_mode_list, "並べ替えの方法", "連続表示,コメント");
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
            //    u = SRC.UList.Item(tmp2[ret]);
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
            //            if (withBlock7.IsFeatureAvailable("装備個所"))
            //            {
            //                buf = withBlock7.FeatureData("装備個所");
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
            //                if (withBlock7.Feature(i) == "ハードポイント")
            //                {
            //                    ipart = withBlock7.FeatureData(i);
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
            //                    if ((tmp_part_list[i] ?? "") == (selected_part ?? "") || (selected_part == "片手" || selected_part == "両手" || selected_part == "盾") && (tmp_part_list[i] == "右手" || tmp_part_list[i] == "左手") || (selected_part == "肩" || selected_part == "両肩") && (tmp_part_list[i] == "右肩" || tmp_part_list[i] == "左肩") || (selected_part == "アイテム" || selected_part == "強化パーツ") && (tmp_part_list[i] == "アイテム" || tmp_part_list[i] == "強化パーツ"))
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
            //                {
            //                    var withBlock8 = withBlock7.Item(i);
            //                    if (withBlock8.Class() == "固定" && withBlock8.IsFeatureAvailable("非表示"))
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
            //                                if (withBlock8.Part() == "強化パーツ" || withBlock8.Part() == "アイテム")
            //                                {
            //                                    var loopTo23 = Information.UBound(part_list);
            //                                    for (j = 1; j <= loopTo23; j++)
            //                                    {
            //                                        if ((part_list[j] == "強化パーツ" || part_list[j] == "アイテム") && string.IsNullOrEmpty(part_item[j]))
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
            //                                        if ((part_list[j] ?? "") == (withBlock8.Part() ?? "") && string.IsNullOrEmpty(part_item[j]))
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

            //                                if (j > Information.UBound(part_list) && string.IsNullOrEmpty(selected_part))
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
            //                            list[i] = GeneralLib.RightPaddedString("----", 23) + part_list[i];
            //                            break;
            //                        }

            //                    case ":":
            //                        {
            //                            list[i] = GeneralLib.RightPaddedString(" :  ", 23) + part_list[i];
            //                            GUI.ListItemComment[i] = GUI.ListItemComment[i - 1];
            //                            GUI.ListItemFlag[i] = GUI.ListItemFlag[i - 1];
            //                            break;
            //                        }

            //                    default:
            //                        {
            //                            var tmp3 = part_item;
            //                            {
            //                                var withBlock9 = SRC.IList.Item(tmp3[i]);
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

            //                                if (localIsGlobalVariableDefined() || withBlock9.Class() == "固定" || withBlock9.IsFeatureAvailable("呪い"))
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
            //            if (withBlock7.CountPilot() > 0 && !Expression.IsOptionDefined("等身大基準"))
            //            {
            //                caption_str = caption_str + " (" + withBlock7.MainPilot().get_Nickname(false) + ")";
            //            }

            //            caption_str = caption_str + "  " + Expression.Term("ＨＰ", u) + "=" + SrcFormatter.Format(withBlock7.MaxHP) + " " + Expression.Term("ＥＮ", u) + "=" + SrcFormatter.Format(withBlock7.MaxEN) + " " + Expression.Term("装甲", u) + "=" + SrcFormatter.Format(withBlock7.get_Armor("")) + " " + Expression.Term("運動性", u) + "=" + SrcFormatter.Format(withBlock7.get_Mobility("")) + " " + Expression.Term("移動力", u) + "=" + SrcFormatter.Format(withBlock7.Speed);
            //            GUI.TopItem = top_item2;
            //            ret = GUI.ListBox(caption_str, list, "アイテム               分類", "連続表示,コメント");
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
            //                if (withBlock7.CountPilot() > 0 && !Expression.IsOptionDefined("等身大基準"))
            //                {
            //                    caption_str = caption_str + " (" + withBlock7.MainPilot().get_Nickname(false) + ")";
            //                }

            //                caption_str = caption_str + "  " + Expression.Term("ＨＰ", u) + "=" + SrcFormatter.Format(withBlock7.MaxHP) + " " + Expression.Term("ＥＮ", u) + "=" + SrcFormatter.Format(withBlock7.MaxEN) + " " + Expression.Term("装甲", u) + "=" + SrcFormatter.Format(withBlock7.get_Armor("")) + " " + Expression.Term("運動性", u) + "=" + SrcFormatter.Format(withBlock7.get_Mobility("")) + " " + Expression.Term("移動力", u) + "=" + SrcFormatter.Format(withBlock7.Speed);
            //                ret = GUI.ListBox(caption_str, list, "アイテム               分類", "連続表示,コメント");
            //                if (ret != 0)
            //                {
            //                    if (ret < Information.UBound(list))
            //                    {
            //                        // 指定されたアイテムを外す
            //                        if (!string.IsNullOrEmpty(id_list[ret]))
            //                        {
            //                            var tmp4 = id_list;
            //                            withBlock7.DeleteItem(tmp4[ret], false);
            //                        }
            //                        else if (GeneralLib.LIndex(list[ret], 1) == ":")
            //                        {
            //                            var tmp5 = id_list;
            //                            withBlock7.DeleteItem(tmp5[ret - 1], false);
            //                        }
            //                    }
            //                    else
            //                    {
            //                        // 全てのアイテムを外す
            //                        var loopTo30 = (Information.UBound(list) - 1);
            //                        for (i = 1; i <= loopTo30; i++)
            //                        {
            //                            if (!GUI.ListItemFlag[i] && !string.IsNullOrEmpty(id_list[i]))
            //                            {
            //                                var tmp6 = id_list;
            //                                withBlock7.DeleteItem(tmp6[i], false);
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
            //                            {
            //                                var withBlock10 = withBlock7.Item(i);
            //                                if (withBlock10.Part() == "片手")
            //                                {
            //                                    bool localIsGlobalVariableDefined1() { string argvname = "Fix(" + withBlock10.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

            //                                    if (localIsGlobalVariableDefined1() || withBlock10.Class() == "固定" || withBlock10.IsFeatureAvailable("呪い"))
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

            //                                    if (localIsGlobalVariableDefined2() || withBlock10.Class() == "固定" || withBlock10.IsFeatureAvailable("呪い"))
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
            //                            {
            //                                var withBlock11 = withBlock7.Item(i);
            //                                if (withBlock11.Part() == "肩")
            //                                {
            //                                    bool localIsGlobalVariableDefined3() { string argvname = "Fix(" + withBlock11.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

            //                                    if (localIsGlobalVariableDefined3() || withBlock11.Class() == "固定" || withBlock11.IsFeatureAvailable("呪い"))
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
            //                            {
            //                                var withBlock12 = withBlock7.Item(i);
            //                                if (withBlock12.Part() == "強化パーツ" || withBlock12.Part() == "アイテム")
            //                                {
            //                                    bool localIsGlobalVariableDefined4() { string argvname = "Fix(" + withBlock12.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

            //                                    if (localIsGlobalVariableDefined4() || withBlock12.Class() == "固定" || withBlock12.IsFeatureAvailable("呪い"))
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

            //                            if (localFeature() == "ハードポイント" && (localFeatureData() ?? "") == (ipart ?? ""))
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
            //                            {
            //                                var withBlock13 = withBlock7.Item(i);
            //                                if ((withBlock13.Part() ?? "") == (ipart ?? ""))
            //                                {
            //                                    bool localIsGlobalVariableDefined5() { string argvname = "Fix(" + withBlock13.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

            //                                    if (localIsGlobalVariableDefined5() || withBlock13.Class() == "固定" || withBlock13.IsFeatureAvailable("呪い"))
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
            //                                                if (!is_right_hand_available || !is_left_hand_available)
            //                                                {
            //                                                    goto NextItem;
            //                                                }

            //                                                break;
            //                                            }

            //                                        case "片手":
            //                                            {
            //                                                if (u.IsFeatureAvailable("両手持ち"))
            //                                                {
            //                                                    if (!is_right_hand_available && !is_left_hand_available)
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
            //                                                if (!is_right_hand_available || !is_left_hand_available)
            //                                                {
            //                                                    goto NextItem;
            //                                                }

            //                                                break;
            //                                            }

            //                                        case "片手":
            //                                            {
            //                                                if (u.IsFeatureAvailable("両手持ち"))
            //                                                {
            //                                                    if (!is_right_hand_available && !is_left_hand_available)
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
            //                                    if (withBlock14.Part() != "両肩" && withBlock14.Part() != "肩")
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
            //                                    if (withBlock14.Part() != "強化パーツ" && withBlock14.Part() != "アイテム")
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

            //                        if (withBlock14.Unit is object)
            //                        {
            //                            {
            //                                var withBlock15 = withBlock14.Unit.CurrentForm();
            //                                // 離脱したユニットが装備している
            //                                if (withBlock15.Status == "離脱")
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
            //                            if (withBlock14.IsFeatureAvailable("呪い"))
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
            //                    {
            //                        var withBlock16 = SRC.IDList.Item(iname);
            //                        string localRightPaddedString() { string argbuf = withBlock16.Nickname; var ret = GeneralLib.RightPaddedString(argbuf, 22); withBlock16.Nickname = argbuf; return ret; }

            //                        list[i] = localRightPaddedString() + " ";
            //                        if (withBlock16.IsFeatureAvailable("大型アイテム"))
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
            //                                        if (withBlock17.Unit is null)
            //                                        {
            //                                            inum = (inum + 1);
            //                                            inum2 = (inum2 + 1);
            //                                        }
            //                                        else if (withBlock17.Unit.CurrentForm().Status != "離脱")
            //                                        {
            //                                            if (!withBlock17.IsFeatureAvailable("呪い"))
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
            //                if (withBlock7.CountPilot() > 0 && !Expression.IsOptionDefined("等身大基準"))
            //                {
            //                    caption_str = caption_str + " (" + withBlock7.MainPilot().get_Nickname(false) + ")";
            //                }

            //                caption_str = caption_str + "  " + Expression.Term("ＨＰ", u) + "=" + SrcFormatter.Format(withBlock7.MaxHP) + " " + Expression.Term("ＥＮ", u) + "=" + SrcFormatter.Format(withBlock7.MaxEN) + " " + Expression.Term("装甲", u) + "=" + SrcFormatter.Format(withBlock7.get_Armor("")) + " " + Expression.Term("運動性", u) + "=" + SrcFormatter.Format(withBlock7.get_Mobility("")) + " " + Expression.Term("移動力", u) + "=" + SrcFormatter.Format(withBlock7.Speed);
            //                ret = GUI.ListBox(caption_str, list, "アイテム               分類            数量", "連続表示,コメント");

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
            //                        if ((withBlock18.Name ?? "") == (iname ?? "") && withBlock18.Exist)
            //                        {
            //                            if (withBlock18.Unit is null)
            //                            {
            //                                // 未装備の装備が見つかったのでそれを装備
            //                                if (!string.IsNullOrEmpty(iid))
            //                                {
            //                                    u.DeleteItem(iid);
            //                                }
            //                                // 呪いのアイテムを装備……
            //                                if (withBlock18.IsFeatureAvailable("呪い"))
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

            //                if (!localItem7().IsFeatureAvailable("呪い"))
            //                {
            //                    foreach (Item currentIt3 in SRC.IList)
            //                    {
            //                        it = currentIt3;
            //                        if ((it.Name ?? "") != (iname ?? "") || !it.Exist)
            //                        {
            //                            goto NextItem2;
            //                        }

            //                        if (it.Unit is null)
            //                        {
            //                            goto NextItem2;
            //                        }

            //                        {
            //                            var withBlock19 = it.Unit.CurrentForm();
            //                            if (withBlock19.Status == "離脱")
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
            //                            if (!Expression.IsOptionDefined("等身大基準") && withBlock19.CountPilot() > 0)
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
            //                                {
            //                                    var withBlock20 = withBlock19.Item(i);
            //                                    if ((withBlock20.Class() != "固定" || !withBlock20.IsFeatureAvailable("非表示")) && withBlock20.Part() != "非表示")
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
            //                if (withBlock7.CountPilot() > 0 && !Expression.IsOptionDefined("等身大基準"))
            //                {
            //                    caption_str = caption_str + " (" + withBlock7.MainPilot().get_Nickname(false) + ")";
            //                }

            //                caption_str = caption_str + "  " + Expression.Term("ＨＰ", u) + "=" + SrcFormatter.Format(withBlock7.MaxHP) + " " + Expression.Term("ＥＮ", u) + "=" + SrcFormatter.Format(withBlock7.MaxEN) + " " + Expression.Term("装甲", u) + "=" + SrcFormatter.Format(withBlock7.get_Armor("")) + " " + Expression.Term("運動性", u) + "=" + SrcFormatter.Format(withBlock7.get_Mobility("")) + " " + Expression.Term("移動力", u) + "=" + SrcFormatter.Format(withBlock7.Speed);
            //                GUI.TopItem = 1;
            //                if (Expression.IsOptionDefined("等身大基準"))
            //                {
            //                    ret = GUI.ListBox(caption_str, list, "ユニット", "連続表示,コメント");
            //                }
            //                else
            //                {
            //                    ret = GUI.ListBox(caption_str, list, "ユニット                             パイロット", "連続表示,コメント");
            //                }

            //                // アイテムを交換
            //                if (ret > 0)
            //                {
            //                    if (!string.IsNullOrEmpty(iid))
            //                    {
            //                        withBlock7.DeleteItem(iid);
            //                    }

            //                    var tmp7 = id_list;
            //                    {
            //                        var withBlock21 = SRC.IList.Item(tmp7[ret]);
            //                        if (withBlock21.Unit is object)
            //                        {
            //                            withBlock21.Unit.DeleteItem(withBlock21.ID);
            //                        }

            //                        // 呪いのアイテムを装備……
            //                        if (withBlock21.IsFeatureAvailable("呪い"))
            //                        {
            //                            Interaction.MsgBox(withBlock21.Nickname() + "は呪われていた！");
            //                        }
            //                    }

            //                    Item localItem9() { var tmp = id_list; object argIndex1 = tmp[ret]; var ret = SRC.IList.Item(argIndex1); return ret; }

            //                    withBlock7.AddItem(localItem9());
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
        }

        // 換装コマンド
        private void ExchangeFormCommand()
        {
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
        Beginning:
            ;
            var top_item = 1;

            // 換装可能なユニットのリストを作成
            var units = SRC.UList.Items
                // 待機中の味方ユニット？
                .Where(u => !(u.Party0 != "味方" || u.Status != "待機"))
                // 換装能力を持っている？
                .Where(u => u.IsFeatureAvailable("換装"))
                // いずれかの形態に換装可能？
                .Where(u => u.Feature("換装").DataL.Any(uname => u.OtherForm(uname)?.IsAvailable() ?? false))
                // リストをユニットのＨＰでソート
                .OrderByDescending(u => u.MaxHP)
                .Select(u =>
                {
                    // ユニットのステータスを表示
                    var msg = "";
                    // 等身大基準の場合はパイロットレベルも表示
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        msg = GeneralLib.RightPaddedString(u.Nickname0, 37)
                                 + GeneralLib.LeftPaddedString("" + u.Rank, 2);
                    }
                    else
                    {
                        msg = GeneralLib.RightPaddedString(u.Nickname0, 33)
                                 + GeneralLib.LeftPaddedString("" + u.Rank, 2);
                    }
                    // ユニットに関する情報
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxHP), 6);
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxEN), 5);
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Armor("")), 5);
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Mobility("")), 5);
                    if (u.CountPilot() > 0)
                    {
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MainPilot().Level), 6);
                        // 等身大基準でない場合はパイロット名を表示
                        if (!Expression.IsOptionDefined("等身大基準"))
                        {
                            msg += "  " + u.MainPilot().get_Nickname(false);
                        }
                    }

                    // 装備しているアイテムをコメント欄に列記
                    var comment = string.Join(" ", u.ItemList
                        .Where(itm => (itm.Class() != "固定" || !itm.IsFeatureAvailable("非表示")) && itm.Part() != "非表示")
                        .Select(itm => itm.Nickname()));

                    return new ListBoxItem(msg, u.ID)
                    {
                        ListItemComment = comment,
                    };
                })
                .ToList();

            // 換装するユニットを選択
            string caption = "ユニット選択";
            string info;
            if (Expression.IsOptionDefined("等身大基準"))
            {
                info = "ユニット                         "
                    + Expression.Term("ランク", null, 6) + "  費用 Lv  "
                    + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                    + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", u: null) + " レベル";
            }
            else
            {
                info = "ユニット                     "
                    + Expression.Term("ランク", null, 6) + "  費用  "
                    + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                    + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", u: null) + " パイロット";
            }
            GUI.TopItem = top_item;
            var ret = GUI.ListBox(new ListBoxArgs
            {
                Items = units.ToList(),
                lb_caption = caption,
                lb_info = info,
                lb_mode = "連続表示,コメント",
            });
            top_item = GUI.TopItem;

            // キャンセル？
            if (ret == 0)
            {
                return;
            }

            // 選択されたユニットを検索
            // 換装可能な形態のリストを作成
            {
                var baseU = SRC.UList.Item(units[ret - 1].ListItemID);
                // 換装先のリストを作成
                var tfUnits = baseU.Feature("換装").DataL.Select(uname => baseU.OtherForm(uname))
                    .Where(u => u?.IsAvailable() ?? false)
                    .Select(u =>
                    {
                        // ユニットランクを合わせる
                        u.Rank = baseU.Rank;
                        u.Rank = baseU.Rank;
                        u.Update();
                        // ユニットのステータスを表示
                        var msg = "";
                        msg += (u.Nickname == baseU.Nickname0)
                            ? GeneralLib.RightPaddedString(u.Name, 27)
                            : GeneralLib.RightPaddedString(u.Nickname0, 27);
                        // ユニットに関する情報
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxHP), 6);
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxEN), 5);
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Armor("")), 5);
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Mobility("")), 5);

                        var weapons = u.Weapons.Where(uw => uw.IsDisplayFor(WeaponListMode.List)).ToList();
                        // 最大攻撃力
                        var maxPower = weapons.Max(uw => uw.WeaponPower(""));
                        msg += GeneralLib.LeftPaddedString("" + maxPower, 7);
                        // 最大射程
                        var maxRange = weapons.Max(uw => uw.WeaponMaxRange());
                        msg += GeneralLib.LeftPaddedString("" + maxRange, 5);

                        // 換装先が持つ特殊能力一覧
                        var comment = string.Join(" ", u.Features
                            .Select(fd => fd.FeatureName(u))
                            .Where(x => !string.IsNullOrEmpty(x))
                            .Distinct());
                        return new ListBoxItem(msg, u.ID)
                        {
                            ListItemComment = comment,
                        };
                    })
                    .ToList();

                // 換装先の形態を選択
                GUI.TopItem = 1;
                var tfRet = GUI.ListBox(new ListBoxArgs
                {
                    Items = tfUnits.ToList(),
                    lb_caption = "変更先選択",
                    lb_info = "ユニット                     "
                        + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                        + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", u: null) + " 適応 攻撃力 射程",
                    lb_mode = "連続表示,コメント",
                });
                // キャンセル？
                if (tfRet == 0)
                {
                    goto Beginning;
                }

                // 換装を実施
                baseU.Transform(tfUnits[tfRet - 1].ListItemID);
            }

            goto Beginning;
        }

        // ステータスコマンド中かどうかを返す
        public bool InStatusCommand()
        {
            throw new NotImplementedException();
            //    bool InStatusCommandRet = default;
            //    if (string.IsNullOrEmpty(Map.MapFileName))
            //    {
            //        if (Strings.InStr(SRC.ScenarioFileName, @"\ユニットステータス表示.eve") > 0 || Strings.InStr(SRC.ScenarioFileName, @"\パイロットステータス表示.eve") > 0 || SRC.IsSubStage)
            //        {
            //            InStatusCommandRet = true;
            //        }
            //    }

            //    return InStatusCommandRet;
        }
    }
}
