using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// DialogItem クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class DialogItemTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_SetsNameAndMessage()
        {
            var item = new DialogItem("主人公", "いくぞ！");
            Assert.AreEqual("主人公", item.strName);
            Assert.AreEqual("いくぞ！", item.strMessage);
        }

        [TestMethod]
        public void Constructor_WithEmptyStrings_SetsEmpty()
        {
            var item = new DialogItem("", "");
            Assert.AreEqual("", item.strName);
            Assert.AreEqual("", item.strMessage);
        }

        [TestMethod]
        public void Constructor_WithNullName_SetsNull()
        {
            var item = new DialogItem(null, "message");
            Assert.IsNull(item.strName);
            Assert.AreEqual("message", item.strMessage);
        }

        [TestMethod]
        public void Constructor_WithNullMessage_SetsNull()
        {
            var item = new DialogItem("name", null);
            Assert.AreEqual("name", item.strName);
            Assert.IsNull(item.strMessage);
        }

        [TestMethod]
        public void Constructor_WithJapaneseName_SetsCorrectly()
        {
            var item = new DialogItem("田中太郎", "よろしくお願いします");
            Assert.AreEqual("田中太郎", item.strName);
            Assert.AreEqual("よろしくお願いします", item.strMessage);
        }

        [TestMethod]
        public void Properties_AreReadOnly_CannotBeChanged()
        {
            // DialogItem の strName と strMessage は get のみ（コンストラクタで設定）
            var item = new DialogItem("speaker", "line");
            // プロパティは変更できないことを確認
            Assert.AreEqual("speaker", item.strName);
            Assert.AreEqual("line", item.strMessage);
        }

        [TestMethod]
        public void TwoInstances_WithSameValues_AreNotSameObject()
        {
            var item1 = new DialogItem("A", "msg");
            var item2 = new DialogItem("A", "msg");
            Assert.AreNotSame(item1, item2);
            Assert.AreEqual(item1.strName, item2.strName);
            Assert.AreEqual(item1.strMessage, item2.strMessage);
        }

        [TestMethod]
        public void Constructor_WithLongMessage_SetsCorrectly()
        {
            var longMsg = new string('あ', 1000);
            var item = new DialogItem("speaker", longMsg);
            Assert.AreEqual(longMsg, item.strMessage);
        }
    }
}
