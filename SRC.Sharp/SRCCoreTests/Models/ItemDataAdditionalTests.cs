using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// ItemData クラスの追加カバレッジテスト
    /// </summary>
    [TestClass]
    public class ItemDataAdditionalTests2
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // コンストラクタのデフォルト値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_Name_DefaultIsNull()
        {
            var item = new ItemData(CreateSrc());
            Assert.IsNull(item.Name);
        }

        [TestMethod]
        public void Constructor_Class_DefaultIsNull()
        {
            var item = new ItemData(CreateSrc());
            Assert.IsNull(item.Class);
        }

        [TestMethod]
        public void Constructor_Part_DefaultIsNull()
        {
            var item = new ItemData(CreateSrc());
            Assert.IsNull(item.Part);
        }

        [TestMethod]
        public void Constructor_Armor_DefaultIsZero()
        {
            var item = new ItemData(CreateSrc());
            Assert.AreEqual(0, item.Armor);
        }

        [TestMethod]
        public void Constructor_Mobility_DefaultIsZero()
        {
            var item = new ItemData(CreateSrc());
            Assert.AreEqual(0, item.Mobility);
        }

        [TestMethod]
        public void Constructor_Speed_DefaultIsZero()
        {
            var item = new ItemData(CreateSrc());
            Assert.AreEqual(0, item.Speed);
        }

        [TestMethod]
        public void Constructor_Comment_DefaultIsNull()
        {
            var item = new ItemData(CreateSrc());
            Assert.IsNull(item.Comment);
        }

        [TestMethod]
        public void Constructor_DataComment_DefaultIsEmpty()
        {
            var item = new ItemData(CreateSrc());
            Assert.AreEqual("", item.DataComment);
        }

        // ──────────────────────────────────────────────
        // KanaName プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void KanaName_SetEmptyString_ReturnsEmpty()
        {
            var item = new ItemData(CreateSrc());
            item.KanaName = "";
            Assert.AreEqual("", item.KanaName);
        }

        [TestMethod]
        public void KanaName_SetMultipleTimes_ReturnsLatest()
        {
            var item = new ItemData(CreateSrc());
            item.KanaName = "あいうえお";
            item.KanaName = "かきくけこ";
            Assert.AreEqual("かきくけこ", item.KanaName);
        }

        // ──────────────────────────────────────────────
        // Nickname プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Nickname_SetAndGet_ReturnsValue()
        {
            var item = new ItemData(CreateSrc());
            item.Nickname = "テスト愛称";
            Assert.AreEqual("テスト愛称", item.Nickname);
        }

        [TestMethod]
        public void Nickname_SetEmptyString_ReturnsEmpty()
        {
            var item = new ItemData(CreateSrc());
            item.Nickname = "";
            Assert.AreEqual("", item.Nickname);
        }

        // ──────────────────────────────────────────────
        // HP, EN, Armor, Mobility, Speed 境界値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HP_NegativeValue_CanBeSet()
        {
            var item = new ItemData(CreateSrc()) { HP = -100 };
            Assert.AreEqual(-100, item.HP);
        }

        [TestMethod]
        public void EN_NegativeValue_CanBeSet()
        {
            var item = new ItemData(CreateSrc()) { EN = -50 };
            Assert.AreEqual(-50, item.EN);
        }

        [TestMethod]
        public void Armor_NegativeValue_CanBeSet()
        {
            var item = new ItemData(CreateSrc()) { Armor = -200 };
            Assert.AreEqual(-200, item.Armor);
        }

        [TestMethod]
        public void Mobility_NegativeValue_CanBeSet()
        {
            var item = new ItemData(CreateSrc()) { Mobility = -10 };
            Assert.AreEqual(-10, item.Mobility);
        }

        [TestMethod]
        public void Speed_NegativeValue_CanBeSet()
        {
            var item = new ItemData(CreateSrc()) { Speed = -3 };
            Assert.AreEqual(-3, item.Speed);
        }

        [TestMethod]
        public void HP_LargeValue_CanBeSet()
        {
            var item = new ItemData(CreateSrc()) { HP = 99999 };
            Assert.AreEqual(99999, item.HP);
        }

        // ──────────────────────────────────────────────
        // Features / Weapons / Abilities リスト初期化
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Features_AfterAddFeature_ContainsItem()
        {
            var item = new ItemData(CreateSrc());
            item.AddFeature("修理");
            Assert.AreEqual(1, item.Features.Count);
            Assert.AreEqual("修理", item.Features[0].Name);
        }

        [TestMethod]
        public void Weapons_AfterAddWeapon_ContainsItem()
        {
            var item = new ItemData(CreateSrc());
            item.AddWeapon("ビームライフル");
            Assert.AreEqual(1, item.Weapons.Count);
            Assert.AreEqual("ビームライフル", item.Weapons[0].Name);
        }

        [TestMethod]
        public void Abilities_AfterAddAbility_ContainsItem()
        {
            var item = new ItemData(CreateSrc());
            item.AddAbility("回復");
            Assert.AreEqual(1, item.Abilities.Count);
            Assert.AreEqual("回復", item.Abilities[0].Name);
        }

        // ──────────────────────────────────────────────
        // Feature (string Index)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Feature_ByStringIndex_ReturnsCorrectFeature()
        {
            var item = new ItemData(CreateSrc());
            item.AddFeature("修理");
            var fd = item.Feature("修理");
            Assert.IsNotNull(fd);
            Assert.AreEqual("修理", fd.Name);
        }

        [TestMethod]
        public void FeatureName_ByStringIndex_ReturnsName()
        {
            var item = new ItemData(CreateSrc());
            item.AddFeature("補給");
            Assert.AreEqual("補給", item.FeatureName("補給"));
        }

        [TestMethod]
        public void FeatureName_ByStringIndex_NonExisting_ReturnsEmpty()
        {
            var item = new ItemData(CreateSrc());
            Assert.AreEqual("", item.FeatureName("存在しない"));
        }

        [TestMethod]
        public void FeatureName_ByIntIndex_NonExisting_ThrowsException()
        {
            var item = new ItemData(CreateSrc());
            Assert.Throws<System.IndexOutOfRangeException>(() => item.FeatureName(999));
        }

        // ──────────────────────────────────────────────
        // AddFeature パース: 必要技能 / 必要条件
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddFeature_WithNecessarySkill_ParsesCorrectly()
        {
            var item = new ItemData(CreateSrc());
            item.AddFeature("修理 (ニュータイプLv3)");
            var fd = item.Feature("修理");
            Assert.IsNotNull(fd);
            Assert.AreEqual("修理", fd.Name);
            Assert.AreEqual("ニュータイプLv3", fd.NecessarySkill);
        }

        [TestMethod]
        public void AddFeature_WithNecessaryCondition_ParsesCorrectly()
        {
            var item = new ItemData(CreateSrc());
            item.AddFeature("修理 <気力120以上>");
            var fd = item.Feature("修理");
            Assert.IsNotNull(fd);
            Assert.AreEqual("修理", fd.Name);
            Assert.AreEqual("気力120以上", fd.NecessaryCondition);
        }

        [TestMethod]
        public void AddFeature_WithLevelAndData_ParsesCorrectly()
        {
            var item = new ItemData(CreateSrc());
            item.AddFeature("特殊能力Lv3=データ値");
            var fd = item.Feature("特殊能力");
            Assert.IsNotNull(fd);
            Assert.AreEqual(3d, fd.Level);
            Assert.AreEqual("データ値", fd.StrData);
        }

        [TestMethod]
        public void AddFeature_WithQuotedData_RemovesQuotes()
        {
            var item = new ItemData(CreateSrc());
            item.AddFeature("特殊=\"テストデータ\"");
            var fd = item.Feature("特殊");
            Assert.IsNotNull(fd);
            Assert.AreEqual("テストデータ", fd.StrData);
        }

        [TestMethod]
        public void AddFeature_DuplicateName_BothStored()
        {
            var item = new ItemData(CreateSrc());
            item.AddFeature("修理");
            item.AddFeature("修理");
            Assert.AreEqual(2, item.CountFeature());
        }

        // ──────────────────────────────────────────────
        // Weapon (string Index)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Weapon_ByStringIndex_ReturnsCorrectWeapon()
        {
            var item = new ItemData(CreateSrc());
            item.AddWeapon("ビームサーベル");
            var wd = item.Weapon("ビームサーベル0");
            Assert.IsNotNull(wd);
            Assert.AreEqual("ビームサーベル", wd.Name);
        }

        [TestMethod]
        public void AddWeapon_SetsCorrectName()
        {
            var item = new ItemData(CreateSrc());
            var wd = item.AddWeapon("ファンネル");
            Assert.AreEqual("ファンネル", wd.Name);
        }

        // ──────────────────────────────────────────────
        // Size
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Size_WithLargeItemLv3_ReturnsFour()
        {
            var item = new ItemData(CreateSrc());
            item.AddFeature("大型アイテムLv3");
            Assert.AreEqual(4, item.Size());
        }

        // ──────────────────────────────────────────────
        // Raw / DataComment
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Raw_CanBeSetAndRead()
        {
            var item = new ItemData(CreateSrc());
            item.Raw = "rawdata";
            Assert.AreEqual("rawdata", item.Raw);
        }

        [TestMethod]
        public void DataComment_CanBeSetAndRead()
        {
            var item = new ItemData(CreateSrc());
            item.DataComment = "コメント";
            Assert.AreEqual("コメント", item.DataComment);
        }
    }
}
