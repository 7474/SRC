// Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
// 本プログラムはフリーソフトであり、無保証です。
// 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
// 再頒布または改変することができます。
using Newtonsoft.Json.Linq;
using SRCCore.Exceptions;
using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.Units;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SRCCore
{
    /// <summary>
    /// 元の SRC（VB5）のセーブデータの読み込み時に、元の SRC がメッセージを出してから End で終わるところ（データが定義されていないなど）
    /// </summary>
    public class SaveDataTerminateException : TerminateException
    {
        public SaveDataTerminateException(string message) : base(message)
        {
        }
    }

    // 元の SRC（VB5）のセーブデータ（クリアデータ・中断データ）を読む（#1577）。
    // 形式と版による読み替えは docs/history/porting/vb-savedata.md。
    // 手続きは SRC.bas の LoadData・RestoreData と同じ順に行い、読んだ後の処理（RestoreData の背景・ＢＧＭ・乱数系列など）は
    // JSON から読んだときと共通（SRC.save.cs）
    public partial class SRC
    {
        /// <summary>
        /// セーブデータ（拡張子 .src）がクリアデータか。元の SRC の Main と同じく、最初の値が数値で 10000 より大きければ次の値も読み、
        /// その値が数値ならクリアデータ、数値でなければ中断データとする。SRC# の JSON なら Kind で決める
        /// </summary>
        public static bool IsClearSaveData(byte[] bytes)
        {
            if (!VbSaveDataReader.IsVbSaveData(bytes))
            {
                var kind = JObject.Parse(ReadJson(bytes))["Kind"];
                return kind == null || kind.ToObject<SRCSaveKind>() == SRCSaveKind.Normal;
            }

            var reader = VbSaveDataReader.FromBytes(bytes);
            // 第１項目を読み込み
            var buf = reader.EOF ? "" : reader.Input();
            // 第１項目はセーブデータバージョン？
            if (Information.IsNumeric(buf) && Conversions.ToDouble(buf) > 10000d && !reader.EOF)
            {
                // バージョンデータであれば第２項目を読み込み
                buf = reader.Input();
            }
            return Information.IsNumeric(buf);
        }

        /// <summary>
        /// 旧形式のユニットＩＤを新形式に変換する（VB: SRC.bas の ConvertUnitID）。
        /// 旧形式）ユニット名称+数値、新形式）ユニット名称+":"+数値
        /// </summary>
        public static string ConvertUnitID(string id)
        {
            if (Strings.InStr(id, ":") > 0)
            {
                return id;
            }

            // 数値部分を読み飛ばす
            var i = Strings.Len(id);
            while (i > 0)
            {
                var c = id[i - 1];
                if (c < '0' || c > '9')
                {
                    break;
                }
                i = i - 1;
            }

            // ユニット名称と数値部分の間に「:」を挿入
            return Strings.Left(id, i) + ":" + Strings.Mid(id, i + 1);
        }

        /// <summary>
        /// 元の SRC のセーブデータにあるシナリオのファイル（イベント・マップ）の名前を、いまのシナリオのフォルダで解く。
        /// 元の SRC は「:」を含まなければシナリオのフォルダからの相対パスとし、含めば絶対パスとして読む。
        /// 絶対パスは、JSON の以前の版の絶対パスと同じく、いまのシナリオのフォルダにある同じ名前のファイルを探す（#1375）
        /// </summary>
        internal string ResolveVbSavedPath(string path)
        {
            if (Strings.InStr(path, ":") > 0 && !string.IsNullOrEmpty(ScenarioPath))
            {
                return FindInScenarioFolder(path) ?? path;
            }
            return ResolveSavedPath(path);
        }

        /// <summary>
        /// 読み込みを打ち切る（VB: ErrorMessage の後の TerminateSRC と End）
        /// </summary>
        internal SaveDataTerminateException TerminateLoading(string msg)
        {
            GUI.ErrorMessage(msg);
            return new SaveDataTerminateException(msg);
        }

        // データをロード（VB: SRC.bas の LoadData）
        private void LoadVbData(VbSaveDataReader reader)
        {
            int num;
            reader.SaveDataVersion = reader.InputInt();
            if (reader.SaveDataVersion > 10000)
            {
                num = reader.InputInt();
            }
            else
            {
                num = reader.SaveDataVersion;
            }

            GUI.SetLoadImageSize(num * 2 + 5);

            Titles = new List<string>();
            for (var i = 1; i <= num; i++)
            {
                var title = reader.Input();
                Titles.Add(title);
                IncludeData(title);
            }

            // ローカルデータの読みこみ
            LoadLocalData();

            var fname2 = reader.Input();
            TotalTurn = reader.InputInt();
            Money = reader.InputInt();
            // パーツ用のダミー
            reader.InputInt();

            Event.LoadGlobalVariables(reader);
            if (!Expression.IsGlobalVariableDefined("次ステージ"))
            {
                Expression.DefineGlobalVariable("次ステージ");
            }
            Expression.SetVariableAsString("次ステージ", fname2);

            PList.Load(reader);
            UList.Load(reader);
            IList.Load(reader);

            // リンクデータを処理するため、セーブファイルを一旦閉じてから再度読み込み
            reader = reader.Reopen();
            if (reader.SaveDataVersion > 10000)
            {
                reader.Input();
            }
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                reader.Input();
            }
            reader.Input();
            TotalTurn = reader.InputInt();
            Money = reader.InputInt();
            // パーツ用のダミー
            reader.InputInt();
            // 元の SRC はもう一度「パーツ用のダミー」として読み、それがグローバル変数の数になる
            num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                reader.LineInput();
            }
            PList.LoadLinkInfo(reader);
            UList.LoadLinkInfo(reader);
            IList.LoadLinkInfo(reader);

            GUI.DisplayLoadingProgress();

            // ユニットの状態を回復
            foreach (Unit u in UList.Items)
            {
                u.Reset();
            }
            GUI.DisplayLoadingProgress();

            // 追加されたシステム側イベントデータの読み込み
            Event.LoadEventData("", load_mode: "");
            GUI.DisplayLoadingProgress();
        }

        // 一時中断用データをロード（VB: SRC.bas の RestoreData のうち、PList.UpdateSupportMod の前まで）
        private RestoredSuspendData RestoreVbData(VbSaveDataReader reader, bool quick_load)
        {
            int num;
            var fname2 = reader.Input();
            if (Information.IsNumeric(fname2))
            {
                reader.SaveDataVersion = VBMath.CLng(Conversions.ToDouble(fname2));
                fname2 = reader.Input();
            }
            else
            {
                reader.SaveDataVersion = 1;
            }

            // ウィンドウのタイトルを設定
            var savedScenarioFileName = ResolveVbSavedPath(fname2);
            var scenario_file_is_different = !FileSystem.RelativePathEuqals(ScenarioPath, ScenarioFileName, savedScenarioFileName);
            if (scenario_file_is_different)
            {
                GUI.MainFormText = "SRC# - " + Path.GetFileNameWithoutExtension(PathResolver.ToNative(savedScenarioFileName));
            }
            ScenarioFileName = savedScenarioFileName;

            num = reader.InputInt();

            // 使用するデータをロード
            if (!quick_load)
            {
                GUI.SetLoadImageSize(num * 2 + 5);

                Titles = new List<string>();
                for (var i = 1; i <= num; i++)
                {
                    var title = reader.Input();
                    Titles.Add(title);
                    IncludeData(title);
                }

                // ローカルデータの読みこみ
                LoadLocalData();
                Event.LoadEventData(ScenarioFileName, "リストア");
                GUI.DisplayLoadingProgress();
            }
            else
            {
                for (var i = 1; i <= num; i++)
                {
                    reader.LineInput();
                }

                if (scenario_file_is_different)
                {
                    Event.LoadEventData(ScenarioFileName, "リストア");
                }
            }

            Turn = reader.InputInt();
            TotalTurn = reader.InputInt();
            Money = reader.InputInt();

            Event.RestoreEventData(reader);

            PList.Restore(reader);
            IList.Restore(reader);
            UList.Restore(reader);

            // マップデータの互換性維持のため、RestoreMapData でＢＧＭ関連の１行目まで読み込んで戻り値にした
            fname2 = RestoreVbMapData(reader);
            var restored = new RestoredSuspendData
            {
                BGMFile = Sound.SearchMidiFile("(" + fname2 + ")"),
            };
            if (!string.IsNullOrEmpty(restored.BGMFile))
            {
                restored.RepeatMode = reader.InputBool();
                restored.KeepBGM = reader.InputBool();
                restored.BossBGM = reader.InputBool();
            }
            else
            {
                reader.LineInput();
                reader.LineInput();
                reader.LineInput();
            }

            // 乱数系列（デバッグ・乱数系列非保存のオプションは、読んだ後の共通の処理で見る）
            if (!reader.EOF)
            {
                restored.HasRandomSeries = true;
                restored.RndSeed = reader.InputInt();
                restored.RndIndex = reader.InputInt();
            }

            // リンクデータを処理するため、セーブファイルを一旦閉じてから再度読み込み
            reader = SkipVbSuspendHeader(reader.Reopen());
            PList.RestoreLinkInfo(reader);
            IList.RestoreLinkInfo(reader);
            UList.RestoreLinkInfo(reader);

            // パラメータ情報を処理するため、セーブファイルを一旦閉じてから再度読み込み。
            // 霊力やＨＰ、ＥＮといったパラメータは最大値が特殊能力で変動するため、
            // 特殊能力の設定が終わってから改めて設定してやる必要がある。
            reader = SkipVbSuspendHeader(reader.Reopen());
            PList.RestoreParameter(reader);
            IList.RestoreParameter(reader);
            UList.RestoreParameter(reader);

            return restored;
        }

        // 中断データを開き直して、イベントデータの後（パイロットの前）まで読み飛ばす
        private static VbSaveDataReader SkipVbSuspendHeader(VbSaveDataReader reader)
        {
            // SaveDataVersion
            if (reader.SaveDataVersion > 10000)
            {
                reader.LineInput();
            }

            // ScenarioFileName
            reader.LineInput();

            // 使用するデータ名
            var num = reader.InputInt();
            for (var i = 1; i <= num; i++)
            {
                reader.LineInput();
            }

            // Turn
            reader.LineInput();
            // TotalTurn
            reader.LineInput();
            // Money
            reader.LineInput();

            Events.Event.SkipEventData(reader);
            return reader;
        }

        // 中断用セーブデータからマップデータをロードし、ＢＧＭ関連の１行目を返す（VB: Map.bas の RestoreMapData）。
        // マップを戻すのは JSON から読むときと同じ Map.Restore（マップが変わったかを比べて IsMapDirty を立て、ユニットを配置する）
        private string RestoreVbMapData(VbSaveDataReader reader)
        {
            // マップファイル名, マップ描画方法
            var sbuf1 = reader.Input();
            var sbuf2 = reader.Input();
            var map = new Map(this);
            map.MapFileName = string.IsNullOrEmpty(sbuf1) ? sbuf1 : ResolveVbSavedPath(sbuf1);
            if (Strings.Right(sbuf2, 7) == "(マップ限定)")
            {
                map.MapDrawMode = Strings.Left(sbuf2, Strings.Len(sbuf2) - 7);
                map.MapDrawIsMapOnly = true;
            }
            else
            {
                map.MapDrawMode = sbuf2;
                map.MapDrawIsMapOnly = false;
            }
            // フィルタの色と透過度は元の SRC が保存しないので、いまの値のまま
            map.MapDrawFilterColor = Map.MapDrawFilterColor;
            map.MapDrawFilterTransPercent = Map.MapDrawFilterTransPercent;

            // マップ幅, マップ高さ
            map.MapWidth = reader.InputInt();
            map.MapHeight = reader.InputInt();
            map.MapData = new Src2DArray<MapCell>(map.MapWidth, map.MapHeight);

            // 各地形
            for (var i = 1; i <= map.MapWidth; i++)
            {
                for (var j = 1; j <= map.MapHeight; j++)
                {
                    map.MapData[i, j] = new MapCell
                    {
                        X = i,
                        Y = j,
                        TerrainType = reader.InputInt(),
                        BitmapNo = reader.InputInt(),
                    };
                }
            }

            // 表示位置
            var mapX = reader.InputInt();
            var mapY = reader.InputInt();

            var buf = reader.Input();
            if (buf == "Layer")
            {
                // 各レイヤ
                for (var i = 1; i <= map.MapWidth; i++)
                {
                    for (var j = 1; j <= map.MapHeight; j++)
                    {
                        var cell = map.MapData[i, j];
                        cell.LayerType = reader.InputInt();
                        cell.LayerBitmapNo = reader.InputInt();
                        cell.BoxType = (BoxTypes)reader.InputInt();
                    }
                }
                // ＢＧＭ関連情報の1行目を読み込む
                buf = reader.Input();
            }

            Map.Restore(new SRCSuspendData { Map = map });
            // マップの表示位置は SetMapSize によるリセットの後で復元する
            GUI.MapX = mapX;
            GUI.MapY = mapY;
            return buf;
        }
    }
}
