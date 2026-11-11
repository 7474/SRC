using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using SRCCore.Models;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Command.bas の StartLaunchCommand・FinishLaunchCommand を元の SRC と突き合わせたテスト（#1498）。
    /// </summary>
    [TestClass]
    public class CommandLaunchVBMatchTests
    {
        private static CommandUnitOpsFixture CreateField(out Units.Unit carrier, out Units.Unit fighter, bool fighterIsCarrier = false)
        {
            var f = new CommandUnitOpsFixture();
            f.DefineUnit("母艦", speed: 3, features: "母艦");
            if (fighterIsCarrier)
            {
                f.DefineUnit("艦載機", speed: 3, features: "母艦");
            }
            else
            {
                f.DefineUnit("艦載機", speed: 3);
            }
            carrier = f.Place("母艦", "艦長", "味方", 5, 5);
            fighter = f.Place("艦載機", "パイロット", "味方", 4, 5);
            fighter.Land(carrier);
            Assert.IsTrue(carrier.UnitOnBoards.Contains(fighter));
            f.Gui.Handle("ListBox", args => 1);
            f.Commands.SelectedUnit = carrier;
            f.Commands.CommandState = "コマンド選択";
            return f;
        }

        private static void ClickTarget(CommandUnitOpsFixture f, int x, int y)
        {
            f.Commands.ProceedCommand(false, GuiButton.Left, f.Cell(x, y), f.Src.Map.MapDataForUnit[x, y]);
        }

        [TestMethod]
        public void CancelAfterLaunch_ReturnsUnitToCarrier()
        {
            // 元の SRC の FinishLaunchCommand は母艦の位置を PrevUnitX・PrevUnitY に記録する。
            // 発進した後のコマンド選択でキャンセルすると、発進したユニットは母艦に戻り、母艦を選ぶ
            var f = CreateField(out var carrier, out var fighter);
            f.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));
            Assert.AreEqual("ターゲット選択", f.Commands.CommandState);
            Assert.AreSame(fighter, f.Commands.SelectedTarget);

            ClickTarget(f, 5, 7);
            Assert.AreSame(fighter, f.Src.Map.MapDataForUnit[5, 7]);
            Assert.AreSame(carrier, f.Src.Map.MapDataForUnit[5, 5]);
            Assert.AreEqual("移動後コマンド選択", f.Commands.CommandState);
            // 母艦を描き直す
            Assert.IsTrue(f.Calls("PaintUnitBitmap").Any(c => ReferenceEquals(c.Args[0], carrier)));

            f.Commands.CancelCommand();

            Assert.IsTrue(carrier.UnitOnBoards.Contains(fighter));
            Assert.IsNull(f.Src.Map.MapDataForUnit[5, 7]);
            Assert.AreSame(carrier, f.Src.Map.MapDataForUnit[5, 5]);
            Assert.AreSame(carrier, f.Commands.SelectedUnit);
        }

        [TestMethod]
        public void FinishLaunch_OntoCarrier_AsksLanding_EvenIfLaunchingUnitIsCarrier()
        {
            // 元の SRC は目的地のユニットが母艦なら、発進するユニットが母艦でも「着艦しますか？」と聞く
            var f = CreateField(out var carrier, out var fighter, fighterIsCarrier: true);
            f.DefineUnit("別の母艦", speed: 3, features: "母艦");
            f.Place("別の母艦", "別の艦長", "味方", 5, 7);
            string confirmTitle = null;
            f.Gui.Handle("Confirm", args => { confirmTitle = (string)args[1]; return GuiDialogResult.Cancel; });
            f.Commands.UnitCommand(new UiCommand(Command.LaunchCmdID, "発進"));
            f.Src.Map.MaskData[5, 7] = false;

            ClickTarget(f, 5, 7);

            Assert.AreEqual("着艦", confirmTitle);
        }
    }
}
