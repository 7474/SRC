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

        // ──────────────────────────────────────────────
        // KanaName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void KanaName_SetAndGet_ReturnsValue()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.KanaName = "てすと";
            Assert.AreEqual("てすと", ud.KanaName);
        }

        // ──────────────────────────────────────────────
        // AddAbility / CountAbility
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CountAbility_InitiallyZero()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            Assert.AreEqual(0, ud.CountAbility());
        }

        [TestMethod]
        public void AddAbility_IncreasesCount()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddAbility("マップ兵器");
            Assert.AreEqual(1, ud.CountAbility());
        }

        // ──────────────────────────────────────────────
        // FeatureLevel / FeatureData / FeatureName / Feature
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureLevel_WithLv_ReturnsLevel()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("格闘強化Lv3");
            Assert.AreEqual(3d, ud.FeatureLevel("格闘強化"));
        }

        [TestMethod]
        public void FeatureData_WithData_ReturnsData()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("吸収=ビーム");
            Assert.AreEqual("ビーム", ud.FeatureData("吸収"));
        }

        [TestMethod]
        public void FeatureName_WithLv_ReturnsNameWithLv()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("射撃強化Lv2");
            Assert.AreEqual("射撃強化Lv2", ud.FeatureName("射撃強化"));
        }

        [TestMethod]
        public void Feature_ByIndex_ReturnsFeatureData()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("ビーム無効");
            var fd = ud.Feature(1);
            Assert.IsNotNull(fd);
            Assert.AreEqual("ビーム無効", fd.Name);
        }

        // ──────────────────────────────────────────────
        // AddWeapon / Weapon
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddWeapon_ReturnsWeaponWithCorrectName()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            var wd = ud.AddWeapon("ビームライフル");
            Assert.AreEqual("ビームライフル", wd.Name);
        }

        [TestMethod]
        public void Weapon_ByIndex_ReturnsWeapon()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddWeapon("格闘武器");
            var wd = ud.Weapon(1);
            Assert.IsNotNull(wd);
            Assert.AreEqual("格闘武器", wd.Name);
        }

        // ──────────────────────────────────────────────
        // Clear
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_ResetsAllCollections()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("ビーム無効");
            ud.AddWeapon("ビームライフル");
            ud.AddAbility("マップ兵器");
            ud.Clear();
            Assert.AreEqual(0, ud.CountFeature());
            Assert.AreEqual(0, ud.CountWeapon());
            Assert.AreEqual(0, ud.CountAbility());
        }

        // ──────────────────────────────────────────────
        // ID フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ID_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト", ID = 42 };
            Assert.AreEqual(42, ud.ID);
        }
    }
}
