using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using System.Collections.Generic;

namespace SRCCore.Logging.Tests
{
    [TestClass]
    public class LogOptionsTests
    {
        private static LocalFileConfig Config(params (string name, string value)[] items)
        {
            var config = new LocalFileConfig();
            foreach (var (name, value) in items)
            {
                config.SetItem(LogSettings.ConfigSection, name, value);
            }
            return config;
        }

        [TestMethod]
        public void FromArgs_ReadsLogArgs_AndReturnsRest()
        {
            var settings = LogSettings.FromArgs(new[] { "play", "-v", "--log-dir", "/tmp/x", "a.eve", "--no-log-file" }, out var rest);
            Assert.AreEqual(LogLevel.Debug, settings.Level);
            Assert.AreEqual("/tmp/x", settings.Directory);
            Assert.IsFalse(settings.File.Value);
            CollectionAssert.AreEqual(new[] { "play", "a.eve" }, rest);
        }

        [TestMethod]
        public void FromArgs_LogLevel()
        {
            Assert.AreEqual(LogLevel.Trace, LogSettings.FromArgs(new[] { "-vv" }).Level);
            Assert.AreEqual(LogLevel.Warning, LogSettings.FromArgs(new[] { "--log-level", "warning" }).Level);
            Assert.AreEqual(LogLevel.Error, LogSettings.FromArgs(new[] { "--log-level=Error" }).Level);
            Assert.IsNull(LogSettings.FromArgs(new[] { "--game-thread" }).Level);
        }

        [TestMethod]
        public void FromEnvironment()
        {
            var env = new Dictionary<string, string>
            {
                [LogSettings.EnvLevel] = "Verbose",
                [LogSettings.EnvFile] = "Off",
                [LogSettings.EnvDirectory] = "/var/log/src",
            };
            var settings = LogSettings.FromEnvironment(k => env.TryGetValue(k, out var v) ? v : null);
            Assert.AreEqual(LogLevel.Trace, settings.Level);
            Assert.IsFalse(settings.File.Value);
            Assert.AreEqual("/var/log/src", settings.Directory);
        }

        [TestMethod]
        public void FromConfig()
        {
            var settings = LogSettings.FromConfig(Config(
                ("Level", "Debug"),
                ("Level:SRCCore.Expression", "Trace"),
                ("File", "Off"),
                ("Path", "C:\\logs"),
                ("RetainDays", "3")));
            Assert.AreEqual(LogLevel.Debug, settings.Level);
            Assert.AreEqual(LogLevel.Trace, settings.CategoryLevels[LogCategories.Expression]);
            Assert.IsFalse(settings.File.Value);
            Assert.AreEqual("C:\\logs", settings.Directory);
            Assert.AreEqual(3, settings.RetainDays);
        }

        [TestMethod]
        public void FromConfig_Empty()
        {
            var settings = LogSettings.FromConfig(new LocalFileConfig());
            Assert.IsNull(settings.Level);
            Assert.IsNull(settings.File);
            Assert.IsNull(settings.Directory);
            Assert.AreEqual(0, settings.CategoryLevels.Count);
        }

        [TestMethod]
        public void Resolve_Defaults()
        {
            var options = LogOptions.Resolve(true);
            Assert.AreEqual(LogLevel.Information, options.Level);
            Assert.IsTrue(options.File);
            Assert.AreEqual(LogOptions.DefaultRetainDays, options.RetainDays);
            Assert.IsFalse(string.IsNullOrEmpty(options.Directory));
        }

        [TestMethod]
        public void Resolve_ArgsWinOverEnvironmentAndConfig()
        {
            var args = LogSettings.FromArgs(new[] { "-vv" });
            var env = new LogSettings { Level = LogLevel.Warning, File = false };
            var config = LogSettings.FromConfig(Config(("Level", "Error"), ("File", "On"), ("RetainDays", "2")));
            var options = LogOptions.Resolve(true, args, env, config);
            Assert.AreEqual(LogLevel.Trace, options.Level);
            Assert.IsFalse(options.File);
            Assert.AreEqual(2, options.RetainDays);
        }

        [TestMethod]
        public void IsEnabled_UsesLongestCategoryPrefix()
        {
            var config = LogSettings.FromConfig(Config(
                ("Level", "Warning"),
                ("Level:SRCCore", "Information"),
                ("Level:SRCCore.Expression", "Trace")));
            var options = LogOptions.Resolve(false, config);
            Assert.IsTrue(options.IsEnabled("SRCCore.Expression", LogLevel.Trace));
            Assert.IsFalse(options.IsEnabled("SRCCore.Event", LogLevel.Debug));
            Assert.IsTrue(options.IsEnabled("SRCCore.Event", LogLevel.Information));
            Assert.IsFalse(options.IsEnabled("SRCRender", LogLevel.Information));
            Assert.IsTrue(options.IsEnabled("SRCRender", LogLevel.Warning));
            Assert.IsFalse(options.IsEnabled("SRCRender", LogLevel.None));
        }
    }
}
