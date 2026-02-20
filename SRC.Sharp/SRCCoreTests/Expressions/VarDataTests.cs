using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// VarData クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class VarDataTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefaultConstructor_InitializesEmpty()
        {
            var v = new VarData();
            Assert.AreEqual("", v.Name);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
        }

        [TestMethod]
        public void ParameterizedConstructor_SetsValues()
        {
            var v = new VarData("myVar", ValueType.NumericType, "42", 42d);
            Assert.AreEqual("myVar", v.Name);
            Assert.AreEqual(ValueType.NumericType, v.VariableType);
        }

        // ──────────────────────────────────────────────
        // SetValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetValue_NumericType_SetsStringFromNumber()
        {
            var v = new VarData();
            v.SetValue("x", ValueType.NumericType, "", 3.14);
            Assert.AreEqual("x", v.Name);
            Assert.AreEqual(ValueType.NumericType, v.VariableType);
            Assert.AreEqual(3.14, v.NumericValue);
            // NumericType の場合は StringValue も数値から生成される
            Assert.IsFalse(string.IsNullOrEmpty(v.StringValue));
        }

        [TestMethod]
        public void SetValue_StringType_SetsNumericFromString()
        {
            var v = new VarData();
            v.SetValue("s", ValueType.StringType, "100", 0d);
            Assert.AreEqual("s", v.Name);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
            Assert.AreEqual("100", v.StringValue);
            Assert.AreEqual(100d, v.NumericValue);
        }

        [TestMethod]
        public void SetValue_StringType_NonNumericString_NumericValueIsZero()
        {
            var v = new VarData();
            v.SetValue("s", ValueType.StringType, "abc", 0d);
            Assert.AreEqual("abc", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        [TestMethod]
        public void SetValue_UndefinedType_StoredAsIs()
        {
            var v = new VarData();
            v.SetValue("u", ValueType.UndefinedType, "val", 5d);
            Assert.AreEqual(ValueType.UndefinedType, v.VariableType);
            Assert.AreEqual("val", v.StringValue);
            Assert.AreEqual(5d, v.NumericValue);
        }

        // ──────────────────────────────────────────────
        // Clear / Init
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_ResetsToEmpty()
        {
            var v = new VarData("x", ValueType.NumericType, "1", 1d);
            v.Clear();
            Assert.AreEqual("", v.Name);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        [TestMethod]
        public void Init_SetsNameOnly()
        {
            var v = new VarData("x", ValueType.NumericType, "5", 5d);
            v.Init("newName");
            Assert.AreEqual("newName", v.Name);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        // ──────────────────────────────────────────────
        // SetFrom
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetFrom_CopiesAllFields()
        {
            var src = new VarData("original", ValueType.NumericType, "99", 99d);
            var dst = new VarData();
            dst.SetFrom(src);

            Assert.AreEqual("original", dst.Name);
            Assert.AreEqual(ValueType.NumericType, dst.VariableType);
            Assert.AreEqual("99", dst.StringValue);
            Assert.AreEqual(99d, dst.NumericValue);
        }

        // ──────────────────────────────────────────────
        // ReferenceValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReferenceValue_NumericRequest_ReturnsNumber()
        {
            var v = new VarData("x", ValueType.NumericType, "", 42d);
            var type = v.ReferenceValue(ValueType.NumericType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, type);
            Assert.AreEqual(42d, num);
        }

        [TestMethod]
        public void ReferenceValue_StringRequest_ReturnsString()
        {
            var v = new VarData("s", ValueType.StringType, "hello", 0d);
            var type = v.ReferenceValue(ValueType.StringType, out var str, out var num);
            Assert.AreEqual(ValueType.StringType, type);
            Assert.AreEqual("hello", str);
        }

        [TestMethod]
        public void ReferenceValue_UndefinedRequest_StringTypeVar_ReturnsString()
        {
            var v = new VarData("s", ValueType.StringType, "test", 0d);
            var type = v.ReferenceValue(ValueType.UndefinedType, out var str, out var num);
            Assert.AreEqual(ValueType.StringType, type);
            Assert.AreEqual("test", str);
        }

        [TestMethod]
        public void ReferenceValue_UndefinedRequest_NumericTypeVar_ReturnsNumeric()
        {
            var v = new VarData("n", ValueType.NumericType, "", 7d);
            var type = v.ReferenceValue(ValueType.UndefinedType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, type);
            Assert.AreEqual(7d, num);
        }

        [TestMethod]
        public void ReferenceValue_StringRequest_NumericTypeVar_FormatsNumber()
        {
            var v = new VarData("n", ValueType.NumericType, "", 3.5);
            var type = v.ReferenceValue(ValueType.StringType, out var str, out var num);
            Assert.AreEqual(ValueType.StringType, type);
            Assert.IsFalse(string.IsNullOrEmpty(str));
        }

        [TestMethod]
        public void ReferenceValue_NumericRequest_StringTypeVar_ParsesString()
        {
            var v = new VarData("s", ValueType.StringType, "123", 0d);
            var type = v.ReferenceValue(ValueType.NumericType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, type);
            Assert.AreEqual(123d, num);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ReturnsFormattedString()
        {
            var v = new VarData("myVar", ValueType.StringType, "hello", 0d);
            var s = v.ToString();
            Assert.IsTrue(s.Contains("myVar"));
            Assert.IsTrue(s.Contains("hello"));
        }
    }
}
