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

        // ──────────────────────────────────────────────
        // ユニット限定のサブシチュエーション（#555）
        // ──────────────────────────────────────────────

        private static Units.Unit CreateUnit(SRC src, string name, string unitClass)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.Class = unitClass;
            var u = src.UList.Add(name, 0, "味方");
            u.Update();
            return u;
        }

        [TestMethod]
        public void SelectMessage_UnitClassSubSituation_Matches()
        {
            // 元の SRC は全シチュエーション×全サブシチュエーションで照合する。
            // ユニット名以外（ここではユニットクラス）のサブシチュエーションでも一致する
            var src = CreateSRC();
            var u = CreateUnit(src, "テスト機", "テストクラス");
            var items = CreateItems(("回避(テストクラス)", "クラス限定の回避メッセージ"));

            var result = items.SelectMessage(src, "回避", u);
            Assert.IsNotNull(result);
            Assert.AreEqual("クラス限定の回避メッセージ", result.Message);
        }

        [TestMethod]
        public void SelectMessage_UnitNameSubSituation_Matches()
        {
            var src = CreateSRC();
            var u = CreateUnit(src, "テスト機", "テストクラス");
            var items = CreateItems(("回避(テスト機)", "機体限定の回避メッセージ"));

            var result = items.SelectMessage(src, "回避", u);
            Assert.IsNotNull(result);
            Assert.AreEqual("機体限定の回避メッセージ", result.Message);
        }
    }
}
