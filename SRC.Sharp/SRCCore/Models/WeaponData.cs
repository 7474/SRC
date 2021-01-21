// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRC.Core.Models
{
    // 武器データクラス
    public class WeaponData
    {
        // 武器名
        public string Name;
        // 攻撃力
        public int Power;
        // 最小射程
        public short MinRange;
        // 最大射程
        public short MaxRange;
        // 命中率
        public short Precision;
        // 弾数
        public short Bullet;
        // 消費ＥＮ
        public short ENConsumption;
        // 必要気力
        public short NecessaryMorale;
        // 地形適応
        public string Adaption;
        // ＣＴ率
        public short Critical;
        // 属性
        // UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public string Class_Renamed;
        // 必要技能
        public string NecessarySkill;
        // 必要条件
        public string NecessaryCondition;

        //// 武器愛称
        //public string Nickname()
        //{
        //    string NicknameRet = default;
        //    NicknameRet = Name;
        //    Expression.ReplaceSubExpression(ref NicknameRet);
        //    if (Strings.InStr(NicknameRet, "(") > 0)
        //    {
        //        NicknameRet = Strings.Left(NicknameRet, Strings.InStr(NicknameRet, "(") - 1);
        //    }

        //    return NicknameRet;
        //}

        //// 使い捨てアイテムによる武器かどうかを返す
        //public bool IsItem()
        //{
        //    bool IsItemRet = default;
        //    short i;
        //    var loopTo = GeneralLib.LLength(ref NecessarySkill);
        //    for (i = 1; i <= loopTo; i++)
        //    {
        //        if (GeneralLib.LIndex(ref NecessarySkill, i) == "アイテム")
        //        {
        //            IsItemRet = true;
        //            return IsItemRet;
        //        }
        //    }

        //    return IsItemRet;
        //}
    }
}