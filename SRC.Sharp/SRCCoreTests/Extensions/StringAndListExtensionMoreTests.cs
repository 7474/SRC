using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// StringExtension および ListExtension のエッジケース追加テスト
    /// </summary>
    [TestClass]
    public class StringAndListExtensionMoreTests
    {
        // ──────────────────────────────────────────────
        // StringExtension: ArrayIndexByName (エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ArrayIndexByName_SpaceInIndex_ReturnsSpaceIndex()
        {
            // インデックスにスペースが含まれる場合
            Assert.AreEqual("a b", "var[a b]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_NumbersAndLetters_ReturnsCorrectIndex()
        {
            Assert.AreEqual("abc123", "data[abc123]".ArrayIndexByName());
        }

        [TestMethod]
        public void ArrayIndexByName_SingleChar_ReturnsChar()
        {
            Assert.AreEqual("x", "v[x]".ArrayIndexByName());
        }

        // ──────────────────────────────────────────────
        // StringExtension: InsideKakko (エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InsideKakko_NumberArgument_ReturnsNumber()
        {
            Assert.AreEqual("123", "func(123)".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_MultipleArgs_ReturnsAll()
        {
            // InsideKakko は最初の () の内容を返す
            Assert.AreEqual("a,b,c", "f(a,b,c)".InsideKakko());
        }

        [TestMethod]
        public void InsideKakko_NoParens_ReturnsEmpty()
        {
            Assert.AreEqual("", "noParen".InsideKakko());
        }

        // ──────────────────────────────────────────────
        // StringExtension: ReplaceNewLine (エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplaceNewLine_EmptyString_ReturnsEmpty()
        {
            Assert.AreEqual("", "".ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_NoNewLines_ReturnsOriginal()
        {
            Assert.AreEqual("abc", "abc".ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_MultipleConsecutiveCRLF_ReplacesAll()
        {
            Assert.AreEqual("a  b", "a\r\n\r\nb".ReplaceNewLine(" "));
        }

        [TestMethod]
        public void ReplaceNewLine_WithEmptyReplacement_RemovesNewlines()
        {
            Assert.AreEqual("ab", "a\nb".ReplaceNewLine(""));
        }

        // ──────────────────────────────────────────────
        // StringExtension: RemoveLineComment (エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveLineComment_CodeBeforeComment_ReturnsCodePart()
        {
            Assert.AreEqual("Set x 1 ", "Set x 1 // コメント".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_DoubleQuoteBeforeComment_KeepsComment()
        {
            // ダブルクォートが閉じられていない場合は // をコメントとして扱わない
            Assert.AreEqual("\"// hoge", "\"// hoge".RemoveLineComment());
        }

        [TestMethod]
        public void RemoveLineComment_MultipleSlashesOnly_ReturnsEmpty()
        {
            Assert.AreEqual("", "///comment".RemoveLineComment());
        }

        // ──────────────────────────────────────────────
        // ListExtension: CloneList (エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CloneList_EmptyList_ReturnsEmptyList()
        {
            IList<int> list = new List<int>();
            var clone = list.CloneList();
            Assert.IsNotNull(clone);
            Assert.AreEqual(0, clone.Count);
        }

        [TestMethod]
        public void CloneList_ModifyClone_DoesNotAffectOriginal()
        {
            IList<string> original = new List<string> { "a", "b", "c" };
            var clone = original.CloneList();
            clone.Add("d");
            Assert.AreEqual(3, original.Count);
            Assert.AreEqual(4, clone.Count);
        }

        [TestMethod]
        public void CloneList_SingleElement_ReturnsEquivalent()
        {
            IList<int> list = new List<int> { 42 };
            var clone = list.CloneList();
            Assert.AreEqual(1, clone.Count);
            Assert.AreEqual(42, clone[0]);
        }

        // ──────────────────────────────────────────────
        // ListExtension: SafeRefOneOffset (エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SafeRefOneOffset_NegativeIndex_ReturnsDefault()
        {
            var list = new List<string> { "a", "b" };
            Assert.IsNull(list.SafeRefOneOffset(-1));
        }

        [TestMethod]
        public void SafeRefOneOffset_ExactLastIndex_ReturnsElement()
        {
            var list = new List<int> { 10, 20, 30 };
            Assert.AreEqual(30, list.SafeRefOneOffset(3));
        }

        [TestMethod]
        public void SafeRefOneOffset_SingleElementList_Index1_ReturnsElement()
        {
            var list = new List<int> { 99 };
            Assert.AreEqual(99, list.SafeRefOneOffset(1));
        }

        // ──────────────────────────────────────────────
        // ListExtension: SafeRefZeroOffset (エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SafeRefZeroOffset_ExactLastIndex_ReturnsElement()
        {
            var list = new List<int> { 10, 20, 30 };
            Assert.AreEqual(30, list.SafeRefZeroOffset(2));
        }

        [TestMethod]
        public void SafeRefZeroOffset_LargeIndex_ReturnsDefault()
        {
            var list = new List<string> { "x" };
            Assert.IsNull(list.SafeRefZeroOffset(100));
        }

        [TestMethod]
        public void SafeRefZeroOffset_SingleElement_Index0_ReturnsElement()
        {
            var list = new List<string> { "only" };
            Assert.AreEqual("only", list.SafeRefZeroOffset(0));
        }

        // ──────────────────────────────────────────────
        // ListExtension: RemoveItem (エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RemoveItem_EmptyList_DoesNotThrow()
        {
            IList<int> list = new List<int>();
            list.RemoveItem(x => x > 0);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void RemoveItem_RemoveSingleMatch_LeavesRest()
        {
            IList<string> list = new List<string> { "apple", "banana", "cherry" };
            list.RemoveItem(x => x == "banana");
            CollectionAssert.AreEqual(new[] { "apple", "cherry" }, list.ToArray());
        }

        // ──────────────────────────────────────────────
        // ListExtension: AppendRange (エッジケース)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AppendRange_NullAppend_EmptyAppend_ReturnsOriginal()
        {
            IEnumerable<int> list = new List<int> { 1, 2, 3 };
            var result = list.AppendRange(new List<int>()).ToList();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, result);
        }

        [TestMethod]
        public void AppendRange_BothNonEmpty_CombinesCorrectly()
        {
            IEnumerable<string> a = new[] { "x" };
            IEnumerable<string> b = new[] { "y", "z" };
            var result = a.AppendRange(b).ToList();
            CollectionAssert.AreEqual(new[] { "x", "y", "z" }, result);
        }
    }
}
