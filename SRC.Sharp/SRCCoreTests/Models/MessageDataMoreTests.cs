using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// MessageData / MessageDataList の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class MessageDataMoreTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // MessageDataList 基本 CRUD
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageDataList_Add_IncreasesCount()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            Assert.AreEqual(0, mdl.Count());

            mdl.Add("アムロ");
            Assert.AreEqual(1, mdl.Count());
        }

        [TestMethod]
        public void MessageDataList_Add_ReturnsMessageDataWithCorrectName()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            var md = mdl.Add("シャア");
            Assert.AreEqual("シャア", md.Name);
        }

        [TestMethod]
        public void MessageDataList_Add_Multiple_IncreasesCount()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            mdl.Add("アムロ");
            mdl.Add("シャア");
            mdl.Add("カミーユ");
            Assert.AreEqual(3, mdl.Count());
        }

        [TestMethod]
        public void MessageDataList_Delete_DecreasesCount()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            mdl.Add("アムロ");
            mdl.Add("シャア");
            mdl.Delete("アムロ");
            Assert.AreEqual(1, mdl.Count());
        }

        [TestMethod]
        public void MessageDataList_Delete_RemovedItemIsNoLongerAccessible()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            mdl.Add("アムロ");
            mdl.Delete("アムロ");
            Assert.IsNull(mdl.Item("アムロ"));
        }

        // ──────────────────────────────────────────────
        // Item / IsDefined
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageDataList_Item_ReturnsCorrectData()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            mdl.Add("テスト");
            var md = mdl.Item("テスト");
            Assert.IsNotNull(md);
            Assert.AreEqual("テスト", md.Name);
        }

        [TestMethod]
        public void MessageDataList_Item_NonExisting_ReturnsNull()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            Assert.IsNull(mdl.Item("存在しない"));
        }

        [TestMethod]
        public void MessageDataList_IsDefined_ExistingKey_ReturnsTrue()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            mdl.Add("ガンダム");
            Assert.IsTrue(mdl.IsDefined("ガンダム"));
        }

        [TestMethod]
        public void MessageDataList_IsDefined_NonExistingKey_ReturnsFalse()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            Assert.IsFalse(mdl.IsDefined("ガンダム"));
        }

        [TestMethod]
        public void MessageDataList_IsDefined_AfterDelete_ReturnsFalse()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            mdl.Add("ガンダム");
            mdl.Delete("ガンダム");
            Assert.IsFalse(mdl.IsDefined("ガンダム"));
        }

        // ──────────────────────────────────────────────
        // Items リスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageDataList_Items_ReflectsAddedEntries()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            mdl.Add("アムロ");
            mdl.Add("シャア");
            Assert.AreEqual(2, mdl.Items.Count);
        }

        [TestMethod]
        public void MessageDataList_InitialCount_IsZero()
        {
            var src = CreateSRC();
            var mdl = new MessageDataList(src);
            Assert.AreEqual(0, mdl.Count());
        }

        // ──────────────────────────────────────────────
        // MessageData.SelectMessage 各シチュエーション
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MessageData_SelectMessage_撃墜_ReturnsCorrectMessage()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("撃墜", "やられた！");
            Assert.AreEqual("やられた！", md.SelectMessage("撃墜"));
        }

        [TestMethod]
        public void MessageData_SelectMessage_複数_マッチしたものを返す()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("格闘", "格闘！");
            md.AddMessage("回避", "かわした！");
            md.AddMessage("撃墜", "やられた！");

            Assert.AreEqual("かわした！", md.SelectMessage("回避"));
            Assert.AreEqual("やられた！", md.SelectMessage("撃墜"));
        }

        [TestMethod]
        public void MessageData_SelectMessage_存在しないシチュエーション_空文字を返す()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            md.AddMessage("格闘", "格闘！");
            var result = md.SelectMessage("特殊シチュエーション");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void MessageData_SelectMessage_Items空_空文字を返す()
        {
            var src = CreateSRC();
            var md = new MessageData(src);
            Assert.AreEqual("", md.SelectMessage("格闘"));
        }
    }
}
