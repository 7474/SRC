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

    public class SRCSuspendData : SRCSaveData
    {
        public string ScenarioFileName { get; set; }
        public int Turn { get; set; }
        public SrcCollection<VarData> LocalVariableList { get; set; }
        public IList<string> DisableEventLabels { get; set; }
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
        // TODO セーブ・ロードの精査（基本的な保存・読み込みは実装済み; 完全な互換性検証が残タスク）
        // データをセーブ
        public void SaveData(Stream stream)
        {
            try
            {
                var data = new SRCSaveData()
                {
                    // XXX EntryなのかCoreなのか
                    Version = Assembly.GetEntryAssembly().GetName().Version.ToString(),
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
                // XXX Version プロパティだけのオブジェクトでバージョンチェックなど
                var data = JsonConvert.DeserializeObject<SRCSaveData>((new StreamReader(stream).ReadToEnd()));
                GUI.SetLoadImageSize((data.Titles.Count * 2 + 5));
                Titles = data.Titles;
                if (!Expression.IsGlobalVariableDefined("次ステージ"))
                {
                    Expression.DefineGlobalVariable("次ステージ");
                }
                Expression.SetVariableAsString("次ステージ", data.NextStage);
                TotalTurn = data.TotalTurn;
                Money = data.Money;
                Event.GlobalVariableList = data.GlobalVariableList;
                PList = data.PList;
                UList = data.UList;
                IList = data.IList;

                foreach (var title in Titles)
                {
                    IncludeData(title);
                }
                // ローカルデータの読みこみ
                LoadLocalData();

                PList.Restore(this);
                UList.Restore(this);
                IList.Restore(this);

                PList.Update();
                UList.Update();
                IList.Update();

                //    // リンクデータを処理するため、セーブファイルを一旦閉じてから再度読み込み
                //    PList.LoadLinkInfo();
                //    UList.LoadLinkInfo();
                //    IList.LoadLinkInfo();
                //    FileSystem.FileClose(SaveDataFileNumber);
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
            catch (Exception ex)
            {
                LogError(ex);
                GUI.ErrorMessage("ロード中にエラーが発生しました");
                TerminateSRC();
            }
        }

        public void DumpData(string fname, SRCSaveKind saveKind)
        {
            // XXX FileName
            LastSaveDataFileName = fname;
            using (var stream = FileSystem.Create(fname))
            {
                DumpData(stream, saveKind);
            }
        }

        // 一時中断用データをファイルにセーブする
        public void DumpData(Stream stream, SRCSaveKind saveKind)
        {
            try
            {
                var data = new SRCSuspendData()
                {
                    // XXX EntryなのかCoreなのか
                    Version = Assembly.GetEntryAssembly().GetName().Version.ToString(),
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
                    AdditionalEventFileNames = Event.AdditionalEventFileNames.ToList(),
                    Map = Map,
                    MapX = GUI.MapX,
                    MapY = GUI.MapY,
                    // BGMファイルパスをシナリオパスからの相対パスで保存
                    BGMFileName = !string.IsNullOrEmpty(Sound.BGMFileName)
                        ? FileSystem.ToRelativePath(ScenarioPath, Sound.BGMFileName)
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
            var parts = FileSystem.NormalizePath(path).Split('\\', StringSplitOptions.RemoveEmptyEntries);
            for (var i = 0; i < parts.Length; i++)
            {
                var candidate = FileSystem.PathCombine(ScenarioPath, string.Join("\\", parts.Skip(i)));
                if (FileSystem.FileExists(candidate))
                {
                    return candidate;
                }
            }
            return path;
        }

        public void RestoreData(string fname, SRCSaveKind saveKind)
        {
            // XXX FileName
            LastSaveDataFileName = fname;
            using (var stream = File.OpenRead(fname))
            {
                RestoreData(stream, saveKind);
            }
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

                // XXX Version プロパティだけのオブジェクトでバージョンチェックなど
                var data = JsonConvert.DeserializeObject<SRCSuspendData>((new StreamReader(stream).ReadToEnd()));
                //SaveDataVersion = Conversions.ToInteger(fname2);
                var savedScenarioFileName = ResolveSavedPath(data.ScenarioFileName);
                var scenario_file_is_different = !FileSystem.RelativePathEuqals(ScenarioPath, ScenarioFileName, savedScenarioFileName);
                if (scenario_file_is_different)
                {
                    // ウィンドウのタイトルを設定（セーブデータ側のシナリオ名）
                    GUI.MainFormText = "SRC# - " + Path.GetFileNameWithoutExtension(PathResolver.ToNative(savedScenarioFileName));
                }
                Titles = data.Titles;
                TotalTurn = data.TotalTurn;
                Money = data.Money;
                Event.GlobalVariableList = data.GlobalVariableList;
                PList = data.PList;
                UList = data.UList;
                IList = data.IList;
                //
                ScenarioFileName = savedScenarioFileName;
                Turn = data.Turn;
                Event.LocalVariableList = data.LocalVariableList;

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

                Event.Restore(data);
                PList.Restore(this);
                UList.Restore(this);
                IList.Restore(this);
                Map.Restore(data);
                // マップの表示位置は SetMapSize によるリセットの後で復元する（VB: RestoreMapData）
                GUI.MapX = data.MapX;
                GUI.MapY = data.MapY;

                // XXX 仮処理
                UList.Items.ToList().ForEach(x => x.Update());
                PList.Items.ToList().ForEach(x => x.Update());

                // ＢＧＭ関連の設定を復元
                // BGMファイルを相対パスまたはファイル名で検索
                var bgmFile = !string.IsNullOrEmpty(data.BGMFileName)
                    ? Sound.SearchMidiFile("(" + data.BGMFileName + ")")
                        ?? Sound.SearchMidiFile("(" + Path.GetFileName(data.BGMFileName) + ")")
                    : null;
                if (!string.IsNullOrEmpty(bgmFile))
                {
                    // BGMが必ず切り替わる状態で変更してから関連設定を復元する
                    Sound.KeepBGM = false;
                    Sound.BossBGM = false;
                    Sound.ChangeBGM(bgmFile);
                }
                else
                {
                    Sound.StopBGM();
                }
                Sound.RepeatMode = data.RepeatMode;
                Sound.KeepBGM = data.KeepBGM;
                Sound.BossBGM = data.BossBGM;

                // 乱数系列を復元
                if (!Expression.IsOptionDefined("デバッグ") && !Expression.IsOptionDefined("乱数系列非保存"))
                {
                    GeneralLib.RndSeed = data.RndSeed;
                    GeneralLib.RndReset();
                    GeneralLib.RndIndex = data.RndIndex;
                }

                if (!quick_load)
                {
                    GUI.DisplayLoadingProgress();
                }

                //    // リンクデータを処理するため、セーブファイルを一旦閉じてから再度読み込み
                //    Event.SkipEventData();
                //    PList.RestoreLinkInfo();
                //    IList.RestoreLinkInfo();
                //    UList.RestoreLinkInfo();
                //    FileSystem.FileClose(SaveDataFileNumber);

                //    // パラメータ情報を処理するため、セーブファイルを一旦閉じてから再度読み込み。
                //    // 霊力やＨＰ、ＥＮといったパラメータは最大値が特殊能力で変動するため、
                //    // 特殊能力の設定が終わってから改めて設定してやる必要がある。

                //    Event.SkipEventData();
                //    PList.RestoreParameter();
                //    IList.RestoreParameter();
                //    UList.RestoreParameter();
                //    PList.UpdateSupportMod();

                // 背景書き換え
                // XXX 元はマップがロードに伴って変化した時のみ処理していたが一律処理する
                //if (Map.IsMapDirty)
                {
                    int map_x, map_y;
                    map_x = GUI.MapX;
                    map_y = GUI.MapY;
                    GUI.SetupBackground(Map.MapDrawMode, "非同期");
                    GUI.MapX = map_x;
                    GUI.MapY = map_y;

                    // 再開イベントによるマップ画像の書き換え処理を行う
                    Event.HandleEvent("再開");
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
                }
            }
            catch (Exception ex)
            {
                LogError(ex);
                GUI.ErrorMessage("ロード中にエラーが発生しました");
                TerminateSRC();
            }
        }
    }
}
