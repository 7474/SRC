using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore.Filesystem;
using SRCCore.TestLib;
using System;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Tests
{
    /// <summary>
    /// 元の SRC の Sound.bas と分岐ごとに突き合わせて見つかった違い（7474/SRC#1498）。
    /// 期待値は元の SRC（SRC/SRC_20121125/Sound.bas）の動作から取る。
    /// SRC# のパスの区切りは / のこともあるので、元の SRC が \ で区切ったときと同じ結果になるかを見る。
    /// </summary>
    [TestClass]
    public class SoundBasVbMatchTests
    {
        private sealed class RecordingPlayer : IPlaySound
        {
            public List<string> Calls { get; } = new List<string>();
            public BGMStatus BGMStatus { get; set; } = BGMStatus.Playing;
            public float SoundVolume { get; set; }
            public void Play(int channel, Stream stream, SoundType soundType, PlaySoundMode mode) => Calls.Add("Play");
            public void Stop(int channel) => Calls.Add("Stop");
            public SoundType ResolveSoundType(string path) => SoundType.Midi;
            public void Dispose() { }
        }

        private string dir;

        [TestInitialize]
        public void Initialize()
        {
            dir = Path.Combine(Path.GetTempPath(), "srcsoundvb-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(dir, "Midi"));
            File.WriteAllBytes(Path.Combine(dir, "Midi", "Map1.mid"), new byte[0]);
            File.WriteAllBytes(Path.Combine(dir, "Midi", "b.mid"), new byte[0]);
            PathResolver.Invalidate();
        }

        [TestCleanup]
        public void Cleanup()
        {
            Directory.Delete(dir, true);
        }

        private SRC CreateSrc(out RecordingPlayer player)
        {
            var fs = new LocalFileSystem();
            fs.AddPath(dir);
            var src = new SRC { GUI = new MockGUI(), FileSystem = fs };
            src.SystemConfig.AppPath = dir;
            player = new RecordingPlayer();
            src.Sound.Player = player;
            return src;
        }

        // ──────────────────────────────────────────────
        // StartBGM
        // ──────────────────────────────────────────────

        [TestMethod]
        public void StartBGM_SameBgmWithSlashPath_KeepsPlaying()
        {
            // Sound.bas:91 InStr(BGMFileName, "\" & fname0 & ".") > 0 で演奏中なら続ける。
            // SRC# のパスの区切りが / でも同じ曲とみなす
            var src = CreateSrc(out var player);
            src.Sound.BGMFileName = dir + "/Midi/Map1.mid";
            src.Sound.StartBGM("Map1.mid");
            CollectionAssert.AreEqual(new string[0], player.Calls);
            Assert.AreEqual(dir + "/Midi/Map1.mid", src.Sound.BGMFileName);
        }

        [TestMethod]
        public void StartBGM_BgmNameWithBackslashFolder_ComparesFileNameOnly()
        {
            // Sound.bas:85 ファイル名の本体部分は最後の \ より後ろ
            Directory.CreateDirectory(Path.Combine(dir, "Midi", "sub"));
            File.WriteAllBytes(Path.Combine(dir, "Midi", "sub", "Map1.mid"), new byte[0]);
            PathResolver.Invalidate();
            var src = CreateSrc(out var player);
            Assert.IsNotNull(src.Sound.SearchMidiFile(@"(sub\Map1.mid)"));
            src.Sound.BGMFileName = dir + "/Midi/Map1.mid";
            src.Sound.StartBGM(@"sub\Map1.mid");
            CollectionAssert.AreEqual(new string[0], player.Calls);
        }

        [TestMethod]
        public void StartBGM_RelativeBgmFileName_KeepsPlaying()
        {
            // 元の SRC の BGMFileName はいつもフォルダの付いたパス。SRC# は読み込み対象のフォルダの直下で見つけると
            // フォルダの無い名前になるので、それでも同じ曲とみなす
            var src = CreateSrc(out var player);
            src.Sound.BGMFileName = "Map1.mid";
            src.Sound.StartBGM("Map1.mid");
            CollectionAssert.AreEqual(new string[0], player.Calls);
        }

        [TestMethod]
        public void StartBGM_SameBgmButStopped_PlaysAgain()
        {
            // Sound.bas:92 演奏中でなければ（止まっていれば）演奏し直す
            var src = CreateSrc(out var player);
            player.BGMStatus = BGMStatus.Stopped;
            src.Sound.BGMFileName = dir + "/Midi/Map1.mid";
            src.Sound.StartBGM("Map1.mid");
            CollectionAssert.AreEqual(new[] { "Stop", "Play" }, player.Calls);
        }

        // ──────────────────────────────────────────────
        // ChangeBGM
        // ──────────────────────────────────────────────

        [TestMethod]
        public void ChangeBGM_SameBgmWithSlashPath_KeepsPlaying()
        {
            // Sound.bas:539 既に同じ曲を演奏していればそのまま
            var src = CreateSrc(out var player);
            player.BGMStatus = BGMStatus.Stopped;
            src.Sound.BGMFileName = dir + "/Midi/Map1.mid";
            src.Sound.ChangeBGM("Map1.mid");
            CollectionAssert.AreEqual(new string[0], player.Calls);
        }

        [TestMethod]
        public void ChangeBGM_NumberedVariantWithSlashPath_KeepsPlaying()
        {
            // Sound.bas:546 演奏中の曲が番号違い（(2)〜(9)）なら同じ曲とみなす
            var src = CreateSrc(out var player);
            src.Sound.BGMFileName = dir + "/Midi/Map1(2).mid";
            src.Sound.ChangeBGM("Map1.mid");
            CollectionAssert.AreEqual(new string[0], player.Calls);
        }

        [TestMethod]
        public void ChangeBGM_OtherBgm_StartsIt()
        {
            var src = CreateSrc(out var player);
            src.Sound.BGMFileName = dir + "/Midi/Map1.mid";
            src.Sound.ChangeBGM("b.mid");
            CollectionAssert.AreEqual(new[] { "Stop", "Play" }, player.Calls);
            StringAssert.EndsWith(src.Sound.BGMFileName, "b.mid");
        }

        // ──────────────────────────────────────────────
        // RestartBGM
        // ──────────────────────────────────────────────

        [TestMethod]
        public void RestartBGM_WithoutBgm_DoesNothing()
        {
            // Sound.bas:287 BGMStatus は演奏中のファイルが無ければ空文字列なので、"stopped" でなく何もしない
            var src = CreateSrc(out var player);
            player.BGMStatus = BGMStatus.Stopped;
            src.Sound.BGMFileName = "";
            src.Sound.RestartBGM();
            CollectionAssert.AreEqual(new string[0], player.Calls);
        }

        // ──────────────────────────────────────────────
        // SearchMidiFile
        // ──────────────────────────────────────────────

        [TestMethod]
        public void SearchMidiFile_AbsolutePathNotFound_DoesNotTryNextName()
        {
            // Sound.bas:833 フルパスで指定したファイルが無ければ、残りの候補は探さずに終える
            var src = CreateSrc(out _);
            Assert.IsNull(src.Sound.SearchMidiFile(Path.Combine(dir, "none.mid") + " b.mid"));
            StringAssert.EndsWith(src.Sound.SearchMidiFile("none.mid b.mid"), "b.mid");
        }

        [TestMethod]
        public void SearchMidiFile_UnbalancedList_FindsNothing()
        {
            // Sound.bas:802 ListLength が -1（括弧の対応が取れていない）なら探さない
            var src = CreateSrc(out _);
            Assert.IsNull(src.Sound.SearchMidiFile("b.mid )"));
            Assert.IsNull(src.Sound.SearchMidiFile("b.mid (x"));
        }
    }
}
