using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.TestLib;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.Update でのアイテムによる能力値の修正（元の SRC の Unit.Update「アイテムによる修正」）
    /// </summary>
    [TestClass]
    public class UnitItemModifierTests
    {
        private static (SRC src, Unit unit) Create()
        {
            var src = new SRC { GUI = new MockGUI() };
            var ud = src.UDList.Add("テスト機体");
            ud.HP = 1000;
            ud.EN = 100;
            ud.Armor = 30;
            ud.Mobility = 60;
            ud.Speed = 4;
            var unit = src.UList.Add("テスト機体", 0, "味方");
            unit.Update();
            unit.FullRecover();
            return (src, unit);
        }

        [TestMethod]
        public void EquippedItem_AddsParameters()
        {
            // ロボダンは装甲が 100 未満の敵に「装甲１００」（装甲 +100）を装備させ、100 以上になるまで繰り返す
            var (src, unit) = Create();
            var data = src.IDList.Add("装甲１００");
            data.Class = "固定";
            data.Part = "非表示";
            data.HP = 500;
            data.EN = 20;
            data.Armor = 100;
            data.Mobility = 5;
            data.Speed = 1;

            unit.AddItem(src.IList.Add("装甲１００"));

            Assert.AreEqual(130, unit.get_Armor(""));
            Assert.AreEqual(1500, unit.MaxHP);
            Assert.AreEqual(120, unit.MaxEN);
            Assert.AreEqual(65, unit.get_Mobility(""));
            Assert.AreEqual(5, unit.Speed);
        }

        [TestMethod]
        public void Update_KeepsHPRatio()
        {
            // 最大値が変わらなければ、Update を繰り返しても HP は減らない（比率から戻す際の誤差で 1 ずつ減らない）
            var (_, unit) = Create();
            unit.HP = 7;
            for (var i = 0; i < 10; i++)
            {
                unit.Update();
            }
            Assert.AreEqual(7, unit.HP);
        }
    }
}
