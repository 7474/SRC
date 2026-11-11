using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// アビリティの使用回数・消費ＥＮ・使用時の消費で、実数から整数への変換が
    /// 元の SRC（Unit.cls）と同じ丸めになることのテスト。
    /// VB の Integer・Long への代入は偶数への丸め、\ は被除数を丸めてから整数除算する。
    /// </summary>
    [TestClass]
    public class UnitAbilityVBRoundingTests
    {
        private UnitAbilityCoverageFixture fx;

        [TestInitialize]
        public void Setup()
        {
            fx = new UnitAbilityCoverageFixture();
        }

        private (Unit user, UnitAbility ability) Create(string effect, string aclass = "", int stock = 0, int en = 0, int hp = 1000)
        {
            var ud = fx.AddUnitData("使用機", hp: hp);
            var ad = ud.AddAbility("テストアビリティ");
            ad.SetEffect(effect);
            ad.Class = aclass;
            ad.MinRange = 1;
            ad.MaxRange = 1;
            ad.Stock = stock;
            ad.ENConsumption = en;
            var user = fx.CreateUnit("使用機", "使用者");
            return (user, user.Ability(1));
        }

        [TestMethod]
        public void MaxStock_BossRank_RoundsToEven()
        {
            // VB: MaxStock = Ability(a).Stock * (5 + BossRank) / 5（Integer の戻り値なので丸め）
            // 3 * 6 / 5 = 3.6 → 4（切り捨てなら 3）
            var (user, a) = Create("回復Lv1", stock: 3);
            user.BossRank = 1;
            Assert.AreEqual(4, a.MaxStock());
        }

        [TestMethod]
        public void Stock_RatioTimesMax_RoundsInsteadOfTruncating()
        {
            // VB: Stock = dblStock(a) * MaxStock(a)（Integer の戻り値なので丸め）
            // dblStock = 2 / 3 は 2 / 3 * 3 = 1.999... になりうるが、丸めで 2 に戻る
            var (_, a) = Create("回復Lv1", stock: 3);
            a.SetStockFull();
            a.SetStock(2);
            Assert.AreEqual(2, a.Stock());
            a.SetStock(1);
            Assert.AreEqual(1, a.Stock());
        }

        [TestMethod]
        public void ENConsumption_SpellSkill_RoundsToEven()
        {
            // VB: AbilityENConsumption = 0.9 * AbilityENConsumption（Integer の戻り値なので丸め）
            // 術Lv2 で 15 * 0.9 = 13.5 → 14（偶数への丸め。切り捨てなら 13）
            fx.AddPilotData("使用者").AddSkill("術", 2d, "", 1);
            var (_, a) = Create("回復Lv1", aclass: "術", en: 15);
            Assert.AreEqual(14, a.AbilityENConsumption());
        }

        [TestMethod]
        public void ENConsumption_SpellSkill_HalfRoundsToEvenDown()
        {
            // 術Lv6 で 25 * 0.5 = 12.5 → 12（偶数への丸め）
            fx.AddPilotData("使用者").AddSkill("術", 6d, "", 1);
            var (_, a) = Create("回復Lv1", aclass: "術", en: 25);
            Assert.AreEqual(12, a.AbilityENConsumption());
        }

        [TestMethod]
        public void Use_NormalAbility_SimultaneousStockUsesIntegerQuotient()
        {
            // VB: SetStock i, MinLng(CInt(MaxStock(i) * Stock(a) \ MaxStock(a) + 0.49999), Stock(i))
            // \ が + より先なので 5 * 1 \ 3 = 1 に 0.49999 を足して CInt → 1
            // （残りの割合 1/3 に 5 を掛けて 0.49999 を足し切り捨てると 2 になっていた）
            var ud = fx.AddUnitData("使用機");
            var ad1 = ud.AddAbility("通常");
            ad1.SetEffect("補給Lv1");
            ad1.Stock = 3;
            var ad2 = ud.AddAbility("一斉");
            ad2.SetEffect("回復Lv1");
            ad2.Class = "斉";
            ad2.Stock = 5;
            var u = fx.CreateUnit("使用機", "使用者");
            u.Ability(1).SetStock(2);

            u.Ability(1).UseAbility();

            Assert.AreEqual(1, u.Ability(1).Stock());
            Assert.AreEqual(1, u.Ability(2).Stock());
        }

        [TestMethod]
        public void Use_HPConsumption_RoundsDividendBeforeIntegerDivision()
        {
            // VB: HP = MaxLng(HP - MaxHP * AbilityLevel(a, "失") \ 10, 0)
            // 390 * 0.05 = 19.5 → 20 \ 10 = 2（切り捨ててから割ると 1）
            var (user, a) = Create("回復Lv1", aclass: "失L0.05", hp: 390);
            Assert.AreEqual(390, user.HP);
            a.UseAbility();
            Assert.AreEqual(388, user.HP);
        }

        [TestMethod]
        public void Execute_WeakTechnique_AttachDurationRoundsToEven()
        {
            // VB: .AddCondition ftype & "付加", elevel2, ...（Integer の引数なので丸め）
            // 不得手で 6 × 0.6 = 3.6 → 4（切り捨てなら 3）
            fx.AddPilotData("使用者").AddSkill("不得手", 1d, "魔", 1);
            var (user, a) = Create("付加Lv6=耐性Lv1=火", aclass: "魔");
            fx.AddUnitData("対象機");
            var target = fx.CreateUnit("対象機", "対象者");

            user.ExecuteAbility(a, target);

            Assert.AreEqual(4, target.ConditionLifetime("耐性付加"));
        }

        [TestMethod]
        public void Use_MoraleConsumption_RoundsToEven()
        {
            // VB: IncreaseMorale -5 * AbilityLevel(a, "気")（Integer の引数なので丸め）
            // -5 * 0.7 = -3.5 → -4（切り捨てなら -3）
            var (user, a) = Create("回復Lv1", aclass: "気L0.7");
            var p = user.MainPilot();
            p.Morale = 120;
            a.UseAbility();
            Assert.AreEqual(116, p.Morale);
        }
    }
}
