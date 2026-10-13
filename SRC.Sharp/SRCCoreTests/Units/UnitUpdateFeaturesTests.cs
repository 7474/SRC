using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.VB;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.UpdateFeatures（必要技能を満たさない特殊能力を外し、残りを登録しなおす）
    /// </summary>
    [TestClass]
    public class UnitUpdateFeaturesTests
    {
        [TestMethod]
        public void ReRegister_ManyFeaturesWithSameName_DoesNotThrow()
        {
            // 同じ名前の特殊能力が 3 つ以上あっても、登録しなおしでキーが重複しない（ロボダンの「パイロット能力付加」）
            var src = new SRC { GUI = new MockGUI() };
            src.UDList.Add("テスト機体");
            var unit = src.UList.Add("テスト機体", 0, "味方");
            var colFeature = (SrcCollection<FeatureData>)typeof(Unit)
                .GetField("colFeature", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(unit);
            colFeature.Clear();
            colFeature.Add(new FeatureData { Name = "パイロット能力付加", StrData = "A" }, "パイロット能力付加");
            colFeature.Add(new FeatureData { Name = "パイロット能力付加", StrData = "B" }, "パイロット能力付加:2");
            colFeature.Add(new FeatureData { Name = "パイロット能力付加", StrData = "C" }, "パイロット能力付加:3");
            colFeature.Add(new FeatureData { Name = "パイロット能力付加", StrData = "D" }, "パイロット能力付加:4");
            // 必要技能を満たさないため外される（登録しなおしが起きる）
            colFeature.Add(new FeatureData { Name = "テスト能力", NecessarySkill = "存在しない技能" }, "テスト能力");

            typeof(Unit).GetMethod("UpdateFeatures", BindingFlags.NonPublic | BindingFlags.Instance)
                .Invoke(unit, new object[] { "" });

            Assert.AreEqual(4, colFeature.Count);
            Assert.IsFalse(colFeature.ContainsKey("テスト能力"));
        }
    }
}
