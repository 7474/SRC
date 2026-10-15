using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Config;
using SRCCore.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Command.mapcommend.cs（マップコマンド）のテスト。
    /// VB 版 Command.bas の MapCommand / EndTurnCommand / UnitListCommand /
    /// SearchSpecialPowerCommand / RestartCommand / QuickLoadCommand / QuickSaveCommand /
    /// DumpCommand / GlobalMapCommand に対応。
    /// </summary>
    [TestClass]
    public class CommandUnitOpsMapCommandTests
    {
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

        // 味方 3 機（うち 1 機は行動済み、1 機は格納中）と敵 1 機
        private static CommandUnitOpsFixture CreateUnitListField(out Unit alpha, out Unit beta, out Unit stored, out Unit enemy)
        {
            var f = new CommandUnitOpsFixture();
            f.DefineUnit("アルファ機", hp: 1000, en: 100);
            f.DefineUnit("ベータ機", hp: 3000, en: 200);
            f.DefineUnit("敵機");
            alpha = f.Place("アルファ機", "アルファ", "味方", 2, 2, level: 10);
            beta = f.Place("ベータ機", "ベータ", "味方", 7, 8, level: 5);
            stored = f.Place("アルファ機", "ガンマ", "味方", 3, 3, level: 1);
            enemy = f.Place("敵機", "敵兵", "敵", 9, 9, level: 3);
            beta.UsedAction = beta.MaxAction();
            f.Src.Map.MapDataForUnit[3, 3] = null;
            stored.Status = "格納";
            return f;
        }

        // ──────────────────────────────────────────────
        // MapCommand の振り分け
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapCommand_UnknownId_Throws()
        {
            // 定義されていないマップコマンドは例外にする（移植版独自の防御）
            var f = new CommandUnitOpsFixture();
            Assert.Throws<NotSupportedException>(() => f.Commands.MapCommand(new UiCommand(9999, "不明")));
        }

        [TestMethod]
        public void MapCommand_GlobalMap_DisplaysGlobalMapAndResetsState()
        {
            // マップコマンド実行時は常にユニット選択状態に戻る
            var f = new CommandUnitOpsFixture();
            f.Commands.CommandState = "マップコマンド";
            f.Src.IsScenarioFinished = true;

            f.Commands.MapCommand(new UiCommand(Command.GlobalMapCmdID, "全体マップ"));

            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
            Assert.IsTrue(f.Called("DisplayGlobalMap"));
            Assert.IsFalse(f.Src.IsScenarioFinished, "実行後は IsScenarioFinished を下ろす");
            var members = f.Gui.Calls.Select(x => x.Member).Where(x => x == "LockGUI" || x == "DisplayGlobalMap" || x == "UnlockGUI").ToList();
            CollectionAssert.AreEqual(new[] { "LockGUI", "DisplayGlobalMap", "UnlockGUI" }, members);
        }

        [TestMethod]
        public void MapCommand_OperationObject_RedrawsScreen()
        {
            // 作戦目的: 勝利条件イベントを実行して画面を再描画
            var f = new CommandUnitOpsFixture();

            f.Commands.MapCommand(new UiCommand(Command.OperationObjectCmdID, "作戦目的"));

            Assert.IsTrue(f.Called("RedrawScreen"));
            Assert.IsTrue(f.Called("UnlockGUI"));
        }

        [TestMethod]
        public void MapCommand_Configuration_OpensConfigureDialog()
        {
            var f = new CommandUnitOpsFixture();

            f.Commands.MapCommand(new UiCommand(Command.ConfigurationCmdID, "設定変更"));

            Assert.IsTrue(f.Called("Configure"));
        }

        [TestMethod]
        public void MapCommand_AutoDefense_TogglesAndSavesConfig()
        {
            // 自動反撃モードの切り替えは設定ファイルに保存される（VB: WriteIni "Option", "AutoDefense"）
            var dir = Path.Combine(Path.GetTempPath(), "srcs-autodef-" + Guid.NewGuid());
            Directory.CreateDirectory(dir);
            try
            {
                var f = new CommandUnitOpsFixture();
                var config = (LocalFileConfig)f.Src.SystemConfig;
                config.AppPath = dir;
                Assert.IsFalse(config.AutoDefense);

                f.Commands.MapCommand(new UiCommand(Command.AutoDefenseCmdID, "自動反撃モード"));

                Assert.IsTrue(config.AutoDefense);
                var saved = File.ReadAllText(Path.Combine(dir, "srcs.json"));
                StringAssert.Contains(saved, "AutoDefense");

                f.Commands.MapCommand(new UiCommand(Command.AutoDefenseCmdID, "自動反撃モード"));
                Assert.IsFalse(config.AutoDefense);
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        // ──────────────────────────────────────────────
        // ターン終了
        // ──────────────────────────────────────────────

        [TestMethod]
        public void EndTurn_InViewMode_OnlyLeavesViewMode()
        {
            // 閲覧モード中のターン終了は閲覧モードを抜けるだけ
            var f = new CommandUnitOpsFixture();
            f.Commands.ViewMode = true;

            f.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));

            Assert.IsFalse(f.Commands.ViewMode);
            Assert.IsFalse(f.Called("Confirm"));
        }

        [TestMethod]
        public void EndTurn_WithIdleUnits_Cancel_KeepsTurn()
        {
            // 行動していない味方ユニット（出撃・格納中）の数を示して確認し、キャンセルならターンを終了しない
            var f = CreateUnitListField(out _, out var beta, out _, out _);
            string message = null;
            f.Gui.Handle("Confirm", args => { message = (string)args[0]; return GuiDialogResult.Cancel; });
            f.Src.Stage = "味方";

            f.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));

            Assert.AreEqual("行動していないユニットが2体あります" + Environment.NewLine + "このターンを終了しますか？", message);
            Assert.AreEqual(1, beta.UsedAction, "行動済みのまま");
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        [Timeout(10000)]
        public void EndTurn_Ok_RunsPhasesAndReturnsToPlayerPhase()
        {
            // 確認後は敵・中立・ＮＰＣのフェイズを経て味方フェイズに戻り、行動数が回復する
            var f = CreateUnitListField(out var alpha, out var beta, out _, out var enemy);
            f.Src.Map.MapDataForUnit[9, 9] = null;
            enemy.Status = "破壊";
            f.Gui.Handle("Confirm", args => GuiDialogResult.Ok);

            f.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));

            Assert.AreEqual("味方", f.Src.Stage);
            Assert.AreEqual(0, beta.UsedAction);
            Assert.IsFalse(f.Src.IsScenarioFinished);
        }

        [TestMethod]
        [Timeout(10000)]
        public void EndTurn_AllUnitsActed_DoesNotConfirm()
        {
            // 全員行動済みなら確認なしでターンを終了する
            var f = new CommandUnitOpsFixture();
            f.DefineUnit("機体");
            var u = f.Place("機体", "乗り手", "味方", 2, 2);
            u.UsedAction = u.MaxAction();

            f.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));

            Assert.IsFalse(f.Called("Confirm"));
            Assert.AreEqual("味方", f.Src.Stage);
            Assert.AreEqual(0, u.UsedAction);
        }

        // ──────────────────────────────────────────────
        // 部隊表（UnitListCommand）
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitList_ListsPlayerUnitsSortedByLevel()
        {
            // 既定は味方の出撃・格納中ユニットをパイロットレベル順に表示する
            var f = CreateUnitListField(out var alpha, out var beta, out var stored, out _);
            var calls = ScriptListBox(f, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            var args = calls.Single();
            Assert.AreEqual("味方ユニット一覧", args.lb_caption);
            Assert.AreEqual("連続表示", args.lb_mode);
            Assert.AreEqual("ユニット               パイロット Lv     ＨＰ    ＥＮ", args.lb_info);
            CollectionAssert.AreEqual(
                new[] { "▽陣営変更・並べ替え▽", alpha.ID, beta.ID, stored.ID },
                args.Items.Select(x => x.ListItemID).ToList());
            // ユニット名(23桁) パイロット名(34桁まで) レベル(2桁) を 37 桁に揃え、ＨＰ／ＥＮを続ける
            Assert.AreEqual("アルファ機             アルファ   10  1000/ 1000 100/100", args.Items[1].Text);
            StringAssert.EndsWith(args.Items[2].Text, "済", "行動済みのユニット");
            StringAssert.EndsWith(args.Items[3].Text, "格", "格納中のユニット");
        }

        [TestMethod]
        public void UnitList_Cancel_ClosesListAndUnlocks()
        {
            var f = CreateUnitListField(out _, out _, out _, out _);
            ScriptListBox(f, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            Assert.IsTrue(f.Called("CloseListBox"));
            Assert.IsTrue(f.Called("ReduceListBoxHeight"));
            Assert.IsTrue(f.Called("EnlargeListBoxWidth"));
            Assert.IsTrue(f.Called("UnlockGUI"));
            Assert.IsFalse(f.Called("Center"));
        }

        [TestMethod]
        public void UnitList_SelectUnit_CentersAndShowsStatus()
        {
            // 選択したユニットを画面中央に表示してステータスを表示する
            var f = CreateUnitListField(out _, out var beta, out _, out _);
            ((LocalFileConfig)f.Src.SystemConfig).AutoMoveCursor = true;
            ScriptListBox(f, 3);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            var center = f.Calls("Center").Single();
            Assert.AreEqual(7, center.Args[0]);
            Assert.AreEqual(8, center.Args[1]);
            Assert.AreSame(beta, f.Calls("DisplayUnitStatus").Single().Args[0]);
            Assert.AreSame(beta, f.Calls("MoveCursorPos").Single().Args[1]);
        }

        [TestMethod]
        public void UnitList_ChangeParty_ShowsEnemyList()
        {
            // 先頭項目から陣営を切り替えると、その陣営の一覧を表示し直す
            var f = CreateUnitListField(out _, out _, out _, out var enemy);
            var calls = ScriptListBox(f, 1, 3, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual("選択", calls[1].lb_caption);
            Assert.AreEqual("一覧表示方法", calls[1].lb_info);
            Assert.AreEqual("敵一覧", calls[1].Items[2].ListItemID);
            Assert.AreEqual("敵ユニット一覧", calls[2].lb_caption);
            CollectionAssert.AreEqual(new[] { "▽陣営変更・並べ替え▽", enemy.ID }, calls[2].Items.Select(x => x.ListItemID).ToList());
        }

        [TestMethod]
        public void UnitList_SortByUnitName_UsesKanaName()
        {
            // ユニット名称で並べ替えると読み仮名の昇順になる
            var f = CreateUnitListField(out var alpha, out var beta, out var stored, out _);
            f.Src.UDList.Item("アルファ機").KanaName = "ん";
            f.Src.UDList.Item("ベータ機").KanaName = "あ";
            var calls = ScriptListBox(f, 1, 8, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            Assert.AreEqual("ユニット名称で並べ替え", calls[1].Items[7].ListItemID);
            Assert.AreEqual(beta.ID, calls[2].Items[1].ListItemID);
        }

        [TestMethod]
        [Ignore("移植の不具合: 部隊表のＨＰ／ＥＮ順の並べ替えが現在値ではなく最大値で行われる")]
        public void UnitList_SortByHP_UsesCurrentHP()
        {
            // VB: key_list(i) = .Item(id_list(i)).HP（現在のＨＰの降順）
            var f = CreateUnitListField(out var alpha, out var beta, out _, out _);
            alpha.HP = 900;
            beta.HP = 100;
            var calls = ScriptListBox(f, 1, 5, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            Assert.AreEqual(alpha.ID, calls[2].Items[1].ListItemID);
        }

        [TestMethod]
        [Ignore("移植の不具合: 部隊表の並べ替えメニューに「パイロット名称で並べ替え」がなく「ユニット名称で並べ替え」が重複している")]
        public void UnitList_SortMenu_HasPilotNameSort()
        {
            // VB: mode_list(5..9) = ＨＰ, ＥＮ, パイロットレベル, ユニット名称, パイロット名称
            var f = CreateUnitListField(out _, out _, out _, out _);
            var calls = ScriptListBox(f, 1, 0, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            Assert.AreEqual(
                string.Join(",", new[] { "味方一覧", "ＮＰＣ一覧", "敵一覧", "中立一覧", "ＨＰで並べ替え", "ＥＮで並べ替え", "パイロットレベルで並べ替え", "ユニット名称で並べ替え", "パイロット名称で並べ替え" }),
                string.Join(",", calls[1].Items.Select(x => x.ListItemID)));
        }

        [TestMethod]
        [Ignore("移植の不具合: 等身大基準の部隊表の並べ替えメニューで「ＥＮで並べ替え」が「スペシャルパワーで並べ替え」になっている")]
        public void UnitList_PersonScale_SortMenu_HasENSort()
        {
            // VB: 等身大基準では mode_list(8) = Term("ＥＮ") & "で並べ替え"
            var f = CreateUnitListField(out _, out _, out _, out _);
            f.Src.Expression.DefineGlobalVariable("Option(等身大基準)");
            var calls = ScriptListBox(f, 1, 0, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            Assert.AreEqual("ＥＮで並べ替え", calls[1].Items[7].ListItemID);
        }

        [TestMethod]
        public void UnitList_DummyUnit_ShowsPilotStatusList()
        {
            // ダミーユニットに乗ったパイロットはパイロット一覧（レベル・ＳＰ・スペシャルパワー）で表示する
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("集中", "集", 10, "自分", "気力増加Lv1");
            f.DefineUnit("ダミー", features: "ダミーユニット");
            var pd = f.DefinePilot("隊長", sp: 50);
            pd.AddSpecialPower("集中", 1, 0);
            var u = f.Place("ダミー", "隊長", "味方", 2, 2, level: 3);
            u.MainPilot().SP = 40;
            var calls = ScriptListBox(f, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            var args = calls.Single();
            Assert.AreEqual("味方パイロット一覧", args.lb_caption);
            StringAssert.StartsWith(args.Items[1].Text, "隊長");
            StringAssert.Contains(args.Items[1].Text, "40/" + u.MainPilot().MaxSP);
            StringAssert.EndsWith(args.Items[1].Text, "集");
        }

        [TestMethod]
        [Ignore("移植の不具合: パイロット一覧で残りＳＰがちょうど消費ＳＰと等しいスペシャルパワーが使用可能として表示されない")]
        public void UnitList_DummyUnit_ShowsSpecialPowerWhenSPEqualsCost()
        {
            // VB: If .SP >= .SpecialPowerCost(.SpecialPower(i)) Then（等しい場合も表示）
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("集中", "集", 50, "自分", "気力増加Lv1");
            f.DefineUnit("ダミー", features: "ダミーユニット");
            var pd = f.DefinePilot("隊長", sp: 50);
            pd.AddSpecialPower("集中", 1, 0);
            f.Place("ダミー", "隊長", "味方", 2, 2, level: 3).MainPilot().SP = 50;
            var calls = ScriptListBox(f, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            StringAssert.EndsWith(calls.Single().Items[1].Text, "集");
        }

        // ──────────────────────────────────────────────
        // スペシャルパワー検索
        // ──────────────────────────────────────────────

        private static CommandUnitOpsFixture CreateSPField(out Unit unit, out Pilots.Pilot pilot)
        {
            var f = new CommandUnitOpsFixture();
            f.DefineSpecialPower("熱血", "熱", 40, "自分", "気力増加Lv1", kana: "ねっけつ");
            f.DefineSpecialPower("集中", "集", 15, "自分", "気力増加Lv1", kana: "しゅうちゅう");
            f.DefineSpecialPower("イベント専用", "非表示", 0, "自分", "気力増加Lv1", kana: "あ");
            f.DefineUnit("機体");
            var pd = f.DefinePilot("エース", sp: 30);
            pd.AddSpecialPower("集中", 1, 0);
            unit = f.Place("機体", "エース", "味方", 4, 6);
            pilot = unit.MainPilot();
            pilot.SP = 30;
            return f;
        }

        [TestMethod]
        public void SearchSpecialPower_ListsSpecialPowersSortedByKana_WithAvailability()
        {
            // 非表示のスペシャルパワーを除き読み仮名順に並べ、使えるパイロットがいないものは選択不可にする
            var f = CreateSPField(out _, out _);
            ListBoxArgs spArgs = null;
            f.Gui.Handle("MultiColumnListBox", args => { spArgs = (ListBoxArgs)args[0]; return 0; });

            f.Commands.MapCommand(new UiCommand(Command.SearchSpecialPowerCmdID, "スペシャルパワー検索"));

            Assert.AreEqual("スペシャルパワー検索", spArgs.lb_caption);
            CollectionAssert.AreEqual(new[] { "集中", "熱血" }, spArgs.Items.Select(x => x.ListItemID).ToList());
            Assert.IsFalse(spArgs.Items[0].ListItemFlag);
            Assert.IsTrue(spArgs.Items[1].ListItemFlag, "誰も使えない");
            Assert.AreEqual("集中の解説", spArgs.Items[0].ListItemComment);
            Assert.IsTrue(f.Called("UnlockGUI"));
        }

        [TestMethod]
        public void SearchSpecialPower_SelectPilot_CentersOnUnit()
        {
            // スペシャルパワーを選ぶと使用できるパイロットの一覧を出し、選んだパイロットの機体を中央に表示する
            var f = CreateSPField(out var unit, out var pilot);
            f.Gui.Handle("MultiColumnListBox", args => 1);
            var calls = ScriptListBox(f, 1);

            f.Commands.MapCommand(new UiCommand(Command.SearchSpecialPowerCmdID, "スペシャルパワー検索"));

            var args = calls.Single();
            Assert.AreEqual("パイロット選択", args.lb_caption);
            Assert.AreEqual(pilot.ID, args.Items.Single().ListItemID);
            StringAssert.StartsWith(args.Items[0].Text, "エース");
            StringAssert.Contains(args.Items[0].Text, "30/" + pilot.MaxSP);
            var center = f.Calls("Center").Single();
            Assert.AreEqual(4, center.Args[0]);
            Assert.AreEqual(6, center.Args[1]);
            Assert.AreSame(unit, f.Calls("DisplayUnitStatus").Single().Args[0]);
            Assert.AreEqual("", f.Commands.SelectedSpecialPower);
        }

        [TestMethod]
        public void SearchSpecialPower_ShortSPAndActed_AreNoted()
        {
            // ＳＰが足りない・行動済みのパイロットはその旨を付記する。パイロット一覧でキャンセルすると検索に戻る
            var f = CreateSPField(out var unit, out var pilot);
            pilot.SP = 5;
            unit.UsedAction = unit.MaxAction();
            var spCalls = 0;
            f.Gui.Handle("MultiColumnListBox", args => ++spCalls == 1 ? 1 : 0);
            var calls = ScriptListBox(f, 0);

            f.Commands.MapCommand(new UiCommand(Command.SearchSpecialPowerCmdID, "スペシャルパワー検索"));

            StringAssert.Contains(calls.Single().Items[0].Text, " ＳＰ不足");
            StringAssert.EndsWith(calls.Single().Items[0].Text, " 行動済");
            Assert.AreEqual(2, spCalls, "パイロット選択をキャンセルするとスペシャルパワー選択に戻る");
            Assert.IsFalse(f.Called("Center"));
        }

        // ──────────────────────────────────────────────
        // リスタート・クイックロード・クイックセーブ・中断
        // ──────────────────────────────────────────────

        [TestMethod]
        public void Restart_Cancel_DoesNotRestore()
        {
            var f = new CommandUnitOpsFixture();
            string title = null;
            f.Gui.Handle("Confirm", args => { title = (string)args[1]; return GuiDialogResult.Cancel; });

            f.Commands.MapCommand(new UiCommand(Command.RestartCmdID, "リスタート"));

            Assert.AreEqual("リスタート", title);
            Assert.IsFalse(f.Called("LockGUI"));
        }

        [TestMethod]
        public void QuickLoad_Cancel_DoesNotOpenStream()
        {
            var f = new CommandUnitOpsFixture();
            string message = null;
            f.Gui.Handle("Confirm", args => { message = (string)args[0]; return GuiDialogResult.Cancel; });

            f.Commands.MapCommand(new UiCommand(Command.QuickLoadCmdID, "クイックロード"));

            Assert.AreEqual("データをロードしますか？", message);
            Assert.IsFalse(f.Called("OpenQuikSaveStream"));
        }

        [TestMethod]
        public void QuickSave_WritesQuickSaveData()
        {
            // クイックセーブは確認なしで書き込み、砂時計カーソルを元に戻す
            var f = new CommandUnitOpsFixture();
            f.DefineUnit("機体");
            f.Place("機体", "乗り手", "味方", 2, 2);
            var stream = new KeepOpenMemoryStream();
            f.Gui.Handle("OpenQuikSaveStream", args => stream);

            f.Commands.MapCommand(new UiCommand(Command.QuickSaveCmdID, "クイックセーブ"));

            Assert.AreEqual(FileAccess.Write, f.Calls("OpenQuikSaveStream").Single().Args[0]);
            Assert.IsTrue(stream.ToArray().Length > 0);
            var statuses = f.Calls("ChangeStatus").Select(x => x.Args[0]).ToList();
            CollectionAssert.AreEqual(new object[] { GuiStatus.WaitCursor, GuiStatus.Default }, statuses);
            Assert.IsFalse(f.Called("Confirm"));
        }

        [TestMethod]
        public void Dump_Cancel_DoesNotSave()
        {
            var f = new CommandUnitOpsFixture();
            f.Gui.Handle("Confirm", args => GuiDialogResult.Cancel);

            f.Commands.MapCommand(new UiCommand(Command.DumpCmdID, "中断"));

            Assert.IsFalse(f.Called("SelectSaveStream"));
            Assert.IsFalse(f.Called("MainFormHide"));
        }

        [TestMethod]
        public void Dump_CancelFileSelection_DoesNotExit()
        {
            // 保存先の選択をキャンセルした場合はゲームを終了しない。保存名は「シナリオ名を中断.srcq」
            var f = new CommandUnitOpsFixture();
            f.Src.ScenarioFileName = Path.Combine("scenario", "第1話.eve");
            f.Gui.Handle("Confirm", args => GuiDialogResult.Ok);

            f.Commands.MapCommand(new UiCommand(Command.DumpCmdID, "中断"));

            var select = f.Calls("SelectSaveStream").Single();
            Assert.AreEqual(SRCSaveKind.Suspend, select.Args[0]);
            Assert.AreEqual("第1話を中断.srcq", select.Args[1]);
            Assert.IsFalse(f.Called("MainFormHide"));
            Assert.IsFalse(f.Gui.IsTerminated);
        }

        private class KeepOpenMemoryStream : MemoryStream
        {
            protected override void Dispose(bool disposing)
            {
                // 内容を検証できるよう閉じない
            }
        }

        [TestMethod]
        public void UnitList_PersonScale_UsesUnitNameAndLevelColumns()
        {
            // 等身大基準ではユニット名(33桁)とレベル(3桁)の書式になる
            var f = CreateUnitListField(out var alpha, out _, out _, out _);
            f.Src.Expression.DefineGlobalVariable("Option(等身大基準)");
            var calls = ScriptListBox(f, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            var args = calls.Single();
            Assert.AreEqual("ユニット名                        Lv     ＨＰ    ＥＮ", args.lb_info);
            Assert.AreEqual("アルファ機                        10  1000/ 1000 100/100", args.Items[1].Text);
        }

        [TestMethod]
        public void UnitList_UnknownData_HidesHPAndEN()
        {
            // データ不明のユニットはＨＰ・ＥＮを伏せる
            var f = CreateUnitListField(out var alpha, out _, out _, out _);
            alpha.AddCondition("データ不明", -1, cdata: "");
            var calls = ScriptListBox(f, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            StringAssert.EndsWith(calls.Single().Items[1].Text, "?????/????? ???/???");
        }

        [TestMethod]
        public void UnitList_HiddenUnit_IsNotListed()
        {
            // ユニット情報隠蔽状態のユニットは一覧に表示しない
            var f = CreateUnitListField(out var alpha, out _, out _, out _);
            alpha.AddCondition("ユニット情報隠蔽", -1, cdata: "");
            var calls = ScriptListBox(f, 0);

            f.Commands.MapCommand(new UiCommand(Command.UnitListCmdID, "部隊表"));

            CollectionAssert.DoesNotContain(calls.Single().Items.Select(x => x.ListItemID).ToList(), alpha.ID);
        }
    }
}
