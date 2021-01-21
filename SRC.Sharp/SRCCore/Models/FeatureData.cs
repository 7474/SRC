// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRC.Core.Models
{
    // 特殊能力のクラス
    public class FeatureData
    {
        // 名称
        public string Name;
        // レベル
        // XXX 本当に double? decimal がいいのでは？
        public double Level;
        // データ
        public string StrData;
        // 必要技能
        public string NecessarySkill;
        // 必要条件
        public string NecessaryCondition;
    }
}