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
        // イベントデータを初期化
        // SRC# の EventData・EventCmd とラベルの行番号（EventDataId）は 0 始まり。
        // VB の行番号は 1 始まりで、ラベルが見つからないときの 0 は SRC# では -1 にしている
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
            // VB の EventFileID・EventLineNum は EventDataLine が持つ。
            // EventFileNames はシステム側のファイル（SysEventFileNum 個）だけを残す
            EventData = EventData.Where(x => x.IsSystemData).ToList();
            EventFileNames = (EventFileNames ?? new List<string>()).Take(SysEventFileNum).ToList();
            AdditionalEventFileNames = new List<string>();
            CurrentLineNum = SysEventDataSize;
            // VB は各スタックの中身を消さず、添字だけを戻す
            CallDepth = 0;
            ArgIndex = 0;
            UpVarLevel = 0;
            VarIndex = -1;
            ForIndex = 0;
            CallStack[CallDepth] = 0;
            ArgIndexStack[CallDepth] = ArgIndex;
            VarIndexStack[CallDepth] = VarIndex;
            ForIndexStack[CallDepth] = ForIndex;

            HotPointList = new List<HotPoint>();
            ObjColor = Color.White;
            ObjFillColor = Color.White;
            // VB の vbFSTransparent
            ObjFillStyle = FillStyle.VbFSTransparent;
            ObjDrawWidth = 1;
            ObjDrawOption = "";

            // ラベルの初期化
            // SRC# は EventData の全部の行のラベルを登録しなおすので、全部消す。
            // VB はシステム側のイベントラベル（LineNum <= SysEventDataSize）を消さずに残すので、Enable もステージをまたいで残る。
            // SRC# は登録し直した後に、システム側のイベントラベルの Enable を行（EventDataId）ごとに戻す（#1529）。
            // システムデータを読み直すとき（システム側のロードと、最初のシナリオのロードでのシナリオの Lib の読み直し）は戻さない
            var keepSystemEventLabelEnable = load_mode != "システム" && ScenarioLibChecked;
            // ここでの EventData はシステム側の行だけになっている
            var systemEventDataIds = new HashSet<int>(EventData.Select(x => x.ID));
            var systemEventLabelEnable = keepSystemEventLabelEnable
                ? colEventLabelList.List.Where(x => systemEventDataIds.Contains(x.EventDataId))
                    .GroupBy(x => x.EventDataId)
                    .ToDictionary(x => x.Key, x => x.First().Enable)
                : null;
            colNormalLabelList.Clear();
            colEventLabelList.Clear();
            colSysNormalLabelList.Clear();

            // デバッグモードの設定
            if (SRC.Config.GetFlag("Option", "DebugMode"))
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
                // 無ければ戦闘アニメを使わない（設定には残さない）
                SRC.BattleAnimationUnavailable = !battleAnimeIncludeLoaded;

                // システム側のイベントデータの総行数＆ファイル数を記録しておく
                sys_event_data_size = EventData.Count;
                sys_event_file_num = EventFileNames.Count;
            }
            else if (!ScenarioLibChecked)
            {
                // シナリオ側のシステムデータをチェック
                ScenarioLibChecked = true;

                // VB はシナリオの Lib にスペシャルパワー.eve・精神コマンド.eve・汎用戦闘アニメ\include.eve の
                // どれかがあるときだけ読み直す。無ければ読み直してもシステム側のロードと同じファイルを
                // 同じ順に読むだけなので、SRC# はいつも読み直す
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
                    // 無ければ戦闘アニメを使わない（設定には残さない）
                    SRC.BattleAnimationUnavailable = !battleAnimeIncludeLoaded;
                }

                // シナリオ添付の汎用インクルードファイルをダウンロード
                LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ScenarioPath, "Lib", "include.eve"), EventDataSource.System);

                // システム側のイベントデータの総行数＆ファイル数を記録しておく
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

            // 複数行に分割されたコマンドの結合は SrcEveReader が行う

            // ラベルの登録
            // VB はシステム側のロードではラベルの末尾の全角文字を調べない
            var label_error_found = RegisterLabel(load_mode != "システム");
            if (systemEventLabelEnable != null)
            {
                foreach (var lab in colEventLabelList.List)
                {
                    if (systemEventLabelEnable.TryGetValue(lab.EventDataId, out var enable))
                    {
                        lab.Enable = enable;
                    }
                }
            }

            // コマンドデータ配列（EventCmd）は ParseCommand で作る

            // 書式チェックはシナリオ側にのみ実施
            if (load_mode != "システム")
            {
                ParseCommand(label_error_found);
                ValidateCommandArgs();
            }

            // VB はシステム側のロードでは CmdData の生成だけを行う。
            // SRC# の EventCmd はシナリオ側の ParseCommand でシステム側の行の分も作る

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

        private bool RegisterLabel(bool checkSystemLabelSuffix = true)
        {
            // 「登録済なら無視」の判定に、登録済みの行の索引を使う
            registeringNormalLabelIds = new LabelIdIndex(colNormalLabelList);
            registeringEventLabelIds = new LabelIdIndex(colEventLabelList);
            registeringSysNormalLabelIds = new LabelIdIndex(colSysNormalLabelList);
            // VB はラベルを登録する行を CurrentLineNum に入れ（LabelData のエラー表示に使う）、終わったら戻す
            var num = CurrentLineNum;
            try
            {
                return RegisterLabels(checkSystemLabelSuffix);
            }
            finally
            {
                CurrentLineNum = num;
                registeringNormalLabelIds = null;
                registeringEventLabelIds = null;
                registeringSysNormalLabelIds = null;
            }
        }

        private bool RegisterLabels(bool checkSystemLabelSuffix)
        {
            var error_found = false;
            foreach (var line in EventData)
            {
                CurrentLineNum = line.ID;
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
                            if (line.IsSystemData && !checkSystemLabelSuffix)
                            {
                                break;
                            }
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
            for (var lineIndex = 0; lineIndex < EventData.Count; lineIndex++)
            {
                var eventDataLine = EventData[lineIndex];
                // コマンドの構文解析
                var command = parser.Parse(SRC, eventDataLine, out var parsed);
                EventCmd.Add(command);

                // システムデータなら無視しておく
                if (eventDataLine.IsSystemData) { continue; }

                if (!parsed)
                {
                    error_found = true;
                }

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
                            // VB と同じ（Talk の中では、どの Next も「Nextに対応するコマンドがありません」になる）
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

                            // VB と同じく、後ろの引数から順にオプションを読み飛ばす。「終了」があれば選択肢を続けて書かない形。
                            // 括弧の閉じていない行（ArgNum = -1）はループに入らず、選択肢の始まりとして扱う
                            // （VB: Event.bas の LoadEventData の Case AskCmd）
                            var i = command.ArgNum;
                            while (i > 1)
                            {
                                var opt = command.GetArg(i);
                                if (opt == "通常" || opt == "拡大" || opt == "連続表示" || opt == "キャンセル可")
                                {
                                    i--;
                                    continue;
                                }
                                if (opt == "終了")
                                {
                                    i = 3;
                                }
                                break;
                            }
                            if (i < 3)
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
                                        DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
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
                            // VB は "_" で結合した行・コメントの行を " " の行として残し、ここで "" にする。
                            // SrcEveReader はそれらの行を EventData に入れないので、その処理は要らない
                            switch (StackTop())
                            {
                                case CmdType.TalkCmd:
                                case CmdType.AutoTalkCmd:
                                case CmdType.AskCmd:
                                case CmdType.QuestionCmd:
                                    {
                                        // 次の行の最初の語が対応するコマンド・End・Suspend 以外の半角文字だけの語なら、
                                        // End が抜けているとみなす。
                                        // VB は末尾にエラー表示用の空行を足すので、最後の行の次の行は空行とみなす
                                        var nextData = lineIndex + 1 < EventData.Count ? EventData[lineIndex + 1].Data : "";
                                        var buf = Strings.LCase(GeneralLib.ListIndex(nextData, 1));
                                        string buf2;
                                        switch (StackTop())
                                        {
                                            case CmdType.TalkCmd:
                                                buf2 = "talk";
                                                break;
                                            case CmdType.AutoTalkCmd:
                                                buf2 = "autotalk";
                                                break;
                                            case CmdType.AskCmd:
                                                buf2 = "ask";
                                                break;
                                            default:
                                                buf2 = "question";
                                                break;
                                        }
                                        if (buf != buf2 && buf != "end" && buf != "suspend"
                                            && Strings.Len(buf) == Strings.LenB(buf))
                                        {
                                            DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                            cmdStack.Pop();
                                            cmdPosStack.Pop();
                                            error_found = true;
                                        }
                                        break;
                                    }
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
                // VB はシナリオ側の行だけを調べる
                if (command.EventData.IsSystemData)
                {
                    continue;
                }

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
            // 最後に読んだ行（VB の buf。エラーのメッセージを選ぶのに使う）
            var buf = "";
            try
            {
                using (var stream = SRC.FileSystem.OpenText(SRC.Config.SRCCompatibilityMode, fname))
                using (var reader = new SrcEveReader(fname, stream))
                {
                    while (reader.HasMore)
                    {
                        var line = reader.GetLine();
                        lineNumber = reader.LineNumber;
                        buf = line;

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
                if (Strings.Len(buf) == 0)
                {
                    GUI.ErrorMessage(fname + "が開けません");
                }
                else
                {
                    GUI.ErrorMessage(fname + "のロード中にエラーが発生しました" + Constants.vbCr + SrcFormatter.Format(lineNumber) + "行目のイベントデータが不正です");
                }
                SRC.TerminateSRC();
                throw;
            }
        }

        // シナリオ開始後に読み込んだ行（firstLineId 以降）だけのラベルを登録し、構文解析する
        // （VB: CmdData.cls ExecLoadCmd の「ラベルの登録」「イベントデータの構文解析」）。
        // 前からある行の EventCmd は作り直さないので、実行中のコマンドとイベントの続きに影響しない。
        // VB と同じく、ラベルの末尾の全角文字と制御構造の書式はチェックしない。
        // 複数行に分割されたコマンドの結合は、LoadEventData と同じく SrcEveReader がファイルごとに行う
        public void RegisterAndParseAddedEventData(int firstLineId)
        {
            // ラベルの登録
            // VB の AddLabel は登録済みかを見ない。SRC# の AddLabel は登録済みの行を飛ばすが、新しい行は登録されていないので同じになる
            registeringNormalLabelIds = new LabelIdIndex(colNormalLabelList);
            registeringEventLabelIds = new LabelIdIndex(colEventLabelList);
            registeringSysNormalLabelIds = new LabelIdIndex(colSysNormalLabelList);
            // LabelData のエラー表示に使う行（RegisterLabel と同じ）
            var num = CurrentLineNum;
            try
            {
                for (var i = firstLineId; i < EventData.Count; i++)
                {
                    var line = EventData[i];
                    if (Strings.Right(line.Data, 1) == ":")
                    {
                        CurrentLineNum = line.ID;
                        AddLabel(Strings.Left(line.Data, Strings.Len(line.Data) - 1), line.ID);
                    }
                }
            }
            finally
            {
                CurrentLineNum = num;
                registeringNormalLabelIds = null;
                registeringEventLabelIds = null;
                registeringSysNormalLabelIds = null;
            }

            // イベントデータの構文解析
            // VB は Parse の戻り値を見ない
            var parser = new CmdParser();
            for (var i = firstLineId; i < EventData.Count; i++)
            {
                var command = parser.Parse(SRC, EventData[i]);
                if (i < EventCmd.Count)
                {
                    EventCmd[i] = command;
                }
                else
                {
                    EventCmd.Add(command);
                }
            }
        }

        // シナリオ開始後の動的なファイル読み込み（Require コマンド）
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
