using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;

namespace SRCCore.Coverage.Tests.HelpCoverage
{
    /// <summary>
    /// Help.FeatureHelpMessage（VB: Help.bas FeatureHelpMessage）のテスト。
    /// ユニット用特殊能力の解説文を、レベル・データ・パイロット技能から組み立てる規則を確認する。
    /// </summary>
    [TestClass]
    public class HelpFeatureHelpMessageTests
    {
        // 特殊能力を1つ持つユニットを作り、その能力の解説を得る
        private static string FeatureMessage(string fdef, double sdefLevel = 0)
        {
            var fx = new HelpTestFixture();
            if (sdefLevel > 0)
            {
                fx.PilotData.AddSkill("Ｓ防御", sdefLevel, "", 1);
            }
            fx.UnitData.AddFeature(fdef);
            var u = fx.Build();
            return fx.Src.Help.FeatureHelpMessage(u, 1, false);
        }

        // ──────────────────────────────────────────────
        // 防御系能力
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Barrier_LevelAndEnAndMorale_AreDescribed()
        {
            // バリア: 1000×Lv 以下のダメージを無効化、第3引数が消費ＥＮ、第4引数(>50)が必要気力
            Assert.AreEqual(
                "ダメージ2000以下の攻撃を無効化。;発動時に20ＥＮ消費。気力110以上で使用可能。",
                FeatureMessage("バリアLv2=バリア 全 20 110"));
        }

        [TestMethod]
        public void Barrier_NegatedAttributeAndNegativeEn_AreDescribed()
        {
            // 「!属性」は「属性を持たない」、負の消費ＥＮは「増加」になる
            Assert.AreEqual(
                "「魔」属性を持たないダメージ1000以下の攻撃を無効化。;発動時に10ＥＮ増加。",
                FeatureMessage("バリアLv1=バリア !魔 -10"));
        }

        [TestMethod]
        public void Barrier_NonNumericEn_DefaultsToTen()
        {
            // 消費ＥＮの指定がない場合は 10ＥＮ 消費
            Assert.AreEqual(
                "「ビーム」属性を持つダメージ3000以下の攻撃を無効化。;発動時に10ＥＮ消費。",
                FeatureMessage("バリアLv3=バリア ビーム"));
        }

        [TestMethod]
        public void Barrier_Options_AreAppended()
        {
            // 第5引数以降のオプション（相殺・近接無効・手動）の説明が付く
            Assert.AreEqual(
                "ダメージ1000以下の攻撃を無効化。;発動時に10ＥＮ消費。"
                + ";バリアを持つユニット同士の場合、隣接時に効果は相殺。"
                + ";「武」「突」「接」による攻撃には無効。"
                + ";防御選択時にのみ発動。",
                FeatureMessage("バリアLv1=バリア 全 10 0 相殺 近接無効 手動"));
        }

        [TestMethod]
        public void Barrier_WithoutLevel_TreatsLevelAsOne()
        {
            // VB の AllFeatureLevel はレベル指定なしなら 1 を返す
            Assert.AreEqual(
                "「魔」属性を持たないダメージ1000以下の攻撃を無効化。;発動時に10ＥＮ増加。",
                FeatureMessage("バリア=バリア !魔 -10"));
        }

        [TestMethod]
        public void Field_ReducesDamageBy500PerLevel()
        {
            Assert.AreEqual("攻撃のダメージを1000減少させる。", FeatureMessage("フィールドLv2=フィールド"));
        }

        [TestMethod]
        public void Field_SameDataLevelsAccumulate()
        {
            // フィールド・アーマー等はデータが同じ能力のレベルを累積する
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("フィールドLv1=フィールド");
            fx.UnitData.AddFeature("フィールドLv2=フィールド");
            var u = fx.Build();
            Assert.AreEqual("攻撃のダメージを1500減少させる。", fx.Src.Help.FeatureHelpMessage(u, 1, false));
        }

        [TestMethod]
        public void Armor_IncreasesArmorBy100PerLevel()
        {
            Assert.AreEqual("攻撃に対して装甲を300増加させる。", FeatureMessage("アーマーLv3"));
        }

        [TestMethod]
        public void Resist_AttributeAndPercent()
        {
            Assert.AreEqual("「火」属性を持つ攻撃に対してダメージを50%軽減させる。", FeatureMessage("レジストLv5=レジスト 火"));
        }

