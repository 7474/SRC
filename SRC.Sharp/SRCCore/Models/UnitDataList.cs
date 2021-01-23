// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRC.Core.Exceptions;
using SRC.Core.Lib;
using SRC.Core.VB;
using System.Collections.Generic;
using System.IO;

namespace SRC.Core.Models
{
    // 全ユニットデータを管理するリストのクラス
    public class UnitDataList
    {
        // ユニットデータ用コレクション
        private SrcCollection<UnitData> colUnitDataList;

        // ID作成用変数
        private int IDNum;

        public UnitDataList()
        {
            colUnitDataList = new SrcCollection<UnitData>();
        }

        private void AddDummyData()
        {

            var ud = new UnitData();
            ud.Name = "ステータス表示用ダミーユニット";
            ud.Nickname = "ユニット無し";
            ud.PilotNum = 1;
            ud.Transportation = "陸";
            ud.Adaption = "AAAA";
            ud.Bitmap = ".bmp";
            string argfdef = "ダミーユニット=システム用非表示能力";
            ud.AddFeature(argfdef);
            colUnitDataList.Add(ud, ud.Name);
        }

        // ユニットデータリストにデータを追加
        public UnitData Add(string uname)
        {
            UnitData AddRet = default;
            var ud = new UnitData();
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
            try
            {
                return colUnitDataList[Index];
            }
            catch
            {
                return null;
            }
        }

        // ユニットデータリストに登録されているか？
        public bool IsDefined(string Index)
        {
            try
            {
                return colUnitDataList[Index] != null;
            }
            catch
            {
                return false;
            }
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
            //try
            //{
            using (var reader = new SrcReader(fname, stream))
            {
                while (reader.HasMore)
                {
                    UnitData ud = LoadUnit(reader); ;
                }
            }
            //}
            //catch (InvalidSrcDataException)
            //{
            //    throw;
            //}
        }

        // reader から１つユニットを読み込む。
        // 返却したUnitDataはリストに追加されている状態。
        private UnitData LoadUnit(SrcReader reader)
        {
            UnitData ud;
            var continuesErrors = new List<InvalidSrcData>();
            int ret;
            string buf, buf2;
            var line_buf = "";

            // 空行をスキップ
            while (reader.HasMore && string.IsNullOrEmpty(line_buf))
            {
                line_buf = reader.GetLine();
            }
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
                throw reader.InvalidDataException("名称に半角スペースは使用出来ません。", data_name);
            }

            if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
            {
                throw reader.InvalidDataException(@"名称に全角括弧は使用出来ません。", data_name);
            }

            if (Strings.InStr(data_name, "\"") > 0)
            {
                throw reader.InvalidDataException(@"名称に\は使用出来ません。", data_name);
            }

            object argIndex2 = (object)data_name;
            if (IsDefined(data_name))
            {
                object argIndex1 = (object)data_name;
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
                ;
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
                ud.Class_Renamed = buf2;
            }
            else
            {
                continuesErrors.Add(reader.InvalidData(@"ユニットクラスの設定が間違っています。", data_name));
                ud.Class_Renamed = "汎用";
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
                    continuesErrors.Add(reader.InvalidData(@"パイロット数の設定が間違っています。", data_name));
                    ud.PilotNum = 1;
                }
                if (ud.PilotNum < 1)
                {
                    continuesErrors.Add(reader.InvalidData(@"パイロット数の設定が間違っています。", data_name));
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
                    continuesErrors.Add(reader.InvalidData(@"パイロット数の設定が間違っています。", data_name));
                    ud.PilotNum = 1;
                }

                if (ud.PilotNum < 1)
                {
                    continuesErrors.Add(reader.InvalidData(@"パイロット数の設定が間違っています。", data_name));
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
                continuesErrors.Add(reader.InvalidData(@"アイテム数の設定が間違っています。", data_name));
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
                continuesErrors.Add(reader.InvalidData(@"移動可能地形の設定が間違っています。", data_name));
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
                continuesErrors.Add(reader.InvalidData(@"移動力の設定が間違っています。", data_name));
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
                        continuesErrors.Add(reader.InvalidData(@"サイズの設定が間違っています。", data_name));
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
                continuesErrors.Add(reader.InvalidData(@"修理費の設定が間違っています。", data_name));
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
                continuesErrors.Add(reader.InvalidData(@"経験値の設定が間違っています。", data_name));
            }

            // 特殊能力データ
            line_buf = reader.GetLine();
            if (line_buf == "特殊能力なし")
            {
                line_buf = reader.GetLine();
            }
            else if (line_buf == "特殊能力")
            {
                // 新形式による特殊能力表記
                line_buf = reader.GetLine();
                buf = line_buf;
                int i = 0;
                while (true)
                {
                    i = i + 1;
                    ret = 0;
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

                    if (Information.IsNumeric(buf2))
                    {
                        break;
                    }
                    else if (string.IsNullOrEmpty(buf2) | Information.IsNumeric(buf2))
                    {
                        string argmsg11 = "行頭から" + i + "番目の特殊能力の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(argmsg11, data_name));
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
                        string argmsg12 = SrcFormatter.Format(i + "番目の特殊能力の設定が間違っています。");
                        continuesErrors.Add(reader.InvalidData(argmsg12, data_name));
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
                    string argmsg13 = SrcFormatter.Format(i + "番目の特殊能力の設定が間違っています。");
                    continuesErrors.Add(reader.InvalidData(argmsg13, data_name));
                }

                line_buf = reader.GetLine();
            }
            else
            {
                throw reader.InvalidDataException(@"特殊能力の設定が抜けています。", data_name);
            }

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
                continuesErrors.Add(reader.InvalidData(@"最大ＨＰの設定が間違っています。", data_name));
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
                continuesErrors.Add(reader.InvalidData(@"最大ＥＮの設定が間違っています。", data_name));
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
                continuesErrors.Add(reader.InvalidData(@"装甲の設定が間違っています。", data_name));
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
                continuesErrors.Add(reader.InvalidData(@"運動性の設定が間違っています。", data_name));
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
                continuesErrors.Add(reader.InvalidData(@"地形適応の設定が間違っています。", data_name));
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
                continuesErrors.Add(reader.InvalidData(@"ビットマップの設定が間違っています。", data_name));
                ud.IsBitmapMissing = true;
            }

