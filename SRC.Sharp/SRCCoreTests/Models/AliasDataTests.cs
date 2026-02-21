using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AliasDataType クラスのユニットテスト
    /// </summary>
    [TestClass]
    public class AliasDataTests
    {
        // ──────────────────────────────────────────────
        // AddAlias 基本ケース
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddAlias_SimpleType_AddsElement()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃");
            Assert.AreEqual(1, ad.Count);
            Assert.AreEqual("攻撃", ad.Elements[0].strAliasType);
            Assert.AreEqual("", ad.Elements[0].strAliasData);
        }

        [TestMethod]
        public void AddAlias_TypeWithData_ParsesCorrectly()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃=火炎");
            Assert.AreEqual(1, ad.Count);
            Assert.AreEqual("攻撃", ad.Elements[0].strAliasType);
            Assert.AreEqual("火炎", ad.Elements[0].strAliasData);
        }

        [TestMethod]
        public void AddAlias_TypeWithLevel_ParsesLevel()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃Lv5");
            Assert.AreEqual(1, ad.Count);
            Assert.AreEqual("攻撃", ad.Elements[0].strAliasType);
            Assert.AreEqual(5d, ad.Elements[0].dblAliasLevel);
        }

        [TestMethod]
        public void AddAlias_TypeWithLevelAndData_ParsesBoth()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃Lv3=火炎");
            Assert.AreEqual(1, ad.Count);
            Assert.AreEqual("攻撃", ad.Elements[0].strAliasType);
            Assert.AreEqual(3d, ad.Elements[0].dblAliasLevel);
            Assert.AreEqual("火炎", ad.Elements[0].strAliasData);
        }

        [TestMethod]
        public void AddAlias_PlusModifier_SetsFlag()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃Lv+3");
            Assert.AreEqual(1, ad.Count);
            Assert.IsTrue(ad.Elements[0].blnAliasLevelIsPlusMod);
            Assert.IsFalse(ad.Elements[0].blnAliasLevelIsMultMod);
        }

        [TestMethod]
        public void AddAlias_MultModifier_SetsFlag()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃Lv*2");
            Assert.AreEqual(1, ad.Count);
            Assert.IsFalse(ad.Elements[0].blnAliasLevelIsPlusMod);
            Assert.IsTrue(ad.Elements[0].blnAliasLevelIsMultMod);
        }

        [TestMethod]
        public void AddAlias_NecessarySkill_ParsesSkill()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃 (必要技能)");
            Assert.AreEqual(1, ad.Count);
            Assert.AreEqual("必要技能", ad.Elements[0].strAliasNecessarySkill);
        }

        [TestMethod]
        public void AddAlias_OnlyNecessarySkill_ParsesSkill()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("(必要技能)");
            Assert.AreEqual(1, ad.Count);
            Assert.AreEqual("必要技能", ad.Elements[0].strAliasNecessarySkill);
        }

        [TestMethod]
        public void AddAlias_MultipleAliases_AddsAll()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘");
            ad.AddAlias("射撃");
            ad.AddAlias("防御");
            Assert.AreEqual(3, ad.Count);
        }

        // ──────────────────────────────────────────────
        // HasType
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HasType_ExistingType_ReturnsTrue()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘");
            ad.AddAlias("射撃");
            Assert.IsTrue(ad.HasType("格闘"));
            Assert.IsTrue(ad.HasType("射撃"));
        }

        [TestMethod]
        public void HasType_NonExistingType_ReturnsFalse()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘");
            Assert.IsFalse(ad.HasType("防御"));
        }

        // ──────────────────────────────────────────────
        // ReplaceTypeName
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ReplaceTypeName_WithMatchingData_ReturnsAliasType()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘=物理");
            // "物理" が strAliasData と一致するので "格闘" が返る
            var result = ad.ReplaceTypeName("物理");
            Assert.AreEqual("格闘", result);
        }

        [TestMethod]
        public void ReplaceTypeName_NoMatch_ReturnsFirstType()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘");
            // 一致するデータがない場合は最初のエイリアス種類を返す
            var result = ad.ReplaceTypeName("unknown");
            Assert.AreEqual("格闘", result);
        }

        // ──────────────────────────────────────────────
        // Count プロパティ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Count_InitiallyZero()
        {
            var ad = new AliasDataType { Name = "テスト" };
            Assert.AreEqual(0, ad.Count);
        }

        [TestMethod]
        public void Count_AfterAddAlias_IncreasesCorrectly()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘");
            Assert.AreEqual(1, ad.Count);
            ad.AddAlias("射撃");
            Assert.AreEqual(2, ad.Count);
        }

        [TestMethod]
        public void AddAlias_Elements_NotNull()
        {
            var ad = new AliasDataType { Name = "テスト" };
            Assert.IsNotNull(ad.Elements);
        }

        [TestMethod]
        public void AddAlias_NoModifier_BothFlagsAreFalse()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃Lv3");
            Assert.IsFalse(ad.Elements[0].blnAliasLevelIsPlusMod);
            Assert.IsFalse(ad.Elements[0].blnAliasLevelIsMultMod);
        }

        [TestMethod]
        public void AddAlias_DefaultLevel_IsDefaultLevel()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("攻撃");
            // Level without explicit Lv should be DEFAULT_LEVEL
            Assert.AreEqual(Constants.DEFAULT_LEVEL, ad.Elements[0].dblAliasLevel);
        }

        [TestMethod]
        public void AddAlias_NecessaryConditionEmptyByDefault()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘");
            Assert.AreEqual("", ad.Elements[0].strAliasNecessaryCondition ?? "");
        }

        [TestMethod]
        public void Name_CanBeSetAndRead()
        {
            var ad = new AliasDataType { Name = "エリアス名" };
            Assert.AreEqual("エリアス名", ad.Name);
        }
    }
}
