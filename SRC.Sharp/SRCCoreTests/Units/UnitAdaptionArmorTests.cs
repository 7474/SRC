using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// 移植精度検証: Unit の地形適応計算 (get_AdaptionMod / get_Adaption) と
    /// 装甲値計算 (get_Armor) のユニットテスト。
    /// VB6 版の数値をそのまま期待値として検証する。
    /// </summary>
    [TestClass]
    public class UnitAdaptionArmorTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // ヘルパー: private フィールドを直接設定する
        // ──────────────────────────────────────────────

        private static void SetAdaption(Unit unit, string adaption)
        {
            var f = typeof(Unit).GetField("strAdaption", BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(unit, adaption);
        }

        private static void SetArmor(Unit unit, int armor)
        {
            var f = typeof(Unit).GetField("lngArmor", BindingFlags.NonPublic | BindingFlags.Instance);
            f.SetValue(unit, armor);
        }

        /// <summary>
        /// パイロットを生成してユニットに搭乗させ、パイロットの地形適応を設定する。
        /// </summary>
        private static void AddPilotWithAdaption(SRC src, Unit unit, string adaption)
        {
            var name = "テストパイロット_" + System.Guid.NewGuid().ToString("N");
            var pd = src.PDList.Add(name);
            pd.SP = 10;
            var pilot = src.PList.Add(name, 1, "味方");
            pilot.Adaption = adaption;
            unit.AddPilot(pilot);
            pilot.Unit = unit;
        }

        // ══════════════════════════════════════════════
        // get_Adaption — 地形適応文字列の各位置のパース
        // パイロットなし → pad=4(A相当) → min(uad, pad)
        // ══════════════════════════════════════════════

        [TestMethod]
        public void GetAdaption_NoPilot_S_Returns4()
        {
            // S(uad=5) と pad=4(パイロットなし) → min(5,4)=4
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "SAAA");
            Assert.AreEqual(4, unit.get_Adaption(1));
        }

        [TestMethod]
        public void GetAdaption_NoPilot_A_Returns4()
        {
            // A(uad=4) と pad=4(パイロットなし) → min(4,4)=4
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "AAAA");
            Assert.AreEqual(4, unit.get_Adaption(1));
        }

        [TestMethod]
        public void GetAdaption_NoPilot_B_Returns3()
        {
            // B(uad=3) と pad=4(パイロットなし) → min(3,4)=3
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "BAAA");
            Assert.AreEqual(3, unit.get_Adaption(1));
        }

        [TestMethod]
        public void GetAdaption_NoPilot_C_Returns2()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "CAAA");
            Assert.AreEqual(2, unit.get_Adaption(1));
        }

        [TestMethod]
        public void GetAdaption_NoPilot_D_Returns1()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "DAAA");
            Assert.AreEqual(1, unit.get_Adaption(1));
        }

        [TestMethod]
        public void GetAdaption_Dash_Returns0()
        {
            // "-" → 即座に 0 を返す (パイロット有無に関係なし)
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "-AAA");
            Assert.AreEqual(0, unit.get_Adaption(1));
        }

        [TestMethod]
        public void GetAdaption_Empty_Returns0()
        {
            // 地形適応文字列が空 → uad=0 → min(0,4)=0
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "");
            Assert.AreEqual(0, unit.get_Adaption(1));
        }

        [TestMethod]
        public void GetAdaption_WithPilot_S_Returns5()
        {
            // ユニットS(5) かつパイロットS(5) → min(5,5)=5
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "SAAA");
            AddPilotWithAdaption(src, unit, "SAAA");
            Assert.AreEqual(5, unit.get_Adaption(1));
        }

        [TestMethod]
        public void GetAdaption_PilotLimitsUnit_Returns2()
        {
            // ユニットS(5) かつパイロットC(2) → min(5,2)=2
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "SAAA");
            AddPilotWithAdaption(src, unit, "CAAA");
            Assert.AreEqual(2, unit.get_Adaption(1));
        }

        [TestMethod]
        public void GetAdaption_MultiplePositions_ReturnsCorrectValues()
        {
            // パイロットなし → 空中=S→4, 地上=A→4, 水中=B→3, 宇宙=C→2
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "SABC");
            Assert.AreEqual(4, unit.get_Adaption(1)); // 空中 S → 4 (padで制限)
            Assert.AreEqual(4, unit.get_Adaption(2)); // 地上 A → 4
            Assert.AreEqual(3, unit.get_Adaption(3)); // 水中 B → 3
            Assert.AreEqual(2, unit.get_Adaption(4)); // 宇宙 C → 2
        }

        // ══════════════════════════════════════════════
        // get_AdaptionMod — デフォルトモード (オプションなし)
        // ══════════════════════════════════════════════
        // S=1.4, A=1.2, B=1.0, C=0.8, D=0.6, -/""=0.0
        // ※ パイロットなしの場合 pad=4(A) で制限されるため
        //   ユニット単体で S=1.4 を得るにはパイロットも S が必要

        [TestMethod]
        public void AdaptionMod_DefaultMode_WithPilot_S_Returns1_4()
        {
            // ユニット&パイロット共に S → uad=5 → 1.4
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "SAAA");
            AddPilotWithAdaption(src, unit, "SAAA");
            Assert.AreEqual(1.4d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_DefaultMode_NoPilot_S_Returns1_2()
        {
            // ユニットSだがパイロットなし → pad=4(A) → uad=4 → 1.2
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "SAAA");
            Assert.AreEqual(1.2d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_DefaultMode_A_Returns1_2()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "AAAA");
            Assert.AreEqual(1.2d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_DefaultMode_B_Returns1_0()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "BAAA");
            Assert.AreEqual(1.0d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_DefaultMode_C_Returns0_8()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "CAAA");
            Assert.AreEqual(0.8d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_DefaultMode_D_Returns0_6()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "DAAA");
            Assert.AreEqual(0.6d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_DefaultMode_Dash_Returns0_0()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "-AAA");
            Assert.AreEqual(0.0d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_DefaultMode_Empty_Returns0_0()
        {
            // 地形適応文字列が空 → uad=0 → 0.0
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "");
            Assert.AreEqual(0.0d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_DefaultMode_GroundIdx2_B_Returns1_0()
        {
            // "ABAA": 空中=A, 地上=B, 水中=A, 宇宙=A → 地上 idx=2 → B → 1.0
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "ABAA");
            Assert.AreEqual(1.0d, unit.get_AdaptionMod(2, 0));
        }

        [TestMethod]
        public void AdaptionMod_DefaultMode_SpaceIdx4_D_Returns0_6()
        {
            // 宇宙適応 D → 1
            var src = CreateSrc();
            var unit = new Unit(src);
            SetAdaption(unit, "AAAD");
            Assert.AreEqual(0.6d, unit.get_AdaptionMod(4, 0));
        }

        // ══════════════════════════════════════════════
        // get_AdaptionMod — 地形適応修正緩和オプション
        // ══════════════════════════════════════════════
        // S=1.2, A=1.1, B=1.0, C=0.9, D=0.8

        [TestMethod]
        public void AdaptionMod_Kanwa_WithPilot_S_Returns1_2()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            var unit = new Unit(src);
            SetAdaption(unit, "SAAA");
            AddPilotWithAdaption(src, unit, "SAAA");
            Assert.AreEqual(1.2d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_Kanwa_A_Returns1_1()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            var unit = new Unit(src);
            SetAdaption(unit, "AAAA");
            Assert.AreEqual(1.1d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_Kanwa_B_Returns1_0()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            var unit = new Unit(src);
            SetAdaption(unit, "BAAA");
            Assert.AreEqual(1.0d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_Kanwa_C_Returns0_9()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            var unit = new Unit(src);
            SetAdaption(unit, "CAAA");
            Assert.AreEqual(0.9d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_Kanwa_D_Returns0_8()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            var unit = new Unit(src);
            SetAdaption(unit, "DAAA");
            Assert.AreEqual(0.8d, unit.get_AdaptionMod(1, 0));
        }

        // ══════════════════════════════════════════════
        // get_AdaptionMod — 地形適応修正繰り下げオプション
        // ══════════════════════════════════════════════
        // S=1.2, A=1.0, B=0.8, C=0.6, D=0.4

        [TestMethod]
        public void AdaptionMod_Kuriagari_WithPilot_S_Returns1_2()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var unit = new Unit(src);
            SetAdaption(unit, "SAAA");
            AddPilotWithAdaption(src, unit, "SAAA");
            Assert.AreEqual(1.2d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_Kuriagari_A_Returns1_0()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var unit = new Unit(src);
            SetAdaption(unit, "AAAA");
            Assert.AreEqual(1.0d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_Kuriagari_B_Returns0_8()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var unit = new Unit(src);
            SetAdaption(unit, "BAAA");
            Assert.AreEqual(0.8d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_Kuriagari_C_Returns0_6()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var unit = new Unit(src);
            SetAdaption(unit, "CAAA");
            Assert.AreEqual(0.6d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_Kuriagari_D_Returns0_4()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var unit = new Unit(src);
            SetAdaption(unit, "DAAA");
            Assert.AreEqual(0.4d, unit.get_AdaptionMod(1, 0));
        }

        // ══════════════════════════════════════════════
        // get_AdaptionMod — 両オプション同時 (緩和+繰り下げ)
        // ══════════════════════════════════════════════
        // S=1.1, A=1.0, B=0.9, C=0.8, D=0.7

        [TestMethod]
        public void AdaptionMod_KanwaAndKuriagari_WithPilot_S_Returns1_1()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var unit = new Unit(src);
            SetAdaption(unit, "SAAA");
            AddPilotWithAdaption(src, unit, "SAAA");
            Assert.AreEqual(1.1d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_KanwaAndKuriagari_A_Returns1_0()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var unit = new Unit(src);
            SetAdaption(unit, "AAAA");
            Assert.AreEqual(1.0d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_KanwaAndKuriagari_B_Returns0_9()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var unit = new Unit(src);
            SetAdaption(unit, "BAAA");
            Assert.AreEqual(0.9d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_KanwaAndKuriagari_C_Returns0_8()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var unit = new Unit(src);
            SetAdaption(unit, "CAAA");
            Assert.AreEqual(0.8d, unit.get_AdaptionMod(1, 0));
        }

        [TestMethod]
        public void AdaptionMod_KanwaAndKuriagari_D_Returns0_7()
        {
            var src = CreateSrc();
            src.Expression.DefineGlobalVariable("Option(地形適応修正緩和)");
            src.Expression.DefineGlobalVariable("Option(地形適応修正繰り下げ)");
            var unit = new Unit(src);
            SetAdaption(unit, "DAAA");
            Assert.AreEqual(0.7d, unit.get_AdaptionMod(1, 0));
        }

        // ══════════════════════════════════════════════
        // get_Armor — 基本値 (条件なし)
        // ══════════════════════════════════════════════

        [TestMethod]
        public void Armor_NoCondition_ReturnsBaseArmor()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 1000);
            Assert.AreEqual(1000, unit.get_Armor(""));
        }

        [TestMethod]
        public void Armor_ZeroArmor_ReturnsZero()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 0);
            Assert.AreEqual(0, unit.get_Armor(""));
        }

        [TestMethod]
        public void Armor_LargeValue_Returns99999()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 99999);
            Assert.AreEqual(99999, unit.get_Armor(""));
        }

        // ══════════════════════════════════════════════
        // get_Armor — 装甲劣化 (装甲半減)
        // ══════════════════════════════════════════════

        [TestMethod]
        public void Armor_WithSokoKaRei_HalvesArmor()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 1000);
            unit.AddCondition("装甲劣化", -1);
            Assert.AreEqual(500, unit.get_Armor(""));
        }

        [TestMethod]
        public void Armor_WithSokoKaRei_OddArmor_TruncatesHalf()
        {
            // 整数除算 → 切り捨て
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 1001);
            unit.AddCondition("装甲劣化", -1);
            Assert.AreEqual(500, unit.get_Armor(""));
        }

        [TestMethod]
        public void Armor_WithSokoKaRei_BaseMode_HalvesArmor()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 1000);
            unit.AddCondition("装甲劣化", -1);
            Assert.AreEqual(500, unit.get_Armor("基本値"));
        }

        // ══════════════════════════════════════════════
        // get_Armor — 石化 (装甲2倍)
        // ══════════════════════════════════════════════

        [TestMethod]
        public void Armor_WithPetrify_DoublesArmor()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 800);
            unit.AddCondition("石化", -1);
            Assert.AreEqual(1600, unit.get_Armor(""));
        }

        [TestMethod]
        public void Armor_WithPetrify_BaseMode_DoublesArmor()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 800);
            unit.AddCondition("石化", -1);
            Assert.AreEqual(1600, unit.get_Armor("基本値"));
        }

        // ══════════════════════════════════════════════
        // get_Armor — 凍結 (装甲半減)
        // ══════════════════════════════════════════════

        [TestMethod]
        public void Armor_WithFreeze_HalvesArmor()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 1000);
            unit.AddCondition("凍結", -1);
            Assert.AreEqual(500, unit.get_Armor(""));
        }

        [TestMethod]
        public void Armor_WithFreeze_BaseMode_HalvesArmor()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 1000);
            unit.AddCondition("凍結", -1);
            Assert.AreEqual(500, unit.get_Armor("基本値"));
        }

        // ══════════════════════════════════════════════
        // get_Armor — 修正値モード
        // ══════════════════════════════════════════════

        [TestMethod]
        public void Armor_ModificationMode_ReturnsZero_WhenNoPilot()
        {
            // "修正値" モードはパイロット修正のみ返す → パイロットなしなら 0
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 1000);
            Assert.AreEqual(0, unit.get_Armor("修正値"));
        }

        // ══════════════════════════════════════════════
        // get_Armor — 基本値モード (条件と組み合わせ)
        // ══════════════════════════════════════════════

        [TestMethod]
        public void Armor_BaseMode_NoCondition_ReturnsBaseArmor()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 1500);
            Assert.AreEqual(1500, unit.get_Armor("基本値"));
        }

        [TestMethod]
        public void Armor_BaseMode_WithPetrifyAndFreeze_AppliesToDifferentConditions()
        {
            // 石化: 2倍, 凍結: 半減 は独立して適用
            var src = CreateSrc();
            var unit = new Unit(src);
            SetArmor(unit, 400);
            unit.AddCondition("凍結", -1);
            // 凍結のみ → 400/2 = 200
            Assert.AreEqual(200, unit.get_Armor("基本値"));
        }
    }
}
