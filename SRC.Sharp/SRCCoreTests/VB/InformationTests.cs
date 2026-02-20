using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    [TestClass()]
    public class InformationTests
    {
        [TestMethod()]
        public void IsNumericTest_Numbers()
        {
            Assert.IsTrue(Information.IsNumeric("42"));
            Assert.IsTrue(Information.IsNumeric("3.14"));
            Assert.IsTrue(Information.IsNumeric("-10"));
            Assert.IsTrue(Information.IsNumeric("0"));
        }

        [TestMethod()]
        public void IsNumericTest_NonNumbers()
        {
            Assert.IsFalse(Information.IsNumeric("abc"));
            Assert.IsFalse(Information.IsNumeric("12abc"));
            Assert.IsFalse(Information.IsNumeric("abc12"));
        }

        [TestMethod()]
        public void IsNumericTest_EmptyOrNull()
        {
            Assert.IsFalse(Information.IsNumeric(""));
            Assert.IsFalse(Information.IsNumeric(null));
        }

        [TestMethod()]
        public void IsNumericTest_WhitespaceIgnored()
        {
            // 空白は無視して判断する
            Assert.IsTrue(Information.IsNumeric("  42  "));
            Assert.IsTrue(Information.IsNumeric(" 3.14 "));
        }
    }
}
