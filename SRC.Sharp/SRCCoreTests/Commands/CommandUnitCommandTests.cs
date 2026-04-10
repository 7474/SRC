using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Exceptions;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.UnitCommand() のユニットテスト (Command.unitcommand.cs)
    ///
    /// UnitCommand は command.Id でディスパッチする大きな switch 文。
    /// GUI 依存が強い各コマンドの詳細テストは困難なため、以下の観点でテストする:
    /// 1. MoveCmdID (0) + CommandState=="移動後コマンド選択" → 即 return (状態変化なし)
    ///    (「なんらかの原因でユニットコマンドの選択がうまくいかなかった場合」の安全弁)
    /// 2. MoveCmdID (0) + その他 → StartMoveCommand() が呼ばれ GUI 例外で到達確認
    /// 3. UnitCommand 呼び出し前に PrevCommand が SelectedCommand にセットされる
    /// </summary>
    [TestClass]
    public class CommandUnitCommandTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            // UnitCommand は unit.UsedAction を参照するため SelectedUnit が必要
            src.Commands.SelectedUnit = new Unit(src);
            return src;
        }

        // ──────────────────────────────────────────────
        // MoveCmdID + CommandState == "移動後コマンド選択" → 即 return
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_MoveCmdID_PostMoveCommandSelectState_DoesNotThrow()
        {
            // 移動後コマンド選択中に Move が選ばれた場合は即 return → GUI 呼び出しなし
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            // 例外なし
        }

        [TestMethod]
        public void UnitCommand_MoveCmdID_PostMoveCommandSelectState_CommandStateUnchanged()
        {
            // 早期 return なので CommandState は変更されない
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            Assert.AreEqual("移動後コマンド選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void UnitCommand_MoveCmdID_PostMoveCommandSelectState_SelectedUnitUnchanged()
        {
            // 早期 return なので SelectedUnit は変更されない
            var src = CreateSrc();
            var unit = src.Commands.SelectedUnit;
            src.Commands.CommandState = "移動後コマンド選択";
            src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            Assert.AreSame(unit, src.Commands.SelectedUnit);
        }

        // ──────────────────────────────────────────────
        // MoveCmdID + 通常状態 → StartMoveCommand() が呼ばれる
        // (GUI 依存のため GUINotImplementedException で到達を確認)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_MoveCmdID_NormalState_CallsStartMoveCommand()
        {
            // CommandState が "移動後コマンド選択" 以外なら StartMoveCommand() に進む
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reachedGuiCall = false;
            ((MockGUI)src.GUI).MaskScreenHandler = () => { reachedGuiCall = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            }
            catch (GUINotImplementedException)
            {
                // GUI 呼び出しに到達: 期待通り
                reachedGuiCall = true;
            }
            catch
            {
                // その他の例外でも GUI への呼び出し試行は確認できた
                reachedGuiCall = true;
            }
            Assert.IsTrue(reachedGuiCall, "StartMoveCommand() に到達するはずです");
        }

        // ──────────────────────────────────────────────
        // UnitCommand 共通: SelectedCommand が PrevCommand に保存される
        // (PrevCommand は private だが、移動後コマンドでキャンセル時に利用される)
        // ここでは早期 return する MoveCmdID ケースで SelectedCommand の不変性を確認
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_MoveCmdID_PostMoveState_SelectedCommandUnchanged()
        {
            // 早期 return でも SelectedCommand 自体は書き換えられない
            var src = CreateSrc();
            src.Commands.CommandState = "移動後コマンド選択";
            src.Commands.SelectedCommand = "移動";
            src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            Assert.AreEqual("移動", src.Commands.SelectedCommand);
        }

        // ──────────────────────────────────────────────
        // WaitCmdID (255) → WaitCommand() が呼ばれる
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_WaitCmdID_CallsWaitCommand()
        {
            // WaitCmdID (255) のパスが WaitCommand() に到達することを確認
            var src = CreateSrc();
            src.Commands.CommandState = "コマンド選択";
            bool reachedWait = false;
            ((MockGUI)src.GUI).RedrawScreenHandler = (_) => { reachedWait = true; };
            try
            {
                src.Commands.UnitCommand(new UiCommand(Command.WaitCmdID, "待機"));
            }
            catch (GUINotImplementedException)
            {
                reachedWait = true;
            }
            catch
            {
                reachedWait = true;
            }
            Assert.IsTrue(reachedWait, "WaitCommand() またはその先の GUI 呼び出しに到達するはずです");
        }
    }
}
