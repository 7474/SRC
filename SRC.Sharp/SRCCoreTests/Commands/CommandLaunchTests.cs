using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.launch.cs (StartLaunchCommand / FinishLaunchCommand) のユニットテスト。
    ///
    /// StartLaunchCommand の動作:
    ///   1. SelectedUnit.UnitOnBoards から発進可能ユニットのリストを構築
    ///   2. GUI.TopItem = 1 を設定してから GUI.ListBox を呼ぶ
    ///   3. キャンセル (ret=0) の場合: CancelCommand() を呼んで終了
    ///   4. 選択した場合: SelectedCommand = "発進"、SelectedTarget にユニットを設定、
    ///      発進先のエリア計算を行い CommandState を変更
    ///
    /// GUI 依存が深いため、ルーティングと状態変化の観点でテストする。
    /// </summary>
    [TestClass]
    public class CommandLaunchTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // LaunchCmdID → StartLaunchCommand ルーティング確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LaunchCmdID_StartsLaunchCommand_ReachesGuiListBox()
        {
            // LaunchCmdID を指定すると StartLaunchCommand() が呼ばれ
            // GUI.ListBox に到達することを確認
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";

            bool listBoxCalled = false;
            ((MockGUI)src.GUI).ListBoxHandler = args => { listBoxCalled = true; return 0; };
            ((MockGUI)src.GUI).RedrawScreenHandler = _ => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));
            }
            catch (GUINotImplementedException) { }

            Assert.IsTrue(listBoxCalled, "GUI.ListBox が呼ばれるはずです");
        }

        [TestMethod]
        public void LaunchCmdID_TopItemSetToOne_BeforeListBox()
        {
            // StartLaunchCommand は GUI.ListBox を呼ぶ前に GUI.TopItem = 1 を設定する
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            ((MockGUI)src.GUI).TopItem = 99; // 初期値を 1 以外に設定

            int topItemWhenListBoxCalled = -1;
            ((MockGUI)src.GUI).ListBoxHandler = args =>
            {
                topItemWhenListBoxCalled = ((MockGUI)src.GUI).TopItem;
                return 0;
            };
            ((MockGUI)src.GUI).RedrawScreenHandler = _ => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));
            }
            catch (GUINotImplementedException) { }

            Assert.AreEqual(1, topItemWhenListBoxCalled, "GUI.TopItem は ListBox 呼び出し前に 1 に設定されるはずです");
        }

        [TestMethod]
        public void LaunchCmdID_EmptyUnitOnBoards_ListBoxCalledWithEmptyItems()
        {
            // UnitOnBoards が空の母艦では ListBox に空リストが渡される
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";

            int itemCount = -1;
            ((MockGUI)src.GUI).ListBoxHandler = args =>
            {
                itemCount = args.Items.Count;
                return 0;
            };
            ((MockGUI)src.GUI).RedrawScreenHandler = _ => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));
            }
            catch (GUINotImplementedException) { }

            Assert.AreEqual(0, itemCount, "UnitOnBoards が空なら Items は空のはずです");
        }

        // ──────────────────────────────────────────────
        // キャンセル時の状態変化
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LaunchCmdID_CancelFromListBox_CommandStateBecomesUnitSelect()
        {
            // ListBox でキャンセル (ret=0) → CancelCommand() → CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            ((MockGUI)src.GUI).ListBoxHandler = _ => 0;
            ((MockGUI)src.GUI).RedrawScreenHandler = _ => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));
            }
            catch (GUINotImplementedException) { }

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void LaunchCmdID_CancelFromListBox_SelectedCommandCleared()
        {
            // キャンセル → CancelCommand() → SelectedCommand がクリアされる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "発進前";
            ((MockGUI)src.GUI).ListBoxHandler = _ => 0;
            ((MockGUI)src.GUI).RedrawScreenHandler = _ => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));
            }
            catch (GUINotImplementedException) { }

            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LaunchCmdID_CancelFromListBox_CommandStateIsUnitSelect()
        {
            // CancelCommand() 後は CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            ((MockGUI)src.GUI).ListBoxHandler = _ => 0;
            ((MockGUI)src.GUI).RedrawScreenHandler = _ => { };

            try { src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進")); }
            catch (GUINotImplementedException) { }

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void LaunchCmdID_ListBoxCaption_IsUnitSelect()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            string captionUsed = null;
            ((MockGUI)src.GUI).ListBoxHandler = args => { captionUsed = args.lb_caption; return 0; };
            ((MockGUI)src.GUI).RedrawScreenHandler = _ => { };

            try { src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進")); }
            catch (GUINotImplementedException) { }

            Assert.AreEqual("ユニット選択", captionUsed);
        }

        [TestMethod]
        public void LaunchCmdID_ListBoxHasFlag_IsTrue()
        {
            // StartLaunchCommand は HasFlag=true でリストを表示する
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool? hasFlagValue = null;
            ((MockGUI)src.GUI).ListBoxHandler = args => { hasFlagValue = args.HasFlag; return 0; };
            ((MockGUI)src.GUI).RedrawScreenHandler = _ => { };

            try { src.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進")); }
            catch (GUINotImplementedException) { }

            Assert.AreEqual(true, hasFlagValue, "StartLaunchCommand は HasFlag=true を渡す");
        }

        [TestMethod]
        public void LaunchCmdID_InitialSelectedCommand_IsEmpty()
        {
            var src = CreateSrc();
            Assert.IsTrue(string.IsNullOrEmpty(src.Commands.SelectedCommand));
        }
    }
}
