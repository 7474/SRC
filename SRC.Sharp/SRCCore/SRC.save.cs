using Newtonsoft.Json;
using SRCCore.Events;
using SRCCore.Expressions;
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
        // TODO セーブ・ロードの精査（まだとりあえず保存できるだけ）
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
            catch
            {
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
                // XXX Dataフォルダ直下って読んでる？
                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\alias.txt"))
                //    {
                //        ALDList.Load(ScenarioPath + @"Data\alias.txt");
                //    }

                //    bool localFileExists() { string argfname = ScenarioPath + @"Data\mind.txt"; var ret = SRC.FileSystem.FileExists(argfname); return ret; }

                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\sp.txt"))
                //    {
                //        SPDList.Load(ScenarioPath + @"Data\sp.txt");
                //    }
                //    else if (localFileExists())
                //    {
                //        SPDList.Load(ScenarioPath + @"Data\mind.txt");
                //    }

                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\pilot.txt"))
                //    {
                //        PDList.Load(ScenarioPath + @"Data\pilot.txt");
                //    }

                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\non_pilot.txt"))
                //    {
                //        NPDList.Load(ScenarioPath + @"Data\non_pilot.txt");
                //    }

                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\robot.txt"))
                //    {
                //        UDList.Load(ScenarioPath + @"Data\robot.txt");
                //    }

                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\unit.txt"))
                //    {
                //        UDList.Load(ScenarioPath + @"Data\unit.txt");
                //    }

                //    GUI.DisplayLoadingProgress();
                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\pilot_message.txt"))
                //    {
                //        MDList.Load(ScenarioPath + @"Data\pilot_message.txt");
                //    }

                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\pilot_dialog.txt"))
                //    {
                //        DDList.Load(ScenarioPath + @"Data\pilot_dialog.txt");
                //    }

                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\effect.txt"))
                //    {
                //        EDList.Load(ScenarioPath + @"Data\effect.txt");
                //    }

                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\animation.txt"))
                //    {
                //        ADList.Load(ScenarioPath + @"Data\animation.txt");
                //    }

                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\ext_animation.txt"))
                //    {
                //        EADList.Load(ScenarioPath + @"Data\ext_animation.txt");
                //    }

                //    if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\item.txt"))
                //    {
                //        IDList.Load(ScenarioPath + @"Data\item.txt");
                //    }

                GUI.DisplayLoadingProgress();
                IsLocalDataLoaded = true;

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
            catch
            {
                GUI.ErrorMessage("ロード中にエラーが発生しました");
                TerminateSRC();
            }
        }

        public void DumpData(string fname, SRCSaveKind saveKind)
        {
            // XXX FileName
            LastSaveDataFileName = fname;
            using (var stream = new FileInfo(fname).Open(FileMode.Create))
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
                    ScenarioFileName = ScenarioFileName,
                    Turn = Turn,
                    LocalVariableList = Event.LocalVariableList,
                    DisableEventLabels = Event.colEventLabelList.List.Where(x => !x.Enable).Select(x => x.Data).ToList(),
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
            catch
            {
                GUI.ErrorMessage("セーブ中にエラーが発生しました");
            }

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
                var scenario_file_is_different = !FileSystem.RelativePathEuqals(ScenarioPath, ScenarioFileName, data.ScenarioFileName);
                if (scenario_file_is_different)
                {
                    // ウィンドウのタイトルを設定
                    GUI.MainFormText = "SRC# - " + Path.GetFileNameWithoutExtension(ScenarioFileName);
                }
                Titles = data.Titles;
                TotalTurn = data.TotalTurn;
                Money = data.Money;
                Event.GlobalVariableList = data.GlobalVariableList;
                PList = data.PList;
                UList = data.UList;
                IList = data.IList;
                //
                ScenarioFileName = FileSystem.ToAbsolutePath(ScenarioPath, data.ScenarioFileName);
                Turn = data.Turn;
                Event.LocalVariableList = data.LocalVariableList;
                GUI.MapX = data.MapX;
                GUI.MapY = data.MapY;

                // 使用するデータをロード
                if (!quick_load)
                {
                    GUI.SetLoadImageSize(data.Titles.Count * 2 + 5);
                    foreach (var title in Titles)
                    {
                        IncludeData(title);
                    }

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\alias.txt"))
                    //{
                    //    ALDList.Load(ScenarioPath + @"Data\alias.txt");
                    //}

                    //bool localFileExists() { string argfname = ScenarioPath + @"Data\mind.txt"; var ret = SRC.FileSystem.FileExists(argfname); return ret; }

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\sp.txt"))
                    //{
                    //    SPDList.Load(ScenarioPath + @"Data\sp.txt");
                    //}
                    //else if (localFileExists())
                    //{
                    //    SPDList.Load(ScenarioPath + @"Data\mind.txt");
                    //}

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\pilot.txt"))
                    //{
                    //    PDList.Load(ScenarioPath + @"Data\pilot.txt");
                    //}

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\non_pilot.txt"))
                    //{
                    //    NPDList.Load(ScenarioPath + @"Data\non_pilot.txt");
                    //}

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\robot.txt"))
                    //{
                    //    UDList.Load(ScenarioPath + @"Data\robot.txt");
                    //}

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\unit.txt"))
                    //{
                    //    UDList.Load(ScenarioPath + @"Data\unit.txt");
                    //}

                    //GUI.DisplayLoadingProgress();
                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\pilot_message.txt"))
                    //{
                    //    MDList.Load(ScenarioPath + @"Data\pilot_message.txt");
                    //}

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\pilot_dialog.txt"))
                    //{
                    //    DDList.Load(ScenarioPath + @"Data\pilot_dialog.txt");
                    //}

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\effect.txt"))
                    //{
                    //    EDList.Load(ScenarioPath + @"Data\effect.txt");
                    //}

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\animation.txt"))
                    //{
                    //    ADList.Load(ScenarioPath + @"Data\animation.txt");
                    //}

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\ext_animation.txt"))
                    //{
                    //    EADList.Load(ScenarioPath + @"Data\ext_animation.txt");
                    //}

                    //if (SRC.FileSystem.FileExists(ScenarioPath + @"Data\item.txt"))
                    //{
                    //    IDList.Load(ScenarioPath + @"Data\item.txt");
                    //}

                    GUI.DisplayLoadingProgress();
                    IsLocalDataLoaded = true;
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
            catch
            {
                GUI.ErrorMessage("ロード中にエラーが発生しました");
                TerminateSRC();
            }
        }
    }
}
