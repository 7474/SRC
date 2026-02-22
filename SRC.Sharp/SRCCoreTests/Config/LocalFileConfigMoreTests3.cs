using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;

namespace SRCCore.Config.Tests
{
    /// <summary>
    /// LocalFileConfig の追加ユニットテスト（その3）
    /// </summary>
    [TestClass]
    public class LocalFileConfigMoreTests3
    {
        // ──────────────────────────────────────────────
        // デフォルト値の検証
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SoundVolume_DefaultIsFifty()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual(50, config.SoundVolume);
        }

        [TestMethod]
        public void SoundVolume_CanBeSet()
        {
            var config = new LocalFileConfig();
            config.SoundVolume = 80;
            Assert.AreEqual(80, config.SoundVolume);
        }

        [TestMethod]
        public void ExtDataPath_DefaultIsEmpty()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual("", config.ExtDataPath);
        }

        [TestMethod]
        public void ExtDataPath2_DefaultIsEmpty()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual("", config.ExtDataPath2);
        }

        [TestMethod]
        public void ExtDataPath_CanBeSet()
        {
            var config = new LocalFileConfig();
            config.ExtDataPath = @"C:\Data\SRC";
            Assert.AreEqual(@"C:\Data\SRC", config.ExtDataPath);
        }

        [TestMethod]
        public void ExtDataPath2_CanBeSet()
        {
            var config = new LocalFileConfig();
            config.ExtDataPath2 = @"D:\Extra";
            Assert.AreEqual(@"D:\Extra", config.ExtDataPath2);
        }

        // ──────────────────────────────────────────────
        // BooleanプロパティのToggle（SetFlag経由）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AutoDefense_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            Assert.IsFalse(config.AutoDefense);
        }

        [TestMethod]
        public void AutoDefense_CanBeSetTrue()
        {
            var config = new LocalFileConfig();
            config.AutoDefense = true;
            Assert.IsTrue(config.AutoDefense);
        }

        [TestMethod]
        public void AutoDefense_CanBeToggled()
        {
            var config = new LocalFileConfig();
            config.AutoDefense = true;
            config.AutoDefense = false;
            Assert.IsFalse(config.AutoDefense);
        }

        // ──────────────────────────────────────────────
        // BattleAnimation / WeaponAnimation
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BattleAnimation_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            Assert.IsFalse(config.BattleAnimation);
        }

        [TestMethod]
        public void BattleAnimation_CanBeSetTrue()
        {
            var config = new LocalFileConfig();
            config.BattleAnimation = true;
            Assert.IsTrue(config.BattleAnimation);
        }

        [TestMethod]
        public void WeaponAnimation_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            Assert.IsFalse(config.WeaponAnimation);
        }

        [TestMethod]
        public void WeaponAnimation_CanBeSetTrue()
        {
            var config = new LocalFileConfig();
            config.WeaponAnimation = true;
            Assert.IsTrue(config.WeaponAnimation);
        }

        // ──────────────────────────────────────────────
        // Sections リスト操作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Sections_DefaultIsEmpty()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual(0, config.Sections.Count);
        }

        [TestMethod]
        public void SetItem_AddsSection_WhenNew()
        {
            var config = new LocalFileConfig();
            config.SetItem("MySection", "Key", "Val");
            Assert.AreEqual(1, config.Sections.Count);
            Assert.AreEqual("MySection", config.Sections[0].Name);
        }

        [TestMethod]
        public void SetItem_SameSection_DoesNotDuplicate()
        {
            var config = new LocalFileConfig();
            config.SetItem("S", "K1", "V1");
            config.SetItem("S", "K2", "V2");
            Assert.AreEqual(1, config.Sections.Count);
        }

        [TestMethod]
        public void GetItem_MissingSection_ReturnsEmpty()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual("", config.GetItem("NoSection", "NoKey"));
        }
    }
}
