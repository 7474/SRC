using MeltySynth;
using SRCCore;
using SRCSound;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace SRCSharpWeb
{
    /// <summary>
    /// Web Audio API による IPlaySound。
    /// WAV・MP3 はブラウザで復号して鳴らす。MIDI は SoundFont があれば MeltySynth で合成し、PCM を少しずつ送る。
    /// JS を呼べるのは UI 側のスレッドだけのため、依頼は WebScreenHost の描画の機会に行う（Pump）。
    /// </summary>
    public sealed class WebSoundPlayer : IPlaySound
    {
        // 1 回に送る長さ（44.1kHz で約 0.19 秒）と、先に送っておく長さ
        private const int ChunkFrames = 8192;
        private const double AheadSeconds = 1.0;
        private const int RenderedChunks = 8;

        private readonly WebScreenHost host;
        private readonly Func<SoundFont> soundFontProvider;
        private readonly object sync = new object();
        private readonly Dictionary<int, MidiVoice> midiVoices = new Dictionary<int, MidiVoice>();
        private SoundFont soundFont;
        private bool soundFontResolved;
        private float volume = 1f;
        private volatile bool bgmPlaying;

        /// <param name="soundFontProvider">MIDI に使う SoundFont を返す（無ければ null）。初めて MIDI を再生する時に呼ぶ。</param>
        public WebSoundPlayer(WebScreenHost host, Func<SoundFont> soundFontProvider)
        {
            this.host = host;
            this.soundFontProvider = soundFontProvider;
            host.Flushed += Pump;
        }

        public BGMStatus BGMStatus => bgmPlaying ? BGMStatus.Playing : BGMStatus.Stopped;

        public float SoundVolume
        {
            get => volume;
            set
            {
                volume = Math.Max(0f, Math.Min(1f, value));
                var v = volume;
                host.Post(() => WebInterop.SoundVolume(v));
            }
        }

        public SoundType ResolveSoundType(string path)
        {
            switch (Path.GetExtension(path ?? "").ToLowerInvariant())
            {
                case ".mid":
                case ".midi":
                    return SoundType.Midi;
                case ".wav":
                    return SoundType.Wave;
                case ".mp3":
                    return SoundType.Mp3;
                case ".aif":
                case ".aiff":
                    return SoundType.Aiff;
                default:
                    return SoundType.Unknown;
            }
        }

        public void Play(int channel, Stream stream, SoundType soundType, PlaySoundMode mode)
        {
            Stop(channel);
            var loop = mode.HasFlag(PlaySoundMode.Repeat);
            byte[] bytes;
            using (var buffer = new MemoryStream())
            {
                stream.CopyTo(buffer);
                bytes = buffer.ToArray();
            }
            switch (soundType)
            {
                case SoundType.Wave:
                case SoundType.Mp3:
                    host.Post(() => WebInterop.SoundPlayEncoded(channel, new ArraySegment<byte>(bytes), loop));
                    SetBgm(channel, true);
                    break;
                case SoundType.Midi:
                    var font = SoundFont();
                    if (font == null)
                    {
                        return;
                    }
                    var voice = new MidiVoice(new MidiSource(font, new MemoryStream(bytes)), loop);
                    lock (sync)
                    {
                        midiVoices[channel] = voice;
                    }
                    voice.Start();
                    SetBgm(channel, true);
                    break;
                default:
                    // AIFF などは鳴らさない
                    break;
            }
        }

        public void Stop(int channel)
        {
            lock (sync)
            {
                if (midiVoices.TryGetValue(channel, out var voice))
                {
                    voice.Dispose();
                    midiVoices.Remove(channel);
                }
            }
            SetBgm(channel, false);
            host.Post(() => WebInterop.SoundStop(channel));
        }

        private void SetBgm(int channel, bool playing)
        {
            if (channel == PlaySoundConstants.CH_BGM)
            {
                bgmPlaying = playing;
            }
        }

        private SoundFont SoundFont()
        {
            lock (sync)
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
                        Console.Error.WriteLine("SoundFont を読み込めません: " + ex.Message);
                    }
                    if (soundFont == null)
                    {
                        Console.WriteLine("SoundFont（.sf2）が無いため MIDI は鳴りません。");
                    }
                }
                return soundFont;
            }
        }

        /// <summary>UI 側のスレッドで、合成済みの MIDI を必要な分だけ送る</summary>
        private void Pump()
        {
            List<KeyValuePair<int, MidiVoice>> list;
            lock (sync)
            {
                list = new List<KeyValuePair<int, MidiVoice>>(midiVoices);
            }
            foreach (var (channel, voice) in list)
            {
                while (WebInterop.SoundAhead(channel) < AheadSeconds && voice.Chunks.TryDequeue(out var chunk))
                {
                    WebInterop.SoundQueuePcm(channel, new ArraySegment<byte>(MemoryMarshal.AsBytes(chunk.AsSpan()).ToArray()), 2, MidiSource.Rate);
                }
                if (voice.Finished && voice.Chunks.IsEmpty)
                {
                    lock (sync)
                    {
                        if (midiVoices.TryGetValue(channel, out var current) && current == voice)
                        {
                            midiVoices.Remove(channel);
                            SetBgm(channel, false);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            lock (sync)
            {
                foreach (var voice in midiVoices.Values)
                {
                    voice.Dispose();
                }
                midiVoices.Clear();
            }
        }

        /// <summary>MIDI を別のスレッドで少し先まで合成しておく</summary>
        private sealed class MidiVoice : IDisposable
        {
            private readonly ISampleSource source;
            private readonly bool loop;
            private volatile bool disposed;

            public MidiVoice(ISampleSource source, bool loop)
            {
                this.source = source;
                this.loop = loop;
            }

            public ConcurrentQueue<float[]> Chunks { get; } = new ConcurrentQueue<float[]>();
            public bool Finished { get; private set; }

            public void Start()
            {
                new Thread(Render) { IsBackground = true, Name = "SRC#Web MIDI" }.Start();
            }

            private void Render()
            {
                try
                {
                    while (!disposed)
                    {
                        if (Chunks.Count >= RenderedChunks)
                        {
                            Thread.Sleep(20);
                            continue;
                        }
                        var chunk = new float[ChunkFrames * 2];
                        var read = source.Read(chunk, 0, chunk.Length);
                        if (read == 0)
                        {
                            if (!loop)
                            {
                                break;
                            }
                            source.Rewind();
                            continue;
                        }
                        if (read < chunk.Length)
                        {
                            Array.Resize(ref chunk, read);
                        }
                        Chunks.Enqueue(chunk);
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine("MIDI を合成できません: " + ex.Message);
                }
                Finished = true;
            }

            public void Dispose()
            {
                disposed = true;
            }
        }
    }
}
