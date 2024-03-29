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
    // 全ユニットデータを管理するリストのクラス
    public class UnitDataList
    {
        // ユニットデータ用コレクション
        private SrcCollection<UnitData> colUnitDataList;

        // ID作成用変数
        private int IDNum;

        public string Raw = "";
        public string DataComment = "";

        public IList<UnitData> Items => colUnitDataList.List;

        private SRC SRC;
        public UnitDataList(SRC src)
        {
            SRC = src;
            IDNum = 0;
            colUnitDataList = new SrcCollection<UnitData>();
            AddDummyData();
        }

        private void AddDummyData()
        {
            var ud = new UnitData(SRC);
            ud.Name = "ステータス表示用ダミーユニット";
            ud.Nickname = "ユニット無し";
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.Bitmap = ".bmp";
            ud.AddFeature("ダミーユニット=システム用非表示能力");
            colUnitDataList.Add(ud, ud.Name);
        }

        public void Clear()
        {
            colUnitDataList.Clear();
            Raw = "";
            DataComment = "";
        }

        // ユニットデータリストにデータを追加
        public UnitData Add(string uname)
        {
            UnitData AddRet = default;
            var ud = new UnitData(SRC);
            ud.Name = uname;
            colUnitDataList.Add(ud, uname);
            IDNum = IDNum + 1;
            ud.ID = IDNum;
            AddRet = ud;
            return AddRet;
        }

        // ユニットデータリストに登録されているデータ数
        public int Count()
        {
            return colUnitDataList.Count;
        }

        // ユニットデータリストからデータを削除
        public void Delete(int Index)
        {
            colUnitDataList.RemoveAt(Index);
        }

        // ユニットデータリストからデータを取り出す
        public UnitData Item(string Index)
        {
            return colUnitDataList.ContainsKey(Index)
                ? colUnitDataList[Index]
                : null;
        }

        // ユニットデータリストに登録されているか？
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
            //try
            //{
            using (var reader = new SrcDataReader(fname, stream))
            {
                UnitData lastUd = null;
                while (reader.HasMore)
                {
                    lastUd = LoadUnit(reader, lastUd);
                }
                Raw = reader.Raw;
            }
            //}
            //catch (InvalidSrcDataException)
            //{
            //    throw;
            //}
        }

        // reader から１つユニットを読み込む。
        // 返却したUnitDataはリストに追加されている状態。
        private UnitData LoadUnit(SrcDataReader reader, UnitData lastUd)
        {
            UnitData ud = null;
            try
            {
                int ret;
                string buf, buf2;
                var line_buf = "";

                // 空行をスキップ
                while (reader.HasMore && string.IsNullOrEmpty(line_buf))
                {
                    line_buf = reader.GetLine();
                }
                if (lastUd != null)
                {
                    lastUd.DataComment = reader.RawComment.Trim();
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

                // 名称
                string data_name;
                ret = Strings.InStr(line_buf, ",");
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
                    throw reader.InvalidDataException(@"名称に半角スペースは使用出来ません。", data_name);
                }

                if (Strings.InStr(data_name, "（") > 0 || Strings.InStr(data_name, "）") > 0)
                {
                    throw reader.InvalidDataException(@"名称に全角括弧は使用出来ません。", data_name);
                }

                if (Strings.InStr(data_name, "\"") > 0)
                {
                    throw reader.InvalidDataException(@"名称に\は使用出来ません。", data_name);
                }

                if (IsDefined(data_name))
                {
                    ud = Item(data_name);
                    ud.Clear();
                }
                else
                {
                    ud = Add(data_name);
                }
                // 読み仮名
                ret = Strings.InStr(buf, ",");
                if (ret > 0)
                {
                    throw reader.InvalidDataException(@"読み仮名の後に余分なデータが指定されています。", data_name);
                }
                ud.KanaName = buf;

                // 愛称, 読み仮名, ユニットクラス, パイロット数, アイテム数
                line_buf = reader.GetLine();
                // 書式チェックのため、コンマの数を数えておく
                int comma_num = 0;
                var loopTo = Strings.Len(line_buf);
                for (int i = 1; i <= loopTo; i++)
                {
                    if (Strings.Mid(line_buf, i, 1) == ",")
                    {
                        comma_num = (comma_num + 1);
                    }
                }

                if (comma_num < 3)
                {
                    throw reader.InvalidDataException(@"設定に抜けがあります。", data_name);
                }
                else if (comma_num > 4)
                {
                    throw reader.InvalidDataException(@"余分な「,」があります。", data_name);
                }

                // 愛称
                if (Strings.Len(line_buf) == 0)
                {
                    throw reader.InvalidDataException(@"愛称の設定が抜けています。", data_name);
                }

                ret = Strings.InStr(line_buf, ",");
                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                ud.Nickname = buf2;

                // 読み仮名
                if (comma_num == 4)
                {
                    ret = Strings.InStr(buf, ",");
                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    ud.KanaName = buf2;
                }
                else
                {
                    ud.KanaName = GeneralLib.StrToHiragana(ud.Nickname);
                }

                // ユニットクラス
                ret = Strings.InStr(buf, ",");
                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (!Information.IsNumeric(buf2))
                {
                    ud.Class = buf2;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"ユニットクラスの設定が間違っています。", data_name));
                    ud.Class = "汎用";
                }

                // パイロット数
                ret = Strings.InStr(buf, ",");
                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Strings.Left(buf2, 1) != "(")
                {
                    if (Information.IsNumeric(buf2))
                    {
                        ud.PilotNum = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                    }
                    else
                    {
                        SRC.AddDataError(reader.InvalidData(@"パイロット数の設定が間違っています。", data_name));
                        ud.PilotNum = 1;
                    }
                    if (ud.PilotNum < 1)
                    {
                        SRC.AddDataError(reader.InvalidData(@"パイロット数の設定が間違っています。", data_name));
                        ud.PilotNum = 1;
                    }
                }
                else
                {
                    if (Strings.Right(buf2, 1) != ")")
                    {
                        throw reader.InvalidDataException(@"パイロット数の設定が間違っています。", data_name);
                    }

                    buf2 = Strings.Mid(buf2, 2, Strings.Len(buf2) - 2);
                    if (Information.IsNumeric(buf2))
                    {
                        ud.PilotNum = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                    }
                    else
                    {
                        SRC.AddDataError(reader.InvalidData(@"パイロット数の設定が間違っています。", data_name));
                        ud.PilotNum = 1;
                    }

                    if (ud.PilotNum < 1)
                    {
                        SRC.AddDataError(reader.InvalidData(@"パイロット数の設定が間違っています。", data_name));
                        ud.PilotNum = 1;
                    }

                    // XXX 何で負数にしてるの？
                    ud.PilotNum = -ud.PilotNum;
                }

                // アイテム数
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    throw reader.InvalidDataException(@"アイテム数の設定が抜けています。", data_name);
                }

                if (Information.IsNumeric(buf))
                {
                    ud.ItemNum = GeneralLib.MinLng(Conversions.ToInteger(buf), 99);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"アイテム数の設定が間違っています。", data_name));
                    ud.ItemNum = 4;
                }

                // 移動可能地形, 移動力, サイズ, 修理費, 経験値
                line_buf = reader.GetLine();

                // 移動可能地形
                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"移動力の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (!Information.IsNumeric(buf2))
                {
                    ud.Transportation = buf2;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"移動可能地形の設定が間違っています。", data_name));
                    ud.Transportation = "陸";
                }

                // 移動力
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"サイズの設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    ud.Speed = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"移動力の設定が間違っています。", data_name));
                }

                // サイズ
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"経験値の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                switch (buf2 ?? "")
                {
                    case "XL":
                    case "LL":
                    case "L":
                    case "M":
                    case "S":
                    case "SS":
                        {
                            ud.Size = buf2;
                            break;
                        }

                    default:
                        {
                            SRC.AddDataError(reader.InvalidData(@"サイズの設定が間違っています。", data_name));
                            ud.Size = "M";
                            break;
                        }
                }

                // 修理費
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"経験値の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    ud.Value = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"修理費の設定が間違っています。", data_name));
                }

                // 経験値
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    throw reader.InvalidDataException(@"経験値の設定が抜けています。", data_name);
                }

                if (Information.IsNumeric(buf))
                {
                    ud.ExpValue = GeneralLib.MinLng(Conversions.ToInteger(buf), 9999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"経験値の設定が間違っています。", data_name));
                }

                // 特殊能力データ
                line_buf = LoadFeatureOuter(data_name, ud, reader, SRC);

                // 最大ＨＰ
                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"最大ＥＮの設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    ud.HP = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"最大ＨＰの設定が間違っています。", data_name));
                    ud.HP = 1000;
                }

                // 最大ＥＮ
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"装甲の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    ud.EN = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"最大ＥＮの設定が間違っています。", data_name));
                    ud.EN = 100;
                }

                // 装甲
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"運動性の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    ud.Armor = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"装甲の設定が間違っています。", data_name));
                }

                // 運動性
                buf2 = Strings.Trim(buf);
                if (Strings.Len(buf2) == 0)
                {
                    throw reader.InvalidDataException(@"運動性の設定が抜けています。", data_name);
                }

                if (Information.IsNumeric(buf2))
                {
                    ud.Mobility = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"運動性の設定が間違っています。", data_name));
                }

                // 地形適応, ビットマップ
                line_buf = reader.GetLine();

                // 地形適応
                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"ビットマップの設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (Strings.Len(buf2) == 4)
                {
                    ud.Adaption = buf2;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"地形適応の設定が間違っています。", data_name));
                    ud.Adaption = "AAAA";
                }

                // ビットマップ
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    throw reader.InvalidDataException(@"ビットマップの設定が抜けています。", data_name);
                }

                if (Strings.LCase(Strings.Right(buf, 4)) == ".bmp")
                {
                    ud.Bitmap = buf;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"ビットマップの設定が間違っています。", data_name));
                    ud.IsBitmapMissing = true;
                }

                if (reader.EOT)
                {
                    return ud;
                }

                // 武器データ
                line_buf = LoadWepon(data_name, ud, reader, SRC);

                if (line_buf != "===")
                {
                    return ud;
                }

                // アビリティデータ
                line_buf = LoadAbility(data_name, ud, reader, SRC);

            }
            finally
            {
                if (ud != null)
                {
                    ud.Raw = reader.RawData;
                    reader.ClearRawData();
                }
            }
            return ud;
        }

        public static string LoadFeatureOuter(string data_name, IUnitDataElements ud, SrcDataReader reader, SRC SRC)
        {
            int ret;
            string buf;
            string buf2;
            string line_buf = reader.GetLine();
            if (line_buf == "特殊能力なし")
            {
                line_buf = reader.GetLine();
            }
            else if (line_buf == "特殊能力")
            {
                // 新形式による特殊能力表記
                line_buf = LoadFeature(data_name, ud, reader, SRC);
            }
            else if (Strings.InStr(line_buf, "特殊能力,") == 1)
            {
                // 旧形式による特殊能力表記
                buf = Strings.Mid(line_buf, 6);
                ret = 0;
                bool in_quote = false;
                var loopTo2 = Strings.Len(buf);
                int k;
                for (k = 1; k <= loopTo2; k++)
                {
                    switch (Strings.Mid(buf, k, 1) ?? "")
                    {
                        case ",":
                            {
                                if (!in_quote)
                                {
                                    ret = k;
                                    break;
                                }

                                break;
                            }

                        case "\"":
                            {
                                in_quote = !in_quote;
                                break;
                            }
                    }
                }

                int i = 0;
                while (ret > 0)
                {
                    i = (i + 1);
                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    buf = Strings.Mid(buf, ret + 1);
                    ret = Strings.InStr(buf, ",");
                    if (!string.IsNullOrEmpty(buf2))
                    {
                        ud.AddFeature(buf2);
                    }
                    else
                    {
                        SRC.AddDataError(reader.InvalidData(SrcFormatter.Format(i + "番目の特殊能力の設定が間違っています。"), data_name));
                    }
                }

                i = (i + 1);
                buf2 = Strings.Trim(buf);
                if (!string.IsNullOrEmpty(buf2))
                {
                    ud.AddFeature(buf2);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(SrcFormatter.Format(i + "番目の特殊能力の設定が間違っています。"), data_name));
                }

                line_buf = reader.GetLine();
            }
            else
            {
                throw reader.InvalidDataException(@"特殊能力の設定が抜けています。", data_name);
            }

            return line_buf;
        }

        public static string LoadFeature(string data_name, IUnitDataElements ud, SrcDataReader reader, SRC SRC)
        {
            string line_buf = reader.GetLine();
            string buf = line_buf;
            string buf2;
            int i = 0;
            while (true)
            {
                // パイロットデータ時には空行やデータ区切りでブレークする。
                if (string.IsNullOrEmpty(line_buf) || line_buf == "===") { break; }

                i = i + 1;
                int ret = 0;
                bool in_quote = false;
                var loopTo1 = Strings.Len(buf);
                int j;
                for (j = 1; j <= loopTo1; j++)
                {
                    switch (Strings.Mid(buf, j, 1) ?? "")
                    {
                        case ",":
                            {
                                if (!in_quote)
                                {
                                    ret = j;
                                    break;
                                }
                                break;
                            }

                        case "\"":
                            {
                                in_quote = !in_quote;
                                break;
                            }
                    }
                }

                if (ret > 0)
                {
                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                    if (j == 1)
                    {
                        if (Information.IsNumeric(buf2))
                        {
                            break;
                        }
                    }
                    buf = Strings.Trim(Strings.Mid(buf, ret + 1));
                }
                else
                {
                    buf2 = buf;
                    buf = "";
                }

                // ユニットとアイテムは特殊能力に続いて数値のみの行が続く。
                if (Information.IsNumeric(buf2.Replace("+", "").Replace("-", "")))
                {
                    break;
                }
                else if (string.IsNullOrEmpty(buf2) || Information.IsNumeric(buf2))
                {
                    SRC.AddDataError(reader.InvalidData("行頭から" + i + "番目の特殊能力の設定が間違っています。", data_name));
                }
                else
                {
                    ud.AddFeature(buf2);
                }

                if (string.IsNullOrEmpty(buf))
                {
                    line_buf = reader.GetLine();
                    buf = line_buf;
                    i = 0;
                }
            }
            return line_buf;
        }

        public static string LoadWepon(string data_name, IUnitDataElements ud, SrcDataReader reader, SRC SRC)
        {
            int ret;
            string buf;
            string buf2;
            string line_buf = reader.GetLine();
            // アビリティとの区切り文字かアイテムの解説行で終了
            while (Strings.Len(line_buf) > 0
                && line_buf != "==="
                && !line_buf.StartsWith("*"))
            {
                // 武器名
                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"武器データの終りには空行を置いてください。", data_name);
                }

                string wname = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (string.IsNullOrEmpty(wname))
                {
                    throw reader.InvalidDataException(@"武器名の設定が間違っています。", data_name);
                }

                // 武器を登録
                WeaponData wd = ud.AddWeapon(wname);

                // 攻撃力
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@wname + "の最小射程が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    wd.Power = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99999);
                }
                else if (buf == "-")
                {
                    wd.Power = 0;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@wname + "の攻撃力の設定が間違っています。", data_name));
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        wd.Power = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                    }
                }

                // 最小射程
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@wname + "の最大射程の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    wd.MinRange = Conversions.ToInteger(buf2);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@wname + "の最小射程の設定が間違っています。", data_name));
                    wd.MinRange = 1;
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        wd.MinRange = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                    }
                }

                // 最大射程
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@wname + "の命中率の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    wd.MaxRange = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@wname + "の最大射程の設定が間違っています。", data_name));
                    wd.MaxRange = 1;
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        wd.MaxRange = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                    }
                }

                // 命中率
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@wname + "の弾数の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    int n = Conversions.ToInteger(buf2);
                    if (n > 999)
                    {
                        n = 999;
                    }
                    else if (n < -999)
                    {
                        n = -999;
                    }

                    wd.Precision = n;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@wname + "の命中率の設定が間違っています。", data_name));
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        wd.Precision = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                    }
                }

                // 弾数
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@wname + "の消費ＥＮの設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (buf2 != "-")
                {
                    if (Information.IsNumeric(buf2))
                    {
                        wd.Bullet = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                    }
                    else
                    {
                        SRC.AddDataError(reader.InvalidData(@wname + "の弾数の設定が間違っています。", data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            wd.Bullet = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                        }
                    }
                }

                // 消費ＥＮ
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@wname + "の必要気力が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (buf2 != "-")
                {
                    if (Information.IsNumeric(buf2))
                    {
                        wd.ENConsumption = GeneralLib.MinLng(Conversions.ToInteger(buf2), 999);
                    }
                    else
                    {
                        SRC.AddDataError(reader.InvalidData(@wname + "の消費ＥＮの設定が間違っています。", data_name));
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            wd.ENConsumption = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                        }
                    }
                }

                // 必要気力
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@wname + "の地形適応が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (buf2 != "-")
                {
                    if (Information.IsNumeric(buf2))
                    {
                        int n = Conversions.ToInteger(buf2);
                        if (n > 1000)
                        {
                            n = 1000;
                        }
                        else if (n < 0)
                        {
                            n = 0;
                        }

                        wd.NecessaryMorale = n;
                    }
                    else
                    {
                        SRC.AddDataError(reader.InvalidData(@wname + "の必要気力の設定が間違っています。", data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            wd.NecessaryMorale = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                        }
                    }
                }

                // 地形適応
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@wname + "のクリティカル率が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Strings.Len(buf2) == 4)
                {
                    wd.Adaption = buf2;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@wname + "の地形適応の設定が間違っています。", data_name));
                    wd.Adaption = "----";
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        wd.Adaption = GeneralLib.LIndex(buf2, 1);
                    }
                }

                // クリティカル率
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@wname + "の武器属性が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    int n = Conversions.ToInteger(buf2);
                    if (n > 999)
                    {
                        n = 999;
                    }
                    else if (n < -999)
                    {
                        n = -999;
                    }

                    wd.Critical = n;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@wname + "のクリティカル率の設定が間違っています。", data_name));
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        wd.Critical = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                    }
                }

                // 武器属性
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    SRC.AddDataError(reader.InvalidData(@wname + "の武器属性の設定が間違っています。", data_name));
                }

                if (Strings.Right(buf, 1) == ")")
                {
                    // 必要技能
                    ret = Strings.InStr(buf, "> ");
                    if (ret > 0)
                    {
                        if (ret > 0)
                        {
                            wd.NecessarySkill = Strings.Mid(buf, ret + 2);
                            buf = Strings.Trim(Strings.Left(buf, ret + 1));
                            ret = Strings.InStr(wd.NecessarySkill, "(");
                            wd.NecessarySkill = Strings.Mid(wd.NecessarySkill, ret + 1, Strings.Len(wd.NecessarySkill) - ret - 1);
                        }
                    }
                    else
                    {
                        ret = Strings.InStr(buf, "(");
                        if (ret > 0)
                        {
                            wd.NecessarySkill = Strings.Trim(Strings.Mid(buf, ret + 1, Strings.Len(buf) - ret - 1));
                            buf = Strings.Trim(Strings.Left(buf, ret - 1));
                        }
                    }
                }

                if (Strings.Right(buf, 1) == ">")
                {
                    // 必要条件
                    ret = Strings.InStr(buf, "<");
                    if (ret > 0)
                    {
                        wd.NecessaryCondition = Strings.Trim(Strings.Mid(buf, ret + 1, Strings.Len(buf) - ret - 1));
                        buf = Strings.Trim(Strings.Left(buf, ret - 1));
                    }
                }

                wd.Class = buf;
                if (wd.Class == "-")
                {
                    wd.Class = "";
                }

                if (Strings.InStr(wd.Class, "Lv") > 0)
                {
                    SRC.AddDataError(reader.InvalidData(@wname + "の属性のレベル指定が間違っています。", data_name));
                }

                if (reader.EOT)
                {
                    return "";
                }

                line_buf = reader.GetLine();
            }
            return line_buf;
        }

        public static string LoadAbility(string data_name, IUnitDataElements ud, SrcDataReader reader, SRC SRC)
        {
            int ret;
            string buf;
            string buf2;
            string line_buf = reader.GetLine();
            // アイテムの解説行で終了
            while (Strings.Len(line_buf) > 0
                && !line_buf.StartsWith("*"))
            {
                // アビリティ名
                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"アビリティデータの終りに空行を置いてください。", data_name);
                }

                string sname = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (string.IsNullOrEmpty(sname))
                {
                    throw reader.InvalidDataException(@"アビリティ名の設定が間違っています。", data_name);
                }

                // アビリティを登録
                AbilityData sd = ud.AddAbility(sname);

                // 効果
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@sname + "の射程の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                sd.SetEffect(buf2);

                // 射程
                sd.MinRange = 0;
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@sname + "の回数の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    sd.MaxRange = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                }
                else if (buf2 == "-")
                {
                    sd.MaxRange = 0;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@sname + "の射程の設定が間違っています。", data_name));
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        sd.MaxRange = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                    }
                }

                // 回数
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@sname + "の消費ＥＮの設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (buf2 != "-")
                {
                    if (Information.IsNumeric(buf2))
                    {
                        sd.Stock = GeneralLib.MinLng(Conversions.ToInteger(buf2), 99);
                    }
                    else
                    {
                        SRC.AddDataError(reader.InvalidData(@sname + "の回数の設定が間違っています。", data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            sd.Stock = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                        }
                    }
                }

                // 消費ＥＮ
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@sname + "の必要気力の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (buf2 != "-")
                {
                    if (Information.IsNumeric(buf2))
                    {
                        sd.ENConsumption = GeneralLib.MinLng(Conversions.ToInteger(buf2), 999);
                    }
                    else
                    {
                        SRC.AddDataError(reader.InvalidData(@sname + "の消費ＥＮの設定が間違っています。", data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            sd.ENConsumption = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                        }
                    }
                }

                // 必要気力
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@sname + "のアビリティ属性の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (buf2 != "-")
                {
                    if (Information.IsNumeric(buf2))
                    {
                        int n = Conversions.ToInteger(buf2);
                        if (n > 1000)
                        {
                            n = 1000;
                        }
                        else if (n < 0)
                        {
                            n = 0;
                        }

                        sd.NecessaryMorale = n;
                    }
                    else
                    {
                        SRC.AddDataError(reader.InvalidData(@sname + "の必要気力の設定が間違っています。", data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            sd.NecessaryMorale = GeneralLib.StrToLng(GeneralLib.LIndex(buf2, 1));
                        }
                    }
                }

                // アビリティ属性
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    SRC.AddDataError(reader.InvalidData(@sname + "のアビリティ属性の設定が間違っています。", data_name));
                }

                if (Strings.Right(buf, 1) == ")")
                {
                    // 必要技能
                    ret = Strings.InStr(buf, "> ");
                    if (ret > 0)
                    {
                        if (ret > 0)
                        {
                            sd.NecessarySkill = Strings.Mid(buf, ret + 2);
                            buf = Strings.Trim(Strings.Left(buf, ret + 1));
                            ret = Strings.InStr(sd.NecessarySkill, "(");
                            sd.NecessarySkill = Strings.Mid(sd.NecessarySkill, ret + 1, Strings.Len(sd.NecessarySkill) - ret - 1);
                        }
                    }
                    else
                    {
                        ret = Strings.InStr(buf, "(");
                        if (ret > 0)
                        {
                            sd.NecessarySkill = Strings.Trim(Strings.Mid(buf, ret + 1, Strings.Len(buf) - ret - 1));
                            buf = Strings.Trim(Strings.Left(buf, ret - 1));
                        }
                    }
                }

                if (Strings.Right(buf, 1) == ">")
                {
                    // 必要条件
                    ret = Strings.InStr(buf, "<");
                    if (ret > 0)
                    {
                        sd.NecessaryCondition = Strings.Trim(Strings.Mid(buf, ret + 1, Strings.Len(buf) - ret - 1));
                        buf = Strings.Trim(Strings.Left(buf, ret - 1));
                    }
                }

                sd.Class = buf;
                if (sd.Class == "-")
                {
                    sd.Class = "";
                }

                if (Strings.InStr(sd.Class, "Lv") > 0)
                {
                    SRC.AddDataError(reader.InvalidData(@sname + "の属性のレベル指定が間違っています。", data_name));
                }
                line_buf = reader.GetLine();
            }

            return line_buf;
        }
    }
}
