using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SRCCore.VB.Tests
{
    [TestClass]
    public class VBMathTests
    {
        // VB5 の CLng は偶数丸め（銀行型丸め）
        [TestMethod]
        [DataRow(0.5, 0)]
        [DataRow(1.5, 2)]
        [DataRow(2.5, 2)]
        [DataRow(3.5, 4)]
        [DataRow(-0.5, 0)]
        [DataRow(-1.5, -2)]
        [DataRow(-2.5, -2)]
        [DataRow(85.5, 86)]
        [DataRow(84.5, 84)]
        public void CLng_RoundsHalfToEven(double x, int expected)
        {
            Assert.AreEqual(expected, VBMath.CLng(x));
        }

        [TestMethod]
        [DataRow(0.0, 0)]
        [DataRow(1.4, 1)]
        [DataRow(1.6, 2)]
        [DataRow(-1.4, -1)]
        [DataRow(-1.6, -2)]
        [DataRow(2250.75, 2251)]
        public void CLng_RoundsToNearest(double x, int expected)
        {
            Assert.AreEqual(expected, VBMath.CLng(x));
        }

        [TestMethod]
        public void CLng_DiffersFromTruncation()
        {
            // (int) キャストでは切り捨てになる値
            var x = 100 * 0.75 * 1.5; // 112.5
            Assert.AreEqual(112, (int)x);
            Assert.AreEqual(112, VBMath.CLng(x));
            Assert.AreEqual(113, VBMath.CLng(112.6));
            Assert.AreEqual(19, VBMath.CLng(100d * 3 / 16d)); // 18.75
        }

        [TestMethod]
        public void CLng_Overflow_Throws()
        {
            Assert.Throws<OverflowException>(() => VBMath.CLng(1e10));
        }
    }
}
