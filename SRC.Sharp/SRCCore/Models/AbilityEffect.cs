
// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Models
{
    // アビリティの効果のクラス
    public class AbilityEffect
    {
        public string Name;
        public double Level;
        public string Data;

        // 効果の種類
        public string EffectType => Name;
    }
}