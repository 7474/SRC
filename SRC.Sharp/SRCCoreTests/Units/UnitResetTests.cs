using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.Reset（ステージの終了時やデータのロード時に、ユニットの状態を回復する。元の SRC の Unit.Reset）
    /// </summary>
    [TestClass]
    public class UnitResetTests
    {
        private static (SRC src, Unit unit) Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            var ud = src.UDList.Add("テスト機体");
            ud.HP = 1000;
            ud.EN = 100;
            var unit = src.UList.Add("テスト機体", 0, "味方");
            unit.Update();
            unit.FullRecover();
            return (src, unit);
        }

        [TestMethod]
        public void Reset_RestoresHPAndEN()
        {
            var (_, unit) = Create();
            unit.HP = 50;
            unit.EN = 10;

            unit.Reset();

            Assert.AreEqual(1000, unit.HP);
            Assert.AreEqual(100, unit.EN);
        }

        [TestMethod]
        public void Reset_ClearsConditionsAndMode()
        {
            var (_, unit) = Create();
            unit.AddCondition("行動不能", 1, 0d, "");
            unit.Mode = "固定";

            unit.Reset();

            Assert.AreEqual(0, unit.CountCondition());
            Assert.AreEqual("通常", unit.Mode);
        }

        [TestMethod]
        public void UnitsUpdate_RestoresUnitsForNextStage()
        {
            // ステージの終了時（Units.Update）に、ダメージを受けた味方ユニットは全快する
            var (src, unit) = Create();
            unit.HP = 1;

            src.UList.Update();

            Assert.AreEqual(1000, unit.HP);
        }
    }
}
