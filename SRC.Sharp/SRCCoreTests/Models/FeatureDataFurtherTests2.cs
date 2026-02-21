using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// FeatureData クラスのさらなるユニットテスト
    /// </summary>
    [TestClass]
    public class FeatureDataFurtherTests2
    {
        // ──────────────────────────────────────────────
        // プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var fd = new FeatureData { Name = "バリア" };
            Assert.AreEqual("バリア", fd.Name);
        }

        [TestMethod]
        public void Level_CanBeSetAndRead()
        {
            var fd = new FeatureData { Level = 3.0 };
            Assert.AreEqual(3.0, fd.Level, 1e-10);
        }

        [TestMethod]
        public void StrData_CanBeSetAndRead()
        {
            var fd = new FeatureData { StrData = "テストデータ" };
            Assert.AreEqual("テストデータ", fd.StrData);
        }

        [TestMethod]
        public void DefaultLevel_IsZero()
        {
            var fd = new FeatureData();
            Assert.AreEqual(0d, fd.Level, 1e-10);
        }

        // ──────────────────────────────────────────────
        // HasLevel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasLevel_DefaultLevel_ReturnsFalse()
        {
            var fd = new FeatureData { Level = Constants.DEFAULT_LEVEL };
            Assert.IsFalse(fd.HasLevel);
        }

        [TestMethod]
        public void HasLevel_PositiveLevel_ReturnsTrue()
        {
            var fd = new FeatureData { Level = 5.0 };
            Assert.IsTrue(fd.HasLevel);
        }

        [TestMethod]
        public void HasLevel_ZeroLevel_ReturnsTrue()
        {
            var fd = new FeatureData { Level = 0.0 };
            Assert.IsTrue(fd.HasLevel);
        }

        // ──────────────────────────────────────────────
        // FeatureNameWithLv
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureNameWithLv_WithLevel_IncludesLvInName()
        {
            var fd = new FeatureData { Name = "シールド", Level = 3.0, StrData = "" };
            var result = fd.FeatureNameWithLv();
            Assert.IsTrue(result.Contains("シールド"), $"Contains シールド: {result}");
            Assert.IsTrue(result.Contains("Lv") || result.Contains("3"), $"Contains level: {result}");
        }

        [TestMethod]
        public void FeatureNameWithLv_DefaultLevel_ReturnsNameOnly()
        {
            var fd = new FeatureData { Name = "HP回復", Level = Constants.DEFAULT_LEVEL, StrData = "" };
            Assert.AreEqual("HP回復", fd.FeatureNameWithLv());
        }

        [TestMethod]
        public void FeatureNameWithLv_StrDataSet_ReturnsStrData()
        {
            var fd = new FeatureData { Name = "特殊", Level = 1.0, StrData = "カスタム名" };
            Assert.AreEqual("カスタム名", fd.FeatureNameWithLv());
        }

        // ──────────────────────────────────────────────
        // Data プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Data_EmptyStrData_ReturnsEmpty()
        {
            var fd = new FeatureData { StrData = "" };
            Assert.AreEqual("", fd.Data);
        }

        [TestMethod]
        public void Data_SingleWord_ReturnsThatWord()
        {
            var fd = new FeatureData { StrData = "hello" };
            Assert.AreEqual("hello", fd.Data);
        }

        // ──────────────────────────────────────────────
        // DataL プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DataL_EmptyStrData_ReturnsEmpty()
        {
            var fd = new FeatureData { StrData = "" };
            Assert.AreEqual(0, fd.DataL.Count);
        }

        [TestMethod]
        public void DataL_MultipleWords_ReturnsList()
        {
            var fd = new FeatureData { StrData = "a b c" };
            Assert.AreEqual(3, fd.DataL.Count);
        }

        [TestMethod]
        public void DataL_FirstElement_ReturnsFirst()
        {
            var fd = new FeatureData { StrData = "first second third" };
            Assert.AreEqual("first", fd.DataL[0]);
        }
    }
}
