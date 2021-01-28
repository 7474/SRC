using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRC.Core.Expressions;
using SRCCoreTests.TestLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRC.Core.Expressions.Tests
{
    [TestClass()]
    public class ExpressionTests
    {
        private Expression Create()
        {
            return new Expression(new SRC()
            {
                GUI = new MockGUI(),
            });
        }

        [TestMethod()]
        public void GetValueAsStringTest()
        {
            var exp = Create();

            exp.SetVariableAsString("文字列変数名", "文字列値");
            Assert.AreEqual("文字列値", exp.GetValueAsString("文字列変数名"));
        }
    }
}