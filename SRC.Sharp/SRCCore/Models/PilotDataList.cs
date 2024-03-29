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
    // 全パイロットデータを管理するリストのクラス
    public class PilotDataList
    {
        // パイロットデータのコレクション
        private SrcCollection<PilotData> colPilotDataList;

        public IList<PilotData> Items => colPilotDataList.List;

        public string Raw = "";
        public string DataComment = "";

        private SRC SRC;
        public PilotDataList(SRC src)
        {
            SRC = src;
            colPilotDataList = new SrcCollection<PilotData>();
            AddDummyData();
        }
        private void AddDummyData()
        {
            var pd = new PilotData(SRC);

            // ユニットステータスコマンドの無人ユニット用
            pd.Name = "ステータス表示用ダミーパイロット(ザコ)";
            pd.Nickname = "パイロット不在";
            pd.Adaption = "AAAA";
            pd.Bitmap = ".bmp";
            colPilotDataList.Add(pd, pd.Name);
        }
        public void Clear()
        {
            colPilotDataList.Clear();
            Raw = "";
            DataComment = "";
        }

        // パイロットデータリストにデータを追加
        public PilotData Add(string pname)
        {
            var new_pilot_data = new PilotData(SRC);
            new_pilot_data.Name = pname;
            colPilotDataList.Add(new_pilot_data, pname);
            return new_pilot_data;
        }

        // パイロットデータリストに登録されているデータの総数
        public int Count()
        {
            int CountRet = default;
            CountRet = colPilotDataList.Count;
            return CountRet;
        }

        // パイロットデータリストから指定したデータを消去
        public void Delete(string Index)
        {
            colPilotDataList.Remove(Index);
        }

        // パイロットデータリストから指定したデータを取り出す
        public PilotData Item(string Index)
        {
            try
            {
                var pilot = colPilotDataList[Index];
                if (pilot != null) { return pilot; }
                var pname = Conversions.ToString(Index);
                foreach (PilotData pd in colPilotDataList)
                {
                    if ((pd.Nickname ?? "") == (pname ?? ""))
                    {
                        return pd;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        // パイロットデータリストに指定したデータが登録されているか？
        public bool IsDefined(string Index)
        {
            return Item(Index) != null;
        }

        // パイロットデータリストに指定したデータが登録されているか？ (愛称は見ない)
        public bool IsDefined2(string Index)
        {
            return colPilotDataList.ContainsKey(Index);
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
                PilotData lastPd = null;
                while (reader.HasMore)
                {
                    lastPd = LoadPilot(reader, lastPd);
                }
                Raw = reader.Raw;
            }
        }

        public PilotData LoadPilot(SrcDataReader reader, PilotData lastPd)
        {
            PilotData pd = null;
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
                if (lastPd != null)
                {
                    lastPd.DataComment = reader.RawComment.Trim();
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
                    throw reader.InvalidDataException(@"名称に半角スペースは使用出来ません。", data_name);
                }

                if (Strings.InStr(data_name, "（") > 0 || Strings.InStr(data_name, "）") > 0)
                {
                    throw reader.InvalidDataException(@"名称に全角括弧は使用出来ません", data_name);
                }

                if (Strings.InStr(data_name, "\"") > 0)
                {
                    throw reader.InvalidDataException("名称に\"は使用出来ません。", data_name);
                }

                if (IsDefined2(data_name))
                {
                    // すでに定義済みのパイロットの場合はデータを置き換え
                    pd = Item(data_name);
                    pd.Clear();
                }
                else
                {
                    pd = Add(data_name);
                }

                // 愛称, 読み仮名, 性別, クラス, 地形適応, 経験値
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
                else if (comma_num > 5)
                {
                    throw reader.InvalidDataException(@"余分な「,」があります。", data_name);
                }

                // 愛称
                int ret = Strings.InStr(line_buf, ",");
                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (Strings.Len(buf2) == 0)
                {
                    throw reader.InvalidDataException(@"愛称の設定が抜けています。", data_name);
                }

                pd.Nickname = buf2;
                switch (comma_num)
                {
                    case 4:
                        {
                            // 読み仮名 or 性別
                            ret = Strings.InStr(buf, ",");
                            buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                            buf = Strings.Mid(buf, ret + 1);
                            switch (buf2 ?? "")
                            {
                                case "男性":
                                case "女性":
                                case "-":
                                    {
                                        pd.KanaName = GeneralLib.StrToHiragana(pd.Nickname);
                                        pd.Sex = buf2;
                                        break;
                                    }

                                default:
                                    {
                                        pd.KanaName = buf2;
                                        break;
                                    }
                            }

                            break;
                        }

                    case 5:
                        {
                            // 読み仮名
                            ret = Strings.InStr(buf, ",");
                            buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                            buf = Strings.Mid(buf, ret + 1);
                            switch (buf2 ?? "")
                            {
                                case "男性":
                                case "女性":
                                case "-":
                                    {
                                        SRC.AddDataError(reader.InvalidData(@"読み仮名の設定が抜けています。", data_name));
                                        pd.KanaName = GeneralLib.StrToHiragana(pd.Nickname);
                                        break;
                                    }

                                default:
                                    {
                                        pd.KanaName = buf2;
                                        break;
                                    }
                            }

                            // 性別
                            ret = Strings.InStr(buf, ",");
                            buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                            buf = Strings.Mid(buf, ret + 1);
                            switch (buf2 ?? "")
                            {
                                case "男性":
                                case "女性":
                                case "-":
                                    {
                                        pd.Sex = buf2;
                                        break;
                                    }

                                default:
                                    {
                                        SRC.AddDataError(reader.InvalidData(@"性別の設定が間違っています。", data_name));
                                        break;
                                    }
                            }

                            break;
                        }

                    default:
                        {
                            pd.KanaName = GeneralLib.StrToHiragana(pd.Nickname);
                            break;
                        }
                }

                // クラス
                ret = Strings.InStr(buf, ",");
                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (!Information.IsNumeric(buf2))
                {
                    pd.Class = buf2;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"クラスの設定が間違っています。", data_name));
                }

                // 地形適応
                ret = Strings.InStr(buf, ",");
                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Strings.Len(buf2) == 4)
                {
                    pd.Adaption = buf2;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"地形適応の設定が間違っています。", data_name));
                    pd.Adaption = "AAAA";
                }

                // 経験値
                buf2 = Strings.Trim(buf);
                if (Information.IsNumeric(buf2))
                {
                    pd.ExpValue = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"経験値の設定が間違っています。", data_name));
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
                    string aname = "";
                    string adata = "";
                    double alevel = Constants.DEFAULT_LEVEL;
                    while (true)
                    {
                        i = (i + 1);

                        // コンマの位置を検索
                        ret = Strings.InStr(buf, ",");
                        // 「"」が使われているか検索
                        int ret2 = Strings.InStr(buf, "\"");
                        if (ret2 < ret && ret2 > 0)
                        {
                            // 「"」が見つかった場合、次の「"」後のコンマを検索
                            bool in_quote = true;
                            int j = (ret2 + 1);
                            while (j <= Strings.Len(buf))
                            {
                                switch (Strings.Mid(buf, j, 1) ?? "")
                                {
                                    case "\"":
                                        {
                                            in_quote = !in_quote;
                                            break;
                                        }

                                    case ",":
                                        {
                                            if (!in_quote)
                                            {
                                                buf2 = Strings.Left(buf, j - 1);
                                                buf = Strings.Mid(buf, j + 1);
                                            }

                                            break;
                                        }
                                }

                                j = (j + 1);
                            }

                            if (j == Strings.Len(buf))
                            {
                                buf2 = buf;
                                buf = "";
                            }

                            in_quote = false;
                        }
                        else if (ret > 0)
                        {
                            // コンマが見つかったらコンマまでの文字列を切り出す
                            buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                            buf = Strings.Trim(Strings.Mid(buf, ret + 1));

                            // コンマの後ろの文字列が空白の場合
                            if (string.IsNullOrEmpty(buf))
                            {
                                if (i % 2 == 1)
                                {
                                    throw reader.InvalidDataException(@"行末の「,」の後に特殊能力指定が抜けています。", data_name);
                                }
                                else
                                {
                                    throw reader.InvalidDataException(@"行末の「,」の後に特殊能力レベル指定が抜けています。", data_name);
                                };
                            }
                        }
                        else
                        {
                            // 行末の文字列
                            buf2 = buf;
                            buf = "";
                        }

                        if (i % 2 == 1)
                        {
                            // 特殊能力名＆レベル

                            if (Information.IsNumeric(buf2))
                            {
                                if (i == 1)
                                {
                                    // 特殊能力の指定は終り。能力値の指定へ
                                    buf = buf2 + ", " + buf;
                                    break;
                                }
                                else
                                {
                                    SRC.AddDataError(reader.InvalidData(@"行頭から" + SrcFormatter.Format((object)((i + 1) / 2)) + "番目の特殊能力名の設定が間違っています。", data_name));
                                }
                            }

                            if (Strings.InStr(buf2, " ") > 0)
                            {
                                if (Strings.Left(buf2, 4) != "先手必勝" && Strings.Left(buf2, 6) != "ＳＰ消費減少" && Strings.Left(buf2, 12) != "スペシャルパワー自動発動" && Strings.Left(buf2, 4) != "ハンター" && Strings.InStr(buf2, "=解説 ") == 0)
                                {
                                    if (string.IsNullOrEmpty(aname))
                                    {
                                        throw reader.InvalidDataException(@"行頭から" + SrcFormatter.Format((object)((i + 1) / 2)) + "番目の特殊能力「" + Strings.Trim(Strings.Left(buf2, Strings.InStr(buf2, " "))) + "」の指定の後に「,」が抜けています。", data_name);
                                    }
                                    else
                                    {
                                        throw reader.InvalidDataException(@"特殊能力「" + aname + "」のレベル指定の後に「,」が抜けています。", data_name);
                                    };
                                }
                            }

                            // 特殊能力の別名指定がある？
                            int j = Strings.InStr(buf2, "=");
                            if (j > 0)
                            {
                                adata = Strings.Mid(buf2, j + 1);
                                buf2 = Strings.Left(buf2, j - 1);
                            }
                            else
                            {
                                adata = "";
                            }

                            // 特殊能力のレベル指定を切り出す
                            j = Strings.InStr(buf2, "Lv");
                            switch (j)
                            {
                                case 0:
                                    {
                                        // 指定なし
                                        aname = buf2;
                                        alevel = Constants.DEFAULT_LEVEL;
                                        break;
                                    }

                                case 1:
                                    {
                                        // レベル指定のみあり
                                        if (!Information.IsNumeric(Strings.Mid(buf2, j + 2)))
                                        {
                                            SRC.AddDataError(reader.InvalidData(@"特殊能力「" + aname + "」のレベル指定が不正です。", data_name));
                                        }

                                        alevel = Conversions.ToDouble(Strings.Mid(buf2, j + 2));
                                        if (string.IsNullOrEmpty(aname))
                                        {
                                            SRC.AddDataError(reader.InvalidData(@"行頭から" + SrcFormatter.Format((object)((i + 1) / 2)) + "番目の特殊能力名の設定が間違っています。", data_name));
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        // 特殊能力名とレベルの両方が指定されている
                                        aname = Strings.Left(buf2, j - 1);
                                        alevel = Conversions.ToDouble(Strings.Mid(buf2, j + 2));
                                        break;
                                    }
                            }
                        }
                        // 特殊能力修得レベル
                        else if (Information.IsNumeric(buf2))
                        {
                            pd.AddSkill(aname, alevel, adata, Conversions.ToInteger(buf2));
                        }
                        else
                        {
                            if (alevel > 0d)
                            {
                                SRC.AddDataError(reader.InvalidData(@"特殊能力「" + aname + "Lv" + SrcFormatter.Format((object)alevel) + "」の修得レベルが間違っています。", data_name));
                            }
                            else
                            {
                                SRC.AddDataError(reader.InvalidData(@"特殊能力「" + aname + "」の修得レベルが間違っています。", data_name));
                            }

                            pd.AddSkill(aname, alevel, adata, 1);
                        }

                        if (string.IsNullOrEmpty(buf))
                        {
                            // ここでこの行は終り

                            // iが奇数の場合は修得レベルが抜けている
                            if (i % 2 == 1)
                            {
                                if (alevel > 0d)
                                {
                                    SRC.AddDataError(reader.InvalidData(@"特殊能力「" + aname + "Lv" + SrcFormatter.Format((object)alevel) + "」の修得レベルが間違っています。", data_name));
                                }
                                else
                                {
                                    SRC.AddDataError(reader.InvalidData(@"特殊能力「" + aname + "」の修得レベルが間違っています。", data_name));
                                }
                            }

                            line_buf = reader.GetLine();
                            buf = line_buf;
                            i = 0;
                            aname = "";
                        }
                    }
                }
                else if (Strings.InStr(line_buf, "特殊能力,") == 1)
                {
                    // 旧形式による特殊能力表記
                    buf = Strings.Mid(line_buf, 6);
                    int i = 0;
                    string aname = "";
                    string adata = "";
                    double alevel = Constants.DEFAULT_LEVEL;
                    do
                    {
                        i = (i + 1);

                        // コンマの位置を検索
                        ret = Strings.InStr(buf, ",");
                        // 「"」が使われているか検索
                        int ret2 = Strings.InStr(buf, "\"");
                        if (ret2 < ret && ret2 > 0)
                        {
                            // 「"」が見つかった場合、次の「"」後のコンマを検索
                            bool in_quote = true;
                            int j = (ret2 + 1);
                            while (j <= Strings.Len(buf))
                            {
                                switch (Strings.Mid(buf, j, 1) ?? "")
                                {
                                    case "\"":
                                        {
                                            in_quote = !in_quote;
                                            break;
                                        }

                                    case ",":
                                        {
                                            if (!in_quote)
                                            {
                                                buf2 = Strings.Left(buf, j - 1);
                                                buf = Strings.Mid(buf, j + 1);
                                            }

                                            break;
                                        }
                                }

                                j = (j + 1);
                            }

                            if (j == Strings.Len(buf))
                            {
                                buf2 = buf;
                                buf = "";
                            }

                            in_quote = false;
                        }
                        else if (ret > 0)
                        {
                            // コンマが見つかったらコンマまでの文字列を切り出す
                            buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                            buf = Strings.Mid(buf, ret + 1);

                            // コンマの後ろの文字列が空白の場合
                            if (string.IsNullOrEmpty(buf))
                            {
                                if (i % 2 == 1)
                                {
                                    throw reader.InvalidDataException(@"行末の「,」の後に特殊能力指定が抜けています。", data_name);
                                }
                                else
                                {
                                    throw reader.InvalidDataException(@"行末の「,」の後に特殊能力レベル指定が抜けています。", data_name);
                                };
                            }
                        }
                        else
                        {
                            // 行末の文字列
                            buf2 = buf;
                            buf = "";
                        }

                        if (i % 2 == 1)
                        {
                            // 特殊能力名＆レベル

                            if (Strings.InStr(buf2, " ") > 0)
                            {
                                if (string.IsNullOrEmpty(aname))
                                {
                                    throw reader.InvalidDataException(@"行頭から" + SrcFormatter.Format((object)((i + 1) / 2)) + "番目の特殊能力の指定の後に「,」が抜けています。", data_name);
                                }
                                else
                                {
                                    throw reader.InvalidDataException(@"特殊能力「" + aname + "」のレベル指定の後に「,」が抜けています。", data_name);
                                };
                            }

                            // 特殊能力の別名指定がある？
                            int j = Strings.InStr(buf2, "=");
                            if (j > 0)
                            {
                                adata = Strings.Mid(buf2, j + 1);
                                buf2 = Strings.Left(buf2, j - 1);
                            }
                            else
                            {
                                adata = "";
                            }

                            // 特殊能力のレベル指定を切り出す
                            j = Strings.InStr(buf2, "Lv");
                            switch (j)
                            {
                                case 0:
                                    {
                                        // 指定なし
                                        aname = buf2;
                                        alevel = Constants.DEFAULT_LEVEL;
                                        break;
                                    }

                                case 1:
                                    {
                                        // レベル指定のみあり
                                        if (!Information.IsNumeric(Strings.Mid(buf2, j + 2)))
                                        {
                                            throw reader.InvalidDataException(@"特殊能力「" + aname + "」のレベル指定が不正です", data_name);
                                        }

                                        alevel = Conversions.ToDouble(Strings.Mid(buf2, j + 2));
                                        if (string.IsNullOrEmpty(aname))
                                        {
                                            throw reader.InvalidDataException(@"行頭から" + SrcFormatter.Format((object)((i + 1) / 2)) + "番目の特殊能力の名前「" + buf2 + "」が不正です", data_name);
                                        }

                                        break;
                                    }

                                default:
                                    {
                                        // 特殊能力名とレベルの両方が指定されている
                                        aname = Strings.Left(buf2, j - 1);
                                        alevel = Conversions.ToDouble(Strings.Mid(buf2, j + 2));
                                        break;
                                    }
                            }
                        }
                        // 特殊能力修得レベル
                        else if (Information.IsNumeric(buf2))
                        {
                            pd.AddSkill(aname, alevel, adata, Conversions.ToInteger(buf2));
                        }
                        else
                        {
                            if (alevel > 0d)
                            {
                                SRC.AddDataError(reader.InvalidData(@"特殊能力「" + aname + "Lv" + SrcFormatter.Format((object)alevel) + "」の修得レベルが間違っています。", data_name));
                            }
                            else
                            {
                                SRC.AddDataError(reader.InvalidData(@"特殊能力「" + aname + "」の修得レベルが間違っています。", data_name));
                            }

                            pd.AddSkill(aname, alevel, adata, 1);
                        }
                    }
                    while (ret > 0);

                    // iが奇数の場合は修得レベルが抜けている
                    if (i % 2 == 1)
                    {
                        if (alevel > 0d)
                        {
                            SRC.AddDataError(reader.InvalidData(@"特殊能力「" + aname + "Lv" + SrcFormatter.Format((object)alevel) + "」の修得レベルが間違っています。", data_name));
                        }
                        else
                        {
                            SRC.AddDataError(reader.InvalidData(@"特殊能力「" + aname + "」の修得レベルが間違っています。", data_name));
                        }
                    }

                    line_buf = reader.GetLine();
                }
                else
                {
                    throw reader.InvalidDataException(@"特殊能力の設定が抜けています。", data_name);
                }

                // 格闘
                if (Strings.Len(line_buf) == 0)
                {
                    throw reader.InvalidDataException(@"格闘攻撃力の設定が抜けています。", data_name);
                }

                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"射撃攻撃力の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Infight = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"格闘攻撃力の設定が間違っています。", data_name));
                }

                // 射撃
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"命中の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Shooting = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"射撃攻撃力の設定が間違っています。", data_name));
                }

                // 命中
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"回避の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Hit = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"命中の設定が間違っています。", data_name));
                }

                // 回避
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"技量の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Dodge = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"回避の設定が間違っています。", data_name));
                }

                // 技量
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"反応の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Technique = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"技量の設定が間違っています。", data_name));
                }

                // 反応
                ret = Strings.InStr(buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"性格の設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                buf = Strings.Mid(buf, ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    pd.Intuition = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"反応の設定が間違っています。", data_name));
                }

                // 性格
                buf2 = Strings.Trim(buf);
                if (Strings.Len(buf2) == 0)
                {
                    throw reader.InvalidDataException(@"性格の設定が抜けています。", data_name);
                }

                if (Strings.InStr(buf2, ",") > 0)
                {
                    SRC.AddDataError(reader.InvalidData(@"行末に余分なコンマが付けられています。", data_name));
                    buf2 = Strings.Trim(Strings.Left(buf2, Strings.InStr(buf2, ",") - 1));
                }

                if (!Information.IsNumeric(buf2))
                {
                    pd.Personality = buf2;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"性格の設定が間違っています。", data_name));
                }

                // スペシャルパワー
                line_buf = reader.GetLine();
                switch (line_buf ?? "")
                {
                    // スペシャルパワーを持っていない
                    case "ＳＰなし":
                    case "精神なし":
                        {
                            break;
                        }

                    case var @case when @case == "":
                        {
                            throw reader.InvalidDataException(@"スペシャルパワーの設定が抜けています。", data_name);
                        }

                    default:
                        {
                            ret = Strings.InStr(line_buf, ",");
                            if (ret == 0)
                            {
                                throw reader.InvalidDataException(@"ＳＰ値の設定が抜けています。", data_name);
                            }

                            buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                            buf = Strings.Mid(line_buf, ret + 1);
                            if (buf2 != "ＳＰ" && buf2 != "精神")
                            {
                                throw reader.InvalidDataException(@"スペシャルパワーの設定が抜けています。", data_name);
                            }

                            // ＳＰ値
                            ret = Strings.InStr(buf, ",");
                            if (ret > 0)
                            {
                                buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                                buf = Strings.Mid(buf, ret + 1);
                            }
                            else
                            {
                                buf2 = Strings.Trim(buf);
                                buf = "";
                            }

                            if (Information.IsNumeric(buf2))
                            {
                                pd.SP = GeneralLib.MinLng(Conversions.ToInteger(buf2), 9999);
                            }
                            else
                            {
                                SRC.AddDataError(reader.InvalidData(@"ＳＰの設定が間違っています。", data_name));
                                pd.SP = 1;
                            }

                            // スペシャルパワーと獲得レベル
                            ret = Strings.InStr(buf, ",");
                            string sname = "";
                            while (ret > 0)
                            {
                                sname = Strings.Trim(Strings.Left(buf, ret - 1));
                                int sp_cost = 0;
                                buf = Strings.Mid(buf, ret + 1);

                                // ＳＰ消費量
                                if (Strings.InStr(sname, "=") > 0)
                                {
                                    sp_cost = GeneralLib.StrToLng(Strings.Mid(sname, Strings.InStr(sname, "=") + 1));
                                    sname = Strings.Left(sname, Strings.InStr(sname, "=") - 1);
                                }
                                else
                                {
                                    sp_cost = 0;
                                }

                                ret = Strings.InStr(buf, ",");
                                if (ret == 0)
                                {
                                    buf2 = Strings.Trim(buf);
                                    buf = "";
                                }
                                else
                                {
                                    buf2 = Strings.Trim(Strings.Left(buf, ret - 1));
                                    buf = Strings.Mid(buf, ret + 1);
                                }

                                if (string.IsNullOrEmpty(sname))
                                {
                                    SRC.AddDataError(reader.InvalidData(@"スペシャルパワーの指定が抜けています。", data_name));
                                }
                                else if (!SRC.SPDList.IsDefined(sname))
                                {
                                    SRC.AddDataError(reader.InvalidData(@sname + "というスペシャルパワーは存在しません。", data_name));
                                }
                                else if (!Information.IsNumeric(buf2))
                                {
                                    SRC.AddDataError(reader.InvalidData(@"スペシャルパワー「" + sname + "」の獲得レベルが間違っています。", data_name));
                                    pd.AddSpecialPower(sname, 1, sp_cost);
                                }
                                else
                                {
                                    pd.AddSpecialPower(sname, Conversions.ToInteger(buf2), sp_cost);
                                }

                                ret = Strings.InStr(buf, ",");
                            }

                            if (!string.IsNullOrEmpty(buf))
                            {
                                SRC.AddDataError(reader.InvalidData(@"スペシャルパワー「" + Strings.Trim(sname) + "」の獲得レベル指定が抜けています。", data_name));
                            }

                            break;
                        }
                }

                // ビットマップ, ＭＩＤＩ
                line_buf = reader.GetLine();

                // ビットマップ
                if (Strings.Len(line_buf) == 0)
                {
                    throw reader.InvalidDataException(@"ビットマップの設定が抜けています。", data_name);
                }

                ret = Strings.InStr(line_buf, ",");
                if (ret == 0)
                {
                    throw reader.InvalidDataException(@"ＭＩＤＩの設定が抜けています。", data_name);
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, ret - 1));
                buf = Strings.Mid(line_buf, ret + 1);
                if (Strings.LCase(Strings.Right(buf2, 4)) == ".bmp")
                {
                    pd.Bitmap = buf2;
                }
                else
                {
                    SRC.AddDataError(reader.InvalidData(@"ビットマップの設定が間違っています。", data_name));
                    pd.IsBitmapMissing = true;
                }

                // ＭＩＤＩ
                buf = Strings.Trim(buf);
                buf2 = buf;
                while (Strings.Right(buf2, 1) == ")")
                    buf2 = Strings.Left(buf2, Strings.Len(buf2) - 1);
                switch (Strings.LCase(Strings.Right(buf2, 4)) ?? "")
                {
                    case ".mid":
                    case ".mp3":
                    case ".wav":
                    case "-":
                        {
                            pd.BGM = buf;
                            break;
                        }

                    case var case1 when case1 == "":
                        {
                            SRC.AddDataError(reader.InvalidData(@"ＭＩＤＩの設定が抜けています。", data_name));
                            pd.Bitmap = "-.mid";
                            break;
                        }

                    default:
                        {
                            SRC.AddDataError(reader.InvalidData(@"ＭＩＤＩの設定が間違っています。", data_name));
                            pd.Bitmap = "-.mid";
                            break;
                        }
                }

                if (reader.EOT)
                {
                    return pd;
                }

                line_buf = reader.GetLine();
                if (line_buf != "===")
                {
                    return pd;
                }

                // 特殊能力データ
                line_buf = UnitDataList.LoadFeature(data_name, pd, reader, SRC);


                if (line_buf != "===")
                {
                    return pd;
                }

                // 武器データ
                line_buf = UnitDataList.LoadWepon(data_name, pd, reader, SRC);

                if (line_buf != "===")
                {
                    return pd;
                }

                // アビリティデータ
                line_buf = UnitDataList.LoadAbility(data_name, pd, reader, SRC);

            }
            finally
            {
                if (pd != null)
                {
                    pd.Raw = reader.RawData;
                    reader.ClearRawData();
                }
            }
            return pd;
        }
    }
}
