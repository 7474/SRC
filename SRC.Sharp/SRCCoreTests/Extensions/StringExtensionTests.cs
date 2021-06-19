using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCCore.Extensions.Tests
{
    [TestClass()]
    public class StringExtensionTests
    {
        [TestMethod()]
        public void ArrayIndexByNameTest()
        {
            Assert.AreEqual("", "hoge".ArrayIndexByName());
            Assert.AreEqual("", "ho[1]ge".ArrayIndexByName());
            Assert.AreEqual("", "[1]hoge".ArrayIndexByName());
            Assert.AreEqual("", "[1]".ArrayIndexByName());
            Assert.AreEqual("1", "hoge[1]".ArrayIndexByName());
            Assert.AreEqual("abc", "hoge[abc]".ArrayIndexByName());
            Assert.AreEqual("fuga[abc]", "hoge[fuga[abc]]".ArrayIndexByName());
        }

        [TestMethod()]
        public void InsideKakkoTest()
        {
            Assert.AreEqual("", "hoge".InsideKakko());
            Assert.AreEqual("1", "hoge(1)".InsideKakko());
            Assert.AreEqual("abc", "ho(abc)ge".InsideKakko());
            Assert.AreEqual("fuga(abc)", "(fuga(abc))".InsideKakko());
        }

        [TestMethod()]
        public void ReplaceNewLineTest()
        {
            Assert.AreEqual("123 456  789", @"123
456

789".ReplaceNewLine(" "));
        }

        [TestMethod()]
        public void RemoveLineCommentTest()
        {
            Assert.AreEqual("", "// hoge".RemoveLineComment());
            Assert.AreEqual("a", "a// hoge".RemoveLineComment());
            Assert.AreEqual("a ", "a // hoge".RemoveLineComment());
            Assert.AreEqual("'// hoge'", "'// hoge'".RemoveLineComment());
            Assert.AreEqual("'// hoge'", "'// hoge'// fuga".RemoveLineComment());
            Assert.AreEqual("\"// hoge", "\"// hoge".RemoveLineComment());
        }
    }
}
