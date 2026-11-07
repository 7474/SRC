// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Models
{
    // 特殊状態のクラス
    public class Condition
    {
        // 名称
        public string Name;
        // 有効期間。負数は無期限（VB の Unit.AddCondition で Lifetime = -1 にする）
        public int Lifetime;
        // レベル
        public double Level;
        // データ
        public string StrData;

        public bool IsEnable => Lifetime != 0;
    }
}
