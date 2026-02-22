using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    [TestClass]
    public class BCVariableMoreTests3
    {
        [TestMethod]
        public void NewInstance_AttackExp_IsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.AttackExp);
        }

        [TestMethod]
        public void NewInstance_AttackVariable_IsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.AttackVariable);
        }

        [TestMethod]
        public void NewInstance_DffenceVariable_IsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.DffenceVariable);
        }

        [TestMethod]
        public void NewInstance_TerrainAdaption_IsOne()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(1d, bcv.TerrainAdaption);
        }

        [TestMethod]
        public void NewInstance_SizeMod_IsOne()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(1d, bcv.SizeMod);
        }

        [TestMethod]
        public void NewInstance_LastVariable_IsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.LastVariable);
        }

        [TestMethod]
        public void NewInstance_WeaponPower_IsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.WeaponPower);
        }

        [TestMethod]
        public void NewInstance_Armor_IsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.Armor);
        }

        [TestMethod]
        public void NewInstance_CommonEnemy_IsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.CommonEnemy);
        }

        [TestMethod]
        public void SetAttackExp_ThenDataReset_ReturnsZero()
        {
            var bcv = new BCVariable();
            bcv.AttackExp = 1000;
            bcv.DataReset();
            Assert.AreEqual(0, bcv.AttackExp);
        }

        [TestMethod]
        public void SetTerrainAdaption_ThenDataReset_ReturnsOne()
        {
            var bcv = new BCVariable();
            bcv.TerrainAdaption = 2.5d;
            bcv.DataReset();
            Assert.AreEqual(1d, bcv.TerrainAdaption);
        }

        [TestMethod]
        public void SetSizeMod_ThenDataReset_ReturnsOne()
        {
            var bcv = new BCVariable();
            bcv.SizeMod = 0.5d;
            bcv.DataReset();
            Assert.AreEqual(1d, bcv.SizeMod);
        }

        [TestMethod]
        public void SetWeaponPower_ThenDataReset_ReturnsZero()
        {
            var bcv = new BCVariable();
            bcv.WeaponPower = 9999;
            bcv.DataReset();
            Assert.AreEqual(0, bcv.WeaponPower);
        }

        [TestMethod]
        public void SetCommonEnemy_ThenDataReset_ReturnsZero()
        {
            var bcv = new BCVariable();
            bcv.CommonEnemy = 99;
            bcv.DataReset();
            Assert.AreEqual(0, bcv.CommonEnemy);
        }

        [TestMethod]
        public void IsConfig_InitiallyFalse()
        {
            var bcv = new BCVariable();
            Assert.IsFalse(bcv.IsConfig);
        }

        [TestMethod]
        public void MeUnit_InitiallyNull()
        {
            var bcv = new BCVariable();
            Assert.IsNull(bcv.MeUnit);
        }

        [TestMethod]
        public void AtkUnit_InitiallyNull()
        {
            var bcv = new BCVariable();
            Assert.IsNull(bcv.AtkUnit);
        }

        [TestMethod]
        public void DefUnit_InitiallyNull()
        {
            var bcv = new BCVariable();
            Assert.IsNull(bcv.DefUnit);
        }

        [TestMethod]
        public void WeaponNumber_InitiallyZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.WeaponNumber);
        }

        [TestMethod]
        public void Armor_SetAndGet_ReturnsCorrectValue()
        {
            var bcv = new BCVariable();
            bcv.Armor = 1200;
            Assert.AreEqual(1200, bcv.Armor);
        }

        [TestMethod]
        public void DataReset_CalledTwice_StillReturnsDefaults()
        {
            var bcv = new BCVariable();
            bcv.AttackExp = 500;
            bcv.DataReset();
            bcv.AttackExp = 300;
            bcv.DataReset();
            Assert.AreEqual(0, bcv.AttackExp);
        }
    }
}
