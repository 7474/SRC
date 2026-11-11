using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using System;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// Expression.bas EvalExpr を元の SRC（VB 版）と突き合わせたテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class ExpressionEvalVBMatchTests
    {
        private Expression Create()
        {
            var src = new SRC
            {
                GUI = new MockGUI(),
            };
            return new Expression(src);
        }

        [TestMethod]
        public void EvalExpr_TrailingOperator_IsNotEvaluated()
        {
            // VB: 演算子を探すのは For i = 1 To tnum - 1。最後の項が演算子でも演算はせず、式をそのまま文字列にする
            var exp = Create();
            Assert.AreEqual("1 +", exp.GetValueAsString("1 +"));
            Assert.AreEqual("a =", exp.GetValueAsString("a ="));
            Assert.AreEqual("x Or", exp.GetValueAsString("x Or"));
            // 数値として読むときは num_result に何も入れないので 0
            Assert.AreEqual(0d, exp.GetValueAsDouble("1 +"));
        }

        [TestMethod]
        public void EvalExpr_OperatorBeforeLastTerm_IsStillEvaluated()
        {
            // 最後から 2 番目までの項は演算子として見る
            var exp = Create();
            Assert.AreEqual(3d, exp.GetValueAsDouble("1 + 2"));
            Assert.AreEqual("1", exp.GetValueAsString("Not 0"));
        }

        [TestMethod]
        public void Mod_FractionalOperands_RoundsBothToEvenBeforeDividing()
        {
            // VB: num_result = lnum Mod rnum。Mod は両辺を偶数丸めで Long にしてから割った余り
            var exp = Create();
            // 7.5 → 8、8 Mod 2 = 0（実数の余りなら 1.5）
            Assert.AreEqual(0d, exp.GetValueAsDouble("7.5 Mod 2"));
            // 2.5 → 2（偶数丸め）、7 Mod 2 = 1（実数の余りなら 2）
            Assert.AreEqual(1d, exp.GetValueAsDouble("7 Mod 2.5"));
            // -7.5 → -8、-8 Mod 3 = -2（符号は被除数に合わせる）
            Assert.AreEqual(-2d, exp.GetValueAsDouble("-7.5 Mod 3"));
            Assert.AreEqual("0", exp.GetValueAsString("7.5 Mod 2"));
        }

        [TestMethod]
        public void Mod_DivisorRoundsToZero_Throws()
        {
            // VB: EvalExpr の Mod は \ と違って rnum <> 0 を確かめないので、除数が丸めて 0 なら実行時エラー 11。
            // イベントコマンドの実行（CmdData.Exec）の中ではエラーとして扱われる
            var exp = Create();
            Assert.ThrowsExactly<DivideByZeroException>(() => exp.GetValueAsDouble("5 Mod 0"));
            Assert.ThrowsExactly<DivideByZeroException>(() => exp.GetValueAsDouble("5 Mod 0.4"));
        }
    }
}
