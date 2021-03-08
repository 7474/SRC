using NAudio.Midi;
using NAudio.Wave;
using SRCCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SRCTestForm.Resoruces
{
    public class NAudioPlayer : IPlaySound
    {
        // https://github.com/naudio/NAudio/blob/master/Docs/PlayAudioFileWinForms.md
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        private MidiOut midiOut;
        private MidiFile midiFile;

        public BGMStatus BGMStatus => throw new NotImplementedException();

        public void Dispose()
        {
            midiOut?.Dispose();
            outputDevice?.Dispose();
            outputDevice = null;
            audioFile?.Dispose();
            audioFile = null;
        }

        public void Initialize()
        {
            if (midiOut == null)
            {
                // XXX 選択
                midiOut = new MidiOut(1);
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

            // XXX いろいろ
            outputDevice.Stop();
            switch (Path.GetExtension(path).ToLower())
            {
                case ".mid":
                    midiFile = new MidiFile(path, false);
                    var task = Task.Run(async () =>
                    {
                        var sw = new Stopwatch();
                        sw.Start();
                        var startMillis = midiFile.Events.StartAbsoluteTime;
                        foreach (var melist in midiFile.Events)
                        {
                            foreach (var me in melist)
                            {
                                var eventTime = me.AbsoluteTime - startMillis;
                                if (eventTime - sw.ElapsedMilliseconds > 0)
                                {
                                    await Task.Delay((int)Math.Max(0, eventTime - sw.ElapsedMilliseconds));
                                }
                                midiOut.Send(me.GetAsShortMessage());
                            }
                        }
                    });
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
