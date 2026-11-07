using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Units.cls（元の SRC）と動作を突き合わせたテスト
    /// </summary>
    [TestClass]
    public class UnitsClsVbCompatTests
    {
        private static SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        private static UnitData AddUnitData(SRC src, string name)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            return ud;
        }

        [TestMethod]
        public void Add_PartsCombineSingleName_AddsOtherForm()
        {
            var src = CreateSrc();
            AddUnitData(src, "パーツ機").AddFeature("パーツ合体=合体機");
            AddUnitData(src, "合体機");

            var u = src.UList.Add("パーツ機", 1, "味方");

            Assert.IsNotNull(u);
            Assert.IsTrue(u.IsOtherFormDefined("合体機"));
        }

        [TestMethod]
        public void Add_PartsCombineDataWithSpace_FailsLikeVB()
        {
            // VB: uname2 = ud.FeatureData("パーツ合体") でデータ全体を形態名として探すので、
            // 「合体機 予備」は見つからず、エラーを出して Nothing を返す
            var src = CreateSrc();
            AddUnitData(src, "パーツ機").AddFeature("パーツ合体=合体機 予備");
            AddUnitData(src, "合体機");
            AddUnitData(src, "予備");

            var u = src.UList.Add("パーツ機", 1, "味方");

            Assert.IsNull(u);
        }
    }
}
