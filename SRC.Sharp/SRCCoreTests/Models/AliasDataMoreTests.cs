using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AliasDataType / AliasDataElement の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class AliasDataMoreTests
    {
        // ──────────────────────────────────────────────
        // Lv* 乗算修正
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddAlias_MultModifier_LevelIsParsed()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃Lv*3");
            Assert.AreEqual(3d, ad.Elements[0].dblAliasLevel);
        }

        [TestMethod]
        public void AddAlias_MultModifier_TypeIsParsed()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘Lv*2");
            Assert.AreEqual("格闘", ad.Elements[0].strAliasType);
        }

        [TestMethod]
        public void AddAlias_MultModifier_IsMultModFlagSet_PlusModFlagNotSet()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("防御Lv*4");
            Assert.IsTrue(ad.Elements[0].blnAliasLevelIsMultMod);
            Assert.IsFalse(ad.Elements[0].blnAliasLevelIsPlusMod);
        }

        // ──────────────────────────────────────────────
        // 必要条件のみの場合 <cond>
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddAlias_OnlyCondition_SetsNecessaryCondition()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("<特定条件>");
            Assert.AreEqual("特定条件", ad.Elements[0].strAliasNecessaryCondition);
        }

        [TestMethod]
        public void AddAlias_OnlyCondition_TypeIsEmpty()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("<特定条件>");
            Assert.AreEqual("", ad.Elements[0].strAliasType);
        }

        [TestMethod]
        public void AddAlias_TypeWithCondition_ParsesBoth()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃 <強化後>");
            Assert.AreEqual("攻撃", ad.Elements[0].strAliasType);
            Assert.AreEqual("強化後", ad.Elements[0].strAliasNecessaryCondition);
        }

        // ──────────────────────────────────────────────
        // Lv=data + 必要技能 (skill) の組み合わせ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddAlias_LvAndDataAndSkill_ParsesType()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘強化Lv3=物理 (格闘技能)");
            Assert.AreEqual("格闘強化", ad.Elements[0].strAliasType);
        }

        [TestMethod]
        public void AddAlias_LvAndDataAndSkill_ParsesLevel()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘強化Lv3=物理 (格闘技能)");
            Assert.AreEqual(3d, ad.Elements[0].dblAliasLevel);
        }

        [TestMethod]
        public void AddAlias_LvAndDataAndSkill_ParsesData()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘強化Lv3=物理 (格闘技能)");
            Assert.AreEqual("物理", ad.Elements[0].strAliasData);
        }

        [TestMethod]
        public void AddAlias_LvAndDataAndSkill_ParsesSkill()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘強化Lv3=物理 (格闘技能)");
            Assert.AreEqual("格闘技能", ad.Elements[0].strAliasNecessarySkill);
        }

        // ──────────────────────────────────────────────
        // 複数要素と HasType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddAlias_MultipleElements_HasType_ChecksAll()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘");
            ad.AddAlias("射撃");
            ad.AddAlias("防御");
            Assert.IsTrue(ad.HasType("格闘"));
            Assert.IsTrue(ad.HasType("射撃"));
            Assert.IsTrue(ad.HasType("防御"));
        }

        [TestMethod]
        public void AddAlias_MultipleElements_HasType_ReturnsFalseForAbsent()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘");
            ad.AddAlias("射撃");
            Assert.IsFalse(ad.HasType("回避"));
        }

        [TestMethod]
        public void AddAlias_MultipleElements_HasType_DistinctTypes()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘Lv2=A");
            ad.AddAlias("射撃Lv3=B");
            Assert.IsTrue(ad.HasType("格闘"));
            Assert.IsTrue(ad.HasType("射撃"));
            Assert.IsFalse(ad.HasType("A"));
        }

        // ──────────────────────────────────────────────
        // ReplaceTypeName（先頭要素が一致する場合）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplaceTypeName_FirstElementMatches_ReturnsFirstType()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘=物理");
            ad.AddAlias("射撃=熱");
            // 最初の要素のデータが一致すれば最初の種類を返す
            var result = ad.ReplaceTypeName("物理");
            Assert.AreEqual("格闘", result);
        }

        [TestMethod]
        public void ReplaceTypeName_SecondElementMatches_ReturnsSecondType()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘=物理");
            ad.AddAlias("射撃=熱");
            var result = ad.ReplaceTypeName("熱");
            Assert.AreEqual("射撃", result);
        }

        [TestMethod]
        public void ReplaceTypeName_NoMatch_ReturnsFirstElementType()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘=物理");
            ad.AddAlias("射撃=熱");
            // どれにも一致しない場合は先頭の種類を返す
            var result = ad.ReplaceTypeName("未知");
            Assert.AreEqual("格闘", result);
        }
    }
}
