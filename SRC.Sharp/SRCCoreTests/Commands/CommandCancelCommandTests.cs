using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.CancelCommand() のユニットテスト (Command.process.cs)
    ///
    /// CancelCommand は CommandState によって分岐する:
    /// - null / "" / "ユニット選択" → 何もしない (no-op)
    /// - "コマンド選択" → CommandState を "ユニット選択" に戻し SelectedCommand をクリア
    /// - その他 (GUI 依存が強いため詳細テストは省略)
    /// </summary>
    [TestClass]
    public class CommandCancelCommandTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // CommandState == null (未初期化)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CancelCommand_NullCommandState_DoesNotThrow()
        {
            // null は switch の ?? "" で "" (= "ユニット選択" 相当) として扱われる
            var src = CreateSrc();
            src.Commands.CommandState = null;
            src.Commands.CancelCommand(); // 例外なし
            Assert.IsNull(src.Commands.CommandState);
        }

        [TestMethod]
        public void CancelCommand_NullCommandState_StateRemainsNull()
        {
            var src = CreateSrc();
            src.Commands.CommandState = null;
            src.Commands.CancelCommand();
            Assert.IsNull(src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // CommandState == "ユニット選択"
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CancelCommand_UnitSelectState_StateUnchanged()
        {
            // "ユニット選択" ではコマンドは何もしない
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";
            src.Commands.CancelCommand();
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void CancelCommand_UnitSelectState_SelectedCommandUnchanged()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "ユニット選択";
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.CancelCommand();
            // SelectedCommand は変更されない
            Assert.AreEqual("攻撃", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // CommandState == "コマンド選択"
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CancelCommand_CommandSelectState_SetsStateToUnitSelect()
        {
            // "コマンド選択" のキャンセルは "ユニット選択" に戻る
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.CancelCommand();
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void CancelCommand_CommandSelectState_ClearsSelectedCommand()
        {
            // SelectedCommand が空文字にリセットされる
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.CancelCommand();
            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void CancelCommand_CommandSelectState_WhenSelectedCommandIsEmpty_RemainsEmpty()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.SelectedCommand = "";
            src.Commands.CancelCommand();
            Assert.AreEqual("", src.Commands.SelectedCommand);
        }

        [TestMethod]
        public void CancelCommand_CommandSelectState_CalledTwice_StaysAtUnitSelect()
        {
            // 2回呼んでも "ユニット選択" のまま
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            src.Commands.CancelCommand();
            src.Commands.CancelCommand(); // 2回目は "ユニット選択" → no-op
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }
    }
}
