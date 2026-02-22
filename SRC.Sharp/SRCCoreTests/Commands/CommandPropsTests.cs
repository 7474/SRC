using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command クラスの基本プロパティのユニットテスト
    /// </summary>
    [TestClass]
    public class CommandPropsTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // CommandState プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CommandState_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Commands.CommandState);
        }

        [TestMethod]
        public void CommandState_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void CommandState_CanBeOverwritten()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "移動先選択";
            src.Commands.CommandState = "攻撃先選択";
            Assert.AreEqual("攻撃先選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void CommandState_CanBeSetToEmpty()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "";
            Assert.AreEqual("", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // WaitClickMode プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WaitClickMode_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Commands.WaitClickMode);
        }

        [TestMethod]
        public void WaitClickMode_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.Commands.WaitClickMode = true;
            Assert.IsTrue(src.Commands.WaitClickMode);
        }

        [TestMethod]
        public void WaitClickMode_CanBeToggled()
        {
            var src = CreateSrc();
            src.Commands.WaitClickMode = true;
            src.Commands.WaitClickMode = false;
            Assert.IsFalse(src.Commands.WaitClickMode);
        }

        // ──────────────────────────────────────────────
        // ViewMode プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ViewMode_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void ViewMode_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            Assert.IsTrue(src.Commands.ViewMode);
        }

        // ──────────────────────────────────────────────
        // SelectedCommand プロパティ（SelectedState 経由）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedCommand_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void SelectedCommand_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedCommand = "攻撃";
            Assert.AreEqual("攻撃", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // SelectedX/SelectedY プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedX_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Commands.SelectedX);
        }

        [TestMethod]
        public void SelectedX_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedX = 5;
            Assert.AreEqual(5, src.Commands.SelectedX);
        }

        [TestMethod]
        public void SelectedY_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Commands.SelectedY);
        }

        [TestMethod]
        public void SelectedY_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedY = 7;
            Assert.AreEqual(7, src.Commands.SelectedY);
        }

        // ──────────────────────────────────────────────
        // SelectedWeapon / SelectedTWeapon
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedWeapon_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Commands.SelectedWeapon);
        }

        [TestMethod]
        public void SelectedWeapon_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedWeapon = 3;
            Assert.AreEqual(3, src.Commands.SelectedWeapon);
        }

        [TestMethod]
        public void SelectedTWeapon_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Commands.SelectedTWeapon);
        }

        [TestMethod]
        public void SelectedTWeapon_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedTWeapon = 2;
            Assert.AreEqual(2, src.Commands.SelectedTWeapon);
        }

        // ──────────────────────────────────────────────
        // SelectedWeaponName / SelectedTWeaponName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedWeaponName_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Commands.SelectedWeaponName);
        }

        [TestMethod]
        public void SelectedWeaponName_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedWeaponName = "ビームライフル";
            Assert.AreEqual("ビームライフル", src.Commands.SelectedWeaponName);
        }

        [TestMethod]
        public void SelectedTWeaponName_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedTWeaponName = "バズーカ";
            Assert.AreEqual("バズーカ", src.Commands.SelectedTWeaponName);
        }

        // ──────────────────────────────────────────────
        // SelectedDefenseOption
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedDefenseOption_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Commands.SelectedDefenseOption);
        }

        [TestMethod]
        public void SelectedDefenseOption_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedDefenseOption = "防御";
            Assert.AreEqual("防御", src.Commands.SelectedDefenseOption);
        }

        // ──────────────────────────────────────────────
        // SelectedAbility / SelectedAbilityName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedAbility_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Commands.SelectedAbility);
        }

        [TestMethod]
        public void SelectedAbility_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedAbility = 2;
            Assert.AreEqual(2, src.Commands.SelectedAbility);
        }

        [TestMethod]
        public void SelectedAbilityName_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedAbilityName = "修理装置";
            Assert.AreEqual("修理装置", src.Commands.SelectedAbilityName);
        }

        // ──────────────────────────────────────────────
        // SelectedItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedItem_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Commands.SelectedItem);
        }

        [TestMethod]
        public void SelectedItem_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedItem = 5;
            Assert.AreEqual(5, src.Commands.SelectedItem);
        }

        // ──────────────────────────────────────────────
        // SelectedSpecialPower
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedSpecialPower_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Commands.SelectedSpecialPower);
        }

        [TestMethod]
        public void SelectedSpecialPower_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedSpecialPower = "熱血";
            Assert.AreEqual("熱血", src.Commands.SelectedSpecialPower);
        }

        // ──────────────────────────────────────────────
        // SelectedUnitMoveCost
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedUnitMoveCost_DefaultIsZero()
        {
            var src = CreateSrc();
            Assert.AreEqual(0, src.Commands.SelectedUnitMoveCost);
        }

        [TestMethod]
        public void SelectedUnitMoveCost_CanBeSet()
        {
            var src = CreateSrc();
            src.Commands.SelectedUnitMoveCost = 3;
            Assert.AreEqual(3, src.Commands.SelectedUnitMoveCost);
        }

        // ──────────────────────────────────────────────
        // UseSupportAttack / UseSupportGuard
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UseSupportAttack_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Commands.UseSupportAttack);
        }

        [TestMethod]
        public void UseSupportAttack_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.Commands.UseSupportAttack = true;
            Assert.IsTrue(src.Commands.UseSupportAttack);
        }

        [TestMethod]
        public void UseSupportGuard_DefaultIsFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Commands.UseSupportGuard);
        }

        [TestMethod]
        public void UseSupportGuard_CanBeSetTrue()
        {
            var src = CreateSrc();
            src.Commands.UseSupportGuard = true;
            Assert.IsTrue(src.Commands.UseSupportGuard);
        }

        // ──────────────────────────────────────────────
        // AttackUnit / SupportAttackUnit
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttackUnit_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Commands.AttackUnit);
        }

        [TestMethod]
        public void SupportAttackUnit_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Commands.SupportAttackUnit);
        }

        [TestMethod]
        public void SupportGuardUnit_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Commands.SupportGuardUnit);
        }

        [TestMethod]
        public void MovedUnit_DefaultIsNull()
        {
            var src = CreateSrc();
            Assert.IsNull(src.Commands.MovedUnit);
        }

        // ──────────────────────────────────────────────
        // SavedStates
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SavedStates_DefaultIsEmpty()
        {
            var src = CreateSrc();
            var states = src.Commands.SavedStates;
            var count = 0;
            foreach (var s in states) count++;
            Assert.AreEqual(0, count);
        }

        // ──────────────────────────────────────────────
        // SelectedPartners
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedPartners_DefaultIsEmpty()
        {
            var src = CreateSrc();
            Assert.IsNotNull(src.Commands.SelectedPartners);
            Assert.AreEqual(0, src.Commands.SelectedPartners.Count);
        }
    }
}
