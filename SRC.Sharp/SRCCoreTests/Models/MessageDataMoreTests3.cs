using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class MessageDataMoreTests3
    {
        private SRC CreateSRC() => new SRC { GUI = new MockGUI() };

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.Name = "ガンダム";
            Assert.AreEqual("ガンダム", md.Name);
        }

        [TestMethod]
        public void Name_DefaultIsNull()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            Assert.IsNull(md.Name);
        }

        [TestMethod]
        public void Items_Initially_IsEmpty()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            Assert.AreEqual(0, md.Items.Count);
        }

        [TestMethod]
        public void AddMessage_IncreasesItemsCount()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("格闘", "いくぞ！");
            Assert.AreEqual(1, md.Items.Count);
        }

        [TestMethod]
        public void AddMessage_MultipleItems_CountIsCorrect()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("格闘", "msg1");
            md.AddMessage("射撃", "msg2");
            md.AddMessage("回避", "msg3");
            Assert.AreEqual(3, md.Items.Count);
        }

        [TestMethod]
        public void AddMessage_StoresSituation()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("移動", "moving");
            Assert.AreEqual("移動", md.Items[0].Situation);
        }

        [TestMethod]
        public void AddMessage_StoresMessage()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("攻撃", "攻撃メッセージ");
            Assert.AreEqual("攻撃メッセージ", md.Items[0].Message);
        }

        [TestMethod]
        public void SelectMessage_MatchingSituation_ReturnsMessage()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("格闘", "斬る！");
            var result = md.SelectMessage("格闘");
            Assert.AreEqual("斬る！", result);
        }

        [TestMethod]
        public void SelectMessage_NoMatchingSituation_ReturnsEmpty()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("格闘", "斬る！");
            var result = md.SelectMessage("射撃");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void Name_CanBeChanged()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.Name = "ザク";
            md.Name = "グフ";
            Assert.AreEqual("グフ", md.Name);
        }

        [TestMethod]
        public void AddMessage_EmptyItems_SelectMessage_ReturnsEmpty()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            var result = md.SelectMessage("何でも");
            Assert.AreEqual("", result);
        }
    }
}