        [TestMethod]
        public void SuperEvasion_EnMoraleAndManual()
        {
            Assert.AreEqual(
                "あらゆる攻撃を20%の確率で回避。;発動時に10ＥＮ消費。;気力120以上で使用可能。;回避選択時にのみ発動。",
                FeatureMessage("超回避Lv2=超回避 10 120 手動"));
        }

        [TestMethod]
        public void AttackEvasion_ReducesHitRate()
        {
            Assert.AreEqual("攻撃の命中率を本来の80%に減少させる。", FeatureMessage("攻撃回避Lv2=攻撃回避 全"));
        }

        [TestMethod]
        public void Resistance_ReducesEffectChance()
        {
            Assert.AreEqual("武器の特殊効果を受ける確率を20%減少させる。", FeatureMessage("抵抗力Lv2"));
        }

        // ──────────────────────────────────────────────
        // シールド系（パイロットのＳ防御レベルで確率が決まる）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Shield_ProbabilityFromSkillLevel()
        {
            // Ｓ防御Lv4 → 4×100\16 = 25%
            Assert.AreEqual("Ｓ防御Lv/16の確率(25%)で防御を行い、ダメージを半減。", FeatureMessage("シールド", 4));
        }

        [TestMethod]
        public void LargeShield_AddsOneLevel()
        {
            // (4+1)×100\16 = 31%
            Assert.AreEqual("(Ｓ防御Lv+1)/16の確率(31%)で防御を行い、ダメージを半減。", FeatureMessage("大型シールド", 4));
        }

        [TestMethod]
        public void LargeShield_WithoutSkill_ProbabilityZero()
        {
            // Ｓ防御を持たない場合は確率 0%
            Assert.AreEqual("(Ｓ防御Lv+1)/16の確率(0%)で防御を行い、ダメージを半減。", FeatureMessage("大型シールド"));
        }

        [TestMethod]
        public void SmallShield_ReducesToTwoThirds()
        {
            Assert.AreEqual("Ｓ防御Lv/16の確率(25%)で防御を行い、ダメージを2/3に減少。", FeatureMessage("小型シールド", 4));
        }

        [TestMethod]
        public void EnergyShield_WithLevel_ReducesFurther()
        {
            Assert.AreEqual(
                "Ｓ防御Lv/16の確率(25%)で防御を行い、ダメージを半減した上で更に200減少。発動時に5ＥＮ消費。「無」属性を持つ武器には無効。",
                FeatureMessage("エネルギーシールドLv2", 4));
        }

        [TestMethod]
        public void ActiveShield_AddsTwoLevels()
        {
            // (4+2)×100\16 = 37%
            Assert.AreEqual("(Ｓ防御Lv+2)/16の確率(37%)で防御を行い、ダメージを半減。", FeatureMessage("アクティブシールド", 4));
        }

        [TestMethod]
        public void Tate_ReductionFromSkillLevel()
        {
            // 盾: 減少量は 100×Ｓ防御Lv+400
            Assert.AreEqual(
                "2回、攻撃によって貫通されるまでシールド防御を行い、ダメージを減少させる(-800)。;"
                + "ただし攻撃側が「破」属性を持っていた場合、一度に２回分破壊される。;"
                + "ダメージの減少量はパイロットのＳ防御レベルによって決まる。",
                FeatureMessage("盾Lv2", 4));
        }

        // ──────────────────────────────────────────────
        // 回復・消費・強化系
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HpRecovery_TenPercentPerLevel()
        {
            Assert.AreEqual("毎ターン最大ＨＰの20%分のＨＰを回復。", FeatureMessage("ＨＰ回復Lv2"));
        }

        [TestMethod]
        public void EnRecovery_TenPercentPerLevel()
        {
            Assert.AreEqual("毎ターン最大ＥＮの30%分のＥＮを回復。", FeatureMessage("ＥＮ回復Lv3"));
        }

        [TestMethod]
        public void HpConsumption_TenPercentPerLevel()
        {
            Assert.AreEqual("毎ターン最大ＨＰの10%分のＨＰを消費。", FeatureMessage("ＨＰ消費Lv1"));
        }

        [TestMethod]
        public void MeleeUp_FivePerLevel()
        {
            Assert.AreEqual("パイロットの格闘を+10。", FeatureMessage("格闘強化Lv2"));
        }

        [TestMethod]
        public void HpUp_TwoHundredPerLevel_WithMoraleCondition()
        {
            // 第2引数が数値なら発動に必要な気力
            Assert.AreEqual("最大ＨＰを600増加。;気力120以上で発動。", FeatureMessage("ＨＰ強化Lv3=ＨＰ強化 120"));
        }

