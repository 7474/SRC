// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;

// 全メッセージデータ(または特殊効果データ)を管理するコレクションクラス
namespace SRCCore.Models
{
    public class MessageDataList
    {
        // メッセージデータ(または特殊効果データ)一覧
        private SrcCollection<MessageData> colMessageDataList = new SrcCollection<MessageData>();

        public IList<MessageData> Items => colMessageDataList;

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
            return colMessageDataList.ContainsKey(Index)
                ? colMessageDataList[Index]
                : null;
        }

        // メッセージデータが登録されているか？
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
        }

        // メッセージデータをファイルからロード
        public void Load(string fname, bool isEffect)
        {
            using (var stream = new FileStream(fname, FileMode.Open))
            {
                Load(fname, isEffect, stream);
            }
        }

        /// <param name="fname"></param>
        /// <param name="isEffect">特殊効果データor戦闘アニメデータか？</param>
        /// <param name="stream"></param>
        public void Load(string fname, bool isEffect, Stream stream)
        {
            using (var reader = new SrcDataReader(fname, stream))
            {
                string line_buf = "";
                while (reader.HasMore)
                {
                    line_buf = reader.GetLine();
                    while (reader.HasMore && string.IsNullOrEmpty(line_buf))
                    {
                        line_buf = reader.GetLine();
                    }
                    var data_name = line_buf;

                    if (Strings.InStr(line_buf, ",") > 0)
                    {
                        throw reader.InvalidDataException("名称の設定が抜けています。", data_name);
                    }
                    if (Strings.InStr(data_name, " ") > 0)
                    {
                        throw reader.InvalidDataException("名称に半角スペースは使用出来ません。", data_name);
                    }

                    if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
                    {
                        throw reader.InvalidDataException("名称に全角括弧は使用出来ません。", data_name);
                    }

                    if (Strings.InStr(data_name, "\"") > 0)
                    {
                        throw reader.InvalidDataException("名称に\"は使用出来ません。", data_name);
                    }

                    // 重複して定義されたデータの場合
                    MessageData md;
                    if (IsDefined(data_name))
                    {
                        if (!isEffect)
                        {
                            // パイロットメッセージの場合は後から定義されたものを優先
                            Delete(data_name);
                            md = Add(data_name);
                        }
                        else
                        {
                            // 特殊効果データの場合は既存のものに追加
                            md = Item(data_name);
                        }
                    }
                    else
                    {
                        md = Add(data_name);
                    }

                    line_buf = reader.GetLine();
                    while (Strings.Len(line_buf) > 0)
                    {
                        var ret = Strings.InStr(line_buf, ",");
                        if (ret < 2)
                        {
                            /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 4751
                             */
                        }

                        var sname = Strings.Left(line_buf, (int)ret - 1);
                        var msg = Strings.Trim(Strings.Mid(line_buf, (int)ret + 1));
                        if (Strings.Len(sname) == 0)
                        {
                            throw reader.InvalidDataException("シチュエーションの指定が抜けています。", data_name);
                        }

                        md.AddMessage(sname, msg);

                        if (!reader.HasMore)
                        {
                            break;
                        }
                        line_buf = reader.GetLine();
                    }
                }
            }
        }

        // デフォルトの戦闘アニメデータを定義
        public void AddDefaultAnimation()
        {
            return;
            // TODO Impl

            //// アニメデータが用意されていない場合は Data\System\animation.txt を読み込む
            //if (Count() == 0)
            //{
            //    string argfname1 = SRC.AppPath + @"Data\System\animation.txt";
            //    if (GeneralLib.FileExists(argfname1))
            //    {
            //        string argfname = SRC.AppPath + @"Data\System\animation.txt";
            //        Load(argfname);
            //    }
            //}

            //MessageData md;
            //if (IsDefined("汎用"))
            //{
            //    md = Item("汎用");
            //}
            //else
            //{
            //    md = Add("汎用");
            //}

            //if (Event_Renamed.FindNormalLabel("戦闘アニメ_回避発動") > 0)
            //{
            //    string argmsg_situation = "回避";
            //    Unit argu = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(argmsg_situation, u: argu)))
            //    {
            //        string argsit = "回避";
            //        string argmsg = "回避";
            //        md.AddMessage(argsit, argmsg);
            //    }
            //}

            //string arglname1 = "戦闘アニメ_切り払い発動";
            //if (Event_Renamed.FindNormalLabel(arglname1) > 0)
            //{
            //    string argmsg_situation1 = "切り払い";
            //    Unit argu1 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(argmsg_situation1, u: argu1)))
            //    {
            //        string argsit1 = "切り払い";
            //        string argmsg1 = "切り払い";
            //        md.AddMessage(argsit1, argmsg1);
            //    }
            //}

            //string arglname2 = "戦闘アニメ_迎撃発動";
            //if (Event_Renamed.FindNormalLabel(arglname2) > 0)
            //{
            //    string argmsg_situation2 = "迎撃";
            //    Unit argu2 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(argmsg_situation2, u: argu2)))
            //    {
            //        string argsit2 = "迎撃";
            //        string argmsg2 = "迎撃";
            //        md.AddMessage(argsit2, argmsg2);
            //    }
            //}

            //string arglname3 = "戦闘アニメ_ダミー発動";
            //if (Event_Renamed.FindNormalLabel(arglname3) > 0)
            //{
            //    string argmsg_situation3 = "ダミー";
            //    Unit argu3 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(argmsg_situation3, u: argu3)))
            //    {
            //        string argsit3 = "ダミー";
            //        string argmsg3 = "ダミー";
            //        md.AddMessage(argsit3, argmsg3);
            //    }
            //}

            //string arglname4 = "戦闘アニメ_修理装置発動";
            //if (Event_Renamed.FindNormalLabel(arglname4) > 0)
            //{
            //    string argmsg_situation4 = "修理";
            //    Unit argu4 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(argmsg_situation4, u: argu4)))
            //    {
            //        string argsit4 = "修理";
            //        string argmsg4 = "修理装置";
            //        md.AddMessage(argsit4, argmsg4);
            //    }
            //}

            //string arglname5 = "戦闘アニメ_補給装置発動";
            //if (Event_Renamed.FindNormalLabel(arglname5) > 0)
            //{
            //    string argmsg_situation5 = "補給";
            //    Unit argu5 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(argmsg_situation5, u: argu5)))
            //    {
            //        string argsit5 = "補給";
            //        string argmsg5 = "補給装置";
            //        md.AddMessage(argsit5, argmsg5);
            //    }
            //}

            //string arglname6 = "戦闘アニメ_終了発動";
            //if (Event_Renamed.FindNormalLabel(arglname6) > 0)
            //{
            //    string argmsg_situation6 = "終了";
            //    Unit argu6 = null;
            //    if (string.IsNullOrEmpty(md.SelectMessage(argmsg_situation6, u: argu6)))
            //    {
            //        string argsit6 = "終了";
            //        string argmsg6 = "終了";
            //        md.AddMessage(argsit6, argmsg6);
            //    }
            //}
        }
    }
}