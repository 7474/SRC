using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.VB;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// VarData クラスの追加テスト（エッジケース・ReferenceValue の追加ケース）
    /// </summary>
    [TestClass]
    public class VarDataAdditionalTests
    {
        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_Default_AllFieldsEmpty()
        {
            var v = new VarData();
            Assert.AreEqual("", v.Name);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
        }

        [TestMethod]
        public void Constructor_WithName_SetsName()
        {
            var v = new VarData("test", ValueType.NumericType, "", 42d);
            Assert.AreEqual("test", v.Name);
        }

        // ──────────────────────────────────────────────
        // SetValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetValue_NumericType_LargeNumber()
        {
            var v = new VarData();
            v.SetValue("x", ValueType.NumericType, "", 999999d);
            Assert.AreEqual(999999d, v.NumericValue);
            Assert.AreEqual("999999", v.StringValue);
        }

        [TestMethod]
        public void SetValue_NumericType_NegativeNumber()
        {
            var v = new VarData();
            v.SetValue("x", ValueType.NumericType, "", -100d);
            Assert.AreEqual(-100d, v.NumericValue);
            Assert.AreEqual("-100", v.StringValue);
        }

        [TestMethod]
        public void SetValue_StringType_JapaneseString()
        {
            var v = new VarData();
            v.SetValue("name", ValueType.StringType, "テスト", 0d);
            Assert.AreEqual("テスト", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        [TestMethod]
        public void SetValue_StringType_NumericString_ParsesNumber()
        {
            var v = new VarData();
            v.SetValue("val", ValueType.StringType, "3.14", 0d);
            Assert.AreEqual(3.14d, v.NumericValue, 1e-10);
            Assert.AreEqual("3.14", v.StringValue);
        }

        [TestMethod]
        public void SetValue_StringType_InvalidNumericString_NumericZero()
        {
            var v = new VarData();
            v.SetValue("val", ValueType.StringType, "not-a-number", 0d);
            Assert.AreEqual(0d, v.NumericValue);
        }

        [TestMethod]
        public void SetValue_NumericType_FloatNumber()
        {
            var v = new VarData();
            v.SetValue("x", ValueType.NumericType, "", 0.5d);
            Assert.AreEqual(0.5d, v.NumericValue);
        }

        // ──────────────────────────────────────────────
        // Init
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Init_SetsNameAndClearsValues()
        {
            var v = new VarData("old", ValueType.NumericType, "", 100d);
            v.Init("new");
            Assert.AreEqual("new", v.Name);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        // ──────────────────────────────────────────────
        // Clear
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_ResetsToDefaults()
        {
            var v = new VarData("x", ValueType.NumericType, "", 42d);
            v.Clear();
            Assert.AreEqual("", v.Name);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        // ──────────────────────────────────────────────
        // SetFrom
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetFrom_CopiesAllProperties()
        {
            var source = new VarData("src", ValueType.NumericType, "100", 100d);
            var target = new VarData();
            target.SetFrom(source);

            Assert.AreEqual("src", target.Name);
            Assert.AreEqual(ValueType.NumericType, target.VariableType);
            Assert.AreEqual("100", target.StringValue);
            Assert.AreEqual(100d, target.NumericValue);
        }

        [TestMethod]
        public void SetFrom_StringType_CopiesStringValue()
        {
            var source = new VarData("msg", ValueType.StringType, "hello", 0d);
            var target = new VarData();
            target.SetFrom(source);

            Assert.AreEqual("msg", target.Name);
            Assert.AreEqual("hello", target.StringValue);
        }

        [TestMethod]
        public void SetFrom_DoesNotSharedReferenceWithSource()
        {
            var source = new VarData("original", ValueType.NumericType, "", 1d);
            var target = new VarData();
            target.SetFrom(source);

            // Changing source should not affect target
            source.SetValue("modified", ValueType.NumericType, "", 999d);
            Assert.AreEqual("original", target.Name);
            Assert.AreEqual(1d, target.NumericValue);
        }

        // ──────────────────────────────────────────────
        // ReferenceValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReferenceValue_Numeric_NumericVar_ReturnsNumericValue()
        {
            var v = new VarData("x", ValueType.NumericType, "", 42d);
            var type = v.ReferenceValue(ValueType.NumericType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, type);
            Assert.AreEqual(42d, num);
        }

        [TestMethod]
        public void ReferenceValue_String_NumericVar_FormatsNumber()
        {
            var v = new VarData("x", ValueType.NumericType, "", 42d);
            var type = v.ReferenceValue(ValueType.StringType, out var str, out var num);
            Assert.AreEqual(ValueType.StringType, type);
            Assert.AreEqual("42", str);
        }

        [TestMethod]
        public void ReferenceValue_Numeric_StringVar_ParsesString()
        {
            var v = new VarData("x", ValueType.StringType, "3.14", 0d);
            v.SetValue("x", ValueType.StringType, "3.14", 0d);
            var type = v.ReferenceValue(ValueType.NumericType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, type);
            Assert.AreEqual(3.14d, num, 1e-10);
        }

        [TestMethod]
        public void ReferenceValue_String_StringVar_ReturnsStringValue()
        {
            var v = new VarData("name", ValueType.StringType, "テスト", 0d);
            var type = v.ReferenceValue(ValueType.StringType, out var str, out var num);
            Assert.AreEqual(ValueType.StringType, type);
            Assert.AreEqual("テスト", str);
        }

        [TestMethod]
        public void ReferenceValue_Undefined_StringVar_ReturnsString()
        {
            var v = new VarData("x", ValueType.StringType, "hello", 0d);
            var type = v.ReferenceValue(ValueType.UndefinedType, out var str, out var num);
            Assert.AreEqual(ValueType.StringType, type);
            Assert.AreEqual("hello", str);
        }

        [TestMethod]
        public void ReferenceValue_Undefined_NumericVar_ReturnsNumeric()
        {
            var v = new VarData("x", ValueType.NumericType, "", 99d);
            var type = v.ReferenceValue(ValueType.UndefinedType, out var str, out var num);
            Assert.AreEqual(ValueType.NumericType, type);
            Assert.AreEqual(99d, num);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ContainsNameAndValues()
        {
            var v = new VarData("myVar", ValueType.NumericType, "100", 100d);
            var str = v.ToString();
            Assert.IsTrue(str.Contains("myVar"), $"Expected 'myVar' in '{str}'");
        }

        [TestMethod]
        public void ToString_EmptyName_ReturnsFormattedString()
        {
            var v = new VarData();
            var str = v.ToString();
            Assert.IsNotNull(str);
        }
    }
}
