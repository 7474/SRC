using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;
using System;

namespace SRCCore.VB.Tests
{
    [TestClass]
    public class SrcArrayMoreTests3
    {
        [TestMethod]
        public void NewArray_IsEmpty()
        {
            var arr = new SrcArray<int>();
            Assert.AreEqual(0, arr.Count);
        }

        [TestMethod]
        public void AddSingleItem_CountIsOne()
        {
            var arr = new SrcArray<string>();
            arr.Add("hello");
            Assert.AreEqual(1, arr.Count);
        }

        [TestMethod]
        public void AddMultipleItems_CountIsCorrect()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            arr.Add(2);
            arr.Add(3);
            Assert.AreEqual(3, arr.Count);
        }

        [TestMethod]
        public void OneBasedIndex_FirstElement_ReturnsCorrectValue()
        {
            var arr = new SrcArray<int>();
            arr.Add(42);
            Assert.AreEqual(42, arr[1]);
        }

        [TestMethod]
        public void OneBasedIndex_LastElement_ReturnsCorrectValue()
        {
            var arr = new SrcArray<string>();
            arr.Add("a");
            arr.Add("b");
            arr.Add("c");
            Assert.AreEqual("c", arr[3]);
        }

        [TestMethod]
        public void SetByIndex_UpdatesValue()
        {
            var arr = new SrcArray<int>();
            arr.Add(10);
            arr[1] = 99;
            Assert.AreEqual(99, arr[1]);
        }

        [TestMethod]
        public void SetByIndex_MiddleElement_UpdatesCorrectly()
        {
            var arr = new SrcArray<string>();
            arr.Add("x");
            arr.Add("y");
            arr.Add("z");
            arr[2] = "Y";
            Assert.AreEqual("x", arr[1]);
            Assert.AreEqual("Y", arr[2]);
            Assert.AreEqual("z", arr[3]);
        }

        [TestMethod]
        public void IndexZero_ThrowsArgumentOutOfRangeException()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = arr[0]; });
        }

        [TestMethod]
        public void IndexBeyondCount_ThrowsArgumentOutOfRangeException()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            Assert.Throws<ArgumentOutOfRangeException>(() => { var _ = arr[2]; });
        }

        [TestMethod]
        public void BoolArray_DefaultIsFalse()
        {
            var arr = new SrcArray<bool>();
            arr.Add(false);
            Assert.IsFalse(arr[1]);
        }

        [TestMethod]
        public void BoolArray_SetTrue_ReturnsTrue()
        {
            var arr = new SrcArray<bool>();
            arr.Add(false);
            arr[1] = true;
            Assert.IsTrue(arr[1]);
        }

        [TestMethod]
        public void StringArray_NullValue_CanBeStored()
        {
            var arr = new SrcArray<string>();
            arr.Add(null);
            Assert.IsNull(arr[1]);
        }

        [TestMethod]
        public void Clear_RemovesAllItems()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            arr.Add(2);
            arr.Clear();
            Assert.AreEqual(0, arr.Count);
        }

        [TestMethod]
        public void Contains_ExistingValue_ReturnsTrue()
        {
            var arr = new SrcArray<int>();
            arr.Add(7);
            Assert.IsTrue(arr.Contains(7));
        }

        [TestMethod]
        public void Contains_NonExistingValue_ReturnsFalse()
        {
            var arr = new SrcArray<int>();
            arr.Add(7);
            Assert.IsFalse(arr.Contains(99));
        }

        [TestMethod]
        public void AddRange_AddsItemsInOrder()
        {
            var arr = new SrcArray<int>();
            arr.AddRange(new[] { 10, 20, 30 });
            Assert.AreEqual(10, arr[1]);
            Assert.AreEqual(20, arr[2]);
            Assert.AreEqual(30, arr[3]);
        }
    }
}
