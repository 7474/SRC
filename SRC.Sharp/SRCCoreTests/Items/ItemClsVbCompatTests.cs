using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Items.Tests
{
    /// <summary>
    /// Item.cls・ItemData.cls（元の SRC）と動作を突き合わせたテスト
    /// </summary>
    [TestClass]
    public class ItemClsVbCompatTests
    {
        [TestMethod]
        public void FeatureNecessarySkill_UndefinedFeature_ReturnsEmpty()
        {
            // VB: ItemData.FeatureNecessarySkill は On Error で "" を返す
            var src = new SRC { GUI = new MockGUI() };
            var id = src.IDList.Add("テスト装備");
            id.AddFeature("バリア=テスト 1000 10");
            var item = src.IList.Add("テスト装備");

            Assert.AreEqual("", item.FeatureNecessarySkill("無い能力"));
        }
    }
}
