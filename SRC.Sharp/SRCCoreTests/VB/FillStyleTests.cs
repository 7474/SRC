using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.VB;

namespace SRCCore.VB.Tests
{
    /// <summary>
    /// FillStyle enum のユニットテスト
    /// </summary>
    [TestClass]
    public class FillStyleTests
    {
        // ──────────────────────────────────────────────
        // FillStyle 各値の確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FillStyle_VbFSSolid_IsZero()
        {
            Assert.AreEqual(0, (int)FillStyle.VbFSSolid);
        }

        [TestMethod]
        public void FillStyle_VbFSTransparent_IsOne()
        {
            Assert.AreEqual(1, (int)FillStyle.VbFSTransparent);
        }

        [TestMethod]
        public void FillStyle_VbHorizontalLine_IsTwo()
        {
            Assert.AreEqual(2, (int)FillStyle.VbHorizontalLine);
        }

        [TestMethod]
        public void FillStyle_VbVerticalLine_IsThree()
        {
            Assert.AreEqual(3, (int)FillStyle.VbVerticalLine);
        }

        [TestMethod]
        public void FillStyle_VbUpwardDiagonal_IsFour()
        {
            Assert.AreEqual(4, (int)FillStyle.VbUpwardDiagonal);
        }

        [TestMethod]
        public void FillStyle_VbDownwardDiagonal_IsFive()
        {
            Assert.AreEqual(5, (int)FillStyle.VbDownwardDiagonal);
        }

        [TestMethod]
        public void FillStyle_VbCross_IsSix()
        {
            Assert.AreEqual(6, (int)FillStyle.VbCross);
        }

        [TestMethod]
        public void FillStyle_VbDiagonalCross_IsSeven()
        {
            Assert.AreEqual(7, (int)FillStyle.VbDiagonalCross);
        }

        [TestMethod]
        public void FillStyle_HasEightValues()
        {
            Assert.AreEqual(8, System.Enum.GetValues(typeof(FillStyle)).Length);
        }

        [TestMethod]
        public void FillStyle_AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(FillStyle));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (FillStyle v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値が見つかりました: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void FillStyle_CanBeParsedFromString()
        {
            Assert.AreEqual(FillStyle.VbFSSolid, System.Enum.Parse<FillStyle>("VbFSSolid"));
            Assert.AreEqual(FillStyle.VbFSTransparent, System.Enum.Parse<FillStyle>("VbFSTransparent"));
            Assert.AreEqual(FillStyle.VbCross, System.Enum.Parse<FillStyle>("VbCross"));
        }

        [TestMethod]
        public void FillStyle_IsDefined_ForAllValues()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(FillStyle), FillStyle.VbFSSolid));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FillStyle), FillStyle.VbFSTransparent));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FillStyle), FillStyle.VbHorizontalLine));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FillStyle), FillStyle.VbVerticalLine));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FillStyle), FillStyle.VbUpwardDiagonal));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FillStyle), FillStyle.VbDownwardDiagonal));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FillStyle), FillStyle.VbCross));
            Assert.IsTrue(System.Enum.IsDefined(typeof(FillStyle), FillStyle.VbDiagonalCross));
        }
    }
}
