using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Exec / SaveData コマンドのユニットテスト
    /// ヘルプ「Execコマンド.md」「SaveDataコマンド.md」に基づく
    /// </summary>
    [TestClass]
    public class ExecSaveDataCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
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
        // ExecCmd
        // ヘルプ: Exec file [option]
        // 引数は2〜3個でなければエラー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ExecCmd_NoArgs_ReturnsError()
        {
            // 書式: Exec file [option] (引数1個のみはエラー)
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Exec");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ExecCmd_TooManyArgs_ReturnsError()
        {
            // 引数が4個以上はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Exec file.eve 通常ステージ 余分");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // SaveDataCmd
        // ヘルプ: SaveData [file]
        // 引数は1〜2個でなければエラー
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SaveDataCmd_TooManyArgs_ReturnsError()
        {
            // 引数が3個以上はエラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "SaveData file1.src file2.src");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void SaveDataCmd_NoArgs_CallsGUISelectSaveStream()
        {
            // SaveData (引数なし) → GUI.SelectSaveStream() が呼ばれ、null を返すのでセーブされずに NextID を返す
            var src = CreateSrc();
            var mockGui = (MockGUI)src.GUI;
            // SelectSaveStream が null を返すようモックを設定 (デフォルトは null → save はスキップ)
            mockGui.SelectSaveStreamHandler = (_, __) => null;
            var cmd = CreateCmd(src, "SaveData");
            var result = cmd.Exec();
            // セーブなし → NextID (=1) を返す
            Assert.AreEqual(1, result);
        }
    }
}
