// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.VB;

// 全メッセージデータ(または特殊効果データ)を管理するコレクションクラス
namespace SRCCore.Models
{
    public class MessageDataList
    {
        // メッセージデータ(または特殊効果データ)一覧
        private SrcCollection<MessageData> colMessageDataList = new SrcCollection<MessageData>();

        // メッセージデータの追加
        public MessageData Add(string mname)
        {
            var new_md = new MessageData();
            new_md.Name = mname;
            colMessageDataList.Add(new_md, mname);
            return new_md;
        }

        // メッセージデータの総数
        public int Count()
        {
            return colMessageDataList.Count;
        }

        public void Delete(string Index)
        {
            colMessageDataList.Remove(Index);
        }

        // メッセージデータの検索
        public MessageData Item(string Index)
        {
            try
            {
                return colMessageDataList[Index];
            }
            catch
            {
                return null;
            }
        }

        // メッセージデータが登録されているか？
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
        }

        // メッセージデータをファイルからロード
        public void Load(string fname)
        {
            return;
            // TODO Impl
            //            short FileNumber;
            //            short ret;
            //            int line_num;
            //            var line_buf = default(string);
            //            MessageData md;
            //            var is_effect = default(bool);
            //            string sname, msg;
            //            string data_name;
            //            string err_msg;

            //            // 特殊効果データor戦闘アニメデータか？
            //            if (Strings.InStr(Strings.LCase(fname), "effect.txt") > 0 | Strings.InStr(Strings.LCase(fname), "animation.txt") > 0)
            //            {
            //                is_effect = true;
            //            };
            //#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            //            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 2904


            //            Input:

            //                    On Error GoTo ErrorHandler

            //             */
            //            FileNumber = (short)FileSystem.FreeFile();
            //            FileSystem.FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read);
            //            line_num = 0;
            //            while (true)
            //            {
            //                data_name = "";
            //                do
            //                {
            //                    if (FileSystem.EOF((int)FileNumber))
            //                    {
            //                        FileSystem.FileClose((int)FileNumber);
            //                        return;
            //                    }

            //                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
            //                }
            //                while (Strings.Len(line_buf) == 0);
            //                if (Strings.InStr(line_buf, ",") > 0)
            //                {
            //                    err_msg = "名称の設定が抜けています。";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3656


            //                    Input:
            //                                    Error(0)

            //                     */
            //                }

            //                data_name = line_buf;
            //                if (Strings.InStr(data_name, " ") > 0)
            //                {
            //                    err_msg = "名称に半角スペースは使用出来ません。";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 3827


            //                    Input:
            //                                    Error(0)

            //                     */
            //                }

            //                if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
            //                {
            //                    err_msg = "名称に全角括弧は使用出来ません。";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4027


            //                    Input:
            //                                    Error(0)

            //                     */
            //                }

            //                if (Strings.InStr(data_name, "\"") > 0)
            //                {
            //                    err_msg = "名称に\"は使用出来ません。";
            //                    ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4160


            //                    Input:
            //                                    Error(0)

            //                     */
            //                }

            //                // 重複して定義されたデータの場合
            //                object argIndex3 = (object)data_name;
            //                if (IsDefined(ref argIndex3))
            //                {
            //                    if (!is_effect)
            //                    {
            //                        // パイロットメッセージの場合は後から定義されたものを優先
            //                        object argIndex1 = (object)data_name;
            //                        Delete(ref argIndex1);
            //                        md = Add(ref data_name);
            //                    }
            //                    else
            //                    {
            //                        // 特殊効果データの場合は既存のものに追加
            //                        object argIndex2 = (object)data_name;
            //                        md = Item(ref argIndex2);
            //                    }
            //                }
            //                else
            //                {
            //                    md = Add(ref data_name);
            //                }

            //                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
            //                while (Strings.Len(line_buf) > 0)
            //                {
            //                    ret = (short)Strings.InStr(line_buf, ",");
            //                    if ((int)ret < 2)
            //                    {
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4751


            //                        Input:
            //                                                Error(0)

            //                         */
            //                    }

            //                    sname = Strings.Left(line_buf, (int)ret - 1);
            //                    msg = Strings.Trim(Strings.Mid(line_buf, (int)ret + 1));
            //                    if (Strings.Len(sname) == 0)
            //                    {
            //                        err_msg = "シチュエーションの指定が抜けています。";
            //                        ;
            //#error Cannot convert ErrorStatementSyntax - see comment for details
            //                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5091


            //                        Input:
            //                                                Error(0)

            //                         */
            //                    }

            //                    md.AddMessage(ref sname, ref msg);
            //                    if (FileSystem.EOF((int)FileNumber))
            //                    {
            //                        FileSystem.FileClose((int)FileNumber);
            //                        return;
            //                    }

            //                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
            //                }
            //            }

            //        ErrorHandler:
            //            ;

            //            // エラー処理
            //            if (line_num == 0)
            //            {
            //                string argmsg = fname + "が開けません。";
            //                GUI.ErrorMessage(ref argmsg);
            //            }
            //            else
            //            {
            //                FileSystem.FileClose((int)FileNumber);
            //                string argmsg1 = "";
            //                GUI.DataErrorMessage(ref argmsg1, ref fname, (short)line_num, ref line_buf, ref data_name);
            //            }

            //            SRC.TerminateSRC();
        }

