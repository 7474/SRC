using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.VB;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.feature.cs の特殊能力関連処理の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class UnitFeatureMoreTests3
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        /// <summary>
        /// colFeature にリフレクション経由で特殊能力を直接追加するヘルパー
        /// </summary>
        private static void AddFeatureToUnit(Unit unit, string name, double level = SRCCore.Constants.DEFAULT_LEVEL, string data = "")
        {
            var field = typeof(Unit).GetField("colFeature", BindingFlags.NonPublic | BindingFlags.Instance);
            var colFeature = (SrcCollection<FeatureData>)field.GetValue(unit);
            var fd = new FeatureData { Name = name, Level = level, StrData = data };
            if (!colFeature.ContainsKey(name))
            {
                colFeature.Add(fd, name);
            }
        }

        // ──────────────────────────────────────────────
        // IsFeatureAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsFeatureAvailable_ReturnsFalse_WhenNotAdded()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsFalse(unit.IsFeatureAvailable("装甲強化"));
        }

        [TestMethod]
        public void IsFeatureAvailable_ReturnsTrue_WhenAdded()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            AddFeatureToUnit(unit, "装甲強化");
            Assert.IsTrue(unit.IsFeatureAvailable("装甲強化"));
        }

        [TestMethod]
        public void IsFeatureAvailable_ReturnsTrue_ForMultipleFeatures()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            AddFeatureToUnit(unit, "バリア");
            AddFeatureToUnit(unit, "シールド防御");
            Assert.IsTrue(unit.IsFeatureAvailable("バリア"));
            Assert.IsTrue(unit.IsFeatureAvailable("シールド防御"));
        }

        [TestMethod]
        public void IsFeatureAvailable_ReturnsFalse_ForNonExistentAfterOtherAdded()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            AddFeatureToUnit(unit, "EN回復");
            Assert.IsFalse(unit.IsFeatureAvailable("HP回復"));
        }

        // ──────────────────────────────────────────────
        // CountFeature
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountFeature_NewUnit_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0, unit.CountFeature());
        }

        [TestMethod]
        public void CountFeature_AfterOneFeature_ReturnsOne()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            AddFeatureToUnit(unit, "再生");
            Assert.AreEqual(1, unit.CountFeature());
        }

        [TestMethod]
        public void CountFeature_AfterThreeFeatures_ReturnsThree()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            AddFeatureToUnit(unit, "再生");
            AddFeatureToUnit(unit, "EN回復");
            AddFeatureToUnit(unit, "底力");
            Assert.AreEqual(3, unit.CountFeature());
        }

        // ──────────────────────────────────────────────
        // FeatureLevel
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureLevel_ByStringIndex_ReturnsCorrectLevel()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            AddFeatureToUnit(unit, "底力", level: 3.0);
            Assert.AreEqual(3.0, unit.FeatureLevel("底力"));
        }

        [TestMethod]
        public void FeatureLevel_MissingFeature_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.AreEqual(0d, unit.FeatureLevel("存在しない特殊能力"));
        }

        [TestMethod]
        public void FeatureLevel_ByIntIndex_ReturnsCorrectLevel()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            AddFeatureToUnit(unit, "技量低下", level: 2.0);
            Assert.AreEqual(2.0, unit.FeatureLevel(1));
        }

        [TestMethod]
        public void FeatureLevel_OutOfRange_ThrowsIndexOutOfRangeException()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.ThrowsException<System.IndexOutOfRangeException>(() => unit.FeatureLevel(99));
        }

        // ──────────────────────────────────────────────
        // Feature (by index)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Feature_ByStringIndex_ReturnsFeatureData()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            AddFeatureToUnit(unit, "修理装置");
            var fd = unit.Feature("修理装置");
            Assert.IsNotNull(fd);
            Assert.AreEqual("修理装置", fd.Name);
        }

        [TestMethod]
        public void Feature_ByStringIndex_Missing_ReturnsNull()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsNull(unit.Feature("なし"));
        }

        [TestMethod]
        public void Feature_ByIntIndex_ValidIndex_ReturnsFeatureData()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            AddFeatureToUnit(unit, "補給装置");
            var fd = unit.Feature(1);
            Assert.IsNotNull(fd);
            Assert.AreEqual("補給装置", fd.Name);
        }
    }
}
