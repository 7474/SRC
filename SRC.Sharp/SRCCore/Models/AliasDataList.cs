using System.Collections;
using Microsoft.VisualBasic;

namespace Project1
{
    internal class AliasDataList : IEnumerable
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 全エリアスデータを管理するリストのクラス


        // エリアスデータのコレクション
        private Collection colAliasDataList = new Collection();


        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            {
                var withBlock = colAliasDataList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colAliasDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colAliasDataList = null;
        }

        ~AliasDataList()
        {
            Class_Terminate_Renamed();
        }

        // エリアスデータリストにデータを追加
        public AliasDataType Add(ref string aname)
        {
            AliasDataType AddRet = default;
            var ad = new AliasDataType();
            ad.Name = aname;
            colAliasDataList.Add(ad, aname);
            AddRet = ad;
            return AddRet;
        }

        // エリアスデータリストに登録されているデータの総数
        public short Count()
        {
            short CountRet = default;
            CountRet = (short)colAliasDataList.Count;
            return CountRet;
        }

        // エリアスデータリストからデータを削除
        public void Delete(ref object Index)
        {
            colAliasDataList.Remove(Index);
        }

        // エリアスデータリストからデータを取り出す
        public AliasDataType Item(ref object Index)
        {
            AliasDataType ItemRet = default;
            ItemRet = (AliasDataType)colAliasDataList[Index];
            return ItemRet;
        }

        // エリアスデータリストに指定したデータが定義されているか？
        public bool IsDefined(ref object Index)
        {
            bool IsDefinedRet = default;
            AliasDataType ad;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 1909


            Input:

                    On Error GoTo ErrorHandler

             */
            ad = (AliasDataType)colAliasDataList[Index];
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
            int line_num;
            var line_buf = default(string);
            AliasDataType ad;
            string data_name;
            string err_msg;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2322


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
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3074


                    Input:
                                    Error(0)

                     */
                }

                data_name = line_buf;
                if (Strings.InStr(data_name, " ") > 0)
                {
                    err_msg = "名称に半角スペースは使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3245


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
                {
                    err_msg = "名称に全角括弧は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3445


                    Input:
                                    Error(0)

                     */
                }

                if (Strings.InStr(data_name, "\"") > 0)
                {
                    err_msg = "名称に\"は使用出来ません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3578


                    Input:
                                    Error(0)

                     */
                }

                object argIndex2 = (object)data_name;
                if (IsDefined(ref argIndex2))
                {
                    // すでに定義されているエリアスのデータであれば置き換える
                    object argIndex1 = (object)data_name;
                    Delete(ref argIndex1);
                }

                ad = Add(ref data_name);
                while (true)
                {
                    if (FileSystem.EOF((int)FileNumber))
                    {
                        FileSystem.FileClose((int)FileNumber);
                        return;
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                    if (Strings.Len(line_buf) == 0)
                    {
                        break;
                    }

                    ad.AddAlias(line_buf);
                }

                if ((int)ad.Count == 0)
                {
                    err_msg = "エリアス対象のデータが定義されていません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4239


                    Input:
                                        Error(0)

                     */
                }
            }

            ErrorHandler:
            ;

            // エラー処理
            if (line_num == 0)
            {
                string argmsg = fname + "が開けません。";
                GUI.ErrorMessage(ref argmsg);
            }
            else
            {
                FileSystem.FileClose((int)FileNumber);
                GUI.DataErrorMessage(ref err_msg, ref fname, (short)line_num, ref line_buf, ref data_name);
            }

            SRC.TerminateSRC();
        }

        // ForEach用関数
        // UPGRADE_NOTE: NewEnum プロパティがコメント アウトされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' をクリックしてください。
        // Public Function NewEnum() As stdole.IUnknown
        // NewEnum = colAliasDataList.GetEnumerator
        // End Function

        public IEnumerator GetEnumerator()
        {
            return default;
            // UPGRADE_TODO: コレクション列挙子を返すには、コメントを解除して以下の行を変更してください。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' をクリックしてください。
            // GetEnumerator = colAliasDataList.GetEnumerator
        }
    }
}