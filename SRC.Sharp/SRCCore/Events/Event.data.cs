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
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace SRCCore.Events
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
            ObjFillStyle = HatchStyle.Min;
            ObjDrawWidth = 1;
            ObjDrawOption = "";

            // ラベルの初期化
            // 全部登録しなおしているので全部消してしまってよい
            // XXX 本当か？　手抜きが祟っているだけでは？
            colNormalLabelList.Clear();
            colEventLabelList.Clear();
            colSysNormalLabelList.Clear();

            // デバッグモードの設定
            // TODO Impl
            //if (Strings.LCase(GeneralLib.ReadIni("Option", "DebugMode")) == "on")
            //{
            //    if (!Expression.IsOptionDefined("デバッグ"))
            //    {
            //        Expression.DefineGlobalVariable("Option(デバッグ)");
            //    }

            //    Expression.SetVariableAsLong("Option(デバッグ)", 1);
            //}

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
                // TODO Impl
                //if (Strings.LCase(GeneralLib.ReadIni("Option", "BattleAnimation")) != "off")
                //{
                //    SRC.BattleAnimation = true;
                //}

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
                    // TODO Impl
                    //if (Strings.LCase(GeneralLib.ReadIni("Option", "BattleAnimation")) != "off")
                    //{
                    //    SRC.BattleAnimation = true;
                    //}


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
                    LoadEventData2IfExist(SRC.FileSystem.PathCombine(tfolder, "include.eve"), EventDataSource.Scenario);
                }
                // 汎用Dataインクルードファイルをロード
                LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ScenarioPath, "Data", "include.eve"), EventDataSource.Scenario);
                LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath, "Data", "include.eve"), EventDataSource.Scenario);
                LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.ExtDataPath2, "Data", "include.eve"), EventDataSource.Scenario);
                LoadEventData2IfExist(SRC.FileSystem.PathCombine(SRC.AppPath, "Data", "include.eve"), EventDataSource.Scenario);
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
            RegisterLabel();

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
                ParseCommand();
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

            LoadData(fname, new_titles);
        }

        private void RegisterLabel()
        {
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
                            break;
                        }
                }
            }
        }

        private void ParseCommand()
        {
            // 構文解析と書式チェックその１
            // 制御構造
            var parser = new CmdParser();
            var error_found = false;
            var cmdStack = new Stack<CmdType>();
            var cmdPosStack = new Stack<int>();
            EventCmd.Clear();
            // XXX 無駄解析しないようにする
            //foreach (var eventDataLine in EventData.Where(x => !x.IsSystemData))
            foreach (var eventDataLine in EventData)
            {
                // コマンドの構文解析
                var command = parser.Parse(SRC, eventDataLine);
                EventCmd.Add(command);

                // システムデータなら無視しておく
                if (eventDataLine.IsSystemData) { continue; }

                // TODO Impl
                // リスト長がマイナスのときは括弧の対応が取れていない
                if (command.ArgNum <= -1)
                {
                    switch (cmdStack.Peek())
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
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
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
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (cmdStack.Any() && cmdStack.Peek() != CmdType.IfCmd)
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
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
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
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.IfCmd)
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
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
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
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.DoCmd)
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
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
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
                            if (command.ArgNum == 1 | command.ArgNum == 2)
                            {
                                if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
                                {
                                    DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                    cmdStack.Pop();
                                    cmdPosStack.Pop();
                                    error_found = true;
                                }
                                // XXX defaultのエラー抜けちゃう
                                if (cmdStack.Any())
                                {
                                    switch (cmdStack.Peek())
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
                            }
                            // XXX 条件こけてる気がする
                            else if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
                            {
                                switch (cmdStack.Peek())
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
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
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
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (cmdStack.Any() && cmdStack.Peek() != CmdType.SwitchCmd)
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
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.SwitchCmd)
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
                            if (cmdStack.Any() && cmdStack.Peek() != command.Name)
                            {
                                cmdStack.Push(command.Name);
                                cmdPosStack.Push(command.EventData.ID);
                            }

                            break;
                        }

                    case CmdType.AskCmd:
                        {
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
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
                            if ((command.ArgNum < 3 || command.GetArgs().ToList()
                                    .FindIndex(x => opts.Contains(x.strArg)) < 3)
                                && !command.GetArgs().Select(x => x.strArg).Contains("終了"))
                            {
                                cmdStack.Push(CmdType.AskCmd);
                                cmdPosStack.Push(command.EventData.ID);
                            }

                            break;
                        }

                    case CmdType.QuestionCmd:
                        {
                            if (cmdStack.Any() && cmdStack.Peek() == CmdType.TalkCmd)
                            {
                                DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                cmdStack.Pop();
                                cmdPosStack.Pop();
                                error_found = true;
                            }

                            var i = command.ArgNum;
                            while (i > 1)
                            {
                                switch (command.GetArg(command.ArgNum) ?? "")
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
                                cmdStack.Push(CmdType.QuestionCmd);
                                cmdPosStack.Push(command.EventData.ID);
                            }

                            break;
                        }

                    case CmdType.EndCmd:
                        {
                            if (cmdStack.Any())
                            {
                                switch (cmdStack.Peek())
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
                            }
                            break;
                        }

                    case CmdType.SuspendCmd:
                        {
                            if (cmdStack.Any())
                            {
                                switch (cmdStack.Peek())
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
                            }
                            break;
                        }

                    case CmdType.ExitCmd:
                    case CmdType.PlaySoundCmd:
                    case CmdType.WaitCmd:
                        {
                            if (cmdStack.Any())
                            {
                                switch (cmdStack.Peek())
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
                                // TODO Impl
                                // XXX これ要るの？
                                //        switch (cmdStack.Peek())
                                //        {
                                //            case CmdType.TalkCmd:
                                //            case CmdType.AutoTalkCmd:
                                //            case CmdType.AskCmd:
                                //            case CmdType.QuestionCmd:
                                //                {
                                //                    if (CurrentLineNum == Information.UBound(EventData))
                                //                    {
                                //                        DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                //                        cmdStack.Pop();
                                //                        cmdPosStack.Pop();
                                //                        error_found = true;
                                //                    }
                                //                    else
                                //                    {
                                //                        string buf = Strings.LCase(GeneralLib.ListIndex(EventData[CurrentLineNum + 1], 1));
                                //                        string buf2;
                                //                        switch (cmdStack.Peek())
                                //                        {
                                //                            case CmdType.TalkCmd:
                                //                                {
                                //                                    buf2 = "talk";
                                //                                    break;
                                //                                }

                                //                            case CmdType.AutoTalkCmd:
                                //                                {
                                //                                    buf2 = "autotalk";
                                //                                    break;
                                //                                }

                                //                            case CmdType.AskCmd:
                                //                                {
                                //                                    buf2 = "ask";
                                //                                    break;
                                //                                }

                                //                            case CmdType.QuestionCmd:
                                //                                {
                                //                                    buf2 = "question";
                                //                                    break;
                                //                                }

                                //                            default:
                                //                                {
                                //                                    buf2 = "";
                                //                                    break;
                                //                                }
                                //                        }
                                //                        if ((buf ?? "") != (buf2 ?? "") & buf != "end" & buf != "suspend" & Strings.Len(buf) == LenB(Strings.StrConv(buf, vbFromUnicode)))
                                //                        {
                                //                            DisplayEventErrorMessage(cmdPosStack.Peek(), "Talkに対応するEndがありません");
                                //                            cmdStack.Pop();
                                //                            cmdPosStack.Pop();
                                //                            error_found = true;
                                //                        }
                                //                    }

                                //                    break;
                                //                }
                                //        }
                            }

                            break;
                        }
                }
            }

            // ファイルの末尾まで読んでもコマンドの終わりがなかった？
            if (cmdStack.Count > 0)
            {
                switch (cmdStack.Peek())
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

            // TODO まだエラー出ないようになってない
            //// 書式エラーが見つかった場合はSRCを終了
            //if (error_found)
            //{
            //    SRC.TerminateSRC();
            //}
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

            // 使用しているタイトルのデータをロード
            foreach (var title in new_titles)
            {
                SRC.IncludeData(title);
            }

            // ローカルデータの読みこみ
            if (!SRC.IsLocalDataLoaded || new_titles.Any())
            {
                // TODO Impl
                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\alias.txt"))
                //{
                //    SRC.ALDList.Load(SRC.ScenarioPath + @"Data\alias.txt");
                //}

                //bool localFileExists23() { string argfname = SRC.ScenarioPath + @"Data\mind.txt"; var ret = SRC.FileSystem.FileExists(argfname); return ret; }

                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\sp.txt"))
                //{
                //    SRC.SPDList.Load(SRC.ScenarioPath + @"Data\sp.txt");
                //}
                //else if (localFileExists23())
                //{
                //    SRC.SPDList.Load(SRC.ScenarioPath + @"Data\mind.txt");
                //}

                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\pilot.txt"))
                //{
                //    SRC.PDList.Load(SRC.ScenarioPath + @"Data\pilot.txt");
                //}

                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\non_pilot.txt"))
                //{
                //    SRC.NPDList.Load(SRC.ScenarioPath + @"Data\non_pilot.txt");
                //}

                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\robot.txt"))
                //{
                //    SRC.UDList.Load(SRC.ScenarioPath + @"Data\robot.txt");
                //}

                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\unit.txt"))
                //{
                //    SRC.UDList.Load(SRC.ScenarioPath + @"Data\unit.txt");
                //}

                //GUI.DisplayLoadingProgress();
                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\pilot_message.txt"))
                //{
                //    SRC.MDList.Load(SRC.ScenarioPath + @"Data\pilot_message.txt");
                //}

                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\pilot_dialog.txt"))
                //{
                //    SRC.DDList.Load(SRC.ScenarioPath + @"Data\pilot_dialog.txt");
                //}

                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\effect.txt"))
                //{
                //    SRC.EDList.Load(SRC.ScenarioPath + @"Data\effect.txt");
                //}

                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\animation.txt"))
                //{
                //    SRC.ADList.Load(SRC.ScenarioPath + @"Data\animation.txt");
                //}

                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\ext_animation.txt"))
                //{
                //    SRC.EADList.Load(SRC.ScenarioPath + @"Data\ext_animation.txt");
                //}

                //if (SRC.FileSystem.FileExists(SRC.ScenarioPath + @"Data\item.txt"))
                //{
                //    SRC.IDList.Load(SRC.ScenarioPath + @"Data\item.txt");
                //}

                GUI.DisplayLoadingProgress();
                SRC.IsLocalDataLoaded = true;
            }

            // デフォルトの戦闘アニメデータを設定
            SRC.ADList.AddDefaultAnimation();

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
                            // TODO Impl
                            var fname2 = Strings.Mid(line, 2, Strings.Len(line) - 2);
                            if (fname2 != @"Lib\スペシャルパワー.eve" & fname2 != @"Lib\汎用戦闘アニメ\include.eve" & fname2 != @"Lib\include.eve")
                            {
                                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + fname2)) > 0)
                                {
                                    LoadEventData2(SRC.ScenarioPath + fname2, source);
                                }
                                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                else if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + fname2)) > 0)
                                {
                                    LoadEventData2(SRC.ExtDataPath + fname2, source);
                                }
                                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                else if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + fname2)) > 0)
                                {
                                    LoadEventData2(SRC.ExtDataPath2 + fname2, source);
                                }
                                // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                                else if (Strings.Len(FileSystem.Dir(SRC.AppPath + fname2)) > 0)
                                {
                                    LoadEventData2(SRC.AppPath + fname2, source);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SRC.Log.LogError(ex.Message, ex);
                // XXX
                GUI.ErrorMessage(fname + "のロード中にエラーが発生しました" + Constants.vbCr + SrcFormatter.Format(lineNumber) + "行目のイベントデータが不正です");
                SRC.TerminateSRC();
                throw;
            }
        }
    }
}