        [TestMethod]
        public void ArmorRateUp_FivePercentPerLevel()
        {
            Assert.AreEqual("装甲を10%分増加。", FeatureMessage("装甲割合強化Lv2"));
        }

        [TestMethod]
        public void Ecm_ReducesHitRateAndCancelsEnemyEcm()
        {
            Assert.AreEqual(
                "半径３マス以内の味方ユニットに対する攻撃の命中率を元の85%に減少させる。同時に相手のＥＣＭ能力のレベルを、この能力のレベル分だけ下げる。;思念誘導攻撃や近接攻撃には無効。",
                FeatureMessage("ＥＣＭLv3"));
        }

        [TestMethod]
        public void Boost_TwentyFivePercent()
        {
            Assert.AreEqual("気力130以上で発動し、ダメージを 25% アップ。", FeatureMessage("ブースト"));
        }

        // ──────────────────────────────────────────────
        // 移動系
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Jump_SpeedPlusLevelAndEn()
        {
            // 移動力 = ユニットの移動力(5) + Lv(2)
            Assert.AreEqual("移動力7で地上地形を無視しながらジャンプ移動。;10ＥＮ消費。", FeatureMessage("ジャンプLv2=ジャンプ 10"));
        }

        [TestMethod]
        public void Teleport_WithoutLevel_SpeedPlusOne()
        {
            // VB: 移動力 = u.Speed + AllFeatureLevel(=1)
            Assert.AreEqual("テレポートを行い、移動力6で地形を無視して移動。;20ＥＮ消費。", FeatureMessage("テレポート=テレポート 20"));
        }

        [TestMethod]
        public void Teleport_WithLevel_SpeedPlusLevel()
        {
            Assert.AreEqual("テレポートを行い、移動力6で地形を無視して移動。;20ＥＮ消費。", FeatureMessage("テレポートLv1=テレポート 20"));
        }

        [TestMethod]
        public void Hover_DescribesEnCost()
        {
            Assert.AreEqual(
                "空中に浮きながら移動することで砂漠と雪原の移動コストが１になる。また、水上移動も可能。ただし移動時に5ＥＮ消費。",
                FeatureMessage("ホバー移動"));
        }

        [TestMethod]
        public void MoveRestriction_ListsTerrains()
        {
            Assert.AreEqual("平地、森林上のみを移動可能。", FeatureMessage("移動制限=移動制限 平地 森林"));
        }

        [TestMethod]
        public void NoEntry_ListsTerrains()
        {
            Assert.AreEqual("宇宙、月面には進入不可。", FeatureMessage("進入不可=進入不可 宇宙 月面"));
        }

        [TestMethod]
        public void AdditionalSpeed_TerrainAndLevel()
        {
            Assert.AreEqual("平地にいると、移動力が2増加。", FeatureMessage("追加移動力Lv2=追加移動力 平地"));
        }

        [TestMethod]
        public void Stealth_WithLevel_UsesWideDigits()
        {
            // レベル指定ありなら全角数字のマス数
            Assert.AreEqual(
                "敵から５マス以内にいない限り発見されない。ただし自分から攻撃すると１ターン無効。",
                FeatureMessage("ステルスLv5"));
        }

        [TestMethod]
        public void Stealth_WithoutLevel_ThreeSquares()
        {
            Assert.AreEqual(
                "敵から３マス以内にいない限り発見されない。ただし自分から攻撃すると１ターン無効。",
                FeatureMessage("ステルス"));
        }

        [TestMethod]
        public void Zoc_DefaultRange()
        {
            Assert.AreEqual(
                "このユニットから1マス以内を通過する敵ユニットに、ＺＯＣによる影響を与える。",
                FeatureMessage("ＺＯＣLv1"));
        }

        [TestMethod]
        public void ZocNullify_LevelOneVsHigher()
        {
            Assert.AreEqual("このユニットは敵ユニットによるＺＯＣの影響を受けない。", FeatureMessage("ＺＯＣ無効化Lv1"));
            Assert.AreEqual("このユニットは敵ユニットによる2レベル以下のＺＯＣの影響を受けない。", FeatureMessage("ＺＯＣ無効化Lv2"));
        }

