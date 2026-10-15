using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// COM.TryFix（修理装置の使用）のテスト。
    /// VB 版 COM.bas の TryFix に対応する。
    /// </summary>
    [TestClass]
    public class COMTryFixTests
    {
        private static COMTestField CreateField()
        {
            var f = new COMTestField();
            var ud = f.DefineUnit("修理機", speed: 4);
            ud.AddFeature("修理装置");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            COMTestField.AddWeapon(f.DefineUnit("機体"), "パンチ", 1200);
            return f;
        }

        [TestMethod]
        public void TryFix_WithoutRepairFeature_ReturnsFalse()
        {
            var f = CreateField();
            var u = f.Deploy("機体", "敵A", "敵", 3, 3);
            var t = f.Deploy("機体", "敵B", "敵", 4, 3);
            t.HP = 100;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryFix(true));
            Assert.AreEqual(100, t.HP);
        }

        [TestMethod]
        public void TryFix_AdjacentDamagedAlly_RecoversThirtyPercentAndReports()
        {
            // 修理装置(Lv指定なし)は最大ＨＰの 30% を回復させる
            var f = CreateField();
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var t = f.Deploy("機体", "敵B", "敵", 4, 3);
            t.HP = 1000;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryFix(true));

            Assert.AreEqual(1900, t.HP);
            Assert.AreSame(t, f.Src.Commands.SelectedTarget);
            var messages = f.GUI.CallsOf(nameof(IGUI.DisplaySysMessage)).Select(x => (string)x.Args[0]).ToList();
            CollectionAssert.Contains(messages, "修理機は[機体]に[修理装置]を使った。");
            CollectionAssert.Contains(messages, "機体のＨＰが[900]回復した。");
        }

        [TestMethod]
        public void TryFix_Level2_RecoversFiftyPercent()
        {
            var f = new COMTestField();
            var ud = f.DefineUnit("修理機");
            ud.AddFeature("修理装置Lv2");
            f.DefineUnit("機体");
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var t = f.Deploy("機体", "敵B", "敵", 3, 4);
            t.HP = 1000;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryFix(true));
            Assert.AreEqual(2500, t.HP);
        }

        [TestMethod]
        public void TryFix_SelectsMostDamagedAlly()
        {
            // ＨＰの割合が最も低い味方を修理する
            var f = CreateField();
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var light = f.Deploy("機体", "敵B", "敵", 4, 3);
            var heavy = f.Deploy("機体", "敵C", "敵", 2, 3);
            light.HP = 2000;
            heavy.HP = 600;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryFix(true));
            Assert.AreSame(heavy, f.Src.Commands.SelectedTarget);
            Assert.AreEqual(1500, heavy.HP);
            Assert.AreEqual(2000, light.HP);
        }

        [TestMethod]
        public void TryFix_SlightlyDamagedOrEnemyUnits_AreNotRepaired()
        {
            // ＨＰが 90% を超える味方と、敵対するユニットは修理しない
            var f = CreateField();
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var ally = f.Deploy("機体", "敵B", "敵", 4, 3);
            var enemy = f.Deploy("機体", "味方A", "味方", 2, 3);
            ally.HP = 2800;
            enemy.HP = 100;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryFix(true));
            Assert.AreEqual(2800, ally.HP);
            Assert.AreEqual(100, enemy.HP);
        }

        [TestMethod]
        public void TryFix_AfterMove_OnlyAdjacentUnitsAreReachable()
        {
            // 移動後は隣接するユニットしか修理できない
            var f = CreateField();
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var t = f.Deploy("機体", "敵B", "敵", 5, 3);
            t.HP = 100;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryFix(true));
            Assert.AreEqual(100, t.HP);
        }

        [TestMethod]
        public void TryFix_BeforeMove_MovesNextToTargetThenRepairs()
        {
            // 移動前なら移動範囲内で修理対象に隣接する地点へ移動してから修理する
            var f = CreateField();
            var u = f.Deploy("修理機", "敵A", "敵", 1, 1);
            var t = f.Deploy("機体", "敵B", "敵", 1, 5);
            t.HP = 1000;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryFix(false));

            Assert.AreEqual(1, u.x);
            Assert.AreEqual(4, u.y);
            Assert.AreSame(u, f.Src.Map.MapDataForUnit[1, 4]);
            Assert.AreEqual(1900, t.HP);
        }

        [TestMethod]
        public void TryFix_WithDefaultTarget_IgnoresOtherDamagedUnits()
        {
            // 修理対象が指定されている場合はそのユニット以外は修理しない
            var f = CreateField();
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var other = f.Deploy("機体", "敵B", "敵", 4, 3);
            var guarded = f.Deploy("機体", "敵C", "敵", 2, 3);
            other.HP = 100;
            guarded.HP = 2000;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryFix(true, guarded));
            Assert.AreEqual(100, other.HP);
            Assert.AreEqual(2900, guarded.HP);
        }

        [TestMethod]
        public void TryFix_UnrepairableByThisDevice_IsSkipped()
        {
            // 修理不可で名指しされた修理装置では修理できない
            var f = CreateField();
            f.DefineUnit("修理不可機").AddFeature("修理不可=修理不可 修理装置");
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var t = f.Deploy("修理不可機", "敵B", "敵", 4, 3);
            t.HP = 100;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryFix(true));
            Assert.AreEqual(100, t.HP);
        }

        [TestMethod]
        public void TryFix_UnrepairableByOtherDevice_IsRepaired()
        {
            // 修理不可で名指しされていない修理装置なら修理できる
            var f = CreateField();
            f.DefineUnit("修理不可機").AddFeature("修理不可=修理不可 別の装置");
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var t = f.Deploy("修理不可機", "敵B", "敵", 4, 3);
            t.HP = 100;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryFix(true));
            Assert.AreEqual(1000, t.HP);
        }

        [TestMethod]
        public void TryFix_Berserk_DoesNotRepair()
        {
            var f = CreateField();
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var t = f.Deploy("機体", "敵B", "敵", 4, 3);
            t.HP = 100;
            u.AddCondition("狂戦士", 1);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryFix(true));
            Assert.AreEqual(100, t.HP);
        }
    }
}
