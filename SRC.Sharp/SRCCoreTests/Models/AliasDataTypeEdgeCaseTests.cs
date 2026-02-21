using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// AliasDataType クラスの追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class AliasDataTypeEdgeCaseTests
    {
        // ──────────────────────────────────────────────
        // AliasDataType 基本テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AliasDataType_DefaultName_IsNull()
        {
            var ad = new AliasDataType();
            Assert.IsNull(ad.Name);
        }

        [TestMethod]
        public void AliasDataType_Name_CanBeSetAndRead()
        {
            var ad = new AliasDataType { Name = "エイリアス1" };
            Assert.AreEqual("エイリアス1", ad.Name);
        }

        [TestMethod]
        public void AliasDataType_Elements_DefaultIsEmpty()
        {
            var ad = new AliasDataType { Name = "テスト" };
            Assert.IsNotNull(ad.Elements);
            Assert.AreEqual(0, ad.Elements.Count);
        }

        // ──────────────────────────────────────────────
        // AddAlias - 各種フォーマット
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AddAlias_SimpleType_AddsElement()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘");
            Assert.AreEqual(1, ad.Elements.Count);
            Assert.AreEqual("格闘", ad.Elements[0].strAliasType);
        }

        [TestMethod]
        public void AddAlias_WithLevel_AddsElementWithLevel()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("射撃Lv3");
            Assert.AreEqual(1, ad.Elements.Count);
            Assert.AreEqual("射撃", ad.Elements[0].strAliasType);
            Assert.AreEqual(3d, ad.Elements[0].dblAliasLevel);
        }

        [TestMethod]
        public void AddAlias_MultipleAlias_AddsMultipleElements()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘Lv2");
            ad.AddAlias("射撃Lv3");
            ad.AddAlias("防御Lv1");
            Assert.AreEqual(3, ad.Elements.Count);
        }

        [TestMethod]
        public void AddAlias_WithData_AddsElementWithData()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("属性=火炎");
            Assert.AreEqual(1, ad.Elements.Count);
        }

        [TestMethod]
        public void AddAlias_PlusModLevel_IsPlusMod()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘Lv+3");
            Assert.IsTrue(ad.Elements[0].blnAliasLevelIsPlusMod);
        }

        [TestMethod]
        public void AddAlias_MultModLevel_IsMultMod()
        {
            var ad = new AliasDataType { Name = "テスト" };
            ad.AddAlias("格闘Lv*2");
            Assert.IsTrue(ad.Elements[0].blnAliasLevelIsMultMod);
        }

        // ──────────────────────────────────────────────
        // AliasDataElement 詳細フィールドテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AliasDataElement_AllFieldsNull_WhenCreatedEmpty()
        {
            var elem = new AliasDataElement();
            Assert.IsNull(elem.strAliasType);
            Assert.IsNull(elem.strAliasData);
            Assert.IsNull(elem.strAliasNecessarySkill);
            Assert.IsNull(elem.strAliasNecessaryCondition);
        }

        [TestMethod]
        public void AliasDataElement_LargeLevel_CanBeSet()
        {
            var elem = new AliasDataElement { dblAliasLevel = 9999.9 };
            Assert.AreEqual(9999.9, elem.dblAliasLevel);
        }

        [TestMethod]
        public void AliasDataElement_JapaneseType_CanBeSet()
        {
            var elem = new AliasDataElement { strAliasType = "特殊回避" };
            Assert.AreEqual("特殊回避", elem.strAliasType);
        }

        [TestMethod]
        public void AliasDataElement_BothMods_CanBothBeTrue()
        {
            // 実際の使用では片方だけが true になるが、フィールド設定自体は可能
            var elem = new AliasDataElement
            {
                blnAliasLevelIsPlusMod = true,
                blnAliasLevelIsMultMod = true
            };
            Assert.IsTrue(elem.blnAliasLevelIsPlusMod);
            Assert.IsTrue(elem.blnAliasLevelIsMultMod);
        }
    }
}
