using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using SRCCore.Models;
using System.Collections.Generic;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// CombineFeature クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class CombineFeatureTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ基本動作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_SetsFeatureData()
        {
            var fd = new FeatureData { StrData = "合体ダブル 合体後ユニット パートA パートB" };
            var cf = new CombineFeature(fd);
            Assert.AreEqual(fd, cf.FeatureData);
        }

        [TestMethod]
        public void Constructor_SetsCombineName()
        {
            var fd = new FeatureData { StrData = "合体ダブル 合体後ユニット パートA パートB" };
            var cf = new CombineFeature(fd);
            Assert.AreEqual("合体ダブル", cf.CombineName);
        }

        [TestMethod]
        public void Constructor_SetsConbineUnitName()
        {
            var fd = new FeatureData { StrData = "合体ダブル 合体後ユニット パートA パートB" };
            var cf = new CombineFeature(fd);
            Assert.AreEqual("合体後ユニット", cf.ConbineUnitName);
        }

        [TestMethod]
        public void Constructor_SetsPartUnitNames_TwoParts()
        {
            var fd = new FeatureData { StrData = "合体ダブル 合体後ユニット パートA パートB" };
            var cf = new CombineFeature(fd);
            Assert.AreEqual(2, cf.PartUnitNames.Count);
            Assert.AreEqual("パートA", cf.PartUnitNames[0]);
            Assert.AreEqual("パートB", cf.PartUnitNames[1]);
        }

        [TestMethod]
        public void Constructor_SetsPartUnitNames_OnePart()
        {
            var fd = new FeatureData { StrData = "合体 合体後ユニット パートA" };
            var cf = new CombineFeature(fd);
            Assert.AreEqual(1, cf.PartUnitNames.Count);
            Assert.AreEqual("パートA", cf.PartUnitNames[0]);
        }

        [TestMethod]
        public void Constructor_SetsPartUnitNames_NoParts()
        {
            var fd = new FeatureData { StrData = "合体 合体後ユニット" };
            var cf = new CombineFeature(fd);
            Assert.AreEqual(0, cf.PartUnitNames.Count);
        }

        [TestMethod]
        public void Constructor_ThreeParts_AllSet()
        {
            var fd = new FeatureData { StrData = "合体トリプル 合体後 パートA パートB パートC" };
            var cf = new CombineFeature(fd);
            Assert.AreEqual("合体トリプル", cf.CombineName);
            Assert.AreEqual("合体後", cf.ConbineUnitName);
            Assert.AreEqual(3, cf.PartUnitNames.Count);
            Assert.AreEqual("パートA", cf.PartUnitNames[0]);
            Assert.AreEqual("パートB", cf.PartUnitNames[1]);
            Assert.AreEqual("パートC", cf.PartUnitNames[2]);
        }

        // ──────────────────────────────────────────────
        // ASCII英数字での動作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_AsciiNames_ParsedCorrectly()
        {
            var fd = new FeatureData { StrData = "CombineX CombinedUnit PartA PartB" };
            var cf = new CombineFeature(fd);
            Assert.AreEqual("CombineX", cf.CombineName);
            Assert.AreEqual("CombinedUnit", cf.ConbineUnitName);
            Assert.AreEqual(2, cf.PartUnitNames.Count);
            Assert.AreEqual("PartA", cf.PartUnitNames[0]);
            Assert.AreEqual("PartB", cf.PartUnitNames[1]);
        }
    }
}
