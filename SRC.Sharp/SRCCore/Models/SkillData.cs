// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRC.Core.Models
{
    // パイロット用特殊能力のクラス
    public class SkillData
    {
        // 名称
        public string Name;
        // レベル (レベル指定のない能力の場合はDEFAULT_LEVEL)
        public double Level;
        // データ
        public string StrData;
        // 習得レベル
        public int NecessaryLevel;
    }
}