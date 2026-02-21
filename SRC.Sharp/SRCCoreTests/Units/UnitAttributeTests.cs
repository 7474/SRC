using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit の防御属性判定メソッドのユニットテスト
    /// </summary>
    [TestClass]
    public class UnitAttributeTests
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // Absorb (吸収属性)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Absorb_AllAttribute_ReturnsTrue_ForAnyAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strAbsorb = "全";
            Assert.IsTrue(unit.Absorb("火"));
        }

        [TestMethod]
        public void Absorb_AllAttribute_ReturnsTrue_ForEmptyAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strAbsorb = "全";
            Assert.IsTrue(unit.Absorb(""));
        }

        [TestMethod]
        public void Absorb_PhysicalAttribute_ReturnsTrue_ForNonMagicAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strAbsorb = "物";
            Assert.IsTrue(unit.Absorb("実"));
        }

        [TestMethod]
        public void Absorb_PhysicalAttribute_ReturnsFalse_ForMagicAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strAbsorb = "物";
            Assert.IsFalse(unit.Absorb("魔法"));
        }

        [TestMethod]
        public void Absorb_PhysicalAttribute_ReturnsTrue_ForEmptyAttribute()
        {
            // 無属性は物理攻撃に分類される
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strAbsorb = "物";
            Assert.IsTrue(unit.Absorb(""));
        }

        [TestMethod]
        public void Absorb_MagicAttribute_ReturnsTrue_ForMagicAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strAbsorb = "魔";
            Assert.IsTrue(unit.Absorb("魔法"));
        }

        [TestMethod]
        public void Absorb_MagicAttribute_ReturnsFalse_ForMagicWeapon()
        {
            // 「魔武」は魔法武器なので魔属性に該当しない
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strAbsorb = "魔";
            Assert.IsFalse(unit.Absorb("魔武"));
        }

        [TestMethod]
        public void Absorb_SpecificAttribute_ReturnsTrue_WhenMatches()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strAbsorb = "火";
            Assert.IsTrue(unit.Absorb("火炎"));
        }

        [TestMethod]
        public void Absorb_SpecificAttribute_ReturnsFalse_WhenNoMatch()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strAbsorb = "火";
            Assert.IsFalse(unit.Absorb("氷"));
        }

        [TestMethod]
        public void Absorb_EmptyAbsorbString_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strAbsorb = "";
            Assert.IsFalse(unit.Absorb("火"));
        }

        // ──────────────────────────────────────────────
        // Immune (無効化属性)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Immune_AllAttribute_ReturnsTrue_ForAnyAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strImmune = "全";
            Assert.IsTrue(unit.Immune("火"));
        }

        [TestMethod]
        public void Immune_PhysicalAttribute_ReturnsTrue_ForEmptyAttribute()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strImmune = "物";
            Assert.IsTrue(unit.Immune(""));
        }

        [TestMethod]
        public void Immune_PhysicalAttribute_ReturnsFalse_ForMagicAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strImmune = "物";
            Assert.IsFalse(unit.Immune("魔"));
        }

        [TestMethod]
        public void Immune_SpecificAttribute_ReturnsTrue_WhenMatches()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strImmune = "火";
            Assert.IsTrue(unit.Immune("火"));
        }

        [TestMethod]
        public void Immune_EmptyImmuneString_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strImmune = "";
            Assert.IsFalse(unit.Immune("火"));
        }

        [TestMethod]
        public void Immune_MagicAttribute_ReturnsFalse_ForMagicGun()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strImmune = "魔";
            Assert.IsFalse(unit.Immune("魔銃"));
        }

        // ──────────────────────────────────────────────
        // Resist (耐性属性)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Resist_AllAttribute_ReturnsTrue_ForAnyAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strResist = "全";
            Assert.IsTrue(unit.Resist("実"));
        }

        [TestMethod]
        public void Resist_PhysicalAttribute_ReturnsTrue_ForEmptyAttribute()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strResist = "物";
            Assert.IsTrue(unit.Resist(""));
        }

        [TestMethod]
        public void Resist_SpecificAttribute_ReturnsTrue_WhenMatches()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strResist = "火";
            Assert.IsTrue(unit.Resist("火炎"));
        }

        [TestMethod]
        public void Resist_EmptyResistString_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strResist = "";
            Assert.IsFalse(unit.Resist("火"));
        }

        [TestMethod]
        public void Resist_MagicAttribute_ReturnsFalse_ForMagicContact()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strResist = "魔";
            Assert.IsFalse(unit.Resist("魔接"));
        }

        // ──────────────────────────────────────────────
        // Weakness (弱点属性)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Weakness_AllAttribute_ReturnsTrue_ForAnyAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strWeakness = "全";
            Assert.IsTrue(unit.Weakness("火"));
        }

        [TestMethod]
        public void Weakness_PhysicalAttribute_ReturnsTrue_ForEmptyAttribute()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strWeakness = "物";
            Assert.IsTrue(unit.Weakness(""));
        }

        [TestMethod]
        public void Weakness_PhysicalAttribute_ReturnsFalse_ForMagicAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strWeakness = "物";
            Assert.IsFalse(unit.Weakness("精神攻撃"));
        }

        [TestMethod]
        public void Weakness_SpecificAttribute_ReturnsTrue_WhenMatches()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strWeakness = "氷";
            Assert.IsTrue(unit.Weakness("氷結"));
        }

        [TestMethod]
        public void Weakness_EmptyWeaknessString_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strWeakness = "";
            Assert.IsFalse(unit.Weakness("火"));
        }

        // ──────────────────────────────────────────────
        // Effective (有効属性)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Effective_AllAttribute_ReturnsTrue_ForAnyAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strEffective = "全";
            Assert.IsTrue(unit.Effective("火"));
        }

        [TestMethod]
        public void Effective_PhysicalAttribute_ReturnsTrue_ForEmptyAttribute()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strEffective = "物";
            Assert.IsTrue(unit.Effective(""));
        }

        [TestMethod]
        public void Effective_SpecificAttribute_ReturnsTrue_WhenMatches()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strEffective = "電";
            Assert.IsTrue(unit.Effective("電撃"));
        }

        [TestMethod]
        public void Effective_EmptyEffectiveString_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strEffective = "";
            Assert.IsFalse(unit.Effective("火"));
        }

        [TestMethod]
        public void Effective_PhysicalAttribute_ReturnsFalse_ForMagicAttack()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strEffective = "物";
            Assert.IsFalse(unit.Effective("魔法"));
        }

        // ──────────────────────────────────────────────
        // SpecialEffectImmune (特殊効果無効化属性)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SpecialEffectImmune_AllAttribute_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strSpecialEffectImmune = "全";
            Assert.IsTrue(unit.SpecialEffectImmune("火"));
        }

        [TestMethod]
        public void SpecialEffectImmune_EmptyAname_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strSpecialEffectImmune = "火";
            Assert.IsFalse(unit.SpecialEffectImmune(""));
        }

        [TestMethod]
        public void SpecialEffectImmune_MatchingAttribute_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strSpecialEffectImmune = "火";
            Assert.IsTrue(unit.SpecialEffectImmune("火"));
        }

        [TestMethod]
        public void SpecialEffectImmune_NoMatch_ReturnsFalse()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            unit.strSpecialEffectImmune = "火";
            Assert.IsFalse(unit.SpecialEffectImmune("氷"));
        }

        // ──────────────────────────────────────────────
        // IsAttributeClassified (属性該当判定)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void IsAttributeClassified_EmptyClass1_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            // class1 が空の場合は常に true
            Assert.IsTrue(unit.IsAttributeClassified("", "火"));
        }

        [TestMethod]
        public void IsAttributeClassified_Class1IsAll_ReturnsTrue()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsAttributeClassified("全", "火"));
        }

        [TestMethod]
        public void IsAttributeClassified_Class1IsAll_ReturnsTrue_ForEmptyClass2()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsAttributeClassified("全", ""));
        }

        [TestMethod]
        public void IsAttributeClassified_PhysicalClass1_ReturnsTrue_ForNonMagicClass2()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsAttributeClassified("物", "実"));
        }

        [TestMethod]
        public void IsAttributeClassified_PhysicalClass1_ReturnsFalse_ForMagicClass2()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsFalse(unit.IsAttributeClassified("物", "魔法"));
        }

        [TestMethod]
        public void IsAttributeClassified_PhysicalClass1_ReturnsTrue_ForEmptyClass2()
        {
            // 無属性は物理攻撃に分類される
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsAttributeClassified("物", ""));
        }

        [TestMethod]
        public void IsAttributeClassified_SpecificAttribute_ReturnsTrue_WhenMatches()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsAttributeClassified("火", "火炎"));
        }

        [TestMethod]
        public void IsAttributeClassified_SpecificAttribute_ReturnsFalse_WhenNoMatch()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsFalse(unit.IsAttributeClassified("火", "氷結"));
        }

        [TestMethod]
        public void IsAttributeClassified_MagicClass1_ReturnsTrue_ForPureMagicClass2()
        {
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsAttributeClassified("魔", "魔法"));
        }

        [TestMethod]
        public void IsAttributeClassified_MagicClass1_ReturnsFalse_ForMagicWeapon()
        {
            // 「魔武」は魔法武器なので「魔」クラスには該当しない
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsFalse(unit.IsAttributeClassified("魔", "魔武"));
        }

        [TestMethod]
        public void IsAttributeClassified_NegationClass1_ReturnsFalse_WhenMatches()
        {
            // "!火" → 火属性でないことを要求
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsFalse(unit.IsAttributeClassified("火!", "火炎"));
        }

        [TestMethod]
        public void IsAttributeClassified_NegationClass1_ReturnsTrue_WhenNotMatches()
        {
            // "!火" → 氷属性なので true
            var src = CreateSrc();
            var unit = new Unit(src);
            Assert.IsTrue(unit.IsAttributeClassified("火!", "氷結"));
        }
    }
}
