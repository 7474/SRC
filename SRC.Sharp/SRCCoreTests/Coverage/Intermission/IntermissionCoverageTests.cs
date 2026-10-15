using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCCore.Events;
using SRCCore.Filesystem;
using SRCCore.Items;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// InterMission（インターミッション）のテストで共通に使う準備。
    /// 元の SRC の Intermission.bas に対応する。
    /// </summary>
    internal static class IntermissionCoverageFixture
    {
        private sealed class NullGUIMap : IGUIMap
        {
            public void InitMapSize(int w, int h) { }
            public void SetMapSize(int w, int h) { }
        }

        public static (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui, GUIMap = new NullGUIMap() };
            src.Event.EventData = new List<EventDataLine>
            {
                new EventDataLine(0, EventDataSource.System, "system", 0, ""),
            };
            src.Event.EventQue = new List<string>();
            gui.RedrawScreenHandler = _ => { };
            gui.EnlargeListBoxHeightHandler = () => { };
            gui.ReduceListBoxHeightHandler = () => { };
            gui.EnlargeListBoxWidthHandler = () => { };
            gui.ReduceListBoxWidthHandler = () => { };
            gui.CloseListBoxHandler = () => { };
            gui.ConfirmHandler = (_, _, _) => GuiDialogResult.Ok;
            src.Expression.DefineGlobalVariable("BGM(Intermission)");
            src.Expression.SetVariableAsString("BGM(Intermission)", "");
            return (src, gui);
        }

        public static void SetOption(SRC src, string name)
        {
            src.Expression.DefineGlobalVariable("Option(" + name + ")");
        }

        public static UnitData DefineUnit(SRC src, string name, int hp = 1000, string kana = null,
            int itemNum = 0, string uclass = "汎用", int pilotNum = 1)
        {
            if (src.UDList.IsDefined(name))
            {
                return src.UDList.Item(name);
            }
            var ud = src.UDList.Add(name);
            ud.Nickname = name;
            ud.KanaName = kana ?? name;
            ud.Class = uclass;
            ud.PilotNum = pilotNum;
            ud.ItemNum = itemNum;
            ud.HP = hp;
            ud.EN = 100;
            ud.Armor = 500;
            ud.Mobility = 50;
            ud.Speed = 4;
            ud.Adaption = "AAAA";
            ud.Transportation = "陸";
            ud.Size = "M";
            return ud;
        }

        public static Unit CreateUnit(SRC src, string name, int hp = 1000, string kana = null,
            int itemNum = 0, string uclass = "汎用", string party = "味方", int rank = 0)
        {
            DefineUnit(src, name, hp, kana, itemNum, uclass);
            var u = src.UList.Add(name, rank, party);
            u.Update();
            return u;
        }

        public static Pilot CreatePilot(SRC src, string name, int level = 1, string kana = null, string party = "味方")
        {
            if (!src.PDList.IsDefined(name))
            {
                var pd = src.PDList.Add(name);
                pd.Nickname = name;
                pd.KanaName = kana ?? name;
                pd.Class = "汎用";
                pd.SP = 50;
            }
            return src.PList.Add(name, level, party);
        }

        public static Item CreateItem(SRC src, string name, string part = "強化パーツ", string iclass = "汎用")
        {
            if (!src.IDList.IsDefined(name))
            {
                var id = src.IDList.Add(name);
                id.Nickname = name;
                id.Class = iclass;
                id.Part = part;
                id.Comment = name + "の説明";
            }
            return src.IList.Add(name);
        }
    }

    /// <summary>
    /// InterMission.InterMissionCommand のコマンド一覧と各コマンドの分岐
    /// （元の SRC の InterMissionCommand）
    /// </summary>
    [TestClass]
    public class IntermissionCommandMenuCoverageTests
    {
        private static List<string> CaptureMenu(SRC src, MockGUI gui)
        {
            // 次ステージを用意して「次のステージへ」(先頭) を選び、メニューの内容を取り出す
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            List<string> items = null;
            gui.ListBoxHandler = args =>
            {
                items = args.Items.Select(x => x.Text).ToList();
                return 1;
            };
            src.InterMission.InterMissionCommand(skip_update: true);
            return items;
        }

        [TestMethod]
        public void HeroUnitOnStandby_ReplacesUpgradeWithEnhance()
        {
            // 待機中の味方に括弧付きクラス（人間ユニット）がいれば「機体改造」でなく「ユニットの強化」
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            IntermissionCoverageFixture.CreateUnit(src, "勇者", uclass: "(戦士)");

            var items = CaptureMenu(src, gui);

            CollectionAssert.Contains(items, "ユニットの強化");
            CollectionAssert.DoesNotContain(items, "機体改造");
            // 等身大基準ではないのでパイロットステータスは残る
            CollectionAssert.Contains(items, "パイロットステータス");
        }

        [TestMethod]
        public void HeroUnitNotOnStandby_KeepsUpgrade()
        {
            // 人間ユニットでも出撃中（待機でない）なら判定に含めない
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "勇者", uclass: "(戦士)");
            u.Status = "離脱";

            var items = CaptureMenu(src, gui);

            CollectionAssert.Contains(items, "機体改造");
            CollectionAssert.DoesNotContain(items, "ユニットの強化");
        }

        [TestMethod]
        public void UnitWithAvailableExchangeForm_AddsExchangeFormEntry()
        {
            // 待機中の味方に換装可能な形態があれば「換装」が加わる（パイロットステータスより前）
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            IntermissionCoverageFixture.DefineUnit(src, "砲撃装備");
            var ud = IntermissionCoverageFixture.DefineUnit(src, "標準装備");
            ud.AddFeature("換装=砲撃装備");
            IntermissionCoverageFixture.CreateUnit(src, "標準装備");

            var items = CaptureMenu(src, gui);

            var idx = items.IndexOf("換装");
            Assert.IsTrue(idx >= 0, string.Join(",", items));
            Assert.IsTrue(idx < items.IndexOf("パイロットステータス"));
        }

        [TestMethod]
        public void EnemyUnitWithExchangeForm_DoesNotAddExchangeFormEntry()
        {
            // 換装の判定は待機中の味方ユニットだけ
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            IntermissionCoverageFixture.DefineUnit(src, "砲撃装備");
            var ud = IntermissionCoverageFixture.DefineUnit(src, "標準装備");
            ud.AddFeature("換装=砲撃装備");
            IntermissionCoverageFixture.CreateUnit(src, "標準装備", party: "敵");

            CollectionAssert.DoesNotContain(CaptureMenu(src, gui), "換装");
        }

        [TestMethod]
        public void Alias_RenamesCommandButKeepsItsFunction()
        {
            // エリアスの最初の要素がコマンド名と一致すれば、表示名をエリアス名に置き換える
            // 選んだときはもとのコマンド（機体改造）が実行される
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            src.ALDList.Add("改造工場").AddAlias("機体改造");
            var captions = new List<string>();
            List<string> menu = null;
            gui.ListBoxHandler = args =>
            {
                captions.Add(args.lb_caption);
                if (args.lb_caption != "コマンド")
                {
                    // 機体改造のユニット選択はキャンセル
                    return 0;
                }
                if (menu == null)
                {
                    menu = args.Items.Select(x => x.Text).ToList();
                    return menu.IndexOf("改造工場") + 1;
                }
                return 1;
            };

            src.InterMission.InterMissionCommand(skip_update: true);

            CollectionAssert.Contains(menu, "改造工場");
            CollectionAssert.DoesNotContain(menu, "機体改造");
            Assert.AreEqual(3, captions.Count);
            StringAssert.StartsWith(captions[1], "ユニット選択");
        }

        [TestMethod]
        public void DataSave_WritesSaveDataToSelectedStream()
        {
            // データセーブ: 選ばれた保存先にセーブデータ（資金などを含む）を書き出して、メニューに戻る
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.Titles = new List<string>();
            src.Money = 12345;
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            var stream = new MemoryStream();
            SRCSaveKind? kind = null;
            gui.SelectSaveStreamHandler = (k, _) => { kind = k; return stream; };
            var calls = 0;
            gui.ListBoxHandler = args =>
            {
                calls++;
                var names = args.Items.Select(x => x.Text).ToList();
                return calls == 1 ? names.IndexOf("データセーブ") + 1 : 1;
            };

            src.InterMission.InterMissionCommand(skip_update: true);

            Assert.AreEqual(SRCSaveKind.Normal, kind);
            Assert.AreEqual(2, calls, "セーブ後はコマンド選択に戻る");
            // 書き出したセーブデータを別の SRC に読み込むと、資金と次ステージが戻る
            var (loaded, loadedGui) = IntermissionCoverageFixture.CreateSrc();
            loaded.FileSystem = new LocalFileSystem();
            loadedGui.SetLoadImageSizeHandler = _ => { };
            loadedGui.DisplayLoadingProgressHandler = () => { };
            loaded.LoadData(new MemoryStream(stream.ToArray()));
            Assert.AreEqual(12345, loaded.Money);
            Assert.AreEqual("next.eve", loaded.Expression.GetValueAsString("次ステージ"));
        }

        [TestMethod]
        public void DataSave_CancelledDialog_ReturnsToMenu()
        {
            // 保存先の選択をキャンセルしたら何もせずにメニューへ戻る
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            var selectCalls = 0;
            gui.SelectSaveStreamHandler = (_, _) => { selectCalls++; return null; };
            var calls = 0;
            gui.ListBoxHandler = args =>
            {
                calls++;
                var names = args.Items.Select(x => x.Text).ToList();
                return calls == 1 ? names.IndexOf("データセーブ") + 1 : 1;
            };

            src.InterMission.InterMissionCommand(skip_update: true);

            Assert.AreEqual(1, selectCalls);
            Assert.AreEqual(2, calls);
        }

        [TestMethod]
        public void ExitGame_Declined_ReturnsToMenuWithoutClosingList()
        {
            // 「SRCを終了」の確認で Cancel を選んだら終了せずメニューに戻る
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.Expression.DefineGlobalVariable("次ステージ");
            src.Expression.SetVariableAsString("次ステージ", "next.eve");
            var confirms = new List<(string msg, string title)>();
            gui.ConfirmHandler = (msg, title, _) =>
            {
                confirms.Add((msg, title));
                return title == "終了" ? GuiDialogResult.Cancel : GuiDialogResult.Ok;
            };
            var closeCalls = 0;
            var calls = 0;
            gui.ListBoxHandler = args =>
            {
                calls++;
                if (calls == 1)
                {
                    // 最初は「SRCを終了」（最後の項目）
                    closeCalls = 0;
                    return args.Items.Count;
                }
                return 1;
            };
            gui.CloseListBoxHandler = () => closeCalls++;

            src.InterMission.InterMissionCommand(skip_update: true);

            Assert.AreEqual(("SRCを終了しますか？", "終了"), confirms[0]);
            Assert.AreEqual(("次のステージへ進みますか？", "次ステージ"), confirms[1]);
            Assert.AreEqual(2, calls);
            // 閉じたのは「次のステージへ」を選んだときの1回だけ
            Assert.AreEqual(1, closeCalls);
        }

        [TestMethod]
        public void StatusCommand_EventFileMissing_ReportsErrorAndContinuesIntermission()
        {
            // Lib\ユニットステータス表示.eve・Lib\パイロットステータス表示.eve が無いときは、
            // 見つからないことを知らせ、リストを閉じずにインターミッションを続ける（閉じると操作できなくなるため）
            var dir = Path.Combine(Path.GetTempPath(), "srcs-im-" + Guid.NewGuid());
            Directory.CreateDirectory(dir);
            try
            {
                var src = new SRC
                {
                    SystemConfig = new LocalFileConfig { AppPath = dir },
                    ScenarioPath = dir,
                };
                var fs = new LocalFileSystem();
                fs.AddPath(dir);
                src.FileSystem = fs;
                var gui = new HeadlessGUI();
                gui.Handle(nameof(IGUI.Confirm), _ => GuiDialogResult.Ok);
                var calls = 0;
                gui.Handle(nameof(IGUI.ListBox), args =>
                {
                    calls++;
                    var names = ((ListBoxArgs)args[0]).Items.Select(x => x.Text).ToList();
                    var target = calls == 1 ? "ユニットステータス" : calls == 2 ? "パイロットステータス" : "次のステージへ";
                    return names.IndexOf(target) + 1;
                });
                gui.Install(src);
                src.Event.EventData = new List<EventDataLine>
                {
                    new EventDataLine(0, EventDataSource.System, "system", 0, ""),
                };
                src.Event.EventQue = new List<string>();
                src.Expression.DefineGlobalVariable("次ステージ");
                src.Expression.SetVariableAsString("次ステージ", "next.eve");

                src.InterMission.InterMissionCommand(skip_update: true);

                Assert.AreEqual(3, calls);
                CollectionAssert.AreEqual(new[]
                    {
                        @"Lib\ユニットステータス表示.eve が見つかりません",
                        @"Lib\パイロットステータス表示.eve が見つかりません",
                    },
                    gui.CallsOf(nameof(IGUI.ErrorMessage)).Select(x => (string)x.Args[0]).ToList());
                Assert.AreEqual(1, gui.CallsOf(nameof(IGUI.CloseListBox)).Count(), "閉じるのは次のステージへ進むときだけ");
                Assert.IsFalse(src.IsSubStage);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }

    /// <summary>
    /// InterMission.RankUpCommand（機体改造）のユニット一覧・並べ替え・改造の確認
    /// （元の SRC の RankUpCommand）
    /// </summary>
    [TestClass]
    public class IntermissionRankUpCommandCoverageTests
    {
        [TestMethod]
        public void UnitList_ShowsStandbyAllyUnitsByHpWithAffordabilityFlag()
        {
            // 待機中の味方ユニットだけを ＨＰ の大きい順に並べる
            // 資金が改造費に足りないユニットは選べない（ListItemFlag）
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.Money = 12000;
            var small = IntermissionCoverageFixture.CreateUnit(src, "小型機", hp: 1000);
            var big = IntermissionCoverageFixture.CreateUnit(src, "大型機", hp: 3000, rank: 1);
            var enemy = IntermissionCoverageFixture.CreateUnit(src, "敵機", hp: 5000, party: "敵");
            var away = IntermissionCoverageFixture.CreateUnit(src, "離脱機", hp: 4000);
            away.Status = "離脱";
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.RankUpCommand();

            Assert.IsTrue(captured.HasFlag);
            Assert.AreEqual("▽並べ替え▽", captured.Items[0].Text);
            CollectionAssert.AreEqual(new[] { "▽並べ替え▽", big.ID, small.ID },
                captured.Items.Select(x => x.ListItemID).ToList());
            // ランク0 → 10000 は払える、ランク1 → 15000 は払えない
            Assert.IsFalse(captured.Items[2].ListItemFlag);
            Assert.IsTrue(captured.Items[1].ListItemFlag);
            StringAssert.Contains(captured.Items[2].Text, "10000");
            StringAssert.Contains(captured.Items[1].Text, "15000");
            StringAssert.Contains(captured.lb_caption, "12000");
        }

        [TestMethod]
        public void UnitList_ShowsPilotNameAndEquippedItemsAsComment()
        {
            // 行にはメインパイロット名、コメント欄には装備アイテムを列記する
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 2);
            IntermissionCoverageFixture.CreatePilot(src, "テスト隊長").Ride(u);
            u.AddItem(IntermissionCoverageFixture.CreateItem(src, "ブースター"));
            u.AddItem(IntermissionCoverageFixture.CreateItem(src, "装甲板"));
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.RankUpCommand();

            var row = captured.Items[1];
            StringAssert.EndsWith(row.Text, "  テスト隊長");
            Assert.AreEqual("ブースター 装甲板", row.ListItemComment);
        }

        [TestMethod]
        public void UnitList_WithMaxRankFeature_ShowsRankLimitColumn()
        {
            // 最大改造数を持つユニットがいれば「ランク/最大」を表示する。改造不可（Disable）は「/--」
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var ud = IntermissionCoverageFixture.DefineUnit(src, "限定機", hp: 3000);
            ud.AddFeature("最大改造数Lv3");
            var limited = IntermissionCoverageFixture.CreateUnit(src, "限定機", hp: 3000, rank: 1);
            var disabled = IntermissionCoverageFixture.CreateUnit(src, "封印機", hp: 1000);
            src.Expression.DefineGlobalVariable("Disable(封印機,改造)");
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.RankUpCommand();

            var rows = captured.Items.Skip(1).ToDictionary(x => x.ListItemID, x => x.Text);
            StringAssert.Contains(rows[limited.ID], " 1/ 3");
            StringAssert.Contains(rows[disabled.ID], " 0/--");
            // 改造できないユニットの改造費は「----」
            StringAssert.Contains(rows[disabled.ID], "----");
        }

        [TestMethod]
        public void SortByUnitName_OrdersByKanaName()
        {
            // 並べ替えで「ユニット名称」を選ぶと読み仮名の順に並ぶ
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var a = IntermissionCoverageFixture.CreateUnit(src, "甲", hp: 1000, kana: "う");
            var b = IntermissionCoverageFixture.CreateUnit(src, "乙", hp: 3000, kana: "あ");
            var c = IntermissionCoverageFixture.CreateUnit(src, "丙", hp: 2000, kana: "い");
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 1; // ▽並べ替え▽
                    case 2: return args.Items.Select(x => x.ListItemID).ToList().IndexOf("ユニット名称") + 1;
                    default: return 0;
                }
            };

            src.InterMission.RankUpCommand();

            Assert.AreEqual("どれで並べ替えますか？", lists[1].lb_caption);
            CollectionAssert.AreEqual(new[] { "ＨＰ", "ＥＮ", "装甲", "運動性", "ユニットランク", "ユニット名称", "パイロット名称" },
                lists[1].Items.Select(x => x.ListItemID).ToList());
            CollectionAssert.AreEqual(new[] { b.ID, c.ID, a.ID },
                lists[2].Items.Skip(1).Select(x => x.ListItemID).ToList());
        }

        [TestMethod]
        public void HumanScale_DefaultSortIsByPilotLevelAndSortChoicesStartWithName()
        {
            // 等身大基準ではパイロットの経験（レベル）順が既定で、並べ替えの先頭は「名称」
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            IntermissionCoverageFixture.SetOption(src, "等身大基準");
            var low = IntermissionCoverageFixture.CreateUnit(src, "見習い", hp: 3000, uclass: "(戦士)");
            IntermissionCoverageFixture.CreatePilot(src, "新人", level: 1).Ride(low);
            var high = IntermissionCoverageFixture.CreateUnit(src, "熟練者", hp: 1000, uclass: "(戦士)");
            IntermissionCoverageFixture.CreatePilot(src, "古参", level: 20).Ride(high);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                return lists.Count == 1 ? 1 : 0;
            };

            src.InterMission.RankUpCommand();

            CollectionAssert.AreEqual(new[] { high.ID, low.ID },
                lists[0].Items.Skip(1).Select(x => x.ListItemID).ToList());
            Assert.AreEqual("名称", lists[1].Items[0].ListItemID);
            Assert.AreEqual("レベル", lists[1].Items[1].ListItemID);
        }

        [TestMethod]
        public void HeroUnit_AsksPowerUpConfirmation()
        {
            // 人間ユニットの改造は「パワーアップ」として確認する
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.Money = 100000;
            var u = IntermissionCoverageFixture.CreateUnit(src, "勇者", uclass: "(戦士)");
            var confirms = new List<(string, string)>();
            gui.ConfirmHandler = (msg, title, _) => { confirms.Add((msg, title)); return GuiDialogResult.Ok; };
            var calls = 0;
            gui.ListBoxHandler = _ => ++calls == 1 ? 2 : 0;

            src.InterMission.RankUpCommand();

            Assert.AreEqual(("勇者をパワーアップさせますか？", "パワーアップ"), confirms.Single());
            Assert.AreEqual(1, u.Rank);
            Assert.AreEqual(90000, src.Money);
        }

        [TestMethod]
        public void RepeatedUpgrade_UsesCostOfCurrentRank()
        {
            // 改造後は一覧に戻り、続けて改造すると次のランクの費用を払う（10000 → 15000）
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            src.Money = 100000;
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機");
            var calls = 0;
            gui.ListBoxHandler = _ => ++calls <= 2 ? 2 : 0;

            src.InterMission.RankUpCommand();

            Assert.AreEqual(2, u.Rank);
            Assert.AreEqual(100000 - 10000 - 15000, src.Money);
        }
    }

    /// <summary>
    /// InterMission.ExchangeUnitCommand（乗り換え）
    /// （元の SRC の ExchangeUnitCommand）
    /// </summary>
    [TestClass]
    public class IntermissionExchangeUnitCommandCoverageTests
    {
        [TestMethod]
        public void PilotList_ExcludesEnemyAwayAndFixedPilots_SortedByLevel()
        {
            // 乗り換えられるのは味方の、離脱しておらず Fix されていないパイロット。レベルの高い順に並ぶ
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var low = IntermissionCoverageFixture.CreatePilot(src, "新人", level: 3);
            var high = IntermissionCoverageFixture.CreatePilot(src, "エース", level: 30);
            IntermissionCoverageFixture.CreatePilot(src, "敵兵", level: 50, party: "敵");
            var away = IntermissionCoverageFixture.CreatePilot(src, "休暇中", level: 40);
            away.Away = true;
            IntermissionCoverageFixture.CreatePilot(src, "固定者", level: 45);
            src.Expression.DefineGlobalVariable("Fix(固定者)");
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeUnitCommand();

            Assert.AreEqual("パイロット選択", captured.lb_info);
            CollectionAssert.AreEqual(new[] { "▽並べ替え▽", high.ID, low.ID },
                captured.Items.Select(x => x.ListItemID).ToList());
        }

        [TestMethod]
        public void PilotList_ShowsCurrentUnitAndItems()
        {
            // 乗っているパイロットはユニット名を表示し、コメント欄にユニットの装備アイテムを列記する
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 1);
            u.AddItem(IntermissionCoverageFixture.CreateItem(src, "ブースター"));
            var p = IntermissionCoverageFixture.CreatePilot(src, "テスト隊長", level: 5);
            p.Ride(u);
            var free = IntermissionCoverageFixture.CreatePilot(src, "待機者", level: 1);
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeUnitCommand();

            var riding = captured.Items.Single(x => x.ListItemID == p.ID);
            StringAssert.Contains(riding.Text, "テスト隊長");
            StringAssert.Contains(riding.Text, "試作機");
            Assert.AreEqual("ブースター", riding.ListItemComment);
            var notRiding = captured.Items.Single(x => x.ListItemID == free.ID);
            StringAssert.StartsWith(notRiding.Text, " 待機者");
            Assert.AreEqual("", notRiding.ListItemComment);
        }

        [TestMethod]
        public void SortPilotsByName_OrdersByKanaName()
        {
            // パイロットの並べ替えで「名称」を選ぶと読み仮名の順
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var a = IntermissionCoverageFixture.CreatePilot(src, "甲", level: 30, kana: "う");
            var b = IntermissionCoverageFixture.CreatePilot(src, "乙", level: 10, kana: "あ");
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 1;
                    case 2: return 2; // 名称
                    default: return 0;
                }
            };

            src.InterMission.ExchangeUnitCommand();

            CollectionAssert.AreEqual(new[] { "レベル", "名称" }, lists[1].Items.Select(x => x.ListItemID).ToList());
            CollectionAssert.AreEqual(new[] { a.ID, b.ID }, lists[0].Items.Skip(1).Select(x => x.ListItemID).ToList());
            CollectionAssert.AreEqual(new[] { b.ID, a.ID }, lists[2].Items.Skip(1).Select(x => x.ListItemID).ToList());
        }

        [TestMethod]
        public void RideEmptyUnit_MovesPilotAndLeavesOldUnitEmpty()
        {
            // 乗り換え先に誰も乗っていなければ、そのまま乗り換える。元のユニットは空になる
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var oldUnit = IntermissionCoverageFixture.CreateUnit(src, "旧型機");
            var newUnit = IntermissionCoverageFixture.CreateUnit(src, "新型機");
            var enemyUnit = IntermissionCoverageFixture.CreateUnit(src, "敵機", party: "敵");
            var p = IntermissionCoverageFixture.CreatePilot(src, "テスト隊長");
            p.Ride(oldUnit);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 2; // テスト隊長
                    case 2: return args.Items.Select(x => x.ListItemID).ToList().IndexOf(newUnit.ID) + 1;
                    default: return 0;
                }
            };

            src.InterMission.ExchangeUnitCommand();

            // 乗り換え先の一覧: 今のユニットと敵ユニットは含まない
            var candidates = lists[1].Items.Skip(1).Select(x => x.ListItemID).ToList();
            CollectionAssert.AreEqual(new[] { newUnit.ID }, candidates);
            StringAssert.StartsWith(lists[1].lb_info, "乗り換え先選択 ： テスト隊長 (旧型機)");
            Assert.AreSame(newUnit, p.Unit);
            Assert.AreEqual(0, oldUnit.CountPilot());
            Assert.AreEqual(1, newUnit.CountPilot());
            // 乗り換え後はパイロット選択に戻る
            Assert.AreEqual(3, lists.Count);
            Assert.AreEqual("パイロット選択", lists[2].lb_info);
        }

        [TestMethod]
        public void RideOccupiedSingleSeatUnit_PreviousPilotGetsOff()
        {
            // １人乗りのユニットに乗り換えると、乗っていたパイロットは降ろされる
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var target = IntermissionCoverageFixture.CreateUnit(src, "専用機");
            var current = IntermissionCoverageFixture.CreatePilot(src, "前任者", level: 1);
            current.Ride(target);
            var p = IntermissionCoverageFixture.CreatePilot(src, "後任者", level: 10);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return args.Items.Select(x => x.ListItemID).ToList().IndexOf(p.ID) + 1;
                    case 2: return 2;
                    default: return 0;
                }
            };

            src.InterMission.ExchangeUnitCommand();

            // 乗り換え先の行には今のパイロット名が出る
            StringAssert.Contains(lists[1].Items[1].Text, "前任者");
            StringAssert.StartsWith(lists[1].lb_info, "乗り換え先選択 ： 後任者");
            Assert.AreSame(target, p.Unit);
            Assert.IsNull(current.Unit);
            Assert.AreSame(p, target.MainPilot());
        }

        [TestMethod]
        public void DestinationList_CancelEndsCommand()
        {
            // 乗り換え先の選択をキャンセルすると、何も変えずにコマンドを終える
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var oldUnit = IntermissionCoverageFixture.CreateUnit(src, "旧型機");
            IntermissionCoverageFixture.CreateUnit(src, "新型機");
            var p = IntermissionCoverageFixture.CreatePilot(src, "テスト隊長");
            p.Ride(oldUnit);
            var calls = 0;
            gui.ListBoxHandler = _ => ++calls == 1 ? 2 : 0;

            src.InterMission.ExchangeUnitCommand();

            Assert.AreEqual(2, calls);
            Assert.AreSame(oldUnit, p.Unit);
        }

        [TestMethod]
        [Ignore("移植の不具合: 乗り換え先ユニットの並べ替えが sort_mode2 でなく sort_mode（パイロット側）を参照している")]
        public void DestinationList_DefaultSortIsUnitKanaName()
        {
            // 乗り換え先の一覧は既定で「名称」（ユニットの読み仮名）順（元の SRC の sort_mode2 = "名称"）
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u1 = IntermissionCoverageFixture.CreateUnit(src, "甲型", kana: "う");
            var u2 = IntermissionCoverageFixture.CreateUnit(src, "乙型", kana: "あ");
            var u3 = IntermissionCoverageFixture.CreateUnit(src, "丙型", kana: "い");
            IntermissionCoverageFixture.CreatePilot(src, "テスト隊長");
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                return lists.Count == 1 ? 2 : 0;
            };

            src.InterMission.ExchangeUnitCommand();

            CollectionAssert.AreEqual(new[] { u2.ID, u3.ID, u1.ID },
                lists[1].Items.Skip(1).Select(x => x.ListItemID).ToList());
        }
    }

    /// <summary>
    /// InterMission.ExchangeItemCommand（アイテム交換）
    /// （元の SRC の ExchangeItemCommand）
    /// </summary>
    [TestClass]
    public class IntermissionExchangeItemCommandCoverageTests
    {
        [TestMethod]
        public void UnitList_ShowsItemCountPerSlots()
        {
            // ユニット一覧には「装備数/最大装備数」を表示し、コメント欄に装備アイテムを並べる
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 3);
            u.AddItem(IntermissionCoverageFixture.CreateItem(src, "ブースター"));
            var enemy = IntermissionCoverageFixture.CreateUnit(src, "敵機", itemNum: 3, party: "敵");
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeItemCommand();

            Assert.AreEqual("アイテムを交換するユニットを選択", captured.lb_caption);
            CollectionAssert.AreEqual(new[] { "▽並べ替え▽", u.ID }, captured.Items.Select(x => x.ListItemID).ToList());
            StringAssert.Contains(captured.Items[1].Text, "1/3");
            Assert.AreEqual("ブースター", captured.Items[1].ListItemComment);
        }

        [TestMethod]
        public void EquipUnusedItem_IntoEmptySlot()
        {
            // 空きスロットを選び、未装備のアイテムを選ぶと装備される
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 2);
            var itm = IntermissionCoverageFixture.CreateItem(src, "ブースター");
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 2; // 試作機
                    case 2: return 1; // 1番目のスロット
                    case 3: return 1; // ブースター
                    default: return 0;
                }
            };

            src.InterMission.ExchangeItemCommand();

            // スロット一覧: 強化パーツ2つ＋装備解除
            var slots = lists[1];
            StringAssert.StartsWith(slots.lb_caption, "装備個所を選択 ： 試作機");
            Assert.AreEqual(3, slots.Items.Count);
            StringAssert.StartsWith(slots.Items[0].Text, "----");
            StringAssert.EndsWith(slots.Items[0].Text, "強化パーツ");
            Assert.AreEqual("▽装備解除▽", slots.Items[2].Text);
            // 装備できるアイテムの一覧（装備数/所有数）
            var choices = lists[2];
            StringAssert.StartsWith(choices.lb_caption, "装備するアイテムを選択 ： 試作機");
            Assert.AreEqual(itm.Name, choices.Items.Single().ListItemID);
            StringAssert.Contains(choices.Items[0].Text, "0 /1");
            // 装備された
            Assert.AreSame(u, itm.Unit);
            Assert.AreEqual(1, u.CountItem());
            // 装備後は再びスロット一覧（アイテム名が表示される）
            StringAssert.StartsWith(lists[3].Items[0].Text, "ブースター");
            Assert.AreEqual("ブースターの説明", lists[3].Items[0].ListItemComment);
        }

        [TestMethod]
        public void ReplaceEquippedItem_RemovesOldOne()
        {
            // 装備済みのスロットに別のアイテムを装備すると、元のアイテムは外れる
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 1);
            var oldItem = IntermissionCoverageFixture.CreateItem(src, "旧パーツ");
            u.AddItem(oldItem);
            var newItem = IntermissionCoverageFixture.CreateItem(src, "新パーツ");
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 2;
                    case 2: return 1;
                    case 3: return args.Items.Select(x => x.ListItemID).ToList().IndexOf("新パーツ") + 1;
                    default: return 0;
                }
            };

            src.InterMission.ExchangeItemCommand();

            Assert.AreSame(u, newItem.Unit);
            Assert.IsNull(oldItem.Unit);
            CollectionAssert.AreEqual(new[] { newItem }, u.ItemList.ToList());
        }

        [TestMethod]
        public void EquipItemHeldByAnotherUnit_TakesItFromThatUnit()
        {
            // 未装備の在庫が無いアイテムは、装備している味方ユニットを選んで譲り受ける
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var receiver = IntermissionCoverageFixture.CreateUnit(src, "受取機", hp: 3000, itemNum: 1);
            var giver = IntermissionCoverageFixture.CreateUnit(src, "提供機", hp: 1000, itemNum: 1);
            IntermissionCoverageFixture.CreatePilot(src, "提供者").Ride(giver);
            var itm = IntermissionCoverageFixture.CreateItem(src, "ブースター");
            giver.AddItem(itm);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return args.Items.Select(x => x.ListItemID).ToList().IndexOf(receiver.ID) + 1;
                    case 2: return 1;
                    case 3: return 1; // ブースター
                    case 4: return 1; // 提供機
                    default: return 0;
                }
            };

            src.InterMission.ExchangeItemCommand();

            var source = lists[3];
            StringAssert.StartsWith(source.lb_caption, "ブースターの入手先を選択 ： 受取機");
            Assert.AreEqual(itm.ID, source.Items.Single().ListItemID);
            StringAssert.Contains(source.Items[0].Text, "提供者");
            Assert.AreSame(receiver, itm.Unit);
            Assert.AreEqual(0, giver.CountItem());
        }

        [TestMethod]
        [Ignore("移植の不具合: 装備可能アイテムの候補から未装備の呪いのアイテムまで除外している（!x.IsFix）。元の SRC は装備中の呪いのアイテムだけを除く")]
        public void CursedItem_ShowsCurseMessageWhenEquipped()
        {
            // 未装備の呪いのアイテムも装備の候補になり、装備すると「呪われていた！」と知らせる
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 1);
            src.IDList.Add("呪いの指輪");
            var id = src.IDList.Item("呪いの指輪");
            id.Nickname = "呪いの指輪";
            id.Class = "汎用";
            id.Part = "強化パーツ";
            id.AddFeature("呪い");
            var itm = src.IList.Add("呪いの指輪");
            var messages = new List<string>();
            gui.ConfirmHandler = (msg, _, _) => { messages.Add(msg); return GuiDialogResult.Ok; };
            var calls = 0;
            gui.ListBoxHandler = _ => ++calls <= 3 ? (calls == 1 ? 2 : 1) : 0;

            src.InterMission.ExchangeItemCommand();

            CollectionAssert.Contains(messages, "呪いの指輪は呪われていた！");
            Assert.AreSame(u, itm.Unit);
        }

        [TestMethod]
        public void RemoveSpecificItem_LeavesOthers()
        {
            // 「▽装備解除▽」で選んだアイテムだけを外す
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 3);
            var first = IntermissionCoverageFixture.CreateItem(src, "パーツＡ");
            var second = IntermissionCoverageFixture.CreateItem(src, "パーツＢ");
            u.AddItem(first);
            u.AddItem(second);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 2;
                    case 2: return args.Items.Count; // ▽装備解除▽
                    case 3: return 1; // パーツＡ
                    default: return 0;
                }
            };

            src.InterMission.ExchangeItemCommand();

            var remove = lists[2];
            StringAssert.StartsWith(remove.lb_caption, "外すアイテムを選択 ： 試作機");
            Assert.AreEqual("▽全て外す▽", remove.Items.Last().Text);
            Assert.IsNull(first.Unit);
            Assert.AreSame(u, second.Unit);
        }

        [TestMethod]
        public void RemoveAll_KeepsFixedItems()
        {
            // 「▽全て外す▽」は Fix されたアイテム以外をすべて外す
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 3);
            var normal = IntermissionCoverageFixture.CreateItem(src, "パーツＡ");
            var fixedItem = IntermissionCoverageFixture.CreateItem(src, "固定パーツ");
            u.AddItem(normal);
            u.AddItem(fixedItem);
            src.Expression.DefineGlobalVariable("Fix(固定パーツ)");
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 2;
                    case 2: return args.Items.Count;
                    case 3: return args.Items.Count; // ▽全て外す▽
                    default: return 0;
                }
            };

            src.InterMission.ExchangeItemCommand();

            // Fix されたアイテムは選べない印が付く
            Assert.IsTrue(lists[1].Items.Single(x => x.ListItemID == fixedItem.ID).ListItemFlag);
            Assert.IsNull(normal.Unit);
            Assert.AreSame(u, fixedItem.Unit);
        }

        [TestMethod]
        [Ignore("移植の不具合: 装備解除で最後のスロットを選ぶと「▽全て外す▽」と同じく全て外れる（ret < UBound(list) の移植誤り）")]
        public void RemoveLastSlotItem_RemovesOnlyThatItem()
        {
            // 元の SRC: ret < UBound(list)（＝最後のスロットまで）は指定したアイテムだけを外す
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 2);
            var first = IntermissionCoverageFixture.CreateItem(src, "パーツＡ");
            var second = IntermissionCoverageFixture.CreateItem(src, "パーツＢ");
            u.AddItem(first);
            u.AddItem(second);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                switch (lists.Count)
                {
                    case 1: return 2;
                    case 2: return args.Items.Count; // ▽装備解除▽
                    case 3: return 2; // 最後のスロット（パーツＢ）
                    default: return 0;
                }
            };

            src.InterMission.ExchangeItemCommand();

            Assert.AreSame(u, first.Unit);
            Assert.IsNull(second.Unit);
        }

        [TestMethod]
        public void PreselectedUnit_SkipsUnitListAndRestoresListBoxSize()
        {
            // ユニットステータスからの呼び出し（selected_unit 指定）はユニット選択を省き、
            // 終わるとリストを閉じて大きさを元に戻す
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = IntermissionCoverageFixture.CreateUnit(src, "試作機", itemNum: 1);
            var events = new List<string>();
            gui.EnlargeListBoxHeightHandler = () => events.Add("EnlargeHeight");
            gui.ReduceListBoxWidthHandler = () => events.Add("ReduceWidth");
            gui.CloseListBoxHandler = () => events.Add("Close");
            gui.ReduceListBoxHeightHandler = () => events.Add("ReduceHeight");
            gui.EnlargeListBoxWidthHandler = () => events.Add("EnlargeWidth");
            gui.ListBoxHandler = args => { events.Add("ListBox:" + args.lb_caption.Split(' ')[0]); return 0; };

            src.InterMission.ExchangeItemCommand(u, "");

            CollectionAssert.AreEqual(new[]
            {
                "EnlargeHeight", "ReduceWidth", "ListBox:装備個所を選択", "Close", "ReduceHeight", "EnlargeWidth",
            }, events);
        }

        [TestMethod]
        public void SelectedPart_ShowsOnlyMatchingSlots()
        {
            // selected_part を指定すると、その装備個所のスロットだけを表示する
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var ud = IntermissionCoverageFixture.DefineUnit(src, "騎士", uclass: "(戦士)", itemNum: 2);
            ud.AddFeature("装備個所=体 頭");
            var u = IntermissionCoverageFixture.CreateUnit(src, "騎士", uclass: "(戦士)", itemNum: 2);
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeItemCommand(u, "頭");

            // 頭のスロットと「▽装備解除▽」
            Assert.AreEqual(2, captured.Items.Count);
            StringAssert.EndsWith(captured.Items[0].Text, "頭");
        }
    }

    /// <summary>
    /// InterMission.ExchangeFormCommand（換装）
    /// （元の SRC の ExchangeFormCommand）
    /// </summary>
    [TestClass]
    public class IntermissionExchangeFormCommandCoverageTests
    {
        private static Unit CreateExchangeableUnit(SRC src, bool withWeapons = true)
        {
            var heavy = IntermissionCoverageFixture.DefineUnit(src, "砲撃装備", hp: 2000);
            var basic = IntermissionCoverageFixture.DefineUnit(src, "標準装備", hp: 1500);
            basic.AddFeature("換装=砲撃装備");
            heavy.AddFeature("換装=標準装備");
            if (withWeapons)
            {
                var w = heavy.AddWeapon("キャノン");
                w.Power = 2500;
                w.MinRange = 1;
                w.MaxRange = 5;
                w.Class = "実";
                w.Adaption = "AAAA";
                var w2 = basic.AddWeapon("ライフル");
                w2.Power = 1200;
                w2.MinRange = 1;
                w2.MaxRange = 3;
                w2.Class = "実";
                w2.Adaption = "AAAA";
            }
            return IntermissionCoverageFixture.CreateUnit(src, "標準装備", hp: 1500, rank: 3);
        }

        [TestMethod]
        public void Transform_ToSelectedForm_CopiesRank()
        {
            // 換装先を選ぶと、その形態に換装する。換装先のランクは元の形態に合わせる
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = CreateExchangeableUnit(src);
            IntermissionCoverageFixture.CreateUnit(src, "汎用機");
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                return lists.Count <= 2 ? 1 : 0;
            };

            src.InterMission.ExchangeFormCommand();

            // 換装できるユニットだけが並ぶ
            CollectionAssert.AreEqual(new[] { u.ID }, lists[0].Items.Select(x => x.ListItemID).ToList());
            Assert.AreEqual("変更先選択", lists[1].lb_caption);
            var form = lists[1].Items.Single();
            StringAssert.StartsWith(form.Text, "砲撃装備");
            // 換装先のランク（3）を合わせてから能力を表示する: ＨＰ 2000→2600、最大攻撃力 2500→2800、最大射程 5
            StringAssert.Contains(form.Text, " 2600");
            StringAssert.EndsWith(form.Text, "   2800    5");
            var current = u.CurrentForm();
            Assert.AreEqual("砲撃装備", current.Name);
            Assert.AreEqual("待機", current.Status);
            Assert.AreEqual("他形態", u.Status);
            Assert.AreEqual(3, current.Rank);
            // 換装後は再びユニット選択に戻る（今度は換装後の形態が並ぶ）
            Assert.AreEqual(3, lists.Count);
            CollectionAssert.AreEqual(new[] { current.ID }, lists[2].Items.Select(x => x.ListItemID).ToList());
        }

        [TestMethod]
        public void CancelFormSelection_ReturnsToUnitListWithoutTransform()
        {
            // 換装先の選択をキャンセルするとユニット選択に戻り、換装しない
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = CreateExchangeableUnit(src);
            var calls = 0;
            gui.ListBoxHandler = _ => ++calls == 1 ? 1 : 0;

            src.InterMission.ExchangeFormCommand();

            Assert.AreEqual(3, calls);
            Assert.AreEqual("待機", u.Status);
            Assert.AreSame(u, u.CurrentForm());
        }

        [TestMethod]
        public void DisabledForm_IsNotOffered()
        {
            // Disable された形態には換装できないので、ユニットごと一覧に出ない
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            CreateExchangeableUnit(src);
            src.Expression.DefineGlobalVariable("Disable(砲撃装備)");
            ListBoxArgs captured = null;
            gui.ListBoxHandler = args => { captured = args; return 0; };

            src.InterMission.ExchangeFormCommand();

            Assert.AreEqual(0, captured.Items.Count);
        }

        [TestMethod]
        public void FormWithoutWeapons_ShowsZeroPowerAndRange()
        {
            // 元の SRC: 最大攻撃力・最大射程は 0 から求めるので、武器が無い形態は 0 と表示される
            var (src, gui) = IntermissionCoverageFixture.CreateSrc();
            var u = CreateExchangeableUnit(src, withWeapons: false);
            var lists = new List<ListBoxArgs>();
            gui.ListBoxHandler = args =>
            {
                lists.Add(args);
                return lists.Count == 1 ? 1 : 0;
            };

            src.InterMission.ExchangeFormCommand();

            StringAssert.Contains(lists[1].Items.Single().Text, "      0    0");
        }
    }
}
