using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// VarData クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class VarDataStructTests
    {
        // ──────────────────────────────────────────────
        // SetFrom
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetFrom_CopiesAllFields()
        {
            var src = new VarData("sourceVar", ValueType.NumericType, "42", 42d);
            var dst = new VarData();
            dst.SetFrom(src);

            Assert.AreEqual("sourceVar", dst.Name);
            Assert.AreEqual(ValueType.NumericType, dst.VariableType);
            Assert.AreEqual(42d, dst.NumericValue);
        }

        [TestMethod]
        public void SetFrom_StringType_CopiesCorrectly()
        {
            var src = new VarData("strVar", ValueType.StringType, "hello", 0d);
            var dst = new VarData();
            dst.SetFrom(src);

            Assert.AreEqual("strVar", dst.Name);
            Assert.AreEqual(ValueType.StringType, dst.VariableType);
            Assert.AreEqual("hello", dst.StringValue);
        }

        [TestMethod]
        public void SetFrom_DoesNotAlterSource()
        {
            var src = new VarData("original", ValueType.NumericType, "10", 10d);
            var dst = new VarData();
            dst.SetFrom(src);
            dst.Init("changed");

            Assert.AreEqual("original", src.Name);
        }

        // ──────────────────────────────────────────────
        // ReferenceValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReferenceValue_Numeric_ReturnsNumericValue()
        {
            var v = new VarData("x", ValueType.NumericType, "5", 5d);
            var resultType = v.ReferenceValue(ValueType.NumericType, out var str, out var num);

            Assert.AreEqual(ValueType.NumericType, resultType);
            Assert.AreEqual(5d, num);
        }

        [TestMethod]
        public void ReferenceValue_String_ReturnsStringValue()
        {
            var v = new VarData("s", ValueType.StringType, "hello", 0d);
            var resultType = v.ReferenceValue(ValueType.StringType, out var str, out var num);

            Assert.AreEqual(ValueType.StringType, resultType);
            Assert.AreEqual("hello", str);
        }

        [TestMethod]
        public void ReferenceValue_NumericFromString_Converts()
        {
            var v = new VarData("x", ValueType.StringType, "123", 0d);
            v.ReferenceValue(ValueType.NumericType, out var str, out var num);
            Assert.AreEqual(123d, num);
        }

        [TestMethod]
        public void ReferenceValue_StringFromNumeric_Formats()
        {
            var v = new VarData("x", ValueType.NumericType, "7", 7d);
            v.ReferenceValue(ValueType.StringType, out var str, out var num);
            Assert.AreEqual("7", str);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var v = new VarData("myVar", ValueType.NumericType, "10", 10d);
            var result = v.ToString();
            Assert.IsTrue(result.Contains("myVar"));
        }

        // ──────────────────────────────────────────────
        // Clear / Init
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_SetsEmptyName()
        {
            var v = new VarData("name", ValueType.NumericType, "5", 5d);
            v.Clear();
            Assert.AreEqual("", v.Name);
        }

        [TestMethod]
        public void Init_SetsProvidedName()
        {
            var v = new VarData();
            v.Init("newName");
            Assert.AreEqual("newName", v.Name);
        }

        [TestMethod]
        public void Init_SetsStringTypeAndEmptyValue()
        {
            var v = new VarData();
            v.Init("x");
            Assert.AreEqual(ValueType.StringType, v.VariableType);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }
    }
}
