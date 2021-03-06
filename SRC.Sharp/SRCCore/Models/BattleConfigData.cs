
namespace Project1
{
    internal class BattleConfigData
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // バトルコンフィグデータのクラス
        // --- ダメージ計算、命中率算出など、バトルに関連するエリアスを定義します。

        // 名称
        public string Name;

        // 計算式
        public string ConfigCalc;

        // クラスの初期化
        // UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Initialize_Renamed()
        {
            ConfigCalc = "";
        }

        public BattleConfigData() : base()
        {
            Class_Initialize_Renamed();
        }

        // バトルコンフィグデータに基づいた置換＆計算の実行
        // 実行前に使う可能性のある変数を事前に代入しておくこと
        public double Calculate()
        {
            double CalculateRet = default;
            string expr;
            int morales;
            expr = ConfigCalc;

            // コンフィグ変数を有効にする
            BCVariable.IsConfig = true;

            // 式を評価する
            CalculateRet = Expression.GetValueAsDouble(ref expr);

            // コンフィグ変数を無効にする
            BCVariable.IsConfig = false;
            return CalculateRet;
        }
    }
}