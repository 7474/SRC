using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.Linq;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// Src2DArray の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class Src2DArrayAdditionalTests
    {
        // ──────────────────────────────────────────────
        // double 型
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DoubleType_DefaultIsZero()
        {
            var arr = new Src2DArray<double>(2, 2);
            Assert.AreEqual(0.0, arr[1, 1]);
            Assert.AreEqual(0.0, arr[2, 2]);
        }

        [TestMethod]
        public void DoubleType_SetAndRead()
        {
            var arr = new Src2DArray<double>(3, 3);
            arr[1, 2] = 3.14;
            arr[3, 1] = -2.5;
            Assert.AreEqual(3.14, arr[1, 2], 1e-10);
            Assert.AreEqual(-2.5, arr[3, 1], 1e-10);
        }

        [TestMethod]
        public void DoubleType_Overwrite()
        {
            var arr = new Src2DArray<double>(2, 2);
            arr[1, 1] = 1.0;
            arr[1, 1] = 99.9;
            Assert.AreEqual(99.9, arr[1, 1], 1e-10);
        }

        // ──────────────────────────────────────────────
        // 非正方形の次元 (Non-square dimensions)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void NonSquare_ThreeByFive_CorrectDimensions()
        {
            var arr = new Src2DArray<int>(3, 5);
            Assert.AreEqual(3, arr.X);
            Assert.AreEqual(5, arr.Y);
        }

        [TestMethod]
        public void NonSquare_ThreeByFive_AllCountIsCorrect()
        {
            var arr = new Src2DArray<int>(3, 5);
            Assert.AreEqual(15, arr.All.Count());
        }

        [TestMethod]
        public void NonSquare_OneByTen_AllAccessible()
        {
            var arr = new Src2DArray<int>(1, 10);
            for (int y = 1; y <= 10; y++)
            {
                arr[1, y] = y * 10;
            }
            for (int y = 1; y <= 10; y++)
            {
                Assert.AreEqual(y * 10, arr[1, y]);
            }
        }

        [TestMethod]
        public void NonSquare_TenByOne_AllAccessible()
        {
            var arr = new Src2DArray<int>(10, 1);
            for (int x = 1; x <= 10; x++)
            {
                arr[x, 1] = x;
            }
            for (int x = 1; x <= 10; x++)
            {
                Assert.AreEqual(x, arr[x, 1]);
            }
        }

        [TestMethod]
        public void NonSquare_XRange_ReturnsCorrectRange()
        {
            var arr = new Src2DArray<int>(3, 7);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, arr.XRange.ToArray());
        }

        [TestMethod]
        public void NonSquare_YRange_ReturnsCorrectRange()
        {
            var arr = new Src2DArray<int>(3, 7);
            CollectionAssert.AreEqual(new[] { 1, 2, 3, 4, 5, 6, 7 }, arr.YRange.ToArray());
        }

        // ──────────────────────────────────────────────
        // All プロパティの列挙順序 (row-major: x外ループ, y内ループ)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void All_TwoByThree_RowMajorOrder()
        {
            // All は x=1..X の外ループ, y=1..Y の内ループで列挙される
            // (1,1), (1,2), (1,3), (2,1), (2,2), (2,3)
            var arr = new Src2DArray<int>(2, 3);
            arr[1, 1] = 11;
            arr[1, 2] = 12;
            arr[1, 3] = 13;
            arr[2, 1] = 21;
            arr[2, 2] = 22;
            arr[2, 3] = 23;

            var values = arr.All.ToArray();
            Assert.AreEqual(6, values.Length);
            Assert.AreEqual(11, values[0]);
            Assert.AreEqual(12, values[1]);
            Assert.AreEqual(13, values[2]);
            Assert.AreEqual(21, values[3]);
            Assert.AreEqual(22, values[4]);
            Assert.AreEqual(23, values[5]);
        }

        [TestMethod]
        public void All_ReturnsAllSetValues_UsingSum()
        {
            var arr = new Src2DArray<int>(4, 4);
            // 対角に 1 を設定
            arr[1, 1] = 1;
            arr[2, 2] = 1;
            arr[3, 3] = 1;
            arr[4, 4] = 1;
            Assert.AreEqual(4, arr.All.Sum());
            Assert.AreEqual(16, arr.All.Count());
        }

        // ──────────────────────────────────────────────
        // object 型
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ObjectType_DefaultIsNull()
        {
            var arr = new Src2DArray<object>(2, 2);
            Assert.IsNull(arr[1, 1]);
            Assert.IsNull(arr[2, 2]);
        }

        [TestMethod]
        public void ObjectType_SetAndRead()
        {
            var arr = new Src2DArray<object>(2, 2);
            var obj = new { Name = "test", Value = 42 };
            arr[1, 1] = obj;
            Assert.AreEqual(obj, arr[1, 1]);
        }

        [TestMethod]
        public void ObjectType_MixedTypes_CanStore()
        {
            var arr = new Src2DArray<object>(2, 2);
            arr[1, 1] = "string value";
            arr[1, 2] = 123;
            arr[2, 1] = 3.14;
            arr[2, 2] = true;

            Assert.AreEqual("string value", arr[1, 1]);
            Assert.AreEqual(123, arr[1, 2]);
            Assert.AreEqual(3.14, arr[2, 1]);
            Assert.AreEqual(true, arr[2, 2]);
        }

        // ──────────────────────────────────────────────
        // 境界アクセス (Boundary access)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Boundary_FirstElement_IsAccessible()
        {
            var arr = new Src2DArray<int>(5, 5);
            arr[1, 1] = 777;
            Assert.AreEqual(777, arr[1, 1]);
        }

        [TestMethod]
        public void Boundary_LastElement_IsAccessible()
        {
            var arr = new Src2DArray<int>(5, 5);
            arr[5, 5] = 888;
            Assert.AreEqual(888, arr[5, 5]);
        }

        [TestMethod]
        public void Boundary_OutOfRange_ThrowsIndexOutOfRangeException()
        {
            var arr = new Src2DArray<int>(3, 3);
            Assert.ThrowsException<System.IndexOutOfRangeException>(() => { var _ = arr[4, 1]; });
        }

        [TestMethod]
        public void Boundary_ZeroIndex_ThrowsIndexOutOfRangeException()
        {
            var arr = new Src2DArray<int>(3, 3);
            Assert.ThrowsException<System.IndexOutOfRangeException>(() => { var _ = arr[0, 1]; });
        }

        // ──────────────────────────────────────────────
        // 1×1 配列
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OneByOne_AllHasSingleElement()
        {
            var arr = new Src2DArray<int>(1, 1);
            arr[1, 1] = 42;
            var all = arr.All.ToArray();
            Assert.AreEqual(1, all.Length);
            Assert.AreEqual(42, all[0]);
        }

        [TestMethod]
        public void OneByOne_XRangeAndYRange_SingleElement()
        {
            var arr = new Src2DArray<double>(1, 1);
            CollectionAssert.AreEqual(new[] { 1 }, arr.XRange.ToArray());
            CollectionAssert.AreEqual(new[] { 1 }, arr.YRange.ToArray());
        }
    }
}
