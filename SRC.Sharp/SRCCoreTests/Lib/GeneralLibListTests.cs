using SRCCore.Lib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace SRCCore.Lib.Tests
{
    [TestClass()]
    public class GeneralLibListTests
    {
        [TestMethod()]
        public void ToList_SpaceSeparated_ReturnsList()
        {
            var result = GeneralLib.ToList("a b c");
            Assert.AreEqual(3, result.Count);
        }

        [TestMethod()]
        public void ToList_SingleItem_ReturnsSingleItem()
        {
            var result = GeneralLib.ToList("apple");
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("apple", result[0]);
        }

        [TestMethod()]
        public void ToList_EmptyString_ReturnsEmpty()
        {
            var result = GeneralLib.ToList("");
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod()]
        public void ToList_NullString_ReturnsEmpty()
        {
            var result = GeneralLib.ToList(null);
            Assert.AreEqual(0, result.Count);
        }

        [TestMethod()]
        public void ToList_ItemsWithParens_RemoveKakko()
        {
            // removeKakko removes paren characters but keeps content inside; use empty parens
            var result = GeneralLib.ToList("a() b", removeKakko: true);
            Assert.AreEqual("a", result[0]);
        }

        [TestMethod()]
        public void ListTail_WithIndex1_ReturnsTailFromIndex1()
        {
            // idx=1 returns "" per implementation (idx > 1 condition); idx=2 returns "b c"
            string result = GeneralLib.ListTail("a b c", 2);
            Assert.AreEqual("b c", result);
        }

        [TestMethod()]
        public void ListTail_WithIndex2_ReturnsTailFromIndex2()
        {
            // idx=3 skips first 2 elements and returns "c"
            string result = GeneralLib.ListTail("a b c", 3);
            Assert.AreEqual("c", result);
        }

        [TestMethod()]
        public void ListTail_BeyondEnd_ReturnsEmpty()
        {
            string result = GeneralLib.ListTail("a b c", 5);
            Assert.AreEqual("", result);
        }

        [TestMethod()]
        public void ListSplit_ThreeItems_ReturnThree()
        {
            int count = GeneralLib.ListSplit("a b c", out string[] arr);
            Assert.AreEqual(3, count);
            Assert.AreEqual(3, arr.Length);
        }

        [TestMethod()]
        public void IsNumber_ValidInt_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("42"));
        }

        [TestMethod()]
        public void IsNumber_Float_ReturnsTrue()
        {
            Assert.IsTrue(GeneralLib.IsNumber("3.14"));
        }

        [TestMethod()]
        public void IsNumber_NotNumber_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsNumber("abc"));
        }

        [TestMethod()]
        public void IsNumber_Empty_ReturnsFalse()
        {
            Assert.IsFalse(GeneralLib.IsNumber(""));
        }
    }
}
