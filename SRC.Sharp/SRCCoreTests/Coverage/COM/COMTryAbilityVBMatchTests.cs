using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.Models;
using System.Linq;
using System.Reflection;

namespace SRCCore.Coverage.Tests.COMTests
{
    /// <summary>
    /// VB 版 COM.bas の TryHyperMode・TryBattleTransform・TryInstantAbility・TryMapHealing・TryHealing と
    /// 同じ動作になるかのテスト（7474/SRC#1501）。期待値は VB 版の実装に従う。
    /// </summary>
    [TestClass]
    public class COMTryAbilityVBMatchTests
    {
        private static COMTestField CreateField()
        {
            var f = new COMTestField();
            f.Src.FileSystem = new LocalFileSystem();
            // 使用イベント・変形イベントを呼ぶためイベントデータを初期化しておく（イベントは空）
            f.Src.Event.InitEventData();
            COMTestField.AddWeapon(f.DefineUnit("機体"), "パンチ", 1200);
            return f;
        }

        private static AbilityData AddAbility(UnitData ud, string name, string effect, int maxRange, int en = 0, string aclass = "", int stock = 0)
        {
            var ad = ud.AddAbility(name);
            ad.SetEffect(effect);
            ad.Class = aclass;
            ad.MinRange = maxRange == 0 ? 0 : 1;
            ad.MaxRange = maxRange;
            ad.ENConsumption = en;
            ad.Stock = stock;
            return ad;
        }

        private static void InvokeTryHyperMode(COMTestField f)
        {
            typeof(COM).GetMethod("TryHyperMode", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(f.Src.COM, null);
        }

        [TestMethod]
        public void TryHyperMode_NecessaryMoraleIsRoundedToEvenLikeVB()
        {
            // VB 版は 100 + CInt(10# * flevel)。Lv0.25 なら 2.5 → 2 で、気力 102 で発動する
            var f = CreateField();
            var formA = f.DefineUnit("形態A");
            formA.AddFeature("ハイパーモードLv0.25=ハイパーモード 形態B");
            COMTestField.AddWeapon(formA, "パンチ", 1500);
            COMTestField.AddWeapon(f.DefineUnit("形態B"), "キック", 1500);
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            u.MainPilot().Morale = 102;
            f.Src.Commands.SelectedUnit = u;

            InvokeTryHyperMode(f);

            Assert.AreEqual("形態B", f.Src.Commands.SelectedUnit.Name);
        }

        [TestMethod]
        public void TryHyperMode_MoraleBelowRoundedLevel_DoesNotTransform()
        {
            var f = CreateField();
            var formA = f.DefineUnit("形態A");
            formA.AddFeature("ハイパーモードLv0.25=ハイパーモード 形態B");
            COMTestField.AddWeapon(formA, "パンチ", 1500);
            COMTestField.AddWeapon(f.DefineUnit("形態B"), "キック", 1500);
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            u.MainPilot().Morale = 101;
            f.Src.Commands.SelectedUnit = u;

            InvokeTryHyperMode(f);

            Assert.AreEqual("形態A", f.Src.Commands.SelectedUnit.Name);
        }

        [TestMethod]
        public void TryBattleTransform_AdditionalPilotAlreadyExists_IsNotAddedAgain()
        {
            // VB 版は PList.IsDefined(u.FeatureData("追加パイロット")) で、いれば作らない。
            // 元は "追加パイロット" という名前で調べていたので、いつも作ろうとして「既に登録されています」になっていた
            var f = CreateField();
            var formA = f.DefineUnit("形態A", mobility: 100);
            formA.AddFeature("変形=変形 形態B");
            COMTestField.AddWeapon(formA, "パンチ", 1500);
            var formB = f.DefineUnit("形態B", mobility: 120);
            formB.AddFeature("変形=変形 形態A");
            formB.AddFeature("追加パイロット=サブ");
            COMTestField.AddWeapon(formB, "キック", 1500);
            f.DefinePilot("サブ");
            f.Src.PList.Add("サブ", 1, "敵");
            var u = f.Deploy("形態A", "敵A", "敵", 3, 3);
            f.Deploy("機体", "味方A", "味方", 3, 4);
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryBattleTransform());

            Assert.AreEqual("形態B", f.Src.Commands.SelectedUnit.Name);
            var errors = f.GUI.CallsOf(nameof(IGUI.ErrorMessage)).Select(x => (string)x.Args[0]).ToList();
            Assert.IsFalse(errors.Any(x => x.Contains("既に登録されています")), string.Join("\n", errors));
        }

