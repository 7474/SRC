using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Coverage.Tests.UnitsCoverage;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;
using System.Linq;

namespace SRCCore.Coverage.Tests.Commands
{
    /// <summary>
    /// CmdData.cls の ExecSunsetCmd・ExecWaterCmd・ExecUpgradeCmd・ExecWaitCmd・ExecWhiteInCmd を、
    /// 元の SRC の動作を期待値にして確かめる（7474/SRC#1505）。
    /// </summary>
    [TestClass]
    public class CmdDataSunsetToWhiteInVBMatchTests
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

        private static SRC CreateSrc(MockGUI gui)
        {
            var src = new SRC { GUI = gui };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventQue = new List<string>();
            return src;
        }

        [TestMethod]
        [DataRow("Sunset", "夕焼け")]
        [DataRow("Water", "水中")]
        public void RestoresMapPositionAfterSetupBackground(string cmdName, string mode)
        {
            // 元の SRC: prev_x = MapX: prev_y = MapY → SetupBackground → Center prev_x, prev_y → RedrawScreen
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
            f.GUI.SetupBackgroundHandler = (m, opt, color, trans) =>
            {
                log.Add("SetupBackground " + m);
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

            var ret = CreateCmd(f.Src, cmdName + " 非同期").Exec();

            Assert.AreEqual(1, ret);
            CollectionAssert.AreEqual(new[] { "SetupBackground " + mode, "Center 4,2", "RedrawScreen True" }, log);
            Assert.AreEqual(4, f.GUI.MapX);
            Assert.AreEqual(2, f.GUI.MapY);
        }

        [TestMethod]
        public void Upgrade_StoredUnit_LandsOnCarrier()
        {
            // 元の SRC: 格納されていたユニットは .UnloadUnit u1.ID: u2.Land u, True で母艦に格納し直す。
            // Land が状態を「格納」にし、座標を母艦に合わせ、地形を「地上」（宇宙・空中以外）にする
            var f = new UnitMapActionFixture();
            f.AddUnitData("母艦", features: "母艦");
            f.AddUnitData("旧機体");
            f.AddUnitData("新機体");
            var carrier = f.Deploy(f.CreateUnit("母艦"), 5, 6);
            var u1 = f.CreateUnit("旧機体", "味方", f.CreatePilot("パイロット"));
            u1.Status = "格納";
            u1.Area = "水中";
            carrier.LoadUnit(u1);

            var ret = CreateCmd(f.Src, "Upgrade 旧機体 新機体").Exec();

            Assert.AreEqual(1, ret);
            var u2 = carrier.UnitOnBoards.Single();
            Assert.AreEqual("新機体", u2.Name);
            Assert.AreEqual("格納", u2.Status);
            Assert.AreEqual((5, 6), (u2.x, u2.y));
            Assert.AreEqual("地上", u2.Area);
            Assert.AreEqual("破棄", u1.Status);
            Assert.AreSame(u2, f.Src.PList.Item("パイロット").Unit);
        }

        [TestMethod]
        public void Wait_Click_RightButton_ClearsMouseButton()
        {
            // 元の SRC: 右ボタンでクリック待ちを抜けたときは MouseButton = 0（押されたボタンの記録を消す）
            var gui = new MockGUI
            {
                MainFormVisible = true,
                MouseButton = GuiButton.Right,
                SleepHandler = (ms, e) => { },
                IsRButtonPressedHandler = _ => true,
            };
            var src = CreateSrc(gui);

            var ret = CreateCmd(src, "Wait Click").Exec();

            Assert.AreEqual(1, ret);
            Assert.AreEqual(GuiButton.None, gui.MouseButton);
            Assert.IsFalse(gui.IsFormClicked);
            Assert.IsFalse(src.Commands.WaitClickMode);
        }

        [TestMethod]
        public void WhiteIn_RightButton_StillSavesScreenAndFades()
        {
            // 元の SRC の WhiteIn は FadeIn と違い、最初に右ボタンを確かめて省略しない。
            // 画面を退避し、フェードの途中で右ボタンを見て元の画面に戻す
            var log = new List<string>();
            var gui = new MockGUI
            {
                IsRButtonPressedHandler = _ => true,
                SaveScreenHandler = () => log.Add("SaveScreen"),
                TransionScreanHandler = (p, c, n, ms) => log.Add("TransionScrean " + p + " " + n),
                RefreshScreenHandler = (a, b) => log.Add("RefreshScreen"),
            };
            var src = CreateSrc(gui);

            var ret = CreateCmd(src, "WhiteIn 5").Exec();

            Assert.AreEqual(1, ret);
            CollectionAssert.AreEqual(new[] { "SaveScreen", "TransionScrean FadeIn 5" }, log);
        }

        [TestMethod]
        public void WhiteIn_RightButton_WrongArgCountIsError()
        {
            // 元の SRC: 右ボタンが押されていても引数の数を確かめる
            var gui = new MockGUI
            {
                IsRButtonPressedHandler = _ => true,
                SaveScreenHandler = () => { },
                TransionScreanHandler = (p, c, n, ms) => { },
                RefreshScreenHandler = (a, b) => { },
            };
            var src = CreateSrc(gui);

            var ret = CreateCmd(src, "WhiteIn 1 2").Exec();

            Assert.AreEqual(-1, ret);
        }
    }
}
