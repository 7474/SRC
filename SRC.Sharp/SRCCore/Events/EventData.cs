using SRC.Core.CmdDatas;
using SRC.Core.Expressions;
using SRC.Core.Lib;
using SRC.Core.VB;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;

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
            string tname, tfolder;
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
            string argini_section = "Option";
            string argini_entry = "DebugMode";
            if (Strings.LCase(GeneralLib.ReadIni(argini_section, argini_entry)) == "on")
            {
                string argoname = "デバッグ";
                if (!Expression.IsOptionDefined(argoname))
                {
                    string argvname = "Option(デバッグ)";
                    Expression.DefineGlobalVariable(argvname);
                }

                string argvname1 = "Option(デバッグ)";
                Expression.SetVariableAsLong(argvname1, 1);
            }

            // システム側のイベントデータのロード
            if (load_mode == "システム")
            {
                // 本体側のシステムデータをチェック

                // スペシャルパワーアニメ用インクルードファイルをダウンロード
                bool localFileExists() { string argfname = SRC.ExtDataPath2 + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                bool localFileExists1() { string argfname = SRC.AppPath + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                bool localFileExists2() { string argfname = SRC.ExtDataPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                bool localFileExists3() { string argfname = SRC.ExtDataPath2 + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                bool localFileExists4() { string argfname = SRC.AppPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                string argfname6 = SRC.ExtDataPath + @"Lib\スペシャルパワー.eve";
                if (GeneralLib.FileExists(argfname6))
                {
                    string argfname = SRC.ExtDataPath + @"Lib\スペシャルパワー.eve";
                    LoadEventData2(argfname);
                }
                else if (localFileExists())
                {
                    string argfname1 = SRC.ExtDataPath2 + @"Lib\スペシャルパワー.eve";
                    LoadEventData2(argfname1);
                }
                else if (localFileExists1())
                {
                    string argfname2 = SRC.AppPath + @"Lib\スペシャルパワー.eve";
                    LoadEventData2(argfname2);
                }
                else if (localFileExists2())
                {
                    string argfname3 = SRC.ExtDataPath + @"Lib\精神コマンド.eve";
                    LoadEventData2(argfname3);
                }
                else if (localFileExists3())
                {
                    string argfname4 = SRC.ExtDataPath2 + @"Lib\精神コマンド.eve";
                    LoadEventData2(argfname4);
                }
                else if (localFileExists4())
                {
                    string argfname5 = SRC.AppPath + @"Lib\精神コマンド.eve";
                    LoadEventData2(argfname5);
                }

                // 汎用戦闘アニメ用インクルードファイルをダウンロード
                string argini_section1 = "Option";
                string argini_entry1 = "BattleAnimation";
                if (Strings.LCase(GeneralLib.ReadIni(argini_section1, argini_entry1)) != "off")
                {
                    SRC.BattleAnimation = true;
                }

                bool localFileExists5() { string argfname = SRC.ExtDataPath2 + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                bool localFileExists6() { string argfname = SRC.AppPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                string argfname10 = SRC.ExtDataPath + @"Lib\汎用戦闘アニメ\include.eve";
                if (GeneralLib.FileExists(argfname10))
                {
                    string argfname7 = SRC.ExtDataPath + @"Lib\汎用戦闘アニメ\include.eve";
                    LoadEventData2(argfname7);
                }
                else if (localFileExists5())
                {
                    string argfname8 = SRC.ExtDataPath2 + @"Lib\汎用戦闘アニメ\include.eve";
                    LoadEventData2(argfname8);
                }
                else if (localFileExists6())
                {
                    string argfname9 = SRC.AppPath + @"Lib\汎用戦闘アニメ\include.eve";
                    LoadEventData2(argfname9);
                }
                else
                {
                    // 戦闘アニメ表示切り替えコマンドを非表示に
                    SRC.BattleAnimation = false;
                }

                // システム側のイベントデータの総行数＆ファイル数を記録しておく
                sys_event_data_size = Information.UBound(EventData);
                sys_event_file_num = Information.UBound(EventFileNames);
            }
            else if (!ScenarioLibChecked)
            {
                // シナリオ側のシステムデータをチェック

                ScenarioLibChecked = true;
                bool localFileExists17() { string argfname = SRC.ScenarioPath + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                bool localFileExists18() { string argfname = SRC.ScenarioPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                bool localFileExists19() { string argfname = SRC.ScenarioPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                if (localFileExists17() | localFileExists18() | localFileExists19())
                {
                    // システムデータのロードをやり直す
                    EventData = new string[1];
                    EventFileID = new int[1];
                    EventLineNum = new int[1];
                    EventFileNames = new string[1];
                    CurrentLineNum = 0;
                    SysEventDataSize = 0;
                    SysEventFileNum = 0;
                    {
                        var withBlock2 = colSysNormalLabelList;
                        var loopTo2 = withBlock2.Count;
                        for (i = 1; i <= loopTo2; i++)
                            withBlock2.Remove(1);
                    }

                    {
                        var withBlock3 = colNormalLabelList;
                        var loopTo3 = withBlock3.Count;
                        for (i = 1; i <= loopTo3; i++)
                            withBlock3.Remove(1);
                    }

                    {
                        var withBlock4 = colEventLabelList;
                        var loopTo4 = withBlock4.Count;
                        for (i = 1; i <= loopTo4; i++)
                            withBlock4.Remove(1);
                    }

                    // スペシャルパワーアニメ用インクルードファイルをダウンロード
                    bool localFileExists7() { string argfname = SRC.ScenarioPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    bool localFileExists8() { string argfname = SRC.ExtDataPath + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    bool localFileExists9() { string argfname = SRC.ExtDataPath2 + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    bool localFileExists10() { string argfname = SRC.AppPath + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    bool localFileExists11() { string argfname = SRC.ExtDataPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    bool localFileExists12() { string argfname = SRC.ExtDataPath2 + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    bool localFileExists13() { string argfname = SRC.AppPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    string argfname19 = SRC.ScenarioPath + @"Lib\スペシャルパワー.eve";
                    if (GeneralLib.FileExists(argfname19))
                    {
                        string argfname11 = SRC.ScenarioPath + @"Lib\スペシャルパワー.eve";
                        LoadEventData2(argfname11);
                    }
                    else if (localFileExists7())
                    {
                        string argfname12 = SRC.ScenarioPath + @"Lib\精神コマンド.eve";
                        LoadEventData2(argfname12);
                    }
                    else if (localFileExists8())
                    {
                        string argfname13 = SRC.ExtDataPath + @"Lib\スペシャルパワー.eve";
                        LoadEventData2(argfname13);
                    }
                    else if (localFileExists9())
                    {
                        string argfname14 = SRC.ExtDataPath2 + @"Lib\スペシャルパワー.eve";
                        LoadEventData2(argfname14);
                    }
                    else if (localFileExists10())
                    {
                        string argfname15 = SRC.AppPath + @"Lib\スペシャルパワー.eve";
                        LoadEventData2(argfname15);
                    }
                    else if (localFileExists11())
                    {
                        string argfname16 = SRC.ExtDataPath + @"Lib\精神コマンド.eve";
                        LoadEventData2(argfname16);
                    }
                    else if (localFileExists12())
                    {
                        string argfname17 = SRC.ExtDataPath2 + @"Lib\精神コマンド.eve";
                        LoadEventData2(argfname17);
                    }
                    else if (localFileExists13())
                    {
                        string argfname18 = SRC.AppPath + @"Lib\精神コマンド.eve";
                        LoadEventData2(argfname18);
                    }

                    // 汎用戦闘アニメ用インクルードファイルをダウンロード
                    string argini_section2 = "Option";
                    string argini_entry2 = "BattleAnimation";
                    if (Strings.LCase(GeneralLib.ReadIni(argini_section2, argini_entry2)) != "off")
                    {
                        SRC.BattleAnimation = true;
                    }

                    bool localFileExists14() { string argfname = SRC.ExtDataPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    bool localFileExists15() { string argfname = SRC.ExtDataPath2 + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    bool localFileExists16() { string argfname = SRC.AppPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                    string argfname24 = SRC.ScenarioPath + @"Lib\汎用戦闘アニメ\include.eve";
                    if (GeneralLib.FileExists(argfname24))
                    {
                        string argfname20 = SRC.ScenarioPath + @"Lib\汎用戦闘アニメ\include.eve";
                        LoadEventData2(argfname20);
                    }
                    else if (localFileExists14())
                    {
                        string argfname21 = SRC.ExtDataPath + @"Lib\汎用戦闘アニメ\include.eve";
                        LoadEventData2(argfname21);
                    }
                    else if (localFileExists15())
                    {
                        string argfname22 = SRC.ExtDataPath2 + @"Lib\汎用戦闘アニメ\include.eve";
                        LoadEventData2(argfname22);
                    }
                    else if (localFileExists16())
                    {
                        string argfname23 = SRC.AppPath + @"Lib\汎用戦闘アニメ\include.eve";
                        LoadEventData2(argfname23);
                    }
                    else
                    {
                        // 戦闘アニメ表示切り替えコマンドを非表示に
                        SRC.BattleAnimation = false;
                    }
                }

                // シナリオ添付の汎用インクルードファイルをダウンロード
                string argfname26 = SRC.ScenarioPath + @"Lib\include.eve";
                if (GeneralLib.FileExists(argfname26))
                {
                    string argfname25 = SRC.ScenarioPath + @"Lib\include.eve";
                    LoadEventData2(argfname25);
                }

                // システム側のイベントデータの総行数＆ファイル数を記録しておく
                sys_event_data_size = Information.UBound(EventData);
                sys_event_file_num = Information.UBound(EventFileNames);

                // シナリオ側のイベントデータのロード
                LoadEventData2(fname);
            }
            else
            {
                // シナリオ側のイベントデータのロード
                LoadEventData2(fname);
            }

            // データ読みこみ指定
            foreach (var line in EventData.Where(x => !x.IsSystemData))
            {
                if (Strings.Left(line.Data, 1) == "@")
                {
                    tname = Strings.Mid(line.Data, 2);

                    // 既にそのデータが読み込まれているかチェック
                    if (!SRC.Titles.Contains(tname))
                    {
                        // フォルダを検索
                        tfolder = SRC.SearchDataFolder(tname);
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
            // XXX 未処理
            if (load_mode != "システム")
            {
                // 作品毎のインクルードファイル
                foreach (var title in SRC.Titles)
                {
                    tfolder = SRC.SearchDataFolder(title);
                    string argfname28 = tfolder + @"\include.eve";
                    if (GeneralLib.FileExists(argfname28))
                    {
                        string argfname27 = tfolder + @"\include.eve";
                        LoadEventData2(argfname27);
                    }
                }

                // 汎用Dataインクルードファイルをロード
                bool localFileExists20() { string argfname = SRC.ExtDataPath + @"Data\include.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                bool localFileExists21() { string argfname = SRC.ExtDataPath2 + @"Data\include.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                bool localFileExists22() { string argfname = SRC.AppPath + @"Data\include.eve"; var ret = GeneralLib.FileExists(argfname); return ret; }

                string argfname33 = SRC.ScenarioPath + @"Data\include.eve";
                if (GeneralLib.FileExists(argfname33))
                {
                    string argfname29 = SRC.ScenarioPath + @"Data\include.eve";
                    LoadEventData2(argfname29);
                }
                else if (localFileExists20())
                {
                    string argfname30 = SRC.ExtDataPath + @"Data\include.eve";
                    LoadEventData2(argfname30);
                }
                else if (localFileExists21())
                {
                    string argfname31 = SRC.ExtDataPath2 + @"Data\include.eve";
                    LoadEventData2(argfname31);
                }
                else if (localFileExists22())
                {
                    string argfname32 = SRC.AppPath + @"Data\include.eve";
                    LoadEventData2(argfname32);
                }
            }

            // 複数行に分割されたコマンドを結合
            var loopTo8 = Information.UBound(EventData) - 1;
            for (i = SysEventDataSize + 1; i <= loopTo8; i++)
            {
                if (Strings.Right(EventData[i], 1) == "_")
                {
                    EventData[i + 1] = Strings.Left(EventData[i], Strings.Len(EventData[i]) - 1) + EventData[i + 1];
                    EventData[i] = " ";
                }
            }

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

            // CmdDataクラスのインスタンスの生成のみ行っておく
            else if (CurrentLineNum > Information.UBound(EventCmd))
            {
                Array.Resize(EventCmd, CurrentLineNum + 1);
                i = CurrentLineNum;
                while (EventCmd[i] is null)
                {
                    EventCmd[i] = new CmdData();
                    EventCmd[i].LineNum = i;
                    i = i - 1;
                }
            }

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

            // ロードするデータ数をカウント
            num = 2 * Information.UBound(new_titles);
            if (SRC.IsLocalDataLoaded)
            {
                if (num > 0)
                {
                    num = num + 2;
                }
            }
            else
            {
                num = num + 2;
            }

            string argfname34 = Strings.Left(fname, Strings.Len(fname) - 4) + ".map";
            if (GeneralLib.FileExists(argfname34))
            {
                num = num + 1;
            }

            if (num == 0 & SRC.IsLocalDataLoaded)
            {
                // デフォルトの戦闘アニメデータを設定
                SRC.ADList.AddDefaultAnimation();
                return;
            }

            // ロード画面を表示
            GUI.OpenNowLoadingForm();

            // ロードサイズを設定
            GUI.SetLoadImageSize(num);

            // 使用しているタイトルのデータをロード
            var loopTo16 = Information.UBound(new_titles);
            for (i = 1; i <= loopTo16; i++)
                SRC.IncludeData(new_titles[i]);

            // ローカルデータの読みこみ
            if (!SRC.IsLocalDataLoaded | Information.UBound(new_titles) > 0)
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
            string argfname61 = Strings.Left(fname, Strings.Len(fname) - 4) + ".map";
            if (GeneralLib.FileExists(argfname61))
            {
                string argfname60 = Strings.Left(fname, Strings.Len(fname) - 4) + ".map";
                Map.LoadMapData(argfname60);
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

        // イベントファイルの読み込み
        public void LoadEventData2(string fname, int lnum = 0)
        {
            int FileNumber, CurrentLineNum2;
            int i;
            string buf, fname2;
            int fid;
            bool in_single_quote, in_double_quote;
            if (string.IsNullOrEmpty(fname))
            {
                return;
            }

            // イベントファイル名を記録しておく (エラー表示用)
            Array.Resize(EventFileNames, Information.UBound(EventFileNames) + 1 + 1);
            EventFileNames[Information.UBound(EventFileNames)] = fname;
            fid = Information.UBound(EventFileNames);
            ;

            // ファイルを開く
            FileNumber = FileSystem.FreeFile();
            FileSystem.FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read);

            // 行番号の設定
            if (lnum > 0)
            {
                CurrentLineNum = lnum;
            }

            CurrentLineNum2 = 0;

            // 各行の読み込み
            while (!FileSystem.EOF(FileNumber))
            {
                CurrentLineNum = CurrentLineNum + 1;
                CurrentLineNum2 = (CurrentLineNum2 + 1);

                // データ領域確保
                Array.Resize(EventData, CurrentLineNum + 1);
                Array.Resize(EventFileID, CurrentLineNum + 1);
                Array.Resize(EventLineNum, CurrentLineNum + 1);

                // 行の読み込み
                buf = FileSystem.LineInput(FileNumber);
                GeneralLib.TrimString(buf);

                // コメントを削除
                if (Strings.Left(buf, 1) == "#")
                {
                    buf = " ";
                }
                else if (Strings.InStr(buf, "//") > 0)
                {
                    in_single_quote = false;
                    in_double_quote = false;
                    var loopTo = Strings.Len(buf);
                    for (i = 1; i <= loopTo; i++)
                    {
                        switch (Strings.Mid(buf, i, 1) ?? "")
                        {
                            case "`":
                                {
                                    // シングルクオート
                                    if (!in_double_quote)
                                    {
                                        in_single_quote = !in_single_quote;
                                    }

                                    break;
                                }

                            case "\"":
                                {
                                    // ダブルクオート
                                    if (!in_single_quote)
                                    {
                                        in_double_quote = !in_double_quote;
                                    }

                                    break;
                                }

                            case "/":
                                {
                                    // コメント？
                                    if (!in_double_quote & !in_single_quote)
                                    {
                                        if (i > 1)
                                        {
                                            if (Strings.Mid(buf, i - 1, 1) == "/")
                                            {
                                                buf = Strings.Left(buf, i - 2);
                                                if (string.IsNullOrEmpty(buf))
                                                {
                                                    buf = " ";
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

                // 行を保存
                EventData[CurrentLineNum] = buf;
                EventFileID[CurrentLineNum] = fid;
                EventLineNum[CurrentLineNum] = CurrentLineNum2;

                // 他のイベントファイルの読み込み
                if (Strings.Left(buf, 1) == "<")
                {
                    if (Strings.InStr(buf, ">") == Strings.Len(buf) & buf != "<>")
                    {
                        CurrentLineNum = CurrentLineNum - 1;
                        fname2 = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                        if (fname2 != @"Lib\スペシャルパワー.eve" & fname2 != @"Lib\汎用戦闘アニメ\include.eve" & fname2 != @"Lib\include.eve")
                        {
                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                            if (Strings.Len(FileSystem.Dir(SRC.ScenarioPath + fname2)) > 0)
                            {
                                string argfname = SRC.ScenarioPath + fname2;
                                LoadEventData2(argfname);
                            }
                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                            else if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + fname2)) > 0)
                            {
                                string argfname1 = SRC.ExtDataPath + fname2;
                                LoadEventData2(argfname1);
                            }
                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                            else if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + fname2)) > 0)
                            {
                                string argfname2 = SRC.ExtDataPath2 + fname2;
                                LoadEventData2(argfname2);
                            }
                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                            else if (Strings.Len(FileSystem.Dir(SRC.AppPath + fname2)) > 0)
                            {
                                string argfname3 = SRC.AppPath + fname2;
                                LoadEventData2(argfname3);
                            }
                        }
                    }
                }
            }

            // ファイルを閉じる
            FileSystem.FileClose(FileNumber);
            return;
        ErrorHandler:
            ;
            if (Strings.Len(buf) == 0)
            {
                string argmsg = fname + "が開けません";
                GUI.ErrorMessage(argmsg);
            }
            else
            {
                string argmsg1 = fname + "のロード中にエラーが発生しました" + Constants.vbCr + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CurrentLineNum2) + "行目のイベントデータが不正です";
                GUI.ErrorMessage(argmsg1);
            }

            SRC.TerminateSRC();
        }
    }
}
