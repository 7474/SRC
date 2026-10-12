using SRCCore;
using System.IO;

namespace SRCRender
{
    /// <summary>
    /// 音を鳴らさない IPlaySound（テスト・音声出力を使わない場合）。音を鳴らすには SRCSound.OpenALSoundPlayer を使う。
    /// </summary>
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
