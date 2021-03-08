using Commons.Music.Midi;
using Commons.Music.Midi.Alsa;
using Commons.Music.Midi.CoreMidiApi;
using Commons.Music.Midi.PortMidi;
using Commons.Music.Midi.RtMidi;
using Commons.Music.Midi.WinMM;
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
                // Windows 10 で試した時の Default は WinMMMidiAccess だった。
                var access = MidiAccessManager.Default;
                var midiPort = access.Outputs.Last();
                try
                {
                    var rtAccess = new RtMidiAccess();
                    midiPort = rtAccess.Outputs.Last();
                    access = rtAccess;
                }
                catch
                {
                    // ignore
                }
                try
                {
                    var portAccess = new PortMidiAccess();
                    midiPort = portAccess.Outputs.Last();
                    access = portAccess;
                }
                catch
                {
                    // ignore
                }
                try
                {
                    var mmAccess = new WinMMMidiAccess();
                    midiPort = mmAccess.Outputs.Last();
                    access = mmAccess;
                }
                catch
                {
                    // ignore
                }
                try
                {
                    var coreAccess = new CoreMidiAccess();
                    midiPort = coreAccess.Outputs.Last();
                    access = coreAccess;
                }
                catch
                {
                    // ignore
                }
                try
                {
                    var alsaAccess = new AlsaMidiAccess();
                    midiPort = alsaAccess.Outputs.Last();
                    access = alsaAccess;
                }
                catch
                {
                    // ignore
                }
                midiOutput = access.OpenOutputAsync(midiPort.Id).Result;
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
