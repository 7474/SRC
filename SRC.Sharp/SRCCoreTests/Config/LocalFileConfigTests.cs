using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCCore.Config.Tests
{
    [TestClass()]
    public class LocalFileConfigTests
    {
        [TestMethod()]
        public void LoadTest()
        {
            var config = new LocalFileConfig();
            config.Load();

            Assert.AreEqual(true, config.ShowSquareLine);
            Assert.AreEqual("TestValue1", config.GetItem("TestSection1", "TestName1"));
            Assert.AreEqual("TestValue2", config.GetItem("TestSection1", "TestName2"));
            Assert.AreEqual("", config.GetItem("TestSection1", "TestName3"));
            Assert.AreEqual("", config.GetItem("TestSection2", "TestName1"));
        }

        // ──────────────────────────────────────────────
        // GetItem / SetItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetItem_NonExistentSection_ReturnsEmpty()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual("", config.GetItem("NonExistent", "Name"));
        }

        [TestMethod]
        public void GetItem_NonExistentItem_ReturnsEmpty()
        {
            var config = new LocalFileConfig();
            config.SetItem("Section", "ExistingItem", "value");
            Assert.AreEqual("", config.GetItem("Section", "NonExistent"));
        }

        [TestMethod]
        public void SetItem_NewSection_CreatesSection()
        {
            var config = new LocalFileConfig();
            config.SetItem("NewSection", "Name", "Value");
            Assert.AreEqual("Value", config.GetItem("NewSection", "Name"));
        }

        [TestMethod]
        public void SetItem_ExistingItem_UpdatesValue()
        {
            var config = new LocalFileConfig();
            config.SetItem("Section", "Name", "OldValue");
            config.SetItem("Section", "Name", "NewValue");
            Assert.AreEqual("NewValue", config.GetItem("Section", "Name"));
        }

        [TestMethod]
        public void SetItem_MultipleItems_AllRetrievable()
        {
            var config = new LocalFileConfig();
            config.SetItem("Section", "Name1", "Value1");
            config.SetItem("Section", "Name2", "Value2");
            Assert.AreEqual("Value1", config.GetItem("Section", "Name1"));
            Assert.AreEqual("Value2", config.GetItem("Section", "Name2"));
        }

        // ──────────────────────────────────────────────
        // AutoDefense (uses GetFlag/SetFlag extension)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AutoDefense_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            Assert.IsFalse(config.AutoDefense);
        }

        [TestMethod]
        public void AutoDefense_SetTrue_GetTrue()
        {
            var config = new LocalFileConfig();
            config.AutoDefense = true;
            Assert.IsTrue(config.AutoDefense);
        }

        [TestMethod]
        public void AutoDefense_SetFalse_GetFalse()
        {
            var config = new LocalFileConfig();
            config.AutoDefense = true;
            config.AutoDefense = false;
            Assert.IsFalse(config.AutoDefense);
        }

        // ──────────────────────────────────────────────
        // SRCCompatibilityMode
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SRCCompatibilityMode_DefaultIsReadWrite()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual(SRCCompatibilityMode.ReadWrite, config.SRCCompatibilityMode);
        }

        [TestMethod]
        public void SRCCompatibilityMode_CanBeChanged()
        {
            var config = new LocalFileConfig();
            config.SRCCompatibilityMode = SRCCompatibilityMode.Read;
            Assert.AreEqual(SRCCompatibilityMode.Read, config.SRCCompatibilityMode);
        }

        // ──────────────────────────────────────────────
        // SoundVolume
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SoundVolume_DefaultIs50()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual(50, config.SoundVolume);
        }

        [TestMethod]
        public void SoundVolume_CanBeChanged()
        {
            var config = new LocalFileConfig();
            config.SoundVolume = 75;
            Assert.AreEqual(75, config.SoundVolume);
        }

        // ──────────────────────────────────────────────
        // ExtDataPath
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExtDataPath_DefaultIsEmpty()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual("", config.ExtDataPath);
        }

        [TestMethod]
        public void ExtDataPath_CanBeChanged()
        {
            var config = new LocalFileConfig();
            config.ExtDataPath = "/path/to/data";
            Assert.AreEqual("/path/to/data", config.ExtDataPath);
        }

        [TestMethod]
        public void ExtDataPath2_DefaultIsEmpty()
        {
            var config = new LocalFileConfig();
            Assert.AreEqual("", config.ExtDataPath2);
        }

        // ──────────────────────────────────────────────
        // Boolean properties
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
        public void ShowSquareLine_DefaultIsFalse()
        {
            var config = new LocalFileConfig();
            // Default (not loaded) is false
            Assert.IsFalse(config.ShowSquareLine);
        }

        [TestMethod]
        public void SetItem_MultipleSections_IndependentStorage()
        {
            var config = new LocalFileConfig();
            config.SetItem("SectionA", "Key", "ValueA");
            config.SetItem("SectionB", "Key", "ValueB");
            Assert.AreEqual("ValueA", config.GetItem("SectionA", "Key"));
            Assert.AreEqual("ValueB", config.GetItem("SectionB", "Key"));
        }
    }
}