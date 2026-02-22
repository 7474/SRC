using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions.Functions;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// AFunction 基底クラスおよび WindowWidth / WindowHeight 関数のユニットテスト
    /// </summary>
    [TestClass]
    public class FunctionBaseAndWindowTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // AFunction.Name プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Abs_Name_ReturnsClassName()
        {
            var func = new Abs();
            Assert.AreEqual("Abs", func.Name);
        }

        [TestMethod]
        public void Sin_Name_ReturnsClassName()
        {
            var func = new Sin();
            Assert.AreEqual("Sin", func.Name);
        }

        [TestMethod]
        public void Cos_Name_ReturnsClassName()
        {
            var func = new Cos();
            Assert.AreEqual("Cos", func.Name);
        }

        [TestMethod]
        public void Len_Name_ReturnsClassName()
        {
            var func = new Len();
            Assert.AreEqual("Len", func.Name);
        }

        [TestMethod]
        public void LLength_Name_ReturnsClassName()
        {
            var func = new LLength();
            Assert.AreEqual("LLength", func.Name);
        }

        [TestMethod]
        public void RegExp_Name_ReturnsClassName()
        {
            var func = new RegExp();
            Assert.AreEqual("RegExp", func.Name);
        }

        [TestMethod]
        public void RGB_Name_ReturnsClassName()
        {
            var func = new RGB();
            Assert.AreEqual("RGB", func.Name);
        }

        [TestMethod]
        public void Dir_Name_ReturnsClassName()
        {
            var func = new Dir();
            Assert.AreEqual("Dir", func.Name);
        }

        // ──────────────────────────────────────────────
        // WindowWidth / WindowHeight
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WindowWidth_ReturnsNumericValue()
        {
            var exp = Create();
            var result = exp.GetValueAsDouble("WindowWidth()");
            Assert.IsTrue(result >= 0d);
        }

        [TestMethod]
        public void WindowWidth_StringReturn_ReturnsNonEmpty()
        {
            var exp = Create();
            var result = exp.GetValueAsString("WindowWidth()");
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }

        [TestMethod]
        public void WindowHeight_ReturnsNumericValue()
        {
            var exp = Create();
            var result = exp.GetValueAsDouble("WindowHeight()");
            Assert.IsTrue(result >= 0d);
        }

        [TestMethod]
        public void WindowHeight_StringReturn_ReturnsNonEmpty()
        {
            var exp = Create();
            var result = exp.GetValueAsString("WindowHeight()");
            Assert.IsFalse(string.IsNullOrEmpty(result));
        }
    }
}
