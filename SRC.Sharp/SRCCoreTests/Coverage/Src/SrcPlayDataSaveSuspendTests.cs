using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SRCCore.Lib;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// SRC.SaveData / LoadData（クリアデータ）と DumpData / RestoreData（中断・クイックセーブ・リスタート）
    /// 元の SRC.bas の同名関数
    /// </summary>
    [TestClass]
    public class SrcPlayDataSaveSuspendTests
    {
        private static readonly string[] Scenario =
        {
            // 先頭行のラベルは IsEventDefined が見落とすため、ダミーのラベルを先頭に置く
            "準備:",
            "Exit",
            "",
            "再開:",
            "Global 再開回数",
            "Incr 再開回数",
            "Exit",
            "",
            "スタート:",
            "Global スタート回数",
            "Incr スタート回数",
            "Exit",
            "",
            "ターン 2 味方:",
            "Exit",
        };

        /// <summary>
        /// シナリオ test.eve を読み込み、3x3 マップを用意した中断前の状態を作る。
        /// </summary>
        private static string PrepareScenario(SrcPlayDataSaveFixture f)
        {
            var path = f.Write("test.eve", Scenario);
            f.Src.ScenarioFileName = path;
            f.Src.Event.LoadEventData(path, "");
            f.Src.Map.SetMapSize(3, 3);
            f.Src.Map.MapFileName = Path.Combine(f.ScenarioDir, "test.map");
            f.Src.Turn = 3;
            f.Src.TotalTurn = 10;
            f.Src.Money = 5000;
            f.Src.Expression.DefineGlobalVariable("進行度");
            f.Src.Expression.SetVariableAsLong("進行度", 7);
            return path;
        }

        private static MemoryStream Dump(SrcPlayDataSaveFixture f, SRCSaveKind kind)
        {
            var stream = new MemoryStream();
            f.Src.DumpData(stream, kind);
            return new MemoryStream(stream.ToArray());
        }

        private static SRCSuspendData Parse(MemoryStream stream)
        {
            return JsonConvert.DeserializeObject<SRCSuspendData>(Encoding.UTF8.GetString(stream.ToArray()));
        }

        // ── SaveData / LoadData ──

        [TestMethod]
        public void SaveData_LoadData_RestoresProgressAndIncludesTitles()
        {
            // クリアデータには使用作品、次ステージ、総ターン数、資金、グローバル変数を保存し、
            // ロード時には使用作品のデータを読み込み直す
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/作品/alias.txt", "作品エリアス", "バリア=バリア 1000", "");
            f.Src.Titles = new List<string> { "作品" };
            f.Src.Expression.DefineGlobalVariable("次ステージ");
            f.Src.Expression.SetVariableAsString("次ステージ", "stage2.eve");
            f.Src.Expression.DefineGlobalVariable("クリア数");
            f.Src.Expression.SetVariableAsLong("クリア数", 4);
            f.Src.TotalTurn = 42;
            f.Src.Money = 12345;
            var stream = new MemoryStream();
            f.Src.SaveData(stream);

            f.NewSession();
            f.Src.LoadData(new MemoryStream(stream.ToArray()));

            CollectionAssert.AreEqual(new[] { "作品" }, f.Src.Titles.ToArray());
            Assert.AreEqual("stage2.eve", f.Src.Expression.GetValueAsString("次ステージ"));
            Assert.AreEqual(4d, f.Src.Expression.GetValueAsDouble("クリア数"));
            Assert.AreEqual(42, f.Src.TotalTurn);
            Assert.AreEqual(12345, f.Src.Money);
            Assert.IsTrue(f.Src.ALDList.IsDefined("作品エリアス"));
            // ロード画面の進捗は 使用作品数 * 2 + 5
            Assert.AreEqual(7, f.Gui.CallsOf(nameof(IGUI.SetLoadImageSize)).Single().Args[0]);
            Assert.IsFalse(f.Gui.IsTerminated);
        }

        [TestMethod]
        public void SaveData_WritesNormalKind()
        {
            using var f = new SrcPlayDataSaveFixture();
            f.Src.Expression.DefineGlobalVariable("次ステージ");
            f.Src.Money = 100;
            var stream = new MemoryStream();

            f.Src.SaveData(stream);

            var data = JsonConvert.DeserializeObject<SRCSaveData>(Encoding.UTF8.GetString(stream.ToArray()));
            Assert.AreEqual(SRCSaveKind.Normal, data.Kind);
            Assert.AreEqual(100, data.Money);
        }

        [TestMethod]
        public void SaveData_WriteFailure_ShowsError()
        {
            // 書き込みに失敗したらエラーメッセージを表示する（終了はしない）
            using var f = new SrcPlayDataSaveFixture();
            f.Src.Expression.DefineGlobalVariable("次ステージ");

            f.Src.SaveData(new MemoryStream(new byte[0], false));

            Assert.AreEqual("セーブ中にエラーが発生しました",
                f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Single().Args[0]);
            Assert.IsFalse(f.Gui.IsTerminated);
        }

        [TestMethod]
        public void LoadData_BrokenData_ShowsErrorAndTerminates()
        {
            using var f = new SrcPlayDataSaveFixture();

            SrcPlayDataSaveFixture.Run(() => f.Src.LoadData(new MemoryStream(Encoding.UTF8.GetBytes("壊れたデータ"))));

            Assert.IsTrue(f.Gui.CallsOf(nameof(IGUI.ErrorMessage))
                .Any(x => (string)x.Args[0] == "ロード中にエラーが発生しました"));
            Assert.IsTrue(f.Gui.IsTerminated);
        }

        // ── DumpData ──

        [TestMethod]
        [DataRow(SRCSaveKind.Suspend, false, false)]
        [DataRow(SRCSaveKind.Quik, true, false)]
        [DataRow(SRCSaveKind.Restart, false, true)]
        public void DumpData_SetsAvailabilityFlagByKind(SRCSaveKind kind, bool quick, bool restart)
        {
            // クイックセーブ・リスタート用データを書いたらそれぞれロード可能にする
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);

            var data = Parse(Dump(f, kind));

            Assert.AreEqual(kind, data.Kind);
            Assert.AreEqual(quick, f.Src.IsQuickSaveDataAvailable);
            Assert.AreEqual(restart, f.Src.IsRestartSaveDataAvailable);
        }

        [TestMethod]
        public void DumpData_WritesScenarioStateAndDisabledLabels()
        {
            // 中断データにはシナリオファイル、ターン数、マップ、無効化されたイベントラベルなどを保存する
            using var f = new SrcPlayDataSaveFixture();
            var path = PrepareScenario(f);
            f.Src.Event.colEventLabelList.List.First(x => x.Data == "ターン 2 味方").Enable = false;

            var data = Parse(Dump(f, SRCSaveKind.Suspend));

            Assert.AreEqual(path, data.ScenarioFileName);
            Assert.AreEqual(3, data.Turn);
            Assert.AreEqual(10, data.TotalTurn);
            Assert.AreEqual(5000, data.Money);
            CollectionAssert.AreEqual(new[] { "ターン 2 味方" }, data.DisableEventLabels.ToArray());
            Assert.AreEqual(3, data.Map.MapWidth);
            Assert.AreEqual(f.Src.Map.MapFileName, data.Map.MapFileName);
        }

        [TestMethod]
        public void DumpData_StoresBgmPathRelativeToScenario()
        {
            // 演奏中のＢＧＭはシナリオフォルダからの相対パスで保存する
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            f.Write("Midi/Map1.mid", "dummy");
            f.Src.Sound.StartBGM("Map1.mid", false);

            var data = Parse(Dump(f, SRCSaveKind.Suspend));

            // SRC のパス表記（\ 区切り）で保存される
            Assert.AreEqual("Midi/Map1.mid", data.BGMFileName.Replace('\\', '/'));
            Assert.IsFalse(data.RepeatMode);
        }

        [TestMethod]
        public void DumpData_ToFile_SetsLastSaveDataFileName()
        {
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            var fname = Path.Combine(f.ScenarioDir, "中断.srcq");

            f.Src.DumpData(fname, SRCSaveKind.Suspend);

            Assert.AreEqual(fname, f.Src.LastSaveDataFileName);
            Assert.AreEqual(3, JsonConvert.DeserializeObject<SRCSuspendData>(File.ReadAllText(fname)).Turn);
        }

        [TestMethod]
        public void DumpData_WriteFailure_ShowsError()
        {
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);

            f.Src.DumpData(new MemoryStream(new byte[0], false), SRCSaveKind.Quik);

            Assert.AreEqual("セーブ中にエラーが発生しました",
                f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Single().Args[0]);
            Assert.IsFalse(f.Src.IsQuickSaveDataAvailable);
        }

        // ── RestoreData ──

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_Suspend_RestoresStateAndResumesPlayerPhase()
        {
            // 中断データのロード：ロード画面を出してデータ・イベントを読み込み直し、
            // 再開イベントを実行してから味方フェイズのユニット選択に戻る
            using var f = new SrcPlayDataSaveFixture();
            var path = PrepareScenario(f);
            var stream = Dump(f, SRCSaveKind.Suspend);

            f.NewSession();
            f.Src.RestoreData(stream, SRCSaveKind.Suspend);

            Assert.AreEqual(path, f.Src.ScenarioFileName);
            Assert.AreEqual(3, f.Src.Turn);
            Assert.AreEqual(10, f.Src.TotalTurn);
            Assert.AreEqual(5000, f.Src.Money);
            Assert.AreEqual(7d, f.Src.Expression.GetValueAsDouble("進行度"));
            Assert.AreEqual(3, f.Src.Map.MapWidth);
            Assert.IsFalse(f.Src.Map.IsStatusView);
            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("再開回数"));
            Assert.IsFalse(f.Src.Expression.IsGlobalVariableDefined("スタート回数"));
            Assert.AreEqual("味方", f.Src.Stage);
            Assert.AreEqual("ユニット選択", f.Src.Commands.CommandState);
            Assert.IsNull(f.Src.Commands.SelectedUnit);
            Assert.IsTrue(f.Called(nameof(IGUI.OpenNowLoadingForm)));
            Assert.IsTrue(f.Called(nameof(IGUI.CloseNowLoadingForm)));
            Assert.IsTrue(f.Called(nameof(IGUI.MainFormShow)));
            var status = f.Gui.CallsOf(nameof(IGUI.ChangeStatus)).Select(x => x.Args[0]).ToList();
            Assert.AreEqual(GuiStatus.WaitCursor, status.First());
            Assert.AreEqual(GuiStatus.Default, status.Last());
            Assert.IsFalse(f.Src.IsQuickSaveDataAvailable);
            Assert.IsFalse(f.Src.IsRestartSaveDataAvailable);
            Assert.IsFalse(f.Gui.IsTerminated);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_Quick_DoesNotShowLoadingFormAndEnablesQuickLoad()
        {
            // クイックロードではロード画面を出さず、同じシナリオならイベントも読み込み直さない
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            var stream = Dump(f, SRCSaveKind.Quik);
            f.Src.IsQuickSaveDataAvailable = false;
            f.Src.Money = 1;
            f.Src.Turn = 9;
            var mark = f.Gui.Calls.Count;

            f.Src.RestoreData(stream, SRCSaveKind.Quik);

            Assert.AreEqual(5000, f.Src.Money);
            Assert.AreEqual(3, f.Src.Turn);
            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("再開回数"));
            Assert.IsFalse(f.CalledSince(mark, nameof(IGUI.OpenNowLoadingForm)));
            Assert.IsFalse(f.CalledSince(mark, nameof(IGUI.CloseNowLoadingForm)));
            Assert.IsTrue(f.Src.IsQuickSaveDataAvailable);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_Restart_EnablesRestart()
        {
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            var stream = Dump(f, SRCSaveKind.Restart);
            f.Src.IsRestartSaveDataAvailable = false;
            var mark = f.Gui.Calls.Count;

            f.Src.RestoreData(stream, SRCSaveKind.Restart);

            Assert.IsTrue(f.Src.IsRestartSaveDataAvailable);
            Assert.IsFalse(f.CalledSince(mark, nameof(IGUI.OpenNowLoadingForm)));
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_TurnZero_RunsStartEventAndStartsPlayerPhase()
        {
            // スタートイベント前（Turn = 0）のデータならスタートイベントを実行して味方フェイズを開始する
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            f.Src.Turn = 0;
            var stream = Dump(f, SRCSaveKind.Restart);

            f.Src.RestoreData(stream, SRCSaveKind.Restart);

            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("スタート回数"));
            // マップがあるので味方フェイズの開始でターンが進む
            Assert.AreEqual(1, f.Src.Turn);
            Assert.AreEqual("味方", f.Src.Stage);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_RestoresDisabledEventLabels()
        {
            // ClearEvent 等で無効化されたイベントラベルは無効のまま復元する
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            f.Src.Event.colEventLabelList.List.First(x => x.Data == "ターン 2 味方").Enable = false;
            var stream = Dump(f, SRCSaveKind.Suspend);

            f.NewSession();
            f.Src.RestoreData(stream, SRCSaveKind.Suspend);

            var labels = f.Src.Event.colEventLabelList.List;
            Assert.IsFalse(labels.First(x => x.Data == "ターン 2 味方").Enable);
            Assert.IsTrue(labels.First(x => x.Data == "再開").Enable);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_RestoresBgmAndItsModes()
        {
            // 保存したＢＧＭを演奏し直し、連続演奏・固定・ボス用の設定を復元する
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            f.Write("Midi/Map1.mid", "dummy");
            f.Src.Sound.StartBGM("Map1.mid", true);
            f.Src.Sound.KeepBGM = true;
            f.Src.Sound.BossBGM = true;
            var stream = Dump(f, SRCSaveKind.Suspend);

            f.NewSession();
            f.Src.RestoreData(stream, SRCSaveKind.Suspend);

            Assert.IsTrue(f.Src.Sound.BGMFileName.Replace('\\', '/').EndsWith("Midi/Map1.mid"), f.Src.Sound.BGMFileName);
            Assert.IsTrue(f.Src.Sound.RepeatMode);
            Assert.IsTrue(f.Src.Sound.KeepBGM);
            Assert.IsTrue(f.Src.Sound.BossBGM);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_BgmFileMissing_StopsBgm()
        {
            // 保存したＢＧＭが見つからなければＢＧＭを止める
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            var midi = f.Write("Midi/Map1.mid", "dummy");
            f.Src.Sound.StartBGM("Map1.mid", true);
            var stream = Dump(f, SRCSaveKind.Suspend);
            File.Delete(midi);

            f.Src.RestoreData(stream, SRCSaveKind.Suspend);

            Assert.AreEqual("", f.Src.Sound.BGMFileName);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_RestoresRandomSequence()
        {
            // 乱数系列（シードと使用位置）を復元し、ロード後に同じ乱数が出るようにする
            var seed = GeneralLib.RndSeed;
            var index = GeneralLib.RndIndex;
            try
            {
                using var f = new SrcPlayDataSaveFixture();
                PrepareScenario(f);
                GeneralLib.RndSeed = 12345;
                GeneralLib.RndReset();
                GeneralLib.Dice(100);
                GeneralLib.Dice(100);
                var stream = Dump(f, SRCSaveKind.Quik);
                var expected = new[] { GeneralLib.Dice(1000), GeneralLib.Dice(1000), GeneralLib.Dice(1000) };

                GeneralLib.RndSeed = 1;
                GeneralLib.RndReset();
                f.Src.RestoreData(stream, SRCSaveKind.Quik);

                Assert.AreEqual(12345, GeneralLib.RndSeed);
                Assert.AreEqual(2, GeneralLib.RndIndex);
                CollectionAssert.AreEqual(expected, new[] { GeneralLib.Dice(1000), GeneralLib.Dice(1000), GeneralLib.Dice(1000) });
            }
            finally
            {
                GeneralLib.RndSeed = seed;
                GeneralLib.RndReset();
                GeneralLib.RndIndex = index;
            }
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_DebugOption_KeepsCurrentRandomSequence()
        {
            // Option(デバッグ) では乱数系列を復元しない
            var seed = GeneralLib.RndSeed;
            var index = GeneralLib.RndIndex;
            try
            {
                using var f = new SrcPlayDataSaveFixture();
                PrepareScenario(f);
                f.Src.Expression.DefineGlobalVariable("Option(デバッグ)");
                f.Src.Expression.SetVariableAsLong("Option(デバッグ)", 1);
                GeneralLib.RndSeed = 12345;
                GeneralLib.RndReset();
                var stream = Dump(f, SRCSaveKind.Quik);

                GeneralLib.RndSeed = 1;
                GeneralLib.RndReset();
                f.Src.RestoreData(stream, SRCSaveKind.Quik);

                Assert.AreEqual(1, GeneralLib.RndSeed);
            }
            finally
            {
                GeneralLib.RndSeed = seed;
                GeneralLib.RndReset();
                GeneralLib.RndIndex = index;
            }
        }

        [TestMethod]
        [Timeout(20000)]
        [Ignore("移植の不具合: シナリオが異なる中断データのロード時、ウィンドウタイトルにセーブデータ側ではなく直前のシナリオ名を表示する")]
        public void RestoreData_DifferentScenario_SetsTitleFromSavedScenario()
        {
            // VB: MainForm.Caption = "SRC - " & Left$(fname2, Len(fname2) - 4)（fname2 はセーブデータ側のシナリオ）
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            var stream = Dump(f, SRCSaveKind.Suspend);

            f.NewSession();
            f.Src.ScenarioFileName = Path.Combine(f.ScenarioDir, "other.eve");
            f.Src.RestoreData(stream, SRCSaveKind.Suspend);

            Assert.AreEqual("SRC# - test", f.Gui.GUI.MainFormText);
        }

        [TestMethod]
        [Timeout(20000)]
        [Ignore("移植の不具合: マップ表示位置（MapX, MapY）を Map.Restore の SetMapSize より前に設定するため、中央にリセットされてしまう")]
        public void RestoreData_RestoresMapDisplayPosition()
        {
            // VB の RestoreMapData は SetMapSize の後で MapX, MapY を読み込み、Center MapX, MapY する
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            f.Gui.GUI.MapX = 3;
            f.Gui.GUI.MapY = 2;
            var stream = Dump(f, SRCSaveKind.Suspend);

            f.NewSession();
            f.Src.RestoreData(stream, SRCSaveKind.Suspend);

            var center = f.Gui.CallsOf(nameof(IGUI.Center)).Last();
            Assert.AreEqual(3, center.Args[0]);
            Assert.AreEqual(2, center.Args[1]);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_BrokenData_ShowsErrorAndTerminates()
        {
            using var f = new SrcPlayDataSaveFixture();

            SrcPlayDataSaveFixture.Run(() => f.Src.RestoreData(new MemoryStream(Encoding.UTF8.GetBytes("{")), SRCSaveKind.Suspend));

            Assert.IsTrue(f.Gui.CallsOf(nameof(IGUI.ErrorMessage))
                .Any(x => (string)x.Args[0] == "ロード中にエラーが発生しました"));
            Assert.IsTrue(f.Gui.IsTerminated);
        }
    }
}
