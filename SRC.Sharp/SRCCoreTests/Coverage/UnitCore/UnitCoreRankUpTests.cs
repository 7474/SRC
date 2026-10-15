using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.UnitCore
{
    /// <summary>
    /// Unit.RankUp と最大ＨＰ/ＥＮのランク補正（VB: Unit.cls Update の MaxHP = HP + 200 * Rank 等）のテスト
    /// </summary>
    [TestClass]
    public class UnitCoreRankUpTests
    {
        private static string UnitBlock(string name, int hp, int en, params string[] features)
        {
            var feat = features.Length == 0 ? "特殊能力なし" : "特殊能力\n" + string.Join("\n", features);
            return $@"
{name}
{name}, てすと, 汎用, 1, 2
陸, 5, M, 1000, 100
{feat}
{hp}, {en}, 800, 90
AAAA, test.bmp
テストソード, 1500, 1, 1, +15, -, -, -, AAAA, +10, 武
";
        }

        [TestMethod]
        public void RankUp_IncreasesMaxHpBy200AndMaxEnBy10PerRank()
        {
            // 改造１段階ごとに最大ＨＰ +200、最大ＥＮ +10。他形態も同じランクの値になる
            var src = UnitCoreFixture.CreateSrc();
            UnitCoreFixture.LoadUnits(src,
                UnitBlock("人型", 3000, 100, "変形=変形 飛行型")
                + UnitBlock("飛行型", 2000, 200, "変形=変形 人型"));
            var u = src.UList.Add("人型", 0, "味方");
            Assert.AreEqual(3000, u.MaxHP);
            Assert.AreEqual(100, u.MaxEN);

            u.RankUp(2, 0.5, 1d);

            Assert.AreEqual(3400, u.MaxHP);
            Assert.AreEqual(120, u.MaxEN);
            Assert.AreEqual(1700, u.HP);
            Assert.AreEqual(120, u.EN);
            var fly = u.OtherForm("飛行型");
            Assert.AreEqual(2400, fly.MaxHP);
            Assert.AreEqual(220, fly.MaxEN);
            Assert.AreEqual(1200, fly.HP);
            Assert.AreEqual(220, fly.EN);
        }
    }
}
