using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Lib;
using SRCCore.Models;
using SRCCore.Units;
using SRCCore.VB;
using System.Linq;
using System.Reflection;
using static SRCCore.Coverage.Tests.CommandsCoverage.CommandAttackProcessFixture;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// 元の SRC（Command.bas の StartMoveCommand〜FinishAttackCommand）の動作を期待値にしたテスト。
    /// </summary>
    [TestClass]
    public class CommandAttackMoveVBMatchTests
    {
        private static void Invoke(Command commands, string name)
        {
            typeof(Command).GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(commands, null);
        }

        // ══════════════════════════════════════════════════════
        // StartMoveCommand
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void StartMove_LargeMapOption_DoesNotCenter()
        {
            // VB: If Not IsOptionDefined("大型マップ") Then Center SelectedUnit.X, SelectedUnit.Y
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("自機");
            var ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";
            f.SRC.Expression.DefineGlobalVariable("Option(大型マップ)");

            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));

            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.AreEqual(0, f.Headless.CallsOf("Center").Count());
        }

        [TestMethod]
        public void StartMove_NormalMap_CentersOnUnit()
        {
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("自機");
            var ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));

            CollectionAssert.AreEqual(new object[] { 5, 5 }, f.Headless.CallsOf("Center").Last().Args);
        }

        // ══════════════════════════════════════════════════════
        // FinishMoveCommand / FinishTeleportCommand / FinishJumpCommand の確認
        // ══════════════════════════════════════════════════════

        private static string ConfirmTitleWhenCarrierMovesOntoCarrier(string finishMethod)
        {
            var f = new CommandAttackProcessFixture();
            f.AddUnitData("母艦A", 4, "母艦");
            f.AddUnitData("母艦B", 4, "母艦");
            var mover = f.Deploy("母艦A", "艦長A", "味方", 5, 5);
            f.Deploy("母艦B", "艦長B", "味方", 6, 5);
            f.Commands.SelectedUnit = mover;
            f.Commands.CommandState = "ターゲット選択";
            f.Commands.SelectedX = 6;
            f.Commands.SelectedY = 5;
            f.Headless.Handle("Confirm", _ => GuiDialogResult.Cancel);

            Invoke(f.Commands, finishMethod);

            return (string)f.Headless.CallsOf("Confirm").Single().Args[1];
        }

        [TestMethod]
        public void FinishMove_CarrierOntoCarrier_AsksCombine()
        {
            // VB の FinishMoveCommand: 移動先が母艦でも、移動するユニット自身が母艦なら「合体しますか？」
            Assert.AreEqual("合体", ConfirmTitleWhenCarrierMovesOntoCarrier("FinishMoveCommand"));
        }

        [TestMethod]
        public void FinishTeleport_CarrierOntoCarrier_AsksLanding()
        {
            // VB の FinishTeleportCommand: 移動先が母艦なら（自身が母艦でも）「着艦しますか？」
            Assert.AreEqual("着艦", ConfirmTitleWhenCarrierMovesOntoCarrier("FinishTeleportCommand"));
        }

        [TestMethod]
        public void FinishJump_CarrierOntoCarrier_AsksLanding()
        {
            // VB の FinishJumpCommand: 移動先が母艦なら（自身が母艦でも）「着艦しますか？」
            Assert.AreEqual("着艦", ConfirmTitleWhenCarrierMovesOntoCarrier("FinishJumpCommand"));
        }

        // ══════════════════════════════════════════════════════
        // FinishAttackCommand
        // ══════════════════════════════════════════════════════

        [TestMethod]
        public void FinishAttack_WeaponOrderChangedBeforeAttack_DoesNotAttack()
        {
            // VB: ElseIf wname <> .Weapon(SelectedWeapon).Name Then SelectedWeapon = -1
            // 武器の並びが変わり、選んだ番号の武器の名前が違えば攻撃しない（名前で探し直さない）
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機");
            ud.HP = 100000;
            var first = f.AddWeapon(ud, "前武器");
            f.AddWeapon(ud, "主武器").Precision = 999;
            f.AddWeapon(ud, "後武器");
            var ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            var eud = f.AddUnitData("敵機");
            eud.HP = 100000;
            var enemy = f.Deploy("敵機", "敵兵", "敵", 6, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";
            var w = ally.Weapons.First(x => x.Name == "主武器");
            f.Headless.Handle("WeaponListBox", _ => w);
            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));
            Assert.AreEqual(2, f.Commands.SelectedWeapon);

            // 戦闘の開始（メッセージウィンドウを開く）までに 1 番目の武器が無くなる
            f.Headless.Handle("OpenMessageForm", _ =>
            {
                var col = (SrcCollection<WeaponData>)typeof(UnitData)
                    .GetField("colWeaponData", BindingFlags.Instance | BindingFlags.NonPublic)
                    .GetValue(ud);
                if (col.Remove(first))
                {
                    ally.Update();
                }
                return null;
            });

            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[6, 5], enemy);

            Assert.AreEqual("後武器", ally.Weapon(2).Name);
            Assert.AreEqual(enemy.MaxHP, enemy.HP, "攻撃していない");
            Assert.AreEqual("", f.Commands.SelectedWeaponName);
        }

        private static CommandAttackProcessFixture CreateSyncAttack(int syncLevel, out Unit enemy)
        {
            var f = new CommandAttackProcessFixture();
            var leader = f.AddPilotData("隊長");
            if (syncLevel > 0)
            {
                leader.AddSkill("統率", syncLevel, "", 1);
            }
            var helper = f.AddPilotData("僚機乗り");
            helper.AddSkill("援護攻撃", 1, "", 1);
            // 通常の判定ではほぼクリティカルにならないようにする（発生率は 1% になる）
            leader.Technique = 0;
            helper.Technique = 0;
            f.AddPilotData("敵兵").Technique = 9999;

            var ud = f.AddUnitData("自機");
            ud.HP = 100000;
            f.AddWeapon(ud, "主武器", power: 1000).Precision = 999;
            var hud = f.AddUnitData("僚機");
            hud.HP = 100000;
            f.AddWeapon(hud, "援護武器", power: 1000, maxRange: 2).Precision = 999;
            var eud = f.AddUnitData("敵機");
            eud.HP = 100000;
            eud.Armor = 0;

            var ally = f.Deploy("自機", "隊長", "味方", 5, 5);
            f.Deploy("僚機", "僚機乗り", "味方", 5, 6);
            enemy = f.Deploy("敵機", "敵兵", "敵", 6, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";
            var w = ally.Weapons.Single();
            f.Headless.Handle("WeaponListBox", _ => w);
            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));
            return f;
        }

        // 乱数を固定して攻撃する（Dice(100) が 1 にならない系列で、1% のクリティカルを起こさない）
        private static void AttackWithFixedRandom(CommandAttackProcessFixture f, Unit enemy)
        {
            var savedSeed = GeneralLib.RndSeed;
            try
            {
                GeneralLib.RndSeed = 12345;
                GeneralLib.RndReset();
                f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[6, 5], enemy);
            }
            finally
            {
                GeneralLib.RndSeed = savedSeed;
                GeneralLib.RndReset();
            }
        }

        private static int CountCriticalMessages(CommandAttackProcessFixture f)
        {
            return f.Headless.Calls
                .Count(c => c.Args.OfType<string>().Any(a => a.Contains("クリティカル！")));
        }

        [TestMethod]
        public void FinishAttack_SyncAttackSkill_AttacksInLeaderMode()
        {
            // VB: If Not SupportAttackUnit Is Nothing And .MaxSyncAttack > .UsedSyncAttack Then
            //         .Attack SelectedWeapon, SelectedTarget, "統率", def_mode
            // 統率の攻撃は必ずクリティカルになる（同時援護攻撃も必ずクリティカル）
            var f = CreateSyncAttack(1, out var enemy);

            AttackWithFixedRandom(f, enemy);

            Assert.AreEqual(1, f.Commands.SelectedUnit.UsedSyncAttack, "同時援護攻撃をした");
            Assert.AreEqual(2, CountCriticalMessages(f), "統率の攻撃と同時援護攻撃の両方がクリティカル");
        }

        [TestMethod]
        public void FinishAttack_WithoutSyncAttackSkill_SupportAttackOnlyIsNotForcedCritical()
        {
            // 統率が無ければ通常の攻撃のあとに援護攻撃（どちらもクリティカルにはならない）
            var f = CreateSyncAttack(0, out var enemy);

            AttackWithFixedRandom(f, enemy);

            Assert.AreEqual(0, f.Commands.SelectedUnit.UsedSyncAttack);
            Assert.AreEqual(0, CountCriticalMessages(f));
        }

        private static int RandomCountOfPreemptiveCounter(string counterClass, int readAhead)
        {
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機");
            ud.HP = 100000;
            f.AddWeapon(ud, "主武器", power: 100).Precision = 999;
            var pd = f.AddPilotData("反撃者");
            if (readAhead > 0)
            {
                pd.AddSkill("先読み", readAhead, "", 1);
            }
            var eud = f.AddUnitData("敵機");
            eud.HP = 100000;
            f.AddWeapon(eud, "反撃武器", power: 100, wclass: counterClass).Precision = 999;
            var ally = f.Deploy("自機", "主人公", "味方", 5, 5);
            var enemy = f.Deploy("敵機", "反撃者", "敵", 6, 5);
            f.Commands.SelectedUnit = ally;
            f.Commands.CommandState = "コマンド選択";
            var w = ally.Weapons.Single();
            f.Headless.Handle("WeaponListBox", _ => w);
            f.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            var savedSeed = GeneralLib.RndSeed;
            try
            {
                GeneralLib.RndSeed = 12345;
                GeneralLib.RndReset();
                f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[6, 5], enemy);
                Assert.AreEqual("反撃武器", f.Commands.SelectedTWeaponName);
                return GeneralLib.RndIndex;
            }
            finally
            {
                GeneralLib.RndSeed = savedSeed;
                GeneralLib.RndReset();
            }
        }

        [TestMethod]
        public void FinishAttack_PreemptiveCounterWithFirstStrikeWeapon_ConsumesReadAheadDice()
        {
            // VB: ElseIf .IsWeaponClassifiedAs(SelectedTWeapon, "先") Or .MainPilot.SkillLevel("先読み") >= Dice(16) Or ...
            // VB の Or は短絡しないので、反撃の武器が「先」でも Dice(16) を 1 回呼ぶ。
            // 「先」の武器で先制する場合と、先読み Lv16（必ず先制）で先制する場合とで、乱数の消費は同じになる
            var withFirstStrike = RandomCountOfPreemptiveCounter("格先", 0);
            var withReadAhead = RandomCountOfPreemptiveCounter("格", 16);

            Assert.AreEqual(withReadAhead, withFirstStrike);
        }
    }
}
