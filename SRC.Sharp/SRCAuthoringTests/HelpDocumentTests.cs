using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCSharpTools;
using System.IO;
using System.Linq;

namespace SRCAuthoring.Tests
{
    /// <summary>srcsharp help で引く同梱のヘルプ</summary>
    [TestClass]
    public class HelpDocumentTests
    {
        [TestMethod]
        public void Topics_AreBundled()
        {
            Assert.IsTrue(HelpDocument.Topics.Count > 400);
            Assert.IsFalse(HelpDocument.Topics.Any(x => x.Name == "404"));
            var ask = HelpDocument.Topics.Single(x => x.Name == "Askコマンド");
            Assert.AreEqual("Askコマンド", ask.Title);
            Assert.IsFalse(ask.Body.StartsWith("---"));
            StringAssert.StartsWith(ask.Source, "---");
        }

        [TestMethod]
        public void Title_NumberedPageUsesHeading()
        {
            Assert.AreEqual("パイロットの名称", HelpDocument.Topics.Single(x => x.Name == "HLP000164").Title);
        }

        [TestMethod]
        [DataRow("Askコマンド")]
        [DataRow("askコマンド")]
        [DataRow("Askコマンド.md")]
        [DataRow("Askコマンド.md#書式")]
        [DataRow("Ask")]
        public void Find_ByNameOrLink(string query)
        {
            var found = HelpDocument.Find(query);
            Assert.AreEqual(1, found.Count, string.Join(",", found.Select(x => x.Name)));
            Assert.AreEqual("Askコマンド", found[0].Name);
        }

        [TestMethod]
        public void Find_ByTitle()
        {
            Assert.AreEqual("HLP000164", HelpDocument.Find("パイロットの名称").Single().Name);
        }

        [TestMethod]
        public void Find_AmbiguousAndMissing()
        {
            Assert.IsTrue(HelpDocument.Find("Clear").Count > 1);
            Assert.AreEqual(0, HelpDocument.Find("存在しない項目XYZ").Count);
        }

        [TestMethod]
        public void Search_ReturnsLines()
        {
            var hits = HelpDocument.Search("プレイヤーによる選択肢入力").ToList();
            Assert.IsTrue(hits.Any(x => x.topic.Name == "Askコマンド" && x.line > 0));
        }

        [TestMethod]
        public void Export_WritesAllPages()
        {
            var dir = Path.Combine(Path.GetTempPath(), "srcsharp-help-" + System.Guid.NewGuid().ToString("N"));
            try
            {
                Assert.AreEqual(HelpDocument.Topics.Count, HelpDocument.Export(dir));
                Assert.IsTrue(File.Exists(Path.Combine(dir, "Askコマンド.md")));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
