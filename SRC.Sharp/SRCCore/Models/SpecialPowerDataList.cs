// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRCCore.Lib;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;

namespace SRCCore.Models
{
    // 全スペシャルパワーデータを管理するリストのクラス
    public class SpecialPowerDataList
    {
        // スペシャルパワーデータのコレクション
        private SrcCollection<SpecialPowerData> colSpecialPowerDataList = new SrcCollection<SpecialPowerData>();

        public IList<SpecialPowerData> Items => colSpecialPowerDataList.List;

        private SRC SRC;
        public SpecialPowerDataList(SRC src)
        {
            SRC = src;
        }

        // スペシャルパワーデータリストにデータを追加
        public SpecialPowerData Add(string sname)
        {
            var new_data = new SpecialPowerData(SRC);
            new_data.Name = sname;
            colSpecialPowerDataList.Add(new_data, sname);
            return new_data;
        }

        // スペシャルパワーデータリストに登録されているデータの総数
        public int Count()
        {
            return colSpecialPowerDataList.Count;
        }

        // スペシャルパワーデータリストから指定したデータを削除
        public void Delete(string Index)
        {
            colSpecialPowerDataList.Remove(Index);
        }

        // スペシャルパワーデータリストから指定したデータを取り出す
        public SpecialPowerData Item(string Index)
        {
            return colSpecialPowerDataList[Index];
        }

        // スペシャルパワーデータリストに指定したデータが登録されているか？
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
                while (reader.HasMore)
                {
                    string buf, buf2;
                    string data_name = "";
                    string line_buf = "";
                    // 空行をスキップ
                    while (reader.HasMore && string.IsNullOrEmpty(line_buf))
                    {
                        line_buf = reader.GetLine();
                    }
                    if (string.IsNullOrEmpty(line_buf))
                    {
                        break;
                    }

                    // 名称
                    var ret = Strings.InStr(line_buf, ",");
                    if (ret > 0)
                    {
                        data_name = Strings.Trim(Strings.Left(line_buf, ret - 1));
                        buf = Strings.Mid(line_buf, ret + 1);
                    }
                    else
                    {
                        data_name = line_buf;
                        buf = "";
                    }

                    if (Strings.InStr(data_name, " ") > 0)
                    {
                        throw reader.InvalidDataException("名称に半角スペースは使用出来ません。", line_buf);
                    }

                    if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
                    {
                        throw reader.InvalidDataException("名称に全角括弧は使用出来ません。", line_buf);
                    }

                    if (Strings.InStr(data_name, "\"") > 0)
                    {
                        throw reader.InvalidDataException("名称に\"は使用出来ません。", line_buf);
                    }

                    if (IsDefined(data_name))
                    {
                        Delete(data_name);
                    }

                    var sd = Add(data_name);
                    // 読み仮名
                    ret = Strings.InStr(buf, ",");
                    if (ret > 0)
                    {
                        throw reader.InvalidDataException("読み仮名の後に余分なデータが指定されています。", line_buf);
                    }

                    sd.KanaName = Strings.Trim(buf);
                    if (string.IsNullOrEmpty(sd.KanaName))
                    {
                        sd.KanaName = GeneralLib.StrToHiragana(data_name);
                    }

                    // 短縮形, 消費ＳＰ, 対象, 効果時間, 適用条件, 使用条件, アニメ
                    line_buf = reader.GetLine();

                    // 短縮形
                    ret = Strings.InStr(line_buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("消費ＳＰが抜けています。", line_buf);
                    }

                    buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                    buf = Strings.Mid(line_buf, ret + 1);
                    sd.ShortName = buf2;

                    // 消費ＳＰ
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("対象が抜けています。", line_buf);
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        sd.SPConsumption = Conversions.ToInteger(buf2);
                    }
                    else
                    {
                        SRC.AddDataError(reader.InvalidData("消費ＳＰの設定が間違っています。", data_name));
                    }

                    // 対象
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("効果時間が抜けています。", line_buf);
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        throw reader.InvalidDataException("対象が間違っています。", line_buf);
                    }

                    sd.TargetType = buf2;

                    // 効果時間
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("効果時間が抜けています。", line_buf);
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        throw reader.InvalidDataException("効果時間が間違っています。", line_buf);
                    }

                    sd.Duration = buf2;

                    // 適用条件
                    ret = Strings.InStr(buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException("使用条件が抜けています。", line_buf);
                    }

                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    if (Information.IsNumeric(buf2))
                    {
                        throw reader.InvalidDataException("適用条件が間違っています。", line_buf);
                    }

                    sd.NecessaryCondition = buf2;

                    // 使用条件, アニメ
                    ret = Strings.InStr(buf, ",");
                    if (ret > 0)
                    {
                        sd.Animation = Strings.Trim(Strings.Mid(buf, Strings.InStr(buf, ",") + 1));
                    }
                    else
                    {
                        sd.Animation = sd.Name;
                    }

                    // 効果
                    line_buf = reader.GetLine();
                    if (Strings.Len(line_buf) == 0)
                    {
                        throw reader.InvalidDataException("効果が指定されていません。", line_buf);
                    }

                    sd.SetEffect(line_buf);

                    // 解説
                    line_buf = reader.GetLine();
                    if (Strings.Len(line_buf) == 0)
                    {
                        throw reader.InvalidDataException("解説が指定されていません。", line_buf);
                    }

                    sd.Comment = line_buf;
                }
            }
        }
    }
}
