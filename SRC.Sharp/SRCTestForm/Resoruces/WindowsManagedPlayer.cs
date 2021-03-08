using Commons.Music.Midi;
using NAudio.Wave;
using SRCCore;
using System;
using System.IO;
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
            midiPlayer = null;
            midiOutput?.Dispose();
            midiOutput = null;

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
                    OpenMidiOutput();
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
            if (CH_MIDI == channel)
            {
                ReleaseMidiPlayer();
            }
            outputDevice?.Stop();
        }

        private void OpenMidiOutput()
        {
            // Windows 10 で試した時の Default は WinMMMidiAccess だった。
            var access = MidiAccessManager.Default;
            var midiPort = access.Outputs.Last();
            midiOutput = access.OpenOutputAsync(midiPort.Id).Result;
        }

        private void ReleaseMidiPlayer()
        {
            if (midiPlayer != null)
            {
                midiPlayer.Finished -= MidiFinished;
                midiPlayer.Dispose();
                midiPlayer = null;
                // XXX コントロールをリセットしたい
                midiOutput?.Dispose();
                midiOutput = null;
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
