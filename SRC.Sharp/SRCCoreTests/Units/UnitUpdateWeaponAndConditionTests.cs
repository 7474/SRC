using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Linq;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// 元の SRC の Unit.Update の後半（武器攻撃力の更新〜制御不可・不安定による暴走）との突き合わせ（#1498）
    /// </summary>
    [TestClass]
    public class UnitUpdateWeaponAndConditionTests
    {
        private static SRC CreateSrc()
        {
            return new SRC { GUI = new MockGUI() };
        }

        private static UnitData AddUnitData(SRC src, string name = "テスト機体")
        {
            var ud = src.UDList.Add(name);
            ud.Class = "汎用";
            ud.HP = 1000;
            ud.EN = 100;
            ud.Armor = 500;
            ud.Mobility = 50;
            ud.Speed = 4;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            return ud;
        }

        private static void AddWeapon(UnitData ud, string name, int power, string wclass)
        {
            var wd = ud.AddWeapon(name);
            wd.Power = power;
            wd.Class = wclass;
            wd.MinRange = 1;
            wd.MaxRange = 3;
        }

        private static Unit CreateUnit(SRC src, UnitData ud)
        {
            var unit = src.UList.Add(ud.Name, 0, "味方");
            src.PDList.Add("テストパイロット");
            var pilot = src.PList.Add("テストパイロット", 1, "味方");
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            unit.Update();
            unit.FullRecover();
            return unit;
        }

        private static int Power(Unit unit, string wname)
        {
            return unit.Weapons.First(w => w.Name == wname).WeaponPower("");
        }

        [TestMethod]
        public void WeaponEnhance_WithoutTarget_EnhancesFixedDamageWeapon()
        {
            // VB: 武器指定が無い（flen = 0）ときは flag = True で、固属性の武器も強化する
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "固定", 1000, "射固");
            ud.AddFeature("武器強化Lv1");
            ud.AddFeature("武器割合強化Lv2");
            var unit = CreateUnit(src, ud);

            // 1000 + 100 + 1000 * 2 \ 20
            Assert.AreEqual(1200, Power(unit, "固定"));
        }

        [TestMethod]
        public void WeaponEnhance_FixedDamageWeapon_NegatedOtherClassDoesNotExclude()
        {
            // VB: 固属性の武器は各指定を武器名・表示名・「固」だけで判定する。
            // 「!射」は当てはまらない（found = False）ので除外されず、指定無しと同じくすべての武器を強化する
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "固定", 1000, "射固");
            ud.AddFeature("武器強化Lv1=!射");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(1100, Power(unit, "固定"));
        }

        [TestMethod]
        public void WeaponEnhance_FixedDamageWeapon_NegatedClassAfterFixedDoesNotCancel()
        {
            // VB: 「固 !射」は「固」で当てはまり、「!射」は固属性の武器では当てはまらないので強化する
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "固定", 1000, "射固");
            ud.AddFeature("武器強化Lv1=\"固 !射\"");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(1100, Power(unit, "固定"));
        }

        [TestMethod]
        public void WeaponEnhance_FixedDamageWeapon_NotLimitedTo99999()
        {
            // VB: 固属性の武器は GoTo NextWeapon で、攻撃力の最高値 99999 を適用しない
            var src = CreateSrc();
            var ud = AddUnitData(src);
            AddWeapon(ud, "固定", 99950, "射固");
            AddWeapon(ud, "通常", 99950, "射");
            ud.AddFeature("武器強化Lv1=固定 通常");
            var unit = CreateUnit(src, ud);

            Assert.AreEqual(100050, Power(unit, "固定"));
            Assert.AreEqual(99999, Power(unit, "通常"));
        }

        [TestMethod]
        public void Update_UncontrollableFeatureGained_AddsBerserkWhileSortied()
        {
            // VB: 出撃中に制御不可を得たら暴走を付ける
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));
            unit.Status = "出撃";

            unit.AddCondition("制御不可付加", -1, 1d);
            unit.Update();

            Assert.IsTrue(unit.IsConditionSatisfied("暴走"));
            Assert.AreEqual(-1, unit.ConditionLifetime("暴走"));
        }

        [TestMethod]
        public void Update_UncontrollableFeatureLost_RemovesBerserk()
        {
            // VB: 制御不可を失ったら暴走を外す
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));
            unit.Status = "出撃";
            unit.AddCondition("制御不可付加", -1, 1d);
            unit.Update();

            unit.DeleteCondition0("制御不可付加");
            unit.Update();

            Assert.IsFalse(unit.IsConditionSatisfied("暴走"));
        }

        [TestMethod]
        public void Update_UncontrollableFeatureGained_NotSortied_DoesNothing()
        {
            // VB: 出撃していなければ Exit Sub
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));
            unit.Status = "待機";

            unit.AddCondition("制御不可付加", -1, 1d);
            unit.Update();

            Assert.IsFalse(unit.IsConditionSatisfied("暴走"));
        }

        [TestMethod]
        public void Update_UnstableFeatureGained_AddsBerserkOnlyWhenHPAtMostQuarter()
        {
            // VB: 不安定を得たとき HP <= MaxHP \ 4 なら暴走を付ける
            var src = CreateSrc();
            var unit = CreateUnit(src, AddUnitData(src));
            unit.Status = "出撃";
            unit.HP = 251;

            unit.AddCondition("不安定付加", -1, 1d);
            unit.Update();
            Assert.IsFalse(unit.IsConditionSatisfied("暴走"));

            unit.DeleteCondition0("不安定付加");
            unit.Update();
            unit.HP = 250;
            unit.AddCondition("不安定付加", -1, 1d);
            unit.Update();
            Assert.IsTrue(unit.IsConditionSatisfied("暴走"));

            // 不安定を失ったら暴走を外す
            unit.DeleteCondition0("不安定付加");
            unit.Update();
            Assert.IsFalse(unit.IsConditionSatisfied("暴走"));
        }
    }
}
