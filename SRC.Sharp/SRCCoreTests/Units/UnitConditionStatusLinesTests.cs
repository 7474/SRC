using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests.SpecialPower;
using System.Linq;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.ConditionStatusLines のテスト。
    /// VB 版 Status.bas の「ユニットにかかっている特殊ステータス」と同じ行になることを確かめる。
    /// </summary>
    [TestClass]
    public class UnitConditionStatusLinesTests
    {
        private static Unit CreateUnit(string uclass = "")
        {
            var f = new SpecialPowerTestFixture();
            var u = f.AddUnit("テストユニット", "テストパイロット", "味方", 1, 1);
            u.Data.Class = uclass;
            return u;
        }

        [TestMethod]
        public void OrdinaryConditions_AreShown()
        {
            // 毒や麻痺のような普通の状態も表示する
            var u = CreateUnit();
            u.AddCondition("毒", 3, cdata: "");
            u.AddCondition("麻痺", 20, cdata: "");
            u.AddCondition("睡眠", -1, cdata: "");

            CollectionAssert.AreEqual(
                new[] { "毒 残り3ターン", "麻痺", "睡眠" },
                u.ConditionStatusLines().ToArray());
        }

        [TestMethod]
        public void ArmorDegradation_UsesTermAndShortLifetime()
        {
            var u = CreateUnit();
            u.AddCondition("装甲劣化", 2, Constants.DEFAULT_LEVEL, "装甲劣化");
            CollectionAssert.AreEqual(new[] { "装甲劣化 残り2ターン" }, u.ConditionStatusLines().ToArray());
        }

        [TestMethod]
        public void HiddenConditions_AreNotShown()
        {
            var u = CreateUnit();
            u.AddCondition("データ不明", 3, cdata: "");
            u.AddCondition("混乱", 3, cdata: "非表示");
            u.AddCondition("石化", 3, cdata: "解説 説明文");
            u.AddCondition("切り払い付加２", 3, 1, "切り払い");
            u.AddCondition("切り払い強化２", 3, 1, "切り払い");
            Assert.AreEqual(0, u.ConditionStatusLines().Count);
        }

        [TestMethod]
        public void Charging_NonHero_ShowsNameAndLifetime()
        {
            var u = CreateUnit();
            u.AddCondition("必殺技充填中", 2, cdata: "");
            CollectionAssert.AreEqual(new[] { "必殺技充填中 残り2ターン" }, u.ConditionStatusLines().ToArray());
        }

        [TestMethod]
        public void Charging_Hero_ShowsPreparing()
        {
            var u = CreateUnit("(人間)");
            u.AddCondition("必殺技充填中", 2, cdata: "");
            CollectionAssert.AreEqual(new[] { "必殺技準備中 残り2ターン" }, u.ConditionStatusLines().ToArray());
        }

        [TestMethod]
        public void WeaponEnhancement_ShowsLevelDataAndLifetime()
        {
            var u = CreateUnit();
            u.AddCondition("武器強化付加", 3, 2, "格闘");
            u.AddCondition("命中率強化付加", 100, 1, "");
            CollectionAssert.AreEqual(
                new[] { "武器強化Lv2付加(格闘) 残り3ターン", "命中率強化Lv1付加" },
                u.ConditionStatusLines().ToArray());
        }

        [TestMethod]
        public void ResistanceGrant_UsesShortTurnNotation()
        {
            // VB 版ではこの種類だけ「 3T」と表示する
            var u = CreateUnit();
            u.AddCondition("耐性付加", 3, Constants.DEFAULT_LEVEL, "ビ");
            CollectionAssert.AreEqual(new[] { "ビ耐性付加 3T" }, u.ConditionStatusLines().ToArray());
        }

        [TestMethod]
        public void ShieldAndTerrainGrant_AreShown()
        {
            var u = CreateUnit();
            u.AddCondition("盾付加", 5, 3, "シールド");
            u.AddCondition("地形適応変更付加", 5, Constants.DEFAULT_LEVEL, "空 A");
            CollectionAssert.AreEqual(
                new[] { "シールド付加(3) 残り5ターン", "地形適応変更付加 残り5ターン" },
                u.ConditionStatusLines().ToArray());
        }

        [TestMethod]
        public void AbilityGrantAndEnhancement_FollowVbNaming()
        {
            var u = CreateUnit();
            u.AddCondition("バリア付加", 3, 2, "Ｉフィールド");
            u.AddCondition("回避付加", 30, 2, "");
            u.AddCondition("分身付加", 30, Constants.DEFAULT_LEVEL, "");
            u.AddCondition("Ｓ防御強化", 30, 1, "Ｓ防御");
            CollectionAssert.AreEqual(
                new[] { "Ｉフィールド付加 残り3ターン", "回避Lv2付加", "分身付加", "Ｓ防御強化Lv1" },
                u.ConditionStatusLines().ToArray());
        }

        [TestMethod]
        public void AliasedGrant_IsShownOnce()
        {
            // エリアスで複数付加されても同じ表示は一度だけ
            var u = CreateUnit();
            u.AddCondition("バリア付加", 3, 2, "Ｉフィールド");
            u.AddCondition("ＥＮ消費減少付加", 3, 2, "Ｉフィールド");
            CollectionAssert.AreEqual(new[] { "Ｉフィールド付加 残り3ターン" }, u.ConditionStatusLines().ToArray());
        }

        [TestMethod]
        public void BarrierActivation_AlwaysShowsLifetime()
        {
            var u = CreateUnit();
            u.AddCondition("バリア発動", 1, Constants.DEFAULT_LEVEL, "");
            u.AddCondition("フィールド発動", 1, Constants.DEFAULT_LEVEL, "Ａフィールド");
            CollectionAssert.AreEqual(
                new[] { "バリア発動 残り1ターン", "Ａフィールド 残り1ターン" },
                u.ConditionStatusLines().ToArray());
        }
    }
}
