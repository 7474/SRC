using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Linq;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// WeaponData / PilotData / ItemData の追加エッジケーステスト
    /// </summary>
    [TestClass]
    public class ModelDataAdditionalTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // WeaponData 基本フィールドテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void WeaponData_Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Name = "ビームライフル" };
            Assert.AreEqual("ビームライフル", wd.Name);
        }

        [TestMethod]
        public void WeaponData_Range_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src)
            {
                MinRange = 2,
                MaxRange = 5
            };
            Assert.AreEqual(2, wd.MinRange);
            Assert.AreEqual(5, wd.MaxRange);
        }

        [TestMethod]
        public void WeaponData_Power_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Power = 3000 };
            Assert.AreEqual(3000, wd.Power);
        }

        [TestMethod]
        public void WeaponData_Bullet_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { Bullet = 10 };
            Assert.AreEqual(10, wd.Bullet);
        }

        [TestMethod]
        public void WeaponData_ENConsumption_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var wd = new WeaponData(src) { ENConsumption = 50 };
            Assert.AreEqual(50, wd.ENConsumption);
        }

        // ──────────────────────────────────────────────
        // ItemData 基本フィールドテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ItemData_Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var id = new ItemData(src) { Name = "ビームライフル" };
            Assert.AreEqual("ビームライフル", id.Name);
        }

        [TestMethod]
        public void ItemData_Class_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var id = new ItemData(src) { Class = "武器" };
            Assert.AreEqual("武器", id.Class);
        }

        // ──────────────────────────────────────────────
        // PilotData 基本フィールドテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotData_Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var pd = new PilotData(src) { Name = "アムロ・レイ" };
            Assert.AreEqual("アムロ・レイ", pd.Name);
        }

        [TestMethod]
        public void PilotData_DefaultValues_NameIsNull()
        {
            var src = CreateSRC();
            var pd = new PilotData(src);
            Assert.IsNull(pd.Name);
        }

        // ──────────────────────────────────────────────
        // FeatureData 基本フィールドテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureData_Name_CanBeSetAndRead()
        {
            var fd = new FeatureData { Name = "シールド防御" };
            Assert.AreEqual("シールド防御", fd.Name);
        }

        [TestMethod]
        public void FeatureData_DefaultValues_AreExpected()
        {
            var fd = new FeatureData();
            Assert.IsNull(fd.Name);
        }

        // ──────────────────────────────────────────────
        // UnitData 基本フィールドテスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitData_Name_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "ガンダム" };
            Assert.AreEqual("ガンダム", ud.Name);
        }

        [TestMethod]
        public void UnitData_DefaultValues_AreExpected()
        {
            var src = CreateSRC();
            var ud = new UnitData(src);
            Assert.IsNull(ud.Name);
        }

        // ──────────────────────────────────────────────
        // DataList でユニットデータが追加・参照できるか
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitDataList_Add_ItemIsAccessible()
        {
            var src = CreateSRC();
            var list = src.UDList;
            list.Add("ガンダム");
            var item = list.Item("ガンダム");
            Assert.IsNotNull(item);
            Assert.AreEqual("ガンダム", item.Name);
        }

        [TestMethod]
        public void PilotDataList_Add_ItemIsAccessible()
        {
            var src = CreateSRC();
            var list = src.PDList;
            list.Add("アムロ");
            var item = list.Item("アムロ");
            Assert.IsNotNull(item);
            Assert.AreEqual("アムロ", item.Name);
        }

        [TestMethod]
        public void ItemDataList_Add_ItemIsAccessible()
        {
            var src = CreateSRC();
            var list = src.IDList;
            list.Add("ビームサーベル");
            var item = list.Item("ビームサーベル");
            Assert.IsNotNull(item);
            Assert.AreEqual("ビームサーベル", item.Name);
        }

        // ──────────────────────────────────────────────
        // 複数追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitDataList_MultipleAdds_CountIsCorrect()
        {
            var src = CreateSRC();
            var list = src.UDList;
            var initial = list.Count();
            list.Add("ガンダム");
            list.Add("ザク");
            list.Add("シャアザク");
            Assert.AreEqual(initial + 3, list.Count());
        }

        [TestMethod]
        public void UnitDataList_IsDefinedAfterAdd_ReturnsTrue()
        {
            var src = CreateSRC();
            var list = src.UDList;
            list.Add("ドム");
            Assert.IsTrue(list.IsDefined("ドム"));
        }

        [TestMethod]
        public void UnitDataList_IsDefinedForMissing_ReturnsFalse()
        {
            var src = CreateSRC();
            var list = src.UDList;
            Assert.IsFalse(list.IsDefined("存在しないユニット_テスト"));
        }
    }
}
