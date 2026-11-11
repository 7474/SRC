using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Coverage.Tests;
using SRCCore.Events;
using System.Collections.Generic;

namespace SRCCore.Coverage.Commands.Tests
{
    /// <summary>
    /// CmdData.cls の ExecColorFilterCmd・ExecCreateCmd・ExecCopyArrayCmd などを、元の SRC の動作を期待値にして確かめる（7474/SRC#1503）。
    /// </summary>
    [TestClass]
    public class CmdDataCloseToDrawVBMatchTests
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

        private static MapCoverageFixture CreateFixture()
        {
            var f = new MapCoverageFixture();
            f.Src.Event.EventData = new List<EventDataLine>();
            f.Src.Event.EventCmd = new List<CmdData>();
            f.Src.Event.EventQue = new List<string>();
            f.GUI.ChangeStatusHandler = _ => { };
            return f;
        }

        [TestMethod]
        public void ColorFilter_RestoresMapPositionAfterSetupBackground()
        {
            // 元の SRC: prev_x = MapX: prev_y = MapY → SetupBackground（MapX・MapY を中央にする）→ Center prev_x, prev_y → RedrawScreen
            var f = CreateFixture();
            f.LoadGrid(
                ".....",
                ".....",
                ".....");
            f.GUI.MapX = 4;
            f.GUI.MapY = 2;
            var log = new List<string>();
            f.GUI.SetupBackgroundHandler = (mode, opt, color, trans) =>
            {
                log.Add("SetupBackground");
                f.GUI.MapX = f.GUI.MainWidth / 2 + 1;
                f.GUI.MapY = f.GUI.MainHeight / 2 + 1;
            };
            f.GUI.CenterHandler = (x, y) =>
            {
                log.Add("Center " + x + "," + y);
                f.GUI.MapX = x;
                f.GUI.MapY = y;
            };
            f.GUI.RedrawScreenHandler = _ => log.Add("RedrawScreen");

            var ret = CreateCmd(f.Src, "ColorFilter #FF0000").Exec();

            Assert.AreEqual(1, ret);
            CollectionAssert.AreEqual(new[] { "SetupBackground", "Center 4,2", "RedrawScreen" }, log);
            Assert.AreEqual(4, f.GUI.MapX);
            Assert.AreEqual(2, f.GUI.MapY);
        }

        [TestMethod]
        public void Create_CentersMapBeforeStandBy()
        {
            // 元の SRC: Center ux, uy・RefreshScreen を、FullRecover・StandBy（出撃のアニメーションと PaintUnitBitmap）より前に行う
            var f = CreateFixture();
            f.LoadGrid(
                ".....",
                ".....",
                ".....");
            f.DefineUnit("歩兵", "陸", 3);
            var pd = f.Src.PDList.Add("隊員");
            pd.Nickname = "隊員";
            pd.Personality = "普通";
            f.GUI.MainFormVisible = true;
            f.GUI.IsPictureVisible = false;
            var log = new List<string>();
            f.GUI.CenterHandler = (x, y) => log.Add("Center " + x + "," + y);
            f.GUI.RefreshScreenHandler = (_, _) => log.Add("RefreshScreen");
            f.GUI.PaintUnitBitmapHandler = (u, _) => log.Add("PaintUnitBitmap " + u.x + "," + u.y);

            var ret = CreateCmd(f.Src, "Create 味方 歩兵 0 隊員 1 3 2 アニメ非表示").Exec();

            Assert.AreEqual(1, ret);
            CollectionAssert.AreEqual(new[] { "Center 3,2", "RefreshScreen", "PaintUnitBitmap 3,2" }, log);
            Assert.AreEqual("出撃", f.Src.Event.SelectedUnitForEvent.Status);
        }

        [TestMethod]
        public void CopyArray_GlobalToGlobal_CopiesElementsInDefinedOrder()
        {
            // 元の SRC: GlobalVariableList（Collection）を定義した順に列挙し、要素ごとに SetVariable する
            var f = CreateFixture();
            CreateCmd(f.Src, "Global a").Exec();
            CreateCmd(f.Src, "Global b").Exec();
            f.Src.Expression.SetVariableAsString("a[2]", "two");
            f.Src.Expression.SetVariableAsString("a[1]", "one");
            f.Src.Expression.SetVariableAsString("b[9]", "old");

            var ret = CreateCmd(f.Src, "CopyArray a b").Exec();

            Assert.AreEqual(3, ret);
            Assert.AreEqual("two", f.Src.Expression.GetValueAsString("b[2]"));
            Assert.AreEqual("one", f.Src.Expression.GetValueAsString("b[1]"));
            // コピー先は初期化される
            Assert.AreEqual("", f.Src.Expression.GetValueAsString("b[9]"));
        }
    }
}
