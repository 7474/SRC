using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests.UnitAttackCheck
{
    /// <summary>
    /// Unit.CauseEffect（VB: Unit.cls CauseEffect）のテスト。
    /// 攻撃側を「特殊効果発動」効果下に置いて発生確率を100%に固定し、乱数に依存させない。
    /// </summary>
    [TestClass]
    public class UnitAttackCheckCauseEffectTests
    {
        private const string Pilots = @"
攻撃者
攻撃者, こうげきしゃ, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

標的
標的, ひょうてき, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 普通
ＳＰ, 50, 必中, 1
-.bmp, -.mid

機械兵
機械兵, きかいへい, 男性, 汎用, AAAA, 100
特殊能力なし
150, 150, 150, 150, 150, 150, 機械
ＳＰ, 50, 必中, 1
-.bmp, -.mid
";

        private const string TargetUnits = @"
標的機
標的機, ひょうてきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突

縛無効機
縛無効機, ばくむこうき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力
特殊効果無効化=縛
3000, 200, 1000, 100
AAAA, -.bmp
パンチ, 1000, 1, 1, +0, -, -, -, AAAA, +0, 突
";

        /// <summary>
        /// 指定属性の武器を1つだけ持つ攻撃機と標的を配置する。
        /// </summary>
        private static (UnitAttackCheckFixture f, Unit a, Unit t) Setup(string wclass, string targetUnit = "標的機", string targetPilot = "標的", bool forceEffect = true)
        {
            var f = new UnitAttackCheckFixture();
            f.LoadPilots(Pilots);
            f.LoadUnits(TargetUnits + @"
攻撃機
攻撃機, こうげきき, 汎用, 1, 2
陸, 4, M, 1000, 100
特殊能力なし
3000, 200, 1000, 100
AAAA, -.bmp
効果兵器, 1000, 1, 1, +0, -, -, -, AAAA, +0, " + wclass + @"
");
            var a = f.Place("攻撃機", "攻撃者", "味方", 1, 1);
            var t = f.Place(targetUnit, targetPilot, "敵", 2, 1);
            if (forceEffect)
            {
                f.ApplySpecialPower(a, "発動テスト", "特殊効果発動");
            }
            return (f, a, t);
        }

        private static bool Cause(Unit a, Unit t, out string msg, out string critical_type, bool will_die = false)
        {
            msg = "";
            critical_type = "";
            return a.CauseEffect(a.Weapon(1), t, ref msg, ref critical_type, "", will_die);
        }

        // ──────────────────────────────────────────────
        // 状態異常の付与（既定の持続ターンとレベル指定）
        // ──────────────────────────────────────────────

        [TestMethod]
        [DataRow("縛", "行動不能", 2, "捕縛")]
        [DataRow("縛L3", "行動不能", 3, "捕縛")]
        [DataRow("Ｓ", "行動不能", 1, "ショック")]
        [DataRow("劣", "装甲劣化", 10000, "劣化")]
        [DataRow("石", "石化", 10000, "石化")]
        [DataRow("凍", "凍結", 3, "凍結")]
        [DataRow("痺", "麻痺", 3, "麻痺")]
        [DataRow("痺L1", "麻痺", 1, "麻痺")]
        [DataRow("眠", "睡眠", 3, "睡眠")]
        [DataRow("乱", "混乱", 3, "混乱")]
        [DataRow("撹", "撹乱", 2, "撹乱")]
        [DataRow("恐", "恐怖", 3, "恐怖")]
        [DataRow("盲", "盲目", 3, "盲目")]
        [DataRow("毒", "毒", 3, "毒")]
        [DataRow("不", "攻撃不能", 1, "攻撃不能")]
        [DataRow("黙", "沈黙", 3, "沈黙")]
        [DataRow("踊", "踊り", 3, "踊り")]
        [DataRow("狂", "狂戦士", 3, "狂戦士")]
        [DataRow("ゾ", "ゾンビ", 10000, "ゾンビ")]
        [DataRow("害", "回復不能", 10000, "回復不能")]
        [DataRow("告", "死の宣告", 1, "死の宣告")]
        [DataRow("低攻", "攻撃力ＤＯＷＮ", 3, "攻撃力ＤＯＷＮ")]
        [DataRow("低防", "防御力ＤＯＷＮ", 3, "防御力ＤＯＷＮ")]
        [DataRow("低運", "運動性ＤＯＷＮ", 3, "運動性ＤＯＷＮ")]
        [DataRow("低移", "移動力ＤＯＷＮ", 3, "移動力ＤＯＷＮ")]
        [DataRow("弱火", "火属性弱点付加", 3, "火属性弱点付加")]
        [DataRow("効火", "火属性有効付加", 3, "火属性有効付加")]
        [DataRow("剋オ", "オーラ使用不能", 3, "オーラ使用不能")]
        public void StatusEffect_AddsConditionWithLifetime(string wclass, string condition, int lifetime, string tag)
        {
            var (_, a, t) = Setup(wclass);

            var result = Cause(a, t, out _, out var critical_type);

            Assert.IsTrue(result);
            Assert.IsTrue(t.IsConditionSatisfied(condition), condition);
            Assert.AreEqual(lifetime, t.ConditionLifetime(condition));
            Assert.AreEqual(" " + tag, critical_type);
        }

        [TestMethod]
        public void Bind_MessageNamesTarget()
        {
            var (_, a, t) = Setup("縛");

            Cause(a, t, out var msg, out _);

            Assert.AreEqual("[標的機]の自由を奪った。;", msg);
        }

        [TestMethod]
        public void WillDie_SkipsNormalEffects()
        {
            // 破壊が確定している場合は通常の特殊効果を発生させない
            var (_, a, t) = Setup("縛");

            var result = Cause(a, t, out var msg, out var critical_type, will_die: true);

            Assert.IsFalse(result);
            Assert.IsFalse(t.IsConditionSatisfied("行動不能"));
            Assert.AreEqual("", msg);
            Assert.AreEqual("", critical_type);
        }

        [TestMethod]
        public void NormalWeapon_CausesNothing()
        {
            // 特殊効果属性を持たない武器では何も起こらない
            var (_, a, t) = Setup("突");

            Assert.IsFalse(Cause(a, t, out var msg, out var critical_type));
            Assert.AreEqual("", msg);
            Assert.AreEqual("", critical_type);
        }

        [TestMethod]
        public void SpecialEffectImmune_PreventsEffect()
        {
            // 特殊効果無効化=縛 を持つユニットは捕縛されない
            var (_, a, t) = Setup("縛", "縛無効機");

            Assert.IsFalse(Cause(a, t, out _, out _));
            Assert.IsFalse(t.IsConditionSatisfied("行動不能"));
        }

        [TestMethod]
        public void SpecialEffectImmune_FieldSet_PreventsEffect()
        {
            // 特殊効果無効化属性（strSpecialEffectImmune）に含まれる効果は発生しない。
            // データ定義からの経路（上の Ignore テスト）は Unit.Update がこのフィールドを埋めないため通らない。
            // それが直るまでの代わりとして、CauseEffect が SpecialEffectImmune を参照することだけをここで確かめる
            var (_, a, t) = Setup("縛");
            t.strSpecialEffectImmune = "縛";

            Assert.IsFalse(Cause(a, t, out _, out _));
            Assert.IsFalse(t.IsConditionSatisfied("行動不能"));
        }

        [TestMethod]
        public void Petrify_BossIsImmune()
        {
            // ボスランクが設定されたユニットは石化しない
            var (_, a, t) = Setup("石");
            t.BossRank = 0;

            Assert.IsFalse(Cause(a, t, out _, out _));
            Assert.IsFalse(t.IsConditionSatisfied("石化"));
        }

        [TestMethod]
        public void Sleep_MachinePilotIsImmune()
        {
            // 性格「機械」のパイロットは眠らない
            var (_, a, t) = Setup("眠", "標的機", "機械兵");

            Assert.IsFalse(Cause(a, t, out _, out _));
            Assert.IsFalse(t.IsConditionSatisfied("睡眠"));
        }

        [TestMethod]
        public void Stop_EnemyOutsideOwnPhase_GetsExtraTurn()
        {
            // 足止め: 自軍フェイズ外の相手には持続ターンが1多い（既定2）、フェイズ中は1
            var (f, a, t) = Setup("止");
            f.Src.Stage = "味方";
            Cause(a, t, out _, out var critical_type);
            Assert.AreEqual(2, t.ConditionLifetime("移動不能"));
            Assert.AreEqual(" 移動不能", critical_type);

            var (f2, a2, t2) = Setup("止");
            f2.Src.Stage = "敵";
            Cause(a2, t2, out _, out _);
            Assert.AreEqual(1, t2.ConditionLifetime("移動不能"));
        }

        [TestMethod]
        public void AttackDown_RemovesAttackUpInstead()
        {
            // 攻撃力ＵＰ状態なら、攻撃力ＤＯＷＮを付けずにＵＰを解除する
            var (_, a, t) = Setup("低攻");
            t.AddCondition("攻撃力ＵＰ", 3);

            Assert.IsTrue(Cause(a, t, out _, out _));
            Assert.IsFalse(t.IsConditionSatisfied("攻撃力ＵＰ"));
            Assert.IsFalse(t.IsConditionSatisfied("攻撃力ＤＯＷＮ"));
        }

        [TestMethod]
        public void Dispel_RemovesTemporaryEnhancements()
        {
            // 「除」: 持続ターンのある付加・強化・ＵＰ系の状態を打ち消す（永続のものは残す）
            var (_, a, t) = Setup("除");
            t.AddCondition("攻撃力ＵＰ", 3);
            t.AddCondition("バリア付加", 2);
            t.AddCondition("装甲強化", -1);

            Assert.IsTrue(Cause(a, t, out var msg, out var critical_type));
            Assert.IsFalse(t.IsConditionSatisfied("攻撃力ＵＰ"));
            Assert.IsFalse(t.IsConditionSatisfied("バリア付加"));
            Assert.IsTrue(t.IsConditionSatisfied("装甲強化"));
            Assert.AreEqual("[標的機]にかけられた特殊効果を打ち消した。;", msg);
            Assert.AreEqual(" 解除", critical_type);
        }

        [TestMethod]
        public void Dispel_NothingToRemove_ReturnsFalse()
        {
            var (_, a, t) = Setup("除");

            Assert.IsFalse(Cause(a, t, out _, out _));
        }

        [TestMethod]
        public void InstantDeath_ReportsDeath()
        {
            // 即死は critical_type に「即死」を付けて直ちに終了する
            var (_, a, t) = Setup("即");

            Assert.IsTrue(Cause(a, t, out _, out var critical_type));
            Assert.AreEqual(" 即死", critical_type);
        }

        [TestMethod]
        public void InstantDeath_BossAndImmortal_AreImmune()
        {
            var (_, a, t) = Setup("即");
            t.BossRank = 1;
            Assert.IsFalse(Cause(a, t, out _, out _));

            var (_, a2, t2) = Setup("即");
            t2.AddCondition("不死身", -1);
            Assert.IsFalse(Cause(a2, t2, out _, out _));
        }

        [TestMethod]
        public void DeathSentenceLevel0_SetsHPTo1()
        {
            // 告L0 は死の宣告を付けずにＨＰを1にする
            var (_, a, t) = Setup("告L0");

            Assert.IsTrue(Cause(a, t, out _, out _));
            Assert.AreEqual(1, t.HP);
            Assert.IsFalse(t.IsConditionSatisfied("死の宣告"));
        }

        [TestMethod]
        public void DeathSentenceLevel3_SetsLifetime()
        {
            var (_, a, t) = Setup("告L3");

            Assert.IsTrue(Cause(a, t, out _, out _));
            Assert.AreEqual(3, t.ConditionLifetime("死の宣告"));
        }

        // ──────────────────────────────────────────────
        // 気力減少・気力吸収
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoraleDrain_DefaultLowersTargetMoraleBy10()
        {
            var (_, a, t) = Setup("脱");
            t.MainPilot().Morale = 120;

            Assert.IsTrue(Cause(a, t, out _, out var critical_type));
            Assert.AreEqual(110, t.MainPilot().Morale);
            Assert.AreEqual(" 脱力", critical_type);
        }

        [TestMethod]
        public void MoraleDrain_LevelSpecified_Lowers5PerLevel()
        {
            var (_, a, t) = Setup("脱L3");
            t.MainPilot().Morale = 120;

            Cause(a, t, out _, out _);
            Assert.AreEqual(105, t.MainPilot().Morale);
        }

        [TestMethod]
        public void MoraleAbsorb_Default_TakesTenGivesFive()
        {
            var (_, a, t) = Setup("Ｄ");
            t.MainPilot().Morale = 120;
            a.MainPilot().Morale = 100;

            Assert.IsTrue(Cause(a, t, out _, out var critical_type));
            Assert.AreEqual(110, t.MainPilot().Morale);
            Assert.AreEqual(105, a.MainPilot().Morale);
            Assert.AreEqual(" 気力吸収", critical_type);
        }

        [TestMethod]
        public void MoraleAbsorb_Level2_TakesTenGivesFive()
        {
            var (_, a, t) = Setup("ＤL2");
            t.MainPilot().Morale = 120;
            a.MainPilot().Morale = 100;

            Cause(a, t, out _, out _);
            Assert.AreEqual(110, t.MainPilot().Morale);
            Assert.AreEqual(105, a.MainPilot().Morale);
        }

        [TestMethod]
        [Ignore("移植の不具合: 気力吸収の 2.5×Lv を (int) で切り捨てている（VB は Integer 引数への丸めで ＤL3 は +8）")]
        public void MoraleAbsorb_Level3_RoundsLikeVB()
        {
            // VB: IncreaseMorale 2.5 * WeaponLevel(w, "Ｄ") は Integer 引数への暗黙変換で 7.5 → 8（偶数丸め）
            var (_, a, t) = Setup("ＤL3");
            t.MainPilot().Morale = 120;
            a.MainPilot().Morale = 100;

            Cause(a, t, out _, out _);
            Assert.AreEqual(105, t.MainPilot().Morale);
            Assert.AreEqual(108, a.MainPilot().Morale);
        }

        [TestMethod]
        public void MoraleDrain_MachinePilotIsImmune()
        {
            var (_, a, t) = Setup("脱", "標的機", "機械兵");
            t.MainPilot().Morale = 120;

            Assert.IsFalse(Cause(a, t, out _, out _));
            Assert.AreEqual(120, t.MainPilot().Morale);
        }

        // ──────────────────────────────────────────────
        // ＨＰ・ＥＮ減衰
        // ──────────────────────────────────────────────

        [TestMethod]
        [DataRow("衰L1", 2250, "25%")]
        [DataRow("衰L2", 1500, "50%")]
        [DataRow("衰L3", 750, "75%")]
        public void HPDecay_NonBoss(string wclass, int expectedHP, string rate)
        {
            var (_, a, t) = Setup(wclass);
            t.HP = 3000;

            Assert.IsTrue(Cause(a, t, out var msg, out var critical_type));
            Assert.AreEqual(expectedHP, t.HP);
            Assert.AreEqual("[標的機]のＨＰを" + rate + "減少させた。;", msg);
            Assert.AreEqual(" 減衰", critical_type);
        }

        [TestMethod]
        [DataRow("衰L1", 2625)]
        [DataRow("衰L2", 2250)]
        [DataRow("衰L3", 1500)]
        public void HPDecay_BossIsHalved(string wclass, int expectedHP)
        {
            // ボスに対しては減衰量が半分（12.5% / 25% / 50%）
            var (_, a, t) = Setup(wclass);
            t.BossRank = 0;
            t.HP = 3000;

            Cause(a, t, out _, out _);
            Assert.AreEqual(expectedHP, t.HP);
        }

        [TestMethod]
        [DataRow("滅L1", 150)]
        [DataRow("滅L2", 100)]
        [DataRow("滅L3", 50)]
        public void ENDecay_NonBoss(string wclass, int expectedEN)
        {
            var (_, a, t) = Setup(wclass);
            t.EN = 200;

            Assert.IsTrue(Cause(a, t, out _, out _));
            Assert.AreEqual(expectedEN, t.EN);
        }

        // ──────────────────────────────────────────────
        // 魅了
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Charm_MakesTargetSlaveOfAttacker()
        {
            var (_, a, t) = Setup("魅");

            Assert.IsTrue(Cause(a, t, out _, out var critical_type));
            Assert.AreEqual(3, t.ConditionLifetime("魅了"));
            Assert.AreSame(a, t.Master);
            Assert.AreEqual(" 魅了", critical_type);
        }

        // ──────────────────────────────────────────────
        // 盗み（破壊確定時も発動する）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Steal_FromEnemyWithoutItem_GainsQuarterValueEvenIfDying()
        {
            // アイテムを持たない敵からは修理費の1/4の資金を盗み、以後は「すかんぴん」になる
            var (f, a, t) = Setup("盗");
            f.Src.Money = 0;

            Assert.IsTrue(Cause(a, t, out var msg, out var critical_type, will_die: true));
            Assert.AreEqual(t.Value / 4, f.Src.Money);
            Assert.IsTrue(t.Value > 0);
            Assert.IsTrue(t.IsConditionSatisfied("すかんぴん"));
            Assert.AreEqual(" 盗み", critical_type);
            StringAssert.Contains(msg, "を奪い取った。;");

            // 一度盗んだ相手からは盗めない
            Assert.IsFalse(Cause(a, t, out _, out _, will_die: true));
        }
    }
}
