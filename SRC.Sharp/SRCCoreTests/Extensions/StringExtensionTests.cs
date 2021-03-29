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
            Assert.AreEqual("1", "hoge[1]".ArrayIndexByName());
            Assert.AreEqual("abc", "hoge[abc]".ArrayIndexByName());
            Assert.AreEqual("fuga[abc]", "hoge[fuga[abc]]".ArrayIndexByName());
        }
    }
}