        // デフォルトの戦闘アニメデータを定義
        public void AddDefaultAnimation()
        {
            return;
            // TODO Impl
            //MessageData md;

            //// アニメデータが用意されていない場合は Data\System\animation.txt を読み込む
            //if (Count() == 0)
            //{
            //    string argfname1 = SRC.AppPath + @"Data\System\animation.txt";
            //    if (GeneralLib.FileExists(ref argfname1))
            //    {
            //        string argfname = SRC.AppPath + @"Data\System\animation.txt";
            //        Load(ref argfname);
            //    }
            //}

            //object argIndex2 = "汎用";
            //if (IsDefined(ref argIndex2))
            //{
            //    object argIndex1 = "汎用";
            //    md = Item(ref argIndex1);
            //}
            //else
            //{
            //    string argmname = "汎用";
            //    md = Add(ref argmname);
            //}

            //string arglname = "戦闘アニメ_回避発動";
            //if (Event_Renamed.FindNormalLabel(ref arglname) > 0)
            //{
            //    string argmsg_situation = "回避";
            //    Unit argu = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(ref argmsg_situation, u: ref argu)))
            //    {
            //        string argsit = "回避";
            //        string argmsg = "回避";
            //        md.AddMessage(ref argsit, ref argmsg);
            //    }
            //}

            //string arglname1 = "戦闘アニメ_切り払い発動";
            //if (Event_Renamed.FindNormalLabel(ref arglname1) > 0)
            //{
            //    string argmsg_situation1 = "切り払い";
            //    Unit argu1 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(ref argmsg_situation1, u: ref argu1)))
            //    {
            //        string argsit1 = "切り払い";
            //        string argmsg1 = "切り払い";
            //        md.AddMessage(ref argsit1, ref argmsg1);
            //    }
            //}

            //string arglname2 = "戦闘アニメ_迎撃発動";
            //if (Event_Renamed.FindNormalLabel(ref arglname2) > 0)
            //{
            //    string argmsg_situation2 = "迎撃";
            //    Unit argu2 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(ref argmsg_situation2, u: ref argu2)))
            //    {
            //        string argsit2 = "迎撃";
            //        string argmsg2 = "迎撃";
            //        md.AddMessage(ref argsit2, ref argmsg2);
            //    }
            //}

            //string arglname3 = "戦闘アニメ_ダミー発動";
            //if (Event_Renamed.FindNormalLabel(ref arglname3) > 0)
            //{
            //    string argmsg_situation3 = "ダミー";
            //    Unit argu3 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(ref argmsg_situation3, u: ref argu3)))
            //    {
            //        string argsit3 = "ダミー";
            //        string argmsg3 = "ダミー";
            //        md.AddMessage(ref argsit3, ref argmsg3);
            //    }
            //}

            //string arglname4 = "戦闘アニメ_修理装置発動";
            //if (Event_Renamed.FindNormalLabel(ref arglname4) > 0)
            //{
            //    string argmsg_situation4 = "修理";
            //    Unit argu4 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(ref argmsg_situation4, u: ref argu4)))
            //    {
            //        string argsit4 = "修理";
            //        string argmsg4 = "修理装置";
            //        md.AddMessage(ref argsit4, ref argmsg4);
            //    }
            //}

            //string arglname5 = "戦闘アニメ_補給装置発動";
            //if (Event_Renamed.FindNormalLabel(ref arglname5) > 0)
            //{
            //    string argmsg_situation5 = "補給";
            //    Unit argu5 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(ref argmsg_situation5, u: ref argu5)))
            //    {
            //        string argsit5 = "補給";
            //        string argmsg5 = "補給装置";
            //        md.AddMessage(ref argsit5, ref argmsg5);
            //    }
            //}

            //string arglname6 = "戦闘アニメ_終了発動";
            //if (Event_Renamed.FindNormalLabel(ref arglname6) > 0)
            //{
            //    string argmsg_situation6 = "終了";
            //    Unit argu6 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(ref argmsg_situation6, u: ref argu6)))
            //    {
            //        string argsit6 = "終了";
            //        string argmsg6 = "終了";
            //        md.AddMessage(ref argsit6, ref argmsg6);
            //    }
            //}
        }
    }
}