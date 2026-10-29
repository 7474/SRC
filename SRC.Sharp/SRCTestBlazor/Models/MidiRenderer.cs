using MeltySynth;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SRCTestBlazor.Models
{
    /// <summary>
    /// MIDI を SoundFont で WAV に合成する（ブラウザは MIDI を鳴らせないため。SRC#Web と同じ MeltySynth）。
    /// できた WAV は &lt;audio&gt; で鳴らす
    /// </summary>
    public class MidiRenderer
    {
        public const int SampleRate = 44100;
        /// <summary>合成する長さの上限</summary>
        public static readonly TimeSpan MaxLength = TimeSpan.FromMinutes(10);

        private readonly Dictionary<string, Task<SoundFont>> soundFonts = new Dictionary<string, Task<SoundFont>>();

        private Task<SoundFont> GetSoundFontAsync(HttpClient http, string uri)
        {
            lock (soundFonts)
            {
                if (!soundFonts.TryGetValue(uri, out var task))
                {
                    task = LoadSoundFontAsync(http, uri);
                    soundFonts[uri] = task;
                }
                return task;
            }
        }

        private static async Task<SoundFont> LoadSoundFontAsync(HttpClient http, string uri)
        {
            using var stream = new MemoryStream(await http.GetByteArrayAsync(uri));
            return new SoundFont(stream);
        }

        /// <summary>MIDI を合成して WAV（16 ビット・ステレオ）にする。progress は 0〜1</summary>
        public async Task<byte[]> RenderAsync(HttpClient http, string midiUri, string soundFontUri, Action<double> progress, CancellationToken cancellationToken = default)
        {
            var soundFont = await GetSoundFontAsync(http, soundFontUri);
            using var midiStream = new MemoryStream(await http.GetByteArrayAsync(midiUri, cancellationToken));
            var midi = new MidiFile(midiStream);
            var length = midi.Length < MaxLength ? midi.Length : MaxLength;
            var frames = (int)(length.TotalSeconds * SampleRate);
            var sequencer = new MidiFileSequencer(new Synthesizer(soundFont, SampleRate));
            sequencer.Play(midi, false);

            var pcm = new byte[frames * 4];
            // WebAssembly は 1 つのスレッドで動くので、少しずつ合成して画面に戻す
            const int block = SampleRate / 2;
            var left = new float[block];
            var right = new float[block];
            for (var done = 0; done < frames; done += block)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var n = Math.Min(block, frames - done);
                sequencer.Render(left.AsSpan(0, n), right.AsSpan(0, n));
                for (var i = 0; i < n; i++)
                {
                    WriteSample(pcm, (done + i) * 4, left[i]);
                    WriteSample(pcm, (done + i) * 4 + 2, right[i]);
                }
                progress?.Invoke((double)(done + n) / frames);
                await Task.Yield();
            }
            return Wav(pcm);
        }

        private static void WriteSample(byte[] buffer, int offset, float value)
        {
            var s = (short)Math.Clamp((int)(value * short.MaxValue), short.MinValue, short.MaxValue);
            buffer[offset] = (byte)s;
            buffer[offset + 1] = (byte)(s >> 8);
        }

        private static byte[] Wav(byte[] pcm)
        {
            using var ms = new MemoryStream(44 + pcm.Length);
            using var w = new BinaryWriter(ms);
            w.Write("RIFF"u8);
            w.Write(36 + pcm.Length);
            w.Write("WAVEfmt "u8);
            w.Write(16);
            w.Write((short)1);
            w.Write((short)2);
            w.Write(SampleRate);
            w.Write(SampleRate * 4);
            w.Write((short)4);
            w.Write((short)16);
            w.Write("data"u8);
            w.Write(pcm.Length);
            w.Write(pcm);
            return ms.ToArray();
        }
    }
}
