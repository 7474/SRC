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
        public void Grep_ReturnsLines()
        {
            var hits = HelpDocument.Grep("プレイヤーによる選択肢入力").ToList();
            Assert.IsTrue(hits.Any(x => x.topic.Name == "Askコマンド" && x.line > 0));
        }

        [TestMethod]
        [DataRow("選択肢を表示する", "Askコマンド")]
        [DataRow("ユニットのHPを回復", "RecoverHPコマンド")]
        [DataRow("配列をコピー", "CopyArrayコマンド")]
        [DataRow("ｂｇｍ 再生", "StartBGMコマンド")]
        public void Search_RanksRelevantPages(string query, string expected)
        {
            var results = HelpDocument.Search(query, 5);
            CollectionAssert.Contains(results.Select(x => x.Topic.Name).ToList(), expected);
            Assert.IsTrue(results.Zip(results.Skip(1)).All(x => x.First.Score >= x.Second.Score));
            Assert.IsTrue(results.All(x => x.Lines.Count > 0));
        }

        [TestMethod]
        public void Search_NoTerms()
        {
            Assert.AreEqual(0, HelpDocument.Search("  ", 5).Count);
        }

        [TestMethod]
        public void Tokenize_JapaneseBigramsAndWords()
        {
            CollectionAssert.AreEqual(new[] { "changemap", "マッ", "ップ", "hp" }, HelpIndex.Tokenize("ChangeMap マップ ＨＰ").ToArray());
            CollectionAssert.AreEqual(new[] { "回" }, HelpIndex.Tokenize("回").ToArray());
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
