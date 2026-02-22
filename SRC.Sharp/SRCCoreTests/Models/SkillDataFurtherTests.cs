using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// SkillData クラスのさらなる追加ユニットテスト
    /// </summary>
    [TestClass]
    public class SkillDataFurtherTests
    {
        // ──────────────────────────────────────────────
        // SkillData のプロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var sd = new SkillData { Name = "格闘" };
            Assert.AreEqual("格闘", sd.Name);
        }

        [TestMethod]
        public void Level_CanBeSetAndRead()
        {
            var sd = new SkillData { Level = 5 };
            Assert.AreEqual(5, sd.Level);
        }

        [TestMethod]
        public void StrData_CanBeSetAndRead()
        {
            var sd = new SkillData { StrData = "技量+10" };
            Assert.AreEqual("技量+10", sd.StrData);
        }

        [TestMethod]
        public void DefaultValues_AreCorrect()
        {
            var sd = new SkillData();
            Assert.IsNull(sd.Name);
            Assert.AreEqual(0, sd.NecessaryLevel);
            Assert.IsNull(sd.StrData);
        }

        [TestMethod]
        public void Level_Zero_IsAllowed()
        {
            var sd = new SkillData { Level = 0 };
            Assert.AreEqual(0, sd.Level);
        }

        [TestMethod]
        public void Level_Negative_IsAllowed()
        {
            var sd = new SkillData { Level = -1 };
            Assert.AreEqual(-1, sd.Level);
        }

        [TestMethod]
        public void Level_LargeValue_IsAllowed()
        {
            var sd = new SkillData { Level = 999 };
            Assert.AreEqual(999, sd.Level);
        }

        [TestMethod]
        public void Name_EmptyString_IsAllowed()
        {
            var sd = new SkillData { Name = "" };
            Assert.AreEqual("", sd.Name);
        }

        [TestMethod]
        public void StrData_EmptyString_IsAllowed()
        {
            var sd = new SkillData { StrData = "" };
            Assert.AreEqual("", sd.StrData);
        }

        [TestMethod]
        public void AllProperties_SetAtOnce()
        {
            var sd = new SkillData { Name = "回避", Level = 3, StrData = "回避+15" };
            Assert.AreEqual("回避", sd.Name);
            Assert.AreEqual(3, sd.Level);
            Assert.AreEqual("回避+15", sd.StrData);
        }
    }
}
