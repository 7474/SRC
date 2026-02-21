using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// BCVariable クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class BCVariableMoreTests
    {
        // ──────────────────────────────────────────────
        // Armor フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Armor_DefaultIsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.Armor);
        }

        [TestMethod]
        public void Armor_CanBeSetAndRead()
        {
            var bcv = new BCVariable();
            bcv.Armor = 500;
            Assert.AreEqual(500, bcv.Armor);
        }

        [TestMethod]
        public void Armor_CanBeSetToNegative()
        {
            var bcv = new BCVariable();
            bcv.Armor = -10;
            Assert.AreEqual(-10, bcv.Armor);
        }

        [TestMethod]
        public void DataReset_DoesNotResetArmor()
        {
            // Armor は DataReset でリセットされない (BCVariable.DataReset の仕様通り)
            var bcv = new BCVariable();
            bcv.Armor = 300;
            bcv.DataReset();
            // Armor は DataReset の対象外なので変わらない
            Assert.AreEqual(300, bcv.Armor);
        }

        // ──────────────────────────────────────────────
        // TerrainAdaption と SizeMod の詳細テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TerrainAdaption_DefaultIsOne()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(1d, bcv.TerrainAdaption);
        }

        [TestMethod]
        public void TerrainAdaption_CanBeSetAboveOne()
        {
            var bcv = new BCVariable();
            bcv.TerrainAdaption = 1.5;
            Assert.AreEqual(1.5, bcv.TerrainAdaption);
        }

        [TestMethod]
        public void TerrainAdaption_CanBeSetBelowOne()
        {
            var bcv = new BCVariable();
            bcv.TerrainAdaption = 0.75;
            Assert.AreEqual(0.75, bcv.TerrainAdaption);
        }

        [TestMethod]
        public void SizeMod_DefaultIsOne()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(1d, bcv.SizeMod);
        }

        [TestMethod]
        public void SizeMod_CanBeSetToTwo()
        {
            var bcv = new BCVariable();
            bcv.SizeMod = 2.0;
            Assert.AreEqual(2.0, bcv.SizeMod);
        }

        [TestMethod]
        public void SizeMod_CanBeSetToZero()
        {
            var bcv = new BCVariable();
            bcv.SizeMod = 0d;
            Assert.AreEqual(0d, bcv.SizeMod);
        }

        // ──────────────────────────────────────────────
        // WeaponPower と CommonEnemy の詳細テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponPower_DefaultIsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.WeaponPower);
        }

        [TestMethod]
        public void WeaponPower_CanBeSetToLargeValue()
        {
            var bcv = new BCVariable();
            bcv.WeaponPower = 99999;
            Assert.AreEqual(99999, bcv.WeaponPower);
        }

        [TestMethod]
        public void CommonEnemy_DefaultIsZero()
        {
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.CommonEnemy);
        }

        [TestMethod]
        public void CommonEnemy_CanBeSetToPositive()
        {
            var bcv = new BCVariable();
            bcv.CommonEnemy = 5;
            Assert.AreEqual(5, bcv.CommonEnemy);
        }

        // ──────────────────────────────────────────────
        // AttackExp と LastVariable の詳細テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AttackExp_CanBeSetToLargeValue()
        {
            var bcv = new BCVariable();
            bcv.AttackExp = 10000;
            Assert.AreEqual(10000, bcv.AttackExp);
        }

        [TestMethod]
        public void LastVariable_AfterDataReset_IsZero()
        {
            var bcv = new BCVariable();
            bcv.LastVariable = 500;
            bcv.DataReset();
            Assert.AreEqual(0, bcv.LastVariable);
        }

        [TestMethod]
        public void AttackVariable_AfterDataReset_IsZero()
        {
            var bcv = new BCVariable();
            bcv.AttackVariable = 200;
            bcv.DataReset();
            Assert.AreEqual(0, bcv.AttackVariable);
        }

        [TestMethod]
        public void DffenceVariable_AfterDataReset_IsZero()
        {
            var bcv = new BCVariable();
            bcv.DffenceVariable = 300;
            bcv.DataReset();
            Assert.AreEqual(0, bcv.DffenceVariable);
        }

        // ──────────────────────────────────────────────
        // 複数フィールドの同時テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllFields_CanBeSetAndRead_Independently()
        {
            var bcv = new BCVariable
            {
                AttackExp = 100,
                AttackVariable = 200,
                DffenceVariable = 300,
                TerrainAdaption = 1.2,
                SizeMod = 1.5,
                LastVariable = 400,
                WeaponPower = 500,
                Armor = 600,
                CommonEnemy = 7
            };

            Assert.AreEqual(100, bcv.AttackExp);
            Assert.AreEqual(200, bcv.AttackVariable);
            Assert.AreEqual(300, bcv.DffenceVariable);
            Assert.AreEqual(1.2, bcv.TerrainAdaption);
            Assert.AreEqual(1.5, bcv.SizeMod);
            Assert.AreEqual(400, bcv.LastVariable);
            Assert.AreEqual(500, bcv.WeaponPower);
            Assert.AreEqual(600, bcv.Armor);
            Assert.AreEqual(7, bcv.CommonEnemy);
        }
    }
}
