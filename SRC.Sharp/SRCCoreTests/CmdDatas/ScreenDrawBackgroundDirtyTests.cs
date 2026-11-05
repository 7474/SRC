using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// 描画コマンドで背景に描き込んだら、マップに書き込みがあった（IsMapDirty）と記録する。
    /// 中断・クイックセーブから再開するときに、再開イベントで書き直すかどうかの判定に使う
    /// （元の SRC の CmdData.cls の各描画コマンド。7474/SRC#1466）。
    /// </summary>
    [TestClass]
    public class ScreenDrawBackgroundDirtyTests
    {
        private static SRC CreateSrc(string drawOption)
        {
            var src = new SRC();
            var gui = new HeadlessGUI();
            gui.Handle("get_MapPWidth", _ => 480);
            gui.Handle("get_MapPHeight", _ => 480);
            gui.Install(src);
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventQue = new List<string>();
            src.Event.ObjDrawOption = drawOption;
            return src;
        }

        private static void Exec(SRC src, string cmdText)
        {
            var line = new EventDataLine(0, EventDataSource.Scenario, "test", 0, cmdText);
            src.Event.EventData.Add(line);
            var cmd = new CmdParser().Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            cmd.Exec();
        }

        [DataTestMethod]
        [DataRow("Arc 100 100 50 0 90")]
        [DataRow("Circle 100 100 50")]
        [DataRow("Line 10 10 100 100")]
        [DataRow("Oval 100 100 50 0.5")]
        [DataRow("PSet 10 10")]
        [DataRow("Polygon 10 10 100 10 50 100")]
        public void DrawOnBackground_MarksMapDirty(string cmdText)
        {
            var src = CreateSrc("背景");

            Exec(src, cmdText);

            Assert.IsTrue(src.Map.IsMapDirty);
        }

        [DataTestMethod]
        [DataRow("Circle 100 100 50")]
        [DataRow("Line 10 10 100 100")]
        public void DrawOnScreen_DoesNotMarkMapDirty(string cmdText)
        {
            var src = CreateSrc("");

            Exec(src, cmdText);

            Assert.IsFalse(src.Map.IsMapDirty);
        }
    }
}
