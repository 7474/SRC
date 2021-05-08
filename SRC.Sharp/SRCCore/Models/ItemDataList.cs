// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore.Models
{
    // 全アイテムデータを管理するリストのクラス
    public class ItemDataList
    {
        // アイテムデータのコレクション
        private SrcCollection<ItemData> colItemDataList = new SrcCollection<ItemData>();

        public IList<ItemData> Items => colItemDataList.List;

        public string Raw = "";
        public string DataComment = "";

        private SRC SRC;
        public ItemDataList(SRC src)
        {
            SRC = src;
        }

        public void Clear()
        {
            colItemDataList.Clear();
            Raw = "";
            DataComment = "";
        }

        // アイテムデータリストにデータを追加
        public ItemData Add(string new_name)
        {
            var new_Item_data = new ItemData();
            new_Item_data.Name = new_name;
            colItemDataList.Add(new_Item_data, new_name);
            return new_Item_data;
        }

        // アイテムデータリストに登録されているデータの総数
        public int Count()
        {
            return colItemDataList.Count;
        }

        // アイテムデータリストから指定したデータを削除
        public void Delete(string Index)
        {
            colItemDataList.Remove(Index);
        }

        // アイテムデータリストから指定したデータを取り出す
        public ItemData Item(string Index)
        {
            return colItemDataList[Index];
        }

        // アイテムデータリストに指定したデータが登録されているか？
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
        }

        // データファイル fname からデータをロード
        public void Load(string fname)
        {
            using (var stream = SRC.FileSystem.OpenText(SRC.SystemConfig.SRCCompatibilityMode, fname))
            {
                Load(fname, stream);
            }
        }

        public void Load(string fname, Stream stream)
        {
            using (var reader = new SrcDataReader(fname, stream))
            {
                ItemData lastId = null;
                while (reader.HasMore)
                {
                    lastId = LoadItem(reader, lastId);
                }
                Raw = reader.Raw;
            }
        }

        private ItemData LoadItem(SrcDataReader reader, ItemData lastId)
        {
            ItemData nd = null;
            try
            {
                string buf, buf2;

                string data_name = "";
                string line_buf = "";
                // 空行をスキップ
                while (reader.HasMore && string.IsNullOrEmpty(line_buf))
                {
                    line_buf = reader.GetLine();
                }
                if (lastId != null)
                {
                    lastId.DataComment = reader.RawComment.Trim();
                }
                else
                {
                    DataComment = string.Join(Environment.NewLine + Environment.NewLine,
                        new string[]{
                            DataComment,
                            reader.RawComment.Trim(),
                        }.Where(x => !string.IsNullOrEmpty(x)));
                }
                reader.ClearRawComment();
                if (reader.EOT)
                {
                    return null;
                }

                if (Strings.InStr(line_buf, ",") > 0)
                {
                    throw reader.InvalidDataException(@"名称の設定が抜けています。", data_name);
                }

                // 名称
                data_name = line_buf;
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
                    Delete(data_name);
                }

                nd = Add(data_name);
                // 愛称、読み仮名、アイテムクラス、装備個所
                line_buf = reader.GetLine();

                // 書式チェックのため、コンマの数を数えておく
                var comma_num = 0;
                var loopTo = Strings.Len(line_buf);
                for (var i = 1; i <= loopTo; i++)
                {
                    if (Strings.Mid(line_buf, i, 1) == ",")
                    {
                        comma_num = (comma_num + 1);
                    }
                }

                if (comma_num < 2)
                {
                    throw reader.InvalidDataException("設定に抜けがあります。", data_name);
                }
                else if (comma_num > 3)
                {
                    throw reader.InvalidDataException("余分な「,」があります。", data_name);
                }

                // 愛称
                if (Strings.Len(line_buf) == 0)
                {
                    throw reader.InvalidDataException("愛称の設定が抜けています。", data_name);
                }

                var ret = Strings.InStr(line_buf, ",");
                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                nd.Nickname = buf2;

                // 読み仮名
                if (comma_num == 3)
                {
                    ret = Strings.InStr(buf, ",");
                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    nd.KanaName = buf2;
                }
                else
                {
                    nd.KanaName = GeneralLib.StrToHiragana(nd.Nickname);
                }

                // アイテムクラス
                ret = Strings.InStr(buf, ",");
                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                nd.Class = buf2;

                // 装備個所
                buf2 = Strings.Trim(buf);
                if (Strings.Len(buf2) == 0)
                {
                    throw reader.InvalidDataException("装備個所の設定が抜けています。", data_name);
                }

                nd.Part = buf2;

                // 特殊能力データ
                line_buf = UnitDataList.LoadFeatureOuter(data_name, nd, reader, SRC);

                // 最大ＨＰ修正値, 最大ＥＮ修正値, 装甲修正値, 運動性修正値, 移動力修正値

                // 最大ＨＰ修正値
                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException("最大ＥＮ修正値の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    nd.HP = Conversions.ToInteger(buf2);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData("最大ＨＰ修正値の設定が間違っています。", data_name));
                }

                // 最大ＥＮ修正値
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException("装甲修正値の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    nd.EN = Conversions.ToInteger(buf2);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData("最大ＥＮ修正値の設定が間違っています。", data_name));
                }

                // 装甲修正値
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException("運動性修正値の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    nd.Armor = Conversions.ToInteger(buf2);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData("装甲修正値の設定が間違っています。", data_name));
                }

                // 運動性修正値
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException("移動力修正値の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    nd.Mobility = Conversions.ToInteger(buf2);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData("運動性修正値の設定が間違っています。", data_name));
                }

                // 移動力修正値
                buf2 = Strings.Trim(buf);
                if (Strings.Len(buf2) == 0)
                {
                    throw reader.InvalidDataException("移動力修正値の設定が抜けています。", data_name);
                }

                if (Information.IsNumeric(buf2))
                {
                    nd.Speed = Conversions.ToInteger(buf2);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData("移動力修正値の設定が間違っています。", data_name));
                }

                if (reader.EOT)
                {
                    return nd;
                }

                // 武器データ
                line_buf = UnitDataList.LoadWepon(data_name, nd, reader, SRC);

                if (line_buf == "===")
                {
                    // アビリティデータ
                    line_buf = UnitDataList.LoadAbility(data_name, nd, reader, SRC);
                }

                // 解説
                while (Strings.Left(line_buf, 1) == "*")
                {
                    if (Strings.Len(nd.Comment) > 0)
                    {
                        nd.Comment = nd.Comment + Environment.NewLine;
                    }

                    nd.Comment = nd.Comment + Strings.Mid(line_buf, 2);
                    if (reader.EOT)
                    {
                        break;
                    }

                    line_buf = reader.GetLine();
                }
            }
            finally
            {
                if (nd != null)
                {
                    nd.Raw = reader.RawData;
                    reader.ClearRawData();
                }
            }
            return nd;
        }
    }
}
