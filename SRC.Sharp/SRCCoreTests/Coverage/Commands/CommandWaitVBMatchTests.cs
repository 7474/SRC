using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Commands;
using System.Linq;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// Command.bas の WaitCommand を元の SRC と突き合わせたテスト（#1498）。
    /// </summary>
    [TestClass]
    public class CommandWaitVBMatchTests
    {
        [TestMethod]
        public void Wait_UpdatesUnitStatusWindow()
        {
            // 元の SRC は待機の最後に、出撃中でメインの画面の幅が 15 ならステータスを表示し直す
            var f = new CommandUnitOpsFixture();
            f.Src.GUI.MainWidth = 15;
            f.DefineUnit("機体", speed: 3);
            var u = f.Place("機体", "パイロット", "味方", 5, 5);
            f.Commands.SelectedUnit = u;
            f.Commands.CommandState = "コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.WaitCmdID, "待機"));

            var last = f.Gui.Calls.Last(c => c.Member == "DisplayUnitStatus" || c.Member == "ClearUnitStatus");
            Assert.AreEqual("DisplayUnitStatus", last.Member);
            Assert.AreSame(u, last.Args[0]);
            Assert.AreEqual("ユニット選択", f.Commands.CommandState);
        }

        [TestMethod]
        public void Wait_NotStandardWidth_ClearsUnitStatusWindow()
        {
            // メインの画面の幅が 15 でなければ（新 GUI）ステータスを消す
            var f = new CommandUnitOpsFixture();
            f.Src.GUI.MainWidth = 20;
            f.DefineUnit("機体", speed: 3);
            var u = f.Place("機体", "パイロット", "味方", 5, 5);
            f.Commands.SelectedUnit = u;
            f.Commands.CommandState = "コマンド選択";

            f.Commands.UnitCommand(new UiCommand(Command.WaitCmdID, "待機"));

            var last = f.Gui.Calls.Last(c => c.Member == "DisplayUnitStatus" || c.Member == "ClearUnitStatus");
            Assert.AreEqual("ClearUnitStatus", last.Member);
        }
    }
}
