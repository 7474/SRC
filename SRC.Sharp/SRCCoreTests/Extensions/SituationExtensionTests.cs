using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// SituationExtension.SelectMessage のユニットテスト
    /// </summary>
    [TestClass]
    public class SituationExtensionTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private List<MessageDataItem> CreateItems(params (string situation, string message)[] entries)
        {
            var list = new List<MessageDataItem>();
            foreach (var (s, m) in entries)
            {
                list.Add(new MessageDataItem(s, m));
            }
            return list;
        }

        // ──────────────────────────────────────────────
        // 完全一致
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_ExactMatch_ReturnsMatchingItem()
        {
            var src = CreateSRC();
            var items = CreateItems(
                ("攻撃", "攻撃メッセージ"),
                ("回避", "回避メッセージ")
            );

            var result = items.SelectMessage(src, "回避");
            Assert.IsNotNull(result);
            Assert.AreEqual("回避メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_NoMatch_ReturnsNull()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃", "攻撃メッセージ"));

            var result = items.SelectMessage(src, "修理");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SelectMessage_EmptyList_ReturnsNull()
        {
            var src = CreateSRC();
            var items = new List<MessageDataItem>();

            var result = items.SelectMessage(src, "攻撃");
            Assert.IsNull(result);
        }

        // ──────────────────────────────────────────────
        // 格闘・射撃 → 攻撃 へのフォールバック
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘_MatchesExact()
        {
            var src = CreateSRC();
            var items = CreateItems(("格闘", "格闘メッセージ"));

            var result = items.SelectMessage(src, "格闘");
            Assert.IsNotNull(result);
            Assert.AreEqual("格闘メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_格闘_FallsBackTo攻撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃", "攻撃メッセージ"));

            // 格闘で一致がなければ攻撃にフォールバック
            var result = items.SelectMessage(src, "格闘");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_射撃_FallsBackTo攻撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃", "攻撃メッセージ"));

            var result = items.SelectMessage(src, "射撃");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // IsAvailable フィルタ (u=null では true)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_NullUnit_ItemsAlwaysAvailable()
        {
            var src = CreateSRC();
            var items = CreateItems(
                ("攻撃", "攻撃メッセージ1"),
                ("攻撃", "攻撃メッセージ2")
            );

            var result = items.SelectMessage(src, "攻撃", u: null);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Message.StartsWith("攻撃メッセージ"));
        }
    }
}
