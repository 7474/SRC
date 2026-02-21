using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Strings クラスのバイト操作関数・その他の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class StringsAdditionalTests
    {
        // Strings.Left / Right / Mid
        [TestMethod] public void Left_EmptyString_ReturnsEmpty() => Assert.AreEqual("", Strings.Left("", 5));
        [TestMethod] public void Left_ZeroLength_ReturnsEmpty() => Assert.AreEqual("", Strings.Left("hello", 0));
        [TestMethod] public void Left_LengthExceedsString_ReturnsFullString() => Assert.AreEqual("hi", Strings.Left("hi", 100));
        [TestMethod] public void Right_EmptyString_ReturnsEmpty() => Assert.AreEqual("", Strings.Right("", 5));
        [TestMethod] public void Right_ZeroLength_ReturnsEmpty() => Assert.AreEqual("", Strings.Right("hello", 0));
        [TestMethod] public void Right_LengthExceedsString_ReturnsFullString() => Assert.AreEqual("hi", Strings.Right("hi", 100));
        [TestMethod] public void Mid_EmptyString_ReturnsEmpty() => Assert.AreEqual("", Strings.Mid("", 1, 5));
        [TestMethod] public void Mid_StartBeyondString_ReturnsEmpty() => Assert.AreEqual("", Strings.Mid("hi", 10, 5));
        [TestMethod] public void Mid_NullString_ReturnsEmpty() => Assert.AreEqual("", Strings.Mid(null, 1, 5));
        [TestMethod] public void Mid_LengthExceedsString_ReturnsRemainder() => Assert.AreEqual("llo", Strings.Mid("hello", 3, 100));
        [TestMethod] public void Mid_NoLengthArg_ReturnsFromStart() => Assert.AreEqual("llo", Strings.Mid("hello", 3));

        // Strings.Len / LenB
        [TestMethod] public void Len_EmptyString_ReturnsZero() => Assert.AreEqual(0, Strings.Len(""));
        [TestMethod] public void Len_NullString_ReturnsZero() => Assert.AreEqual(0, Strings.Len(null));
        [TestMethod] public void Len_Spaces_CountsSpaces() => Assert.AreEqual(5, Strings.Len("     "));
        [TestMethod] public void Len_JapaneseCharacters_CountsChars() => Assert.AreEqual(3, Strings.Len("テスト"));
        [TestMethod] public void LenB_AsciiString_TwiceLenInShiftJIS() => Assert.AreEqual(5, Strings.LenB("hello"));
        [TestMethod] public void LenB_JapaneseString_TwiceLenInShiftJIS() => Assert.AreEqual(6, Strings.LenB("テスト"));

        // Strings.Trim
        [TestMethod] public void Trim_EmptyString_ReturnsEmpty() => Assert.AreEqual("", Strings.Trim(""));
        [TestMethod] public void Trim_OnlySpaces_ReturnsEmpty() => Assert.AreEqual("", Strings.Trim("   "));
        [TestMethod] public void Trim_LeadingAndTrailingSpaces_Removes() => Assert.AreEqual("hello", Strings.Trim("  hello  "));
        [TestMethod] public void Trim_NullString_ReturnsEmpty() => Assert.AreEqual("", Strings.Trim(null));

        // Strings.LCase
        [TestMethod] public void LCase_UppercaseInput_ReturnsLowercase() => Assert.AreEqual("hello", Strings.LCase("HELLO"));
        [TestMethod] public void LCase_EmptyString_ReturnsEmpty() => Assert.AreEqual("", Strings.LCase(""));
        [TestMethod] public void LCase_MixedCase_AllLowercase() => Assert.AreEqual("hello world", Strings.LCase("Hello World"));
        [TestMethod] public void LCase_AlreadyLower_Unchanged() => Assert.AreEqual("test", Strings.LCase("test"));

        // Strings.InStr
        [TestMethod] public void InStr_SubstringFound_ReturnsPosition() => Assert.AreEqual(3, Strings.InStr("abcdef", "cd"));
        [TestMethod] public void InStr_SubstringNotFound_ReturnsZero() => Assert.AreEqual(0, Strings.InStr("abcdef", "xyz"));
        [TestMethod] public void InStr_EmptyString_ReturnsZero() => Assert.AreEqual(0, Strings.InStr("", "abc"));
        [TestMethod] public void InStr_NullString_ReturnsZero() => Assert.AreEqual(0, Strings.InStr(null, "abc"));
        [TestMethod] public void InStr_WithStartPosition_SearchesFromPosition() => Assert.AreEqual(5, Strings.InStr(3, "abcdeabc", "ea"));
        [TestMethod] public void InStr_AtStart_ReturnsOne() => Assert.AreEqual(1, Strings.InStr("hello", "he"));
        [TestMethod] public void InStr_AtEnd_ReturnsCorrectPosition() => Assert.AreEqual(4, Strings.InStr("hello", "lo"));

        // Strings.Space
        [TestMethod] public void Space_ZeroCount_ReturnsEmpty() => Assert.AreEqual("", Strings.Space(0));
        [TestMethod] public void Space_PositiveCount_ReturnsSpaces() => Assert.AreEqual("   ", Strings.Space(3));
        [TestMethod] public void Space_OneCount_ReturnsSingleSpace() => Assert.AreEqual(" ", Strings.Space(1));

        // Strings.Asc
        [TestMethod] public void Asc_LetterA_Returns65() => Assert.AreEqual(65, Strings.Asc("A"));
        [TestMethod] public void Asc_LowercaseA_Returns97() => Assert.AreEqual(97, Strings.Asc("a"));
        [TestMethod] public void Asc_Dollar_Returns36() => Assert.AreEqual(36, Strings.Asc("$"));
        [TestMethod] public void Asc_SingleChar_ReturnsCorrectValue() => Assert.AreEqual(42, Strings.Asc("*"));
        [TestMethod] public void Asc_Char_ReturnsCorrectValue() => Assert.AreEqual(48, Strings.Asc("0"));

        // Strings.StrDup
        [TestMethod] public void StrDup_ZeroCount_ReturnsEmpty() => Assert.AreEqual("", Strings.StrDup("a", 0));
        [TestMethod] public void StrDup_MultipleCount_RepeatsString() => Assert.AreEqual("aaa", Strings.StrDup("a", 3));
        [TestMethod] public void StrDup_LongerString_RepeatsString() => Assert.AreEqual("abab", Strings.StrDup("ab", 2));

        // Strings.StrComp
        [TestMethod] public void StrComp_EqualStrings_ReturnsZero() => Assert.AreEqual(0, Strings.StrComp("abc", "abc"));
        [TestMethod] public void StrComp_LessThan_ReturnsNegative() => Assert.IsTrue(Strings.StrComp("abc", "abd") < 0);
        [TestMethod] public void StrComp_GreaterThan_ReturnsPositive() => Assert.IsTrue(Strings.StrComp("abd", "abc") > 0);

        // Strings byte functions (LeftB, RightB, MidB, InStrB)
        [TestMethod] public void LeftB_AsciiChars_ReturnsCorrectBytes() => Assert.AreEqual("he", Strings.LeftB("hello", 2));
        [TestMethod] public void RightB_AsciiChars_ReturnsCorrectBytes() => Assert.AreEqual("lo", Strings.RightB("hello", 2));
        [TestMethod] public void MidB_AsciiChars_ReturnsCorrectSubstring() => Assert.AreEqual("el", Strings.MidB("hello", 2, 2));
        [TestMethod] public void InStrB_FindSubstring_ReturnsPosition() => Assert.AreEqual(2, Strings.InStrB("hello", "el"));
        [TestMethod] public void InStrB_NotFound_ReturnsZero() => Assert.AreEqual(0, Strings.InStrB("hello", "xyz"));
        [TestMethod] public void InStrRevB_FindLast_ReturnsPosition() => Assert.AreEqual(4, Strings.InStrRevB("abcabc", "ab"));
        [TestMethod] public void InStrRevB_NotFound_ReturnsZero() => Assert.AreEqual(0, Strings.InStrRevB("hello", "xyz"));
    }
}
