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
    // 全パイロットデータを管理するリストのクラス
    public class PilotDataList
    {
        // パイロットデータのコレクション
        private SrcCollection<PilotData> colPilotDataList;

        public PilotDataList()
        {
            colPilotDataList = new SrcCollection<PilotData>();
            AddDummyData();
        }
        private void AddDummyData()
        {
            var pd = new PilotData();

            // ユニットステータスコマンドの無人ユニット用
            pd.Name = "ステータス表示用ダミーパイロット(ザコ)";
            pd.Nickname = "パイロット不在";
            pd.Adaption = "AAAA";
            pd.Bitmap = ".bmp";
            colPilotDataList.Add(pd, pd.Name);
        }

        // パイロットデータリストにデータを追加
        public PilotData Add(string pname)
        {
            PilotData AddRet = default;
            var new_pilot_data = new PilotData();
            new_pilot_data.Name = pname;
            colPilotDataList.Add(new_pilot_data, pname);
            AddRet = new_pilot_data;
            return AddRet;
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
            try
            {
                return colPilotDataList[Index] != null;
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
            using (var reader = new SrcReader(fname, stream))
            {
                while (reader.HasMore)
                {
                    PilotData pd = LoadPilot(reader); ;
                }
            }
        }

        public PilotData LoadPilot(SrcReader reader)
        {
            PilotData pd;
            var continuesErrors = new List<InvalidSrcData>();
            string buf, line_buf, buf2;
            string data_name;

            while (true)
            {
                data_name = "";
                line_buf = reader.GetLine();
                while (Strings.Len(line_buf) == 0) ;

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

                if (Strings.InStr(data_name, "（") > 0 | Strings.InStr(data_name, "）") > 0)
                {
                    throw reader.InvalidDataException(@"名称に全角括弧は使用出来ません", data_name);
                }

                if (Strings.InStr(data_name, "\"") > 0)
                {
                    throw reader.InvalidDataException("名称に\"は使用出来ません。", data_name);
                }

                if (IsDefined(data_name))
                {
                    // すでに定義済みのパイロットの場合はデータを置き換え
                    PilotData localItem() { object argIndex1 = (object)data_name; var ret = Item(argIndex1); return ret; }

                    if ((localItem().Name ?? "") == (data_name ?? ""))
                    {
                        pd = Item(data_name);
                        pd.Clear();
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
                                        string argstr_Renamed = pd.Nickname;
                                        pd.KanaName = GeneralLib.StrToHiragana(argstr_Renamed);
                                        pd.Nickname = argstr_Renamed;
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
                                        string argmsg = "読み仮名の設定が抜けています。";
                                        continuesErrors.Add(reader.InvalidData(@argmsg, data_name));
                                        string argstr_Renamed1 = pd.Nickname;
                                        pd.KanaName = GeneralLib.StrToHiragana(argstr_Renamed1);
                                        pd.Nickname = argstr_Renamed1;
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
                                        string argmsg1 = "性別の設定が間違っています。";
                                        continuesErrors.Add(reader.InvalidData(@argmsg1, data_name));
                                        break;
                                    }
                            }

                            break;
                        }

                    default:
                        {
                            string argstr_Renamed2 = pd.Nickname;
                            pd.KanaName = GeneralLib.StrToHiragana(argstr_Renamed2);
                            pd.Nickname = argstr_Renamed2;
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
                    string argmsg2 = "クラスの設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg2, data_name));
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
                    string argmsg3 = "地形適応の設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg3, data_name));
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
                    string argmsg4 = "経験値の設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg4, data_name));
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
                    i = 0;
                    aname = "";
                    while (true)
                    {
                        i = (i + 1);

                        // コンマの位置を検索
                        ret = Strings.InStr(buf, ",");
                        // 「"」が使われているか検索
                        ret2 = Strings.InStr(buf, "\"");
                        if (ret2 < ret & ret2 > 0)
                        {
                            // 「"」が見つかった場合、次の「"」後のコンマを検索
                            in_quote = true;
                            j = (ret2 + 1);
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
                                    string argmsg5 = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)((i + 1) / 2)) + "番目の特殊能力名の設定が間違っています。";
                                    continuesErrors.Add(reader.InvalidData(@argmsg5, data_name));
                                }
                            }

                            if (Strings.InStr(buf2, " ") > 0)
                            {
                                if (Strings.Left(buf2, 4) != "先手必勝" & Strings.Left(buf2, 6) != "ＳＰ消費減少" & Strings.Left(buf2, 12) != "スペシャルパワー自動発動" & Strings.Left(buf2, 4) != "ハンター" & Strings.InStr(buf2, "=解説 ") == 0)
                                {
                                    if (string.IsNullOrEmpty(aname))
                                    {
                                        throw reader.InvalidDataException(@"行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)((i + 1) / 2)) + "番目の特殊能力「" + Strings.Trim(Strings.Left(buf2, Strings.InStr(buf2, " "))) + "」の指定の後に「,」が抜けています。", data_name);
                                    }
                                    else
                                    {
                                        throw reader.InvalidDataException(@"特殊能力「" + aname + "」のレベル指定の後に「,」が抜けています。", data_name);
                                    };
                                }
                            }

                            // 特殊能力の別名指定がある？
                            j = Strings.InStr(buf2, "=");
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
                                        alevel = (double)SRC.DEFAULT_LEVEL;
                                        break;
                                    }

                                case 1:
                                    {
                                        // レベル指定のみあり
                                        if (!Information.IsNumeric(Strings.Mid(buf2, j + 2)))
                                        {
                                            string argmsg6 = "特殊能力「" + aname + "」のレベル指定が不正です。";
                                            continuesErrors.Add(reader.InvalidData(@argmsg6, data_name));
                                        }

                                        alevel = (double)Conversions.Toint(Strings.Mid(buf2, j + 2));
                                        if (string.IsNullOrEmpty(aname))
                                        {
                                            string argmsg7 = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)((i + 1) / 2)) + "番目の特殊能力名の設定が間違っています。";
                                            continuesErrors.Add(reader.InvalidData(@argmsg7, data_name));
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
                            pd.AddSkill(aname, alevel, adata, Conversions.Toint(buf2));
                        }
                        else
                        {
                            if (alevel > 0d)
                            {
                                string argmsg8 = "特殊能力「" + aname + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)alevel) + "」の修得レベルが間違っています。";
                                continuesErrors.Add(reader.InvalidData(@argmsg8, data_name));
                            }
                            else
                            {
                                string argmsg9 = "特殊能力「" + aname + "」の修得レベルが間違っています。";
                                continuesErrors.Add(reader.InvalidData(@argmsg9, data_name));
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
                                    string argmsg10 = "特殊能力「" + aname + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)alevel) + "」の修得レベルが間違っています。";
                                    continuesErrors.Add(reader.InvalidData(@argmsg10, data_name));
                                }
                                else
                                {
                                    string argmsg11 = "特殊能力「" + aname + "」の修得レベルが間違っています。";
                                    continuesErrors.Add(reader.InvalidData(@argmsg11, data_name));
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
                    i = 0;
                    aname = "";
                    do
                    {
                        i = (i + 1);

                        // コンマの位置を検索
                        ret = Strings.InStr(buf, ",");
                        // 「"」が使われているか検索
                        ret2 = Strings.InStr(buf, "\"");
                        if (ret2 < ret & ret2 > 0)
                        {
                            // 「"」が見つかった場合、次の「"」後のコンマを検索
                            in_quote = true;
                            j = (ret2 + 1);
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
                                    throw reader.InvalidDataException(@"行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)((i + 1) / 2)) + "番目の特殊能力の指定の後に「,」が抜けています。", data_name);
                                }
                                else
                                {
                                    throw reader.InvalidDataException(@"特殊能力「" + aname + "」のレベル指定の後に「,」が抜けています。", data_name);
                                };
                            }

                            // 特殊能力の別名指定がある？
                            j = Strings.InStr(buf2, "=");
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
                                        alevel = (double)SRC.DEFAULT_LEVEL;
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
                                            throw reader.InvalidDataException(@"行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)((i + 1) / 2)) + "番目の特殊能力の名前「" + buf2 + "」が不正です", data_name);
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
                            pd.AddSkill(aname, alevel, adata, Conversions.Toint(buf2));
                        }
                        else
                        {
                            if (alevel > 0d)
                            {
                                string argmsg12 = "特殊能力「" + aname + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)alevel) + "」の修得レベルが間違っています。";
                                continuesErrors.Add(reader.InvalidData(@argmsg12, data_name));
                            }
                            else
                            {
                                string argmsg13 = "特殊能力「" + aname + "」の修得レベルが間違っています。";
                                continuesErrors.Add(reader.InvalidData(@argmsg13, data_name));
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
                            string argmsg14 = "特殊能力「" + aname + "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)alevel) + "」の修得レベルが間違っています。";
                            continuesErrors.Add(reader.InvalidData(@argmsg14, data_name));
                        }
                        else
                        {
                            string argmsg15 = "特殊能力「" + aname + "」の修得レベルが間違っています。";
                            continuesErrors.Add(reader.InvalidData(@argmsg15, data_name));
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
                    string argmsg16 = "格闘攻撃力の設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg16, data_name));
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
                    string argmsg17 = "射撃攻撃力の設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg17, data_name));
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
                    string argmsg18 = "命中の設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg18, data_name));
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
                    string argmsg19 = "回避の設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg19, data_name));
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
                    string argmsg20 = "技量の設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg20, data_name));
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
                    string argmsg21 = "反応の設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg21, data_name));
                }

                // 性格
                buf2 = Strings.Trim(buf);
                if (Strings.Len(buf2) == 0)
                {
                    throw reader.InvalidDataException(@"性格の設定が抜けています。", data_name);
                }

                if (Strings.InStr(buf2, ",") > 0)
                {
                    string argmsg22 = "行末に余分なコンマが付けられています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg22, data_name));
                    buf2 = Strings.Trim(Strings.Left(buf2, Strings.InStr(buf2, ",") - 1));
                }

                if (!Information.IsNumeric(buf2))
                {
                    pd.Personality = buf2;
                }
                else
                {
                    string argmsg23 = "性格の設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg23, data_name));
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
                            if (buf2 != "ＳＰ" & buf2 != "精神")
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
                                string argmsg24 = "ＳＰの設定が間違っています。";
                                continuesErrors.Add(reader.InvalidData(@argmsg24, data_name));
                                pd.SP = 1;
                            }

                            // スペシャルパワーと獲得レベル
                            ret = Strings.InStr(buf, ",");
                            while (ret > 0)
                            {
                                sname = Strings.Trim(Strings.Left(buf, ret - 1));
                                buf = Strings.Mid(buf, ret + 1);

                                // ＳＰ消費量
                                if (Strings.InStr(sname, "=") > 0)
                                {
                                    string argexpr = Strings.Mid(sname, Strings.InStr(sname, "=") + 1);
                                    sp_cost = GeneralLib.StrToLng(argexpr);
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

                                bool localIsDefined() { object argIndex1 = (object)sname; var ret = SRC.SPDList.IsDefined(argIndex1); return ret; }

                                if (string.IsNullOrEmpty(sname))
                                {
                                    string argmsg25 = "スペシャルパワーの指定が抜けています。";
                                    continuesErrors.Add(reader.InvalidData(@argmsg25, data_name));
                                }
                                else if (!localIsDefined())
                                {
                                    string argmsg26 = sname + "というスペシャルパワーは存在しません。";
                                    continuesErrors.Add(reader.InvalidData(@argmsg26, data_name));
                                }
                                else if (!Information.IsNumeric(buf2))
                                {
                                    string argmsg27 = "スペシャルパワー「" + sname + "」の獲得レベルが間違っています。";
                                    continuesErrors.Add(reader.InvalidData(@argmsg27, data_name));
                                    pd.AddSpecialPower(sname, 1, sp_cost);
                                }
                                else
                                {
                                    pd.AddSpecialPower(sname, Conversions.Toint(buf2), sp_cost);
                                }

                                ret = Strings.InStr(buf, ",");
                            }

                            if (!string.IsNullOrEmpty(buf))
                            {
                                string argmsg28 = "スペシャルパワー「" + Strings.Trim(sname) + "」の獲得レベル指定が抜けています。";
                                continuesErrors.Add(reader.InvalidData(@argmsg28, data_name));
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
                    string argmsg29 = "ビットマップの設定が間違っています。";
                    continuesErrors.Add(reader.InvalidData(@argmsg29, data_name));
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
                            string argmsg30 = "ＭＩＤＩの設定が抜けています。";
                            continuesErrors.Add(reader.InvalidData(@argmsg30, data_name));
                            pd.Bitmap = "-.mid";
                            break;
                        }

                    default:
                        {
                            string argmsg31 = "ＭＩＤＩの設定が間違っています。";
                            continuesErrors.Add(reader.InvalidData(@argmsg31, data_name));
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
                UnitDataList.AddFeature(data_name, pd, reader, continuesErrors);
                line_buf = reader.GetLine();
                buf = line_buf;
                i = 0;
                while (line_buf != "===")
                {
                    i = (i + 1);
                    ret = 0;
                    in_quote = false;
                    var loopTo1 = Strings.Len(buf);
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
                        buf = Strings.Trim(Strings.Mid(buf, ret + 1));
                    }
                    else
                    {
                        buf2 = buf;
                        buf = "";
                    }

                    if (string.IsNullOrEmpty(buf2) | Information.IsNumeric(buf2))
                    {
                        string argmsg32 = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i) + "番目の特殊能力の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg32, data_name));
                    }
                    else
                    {
                        pd.AddFeature(buf2);
                    }

                    if (string.IsNullOrEmpty(buf))
                    {
                        if (FileSystem.EOF(FileNumber))
                        {
                            FileSystem.FileClose(FileNumber);
                            return;
                        }

                        line_buf = reader.GetLine();
                        buf = line_buf;
                        i = 0;
                        if (string.IsNullOrEmpty(line_buf) | line_buf == "===")
                        {
                            break;
                        }
                    }
                }

                if (line_buf != "===")
                {
                    goto SkipRest;
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

                    wname = Strings.Trim(Strings.Left(line_buf, ret - 1));
                    buf = Strings.Mid(line_buf, ret + 1);
                    if (string.IsNullOrEmpty(wname))
                    {
                        throw reader.InvalidDataException(@"武器名の設定が間違っています。", data_name);
                    }

                    // 武器を登録
                    wd = pd.AddWeapon(wname);

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
                        string argmsg33 = wname + "の攻撃力の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg33, data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr1 = GeneralLib.LIndex(buf2, 1);
                            wd.Power = GeneralLib.StrToLng(argexpr1);
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
                        wd.MinRange = Conversions.Toint(buf2);
                    }
                    else
                    {
                        string argmsg34 = wname + "の最小射程の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg34, data_name));
                        wd.MinRange = 1;
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr2 = GeneralLib.LIndex(buf2, 1);
                            wd.MinRange = GeneralLib.StrToLng(argexpr2);
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
                        string argmsg35 = wname + "の最大射程の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg35, data_name));
                        wd.MaxRange = 1;
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr3 = GeneralLib.LIndex(buf2, 1);
                            wd.MaxRange = GeneralLib.StrToLng(argexpr3);
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
                        string argmsg36 = wname + "の命中率の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg36, data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr4 = GeneralLib.LIndex(buf2, 1);
                            wd.Precision = GeneralLib.StrToLng(argexpr4);
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
                            string argmsg37 = wname + "の弾数の設定が間違っています。";
                            continuesErrors.Add(reader.InvalidData(@argmsg37, data_name));
                            if (GeneralLib.LLength(buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                                string argexpr5 = GeneralLib.LIndex(buf2, 1);
                                wd.Bullet = GeneralLib.StrToLng(argexpr5);
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
                            string argmsg38 = wname + "の消費ＥＮの設定が間違っています。";
                            continuesErrors.Add(reader.InvalidData(@argmsg38, data_name));
                            if (GeneralLib.LLength(buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                                string argexpr6 = GeneralLib.LIndex(buf2, 1);
                                wd.ENConsumption = GeneralLib.StrToLng(argexpr6);
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
                            string argmsg39 = wname + "の必要気力の設定が間違っています。";
                            continuesErrors.Add(reader.InvalidData(@argmsg39, data_name));
                            if (GeneralLib.LLength(buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                                string argexpr7 = GeneralLib.LIndex(buf2, 1);
                                wd.NecessaryMorale = GeneralLib.StrToLng(argexpr7);
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
                        string argmsg40 = wname + "の地形適応の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg40, data_name));
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
                        string argmsg41 = wname + "のクリティカル率の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg41, data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr8 = GeneralLib.LIndex(buf2, 1);
                            wd.Critical = GeneralLib.StrToLng(argexpr8);
                        }
                    }

                    // 武器属性
                    buf = Strings.Trim(buf);
                    if (Strings.Len(buf) == 0)
                    {
                        string argmsg42 = wname + "の武器属性の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg42, data_name));
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
                        string argmsg43 = wname + "の属性のレベル指定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg43, data_name));
                    }

                    if (FileSystem.EOF(FileNumber))
                    {
                        FileSystem.FileClose(FileNumber);
                        return;
                    }

                    line_buf = reader.GetLine();
                }

                if (line_buf != "===")
                {
                    goto SkipRest;
                }

                // アビリティデータ
                line_buf = reader.GetLine();
                while (Strings.Len(line_buf) > 0)
                {
                    // アビリティ名
                    ret = Strings.InStr(line_buf, ",");
                    if (ret == 0)
                    {
                        throw reader.InvalidDataException(@"アビリティデータの終りには空行を置いてください。", data_name);
                    }

                    sname = Strings.Trim(Strings.Left(line_buf, ret - 1));
                    buf = Strings.Mid(line_buf, ret + 1);
                    if (string.IsNullOrEmpty(sname))
                    {
                        throw reader.InvalidDataException(@"アビリティ名の設定が間違っています。", data_name);
                    }

                    // アビリティを登録
                    sd = pd.AddAbility(sname);

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
                        ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                        /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 44969


                        Input:
                                                Error(0)

                         */
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
                        string argmsg44 = sname + "の射程の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg44, data_name));
                        if (GeneralLib.LLength(buf2) > 1)
                        {
                            buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                            string argexpr9 = GeneralLib.LIndex(buf2, 1);
                            sd.MaxRange = GeneralLib.StrToLng(argexpr9);
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
                            string argmsg45 = sname + "の回数の設定が間違っています。";
                            continuesErrors.Add(reader.InvalidData(@argmsg45, data_name));
                            if (GeneralLib.LLength(buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                                string argexpr10 = GeneralLib.LIndex(buf2, 1);
                                sd.Stock = GeneralLib.StrToLng(argexpr10);
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
                            string argmsg46 = sname + "の消費ＥＮの設定が間違っています。";
                            continuesErrors.Add(reader.InvalidData(@argmsg46, data_name));
                            if (GeneralLib.LLength(buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                                string argexpr11 = GeneralLib.LIndex(buf2, 1);
                                sd.ENConsumption = GeneralLib.StrToLng(argexpr11);
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
                            string argmsg47 = sname + "の必要気力の設定が間違っています。";
                            continuesErrors.Add(reader.InvalidData(@argmsg47, data_name));
                            if (GeneralLib.LLength(buf2) > 1)
                            {
                                buf = GeneralLib.LIndex(buf2, 2) + "," + buf;
                                string argexpr12 = GeneralLib.LIndex(buf2, 1);
                                sd.NecessaryMorale = GeneralLib.StrToLng(argexpr12);
                            }
                        }
                    }

                    // アビリティ属性
                    buf = Strings.Trim(buf);
                    if (Strings.Len(buf) == 0)
                    {
                        string argmsg48 = sname + "のアビリティ属性の設定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg48, data_name));
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
                        string argmsg49 = sname + "の属性のレベル指定が間違っています。";
                        continuesErrors.Add(reader.InvalidData(@argmsg49, data_name));
                    }

                    if (FileSystem.EOF(FileNumber))
                    {
                        FileSystem.FileClose(FileNumber);
                        return;
                    }

                    line_buf = reader.GetLine();
                }
            }
            return pd;
        }
    }
}