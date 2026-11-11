using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Coverage.Tests.UnitAttack;
using SRCCore.Models;
using SRCCore.Units;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Unit.ExecuteAbility を VB 版（Unit.cls の Function ExecuteAbility）と突き合わせたときに見つかった違いのテスト（#1502）。
    /// 期待値は VB 版の動作。
    /// </summary>
    [TestClass]
    public class UnitExecuteAbilityVBMatchTests
    {
        private static AbilityData AddAbility(UnitData ud, string name, string effect, string aclass = "", int maxRange = 1)
        {
            var ad = ud.AddAbility(name);
            ad.SetEffect(effect);
            ad.Class = aclass;
            ad.MinRange = maxRange == 0 ? 0 : 1;
            ad.MaxRange = maxRange;
            ad.NecessarySkill = "";
            ad.NecessaryCondition = "";
            return ad;
        }

        // VB: Case "変身" の最後の Exit For で効果の適用を打ち切る。後ろに書いた回復は適用されない
        [TestMethod]
        public void Transform_StopsApplyingRemainingEffects()
        {
            var fx = new UnitAbilityCoverageFixture();
            var ud = fx.AddUnitData("変身前");
            AddAbility(ud, "変身術", "変身=変身後 回復Lv1");
            fx.AddUnitData("変身後");
            var user = fx.CreateUnit("変身前", "使用者");
            var target = fx.CreateUnit("変身前", "対象者");
            target.HP = 300;

            user.ExecuteAbility(user.Ability(1), target);

            var tf = target.CurrentForm();
            Assert.AreEqual("変身後", tf.Name);
            Assert.IsFalse(fx.SysMessages.Any(m => m.Contains("回復した")));
        }

        // VB: 「アビリティの種類は？」のループは召喚で Exit For するので、後ろの効果は atype にならない。
        // atype が空ならアビリティ使用のメッセージを表示しない
        [TestMethod]
        public void Summon_StopsDecidingAbilityType()
        {
            var fx = new UnitAbilityCoverageFixture();
            var ud = fx.AddUnitData("召喚機");
            // 召喚するユニットのデータは無いので、召喚そのものは ErrorMessage で止まる
            AddAbility(ud, "召喚術", "召喚=未定義の召喚獣 回復Lv1");
            var user = fx.CreateUnit("召喚機", "使用者");

            user.ExecuteAbility(user.Ability(1), user);

            Assert.IsFalse(fx.SysMessages.Any(m => m.Contains("召喚術")));
        }

        // VB: 解説のメッセージ・使用のメッセージの前に開くのは frmMessage が表示されていないとき（メインの画面の表示とは関係しない）
        [TestMethod]
        public void UseMessage_OpensMessageFormWhenMessageFormIsHidden()
        {
            var fx = new UnitAbilityCoverageFixture();
            var ud = fx.AddUnitData("使用機");
            AddAbility(ud, "テストアビリティ", "回復Lv1");
            fx.AddUnitData("対象機");
            var user = fx.CreateUnit("使用機", "使用者");
            var target = fx.CreateUnit("対象機", "対象者");
            target.HP = 300;
            fx.Gui.Handle("get_MainFormVisible", _ => true);
            fx.Gui.Handle("get_MessageFormVisible", _ => false);

            user.ExecuteAbility(user.Ability(1), target);

            var calls = fx.Gui.Calls.Select(x => x.Member).ToList();
            var open = calls.IndexOf(nameof(IGUI.OpenMessageForm));
            var msg = calls.IndexOf(nameof(IGUI.DisplaySysMessage));
            Assert.IsTrue(open >= 0);
            Assert.IsTrue(open < msg);
        }

        // VB: ＳＰ回復で複数のパイロットが対象のとき、DrawSysString で数値を出すのはメインパイロットだけ
        [TestMethod]
        public void SpRecovery_MultiplePilots_DrawsOnlyMainPilotValue()
        {
            var fx = new UnitAbilityCoverageFixture();
            var ud = fx.AddUnitData("使用機");
            AddAbility(ud, "ＳＰ回復術", "ＳＰ回復Lv1");
            var tud = fx.AddUnitData("対象機");
            tud.PilotNum = 2;
            var user = fx.CreateUnit("使用機", "使用者");
            var target = fx.CreateUnit("対象機", "対象者1");
            fx.AddPilotData("対象者2");
            var p2 = fx.Src.PList.Add("対象者2", 1, "味方");
            target.AddPilot(p2);
            p2.Unit = target;
            target.Update();
            foreach (var p in target.AllPilots)
            {
                p.SP = 0;
            }

            user.ExecuteAbility(user.Ability(1), target);

            Assert.AreEqual(2, target.AllPilots.Count());
            Assert.AreEqual(2, fx.SysMessages.Count(m => m.Contains("回復した。")));
            Assert.AreEqual(1, fx.Gui.CallsOf(nameof(IGUI.DrawSysString)).Count());
        }

        // VB: 合体技のパートナーが同名のアビリティを持たないときは、使用者のアビリティの失属性でパートナーのＨＰも減る
        [TestMethod]
        public void Combination_PartnerWithoutAbility_LosesHpByUserAbility()
        {
            var f = new UnitAttackFixture();
            var user = f.AddUnit("使用機", "味方", 2, 2, features: new[] { "合体技=合体アビ 僚機" });
            var partner = f.AddUnit("僚機", "味方", 3, 2);
            AddAbility(f.Src.UDList.Item("使用機"), "合体アビ", "気力増加Lv1", "合失L3", maxRange: 0);
            user.Update();

            user.ExecuteAbility(user.Ability(1), user);

            Assert.AreEqual(700, user.HP);
            Assert.AreEqual(700, partner.HP);
        }

        // VB: 合体技のパートナーが同名のアビリティを持たないときは、使用者のアビリティの自属性でパートナーも破壊される
        [TestMethod]
        public void Combination_PartnerWithoutAbility_SelfDestructs()
        {
            var f = new UnitAttackFixture();
            var user = f.AddUnit("使用機", "味方", 2, 2, features: new[] { "合体技=合体アビ 僚機" });
            var partner = f.AddUnit("僚機", "味方", 3, 2);
            AddAbility(f.Src.UDList.Item("使用機"), "合体アビ", "回復Lv1", "合自", maxRange: 0);
            user.Update();

            user.ExecuteAbility(user.Ability(1), user);

            Assert.AreEqual("破壊", partner.Status);
        }

        // VB: 合体技のパートナーが同名のアビリティを持つときは、そのアビリティの自属性でパートナーも破壊される
        [TestMethod]
        public void Combination_PartnerWithAbility_SelfDestructsByOwnAbility()
        {
            var f = new UnitAttackFixture();
            var user = f.AddUnit("使用機", "味方", 2, 2, features: new[] { "合体技=合体アビ 僚機" });
            var partner = f.AddUnit("僚機", "味方", 3, 2);
            AddAbility(f.Src.UDList.Item("使用機"), "合体アビ", "回復Lv1", "合", maxRange: 0);
            AddAbility(f.Src.UDList.Item("僚機"), "合体アビ", "回復Lv1", "合自", maxRange: 0);
            user.Update();
            partner.Update();

            user.ExecuteAbility(user.Ability(1), user);

            Assert.AreEqual("破壊", partner.Status);
            Assert.AreNotEqual("破壊", user.Status);
        }
    }
}
