// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;

// 全メッセージデータ(または特殊効果データ)を管理するコレクションクラス
namespace SRCCore.Models
{
    public class MessageDataList
    {
        private SRC SRC;
        public MessageDataList(SRC src)
        {
            SRC = src;
        }

        // メッセージデータ(または特殊効果データ)一覧
        private SrcCollection<MessageData> colMessageDataList = new SrcCollection<MessageData>();

        public IList<MessageData> Items => colMessageDataList.List;

        // メッセージデータの追加
        public MessageData Add(string mname)
        {
            var new_md = new MessageData(SRC);
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
            using (var stream = SRC.FileSystem.OpenText(SRC.SystemConfig.SRCCompatibilityMode, fname))
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
            //    if (SRC.FileSystem.FileExists(SRC.AppPath + @"Data\System\animation.txt"))
            //    {
            //        Load(SRC.AppPath + @"Data\System\animation.txt");
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

            //if (Event.FindNormalLabel("戦闘アニメ_回避発動") > 0)
            //{
            //    if (string.IsNullOrEmpty(md.SelectMessage("回避", u: null)))
            //    {
            //        md.AddMessage("回避", "回避");
            //    }
            //}

            //if (Event.FindNormalLabel("戦闘アニメ_切り払い発動") > 0)
            //{
            //    if (string.IsNullOrEmpty(md.SelectMessage("切り払い", u: null)))
            //    {
            //        md.AddMessage("切り払い", "切り払い");
            //    }
            //}

            //if (Event.FindNormalLabel("戦闘アニメ_迎撃発動") > 0)
            //{
            //    if (string.IsNullOrEmpty(md.SelectMessage("迎撃", u: null)))
            //    {
            //        md.AddMessage("迎撃", "迎撃");
            //    }
            //}

            //if (Event.FindNormalLabel("戦闘アニメ_ダミー発動") > 0)
            //{
            //    if (string.IsNullOrEmpty(md.SelectMessage("ダミー", u: null)))
            //    {
            //        md.AddMessage("ダミー", "ダミー");
            //    }
            //}

            //if (Event.FindNormalLabel("戦闘アニメ_修理装置発動") > 0)
            //{
            //    if (string.IsNullOrEmpty(md.SelectMessage("修理", u: null)))
            //    {
            //        md.AddMessage("修理", "修理装置");
            //    }
            //}

            //if (Event.FindNormalLabel("戦闘アニメ_補給装置発動") > 0)
            //{
            //    if (string.IsNullOrEmpty(md.SelectMessage("補給", u: null)))
            //    {
            //        md.AddMessage("補給", "補給装置");
            //    }
            //}

            //if (Event.FindNormalLabel("戦闘アニメ_終了発動") > 0)
            //{
            //    if (string.IsNullOrEmpty(md.SelectMessage("終了", u: null)))
            //    {
            //        md.AddMessage("終了", "終了");
            //    }
            //}
        }
    }
}
