using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// InterMission のコマンド一覧の表示と、機体改造・乗り換え・アイテム交換の残りの分岐
    /// （元の SRC の Intermission.bas）
    /// </summary>
    [TestClass]
    public class IntermissionReviewCoverageTests
    {
        private static Unit CreateSeatedUnit(SRC src, string name, int pilotNum)
        {
            IntermissionCoverageFixture.DefineUnit(src, name, pilotNum: pilotNum);
            var u = src.UList.Add(name, 0, "味方");
            u.Update();
            return u;
        }

        private static Pilots.Pilot CreateSupportPilot(SRC src, string name, int level)
        {
            // パイロットクラスが「(汎用サポート)」だけのパイロットはサポートとしてしか乗れない
            var p = IntermissionCoverageFixture.CreatePilot(src, name, level: level);
            src.PDList.Item(name).Class = "(汎用サポート)";
            return p;
        }

        [TestMethod]
        [Ignore("移植の不具合: インターミッションのコマンド一覧の表題に総ターン数と資金を表示せず「コマンド」を表示している（arglb_caption を使っていない）")]
        public void Menu_CaptionShowsTotalTurnsAndMoney()
        {
            // 元の SRC: ListBox("インターミッション： 総ターン数N 資金M", name_list, "コマンド", "連続表示")
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.TotalTurn = 12;
            src.Money = 3456;
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 1; };

            src.InterMission.InterMissionCommand(skip_update: true);

            Assert.AreEqual("インターミッション： 総ターン数12 資金3456", captured.lb_caption);
            Assert.AreEqual("コマンド", captured.lb_info);
            Assert.AreEqual("連続表示", captured.lb_mode);
        }

        [TestMethod]
        public void UserDefinedCommand_LabelIsFormattedValueOfName()
        {
            // IntermissionCommand(名前) の「名前」は式として評価し、式置換した結果を表示する。
            // コマンドの識別には変数名を使う
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            src.Expression.DefineGlobalVariable("会議名");
            src.Expression.SetVariableAsString("会議名", "第$(回数)回作戦会議");
            src.Expression.DefineGlobalVariable("回数");
            src.Expression.SetVariableAsDouble("回数", 3d);
            src.Expression.DefineGlobalVariable("IntermissionCommand(会議名)");
            src.Expression.SetVariableAsString("IntermissionCommand(会議名)", "meeting.eve");
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 1; };

            src.InterMission.InterMissionCommand(skip_update: true);

            var item = captured.Items.Single(x => x.ListItemID == "IntermissionCommand(会議名)");
            Assert.AreEqual("第3回作戦会議", item.Text);
            // 「SRCを終了」の直前に並ぶ
            Assert.AreEqual(captured.Items.Count - 2, captured.Items.IndexOf(item));
        }

        [TestMethod]
        public void RankUp_Declined_KeepsRankAndMoney()
        {
            // 改造の確認で Cancel を選ぶと改造せずにユニット一覧に戻る
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.Money = 100000;
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機");
            var confirms = new List<string>();
            gui.ConfirmHandler = (msg, _, _) => { confirms.Add(msg); return GuiDialogResult.Cancel; };
            var calls = 0;
            gui.ListBoxHandler = _ => ++calls == 1 ? 2 : 0;

            src.InterMission.RankUpCommand();

            CollectionAssert.AreEqual(new[] { "試作機を改造しますか？" }, confirms);
            Assert.AreEqual(2, calls, "ユニット一覧に戻っていない");
            Assert.AreEqual(0, u.Rank);
            Assert.AreEqual(100000, src.Money);
        }

        [TestMethod]
        public void RankUp_UnitListFlagsUnaffordableUnits()
        {
            // 改造費（ランク0→1 は 10000）が資金を超えるユニットには選べない印を付ける
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機");
            var flags = new List<bool>();
            gui.ListBoxHandler = args =>
            {
                Assert.IsTrue(args.HasFlag);
                flags.Add(args.Items.Single(x => x.ListItemID == u.ID).ListItemFlag);
                return 0;
            };

            src.Money = 9999;
            src.InterMission.RankUpCommand();
            src.Money = 10000;
            src.InterMission.RankUpCommand();

            CollectionAssert.AreEqual(new[] { true, false }, flags);
        }

        [TestMethod]
        public void ExchangeUnit_ExcludesPilotsOfThreeSeatUnitsAndUnitsWithSeveralSupports()
        {
            // ３人乗り以上のユニットのパイロットと、サポートが複数乗っているユニットのパイロットは乗り換えられない。
            // ２人乗りのユニットのパイロットは何番目かを表示し、定員に満たなければ先頭に「-」を付ける
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var trio = CreateSeatedUnit(src, "三人乗り", 3);
            IntermissionCoverageFixture.CreatePilot(src, "三人目", level: 10).Ride(trio);
            var supported = CreateSeatedUnit(src, "支援機", 1);
            var main = IntermissionCoverageFixture.CreatePilot(src, "主操縦", level: 10);
            main.Ride(supported);
            var sup1 = CreateSupportPilot(src, "支援Ａ", 10);
            sup1.Ride(supported, is_support: true);
            var sup2 = CreateSupportPilot(src, "支援Ｂ", 10);
            sup2.Ride(supported, is_support: true);
            var duo = CreateSeatedUnit(src, "複座機", 2);
            var front = IntermissionCoverageFixture.CreatePilot(src, "前席", level: 20);
            front.Ride(duo);
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeUnitCommand();

            CollectionAssert.AreEqual(new[] { "▽並べ替え▽", front.ID },
                captured.Items.Select(x => x.ListItemID).ToList());
            StringAssert.StartsWith(captured.Items[1].Text, "-前席(1)");
            StringAssert.Contains(captured.Items[1].Text, "複座機");
        }

        [TestMethod]
        public void ExchangeUnit_SupportPilotIsMarkedWithAsterisk()
        {
            // サポートパイロットは「*」付きで、乗っているユニットとメインパイロットを表示する
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = CreateSeatedUnit(src, "支援機", 1);
            var main = IntermissionCoverageFixture.CreatePilot(src, "主操縦", level: 20);
            main.Ride(u);
            var sup = CreateSupportPilot(src, "支援Ａ", 10);
            sup.Ride(u, is_support: true);
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeUnitCommand();

            var row = captured.Items.Single(x => x.ListItemID == sup.ID);
            StringAssert.StartsWith(row.Text, "*支援Ａ");
            StringAssert.EndsWith(row.Text, "(主操縦)");
            // メインパイロット側の行には乗っているサポートを括弧で添える
            StringAssert.EndsWith(captured.Items.Single(x => x.ListItemID == main.ID).Text, "(支援Ａ)");
        }

        [TestMethod]
        [Ignore("移植の不具合: パイロット数を括弧で指定した1人乗り（PilotNum=-1）のユニットのパイロットも乗り換え対象にしている。元の SRC は PilotNum <> 1 And Abs(PilotNum) <> 2 で除く")]
        public void ExchangeUnit_ExcludesPilotOfParenthesizedSingleSeatUnit()
        {
            // 元の SRC の ExchangeUnitCommand: 通常のパイロットは PilotNum が 1, 2, (2) のユニットだけ乗り降りできる
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = CreateSeatedUnit(src, "変形機", -1);
            var p = IntermissionCoverageFixture.CreatePilot(src, "操縦者", level: 10);
            p.Ride(u);
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeUnitCommand();

            CollectionAssert.DoesNotContain(captured.Items.Select(x => x.ListItemID).ToList(), p.ID);
        }

        [TestMethod]
        public void RemoveItem_EmptySlotRow_RemovesNothing()
        {
            // 「外すアイテムを選択」で空きスロット（----）を選んでも何も外さない（元の SRC は id_list が空なので何もしない）
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 3);
            var itm = IntermissionCoverageFixture.CreateItem(src, "パーツＡ");
            u.AddItem(itm);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 2;
                    case 2: return args.Items.Count; // ▽装備解除▽
                    case 3: return 2; // 空きスロット
                    default: return 0;
                }
            };

            src.InterMission.ExchangeItemCommand();

            StringAssert.StartsWith(lists[2].Items[1].Text, "----");
            Assert.AreSame(u, itm.Unit);
            Assert.AreEqual(1, u.CountItem());
        }

        [TestMethod]
        public void RemoveItem_OccupiedRow_RemovesTheLargeItem()
        {
            // 大型アイテムが占める2つ目のスロット（:）を選ぶと、その大型アイテムを外す
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 3);
            var large = IntermissionCoverageFixture.CreateItem(src, "大型パーツ");
            src.IDList.Item("大型パーツ").AddFeature("大型アイテムLv1");
            var small = IntermissionCoverageFixture.CreateItem(src, "小型パーツ");
            u.AddItem(large);
            u.AddItem(small);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 2;
                    case 2: return args.Items.Count; // ▽装備解除▽
                    case 3: return 2; // 大型パーツの2つ目のスロット
                    default: return 0;
                }
            };

            src.InterMission.ExchangeItemCommand();

            StringAssert.StartsWith(lists[2].Items[1].Text, " :");
            Assert.IsNull(large.Unit);
            Assert.AreSame(u, small.Unit);
        }
    }
}
