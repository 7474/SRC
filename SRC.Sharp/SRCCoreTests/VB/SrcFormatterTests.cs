using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCCore.VB.Tests
{
    [TestClass()]
    public class SrcFormatterTests
    {
        [TestMethod()]
        public void FormatTest()
        {
            Assert.AreEqual("01", SrcFormatter.Format(1, "00"));
        }

        [TestMethod()]
        public void FormatTest_Object()
        {
            Assert.AreEqual("42", SrcFormatter.Format((object)42));
            Assert.AreEqual("hello", SrcFormatter.Format((object)"hello"));
            Assert.AreEqual("", SrcFormatter.Format(null));
        }

        [TestMethod()]
        public void FormatTest_IntWithFormat()
        {
            Assert.AreEqual("007", SrcFormatter.Format(7, "000"));
            Assert.AreEqual("100", SrcFormatter.Format(100, "000"));
        }

        [TestMethod()]
        public void FormatTest_DoubleWithFormat()
        {
            Assert.AreEqual("3.14", SrcFormatter.Format(3.14, "0.##"));
            Assert.AreEqual("3", SrcFormatter.Format(3.0, "0.##"));
        }

        [TestMethod()]
        public void FormatTest_InvalidFormat_FallsBack()
        {
            // 無効なフォーマット文字列の場合は値をそのまま文字列化する
            var result = SrcFormatter.Format(42, "\x00invalid\x01");
            Assert.IsNotNull(result);
        }
    }
}