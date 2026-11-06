using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// 合体技のパートナーの検索（Unit.cls の CombinationPartner・IsCombinationAttackAvailable・IsCombinationAbilityAvailable）を
    /// 元の SRC の動作を期待値にして確かめるテスト。
    /// </summary>
    [TestClass]
    public class UnitCombinationVBMatchTests
    {
        // 名称, 攻撃力, 最小射程, 最大射程, 命中, 弾数, ＥＮ, 必要気力, 地形適応, ＣＴ, 属性
        private const string ComboWeapon = "ダブルアタック, 3000, 1, 1, +0, -, 50, -, AAAA, +0, 合";
        // 名称, 効果, 射程, 回数, ＥＮ, 必要気力, 属性
        private const string ComboAbility = "合体回復, 回復Lv1, 1, -, -, -, 合";

        [TestMethod]
        public void CombinationPartner_Weapon_IgnoresSameNamedAbilityOfPartner()
        {
            // VB: ctype が "武装" のときは同名の武器だけを探す。同名の武器が無ければ気力・ＥＮ・霊力だけを調べる。
            // パートナーの同名のアビリティ（回数が尽きている）は見ない
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク");
            h.DefineUnit("主役機", "合体技=ダブルアタック 相棒機", ComboWeapon);
            h.DefineUnit("相棒機", abilities: "ダブルアタック, 回復Lv1, 1, 1, -, -, -");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            var partner = h.Place("相棒機", "ベルク", 4, 3);
            partner.Ability(1).SetStock(0);

            var result = leader.Weapon(1).CombinationPartner();

            CollectionAssert.AreEqual(new[] { partner }, result.ToArray());
        }

        [TestMethod]
        public void CombinationPartner_Ability_IgnoresSameNamedWeaponOfPartner()
        {
            // VB: ctype が "アビリティ" のときは同名のアビリティだけを探す。パートナーの同名の武器（弾切れ）は見ない
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク");
            h.DefineUnit("主役機", "合体技=合体回復 相棒機", abilities: ComboAbility);
            h.DefineUnit("相棒機", weapons: "合体回復, 1000, 1, 1, +0, 1, -, -, AAAA, +0, 射");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            var partner = h.Place("相棒機", "ベルク", 4, 3);
            partner.Weapon(1).SetBullet(0);

            var result = leader.Ability(1).CombinationPartner();

            CollectionAssert.AreEqual(new[] { partner }, result.ToArray());
        }

        [TestMethod]
        public void IsCombinationAbilityAvailable_Range1_LooksAroundAdjacentAlly()
        {
            // VB: 射程１の合体技アビリティは、隣の味方ユニット（IsAlly）の周りにパートナーがいれば使える
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク");
            h.DefinePilot("ゾルグ");
            h.DefineUnit("主役機", "合体技=合体回復 相棒機", abilities: ComboAbility);
            h.DefineUnit("相棒機", abilities: ComboAbility);
            h.DefineUnit("僚機");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            h.Place("僚機", "ゾルグ", 4, 3);
            var partner = h.Place("相棒機", "ベルク", 4, 4);

            Assert.IsTrue(leader.Ability(1).IsCombinationAbilityAvailable());
            CollectionAssert.AreEqual(new[] { partner }, h.Src.Commands.SelectedPartners.ToArray());
        }

        [TestMethod]
        public void IsCombinationAbilityAvailable_Range1_EnemyNeighborIsNotBase()
        {
            // VB: 射程１の合体技アビリティは敵ユニットを基点にしない
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク");
            h.DefinePilot("ゾルグ");
            h.DefineUnit("主役機", "合体技=合体回復 相棒機", abilities: ComboAbility);
            h.DefineUnit("相棒機", abilities: ComboAbility);
            h.DefineUnit("敵機");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            h.Place("敵機", "ゾルグ", 4, 3, party: "敵");
            h.Place("相棒機", "ベルク", 4, 4);

            Assert.IsFalse(leader.Ability(1).IsCombinationAbilityAvailable());
        }

        [TestMethod]
        public void IsCombinationAttackAvailable_Range1_PassesCheckFormation()
        {
            // VB: 射程１の合体技で隣の敵の周りを調べるときも check_formation を渡す（ＥＮなどは調べない）
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク");
            h.DefinePilot("ゾルグ");
            h.DefineUnit("主役機", "合体技=ダブルアタック 相棒機", ComboWeapon);
            h.DefineUnit("相棒機", weapons: ComboWeapon);
            h.DefineUnit("敵機");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 3, 3);
            h.Place("敵機", "ゾルグ", 4, 3, party: "敵");
            var partner = h.Place("相棒機", "ベルク", 4, 4);
            Assert.IsTrue(leader.Weapon(1).IsCombinationAttackAvailable(false));
            // ＥＮ 0 は行動できなくなるので、合体技のＥＮ（50）に足りない 10 にする
            partner.EN = 10;

            Assert.IsTrue(leader.Weapon(1).IsCombinationAttackAvailable(true));
            Assert.IsFalse(leader.Weapon(1).IsCombinationAttackAvailable(false));
        }

        [TestMethod]
        public void IsCombinationAttackAvailable_Range1_ChecksLeftRightUpDown()
        {
            // VB: 隣の敵は左・右・上・下の順に調べ、最初に見つかった組を SelectedPartners に残す
            var h = new UnitLookupTestHelper();
            h.DefinePilot("アレン");
            h.DefinePilot("ベルク");
            h.DefinePilot("ゾルグ");
            h.DefinePilot("ダリル");
            h.DefinePilot("カイ");
            h.DefineUnit("主役機", "合体技=ダブルアタック 相棒機", ComboWeapon);
            h.DefineUnit("相棒機", weapons: ComboWeapon);
            h.DefineUnit("敵機");
            h.Src.Map.MapFileName = "test.map";
            var leader = h.Place("主役機", "アレン", 4, 4);
            // 右の敵 (5,4) の周りにパートナー (6,4)、左の敵 (3,4) の周りにパートナー (2,4)
            h.Place("敵機", "ゾルグ", 5, 4, party: "敵");
            h.Place("敵機", "ダリル", 3, 4, party: "敵");
            h.Place("相棒機", "ベルク", 6, 4);
            var left = h.Place("相棒機", "カイ", 2, 4);

            Assert.IsTrue(leader.Weapon(1).IsCombinationAttackAvailable());
            CollectionAssert.AreEqual(new[] { left }, h.Src.Commands.SelectedPartners.ToArray());
        }
    }
}
