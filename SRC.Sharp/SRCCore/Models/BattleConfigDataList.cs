using Microsoft.VisualBasic;

namespace Project1
{
    internal class BattleConfigDataList
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // バトルコンフィグデータを管理するクラス
        // --- ダメージ計算、命中率算出など、バトルに関連するエリアスの定義を設定します。

        // バトルコンフィグデータのコレクション
        private Collection colBattleConfigData = new Collection();


        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            {
                var withBlock = colBattleConfigData;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }
            // UPGRADE_NOTE: オブジェクト colBattleConfigData をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            colBattleConfigData = null;
        }

        ~BattleConfigDataList()
        {
            Class_Terminate_Renamed();
        }

        // バトルコンフィグデータリストにデータを追加
        public BattleConfigData Add(ref string cname)
        {
            BattleConfigData AddRet = default;
            var cd = new BattleConfigData();
            cd.Name = cname;
            colBattleConfigData.Add(cd, cname);
            AddRet = cd;
            return AddRet;
        }

        // バトルコンフィグデータリストに登録されているデータの総数
        public short Count()
        {
            short CountRet = default;
            CountRet = (short)colBattleConfigData.Count;
            return CountRet;
        }

        // バトルコンフィグデータリストからデータを削除
        public void Delete(ref object Index)
        {
            colBattleConfigData.Remove(Index);
        }

        // バトルコンフィグデータリストからデータを取り出す
        public BattleConfigData Item(ref string Index)
        {
            BattleConfigData ItemRet = default;
            ItemRet = (BattleConfigData)colBattleConfigData[Index];
            return ItemRet;
        }

        // バトルコンフィグデータリストに指定したデータが定義されているか？
        public bool IsDefined(ref object Index)
        {
            bool IsDefinedRet = default;
            BattleConfigData cd;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 1967


            Input:

                    On Error GoTo ErrorHandler

             */
            cd = (BattleConfigData)colBattleConfigData[Index];
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
            BattleConfigData cd;
            string data_name;
            string err_msg;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2386


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
                    if (FileSystem.EOF(FileNumber))
                    {
                        FileSystem.FileClose((int)FileNumber);
                        return;
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }
                while (Strings.Len(line_buf) == 0);
                data_name = line_buf;
                object argIndex2 = data_name;
                if (IsDefined(ref argIndex2))
                {
                    // すでに定義されているエリアスのデータであれば置き換える
                    object argIndex1 = data_name;
                    Delete(ref argIndex1);
                }

                cd = Add(ref data_name);
                while (true)
                {
                    if (FileSystem.EOF(FileNumber))
                    {
                        FileSystem.FileClose((int)FileNumber);
                        return;
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                    if (Strings.Len(line_buf) == 0)
                    {
                        break;
                    }

                    cd.ConfigCalc = line_buf;
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
    }
}