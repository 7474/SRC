using Newtonsoft.Json;
using SRCCore.Expressions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace SRCCore
{
    public class SRCSaveData
    {
        public string Version { get; set; }
        public IList<string> Titles { get; set; }
        public string NextStage { get; set; }
        public int TotalTurn { get; set; }
        public int Money { get; set; }

        // XXX 列挙時の順番がDictionaryだと問題になるかも
        public IDictionary<string, VarData> GlobalVariableList;

        public Pilots.Pilots PList { get; set; }
        public Units.Units UList { get; set; }
        public Items.Items IList { get; set; }
    }

    public partial class SRC
    {
        // TODO セーブ・ロードの精査（まだとりあえず保存できるだけ）
        // データをセーブ
        //public void SaveData(string fname)
        //{
        //}
        public void SaveData(Stream stream)
        {
            try
            {
                var data = new SRCSaveData()
                {
                    // XXX EntryなのかCoreなのか
                    Version = Assembly.GetEntryAssembly().GetName().Version.ToString(),
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

                //    GUI.SetLoadImageSize((num * 2 + 5));
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
                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\alias.txt"))
                //    {
                //        ALDList.Load(ScenarioPath + @"Data\alias.txt");
                //    }

                //    bool localFileExists() { string argfname = ScenarioPath + @"Data\mind.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\sp.txt"))
                //    {
                //        SPDList.Load(ScenarioPath + @"Data\sp.txt");
                //    }
                //    else if (localFileExists())
                //    {
                //        SPDList.Load(ScenarioPath + @"Data\mind.txt");
                //    }

                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\pilot.txt"))
                //    {
                //        PDList.Load(ScenarioPath + @"Data\pilot.txt");
                //    }

                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\non_pilot.txt"))
                //    {
                //        NPDList.Load(ScenarioPath + @"Data\non_pilot.txt");
                //    }

                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\robot.txt"))
                //    {
                //        UDList.Load(ScenarioPath + @"Data\robot.txt");
                //    }

                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\unit.txt"))
                //    {
                //        UDList.Load(ScenarioPath + @"Data\unit.txt");
                //    }

                //    GUI.DisplayLoadingProgress();
                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\pilot_message.txt"))
                //    {
                //        MDList.Load(ScenarioPath + @"Data\pilot_message.txt");
                //    }

                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\pilot_dialog.txt"))
                //    {
                //        DDList.Load(ScenarioPath + @"Data\pilot_dialog.txt");
                //    }

                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\effect.txt"))
                //    {
                //        EDList.Load(ScenarioPath + @"Data\effect.txt");
                //    }

                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\animation.txt"))
                //    {
                //        ADList.Load(ScenarioPath + @"Data\animation.txt");
                //    }

                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\ext_animation.txt"))
                //    {
                //        EADList.Load(ScenarioPath + @"Data\ext_animation.txt");
                //    }

                //    if (GeneralLib.FileExists(ScenarioPath + @"Data\item.txt"))
                //    {
                //        IDList.Load(ScenarioPath + @"Data\item.txt");
                //    }

                //    GUI.DisplayLoadingProgress();
                //    IsLocalDataLoaded = true;

                PList.Restore(this);
                UList.Restore(this);
                IList.Restore(this);

                PList.Update();
                UList.Update();
                IList.Update();

                //    // リンクデータを処理するため、セーブファイルを一旦閉じてから再度読み込み

                //    SaveDataFileNumber = FileSystem.FreeFile();
                //    FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);
                //    if (SaveDataVersion > 10000)
                //    {
                //        FileSystem.Input(SaveDataFileNumber, dummy);
                //    }

                //    FileSystem.Input(SaveDataFileNumber, num);
                //    Titles = new string[(num + 1)];
                //    var loopTo1 = num;
                //    for (i = 1; i <= loopTo1; i++)
                //        FileSystem.Input(SaveDataFileNumber, Titles[i]);
                //    FileSystem.Input(SaveDataFileNumber, dummy);
                //    FileSystem.Input(SaveDataFileNumber, TotalTurn);
                //    FileSystem.Input(SaveDataFileNumber, Money);
                //    FileSystem.Input(SaveDataFileNumber, num); // パーツ用のダミー
                //    FileSystem.Input(SaveDataFileNumber, num); // パーツ用のダミー
                //    var loopTo2 = num;
                //    for (i = 1; i <= loopTo2; i++)
                //        dummy = FileSystem.LineInput(SaveDataFileNumber);
                //    PList.LoadLinkInfo();
                //    UList.LoadLinkInfo();
                //    IList.LoadLinkInfo();
                //    FileSystem.FileClose(SaveDataFileNumber);
                //    GUI.DisplayLoadingProgress();

                //    // ユニットの状態を回復
                //    foreach (Unit u in UList)
                //        u.Reset_Renamed();
                //    GUI.DisplayLoadingProgress();

                //    // 追加されたシステム側イベントデータの読み込み
                //    Event_Renamed.LoadEventData("", load_mode: "");
                //    GUI.DisplayLoadingProgress();
            }
            catch
            {
                GUI.ErrorMessage("ロード中にエラーが発生しました");
                TerminateSRC();
            }
        }

        // 一時中断用データをファイルにセーブする
        public void DumpData(string fname)
        {
            throw new NotImplementedException();
            //    int i;
            //    int num;
            //    ;

            //    // 中断データをセーブ
            //    SaveDataFileNumber = FileSystem.FreeFile();
            //    FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Output, OpenAccess.Write);

            //    {
            //        var withBlock = App;
            //        num = 10000 * My.MyProject.Application.Info.Version.Major + 100 * My.MyProject.Application.Info.Version.Minor + My.MyProject.Application.Info.Version.Revision;
            //    }

            //    FileSystem.WriteLine(SaveDataFileNumber, (object)num);
            //    FileSystem.WriteLine(SaveDataFileNumber, Strings.Mid(ScenarioFileName, Strings.Len(ScenarioPath) + 1));
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)Information.UBound(Titles));
            //    var loopTo = Information.UBound(Titles);
            //    for (i = 1; i <= loopTo; i++)
            //        FileSystem.WriteLine(SaveDataFileNumber, Titles[i]);
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)Turn);
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)TotalTurn);
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)Money);
            //    Event_Renamed.DumpEventData();
            //    PList.Dump();
            //    IList.Dump();
            //    UList.Dump();
            //    Map.DumpMapData();

            //    // Midi じゃなくて midi じゃないと検索失敗するようになってるので。
            //    if (Strings.InStr(Strings.LCase(Sound.BGMFileName), @"\midi\") > 0)
            //    {
            //        FileSystem.WriteLine(SaveDataFileNumber, Strings.Mid(Sound.BGMFileName, Strings.InStr(Strings.LCase(Sound.BGMFileName), @"\midi\") + 6));
            //    }
            //    else if (Strings.InStr(Sound.BGMFileName, @"\") > 0)
            //    {
            //        FileSystem.WriteLine(SaveDataFileNumber, Strings.Mid(Sound.BGMFileName, Strings.InStr(Sound.BGMFileName, @"\") + 1));
            //    }
            //    else
            //    {
            //        FileSystem.WriteLine(SaveDataFileNumber, Sound.BGMFileName);
            //    }

            //    FileSystem.WriteLine(SaveDataFileNumber, (object)Sound.RepeatMode);
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)Sound.KeepBGM);
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)Sound.BossBGM);
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)GeneralLib.RndSeed);
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)GeneralLib.RndIndex);
            //    FileSystem.FileClose(SaveDataFileNumber);
            //    LastSaveDataFileName = fname;
            //    if (Strings.InStr(fname, @"\_リスタート.src") > 0)
            //    {
            //        IsRestartSaveDataAvailable = true;
            //    }
            //    else if (Strings.InStr(fname, @"\_クイックセーブ.src") > 0)
            //    {
            //        IsQuickSaveDataAvailable = true;
            //    }

            //    return;
            //ErrorHandler:
            //    ;
            //    GUI.ErrorMessage("セーブ中にエラーが発生しました");
            //    FileSystem.FileClose(SaveDataFileNumber);
        }

        // 一時中断用データをロード
        public void RestoreData(string fname, bool quick_load)
        {
            throw new NotImplementedException();
            //    int i, num = default;
            //    var fname2 = default(string);
            //    string dummy;
            //    var scenario_file_is_different = default(bool);
            //    ;

            //    // マウスカーソルを砂時計に
            //    // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            //    Cursor.Current = Cursors.WaitCursor;
            //    if (quick_load)
            //    {
            //        if (Expression.IsOptionDefined("デバッグ"))
            //        {
            //            Event_Renamed.LoadEventData(ScenarioFileName, "クイックロード");
            //        }
            //    }

            //    if (!quick_load)
            //    {
            //        GUI.OpenNowLoadingForm();
            //    }

            //    SaveDataFileNumber = FileSystem.FreeFile();
            //    FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);
            //    FileSystem.Input(SaveDataFileNumber, fname2);
            //    if (Information.IsNumeric(fname2))
            //    {
            //        SaveDataVersion = Conversions.ToInteger(fname2);
            //        FileSystem.Input(SaveDataFileNumber, fname2);
            //    }
            //    else
            //    {
            //        SaveDataVersion = 1;
            //    }

            //    // ウィンドウのタイトルを設定
            //    if ((ScenarioFileName ?? "") != (ScenarioPath + fname2 ?? ""))
            //    {
            //        GUI.MainForm.Text = "SRC - " + Strings.Left(fname2, Strings.Len(fname2) - 4);
            //        ScenarioFileName = ScenarioPath + fname2;
            //        scenario_file_is_different = true;
            //    }

            //    FileSystem.Input(SaveDataFileNumber, num);

            //    // 使用するデータをロード
            //    if (!quick_load)
            //    {
            //        GUI.SetLoadImageSize((num * 2 + 5));
            //        Titles = new string[(num + 1)];
            //        var loopTo = num;
            //        for (i = 1; i <= loopTo; i++)
            //        {
            //            FileSystem.Input(SaveDataFileNumber, Titles[i]);
            //            IncludeData(Titles[i]);
            //        }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\alias.txt"))
            //        {
            //            ALDList.Load(ScenarioPath + @"Data\alias.txt");
            //        }

            //        bool localFileExists() { string argfname = ScenarioPath + @"Data\mind.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\sp.txt"))
            //        {
            //            SPDList.Load(ScenarioPath + @"Data\sp.txt");
            //        }
            //        else if (localFileExists())
            //        {
            //            SPDList.Load(ScenarioPath + @"Data\mind.txt");
            //        }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\pilot.txt"))
            //        {
            //            PDList.Load(ScenarioPath + @"Data\pilot.txt");
            //        }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\non_pilot.txt"))
            //        {
            //            NPDList.Load(ScenarioPath + @"Data\non_pilot.txt");
            //        }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\robot.txt"))
            //        {
            //            UDList.Load(ScenarioPath + @"Data\robot.txt");
            //        }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\unit.txt"))
            //        {
            //            UDList.Load(ScenarioPath + @"Data\unit.txt");
            //        }

            //        GUI.DisplayLoadingProgress();
            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\pilot_message.txt"))
            //        {
            //            MDList.Load(ScenarioPath + @"Data\pilot_message.txt");
            //        }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\pilot_dialog.txt"))
            //        {
            //            DDList.Load(ScenarioPath + @"Data\pilot_dialog.txt");
            //        }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\effect.txt"))
            //        {
            //            EDList.Load(ScenarioPath + @"Data\effect.txt");
            //        }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\animation.txt"))
            //        {
            //            ADList.Load(ScenarioPath + @"Data\animation.txt");
            //        }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\ext_animation.txt"))
            //        {
            //            EADList.Load(ScenarioPath + @"Data\ext_animation.txt");
            //        }

            //        if (GeneralLib.FileExists(ScenarioPath + @"Data\item.txt"))
            //        {
            //            IDList.Load(ScenarioPath + @"Data\item.txt");
            //        }

            //        GUI.DisplayLoadingProgress();
            //        IsLocalDataLoaded = true;
            //        Event_Renamed.LoadEventData(ScenarioFileName, "リストア");
            //        GUI.DisplayLoadingProgress();
            //    }
            //    else
            //    {
            //        var loopTo1 = num;
            //        for (i = 1; i <= loopTo1; i++)
            //            dummy = FileSystem.LineInput(SaveDataFileNumber);
            //        if (scenario_file_is_different)
            //        {
            //            Event_Renamed.LoadEventData(ScenarioFileName, "リストア");
            //        }
            //    }

            //    FileSystem.Input(SaveDataFileNumber, Turn);
            //    FileSystem.Input(SaveDataFileNumber, TotalTurn);
            //    FileSystem.Input(SaveDataFileNumber, Money);
            //    Event_Renamed.RestoreEventData();
            //    PList.Restore();
            //    IList.Restore();
            //    UList.Restore();

            //    // MOD START 240a
            //    // RestoreMapData
            //    // 'ＢＧＭ関連の設定を復元
            //    // Input #SaveDataFileNumber, fname2
            //    // マップデータの互換性維持のため、RestoreMapDataでＢＧＭ関連の１行目まで読み込んで戻り値にした
            //    fname2 = Map.RestoreMapData();
            //    // MOD  END  240a
            //    fname2 = Sound.SearchMidiFile("(" + fname2 + ")");
            //    if (!string.IsNullOrEmpty(fname2))
            //    {
            //        Sound.KeepBGM = false;
            //        Sound.BossBGM = false;
            //        Sound.ChangeBGM(fname2);
            //        FileSystem.Input(SaveDataFileNumber, Sound.RepeatMode);
            //        FileSystem.Input(SaveDataFileNumber, Sound.KeepBGM);
            //        FileSystem.Input(SaveDataFileNumber, Sound.BossBGM);
            //    }
            //    else
            //    {
            //        Sound.StopBGM();
            //        dummy = FileSystem.LineInput(SaveDataFileNumber);
            //        dummy = FileSystem.LineInput(SaveDataFileNumber);
            //        dummy = FileSystem.LineInput(SaveDataFileNumber);
            //    }

            //    // 乱数系列を復元
            //    if (!Expression.IsOptionDefined("デバッグ") & !Expression.IsOptionDefined("乱数系列非保存") & !FileSystem.EOF(SaveDataFileNumber))
            //    {
            //        FileSystem.Input(SaveDataFileNumber, GeneralLib.RndSeed);
            //        GeneralLib.RndReset();
            //        FileSystem.Input(SaveDataFileNumber, GeneralLib.RndIndex);
            //    }

            //    if (!quick_load)
            //    {
            //        GUI.DisplayLoadingProgress();
            //    }

            //    FileSystem.FileClose(SaveDataFileNumber);

            //    // リンクデータを処理するため、セーブファイルを一旦閉じてから再度読み込み

            //    SaveDataFileNumber = FileSystem.FreeFile();
            //    FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);

            //    // SaveDataVersion
            //    if (SaveDataVersion > 10000)
            //    {
            //        dummy = FileSystem.LineInput(SaveDataFileNumber);
            //    }

            //    // ScenarioFileName
            //    dummy = FileSystem.LineInput(SaveDataFileNumber);

            //    // 使用するデータ名
            //    FileSystem.Input(SaveDataFileNumber, num);
            //    var loopTo2 = num;
            //    for (i = 1; i <= loopTo2; i++)
            //        dummy = FileSystem.LineInput(SaveDataFileNumber);

            //    // Turn
            //    dummy = FileSystem.LineInput(SaveDataFileNumber);
            //    // TotalTurn
            //    dummy = FileSystem.LineInput(SaveDataFileNumber);
            //    // Money
            //    dummy = FileSystem.LineInput(SaveDataFileNumber);
            //    Event_Renamed.SkipEventData();
            //    PList.RestoreLinkInfo();
            //    IList.RestoreLinkInfo();
            //    UList.RestoreLinkInfo();
            //    FileSystem.FileClose(SaveDataFileNumber);

            //    // パラメータ情報を処理するため、セーブファイルを一旦閉じてから再度読み込み。
            //    // 霊力やＨＰ、ＥＮといったパラメータは最大値が特殊能力で変動するため、
            //    // 特殊能力の設定が終わってから改めて設定してやる必要がある。

            //    SaveDataFileNumber = FileSystem.FreeFile();
            //    FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);

            //    // SaveDataVersion
            //    if (SaveDataVersion > 10000)
            //    {
            //        dummy = FileSystem.LineInput(SaveDataFileNumber);
            //    }

            //    // ScenarioFileName
            //    dummy = FileSystem.LineInput(SaveDataFileNumber);

            //    // 使用するデータ名
            //    FileSystem.Input(SaveDataFileNumber, num);
            //    var loopTo3 = num;
            //    for (i = 1; i <= loopTo3; i++)
            //        dummy = FileSystem.LineInput(SaveDataFileNumber);

            //    // Turn
            //    dummy = FileSystem.LineInput(SaveDataFileNumber);
            //    // TotalTurn
            //    dummy = FileSystem.LineInput(SaveDataFileNumber);
            //    // Money
            //    dummy = FileSystem.LineInput(SaveDataFileNumber);
            //    Event_Renamed.SkipEventData();
            //    PList.RestoreParameter();
            //    IList.RestoreParameter();
            //    UList.RestoreParameter();
            //    PList.UpdateSupportMod();

            //    // 背景書き換え
            //    int map_x, map_y;
            //    if (Map.IsMapDirty)
            //    {
            //        map_x = GUI.MapX;
            //        map_y = GUI.MapY;
            //        GUI.SetupBackground(Map.MapDrawMode, "非同期", filter_color: 0, filter_trans_par: 0d);
            //        GUI.MapX = map_x;
            //        GUI.MapY = map_y;

            //        // 再開イベントによるマップ画像の書き換え処理を行う
            //        Event_Renamed.HandleEvent("再開");
            //        Map.IsMapDirty = false;
            //    }

            //    // UPGRADE_NOTE: オブジェクト SelectedUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Commands.SelectedUnit = null;
            //    // UPGRADE_NOTE: オブジェクト SelectedTarget をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            //    Commands.SelectedTarget = null;

            //    // ユニット画像生成
            //    foreach (Unit u in UList)
            //    {
            //        {
            //            var withBlock = u;
            //            if (withBlock.BitmapID == 0)
            //            {
            //                withBlock.BitmapID = GUI.MakeUnitBitmap(u);
            //            }
            //        }
            //    }

            //    // 画面更新
            //    GUI.Center(GUI.MapX, GUI.MapY);
            //    FileSystem.FileClose(SaveDataFileNumber);
            //    if (!quick_load)
            //    {
            //        GUI.DisplayLoadingProgress();
            //    }

            //    if (!quick_load)
            //    {
            //        GUI.CloseNowLoadingForm();
            //    }

            //    if (!quick_load)
            //    {
            //        GUI.MainForm.Show();
            //    }

            //    // マウスカーソルを元に戻す
            //    // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            //    Cursor.Current = Cursors.Default;
            //    Status.ClearUnitStatus();
            //    if (!GUI.MainForm.Visible)
            //    {
            //        GUI.MainForm.Show();
            //        GUI.MainForm.Refresh();
            //    }

            //    GUI.RedrawScreen();
            //    if (Turn == 0)
            //    {
            //        Event_Renamed.HandleEvent("スタート");

            //        // MOD START MARGE
            //        // StartTurn "味方"
            //        // スタートイベントから次のステージが開始された場合、StartTurnが上のHandleEventで
            //        // 実行されてしまう。
            //        // 味方ターンの処理が２重起動されるのを防ぐため、Turnをチェックしてから起動する
            //        if (Turn == 0)
            //        {
            //            StartTurn("味方");
            //        }
            //    }
            //    // MOD END MARGE
            //    else
            //    {
            //        Commands.CommandState = "ユニット選択";
            //        Stage = "味方";
            //    }

            //    LastSaveDataFileName = fname;
            //    if (Strings.InStr(fname, @"\_リスタート.src") > 0)
            //    {
            //        IsRestartSaveDataAvailable = true;
            //    }
            //    else if (Strings.InStr(fname, @"\_クイックセーブ.src") > 0)
            //    {
            //        IsQuickSaveDataAvailable = true;
            //    }

            //    return;
            //ErrorHandler:
            //    ;
            //    GUI.ErrorMessage("ロード中にエラーが発生しました");
            //    FileSystem.FileClose(SaveDataFileNumber);
            //    TerminateSRC();
        }
    }
}
