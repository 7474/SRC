using NAudio.Wave;
using SRCCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Commons.Music.Midi;
using System.Linq;

namespace SRCTestForm.Resoruces
{
    public class WindowsManagedPlayer : IPlaySound
    {
        // https://github.com/naudio/NAudio/blob/master/Docs/PlayAudioFileWinForms.md
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        // https://github.com/atsushieno/managed-midi/
        private IMidiOutput midiOutput;
        private MidiPlayer midiPlayer;

        private bool repeate;

        public const int CH_MIDI = 1;
        public BGMStatus BGMStatus => throw new NotImplementedException();

        public void Dispose()
        {
            midiPlayer?.Dispose();
            midiOutput?.Dispose();

            outputDevice?.Dispose();
            outputDevice = null;
            audioFile?.Dispose();
            audioFile = null;
        }

        public void Initialize()
        {
            if (midiOutput == null)
            {
                var access = MidiAccessManager.Default;
                midiOutput = access.OpenOutputAsync(access.Outputs.Last().Id).Result;
            }
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
                    var music = MidiMusic.Read(File.OpenRead(path));
                    midiPlayer = new MidiPlayer(music, midiOutput);
                    midiPlayer.Finished += MidiFinished;
                    midiPlayer.Play();
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
            ReleaseMidiPlayer(channel);
            outputDevice?.Stop();
        }

        private void ReleaseMidiPlayer(int channel)
        {
            if (CH_MIDI == channel)
            {
                if (midiPlayer != null)
                {
                    midiPlayer.Finished -= MidiFinished;
                    midiPlayer.Dispose();
                    midiPlayer = null;
                }
            }
        }

        private void MidiFinished()
        {
            if (repeate)
            {
                midiPlayer?.Play();
            }
        }
    }
}
