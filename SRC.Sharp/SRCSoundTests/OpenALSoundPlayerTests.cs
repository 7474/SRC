using Microsoft.VisualStudio.TestTools.UnitTesting;
using SRCCore;
using System;
using System.IO;
using System.Threading;

namespace SRCSound.Tests
{
    /// <summary>
    /// 音声出力の無い環境（CI）では OpenAL Soft の null 出力（環境変数 ALSOFT_DRIVERS=null）で動かす。
    /// </summary>
    [TestClass]
    public class OpenALSoundPlayerTests
    {
        private static OpenALSoundPlayer CreatePlayer(Func<MeltySynth.SoundFont> soundFont = null)
        {
            var player = new OpenALSoundPlayer(soundFont);
            if (!player.IsAvailable)
            {
                player.Dispose();
                Assert.Inconclusive("音声出力を開けません（ALSOFT_DRIVERS=null で null 出力を使える）");
            }
            return player;
        }

        private static void WaitUntil(Func<bool> condition, string description)
        {
            var deadline = DateTime.UtcNow.AddSeconds(5);
            while (!condition())
            {
                if (DateTime.UtcNow > deadline)
                {
                    Assert.Fail("timeout: " + description);
                }
                Thread.Sleep(10);
            }
        }

        [TestMethod]
        public void PlaysWaveOnceAndStops()
        {
            using (var player = CreatePlayer())
            {
                player.Play(PlaySoundConstants.CH_BGM, new MemoryStream(TestAudio.Wave(22050, 1, 2205)), SoundType.Wave, PlaySoundMode.None);
                Assert.AreEqual(BGMStatus.Playing, player.BGMStatus);
                // 0.1 秒の音は再生し終える
                WaitUntil(() => player.BGMStatus == BGMStatus.Stopped, "finished");
            }
        }

        [TestMethod]
        public void RepeatKeepsPlayingUntilStop()
        {
            using (var player = CreatePlayer())
            {
                player.Play(PlaySoundConstants.CH_BGM, new MemoryStream(TestAudio.Wave(22050, 2, 2205)), SoundType.Wave, PlaySoundMode.Repeat);
                Thread.Sleep(500);
                Assert.AreEqual(BGMStatus.Playing, player.BGMStatus);
                player.Stop(PlaySoundConstants.CH_BGM);
                Assert.AreEqual(BGMStatus.Stopped, player.BGMStatus);
            }
        }

        [TestMethod]
        public void PlaysMp3AndChangesVolume()
        {
            using (var player = CreatePlayer())
            using (var stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Assets", "sine440.mp3")))
            {
                var memory = new MemoryStream();
                stream.CopyTo(memory);
                memory.Position = 0;
                player.Play(2, memory, SoundType.Mp3, PlaySoundMode.None);
                player.SoundVolume = 0.3f;
                Assert.AreEqual(0.3f, player.SoundVolume);
                player.Stop(2);
            }
        }

        [TestMethod]
        public void MidiWithoutSoundFontIsSilent()
        {
            using (var player = CreatePlayer(() => null))
            {
                player.Play(PlaySoundConstants.CH_BGM, new MemoryStream(TestAudio.Midi()), SoundType.Midi, PlaySoundMode.Repeat);
                Assert.AreEqual(BGMStatus.Stopped, player.BGMStatus);
            }
        }

        [TestMethod]
        public void BrokenDataIsIgnored()
        {
            using (var player = CreatePlayer())
            {
                player.Play(PlaySoundConstants.CH_BGM, new MemoryStream(new byte[16]), SoundType.Wave, PlaySoundMode.None);
                Assert.AreEqual(BGMStatus.Stopped, player.BGMStatus);
            }
        }

        [TestMethod]
        public void ResolveSoundType()
        {
            using (var player = new OpenALSoundPlayer())
            {
                Assert.AreEqual(SoundType.Wave, player.ResolveSoundType(@"Sound\Hit.WAV"));
                Assert.AreEqual(SoundType.Mp3, player.ResolveSoundType("bgm.mp3"));
                Assert.AreEqual(SoundType.Midi, player.ResolveSoundType("bgm.mid"));
                Assert.AreEqual(SoundType.Aiff, player.ResolveSoundType("a.aiff"));
                Assert.AreEqual(SoundType.Unknown, player.ResolveSoundType("a.txt"));
            }
        }
    }
}