        [TestMethod]
        public void TryInstantAbility_PlanaConsumptionMoreThanHalf_IsStillUsed()
        {
            // VB 版の TryInstantAbility は EN の消費だけを見て、霊力の消費が残りの半分以上でも使う
            // （使えるかどうかは IsAbilityUseful の中の IsAbilityAvailable が見る）
            var f = CreateField();
            var ud = f.DefineUnit("術機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            AddAbility(ud, "集中術", "状態=集中 再行動", 0, aclass: "霊L2");
            f.DefinePilot("術士").AddSkill("霊力", 50d, "", 1);
            var u = f.Deploy("術機", "術士", "敵", 3, 3);
            u.MainPilot().Plana = 15;
            f.Deploy("機体", "味方A", "味方", 3, 5);
            f.Src.Commands.SelectedUnit = u;

            f.Src.COM.TryInstantAbility();

            Assert.AreEqual("集中術", f.Src.Commands.SelectedAbilityName);
            Assert.AreEqual(5, u.MainPilot().Plana);
        }

        [TestMethod]
        public void TryHealing_AbilityWithBothPAndQ_DoesNotMoveToHeal()
        {
            // VB 版は「射程 1 か Ｐ属性」でも、Ｑ属性があれば移動してまで使わない
            var f = CreateField();
            var ud = f.DefineUnit("修理機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            AddAbility(ud, "修理ビーム", "回復Lv1", 3, en: 10, aclass: "ＰＱ");
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var t = f.Deploy("機体", "敵B", "敵", 7, 3);
            t.HP = 1000;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsFalse(f.Src.COM.TryHealing(false));
            Assert.AreEqual(3, u.x);
            Assert.AreEqual(1000, t.HP);
        }

        [TestMethod]
        public void TryHealing_SamePowerAndEN_ComparesStockOfAbilityData()
        {
            // 回復量と EN が同じなら、VB 版は .Ability(a).Stock（データの使用回数）で比べる。
            // 残りの使用回数ではないので、残りが 1 回でもデータの使用回数が多い後のアビリティに替える
            var f = CreateField();
            var ud = f.DefineUnit("修理機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            AddAbility(ud, "修理A", "回復Lv1", 3, en: 10, stock: 3);
            AddAbility(ud, "修理B", "回復Lv1", 3, en: 10, stock: 5);
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            u.Abilities[1].SetStock(1);
            var t = f.Deploy("機体", "敵B", "敵", 4, 3);
            t.HP = 1000;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryHealing(false));

            Assert.AreEqual("修理B", f.Src.Commands.SelectedAbilityName);
        }

        [TestMethod]
        public void TryMapHealing_SameScore_PrefersLastAbilityLikeVB()
        {
            // VB 版は最後のアビリティから順に調べ、評価が上回ったときだけ替えるので、同じ評価なら番号の大きいものを使う
            var f = CreateField();
            var ud = f.DefineUnit("修理機");
            COMTestField.AddWeapon(ud, "パンチ", 1200);
            AddAbility(ud, "回復フィールドA", "回復Lv1", 2, en: 10, aclass: "Ｍ全");
            AddAbility(ud, "回復フィールドB", "回復Lv1", 2, en: 10, aclass: "Ｍ全");
            var u = f.Deploy("修理機", "敵A", "敵", 3, 3);
            var t1 = f.Deploy("機体", "敵B", "敵", 4, 3);
            var t2 = f.Deploy("機体", "敵C", "敵", 3, 4);
            t1.HP = 1000;
            t2.HP = 1000;
            f.Src.Commands.SelectedUnit = u;

            Assert.IsTrue(f.Src.COM.TryMapHealing(false));

            Assert.AreEqual("回復フィールドB", f.Src.Commands.SelectedAbilityName);
        }
    }
}
