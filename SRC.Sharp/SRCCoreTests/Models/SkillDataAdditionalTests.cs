using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// SkillData クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class SkillDataExtraTests
    {
        // ──────────────────────────────────────────────
        // フィールドの設定・読み取り
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
            var sd = new SkillData { Level = 5.0 };
            Assert.AreEqual(5.0, sd.Level);
        }

        [TestMethod]
        public void StrData_CanBeSetAndRead()
        {
            var sd = new SkillData { StrData = "テストデータ" };
            Assert.AreEqual("テストデータ", sd.StrData);
        }

        [TestMethod]
        public void NecessaryLevel_CanBeSetAndRead()
        {
            var sd = new SkillData { NecessaryLevel = 10 };
            Assert.AreEqual(10, sd.NecessaryLevel);
        }

        [TestMethod]
        public void DefaultValues_AreExpected()
        {
            var sd = new SkillData();
            Assert.IsNull(sd.Name);
            Assert.AreEqual(0.0, sd.Level);
            Assert.IsNull(sd.StrData);
            Assert.AreEqual(0, sd.NecessaryLevel);
        }

        // ──────────────────────────────────────────────
        // HasLevel 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasLevel_ReturnsFalse_WhenLevelIsExactlyDefaultLevel()
        {
            var sd = new SkillData { Level = Constants.DEFAULT_LEVEL };
            Assert.IsFalse(sd.HasLevel);
        }

        [TestMethod]
        public void HasLevel_ReturnsTrue_WhenLevelIsDefaultLevelMinusOne()
        {
            var sd = new SkillData { Level = Constants.DEFAULT_LEVEL - 1 };
            Assert.IsTrue(sd.HasLevel);
        }

        [TestMethod]
        public void HasLevel_ReturnsTrue_WhenLevelIsDefaultLevelPlusOne()
        {
            var sd = new SkillData { Level = Constants.DEFAULT_LEVEL + 1 };
            Assert.IsTrue(sd.HasLevel);
        }

        [TestMethod]
        public void HasLevel_ReturnsTrue_WhenLevelIsLargePositive()
        {
            var sd = new SkillData { Level = 99 };
            Assert.IsTrue(sd.HasLevel);
        }

        // ──────────────────────────────────────────────
        // LevelOrDefault 追加ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LevelOrDefault_WhenLevelIsZero_ReturnsZero()
        {
            // Level=0 は DEFAULT_LEVEL でないため HasLevel=true → Level(=0) を返す
            var sd = new SkillData { Level = 0 };
            Assert.AreEqual(0d, sd.LevelOrDefault(1d));
        }

        [TestMethod]
        public void LevelOrDefault_WhenLevelIsNegative_ReturnsNegative()
        {
            var sd = new SkillData { Level = -5 };
            Assert.AreEqual(-5d, sd.LevelOrDefault(1d));
        }

        [TestMethod]
        public void LevelOrDefault_DefaultArg_Zero_UsedWhenNoLevel()
        {
            var sd = new SkillData { Level = Constants.DEFAULT_LEVEL };
            Assert.AreEqual(0d, sd.LevelOrDefault(0d));
        }

        [TestMethod]
        public void LevelOrDefault_DefaultArg_Negative_UsedWhenNoLevel()
        {
            var sd = new SkillData { Level = Constants.DEFAULT_LEVEL };
            Assert.AreEqual(-1d, sd.LevelOrDefault(-1d));
        }
    }
}
