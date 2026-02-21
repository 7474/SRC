using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// VarData クラスの追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class VarDataEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // ValueType enum
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ValueType_AllValues_AreDistinct()
        {
            var values = System.Enum.GetValues(typeof(ValueType));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (ValueType v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複: {v}={v:D}");
            }
        }

        [TestMethod]
        public void ValueType_NumericType_NotEqualTo_StringType()
        {
            Assert.AreNotEqual(ValueType.NumericType, ValueType.StringType);
        }

        [TestMethod]
        public void ValueType_UndefinedType_NotEqualTo_NumericType()
        {
            Assert.AreNotEqual(ValueType.UndefinedType, ValueType.NumericType);
        }

        // ──────────────────────────────────────────────
        // VarData - コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefaultConstructor_ClearsToStringType()
        {
            var vd = new VarData();
            // Clear() -> Init("") -> SetValue("", StringType, "", 0d)
            Assert.AreEqual(ValueType.StringType, vd.VariableType);
            Assert.AreEqual("", vd.StringValue);
            Assert.AreEqual(0d, vd.NumericValue);
        }

        [TestMethod]
        public void ParamConstructor_NumericType_SetsCorrectly()
        {
            var vd = new VarData("score", ValueType.NumericType, "", 100d);
            Assert.AreEqual("score", vd.Name);
            Assert.AreEqual(ValueType.NumericType, vd.VariableType);
            Assert.AreEqual(100d, vd.NumericValue);
        }

        [TestMethod]
        public void ParamConstructor_StringType_SetsCorrectly()
        {
            var vd = new VarData("name", ValueType.StringType, "テスト", 0d);
            Assert.AreEqual("name", vd.Name);
            Assert.AreEqual(ValueType.StringType, vd.VariableType);
            Assert.AreEqual("テスト", vd.StringValue);
        }

        // ──────────────────────────────────────────────
        // SetValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetValue_NumericType_SetsStringValueToFormatted()
        {
            var vd = new VarData();
            vd.SetValue("x", ValueType.NumericType, "", 3.14);
            Assert.AreEqual(ValueType.NumericType, vd.VariableType);
            Assert.AreEqual(3.14, vd.NumericValue, 0.001);
            Assert.IsNotNull(vd.StringValue); // StringValue は数値を文字列変換した値
        }

        [TestMethod]
        public void SetValue_StringType_SetsNumericValueFromParse()
        {
            var vd = new VarData();
            vd.SetValue("y", ValueType.StringType, "42", 0d);
            Assert.AreEqual(ValueType.StringType, vd.VariableType);
            Assert.AreEqual("42", vd.StringValue);
            Assert.AreEqual(42d, vd.NumericValue, 0.001); // 文字列から数値に変換
        }

        [TestMethod]
        public void SetValue_StringType_NonNumericString_NumericIsZero()
        {
            var vd = new VarData();
            vd.SetValue("z", ValueType.StringType, "abc", 0d);
            Assert.AreEqual("abc", vd.StringValue);
            Assert.AreEqual(0d, vd.NumericValue, 0.001);
        }

        [TestMethod]
        public void SetValue_NegativeNumeric_SetsCorrectly()
        {
            var vd = new VarData();
            vd.SetValue("n", ValueType.NumericType, "", -7.5);
            Assert.AreEqual(-7.5, vd.NumericValue, 0.001);
        }

        [TestMethod]
        public void SetValue_ZeroNumeric_SetsCorrectly()
        {
            var vd = new VarData();
            vd.SetValue("z", ValueType.NumericType, "", 0d);
            Assert.AreEqual(0d, vd.NumericValue);
        }

        // ──────────────────────────────────────────────
        // SetFrom
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetFrom_CopiesAllFields()
        {
            var src = new VarData("score", ValueType.NumericType, "", 99d);
            var dst = new VarData();
            dst.SetFrom(src);
            Assert.AreEqual(src.Name, dst.Name);
            Assert.AreEqual(src.VariableType, dst.VariableType);
            Assert.AreEqual(src.NumericValue, dst.NumericValue);
            Assert.AreEqual(src.StringValue, dst.StringValue);
        }

        // ──────────────────────────────────────────────
        // Clear / Init
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_ResetsToDefaultStringType()
        {
            var vd = new VarData("score", ValueType.NumericType, "", 100d);
            vd.Clear();
            Assert.AreEqual("", vd.Name);
            Assert.AreEqual(ValueType.StringType, vd.VariableType);
            Assert.AreEqual("", vd.StringValue);
        }

        [TestMethod]
        public void Init_SetsName_ResetsValues()
        {
            var vd = new VarData("old", ValueType.NumericType, "", 100d);
            vd.Init("newname");
            Assert.AreEqual("newname", vd.Name);
            Assert.AreEqual(ValueType.StringType, vd.VariableType);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsName()
        {
            var vd = new VarData("myvar", ValueType.StringType, "hello", 0d);
            var str = vd.ToString();
            Assert.IsTrue(str.Contains("myvar"), $"Expected 'myvar' in '{str}'");
        }

        [TestMethod]
        public void ToString_ContainsStringValue()
        {
            var vd = new VarData("v", ValueType.StringType, "world", 0d);
            var str = vd.ToString();
            Assert.IsTrue(str.Contains("world"), $"Expected 'world' in '{str}'");
        }

        // ──────────────────────────────────────────────
        // ReferenceValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReferenceValue_NumericRequest_ReturnsNumericType()
        {
            var vd = new VarData("n", ValueType.NumericType, "", 42d);
            var result = vd.ReferenceValue(ValueType.NumericType, out string str, out double num);
            Assert.AreEqual(ValueType.NumericType, result);
            Assert.AreEqual(42d, num);
        }

        [TestMethod]
        public void ReferenceValue_StringRequest_ReturnsStringType()
        {
            var vd = new VarData("s", ValueType.StringType, "hello", 0d);
            var result = vd.ReferenceValue(ValueType.StringType, out string str, out double num);
            Assert.AreEqual(ValueType.StringType, result);
            Assert.AreEqual("hello", str);
        }

        [TestMethod]
        public void ReferenceValue_UndefinedRequest_NumericVar_ReturnsNumericType()
        {
            var vd = new VarData("n", ValueType.NumericType, "", 77d);
            var result = vd.ReferenceValue(ValueType.UndefinedType, out string str, out double num);
            Assert.AreEqual(ValueType.NumericType, result);
            Assert.AreEqual(77d, num);
        }
    }
}
