using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    /// <summary>
    /// UnitData の追加ユニットテスト
    /// </summary>
    [TestClass]
    public class UnitDataMoreTests
    {
        private SRC CreateSRC()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ──────────────────────────────────────────────
        // PilotNum / Transportation 境界値
        // ──────────────────────────────────────────────

        [TestMethod]
        public void PilotNum_Zero_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { PilotNum = 0 };
            Assert.AreEqual(0, ud.PilotNum);
        }

        [TestMethod]
        public void PilotNum_LargeValue_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { PilotNum = 10 };
            Assert.AreEqual(10, ud.PilotNum);
        }

        [TestMethod]
        public void Transportation_EmptyString_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Transportation = "" };
            Assert.AreEqual("", ud.Transportation);
        }

        [TestMethod]
        public void Transportation_MultipleValues_CanBeSetAndRead()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Transportation = "空陸" };
            Assert.AreEqual("空陸", ud.Transportation);
        }

        // ──────────────────────────────────────────────
        // FeatureLevel int インデックス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureLevel_ByIntIndex_ReturnsLevel()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("格闘強化Lv3");
            Assert.AreEqual(3d, ud.FeatureLevel(1));
        }

        [TestMethod]
        public void FeatureLevel_ByIntIndex_DefaultLevel_ReturnsOne()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("ビーム無効");
            Assert.AreEqual(1d, ud.FeatureLevel(1));
        }

        // ──────────────────────────────────────────────
        // FeatureData int インデックス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureData_ByIntIndex_ReturnsData()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("特殊=カスタムデータ");
            Assert.AreEqual("カスタムデータ", ud.FeatureData(1));
        }

        [TestMethod]
        public void FeatureData_ByIntIndex_NoData_ReturnsEmpty()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("ビーム無効");
            Assert.AreEqual("", ud.FeatureData(1));
        }

        // ──────────────────────────────────────────────
        // FeatureName int インデックス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureName_ByIntIndex_ReturnsName()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("底力");
            Assert.AreEqual("底力", ud.FeatureName(1));
        }

        [TestMethod]
        public void FeatureName_ByIntIndex_WithLevel_ReturnsNameWithLv()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("射撃強化Lv2");
            Assert.AreEqual("射撃強化Lv2", ud.FeatureName(1));
        }

        [TestMethod]
        public void FeatureName_ByIntIndex_SecondFeature_ReturnsSecondName()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("底力");
            ud.AddFeature("援護攻撃");
            Assert.AreEqual("援護攻撃", ud.FeatureName(2));
        }

        // ──────────────────────────────────────────────
        // Feature int インデックス
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Feature_ByIntIndex_ReturnsFeatureDataWithCorrectName()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("援護防御");
            var fd = ud.Feature(1);
            Assert.IsNotNull(fd);
            Assert.AreEqual("援護防御", fd.Name);
        }

        [TestMethod]
        public void Feature_ByIntIndex_ReturnsCorrectFeatureByOrder()
        {
            var src = CreateSRC();
            var ud = new UnitData(src) { Name = "テスト" };
            ud.AddFeature("底力");
            ud.AddFeature("援護攻撃");
            var second = ud.Feature(2);
            Assert.IsNotNull(second);
            Assert.AreEqual("援護攻撃", second.Name);
        }
    }
}
