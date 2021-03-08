using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Devices;
using NAudio.Wave;
using SRCCore;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SRCTestForm.Resoruces
{
    public class WindowsManagedPlayer : IPlaySound
    {
        // https://github.com/naudio/NAudio/blob/master/Docs/PlayAudioFileWinForms.md
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        // https://github.com/melanchall/drywetmidi
        private OutputDevice midiOutput;
        private MidiFile midiFile;
        private Playback midiPlayback;

        private bool repeate;

        private const int CH_MIDI = IPlaySound.CH_BGM;

        public BGMStatus BGMStatus => throw new NotImplementedException();

        public void Dispose()
        {
            ReleaseMidiPlayback();

            outputDevice?.Dispose();
            outputDevice = null;
            audioFile?.Dispose();
            audioFile = null;
        }

        public void Initialize()
        {
            if (outputDevice == null)
            {
                outputDevice = new WaveOutEvent();
                //outputDevice.PlaybackStopped += OnPlaybackStopped;
            }
        }

        public void Play(int channel, string path, PlaySoundMode mode)
        {
            Initialize();
            Stop(channel);

            repeate = mode.HasFlag(PlaySoundMode.Repeat);
            switch (Path.GetExtension(path).ToLower())
            {
                case ".mid":
                    if (CH_MIDI != channel)
                    {
                        throw new ArgumentException($"{channel} is not support MIDI. MIDI channel is {CH_MIDI}");
                    }
                    StartMidiPlayback(path);
                    break;
                default:
                    audioFile = new AudioFileReader(path);
                    outputDevice.Init(audioFile);
                    outputDevice.Play();
                    break;
            }
        }

        public void Stop(int channel)
        {
            if (CH_MIDI == channel)
            {
                ReleaseMidiPlayback();
            }
            outputDevice?.Stop();
        }

        private void StartMidiPlayback(string path)
        {
            midiFile = MidiFile.Read(path);
            try
            {
                midiOutput = OutputDevice.GetAll().First();
                midiPlayback = midiFile.GetPlayback(midiOutput);
                midiPlayback.Loop = repeate;
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
    }
}
