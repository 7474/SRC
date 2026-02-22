using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// SelectedState クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class SelectedStateTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_SelectedPartners_IsNotNull()
        {
            var state = new SelectedState();
            Assert.IsNotNull(state.SelectedPartners);
        }

        [TestMethod]
        public void Constructor_SelectedPartners_IsEmpty()
        {
            var state = new SelectedState();
            Assert.AreEqual(0, state.SelectedPartners.Count);
        }

        [TestMethod]
        public void Constructor_SelectedUnit_IsNull()
        {
            var state = new SelectedState();
            Assert.IsNull(state.SelectedUnit);
        }

        [TestMethod]
        public void Constructor_SelectedCommand_IsNull()
        {
            var state = new SelectedState();
            Assert.IsNull(state.SelectedCommand);
        }

        [TestMethod]
        public void Constructor_SelectedTarget_IsNull()
        {
            var state = new SelectedState();
            Assert.IsNull(state.SelectedTarget);
        }

        [TestMethod]
        public void Constructor_SelectedX_IsZero()
        {
            var state = new SelectedState();
            Assert.AreEqual(0, state.SelectedX);
        }

        [TestMethod]
        public void Constructor_SelectedY_IsZero()
        {
            var state = new SelectedState();
            Assert.AreEqual(0, state.SelectedY);
        }

        [TestMethod]
        public void Constructor_SelectedWeapon_IsZero()
        {
            var state = new SelectedState();
            Assert.AreEqual(0, state.SelectedWeapon);
        }

        [TestMethod]
        public void Constructor_SelectedAbility_IsZero()
        {
            var state = new SelectedState();
            Assert.AreEqual(0, state.SelectedAbility);
        }

        [TestMethod]
        public void Constructor_SelectedItem_IsZero()
        {
            var state = new SelectedState();
            Assert.AreEqual(0, state.SelectedItem);
        }

        // ──────────────────────────────────────────────
        // プロパティ設定
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectedCommand_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedCommand = "攻撃";
            Assert.AreEqual("攻撃", state.SelectedCommand);
        }

        [TestMethod]
        public void SelectedX_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedX = 5;
            Assert.AreEqual(5, state.SelectedX);
        }

        [TestMethod]
        public void SelectedY_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedY = 10;
            Assert.AreEqual(10, state.SelectedY);
        }

        [TestMethod]
        public void SelectedWeapon_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedWeapon = 3;
            Assert.AreEqual(3, state.SelectedWeapon);
        }

        [TestMethod]
        public void SelectedWeaponName_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedWeaponName = "ビームライフル";
            Assert.AreEqual("ビームライフル", state.SelectedWeaponName);
        }

        [TestMethod]
        public void SelectedTWeapon_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedTWeapon = 2;
            Assert.AreEqual(2, state.SelectedTWeapon);
        }

        [TestMethod]
        public void SelectedTWeaponName_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedTWeaponName = "バズーカ";
            Assert.AreEqual("バズーカ", state.SelectedTWeaponName);
        }

        [TestMethod]
        public void SelectedDefenseOption_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedDefenseOption = "防御";
            Assert.AreEqual("防御", state.SelectedDefenseOption);
        }

        [TestMethod]
        public void SelectedAbility_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedAbility = 1;
            Assert.AreEqual(1, state.SelectedAbility);
        }

        [TestMethod]
        public void SelectedAbilityName_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedAbilityName = "修理装置";
            Assert.AreEqual("修理装置", state.SelectedAbilityName);
        }

        [TestMethod]
        public void SelectedItem_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedItem = 7;
            Assert.AreEqual(7, state.SelectedItem);
        }

        [TestMethod]
        public void SelectedSpecialPower_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedSpecialPower = "熱血";
            Assert.AreEqual("熱血", state.SelectedSpecialPower);
        }

        [TestMethod]
        public void SelectedUnitMoveCost_CanBeSet()
        {
            var state = new SelectedState();
            state.SelectedUnitMoveCost = 4;
            Assert.AreEqual(4, state.SelectedUnitMoveCost);
        }

        // ──────────────────────────────────────────────
        // Clone
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clone_ReturnsNonNullInstance()
        {
            var state = new SelectedState();
            var clone = state.Clone();
            Assert.IsNotNull(clone);
        }

        [TestMethod]
        public void Clone_ReturnsDifferentInstance()
        {
            var state = new SelectedState();
            var clone = state.Clone();
            Assert.AreNotSame(state, clone);
        }

        [TestMethod]
        public void Clone_CopiesSelectedCommand()
        {
            var state = new SelectedState { SelectedCommand = "移動" };
            var clone = state.Clone();
            Assert.AreEqual("移動", clone.SelectedCommand);
        }

        [TestMethod]
        public void Clone_CopiesSelectedX()
        {
            var state = new SelectedState { SelectedX = 3 };
            var clone = state.Clone();
            Assert.AreEqual(3, clone.SelectedX);
        }

        [TestMethod]
        public void Clone_CopiesSelectedY()
        {
            var state = new SelectedState { SelectedY = 8 };
            var clone = state.Clone();
            Assert.AreEqual(8, clone.SelectedY);
        }

        [TestMethod]
        public void Clone_CopiesSelectedWeapon()
        {
            var state = new SelectedState { SelectedWeapon = 2 };
            var clone = state.Clone();
            Assert.AreEqual(2, clone.SelectedWeapon);
        }

        [TestMethod]
        public void Clone_CopiesSelectedAbility()
        {
            var state = new SelectedState { SelectedAbility = 5 };
            var clone = state.Clone();
            Assert.AreEqual(5, clone.SelectedAbility);
        }

        [TestMethod]
        public void Clone_SelectedPartners_IsDifferentList()
        {
            var state = new SelectedState();
            var clone = state.Clone();
            Assert.AreNotSame(state.SelectedPartners, clone.SelectedPartners);
        }

        [TestMethod]
        public void Clone_SelectedPartners_HasSameCount()
        {
            var state = new SelectedState();
            var clone = state.Clone();
            Assert.AreEqual(state.SelectedPartners.Count, clone.SelectedPartners.Count);
        }

        [TestMethod]
        public void Clone_ModifyClone_DoesNotAffectOriginal()
        {
            var state = new SelectedState { SelectedCommand = "攻撃" };
            var clone = state.Clone();
            clone.SelectedCommand = "防御";
            Assert.AreEqual("攻撃", state.SelectedCommand);
        }

        [TestMethod]
        public void Clone_CopiesSelectedDefenseOption()
        {
            var state = new SelectedState { SelectedDefenseOption = "回避" };
            var clone = state.Clone();
            Assert.AreEqual("回避", clone.SelectedDefenseOption);
        }

        [TestMethod]
        public void Clone_CopiesSelectedSpecialPower()
        {
            var state = new SelectedState { SelectedSpecialPower = "気合" };
            var clone = state.Clone();
            Assert.AreEqual("気合", clone.SelectedSpecialPower);
        }

        [TestMethod]
        public void Clone_CopiesSelectedUnitMoveCost()
        {
            var state = new SelectedState { SelectedUnitMoveCost = 6 };
            var clone = state.Clone();
            Assert.AreEqual(6, clone.SelectedUnitMoveCost);
        }

        [TestMethod]
        public void Clone_CopiesSelectedItem()
        {
            var state = new SelectedState { SelectedItem = 9 };
            var clone = state.Clone();
            Assert.AreEqual(9, clone.SelectedItem);
        }

        [TestMethod]
        public void Clone_CopiesSelectedWeaponName()
        {
            var state = new SelectedState { SelectedWeaponName = "マシンキャノン" };
            var clone = state.Clone();
            Assert.AreEqual("マシンキャノン", clone.SelectedWeaponName);
        }

        [TestMethod]
        public void Clone_CopiesSelectedTWeaponName()
        {
            var state = new SelectedState { SelectedTWeaponName = "ハンドガン" };
            var clone = state.Clone();
            Assert.AreEqual("ハンドガン", clone.SelectedTWeaponName);
        }

        [TestMethod]
        public void Clone_CopiesSelectedAbilityName()
        {
            var state = new SelectedState { SelectedAbilityName = "補給装置" };
            var clone = state.Clone();
            Assert.AreEqual("補給装置", clone.SelectedAbilityName);
        }

        [TestMethod]
        public void Clone_CopiesSelectedTWeapon()
        {
            var state = new SelectedState { SelectedTWeapon = 4 };
            var clone = state.Clone();
            Assert.AreEqual(4, clone.SelectedTWeapon);
        }
    }
}
