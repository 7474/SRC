using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// SpecialPowerData / NonPilotData クラスの追加テスト
    /// </summary>
    [TestClass]
    public class SpecialPowerAndNonPilotAdditionalTests
    {
        // ──────────────────────────────────────────────
        // SpecialPowerData 基本フィールドテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowerData_Name_CanBeSetAndRead()
        {
            var src = new SRC { GUI = new MockGUI() };
            var sp = new SpecialPowerData(src) { Name = "精神コマンド" };
            Assert.AreEqual("精神コマンド", sp.Name);
        }

        [TestMethod]
        public void SpecialPowerData_SPConsumption_CanBeSetAndRead()
        {
            var src = new SRC { GUI = new MockGUI() };
            var sp = new SpecialPowerData(src) { SPConsumption = 20 };
            Assert.AreEqual(20, sp.SPConsumption);
        }

        [TestMethod]
        public void SpecialPowerData_DefaultValues_AreExpected()
        {
            var src = new SRC { GUI = new MockGUI() };
            var sp = new SpecialPowerData(src);
            Assert.IsNull(sp.Name);
            Assert.AreEqual(0, sp.SPConsumption);
        }

        // ──────────────────────────────────────────────
        // AbilityData 基本フィールドテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityData_Name_CanBeSetAndRead()
        {
            var src = new SRC { GUI = new MockGUI() };
            var ab = new AbilityData(src) { Name = "援護攻撃" };
            Assert.AreEqual("援護攻撃", ab.Name);
        }

        [TestMethod]
        public void AbilityData_DefaultValues_AreExpected()
        {
            var src = new SRC { GUI = new MockGUI() };
            var ab = new AbilityData(src);
            Assert.IsNull(ab.Name);
        }

        // ──────────────────────────────────────────────
        // SkillData テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillData_Name_CanBeSetAndRead()
        {
            var sk = new SkillData { Name = "格闘" };
            Assert.AreEqual("格闘", sk.Name);
        }

        [TestMethod]
        public void SkillData_Level_CanBeSetAndRead()
        {
            var sk = new SkillData { Level = 9d };
            Assert.AreEqual(9d, sk.Level);
        }

        [TestMethod]
        public void SkillData_NecessaryLevel_CanBeSetAndRead()
        {
            var sk = new SkillData { NecessaryLevel = 3 };
            Assert.AreEqual(3, sk.NecessaryLevel);
        }

        [TestMethod]
        public void SkillData_DefaultValues_AreExpected()
        {
            var sk = new SkillData();
            Assert.IsNull(sk.Name);
            Assert.AreEqual(0d, sk.Level);
            Assert.AreEqual(0, sk.NecessaryLevel);
        }

        // ──────────────────────────────────────────────
        // SpecialPowerEffect / AbilityEffect 詳細テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowerEffect_NegativeLevel_CanBeSet()
        {
            var eff = new SpecialPowerEffect { dblEffectLevel = -1.0 };
            Assert.AreEqual(-1.0, eff.dblEffectLevel);
        }

        [TestMethod]
        public void SpecialPowerEffect_LargeLevel_CanBeSet()
        {
            var eff = new SpecialPowerEffect { dblEffectLevel = 999.9 };
            Assert.AreEqual(999.9, eff.dblEffectLevel);
        }

        [TestMethod]
        public void AbilityEffect_NegativeLevel_CanBeSet()
        {
            var ae = new AbilityEffect { Level = -5.0 };
            Assert.AreEqual(-5.0, ae.Level);
        }

        [TestMethod]
        public void AbilityEffect_ZeroLevel_CanBeSet()
        {
            var ae = new AbilityEffect { Level = 0d };
            Assert.AreEqual(0d, ae.Level);
        }

        [TestMethod]
        public void AbilityEffect_EffectType_ReturnsSameAsName_ForJapanese()
        {
            var ae = new AbilityEffect { Name = "射撃強化" };
            Assert.AreEqual("射撃強化", ae.EffectType);
        }
    }
}
