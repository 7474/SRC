using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class SpecialPowerDataList
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 全スペシャルパワーデータを管理するリストのクラス

        // スペシャルパワーデータのコレクション
        private Collection colSpecialPowerDataList = new Collection();


        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            {
                var withBlock = colSpecialPowerDataList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colSpecialPowerDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colSpecialPowerDataList = null;
        }

        ~SpecialPowerDataList()
        {
            Class_Terminate_Renamed();
        }

        // スペシャルパワーデータリストにデータを追加
        public SpecialPowerData Add(ref string sname)
        {
            SpecialPowerData AddRet = default;
            var new_data = new SpecialPowerData();
            new_data.Name = sname;
            colSpecialPowerDataList.Add(new_data, sname);
            AddRet = new_data;
            return AddRet;
        }

        // スペシャルパワーデータリストに登録されているデータの総数
        public short Count()
        {
            short CountRet = default;
            CountRet = (short)colSpecialPowerDataList.Count;
            return CountRet;
        }

        // スペシャルパワーデータリストから指定したデータを削除
        public void Delete(ref object Index)
        {
            colSpecialPowerDataList.Remove(Index);
        }

        // スペシャルパワーデータリストから指定したデータを取り出す
        public SpecialPowerData Item(ref object Index)
        {
            SpecialPowerData ItemRet = default;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 1775


            Input:
                    On Error GoTo ErrorHandler

             */
            ItemRet = (SpecialPowerData)colSpecialPowerDataList[Index];
            return ItemRet;
            ErrorHandler:
            ;

            // UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            ItemRet = null;
        }

        // スペシャルパワーデータリストに指定したデータが登録されているか？
        public bool IsDefined(ref object Index)
        {
            bool IsDefinedRet = default;
            SpecialPowerData dummy;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2272


            Input:

                    On Error GoTo ErrorHandler

             */
            dummy = (SpecialPowerData)colSpecialPowerDataList[Index];
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
            short ret;
            int line_num;
            string buf, line_buf = default, buf2;
            SpecialPowerData sd;
            string data_name;
            string err_msg;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2729


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
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3842


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
                {
                    err_msg = "名称に全角括弧は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4042


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "\"") > 0)
                {
                    err_msg = "名称に\"は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4175


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

                sd = Add(ref data_name);
                // 読み仮名
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret > 0)
                {
                    err_msg = "読み仮名の後に余分なデータが指定されています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4461


                    Input:
                                        Error(0)

                     */
                }

                sd.KanaName = Strings.Trim(buf);
                if (string.IsNullOrEmpty(sd.KanaName))
                {
                    sd.KanaName = GeneralLib.StrToHiragana(ref data_name);
                }

                // 短縮形, 消費ＳＰ, 対象, 効果時間, 適用条件, 使用条件, アニメ
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

                // 短縮形
                ret = (short)Strings.InStr(line_buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "消費ＳＰが抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4920


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                buf = Strings.Mid(line_buf, (int)ret + 1);
                sd.ShortName = buf2;

                // 消費ＳＰ
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "対象が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5290


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    sd.SPConsumption = Conversions.ToShort(buf2);
                }
                else
                {
                    string argmsg = "消費ＳＰの設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 対象
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "効果時間が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5856


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    err_msg = "対象が間違っています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6162


                    Input:
                                        Error(0)

                     */
                }

                sd.TargetType = buf2;

                // 効果時間
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "効果時間が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6346


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    err_msg = "効果時間が間違っています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6654


                    Input:
                                        Error(0)

                     */
                }

                sd.Duration = buf2;

                // 適用条件
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "使用条件が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6836


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    err_msg = "適用条件が間違っています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 7144


                    Input:
                                        Error(0)

                     */
                }

                sd.NecessaryCondition = buf2;

                // 使用条件, アニメ
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret > 0)
                {
                    sd.Animation = Strings.Trim(Strings.Mid(buf, Strings.InStr(buf, ",") + 1));
                }
                else
                {
                    sd.Animation = sd.Name;
                }

                // 効果
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                if (Strings.Len(line_buf) == 0)
                {
                    err_msg = "効果が指定されていません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 7712


                    Input:
                                        Error(0)

                     */
                }

                sd.SetEffect(ref line_buf);

                // 解説
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                if (Strings.Len(line_buf) == 0)
                {
                    err_msg = "解説が指定されていません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 7952


                    Input:
                                        Error(0)

                     */
                }

                sd.Comment = line_buf;
            }

            ErrorHandler:
            ;

            // エラー処理
            if (line_num == 0)
            {
                string argmsg1 = fname + "が開けません。";
                GUI.ErrorMessage(ref argmsg1);
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