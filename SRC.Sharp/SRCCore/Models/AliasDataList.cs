﻿// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Models
{
    // 全エリアスデータを管理するリストのクラス
    public class AliasDataList
    {
        private SrcCollection<AliasDataType> colAliasDataList = new SrcCollection<AliasDataType>();

        public IList<AliasDataType> Items => colAliasDataList;

        // エリアスデータリストにデータを追加
        public AliasDataType Add(string aname)
        {
            var ad = new AliasDataType();
            ad.Name = aname;
            colAliasDataList.Add(ad, aname);
            return ad;
        }

        // エリアスデータリストに登録されているデータの総数
        public int Count()
        {
            return colAliasDataList.Count;
        }

        // エリアスデータリストからデータを削除
        public void Delete(string Index)
        {
            colAliasDataList.Remove(Index);
        }

        // エリアスデータリストからデータを取り出す
        public AliasDataType Item(string Index)
        {
            return colAliasDataList[Index];
        }

        // エリアスデータリストに指定したデータが定義されているか？
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
        }

        // データファイル fname からデータをロード
        public void Load(string fname)
        {
            using (var stream = new FileStream(fname, FileMode.Open))
            {
                Load(fname, stream);
            }
        }

        public void Load(string fname, Stream stream)
        {
            using (var reader = new SrcDataReader(fname, stream))
            {
                string line_buf = "";
                while (reader.HasMore)
                {
                    line_buf = reader.GetLine();
                    if (string.IsNullOrEmpty(line_buf))
                    {
                        continue;
                    }
                    if (Strings.InStr(line_buf, ",") > 0)
                    {
                        throw reader.InvalidDataException("名称の設定が抜けています。", line_buf);
                    }

                    var data_name = line_buf;
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

                    if (IsDefined(data_name))
                    {
                        // すでに定義されているエリアスのデータであれば置き換える
                        Delete(data_name);
                    }

                    var ad = Add(data_name);
                    while (reader.HasMore)
                    {
                        line_buf = reader.GetLine();
                        if (Strings.Len(line_buf) == 0)
                        {
                            break;
                        }

                        ad.AddAlias(line_buf);
                    }

                    if (ad.Count == 0)
                    {
                        throw reader.InvalidDataException("エリアス対象のデータが定義されていません。", data_name);
                    }
                }
            }
        }
    }
}