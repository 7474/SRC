using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Map.bas の AreaInSpeed・AreaInTeleport・NearestPoint・SafetyPoint を VB 版と分岐ごとに突き合わせたテスト（7474/SRC#1498）。
    /// 期待値は元の SRC（SRC/SRC_20121125/Map.bas）の動作。
    /// </summary>
    [TestClass]
    public class MapMoveVbBranchTests
    {
        private static MapCoverageFixture Create(params string[] rows)
        {
            var f = new MapCoverageFixture();
            f.LoadGrid(rows);
            return f;
        }

        #region AreaInSpeed

        [TestMethod]
        public void AreaInSpeed_MultipleTerrainAdaptions_UsesAllOfThem()
        {
            // VB: 地形適応の地形のリストは、すべての「地形適応」の能力から集める。
            // ２つ目の地形適応で山を指定すれば、山の移動コストは 1（内部では 2）になる
            var f = Create(".M..");
            f.DefineUnit("歩兵", "陸", 2, "AA-A", "地形適応=平地戦用 平地", "地形適応=山岳戦用 山");
            var u = f.Place("歩兵", "味方", 1, 1);

            f.Map.AreaInSpeed(u);
            Assert.AreEqual(2, f.Map.TotalMoveCost[2, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "3,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_UnknownArea_CannotMove()
        {
            // VB: 移動コストの Select Case move_area は扱わない場所では何もしないので、
            // 移動コストは 1000000 のまま（例外にはならず、現在いる場所だけが選べる）
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 3);
            var u = f.Place("歩兵", "味方", 1, 1, area: "");

            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1" }, f.Unmasked().ToList());
        }

        private static MapCoverageFixture CreateZocField(string enemyFeature, params string[] moverFeatures)
        {
            // 6x3 の平地、移動側は (1,2)、ＺＯＣを持つ敵は (5,2)
            var f = Create("......", "......", "......");
            f.DefineUnit("歩兵", "陸", 6, "AA-A", moverFeatures);
            f.DefineUnit("守兵", "陸", 2, "AA-A", enemyFeature);
            return f;
        }

        [TestMethod]
        public void AreaInSpeed_ZocNullificationLevel_IsRoundedToInteger()
        {
            // VB: l（Integer）= u.FeatureLevel("ＺＯＣ無効化") なので、Lv2.6 は 3 に丸めてから ＺＯＣLv3 と比べる（3 >= 3 で無効）
            var f = CreateZocField("ＺＯＣLv3", "ＺＯＣ無効化Lv2.6");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);

            f.Map.AreaInSpeed(u);
            Assert.AreEqual(0, f.Map.PointInZOC[5, 1]);
            Assert.IsFalse(f.Map.MaskData[6, 1]);
        }

        [TestMethod]
        public void AreaInSpeed_AdjacentZocNullificationLevel_IsRoundedToInteger()
        {
            // VB: 隣接ユニットＺＯＣ無効化のレベルも Integer の l に入れて（偶数丸め）から ＺＯＣLv と比べる
            var f = CreateZocField("ＺＯＣLv3");
            f.DefineUnit("封じ兵", "陸", 2, "AA-A", "隣接ユニットＺＯＣ無効化Lv2.6");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);
            f.Place("封じ兵", "味方", 6, 2);

            f.Map.AreaInSpeed(u);
            Assert.AreEqual(0, f.Map.PointInZOC[5, 1]);
            Assert.IsFalse(f.Map.MaskData[6, 1]);
        }

        [TestMethod]
        public void AreaInSpeed_WideZocNullification_UsesZareaOfPreviousBlockingUnit()
        {
            // VB: zarea は手続きの変数で、ＺＯＣの判定のときだけ設定し直す。
            // 「広域ＺＯＣ無効化」の「相対距離 - zarea <= 移動力」には、前に判定した敵の zarea（ここでは 5）が使われる。
            // 移動側 (1,1) の移動力は 1（内部では 2）、妨害兵 (5,1) は距離 4 なので、zarea が 0 なら処理されない
            var f = Create("............");
            f.DefineUnit("歩兵", "陸", 1);
            f.DefineUnit("守兵", "陸", 2, "AA-A", "ＺＯＣ=砂地獄 5");
            f.DefineUnit("妨害兵", "陸", 2, "AA-A", "広域ＺＯＣ無効化=ＺＯＣ妨害 1");
            var u = f.Place("歩兵", "味方", 1, 1);
            f.Place("守兵", "敵", 12, 1);
            f.Place("妨害兵", "味方", 5, 1);

            f.Map.AreaInSpeed(u);
            Assert.AreEqual(-10000, f.Map.PointInZOC[4, 1]);
            Assert.AreEqual(-10000, f.Map.PointInZOC[6, 1]);
        }

        [TestMethod]
        public void AreaInSpeed_CombinePartnerWithCombineRestriction_CanCombine()
        {
            // VB: 合体の相手が u2 そのもの（u2.Name = LIndex(buf, 3)）なら、u2 の合体制限に関わらず合体のために進入できる。
            // 合体制限を見るのは、相手の現在の形態が u2 の場合（ElseIf）だけ
            var f = Create(".....");
            f.DefineUnit("A機", "陸", 4, "AA-A", "合体=合体 合体形態 B機");
            f.DefineUnit("B機", "陸", 4, "AA-A", "合体=合体 合体形態 A機", "合体制限");
            f.DefineUnit("合体形態", "陸", 4);
            f.Src.UList.Add("合体形態", 0, "味方").Status = "他形態";
            var a = f.Place("A機", "味方", 1, 1);
            f.Place("B機", "味方", 3, 1);

            f.Map.AreaInSpeed(a);
            Assert.IsFalse(f.Map.MaskData[3, 1]);
        }

        [TestMethod]
        public void AreaInSpeed_CombinedUnitDisabled_StopsCheckingOtherCombineFeatures()
        {
            // VB: 合体後のユニットが行動不能なら Exit For で残りの「合体」の能力を見ない。
            // ２つ目の合体能力では合体できても、そのマスには進入できない
            var f = Create(".....");
            f.DefineUnit("A機", "陸", 4, "AA-A", "合体=合体 合体形態1 B機", "合体=合体 合体形態2 B機");
            f.DefineUnit("B機", "陸", 4, "AA-A", "合体=合体 合体形態1 A機");
            f.DefineUnit("合体形態1", "陸", 4);
            f.DefineUnit("合体形態2", "陸", 4);
            var c1 = f.Src.UList.Add("合体形態1", 0, "味方");
            c1.Status = "他形態";
            c1.AddCondition("行動不能", 1);
            f.Src.UList.Add("合体形態2", 0, "味方").Status = "他形態";
            var a = f.Place("A機", "味方", 1, 1);
            f.Place("B機", "味方", 3, 1);

            f.Map.AreaInSpeed(a);
            Assert.IsTrue(f.Map.MaskData[3, 1]);
        }

        [TestMethod]
        public void AreaInSpeed_CombineFeatureOrder_FirstAvailableFeatureAllowsCombine()
        {
            // 比較用: 合体後のユニットが行動不能でなければ、同じ配置で合体のために進入できる
            var f = Create(".....");
            f.DefineUnit("A機", "陸", 4, "AA-A", "合体=合体 合体形態1 B機", "合体=合体 合体形態2 B機");
            f.DefineUnit("B機", "陸", 4, "AA-A", "合体=合体 合体形態1 A機");
            f.DefineUnit("合体形態1", "陸", 4);
            f.DefineUnit("合体形態2", "陸", 4);
            f.Src.UList.Add("合体形態1", 0, "味方").Status = "他形態";
            f.Src.UList.Add("合体形態2", 0, "味方").Status = "他形態";
            var a = f.Place("A機", "味方", 1, 1);
            f.Place("B機", "味方", 3, 1);

            f.Map.AreaInSpeed(a);
            Assert.IsFalse(f.Map.MaskData[3, 1]);
        }

        [TestMethod]
        public void AreaInSpeed_DisabledCarrier_CanStillBeLandedOn()
        {
            // VB: AreaInSpeed の着艦の判定は母艦の封印（IsDisabled）を見ない
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 4);
            f.DefineUnit("母艦", "陸", 4, "AA-A", "母艦");
            var u = f.Place("歩兵", "味方", 1, 1);
            f.Place("母艦", "味方", 3, 1);
            f.Src.Expression.DefineGlobalVariable("Disable(母艦)");

            f.Map.AreaInSpeed(u);
            Assert.IsFalse(f.Map.MaskData[3, 1]);
        }

        private const int Space = 10;

        private static MapCoverageFixture CreateRow(params int[] row)
        {
            // 宇宙（宇宙 コスト1）を追加した１行マップ
            var f = new MapCoverageFixture();
            f.Src.TDList.Load("terrain3.txt", MapCoverageFixture.ToStream(
                "10\r\n宇宙, space\r\n宇宙, 1, 0, 0\r\n\r\n"));
            var sb = new System.Text.StringBuilder();
            sb.Append("MapData\r\n\r\n" + row.Length + ",1\r\n");
            foreach (var t in row)
            {
                sb.Append(t + ",0\r\n");
            }
            f.Map.LoadMapData("row.map", MapCoverageFixture.ToStream(sb.ToString()));
            return f;
        }

        [TestMethod]
        public void AreaInSpeed_SpaceArea_LandNeedsSkyOrGroundTransportation()
        {
            // VB: 宇宙にいるユニットは、陸・屋内を空を飛べれば（is_trans_available_in_sky）コスト 1、
            // 陸を移動できればその地形のコスト、どちらも無ければ進入できない
            var row = new[] { Space, Space, MapCoverageFixture.Plain, Space };
            var f = CreateRow(row);
            f.DefineUnit("宇宙艇", "宇宙", 4, "---A");
            var u = f.Place("宇宙艇", "味方", 1, 1, area: "宇宙");
            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1" }, f.Unmasked().ToList());

            var f2 = CreateRow(row);
            f2.DefineUnit("宇宙機", "空宇宙", 4, "A--A");
            var u2 = f2.Place("宇宙機", "味方", 1, 1, area: "宇宙");
            f2.Map.AreaInSpeed(u2);
            Assert.AreEqual(4, f2.Map.TotalMoveCost[3, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "3,1", "4,1" }, f2.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_GroundArea_SpaceNeedsSpaceAdaption()
        {
            // VB: 地上のユニットは、宇宙の地形には宇宙適応（または宇宙移動）があるときだけ進入できる
            var row = new[] { MapCoverageFixture.Plain, Space, MapCoverageFixture.Plain };
            var f = CreateRow(row);
            f.DefineUnit("歩兵", "陸", 3, "AA--");
            var u = f.Place("歩兵", "味方", 1, 1);
            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1" }, f.Unmasked().ToList());

            var f2 = CreateRow(row);
            f2.DefineUnit("歩兵", "陸", 3, "AA--", "宇宙移動");
            var u2 = f2.Place("歩兵", "味方", 1, 1);
            f2.Map.AreaInSpeed(u2);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "3,1" }, f2.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_UndergroundArea_OnlyLandAtCostOne()
        {
            // VB: 地中のユニットは陸（・月面）をコスト 1 で移動し、それ以外には進入できない
            var f = Create(".MM.W");
            f.DefineUnit("モグラ", "陸", 4);
            var u = f.Place("モグラ", "味方", 1, 1, area: "地中");

            f.Map.AreaInSpeed(u);
            Assert.AreEqual(4, f.Map.TotalMoveCost[3, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "3,1", "4,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_UnderwaterArea_DeepWaterNeedsSwimming()
        {
            // VB: 水中のユニットは、水を移動できなければ水はその地形のコスト、深水は水泳があるときだけ進入できる
            var f = Create("WWDW");
            f.DefineUnit("潜水兵", "陸", 4, "AAAA");
            var u = f.Place("潜水兵", "味方", 1, 1, area: "水中");
            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1" }, f.Unmasked().ToList());

            var f2 = Create("WWDW");
            f2.DefineUnit("潜水兵", "陸", 4, "AAAA", "水泳");
            var u2 = f2.Place("潜水兵", "味方", 1, 1, area: "水中");
            f2.Map.AreaInSpeed(u2);
            Assert.AreEqual(8, f2.Map.TotalMoveCost[3, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "3,1" }, f2.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_AirArea_SpaceNeedsSpaceAdaption()
        {
            // VB: 空中のユニットは、空以外の地形をコスト 1 で移動するが、宇宙には宇宙適応が要る
            var row = new[] { MapCoverageFixture.Plain, Space, MapCoverageFixture.Plain };
            var f = CreateRow(row);
            f.DefineUnit("戦闘機", "空", 3, "A---");
            var u = f.Place("戦闘機", "味方", 1, 1, area: "空中");
            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_Immobile_CannotMove()
        {
            // VB: 移動不能の状態では移動力が 0 になる
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 3);
            var u = f.Place("歩兵", "味方", 3, 1);
            u.AddCondition("移動不能", 1);

            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "3,1" }, f.Unmasked().ToList());
        }

        #endregion

        #region AreaInTeleport

        [TestMethod]
        public void AreaInTeleport_DisabledCarrier_CannotBeLandedOn()
        {
            // VB: AreaInTeleport は Not u2.IsDisabled("母艦") のときだけ着艦のために進入できる
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 4);
            f.DefineUnit("母艦", "陸", 4, "AA-A", "母艦");
            var u = f.Place("歩兵", "味方", 1, 1);
            f.Place("母艦", "味方", 3, 1);

            f.Map.AreaInTeleport(u, 3);
            Assert.IsFalse(f.Map.MaskData[3, 1]);

            f.Src.Expression.DefineGlobalVariable("Disable(母艦)");
            f.Map.AreaInTeleport(u, 3);
            Assert.IsTrue(f.Map.MaskData[3, 1]);
        }

        #endregion

        #region NearestPoint

        [TestMethod]
        public void NearestPoint_JumpMovement_UsesCostOneOnLand()
        {
            // VB: NearestPoint は「ジャンプ移動」を持つ地上ユニットの陸・月面の移動コストを 1 にする（AreaInSpeed には無い）。
            // 山越えの (1,1) からの方が目的地 (5,1) に近いとみなすので、下を回る (1,3) ではなく (1,1) を選ぶ
            var rows = new[] { ".MMM.", ".###.", "....." };
            var f = Create(rows);
            f.DefineUnit("跳兵", "陸", 1, "AA-A", "ジャンプ移動");
            var u = f.Place("跳兵", "味方", 1, 2);
            f.Map.AreaInSpeed(u);

            f.Map.NearestPoint(u, 5, 1, out var x, out var y);
            Assert.AreEqual(1, x);
            Assert.AreEqual(1, y);

            // 比較用: ジャンプ移動が無ければ山のコストが高いので (1,3) を選ぶ
            var f2 = Create(rows);
            f2.DefineUnit("歩兵", "陸", 1);
            var u2 = f2.Place("歩兵", "味方", 1, 2);
            f2.Map.AreaInSpeed(u2);

            f2.Map.NearestPoint(u2, 5, 1, out x, out y);
            Assert.AreEqual(1, x);
            Assert.AreEqual(3, y);
        }

        [TestMethod]
        public void NearestPoint_WaterSurfaceMovementWithoutWaterAdaption_TreatsWaterAsImpassable()
        {
            // VB: NearestPoint の地上の「水」は水中を移動できる（is_trans_available_in_water）ときだけコスト 2 で、
            // 水上移動では通れない（AreaInSpeed の移動コストとは違う）。
            // 水を渡る (1,1) からの経路は使えないとみなし、陸を回る (1,3) を選ぶ
            var f = Create(".WWW.", ".###.", ".....");
            f.DefineUnit("ボート", "陸", 1, "AA-A", "水上移動");
            var u = f.Place("ボート", "味方", 1, 2);
            f.Map.AreaInSpeed(u);
            Assert.IsFalse(f.Map.MaskData[1, 1]);

            f.Map.NearestPoint(u, 5, 1, out var x, out var y);
            Assert.AreEqual(1, x);
            Assert.AreEqual(3, y);
        }

        #endregion

        #region SafetyPoint

        [TestMethod]
        public void SafetyPoint_NoCandidate_KeepsPassedCoordinates()
        {
            // VB: X・Y は ByRef。移動できる空きマスが無ければ、渡された値のまま残す
            var f = Create(".....");
            f.DefineUnit("歩兵", "陸", 2);
            var u = f.Place("歩兵", "味方", 3, 1);
            f.Place("歩兵", "敵", 1, 1);
            f.Map.ClearMask();
            f.Map.MaskData[3, 1] = false;

            int x = 0, y = 0;
            f.Map.SafetyPoint(u, ref x, ref y);
            Assert.AreEqual(0, x);
            Assert.AreEqual(0, y);
        }

        #endregion
    }
}
