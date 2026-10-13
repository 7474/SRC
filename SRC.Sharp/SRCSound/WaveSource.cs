using System;
using System.IO;
using System.Text;

namespace SRCSound
{
    /// <summary>
    /// WAV（RIFF）の PCM（8/16/24/32 bit）と IEEE float を読む。
    /// </summary>
    public sealed class WaveSource : ISampleSource
    {
        private readonly float[] samples;
        private int position;

        public WaveSource(Stream stream)
        {
            using (var reader = new BinaryReader(stream, Encoding.ASCII, true))
            {
                if (new string(reader.ReadChars(4)) != "RIFF")
                {
                    throw new InvalidDataException("RIFF ヘッダがありません");
                }
                reader.ReadInt32();
                if (new string(reader.ReadChars(4)) != "WAVE")
                {
                    throw new InvalidDataException("WAVE ではありません");
                }

                int format = 0, bits = 0;
                byte[] data = null;
                while (stream.Position + 8 <= stream.Length)
                {
                    var id = new string(reader.ReadChars(4));
                    var size = reader.ReadInt32();
                    var next = stream.Position + size + (size & 1);
                    if (id == "fmt ")
                    {
                        format = reader.ReadInt16();
                        Channels = reader.ReadInt16();
                        SampleRate = reader.ReadInt32();
                        reader.ReadInt32();
                        reader.ReadInt16();
                        bits = reader.ReadInt16();
                        if (format == 0xFFFE && size >= 40)
                        {
                            // WAVE_FORMAT_EXTENSIBLE: SubFormat の先頭 2 バイトが形式
                            reader.ReadInt16();
                            reader.ReadInt16();
                            reader.ReadInt32();
                            format = reader.ReadInt16();
                        }
                    }
                    else if (id == "data")
                    {
                        data = reader.ReadBytes((int)Math.Min(size, stream.Length - stream.Position));
                    }
                    if (next > stream.Length)
                    {
                        break;
                    }
                    stream.Position = next;
                }

                if (data == null || Channels < 1 || Channels > 2)
                {
                    throw new InvalidDataException("対応していない WAV です");
                }
                samples = Convert(data, format, bits);
            }
        }

        public int SampleRate { get; }
        public int Channels { get; }

        /// <summary>サンプル数（全チャンネル分）</summary>
        public int Length => samples.Length;

        private static float[] Convert(byte[] data, int format, int bits)
        {
            switch (format)
            {
                case 1:
                    switch (bits)
                    {
                        case 8:
                            {
                                var result = new float[data.Length];
                                for (var i = 0; i < result.Length; i++)
                                {
                                    result[i] = (data[i] - 128) / 128f;
                                }
                                return result;
                            }
                        case 16:
                            {
                                var result = new float[data.Length / 2];
                                for (var i = 0; i < result.Length; i++)
                                {
                                    result[i] = BitConverter.ToInt16(data, i * 2) / 32768f;
                                }
                                return result;
                            }
                        case 24:
                            {
                                var result = new float[data.Length / 3];
                                for (var i = 0; i < result.Length; i++)
                                {
                                    var v = data[i * 3] | (data[i * 3 + 1] << 8) | (data[i * 3 + 2] << 16);
                                    result[i] = ((v << 8) >> 8) / 8388608f;
                                }
                                return result;
                            }
                        case 32:
                            {
                                var result = new float[data.Length / 4];
                                for (var i = 0; i < result.Length; i++)
                                {
                                    result[i] = BitConverter.ToInt32(data, i * 4) / 2147483648f;
                                }
                                return result;
                            }
                    }
                    break;
                case 3:
                    if (bits == 32)
                    {
                        var result = new float[data.Length / 4];
                        Buffer.BlockCopy(data, 0, result, 0, result.Length * 4);
                        return result;
                    }
                    break;
            }
            throw new InvalidDataException($"対応していない WAV の形式です（format={format}, bits={bits}）");
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var n = Math.Min(count, samples.Length - position);
            Array.Copy(samples, position, buffer, offset, n);
            position += n;
            return n;
        }

        public void Rewind()
        {
            position = 0;
        }
    }
}
