using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests.Commands
{
    /// <summary>
    /// CmdData.cls の ExecMonotoneCmd・ExecNightCmd・ExecNoonCmd を、元の SRC の動作を期待値にして確かめる（7474/SRC#1504）。
    /// </summary>
    [TestClass]
    public class CmdDataMonotoneNightNoonVBMatchTests
    {
        private static CmdData CreateCmd(SRC src, string cmdText)
        {
            var id = src.Event.EventData.Count;
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        private static (MapCoverageFixture f, List<string> log) CreateFixture()
        {
            var f = new MapCoverageFixture();
            f.Src.Event.EventData = new List<EventDataLine>();
            f.Src.Event.EventCmd = new List<CmdData>();
            f.Src.Event.EventQue = new List<string>();
            f.GUI.ChangeStatusHandler = _ => { };
            f.LoadGrid(
                ".....",
                ".....",
                ".....");
            f.GUI.MapX = 4;
            f.GUI.MapY = 2;
            var log = new List<string>();
            f.GUI.SetupBackgroundHandler = (mode, opt, color, trans) =>
            {
                log.Add("SetupBackground " + mode);
                // SetupBackground はマップの表示位置を中央にする
                f.GUI.MapX = f.GUI.MainWidth / 2 + 1;
                f.GUI.MapY = f.GUI.MainHeight / 2 + 1;
            };
            f.GUI.CenterHandler = (x, y) =>
            {
                log.Add("Center " + x + "," + y);
                f.GUI.MapX = x;
                f.GUI.MapY = y;
            };
            f.GUI.RedrawScreenHandler = late => log.Add("RedrawScreen " + late);
            return (f, log);
        }

        [TestMethod]
        [DataRow("Monotone", "白黒")]
        [DataRow("Night", "夜")]
        [DataRow("Noon", "")]
        public void RestoresMapPositionAfterSetupBackground(string cmdName, string mode)
        {
            // 元の SRC: prev_x = MapX: prev_y = MapY → SetupBackground → Center prev_x, prev_y → RedrawScreen
            var (f, log) = CreateFixture();

            var ret = CreateCmd(f.Src, cmdName).Exec();

            Assert.AreEqual(1, ret);
            CollectionAssert.AreEqual(new[] { "SetupBackground " + mode, "Center 4,2", "RedrawScreen False" }, log);
            Assert.AreEqual(4, f.GUI.MapX);
            Assert.AreEqual(2, f.GUI.MapY);
        }

        [TestMethod]
        public void Noon_AsyncOption_RedrawsLate()
        {
            var (f, log) = CreateFixture();

            var ret = CreateCmd(f.Src, "Noon 非同期").Exec();

            Assert.AreEqual(1, ret);
            CollectionAssert.AreEqual(new[] { "SetupBackground ", "Center 4,2", "RedrawScreen True" }, log);
        }

        [TestMethod]
        [DataRow("Noon 同期")]
        [DataRow("Noon 非同期 非同期")]
        public void Noon_InvalidArguments_AreErrors(string cmdText)
        {
            // 元の SRC: 引数は「非同期」1 つだけ。ほかの文字列は「オプションが不正」、2 つ以上は「引数の数が違います」のエラー
            var (f, log) = CreateFixture();

            var ret = CreateCmd(f.Src, cmdText).Exec();

            Assert.AreEqual(-1, ret);
            Assert.AreEqual(0, log.Count);
        }
    }
}
