using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// スクリーン関連コマンドのユニットテスト
    /// (Show, Hide, Redraw, Refresh, ClearPicture, ClearObj, ClearLayer, ClearImage)
    /// </summary>
    [TestClass]
    public class ScreanCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new Queue<string>();
            return src;
        }

        private CmdData CreateCmd(SRC src, string cmdText, int id = 0)
        {
            var line = new EventDataLine(id, EventDataSource.Scenario, "test", id, cmdText);
            src.Event.EventData.Add(line);
            var parser = new CmdParser();
            var cmd = parser.Parse(src, line);
            src.Event.EventCmd.Add(cmd);
            return cmd;
        }

        // ──────────────────────────────────────────────
        // ShowCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ShowCmd_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Show");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // HideCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void HideCmd_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Hide");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // RedrawCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RedrawCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Redraw");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void RedrawCmd_WithAsync_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Redraw 非同期");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void RedrawCmd_WithOtherArg_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Redraw 同期");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // RefreshCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RefreshCmd_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Refresh");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // ClearPictureCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearPictureCmd_NoArgs_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearPicture");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ClearPictureCmd_WithCoordinates_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearPicture 0 0 100 100");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ClearPictureCmd_WrongArgCount_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearPicture 10 20");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ClearObjCmd は GUI.UpdateHotPoint() を呼び出すが、
        // MockGUI では GUINotImplementedException が発生し、
        // CmdData.Exec() がこれをキャッチして NextID を返す。

        // ──────────────────────────────────────────────
        // ClearObjCmd
        // ヘルプ: ホットスポットオブジェクトをクリアする
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearObjCmd_NoArgs_ReturnsNextId()
        {
            // ヘルプ: ClearObj - すべてのホットスポットをクリアする
            var src = CreateSrc();
            src.Event.HotPointList = new System.Collections.Generic.List<HotPoint>();
            var cmd = CreateCmd(src, "ClearObj");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ClearObjCmd_WithArg_ReturnsNextId()
        {
            // ヘルプ: ClearObj name - 名前付きホットスポットを削除する
            var src = CreateSrc();
            src.Event.HotPointList = new System.Collections.Generic.List<HotPoint>();
            var cmd = CreateCmd(src, "ClearObj MyHotspot");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ClearObjCmd_WrongArgCount_ReturnsError()
        {
            // 引数が3個以上はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearObj a b c");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // ClearLayerCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearLayerCmd_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearLayer");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        // ──────────────────────────────────────────────
        // ClearImageCmd
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClearImageCmd_ReturnsNextId()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "ClearImage");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }
    }
}
