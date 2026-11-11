using System;
using Newtonsoft.Json;
using SRCCore.Events;
using SRCCore.Expressions;
using SRCCore.Filesystem;
using SRCCore.Lib;
using SRCCore.Units;
using SRCCore.VB;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SRCCore
{
    public enum SRCSaveKind
    {
        Normal,
        Suspend,
        Quik,
        Restart,
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Porting", "SRCP003", Justification = "SRC# のセーブデータ（JSON）の形で、VB の変数ではない")]
    public class SRCSaveData
    {
        public string Version { get; set; }
        public SRCSaveKind Kind { get; set; }
        public IList<string> Titles { get; set; }
        public string NextStage { get; set; }
        public int TotalTurn { get; set; }
        public int Money { get; set; }

        public SrcCollection<VarData> GlobalVariableList;

        public Pilots.Pilots PList { get; set; }
        public Units.Units UList { get; set; }
        public Items.Items IList { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Porting", "SRCP003", Justification = "SRC# のセーブデータ（JSON）の形で、VB の変数ではない")]
    public class SRCSuspendData : SRCSaveData
    {
        public string ScenarioFileName { get; set; }
        public int Turn { get; set; }
        public SrcCollection<VarData> LocalVariableList { get; set; }
        // 古いセーブ（EventLabelEnabled を持たないもの）と以前の版の SRC# のために書く。読むのは EventLabelEnabled が無いときだけ
        public IList<string> DisableEventLabels { get; set; }
        // イベントラベルの有効・無効を colEventLabelList の順に持つ（VB: DumpEventData の lab.Enable、#1529）
        public IList<bool> EventLabelEnabled { get; set; }
        public IList<string> AdditionalEventFileNames { get; set; }
        public Maps.Map Map { get; set; }
        public int MapX { get; set; }
        public int MapY { get; set; }


        public string BGMFileName { get; set; }
        public bool RepeatMode { get; set; }
        public bool KeepBGM { get; set; }
        public bool BossBGM { get; set; }

        // 乱数系列の状態
        public int RndSeed { get; set; }
        public int RndIndex { get; set; }
    }

    public partial class SRC
    {
        // セーブデータは JSON で書き、元の SRC（SRC.bas の SaveData・LoadData・DumpData・RestoreData）の形式では書かない。
        // 読むときは、JSON のほかに元の SRC の形式も読む（SRC.save.vb.cs。#1577）。
        // 保存・読み込みの前後の処理（データの読み込み・イベントデータの読み込み・リンクとパラメータの復元・画面の更新）は
        // 元の SRC と同じ順に行う

        /// <summary>
        /// セーブデータに書く版。元の SRC は実行ファイル（App）の版を書く。
        /// 実行ファイルの版が取れないとき（ほかのホストから呼ぶとき）は SRCCore の版にする
        /// </summary>
        private static string SaveDataVersionString => (Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly()).GetName().Version.ToString();

        // データをセーブ
        public void SaveData(Stream stream)
        {
            try
            {
                var data = new SRCSaveData()
                {
                    Version = SaveDataVersionString,
                    Kind = SRCSaveKind.Normal,
                    Titles = Titles,
                    NextStage = Expression.GetValueAsString("次ステージ"),
                    TotalTurn = TotalTurn,
                    Money = Money,
                    GlobalVariableList = Event.GlobalVariableList,
                    PList = PList,
                    UList = UList,
                    IList = IList,
                };

                stream.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Formatting.Indented)));
            }
            catch (Exception ex)
            {
                LogError(ex);
                GUI.ErrorMessage("セーブ中にエラーが発生しました");
            }
        }

        // データをロード
        public void LoadData(Stream stream)
        {
            try
            {
                var bytes = ReadAllBytes(stream);
                if (VbSaveDataReader.IsVbSaveData(bytes))
                {
                    // 元の SRC の形式（#1577）
                    LoadVbData(VbSaveDataReader.FromBytes(bytes));
                    return;
                }

                // 元の SRC は版（SaveDataVersion）で古い形式を読み替える。SRC# の JSON はまだ版で読み方を変えないので、版は見ない
                var data = JsonConvert.DeserializeObject<SRCSaveData>(ReadJson(bytes));
                GUI.SetLoadImageSize((data.Titles.Count * 2 + 5));
                Titles = data.Titles;

                foreach (var title in Titles)
                {
                    IncludeData(title);
                }
                // ローカルデータの読みこみ
                LoadLocalData();

                TotalTurn = data.TotalTurn;
                Money = data.Money;
                // グローバル変数を読み込んでから次ステージを設定する（元の SRC の LoadGlobalVariables の後）
                Event.GlobalVariableList = data.GlobalVariableList;
                if (!Expression.IsGlobalVariableDefined("次ステージ"))
                {
                    Expression.DefineGlobalVariable("次ステージ");
                }
                Expression.SetVariableAsString("次ステージ", data.NextStage);

                PList = data.PList;
                UList = data.UList;
                IList = data.IList;
                PList.Restore(this);
                UList.Restore(this);
                IList.Restore(this);

                PList.Update();
                UList.Update();
                IList.Update();

                // 元の SRC はここでセーブファイルを読み直してリンクデータ（パイロットとユニット、ユニットと他形態・パイロット・アイテム）を
                // 戻し（PList・UList・IList の LoadLinkInfo）、全ユニットを Update する。
                // SRC# はつながりを JSON（UnitId など）で持ち、上の Restore・Update で戻している
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
            catch (SaveDataTerminateException ex)
            {
                // 元の SRC はメッセージを出してから End で終わる（「ロード中にエラーが発生しました」は出さない）
                LogError(ex);
                TerminateSRC();
            }
            catch (Exception ex)
            {
                LogError(ex);
                GUI.ErrorMessage("ロード中にエラーが発生しました");
                TerminateSRC();
            }
        }

        public void DumpData(string fname, SRCSaveKind saveKind)
        {
            // 元の SRC は書き終えてから LastSaveDataFileName を設定する。
            // ストリームに書く DumpData はファイル名を知らないので、ここで設定する
            LastSaveDataFileName = fname;
            using (var stream = FileSystem.Create(fname))
            {
                DumpData(stream, saveKind);
            }
            IsLastSaveDataQuikSaveStream = false;
        }

        // 一時中断用データをファイルにセーブする
        public void DumpData(Stream stream, SRCSaveKind saveKind)
        {
            try
            {
                var data = new SRCSuspendData()
                {
                    Version = SaveDataVersionString,
                    Kind = saveKind,
                    Titles = Titles,
                    NextStage = Expression.GetValueAsString("次ステージ"),
                    TotalTurn = TotalTurn,
                    Money = Money,
                    GlobalVariableList = Event.GlobalVariableList,
                    PList = PList,
                    UList = UList,
                    IList = IList,
                    // QuikSave
                    // シナリオのフォルダからの相対パスで持つ。フォルダを移しても再開できる（#1375、VB: DumpData）
                    ScenarioFileName = ToSavedPath(ScenarioFileName),
                    Turn = Turn,
                    LocalVariableList = Event.LocalVariableList,
                    DisableEventLabels = Event.colEventLabelList.List.Where(x => !x.Enable).Select(x => x.Data).Distinct().ToList(),
                    EventLabelEnabled = Event.colEventLabelList.List.Select(x => x.Enable).ToList(),
                    AdditionalEventFileNames = Event.AdditionalEventFileNames.ToList(),
                    Map = Map,
                    MapX = GUI.MapX,
                    MapY = GUI.MapY,
                    // BGMファイルパスをシナリオパスからの相対パスで保存
                    BGMFileName = !string.IsNullOrEmpty(Sound.BGMFileName)
                        ? ToSavedPath(Sound.BGMFileName)
                        : "",
                    RepeatMode = Sound.RepeatMode,
                    KeepBGM = Sound.KeepBGM,
                    BossBGM = Sound.BossBGM,
                    // 乱数系列の状態を保存
                    RndSeed = GeneralLib.RndSeed,
                    RndIndex = GeneralLib.RndIndex,
                };

                stream.Write(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data, Formatting.Indented)));
                if (saveKind == SRCSaveKind.Restart)
                {
                    IsRestartSaveDataAvailable = true;
                }
                else if (saveKind == SRCSaveKind.Quik)
                {
                    IsQuickSaveDataAvailable = true;
                    // クイックセーブのストリームに書いた（読んだ）。ファイルに書く（読む）ときは呼び出し元で倒す（#1530）
                    IsLastSaveDataQuikSaveStream = true;
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                GUI.ErrorMessage("セーブ中にエラーが発生しました");
            }

        }

        /// <summary>
        /// セーブデータに書くシナリオのファイルのパス。シナリオのフォルダの中ならシナリオのフォルダからの相対パスにする（#1375）。
        /// </summary>
        public string ToSavedPath(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(ScenarioPath))
            {
                return path;
            }
            var relative = FileSystem.ToRelativePath(ScenarioPath, path);
            return relative == FileSystem.NormalizePath(path) ? path : relative.TrimStart('\\');
        }

        /// <summary>
        /// セーブデータが持つシナリオのファイル（イベント・マップ）のパスを、いまのシナリオのフォルダで解く（#1375）。
        /// シナリオのフォルダからの相対パスならそのまま繋ぐ。
        /// 以前の版のセーブは絶対パスで持っていたので、いまのシナリオのフォルダの外を指していれば、
        /// 先頭から区切りを 1 つずつ外し、いまのシナリオのフォルダにあるファイルを探す（長く一致するものを先に）。
        /// 見つからなければ元のパスのままにする。
        /// </summary>
        public string ResolveSavedPath(string path)
        {
            if (string.IsNullOrEmpty(path) || string.IsNullOrEmpty(ScenarioPath))
            {
                return path;
            }
            // 相対パス、またはいまのシナリオのフォルダの中を指す絶対パス
            if (!FileSystem.IsAbsolutePath(path) || FileSystem.ToRelativePath(ScenarioPath, path) != FileSystem.NormalizePath(path))
            {
                return FileSystem.ToAbsolutePath(ScenarioPath, path);
            }
            return FindInScenarioFolder(path) ?? path;
        }

        /// <summary>
        /// 別のフォルダの絶対パスを、先頭から区切りを 1 つずつ外して、いまのシナリオのフォルダで探す（長く一致するものを先に）。
        /// 見つからなければ null
        /// </summary>
        private string FindInScenarioFolder(string path)
        {
            var parts = FileSystem.NormalizePath(path).Split('\\', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < parts.Length; i++)
            {
                var candidate = FileSystem.PathCombine(ScenarioPath, string.Join("\\", parts.Skip(i)));
                if (FileSystem.FileExists(candidate))
                {
                    return candidate;
                }
            }
            return null;
        }

        /// <summary>
        /// SRC# の JSON の中断データを読み、ユニットなどのリンクとパラメータまでを戻す（VB: RestoreData の PList.UpdateSupportMod の前まで）。
        /// </summary>
        private RestoredSuspendData RestoreJsonData(byte[] bytes, bool quick_load)
        {
            // 元の SRC は版（SaveDataVersion）で古い形式を読み替える。SRC# の JSON はまだ版で読み方を変えないので、版は見ない
            var data = JsonConvert.DeserializeObject<SRCSuspendData>(ReadJson(bytes));
            var savedScenarioFileName = ResolveSavedPath(data.ScenarioFileName);
            var scenario_file_is_different = !FileSystem.RelativePathEuqals(ScenarioPath, ScenarioFileName, savedScenarioFileName);
            if (scenario_file_is_different)
            {
                // ウィンドウのタイトルを設定（セーブデータ側のシナリオ名）
                GUI.MainFormText = "SRC# - " + Path.GetFileNameWithoutExtension(PathResolver.ToNative(savedScenarioFileName));
            }
            ScenarioFileName = savedScenarioFileName;
            Titles = data.Titles;

            // 使用するデータをロード
            if (!quick_load)
            {
                GUI.SetLoadImageSize(data.Titles.Count * 2 + 5);
                foreach (var title in Titles)
                {
                    IncludeData(title);
                }

                // ローカルデータの読みこみ
                LoadLocalData();
                Event.LoadEventData(ScenarioFileName, "リストア");
                GUI.DisplayLoadingProgress();
            }
            else
            {
                if (scenario_file_is_different)
                {
                    Event.LoadEventData(ScenarioFileName, "リストア");
                }
            }

            // 変数とユニットなどはイベントデータを読み込んだ後で戻す（元の SRC の RestoreEventData・PList.Restore など）。
            // イベントデータの読み込みが定義した変数（DebugMode の Option(デバッグ)）は、元の SRC と同じくセーブデータの変数で置き換わる
            Turn = data.Turn;
            TotalTurn = data.TotalTurn;
            Money = data.Money;
            Event.GlobalVariableList = data.GlobalVariableList;
            Event.LocalVariableList = data.LocalVariableList;
            Event.Restore(data);
            PList = data.PList;
            UList = data.UList;
            IList = data.IList;
            PList.Restore(this);
            UList.Restore(this);
            IList.Restore(this);
            Map.Restore(data);
            // マップの表示位置は SetMapSize によるリセットの後で復元する（VB: RestoreMapData）
            GUI.MapX = data.MapX;
            GUI.MapY = data.MapY;

            // 中断データのリンクとパラメータを戻す（VB: RestoreData の RestoreLinkInfo・RestoreParameter・UpdateSupportMod）
            // ユニットのステータスを更新してアビリティの残り使用回数を戻す（VB: Unit.RestoreLinkInfo）
            UList.Items.ToList().ForEach(x => x.RestoreLinkInfo());
            PList.Items.ToList().ForEach(x => x.Update());
            // ＳＰ・気力・霊力は最大値が特殊能力で変わるので、特殊能力を設定した後に設定し直す（VB: PList.RestoreParameter）
            // ユニットのＨＰ・ＥＮは JSON の値をそのまま使う（VB: UList.RestoreParameter）
            PList.Items.ToList().ForEach(x => x.RestoreParameter());

            // BGMファイルを相対パスまたはファイル名で検索
            var bgmFile = !string.IsNullOrEmpty(data.BGMFileName)
                ? Sound.SearchMidiFile("(" + data.BGMFileName + ")")
                    ?? Sound.SearchMidiFile("(" + Path.GetFileName(data.BGMFileName) + ")")
                : null;
            return new RestoredSuspendData
            {
                BGMFile = bgmFile,
                RepeatMode = data.RepeatMode,
                KeepBGM = data.KeepBGM,
                BossBGM = data.BossBGM,
                HasRandomSeries = true,
                RndSeed = data.RndSeed,
                RndIndex = data.RndIndex,
            };
        }

        public void RestoreData(string fname, SRCSaveKind saveKind)
        {
            // 元の SRC は読み終えてから LastSaveDataFileName を設定する。読み込みに失敗したら終了するので、先に設定しても変わらない。
            // ストリームから読む RestoreData はファイル名を知らないので、ここで設定する
            LastSaveDataFileName = fname;
            using (var stream = File.OpenRead(fname))
            {
                RestoreData(stream, saveKind);
            }
            IsLastSaveDataQuikSaveStream = false;
        }
        // 一時中断用データをロード
        public void RestoreData(Stream stream, SRCSaveKind saveKind)
        {
            var quick_load = saveKind == SRCSaveKind.Quik || saveKind == SRCSaveKind.Restart;
            try
            {
                // マウスカーソルを砂時計に
                GUI.ChangeStatus(GuiStatus.WaitCursor);
                if (quick_load)
                {
                    if (Expression.IsOptionDefined("デバッグ"))
                    {
                        Event.LoadEventData(ScenarioFileName, "クイックロード");
                    }
                }
                if (!quick_load)
                {
                    GUI.OpenNowLoadingForm();
                }

                var bytes = ReadAllBytes(stream);
                var restored = VbSaveDataReader.IsVbSaveData(bytes)
                    // 元の SRC の形式（#1577）
                    ? RestoreVbData(VbSaveDataReader.FromBytes(bytes), quick_load)
                    : RestoreJsonData(bytes, quick_load);

                // 支援効果による修正値はセーブしないので、パラメータを戻した後に計算し直す（VB: RestoreData の PList.UpdateSupportMod）
                PList.UpdateSupportMod();

                // ＢＧＭ関連の設定を復元
                if (!string.IsNullOrEmpty(restored.BGMFile))
                {
                    // BGMが必ず切り替わる状態で変更してから関連設定を復元する
                    Sound.KeepBGM = false;
                    Sound.BossBGM = false;
                    Sound.ChangeBGM(restored.BGMFile);
                    Sound.RepeatMode = restored.RepeatMode;
                    Sound.KeepBGM = restored.KeepBGM;
                    Sound.BossBGM = restored.BossBGM;
                }
                else
                {
                    // 元の SRC はＢＧＭが見つからなければ止め、連続演奏・固定・ボス用の設定は読み飛ばす（いまの設定のまま）
                    Sound.StopBGM();
                }

                // 乱数系列を復元（元の SRC の形式で、乱数系列を書いていない古いものは戻さない）
                if (restored.HasRandomSeries && !Expression.IsOptionDefined("デバッグ") && !Expression.IsOptionDefined("乱数系列非保存"))
                {
                    GeneralLib.RndSeed = restored.RndSeed;
                    GeneralLib.RndReset();
                    GeneralLib.RndIndex = restored.RndIndex;
                }

                if (!quick_load)
                {
                    GUI.DisplayLoadingProgress();
                }

                // 元の SRC はＢＧＭと乱数系列を戻してから、セーブファイルを 2 回読み直し（イベントデータは SkipEventData で読み飛ばす）、
                // リンク（PList・IList・UList の RestoreLinkInfo）とパラメータ（RestoreParameter）を戻してから PList.UpdateSupportMod を呼ぶ。
                // SRC# は上の RestoreJsonData（JSON）・RestoreVbData（元の SRC の形式）と PList.UpdateSupportMod で、ＢＧＭより先に同じことをする。
                // JSON から読むときは、アイテムのユニットは JSON の UnitId で引くので IList の分は要らない

                // 背景書き換え
                // 元の SRC はマップがロードに伴って変化したとき（または背景に書き込みがあったとき）だけ背景を書き直す。
                // SRC# はマップの復元（Map.Restore の SetMapSize）で背景のバッファを作り直すので、背景は一律に書き直す。
                // 再開イベントは元の SRC と同じく、背景を書き直す必要があったとき（Map.IsMapDirty）だけ発生させる（#1466）
                {
                    var is_map_dirty = Map.IsMapDirty;
                    int map_x, map_y;
                    map_x = GUI.MapX;
                    map_y = GUI.MapY;
                    GUI.SetupBackground(Map.MapDrawMode, "非同期");
                    GUI.MapX = map_x;
                    GUI.MapY = map_y;

                    if (is_map_dirty)
                    {
                        // 再開イベントによるマップ画像の書き換え処理を行う
                        Event.HandleEvent("再開");
                    }
                    Map.IsMapDirty = false;
                }

                Commands.SelectedUnit = null;
                Commands.SelectedTarget = null;

                // 画面更新
                GUI.Center(GUI.MapX, GUI.MapY);
                if (!quick_load)
                {
                    GUI.DisplayLoadingProgress();
                }

                if (!quick_load)
                {
                    GUI.CloseNowLoadingForm();
                }

                if (!quick_load)
                {
                    GUI.MainFormShow();
                }

                // マウスカーソルを元に戻す
                GUI.ChangeStatus(GuiStatus.Default);
                GUIStatus.ClearUnitStatus();
                if (!GUI.MainFormVisible)
                {
                    GUI.MainFormShow();
                    GUI.RefreshScreen();
                }

                GUI.RedrawScreen();
                if (Turn == 0)
                {
                    Event.HandleEvent("スタート");

                    // スタートイベントから次のステージが開始された場合、StartTurnが上のHandleEventで
                    // 実行されてしまう。
                    // 味方ターンの処理が２重起動されるのを防ぐため、Turnをチェックしてから起動する
                    if (Turn == 0)
                    {
                        StartTurn("味方");
                    }
                }
                else
                {
                    Commands.CommandState = "ユニット選択";
                    Stage = "味方";
                }
                if (saveKind == SRCSaveKind.Restart)
                {
                    IsRestartSaveDataAvailable = true;
                }
                else if (saveKind == SRCSaveKind.Quik)
                {
                    IsQuickSaveDataAvailable = true;
                    // クイックセーブのストリームに書いた（読んだ）。ファイルに書く（読む）ときは呼び出し元で倒す（#1530）
                    IsLastSaveDataQuikSaveStream = true;
                }
            }
            catch (SaveDataTerminateException ex)
            {
                // 元の SRC はメッセージを出してから End で終わる（「ロード中にエラーが発生しました」は出さない）
                LogError(ex);
                TerminateSRC();
            }
            catch (Exception ex)
            {
                LogError(ex);
                GUI.ErrorMessage("ロード中にエラーが発生しました");
                TerminateSRC();
            }
        }

        private static byte[] ReadAllBytes(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }

        // SRC# の JSON は UTF-8（BOM があれば除く）
        private static string ReadJson(byte[] bytes)
        {
            using (var reader = new StreamReader(new MemoryStream(bytes)))
            {
                return reader.ReadToEnd();
            }
        }

        /// <summary>中断データから読み、読んだ後の共通の処理（ＢＧＭ・乱数系列）に渡す値</summary>
        private sealed class RestoredSuspendData
        {
            /// <summary>見つけたＢＧＭのファイル（見つからなければ空か null）</summary>
            public string BGMFile { get; set; } = "";
            public bool RepeatMode { get; set; }
            public bool KeepBGM { get; set; }
            public bool BossBGM { get; set; }
            /// <summary>乱数系列を持っているか（元の SRC の古い形式は持たない）</summary>
            public bool HasRandomSeries { get; set; }
            public int RndSeed { get; set; }
            public int RndIndex { get; set; }
        }
    }
}
