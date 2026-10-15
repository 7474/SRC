using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SRCCore.Coverage.Tests.SpecialPower
{
    /// <summary>
    /// 復活効果 (SpecialPowerData.Apply の「復活」) のテスト。
    /// 元の SRC の SpecialPowerData.Apply の Case "復活" に対応。
    /// </summary>
    [TestClass]
    public class SpecialPowerReviveTests
    {
        private static Units.Unit Destroy(SpecialPowerTestFixture f, Units.Unit u)
        {
            f.Src.Map.MapDataForUnit[u.x, u.y] = null;
            u.HP = 0;
            u.Status = "破壊";
            return u;
        }

        [TestMethod]
        public void Revive_DestroyedAlly_FullyRecoversNextToUser()
        {
            // 破壊された味方を全回復させ、使用者の近くに出撃させる
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("祈り", "破壊味方", "即効", "復活");
            var me = f.AddUnit("自機", "自分", "味方", 5, 5);
            var dead = Destroy(f, f.AddUnit("僚機", "僚友", "味方", 9, 9));
            dead.UseAction();

            Assert.IsTrue(sd.Effective(me.MainPilot(), dead));
            sd.Apply(me.MainPilot(), dead, is_event: true);

            Assert.AreEqual("出撃", dead.Status);
            Assert.AreEqual(dead.MaxHP, dead.HP);
            Assert.AreEqual(1, dead.Action, "復活したユニットは行動可能");
            Assert.AreEqual(1, System.Math.Abs(dead.x - me.x) + System.Math.Abs(dead.y - me.y));
            Assert.AreSame(dead, f.Src.Map.MapDataForUnit[dead.x, dead.y]);
        }

        [TestMethod]
        public void Revive_OnDestroyDuration_AsInstant_RestoresHP()
        {
            // 効果時間「破壊」の復活は破壊直後にＨＰを全快させる
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("ド根性", "自分", "破壊", "復活");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.HP = 0;

            sd.Apply(u.MainPilot(), u, is_event: true);
            Assert.IsTrue(u.IsSpecialPowerInEffect("ド根性"), "通常は持続効果として付加されるだけ");
            Assert.AreEqual(0, u.HP);

            sd.Apply(u.MainPilot(), u, is_event: true, as_instant: true);
            Assert.AreEqual(u.MaxHP, u.HP);
        }

        [TestMethod]
        public void Revive_HyperModeUnit_ReturnsToNormalFormEvenWithOptions()
        {
            // VB: .Transform LIndex(.FeatureData("ノーマルモード"), 1)
            // 「ノーマルモード=通常形 消耗あり」のように付帯指定があっても通常形態に戻って復活する
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("祈り", "破壊味方", "即効", "復活");
            var normal = f.Src.UDList.Add("通常形");
            normal.HP = 1000;
            normal.EN = 100;
            normal.Transportation = "陸";
            normal.Adaption = "AAAA";
            f.Src.UDList.Add("ハイパー形").AddFeature("ノーマルモード=通常形 消耗あり");
            var me = f.AddUnit("自機", "自分", "味方", 5, 5);
            var dead = Destroy(f, f.AddUnit("ハイパー形", "僚友", "味方", 9, 9));

            sd.Apply(me.MainPilot(), dead, is_event: true);

            var revived = dead.CurrentForm();
            Assert.AreEqual("通常形", revived.Name);
            Assert.AreEqual("出撃", revived.Status);
            Assert.AreEqual(revived.MaxHP, revived.HP);
        }
    }
}
