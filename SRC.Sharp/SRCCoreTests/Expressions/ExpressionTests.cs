using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Expressions;
using SRCCoreTests.TestLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCCore.Expressions.Tests
{
    [TestClass()]
    public class ExpressionTests
    {
        private (Expression, SRC) Create()
        {
            var src = new SRC()
            {
                GUI = new MockGUI(),
            };
            return (new Expression(src), src);
        }

        [TestMethod()]
        public void GetValueAsStringTest()
        {
            var (exp, src) = Create();

            exp.SetVariableAsString("文字列変数名", "文字列値");
            Assert.AreEqual("文字列値", exp.GetValueAsString("文字列変数名"));

            // XXX case: term = true
        }

        [TestMethod()]
        public void SetVariableTest()
        {
            var (exp, src) = Create();

            exp.SetVariable("フラグ名", ValueType.UndefinedType, "001", 1d);
            Assert.AreEqual(1d, exp.GetValueAsDouble("フラグ名"));
            Assert.AreEqual("1", exp.GetValueAsString("フラグ名"));
            exp.SetVariable("フラグ名", ValueType.StringType, "001", 1d);
            Assert.AreEqual(1d, exp.GetValueAsDouble("フラグ名"));
            Assert.AreEqual("001", exp.GetValueAsString("フラグ名"));
            exp.SetVariable("フラグ名", ValueType.NumericType, "001", 1d);
            Assert.AreEqual(1d, exp.GetValueAsDouble("フラグ名"));
            Assert.AreEqual("1", exp.GetValueAsString("フラグ名"));

            exp.SetVariable("添え字", ValueType.NumericType, "", 10);
            exp.SetVariable("PrevX[添え字]", ValueType.NumericType, "", 123);
            Debug.WriteLine(exp.DumpVariables());
            Assert.AreEqual(123, exp.GetValueAsDouble("PrevX[添え字]", true));
            Assert.AreEqual(123, exp.GetValueAsDouble("PrevX[10]", true));
            Assert.AreEqual(0, exp.GetValueAsDouble("PrevX[\"添え字\"]", true));

            // XXX いろいろ
        }
    }
}