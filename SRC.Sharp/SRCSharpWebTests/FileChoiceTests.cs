using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCSharpWeb.Tests
{
    /// <summary>
    /// ファイルを開くダイアログ（ファイルの一覧から選ぶモーダル）で初めに選んでおく名前（FileChoice.Initial）。
    /// LoadFileDialog 関数の第 3 引数（デフォルトファイル）を選んでおく（7474/SRC#1479）
    /// </summary>
    [TestClass]
    public class FileChoiceTests
    {
        private static readonly string[] Names = { "a.txt", "B.txt", "c.txt" };

        [TestMethod]
        public void Initial_ReturnsMatchingName()
        {
            Assert.AreEqual("c.txt", FileChoice.Initial(Names, "c.txt"));
        }

        [TestMethod]
        public void Initial_IgnoresCaseAndFolder()
        {
            Assert.AreEqual("B.txt", FileChoice.Initial(Names, "b.TXT"));
            Assert.AreEqual("a.txt", FileChoice.Initial(Names, @"sub\a.txt"));
            Assert.AreEqual("a.txt", FileChoice.Initial(Names, "sub/a.txt"));
        }

        [TestMethod]
        public void Initial_ReturnsNullWhenNotSpecifiedOrMissing()
        {
            Assert.IsNull(FileChoice.Initial(Names, ""));
            Assert.IsNull(FileChoice.Initial(Names, null));
            Assert.IsNull(FileChoice.Initial(Names, "d.txt"));
            Assert.IsNull(FileChoice.Initial(Names, @"sub\"));
        }
    }
}
