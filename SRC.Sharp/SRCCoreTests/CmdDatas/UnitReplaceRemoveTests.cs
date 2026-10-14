using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// RemoveUnit（ユニット名の指定）・ReplacePilot の動作のテスト
    /// </summary>
    [TestClass]
    public class UnitReplaceRemoveTests
    {
        // ユニットの退避（Escape）でステータス表示を参照するため
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
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        private static Units.Unit CreateUnit(SRC src, string name)
        {
            if (!src.UDList.IsDefined(name))
            {
                var ud = src.UDList.Add(name);
                ud.HP = 1000;
                ud.EN = 100;
                ud.PilotNum = 1;
            }
            var u = src.UList.Add(name, 0, "味方");
            u.Update();
            return u;
        }

        private static Pilots.Pilot CreatePilot(SRC src, string name)
        {
            if (!src.PDList.IsDefined(name))
            {
                var pd = src.PDList.Add(name);
                pd.SP = 100;
            }
            return src.PList.Add(name, 1, "味方");
        }

        [TestMethod]
        public void RemoveUnit_ByName_RemovesUnitWithoutPilotFirst()
        {
            var src = CreateSrc();
            var piloted = CreateUnit(src, "ランタン号");
            CreatePilot(src, "マルタ").Ride(piloted);
            var empty = CreateUnit(src, "ランタン号");

            var result = CreateCmd(src, "RemoveUnit ランタン号").Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual("破棄", empty.Status);
            Assert.AreNotEqual("破棄", piloted.Status);
        }

        [TestMethod]
        public void RemoveUnit_ByName_RemovesPilotedUnitWhenNoEmptyOne()
        {
            var src = CreateSrc();
            var piloted = CreateUnit(src, "ランタン号");
            var p = CreatePilot(src, "マルタ");
            p.Ride(piloted);

            CreateCmd(src, "RemoveUnit ランタン号").Exec();

            Assert.AreEqual("破棄", piloted.Status);
            Assert.IsNull(p.Unit);
        }

        [TestMethod]
        public void RemoveUnit_ByName_DoesNotTouchOtherUnits()
        {
            var src = CreateSrc();
            var other = CreateUnit(src, "別の船");

            CreateCmd(src, "RemoveUnit ランタン号").Exec();

            Assert.AreNotEqual("破棄", other.Status);
        }

        [TestMethod]
        public void ReplacePilot_NewPilotBoardsTheUnit()
        {
            var src = CreateSrc();
            var u = CreateUnit(src, "ランタン号");
            var p1 = CreatePilot(src, "マルタ");
            p1.Ride(u);
            src.PDList.Add("ソラ").SP = 100;

            var result = CreateCmd(src, "ReplacePilot マルタ ソラ").Exec();

            Assert.AreEqual(1, result);
            Assert.AreEqual("ソラ", u.MainPilot().Name);
            var p2 = src.PList.Items.First(x => x.Name == "ソラ");
            Assert.AreSame(u, p2.Unit);
            Assert.IsFalse(p1.Alive);
            Assert.IsNull(p1.Unit);
        }
    }
}
