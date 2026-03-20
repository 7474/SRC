using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Events;
using SRCCore.TestLib;
using System.Collections.Generic;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Load / Move / CallIntermissionCommand / MakeUnitList / MakePilotList / QuickLoad
    /// コマンドのユニットテスト
    /// ヘルプの記載に基づく期待値を検証する
    ///
    /// 備考:
    /// - ABGMCmd / AIfCmd / ATalkCmd は抽象基底クラスであり、具象サブクラス
    ///   (PlayMIDICmd / IfCmd / TalkCmd 等) を通じて既にテスト済みのため除外。
    ///   (BGMCmdTests.cs / ElseIfLoopNextCmdTests.cs / TalkAndAskCmdTests.cs 等で網羅)
    /// - MakeUnitListCmd / MakePilotListCmd / QuickLoadCmd は GUI 依存が深く
    ///   (MockGUI の多数のハンドラ設定が必要)、型確認テストに留めた。
    /// - FontCmd は System.Drawing.Font が Linux 環境でランタイムエラーが発生するためテスト除外。
    ///   詳細は FontClsTelopCmdTests.cs を参照。
    /// </summary>
    [TestClass]
    public class LoadMoveCmdTests
    {
        private SRC CreateSrc()
        {
            var src = new SRC { GUI = new MockGUI() };
            src.Event.EventData = new List<EventDataLine>();
            src.Event.EventCmd = new List<CmdData>();
            src.Event.EventFileNames = new List<string>();
            src.Event.AdditionalEventFileNames = new List<string>();
            src.Event.EventQue = new Queue<string>();
            src.Titles = new List<string>();
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
        // LoadCmd
        // ヘルプ: 指定した作品 title のデータをロードします。
        //         指定した title のデータが既にロード済みの場合はロードは行われません。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void LoadCmd_NoTitleArgs_ReturnsNextId()
        {
            // ヘルプ: Load title — title が指定されなければロードは行われない
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Load");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void LoadCmd_TitleAlreadyLoaded_SkipsLoadAndReturnsNextId()
        {
            // ヘルプ: 指定した title のデータが既にロード済みの場合はロードは行われません
            var src = CreateSrc();
            src.Titles.Add("ドラゴンライダーズ");
            var cmd = CreateCmd(src, "Load ドラゴンライダーズ");
            var result = cmd.Exec();
            // 既にロード済みなので new_titles は空 → GUI 操作なしに NextID を返す
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void LoadCmd_ParsesCorrectly()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Load ドラゴンライダーズ");
            Assert.IsInstanceOfType(cmd, typeof(LoadCmd));
        }

        // ──────────────────────────────────────────────
        // MoveCmd
        // ヘルプ: unit を (x,y) 地点まで移動させます。
        //         移動力による制限を受けません。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MoveCmd_NonExistentUnit_ReturnsError()
        {
            // 存在しないパイロット名を指定した場合はエラーになる
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Move 存在しないパイロット 1 1");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void MoveCmd_ParsesCorrectly()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "Move ミルク 5 5");
            Assert.IsInstanceOfType(cmd, typeof(MoveCmd));
        }

        // ──────────────────────────────────────────────
        // CallIntermissionCommandCmd
        // ヘルプ: 本体に実装されたインターミッションコマンドの機能を呼び出します。
        //         command には "ユニットの強化", "乗り換え", "アイテム交換",
        //         "換装", "パイロットステータス", "ユニットステータス", "データセーブ" が指定可能
        // ──────────────────────────────────────────────

        [TestMethod]
        public void CallIntermissionCommandCmd_WrongArgCount_ReturnsError()
        {
            // ヘルプ: CallIntermissionCommand command — 引数は1個のみ必須
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CallIntermissionCommand");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CallIntermissionCommandCmd_TooManyArgs_ReturnsError()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CallIntermissionCommand データセーブ 余分な引数");
            var result = cmd.Exec();
            Assert.AreEqual(-1, result);
        }

        [TestMethod]
        public void CallIntermissionCommandCmd_UnknownCommand_ReturnsNextId()
        {
            // スイッチの default ケース (未知のコマンド) は何もせずに NextID を返す
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CallIntermissionCommand 知らないコマンド");
            var result = cmd.Exec();
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void CallIntermissionCommandCmd_ParsesCorrectly()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "CallIntermissionCommand ユニットの強化");
            Assert.IsInstanceOfType(cmd, typeof(CallIntermissionCommandCmd));
        }

        // ──────────────────────────────────────────────
        // MakeUnitListCmd
        // ヘルプ: ユニット一覧画面を作成し、表示します。
        //         mode にはユニットのソートに使う能力値を指定します。
        // 備考: GUI 依存が深いため (MockGUI の多数のハンドラ設定が必要)、型確認テストに留める。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MakeUnitListCmd_ParsesCorrectly()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "MakeUnitList ＨＰ");
            Assert.IsInstanceOfType(cmd, typeof(MakeUnitListCmd));
        }

        // ──────────────────────────────────────────────
        // MakePilotListCmd
        // ヘルプ: パイロット一覧画面を作成し、表示します。
        //         mode にはパイロットのソートに使う能力値を指定します。
        // 備考: GUI 依存が深いため (MockGUI の多数のハンドラ設定が必要)、型確認テストに留める。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void MakePilotListCmd_ParsesCorrectly()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "MakePilotList レベル");
            Assert.IsInstanceOfType(cmd, typeof(MakePilotListCmd));
        }

        // ──────────────────────────────────────────────
        // QuickLoadCmd
        // ヘルプ: 最後にクイックセーブした時点からプレイを再開します。
        // 備考: GUI 依存が深いため (MockGUI の多数のハンドラ設定が必要)、型確認テストに留める。
        // ──────────────────────────────────────────────

        [TestMethod]
        public void QuickLoadCmd_ParsesCorrectly()
        {
            var src = CreateSrc();
            var cmd = CreateCmd(src, "QuickLoad");
            Assert.IsInstanceOfType(cmd, typeof(QuickLoadCmd));
        }
    }
}
