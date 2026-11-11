using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Drawing;
using System.Globalization;

namespace SRCCore.Extensions.Tests
{
    /// <summary>
    /// ColorExtension.FromHexString が System.Drawing.ColorTranslator.FromHtml と同じ結果を返すことのテスト
    /// </summary>
    [TestClass]
    public class ColorExtensionHtmlCompatTests
    {
        [TestMethod]
        [DataRow("")]
        [DataRow("#000000")]
        [DataRow("#AbCdEf")]
        [DataRow("#f0a")]
        [DataRow("#12345")]
        [DataRow("#1234567")]
        [DataRow("red")]
        [DataRow(" Red ")]
        [DataRow("lightgrey")]
        [DataRow("transparent")]
        [DataRow("buttonface")]
        [DataRow("'blue'")]
        [DataRow("0xFF0000")]
        [DataRow("0x00FFFF")]
        [DataRow("&H0000FF")]
        [DataRow("255")]
        [DataRow("-1")]
        [DataRow("255,0,0")]
        [DataRow("128,255,0,0")]
        public void FromHexString_SameAsColorTranslator(string str)
        {
            // ColorTranslator は実行する環境のリストの区切り（ドイツ語では「;」）で「255,0,0」を分ける。
            // SRC# は日本語の設定（「,」）に固定しているので、比べる側も「,」の設定で求める（7474/SRC#1554）
            var culture = CultureInfo.CurrentCulture;
            Color expected;
            try
            {
                CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
                expected = ColorTranslator.FromHtml(str);
            }
            finally
            {
                CultureInfo.CurrentCulture = culture;
            }
            var actual = ColorExtension.FromHexString(str);
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        [DataRow("#xyz123")]
        [DataRow("notacolor")]
        [DataRow("1,2")]
        [DataRow("#")]
        public void FromHexString_InvalidInput_ThrowsSameExceptionAsColorTranslator(string str)
        {
            Type expected = null;
            Type actual = null;
            try { ColorTranslator.FromHtml(str); } catch (Exception ex) { expected = ex.GetType(); }
            try { ColorExtension.FromHexString(str); } catch (Exception ex) { actual = ex.GetType(); }
            Assert.IsNotNull(expected);
            Assert.AreEqual(expected, actual);
        }
    }
}
