using System;
using System.IO;
using System.Text;

namespace SRCSound.Tests
{
    /// <summary>テスト用の音声データを作る。</summary>
    internal static class TestAudio
    {
        /// <summary>16 bit PCM の WAV（正弦波）</summary>
        public static byte[] Wave(int sampleRate, int channels, int frames, int bits = 16)
        {
            using (var stream = new MemoryStream())
            using (var writer = new BinaryWriter(stream, Encoding.ASCII))
            {
                var bytesPerSample = bits / 8;
                var dataSize = frames * channels * bytesPerSample;
                writer.Write(Encoding.ASCII.GetBytes("RIFF"));
                writer.Write(36 + dataSize);
                writer.Write(Encoding.ASCII.GetBytes("WAVE"));
                writer.Write(Encoding.ASCII.GetBytes("fmt "));
                writer.Write(16);
                writer.Write((short)1);
                writer.Write((short)channels);
                writer.Write(sampleRate);
                writer.Write(sampleRate * channels * bytesPerSample);
                writer.Write((short)(channels * bytesPerSample));
                writer.Write((short)bits);
                writer.Write(Encoding.ASCII.GetBytes("data"));
                writer.Write(dataSize);
                for (var i = 0; i < frames; i++)
                {
                    var v = Math.Sin(i * 440 * 2 * Math.PI / sampleRate) * 0.5;
                    for (var c = 0; c < channels; c++)
                    {
                        if (bits == 8)
                        {
                            writer.Write((byte)(128 + v * 127));
                        }
                        else
                        {
                            writer.Write((short)(v * 32767));
                        }
                    }
                }
                return stream.ToArray();
            }
        }

        /// <summary>1 音だけの SMF（format 0）</summary>
        public static byte[] Midi()
        {
            var track = new byte[]
            {
                0x00, 0xC0, 0x00,             // program change
                0x00, 0x90, 0x3C, 0x64,       // note on C4
                0x83, 0x60, 0x80, 0x3C, 0x40, // 480 ticks later note off
                0x00, 0xFF, 0x2F, 0x00,       // end of track
            };
            using (var stream = new MemoryStream())
            {
                void WriteBE(int value, int size)
                {
                    for (var i = size - 1; i >= 0; i--)
                    {
                        stream.WriteByte((byte)(value >> (8 * i)));
                    }
                }
                stream.Write(Encoding.ASCII.GetBytes("MThd"), 0, 4);
                WriteBE(6, 4);
                WriteBE(0, 2);
                WriteBE(1, 2);
                WriteBE(480, 2);
                stream.Write(Encoding.ASCII.GetBytes("MTrk"), 0, 4);
                WriteBE(track.Length, 4);
                stream.Write(track, 0, track.Length);
                return stream.ToArray();
            }
        }
    }
}
