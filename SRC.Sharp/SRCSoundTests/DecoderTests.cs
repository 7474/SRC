using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;

namespace SRCSound.Tests
{
    [TestClass]
    public class DecoderTests
    {
        private static float[] ReadAll(ISampleSource source)
        {
            var result = new System.Collections.Generic.List<float>();
            var buffer = new float[1000];
            int n;
            while ((n = source.Read(buffer, 0, buffer.Length)) > 0)
            {
                result.AddRange(buffer.Take(n));
            }
            return result.ToArray();
        }

        [DataTestMethod]
        [DataRow(1, 16)]
        [DataRow(2, 16)]
        [DataRow(1, 8)]
        public void WaveSource_ReadsPcm(int channels, int bits)
        {
            var source = new WaveSource(new MemoryStream(TestAudio.Wave(22050, channels, 2205, bits)));
            Assert.AreEqual(22050, source.SampleRate);
            Assert.AreEqual(channels, source.Channels);
            var samples = ReadAll(source);
            Assert.AreEqual(2205 * channels, samples.Length);
            // 振幅 0.5 の正弦波
            Assert.AreEqual(0.5, samples.Max(), 0.02);
            Assert.AreEqual(-0.5, samples.Min(), 0.02);

            // 先頭に戻すと同じものを読む
            source.Rewind();
            CollectionAssert.AreEqual(samples, ReadAll(source));
        }

        [TestMethod]
        public void WaveSource_RejectsNonWave()
        {
            Assert.ThrowsExactly<InvalidDataException>(() => new WaveSource(new MemoryStream(new byte[64])));
        }

        [TestMethod]
        public void Mp3Source_DecodesFile()
        {
            using (var stream = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "Assets", "sine440.mp3")))
            {
                var source = new Mp3Source(stream);
                Assert.AreEqual(22050, source.SampleRate);
                Assert.AreEqual(1, source.Channels);
                var samples = ReadAll(source);
                // 0.5 秒分（エンコーダの遅延分の誤差を許す）
                Assert.AreEqual(22050 / 2, samples.Length, 2000);
                // ffmpeg の sine は振幅 1/8
                Assert.AreEqual(0.125, samples.Max(), 0.02);
            }
        }

        [TestMethod]
        public void SoundFontLocator_PrefersConfiguredPath()
        {
            var dir = Path.Combine(Path.GetTempPath(), "srcsound-" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(Path.Combine(dir, "Midi"));
            try
            {
                var inMidi = Path.Combine(dir, "Midi", "a.sf2");
                File.WriteAllBytes(inMidi, new byte[0]);
                Assert.AreEqual(inMidi, SoundFontLocator.Find(dir));
                var configured = Path.Combine(dir, "configured.sf2");
                File.WriteAllBytes(configured, new byte[0]);
                Assert.AreEqual(configured, SoundFontLocator.Find(dir, configured));
                // 直下のものは Midi フォルダより先
                Assert.AreEqual(configured, SoundFontLocator.Find(dir));
            }
            finally
            {
                Directory.Delete(dir, true);
            }
        }

        [TestMethod]
        public void MidiSource_SynthesizesWithSoundFont()
        {
            var font = SoundFontLocator.Load(null);
            if (font == null)
            {
                Assert.Inconclusive("SoundFont がありません（SRC_SOUNDFONT か /usr/share/sounds/sf2/ に置く）");
            }
            var source = new MidiSource(font, new MemoryStream(TestAudio.Midi()));
            var samples = ReadAll(source);
            Assert.IsTrue(samples.Length > 0);
            Assert.IsTrue(samples.Any(x => Math.Abs(x) > 0.01f), "audible");
        }
    }
}
