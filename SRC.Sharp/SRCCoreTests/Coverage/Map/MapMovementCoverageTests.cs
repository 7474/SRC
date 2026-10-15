using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Map の移動範囲計算（AreaInSpeed / FillMoveCost / ＺＯＣ / AreaInTeleport / AreaInMoveAction /
    /// AreaInReachable）と経路・位置探索（SearchMoveRoute / NearestPoint / SafetyPoint）のテスト。
    /// 移動コストは実際の２倍で記録され、移動力も２倍で比較する。VB: Map.bas の同名関数
    /// </summary>
    [TestClass]
    public class MapMovementCoverageTests
    {
        private static MapCoverageFixture Create(params string[] rows)
        {
            var f = new MapCoverageFixture();
            f.LoadGrid(rows);
            return f;
        }

        [TestMethod]
        public void AreaInSpeed_OpenField_SelectsDiamondOfSpeed()
        {
            // 平地（コスト1）では移動力ぶんのひし形が移動範囲
            var f = Create(".....", ".....", ".....", ".....", ".....");
            f.DefineUnit("歩兵", "陸", 2);
            var u = f.Place("歩兵", "味方", 3, 3);

            f.Map.AreaInSpeed(u);

            Assert.AreEqual(13, f.Unmasked().Count);
            Assert.IsFalse(f.Map.MaskData[5, 3]);
            Assert.IsFalse(f.Map.MaskData[2, 2]);
            Assert.IsTrue(f.Map.MaskData[1, 1]);
            Assert.AreEqual(0, f.Map.TotalMoveCost[3, 3]);
            Assert.AreEqual(2, f.Map.TotalMoveCost[3, 2]);
            Assert.AreEqual(4, f.Map.TotalMoveCost[5, 3]);
        }

        [TestMethod]
        public void AreaInSpeed_TerrainCost_LimitsReach()
        {
            // 山（コスト3）に入るには移動力3が必要
            var f = Create(".M...");
            f.DefineUnit("歩兵", "陸", 2);
            f.DefineUnit("強兵", "陸", 3);
            var weak = f.Place("歩兵", "味方", 1, 1);

            f.Map.AreaInSpeed(weak);
            CollectionAssert.AreEquivalent(new[] { "1,1" }, f.Unmasked().ToList());

            f.Map.MapDataForUnit[1, 1] = null;
            weak.Status = "待機";
            var strong = f.Place("強兵", "味方", 1, 1);
            f.Map.AreaInSpeed(strong);
            Assert.AreEqual(6, f.Map.TotalMoveCost[2, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_Water_RequiresWaterAdaption()
        {
            // 水適応「-」の陸ユニットは水地形に入れない。水適応があればコスト通りに入れる
            var f = Create("..W..");
            f.DefineUnit("陸兵", "陸", 5, "AA-A");
            f.DefineUnit("両用兵", "陸", 5, "AAAA");
            var land = f.Place("陸兵", "味方", 1, 1);

            f.Map.AreaInSpeed(land);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1" }, f.Unmasked().ToList());

            f.Map.MapDataForUnit[1, 1] = null;
            land.Status = "待機";
            var amph = f.Place("両用兵", "味方", 1, 1);
            f.Map.AreaInSpeed(amph);
            Assert.AreEqual(6, f.Map.TotalMoveCost[3, 1]);
            Assert.AreEqual(5, f.Unmasked().Count);
        }

        [TestMethod]
        public void AreaInSpeed_EnemyBlocksButAllyCanBePassedThrough()
        {
            // 敵ユニットのマスは通り抜けられない。味方は通り抜けられるが、そのマスには止まれない
            var corridor = new[] { "#####", ".....", "#####" };
            var f = Create(corridor);
            f.DefineUnit("歩兵", "陸", 4);
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("歩兵", "敵", 3, 2);
            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,2", "2,2" }, f.Unmasked().ToList());

            var f2 = Create(corridor);
            f2.DefineUnit("歩兵", "陸", 4);
            var u2 = f2.Place("歩兵", "味方", 1, 2);
            f2.Place("歩兵", "ＮＰＣ", 3, 2);
            f2.Map.AreaInSpeed(u2);
            CollectionAssert.AreEquivalent(new[] { "1,2", "2,2", "4,2", "5,2" }, f2.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_AllyCarrier_CanBeLandedOn()
        {
            // 母艦を持つ味方ユニットのマスには着艦のため進入できる
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 4);
            f.DefineUnit("母艦", "陸", 4, "AA-A", "母艦");
            var u = f.Place("歩兵", "味方", 1, 1);
            f.Place("母艦", "味方", 3, 1);

            f.Map.AreaInSpeed(u);
            Assert.IsFalse(f.Map.MaskData[3, 1]);
            Assert.IsFalse(f.Map.MaskData[5, 1]);
        }

        [TestMethod]
        public void AreaInSpeed_OtherPartyDoesNotLandOrCombine()
        {
            // 合体・着艦するのは味方のみ。敵ユニット同士は通過できるが、敵の母艦のマスには止まれない
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 4);
            f.DefineUnit("母艦", "陸", 4, "AA-A", "母艦");
            var enemy = f.Place("歩兵", "敵", 1, 1);
            f.Place("母艦", "敵", 3, 1);

            f.Map.AreaInSpeed(enemy);
            Assert.IsTrue(f.Map.MaskData[3, 1]);
            Assert.IsFalse(f.Map.MaskData[4, 1]);
        }

        private static MapCoverageFixture CreateZocField(string enemyFeature, string moverFeature = null)
        {
            // 6x3 の平地、(5,2) の敵の上下 (5,1)(5,3) が ＺＯＣ になると６列目へは抜けられない
            var f = Create("......", "......", "......");
            if (moverFeature == null)
            {
                f.DefineUnit("歩兵", "陸", 6);
            }
            else
            {
                f.DefineUnit("歩兵", "陸", 6, "AA-A", moverFeature);
            }
            if (enemyFeature == null)
            {
                f.DefineUnit("守兵", "陸", 2);
            }
            else
            {
                f.DefineUnit("守兵", "陸", 2, "AA-A", enemyFeature);
            }
            return f;
        }

        [TestMethod]
        public void AreaInSpeed_WithoutZoc_CanGoAroundEnemy()
        {
            var f = CreateZocField(null);
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);

            f.Map.AreaInSpeed(u);
            Assert.IsFalse(f.Map.MaskData[6, 1]);
            Assert.IsFalse(f.Map.MaskData[6, 3]);
            Assert.IsTrue(f.Map.MaskData[5, 2]);
        }

        [TestMethod]
        public void AreaInSpeed_ZocFeature_StopsMovementOnZocCells()
        {
            // ＺＯＣ のマスには進入できるが、そこから先へは進めない
            var f = CreateZocField("ＺＯＣ");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);

            f.Map.AreaInSpeed(u);
            Assert.IsFalse(f.Map.MaskData[5, 1]);
            Assert.IsFalse(f.Map.MaskData[5, 3]);
            Assert.IsTrue(f.Map.MaskData[6, 1]);
            Assert.IsTrue(f.Map.MaskData[6, 3]);
            Assert.IsTrue(f.Map.PointInZOC[5, 1] > 0);
            Assert.IsTrue(f.Map.PointInZOC[4, 2] > 0);
        }

        [TestMethod]
        public void AreaInSpeed_ZocOption_AppliesToAllEnemies()
        {
            // Option「ＺＯＣ」が指定されていれば全ての敵ユニットがＺＯＣを持つ
            var f = CreateZocField(null);
            f.Src.Expression.DefineGlobalVariable("Option(ＺＯＣ)");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);

            f.Map.AreaInSpeed(u);
            Assert.IsTrue(f.Map.MaskData[6, 1]);
            Assert.IsTrue(f.Map.MaskData[6, 3]);
        }

        [TestMethod]
        public void AreaInSpeed_ZocNullification_IgnoresZoc()
        {
            // ＺＯＣ無効化（レベル指定なし）を持つユニットはＺＯＣの影響を受けない
            var f = CreateZocField("ＺＯＣ", "ＺＯＣ無効化");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);

            f.Map.AreaInSpeed(u);
            Assert.IsFalse(f.Map.MaskData[6, 1]);
            Assert.IsFalse(f.Map.MaskData[6, 3]);
        }

        [TestMethod]
        public void AreaInSpeed_ZocLine_OnlyHorizontalAndVertical()
        {
            // 「直線」指定のＺＯＣ範囲2 は上下左右の直線上のみ
            var f = CreateZocField("ＺＯＣ=砂地獄 2 直線");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);

            f.Map.AreaInSpeed(u);
            Assert.IsTrue(f.Map.PointInZOC[3, 2] > 0);
            Assert.IsTrue(f.Map.PointInZOC[4, 2] > 0);
            Assert.IsTrue(f.Map.PointInZOC[5, 1] > 0);
            Assert.AreEqual(0, f.Map.PointInZOC[4, 1]);
            Assert.AreEqual(0, f.Map.PointInZOC[4, 3]);
        }

        [TestMethod]
        public void AreaInSpeed_ZocWithRange_CoversDiamond()
        {
            // 効果範囲2 の全方位ＺＯＣは距離2以内のひし形
            var f = CreateZocField("ＺＯＣ=砂地獄 2");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);

            f.Map.AreaInSpeed(u);
            Assert.IsTrue(f.Map.PointInZOC[3, 2] > 0);
            Assert.IsTrue(f.Map.PointInZOC[4, 1] > 0);
            Assert.AreEqual(0, f.Map.PointInZOC[2, 2]);
            Assert.AreEqual(0, f.Map.PointInZOC[3, 1]);
            // ＺＯＣに入ったところで止まるので (3,2) より先の (4,2) には進めない
            Assert.IsFalse(f.Map.MaskData[3, 2]);
            Assert.IsTrue(f.Map.MaskData[4, 2]);
        }

        [TestMethod]
        public void AreaInSpeed_WideZocNullification_CancelsNearbyZoc()
        {
            // 広域ＺＯＣ無効化を持つ味方ユニットの周囲（範囲3）のＺＯＣは無効になる
            var f = CreateZocField("ＺＯＣ");
            f.DefineUnit("妨害兵", "陸", 2, "AA-A", "広域ＺＯＣ無効化=ＺＯＣ妨害 3");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);
            f.Place("妨害兵", "味方", 3, 3);

            f.Map.AreaInSpeed(u);
            // (5,3) は (3,3) から距離2 なので無効化され、通過して (6,3) に到達できる
            Assert.IsTrue(f.Map.PointInZOC[5, 3] <= 0);
            Assert.IsFalse(f.Map.MaskData[6, 3]);
            // (5,1) は距離4 なのでＺＯＣのまま
            Assert.IsTrue(f.Map.PointInZOC[5, 1] > 0);
        }

        [TestMethod]
        [Ignore("移植の不具合: 隣接ユニットＺＯＣ無効化の判定で、ＺＯＣ側と同じ陣営の隣接ユニットを除外していない")]
        public void AreaInSpeed_AdjacentZocNullification_IgnoresSameSideUnits()
        {
            // VB: 隣接ユニットＺＯＣ無効化は「対抗陣営」の隣接ユニットのみ有効。
            // ＺＯＣを持つ敵の隣にいる敵ユニットが持っていても ＺＯＣ は消えない
            var f = CreateZocField("ＺＯＣ");
            f.DefineUnit("封じ兵", "陸", 2, "AA-A", "隣接ユニットＺＯＣ無効化=ＺＯＣ封じ");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);
            f.Place("封じ兵", "敵", 6, 2);

            f.Map.AreaInSpeed(u);
            Assert.IsTrue(f.Map.PointInZOC[5, 1] > 0);
            Assert.IsTrue(f.Map.MaskData[6, 1]);
        }

        [TestMethod]
        public void AreaInSpeed_AdjacentZocNullification_ByOpposingUnit()
        {
            // 対抗陣営（移動側の味方）の隣接ユニットが隣接ユニットＺＯＣ無効化を持てば ＺＯＣ は発生しない
            var f = CreateZocField("ＺＯＣ");
            f.DefineUnit("封じ兵", "陸", 2, "AA-A", "隣接ユニットＺＯＣ無効化=ＺＯＣ封じ");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);
            f.Place("封じ兵", "味方", 6, 2);

            f.Map.AreaInSpeed(u);
            Assert.AreEqual(0, f.Map.PointInZOC[5, 1]);
            Assert.IsFalse(f.Map.MaskData[6, 1]);
        }

        [TestMethod]
        public void AreaInSpeed_MoveStopTerrain_StopsMovement()
        {
            // 移動停止地形はＺＯＣとして扱われ、進入した時点で止まる
            var f = Create("..S....");
            f.DefineUnit("歩兵", "陸", 5);
            var u = f.Place("歩兵", "味方", 1, 1);

            f.Map.AreaInSpeed(u);
            Assert.AreEqual(20000, f.Map.PointInZOC[3, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "3,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_Remove_SubtractsFirstMoveCost()
        {
            // 再移動時は最初の移動で消費した分だけ移動力が減る
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 3);
            var u = f.Place("歩兵", "味方", 1, 1);
            f.Src.Commands.SelectedCommand = "再移動";
            f.Src.Commands.SelectedUnitMoveCost = 4;

            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_Jump_IgnoresTerrainCostButNotUnenterableCells()
        {
            // ジャンプは移動力＋ジャンプLv、地形コストは一律1。ただし着地できない地形（水）は選べない
            var f = Create(".MW...");
            f.DefineUnit("跳兵", "陸", 1, "AA-A", "ジャンプLv2");
            var u = f.Place("跳兵", "味方", 1, 1);

            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1" }, f.Unmasked().ToList());

            f.Map.AreaInSpeed(u, true);
            Assert.AreEqual(2, f.Map.TotalMoveCost[2, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "4,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_Sky_IsUnenterableOnGround()
        {
            // 空地形は地上ユニットには進入不可
            var f = Create(".K...");
            f.DefineUnit("歩兵", "陸", 4);
            var u = f.Place("歩兵", "味方", 1, 1);

            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_Flying_CostsOnePerCellAnywhere()
        {
            // 空中のユニットは空以外の地形を一律コスト1で移動する
            var f = Create(".MW..");
            f.DefineUnit("飛兵", "空陸", 4);
            var u = f.Place("飛兵", "味方", 1, 1, "空中");

            f.Map.AreaInSpeed(u);
            Assert.AreEqual(4, f.Map.TotalMoveCost[3, 1]);
            Assert.AreEqual(5, f.Unmasked().Count);
        }

        [TestMethod]
        [Ignore("移植の不具合: FillMoveCost が進入不可・移動制限を移動コストでなく MaskData に反映しており、移動範囲に効かない")]
        public void AreaInSpeed_ProhibitedTerrain_CannotBeEntered()
        {
            // VB: 進入不可の地形は move_cost を 1000000 にして進入できなくする
            var f = Create(".M...");
            f.DefineUnit("歩兵", "陸", 5, "AA-A", "進入不可=山嫌い 山");
            var u = f.Place("歩兵", "味方", 1, 1);

            f.Map.AreaInSpeed(u);
            Assert.IsTrue(f.Map.MaskData[2, 1]);
            Assert.IsTrue(f.Map.MaskData[3, 1]);
        }

        [TestMethod]
        public void SearchMoveRoute_TracesBackFromTargetToOrigin()
        {
            // 経路は目的地から現在地へ向かって必要移動力の低い隣接マスを辿る
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 4);
            var u = f.Place("歩兵", "味方", 1, 1);
            f.Map.AreaInSpeed(u);

            f.Map.SearchMoveRoute(4, 1, out var xs, out var ys);
            CollectionAssert.AreEqual(new[] { 4, 3, 2, 1 }, xs.ToArray());
            CollectionAssert.AreEqual(new[] { 1, 1, 1, 1 }, ys.ToArray());
        }

        [TestMethod]
        public void SearchMoveRoute_DetoursAroundBlockedCell()
        {
            // 通れないマスを迂回する経路になる
            var f = Create("...", ".#.", "...");
            f.DefineUnit("歩兵", "陸", 4);
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Map.AreaInSpeed(u);

            f.Map.SearchMoveRoute(3, 2, out var xs, out var ys);
            Assert.AreEqual(5, xs.Count);
            Assert.AreEqual(3, xs.First());
            Assert.AreEqual(2, ys.First());
            Assert.AreEqual(1, xs.Last());
            Assert.AreEqual(2, ys.Last());
            Assert.IsFalse(xs.Zip(ys, (x, y) => x == 2 && y == 2).Any(b => b));
            // 隣接マスを１つずつ辿る
            for (var i = 1; i < xs.Count; i++)
            {
                Assert.AreEqual(1, System.Math.Abs(xs[i] - xs[i - 1]) + System.Math.Abs(ys[i] - ys[i - 1]));
            }
        }

        [TestMethod]
        [Ignore("移植の不具合: SearchMoveRoute の直進距離 MovedUnitSpeed の判定条件が反転しており、先頭の移動方向も飛ばしている")]
        public void SearchMoveRoute_StraightRoute_CountsStraightDistance()
        {
            // VB: 目的地側から同じ方向が続く数を数える。３マス直進なら 3
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 4);
            var u = f.Place("歩兵", "味方", 1, 1);
            f.Map.AreaInSpeed(u);

            f.Map.SearchMoveRoute(4, 1, out _, out _);
            Assert.AreEqual(3, f.Src.Commands.MovedUnitSpeed);
        }

        [TestMethod]
        public void AreaInTeleport_SelectsDiamondExceptUnenterableAndOccupied()
        {
            // テレポートは地形コストを無視して距離 lv 以内に移動できるが、進入できない地形やユニットのいるマスは除く
            var f = Create(
                ".....",
                "..W..",
                ".....",
                ".....",
                ".....");
            f.DefineUnit("歩兵", "陸", 1);
            var u = f.Place("歩兵", "味方", 3, 3);
            f.Place("歩兵", "敵", 3, 5);
            f.Place("歩兵", "味方", 4, 3);

            f.Map.AreaInTeleport(u, 2);
            var set = f.Unmasked();
            Assert.AreEqual(13 - 3, set.Count);
            Assert.IsFalse(set.Contains("3,2")); // 水
            Assert.IsFalse(set.Contains("3,5")); // 敵
            Assert.IsFalse(set.Contains("4,3")); // 味方（母艦でない）
            Assert.IsTrue(set.Contains("3,1"));
            Assert.IsTrue(set.Contains("3,3"));
        }

        [TestMethod]
        public void AreaInTeleport_DefaultRange_IsSpeedPlusTeleportLevel()
        {
            // lv 省略時は 移動力＋テレポートLv
            var f = Create(".......");
            f.DefineUnit("瞬兵", "陸", 1, "AA-A", "テレポートLv2");
            var u = f.Place("瞬兵", "味方", 1, 1);

            f.Map.AreaInTeleport(u);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "3,1", "4,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInMoveAction_ExcludesWallsLineBlockedWaterAndUnits()
        {
            // Ｍ移の移動先: 射程内で、進入可能な地形・ユニット無し・壁に遮られていないマス
            var f = Create(
                ".....",
                "..#..",
                "W....");
            f.DefineUnit("歩兵", "陸", 3);
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("歩兵", "味方", 2, 1);

            f.Map.AreaInMoveAction(u, 3);
            Assert.IsTrue(f.Map.MaskData[3, 2]);  // 壁
            Assert.IsTrue(f.Map.MaskData[4, 2]);  // 壁の向こう
            Assert.IsTrue(f.Map.MaskData[1, 3]);  // 水
            Assert.IsTrue(f.Map.MaskData[2, 1]);  // ユニット
            Assert.IsTrue(f.Map.MaskData[5, 2]);  // 射程外
            Assert.IsFalse(f.Map.MaskData[1, 2]); // 現在地
            Assert.IsFalse(f.Map.MaskData[2, 2]);
            Assert.IsFalse(f.Map.MaskData[1, 1]);
            Assert.IsFalse(f.Map.MaskData[2, 3]);
        }

        [TestMethod]
        public void AreaInReachable_ExpandsMoveAreaByRangeAndFiltersParty()
        {
            // 移動範囲を射程ぶん広げた範囲が対象。範囲内の味方ユニットは「味方の敵」指定で外れる
            var f = Create(".......", ".......", ".......", ".......", ".......", ".......", ".......");
            f.DefineUnit("歩兵", "陸", 1);
            var u = f.Place("歩兵", "味方", 4, 4);
            f.Place("歩兵", "敵", 4, 2);
            f.Place("歩兵", "味方", 6, 4);

            f.Map.AreaInReachable(u, 1, "味方の敵");
            var set = f.Unmasked();
            Assert.AreEqual(13 - 1, set.Count);
            Assert.IsTrue(set.Contains("4,2"));   // 敵
            Assert.IsFalse(set.Contains("6,4"));  // 味方
            Assert.IsTrue(set.Contains("4,4"));   // 中心
            Assert.IsTrue(set.Contains("3,3"));
            Assert.IsFalse(set.Contains("4,1"));
        }

        [TestMethod]
        public void NearestPoint_ChoosesReachableCellClosestToDestination()
        {
            // 移動範囲内で目的地への必要移動力が最小のマスを選ぶ
            var f = Create(".......");
            f.DefineUnit("歩兵", "陸", 2);
            var u = f.Place("歩兵", "味方", 1, 1);
            f.Map.AreaInSpeed(u);

            f.Map.NearestPoint(u, 7, 1, out var x, out var y);
            Assert.AreEqual(3, x);
            Assert.AreEqual(1, y);

            // マップ外の目的地はマップ内に丸める
            f.Map.NearestPoint(u, 99, -5, out x, out y);
            Assert.AreEqual(3, x);
            Assert.AreEqual(1, y);
        }

        [TestMethod]
        public void NearestPoint_AvoidsImpassableTerrainCost()
        {
            // 直線距離が近くても通れない地形を挟むマスより、実際に辿り着けるマスを優先する
            var f = Create(
                ".....",
                ".###.",
                ".....");
            f.DefineUnit("歩兵", "陸", 3);
            var u = f.Place("歩兵", "味方", 3, 1);
            f.Map.AreaInSpeed(u);

            f.Map.NearestPoint(u, 3, 3, out var x, out var y);
            // 壁を迂回して (1,2) か (5,2) へ向かう
            Assert.AreEqual(2, y);
            Assert.IsTrue(x == 1 || x == 5, "x=" + x);
        }

        [TestMethod]
        public void SafetyPoint_ChoosesReachableCellFarthestFromEnemies()
        {
            // 移動範囲内で最も敵から遠いマスを選ぶ
            var f = Create(".......");
            f.DefineUnit("歩兵", "陸", 2);
            var u = f.Place("歩兵", "味方", 4, 1);
            f.Place("歩兵", "敵", 1, 1);
            f.Map.AreaInSpeed(u);

            f.Map.SafetyPoint(u, out var x, out var y);
            Assert.AreEqual(6, x);
            Assert.AreEqual(1, y);
        }
    }
}
