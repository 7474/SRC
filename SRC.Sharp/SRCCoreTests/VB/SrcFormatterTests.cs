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
    }
}