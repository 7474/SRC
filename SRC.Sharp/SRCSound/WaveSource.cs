using System;
using System.IO;
using System.Text;

namespace SRCSound
{
    /// <summary>
    /// WAV（RIFF）を読む。PCM（8/16/24/32 bit）・IEEE float と、元の SRC の時代の素材で使われていた
    /// 圧縮形式（MS ADPCM・IMA ADPCM・μ-law・A-law）に対応する。元の SRC は Windows の音声の仕組み（ACM）で再生していたため、
    /// これらも鳴っていた。
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

                int format = 0, bits = 0, blockAlign = 0, samplesPerBlock = 0;
                short[] coefficients = null;
                byte[] data = null;
                while (stream.Position + 8 <= stream.Length)
                {
                    var id = new string(reader.ReadChars(4));
                    var size = reader.ReadInt32();
                    var next = stream.Position + size + (size & 1);
                    if (id == "fmt ")
                    {
                        format = reader.ReadUInt16();
                        Channels = reader.ReadInt16();
                        SampleRate = reader.ReadInt32();
                        reader.ReadInt32();
                        blockAlign = reader.ReadUInt16();
                        bits = reader.ReadInt16();
                        var extra = size >= 18 ? reader.ReadUInt16() : 0;
                        if (format == FormatExtensible && extra >= 22)
                        {
                            // WAVE_FORMAT_EXTENSIBLE: SubFormat の先頭 2 バイトが形式
                            reader.ReadInt16();
                            reader.ReadInt32();
                            format = reader.ReadUInt16();
                        }
                        else if ((format == FormatMsAdpcm || format == FormatImaAdpcm) && extra >= 2)
                        {
                            samplesPerBlock = reader.ReadUInt16();
                            if (format == FormatMsAdpcm && extra >= 4)
                            {
                                var count = reader.ReadUInt16();
                                coefficients = new short[count * 2];
                                for (var i = 0; i < coefficients.Length && stream.Position + 2 <= next; i++)
                                {
                                    coefficients[i] = reader.ReadInt16();
                                }
                            }
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
                samples = format == FormatMsAdpcm ? DecodeMsAdpcm(data, Channels, blockAlign, samplesPerBlock, coefficients)
                    : format == FormatImaAdpcm ? DecodeImaAdpcm(data, Channels, blockAlign, samplesPerBlock)
                    : Convert(data, format, bits);
            }
        }

        private const int FormatMsAdpcm = 2;
        private const int FormatALaw = 6;
        private const int FormatMuLaw = 7;
        private const int FormatImaAdpcm = 0x11;
        private const int FormatExtensible = 0xFFFE;

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
                case FormatMuLaw:
                case FormatALaw:
                    if (bits == 8)
                    {
                        var result = new float[data.Length];
                        for (var i = 0; i < result.Length; i++)
                        {
                            result[i] = (format == FormatMuLaw ? MuLaw(data[i]) : ALaw(data[i])) / 32768f;
                        }
                        return result;
                    }
                    break;
            }
            throw new InvalidDataException($"対応していない WAV の形式です（format={format}, bits={bits}）");
        }

        /// <summary>G.711 μ-law の 1 サンプル</summary>
        internal static short MuLaw(byte value)
        {
            var u = ~value & 0xFF;
            var t = (((u & 0x0F) << 3) + 0x84) << ((u & 0x70) >> 4);
            return (short)((u & 0x80) != 0 ? 0x84 - t : t - 0x84);
        }

        /// <summary>G.711 A-law の 1 サンプル</summary>
        internal static short ALaw(byte value)
        {
            var a = value ^ 0x55;
            var t = (a & 0x0F) << 4;
            var segment = (a & 0x70) >> 4;
            t = segment == 0 ? t + 8 : segment == 1 ? t + 0x108 : (t + 0x108) << (segment - 1);
            return (short)((a & 0x80) != 0 ? t : -t);
        }

        private static readonly int[] MsAdpcmAdaptation = { 230, 230, 230, 230, 307, 409, 512, 614, 768, 614, 512, 409, 307, 230, 230, 230 };
        private static readonly short[] MsAdpcmDefaultCoefficients = { 256, 0, 512, -256, 0, 0, 192, 64, 240, 0, 460, -208, 392, -232 };

        private static short Clamp16(int value) => (short)Math.Max(short.MinValue, Math.Min(short.MaxValue, value));

        /// <summary>
        /// MS ADPCM（WAVE_FORMAT_ADPCM）を復号する。ブロックごとに、チャンネルごとの予測係数の番号・量子化幅・直前の 2 サンプルを持ち、
        /// 続く 4 ビットの差分（上位のニブルが先）から 1 サンプルずつ求める
        /// </summary>
        internal static float[] DecodeMsAdpcm(byte[] data, int channels, int blockAlign, int samplesPerBlock, short[] coefficients)
        {
            coefficients = coefficients == null || coefficients.Length < 2 ? MsAdpcmDefaultCoefficients : coefficients;
            var headerSize = 7 * channels;
            if (blockAlign <= headerSize)
            {
                throw new InvalidDataException("MS ADPCM のブロックの大きさが不正です");
            }
            if (samplesPerBlock <= 0)
            {
                samplesPerBlock = (blockAlign - headerSize) * 2 / channels + 2;
            }
            var result = new System.Collections.Generic.List<float>(data.Length / blockAlign * samplesPerBlock * channels + samplesPerBlock * channels);
            var coef1 = new int[channels];
            var coef2 = new int[channels];
            var delta = new int[channels];
            var sample1 = new int[channels];
            var sample2 = new int[channels];
            for (var block = 0; block + headerSize <= data.Length; block += blockAlign)
            {
                var end = Math.Min(block + blockAlign, data.Length);
                var p = block;
                for (var c = 0; c < channels; c++)
                {
                    var index = Math.Min(data[p++], coefficients.Length / 2 - 1);
                    coef1[c] = coefficients[index * 2];
                    coef2[c] = coefficients[index * 2 + 1];
                }
                for (var c = 0; c < channels; c++, p += 2) { delta[c] = BitConverter.ToInt16(data, p); }
                for (var c = 0; c < channels; c++, p += 2) { sample1[c] = BitConverter.ToInt16(data, p); }
                for (var c = 0; c < channels; c++, p += 2) { sample2[c] = BitConverter.ToInt16(data, p); }
                // ブロックの最初の 2 サンプルは、ヘッダの古い方（sample2）から
                for (var c = 0; c < channels; c++) { result.Add(sample2[c] / 32768f); }
                for (var c = 0; c < channels; c++) { result.Add(sample1[c] / 32768f); }
                var produced = 2;
                var channel = 0;
                for (; p < end && produced < samplesPerBlock; p++)
                {
                    foreach (var nibble in new[] { data[p] >> 4, data[p] & 0x0F })
                    {
                        var signed = nibble >= 8 ? nibble - 16 : nibble;
                        var predicted = (sample1[channel] * coef1[channel] + sample2[channel] * coef2[channel]) / 256 + signed * delta[channel];
                        var sample = Clamp16(predicted);
                        sample2[channel] = sample1[channel];
                        sample1[channel] = sample;
                        delta[channel] = Math.Max(16, MsAdpcmAdaptation[nibble] * delta[channel] / 256);
                        result.Add(sample / 32768f);
                        channel = (channel + 1) % channels;
                        if (channel == 0)
                        {
                            produced++;
                        }
                    }
                }
            }
            return result.ToArray();
        }

        private static readonly int[] ImaIndexTable = { -1, -1, -1, -1, 2, 4, 6, 8, -1, -1, -1, -1, 2, 4, 6, 8 };
        private static readonly int[] ImaStepTable =
        {
            7, 8, 9, 10, 11, 12, 13, 14, 16, 17, 19, 21, 23, 25, 28, 31, 34, 37, 41, 45, 50, 55, 60, 66, 73, 80, 88, 97, 107, 118,
            130, 143, 157, 173, 190, 209, 230, 253, 279, 307, 337, 371, 408, 449, 494, 544, 598, 658, 724, 796, 876, 963, 1060,
            1166, 1282, 1411, 1552, 1707, 1878, 2066, 2272, 2499, 2749, 3024, 3327, 3660, 4026, 4428, 4871, 5358, 5894, 6484,
            7132, 7845, 8630, 9493, 10442, 11487, 12635, 13899, 15289, 16818, 18500, 20350, 22385, 24623, 27086, 29794, 32767,
        };

        /// <summary>
        /// IMA ADPCM（WAVE_FORMAT_DVI_ADPCM）を復号する。ブロックごとに、チャンネルごとの最初のサンプルと量子化幅の番号を持ち、
        /// 続くデータはチャンネルごとに 4 バイト（8 サンプル。下位のニブルが先）ずつ交互に並ぶ
        /// </summary>
        internal static float[] DecodeImaAdpcm(byte[] data, int channels, int blockAlign, int samplesPerBlock)
        {
            var headerSize = 4 * channels;
            if (blockAlign <= headerSize)
            {
                throw new InvalidDataException("IMA ADPCM のブロックの大きさが不正です");
            }
            if (samplesPerBlock <= 0)
            {
                samplesPerBlock = (blockAlign - headerSize) * 2 / channels + 1;
            }
            var result = new System.Collections.Generic.List<float>(data.Length / blockAlign * samplesPerBlock * channels + samplesPerBlock * channels);
            var predictor = new int[channels];
            var index = new int[channels];
            var blockSamples = new float[channels][];
            for (var block = 0; block + headerSize <= data.Length; block += blockAlign)
            {
                var end = Math.Min(block + blockAlign, data.Length);
                var p = block;
                for (var c = 0; c < channels; c++)
                {
                    predictor[c] = BitConverter.ToInt16(data, p);
                    index[c] = Math.Max(0, Math.Min(88, (int)data[p + 2]));
                    p += 4;
                    blockSamples[c] = new float[samplesPerBlock];
                    blockSamples[c][0] = predictor[c] / 32768f;
                }
                var counts = new int[channels];
                for (var c = 0; c < channels; c++) { counts[c] = 1; }
                while (p < end)
                {
                    for (var c = 0; c < channels && p < end; c++)
                    {
                        for (var b = 0; b < 4 && p < end; b++, p++)
                        {
                            foreach (var nibble in new[] { data[p] & 0x0F, data[p] >> 4 })
                            {
                                var step = ImaStepTable[index[c]];
                                var diff = step >> 3;
                                if ((nibble & 4) != 0) { diff += step; }
                                if ((nibble & 2) != 0) { diff += step >> 1; }
                                if ((nibble & 1) != 0) { diff += step >> 2; }
                                predictor[c] = Clamp16((nibble & 8) != 0 ? predictor[c] - diff : predictor[c] + diff);
                                index[c] = Math.Max(0, Math.Min(88, index[c] + ImaIndexTable[nibble]));
                                if (counts[c] < samplesPerBlock)
                                {
                                    blockSamples[c][counts[c]++] = predictor[c] / 32768f;
                                }
                            }
                        }
                    }
                }
                var length = counts[0];
                for (var c = 1; c < channels; c++) { length = Math.Min(length, counts[c]); }
                for (var i = 0; i < length; i++)
                {
                    for (var c = 0; c < channels; c++)
                    {
                        result.Add(blockSamples[c][i]);
                    }
                }
            }
            return result.ToArray();
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
