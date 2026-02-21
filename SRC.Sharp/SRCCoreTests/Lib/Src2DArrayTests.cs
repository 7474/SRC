using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Lib;
using System.Linq;

namespace SRCCore.Lib.Tests
{
    /// <summary>
    /// 1オフセットの2次元配列クラス Src2DArray のユニットテスト
    /// </summary>
    [TestClass]
    public class Src2DArrayTests
    {
        // ──────────────────────────────────────────────
        // 基本的なアクセス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Indexer_OneBasedAccess_GetAndSet()
        {
            var arr = new Src2DArray<int>(3, 3);
            arr[1, 1] = 10;
            arr[2, 3] = 99;

            Assert.AreEqual(10, arr[1, 1]);
            Assert.AreEqual(99, arr[2, 3]);
        }

        [TestMethod]
        public void Indexer_DefaultValue_IsDefault()
        {
            var arr = new Src2DArray<string>(2, 2);
            Assert.IsNull(arr[1, 1]);
            Assert.IsNull(arr[2, 2]);
        }

        [TestMethod]
        public void Indexer_OverwriteValue()
        {
            var arr = new Src2DArray<int>(2, 2);
            arr[1, 1] = 5;
            arr[1, 1] = 42;
            Assert.AreEqual(42, arr[1, 1]);
        }

        // ──────────────────────────────────────────────
        // サイズプロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void XProperty_ReturnsCorrectWidth()
        {
            var arr = new Src2DArray<int>(4, 3);
            Assert.AreEqual(4, arr.X);
        }

        [TestMethod]
        public void YProperty_ReturnsCorrectHeight()
        {
            var arr = new Src2DArray<int>(4, 3);
            Assert.AreEqual(3, arr.Y);
        }

        // ──────────────────────────────────────────────
        // XRange / YRange
        // ──────────────────────────────────────────────

        [TestMethod]
        public void XRange_ReturnsOneBasedRange()
        {
            var arr = new Src2DArray<int>(3, 2);
            CollectionAssert.AreEqual(new[] { 1, 2, 3 }, arr.XRange.ToArray());
        }

        [TestMethod]
        public void YRange_ReturnsOneBasedRange()
        {
            var arr = new Src2DArray<int>(3, 2);
            CollectionAssert.AreEqual(new[] { 1, 2 }, arr.YRange.ToArray());
        }

        // ──────────────────────────────────────────────
        // All
        // ──────────────────────────────────────────────

        [TestMethod]
        public void All_EnumeratesAllElements()
        {
            var arr = new Src2DArray<int>(2, 2);
            arr[1, 1] = 1;
            arr[1, 2] = 2;
            arr[2, 1] = 3;
            arr[2, 2] = 4;

            var values = arr.All.ToArray();
            Assert.AreEqual(4, values.Length);
            CollectionAssert.Contains(values, 1);
            CollectionAssert.Contains(values, 2);
            CollectionAssert.Contains(values, 3);
            CollectionAssert.Contains(values, 4);
        }

        [TestMethod]
        public void All_EmptyArray_AllDefaultValues()
        {
            var arr = new Src2DArray<int>(2, 3);
            Assert.AreEqual(6, arr.All.Count());
            Assert.IsTrue(arr.All.All(v => v == 0));
        }

        // ──────────────────────────────────────────────
        // 境界値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Indexer_MaxBoundary_AccessesCorrectly()
        {
            var arr = new Src2DArray<int>(5, 4);
            arr[5, 4] = 999;
            Assert.AreEqual(999, arr[5, 4]);
        }

        [TestMethod]
        public void Indexer_SingleCell_Works()
        {
            var arr = new Src2DArray<string>(1, 1);
            arr[1, 1] = "only";
            Assert.AreEqual("only", arr[1, 1]);
        }

        // ──────────────────────────────────────────────
        // bool value type
        // ──────────────────────────────────────────────

        [TestMethod]
        public void BoolType_DefaultIsFalse()
        {
            var arr = new Src2DArray<bool>(3, 3);
            Assert.IsFalse(arr[1, 1]);
            Assert.IsFalse(arr[3, 3]);
        }

        [TestMethod]
        public void BoolType_SetAndRead()
        {
            var arr = new Src2DArray<bool>(3, 3);
            arr[2, 2] = true;
            Assert.IsTrue(arr[2, 2]);
            Assert.IsFalse(arr[1, 1]);
        }

        // ──────────────────────────────────────────────
        // Large dimensions (10x10)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LargeDimensions_TenByTen_AllAccessible()
        {
            var arr = new Src2DArray<int>(10, 10);
            // Set corners and center
            arr[1, 1] = 1;
            arr[10, 10] = 100;
            arr[5, 5] = 55;

            Assert.AreEqual(1, arr[1, 1]);
            Assert.AreEqual(100, arr[10, 10]);
            Assert.AreEqual(55, arr[5, 5]);
        }

        [TestMethod]
        public void LargeDimensions_TenByTen_CorrectCount()
        {
            var arr = new Src2DArray<int>(10, 10);
            Assert.AreEqual(10, arr.X);
            Assert.AreEqual(10, arr.Y);
            Assert.AreEqual(100, arr.All.Count());
        }
    }
}
