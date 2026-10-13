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
    }
}
