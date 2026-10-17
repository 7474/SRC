using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using SRCLogging;
using System;
using System.IO;
using System.Linq;

namespace SRCCore.Logging.Tests
{
    [TestClass]
    public class LogHostTests
    {
        private string dir;

        [TestInitialize]
        public void Init()
        {
            dir = Path.Combine(Path.GetTempPath(), "srclog-" + Guid.NewGuid().ToString("N"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            try { Directory.Delete(dir, true); } catch { }
        }

        private string ReadLog()
        {
            var file = Directory.GetFiles(dir, "test-*.log").Single();
            using var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            return new StreamReader(stream).ReadToEnd();
        }

        [TestMethod]
        public void WritesFile_WithLevelAndCategory()
        {
            using (var host = new LogHost("test", new LogSettings { Directory = dir }, LogSettings.Empty, LogOutputs.None))
            {
                var log = host.LoggerFactory.CreateLogger("SRCCore.Event");
                log.LogDebug("出ない");
                log.LogError("イベントのエラー {File}({Line})", "a.eve", 3);
                Assert.AreEqual(dir, host.FileDirectory);
            }
            var text = ReadLog();
            StringAssert.Contains(text, "[ERR] SRCCore.Event: イベントのエラー a.eve(3)");
            Assert.IsFalse(text.Contains("出ない"));
        }

        [TestMethod]
        public void ApplyConfig_ChangesLevel_UnlessFixedByArgs()
        {
            var config = new LocalFileConfig();
            config.SetItem(LogSettings.ConfigSection, "Level", "Debug");

            using (var host = new LogHost("test", new LogSettings { Directory = dir }, LogSettings.Empty, LogOutputs.None))
            {
                var log = host.LoggerFactory.CreateLogger("SRCCore");
                Assert.IsFalse(log.IsEnabled(LogLevel.Debug));
                host.ApplyConfig(config);
                Assert.IsTrue(log.IsEnabled(LogLevel.Debug));
                Assert.IsFalse(host.LevelFixed);
            }

            using (var host = new LogHost("test", new LogSettings { Directory = dir, Level = LogLevel.Error }, LogSettings.Empty, LogOutputs.None))
            {
                host.ApplyConfig(config);
                Assert.IsFalse(host.LoggerFactory.CreateLogger("SRCCore").IsEnabled(LogLevel.Warning));
                Assert.IsTrue(host.LevelFixed);
            }
        }

        [TestMethod]
        public void ApplyConfig_FileOff()
        {
            var config = new LocalFileConfig();
            config.SetItem(LogSettings.ConfigSection, "File", "Off");
            using var host = new LogHost("test", new LogSettings { Directory = dir }, LogSettings.Empty, LogOutputs.None);
            host.ApplyConfig(config);
            Assert.IsNull(host.FileDirectory);
        }
    }
}
