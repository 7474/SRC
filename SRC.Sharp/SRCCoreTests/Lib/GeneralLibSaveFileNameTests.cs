using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Lib.Tests
{
    [TestClass]
    public class GeneralLibSaveFileNameTests
    {
        [TestMethod]
        [DataRow("第1話までクリア.src", "srcs", "第1話までクリア.srcs")]
        [DataRow("第1話までクリア.SRC", "srcs", "第1話までクリア.srcs")]
        [DataRow("中断.src", "srcq", "中断.srcq")]
        [DataRow("第1話までクリア.src.srcs", "srcs", "第1話までクリア.srcs")]
        [DataRow(@"C:\scenario\save.src.srcs", "srcs", @"C:\scenario\save.srcs")]
        [DataRow("第1話までクリア.srcs", "srcs", "第1話までクリア.srcs")]
        [DataRow("save", "srcs", "save")]
        [DataRow("source", "srcs", "source")]
        [DataRow("", "srcs", "")]
        [DataRow(null, "srcs", null)]
        public void ToSaveFileName(string fname, string extension, string expected)
        {
            Assert.AreEqual(expected, GeneralLib.ToSaveFileName(fname, extension));
        }
    }
}
