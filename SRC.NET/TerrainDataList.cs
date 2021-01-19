using System;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    internal class TerrainDataList
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // 全地形データを管理するリストのクラス

        // 地形データの登録数
        public short Count;

        // 地形データの配列
        // 他のリスト管理用クラスと異なり配列を使っているのはアクセスを高速化するため
        // UPGRADE_NOTE: TerrainDataList は TerrainDataList_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private TerrainData[] TerrainDataList_Renamed = new TerrainData[(Map.MAX_TERRAIN_DATA_NUM + 1)];

        // 地形データの登録順を記録するための配列
        // UPGRADE_WARNING: 配列 OrderList の下限が 1 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
        private short[] OrderList = new short[(Map.MAX_TERRAIN_DATA_NUM + 1)];


        // クラスの初期化
        // UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Initialize_Renamed()
        {
            short i;
            for (i = 0; i <= Map.MAX_TERRAIN_DATA_NUM; i++)
                TerrainDataList_Renamed[i] = new TerrainData();
        }

        public TerrainDataList() : base()
        {
            Class_Initialize_Renamed();
        }

        // クラスの解放
        // UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        private void Class_Terminate_Renamed()
        {
            short i;
            for (i = 0; i <= Map.MAX_TERRAIN_DATA_NUM; i++)
                // UPGRADE_NOTE: オブジェクト TerrainDataList_Renamed() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
                TerrainDataList_Renamed[i] = null;
        }

        ~TerrainDataList()
        {
            Class_Terminate_Renamed();
        }


        // 指定したデータは登録されているか？
        public bool IsDefined(short ID)
        {
            bool IsDefinedRet = default;
            if (TerrainDataList_Renamed[ID].ID >= 0)
            {
                IsDefinedRet = true;
            }
            else
            {
                IsDefinedRet = false;
            }

            return IsDefinedRet;
        }


        // 地形データリストから指定したデータを取り出す
        public TerrainData Item(short ID)
        {
            TerrainData ItemRet = default;
            ItemRet = TerrainDataList_Renamed[ID];
            return ItemRet;
        }

        // 指定したデータの名称
        public string Name(short ID)
        {
            string NameRet = default;
            NameRet = TerrainDataList_Renamed[ID].Name;
            return NameRet;
        }

        // 指定したデータの画像ファイル名
        public string Bitmap(short ID)
        {
            string BitmapRet = default;
            BitmapRet = TerrainDataList_Renamed[ID].Bitmap_Renamed;
            return BitmapRet;
        }

        // 指定したデータのクラス
        // UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
        public string Class_Renamed(short ID)
        {
            string Class_RenamedRet = default;
            Class_RenamedRet = TerrainDataList_Renamed[ID].Class_Renamed;
            return Class_RenamedRet;
        }

        // 指定したデータの移動コスト
        public short MoveCost(short ID)
        {
            short MoveCostRet = default;
            MoveCostRet = TerrainDataList_Renamed[ID].MoveCost;
            return MoveCostRet;
        }

        // 指定したデータの命中修正
        public short HitMod(short ID)
        {
            short HitModRet = default;
            HitModRet = TerrainDataList_Renamed[ID].HitMod;
            return HitModRet;
        }

        // 指定したデータのダメージ修正
        public short DamageMod(short ID)
        {
            short DamageModRet = default;
            DamageModRet = TerrainDataList_Renamed[ID].DamageMod;
            return DamageModRet;
        }


        // 指定したデータの特殊能力

        public bool IsFeatureAvailable(short ID, ref string ftype)
        {
            bool IsFeatureAvailableRet = default;
            IsFeatureAvailableRet = TerrainDataList_Renamed[ID].IsFeatureAvailable(ref ftype);
            return IsFeatureAvailableRet;
        }

        public double FeatureLevel(short ID, ref string ftype)
        {
            double FeatureLevelRet = default;
            object argIndex1 = ftype;
            FeatureLevelRet = TerrainDataList_Renamed[ID].FeatureLevel(ref argIndex1);
            return FeatureLevelRet;
        }

        public string FeatureData(short ID, ref string ftype)
        {
            string FeatureDataRet = default;
            object argIndex1 = ftype;
            FeatureDataRet = TerrainDataList_Renamed[ID].FeatureData(ref argIndex1);
            return FeatureDataRet;
        }


        // Ｎ番目に登録したデータの番号
        public short OrderedID(short n)
        {
            short OrderedIDRet = default;
            OrderedIDRet = OrderList[n];
            return OrderedIDRet;
        }


        // データファイル fname からデータをロード
        public void Load(ref string fname)
        {
            short FileNumber;
            short ret;
            int line_num;
            short i, j;
            string buf, line_buf = default, buf2;
            TerrainData td;
            short data_id;
            string data_name;
            string err_msg;
            bool in_quote;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 4591


            Input:

                    On Error GoTo ErrorHandler

             */
            FileNumber = (short)FileSystem.FreeFile();
            FileSystem.FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read);
            line_num = 0;
            while (true)
            {
                data_name = "";
                do
                {
                    if (FileSystem.EOF((int)FileNumber))
                    {
                        FileSystem.FileClose((int)FileNumber);
                        return;
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }
                while (Strings.Len(line_buf) == 0);

                // 番号
                if (Information.IsNumeric(line_buf))
                {
                    data_id = Conversions.ToShort(line_buf);
                }
                else
                {
                    err_msg = "番号の設定が間違っています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5392


                    Input:
                                    Error(0)

                     */
                }

                if ((int)data_id < 0 | data_id >= Map.MAX_TERRAIN_DATA_NUM)
                {
                    err_msg = "番号の設定が間違っています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 5528


                    Input:
                                    Error(0)

                     */
                }

                td = TerrainDataList_Renamed[(int)data_id];
                // 新規登録？
                if ((int)td.ID < 0)
                {
                    Count = (short)((int)Count + 1);
                    OrderList[(int)Count] = data_id;
                }
                else
                {
                    td.Clear();
                }

                td.ID = data_id;

                // 名称, 画像ファイル名
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

                // 名称
                ret = (short)Strings.InStr(line_buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "画像ファイル名が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6014


                    Input:
                                        Error(0)

                     */
                }

                data_name = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                td.Name = data_name;
                buf = Strings.Mid(line_buf, (int)ret + 1);

                // 画像ファイル名
                td.Bitmap_Renamed = Strings.Trim(buf);
                if (Strings.Len(td.Bitmap_Renamed) == 0)
                {
                    err_msg = "画像ファイル名が指定されていません。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6460


                    Input:
                                        Error(0)

                     */
                }

                // 地形タイプ, 移動コスト, 命中修正, ダメージ修正
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);

                // 地形タイプ
                ret = (short)Strings.InStr(line_buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "移動コストが抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 6738


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(line_buf, (int)ret - 1));
                buf = Strings.Mid(line_buf, (int)ret + 1);
                td.Class_Renamed = buf2;

                // 移動コスト
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "命中修正が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 7115


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (buf2 == "-")
                {
                    td.MoveCost = (short)1000;
                }
                else if (Information.IsNumeric(buf2))
                {
                    // 0.5刻みの移動コストを使えるようにするため、実際の２倍の値で記録する
                    td.MoveCost = (short)(2d * Conversions.ToDouble(buf2));
                }

                if ((int)td.MoveCost <= 0)
                {
                    string argmsg = "移動コストの設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 命中修正
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret == 0)
                {
                    err_msg = "ダメージ修正が抜けています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 7816


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                buf = Strings.Mid(buf, (int)ret + 1);
                if (Information.IsNumeric(buf2))
                {
                    td.HitMod = Conversions.ToShort(buf2);
                }
                else
                {
                    string argmsg1 = "命中修正の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg1, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // ダメージ修正
                ret = (short)Strings.InStr(buf, ",");
                if ((int)ret > 0)
                {
                    err_msg = "余分な「,」が指定されています。";
                    ;
#error Cannot convert ErrorStatementSyntax - see comment for details
                    /* Cannot convert ErrorStatementSyntax, CONVERSION ERROR: Conversion for ErrorStatement not implemented, please report this issue in 'Error(0)' at character 8383


                    Input:
                                        Error(0)

                     */
                }

                buf2 = Strings.Trim(buf);
                if (Information.IsNumeric(buf2))
                {
                    td.DamageMod = Conversions.ToShort(buf2);
                }
                else
                {
                    string argmsg2 = "ダメージ修正の設定が間違っています。";
                    GUI.DataErrorMessage(ref argmsg2, ref fname, (short)line_num, ref line_buf, ref data_name);
                }

                // 地形効果
                GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                while (Strings.Len(line_buf) > 0)
                {
                    buf = line_buf;
                    i = (short)0;
                    while (Strings.Len(buf) > 0)
                    {
                        i = (short)((int)i + 1);
                        ret = (short)0;
                        in_quote = false;
                        var loopTo = (short)Strings.Len(buf);
                        for (j = (short)1; j <= loopTo; j++)
                        {
                            switch (Strings.Mid(buf, (int)j, 1) ?? "")
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

                        if ((int)ret > 0)
                        {
                            buf2 = Strings.Trim(Strings.Left(buf, (int)ret - 1));
                            buf = Strings.Trim(Strings.Mid(buf, (int)ret + 1));
                        }
                        else
                        {
                            buf2 = buf;
                            buf = "";
                        }

                        if (!string.IsNullOrEmpty(buf2))
                        {
                            td.AddFeature(ref buf2);
                        }
                        else
                        {
                            string argmsg3 = "行頭から" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format((object)i) + "番目の地形効果の設定が間違っています。";
                            GUI.DataErrorMessage(ref argmsg3, ref fname, (short)line_num, ref line_buf, ref data_name);
                        }
                    }

                    if (FileSystem.EOF((int)FileNumber))
                    {
                        FileSystem.FileClose((int)FileNumber);
                        return;
                    }

                    GeneralLib.GetLine(ref FileNumber, ref line_buf, ref line_num);
                }
            }

            ErrorHandler:
            ;

            // エラー処理
            if (line_num == 0)
            {
                string argmsg4 = fname + "が開けません。";
                GUI.ErrorMessage(ref argmsg4);
            }
            else
            {
                FileSystem.FileClose((int)FileNumber);
                GUI.DataErrorMessage(ref err_msg, ref fname, (short)line_num, ref line_buf, ref data_name);
            }

            Environment.Exit(0);
        }

        // リストをクリア
        public void Clear()
        {
            short i;
            for (i = 0; i <= Map.MAX_TERRAIN_DATA_NUM - 1; i++)
                TerrainDataList_Renamed[i].Clear();
            Count = 0;
        }
    }
}