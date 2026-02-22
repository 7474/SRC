using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.Linq;

namespace SRCCore.Lib.Tests
{
    [TestClass]
    public class Src2DArrayMoreTests3
    {
        [TestMethod]
        public void Constructor_SetsXDimension()
        {
            var arr = new Src2DArray<int>(5, 3);
            Assert.AreEqual(5, arr.X);
        }

        [TestMethod]
        public void Constructor_SetsYDimension()
        {
            var arr = new Src2DArray<int>(5, 3);
            Assert.AreEqual(3, arr.Y);
        }

        [TestMethod]
        public void IntType_DefaultIsZero()
        {
            var arr = new Src2DArray<int>(2, 2);
            Assert.AreEqual(0, arr[1, 1]);
        }

        [TestMethod]
        public void StringType_DefaultIsNull()
        {
            var arr = new Src2DArray<string>(2, 2);
            Assert.IsNull(arr[1, 2]);
        }

        [TestMethod]
        public void SetAndGet_IntValue_FirstCell()
        {
            var arr = new Src2DArray<int>(3, 3);
            arr[1, 1] = 100;
            Assert.AreEqual(100, arr[1, 1]);
        }

        [TestMethod]
        public void SetAndGet_StringValue_MiddleCell()
        {
            var arr = new Src2DArray<string>(3, 3);
            arr[2, 2] = "center";
            Assert.AreEqual("center", arr[2, 2]);
        }

        [TestMethod]
        public void SetAndGet_LastCell_CorrectValue()
        {
            var arr = new Src2DArray<int>(4, 4);
            arr[4, 4] = 999;
            Assert.AreEqual(999, arr[4, 4]);
        }

        [TestMethod]
        public void XRange_MatchesDimension()
        {
            var arr = new Src2DArray<int>(3, 2);
            var xr = arr.XRange.ToArray();
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, xr);
        }

        [TestMethod]
        public void YRange_MatchesDimension()
        {
            var arr = new Src2DArray<int>(3, 4);
            var yr = arr.YRange.ToArray();
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4 }, yr);
        }

        [TestMethod]
        public void All_CountMatchesTotalCells()
        {
            var arr = new Src2DArray<int>(3, 4);
            Assert.AreEqual(12, arr.All.Count());
        }

        [TestMethod]
        public void All_AllDefaultInts_AreZero()
        {
            var arr = new Src2DArray<int>(2, 2);
            Assert.IsTrue(arr.All.All(v => v == 0));
        }

        [TestMethod]
        public void OverwriteValue_ReturnsNewValue()
        {
            var arr = new Src2DArray<int>(2, 2);
            arr[1, 1] = 5;
            arr[1, 1] = 10;
            Assert.AreEqual(10, arr[1, 1]);
        }

        [TestMethod]
        public void SetMultipleCells_EachIndependent()
        {
            var arr = new Src2DArray<int>(3, 3);
            arr[1, 1] = 1;
            arr[1, 2] = 2;
            arr[2, 1] = 3;
            arr[3, 3] = 9;
            Assert.AreEqual(1, arr[1, 1]);
            Assert.AreEqual(2, arr[1, 2]);
            Assert.AreEqual(3, arr[2, 1]);
            Assert.AreEqual(9, arr[3, 3]);
        }

        [TestMethod]
        public void SingleCell_XY_One_Works()
        {
            var arr = new Src2DArray<string>(1, 1);
            arr[1, 1] = "only";
            Assert.AreEqual("only", arr[1, 1]);
        }
    }
}
