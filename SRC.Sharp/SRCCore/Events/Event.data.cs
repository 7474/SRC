using Microsoft.Extensions.Logging;
using SRCCore.CmdDatas;
using SRCCore.CmdDatas.Commands;
using SRCCore.Extensions;
using SRCCore.Lib;
using SRCCore.Maps;
using SRCCore.VB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SRCCore.Events
{
    public partial class Event
    {
        // XXX 全般に1オフセットをどうするのかどっかで考えないといけない。
        // イベントデータを初期化
        public void InitEventData()
        {
            SRC.Titles = new List<string>();
            EventData = new List<EventDataLine>();
            EventFileNames = new List<string>();
            EventCmd = new List<CmdData>();
            EventQue = new List<string>();

            // 本体側のシナリオデータをチェックする
            LoadEventData("", "システム");
        }

        // イベントファイルのロード
        public void LoadEventData(string fname, string load_mode = "")
        {
            string buf;
            var new_titles = new List<string>();
            int sys_event_data_size = default;
            int sys_event_file_num = default;

            // データの初期化
            // XXX List値それぞれの初期化
            EventData = EventData.Where(x => x.IsSystemData).ToList();
            EventFileNames = EventData.Where(x => x.IsSystemData).Select(x => x.File).ToList();
            AdditionalEventFileNames = new List<string>();
            CurrentLineNum = SysEventDataSize;
            CallDepth = 0;
            //CallStack.Clear();
            ArgIndex = 0;
            //ArgIndexStack.Clear();
            //ArgStack.Clear();
            UpVarLevel = 0;
            //UpVarLevelStack.Clear();
            VarIndex = -1;
            //VarIndexStack.Clear();
            //VarStack.Clear();
            ForIndex = 0;
            //ForIndexStack.Clear();
            //ForLimitStack.Clear();
            CallStack[CallDepth] = 0;
            ArgIndexStack[CallDepth] = ArgIndex;
            VarIndexStack[CallDepth] = VarIndex;
            ForIndexStack[CallDepth] = ForIndex;

            HotPointList = new List<HotPoint>();
            ObjColor = Color.White;
            ObjFillColor = Color.White;
            // XXX マッピング先が微妙。実装見て見直す。
            ObjFillStyle = FillStyle.VbFSTransparent;
            ObjDrawWidth = 1;
            ObjDrawOption = "";

            // ラベルの初期化
            // 全部登録しなおしているので全部消してしまってよい
            // XXX 本当か？　手抜きが祟っているだけでは？
            colNormalLabelList.Clear();
            colEventLabelList.Clear();
            colSysNormalLabelList.Clear();

            // デバッグモードの設定
            if (SRC.SystemConfig.GetFlag("Option", "DebugMode"))
            {
                if (!Expression.IsOptionDefined("デバッグ"))
                {
                    Expression.DefineGlobalVariable("Option(デバッグ)");
                }

                Expression.SetVariableAsLong("Option(デバッグ)", 1);
            }

            // システム側のイベントデータのロード
            if (load_mode == "システム")
            {
                // 本体側のシステムデータをチェック

                // スペシャルパワーアニメ用インクルードファイルをダウンロード
                bool spAnimeIncludeLoaded =
                    LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath2, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.AppPath, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath, "Lib", "精神コマンド.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath2, "Lib", "精神コマンド.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.AppPath, "Lib", "精神コマンド.eve"), EventDataSource.System);

                // 汎用戦闘アニメ用インクルードファイルをダウンロード
                bool battleAnimeIncludeLoaded =
                    LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath2, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System)
                    || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.AppPath, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System);
                if (!battleAnimeIncludeLoaded)
                {
                    // 戦闘アニメ表示切り替えコマンドを非表示に
                    SRC.SystemConfig.BattleAnimation = false;
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
                //bool localFileExists17() { string argfname = SRC.ScenarioPath + @"Lib\スペシャルパワー.eve"; var ret = SRC.FileSystem.FileExists(argfname); return ret; }
                //bool localFileExists18() { string argfname = SRC.ScenarioPath + @"Lib\精神コマンド.eve"; var ret = SRC.FileSystem.FileExists(argfname); return ret; }
                //bool localFileExists19() { string argfname = SRC.ScenarioPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = SRC.FileSystem.FileExists(argfname); return ret; }
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
                        LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ScenarioPath, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ScenarioPath, "Lib", "精神コマンド.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath2, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.AppPath, "Lib", "スペシャルパワー.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath, "Lib", "精神コマンド.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath2, "Lib", "精神コマンド.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.AppPath, "Lib", "精神コマンド.eve"), EventDataSource.System);

                    // 汎用戦闘アニメ用インクルードファイルをダウンロード
                    bool battleAnimeIncludeLoaded =
                        LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ScenarioPath, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath2, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System)
                        || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.AppPath, "Lib", "汎用戦闘アニメ", "include.eve"), EventDataSource.System);
                    if (!battleAnimeIncludeLoaded)
                    {
                        // 戦闘アニメ表示切り替えコマンドを非表示に
                        SRC.SystemConfig.BattleAnimation = false;
                    }
                }

                // シナリオ添付の汎用インクルードファイルをダウンロード
                LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ScenarioPath, "Lib", "include.eve"), EventDataSource.System);

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
                        if (Strings.Len(tfolder) == 0 && !SkipExternalSourceLoad)
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
                    LoadEventData2IfExist(SRC.FileSystem.PathCombine(tfolder, "include.eve"), EventDataSource.Scenario);
                }
                // 汎用Dataインクルードファイルをロード
                // 最初に見つかったひとつだけを読み込む（VB: Event.bas LoadEventData の If ... ElseIf）
                // すべて読み込むと ScenarioPath と AppPath 等が同じ場合に同じファイルを重複して読み込み、
                // 常時イベントのラベルが二重に登録されて繰り返し起動してしまう
                _ = LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ScenarioPath, "Data", "include.eve"), EventDataSource.Scenario)
                    || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath, "Data", "include.eve"), EventDataSource.Scenario)
                    || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath2, "Data", "include.eve"), EventDataSource.Scenario)
                    || LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.AppPath, "Data", "include.eve"), EventDataSource.Scenario);
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
            var label_error_found = RegisterLabel();

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
                ParseCommand(label_error_found);
                ValidateCommandArgs();
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
                        SRC.ADList.AddDefaultAnimation();
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
            if (!SkipExternalSourceLoad)
            {
                LoadData(fname, new_titles);
            }
        }

        private bool RegisterLabel()
        {
            // 「登録済なら無視」の判定に、登録済みの行の索引を使う
            registeringNormalLabelIds = new LabelIdIndex(colNormalLabelList);
            registeringEventLabelIds = new LabelIdIndex(colEventLabelList);
            registeringSysNormalLabelIds = new LabelIdIndex(colSysNormalLabelList);
            try
            {
                return RegisterLabels();
            }
            finally
            {
                registeringNormalLabelIds = null;
                registeringEventLabelIds = null;
                registeringSysNormalLabelIds = null;
            }
        }

        private bool RegisterLabels()
        {
            var error_found = false;
            foreach (var line in EventData)
            {
                var lbuf = line.Data;
                switch (Strings.Right(lbuf, 1) ?? "")
                {
                    case ":":
                        {
                            string labelName = Strings.Left(lbuf, Strings.Len(lbuf) - 1);
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
                            DisplayEventErrorMessage(line.ID, "ラベルの末尾が全角文字になっています");
                            error_found = true;
                            break;
                        }
                }
            }
            return error_found;
        }

        private void ParseCommand(bool error_found = false)
        {
            // 構文解析と書式チェックその１
            // 制御構造
            var parser = new CmdParser();
            var cmdStack = new Stack<CmdType>();
            var cmdPosStack = new Stack<int>();
            // スタックが空のときは底の NullCmd を返す（VB の CmdStack(0)）
            CmdType StackTop() => cmdStack.Count > 0 ? cmdStack.Peek() : CmdType.NullCmd;
            EventCmd.Clear();
            // 全eventDataを走査してコマンドを解析する（システムデータも含めて解析することで
            // 処理の整合性を保つ; パフォーマンス改善の余地はあるが現在の規模では問題ない）
            //foreach (var eventDataLine in EventData.Where(x => !x.IsSystemData))
            foreach (var eventDataLine in EventData)
            {
                // コマンドの構文解析
                var command = parser.Parse(SRC, eventDataLine);
                EventCmd.Add(command);

                // システムデータなら無視しておく
                if (eventDataLine.IsSystemData) { continue; }

                // リスト長がマイナスのときは括弧の対応が取れていない
                if (command.ArgNum <= -1)
                {
                    switch (StackTop())
                    {
                        // これらのコマンドの入力の場合は無視する
                        case CmdType.AskCmd:
                        case CmdType.AutoTalkCmd:
                        case CmdType.QuestionCmd:
                        case CmdType.TalkCmd:
                            break;

                        default:
                            DisplayEventErrorMessage(command.EventData, "括弧の対応が取れていません");
                            error_found = true;
                            break;
                    }
                }

                // コマンドに応じて制御構造をチェック
                switch (command.Name)
                {
                    case CmdType.IfCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if ((command as AIfCmd)?.IfCmdType == IfCmdType.Then)
                            {
                                cmdStack.Push(CmdType.IfCmd);
                                cmdPosStack.Push(command.EventData.ID);
                            }

                            break;
                        }

                    case CmdType.ElseIfCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (StackTop() != CmdType.IfCmd)
                            {
                                DisplayEventErrorMessage(command.EventData, "ElseIfに対応するIfがありません");
                                error_found = true;
                                cmdStack.Push(CmdType.IfCmd);
                                cmdPosStack.Push(command.EventData.ID);
                            }

                            break;
                        }

                    case CmdType.ElseCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(command.EventData, "Elseに対応するIfがありません");
                                error_found = true;
                                cmdStack.Push(CmdType.IfCmd);
                                cmdPosStack.Push(command.EventData.ID);
                            }

                            break;
                        }

                    case CmdType.EndIfCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (StackTop() == CmdType.IfCmd)
                            {
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                            }
                            else
                            {
                                DisplayEventErrorMessage(command.EventData, "EndIfに対応するIfがありません");
                                error_found = true;
                            }

                            break;
                        }

                    case CmdType.DoCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            cmdStack.Push(CmdType.DoCmd);
                            cmdPosStack.Push(command.EventData.ID);
                            break;
                        }

                    case CmdType.LoopCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (StackTop() == CmdType.DoCmd)
                            {
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                            }
                            else
                            {
                                DisplayEventErrorMessage(command.EventData, "Loopに対応するDoがありません");
                                error_found = true;
                            }

                            break;
                        }

                    case CmdType.ForCmd:
                    case CmdType.ForEachCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            cmdStack.Push(command.Name);
                            cmdPosStack.Push(command.EventData.ID);
                            break;
                        }

                    case CmdType.NextCmd:
                        {
                            if (command.ArgNum == 1 || command.ArgNum == 2)
                            {
                                if (StackTop() == CmdType.TalkCmd)
                                {
                                    DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                    cmdStack.Pop();
                                    cmdPosStack.Pop();
                                    error_found = true;
                                }
                                switch (StackTop())
                                {
                                    case CmdType.ForCmd:
                                    case CmdType.ForEachCmd:
                                        {
                                            cmdStack.Pop();
                                            cmdPosStack.Pop();
                                            break;
                                        }

                                    default:
                                        {
                                            DisplayEventErrorMessage(command.EventData, "Nextに対応するコマンドがありません");
                                            error_found = true;
                                            break;
                                        }
                                }
                            }
                            // XXX 条件こけてる気がする
                            else if (StackTop() == CmdType.TalkCmd)
                            {
                                switch (StackTop())
                                {
                                    case CmdType.ForCmd:
                                    case CmdType.ForEachCmd:
                                        {
                                            cmdStack.Pop();
                                            cmdPosStack.Pop();
                                            break;
                                        }

                                    default:
                                        {
                                            DisplayEventErrorMessage(command.EventData, "Nextに対応するコマンドがありません");
                                            error_found = true;
                                            break;
                                        }
                                }
                            }

                            break;
                        }

                    case CmdType.SwitchCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                error_found = true;
                            }

                            cmdStack.Push(CmdType.SwitchCmd);
                            cmdPosStack.Push(command.EventData.ID);
                            break;
                        }

                    case CmdType.CaseCmd:
                    case CmdType.CaseElseCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (StackTop() != CmdType.SwitchCmd)
                            {
                                DisplayEventErrorMessage(command.EventData.ID, "Caseに対応するSwitchがありません");
                                error_found = true;
                                cmdStack.Push(CmdType.SwitchCmd);
                                cmdPosStack.Push(command.EventData.ID);
                            }

                            break;
                        }

                    case CmdType.EndSwCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (StackTop() == CmdType.SwitchCmd)
                            {
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                            }
                            else
                            {
                                DisplayEventErrorMessage(command.EventData.ID, "EndSwに対応するSwitchがありません");
                                error_found = true;
                            }

                            break;
                        }

                    case CmdType.TalkCmd:
                    case CmdType.AutoTalkCmd:
                        {
                            if (StackTop() != command.Name)
                            {
                                cmdStack.Push(command.Name);
                                cmdPosStack.Push(command.EventData.ID);
                            }

                            break;
                        }

                    case CmdType.AskCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            var opts = new string[]
                            {
                                "通常",
                                "拡大",
                                "連続表示",
                                "キャンセル可",
                            };
                            var args = command.GetArgs().Reverse().SkipWhile(x => opts.Contains(x.strArg)).ToList();
                            if (args.Count < 3
                                && !command.GetArgs().Select(x => x.strArg).Contains("終了"))
                            {
                                cmdStack.Push(CmdType.AskCmd);
                                cmdPosStack.Push(command.EventData.ID);
                            }

                            break;
                        }

                    case CmdType.QuestionCmd:
                        {
                            if (StackTop() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            // VB と同じく最後の引数がオプションかどうかを判定し、
                            // オプション以外ならその時点でループを抜ける（Exit Do）
                            var i = command.ArgNum;
                            while (i > 1)
                            {
                                var opt = command.GetArg(command.ArgNum) ?? "";
                                if (opt == "終了")
                                {
                                    i = 4;
                                    break;
                                }
                                else if (opt != "通常" && opt != "拡大" && opt != "連続表示" && opt != "キャンセル可")
                                {
                                    break;
                                }

                                i = i - 1;
                            }

                            if (i < 4)
                            {
                                cmdStack.Push(CmdType.QuestionCmd);
                                cmdPosStack.Push(command.EventData.ID);
                            }

                            break;
                        }

                    case CmdType.EndCmd:
                        {
                            switch (StackTop())
                            {
                                case CmdType.TalkCmd:
                                case CmdType.AutoTalkCmd:
                                case CmdType.AskCmd:
                                case CmdType.QuestionCmd:
                                    {
                                        cmdStack.Pop();
                                        cmdPosStack.Pop();
                                        break;
                                    }

                                default:
                                    {
                                        DisplayEventErrorMessage(command.EventData.ID, "Endに対応するTalkがありません");
                                        error_found = true;
                                        break;
                                    }
                            }
                            break;
                        }

                    case CmdType.SuspendCmd:
                        {
                            switch (StackTop())
                            {
                                case CmdType.TalkCmd:
                                case CmdType.AutoTalkCmd:
                                    {
                                        cmdStack.Pop();
                                        cmdPosStack.Pop();
                                        break;
                                    }

                                default:
                                    {
                                        DisplayEventErrorMessage(command.EventData.ID, "Suspendに対応するTalkがありません");
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
                            switch (StackTop())
                            {
                                case CmdType.TalkCmd:
                                case CmdType.AutoTalkCmd:
                                case CmdType.AskCmd:
                                case CmdType.QuestionCmd:
                                    {
                                        DisplayEventErrorMessage(cmdPosStack.Peek(), "Talk/AutoTalk/Ask/Questionに対応するEndがありません");
                                        cmdStack.Pop();
                                        cmdPosStack.Pop();
                                        error_found = true;
                                        break;
                                    }
                            }

                            break;
                        }

                    case CmdType.NopCmd:
                        {
                            if (eventDataLine.Data == " ")
                            {
                                // "_"で消去された行。Talk中の改行に対応するためのダミーの空白
                                // XXX これ消しとかないとまずいの？
                                //eventDataLine.Data = "";
                            }
                            else
                            {
                                // NopCmdがTalk/AutoTalk/Ask/Question内に現れた場合の中途検証は
                                // ファイル末尾の未終了チェック（下記 cmdStack.Count > 0 の箇所）で対応済み。
                            }

                            break;
                        }
                }
            }

            // ファイルの末尾まで読んでもコマンドの終わりがなかった？
            if (cmdStack.Count > 0)
            {
                switch (StackTop())
                {
                    case CmdType.AskCmd:
                        {
                            DisplayEventErrorMessage(cmdPosStack.Peek(), "Askに対応するEndがありません");
                            break;
                        }

                    case CmdType.AutoTalkCmd:
                        {
                            DisplayEventErrorMessage(cmdPosStack.Peek(), "AutoTalkに対応するEndがありません");
                            break;
                        }

                    case CmdType.DoCmd:
                        {
                            DisplayEventErrorMessage(cmdPosStack.Peek(), "Doに対応するLoopがありません");
                            break;
                        }

                    case CmdType.ForCmd:
                        {
                            DisplayEventErrorMessage(cmdPosStack.Peek(), "Forに対応するNextがありません");
                            break;
                        }

                    case CmdType.ForEachCmd:
                        {
                            DisplayEventErrorMessage(cmdPosStack.Peek(), "ForEachに対応するNextがありません");
                            break;
                        }

                    case CmdType.IfCmd:
                        {
                            DisplayEventErrorMessage(cmdPosStack.Peek(), "Ifに対応するEndIfがありません");
                            break;
                        }

                    case CmdType.QuestionCmd:
                        {
                            DisplayEventErrorMessage(cmdPosStack.Peek(), "Questionに対応するEndがありません");
                            break;
                        }

                    case CmdType.SwitchCmd:
                        {
                            DisplayEventErrorMessage(cmdPosStack.Peek(), "Switchに対応するEndSwがありません");
                            break;
                        }

                    case CmdType.TalkCmd:
                        {
                            DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
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
        }

        private bool ValidateCommandArgs()
        {
            bool error_found = false;

            // 書式チェックその２
            // 主なコマンドの引数の数をチェック
            foreach (var command in EventCmd)
            {
                switch (command.Name)
                {
                    case CmdType.CreateCmd:
                        {
                            if (command.ArgNum < 8)
                            {
                                DisplayEventErrorMessage(command.EventData.ID, "Createコマンドのパラメータ数が違います");
                                error_found = true;
                            }

                            break;
                        }

                    case CmdType.PilotCmd:
                        {
                            if (command.ArgNum < 3)
                            {
                                DisplayEventErrorMessage(command.EventData.ID, "Pilotコマンドのパラメータ数が違います");
                                error_found = true;
                            }

                            break;
                        }

                    case CmdType.UnitCmd:
                        {
                            if (command.ArgNum != 3)
                            {
                                DisplayEventErrorMessage(command.EventData.ID, "Unitコマンドのパラメータ数が違います");
                                error_found = true;
                            }

                            break;
                        }
                }
            }

            // 書式エラーが見つかった場合はSRCを終了
            if (error_found)
            {
                SRC.TerminateSRC();
            }

            return error_found;
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
            if (SRC.FileSystem.FileExists(mapFileName))
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
            LoadTitles(new_titles);

            // マップデータをロード
            if (SRC.FileSystem.FileExists(mapFileName))
            {
                Map.LoadMapData(mapFileName);
                GUI.SetupBackground(draw_mode: "", draw_option: "");
                GUI.RedrawScreen();
                GUI.DisplayLoadingProgress();
            }

            // ロード画面を閉じる
            GUI.CloseNowLoadingForm();
        }

        public void LoadTitles(IList<string> new_titles)
        {
            // 使用しているタイトルのデータをロード
            foreach (var title in new_titles)
            {
                SRC.IncludeData(title);
            }

            // ローカルデータの読みこみ
            if (!SRC.IsLocalDataLoaded || new_titles.Any())
            {
                SRC.LoadLocalData();
            }

            // デフォルトの戦闘アニメデータを設定
            SRC.ADList.AddDefaultAnimation();
        }

        public bool LoadEventData2IfExist(string fname, EventDataSource source)
        {
            if (SRC.FileSystem.FileExists(fname))
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

            // イベントファイル名を記録しておく（VB: Event.bas LoadEventData2）
            // Require コマンド等で既に読み込まれたファイルを再度読み込まないための判定に使う
            EventFileNames.Add(fname);

            var lineNumber = -1;
            try
            {
                using (var stream = SRC.FileSystem.OpenText(SRC.SystemConfig.SRCCompatibilityMode, fname))
                using (var reader = new SrcEveReader(fname, stream))
                {
                    while (reader.HasMore)
                    {
                        var line = reader.GetLine();
                        lineNumber = reader.LineNumber;

                        var isIncludeLine = Strings.Left(line, 1) == "<" && Strings.InStr(line, ">") == Strings.Len(line) && line != "<>";
                        if (!isIncludeLine)
                        {
                            var eventLine = new EventDataLine(EventData.Count, source, reader.FileName, reader.LineNumber, line);
                            EventData.Add(eventLine);
                        }
                        else
                        {
                            // 他のイベントファイルの読み込み
                            // '<filepath>' -> 'filepath'
                            var fname2 = Strings.Mid(line, 2, Strings.Len(line) - 2).Trim();
                            if (!SRC.FileSystem.PathEquals(fname2, @"Lib\スペシャルパワー.eve")
                                && !SRC.FileSystem.PathEquals(fname2, @"Lib\汎用戦闘アニメ\include.eve")
                                && !SRC.FileSystem.PathEquals(fname2, @"Lib\include.eve")
                                && !SRC.FileSystem.PathEquals(fname2, @"Lib\スペシャルパワー.eve"))
                            {
                                foreach (var dir in new string[]
                                {
                                    SRC.ScenarioPath,
                                    SRC.ExtDataPath,
                                    SRC.ExtDataPath2,
                                    SRC.AppPath,
                                })
                                {
                                    var path = SRC.FileSystem.PathCombine(dir, fname2);
                                    if (SRC.FileSystem.FileExists(path))
                                    {
                                        LoadEventData2(path, source);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SRC.Log.LogError(ex, "イベントデータを読み込めません {File}({Line})", fname, lineNumber);
                // XXX
                GUI.ErrorMessage(fname + "のロード中にエラーが発生しました" + Constants.vbCr + SrcFormatter.Format(lineNumber) + "行目のイベントデータが不正です");
                SRC.TerminateSRC();
                throw;
            }
        }

        // シナリオ開始後の動的なファイル読み込み
        public void LoadDynamic(string fname)
        {
            // 既に読み込まれている場合はスキップ
            if (EventFileNames.Contains(fname))
            {
                return;
            }

            // ファイルをロード
            LoadEventData2(fname, EventDataSource.Scenario);

            // ラベルを登録
            var label_error_found = RegisterLabel();

            // コマンドデータ配列を設定
            ParseCommand(label_error_found);
        }
    }
}
