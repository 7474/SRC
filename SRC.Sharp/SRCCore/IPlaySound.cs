// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Units;
using System;

namespace SRCCore
{
    // 音声再生のインタフェース
    public interface IPlaySound : IDisposable
    {
        public const int CH_BGM = 1;

        BGMStatus BGMStatus { get; }
        /// <summary>
        /// min: 0.0
        /// max: 1.0
        /// </summary>
        float SoundVolume { get; set; }

        // XXX 今呼んでない。
        void Initialize();

        void Play(int channel, string path, PlaySoundMode mode);

        void Stop(int channel);
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
