using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class SkillDataMoreTests3
    {
        // ──────────────────────────────────────────────
        // プロパティの基本テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SkillData_DefaultName_IsNullOrEmpty()
        {
            var skill = new SkillData();
            Assert.IsNull(skill.Name);
        }

        [TestMethod]
        public void SkillData_SetName_ReturnsCorrectName()
        {
            var skill = new SkillData { Name = "テストスキル" };
            Assert.AreEqual("テストスキル", skill.Name);
        }

        [TestMethod]
        public void SkillData_SetLevel_ReturnsCorrectLevel()
        {
            var skill = new SkillData { Level = 5.0 };
            Assert.AreEqual(5.0, skill.Level);
        }

        [TestMethod]
        public void SkillData_SetStrData_ReturnsCorrectData()
        {
            var skill = new SkillData { StrData = "SomeData" };
            Assert.AreEqual("SomeData", skill.StrData);
        }

        [TestMethod]
        public void SkillData_SetNecessaryLevel_ReturnsCorrectLevel()
        {
            var skill = new SkillData { NecessaryLevel = 10 };
            Assert.AreEqual(10, skill.NecessaryLevel);
        }

        // ──────────────────────────────────────────────
        // HasLevel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasLevel_DefaultLevel_ReturnsFalse()
        {
            var skill = new SkillData { Level = Constants.DEFAULT_LEVEL };
            Assert.IsFalse(skill.HasLevel);
        }

        [TestMethod]
        public void HasLevel_NonDefaultLevel_ReturnsTrue()
        {
            var skill = new SkillData { Level = 3.0 };
            Assert.IsTrue(skill.HasLevel);
        }

        [TestMethod]
        public void HasLevel_ZeroLevel_ReturnsTrue()
        {
            var skill = new SkillData { Level = 0.0 };
            Assert.IsTrue(skill.HasLevel);
        }

        // ──────────────────────────────────────────────
        // LevelOrDefault
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LevelOrDefault_WhenHasLevel_ReturnsOwnLevel()
        {
            var skill = new SkillData { Level = 7.0 };
            Assert.AreEqual(7.0, skill.LevelOrDefault(1.0));
        }

        [TestMethod]
        public void LevelOrDefault_WhenDefaultLevel_ReturnsProvidedDefault()
        {
            var skill = new SkillData { Level = Constants.DEFAULT_LEVEL };
            Assert.AreEqual(99.0, skill.LevelOrDefault(99.0));
        }

        [TestMethod]
        public void LevelOrDefault_WhenDefaultLevel_ReturnsZeroDefault()
        {
            var skill = new SkillData { Level = Constants.DEFAULT_LEVEL };
            Assert.AreEqual(0.0, skill.LevelOrDefault(0.0));
        }
    }
}
