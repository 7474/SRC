using System;
using System.Collections.Generic;
using System.Text;

namespace SRCCore
{
    public partial class SRC
    {
        // データをセーブ
        public void SaveData(string fname)
        {
            throw new NotImplementedException();
            //try
            //{
            //    int i;
            //    int num;
            //    SaveDataFileNumber = FileSystem.FreeFile();
            //    FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Output, OpenAccess.Write);

            //    {
            //        var withBlock = App;
            //        num = 10000 * My.MyProject.Application.Info.Version.Major + 100 * My.MyProject.Application.Info.Version.Minor + My.MyProject.Application.Info.Version.Revision;
            //    }

            //    FileSystem.WriteLine(SaveDataFileNumber, (object)num);
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)Information.UBound(Titles));
            //    var loopTo = Information.UBound(Titles);
            //    for (i = 1; i <= loopTo; i++)
            //        FileSystem.WriteLine(SaveDataFileNumber, Titles[i]);
            //    string argexpr = "次ステージ";
            //    FileSystem.WriteLine(SaveDataFileNumber, Expression.GetValueAsString(argexpr));
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)TotalTurn);
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)Money);
            //    FileSystem.WriteLine(SaveDataFileNumber, (object)0); // パーツ用のダミー
            //    Event_Renamed.SaveGlobalVariables();
            //    PList.Save();
            //    UList.Save();
            //    IList.Save();
            //    FileSystem.FileClose(SaveDataFileNumber);
            //}
            //catch
            //{
            //    string argmsg = "セーブ中にエラーが発生しました";
            //    GUI.ErrorMessage(argmsg);
            //}
        }

        // データをロード
        public void LoadData(string fname)
        {
            throw new NotImplementedException();
            //    int i, num = default;
            //    var fname2 = default(string);
            //    var dummy = default(string);
            //    SaveDataFileNumber = FileSystem.FreeFile();
            //    FileSystem.FileOpen(SaveDataFileNumber, fname, OpenMode.Input);
            //    FileSystem.Input(SaveDataFileNumber, SaveDataVersion);
            //    if (SaveDataVersion > 10000)
            //    {
            //        FileSystem.Input(SaveDataFileNumber, num);
            //    }
            //    else
            //    {
            //        num = SaveDataVersion;
            //    }

            //    GUI.SetLoadImageSize((num * 2 + 5));
            //    Titles = new string[(num + 1)];
            //    var loopTo = num;
            //    for (i = 1; i <= loopTo; i++)
            //    {
            //        FileSystem.Input(SaveDataFileNumber, Titles[i]);
            //        IncludeData(Titles[i]);
            //    }

            //    string argfname1 = ScenarioPath + @"Data\alias.txt";
            //    if (GeneralLib.FileExists(argfname1))
            //    {
            //        string argfname = ScenarioPath + @"Data\alias.txt";
            //        ALDList.Load(argfname);
            //    }

            //    bool localFileExists() { string argfname = ScenarioPath + @"Data\mind.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

            //    string argfname4 = ScenarioPath + @"Data\sp.txt";
            //    if (GeneralLib.FileExists(argfname4))
            //    {
            //        string argfname2 = ScenarioPath + @"Data\sp.txt";
            //        SPDList.Load(argfname2);
            //    }
            //    else if (localFileExists())
            //    {
            //        string argfname3 = ScenarioPath + @"Data\mind.txt";
            //        SPDList.Load(argfname3);
            //    }

            //    string argfname6 = ScenarioPath + @"Data\pilot.txt";
            //    if (GeneralLib.FileExists(argfname6))
            //    {
            //        string argfname5 = ScenarioPath + @"Data\pilot.txt";
            //        PDList.Load(argfname5);
            //    }

            //    string argfname8 = ScenarioPath + @"Data\non_pilot.txt";
            //    if (GeneralLib.FileExists(argfname8))
            //    {
            //        string argfname7 = ScenarioPath + @"Data\non_pilot.txt";
            //        NPDList.Load(argfname7);
            //    }

            //    string argfname10 = ScenarioPath + @"Data\robot.txt";
            //    if (GeneralLib.FileExists(argfname10))
            //    {
            //        string argfname9 = ScenarioPath + @"Data\robot.txt";
            //        UDList.Load(argfname9);
            //    }

            //    string argfname12 = ScenarioPath + @"Data\unit.txt";
            //    if (GeneralLib.FileExists(argfname12))
            //    {
            //        string argfname11 = ScenarioPath + @"Data\unit.txt";
            //        UDList.Load(argfname11);
            //    }

            //    GUI.DisplayLoadingProgress();
            //    string argfname14 = ScenarioPath + @"Data\pilot_message.txt";
            //    if (GeneralLib.FileExists(argfname14))
            //    {
            //        string argfname13 = ScenarioPath + @"Data\pilot_message.txt";
            //        MDList.Load(argfname13);
            //    }

            //    string argfname16 = ScenarioPath + @"Data\pilot_dialog.txt";
            //    if (GeneralLib.FileExists(argfname16))
            //    {
            //        string argfname15 = ScenarioPath + @"Data\pilot_dialog.txt";
            //        DDList.Load(argfname15);
            //    }

            //    string argfname18 = ScenarioPath + @"Data\effect.txt";
            //    if (GeneralLib.FileExists(argfname18))
            //    {
            //        string argfname17 = ScenarioPath + @"Data\effect.txt";
            //        EDList.Load(argfname17);
            //    }

            //    string argfname20 = ScenarioPath + @"Data\animation.txt";
            //    if (GeneralLib.FileExists(argfname20))
            //    {
            //        string argfname19 = ScenarioPath + @"Data\animation.txt";
            //        ADList.Load(argfname19);
            //    }

            //    string argfname22 = ScenarioPath + @"Data\ext_animation.txt";
            //    if (GeneralLib.FileExists(argfname22))
            //    {
            //        string argfname21 = ScenarioPath + @"Data\ext_animation.txt";
            //        EADList.Load(argfname21);
            //    }

            //    string argfname24 = ScenarioPath + @"Data\item.txt";
            //    if (GeneralLib.FileExists(argfname24))
            //    {
            //        string argfname23 = ScenarioPath + @"Data\item.txt";
            //        IDList.Load(argfname23);
            //    }

            //    GUI.DisplayLoadingProgress();
            //    IsLocalDataLoaded = true;
            //    FileSystem.Input(SaveDataFileNumber, fname2);
            //    FileSystem.Input(SaveDataFileNumber, TotalTurn);
            //    FileSystem.Input(SaveDataFileNumber, Money);
            //    FileSystem.Input(SaveDataFileNumber, num); // パーツ用のダミー
            //    Event_Renamed.LoadGlobalVariables();
            //    string argvname1 = "次ステージ";
            //    if (!Expression.IsGlobalVariableDefined(argvname1))
            //    {
            //        string argvname = "次ステージ";
            //        Expression.DefineGlobalVariable(argvname);
            //    }

            //    string argvname2 = "次ステージ";
            //    Expression.SetVariableAsString(argvname2, fname2);
            //    PList.Load();
            //    UList.Load();
            //    IList.Load();
            //    FileSystem.FileClose(SaveDataFileNumber);

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
            //    string argfname25 = "";
            //    string argload_mode = "";
            //    Event_Renamed.LoadEventData(argfname25, load_mode: argload_mode);
            //    GUI.DisplayLoadingProgress();
            //    return;
            //ErrorHandler:
            //    ;
            //    string argmsg = "ロード中にエラーが発生しました";
            //    GUI.ErrorMessage(argmsg);
            //    FileSystem.FileClose(SaveDataFileNumber);
            //    TerminateSRC();
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
            //    string argmsg = "セーブ中にエラーが発生しました";
            //    GUI.ErrorMessage(argmsg);
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
            //        string argoname = "デバッグ";
            //        if (Expression.IsOptionDefined(argoname))
            //        {
            //            string argload_mode = "クイックロード";
            //            Event_Renamed.LoadEventData(ScenarioFileName, argload_mode);
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

            //        string argfname1 = ScenarioPath + @"Data\alias.txt";
            //        if (GeneralLib.FileExists(argfname1))
            //        {
            //            string argfname = ScenarioPath + @"Data\alias.txt";
            //            ALDList.Load(argfname);
            //        }

            //        bool localFileExists() { string argfname = ScenarioPath + @"Data\mind.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

            //        string argfname4 = ScenarioPath + @"Data\sp.txt";
            //        if (GeneralLib.FileExists(argfname4))
            //        {
            //            string argfname2 = ScenarioPath + @"Data\sp.txt";
            //            SPDList.Load(argfname2);
            //        }
            //        else if (localFileExists())
            //        {
            //            string argfname3 = ScenarioPath + @"Data\mind.txt";
            //            SPDList.Load(argfname3);
            //        }

            //        string argfname6 = ScenarioPath + @"Data\pilot.txt";
            //        if (GeneralLib.FileExists(argfname6))
            //        {
            //            string argfname5 = ScenarioPath + @"Data\pilot.txt";
            //            PDList.Load(argfname5);
            //        }

            //        string argfname8 = ScenarioPath + @"Data\non_pilot.txt";
            //        if (GeneralLib.FileExists(argfname8))
            //        {
            //            string argfname7 = ScenarioPath + @"Data\non_pilot.txt";
            //            NPDList.Load(argfname7);
            //        }

            //        string argfname10 = ScenarioPath + @"Data\robot.txt";
            //        if (GeneralLib.FileExists(argfname10))
            //        {
            //            string argfname9 = ScenarioPath + @"Data\robot.txt";
            //            UDList.Load(argfname9);
            //        }

            //        string argfname12 = ScenarioPath + @"Data\unit.txt";
            //        if (GeneralLib.FileExists(argfname12))
            //        {
            //            string argfname11 = ScenarioPath + @"Data\unit.txt";
            //            UDList.Load(argfname11);
            //        }

            //        GUI.DisplayLoadingProgress();
            //        string argfname14 = ScenarioPath + @"Data\pilot_message.txt";
            //        if (GeneralLib.FileExists(argfname14))
            //        {
            //            string argfname13 = ScenarioPath + @"Data\pilot_message.txt";
            //            MDList.Load(argfname13);
            //        }

            //        string argfname16 = ScenarioPath + @"Data\pilot_dialog.txt";
            //        if (GeneralLib.FileExists(argfname16))
            //        {
            //            string argfname15 = ScenarioPath + @"Data\pilot_dialog.txt";
            //            DDList.Load(argfname15);
            //        }

            //        string argfname18 = ScenarioPath + @"Data\effect.txt";
            //        if (GeneralLib.FileExists(argfname18))
            //        {
            //            string argfname17 = ScenarioPath + @"Data\effect.txt";
            //            EDList.Load(argfname17);
            //        }

            //        string argfname20 = ScenarioPath + @"Data\animation.txt";
            //        if (GeneralLib.FileExists(argfname20))
            //        {
            //            string argfname19 = ScenarioPath + @"Data\animation.txt";
            //            ADList.Load(argfname19);
            //        }

            //        string argfname22 = ScenarioPath + @"Data\ext_animation.txt";
            //        if (GeneralLib.FileExists(argfname22))
            //        {
            //            string argfname21 = ScenarioPath + @"Data\ext_animation.txt";
            //            EADList.Load(argfname21);
            //        }

            //        string argfname24 = ScenarioPath + @"Data\item.txt";
            //        if (GeneralLib.FileExists(argfname24))
            //        {
            //            string argfname23 = ScenarioPath + @"Data\item.txt";
            //            IDList.Load(argfname23);
            //        }

            //        GUI.DisplayLoadingProgress();
            //        IsLocalDataLoaded = true;
            //        string argload_mode1 = "リストア";
            //        Event_Renamed.LoadEventData(ScenarioFileName, argload_mode1);
            //        GUI.DisplayLoadingProgress();
            //    }
            //    else
            //    {
            //        var loopTo1 = num;
            //        for (i = 1; i <= loopTo1; i++)
            //            dummy = FileSystem.LineInput(SaveDataFileNumber);
            //        if (scenario_file_is_different)
            //        {
            //            string argload_mode2 = "リストア";
            //            Event_Renamed.LoadEventData(ScenarioFileName, argload_mode2);
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
            //    string argmidi_name = "(" + fname2 + ")";
            //    fname2 = Sound.SearchMidiFile(argmidi_name);
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
            //    string argoname1 = "デバッグ";
            //    string argoname2 = "乱数系列非保存";
            //    if (!Expression.IsOptionDefined(argoname1) & !Expression.IsOptionDefined(argoname2) & !FileSystem.EOF(SaveDataFileNumber))
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
            //        string argdraw_option = "非同期";
            //        int argfilter_color = 0;
            //        double argfilter_trans_par = 0d;
            //        GUI.SetupBackground(Map.MapDrawMode, argdraw_option, filter_color: argfilter_color, filter_trans_par: argfilter_trans_par);
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
            //            string arguparty = "味方";
            //            StartTurn(arguparty);
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
            //    string argmsg = "ロード中にエラーが発生しました";
            //    GUI.ErrorMessage(argmsg);
            //    FileSystem.FileClose(SaveDataFileNumber);
            //    TerminateSRC();
        }
    }
}
