using SRCCore.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Lib.Tests
{
    [TestClass()]
    public class GeneralLibTests
    {
        struct ToListTestCase
        {
            public string input;
            public IList<string> expected;
        }
        [TestMethod()]
        public void ToListTest()
        {
            // TODO カッコの扱いちゃんと見る
            var cases = new ToListTestCase[]
            {
                new ToListTestCase{ input= "いちご ニンジン サンダル", expected= new List<string>{ "いちご", "ニンジン", "サンダル" } },
                new ToListTestCase{ input= "This is a pen.", expected= new List<string>{ "This", "is", "a", "pen." } },
                //new ToListTestCase{ input= "a (b c) (d (e f))", expected= new List<string>{ "a", "b c", "d (e f)" } },
                new ToListTestCase{ input= "こぶた たぬき きつね ねこ", expected= new List<string>{ "こぶた", "たぬき", "きつね", "ねこ" } },
                //new ToListTestCase{ input= "a (b c)) (d (e f))", expected= new List<string>{ "a", "b c", ") (d (e f))" } },
            };
            foreach (var c in cases)
            {
                var actual = GeneralLib.ToList(c.input);
                Console.WriteLine(c.input + ": " + JsonConvert.SerializeObject(actual));
                Assert.IsTrue(c.expected.SequenceEqual(actual), $"case: {c.input}");
            }
        }

        [TestMethod()]
        public void FormatNumTest()
        {
            Assert.AreEqual("100000000000000000000", GeneralLib.FormatNum(1e20));
            Assert.AreEqual("0.1", GeneralLib.FormatNum(1e-1));
        }

        [TestMethod()]
        public void StrWidthTest()
        {
            Assert.AreEqual(0, GeneralLib.StrWidth(null));
            Assert.AreEqual(0, GeneralLib.StrWidth(""));
            Assert.AreEqual(3, GeneralLib.StrWidth("123"));
            Assert.AreEqual(4, GeneralLib.StrWidth("1２3"));
            Assert.AreEqual(6, GeneralLib.StrWidth("１２３"));
        }
    }
}
