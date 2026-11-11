using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Models;
using SRCCore.Pilots;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// アビリティ・スペシャルパワー・チャージのコマンドを、元の SRC（Command.bas の MapAbilityCommand・
    /// StartSpecialPowerCommand・ChargeCommand）の動作を期待値にして確かめるテスト。
    /// </summary>
    [TestClass]
    public class CommandUnitOpsVBMatchTests
    {
        private static AbilityData AddAbility(UnitData ud, string name, string aclass, int minRange, int maxRange)
        {
            var ad = ud.AddAbility(name);
            ad.Class = aclass;
            ad.MinRange = minRange;
            ad.MaxRange = maxRange;
            ad.ENConsumption = 10;
            ad.SetEffect("回復Lv1");
            return ad;
        }

        private static WeaponData AddWeapon(UnitData ud, string name, string wclass, int en)
        {
            var wd = ud.AddWeapon(name);
            wd.Power = 1000;
            wd.MinRange = 1;
            wd.MaxRange = 1;
            wd.Bullet = 0;
            wd.ENConsumption = en;
            wd.Adaption = "AAAA";
            wd.Class = wclass;
            return wd;
        }

        // ──────────────────────────────────────────────
        // MapAbilityCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapAbility_CombinationRange1_FindsPartnersWithoutTarget()
        {
            // VB の MapAbilityCommand は合体技のパートナーを射程に関わらず .CombinationPartner "アビリティ", a, partners で探す
            // （目標の SelectedTarget は使わない。マップアビリティでは SelectedTarget が決まっていない）
            var f = new CommandUnitOpsFixture();
            var leaderData = f.DefineUnit("主役機", 1000, 100, 5, "合体技=合体回復 相棒機");
            AddAbility(leaderData, "合体回復", "合Ｍ全", 1, 1);
            var partnerData = f.DefineUnit("相棒機");
            AddAbility(partnerData, "合体回復", "合Ｍ全", 1, 1);
            f.DefineUnit("戦闘機", hp: 2000);
            var leader = f.Place("主役機", "アレン", "味方", 5, 5);
            var partner = f.Place("相棒機", "ベルク", "味方", 4, 5);
            var damaged = f.Place("戦闘機", "損傷", "味方", 5, 6);
            damaged.HP = 1000;
            f.Commands.SelectedUnit = leader;
            f.Commands.SelectedTarget = null;
            f.Commands.CommandState = "コマンド選択";
            f.Gui.Handle("AbilityListBox", args => ((Unit)args[0]).Abilities.First());

            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "合体回復"));
            f.Commands.ProceedCommand(false, GuiButton.Left, f.Cell(5, 6), damaged);

            Assert.AreEqual(1500, damaged.HP);
            Assert.AreEqual(1, leader.UsedAction);
            Assert.AreEqual(1, partner.UsedAction, "合体技のパートナーも行動数を消費する");
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // StartSpecialPowerCommand（味方スペシャルパワー実行）
        // ──────────────────────────────────────────────

        private static (CommandUnitOpsFixture f, Pilot friend) CreateAllySpField(params (string name, string kana)[] powers)
        {
            var f = new CommandUnitOpsFixture();
            foreach (var (name, kana) in powers)
            {
                f.DefineSpecialPower(name, name.Substring(0, 1), 10, "自分", "気力増加Lv1", kana: kana);
            }
            f.DefineSpecialPower("友情", "友", 5, "自分", "味方スペシャルパワー実行", kana: "ゆうじょう");
            f.DefineUnit("機体");
            f.DefinePilot("友人", sp: 50).AddSpecialPower("友情", 1, 0);
            var unit = f.Place("機体", "友人", "味方", 5, 5);
            var friend = unit.MainPilot();
            friend.SP = 50;
            f.Commands.SelectedUnit = unit;
            f.Commands.CommandState = "コマンド選択";
            f.Gui.Handle("ListBox", args => 1);
            return (f, friend);
        }

        [TestMethod]
        public void AllySpecialPower_SortsByKanaLikeVBTextCompare()
        {
            // VB は読み仮名を StrComp(…, vbTextCompare) で比べる選択ソートで並べる。
            // ひらがな・カタカナ・全角・半角を区別しないので、読みが同じものは SPDList の順のまま残る
            var (f, _) = CreateAllySpField(("覚醒", "カクセイ"), ("覚醒改", "かくせい"), ("気合", "きあい"), ("加速", "ｶｿｸ"));
            ListBoxArgs spArgs = null;
            f.Gui.Handle("MultiColumnListBox", args => { spArgs = (ListBoxArgs)args[0]; return 0; });

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            CollectionAssert.AreEqual(new[] { "覚醒", "覚醒改", "加速", "気合" },
                spArgs.Items.Select(x => x.ListItemID).ToList());
        }

        [TestMethod]
        public void AllySpecialPower_OnlyFirstEffectIsChecked()
        {
            // VB は SPDList.Item(SelectedSpecialPower).EffectType(1) だけを見る。
            // ２番目の効果が味方スペシャルパワー実行でも、他のパイロットのスペシャルパワーの一覧は出さない
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("応援", "応", 5, "自分", "気力増加Lv1 味方スペシャルパワー実行");
            f.DefineUnit("機体");
            f.DefinePilot("友人", sp: 50).AddSpecialPower("応援", 1, 0);
            var unit = f.Place("機体", "友人", "味方", 5, 5);
            unit.MainPilot().SP = 50;
            f.Commands.SelectedUnit = unit;
            f.Commands.CommandState = "コマンド選択";
            f.Gui.Handle("ListBox", args => 1);
            f.Gui.Handle("MultiColumnListBox", args => 0);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.IsFalse(f.Called("MultiColumnListBox"));
            Assert.AreEqual(45, unit.MainPilot().SP, "消費ＳＰは２倍にならない");
        }

        [TestMethod]
        public void AllySpecialPower_ListIncludesPowerWhoseSecondEffectIsAllySpecialPower()
        {
            // VB は一覧から除くスペシャルパワーも EffectType(1) で判定する
            var (f, _) = CreateAllySpField(("気合", "きあい"));
            f.DefineSpecialPower("応援", "応", 5, "自分", "気力増加Lv1 味方スペシャルパワー実行", kana: "おうえん");
            ListBoxArgs spArgs = null;
            f.Gui.Handle("MultiColumnListBox", args => { spArgs = (ListBoxArgs)args[0]; return 0; });

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            CollectionAssert.AreEqual(new[] { "応援", "気合" }, spArgs.Items.Select(x => x.ListItemID).ToList());
        }

        // ──────────────────────────────────────────────
        // ChargeCommand
        // ──────────────────────────────────────────────

        private static (CommandUnitOpsFixture f, Unit leader, Unit weaponPartner, Unit abilityPartner) CreateChargeField(int weaponEN)
        {
            var f = new CommandUnitOpsFixture();
            var leaderData = f.DefineUnit("主役機", 1000, 100, 5, "合体技=チャージ斬り 剣機", "合体技=チャージ回復 盾機");
            AddWeapon(leaderData, "チャージ斬り", "Ｃ合", weaponEN);
            AddAbility(leaderData, "チャージ回復", "Ｃ合", 1, 1);
            AddWeapon(f.DefineUnit("剣機"), "チャージ斬り", "Ｃ合", weaponEN);
            AddAbility(f.DefineUnit("盾機"), "チャージ回復", "Ｃ合", 1, 1);
            var leader = f.Place("主役機", "アレン", "味方", 5, 5);
            var weaponPartner = f.Place("剣機", "ベルク", "味方", 4, 5);
            var abilityPartner = f.Place("盾機", "ゾルグ", "味方", 6, 5);
            f.Commands.SelectedUnit = leader;
            f.Commands.CommandState = "コマンド選択";
            f.Gui.Handle("Confirm", args => GuiDialogResult.Ok);
            return (f, leader, weaponPartner, abilityPartner);
        }

        [TestMethod]
        public void Charge_WeaponPartnersFound_DoesNotLookAtAbilities()
        {
            // VB: Ｃ・合の武器でパートナーが見つかれば（UBound(partners) > 0）アビリティは調べない
            var (f, leader, weaponPartner, abilityPartner) = CreateChargeField(10);

            f.Commands.UnitCommand(new UiCommand(Command.ChargeCmdID, "チャージ"));

            Assert.IsTrue(leader.IsConditionSatisfied("チャージ"));
            Assert.IsTrue(weaponPartner.IsConditionSatisfied("チャージ"));
            Assert.IsFalse(abilityPartner.IsConditionSatisfied("チャージ"));
        }

        [TestMethod]
        public void Charge_WeaponNotAvailable_UsesAbilityPartners()
        {
            // VB: .IsWeaponAvailable(i, "チャージ") で使えない武器は飛ばし、パートナーが無ければアビリティで探す
            var (f, leader, weaponPartner, abilityPartner) = CreateChargeField(500);

            f.Commands.UnitCommand(new UiCommand(Command.ChargeCmdID, "チャージ"));

            Assert.IsTrue(leader.IsConditionSatisfied("チャージ"));
            Assert.IsFalse(weaponPartner.IsConditionSatisfied("チャージ"));
            Assert.IsTrue(abilityPartner.IsConditionSatisfied("チャージ"));
        }
    }
}
