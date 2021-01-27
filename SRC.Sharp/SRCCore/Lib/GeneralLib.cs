﻿// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRC.Core.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SRC.Core.Lib
{
    // 汎用的な処理を行うモジュール
    public static class GeneralLib
    {
        //        // iniファイルの読み出し
        //        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        //        [DllImport("kernel32", EntryPoint = "GetPrivateProfileStringA")]
        //        static extern int GetPrivateProfileString(string lpApplicationName, Any lpKeyName, string lpDefault, string lpReturnedString, int nSize, string lpFileName);

        //        // iniファイルへの書き込み
        //        // UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
        //        [DllImport("kernel32", EntryPoint = "WritePrivateProfileStringA")]
        //        static extern int WritePrivateProfileString(string lpApplicationName, Any lpKeyName, string lpString, string lpFileName);

        //        // Windowsが起動してからの時間を返す(ミリ秒)
        //        [DllImport("winmm.dll")]
        //        static extern int timeGetTime();

        //        // 時間処理の解像度を変更する
        //        [DllImport("winmm.dll")]
        //        static extern int timeBeginPeriod(int uPeriod);
        //        [DllImport("winmm.dll")]
        //        static extern int timeEndPeriod(int uPeriod);

        //        // ファイル属性を返す
        //        [DllImport("kernel32", EntryPoint = "GetFileAttributesA")]
        //        static extern int GetFileAttributes(string lpFileName);

        //        // OSのバージョン情報を返す
        //        public struct OSVERSIONINFO
        //        {
        //            public int dwOSVersionInfoSize;
        //            public int dwMajorVersion;
        //            public int dwMinorVersion;
        //            public int dwBuildNumber;
        //            public int dwPlatformId;
        //            // UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
        //            [VBFixedString(128)]
        //            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        //            public char[] szCSDVersion;
        //        }

        //        // UPGRADE_WARNING: 構造体 OSVERSIONINFO に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
        //        [DllImport("kernel32", EntryPoint = "GetVersionExA")]
        //        static extern int GetVersionEx(ref OSVERSIONINFO lpVersionInformation);

        //        private const int VER_PLATFORM_WIN32_NT = 2;


        //        // 乱数発生用シード値
        //        public static int RndSeed;

        //        // 乱数系列
        //        private static float[] RndHistory = new float[4097];

        //        // 乱数系列の中で現在使用している値のインデックス
        //        public static int RndIndex;

        //        // 乱数系列のリセット
        //        public static void RndReset()
        //        {
        //            int i;
        //            VBMath.Randomize(RndSeed);

        //            // 乱数系列のセーブ＆ロードが出来るよう乱数系列をあらかじめ
        //            // 配列に保存して確定させる
        //            var loopTo = Information.UBound(RndHistory);
        //            for (i = 1; i <= loopTo; i++)
        //                RndHistory[i] = VBMath.Rnd();
        //            RndIndex = 0;
        //        }

        //        // 1～max の乱数を返す
        //        public static int Dice(int max)
        //        {
        //            int DiceRet = default;
        //            if (max <= 1)
        //            {
        //                DiceRet = max;
        //                return DiceRet;
        //            }

        //            string argoname = "乱数系列非保存";
        //            if (Expression.IsOptionDefined(ref argoname))
        //            {
        //                DiceRet = Conversion.Int(max * VBMath.Rnd() + 1f);
        //                return DiceRet;
        //            }

        //            RndIndex = (RndIndex + 1);
        //            if (RndIndex > Information.UBound(RndHistory))
        //            {
        //                RndIndex = 1;
        //            }

        //            DiceRet = Conversion.Int(max * RndHistory[RndIndex] + 1f);
        //            return DiceRet;
        //        }

        // リスト list から idx 番目の要素を返す
        public static string LIndex(string list, int idx)
        {
            string LIndexRet = default;
            int i, n;
            int list_len;
            int begin;

            // idxが正の数でなければ空文字列を返す
            if (idx < 1)
            {
                return LIndexRet;
            }

            list_len = Strings.Len(list);

            // idx番目の要素まで読み飛ばす
            n = 0;
            i = 0;
            while (true)
            {
                // 空白を読み飛ばす
                do
                {
                    i = (i + 1);
                    if (i > list_len)
                    {
                        return LIndexRet;
                    }
                }
                while (Strings.Mid(list, i, 1) == " ");

                // 要素数を１つ増やす
                n = (n + 1);

                // 求める要素？
                if (n == idx)
                {
                    break;
                }

                // 要素を読み飛ばす
                do
                {
                    i = (i + 1);
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
                i = (i + 1);
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
        public static int LLength(string list)
        {
            int LLengthRet = default;
            int i;
            int list_len;
            LLengthRet = 0;
            list_len = Strings.Len(list);
            i = 0;
            while (true)
            {
                // 空白を読み飛ばす
                do
                {
                    i = (i + 1);
                    if (i > list_len)
                    {
                        return LLengthRet;
                    }
                }
                while (Strings.Mid(list, i, 1) == " ");

                // 要素数を１つ増やす
                LLengthRet = (LLengthRet + 1);

                // 要素を読み飛ばす
                do
                {
                    i = (i + 1);
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
        public static int LSplit(string list, out string[] larray)
        {
            int LSplitRet = default;
            int i;
            int list_len;
            int begin;
            LSplitRet = 0;
            list_len = Strings.Len(list);
            larray = new string[1];
            i = 0;
            while (true)
            {
                // 空白を読み飛ばす
                do
                {
                    i = (i + 1);
                    if (i > list_len)
                    {
                        return LSplitRet;
                    }
                }
                while (Strings.Mid(list, i, 1) == " ");

                // 要素数を１つ増やす
                LSplitRet = (LSplitRet + 1);

                // 要素を読み込む
                Array.Resize(ref larray, LSplitRet + 1);
                begin = i;
                do
                {
                    i = (i + 1);
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

        //        // 文字列 ch が空白かどうか調べる
        //        public static bool IsSpace(ref string ch)
        //        {
        //            bool IsSpaceRet = default;
        //            if (Strings.Len(ch) == 0)
        //            {
        //                IsSpaceRet = true;
        //                return IsSpaceRet;
        //            }

        //            switch (Strings.Asc(ch))
        //            {
        //                case 9:
        //                case 13:
        //                case 32:
        //                case 160:
        //                    {
        //                        IsSpaceRet = true;
        //                        break;
        //                    }
        //            }

        //            return IsSpaceRet;
        //        }

        //        // リスト list に要素 str を追加
        //        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        //        public static void LAppend(ref string list, ref string str_Renamed)
        //        {
        //            list = Strings.Trim(list);
        //            str_Renamed = Strings.Trim(str_Renamed);
        //            if (!string.IsNullOrEmpty(list))
        //            {
        //                if (!string.IsNullOrEmpty(str_Renamed))
        //                {
        //                    list = list + " " + str_Renamed;
        //                }
        //            }
        //            else if (!string.IsNullOrEmpty(str_Renamed))
        //            {
        //                list = str_Renamed;
        //            }
        //        }

        //        // リスト list に str が登場する位置を返す
        //        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        //        public static int SearchList(ref string list, ref string str_Renamed)
        //        {
        //            int SearchListRet = default;
        //            int i;
        //            var loopTo = LLength(ref list);
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                if ((LIndex(ref list, i) ?? "") == (str_Renamed ?? ""))
        //                {
        //                    SearchListRet = i;
        //                    return SearchListRet;
        //                }
        //            }

        //            SearchListRet = 0;
        //            return SearchListRet;
        //        }

        // リスト list の要素を分割して返す (括弧を考慮)
        public static IList<string> ToList(string list)
        {
            bool in_single_quote = false;
            bool in_double_quote = false;
            int i = -1;
            int paren = 0;
            var current = new StringBuilder();
            var result = new List<string>();
            foreach (var c in list.ToCharArray())
            {
                i++;
                if (!in_single_quote & !in_double_quote & paren == 0)
                {
                    if (c == ' ' || c == '\t')
                    {
                        if (current.Length > 0)
                        {
                            result.Add(current.ToString());
                            current.Clear();
                        }
                        continue;
                    }
                }
                bool append = true;

                if (in_single_quote)
                {
                    if (c == '\'') // "`"
                    {
                        in_single_quote = false;
                    }
                }
                else if (in_double_quote)
                {
                    if (c == '"') // """"
                    {
                        in_double_quote = false;
                    }
                }
                else
                {
                    switch (c)
                    {
                        case '(':
                        case '[': // "(", "["
                            paren = (paren + 1);
                            if (paren == 1) { append = false; }
                            break;

                        case ')':
                        case ']': // ")", "]"
                            paren = (paren - 1);
                            if (paren < 0)
                            {
                                // 括弧の対応が取れていない
                                result.Add(current.ToString());
                                result.Add(new string(list.ToCharArray().Skip(i).ToArray()));
                                return result;
                            }
                            if (paren == 0) { append = false; }
                            break;

                        case '\'': // "`"
                            in_single_quote = true;
                            break;

                        case '"': // """"
                            in_double_quote = true;
                            break;
                    }
                }
                if (append)
                {
                    current.Append(c);
                }
            }
            if (current.Length > 0)
            {
                result.Add(current.ToString());
            }
            return result;
        }

        // リスト list から idx 番目の要素を返す (括弧を考慮)
        public static string ListIndex(string list, int idx)
        {
            // idxが正の数でなければ空文字列を返す
            if (idx < 1)
            {
                return "";
            }
            var listList = ToList(list);
            return listList.Count >= idx ? listList[idx - 1] : "";

        }

        //        // リスト list の要素数を返す (括弧を考慮)
        //        public static int ListLength(ref string list)
        //        {
        //            int ListLengthRet = default;
        //            int i, ch;
        //            int list_len, paren = default;
        //            bool in_single_quote = default, in_double_quote = default;
        //            ListLengthRet = 0;
        //            list_len = Strings.Len(list);
        //            i = 0;
        //            while (true)
        //            {
        //                // 空白を読み飛ばす
        //                while (true)
        //                {
        //                    i = (i + 1);

        //                    // 文字列の終り？
        //                    if (i > list_len)
        //                    {
        //                        return ListLengthRet;
        //                    }

        //                    // 次の文字
        //                    ch = Strings.Asc(Strings.Mid(list, i, 1));

        //                    // 空白でない？
        //                    switch (ch)
        //                    {
        //                        // スキップ
        //                        case 9:
        //                        case 32:
        //                            {
        //                                break;
        //                            }

        //                        default:
        //                            {
        //                                break;
        //                            }
        //                    }
        //                }

        //                // 要素数を１つ増やす
        //                ListLengthRet = (ListLengthRet + 1);

        //                // 要素を読み飛ばす
        //                while (true)
        //                {
        //                    if (in_single_quote)
        //                    {
        //                        if (ch == 96) // "`"
        //                        {
        //                            in_single_quote = false;
        //                        }
        //                    }
        //                    else if (in_double_quote)
        //                    {
        //                        if (ch == 34) // """"
        //                        {
        //                            in_double_quote = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        switch (ch)
        //                        {
        //                            case 40:
        //                            case 91: // "(", "["
        //                                {
        //                                    paren = (paren + 1);
        //                                    break;
        //                                }

        //                            case 41:
        //                            case 93: // ")", "]"
        //                                {
        //                                    paren = (paren - 1);
        //                                    if (paren < 0)
        //                                    {
        //                                        // 括弧の対応が取れていない
        //                                        ListLengthRet = -1;
        //                                        return ListLengthRet;
        //                                    }

        //                                    break;
        //                                }

        //                            case 96: // "`"
        //                                {
        //                                    in_single_quote = true;
        //                                    break;
        //                                }

        //                            case 34: // """"
        //                                {
        //                                    in_double_quote = true;
        //                                    break;
        //                                }
        //                        }
        //                    }

        //                    i = (i + 1);

        //                    // 文字列の終り？
        //                    if (i > list_len)
        //                    {
        //                        // クォートや括弧の対応が取れている？
        //                        if (in_single_quote | in_double_quote | paren != 0)
        //                        {
        //                            ListLengthRet = -1;
        //                        }

        //                        return ListLengthRet;
        //                    }

        //                    // 次の文字
        //                    ch = Strings.Asc(Strings.Mid(list, i, 1));

        //                    // 要素の末尾か判定
        //                    if (!in_single_quote & !in_double_quote & paren == 0)
        //                    {
        //                        // 空白？
        //                        switch (ch)
        //                        {
        //                            case 9:
        //                            case 32:
        //                                {
        //                                    break;
        //                                }
        //                        }
        //                    }
        //                }
        //            }
        //        }

        // リスト list から、リストの要素の配列 larray を作成し、
        // リストの要素数を返す (括弧を考慮)
        public static int ListSplit(string list, out string[] larray)
        {
            larray = ToList(list).ToArray();
            return larray.Length;
        }

        //        // リスト list から idx 番目以降の全要素を返す (括弧を考慮)
        //        public static string ListTail(ref string list, int idx)
        //        {
        //            string ListTailRet = default;
        //            int n, i, ch;
        //            int list_len, paren = default;
        //            bool in_single_quote = default, in_double_quote = default;

        //            // idxが正の数でなければ空文字列を返す
        //            if (idx < 1)
        //            {
        //                return ListTailRet;
        //            }

        //            list_len = Strings.Len(list);

        //            // idx番目の要素まで読み飛ばす
        //            n = 0;
        //            i = 0;
        //            while (true)
        //            {
        //                // 空白を読み飛ばす
        //                while (true)
        //                {
        //                    i = (i + 1);

        //                    // 文字列の終り？
        //                    if (i > list_len)
        //                    {
        //                        return ListTailRet;
        //                    }

        //                    // 次の文字
        //                    ch = Strings.Asc(Strings.Mid(list, i, 1));

        //                    // 空白でない？
        //                    switch (ch)
        //                    {
        //                        // スキップ
        //                        case 9:
        //                        case 32:
        //                            {
        //                                break;
        //                            }

        //                        default:
        //                            {
        //                                break;
        //                            }
        //                    }
        //                }

        //                // 要素数を１つ増やす
        //                n = (n + 1);

        //                // 求める要素？
        //                if (n == idx)
        //                {
        //                    break;
        //                }

        //                // 要素を読み飛ばす
        //                while (true)
        //                {
        //                    if (in_single_quote)
        //                    {
        //                        if (ch == 96) // "`"
        //                        {
        //                            in_single_quote = false;
        //                        }
        //                    }
        //                    else if (in_double_quote)
        //                    {
        //                        if (ch == 34) // """"
        //                        {
        //                            in_double_quote = false;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        switch (ch)
        //                        {
        //                            case 40:
        //                            case 91: // "(", "["
        //                                {
        //                                    paren = (paren + 1);
        //                                    break;
        //                                }

        //                            case 41:
        //                            case 93: // ")", "]"
        //                                {
        //                                    paren = (paren - 1);
        //                                    if (paren < 0)
        //                                    {
        //                                        // 括弧の対応が取れていない
        //                                        return ListTailRet;
        //                                    }

        //                                    break;
        //                                }

        //                            case 96: // "`"
        //                                {
        //                                    in_single_quote = true;
        //                                    break;
        //                                }

        //                            case 34: // """"
        //                                {
        //                                    in_double_quote = true;
        //                                    break;
        //                                }
        //                        }
        //                    }

        //                    i = (i + 1);

        //                    // 文字列の終り？
        //                    if (i > list_len)
        //                    {
        //                        return ListTailRet;
        //                    }

        //                    // 次の文字
        //                    ch = Strings.Asc(Strings.Mid(list, i, 1));

        //                    // 要素の末尾か判定
        //                    if (!in_single_quote & !in_double_quote & paren == 0)
        //                    {
        //                        // 空白？
        //                        switch (ch)
        //                        {
        //                            case 9:
        //                            case 32:
        //                                {
        //                                    break;
        //                                }
        //                        }
        //                    }
        //                }
        //            }

        //            ListTailRet = Strings.Mid(list, i);
        //            return ListTailRet;
        //        }


        //        // タブを考慮したTrim
        //        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        //        public static void TrimString(ref string str_Renamed)
        //        {
        //            int j, i, lstr;
        //            lstr = Strings.Len(str_Renamed);
        //            i = 1;
        //            j = lstr;

        //            // 先頭の空白を検索
        //            while (i <= j)
        //            {
        //                switch (Strings.Asc(Strings.Mid(str_Renamed, i)))
        //                {
        //                    case 9:
        //                    case 32:
        //                    case -32448:
        //                        {
        //                            i = (i + 1);
        //                            break;
        //                        }

        //                    default:
        //                        {
        //                            break;
        //                        }
        //                }
        //            }

        //            // 末尾の空白を検索
        //            while (i < j)
        //            {
        //                switch (Strings.Asc(Strings.Mid(str_Renamed, j)))
        //                {
        //                    case 9:
        //                    case 32:
        //                    case -32448:
        //                        {
        //                            j = (j - 1);
        //                            break;
        //                        }

        //                    default:
        //                        {
        //                            break;
        //                        }
        //                }
        //            }

        //            // 空白があれば置き換え
        //            if (i != 1 | j != lstr)
        //            {
        //                str_Renamed = Strings.Mid(str_Renamed, i, j - i + 1);
        //            }
        //        }

        //        // 文字列 str 中に str2 が出現する位置を末尾から検索
        //        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        //        public static int InStr2(ref string str_Renamed, ref string str2)
        //        {
        //            int InStr2Ret = default;
        //            int slen, i;
        //            slen = Strings.Len(str2);
        //            i = (Strings.Len(str_Renamed) - slen + 1);
        //            while (i > 0)
        //            {
        //                if ((Strings.Mid(str_Renamed, i, slen) ?? "") == (str2 ?? ""))
        //                {
        //                    InStr2Ret = i;
        //                    return InStr2Ret;
        //                }

        //                i = (i - 1);
        //            }

        //            return InStr2Ret;
        //        }


        //        // 文字列をDoubleに変換
        //        public static double StrToDbl(ref string expr)
        //        {
        //            double StrToDblRet = default;
        //            if (Information.IsNumeric(expr))
        //            {
        //                StrToDblRet = Conversions.ToDouble(expr);
        //            }

        //            return StrToDblRet;
        //        }

        // 文字列をLongに変換
        // memo: VBのLongは32bit
        public static int StrToLng(string expr)
        {
            try
            {
                return Conversions.ToInteger(expr);
            }
            catch
            {
                return 0;
            }
        }

        // 文字列をひらがなに変換
        // ひらがなへの変換は日本語以外のOSを使うとエラーが発生するようなので
        // エラーをトラップする必要がある
        public static string StrToHiragana(string str)
        {
            return Strings.StrConv(str, VbStrConv.Hiragana);
        }

        // aとbの最大値を返す
        public static int MaxLng(int a, int b)
        {
            return Math.Max(a, b);
        }

        // aとbの最小値を返す
        public static int MinLng(int a, int b)
        {
            return Math.Min(a, b);
        }

        //        // aとbの最大値を返す (Double)
        //        public static double MaxDbl(double a, double b)
        //        {
        //            double MaxDblRet = default;
        //            if (a > b)
        //            {
        //                MaxDblRet = a;
        //            }
        //            else
        //            {
        //                MaxDblRet = b;
        //            }

        //            return MaxDblRet;
        //        }

        //        // aとbの最小値を返す (Double)
        //        public static double MinDbl(double a, double b)
        //        {
        //            double MinDblRet = default;
        //            if (a < b)
        //            {
        //                MinDblRet = a;
        //            }
        //            else
        //            {
        //                MinDblRet = b;
        //            }

        //            return MinDblRet;
        //        }


        //        // 文字列 buf の長さが length になるように左側にスペースを付加する
        //        public static string LeftPaddedString(ref string buf, int length)
        //        {
        //            string LeftPaddedStringRet = default;
        //            // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
        //            // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
        //            LeftPaddedStringRet = Strings.Space(MaxLng(length - LenB(Strings.StrConv(buf, vbFromUnicode)), 0)) + buf;
        //            return LeftPaddedStringRet;
        //        }

        //        // 文字列 buf の長さが length になるように右側にスペースを付加する
        //        public static string RightPaddedString(ref string buf, int length)
        //        {
        //            string RightPaddedStringRet = default;
        //            // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
        //            // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
        //            RightPaddedStringRet = buf + Strings.Space(MaxLng(length - LenB(Strings.StrConv(buf, vbFromUnicode)), 0));
        //            return RightPaddedStringRet;
        //        }


        //        // Src.ini ファイルの ini_section から ini_entry の値を読み出す
        //        public static string ReadIni(ref string ini_section, ref string ini_entry)
        //        {
        //            string ReadIniRet = default;
        //            var s = new Microsoft.VisualBasic.Compatibility.VB6.FixedLengthString(1024);
        //            var ret = default;

        //            // シナリオ側に Src.ini ファイルがあればそちらを優先
        //            string argfname = SRC.ScenarioPath + "Src.ini";
        //            if (FileExists(ref argfname))
        //            {
        //                string arglpDefault = "";
        //                string arglpReturnedString = s.Value;
        //                string arglpFileName = SRC.ScenarioPath + "Src.ini";
        //                ret = GetPrivateProfileString(ref ini_section, ini_entry, ref arglpDefault, ref arglpReturnedString, 1024, ref arglpFileName);
        //                s.Value = arglpReturnedString;
        //            }

        //            // シナリオ側 Src.ini にエントリが無ければ本体側から読み出し
        //            if (ret == 0)
        //            {
        //                string arglpDefault1 = "";
        //                string arglpReturnedString1 = s.Value;
        //                string arglpFileName1 = SRC.AppPath + "Src.ini";
        //                ret = GetPrivateProfileString(ref ini_section, ini_entry, ref arglpDefault1, ref arglpReturnedString1, 1024, ref arglpFileName1);
        //                s.Value = arglpReturnedString1;
        //            }

        //            // 不要部分を削除
        //            ReadIniRet = Strings.Left(s.Value, Strings.InStr(s.Value, Constants.vbNullChar) - 1);
        //            return ReadIniRet;
        //        }


        //        // Src.ini ファイルの ini_section の ini_entry に値 ini_data を書き込む
        //        public static void WriteIni(ref string ini_section, ref string ini_entry, ref string ini_data)
        //        {
        //            var s = new Microsoft.VisualBasic.Compatibility.VB6.FixedLengthString(1024);
        //            var ret = default;

        //            // LastFolderの設定のみは必ず本体側の Src.ini に書き込む
        //            if (ini_entry == "LastFolder")
        //            {
        //                string arglpFileName = SRC.AppPath + "Src.ini";
        //                ret = WritePrivateProfileString(ref ini_section, ini_entry, ref ini_data, ref arglpFileName);
        //                return;
        //            }

        //            // シナリオ側に Src.ini ファイルがあればそちらを優先
        //            bool localFileExists() { string argfname = SRC.ScenarioPath + "Src.ini"; var ret = FileExists(ref argfname); return ret; }

        //            if (Strings.Len(SRC.ScenarioPath) > 0 & localFileExists())
        //            {
        //                // エントリが存在するかチェック
        //                string arglpDefault = "";
        //                string arglpReturnedString = s.Value;
        //                string arglpFileName1 = SRC.ScenarioPath + "Src.ini";
        //                ret = GetPrivateProfileString(ref ini_section, ini_entry, ref arglpDefault, ref arglpReturnedString, 1024, ref arglpFileName1);
        //                s.Value = arglpReturnedString;
        //                if (ret > 1)
        //                {
        //                    string arglpFileName2 = SRC.ScenarioPath + "Src.ini";
        //                    ret = WritePrivateProfileString(ref ini_section, ini_entry, ref ini_data, ref arglpFileName2);
        //                }
        //            }

        //            // シナリオ側 Src.ini にエントリが無ければ本体側から読み出し
        //            if (ret == 0)
        //            {
        //                string arglpFileName3 = SRC.AppPath + "Src.ini";
        //                ret = WritePrivateProfileString(ref ini_section, ini_entry, ref ini_data, ref arglpFileName3);
        //            }
        //        }


        //        // 文字列 s1 中の s2 を s3 に置換
        //        // 置換を行ったときにはTrueを返す
        //        public static bool ReplaceString(ref string s1, ref string s2, ref string s3)
        //        {
        //            bool ReplaceStringRet = default;
        //            string buf;
        //            int len2, len3;
        //            int idx;
        //            idx = Strings.InStr(s1, s2);

        //            // 置換が必要？
        //            if (idx == 0)
        //            {
        //                return ReplaceStringRet;
        //            }

        //            len2 = Strings.Len(s2);
        //            len3 = Strings.Len(s3);

        //            // &は遅いので出来るだけMidを使う
        //            if (len2 == len3)
        //            {
        //                do
        //                {
        //                    StringType.MidStmtStr(ref s1, idx, len2, s3);
        //                    idx = Strings.InStr(s1, s2);
        //                }
        //                while (idx > 0);
        //            }
        //            else
        //            {
        //                buf = s1;
        //                s1 = "";
        //                do
        //                {
        //                    s1 = s1 + Strings.Left(buf, idx - 1) + s3;
        //                    buf = Strings.Mid(buf, idx + len2);
        //                    idx = Strings.InStr(buf, s2);
        //                }
        //                while (idx > 0);
        //                s1 = s1 + buf;
        //            }

        //            ReplaceStringRet = true;
        //            return ReplaceStringRet;
        //        }

        // ファイル fname が存在するか判定
        public static bool FileExists(string fname)
        {
            return File.Exists(fname);
        }

        //        // データファイルfnumからデータを一行読み込み、line_bufに格納するとともに
        //        // 行番号line_numを更新する。
        //        // 行頭に「#」がある場合は行の読み飛ばしを行う。
        //        // 行中に「//」がある場合、そこからはコメントと見なして無視する。
        //        // 行末に「_」がある場合は行の結合を行う。
        //        public static void GetLine(ref int fnum, ref string line_buf, ref int line_num)
        //        {
        //            string buf;
        //            int idx;
        //            line_buf = "";
        //            while (!FileSystem.EOF(fnum))
        //            {
        //                line_num = line_num + 1;
        //                buf = FileSystem.LineInput(fnum);

        //                // コメント行はスキップ
        //                if (Strings.Left(buf, 1) == "#")
        //                {
        //                    goto NextLine;
        //                }

        //                // コメント部分を削除
        //                idx = Strings.InStr(buf, "//");
        //                if (idx > 0)
        //                {
        //                    buf = Strings.Left(buf, idx - 1);
        //                }

        //                // 行末が「_」でなければ行の読み込みを完了
        //                if (Strings.Right(buf, 1) != "_")
        //                {
        //                    TrimString(ref buf);
        //                    line_buf = line_buf + buf;
        //                    break;
        //                }

        //                // 行末が「_」の場合は行を結合
        //                TrimString(ref buf);
        //                line_buf = line_buf + Strings.Left(buf, Strings.Len(buf) - 1);
        //            NextLine:
        //                ;
        //            }

        //            string args2 = "，";
        //            string args3 = ", ";
        //            ReplaceString(ref line_buf, ref args2, ref args3);
        //        }


        //        // Windowsのバージョンを判定する
        //        public static int GetWinVersion()
        //        {
        //            int GetWinVersionRet = default;
        //            var vinfo = default(OSVERSIONINFO);
        //            int ret;
        //            {
        //                var withBlock = vinfo;
        //                // dwOSVersionInfoSizeに構造体のサイズをセットする。
        //                withBlock.dwOSVersionInfoSize = Strings.Len(vinfo);

        //                // OSのバージョン情報を得る。
        //                ret = GetVersionEx(ref vinfo);
        //                if (ret == 0)
        //                {
        //                    return GetWinVersionRet;
        //                }

        //                GetWinVersionRet = (withBlock.dwMajorVersion * 100 + withBlock.dwMinorVersion);
        //            }

        //            return GetWinVersionRet;
        //        }


        // 数値を指数表記を使わずに文字列表記する
        public static string FormatNum(double n)
        {
            string FormatNumRet = default;
            if (n % 1 == 0d)
            {
                FormatNumRet = n.ToString("0");
            }
            else
            {
                FormatNumRet = n.ToString("0.#######################################################################");
            }

            return FormatNumRet;
        }


        // 文字列 str が数値かどうか調べる
        public static bool IsNumber( string str)
        {
            bool IsNumberRet = default;
            if (!Information.IsNumeric(str))
            {
                return IsNumberRet;
            }

            // "(1)"のような文字列が数値と判定されてしまうのを防ぐ
            if (Strings.Asc(str) == 40)
            {
                return IsNumberRet;
            }

            IsNumberRet = true;
            return IsNumberRet;
        }


        //        // 武器属性処理用の関数群。

        //        // 属性を一つ取得する。複数文字の属性もひとつとして取得する。
        //        // ただしＭは防御特性において単体文字として扱われるために除く。
        //        // 検索文字列 aname
        //        // 検索位置 idx (検索終了位置を返す)
        //        // 取得文字長 length (特殊効果数カウント用。基本的に0(属性取得)か1(属性頭文字取得))
        //        public static string GetClassBundle(ref string aname, ref int idx, int length = 0)
        //        {
        //            string GetClassBundleRet = default;
        //            int i;
        //            string ch;
        //            i = idx;
        //            ch = Strings.Mid(aname, i, 1);
        //            // 弱、効、剋があればその次の文字まで一緒に取得する。
        //            // 入れ子可能なため弱、効、剋が続く限りループ
        //            while (ch == "弱" | ch == "効" | ch == "剋")
        //            {
        //                // 属性指定の最後の文字が弱効剋だった場合、属性なし
        //                if (i >= Strings.Len(aname))
        //                    goto NotFoundClass;
        //                i = (i + 1);
        //                ch = Strings.Mid(aname, i, 1);
        //            }
        //            // 低があればその次の文字まで一緒に取得する。
        //            if (ch == "低")
        //            {
        //                i = (i + 1);
        //                // midの開始位置指定は文字数を超えていても大丈夫なはずですが念の為
        //                if (i > Strings.Len(aname))
        //                {
        //                    goto NotFoundClass;
        //                }

        //                ch = Strings.Mid(aname, i, 1);
        //                if (ch != "攻" & ch != "防" & ch != "運" & ch != "移")
        //                {
        //                    goto NotFoundClass;
        //                }
        //            }

        //            if (length == 0)
        //            {
        //                GetClassBundleRet = Strings.Mid(aname, idx, i - idx + 1);
        //            }
        //            else
        //            {
        //                GetClassBundleRet = Strings.Mid(aname, idx, length);
        //            }

        //        NotFoundClass:
        //            ;
        //            idx = i;
        //            return GetClassBundleRet;
        //        }

        //        // InStrと同じ動作。ただし見つかった文字の前に「弱」「効」「剋」があった場合、別属性と判定する)
        //        public static int InStrNotNest(ref string string1, ref string string2, int start = 1)
        //        {
        //            int InStrNotNestRet = default;
        //            int i;
        //            string c;
        //            i = Strings.InStr(start, string1, string2);
        //            // 先頭一致か、一致なしのとき、ここで取得
        //            if (i <= 1)
        //            {
        //                InStrNotNestRet = i;
        //            }
        //            else
        //            {
        //                while (i > 0)
        //                {
        //                    c = Strings.Mid(string1, i - 1, 1);
        //                    // 検知した文字の前の文字が弱効剋でなかったら、InStrの結果を返す
        //                    if (c != "弱" & c != "効" & c != "剋")
        //                    {
        //                        break;
        //                    }
        //                    // 検知した文字の前の文字が弱効剋だったら、再度文字列を探索する
        //                    if (i < Strings.Len(string1))
        //                    {
        //                        i = Strings.InStr(i + 1, string1, string2);
        //                    }
        //                    else
        //                    {
        //                        i = 0;
        //                    }
        //                }

        //                InStrNotNestRet = i;
        //            }

        //            return InStrNotNestRet;
        //        }
    }
}
