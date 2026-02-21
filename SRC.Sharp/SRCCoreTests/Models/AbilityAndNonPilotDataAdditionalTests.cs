using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AbilityData クラスの追加テスト
    /// </summary>
    [TestClass]
    public class AbilityDataAdditionalTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // 基本フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var ad = new AbilityData(CreateSrc()) { Name = "ファイア" };
            Assert.AreEqual("ファイア", ad.Name);
        }

        [TestMethod]
        public void Class_CanBeSetAndRead()
        {
            var ad = new AbilityData(CreateSrc()) { Class = "魔法" };
            Assert.AreEqual("魔法", ad.Class);
        }

        [TestMethod]
        public void NecessarySkill_CanBeSetAndRead()
        {
            var ad = new AbilityData(CreateSrc()) { NecessarySkill = "魔力Lv3" };
            Assert.AreEqual("魔力Lv3", ad.NecessarySkill);
        }

        [TestMethod]
        public void NecessaryCondition_CanBeSetAndRead()
        {
            var ad = new AbilityData(CreateSrc()) { NecessaryCondition = "変身" };
            Assert.AreEqual("変身", ad.NecessaryCondition);
        }

        // ──────────────────────────────────────────────
        // 複数インスタンス独立性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TwoAbilities_AreIndependent()
        {
            var src = CreateSrc();
            var ad1 = new AbilityData(src) { Name = "ファイア" };
            var ad2 = new AbilityData(src) { Name = "ブリザド" };
            Assert.AreNotEqual(ad1.Name, ad2.Name);
        }
    }

    /// <summary>
    /// NonPilotData クラスの追加テスト
    /// </summary>
    [TestClass]
    public class NonPilotDataAdditionalTests
    {
        // ──────────────────────────────────────────────
        // 基本フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var np = new NonPilotData { Name = "テストNPC" };
            Assert.AreEqual("テストNPC", np.Name);
        }

        [TestMethod]
        public void Name_JapaneseValue_PreservedCorrectly()
        {
            var np = new NonPilotData { Name = "ロボット一号" };
            Assert.AreEqual("ロボット一号", np.Name);
        }

        [TestMethod]
        public void IsBitmapMissing_DefaultValue_IsFalse()
        {
            var np = new NonPilotData();
            Assert.IsFalse(np.IsBitmapMissing);
        }

        [TestMethod]
        public void IsBitmapMissing_CanBeSetToTrue()
        {
            var np = new NonPilotData { IsBitmapMissing = true };
            Assert.IsTrue(np.IsBitmapMissing);
        }

        // ──────────────────────────────────────────────
        // 複数インスタンス独立性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TwoNonPilots_AreIndependent()
        {
            var np1 = new NonPilotData { Name = "NPC1" };
            var np2 = new NonPilotData { Name = "NPC2" };
            Assert.AreNotEqual(np1.Name, np2.Name);
        }
    }
}