        [TestMethod]
        public void TerrainEffectNullify_ListedOrAll()
        {
            Assert.AreEqual(
                "砂漠、宇宙のＨＰ・ＥＮ減少や状態付加等の特殊効果を無効化する。",
                FeatureMessage("地形効果無効化=地形効果無効化 砂漠 宇宙"));
            Assert.AreEqual(
                "全地形のＨＰ・ＥＮ減少や状態付加等の特殊効果を無効化する。",
                FeatureMessage("地形効果無効化"));
        }

        // ──────────────────────────────────────────────
        // その他
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RepairDevice_LevelSelectsAmount()
        {
            Assert.AreEqual("他のユニットのＨＰを最大ＨＰの50%だけ回復。", FeatureMessage("修理装置Lv2"));
            Assert.AreEqual("他のユニットのＨＰを全快。", FeatureMessage("修理装置Lv3"));
        }

        [TestMethod]
        public void RepairDevice_WithoutLevel_ThirtyPercent()
        {
            Assert.AreEqual("他のユニットのＨＰを最大ＨＰの30%だけ回復。", FeatureMessage("修理装置"));
        }

        [TestMethod]
        public void SupplyDevice_MoraleCost()
        {
            Assert.AreEqual("他のユニットのＥＮと弾薬を全快。;ただしユニットのパイロットの気力は-10。", FeatureMessage("補給装置"));
        }

        [TestMethod]
        public void Dummy_NameAndCount()
        {
            // 身代わりの名称は能力名（別名）から Lv 以降を除いたもの
            Assert.AreEqual("デコイを身代わりにして攻撃を2回まで回避。", FeatureMessage("ダミーLv2=デコイ"));
        }

        [TestMethod]
        public void SimpleFlags_HaveFixedText()
        {
            Assert.AreEqual("攻撃を受けた際に防御運動を取ることが出来ない。", FeatureMessage("防御不可"));
            Assert.AreEqual("攻撃を受けた際に回避運動を取ることが出来ない。", FeatureMessage("回避不可"));
            Assert.AreEqual("母艦に格納することが出来ない。", FeatureMessage("格納不可"));
            Assert.AreEqual("50%の確率で攻撃を完全に回避。;発動条件：気力130以上", FeatureMessage("分身"));
        }

        [TestMethod]
        public void DummyFeature_UsesLastDataElementWithoutQuotes()
        {
            // 未知の能力（ダミー能力）はデータの最後の要素を解説として使う
            Assert.AreEqual("独自の解説", FeatureMessage("独自能力=独自 \"独自の解説\""));
        }

        [TestMethod]
        public void UnknownFeature_WithoutData_ReturnsEmpty()
        {
            // 解説がない未知の能力は空
            Assert.IsTrue(string.IsNullOrEmpty(FeatureMessage("謎能力")));
        }

        [TestMethod]
        public void UnknownFeature_WithLevelWithoutData_ReturnsEmpty()
        {
            // VB は AllFeatureData が "" を返すので解説なしとして空を返す
            Assert.IsTrue(string.IsNullOrEmpty(FeatureMessage("謎能力Lv2")));
        }

        [TestMethod]
        public void Explanation_DefinedByAliasFeature_OverridesText()
        {
            // 別名と同名の「解説」能力があれば、その解説文で置き換える
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("ビーム吸収=ビームコート");
            fx.UnitData.AddFeature("ビームコート=解説 \"ビームを 弾く\"");
            var u = fx.Build();
            Assert.AreEqual("ビームを 弾く", fx.Src.Help.FeatureHelpMessage(u, 1, false));
        }

        [TestMethod]
        public void TermRename_IsReflected()
        {
            // 用語名能力で ＨＰ を別名にすると解説文にも反映される
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("用語名=ＨＰ 体力");
            fx.UnitData.AddFeature("ＨＰ回復Lv1");
            var u = fx.Build();
            Assert.AreEqual("毎ターン最大体力の10%分の体力を回復。", fx.Src.Help.FeatureHelpMessage(u, "ＨＰ回復", false));
        }

        [TestMethod]
        public void LifeSizeOption_ReplacesPilotWord()
        {
            // 等身大基準オプションでは「パイロット」を「ユニット」に置き換える
            var fx = new HelpTestFixture();
            fx.UnitData.AddFeature("格闘強化Lv1");
            var u = fx.Build();
            fx.Src.Expression.DefineGlobalVariable("Option(等身大基準)");
            Assert.AreEqual("ユニットの格闘を+5。", fx.Src.Help.FeatureHelpMessage(u, 1, false));
        }
    }
}
