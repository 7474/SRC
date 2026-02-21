using SRCCore.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Lib.Tests
{
    [TestClass()]
    public class GeneralLibDiceTests
    {
        [TestMethod()]
        public void Dice_MaxOne_ReturnsOne()
        {
            for (int i = 0; i < 10; i++)
            {
                Assert.AreEqual(1, GeneralLib.Dice(1));
            }
        }

        [TestMethod()]
        public void Dice_MaxReturnsPositiveOrOne()
        {
            int result = GeneralLib.Dice(6);
            Assert.IsTrue(result >= 1 && result <= 6);
        }

        [TestMethod()]
        public void Dice_MaxTwo_ReturnsBetweenOneAndTwo()
        {
            int result = GeneralLib.Dice(2);
            Assert.IsTrue(result == 1 || result == 2);
        }

        [TestMethod()]
        public void Dice_CalledMultipleTimes_ReturnsValidValues()
        {
            for (int i = 0; i < 20; i++)
            {
                int result = GeneralLib.Dice(6);
                Assert.IsTrue(result >= 1 && result <= 6, $"Dice(6) returned {result} on iteration {i}");
            }
        }

        [TestMethod()]
        public void RndReset_CanBeCalled_NoException()
        {
            GeneralLib.RndReset();
        }

        [TestMethod()]
        public void StrToLng_NumberString_ReturnsInt()
        {
            Assert.AreEqual(42, GeneralLib.StrToLng("42"));
        }

        [TestMethod()]
        public void StrToLng_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0, GeneralLib.StrToLng(""));
        }

        [TestMethod()]
        public void StrToDbl_FloatString_ReturnsDouble()
        {
            Assert.AreEqual(3.14, GeneralLib.StrToDbl("3.14"), 1e-10);
        }

        [TestMethod()]
        public void StrToDbl_EmptyString_ReturnsZero()
        {
            Assert.AreEqual(0.0, GeneralLib.StrToDbl(""), 1e-10);
        }

        [TestMethod()]
        public void MaxLng_ReturnsBigger()
        {
            Assert.AreEqual(7, GeneralLib.MaxLng(3, 7));
        }

        [TestMethod()]
        public void MinLng_ReturnsSmaller()
        {
            Assert.AreEqual(3, GeneralLib.MinLng(3, 7));
        }

        [TestMethod()]
        public void MaxDbl_ReturnsBigger()
        {
            Assert.AreEqual(7.5, GeneralLib.MaxDbl(3.5, 7.5), 1e-10);
        }

        [TestMethod()]
        public void MinDbl_ReturnsSmaller()
        {
            Assert.AreEqual(3.5, GeneralLib.MinDbl(3.5, 7.5), 1e-10);
        }

        [TestMethod()]
        public void MaxLng_BothNegative_ReturnsBigger()
        {
            Assert.AreEqual(-3, GeneralLib.MaxLng(-3, -7));
        }

        [TestMethod()]
        public void MinLng_BothNegative_ReturnsSmaller()
        {
            Assert.AreEqual(-7, GeneralLib.MinLng(-3, -7));
        }
    }
}
