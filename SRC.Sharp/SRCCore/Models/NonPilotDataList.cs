// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Exceptions;
using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Models
{
    // 全ノンパイロットデータを管理するリストのクラス
    public class NonPilotDataList
    {
        // ノンパイロットデータのコレクション
        private SrcCollection<NonPilotData> colNonPilotDataList = new SrcCollection<NonPilotData>();

        public IList<NonPilotData> Items => colNonPilotDataList;

        public NonPilotDataList()
        {
            // Talkコマンド用
            var npd = new NonPilotData();
            npd.Name = "ナレーター";
            npd.Nickname = "ナレーター";
            npd.Bitmap = ".bmp";
            colNonPilotDataList.Add(npd, npd.Name);
        }

        // ノンパイロットデータリストにデータを追加
        public NonPilotData Add(string pname)
        {
            var new_pilot_data = new NonPilotData();
            new_pilot_data.Name = pname;
            colNonPilotDataList.Add(new_pilot_data, pname);
            return new_pilot_data;
        }

        // ノンパイロットデータリストに登録されているデータの総数
        public int Count()
        {
            return colNonPilotDataList.Count;
        }

        // ノンパイロットデータリストからデータを削除
        public void Delete(string Index)
        {
            colNonPilotDataList.Remove(Index);
        }

        // ノンパイロットデータリストからデータを取り出す
        public NonPilotData Item(string Index)
        {
            if (colNonPilotDataList.ContainsKey(Index))
            {
                return colNonPilotDataList[Index];
            }

            string pname = Index;
            foreach (NonPilotData pd in colNonPilotDataList)
            {
                if ((pd.Nickname0 ?? "") == (pname ?? ""))
                {
                    return pd;
                }
            }
            return null;
        }

        // ノンパイロットデータリストに指定したデータが定義されているか？
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
        }

        // ノンパイロットデータリストに指定したデータが定義されているか？ (愛称は見ない)
        public bool IsDefined2(string Index)
        {
            return colNonPilotDataList.ContainsKey(Index);
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
            // TODO Log
            var continuesErrors = new List<InvalidSrcData>();

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

                    // 名称
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

                    NonPilotData pd;
                    if (IsDefined(data_name))
                    {
                        // すでに定義されているノンパイロットのデータであれば置き換える
                        if ((Item(data_name).Name ?? "") == (data_name ?? ""))
                        {
                            pd = Item(data_name);
                        }
                        else
                        {
                            pd = Add(data_name);
                        }
                    }
                    else
                    {
                        pd = Add(data_name);
                    }
                    // 愛称、ビットマップ
                    line_buf = reader.GetLine();

                    // 愛称
                    var ret = Strings.InStr(line_buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("ビットマップの設定が抜けています。", data_name);
                    }

                    var buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                    var buf = Strings.Mid(line_buf, ret + 1);
                    if (!string.IsNullOrEmpty(buf2))
                    {
                        pd.Nickname = buf2;
                    }
                    else
                    {
                        throw reader.InvalidDataException("愛称の設定が間違っています。", data_name);
                    }

                    // ビットマップ
                    buf2 = Strings.Trim(buf);
                    if (Strings.LCase(Strings.Right(buf2, 4)) == ".bmp")
                    {
                        pd.Bitmap = buf2;
                    }
                    else
                    {
                        continuesErrors.Add(reader.InvalidData("ビットマップの設定が間違っています。", data_name));
                    }
                }
            }
        }
    }
}