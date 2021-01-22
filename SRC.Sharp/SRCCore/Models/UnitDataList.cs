using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class UnitDataList
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 全ユニットデータを管理するリストのクラス

        // ユニットデータ用コレクション
        private Collection colUnitDataList = new Collection();

        // ID作成用変数
        private int IDNum;

        // クラスの初期化
        // UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Initialize_Renamed()
        {
            var ud = new UnitData();
            ud.Name = "ステータス表示用ダミーユニット";
            ud.Nickname = "ユニット無し";
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.Bitmap = ".bmp";
            string argfdef = "ダミーユニット=システム用非表示能力";
            ud.AddFeature(ref argfdef);
            colUnitDataList.Add(ud, ud.Name);
        }

        public UnitDataList() : base()
        {
            Class_Initialize_Renamed();
        }

        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            {
                var withBlock = colUnitDataList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colUnitDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colUnitDataList = null;
        }

        ~UnitDataList()
        {
            Class_Terminate_Renamed();
        }

        // ユニットデータリストにデータを追加
        public UnitData Add(ref string uname)
        {
            UnitData AddRet = default;
            var ud = new UnitData();
            ud.Name = uname;
            colUnitDataList.Add(ud, uname);
            IDNum = IDNum + 1;
            ud.ID = IDNum;
            AddRet = ud;
            return AddRet;
        }

        // ユニットデータリストに登録されているデータ数
        public short Count()
        {
            short CountRet = default;
            CountRet = (short)colUnitDataList.Count;
            return CountRet;
        }

        // ユニットデータリストからデータを削除
        public void Delete(ref object Index)
        {
            colUnitDataList.Remove(Index);
        }

        // ユニットデータリストからデータを取り出す
        public UnitData Item(ref object Index)
        {
            UnitData ItemRet = default;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2372


            Input:
                    On Error GoTo ErrorHandler

             */
            ItemRet = (UnitData)colUnitDataList[Index];
            return ItemRet;
        ErrorHandler:
            ;

            // UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            ItemRet = null;
        }

        // ユニットデータリストに登録されているか？
        public bool IsDefined(ref object Index)
        {
            bool IsDefinedRet = default;
            UnitData dummy;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2841


            Input:

                    On Error GoTo ErrorHandler

             */
            dummy = (UnitData)colUnitDataList[Index];
            IsDefinedRet = true;
            return IsDefinedRet;
        ErrorHandler:
            ;
            IsDefinedRet = false;
        }

        // データファイル fname からデータをロード
        public void Load(ref string fname)
        {
            short FileNumber;
            short j, i, k;
            int n, line_num;
            string buf, line_buf = default, buf2;
            short ret;
            UnitData ud;
            WeaponData wd;
            AbilityData sd;
            string wname, sname;
            string data_name;
            string err_msg;
            bool in_quote;
            short comma_num;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 3473


            Input:

                    On Error GoTo ErrorHandler

             */
            FileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read);
            line_num = 0;
            while (true)
            {
                data_name = "";

                // 空行をスキップ
                do
                {
                    if (FileSystem.EOF((int)FileNumber))
                    {
                        FileSystem.FileClose((int)FileNumber);
                        return;
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }
                while (Strings.Len(line_buf) == 0);

                // 名称
                ret = (short)Strings.InStr(line_buf, ",");
                if ((int)ret > 0)
                {
                    data_name = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                    buf = Strings.Mid(line_buf, (int)ret + 1);
                }
                else
                {
                    data_name = line_buf;
                    buf = "";
                }

                if (Strings.InStr(data_name, " ") > 0)
                {
                    err_msg = "名称に半角スペースは使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4599


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
                {
                    err_msg = "名称に全角括弧は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4799


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "\"") > 0)
                {
                    err_msg = "名称に\"は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4932


                    Input:
                                    Error(0)

                     */
                }

                object argIndex2 = (object)data_name;
                if (IsDefined(ref argIndex2))
                {
                    object argIndex1 = (object)data_name;
                    ud = Item(ref argIndex1);
                    ud.Clear();
                }
                else
                {
                    ud = Add(ref data_name);
                }
                // 読み仮名
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret > 0)
                {
                    err_msg = "読み仮名の後に余分なデータが指定されています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5247


                    Input:
                                        Error(0)

                     */
                }

                ud.KanaName = buf;

                // 愛称, 読み仮名, ユニットクラス, パイロット数, アイテム数
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

                // 書式チェックのため、コンマの数を数えておく
                comma_num = (short)0;
                var loopTo = (short)Strings.Len(line_buf);
                for (i = (short)1; i <= loopTo; i++)
                {
                    if (Strings.Mid(line_buf, (int)i, 1) == ",")
                    {
                        comma_num = (short)((int)comma_num + 1);
                    }
                }

                if ((int)comma_num < 3)
                {
                    err_msg = "設定に抜けがあります。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5733


                    Input:
                                        Error(0)

                     */
                }
                else if ((int)comma_num > 4)
                {
                    err_msg = "余分な「,」があります。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5811


                    Input:
                                        Error(0)

                     */
                }

                // 愛称
                if (Strings.Len(line_buf) == 0)
                {
                    err_msg = "愛称の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5953


                    Input:
                                        Error(0)

                     */
                }

                ret = (short)Strings.InStr(line_buf, ",");
                buf2 = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                buf = Strings.Mid(line_buf, (int)ret + 1);
                ud.Nickname = buf2;

                // 読み仮名
                if ((int)comma_num == 4)
                {
                    ret = (short)Strings.InStr(buf, ",");
                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    ud.KanaName = buf2;
                }
                else
                {
                    string argstr_Renamed = ud.Nickname;
                    ud.KanaName = GeneralLib.StrToHiragana(ref argstr_Renamed);
                    ud.Nickname = argstr_Renamed;
                }

                // ユニットクラス
                ret = (short)Strings.InStr(buf, ",");
                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (!Information.IsNumeric(buf2))
                {
                    ud.Class_Renamed = buf2;
                }
                else
                {
                    string argmsg = "ユニットクラスの設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    ud.Class_Renamed = "汎用";
                }

                // パイロット数
                ret = (short)Strings.InStr(buf, ",");
                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Strings.Left(buf2, 1) != "(")
                {
                    if (Information.IsNumeric(buf2))
                    {
                        ud.PilotNum = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                    }
                    else
                    {
                        string argmsg1 = "パイロット数の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg1, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        ud.PilotNum = (short)1;
                    }

                    if ((int)ud.PilotNum < 1)
                    {
                        string argmsg2 = "パイロット数の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg2, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        ud.PilotNum = (short)1;
                    }
                }
                else
                {
                    if (Strings.Right(buf2, 1) != ")")
                    {
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 8044


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Mid(buf2, 2, Strings.Len(buf2) - 2);
                    if (Information.IsNumeric(buf2))
                    {
                        ud.PilotNum = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                    }
                    else
                    {
                        string argmsg3 = "パイロット数の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg3, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        ud.PilotNum = (short)1;
                    }

                    if ((int)ud.PilotNum < 1)
                    {
                        string argmsg4 = "パイロット数の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg4, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        ud.PilotNum = (short)1;
                    }

                    ud.PilotNum = -ud.PilotNum;
                }

                // アイテム数
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    err_msg = "アイテム数の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 8851


                    Input:
                                        Error(0)

                     */
                }

                if (Information.IsNumeric(buf))
                {
                    ud.ItemNum = (short)GeneralLib.MinLng(Conversions.ToInteger(buf), 99);
                }
                else
                {
                    string argmsg5 = "アイテム数の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg5, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    ud.ItemNum = (short)4;
                }

                // 移動可能地形, 移動力, サイズ, 修理費, 経験値
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

                // 移動可能地形
                ret = (short)Strings.InStr(line_buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "移動力の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 9405


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                buf = Strings.Mid(line_buf, (int)ret + 1);
                if (!Information.IsNumeric(buf2))
                {
                    ud.Transportation = buf2;
                }
                else
                {
                    string argmsg6 = "移動可能地形の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg6, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    ud.Transportation = "陸";
                }

                // 移動力
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "サイズの設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 10007


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    ud.Speed = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                }
                else
                {
                    string argmsg7 = "移動力の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg7, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                }

                // サイズ
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "経験値の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 10600


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                switch (buf2 ?? "")
                {
                    case "XL":
                    case "LL":
                    case "L":
                    case "M":
                    case "S":
                    case "SS":
                        {
                            ud.Size = buf2;
                            break;
                        }

                    default:
                        {
                            string argmsg8 = "サイズの設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg8, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                            ud.Size = "M";
                            break;
                        }
                }

                // 修理費
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "経験値の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 11173


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    ud.Value = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999999);
                }
                else
                {
                    string argmsg9 = "修理費の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg9, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                }

                // 経験値
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    err_msg = "経験値の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 11807


                    Input:
                                        Error(0)

                     */
                }

                if (Information.IsNumeric(buf))
                {
                    ud.ExpValue = (short)GeneralLib.MinLng(Conversions.ToInteger(buf), 9999);
                }
                else
                {
                    string argmsg10 = "経験値の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg10, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                }

                // 特殊能力データ
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                if (line_buf == "特殊能力なし")
                {
                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }
                else if (line_buf == "特殊能力")
                {
                    // 新形式による特殊能力表記
                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                    buf = line_buf;
                    i = (short)0;
                    while (true)
                    {
                        i = (short)((int)i + 1);
                        ret = (short)0;
                        in_quote = false;
                        var loopTo1 = (short)Strings.Len(buf);
                        for (j = (short)1; j <= loopTo1; j++)
                        {
                            switch (Strings.Mid(buf, (int)j, 1) ?? "")
                            {
                                case ",":
                                    {
                                        if (!in_quote)
                                        {
                                            ret = j;
                                            break;
                                        }

                                        break;
                                    }

                                case "\"":
                                    {
                                        in_quote = !in_quote;
                                        break;
                                    }
                            }
                        }

                        if ((int)ret > 0)
                        {
                            buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                            if ((int)j == 1)
                            {
                                if (Information.IsNumeric(buf2))
                                {
                                    break;
                                }
                            }

                            buf = Strings.Trim(Strings.Mid(buf, (int)ret + 1));
                        }
                        else
                        {
                            buf2 = buf;
                            buf = "";
                        }

                        if (Information.IsNumeric(buf2))
                        {
                            break;
                        }
                        else if (string.IsNullOrEmpty(buf2) | Information.IsNumeric(buf2))
                        {
                            string argmsg11 = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i) + "番目の特殊能力の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg11, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        }
                        else
                        {
                            ud.AddFeature(ref buf2);
                        }

                        if (string.IsNullOrEmpty(buf))
                        {
                            GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                            buf = line_buf;
                            i = (short)0;
                        }
                    }
                }
                else if (Strings.InStr(line_buf, "特殊能力,") == 1)
                {
                    // 旧形式による特殊能力表記
                    buf = Strings.Mid(line_buf, 6);
                    ret = (short)0;
                    in_quote = false;
                    var loopTo2 = (short)Strings.Len(buf);
                    for (k = (short)1; k <= loopTo2; k++)
                    {
                        switch (Strings.Mid(buf, (int)k, 1) ?? "")
                        {
                            case ",":
                                {
                                    if (!in_quote)
                                    {
                                        ret = k;
                                        break;
                                    }

                                    break;
                                }

                            case "\"":
                                {
                                    in_quote = !in_quote;
                                    break;
                                }
                        }
                    }

                    i = (short)0;
                    while ((int)ret > 0)
                    {
                        i = (short)((int)i + 1);
                        buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                        buf = Strings.Mid(buf, (int)ret + 1);
                        ret = (short)Strings.InStr(buf, ",");
                        if (!string.IsNullOrEmpty(buf2))
                        {
                            ud.AddFeature(ref buf2);
                        }
                        else
                        {
                            string argmsg12 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i) + "番目の特殊能力の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg12, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        }
                    }

                    i = (short)((int)i + 1);
                    buf2 = Strings.Trim(buf);
                    if (!string.IsNullOrEmpty(buf2))
                    {
                        ud.AddFeature(ref buf2);
                    }
                    else
                    {
                        string argmsg13 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i) + "番目の特殊能力の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg13, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }
                else
                {
                    err_msg = "特殊能力の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 15468


                    Input:
                                        Error(0)

                     */
                }

                // 最大ＨＰ
                ret = (short)Strings.InStr(line_buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "最大ＥＮの設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 15636


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                buf = Strings.Mid(line_buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    ud.HP = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999999);
                }
                else
                {
                    string argmsg14 = "最大ＨＰの設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg14, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    ud.HP = 1000;
                }

                // 最大ＥＮ
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "装甲の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 16259


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    ud.EN = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    string argmsg15 = "最大ＥＮの設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg15, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    ud.EN = (short)100;
                }

                // 装甲
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "運動性の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 16867


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    ud.Armor = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99999);
                }
                else
                {
                    string argmsg16 = "装甲の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg16, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                }

                // 運動性
                buf2 = Strings.Trim(buf);
                if (Strings.Len(buf2) == 0)
                {
                    err_msg = "運動性の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 17500


                    Input:
                                        Error(0)

                     */
                }

                if (Information.IsNumeric(buf2))
                {
                    ud.Mobility = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    string argmsg17 = "運動性の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg17, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                }

                // 地形適応, ビットマップ
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

                // 地形適応
                ret = (short)Strings.InStr(line_buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "ビットマップの設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 18025


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                buf = Strings.Mid(line_buf, (int)ret + 1);
                if (Strings.Len(buf2) == 4)
                {
                    ud.Adaption = buf2;
                }
                else
                {
                    string argmsg18 = "地形適応の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg18, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    ud.Adaption = "AAAA";
                }

                // ビットマップ
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    err_msg = "ビットマップの設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 18648


                    Input:
                                        Error(0)

                     */
                }

                if (Strings.LCase(Strings.Right(buf, 4)) == ".bmp")
                {
                    ud.Bitmap = buf;
                }
                else
                {
                    string argmsg19 = "ビットマップの設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg19, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    ud.IsBitmapMissing = true;
                }

                if (FileSystem.EOF((int)FileNumber))
                {
                    FileSystem.FileClose((int)FileNumber);
                    return;
                }

                // 武器データ
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                while (Strings.Len(line_buf) > 0 & line_buf != "===")
                {
                    // 武器名
                    ret = (short)Strings.InStr(line_buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = "武器データの終りには空行を置いてください。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 19459


                        Input:
                                                Error(0)

                         */
                    }

                    wname = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                    buf = Strings.Mid(line_buf, (int)ret + 1);
                    if (string.IsNullOrEmpty(wname))
                    {
                        err_msg = "武器名の設定が間違っています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 19747


                        Input:
                                                Error(0)

                         */
                    }

                    // 武器を登録
                    wd = ud.AddWeapon(ref wname);

                    // 攻撃力
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = wname + "の最小射程が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 19971


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        wd.Power = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99999);
                    }
                    else if (buf == "-")
                    {
                        wd.Power = 0;
                    }
                    else
                    {
                        string argmsg20 = wname + "の攻撃力の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg20, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr = GeneralLib.LIndex(ref buf2, (short)1);
                            wd.Power = GeneralLib.StrToLng(ref argexpr);
                        }
                    }

                    // 最小射程
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = wname + "の最大射程の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 20894


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        wd.MinRange = Conversions.ToShort(buf2);
                    }
                    else
                    {
                        string argmsg21 = wname + "の最小射程の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg21, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        wd.MinRange = (short)1;
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr1 = GeneralLib.LIndex(ref buf2, (short)1);
                            wd.MinRange = (short)GeneralLib.StrToLng(ref argexpr1);
                        }
                    }

                    // 最大射程
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = wname + "の命中率の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 21757


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        wd.MaxRange = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                    }
                    else
                    {
                        string argmsg22 = wname + "の最大射程の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg22, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        wd.MaxRange = (short)1;
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr2 = GeneralLib.LIndex(ref buf2, (short)1);
                            wd.MaxRange = (short)GeneralLib.StrToLng(ref argexpr2);
                        }
                    }

                    // 命中率
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = wname + "の弾数の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 22655


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        n = Conversions.ToInteger(buf2);
                        if (n > 999)
                        {
                            n = 999;
                        }
                        else if (n < -999)
                        {
                            n = -999;
                        }

                        wd.Precision = (short)n;
                    }
                    else
                    {
                        string argmsg23 = wname + "の命中率の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg23, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr3 = GeneralLib.LIndex(ref buf2, (short)1);
                            wd.Precision = (short)GeneralLib.StrToLng(ref argexpr3);
                        }
                    }

                    // 弾数
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = wname + "の消費ＥＮの設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 23605


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (buf2 != "-")
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            wd.Bullet = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                        }
                        else
                        {
                            string argmsg24 = wname + "の弾数の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg24, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr4 = GeneralLib.LIndex(ref buf2, (short)1);
                                wd.Bullet = (short)GeneralLib.StrToLng(ref argexpr4);
                            }
                        }
                    }

                    // 消費ＥＮ
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = wname + "の必要気力が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 24522


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (buf2 != "-")
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            wd.ENConsumption = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 999);
                        }
                        else
                        {
                            string argmsg25 = wname + "の消費ＥＮの設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg25, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr5 = GeneralLib.LIndex(ref buf2, (short)1);
                                wd.ENConsumption = (short)GeneralLib.StrToLng(ref argexpr5);
                            }
                        }
                    }

                    // 必要気力
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = wname + "の地形適応が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 25456


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (buf2 != "-")
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            n = Conversions.ToInteger(buf2);
                            if (n > 1000)
                            {
                                n = 1000;
                            }
                            else if (n < 0)
                            {
                                n = 0;
                            }

                            wd.NecessaryMorale = (short)n;
                        }
                        else
                        {
                            string argmsg26 = wname + "の必要気力の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg26, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr6 = GeneralLib.LIndex(ref buf2, (short)1);
                                wd.NecessaryMorale = (short)GeneralLib.StrToLng(ref argexpr6);
                            }
                        }
                    }

                    // 地形適応
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = wname + "のクリティカル率が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 26471


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (Strings.Len(buf2) == 4)
                    {
                        wd.Adaption = buf2;
                    }
                    else
                    {
                        string argmsg27 = wname + "の地形適応の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg27, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        wd.Adaption = "----";
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            wd.Adaption = GeneralLib.LIndex(ref buf2, (short)1);
                        }
                    }

                    // クリティカル率
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = wname + "の武器属性が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 27289


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        n = Conversions.ToInteger(buf2);
                        if (n > 999)
                        {
                            n = 999;
                        }
                        else if (n < -999)
                        {
                            n = -999;
                        }

                        wd.Critical = (short)n;
                    }
                    else
                    {
                        string argmsg28 = wname + "のクリティカル率の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg28, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr7 = GeneralLib.LIndex(ref buf2, (short)1);
                            wd.Critical = (short)GeneralLib.StrToLng(ref argexpr7);
                        }
                    }

                    // 武器属性
                    buf = Strings.Trim(buf);
                    if (Strings.Len(buf) == 0)
                    {
                        string argmsg29 = wname + "の武器属性の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg29, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    }

                    if (Strings.Right(buf, 1) == ")")
                    {
                        // 必要技能
                        ret = (short)Strings.InStr(buf, "> ");
                        if ((int)ret > 0)
                        {
                            if ((int)ret > 0)
                            {
                                wd.NecessarySkill = Strings.Mid(buf, (int)ret + 2);
                                buf = Strings.Trim(Strings.Left(buf, (int)ret + 1));
                                ret = (short)Strings.InStr(wd.NecessarySkill, "(");
                                wd.NecessarySkill = Strings.Mid(wd.NecessarySkill, (int)ret + 1, Strings.Len(wd.NecessarySkill) - (int)ret - 1);
                            }
                        }
                        else
                        {
                            ret = (short)Strings.InStr(buf, "(");
                            if ((int)ret > 0)
                            {
                                wd.NecessarySkill = Strings.Trim(Strings.Mid(buf, (int)ret + 1, Strings.Len(buf) - (int)ret - 1));
                                buf = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                            }
                        }
                    }

                    if (Strings.Right(buf, 1) == ">")
                    {
                        // 必要条件
                        ret = (short)Strings.InStr(buf, "<");
                        if ((int)ret > 0)
                        {
                            wd.NecessaryCondition = Strings.Trim(Strings.Mid(buf, (int)ret + 1, Strings.Len(buf) - (int)ret - 1));
                            buf = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                        }
                    }

                    wd.Class_Renamed = buf;
                    if (wd.Class_Renamed == "-")
                    {
                        wd.Class_Renamed = "";
                    }

                    if (Strings.InStr(wd.Class_Renamed, "Lv") > 0)
                    {
                        string argmsg30 = wname + "の属性のレベル指定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg30, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    }

                    if (FileSystem.EOF((int)FileNumber))
                    {
                        FileSystem.FileClose((int)FileNumber);
                        return;
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }

                if (line_buf != "===")
                {
                    goto SkipRest;
                }

                // アビリティデータ
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                while (Strings.Len(line_buf) > 0)
                {
                    // アビリティ名
                    ret = (short)Strings.InStr(line_buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = "アビリティデータの終りに空行を置いてください。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 30928


                        Input:
                                                Error(0)

                         */
                    }

                    sname = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                    buf = Strings.Mid(line_buf, (int)ret + 1);
                    if (string.IsNullOrEmpty(sname))
                    {
                        err_msg = "アビリティ名の設定が間違っています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 31219


                        Input:
                                                Error(0)

                         */
                    }

                    // アビリティを登録
                    sd = ud.AddAbility(ref sname);

                    // 効果
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = sname + "の射程の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 31447


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    sd.SetEffect(ref buf2);

                    // 射程
                    sd.MinRange = (short)0;
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = sname + "の回数の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 31850


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        sd.MaxRange = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                    }
                    else if (buf2 == "-")
                    {
                        sd.MaxRange = (short)0;
                    }
                    else
                    {
                        string argmsg31 = sname + "の射程の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg31, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr8 = GeneralLib.LIndex(ref buf2, (short)1);
                            sd.MaxRange = (short)GeneralLib.StrToLng(ref argexpr8);
                        }
                    }

                    // 回数
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = sname + "の消費ＥＮの設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 32777


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (buf2 != "-")
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            sd.Stock = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                        }
                        else
                        {
                            string argmsg32 = sname + "の回数の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg32, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr9 = GeneralLib.LIndex(ref buf2, (short)1);
                                sd.Stock = (short)GeneralLib.StrToLng(ref argexpr9);
                            }
                        }
                    }

                    // 消費ＥＮ
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = sname + "の必要気力の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 33695


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (buf2 != "-")
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            sd.ENConsumption = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 999);
                        }
                        else
                        {
                            string argmsg33 = sname + "の消費ＥＮの設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg33, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr10 = GeneralLib.LIndex(ref buf2, (short)1);
                                sd.ENConsumption = (short)GeneralLib.StrToLng(ref argexpr10);
                            }
                        }
                    }

                    // 必要気力
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = sname + "のアビリティ属性の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 34635


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (buf2 != "-")
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            n = Conversions.ToInteger(buf2);
                            if (n > 1000)
                            {
                                n = 1000;
                            }
                            else if (n < 0)
                            {
                                n = 0;
                            }

                            sd.NecessaryMorale = (short)n;
                        }
                        else
                        {
                            string argmsg34 = sname + "の必要気力の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg34, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr11 = GeneralLib.LIndex(ref buf2, (short)1);
                                sd.NecessaryMorale = (short)GeneralLib.StrToLng(ref argexpr11);
                            }
                        }
                    }

                    // アビリティ属性
                    buf = Strings.Trim(buf);
                    if (Strings.Len(buf) == 0)
                    {
                        string argmsg35 = sname + "のアビリティ属性の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg35, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    }

                    if (Strings.Right(buf, 1) == ")")
                    {
                        // 必要技能
                        ret = (short)Strings.InStr(buf, "> ");
                        if ((int)ret > 0)
                        {
                            if ((int)ret > 0)
                            {
                                sd.NecessarySkill = Strings.Mid(buf, (int)ret + 2);
                                buf = Strings.Trim(Strings.Left(buf, (int)ret + 1));
                                ret = (short)Strings.InStr(sd.NecessarySkill, "(");
                                sd.NecessarySkill = Strings.Mid(sd.NecessarySkill, (int)ret + 1, Strings.Len(sd.NecessarySkill) - (int)ret - 1);
                            }
                        }
                        else
                        {
                            ret = (short)Strings.InStr(buf, "(");
                            if ((int)ret > 0)
                            {
                                sd.NecessarySkill = Strings.Trim(Strings.Mid(buf, (int)ret + 1, Strings.Len(buf) - (int)ret - 1));
                                buf = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                            }
                        }
                    }

                    if (Strings.Right(buf, 1) == ">")
                    {
                        // 必要条件
                        ret = (short)Strings.InStr(buf, "<");
                        if ((int)ret > 0)
                        {
                            sd.NecessaryCondition = Strings.Trim(Strings.Mid(buf, (int)ret + 1, Strings.Len(buf) - (int)ret - 1));
                            buf = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                        }
                    }

                    sd.Class_Renamed = buf;
                    if (sd.Class_Renamed == "-")
                    {
                        sd.Class_Renamed = "";
                    }

                    if (Strings.InStr(sd.Class_Renamed, "Lv") > 0)
                    {
                        string argmsg36 = sname + "の属性のレベル指定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg36, ref fname, (short)line_num, ref line_buf, ref ud.Name);
                    }

                    if (FileSystem.EOF((int)FileNumber))
                    {
                        FileSystem.FileClose((int)FileNumber);
                        return;
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }

            SkipRest:
                ;
            }

        ErrorHandler:
            ;

            // エラー処理
            if (line_num == 0)
            {
                string argmsg37 = fname + "が開けません。";
                GUI.ErrorMessage(ref argmsg37);
            }
            else
            {
                FileSystem.FileClose((int)FileNumber);
                GUI.DataErrorMessage(ref err_msg, ref fname, (short)line_num, ref line_buf, ref data_name);
            }

            SRC.TerminateSRC();
        }
    }
}