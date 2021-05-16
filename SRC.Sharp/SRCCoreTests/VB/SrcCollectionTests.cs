using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRCCore.VB.Tests
{
    [TestClass()]
    public class SrcCollectionTests
    {
        [TestMethod()]
        public void SrcCollectionTest()
        {
            var sc = new SrcCollection<object>();
            Assert.IsNotNull(sc);
        }

        [TestMethod()]
        public void IndexerListTest()
        {
            var sc = new SrcCollection<int>
            {
                ["1"] = 1,
                ["2"] = 2,
                ["3"] = 3,
            };

            Assert.AreEqual(1, sc[1]);
            Assert.AreEqual(2, sc[2]);
            Assert.AreEqual(3, sc[3]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => sc[0]);
            Assert.ThrowsException<IndexOutOfRangeException>(() => sc[4]);
        }

        [TestMethod()]
        public void IndexerDictTest()
        {
            var sc = new SrcCollection<string>
            {
                ["1"] = "1",
                ["Two"] = "2",
                ["Ｔｒｅｅ"] = "3",
            };

            Assert.AreEqual("1", sc["1"]);
            Assert.AreEqual("2", sc["two"]);
            Assert.AreEqual("3", sc["tree"]);
            Assert.AreEqual(null, sc[""]);
            Assert.AreEqual(null, sc["2"]);
        }

        [TestMethod()]
        [Ignore]
        public void AddTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void AddTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void AddTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void AddTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void AddTest4()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void ClearTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void ContainsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void ContainsTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void ContainsKeyTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void CopyToTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void CopyToTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void GetEnumeratorTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void IndexOfTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void InsertTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void RemoveTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void RemoveTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void RemoveTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void RemoveAtTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [Ignore]
        public void TryGetValueTest()
        {
            Assert.Fail();
        }
    }
}