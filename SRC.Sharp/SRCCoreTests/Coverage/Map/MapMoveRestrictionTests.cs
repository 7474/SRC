using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// 移動制限で地形が指定されていない場合のテスト。
    /// VB: Map.bas AreaInSpeed（FillMoveCost 相当）・NearestPoint は
    /// For k = 2 To n で一致する地形がなければ（k > n）move_cost を 1000000 にするので、
    /// 地形の列挙がなければ全ての地形に進入できない。
    /// （NearestPoint も同じ移動コストを使う）
    /// </summary>
    [TestClass]
    public class MapMoveRestrictionTests
    {
        private static MapCoverageFixture Create(params string[] rows)
        {
            var f = new MapCoverageFixture();
            f.LoadGrid(rows);
            return f;
        }

        [TestMethod]
        public void AreaInSpeed_MoveRestrictionWithoutTerrains_CannotMove()
        {
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 5, "AA-A", "移動制限=移動不能");
            var u = f.Place("歩兵", "味方", 1, 1);

            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInTeleport_MoveRestrictionWithoutTerrains_CannotTeleport()
        {
            // VB: AreaInTeleport は UBound(allowed_terrains) > 0（データが空でない）なら
            // 一致する地形がない地点をマスクする
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 5, "AA-A", "移動制限=移動不能");
            var u = f.Place("歩兵", "味方", 1, 1);

            f.Map.AreaInTeleport(u, 3);
            CollectionAssert.AreEquivalent(new[] { "1,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInTeleport_NoMoveRestriction_CanTeleport()
        {
            // 比較用: 移動制限がなければテレポートできる
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 5);
            var u = f.Place("歩兵", "味方", 1, 1);

            f.Map.AreaInTeleport(u, 3);
            Assert.IsTrue(f.Unmasked().Count() > 1);
        }
    }
}
