using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// SrcArray の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class SrcArrayFurtherTests
    {
        // ──────────────────────────────────────────────
        // インデックス境界テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IndexerGet_ExactlyLastElement_ReturnsValue()
        {
            var arr = new SrcArray<int>();
            arr.Add(100);
            arr.Add(200);
            arr.Add(300);
            Assert.AreEqual(300, arr[3]);
        }

        [TestMethod]
        public void IndexerSet_LastElement_ChangesValue()
        {
            var arr = new SrcArray<string>();
            arr.Add("first");
            arr.Add("last");
            arr[2] = "changed";
            Assert.AreEqual("changed", arr[2]);
            Assert.AreEqual("first", arr[1]);
        }

        [TestMethod]
        public void IndexerGet_IndexTooLarge_ThrowsException()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => { var _ = arr[2]; });
        }

        [TestMethod]
        public void IndexerGet_IndexZero_ThrowsException()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => { var _ = arr[0]; });
        }

        [TestMethod]
        public void IndexerGet_IndexNegative_ThrowsException()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            Assert.ThrowsException<System.ArgumentOutOfRangeException>(() => { var _ = arr[-1]; });
        }

        // ──────────────────────────────────────────────
        // 型テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SrcArray_Double_StoresAndRetrieves()
        {
            var arr = new SrcArray<double>();
            arr.Add(3.14);
            arr.Add(2.71);
            Assert.AreEqual(3.14, arr[1], 0.001);
            Assert.AreEqual(2.71, arr[2], 0.001);
        }

        [TestMethod]
        public void SrcArray_Bool_StoresAndRetrieves()
        {
            var arr = new SrcArray<bool>();
            arr.Add(true);
            arr.Add(false);
            Assert.IsTrue(arr[1]);
            Assert.IsFalse(arr[2]);
        }

        [TestMethod]
        public void SrcArray_Object_StoresAndRetrieves()
        {
            var obj1 = new System.Object();
            var obj2 = new System.Object();
            var arr = new SrcArray<System.Object>();
            arr.Add(obj1);
            arr.Add(obj2);
            Assert.AreSame(obj1, arr[1]);
            Assert.AreSame(obj2, arr[2]);
        }

        // ──────────────────────────────────────────────
        // List の基本操作
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Clear_RemovesAllElements()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            arr.Add(2);
            arr.Add(3);
            arr.Clear();
            Assert.AreEqual(0, arr.Count);
        }

        [TestMethod]
        public void Remove_RemovesSpecificElement()
        {
            var arr = new SrcArray<string>();
            arr.Add("a");
            arr.Add("b");
            arr.Add("c");
            arr.Remove("b");
            Assert.AreEqual(2, arr.Count);
            Assert.AreEqual("a", arr[1]);
            Assert.AreEqual("c", arr[2]);
        }

        [TestMethod]
        public void Insert_InsertsAtPosition()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            arr.Add(3);
            // List のメソッドを使用 (0-offset)
            arr.Insert(1, 2);  // インデックス1に "2" を挿入
            Assert.AreEqual(3, arr.Count);
            Assert.AreEqual(1, arr[1]);
            Assert.AreEqual(2, arr[2]);
            Assert.AreEqual(3, arr[3]);
        }

        [TestMethod]
        public void IndexOf_FindsElement_ReturnsZeroBasedIndex()
        {
            var arr = new SrcArray<string>();
            arr.Add("alpha");
            arr.Add("beta");
            arr.Add("gamma");
            // List.IndexOf は0ベース
            Assert.AreEqual(1, arr.IndexOf("beta"));
        }

        [TestMethod]
        public void Enumerate_IteratesAllElements()
        {
            var arr = new SrcArray<int>();
            arr.Add(10);
            arr.Add(20);
            arr.Add(30);

            int sum = 0;
            foreach (var item in arr)
            {
                sum += item;
            }
            Assert.AreEqual(60, sum);
        }

        // ──────────────────────────────────────────────
        // AddRange
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddRange_EmptyArray_NoChange()
        {
            var arr = new SrcArray<int>();
            arr.Add(1);
            arr.AddRange(new int[] { });
            Assert.AreEqual(1, arr.Count);
        }

        [TestMethod]
        public void AddRange_MultipleElements_AllAdded()
        {
            var arr = new SrcArray<int>();
            arr.AddRange(new[] { 1, 2, 3, 4, 5 });
            Assert.AreEqual(5, arr.Count);
            Assert.AreEqual(1, arr[1]);
            Assert.AreEqual(5, arr[5]);
        }
    }
}
