using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Unit.cls の他形態・パイロットの手続き（AddOtherForm〜AdditionalSupport）を元の SRC と突き合わせたテスト（#1498）。
    /// </summary>
    [TestClass]
    public class UnitOtherFormPilotVBMatchTests
    {
        [TestMethod]
        public void DeleteOtherForm_UnitName_RemovesFormFoundByName()
        {
            // 元の SRC: キー（ID）で見つからなければユニット名称で探して削除する
            var fx = new UnitAbilityCoverageFixture();
            fx.AddUnitData("形態Ａ");
            fx.AddUnitData("形態Ｂ");
            var u = fx.CreateUnit("形態Ａ", "乗るパイロット");
            var other = u.OtherForm("形態Ｂ");
            Assert.IsNotNull(other);
            Assert.AreEqual(1, u.CountOtherForm());

            u.DeleteOtherForm("形態Ｂ");

            Assert.AreEqual(0, u.CountOtherForm());
        }

        [TestMethod]
        public void DeleteTemporaryOtherForm_DuplicateFeatureName_UsesDataOfFirstFeature()
        {
            // 元の SRC: 必要な形態の一覧は FeatureData(fname) で名前から引くので、
            // 同じ名前の特殊能力が 2 つあると 1 つ目のデータを 2 回使い、2 つ目に書いた形態は不要として破棄される
            var fx = new UnitAbilityCoverageFixture();
            var ud = fx.AddUnitData("本体");
            ud.AddFeature("他形態=形態Ａ");
            ud.AddFeature("他形態=形態Ｂ");
            fx.AddUnitData("形態Ａ");
            fx.AddUnitData("形態Ｂ");
            var u = fx.CreateUnit("本体", "乗るパイロット");
            var a = u.OtherForm("形態Ａ");
            var b = u.OtherForm("形態Ｂ");
            Assert.AreEqual(2, u.Features.Count(x => x.Name == "他形態"));

            u.DeleteTemporaryOtherForm();

            Assert.IsTrue(u.IsOtherFormDefined("形態Ａ"));
            Assert.AreNotEqual("破棄", a.Status);
            Assert.IsFalse(u.IsOtherFormDefined("形態Ｂ"));
            Assert.AreEqual("破棄", b.Status);
        }

        [TestMethod]
        public void MainPilot_ExistingBerserkPilotWithoutUpdate_SetsUnit()
        {
            // 元の SRC: 作成済みの暴走時パイロットは、without_update に関わらず先にユニットを設定する
            var fx = new UnitAbilityCoverageFixture();
            foreach (var name in new[] { "暴走機Ａ", "暴走機Ｂ" })
            {
                var ud = fx.AddUnitData(name);
                ud.PilotNum = 1;
                ud.AddFeature("暴走時パイロット=暴走パイロット");
            }
            fx.AddPilotData("暴走パイロット");
            var u1 = fx.CreateUnit("暴走機Ａ", "乗るＡ");
            var u2 = fx.CreateUnit("暴走機Ｂ", "乗るＢ");
            u1.AddCondition("暴走", 3);
            u2.AddCondition("暴走", 3);
            var berserk = u1.MainPilot();
            Assert.AreSame(u1, berserk.Unit);

            var main = u2.MainPilot(true);

            Assert.AreSame(berserk, main);
            Assert.AreSame(u2, main.Unit);
        }

        [TestMethod]
        public void MainPilot_AdditionalPilotOfOtherForm_KeepsMoraleAndSetsUnit()
        {
            // 元の SRC: 他形態の追加パイロットを引き継ぐときは、陣営とユニットを設定するだけで、
            // 乗っているパイロットの気力・レベル・経験値には合わせない
            var fx = new UnitAbilityCoverageFixture();
            foreach (var name in new[] { "形態Ａ", "形態Ｂ" })
            {
                var ud = fx.AddUnitData(name);
                ud.PilotNum = 1;
                ud.AddFeature("追加パイロット=合体パイロット");
            }
            fx.AddPilotData("合体パイロット");
            var ua = fx.CreateUnit("形態Ａ", "乗るＡ");
            var main = ua.MainPilot();
            Assert.AreEqual("合体パイロット", main.Name);
            main.Morale = 110;
            // 変形などで作られる他形態（作っただけでは追加パイロットを引いていない）に別のパイロットを乗せる
            var ub = ua.OtherForm("形態Ｂ");
            fx.AddPilotData("乗るＢ");
            var pb = fx.Src.PList.Add("乗るＢ", 1, "味方");
            pb.Morale = 130;
            ub.AddPilot(pb);
            pb.Unit = ub;
            Assert.AreSame(ua, main.Unit);

            var mainB = ub.MainPilot();

            Assert.AreSame(main, mainB);
            Assert.AreSame(ub, mainB.Unit);
            Assert.AreEqual(110, mainB.Morale);
        }

        [TestMethod]
        public void ReplaceSupport_RegistersSupportsByID()
        {
            // 元の SRC: AddSupport は名前をキーにするが、ReplaceSupport は入れ替えのあと ID をキーにして登録し直す
            var fx = new UnitAbilityCoverageFixture();
            fx.AddUnitData("支援機");
            fx.AddPilotData("サポート");
            fx.AddPilotData("代わり(ザコ)");
            var u = fx.CreateUnit("支援機", "乗るパイロット");
            var s1 = fx.Src.PList.Add("サポート", 1, "味方");
            u.AddSupport(s1);
            s1.Unit = u;
            var s2 = fx.Src.PList.Add("代わり(ザコ)", 1, "味方");
            Assert.AreNotEqual(s2.Name, s2.ID);

            u.ReplaceSupport(s1, s2);

            Assert.AreEqual(1, u.CountSupport());
            Assert.AreSame(s2, u.Support(s2.ID));
            Assert.AreSame(u, s2.Unit);
            Assert.IsNull(s1.Unit);
            Assert.IsFalse(s1.Alive);
        }
    }
}
