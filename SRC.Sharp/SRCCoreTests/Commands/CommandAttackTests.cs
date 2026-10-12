using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.attack.cs StartAttackCommand のキャンセルパス等のユニットテスト。
    /// </summary>
    [TestClass]
    public class CommandAttackTests
    {
        private SRC CreateSrc(out MockGUI mock)
        {
            var src = new SRC { GUI = new MockGUI() };
            mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.UnlockGUIHandler = () => { };
            mock.CloseListBoxHandler = () => { };
            var unit = new Unit(src);
            src.PDList.Add("攻撃テスト");
            var pilot = src.PList.Add("攻撃テスト", 1, "味方");
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            src.Commands.SelectedUnit = unit;
            return src;
        }

        [TestMethod]
        public void StartAttack_CommandSelectState_UsesBeforeMoveListAndCancels()
        {
            var src = CreateSrc(out var mock);
            src.Commands.CommandState = "コマンド選択";
            string mode = null;
            mock.WeaponListBoxHandler = (u, l, cap, m, bgm) => { mode = m; return null; };

            src.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.AreEqual("移動前", mode);
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void StartAttack_AfterMoveState_UsesAfterMoveList()
        {
            var src = CreateSrc(out var mock);
            src.Commands.CommandState = "ターゲット選択";
            string mode = null;
            mock.WeaponListBoxHandler = (u, l, cap, m, bgm) => { mode = m; return null; };

            // キャンセル後の CancelCommand は Map 未初期化で例外になり得るが、モード確認には影響しない
            try { src.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃")); } catch { }

            Assert.AreEqual("移動後", mode);
        }

        [TestMethod]
        public void StartAttack_Cancel_ResetsSelectedWeaponAndUnlocks()
        {
            var src = CreateSrc(out var mock);
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedWeapon = 3;
            bool unlocked = false;
            mock.UnlockGUIHandler = () => { unlocked = true; };
            mock.WeaponListBoxHandler = (u, l, cap, m, bgm) => null;

            src.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.AreEqual(0, src.Commands.SelectedWeapon);
            Assert.IsTrue(unlocked);
        }

        [TestMethod]
        public void StartAttack_Cancel_SetsUseSupportAttackTrue()
        {
            var src = CreateSrc(out var mock);
            src.Commands.CommandState = "コマンド選択";
            src.Commands.UseSupportAttack = false;
            mock.WeaponListBoxHandler = (u, l, cap, m, bgm) => null;

            src.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.IsTrue(src.Commands.UseSupportAttack);
        }

        [TestMethod]
        public void StartAttack_WeaponListBoxReceivesSelectedUnit()
        {
            var src = CreateSrc(out var mock);
            src.Commands.CommandState = "コマンド選択";
            Unit received = null;
            string caption = null;
            mock.WeaponListBoxHandler = (u, l, cap, m, bgm) => { received = u; caption = cap; return null; };

            src.Commands.UnitCommand(new UiCommand(Command.AttackCmdID, "攻撃"));

            Assert.AreSame(src.Commands.SelectedUnit, received);
            Assert.AreEqual("武器選択", caption);
        }
    }
}
