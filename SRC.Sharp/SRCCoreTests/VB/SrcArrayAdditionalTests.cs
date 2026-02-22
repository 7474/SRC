using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcArray クラスの追加ユニットテスト
    /// </summary>
    [TestClass]
    public class SrcArrayAdditionalTests
    {
        private static SrcArray<T> CreateArray<T>(params T[] items)
        {
            var arr = new SrcArray<T>();
            foreach (var item in items)
            {
                arr.Add(item);
            }
            return arr;
        }

        // ──────────────────────────────────────────────
        // コンストラクタ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Constructor_EmptyList_HasZeroCount()
        {
            var arr = new SrcArray<string>();
            Assert.AreEqual(0, arr.Count);
        }

        [TestMethod]
        public void AddSingleElement_CountIsOne()
        {
            var arr = CreateArray(42);
            Assert.AreEqual(1, arr.Count);
        }

        [TestMethod]
        public void AddMultipleElements_CountIsCorrect()
        {
            var arr = CreateArray("a", "b", "c");
            Assert.AreEqual(3, arr.Count);
        }

        // ──────────────────────────────────────────────
        // インデクサー（1始まり）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Indexer_OneBasedAccess_ReturnsCorrectElement()
        {
            var arr = CreateArray("first", "second", "third");
            Assert.AreEqual("first", arr[1]);
            Assert.AreEqual("second", arr[2]);
            Assert.AreEqual("third", arr[3]);
        }

        [TestMethod]
        public void Indexer_SetValue_CanBeRead()
        {
            var arr = CreateArray("a", "b", "c");
            arr[2] = "updated";
            Assert.AreEqual("updated", arr[2]);
        }

        [TestMethod]
        public void Indexer_SingleElement_FirstIsAtIndex1()
        {
            var arr = CreateArray(99);
            Assert.AreEqual(99, arr[1]);
        }

        // ──────────────────────────────────────────────
        // IEnumerable
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Enumerable_CanIterateAllElements()
        {
            var arr = CreateArray(10, 20, 30);
            var sum = 0;
            foreach (var item in arr)
            {
                sum += item;
            }
            Assert.AreEqual(60, sum);
        }

        // ──────────────────────────────────────────────
        // Count プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Count_AfterClear_IsZero()
        {
            var arr = CreateArray("a", "b", "c", "d");
            arr.Clear();
            Assert.AreEqual(0, arr.Count);
        }

        // ──────────────────────────────────────────────
        // 整数型配列
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IntArray_ElementsAreCorrectlyStored()
        {
            var arr = CreateArray(5, 10, 15, 20, 25);
            Assert.AreEqual(5, arr.Count);
            Assert.AreEqual(5, arr[1]);
            Assert.AreEqual(25, arr[5]);
        }

        // ──────────────────────────────────────────────
        // 浮動小数点型配列
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DoubleArray_ElementsAreCorrectlyStored()
        {
            var arr = CreateArray(1.5, 2.5, 3.5);
            Assert.AreEqual(3, arr.Count);
            Assert.AreEqual(1.5, arr[1], 1e-10);
            Assert.AreEqual(3.5, arr[3], 1e-10);
        }

        // ──────────────────────────────────────────────
        // 上書き
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Overwrite_ThenRead_ReturnsNewValue()
        {
            var arr = CreateArray(1, 2, 3);
            arr[1] = 100;
            arr[3] = 300;
            Assert.AreEqual(100, arr[1]);
            Assert.AreEqual(2, arr[2]);
            Assert.AreEqual(300, arr[3]);
        }
    }
}
