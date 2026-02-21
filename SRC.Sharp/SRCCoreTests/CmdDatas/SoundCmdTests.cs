using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// RenameBGM コマンドのユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// </summary>
    [TestClass]
    public class SoundCmdTests
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
        // RenameBGMCmd
        // ヘルプ: BGMに使用されるMIDIファイル名を変更
        // 書式: RenameBGM bgm file
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RenameBGMCmd_Map1_StoresInGlobalVariable()
        {
            // ヘルプ: Map1 → 味方フェイズ開始時
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameBGM Map1 my_map1.mid");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("my_map1.mid", src.Expression.GetValueAsString("BGM(Map1)"));
        }

        [TestMethod]
        public void RenameBGMCmd_Map2_StoresInGlobalVariable()
        {
            // ヘルプ: Map2 → 敵フェイズ開始時
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameBGM Map2 enemy_bgm.mid");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("enemy_bgm.mid", src.Expression.GetValueAsString("BGM(Map2)"));
        }

        [TestMethod]
        public void RenameBGMCmd_Intermission_StoresInGlobalVariable()
        {
            // ヘルプ: Intermission → インターミッション時
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameBGM Intermission intermission2.mid");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("intermission2.mid", src.Expression.GetValueAsString("BGM(Intermission)"));
        }

        [TestMethod]
        public void RenameBGMCmd_Default_StoresInGlobalVariable()
        {
            // ヘルプ: default → 戦闘時にパイロットデータに設定されたMIDIファイルが見つからない際
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameBGM default fallback.mid");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("fallback.mid", src.Expression.GetValueAsString("BGM(default)"));
        }

        [TestMethod]
        public void RenameBGMCmd_End_StoresInGlobalVariable()
        {
            // ヘルプ: End → ゲームオーバー時
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameBGM End gameover.mid");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("gameover.mid", src.Expression.GetValueAsString("BGM(End)"));
        }

        [TestMethod]
        public void RenameBGMCmd_InvalidBGMName_ReturnsError()
        {
            // 不正なBGM名を指定した場合エラー
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameBGM InvalidName music.mid");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RenameBGMCmd_WrongArgCount_ReturnsError()
        {
            // 書式違反: 引数が足りない
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameBGM Map1");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RenameBGMCmd_NoArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "RenameBGM");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void RenameBGMCmd_AllValidBGMNames_AcceptsAll()
        {
            // ヘルプに記載された全BGMシチュエーションが有効
            var validNames = new[] { "Map1", "Map2", "Map3", "Map4", "Map5", "Map6",
                "Briefing", "Intermission", "Subtitle", "End", "default" };
            foreach (var bgmName in validNames)
            {
                var src = CreateSrc();
                var cmd = CreateCmd(src, $"RenameBGM {bgmName} test.mid");
                var result = cmd.Exec();
                Assert.AreEqual(1, result, $"BGM名「{bgmName}」が受け入れられませんでした");
            }
        }
    }
}
