using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// Unit.lookup.cs の地形進入判定（IsAbleToEnter）と合体技のパートナー探索（CombinationPartner）のテスト。
    /// 元の SRC の Unit.cls の IsAbleToEnter / CombinationPartner に対応する。
    /// </summary>
    [TestClass]
    public class UnitLookupMapTests
    {
        // ===== IsAbleToEnter =====

        private static UnitLookupTestHelper CreateTerrainWorld()
        {
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefineUnit("陸戦機", trans: "陸", adaption: "-A-A");
            h.DefineUnit("飛行機", trans: "空陸", adaption: "AA--");
            h.DefineUnit("水陸機", trans: "陸", adaption: "-AAA");
            h.DefineUnit("潜水機", trans: "陸水", adaption: "-AAA");
            h.DefineUnit("船", trans: "水上", adaption: "-AA-");
            h.DefineUnit("宇宙機", trans: "陸宇", adaption: "-A-A");
            h.SetTerrain(1, 1, "空", "空");
            h.SetTerrain(2, 1, "海", "水");
            h.SetTerrain(3, 1, "深海", "深水");
            h.SetTerrain(4, 1, "宇宙", "宇宙");
            h.SetTerrain(5, 1, "月面", "月面");
            h.SetTerrain(6, 1, "壁", "陸", 1000);
            return h;
        }

        [TestMethod]
        public void IsAbleToEnter_WithoutMapFile_OnlyChecksAvailability()
        {
            // マップファイルが無い場合（必要技能のチェックのみ）は常に進入可能
            var h = CreateTerrainWorld();
            var u = h.Create("陸戦機", "アレン");
            h.Src.Map.MapFileName = "";

            Assert.IsTrue(u.IsAbleToEnter(1, 1));
            Assert.IsTrue(u.IsAbleToEnter(100, 100));
        }

        [TestMethod]
        public void IsAbleToEnter_OutsideMap_IsFalse()
        {
            var h = CreateTerrainWorld();
            var u = h.Create("陸戦機", "アレン");
            h.Src.Map.MapFileName = "test.map";

            Assert.IsFalse(u.IsAbleToEnter(0, 3));
            Assert.IsFalse(u.IsAbleToEnter(3, 0));
            Assert.IsFalse(u.IsAbleToEnter(8, 3));
            Assert.IsFalse(u.IsAbleToEnter(3, 8));
            Assert.IsTrue(u.IsAbleToEnter(3, 3));
        }

        [TestMethod]
        public void IsAbleToEnter_TerrainClassRules()
        {
            var h = CreateTerrainWorld();
            h.Src.Map.MapFileName = "test.map";
            var land = h.Create("陸戦機", "アレン");
            var air = h.Create("飛行機", null);
            var amphibious = h.Create("水陸機", null);
            var submarine = h.Create("潜水機", null);
            var ship = h.Create("船", null);
            var space = h.Create("宇宙機", null);

            // 空: 空を飛べるユニットのみ
            Assert.IsFalse(land.IsAbleToEnter(1, 1));
            Assert.IsTrue(air.IsAbleToEnter(1, 1));

            // 水: 飛行・水上移動、または水中適応があれば進入可
            Assert.IsFalse(land.IsAbleToEnter(2, 1));
            Assert.IsTrue(air.IsAbleToEnter(2, 1));
            Assert.IsTrue(amphibious.IsAbleToEnter(2, 1));
            Assert.IsTrue(ship.IsAbleToEnter(2, 1));

            // 深水: 水中適応だけでは駄目で、水移動（または飛行・水上）が必要
            Assert.IsFalse(amphibious.IsAbleToEnter(3, 1));
            Assert.IsTrue(submarine.IsAbleToEnter(3, 1));
            Assert.IsTrue(ship.IsAbleToEnter(3, 1));

            // 宇宙: 宇宙適応が必要
            Assert.IsFalse(air.IsAbleToEnter(4, 1));
            Assert.IsTrue(space.IsAbleToEnter(4, 1));

            // 月面: 飛行・宇宙移動があれば可、なければ通常通り（移動コストのみ判定）
            Assert.IsTrue(land.IsAbleToEnter(5, 1));

            // 陸地: 陸移動か飛行が必要
            Assert.IsTrue(land.IsAbleToEnter(3, 3));
            Assert.IsTrue(air.IsAbleToEnter(3, 3));
            Assert.IsFalse(ship.IsAbleToEnter(3, 3));

            // 移動コスト 1000 以上は進入不能（飛行ユニットは陸地判定前に進入可となる）
            Assert.IsFalse(land.IsAbleToEnter(6, 1));
            Assert.IsTrue(air.IsAbleToEnter(6, 1));
        }

        [TestMethod]
        public void IsAbleToEnter_UnavailableForm_IsFalse()
        {
            // 使用不能の形態はどの地形にも進入不能
            var h = CreateTerrainWorld();
            var u = h.Create("陸戦機", "アレン");
            h.Src.Expression.DefineGlobalVariable("Disable(陸戦機)");

            Assert.IsFalse(u.IsAbleToEnter(3, 3));
        }

        // ===== CombinationPartner =====

        private const string ComboWeapon = "ダブルアタック, 3000, 1, 1, +0, -, -, -, AAAA, +0, 合";

        private static UnitLookupTestHelper CreateComboWorld(string partnerFeatures = "合体技=ダブルアタック 主役機",
            string leaderFeature = "合体技=ダブルアタック 相棒機", string partnerWeapons = ComboWeapon)
        {
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク");
            h.DefinePilot("ゾルグ");
            h.DefineUnit("主役機", leaderFeature, ComboWeapon);
            h.DefineUnit("相棒機", partnerFeatures, partnerWeapons);
            return h;
        }

        [TestMethod]
        public void CombinationPartner_NoMatchingFeature_ReturnsEmpty()
        {
            var h = CreateComboWorld();
            var leader = h.Place("主役機", "アレン", 3, 3);
            h.Place("相棒機", "ベルク", 4, 3);
            h.Src.Map.MapFileName = "test.map";

            Assert.AreEqual(0, leader.CombinationPartner("トリプルアタック", 0, 0, 0, 1).Count);
        }

        [TestMethod]
        public void CombinationPartner_NotOnMap_OnlyNeedsPartnerInParty()
        {
            // 出撃していない場合はパートナーが仲間（出撃・待機）にいればよい
            var h = CreateComboWorld();
            var leader = h.Create("主役機", "アレン");
            var partner = h.Create("相棒機", "ベルク");
            leader.Status = "待機";
            partner.Status = "待機";

            // VB は ReDim partners(cnum) で要素数だけを返す（中身は Nothing）ため、仕様は件数のみ。
            // C# は実際のパートナーを格納して返すが、これは C# 独自の拡張
            var result = leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1);
            Assert.AreEqual(1, result.Count);
            CollectionAssert.AreEqual(new[] { partner }, result.ToArray());

            // パイロット名で指定されていても良い
            var h2 = CreateComboWorld(leaderFeature: "合体技=ダブルアタック ベルク");
            var leader2 = h2.Create("主役機", "アレン");
            var partner2 = h2.Create("相棒機", "ベルク");
            leader2.Status = "待機";
            partner2.Status = "待機";
            Assert.AreEqual(1, leader2.CombinationPartner("ダブルアタック", 0, 0, 0, 1).Count);

            // パートナーが離脱していれば使えない
            partner.Status = "離脱";
            Assert.AreEqual(0, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1).Count);
        }

        [TestMethod]
        public void CombinationPartner_OnMap_FindsAdjacentPartnerAndRecordsSelection()
        {
            var h = CreateComboWorld();
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            var partner = h.Place("相棒機", "ベルク", 3, 4);

            var result = leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1);

            CollectionAssert.AreEqual(new[] { partner }, result.ToArray());
            // 合体技メッセージ判定用に選択されたパートナーが記録される
            CollectionAssert.AreEqual(new[] { partner }, h.Src.Commands.SelectedPartners.ToArray());
        }

        [TestMethod]
        public void CombinationPartner_OnMap_DiagonalOnlyWithOption()
        {
            // ２体合体技は隣接のみ。「合体技斜め隣接可」オプションで斜めも可
            var h = CreateComboWorld();
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            var partner = h.Place("相棒機", "ベルク", 4, 4);

            Assert.AreEqual(0, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1).Count);

            h.Src.Expression.DefineGlobalVariable("Option(合体技斜め隣接可)");
            CollectionAssert.AreEqual(new[] { partner }, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1).ToArray());
        }

        [TestMethod]
        public void CombinationPartner_OnMap_UsesGivenBasePoint()
        {
            // tx, ty を指定するとその地点を基点にパートナーを探す
            var h = CreateComboWorld();
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 1, 1);
            var partner = h.Place("相棒機", "ベルク", 5, 5);

            Assert.AreEqual(0, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1).Count);
            CollectionAssert.AreEqual(new[] { partner }, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1, 5, 4).ToArray());
        }

        [TestMethod]
        public void CombinationPartner_OnMap_RejectsEnemyAndDisabledPartner()
        {
            var h = CreateComboWorld();
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            h.Place("相棒機", "ベルク", 4, 3, party: "敵");
            // 敵はパートナーにならない
            Assert.AreEqual(0, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1).Count);

            var h2 = CreateComboWorld();
            h2.Src.Map.MapFileName = "test.map";
            var leader2 = h2.Place("主役機", "アレン", 3, 3);
            var partner2 = h2.Place("相棒機", "ベルク", 4, 3);
            // 行動不能（恐怖）のパートナーは使えない
            partner2.AddCondition("恐怖", 2);
            Assert.AreEqual(0, leader2.CombinationPartner("ダブルアタック", 0, 0, 0, 1).Count);

            // 自分が混乱していれば合体技は使えない
            var h3 = CreateComboWorld();
            h3.Src.Map.MapFileName = "test.map";
            var leader3 = h3.Place("主役機", "アレン", 3, 3);
            h3.Place("相棒機", "ベルク", 4, 3);
            leader3.AddCondition("混乱", 2);
            Assert.AreEqual(0, leader3.CombinationPartner("ダブルアタック", 0, 0, 0, 1).Count);
        }

        [TestMethod]
        public void CombinationPartner_LevelRequiresMutualRelation()
        {
            // 合体技にレベルがある場合はパイロット間の信頼度が双方ともレベル以上必要
            var h = CreateComboWorld(leaderFeature: "合体技Lv2=ダブルアタック 相棒機");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            var partner = h.Place("相棒機", "ベルク", 4, 3);

            h.SetRelation(leader.MainPilot(), partner.MainPilot(), 2);
            Assert.AreEqual(0, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1).Count);

            h.SetRelation(partner.MainPilot(), leader.MainPilot(), 2);
            CollectionAssert.AreEqual(new[] { partner }, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1).ToArray());
        }

        [TestMethod]
        public void CombinationPartner_PartnerWeaponRequirements()
        {
            // パートナーが同名武器を持つ場合、その武器の必要気力・消費ＥＮを満たす必要がある
            var h = CreateComboWorld(partnerWeapons: "ダブルアタック, 3000, 1, 1, +0, -, 50, 120, AAAA, +0, 合");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            var partner = h.Place("相棒機", "ベルク", 4, 3);
            var pp = partner.MainPilot();

            pp.Morale = 110;
            Assert.AreEqual(0, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1).Count);

            pp.Morale = 120;
            CollectionAssert.AreEqual(new[] { partner }, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1).ToArray());

            partner.EN = 49;
            Assert.AreEqual(0, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1).Count);

            // フォーメーションのチェックのみの場合は気力・ＥＮを問わない
            CollectionAssert.AreEqual(new[] { partner }, leader.CombinationPartner("ダブルアタック", 0, 0, 0, 1, check_formation: true).ToArray());
        }

        [TestMethod]
        public void CombinationPartner_PartnerWithoutWeapon_UsesGivenRequirements()
        {
            // パートナーが同名の武器・アビリティを持たない場合は引数の気力・ＥＮ条件で判定
            var h = CreateComboWorld(partnerWeapons: "パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            var partner = h.Place("相棒機", "ベルク", 4, 3);
            partner.MainPilot().Morale = 100;
            partner.EN = 30;

            Assert.AreEqual(0, leader.CombinationPartner("ダブルアタック", 0, 110, 0, 1).Count);
            Assert.AreEqual(0, leader.CombinationPartner("ダブルアタック", 40, 0, 0, 1).Count);
            CollectionAssert.AreEqual(new[] { partner }, leader.CombinationPartner("ダブルアタック", 30, 100, 0, 1).ToArray());
        }

        [TestMethod]
        public void CombinationPartner_FourUnitCombo_AllowsDiagonal()
        {
            // 射程1で4体以上の合体技は斜め隣接も可。全員そろわなければ使えない
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク");
            h.DefinePilot("カイ");
            h.DefinePilot("ドナ");
            h.DefinePilot("エマ");
            h.DefineUnit("主役機", "合体技=カルテット 機体Ｂ 機体Ｃ 機体Ｄ 機体Ｅ");
            h.DefineUnit("機体Ｂ");
            h.DefineUnit("機体Ｃ");
            h.DefineUnit("機体Ｄ");
            h.DefineUnit("機体Ｅ");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            var b = h.Place("機体Ｂ", "ベルク", 2, 2);
            var c = h.Place("機体Ｃ", "カイ", 4, 4);
            var d = h.Place("機体Ｄ", "ドナ", 2, 4);

            Assert.AreEqual(0, leader.CombinationPartner("カルテット", 0, 0, 0, 1).Count);

            var e = h.Place("機体Ｅ", "エマ", 4, 2);
            CollectionAssert.AreEqual(new[] { b, c, d, e }, leader.CombinationPartner("カルテット", 0, 0, 0, 1).ToArray());
        }
    }
}
