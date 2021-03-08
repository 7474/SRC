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
    public class NAudioPlayer : IPlaySound
    {
        // https://github.com/naudio/NAudio/blob/master/Docs/PlayAudioFileWinForms.md
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;


        public BGMStatus BGMStatus => throw new NotImplementedException();

        public void Dispose()
        {
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

            // XXX いろいろ
            outputDevice.Stop();
            switch (Path.GetExtension(path).ToLower())
            {
                case ".mid":
                    var access = MidiAccessManager.Default;
                    var output = access.OpenOutputAsync(access.Outputs.Last().Id).Result;
                    var music = MidiMusic.Read(File.OpenRead(path));
                    var player = new MidiPlayer(music, output);
                    player.EventReceived += (MidiEvent e) => {
                        if (e.EventType == MidiEvent.Program)
                            Console.WriteLine($"Program changed: Channel:{e.Channel} Instrument:{e.Msb}");
                    };
                    player.Play();
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
            outputDevice?.Stop();
        }


    }
}
