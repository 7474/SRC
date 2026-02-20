using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    [TestClass()]
    public class StringsTests
    {
        // ──────────────────────────────────────────────
        // Asc
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void AscTest_AsciiChar()
        {
            Assert.AreEqual(65, Strings.Asc('A'));
            Assert.AreEqual(97, Strings.Asc('a'));
            Assert.AreEqual(48, Strings.Asc('0'));
        }

        [TestMethod()]
        public void AscTest_String()
        {
            Assert.AreEqual(65, Strings.Asc("ABC"));  // 先頭文字 'A'
            Assert.AreEqual(49, Strings.Asc("123"));  // 先頭文字 '1'
        }

        // ──────────────────────────────────────────────
        // LCase
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void LCaseTest_String()
        {
            Assert.AreEqual("hello", Strings.LCase("HELLO"));
            Assert.AreEqual("hello world", Strings.LCase("Hello World"));
            Assert.AreEqual("abc123", Strings.LCase("ABC123"));
        }

        [TestMethod()]
        public void LCaseTest_Char()
        {
            Assert.AreEqual('a', Strings.LCase('A'));
            Assert.AreEqual('z', Strings.LCase('Z'));
            Assert.AreEqual('1', Strings.LCase('1')); // 数字はそのまま
        }

        // ──────────────────────────────────────────────
        // Len
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void LenTest()
        {
            Assert.AreEqual(0, Strings.Len(null));
            Assert.AreEqual(0, Strings.Len(""));
            Assert.AreEqual(5, Strings.Len("hello"));
            Assert.AreEqual(3, Strings.Len("あいう")); // 文字数（バイト数でなく）
        }

        // ──────────────────────────────────────────────
        // Left
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void LeftTest_BasicUsage()
        {
            Assert.AreEqual("hel", Strings.Left("hello", 3));
            Assert.AreEqual("hello", Strings.Left("hello", 5));
            Assert.AreEqual("hello", Strings.Left("hello", 10)); // 文字列より長い
        }

        [TestMethod()]
        public void LeftTest_ZeroLength()
        {
            Assert.AreEqual("", Strings.Left("hello", 0));
        }

        [TestMethod()]
        public void LeftTest_EmptyString()
        {
            Assert.AreEqual("", Strings.Left("", 3));
            Assert.AreEqual("", Strings.Left(null, 3));
        }

        // ──────────────────────────────────────────────
        // Right
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void RightTest_BasicUsage()
        {
            Assert.AreEqual("llo", Strings.Right("hello", 3));
            Assert.AreEqual("hello", Strings.Right("hello", 5));
            Assert.AreEqual("hello", Strings.Right("hello", 10)); // 文字列より長い
        }

        [TestMethod()]
        public void RightTest_ZeroLength()
        {
            Assert.AreEqual("", Strings.Right("hello", 0));
        }

        [TestMethod()]
        public void RightTest_EmptyString()
        {
            Assert.AreEqual("", Strings.Right("", 3));
            Assert.AreEqual("", Strings.Right(null, 3));
        }

        // ──────────────────────────────────────────────
        // Mid
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void MidTest_WithLengthArg()
        {
            Assert.AreEqual("ell", Strings.Mid("hello", 2, 3));
            Assert.AreEqual("h", Strings.Mid("hello", 1, 1));
            Assert.AreEqual("hello", Strings.Mid("hello", 1, 10)); // 文字列より長い
        }

        [TestMethod()]
        public void MidTest_WithoutLengthArg()
        {
            Assert.AreEqual("ello", Strings.Mid("hello", 2));
            Assert.AreEqual("hello", Strings.Mid("hello", 1));
        }

        [TestMethod()]
        public void MidTest_EmptyString()
        {
            Assert.AreEqual("", Strings.Mid("", 1));
            Assert.AreEqual("", Strings.Mid(null, 1));
        }

        [TestMethod()]
        public void MidTest_ZeroLength()
        {
            Assert.AreEqual("", Strings.Mid("hello", 1, 0));
        }

        [TestMethod()]
        public void MidTest_StartBeyondEnd()
        {
            Assert.AreEqual("", Strings.Mid("hello", 10));
        }

        // ──────────────────────────────────────────────
        // InStr
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void InStrTest_Found()
        {
            Assert.AreEqual(2, Strings.InStr("hello", "el")); // 2文字目から一致
            Assert.AreEqual(1, Strings.InStr("hello", "h"));  // 先頭
            Assert.AreEqual(5, Strings.InStr("hello", "o"));  // 末尾
        }

        [TestMethod()]
        public void InStrTest_NotFound()
        {
            Assert.AreEqual(0, Strings.InStr("hello", "xyz"));
        }

        [TestMethod()]
        public void InStrTest_WithStartPosition()
        {
            Assert.AreEqual(3, Strings.InStr(3, "hello", "l")); // 3文字目以降から検索 → 3文字目の'l'
            Assert.AreEqual(4, Strings.InStr(4, "hello", "l")); // 4文字目以降から検索 → 4文字目の'l'
            Assert.AreEqual(0, Strings.InStr(5, "hello", "l")); // 5文字目以降には'l'がない
        }

        [TestMethod()]
        public void InStrTest_EmptyStrings()
        {
            Assert.AreEqual(1, Strings.InStr("hello", "")); // 空文字列は位置1で一致
            Assert.AreEqual(0, Strings.InStr("", "hello"));
            Assert.AreEqual(0, Strings.InStr(null, "hello"));
        }

        // ──────────────────────────────────────────────
        // Space
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void SpaceTest()
        {
            Assert.AreEqual("   ", Strings.Space(3));
            Assert.AreEqual("", Strings.Space(0));
            Assert.AreEqual(" ", Strings.Space(1));
        }

        // ──────────────────────────────────────────────
        // StrDup
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void StrDupTest_SingleChar()
        {
            Assert.AreEqual("aaa", Strings.StrDup("a", 3));
            Assert.AreEqual("a", Strings.StrDup("a", 1));
            Assert.AreEqual("", Strings.StrDup("a", 0));
        }

        [TestMethod()]
        public void StrDupTest_MultiChar()
        {
            Assert.AreEqual("abcabcabc", Strings.StrDup("abc", 3));
        }

        [TestMethod()]
        public void StrDupTest_EmptyString()
        {
            Assert.AreEqual("", Strings.StrDup("", 3));
            Assert.AreEqual("", Strings.StrDup(null, 3));
        }

        // ──────────────────────────────────────────────
        // StrComp
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void StrCompTest_EqualStrings()
        {
            Assert.AreEqual(0, Strings.StrComp("hello", "hello"));
        }

        [TestMethod()]
        public void StrCompTest_LessThan()
        {
            Assert.IsTrue(Strings.StrComp("abc", "abd") < 0);
        }

        [TestMethod()]
        public void StrCompTest_GreaterThan()
        {
            Assert.IsTrue(Strings.StrComp("abd", "abc") > 0);
        }

        // ──────────────────────────────────────────────
        // Trim
        // ──────────────────────────────────────────────

        [TestMethod()]
        public void TrimTest()
        {
            Assert.AreEqual("hello", Strings.Trim("  hello  "));
            Assert.AreEqual("hello", Strings.Trim("hello"));
            Assert.AreEqual("", Strings.Trim("   "));
            Assert.AreEqual("", Strings.Trim(""));
            Assert.AreEqual("", Strings.Trim(null));
        }
    }
}
