// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.VB;
using System.Collections.Generic;

namespace SRCCore.Models
{
    // バトルコンフィグデータのクラス
    public class BattleConfigData
    {
        // --- ダメージ計算、命中率算出など、バトルに関連するエリアスを定義します。
        // 名称
        public string Name;

        // 計算式
        public string ConfigCalc;

        private SRC SRC { get; }
        public BattleConfigData(SRC src)
        {
            SRC = src;
        }

        // バトルコンフィグデータに基づいた置換＆計算の実行
        // 実行前に使う可能性のある変数を事前に代入しておくこと
        public double Calculate()
        {
            // コンフィグ変数を有効にする
            SRC.Event.BCVariable.IsConfig = true;
            try
            {
                // 式を評価する
                return SRC.Expression.GetValueAsDouble(ConfigCalc);
            }
            finally
            {
                // コンフィグ変数を無効にする
                SRC.Event.BCVariable.IsConfig = false;
            }
        }
    }
}