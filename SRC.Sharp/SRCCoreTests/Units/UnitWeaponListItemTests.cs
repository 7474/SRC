using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// UnitWeaponListItem のユニットテスト
    /// </summary>
    [TestClass]
    public class UnitWeaponListItemTests
    {
        [TestMethod]
        public void DefaultConstructor_WeaponIsNull()
        {
            var item = new UnitWeaponListItem();
            Assert.IsNull(item.Weapon);
        }

        [TestMethod]
        public void DefaultConstructor_CanUseIsFalse()
        {
            var item = new UnitWeaponListItem();
            Assert.IsFalse(item.CanUse);
        }

        [TestMethod]
        public void CanSetAndGetWeapon()
        {
            var item = new UnitWeaponListItem();
            // UnitWeapon のコンストラクタは依存が多いため null 設定・取得のみ確認
            item.Weapon = null;
            Assert.IsNull(item.Weapon);
        }

        [TestMethod]
        public void CanSetCanUseToTrue()
        {
            var item = new UnitWeaponListItem();
            item.CanUse = true;
            Assert.IsTrue(item.CanUse);
        }

        [TestMethod]
        public void CanToggleCanUse()
        {
            var item = new UnitWeaponListItem();
            item.CanUse = true;
            Assert.IsTrue(item.CanUse);
            item.CanUse = false;
            Assert.IsFalse(item.CanUse);
        }
    }
}
