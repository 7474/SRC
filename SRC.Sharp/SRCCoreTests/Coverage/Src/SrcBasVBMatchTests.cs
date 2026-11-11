using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using SRCCore.Config;
using SRCCore.Extensions;
using SRCCore.Filesystem;
using SRCCore.Models;
using SRCCore.TestLib;
using SRCCore.Units;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace SRCCore.Coverage.Tests
{
    /// <summary>
    /// 元の SRC（SRC/SRC_20121125/SRC.bas）の Main・StartScenario・StartTurn・LoadData・RestoreData・
    /// IncludeData・ConvertUnitID の動作を期待値にしたテスト（7474/SRC#1498）
    /// </summary>
    [TestClass]
    public class SrcBasVBMatchTests
    {
        private static readonly string[] NormalScenario =
        {
            "プロローグ:",
            "Global プロローグ実行",
            "Set プロローグ実行 1",
            "Exit",
            "",
            "スタート:",
            "Global スタート実行",
            "Set スタート実行 1",
            "Exit",
        };

        // ── ConvertUnitID ──

        [TestMethod]
        [DataRow("ガンダム12", "ガンダム:12")]
        [DataRow("ガンダム", "ガンダム:")]
        [DataRow("ガンダム2号機3", "ガンダム2号機:3")]
        [DataRow("123", ":123")]
        public void ConvertUnitID_InsertsColonBeforeTrailingDigits(string id, string expected)
        {
            // VB: 末尾から数字（48～57）を読み飛ばし、数字でない文字で Exit Do する
            var src = new SRC();

            src.ConvertUnitID(ref id);

            Assert.AreEqual(expected, id);
        }

        // ── IncludeData ──

        [TestMethod]
        public void IncludeData_AdvancesLoadingProgressTwicePerTitle()
        {
            // VB: IncludeData は作品ごとに DisplayLoadingProgress を 2 回呼ぶ（ロード画面の長さは作品数 * 2 + 5）
            using var f = new SrcPlayDataSaveFixture();
            f.Write("Data/作品/alias.txt", "作品エリアス", "バリア=バリア 1000", "");

            f.Src.IncludeData("作品");

            Assert.AreEqual(2, f.Gui.CallsOf(nameof(IGUI.DisplayLoadingProgress)).Count());
        }

        // ── StartScenario ──

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_EmptyName_TerminatesWithoutMessage()
        {
            // VB: If Len(fname) = 0 Then TerminateSRC: End（メッセージは出さない）
            using var f = new SrcPlayDataSaveFixture();

            SrcPlayDataSaveFixture.Run(() => f.Src.StartScenario(""));

            Assert.IsTrue(f.Gui.IsTerminated);
            Assert.IsFalse(f.Called(nameof(IGUI.ErrorMessage)));
        }

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_MapClearedTo1x1_ResizesTo15x15BeforePrologue()
        {
            // VB: If MapWidth = 1 Then SetMapSize 15, 15（ClearMap の後のステージ）
            using var f = new SrcPlayDataSaveFixture();
            f.Write("test.eve", NormalScenario);
            f.Src.Map.ClearMap();
            Assert.AreEqual(1, f.Src.Map.MapWidth);

            f.Src.StartScenario("test.eve");

            Assert.AreEqual(15, f.Src.Map.MapWidth);
            Assert.AreEqual(15, f.Src.Map.MapHeight);
        }

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_MapNot1x1_KeepsSize()
        {
            using var f = new SrcPlayDataSaveFixture();
            f.Write("test.eve", NormalScenario);
            f.Src.Map.SetMapSize(3, 4);

            f.Src.StartScenario("test.eve");

            Assert.AreEqual(3, f.Src.Map.MapWidth);
            Assert.AreEqual(4, f.Src.Map.MapHeight);
        }

        [TestMethod]
        [Timeout(20000)]
        [DataRow("Date", "シナリオ側のDataフォルダ名がDateになっています。", "フォルダ名をDataに直してください。")]
        [DataRow("Ｄａｔａ", "シナリオ側のDataフォルダ名が全角文字になっています。", "フォルダ名を半角文字に直してください。")]
        [DataRow("Ｂｉｔｍａｐ", "シナリオ側のBitmapフォルダ名が全角文字になっています。", "フォルダ名を半角文字に直してください。")]
        [DataRow("Ｌｉｂ", "シナリオ側のLibフォルダ名が全角文字になっています。", "フォルダ名を半角文字に直してください。")]
        [DataRow("Ｍｉｄｉ", "シナリオ側のMidiフォルダ名が全角文字になっています。", "フォルダ名を半角文字に直してください。")]
        [DataRow("Ｓｏｕｎｄ", "シナリオ側のSoundフォルダ名が全角文字になっています。", "フォルダ名を半角文字に直してください。")]
        public void StartScenario_WrongFolderName_ShowsErrorAndTerminates(string folder, string message, string howToFix)
        {
            // VB: シナリオ側のフォルダ名の誤り（Date・全角）を確かめて TerminateSRC する
            using var f = new SrcPlayDataSaveFixture();
            f.Write("test.eve", NormalScenario);
            Directory.CreateDirectory(Path.Combine(f.ScenarioDir, folder));

            SrcPlayDataSaveFixture.Run(() => f.Src.StartScenario("test.eve"));

            var error = (string)f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Single().Args[0];
            Assert.IsTrue(error.StartsWith(message + "\r\n"), error);
            Assert.IsTrue(error.Contains(folder), error);
            Assert.IsTrue(error.EndsWith("\r\n" + howToFix), error);
            Assert.IsTrue(f.Gui.IsTerminated);
            Assert.IsFalse(f.Src.Expression.IsGlobalVariableDefined("プロローグ実行"));
        }

        [TestMethod]
        [Timeout(20000)]
        public void StartScenario_SubStage_DoesNotCheckFolderNames()
        {
            // VB: フォルダ名の確認は If Not IsSubStage の中
            using var f = new SrcPlayDataSaveFixture();
            f.Write("test.eve", NormalScenario);
            Directory.CreateDirectory(Path.Combine(f.ScenarioDir, "Date"));
            f.Src.IsSubStage = true;

            f.Src.StartScenario("test.eve");

            Assert.IsFalse(f.Called(nameof(IGUI.ErrorMessage)));
            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("スタート実行"));
        }

        // ── StartTurn ──

        private static SrcPlayDataSaveFixture CreateWithMap()
        {
            var f = new SrcPlayDataSaveFixture();
            var map = f.Src.Map;
            map.SetMapSize(3, 3);
            map.MapFileName = Path.Combine(f.ScenarioDir, "test.map");
            var plain = new TerrainData { ID = 0, Name = "平地", Class = "陸", MoveCost = 2 };
            foreach (var x in map.MapData.XRange)
            {
                foreach (var y in map.MapData.YRange)
                {
                    map.MapData[x, y].UnderTerrain = plain;
                }
            }
            return f;
        }

        private static Unit AddUnit(SRC src, string name, string party)
        {
            var ud = src.UDList.Add(name);
            ud.HP = 1000;
            ud.EN = 100;
            ud.Speed = 5;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            src.PDList.Add(name + "パイロット");
            var unit = src.UList.Add(name, 0, party);
            var pilot = src.PList.Add(name + "パイロット", 1, party);
            unit.AddPilot(pilot);
            pilot.Unit = unit;
            unit.Update();
            unit.FullRecover();
            return unit;
        }

        private static void LoadEvent(SrcPlayDataSaveFixture f, params string[] lines)
        {
            var path = f.Write("test.eve", lines);
            f.Src.Event.LoadEventData(path, "");
        }

        [TestMethod]
        [Timeout(20000)]
        [DataRow("味方")]
        [DataRow("敵")]
        public void StartTurn_TurnEventHasNoSelectedUnit(string party)
        {
            // VB: 状態回復は For Each SelectedUnit In UList で回すので、回り終えると SelectedUnit は Nothing。
            // ターンイベントの対象ユニットは無い
            using var f = CreateWithMap();
            AddUnit(f.Src, "テスト機", "味方");
            LoadEvent(f,
                "ターン 1 " + party + ":",
                "Global 実行 対象",
                "Set 実行 1",
                "Set 対象 対象ユニットＩＤ",
                "Exit");
            // 味方フェイズはターン数を進めてからターンイベントを行う
            f.Src.Turn = party == "味方" ? 0 : 1;

            f.Src.StartTurn(party);

            Assert.AreEqual(1d, f.Src.Expression.GetValueAsDouble("実行"));
            Assert.AreEqual("", f.Src.Expression.GetValueAsString("対象"));
        }

        // ── LoadData ──

        [TestMethod]
        public void LoadData_NextStageOfSaveDataOverridesGlobalVariable()
        {
            // VB: LoadGlobalVariables の後で SetVariableAsString "次ステージ", fname2 する
            using var f = new SrcPlayDataSaveFixture();
            f.Src.Titles = new System.Collections.Generic.List<string>();
            f.Src.Expression.DefineGlobalVariable("次ステージ");
            f.Src.Expression.SetVariableAsString("次ステージ", "global.eve");
            var stream = new MemoryStream();
            f.Src.SaveData(stream);
            var json = JObject.Parse(Encoding.UTF8.GetString(stream.ToArray()));
            json["NextStage"] = "saved.eve";

            f.NewSession();
            f.Src.LoadData(new MemoryStream(Encoding.UTF8.GetBytes(json.ToString())));

            Assert.AreEqual("saved.eve", f.Src.Expression.GetValueAsString("次ステージ"));
        }

        // ── RestoreData ──

        private static string PrepareScenario(SrcPlayDataSaveFixture f)
        {
            var path = f.Write("test.eve", "準備:", "Exit", "", "再開:", "Exit");
            f.Src.ScenarioFileName = path;
            f.Src.Event.LoadEventData(path, "");
            f.Src.Map.SetMapSize(3, 3);
            f.Src.Map.MapFileName = Path.Combine(f.ScenarioDir, "test.map");
            f.Src.Turn = 3;
            return path;
        }

        private static MemoryStream Dump(SrcPlayDataSaveFixture f, SRCSaveKind kind)
        {
            var stream = new MemoryStream();
            f.Src.DumpData(stream, kind);
            return new MemoryStream(stream.ToArray());
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_VariablesDefinedByLoadEventDataAreReplacedBySavedVariables()
        {
            // VB: LoadEventData ScenarioFileName, "リストア" の後で RestoreEventData（LoadGlobalVariables）する。
            // DebugMode で LoadEventData が定義した Option(デバッグ) は、セーブデータに無ければ消える
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            var stream = Dump(f, SRCSaveKind.Suspend);
            f.Config.SetFlag("Option", "DebugMode", true);

            f.NewSession();
            f.Src.RestoreData(stream, SRCSaveKind.Suspend);

            Assert.IsFalse(f.Src.Expression.IsOptionDefined("デバッグ"));
            Assert.IsFalse(f.Gui.IsTerminated);
        }

        [TestMethod]
        [Timeout(20000)]
        public void RestoreData_BgmFileMissing_KeepsCurrentBgmModes()
        {
            // VB: ＢＧＭが見つからなければ StopBGM し、RepeatMode・KeepBGM・BossBGM は読み飛ばす
            using var f = new SrcPlayDataSaveFixture();
            PrepareScenario(f);
            var midi = f.Write("Midi/Map1.mid", "dummy");
            f.Src.Sound.StartBGM("Map1.mid", true);
            f.Src.Sound.KeepBGM = true;
            f.Src.Sound.BossBGM = true;
            var stream = Dump(f, SRCSaveKind.Suspend);
            File.Delete(midi);

            f.NewSession();
            f.Src.RestoreData(stream, SRCSaveKind.Suspend);

            Assert.AreEqual("", f.Src.Sound.BGMFileName ?? "");
            Assert.IsFalse(f.Src.Sound.RepeatMode);
            Assert.IsFalse(f.Src.Sound.KeepBGM);
            Assert.IsFalse(f.Src.Sound.BossBGM);
        }

        // ── Main（SRC.Execute）──

        private sealed class AppFixture : IDisposable
        {
            public string AppPath { get; }
            public SRC Src { get; }
            public HeadlessGUI Gui { get; }

            public AppFixture()
            {
                AppPath = Path.Combine(Path.GetTempPath(), "srcs-srcbas-" + Guid.NewGuid().ToString("N"));
                // SRC.ValidateEnvironment / LoadIni が要求する最小構成
                Directory.CreateDirectory(Path.Combine(AppPath, "Bitmap", "Event"));
                Directory.CreateDirectory(Path.Combine(AppPath, "Bitmap", "Map"));
                File.WriteAllBytes(Path.Combine(AppPath, "Bitmap", "Map", "plain0000.bmp"), new byte[0]);
                Directory.CreateDirectory(Path.Combine(AppPath, "Sound"));
                File.WriteAllBytes(Path.Combine(AppPath, "Sound", "dummy.wav"), new byte[0]);
                Directory.CreateDirectory(Path.Combine(AppPath, "Data", "System"));
                File.WriteAllText(Path.Combine(AppPath, "Data", "System", "terrain.txt"),
                    "0\r\n平地, plain\r\n陸, 1, 0, 0\r\n\r\n", new UTF8Encoding(false));

                Src = new SRC
                {
                    SystemConfig = new LocalFileConfig { AppPath = AppPath },
                };
                var fs = new LocalFileSystem();
                fs.AddPath(AppPath);
                Src.FileSystem = fs;
                Gui = new HeadlessGUI();
                Gui.Install(Src);
            }

            public Exception Execute(string fname)
            {
                Exception error = null;
                var game = new Thread(() =>
                {
                    try
                    {
                        Src.Execute(fname);
                    }
                    catch (Exception ex)
                    {
                        error = ex;
                    }
                })
                {
                    IsBackground = true,
                };
                game.Start();
                Assert.IsTrue(game.Join(TimeSpan.FromSeconds(20)), "scenario timeout");
                return error;
            }

            public void Dispose()
            {
                try
                {
                    Directory.Delete(AppPath, true);
                }
                catch
                {
                    // ignore
                }
            }
        }

        [TestMethod]
        [Timeout(30000)]
        public void Execute_ClosesTitleFormOnce()
        {
            // VB: Main はオープニング曲の演奏・InitEventData・CloseTitleForm を 1 回だけ行う
            using var f = new AppFixture();
            File.WriteAllText(Path.Combine(f.AppPath, "test.eve"), "プロローグ:\r\nQuit\r\n", new UTF8Encoding(false));

            var error = f.Execute(Path.Combine(f.AppPath, "test.eve"));

            Assert.IsInstanceOfType(error, typeof(HeadlessTerminatedException), error?.ToString());
            Assert.AreEqual(1, f.Gui.CallsOf(nameof(IGUI.CloseTitleForm)).Count());
        }

        [TestMethod]
        [Timeout(30000)]
        public void Execute_FileNotFound_ShowsErrorAndTerminates()
        {
            // VB: If Not FileExists(fname) Then ErrorMessage "指定したファイルが存在しません": TerminateSRC
            using var f = new AppFixture();

            var error = f.Execute(Path.Combine(f.AppPath, "無い.srcs"));

            Assert.IsInstanceOfType(error, typeof(HeadlessTerminatedException), error?.ToString());
            Assert.AreEqual("指定したファイルが存在しません",
                f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Single().Args[0]);
        }

        [TestMethod]
        [Timeout(30000)]
        [DataRow("Ｂｉｔｍａｐ", "Bitmapフォルダのフォルダ名が全角文字になっています。")]
        [DataRow("Ｓｏｕｎｄ", "Soundフォルダのフォルダ名が全角文字になっています。")]
        public void Execute_FullWidthAppFolder_ShowsErrorAndTerminates(string folder, string message)
        {
            // VB: SRC.exe のフォルダの Bitmap・Sound のフォルダ名が全角文字ならエラーにして終了する
            using var f = new AppFixture();
            Directory.CreateDirectory(Path.Combine(f.AppPath, folder));
            File.WriteAllText(Path.Combine(f.AppPath, "test.eve"), "プロローグ:\r\nQuit\r\n", new UTF8Encoding(false));

            var error = f.Execute(Path.Combine(f.AppPath, "test.eve"));

            Assert.IsInstanceOfType(error, typeof(HeadlessTerminatedException), error?.ToString());
            var shown = (string)f.Gui.CallsOf(nameof(IGUI.ErrorMessage)).Single().Args[0];
            Assert.IsTrue(shown.StartsWith(message), shown);
        }
    }
}
