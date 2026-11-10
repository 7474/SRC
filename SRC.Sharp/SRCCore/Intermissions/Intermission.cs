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

        /// <summary>
        /// Lib フォルダのファイル（パイロットステータス表示.eve など）を、元の SRC と同じく
        /// シナリオのフォルダ → ExtDataPath → ExtDataPath2 → SRC のフォルダの順に探す。見つからなければ null
        /// </summary>
        public string SearchLibFile(string fileName)
        {
            return new string[]
            {
                SRC.ScenarioPath,
                SRC.ExtDataPath,
                SRC.ExtDataPath2,
                SRC.AppPath,
            }
                // 拡張データフォルダを設定していなければ探さない
                .Where((x, i) => !(i == 1 || i == 2) || !string.IsNullOrEmpty(x))
                .Select(x => Path.Combine(x ?? "", "Lib", fileName))
                .FirstOrDefault(x => SRC.FileSystem.FileExists(x));
        }


        // インターミッション
        public void InterMissionCommand(bool skip_update = false)
        {
            SRC.Stage = "インターミッション";
            SRC.IsSubStage = false;

            // ＢＧＭを変更
            // 元の SRC は ChangeBGM を使わず、インターミッションのＢＧＭでなければ止めてから演奏し直す
            // （番号違いのＢＧＭも別の曲とみなす）。VB: InStr(BGMFileName, "\" & BGMName("Intermission")) = 0。
            // StartBGM・ChangeBGM（本体部分 & "." で比べる）と違い、拡張子まで含めた名前で比べる
            Sound.KeepBGM = false;
            Sound.BossBGM = false;
            if (!Sound.BGMFileNameContains(Sound.BGMName("Intermission")))
            {
                Sound.StopBGM();
                Sound.StartBGM(Sound.BGMName("Intermission"));
            }

            // マップをクリア
            // 元の SRC と同じく、UList.Update などより前にユニットの配置を消しておく
            // （ClearMap はこのあと、データの更新が終わってから呼ぶ）
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
                // ListItemID はコマンドの名前（エリアスを適用する前）。元の SRC と同じく、これで実行するコマンドを決める
                var cmd_list = new List<ListBoxItem>();
                // ユーザー定義のインターミッションコマンドの変数名（ユーザー定義でなければ null）
                var var_names = new List<string>();

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
                while (var_names.Count < cmd_list.Count)
                {
                    var_names.Add(null);
                }

                // ユーザー定義のインターミッションコマンド
                foreach (var var in Event.GlobalVariableList.Values)
                {
                    if (Strings.InStr(var.Name, "IntermissionCommand(") == 1)
                    {
                        var pos = Strings.Len("IntermissionCommand(");
                        var buf = Strings.Mid(var.Name, pos + 1, Strings.Len(var.Name) - pos - 1);
                        buf = Expression.GetValueAsString(buf);
                        Expression.FormatMessage(ref buf);
                        cmd_list.Add(new ListBoxItem(buf, buf));
                        var_names.Add(var.Name);
                    }
                }

                // 終了コマンド
                cmd_list.Add(new ListBoxItem("SRCを終了"));
                var_names.Add(null);

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
                    lb_caption = arglb_caption,
                    lb_info = "コマンド",
                    lb_mode = "連続表示",
                });
                // キャンセル
                if (ret == 0) { continue; }

                var selectedItem = cmd_list[ret - 1];
                // 選択されたインターミッションコマンドを実行
                // 元の SRC と同じく名前で分けるので、ユーザー定義のコマンドの名前が「機体改造」などなら、そのコマンドを実行する
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
                        // ファイルの選択（元の SRC はシナリオフォルダの外を選ぶと確かめる）は GUI に任せる
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
                            var eveFile = SearchLibFile("パイロットステータス表示.eve");
                            if (string.IsNullOrEmpty(eveFile))
                            {
                                // 元の SRC は AppPath の Lib を必ず読む（無ければ読み込みのエラー）。
                                // 何も始めずに戻ると操作できなくなるため、知らせてインターミッションを続ける
                                GUI.ErrorMessage(@"Lib\パイロットステータス表示.eve が見つかりません");
                                break;
                            }

                            GUI.CloseListBox();
                            GUI.ReduceListBoxHeight();
                            SRC.IsSubStage = true;
                            SRC.StartScenario(eveFile);
                            // サブステージを通常のステージとして実行
                            SRC.IsSubStage = true;
                            return;
                        }

                    case "ユニットステータス":
                        {
                            var eveFile = SearchLibFile("ユニットステータス表示.eve");
                            if (string.IsNullOrEmpty(eveFile))
                            {
                                // 元の SRC は AppPath の Lib を必ず読む（無ければ読み込みのエラー）。
                                // 何も始めずに戻ると操作できなくなるため、知らせてインターミッションを続ける
                                GUI.ErrorMessage(@"Lib\ユニットステータス表示.eve が見つかりません");
                                break;
                            }

                            GUI.CloseListBox();
                            GUI.ReduceListBoxHeight();
                            SRC.IsSubStage = true;
                            SRC.StartScenario(eveFile);
                            // サブステージを通常のステージとして実行
                            SRC.IsSubStage = true;
                            return;
                        }

                    case "キャンセル":
                        // キャンセル
                        break;

                    // ユーザー定義のインターミッションコマンド
                    default:
                        {
                            GUI.CloseListBox();
                            GUI.ReduceListBoxHeight();
                            SRC.IsSubStage = true;
                            SRC.StartScenario(Expression.GetValueAsString(var_names[ret - 1]));
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

        // 元の SRC の StrComp(…, vbTextCompare) に当たる比較
        private static readonly StringComparer TextCompare = SrcCollection<string>.KeyComparer;

        /// <summary>
        /// 元の SRC の並べ替え（大きい順の選択ソート）。同じ値は入れ替えないが、
        /// 入れ替えで順が崩れるので安定ではない。keys は並べ替えの途中で書き換える
        /// </summary>
        private static void SortDescending<T>(IList<T> items, IList<long> keys)
        {
            for (var i = 0; i < items.Count - 1; i++)
            {
                var max_item = i;
                var max_value = keys[i];
                for (var j = i + 1; j < items.Count; j++)
                {
                    if (keys[j] > max_value)
                    {
                        max_item = j;
                        max_value = keys[j];
                    }
                }
                if (max_item != i)
                {
                    (items[i], items[max_item]) = (items[max_item], items[i]);
                    keys[max_item] = keys[i];
                }
            }
        }

        /// <summary>
        /// 元の SRC の並べ替え（StrComp(…, vbTextCompare) = -1 で小さい順に選ぶ選択ソート）
        /// </summary>
        private static void SortAscendingText<T>(IList<T> items, IList<string> keys)
        {
            for (var i = 0; i < items.Count - 1; i++)
            {
                var max_item = i;
                var max_str = keys[i] ?? "";
                for (var j = i + 1; j < items.Count; j++)
                {
                    if (TextCompare.Compare(keys[j] ?? "", max_str) < 0)
                    {
                        max_item = j;
                        max_str = keys[j] ?? "";
                    }
                }
                if (max_item != i)
                {
                    (items[i], items[max_item]) = (items[max_item], items[i]);
                    keys[max_item] = keys[i];
                }
            }
        }

        // ユニットの一覧の数値による並べ替えのキー（元の SRC の RankUpCommand・ExchangeItemCommand）
        private static long UnitSortKey(Unit u, string sort_mode)
        {
            switch (sort_mode ?? "")
            {
                case "ＨＰ": return u.MaxHP;
                case "ＥＮ": return u.MaxEN;
                case "装甲": return u.get_Armor("");
                case "運動性": return u.get_Mobility("");
                case "ユニットランク": return u.Rank;
                case "レベル":
                    {
                        if (u.CountPilot() > 0)
                        {
                            var p = u.MainPilot();
                            return 500L * p.Level + p.Exp;
                        }
                        return 0;
                    }
                default: return 0;
            }
        }

        // ユニットの一覧の文字列による並べ替えのキー（元の SRC の RankUpCommand・ExchangeItemCommand）
        private static string UnitSortStrKey(Unit u, string sort_mode)
        {
            switch (sort_mode ?? "")
            {
                case "名称":
                case "ユニット名称":
                    return u.KanaName;
                case "パイロット名称":
                    return u.CountPilot() > 0 ? u.MainPilot().KanaName : "";
                default: return "";
            }
        }

        // ユニットの ID の並びを、元の SRC と同じく sort_mode で並べ替える
        private void SortUnitIds(IList<string> id_list, string sort_mode)
        {
            if (Strings.InStr(sort_mode, "名称") == 0)
            {
                // 数値を使ったソート
                SortDescending(id_list, id_list.Select(id => UnitSortKey(SRC.UList.Item(id), sort_mode)).ToList());
            }
            else
            {
                // 文字列を使ったソート
                SortAscendingText(id_list, id_list.Select(id => UnitSortStrKey(SRC.UList.Item(id), sort_mode)).ToList());
            }
        }

        // ランクの表示（元の SRC は 10 未満なら全角の 1 文字にする）
        private static string RankString(int rank)
        {
            return rank < 10
                ? Strings.StrConv(SrcFormatter.Format(rank), VbStrConv.Wide)
                : SrcFormatter.Format(rank);
        }

        // ユニットが装備しているアイテムを列記したコメント（元の SRC と同じく、それぞれの後ろに空白を付ける）
        private static string ItemListComment(Unit u)
        {
            return string.Concat(u.ItemList
                .Where(itm => (itm.Class() != "固定" || !itm.IsFeatureAvailable("非表示")) && itm.Part() != "非表示")
                .Select(itm => itm.Nickname() + " "));
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
            // 元の SRC と同じく、並べ替えは前の並びから行う（並びを引き継ぐ）
            var id_list = SRC.UList.Items
                .Where(u => !(u.Party0 != "味方" || u.Status != "待機"))
                .Select(u => u.ID)
                .ToList();

        Beginning:
            ;

            // ソート
            SortUnitIds(id_list, sort_mode);

            // 各行を作る。元の SRC は改造したユニット（と分離形態）の行だけを作り直すが、
            // SRC# は毎回すべての行を作り直す（作り直す行の中身は同じ）
            var units = id_list.Select(id =>
            {
                var u = SRC.UList.Item(id);
                // 改造が可能？
                var cost = RankUpCost(u);
                // ユニットランク
                string msg;
                if (use_max_rank)
                {
                    msg = GeneralLib.RightPaddedString(u.Nickname0, name_width)
                        + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.Rank), 2) + "/";
                    if (MaxRank(u) > 0)
                    {
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(MaxRank(u)), 2);
                    }
                    else
                    {
                        msg += "--";
                    }
                }
                else
                {
                    msg = GeneralLib.RightPaddedString(u.Nickname0, name_width) + RankString(u.Rank);
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
                msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxHP), 6)
                    + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxEN), 4)
                    + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Armor("")), 6)
                    + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Mobility("")), 4);
                // 等身大基準でない場合はパイロット名を表示
                if (!Expression.IsOptionDefined("等身大基準"))
                {
                    if (u.CountPilot() > 0)
                    {
                        msg += "  " + u.MainPilot().get_Nickname(false);
                    }
                }

                return new ListBoxItem(msg, u.ID)
                {
                    // 装備しているアイテムをコメント欄に列記
                    ListItemComment = ItemListComment(u),
                    ListItemFlag = cost > SRC.Money || cost > 10000000,
                };
            }).ToList();

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
                        + Expression.Term("ランク", null, 6) + "   費用 Lv  "
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
                        + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", null, 4) + " パイロット";
                }
                else
                {
                    info = "ユニット                     "
                        + Expression.Term("ランク", null, 6) + "   費用  "
                        + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                        + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", null, 4) + " パイロット";
                }
            }
            var list = new List<ListBoxItem> { new ListBoxItem("▽並べ替え▽", "▽並べ替え▽") };
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
                                new ListBoxItem("ユニット名称", "ユニット名称"),
                                new ListBoxItem("パイロット名称", "パイロット名称"),
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
            {
                var u = SRC.UList.Item(id_list[ret - 2]);

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

                // ユニットランクを一段階上げる（他形態・合体と分離のユニットのランクも上げる）
                u.RankUp();
            }

            goto Beginning;
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
                    if ((GeneralLib.LLength(u.FeatureData("分離")) == 3
                        && !u.IsFeatureAvailable("主形態")) || u.IsFeatureAvailable("制限時間"))
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
                    RankUpCostRet = VBMath.CLng(RankUpCostRet * (1d + u.FeatureLevel("改造費修正") / 10d));
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
                MaxRankRet = GeneralLib.MinLng(MaxRankRet, VBMath.CLng(u.FeatureLevel("最大改造数")));
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
            var pilots = new List<ListBoxItem>();
            foreach (var p in SRC.PList.Items)
            {
                if (p.Party != "味方" || p.Away || p.IsFix)
                {
                    continue;
                }

                // 追加パイロット＆サポートは乗り換え不可
                if (p.IsAdditionalPilot || p.IsAdditionalSupport)
                {
                    continue;
                }

                var is_support = false;
                if (p.Unit != null)
                {
                    // サポートが複数乗っている場合は乗り降り不可
                    if (p.Unit.CountSupport() > 1)
                    {
                        continue;
                    }

                    // サポートパイロットとして乗り込んでいるかを判定
                    if (p.Unit.CountSupport() == 1)
                    {
                        if (p.ID == p.Unit.Supports.First().ID)
                        {
                            is_support = true;
                        }
                    }

                    // 通常のパイロットの場合
                    if (!is_support)
                    {
                        // ３人乗り以上は乗り降り不可
                        if (p.Unit.Data.PilotNum != 1 && Math.Abs(p.Unit.Data.PilotNum) != 2)
                        {
                            continue;
                        }
                    }
                }

                if (is_support)
                {
                    // サポートパイロットの場合
                    // パイロットのステータス
                    var msg = GeneralLib.RightPaddedString("*" + p.get_Nickname(false), 25)
                        + GeneralLib.LeftPaddedString(Strings.StrConv(SrcFormatter.Format(p.Level), VbStrConv.Wide), 4);
                    // ユニットのステータス
                    msg += "  " + GeneralLib.RightPaddedString(p.Unit.Nickname0, 29)
                        + "(" + p.Unit.MainPilot().get_Nickname(false) + ")";
                    pilots.Add(new ListBoxItem(msg, p.ID)
                    {
                        // ユニットが装備しているアイテム一覧
                        ListItemComment = ItemListComment(p.Unit),
                    });
                }
                else if (p.Unit is null)
                {
                    // ユニットに乗っていないパイロットの場合
                    // パイロットのステータス
                    var msg = GeneralLib.RightPaddedString(" " + p.get_Nickname(false), 25)
                        + GeneralLib.LeftPaddedString(Strings.StrConv(SrcFormatter.Format(p.Level), VbStrConv.Wide), 4);
                    pilots.Add(new ListBoxItem(msg, p.ID)
                    {
                        ListItemComment = "",
                    });
                }
                else if (p.Unit.CountPilot() <= 2)
                {
                    // 複数乗りのユニットに乗っているパイロットの場合
                    // パイロットが足りない？
                    var msg = p.Unit.CountPilot() < Math.Abs(p.Unit.Data.PilotNum) ? "-" : " ";

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
                        var k = 0;
                        foreach (var up in p.Unit.Pilots)
                        {
                            k++;
                            if (ReferenceEquals(up, p))
                            {
                                pname = pname + "(" + SrcFormatter.Format(k) + ")";
                            }
                        }
                    }

                    // パイロット＆ユニットのステータス
                    msg += GeneralLib.RightPaddedString(pname, 24)
                        + GeneralLib.LeftPaddedString(Strings.StrConv(SrcFormatter.Format(p.Level), VbStrConv.Wide), 4)
                        + "  " + GeneralLib.RightPaddedString(p.Unit.Nickname0, 29);
                    if (p.Unit.CountSupport() > 0)
                    {
                        msg += "(" + p.Unit.Supports.First().get_Nickname(false) + ")";
                    }
                    pilots.Add(new ListBoxItem(msg, p.ID)
                    {
                        // ユニットが装備しているアイテム一覧
                        ListItemComment = ItemListComment(p.Unit),
                    });
                }
                // 3 人以上乗っているユニットのパイロットは一覧に出さない
            }

        SortAgain:
            ;

            // ソート
            // 元の SRC と同じ選択ソートで、前の並びから並べ替える
            if (sort_mode == "レベル")
            {
                // レベルによるソート
                SortDescending(pilots, pilots.Select(x =>
                {
                    var p = SRC.PList.Item(x.ListItemID);
                    return 500L * p.Level + p.Exp;
                }).ToList());
            }
            else
            {
                // 読み仮名によるソート
                SortAscendingText(pilots, pilots.Select(x => SRC.PList.Item(x.ListItemID).KanaName).ToList());
            }

            // パイロットを選択
            GUI.TopItem = top_item;
            string caption;
            string info;
            if (Expression.IsOptionDefined("等身大基準"))
            {
                caption = "キャラクター選択";
                info = " キャラクター          レベル  ユニット";
            }
            else
            {
                caption = "パイロット選択";
                info = " パイロット            レベル  ユニット";
            }
            var list = new List<ListBoxItem> { new ListBoxItem("▽並べ替え▽", "▽並べ替え▽") };
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
                var units = new List<ListBoxItem>();
                foreach (var u in SRC.UList.Items)
                {
                    if (u.Party0 != "味方" || u.Status != "待機")
                    {
                        continue;
                    }

                    if (u.CountSupport() > 1)
                    {
                        if (Strings.InStr(p.Class, "専属サポート") == 0)
                        {
                            continue;
                        }
                    }

                    if (ReferenceEquals(u, p.Unit))
                    {
                        continue;
                    }

                    if (!p.IsAbleToRide(u))
                    {
                        continue;
                    }

                    // サポートキャラでなければ乗り換えられるパイロット数に制限がある
                    if (!p.IsSupport(u))
                    {
                        if (u.Data.PilotNum != 1 && Math.Abs(u.Data.PilotNum) != 2)
                        {
                            continue;
                        }
                    }

                    string msg;
                    if (u.CountPilot() > 0)
                    {
                        // Fixコマンドでパイロットが固定されたユニットはサポートでない限り乗り換え不可
                        if (u.Pilots.First().IsFix && !p.IsSupport(u))
                        {
                            continue;
                        }

                        // パイロットが足りている？
                        msg = u.CountPilot() < Math.Abs(u.Data.PilotNum) ? "-" : " ";
                        msg += GeneralLib.RightPaddedString(u.Nickname0, 35)
                            + GeneralLib.RightPaddedString(u.MainPilot().get_Nickname(false), 21)
                            + " " + RankString(u.Rank);
                        if (u.CountSupport() > 0)
                        {
                            msg += " (" + u.Supports.First().get_Nickname(false) + ")";
                        }
                    }
                    else if (!p.IsSupport(u))
                    {
                        // 誰も乗ってないユニットに乗れるのは通常パイロットのみ
                        msg = " " + GeneralLib.RightPaddedString(u.Nickname0, 35)
                            + Strings.Space(21)
                            + " " + RankString(u.Rank);
                    }
                    else
                    {
                        continue;
                    }

                    units.Add(new ListBoxItem(msg, u.ID)
                    {
                        // ユニットに装備されているアイテムをコメント欄に列記
                        ListItemComment = ItemListComment(u),
                    });
                }

            SortAgain2:
                ;

                // ソート
                SortUnitIds2(units, sort_mode2);

                // 乗り換え先を選択
                {
                    GUI.TopItem = 1;
                    var u = p.Unit;
                    string uCaption;
                    string uInfo;
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        uInfo = " ユニット                           キャラクター       " + Expression.Term("ランク", u: null);
                    }
                    else
                    {
                        uInfo = " ユニット                           パイロット         " + Expression.Term("ランク", u: null);
                    }
                    if (u != null)
                    {
                        if (u.IsFeatureAvailable("追加パイロット"))
                        {
                            uCaption = "乗り換え先選択 ： " + u.MainPilot().get_Nickname(false) + " (" + u.Nickname + ")";
                        }
                        else
                        {
                            uCaption = "乗り換え先選択 ： " + p.get_Nickname(false) + " (" + u.Nickname + ")";
                        }
                    }
                    else
                    {
                        uCaption = "乗り換え先選択 ： " + p.get_Nickname(false);
                    }
                    var uRet = GUI.ListBox(new ListBoxArgs
                    {
                        Items = list.AppendRange(units).ToList(),
                        lb_caption = uCaption,
                        lb_info = uInfo,
                        lb_mode = "連続表示,コメント",
                    });

                    switch (uRet)
                    {
                        case 0:
                            // キャンセル（元の SRC もここでコマンドを終える）
                            return;

                        case 1:
                            // ソート方法を選択
                            IList<ListBoxItem> sortItems;
                            if (Expression.IsOptionDefined("等身大基準"))
                            {
                                sortItems = new List<ListBoxItem>
                            {
                                new ListBoxItem("名称", "名称"),
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
                                new ListBoxItem("ユニット名称", "ユニット名称"),
                            };
                            }
                            GUI.TopItem = 1;
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

        // 乗り換え先のユニットの一覧を並べ替える（元の SRC の ExchangeUnitCommand の SortAgain2。
        // 数値のキーに「レベル」は無い）
        private void SortUnitIds2(IList<ListBoxItem> units, string sort_mode2)
        {
            if (Strings.InStr(sort_mode2, "名称") == 0)
            {
                // 数値によるソート
                SortDescending(units, units.Select(x => sort_mode2 == "レベル"
                    ? 0
                    : UnitSortKey(SRC.UList.Item(x.ListItemID), sort_mode2)).ToList());
            }
            else
            {
                // 読み仮名によるソート
                SortAscendingText(units, units.Select(x => SRC.UList.Item(x.ListItemID).KanaName).ToList());
            }
        }

        // アイテム交換コマンド
        public void ExchangeItemCommand(Unit selected_unit = null, string selected_part = "")
        {
            selected_part = selected_part ?? "";
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
            Unit u;
            if (selected_unit is object)
            {
                GUI.EnlargeListBoxHeight();
                GUI.ReduceListBoxWidth();
                u = selected_unit;
                if (GUI.MainFormVisible)
                {
                    if (!ReferenceEquals(u, SRC.GUIStatus.DisplayedUnit))
                    {
                        SRC.GUIStatus.DisplayUnitStatus(u);
                    }
                }
                goto MakeEquipedItemList;
            }

        Beginning:
            ;

            // ユニット一覧の作成
            var units = new List<ListBoxItem>();
            foreach (var lu in SRC.UList.Items)
            {
                if (lu.Party0 != "味方" || lu.Status != "待機")
                {
                    continue;
                }

                // 装備しているアイテムの数を数える
                var inum = 0;
                var inum2 = 0;
                foreach (var itm in lu.ItemList)
                {
                    if ((itm.Class() != "固定" || !itm.IsFeatureAvailable("非表示")) && itm.Part() != "非表示")
                    {
                        if (itm.Part() == "強化パーツ" || itm.Part() == "アイテム")
                        {
                            inum += itm.Size();
                        }
                        else
                        {
                            inum2 += itm.Size();
                        }
                    }
                }

                // リストを作成
                string msg;
                if (Expression.IsOptionDefined("等身大基準"))
                {
                    msg = GeneralLib.RightPaddedString(lu.Nickname0, 39);
                }
                else
                {
                    msg = GeneralLib.RightPaddedString(lu.Nickname0, 31);
                }
                msg += SrcFormatter.Format(inum) + "/" + SrcFormatter.Format(lu.MaxItemNum());
                if (inum2 > 0)
                {
                    msg += "(" + SrcFormatter.Format(inum2) + ")   ";
                }
                else
                {
                    msg += "      ";
                }
                msg += RankString(lu.Rank);
                if (Expression.IsOptionDefined("等身大基準"))
                {
                    if (lu.CountPilot() > 0)
                    {
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(lu.MainPilot().Level), 3);
                    }
                }
                msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(lu.MaxHP), 6)
                    + GeneralLib.LeftPaddedString(SrcFormatter.Format(lu.MaxEN), 4)
                    + GeneralLib.LeftPaddedString(SrcFormatter.Format(lu.get_Armor("")), 5)
                    + GeneralLib.LeftPaddedString(SrcFormatter.Format(lu.get_Mobility("")), 5);
                if (!Expression.IsOptionDefined("等身大基準"))
                {
                    if (lu.CountPilot() > 0)
                    {
                        msg += " " + lu.MainPilot().get_Nickname(false);
                    }
                }

                units.Add(new ListBoxItem(msg, lu.ID)
                {
                    ListItemComment = ItemListComment(lu),
                });
            }

        SortAgain:
            ;

            // ソート（元の SRC と同じ選択ソートで、前の並びから並べ替える）
            if (Strings.InStr(sort_mode, "名称") == 0)
            {
                // 数値によるソート
                SortDescending(units, units.Select(x => UnitSortKey(SRC.UList.Item(x.ListItemID), sort_mode)).ToList());
            }
            else
            {
                // 文字列によるソート
                SortAscendingText(units, units.Select(x => UnitSortStrKey(SRC.UList.Item(x.ListItemID), sort_mode)).ToList());
            }

            // アイテムを交換するユニットを選択
            {
                string caption = "アイテムを交換するユニットを選択";
                string info;
                if (Expression.IsOptionDefined("等身大基準"))
                {
                    info = "ユニット                               アイテム "
                        + Expression.Term("RK", null, 2) + " Lv  "
                        + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                        + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", u: null);
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
                    Items = units.Prepend(new ListBoxItem("▽並べ替え▽", "▽並べ替え▽")).ToList(),
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
                                new ListBoxItem("ユニット名称", "ユニット名称"),
                                new ListBoxItem("パイロット名称", "パイロット名称"),
                            };
                        }
                        GUI.TopItem = 1;
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

                // ユニットを選択
                u = SRC.UList.Item(units[ret - 2].ListItemID);
            }

        MakeEquipedItemList:
            ;

            // 選択されたユニットが装備しているアイテム一覧の作成
            // 元の SRC と同じく、配列は 1 から使う（0 番目は使わない）
            var arm_point = 0;
            var shoulder_point = 0;
            while (true)
            {
                // アイテムの装備個所一覧を作成
                var part_list = new List<string> { "" };
                if (u.IsFeatureAvailable("装備個所"))
                {
                    var buf = u.FeatureData("装備個所");
                    if (Strings.InStr(buf, "腕") > 0)
                    {
                        arm_point = part_list.Count;
                        part_list.Add("右手");
                        part_list.Add("左手");
                    }

                    if (Strings.InStr(buf, "肩") > 0)
                    {
                        shoulder_point = part_list.Count;
                        part_list.Add("右肩");
                        part_list.Add("左肩");
                    }

                    if (Strings.InStr(buf, "体") > 0)
                    {
                        part_list.Add("体");
                    }

                    if (Strings.InStr(buf, "頭") > 0)
                    {
                        part_list.Add("頭");
                    }
                }

                foreach (var fd in u.Features.Where(x => x.Name == "ハードポイント"))
                {
                    var hpart = fd.Data;
                    switch (hpart)
                    {
                        // 表示しない
                        case "強化パーツ":
                        case "アイテム":
                        case "非表示":
                            break;

                        default:
                            if (!part_list.Skip(1).Contains(hpart))
                            {
                                var size = u.ItemSlotSize(hpart);
                                for (var n = 0; n < size; n++)
                                {
                                    part_list.Add(hpart);
                                }
                            }
                            break;
                    }
                }

                for (var n = 0; n < u.MaxItemNum(); n++)
                {
                    part_list.Add(u.IsHero() ? "アイテム" : "強化パーツ");
                }

                // 特定の装備個所のアイテムのみを交換する？
                if (!string.IsNullOrEmpty(selected_part))
                {
                    var tmp_part_list = part_list.Skip(1).ToList();
                    part_list = new List<string> { "" };
                    arm_point = 0;
                    shoulder_point = 0;
                    foreach (var tmp in tmp_part_list)
                    {
                        if (tmp == selected_part
                            || ((selected_part == "片手" || selected_part == "両手" || selected_part == "盾")
                                && (tmp == "右手" || tmp == "左手"))
                            || ((selected_part == "肩" || selected_part == "両肩")
                                && (tmp == "右肩" || tmp == "左肩"))
                            || ((selected_part == "アイテム" || selected_part == "強化パーツ")
                                && (tmp == "アイテム" || tmp == "強化パーツ")))
                        {
                            part_list.Add(tmp);
                            switch (tmp)
                            {
                                case "右手":
                                    arm_point = part_list.Count - 1;
                                    break;
                                case "右肩":
                                    shoulder_point = part_list.Count - 1;
                                    break;
                            }
                        }
                    }
                }

                var part_item = part_list.Select(_ => "").ToList();
                // 元の SRC は範囲を確かめずに書き込む（左手の無い一覧で左手に書くなど）。SRC# は範囲外に書かない
                void SetPartItem(int idx, string value)
                {
                    if (idx >= 1 && idx < part_item.Count)
                    {
                        part_item[idx] = value;
                    }
                }
                string GetPartItem(int idx) => idx >= 1 && idx < part_item.Count ? part_item[idx] : "";

                // 装備個所に現在装備しているアイテムを割り当て
                foreach (var itm in u.ItemList)
                {
                    if (itm.Class() == "固定" && itm.IsFeatureAvailable("非表示"))
                    {
                        continue;
                    }

                    switch (itm.Part() ?? "")
                    {
                        case "両手":
                            if (arm_point == 0)
                            {
                                continue;
                            }
                            SetPartItem(arm_point, itm.ID);
                            SetPartItem(arm_point + 1, ":");
                            break;

                        case "片手":
                            if (arm_point == 0)
                            {
                                continue;
                            }
                            if (GetPartItem(arm_point) == "")
                            {
                                SetPartItem(arm_point, itm.ID);
                            }
                            else
                            {
                                SetPartItem(arm_point + 1, itm.ID);
                            }
                            break;

                        case "盾":
                            if (arm_point == 0)
                            {
                                continue;
                            }
                            SetPartItem(arm_point + 1, itm.ID);
                            break;

                        case "両肩":
                            // 元の SRC は右肩にだけ割り当て、左肩を ":" にしない
                            if (shoulder_point == 0)
                            {
                                continue;
                            }
                            SetPartItem(shoulder_point, itm.ID);
                            break;

                        case "肩":
                            if (shoulder_point == 0)
                            {
                                continue;
                            }
                            if (GetPartItem(shoulder_point) == "")
                            {
                                SetPartItem(shoulder_point, itm.ID);
                            }
                            else
                            {
                                SetPartItem(shoulder_point + 1, itm.ID);
                            }
                            break;

                        case "非表示":
                            // 無視
                            break;

                        default:
                            {
                                var found = false;
                                var isItemSlot = itm.Part() == "強化パーツ" || itm.Part() == "アイテム";
                                for (var j = 1; j < part_list.Count; j++)
                                {
                                    var match = isItemSlot
                                        ? (part_list[j] == "強化パーツ" || part_list[j] == "アイテム")
                                        : part_list[j] == itm.Part();
                                    if (match && part_item[j] == "")
                                    {
                                        part_item[j] = itm.ID;
                                        for (var k = j + 1; k <= j + itm.Size() - 1; k++)
                                        {
                                            if (k >= part_item.Count)
                                            {
                                                break;
                                            }
                                            part_item[k] = ":";
                                        }
                                        found = true;
                                        break;
                                    }
                                }
                                // 合う装備個所が無ければ、その装備個所の行を足す
                                if (!found && string.IsNullOrEmpty(selected_part))
                                {
                                    part_list.Add(itm.Part());
                                    part_item.Add(itm.ID);
                                }
                                break;
                            }
                    }
                }

                // リストを構築
                var n_slot = part_list.Count - 1;
                var texts = new string[n_slot + 2];
                var id_list = Enumerable.Repeat("", n_slot + 2).ToArray();
                var comments = Enumerable.Repeat("", n_slot + 2).ToArray();
                var flags = new bool[n_slot + 2];
                for (var i = 1; i <= n_slot; i++)
                {
                    switch (part_item[i])
                    {
                        case "":
                            texts[i] = GeneralLib.RightPaddedString("----", 23) + part_list[i];
                            break;

                        case ":":
                            texts[i] = GeneralLib.RightPaddedString(" :  ", 23) + part_list[i];
                            comments[i] = comments[i - 1];
                            flags[i] = flags[i - 1];
                            break;

                        default:
                            {
                                var itm = SRC.IList.Item(part_item[i]);
                                texts[i] = GeneralLib.RightPaddedString(itm.Nickname(), 22) + " " + part_list[i];
                                comments[i] = itm.Data.Comment;
                                id_list[i] = itm.ID;
                                for (var j = i + 1; j <= i + itm.Size() - 1; j++)
                                {
                                    if (j > n_slot)
                                    {
                                        break;
                                    }
                                    id_list[j] = itm.ID;
                                }
                                if (itm.IsFix)
                                {
                                    flags[i] = true;
                                    for (var j = i + 1; j <= i + itm.Size() - 1; j++)
                                    {
                                        if (j > n_slot)
                                        {
                                            break;
                                        }
                                        flags[j] = true;
                                    }
                                }
                                break;
                            }
                    }
                }

                List<ListBoxItem> MakeSlotItems(string lastText)
                {
                    var items = new List<ListBoxItem>();
                    for (var i = 1; i <= n_slot; i++)
                    {
                        items.Add(new ListBoxItem(texts[i], id_list[i])
                        {
                            ListItemComment = comments[i],
                            ListItemFlag = flags[i],
                        });
                    }
                    items.Add(new ListBoxItem(lastText, lastText));
                    return items;
                }

                string UnitCaption(string head)
                {
                    var cap = head + u.Nickname;
                    if (u.CountPilot() > 0 && !Expression.IsOptionDefined("等身大基準"))
                    {
                        cap = cap + " (" + u.MainPilot().get_Nickname(false) + ")";
                    }

                    return cap + "  " + Expression.Term("ＨＰ", u) + "=" + SrcFormatter.Format(u.MaxHP)
                        + " " + Expression.Term("ＥＮ", u) + "=" + SrcFormatter.Format(u.MaxEN)
                        + " " + Expression.Term("装甲", u) + "=" + SrcFormatter.Format(u.get_Armor(""))
                        + " " + Expression.Term("運動性", u) + "=" + SrcFormatter.Format(u.get_Mobility(""))
                        + " " + Expression.Term("移動力", u) + "=" + SrcFormatter.Format(u.Speed);
                }

                // 交換するアイテムを選択
                GUI.TopItem = top_item2;
                var slotRet = GUI.ListBox(new ListBoxArgs
                {
                    lb_caption = UnitCaption("装備個所を選択 ： "),
                    lb_info = "アイテム               分類",
                    lb_mode = "連続表示,コメント",
                    Items = MakeSlotItems("▽装備解除▽"),
                    HasFlag = true,
                });
                top_item2 = GUI.TopItem;
                if (slotRet == 0)
                {
                    break;
                }

                // 装備を解除する場合
                if (slotRet == n_slot + 1)
                {
                    var removeRet = GUI.ListBox(new ListBoxArgs
                    {
                        lb_caption = UnitCaption("外すアイテムを選択 ： "),
                        lb_info = "アイテム               分類",
                        lb_mode = "連続表示,コメント",
                        Items = MakeSlotItems("▽全て外す▽"),
                        HasFlag = true,
                    });
                    if (removeRet != 0)
                    {
                        if (removeRet <= n_slot)
                        {
                            // 指定されたアイテムを外す
                            if (!string.IsNullOrEmpty(id_list[removeRet]))
                            {
                                u.DeleteItem(SRC.IList.Item(id_list[removeRet]), false);
                            }
                            else if (GeneralLib.LIndex(texts[removeRet], 1) == ":")
                            {
                                if (!string.IsNullOrEmpty(id_list[removeRet - 1]))
                                {
                                    u.DeleteItem(SRC.IList.Item(id_list[removeRet - 1]), false);
                                }
                            }
                        }
                        else
                        {
                            // 全てのアイテムを外す
                            for (var i = 1; i <= n_slot; i++)
                            {
                                if (!flags[i] && !string.IsNullOrEmpty(id_list[i]))
                                {
                                    u.DeleteItem(SRC.IList.Item(id_list[i]), false);
                                }
                            }
                        }

                        if (string.IsNullOrEmpty(Map.MapFileName))
                        {
                            u.FullRecover();
                        }

                        if (GUI.MainFormVisible)
                        {
                            SRC.GUIStatus.DisplayUnitStatus(u);
                        }
                    }
                    continue;
                }

                // 交換するアイテムの装備個所
                var iid = id_list[slotRet];
                string ipart;
                if (!string.IsNullOrEmpty(iid))
                {
                    ipart = SRC.IList.Item(iid).Part();
                }
                else
                {
                    ipart = GeneralLib.LIndex(texts[slotRet], 2);
                }

                // 空きスロットを調べておく
                var is_right_hand_available = true;
                var is_left_hand_available = true;
                var empty_slot = 0;
                switch (ipart ?? "")
                {
                    case "右手":
                    case "左手":
                    case "片手":
                    case "両手":
                    case "盾":
                        foreach (var itm in u.ItemList)
                        {
                            if (itm.Part() == "片手")
                            {
                                if (itm.IsFix)
                                {
                                    if (is_right_hand_available)
                                    {
                                        is_right_hand_available = false;
                                    }
                                    else
                                    {
                                        is_left_hand_available = false;
                                    }
                                }
                            }
                            else if (itm.Part() == "盾")
                            {
                                if (itm.IsFix)
                                {
                                    is_left_hand_available = false;
                                }
                            }
                        }
                        break;

                    case "右肩":
                    case "左肩":
                    case "肩":
                        empty_slot = 2;
                        foreach (var itm in u.ItemList)
                        {
                            if (itm.Part() == "肩" && itm.IsFix)
                            {
                                empty_slot = empty_slot - 1;
                            }
                        }
                        break;

                    case "強化パーツ":
                    case "アイテム":
                        empty_slot = u.MaxItemNum();
                        foreach (var itm in u.ItemList)
                        {
                            if ((itm.Part() == "強化パーツ" || itm.Part() == "アイテム") && itm.IsFix)
                            {
                                empty_slot = empty_slot - itm.Size();
                            }
                        }
                        break;

                    default:
                        empty_slot = 0;
                        foreach (var fd in u.Features)
                        {
                            if (fd.Name == "ハードポイント" && fd.Data == ipart)
                            {
                                empty_slot = VBMath.CLng(empty_slot + fd.FeatureLevel);
                            }
                        }
                        if (empty_slot == 0)
                        {
                            empty_slot = 1;
                        }
                        foreach (var itm in u.ItemList)
                        {
                            if (itm.Part() == ipart && itm.IsFix)
                            {
                                empty_slot = empty_slot - itm.Size();
                            }
                        }
                        break;
                }

                while (true)
                {
                    // 装備可能なアイテムを調べる
                    var item_list = new List<string>();
                    foreach (var it in SRC.IList.List)
                    {
                        if (!it.Exist)
                        {
                            continue;
                        }

                        // 装備スロットが空いている？
                        var part = it.Part();
                        var ok = true;
                        switch (ipart ?? "")
                        {
                            case "右手":
                            case "左手":
                            case "片手":
                            case "両手":
                                switch (part)
                                {
                                    case "両手":
                                        ok = is_right_hand_available && is_left_hand_available;
                                        break;
                                    case "片手":
                                        ok = u.IsFeatureAvailable("両手持ち")
                                            ? (is_right_hand_available || is_left_hand_available)
                                            : is_right_hand_available;
                                        break;
                                    case "盾":
                                        ok = is_left_hand_available;
                                        break;
                                    default:
                                        ok = false;
                                        break;
                                }
                                break;

                            case "盾":
                                switch (part)
                                {
                                    case "両手":
                                        ok = is_right_hand_available && is_left_hand_available;
                                        break;
                                    case "片手":
                                        ok = u.IsFeatureAvailable("両手持ち")
                                            && (is_right_hand_available || is_left_hand_available);
                                        break;
                                    case "盾":
                                        ok = is_left_hand_available;
                                        break;
                                    default:
                                        ok = false;
                                        break;
                                }
                                break;

                            case "右肩":
                            case "左肩":
                            case "肩":
                                ok = (part == "両肩" || part == "肩")
                                    && !(part == "両肩" && empty_slot < 2);
                                break;

                            case "強化パーツ":
                            case "アイテム":
                                ok = (part == "強化パーツ" || part == "アイテム") && empty_slot >= it.Size();
                                break;

                            default:
                                ok = part == ipart && empty_slot >= it.Size();
                                break;
                        }
                        if (!ok)
                        {
                            continue;
                        }

                        if (it.Unit != null)
                        {
                            var cf = it.Unit.CurrentForm();
                            // 離脱したユニットが装備している
                            if (cf.Status == "離脱")
                            {
                                continue;
                            }

                            // 敵ユニットが装備している
                            if (cf.Party != "味方")
                            {
                                continue;
                            }

                            // 呪われているので外せない……
                            if (it.IsFeatureAvailable("呪い"))
                            {
                                continue;
                            }
                        }

                        // 既に登録済み？
                        if (item_list.Contains(it.Name))
                        {
                            continue;
                        }

                        // 装備可能？
                        if (!u.IsAbleToEquip(it))
                        {
                            continue;
                        }

                        item_list.Add(it.Name);
                    }

                    // 装備可能なアイテムの一覧を表示
                    var choices = new List<ListBoxItem>();
                    var strkey_list = new List<string>();
                    foreach (var iname in item_list)
                    {
                        var idata = SRC.IDList.Item(iname);
                        var msg = GeneralLib.RightPaddedString(idata.Nickname, 22) + " ";
                        if (idata.IsFeatureAvailable("大型アイテム"))
                        {
                            msg += GeneralLib.RightPaddedString(idata.Part + "[" + SrcFormatter.Format(idata.Size()) + "]", 15);
                        }
                        else
                        {
                            msg += GeneralLib.RightPaddedString(idata.Part, 15);
                        }

                        // アイテムの数をカウント
                        // inum: 使える数（未装備と、離脱していないユニットが装備している呪われていないもの）
                        // inum2: 未装備の数
                        var inum = 0;
                        var inum2 = 0;
                        foreach (var it in SRC.IList.List)
                        {
                            if (it.Name == iname && it.Exist)
                            {
                                if (it.Unit is null)
                                {
                                    inum++;
                                    inum2++;
                                }
                                else if (it.Unit.CurrentForm().Status != "離脱")
                                {
                                    if (!it.IsFeatureAvailable("呪い"))
                                    {
                                        inum++;
                                    }
                                }
                            }
                        }

                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(inum2), 2) + "/"
                            + GeneralLib.LeftPaddedString(SrcFormatter.Format(inum), 2);
                        choices.Add(new ListBoxItem(msg, idata.Name)
                        {
                            ListItemComment = idata.Comment,
                        });
                        strkey_list.Add(idata.KanaName);
                    }

                    // アイテムを名前（読み仮名）順にソート
                    SortAscendingText(choices, strkey_list);

                    // 装備するアイテムの種類を選択
                    var itemRet = GUI.ListBox(new ListBoxArgs
                    {
                        lb_caption = UnitCaption("装備するアイテムを選択 ： "),
                        Items = choices,
                        lb_info = "アイテム               分類            数量",
                        lb_mode = "連続表示,コメント",
                    });

                    // キャンセルされた？
                    if (itemRet == 0)
                    {
                        break;
                    }

                    var selectedName = choices[itemRet - 1].ListItemID;

                    // 未装備のアイテムがあるかどうか探す
                    var unused = SRC.IList.List.FirstOrDefault(x => x.Name == selectedName && x.Exist && x.Unit is null);
                    if (unused != null)
                    {
                        // 未装備の装備が見つかったのでそれを装備
                        EquipItem(u, iid, unused);
                        break;
                    }

                    // 選択されたアイテムを列挙
                    var sources = new List<ListBoxItem>();
                    if (!SRC.IDList.Item(selectedName).IsFeatureAvailable("呪い"))
                    {
                        foreach (var it in SRC.IList.List)
                        {
                            if (it.Name != selectedName || !it.Exist)
                            {
                                continue;
                            }
                            if (it.Unit is null)
                            {
                                continue;
                            }

                            var cf = it.Unit.CurrentForm();
                            if (cf.Status == "離脱")
                            {
                                continue;
                            }
                            if (cf.Party != "味方")
                            {
                                continue;
                            }

                            string msg;
                            if (!Expression.IsOptionDefined("等身大基準") && cf.CountPilot() > 0)
                            {
                                msg = GeneralLib.RightPaddedString(cf.Nickname0, 36) + " " + cf.MainPilot().get_Nickname(false);
                            }
                            else
                            {
                                msg = cf.Nickname0;
                            }
                            sources.Add(new ListBoxItem(msg, it.ID)
                            {
                                ListItemComment = ItemListComment(cf),
                            });
                        }
                    }

                    // どのアイテムを装備するか選択
                    // 元の SRC は一覧の 1 番目のアイテムの名前を出す（一覧が空なら範囲外を読む）。空ならアイテムのデータの名前を出す
                    var itemNickname = sources.Count > 0
                        ? SRC.IList.Item(sources[0].ListItemID).Nickname()
                        : SRC.IDList.Item(selectedName).Nickname;
                    GUI.TopItem = 1;
                    var unitRet = GUI.ListBox(new ListBoxArgs
                    {
                        lb_caption = UnitCaption(itemNickname + "の入手先を選択 ： "),
                        Items = sources,
                        lb_info = Expression.IsOptionDefined("等身大基準")
                            ? "ユニット"
                            : "ユニット                             パイロット",
                        lb_mode = "連続表示,コメント",
                    });

                    // アイテムを交換
                    if (unitRet > 0)
                    {
                        EquipItem(u, iid, SRC.IList.Item(sources[unitRet - 1].ListItemID));
                        break;
                    }
                    // キャンセルされたら装備するアイテムの選択に戻る
                }
            }

            // ユニットがあらかじめ選択されている場合
            // (ユニットステータスからのアイテム交換時)
            if (selected_unit is object)
            {
                GUI.CloseListBox();
                GUI.ReduceListBoxHeight();
                GUI.EnlargeListBoxWidth();
                return;
            }

            goto Beginning;
        }

        // アイテム itm を u に装備する（iid は外すアイテム）。元の SRC の ExchangeItemCommand のアイテムの交換
        private void EquipItem(Unit u, string iid, Item itm)
        {
            if (!string.IsNullOrEmpty(iid))
            {
                u.DeleteItem(SRC.IList.Item(iid));
            }

            if (itm.Unit != null)
            {
                itm.Unit.DeleteItem(itm);
            }

            // 呪いのアイテムを装備……
            if (itm.IsFeatureAvailable("呪い"))
            {
                GUI.Confirm(itm.Nickname() + "は呪われていた！", "", GuiConfirmOption.Ok);
            }

            u.AddItem(itm);
            if (string.IsNullOrEmpty(Map.MapFileName))
            {
                u.FullRecover();
            }

            if (GUI.MainFormVisible)
            {
                SRC.GUIStatus.DisplayUnitStatus(u);
            }
        }

        // 換装コマンド
        public void ExchangeFormCommand()
        {
        Beginning:
            ;

            // 元の SRC は Beginning で top_item を 1 に戻す
            var top_item = 1;

            // 換装可能なユニットのリストを作成
            var units = SRC.UList.Items
                // 待機中の味方ユニット？
                .Where(u => !(u.Party0 != "味方" || u.Status != "待機"))
                // 換装能力を持っている？
                .Where(u => u.IsFeatureAvailable("換装"))
                // いずれかの形態に換装可能？
                .Where(u => u.Feature("換装").DataL.Any(uname => u.OtherForm(uname)?.IsAvailable() ?? false))
                .Select(u =>
                {
                    // ユニットのステータスを表示
                    string msg;
                    if (Expression.IsOptionDefined("等身大基準"))
                    {
                        msg = GeneralLib.RightPaddedString(u.Nickname0, 37) + RankString(u.Rank);
                    }
                    else
                    {
                        msg = GeneralLib.RightPaddedString(u.Nickname0, 33) + RankString(u.Rank);
                    }
                    // ユニットに関する情報
                    msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxHP), 6)
                        + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxEN), 5)
                        + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Armor("")), 5)
                        + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Mobility("")), 5);
                    if (u.CountPilot() > 0)
                    {
                        // 等身大基準の場合はパイロットレベル、そうでなければパイロット名を表示
                        if (Expression.IsOptionDefined("等身大基準"))
                        {
                            msg += "  " + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MainPilot().Level), 6);
                        }
                        else
                        {
                            msg += "  " + u.MainPilot().get_Nickname(false);
                        }
                    }

                    return new ListBoxItem(msg, u.ID)
                    {
                        // ユニットに装備されているアイテムをコメント欄に列記
                        ListItemComment = ItemListComment(u),
                    };
                })
                .ToList();

            // リストをユニットのＨＰでソート（元の SRC と同じ選択ソート）
            // 元の SRC は最大値を Integer に入れるので、ＨＰが 32767 を超えると桁が落ちる。SRC# は落とさない
            SortDescending(units, units.Select(x => (long)SRC.UList.Item(x.ListItemID).MaxHP).ToList());

            // 換装するユニットを選択
            string caption = "ユニット選択";
            string info;
            if (Expression.IsOptionDefined("等身大基準"))
            {
                info = "ユニット                         "
                    + Expression.Term("ランク", null, 6) + "   "
                    + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                    + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", null, 4) + " レベル";
            }
            else
            {
                info = "ユニット                     "
                    + Expression.Term("ランク", null, 6) + "   "
                    + Expression.Term("ＨＰ", null, 4) + " " + Expression.Term("ＥＮ", null, 4) + " "
                    + Expression.Term("装甲", null, 4) + " " + Expression.Term("運動", null, 4) + " パイロット";
            }
            GUI.TopItem = top_item;
            var ret = GUI.ListBox(new ListBoxArgs
            {
                Items = units.ToList(),
                lb_caption = caption,
                lb_info = info,
                lb_mode = "連続表示,コメント",
            });

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
                        u.BossRank = baseU.BossRank;
                        u.Update();
                        // ユニットのステータスを表示
                        var msg = "";
                        msg += (baseU.Nickname0 == u.Nickname)
                            ? GeneralLib.RightPaddedString(u.Name, 27)
                            : GeneralLib.RightPaddedString(u.Nickname0, 27);
                        // ユニットに関する情報
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxHP), 6)
                            + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.MaxEN), 5)
                            + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Armor("")), 5)
                            + GeneralLib.LeftPaddedString(SrcFormatter.Format(u.get_Mobility("")), 5)
                            + " " + u.Data.Adaption;

                        // 必要技能を満たし、Disable されていない、合体技でない武器
                        var weapons = u.Weapons
                            .Where(uw => uw.IsWeaponMastered() && !u.IsDisabled(uw.Name) && !uw.IsWeaponClassifiedAs("合"))
                            .ToList();
                        // 最大攻撃力
                        var maxPower = weapons.Select(uw => uw.WeaponPower("")).Append(0).Max();
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(maxPower), 7);
                        // 最大射程
                        var maxRange = weapons.Select(uw => uw.WeaponMaxRange()).Append(0).Max();
                        msg += GeneralLib.LeftPaddedString(SrcFormatter.Format(maxRange), 5);

                        // 換装先が持つ特殊能力一覧（重複する特殊能力は表示しない）
                        var comment = string.Concat(u.Features
                                    .Select(fd => fd.FeatureName(u))
                                    .Where(x => !string.IsNullOrEmpty(x))
                                    .Distinct()
                                    .Select(x => x + " "));
                        return new ListBoxItem(msg, u.Name)
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
                        + Expression.Term("ＨＰ", baseU, 4) + " " + Expression.Term("ＥＮ", baseU, 4) + " "
                        + Expression.Term("装甲", baseU, 4) + " " + Expression.Term("運動", baseU, 4) + " 適応 攻撃力 射程",
                    lb_mode = "連続表示,コメント",
                });
                // キャンセル？
                if (tfRet == 0)
                {
                    goto Beginning;
                }

                // 換装を実施（元の SRC と同じく形態の名前で指定する）
                baseU.Transform(tfUnits[tfRet - 1].ListItemID);
            }

            goto Beginning;
        }

        // ステータスコマンド中かどうかを返す
        public bool InStatusCommand()
        {
            if (Map.IsStatusView)
            {
                // 元の SRC は InStr(ScenarioFileName, "\ユニットステータス表示.eve") で調べる。
                // SRC# はどの OS でも取れるよう、区切りを / と \ の両方とみなしてファイル名を取り出す
                var fname = SRC.ScenarioFileName ?? "";
                fname = fname.Substring(Math.Max(fname.LastIndexOf('/'), fname.LastIndexOf('\\')) + 1).ToLower();
                if (fname == "ユニットステータス表示.eve"
                    || fname == "パイロットステータス表示.eve"
                    || SRC.IsSubStage)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
