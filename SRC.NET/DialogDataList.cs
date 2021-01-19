using Microsoft.VisualBasic;

namespace Project1
{
    internal class DialogDataList
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 全ダイアログデータを管理するリストのクラス

        // ダイアログデータのコレクション
        private Collection colDialogDataList = new Collection();

        // クラスを解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            {
                var withBlock = colDialogDataList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colDialogDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colDialogDataList = null;
        }

        ~DialogDataList()
        {
            Class_Terminate_Renamed();
        }

        // ダイアログデータを追加
        public DialogData Add(ref string dname)
        {
            DialogData AddRet = default;
            var new_dd = new DialogData();
            new_dd.Name = dname;
            colDialogDataList.Add(new_dd, dname);
            AddRet = new_dd;
            return AddRet;
        }

        // ダイアログデータの総数
        public short Count()
        {
            short CountRet = default;
            CountRet = (short)colDialogDataList.Count;
            return CountRet;
        }

        public void Delete(ref object Index)
        {
            colDialogDataList.Remove(Index);
        }

        // ダイアログデータを返す
        public DialogData Item(ref object Index)
        {
            DialogData ItemRet = default;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 1618


            Input:
                    On Error GoTo ErrorHandler

             */
            ItemRet = (DialogData)colDialogDataList[Index];
            return ItemRet;
            ErrorHandler:
            ;

            // UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            ItemRet = null;
        }

        // 指定したダイアログデータが定義されているか？
        public bool IsDefined(ref object Index)
        {
            bool IsDefinedRet = default;
            DialogData dummy;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2093


            Input:

                    On Error GoTo ErrorHandler

             */
            dummy = (DialogData)colDialogDataList[Index];
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
            short i, ret;
            int line_num;
            var line_buf = default(string);
            string pilot_list;
            Dialog d;
            DialogData dd;
            string err_msg;
            string pname, msg;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2594


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

                // UPGRADE_NOTE: オブジェクト dd をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                dd = null;

                // パイロット名一覧
                if ((int)GeneralLib.LLength(ref line_buf) == 0)
                {
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3503


                    Input:
                                    Error(0)

                     */
                }

                pilot_list = "";
                var loopTo = GeneralLib.LLength(ref line_buf);
                for (i = (short)1; i <= loopTo; i++)
                    pilot_list = pilot_list + " " + GeneralLib.LIndex(ref line_buf, i);
                pilot_list = Strings.Trim(pilot_list);
                object argIndex2 = (object)pilot_list;
                if (IsDefined(ref argIndex2))
                {
                    object argIndex1 = (object)pilot_list;
                    Delete(ref argIndex1);
                }

                dd = Add(ref pilot_list);
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                while (Strings.Len(line_buf) > 0)
                {
                    // シチューション
                    d = dd.AddDialog(ref line_buf);
                    while (true)
                    {
                        if (FileSystem.EOF((int)FileNumber))
                        {
                            FileSystem.FileClose((int)FileNumber);
                            return;
                        }

                        GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

                        // 話者
                        ret = (short)Strings.InStr(line_buf, ",");
                        if ((int)ret == 0)
                        {
                            break;
                        }

                        pname = Strings.Left(line_buf, (int)ret - 1);

                        // 指定した話者のデータが存在するかチェック。
                        // ただし合体技のパートナーは場合は他の作品のパイロットであることも
                        // あるので話者チェックを行わない。
                        if (Strings.Left(pname, 1) != "@")
                        {
                            bool localIsDefined() { object argIndex1 = (object)pname; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                            bool localIsDefined1() { object argIndex1 = (object)pname; var ret = SRC.NPDList.IsDefined(ref argIndex1); return ret; }

                            if (!localIsDefined() & !localIsDefined1() & pname != "システム")
                            {
                                err_msg = "パイロット「" + pname + "」が定義されていません。";
                                ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                                /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4922


                                Input:
                                                            Error(0)

                                 */
                            }
                        }

                        // メッセージ
                        if (Strings.Len(line_buf) == (int)ret)
                        {
                            err_msg = "メッセージが定義されていません。";
                            ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5092


                            Input:
                                                    Error(0)

                             */
                        }

                        msg = Strings.Trim(Strings.Mid(line_buf, (int)ret + 1));
                        d.AddMessage(ref pname, ref msg);
                    }
                }
            }

            ErrorHandler:
            ;
            if (line_num == 0)
            {
                string argmsg = fname + "が開けません。";
                GUI.ErrorMessage(ref argmsg);
            }
            else
            {
                FileSystem.FileClose((int)FileNumber);
                if (dd is null)
                {
                    string argdname = "";
                    GUI.DataErrorMessage(ref err_msg, ref fname, (short)line_num, ref line_buf, ref argdname);
                }
                else
                {
                    GUI.DataErrorMessage(ref err_msg, ref fname, (short)line_num, ref line_buf, ref dd.Name);
                }
            }

            SRC.TerminateSRC();
        }
    }
}