using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class PilotDataList
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 全パイロットデータを管理するリストのクラス

        // パイロットデータのコレクション
        private Collection colPilotDataList = new Collection();

        // クラスの初期化
        // UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Initialize_Renamed()
        {
            var pd = new PilotData();

            // ユニットステータスコマンドの無人ユニット用
            pd.Name = "ステータス表示用ダミーパイロット(ザコ)";
            pd.Nickname = "パイロット不在";
            pd.Adaption = "AAAA";
            pd.Bitmap = ".bmp";
            colPilotDataList.Add(pd, pd.Name);
        }

        public PilotDataList() : base()
        {
            Class_Initialize_Renamed();
        }

        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            {
                var withBlock = colPilotDataList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colPilotDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colPilotDataList = null;
        }

        ~PilotDataList()
        {
            Class_Terminate_Renamed();
        }

        // パイロットデータリストにデータを追加
        public PilotData Add(ref string pname)
        {
            PilotData AddRet = default;
            var new_pilot_data = new PilotData();
            new_pilot_data.Name = pname;
            colPilotDataList.Add(new_pilot_data, pname);
            AddRet = new_pilot_data;
            return AddRet;
        }

        // パイロットデータリストに登録されているデータの総数
        public short Count()
        {
            short CountRet = default;
            CountRet = (short)colPilotDataList.Count;
            return CountRet;
        }

        // パイロットデータリストから指定したデータを消去
        public void Delete(ref object Index)
        {
            colPilotDataList.Remove(Index);
        }

        // パイロットデータリストから指定したデータを取り出す
        public PilotData Item(ref object Index)
        {
            PilotData ItemRet = default;
            string pname;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2350


            Input:

                    On Error GoTo ErrorHandler

             */
            ItemRet = (PilotData)colPilotDataList[Index];
            return ItemRet;
            ErrorHandler:
            ;

            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            pname = Conversions.ToString(Index);
            foreach (PilotData pd in colPilotDataList)
            {
                if ((pd.Nickname ?? "") == (pname ?? ""))
                {
                    ItemRet = pd;
                    return ItemRet;
                }
            }
        }

        // パイロットデータリストに指定したデータが登録されているか？
        public bool IsDefined(ref object Index)
        {
            bool IsDefinedRet = default;
            PilotData pd;
            string pname;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2965


            Input:

                    On Error GoTo ErrorHandler

             */
            pd = (PilotData)colPilotDataList[Index];
            IsDefinedRet = true;
            return IsDefinedRet;
            ErrorHandler:
            ;

            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            pname = Conversions.ToString(Index);
            foreach (PilotData currentPd in colPilotDataList)
            {
                pd = currentPd;
                if ((pd.Nickname ?? "") == (pname ?? ""))
                {
                    IsDefinedRet = true;
                    return IsDefinedRet;
                }
            }

            IsDefinedRet = false;
        }

        // パイロットデータリストに指定したデータが登録されているか？ (愛称は見ない)
        public bool IsDefined2(ref object Index)
        {
            bool IsDefined2Ret = default;
            PilotData pd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 3613


            Input:

                    On Error GoTo ErrorHandler

             */
            pd = (PilotData)colPilotDataList[Index];
            IsDefined2Ret = true;
            return IsDefined2Ret;
            ErrorHandler:
            ;
            IsDefined2Ret = false;
        }

        // データファイル fname からデータをロード
        public void Load(ref string fname)
        {
            short FileNumber;
            int line_num;
            short i, j;
            int ret, n, ret2;
            string buf, line_buf = default, buf2;
            PilotData pd;
            string data_name;
            string err_msg;
            string aname, adata = default;
            var alevel = default(double);
            WeaponData wd;
            AbilityData sd;
            string wname, sname = default;
            short sp_cost;
            bool in_quote;
            short comma_num;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 4329


            Input:

                    On Error GoTo ErrorHandler

             */
            FileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read);
            line_num = 0;
            while (true)
            {
                data_name = "";
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
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5081


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
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5260


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
                {
                    err_msg = "名称に全角括弧は使用出来ません";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5459


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "\"") > 0)
                {
                    err_msg = "名称に\"は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5592


                    Input:
                                    Error(0)

                     */
                }

                object argIndex2 = (object)data_name;
                if (IsDefined(ref argIndex2))
                {
                    // すでに定義済みのパイロットの場合はデータを置き換え
                    PilotData localItem() { object argIndex1 = (object)data_name; var ret = Item(ref argIndex1); return ret; }

                    if ((localItem().Name ?? "") == (data_name ?? ""))
                    {
                        object argIndex1 = (object)data_name;
                        pd = Item(ref argIndex1);
                        pd.Clear();
                    }
                    else
                    {
                        pd = Add(ref data_name);
                    }
                }
                else
                {
                    pd = Add(ref data_name);
                }
                // 愛称, 読み仮名, 性別, クラス, 地形適応, 経験値
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
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6331


                    Input:
                                        Error(0)

                     */
                }
                else if ((int)comma_num > 5)
                {
                    err_msg = "余分な「,」があります。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6409


                    Input:
                                        Error(0)

                     */
                }

                // 愛称
                ret = Strings.InStr(line_buf, ",");
                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (Strings.Len(buf2) == 0)
                {
                    err_msg = "愛称の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6805


                    Input:
                                        Error(0)

                     */
                }

                pd.Nickname = buf2;
                switch (comma_num)
                {
                    case 4:
                        {
                            // 読み仮名 or 性別
                            ret = Strings.InStr(buf, ",");
                            buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                            buf = Strings.Mid(buf, ret + 1);
                            switch (buf2 ?? "")
                            {
                                case "男性":
                                case "女性":
                                case "-":
                                    {
                                        string argstr_Renamed = pd.Nickname;
                                        pd.KanaName = GeneralLib.StrToHiragana(ref argstr_Renamed);
                                        pd.Nickname = argstr_Renamed;
                                        pd.Sex = buf2;
                                        break;
                                    }

                                default:
                                    {
                                        pd.KanaName = buf2;
                                        break;
                                    }
                            }

                            break;
                        }

                    case 5:
                        {
                            // 読み仮名
                            ret = Strings.InStr(buf, ",");
                            buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                            buf = Strings.Mid(buf, ret + 1);
                            switch (buf2 ?? "")
                            {
                                case "男性":
                                case "女性":
                                case "-":
                                    {
                                        string argmsg = "読み仮名の設定が抜けています。";
                                        GUI.DataErrorMessage(ref argmsg, ref fname, (short)line_num, ref line_buf, ref data_name);
                                        string argstr_Renamed1 = pd.Nickname;
                                        pd.KanaName = GeneralLib.StrToHiragana(ref argstr_Renamed1);
                                        pd.Nickname = argstr_Renamed1;
                                        break;
                                    }

                                default:
                                    {
                                        pd.KanaName = buf2;
                                        break;
                                    }
                            }

                            // 性別
                            ret = Strings.InStr(buf, ",");
                            buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                            buf = Strings.Mid(buf, ret + 1);
                            switch (buf2 ?? "")
                            {
                                case "男性":
                                case "女性":
                                case "-":
                                    {
                                        pd.Sex = buf2;
                                        break;
                                    }

                                default:
                                    {
                                        string argmsg1 = "性別の設定が間違っています。";
                                        GUI.DataErrorMessage(ref argmsg1, ref fname, (short)line_num, ref line_buf, ref data_name);
                                        break;
                                    }
                            }

                            break;
                        }

                    default:
                        {
                            string argstr_Renamed2 = pd.Nickname;
                            pd.KanaName = GeneralLib.StrToHiragana(ref argstr_Renamed2);
                            pd.Nickname = argstr_Renamed2;
                            break;
                        }
                }

                // クラス
                ret = Strings.InStr(buf, ",");
                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (!Information.IsNumeric(buf2))
                {
                    pd.Class_Renamed = buf2;
                }
                else
                {
                    string argmsg2 = "クラスの設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg2, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 地形適応
                ret = Strings.InStr(buf, ",");
                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Strings.Len(buf2) == 4)
                {
                    pd.Adaption = buf2;
                }
                else
                {
                    string argmsg3 = "地形適応の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg3, ref fname, (short)line_num, ref line_buf, ref data_name);
                    pd.Adaption = "AAAA";
                }

                // 経験値
                buf2 = Strings.Trim(buf);
                if (Information.IsNumeric(buf2))
                {
                    pd.ExpValue = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    string argmsg4 = "経験値の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg4, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                    aname = "";
                    while (true)
                    {
                        i = (short)((int)i + 1);

                        // コンマの位置を検索
                        ret = Strings.InStr(buf, ",");
                        // 「"」が使われているか検索
                        ret2 = Strings.InStr(buf, "\"");
                        if (ret2 < ret & ret2 > 0)
                        {
                            // 「"」が見つかった場合、次の「"」後のコンマを検索
                            in_quote = true;
                            j = (short)(ret2 + 1);
                            while ((int)j <= Strings.Len(buf))
                            {
                                switch (Strings.Mid(buf, (int)j, 1) ?? "")
                                {
                                    case "\"":
                                        {
                                            in_quote = !in_quote;
                                            break;
                                        }

                                    case ",":
                                        {
                                            if (!in_quote)
                                            {
                                                buf2 = Strings.Left(buf, (int)j - 1);
                                                buf = Strings.Mid(buf, (int)j + 1);
                                            }

                                            break;
                                        }
                                }

                                j = (short)((int)j + 1);
                            }

                            if ((int)j == Strings.Len(buf))
                            {
                                buf2 = buf;
                                buf = "";
                            }

                            in_quote = false;
                        }
                        else if (ret > 0)
                        {
                            // コンマが見つかったらコンマまでの文字列を切り出す
                            buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                            buf = Strings.Trim(Strings.Mid(buf, ret + 1));

                            // コンマの後ろの文字列が空白の場合
                            if (string.IsNullOrEmpty(buf))
                            {
                                if ((int)i % 2 == 1)
                                {
                                    err_msg = "行末の「,」の後に特殊能力指定が抜けています。";
                                }
                                else
                                {
                                    err_msg = "行末の「,」の後に特殊能力レベル指定が抜けています。";
                                };
#error Cannot convert ErrorStatementSyntax - see comment for details
                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 11688


                                Input:
                                                                Error(0)

                                 */
                            }
                        }
                        else
                        {
                            // 行末の文字列
                            buf2 = buf;
                            buf = "";
                        }

                        if ((int)i % 2 == 1)
                        {
                            // 特殊能力名＆レベル

                            if (Information.IsNumeric(buf2))
                            {
                                if ((int)i == 1)
                                {
                                    // 特殊能力の指定は終り。能力値の指定へ
                                    buf = buf2 + ", " + buf;
                                    break;
                                }
                                else
                                {
                                    string argmsg5 = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)(((int)i + 1) / 2)) + "番目の特殊能力名の設定が間違っています。";
                                    GUI.DataErrorMessage(ref argmsg5, ref fname, (short)line_num, ref line_buf, ref data_name);
                                }
                            }

                            if (Strings.InStr(buf2, " ") > 0)
                            {
                                if (Strings.Left(buf2, 4) != "先手必勝" & Strings.Left(buf2, 6) != "ＳＰ消費減少" & Strings.Left(buf2, 12) != "スペシャルパワー自動発動" & Strings.Left(buf2, 4) != "ハンター" & Strings.InStr(buf2, "=解説 ") == 0)
                                {
                                    if (string.IsNullOrEmpty(aname))
                                    {
                                        err_msg = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)(((int)i + 1) / 2)) + "番目の特殊能力「" + Strings.Trim(Strings.Left(buf2, Strings.InStr(buf2, " "))) + "」の指定の後に「,」が抜けています。";
                                    }
                                    else
                                    {
                                        err_msg = "特殊能力「" + aname + "」のレベル指定の後に「,」が抜けています。";
                                    };
#error Cannot convert ErrorStatementSyntax - see comment for details
                                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 13110


                                    Input:
                                                                        Error(0)

                                     */
                                }
                            }

                            // 特殊能力の別名指定がある？
                            j = (short)Strings.InStr(buf2, "=");
                            if ((int)j > 0)
                            {
                                adata = Strings.Mid(buf2, (int)j + 1);
                                buf2 = Strings.Left(buf2, (int)j - 1);
                            }
                            else
                            {
                                adata = "";
                            }

                            // 特殊能力のレベル指定を切り出す
                            j = (short)Strings.InStr(buf2, "Lv");
                            switch (j)
                            {
                                case 0:
                                    {
                                        // 指定なし
                                        aname = buf2;
                                        alevel = (double)SRC.DEFAULT_LEVEL;
                                        break;
                                    }

                                case 1:
                                    {
                                        // レベル指定のみあり
                                        if (!Information.IsNumeric(Strings.Mid(buf2, (int)j + 2)))
                                        {
                                            string argmsg6 = "特殊能力「" + aname + "」のレベル指定が不正です。";
                                            GUI.DataErrorMessage(ref argmsg6, ref fname, (short)line_num, ref line_buf, ref data_name);
                                        }

                                        alevel = (double)Conversions.ToShort(Strings.Mid(buf2, (int)j + 2));
                                        if (string.IsNullOrEmpty(aname))
                                        {
                                            string argmsg7 = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)(((int)i + 1) / 2)) + "番目の特殊能力名の設定が間違っています。";
                                            GUI.DataErrorMessage(ref argmsg7, ref fname, (short)line_num, ref line_buf, ref data_name);
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        // 特殊能力名とレベルの両方が指定されている
                                        aname = Strings.Left(buf2, (int)j - 1);
                                        alevel = Conversions.ToDouble(Strings.Mid(buf2, (int)j + 2));
                                        break;
                                    }
                            }
                        }
                        // 特殊能力修得レベル
                        else if (Information.IsNumeric(buf2))
                        {
                            pd.AddSkill(ref aname, alevel, adata, Conversions.ToShort(buf2));
                        }
                        else
                        {
                            if (alevel > 0d)
                            {
                                string argmsg8 = "特殊能力「" + aname + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)alevel) + "」の修得レベルが間違っています。";
                                GUI.DataErrorMessage(ref argmsg8, ref fname, (short)line_num, ref line_buf, ref data_name);
                            }
                            else
                            {
                                string argmsg9 = "特殊能力「" + aname + "」の修得レベルが間違っています。";
                                GUI.DataErrorMessage(ref argmsg9, ref fname, (short)line_num, ref line_buf, ref data_name);
                            }

                            pd.AddSkill(ref aname, alevel, adata, (short)1);
                        }

                        if (string.IsNullOrEmpty(buf))
                        {
                            // ここでこの行は終り

                            // iが奇数の場合は修得レベルが抜けている
                            if ((int)i % 2 == 1)
                            {
                                if (alevel > 0d)
                                {
                                    string argmsg10 = "特殊能力「" + aname + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)alevel) + "」の修得レベルが間違っています。";
                                    GUI.DataErrorMessage(ref argmsg10, ref fname, (short)line_num, ref line_buf, ref data_name);
                                }
                                else
                                {
                                    string argmsg11 = "特殊能力「" + aname + "」の修得レベルが間違っています。";
                                    GUI.DataErrorMessage(ref argmsg11, ref fname, (short)line_num, ref line_buf, ref data_name);
                                }
                            }

                            GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                            buf = line_buf;
                            i = (short)0;
                            aname = "";
                        }
                    }
                }
                else if (Strings.InStr(line_buf, "特殊能力,") == 1)
                {
                    // 旧形式による特殊能力表記
                    buf = Strings.Mid(line_buf, 6);
                    i = (short)0;
                    aname = "";
                    do
                    {
                        i = (short)((int)i + 1);

                        // コンマの位置を検索
                        ret = Strings.InStr(buf, ",");
                        // 「"」が使われているか検索
                        ret2 = Strings.InStr(buf, "\"");
                        if (ret2 < ret & ret2 > 0)
                        {
                            // 「"」が見つかった場合、次の「"」後のコンマを検索
                            in_quote = true;
                            j = (short)(ret2 + 1);
                            while ((int)j <= Strings.Len(buf))
                            {
                                switch (Strings.Mid(buf, (int)j, 1) ?? "")
                                {
                                    case "\"":
                                        {
                                            in_quote = !in_quote;
                                            break;
                                        }

                                    case ",":
                                        {
                                            if (!in_quote)
                                            {
                                                buf2 = Strings.Left(buf, (int)j - 1);
                                                buf = Strings.Mid(buf, (int)j + 1);
                                            }

                                            break;
                                        }
                                }

                                j = (short)((int)j + 1);
                            }

                            if ((int)j == Strings.Len(buf))
                            {
                                buf2 = buf;
                                buf = "";
                            }

                            in_quote = false;
                        }
                        else if (ret > 0)
                        {
                            // コンマが見つかったらコンマまでの文字列を切り出す
                            buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                            buf = Strings.Mid(buf, ret + 1);

                            // コンマの後ろの文字列が空白の場合
                            if (string.IsNullOrEmpty(buf))
                            {
                                if ((int)i % 2 == 1)
                                {
                                    err_msg = "行末の「,」の後に特殊能力指定が抜けています。";
                                }
                                else
                                {
                                    err_msg = "行末の「,」の後に特殊能力レベル指定が抜けています。";
                                };
#error Cannot convert ErrorStatementSyntax - see comment for details
                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 17476


                                Input:
                                                                Error(0)

                                 */
                            }
                        }
                        else
                        {
                            // 行末の文字列
                            buf2 = buf;
                            buf = "";
                        }

                        if ((int)i % 2 == 1)
                        {
                            // 特殊能力名＆レベル

                            if (Strings.InStr(buf2, " ") > 0)
                            {
                                if (string.IsNullOrEmpty(aname))
                                {
                                    err_msg = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)(((int)i + 1) / 2)) + "番目の特殊能力の指定の後に「,」が抜けています。";
                                }
                                else
                                {
                                    err_msg = "特殊能力「" + aname + "」のレベル指定の後に「,」が抜けています。";
                                };
#error Cannot convert ErrorStatementSyntax - see comment for details
                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 17969


                                Input:
                                                                Error(0)

                                 */
                            }

                            // 特殊能力の別名指定がある？
                            j = (short)Strings.InStr(buf2, "=");
                            if ((int)j > 0)
                            {
                                adata = Strings.Mid(buf2, (int)j + 1);
                                buf2 = Strings.Left(buf2, (int)j - 1);
                            }
                            else
                            {
                                adata = "";
                            }

                            // 特殊能力のレベル指定を切り出す
                            j = (short)Strings.InStr(buf2, "Lv");
                            switch (j)
                            {
                                case 0:
                                    {
                                        // 指定なし
                                        aname = buf2;
                                        alevel = (double)SRC.DEFAULT_LEVEL;
                                        break;
                                    }

                                case 1:
                                    {
                                        // レベル指定のみあり
                                        if (!Information.IsNumeric(Strings.Mid(buf2, (int)j + 2)))
                                        {
                                            err_msg = "特殊能力「" + aname + "」のレベル指定が不正です";
                                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 18768


                                            Input:
                                                                                    Error(0)

                                             */
                                        }

                                        alevel = Conversions.ToDouble(Strings.Mid(buf2, (int)j + 2));
                                        if (string.IsNullOrEmpty(aname))
                                        {
                                            err_msg = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)(((int)i + 1) / 2)) + "番目の特殊能力の名前「" + buf2 + "」が不正です";
                                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 19057


                                            Input:
                                                                                    Error(0)

                                             */
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        // 特殊能力名とレベルの両方が指定されている
                                        aname = Strings.Left(buf2, (int)j - 1);
                                        alevel = Conversions.ToDouble(Strings.Mid(buf2, (int)j + 2));
                                        break;
                                    }
                            }
                        }
                        // 特殊能力修得レベル
                        else if (Information.IsNumeric(buf2))
                        {
                            pd.AddSkill(ref aname, alevel, adata, Conversions.ToShort(buf2));
                        }
                        else
                        {
                            if (alevel > 0d)
                            {
                                string argmsg12 = "特殊能力「" + aname + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)alevel) + "」の修得レベルが間違っています。";
                                GUI.DataErrorMessage(ref argmsg12, ref fname, (short)line_num, ref line_buf, ref data_name);
                            }
                            else
                            {
                                string argmsg13 = "特殊能力「" + aname + "」の修得レベルが間違っています。";
                                GUI.DataErrorMessage(ref argmsg13, ref fname, (short)line_num, ref line_buf, ref data_name);
                            }

                            pd.AddSkill(ref aname, alevel, adata, (short)1);
                        }
                    }
                    while (ret > 0);

                    // iが奇数の場合は修得レベルが抜けている
                    if ((int)i % 2 == 1)
                    {
                        if (alevel > 0d)
                        {
                            string argmsg14 = "特殊能力「" + aname + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)alevel) + "」の修得レベルが間違っています。";
                            GUI.DataErrorMessage(ref argmsg14, ref fname, (short)line_num, ref line_buf, ref data_name);
                        }
                        else
                        {
                            string argmsg15 = "特殊能力「" + aname + "」の修得レベルが間違っています。";
                            GUI.DataErrorMessage(ref argmsg15, ref fname, (short)line_num, ref line_buf, ref data_name);
                        }
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }
                else
                {
                    err_msg = "特殊能力の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 20520


                    Input:
                                        Error(0)

                     */
                }

                // 格闘
                if (Strings.Len(line_buf) == 0)
                {
                    err_msg = "格闘攻撃力の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 20665


                    Input:
                                        Error(0)

                     */
                }

                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    err_msg = "射撃攻撃力の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 20817


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Infight = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    string argmsg16 = "格闘攻撃力の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg16, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 射撃
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    err_msg = "命中の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 21428


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Shooting = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    string argmsg17 = "射撃攻撃力の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg17, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 命中
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    err_msg = "回避の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 22030


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Hit = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    string argmsg18 = "命中の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg18, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 回避
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    err_msg = "技量の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 22624


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Dodge = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    string argmsg19 = "回避の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg19, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 技量
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    err_msg = "反応の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 23220


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Technique = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    string argmsg20 = "技量の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg20, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 反応
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    err_msg = "性格の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 23820


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Intuition = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    string argmsg21 = "反応の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg21, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 性格
                buf2 = Strings.Trim(buf);
                if (Strings.Len(buf2) == 0)
                {
                    err_msg = "性格の設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 24458


                    Input:
                                        Error(0)

                     */
                }

                if (Strings.InStr(buf2, ",") > 0)
                {
                    string argmsg22 = "行末に余分なコンマが付けられています。";
                    GUI.DataErrorMessage(ref argmsg22, ref fname, (short)line_num, ref line_buf, ref data_name);
                    buf2 = Strings.Trim(Strings.Left(buf2, Strings.InStr(buf2, ",") - 1));
                }

                if (!Information.IsNumeric(buf2))
                {
                    pd.Personality = buf2;
                }
                else
                {
                    string argmsg23 = "性格の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg23, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // スペシャルパワー
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                switch (line_buf ?? "")
                {
                    // スペシャルパワーを持っていない
                    case "ＳＰなし":
                    case "精神なし":
                        {
                            break;
                        }

                    case var @case when @case == "":
                        {
                            err_msg = "スペシャルパワーの設定が抜けています。";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 25279


                            Input:
                                                    Error(0)

                             */
                            break;
                        }

                    default:
                        {
                            ret = Strings.InStr(line_buf, ",");
                            if (ret == 0)
                            {
                                err_msg = "ＳＰ値の設定が抜けています。";
                                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 25441


                                Input:
                                                            Error(0)

                                 */
                            }

                            buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                            buf = Strings.Mid(line_buf, ret + 1);
                            if (buf2 != "ＳＰ" & buf2 != "精神")
                            {
                                err_msg = "スペシャルパワーの設定が抜けています。";
                                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 25750


                                Input:
                                                            Error(0)

                                 */
                            }

                            // ＳＰ値
                            ret = Strings.InStr(buf, ",");
                            if (ret > 0)
                            {
                                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                                buf = Strings.Mid(buf, ret + 1);
                            }
                            else
                            {
                                buf2 = Strings.Trim(buf);
                                buf = "";
                            }

                            if (Information.IsNumeric(buf2))
                            {
                                pd.SP = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                            }
                            else
                            {
                                string argmsg24 = "ＳＰの設定が間違っています。";
                                GUI.DataErrorMessage(ref argmsg24, ref fname, (short)line_num, ref line_buf, ref data_name);
                                pd.SP = (short)1;
                            }

                            // スペシャルパワーと獲得レベル
                            ret = Strings.InStr(buf, ",");
                            while (ret > 0)
                            {
                                sname = Strings.Trim(Strings.Left(buf, ret - 1));
                                buf = Strings.Mid(buf, ret + 1);

                                // ＳＰ消費量
                                if (Strings.InStr(sname, "=") > 0)
                                {
                                    string argexpr = Strings.Mid(sname, Strings.InStr(sname, "=") + 1);
                                    sp_cost = (short)GeneralLib.StrToLng(ref argexpr);
                                    sname = Strings.Left(sname, Strings.InStr(sname, "=") - 1);
                                }
                                else
                                {
                                    sp_cost = (short)0;
                                }

                                ret = Strings.InStr(buf, ",");
                                if (ret == 0)
                                {
                                    buf2 = Strings.Trim(buf);
                                    buf = "";
                                }
                                else
                                {
                                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                                    buf = Strings.Mid(buf, ret + 1);
                                }

                                bool localIsDefined() { object argIndex1 = (object)sname; var ret = SRC.SPDList.IsDefined(ref argIndex1); return ret; }

                                if (string.IsNullOrEmpty(sname))
                                {
                                    string argmsg25 = "スペシャルパワーの指定が抜けています。";
                                    GUI.DataErrorMessage(ref argmsg25, ref fname, (short)line_num, ref line_buf, ref data_name);
                                }
                                else if (!localIsDefined())
                                {
                                    string argmsg26 = sname + "というスペシャルパワーは存在しません。";
                                    GUI.DataErrorMessage(ref argmsg26, ref fname, (short)line_num, ref line_buf, ref data_name);
                                }
                                else if (!Information.IsNumeric(buf2))
                                {
                                    string argmsg27 = "スペシャルパワー「" + sname + "」の獲得レベルが間違っています。";
                                    GUI.DataErrorMessage(ref argmsg27, ref fname, (short)line_num, ref line_buf, ref data_name);
                                    pd.AddSpecialPower(ref sname, (short)1, sp_cost);
                                }
                                else
                                {
                                    pd.AddSpecialPower(ref sname, Conversions.ToShort(buf2), sp_cost);
                                }

                                ret = Strings.InStr(buf, ",");
                            }

                            if (!string.IsNullOrEmpty(buf))
                            {
                                string argmsg28 = "スペシャルパワー「" + Strings.Trim(sname) + "」の獲得レベル指定が抜けています。";
                                GUI.DataErrorMessage(ref argmsg28, ref fname, (short)line_num, ref line_buf, ref data_name);
                            }

                            break;
                        }
                }

                // ビットマップ, ＭＩＤＩ
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

                // ビットマップ
                if (Strings.Len(line_buf) == 0)
                {
                    err_msg = "ビットマップの設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 28808


                    Input:
                                        Error(0)

                     */
                }

                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    err_msg = "ＭＩＤＩの設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 28959


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (Strings.LCase(Strings.Right(buf2, 4)) == ".bmp")
                {
                    pd.Bitmap = buf2;
                }
                else
                {
                    string argmsg29 = "ビットマップの設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg29, ref fname, (short)line_num, ref line_buf, ref data_name);
                    pd.IsBitmapMissing = true;
                }

                // ＭＩＤＩ
                buf = Strings.Trim(buf);
                buf2 = buf;
                while (Strings.Right(buf2, 1) == ")")
                    buf2 = Strings.Left(buf2, Strings.Len(buf2) - 1);
                switch (Strings.LCase(Strings.Right(buf2, 4)) ?? "")
                {
                    case ".mid":
                    case ".mp3":
                    case ".wav":
                    case "-":
                        {
                            pd.BGM = buf;
                            break;
                        }

                    case var case1 when case1 == "":
                        {
                            string argmsg30 = "ＭＩＤＩの設定が抜けています。";
                            GUI.DataErrorMessage(ref argmsg30, ref fname, (short)line_num, ref line_buf, ref data_name);
                            pd.Bitmap = "-.mid";
                            break;
                        }

                    default:
                        {
                            string argmsg31 = "ＭＩＤＩの設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg31, ref fname, (short)line_num, ref line_buf, ref data_name);
                            pd.Bitmap = "-.mid";
                            break;
                        }
                }

                if (FileSystem.EOF((int)FileNumber))
                {
                    FileSystem.FileClose((int)FileNumber);
                    return;
                }

                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                if (line_buf != "===")
                {
                    goto SkipRest;
                }

                // 特殊能力データ
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                buf = line_buf;
                i = (short)0;
                while (line_buf != "===")
                {
                    i = (short)((int)i + 1);
                    ret = 0;
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
                                        ret = (int)j;
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

                    if (ret > 0)
                    {
                        buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                        buf = Strings.Trim(Strings.Mid(buf, ret + 1));
                    }
                    else
                    {
                        buf2 = buf;
                        buf = "";
                    }

                    if (string.IsNullOrEmpty(buf2) | Information.IsNumeric(buf2))
                    {
                        string argmsg32 = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i) + "番目の特殊能力の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg32, ref fname, (short)line_num, ref line_buf, ref data_name);
                    }
                    else
                    {
                        pd.AddFeature(ref buf2);
                    }

                    if (string.IsNullOrEmpty(buf))
                    {
                        if (FileSystem.EOF((int)FileNumber))
                        {
                            FileSystem.FileClose((int)FileNumber);
                            return;
                        }

                        GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                        buf = line_buf;
                        i = (short)0;
                        if (string.IsNullOrEmpty(line_buf) | line_buf == "===")
                        {
                            break;
                        }
                    }
                }

                if (line_buf != "===")
                {
                    goto SkipRest;
                }

                // 武器データ
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                while (Strings.Len(line_buf) > 0 & line_buf != "===")
                {
                    // 武器名
                    ret = Strings.InStr(line_buf, ",");
                    if (ret == 0)
                    {
                        err_msg = "武器データの終りには空行を置いてください。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 32533


                        Input:
                                                Error(0)

                         */
                    }

                    wname = Strings.Trim(Strings.Left(line_buf, ret - 1));
                    buf = Strings.Mid(line_buf, ret + 1);
                    if (string.IsNullOrEmpty(wname))
                    {
                        err_msg = "武器名の設定が間違っています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 32821


                        Input:
                                                Error(0)

                         */
                    }

                    // 武器を登録
                    wd = pd.AddWeapon(ref wname);

                    // 攻撃力
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = wname + "の最小射程が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 33045


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
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
                        string argmsg33 = wname + "の攻撃力の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg33, ref fname, (short)line_num, ref line_buf, ref data_name);
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr1 = GeneralLib.LIndex(ref buf2, (short)1);
                            wd.Power = GeneralLib.StrToLng(ref argexpr1);
                        }
                    }

                    // 最小射程
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = wname + "の最大射程の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 33972


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        wd.MinRange = Conversions.ToShort(buf2);
                    }
                    else
                    {
                        string argmsg34 = wname + "の最小射程の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg34, ref fname, (short)line_num, ref line_buf, ref data_name);
                        wd.MinRange = (short)1;
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr2 = GeneralLib.LIndex(ref buf2, (short)1);
                            wd.MinRange = (short)GeneralLib.StrToLng(ref argexpr2);
                        }
                    }

                    // 最大射程
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = wname + "の命中率の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 34839


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        wd.MaxRange = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                    }
                    else
                    {
                        string argmsg35 = wname + "の最大射程の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg35, ref fname, (short)line_num, ref line_buf, ref data_name);
                        wd.MaxRange = (short)1;
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr3 = GeneralLib.LIndex(ref buf2, (short)1);
                            wd.MaxRange = (short)GeneralLib.StrToLng(ref argexpr3);
                        }
                    }

                    // 命中率
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = wname + "の弾数の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 35741


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
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
                        string argmsg36 = wname + "の命中率の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg36, ref fname, (short)line_num, ref line_buf, ref data_name);
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr4 = GeneralLib.LIndex(ref buf2, (short)1);
                            wd.Precision = (short)GeneralLib.StrToLng(ref argexpr4);
                        }
                    }

                    // 弾数
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = wname + "の消費ＥＮの設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 36695


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (buf2 != "-")
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            wd.Bullet = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                        }
                        else
                        {
                            string argmsg37 = wname + "の弾数の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg37, ref fname, (short)line_num, ref line_buf, ref data_name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr5 = GeneralLib.LIndex(ref buf2, (short)1);
                                wd.Bullet = (short)GeneralLib.StrToLng(ref argexpr5);
                            }
                        }
                    }

                    // 消費ＥＮ
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = wname + "の必要気力が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 37616


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (buf2 != "-")
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            wd.ENConsumption = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 999);
                        }
                        else
                        {
                            string argmsg38 = wname + "の消費ＥＮの設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg38, ref fname, (short)line_num, ref line_buf, ref data_name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr6 = GeneralLib.LIndex(ref buf2, (short)1);
                                wd.ENConsumption = (short)GeneralLib.StrToLng(ref argexpr6);
                            }
                        }
                    }

                    // 必要気力
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = wname + "の地形適応が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 38554


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
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
                            string argmsg39 = wname + "の必要気力の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg39, ref fname, (short)line_num, ref line_buf, ref data_name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr7 = GeneralLib.LIndex(ref buf2, (short)1);
                                wd.NecessaryMorale = (short)GeneralLib.StrToLng(ref argexpr7);
                            }
                        }
                    }

                    // 地形適応
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = wname + "のクリティカル率が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 39573


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (Strings.Len(buf2) == 4)
                    {
                        wd.Adaption = buf2;
                    }
                    else
                    {
                        string argmsg40 = wname + "の地形適応の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg40, ref fname, (short)line_num, ref line_buf, ref data_name);
                        wd.Adaption = "----";
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            wd.Adaption = GeneralLib.LIndex(ref buf2, (short)1);
                        }
                    }

                    // クリティカル率
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = wname + "の武器属性が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 40395


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
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
                        string argmsg41 = wname + "のクリティカル率の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg41, ref fname, (short)line_num, ref line_buf, ref data_name);
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr8 = GeneralLib.LIndex(ref buf2, (short)1);
                            wd.Critical = (short)GeneralLib.StrToLng(ref argexpr8);
                        }
                    }

                    // 武器属性
                    buf = Strings.Trim(buf);
                    if (Strings.Len(buf) == 0)
                    {
                        string argmsg42 = wname + "の武器属性の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg42, ref fname, (short)line_num, ref line_buf, ref data_name);
                    }

                    if (Strings.Right(buf, 1) == ")")
                    {
                        // 必要技能
                        ret = Strings.InStr(buf, "> ");
                        if (ret > 0)
                        {
                            if (ret > 0)
                            {
                                wd.NecessarySkill = Strings.Mid(buf, ret + 2);
                                buf = Strings.Trim(Strings.Left(buf, ret + 1));
                                ret = Strings.InStr(wd.NecessarySkill, "(");
                                wd.NecessarySkill = Strings.Mid(wd.NecessarySkill, ret + 1, Strings.Len(wd.NecessarySkill) - ret - 1);
                            }
                        }
                        else
                        {
                            ret = Strings.InStr(buf, "(");
                            if (ret > 0)
                            {
                                wd.NecessarySkill = Strings.Trim(Strings.Mid(buf, ret + 1, Strings.Len(buf) - ret - 1));
                                buf = Strings.Trim(Strings.Left(buf, ret - 1));
                            }
                        }
                    }

                    if (Strings.Right(buf, 1) == ">")
                    {
                        // 必要条件
                        ret = Strings.InStr(buf, "<");
                        if (ret > 0)
                        {
                            wd.NecessaryCondition = Strings.Trim(Strings.Mid(buf, ret + 1, Strings.Len(buf) - ret - 1));
                            buf = Strings.Trim(Strings.Left(buf, ret - 1));
                        }
                    }

                    wd.Class_Renamed = buf;
                    if (wd.Class_Renamed == "-")
                    {
                        wd.Class_Renamed = "";
                    }

                    if (Strings.InStr(wd.Class_Renamed, "Lv") > 0)
                    {
                        string argmsg43 = wname + "の属性のレベル指定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg43, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                    ret = Strings.InStr(line_buf, ",");
                    if (ret == 0)
                    {
                        err_msg = "アビリティデータの終りには空行を置いてください。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 44047


                        Input:
                                                Error(0)

                         */
                    }

                    sname = Strings.Trim(Strings.Left(line_buf, ret - 1));
                    buf = Strings.Mid(line_buf, ret + 1);
                    if (string.IsNullOrEmpty(sname))
                    {
                        err_msg = "アビリティ名の設定が間違っています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 44338


                        Input:
                                                Error(0)

                         */
                    }

                    // アビリティを登録
                    sd = pd.AddAbility(ref sname);

                    // 効果
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = sname + "の射程の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 44566


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    sd.SetEffect(ref buf2);

                    // 射程
                    sd.MinRange = (short)0;
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = sname + "の回数の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 44969


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
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
                        string argmsg44 = sname + "の射程の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg44, ref fname, (short)line_num, ref line_buf, ref data_name);
                        if ((int)GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                            string argexpr9 = GeneralLib.LIndex(ref buf2, (short)1);
                            sd.MaxRange = (short)GeneralLib.StrToLng(ref argexpr9);
                        }
                    }

                    // 回数
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = sname + "の消費ＥＮの設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 45900


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (buf2 != "-")
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            sd.Stock = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                        }
                        else
                        {
                            string argmsg45 = sname + "の回数の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg45, ref fname, (short)line_num, ref line_buf, ref data_name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr10 = GeneralLib.LIndex(ref buf2, (short)1);
                                sd.Stock = (short)GeneralLib.StrToLng(ref argexpr10);
                            }
                        }
                    }

                    // 消費ＥＮ
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = sname + "の必要気力の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 46822


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (buf2 != "-")
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            sd.ENConsumption = (short)GeneralLib.MinLng(Conversions.ToInteger(buf2), 999);
                        }
                        else
                        {
                            string argmsg46 = sname + "の消費ＥＮの設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg46, ref fname, (short)line_num, ref line_buf, ref data_name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr11 = GeneralLib.LIndex(ref buf2, (short)1);
                                sd.ENConsumption = (short)GeneralLib.StrToLng(ref argexpr11);
                            }
                        }
                    }

                    // 必要気力
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        err_msg = sname + "のアビリティ属性の設定が抜けています。";
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 47766


                        Input:
                                                Error(0)

                         */
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
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
                            string argmsg47 = sname + "の必要気力の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg47, ref fname, (short)line_num, ref line_buf, ref data_name);
                            if ((int)GeneralLib.LLength(ref buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(ref buf2, (short)2) + "," + buf;
                                string argexpr12 = GeneralLib.LIndex(ref buf2, (short)1);
                                sd.NecessaryMorale = (short)GeneralLib.StrToLng(ref argexpr12);
                            }
                        }
                    }

                    // アビリティ属性
                    buf = Strings.Trim(buf);
                    if (Strings.Len(buf) == 0)
                    {
                        string argmsg48 = sname + "のアビリティ属性の設定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg48, ref fname, (short)line_num, ref line_buf, ref data_name);
                    }

                    if (Strings.Right(buf, 1) == ")")
                    {
                        // 必要技能
                        ret = Strings.InStr(buf, "> ");
                        if (ret > 0)
                        {
                            if (ret > 0)
                            {
                                sd.NecessarySkill = Strings.Mid(buf, ret + 2);
                                buf = Strings.Trim(Strings.Left(buf, ret + 1));
                                ret = Strings.InStr(sd.NecessarySkill, "(");
                                sd.NecessarySkill = Strings.Mid(sd.NecessarySkill, ret + 1, Strings.Len(sd.NecessarySkill) - ret - 1);
                            }
                        }
                        else
                        {
                            ret = Strings.InStr(buf, "(");
                            if (ret > 0)
                            {
                                sd.NecessarySkill = Strings.Trim(Strings.Mid(buf, ret + 1, Strings.Len(buf) - ret - 1));
                                buf = Strings.Trim(Strings.Left(buf, ret - 1));
                            }
                        }
                    }

                    if (Strings.Right(buf, 1) == ">")
                    {
                        // 必要条件
                        ret = Strings.InStr(buf, "<");
                        if (ret > 0)
                        {
                            sd.NecessaryCondition = Strings.Trim(Strings.Mid(buf, ret + 1, Strings.Len(buf) - ret - 1));
                            buf = Strings.Trim(Strings.Left(buf, ret - 1));
                        }
                    }

                    sd.Class_Renamed = buf;
                    if (sd.Class_Renamed == "-")
                    {
                        sd.Class_Renamed = "";
                    }

                    if (Strings.InStr(sd.Class_Renamed, "Lv") > 0)
                    {
                        string argmsg49 = sname + "の属性のレベル指定が間違っています。";
                        GUI.DataErrorMessage(ref argmsg49, ref fname, (short)line_num, ref line_buf, ref data_name);
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
                string argmsg50 = fname + "が開けません";
                GUI.ErrorMessage(ref argmsg50);
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