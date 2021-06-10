// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Items;
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
                        ListItemComment = comment,
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
            var top_item = 1;
            // デフォルトのソート方法
            var sort_mode = "レベル";
            var sort_mode2 = "名称";
        Beginning:
            ;


            // 乗り換えるパイロットの一覧を作成
            var list = new List<ListBoxItem>();
            list.Add(new ListBoxItem("▽並べ替え▽", "▽並べ替え▽"));

            var pilots = SRC.PList.Items
                .Where(p => !(p.Party != "味方" || p.Away || p.IsFix))
                // 追加パイロット＆サポートは乗り換え不可
                .Where(p => !(p.IsAdditionalPilot || p.IsAdditionalSupport))
                .Where(p =>
                {
                    if (p.Unit != null)
                    {
                        // サポートが複数乗っている場合は乗り降り不可
                        if (p.Unit.CountSupport() > 1)
                        {
                            return false;
                        }

                        if (!p.IsRidingAdSupport)
                        {
                            // ３人乗り以上は乗り降り不可
                            if (Math.Abs(p.Unit.Data.PilotNum) >= 3)
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                })
                .Select(p =>
                {
                    var msg = "";
                    if (p.IsRidingAdSupport)
                    {
                        // サポートパイロットの場合
                        // パイロットのステータス
                        msg = GeneralLib.RightPaddedString("*" + p.get_Nickname(false), 25) + GeneralLib.LeftPaddedString("" + p.Level, 4);

                        if (p.Unit is object)
                        {
                            msg += "  " + GeneralLib.RightPaddedString(p.Unit.Nickname0, 29) + "(" + p.Unit.MainPilot().get_Nickname(false) + ")";
                        }
                    }
                    else if (p.Unit is null)
                    {
                        // ユニットに乗っていないパイロットの場合
                        // パイロットのステータス
                        msg = GeneralLib.RightPaddedString(" " + p.get_Nickname(false), 25) + GeneralLib.LeftPaddedString("" + p.Level, 4);
                    }
                    else if (p.Unit.CountPilot() <= 2)
                    {
                        // 複数乗りのユニットに乗っているパイロットの場合
                        // パイロットが足りない？
                        if (p.Unit.CountPilot() < Math.Abs(p.Unit.Data.PilotNum))
                        {
                            msg += "-";
                        }
                        else
                        {
                            msg += " ";
                        }

                        string pname;
                        if (p.Unit.IsFeatureAvailable("追加パイロット"))
                        {
                            pname = p.Unit.MainPilot().get_Nickname(false);
                        }
                        else
                        {
                            pname = p.get_Nickname(false);
                        }

                        // 複数乗りの場合は何番目のパイロットか表示
                        if (Math.Abs(p.Unit.Data.PilotNum) > 1)
                        {
                            pname = pname + "(" + (p.Unit.Pilots.IndexOf(p) + 1) + ")";
                        }

                        // パイロット＆ユニットのステータス
                        msg += GeneralLib.RightPaddedString(pname, 24)
                            + GeneralLib.LeftPaddedString("" + p.Level, 4)
                            + "  " + GeneralLib.RightPaddedString(p.Unit.Nickname0, 29);
                        if (p.Unit.Supports.Any())
                        {
                            msg += "(" + p.Unit.Supports.First().get_Nickname(false) + ")";
                        }
                    }

                    // 装備しているアイテムをコメント欄に列記
                    var comment = p.Unit != null
                        ? string.Join(" ", p.Unit.ItemList
                               .Where(itm => (itm.Class() != "固定" || !itm.IsFeatureAvailable("非表示")) && itm.Part() != "非表示")
                               .Select(itm => itm.Nickname()))
                        : "";

                    return new ListBoxItem(msg, p.ID)
                    {
                        ListItemComment = comment,
                    };
                }).ToList();

        SortAgain:
            ;

            // ソート
            pilots = sort_mode == "レベル"
                ? pilots.OrderByDescending(x => SRC.PList.Item(x.ListItemID).Level).ToList()
                : pilots.OrderBy(x => SRC.PList.Item(x.ListItemID).KanaName).ToList();

            // パイロットを選択
            GUI.TopItem = top_item;
            string caption;
            string info;
            if (Expression.IsOptionDefined("等身大基準"))
            {
                caption = " キャラクター          レベル  ユニット";
                info = "キャラクター選択";
            }
            else
            {
                caption = " パイロット            レベル  ユニット";
                info = "パイロット選択";
            }
            var ret = GUI.ListBox(new ListBoxArgs
            {
                Items = list.AppendRange(pilots).ToList(),
                lb_caption = caption,
                lb_info = info,
                lb_mode = "連続表示,コメント",
            });
            top_item = GUI.TopItem;

            switch (ret)
            {
                case 0:
                    // キャンセル
                    return;

                case 1:
                    // ソート方法を選択
                    IList<ListBoxItem> sortItems;
                    sortItems = new List<ListBoxItem>
                            {
                                new ListBoxItem("レベル", "レベル"),
                                new ListBoxItem("名称", "名称"),
                            };
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

                    goto SortAgain;
            }

            {
                // 乗り換えさせるパイロット
                var p = SRC.PList.Item(pilots[ret - 2].ListItemID);

                // 乗り換え先ユニット一覧作成
                var units = SRC.UList.Items
                    .Where(u => !(u.Party0 != "味方" || u.Status != "待機"))
                    .Where(u => !(u.CountSupport() > 1 && Strings.InStr(p.Class, "専属サポート") == 0))
                    .Where(u => u != p.Unit)
                    .Where(u => p.IsAbleToRide(u))
                    // サポートキャラでなければ乗り換えられるパイロット数に制限がある
                    .Where(u => !(!p.IsSupport(u) && u.Data.PilotNum != 1 && Math.Abs(u.Data.PilotNum) != 2))
                    // Fixコマンドでパイロットが固定されたユニットはサポートでない限り乗り換え不可
                    .Where(u => !(u.CountPilot() > 0 && u.Pilots.First().IsFix && !p.IsSupport(u)))
                    // 誰も乗ってないユニットに乗れるのは通常パイロットのみ
                    .Where(u => !(u.CountPilot() == 0 && p.IsSupport(u)))
                    .Select(u =>
                    {
                        var msg = "";
                        if (u.CountPilot() > 0)
                        {
                            // パイロットが足りている？
                            if (u.CountPilot() < Math.Abs(u.Data.PilotNum))
                            {
                                msg += "-";
                            }
                            else
                            {
                                msg += " ";
                            }

                            msg += GeneralLib.RightPaddedString(u.Nickname0, 35)
                                + GeneralLib.RightPaddedString(u.MainPilot().get_Nickname(false), 21)
                                // XXX 全半角の処理
                                + " " + GeneralLib.LeftPaddedString("" + u.Rank, 3);
                            if (u.CountSupport() > 0)
                            {
                                msg += " (" + u.Supports.First().get_Nickname(false) + ")";
                            }
                        }
                        else
                        {
                            msg += " " + GeneralLib.RightPaddedString(u.Nickname0, 35)
                                + Strings.Space(21)
                                // XXX 全半角の処理
                                + " " + GeneralLib.LeftPaddedString("" + u.Rank, 3);
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
            SortAgain2:
                ;


                // ソート
                if (Strings.InStr(sort_mode2, "名称") == 0)
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

                // 乗り換え先を選択
                {
                    GUI.TopItem = 1;
                    var u = p.Unit;
                    string uCaption;
                    string uInfo;
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        uCaption = " ユニット                           キャラクター       " + Expression.Term("ランク", u: null);
                    }
                    else
                    {
                        uCaption = " ユニット                           パイロット         " + Expression.Term("ランク", u: null);
                    }
                    if (u != null)
                    {
                        if (u.IsFeatureAvailable("追加パイロット"))
                        {
                            uInfo = "乗り換え先選択 ： " + u.MainPilot().get_Nickname(false) + " (" + u.Nickname + ")";
                        }
                        else
                        {
                            uInfo = "乗り換え先選択 ： " + p.get_Nickname(false) + " (" + u.Nickname + ")";
                        }
                    }
                    else
                    {
                        uInfo = "乗り換え先選択 ： " + p.get_Nickname(false);
                    }
                    var uRet = GUI.ListBox(new ListBoxArgs
                    {
                        HasFlag = true,
                        Items = list.AppendRange(units).ToList(),
                        lb_caption = uCaption,
                        lb_info = uInfo,
                        lb_mode = "連続表示,コメント",
                    });

                    switch (uRet)
                    {
                        case 0:
                            // XXX return でいいの？
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
                                sort_mode2 = sortItems[sortRes - 1].ListItemID;
                            }
                            goto SortAgain2;
                    }

                    // キャンセル？
                    if (uRet == 0)
                    {
                        goto Beginning;
                    }

                    u = SRC.UList.Item(units[uRet - 2].ListItemID);

                    // 元のユニットから降ろす
                    p.GetOff();

                    // 乗り換え
                    if (!p.IsSupport(u))
                    {
                        // 通常のパイロット
                        if (u.CountPilot() == u.Data.PilotNum)
                        {
                            u.Pilots.First().GetOff();
                        }
                    }
                    else
                    {
                        // サポートパイロット
                        while (u.CountSupport() > 0)
                        {
                            u.Supports.First().GetOff();
                        }
                    }
                    p.Ride(u);
                }
            }
            goto Beginning;
        }

        // アイテム交換コマンド
        public void ExchangeItemCommand(Unit selected_unit = null, string selected_part = "")
        {
            // デフォルトのソート方法
            var top_item1 = 1;
            var top_item2 = 1;
            string sort_mode;
            if (Expression.IsOptionDefined("等身大基準"))
            {
                sort_mode = "レベル";
            }
            else
            {
                sort_mode = "ＨＰ";
            }

            // ユニットがあらかじめ選択されている場合
            // (ユニットステータスからのアイテム交換時)
            Unit selectedUnit;
            if (selected_unit is object)
            {
                GUI.EnlargeListBoxHeight();
                GUI.ReduceListBoxWidth();
                if (GUI.MainFormVisible)
                {
                    if (!ReferenceEquals(selected_unit, SRC.GUIStatus.DisplayedUnit))
                    {
                        SRC.GUIStatus.DisplayUnitStatus(selected_unit);
                    }
                }
                selectedUnit = selected_unit;
                goto MakeEquipedItemList;
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
                    var inum = 0;
                    var inum2 = 0;
                    var comment = "";
                    foreach (var itm in u.ItemList.Where(x => x.Class() != "固定" && !x.IsFeatureAvailable("非表示")))
                    {
                        comment += itm.Nickname() + "";
                        if (itm.Part() == "強化パーツ" || itm.Part() == "アイテム")
                        {
                            inum += itm.Size();
                        }
                        else
                        {
                            inum2 += itm.Size();
                        }
                    }

                    // リストを作成
                    var msg = "";
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        msg = GeneralLib.RightPaddedString(u.Nickname0, 39);
                    }
                    else
                    {
                        msg = GeneralLib.RightPaddedString(u.Nickname0, 31);
                    }
                    msg += $"{inum}/{u.MaxItemNum()}";
                    if (inum2 > 0)
                    {
                        msg += $"({inum2})   ";
                    }
                    else
                    {
                        msg += "      ";
                    }

                    msg += GeneralLib.LeftPaddedString("" + u.Rank, 2);

                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        if (u.CountPilot() > 0)
                        {
                            msg += GeneralLib.LeftPaddedString("" + u.MainPilot().Level, 3);
                        }
                    }
                    msg += GeneralLib.LeftPaddedString("" + u.MaxHP, 6)
                        + GeneralLib.LeftPaddedString("" + u.MaxEN, 4)
                        + GeneralLib.LeftPaddedString("" + u.get_Armor(""), 5)
                        + GeneralLib.LeftPaddedString("" + u.get_Mobility(""), 5);
                    if (!Expression.IsOptionDefined("等身大基準"))
                    {
                        if (u.CountPilot() > 0)
                        {
                            msg += " " + u.MainPilot().get_Nickname(false);
                        }
                    }

                    return new ListBoxItem(msg, u.ID)
                    {
                        ListItemComment = comment,
                    }; ;
                })
                .ToList();

        SortAgain:
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

            // アイテムを交換するユニットを選択
            string caption = "アイテムを交換するユニットを選択";
            string info;
            if (Expression.IsOptionDefined("等身大基準"))
            {
                info = "ユニット                               アイテム "
                    + Expression.Term("RK", null, 2) + " Lv  "
                    + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                    + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", null, 4) + " レベル";
            }
            else
            {
                info = "ユニット                       アイテム "
                    + Expression.Term("RK", null, 2) + "  "
                    + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                    + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", null, 4) + " パイロット";
            }
            GUI.TopItem = top_item1;
            var ret = GUI.ListBox(new ListBoxArgs
            {
                Items = units.ToList(),
                lb_caption = caption,
                lb_info = info,
                lb_mode = "連続表示,コメント",
            });
            top_item1 = GUI.TopItem;

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

            // ユニットを選択
            selectedUnit = SRC.UList.Item(units[ret - 2].ListItemID);

        MakeEquipedItemList:
            ;

            // 選択されたユニットが装備しているアイテム一覧の作成
            string[] tmp_part_list;
            {
                var u = selectedUnit;
                while (true)
                {
                    // アイテムの装備個所一覧を作成
                    var itemSlots = new ItemSlots(u);
                    // 装備個所に現在装備しているアイテムを割り当て
                    itemSlots.FillSlot(u);

                    // 特定の装備個所のアイテムのみを交換する？
                    if (!string.IsNullOrEmpty(selected_part))
                    {
                        itemSlots.Slots = itemSlots.Slots.Where(x => x.IsMatch(selected_part)).ToList();
                    }

                    var comment = "";
                    var inum = 0;
                    var inum2 = 0;
                    foreach (var itm in u.ItemList.Where(x => x.Class() != "固定" && !x.IsFeatureAvailable("非表示")))
                    {
                        comment += itm.Nickname() + "";
                        if (itm.Part() == "強化パーツ" || itm.Part() == "アイテム")
                        {
                            inum += itm.Size();
                        }
                        else
                        {
                            inum2 += itm.Size();
                        }
                    }

                    // リストを構築
                    var equipList = itemSlots.Slots.Select(x => new ListBoxItem()
                    {
                        Text = x.IsEmpty ? GeneralLib.RightPaddedString("----", 23) + x.SlotName
                            : x.IsOccupied ? GeneralLib.RightPaddedString(" :  ", 23) + x.SlotName
                                : GeneralLib.RightPaddedString(x.Item.Nickname(), 22) + " " + x.SlotName,
                        ListItemComment = x.IsEmpty ? "" : x.Item.Data.Comment,
                        ListItemID = x.IsEmpty ? "" : x.Item.ID,
                        ListItemFlag = x.IsEmpty ? true : x.Item.IsFix,
                    })
                    .ToList();
                    //.Append(new ListBoxItem("▽装備解除▽", "▽装備解除▽"))
                    //    // 交換するアイテムを選択
                    //    caption_str = "装備個所を選択 ： " + u.Nickname;
                    //    if (u.CountPilot() > 0 && !Expression.IsOptionDefined("等身大基準"))
                    //    {
                    //        caption_str = caption_str + " (" + u.MainPilot().get_Nickname(false) + ")";
                    //    }

                    //    caption_str = caption_str + "  " + Expression.Term("ＨＰ", selectedUnit) + "=" + SrcFormatter.Format(u.MaxHP) + " " + Expression.Term("ＥＮ", selectedUnit) + "=" + SrcFormatter.Format(u.MaxEN) + " " + Expression.Term("装甲", selectedUnit) + "=" + SrcFormatter.Format(u.get_Armor("")) + " " + Expression.Term("運動性", selectedUnit) + "=" + SrcFormatter.Format(u.get_Mobility("")) + " " + Expression.Term("移動力", selectedUnit) + "=" + SrcFormatter.Format(u.Speed);
                    //    GUI.TopItem = top_item2;
                    //    ret = GUI.ListBox(caption_str, list, "アイテム               分類", "連続表示,コメント");
                    //    top_item2 = GUI.TopItem;
                    //    if (ret == 0)
                    //    {
                    //        break;
                    //    }

                    //    // 装備を解除する場合
                    //    if (ret == Information.UBound(list))
                    //    {
                    //        list[Information.UBound(list)] = "▽全て外す▽";
                    //        caption_str = "外すアイテムを選択 ： " + u.Nickname;
                    //        if (u.CountPilot() > 0 && !Expression.IsOptionDefined("等身大基準"))
                    //        {
                    //            caption_str = caption_str + " (" + u.MainPilot().get_Nickname(false) + ")";
                    //        }

                    //        caption_str = caption_str + "  " + Expression.Term("ＨＰ", selectedUnit) + "=" + SrcFormatter.Format(u.MaxHP) + " " + Expression.Term("ＥＮ", selectedUnit) + "=" + SrcFormatter.Format(u.MaxEN) + " " + Expression.Term("装甲", selectedUnit) + "=" + SrcFormatter.Format(u.get_Armor("")) + " " + Expression.Term("運動性", selectedUnit) + "=" + SrcFormatter.Format(u.get_Mobility("")) + " " + Expression.Term("移動力", selectedUnit) + "=" + SrcFormatter.Format(u.Speed);
                    //        ret = GUI.ListBox(caption_str, list, "アイテム               分類", "連続表示,コメント");
                    //        if (ret != 0)
                    //        {
                    //            if (ret < Information.UBound(list))
                    //            {
                    //                // 指定されたアイテムを外す
                    //                if (!string.IsNullOrEmpty(id_list[ret]))
                    //                {
                    //                    var tmp4 = id_list;
                    //                    u.DeleteItem(tmp4[ret], false);
                    //                }
                    //                else if (GeneralLib.LIndex(list[ret], 1) == ":")
                    //                {
                    //                    var tmp5 = id_list;
                    //                    u.DeleteItem(tmp5[ret - 1], false);
                    //                }
                    //            }
                    //            else
                    //            {
                    //                // 全てのアイテムを外す
                    //                var loopTo30 = (Information.UBound(list) - 1);
                    //                for (i = 1; i <= loopTo30; i++)
                    //                {
                    //                    if (!GUI.ListItemFlag[i] && !string.IsNullOrEmpty(id_list[i]))
                    //                    {
                    //                        var tmp6 = id_list;
                    //                        u.DeleteItem(tmp6[i], false);
                    //                    }
                    //                }
                    //            }

                    //            if (string.IsNullOrEmpty(Map.MapFileName))
                    //            {
                    //                u.FullRecover();
                    //            }

                    //            if (GUI.MainForm.Visible)
                    //            {
                    //                Status.DisplayUnitStatus(selectedUnit);
                    //            }
                    //        }

                    //        goto NextLoop2;
                    //    }

                    //    // 交換するアイテムの装備個所
                    //    iid = id_list[ret];
                    //    if (!string.IsNullOrEmpty(iid))
                    //    {
                    //        Item localItem6() { object argIndex1 = iid; var ret = SRC.IList.Item(argIndex1); return ret; }

                    //        ipart = localItem6().Part();
                    //    }
                    //    else
                    //    {
                    //        ipart = GeneralLib.LIndex(list[ret], 2);
                    //    }

                    //    // 空きスロットを調べておく
                    //    switch (ipart ?? "")
                    //    {
                    //        case "右手":
                    //        case "左手":
                    //        case "片手":
                    //        case "両手":
                    //        case "盾":
                    //            {
                    //                is_right_hand_available = true;
                    //                is_left_hand_available = true;
                    //                var loopTo31 = u.CountItem();
                    //                for (i = 1; i <= loopTo31; i++)
                    //                {
                    //                    {
                    //                        var withBlock10 = u.Item(i);
                    //                        if (withBlock10.Part() == "片手")
                    //                        {
                    //                            bool localIsGlobalVariableDefined1() { string argvname = "Fix(" + withBlock10.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

                    //                            if (localIsGlobalVariableDefined1() || withBlock10.Class() == "固定" || withBlock10.IsFeatureAvailable("呪い"))
                    //                            {
                    //                                if (is_right_hand_available)
                    //                                {
                    //                                    is_right_hand_available = false;
                    //                                }
                    //                                else
                    //                                {
                    //                                    is_left_hand_available = false;
                    //                                }
                    //                            }
                    //                        }
                    //                        else if (withBlock10.Part() == "盾")
                    //                        {
                    //                            bool localIsGlobalVariableDefined2() { string argvname = "Fix(" + withBlock10.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

                    //                            if (localIsGlobalVariableDefined2() || withBlock10.Class() == "固定" || withBlock10.IsFeatureAvailable("呪い"))
                    //                            {
                    //                                is_left_hand_available = false;
                    //                            }
                    //                        }
                    //                    }
                    //                }

                    //                break;
                    //            }

                    //        case "右肩":
                    //        case "左肩":
                    //        case "肩":
                    //            {
                    //                empty_slot = 2;
                    //                var loopTo32 = u.CountItem();
                    //                for (i = 1; i <= loopTo32; i++)
                    //                {
                    //                    {
                    //                        var withBlock11 = u.Item(i);
                    //                        if (withBlock11.Part() == "肩")
                    //                        {
                    //                            bool localIsGlobalVariableDefined3() { string argvname = "Fix(" + withBlock11.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

                    //                            if (localIsGlobalVariableDefined3() || withBlock11.Class() == "固定" || withBlock11.IsFeatureAvailable("呪い"))
                    //                            {
                    //                                empty_slot = (empty_slot - 1);
                    //                            }
                    //                        }
                    //                    }
                    //                }

                    //                break;
                    //            }

                    //        case "強化パーツ":
                    //        case "アイテム":
                    //            {
                    //                empty_slot = u.MaxItemNum();
                    //                var loopTo33 = u.CountItem();
                    //                for (i = 1; i <= loopTo33; i++)
                    //                {
                    //                    {
                    //                        var withBlock12 = u.Item(i);
                    //                        if (withBlock12.Part() == "強化パーツ" || withBlock12.Part() == "アイテム")
                    //                        {
                    //                            bool localIsGlobalVariableDefined4() { string argvname = "Fix(" + withBlock12.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

                    //                            if (localIsGlobalVariableDefined4() || withBlock12.Class() == "固定" || withBlock12.IsFeatureAvailable("呪い"))
                    //                            {
                    //                                empty_slot = (empty_slot - withBlock12.Size());
                    //                            }
                    //                        }
                    //                    }
                    //                }

                    //                break;
                    //            }

                    //        default:
                    //            {
                    //                empty_slot = 0;
                    //                var loopTo34 = u.CountFeature();
                    //                for (i = 1; i <= loopTo34; i++)
                    //                {
                    //                    string localFeature() { object argIndex1 = i; var ret = u.Feature(argIndex1); return ret; }

                    //                    string localFeatureData() { object argIndex1 = i; var ret = u.FeatureData(argIndex1); return ret; }

                    //                    if (localFeature() == "ハードポイント" && (localFeatureData() ?? "") == (ipart ?? ""))
                    //                    {
                    //                        double localFeatureLevel() { object argIndex1 = i; var ret = u.FeatureLevel(argIndex1); return ret; }

                    //                        empty_slot = (empty_slot + localFeatureLevel());
                    //                    }
                    //                }

                    //                if (empty_slot == 0)
                    //                {
                    //                    empty_slot = 1;
                    //                }

                    //                var loopTo35 = u.CountItem();
                    //                for (i = 1; i <= loopTo35; i++)
                    //                {
                    //                    {
                    //                        var withBlock13 = u.Item(i);
                    //                        if ((withBlock13.Part() ?? "") == (ipart ?? ""))
                    //                        {
                    //                            bool localIsGlobalVariableDefined5() { string argvname = "Fix(" + withBlock13.Name + ")"; var ret = Expression.IsGlobalVariableDefined(argvname); return ret; }

                    //                            if (localIsGlobalVariableDefined5() || withBlock13.Class() == "固定" || withBlock13.IsFeatureAvailable("呪い"))
                    //                            {
                    //                                empty_slot = (empty_slot - withBlock13.Size());
                    //                            }
                    //                        }
                    //                    }
                    //                }

                    //                break;
                    //            }
                    //    }

                    //    while (true)
                    //    {
                    //        // 装備可能なアイテムを調べる
                    //        item_list = new string[1];
                    //        foreach (Item currentIt in SRC.IList)
                    //        {
                    //            it = currentIt;
                    //            {
                    //                var withBlock14 = it;
                    //                if (!withBlock14.Exist)
                    //                {
                    //                    goto NextItem;
                    //                }

                    //                // 装備スロットが空いている？
                    //                switch (ipart ?? "")
                    //                {
                    //                    case "右手":
                    //                    case "左手":
                    //                    case "片手":
                    //                    case "両手":
                    //                        {
                    //                            switch (withBlock14.Part() ?? "")
                    //                            {
                    //                                case "両手":
                    //                                    {
                    //                                        if (!is_right_hand_available || !is_left_hand_available)
                    //                                        {
                    //                                            goto NextItem;
                    //                                        }

                    //                                        break;
                    //                                    }

                    //                                case "片手":
                    //                                    {
                    //                                        if (selectedUnit.IsFeatureAvailable("両手持ち"))
                    //                                        {
                    //                                            if (!is_right_hand_available && !is_left_hand_available)
                    //                                            {
                    //                                                goto NextItem;
                    //                                            }
                    //                                        }
                    //                                        else if (!is_right_hand_available)
                    //                                        {
                    //                                            goto NextItem;
                    //                                        }

                    //                                        break;
                    //                                    }

                    //                                case "盾":
                    //                                    {
                    //                                        if (!is_left_hand_available)
                    //                                        {
                    //                                            goto NextItem;
                    //                                        }

                    //                                        break;
                    //                                    }

                    //                                default:
                    //                                    {
                    //                                        goto NextItem;
                    //                                        break;
                    //                                    }
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "盾":
                    //                        {
                    //                            switch (withBlock14.Part() ?? "")
                    //                            {
                    //                                case "両手":
                    //                                    {
                    //                                        if (!is_right_hand_available || !is_left_hand_available)
                    //                                        {
                    //                                            goto NextItem;
                    //                                        }

                    //                                        break;
                    //                                    }

                    //                                case "片手":
                    //                                    {
                    //                                        if (selectedUnit.IsFeatureAvailable("両手持ち"))
                    //                                        {
                    //                                            if (!is_right_hand_available && !is_left_hand_available)
                    //                                            {
                    //                                                goto NextItem;
                    //                                            }
                    //                                        }
                    //                                        else
                    //                                        {
                    //                                            goto NextItem;
                    //                                        }

                    //                                        break;
                    //                                    }

                    //                                case "盾":
                    //                                    {
                    //                                        if (!is_left_hand_available)
                    //                                        {
                    //                                            goto NextItem;
                    //                                        }

                    //                                        break;
                    //                                    }

                    //                                default:
                    //                                    {
                    //                                        goto NextItem;
                    //                                        break;
                    //                                    }
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "右肩":
                    //                    case "左肩":
                    //                    case "肩":
                    //                        {
                    //                            if (withBlock14.Part() != "両肩" && withBlock14.Part() != "肩")
                    //                            {
                    //                                goto NextItem;
                    //                            }

                    //                            if (withBlock14.Part() == "両肩")
                    //                            {
                    //                                if (empty_slot < 2)
                    //                                {
                    //                                    goto NextItem;
                    //                                }
                    //                            }

                    //                            break;
                    //                        }

                    //                    case "強化パーツ":
                    //                    case "アイテム":
                    //                        {
                    //                            if (withBlock14.Part() != "強化パーツ" && withBlock14.Part() != "アイテム")
                    //                            {
                    //                                goto NextItem;
                    //                            }

                    //                            if (empty_slot < withBlock14.Size())
                    //                            {
                    //                                goto NextItem;
                    //                            }

                    //                            break;
                    //                        }

                    //                    default:
                    //                        {
                    //                            if ((withBlock14.Part() ?? "") != (ipart ?? ""))
                    //                            {
                    //                                goto NextItem;
                    //                            }

                    //                            if (empty_slot < withBlock14.Size())
                    //                            {
                    //                                goto NextItem;
                    //                            }

                    //                            break;
                    //                        }
                    //                }

                    //                if (withBlock14.Unit is object)
                    //                {
                    //                    {
                    //                        var withBlock15 = withBlock14.Unit.CurrentForm();
                    //                        // 離脱したユニットが装備している
                    //                        if (withBlock15.Status == "離脱")
                    //                        {
                    //                            goto NextItem;
                    //                        }

                    //                        // 敵ユニットが装備している
                    //                        if (withBlock15.Party != "味方")
                    //                        {
                    //                            goto NextItem;
                    //                        }
                    //                    }

                    //                    // 呪われているので外せない……
                    //                    if (withBlock14.IsFeatureAvailable("呪い"))
                    //                    {
                    //                        goto NextItem;
                    //                    }
                    //                }

                    //                // 既に登録済み？
                    //                var loopTo36 = Information.UBound(item_list);
                    //                for (i = 1; i <= loopTo36; i++)
                    //                {
                    //                    if ((item_list[i] ?? "") == (withBlock14.Name ?? ""))
                    //                    {
                    //                        goto NextItem;
                    //                    }
                    //                }
                    //            }

                    //            // 装備可能？
                    //            if (!u.IsAbleToEquip(it))
                    //            {
                    //                goto NextItem;
                    //            }

                    //            Array.Resize(item_list, Information.UBound(item_list) + 1 + 1);
                    //            item_list[Information.UBound(item_list)] = it.Name;
                    //        NextItem:
                    //            ;
                    //        }

                    //        // 装備可能なアイテムの一覧を表示
                    //        list = new string[Information.UBound(item_list) + 1];
                    //        strkey_list = new string[Information.UBound(item_list) + 1];
                    //        id_list = new string[Information.UBound(item_list) + 1];
                    //        GUI.ListItemFlag = new bool[Information.UBound(item_list) + 1];
                    //        GUI.ListItemComment = new string[Information.UBound(item_list) + 1];
                    //        var loopTo37 = Information.UBound(item_list);
                    //        for (i = 1; i <= loopTo37; i++)
                    //        {
                    //            iname = item_list[i];
                    //            {
                    //                var withBlock16 = SRC.IDList.Item(iname);
                    //                string localRightPaddedString() { string argbuf = withBlock16.Nickname; var ret = GeneralLib.RightPaddedString(argbuf, 22); withBlock16.Nickname = argbuf; return ret; }

                    //                list[i] = localRightPaddedString() + " ";
                    //                if (withBlock16.IsFeatureAvailable("大型アイテム"))
                    //                {
                    //                    string localRightPaddedString1() { string argbuf = withBlock16.Part + "[" + SrcFormatter.Format(withBlock16.Size()) + "]"; var ret = GeneralLib.RightPaddedString(argbuf, 15); return ret; }

                    //                    list[i] = list[i] + localRightPaddedString1();
                    //                }
                    //                else
                    //                {
                    //                    list[i] = list[i] + GeneralLib.RightPaddedString(withBlock16.Part, 15);
                    //                }

                    //                // アイテムの数をカウント
                    //                inum = 0;
                    //                inum2 = 0;
                    //                foreach (Item currentIt1 in SRC.IList)
                    //                {
                    //                    it = currentIt1;
                    //                    {
                    //                        var withBlock17 = it;
                    //                        if ((withBlock17.Name ?? "") == (iname ?? ""))
                    //                        {
                    //                            if (withBlock17.Exist)
                    //                            {
                    //                                if (withBlock17.Unit is null)
                    //                                {
                    //                                    inum = (inum + 1);
                    //                                    inum2 = (inum2 + 1);
                    //                                }
                    //                                else if (withBlock17.Unit.CurrentForm().Status != "離脱")
                    //                                {
                    //                                    if (!withBlock17.IsFeatureAvailable("呪い"))
                    //                                    {
                    //                                        inum = (inum + 1);
                    //                                    }
                    //                                }
                    //                            }
                    //                        }
                    //                    }
                    //                }

                    //                string localLeftPaddedString5() { string argbuf = SrcFormatter.Format(inum2); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

                    //                string localLeftPaddedString6() { string argbuf = SrcFormatter.Format(inum); var ret = GeneralLib.LeftPaddedString(argbuf, 2); return ret; }

                    //                list[i] = list[i] + localLeftPaddedString5() + "/" + localLeftPaddedString6();
                    //                id_list[i] = withBlock16.Name;
                    //                strkey_list[i] = withBlock16.KanaName;
                    //                GUI.ListItemComment[i] = withBlock16.Comment;
                    //            }
                    //        }

                    //        // アイテムを名前順にソート
                    //        var loopTo38 = (Information.UBound(strkey_list) - 1);
                    //        for (i = 1; i <= loopTo38; i++)
                    //        {
                    //            max_item = i;
                    //            max_str = strkey_list[i];
                    //            var loopTo39 = Information.UBound(strkey_list);
                    //            for (j = (i + 1); j <= loopTo39; j++)
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

                    //        // 装備するアイテムの種類を選択
                    //        caption_str = "装備するアイテムを選択 ： " + u.Nickname;
                    //        if (u.CountPilot() > 0 && !Expression.IsOptionDefined("等身大基準"))
                    //        {
                    //            caption_str = caption_str + " (" + u.MainPilot().get_Nickname(false) + ")";
                    //        }

                    //        caption_str = caption_str + "  " + Expression.Term("ＨＰ", selectedUnit) + "=" + SrcFormatter.Format(u.MaxHP) + " " + Expression.Term("ＥＮ", selectedUnit) + "=" + SrcFormatter.Format(u.MaxEN) + " " + Expression.Term("装甲", selectedUnit) + "=" + SrcFormatter.Format(u.get_Armor("")) + " " + Expression.Term("運動性", selectedUnit) + "=" + SrcFormatter.Format(u.get_Mobility("")) + " " + Expression.Term("移動力", selectedUnit) + "=" + SrcFormatter.Format(u.Speed);
                    //        ret = GUI.ListBox(caption_str, list, "アイテム               分類            数量", "連続表示,コメント");

                    //        // キャンセルされた？
                    //        if (ret == 0)
                    //        {
                    //            break;
                    //        }

                    //        iname = id_list[ret];

                    //        // 未装備のアイテムがあるかどうか探す
                    //        foreach (Item currentIt2 in SRC.IList)
                    //        {
                    //            it = currentIt2;
                    //            {
                    //                var withBlock18 = it;
                    //                if ((withBlock18.Name ?? "") == (iname ?? "") && withBlock18.Exist)
                    //                {
                    //                    if (withBlock18.Unit is null)
                    //                    {
                    //                        // 未装備の装備が見つかったのでそれを装備
                    //                        if (!string.IsNullOrEmpty(iid))
                    //                        {
                    //                            selectedUnit.DeleteItem(iid);
                    //                        }
                    //                        // 呪いのアイテムを装備……
                    //                        if (withBlock18.IsFeatureAvailable("呪い"))
                    //                        {
                    //                            Interaction.MsgBox(withBlock18.Nickname() + "は呪われていた！");
                    //                        }

                    //                        selectedUnit.AddItem(it);
                    //                        if (string.IsNullOrEmpty(Map.MapFileName))
                    //                        {
                    //                            selectedUnit.FullRecover();
                    //                        }

                    //                        if (GUI.MainForm.Visible)
                    //                        {
                    //                            Status.DisplayUnitStatus(selectedUnit);
                    //                        }

                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //        }

                    //        // 選択されたアイテムを列挙
                    //        list = new string[1];
                    //        id_list = new string[1];
                    //        GUI.ListItemComment = new string[1];
                    //        inum = 0;
                    //        ItemData localItem7() { object argIndex1 = iname; var ret = SRC.IDList.Item(argIndex1); return ret; }

                    //        if (!localItem7().IsFeatureAvailable("呪い"))
                    //        {
                    //            foreach (Item currentIt3 in SRC.IList)
                    //            {
                    //                it = currentIt3;
                    //                if ((it.Name ?? "") != (iname ?? "") || !it.Exist)
                    //                {
                    //                    goto NextItem2;
                    //                }

                    //                if (it.Unit is null)
                    //                {
                    //                    goto NextItem2;
                    //                }

                    //                {
                    //                    var withBlock19 = it.Unit.CurrentForm();
                    //                    if (withBlock19.Status == "離脱")
                    //                    {
                    //                        goto NextItem2;
                    //                    }

                    //                    if (withBlock19.Party != "味方")
                    //                    {
                    //                        goto NextItem2;
                    //                    }

                    //                    Array.Resize(list, Information.UBound(list) + 1 + 1);
                    //                    Array.Resize(id_list, Information.UBound(list) + 1);
                    //                    Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);
                    //                    if (!Expression.IsOptionDefined("等身大基準") && withBlock19.CountPilot() > 0)
                    //                    {
                    //                        string localRightPaddedString2() { string argbuf = withBlock19.Nickname0; var ret = GeneralLib.RightPaddedString(argbuf, 36); withBlock19.Nickname0 = argbuf; return ret; }

                    //                        list[Information.UBound(list)] = localRightPaddedString2() + " " + withBlock19.MainPilot().get_Nickname(false);
                    //                    }
                    //                    else
                    //                    {
                    //                        list[Information.UBound(list)] = withBlock19.Nickname0;
                    //                    }

                    //                    id_list[Information.UBound(list)] = it.ID;
                    //                    var loopTo40 = withBlock19.CountItem();
                    //                    for (i = 1; i <= loopTo40; i++)
                    //                    {
                    //                        {
                    //                            var withBlock20 = withBlock19.Item(i);
                    //                            if ((withBlock20.Class() != "固定" || !withBlock20.IsFeatureAvailable("非表示")) && withBlock20.Part() != "非表示")
                    //                            {
                    //                                GUI.ListItemComment[Information.UBound(list)] = GUI.ListItemComment[Information.UBound(list)] + withBlock20.Nickname() + " ";
                    //                            }
                    //                        }
                    //                    }

                    //                    inum = (inum + 1);
                    //                }

                    //            NextItem2:
                    //                ;
                    //            }
                    //        }

                    //        GUI.ListItemFlag = new bool[Information.UBound(list) + 1];
                    //        Array.Resize(GUI.ListItemComment, Information.UBound(list) + 1);

                    //        // どのアイテムを装備するか選択
                    //        Item localItem8() { var tmp = id_list; object argIndex1 = tmp[1]; var ret = SRC.IList.Item(argIndex1); return ret; }

                    //        caption_str = localItem8().Nickname() + "の入手先を選択 ： " + u.Nickname;
                    //        if (u.CountPilot() > 0 && !Expression.IsOptionDefined("等身大基準"))
                    //        {
                    //            caption_str = caption_str + " (" + u.MainPilot().get_Nickname(false) + ")";
                    //        }

                    //        caption_str = caption_str + "  " + Expression.Term("ＨＰ", selectedUnit) + "=" + SrcFormatter.Format(u.MaxHP) + " " + Expression.Term("ＥＮ", selectedUnit) + "=" + SrcFormatter.Format(u.MaxEN) + " " + Expression.Term("装甲", selectedUnit) + "=" + SrcFormatter.Format(u.get_Armor("")) + " " + Expression.Term("運動性", selectedUnit) + "=" + SrcFormatter.Format(u.get_Mobility("")) + " " + Expression.Term("移動力", selectedUnit) + "=" + SrcFormatter.Format(u.Speed);
                    //        GUI.TopItem = 1;
                    //        if (Expression.IsOptionDefined("等身大基準"))
                    //        {
                    //            ret = GUI.ListBox(caption_str, list, "ユニット", "連続表示,コメント");
                    //        }
                    //        else
                    //        {
                    //            ret = GUI.ListBox(caption_str, list, "ユニット                             パイロット", "連続表示,コメント");
                    //        }

                    //        // アイテムを交換
                    //        if (ret > 0)
                    //        {
                    //            if (!string.IsNullOrEmpty(iid))
                    //            {
                    //                u.DeleteItem(iid);
                    //            }

                    //            var tmp7 = id_list;
                    //            {
                    //                var withBlock21 = SRC.IList.Item(tmp7[ret]);
                    //                if (withBlock21.Unit is object)
                    //                {
                    //                    withBlock21.Unit.DeleteItem(withBlock21.ID);
                    //                }

                    //                // 呪いのアイテムを装備……
                    //                if (withBlock21.IsFeatureAvailable("呪い"))
                    //                {
                    //                    Interaction.MsgBox(withBlock21.Nickname() + "は呪われていた！");
                    //                }
                    //            }

                    //            Item localItem9() { var tmp = id_list; object argIndex1 = tmp[ret]; var ret = SRC.IList.Item(argIndex1); return ret; }

                    //            u.AddItem(localItem9());
                    //            if (string.IsNullOrEmpty(Map.MapFileName))
                    //            {
                    //                u.FullRecover();
                    //            }

                    //            if (GUI.MainForm.Visible)
                    //            {
                    //                Status.DisplayUnitStatus(selectedUnit);
                    //            }

                    //            break;
                    //        }

                    //    NextLoop:
                    //        ;
                    //    }

                    //NextLoop2:
                    //    ;
                }
            }

            // ユニットがあらかじめ選択されている場合
            // (ユニットステータスからのアイテム交換時)
            if (selected_unit is object)
            {
                GUI.CloseListBox();
                // TODO txtComment Visible
                //if (withBlock22.txtComment.Enabled)
                //{
                //    withBlock22.txtComment.Enabled = false;
                //    withBlock22.txtComment.Visible = false;
                //    withBlock22.Height = SrcFormatter.TwipsToPixelsY(SrcFormatter.PixelsToTwipsY(withBlock22.Height) - 600d);
                //}

                GUI.ReduceListBoxHeight();
                GUI.EnlargeListBoxWidth();
                return;
            }

            goto Beginning;
        }

        // 換装コマンド
        private void ExchangeFormCommand()
        {
            var top_item = 1;
        Beginning:
            ;

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
