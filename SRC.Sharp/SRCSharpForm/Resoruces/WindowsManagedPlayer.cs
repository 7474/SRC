using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using SRCCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCSharpForm.Resoruces
{
    class WaveChannel : IDisposable
    {
        // https://github.com/naudio/NAudio/blob/master/Docs/PlayAudioFileWinForms.md
        public WaveOutEvent outputDevice;
        //public AudioFileReader audioFile;
        public bool repeat;

        public void Dispose()
        {
            //audioFile?.Dispose();
            //audioFile = null;
            outputDevice?.Dispose();
            outputDevice = null;
        }
    }

    public class WindowsManagedPlayer : IPlaySound
    {
        private IDictionary<int, WaveChannel> outputMap = new Dictionary<int, WaveChannel>();

        // https://github.com/melanchall/drywetmidi
        private OutputDevice midiOutput;
        private MidiFile midiFile;
        private Playback midiPlayback;

        private float volume = 0.10f;

        private const int CH_BGM = IPlaySound.CH_BGM;

        public BGMStatus BGMStatus
        {
            get
            {
                if (midiPlayback?.IsRunning == true)
                {
                    return BGMStatus.Playing;
                }
                if (BGMChannel?.outputDevice?.PlaybackState == PlaybackState.Playing)
                {
                    return BGMStatus.Playing;
                }
                return BGMStatus.Stopped;
            }
        }
        public float SoundVolume
        {
            get => volume; set
            {
                if (volume == value)
                {
                    return;
                }
                volume = value;
                // XXX Impl Volume
                foreach (var waveChannel in outputMap.Values)
                {
                    try
                    {
                        waveChannel.outputDevice.Volume = volume;
                    }
                    catch (Exception ex)
                    {
                        Program.Log.LogWarning(ex, ex.Message);
                    }
                }
                try
                {
                    midiOutput.Volume = new Volume((ushort)(Volume.FullLeft.LeftVolume * volume));
                }
                catch (Exception ex)
                {
                    Program.Log.LogWarning(ex, ex.Message);
                }
            }
        }

        private WaveChannel BGMChannel
        {
            get
            {
                WaveChannel waveChannel;
                if (outputMap.TryGetValue(CH_BGM, out waveChannel))
                {
                    return waveChannel;
                }
                else
                {
                    return null;
                }
            }
        }

        public void Dispose()
        {
            ReleaseMidiPlayback();

            foreach (var ch in outputMap.Keys)
            {
                outputMap[ch].Dispose();
                outputMap[ch] = null;
            }
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Play(int channel, Stream stream, SoundType soundType, PlaySoundMode mode)
        {
            Stop(channel);

            switch (soundType)
            {
                case SoundType.Midi:
                    if (CH_BGM != channel)
                    {
                        throw new ArgumentException($"{channel} is not support MIDI. MIDI channel is {CH_BGM}");
                    }
                    StartMidiPlayback(stream, mode);
                    break;
                default:
                    StartWave(channel, stream, soundType, mode);
                    break;
            }
        }

        public void Stop(int channel)
        {
            if (CH_BGM == channel)
            {
                ReleaseMidiPlayback();
            }
            if (outputMap.ContainsKey(channel))
            {
                outputMap[channel].outputDevice.Stop();
            }
        }

        public void StartWave(int channel, Stream stream, SoundType soundType, PlaySoundMode mode)
        {
            if (!outputMap.ContainsKey(channel))
            {
                var outputDevice = new WaveOutEvent();
                //outputDevice.PlaybackStopped += OnPlaybackStopped;
                outputMap[channel] = new WaveChannel()
                {
                    outputDevice = outputDevice,
                };
            }
            WaveChannel waveChannel = outputMap[channel];
            waveChannel.repeat = mode.HasFlag(PlaySoundMode.Repeat);
            waveChannel.outputDevice.Init(new LoopStream(CreateReaderStream(stream, soundType))
            {
                EnableLooping = waveChannel.repeat,
            });
            waveChannel.outputDevice.Volume = volume;
            waveChannel.outputDevice.Play();
        }

        // https://github.com/naudio/NAudio/blob/fb35ce8367f30b8bc5ea84e7d2529e172cf4c381/NAudio/AudioFileReader.cs#L45-L69
        private WaveStream CreateReaderStream(Stream stream, SoundType soundType)
        {
            switch (soundType)
            {
                case SoundType.Midi:
                    throw new InvalidOperationException(soundType + " is not supported.");
                case SoundType.Wave:
                    WaveStream readerStream = new WaveFileReader(stream);
                    if (readerStream.WaveFormat.Encoding != WaveFormatEncoding.Pcm && readerStream.WaveFormat.Encoding != WaveFormatEncoding.IeeeFloat)
                    {
                        readerStream = WaveFormatConversionStream.CreatePcmStream(readerStream);
                        readerStream = new BlockAlignReductionStream(readerStream);
                    }
                    return readerStream;
                case SoundType.Mp3:
                    return new Mp3FileReader(stream);
                case SoundType.Aiff:
                    return new AiffFileReader(stream);
                default:
                    // AudioFileReader は MediaFoundationReader に処理を移譲しているけれど、
                    // ファイル名があっても構築するのはちょっと大変そう
                    //return new MediaFoundationReader(stream);
                    throw new InvalidOperationException(soundType + " is not supported.");
            }
        }

        private void StartMidiPlayback(Stream stream, PlaySoundMode mode)
        {
            midiFile = MidiFile.Read(stream);
            try
            {
                midiOutput = OutputDevice.GetAll().First();
                midiPlayback = midiFile.GetPlayback(midiOutput);
                midiPlayback.Loop = mode.HasFlag(PlaySoundMode.Repeat);
                midiOutput.Volume = new Volume((ushort)(Volume.FullLeft.LeftVolume * volume));
                midiPlayback.Start();
                // XXX いい感じに無音シークしたい。
                //Task.Delay(100).Wait();
                //midiPlayback.MoveToNextSnapPoint(midiPlayback.Snapping.SnapToNotesStarts());
            }
            catch
            {
                ReleaseMidiPlayback();
                throw;
            }
        }

        private void ReleaseMidiPlayback()
        {
            try
            {
                if (midiPlayback != null)
                {
                    midiPlayback.Stop();
                    midiPlayback.Dispose();
                    midiPlayback = null;
                }
            }
            catch
            {
                // ignore
            }
            try
            {
                if (midiOutput != null)
                {
                    midiOutput.Dispose();
                    midiOutput = null;
                }
            }
            catch
            {
                // ignore
            }
        }

        public SoundType ResolveSoundType(string path)
        {
            var fileName = path;
            if (fileName.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                return SoundType.Wave;
            }
            else if (fileName.EndsWith(".mp3", StringComparison.OrdinalIgnoreCase))
            {
                return SoundType.Mp3;
            }
            else if (fileName.EndsWith(".aiff", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".aif", StringComparison.OrdinalIgnoreCase))
            {
                return SoundType.Aiff;
            }
            else if (fileName.EndsWith(".midi", StringComparison.OrdinalIgnoreCase)
                || fileName.EndsWith(".mid", StringComparison.OrdinalIgnoreCase))
            {
                return SoundType.Midi;
            }
            else
            {
                return SoundType.Unknown;
            }
        }
    }

    // https://markheath.net/post/looped-playback-in-net-with-naudio
    /// <summary>
    /// Stream for looping playback
    /// </summary>
    public class LoopStream : WaveStream
    {
        WaveStream sourceStream;

        /// <summary>
        /// Creates a new Loop stream
        /// </summary>
        /// <param name="sourceStream">The stream to read from. Note: the Read method of this stream should return 0 when it reaches the end
        /// or else we will not loop to the start again.</param>
        public LoopStream(WaveStream sourceStream)
        {
            this.sourceStream = sourceStream;
            this.EnableLooping = true;
        }

        /// <summary>
        /// Use this to turn looping on or off
        /// </summary>
        public bool EnableLooping { get; set; }

        /// <summary>
        /// Return source stream's wave format
        /// </summary>
        public override WaveFormat WaveFormat
        {
            get { return sourceStream.WaveFormat; }
        }

        /// <summary>
        /// LoopStream simply returns
        /// </summary>
        public override long Length
        {
            get { return sourceStream.Length; }
        }

        /// <summary>
        /// LoopStream simply passes on positioning to source stream
        /// </summary>
        public override long Position
        {
            get { return sourceStream.Position; }
            set { sourceStream.Position = value; }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int totalBytesRead = 0;

            while (totalBytesRead < count)
            {
                int bytesRead = sourceStream.Read(buffer, offset + totalBytesRead, count - totalBytesRead);
                if (bytesRead == 0)
                {
                    if (sourceStream.Position == 0 || !EnableLooping)
                    {
                        // something wrong with the source stream
                        break;
                    }
                    // loop
                    sourceStream.Position = 0;
                }
                totalBytesRead += bytesRead;
            }
            return totalBytesRead;
        }
    }
}
