using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using System.Collections.Generic;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// SystemConfigExtension クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class SystemConfigExtensionTests
    {
        /// <summary>
        /// テスト用のシンプルなISystemConfig実装
        /// </summary>
        private class SimpleSystemConfig : ISystemConfig
        {
            private Dictionary<string, string> _items = new Dictionary<string, string>();

            public SRCCompatibilityMode SRCCompatibilityMode { get; set; }
            public bool KeepEnemyBGM { get; set; }
            public bool AutoMoveCursor { get; set; }
            public bool ShowSquareLine { get; set; }
            public bool SpecialPowerAnimation { get; set; }
            public bool BattleAnimation { get; set; }
            public bool WeaponAnimation { get; set; }
            public bool ExtendedAnimation { get; set; }
            public bool MoveAnimation { get; set; }
            public string MidiResetType { get; set; }
            public int ImageBufferSize { get; set; }
            public int MaxImageBufferByteSize { get; set; }
            public bool KeepStretchedImage { get; set; }
            public bool AutoDefense { get; set; }
            public string AppPath { get; set; }
            public string ExtDataPath { get; set; }
            public string ExtDataPath2 { get; set; }

            public string GetItem(string section, string name)
            {
                var key = $"{section}/{name}";
                return _items.TryGetValue(key, out var value) ? value : "";
            }

            public void SetItem(string section, string name, string value)
            {
                _items[$"{section}/{name}"] = value;
            }

            public void Save() { }
            public void Load() { }
        }

        // ──────────────────────────────────────────────
        // GetFlag
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetFlag_OnValue_ReturnsTrue()
        {
            var config = new SimpleSystemConfig();
            config.SetItem("Option", "AutoDefense", "On");
            Assert.IsTrue(config.GetFlag("Option", "AutoDefense"));
        }

        [TestMethod]
        public void GetFlag_OffValue_ReturnsFalse()
        {
            var config = new SimpleSystemConfig();
            config.SetItem("Option", "AutoDefense", "Off");
            Assert.IsFalse(config.GetFlag("Option", "AutoDefense"));
        }

        [TestMethod]
        public void GetFlag_EmptyValue_ReturnsFalse()
        {
            var config = new SimpleSystemConfig();
            Assert.IsFalse(config.GetFlag("Option", "NonExistent"));
        }

        [TestMethod]
        public void GetFlag_LowercaseOn_ReturnsTrue()
        {
            var config = new SimpleSystemConfig();
            config.SetItem("Option", "Feature", "on");
            Assert.IsTrue(config.GetFlag("Option", "Feature"));
        }

        [TestMethod]
        public void GetFlag_UppercaseON_ReturnsTrue()
        {
            var config = new SimpleSystemConfig();
            config.SetItem("Option", "Feature", "ON");
            Assert.IsTrue(config.GetFlag("Option", "Feature"));
        }

        // ──────────────────────────────────────────────
        // SetFlag
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetFlag_True_SetsOnValue()
        {
            var config = new SimpleSystemConfig();
            config.SetFlag("Option", "AutoDefense", true);
            Assert.AreEqual("On", config.GetItem("Option", "AutoDefense"));
        }

        [TestMethod]
        public void SetFlag_False_SetsOffValue()
        {
            var config = new SimpleSystemConfig();
            config.SetFlag("Option", "AutoDefense", false);
            Assert.AreEqual("Off", config.GetItem("Option", "AutoDefense"));
        }

        [TestMethod]
        public void SetFlag_ThenGetFlag_RoundTrip()
        {
            var config = new SimpleSystemConfig();
            config.SetFlag("Section", "Name", true);
            Assert.IsTrue(config.GetFlag("Section", "Name"));

            config.SetFlag("Section", "Name", false);
            Assert.IsFalse(config.GetFlag("Section", "Name"));
        }

        // ──────────────────────────────────────────────
        // ToggleFlag
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToggleFlag_FromOn_BecomesOff()
        {
            var config = new SimpleSystemConfig();
            config.SetFlag("Option", "Feature", true);
            config.ToggleFlag("Option", "Feature");
            Assert.IsFalse(config.GetFlag("Option", "Feature"));
        }

        [TestMethod]
        public void ToggleFlag_FromOff_BecomesOn()
        {
            var config = new SimpleSystemConfig();
            config.SetFlag("Option", "Feature", false);
            config.ToggleFlag("Option", "Feature");
            Assert.IsTrue(config.GetFlag("Option", "Feature"));
        }

        [TestMethod]
        public void ToggleFlag_DoubleToggle_RestoresOriginal()
        {
            var config = new SimpleSystemConfig();
            config.SetFlag("Option", "Feature", true);
            config.ToggleFlag("Option", "Feature");
            config.ToggleFlag("Option", "Feature");
            Assert.IsTrue(config.GetFlag("Option", "Feature"));
        }

        [TestMethod]
        public void ToggleFlag_FromUndefined_BecomesOn()
        {
            var config = new SimpleSystemConfig();
            // 未設定はfalseとして扱われるので、トグルでtrueになる
            config.ToggleFlag("Option", "NewFlag");
            Assert.IsTrue(config.GetFlag("Option", "NewFlag"));
        }
    }
}
