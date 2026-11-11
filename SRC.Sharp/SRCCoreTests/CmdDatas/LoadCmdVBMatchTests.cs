using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.CmdDatas;
using SRCCore.Config;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.CmdDatas.Tests
{
    /// <summary>
    /// Load コマンドのデータの読み込みと構文解析を、元の SRC（VB5。CmdData.cls ExecLoadCmd）に合わせる（#1518）。
    /// 期待値は元の SRC のコード（ExecLoadCmd・Event.bas LoadEventData2・AddLabel）の動作。
    /// </summary>
    [TestClass]
    public class LoadCmdVBMatchTests
    {
        private string appPath;
        private RecordingFileSystem fs;

        [TestInitialize]
        public void Initialize()
        {
            appPath = Path.Combine(Path.GetTempPath(), "srcs-load-" + Guid.NewGuid());
            Directory.CreateDirectory(appPath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            try
            {
                Directory.Delete(appPath, true);
            }
            catch
            {
                // ignore
            }
        }

        private void WriteFile(string name, params string[] lines)
        {
            var path = Path.Combine(appPath, name);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, string.Join("\r\n", lines) + "\r\n", new UTF8Encoding(false));
        }

        private (SRC src, HeadlessGUI gui) CreateSrc()
        {
            var src = new SRC
            {
                SystemConfig = new LocalFileConfig { AppPath = appPath },
            };
            var local = new LocalFileSystem();
            local.AddPath(appPath);
            fs = new RecordingFileSystem(local);
            src.FileSystem = fs;
            var gui = new HeadlessGUI();
            gui.Install(src);
            src.ScenarioPath = appPath;
            src.Event.SkipExternalSourceLoad = true;
            src.Event.InitEventData();
            src.Event.LoadEventData(Path.Combine(appPath, "test.eve"));
            src.Expression.DefineGlobalVariable("記録");
            src.Expression.SetVariableAsString("記録", "");
            fs.Opened.Clear();
            return (src, gui);
        }

        private static readonly string[] PilotData =
        {
            "月蟲長",
            "月蟲長, つきむしちょう, -, 錆蟲, AAAA, 40",
            "特殊能力なし",
            "120, 125, 130, 120, 120, 120, 機械",
            "ＳＰなし",
            "cp_chrome.bmp, -.mid",
        };

        private static readonly string[] UnitData =
        {
            "月面錆蟲",
            "月面錆蟲, げつめんさびむし, 錆蟲, 1, 0",
            "宇宙, 5, M, 1600, 60",
            "特殊能力なし",
            "2200, 80, 700, 90",
            "AAAA, cp_u_moon.bmp",
            "錆の顎, 1100, 1, 1, +10, -, -, -, AAAA, +0, 突",
        };

        private int IndexOfOpened(string relativePath)
        {
            var target = Path.GetFullPath(Path.Combine(appPath, relativePath.Replace('\\', Path.DirectorySeparatorChar)));
            return fs.Opened.FindIndex(x => Path.GetFullPath(x.Replace('\\', Path.DirectorySeparatorChar)) == target);
        }

        [TestMethod]
        public void Load_ReadsIncludeDataAndIncludeEveAlternatelyPerTitle()
        {
            // VB: For i = 1 To UBound(new_titles) で、タイトルごとに IncludeData と include.eve の読み込みを交互に行う
            WriteFile("test.eve",
                "スタート:",
                "Load タイトルA タイトルB",
                "Exit");
            WriteFile(Path.Combine("Data", "タイトルA", "pilot.txt"), PilotData);
            WriteFile(Path.Combine("Data", "タイトルA", "include.eve"), "追加A:", "Return");
            WriteFile(Path.Combine("Data", "タイトルB", "unit.txt"), UnitData);
            WriteFile(Path.Combine("Data", "タイトルB", "include.eve"), "追加B:", "Return");
            var (src, _) = CreateSrc();

            src.Event.HandleEvent("スタート");

            var aData = IndexOfOpened(@"Data\タイトルA\pilot.txt");
            var aEve = IndexOfOpened(@"Data\タイトルA\include.eve");
            var bData = IndexOfOpened(@"Data\タイトルB\unit.txt");
            var bEve = IndexOfOpened(@"Data\タイトルB\include.eve");
            var opened = string.Join(", ", fs.Opened);
            Assert.IsTrue(aData >= 0 && aEve >= 0 && bData >= 0 && bEve >= 0, opened);
            Assert.IsTrue(aData < aEve, opened);
            Assert.IsTrue(aEve < bData, opened);
            Assert.IsTrue(bData < bEve, opened);
            Assert.IsTrue(src.UDList.IsDefined("月面錆蟲"));
            Assert.IsTrue(src.PDList.IsDefined("月蟲長"));
        }

        [TestMethod]
        public void Load_RereadsOnlyTheLocalDataThatVBRereads()
        {
            // VB が読み直すのは alias・sp（無ければ mind）・pilot・non_pilot・robot・unit・pilot_message・pilot_dialog・item だけ。
            // effect・animation・ext_animation は読み直さず、ADList.AddDefaultAnimation も呼ばない
            WriteFile("test.eve",
                "スタート:",
                "Load タイトルA",
                "Exit");
            WriteFile(Path.Combine("Data", "タイトルA", "include.eve"), "追加A:", "Return");
            WriteFile(Path.Combine("Data", "pilot.txt"), PilotData);
            WriteFile(Path.Combine("Data", "effect.txt"), "");
            WriteFile(Path.Combine("Data", "animation.txt"), "");
            WriteFile(Path.Combine("Data", "ext_animation.txt"), "");
            var (src, _) = CreateSrc();

            src.Event.HandleEvent("スタート");

            var opened = string.Join(", ", fs.Opened);
            Assert.IsTrue(IndexOfOpened(@"Data\pilot.txt") >= 0, opened);
            Assert.IsTrue(src.PDList.IsDefined("月蟲長"));
            Assert.AreEqual(-1, IndexOfOpened(@"Data\effect.txt"), opened);
            Assert.AreEqual(-1, IndexOfOpened(@"Data\animation.txt"), opened);
            Assert.AreEqual(-1, IndexOfOpened(@"Data\ext_animation.txt"), opened);
            Assert.IsFalse(src.ADList.IsDefined("汎用"));
        }

        [TestMethod]
        public void Load_ParsesOnlyNewLines_AndKeepsRunningCommandObjects()
        {
            // VB: 新しい行（cur_data_size + 1 以降）だけを対象にラベルを登録し、Parse する。
            // 前からある行の EventCmd はそのままなので、Load の後も呼び出し元のイベントの続きが動く
            WriteFile("test.eve",
                "スタート:",
                "Set 記録 (記録 & \"S\")",
                "For i = 1 To 2",
                "If i = 2 Then",
                "Load タイトルA",
                "EndIf",
                "Set 記録 (記録 & i)",
                "Next",
                "Call 追加A",
                "Set 記録 (記録 & \"E\")",
                "Exit");
            WriteFile(Path.Combine("Data", "タイトルA", "include.eve"),
                "追加A:",
                "Set 記録 (記録 & \"A\")",
                "Return",
                "ターン 1 味方:",
                "Exit");
            var (src, gui) = CreateSrc();
            var before = src.Event.EventCmd.ToList();

            src.Event.HandleEvent("スタート");

            Assert.AreEqual("S12AE", src.Expression.GetValueAsString("記録"));
            Assert.IsFalse(gui.IsTerminated);
            for (var i = 0; i < before.Count; i++)
            {
                Assert.AreSame(before[i], src.Event.EventCmd[i], "行 " + i + " の EventCmd が作り直されている");
            }
            Assert.AreEqual(src.Event.EventData.Count, src.Event.EventCmd.Count);
            var labelLine = src.Event.EventData.First(x => x.Data == "追加A:").ID;
            Assert.IsTrue(labelLine >= before.Count);
            Assert.AreEqual(labelLine, src.Event.FindLabel("追加A"));
            Assert.AreEqual(1, src.Event.colEventLabelList.List.Count(x => x.Data == "ターン 1 味方"));
        }

        [TestMethod]
        public void Load_DoesNotCheckControlStructureOfNewLines()
        {
            // VB の ExecLoadCmd は制御構造の書式チェック（LoadEventData の「Ifに対応するEndIfがありません」など）をしない
            WriteFile("test.eve",
                "スタート:",
                "Load タイトルA",
                "Set 記録 \"E\"",
                "Exit");
            WriteFile(Path.Combine("Data", "タイトルA", "include.eve"),
                "追加A:",
                "If 記録 = \"E\" Then",
                "Return");
            var (src, gui) = CreateSrc();

            src.Event.HandleEvent("スタート");

            CollectionAssert.AreEqual(new string[0], src.DataErrors.Select(x => x.msg).ToArray());
            Assert.IsFalse(gui.IsTerminated);
            Assert.AreEqual("E", src.Expression.GetValueAsString("記録"));
        }

        [TestMethod]
        public void Load_ReadsIncludeEveEvenIfAlreadyInEventFileNames()
        {
            // VB の ExecLoadCmd は EventFileNames を見ずに include.eve を読む（読み込み済みの判定をしない）
            WriteFile("test.eve",
                "スタート:",
                "Load タイトルA",
                "Exit");
            WriteFile(Path.Combine("Data", "タイトルA", "include.eve"), "追加A:", "Return");
            var (src, _) = CreateSrc();
            var includeEve = src.FileSystem.PathCombine(src.SearchDataFolder("タイトルA"), "include.eve");
            src.Event.EventFileNames.Add(includeEve);

            src.Event.HandleEvent("スタート");

            Assert.AreEqual(1, src.Event.EventData.Count(x => x.Data == "追加A:"));
            Assert.IsTrue(src.Event.FindLabel("追加A") >= 0);
        }

        /// <summary>
        /// 開いたファイルの順を記録するファイルシステム。
        /// </summary>
        private sealed class RecordingFileSystem : IFileSystem
        {
            private readonly IFileSystem inner;

            public RecordingFileSystem(IFileSystem inner)
            {
                this.inner = inner;
            }

            public List<string> Opened { get; } = new List<string>();

            public event Action<string> FileWritten
            {
                add { inner.FileWritten += value; }
                remove { inner.FileWritten -= value; }
            }

            public string PathCombine(params string[] paths) => inner.PathCombine(paths);
            public bool PathEquals(string a, string b) => inner.PathEquals(a, b);
            public bool FileExists(params string[] paths) => inner.FileExists(paths);

            public Stream Open(params string[] paths)
            {
                Opened.Add(inner.PathCombine(paths));
                return inner.Open(paths);
            }

            public void AddPath(string basePath) => inner.AddPath(basePath);
            public void AddAchive(string basePath, string archivePath) => inner.AddAchive(basePath, archivePath);
            public IEnumerable<string> GetFileSystemEntries(string path, string searchPattern, EntryOption enumerationOptions) => inner.GetFileSystemEntries(path, searchPattern, enumerationOptions);
            public void AddSafePath(string basePath) => inner.AddSafePath(basePath);
            public Stream OpenSafe(SafeOpenMode mode, params string[] paths) => inner.OpenSafe(mode, paths);
            public Stream Create(string path) => inner.Create(path);
            public bool Remove(params string[] paths) => inner.Remove(paths);
            public bool MkDir(params string[] paths) => inner.MkDir(paths);
            public bool RmDir(params string[] paths) => inner.RmDir(paths);
            public bool RelativePathEuqals(string scenarioPath, string a, string b) => inner.RelativePathEuqals(scenarioPath, a, b);
            public bool IsAbsolutePath(string path) => inner.IsAbsolutePath(path);
            public string ToAbsolutePath(string scenarioPath, string path) => inner.ToAbsolutePath(scenarioPath, path);
            public string ToRelativePath(string scenarioPath, string path) => inner.ToRelativePath(scenarioPath, path);
            public string NormalizePath(string path) => inner.NormalizePath(path);
        }
    }
}
