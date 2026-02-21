using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Paint.cs の RGB 関数のユニットテスト
    /// </summary>
    [TestClass]
    public class PaintRGBFunctionTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // RGB 関数
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RGB_Black_ReturnsBlackHex()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RGB(0,0,0)");
            Assert.AreEqual("#000000", result);
        }

        [TestMethod]
        public void RGB_White_ReturnsWhiteHex()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RGB(255,255,255)");
            Assert.AreEqual("#ffffff", result);
        }

        [TestMethod]
        public void RGB_Red_ReturnsRedHex()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RGB(255,0,0)");
            Assert.AreEqual("#ff0000", result);
        }

        [TestMethod]
        public void RGB_Green_ReturnsGreenHex()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RGB(0,255,0)");
            Assert.AreEqual("#00ff00", result);
        }

        [TestMethod]
        public void RGB_Blue_ReturnsBlueHex()
        {
            var exp = Create();
            var result = exp.GetValueAsString("RGB(0,0,255)");
            Assert.AreEqual("#0000ff", result);
        }

        [TestMethod]
        public void RGB_MixedColor_ReturnsCorrectHex()
        {
            var exp = Create();
            // R=128, G=64, B=32
            var result = exp.GetValueAsString("RGB(128,64,32)");
            Assert.AreEqual("#804020", result);
        }

        [TestMethod]
        public void RGB_LowValues_ReturnsPaddedHex()
        {
            var exp = Create();
            // R=1, G=2, B=3 → #010203
            var result = exp.GetValueAsString("RGB(1,2,3)");
            Assert.AreEqual("#010203", result);
        }

        [TestMethod]
        public void RGB_ReturnsStringType()
        {
            var exp = Create();
            // RGB はStringとして返る
            var result = exp.GetValueAsString("RGB(100,150,200)");
            Assert.IsTrue(result.StartsWith("#"));
            Assert.AreEqual(7, result.Length);
        }

        [TestMethod]
        public void RGB_WithExpressionArguments_ComputesCorrectly()
        {
            var exp = Create();
            exp.SetVariableAsDouble("r", 255d);
            exp.SetVariableAsDouble("g", 0d);
            exp.SetVariableAsDouble("b", 128d);
            var result = exp.GetValueAsString("RGB(r,g,b)");
            Assert.AreEqual("#ff0080", result);
        }

        [TestMethod]
        public void RGB_ArithmeticArguments_ComputesCorrectly()
        {
            var exp = Create();
            // R = 200+55 = 255, G = 0, B = 0
            var result = exp.GetValueAsString("RGB(200 + 55, 0, 0)");
            Assert.AreEqual("#ff0000", result);
        }
    }
}
