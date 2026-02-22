using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// UnitAbilityListItem のユニットテスト
    /// </summary>
    [TestClass]
    public class UnitAbilityListItemTests
    {
        [TestMethod]
        public void DefaultConstructor_AbilityIsNull()
        {
            var item = new UnitAbilityListItem();
            Assert.IsNull(item.Ability);
        }

        [TestMethod]
        public void DefaultConstructor_CanUseIsFalse()
        {
            var item = new UnitAbilityListItem();
            Assert.IsFalse(item.CanUse);
        }

        [TestMethod]
        public void CanSetAndGetAbility()
        {
            var item = new UnitAbilityListItem();
            // UnitAbility のコンストラクタは依存が多いため null 設定・取得のみ確認
            item.Ability = null;
            Assert.IsNull(item.Ability);
        }

        [TestMethod]
        public void CanSetCanUseToTrue()
        {
            var item = new UnitAbilityListItem();
            item.CanUse = true;
            Assert.IsTrue(item.CanUse);
        }

        [TestMethod]
        public void CanToggleCanUse()
        {
            var item = new UnitAbilityListItem();
            item.CanUse = true;
            Assert.IsTrue(item.CanUse);
            item.CanUse = false;
            Assert.IsFalse(item.CanUse);
        }
    }
}
