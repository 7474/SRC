using MeltySynth;
using Microsoft.Extensions.Logging;
using Silk.NET.OpenAL;
using SRCCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace SRCSound
{
    /// <summary>
    /// OpenAL（OpenAL Soft）による IPlaySound。Windows / macOS / Linux で動く。
    /// WAV・MP3 を再生し、MIDI は SoundFont があれば MeltySynth で合成して再生する。
    /// 音声出力が無い環境では何も鳴らさずに動く。
    /// </summary>
    public sealed unsafe class OpenALSoundPlayer : IPlaySound
    {
        private const int BufferCount = 4;
        // 1 バッファあたりのフレーム数（44.1kHz で約 0.1 秒）
        private const int BufferFrames = 4096;

        private readonly ILogger logger;
        private readonly object sync = new object();
        private readonly Dictionary<int, Voice> voices = new Dictionary<int, Voice>();
        private readonly Func<SoundFont> soundFontProvider;
        private SoundFont soundFont;
        private bool soundFontResolved;

        private readonly AL al;
        private readonly ALContext alc;
        private readonly Device* device;
        private readonly Context* context;
        private readonly Thread pump;
        private volatile bool disposed;
        private float volume = 1f;

        /// <param name="soundFontProvider">MIDI に使う SoundFont を返す（無ければ null）。初めて MIDI を再生する時に呼ぶ。</param>
        public OpenALSoundPlayer(Func<SoundFont> soundFontProvider = null, ILogger logger = null)
        {
            this.soundFontProvider = soundFontProvider;
            this.logger = logger;
            try
            {
                alc = ALContext.GetApi(true);
                al = AL.GetApi(true);
                device = alc.OpenDevice("");
                if (device == null)
                {
                    logger?.LogWarning("音声出力デバイスを開けません。音は鳴りません。");
                    return;
                }
                context = alc.CreateContext(device, null);
                alc.MakeContextCurrent(context);
                IsAvailable = true;
                pump = new Thread(Pump) { IsBackground = true, Name = "SRC Sound" };
                pump.Start();
            }
            catch (Exception ex)
            {
                // OpenAL のネイティブライブラリが無い環境など
                logger?.LogWarning(ex, "OpenAL を初期化できません。音は鳴りません。");
                IsAvailable = false;
            }
        }

        /// <summary>音声出力が使えるか</summary>
        public bool IsAvailable { get; }

        public BGMStatus BGMStatus
        {
            get
            {
                lock (sync)
                {
                    return voices.TryGetValue(PlaySoundConstants.CH_BGM, out var voice) && !voice.Finished
                        ? BGMStatus.Playing
                        : BGMStatus.Stopped;
                }
            }
        }

        public float SoundVolume
        {
            get => volume;
            set
            {
                volume = Math.Max(0f, Math.Min(1f, value));
                if (!IsAvailable)
                {
                    return;
                }
                lock (sync)
                {
                    foreach (var voice in voices.Values)
                    {
                        al.SetSourceProperty(voice.Source, SourceFloat.Gain, volume);
                    }
                }
            }
        }

        public void Play(int channel, Stream stream, SoundType soundType, PlaySoundMode mode)
        {
            Stop(channel);
            if (!IsAvailable)
            {
                return;
            }
            var source = CreateSource(stream, soundType);
            if (source == null)
            {
                return;
            }
            logger?.LogDebug("Play channel={0} type={1} rate={2} channels={3} mode={4}", channel, soundType, source.SampleRate, source.Channels, mode);
            lock (sync)
            {
                var voice = new Voice(al, source, mode.HasFlag(PlaySoundMode.Repeat));
                al.SetSourceProperty(voice.Source, SourceFloat.Gain, volume);
                voices[channel] = voice;
                voice.Fill(al);
                al.SourcePlay(voice.Source);
            }
        }

        private ISampleSource CreateSource(Stream stream, SoundType soundType)
        {
            try
            {
                switch (soundType)
                {
                    case SoundType.Wave:
                        return new WaveSource(stream);
                    case SoundType.Mp3:
                        return new Mp3Source(stream);
                    case SoundType.Midi:
                        var font = ResolveSoundFont();
                        if (font == null)
                        {
                            return null;
                        }
                        return new MidiSource(font, stream);
                    default:
                        logger?.LogWarning("再生できない形式です: {0}", soundType);
                        return null;
                }
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "音声を読み込めません: {0}", soundType);
                return null;
            }
        }

        private SoundFont ResolveSoundFont()
        {
            if (!soundFontResolved)
            {
                soundFontResolved = true;
                try
                {
                    soundFont = soundFontProvider?.Invoke();
                }
                catch (Exception ex)
                {
                    logger?.LogWarning(ex, "SoundFont を読み込めません。");
                }
                if (soundFont == null)
                {
                    logger?.LogWarning("SoundFont が無いため MIDI は鳴りません。");
                }
            }
            return soundFont;
        }

        public void Stop(int channel)
        {
            if (!IsAvailable)
            {
                return;
            }
            lock (sync)
            {
                if (voices.TryGetValue(channel, out var voice))
                {
                    voice.Dispose(al);
                    voices.Remove(channel);
                }
            }
        }

        public SoundType ResolveSoundType(string path)
        {
            var ext = Path.GetExtension(path ?? "").ToLowerInvariant();
            switch (ext)
            {
                case ".wav":
                    return SoundType.Wave;
                case ".mp3":
                    return SoundType.Mp3;
                case ".aif":
                case ".aiff":
                    return SoundType.Aiff;
                case ".mid":
                case ".midi":
                    return SoundType.Midi;
                default:
                    return SoundType.Unknown;
            }
        }

        /// <summary>再生済みのバッファに続きを詰める。</summary>
        private void Pump()
        {
            while (!disposed)
            {
                lock (sync)
                {
                    if (disposed)
                    {
                        break;
                    }
                    foreach (var voice in voices.Values)
                    {
                        voice.Update(al);
                    }
                }
                Thread.Sleep(20);
            }
        }

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }
            disposed = true;
            if (!IsAvailable)
            {
                return;
            }
            pump?.Join(1000);
            lock (sync)
            {
                foreach (var voice in voices.Values)
                {
                    voice.Dispose(al);
                }
                voices.Clear();
            }
            alc.MakeContextCurrent(null);
            alc.DestroyContext(context);
            alc.CloseDevice(device);
        }

        /// <summary>1 チャンネル分の再生（OpenAL のソースとストリーミング用バッファ）。</summary>
        private sealed class Voice
        {
            private readonly ISampleSource source;
            private readonly bool loop;
            private readonly uint[] buffers;
            private readonly float[] samples;
            private readonly short[] pcm;
            private bool endOfSource;

            public Voice(AL al, ISampleSource source, bool loop)
            {
                this.source = source;
                this.loop = loop;
                Source = al.GenSource();
                buffers = new uint[BufferCount];
                for (var i = 0; i < BufferCount; i++)
                {
                    buffers[i] = al.GenBuffer();
                }
                samples = new float[BufferFrames * source.Channels];
                pcm = new short[samples.Length];
            }

            public uint Source { get; }

            /// <summary>最後まで再生し終えたか</summary>
            public bool Finished { get; private set; }

            /// <summary>最初のバッファを詰めて並べる。</summary>
            public void Fill(AL al)
            {
                foreach (var buffer in buffers)
                {
                    if (!Queue(al, buffer))
                    {
                        break;
                    }
                }
            }

            public void Update(AL al)
            {
                if (Finished)
                {
                    return;
                }
                al.GetSourceProperty(Source, GetSourceInteger.BuffersProcessed, out var processed);
                for (var i = 0; i < processed; i++)
                {
                    uint buffer = 0;
                    al.SourceUnqueueBuffers(Source, 1, &buffer);
                    Queue(al, buffer);
                }
                al.GetSourceProperty(Source, GetSourceInteger.SourceState, out var state);
                if ((SourceState)state != SourceState.Playing)
                {
                    al.GetSourceProperty(Source, GetSourceInteger.BuffersQueued, out var queued);
                    if (queued > 0)
                    {
                        // 詰めるのが遅れて止まった場合は再開する
                        al.SourcePlay(Source);
                    }
                    else if (endOfSource)
                    {
                        Finished = true;
                    }
                }
            }

            private bool Queue(AL al, uint buffer)
            {
                if (endOfSource)
                {
                    return false;
                }
                var count = 0;
                var emptyRewinds = 0;
                while (count < samples.Length)
                {
                    var read = source.Read(samples, count, samples.Length - count);
                    if (read > 0)
                    {
                        count += read;
                        emptyRewinds = 0;
                        continue;
                    }
                    // 終端。ループする場合は先頭に戻す（戻しても読めない長さ 0 の音声は終える）
                    if (!loop || ++emptyRewinds > 1)
                    {
                        break;
                    }
                    source.Rewind();
                }
                if (count == 0)
                {
                    endOfSource = true;
                    return false;
                }
                for (var i = 0; i < count; i++)
                {
                    var v = samples[i];
                    pcm[i] = (short)(v >= 1f ? short.MaxValue : v <= -1f ? short.MinValue : v * 32767f);
                }
                var format = source.Channels == 2 ? BufferFormat.Stereo16 : BufferFormat.Mono16;
                fixed (short* p = pcm)
                {
                    al.BufferData(buffer, format, p, count * sizeof(short), source.SampleRate);
                }
                al.SourceQueueBuffers(Source, 1, &buffer);
                if (count < samples.Length && !loop)
                {
                    endOfSource = true;
                }
                return true;
            }

            public void Dispose(AL al)
            {
                al.SourceStop(Source);
                al.SetSourceProperty(Source, SourceInteger.Buffer, 0);
                al.DeleteSource(Source);
                foreach (var buffer in buffers)
                {
                    al.DeleteBuffer(buffer);
                }
            }
        }
    }
}
