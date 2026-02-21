using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AliasDataElement クラスのフィールドテスト
    /// </summary>
    [TestClass]
    public class AliasDataElementTests
    {
        // ──────────────────────────────────────────────
        // フィールドのデフォルト値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void DefaultValues_AreNull_OrZero_OrFalse()
        {
            var elem = new AliasDataElement();
            Assert.IsNull(elem.strAliasType);
            Assert.AreEqual(0d, elem.dblAliasLevel);
            Assert.IsFalse(elem.blnAliasLevelIsPlusMod);
            Assert.IsFalse(elem.blnAliasLevelIsMultMod);
            Assert.IsNull(elem.strAliasData);
            Assert.IsNull(elem.strAliasNecessarySkill);
            Assert.IsNull(elem.strAliasNecessaryCondition);
        }

        // ──────────────────────────────────────────────
        // フィールドの設定・読み取り
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StrAliasType_CanBeSetAndRead()
        {
            var elem = new AliasDataElement { strAliasType = "格闘" };
            Assert.AreEqual("格闘", elem.strAliasType);
        }

        [TestMethod]
        public void DblAliasLevel_CanBeSetAndRead()
        {
            var elem = new AliasDataElement { dblAliasLevel = 3.5 };
            Assert.AreEqual(3.5, elem.dblAliasLevel);
        }

        [TestMethod]
        public void BlnAliasLevelIsPlusMod_CanBeSetToTrue()
        {
            var elem = new AliasDataElement { blnAliasLevelIsPlusMod = true };
            Assert.IsTrue(elem.blnAliasLevelIsPlusMod);
        }

        [TestMethod]
        public void BlnAliasLevelIsMultMod_CanBeSetToTrue()
        {
            var elem = new AliasDataElement { blnAliasLevelIsMultMod = true };
            Assert.IsTrue(elem.blnAliasLevelIsMultMod);
        }

        [TestMethod]
        public void StrAliasData_CanBeSetAndRead()
        {
            var elem = new AliasDataElement { strAliasData = "火炎属性" };
            Assert.AreEqual("火炎属性", elem.strAliasData);
        }

        [TestMethod]
        public void StrAliasNecessarySkill_CanBeSetAndRead()
        {
            var elem = new AliasDataElement { strAliasNecessarySkill = "格闘技能" };
            Assert.AreEqual("格闘技能", elem.strAliasNecessarySkill);
        }

        [TestMethod]
        public void StrAliasNecessaryCondition_CanBeSetAndRead()
        {
            var elem = new AliasDataElement { strAliasNecessaryCondition = "強化後" };
            Assert.AreEqual("強化後", elem.strAliasNecessaryCondition);
        }

        // ──────────────────────────────────────────────
        // 相互排他性のテスト (PlusMod と MultMod は同時に設定されないはず)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PlusMod_True_MultMod_False_AreMutuallyExclusive()
        {
            var elem = new AliasDataElement
            {
                blnAliasLevelIsPlusMod = true,
                blnAliasLevelIsMultMod = false
            };
            Assert.IsTrue(elem.blnAliasLevelIsPlusMod);
            Assert.IsFalse(elem.blnAliasLevelIsMultMod);
        }

        [TestMethod]
        public void MultMod_True_PlusMod_False_AreMutuallyExclusive()
        {
            var elem = new AliasDataElement
            {
                blnAliasLevelIsPlusMod = false,
                blnAliasLevelIsMultMod = true
            };
            Assert.IsFalse(elem.blnAliasLevelIsPlusMod);
            Assert.IsTrue(elem.blnAliasLevelIsMultMod);
        }

        // ──────────────────────────────────────────────
        // AddAlias を使った組み合わせテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddAlias_AllFieldsPopulated_PlusModType()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("射撃Lv+5=熱 (ビームライフル技能)");

            var elem = ad.Elements[0];
            Assert.AreEqual("射撃", elem.strAliasType);
            Assert.AreEqual(5d, elem.dblAliasLevel);
            Assert.IsTrue(elem.blnAliasLevelIsPlusMod);
            Assert.AreEqual("熱", elem.strAliasData);
            Assert.AreEqual("ビームライフル技能", elem.strAliasNecessarySkill);
        }

        [TestMethod]
        public void AddAlias_LevelNegative_PlusMod()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("防御Lv+-2");
            Assert.AreEqual(-2d, ad.Elements[0].dblAliasLevel);
            Assert.IsTrue(ad.Elements[0].blnAliasLevelIsPlusMod);
        }
    }
}
