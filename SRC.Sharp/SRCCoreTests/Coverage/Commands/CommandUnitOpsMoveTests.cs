using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Command.unitmove.cs（移動・テレポート・ジャンプコマンド）のテスト。
    /// VB 版 Command.bas の StartMoveCommand / FinishMoveCommand / StartTeleportCommand /
    /// FinishTeleportCommand / StartJumpCommand / FinishJumpCommand に対応。
    /// </summary>
    [TestClass]
    public class CommandUnitOpsMoveTests
    {
        private static CommandUnitOpsFixture CreateField(out Units.Unit mover, params string[] features)
        {
            var f = new CommandUnitOpsFixture();
            f.DefineUnit("移動機", speed: 3, features: features);
            mover = f.Place("移動機", "操縦者", "味方", 5, 5);
            f.Commands.SelectedUnit = mover;
            f.Commands.CommandState = "コマンド選択";
            return f;
        }

        // ターゲット選択状態でマス (x, y) をクリックしたものとして処理を進める
        private static void ClickTarget(CommandUnitOpsFixture f, int x, int y)
        {
            f.Commands.ProceedCommand(false, GuiButton.Left, f.Cell(x, y), f.Src.Map.MapDataForUnit[x, y]);
        }

        // ──────────────────────────────────────────────
        // 移動
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartMove_SetsMoveAreaAndTargetSelect()
        {
            // 移動力の範囲だけが選択可能になり、ユニットを中央に表示してターゲット選択に移る
            var f = CreateField(out var mover);

            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));

            Assert.AreEqual("移動", f.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.IsFalse(f.Src.Map.MaskData[5, 8], "移動力 3 で届くマス");
            Assert.IsTrue(f.Src.Map.MaskData[5, 9], "移動力 3 では届かないマス");
            var center = f.Calls("Center").Single();
            Assert.AreEqual(5, center.Args[0]);
            Assert.AreEqual(5, center.Args[1]);
            Assert.IsTrue(f.Called("MaskScreen"));
        }

        [TestMethod]
        public void FinishMove_ToEmptyCell_MovesUnitAndShowsAfterMoveMenu()
        {
            // 空きマスへの移動: 元の位置を記録してユニットを移し、移動後コマンド選択に移る
            var f = CreateField(out var mover);
            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            var cost = f.Src.Map.TotalMoveCost[5, 7];

            ClickTarget(f, 5, 7);

            Assert.AreEqual(5, mover.x);
            Assert.AreEqual(7, mover.y);
            Assert.AreSame(mover, f.Src.Map.MapDataForUnit[5, 7]);
            Assert.IsNull(f.Src.Map.MapDataForUnit[5, 5]);
            Assert.IsTrue(cost > 0);
            Assert.AreEqual(cost, f.Commands.SelectedUnitMoveCost, "移動に要したコストを記録する");
            Assert.AreEqual("移動後コマンド選択", f.Commands.CommandState);
            Assert.AreEqual(0, mover.UsedAction, "移動しただけでは行動終了しない");
            var menu = (System.Collections.Generic.IList<UiCommand>)f.Calls("ShowUnitCommandMenu").Last().Args[0];
            Assert.IsTrue(menu.Any(x => x.Id == Command.WaitCmdID));
        }

        [TestMethod]
        public void FinishMove_Remove_WithMoveCostAlreadySpent_EndsAction()
        {
            // 再移動（既に移動コストを使っている）の場合は移動後に行動終了する（VB: SelectedUnitMoveCost > 0 → WaitCommand）
            var f = CreateField(out var mover);
            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            f.Commands.SelectedCommand = "再移動";
            f.Commands.SelectedUnitMoveCost = 2;

            ClickTarget(f, 6, 5);

            Assert.AreEqual(6, mover.x);
            Assert.AreEqual(1, mover.UsedAction);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void FinishMove_ClickOwnCell_IsIgnored()
        {
            // 自分自身のマスは移動先に選べない
            var f = CreateField(out var mover);
            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));

            ClickTarget(f, 5, 5);

            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.IsFalse(f.Called("LockGUI"));
        }

        [TestMethod]
        public void FinishMove_OntoCarrier_CancelConfirm_KeepsUnitInPlace()
        {
            // 母艦のマスへ移動する場合は着艦の確認を取り、キャンセルなら移動しない
            var f = CreateField(out var mover);
            f.DefineUnit("母艦機", features: "母艦");
            var carrier = f.Place("母艦機", "艦長", "味方", 5, 7);
            string confirmMessage = null;
            f.Gui.Handle("Confirm", args => { confirmMessage = (string)args[0]; return GuiDialogResult.Cancel; });
            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            f.Src.Map.MaskData[5, 7] = false;

            ClickTarget(f, 5, 7);

            Assert.AreEqual("着艦しますか？", confirmMessage);
            Assert.AreEqual(5, mover.y);
            Assert.AreSame(mover, f.Src.Map.MapDataForUnit[5, 5]);
            Assert.AreSame(carrier, f.Src.Map.MapDataForUnit[5, 7]);
            Assert.AreEqual(0, carrier.CountUnitOnBoard());
        }

        [TestMethod]
        public void FinishMove_OntoCarrier_Ok_LandsAndEndsAction()
        {
            // 着艦すると母艦に格納され、行動数を消費してユニット選択に戻る
            var f = CreateField(out var mover);
            f.DefineUnit("母艦機", features: "母艦");
            var carrier = f.Place("母艦機", "艦長", "味方", 5, 7);
            f.Gui.Handle("Confirm", args => GuiDialogResult.Ok);
            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            f.Src.Map.MaskData[5, 7] = false;

            ClickTarget(f, 5, 7);

            Assert.AreEqual("格納", mover.Status);
            Assert.AreEqual(1, carrier.CountUnitOnBoard());
            Assert.AreSame(carrier, f.Src.Map.MapDataForUnit[5, 7]);
            Assert.IsNull(f.Src.Map.MapDataForUnit[5, 5]);
            Assert.AreEqual(1, mover.UsedAction);
            Assert.AreSame(carrier, f.Commands.SelectedTarget);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void FinishMove_OntoNonCarrierUnit_AsksCombine()
        {
            // 母艦以外のユニットがいるマスへの移動は合体の確認になる
            var f = CreateField(out var mover);
            f.DefineUnit("僚機");
            f.Place("僚機", "僚機乗り", "味方", 5, 7);
            string confirmTitle = null;
            f.Gui.Handle("Confirm", args => { confirmTitle = (string)args[1]; return GuiDialogResult.Cancel; });
            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            f.Src.Map.MaskData[5, 7] = false;

            ClickTarget(f, 5, 7);

            Assert.AreEqual("合体", confirmTitle);
            Assert.AreEqual(5, mover.y);
        }

        [TestMethod]
        public void CancelAfterMove_ReturnsUnitToPreviousPosition()
        {
            // 移動後コマンド選択でキャンセルすると元の位置・EN に戻り、再び移動先の選択になる
            var f = CreateField(out var mover);
            f.Commands.UnitCommand(new UiCommand(Command.MoveCmdID, "移動"));
            ClickTarget(f, 5, 7);
            mover.EN = 50;

            f.Commands.CancelCommand();

            Assert.AreEqual(5, mover.y);
            Assert.AreEqual(100, mover.EN);
            Assert.AreSame(mover, f.Src.Map.MapDataForUnit[5, 5]);
            Assert.AreEqual(0, f.Commands.SelectedUnitMoveCost);
            Assert.AreEqual("移動", f.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
        }

        // ──────────────────────────────────────────────
        // テレポート
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartTeleport_LargeMapOption_DoesNotCenter()
        {
            // 「大型マップ」オプション時は画面の中央寄せを行わない
            var f = CreateField(out _, "テレポート=テレポート 30");
            f.Src.Expression.DefineGlobalVariable("Option(大型マップ)");

            f.Commands.UnitCommand(new UiCommand(Command.TeleportCmdID, "テレポート"));

            Assert.AreEqual("テレポート", f.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.IsFalse(f.Called("Center"));
            Assert.IsTrue(f.Called("MaskScreen"));
        }

        [TestMethod]
        public void FinishTeleport_ConsumesSpecifiedEN()
        {
            // テレポート=名称 消費EN の書式なら指定量の EN を消費し、移動後コマンド選択になる
            var f = CreateField(out var mover, "テレポート=テレポート 30");
            f.Commands.UnitCommand(new UiCommand(Command.TeleportCmdID, "テレポート"));
            f.ClearMask();

            ClickTarget(f, 9, 9);

            Assert.AreEqual(9, mover.x);
            Assert.AreEqual(9, mover.y);
            Assert.AreEqual(70, mover.EN);
            Assert.AreEqual(100, f.Commands.SelectedUnitMoveCost, "テレポート後は再移動不可として移動コスト 100 を記録");
            Assert.AreEqual("移動後コマンド選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void FinishTeleport_WithoutENSpec_Consumes40EN()
        {
            // 消費 EN の指定がなければ 40 消費
            var f = CreateField(out var mover, "テレポート");
            f.Commands.UnitCommand(new UiCommand(Command.TeleportCmdID, "テレポート"));
            f.ClearMask();

            ClickTarget(f, 9, 9);

            Assert.AreEqual(60, mover.EN);
        }

        // ──────────────────────────────────────────────
        // ジャンプ
        // ──────────────────────────────────────────────

        [TestMethod]
        public void FinishJump_ConsumesSpecifiedEN()
        {
            // ジャンプ=名称 消費EN の書式なら指定量の EN を消費する
            var f = CreateField(out var mover, "ジャンプ=ジャンプ 20");
            f.Commands.UnitCommand(new UiCommand(Command.JumpCmdID, "ジャンプ"));
            Assert.AreEqual("ジャンプ", f.Commands.SelectedCommand);
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);

            ClickTarget(f, 5, 7);

            Assert.AreEqual(7, mover.y);
            Assert.AreEqual(80, mover.EN);
            Assert.AreEqual(100, f.Commands.SelectedUnitMoveCost);
            Assert.AreEqual("移動後コマンド選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void FinishJump_WithoutENSpec_DoesNotConsumeEN()
        {
            // テレポートと違い、消費 EN の指定がないジャンプは EN を消費しない
            var f = CreateField(out var mover, "ジャンプ");
            f.Commands.UnitCommand(new UiCommand(Command.JumpCmdID, "ジャンプ"));

            ClickTarget(f, 5, 7);

            Assert.AreEqual(7, mover.y);
            Assert.AreEqual(100, mover.EN);
        }

        [TestMethod]
        public void FinishJump_OntoCarrier_LandsAndEndsAction()
        {
            // ジャンプ先が母艦なら着艦し、行動終了してユニット選択に戻る
            var f = CreateField(out var mover, "ジャンプ");
            f.DefineUnit("母艦機", features: "母艦");
            var carrier = f.Place("母艦機", "艦長", "味方", 5, 7);
            f.Gui.Handle("Confirm", args => GuiDialogResult.Ok);
            f.Commands.UnitCommand(new UiCommand(Command.JumpCmdID, "ジャンプ"));
            f.Src.Map.MaskData[5, 7] = false;

            ClickTarget(f, 5, 7);

            Assert.AreEqual("格納", mover.Status);
            Assert.AreEqual(1, carrier.CountUnitOnBoard());
            Assert.AreEqual(1, mover.UsedAction);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void FinishTeleport_OntoCarrier_LandsAndEndsAction()
        {
            // テレポート先が母艦なら着艦の確認後に格納され、行動終了する
            var f = CreateField(out var mover, "テレポート=テレポート 30");
            f.DefineUnit("母艦機", features: "母艦");
            var carrier = f.Place("母艦機", "艦長", "味方", 9, 9);
            string confirmMessage = null;
            f.Gui.Handle("Confirm", args => { confirmMessage = (string)args[0]; return GuiDialogResult.Ok; });
            f.Commands.UnitCommand(new UiCommand(Command.TeleportCmdID, "テレポート"));
            f.ClearMask();

            ClickTarget(f, 9, 9);

            Assert.AreEqual("着艦しますか？", confirmMessage);
            Assert.AreEqual("格納", mover.Status);
            Assert.AreEqual(1, carrier.CountUnitOnBoard());
            Assert.AreEqual(70, mover.EN, "テレポートの EN は消費される");
            Assert.AreEqual(1, mover.UsedAction);
            Assert.AreSame(carrier, f.Calls("DisplayUnitStatus").Last().Args[0]);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void FinishTeleport_CancelConfirm_KeepsEN()
        {
            // 確認でキャンセルした場合は EN を消費せず移動もしない
            var f = CreateField(out var mover, "テレポート=テレポート 30");
            f.DefineUnit("母艦機", features: "母艦");
            f.Place("母艦機", "艦長", "味方", 9, 9);
            f.Gui.Handle("Confirm", args => GuiDialogResult.Cancel);
            f.Commands.UnitCommand(new UiCommand(Command.TeleportCmdID, "テレポート"));
            f.ClearMask();

            ClickTarget(f, 9, 9);

            Assert.AreEqual(100, mover.EN);
            Assert.AreEqual(5, mover.x);
            Assert.AreEqual("出撃", mover.Status);
        }
    }
}
