using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace SRCCore.Logging.Tests
{
    [TestClass]
    public class SRCLogTests
    {
        private static (SRC src, RecordingLoggerProvider provider) Create(LogLevel minimum = LogLevel.Trace)
        {
            var provider = new RecordingLoggerProvider();
            var factory = LoggerFactory.Create(b => b.SetMinimumLevel(minimum).AddProvider(provider));
            return (new SRC(factory), provider);
        }

        [TestMethod]
        public void LogError_IsWrittenAsError()
        {
            var (src, provider) = Create(LogLevel.Information);
            var ex = new InvalidOperationException("boom");
            src.LogError("読み込めません", ex, "a.eve");

            var entry = provider.Entries.Single();
            Assert.AreEqual(LogCategories.Core, entry.Category);
            Assert.AreEqual(LogLevel.Error, entry.Level);
            Assert.AreEqual("読み込めません a.eve", entry.Message);
            Assert.AreSame(ex, entry.Exception);
        }

        [TestMethod]
        public void LogWarn_IsWrittenAsWarning()
        {
            var (src, provider) = Create(LogLevel.Information);
            src.LogWarn("注意", null);

            var entry = provider.Entries.Single();
            Assert.AreEqual(LogLevel.Warning, entry.Level);
            Assert.AreEqual("注意", entry.Message);
        }

        [TestMethod]
        public void LogDebug_HasCallerMethodName()
        {
            var (src, provider) = Create();
            src.LogDebug("msg", "x", "y");

            var entry = provider.Entries.Single();
            Assert.AreEqual(LogLevel.Debug, entry.Level);
            Assert.AreEqual(nameof(LogDebug_HasCallerMethodName) + " msg x, y", entry.Message);
        }

        [TestMethod]
        public void DebugWithCaller_HasCallerMethodName()
        {
            var (src, provider) = Create();
            src.EventLog.DebugWithCaller(">", "Talk");

            var entry = provider.Entries.Single();
            Assert.AreEqual(LogCategories.Event, entry.Category);
            Assert.AreEqual(nameof(DebugWithCaller_HasCallerMethodName) + " > Talk", entry.Message);
        }

        [TestMethod]
        public void Message_WithBraces_IsNotTreatedAsTemplate()
        {
            var (src, provider) = Create();
            src.LogInfo("{0} {Name}", "{x}");

            Assert.AreEqual("{0} {Name} {x}", provider.Entries.Single().Message);
        }

        [TestMethod]
        public void DefaultConstructor_WritesNothing()
        {
            var src = new SRC();
            src.LogError("x");
            Assert.IsFalse(src.Log.IsEnabled(LogLevel.Critical));
        }
    }
}
