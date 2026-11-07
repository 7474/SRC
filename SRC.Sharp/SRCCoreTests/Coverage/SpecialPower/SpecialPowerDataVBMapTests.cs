using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Exceptions;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests.SpecialPower
{
    /// <summary>
    /// SpecialPowerData・SpecialPowerDataList を元の SRC（SpecialPowerData.cls・SpecialPowerDataList.cls）と
    /// 分岐ごとに突き合わせたテスト（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class SpecialPowerDataVBMapTests
    {
        [TestMethod]
        public void SetEffect_CalledTwice_ReplacesEffects()
        {
            // VB の SetEffect は ReDim で配列を作り直す
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("熱血", "自分", "攻撃", "ダメージ増加Lv1 絶対命中");
            sd.SetEffect("ダメージ増加Lv2");
            Assert.AreEqual(1, sd.CountEffect());
            Assert.AreEqual(2d, sd.Effects[0].dblEffectLevel);
        }

        [TestMethod]
        public void SetEffect_NameOnly_EffectDataIsEmpty()
        {
            // VB の String 配列の既定値は ""
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("必中", "自分", "ターン", "絶対命中");
            Assert.AreEqual("", sd.Effects[0].strEffectData);
        }

        [TestMethod]
        public void StatusRecovery_ConditionNamedExactlyAttributeDisabled_IsNotTarget()
        {
            // VB は Len(条件名) > 6 のときだけ「属性使用不能」で終わるかを見る
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("修理", "自分", "即効", "状態回復");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.AddCondition("属性使用不能", 3);

            Assert.IsFalse(sd.Effective(u.MainPilot(), u));
            sd.Apply(u.MainPilot(), u, is_event: true);
            Assert.IsTrue(u.IsConditionSatisfied("属性使用不能"));

            u.AddCondition("火属性使用不能", 3);
            Assert.IsTrue(sd.Effective(u.MainPilot(), u));
            sd.Apply(u.MainPilot(), u, is_event: true);
            Assert.IsFalse(u.IsConditionSatisfied("火属性使用不能"));
            Assert.IsTrue(u.IsConditionSatisfied("属性使用不能"));
        }

        [TestMethod]
        public void Execute_AllAllies_SleepsOnceAfterLoop()
        {
            // VB の「全味方」はマップ全体に適用したあとで一度だけ Sleep 300 する
            var f = new SpecialPowerTestFixture();
            var sleeps = 0;
            f.GUI.SleepHandler = (ms, e) => { if (ms == 300) { sleeps++; } };
            var sd = f.DefineSP("激励", "全味方", "即効", "気力増加Lv1");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);

            sd.Execute(u.MainPilot());

            Assert.AreEqual(1, sleeps);
        }

        [TestMethod]
        public void SPRecovery_SupportAtMaxSP_NoMessageForSupport()
        {
            // VB はサブ・サポートパイロットのＳＰが変わったときだけメッセージを出す
            var f = new SpecialPowerTestFixture();
            var sd = f.DefineSP("補給", "自分", "即効", "ＳＰ回復Lv5");
            var u = f.AddUnit("機体", "メイン", "味方", 1, 1, level: 30);
            f.DefinePilot("サポート");
            var sp = f.Src.PList.Add("サポート", 30, "味方");
            u.AddSupport(sp);
            sp.Unit = u;
            var main = u.MainPilot();
            main.SP = 0;

            sd.Apply(main, u, is_event: false);

            Assert.IsTrue(main.SP > 0);
            Assert.AreEqual(1, f.SysMessages.Count(x => x.EndsWith("回復した。")), string.Join("/", f.SysMessages));
            Assert.IsTrue(f.SysMessages.Any(x => x.StartsWith(main.get_Nickname(false))));
        }

        [TestMethod]
        public void Load_DataError_ReportsDataName()
        {
            // VB の DataErrorMessage には data_name（行の内容ではない）を渡す
            var f = new SpecialPowerTestFixture();
            var data = "熱血, ねっけつ\n熱血だ\nダメージ増加Lv1\n解説\n";
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(data)))
            {
                var ex = Assert.ThrowsExactly<InvalidSrcDataException>(() => f.Src.SPDList.Load("sp.txt", stream));
                Assert.AreEqual("熱血", ex.InvalidDataList.First().dname);
            }
        }
    }
}
