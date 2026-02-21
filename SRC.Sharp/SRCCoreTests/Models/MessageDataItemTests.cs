using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// MessageDataItem / MessageData クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class MessageDataItemTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // MessageDataItem コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageDataItem_Constructor_SetsSituationAndMessage()
        {
            var item = new MessageDataItem("攻撃", "ビームライフル、発射！");
            Assert.AreEqual("攻撃", item.Situation);
            Assert.AreEqual("ビームライフル、発射！", item.Message);
        }

        [TestMethod]
        public void MessageDataItem_IsAvailable_ReturnsTrue_WhenUnitIsNull()
        {
            var item = new MessageDataItem("回避", "かわしてみせる！");
            Assert.IsTrue(item.IsAvailable(null, false));
        }

        [TestMethod]
        public void MessageDataItem_IsAvailable_ReturnsTrue_WhenUnitIsNotNull()
        {
            var src = CreateSRC();
            var unit = new Units.Unit(src);
            var item = new MessageDataItem("格闘", "行くぞ！");
            Assert.IsTrue(item.IsAvailable(unit, false));
        }

        // ──────────────────────────────────────────────
        // MessageData.AddMessage
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageData_AddMessage_IncreasesItemCount()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            Assert.AreEqual(0, md.Items.Count);
            md.AddMessage("攻撃", "テストメッセージ");
            Assert.AreEqual(1, md.Items.Count);
        }

        [TestMethod]
        public void MessageData_AddMessage_Multiple_IncreasesCount()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("攻撃", "メッセージ1");
            md.AddMessage("回避", "メッセージ2");
            md.AddMessage("格闘", "メッセージ3");
            Assert.AreEqual(3, md.Items.Count);
        }

        [TestMethod]
        public void MessageData_AddMessage_SetsCorrectItemData()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("防御", "壁を作る！");
            Assert.AreEqual("防御", md.Items[0].Situation);
            Assert.AreEqual("壁を作る！", md.Items[0].Message);
        }

        // ──────────────────────────────────────────────
        // MessageData.SelectMessage
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageData_SelectMessage_ReturnsMatchingMessage()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("攻撃", "ビームサーベル！");
            var result = md.SelectMessage("攻撃");
            Assert.AreEqual("ビームサーベル！", result);
        }

        [TestMethod]
        public void MessageData_SelectMessage_NoMatch_ReturnsEmpty()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("攻撃", "メッセージ");
            var result = md.SelectMessage("回避");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void MessageData_SelectMessage_EmptyList_ReturnsEmpty()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            var result = md.SelectMessage("攻撃");
            Assert.AreEqual("", result);
        }

        // ──────────────────────────────────────────────
        // MessageData.Name フィールド
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageData_Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var md = new MessageData(src) { Name = "アムロ" };
            Assert.AreEqual("アムロ", md.Name);
        }
    }
}
