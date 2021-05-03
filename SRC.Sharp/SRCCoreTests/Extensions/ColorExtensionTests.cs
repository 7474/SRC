using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCCore.Extensions.Tests
{
    [TestClass()]
    public class ColorExtensionTests
    {
        [TestMethod()]
        public void ToHexStringTest()
        {
            Assert.AreEqual("#ff0000", Color.Red.ToHexString());
            Assert.AreEqual("#00ff00", Color.FromArgb(0, 255, 0).ToHexString());
            Assert.AreEqual("#0000ff", Color.FromArgb(0, 0, 255).ToHexString());
        }
    }
}