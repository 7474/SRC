using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions.Functions;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    [TestClass]
    public class FileFunctionTests
    {
        private static SRC CreateSrc()
        {
            return new SRC
            {
                GUI = new MockGUI(),
            };
        }

        [TestMethod]
        public void Loadfiledialog_StringType_DefaultResultIsEmptyString()
        {
            var src = CreateSrc();
            var sut = new Loadfiledialog();

            var resultType = sut.Invoke(src, ValueType.StringType, new string[1], 0, new bool[1], out var strResult, out var numResult);

            Assert.AreEqual(ValueType.StringType, resultType);
            Assert.AreEqual("", strResult);
            Assert.AreEqual(0d, numResult);
        }

        [TestMethod]
        public void Savefiledialog_StringType_DefaultResultIsEmptyString()
        {
            var src = CreateSrc();
            var sut = new Savefiledialog();

            var resultType = sut.Invoke(src, ValueType.StringType, new string[1], 0, new bool[1], out var strResult, out var numResult);

            Assert.AreEqual(ValueType.StringType, resultType);
            Assert.AreEqual("", strResult);
            Assert.AreEqual(0d, numResult);
        }
    }
}
