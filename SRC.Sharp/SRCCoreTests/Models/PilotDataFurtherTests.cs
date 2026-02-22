using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// PilotData クラスのさらなるユニットテスト
    /// </summary>
    [TestClass]
    public class PilotDataFurtherTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // 基本プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "アムロ" };
            Assert.AreEqual("アムロ", pd.Name);
        }

        [TestMethod]
        public void SP_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { SP = 8 };
            Assert.AreEqual(8, pd.SP);
        }

        [TestMethod]
        public void Infight_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Infight = 200 };
            Assert.AreEqual(200, pd.Infight);
        }

        [TestMethod]
        public void Shooting_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Shooting = 180 };
            Assert.AreEqual(180, pd.Shooting);
        }

        [TestMethod]
        public void Hit_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Hit = 160 };
            Assert.AreEqual(160, pd.Hit);
        }

        [TestMethod]
        public void Dodge_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Dodge = 140 };
            Assert.AreEqual(140, pd.Dodge);
        }

        [TestMethod]
        public void Intuition_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Intuition = 120 };
            Assert.AreEqual(120, pd.Intuition);
        }

        [TestMethod]
        public void Technique_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Technique = 100 };
            Assert.AreEqual(100, pd.Technique);
        }

        // ──────────────────────────────────────────────
        // 追加プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sex_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Sex = "男" };
            Assert.AreEqual("男", pd.Sex);
        }

        [TestMethod]
        public void Nickname_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Nickname = "少年" };
            Assert.AreEqual("少年", pd.Nickname);
        }

        [TestMethod]
        public void BGM_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { BGM = "battle.mp3" };
            Assert.AreEqual("battle.mp3", pd.BGM);
        }

        [TestMethod]
        public void Class_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Class = "戦士" };
            Assert.AreEqual("戦士", pd.Class);
        }

        [TestMethod]
        public void Personality_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Personality = "強気" };
            Assert.AreEqual("強気", pd.Personality);
        }

        // ──────────────────────────────────────────────
        // パラメータが独立していること
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TwoInstances_AreIndependent()
        {
            var src = CreateSRC();
            var pd1 = new PilotData(src) { Name = "アムロ", Infight = 200 };
            var pd2 = new PilotData(src) { Name = "カミーユ", Infight = 220 };
            Assert.AreNotEqual(pd1.Name, pd2.Name);
            Assert.AreNotEqual(pd1.Infight, pd2.Infight);
        }
    }
}
