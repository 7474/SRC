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

        [TestMethod()]
        public void FromHexStringTest()
        {
            var red = ColorExtension.FromHexString("#ff0000");
            Assert.AreEqual(255, red.R);
            Assert.AreEqual(0, red.G);
            Assert.AreEqual(0, red.B);

            var blue = ColorExtension.FromHexString("#0000ff");
            Assert.AreEqual(0, blue.R);
            Assert.AreEqual(0, blue.G);
            Assert.AreEqual(255, blue.B);
        }

        [TestMethod()]
        public void TryFromHexStringTest_ValidColor()
        {
            var result = ColorExtension.TryFromHexString("#ff0000", out var color);
            Assert.IsTrue(result);
            Assert.AreEqual(255, color.R);
            Assert.AreEqual(0, color.G);
            Assert.AreEqual(0, color.B);
        }

        [TestMethod()]
        public void TryFromHexStringTest_InvalidColor()
        {
            var result = ColorExtension.TryFromHexString("invalid", out var color);
            Assert.IsFalse(result);
            Assert.AreEqual(Color.Empty, color);
        }
    }
}