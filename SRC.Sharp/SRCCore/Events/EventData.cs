using SRC.Core.CmdDatas;
using SRC.Core.Expressions;
using SRC.Core.Lib;
using SRC.Core.Maps;
using SRC.Core.VB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace SRC.Core.Events
{
    public partial class Event
    {
        // TODO 全般に1オフセットをどうするのかどっかで考えないといけない。
        // イベントデータを初期化
        public void InitEventData()
        {
            SRC.Titles = new List<string>();
            EventData = new List<EventDataLine>();
            EventFileNames = new List<string>();
            EventCmd = new List<CmdData>();
            EventQue = new Queue<string>();

            // 本体側のシナリオデータをチェックする
            LoadEventData("", "システム");
        }

        // イベントファイルのロード
        public void LoadEventData(string fname, string load_mode = "")
        {
            string buf, buf2;
            var new_titles = new List<string>();
            int i, num;
            int j;
            var CmdStack = new CmdType[51];
            int CmdStackIdx;
            var CmdPosStack = new int[51];
            int CmdPosStackIdx;
            var error_found = default(bool);
            int sys_event_data_size = default;
            int sys_event_file_num = default;

            // データの初期化
            EventData = EventData.Where(x => x.IsSystemData).ToList();
            EventFileNames = EventData.Where(x => x.IsSystemData).Select(x => x.File).ToList();
            AdditionalEventFileNames = new List<string>();
            CurrentLineNum = SysEventDataSize;
            CallDepth = 0;
            CallStack.Clear();
            ArgIndex = 0;
            ArgIndexStack.Clear();
            ArgStack.Clear();
            UpVarLevel = 0;
            UpVarLevelStack.Clear();
            VarIndex = 0;
            VarIndexStack.Clear();
            VarStack.Clear();
            ForIndex = 0;
            ForIndexStack.Clear();
            ForLimitStack.Clear();

            HotPointList = new List<HotPoint>();
            ObjColor = Color.White;
            ObjFillColor = Color.White;
            // XXX マッピング先が微妙。実装見て見直す。
            // UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            ObjFillStyle = HatchStyle.Min;
            ObjDrawWidth = 1;
            ObjDrawOption = "";

            // ラベルの初期化
            colNormalLabelList.Clear();
            var systemLabels = colEventLabelList.Values.Take(SysEventDataSize + 1).ToList();
            colEventLabelList.Clear();
            systemLabels.ForEach(x =>
            {
                colEventLabelList.Add(x);
            });

            // デバッグモードの設定
            // TODO Impl
            //string argini_section = "Option";
            //string argini_entry = "DebugMode";
            //if (Strings.LCase(GeneralLib.ReadIni(argini_section, argini_entry)) == "on")
            //{
            //    string argoname = "デバッグ";
            //    if (!Expression.IsOptionDefined(argoname))
            //    {
            //        string argvname = "Option(デバッグ)";
            //        Expression.DefineGlobalVariable(argvname);
            //    }

            //    string argvname1 = "Option(デバッグ)";
            //    Expression.SetVariableAsLong(argvname1, 1);
            //}

            // システム側のイベントデータのロード
            if (load_mode == "システム")
            {
                // 本体側のシステムデータをチェック

                // スペシャルパワーアニメ用インクルードファイルをダウンロード
                bool spAnimeIncludeLoaded =
                    LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath2, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(Path.Combine(SRC.AppPath, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath, "Lib", "精神コマンド.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath2, "Lib", "精神コマンド.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(Path.Combine(SRC.AppPath, "Lib", "精神コマンド.eve"), EventDataSource.System);

                // 汎用戦闘アニメ用インクルードファイルをダウンロード
                // TODO Impl
                //string argini_section1 = "Option";
                //string argini_entry1 = "BattleAnimation";
                //if (Strings.LCase(GeneralLib.ReadIni(argini_section1, argini_entry1)) != "off")
                //{
                //    SRC.BattleAnimation = true;
                //}

                bool battleAnimeIncludeLoaded =
                    LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath2, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(Path.Combine(SRC.AppPath, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System);
                if (!battleAnimeIncludeLoaded)
                {
                    // 戦闘アニメ表示切り替えコマンドを非表示に
                    SRC.BattleAnimation = false;
                }

                // システム側のイベントデータの総行数＆ファイル数を記録しておく
                // XXX 要らんのでは
                sys_event_data_size = EventData.Count;
                sys_event_file_num = EventFileNames.Count;
            }
            else if (!ScenarioLibChecked)
            {
                // シナリオ側のシステムデータをチェック
                ScenarioLibChecked = true;

                // XXX この辺がある時だけ再ロードするようにする
                //bool localFileExists17() { string argfname = SRC.ScenarioPath + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }
                //bool localFileExists18() { string argfname = SRC.ScenarioPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }
                //bool localFileExists19() { string argfname = SRC.ScenarioPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }
                var hasScenarioSystemData = true;
                if (hasScenarioSystemData)
                {
                    // システムデータのロードをやり直す
                    EventData.Clear();
                    EventFileNames.Clear();
                    CurrentLineNum = 0;
                    SysEventDataSize = 0;
                    SysEventFileNum = 0;
                    colSysNormalLabelList.Clear();
                    colNormalLabelList.Clear();
                    colEventLabelList.Clear();

                    // スペシャルパワーアニメ用インクルードファイルをダウンロード
                    bool spAnimeIncludeLoaded =
                        LoadEventData2IfExist(Path.Combine(SRC.ScenarioPath, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(Path.Combine(SRC.ScenarioPath, "Lib", "精神コマンド.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath2, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(Path.Combine(SRC.AppPath, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath, "Lib", "精神コマンド.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath2, "Lib", "精神コマンド.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(Path.Combine(SRC.AppPath, "Lib", "精神コマンド.eve"), EventDataSource.System);

                    // 汎用戦闘アニメ用インクルードファイルをダウンロード
                    // TODO Impl
                    //string argini_section2 = "Option";
                    //string argini_entry2 = "BattleAnimation";
                    //if (Strings.LCase(GeneralLib.ReadIni(argini_section2, argini_entry2)) != "off")
                    //{
                    //    SRC.BattleAnimation = true;
                    //}


                    bool battleAnimeIncludeLoaded =
                        LoadEventData2IfExist(Path.Combine(SRC.ScenarioPath, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath2, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(Path.Combine(SRC.AppPath, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System);
                    if (!battleAnimeIncludeLoaded)
                    {
                        // 戦闘アニメ表示切り替えコマンドを非表示に
                        SRC.BattleAnimation = false;
                    }
                }

                // シナリオ添付の汎用インクルードファイルをダウンロード
                LoadEventData2IfExist(Path.Combine(SRC.ScenarioPath, "Lib", "include.eve"), EventDataSource.System);

                // システム側のイベントデータの総行数＆ファイル数を記録しておく
                // XXX 要らんのでは
                sys_event_data_size = EventData.Count;
                sys_event_file_num = EventFileNames.Count;

                // シナリオ側のイベントデータのロード
                LoadEventData2(fname, EventDataSource.Scenario);
            }
            else
            {
                // シナリオ側のイベントデータのロード
                LoadEventData2(fname, EventDataSource.Scenario);
            }

            // データ読みこみ指定
            foreach (var line in EventData.Where(x => !x.IsSystemData))
            {
                if (Strings.Left(line.Data, 1) == "@")
                {
                    var tname = Strings.Mid(line.Data, 2);

                    // 既にそのデータが読み込まれているかチェック
                    if (!SRC.Titles.Contains(tname))
                    {
                        // フォルダを検索
                        var tfolder = SRC.SearchDataFolder(tname);
                        if (Strings.Len(tfolder) == 0)
                        {
                            DisplayEventErrorMessage(line.ID, "データ「" + tname + "」のフォルダが見つかりません");
                        }
                        else
                        {
                            new_titles.Add(tname);
                            SRC.Titles.Add(tname);
                        }
                    }
                }
            }

            // 各作品データのinclude.eveを読み込む
            if (load_mode != "システム")
            {
                // 作品毎のインクルードファイル
                foreach (var title in SRC.Titles)
                {
                    var tfolder = SRC.SearchDataFolder(title);
                    LoadEventData2IfExist(Path.Combine(tfolder, "include.eve"), EventDataSource.Scenario);
                }
                // 汎用Dataインクルードファイルをロード
                LoadEventData2IfExist(Path.Combine(SRC.ScenarioPath, "Data", "include.eve"), EventDataSource.Scenario);
                LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath, "Data", "include.eve"), EventDataSource.Scenario);
                LoadEventData2IfExist(Path.Combine(SRC.ExtDataPath2, "Data", "include.eve"), EventDataSource.Scenario);
                LoadEventData2IfExist(Path.Combine(SRC.AppPath, "Data", "include.eve"), EventDataSource.Scenario);

            }

            // XXX 多分要らん
            //// 複数行に分割されたコマンドを結合
            //var loopTo8 = Information.UBound(EventData) - 1;
            //for (i = SysEventDataSize + 1; i <= loopTo8; i++)
            //{
            //    if (Strings.Right(EventData[i], 1) == "_")
            //    {
            //        EventData[i + 1] = Strings.Left(EventData[i], Strings.Len(EventData[i]) - 1) + EventData[i + 1];
            //        EventData[i] = " ";
            //    }
            //}

            // ラベルの登録
            foreach (var line in EventData)
            {
                buf = line.Data;
                switch (Strings.Right(buf, 1) ?? "")
                {
                    case ":":
                        {
                            string labelName = Strings.Left(buf, Strings.Len(buf) - 1);
                            if (line.IsSystemData)
                            {
                                AddSysLabel(labelName, line.ID);
                            }
                            else
                            {
                                AddLabel(labelName, line.ID);

                            }
                            break;
                        }

                    case "：":
                        {
                            DisplayEventErrorMessage(CurrentLineNum, "ラベルの末尾が全角文字になっています");
                            error_found = true;
                            break;
                        }
                }
            }

            // XXX 多分要らん
            //// コマンドデータ配列を設定
            //if (Information.UBound(EventData) > Information.UBound(EventCmd))
            //{
            //    num = Information.UBound(EventCmd);
            //    Array.Resize(EventCmd, Information.UBound(EventData) + 1);
            //    var loopTo13 = Information.UBound(EventCmd);
            //    for (i = num + 1; i <= loopTo13; i++)
            //    {
            //        EventCmd[i] = new CmdData();
            //        EventCmd[i].LineNum = i;
            //    }
            //}

            // 書式チェックはシナリオ側にのみ実施
            if (load_mode != "システム")
            {

                // 構文解析と書式チェックその１
                // 制御構造
                CmdStackIdx = 0;
                CmdPosStackIdx = 0;
                var loopTo14 = Information.UBound(EventData);
                for (CurrentLineNum = SysEventDataSize + 1; CurrentLineNum <= loopTo14; CurrentLineNum++)
                {
                    if (EventCmd[CurrentLineNum] is null)
                    {
                        EventCmd[CurrentLineNum] = new CmdData();
                        EventCmd[CurrentLineNum].LineNum = CurrentLineNum;
                    }

                    {
                        var withBlock5 = EventCmd[CurrentLineNum];
                        // コマンドの構文解析
                        if (!withBlock5.Parse(EventData[CurrentLineNum]))
                        {
                            error_found = true;
                        }

                        // リスト長がマイナスのときは括弧の対応が取れていない
                        if (withBlock5.ArgNum == -1)
                        {
                            switch (CmdStack[CmdStackIdx])
                            {
                                // これらのコマンドの入力の場合は無視する
                                case CmdType.AskCmd:
                                case CmdType.AutoTalkCmd:
                                case CmdType.QuestionCmd:
                                case CmdType.TalkCmd:
                                    {
                                        break;
                                    }

                                default:
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "括弧の対応が取れていません");
                                        error_found = true;
                                        break;
                                    }
                            }
                        }

                        // コマンドに応じて制御構造をチェック
                        switch (withBlock5.Name)
                        {
                            case CmdType.IfCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (withBlock5.GetArg(4) == "then")
                                    {
                                        CmdStackIdx = (CmdStackIdx + 1);
                                        CmdPosStackIdx = (CmdPosStackIdx + 1);
                                        CmdStack[CmdStackIdx] = CmdType.IfCmd;
                                        CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    }

                                    break;
                                }

                            case CmdType.ElseIfCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] != CmdType.IfCmd)
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "ElseIfに対応するIfがありません");
                                        error_found = true;
                                        CmdStackIdx = (CmdStackIdx + 1);
                                        CmdPosStackIdx = (CmdPosStackIdx + 1);
                                        CmdStack[CmdStackIdx] = CmdType.IfCmd;
                                        CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    }

                                    break;
                                }

                            case CmdType.ElseCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "Elseに対応するIfがありません");
                                        error_found = true;
                                        CmdStackIdx = (CmdStackIdx + 1);
                                        CmdPosStackIdx = (CmdPosStackIdx + 1);
                                        CmdStack[CmdStackIdx] = CmdType.IfCmd;
                                        CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    }

                                    break;
                                }

                            case CmdType.EndIfCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] == CmdType.IfCmd)
                                    {
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                    }
                                    else
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "EndIfに対応するIfがありません");
                                        error_found = true;
                                    }

                                    break;
                                }

                            case CmdType.DoCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    CmdStackIdx = (CmdStackIdx + 1);
                                    CmdPosStackIdx = (CmdPosStackIdx + 1);
                                    CmdStack[CmdStackIdx] = CmdType.DoCmd;
                                    CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    break;
                                }

                            case CmdType.LoopCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] == CmdType.DoCmd)
                                    {
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                    }
                                    else
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "Loopに対応するDoがありません");
                                        error_found = true;
                                    }

                                    break;
                                }

                            case CmdType.ForCmd:
                            case CmdType.ForEachCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    CmdStackIdx = (CmdStackIdx + 1);
                                    CmdPosStackIdx = (CmdPosStackIdx + 1);
                                    CmdStack[CmdStackIdx] = withBlock5.Name;
                                    CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    break;
                                }

                            case CmdType.NextCmd:
                                {
                                    if (withBlock5.ArgNum == 1 | withBlock5.ArgNum == 2)
                                    {
                                        if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                        {
                                            num = CmdPosStack[CmdPosStackIdx];
                                            DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                            CmdStackIdx = (CmdStackIdx - 1);
                                            CmdPosStackIdx = (CmdPosStackIdx - 1);
                                            error_found = true;
                                        }

                                        switch (CmdStack[CmdStackIdx])
                                        {
                                            case CmdType.ForCmd:
                                            case CmdType.ForEachCmd:
                                                {
                                                    CmdStackIdx = (CmdStackIdx - 1);
                                                    CmdPosStackIdx = (CmdPosStackIdx - 1);
                                                    break;
                                                }

                                            default:
                                                {
                                                    DisplayEventErrorMessage(CurrentLineNum, "Nextに対応するコマンドがありません");
                                                    error_found = true;
                                                    break;
                                                }
                                        }
                                    }
                                    else if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        switch (CmdStack[CmdStackIdx])
                                        {
                                            case CmdType.ForCmd:
                                            case CmdType.ForEachCmd:
                                                {
                                                    CmdStackIdx = (CmdStackIdx - 1);
                                                    CmdPosStackIdx = (CmdPosStackIdx - 1);
                                                    break;
                                                }

                                            default:
                                                {
                                                    DisplayEventErrorMessage(CurrentLineNum, "Nextに対応するコマンドがありません");
                                                    error_found = true;
                                                    break;
                                                }
                                        }
                                    }

                                    break;
                                }

                            case CmdType.SwitchCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        error_found = true;
                                    }

                                    CmdStackIdx = (CmdStackIdx + 1);
                                    CmdPosStackIdx = (CmdPosStackIdx + 1);
                                    CmdStack[CmdStackIdx] = CmdType.SwitchCmd;
                                    CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    break;
                                }

                            case CmdType.CaseCmd:
                            case CmdType.CaseElseCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] != CmdType.SwitchCmd)
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "Caseに対応するSwitchがありません");
                                        error_found = true;
                                        CmdStackIdx = (CmdStackIdx + 1);
                                        CmdPosStackIdx = (CmdPosStackIdx + 1);
                                        CmdStack[CmdStackIdx] = CmdType.SwitchCmd;
                                        CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    }

                                    break;
                                }

                            case CmdType.EndSwCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] == CmdType.SwitchCmd)
                                    {
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                    }
                                    else
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "EndSwに対応するSwitchがありません");
                                        error_found = true;
                                    }

                                    break;
                                }

                            case CmdType.TalkCmd:
                            case CmdType.AutoTalkCmd:
                                {
                                    if (CmdStack[CmdStackIdx] != withBlock5.Name)
                                    {
                                        CmdStackIdx = (CmdStackIdx + 1);
                                        CmdPosStackIdx = (CmdPosStackIdx + 1);
                                        CmdStack[CmdStackIdx] = withBlock5.Name;
                                        CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    }

                                    break;
                                }

                            case CmdType.AskCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    i = withBlock5.ArgNum;
                                    while (i > 1)
                                    {
                                        switch (withBlock5.GetArg(i) ?? "")
                                        {
                                            case "通常":
                                                {
                                                    break;
                                                }

                                            case "拡大":
                                                {
                                                    break;
                                                }

                                            case "連続表示":
                                                {
                                                    break;
                                                }

                                            case "キャンセル可":
                                                {
                                                    break;
                                                }

                                            case "終了":
                                                {
                                                    i = 3;
                                                    break;
                                                }

                                            default:
                                                {
                                                    break;
                                                }
                                        }

                                        i = i - 1;
                                    }

                                    if (i < 3)
                                    {
                                        CmdStackIdx = (CmdStackIdx + 1);
                                        CmdPosStackIdx = (CmdPosStackIdx + 1);
                                        CmdStack[CmdStackIdx] = CmdType.AskCmd;
                                        CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    }

                                    break;
                                }

                            case CmdType.QuestionCmd:
                                {
                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        num = CmdPosStack[CmdPosStackIdx];
                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                        CmdStackIdx = (CmdStackIdx - 1);
                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    i = withBlock5.ArgNum;
                                    while (i > 1)
                                    {
                                        switch (withBlock5.GetArg(withBlock5.ArgNum) ?? "")
                                        {
                                            case "通常":
                                                {
                                                    break;
                                                }

                                            case "拡大":
                                                {
                                                    break;
                                                }

                                            case "連続表示":
                                                {
                                                    break;
                                                }

                                            case "キャンセル可":
                                                {
                                                    break;
                                                }

                                            case "終了":
                                                {
                                                    i = 4;
                                                    break;
                                                }

                                            default:
                                                {
                                                    break;
                                                }
                                        }

                                        i = i - 1;
                                    }

                                    if (i < 4)
                                    {
                                        CmdStackIdx = (CmdStackIdx + 1);
                                        CmdPosStackIdx = (CmdPosStackIdx + 1);
                                        CmdStack[CmdStackIdx] = CmdType.QuestionCmd;
                                        CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    }

                                    break;
                                }

                            case CmdType.EndCmd:
                                {
                                    switch (CmdStack[CmdStackIdx])
                                    {
                                        case CmdType.TalkCmd:
                                        case CmdType.AutoTalkCmd:
                                        case CmdType.AskCmd:
                                        case CmdType.QuestionCmd:
                                            {
                                                CmdStackIdx = (CmdStackIdx - 1);
                                                CmdPosStackIdx = (CmdPosStackIdx - 1);
                                                break;
                                            }

                                        default:
                                            {
                                                DisplayEventErrorMessage(CurrentLineNum, "Endに対応するTalkがありません");
                                                error_found = true;
                                                break;
                                            }
                                    }

                                    break;
                                }

                            case CmdType.SuspendCmd:
                                {
                                    switch (CmdStack[CmdStackIdx])
                                    {
                                        case CmdType.TalkCmd:
                                        case CmdType.AutoTalkCmd:
                                            {
                                                CmdStackIdx = (CmdStackIdx - 1);
                                                CmdPosStackIdx = (CmdPosStackIdx - 1);
                                                break;
                                            }

                                        default:
                                            {
                                                DisplayEventErrorMessage(CurrentLineNum, "Suspendに対応するTalkがありません");
                                                error_found = true;
                                                break;
                                            }
                                    }

                                    break;
                                }

                            case CmdType.ExitCmd:
                            case CmdType.PlaySoundCmd:
                            case CmdType.WaitCmd:
                                {
                                    switch (CmdStack[CmdStackIdx])
                                    {
                                        case CmdType.TalkCmd:
                                        case CmdType.AutoTalkCmd:
                                        case CmdType.AskCmd:
                                        case CmdType.QuestionCmd:
                                            {
                                                num = CmdPosStack[CmdPosStackIdx];
                                                DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                                CmdStackIdx = (CmdStackIdx - 1);
                                                CmdPosStackIdx = (CmdPosStackIdx - 1);
                                                error_found = true;
                                                break;
                                            }
                                    }

                                    break;
                                }

                            case CmdType.NopCmd:
                                {
                                    if (EventData[CurrentLineNum] == " ")
                                    {
                                        // "_"で消去された行。Talk中の改行に対応するためのダミーの空白
                                        EventData[CurrentLineNum] = "";
                                    }
                                    else
                                    {
                                        switch (CmdStack[CmdStackIdx])
                                        {
                                            case CmdType.TalkCmd:
                                            case CmdType.AutoTalkCmd:
                                            case CmdType.AskCmd:
                                            case CmdType.QuestionCmd:
                                                {
                                                    if (CurrentLineNum == Information.UBound(EventData))
                                                    {
                                                        num = CmdPosStack[CmdPosStackIdx];
                                                        DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                                        CmdStackIdx = (CmdStackIdx - 1);
                                                        CmdPosStackIdx = (CmdPosStackIdx - 1);
                                                        error_found = true;
                                                    }
                                                    else
                                                    {
                                                        buf = Strings.LCase(GeneralLib.ListIndex(EventData[CurrentLineNum + 1], 1));
                                                        switch (CmdStack[CmdStackIdx])
                                                        {
                                                            case CmdType.TalkCmd:
                                                                {
                                                                    buf2 = "talk";
                                                                    break;
                                                                }

                                                            case CmdType.AutoTalkCmd:
                                                                {
                                                                    buf2 = "autotalk";
                                                                    break;
                                                                }

                                                            case CmdType.AskCmd:
                                                                {
                                                                    buf2 = "ask";
                                                                    break;
                                                                }

                                                            case CmdType.QuestionCmd:
                                                                {
                                                                    buf2 = "question";
                                                                    break;
                                                                }

                                                            default:
                                                                {
                                                                    buf2 = "";
                                                                    break;
                                                                }
                                                        }
                                                        // UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
                                                        // UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
                                                        if ((buf ?? "") != (buf2 ?? "") & buf != "end" & buf != "suspend" & Strings.Len(buf) == LenB(Strings.StrConv(buf, vbFromUnicode)))
                                                        {
                                                            num = CmdPosStack[CmdPosStackIdx];
                                                            DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                                            CmdStackIdx = (CmdStackIdx - 1);
                                                            CmdPosStackIdx = (CmdPosStackIdx - 1);
                                                            error_found = true;
                                                        }
                                                    }

                                                    break;
                                                }
                                        }
                                    }

                                    break;
                                }
                        }
                    }
                }

                // ファイルの末尾まで読んでもコマンドの終わりがなかった？
                if (CmdStackIdx > 0)
                {
                    num = CmdPosStack[CmdPosStackIdx];
                    switch (CmdStack[CmdStackIdx])
                    {
                        case CmdType.AskCmd:
                            {
                                DisplayEventErrorMessage(num, "Askに対応するEndがありません");
                                break;
                            }

                        case CmdType.AutoTalkCmd:
                            {
                                DisplayEventErrorMessage(num, "AutoTalkに対応するEndがありません");
                                break;
                            }

                        case CmdType.DoCmd:
                            {
                                DisplayEventErrorMessage(num, "Doに対応するLoopがありません");
                                break;
                            }

                        case CmdType.ForCmd:
                            {
                                DisplayEventErrorMessage(num, "Forに対応するNextがありません");
                                break;
                            }

                        case CmdType.ForEachCmd:
                            {
                                DisplayEventErrorMessage(num, "ForEachに対応するNextがありません");
                                break;
                            }

                        case CmdType.IfCmd:
                            {
                                DisplayEventErrorMessage(num, "Ifに対応するEndIfがありません");
                                break;
                            }

                        case CmdType.QuestionCmd:
                            {
                                DisplayEventErrorMessage(num, "Questionに対応するEndがありません");
                                break;
                            }

                        case CmdType.SwitchCmd:
                            {
                                DisplayEventErrorMessage(num, "Switchに対応するEndSwがありません");
                                break;
                            }

                        case CmdType.TalkCmd:
                            {
                                DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                break;
                            }
                    }

                    error_found = true;
                }

                // 書式エラーが見つかった場合はSRCを終了
                if (error_found)
                {
                    SRC.TerminateSRC();
                }

                // 書式チェックその２
                // 主なコマンドの引数の数をチェック
                var loopTo15 = Information.UBound(EventData);
                for (CurrentLineNum = SysEventDataSize + 1; CurrentLineNum <= loopTo15; CurrentLineNum++)
                {
                    {
                        var withBlock6 = EventCmd[CurrentLineNum];
                        switch (withBlock6.Name)
                        {
                            case CmdType.CreateCmd:
                                {
                                    if (withBlock6.ArgNum < 8)
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "Createコマンドのパラメータ数が違います");
                                        error_found = true;
                                    }

                                    break;
                                }

                            case CmdType.PilotCmd:
                                {
                                    if (withBlock6.ArgNum < 3)
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "Pilotコマンドのパラメータ数が違います");
                                        error_found = true;
                                    }

                                    break;
                                }

                            case CmdType.UnitCmd:
                                {
                                    if (withBlock6.ArgNum != 3)
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "Unitコマンドのパラメータ数が違います");
                                        error_found = true;
                                    }

                                    break;
                                }
                        }
                    }
                }

                // 書式エラーが見つかった場合はSRCを終了
                if (error_found)
                {
                    SRC.TerminateSRC();
                }
            }

            // シナリオ側のイベントデータの場合はここまでスキップ

            // システム側のイベントデータの場合の処理

            // XXX 多分要らん
            //// CmdDataクラスのインスタンスの生成のみ行っておく
            //else if (CurrentLineNum > Information.UBound(EventCmd))
            //{
            //    Array.Resize(EventCmd, CurrentLineNum + 1);
            //    i = CurrentLineNum;
            //    while (EventCmd[i] is null)
            //    {
            //        EventCmd[i] = new CmdData();
            //        EventCmd[i].LineNum = i;
            //        i = i - 1;
            //    }
            //}

            // イベントデータの読み込みが終了したのでシステム側イベントデータのサイズを決定。
            // システム側イベントデータは読み込みを一度だけやればよい。
            if (sys_event_data_size > 0)
            {
                SysEventDataSize = sys_event_data_size;
                SysEventFileNum = sys_event_file_num;
            }

            // クイックロードやリスタートの場合はシナリオデータの再ロードのみ
            switch (load_mode ?? "")
            {
                case "リストア":
                    {
                        // TODO Impl
                        //SRC.ADList.AddDefaultAnimation();
                        return;
                    }

                case "システム":
                case "クイックロード":
                case "リスタート":
                    {
                        return;
                    }
            }

            // 追加されたシステム側イベントデータをチェックする場合はここで終了
            if (string.IsNullOrEmpty(fname))
            {
                return;
            }

            LoadData(fname, new_titles);
        }

        private void LoadData(string fname, IList<string> new_titles)
        {

            // ロードするデータ数をカウント
            var progressMax = new_titles.Count;
            if (SRC.IsLocalDataLoaded)
            {
                if (progressMax > 0)
                {
                    progressMax = progressMax + 2;
                }
            }
            else
            {
                progressMax = progressMax + 2;
            }

            string mapFileName = Strings.Left(fname, Strings.Len(fname) - 4) + ".map";
            if (GeneralLib.FileExists(mapFileName))
            {
                progressMax = progressMax + 1;
            }

            if (progressMax == 0 && SRC.IsLocalDataLoaded)
            {
                // デフォルトの戦闘アニメデータを設定
                SRC.ADList.AddDefaultAnimation();
                return;
            }

            // ロード画面を表示
            GUI.OpenNowLoadingForm();

            // ロードサイズを設定
            GUI.SetLoadImageSize(progressMax);

            // 使用しているタイトルのデータをロード
            foreach (var title in new_titles)
            {
                SRC.IncludeData(title);
            }

            // ローカルデータの読みこみ
            if (!SRC.IsLocalDataLoaded || new_titles.Any())
            {
                string argfname36 = SRC.ScenarioPath + @"Data\alias.txt";
                if (GeneralLib.FileExists(argfname36))
                {
                    string argfname35 = SRC.ScenarioPath + @"Data\alias.txt";
                    SRC.ALDList.Load(argfname35);
                }

                bool localFileExists23() { string argfname = SRC.ScenarioPath + @"Data\mind.txt"; var ret = GeneralLib.FileExists(argfname); return ret; }

                string argfname39 = SRC.ScenarioPath + @"Data\sp.txt";
                if (GeneralLib.FileExists(argfname39))
                {
                    string argfname37 = SRC.ScenarioPath + @"Data\sp.txt";
                    SRC.SPDList.Load(argfname37);
                }
                else if (localFileExists23())
                {
                    string argfname38 = SRC.ScenarioPath + @"Data\mind.txt";
                    SRC.SPDList.Load(argfname38);
                }

                string argfname41 = SRC.ScenarioPath + @"Data\pilot.txt";
                if (GeneralLib.FileExists(argfname41))
                {
                    string argfname40 = SRC.ScenarioPath + @"Data\pilot.txt";
                    SRC.PDList.Load(argfname40);
                }

                string argfname43 = SRC.ScenarioPath + @"Data\non_pilot.txt";
                if (GeneralLib.FileExists(argfname43))
                {
                    string argfname42 = SRC.ScenarioPath + @"Data\non_pilot.txt";
                    SRC.NPDList.Load(argfname42);
                }

                string argfname45 = SRC.ScenarioPath + @"Data\robot.txt";
                if (GeneralLib.FileExists(argfname45))
                {
                    string argfname44 = SRC.ScenarioPath + @"Data\robot.txt";
                    SRC.UDList.Load(argfname44);
                }

                string argfname47 = SRC.ScenarioPath + @"Data\unit.txt";
                if (GeneralLib.FileExists(argfname47))
                {
                    string argfname46 = SRC.ScenarioPath + @"Data\unit.txt";
                    SRC.UDList.Load(argfname46);
                }

                GUI.DisplayLoadingProgress();
                string argfname49 = SRC.ScenarioPath + @"Data\pilot_message.txt";
                if (GeneralLib.FileExists(argfname49))
                {
                    string argfname48 = SRC.ScenarioPath + @"Data\pilot_message.txt";
                    SRC.MDList.Load(argfname48);
                }

                string argfname51 = SRC.ScenarioPath + @"Data\pilot_dialog.txt";
                if (GeneralLib.FileExists(argfname51))
                {
                    string argfname50 = SRC.ScenarioPath + @"Data\pilot_dialog.txt";
                    SRC.DDList.Load(argfname50);
                }

                string argfname53 = SRC.ScenarioPath + @"Data\effect.txt";
                if (GeneralLib.FileExists(argfname53))
                {
                    string argfname52 = SRC.ScenarioPath + @"Data\effect.txt";
                    SRC.EDList.Load(argfname52);
                }

                string argfname55 = SRC.ScenarioPath + @"Data\animation.txt";
                if (GeneralLib.FileExists(argfname55))
                {
                    string argfname54 = SRC.ScenarioPath + @"Data\animation.txt";
                    SRC.ADList.Load(argfname54);
                }

                string argfname57 = SRC.ScenarioPath + @"Data\ext_animation.txt";
                if (GeneralLib.FileExists(argfname57))
                {
                    string argfname56 = SRC.ScenarioPath + @"Data\ext_animation.txt";
                    SRC.EADList.Load(argfname56);
                }

                string argfname59 = SRC.ScenarioPath + @"Data\item.txt";
                if (GeneralLib.FileExists(argfname59))
                {
                    string argfname58 = SRC.ScenarioPath + @"Data\item.txt";
                    SRC.IDList.Load(argfname58);
                }

                GUI.DisplayLoadingProgress();
                SRC.IsLocalDataLoaded = true;
            }

            // デフォルトの戦闘アニメデータを設定
            SRC.ADList.AddDefaultAnimation();

            // マップデータをロード
            if (GeneralLib.FileExists(mapFileName))
            {
                Map.LoadMapData(mapFileName);
                string argdraw_mode = "";
                string argdraw_option = "";
                int argfilter_color = 0;
                double argfilter_trans_par = 0d;
                GUI.SetupBackground(draw_mode: argdraw_mode, draw_option: argdraw_option, filter_color: argfilter_color, filter_trans_par: argfilter_trans_par);
                GUI.RedrawScreen();
                GUI.DisplayLoadingProgress();
            }

            // ロード画面を閉じる
            GUI.CloseNowLoadingForm();
        }

        public bool LoadEventData2IfExist(string fname, EventDataSource source)
        {
            if (GeneralLib.FileExists(fname))
            {
                LoadEventData2(fname, source);
                return true;
            }
            return false;
        }

        // イベントファイルの読み込み
        public void LoadEventData2(string fname, EventDataSource source)
        {
            if (string.IsNullOrEmpty(fname))
            {
                return;
            }

            var lineNumber = -1;
            try
            {
                using (var stream = new FileStream(fname, FileMode.Open))
                using (var reader = new SrcEveReader(fname, stream))
                {
                    while (reader.HasMore)
                    {
                        var line = reader.GetLine();
                        lineNumber = reader.LineNumber;

                        var isIncludeLine = Strings.Left(line, 1) == "<" && Strings.InStr(line, ">") == Strings.Len(line) && line != "<>";
                        if (!isIncludeLine)
                        {
                            var eventLine = new EventDataLine(EventData.Count + 1, source, reader.FileName, reader.LineNumber, line);
                            EventData.Add(eventLine);
                        }
                        else
                        {
                            // 他のイベントファイルの読み込み
                            // TODO Impl
                            var fname2 = Strings.Mid(line, 2, Strings.Len(line) - 2);
                            if (fname2 != @"Lib\スペシャルパワー.eve" & fname2 != @"Lib\汎用戦闘アニメ\include.eve" & fname2 != @"Lib\include.eve")
                            {
                                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + fname2)) > 0)
                                {
                                    string argfname = SRC.ScenarioPath + fname2;
                                    LoadEventData2(argfname, source);
                                }
                                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                else if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + fname2)) > 0)
                                {
                                    string argfname1 = SRC.ExtDataPath + fname2;
                                    LoadEventData2(argfname1, source);
                                }
                                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                else if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + fname2)) > 0)
                                {
                                    string argfname2 = SRC.ExtDataPath2 + fname2;
                                    LoadEventData2(argfname2, source);
                                }
                                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                else if (Strings.Len(FileSystem.Dir(SRC.AppPath + fname2)) > 0)
                                {
                                    string argfname3 = SRC.AppPath + fname2;
                                    LoadEventData2(argfname3, source);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                // XXX
                string argmsg1 = fname + "のロード中にエラーが発生しました" + Constants.vbCr + SrcFormatter.Format(lineNumber) + "行目のイベントデータが不正です";
                GUI.ErrorMessage(argmsg1);
                SRC.TerminateSRC();
                throw;
            }
        }
    }
}
