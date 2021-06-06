using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Expressions.Functions
{
    //ファイル処理関数
    //Dirファイルの存在判定
    //EOFファイルの末尾か判定
    //LoadFileDialog読み出し用ファイルの選択ダイアログを表示
    //SaveFileDialog書き込み用ファイルの選択ダイアログを表示

    public class Dir : AFunction
    {
        private IEnumerator<string> _lastResults;

        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            if (pcount > 0)
            {
                var fname = SRC.Expression.GetValueAsString(@params[1], is_term[1]);
                // フルパス指定でなければシナリオフォルダを起点に検索
                if (!SRC.FileSystem.IsAbsolutePath(fname))
                {
                    fname = SRC.FileSystem.PathCombine(SRC.ScenarioPath, fname);
                }

                var searchOption = EntryOption.All;
                if (pcount > 2)
                {
                    switch (SRC.Expression.GetValueAsString(@params[2], is_term[2]) ?? "")
                    {
                        case "ファイル":
                            searchOption = EntryOption.File;
                            break;

                        case "フォルダ":
                            searchOption = EntryOption.Directory;
                            break;
                    }
                }

                _lastResults = SRC.FileSystem.GetFileSystemEntries(
                    Path.GetDirectoryName(fname), Path.GetFileName(fname), searchOption)
                    .GetEnumerator();
            }

            if (_lastResults != null && _lastResults.MoveNext())
            {
                str_result = _lastResults.Current;
            }
            return ValueType.StringType;
        }
    }

    public class EOF : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            var f = SRC.FileHandleManager.Get(SRC.Expression.GetValueAsLong(@params[1], is_term[1]));

            if (f.Reader.EndOfStream)
            {
                str_result = "1";
                num_result = 1d;
            }
            else
            {
                str_result = "0";
                num_result = 0d;
            }

            if (etype == ValueType.StringType)
            {
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }


    public class Loadfiledialog : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Loadfiledialog
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    str_result = FileDialog.LoadFileDialog("ファイルを開く", SRC.ScenarioPath, "", 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: GetValueAsString(@params[1], is_term[1])2, fsuffix2: GetValueAsString(@params[2], is_term[2])2, ftype3: GetValueAsString(@params[1], is_term[1])3, fsuffix3: GetValueAsString(@params[2], is_term[2])3);
            //                                    break;
            //                                }

            //                            case 3:
            //                                {
            //                                    str_result = FileDialog.LoadFileDialog("ファイルを開く", SRC.ScenarioPath, GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: ""1, fsuffix2: ""1, ftype3: ""1, fsuffix3: ""1);
            //                                    break;
            //                                }

            //                            case 4:
            //                                {
            //                                    str_result = FileDialog.LoadFileDialog("ファイルを開く", SRC.ScenarioPath + GetValueAsString(@params[4], is_term[4]), GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;

            //                        // 本当はこれだけでいいはずだけど……
            //                        if (Strings.InStr(str_result, SRC.ScenarioPath) > 0)
            //                        {
            //                            str_result = Strings.Mid(str_result, Strings.Len(SRC.ScenarioPath) + 1);
            //                            return CallFunctionRet;
            //                        }

            //                        // フルパス指定ならここで終了
            //                        if (Strings.Right(Strings.Left(str_result, 3), 2) == @":\")
            //                        {
            //                            str_result = "";
            //                            return CallFunctionRet;
            //                        }

            //                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                        while (string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + str_result, FileAttribute.Normal)))
            //                        {
            //                            if (Strings.InStr(str_result, @"\") == 0)
            //                            {
            //                                // シナリオフォルダ外のファイルだった
            //                                str_result = "";
            //                                return CallFunctionRet;
            //                            }

            //                            str_result = Strings.Mid(str_result, Strings.InStr(str_result, @"\") + 1);
            //                        }

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }

    public class Savefiledialog : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Savefiledialog
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    str_result = FileDialog.SaveFileDialog("ファイルを保存", SRC.ScenarioPath, "", 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }

            //                            case 3:
            //                                {
            //                                    str_result = FileDialog.SaveFileDialog("ファイルを保存", SRC.ScenarioPath, GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }

            //                            case 4:
            //                                {
            //                                    str_result = FileDialog.SaveFileDialog("ファイルを保存", SRC.ScenarioPath + GetValueAsString(@params[4], is_term[4]), GetValueAsString(@params[3], is_term[3]), 2, GetValueAsString(@params[1], is_term[1]), GetValueAsString(@params[2], is_term[2]), ftype2: "", fsuffix2: "", ftype3: "", fsuffix3: "");
            //                                    break;
            //                                }
            //                        }

            //                        CallFunctionRet = ValueType.StringType;

            //                        // 本当はこれだけでいいはずだけど……
            //                        if (Strings.InStr(str_result, SRC.ScenarioPath) > 0)
            //                        {
            //                            str_result = Strings.Mid(str_result, Strings.Len(SRC.ScenarioPath) + 1);
            //                            return CallFunctionRet;
            //                        }

            //                        if (Strings.InStr(str_result, @"\") == 0)
            //                        {
            //                            return CallFunctionRet;
            //                        }

            //                        var loopTo13 = Strings.Len(str_result);
            //                        for (i = 1; i <= loopTo13; i++)
            //                        {
            //                            if (Strings.Mid(str_result, Strings.Len(str_result) - i + 1, 1) == @"\")
            //                            {
            //                                break;
            //                            }
            //                        }

            //                        buf = Strings.Left(str_result, Strings.Len(str_result) - i);
            //                        str_result = Strings.Mid(str_result, Strings.Len(str_result) - i + 2);
            //                        while (Strings.InStr(buf, @"\") > 0)
            //                        {
            //                            buf = Strings.Mid(buf, Strings.InStr(buf, @"\") + 1);
            //                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                            if (!string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + buf, FileAttribute.Directory)))
            //                            {
            //                                str_result = buf + @"\" + str_result;
            //                                return CallFunctionRet;
            //                            }
            //                        }

            //                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                        if (!string.IsNullOrEmpty(FileSystem.Dir(SRC.ScenarioPath + buf, FileAttribute.Directory)))
            //                        {
            //                            str_result = buf + @"\" + str_result;
            //                        }

            if (etype == ValueType.StringType)
            {
                str_result = GeneralLib.FormatNum(num_result);
                return ValueType.StringType;
            }
            else
            {
                return ValueType.NumericType;
            }
        }
    }
}