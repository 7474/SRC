using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class NonPilotDataList
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 全ノンパイロットデータを管理するリストのクラス

        // ノンパイロットデータのコレクション
        private Collection colNonPilotDataList = new Collection();

        // クラスの初期化
        // UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Initialize_Renamed()
        {
            var npd = new NonPilotData();

            // Talkコマンド用
            npd.Name = "ナレーター";
            npd.Nickname = "ナレーター";
            npd.Bitmap = ".bmp";
            colNonPilotDataList.Add(npd, npd.Name);
        }

        public NonPilotDataList() : base()
        {
            Class_Initialize_Renamed();
        }

        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            {
                var withBlock = colNonPilotDataList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colNonPilotDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colNonPilotDataList = null;
        }

        ~NonPilotDataList()
        {
            Class_Terminate_Renamed();
        }

        // ノンパイロットデータリストにデータを追加
        public NonPilotData Add(ref string pname)
        {
            NonPilotData AddRet = default;
            var new_pilot_data = new NonPilotData();
            new_pilot_data.Name = pname;
            colNonPilotDataList.Add(new_pilot_data, pname);
            AddRet = new_pilot_data;
            return AddRet;
        }

        // ノンパイロットデータリストに登録されているデータの総数
        public short Count()
        {
            short CountRet = default;
            CountRet = (short)colNonPilotDataList.Count;
            return CountRet;
        }

        // ノンパイロットデータリストからデータを削除
        public void Delete(ref object Index)
        {
            colNonPilotDataList.Remove(Index);
        }

        // ノンパイロットデータリストからデータを取り出す
        public NonPilotData Item(ref object Index)
        {
            NonPilotData ItemRet = default;
            string pname;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2348


            Input:

                    On Error GoTo ErrorHandler

             */
            ItemRet = (NonPilotData)colNonPilotDataList[Index];
            return ItemRet;
            ErrorHandler:
            ;

            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            pname = Conversions.ToString(Index);
            foreach (NonPilotData pd in colNonPilotDataList)
            {
                if ((pd.Nickname0 ?? "") == (pname ?? ""))
                {
                    ItemRet = pd;
                    return ItemRet;
                }
            }
        }

        // ノンパイロットデータリストに指定したデータが定義されているか？
        public bool IsDefined(ref object Index)
        {
            bool IsDefinedRet = default;
            NonPilotData pd;
            string pname;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2975


            Input:

                    On Error GoTo ErrorHandler

             */
            pd = (NonPilotData)colNonPilotDataList[Index];
            IsDefinedRet = true;
            return IsDefinedRet;
            ErrorHandler:
            ;

            // UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            pname = Conversions.ToString(Index);
            foreach (NonPilotData currentPd in colNonPilotDataList)
            {
                pd = currentPd;
                if ((pd.Nickname0 ?? "") == (pname ?? ""))
                {
                    IsDefinedRet = true;
                    return IsDefinedRet;
                }
            }

            IsDefinedRet = false;
        }

        // ノンパイロットデータリストに指定したデータが定義されているか？ (愛称は見ない)
        public bool IsDefined2(ref object Index)
        {
            bool IsDefined2Ret = default;
            NonPilotData pd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 3635


            Input:

                    On Error GoTo ErrorHandler

             */
            pd = (NonPilotData)colNonPilotDataList[Index];
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
            string buf, line_buf = default, buf2;
            short ret;
            var pd = new NonPilotData();
            string data_name;
            string err_msg;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 4087


            Input:

                    On Error GoTo ErrorHandler

             */
            FileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read);
            line_num = 0;
            while (true)
            {
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
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4815


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
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4994


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
                {
                    err_msg = "名称に全角括弧は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5194


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "\"") > 0)
                {
                    err_msg = "名称に\"は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5327


                    Input:
                                    Error(0)

                     */
                }

                object argIndex2 = (object)data_name;
                if (IsDefined(ref argIndex2))
                {
                    // すでに定義されているノンパイロットのデータであれば置き換える
                    NonPilotData localItem() { object argIndex1 = (object)data_name; var ret = Item(ref argIndex1); return ret; }

                    if ((localItem().Name ?? "") == (data_name ?? ""))
                    {
                        object argIndex1 = (object)data_name;
                        pd = Item(ref argIndex1);
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
                // 愛称、ビットマップ
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

                // 愛称
                ret = (short)Strings.InStr(line_buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "ビットマップの設定が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5855


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                buf = Strings.Mid(line_buf, (int)ret + 1);
                if (!string.IsNullOrEmpty(buf2))
                {
                    pd.Nickname = buf2;
                }
                else
                {
                    err_msg = "愛称の設定が間違っています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6161


                    Input:
                                        Error(0)

                     */
                }

                // ビットマップ
                buf2 = Strings.Trim(buf);
                if (Strings.LCase(Strings.Right(buf2, 4)) == ".bmp")
                {
                    pd.Bitmap = buf2;
                }
                else
                {
                    string argmsg = "ビットマップの設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg, ref fname, (short)line_num, ref line_buf, ref pd.Name);
                }
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