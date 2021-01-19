
namespace Project1
{
    static class BCVariable
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // バトルコンフィグデータが有効か？
        public static bool IsConfig;

        // バトルコンフィグデータの各種変数を定義する

        // バトルコンフィグデータ対象中心ユニット定義
        // ---------かならず定義されるデータ
        public static Unit MeUnit;

        // 攻撃側ユニット定義
        public static Unit AtkUnit;

        // 防御側ユニット定義
        public static Unit DefUnit;

        // 武器番号
        public static short WeaponNumber;

        // ---------定義されない場合もある(計算後にリセットされる)データ
        // 攻撃値
        public static int AttackExp;

        // 攻撃側定義変数
        public static int AttackVariable;

        // 防御側定義変数
        public static int DffenceVariable;

        // 地形補正
        public static double TerrainAdaption;

        // サイズ補正
        public static double SizeMod;

        // 最終値
        public static int LastVariable;

        // 武器攻撃力
        public static int WeaponPower;

        // 装甲値
        public static int Armor;

        // ザコ補正
        public static int CommonEnemy;

        // 定義されないこともあるデータをここでリセットする
        public static void DataReset()
        {
            AttackExp = 0;
            AttackVariable = 0;
            DffenceVariable = 0;
            TerrainAdaption = 1d;
            SizeMod = 1d;
            LastVariable = 0;
            WeaponPower = 0;
            CommonEnemy = 0;
        }
    }
}