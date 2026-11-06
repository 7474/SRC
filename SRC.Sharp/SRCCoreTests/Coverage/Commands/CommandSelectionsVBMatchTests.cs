using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;

namespace SRCCore.Coverage.Tests.CommandsCoverage
{
    /// <summary>
    /// Command.bas の RestoreSelections を元の SRC と突き合わせたテスト（#1498・#1521）。
    /// 元の SRC は退避したユニットの CurrentForm を選び直す。
    /// Split・Transform のイベントコマンドは今の選択だけを書き換え、退避した選択は書き換えない。
    /// </summary>
    [TestClass]
    public class CommandSelectionsVBMatchTests
    {
        private static int ExecEvent(CommandUnitFormFixture f, string text)
        {
            var id = f.Src.Event.EventData.Count;
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, text);
            f.Src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(f.Src, line);
            f.Src.Event.EventCmd.Add(cmd);
            return cmd.Exec();
        }

        [TestMethod]
        public void RestoreSelections_UnitTransformedWithoutCommand_SelectsCurrentForm()
        {
            // 退避した後にコマンドを通らずに変形したとき（戦闘中の変形など）、戻すと変形後の形態を選ぶ
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("形態A", "変形=変形 形態B");
            f.AddUnitData("形態B", "変形=変形 形態A");
            var ua = f.CreateUnit("形態A", "パイロット", x: 3, y: 3);
            var ut = f.CreateUnit("形態A", "相手", party: "敵", x: 5, y: 5);
            f.Src.Commands.SelectedUnit = ua;
            f.Src.Commands.SelectedTarget = ut;
            f.Src.Event.SelectedUnitForEvent = ua;
            f.Src.Event.SelectedTargetForEvent = ut;
            f.Src.Commands.SaveSelections();

            ua.Transform("形態B");
            ut.Transform("形態B");
            var ub = ua.CurrentForm();
            var utb = ut.CurrentForm();
            Assert.AreEqual("形態B", ub.Name);
            Assert.AreNotSame(ua, ub);

            f.Src.Commands.RestoreSelections();

            Assert.AreSame(ub, f.Src.Commands.SelectedUnit);
            Assert.AreSame(utb, f.Src.Commands.SelectedTarget);
            Assert.AreSame(ub, f.Src.Event.SelectedUnitForEvent);
            Assert.AreSame(utb, f.Src.Event.SelectedTargetForEvent);
        }

        [TestMethod]
        public void RestoreSelections_NullUnits_StayNull()
        {
            var f = new CommandUnitFormFixture(onMap: true);
            f.Src.Commands.SelectedUnit = null;
            f.Src.Commands.SelectedTarget = null;
            f.Src.Commands.SaveSelections();
            f.AddUnitData("形態A");
            var u = f.CreateUnit("形態A", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Commands.SelectedTarget = u;

            f.Src.Commands.RestoreSelections();

            Assert.IsNull(f.Src.Commands.SelectedUnit);
            Assert.IsNull(f.Src.Commands.SelectedTarget);
        }

        [TestMethod]
        public void TransformCmd_DoesNotRewriteSavedSelections_RestoreSelectsCurrentForm()
        {
            // Transform は今の選択だけを変形後にする。戻すと、退避した変形前のユニットの CurrentForm（変形後）を選ぶ
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("形態A", "変形=変形 形態B");
            f.AddUnitData("形態B", "変形=変形 形態A");
            var ua = f.CreateUnit("形態A", "パイロット", x: 3, y: 3);
            f.Src.Commands.SelectedUnit = ua;
            f.Src.Event.SelectedUnitForEvent = ua;
            f.Src.Commands.SaveSelections();

            ExecEvent(f, "Transform 形態B");

            var ub = ua.CurrentForm();
            Assert.AreEqual("形態B", ub.Name);
            Assert.AreSame(ub, f.Src.Commands.SelectedUnit);
            Assert.AreSame(ub, f.Src.Event.SelectedUnitForEvent);
            foreach (var state in f.Src.Commands.SavedStates)
            {
                Assert.AreSame(ua, state.SelectedUnit);
            }

            f.Src.Commands.RestoreSelections();

            Assert.AreSame(ub, f.Src.Commands.SelectedUnit);
            Assert.AreSame(ub, f.Src.Event.SelectedUnitForEvent);
        }

        [TestMethod]
        public void SplitCmd_DoesNotRewriteSavedSelections_RestoreSelectsOriginalUnit()
        {
            // Split は今の選択だけを分離後の１番目のユニットにする。
            // 退避した選択は元のユニットのままで、戻すと元のユニット（他形態が無いので CurrentForm は自身）を選ぶ
            var f = new CommandUnitFormFixture(onMap: true);
            f.AddUnitData("合体形態", "分離=分離 子1 子2").PilotNum = 2;
            f.AddUnitData("子1", "合体=合体 合体形態 子2").PilotNum = 1;
            f.AddUnitData("子2", "合体=合体 合体形態 子1").PilotNum = 1;
            var c1 = f.Src.UList.Add("子1", 0, "味方");
            var c2 = f.Src.UList.Add("子2", 0, "味方");
            c1.Status = "旧主形態";
            c2.Status = "旧形態";
            var u = f.CreateUnit("合体形態", "パイロット1", x: 4, y: 4);
            f.AddPilotData("パイロット2");
            f.Src.PList.Add("パイロット2", 1, "味方").Ride(u);
            f.Src.Commands.SelectedUnit = u;
            f.Src.Event.SelectedUnitForEvent = u;
            f.Src.Commands.SaveSelections();

            ExecEvent(f, "Split");

            Assert.AreSame(c1, f.Src.Commands.SelectedUnit);
            Assert.AreSame(c1, f.Src.Event.SelectedUnitForEvent);

            f.Src.Commands.RestoreSelections();

            Assert.AreSame(u, f.Src.Commands.SelectedUnit);
            Assert.AreSame(u, f.Src.Event.SelectedUnitForEvent);
        }
    }
}
