using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// ShowImage / ClearImage は描いた（消した）結果をすぐに画面へ反映する。
    /// VB: CmdData.cls ExecShowImageCmd / ExecClearImageCmd の MainForm.picMain(0).Refresh
    /// </summary>
    [TestClass]
    public class ShowImageRefreshTests
    {
        private static (SRC src, MockGUI gui, List<string> calls) CreateSrc()
        {
            var calls = new List<string>();
            var gui = new MockGUI();
            gui.MainFormShowHandler = () => { calls.Add("MainFormShow"); gui.MainFormVisible = true; };
            gui.DrawPictureHandler = (fname, dx, dy, dw, dh, sx, sy, sw, sh, opt) =>
            {
                calls.Add($"DrawPicture {fname} {dw} {dh}");
                return true;
            };
            gui.ClearPictureHandler = () => calls.Add("ClearPicture");
            gui.UpdateScreenHandler = () => calls.Add("UpdateScreen");
            var src = new SRC { GUI = gui };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new List<string>();
            return (src, gui, calls);
        }

        private static CmdData CreateCmd(SRC src, string cmdText)
        {
            var line = new EventDataLine(src.Event.EventData.Count, EventDataSource.Scenario, "test", src.Event.EventData.Count, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        [TestMethod]
        public void ShowImage_DrawsAndRefreshesScreen()
        {
            var (src, gui, calls) = CreateSrc();
            gui.MainFormVisible = true;
            var cmd = CreateCmd(src, "ShowImage test.png 128 64");
            Assert.AreEqual(1, cmd.Exec());
            CollectionAssert.AreEqual(new[] { "DrawPicture test.png 128 64", "UpdateScreen" }, calls);
        }

        [TestMethod]
        public void ShowImage_ShowsMainFormWhenHidden()
        {
            var (src, gui, calls) = CreateSrc();
            gui.MainFormVisible = false;
            var cmd = CreateCmd(src, "ShowImage test.png");
            cmd.Exec();
            CollectionAssert.AreEqual(new[] { "MainFormShow", $"DrawPicture test.png {Constants.DEFAULT_LEVEL} {Constants.DEFAULT_LEVEL}", "UpdateScreen" }, calls);
        }

        [TestMethod]
        public void ClearImage_ClearsAndRefreshesScreen()
        {
            var (src, _, calls) = CreateSrc();
            var cmd = CreateCmd(src, "ClearImage");
            Assert.AreEqual(1, cmd.Exec());
            CollectionAssert.AreEqual(new[] { "ClearPicture", "UpdateScreen" }, calls);
        }
    }
}
