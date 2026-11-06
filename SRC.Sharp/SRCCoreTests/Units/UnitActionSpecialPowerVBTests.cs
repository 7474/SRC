using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests.SpecialPower;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// Unit.cls の行動数（Action・UseAction）とスペシャルパワーの手続きを、VB 版の動作に合わせたテスト（#1498）。
    /// </summary>
    [TestClass]
    public class UnitActionSpecialPowerVBTests
    {
        [TestMethod]
        public void UseAction_ENが0でも行動数を消費する()
        {
            // VB の UseAction は MaxAction を使わず、特殊状態とＥＮを見ずに最大行動数を決める
            var f = new SpecialPowerTestFixture();
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.EN = 0;
            Assert.AreEqual(0, u.MaxAction());

            u.UseAction();
            Assert.AreEqual(1, u.UsedAction);

            // ＥＮが戻っても、すでに行動したので行動数は残らない
            u.EN = 100;
            Assert.AreEqual(0, u.Action);
        }

        [TestMethod]
        public void UseAction_麻痺でも行動数を消費する()
        {
            var f = new SpecialPowerTestFixture();
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.AddCondition("麻痺", 1);

            u.UseAction();
            Assert.AreEqual(1, u.UsedAction);
        }

        [TestMethod]
        public void Action_行動できないときはUsedActionが負でも0()
        {
            // VB: If MaxAction > 0 Then ... Else Action = 0
            var f = new SpecialPowerTestFixture();
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.UsedAction = -1;
            Assert.AreEqual(2, u.Action);

            u.AddCondition("麻痺", 1);
            Assert.AreEqual(0, u.Action);
        }

        [TestMethod]
        public void IsSpecialPowerInEffect_Collectionのキーと同じくひらがなとカタカナを区別しない()
        {
            // VB は colSpecialPowerInEffect.Item(sname) で探すので、Collection のキーの比較になる
            var f = new SpecialPowerTestFixture();
            f.DefineSP("ひらめき", "自分", "1回", "回避");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.MakeSpecialPowerInEffect("ひらめき");

            Assert.IsTrue(u.IsSpecialPowerInEffect("ヒラメキ"));

            // すでにかかっているので何もしない（キーの重複で例外にならない）
            u.MakeSpecialPowerInEffect("ヒラメキ");
            Assert.AreEqual(1, u.CountSpecialPower());
        }

        [TestMethod]
        public void SpecialPowerEffectLevel_ひとつのスペシャルパワーでは最初の効果だけを見る()
        {
            // VB: 該当する効果を見つけたら Exit For する
            var f = new SpecialPowerTestFixture();
            f.DefineSP("熱血", "自分", "攻撃", "ダメージ増加Lv1 ダメージ増加Lv3");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.MakeSpecialPowerInEffect("熱血");

            Assert.AreEqual(1d, u.SpecialPowerEffectLevel("ダメージ増加"));
        }

        [TestMethod]
        public void SpecialPowerEffectLevel_複数のスペシャルパワーでは最も高いレベル()
        {
            var f = new SpecialPowerTestFixture();
            f.DefineSP("熱血", "自分", "攻撃", "ダメージ増加Lv1");
            f.DefineSP("魂", "自分", "攻撃", "ダメージ増加Lv2");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.MakeSpecialPowerInEffect("熱血");
            u.MakeSpecialPowerInEffect("魂");

            Assert.AreEqual(2d, u.SpecialPowerEffectLevel("ダメージ増加"));
            Assert.AreEqual(0d, u.SpecialPowerEffectLevel("命中強化"));
        }

        [TestMethod]
        public void RemoveSpecialPowerInEffect2_範囲外の番号は無視する()
        {
            // VB: On Error GoTo ErrorHandler で無視する
            var f = new SpecialPowerTestFixture();
            f.DefineSP("熱血", "自分", "攻撃", "ダメージ増加Lv1");
            var u = f.AddUnit("機体", "パイロット", "味方", 1, 1);
            u.MakeSpecialPowerInEffect("熱血");

            u.RemoveSpecialPowerInEffect2(2);
            u.RemoveSpecialPowerInEffect2(0);
            Assert.AreEqual(1, u.CountSpecialPower());

            u.RemoveSpecialPowerInEffect2(1);
            Assert.AreEqual(0, u.CountSpecialPower());
        }
    }
}
