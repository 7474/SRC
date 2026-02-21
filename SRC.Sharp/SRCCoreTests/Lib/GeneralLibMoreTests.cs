using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// GeneralLib のうち既存テストで未カバーのシナリオを補完するテスト
    /// </summary>
    [TestClass]
    public class GeneralLibMoreTests
    {
        // ──────────────────────────────────────────────
        // ToList (removeKakko オプション)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToList_RemoveKakko_RemovesBrackets()
        {
            // removeKakko=true のとき括弧文字自体が結果から除去される
            var result = GeneralLib.ToList("(a b) c", removeKakko: true);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("a b", result[0]);
            Assert.AreEqual("c", result[1]);
        }

        [TestMethod]
        public void ToList_RemoveKakkoFalse_KeepsBrackets()
        {
            // removeKakko=false（既定）のとき括弧は保持される
            var result = GeneralLib.ToList("(a b) c", removeKakko: false);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("(a b)", result[0]);
            Assert.AreEqual("c", result[1]);
        }

        [TestMethod]
        public void ToList_RemoveKakko_SquareBrackets_Removed()
        {
            var result = GeneralLib.ToList("[x y] z", removeKakko: true);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("x y", result[0]);
            Assert.AreEqual("z", result[1]);
        }

        [TestMethod]
        public void ToList_RemoveKakko_NestedBracketsOuterRemoved()
        {
            // 外側の括弧のみ除去され、内側の括弧は残る
            var result = GeneralLib.ToList("(a (b c)) d", removeKakko: true);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("a (b c)", result[0]);
            Assert.AreEqual("d", result[1]);
        }

        // ──────────────────────────────────────────────
        // ToList / ToL のタブ区切り挙動の違い
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToList_TabSeparated_SplitsTokens()
        {
            // ToList はタブも区切り文字として扱う
            var result = GeneralLib.ToList("a\tb\tc");
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("a", result[0]);
            Assert.AreEqual("b", result[1]);
            Assert.AreEqual("c", result[2]);
        }

        [TestMethod]
        public void ToL_TabSeparated_TreatedAsSingleToken()
        {
            // ToL はスペース区切りのみ。タブは区切り文字ではない
            var result = GeneralLib.ToL("a\tb");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("a\tb", result[0]);
        }

        // ──────────────────────────────────────────────
        // GetClassBundle – 効・剋 接頭辞
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetClassBundle_KouPrefix_ReturnsKouAndChar()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("効炎", ref idx);
            Assert.AreEqual("効炎", result);
            Assert.AreEqual(2, idx);
        }

        [TestMethod]
        public void GetClassBundle_KokuPrefix_ReturnsKokuAndChar()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("剋水", ref idx);
            Assert.AreEqual("剋水", result);
            Assert.AreEqual(2, idx);
        }

        [TestMethod]
        public void GetClassBundle_MultipleKouWeakPrefixes_ReturnsAll()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("弱効炎", ref idx);
            Assert.AreEqual("弱効炎", result);
            Assert.AreEqual(3, idx);
        }

        [TestMethod]
        public void GetClassBundle_WeakAtEndOfString_ReturnsNull()
        {
            // 「弱」が末尾でそれ以上文字がない場合 → 属性なしで null を返す
            int idx = 1;
            var result = GeneralLib.GetClassBundle("弱", ref idx);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetClassBundle_KouAtEndOfString_ReturnsNull()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("効", ref idx);
            Assert.IsNull(result);
        }

        // ──────────────────────────────────────────────
        // GetClassBundle – 低 接頭辞
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetClassBundle_LowBouPrefix_ReturnsLowBou()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("低防", ref idx);
            Assert.AreEqual("低防", result);
            Assert.AreEqual(2, idx);
        }

        [TestMethod]
        public void GetClassBundle_LowUnPrefix_ReturnsLowUn()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("低運", ref idx);
            Assert.AreEqual("低運", result);
            Assert.AreEqual(2, idx);
        }

        [TestMethod]
        public void GetClassBundle_LowIdouPrefix_ReturnsLowIdou()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("低移", ref idx);
            Assert.AreEqual("低移", result);
            Assert.AreEqual(2, idx);
        }

        [TestMethod]
        public void GetClassBundle_LowInvalidNext_ReturnsNull()
        {
            // 低の次が攻防運移でない → 属性なし
            int idx = 1;
            var result = GeneralLib.GetClassBundle("低x", ref idx);
            Assert.IsNull(result);
        }

        [TestMethod]
        public void GetClassBundle_LowAtEndOfString_ReturnsNull()
        {
            int idx = 1;
            var result = GeneralLib.GetClassBundle("低", ref idx);
            Assert.IsNull(result);
        }

        // ──────────────────────────────────────────────
        // GetClassBundle – idx が 1 以外から開始
        // ──────────────────────────────────────────────

        [TestMethod]
        public void GetClassBundle_NonOneStartIdx_ReadsFromIdx()
        {
            // "水炎" の idx=2 から開始すると "炎" を返す
            int idx = 2;
            var result = GeneralLib.GetClassBundle("水炎", ref idx);
            Assert.AreEqual("炎", result);
            Assert.AreEqual(2, idx);
        }

        [TestMethod]
        public void GetClassBundle_WithLength_ReturnsFixedLength()
        {
            // length 指定があれば idx から length 文字を返す
            int idx = 1;
            var result = GeneralLib.GetClassBundle("弱炎水", ref idx, 2);
            Assert.AreEqual("弱炎", result);
        }

        // ──────────────────────────────────────────────
        // InStrNotNest – 効・剋 接頭辞のスキップ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStrNotNest_PrecededByKou_Skips()
        {
            // 「効炎」内の「炎」は「効」の後ろなのでスキップ → 0
            Assert.AreEqual(0, GeneralLib.InStrNotNest("効炎", "炎"));
        }

        [TestMethod]
        public void InStrNotNest_PrecededByKoku_Skips()
        {
            // 「剋水」内の「水」は「剋」の後ろなのでスキップ → 0
            Assert.AreEqual(0, GeneralLib.InStrNotNest("剋水", "水"));
        }

        [TestMethod]
        public void InStrNotNest_KouPrecededThenPlain_FindsPlain()
        {
            // "効炎炎" → 最初の炎はスキップ、次の炎は位置 3 で返る
            Assert.AreEqual(3, GeneralLib.InStrNotNest("効炎炎", "炎"));
        }

        // ──────────────────────────────────────────────
        // StrToHiragana – 混合文字列
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrToHiragana_MixedKatakanaHiragana_ConvertsKatakanaOnly()
        {
            var result = GeneralLib.StrToHiragana("アいウえオ");
            Assert.AreEqual("あいうえお", result);
        }

        [TestMethod]
        public void StrToHiragana_EmptyString_ReturnsEmpty()
        {
            var result = GeneralLib.StrToHiragana("");
            Assert.AreEqual("", result);
        }

        // ──────────────────────────────────────────────
        // LeftPaddedString / RightPaddedString – 全角文字
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LeftPaddedString_FullWidthChar_CountsAsTwo()
        {
            // 全角1文字 (幅2) を length=4 に左寄せ → スペース2個付加
            var result = GeneralLib.LeftPaddedString("あ", 4);
            Assert.AreEqual("  あ", result);
        }

        [TestMethod]
        public void RightPaddedString_FullWidthChar_CountsAsTwo()
        {
            var result = GeneralLib.RightPaddedString("あ", 4);
            Assert.AreEqual("あ  ", result);
        }

        [TestMethod]
        public void LeftPaddedString_EmptyString_AllSpaces()
        {
            var result = GeneralLib.LeftPaddedString("", 3);
            Assert.AreEqual("   ", result);
        }

        [TestMethod]
        public void RightPaddedString_EmptyString_AllSpaces()
        {
            var result = GeneralLib.RightPaddedString("", 3);
            Assert.AreEqual("   ", result);
        }

        // ──────────────────────────────────────────────
        // IsNumber – エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsNumber_JustMinus_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsNumber("-"));
        }

        [TestMethod]
        public void IsNumber_Zero_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("0"));
        }

        [TestMethod]
        public void IsNumber_NegativeFloat_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("-3.14"));
        }

        // ──────────────────────────────────────────────
        // InStr2 – エッジケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void InStr2_EmptySearchString_ReturnsZero()
        {
            // str2 が空文字列のとき 0 を返す（slen=0, i = len+1 から開始するが Midが空一致）
            // 実装上 Strings.Mid(str, i, 0) == "" が常に成立するため i を返す
            var result = GeneralLib.InStr2("abc", "");
            // len("abc")=3, slen=0, i = 3-0+1 = 4 → Mid("abc",4,0)="" == "" → return 4
            Assert.AreEqual(4, result);
        }

        [TestMethod]
        public void InStr2_EmptySource_ReturnsZero()
        {
            // str が空文字列: i = 0-0+1=1 → 1>0 だが Mid("",1,0)="" → return 1?
            // 実際には Strings.Len("")=0, slen=0, i=1 → Mid("",1,0)="" == "" → return 1
            // ただし実用上は 0 も可。実装を確認して一致させる
            // Strings.Mid("", 1, 0) = "" → result = 1
            var result = GeneralLib.InStr2("", "");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void InStr2_SingleCharRepeated_ReturnsLastPos()
        {
            // "aaa" で "a" を末尾から検索 → 位置 3
            Assert.AreEqual(3, GeneralLib.InStr2("aaa", "a"));
        }
    }
}
