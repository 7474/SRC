using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// ItemData の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class ItemDataMoreTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // AddAbility / CountAbility
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountAbility_InitiallyZero()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            Assert.AreEqual(0, item.CountAbility());
        }

        [TestMethod]
        public void AddAbility_IncreasesCount()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddAbility("必殺技");
            Assert.AreEqual(1, item.CountAbility());
        }

        [TestMethod]
        public void AddAbility_Multiple_IncreasesCount()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddAbility("必殺技A");
            item.AddAbility("必殺技B");
            item.AddAbility("必殺技C");
            Assert.AreEqual(3, item.CountAbility());
        }

        [TestMethod]
        public void AddAbility_ReturnsAbilityDataWithCorrectName()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            var ad = item.AddAbility("スペシャル");
            Assert.IsNotNull(ad);
            Assert.AreEqual("スペシャル", ad.Name);
        }

        [TestMethod]
        public void Ability_ByKey_ReturnsCorrectAbility()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddAbility("スペシャル");
            var ad = item.Ability("スペシャル0");
            Assert.IsNotNull(ad);
        }

        // ──────────────────────────────────────────────
        // Feature / FeatureLevel / FeatureData int インデックス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Feature_ByIntIndex_ReturnsFeatureData()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("修理");
            var fd = item.Feature(1);
            Assert.IsNotNull(fd);
            Assert.AreEqual("修理", fd.Name);
        }

        [TestMethod]
        public void Feature_ByIntIndex_SecondFeature_ReturnsSecondFeatureData()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("修理");
            item.AddFeature("補給");
            var second = item.Feature(2);
            Assert.IsNotNull(second);
            Assert.AreEqual("補給", second.Name);
        }

        [TestMethod]
        public void FeatureLevel_ByIntIndex_ReturnsLevel()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("大型アイテムLv2");
            Assert.AreEqual(2d, item.FeatureLevel(1));
        }

        [TestMethod]
        public void FeatureLevel_ByIntIndex_DefaultLevel_ReturnsOne()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("修理");
            Assert.AreEqual(1d, item.FeatureLevel(1));
        }

        [TestMethod]
        public void FeatureData_ByIntIndex_ReturnsData()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("特殊能力=追加データ");
            Assert.AreEqual("追加データ", item.FeatureData(1));
        }

        [TestMethod]
        public void FeatureData_ByIntIndex_NoData_ReturnsEmpty()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("修理");
            Assert.AreEqual("", item.FeatureData(1));
        }

        [TestMethod]
        public void FeatureName_ByIntIndex_ReturnsName()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("補給");
            Assert.AreEqual("補給", item.FeatureName(1));
        }

        [TestMethod]
        public void FeatureName_ByIntIndex_WithLevel_ReturnsNameWithLv()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("大型アイテムLv3");
            Assert.AreEqual("大型アイテムLv3", item.FeatureName(1));
        }
    }
}
