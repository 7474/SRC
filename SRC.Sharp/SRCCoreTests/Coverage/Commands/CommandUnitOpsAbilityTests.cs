using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Config;
using SRCCore.Units;
using SRCCore.Filesystem;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Command.unitability.cs（アビリティコマンド）のテスト。
    /// VB 版 Command.bas の StartAbilityCommand / FinishAbilityCommand / MapAbilityCommand に対応。
    /// </summary>
    [TestClass]
    public class CommandUnitOpsAbilityTests
    {
        // 回復役 (5,5)、損傷した味方 (5,6)、無傷の味方 (4,5)、損傷した味方（遠方） (8,5)
        private static CommandUnitOpsFixture CreateField(string aclass, int minRange, int maxRange, string effect,
            out Unit healer, out Unit damaged, out Unit intact, out Unit far)
        {
            var f = new CommandUnitOpsFixture();
            var ud = f.DefineUnit("回復機");
            var ad = ud.AddAbility("リペア");
            ad.Class = aclass;
            ad.MinRange = minRange;
            ad.MaxRange = maxRange;
            ad.ENConsumption = 10;
            ad.SetEffect(effect);
            f.DefineUnit("戦闘機", hp: 2000);
            healer = f.Place("回復機", "回復役", "味方", 5, 5);
            damaged = f.Place("戦闘機", "損傷", "味方", 5, 6);
            intact = f.Place("戦闘機", "無傷", "味方", 4, 5);
            far = f.Place("戦闘機", "遠方", "味方", 8, 5);
            damaged.HP = 1000;
            far.HP = 1000;
            f.Commands.SelectedUnit = healer;
            f.Commands.CommandState = "コマンド選択";
            // アビリティ選択ダイアログでは最初のアビリティを選ぶ
            f.Gui.Handle("AbilityListBox", args => ((Unit)args[0]).Abilities.First());
            return f;
        }

        private static void ClickTarget(CommandUnitOpsFixture f, int x, int y)
        {
            f.Commands.ProceedCommand(false, GuiButton.Left, f.Cell(x, y), f.Src.Map.MapDataForUnit[x, y]);
        }

        // ──────────────────────────────────────────────
        // StartAbilityCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartAbility_MasksUnitsWhereAbilityIsNotEffective()
        {
            // 射程内のユニットのうち、効果のないユニット（ＨＰ満タン）はマスクされる
            var f = CreateField("", 1, 1, "回復Lv1", out var healer, out _, out _, out _);

            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            var mask = f.Src.Map.MaskData;
            Assert.AreEqual(1, f.Commands.SelectedAbility);
            Assert.AreEqual("アビリティ", f.Commands.SelectedCommand);
            Assert.IsFalse(mask[5, 6], "損傷した味方は対象にできる");
            Assert.IsTrue(mask[4, 5], "無傷の味方には効果がない");
            Assert.IsTrue(mask[8, 5], "射程外");
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            var center = f.Calls("Center").Single();
            Assert.AreEqual(5, center.Args[0]);
            Assert.AreEqual(5, center.Args[1]);
            // 選択ダイアログには「移動前」モードとアビリティ名の見出しを渡す
            var listBox = f.Calls("AbilityListBox").Single();
            Assert.AreEqual("アビリティ選択", listBox.Args[2]);
            Assert.AreEqual("移動前", listBox.Args[3]);
            Assert.AreEqual(false, listBox.Args[4]);
        }

        [TestMethod]
        public void StartAbility_AfterMove_UsesAfterMoveModeAndState()
        {
            var f = CreateField("", 1, 1, "回復Lv1", out _, out _, out _, out _);
            f.Commands.CommandState = "移動後コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            Assert.AreEqual("移動後", f.Calls("AbilityListBox").Single().Args[3]);
            Assert.AreEqual("移動後ターゲット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void StartAbility_SupportOnly_CannotTargetSelf()
        {
            // 「援」属性のアビリティは自分自身には使用できない
            var f = CreateField("援", 0, 1, "回復Lv1", out var healer, out _, out _, out _);
            healer.HP = 100;

            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            Assert.IsTrue(f.Src.Map.MaskData[5, 5]);
            Assert.IsFalse(f.Src.Map.MaskData[5, 6]);
        }

        [TestMethod]
        public void StartAbility_MapAbility_SetsMapAbilityCommand()
        {
            // Ｍ属性のアビリティはマップアビリティとして扱う
            var f = CreateField("Ｍ全", 1, 2, "回復Lv1", out _, out _, out _, out _);

            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            Assert.AreEqual("マップアビリティ", f.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void StartAbility_AutoMoveCursor_PicksNearestSelectableAlly()
        {
            // AutoMoveCursor 有効時は選択可能な味方のうち最も近いユニットにカーソルを合わせる
            var f = CreateField("", 1, 5, "回復Lv1", out _, out var damaged, out _, out var far);
            ((LocalFileConfig)f.Src.SystemConfig).AutoMoveCursor = true;

            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            Assert.IsFalse(f.Src.Map.MaskData[8, 5], "射程 5 なら遠方の味方も選べる");
            Assert.AreSame(damaged, f.Calls("MoveCursorPos").Single().Args[1]);
            Assert.AreSame(damaged, f.Calls("DisplayUnitStatus").Last().Args[0]);
        }

        [TestMethod]
        public void StartAbility_Cancel_RestoresCursorWhenAutoMove()
        {
            // キャンセル時、AutoMoveCursor ならカーソル位置を元に戻してコマンド選択を取り消す
            var f = CreateField("", 1, 1, "回復Lv1", out _, out _, out _, out _);
            ((LocalFileConfig)f.Src.SystemConfig).AutoMoveCursor = true;
            f.Gui.Handle("AbilityListBox", args => null);

            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            Assert.IsTrue(f.Called("RestoreCursorPos"));
            Assert.AreEqual(0, f.Commands.SelectedAbility);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // 射程 0 のアビリティ（その場で実行）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartAbility_RangeZero_ExecutesOnSelfImmediately()
        {
            // 射程 0 のアビリティは対象選択なしに自分に対して実行し、行動終了する
            var f = CreateField("", 0, 0, "回復Lv1", out var healer, out _, out _, out _);
            healer.HP = 100;

            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            Assert.AreEqual(600, healer.HP);
            Assert.AreEqual(90, healer.EN);
            Assert.AreEqual("リペア", f.Commands.SelectedAbilityName);
            Assert.AreEqual(1, healer.UsedAction);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // FinishAbilityCommand
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FinishAbility_AppliesEffectToTargetAndEndsAction()
        {
            // 対象を選ぶとアビリティを実行し（回復Lv1 = ＨＰ+500、EN 消費）、行動終了する
            var f = CreateField("", 1, 1, "回復Lv1", out var healer, out var damaged, out _, out _);
            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            ClickTarget(f, 5, 6);

            Assert.AreEqual(1500, damaged.HP);
            Assert.AreEqual(90, healer.EN);
            Assert.AreEqual("リペア", f.Commands.SelectedAbilityName);
            Assert.AreEqual(1, healer.UsedAction);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.IsTrue(f.Called("ClearUnitStatus"));
        }

        [TestMethod]
        public void FinishAbility_ClickOnMaskedUnit_DoesNothing()
        {
            // 効果のないユニットを選んでも実行されない
            var f = CreateField("", 1, 1, "回復Lv1", out var healer, out _, out var intact, out _);
            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            ClickTarget(f, 4, 5);

            Assert.AreEqual(100, healer.EN);
            Assert.AreEqual(0, healer.UsedAction);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void FinishAbility_RaidSkill_AllowsRemoveAfterRangeOneAbility()
        {
            // 「遊撃」を持つパイロットは射程１（Ｑ属性なし）のアビリティ使用後に再移動できる
            var f = CreateField("", 1, 1, "回復Lv1", out var healer, out var damaged, out _, out _);
            f.Src.PDList.Item("回復役").AddSkill("遊撃", 1, "", 1);
            healer.Update();
            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            ClickTarget(f, 5, 6);

            Assert.AreEqual(1500, damaged.HP);
            Assert.AreEqual("再移動", f.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.AreEqual(0, healer.UsedAction, "再移動が残っているので行動終了しない");
            Assert.IsFalse(f.Src.Map.MaskData[5, 3], "移動範囲が表示される");
        }

        [TestMethod]
        public void FinishAbility_RaidSkill_QuickAbility_DoesNotAllowRemove()
        {
            // Ｑ属性の射程１アビリティは再移動の対象外
            var f = CreateField("Ｑ", 1, 1, "回復Lv1", out var healer, out _, out _, out _);
            f.Src.PDList.Item("回復役").AddSkill("遊撃", 1, "", 1);
            healer.Update();
            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            ClickTarget(f, 5, 6);

            Assert.AreEqual(1, healer.UsedAction);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // MapAbilityCommand（目標地点を選ぶタイプ）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapAbility_Throw_SelectsPointAndShowsEffectArea()
        {
            // Ｍ投: 目標地点を選ぶと効果範囲を表示し「マップ攻撃使用」状態で再度確認を求める
            var f = CreateField("Ｍ投L1", 1, 3, "回復Lv1", out var healer, out _, out _, out _);
            f.Gui.Handle("PixelToMapX", args => 5);
            f.Gui.Handle("PixelToMapY", args => 7);
            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));
            Assert.AreEqual("マップアビリティ", f.Commands.SelectedCommand);

            ClickTarget(f, 5, 7);

            Assert.AreEqual("マップ攻撃使用", f.Commands.CommandState);
            Assert.AreEqual(5, f.Commands.SelectedX);
            Assert.AreEqual(7, f.Commands.SelectedY);
            // 効果範囲は目標地点から Lv マス
            Assert.IsFalse(f.Src.Map.MaskData[5, 6]);
            Assert.IsFalse(f.Src.Map.MaskData[5, 8]);
            Assert.IsTrue(f.Src.Map.MaskData[5, 9]);
            Assert.AreEqual(0, healer.UsedAction);
        }

        [TestMethod]
        public void MapAbility_Line_AfterMove_SelectsPoint()
        {
            // Ｍ線: 移動後に使った場合は「移動後マップ攻撃使用」になる
            var f = CreateField("Ｍ線", 1, 3, "回復Lv1", out _, out _, out _, out _);
            f.Gui.Handle("PixelToMapX", args => 5);
            f.Gui.Handle("PixelToMapY", args => 8);
            f.Commands.CommandState = "移動後コマンド選択";
            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));
            f.ClearMask();

            ClickTarget(f, 5, 8);

            Assert.AreEqual("移動後マップ攻撃使用", f.Commands.CommandState);
            Assert.AreEqual(8, f.Commands.SelectedY);
            Assert.IsFalse(f.Src.Map.MaskData[5, 7], "自分と目標地点を結ぶ線上");
            Assert.IsTrue(f.Src.Map.MaskData[6, 7], "線から外れた地点");
        }

        [TestMethod]
        public void MapAbility_Move_OccupiedPoint_StaysInTargetSelect()
        {
            // Ｍ移: ユニットがいる地点は移動先に選べないので状態を変えない
            var f = CreateField("Ｍ移", 1, 3, "回復Lv1", out _, out _, out _, out _);
            f.Gui.Handle("PixelToMapX", args => 5);
            f.Gui.Handle("PixelToMapY", args => 6);
            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));
            f.ClearMask();

            ClickTarget(f, 5, 6);

            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void MapAbility_Move_EmptyPoint_GoesToMapAttackUse()
        {
            // Ｍ移: 空いている地点を選ぶと移動経路を効果範囲として表示する
            var f = CreateField("Ｍ移", 1, 3, "回復Lv1", out _, out _, out _, out _);
            f.Gui.Handle("PixelToMapX", args => 3);
            f.Gui.Handle("PixelToMapY", args => 5);
            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));
            f.ClearMask();

            ClickTarget(f, 3, 5);

            Assert.AreEqual("マップ攻撃使用", f.Commands.CommandState);
            Assert.AreEqual(3, f.Commands.SelectedX);
            Assert.IsFalse(f.Src.Map.MaskData[4, 5]);
            Assert.IsTrue(f.Src.Map.MaskData[5, 7]);
        }

        // ──────────────────────────────────────────────
        // MapAbilityCommand（その場で効果範囲が決まるタイプ）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapAbility_AllRange_HealsUnitsInAreaAndEndsAction()
        {
            // Ｍ全: 効果範囲内のユニット全員に効果を与え、行動終了する
            var f = CreateField("Ｍ全", 1, 2, "回復Lv1", out var healer, out var damaged, out _, out var far);
            f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

            ClickTarget(f, 5, 6);

            Assert.AreEqual(1500, damaged.HP);
            Assert.AreEqual(1000, far.HP, "射程外のユニットには効果がない");
            Assert.AreEqual(90, healer.EN);
            Assert.AreEqual(1, healer.UsedAction);
            Assert.IsNull(f.Commands.SelectedTarget);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // 使用イベント・使用後イベントでのキャンセル
        // ──────────────────────────────────────────────

        // イベントファイルを読み込ませる。作成した一時ディレクトリを返す
        private static string LoadEvents(CommandUnitOpsFixture f, params string[] lines)
        {
            var dir = Path.Combine(Path.GetTempPath(), "srcs-ability-" + Guid.NewGuid());
            Directory.CreateDirectory(dir);
            var file = Path.Combine(dir, "test.eve");
            File.WriteAllText(file, string.Join("\r\n", lines) + "\r\n", new UTF8Encoding(false));
            ((LocalFileConfig)f.Src.SystemConfig).AppPath = dir;
            var fs = new LocalFileSystem();
            fs.AddPath(dir);
            f.Src.FileSystem = fs;
            f.Src.ScenarioPath = dir;
            f.Src.Event.InitEventData();
            f.Src.Event.LoadEventData(file);
            return dir;
        }

        [TestMethod]
        public void FinishAbility_CanceledByUseEvent_EndsActionWithoutEffect()
        {
            // 使用イベントで Cancel された場合はアビリティを実行せずに行動終了する（VB: IsCanceled → WaitCommand）
            var f = CreateField("", 1, 1, "回復Lv1", out var healer, out var damaged, out _, out _);
            var dir = LoadEvents(f, "使用 回復役 リペア:", "Cancel", "Exit");
            try
            {
                f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

                ClickTarget(f, 5, 6);

                Assert.AreEqual(1000, damaged.HP);
                Assert.AreEqual(100, healer.EN);
                Assert.AreEqual(1, healer.UsedAction);
                Assert.AreEqual("ユニット選択", f.Commands.CommandState);
                Assert.IsFalse(f.Src.IsCanceled);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [TestMethod]
        public void MapAbility_CanceledByAfterUseEvent_EndsAction()
        {
            // VB MapAbilityCommand: If IsCanceled Then IsCanceled = False: ReDim SelectedPartners(0): WaitCommand: Exit Sub
            var f = CreateField("Ｍ全", 1, 2, "回復Lv1", out var healer, out var damaged, out _, out _);
            var dir = LoadEvents(f, "使用後 回復役 リペア:", "Cancel", "Exit");
            try
            {
                f.Commands.UnitCommand(new UiCommand(Command.AbilityCmdID, "リペア"));

                ClickTarget(f, 5, 6);

                Assert.AreEqual(1500, damaged.HP);
                Assert.AreEqual(1, healer.UsedAction);
                Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
