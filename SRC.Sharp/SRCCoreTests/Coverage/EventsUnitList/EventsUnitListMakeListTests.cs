using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.Pilots;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.EventsUnitList
{
    /// <summary>
    /// インターミッションの「ユニットリスト」「パイロットステータス」で使う一覧画面の作成
    /// （Event.MakeUnitList / MakePilotListCmd。VB: Event.MakeUnitList / CmdData.ExecMakePilotListCmd）のテスト。
    /// ユニット・パイロットの並び順、マップ上の配置、描画される文字列を確認する。
    /// </summary>
    [TestClass]
    public class EventsUnitListMakeListTests
    {
        private static (SRC src, HeadlessGUI gui) CreateSrc()
        {
            var src = new SRC();
            var gui = new HeadlessGUI();
            gui.Install(src);
            // ステータス表示用のマップは画面サイズで作られる
            src.GUI.MainWidth = 20;
            src.GUI.MainHeight = 45;
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return (src, gui);
        }

        private static int Exec(SRC src, string cmdText)
        {
            var line = new EventDataLine(src.Event.EventData.Count, EventDataSource.Scenario, "test", 0, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd.Exec();
        }

        private static Unit AddUnit(SRC src, string name, int hp = 1000, int en = 100, string kana = null, bool withPilot = true, int level = 1)
        {
            var ud = src.UDList.Add(name);
            ud.Nickname = name;
            ud.KanaName = kana ?? name;
            ud.HP = hp;
            ud.EN = en;
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            var u = src.UList.Add(name, 0, "味方");
            u.Status = "待機";
            if (withPilot)
            {
                AddPilot(src, name + "の乗り手", level).Ride(u);
            }
            return u;
        }

        private static Pilot AddPilot(SRC src, string name, int level = 1, string kana = null, int infight = 100)
        {
            var pd = src.PDList.Add(name);
            pd.Nickname = name;
            pd.KanaName = kana ?? name;
            pd.SP = 10;
            pd.Infight = infight;
            pd.Adaption = "AAAA";
            return src.PList.Add(name, level, "味方");
        }

        private static List<(string msg, int x, int y)> DrawnStrings(HeadlessGUI gui)
        {
            return gui.CallsOf(nameof(IGUI.DrawString))
                .Select(c => ((string)c.Args[0], (int)c.Args[1], (int)c.Args[2]))
                .ToList();
        }

        // ──────────────────────────────────────────────
        // MakeUnitList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MakeUnitList_DefaultSortsByHPDescending_AndWrapsAfterThreeColumns()
        {
            // 既定のソート項目は ＨＰ。HPの高い順に5マス間隔で並べ、x>15 で次の行へ折り返す
            var (src, gui) = CreateSrc();
            var hp1 = AddUnit(src, "機体1", hp: 1000);
            var hp3 = AddUnit(src, "機体3", hp: 3000);
            var hp2 = AddUnit(src, "機体2", hp: 2000);
            var hp4 = AddUnit(src, "機体4", hp: 4000);

            src.Event.MakeUnitList();

            Assert.AreEqual((1, 1), (hp4.x, hp4.y));
            Assert.AreEqual((6, 1), (hp3.x, hp3.y));
            Assert.AreEqual((11, 1), (hp2.x, hp2.y));
            Assert.AreEqual((1, 2), (hp1.x, hp1.y));
            foreach (var u in new[] { hp1, hp2, hp3, hp4 })
            {
                Assert.AreEqual("出撃", u.Status);
                // プレイヤーが操作できないように非操作状態にする
                Assert.IsTrue(u.IsConditionSatisfied("非操作"));
            }

            // ユニットの愛称は (32x+2, 32y-31) に描画する
            var names = DrawnStrings(gui).Where(d => d.msg.StartsWith("機体")).ToList();
            CollectionAssert.AreEqual(
                new[] { ("機体4", 34, 1), ("機体3", 194, 1), ("機体2", 354, 1), ("機体1", 34, 33) },
                names);
            Assert.IsFalse(src.GUI.PermanentStringMode, "描画後は PermanentStringMode を戻す");
        }

        [TestMethod]
        public void MakeUnitList_ListsOnlyDeployedAndStandbyUnits()
        {
            // 一覧に出すのは「出撃」「待機」のユニットのみ（出撃中のものは一旦撤退させてから並べ直す）
            var (src, gui) = CreateSrc();
            var standby = AddUnit(src, "待機機", hp: 1000);
            var deployed = AddUnit(src, "出撃機", hp: 2000);
            deployed.StandBy(3, 3);
            var stored = AddUnit(src, "格納機", hp: 3000);
            stored.Status = "格納";
            var destroyed = AddUnit(src, "破壊機", hp: 4000);
            destroyed.Status = "破壊";

            src.Event.MakeUnitList("ＨＰ");

            Assert.AreEqual((1, 1), (deployed.x, deployed.y));
            Assert.AreEqual((6, 1), (standby.x, standby.y));
            Assert.AreEqual("格納", stored.Status);
            Assert.AreEqual("破壊", destroyed.Status);
            var names = DrawnStrings(gui).Select(d => d.msg).ToList();
            CollectionAssert.DoesNotContain(names, "格納機");
            CollectionAssert.DoesNotContain(names, "破壊機");
        }

        [TestMethod]
        public void MakeUnitList_SortByENDescending_AndModeIsRemembered()
        {
            // ソート項目は次回以降の呼び出し（引数省略時）にも引き継がれる（VB では Static 変数）
            var (src, gui) = CreateSrc();
            var lowEn = AddUnit(src, "低EN", hp: 3000, en: 50);
            var highEn = AddUnit(src, "高EN", hp: 1000, en: 200);

            src.Event.MakeUnitList("ＥＮ");
            Assert.AreEqual(1, highEn.x);
            Assert.AreEqual(6, lowEn.x);

            src.Event.MakeUnitList();
            Assert.AreEqual(1, highEn.x, "引数省略時は前回の ＥＮ ソートのまま");
            Assert.AreEqual(6, lowEn.x);
        }

        [TestMethod]
        public void MakeUnitList_SortByName_UsesKanaNameAscending()
        {
            // 「名称」は読み仮名の昇順
            var (src, gui) = CreateSrc();
            var u = AddUnit(src, "うし", kana: "うし");
            var a = AddUnit(src, "あり", kana: "あり");
            var i = AddUnit(src, "いぬ", kana: "いぬ");

            src.Event.MakeUnitList("名称");

            Assert.AreEqual(1, a.x);
            Assert.AreEqual(6, i.x);
            Assert.AreEqual(11, u.x);
        }

        [TestMethod]
        public void MakeUnitList_SortByLevel_DrawsLevelUnderNickname()
        {
            // 「レベル」はメインパイロットのレベル順で、愛称の下 (32y-15) に "Lv" + レベルを表示
            var (src, gui) = CreateSrc();
            var lv3 = AddUnit(src, "機体A", level: 3);
            var lv12 = AddUnit(src, "機体B", level: 12);

            src.Event.MakeUnitList("レベル");

            Assert.AreEqual(1, lv12.x);
            Assert.AreEqual(6, lv3.x);
            var drawn = DrawnStrings(gui);
            CollectionAssert.Contains(drawn, ("Lv12", 34, 17));
            CollectionAssert.Contains(drawn, ("Lv3", 194, 17));
        }

        [TestMethod]
        public void MakeUnitList_UnitWithoutPilot_GetsDummyPilot()
        {
            // パイロットが乗っていないユニットにはステータス表示用のダミーパイロットを乗せる
            var (src, gui) = CreateSrc();
            var empty = AddUnit(src, "無人機", withPilot: false);

            src.Event.MakeUnitList("ＨＰ");

            Assert.AreEqual(1, empty.CountPilot());
            Assert.AreEqual("ステータス表示用ダミーパイロット(ザコ)", empty.MainPilot().Name);
            Assert.AreEqual("出撃", empty.Status);
        }

        [TestMethod]
        public void MakeUnitListCmd_PassesSortModeArgument()
        {
            // MakeUnitList コマンドは引数をソート項目として一覧を作る
            var (src, gui) = CreateSrc();
            var lowEn = AddUnit(src, "低EN", hp: 3000, en: 50);
            var highEn = AddUnit(src, "高EN", hp: 1000, en: 200);

            var ret = Exec(src, "MakeUnitList ＥＮ");

            Assert.AreEqual(1, ret);
            Assert.AreEqual(1, highEn.x);
            Assert.AreEqual(6, lowEn.x);
        }

        // ──────────────────────────────────────────────
        // MakePilotList
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MakePilotList_SortsByLevel_PlacesOnDummyUnitsEveryThreeColumns()
        {
            // パイロットはレベルの高い順に3マス間隔で並べ、x>15 で折り返す。
            // ユニットに乗っていないパイロットはステータス表示用ダミーユニットに乗せる
            var (src, gui) = CreateSrc();
            var pilots = Enumerable.Range(1, 6).Select(i => AddPilot(src, "P" + i, level: i)).ToList();

            var ret = Exec(src, "MakePilotList レベル");

            Assert.AreEqual(1, ret);
            var expected = new[] { (1, 1), (4, 1), (7, 1), (10, 1), (13, 1), (1, 2) };
            for (var i = 0; i < 6; i++)
            {
                var p = pilots[5 - i];
                Assert.IsNotNull(p.Unit, p.Name);
                Assert.AreEqual("ステータス表示用ダミーユニット", p.Unit.Name);
                Assert.AreEqual("出撃", p.Unit.Status);
                Assert.IsTrue(p.Unit.IsConditionSatisfied("非操作"));
                Assert.AreEqual(expected[i], (p.Unit.x, p.Unit.y), p.Name);
            }
            var drawn = DrawnStrings(gui);
            CollectionAssert.Contains(drawn, ("P6", 34, 1));
            CollectionAssert.Contains(drawn, ("Lv6", 34, 17));
            CollectionAssert.Contains(drawn, ("P1", 34, 33));
        }

        [TestMethod]
        public void MakePilotList_ExcludesDeadAndAwayPilots()
        {
            // 死亡(Alive=false)や離脱中(Away)のパイロットは一覧に出さない
            var (src, gui) = CreateSrc();
            var alive = AddPilot(src, "生存");
            var dead = AddPilot(src, "死亡");
            dead.Alive = false;
            var away = AddPilot(src, "離脱");
            away.Away = true;

            Exec(src, "MakePilotList レベル");

            Assert.IsNotNull(alive.Unit);
            Assert.IsNull(dead.Unit);
            Assert.IsNull(away.Unit);
            var names = DrawnStrings(gui).Select(d => d.msg).ToList();
            CollectionAssert.Contains(names, "生存");
            CollectionAssert.DoesNotContain(names, "死亡");
            CollectionAssert.DoesNotContain(names, "離脱");
        }

        [TestMethod]
        public void MakePilotList_RecordsBoardedUnitAndMovesPilotToDummy()
        {
            // 待機中のユニットに乗っていたパイロットは「搭乗ユニット[パイロットID]」に元のユニットIDを記録し、
            // 一覧表示用にダミーユニットへ乗せ換える
            var (src, gui) = CreateSrc();
            var u = AddUnit(src, "愛機");
            var p = u.MainPilot();

            Exec(src, "MakePilotList レベル");

            Assert.AreEqual(u.ID, src.Expression.GetValueAsString("搭乗ユニット[" + p.ID + "]"));
            Assert.AreEqual("ステータス表示用ダミーユニット", p.Unit.Name);
            Assert.AreEqual(0, u.CountPilot());
        }

        [TestMethod]
        public void MakePilotList_UsesPilotSpecificStatusUnitWhenDefined()
        {
            // 「パイロット名ステータス表示用ユニット」が定義されていればダミーユニットの代わりにそれを使う
            var (src, gui) = CreateSrc();
            var ud = src.UDList.Add("専用ステータス表示用ユニット");
            ud.Nickname = "専用";
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            var p = AddPilot(src, "専用");

            Exec(src, "MakePilotList レベル");

            Assert.AreEqual("専用ステータス表示用ユニット", p.Unit.Name);
            Assert.AreEqual("出撃", p.Unit.Status);
        }

        [TestMethod]
        public void MakePilotList_SortByName_UsesKanaNameAscending()
        {
            // 「名称」は読み仮名の昇順で、レベルを表示する
            var (src, gui) = CreateSrc();
            var u = AddPilot(src, "宇佐美", level: 9, kana: "うさみ");
            var a = AddPilot(src, "青木", level: 2, kana: "あおき");
            var i = AddPilot(src, "伊藤", level: 5, kana: "いとう");

            Exec(src, "MakePilotList 名称");

            Assert.AreEqual(1, a.Unit.x);
            Assert.AreEqual(4, i.Unit.x);
            Assert.AreEqual(7, u.Unit.x);
            CollectionAssert.Contains(DrawnStrings(gui), ("Lv2", 34, 17));
        }

        [TestMethod]
        public void MakePilotList_SortByInfight_DrawsFirstCharOfTermAndValue()
        {
            // 「格闘」は格闘値の高い順で、用語の先頭1文字 + 値を表示する
            var (src, gui) = CreateSrc();
            var weak = AddPilot(src, "弱", infight: 80);
            var strong = AddPilot(src, "強", infight: 150);

            Exec(src, "MakePilotList 格闘");

            Assert.AreEqual(1, strong.Unit.x);
            Assert.AreEqual(4, weak.Unit.x);
            CollectionAssert.Contains(DrawnStrings(gui), ("格" + strong.Infight, 34, 17));
            CollectionAssert.Contains(DrawnStrings(gui), ("格" + weak.Infight, 130, 17));
        }

        // ──────────────────────────────────────────────
        // MakeUnitList（表示内容・件数上限）
        // ──────────────────────────────────────────────

        [DataTestMethod]
        [DataRow("ＨＰ")]
        [DataRow("ＥＮ")]
        [DataRow("名称")]
        public void MakeUnitList_HPENAndNameModes_DrawHPAndENUnderNickname(string mode)
        {
            // ＨＰ・ＥＮ・名称のときは愛称の下に「HP<HP> EN<EN>」を表示する
            var (src, gui) = CreateSrc();
            var u = AddUnit(src, "機体A", hp: 1200, en: 80);

            src.Event.MakeUnitList(mode);

            var expected = src.Expression.Term("HP", u) + u.HP + " " + src.Expression.Term("EN", u) + u.EN;
            CollectionAssert.Contains(DrawnStrings(gui), (expected, 34, 17));
            StringAssert.Contains(expected, "1200");
            StringAssert.Contains(expected, "80");
        }

        [TestMethod]
        public void MakeUnitList_SortByRank_DrawsRankHPAndEN()
        {
            // ランク順のときはランクの高い順に並べ「RK<ランク> HP<HP> EN<EN>」を表示する
            var (src, gui) = CreateSrc();
            var low = AddUnit(src, "低ランク");
            low.Rank = 1;
            var high = AddUnit(src, "高ランク");
            high.Rank = 3;

            src.Event.MakeUnitList("ランク");

            Assert.AreEqual(1, high.x);
            Assert.AreEqual(6, low.x);
            var expected = "RK3 " + src.Expression.Term("HP", high) + high.HP + " " + src.Expression.Term("EN", high) + high.EN;
            CollectionAssert.Contains(DrawnStrings(gui), (expected, 34, 17));
        }

        [TestMethod]
        public void MakeUnitList_MoreThan40Rows_StopsListing()
        {
            // 1行3機で40行まで表示し、それを超えるユニットは一覧に出さない（待機のまま）
            var (src, gui) = CreateSrc();
            var units = Enumerable.Range(0, 121)
                .Select(i => AddUnit(src, "機体" + i, hp: 10000 - i))
                .ToList();

            src.Event.MakeUnitList("ＨＰ");

            var last = units[119];
            Assert.AreEqual("出撃", last.Status);
            Assert.AreEqual((11, 40), (last.x, last.y));
            var overflow = units[120];
            Assert.AreEqual("待機", overflow.Status);
            CollectionAssert.DoesNotContain(DrawnStrings(gui).Select(d => d.msg).ToList(), "機体120");
        }

        // ──────────────────────────────────────────────
        // MakePilotList（搭乗ユニットの扱い）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MakePilotList_PilotOnDummyUnit_KeepsThatUnit_AndStatusUnitIsNotRecorded()
        {
            // ダミーユニット能力を持つユニットに乗っているパイロットはそのまま使う。
            // また名前に「ステータス表示用」を含むユニットの搭乗記録は残さない
            var (src, gui) = CreateSrc();
            var ud = src.UDList.Add("自作ステータス表示用ユニット");
            ud.Nickname = "自作";
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.AddFeature("ダミーユニット");
            var dummy = src.UList.Add("自作ステータス表示用ユニット", 0, "味方");
            dummy.Status = "待機";
            var p = AddPilot(src, "常連");
            p.Ride(dummy);
            var unitCount = src.UList.Items.Count();

            Exec(src, "MakePilotList レベル");

            Assert.AreSame(dummy, p.Unit);
            Assert.AreEqual("出撃", dummy.Status);
            Assert.AreEqual((1, 1), (dummy.x, dummy.y));
            Assert.AreEqual(unitCount, src.UList.Items.Count(), "新しいダミーユニットは作らない");
            Assert.IsFalse(src.Expression.IsVariableDefined("搭乗ユニット[" + p.ID + "]"));
        }

        [TestMethod]
        public void MakePilotList_RecordsBoardedUnitForSupportPilots()
        {
            // サポートパイロットも「搭乗ユニット[パイロットID]」に搭乗ユニットを記録する（VB: CountSupport のループ）
            var (src, gui) = CreateSrc();
            var u = AddUnit(src, "愛機");
            var support = AddPilot(src, "サポート役");
            support.Unit = u;
            u.AddSupport(support);

            Exec(src, "MakePilotList レベル");

            Assert.AreEqual(u.ID, src.Expression.GetValueAsString("搭乗ユニット[" + support.ID + "]"));
            Assert.AreEqual(u.ID, src.Expression.GetValueAsString("搭乗ユニット[" + src.PList.Item("愛機の乗り手").ID + "]"));
        }

        [TestMethod]
        public void MakePilotList_UnknownSortKey_ListsInRegistrationOrderWithoutStatusLine()
        {
            // VB の ExecMakePilotListCmd では未知の項目はキーが全て0となり、並べ替えずに愛称だけを表示する
            var (src, gui) = CreateSrc();
            var p1 = AddPilot(src, "P1", level: 1);
            var p2 = AddPilot(src, "P2", level: 9);

            var ret = Exec(src, "MakePilotList 未知");

            Assert.AreEqual(1, ret);
            Assert.AreEqual(1, p1.Unit.x);
            Assert.AreEqual(4, p2.Unit.x);
            Assert.IsFalse(DrawnStrings(gui).Any(d => d.y == 17));
        }
    }
}
