using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// 元の SRC（VarData.cls と、Expression.bas の CallFunction の「実はシステム定義のグローバル変数？」）と突き合わせたテスト
    /// </summary>
    [TestClass]
    public class VarDataVBMatchTests
    {
        // VB の VarData は Name・VariableType・StringValue・NumericValue を持つだけのクラスで、
        // SetVariable は渡した値をそのまま入れる（もう一方の型の値を作らない）
        [TestMethod]
        public void SetValue_NumericType_KeepsStringValueAsPassed()
        {
            var v = new VarData("x", ValueType.NumericType, "", 5d);
            Assert.AreEqual("", v.StringValue);
            Assert.AreEqual(5d, v.NumericValue);
        }

        [TestMethod]
        public void SetValue_StringType_KeepsNumericValueAsPassed()
        {
            var v = new VarData("x", ValueType.StringType, "12", 0d);
            Assert.AreEqual("12", v.StringValue);
            Assert.AreEqual(0d, v.NumericValue);
        }

        // VB の Term は用語のグローバル変数の StringValue をそのまま読む。
        // 数値で入れた変数（SetVariableAsDouble → StringValue = ""）なら空になる
        [TestMethod]
        public void Term_NumericGlobalVariable_ReadsStringValueAsPassed()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Expression.DefineGlobalVariable("Term(気力)");
            src.Expression.SetVariableAsDouble("Term(気力)", 5d);

            Assert.AreEqual("", src.Expression.Term("気力"));
        }

        // CallFunction の「実はシステム定義のグローバル変数？」で文字列を求めたとき、
        // VB は VariableType = StringType なら StringValue、それ以外（UndefinedType も）は FormatNum(NumericValue)
        [TestMethod]
        public void ReferenceValue_StringRequest_UndefinedTypeVar_FormatsNumericValue()
        {
            var v = new VarData("u", ValueType.UndefinedType, "rawStr", 99d);
            var type = v.ReferenceValue(ValueType.StringType, out var str, out _);
            Assert.AreEqual(ValueType.StringType, type);
            Assert.AreEqual("99", str);
        }

        // 数値を求めたとき、文字列の変数は StrToDbl で読む（IsNumeric でない「NaN」は 0）
        [TestMethod]
        public void ReferenceValue_NumericRequest_StringVar_UsesStrToDbl()
        {
            var v = new VarData("s", ValueType.StringType, "NaN", 0d);
            var type = v.ReferenceValue(ValueType.NumericType, out _, out var num);
            Assert.AreEqual(ValueType.NumericType, type);
            Assert.AreEqual(0d, num);
        }
    }
}
