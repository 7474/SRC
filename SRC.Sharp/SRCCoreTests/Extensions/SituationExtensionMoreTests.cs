using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// SituationExtension.SelectMessage の追加テスト（各状況のフォールバック検証）
    /// </summary>
    [TestClass]
    public class SituationExtensionMoreTests
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
        // 格闘(命中) / 射撃(命中) → 攻撃(命中)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘命中_ExactMatch()
        {
            var src = CreateSRC();
            var items = CreateItems(("格闘(命中)", "格闘命中メッセージ"));
            var result = items.SelectMessage(src, "格闘(命中)");
            Assert.IsNotNull(result);
            Assert.AreEqual("格闘命中メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_格闘命中_FallsBackTo攻撃命中()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(命中)", "攻撃命中メッセージ"));
            var result = items.SelectMessage(src, "格闘(命中)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃命中メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_射撃命中_FallsBackTo攻撃命中()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(命中)", "攻撃命中メッセージ"));
            var result = items.SelectMessage(src, "射撃(命中)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃命中メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // 格闘(回避) / 射撃(回避) → 攻撃(回避)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘回避_FallsBackTo攻撃回避()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(回避)", "攻撃回避メッセージ"));
            var result = items.SelectMessage(src, "格闘(回避)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃回避メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_射撃回避_FallsBackTo攻撃回避()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(回避)", "攻撃回避メッセージ"));
            var result = items.SelectMessage(src, "射撃(回避)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃回避メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // 格闘(とどめ) / 射撃(とどめ) → 攻撃(とどめ)
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
        // 格闘(クリティカル) / 射撃(クリティカル) → 攻撃(クリティカル)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘クリティカル_FallsBackTo攻撃クリティカル()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(クリティカル)", "攻撃クリティカルメッセージ"));
            var result = items.SelectMessage(src, "格闘(クリティカル)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃クリティカルメッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_射撃クリティカル_FallsBackTo攻撃クリティカル()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(クリティカル)", "攻撃クリティカルメッセージ"));
            var result = items.SelectMessage(src, "射撃(クリティカル)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃クリティカルメッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // 格闘(反撃) / 射撃(反撃) → 攻撃(反撃)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘反撃_FallsBackTo攻撃反撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(反撃)", "攻撃反撃メッセージ"));
            var result = items.SelectMessage(src, "格闘(反撃)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃反撃メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_射撃反撃_FallsBackTo攻撃反撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(反撃)", "攻撃反撃メッセージ"));
            var result = items.SelectMessage(src, "射撃(反撃)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃反撃メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // 格闘(命中)(反撃) / 射撃(命中)(反撃) → 攻撃(命中)(反撃)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘命中反撃_FallsBackTo攻撃命中反撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(命中)(反撃)", "攻撃命中反撃メッセージ"));
            var result = items.SelectMessage(src, "格闘(命中)(反撃)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃命中反撃メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // 格闘(回避)(反撃) → 攻撃(回避)(反撃)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘回避反撃_FallsBackTo攻撃回避反撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(回避)(反撃)", "攻撃回避反撃メッセージ"));
            var result = items.SelectMessage(src, "格闘(回避)(反撃)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃回避反撃メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // 格闘(とどめ)(反撃) → 攻撃(とどめ)(反撃)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘とどめ反撃_FallsBackTo攻撃とどめ反撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(とどめ)(反撃)", "攻撃とどめ反撃メッセージ"));
            var result = items.SelectMessage(src, "格闘(とどめ)(反撃)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃とどめ反撃メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // 格闘(クリティカル)(反撃) → 攻撃(クリティカル)(反撃)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_格闘クリティカル反撃_FallsBackTo攻撃クリティカル反撃()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃(クリティカル)(反撃)", "攻撃クリティカル反撃メッセージ"));
            var result = items.SelectMessage(src, "格闘(クリティカル)(反撃)");
            Assert.IsNotNull(result);
            Assert.AreEqual("攻撃クリティカル反撃メッセージ", result.Message);
        }

        // ──────────────────────────────────────────────
        // デフォルト状況（上記いずれにも該当しない）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SelectMessage_NonCombatSituation_ReturnsExactMatch()
        {
            var src = CreateSRC();
            var items = CreateItems(("回避", "回避メッセージ"));
            var result = items.SelectMessage(src, "回避");
            Assert.IsNotNull(result);
            Assert.AreEqual("回避メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_UnknownSituation_NoFallback_ReturnsNull()
        {
            var src = CreateSRC();
            var items = CreateItems(("攻撃", "攻撃メッセージ"));
            // "未知" は格闘・射撃でないのでフォールバックしない
            var result = items.SelectMessage(src, "未知");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void SelectMessage_MultipleSituations_StartsWithReturnsAll()
        {
            var src = CreateSRC();
            var items = CreateItems(
                ("攻撃", "攻撃メッセージ"),
                ("格闘", "格闘メッセージ"),
                ("射撃", "射撃メッセージ")
            );

            // 格闘 は 格闘 または 攻撃 にフォールバック
            var result = items.SelectMessage(src, "格闘");
            Assert.IsNotNull(result);
        }
    }
}
