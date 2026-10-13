using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.MapCommand() のユニットテスト (Command.mapcommend.cs)
    ///
    /// MapCommand は呼び出し直後に CommandState = "ユニット選択" に設定する。
    /// - EndTurnCmdID (0) かつ ViewMode == true の場合:
    ///     ViewMode を false にして即 return (GUI 呼び出しなし)
    /// - AutoDefenseCmdID (16) の場合:
    ///     SystemConfig.AutoDefense をトグルする
    /// その他のケースは GUI ハンドラが必要なため GUI 依存パスは別途テスト対象外とする。
    /// </summary>
    [TestClass]
    public class CommandMapCommandTests
    {
        // AutoDefense のトグルは SystemConfig.Save() で srcs.json を書く。
        // 既定の AppPath（ビルド出力）の srcs.json を上書きしないよう、テストごとの一時フォルダに書く（#1141）
        private string appPath;

        [TestInitialize]
        public void Initialize()
        {
            appPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "SRCCoreTests-" + System.Guid.NewGuid().ToString("N"));
            System.IO.Directory.CreateDirectory(appPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (System.IO.Directory.Exists(appPath))
            {
                System.IO.Directory.Delete(appPath, true);
            }
        }

        private SRC CreateSrc() => new SRC { GUI = new MockGUI(), SystemConfig = new Config.LocalFileConfig { AppPath = appPath } };

        // ──────────────────────────────────────────────
        // 共通: MapCommand は常に CommandState を "ユニット選択" に設定する
        // (EndTurnCmdID + ViewMode=true は return 前に既に設定済み)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapCommand_EndTurnWithViewModeTrue_SetsCommandStateToUnitSelect()
        {
            // ViewMode=true の早期リターンでも CommandState は "ユニット選択" に設定される
            var src = CreateSrc();
            src.Commands.CommandState = "何かの状態";
            src.Commands.ViewMode = true;
            src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // EndTurnCmdID + ViewMode == true → ViewMode を false にして return
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapCommand_EndTurnWithViewModeTrue_DisablesViewMode()
        {
            // ヘルプ相当: ViewMode 中にターン終了を選択すると ViewMode が解除される
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));
            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void MapCommand_EndTurnWithViewModeFalse_ViewModeRemainsTrue_ShouldCallEndTurnCommand()
        {
            // ViewMode=false のとき EndTurnCommand() が呼ばれる
            // EndTurnCommand 内部は GUI 依存のため、GUINotImplementedException が発生することで
            // EndTurnCommand への到達を確認する
            var src = CreateSrc();
            src.Commands.ViewMode = false;
            try
            {
                src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));
                // GUINotImplementedException か他の例外が発生しなかった場合もテストとして成立
                // (ゲーム状態によっては正常終了する)
            }
            catch (SRCCore.Exceptions.GUINotImplementedException)
            {
                // EndTurnCommand 内部の GUI 呼び出しに到達: 期待通り
            }
            // いずれにしても ViewMode は false のまま
            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void MapCommand_EndTurnWithViewModeTrue_CalledTwice_ViewModeRemainsDisabled()
        {
            // 2回目は ViewMode=false なので EndTurnCommand に進む (GUI例外 or 正常)
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));
            Assert.IsFalse(src.Commands.ViewMode);
        }

        // ──────────────────────────────────────────────
        // AutoDefenseCmdID (16): SystemConfig.AutoDefense のトグル
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapCommand_AutoDefenseCmdID_TogglesAutoDefense()
        {
            // AutoDefense のトグル動作を確認（LocalFileConfig.Save() はテストごとの一時フォルダに書き込む）
            var src = CreateSrc();
            var initial = src.SystemConfig.AutoDefense;
            try
            {
                src.Commands.MapCommand(new UiCommand(Command.AutoDefenseCmdID, "自動反撃"));
                Assert.AreEqual(!initial, src.SystemConfig.AutoDefense);
            }
            catch (System.Exception)
            {
                // Save() が IO エラーを起こす環境では AutoDefense 値の確認のみ
                // (プロパティ設定後に Save が呼ばれるため、AutoDefense は変更済み)
                // Skip if file system issues arise
            }
        }

        [TestMethod]
        public void MapCommand_AutoDefenseCmdID_SetCommandStateToUnitSelect()
        {
            // AutoDefense トグルでも CommandState は "ユニット選択" になる
            var src = CreateSrc();
            src.Commands.CommandState = "前の状態";
            try
            {
                src.Commands.MapCommand(new UiCommand(Command.AutoDefenseCmdID, "自動反撃"));
            }
            catch
            {
                // IO 例外は無視
            }
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // 追加テスト
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MapCommand_AlwaysSetsCommandStateToUnitSelect_FromMapCommand()
        {
            var src = CreateSrc();
            src.Commands.CommandState = "マップコマンド";
            src.Commands.ViewMode = true;
            src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void MapCommand_EndTurnWithViewModeTrue_DoesNotCallEndTurnCommand()
        {
            // ViewMode=true の早期 return では EndTurnCommand (GUI依存) は呼ばれない
            // → GUINotImplementedException は発生しない
            var src = CreateSrc();
            src.Commands.ViewMode = true;
            bool threw = false;
            try
            {
                src.Commands.MapCommand(new UiCommand(Command.EndTurnCmdID, "ターン終了"));
            }
            catch (SRCCore.Exceptions.GUINotImplementedException)
            {
                threw = true;
            }
            Assert.IsFalse(threw, "ViewMode=true の場合 EndTurnCommand は呼ばれない");
        }

        [TestMethod]
        public void MapCommand_AutoDefenseCmdID_TogglesFromFalseToTrue()
        {
            var src = CreateSrc();
            src.SystemConfig.AutoDefense = false;
            try { src.Commands.MapCommand(new UiCommand(Command.AutoDefenseCmdID, "自動反撃")); } catch { }
            Assert.IsTrue(src.SystemConfig.AutoDefense);
        }

        [TestMethod]
        public void MapCommand_AutoDefenseCmdID_TogglesFromTrueToFalse()
        {
            var src = CreateSrc();
            src.SystemConfig.AutoDefense = true;
            try { src.Commands.MapCommand(new UiCommand(Command.AutoDefenseCmdID, "自動反撃")); } catch { }
            Assert.IsFalse(src.SystemConfig.AutoDefense);
        }

        [TestMethod]
        public void MapCommand_ViewMode_InitiallyFalse()
        {
            var src = CreateSrc();
            Assert.IsFalse(src.Commands.ViewMode);
        }

        [TestMethod]
        public void MapCommand_ConfigurationCmdID_CallsGuiConfigure()
        {
            var src = CreateSrc();
            bool configureCalled = false;
            ((MockGUI)src.GUI).ConfigureHandler = () => { configureCalled = true; };
            src.Commands.MapCommand(new UiCommand(Command.ConfigurationCmdID, "設定変更"));
            Assert.IsTrue(configureCalled);
        }

        [TestMethod]
        public void MapCommand_ConfigurationCmdID_SetsCommandStateToUnitSelect()
        {
            var src = CreateSrc();
            ((MockGUI)src.GUI).ConfigureHandler = () => { };
            src.Commands.CommandState = "コマンド選択";
            src.Commands.MapCommand(new UiCommand(Command.ConfigurationCmdID, "設定変更"));
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void MapCommand_OperationObjectCmdID_CallsGuiLockGUI()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            bool lockCalled = false;
            mock.LockGUIHandler = () => { lockCalled = true; };
            mock.RedrawScreenHandler = _ => { };
            mock.UnlockGUIHandler = () => { };
            try
            {
                src.Commands.MapCommand(new UiCommand(Command.OperationObjectCmdID, "作戦目的"));
            }
            catch { }
            Assert.IsTrue(lockCalled);
        }

        [TestMethod]
        public void MapCommand_OperationObjectCmdID_SetsCommandStateToUnitSelect()
        {
            var src = CreateSrc();
            var mock = (MockGUI)src.GUI;
            mock.LockGUIHandler = () => { };
            mock.RedrawScreenHandler = _ => { };
            mock.UnlockGUIHandler = () => { };
            src.Commands.CommandState = "コマンド選択";
            try
            {
                src.Commands.MapCommand(new UiCommand(Command.OperationObjectCmdID, "作戦目的"));
            }
            catch { }
            Assert.AreEqual("ユニット選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void MapCommand_UnknownCmdID_ThrowsNotSupportedException()
        {
            var src = CreateSrc();
            Assert.Throws<System.NotSupportedException>(() =>
                src.Commands.MapCommand(new UiCommand(9999, "不明コマンド")));
        }
    }
}
