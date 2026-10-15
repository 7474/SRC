using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Pilots;
using SRCCore.Units;
using SRCCore.Config;
using SRCCore.Filesystem;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Command.unitsp.cs（スペシャルパワーコマンド）のテスト。
    /// VB 版 Command.bas の StartSpecialPowerCommand / FinishSpecialPowerCommand に対応。
    /// </summary>
    [TestClass]
    public class CommandUnitOpsSpecialPowerTests
    {
        // エース機 (5,5)、損傷した味方 (5,6)、無傷の味方 (4,5)、敵 (6,5)
        private static CommandUnitOpsFixture CreateField(out Unit unit, out Pilot ace, out Unit damaged, out Unit intact, out Unit enemy)
        {
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("気合", "気", 10, "自分", "気力増加Lv1", kana: "きあい");
            f.DefineSpecialPower("信頼", "信", 20, "味方", "ＨＰ回復Lv5", kana: "しんらい");
            f.DefineSpecialPower("脱力", "脱", 30, "敵", "気力低下Lv1", kana: "だつりょく");
            f.DefineSpecialPower("高価", "高", 90, "自分", "気力増加Lv1", kana: "こうか");
            f.DefineUnit("エース機");
            f.DefineUnit("戦闘機");
            var pd = f.DefinePilot("エース", sp: 60);
            pd.AddSpecialPower("気合", 1, 0);
            pd.AddSpecialPower("信頼", 1, 0);
            pd.AddSpecialPower("脱力", 1, 0);
            pd.AddSpecialPower("高価", 1, 0);
            unit = f.Place("エース機", "エース", "味方", 5, 5);
            damaged = f.Place("戦闘機", "損傷", "味方", 5, 6);
            intact = f.Place("戦闘機", "無傷", "味方", 4, 5);
            enemy = f.Place("戦闘機", "敵兵", "敵", 6, 5);
            damaged.HP = 100;
            ace = unit.MainPilot();
            ace.SP = 60;
            f.Commands.SelectedUnit = unit;
            f.Commands.CommandState = "コマンド選択";
            return f;
        }

        // ListBox の呼び出しを記録し、あらかじめ決めた戻り値を順に返す
        private static List<ListBoxArgs> ScriptListBox(CommandUnitOpsFixture f, params int[] results)
        {
            var calls = new List<ListBoxArgs>();
            var queue = new Queue<int>(results);
            f.Gui.Handle("ListBox", args =>
            {
                calls.Add((ListBoxArgs)args[0]);
                return queue.Count > 0 ? queue.Dequeue() : 0;
            });
            return calls;
        }

        private static void ClickTarget(CommandUnitOpsFixture f, int x, int y)
        {
            f.Commands.ProceedCommand(false, GuiButton.Left, f.Cell(x, y), f.Src.Map.MapDataForUnit[x, y]);
        }

        // ──────────────────────────────────────────────
        // スペシャルパワーの選択
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Start_SinglePilot_ListsSpecialPowersWithCostAndAvailability()
        {
            // パイロットが一人なら選択を省略し、スペシャルパワー一覧（名称・消費ＳＰ・解説）を表示する
            var f = CreateField(out _, out _, out _, out _, out _);
            var calls = ScriptListBox(f, 0);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            var args = calls.Single();
            Assert.AreEqual("スペシャルパワー選択", args.lb_caption);
            Assert.AreEqual("カーソル移動(行きのみ)", args.lb_mode);
            Assert.IsTrue(args.HasFlag);
            StringAssert.StartsWith(args.lb_info, "名称         消費SP（エース SP=60/");
            // 名称を 13 桁、消費ＳＰを 3 桁に揃える
            StringAssert.StartsWith(args.Items[0].Text, "気合          10 ");
            StringAssert.EndsWith(args.Items[0].Text, "気合の解説");
            Assert.IsFalse(args.Items[0].ListItemFlag);
            Assert.IsTrue(args.Items[3].ListItemFlag, "ＳＰが足りないスペシャルパワーは選べない");
        }

        [TestMethod]
        public void Start_SpecialPowerList_SeparatesCommentWithTwoSpaces()
        {
            // VB: RightPaddedString(sname, 13) & LeftPaddedString(Format$(cost), 3) & "  " & Comment
            var f = CreateField(out _, out _, out _, out _, out _);
            var calls = ScriptListBox(f, 0);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.AreEqual("気合          10  気合の解説", calls.Single().Items[0].Text);
        }

        [TestMethod]
        public void Start_Cancel_ReturnsToUnitSelect()
        {
            // スペシャルパワー選択をキャンセルするとステータスを表示し直してコマンドを取り消す
            var f = CreateField(out var unit, out var ace, out _, out _, out _);
            ScriptListBox(f, 0);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.AreSame(unit, f.Calls("DisplayUnitStatus").First().Args[0]);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.AreEqual(60, ace.SP);
        }

        [TestMethod]
        public void Start_SelfTarget_UsesImmediately()
        {
            // 対象が「自分」のスペシャルパワーはその場で使用し、ＳＰを消費する
            var f = CreateField(out var unit, out var ace, out _, out _, out _);
            var morale = ace.Morale;
            ScriptListBox(f, 1);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.AreEqual(50, ace.SP);
            Assert.AreEqual(morale + 10, ace.Morale, "気力増加Lv1 は気力 +10");
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.AreEqual("", f.Commands.SelectedSpecialPower);
            Assert.AreSame(unit, f.Calls("DisplayUnitStatus").Last().Args[0]);
            Assert.AreEqual(0, unit.UsedAction, "スペシャルパワーの使用では行動終了しない");
        }

        // ──────────────────────────────────────────────
        // ターゲットを選ぶスペシャルパワー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Start_AllyTarget_MasksIneligibleUnits()
        {
            // 対象「味方」: 味方・ＮＰＣで効果のあるユニットだけが選択可能
            var f = CreateField(out _, out _, out _, out _, out _);
            ScriptListBox(f, 2);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            var mask = f.Src.Map.MaskData;
            Assert.IsFalse(mask[5, 6], "損傷した味方");
            Assert.IsTrue(mask[4, 5], "ＨＰ満タンの味方には効果がない");
            Assert.IsTrue(mask[6, 5], "敵は対象外");
            Assert.IsTrue(mask[1, 1], "誰もいないマス");
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.AreEqual("信頼", f.Commands.SelectedSpecialPower);
            CollectionAssert.Contains(f.Calls("DisplaySysMessage").Select(x => (string)x.Args[0]).ToList(),
                "エースは信頼を使った。;ターゲットを選んでください。");
        }

        [TestMethod]
        public void Start_EnemyTarget_MasksAllies()
        {
            // 対象「敵」: 味方・ＮＰＣは選べない
            var f = CreateField(out _, out _, out _, out _, out _);
            ScriptListBox(f, 3);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            var mask = f.Src.Map.MaskData;
            Assert.IsFalse(mask[6, 5]);
            Assert.IsTrue(mask[5, 6]);
            Assert.IsTrue(mask[5, 5]);
        }

        [TestMethod]
        public void Finish_AllyTarget_AppliesEffectAndConsumesSP()
        {
            // ターゲットを選ぶとＳＰを消費して効果を適用し、ユニット選択に戻る
            var f = CreateField(out var unit, out var ace, out var damaged, out _, out _);
            ScriptListBox(f, 2);
            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            ClickTarget(f, 5, 6);

            Assert.AreEqual(40, ace.SP);
            Assert.AreEqual(600, damaged.HP, "ＨＰ回復Lv5 は最大ＨＰの 50%（500）を回復");
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.AreEqual("", f.Commands.SelectedSpecialPower);
            Assert.AreSame(damaged, f.Calls("DisplayUnitStatus").Last().Args[0]);
            Assert.AreEqual(0, unit.UsedAction);
        }

        [TestMethod]
        public void Finish_SelfDestruct_ConfirmCancel_DoesNotUse()
        {
            // 自爆効果のスペシャルパワーは確認を取り、キャンセルならＳＰを消費しない
            var f = CreateField(out var unit, out var ace, out _, out _, out var enemy);
            f.DefineSpecialPower("特攻", "特", 10, "敵", "自爆");
            f.Gui.Handle("Confirm", args => GuiDialogResult.Cancel);
            f.Commands.SelectedCommand = "スペシャルパワー";
            f.Commands.SelectedSpecialPower = "特攻";
            f.Commands.SelectedPilot = ace;
            f.Commands.CommandState = "ターゲット選択";
            f.ClearMask(true);
            f.Src.Map.MaskData[6, 5] = false;

            ClickTarget(f, 6, 5);

            Assert.AreEqual("自爆させますか？", f.Calls("Confirm").Single().Args[0]);
            Assert.AreEqual(60, ace.SP);
            Assert.AreEqual("出撃", unit.Status);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void Start_SelfDestruct_ConfirmCancel_DoesNotUse()
        {
            // VB: If ret = vbCancel Then UnlockGUI: Exit Sub（キャンセル時のみ中止）
            var f = CreateField(out var unit, out var ace, out _, out _, out _);
            var pd = f.Src.PDList.Item("エース");
            f.DefineSpecialPower("自滅", "滅", 10, "自分", "自爆");
            pd.AddSpecialPower("自滅", 1, 0);
            f.Gui.Handle("Confirm", args => GuiDialogResult.Cancel);
            ScriptListBox(f, 5);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.AreEqual(60, ace.SP);
            Assert.AreEqual("出撃", unit.Status);
        }

        // ──────────────────────────────────────────────
        // 複数のパイロット
        // ──────────────────────────────────────────────

        private static CommandUnitOpsFixture CreateTwinField(out Unit unit, out Pilot main, out Pilot sub)
        {
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("気合", "気", 10, "自分", "気力増加Lv1");
            f.DefineUnit("複座機").PilotNum = 2;
            var pdMain = f.DefinePilot("メイン", sp: 40);
            pdMain.AddSpecialPower("気合", 1, 0);
            var pdSub = f.DefinePilot("サブ", sp: 40);
            pdSub.AddSpecialPower("気合", 1, 0);
            unit = f.Place("複座機", "メイン", "味方", 5, 5);
            sub = f.Src.PList.Add("サブ", 1, "味方");
            sub.Ride(unit);
            unit.Update();
            main = unit.MainPilot();
            f.Commands.SelectedUnit = unit;
            f.Commands.CommandState = "コマンド選択";
            return f;
        }

        [TestMethod]
        public void Start_TwoPilots_AsksWhichPilot()
        {
            // スペシャルパワーを持つパイロットが複数いれば誰が使うかを選ぶ
            var f = CreateTwinField(out _, out var main, out var sub);
            var calls = ScriptListBox(f, 2, 0);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.AreEqual("パイロット選択", calls[0].lb_caption);
            Assert.AreEqual("連続表示,カーソル移動", calls[0].lb_mode);
            Assert.AreEqual(2, calls[0].Items.Count);
            StringAssert.StartsWith(calls[0].Items[1].Text, "サブ");
            Assert.AreSame(sub, f.Commands.SelectedPilot);
            Assert.AreSame(sub, f.Calls("DisplayPilotStatus").Single().Args[0]);
            StringAssert.Contains(calls[1].lb_info, "（サブ SP=");
        }

        [TestMethod]
        public void Start_TwoPilots_CancelPilotSelection()
        {
            var f = CreateTwinField(out _, out _, out _);
            var calls = ScriptListBox(f, 0);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.AreEqual(1, calls.Count, "スペシャルパワーの選択には進まない");
            Assert.IsTrue(f.Called("CloseListBox"));
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        [Ignore("移植の不具合: スペシャルパワーを使えるパイロットの一覧から最初のサポートパイロットが漏れる（Unit.PilotsHaveSpecialPower の Supports.Skip(1)）")]
        public void Start_SupportPilot_IsSelectable()
        {
            // VB: For i = 1 To .CountPilot + .CountSupport … Support(i - .CountPilot)（全サポートが対象）
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("気合", "気", 10, "自分", "気力増加Lv1");
            f.DefineUnit("機体");
            f.DefinePilot("メイン", sp: 40).AddSpecialPower("気合", 1, 0);
            f.DefinePilot("サポート", sp: 40).AddSpecialPower("気合", 1, 0);
            var unit = f.Place("機体", "メイン", "味方", 5, 5);
            f.Src.PList.Add("サポート", 1, "味方").Ride(unit, true);
            unit.Update();
            f.Commands.SelectedUnit = unit;
            f.Commands.CommandState = "コマンド選択";
            var calls = ScriptListBox(f, 0);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.AreEqual("パイロット選択", calls[0].lb_caption);
            Assert.AreEqual(2, calls[0].Items.Count);
        }

        // ──────────────────────────────────────────────
        // 破壊された味方を対象にするスペシャルパワー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Start_DestroyedAllyTarget_ListsDestroyedAllies()
        {
            // 対象「破壊味方」: 破壊された味方ユニットの一覧から選ぶ。キャンセルならコマンドを取り消す
            var f = CreateField(out _, out var ace, out var damaged, out _, out var enemy);
            f.DefineSpecialPower("復活", "復", 50, "破壊味方", "復活");
            f.Src.PDList.Item("エース").AddSpecialPower("復活", 1, 0);
            damaged.Status = "破壊";
            enemy.Status = "破壊";
            var calls = ScriptListBox(f, 5, 0);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.AreEqual("ユニット選択", calls[1].lb_caption);
            Assert.AreEqual(damaged.ID, calls[1].Items.Single().ListItemID, "破壊された敵は含まない");
            StringAssert.StartsWith(calls[1].Items[0].Text, "戦闘機");
            StringAssert.Contains(calls[1].Items[0].Text, "損傷");
            CollectionAssert.Contains(f.Calls("DisplaySysMessage").Select(x => (string)x.Args[0]).ToList(),
                "エースは復活を使った。;復活させるユニットを選んでください。");
            Assert.AreEqual(60, ace.SP);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // 味方スペシャルパワー実行
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Start_AllySpecialPowerExecution_UsesOtherPilotsPowerAtDoubleCost()
        {
            // 味方スペシャルパワー実行: 出撃中の味方が修得しているスペシャルパワーを消費ＳＰ２倍で使う
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("気合", "気", 10, "自分", "気力増加Lv1", kana: "きあい");
            f.DefineSpecialPower("奇跡", "奇", 30, "自分", "気力増加Lv1", kana: "きせき");
            f.DefineSpecialPower("友情", "友", 5, "自分", "味方スペシャルパワー実行", kana: "ゆうじょう");
            f.DefineUnit("機体");
            f.DefinePilot("仲間", sp: 50).AddSpecialPower("気合", 1, 0);
            f.DefinePilot("友人", sp: 50).AddSpecialPower("友情", 1, 0);
            f.Place("機体", "仲間", "味方", 2, 2);
            var unit = f.Place("機体", "友人", "味方", 5, 5);
            var friend = unit.MainPilot();
            friend.SP = 50;
            f.Commands.SelectedUnit = unit;
            f.Commands.CommandState = "コマンド選択";
            ScriptListBox(f, 1);
            ListBoxArgs spArgs = null;
            f.Gui.Handle("MultiColumnListBox", args => { spArgs = (ListBoxArgs)args[0]; return 1; });

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            // 味方スペシャルパワー実行そのものは一覧に含まない
            CollectionAssert.AreEqual(new[] { "気合", "奇跡" }, spArgs.Items.Select(x => x.ListItemID).ToList());
            Assert.IsFalse(spArgs.Items[0].ListItemFlag);
            Assert.IsTrue(spArgs.Items[1].ListItemFlag, "誰も修得していない");
            Assert.AreEqual(50 - 2 * 10, friend.SP);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void Start_AllySpecialPowerExecution_Cancel()
        {
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("友情", "友", 5, "自分", "味方スペシャルパワー実行");
            f.DefineUnit("機体");
            f.DefinePilot("友人", sp: 50).AddSpecialPower("友情", 1, 0);
            var unit = f.Place("機体", "友人", "味方", 5, 5);
            unit.MainPilot().SP = 50;
            f.Commands.SelectedUnit = unit;
            f.Commands.CommandState = "コマンド選択";
            ScriptListBox(f, 1);
            f.Gui.Handle("MultiColumnListBox", args => 0);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.AreEqual("", f.Commands.SelectedSpecialPower);
            Assert.AreEqual(50, unit.MainPilot().SP);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void Start_TwoPilots_PersonScale_AsksWhichCharacter()
        {
            // 等身大基準では「キャラクター選択」になる
            var f = CreateTwinField(out _, out _, out _);
            f.Src.Expression.DefineGlobalVariable("Option(等身大基準)");
            var calls = ScriptListBox(f, 0);

            f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

            Assert.AreEqual("キャラクター選択", calls[0].lb_caption);
            StringAssert.StartsWith(calls[0].lb_info, "キャラクター");
        }

        // ──────────────────────────────────────────────
        // 使用・使用後イベント
        // ──────────────────────────────────────────────

        // イベントファイルを一時ディレクトリに書いて読み込む（呼び出し側で削除する）
        private static string LoadEvents(CommandUnitOpsFixture f, params string[] lines)
        {
            var dir = Path.Combine(Path.GetTempPath(), "srcs-sp-" + Guid.NewGuid());
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
        public void Start_CanceledByUseEvent_DoesNotUse()
        {
            // 使用イベントで Cancel された場合はスペシャルパワーを使わない（VB StartSpecialPowerCommand: If IsCanceled Then … Exit Sub）
            var f = CreateField(out var unit, out var ace, out _, out _, out _);
            var dir = LoadEvents(f, "使用 エース 気合:", "Cancel", "Exit");
            try
            {
                var morale = ace.Morale;
                ScriptListBox(f, 1);

                f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

                Assert.AreEqual(60, ace.SP);
                Assert.AreEqual(morale, ace.Morale);
                Assert.IsFalse(f.Src.IsCanceled, "キャンセルフラグは下ろす");
                Assert.AreEqual(0, unit.UsedAction);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [TestMethod]
        public void Start_AfterUseEvent_RunsAfterEffect()
        {
            // 使用後イベントはスペシャルパワーの効果を適用した後に実行される
            var f = CreateField(out _, out var ace, out _, out _, out _);
            var dir = LoadEvents(f, "使用後 エース 気合:", "Set 使用後ＳＰ SP(エース)", "Exit");
            try
            {
                ScriptListBox(f, 1);

                f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

                Assert.AreEqual(50, f.Src.Expression.GetValueAsLong("使用後ＳＰ"));
                Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [TestMethod]
        public void Finish_CanceledByUseEvent_DoesNotApplyEffect()
        {
            // ターゲット選択後の使用イベントで Cancel された場合はＳＰを消費せず効果も出ない（VB FinishSpecialPowerCommand）
            var f = CreateField(out _, out var ace, out var damaged, out _, out _);
            var dir = LoadEvents(f, "使用 エース 信頼:", "Cancel", "Exit");
            try
            {
                ScriptListBox(f, 2);
                f.Commands.UnitCommand(new UiCommand(Command.SpecialPowerCmdID, "スペシャルパワー"));

                ClickTarget(f, 5, 6);

                Assert.AreEqual(60, ace.SP);
                Assert.AreEqual(100, damaged.HP);
                Assert.AreEqual("ユニット選択", f.Commands.CommandState);
                Assert.IsFalse(f.Src.IsCanceled);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }
    }
}
