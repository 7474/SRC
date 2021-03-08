using NAudio.Wave;
using SRCCore;
using System;

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
            audioFile = new AudioFileReader(path);
            outputDevice.Init(audioFile);
            outputDevice.Play();
        }

        public void Stop(int channel)
        {
            outputDevice?.Stop();
        }
    }
}
