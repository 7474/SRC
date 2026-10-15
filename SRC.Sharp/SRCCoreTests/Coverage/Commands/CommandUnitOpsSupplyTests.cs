using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Config;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Command.unitsupply.cs（修理・補給コマンド）のテスト。
    /// VB 版 Command.bas の StartFixCommand / FinishFixCommand / StartSupplyCommand / FinishSupplyCommand に対応。
    /// </summary>
    [TestClass]
    public class CommandUnitOpsSupplyTests
    {
        // 補給役 (5,5)、要補給の味方 (5,6)、満タンの味方 (4,5)、要補給の敵 (6,5)
        private static CommandUnitOpsFixture CreateSupplyField(out Units.Unit supplier, out Units.Unit needy, out Units.Unit full, out Units.Unit enemy)
        {
            var f = new CommandUnitOpsFixture();
            f.DefineUnit("補給機", features: "補給装置=補給装置 10");
            f.DefineUnit("戦闘機");
            supplier = f.Place("補給機", "補給役", "味方", 5, 5);
            needy = f.Place("戦闘機", "要補給", "味方", 5, 6);
            full = f.Place("戦闘機", "満タン", "味方", 4, 5);
            enemy = f.Place("戦闘機", "敵兵", "敵", 6, 5);
            needy.EN = 40;
            enemy.EN = 40;
            f.Commands.SelectedUnit = supplier;
            return f;
        }

        // 修理役 (5,5)、小破の味方 (5,6)、大破の味方 (5,4)、無傷の味方 (4,5)、損傷した敵 (6,5)
        private static CommandUnitOpsFixture CreateFixField(string fixFeature, out Units.Unit fixer, out Units.Unit light, out Units.Unit heavy, out Units.Unit intact, out Units.Unit enemy)
        {
            var f = new CommandUnitOpsFixture();
            f.DefineUnit("修理機", features: fixFeature);
            f.DefineUnit("戦闘機");
            fixer = f.Place("修理機", "修理役", "味方", 5, 5);
            light = f.Place("戦闘機", "小破", "味方", 5, 6);
            heavy = f.Place("戦闘機", "大破", "味方", 5, 4);
            intact = f.Place("戦闘機", "無傷", "味方", 4, 5);
            enemy = f.Place("戦闘機", "敵兵", "敵", 6, 5);
            light.HP = 900;
            heavy.HP = 200;
            enemy.HP = 100;
            f.Commands.SelectedUnit = fixer;
            return f;
        }

        // ──────────────────────────────────────────────
        // StartSupplyCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartSupply_MasksUnitsThatCannotBeSupplied()
        {
            // 補給範囲は隣接する味方のうち EN・弾数・使用回数のいずれかが減っているユニット
            var f = CreateSupplyField(out var supplier, out var needy, out var full, out var enemy);
            f.Commands.CommandState = "コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));

            var mask = f.Src.Map.MaskData;
            Assert.AreEqual("補給", f.Commands.SelectedCommand);
            Assert.IsFalse(mask[5, 6], "EN が減っている味方は選択できる");
            Assert.IsTrue(mask[4, 5], "満タンの味方は選択できない");
            Assert.IsTrue(mask[6, 5], "敵は選択できない");
            Assert.IsFalse(mask[5, 5], "自分のマスはマスクしない");
            Assert.IsTrue(mask[5, 8], "射程外はマスクされる");
            Assert.IsTrue(f.Called("MaskScreen"));
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            // 自動カーソル移動が無効ならカーソルは動かさない
            Assert.IsFalse(f.Called("MoveCursorPos"));
        }

        [TestMethod]
        public void StartSupply_AfterMove_GoesToAfterMoveTargetSelect()
        {
            // 移動後に使った場合は「移動後ターゲット選択」になる
            var f = CreateSupplyField(out _, out _, out _, out _);
            f.Commands.CommandState = "移動後コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));

            Assert.AreEqual("移動後ターゲット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void StartSupply_AutoMoveCursor_MovesCursorToSuppliableAlly()
        {
            // AutoMoveCursor 有効時は補給可能な味方へカーソルを移し、そのステータスを表示する
            var f = CreateSupplyField(out var supplier, out var needy, out _, out _);
            ((LocalFileConfig)f.Src.SystemConfig).AutoMoveCursor = true;
            f.Commands.CommandState = "コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));

            var cursor = f.Calls("MoveCursorPos").Single();
            Assert.AreEqual("ユニット選択", cursor.Args[0]);
            Assert.AreSame(needy, cursor.Args[1]);
            Assert.AreSame(needy, f.Calls("DisplayUnitStatus").Last().Args[0]);
        }

        [TestMethod]
        public void StartSupply_AutoMoveCursor_NoTarget_StaysOnSelf()
        {
            // 補給できる味方がいなければ自分自身にカーソルを置き、ステータス表示は切り替えない
            var f = CreateSupplyField(out var supplier, out var needy, out _, out _);
            needy.EN = needy.MaxEN;
            ((LocalFileConfig)f.Src.SystemConfig).AutoMoveCursor = true;
            f.Commands.CommandState = "コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));

            Assert.AreSame(supplier, f.Calls("MoveCursorPos").Single().Args[1]);
            Assert.IsFalse(f.Called("DisplayUnitStatus"));
        }

        // ──────────────────────────────────────────────
        // FinishSupplyCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FinishSupply_RestoresEN_LowersMorale_ConsumesEN_AndEndsAction()
        {
            // 補給: 対象の EN・弾数を全快、気力 -10、補給装置の第2引数分の EN を消費して行動終了
            var f = CreateSupplyField(out var supplier, out var needy, out _, out _);
            f.Commands.CommandState = "コマンド選択";
            f.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));
            var morale = needy.MainPilot().Morale;

            f.Commands.ProceedCommand(false, GuiButton.Left, f.Cell(5, 6), needy);

            Assert.AreEqual(needy.MaxEN, needy.EN);
            Assert.AreEqual(morale - 10, needy.MainPilot().Morale);
            Assert.AreEqual(90, supplier.EN);
            Assert.AreEqual(1, supplier.UsedAction, "補給後は行動終了");
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.IsNull(f.Commands.SelectedTarget);
            var messages = f.Calls("DisplaySysMessage").Select(x => (string)x.Args[0]).ToList();
            CollectionAssert.Contains(messages, "補給機は戦闘機に補給装置を使った。");
            CollectionAssert.Contains(messages, "戦闘機の弾数とＥＮが全快した。");
        }

        [TestMethod]
        public void FinishSupply_ClickOnMaskedCell_DoesNothing()
        {
            // マスクされたマス（満タンの味方）を選んでも補給は実行されない
            var f = CreateSupplyField(out var supplier, out _, out var full, out _);
            f.Commands.CommandState = "コマンド選択";
            f.Commands.UnitCommand(new UiCommand(Command.SupplyCmdID, "補給"));

            f.Commands.ProceedCommand(false, GuiButton.Left, f.Cell(4, 5), full);

            Assert.AreEqual(100, supplier.EN);
            Assert.AreEqual(0, supplier.UsedAction);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // StartFixCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartFix_MasksUndamagedAndEnemyUnits()
        {
            // 修理範囲は隣接する味方のうちＨＰが減っているユニット
            var f = CreateFixField("修理装置=修理装置 20", out _, out _, out _, out _, out _);
            f.Commands.CommandState = "コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));

            var mask = f.Src.Map.MaskData;
            Assert.AreEqual("修理", f.Commands.SelectedCommand);
            Assert.IsFalse(mask[5, 6]);
            Assert.IsFalse(mask[5, 4]);
            Assert.IsTrue(mask[4, 5], "無傷の味方は修理できない");
            Assert.IsTrue(mask[6, 5], "敵は修理できない");
            Assert.IsFalse(mask[5, 5]);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void StartFix_ZombieUnit_IsMasked()
        {
            // ゾンビ状態のユニットは修理できない（VB: IsConditionSatisfied("ゾンビ")）
            var f = CreateFixField("修理装置=修理装置 20", out _, out var light, out _, out _, out _);
            light.AddCondition("ゾンビ", -1, cdata: "");
            f.Commands.CommandState = "コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));

            Assert.IsTrue(f.Src.Map.MaskData[5, 6]);
        }

        [TestMethod]
        public void StartFix_AutoMoveCursor_PrefersMostDamagedAlly()
        {
            // 修理ではＨＰの減りが最も大きい味方にカーソルを合わせる
            var f = CreateFixField("修理装置=修理装置 20", out _, out _, out var heavy, out _, out _);
            ((LocalFileConfig)f.Src.SystemConfig).AutoMoveCursor = true;
            f.Commands.CommandState = "移動後コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));

            Assert.AreSame(heavy, f.Calls("MoveCursorPos").Single().Args[1]);
            Assert.AreSame(heavy, f.Calls("DisplayUnitStatus").Last().Args[0]);
            Assert.AreEqual("移動後ターゲット選択", f.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // FinishFixCommand
        // ──────────────────────────────────────────────

        private static (Units.Unit fixer, Units.Unit target) RunFix(string fixFeature)
        {
            var f = CreateFixField(fixFeature, out var fixer, out _, out var heavy, out _, out _);
            f.Commands.CommandState = "コマンド選択";
            f.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));
            f.Commands.ProceedCommand(false, GuiButton.Left, f.Cell(5, 4), heavy);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.AreEqual(1, fixer.UsedAction, "修理後は行動終了");
            return (fixer, heavy);
        }

        [TestMethod]
        public void FinishFix_Level1_Recovers30PercentAndConsumesEN()
        {
            // レベル指定なし／Lv1: 最大ＨＰの 30% 回復。第2引数の EN を消費
            var (fixer, target) = RunFix("修理装置=修理装置 20");
            Assert.AreEqual(200 + 300, target.HP);
            Assert.AreEqual(80, fixer.EN);
        }

        [TestMethod]
        public void FinishFix_Level2_Recovers50Percent()
        {
            var (fixer, target) = RunFix("修理装置Lv2");
            Assert.AreEqual(200 + 500, target.HP);
            Assert.AreEqual(100, fixer.EN, "EN 消費の指定がなければ EN は減らない");
        }

        [TestMethod]
        public void FinishFix_Level3_RecoversFully()
        {
            var (_, target) = RunFix("修理装置Lv3");
            Assert.AreEqual(target.MaxHP, target.HP);
        }

        [TestMethod]
        public void FinishFix_ShowsRecoveredAmount()
        {
            // 回復量をマップ上とメッセージで表示する
            var f = CreateFixField("修理装置=修理装置 20", out _, out _, out var heavy, out _, out _);
            f.Commands.CommandState = "コマンド選択";
            f.Commands.UnitCommand(new UiCommand(Command.FixCmdID, "修理"));

            f.Commands.ProceedCommand(false, GuiButton.Left, f.Cell(5, 4), heavy);

            var draw = f.Calls("DrawSysString").Single();
            Assert.AreEqual(5, draw.Args[0]);
            Assert.AreEqual(4, draw.Args[1]);
            Assert.AreEqual("+300", draw.Args[2]);
            var messages = f.Calls("DisplaySysMessage").Select(x => (string)x.Args[0]).ToList();
            CollectionAssert.Contains(messages, "修理機は戦闘機に修理装置を使った。");
            CollectionAssert.Contains(messages, "戦闘機のＨＰが300回復した。");
        }
    }
}
