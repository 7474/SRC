// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace SRCCore.Lib
{
    // 汎用的な処理を行うモジュール
    public static class GeneralLib
    {
        //        // iniファイルの読み出し
        //        [DllImport("kernel32", EntryPoint = "GetPrivateProfileStringA")]
        //        static extern int GetPrivateProfileString(string lpApplicationName, Any lpKeyName, string lpDefault, string lpReturnedString, int nSize, string lpFileName);

        //        // iniファイルへの書き込み
        //        [DllImport("kernel32", EntryPoint = "WritePrivateProfileStringA")]
        //        static extern int WritePrivateProfileString(string lpApplicationName, Any lpKeyName, string lpString, string lpFileName);

        //        // Windowsが起動してからの時間を返す(ミリ秒)
        //        [DllImport("winmm.dll")]
        private static Stopwatch sw = new Stopwatch();
        public static int timeGetTime()
        {
            if (!sw.IsRunning)
            {
                sw.Start();
            }
            // XXX
            return (int)sw.ElapsedMilliseconds;
        }

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
        //        static extern int GetVersionEx(OSVERSIONINFO lpVersionInformation);

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

        // 1～max の乱数を返す
        private static Random random = new Random();
        public static int Dice(int max)
        {
            if (max <= 1)
            {
                return max;
            }
            return random.Next(max) + 1;
            // TODO Impl
            //string argoname = "乱数系列非保存";
            //if (Expression.IsOptionDefined(argoname))
            //{
            //    DiceRet = Conversion.Int(max * VBMath.Rnd() + 1f);
            //    return DiceRet;
            //}

            //RndIndex = (RndIndex + 1);
            //if (RndIndex > Information.UBound(RndHistory))
            //{
            //    RndIndex = 1;
            //}

            //DiceRet = Conversion.Int(max * RndHistory[RndIndex] + 1f);
            //return DiceRet;
        }

        public static IList<string> ToL(string list)
        {
            return list.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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

        //        // 文字列 ch が空白かどうか調べる
        //        public static bool IsSpace(string ch)
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
        //        public static void LAppend(string list, string str_Renamed)
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
        //        public static int SearchList(string list, string str_Renamed)
        //        {
        //            int SearchListRet = default;
        //            int i;
        //            var loopTo = LLength(list);
        //            for (i = 1; i <= loopTo; i++)
        //            {
        //                if ((LIndex(list, i) ?? "") == (str_Renamed ?? ""))
        //                {
        //                    SearchListRet = i;
        //                    return SearchListRet;
        //                }
        //            }

        //            SearchListRet = 0;
        //            return SearchListRet;
        //        }

        // リスト list の要素を分割して返す (括弧を考慮)
        public static IList<string> ToList(string list, bool removeKakko = false)
        {
            bool hasError;
            return ToListInternal(list, out hasError, removeKakko);
        }

        private static IList<string> ToListInternal(string list, out bool hasError, bool removeKakko = false)
        {
            hasError = false;
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
                                hasError = true;
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
                // XXX リスト中のカッコ消えちゃうと困るしもとは消してないっぽいから消さないでおく
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

        // リスト list の要素数を返す (括弧を考慮)
        public static int ListLength(string list)
        {
            return ToList(list).Count;
        }

        // リスト list から、リストの要素の配列 larray を作成し、
        // リストの要素数を返す (括弧を考慮)
        // Convert: VBの配列と違って0オフセットな点に注意すること。
        public static int ListSplit(string list, out string[] larray)
        {
            bool hasError;
            larray = ToListInternal(list, out hasError).ToArray();
            return hasError ? -1 : larray.Length;
        }

        // リスト list から idx 番目以降の全要素を返す (括弧を考慮)
        public static string ListTail(string list, int idx)
        {
            return idx > 1 ? string.Join(" ", ToList(list ?? "").Skip(idx - 1)) : "";
        }

        //        // タブを考慮したTrim
        //        // UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        //        public static void TrimString(string str_Renamed)
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
        //        public static int InStr2(string str_Renamed, string str2)
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


        // 文字列をDoubleに変換
        public static double StrToDbl(string expr)
        {
            double StrToDblRet = default;
            if (Information.IsNumeric(expr))
            {
                StrToDblRet = Conversions.ToDouble(expr);
            }

            return StrToDblRet;
        }

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


        // 文字列 buf の長さが length になるように左側にスペースを付加する
        public static string LeftPaddedString(string buf, int length)
        {
            return buf;
            // TODO 面倒くさいのでとりあえずパス
            //string LeftPaddedStringRet = default;
            //LeftPaddedStringRet = Strings.Space(MaxLng(length - LenB(Strings.StrConv(buf, vbFromUnicode)), 0)) + buf;
            //return LeftPaddedStringRet;
        }

        // 文字列 buf の長さが length になるように右側にスペースを付加する
        public static string RightPaddedString(string buf, int length)
        {
            return buf;
            // TODO 面倒くさいのでとりあえずパス
            //string RightPaddedStringRet = default;
            //RightPaddedStringRet = buf + Strings.Space(MaxLng(length - LenB(Strings.StrConv(buf, vbFromUnicode)), 0));
            //return RightPaddedStringRet;
        }

        //        // Src.ini ファイルの ini_section から ini_entry の値を読み出す
        //        public static string ReadIni(string ini_section, string ini_entry)
        //        {
        //            string ReadIniRet = default;
        //            var s = new Microsoft.VisualBasic.Compatibility.VB6.FixedLengthString(1024);
        //            var ret = default;

        //            // シナリオ側に Src.ini ファイルがあればそちらを優先
        //            string argfname = SRC.ScenarioPath + "Src.ini";
        //            if (FileExists(argfname))
        //            {
        //                string arglpDefault = "";
        //                string arglpReturnedString = s.Value;
        //                string arglpFileName = SRC.ScenarioPath + "Src.ini";
        //                ret = GetPrivateProfileString(ini_section, ini_entry, arglpDefault, arglpReturnedString, 1024, arglpFileName);
        //                s.Value = arglpReturnedString;
        //            }

        //            // シナリオ側 Src.ini にエントリが無ければ本体側から読み出し
        //            if (ret == 0)
        //            {
        //                string arglpDefault1 = "";
        //                string arglpReturnedString1 = s.Value;
        //                string arglpFileName1 = SRC.AppPath + "Src.ini";
        //                ret = GetPrivateProfileString(ini_section, ini_entry, arglpDefault1, arglpReturnedString1, 1024, arglpFileName1);
        //                s.Value = arglpReturnedString1;
        //            }

        //            // 不要部分を削除
        //            ReadIniRet = Strings.Left(s.Value, Strings.InStr(s.Value, Constants.vbNullChar) - 1);
        //            return ReadIniRet;
        //        }


        //        // Src.ini ファイルの ini_section の ini_entry に値 ini_data を書き込む
        //        public static void WriteIni(string ini_section, string ini_entry, string ini_data)
        //        {
        //            var s = new Microsoft.VisualBasic.Compatibility.VB6.FixedLengthString(1024);
        //            var ret = default;

        //            // LastFolderの設定のみは必ず本体側の Src.ini に書き込む
        //            if (ini_entry == "LastFolder")
        //            {
        //                string arglpFileName = SRC.AppPath + "Src.ini";
        //                ret = WritePrivateProfileString(ini_section, ini_entry, ini_data, arglpFileName);
        //                return;
        //            }

        //            // シナリオ側に Src.ini ファイルがあればそちらを優先
        //            bool localFileExists() { string argfname = SRC.ScenarioPath + "Src.ini"; var ret = FileExists(argfname); return ret; }

        //            if (Strings.Len(SRC.ScenarioPath) > 0 & localFileExists())
        //            {
        //                // エントリが存在するかチェック
        //                string arglpDefault = "";
        //                string arglpReturnedString = s.Value;
        //                string arglpFileName1 = SRC.ScenarioPath + "Src.ini";
        //                ret = GetPrivateProfileString(ini_section, ini_entry, arglpDefault, arglpReturnedString, 1024, arglpFileName1);
        //                s.Value = arglpReturnedString;
        //                if (ret > 1)
        //                {
        //                    string arglpFileName2 = SRC.ScenarioPath + "Src.ini";
        //                    ret = WritePrivateProfileString(ini_section, ini_entry, ini_data, arglpFileName2);
        //                }
        //            }

        //            // シナリオ側 Src.ini にエントリが無ければ本体側から読み出し
        //            if (ret == 0)
        //            {
        //                string arglpFileName3 = SRC.AppPath + "Src.ini";
        //                ret = WritePrivateProfileString(ini_section, ini_entry, ini_data, arglpFileName3);
        //            }
        //        }


        //        // 文字列 s1 中の s2 を s3 に置換
        //        // 置換を行ったときにはTrueを返す
        //        public static bool ReplaceString(string s1, string s2, string s3)
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
        //                    StringType.MidStmtStr(s1, idx, len2, s3);
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
        //        public static void GetLine(int fnum, string line_buf, int line_num)
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
        //                    TrimString(buf);
        //                    line_buf = line_buf + buf;
        //                    break;
        //                }

        //                // 行末が「_」の場合は行を結合
        //                TrimString(buf);
        //                line_buf = line_buf + Strings.Left(buf, Strings.Len(buf) - 1);
        //            NextLine:
        //                ;
        //            }

        //            string args2 = "，";
        //            string args3 = ", ";
        //            ReplaceString(line_buf, args2, args3);
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
        //                ret = GetVersionEx(vinfo);
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

        // XXX ヒットを配列にして返すのがよさそう
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
            while (ch == "弱" | ch == "効" | ch == "剋")
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
                    if (c != "弱" & c != "効" & c != "剋")
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
