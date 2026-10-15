using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.Models;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// COM.TryBattleTransform（戦闘形態への変形）のテスト。
    /// VB 版 COM.bas の TryBattleTransform に対応する。
    /// 5 マス以内に敵がいる場合、運動性(同じなら攻撃力、装甲)が最も高い形態に変形する。
    /// </summary>
    [TestClass]
    public class COMTryBattleTransformTests
    {
        private static COMTestField CreateField(out UnitData formA, out UnitData formB, int mobilityA = 100, int mobilityB = 120)
        {
            var f = new COMTestField();
            f.Src.FileSystem = new LocalFileSystem();
            // 変形イベントを発生させるためイベントデータを初期化しておく（イベントは空）
            f.Src.Event.InitEventData();
            formA = f.DefineUnit("形態A", mobility: mobilityA);
            formA.AddFeature("変形=変形 形態B");
            COMTestField.AddWeapon(formA, "パンチ", 1500);
            formB = f.DefineUnit("形態B", mobility: mobilityB);
            formB.AddFeature("変形=変形 形態A");
            COMTestField.AddWeapon(formB, "キック", 1500);
            COMTestField.AddWeapon(f.DefineUnit("機体"), "パンチ", 1200);
            return f;
        }

        [TestMethod]
        public void TryBattleTransform_EnemyNearby_TransformsToMoreMobileForm()
        {
            var f = CreateField(out _, out _);
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            f.Deploy("機体", "味方A", "味方", 3, 8); // 距離 5
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryBattleTransform());

            var cur = f.Src.Commands.SelectedUnit;
            Assert.AreEqual("形態B", cur.Name);
            Assert.AreEqual(3, cur.x);
            Assert.AreEqual(3, cur.y);
            Assert.AreSame(cur, f.Src.Map.MapDataForUnit[3, 3]);
        }

        [TestMethod]
        public void TryBattleTransform_NoEnemyWithin5_DoesNotTransform()
        {
            var f = CreateField(out _, out _);
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            f.Deploy("機体", "味方A", "味方", 3, 9); // 距離 6
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryBattleTransform());
            Assert.AreEqual("形態A", f.Src.Commands.SelectedUnit.Name);
        }

        [TestMethod]
        public void TryBattleTransform_CurrentFormIsMostMobile_DoesNotTransform()
        {
            var f = CreateField(out _, out _, mobilityA: 130, mobilityB: 120);
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            f.Deploy("機体", "味方A", "味方", 3, 4);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryBattleTransform());
            Assert.AreEqual("形態A", f.Src.Commands.SelectedUnit.Name);
        }

        [TestMethod]
        public void TryBattleTransform_FormFixed_DoesNotTransform()
        {
            // 形態固定状態では変形しない
            var f = CreateField(out _, out _);
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            f.Deploy("機体", "味方A", "味方", 3, 4);
            u.AddCondition("形態固定", 1);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryBattleTransform());
        }

        [TestMethod]
        public void TryBattleTransform_WithoutTransformFeature_ReturnsFalse()
        {
            var f = CreateField(out _, out _);
            var u = f.Deploy("機体", "敵A", "敵", 3, 3);
            f.Deploy("機体", "味方A", "味方", 3, 4);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryBattleTransform());
        }

        [TestMethod]
        public void TryBattleTransform_SameMobility_PrefersHigherPower()
        {
            // 運動性が同じなら攻撃力が高い形態を優先する
            var f = CreateField(out _, out var formB, mobilityA: 100, mobilityB: 100);
            formB.Weapons[0].Power = 2000;
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            f.Deploy("機体", "味方A", "味方", 3, 4);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryBattleTransform());
            Assert.AreEqual("形態B", f.Src.Commands.SelectedUnit.Name);
        }

        [TestMethod]
        public void TryBattleTransform_SamePowerAndMobility_PrefersHigherArmor()
        {
            // 攻撃力も同じなら装甲が高い形態を優先し、装甲も同じなら変形しない
            var f = CreateField(out _, out var formB, mobilityA: 100, mobilityB: 100);
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            f.Deploy("機体", "味方A", "味方", 3, 4);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryBattleTransform());

            formB.Armor = 1200;
            Assert.IsTrue(f.Src.COM.TryBattleTransform());
            Assert.AreEqual("形態B", f.Src.Commands.SelectedUnit.Name);
        }

        [TestMethod]
        public void TryBattleTransform_SameMobility_ComparesLastWeaponPower()
        {
            // VB: .Data.Weapon(.Data.CountWeapon).Power < u.Data.Weapon(u.Data.CountWeapon).Power なら変形しない
            // 形態B の最後の武器(1000) < 形態A の最後の武器(1500) なので変形しない
            var f = CreateField(out _, out var formB, mobilityA: 100, mobilityB: 100);
            formB.Weapons[0].Power = 3000;
            COMTestField.AddWeapon(formB, "ジャブ", 1000);
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            f.Deploy("機体", "味方A", "味方", 3, 4);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryBattleTransform());
            Assert.AreEqual("形態A", f.Src.Commands.SelectedUnit.Name);
        }

        [TestMethod]
        public void TryBattleTransform_FormDesignatedAsNormalMode_IsIgnored()
        {
            // VB: uname = LIndex(.OtherForm(uname).FeatureData("ノーマルモード"), 1) の形態は候補から外す
            var f = CreateField(out _, out var formB);
            formB.AddFeature("ノーマルモード=形態B");
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            f.Deploy("機体", "味方A", "味方", 3, 4);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryBattleTransform());
            Assert.AreEqual("形態A", f.Src.Commands.SelectedUnit.Name);
        }
    }
}
