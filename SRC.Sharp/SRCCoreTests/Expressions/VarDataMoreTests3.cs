using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;

namespace SRCCore.Expressions.Tests
{
    [TestClass]
    public class VarDataMoreTests3
    {
        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefaultConstructor_ClearsToEmpty()
        {
            var v = new VarData();
            Assert.AreEqual("", v.Name);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
        }

        // ──────────────────────────────────────────────
        // Init
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Init_SetsNameAndClearsValues()
        {
            var v = new VarData("old", ValueType.NumericType, "5", 5d);
            v.Init("newName");
            Assert.AreEqual("newName", v.Name);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
        }

        // ──────────────────────────────────────────────
        // SetValue
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetValue_StringType_StoresStringValue()
        {
            var v = new VarData();
            v.SetValue("s", ValueType.StringType, "hello", 0d);
            Assert.AreEqual("hello", v.StringValue);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
        }

        [TestMethod]
        public void SetValue_NumericType_StoresNumericValue()
        {
            var v = new VarData();
            v.SetValue("n", ValueType.NumericType, "", 42d);
            Assert.AreEqual(42d, v.NumericValue);
            Assert.AreEqual(ValueType.NumericType, v.VariableType);
        }

        [TestMethod]
        public void SetValue_NegativeNumericValue_Stored()
        {
            var v = new VarData();
            v.SetValue("n", ValueType.NumericType, "", -99d);
            Assert.AreEqual(-99d, v.NumericValue);
        }

        [TestMethod]
        public void SetValue_ZeroNumericValue_Stored()
        {
            var v = new VarData();
            v.SetValue("n", ValueType.NumericType, "", 0d);
            Assert.AreEqual(0d, v.NumericValue);
        }

        [TestMethod]
        public void SetValue_JapaneseStringValue_Stored()
        {
            var v = new VarData();
            v.SetValue("j", ValueType.StringType, "日本語テスト", 0d);
            Assert.AreEqual("日本語テスト", v.StringValue);
        }

        // ──────────────────────────────────────────────
        // Clear
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_ResetsToEmpty()
        {
            var v = new VarData("test", ValueType.NumericType, "10", 10d);
            v.Clear();
            Assert.AreEqual("", v.Name);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
            Assert.AreEqual(ValueType.StringType, v.VariableType);
        }

        // ──────────────────────────────────────────────
        // SetFrom
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SetFrom_CopiesAllFields()
        {
            var src = new VarData("source", ValueType.NumericType, "5", 5d);
            var dst = new VarData();
            dst.SetFrom(src);
            Assert.AreEqual("source", dst.Name);
            Assert.AreEqual(ValueType.NumericType, dst.VariableType);
            Assert.AreEqual(src.StringValue, dst.StringValue);
            Assert.AreEqual(5d, dst.NumericValue);
        }

        // ──────────────────────────────────────────────
        // ToString
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ToString_ReturnsNonNull()
        {
            var v = new VarData("x", ValueType.NumericType, "1", 1d);
            Assert.IsNotNull(v.ToString());
        }

        [TestMethod]
        public void ToString_ContainsName()
        {
            var v = new VarData("myVar", ValueType.StringType, "hello", 0d);
            Assert.IsTrue(v.ToString().Contains("myVar"));
        }
    }
}
