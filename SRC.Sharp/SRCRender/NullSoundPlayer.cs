using SRCCore;
using System.IO;

namespace SRCRender
{
    /// <summary>
    /// 音を鳴らさない IPlaySound。
    /// </summary>
    // TODO クロスプラットフォームの音声再生
    public class NullSoundPlayer : IPlaySound
    {
        public float SoundVolume { get; set; } = 1f;
        public BGMStatus BGMStatus => BGMStatus.Stopped;

        public void Play(int channel, Stream stream, SoundType soundType, PlaySoundMode mode)
        {
        }

        public void Stop(int channel)
        {
        }

        public SoundType ResolveSoundType(string path)
        {
            return SoundType.Unknown;
        }

        public void Dispose()
        {
        }
    }
}
