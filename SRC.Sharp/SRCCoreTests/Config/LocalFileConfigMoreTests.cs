using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using System;
using System.IO;

namespace SRCCore.Config.Tests
{
    /// <summary>
    /// LocalFileConfig の追加ユニットテスト（既存テストで未カバーの項目）
    /// </summary>
    [TestClass]
    public class LocalFileConfigMoreTests
    {
        // ──────────────────────────────────────────────
        // AppPath / Sections のデフォルト値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AppPath_Default_IsNotNull()
        {
            var config = new LocalFileConfig();
            Assert.IsNotNull(config.AppPath);
        }

        [TestMethod]
        public void Sections_Default_IsEmptyList()
        {
            var config = new LocalFileConfig();
            Assert.IsNotNull(config.Sections);
            Assert.AreEqual(0, config.Sections.Count);
        }

        // ──────────────────────────────────────────────
        // KeepEnemyBGM プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void KeepEnemyBGM_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            Assert.IsFalse(config.KeepEnemyBGM);
        }

        [TestMethod]
        public void KeepEnemyBGM_CanBeSetTrue()
        {
            var config = new LocalFileConfig();
            config.KeepEnemyBGM = true;
            Assert.IsTrue(config.KeepEnemyBGM);
        }

        // ──────────────────────────────────────────────
        // MidiResetType プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MidiResetType_CanBeSet()
        {
            var config = new LocalFileConfig();
            config.MidiResetType = "GM";
            Assert.AreEqual("GM", config.MidiResetType);
        }

