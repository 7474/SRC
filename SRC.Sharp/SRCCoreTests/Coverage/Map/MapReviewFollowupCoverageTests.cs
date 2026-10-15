using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Map のレビュー指摘で不足していた挙動のテスト。
    /// 陣営指定の別名・行動不能な味方の扱い、移動系特殊能力（すり抜け移動／透過移動／水上移動／ホバー移動／線路移動）、
    /// ＺＯＣの水平・垂直指定とレベル比較、マップファイルの読み込みとクリア。VB: Map.bas の同名関数
    /// </summary>
    [TestClass]
    public class MapReviewFollowupCoverageTests
    {
        private static MapCoverageFixture Create(params string[] rows)
        {
            var f = new MapCoverageFixture();
            f.LoadGrid(rows);
            return f;
        }

        #region AreaInRange の陣営指定

        [TestMethod]
        public void AreaInRange_NpcAliases_BehaveLikeAlly()
        {
            // 「ＮＰＣ」「ＮＰＣの敵」は「味方」「味方の敵」と同じ扱い（VB: AreaInRange の Case "味方", "ＮＰＣ"）
            var f = Create(".....", ".....", ".....", ".....", ".....");
            f.DefineUnit("歩兵", "陸", 2);
            f.Place("歩兵", "味方", 3, 2);
            f.Place("歩兵", "ＮＰＣ", 2, 3);
            f.Place("歩兵", "敵", 3, 4);

            f.Map.AreaInRange(3, 3, 2, 1, "ＮＰＣ");
            Assert.IsFalse(f.Map.MaskData[3, 2]);
            Assert.IsFalse(f.Map.MaskData[2, 3]);
            Assert.IsTrue(f.Map.MaskData[3, 4]);

            f.Map.AreaInRange(3, 3, 2, 1, "ＮＰＣの敵");
            Assert.IsTrue(f.Map.MaskData[3, 2]);
            Assert.IsTrue(f.Map.MaskData[2, 3]);
            Assert.IsFalse(f.Map.MaskData[3, 4]);
        }

        [TestMethod]
        public void AreaInRange_EnemyOfAlly_KeepsDisabledAlliesSelectable()
        {
            // 「味方の敵」でも暴走・魅了・混乱・憑依・睡眠状態の味方／ＮＰＣは対象として選べる
            // （VB: AreaInRange の Case "味方の敵", "ＮＰＣの敵"）
            var f = Create(".....", ".....", ".....", ".....", ".....");
            f.DefineUnit("歩兵", "陸", 2);
            var confused = f.Place("歩兵", "味方", 3, 2);
            confused.AddCondition("混乱", 1);
            var charmed = f.Place("歩兵", "ＮＰＣ", 2, 3);
            charmed.AddCondition("魅了", 1);
            f.Place("歩兵", "味方", 4, 3); // 通常の味方
            var sleeping = f.Place("歩兵", "味方", 3, 4);
            sleeping.AddCondition("睡眠", 1);

            f.Map.AreaInRange(3, 3, 2, 1, "味方の敵");
            Assert.IsFalse(f.Map.MaskData[3, 2]);
            Assert.IsFalse(f.Map.MaskData[2, 3]);
            Assert.IsFalse(f.Map.MaskData[3, 4]);
            Assert.IsTrue(f.Map.MaskData[4, 3]);
        }

        [TestMethod]
        public void AreaInReachable_EnemyOfAlly_KeepsConfusedAllySelectable()
        {
            // AreaInReachable の「味方の敵」も同じ規則で混乱した味方を残す
            var f = Create(".......", ".......", ".......", ".......", ".......", ".......", ".......");
            f.DefineUnit("歩兵", "陸", 1);
            var u = f.Place("歩兵", "味方", 4, 4);
            var confused = f.Place("歩兵", "味方", 6, 4);
            confused.AddCondition("混乱", 1);
            f.Place("歩兵", "味方", 4, 6);

            f.Map.AreaInReachable(u, 1, "味方の敵");
            Assert.IsFalse(f.Map.MaskData[6, 4]);
            Assert.IsTrue(f.Map.MaskData[4, 6]);
        }

        #endregion

        #region 移動系特殊能力

        [TestMethod]
        public void AreaInSpeed_SlipThrough_PassesEnemyButCannotStopOnIt()
        {
            // すり抜け移動を持つユニットは敵ユニットのマスを通過できる（停止はできない）
            var f = Create("#####", ".....", "#####");
            f.DefineUnit("忍者", "陸", 4, "AA-A", "すり抜け移動");
            var u = f.Place("忍者", "味方", 1, 2);
            f.Place("忍者", "敵", 3, 2);

            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,2", "2,2", "4,2", "5,2" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_PassThrough_IgnoresTerrainButCannotEndOnWater()
        {
            // 透過移動は全地形のコストが1になり壁も通過できる。ただし水適応がなければ水上には止まれない
            var f = Create(".#W...");
            f.DefineUnit("幽霊", "陸", 3, "AA-A", "透過移動");
            var u = f.Place("幽霊", "味方", 1, 1);

            f.Map.AreaInSpeed(u);
            Assert.AreEqual(4, f.Map.TotalMoveCost[3, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "4,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_WaterSurfaceMovement_CrossesWaterAtCostOne()
        {
            // 水上移動は水適応が無くても水・深水をコスト1で移動できる
            var f = Create("..WD..");
            f.DefineUnit("ボート", "陸", 4, "AA-A", "水上移動");
            var u = f.Place("ボート", "味方", 1, 1);

            f.Map.AreaInSpeed(u);
            Assert.AreEqual(6, f.Map.TotalMoveCost[4, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "3,1", "4,1", "5,1" }, f.Unmasked().ToList());
        }

        private const int Desert = 8;
        private const int Rail = 9;

        private static MapCoverageFixture CreateWithExtraTerrain(params int[] row)
        {
            // 砂漠（コスト3）と線路を追加した１行マップ
            var f = new MapCoverageFixture();
            f.Src.TDList.Load("terrain2.txt", MapCoverageFixture.ToStream(
                "8\r\n砂漠, desert\r\n陸, 3, 0, 0\r\n\r\n" +
                "9\r\n線路, rail\r\n陸, 1, 0, 0\r\n\r\n"));
            var sb = new StringBuilder();
            sb.Append("MapData\r\n\r\n" + row.Length + ",1\r\n");
            foreach (var t in row)
            {
                sb.Append(t + ",0\r\n");
            }
            f.Map.LoadMapData("extra.map", MapCoverageFixture.ToStream(sb.ToString()));
            return f;
        }

        [TestMethod]
        public void AreaInSpeed_Hover_CrossesDesertAtCostOne()
        {
            // ホバー移動は砂漠・雪原の移動コストが1になる
            var f = CreateWithExtraTerrain(MapCoverageFixture.Plain, Desert, Desert, MapCoverageFixture.Plain);
            f.DefineUnit("歩兵", "陸", 3);
            f.DefineUnit("ホバー", "陸", 3, "AA-A", "ホバー移動");
            var walker = f.Place("歩兵", "味方", 1, 1);

            f.Map.AreaInSpeed(walker);
            Assert.AreEqual(6, f.Map.TotalMoveCost[2, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1" }, f.Unmasked().ToList());

            f.Map.MapDataForUnit[1, 1] = null;
            walker.Status = "待機";
            var hover = f.Place("ホバー", "味方", 1, 1);
            f.Map.AreaInSpeed(hover);
            Assert.AreEqual(4, f.Map.TotalMoveCost[3, 1]);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1", "3,1", "4,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_RailMovement_OnlyOnRails()
        {
            // 線路移動を持つ地上ユニットは「線路」地形しか移動できない（線路のコストは1）
            var f = CreateWithExtraTerrain(Rail, Rail, MapCoverageFixture.Plain, Rail);
            f.DefineUnit("列車", "陸", 5, "AA-A", "線路移動");
            var u = f.Place("列車", "味方", 1, 1);

            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1", "2,1" }, f.Unmasked().ToList());
        }

        [TestMethod]
        public void AreaInSpeed_AllowedTerrains_CannotEnterOtherTerrain()
        {
            // VB: 移動制限で列挙されていない地形は move_cost を 1000000 にして進入・通過できなくする
            var f = Create(".M...");
            f.DefineUnit("歩兵", "陸", 5, "AA-A", "移動制限=平地限定 平地");
            var u = f.Place("歩兵", "味方", 1, 1);

            f.Map.AreaInSpeed(u);
            CollectionAssert.AreEquivalent(new[] { "1,1" }, f.Unmasked().ToList());
        }

        #endregion

        #region ＺＯＣ

        private static MapCoverageFixture CreateZocField(string enemyFeature, string moverFeature = null)
        {
            // 6x3 の平地、移動側は (1,2)、ＺＯＣを持つ敵は (5,2)
            var f = Create("......", "......", "......");
            if (moverFeature == null)
            {
                f.DefineUnit("歩兵", "陸", 6);
            }
            else
            {
                f.DefineUnit("歩兵", "陸", 6, "AA-A", moverFeature);
            }
            f.DefineUnit("守兵", "陸", 2, "AA-A", enemyFeature);
            return f;
        }

        [TestMethod]
        public void AreaInSpeed_HorizontalZoc_OnlyAlongRow()
        {
            // 「水平」指定のＺＯＣは左右方向のみ。上下には及ばないので上側を回り込める
            var f = CreateZocField("ＺＯＣ=砂地獄 2 水平");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);

            f.Map.AreaInSpeed(u);
            Assert.IsTrue(f.Map.PointInZOC[3, 2] > 0);
            Assert.IsTrue(f.Map.PointInZOC[6, 2] > 0);
            Assert.AreEqual(0, f.Map.PointInZOC[5, 1]);
            Assert.AreEqual(0, f.Map.PointInZOC[4, 1]);
            Assert.IsFalse(f.Map.MaskData[6, 1]);
        }

        [TestMethod]
        public void AreaInSpeed_VerticalZoc_OnlyAlongColumn()
        {
            // 「垂直」指定のＺＯＣは上下方向のみ
            var f = CreateZocField("ＺＯＣ=砂地獄 2 垂直");
            var u = f.Place("歩兵", "味方", 1, 2);
            f.Place("守兵", "敵", 5, 2);

            f.Map.AreaInSpeed(u);
            Assert.IsTrue(f.Map.PointInZOC[5, 1] > 0);
            Assert.IsTrue(f.Map.PointInZOC[5, 3] > 0);
            Assert.AreEqual(0, f.Map.PointInZOC[4, 2]);
            Assert.AreEqual(0, f.Map.PointInZOC[3, 2]);
            // (5,1)(5,3) で止まるので６列目へは抜けられない
            Assert.IsTrue(f.Map.MaskData[6, 1]);
            Assert.IsTrue(f.Map.MaskData[6, 3]);
        }

        [TestMethod]
        public void AreaInSpeed_ZocNullificationLevel_MustBeAtLeastZocLevel()
        {
            // ＺＯＣ無効化Lv が ＺＯＣLv 未満なら ＺＯＣ は有効、以上なら無効
            var weak = CreateZocField("ＺＯＣLv3", "ＺＯＣ無効化Lv2");
            var u = weak.Place("歩兵", "味方", 1, 2);
            weak.Place("守兵", "敵", 5, 2);
            weak.Map.AreaInSpeed(u);
            Assert.AreEqual(3, weak.Map.PointInZOC[5, 1]);
            Assert.IsTrue(weak.Map.MaskData[6, 1]);

            var strong = CreateZocField("ＺＯＣLv3", "ＺＯＣ無効化Lv3");
            var u2 = strong.Place("歩兵", "味方", 1, 2);
            strong.Place("守兵", "敵", 5, 2);
            strong.Map.AreaInSpeed(u2);
            Assert.AreEqual(0, strong.Map.PointInZOC[5, 1]);
            Assert.IsFalse(strong.Map.MaskData[6, 1]);
        }

        #endregion

        #region マップファイル

        [TestMethod]
        public void LoadMapData_ByPath_ReadsExistingFileAndFallsBackWhenMissing()
        {
            // ファイルがあれば読み込み、存在しなければマップ無し（ステータス表示用）になる
            // （VB: LoadMapData の If fname = "" Or Not FileExists(fname)）
            var dir = Path.Combine(Path.GetTempPath(), "srcs-mapcov-" + Guid.NewGuid());
            try
            {
                Directory.CreateDirectory(dir);
                var path = Path.Combine(dir, "stage.map");
                File.WriteAllText(path, MapCoverageFixture.BuildMapText("..M", "..."), new UTF8Encoding(false));

                var f = new MapCoverageFixture();
                f.Src.FileSystem = new LocalFileSystem();
                f.Map.LoadMapData(path);
                Assert.AreEqual(path, f.Map.MapFileName);
                Assert.AreEqual(3, f.Map.MapWidth);
                Assert.AreEqual(2, f.Map.MapHeight);
                Assert.AreEqual("山", f.Map.Terrain(3, 1).Name);

                f.Map.LoadMapData(Path.Combine(dir, "missing.map"));
                Assert.AreEqual("", f.Map.MapFileName);
                Assert.IsTrue(f.Map.IsStatusView);
                Assert.AreEqual(f.GUI.MainWidth, f.Map.MapWidth);
                Assert.AreEqual(f.GUI.MainHeight, f.Map.MapHeight);
            }
            finally
            {
                try { Directory.Delete(dir, true); } catch { }
            }
        }

        [TestMethod]
        public void ClearMap_ResetsMapFileName()
        {
            // VB: ClearMap の先頭で MapFileName = "" とする
            var f = Create("...");
            Assert.AreEqual("test.map", f.Map.MapFileName);

            f.Map.ClearMap();
            Assert.AreEqual("", f.Map.MapFileName);
            Assert.IsTrue(f.Map.IsStatusView);
        }

        #endregion
    }
}
