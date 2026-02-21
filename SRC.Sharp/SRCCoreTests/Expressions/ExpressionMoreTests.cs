using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression のさらなる追加テスト
    /// （既存の式テストファイル未カバー分）
    /// </summary>
    [TestClass]
    public class ExpressionMoreTests
    {
        private Expression Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            return new Expression(src);
        }

        // ──────────────────────────────────────────────
        // 文字列連結: 複数項・変数を含む
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Concatenate_ThreeStrings_ReturnsJoined()
        {
            var exp = Create();
            Assert.AreEqual("abc", exp.GetValueAsString("\"a\" & \"b\" & \"c\""));
        }

        [TestMethod]
        public void Concatenate_VariableAndLiteral_ReturnsJoined()
        {
            var exp = Create();
            exp.SetVariableAsString("hp", "100");
            Assert.AreEqual("HP:100", exp.GetValueAsString("\"HP:\" & hp"));
        }

        [TestMethod]
        public void Concatenate_NumericVariableAndString_ReturnsJoined()
        {
            var exp = Create();
            exp.SetVariableAsDouble("score", 42d);
            Assert.AreEqual("score=42", exp.GetValueAsString("\"score=\" & score"));
        }

        [TestMethod]
        public void Concatenate_FourParts_ReturnsAllJoined()
        {
            var exp = Create();
            Assert.AreEqual("1234", exp.GetValueAsString("\"1\" & \"2\" & \"3\" & \"4\""));
        }

        // ──────────────────────────────────────────────
        // 変数を関数引数に使う
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_OfVariable_ReturnsLength()
        {
            var exp = Create();
            exp.SetVariableAsString("str", "hello");
            Assert.AreEqual(5d, exp.GetValueAsDouble("Len(str)"));
        }

        [TestMethod]
        public void Left_OfVariable_ReturnsPrefix()
        {
            var exp = Create();
            exp.SetVariableAsString("name", "アムロ");
            Assert.AreEqual("ア", exp.GetValueAsString("Left(name,1)"));
        }

        [TestMethod]
        public void Abs_OfVariable_ReturnsAbsolute()
        {
            var exp = Create();
            exp.SetVariableAsDouble("val", -7d);
            Assert.AreEqual(7d, exp.GetValueAsDouble("Abs(val)"));
        }

        [TestMethod]
        public void Int_OfVariable_ReturnsFloor()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 3.7d);
            Assert.AreEqual(3d, exp.GetValueAsDouble("Int(x)"));
        }

        // ──────────────────────────────────────────────
        // 関数のネスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Len_OfLeft_ReturnsNestedResult()
        {
            var exp = Create();
            // Left("hello",3) = "hel", Len("hel") = 3
            Assert.AreEqual(3d, exp.GetValueAsDouble("Len(Left(\"hello\",3))"));
        }

        [TestMethod]
        public void Abs_OfSubtraction_ReturnsPositive()
        {
            var exp = Create();
            // Abs(3 - 7) = Abs(-4) = 4
            Assert.AreEqual(4d, exp.GetValueAsDouble("Abs(3 - 7)"));
        }

        [TestMethod]
        public void Max_WithArithmeticArgs_ReturnsMax()
        {
            var exp = Create();
            // Max(6,7) = 7
            Assert.AreEqual(7d, exp.GetValueAsDouble("Max(6,7)"));
        }

        // ──────────────────────────────────────────────
        // ReplaceSubExpression: 算術式を含む $() 置換
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplaceSubExpression_StringVariable_ReplacesCorrectly()
        {
            var exp = Create();
            exp.SetVariableAsString("unit", "ガンダム");
            var str = "機体:$(unit)";
            exp.ReplaceSubExpression(ref str);
            Assert.AreEqual("機体:ガンダム", str);
        }

        [TestMethod]
        public void ReplaceSubExpression_NumericVariable_ReplacesWithFormattedValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("hp", 100d);
            var str = "HP=$(hp)";
            exp.ReplaceSubExpression(ref str);
            Assert.AreEqual("HP=100", str);
        }

        [TestMethod]
        public void ReplaceSubExpression_ThreeVariables_AllReplaced()
        {
            var exp = Create();
            exp.SetVariableAsString("a", "あ");
            exp.SetVariableAsString("b", "い");
            exp.SetVariableAsString("c", "う");
            var str = "$(a)$(b)$(c)";
            exp.ReplaceSubExpression(ref str);
            Assert.AreEqual("あいう", str);
        }

        // ──────────────────────────────────────────────
        // FormatMessage: ダッシュ変換パターン（未カバー variant）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FormatMessage_LongVowelDouble_ConvertedToHorizontalLine()
        {
            var exp = Create();
            var msg = "テストーーテスト";
            exp.FormatMessage(ref msg);
            // 「ーー」は「──」に変換される
            Assert.AreEqual("テスト──テスト", msg);
        }

        [TestMethod]
        public void FormatMessage_MixedDash1_ConvertedToHorizontalLine()
        {
            var exp = Create();
            // 「─―」（ U+2500 + U+2015）は「──」に変換される
            var msg = "A─―B";
            exp.FormatMessage(ref msg);
            Assert.AreEqual("A──B", msg);
        }

        [TestMethod]
        public void FormatMessage_MixedDash2_ConvertedToHorizontalLine()
        {
            var exp = Create();
            // 「─ー」（ U+2500 + ー）は「──」に変換される
            var msg = "A─ーB";
            exp.FormatMessage(ref msg);
            Assert.AreEqual("A──B", msg);
        }

        [TestMethod]
        public void FormatMessage_MultipleDashes_AllConverted()
        {
            var exp = Create();
            var msg = "――ーー";
            exp.FormatMessage(ref msg);
            Assert.AreEqual("────", msg);
        }

        // ──────────────────────────────────────────────
        // 変数スコープ: 変数を上書きした場合の動作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Variable_UpdateThenUseInExpr_UsesNewValue()
        {
            var exp = Create();
            exp.SetVariableAsDouble("x", 10d);
            exp.SetVariableAsDouble("x", 20d);
            Assert.AreEqual(40d, exp.GetValueAsDouble("x * 2"));
        }

        [TestMethod]
        public void Variable_StringAndNumericSameName_LastWins()
        {
            var exp = Create();
            exp.SetVariableAsString("v", "hello");
            exp.SetVariableAsDouble("v", 99d);
            // 上書き後は数値型として取得できる
            Assert.AreEqual(99d, exp.GetValueAsDouble("v"));
        }

        // ──────────────────────────────────────────────
        // 複雑な算術式
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ComplexArithmetic_VariablesAndLiterals()
        {
            var exp = Create();
            exp.SetVariableAsDouble("a", 3d);
            exp.SetVariableAsDouble("b", 4d);
            // ピタゴラス: a^2 + b^2 = 9 + 16 = 25
            Assert.AreEqual(25d, exp.GetValueAsDouble("a ^ 2 + b ^ 2"));
        }

        [TestMethod]
        public void ComplexArithmetic_ModuloInExpression()
        {
            var exp = Create();
            exp.SetVariableAsDouble("n", 17d);
            Assert.AreEqual(2d, exp.GetValueAsDouble("n Mod 5"));
        }

        [TestMethod]
        public void ComplexArithmetic_IntDivisionWithVariable()
        {
            var exp = Create();
            exp.SetVariableAsDouble("total", 100d);
            exp.SetVariableAsDouble("count", 3d);
            Assert.AreEqual(33d, exp.GetValueAsDouble("total \\ count"));
        }

        // ──────────────────────────────────────────────
        // FormatMessage: サブ式置換と結合
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FormatMessage_VariableAndArithmetic()
        {
            var exp = Create();
            exp.SetVariableAsDouble("hp", 50d);
            exp.SetVariableAsDouble("maxhp", 100d);
            var msg = "HP:$(hp)/$(maxhp)";
            exp.FormatMessage(ref msg);
            Assert.AreEqual("HP:50/100", msg);
        }

        [TestMethod]
        public void FormatMessage_EmptyString_UnchangedAndNoThrow()
        {
            var exp = Create();
            var msg = "";
            exp.FormatMessage(ref msg);
            Assert.AreEqual("", msg);
        }
    }
}
