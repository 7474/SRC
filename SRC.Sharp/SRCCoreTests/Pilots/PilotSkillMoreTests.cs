using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Pilots.Tests
{
    [TestClass]
    public class PilotSkillMoreTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private Pilot CreatePilot(SRC src, string name = "テストパイロット", int level = 1)
        {
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            return src.PList.Add(name, level, "味方");
        }

        // ===== IsSkillAvailable =====

        [TestMethod]
        public void IsSkillAvailable_SkillNotAdded_ReturnsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src);
            Assert.IsFalse(pilot.IsSkillAvailable("気合"));
        }

        [TestMethod]
        public void IsSkillAvailable_SkillAdded_ReturnsTrue()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("スキルパイロット");
            pd.SP = 10;
            pd.AddSkill("気合", 1d, "", 1);
            var pilot = src.PList.Add("スキルパイロット", 1, "味方");
            Assert.IsTrue(pilot.IsSkillAvailable("気合"));
        }

        [TestMethod]
        public void IsSkillAvailable_DifferentSkillAdded_ReturnsFalseForAbsent()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("スキルパイロット2");
            pd.SP = 10;
            pd.AddSkill("闘争心", 1d, "", 1);
            var pilot = src.PList.Add("スキルパイロット2", 1, "味方");
            Assert.IsFalse(pilot.IsSkillAvailable("気合"));
        }

        // ===== HasSupportSkill =====

        [TestMethod]
        public void HasSupportSkill_NoSkills_ReturnsFalse()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "サポートなしパイロット");
            Assert.IsFalse(pilot.HasSupportSkill());
        }

        [TestMethod]
        public void HasSupportSkill_WithEngoSkill_ReturnsTrue()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("援護パイロット");
            pd.SP = 10;
            pd.AddSkill("援護", 1d, "", 1);
            var pilot = src.PList.Add("援護パイロット", 1, "味方");
            Assert.IsTrue(pilot.HasSupportSkill());
        }

        [TestMethod]
        public void HasSupportSkill_WithEngoAttackSkill_ReturnsTrue()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("援護攻撃パイロット");
            pd.SP = 10;
            pd.AddSkill("援護攻撃", 1d, "", 1);
            var pilot = src.PList.Add("援護攻撃パイロット", 1, "味方");
            Assert.IsTrue(pilot.HasSupportSkill());
        }

        [TestMethod]
        public void HasSupportSkill_WithEngoDefenseSkill_ReturnsTrue()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("援護防御パイロット");
            pd.SP = 10;
            pd.AddSkill("援護防御", 1d, "", 1);
            var pilot = src.PList.Add("援護防御パイロット", 1, "味方");
            Assert.IsTrue(pilot.HasSupportSkill());
        }

        [TestMethod]
        public void HasSupportSkill_WithTousotuSkill_ReturnsTrue()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("統率パイロット");
            pd.SP = 10;
            pd.AddSkill("統率", 1d, "", 1);
            var pilot = src.PList.Add("統率パイロット", 1, "味方");
            Assert.IsTrue(pilot.HasSupportSkill());
        }

        [TestMethod]
        public void HasSupportSkill_WithShikiSkill_ReturnsTrue()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("指揮パイロット");
            pd.SP = 10;
            pd.AddSkill("指揮", 1d, "", 1);
            var pilot = src.PList.Add("指揮パイロット", 1, "味方");
            Assert.IsTrue(pilot.HasSupportSkill());
        }

        // ===== CountSkill =====

        [TestMethod]
        public void CountSkill_NoSkills_ReturnsZero()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "カウントゼロパイロット");
            Assert.AreEqual(0, pilot.CountSkill());
        }

        [TestMethod]
        public void CountSkill_AfterAddingTwoSkills_ReturnsTwo()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("ツースキルパイロット");
            pd.SP = 10;
            pd.AddSkill("気合", 1d, "", 1);
            pd.AddSkill("闘争心", 1d, "", 1);
            var pilot = src.PList.Add("ツースキルパイロット", 1, "味方");
            Assert.AreEqual(2, pilot.CountSkill());
        }

        // ===== Skill() by name =====

        [TestMethod]
        public void Skill_ByName_ReturnsSkillName_WhenExists()
        {
            var src = CreateSrc();
            var pd = src.PDList.Add("スキル名前パイロット");
            pd.SP = 10;
            pd.AddSkill("気合", 1d, "", 1);
            var pilot = src.PList.Add("スキル名前パイロット", 1, "味方");
            var skillName = pilot.Skill("気合");
            Assert.AreEqual("気合", skillName);
        }

        [TestMethod]
        public void Skill_ByName_ReturnsEmpty_WhenNotExists()
        {
            var src = CreateSrc();
            var pilot = CreatePilot(src, "スキル名前なしパイロット");
            var skillName = pilot.Skill("存在しないスキル");
            Assert.AreEqual("", skillName);
        }
    }
}
