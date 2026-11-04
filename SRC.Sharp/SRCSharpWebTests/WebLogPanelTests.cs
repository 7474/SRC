using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace SRCSharpWeb.Tests
{
    /// <summary>
    /// 画面の下のログ（WebLogPanelProvider）。ログをためておき、srcweb.js が取り出す
    /// </summary>
    [TestClass]
    [DoNotParallelize]
    public class WebLogPanelTests
    {
        [TestInitialize]
        public void Setup()
        {
            WebLogPanelProvider.Take();
        }

        [TestMethod]
        public void Logger_FormatsLevelCategoryAndMessage()
        {
            var logger = new WebLogPanelProvider().CreateLogger("SRCCore.Event");
            logger.LogWarning("行 {Line} で止まりました", 12);

            var line = WebLogPanelProvider.Take();
            StringAssert.Matches(line, new System.Text.RegularExpressions.Regex(@"^\d\d:\d\d:\d\d\.\d{3} WRN SRCCore\.Event: 行 12 で止まりました$"));
            Assert.AreEqual("", WebLogPanelProvider.Take(), "取り出したものは消える");
        }

        [TestMethod]
        public void Logger_AppendsException()
        {
            var logger = new WebLogPanelProvider().CreateLogger("SRCSharpWeb");
            logger.LogError(new InvalidOperationException("壊れた"), "失敗");

            var text = WebLogPanelProvider.Take();
            StringAssert.Contains(text, "ERR SRCSharpWeb: 失敗");
            StringAssert.Contains(text, "System.InvalidOperationException: 壊れた");
        }

        [TestMethod]
        public void Add_KeepsOnlyTheLatestLines()
        {
            for (var i = 0; i < WebLogPanelProvider.Capacity + 10; i++)
            {
                WebLogPanelProvider.Add("line " + i);
            }

            var lines = WebLogPanelProvider.Take().Split('\n');
            Assert.AreEqual(WebLogPanelProvider.Capacity, lines.Length);
            Assert.AreEqual("line 10", lines.First());
            Assert.AreEqual("line " + (WebLogPanelProvider.Capacity + 9), lines.Last());
        }
    }
}
