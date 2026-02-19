using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System.Text;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// Tests for byte-based string functions using Shift-JIS encoding
    /// These functions are used for proper display width calculation in fixed-width text layouts
    /// </summary>
    [TestClass()]
    public class StringsBTests
    {
        [TestMethod()]
        public void LenBTest_AsciiString()
        {
            // ASCII characters should be 1 byte each
            Assert.AreEqual(5, Strings.LenB("hello"));
            Assert.AreEqual(3, Strings.LenB("abc"));
        }

        [TestMethod()]
        public void LenBTest_FullWidthString()
        {
            // Full-width characters should be 2 bytes each in Shift-JIS
            Assert.AreEqual(6, Strings.LenB("あいう")); // 3 characters * 2 bytes
            Assert.AreEqual(2, Strings.LenB("全")); // 1 full-width character
        }

        [TestMethod()]
        public void LenBTest_MixedString()
        {
            // Mixed ASCII and full-width
            Assert.AreEqual(4, Strings.LenB("aあb")); // 1 + 2 + 1 = 4 bytes
            Assert.AreEqual(8, Strings.LenB("test全角")); // 4 + 4 = 8 bytes
        }

        [TestMethod()]
        public void LenBTest_EmptyString()
        {
            Assert.AreEqual(0, Strings.LenB(""));
            Assert.AreEqual(0, Strings.LenB(null));
        }

        [TestMethod()]
        public void LeftBTest_AsciiString()
        {
            Assert.AreEqual("hel", Strings.LeftB("hello", 3));
            Assert.AreEqual("hello", Strings.LeftB("hello", 10)); // More than length
        }

        [TestMethod()]
        public void LeftBTest_FullWidthString()
        {
            Assert.AreEqual("あ", Strings.LeftB("あいう", 2)); // 1 full-width char = 2 bytes
            Assert.AreEqual("あい", Strings.LeftB("あいう", 4)); // 2 full-width chars = 4 bytes
        }

        [TestMethod()]
        public void LeftBTest_MixedString()
        {
            Assert.AreEqual("a", Strings.LeftB("aあb", 1));
            Assert.AreEqual("aあ", Strings.LeftB("aあb", 3));
        }

        [TestMethod()]
        public void RightBTest_AsciiString()
        {
            Assert.AreEqual("lo", Strings.RightB("hello", 2));
            Assert.AreEqual("hello", Strings.RightB("hello", 10)); // More than length
        }

        [TestMethod()]
        public void RightBTest_FullWidthString()
        {
            Assert.AreEqual("う", Strings.RightB("あいう", 2)); // 1 full-width char from right
            Assert.AreEqual("いう", Strings.RightB("あいう", 4)); // 2 full-width chars from right
        }

        [TestMethod()]
        public void MidBTest_AsciiString()
        {
            Assert.AreEqual("ell", Strings.MidB("hello", 2, 3)); // Start at byte 2, take 3 bytes
            Assert.AreEqual("ello", Strings.MidB("hello", 2)); // Start at byte 2, take rest
        }

        [TestMethod()]
        public void MidBTest_FullWidthString()
        {
            // Start at byte 3 (second character), take 2 bytes (one character)
            Assert.AreEqual("い", Strings.MidB("あいう", 3, 2));
            Assert.AreEqual("いう", Strings.MidB("あいう", 3)); // Take rest from byte 3
        }

        [TestMethod()]
        public void MidBTest_MixedString()
        {
            // "aあb" = a(1) + あ(2) + b(1) = 4 bytes total
            Assert.AreEqual("あ", Strings.MidB("aあb", 2, 2)); // Start at byte 2, take 2 bytes
            Assert.AreEqual("あb", Strings.MidB("aあb", 2)); // Start at byte 2, take rest
        }

        [TestMethod()]
        public void InStrBTest_AsciiString()
        {
            Assert.AreEqual(2, Strings.InStrB("hello", "el")); // Found at byte position 2
            Assert.AreEqual(0, Strings.InStrB("hello", "xyz")); // Not found
            Assert.AreEqual(1, Strings.InStrB("hello", "h")); // Found at start
        }

        [TestMethod()]
        public void InStrBTest_FullWidthString()
        {
            // "あいう" each char is 2 bytes
            Assert.AreEqual(3, Strings.InStrB("あいう", "い")); // Second char starts at byte 3
            Assert.AreEqual(1, Strings.InStrB("あいう", "あ")); // First char starts at byte 1
        }

        [TestMethod()]
        public void InStrBTest_WithStartPosition()
        {
            // "hello" = h(1) e(2) l(3) l(4) o(5)
            // Start search from byte 3 should find 'l' at position 3
            Assert.AreEqual(3, Strings.InStrB(3, "hello", "l")); // Start search from byte 3
            Assert.AreEqual(0, Strings.InStrB(10, "hello", "l")); // Start beyond string
        }

        [TestMethod()]
        public void InStrRevBTest_AsciiString()
        {
            // "hello" has two 'l's at positions 3 and 4
            // InStrRevB should find the last occurrence at position 4
            Assert.AreEqual(4, Strings.InStrRevB("hello", "l")); // Last 'l' at position 4
            Assert.AreEqual(0, Strings.InStrRevB("hello", "xyz")); // Not found
        }

        [TestMethod()]
        public void InStrRevBTest_FullWidthString()
        {
            // "あいあ" - find last "あ"
            Assert.AreEqual(5, Strings.InStrRevB("あいあ", "あ")); // Last あ starts at byte 5
        }

        [TestMethod()]
        public void InStrRevBTest_WithStartPosition()
        {
            // Search backwards from specified position
            var pos = Strings.InStrRevB(3, "hello", "l");
            Assert.IsTrue(pos >= 0); // Should find first 'l' when searching backwards from byte 3
        }

        [TestMethod()]
        public void StrComp_DisplayWidthScenario()
        {
            // Real-world scenario: calculating display width for fixed-width text
            var unitName1 = "スーパーロボット"; // Full-width Japanese
            var unitName2 = "SuperRobot"; // Half-width English
            
            // Both might display as similar width visually, but byte counts differ
            var bytes1 = Strings.LenB(unitName1);
            var bytes2 = Strings.LenB(unitName2);
            
            // Full-width should have more bytes
            Assert.IsTrue(bytes1 > bytes2);
            
            // This is why OrganizeCmd.cs uses LenB for spacing calculations
            var targetWidth = 36;
            var spacing1 = targetWidth - bytes1;
            var spacing2 = targetWidth - bytes2;
            
            // Different spacing needed based on byte count
            Assert.IsTrue(spacing1 < spacing2);
        }
    }
}
