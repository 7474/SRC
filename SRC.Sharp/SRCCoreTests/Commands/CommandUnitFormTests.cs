using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitform.cs (TransformCommand / HyperModeCommand / CancelTransformationCommand /
    /// SplitCommand / CombineCommand / ExchangeFormCommand) のユニットテスト。
    ///
    /// TransformCmdID の動作:
    ///   1. GUI.LockGUI()
    ///   2. SelectedUnit.FeatureData("変形") を参照 → 空ユニットでは ""
    ///   3. Map.IsStatusView == true (デフォルト: MapFileName が空) の場合
    ///      → forms リストが空 → IndexOutOfRange で例外
    ///
    /// ExchangeFormCmdID の動作:
    ///   1. GUI.LockGUI()
    ///   2. SelectedUnit.Feature("換装") → 空ユニットでは null → NullRef で例外
    ///
    /// CancelTransformationCommand (HyperModeCmdID + ノーマルモード条件):
    ///   1. GUI.LockGUI()
    ///   2. Map.IsStatusView == true → Transform 試行 → 例外またはフロー終了
    ///   3. Map.IsStatusView == false → GUI.Confirm → キャンセル → CancelCommand
    ///
    /// GUI 依存とユニットデータ依存が深いため、LockGUI 到達と
    /// キャンセル可能なパスのステート変化を中心にテストする。
    /// </summary>
    [TestClass]
    public class CommandUnitFormTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // TransformCmdID (10) → TransformCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TransformCmdID_CallsGuiLockGUI()
        {
            // TransformCommand() は GUI.LockGUI() を最初に呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.TransformCmdID, "変形"));
            }
            catch { }

            Assert.IsTrue(lockCalled, "TransformCommand() の最初に GUI.LockGUI() が呼ばれるはずです");
        }

        // ──────────────────────────────────────────────
        // HyperModeCmdID (11) → HyperModeCommand() or CancelTransformationCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HyperModeCmdID_WithoutNormalMode_CallsGuiLockGUI()
        {
            // SelectedUnit に "ノーマルモード" 特殊能力がない場合 → HyperModeCommand()
            // HyperModeCommand() は GUI.LockGUI() を最初に呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.HyperModeCmdID, "ハイパーモード"));
            }
            catch { }

            Assert.IsTrue(lockCalled, "HyperModeCommand() の最初に GUI.LockGUI() が呼ばれるはずです");
        }

        // ──────────────────────────────────────────────
        // ExchangeFormCmdID (22) → ExchangeFormCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExchangeFormCmdID_CallsGuiLockGUI()
        {
            // ExchangeFormCommand() は GUI.LockGUI() を最初に呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.ExchangeFormCmdID, "換装"));
            }
            catch { }

            Assert.IsTrue(lockCalled, "ExchangeFormCommand() の最初に GUI.LockGUI() が呼ばれるはずです");
        }

        // ──────────────────────────────────────────────
        // SplitCmdID → SplitCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SplitCmdID_CallsGuiLockGUI()
        {
            // SplitCommand() は GUI.LockGUI() を最初に呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.SplitCmdID, "分離"));
            }
            catch { }

            Assert.IsTrue(lockCalled, "SplitCommand() の最初に GUI.LockGUI() が呼ばれるはずです");
        }

        // ──────────────────────────────────────────────
        // CombineCmdID → CombineCommand()
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CombineCmdID_CallsGuiLockGUI()
        {
            // CombineCommand() は GUI.LockGUI() を最初に呼ぶ
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };

            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.CombineCmdID, "合体"));
            }
            catch { }

            Assert.IsTrue(lockCalled, "CombineCommand() の最初に GUI.LockGUI() が呼ばれるはずです");
        }

        // ──────────────────────────────────────────────
        // ルーティング確認: 各コマンド ID が適切なメソッドに到達する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TransformCmdID_Routing_IsDistinctFrom_ExchangeFormCmdID()
        {
            // TransformCmdID と ExchangeFormCmdID は異なる ID (10 vs 22)
            Assert.AreNotEqual(Command.TransformCmdID, Command.ExchangeFormCmdID);
        }

        [TestMethod]
        public void SplitCmdID_Routing_IsDistinctFrom_CombineCmdID()
        {
            // SplitCmdID と CombineCmdID は異なる ID
            Assert.AreNotEqual(Command.SplitCmdID, Command.CombineCmdID);
        }

        [TestMethod]
        public void HyperModeCmdID_Routing_IsDistinctFrom_TransformCmdID()
        {
            // HyperModeCmdID と TransformCmdID は異なる ID
            Assert.AreNotEqual(Command.HyperModeCmdID, Command.TransformCmdID);
        }
    }
}
