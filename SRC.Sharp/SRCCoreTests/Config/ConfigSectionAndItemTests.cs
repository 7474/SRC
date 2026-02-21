using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using System.Collections.Generic;

namespace SRCCore.Config.Tests
{
    /// <summary>
    /// ConfigSection / ConfigItem クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class ConfigSectionAndItemTests
    {
        // ──────────────────────────────────────────────
        // ConfigItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConfigItem_Name_CanBeSetAndRead()
        {
            var item = new ConfigItem { Name = "TestKey" };
            Assert.AreEqual("TestKey", item.Name);
        }

        [TestMethod]
        public void ConfigItem_Value_CanBeSetAndRead()
        {
            var item = new ConfigItem { Value = "TestValue" };
            Assert.AreEqual("TestValue", item.Value);
        }

        [TestMethod]
        public void ConfigItem_DefaultValues_AreNull()
        {
            var item = new ConfigItem();
            Assert.IsNull(item.Name);
            Assert.IsNull(item.Value);
        }

        [TestMethod]
        public void ConfigItem_BothFields_CanBeSetTogether()
        {
            var item = new ConfigItem { Name = "Key", Value = "Val" };
            Assert.AreEqual("Key", item.Name);
            Assert.AreEqual("Val", item.Value);
        }

        [TestMethod]
        public void ConfigItem_Value_CanBeUpdated()
        {
            var item = new ConfigItem { Name = "K", Value = "old" };
            item.Value = "new";
            Assert.AreEqual("new", item.Value);
        }

        // ──────────────────────────────────────────────
        // ConfigSection
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ConfigSection_Name_CanBeSetAndRead()
        {
            var section = new ConfigSection { Name = "Section1" };
            Assert.AreEqual("Section1", section.Name);
        }

        [TestMethod]
        public void ConfigSection_Items_DefaultIsEmpty()
        {
            var section = new ConfigSection();
            Assert.IsNotNull(section.Items);
            Assert.AreEqual(0, section.Items.Count);
        }

        [TestMethod]
        public void ConfigSection_Items_CanAddConfigItems()
        {
            var section = new ConfigSection { Name = "Section" };
            section.Items.Add(new ConfigItem { Name = "Key1", Value = "Val1" });
            section.Items.Add(new ConfigItem { Name = "Key2", Value = "Val2" });

            Assert.AreEqual(2, section.Items.Count);
            Assert.AreEqual("Key1", section.Items[0].Name);
            Assert.AreEqual("Val2", section.Items[1].Value);
        }

        [TestMethod]
        public void ConfigSection_Items_CanBeSetDirectly()
        {
            var section = new ConfigSection
            {
                Name = "SectionX",
                Items = new List<ConfigItem>
                {
                    new ConfigItem { Name = "A", Value = "1" },
                    new ConfigItem { Name = "B", Value = "2" },
                }
            };

            Assert.AreEqual(2, section.Items.Count);
        }
    }
}
