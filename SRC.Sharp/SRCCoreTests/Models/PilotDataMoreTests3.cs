using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class PilotDataMoreTests3
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ===== Basic field assignments =====

        [TestMethod]
        public void PilotData_Name_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "アムロ" };
            Assert.AreEqual("アムロ", pd.Name);
        }

        [TestMethod]
        public void PilotData_SP_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { SP = 25 };
            Assert.AreEqual(25, pd.SP);
        }

        [TestMethod]
        public void PilotData_Infight_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Infight = 150 };
            Assert.AreEqual(150, pd.Infight);
        }

        [TestMethod]
        public void PilotData_Shooting_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Shooting = 140 };
            Assert.AreEqual(140, pd.Shooting);
        }

        [TestMethod]
        public void PilotData_Hit_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Hit = 120 };
            Assert.AreEqual(120, pd.Hit);
        }

        [TestMethod]
        public void PilotData_Dodge_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Dodge = 130 };
            Assert.AreEqual(130, pd.Dodge);
        }

        [TestMethod]
        public void PilotData_Technique_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Technique = 110 };
            Assert.AreEqual(110, pd.Technique);
        }

        [TestMethod]
        public void PilotData_Intuition_CanBeSetAndRead()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Intuition = 100 };
            Assert.AreEqual(100, pd.Intuition);
        }

        // ===== AddSkill and CountSkill =====

        [TestMethod]
        public void PilotData_CountSkill_NoSkills_ReturnsZero()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.AreEqual(0, pd.Skills.Count);
        }

        [TestMethod]
        public void PilotData_AddSkill_CountSkill_Increases()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("気合", 1d, "", 1);
            Assert.AreEqual(1, pd.Skills.Count);
        }

        [TestMethod]
        public void PilotData_AddSkill_TwoSkills_CountIsTwo()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("気合", 1d, "", 1);
            pd.AddSkill("闘争心", 1d, "", 1);
            Assert.AreEqual(2, pd.Skills.Count);
        }

        // ===== IsSkillAvailable =====

        [TestMethod]
        public void PilotData_IsSkillAvailable_ReturnsFalse_WhenNotAdded()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.IsFalse(pd.IsSkillAvailable(1, "気合"));
        }

        [TestMethod]
        public void PilotData_IsSkillAvailable_ReturnsTrue_WhenAdded()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("気合", 1d, "", 1);
            Assert.IsTrue(pd.IsSkillAvailable(1, "気合"));
        }

        [TestMethod]
        public void PilotData_IsSkillAvailable_ReturnsFalse_ForOtherSkill()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddSkill("気合", 1d, "", 1);
            Assert.IsFalse(pd.IsSkillAvailable(1, "闘争心"));
        }
    }
}
