using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    [TestClass()]
    public class SrcArrayTests
    {
        [TestMethod()]
        public void IndexerTest_OneBasedAccess()
        {
            var arr = new SrcArray<int>();
            arr.Add(10);
            arr.Add(20);
            arr.Add(30);

            // 1ベースのインデックスアクセス
            Assert.AreEqual(10, arr[1]);
            Assert.AreEqual(20, arr[2]);
            Assert.AreEqual(30, arr[3]);
        }

        [TestMethod()]
        public void IndexerTest_SetValue()
        {
            var arr = new SrcArray<string>();
            arr.Add("a");
            arr.Add("b");
            arr.Add("c");

            arr[2] = "B";
            Assert.AreEqual("B", arr[2]);
            Assert.AreEqual("a", arr[1]);
            Assert.AreEqual("c", arr[3]);
        }

        [TestMethod()]
        public void IndexerTest_OutOfRange_Throws()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);

            // 0ベースでは範囲外
            Assert.Throws<System.ArgumentOutOfRangeException>(() => { var _ = arr[0]; });
            Assert.Throws<System.ArgumentOutOfRangeException>(() => { var _ = arr[2]; });
        }

        [TestMethod()]
        public void SrcArrayString_BasicUsage()
        {
            var arr = new SrcArray<string>();
            arr.Add("alpha");
            arr.Add("beta");
            arr.Add("gamma");

            Assert.AreEqual("alpha", arr[1]);
            Assert.AreEqual("beta", arr[2]);
            Assert.AreEqual("gamma", arr[3]);
        }

        [TestMethod()]
        public void CountProperty_ReflectsAddedItems()
        {
            var arr = new SrcArray<int>();
            Assert.AreEqual(0, arr.Count);
            arr.Add(10);
            Assert.AreEqual(1, arr.Count);
            arr.Add(20);
            Assert.AreEqual(2, arr.Count);
        }

        [TestMethod()]
        public void AddRange_InheritsListBehavior()
        {
            var arr = new SrcArray<int>();
            arr.AddRange(new[] { 1, 2, 3 });
            Assert.AreEqual(3, arr.Count);
            Assert.AreEqual(1, arr[1]);
            Assert.AreEqual(2, arr[2]);
            Assert.AreEqual(3, arr[3]);
        }

        [TestMethod()]
        public void Contains_InheritsListBehavior()
        {
            var arr = new SrcArray<string>();
            arr.Add("hello");
            arr.Add("world");

            Assert.IsTrue(arr.Contains("hello"));
            Assert.IsTrue(arr.Contains("world"));
            Assert.IsFalse(arr.Contains("foo"));
        }
    }
}
