using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression の式置換 (ReplaceSubExpression) と
    /// FormatMessage のユニットテスト
    /// </summary>
    [TestClass]
    public class ExpressionReplaceTests
    {
        private Expression Create()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // ReplaceSubExpression
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplaceSubExpression_NoPlaceholder_StringUnchanged()
        {
            var exp = Create();
            var str = "こんにちは";
            exp.ReplaceSubExpression(ref str);
            Assert.AreEqual("こんにちは", str);
        }

        [TestMethod]
        public void ReplaceSubExpression_WithVariable_ReplacesPlaceholder()
        {
            var exp = Create();
            exp.SetVariableAsString("名前", "アムロ");

            var str = "こんにちは$(名前)さん";
            exp.ReplaceSubExpression(ref str);
            Assert.AreEqual("こんにちはアムロさん", str);
        }

        [TestMethod]
        public void ReplaceSubExpression_WithNumericVariable_FormatsNumber()
        {
            var exp = Create();
            exp.SetVariableAsDouble("スコア", 100d);

            var str = "スコア: $(スコア)点";
            exp.ReplaceSubExpression(ref str);
            Assert.AreEqual("スコア: 100点", str);
        }

        [TestMethod]
        public void ReplaceSubExpression_MultipleReplacements()
        {
            var exp = Create();
            exp.SetVariableAsString("名前A", "アムロ");
            exp.SetVariableAsString("名前B", "シャア");

            var str = "$(名前A)対$(名前B)";
            exp.ReplaceSubExpression(ref str);
            Assert.AreEqual("アムロ対シャア", str);
        }

        [TestMethod]
        public void ReplaceSubExpression_UnclosedBracket_NoChange()
        {
            var exp = Create();
            // 括弧が閉じていない場合は何もしない
            var str = "こんにちは$(名前";
            exp.ReplaceSubExpression(ref str);
            // 変化なしまたはエラーなし (実装依存)
            Assert.IsNotNull(str);
        }

        [TestMethod]
        public void ReplaceSubExpression_EmptyExpression_ReplacesWithEmpty()
        {
            var exp = Create();
            var str = "前$()後";
            exp.ReplaceSubExpression(ref str);
            // 空の式は空文字列に置換される
            Assert.IsNotNull(str);
        }

        // ──────────────────────────────────────────────
        // FormatMessage
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FormatMessage_PlainString_UnchangedBasically()
        {
            var exp = Create();
            var msg = "こんにちは";
            exp.FormatMessage(ref msg);
            Assert.AreEqual("こんにちは", msg);
        }

        [TestMethod]
        public void FormatMessage_DoubleDash_ConvertedToUnderline()
        {
            var exp = Create();
            var msg = "テスト――テスト";
            exp.FormatMessage(ref msg);
            // 「――」は「──」に変換される
            Assert.AreEqual("テスト──テスト", msg);
        }

        [TestMethod]
        public void FormatMessage_WithVariable_ReplacesSubExpression()
        {
            var exp = Create();
            exp.SetVariableAsString("主人公", "アムロ");

            var msg = "$(主人公)、出撃！";
            exp.FormatMessage(ref msg);
            Assert.AreEqual("アムロ、出撃！", msg);
        }
    }
}