            if (reader.EOT)
            {
                return ud;
            }

            // 武器データ
            line_buf = reader.GetLine();
            while (Strings.Len(line_buf) > 0 & line_buf != "===")
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
                    continuesErrors.Add(reader.InvalidData(@wname + "の攻撃力の設定が間違っています。", data_name));
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        string argexpr = GeneralLib.LIndex(buf2, 1);
                        wd.Power = GeneralLib.StrToLng(argexpr);
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
                    continuesErrors.Add(reader.InvalidData(@wname + "の最小射程の設定が間違っています。", data_name));
                    wd.MinRange = 1;
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        string argexpr1 = GeneralLib.LIndex(buf2, 1);
                        wd.MinRange = GeneralLib.StrToLng(argexpr1);
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
                    continuesErrors.Add(reader.InvalidData(@wname + "の最大射程の設定が間違っています。", data_name));
                    wd.MaxRange = 1;
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        string argexpr2 = GeneralLib.LIndex(buf2, 1);
                        wd.MaxRange = GeneralLib.StrToLng(argexpr2);
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
                    continuesErrors.Add(reader.InvalidData(@wname + "の命中率の設定が間違っています。", data_name));
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        string argexpr3 = GeneralLib.LIndex(buf2, 1);
                        wd.Precision = GeneralLib.StrToLng(argexpr3);
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
                        continuesErrors.Add(reader.InvalidData(@wname + "の弾数の設定が間違っています。", data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr4 = GeneralLib.LIndex(buf2, 1);
                            wd.Bullet = GeneralLib.StrToLng(argexpr4);
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
                        continuesErrors.Add(reader.InvalidData(@wname + "の消費ＥＮの設定が間違っています。", data_name));
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr5 = GeneralLib.LIndex(buf2, 1);
                            wd.ENConsumption = GeneralLib.StrToLng(argexpr5);
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
                        continuesErrors.Add(reader.InvalidData(@wname + "の必要気力の設定が間違っています。", data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr6 = GeneralLib.LIndex(buf2, 1);
                            wd.NecessaryMorale = GeneralLib.StrToLng(argexpr6);
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
                    continuesErrors.Add(reader.InvalidData(@wname + "の地形適応の設定が間違っています。", data_name));
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
                    continuesErrors.Add(reader.InvalidData(@wname + "のクリティカル率の設定が間違っています。", data_name));
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        string argexpr7 = GeneralLib.LIndex(buf2, 1);
                        wd.Critical = GeneralLib.StrToLng(argexpr7);
                    }
                }

                // 武器属性
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    continuesErrors.Add(reader.InvalidData(@wname + "の武器属性の設定が間違っています。", data_name));
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

                wd.Class_Renamed = buf;
                if (wd.Class_Renamed == "-")
                {
                    wd.Class_Renamed = "";
                }

                if (Strings.InStr(wd.Class_Renamed, "Lv") > 0)
                {
                    continuesErrors.Add(reader.InvalidData(@wname + "の属性のレベル指定が間違っています。", data_name));
                }

                if (reader.EOT)
                {
                    return ud;
                }

                line_buf = reader.GetLine();
            }

            if (line_buf != "===")
            {
                return ud;
            }

            // アビリティデータ
            line_buf = reader.GetLine();
            while (Strings.Len(line_buf) > 0)
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
                    continuesErrors.Add(reader.InvalidData(@sname + "の射程の設定が間違っています。", data_name));
                    if (GeneralLib.LLength(buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                        string argexpr8 = GeneralLib.LIndex(buf2, 1);
                        sd.MaxRange = GeneralLib.StrToLng(argexpr8);
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
                        continuesErrors.Add(reader.InvalidData(@sname + "の回数の設定が間違っています。", data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr9 = GeneralLib.LIndex(buf2, 1);
                            sd.Stock = GeneralLib.StrToLng(argexpr9);
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
                        continuesErrors.Add(reader.InvalidData(@sname + "の消費ＥＮの設定が間違っています。", data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr10 = GeneralLib.LIndex(buf2, 1);
                            sd.ENConsumption = GeneralLib.StrToLng(argexpr10);
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
                        continuesErrors.Add(reader.InvalidData(@sname + "の必要気力の設定が間違っています。", data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr11 = GeneralLib.LIndex(buf2, 1);
                            sd.NecessaryMorale = GeneralLib.StrToLng(argexpr11);
                        }
                    }
                }

                // アビリティ属性
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    continuesErrors.Add(reader.InvalidData(@sname + "のアビリティ属性の設定が間違っています。", data_name));
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

                sd.Class_Renamed = buf;
                if (sd.Class_Renamed == "-")
                {
                    sd.Class_Renamed = "";
                }

                if (Strings.InStr(sd.Class_Renamed, "Lv") > 0)
                {
                    continuesErrors.Add(reader.InvalidData(@sname + "の属性のレベル指定が間違っています。", data_name));
                }
            }
            return ud;
        }
    }
}