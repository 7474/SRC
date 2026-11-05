using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Models;
using SRCCore.Units;
using static SRCCore.Coverage.Tests.CommandsCoverage.CommandAttackProcessFixture;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// ユニットコマンドのメニューの「アビリティ」の表示と、一覧を出さずにアビリティを選ぶ条件のテスト。
    /// 元の SRC の Command.bas の ProceedCommand（mnuUnitCommandItem(AbilityCmdID).Caption）と
    /// GUI.bas の AbilityListBox に対応する（7474/SRC#1491）。
    /// </summary>
    [TestClass]
    public class CommandAbilityMenuCaptionTests
    {
        private static AbilityData AddAbility(UnitData ud, string name, string necessarySkill = "")
        {
            var ad = ud.AddAbility(name);
            ad.MinRange = 1;
            ad.MaxRange = 1;
            ad.SetEffect("回復Lv1");
            ad.NecessarySkill = necessarySkill;
            return ad;
        }

        // 回復アビリティを持つ自機 (5,5) と、損傷した味方 (6,5)
        private static CommandAttackProcessFixture Create(out Unit self, params (string name, string skill)[] abilities)
        {
            var f = new CommandAttackProcessFixture();
            var ud = f.AddUnitData("自機");
            foreach (var (name, skill) in abilities)
            {
                AddAbility(ud, name, skill);
            }
            f.AddUnitData("僚機");
            self = f.Deploy("自機", "主人公", "味方", 5, 5);
            var ally = f.Deploy("僚機", "仲間", "味方", 6, 5);
            ally.HP = ally.MaxHP / 2;
            return f;
        }

        private static void OpenMenu(CommandAttackProcessFixture f, Unit self)
        {
            f.Commands.CommandState = "ユニット選択";
            f.Commands.ProceedInput(GuiButton.Left, f.SRC.Map.MapData[5, 5], self);
        }

        private static UnitAbility AbilityWithoutListBox(CommandAttackProcessFixture f, Unit self, string lb_mode = "移動前", bool is_item = false)
        {
            return new UnitAbilityList(AbilityListMode.BeforeMove, self).AbilityWithoutListBox(f.SRC, lb_mode, is_item);
        }

        [TestMethod]
        public void SingleAbility_MenuShowsAbilityName_AndSelectsWithoutList()
        {
            var f = Create(out var self, ("リペア", ""));

            OpenMenu(f, self);

            Assert.AreEqual("リペア", Label(f.LastUnitCommandMenu(), Command.AbilityCmdID));
            Assert.AreEqual("リペア", f.Commands.AbilityCommandCaption);
            Assert.AreEqual("リペア", AbilityWithoutListBox(f, self).Data.Name);
            // 一覧・アイテムでは一覧を出す
            Assert.IsNull(AbilityWithoutListBox(f, self, "一覧"));
            Assert.IsNull(AbilityWithoutListBox(f, self, is_item: true));
        }

        [TestMethod]
        public void TwoAbilities_MenuShowsTerm_AndShowsList()
        {
            var f = Create(out var self, ("リペア", ""), ("ヒール", ""));

            OpenMenu(f, self);

            Assert.AreEqual("アビリティ", Label(f.LastUnitCommandMenu(), Command.AbilityCmdID));
            Assert.IsNull(AbilityWithoutListBox(f, self));
        }

        [TestMethod]
        public void BeforeMove_UnmasteredAbilityIsNotCounted()
        {
            // 移動前は、修得していないアビリティを数えずにアビリティ名を表示し、修得済みのアビリティを選ぶ
            var f = Create(out var self, ("秘術", "存在しない技能"), ("リペア", ""));

            OpenMenu(f, self);

            Assert.AreEqual("リペア", Label(f.LastUnitCommandMenu(), Command.AbilityCmdID));
            Assert.AreEqual("リペア", AbilityWithoutListBox(f, self).Data.Name);
        }

        [TestMethod]
        public void AbilityNameSameAsTerm_ShowsList()
        {
            // メニューの表示が「アビリティ」と同じなら、アビリティが 1 つでも一覧を出す
            var f = Create(out var self, ("アビリティ", ""));

            OpenMenu(f, self);

            Assert.AreEqual("アビリティ", f.Commands.AbilityCommandCaption);
            Assert.IsNull(AbilityWithoutListBox(f, self));
        }

        [TestMethod]
        public void BeforeMenu_ShowsList()
        {
            // メニューを出す前は一覧を出す
            var f = Create(out var self, ("リペア", ""));

            Assert.IsNull(AbilityWithoutListBox(f, self));
        }
    }
}
