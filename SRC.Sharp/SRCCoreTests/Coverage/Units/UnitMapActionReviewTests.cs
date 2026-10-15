using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests.UnitsCoverage
{
    /// <summary>
    /// Unit.map.cs / Unit.action2.cs のレビュー指摘で追加したテスト。
    /// HP/EN の受け継ぎ、格納中ユニットの出撃、獲得経験値増加の効果を確認する。
    /// </summary>
    [TestClass]
    public class UnitMapActionReviewTests
    {
        [TestMethod]
        public void Transform_CarriesHpAndEnRatio()
        {
            // 変形後の HP/EN は変形前の割合を受け継ぐ（Unit.cls Transform: .HP = .MaxHP * hp_ratio / 100）
            var f = new UnitMapActionFixture();
            f.AddUnitData("形態A", hp: 1000, en: 100, features: "変形=変形 形態B");
            f.AddUnitData("形態B", hp: 2000, en: 200, features: "変形=変形 形態A");
            var a = f.CreateUnit("形態A", "味方", f.CreatePilot("パイロット"));
            a.HP = 250;
            a.EN = 40;

            a.Transform("形態B");

            var b = a.OtherForm("形態B");
            Assert.AreEqual(500, b.HP);
            Assert.AreEqual(80, b.EN);
        }

        [TestMethod]
        public void Transform_ToPartsSeparationForm_RestoresFullHp()
        {
            // パーツ分離先の形態へ変形した場合は HP の割合を受け継がず全快する。EN は割合を受け継ぐ
            var f = new UnitMapActionFixture();
            f.AddUnitData("本体", hp: 1000, en: 100, features: "パーツ分離=パーツ分離 コア");
            f.AddUnitData("コア", hp: 600, en: 50);
            var u = f.CreateUnit("本体", "味方", f.CreatePilot("パイロット"));
            u.HP = 100;
            u.EN = 40;

            u.Transform("コア");

            var core = u.OtherForm("コア");
            Assert.AreEqual(600, core.HP);
            Assert.AreEqual(20, core.EN);
        }

        [TestMethod]
        public void Combine_AveragesHpAndEnRatioOfParts()
        {
            // 合体後の HP/EN は各パーツの割合の平均（Unit.cls Combine: .HP = .MaxHP * hp_ratio / 100 / UBound(rarray)）
            var f = new UnitMapActionFixture();
            var uc = f.AddUnitData("合体機", hp: 3000, en: 200, features: "分離=分離 パーツA パーツB");
            uc.PilotNum = 2;
            f.AddUnitData("パーツA", hp: 1000, en: 100, features: "合体=合体 合体機 パーツB");
            f.AddUnitData("パーツB", hp: 1000, en: 100, features: "合体=合体 合体機 パーツA");
            var combined = f.CreateUnit("合体機");
            combined.Status = "他形態";
            var pa = f.CreateUnit("パーツA", "味方", f.CreatePilot("パイロットA"));
            var pb = f.CreateUnit("パーツB", "味方", f.CreatePilot("パイロットB"));
            pa.Status = "待機";
            pb.Status = "待機";
            pa.HP = 500;
            pa.EN = 100;
            pb.EN = 50;

            pa.Combine("合体機", true);

            Assert.AreEqual(2250, combined.HP);
            Assert.AreEqual(150, combined.EN);
        }

        [TestMethod]
        public void StandBy_StoredUnit_IsUnloadedFromCarrier()
        {
            // 格納中のユニットを出撃させると、あらかじめ母艦から降ろされる（Unit.cls StandBy）
            var f = new UnitMapActionFixture();
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("艦載機");
            var carrier = f.Deploy(f.CreateUnit("母艦"), 5, 5);
            var u = f.CreateUnit("艦載機", "味方", f.CreatePilot("パイロット"));
            u.Status = "格納";
            carrier.LoadUnit(u);

            u.StandBy(2, 3);

            Assert.AreEqual(0, carrier.CountUnitOnBoard());
            Assert.AreEqual("出撃", u.Status);
            Assert.AreSame(u, f.Map.MapDataForUnit[2, 3]);
        }

        // 獲得経験値増加Lv10（経験値2倍）のスペシャルパワーを定義する
        private static void DefineExpUp(UnitMapActionFixture f)
        {
            var sp = f.Src.SPDList.Add("努力");
            sp.Duration = "攻撃";
            sp.SetEffect("獲得経験値増加Lv10");
        }

        [TestMethod]
        public void GetExp_ExpUpSpecialPower_MultipliesDestroyExpExceptPartner()
        {
            // 獲得経験値増加は破壊・攻撃の経験値を (1 + 0.1 * Lv) 倍する。パートナーとしての入手には効かない
            var f = new UnitMapActionFixture();
            DefineExpUp(f);
            f.AddUnitData("味方機");
            f.AddUnitData("敵機").ExpValue = 100;
            f.AddPilotData("敵パイロット", expValue: 50);
            var u = f.CreateUnit("味方機", "味方", f.CreatePilot("パイロット", 10));
            var t = f.CreateUnit("敵機", "敵", f.CreatePilot("敵パイロット", 10, "敵"));
            u.MakeSpecialPowerInEffect("努力");

            Assert.AreEqual(300, u.GetExp(t, "破壊", "マップ"));
            Assert.AreEqual(150, u.GetExp(t, "破壊", "パートナー"));
        }

        [TestMethod]
        public void GetExp_ExpUpSpecialPower_SuppressesTalentUnlessOptionSet()
        {
            // 獲得経験値増加の効果中は「素質」が効かない。オプション「収得効果重複」があれば重複する
            var f = new UnitMapActionFixture();
            DefineExpUp(f);
            f.AddUnitData("味方機");
            f.AddPilotData("素質持ち").AddSkill("素質", Constants.DEFAULT_LEVEL, "", 1);
            var u = f.CreateUnit("味方機", "味方", f.CreatePilot("素質持ち", 10));
            var t = f.CreateUnit("味方機", "味方", f.CreatePilot("相手", 10));
            u.MakeSpecialPowerInEffect("努力");

            Assert.AreEqual(100, u.GetExp(t, "修理", "マップ"));

            f.Src.Expression.DefineGlobalVariable("Option(収得効果重複)");
            f.Src.Expression.SetVariableAsLong("Option(収得効果重複)", 1);
            Assert.AreEqual(150, u.GetExp(t, "修理", "マップ"));
        }
    }
}
