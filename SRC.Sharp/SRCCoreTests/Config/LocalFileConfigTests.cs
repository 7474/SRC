using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCCore.Config.Tests
{
    [TestClass()]
    public class LocalFileConfigTests
    {
        [TestMethod()]
        public void LoadTest()
        {
            var config = new LocalFileConfig();
            config.Load();

            Assert.AreEqual(true, config.ShowSquareLine);
            Assert.AreEqual("TestValue1", config.GetItem("TestSection1", "TestName1"));
            Assert.AreEqual("TestValue2", config.GetItem("TestSection1", "TestName2"));
            Assert.AreEqual("", config.GetItem("TestSection1", "TestName3"));
            Assert.AreEqual("", config.GetItem("TestSection2", "TestName1"));
        }
    }
}