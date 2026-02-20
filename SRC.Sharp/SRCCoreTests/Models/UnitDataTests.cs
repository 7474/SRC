using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// UnitData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class UnitDataTests
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
            var ud = new UnitData(src)
            {
                Name = "ガンダム",
                Class = "モビルスーツ",
                PilotNum = 1,
                ItemNum = 4,
                Adaption = "空陸-宇",
                HP = 5000,
                EN = 120,
                Transportation = "空",
                Speed = 7,
                Size = "M",
                Armor = 1500,
                Mobility = 120,
                Value = 25000,
                ExpValue = 150
            };

            Assert.AreEqual("ガンダム", ud.Name);
            Assert.AreEqual("モビルスーツ", ud.Class);
            Assert.AreEqual(1, ud.PilotNum);
            Assert.AreEqual(4, ud.ItemNum);
            Assert.AreEqual("空陸-宇", ud.Adaption);
            Assert.AreEqual(5000, ud.HP);
            Assert.AreEqual(120, ud.EN);
            Assert.AreEqual(7, ud.Speed);
            Assert.AreEqual("M", ud.Size);
            Assert.AreEqual(1500, ud.Armor);
            Assert.AreEqual(120, ud.Mobility);
            Assert.AreEqual(25000, ud.Value);
            Assert.AreEqual(150, ud.ExpValue);
        }

        // ──────────────────────────────────────────────
        // Nickname
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_SetAndGet_ReturnsValue()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "ガンダム" };
            ud.Nickname = "白いモビルスーツ";
            Assert.AreEqual("白いモビルスーツ", ud.Nickname);
        }

        // ──────────────────────────────────────────────
        // Bitmap / IsBitmapMissing
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Bitmap_SetAndGet_ReturnsValue()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "ガンダム" };
            ud.Bitmap = "gundam.bmp";
            Assert.AreEqual("gundam.bmp", ud.Bitmap);
        }

        [TestMethod]
        public void Bitmap_WhenMissing_ReturnsDashBmp()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "ガンダム" };
            ud.Bitmap = "gundam.bmp";
            ud.IsBitmapMissing = true;
            Assert.AreEqual("-.bmp", ud.Bitmap);
        }

        [TestMethod]
        public void Bitmap0_ReturnsOriginalBitmap()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "ガンダム" };
            ud.Bitmap = "unit.bmp";
            ud.IsBitmapMissing = true;
            Assert.AreEqual("unit.bmp", ud.Bitmap0);
        }

        // ──────────────────────────────────────────────
        // AddFeature / CountFeature / IsFeatureAvailable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountFeature_InitiallyZero()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            Assert.AreEqual(0, ud.CountFeature());
        }

        [TestMethod]
        public void AddFeature_IncreasesCount()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("ニュータイプ");
            Assert.AreEqual(1, ud.CountFeature());
        }

        [TestMethod]
        public void IsFeatureAvailable_ExistingFeature_ReturnsTrue()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("ビーム無効");
            Assert.IsTrue(ud.IsFeatureAvailable("ビーム無効"));
        }

        [TestMethod]
        public void IsFeatureAvailable_NonExistingFeature_ReturnsFalse()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            Assert.IsFalse(ud.IsFeatureAvailable("ビーム無効"));
        }

        // ──────────────────────────────────────────────
        // AddWeapon / CountWeapon
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddWeapon_IncreasesCount()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "ガンダム" };
            Assert.AreEqual(0, ud.CountWeapon());
            ud.AddWeapon("ビームライフル");
            Assert.AreEqual(1, ud.CountWeapon());
        }

        [TestMethod]
        public void AddWeapon_Multiple_AddsAll()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "ガンダム" };
            ud.AddWeapon("ビームライフル");
            ud.AddWeapon("ビームサーベル");
            ud.AddWeapon("バルカン砲");
            Assert.AreEqual(3, ud.CountWeapon());
        }
    }
}
