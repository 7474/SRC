using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// PilotData の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class PilotDataMoreTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // BGM / Personality / ExpValue プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BGM_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { BGM = "battle.mid" };
            Assert.AreEqual("battle.mid", pd.BGM);
        }

        [TestMethod]
        public void Personality_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Personality = "強気" };
            Assert.AreEqual("強気", pd.Personality);
        }

        [TestMethod]
        public void ExpValue_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { ExpValue = 200 };
            Assert.AreEqual(200, pd.ExpValue);
        }

        [TestMethod]
        public void ExpValue_Zero_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { ExpValue = 0 };
            Assert.AreEqual(0, pd.ExpValue);
        }

        // ──────────────────────────────────────────────
        // SpecialPower / SpecialPowerDef / CountSpecialPower
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountSpecialPower_NoSpecialPower_ReturnsZero()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.AreEqual(0, pd.CountSpecialPower(1));
        }

        [TestMethod]
        public void AddSpecialPower_CountSpecialPower_Increases()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSpecialPower("集中", 1, 10);
            Assert.AreEqual(1, pd.CountSpecialPower(1));
        }

        [TestMethod]
        public void SpecialPower_ReturnsName_AtGivenLevelAndIndex()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSpecialPower("熱血", 1, 30);
            Assert.AreEqual("熱血", pd.SpecialPower(1, 1));
        }

        [TestMethod]
        public void SpecialPower_HigherLevel_ReturnsNull_WhenNotAvailable()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSpecialPower("覚醒", 20, 60);
            Assert.IsNull(pd.SpecialPower(1, 1));
        }

        [TestMethod]
        public void SpecialPowerDef_ReturnsCorrectDef()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSpecialPower("加速", 1, 15);
            var def = pd.SpecialPowerDef(1, 1);
            Assert.IsNotNull(def);
            Assert.AreEqual("加速", def.Name);
            Assert.AreEqual(15, def.SPConsumption);
        }

        [TestMethod]
        public void SpecialPowerDef_OutOfRange_ReturnsNull()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSpecialPower("加速", 1, 15);
            Assert.IsNull(pd.SpecialPowerDef(1, 2));
        }

        [TestMethod]
        public void CountSpecialPower_LevelGating_CountsOnlyAvailableAtLevel()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSpecialPower("集中", 1, 10);
            pd.AddSpecialPower("熱血", 30, 40);
            Assert.AreEqual(1, pd.CountSpecialPower(10));
            Assert.AreEqual(2, pd.CountSpecialPower(30));
        }

        // ──────────────────────────────────────────────
        // SkillName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillName_ReturnsSkillName_WhenExists()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("気合", 1d, "", 1);
            var name = pd.SkillName(1, "気合");
            Assert.IsNotNull(name);
            Assert.AreNotEqual("非表示", name);
        }

        [TestMethod]
        public void SkillName_WithLevelNotDefault_ContainsLv()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("格闘強化", 3d, "", 1);
            var name = pd.SkillName(1, "格闘強化");
            Assert.IsTrue(name.Contains("Lv"), $"期待: Lv含む, 実際: {name}");
        }

        // ──────────────────────────────────────────────
        // AddFeature 各形式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddFeature_SimpleType_AddsFeature()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddFeature("ニュータイプ");
            Assert.AreEqual(1, pd.CountFeature());
            Assert.IsTrue(pd.IsFeatureAvailable("ニュータイプ"));
        }

        [TestMethod]
        public void AddFeature_WithLevel_SetsLevel()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddFeature("格闘強化Lv3");
            Assert.AreEqual(3d, pd.FeatureLevel("格闘強化"));
        }

        [TestMethod]
        public void AddFeature_WithData_SetsData()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddFeature("特殊能力=カスタム");
            Assert.AreEqual("カスタム", pd.FeatureData("特殊能力"));
        }

        [TestMethod]
        public void AddFeature_Multiple_AllAdded()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddFeature("ニュータイプ");
            pd.AddFeature("底力");
            pd.AddFeature("援護攻撃");
            Assert.AreEqual(3, pd.CountFeature());
        }

        // ──────────────────────────────────────────────
        // FeatureName / FeatureLevel / FeatureData (int インデックス)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureName_ByIntIndex_ReturnsName()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddFeature("底力");
            var name = pd.FeatureName(1);
            Assert.AreEqual("底力", name);
        }

        [TestMethod]
        public void FeatureName_ByIntIndex_WithLevel_ContainsLv()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddFeature("援護攻撃Lv2");
            var name = pd.FeatureName(1);
            Assert.IsTrue(name.Contains("Lv"), $"期待: Lv含む, 実際: {name}");
        }

        [TestMethod]
        public void FeatureLevel_ByStringIndex_DefaultLevel_ReturnsOne()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddFeature("底力");
            Assert.AreEqual(1d, pd.FeatureLevel("底力"));
        }
    }
}
