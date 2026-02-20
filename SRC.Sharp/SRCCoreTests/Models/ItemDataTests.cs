using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// ItemData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class ItemDataTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // フィールドの設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Fields_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var item = new ItemData(src)
            {
                Name = "ミノフスキークラフト",
                Class = "宇宙",
                Part = "本体",
                HP = 500,
                EN = 100,
                Armor = 200,
                Mobility = 10,
                Speed = 1,
                Comment = "宇宙空間用推進装置"
            };

            Assert.AreEqual("ミノフスキークラフト", item.Name);
            Assert.AreEqual("宇宙", item.Class);
            Assert.AreEqual("本体", item.Part);
            Assert.AreEqual(500, item.HP);
            Assert.AreEqual(100, item.EN);
            Assert.AreEqual(200, item.Armor);
            Assert.AreEqual(10, item.Mobility);
            Assert.AreEqual(1, item.Speed);
            Assert.AreEqual("宇宙空間用推進装置", item.Comment);
        }

        // ──────────────────────────────────────────────
        // AddFeature / CountFeature / IsFeatureAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddFeature_Simple_IncreasesCount()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            Assert.AreEqual(0, item.CountFeature());

            item.AddFeature("変形");
            Assert.AreEqual(1, item.CountFeature());
        }

        [TestMethod]
        public void AddFeature_Multiple_AddsAll()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("変形");
            item.AddFeature("修理");
            item.AddFeature("補給");
            Assert.AreEqual(3, item.CountFeature());
        }

        [TestMethod]
        public void IsFeatureAvailable_ExistingFeature_ReturnsTrue()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("修理");
            Assert.IsTrue(item.IsFeatureAvailable("修理"));
        }

        [TestMethod]
        public void IsFeatureAvailable_NonExistingFeature_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("修理");
            Assert.IsFalse(item.IsFeatureAvailable("補給"));
        }

        [TestMethod]
        public void IsFeatureAvailable_EmptyCollection_ReturnsFalse()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            Assert.IsFalse(item.IsFeatureAvailable("修理"));
        }

        // ──────────────────────────────────────────────
        // FeatureLevel / FeatureData
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureLevel_NonExistingFeature_ReturnsZero()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            // 存在しない能力のレベルは 0 を返す
            Assert.AreEqual(0d, item.FeatureLevel("存在しない能力"));
        }

        [TestMethod]
        public void FeatureLevel_WithLevel_ReturnsLevel()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("大型アイテムLv2");
            Assert.AreEqual(2d, item.FeatureLevel("大型アイテム"));
        }

        [TestMethod]
        public void FeatureData_NonExistingFeature_ReturnsEmpty()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            Assert.AreEqual("", item.FeatureData("存在しない能力"));
        }

        // ──────────────────────────────────────────────
        // AddWeapon / CountWeapon
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddWeapon_IncreasesCount()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            Assert.AreEqual(0, item.CountWeapon());

            item.AddWeapon("ビームガン");
            Assert.AreEqual(1, item.CountWeapon());
        }

        [TestMethod]
        public void AddWeapon_Multiple_AddsAll()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddWeapon("ビームガン");
            item.AddWeapon("ビームサーベル");
            Assert.AreEqual(2, item.CountWeapon());
        }

        // ──────────────────────────────────────────────
        // Size
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Size_WithoutLargeItemFeature_ReturnsOne()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            // 大型アイテム能力がない場合サイズは1
            Assert.AreEqual(1, item.Size());
        }

        [TestMethod]
        public void Size_WithLargeItemFeature_ReturnsTwo()
        {
            var src = CreateSRC();
            var item = new ItemData(src);
            item.AddFeature("大型アイテム");
            // 大型アイテム (レベル1) = サイズ2
            Assert.AreEqual(2, item.Size());
        }
    }
}
