using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// 文字列を数値として読む規則（VB の IsNumeric・CDbl。Conversions.TryParseVbNumber）。
    /// 期待値は、日本語版の Windows の元の SRC（Ver.2.2.33）の実機の出力（SRC.Sharp/Compat/probe-expected の VB5.数値.*。#1550）
    /// </summary>
    [TestClass]
    public class VbNumberParseTests
    {
        [TestMethod]
        [DataRow("1e5", 100000d)]
        [DataRow("1E+5", 100000d)]
        [DataRow("1d5", 100000d)]
        [DataRow("&H10", 16d)]
        [DataRow("&HFFFF", 65535d)]      // 符号なし
        [DataRow("&O17", 15d)]
        [DataRow("&o17", 15d)]
        [DataRow("(1)", -1d)]            // 括弧は負の数
        [DataRow("￥100", 100d)]     // 全角の円記号
        [DataRow("\\100", 100d)]         // Shift_JIS の 0x5C
        [DataRow("1-", -1d)]             // 後ろの符号
        [DataRow("+1", 1d)]
        [DataRow(" 1 ", 1d)]
        [DataRow("1,000", 1000d)]
        [DataRow("1.5", 1.5d)]
        [DataRow(".5", 0.5d)]
        [DataRow("1.", 1d)]
        [DataRow("１２３", 123d)]
        [DataRow("2147483648", 2147483648d)]
        [DataRow("- 10", -10d)]          // 符号と数字のあいだの空白（#1413）
        public void Numeric(string text, double expected)
        {
            Assert.IsTrue(Information.IsNumeric(text), text);
            Assert.IsTrue(Conversions.TryParseVbNumber(text, out var value), text);
            Assert.AreEqual(expected, value, 1e-9, text);
            Assert.AreEqual(expected, Conversions.ToDouble(text), 1e-9, text);
        }

        [TestMethod]
        [DataRow("1e")]
        [DataRow("")]
        [DataRow("abc")]
        [DataRow("0x10")]
        [DataRow("&H")]
        [DataRow("1e400")]
        [DataRow("1 2")]
        [DataRow("(-1)")]
        [DataRow("-")]
        public void NotNumeric(string text)
        {
            Assert.IsFalse(Information.IsNumeric(text), text);
            Assert.AreEqual(0d, Conversions.ToDouble(text), text);
        }

        [TestMethod]
        public void IsNumber_ExcludesParenthesis()
        {
            // 元の SRC の IsNumber は「(」で始まる文字列を数値としない（足し算などでは -1 として読む）
            Assert.IsFalse(GeneralLib.IsNumber("(1)"));
            Assert.AreEqual(-1d, GeneralLib.StrToDbl("(1)"));
        }

        [TestMethod]
        public void StrToLng_ReadsHex()
        {
            // データの数値の欄の「&H1000」は 4096（probe-errors/data-numeric）
            Assert.AreEqual(4096, GeneralLib.StrToLng("&H1000"));
        }

        [TestMethod]
        public void FormatNum_NegativeZero()
        {
            Assert.AreEqual("0", GeneralLib.FormatNum(-0d));
        }

        [TestMethod]
        public void Checked_ThrowsOnInfinityAndNaN()
        {
            Assert.ThrowsExactly<OverflowException>(() => VBMath.Checked(double.PositiveInfinity));
            Assert.ThrowsExactly<ArgumentException>(() => VBMath.Checked(double.NaN));
            Assert.AreEqual(1.5d, VBMath.Checked(1.5d));
        }

        [TestMethod]
        public void CInt_ThrowsOnOverflow()
        {
            Assert.AreEqual((short)32767, VBMath.CInt(32767d));
            Assert.ThrowsExactly<OverflowException>(() => VBMath.CInt(65541d));
        }
    }
}