        // ──────────────────────────────────────────────
        // AutoMoveCursor プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AutoMoveCursor_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            Assert.IsFalse(config.AutoMoveCursor);
        }

        [TestMethod]
        public void AutoMoveCursor_CanBeSetTrue()
        {
            var config = new LocalFileConfig();
            config.AutoMoveCursor = true;
            Assert.IsTrue(config.AutoMoveCursor);
        }

        // ──────────────────────────────────────────────
        // SpecialPowerAnimation プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialPowerAnimation_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            Assert.IsFalse(config.SpecialPowerAnimation);
        }

        [TestMethod]
        public void SpecialPowerAnimation_CanBeSetTrue()
        {
            var config = new LocalFileConfig();
            config.SpecialPowerAnimation = true;
            Assert.IsTrue(config.SpecialPowerAnimation);
        }

        // ──────────────────────────────────────────────
        // WeaponAnimation プロパティ
        // ──────────────────────────────────────────────

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
        // ExtendedAnimation プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExtendedAnimation_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            Assert.IsFalse(config.ExtendedAnimation);
        }

        [TestMethod]
        public void ExtendedAnimation_CanBeSetTrue()
        {
            var config = new LocalFileConfig();
            config.ExtendedAnimation = true;
            Assert.IsTrue(config.ExtendedAnimation);
        }

        // ──────────────────────────────────────────────
        // MoveAnimation プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoveAnimation_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            Assert.IsFalse(config.MoveAnimation);
        }

        [TestMethod]
        public void MoveAnimation_CanBeSetTrue()
        {
            var config = new LocalFileConfig();
            config.MoveAnimation = true;
            Assert.IsTrue(config.MoveAnimation);
        }

        // ──────────────────────────────────────────────
        // ImageBufferSize プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ImageBufferSize_DefaultIsZero()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual(0, config.ImageBufferSize);
        }

        [TestMethod]
        public void ImageBufferSize_CanBeSet()
        {
            var config = new LocalFileConfig();
            config.ImageBufferSize = 256;
            Assert.AreEqual(256, config.ImageBufferSize);
        }

        // ──────────────────────────────────────────────
        // MaxImageBufferByteSize プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MaxImageBufferByteSize_DefaultIsZero()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual(0, config.MaxImageBufferByteSize);
        }

        [TestMethod]
        public void MaxImageBufferByteSize_CanBeSet()
        {
            var config = new LocalFileConfig();
            config.MaxImageBufferByteSize = 1024 * 1024;
            Assert.AreEqual(1024 * 1024, config.MaxImageBufferByteSize);
        }

        // ──────────────────────────────────────────────
        // KeepStretchedImage プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void KeepStretchedImage_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            Assert.IsFalse(config.KeepStretchedImage);
        }

        [TestMethod]
        public void KeepStretchedImage_CanBeSetTrue()
        {
            var config = new LocalFileConfig();
            config.KeepStretchedImage = true;
            Assert.IsTrue(config.KeepStretchedImage);
        }

        // ──────────────────────────────────────────────
        // ExtDataPath2 プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExtDataPath2_CanBeSet()
        {
            var config = new LocalFileConfig();
            config.ExtDataPath2 = "/ext2/path";
            Assert.AreEqual("/ext2/path", config.ExtDataPath2);
        }

        // ──────────────────────────────────────────────
        // SetItem 空値 / 上書き
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetItem_EmptyValue_StoresEmpty()
        {
            var config = new LocalFileConfig();
            config.SetItem("Section", "Key", "");
            Assert.AreEqual("", config.GetItem("Section", "Key"));
        }

        [TestMethod]
        public void SetItem_OverwriteWithEmpty_ReturnsEmpty()
        {
            var config = new LocalFileConfig();
            config.SetItem("Section", "Key", "有値");
            config.SetItem("Section", "Key", "");
            Assert.AreEqual("", config.GetItem("Section", "Key"));
        }

        // ──────────────────────────────────────────────
        // Save / Load ラウンドトリップ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SaveLoad_RoundTrip_PreservesProperties()
        {
            var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(tempDir);
            try
            {
                var config = new LocalFileConfig();
                config.AppPath = tempDir;
                config.ShowSquareLine = true;
                config.SoundVolume = 75;
                config.ExtDataPath = "/round/trip";
                config.BattleAnimation = true;
                config.SetItem("RoundTrip", "TestKey", "TestValue");
                config.Save();

                var config2 = new LocalFileConfig();
                config2.AppPath = tempDir;
                config2.Load();

                Assert.IsTrue(config2.ShowSquareLine);
                Assert.AreEqual(75, config2.SoundVolume);
                Assert.AreEqual("/round/trip", config2.ExtDataPath);
                Assert.IsTrue(config2.BattleAnimation);
                Assert.AreEqual("TestValue", config2.GetItem("RoundTrip", "TestKey"));
            }
            finally
            {
                Directory.Delete(tempDir, recursive: true);
            }
        }

        // ──────────────────────────────────────────────
        // ConfigSection プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConfigSection_NameAndItems_DefaultEmptyList()
        {
            var section = new ConfigSection { Name = "TestSection" };
            Assert.AreEqual("TestSection", section.Name);
            Assert.IsNotNull(section.Items);
            Assert.AreEqual(0, section.Items.Count);
        }

        [TestMethod]
        public void ConfigSection_CanAddItems()
        {
            var section = new ConfigSection { Name = "S" };
            section.Items.Add(new ConfigItem { Name = "N", Value = "V" });
            Assert.AreEqual(1, section.Items.Count);
            Assert.AreEqual("N", section.Items[0].Name);
            Assert.AreEqual("V", section.Items[0].Value);
        }

        // ──────────────────────────────────────────────
        // ConfigItem プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConfigItem_NameAndValue_CanBeSetAndRead()
        {
            var item = new ConfigItem { Name = "MyName", Value = "MyValue" };
            Assert.AreEqual("MyName", item.Name);
            Assert.AreEqual("MyValue", item.Value);
        }

        [TestMethod]
        public void ConfigItem_ValueCanBeUpdated()
        {
            var item = new ConfigItem { Name = "K", Value = "Old" };
            item.Value = "New";
            Assert.AreEqual("New", item.Value);
        }
    }
}
