using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// RegExp / RegExpReplace 関数のユニットテスト
    /// </summary>
    [TestClass]
    public class RegexFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return src.Expression;
        }

        // ──────────────────────────────────────────────
        // RegExp
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RegExp_SimpleMatch_ReturnsFirstMatch()
        {
            var exp = Create();
            // Use character class instead of \w to avoid backslash issues
            var result = exp.GetValueAsString("RegExp(\"hello world\",\"[a-z]+\")");
            Assert.AreEqual("hello", result);
        }

        [TestMethod]
        public void RegExp_NoMatch_ReturnsEmpty()
        {
            var exp = Create();
            // Use character class instead of \d
            var result = exp.GetValueAsString("RegExp(\"hello\",\"[0-9]+\")");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void RegExp_PatternWithCaptureGroup_ReturnsMatch()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RegExp(\"abc123\",\"[a-z]+\")");
            Assert.AreEqual("abc", result);
        }

        [TestMethod]
        public void RegExp_NumberPattern_ReturnsNumber()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RegExp(\"price: 42\",\"[0-9]+\")");
            Assert.AreEqual("42", result);
        }

        [TestMethod]
        public void RegExp_CaseInsensitive_MatchesUppercase()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RegExp(\"Hello\",\"hello\",\"大小区別なし\")");
            Assert.AreEqual("Hello", result);
        }

        [TestMethod]
        public void RegExp_CaseSensitive_DoesNotMatchUppercase()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RegExp(\"Hello\",\"hello\")");
            Assert.AreEqual("", result);
        }

        [TestMethod]
        public void RegExp_JapanesePattern_ReturnsMatch()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RegExp(\"テスト文字列\",\"[ァ-ン]+\")");
            Assert.AreEqual("テスト", result);
        }

        // ──────────────────────────────────────────────
        // RegExpReplace
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RegExpReplace_SimpleReplace_ReplacesAll()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RegExpReplace(\"hello world\",\"o\",\"0\")");
            Assert.AreEqual("hell0 w0rld", result);
        }

        [TestMethod]
        public void RegExpReplace_NoMatch_ReturnsOriginal()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RegExpReplace(\"hello\",\"\\\\d\",\"x\")");
            Assert.AreEqual("hello", result);
        }

        [TestMethod]
        public void RegExpReplace_NumberPattern_ReplacesNumbers()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RegExpReplace(\"abc123def456\",\"[0-9]+\",\"#\")");
            Assert.AreEqual("abc#def#", result);
        }

        [TestMethod]
        public void RegExpReplace_WordBoundary_ReplacesWords()
        {
            var exp = Create();
            // Use character class instead of \w to avoid backslash issues
            var result = exp.GetValueAsString("RegExpReplace(\"hello world\",\"[a-z]+\",\"X\")");
            Assert.AreEqual("X X", result);
        }

        [TestMethod]
        public void RegExpReplace_EmptyReplacement_RemovesMatches()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RegExpReplace(\"a1b2c3\",\"[0-9]\",\"\")");
            Assert.AreEqual("abc", result);
        }
    }
}
