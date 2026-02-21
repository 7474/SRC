using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using System.Collections.Generic;

namespace SRCCore.Tests
{
    /// <summary>
    /// ListBoxItem / ListBoxItem&lt;T&gt; / ListBoxArgs クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class ListBoxItemTests
    {
        // ──────────────────────────────────────────────
        // ListBoxItem コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefaultConstructor_DefaultValues()
        {
            var item = new ListBoxItem();
            Assert.IsNull(item.Text);
            Assert.IsNull(item.ListItemID);
            Assert.IsFalse(item.ListItemFlag);
            Assert.IsNull(item.ListItemComment);
        }

        [TestMethod]
        public void Constructor_WithText_SetsTextAndID()
        {
            var item = new ListBoxItem("Option1");
            Assert.AreEqual("Option1", item.Text);
            Assert.AreEqual("Option1", item.ListItemID);
        }

        [TestMethod]
        public void Constructor_WithTextAndId_SetsBoth()
        {
            var item = new ListBoxItem("表示テキスト", "idValue");
            Assert.AreEqual("表示テキスト", item.Text);
            Assert.AreEqual("idValue", item.ListItemID);
        }

        // ──────────────────────────────────────────────
        // TextWithFlag プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TextWithFlag_FlagFalse_HasTwoSpaces()
        {
            var item = new ListBoxItem("テスト") { ListItemFlag = false };
            Assert.AreEqual("  テスト", item.TextWithFlag);
        }

        [TestMethod]
        public void TextWithFlag_FlagTrue_HasX()
        {
            var item = new ListBoxItem("テスト") { ListItemFlag = true };
            Assert.AreEqual("×テスト", item.TextWithFlag);
        }

        [TestMethod]
        public void ListItemComment_CanBeSetAndRead()
        {
            var item = new ListBoxItem("テスト") { ListItemComment = "コメントです" };
            Assert.AreEqual("コメントです", item.ListItemComment);
        }

        // ──────────────────────────────────────────────
        // ListBoxItem<T>
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GenericListBoxItem_DefaultConstructor_DefaultValues()
        {
            var item = new ListBoxItem<int>();
            Assert.IsNull(item.Text);
            Assert.AreEqual(0, item.ListItemObject);
        }

        [TestMethod]
        public void GenericListBoxItem_Constructor_WithText_SetsText()
        {
            var item = new ListBoxItem<string>("選択肢A");
            Assert.AreEqual("選択肢A", item.Text);
        }

        [TestMethod]
        public void GenericListBoxItem_ListItemObject_CanBeSetAndRead()
        {
            var item = new ListBoxItem<int>("選択肢") { ListItemObject = 42 };
            Assert.AreEqual(42, item.ListItemObject);
        }

        [TestMethod]
        public void GenericListBoxItem_ListItemObject_CustomClass()
        {
            var obj = new { Name = "テスト" };
            var item = new ListBoxItem<object>("アイテム") { ListItemObject = obj };
            Assert.AreEqual(obj, item.ListItemObject);
        }

        // ──────────────────────────────────────────────
        // ListBoxArgs
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ListBoxArgs_DefaultItems_IsEmptyList()
        {
            var args = new ListBoxArgs();
            Assert.IsNotNull(args.Items);
            Assert.AreEqual(0, args.Items.Count);
        }

        [TestMethod]
        public void ListBoxArgs_HasFlag_DefaultFalse()
        {
            var args = new ListBoxArgs();
            Assert.IsFalse(args.HasFlag);
        }

        [TestMethod]
        public void ListBoxArgs_Items_CanAddItems()
        {
            var args = new ListBoxArgs();
            args.Items.Add(new ListBoxItem("item1"));
            args.Items.Add(new ListBoxItem("item2"));
            Assert.AreEqual(2, args.Items.Count);
        }

        [TestMethod]
        public void ListBoxArgs_LbMode_DefaultIsEmpty()
        {
            var args = new ListBoxArgs();
            Assert.AreEqual("", args.lb_mode);
        }
    }
}
