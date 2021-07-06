using SRCCore.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Extensions.Tests
{
    [TestClass()]
    public class ListExtensionTests
    {
        [TestMethod()]
        public void SafeRefOneOffsetTest()
        {
            var list = new List<int>() { 1, 2, 3 };
            Assert.AreEqual(0, list.SafeRefOneOffset(0));
            Assert.AreEqual(1, list.SafeRefOneOffset(1));
            Assert.AreEqual(2, list.SafeRefOneOffset(2));
            Assert.AreEqual(3, list.SafeRefOneOffset(3));
            Assert.AreEqual(0, list.SafeRefOneOffset(4));
        }

        [TestMethod()]
        public void SafeRefZeroOffsetTest()
        {
            var list = new List<string>() { "1", "2", "3" };
            Assert.AreEqual(null, list.SafeRefZeroOffset(-1));
            Assert.AreEqual("1", list.SafeRefZeroOffset(0));
            Assert.AreEqual("2", list.SafeRefZeroOffset(1));
            Assert.AreEqual("3", list.SafeRefZeroOffset(2));
            Assert.AreEqual(null, list.SafeRefZeroOffset(3));
        }

        [TestMethod()]
        public void CloneListTest()
        {
            var orgArray = new string[] { "1", "2", "3" };
            var clone1 = orgArray.CloneList();
            var clone2 = clone1.CloneList();
            var clone3 = clone2.CloneList();

            Assert.IsTrue(orgArray.SequenceEqual(clone1));
            Assert.IsTrue(orgArray.SequenceEqual(clone2));
            Assert.IsTrue(orgArray.SequenceEqual(clone3));
            Assert.AreNotEqual(clone1, clone2);
            Assert.AreNotEqual(clone2, clone3);
        }
    }
}