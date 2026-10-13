using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Expressions.Tests
{
    /// <summary>
    /// 括弧 1 文字の文字列（"(" や ")"）と比べる式。ロボダンのアップグレード画面（名称の折り返し）で使う
    /// </summary>
    [TestClass]
    public class ExpressionParenLiteralTests
    {
        private static Expression Create()
        {
            return new Expression(new SRC { GUI = new MockGUI() });
        }

        [TestMethod]
        public void CompareWithCloseParenLiteral()
        {
            var exp = Create();
            exp.SetVariableAsString("文章[2]", "・フルアーマー");
            Assert.AreEqual(0, exp.GetValueAsLong("Left(文章[2], 1) = \")\""));
            exp.SetVariableAsString("文章[2]", ")ABC");
            Assert.AreEqual(1, exp.GetValueAsLong("Left(文章[2], 1) = \")\""));
        }

        [TestMethod]
        public void CompareWithOpenParenLiteral()
        {
            var exp = Create();
            exp.SetVariableAsString("文章[1]", "ガンダム(");
            Assert.AreEqual(1, exp.GetValueAsLong("Right(文章[1], 1) = \"(\""));
        }
    }
}
