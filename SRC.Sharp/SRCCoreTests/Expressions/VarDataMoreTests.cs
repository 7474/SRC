using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// VarData クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class VarDataMoreTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefaultConstructor_InitializesWithStringType()
        {
            var v = new VarData();
            Assert.AreEqual("", v.Name);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        [TestMethod]
        public void ParameterizedConstructor_NumericType_SetsValues()
        {
            var v = new VarData("score", ValueType.NumericType, "100", 100d);
            Assert.AreEqual("score", v.Name);
            Assert.AreEqual(ValueType.NumericType, v.VariableType);
            Assert.AreEqual(100d, v.NumericValue);
        }

        [TestMethod]
        public void ParameterizedConstructor_StringType_SetsValues()
        {
            var v = new VarData("name", ValueType.StringType, "hero", 0d);
            Assert.AreEqual("name", v.Name);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
            Assert.AreEqual("hero", v.StringValue);
        }

        // ──────────────────────────────────────────────
        // Clear / Init
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_ResetsToEmpty()
        {
            var v = new VarData("x", ValueType.NumericType, "5", 5d);
            v.Clear();
            Assert.AreEqual("", v.Name);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        [TestMethod]
        public void Init_SetsNameAndResetsValues()
        {
            var v = new VarData("x", ValueType.NumericType, "5", 5d);
            v.Init("newName");
            Assert.AreEqual("newName", v.Name);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        // ──────────────────────────────────────────────
        // SetValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetValue_NumericType_DerivesStringValue()
        {
            var v = new VarData();
            v.SetValue("x", ValueType.NumericType, "", 42d);
            Assert.AreEqual(ValueType.NumericType, v.VariableType);
            Assert.AreEqual(42d, v.NumericValue);
            // NumericType では StringValue は数値から変換される
            Assert.IsNotNull(v.StringValue);
        }

        [TestMethod]
        public void SetValue_StringType_DerivesNumericValue()
        {
            var v = new VarData();
            v.SetValue("x", ValueType.StringType, "3.14", 0d);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
            Assert.AreEqual("3.14", v.StringValue);
            // StringType では NumericValue は文字列から変換される
            Assert.AreEqual(3.14, v.NumericValue, 1e-9);
        }

        [TestMethod]
        public void SetValue_StringType_NonNumericString_NumericValueIsZero()
        {
            var v = new VarData();
            v.SetValue("x", ValueType.StringType, "hello", 0d);
            Assert.AreEqual("hello", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        // ──────────────────────────────────────────────
        // SetFrom
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetFrom_CopiesAllFields()
        {
            var src = new VarData("orig", ValueType.NumericType, "10", 10d);
            var dst = new VarData();
            dst.SetFrom(src);
            Assert.AreEqual("orig", dst.Name);
            Assert.AreEqual(ValueType.NumericType, dst.VariableType);
            Assert.AreEqual(10d, dst.NumericValue);
        }

        // ──────────────────────────────────────────────
        // ReferenceValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReferenceValue_AsNumeric_FromNumericVar_ReturnsNumericValue()
        {
            var v = new VarData("x", ValueType.NumericType, "", 5d);
            var resultType = v.ReferenceValue(ValueType.NumericType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, resultType);
            Assert.AreEqual(5d, num);
        }

        [TestMethod]
        public void ReferenceValue_AsString_FromStringVar_ReturnsStringValue()
        {
            var v = new VarData("x", ValueType.StringType, "hello", 0d);
            var resultType = v.ReferenceValue(ValueType.StringType, out var str, out var num);
            Assert.AreEqual(ValueType.StringType, resultType);
            Assert.AreEqual("hello", str);
        }

        [TestMethod]
        public void ReferenceValue_AsNumeric_FromStringVar_ConvertsToNumeric()
        {
            var v = new VarData("x", ValueType.StringType, "42", 42d);
            var resultType = v.ReferenceValue(ValueType.NumericType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, resultType);
            Assert.AreEqual(42d, num);
        }

        [TestMethod]
        public void ReferenceValue_AsString_FromNumericVar_FormatsNumber()
        {
            var v = new VarData("x", ValueType.NumericType, "", 100d);
            var resultType = v.ReferenceValue(ValueType.StringType, out var str, out var num);
            Assert.AreEqual(ValueType.StringType, resultType);
            Assert.IsNotNull(str);
            Assert.IsTrue(str.Contains("100"), $"Expected '100' in '{str}'");
        }

        [TestMethod]
        public void ReferenceValue_Undefined_FromStringVar_ReturnsStringType()
        {
            var v = new VarData("x", ValueType.StringType, "test", 0d);
            var resultType = v.ReferenceValue(ValueType.UndefinedType, out var str, out var num);
            Assert.AreEqual(ValueType.StringType, resultType);
            Assert.AreEqual("test", str);
        }

        [TestMethod]
        public void ReferenceValue_Undefined_FromNumericVar_ReturnsNumericType()
        {
            var v = new VarData("x", ValueType.NumericType, "", 7d);
            var resultType = v.ReferenceValue(ValueType.UndefinedType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, resultType);
            Assert.AreEqual(7d, num);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsNameAndValues()
        {
            var v = new VarData("score", ValueType.NumericType, "100", 100d);
            var str = v.ToString();
            Assert.IsTrue(str.Contains("score"), $"Expected 'score' in '{str}'");
        }
    }
}
