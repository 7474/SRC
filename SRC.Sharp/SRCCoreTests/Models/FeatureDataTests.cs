using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class FeatureDataTests
    {
        [TestMethod]
        public void HasLevel_ReturnsFalse_WhenLevelIsDefault()
        {
            var fd = new FeatureData { Level = Constants.DEFAULT_LEVEL };
            Assert.IsFalse(fd.HasLevel);
        }

        [TestMethod]
        public void HasLevel_ReturnsTrue_WhenLevelIsNotDefault()
        {
            var fd = new FeatureData { Level = 5 };
            Assert.IsTrue(fd.HasLevel);
        }

        [TestMethod]
        public void HasLevel_ReturnsTrue_WhenLevelIsZero()
        {
            var fd = new FeatureData { Level = 0 };
            Assert.IsTrue(fd.HasLevel);
        }

        [TestMethod]
        public void FeatureLevel_Returns1_WhenNoLevelSpecified()
        {
            var fd = new FeatureData { Level = Constants.DEFAULT_LEVEL };
            Assert.AreEqual(1d, fd.FeatureLevel);
        }

        [TestMethod]
        public void FeatureLevel_ReturnsActualLevel_WhenLevelSpecified()
        {
            var fd = new FeatureData { Level = 3 };
            Assert.AreEqual(3d, fd.FeatureLevel);
        }

        [TestMethod]
        public void Data_ReturnsEmptyString_WhenStrDataIsNull()
        {
            var fd = new FeatureData { StrData = null };
            Assert.AreEqual("", fd.Data);
        }

        [TestMethod]
        public void Data_ReturnsStrData_WhenSet()
        {
            var fd = new FeatureData { StrData = "test data" };
            Assert.AreEqual("test data", fd.Data);
        }

        [TestMethod]
        public void DataL_ReturnsListFromData()
        {
            var fd = new FeatureData { StrData = "a b c" };
            var list = fd.DataL;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("a", list[0]);
            Assert.AreEqual("b", list[1]);
            Assert.AreEqual("c", list[2]);
        }

        [TestMethod]
        public void DataL_ReturnsEmptyList_WhenNoData()
        {
            var fd = new FeatureData { StrData = null };
            var list = fd.DataL;
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void FeatureNameWithLv_ReturnsStrData_WhenDataHasContent()
        {
            var fd = new FeatureData { Name = "能力", Level = 3, StrData = "別名" };
            Assert.AreEqual("別名", fd.FeatureNameWithLv());
        }

        [TestMethod]
        public void FeatureNameWithLv_ReturnsNameWithLevel_WhenLevelPositive()
        {
            var fd = new FeatureData { Name = "格闘強化", Level = 2, StrData = "" };
            Assert.AreEqual("格闘強化Lv2", fd.FeatureNameWithLv());
        }

        [TestMethod]
        public void FeatureNameWithLv_ReturnsName_WhenNoLevelAndNoData()
        {
            var fd = new FeatureData { Name = "シールド", Level = 0, StrData = "" };
            Assert.AreEqual("シールド", fd.FeatureNameWithLv());
        }

        [TestMethod]
        public void FeatureNameWithLv_ReturnsName_WhenLevelIsNegative()
        {
            var fd = new FeatureData { Name = "テスト", Level = -1, StrData = "" };
            Assert.AreEqual("テスト", fd.FeatureNameWithLv());
        }

        [TestMethod]
        public void Properties_CanBeSetAndRead()
        {
            var fd = new FeatureData
            {
                Name = "HP回復",
                Level = 5,
                StrData = "data",
                NecessarySkill = "スキル",
                NecessaryCondition = "条件"
            };

            Assert.AreEqual("HP回復", fd.Name);
            Assert.AreEqual(5d, fd.Level);
            Assert.AreEqual("data", fd.StrData);
            Assert.AreEqual("スキル", fd.NecessarySkill);
            Assert.AreEqual("条件", fd.NecessaryCondition);
        }
    }
}
