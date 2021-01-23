// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。

using SRC.Core.Exceptions;
using SRC.Core.Lib;
using SRC.Core.VB;
using System;
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
        public void Load(string fname, Stream stream)
        {
            int FileNumber;
            int j, i, k;
            int n;
            string buf, buf2;
            int ret;
            WeaponData wd;
            AbilityData sd;
            string wname, sname;
            string data_name;
            string err_msg;
            bool in_quote;
            int comma_num;
            ;

            try
            {
                using (var reader = new SrcReader(fname, stream))
                {
                    while (reader.HasMore)
                    {
                        UnitData ud = Load1Unit(reader); ;
                    }
                }
            }
            catch (InvalidSrcDataException)
            {
                throw;
            }
        }

        // reader から１つユニットを読み込む。
        // 返却したUnitDataはリストに追加されている状態。
        private UnitData Load1Unit(SrcReader reader)
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


            // 名称
            string data_name = "";
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
                    GUI.DataErrorMessage(ref argmsg3, ref fname, line_num, ref line_buf, ref ud.Name);
                    ud.PilotNum = 1;
                }

                if (ud.PilotNum < 1)
                {
                    continuesErrors.Add(reader.InvalidData(@"パイロット数の設定が間違っています。", data_name));
                    GUI.DataErrorMessage(ref argmsg4, ref fname, line_num, ref line_buf, ref ud.Name);
                    ud.PilotNum = 1;
                }

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
                GUI.DataErrorMessage(ref argmsg5, ref fname, line_num, ref line_buf, ref ud.Name);
                ud.ItemNum = 4;
            }

            // 移動可能地形, 移動力, サイズ, 修理費, 経験値
            GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

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
                GUI.DataErrorMessage(ref argmsg6, ref fname, line_num, ref line_buf, ref ud.Name);
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
                GUI.DataErrorMessage(ref argmsg7, ref fname, line_num, ref line_buf, ref ud.Name);
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
                        GUI.DataErrorMessage(ref argmsg8, ref fname, line_num, ref line_buf, ref ud.Name);
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
                GUI.DataErrorMessage(ref argmsg9, ref fname, line_num, ref line_buf, ref ud.Name);
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
                GUI.DataErrorMessage(ref argmsg10, ref fname, line_num, ref line_buf, ref ud.Name);
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
                GUI.DataErrorMessage(ref argmsg14, ref fname, line_num, ref line_buf, ref ud.Name);
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
                GUI.DataErrorMessage(ref argmsg15, ref fname, line_num, ref line_buf, ref ud.Name);
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
                GUI.DataErrorMessage(ref argmsg16, ref fname, line_num, ref line_buf, ref ud.Name);
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
                GUI.DataErrorMessage(ref argmsg17, ref fname, line_num, ref line_buf, ref ud.Name);
            }

            // 地形適応, ビットマップ
            GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

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
                GUI.DataErrorMessage(ref argmsg18, ref fname, line_num, ref line_buf, ref ud.Name);
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
                GUI.DataErrorMessage(ref argmsg19, ref fname, line_num, ref line_buf, ref ud.Name);
                ud.IsBitmapMissing = true;
            }

            if (FileSystem.EOF(FileNumber))
            {
                FileSystem.FileClose(FileNumber);
                return;
            }

            // 武器データ
            GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
            while (Strings.Len(line_buf) > 0 & line_buf != "===")
            {
                // 武器名
                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"武器データの終りには空行を置いてください。", data_name);
                }

                wname = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (string.IsNullOrEmpty(wname))
                {
                    throw reader.InvalidDataException(@"武器名の設定が間違っています。", data_name);
                }

                // 武器を登録
                wd = ud.AddWeapon(ref wname);

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
                    GUI.DataErrorMessage(ref argmsg20, ref fname, line_num, ref line_buf, ref ud.Name);
                    if (GeneralLib.LLength(ref buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                        string argexpr = GeneralLib.LIndex(ref buf2, 1);
                        wd.Power = GeneralLib.StrToLng(ref argexpr);
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
                    wd.MinRange = Conversions.ToShort(buf2);
                }
                else
                {
                    continuesErrors.Add(reader.InvalidData(@wname + "の最小射程の設定が間違っています。", data_name));
                    GUI.DataErrorMessage(ref argmsg21, ref fname, line_num, ref line_buf, ref ud.Name);
                    wd.MinRange = 1;
                    if (GeneralLib.LLength(ref buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                        string argexpr1 = GeneralLib.LIndex(ref buf2, 1);
                        wd.MinRange = GeneralLib.StrToLng(ref argexpr1);
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
                    GUI.DataErrorMessage(ref argmsg22, ref fname, line_num, ref line_buf, ref ud.Name);
                    wd.MaxRange = 1;
                    if (GeneralLib.LLength(ref buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                        string argexpr2 = GeneralLib.LIndex(ref buf2, 1);
                        wd.MaxRange = GeneralLib.StrToLng(ref argexpr2);
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
                    n = Conversions.ToInteger(buf2);
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
                    GUI.DataErrorMessage(ref argmsg23, ref fname, line_num, ref line_buf, ref ud.Name);
                    if (GeneralLib.LLength(ref buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                        string argexpr3 = GeneralLib.LIndex(ref buf2, 1);
                        wd.Precision = GeneralLib.StrToLng(ref argexpr3);
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
                        GUI.DataErrorMessage(ref argmsg24, ref fname, line_num, ref line_buf, ref ud.Name);
                        if (GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                            string argexpr4 = GeneralLib.LIndex(ref buf2, 1);
                            wd.Bullet = GeneralLib.StrToLng(ref argexpr4);
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
                        GUI.DataErrorMessage(ref argmsg25, ref fname, line_num, ref line_buf, ref ud.Name);
                        if (GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                            string argexpr5 = GeneralLib.LIndex(ref buf2, 1);
                            wd.ENConsumption = GeneralLib.StrToLng(ref argexpr5);
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
                        n = Conversions.ToInteger(buf2);
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
                        GUI.DataErrorMessage(ref argmsg26, ref fname, line_num, ref line_buf, ref ud.Name);
                        if (GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                            string argexpr6 = GeneralLib.LIndex(ref buf2, 1);
                            wd.NecessaryMorale = GeneralLib.StrToLng(ref argexpr6);
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
                    GUI.DataErrorMessage(ref argmsg27, ref fname, line_num, ref line_buf, ref ud.Name);
                    wd.Adaption = "----";
                    if (GeneralLib.LLength(ref buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                        wd.Adaption = GeneralLib.LIndex(ref buf2, 1);
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
                    n = Conversions.ToInteger(buf2);
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
                    GUI.DataErrorMessage(ref argmsg28, ref fname, line_num, ref line_buf, ref ud.Name);
                    if (GeneralLib.LLength(ref buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                        string argexpr7 = GeneralLib.LIndex(ref buf2, 1);
                        wd.Critical = GeneralLib.StrToLng(ref argexpr7);
                    }
                }

                // 武器属性
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    continuesErrors.Add(reader.InvalidData(@wname + "の武器属性の設定が間違っています。", data_name));
                    GUI.DataErrorMessage(ref argmsg29, ref fname, line_num, ref line_buf, ref ud.Name);
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
                    GUI.DataErrorMessage(ref argmsg30, ref fname, line_num, ref line_buf, ref ud.Name);
                }

                if (FileSystem.EOF(FileNumber))
                {
                    FileSystem.FileClose(FileNumber);
                    return;
                }

                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
            }

            if (line_buf != "===")
            {
                return ud;
            }

            // アビリティデータ
            GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
            while (Strings.Len(line_buf) > 0)
            {
                // アビリティ名
                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"アビリティデータの終りに空行を置いてください。", data_name);
                }

                sname = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (string.IsNullOrEmpty(sname))
                {
                    throw reader.InvalidDataException(@"アビリティ名の設定が間違っています。", data_name);
                }

                // アビリティを登録
                sd = ud.AddAbility(ref sname);

                // 効果
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@sname + "の射程の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                sd.SetEffect(ref buf2);

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
                    GUI.DataErrorMessage(ref argmsg31, ref fname, line_num, ref line_buf, ref ud.Name);
                    if (GeneralLib.LLength(ref buf2) > 1)
                    {
                        buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                        string argexpr8 = GeneralLib.LIndex(ref buf2, 1);
                        sd.MaxRange = GeneralLib.StrToLng(ref argexpr8);
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
                        GUI.DataErrorMessage(ref argmsg32, ref fname, line_num, ref line_buf, ref ud.Name);
                        if (GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                            string argexpr9 = GeneralLib.LIndex(ref buf2, 1);
                            sd.Stock = GeneralLib.StrToLng(ref argexpr9);
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
                        GUI.DataErrorMessage(ref argmsg33, ref fname, line_num, ref line_buf, ref ud.Name);
                        if (GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                            string argexpr10 = GeneralLib.LIndex(ref buf2, 1);
                            sd.ENConsumption = GeneralLib.StrToLng(ref argexpr10);
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
                        n = Conversions.ToInteger(buf2);
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
                        GUI.DataErrorMessage(ref argmsg34, ref fname, line_num, ref line_buf, ref ud.Name);
                        if (GeneralLib.LLength(ref buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(ref buf2, 2) + "," + buf;
                            string argexpr11 = GeneralLib.LIndex(ref buf2, 1);
                            sd.NecessaryMorale = GeneralLib.StrToLng(ref argexpr11);
                        }
                    }
                }

                // アビリティ属性
                buf = Strings.Trim(buf);
                if (Strings.Len(buf) == 0)
                {
                    continuesErrors.Add(reader.InvalidData(@sname + "のアビリティ属性の設定が間違っています。", data_name));
                    GUI.DataErrorMessage(ref argmsg35, ref fname, line_num, ref line_buf, ref ud.Name);
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
                    GUI.DataErrorMessage(ref argmsg36, ref fname, line_num, ref line_buf, ref ud.Name);
                }
            }

        }