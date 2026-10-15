using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.HelpCoverage
{
    /// <summary>
    /// Help.SkillHelpMessage（VB: Help.bas SkillHelpMessage）のテスト。
    /// パイロット用特殊能力の解説文をレベル・データから組み立てる規則を確認する。
    /// </summary>
    [TestClass]
    public class HelpSkillHelpMessageTests
    {
        // 技能を1つ持つパイロットを搭乗させ、その技能の解説を得る
        private static string SkillMessage(string sname, double slevel, string sdata = "")
        {
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill(sname, slevel, sdata, 1);
            var u = fx.Build();
            return fx.Src.Help.SkillHelpMessage(u.MainPilot(), "1");
        }

        [TestMethod]
        public void SuperSense_BonusAndRangeExtension()
        {
            // 命中・回避に 2×Lv+3、Lv>3 なら思念誘導攻撃の射程を Lv\4 延長
            Assert.AreEqual("命中・回避に +11 の修正を与える。;思念誘導攻撃(サ)の射程を1だけ延長する。", SkillMessage("超感覚", 4));
            Assert.AreEqual("命中・回避に +7 の修正を与える。", SkillMessage("超感覚", 2));
        }

        [TestMethod]
        public void Parry_ProbabilityIsLevelOver16()
        {
            Assert.AreEqual("格闘武器(武)、突進技(突)、実弾攻撃(実)による攻撃を 50% の確率で切り払って回避する。", SkillMessage("切り払い", 8));
        }

        [TestMethod]
        public void ShieldDefense_WithoutTate_Probability()
        {
            // 盾を持たないユニットではシールド防御の確率
            Assert.AreEqual("25% の確率でシールド防御を行う。", SkillMessage("Ｓ防御", 4));
        }

        [TestMethod]
        public void ShieldDefense_WithTate_Reduction()
        {
            // 盾を持つユニットでは 100×Lv+400 のダメージ減少
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill("Ｓ防御", 4, "", 1);
            fx.UnitData.AddFeature("盾Lv2");
            var u = fx.Build();
            Assert.AreEqual("シールド防御を行い、ダメージを800減少させる。", fx.Src.Help.SkillHelpMessage(u.MainPilot(), "1"));
        }

        [TestMethod]
        public void Guts_FixedText()
        {
            Assert.AreEqual("ＨＰが最大ＨＰの1/4以下の時に発動。;命中＆回避 +30%、クリティカル発生率 +50%。", SkillMessage("底力", 1));
        }

        [TestMethod]
        public void Support_CountFromLevel()
        {
            Assert.AreEqual("隣接するユニットにサポートアタックとサポートガードを１ターンにそれぞれ2回行う。", SkillMessage("援護", 2));
        }

        [TestMethod]
        public void Command_RangeAndBonus()
        {
            // 指揮: 命中・回避に 5×Lv
            Assert.AreEqual("半径２マス以内にいる味方ザコ・汎用及び階級所有パイロットの命中・回避に +10。", SkillMessage("指揮", 2));
        }

        [TestMethod]
        public void MoraleLimit_UsesLevel()
        {
            Assert.AreEqual("気力の上限が170になる。", SkillMessage("気力上限", 170));
        }

        [TestMethod]
        public void Hunter_TargetsAndPercent()
        {
            // VB と同じく先頭にも「、」が付き、2つ目以降は「や」でつなぐ
            Assert.AreEqual("ターゲットが、人間や機械である場合、ターゲットに与えるダメージが10%増加する。", SkillMessage("ハンター", 1, "ハンター 人間 機械"));
            Assert.AreEqual("ターゲットが、人間である場合、ターゲットに与えるダメージが20%減少する。", SkillMessage("ハンター", -2, "ハンター 人間"));
        }

        [TestMethod]
        public void SpCostReduction_ListsSpecialPowers()
        {
            Assert.AreEqual("スペシャルパワー「集中」「熱血」のＳＰ消費量が10%減少する。", SkillMessage("ＳＰ消費減少", 1, "ＳＰ消費減少 集中 熱血"));
        }

        [TestMethod]
        public void ReAttack_ProbabilityAndReactionNote()
        {
            // 100×Lv\16 = 18%
            Assert.AreEqual("自分の攻撃の直後に 18% の確率で再攻撃を行う。ただしパイロットの反応が相手を下回る場合、確率は半減。", SkillMessage("再攻撃", 3));
        }

        [TestMethod]
        public void FightingInstinct_StartMorale()
        {
            // 出撃時の気力 = 100 + 5×Lv
            Assert.AreEqual("出撃時の気力が110に増加する。", SkillMessage("闘争本能", 2));
        }

        [TestMethod]
        public void Endurance_PositiveAndNegative()
        {
            Assert.AreEqual("ダメージ計算の際に装甲を10%増加させる。", SkillMessage("耐久", 2));
            Assert.AreEqual("ダメージ計算の際に装甲を15%減少させる。", SkillMessage("耐久", -3));
        }

        [TestMethod]
        public void Repair_PercentFromLevel()
        {
            Assert.AreEqual("修理装置や回復アビリティを使った際のＨＰ回復量が 10% 増加する。", SkillMessage("修理", 1));
        }

        [TestMethod]
        public void Favorite_UsesSkillData()
        {
            Assert.AreEqual("「剣」属性を持つ武器・アビリティによるダメージ・効果量が 20% 増加。また、アビリティの継続時間が 40% 増加。", SkillMessage("得意技", 1, "剣"));
        }

        [TestMethod]
        public void Concentration_UsesMaxSp()
        {
            // 最大ＳＰ 50 → 20% は 10、10% は 5
            Assert.AreEqual("ＳＰが最大ＳＰの20%未満(10未満)の場合、ターン開始時にＳＰが最大ＳＰの10%分回復する(+5)。", SkillMessage("精神統一", 1));
        }

        [TestMethod]
        public void SkillByName_WithLevelSuffix_IsResolved()
        {
            // 付加された技能は「名称Lvn」形式でも引ける
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill("再攻撃", 3, "", 1);
            var u = fx.Build();
            Assert.AreEqual(
                "自分の攻撃の直後に 18% の確率で再攻撃を行う。ただしパイロットの反応が相手を下回る場合、確率は半減。",
                fx.Src.Help.SkillHelpMessage(u.MainPilot(), "再攻撃Lv3"));
        }

        [TestMethod]
        [Ignore("移植の不具合: Pilot.SkillName0(SkillData) が別名指定ありの技能で別名ではなく空文字を返す")]
        public void Explanation_DefinedByAliasSkill_OverridesText()
        {
            // 別名と同名の「解説」技能があれば、その解説文で置き換える
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill("再攻撃", 3, "連撃", 1);
            fx.PilotData.AddSkill("連撃", 1, "解説 \"素早く 二度斬る\"", 1);
            var u = fx.Build();
            Assert.AreEqual("素早く 二度斬る", fx.Src.Help.SkillHelpMessage(u.MainPilot(), "1"));
        }

        [TestMethod]
        public void LifeSizeOption_ReplacesPilotWord()
        {
            // 等身大基準では「パイロット」を「ユニット」に置き換える
            var fx = new HelpTestFixture();
            fx.PilotData.AddSkill("再攻撃", 3, "", 1);
            var u = fx.Build();
            fx.Src.Expression.DefineGlobalVariable("Option(等身大基準)");
            Assert.AreEqual(
                "自分の攻撃の直後に 18% の確率で再攻撃を行う。ただしユニットの反応が相手を下回る場合、確率は半減。",
                fx.Src.Help.SkillHelpMessage(u.MainPilot(), "1"));
        }
    }
}
