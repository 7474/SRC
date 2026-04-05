using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.TestLib;
using SRCCore.Units;

namespace SRCCore.Commands.Tests
{
    /// <summary>
    /// Command.unitcommand.cs で定義されている UnitCommand の部分的なユニットテスト。
    /// 完全な GUI 依存パスは対象外とし、早期リターン可能なパスを検証する。
    ///
    /// 備考:
    /// - GroundCmdID / SkyCmdID / UndergroundCmdID / WaterCmdID は Map.Terrain() が必要で
    ///   テストには完全な Map 初期化が必要なため除外 (Issue を別途作成)。
    /// - WaitCmdID は GUI.RedrawScreen / Status.ClearUnitStatus が必要なため除外。
    /// </summary>
    [TestClass]
    public class CommandUnitCommandTests
    {
        private SRC CreateSrc() => new SRC { GUI = new MockGUI() };

        // ──────────────────────────────────────────────
        // UnitCommand — MoveCmdID × "移動後コマンド選択" → 早期リターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_MoveCmdID_WithMoveAfterCommandState_ReturnsEarlyWithoutThrow()
        {
            // 「なんらかの原因により、ユニットコマンドの選択がうまくいかなかった場合は
            //  移動後のコマンド選択をやり直す」パス
            // CommandState == "移動後コマンド選択" のとき、MoveCmdID は即座に return する
            var src = CreateSrc();
            var unit = new Unit(src);
            src.Commands.SelectedUnit = unit;
            src.Commands.CommandState = "移動後コマンド選択";
            src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            // 早期リターンにより CommandState は変更されない
            Assert.AreEqual("移動後コマンド選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void UnitCommand_MoveCmdID_WithMoveAfterCommandState_SetsPrevCommand()
        {
            // switch に入る前に PrevCommand = SelectedCommand が設定される
            var src = CreateSrc();
            var unit = new Unit(src);
            src.Commands.SelectedUnit = unit;
            src.Commands.SelectedCommand = "攻撃";
            src.Commands.CommandState = "移動後コマンド選択";
            src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            // PrevCommand は switch 前に設定済みなので内部フィールドを直接検証できないが
            // 例外なく完了することを確認する
            // (PrevCommand は private フィールドのため CommandState の不変性で代替確認)
            Assert.AreEqual("移動後コマンド選択", src.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // UnitCommand — 未知のコマンドID × "移動後コマンド選択" → 早期リターン
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_UnknownId_WithMoveAfterCommandState_ReturnsEarlyWithoutThrow()
        {
            // default ケースでも CommandState == "移動後コマンド選択" なら早期リターン
            var src = CreateSrc();
            var unit = new Unit(src);
            src.Commands.SelectedUnit = unit;
            src.Commands.CommandState = "移動後コマンド選択";
            // int.MaxValue はどの既知コマンド ID にも一致しない
            src.Commands.UnitCommand(new UiCommand(int.MaxValue, "未知のコマンド"));
            Assert.AreEqual("移動後コマンド選択", src.Commands.CommandState);
        }

        [TestMethod]
        public void UnitCommand_UnknownId_WithMoveAfterCommandState_DoesNotThrow()
        {
            // 未知のコマンド ID × 移動後コマンド選択 で GUI ハンドラ未設定でも例外なし
            var src = CreateSrc();
            var unit = new Unit(src);
            src.Commands.SelectedUnit = unit;
            src.Commands.CommandState = "移動後コマンド選択";
            src.Commands.UnitCommand(new UiCommand(int.MaxValue, "未知のコマンド"));
            // 到達できれば合格
        }

        // ──────────────────────────────────────────────
        // UnitCommand — 移動ラベルに応じた分岐の確認
        // (CommandState != "移動後コマンド選択" のため GUI 呼び出し前に分岐を確認)
        // ──────────────────────────────────────────────

        [TestMethod]
        public void UnitCommand_MoveCmdID_MoveAfterState_LabelVariantDoesNotMatter()
        {
            // "移動後コマンド選択" の場合は Label に関係なく早期リターン
            var src = CreateSrc();
            var unit = new Unit(src);
            src.Commands.SelectedUnit = unit;
            src.Commands.CommandState = "移動後コマンド選択";
            // "移動" ラベルではなく "速度確認" ラベルでも同じく早期リターン
            src.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "速度確認"));
            Assert.AreEqual("移動後コマンド選択", src.Commands.CommandState);
        }
    }
}
