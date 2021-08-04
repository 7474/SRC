// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.VB;

namespace SRCCore.Models
{
    // 武器データクラス
    public class WeaponData
    {
        // 武器名
        public string Name;
        // 攻撃力
        public int Power;
        // 最小射程
        public int MinRange;
        // 最大射程
        public int MaxRange;
        // 命中率
        public int Precision;
        // 弾数
        public int Bullet;
        // 消費ＥＮ
        public int ENConsumption;
        // 必要気力
        public int NecessaryMorale;
        // 地形適応
        public string Adaption;
        // ＣＴ率
        public int Critical;
        // 属性
        public string Class;
        // 必要技能
        public string NecessarySkill;
        // 必要条件
        public string NecessaryCondition;

        private SRC SRC;
        private Expressions.Expression Expression => SRC.Expression;

        public WeaponData(SRC src)
        {
            SRC = src;
        }

        // 武器愛称
        public string Nickname()
        {
            string NicknameRet = Name;
            Expression.ReplaceSubExpression(ref NicknameRet);
            if (Strings.InStr(NicknameRet, "(") > 0)
            {
                NicknameRet = Strings.Left(NicknameRet, Strings.InStr(NicknameRet, "(") - 1);
            }

            return NicknameRet;
        }

        // 使い捨てアイテムによる武器かどうかを返す
        public bool IsItem()
        {
            bool IsItemRet = default;
            var loopTo = GeneralLib.LLength(NecessarySkill);
            for (int i = 1; i <= loopTo; i++)
            {
                if (GeneralLib.LIndex(NecessarySkill, i) == "アイテム")
                {
                    IsItemRet = true;
                    return IsItemRet;
                }
            }

            return IsItemRet;
        }
    }
}
