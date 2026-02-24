using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// ClsCmd / TelopCmd のユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    /// 注意: FontCmd は System.Drawing.Font が Linux 非対応のためテスト除外
    /// </summary>
    [TestClass]
    public class FontClsTelopCmdTests
    {
        /// <summary>テスト用の最小 ISystemConfig 実装</summary>
        private class NullSystemConfig : ISystemConfig
        {
            public SRCCompatibilityMode SRCCompatibilityMode { get; set; }
            public bool KeepEnemyBGM { get; set; }
            public bool AutoMoveCursor { get; set; }
            public bool ShowSquareLine { get; set; }
            public bool SpecialPowerAnimation { get; set; }
            public bool BattleAnimation { get; set; }
            public bool WeaponAnimation { get; set; }
            public bool ExtendedAnimation { get; set; }
            public bool MoveAnimation { get; set; }
            public string MidiResetType { get; set; }
            public int ImageBufferSize { get; set; }
            public int MaxImageBufferByteSize { get; set; }
            public bool KeepStretchedImage { get; set; }
            public bool AutoDefense { get; set; }
            public string AppPath { get; set; }
            public string ExtDataPath { get; set; }
            public string ExtDataPath2 { get; set; }
            public string GetItem(string section, string name) => "";
            public void SetItem(string section, string name, string value) { }
            public void Save() { }
            public void Load() { }
        }

        private (SRC src, MockGUI gui) CreateSrc()
        {
            var gui = new MockGUI();
            var src = new SRC { GUI = gui, SystemConfig = new NullSystemConfig() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new System.Collections.Generic.Queue<string>();
            return (src, gui);
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
        // ClsCmd
        // ヘルプ: マップウィンドウの表示を消去し黒で塗りつぶす。色指定可
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ClsCmd_NoArgs_ReturnsNextId()
        {
            // ヘルプ: Cls → マップウィンドウを消去し黒色で塗りつぶす
            var (src, gui) = CreateSrc();
            src.GUIScrean = new MockGUIScrean();
            var cmd = CreateCmd(src, "Cls");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ClsCmd_NoArgs_SetsPictureVisible()
        {
            // Cls実行後 → IsPictureVisible が true に設定される
            var (src, gui) = CreateSrc();
            src.GUIScrean = new MockGUIScrean();
            gui.IsPictureVisible = false;
            var cmd = CreateCmd(src, "Cls");
            cmd.Exec();
            Assert.IsTrue(gui.IsPictureVisible);
        }

        [TestMethod]
        public void ClsCmd_WithValidColor_ReturnsNextId()
        {
            // ヘルプ: rgb に RGBの16進を指定すると指定した色で塗りつぶす
            var (src, _) = CreateSrc();
            src.GUIScrean = new MockGUIScrean();
            var cmd = CreateCmd(src, "Cls #FFFFFF");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void ClsCmd_WithInvalidColor_ReturnsError()
        {
            // 不正な色指定 → エラー
            var (src, _) = CreateSrc();
            src.GUIScrean = new MockGUIScrean();
            var cmd = CreateCmd(src, "Cls BADCOLOR");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void ClsCmd_WrongArgCount_ReturnsError()
        {
            // 引数が3個以上 → エラー
            var (src, _) = CreateSrc();
            src.GUIScrean = new MockGUIScrean();
            var cmd = CreateCmd(src, "Cls #FFFFFF #000000");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        // ──────────────────────────────────────────────
        // TelopCmd
        // ヘルプ: message をテロップとして1秒間画面に表示する
        // ──────────────────────────────────────────────

        [TestMethod]
        public void TelopCmd_DisplaysMessage()
        {
            // ヘルプ: message をテロップとして画面に表示する
            // BGM(Subtitle)="" にすることで SearchMidiFile が早期リターンし FileSystem 不要
            var (src, gui) = CreateSrc();
            src.Expression.DefineGlobalVariable("BGM(Subtitle)");
            src.Expression.SetVariableAsString("BGM(Subtitle)", "");
            string capturedMsg = null;
            gui.DisplayTelopHandler = msg => capturedMsg = msg;
            // 複数単語メッセージは GetValueAsString での変数評価をスキップする
            var cmd = CreateCmd(src, "Telop テスト メッセージ");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
            Assert.AreEqual("テスト メッセージ", capturedMsg);
        }

        [TestMethod]
        public void TelopCmd_ReturnsNextId()
        {
            // TelopCmd は正常終了時に NextID を返す
            var (src, gui) = CreateSrc();
            src.Expression.DefineGlobalVariable("BGM(Subtitle)");
            src.Expression.SetVariableAsString("BGM(Subtitle)", "");
            gui.DisplayTelopHandler = _ => { };
            var cmd = CreateCmd(src, "Telop 敵か 味方か！");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }
    }
}
