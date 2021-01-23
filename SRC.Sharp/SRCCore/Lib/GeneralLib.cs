using System;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    static class GeneralLib
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 汎用的な処理を行うモジュール

        // iniファイルの読み出し
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        [DllImport("kernel32", EntryPoint = "GetPrivateProfileStringA")]
        static extern int GetPrivateProfileString(string lpApplicationName, Any lpKeyName, string lpDefault, string lpReturnedString, int nSize, string lpFileName);

        // iniファイルへの書き込み
        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        [DllImport("kernel32", EntryPoint = "WritePrivateProfileStringA")]
        static extern int WritePrivateProfileString(string lpApplicationName, Any lpKeyName, string lpString, string lpFileName);

        // Windowsが起動してからの時間を返す(ミリ秒)
        [DllImport("winmm.dll")]
        static extern int timeGetTime();

        // 時間処理の解像度を変更する
        [DllImport("winmm.dll")]
        static extern int timeBeginPeriod(int uPeriod);
        [DllImport("winmm.dll")]
        static extern int timeEndPeriod(int uPeriod);

        // ファイル属性を返す
        [DllImport("kernel32", EntryPoint = "GetFileAttributesA")]
        static extern int GetFileAttributes(string lpFileName);

        // OSのバージョン情報を返す
        public struct OSVERSIONINFO
        {
            public int dwOSVersionInfoSize;
            public int dwMajorVersion;
            public int dwMinorVersion;
            public int dwBuildNumber;
            public int dwPlatformId;
            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
            [VBFixedString(128)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
            public char[] szCSDVersion;
        }

        // UPGRADE_WARNING: 構造体 OSVERSIONINFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        [DllImport("kernel32", EntryPoint = "GetVersionExA")]
        static extern int GetVersionEx(ref OSVERSIONINFO lpVersionInformation);

        private const short VER_PLATFORM_WIN32_NT = 2;


        // 乱数発生用シード値
        public static int RndSeed;

        // 乱数系列
        private static float[] RndHistory = new float[4097];

        // 乱数系列の中で現在使用している値のインデックス
        public static short RndIndex;

        // 乱数系列のリセット
        public static void RndReset()
        {
            short i;
            VBMath.Randomize(RndSeed);

            // 乱数系列のセーブ＆ロードが出来るよう乱数系列をあらかじめ
            // 配列に保存して確定させる
            var loopTo = (short)Information.UBound(RndHistory);
            for (i = 1; i <= loopTo; i++)
                RndHistory[i] = VBMath.Rnd();
            RndIndex = 0;
        }

        // 1～max の乱数を返す
        public static int Dice(int max)
        {
            int DiceRet = default;
            if (max <= 1)
            {
                DiceRet = max;
                return DiceRet;
            }

            string argoname = "乱数系列非保存";
            if (Expression.IsOptionDefined(ref argoname))
            {
                DiceRet = (int)Conversion.Int(max * VBMath.Rnd() + 1f);
                return DiceRet;
            }

            RndIndex = (short)(RndIndex + 1);
            if (RndIndex > Information.UBound(RndHistory))
            {
                RndIndex = 1;
            }

            DiceRet = (int)Conversion.Int(max * RndHistory[RndIndex] + 1f);
            return DiceRet;
        }


        // リスト list から idx 番目の要素を返す
        public static string LIndex(ref string list, short idx)
        {
            string LIndexRet = default;
            short i, n;
            short list_len;
            short begin;

            // idxが正の数でなければ空文字列を返す
            if (idx < 1)
            {
                return LIndexRet;
            }

            list_len = (short)Strings.Len(list);

            // idx番目の要素まで読み飛ばす
            n = 0;
            i = 0;
            while (true)
            {
                // 空白を読み飛ばす
                do
                {
                    i = (short)(i + 1);
                    if (i > list_len)
                    {
                        return LIndexRet;
                    }
                }
                while (Strings.Mid(list, i, 1) == " ");

                // 要素数を１つ増やす
                n = (short)(n + 1);

                // 求める要素？
                if (n == idx)
                {
                    break;
                }

                // 要素を読み飛ばす
                do
                {
                    i = (short)(i + 1);
                    if (i > list_len)
                    {
                        return LIndexRet;
                    }
                }
                while (Strings.Mid(list, i, 1) != " ");
            }

            // 求める要素を読み込む
            begin = i;
            do
            {
                i = (short)(i + 1);
                if (i > list_len)
                {
                    LIndexRet = Strings.Mid(list, begin);
                    return LIndexRet;
                }
            }
            while (Strings.Mid(list, i, 1) != " ");
            LIndexRet = Strings.Mid(list, begin, i - begin);
            return LIndexRet;
        }

        // リスト list の要素数を返す
        public static short LLength(ref string list)
        {
            short LLengthRet = default;
            short i;
            short list_len;
            LLengthRet = 0;
            list_len = (short)Strings.Len(list);
            i = 0;
            while (true)
            {
                // 空白を読み飛ばす
                do
                {
                    i = (short)(i + 1);
                    if (i > list_len)
                    {
                        return LLengthRet;
                    }
                }
                while (Strings.Mid(list, i, 1) == " ");

                // 要素数を１つ増やす
                LLengthRet = (short)(LLengthRet + 1);

                // 要素を読み飛ばす
                do
                {
                    i = (short)(i + 1);
                    if (i > list_len)
                    {
                        return LLengthRet;
                    }
                }
                while (Strings.Mid(list, i, 1) != " ");
            }
        }

        // リスト list から、リストの要素の配列 larray を作成し、
        // リストの要素数を返す
        public static short LSplit(ref string list, ref string[] larray)
        {
            short LSplitRet = default;
            short i;
            short list_len;
            short begin;
            LSplitRet = 0;
            list_len = (short)Strings.Len(list);
            larray = new string[1];
            i = 0;
            while (true)
            {
                // 空白を読み飛ばす
                do
                {
                    i = (short)(i + 1);
                    if (i > list_len)
                    {
                        return LSplitRet;
                    }
                }
                while (Strings.Mid(list, i, 1) == " ");

                // 要素数を１つ増やす
                LSplitRet = (short)(LSplitRet + 1);

                // 要素を読み込む
                Array.Resize(ref larray, LSplitRet + 1);
                begin = i;
                do
                {
                    i = (short)(i + 1);
                    if (i > list_len)
                    {
                        larray[LSplitRet] = Strings.Mid(list, begin);
                        return LSplitRet;
                    }
                }
                while (Strings.Mid(list, i, 1) != " ");
                larray[LSplitRet] = Strings.Mid(list, begin, i - begin);
            }
        }

        // 文字列 ch が空白かどうか調べる
        public static bool IsSpace(ref string ch)
        {
            bool IsSpaceRet = default;
            if (Strings.Len(ch) == 0)
            {
                IsSpaceRet = true;
                return IsSpaceRet;
            }

            switch (Strings.Asc(ch))
            {
                case 9:
                case 13:
                case 32:
                case 160:
                    {
                        IsSpaceRet = true;
                        break;
                    }
            }

            return IsSpaceRet;
        }

        // リスト list に要素 str を追加
        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public static void LAppend(ref string list, ref string str_Renamed)
        {
            list = Strings.Trim(list);
            str_Renamed = Strings.Trim(str_Renamed);
            if (!string.IsNullOrEmpty(list))
            {
                if (!string.IsNullOrEmpty(str_Renamed))
                {
                    list = list + " " + str_Renamed;
                }
            }
            else if (!string.IsNullOrEmpty(str_Renamed))
            {
                list = str_Renamed;
            }
        }

        // リスト list に str が登場する位置を返す
        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public static short SearchList(ref string list, ref string str_Renamed)
        {
            short SearchListRet = default;
            short i;
            var loopTo = LLength(ref list);
            for (i = 1; i <= loopTo; i++)
            {
                if ((LIndex(ref list, i) ?? "") == (str_Renamed ?? ""))
                {
                    SearchListRet = i;
                    return SearchListRet;
                }
            }

            SearchListRet = 0;
            return SearchListRet;
        }


        // リスト list から idx 番目の要素を返す (括弧を考慮)
        public static string ListIndex(ref string list, short idx)
        {
            string ListIndexRet = default;
            short n, i, ch;
            short paren = default, list_len, begin;
            bool in_single_quote = default, in_double_quote = default;

            // idxが正の数でなければ空文字列を返す
            if (idx < 1)
            {
                return ListIndexRet;
            }

            list_len = (short)Strings.Len(list);

            // idx番目の要素まで読み飛ばす
            n = 0;
            i = 0;
            while (true)
            {
                // 空白を読み飛ばす
                while (true)
                {
                    i = (short)(i + 1);

                    // 文字列の終り？
                    if (i > list_len)
                    {
                        return ListIndexRet;
                    }

                    // 次の文字
                    ch = (short)Strings.Asc(Strings.Mid(list, i, 1));

                    // 空白でない？
                    switch (ch)
                    {
                        // スキップ
                        case 9:
                        case 32:
                            {
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }

                // 要素数を１つ増やす
                n = (short)(n + 1);

                // 求める要素？
                if (n == idx)
                {
                    break;
                }

                // 要素を読み飛ばす
                while (true)
                {
                    if (in_single_quote)
                    {
                        if (ch == 96) // "`"
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (ch == 34) // """"
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (ch)
                        {
                            case 40:
                            case 91: // "(", "["
                                {
                                    paren = (short)(paren + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ")", "]"
                                {
                                    paren = (short)(paren - 1);
                                    if (paren < 0)
                                    {
                                        // 括弧の対応が取れていない
                                        return ListIndexRet;
                                    }

                                    break;
                                }

                            case 96: // "`"
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // """"
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }

                    i = (short)(i + 1);

                    // 文字列の終り？
                    if (i > list_len)
                    {
                        return ListIndexRet;
                    }

                    // 次の文字
                    ch = (short)Strings.Asc(Strings.Mid(list, i, 1));

                    // 要素の末尾か判定
                    if (!in_single_quote & !in_double_quote & paren == 0)
                    {
                        // 空白？
                        switch (ch)
                        {
                            case 9:
                            case 32:
                                {
                                    break;
                                }
                        }
                    }
                }
            }

            // 求める要素を読み込む
            begin = i;
            while (true)
            {
                if (in_single_quote)
                {
                    if (ch == 96) // "`"
                    {
                        in_single_quote = false;
                    }
                }
                else if (in_double_quote)
                {
                    if (ch == 34) // """"
                    {
                        in_double_quote = false;
                    }
                }
                else
                {
                    switch (ch)
                    {
                        case 40:
                        case 91: // "(", "["
                            {
                                paren = (short)(paren + 1);
                                break;
                            }

                        case 41:
                        case 93: // ")", "]"
                            {
                                paren = (short)(paren - 1);
                                if (paren < 0)
                                {
                                    // 括弧の対応が取れていない
                                    return ListIndexRet;
                                }

                                break;
                            }

                        case 96: // "`"
                            {
                                in_single_quote = true;
                                break;
                            }

                        case 34: // """"
                            {
                                in_double_quote = true;
                                break;
                            }
                    }
                }

                i = (short)(i + 1);

                // 文字列の終り？
                if (i > list_len)
                {
                    ListIndexRet = Strings.Mid(list, begin);
                    return ListIndexRet;
                }

                // 次の文字
                ch = (short)Strings.Asc(Strings.Mid(list, i, 1));

                // 要素の末尾か判定
                if (!in_single_quote & !in_double_quote & paren == 0)
                {
                    // 空白？
                    switch (ch)
                    {
                        case 9:
                        case 32:
                            {
                                break;
                            }
                    }
                }
            }

            ListIndexRet = Strings.Mid(list, begin, i - begin);
            return ListIndexRet;
        }

        // リスト list の要素数を返す (括弧を考慮)
        public static short ListLength(ref string list)
        {
            short ListLengthRet = default;
            short i, ch;
            short list_len, paren = default;
            bool in_single_quote = default, in_double_quote = default;
            ListLengthRet = 0;
            list_len = (short)Strings.Len(list);
            i = 0;
            while (true)
            {
                // 空白を読み飛ばす
                while (true)
                {
                    i = (short)(i + 1);

                    // 文字列の終り？
                    if (i > list_len)
                    {
                        return ListLengthRet;
                    }

                    // 次の文字
                    ch = (short)Strings.Asc(Strings.Mid(list, i, 1));

                    // 空白でない？
                    switch (ch)
                    {
                        // スキップ
                        case 9:
                        case 32:
                            {
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }

                // 要素数を１つ増やす
                ListLengthRet = (short)(ListLengthRet + 1);

                // 要素を読み飛ばす
                while (true)
                {
                    if (in_single_quote)
                    {
                        if (ch == 96) // "`"
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (ch == 34) // """"
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (ch)
                        {
                            case 40:
                            case 91: // "(", "["
                                {
                                    paren = (short)(paren + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ")", "]"
                                {
                                    paren = (short)(paren - 1);
                                    if (paren < 0)
                                    {
                                        // 括弧の対応が取れていない
                                        ListLengthRet = -1;
                                        return ListLengthRet;
                                    }

                                    break;
                                }

                            case 96: // "`"
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // """"
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }

                    i = (short)(i + 1);

                    // 文字列の終り？
                    if (i > list_len)
                    {
                        // クォートや括弧の対応が取れている？
                        if (in_single_quote | in_double_quote | paren != 0)
                        {
                            ListLengthRet = -1;
                        }

                        return ListLengthRet;
                    }

                    // 次の文字
                    ch = (short)Strings.Asc(Strings.Mid(list, i, 1));

                    // 要素の末尾か判定
                    if (!in_single_quote & !in_double_quote & paren == 0)
                    {
                        // 空白？
                        switch (ch)
                        {
                            case 9:
                            case 32:
                                {
                                    break;
                                }
                        }
                    }
                }
            }
        }

        // リスト list から、リストの要素の配列 larray を作成し、
        // リストの要素数を返す (括弧を考慮)
        public static short ListSplit(ref string list, ref string[] larray)
        {
            short ListSplitRet = default;
            short n, i, ch;
            short paren = default, list_len, begin;
            bool in_single_quote = default, in_double_quote = default;
            ListSplitRet = -1;
            list_len = (short)Strings.Len(list);
            larray = new string[1];
            n = 0;
            i = 0;
            while (true)
            {
                // 空白を読み飛ばす
                while (true)
                {
                    i = (short)(i + 1);

                    // 文字列の終り？
                    if (i > list_len)
                    {
                        ListSplitRet = n;
                        return ListSplitRet;
                    }

                    // 次の文字
                    ch = (short)Strings.Asc(Strings.Mid(list, i, 1));

                    // 空白でない？
                    switch (ch)
                    {
                        // スキップ
                        case 9:
                        case 32:
                            {
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }

                // 要素数を１つ増やす
                n = (short)(n + 1);

                // 要素を読み込む
                Array.Resize(ref larray, n + 1);
                begin = i;
                while (true)
                {
                    if (in_single_quote)
                    {
                        if (ch == 96) // "`"
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (ch == 34) // """"
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (ch)
                        {
                            case 40:
                            case 91: // "(", "["
                                {
                                    paren = (short)(paren + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ")", "]"
                                {
                                    paren = (short)(paren - 1);
                                    if (paren < 0)
                                    {
                                        // 括弧の対応が取れていない
                                        larray[n] = Strings.Mid(list, begin);
                                        return ListSplitRet;
                                    }

                                    break;
                                }

                            case 96: // "`"
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // """"
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }

                    i = (short)(i + 1);

                    // 文字列の終り？
                    if (i > list_len)
                    {
                        larray[n] = Strings.Mid(list, begin);
                        // クォートや括弧の対応が取れている？
                        if (!in_single_quote & !in_double_quote & paren == 0)
                        {
                            ListSplitRet = n;
                        }

                        return ListSplitRet;
                    }

                    // 次の文字
                    ch = (short)Strings.Asc(Strings.Mid(list, i, 1));

                    // 要素の末尾か判定
                    if (!in_single_quote & !in_double_quote & paren == 0)
                    {
                        // 空白？
                        switch (ch)
                        {
                            case 9:
                            case 32:
                                {
                                    break;
                                }
                        }
                    }
                }

                larray[n] = Strings.Mid(list, begin, i - begin);
            }
        }

        // リスト list から idx 番目以降の全要素を返す (括弧を考慮)
        public static string ListTail(ref string list, short idx)
        {
            string ListTailRet = default;
            short n, i, ch;
            short list_len, paren = default;
            bool in_single_quote = default, in_double_quote = default;

            // idxが正の数でなければ空文字列を返す
            if (idx < 1)
            {
                return ListTailRet;
            }

            list_len = (short)Strings.Len(list);

            // idx番目の要素まで読み飛ばす
            n = 0;
            i = 0;
            while (true)
            {
                // 空白を読み飛ばす
                while (true)
                {
                    i = (short)(i + 1);

                    // 文字列の終り？
                    if (i > list_len)
                    {
                        return ListTailRet;
                    }

                    // 次の文字
                    ch = (short)Strings.Asc(Strings.Mid(list, i, 1));

                    // 空白でない？
                    switch (ch)
                    {
                        // スキップ
                        case 9:
                        case 32:
                            {
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }
                }

                // 要素数を１つ増やす
                n = (short)(n + 1);

                // 求める要素？
                if (n == idx)
                {
                    break;
                }

                // 要素を読み飛ばす
                while (true)
                {
                    if (in_single_quote)
                    {
                        if (ch == 96) // "`"
                        {
                            in_single_quote = false;
                        }
                    }
                    else if (in_double_quote)
                    {
                        if (ch == 34) // """"
                        {
                            in_double_quote = false;
                        }
                    }
                    else
                    {
                        switch (ch)
                        {
                            case 40:
                            case 91: // "(", "["
                                {
                                    paren = (short)(paren + 1);
                                    break;
                                }

                            case 41:
                            case 93: // ")", "]"
                                {
                                    paren = (short)(paren - 1);
                                    if (paren < 0)
                                    {
                                        // 括弧の対応が取れていない
                                        return ListTailRet;
                                    }

                                    break;
                                }

                            case 96: // "`"
                                {
                                    in_single_quote = true;
                                    break;
                                }

                            case 34: // """"
                                {
                                    in_double_quote = true;
                                    break;
                                }
                        }
                    }

                    i = (short)(i + 1);

                    // 文字列の終り？
                    if (i > list_len)
                    {
                        return ListTailRet;
                    }

                    // 次の文字
                    ch = (short)Strings.Asc(Strings.Mid(list, i, 1));

                    // 要素の末尾か判定
                    if (!in_single_quote & !in_double_quote & paren == 0)
                    {
                        // 空白？
                        switch (ch)
                        {
                            case 9:
                            case 32:
                                {
                                    break;
                                }
                        }
                    }
                }
            }

            ListTailRet = Strings.Mid(list, i);
            return ListTailRet;
        }


        // タブを考慮したTrim
        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public static void TrimString(ref string str_Renamed)
        {
            short j, i, lstr;
            lstr = (short)Strings.Len(str_Renamed);
            i = 1;
            j = lstr;

            // 先頭の空白を検索
            while (i <= j)
            {
                switch (Strings.Asc(Strings.Mid(str_Renamed, i)))
                {
                    case 9:
                    case 32:
                    case -32448:
                        {
                            i = (short)(i + 1);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }

            // 末尾の空白を検索
            while (i < j)
            {
                switch (Strings.Asc(Strings.Mid(str_Renamed, j)))
                {
                    case 9:
                    case 32:
                    case -32448:
                        {
                            j = (short)(j - 1);
                            break;
                        }

                    default:
                        {
                            break;
                        }
                }
            }

            // 空白があれば置き換え
            if (i != 1 | j != lstr)
            {
                str_Renamed = Strings.Mid(str_Renamed, i, j - i + 1);
            }
        }

        // 文字列 str 中に str2 が出現する位置を末尾から検索
        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public static short InStr2(ref string str_Renamed, ref string str2)
        {
            short InStr2Ret = default;
            short slen, i;
            slen = (short)Strings.Len(str2);
            i = (short)(Strings.Len(str_Renamed) - slen + 1);
            while (i > 0)
            {
                if ((Strings.Mid(str_Renamed, i, slen) ?? "") == (str2 ?? ""))
                {
                    InStr2Ret = i;
                    return InStr2Ret;
                }

                i = (short)(i - 1);
            }

            return InStr2Ret;
        }


        // 文字列をDoubleに変換
        public static double StrToDbl(ref string expr)
        {
            double StrToDblRet = default;
            if (Information.IsNumeric(expr))
            {
                StrToDblRet = Conversions.ToDouble(expr);
            }

            return StrToDblRet;
        }

        // 文字列をLongに変換
        public static int StrToLng(ref string expr)
        {
            int StrToLngRet = default;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 19518


            Input:
                    On Error GoTo ErrorHandler

             */
            if (Information.IsNumeric(expr))
            {
                StrToLngRet = Conversions.ToInteger(expr);
            }

            return StrToLngRet;
        ErrorHandler:
            ;
        }

        // 文字列をひらがなに変換
        // ひらがなへの変換は日本語以外のOSを使うとエラーが発生するようなので
        // エラーをトラップする必要がある
        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public static string StrToHiragana(ref string str_Renamed)
        {
            string StrToHiraganaRet = default;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 20019


            Input:
                    On Error GoTo ErrorHandler

             */
            StrToHiraganaRet = Strings.StrConv(str_Renamed, VbStrConv.Hiragana);
            return StrToHiraganaRet;
        ErrorHandler:
            ;
            StrToHiraganaRet = str_Renamed;
        }


        // aとbの最大値を返す
        public static int MaxLng(int a, int b)
        {
            int MaxLngRet = default;
            if (a > b)
            {
                MaxLngRet = a;
            }
            else
            {
                MaxLngRet = b;
            }

            return MaxLngRet;
        }

        // aとbの最小値を返す
        public static int MinLng(int a, int b)
        {
            int MinLngRet = default;
            if (a < b)
            {
                MinLngRet = a;
            }
            else
            {
                MinLngRet = b;
            }

            return MinLngRet;
        }

        // aとbの最大値を返す (Double)
        public static double MaxDbl(double a, double b)
        {
            double MaxDblRet = default;
            if (a > b)
            {
                MaxDblRet = a;
            }
            else
            {
                MaxDblRet = b;
            }

            return MaxDblRet;
        }

        // aとbの最小値を返す (Double)
        public static double MinDbl(double a, double b)
        {
            double MinDblRet = default;
            if (a < b)
            {
                MinDblRet = a;
            }
            else
            {
                MinDblRet = b;
            }

            return MinDblRet;
        }


        // 文字列 buf の長さが length になるように左側にスペースを付加する
        public static string LeftPaddedString(ref string buf, short length)
        {
            string LeftPaddedStringRet = default;
            // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
            LeftPaddedStringRet = Strings.Space(MaxLng(length - LenB(Strings.StrConv(buf, vbFromUnicode)), 0)) + buf;
            return LeftPaddedStringRet;
        }

        // 文字列 buf の長さが length になるように右側にスペースを付加する
        public static string RightPaddedString(ref string buf, short length)
        {
            string RightPaddedStringRet = default;
            // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
            RightPaddedStringRet = buf + Strings.Space(MaxLng(length - LenB(Strings.StrConv(buf, vbFromUnicode)), 0));
            return RightPaddedStringRet;
        }


        // Src.ini ファイルの ini_section から ini_entry の値を読み出す
        public static string ReadIni(ref string ini_section, ref string ini_entry)
        {
            string ReadIniRet = default;
            var s = new Microsoft.VisualBasic.Compatibility.VB6.FixedLengthString(1024);
            var ret = default(int);

            // シナリオ側に Src.ini ファイルがあればそちらを優先
            string argfname = SRC.ScenarioPath + "Src.ini";
            if (FileExists(ref argfname))
            {
                string arglpDefault = "";
                string arglpReturnedString = s.Value;
                string arglpFileName = SRC.ScenarioPath + "Src.ini";
                ret = GetPrivateProfileString(ref ini_section, ini_entry, ref arglpDefault, ref arglpReturnedString, 1024, ref arglpFileName);
                s.Value = arglpReturnedString;
            }

            // シナリオ側 Src.ini にエントリが無ければ本体側から読み出し
            if (ret == 0)
            {
                string arglpDefault1 = "";
                string arglpReturnedString1 = s.Value;
                string arglpFileName1 = SRC.AppPath + "Src.ini";
                ret = GetPrivateProfileString(ref ini_section, ini_entry, ref arglpDefault1, ref arglpReturnedString1, 1024, ref arglpFileName1);
                s.Value = arglpReturnedString1;
            }

            // 不要部分を削除
            ReadIniRet = Strings.Left(s.Value, Strings.InStr(s.Value, Constants.vbNullChar) - 1);
            return ReadIniRet;
        }


        // Src.ini ファイルの ini_section の ini_entry に値 ini_data を書き込む
        public static void WriteIni(ref string ini_section, ref string ini_entry, ref string ini_data)
        {
            var s = new Microsoft.VisualBasic.Compatibility.VB6.FixedLengthString(1024);
            var ret = default(int);

            // LastFolderの設定のみは必ず本体側の Src.ini に書き込む
            if (ini_entry == "LastFolder")
            {
                string arglpFileName = SRC.AppPath + "Src.ini";
                ret = WritePrivateProfileString(ref ini_section, ini_entry, ref ini_data, ref arglpFileName);
                return;
            }

            // シナリオ側に Src.ini ファイルがあればそちらを優先
            bool localFileExists() { string argfname = SRC.ScenarioPath + "Src.ini"; var ret = FileExists(ref argfname); return ret; }

            if (Strings.Len(SRC.ScenarioPath) > 0 & localFileExists())
            {
                // エントリが存在するかチェック
                string arglpDefault = "";
                string arglpReturnedString = s.Value;
                string arglpFileName1 = SRC.ScenarioPath + "Src.ini";
                ret = GetPrivateProfileString(ref ini_section, ini_entry, ref arglpDefault, ref arglpReturnedString, 1024, ref arglpFileName1);
                s.Value = arglpReturnedString;
                if (ret > 1)
                {
                    string arglpFileName2 = SRC.ScenarioPath + "Src.ini";
                    ret = WritePrivateProfileString(ref ini_section, ini_entry, ref ini_data, ref arglpFileName2);
                }
            }

            // シナリオ側 Src.ini にエントリが無ければ本体側から読み出し
            if (ret == 0)
            {
                string arglpFileName3 = SRC.AppPath + "Src.ini";
                ret = WritePrivateProfileString(ref ini_section, ini_entry, ref ini_data, ref arglpFileName3);
            }
        }


        // 文字列 s1 中の s2 を s3 に置換
        // 置換を行ったときにはTrueを返す
        public static bool ReplaceString(ref string s1, ref string s2, ref string s3)
        {
            bool ReplaceStringRet = default;
            string buf;
            short len2, len3;
            short idx;
            idx = (short)Strings.InStr(s1, s2);

            // 置換が必要？
            if (idx == 0)
            {
                return ReplaceStringRet;
            }

            len2 = (short)Strings.Len(s2);
            len3 = (short)Strings.Len(s3);

            // &は遅いので出来るだけMidを使う
            if (len2 == len3)
            {
                do
                {
                    StringType.MidStmtStr(ref s1, idx, len2, s3);
                    idx = (short)Strings.InStr(s1, s2);
                }
                while (idx > 0);
            }
            else
            {
                buf = s1;
                s1 = "";
                do
                {
                    s1 = s1 + Strings.Left(buf, idx - 1) + s3;
                    buf = Strings.Mid(buf, idx + len2);
                    idx = (short)Strings.InStr(buf, s2);
                }
                while (idx > 0);
                s1 = s1 + buf;
            }

            ReplaceStringRet = true;
            return ReplaceStringRet;
        }


        // ファイル fname が存在するか判定
        public static bool FileExists(ref string fname)
        {
            bool FileExistsRet = default;
            if (GeneralLib.GetFileAttributes(ref fname) != -1)
            {
                FileExistsRet = true;
            }

            return FileExistsRet;
        }


        // データファイルfnumからデータを一行読み込み、line_bufに格納するとともに
        // 行番号line_numを更新する。
        // 行頭に「#」がある場合は行の読み飛ばしを行う。
        // 行中に「//」がある場合、そこからはコメントと見なして無視する。
        // 行末に「_」がある場合は行の結合を行う。
        public static void GetLine(ref short fnum, ref string line_buf, ref int line_num)
        {
            string buf;
            short idx;
            line_buf = "";
            while (!FileSystem.EOF(fnum))
            {
                line_num = line_num + 1;
                buf = FileSystem.LineInput(fnum);

                // コメント行はスキップ
                if (Strings.Left(buf, 1) == "#")
                {
                    goto NextLine;
                }

                // コメント部分を削除
                idx = (short)Strings.InStr(buf, "//");
                if (idx > 0)
                {
                    buf = Strings.Left(buf, idx - 1);
                }

                // 行末が「_」でなければ行の読み込みを完了
                if (Strings.Right(buf, 1) != "_")
                {
                    TrimString(ref buf);
                    line_buf = line_buf + buf;
                    break;
                }

                // 行末が「_」の場合は行を結合
                TrimString(ref buf);
                line_buf = line_buf + Strings.Left(buf, Strings.Len(buf) - 1);
            NextLine:
                ;
            }

            string args2 = "，";
            string args3 = ", ";
            ReplaceString(ref line_buf, ref args2, ref args3);
        }


        // Windowsのバージョンを判定する
        public static short GetWinVersion()
        {
            short GetWinVersionRet = default;
            var vinfo = default(OSVERSIONINFO);
            int ret;
            {
                var withBlock = vinfo;
                // dwOSVersionInfoSizeに構造体のサイズをセットする。
                withBlock.dwOSVersionInfoSize = Strings.Len(vinfo);

                // OSのバージョン情報を得る。
                ret = GetVersionEx(ref vinfo);
                if (ret == 0)
                {
                    return GetWinVersionRet;
                }

                GetWinVersionRet = (short)(withBlock.dwMajorVersion * 100 + withBlock.dwMinorVersion);
            }

            return GetWinVersionRet;
        }


        // 数値を指数表記を使わずに文字列表記する
        public static string FormatNum(double n)
        {
            string FormatNumRet = default;
            if (n == Conversion.Int(n))
            {
                FormatNumRet = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(n, "0");
            }
            else
            {
                FormatNumRet = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(n, "0.#######################################################################");
            }

            return FormatNumRet;
        }


        // 文字列 str が数値かどうか調べる
        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public static bool IsNumber(ref string str_Renamed)
        {
            bool IsNumberRet = default;
            if (!Information.IsNumeric(str_Renamed))
            {
                return IsNumberRet;
            }

            // "(1)"のような文字列が数値と判定されてしまうのを防ぐ
            if (Strings.Asc(str_Renamed) == 40)
            {
                return IsNumberRet;
            }

            IsNumberRet = true;
            return IsNumberRet;
        }


        // 武器属性処理用の関数群。

        // 属性を一つ取得する。複数文字の属性もひとつとして取得する。
        // ただしＭは防御特性において単体文字として扱われるために除く。
        // 検索文字列 aname
        // 検索位置 idx (検索終了位置を返す)
        // 取得文字長 length (特殊効果数カウント用。基本的に0(属性取得)か1(属性頭文字取得))
        public static string GetClassBundle(ref string aname, ref short idx, short length = 0)
        {
            string GetClassBundleRet = default;
            short i;
            string ch;
            i = idx;
            ch = Strings.Mid(aname, i, 1);
            // 弱、効、剋があればその次の文字まで一緒に取得する。
            // 入れ子可能なため弱、効、剋が続く限りループ
            while (ch == "弱" | ch == "効" | ch == "剋")
            {
                // 属性指定の最後の文字が弱効剋だった場合、属性なし
                if (i >= Strings.Len(aname))
                    goto NotFoundClass;
                i = (short)(i + 1);
                ch = Strings.Mid(aname, i, 1);
            }
            // 低があればその次の文字まで一緒に取得する。
            if (ch == "低")
            {
                i = (short)(i + 1);
                // midの開始位置指定は文字数を超えていても大丈夫なはずですが念の為
                if (i > Strings.Len(aname))
                {
                    goto NotFoundClass;
                }

                ch = Strings.Mid(aname, i, 1);
                if (ch != "攻" & ch != "防" & ch != "運" & ch != "移")
                {
                    goto NotFoundClass;
                }
            }

            if (length == 0)
            {
                GetClassBundleRet = Strings.Mid(aname, idx, i - idx + 1);
            }
            else
            {
                GetClassBundleRet = Strings.Mid(aname, idx, length);
            }

        NotFoundClass:
            ;
            idx = i;
            return GetClassBundleRet;
        }

        // InStrと同じ動作。ただし見つかった文字の前に「弱」「効」「剋」があった場合、別属性と判定する)
        public static short InStrNotNest(ref string string1, ref string string2, short start = 1)
        {
            short InStrNotNestRet = default;
            short i;
            string c;
            i = (short)Strings.InStr(start, string1, string2);
            // 先頭一致か、一致なしのとき、ここで取得
            if (i <= 1)
            {
                InStrNotNestRet = i;
            }
            else
            {
                while (i > 0)
                {
                    c = Strings.Mid(string1, i - 1, 1);
                    // 検知した文字の前の文字が弱効剋でなかったら、InStrの結果を返す
                    if (c != "弱" & c != "効" & c != "剋")
                    {
                        break;
                    }
                    // 検知した文字の前の文字が弱効剋だったら、再度文字列を探索する
                    if (i < Strings.Len(string1))
                    {
                        i = (short)Strings.InStr(i + 1, string1, string2);
                    }
                    else
                    {
                        i = 0;
                    }
                }

                InStrNotNestRet = i;
            }

            return InStrNotNestRet;
        }
    }
}