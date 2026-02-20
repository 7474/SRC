using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// DialogItem・MessageData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class DialogItemAndMessageDataTests
    {
        // ──────────────────────────────────────────────
        // DialogItem
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DialogItem_Constructor_SetsProperties()
        {
            var item = new DialogItem("主人公", "行くぞ！");
            Assert.AreEqual("主人公", item.strName);
            Assert.AreEqual("行くぞ！", item.strMessage);
        }

        [TestMethod]
        public void DialogItem_Properties_AreReadOnly()
        {
            var item = new DialogItem("ヒロイン", "気をつけて！");
            Assert.AreEqual("ヒロイン", item.strName);
            Assert.AreEqual("気をつけて！", item.strMessage);
        }

        [TestMethod]
        public void DialogItem_EmptyStrings_AreAllowed()
        {
            var item = new DialogItem("", "");
            Assert.AreEqual("", item.strName);
            Assert.AreEqual("", item.strMessage);
        }

        // ──────────────────────────────────────────────
        // MessageData
        // ──────────────────────────────────────────────

        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        [TestMethod]
        public void MessageData_AddMessage_AddsToItems()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("格闘", "やるぞ！");

            Assert.AreEqual(1, md.Items.Count);
            Assert.AreEqual("格闘", md.Items[0].Situation);
            Assert.AreEqual("やるぞ！", md.Items[0].Message);
        }

        [TestMethod]
        public void MessageData_AddMessage_MultipleMessages()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("格闘", "メッセージ1");
            md.AddMessage("射撃", "メッセージ2");
            md.AddMessage("回避", "メッセージ3");

            Assert.AreEqual(3, md.Items.Count);
        }

        [TestMethod]
        public void MessageData_SelectMessage_ReturnsMatchingSituation()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("格闘", "格闘メッセージ");
            md.AddMessage("射撃", "射撃メッセージ");

            var result = md.SelectMessage("格闘");
            Assert.AreEqual("格闘メッセージ", result);
        }

        [TestMethod]
        public void MessageData_SelectMessage_格闘は攻撃にもマッチ()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("攻撃", "攻撃メッセージ");

            // 「格闘」は「攻撃」にもマッチする
            var result = md.SelectMessage("格闘");
            Assert.AreEqual("攻撃メッセージ", result);
        }

        [TestMethod]
        public void MessageData_SelectMessage_NoMatch_ReturnsEmpty()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("格闘", "格闘メッセージ");

            var result = md.SelectMessage("回避");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void MessageData_InitialState_ItemsIsEmpty()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            Assert.AreEqual(0, md.Items.Count);
        }

        [TestMethod]
        public void MessageData_Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var md = new MessageData(src) { Name = "アムロ" };
            Assert.AreEqual("アムロ", md.Name);
        }
    }
}
