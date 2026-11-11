// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;
using System.Globalization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace SRCCore.Lib
{
    // 汎用的な処理を行うモジュール
    public static class GeneralLib
    {
        /// <summary>
        /// データ（ユニット・パイロット・非戦闘員）の画像に指定できるファイル名か。
        /// 元の SRC は .bmp だけだが、SRC# は PaintPicture コマンドなどと同じく .png・.jpg・.gif も受け付ける。
        /// </summary>
        /// <summary>
        /// セーブデータのファイル名の拡張子を SRC# のもの（extension。srcs・srcq）にする。
        /// 元の SRC の拡張子 .src で終わる名前（シナリオが「セーブデータファイル名」に設定したものなど）は、
        /// .src を置き換える。保存のダイアログが付け足した「.src.srcs」も「.srcs」にする。
        /// </summary>
        public static string ToSaveFileName(string fname, string extension)
        {
            if (string.IsNullOrEmpty(fname) || string.IsNullOrEmpty(extension))
            {
                return fname;
            }
            var ext = "." + extension.TrimStart('.');
            if (fname.EndsWith(".src" + ext, StringComparison.OrdinalIgnoreCase))
            {
                return fname.Substring(0, fname.Length - (".src" + ext).Length) + ext;
            }
            if (fname.EndsWith(".src", StringComparison.OrdinalIgnoreCase))
            {
                return fname.Substring(0, fname.Length - ".src".Length) + ext;
            }
            return fname;
        }

        public static bool IsImageFileName(string fname)
        {
            switch (Strings.LCase(Strings.Right(fname ?? "", 4)))
            {
                case ".bmp":
                case ".png":
                case ".jpg":
                case ".gif":
                    return true;
                default:
                    return false;
            }
        }

        private static Stopwatch sw = new Stopwatch();
        private static long skippedMillis;

        /// <summary>
        /// true のあいだ、決まった時間だけ待つ処理（<see cref="TimedSleep"/>）は眠らず、
        /// 待つはずだった時間だけ <see cref="timeGetTime"/> を進める。
        /// 画面を見ないヘッドレス実行（srcsharp play --skip-wait）で使う。
        /// </summary>
        public static bool SkipWait { get; set; }

        public static int timeGetTime()
        {
            if (!sw.IsRunning)
            {
                sw.Start();
            }
            // VB の timeGetTime（Windows を起動してからのミリ秒）と同じく、Long（32 ビット）に収まらない分は桁が落ちて負の値に回る
            return unchecked((int)(sw.ElapsedMilliseconds + Interlocked.Read(ref skippedMillis)));
        }

        /// <summary>
        /// 決まった時間だけ待つ。<see cref="SkipWait"/> のときは眠らずに <see cref="timeGetTime"/> を進める。
        /// 入力や別スレッドを待つポーリングには使わない（そちらは実時間で待つ）。
        /// </summary>
        public static void TimedSleep(int milliseconds)
        {
            if (milliseconds <= 0)
            {
                return;
            }
            if (SkipWait)
            {
                Interlocked.Add(ref skippedMillis, milliseconds);
                return;
            }
            Thread.Sleep(milliseconds);
        }

        // 乱数発生用シード値
        public static int RndSeed;

        // 乱数系列のサイズ（VB5の元実装に合わせて4096個）
        private const int RndHistorySize = 4096;

        // 乱数系列のセーブ＆ロードが出来るよう乱数系列をあらかじめ配列に保存して確定させる
        private static double[] RndHistory = new double[RndHistorySize + 1];

        // 乱数系列の中で現在使用している値のインデックス
        public static int RndIndex;

        /// <summary>
        /// Option「乱数系列非保存」が設定されているか。<see cref="Dice"/> が見る。
        /// GeneralLib は static なので、SRC を作ったときにその SRC のオプションを見るものを入れる
        /// </summary>
        public static Func<bool> IsRndHistoryUnsaved { get; set; }

        // 乱数系列を使わないときの乱数（VB の Rnd）
        private static readonly Random unsavedRand = new Random();

        // 乱数系列のリセット
        // VB は Randomize RndSeed の後の Rnd で埋める。SRC# は .NET の Random で埋めるので、同じシード値でも系列は VB と違う
        public static void RndReset()
        {
            var rng = new Random(RndSeed);
            // インデックスは1始まりで使用する
            for (int i = 1; i <= RndHistorySize; i++)
            {
                RndHistory[i] = rng.NextDouble();
            }
            RndIndex = 0;
        }

        // 1～max の乱数を返す
        public static int Dice(int max)
        {
            if (max <= 1)
            {
                return max;
            }

            // Option「乱数系列非保存」なら乱数系列を使わない（RndIndex は進まない）
            if (IsRndHistoryUnsaved?.Invoke() ?? false)
            {
                return VBMath.Int((max * unsavedRand.NextDouble()) + 1);
            }

            RndIndex = (RndIndex + 1);
            if (RndIndex > RndHistorySize)
            {
                RndIndex = 1;
            }

            return VBMath.Int((max * RndHistory[RndIndex]) + 1);
        }

        public static IList<string> ToL(string list)
        {
            return (list ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public static string LNormalize(string list)
        {
            return string.Join(" ", ToL(list));
        }

        // リスト list から idx 番目の要素を返す
        public static string LIndex(string list, int idx)
        {
            // idxが正の数でなければ空文字列を返す
            if (idx < 1)
            {
                return "";
            }

            var l = ToL(list ?? "");
            if (l.Count >= idx)
            {
                return l[idx - 1];
            }
            return "";
        }

        // リスト list の要素数を返す
        public static int LLength(string list)
        {
            return ToL(list).Count;
        }

        // リスト list から、リストの要素の配列 larray を作成し、
        // リストの要素数を返す
        public static int LSplit(string list, out string[] larray)
        {
            larray = ToL(list).ToArray();
            return larray.Length;
        }

        // 文字列 ch が空白かどうか調べる
        // VB と同じく、空文字列か、先頭の文字がタブ・CR・半角スペース・160 なら空白とみなす
        public static bool IsSpace(string ch)
        {
            if (string.IsNullOrEmpty(ch))
            {
                return true;
            }

            switch (ch[0])
            {
                case '\t':
                case '\r':
                case ' ':
                case ' ':
                    return true;
                default:
                    return false;
            }
        }

        // リスト list の要素を分割して返す (括弧を考慮)
        // SRC# 独自。元の SRC の ListSplit と違い、閉じ括弧が多すぎる箇所の後ろを別の要素にして返し、
        // 閉じていない括弧・クォートも区別しない。元の SRC と同じ結果が要るときは ListSplit・ListIndex・ListLength を使う
        public static IList<string> ToList(string list, bool removeKakko = false)
        {
            bool in_single_quote = false;
            bool in_double_quote = false;
            int i = -1;
            int paren = 0;
            var current = new StringBuilder();
            var result = new List<string>();
            foreach (var c in (list ?? "").ToCharArray())
            {
                i++;
                if (!in_single_quote && !in_double_quote && paren == 0)
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
                    if (c == '`') // "`"
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

                        case '`': // "`"
                            in_single_quote = true;
                            break;

                        case '"': // """"
                            in_double_quote = true;
                            break;
                    }
                }
                // removeKakko は一番外側の括弧を外す（SRC# 独自。元の SRC の ListIndex などは括弧を残す）
                if (append || !removeKakko)
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

        // VB: ListIndex などの空白（タブ・半角スペース）
        private static bool IsListSpace(char ch)
        {
            return ch == '\t' || ch == ' ';
        }

        // VB: ListIndex などが 1 文字ずつ括弧・クォートの状態を進める処理。
        // 閉じ括弧が多すぎる（括弧の対応が取れていない）ときは false
        private static bool ScanListChar(char ch, ref int paren, ref bool in_single_quote, ref bool in_double_quote)
        {
            if (in_single_quote)
            {
                if (ch == '`')
                {
                    in_single_quote = false;
                }
            }
            else if (in_double_quote)
            {
                if (ch == '"')
                {
                    in_double_quote = false;
                }
            }
            else
            {
                switch (ch)
                {
                    case '(':
                    case '[':
                        paren = paren + 1;
                        break;

                    case ')':
                    case ']':
                        paren = paren - 1;
                        if (paren < 0)
                        {
                            // 括弧の対応が取れていない
                            return false;
                        }
                        break;

                    case '`':
                        in_single_quote = true;
                        break;

                    case '"':
                        in_double_quote = true;
                        break;
                }
            }
            return true;
        }

        // VB: ListIndex・ListTail の「idx 番目の要素まで読み飛ばす」。
        // 見つかれば idx 番目の要素の先頭の位置（1 始まり）・その文字・括弧とクォートの状態を返す。
        // その前に文字列の終わりに達したか、括弧の対応が取れていなければ false
        private static bool SkipListElements(string list, int idx, out int pos, out char ch, out int paren, out bool in_single_quote, out bool in_double_quote)
        {
            var list_len = list.Length;
            var n = 0;
            var i = 0;
            ch = '\0';
            paren = 0;
            in_single_quote = false;
            in_double_quote = false;
            pos = 0;
            while (true)
            {
                // 空白を読み飛ばす
                while (true)
                {
                    i = i + 1;
                    // 文字列の終り？
                    if (i > list_len)
                    {
                        return false;
                    }
                    ch = list[i - 1];
                    if (!IsListSpace(ch))
                    {
                        break;
                    }
                }

                // 要素数を１つ増やす
                n = n + 1;

                // 求める要素？
                if (n == idx)
                {
                    pos = i;
                    return true;
                }

                // 要素を読み飛ばす
                while (true)
                {
                    if (!ScanListChar(ch, ref paren, ref in_single_quote, ref in_double_quote))
                    {
                        return false;
                    }
                    i = i + 1;
                    // 文字列の終り？
                    if (i > list_len)
                    {
                        return false;
                    }
                    ch = list[i - 1];
                    // 要素の末尾か判定
                    if (!in_single_quote && !in_double_quote && paren == 0 && IsListSpace(ch))
                    {
                        break;
                    }
                }
            }
        }

        // リスト list から idx 番目の要素を返す (括弧を考慮)
        // VB と同じく、括弧の対応が取れていなければ（閉じ括弧が多すぎれば）空文字列。末尾で閉じていない要素はそのまま返す
        public static string ListIndex(string list, int idx)
        {
            // idxが正の数でなければ空文字列を返す
            if (idx < 1)
            {
                return "";
            }

            list = list ?? "";
            if (!SkipListElements(list, idx, out var i, out var ch, out var paren, out var in_single_quote, out var in_double_quote))
            {
                return "";
            }

            // 求める要素を読み込む
            var list_len = list.Length;
            var begin = i;
            while (true)
            {
                if (!ScanListChar(ch, ref paren, ref in_single_quote, ref in_double_quote))
                {
                    return "";
                }
                i = i + 1;
                // 文字列の終り？
                if (i > list_len)
                {
                    return list.Substring(begin - 1);
                }
                ch = list[i - 1];
                // 要素の末尾か判定
                if (!in_single_quote && !in_double_quote && paren == 0 && IsListSpace(ch))
                {
                    break;
                }
            }
            return list.Substring(begin - 1, i - begin);
        }

        // リスト list の要素数を返す (括弧を考慮)
        // VB と同じく、括弧・クォートの対応が取れていなければ -1
        public static int ListLength(string list)
        {
            list = list ?? "";
            var list_len = list.Length;
            var length = 0;
            var i = 0;
            var paren = 0;
            var in_single_quote = false;
            var in_double_quote = false;
            char ch;
            while (true)
            {
                // 空白を読み飛ばす
                while (true)
                {
                    i = i + 1;
                    // 文字列の終り？
                    if (i > list_len)
                    {
                        return length;
                    }
                    ch = list[i - 1];
                    if (!IsListSpace(ch))
                    {
                        break;
                    }
                }

                // 要素数を１つ増やす
                length = length + 1;

                // 要素を読み飛ばす
                while (true)
                {
                    if (!ScanListChar(ch, ref paren, ref in_single_quote, ref in_double_quote))
                    {
                        // 括弧の対応が取れていない
                        return -1;
                    }
                    i = i + 1;
                    // 文字列の終り？
                    if (i > list_len)
                    {
                        // クォートや括弧の対応が取れている？
                        return in_single_quote || in_double_quote || paren != 0 ? -1 : length;
                    }
                    ch = list[i - 1];
                    // 要素の末尾か判定
                    if (!in_single_quote && !in_double_quote && paren == 0 && IsListSpace(ch))
                    {
                        break;
                    }
                }
            }
        }

        // リスト list から、リストの要素の配列 larray を作成し、
        // リストの要素数を返す (括弧を考慮)
        // Convert: VBの配列と違って0オフセットな点に注意すること。
        // VB と同じく、括弧・クォートの対応が取れていなければ -1。
        // そのとき、最後の要素は対応の取れていない要素の先頭から文字列の終わりまで
        public static int ListSplit(string list, out string[] larray)
        {
            list = list ?? "";
            var list_len = list.Length;
            var result = new List<string>();
            var n = 0;
            var i = 0;
            var paren = 0;
            var in_single_quote = false;
            var in_double_quote = false;
            char ch;
            while (true)
            {
                // 空白を読み飛ばす
                while (true)
                {
                    i = i + 1;
                    // 文字列の終り？
                    if (i > list_len)
                    {
                        larray = result.ToArray();
                        return n;
                    }
                    ch = list[i - 1];
                    if (!IsListSpace(ch))
                    {
                        break;
                    }
                }

                // 要素数を１つ増やす
                n = n + 1;

                // 要素を読み込む
                var begin = i;
                while (true)
                {
                    if (!ScanListChar(ch, ref paren, ref in_single_quote, ref in_double_quote))
                    {
                        // 括弧の対応が取れていない
                        result.Add(list.Substring(begin - 1));
                        larray = result.ToArray();
                        return -1;
                    }
                    i = i + 1;
                    // 文字列の終り？
                    if (i > list_len)
                    {
                        result.Add(list.Substring(begin - 1));
                        larray = result.ToArray();
                        // クォートや括弧の対応が取れている？
                        return !in_single_quote && !in_double_quote && paren == 0 ? n : -1;
                    }
                    ch = list[i - 1];
                    // 要素の末尾か判定
                    if (!in_single_quote && !in_double_quote && paren == 0 && IsListSpace(ch))
                    {
                        break;
                    }
                }
                result.Add(list.Substring(begin - 1, i - begin));
            }
        }

        // リスト list から idx 番目以降の全要素を返す (括弧を考慮)
        // VB と同じく、idx 番目の要素の先頭から文字列の終わりまでをそのまま返す（要素の間・末尾の空白は詰めない）。
        // idx 番目の要素が無いか、その前の要素で括弧・クォートの対応が取れていなければ空文字列
        public static string ListTail(string list, int idx)
        {
            // idxが正の数でなければ空文字列を返す
            if (idx < 1)
            {
                return "";
            }

            list = list ?? "";
            if (!SkipListElements(list, idx, out var i, out _, out _, out _, out _))
            {
                return "";
            }
            return list.Substring(i - 1);
        }

        // タブを考慮したTrim
        // VB: TrimString。タブ・半角スペース・全角スペース（Asc が -32448）を前後から取り除く（VB は引数を書き換える Sub）
        private static readonly char[] TrimStringChars = { '\t', ' ', '　' };
        public static string TrimString(string str)
        {
            return string.IsNullOrEmpty(str) ? "" : str.Trim(TrimStringChars);
        }

        // 文字列 str 中に str2 が出現する位置を末尾から検索
        public static int InStr2(string str, string str2)
        {
            int InStr2Ret = default;
            int slen, i;
            slen = Strings.Len(str2);
            i = (Strings.Len(str) - slen + 1);
            while (i > 0)
            {
                if ((Strings.Mid(str, i, slen) ?? "") == (str2 ?? ""))
                {
                    InStr2Ret = i;
                    return InStr2Ret;
                }

                i = (i - 1);
            }

            return InStr2Ret;
        }

        // 文字列をDoubleに変換
        // VB: IsNumeric でなければ 0。VB の IsNumeric は「NaN」「Infinity」や Double を超える値を数値とみなさない
        public static double StrToDbl(string expr)
        {
            var num = Conversions.ToDouble(expr);
            return double.IsNaN(num) || double.IsInfinity(num) ? 0d : num;
        }

        // 文字列をLongに変換
        // memo: VBのLongは32bit
        public static int StrToLng(string expr)
        {
            // VB版は IsNumeric なら CLng で変換し、エラー（オーバーフロー）時は0を返す
            expr = Conversions.NumericText(expr);
            if (int.TryParse(expr, NumberStyles.Integer, SrcCulture.Japanese, out var ret))
            {
                return ret;
            }
            var num = Conversions.ToDouble(expr);
            if (double.IsNaN(num) || num >= (double)int.MaxValue + 0.5d || num < (double)int.MinValue - 0.5d)
            {
                return 0;
            }
            return Conversions.ToInteger(num);
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

        // aとbの最大値を返す (Double)
        // VB と同じく a > b で比べる（NaN との比較は偽なので b を返す）
        public static double MaxDbl(double a, double b)
        {
            return a > b ? a : b;
        }

        // aとbの最小値を返す (Double)
        // VB と同じく a < b で比べる（NaN との比較は偽なので b を返す）
        public static double MinDbl(double a, double b)
        {
            return a < b ? a : b;
        }

        // 文字列 buf の長さが length になるように左側にスペースを付加する
        public static string LeftPaddedString(string buf, int length)
        {
            return Strings.Space(MaxLng(length - StrWidth(buf), 0)) + buf;
        }

        // 文字列 buf の長さが length になるように右側にスペースを付加する
        public static string RightPaddedString(string buf, int length)
        {
            return buf + Strings.Space(MaxLng(length - StrWidth(buf), 0));
        }

        /// <summary>
        /// 半角換算の文字幅。VB の LenB(StrConv(buf, vbFromUnicode))（Shift_JIS のバイト数）と同じ。
        /// Shift_JIS に無い文字は「?」の 1 バイトになる。
        /// </summary>
        public static int StrWidth(string buf)
        {
            return Strings.LenB(buf);
        }

        // 文字列 s1 中の s2 を s3 に置換した文字列を返す
        // VB版と同様に、s2 と s3 の長さが等しい場合は置換後の文字列の先頭から再検索する
        public static string ReplaceString(string s1, string s2, string s3)
        {
            s1 = s1 ?? "";
            s3 = s3 ?? "";
            // VB版では無限ループとなるケースは置換しない
            if (string.IsNullOrEmpty(s2) || s2 == s3)
            {
                return s1;
            }

            var idx = s1.IndexOf(s2, StringComparison.Ordinal);
            if (idx < 0)
            {
                return s1;
            }

            if (s2.Length == s3.Length)
            {
                var buf = new StringBuilder(s1);
                // 念のため置換回数に上限を設ける
                var limit = Math.Max(1000, s1.Length * s1.Length);
                do
                {
                    buf.Remove(idx, s2.Length).Insert(idx, s3);
                    idx = buf.ToString().IndexOf(s2, StringComparison.Ordinal);
                }
                while (idx >= 0 && --limit > 0);
                return buf.ToString();
            }

            return s1.Replace(s2, s3, StringComparison.Ordinal);
        }

        // 数値を指数表記を使わずに文字列表記する
        public static string FormatNum(double n)
        {
            string FormatNumRet = default;
            if (n % 1 == 0d)
            {
                FormatNumRet = n.ToString("0", SrcCulture.Japanese);
            }
            else
            {
                FormatNumRet = n.ToString("0.#######################################################################", SrcCulture.Japanese);
            }

            return FormatNumRet;
        }

        // 文字列 str が数値かどうか調べる
        public static bool IsNumber(string str)
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

        // 武器属性処理用の関数群。

        // 属性を一つ取得する。複数文字の属性もひとつとして取得する。
        // ただしＭは防御特性において単体文字として扱われるために除く。
        // 検索文字列 aname
        // 検索位置 idx (検索終了位置を返す)
        // 取得文字長 length (特殊効果数カウント用。基本的に0(属性取得)か1(属性頭文字取得))
        public static string GetClassBundle(string aname, ref int idx, int length = 0)
        {
            string GetClassBundleRet = default;
            int i;
            string ch;
            i = idx;
            ch = Strings.Mid(aname, i, 1);
            // 弱、効、剋があればその次の文字まで一緒に取得する。
            // 入れ子可能なため弱、効、剋が続く限りループ
            while (ch == "弱" || ch == "効" || ch == "剋")
            {
                // 属性指定の最後の文字が弱効剋だった場合、属性なし
                if (i >= Strings.Len(aname))
                    goto NotFoundClass;
                i = (i + 1);
                ch = Strings.Mid(aname, i, 1);
            }
            // 低があればその次の文字まで一緒に取得する。
            if (ch == "低")
            {
                i = (i + 1);
                // midの開始位置指定は文字数を超えていても大丈夫なはずですが念の為
                if (i > Strings.Len(aname))
                {
                    goto NotFoundClass;
                }

                ch = Strings.Mid(aname, i, 1);
                if (ch != "攻" && ch != "防" && ch != "運" && ch != "移")
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
        public static int InStrNotNest(string string1, string string2, int start = 1)
        {
            int InStrNotNestRet = default;
            int i;
            string c;
            i = Strings.InStr(start, string1 ?? "", string2);
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
                    if (c != "弱" && c != "効" && c != "剋")
                    {
                        break;
                    }
                    // 検知した文字の前の文字が弱効剋だったら、再度文字列を探索する
                    if (i < Strings.Len(string1))
                    {
                        i = Strings.InStr(i + 1, string1, string2);
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
