using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;

namespace SRCCore.Events.Tests
{
    /// <summary>
    /// BCVariable クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class BCVariableTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_SetsDefaultValues()
        {
            var bcv = new BCVariable();
            Assert.IsFalse(bcv.IsConfig);
            Assert.IsNull(bcv.MeUnit);
            Assert.IsNull(bcv.AtkUnit);
            Assert.IsNull(bcv.DefUnit);
            Assert.AreEqual(0, bcv.WeaponNumber);
        }

        [TestMethod]
        public void Constructor_CallsDataReset()
        {
            // コンストラクタは DataReset を呼ぶので戦闘変数はデフォルト値になる
            var bcv = new BCVariable();
            Assert.AreEqual(0, bcv.AttackExp);
            Assert.AreEqual(0, bcv.AttackVariable);
            Assert.AreEqual(0, bcv.DffenceVariable);
            Assert.AreEqual(1d, bcv.TerrainAdaption);
            Assert.AreEqual(1d, bcv.SizeMod);
            Assert.AreEqual(0, bcv.LastVariable);
            Assert.AreEqual(0, bcv.WeaponPower);
            Assert.AreEqual(0, bcv.CommonEnemy);
        }

        // ──────────────────────────────────────────────
        // DataReset
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DataReset_ClearsAttackAndDefenseVariables()
        {
            var bcv = new BCVariable();

            // 値を変更する
            bcv.AttackExp = 500;
            bcv.AttackVariable = 200;
            bcv.DffenceVariable = 150;
            bcv.TerrainAdaption = 1.5;
            bcv.SizeMod = 0.8;
            bcv.LastVariable = 999;
            bcv.WeaponPower = 300;
            bcv.CommonEnemy = 10;

            // DataReset でリセット
            bcv.DataReset();

            Assert.AreEqual(0, bcv.AttackExp);
            Assert.AreEqual(0, bcv.AttackVariable);
            Assert.AreEqual(0, bcv.DffenceVariable);
            Assert.AreEqual(1d, bcv.TerrainAdaption);
            Assert.AreEqual(1d, bcv.SizeMod);
            Assert.AreEqual(0, bcv.LastVariable);
            Assert.AreEqual(0, bcv.WeaponPower);
            Assert.AreEqual(0, bcv.CommonEnemy);
        }

        [TestMethod]
        public void DataReset_DoesNotClearIsConfig()
        {
            var bcv = new BCVariable();
            bcv.IsConfig = true;
            bcv.DataReset();
            // DataReset は IsConfig を変更しない
            Assert.IsTrue(bcv.IsConfig);
        }

        [TestMethod]
        public void DataReset_DoesNotClearUnitReferences()
        {
            var bcv = new BCVariable();
            bcv.WeaponNumber = 3;
            bcv.DataReset();
            // DataReset は WeaponNumber を変更しない
            Assert.AreEqual(3, bcv.WeaponNumber);
        }

        // ──────────────────────────────────────────────
        // IsConfig フラグ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsConfig_CanBeToggledOn()
        {
            var bcv = new BCVariable();
            Assert.IsFalse(bcv.IsConfig);
            bcv.IsConfig = true;
            Assert.IsTrue(bcv.IsConfig);
        }

        [TestMethod]
        public void IsConfig_CanBeToggledOff()
        {
            var bcv = new BCVariable();
            bcv.IsConfig = true;
            bcv.IsConfig = false;
            Assert.IsFalse(bcv.IsConfig);
        }

        // ──────────────────────────────────────────────
        // フィールドの設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fields_CanBeSetAndRead()
        {
            var bcv = new BCVariable();
            bcv.WeaponNumber = 2;
            bcv.AttackExp = 100;
            bcv.AttackVariable = 50;
            bcv.DffenceVariable = 40;
            bcv.TerrainAdaption = 1.2;
            bcv.SizeMod = 0.9;
            bcv.LastVariable = 80;
            bcv.WeaponPower = 200;
            bcv.Armor = 500;
            bcv.CommonEnemy = 5;

            Assert.AreEqual(2, bcv.WeaponNumber);
            Assert.AreEqual(100, bcv.AttackExp);
            Assert.AreEqual(50, bcv.AttackVariable);
            Assert.AreEqual(40, bcv.DffenceVariable);
            Assert.AreEqual(1.2, bcv.TerrainAdaption);
            Assert.AreEqual(0.9, bcv.SizeMod);
            Assert.AreEqual(80, bcv.LastVariable);
            Assert.AreEqual(200, bcv.WeaponPower);
            Assert.AreEqual(500, bcv.Armor);
            Assert.AreEqual(5, bcv.CommonEnemy);
        }
    }
}
