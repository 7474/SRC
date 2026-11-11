using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.VB;
using System.Reflection;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.cls の特殊能力の参照の関数（FeatureName・FeatureLevel・FeatureData・IsFeatureActivated）を
    /// 元の SRC の動作と突き合わせるテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class UnitFeatureVbCompatTests
    {
        private static Unit CreateUnit()
        {
            return new Unit(new SRC { GUI = new MockGUI() });
        }

        private static SrcCollection<FeatureData> Col(Unit unit, string name)
        {
            var field = typeof(Unit).GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            return (SrcCollection<FeatureData>)field.GetValue(unit);
        }

        private static FeatureData Fd(string name, double level = Constants.DEFAULT_LEVEL, string data = "")
        {
            return new FeatureData { Name = name, Level = level, StrData = data };
        }

        [TestMethod]
        public void FeatureName_NotFound_ReturnsIndex()
        {
            // VB: ErrorHandler で FeatureNameInt = CStr(Index)
            var unit = CreateUnit();
            Assert.AreEqual("オーラバリア", unit.FeatureName("オーラバリア"));
            Assert.AreEqual("3", unit.FeatureName(3));
        }

        [TestMethod]
        public void FeatureName0_NotFound_ReturnsIndexWithoutLevel()
        {
            // VB: FeatureName0 = FeatureName(Index) から「Lv」以降を除く
            var unit = CreateUnit();
            Assert.AreEqual("オーラバリア", unit.FeatureName0("オーラバリア"));
            Assert.AreEqual("バリア", unit.FeatureName0("バリアLv2"));
        }

        [TestMethod]
        public void FeatureName0_Found_StripsLevel()
        {
            var unit = CreateUnit();
            Col(unit, "colFeature").Add(Fd("バリア", 2d, "オーラバリア"), "バリア");
            Col(unit, "colFeature").Add(Fd("底力", 3d), "底力");
            Assert.AreEqual("オーラバリア", unit.FeatureName0("バリア"));
            Assert.AreEqual("底力Lv3", unit.FeatureName("底力"));
            Assert.AreEqual("底力", unit.FeatureName0("底力"));
        }

        [TestMethod]
        public void FeatureData_IntIndexOutOfRange_ReturnsEmpty()
        {
            // VB: ErrorHandler で FeatureData = ""・FeatureLevel = 0
            var unit = CreateUnit();
            Assert.AreEqual("", unit.FeatureData(1));
            Assert.AreEqual(0d, unit.FeatureLevel(0));
        }

        [TestMethod]
        public void IsFeatureActivated_FeatureNotSatisfyingCondition_ReturnsFalse()
        {
            // VB: colAllFeature の fd が colFeature に同じインスタンスとしてあれば True
            var unit = CreateUnit();
            var active = Fd("バリア");
            var inactive = Fd("分身");
            Col(unit, "colAllFeature").Add(active, "バリア");
            Col(unit, "colAllFeature").Add(inactive, "分身");
            Col(unit, "colFeature").Add(active, "バリア");

            Assert.IsTrue(unit.IsFeatureActivated(unit.AllFeature(1)));
            Assert.IsFalse(unit.IsFeatureActivated(unit.AllFeature(2)));
            // 同じ内容の別のインスタンスは同じものとみなさない
            Assert.IsFalse(unit.IsFeatureActivated(Fd("バリア")));
            Assert.IsFalse(unit.IsFeatureActivated(null));
        }
    }
}
