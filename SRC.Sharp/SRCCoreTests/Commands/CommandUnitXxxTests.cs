using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitxxx.cs (ChargeCommand / StartOrderCommand) のユニットテスト。
    ///
    /// ChargeCommand の動作:
    ///   1. GUI.LockGUI()
    ///   2. GUI.Confirm("チャージを開始しますか？") → キャンセルなら CancelCommand → GUI.UnlockGUI
    ///   3. OK なら 使用イベント発火 → チャージ処理 → CommandState = "ユニット選択"
    ///
    /// StartOrderCommand の動作:
    ///   1. GUI.LockGUI()
    ///   2. 命令リスト (自由/移動/攻撃/護衛/帰還) を構築
    ///   3. GUI.ListBox → キャンセル (ret=0) なら CancelCommand → GUI.UnlockGUI
    ///   4. ret=1 (自由) → SelectedUnit.Mode = "通常", CommandState = "ユニット選択"
    ///
    /// GUI 依存が深いため、ルーティングとキャンセル時の状態変化を確認する。
    /// </summary>
    [TestClass]
    public class CommandUnitXxxTests
    {
        /// <summary>
        /// IGUIStatus の no-op 実装 (Status.DisplayUnitStatus を必要とするパスで使用)
        /// </summary>
        private class NullGUIStatus : IGUIStatus
        {
            public Units.Unit DisplayedUnit { get; set; }
            public Pilots.Pilot DisplayedPilot { get; set; }
            public void DisplayGlobalStatus() { }
            public void DisplayUnitStatus(Units.Unit u, Pilots.Pilot p = null) { }
            public void DisplayPilotStatus(Pilots.Pilot p) { }
            public void InstantUnitStatusDisplay(int X, int Y) { }
            public void ClearUnitStatus() { }
        }

        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.GUIStatus = new NullGUIStatus();
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // ChargeCommand (ChargeCmdID)
        // Command.unitxxx.cs: ChargeCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChargeCmdID_ReachesGuiConfirm()
        {
            // ChargeCmdID → ChargeCommand() → GUI.Confirm が呼ばれる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;

            bool confirmCalled = false;
            mock.LockGUIHandler = () => { };
            mock.ConfirmHandler = (msg, title, opt) => { confirmCalled = true; return GuiDialogResult.Cancel; };
            mock.UnlockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.ChargeCmdID, "チャージ"));
            }
            catch (GUINotImplementedException) { }

            Assert.IsTrue(confirmCalled, "GUI.Confirm が呼ばれるはずです");
        }

        [TestMethod]
        public void ChargeCmdID_ConfirmCancel_CommandStateBecomesUnitSelect()
        {
            // チャージ確認でキャンセル → CancelCommand() → CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.ConfirmHandler = (msg, title, opt) => GuiDialogResult.Cancel;
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.ChargeCmdID, "チャージ"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void ChargeCmdID_ConfirmCancel_SelectedCommandCleared()
        {
            // チャージ確認でキャンセル → CancelCommand() → SelectedCommand がクリアされる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "チャージ前";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.ConfirmHandler = (msg, title, opt) => GuiDialogResult.Cancel;
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.ChargeCmdID, "チャージ"));

            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void ChargeCmdID_ConfirmMessage_ContainsChargeText()
        {
            // GUI.Confirm に渡されるメッセージが「チャージ」を含む
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            string capturedMessage = null;
            mock.LockGUIHandler = () => { };
            mock.ConfirmHandler = (msg, title, opt) => { capturedMessage = msg; return GuiDialogResult.Cancel; };
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.ChargeCmdID, "チャージ"));

            StringAssert.Contains(capturedMessage, "チャージ");
        }

        // ──────────────────────────────────────────────
        // StartOrderCommand (OrderCmdID)
        // Command.unitxxx.cs: StartOrderCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void OrderCmdID_ReachesGuiListBox()
        {
            // OrderCmdID → StartOrderCommand() → GUI.ListBox が呼ばれる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;

            bool listBoxCalled = false;
            mock.LockGUIHandler = () => { };
            mock.ListBoxHandler = args => { listBoxCalled = true; return 0; };
            mock.UnlockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));
            }
            catch (GUINotImplementedException) { }

            Assert.IsTrue(listBoxCalled, "GUI.ListBox が呼ばれるはずです");
        }

        [TestMethod]
        public void OrderCmdID_ListBoxItems_ContainsFourBaseItems()
        {
            // 命令一覧には少なくとも4つの基本選択肢 (自由/移動/攻撃/護衛) がある
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;

            int itemCount = -1;
            mock.LockGUIHandler = () => { };
            mock.ListBoxHandler = args => { itemCount = args.Items.Count; return 0; };
            mock.UnlockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));
            }
            catch (GUINotImplementedException) { }

            Assert.AreEqual(4, itemCount, "基本命令は4件 (自由/移動/攻撃/護衛) のはずです");
        }

        [TestMethod]
        public void OrderCmdID_Cancel_CommandStateBecomesUnitSelect()
        {
            // キャンセル (ret=0) → CancelCommand() → CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.ListBoxHandler = _ => 0;
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void OrderCmdID_Cancel_SelectedCommandCleared()
        {
            // キャンセル → CancelCommand() → SelectedCommand がクリアされる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "命令前";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.ListBoxHandler = _ => 0;
            mock.UnlockGUIHandler = () => { };

            src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));

            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void OrderCmdID_SelectJiyuu_UnitModeSetToNormal()
        {
            // 「自由」(ret=1) を選ぶと SelectedUnit.Mode が "通常" に設定される
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.ListBoxHandler = _ => 1; // case 1: 自由
            mock.UnlockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));
            }
            catch (GUINotImplementedException) { }

            Assert.AreEqual("通常", src.Commands.SelectedUnit.Mode);
        }

        [TestMethod]
        public void OrderCmdID_SelectJiyuu_CommandStateBecomesUnitSelect()
        {
            // 「自由」(ret=1) を選ぶと CommandState = "ユニット選択"
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.ListBoxHandler = _ => 1; // case 1: 自由
            mock.UnlockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));
            }
            catch (GUINotImplementedException) { }

            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void OrderCmdID_WithSummoner_ListBoxHasFiveItems()
        {
            // Summoner がいる場合は「帰還」が追加されて5件になる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            // SelectedUnit に Summoner を設定
            var summoner = new Unit(src);
            src.Commands.SelectedUnit.Summoner = summoner;

            var mock = (MockGUI)src.GUI;
            int itemCount = -1;
            mock.LockGUIHandler = () => { };
            mock.ListBoxHandler = args => { itemCount = args.Items.Count; return 0; };
            mock.UnlockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));
            }
            catch (GUINotImplementedException) { }

            Assert.AreEqual(5, itemCount, "Summoner がいる場合は帰還が追加され5件のはずです");
        }

        [TestMethod]
        public void OrderCmdID_WithMaster_ListBoxHasFiveItems()
        {
            // Master がいる場合も「帰還」が追加されて5件になる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            var master = new Unit(src);
            src.Commands.SelectedUnit.Master = master;

            var mock = (MockGUI)src.GUI;
            int itemCount = -1;
            mock.LockGUIHandler = () => { };
            mock.ListBoxHandler = args => { itemCount = args.Items.Count; return 0; };
            mock.UnlockGUIHandler = () => { };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.OrderCmdID, "命令"));
            }
            catch (GUINotImplementedException) { }

            Assert.AreEqual(5, itemCount, "Master がいる場合も帰還が追加され5件のはずです");
        }
    }
}
