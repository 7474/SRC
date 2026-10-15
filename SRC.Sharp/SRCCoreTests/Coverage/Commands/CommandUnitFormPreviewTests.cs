using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Units;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.Commands
{
    /// <summary>
    /// Command.preview.cs（特殊能力一覧・武器一覧・アビリティ一覧・移動範囲・射程範囲）のテスト。
    /// 元の SRC の Command.bas FeatureListCommand / WeaponListCommand / AbilityListCommand /
    /// ShowAreaInSpeedCommand / ShowAreaInRangeCommand に対応する。
    /// </summary>
    [TestClass]
    public class CommandUnitFormPreviewTests
    {
        private static void Run(CommandUnitFormFixture f, int cmdId, string label)
        {
            f.Src.Commands.UnitCommand(new UiCommand(cmdId, label));
        }

        private static List<string> HelpTitles(CommandUnitFormFixture f)
        {
            // Help は DisplayMessage("システム", "<b>名称</b>;解説") で解説を表示する
            return f.Gui.CallsOf("DisplayMessage")
                .Select(c => (string)c.Args[1])
                .Select(m => m.Substring(3, m.IndexOf("</b>") - 3))
                .ToList();
        }

        // ──────────────────────────────────────────────
        // 特殊能力一覧
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FeatureList_ListsPilotSkillsThenUnitFeatures_AndShowsHelpOfSelected()
        {
            // パイロット特殊能力 → ユニット特殊能力の順に一覧を作り、
            // 「表示のみ」で一度表示した後「連続表示」で選択された能力の解説を繰り返し表示する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体", "ホバー移動", "ジャンプ");
            f.AddPilotData("パイロット").AddSkill("底力", 1d, "", 1);
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            var answers = new Queue<int>(new[] { 0, 3, 1, 0 });
            f.ListBoxAnswer = _ => answers.Dequeue();

            Run(f, Command.FeatureListCmdID, "特殊能力一覧");

            Assert.AreEqual(4, f.ListBoxes.Count);
            Assert.AreEqual("特殊能力一覧", f.ListBoxes[0].lb_caption);
            Assert.AreEqual("表示のみ", f.ListBoxes[0].lb_mode);
            Assert.AreEqual("連続表示", f.ListBoxes[1].lb_mode);
            // パイロット能力はレベル付きの名称で表示する
            Assert.AreEqual("底力Lv1|ホバー移動|ジャンプ", string.Join("|", f.ListBoxes[0].Items.Select(x => x.Text)));
            Assert.AreEqual("ジャンプ|底力Lv1", string.Join("|", HelpTitles(f)));
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void FeatureList_SingleFeature_ShowsHelpWithoutList()
        {
            // 表示する能力が１つだけならリストを出さずにその解説を表示する
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("機体", "ホバー移動");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.FeatureListCmdID, "特殊能力一覧");

            Assert.AreEqual(0, f.ListBoxes.Count);
            Assert.AreEqual("ホバー移動", string.Join("|", HelpTitles(f)));
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void FeatureList_ExcludesCombineWhoseCombinedUnitIsNotCreated()
        {
            // 合体能力は合体後の形態が作成されていなければ一覧に出さない
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("合体形態");
            f.AddUnitData("機体", "ホバー移動", "ジャンプ", "合体=合体 合体形態 相方 相方2");
            var u = f.CreateUnit("機体", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;

            Run(f, Command.FeatureListCmdID, "特殊能力一覧");
            Assert.AreEqual("ホバー移動|ジャンプ", string.Join("|", f.ListBoxes[0].Items.Select(x => x.Text)));

            // 合体後の形態を作成すれば表示される
            f.Src.UList.Add("合体形態", 0, "味方").Status = "他形態";
            f.ListBoxes.Clear();
            Run(f, Command.FeatureListCmdID, "特殊能力一覧");
            Assert.AreEqual("ホバー移動|ジャンプ|合体", string.Join("|", f.ListBoxes[0].Items.Select(x => x.Text)));
        }

        // ──────────────────────────────────────────────
        // 武器一覧
        // ──────────────────────────────────────────────

        private static Unit CreateArmedUnit(CommandUnitFormFixture f, string wclass, int minRange, int maxRange, bool onMap)
        {
            var ud = f.AddUnitData("機体");
            var w = ud.AddWeapon("ビームライフル");
            w.Power = 1500;
            w.MinRange = minRange;
            w.MaxRange = maxRange;
            w.Class = wclass;
            w.Adaption = "AAAA";
            return onMap ? f.CreateUnit("機体", "パイロット", x: 4, y: 4) : f.CreateUnit("機体", "パイロット");
        }

        [TestMethod]
        public void WeaponList_StatusView_ListsAttributesAndShowsAttributeHelp()
        {
            // 武器を選ぶと属性の一覧を表示し、選択した属性の解説を表示する。
            // ユニットステータス表示中は「射程範囲」は出さない
            var f = new CommandUnitFormFixture(onMap: false);
            var u = CreateArmedUnit(f, "ＢＰ", 1, 3, onMap: false);
            f.MakeStatusViewUnitList();
            f.Src.Commands.SelectedUnit = u;
            var weaponAnswers = new Queue<UnitWeapon>(new[] { u.Weapons.First(), null });
            f.Gui.Handle("WeaponListBox", args => weaponAnswers.Dequeue());
            var answers = new Queue<int>(new[] { 2, 0 });
            f.ListBoxAnswer = _ => answers.Dequeue();

            Run(f, Command.WeaponListCmdID, "武器一覧");

            var lb = f.ListBoxes[0];
            Assert.AreEqual(2, lb.Items.Count);
            StringAssert.StartsWith(lb.Items[0].Text, "Ｂ");
            StringAssert.StartsWith(lb.Items[1].Text, "Ｐ");
            Assert.IsFalse(lb.Items.Any(x => x.Text == "射程範囲"));
            Assert.AreEqual(1, f.Src.Commands.SelectedWeapon);
            Assert.AreEqual(1, HelpTitles(f).Count);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void WeaponList_LevelAttribute_ShowsLevel()
        {
            // レベル指定付きの属性は「属性Lレベル」「名称レベルn」の形で表示する
            var f = new CommandUnitFormFixture(onMap: false);
            var u = CreateArmedUnit(f, "Ｍ直L2", 1, 3, onMap: false);
            f.MakeStatusViewUnitList();
            f.Src.Commands.SelectedUnit = u;
            var weaponAnswers = new Queue<UnitWeapon>(new[] { u.Weapons.First(), null });
            f.Gui.Handle("WeaponListBox", args => weaponAnswers.Dequeue());

            Run(f, Command.WeaponListCmdID, "武器一覧");

            var text = f.ListBoxes.Single().Items.Single().Text;
            StringAssert.StartsWith(text, "Ｍ直L2");
            StringAssert.EndsWith(text, "レベル2");
        }

        [TestMethod]
        [Ignore("移植の不具合: 武器一覧の属性リストの見出しが「武器属性一覧」ではなく「アビリティ属性一覧」になっている")]
        public void WeaponList_AttributeListCaptionIsWeaponAttributeList()
        {
            // 元の SRC は ListBox("武器属性一覧", …) を表示する
            var f = new CommandUnitFormFixture(onMap: false);
            var u = CreateArmedUnit(f, "Ｂ", 1, 3, onMap: false);
            f.MakeStatusViewUnitList();
            f.Src.Commands.SelectedUnit = u;
            var weaponAnswers = new Queue<UnitWeapon>(new[] { u.Weapons.First(), null });
            f.Gui.Handle("WeaponListBox", args => weaponAnswers.Dequeue());

            Run(f, Command.WeaponListCmdID, "武器一覧");

            Assert.AreEqual("武器属性一覧", f.ListBoxes.Single().lb_caption);
        }

        [TestMethod]
        public void WeaponList_OnMap_RangeOnly_ShowsRangeAreaUntilRightClick()
        {
            // マップ上で属性のない武器は「射程範囲」だけなので、直ちに射程範囲（敵のみ選択）を表示し、
            // クリック（右クリック）を待ってから武器一覧に戻る
            var f = new CommandUnitFormFixture(onMap: true);
            var u = CreateArmedUnit(f, "", 2, 3, onMap: true);
            var ally = f.CreateUnit("機体", "味方", x: 4, y: 6);
            var enemy = f.CreateUnit("機体", "敵", "敵", x: 6, y: 4);
            f.Src.Commands.SelectedUnit = u;
            var weaponAnswers = new Queue<UnitWeapon>(new[] { u.Weapons.First(), null });
            f.Gui.Handle("WeaponListBox", args => weaponAnswers.Dequeue());
            f.Gui.Handle("IsRButtonPressed", args => true);

            Run(f, Command.WeaponListCmdID, "武器一覧");

            Assert.AreEqual(0, f.ListBoxes.Count);
            var mask = f.Src.Map.MaskData;
            Assert.IsFalse(mask[6, 4], "射程内の敵");
            Assert.IsTrue(mask[4, 6], "射程内でも味方は選択しない");
            Assert.IsTrue(mask[5, 4], "最小射程未満");
            Assert.IsFalse(mask[4, 2], "射程内の空きマス");
            Assert.IsTrue(mask[4, 8], "最大射程外");
            Assert.IsFalse(mask[4, 4], "中心は常に選択");
            Assert.IsTrue(f.CountCalls("MaskScreen") > 0);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void WeaponList_OnMap_CrossMapWeapon_ShowsCrossArea()
        {
            // Ｍ直のマップ兵器は十字方向の射程範囲を表示する
            var f = new CommandUnitFormFixture(onMap: true);
            var u = CreateArmedUnit(f, "Ｍ直", 1, 2, onMap: true);
            f.Src.Commands.SelectedUnit = u;
            var weaponAnswers = new Queue<UnitWeapon>(new[] { u.Weapons.First(), null });
            f.Gui.Handle("WeaponListBox", args => weaponAnswers.Dequeue());
            f.Gui.Handle("IsRButtonPressed", args => true);
            // 属性一覧 → 「射程範囲」(2番目) → キャンセル
            var answers = new Queue<int>(new[] { 2, 0 });
            f.ListBoxAnswer = _ => answers.Dequeue();

            Run(f, Command.WeaponListCmdID, "武器一覧");

            Assert.AreEqual("射程範囲", f.ListBoxes[0].Items.Last().Text);
            var mask = f.Src.Map.MaskData;
            Assert.IsFalse(mask[4, 2]);
            Assert.IsFalse(mask[6, 4]);
            Assert.IsFalse(mask[2, 4]);
            Assert.IsFalse(mask[4, 6]);
            Assert.IsTrue(mask[5, 5], "斜めは範囲外");
            Assert.IsTrue(mask[4, 7], "最大射程外");
        }

        // ──────────────────────────────────────────────
        // アビリティ一覧
        // ──────────────────────────────────────────────

        [TestMethod]
        public void AbilityList_StatusView_SelectsAbilityAndShowsAttributes()
        {
            // アビリティを選ぶと SelectedAbility を設定し、属性一覧を表示する
            var f = new CommandUnitFormFixture(onMap: false);
            var ud = f.AddUnitData("機体");
            var a = ud.AddAbility("修理装置");
            a.MinRange = 1;
            a.MaxRange = 1;
            a.Class = "Ｐ";
            var u = f.CreateUnit("機体", "パイロット");
            f.MakeStatusViewUnitList();
            f.Src.Commands.SelectedUnit = u;
            var abilityAnswers = new Queue<UnitAbility>(new[] { u.Abilities.First(), null });
            f.Gui.Handle("AbilityListBox", args => abilityAnswers.Dequeue());
            var answers = new Queue<int>(new[] { 1, 0 });
            f.ListBoxAnswer = _ => answers.Dequeue();

            Run(f, Command.AbilityListCmdID, "アビリティ一覧");

            var lb = f.ListBoxes[0];
            Assert.AreEqual("アビリティ属性一覧", lb.lb_caption);
            StringAssert.StartsWith(lb.Items.Single().Text, "Ｐ");
            Assert.AreEqual(1, HelpTitles(f).Count);
            // キャンセルで終了したときは SelectedAbility を 0 に戻す
            Assert.AreEqual(0, f.Src.Commands.SelectedAbility);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
        }

        [TestMethod]
        public void AbilityList_OnMap_RangeOnly_ShowsRangeArea()
        {
            // マップ上で属性のないアビリティは射程範囲を直接表示する
            var f = new CommandUnitFormFixture(onMap: true);
            var ud = f.AddUnitData("機体");
            var a = ud.AddAbility("遠隔修理");
            a.MinRange = 1;
            a.MaxRange = 3;
            a.Class = "";
            var u = f.CreateUnit("機体", "パイロット", x: 4, y: 4);
            f.Src.Commands.SelectedUnit = u;
            var abilityAnswers = new Queue<UnitAbility>(new[] { u.Abilities.First(), null });
            f.Gui.Handle("AbilityListBox", args => abilityAnswers.Dequeue());
            f.Gui.Handle("IsRButtonPressed", args => true);

            Run(f, Command.AbilityListCmdID, "アビリティ一覧");

            Assert.AreEqual(0, f.ListBoxes.Count);
            var mask = f.Src.Map.MaskData;
            Assert.IsFalse(mask[4, 1]);
            Assert.IsFalse(mask[5, 5]);
            Assert.IsTrue(mask[4, 8]);
        }

        // ──────────────────────────────────────────────
        // 移動範囲・射程範囲
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ShowAreaInSpeed_MasksOutsideMoveRange()
        {
            // 「移動範囲」は移動可能範囲を表示してターゲット選択状態にする
            var f = new CommandUnitFormFixture(onMap: true, width: 12, height: 12);
            f.AddUnitData("機体"); // 移動力 4
            var u = f.CreateUnit("機体", "パイロット", x: 6, y: 6);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.MoveCmdID, "移動範囲");

            Assert.AreEqual("移動範囲", f.Src.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Src.Commands.CommandState);
            var mask = f.Src.Map.MaskData;
            // 平地の移動コストは 1（MoveCost 2 は 0.5 単位）なので移動力 4 で 4 マス
            Assert.IsFalse(mask[6, 2]);
            Assert.IsFalse(mask[8, 8]);
            Assert.IsTrue(mask[6, 1]);
            Assert.IsTrue(mask[9, 8]);
        }

        [TestMethod]
        public void ShowAreaInRange_UsesLongestNonMapWeapon()
        {
            // 「射程範囲」はマップ兵器（Ｍ属性）を除いた最大射程の武器の射程（最小射程 1）を表示する
            var f = new CommandUnitFormFixture(onMap: true, width: 12, height: 12);
            var ud = f.AddUnitData("機体");
            var w1 = ud.AddWeapon("ライフル");
            w1.Power = 1000; w1.MinRange = 2; w1.MaxRange = 3; w1.Class = ""; w1.Adaption = "AAAA";
            var w2 = ud.AddWeapon("マップ兵器");
            w2.Power = 1000; w2.MinRange = 1; w2.MaxRange = 5; w2.Class = "Ｍ全"; w2.Adaption = "AAAA";
            var u = f.CreateUnit("機体", "パイロット", x: 6, y: 6);
            var ally = f.CreateUnit("機体", "味方", x: 7, y: 6);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.CommandState = "コマンド選択";

            Run(f, Command.AttackCmdID, "射程範囲");

            Assert.AreEqual("射程範囲", f.Src.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Src.Commands.CommandState);
            var mask = f.Src.Map.MaskData;
            Assert.IsFalse(mask[6, 3], "射程3");
            Assert.IsTrue(mask[6, 2], "Ｍ属性の射程5は使わない");
            Assert.IsFalse(mask[6, 5], "最小射程は1として表示");
            Assert.IsTrue(mask[7, 6], "味方ユニットは選択しない");
        }
    }
}
