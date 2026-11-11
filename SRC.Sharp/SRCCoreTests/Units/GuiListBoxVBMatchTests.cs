using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Models;
using SRCCore.TestLib;
using System.Linq;

namespace SRCCore.Units.Tests
{
    /// <summary>
    /// 武器選択・アビリティ選択のリストボックスを元の SRC（GUI.bas の WeaponListBox・AbilityListBox）に
    /// 合わせたことのテスト（7474/SRC#1498）。
    /// </summary>
    [TestClass]
    public class GuiListBoxVBMatchTests
    {
        private static Unit CreateUnit(SRC src, System.Action<UnitData> setup)
        {
            var pd = src.PDList.Add("テストパイロット");
            pd.SP = 10;
            var p = src.PList.Add("テストパイロット", 1, "味方");
            var ud = src.UDList.Add("テスト機");
            ud.HP = 1000;
            ud.EN = 100;
            ud.PilotNum = 1;
            setup(ud);
            var u = src.UList.Add("テスト機", 0, "味方");
            p.Ride(u);
            u.Update();
            src.Commands.SelectedUnit = u;
            return u;
        }

        private static Unit CreateAbilityUnit(SRC src)
        {
            return CreateUnit(src, ud =>
            {
                var a1 = ud.AddAbility("修理装置");
                a1.SetEffect("回復Lv2");
                a1.MinRange = 1;
                a1.MaxRange = 1;
                var a2 = ud.AddAbility("回復薬");
                a2.SetEffect("回復Lv1");
                a2.MinRange = 0;
                a2.MaxRange = 0;
                a2.NecessarySkill = "アイテム";
            });
        }

        /// <summary>
        /// アビリティの選択では、アイテムの使用効果（必要技能「アイテム」）はアイテムの選択だけに、
        /// それ以外のアビリティはアビリティの選択だけに出す。一覧では、使えるアイテムの使用効果は出さない。
        /// </summary>
        [TestMethod]
        public void AbilityListBox_SeparatesItemAbilities()
        {
            var src = new SRC { GUI = new MockGUI() };
            var u = CreateAbilityUnit(src);
            Assert.IsTrue(u.Abilities.Single(x => x.Data.Name == "回復薬").Data.IsItem());

            var before = new UnitAbilityList(AbilityListMode.BeforeMove, u);
            CollectionAssert.AreEqual(new[] { "修理装置" },
                before.ToListBoxItems(false).Select(x => x.ListItemObject.Data.Name).Distinct().ToArray());
            CollectionAssert.AreEqual(new[] { "回復薬" },
                before.ToListBoxItems(true).Select(x => x.ListItemObject.Data.Name).Distinct().ToArray());

            var list = new UnitAbilityList(AbilityListMode.List, u);
            CollectionAssert.AreEqual(new[] { "修理装置" },
                list.ToListBoxItems().Select(x => x.ListItemObject.Data.Name).Distinct().ToArray());
        }

        /// <summary>
        /// 反撃の武器選択は「連続表示,カーソル移動」で出す（選んでも閉じず、呼び出し元の COM が閉じる）。
        /// 攻撃・一覧の武器選択はその指定をしない。
        /// </summary>
        [TestMethod]
        public void WeaponListBox_CounterUsesContinuousModeWithCursor()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui };
            var u = CreateUnit(src, ud =>
            {
                var wd = ud.AddWeapon("ビームライフル");
                wd.Power = 1500;
                wd.MinRange = 1;
                wd.MaxRange = 4;
                wd.Adaption = "AAAA";
            });
            // 相手を決めずに表示内容を作るため、予測命中率を出さない
            src.Expression.DefineGlobalVariable("Option(予測命中率非表示)");
            string mode = null;
            gui.ListBoxHandler = args =>
            {
                mode = args.lb_mode;
                return 0;
            };

            new UnitWeaponList(WeaponListMode.List, u) { Mode = WeaponListMode.Counter }.SelectWeapon(src, "反撃", gui.ListBox);
            Assert.AreEqual("連続表示,カーソル移動", mode);

            new UnitWeaponList(WeaponListMode.List, u).SelectWeapon(src, "武装一覧", gui.ListBox);
            Assert.AreEqual("", mode);
        }
    }
}
