using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.Units;
using System.IO;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// SRC.StartTurn（陣営 uparty のフェイズを実行する。元の SRC.bas の StartTurn）
    /// </summary>
    [TestClass]
    public class SrcPlayDataSaveStartTurnTests
    {
        /// <summary>
        /// 3x3 のマップ（MapFileName 設定済み）と Midi\Map1～6.mid を用意する。
        /// </summary>
        private static SrcPlayDataSaveFixture CreateWithMap(string terrainName = "平地", string terrainClass = "陸")
        {
            var f = new SrcPlayDataSaveFixture();
            var map = f.Src.Map;
            map.SetMapSize(3, 3);
            map.MapFileName = Path.Combine(f.ScenarioDir, "test.map");
            var plain = new TerrainData { ID = 0, Name = "平地", Class = "陸", MoveCost = 2 };
            foreach (var x in map.MapData.XRange)
            {
                foreach (var y in map.MapData.YRange)
                {
                    map.MapData[x, y].UnderTerrain = plain;
                }
            }
            // BGM の選択は (1,1) の地形で決まる
            map.MapData[1, 1].UnderTerrain = new TerrainData { ID = 1, Name = terrainName, Class = terrainClass, MoveCost = 2 };
            for (var i = 1; i <= 6; i++)
            {
                f.Write("Midi/Map" + i + ".mid", "dummy");
            }
            return f;
        }

        private static void LoadEvent(SrcPlayDataSaveFixture f, params string[] lines)
        {
            // 先頭行のラベルは IsEventDefined が見落とすため、ダミーのラベルを先頭に置く
            var path = f.Write("test.eve", new[] { "準備:", "Exit", "" }.Concat(lines).ToArray());
            f.Src.Event.LoadEventData(path, "");
        }

        private static Unit AddUnit(SRC src, string name, string party, int level, params string[] features)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.Speed = 5;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            foreach (var feature in features)
            {
                ud.AddFeature(feature);
            }
            src.PDList.Add(name + "パイロット");
            var unit = src.UList.Add(name, 0, party);
            var pilot = src.PList.Add(name + "パイロット", level, party);
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            unit.Update();
            unit.FullRecover();
            return unit;
        }

        private static string BgmName(SrcPlayDataSaveFixture f)
        {
            return Path.GetFileNameWithoutExtension(f.Src.Sound.BGMFileName);
        }

        [TestMethod]
        [Timeout(20000)]
        public void PlayerPhase_WithMap_AdvancesTurnAndRunsTurnEvent()
        {
            // 味方フェイズではターン数（Turn, TotalTurn）を進めてからターンイベントを実行し、
            // 全体ステータスを表示してプレイヤーの操作に移る
            using var f = CreateWithMap();
            LoadEvent(f,
                "ターン 1 味方:",
                "Global ターン実行",
                "Set ターン実行 ターン数",
                "Exit");
            f.Src.TotalTurn = 10;

            f.Src.StartTurn("味方");

            Assert.AreEqual(1, f.Src.Turn);
            Assert.AreEqual(11, f.Src.TotalTurn);
            Assert.AreEqual("味方", f.Src.Stage);
            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("ターン実行"));
            Assert.IsTrue(f.Called(nameof(IGUIStatus.DisplayGlobalStatus)));
            Assert.IsTrue(f.Called(nameof(IGUI.UnlockGUI)));
            Assert.AreEqual("Map1", BgmName(f));
        }

        [TestMethod]
        [Timeout(20000)]
        public void PlayerPhase_StatusView_DoesNotAdvanceTurnNorPlayBgm()
        {
            // マップが無い（ステータス表示用）場合はターンを進めず、ＢＧＭも全体ステータスも出さない
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Midi/Map1.mid", "dummy");

            f.Src.StartTurn("味方");

            Assert.AreEqual(0, f.Src.Turn);
            Assert.AreEqual(0, f.Src.TotalTurn);
            Assert.AreEqual("", f.Src.Sound.BGMFileName ?? "");
            Assert.IsFalse(f.Called(nameof(IGUIStatus.DisplayGlobalStatus)));
        }

        [TestMethod]
        [Timeout(20000)]
        [DataRow("平地", "陸", "Map1")]
        [DataRow("宇宙", "宇宙", "Map5")]
        [DataRow("屋内", "屋内", "Map3")]
        [DataRow("壁", "陸", "Map3")]
        public void PlayerPhase_BgmDependsOnTerrainAt1_1(string name, string cls, string expected)
        {
            // 味方フェイズのＢＧＭ：屋内→Map3、宇宙→Map5、地形名が壁→Map3、それ以外→Map1
            using var f = CreateWithMap(name, cls);

            f.Src.StartTurn("味方");

            Assert.AreEqual(expected, BgmName(f));
        }

        [TestMethod]
        [Timeout(20000)]
        [DataRow("敵", "平地", "陸", "Map2")]
        [DataRow("敵", "宇宙", "宇宙", "Map6")]
        [DataRow("敵", "屋内", "屋内", "Map4")]
        [DataRow("敵", "壁", "陸", "Map4")]
        [DataRow("中立", "平地", "陸", "Map2")]
        [DataRow("ＮＰＣ", "平地", "陸", "Map1")]
        [DataRow("ＮＰＣ", "宇宙", "宇宙", "Map5")]
        [DataRow("ＮＰＣ", "屋内", "屋内", "Map3")]
        [DataRow("ＮＰＣ", "壁", "陸", "Map3")]
        public void CpuPhase_BgmDependsOnPartyAndTerrain(string party, string name, string cls, string expected)
        {
            // 敵(中立)フェイズは Map2/Map4/Map6、ＮＰＣフェイズは味方と同じ Map1/Map3/Map5
            using var f = CreateWithMap(name, cls);
            f.Src.Turn = 1;

            f.Src.StartTurn(party);

            Assert.AreEqual(expected, BgmName(f));
            Assert.AreEqual(party, f.Src.Stage);
            // 味方以外のフェイズではターン数は進めない
            Assert.AreEqual(1, f.Src.Turn);
        }

        [TestMethod]
        [Timeout(20000)]
        public void CpuPhase_RunsTurnEventForParty()
        {
            // 敵フェイズのターンイベントは「ターン n 敵」
            using var f = CreateWithMap();
            LoadEvent(f,
                "ターン 3 敵:",
                "Global 敵ターン",
                "Set 敵ターン 1",
                "Exit");
            f.Src.Turn = 3;

            f.Src.StartTurn("敵");

            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("敵ターン"));
            // CPU の操作後にステータス表示を消す
            Assert.IsTrue(f.Called(nameof(IGUIStatus.ClearUnitStatus)));
        }

        [TestMethod]
        [Timeout(20000)]
        public void CpuPhase_OperatesUnitAndFiresContactAndActionEndEvents()
        {
            // CPU フェイズでは行動可能なユニットを行動させ、行動数を消費した後に
            // 隣接ユニットとの接触イベント、進入イベント、行動終了イベントを順に実行する
            using var f = CreateWithMap();
            LoadEvent(f,
                "接触 敵機パイロット 味方機パイロット:",
                "Global 接触回数",
                "Incr 接触回数",
                "Exit",
                "",
                "進入 敵機パイロット 1 1:",
                "Global 進入回数",
                "Incr 進入回数",
                "Exit",
                "",
                "行動終了 敵機パイロット:",
                "Global 行動終了回数",
                "Incr 行動終了回数",
                "Exit");
            var enemy = AddUnit(f.Src, "敵機", "敵", 1);
            var own = AddUnit(f.Src, "味方機", "味方", 1);
            enemy.StandBy(1, 1);
            own.StandBy(2, 1);
            // その場から動かない
            enemy.Mode = "固定";
            f.Src.Turn = 1;

            f.Src.StartTurn("敵");

            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("接触回数"));
            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("進入回数"));
            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("行動終了回数"));
            Assert.AreEqual(0, enemy.Action);
            Assert.AreSame(own, f.Src.Commands.SelectedTarget);
        }

        [TestMethod]
        [Timeout(20000)]
        public void PlayerPhase_RestsOwnUnitsAndResetsOthersUsedAction()
        {
            // 味方フェイズでは味方ユニットは Rest（ＥＮ回復等）し、それ以外の陣営のユニットは行動回数だけ戻す
            using var f = CreateWithMap();
            var own = AddUnit(f.Src, "味方機", "味方", 1);
            var enemy = AddUnit(f.Src, "敵機", "敵", 1);
            own.StandBy(1, 1);
            enemy.StandBy(3, 3);
            // 1ターン目は回復しないため2ターン目にする
            f.Src.Turn = 1;
            own.EN = 50;
            enemy.EN = 50;
            own.UsedAction = 1;
            enemy.UsedAction = 1;

            f.Src.StartTurn("味方");

            Assert.AreEqual(2, f.Src.Turn);
            Assert.AreEqual(55, own.EN);
            Assert.AreEqual(0, own.UsedAction);
            Assert.AreEqual(50, enemy.EN);
            Assert.AreEqual(0, enemy.UsedAction);
        }

        [TestMethod]
        [Timeout(20000)]
        public void PlayerPhase_StatusView_OnlyResetsUsedAction()
        {
            // ステータス表示用（マップ無し）では Rest せず行動回数だけ戻す
            using var f = new SrcPlayDataSaveFixture();
            f.Src.Map.SetMapSize(3, 3);
            var own = AddUnit(f.Src, "味方機", "味方", 1);
            own.Status = "出撃";
            f.Src.Turn = 5;
            own.EN = 50;
            own.UsedAction = 1;

            f.Src.StartTurn("味方");

            Assert.AreEqual(50, own.EN);
            Assert.AreEqual(0, own.UsedAction);
            Assert.AreEqual(5, f.Src.Turn);
        }

        [TestMethod]
        [Timeout(20000)]
        public void PlayerPhase_CentersMotherShip()
        {
            // 行動可能な味方の母艦ユニットがいればそこを中央に表示し、そのユニットのステータスを表示する
            using var f = CreateWithMap();
            var high = AddUnit(f.Src, "高レベル機", "味方", 50);
            var ship = AddUnit(f.Src, "母艦機", "味方", 1, "母艦");
            high.StandBy(1, 1);
            ship.StandBy(3, 3);

            f.Src.StartTurn("味方");

            var center = f.Gui.CallsOf(nameof(IGUI.Center)).Last();
            Assert.AreEqual(ship.x, center.Args[0]);
            Assert.AreEqual(ship.y, center.Args[1]);
            Assert.AreSame(ship, f.Gui.CallsOf(nameof(IGUIStatus.DisplayUnitStatus)).Last().Args[0]);
            // 母艦を表示した場合はそこで操作に移るので全体ステータスは表示しない
            Assert.IsFalse(f.Called(nameof(IGUIStatus.DisplayGlobalStatus)));
        }

        [TestMethod]
        [Timeout(20000)]
        public void PlayerPhase_CentersHighestLevelUnit()
        {
            // 母艦がいなければメインパイロットのレベルが最も高い味方ユニットを中央に表示する
            using var f = CreateWithMap();
            var low = AddUnit(f.Src, "低レベル機", "味方", 3);
            var high = AddUnit(f.Src, "高レベル機", "味方", 30);
            var enemy = AddUnit(f.Src, "敵機", "敵", 99);
            low.StandBy(1, 1);
            high.StandBy(3, 3);
            enemy.StandBy(1, 3);

            f.Src.StartTurn("味方");

            var center = f.Gui.CallsOf(nameof(IGUI.Center)).Last();
            Assert.AreEqual(high.x, center.Args[0]);
            Assert.AreEqual(high.y, center.Args[1]);
            Assert.IsTrue(f.Called(nameof(IGUIStatus.DisplayGlobalStatus)));
        }

        [TestMethod]
        [Timeout(20000)]
        public void PlayerPhase_DebugOption_ShowsTurnTelopFromTurn2()
        {
            // Option(デバッグ) では2ターン目以降にターン数をテロップ表示する
            using var f = CreateWithMap();
            f.Src.Expression.DefineGlobalVariable("Option(デバッグ)");
            f.Src.Expression.SetVariableAsLong("Option(デバッグ)", 1);
            f.Src.Turn = 1;

            f.Src.StartTurn("味方");

            Assert.AreEqual("ターン2", f.Gui.CallsOf(nameof(IGUI.DisplayTelop)).Single().Args[0]);
        }

        [TestMethod]
        [Timeout(20000)]
        public void PlayerPhase_NoOperablePlayerUnit_RunsCpuPhasesUntilPlayerCanAct()
        {
            // 操作可能な味方ユニットがいなければ敵・中立・ＮＰＣフェイズを実行して次の味方フェイズへ進む。
            // 敵フェイズのターンイベントで敵が撤退すると CPU のユニットもいなくなるので味方フェイズで止まる
            using var f = CreateWithMap();
            LoadEvent(f,
                "ターン 1 敵:",
                "Global 敵フェイズ回数",
                "Incr 敵フェイズ回数",
                "Escape 敵 非同期",
                "Exit");
            var enemy = AddUnit(f.Src, "敵機", "敵", 1);
            enemy.StandBy(3, 3);

            f.Src.StartTurn("味方");

            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("敵フェイズ回数"));
            Assert.AreNotEqual("出撃", enemy.Status);
            Assert.AreEqual(2, f.Src.Turn);
            Assert.AreEqual("味方", f.Src.Stage);
        }

        [TestMethod]
        [Timeout(20000)]
        public void PlayerPhase_ForcePlayerPhaseOption_SkipsCpuPhases()
        {
            // Option(味方フェイズ強制発動) では操作可能な味方がいなくても味方フェイズに留まる
            using var f = CreateWithMap();
            LoadEvent(f,
                "ターン 1 敵:",
                "Global 敵フェイズ回数",
                "Incr 敵フェイズ回数",
                "Exit");
            f.Src.Expression.DefineGlobalVariable("Option(味方フェイズ強制発動)");
            f.Src.Expression.SetVariableAsLong("Option(味方フェイズ強制発動)", 1);
            var enemy = AddUnit(f.Src, "敵機", "敵", 1);
            enemy.StandBy(3, 3);

            f.Src.StartTurn("味方");

            Assert.IsFalse(f.Src.Expression.IsGlobalVariableDefined("敵フェイズ回数"));
            Assert.AreEqual(1, f.Src.Turn);
            Assert.AreEqual("味方", f.Src.Stage);
        }
    }
}
