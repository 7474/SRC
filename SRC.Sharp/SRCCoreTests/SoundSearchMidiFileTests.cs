using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.IO;
using System.Threading.Tasks;

namespace SRCCore.Tests
{
    /// <summary>
    /// Sound.SearchMidiFile のテスト
    /// </summary>
    [TestClass]
    public class SoundSearchMidiFileTests
    {
        private string dir;

        [TestInitialize]
        public void Initialize()
        {
            dir = Path.Combine(Path.GetTempPath(), "srcmidi-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(dir, "Midi"));
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(dir, true);
        }

        private SRC CreateSrc()
        {
            var fs = new LocalFileSystem();
            fs.AddPath(dir);
            return new SRC { GUI = new MockGUI(), FileSystem = fs };
        }

        private static string SearchWithTimeout(SRC src, string name)
        {
            // 無限ループの回帰を検出するため時間を区切る
            var task = Task.Run(() => src.Sound.SearchMidiFile(name));
            Assert.IsTrue(task.Wait(TimeSpan.FromSeconds(5)), "SearchMidiFile did not finish");
            return task.Result;
        }

        [TestMethod]
        public void Mp3NameNotFound_ReturnsNull()
        {
            // .mid で終わらない指定（.mp3）で見つからない場合に無限ループしていた
            Assert.IsNull(SearchWithTimeout(CreateSrc(), "(kt_guardian.mp3)"));
        }

        [TestMethod]
        public void Mp3Name_FindsFileInMidiFolder()
        {
            File.WriteAllBytes(Path.Combine(dir, "Midi", "kt_guardian.mp3"), new byte[0]);
            StringAssert.EndsWith(SearchWithTimeout(CreateSrc(), "(kt_guardian.mp3)"), "kt_guardian.mp3");
        }

        [TestMethod]
        public void ListOfNames_FallsBackToNext()
        {
            File.WriteAllBytes(Path.Combine(dir, "Midi", "b.mid"), new byte[0]);
            StringAssert.EndsWith(SearchWithTimeout(CreateSrc(), "a.mid b.mid"), "b.mid");
            Assert.IsNull(SearchWithTimeout(CreateSrc(), "a.mid c.mid"));
        }

        /// <summary>シナリオ・拡張データフォルダ 1・2・SRC のフォルダを分けた SRC を作る（元の SRC の Sound.bas の SearchMidiFile の確かめ用）</summary>
        private SRC CreateSrcWithFolders(out string scenario, out string ext, out string ext2, out string app)
        {
            scenario = Path.Combine(dir, "scenario");
            ext = Path.Combine(dir, "ext");
            ext2 = Path.Combine(dir, "ext2");
            app = Path.Combine(dir, "app");
            var fs = new LocalFileSystem();
            foreach (var d in new[] { app, ext2, ext, scenario })
            {
                Directory.CreateDirectory(Path.Combine(d, "Midi"));
                fs.AddPath(d);
            }
            var src = new SRC { GUI = new MockGUI(), FileSystem = fs };
            src.SystemConfig.AppPath = app;
            src.SystemConfig.ExtDataPath = ext;
            src.SystemConfig.ExtDataPath2 = ext2;
            src.ScenarioPath = scenario;
            return src;
        }

        private static void Touch(params string[] paths)
        {
            var path = Path.Combine(paths);
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllBytes(path, new byte[0]);
            // 作ったファイルを次の検索で見つけられるよう、フォルダの一覧のキャッシュを捨てる
            PathResolver.Invalidate();
        }

        private static string Normalize(string path) => Path.GetFullPath(path.Replace('\\', '/'));

        [TestMethod]
        public void UnderscoreName_FindsFileInSubFolder()
        {
            // 元の SRC: 「作品名_曲名.mid」は Midi\作品名\ からも探す（7474/SRC#1480）
            var src = CreateSrcWithFolders(out var scenario, out _, out _, out _);
            Touch(scenario, "Midi", "work", "work_battle.mid");
            Assert.AreEqual(Normalize(Path.Combine(scenario, "Midi", "work", "work_battle.mid")), Normalize(SearchWithTimeout(src, "work_battle.mid")));
        }

        [TestMethod]
        public void SubFolder_IsSearchedBeforeMidiFolder_AndMp3BeforeMidi()
        {
            // 同じフォルダでは サブフォルダの MP3 → 直下の MP3 → サブフォルダの MIDI → 直下の MIDI の順
            var src = CreateSrcWithFolders(out var scenario, out _, out _, out _);
            Touch(scenario, "Midi", "work_battle.mid");
            Touch(scenario, "Midi", "work", "work_battle.mid");
            Assert.AreEqual(Normalize(Path.Combine(scenario, "Midi", "work", "work_battle.mid")), Normalize(SearchWithTimeout(src, "work_battle.mid")));
            Touch(scenario, "Midi", "work_battle.mp3");
            Assert.AreEqual(Normalize(Path.Combine(scenario, "Midi", "work_battle.mp3")), Normalize(SearchWithTimeout(src, "work_battle.mid")));
            Touch(scenario, "Midi", "work", "work_battle.mp3");
            Assert.AreEqual(Normalize(Path.Combine(scenario, "Midi", "work", "work_battle.mp3")), Normalize(SearchWithTimeout(src, "work_battle.mid")));
        }

        [TestMethod]
        public void ScenarioMidi_IsUsedBeforeMp3InOtherFolders()
        {
            // 元の SRC: フォルダごとに MP3・MIDI を探すため、シナリオの MIDI は SRC のフォルダの同じ名前の MP3 より先に使う（7474/SRC#1480）
            var src = CreateSrcWithFolders(out var scenario, out var ext, out var ext2, out var app);
            Touch(app, "Midi", "battle.mp3");
            Touch(ext2, "Midi", "battle.mp3");
            Touch(ext, "Midi", "battle.mp3");
            Touch(scenario, "Midi", "battle.mid");
            Assert.AreEqual(Normalize(Path.Combine(scenario, "Midi", "battle.mid")), Normalize(SearchWithTimeout(src, "battle.mid")));
        }

        [TestMethod]
        public void Folders_AreSearchedInOrder_ScenarioExtExt2App()
        {
            var src = CreateSrcWithFolders(out var scenario, out var ext, out var ext2, out var app);
            Touch(app, "Midi", "battle.mid");
            Assert.AreEqual(Normalize(Path.Combine(app, "Midi", "battle.mid")), Normalize(SearchWithTimeout(src, "battle.mid")));
            Touch(ext2, "Midi", "battle.mid");
            Assert.AreEqual(Normalize(Path.Combine(ext2, "Midi", "battle.mid")), Normalize(SearchWithTimeout(src, "battle.mid")));
            Touch(ext, "Midi", "battle.mid");
            Assert.AreEqual(Normalize(Path.Combine(ext, "Midi", "battle.mid")), Normalize(SearchWithTimeout(src, "battle.mid")));
            Touch(scenario, "Midi", "battle.mid");
            Assert.AreEqual(Normalize(Path.Combine(scenario, "Midi", "battle.mid")), Normalize(SearchWithTimeout(src, "battle.mid")));
        }
    }
}
