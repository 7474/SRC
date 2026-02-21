using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using System.Collections.Generic;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// FeatureData の追加テスト（FeatureNameWithLv / Data / DataL の追加パターン）
    /// </summary>
    [TestClass]
    public class FeatureDataMoreTests
    {
        // ──────────────────────────────────────────────
        // Data / DataL の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Data_MultipleWords_ReturnsFullString()
        {
            var fd = new FeatureData { StrData = "a b c d e" };
            Assert.AreEqual("a b c d e", fd.Data);
        }

        [TestMethod]
        public void DataL_MultipleWords_ReturnsList()
        {
            var fd = new FeatureData { StrData = "炎 水 風 雷" };
            var list = fd.DataL;
            Assert.AreEqual(4, list.Count);
            Assert.AreEqual("炎", list[0]);
            Assert.AreEqual("雷", list[3]);
        }

        [TestMethod]
        public void DataL_WhitespaceOnly_ReturnsEmptyList()
        {
            var fd = new FeatureData { StrData = "   " };
            Assert.AreEqual(0, fd.DataL.Count);
        }

        [TestMethod]
        public void DataL_EmptyStrData_ReturnsEmptyList()
        {
            var fd = new FeatureData { StrData = "" };
            Assert.AreEqual(0, fd.DataL.Count);
        }

        // ──────────────────────────────────────────────
        // FeatureNameWithLv の追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureNameWithLv_PositiveLevel_IncludesLv()
        {
            var fd = new FeatureData { Name = "HP回復", Level = 5, StrData = "" };
            var name = fd.FeatureNameWithLv();
            Assert.IsTrue(name.Contains("Lv"), $"Expected 'Lv' in '{name}'");
            Assert.IsTrue(name.Contains("5"), $"Expected '5' in '{name}'");
        }

        [TestMethod]
        public void FeatureNameWithLv_StrDataOverridesNameAndLevel()
        {
            var fd = new FeatureData { Name = "HP回復", Level = 3, StrData = "カスタム名" };
            Assert.AreEqual("カスタム名", fd.FeatureNameWithLv());
        }

        [TestMethod]
        public void FeatureNameWithLv_LevelIsDefaultConstant_ReturnsNameOnly()
        {
            var fd = new FeatureData { Name = "シールド", Level = Constants.DEFAULT_LEVEL, StrData = "" };
            // Level <= 0 (DEFAULT_LEVEL is negative) → returns Name only
            Assert.AreEqual("シールド", fd.FeatureNameWithLv());
        }

        // ──────────────────────────────────────────────
        // HasLevel の追加パターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasLevel_LargePositiveValue_ReturnsTrue()
        {
            var fd = new FeatureData { Level = 100 };
            Assert.IsTrue(fd.HasLevel);
        }

        [TestMethod]
        public void HasLevel_LargeNegativeValue_ReturnsTrue()
        {
            var fd = new FeatureData { Level = -100 };
            Assert.IsTrue(fd.HasLevel);
        }

        // ──────────────────────────────────────────────
        // FeatureLevel の追加パターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureLevel_NegativeLevel_ReturnsNegative()
        {
            var fd = new FeatureData { Level = -3 };
            Assert.AreEqual(-3d, fd.FeatureLevel);
        }

        [TestMethod]
        public void FeatureLevel_FractionalLevel_ReturnsFraction()
        {
            var fd = new FeatureData { Level = 1.5 };
            Assert.AreEqual(1.5d, fd.FeatureLevel);
        }

        // ──────────────────────────────────────────────
        // NecessarySkill / NecessaryCondition の追加パターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NecessarySkill_WithValue_IsPreserved()
        {
            var fd = new FeatureData { NecessarySkill = "魔力" };
            Assert.AreEqual("魔力", fd.NecessarySkill);
        }

        [TestMethod]
        public void NecessaryCondition_WithValue_IsPreserved()
        {
            var fd = new FeatureData { NecessaryCondition = "ハイパーモード" };
            Assert.AreEqual("ハイパーモード", fd.NecessaryCondition);
        }

        [TestMethod]
        public void NecessarySkill_Empty_IsEmpty()
        {
            var fd = new FeatureData { NecessarySkill = "" };
            Assert.AreEqual("", fd.NecessarySkill);
        }

        // ──────────────────────────────────────────────
        // 複数インスタンスの独立性
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MultipleInstances_AreIndependent()
        {
            var fd1 = new FeatureData { Name = "A", Level = 1, StrData = "d1" };
            var fd2 = new FeatureData { Name = "B", Level = 2, StrData = "d2" };

            Assert.AreNotEqual(fd1.Name, fd2.Name);
            Assert.AreNotEqual(fd1.Level, fd2.Level);
            Assert.AreNotEqual(fd1.StrData, fd2.StrData);
        }
    }
}
