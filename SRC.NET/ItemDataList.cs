using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class ItemDataList
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 全アイテムデータを管理するリストのクラス

        // アイテムデータのコレクション
        private Collection colItemDataList = new Collection();


        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            {
                var withBlock = colItemDataList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colItemDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colItemDataList = null;
        }

        ~ItemDataList()
        {
            Class_Terminate_Renamed();
        }

        // アイテムデータリストにデータを追加
        public ItemData Add(ref string new_name)
        {
            ItemData AddRet = default;
            var new_Item_data = new ItemData();
            new_Item_data.Name = new_name;
            colItemDataList.Add(new_Item_data, new_name);
            AddRet = new_Item_data;
            return AddRet;
        }

        // アイテムデータリストに登録されているデータの総数
        public short Count()
        {
            short CountRet = default;
            CountRet = (short)colItemDataList.Count;
            return CountRet;
        }

        // アイテムデータリストから指定したデータを削除
        public void Delete(ref object Index)
        {
            colItemDataList.Remove(Index);
        }

        // アイテムデータリストから指定したデータを取り出す
        public ItemData Item(ref object Index)
        {
            ItemData ItemRet = default;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 1692


            Input:
                    On Error GoTo ErrorHandler

             */
            ItemRet = (ItemData)colItemDataList[Index];
            return ItemRet;
            ErrorHandler:
            ;

            // UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            ItemRet = null;
        }

        // アイテムデータリストに指定したデータが登録されているか？
        public bool IsDefined(ref object Index)
        {
            bool IsDefinedRet = default;
            ItemData dummy;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2169


            Input:

                    On Error GoTo ErrorHandler

             */
            dummy = (ItemData)colItemDataList[Index];
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
            short i, j;
            int n, line_num;
            string buf, line_buf = default, buf2;
            short ret;
            ItemData nd;
            WeaponData wd;
            AbilityData sd;
            string wname, sname;
            string data_name;
            string err_msg;
            bool in_quote;
            short comma_num;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2798


            Input:

                    On Error GoTo ErrorHandler

             */
            FileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read);
            line_num = 0;
            while (true)
            {
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
                if (Strings.InStr(line_buf, ",") > 0)
                {
                    err_msg = "名称の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3539


                    Input:
                                    Error(0)

                     */
                }

                // 名称
                data_name = line_buf;
                if (Strings.InStr(data_name, " ") > 0)
                {
                    err_msg = "名称に半角スペースは使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3718


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
                {
                    err_msg = "名称に全角括弧は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3918


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "\"") > 0)
                {
                    err_msg = "名称に\"は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4051


                    Input:
                                    Error(0)

                     */
                }

                object argIndex2 = (object)data_name;
                if (IsDefined(ref argIndex2))
                {
                    object argIndex1 = (object)data_name;
                    Delete(ref argIndex1);
                }

                nd = Add(ref data_name);
                // 愛称、読み仮名、アイテムクラス、装備個所
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

                if ((int)comma_num < 2)
                {
                    err_msg = "設定に抜けがあります。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4619


                    Input:
                                        Error(0)

                     */
                }
                else if ((int)comma_num > 3)
                {
                    err_msg = "余分な「,」があります。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4697


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
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4839


                    Input:
                                        Error(0)

                     */
                }

                ret = (short)Strings.InStr(line_buf, ",");
                buf2 = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                buf = Strings.Mid(line_buf, (int)ret + 1);
                nd.Nickname = buf2;

                // 読み仮名
                if ((int)comma_num == 3)
                {
                    ret = (short)Strings.InStr(buf, ",");
                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    nd.KanaName = buf2;
                }
                else
                {
                    string argstr_Renamed = nd.Nickname;
                    nd.KanaName = GeneralLib.StrToHiragana(ref argstr_Renamed);
                    nd.Nickname = argstr_Renamed;
                }

                // アイテムクラス
                ret = (short)Strings.InStr(buf, ",");
                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                nd.Class_Renamed = buf2;

                // 装備個所
                buf2 = Strings.Trim(buf);
                if (Strings.Len(buf2) == 0)
                {
                    err_msg = "装備個所の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6015


                    Input:
                                        Error(0)

                     */
                }

                nd.Part = buf2;

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
                            if ((int)i == 1)
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

                        if (!string.IsNullOrEmpty(buf2))
                        {
                            nd.AddFeature(ref buf2);
                        }
                        else
                        {
                            string argmsg = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i) + "番目の特殊能力の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                    for (j = (short)1; j <= loopTo2; j++)
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

                    i = (short)0;
                    while ((int)ret > 0)
                    {
                        i = (short)((int)i + 1);
                        buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                        buf = Strings.Mid(buf, (int)ret + 1);
                        ret = (short)Strings.InStr(buf, ",");
                        if (!string.IsNullOrEmpty(buf2))
                        {
                            nd.AddFeature(ref buf2);
                        }
                        else
                        {
                            string argmsg1 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i) + "番目の特殊能力の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg1, ref fname, (short)line_num, ref line_buf, ref data_name);
                        }
                    }

                    i = (short)((int)i + 1);
                    buf2 = Strings.Trim(buf);
                    if (!string.IsNullOrEmpty(buf2))
                    {
                        nd.AddFeature(ref buf2);
                    }
                    else
                    {
                        string argmsg2 = Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i) + "番目の特殊能力の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg2, ref fname, (short)line_num, ref line_buf, ref data_name);
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }
                else
                {
                    err_msg = "特殊能力の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 9298


                    Input:
                                        Error(0)

                     */
                }

                // 最大ＨＰ修正値, 最大ＥＮ修正値, 装甲修正値, 運動性修正値, 移動力修正値

                // 最大ＨＰ修正値
                ret = (short)Strings.InStr(line_buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "最大ＥＮ修正値の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 9524


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                buf = Strings.Mid(line_buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    nd.HP = Conversions.ToInteger(buf2);
                }
                else
                {
                    string argmsg3 = "最大ＨＰ修正値の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg3, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 最大ＥＮ修正値
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "装甲修正値の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 10099


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    nd.EN = (short)Conversions.ToInteger(buf2);
                }
                else
                {
                    string argmsg4 = "最大ＥＮ修正値の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg4, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 装甲修正値
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "運動性修正値の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 10663


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    nd.Armor = Conversions.ToInteger(buf2);
                }
                else
                {
                    string argmsg5 = "装甲修正値の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg5, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 運動性修正値
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "移動力修正値の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 11229


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    nd.Mobility = (short)Conversions.ToInteger(buf2);
                }
                else
                {
                    string argmsg6 = "運動性修正値の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg6, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 移動力修正値
                buf2 = Strings.Trim(buf);
                if (Strings.Len(buf2) == 0)
                {
                    err_msg = "移動力修正値の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 11837


                    Input:
                                        Error(0)

                     */
                }

                if (Information.IsNumeric(buf2))
                {
                    nd.Speed = (short)Conversions.ToInteger(buf2);
                }
                else
                {
                    string argmsg7 = "移動力修正値の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg7, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                if (FileSystem.EOF((int)FileNumber))
                {
                    FileSystem.FileClose((int)FileNumber);
                    return;
                }

                // 武器データ
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                while (Strings.Len(line_buf) > 0 & Strings.Left(line_buf, 1) != "*" & line_buf != "===")
                {
                    // 武器名
                    ret = (short)Strings.InStr(line_buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = "武器データの終りには空行を置いてください。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 12647


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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 12935


                        Input:
                                                Error(0)

                         */
                    }

                    // 武器を登録
                    wd = nd.AddWeapon(ref wname);

                    // 攻撃力
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = wname + "の最小射程が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 13159


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
                        string argmsg8 = wname + "の攻撃力の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg8, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 14086


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                    buf = Strings.Mid(buf, (int)ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        wd.MinRange = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                    }
                    else
                    {
                        string argmsg9 = wname + "の最小射程の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg9, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 14990


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
                        string argmsg10 = wname + "の最大射程の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg10, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 15892


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
                        string argmsg11 = wname + "の命中率の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg11, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 16846


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
                            string argmsg12 = wname + "の弾数の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg12, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 17767


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
                            string argmsg13 = wname + "の消費ＥＮの設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg13, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 18705


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
                            string argmsg14 = wname + "の必要気力の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg14, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 19724


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
                        string argmsg15 = wname + "の地形適応の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg15, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 20546


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
                        string argmsg16 = wname + "のクリティカル率の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg16, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        string argmsg17 = wname + "の武器属性の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg17, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        string argmsg18 = wname + "の属性のレベル指定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg18, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                while (Strings.Len(line_buf) > 0 & Strings.Left(line_buf, 1) != "*")
                {
                    // アビリティ名
                    ret = (short)Strings.InStr(line_buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = "アビリティデータの終りには空行を置いてください。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 24264


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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 24555


                        Input:
                                                Error(0)

                         */
                    }

                    // アビリティを登録
                    sd = nd.AddAbility(ref sname);

                    // 効果
                    ret = (short)Strings.InStr(buf, ",");
                    if ((int)ret == 0)
                    {
                        err_msg = sname + "の射程の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 24783


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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 25186


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
                        string argmsg19 = sname + "の射程の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg19, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 26117


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
                            string argmsg20 = sname + "の回数の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg20, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 27039


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
                            string argmsg21 = sname + "の消費ＥＮの設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg21, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 27983


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
                            string argmsg22 = sname + "の必要気力の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg22, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        string argmsg23 = sname + "のアビリティ属性の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg23, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                        string argmsg24 = sname + "の属性のレベル指定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg24, ref fname, (short)line_num, ref line_buf, ref data_name);
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


                // 解説
                while (Strings.Left(line_buf, 1) == "*")
                {
                    if (Strings.Len(nd.Comment) > 0)
                    {
                        nd.Comment = nd.Comment + Constants.vbCr + Constants.vbLf;
                    }

                    nd.Comment = nd.Comment + Strings.Mid(line_buf, 2);
                    if (FileSystem.EOF((int)FileNumber))
                    {
                        break;
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }
            }

            ErrorHandler:
            ;

            // エラー処理
            if (line_num == 0)
            {
                string argmsg25 = fname + "が開けません。";
                GUI.ErrorMessage(ref argmsg25);
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