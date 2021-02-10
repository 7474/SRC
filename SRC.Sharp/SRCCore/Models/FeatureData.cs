// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore.Models
{
    // 特殊能力のクラス
    public class FeatureData : ILevelElement
    {
        // 名称
        public string Name { get; set; }
        // レベル (レベル指定のない能力の場合はDEFAULT_LEVEL)
        public double Level { get; set; }
        // データ
        public string StrData { get; set; }
        // 必要技能
        public string NecessarySkill;
        // 必要条件
        public string NecessaryCondition;

        public bool HasLevel => Level != Constants.DEFAULT_LEVEL;
    }
}