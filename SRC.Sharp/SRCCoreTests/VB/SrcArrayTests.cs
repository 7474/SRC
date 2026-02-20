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
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => { var _ = arr[0]; });
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => { var _ = arr[2]; });
        }
    }
}
