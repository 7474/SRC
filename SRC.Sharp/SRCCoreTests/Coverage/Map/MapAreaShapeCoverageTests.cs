using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Map の範囲選択（AreaInRange / AreaInCross / AreaInLine / AreaInWideCross / AreaInCone /
    /// AreaInSector / AreaInSectorCross / AreaInPointToPoint / AreaWithUnit / IsLineBlocked）のテスト。
    /// MaskData が false のマスが「選択された」マス。VB: Map.bas の同名関数
    /// </summary>
    [TestClass]
    public class MapAreaShapeCoverageTests
    {
        private static MapCoverageFixture CreateOpen(int w, int h)
        {
            var f = new MapCoverageFixture();
            var row = new string('.', w);
            f.LoadGrid(Enumerable.Repeat(row, h).ToArray());
            return f;
        }

        private static MapCoverageFixture CreateWithParties()
        {
            // (3,3) を中心に、各陣営のユニットを配置する
            var f = CreateOpen(5, 5);
            f.DefineUnit("歩兵", "陸", 2);
            f.Place("歩兵", "味方", 3, 2);
            f.Place("歩兵", "ＮＰＣ", 2, 3);
            f.Place("歩兵", "敵", 3, 4);
            f.Place("歩兵", "中立", 4, 3);
            f.Place("歩兵", "敵", 5, 5); // 射程外
            return f;
        }

        [TestMethod]
        public void AreaInRange_SelectsDiamondBetweenMinAndMaxAndAlwaysCenter()
        {
            // 距離 min_range～max_range のひし形を選択し、中心は常に選択する
            var f = CreateOpen(5, 5);
            f.Map.AreaInRange(3, 3, 2, 2, "全て");
            var expected = MapCoverageFixture.Points(
                "3,3",
                "3,1", "2,2", "4,2", "1,3", "5,3", "2,4", "4,4", "3,5");
            CollectionAssert.AreEquivalent(expected.ToList(), f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInRange_PartyFilters_MaskUnitsOfOtherSides()
        {
            // uparty に応じて、範囲内のユニットのうち対象外の陣営のマスを外す
            var f = CreateWithParties();

            f.Map.AreaInRange(3, 3, 2, 1, "味方");
            Assert.IsFalse(f.Map.MaskData[3, 2]);  // 味方
            Assert.IsFalse(f.Map.MaskData[2, 3]);  // ＮＰＣ
            Assert.IsTrue(f.Map.MaskData[3, 4]);   // 敵
            Assert.IsTrue(f.Map.MaskData[4, 3]);   // 中立
            Assert.IsFalse(f.Map.MaskData[1, 3]);  // ユニットのいない範囲内のマス
            Assert.IsTrue(f.Map.MaskData[5, 5]);   // 範囲外

            f.Map.AreaInRange(3, 3, 2, 1, "味方の敵");
            Assert.IsTrue(f.Map.MaskData[3, 2]);
            Assert.IsTrue(f.Map.MaskData[2, 3]);
            Assert.IsFalse(f.Map.MaskData[3, 4]);
            Assert.IsFalse(f.Map.MaskData[4, 3]);

            f.Map.AreaInRange(3, 3, 2, 1, "敵");
            Assert.IsTrue(f.Map.MaskData[3, 2]);
            Assert.IsFalse(f.Map.MaskData[3, 4]);
            Assert.IsTrue(f.Map.MaskData[4, 3]);

            f.Map.AreaInRange(3, 3, 2, 1, "敵の敵");
            Assert.IsFalse(f.Map.MaskData[3, 2]);
            Assert.IsTrue(f.Map.MaskData[3, 4]);
            Assert.IsFalse(f.Map.MaskData[4, 3]);

            f.Map.AreaInRange(3, 3, 2, 1, "中立");
            Assert.IsTrue(f.Map.MaskData[3, 2]);
            Assert.IsTrue(f.Map.MaskData[3, 4]);
            Assert.IsFalse(f.Map.MaskData[4, 3]);

            f.Map.AreaInRange(3, 3, 2, 1, "中立の敵");
            Assert.IsFalse(f.Map.MaskData[3, 2]);
            Assert.IsFalse(f.Map.MaskData[3, 4]);
            Assert.IsTrue(f.Map.MaskData[4, 3]);

            // 空間: ユニットのいるマスは全て外す
            f.Map.AreaInRange(3, 3, 2, 1, "空間");
            Assert.IsTrue(f.Map.MaskData[3, 2]);
            Assert.IsTrue(f.Map.MaskData[3, 4]);
            Assert.IsFalse(f.Map.MaskData[1, 3]);

            // 無差別: ユニットの有無で外さない
            f.Map.AreaInRange(3, 3, 2, 1, "無差別");
            Assert.IsFalse(f.Map.MaskData[3, 2]);
            Assert.IsFalse(f.Map.MaskData[3, 4]);
        }

        [TestMethod]
        public void AreaWithUnit_SelectsOnlyCellsWithUnitsOfParty()
        {
            // VB: マップ全域で uparty に該当するユニットのいるマスだけを選択する
            var f = CreateWithParties();

            f.Map.AreaWithUnit("味方");
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("3,2", "2,3").ToList(), f.Unmasked().ToList());

            f.Map.AreaWithUnit("味方の敵");
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("3,4", "4,3", "5,5").ToList(), f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInCross_SelectsFourArmsBetweenMinAndMax()
        {
            // 十字: 上下左右に min_range～max_range、中心も選択
            var f = CreateOpen(7, 7);
            f.Map.AreaInCross(4, 4, 2, 3);
            var expected = MapCoverageFixture.Points(
                "4,4", "4,1", "4,2", "4,6", "4,7", "1,4", "2,4", "6,4", "7,4");
            CollectionAssert.AreEquivalent(expected.ToList(), f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInLine_SelectsOneDirectionClippedToMap()
        {
            // 直線: 指定方向のみ。マップ外ははみ出さない
            var f = CreateOpen(7, 7);
            f.Map.AreaInLine(4, 4, 1, 2, "E");
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("4,4", "5,4", "6,4").ToList(), f.Unmasked().ToList());

            f.Map.AreaInLine(2, 2, 1, 3, "N");
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("2,2", "2,1").ToList(), f.Unmasked().ToList());

            f.Map.AreaInLine(4, 4, 1, 1, "S");
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("4,4", "4,5").ToList(), f.Unmasked().ToList());

            f.Map.AreaInLine(4, 4, 1, 1, "W");
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("4,4", "3,4").ToList(), f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInWideCross_AddsSideColumnsExceptTip()
        {
            // 幅３マスの十字: 両脇は min_range+1 ～ max_range-1 の範囲
            var f = CreateOpen(7, 7);
            f.Map.AreaInWideCross(4, 4, 1, 3);
            var expected = MapCoverageFixture.Points(
                "4,4",
                "4,1", "4,2", "4,3", "3,2", "5,2",
                "4,5", "4,6", "4,7", "3,6", "5,6",
                "1,4", "2,4", "3,4", "2,3", "2,5",
                "5,4", "6,4", "7,4", "6,3", "6,5");
            CollectionAssert.AreEquivalent(expected.ToList(), f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInCone_SelectsWideLineInDirection()
        {
            // 幅３マスの直線
            var f = CreateOpen(7, 7);
            f.Map.AreaInCone(4, 4, 1, 3, "S");
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("4,4", "4,5", "4,6", "4,7", "3,6", "5,6").ToList(),
                f.Unmasked().ToList());

            f.Map.AreaInCone(4, 4, 1, 3, "N");
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("4,4", "4,3", "4,2", "4,1", "3,2", "5,2").ToList(),
                f.Unmasked().ToList());

            f.Map.AreaInCone(4, 4, 1, 3, "E");
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("4,4", "5,4", "6,4", "7,4", "6,3", "6,5").ToList(),
                f.Unmasked().ToList());

            // マップ端では脇の列のうちマップ内の側だけ
            f.Map.AreaInCone(3, 1, 1, 3, "W");
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("3,1", "2,1", "1,1", "1,2").ToList(),
                f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSector_WidthDependsOnLevel()
        {
            // 扇状: Lv1 は距離/3、Lv2 は距離/2、Lv3 は距離-1、Lv4 は距離 だけ左右に広がる
            var f = CreateOpen(9, 9);
            f.Map.AreaInSector(5, 9, 1, 4, "N", 1);
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points(
                    "5,9", "5,8", "5,7", "4,6", "5,6", "6,6", "4,5", "5,5", "6,5").ToList(),
                f.Unmasked().ToList());

            f.Map.AreaInSector(5, 9, 1, 4, "N", 2);
            Assert.AreEqual(13, f.Unmasked().Count);
            Assert.IsFalse(f.Map.MaskData[3, 5]);
            Assert.IsTrue(f.Map.MaskData[4, 8]);

            f.Map.AreaInSector(5, 9, 1, 4, "N", 3);
            Assert.AreEqual(17, f.Unmasked().Count);
            Assert.IsFalse(f.Map.MaskData[2, 5]);

            f.Map.AreaInSector(5, 9, 1, 4, "N", 4);
            Assert.AreEqual(25, f.Unmasked().Count);
            Assert.IsFalse(f.Map.MaskData[1, 5]);
            Assert.IsFalse(f.Map.MaskData[4, 8]);
        }

        [TestMethod]
        public void AreaInSector_OtherDirectionsAndMapEdge()
        {
            var f = CreateOpen(9, 9);
            // 南向き Lv4、距離2まで
            f.Map.AreaInSector(5, 5, 1, 2, "S", 4);
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points(
                    "5,5", "4,6", "5,6", "6,6", "3,7", "4,7", "5,7", "6,7", "7,7").ToList(),
                f.Unmasked().ToList());

            // 東向き Lv2
            f.Map.AreaInSector(5, 5, 1, 2, "E", 2);
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("5,5", "6,5", "7,4", "7,5", "7,6").ToList(),
                f.Unmasked().ToList());

            // 西向き Lv3: マップ端で打ち切る
            f.Map.AreaInSector(2, 5, 1, 3, "W", 3);
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("2,5", "1,5").ToList(),
                f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSectorCross_CombinesFourDirections()
        {
            // 十字状の扇: ４方向の扇を重ねて選択する（各方向で選択を消さない）
            var f = CreateOpen(9, 9);
            f.Map.AreaInSectorCross(5, 5, 1, 2, 4);
            var set = f.Unmasked();
            foreach (var p in new[] { "5,5", "5,3", "5,7", "3,5", "7,5", "4,4", "6,6", "4,3" })
            {
                Assert.IsTrue(set.Contains(p), p);
            }
            Assert.IsFalse(set.Contains("5,2"));
            Assert.IsFalse(set.Contains("1,1"));
        }

        [TestMethod]
        public void AreaInPointToPoint_SelectsStraightAndDiagonalLines()
        {
            // ２点間を結ぶ直線上のマスを選択する
            var f = CreateOpen(5, 5);
            f.Map.AreaInPointToPoint(1, 2, 4, 2);
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("1,2", "2,2", "3,2", "4,2").ToList(), f.Unmasked().ToList());

            // 斜め: 縦方向に１つ進むごとに直前の列と新しい列の両方を選択する
            f.Map.AreaInPointToPoint(1, 1, 3, 3);
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("1,1", "1,2", "2,2", "2,3", "3,3").ToList(), f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInPointToPoint_RoundsIntermediateCoordinatesLikeVB()
        {
            // VB: yy = y1 + (y2 - y1) * (x1 - xx) / (x1 - x2) を Integer に代入するので丸められる
            // (1,1)→(4,2) では xx=3 のとき yy=1.67→2 となり (3,2) を通る
            var f = CreateOpen(5, 5);
            f.Map.AreaInPointToPoint(1, 1, 4, 2);
            CollectionAssert.AreEquivalent(
                MapCoverageFixture.Points("1,1", "2,1", "3,1", "3,2", "4,2").ToList(), f.Unmasked().ToList());
        }

        [TestMethod]
        public void IsLineBlocked_WallBlocksAndBulwarkOnlyBlocksNonFlying()
        {
            // 壁は常に遮る。防壁は飛行中なら遮らない
            var f = new MapCoverageFixture();
            f.LoadGrid(
                ".....",
                "..#..",
                ".....",
                "..B..",
                ".....");

            Assert.IsTrue(f.Map.IsLineBlocked(1, 2, 5, 2));
            Assert.IsTrue(f.Map.IsLineBlocked(1, 2, 5, 2, true));
            Assert.IsFalse(f.Map.IsLineBlocked(1, 1, 5, 1));

            Assert.IsTrue(f.Map.IsLineBlocked(1, 4, 5, 4));
            Assert.IsFalse(f.Map.IsLineBlocked(1, 4, 5, 4, true));

            // 縦方向
            Assert.IsTrue(f.Map.IsLineBlocked(3, 1, 3, 5));
            Assert.IsTrue(f.Map.IsLineBlocked(3, 5, 3, 3));
            Assert.IsFalse(f.Map.IsLineBlocked(3, 5, 3, 3, true));
            Assert.IsFalse(f.Map.IsLineBlocked(4, 1, 4, 5));
            Assert.IsTrue(f.Map.IsLineBlocked(3, 1, 3, 5, true));
        }

        [TestMethod]
        public void IsLineBlocked_RoundsIntermediateCoordinatesLikeVB()
        {
            // VB: (1,1)→(4,2) の直線は xx=3 で yy=2 に丸められ、(3,2) の壁に遮られる
            var f = new MapCoverageFixture();
            f.LoadGrid(
                ".....",
                "..#..",
                ".....");
            Assert.IsTrue(f.Map.IsLineBlocked(1, 1, 4, 2));
        }
    }
}
