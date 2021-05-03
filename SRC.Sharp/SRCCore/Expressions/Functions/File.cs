using SRCCore.Lib;

namespace SRCCore.Expressions.Functions
{
    //ファイル処理関数
    // TODO Impl 当面は実装しない
    //Dirファイルの存在判定
    //EOFファイルの末尾か判定
    //LoadFileDialog読み出し用ファイルの選択ダイアログを表示
    //SaveFileDialog書き込み用ファイルの選択ダイアログを表示

    public class Dir : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Dir
            //                        CallFunctionRet = ValueType.StringType;
            //                        switch (pcount)
            //                        {
            //                            case 2:
            //                                {
            //                                    fname = GetValueAsString(@params[1], is_term[1]);

            //                                    // フルパス指定でなければシナリオフォルダを起点に検索
            //                                    if (Strings.Mid(fname, 2, 1) != ":")
            //                                    {
            //                                        fname = SRC.ScenarioPath + fname;
            //                                    }

            //                                    switch (GetValueAsString(@params[2], is_term[2]) ?? "")
            //                                    {
            //                                        case "ファイル":
            //                                            {
            //                                                num = Constants.vbNormal;
            //                                                break;
            //                                            }

            //                                        case "フォルダ":
            //                                            {
            //                                                num = FileAttribute.Directory;
            //                                                break;
            //                                            }
            //                                    }
            //                                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                    str_result = FileSystem.Dir(fname, (FileAttribute)num);
            //                                    if (Strings.Len(str_result) == 0)
            //                                    {
            //                                        return CallFunctionRet;
            //                                    }

            //                                    // ファイル属性チェック用に検索パスを作成
            //                                    dir_path = fname;
            //                                    if (num == FileAttribute.Directory)
            //                                    {
            //                                        i = GeneralLib.InStr2(fname, @"\");
            //                                        if (i > 0)
            //                                        {
            //                                            dir_path = Strings.Left(fname, i);
            //                                        }
            //                                    }

            //                                    // 単一ファイルの検索？
            //                                    if (Strings.InStr(fname, "*") == 0)
            //                                    {
            //                                        // フォルダの検索の場合は見つかったファイルがフォルダ
            //                                        // かどうかチェックする
            //                                        if (num == FileAttribute.Directory)
            //                                        {
            //                                            if ((FileSystem.GetAttr(dir_path + str_result) & num) == 0)
            //                                            {
            //                                                str_result = "";
            //                                            }
            //                                        }

            //                                        return CallFunctionRet;
            //                                    }

            //                                    if (str_result == ".")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    if (str_result == "..")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    // 検索されたファイル一覧を作成
            //                                    dir_list = new string[1];
            //                                    if (num == FileAttribute.Directory)
            //                                    {
            //                                        while (Strings.Len(str_result) > 0)
            //                                        {
            //                                            // フォルダの検索の場合は見つかったファイルがフォルダ
            //                                            // かどうかチェックする
            //                                            if ((FileSystem.GetAttr(dir_path + str_result) & num) != 0)
            //                                            {
            //                                                Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
            //                                                dir_list[Information.UBound(dir_list)] = str_result;
            //                                            }
            //                                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                            str_result = FileSystem.Dir();
            //                                        }
            //                                    }
            //                                    else
            //                                    {
            //                                        while (Strings.Len(str_result) > 0)
            //                                        {
            //                                            Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
            //                                            dir_list[Information.UBound(dir_list)] = str_result;
            //                                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                            str_result = FileSystem.Dir();
            //                                        }
            //                                    }

            //                                    if (Information.UBound(dir_list) > 0)
            //                                    {
            //                                        str_result = dir_list[1];
            //                                        dir_index = 2;
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = "";
            //                                        dir_index = 1;
            //                                    }

            //                                    break;
            //                                }

            //                            case 1:
            //                                {
            //                                    fname = GetValueAsString(@params[1], is_term[1]);

            //                                    // フルパス指定でなければシナリオフォルダを起点に検索
            //                                    if (Strings.Mid(fname, 2, 1) != ":")
            //                                    {
            //                                        fname = SRC.ScenarioPath + fname;
            //                                    }

            //                                    // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                    str_result = FileSystem.Dir(fname, FileAttribute.Directory);
            //                                    if (Strings.Len(str_result) == 0)
            //                                    {
            //                                        return CallFunctionRet;
            //                                    }

            //                                    // 単一ファイルの検索？
            //                                    if (Strings.InStr(fname, "*") == 0)
            //                                    {
            //                                        return CallFunctionRet;
            //                                    }

            //                                    if (str_result == ".")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    if (str_result == "..")
            //                                    {
            //                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    // 検索されたファイル一覧を作成
            //                                    dir_list = new string[1];
            //                                    while (Strings.Len(str_result) > 0)
            //                                    {
            //                                        Array.Resize(dir_list, Information.UBound(dir_list) + 1 + 1);
            //                                        dir_list[Information.UBound(dir_list)] = str_result;
            //                                        // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
            //                                        str_result = FileSystem.Dir();
            //                                    }

            //                                    if (Information.UBound(dir_list) > 0)
            //                                    {
            //                                        str_result = dir_list[1];
            //                                        dir_index = 2;
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = "";
            //                                        dir_index = 1;
            //                                    }

            //                                    break;
            //                                }

            //                            case 0:
            //                                {
            //                                    if (dir_index <= Information.UBound(dir_list))
            //                                    {
            //                                        str_result = dir_list[dir_index];
            //                                        dir_index = (dir_index + 1);
            //                                    }
            //                                    else
            //                                    {
            //                                        str_result = "";
            //                                    }

            //                                    break;
            //                                }
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

    public class EOF : AFunction
    {
        protected override ValueType InvokeInternal(SRC SRC, ValueType etype, string[] @params, int pcount, bool[] is_term, out string str_result, out double num_result)
        {
            str_result = "";
            num_result = 0d;

            // TODO Impl Eof
            //                        if (etype == ValueType.StringType)
            //                        {
            //                            if (FileSystem.EOF(GetValueAsLong(@params[1], is_term[1])))
            //                            {
            //                                str_result = "1";
            //                            }
            //                            else
            //                            {
            //                                str_result = "0";
            //                            }

            //                            CallFunctionRet = ValueType.StringType;
            //                        }
            //                        else
            //                        {
            //                            if (FileSystem.EOF(GetValueAsLong(@params[1], is_term[1])))
            //                            {
            //                                num_result = 1d;
            //                            }

            //                            CallFunctionRet = ValueType.NumericType;
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