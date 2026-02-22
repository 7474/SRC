using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// SituationExtension.SelectMessage の追加テスト（その3）
    /// </summary>
    [TestClass]
    public class SituationExtensionMoreTests3
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
        // 空リスト → null
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_EmptyList_ReturnsNull()
        {
            var src = CreateSRC();
            var items = new List<MessageDataItem>();
            var result = items.SelectMessage(src, "格闘");
            Assert.IsNull(result);
        }

        // ──────────────────────────────────────────────
        // 完全一致
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_ExactMatch_格闘_ReturnsMessage()
        {
            var src = CreateSRC();
            var items = CreateItems(("格闘", "格闘メッセージ"));
            var result = items.SelectMessage(src, "格闘");
            Assert.IsNotNull(result);
            Assert.AreEqual("格闘メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_ExactMatch_射撃_ReturnsMessage()
        {
            var src = CreateSRC();
            var items = CreateItems(("射撃", "射撃メッセージ"));
            var result = items.SelectMessage(src, "射撃");
            Assert.IsNotNull(result);
            Assert.AreEqual("射撃メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // 格闘 / 射撃 → 攻撃 フォールバック
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘_FallsBackTo攻撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃", "攻撃メッセージ"));
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
        // とどめフォールバック
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘とどめ_FallsBackTo攻撃とどめ()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(とどめ)", "攻撃とどめメッセージ"));
            var result = items.SelectMessage(src, "格闘(とどめ)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃とどめメッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_射撃とどめ_FallsBackTo攻撃とどめ()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(とどめ)", "攻撃とどめメッセージ"));
            var result = items.SelectMessage(src, "射撃(とどめ)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃とどめメッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // クリティカルフォールバック
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘クリティカル_FallsBackTo攻撃クリティカル()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(クリティカル)", "クリティカルメッセージ"));
            var result = items.SelectMessage(src, "格闘(クリティカル)");
            Assert.IsNotNull(result);
            Assert.AreEqual("クリティカルメッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // シチュエーション不一致 → null
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_NoMatchingSituation_ReturnsNull()
        {
            var src = CreateSRC();
            var items = CreateItems(("格闘", "格闘"));
            var result = items.SelectMessage(src, "回避");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SelectMessage_反撃_ExactMatch()
        {
            var src = CreateSRC();
            var items = CreateItems(("格闘(反撃)", "反撃メッセージ"));
            var result = items.SelectMessage(src, "格闘(反撃)");
            Assert.IsNotNull(result);
            Assert.AreEqual("反撃メッセージ", result.Message);
        }
    }
}
