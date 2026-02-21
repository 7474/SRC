using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Maps;

namespace SRCCore.Maps.Tests
{
    /// <summary>
    /// BoxTypes enum のユニットテスト
    /// </summary>
    [TestClass]
    public class BoxTypesEnumTests
    {
        // ──────────────────────────────────────────────
        // 各値の数値確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Under_IsOne()
        {
            Assert.AreEqual(1, (int)BoxTypes.Under);
        }

        [TestMethod]
        public void Upper_IsTwo()
        {
            Assert.AreEqual(2, (int)BoxTypes.Upper);
        }

        [TestMethod]
        public void UpperDataOnly_IsThree()
        {
            Assert.AreEqual(3, (int)BoxTypes.UpperDataOnly);
        }

        [TestMethod]
        public void UpperBmpOnly_IsFour()
        {
            Assert.AreEqual(4, (int)BoxTypes.UpperBmpOnly);
        }

        // ──────────────────────────────────────────────
        // Enum 基本確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AllValuesAreDistinct()
        {
            var values = System.Enum.GetValues(typeof(BoxTypes));
            var set = new System.Collections.Generic.HashSet<int>();
            foreach (BoxTypes v in values)
            {
                Assert.IsTrue(set.Add((int)v), $"重複した値: {v} = {(int)v}");
            }
        }

        [TestMethod]
        public void IsDefined_ForUnder()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), BoxTypes.Under));
        }

        [TestMethod]
        public void IsDefined_ForUpper()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), BoxTypes.Upper));
        }

        [TestMethod]
        public void IsDefined_ForUpperDataOnly()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), BoxTypes.UpperDataOnly));
        }

        [TestMethod]
        public void IsDefined_ForUpperBmpOnly()
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(BoxTypes), BoxTypes.UpperBmpOnly));
        }

        [TestMethod]
        public void CanBeParsedFromString_Under()
        {
            Assert.AreEqual(BoxTypes.Under, System.Enum.Parse<BoxTypes>("Under"));
        }

        [TestMethod]
        public void CanBeParsedFromString_Upper()
        {
            Assert.AreEqual(BoxTypes.Upper, System.Enum.Parse<BoxTypes>("Upper"));
        }

        [TestMethod]
        public void CanBeParsedFromString_UpperDataOnly()
        {
            Assert.AreEqual(BoxTypes.UpperDataOnly, System.Enum.Parse<BoxTypes>("UpperDataOnly"));
        }

        [TestMethod]
        public void CanBeParsedFromString_UpperBmpOnly()
        {
            Assert.AreEqual(BoxTypes.UpperBmpOnly, System.Enum.Parse<BoxTypes>("UpperBmpOnly"));
        }

        [TestMethod]
        public void Under_IsNotEqualToUpper()
        {
            Assert.AreNotEqual(BoxTypes.Under, BoxTypes.Upper);
        }

        [TestMethod]
        public void UpperDataOnly_IsNotEqualToUpperBmpOnly()
        {
            Assert.AreNotEqual(BoxTypes.UpperDataOnly, BoxTypes.UpperBmpOnly);
        }

        // ──────────────────────────────────────────────
        // MapCell.Terrain プロパティとの組み合わせ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapCell_BoxType_Default_IsUnder()
        {
            // MapCell() コンストラクタは BoxType = BoxTypes.Under に初期化する
            var cell = new MapCell();
            Assert.AreEqual(BoxTypes.Under, cell.BoxType);
        }

        [TestMethod]
        public void MapCell_SetBoxType_Upper_CanBeRead()
        {
            var cell = new MapCell { BoxType = BoxTypes.Upper };
            Assert.AreEqual(BoxTypes.Upper, cell.BoxType);
        }

        [TestMethod]
        public void MapCell_SetBoxType_UpperDataOnly_CanBeRead()
        {
            var cell = new MapCell { BoxType = BoxTypes.UpperDataOnly };
            Assert.AreEqual(BoxTypes.UpperDataOnly, cell.BoxType);
        }

        [TestMethod]
        public void MapCell_SetBoxType_UpperBmpOnly_CanBeRead()
        {
            var cell = new MapCell { BoxType = BoxTypes.UpperBmpOnly };
            Assert.AreEqual(BoxTypes.UpperBmpOnly, cell.BoxType);
        }
    }
}
