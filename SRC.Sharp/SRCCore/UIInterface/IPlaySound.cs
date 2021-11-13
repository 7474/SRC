// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Units;
using System;
using System.IO;

namespace SRCCore
{
    // 音声再生のインタフェース
    public interface IPlaySound : IDisposable
    {
        BGMStatus BGMStatus { get; }
        /// <summary>
        /// min: 0.0
        /// max: 1.0
        /// </summary>
        float SoundVolume { get; set; }

        void Play(int channel, Stream stream, SoundType soundType, PlaySoundMode mode);

        void Stop(int channel);

        SoundType ResolveSoundType(string path);
    }

    public static class PlaySoundConstants
    {
        public const int CH_BGM = 1;
    }

    public enum SoundType
    {
        Unknown,
        Midi,
        Wave,
        Mp3,
        Aiff,
    }

    public enum BGMStatus
    {
        Stopped,
        Playing,
    }

    public enum PlaySoundMode
    {
        None,
        Repeat,
    }
}
