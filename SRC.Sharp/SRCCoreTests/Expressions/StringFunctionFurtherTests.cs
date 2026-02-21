using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression のString関連関数の追加テスト (Mid, Left, Right, Len, InStr 等)
    /// </summary>
    [TestClass]
    public class StringFunctionFurtherTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // Mid 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Mid_BasicUsage_ReturnsSubstring()
        {
            var exp = Create();
            Assert.AreEqual("ell", exp.GetValueAsString("Mid(\"hello\",2,3)"));
        }

        [TestMethod]
        public void Mid_FromBeginning_ReturnsFromFirst()
        {
            var exp = Create();
            Assert.AreEqual("he", exp.GetValueAsString("Mid(\"hello\",1,2)"));
        }

        [TestMethod]
        public void Mid_BeyondLength_ReturnsRemainder()
        {
            var exp = Create();
            Assert.AreEqual("lo", exp.GetValueAsString("Mid(\"hello\",4,10)"));
        }

        [TestMethod]
        public void Mid_TwoArgs_ReturnsFromPosition()
        {
            var exp = Create();
            Assert.AreEqual("ello", exp.GetValueAsString("Mid(\"hello\",2)"));
        }

        // ──────────────────────────────────────────────
        // Left 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Left_BasicUsage_ReturnsPrefix()
        {
            var exp = Create();
            Assert.AreEqual("hel", exp.GetValueAsString("Left(\"hello\",3)"));
        }

        [TestMethod]
        public void Left_ZeroLength_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Left(\"hello\",0)"));
        }

        [TestMethod]
        public void Left_LargerThanString_ReturnsFullString()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Left(\"hello\",10)"));
        }

        // ──────────────────────────────────────────────
        // Right 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Right_BasicUsage_ReturnsSuffix()
        {
            var exp = Create();
            Assert.AreEqual("llo", exp.GetValueAsString("Right(\"hello\",3)"));
        }

        [TestMethod]
        public void Right_ZeroLength_ReturnsEmpty()
        {
            var exp = Create();
            Assert.AreEqual("", exp.GetValueAsString("Right(\"hello\",0)"));
        }

        [TestMethod]
        public void Right_LargerThanString_ReturnsFullString()
        {
            var exp = Create();
            Assert.AreEqual("hello", exp.GetValueAsString("Right(\"hello\",10)"));
        }

        [TestMethod]
        public void Right_OneChar_ReturnsLastChar()
        {
            var exp = Create();
            Assert.AreEqual("o", exp.GetValueAsString("Right(\"hello\",1)"));
        }

        // ──────────────────────────────────────────────
        // Len 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_BasicUsage_ReturnsLength()
        {
            var exp = Create();
            Assert.AreEqual(5d, exp.GetValueAsDouble("Len(\"hello\")"));
        }

        [TestMethod]
        public void Len_EmptyString_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("Len(\"\")"));
        }

        [TestMethod]
        public void Len_JapaneseString_ReturnsCharCount()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("Len(\"あいう\")"));
        }

        // ──────────────────────────────────────────────
        // InStr 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr_Found_ReturnsPosition()
        {
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("InStr(\"hello\",\"ll\")"));
        }

        [TestMethod]
        public void InStr_NotFound_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("InStr(\"hello\",\"xyz\")"));
        }

        [TestMethod]
        public void InStr_EmptySearchString_ReturnsOne()
        {
            var exp = Create();
            Assert.AreEqual(1d, exp.GetValueAsDouble("InStr(\"hello\",\"\")"));
        }

        // ──────────────────────────────────────────────
        // StrComp 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrComp_EqualStrings_ReturnsZero()
        {
            var exp = Create();
            Assert.AreEqual(0d, exp.GetValueAsDouble("StrComp(\"abc\",\"abc\")"));
        }

        [TestMethod]
        public void StrComp_LessString_ReturnsNegative()
        {
            var exp = Create();
            Assert.IsTrue(exp.GetValueAsDouble("StrComp(\"abc\",\"def\")") < 0);
        }

        [TestMethod]
        public void StrComp_GreaterString_ReturnsPositive()
        {
            var exp = Create();
            Assert.IsTrue(exp.GetValueAsDouble("StrComp(\"def\",\"abc\")") > 0);
        }
    }
}
