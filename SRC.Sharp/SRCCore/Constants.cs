// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

namespace SRCCore
{
    public static class Constants
    {
        // データ中にレベル指定を省略した場合のデフォルトのレベル値
        public const int DEFAULT_LEVEL = -1000;
        public const string vbCr = "\r";
        public const string vbLf = "\n";
        public const string vbTab = "\t";

        public static readonly string[] DIRECTIONS = new string[] { "N", "S", "W", "E" };
    }
}
