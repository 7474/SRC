using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCore.Expressions.Functions;
using static SRCCore.Coverage.Tests.ExprInfo.ExprInfoFixture;

namespace SRCCore.Coverage.Tests.ExprInfo
{
    /// <summary>
    /// Info 関数の戻り値の型を VB に合わせたことのテスト（7474/SRC#1498）。
    /// VB の EvalInfoFunc（Expression.bas）は途中の Exit Function でも文字列を返すだけで、
    /// 呼び出し元の CallFunction の "info" が etype が NumericType なら StrToDbl して NumericType を、
    /// それ以外なら StringType を返す。
    /// </summary>
    [TestClass]
    public class ExprInfoResultTypeVBMatchTests
    {
        private static ValueType Invoke(SRC src, ValueType etype, out string str_result, out double num_result, params string[] args)
        {
            var @params = new string[args.Length + 1];
            var is_term = new bool[args.Length + 1];
            for (var i = 0; i < args.Length; i++)
            {
                @params[i + 1] = args[i];
                is_term[i + 1] = true;
            }
            return new Info().Invoke(src, etype, @params, args.Length, is_term, out str_result, out num_result);
        }

        [TestMethod]
        public void 性別_NumericType_で呼ぶと数値に変換して返す()
        {
            var src = CreateSrc();
            CreatePilotedUnit(src);

            // VB: Case "性別" は Exit Function するが、呼び出し元で StrToDbl("男性") = 0 の NumericType になる
            var res = Invoke(src, ValueType.NumericType, out var str, out var num, "パイロット", "テストパイロット", "性別");

            Assert.AreEqual(ValueType.NumericType, res);
            Assert.AreEqual(0d, num);
        }

        [TestMethod]
        public void 性別_StringType_で呼ぶと文字列を返す()
        {
            var src = CreateSrc();
            CreatePilotedUnit(src);

            var res = Invoke(src, ValueType.StringType, out var str, out _, "パイロット", "テストパイロット", "性別");

            Assert.AreEqual(ValueType.StringType, res);
            Assert.AreEqual("男性", str);
        }

        [TestMethod]
        public void 第1引数が空文字_UndefinedType_で呼ぶと空の文字列を返す()
        {
            var src = CreateSrc();

            // VB: Case "" で Exit Function → 呼び出し元は etype が NumericType でないので StringType を返す。
            // UndefinedType を返すと CallFunction がユーザー定義関数の探索へ進んでしまう
            var res = Invoke(src, ValueType.UndefinedType, out var str, out var num, "", "名称");

            Assert.AreEqual(ValueType.StringType, res);
            Assert.AreEqual("", str);
            Assert.AreEqual(0d, num);
        }

        [TestMethod]
        public void 第1引数が空文字_NumericType_で呼ぶと0を返す()
        {
            var src = CreateSrc();

            var res = Invoke(src, ValueType.NumericType, out var str, out var num, "", "名称");

            Assert.AreEqual(ValueType.NumericType, res);
            Assert.AreEqual(0d, num);
        }
    }
}
