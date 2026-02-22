using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Models.Tests
{
    [TestClass]
    public class FeatureDataMoreTests3
    {
        private SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        // ===== FeatureData field assignments =====

        [TestMethod]
        public void FeatureData_Name_CanBeSetAndRead()
        {
            var fd = new FeatureData { Name = "シールド防御" };
            Assert.AreEqual("シールド防御", fd.Name);
        }

        [TestMethod]
        public void FeatureData_Level_CanBeSetAndRead()
        {
            var fd = new FeatureData { Level = 5.0 };
            Assert.AreEqual(5.0, fd.Level);
        }

        [TestMethod]
        public void FeatureData_StrData_CanBeSetAndRead()
        {
            var fd = new FeatureData { StrData = "テストデータ" };
            Assert.AreEqual("テストデータ", fd.StrData);
        }

        [TestMethod]
        public void FeatureData_NecessarySkill_CanBeSetAndRead()
        {
            var fd = new FeatureData { NecessarySkill = "格闘" };
            Assert.AreEqual("格闘", fd.NecessarySkill);
        }

        [TestMethod]
        public void FeatureData_NecessaryCondition_CanBeSetAndRead()
        {
            var fd = new FeatureData { NecessaryCondition = "強化" };
            Assert.AreEqual("強化", fd.NecessaryCondition);
        }

        [TestMethod]
        public void FeatureData_HasLevel_ReturnsFalse_ForDefaultLevel()
        {
            var fd = new FeatureData { Level = Constants.DEFAULT_LEVEL };
            Assert.IsFalse(fd.HasLevel);
        }

        [TestMethod]
        public void FeatureData_HasLevel_ReturnsTrue_ForNonDefaultLevel()
        {
            var fd = new FeatureData { Level = 3.0 };
            Assert.IsTrue(fd.HasLevel);
        }

        [TestMethod]
        public void FeatureData_FeatureLevel_ReturnsOne_WhenNoLevel()
        {
            var fd = new FeatureData { Level = Constants.DEFAULT_LEVEL };
            Assert.AreEqual(1d, fd.FeatureLevel);
        }

        [TestMethod]
        public void FeatureData_FeatureLevel_ReturnsSetLevel_WhenSpecified()
        {
            var fd = new FeatureData { Level = 7.0 };
            Assert.AreEqual(7d, fd.FeatureLevel);
        }

        // ===== IsFeatureAvailable via PilotData =====

        [TestMethod]
        public void PilotData_IsFeatureAvailable_ReturnsFalse_WhenNotAdded()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "テスト" };
            Assert.IsFalse(pd.IsFeatureAvailable("シールド防御"));
        }

        [TestMethod]
        public void PilotData_IsFeatureAvailable_ReturnsTrue_WhenAdded()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddFeature("シールド防御");
            Assert.IsTrue(pd.IsFeatureAvailable("シールド防御"));
        }

        [TestMethod]
        public void PilotData_IsFeatureAvailable_ReturnsFalse_ForOtherFeature()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddFeature("シールド防御");
            Assert.IsFalse(pd.IsFeatureAvailable("バリア"));
        }

        [TestMethod]
        public void PilotData_Features_Count_IncreasesAfterAddFeature()
        {
            var src = CreateSrc();
            var pd = new PilotData(src) { Name = "テスト" };
            pd.AddFeature("シールド防御");
            Assert.AreEqual(1, pd.Features.Count);
        }
    }
}
