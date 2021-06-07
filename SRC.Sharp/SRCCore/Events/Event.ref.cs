using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.CompilerServices;

namespace Project1
{
    // UPGRADE_NOTE: Event は Event_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
    static class Event_Renamed
    {

        // Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
        // 本プログラムはフリーソフトであり、無保証です。
        // 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
        // 再頒布または改変することができます。

        // イベントデータの各種処理を行うモジュール

        // イベントデータ
        public static string[] EventData;
        // イベントコマンドリスト
        public static CmdData[] EventCmd;
        // 個々の行がどのイベントファイルに属しているか
        public static short[] EventFileID;
        // 個々の行がイベントファイルの何行目に位置するか
        public static short[] EventLineNum;
        // イベントファイルのファイル名リスト
        public static string[] EventFileNames;
        // Requireコマンドで追加されたイベントファイルのファイル名リスト
        public static string[] AdditionalEventFileNames;

        // システム側のイベントデータのサイズ(行数)
        private static int SysEventDataSize;
        // システム側のイベントファイル数
        private static short SysEventFileNum;
        // シナリオ添付のシステムファイルがチェックされたかどうか
        private static bool ScenarioLibChecked;

        // ラベルのリスト
        public static Collection colEventLabelList = new Collection();
        private static Collection colSysNormalLabelList = new Collection();
        private static Collection colNormalLabelList = new Collection();


        // 変数用のコレクション
        public static Collection GlobalVariableList = new Collection();
        public static Collection LocalVariableList = new Collection();

        // 現在の行番号
        public static int CurrentLineNum;

        // イベントで選択されているユニット・ターゲット
        public static Unit SelectedUnitForEvent;
        public static Unit SelectedTargetForEvent;

        // イベント呼び出しのキュー
        public static string[] EventQue;
        // 現在実行中のイベントラベル
        public static int CurrentLabel;

        // Askコマンドで選択した選択肢
        public static string SelectedAlternative;

        // 関数呼び出し用変数

        // 最大呼び出し階層数
        public const short MaxCallDepth = 50;
        // 引数の最大数
        public const short MaxArgIndex = 200;
        // サブルーチンローカル変数の最大数
        public const short MaxVarIndex = 2000;

        // 呼び出し履歴
        public static short CallDepth;
        public static int[] CallStack = new int[(MaxCallDepth + 1)];
        // 引数スタック
        public static short ArgIndex;
        public static short[] ArgIndexStack = new short[(MaxCallDepth + 1)];
        public static string[] ArgStack = new string[(MaxArgIndex + 1)];
        // UpVarコマンドによって引数が何段階シフトしているか
        public static short UpVarLevel;
        public static short[] UpVarLevelStack = new short[(MaxCallDepth + 1)];
        // サブルーチンローカル変数スタック
        public static short VarIndex;
        public static short[] VarIndexStack = new short[(MaxCallDepth + 1)];
        public static VarData[] VarStack = new VarData[(MaxVarIndex + 1)];
        // Forインデックス用スタック
        public static short ForIndex;
        public static short[] ForIndexStack = new short[(MaxCallDepth + 1)];
        public static int[] ForLimitStack = new int[(MaxCallDepth + 1)];

        // ForEachコマンド用変数
        public static short ForEachIndex;
        public static string[] ForEachSet;

        // Rideコマンド用パイロット搭乗履歴
        public static string LastUnitName;
        public static string[] LastPilotID;

        // Wait開始時刻
        public static int WaitStartTime;
        public static int WaitTimeCount;

        // 描画基準座標
        public static int BaseX;
        public static int BaseY;
        private static int[] SavedBaseX = new int[11];
        private static int[] SavedBaseY = new int[11];
        private static int BasePointIndex;

        // オブジェクトの色
        public static int ObjColor;
        // オブジェクトの線の太さ
        public static int ObjDrawWidth;
        // オブジェクトの背景色
        public static int ObjFillColor;
        // オブジェクトの背景描画方法
        public static int ObjFillStyle;
        // オブジェクトの描画方法
        public static string ObjDrawOption;

        // ホットポイント
        public struct HotPoint
        {
            public string Name;
            // UPGRADE_NOTE: Left は Left_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
            public short Left_Renamed;
            public short Top;
            public short width;
            public short Height;
            public string Caption;
        }

        public static HotPoint[] HotPointList;

        // イベントコマンドエラーメッセージ
        public static string EventErrorMessage;

        // ユニットがセンタリングされたか？
        public static bool IsUnitCenter;


        // イベントコマンドの種類
        public enum CmdType
        {
            NullCmd = 0,
            NopCmd,
            ArcCmd,
            ArrayCmd,
            AskCmd,
            AttackCmd,
            AutoTalkCmd,
            BossRankCmd,
            BreakCmd,
            CallCmd,
            ReturnCmd,
            CallInterMissionCommandCmd,
            CancelCmd,
            CenterCmd,
            ChangeAreaCmd,
            ChangeLayerCmd,
            ChangeMapCmd,
            ChangeModeCmd,
            ChangePartyCmd,
            ChangeTerrainCmd,
            ChangeUnitBitmapCmd,
            ChargeCmd,
            CircleCmd,
            ClearEventCmd,
            ClearImageCmd,
            ClearLayerCmd,
            ClearObjCmd,
            ClearPictureCmd,
            ClearSkillCmd,
            ClearSpecialPowerCmd,
            ClearStatusCmd,
            CloseCmd,
            ClsCmd,
            ColorCmd,
            ColorFilterCmd,
            CombineCmd,
            ConfirmCmd,
            ContinueCmd,
            CopyArrayCmd,
            CopyFileCmd,
            CreateCmd,
            CreateFolderCmd,
            DebugCmd,
            DestroyCmd,
            DisableCmd,
            DoCmd,
            LoopCmd,
            DrawOptionCmd,
            DrawWidthCmd,
            EnableCmd,
            EquipCmd,
            EscapeCmd,
            ExchangeItemCmd,
            ExecCmd,
            ExitCmd,
            ExplodeCmd,
            ExpUpCmd,
            FadeInCmd,
            FadeOutCmd,
            FillColorCmd,
            FillStyleCmd,
            FinishCmd,
            FixCmd,
            FontCmd,
            ForCmd,
            ForEachCmd,
            NextCmd,
            ForgetCmd,
            GameClearCmd,
            GameOverCmd,
            FreeMemoryCmd,
            GetOffCmd,
            GlobalCmd,
            GotoCmd,
            HideCmd,
            HotPointCmd,
            IfCmd,
            ElseCmd,
            ElseIfCmd,
            EndIfCmd,
            IncrCmd,
            IncreaseMoraleCmd,
            InputCmd,
            IntermissionCommandCmd,
            ItemCmd,
            JoinCmd,
            KeepBGMCmd,
            LandCmd,
            LaunchCmd,
            LeaveCmd,
            LevelUpCmd,
            LineCmd,
            LineReadCmd,
            LoadCmd,
            LocalCmd,
            MakePilotListCmd,
            MakeUnitListCmd,
            MapAbilityCmd,
            MapAttackCmd,
            MoneyCmd,
            MonotoneCmd,
            MoveCmd,
            NightCmd,
            NoonCmd,
            OpenCmd,
            OptionCmd,
            OrganizeCmd,
            OvalCmd,
            PaintPictureCmd,
            PaintStringCmd,
            PaintStringRCmd,
            PaintSysStringCmd,
            PilotCmd,
            PlayMIDICmd,
            PlaySoundCmd,
            PolygonCmd,
            PrintCmd,
            PSetCmd,
            QuestionCmd,
            QuickLoadCmd,
            QuitCmd,
            RankUpCmd,
            ReadCmd,
            RecoverENCmd,
            RecoverHPCmd,
            RecoverPlanaCmd,
            RecoverSPCmd,
            RedrawCmd,
            RefreshCmd,
            ReleaseCmd,
            RemoveFileCmd,
            RemoveFolderCmd,
            RemoveItemCmd,
            RemovePilotCmd,
            RemoveUnitCmd,
            RenameBGMCmd,
            RenameFileCmd,
            RenameTermCmd,
            ReplacePilotCmd,
            RequireCmd,
            RestoreEventCmd,
            RideCmd,
            SaveDataCmd,
            SelectCmd,
            SelectTargetCmd,
            SepiaCmd,
            SetCmd,
            SetSkillCmd,
            SetBulletCmd,
            SetMessageCmd,
            SetRelationCmd,
            SetStatusStringColorCmd,
            SetStatusCmd,
            SetStockCmd,
            SetWindowColorCmd,
            SetWindowFrameWidthCmd,
            ShowCmd,
            ShowImageCmd,
            ShowUnitStatusCmd,
            SkipCmd,
            SortCmd,
            SpecialPowerCmd,
            SplitCmd,
            StartBGMCmd,
            StopBGMCmd,
            StopSummoningCmd,
            SupplyCmd,
            SunsetCmd,
            SwapCmd,
            SwitchCmd,
            CaseCmd,
            CaseElseCmd,
            EndSwCmd,
            TalkCmd,
            EndCmd,
            SuspendCmd,
            TelopCmd,
            TransformCmd,
            UnitCmd,
            UnsetCmd,
            UpgradeCmd,
            UpVarCmd,
            UseAbilityCmd,
            WaitCmd,
            WaterCmd,
            WhiteInCmd,
            WhiteOutCmd,
            WriteCmd,
            PlayFlashCmd,
            ClearFlashCmd
        }

        // イベントラベルの種類
        public enum LabelType
        {
            NormalLabel = 0,
            PrologueEventLabel,
            StartEventLabel,
            EpilogueEventLabel,
            TurnEventLabel,
            DamageEventLabel,
            DestructionEventLabel,
            TotalDestructionEventLabel,
            AttackEventLabel,
            AfterAttackEventLabel,
            TalkEventLabel,
            ContactEventLabel,
            EnterEventLabel,
            EscapeEventLabel,
            LandEventLabel,
            UseEventLabel,
            AfterUseEventLabel,
            TransformEventLabel,
            CombineEventLabel,
            SplitEventLabel,
            FinishEventLabel,
            LevelUpEventLabel,
            RequirementEventLabel,
            ResumeEventLabel,
            MapCommandEventLabel,
            UnitCommandEventLabel,
            EffectEventLabel
        }


        // イベントデータを初期化
        public static void InitEventData()
        {
            int i;
            SRC.Titles = new string[1];
            EventData = new string[1];
            EventCmd = new CmdData[50001];
            EventQue = new string[1];

            // オブジェクトの生成には時間がかかるので、
            // あらかじめCmdDataオブジェクトを生成しておく。
            var loopTo = Information.UBound(EventCmd);
            for (i = 1; i <= loopTo; i++)
            {
                EventCmd[i] = new CmdData();
                EventCmd[i].LineNum = i;
            }

            // 本体側のシナリオデータをチェックする
            LoadEventData(ref "", ref "システム");
        }

        // イベントファイルのロード
        public static void LoadEventData(ref string fname, [Optional, DefaultParameterValue("")] ref string load_mode)
        {
            string buf, buf2;
            string tname, tfolder;
            string[] new_titles;
            int i, num;
            short j;
            var CmdStack = new CmdType[51];
            short CmdStackIdx;
            var CmdPosStack = new int[51];
            short CmdPosStackIdx;
            var error_found = default(bool);
            var sys_event_data_size = default(int);
            var sys_event_file_num = default(int);

            // データの初期化
            Array.Resize(ref EventData, SysEventDataSize + 1);
            Array.Resize(ref EventFileID, SysEventDataSize + 1);
            Array.Resize(ref EventLineNum, SysEventDataSize + 1);
            Array.Resize(ref EventFileNames, SysEventFileNum + 1);
            AdditionalEventFileNames = new string[1];
            CurrentLineNum = SysEventDataSize;
            CallDepth = 0;
            ArgIndex = 0;
            UpVarLevel = 0;
            VarIndex = 0;
            if (VarStack[1] is null)
            {
                var loopTo = Information.UBound(VarStack);
                for (i = 1; i <= loopTo; i++)
                    VarStack[i] = new VarData();
            }

            ForIndex = 0;
            new_titles = new string[1];
            HotPointList = new HotPoint[1];
            ObjColor = ColorTranslator.ToOle(Color.White);
            ObjFillColor = ColorTranslator.ToOle(Color.White);
            // UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            ObjFillStyle = vbFSTransparent;
            ObjDrawWidth = 1;
            ObjDrawOption = "";

            // ラベルの初期化
            {
                var withBlock = colNormalLabelList;
                var loopTo1 = withBlock.Count;
                for (i = 1; i <= loopTo1; i++)
                    withBlock.Remove(1);
            }

            i = 1;
            {
                var withBlock1 = colEventLabelList;
                while (i <= withBlock1.Count)
                {
                    // UPGRADE_WARNING: オブジェクト colEventLabelList.Item(i).LineNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(withBlock1[i].LineNum, SysEventDataSize, false)))
                    {
                        withBlock1.Remove(i);
                    }
                    else
                    {
                        i = i + 1;
                    }
                }
            }

            // デバッグモードの設定
            if (Strings.LCase(GeneralLib.ReadIni(ref "Option", ref "DebugMode")) == "on")
            {
                if (!Expression.IsOptionDefined(ref "デバッグ"))
                {
                    Expression.DefineGlobalVariable(ref "Option(デバッグ)");
                }

                Expression.SetVariableAsLong(ref "Option(デバッグ)", 1);
            }

            // システム側のイベントデータのロード
            if (load_mode == "システム")
            {
                // 本体側のシステムデータをチェック

                // スペシャルパワーアニメ用インクルードファイルをダウンロード
                bool localFileExists() { string argfname = SRC.ExtDataPath2 + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists1() { string argfname = SRC.AppPath + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists2() { string argfname = SRC.ExtDataPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists3() { string argfname = SRC.ExtDataPath2 + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists4() { string argfname = SRC.AppPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                if (GeneralLib.FileExists(ref SRC.ExtDataPath + @"Lib\スペシャルパワー.eve"))
                {
                    LoadEventData2(ref SRC.ExtDataPath + @"Lib\スペシャルパワー.eve");
                }
                else if (localFileExists())
                {
                    LoadEventData2(ref SRC.ExtDataPath2 + @"Lib\スペシャルパワー.eve");
                }
                else if (localFileExists1())
                {
                    LoadEventData2(ref SRC.AppPath + @"Lib\スペシャルパワー.eve");
                }
                else if (localFileExists2())
                {
                    LoadEventData2(ref SRC.ExtDataPath + @"Lib\精神コマンド.eve");
                }
                else if (localFileExists3())
                {
                    LoadEventData2(ref SRC.ExtDataPath2 + @"Lib\精神コマンド.eve");
                }
                else if (localFileExists4())
                {
                    LoadEventData2(ref SRC.AppPath + @"Lib\精神コマンド.eve");
                }

                // 汎用戦闘アニメ用インクルードファイルをダウンロード
                if (Strings.LCase(GeneralLib.ReadIni(ref "Option", ref "BattleAnimation")) != "off")
                {
                    SRC.BattleAnimation = true;
                }

                bool localFileExists5() { string argfname = SRC.ExtDataPath2 + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists6() { string argfname = SRC.AppPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                if (GeneralLib.FileExists(ref SRC.ExtDataPath + @"Lib\汎用戦闘アニメ\include.eve"))
                {
                    LoadEventData2(ref SRC.ExtDataPath + @"Lib\汎用戦闘アニメ\include.eve");
                }
                else if (localFileExists5())
                {
                    LoadEventData2(ref SRC.ExtDataPath2 + @"Lib\汎用戦闘アニメ\include.eve");
                }
                else if (localFileExists6())
                {
                    LoadEventData2(ref SRC.AppPath + @"Lib\汎用戦闘アニメ\include.eve");
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
                bool localFileExists17() { string argfname = SRC.ScenarioPath + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists18() { string argfname = SRC.ScenarioPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists19() { string argfname = SRC.ScenarioPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                if (localFileExists17() || localFileExists18() || localFileExists19())
                {
                    // システムデータのロードをやり直す
                    EventData = new string[1];
                    EventFileID = new short[1];
                    EventLineNum = new short[1];
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
                    bool localFileExists7() { string argfname = SRC.ScenarioPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                    bool localFileExists8() { string argfname = SRC.ExtDataPath + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                    bool localFileExists9() { string argfname = SRC.ExtDataPath2 + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                    bool localFileExists10() { string argfname = SRC.AppPath + @"Lib\スペシャルパワー.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                    bool localFileExists11() { string argfname = SRC.ExtDataPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                    bool localFileExists12() { string argfname = SRC.ExtDataPath2 + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                    bool localFileExists13() { string argfname = SRC.AppPath + @"Lib\精神コマンド.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                    if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Lib\スペシャルパワー.eve"))
                    {
                        LoadEventData2(ref SRC.ScenarioPath + @"Lib\スペシャルパワー.eve");
                    }
                    else if (localFileExists7())
                    {
                        LoadEventData2(ref SRC.ScenarioPath + @"Lib\精神コマンド.eve");
                    }
                    else if (localFileExists8())
                    {
                        LoadEventData2(ref SRC.ExtDataPath + @"Lib\スペシャルパワー.eve");
                    }
                    else if (localFileExists9())
                    {
                        LoadEventData2(ref SRC.ExtDataPath2 + @"Lib\スペシャルパワー.eve");
                    }
                    else if (localFileExists10())
                    {
                        LoadEventData2(ref SRC.AppPath + @"Lib\スペシャルパワー.eve");
                    }
                    else if (localFileExists11())
                    {
                        LoadEventData2(ref SRC.ExtDataPath + @"Lib\精神コマンド.eve");
                    }
                    else if (localFileExists12())
                    {
                        LoadEventData2(ref SRC.ExtDataPath2 + @"Lib\精神コマンド.eve");
                    }
                    else if (localFileExists13())
                    {
                        LoadEventData2(ref SRC.AppPath + @"Lib\精神コマンド.eve");
                    }

                    // 汎用戦闘アニメ用インクルードファイルをダウンロード
                    if (Strings.LCase(GeneralLib.ReadIni(ref "Option", ref "BattleAnimation")) != "off")
                    {
                        SRC.BattleAnimation = true;
                    }

                    bool localFileExists14() { string argfname = SRC.ExtDataPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                    bool localFileExists15() { string argfname = SRC.ExtDataPath2 + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                    bool localFileExists16() { string argfname = SRC.AppPath + @"Lib\汎用戦闘アニメ\include.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                    if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Lib\汎用戦闘アニメ\include.eve"))
                    {
                        LoadEventData2(ref SRC.ScenarioPath + @"Lib\汎用戦闘アニメ\include.eve");
                    }
                    else if (localFileExists14())
                    {
                        LoadEventData2(ref SRC.ExtDataPath + @"Lib\汎用戦闘アニメ\include.eve");
                    }
                    else if (localFileExists15())
                    {
                        LoadEventData2(ref SRC.ExtDataPath2 + @"Lib\汎用戦闘アニメ\include.eve");
                    }
                    else if (localFileExists16())
                    {
                        LoadEventData2(ref SRC.AppPath + @"Lib\汎用戦闘アニメ\include.eve");
                    }
                    else
                    {
                        // 戦闘アニメ表示切り替えコマンドを非表示に
                        SRC.BattleAnimation = false;
                    }
                }

                // シナリオ添付の汎用インクルードファイルをダウンロード
                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Lib\include.eve"))
                {
                    LoadEventData2(ref SRC.ScenarioPath + @"Lib\include.eve");
                }

                // システム側のイベントデータの総行数＆ファイル数を記録しておく
                sys_event_data_size = Information.UBound(EventData);
                sys_event_file_num = Information.UBound(EventFileNames);

                // シナリオ側のイベントデータのロード
                LoadEventData2(ref fname);
            }
            else
            {
                // シナリオ側のイベントデータのロード
                LoadEventData2(ref fname);
            }

            // エラー表示用にサイズを大きく取っておく
            Array.Resize(ref EventData, Information.UBound(EventData) + 1 + 1);
            Array.Resize(ref EventLineNum, Information.UBound(EventData) + 1);
            EventData[Information.UBound(EventData)] = "";
            EventLineNum[Information.UBound(EventData)] = (short)(EventLineNum[Information.UBound(EventData) - 1] + 1);

            // データ読みこみ指定
            var loopTo5 = Information.UBound(EventData);
            for (i = SysEventDataSize + 1; i <= loopTo5; i++)
            {
                if (Strings.Left(EventData[i], 1) == "@")
                {
                    tname = Strings.Mid(EventData[i], 2);

                    // 既にそのデータが読み込まれているかチェック
                    var loopTo6 = (short)Information.UBound(SRC.Titles);
                    for (j = 1; j <= loopTo6; j++)
                    {
                        if ((tname ?? "") == (SRC.Titles[j] ?? ""))
                        {
                            break;
                        }
                    }

                    if (j > Information.UBound(SRC.Titles))
                    {
                        // フォルダを検索
                        tfolder = SRC.SearchDataFolder(ref tname);
                        if (Strings.Len(tfolder) == 0)
                        {
                            DisplayEventErrorMessage(i, "データ「" + tname + "」のフォルダが見つかりません");
                        }
                        else
                        {
                            Array.Resize(ref new_titles, Information.UBound(new_titles) + 1 + 1);
                            Array.Resize(ref SRC.Titles, Information.UBound(SRC.Titles) + 1 + 1);
                            new_titles[Information.UBound(new_titles)] = tname;
                            SRC.Titles[Information.UBound(SRC.Titles)] = tname;
                        }
                    }
                }
            }

            // 各作品データのinclude.eveを読み込む
            if (load_mode != "システム")
            {
                // 作品毎のインクルードファイル
                var loopTo7 = Information.UBound(SRC.Titles);
                for (i = 1; i <= loopTo7; i++)
                {
                    tfolder = SRC.SearchDataFolder(ref SRC.Titles[i]);
                    if (GeneralLib.FileExists(ref tfolder + @"\include.eve"))
                    {
                        LoadEventData2(ref tfolder + @"\include.eve");
                    }
                }

                // 汎用Dataインクルードファイルをロード
                bool localFileExists20() { string argfname = SRC.ExtDataPath + @"Data\include.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists21() { string argfname = SRC.ExtDataPath2 + @"Data\include.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                bool localFileExists22() { string argfname = SRC.AppPath + @"Data\include.eve"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\include.eve"))
                {
                    LoadEventData2(ref SRC.ScenarioPath + @"Data\include.eve");
                }
                else if (localFileExists20())
                {
                    LoadEventData2(ref SRC.ExtDataPath + @"Data\include.eve");
                }
                else if (localFileExists21())
                {
                    LoadEventData2(ref SRC.ExtDataPath2 + @"Data\include.eve");
                }
                else if (localFileExists22())
                {
                    LoadEventData2(ref SRC.AppPath + @"Data\include.eve");
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
            num = CurrentLineNum;
            if (load_mode == "システム")
            {
                var loopTo9 = Information.UBound(EventData);
                for (CurrentLineNum = 1; CurrentLineNum <= loopTo9; CurrentLineNum++)
                {
                    buf = EventData[CurrentLineNum];
                    if (Strings.Right(buf, 1) == ":")
                    {
                        AddSysLabel(ref Strings.Left(buf, Strings.Len(buf) - 1), CurrentLineNum);
                    }
                }
            }
            else if (sys_event_data_size > 0)
            {
                // システム側へのイベントデータの追加があった場合
                var loopTo11 = sys_event_data_size;
                for (CurrentLineNum = 1; CurrentLineNum <= loopTo11; CurrentLineNum++)
                {
                    buf = EventData[CurrentLineNum];
                    switch (Strings.Right(buf, 1) ?? "")
                    {
                        case ":":
                            {
                                AddSysLabel(ref Strings.Left(buf, Strings.Len(buf) - 1), CurrentLineNum);
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

                var loopTo12 = Information.UBound(EventData);
                for (CurrentLineNum = sys_event_data_size + 1; CurrentLineNum <= loopTo12; CurrentLineNum++)
                {
                    buf = EventData[CurrentLineNum];
                    switch (Strings.Right(buf, 1) ?? "")
                    {
                        case ":":
                            {
                                AddLabel(ref Strings.Left(buf, Strings.Len(buf) - 1), CurrentLineNum);
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
            }
            else
            {
                var loopTo10 = Information.UBound(EventData);
                for (CurrentLineNum = SysEventDataSize + 1; CurrentLineNum <= loopTo10; CurrentLineNum++)
                {
                    buf = EventData[CurrentLineNum];
                    switch (Strings.Right(buf, 1) ?? "")
                    {
                        case ":":
                            {
                                AddLabel(ref Strings.Left(buf, Strings.Len(buf) - 1), CurrentLineNum);
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
            }

            CurrentLineNum = num;

            // コマンドデータ配列を設定
            if (Information.UBound(EventData) > Information.UBound(EventCmd))
            {
                num = Information.UBound(EventCmd);
                Array.Resize(ref EventCmd, Information.UBound(EventData) + 1);
                var loopTo13 = Information.UBound(EventCmd);
                for (i = num + 1; i <= loopTo13; i++)
                {
                    EventCmd[i] = new CmdData();
                    EventCmd[i].LineNum = i;
                }
            }

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
                        if (!withBlock5.Parse(ref EventData[CurrentLineNum]))
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (withBlock5.GetArg(4) == "then")
                                    {
                                        CmdStackIdx = (short)(CmdStackIdx + 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx + 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] != CmdType.IfCmd)
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "ElseIfに対応するIfがありません");
                                        error_found = true;
                                        CmdStackIdx = (short)(CmdStackIdx + 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx + 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "Elseに対応するIfがありません");
                                        error_found = true;
                                        CmdStackIdx = (short)(CmdStackIdx + 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx + 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] == CmdType.IfCmd)
                                    {
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    CmdStackIdx = (short)(CmdStackIdx + 1);
                                    CmdPosStackIdx = (short)(CmdPosStackIdx + 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] == CmdType.DoCmd)
                                    {
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    CmdStackIdx = (short)(CmdStackIdx + 1);
                                    CmdPosStackIdx = (short)(CmdPosStackIdx + 1);
                                    CmdStack[CmdStackIdx] = withBlock5.Name;
                                    CmdPosStack[CmdPosStackIdx] = CurrentLineNum;
                                    break;
                                }

                            case CmdType.NextCmd:
                                {
                                    if (withBlock5.ArgNum == 1 || withBlock5.ArgNum == 2)
                                    {
                                        if (CmdStack[CmdStackIdx] == CmdType.TalkCmd)
                                        {
                                            num = CmdPosStack[CmdPosStackIdx];
                                            DisplayEventErrorMessage(num, "Talkに対応するEndがありません");
                                            CmdStackIdx = (short)(CmdStackIdx - 1);
                                            CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                            error_found = true;
                                        }

                                        switch (CmdStack[CmdStackIdx])
                                        {
                                            case CmdType.ForCmd:
                                            case CmdType.ForEachCmd:
                                                {
                                                    CmdStackIdx = (short)(CmdStackIdx - 1);
                                                    CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
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
                                                    CmdStackIdx = (short)(CmdStackIdx - 1);
                                                    CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
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

                                    CmdStackIdx = (short)(CmdStackIdx + 1);
                                    CmdPosStackIdx = (short)(CmdPosStackIdx + 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] != CmdType.SwitchCmd)
                                    {
                                        DisplayEventErrorMessage(CurrentLineNum, "Caseに対応するSwitchがありません");
                                        error_found = true;
                                        CmdStackIdx = (short)(CmdStackIdx + 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx + 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    if (CmdStack[CmdStackIdx] == CmdType.SwitchCmd)
                                    {
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx + 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx + 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                        error_found = true;
                                    }

                                    i = withBlock5.ArgNum;
                                    while (i > 1)
                                    {
                                        switch (withBlock5.GetArg((short)i) ?? "")
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
                                        CmdStackIdx = (short)(CmdStackIdx + 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx + 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
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
                                        CmdStackIdx = (short)(CmdStackIdx + 1);
                                        CmdPosStackIdx = (short)(CmdPosStackIdx + 1);
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
                                                CmdStackIdx = (short)(CmdStackIdx - 1);
                                                CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
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
                                                CmdStackIdx = (short)(CmdStackIdx - 1);
                                                CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
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
                                                CmdStackIdx = (short)(CmdStackIdx - 1);
                                                CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
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
                                                        CmdStackIdx = (short)(CmdStackIdx - 1);
                                                        CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
                                                        error_found = true;
                                                    }
                                                    else
                                                    {
                                                        buf = Strings.LCase(GeneralLib.ListIndex(ref EventData[CurrentLineNum + 1], 1));
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
                                                            CmdStackIdx = (short)(CmdStackIdx - 1);
                                                            CmdPosStackIdx = (short)(CmdPosStackIdx - 1);
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
                Array.Resize(ref EventCmd, CurrentLineNum + 1);
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
                SysEventFileNum = (short)sys_event_file_num;
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

            if (GeneralLib.FileExists(ref Strings.Left(fname, Strings.Len(fname) - 4) + ".map"))
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
            GUI.SetLoadImageSize((short)num);

            // 使用しているタイトルのデータをロード
            var loopTo16 = Information.UBound(new_titles);
            for (i = 1; i <= loopTo16; i++)
                SRC.IncludeData(ref new_titles[i]);

            // ローカルデータの読みこみ
            if (!SRC.IsLocalDataLoaded || Information.UBound(new_titles) > 0)
            {
                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\alias.txt"))
                {
                    SRC.ALDList.Load(ref SRC.ScenarioPath + @"Data\alias.txt");
                }

                bool localFileExists23() { string argfname = SRC.ScenarioPath + @"Data\mind.txt"; var ret = GeneralLib.FileExists(ref argfname); return ret; }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\sp.txt"))
                {
                    SRC.SPDList.Load(ref SRC.ScenarioPath + @"Data\sp.txt");
                }
                else if (localFileExists23())
                {
                    SRC.SPDList.Load(ref SRC.ScenarioPath + @"Data\mind.txt");
                }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\pilot.txt"))
                {
                    SRC.PDList.Load(ref SRC.ScenarioPath + @"Data\pilot.txt");
                }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\non_pilot.txt"))
                {
                    SRC.NPDList.Load(ref SRC.ScenarioPath + @"Data\non_pilot.txt");
                }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\robot.txt"))
                {
                    SRC.UDList.Load(ref SRC.ScenarioPath + @"Data\robot.txt");
                }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\unit.txt"))
                {
                    SRC.UDList.Load(ref SRC.ScenarioPath + @"Data\unit.txt");
                }

                GUI.DisplayLoadingProgress();
                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\pilot_message.txt"))
                {
                    SRC.MDList.Load(ref SRC.ScenarioPath + @"Data\pilot_message.txt");
                }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\pilot_dialog.txt"))
                {
                    SRC.DDList.Load(ref SRC.ScenarioPath + @"Data\pilot_dialog.txt");
                }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\effect.txt"))
                {
                    SRC.EDList.Load(ref SRC.ScenarioPath + @"Data\effect.txt");
                }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\animation.txt"))
                {
                    SRC.ADList.Load(ref SRC.ScenarioPath + @"Data\animation.txt");
                }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\ext_animation.txt"))
                {
                    SRC.EADList.Load(ref SRC.ScenarioPath + @"Data\ext_animation.txt");
                }

                if (GeneralLib.FileExists(ref SRC.ScenarioPath + @"Data\item.txt"))
                {
                    SRC.IDList.Load(ref SRC.ScenarioPath + @"Data\item.txt");
                }

                GUI.DisplayLoadingProgress();
                SRC.IsLocalDataLoaded = true;
            }

            // デフォルトの戦闘アニメデータを設定
            SRC.ADList.AddDefaultAnimation();

            // マップデータをロード
            if (GeneralLib.FileExists(ref Strings.Left(fname, Strings.Len(fname) - 4) + ".map"))
            {
                Map.LoadMapData(ref Strings.Left(fname, Strings.Len(fname) - 4) + ".map");
                GUI.SetupBackground(draw_mode: ref "", draw_option: ref "", filter_color: ref 0, filter_trans_par: ref 0d);
                GUI.RedrawScreen();
                GUI.DisplayLoadingProgress();
            }

            // ロード画面を閉じる
            GUI.CloseNowLoadingForm();
        }

        // イベントファイルの読み込み
        public static void LoadEventData2(ref string fname, int lnum = 0)
        {
            short FileNumber, CurrentLineNum2;
            short i;
            string buf, fname2;
            short fid;
            bool in_single_quote, in_double_quote;
            if (string.IsNullOrEmpty(fname))
            {
                return;
            }

            // イベントファイル名を記録しておく (エラー表示用)
            Array.Resize(ref EventFileNames, Information.UBound(EventFileNames) + 1 + 1);
            EventFileNames[Information.UBound(EventFileNames)] = fname;
            fid = (short)Information.UBound(EventFileNames);
            ;

            // ファイルを開く
            FileNumber = (short)FileSystem.FreeFile();
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
                CurrentLineNum2 = (short)(CurrentLineNum2 + 1);

                // データ領域確保
                Array.Resize(ref EventData, CurrentLineNum + 1);
                Array.Resize(ref EventFileID, CurrentLineNum + 1);
                Array.Resize(ref EventLineNum, CurrentLineNum + 1);

                // 行の読み込み
                buf = FileSystem.LineInput(FileNumber);
                GeneralLib.TrimString(ref buf);

                // コメントを削除
                if (Strings.Left(buf, 1) == "#")
                {
                    buf = " ";
                }
                else if (Strings.InStr(buf, "//") > 0)
                {
                    in_single_quote = false;
                    in_double_quote = false;
                    var loopTo = (short)Strings.Len(buf);
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
                                LoadEventData2(ref SRC.ScenarioPath + fname2);
                            }
                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                            else if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath + fname2)) > 0)
                            {
                                LoadEventData2(ref SRC.ExtDataPath + fname2);
                            }
                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                            else if (Strings.Len(FileSystem.Dir(SRC.ExtDataPath2 + fname2)) > 0)
                            {
                                LoadEventData2(ref SRC.ExtDataPath2 + fname2);
                            }
                            // UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
                            else if (Strings.Len(FileSystem.Dir(SRC.AppPath + fname2)) > 0)
                            {
                                LoadEventData2(ref SRC.AppPath + fname2);
                            }
                        }
                    }
                }
            }

            // ファイルを閉じる
            FileSystem.FileClose((int)FileNumber);
            return;
            ErrorHandler:
            ;
            if (Strings.Len(buf) == 0)
            {
                GUI.ErrorMessage(ref fname + "が開けません");
            }
            else
            {
                GUI.ErrorMessage(ref fname + "のロード中にエラーが発生しました" + Constants.vbCr + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(CurrentLineNum2) + "行目のイベントデータが不正です");
            }

            SRC.TerminateSRC();
        }


        // イベントの実行
        // UPGRADE_WARNING: ParamArray Args が ByRef から ByVal に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93C6A0DC-8C99-429A-8696-35FC4DCEFCCC"' をクリックしてください。
        public static void HandleEvent(params object[] Args)
        {
            short event_que_idx;
            int ret;
            short i;
            bool flag;
            bool prev_is_gui_locked;
            short prev_call_depth;
            string uparty;
            Unit u;
            bool main_event_done;

            // 画面入力をロック
            prev_is_gui_locked = GUI.IsGUILocked;
            if (!GUI.IsGUILocked)
            {
                GUI.LockGUI();
            }

            // 現在選択されているユニット＆ターゲットをイベント用に設定
            // (SearchLabel()実行時の式計算用にあらかじめ設定しておく)
            SelectedUnitForEvent = Commands.SelectedUnit;
            // 引数に指定されたユニットを優先
            if (Information.UBound(Args) > 0)
            {
                if (SRC.PList.IsDefined(ref Args[1]))
                {
                    {
                        var withBlock = SRC.PList.Item(ref Args[1]);
                        if (withBlock.Unit_Renamed is object)
                        {
                            SelectedUnitForEvent = withBlock.Unit_Renamed;
                        }
                    }
                }
            }

            SelectedTargetForEvent = Commands.SelectedTarget;

            // イベントキューを作成
            Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
            event_que_idx = (short)Information.UBound(EventQue);
            switch (Args[0])
            {
                case "プロローグ":
                    {
                        EventQue[Information.UBound(EventQue)] = "プロローグ";
                        SRC.Stage = "プロローグ";
                        break;
                    }

                case "エピローグ":
                    {
                        EventQue[Information.UBound(EventQue)] = "エピローグ";
                        SRC.Stage = "エピローグ";
                        break;
                    }

                case "破壊":
                    {
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject("破壊 ", Args[1]));
                        {
                            var withBlock1 = SRC.PList.Item(ref Args[1]);
                            uparty = withBlock1.Party;
                            if (withBlock1.Unit_Renamed is object)
                            {
                                {
                                    var withBlock2 = withBlock1.Unit_Renamed;
                                    // 格納されていたユニットも破壊しておく
                                    // MOD START MARGE
                                    // For i = 1 To .CountUnitOnBoard
                                    // Set u = .UnitOnBoard(1)
                                    // .UnloadUnit u.ID
                                    // u.Status = "破壊"
                                    // u.HP = 0
                                    // ReDim Preserve EventQue(UBound(EventQue) + 1)
                                    // EventQue(UBound(EventQue)) = _
                                    // '                                "破壊 " & u.MainPilot.ID
                                    // Next
                                    while (withBlock2.CountUnitOnBoard() > 0)
                                    {
                                        u = withBlock2.UnitOnBoard(ref 1);
                                        withBlock2.UnloadUnit(ref (object)u.ID);
                                        u.Status_Renamed = "破壊";
                                        u.HP = 0;
                                        Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
                                        EventQue[Information.UBound(EventQue)] = "マップ攻撃破壊 " + u.MainPilot().ID;
                                    }
                                    // MOD END MARGE
                                    uparty = withBlock2.Party0;
                                }
                            }
                        }

                        // 全滅の判定
                        flag = false;
                        foreach (Unit currentU in SRC.UList)
                        {
                            u = currentU;
                            {
                                var withBlock3 = u;
                                if ((withBlock3.Party0 ?? "") == (uparty ?? "") & withBlock3.Status_Renamed == "出撃" & !withBlock3.IsConditionSatisfied(ref "憑依"))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }

                        if (!flag)
                        {
                            Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
                            EventQue[Information.UBound(EventQue)] = "全滅 " + uparty;
                        }

                        break;
                    }

                case "マップ攻撃破壊":
                    {
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject("マップ攻撃破壊 ", Args[1]));
                        {
                            var withBlock4 = SRC.PList.Item(ref Args[1]);
                            uparty = withBlock4.Party;
                            if (withBlock4.Unit_Renamed is object)
                            {
                                {
                                    var withBlock5 = withBlock4.Unit_Renamed;
                                    // 格納されていたユニットも破壊しておく
                                    var loopTo = withBlock5.CountUnitOnBoard();
                                    for (i = 1; i <= loopTo; i++)
                                    {
                                        u = withBlock5.UnitOnBoard(ref i);
                                        withBlock5.UnloadUnit(ref (object)u.ID);
                                        u.Status_Renamed = "破壊";
                                        u.HP = 0;
                                        Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
                                        EventQue[Information.UBound(EventQue)] = "マップ攻撃破壊 " + u.MainPilot().ID;
                                    }

                                    uparty = withBlock5.Party0;
                                }
                            }
                        }

                        break;
                    }

                case "ターン":
                    {
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject("ターン 全 ", Args[2]));
                        Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
                        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject("ターン " + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Args[1]) + " ", Args[2]));
                        break;
                    }

                case "損傷率":
                    {
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("損傷率 ", Args[1]), " "), Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Args[2])));
                        break;
                    }

                case "攻撃":
                    {
                        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("攻撃 ", Args[1]), " "), Args[2]));
                        break;
                    }

                case "攻撃後":
                    {
                        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("攻撃後 ", Args[1]), " "), Args[2]));
                        break;
                    }

                case "会話":
                    {
                        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("会話 ", Args[1]), " "), Args[2]));
                        break;
                    }

                case "接触":
                    {
                        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("接触 ", Args[1]), " "), Args[2]));
                        break;
                    }

                case "進入":
                    {
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("進入 ", Args[1]), " "), Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Args[2])), " "), Microsoft.VisualBasic.Compatibility.VB6.Support.Format(Args[3])));
                        Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("進入 ", Args[1]), " "), Map.TerrainName(Conversions.ToShort(Args[2]), Conversions.ToShort(Args[3]))));
                        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Args[2], 1, false)))
                        {
                            Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
                            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("脱出 ", Args[1]), " W"));
                        }
                        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        else if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Args[2], Map.MapWidth, false)))
                        {
                            Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
                            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("脱出 ", Args[1]), " E"));
                        }
                        // UPGRADE_WARNING: オブジェクト Args(3) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Args[3], 1, false)))
                        {
                            Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
                            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("脱出 ", Args[1]), " N"));
                        }
                        // UPGRADE_WARNING: オブジェクト Args(3) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        else if (Conversions.ToBoolean(Operators.ConditionalCompareObjectEqual(Args[3], Map.MapHeight, false)))
                        {
                            Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
                            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject("脱出 ", Args[1]), " S"));
                        }

                        break;
                    }

                case "収納":
                    {
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject("収納 ", Args[1]));
                        break;
                    }

                case "使用":
                    {
                        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("使用 ", Args[1]), " "), Args[2]));
                        break;
                    }

                case "使用後":
                    {
                        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("使用後 ", Args[1]), " "), Args[2]));
                        break;
                    }

                case "行動終了":
                    {
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject("行動終了 ", Args[1]));
                        break;
                    }

                case "ユニットコマンド":
                    {
                        // UPGRADE_WARNING: オブジェクト Args(2) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("ユニットコマンド ", Args[1]), " "), Args[2]));
                        if (!IsEventDefined(ref EventQue[Information.UBound(EventQue)]))
                        {
                            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(Operators.ConcatenateObject(Operators.ConcatenateObject("ユニットコマンド ", Args[1]), " "), SRC.PList.Item(ref Args[2]).Unit_Renamed.Name));
                        }

                        break;
                    }

                default:
                    {
                        // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        EventQue[Information.UBound(EventQue)] = Conversions.ToString(Args[0]);
                        var loopTo1 = (short)Information.UBound(Args);
                        for (i = 1; i <= loopTo1; i++)
                            // UPGRADE_WARNING: オブジェクト Args(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(EventQue[Information.UBound(EventQue)] + " ", Args[i]));
                        break;
                    }
            }

            if (CallDepth > MaxCallDepth)
            {
                GUI.ErrorMessage(ref "サブルーチンの呼び出し階層が" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(MaxCallDepth) + "を超えているため、イベントの処理が出来ません");
                CallDepth = MaxCallDepth;
                return;
            }

            // 現在の状態を保存
            ArgIndexStack[CallDepth] = ArgIndex;
            VarIndexStack[CallDepth] = VarIndex;
            ForIndexStack[CallDepth] = ForIndex;
            SaveBasePoint();

            // 呼び出し階層数をインクリメント
            prev_call_depth = CallDepth;
            CallDepth = (short)(CallDepth + 1);

            // 各イベントを発生させる
            i = event_que_idx;
            SRC.IsCanceled = false;
            do
            {
                // Debug.Print "HandleEvent (" & EventQue(i) & ")"

                // 前のイベントで他のユニットが出現している可能性があるので
                // 本当に全滅したのか判定
                if (GeneralLib.LIndex(ref EventQue[i], 1) == "全滅")
                {
                    uparty = GeneralLib.LIndex(ref EventQue[i], 2);
                    foreach (Unit currentU1 in SRC.UList)
                    {
                        u = currentU1;
                        if ((u.Party0 ?? "") == (uparty ?? "") & u.Status_Renamed == "出撃" & !u.IsConditionSatisfied(ref "憑依"))
                        {
                            goto NextLoop;
                        }
                    }
                }

                CurrentLabel = 0;
                main_event_done = false;
                while (true)
                {
                    // 現在選択されているユニット＆ターゲットをイベント用に設定
                    // SearchLabel()で入れ替えられる可能性があるので、毎回設定し直す必要あり
                    SelectedUnitForEvent = Commands.SelectedUnit;
                    // 引数に指定されたユニットを優先
                    if (Information.UBound(Args) > 0)
                    {
                        if (SRC.PList.IsDefined(ref Args[1]))
                        {
                            {
                                var withBlock6 = SRC.PList.Item(ref Args[1]);
                                if (withBlock6.Unit_Renamed is object)
                                {
                                    SelectedUnitForEvent = withBlock6.Unit_Renamed;
                                }
                            }
                        }
                    }

                    SelectedTargetForEvent = Commands.SelectedTarget;

                    // 実行するイベントラベルを探す
                    do
                    {
                        if (Information.IsNumeric(EventQue[i]))
                        {
                            if (CurrentLabel == 0)
                            {
                                ret = Conversions.ToInteger(EventQue[i]);
                            }
                            else
                            {
                                ret = 0;
                            }
                        }
                        else
                        {
                            ret = SearchLabel(ref EventQue[i], CurrentLabel + 1);
                        }

                        if (ret == 0)
                        {
                            goto NextLoop;
                        }

                        CurrentLabel = ret;
                        if (Strings.Asc(EventData[ret]) != 42) // *
                        {
                            // 常時イベントではないイベントは１度しか実行しない
                            if (main_event_done)
                            {
                                ret = 0;
                            }
                            else
                            {
                                main_event_done = true;
                            }
                        }
                    }
                    while (ret == 0);

                    // 戦闘後のイベント実行前にはいくつかの後始末が必要
                    if (Strings.Left(EventData[ret], 1) != "*")
                    {
                        // UPGRADE_WARNING: オブジェクト Args(0) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        if (Conversions.ToBoolean(Operators.OrObject(Operators.OrObject(Operators.OrObject(Operators.ConditionalCompareObjectEqual(Args[0], "破壊", false), Operators.ConditionalCompareObjectEqual(Args[0], "損傷率", false)), Operators.ConditionalCompareObjectEqual(Args[0], "攻撃後", false)), Operators.ConditionalCompareObjectEqual(Args[0], "全滅", false))))
                        {
                            // 画面をクリア
                            if (GUI.MainForm.Visible == true)
                            {
                                Status.ClearUnitStatus();
                                GUI.RedrawScreen();
                            }

                            // メッセージウィンドウを閉じる
                            if (My.MyProject.Forms.frmMessage.Visible == true)
                            {
                                GUI.CloseMessageForm();
                            }
                        }
                    }

                    // ラベルの行は実行しても無駄なので
                    ret = ret + 1;
                    Application.DoEvents();

                    // イベントの各コマンドを実行
                    do
                    {
                        CurrentLineNum = ret;
                        if (CurrentLineNum > Information.UBound(EventCmd))
                        {
                            goto ExitLoop;
                        }

                        ret = EventCmd[CurrentLineNum].Exec();
                    }
                    while (ret > 0);

                    // ステージが終了 or キャンセル？
                    if (SRC.IsScenarioFinished || SRC.IsCanceled)
                    {
                        goto ExitLoop;
                    }
                }

                NextLoop:
                ;
                i = (short)(i + 1);
            }
            while (i <= Information.UBound(EventQue));
            ExitLoop:
            ;
            if (CallDepth >= 0)
            {
                // 呼び出し階層数を元に戻す
                // （サブルーチン内でExitが呼ばれることがあるので単純に-1出来ない）
                CallDepth = prev_call_depth;

                // イベント実行前の状態に復帰
                ArgIndex = ArgIndexStack[CallDepth];
                VarIndex = VarIndexStack[CallDepth];
                ForIndex = ForIndexStack[CallDepth];
            }
            else
            {
                ArgIndex = 0;
                VarIndex = 0;
                ForIndex = 0;
            }

            // イベントキューを元に戻す
            Array.Resize(ref EventQue, GeneralLib.MinLng(event_que_idx - 1, Information.UBound(EventQue)) + 1);

            // フォント設定をデフォルトに戻す
            // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            {
                var withBlock7 = GUI.MainForm.picMain(0);
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock7.ForeColor = Information.RGB(255, 255, 255);
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                {
                    var withBlock8 = withBlock7.Font;
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock8.Size = 16;
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock8.Name = "ＭＳ Ｐ明朝";
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock8.Bold = true;
                    // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                    withBlock8.Italic = false;
                }

                GUI.PermanentStringMode = false;
                GUI.KeepStringMode = false;
            }

            // オブジェクト色をデフォルトに戻す
            ObjColor = ColorTranslator.ToOle(Color.White);
            ObjFillColor = ColorTranslator.ToOle(Color.White);
            // UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            ObjFillStyle = vbFSTransparent;
            ObjDrawWidth = 1;
            ObjDrawOption = "";

            // 描画の基準座標位置を元に戻す
            RestoreBasePoint();

            // 画面入力のロックを解除
            if (!prev_is_gui_locked)
            {
                GUI.UnlockGUI();
            }
        }

        // イベントを登録しておき、後で実行
        // UPGRADE_WARNING: ParamArray Args が ByRef から ByVal に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="93C6A0DC-8C99-429A-8696-35FC4DCEFCCC"' をクリックしてください。
        public static void RegisterEvent(params object[] Args)
        {
            short i;
            Array.Resize(ref EventQue, Information.UBound(EventQue) + 1 + 1);
            // UPGRADE_WARNING: オブジェクト Args() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
            EventQue[Information.UBound(EventQue)] = Conversions.ToString(Args[0]);
            var loopTo = (short)Information.UBound(Args);
            for (i = 1; i <= loopTo; i++)
                // UPGRADE_WARNING: オブジェクト Args(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                EventQue[Information.UBound(EventQue)] = Conversions.ToString(Operators.ConcatenateObject(EventQue[Information.UBound(EventQue)] + " ", Args[i]));
        }


        // ラベルの検索
        public static int SearchLabel(ref string lname, int start = 0)
        {
            int SearchLabelRet = default;
            LabelType ltype;
            short llen;
            var litem = default(string[]);
            var lnum = new string[5];
            var is_unit = new bool[5];
            var is_num = new bool[5];
            var is_condition = new bool[5];
            string str2, str1, lname2;
            int i;
            Unit tmp_u;
            bool revrersible = default, reversed;

            // ラベルの各要素をあらかじめ解析
            llen = GeneralLib.ListSplit(ref lname, ref litem);

            // ラベルの種類を判定
            switch (litem[1] ?? "")
            {
                case "プロローグ":
                    {
                        ltype = LabelType.PrologueEventLabel;
                        break;
                    }

                case "スタート":
                    {
                        ltype = LabelType.StartEventLabel;
                        break;
                    }

                case "エピローグ":
                    {
                        ltype = LabelType.EpilogueEventLabel;
                        break;
                    }

                case "ターン":
                    {
                        ltype = LabelType.TurnEventLabel;
                        if (Information.IsNumeric(litem[2]))
                        {
                            is_num[2] = true;
                        }

                        lnum[2] = GeneralLib.StrToLng(ref litem[2]).ToString();
                        break;
                    }

                case "損傷率":
                    {
                        ltype = LabelType.DamageEventLabel;
                        is_unit[2] = true;
                        is_num[3] = true;
                        lnum[3] = GeneralLib.StrToLng(ref litem[3]).ToString();
                        break;
                    }

                case "破壊":
                case "マップ攻撃破壊":
                    {
                        ltype = LabelType.DestructionEventLabel;
                        is_unit[2] = true;
                        break;
                    }

                case "全滅":
                    {
                        ltype = LabelType.TotalDestructionEventLabel;
                        break;
                    }

                case "攻撃":
                    {
                        ltype = LabelType.AttackEventLabel;
                        revrersible = true;
                        is_unit[2] = true;
                        is_unit[3] = true;
                        break;
                    }

                case "攻撃後":
                    {
                        ltype = LabelType.AfterAttackEventLabel;
                        revrersible = true;
                        is_unit[2] = true;
                        is_unit[3] = true;
                        break;
                    }

                case "会話":
                    {
                        ltype = LabelType.TalkEventLabel;
                        is_unit[2] = true;
                        is_unit[3] = true;
                        break;
                    }

                case "接触":
                    {
                        ltype = LabelType.ContactEventLabel;
                        revrersible = true;
                        is_unit[2] = true;
                        is_unit[3] = true;
                        break;
                    }

                case "進入":
                    {
                        ltype = LabelType.EnterEventLabel;
                        is_unit[2] = true;
                        if (llen == 4)
                        {
                            is_num[3] = true;
                            is_num[4] = true;
                            lnum[3] = GeneralLib.StrToLng(ref litem[3]).ToString();
                            lnum[4] = GeneralLib.StrToLng(ref litem[4]).ToString();
                        }

                        break;
                    }

                case "脱出":
                    {
                        ltype = LabelType.EscapeEventLabel;
                        is_unit[2] = true;
                        break;
                    }

                case "収納":
                    {
                        ltype = LabelType.LandEventLabel;
                        is_unit[2] = true;
                        break;
                    }

                case "使用":
                    {
                        ltype = LabelType.UseEventLabel;
                        is_unit[2] = true;
                        break;
                    }

                case "使用後":
                    {
                        ltype = LabelType.AfterUseEventLabel;
                        is_unit[2] = true;
                        break;
                    }

                case "変形":
                    {
                        ltype = LabelType.TransformEventLabel;
                        is_unit[2] = true;
                        break;
                    }

                case "合体":
                    {
                        ltype = LabelType.CombineEventLabel;
                        is_unit[2] = true;
                        break;
                    }

                case "分離":
                    {
                        ltype = LabelType.SplitEventLabel;
                        is_unit[2] = true;
                        break;
                    }

                case "行動終了":
                    {
                        ltype = LabelType.FinishEventLabel;
                        is_unit[2] = true;
                        break;
                    }

                case "レベルアップ":
                    {
                        ltype = LabelType.LevelUpEventLabel;
                        is_unit[2] = true;
                        break;
                    }

                case "勝利条件":
                    {
                        ltype = LabelType.RequirementEventLabel;
                        break;
                    }

                case "再開":
                    {
                        ltype = LabelType.ResumeEventLabel;
                        break;
                    }

                case "マップコマンド":
                    {
                        ltype = LabelType.MapCommandEventLabel;
                        is_condition[3] = true;
                        break;
                    }

                case "ユニットコマンド":
                    {
                        ltype = LabelType.UnitCommandEventLabel;
                        is_condition[4] = true;
                        break;
                    }

                case "特殊効果":
                    {
                        ltype = LabelType.EffectEventLabel;
                        break;
                    }

                default:
                    {
                        ltype = LabelType.NormalLabel;
                        break;
                    }
            }

            // 各ラベルについて一致しているかチェック
            foreach (LabelData lab in colEventLabelList)
            {
                // ラベルの種類が一致している？
                if (ltype != lab.Name)
                {
                    goto NextLabel;
                }

                // ClearEventされていない？
                if (!lab.Enable)
                {
                    goto NextLabel;
                }

                // 検索開始行より後ろ？
                if (lab.LineNum < start)
                {
                    goto NextLabel;
                }

                // パラメータ数が一致している？
                if (llen != lab.CountPara())
                {
                    if (ltype != LabelType.MapCommandEventLabel & ltype != LabelType.UnitCommandEventLabel)
                    {
                        goto NextLabel;
                    }
                }

                // 各パラメータが一致している？
                reversed = false;
                CheckPara:
                ;
                var loopTo = (int)llen;
                for (i = 2; i <= loopTo; i++)
                {
                    // コマンド関連ラベルの最後のパラメータは条件式なのでチェックを省く
                    if (is_condition[i])
                    {
                        break;
                    }

                    // 比較するパラメータ
                    str1 = litem[i];
                    if (reversed)
                    {
                        str2 = lab.Para((short)(5 - i));
                    }
                    else
                    {
                        str2 = lab.Para((short)i);
                    }

                    // 「全」は全てに一致
                    if (str2 == "全")
                    {
                        // だだし、「ターン 全」が２回実行されるのは防ぐ
                        if (ltype != LabelType.TurnEventLabel || i != 2)
                        {
                            goto NextPara;
                        }
                    }

                    // 数値として比較？
                    if (is_num[i])
                    {
                        if (Information.IsNumeric(str2))
                        {
                            if (Conversions.ToDouble(lnum[i]) == Conversions.ToInteger(str2))
                            {
                                goto NextPara;
                            }
                            else if (ltype == LabelType.DamageEventLabel)
                            {
                                // 損傷率ラベルの処理
                                if (Conversions.ToDouble(lnum[i]) > Conversions.ToInteger(str2))
                                {
                                    break;
                                }
                            }
                        }

                        goto NextLabel;
                    }

                    // ユニット指定として比較？
                    if (is_unit[i])
                    {
                        bool localIsDefined() { object argIndex1 = str2; var ret = SRC.PList.IsDefined(ref argIndex1); return ret; }

                        bool localIsDefined1() { object argIndex1 = str2; var ret = SRC.PDList.IsDefined(ref argIndex1); return ret; }

                        bool localIsDefined2() { object argIndex1 = str2; var ret = SRC.UDList.IsDefined(ref argIndex1); return ret; }

                        if (str2 == "味方" || str2 == "ＮＰＣ" || str2 == "敵" || str2 == "中立")
                        {
                            // 陣営名で比較
                            if (str1 != "味方" & str1 != "ＮＰＣ" & str1 != "敵" & str1 != "中立")
                            {
                                if (SRC.PList.IsDefined(ref str1))
                                {
                                    Pilot localItem() { object argIndex1 = str1; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                    str1 = localItem().Party;
                                }
                            }
                        }
                        else if (localIsDefined())
                        {
                            // パイロットで比較
                            {
                                var withBlock = SRC.PList.Item(ref str2);
                                if ((str2 ?? "") == (withBlock.Data.Name ?? "") || (str2 ?? "") == (withBlock.Data.Nickname ?? ""))
                                {
                                    // グループＩＤが付けられていない場合は
                                    // パイロット名で比較
                                    str2 = withBlock.Name;
                                    if (SRC.PList.IsDefined(ref str1))
                                    {
                                        Pilot localItem2() { object argIndex1 = str1; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        str1 = localItem2().Name;
                                    }
                                }
                                else
                                {
                                    // グループＩＤが付けられている場合は
                                    // グループＩＤで比較
                                    if (SRC.PList.IsDefined(ref str1))
                                    {
                                        Pilot localItem3() { object argIndex1 = str1; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                        str1 = localItem3().ID;
                                    }

                                    if (Strings.InStr(str1, ":") > 0)
                                    {
                                        str1 = Strings.Left(str1, Strings.InStr(str1, ":") - 1);
                                    }
                                }
                            }
                        }
                        else if (localIsDefined1())
                        {
                            // パイロット名で比較
                            PilotData localItem4() { object argIndex1 = str2; var ret = SRC.PDList.Item(ref argIndex1); return ret; }

                            str2 = localItem4().Name;
                            if (SRC.PList.IsDefined(ref str1))
                            {
                                Pilot localItem5() { object argIndex1 = str1; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                str1 = localItem5().Name;
                            }
                        }
                        else if (localIsDefined2())
                        {
                            // ユニット名で比較
                            if (SRC.PList.IsDefined(ref str1))
                            {
                                {
                                    var withBlock1 = SRC.PList.Item(ref str1);
                                    if (withBlock1.Unit_Renamed is object)
                                    {
                                        str1 = withBlock1.Unit_Renamed.Name;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // グループＩＤが付けられているおり、なおかつ同じＩＤの
                            // ２番目以降のユニットの場合はグループＩＤで比較
                            if (SRC.PList.IsDefined(ref str1))
                            {
                                Pilot localItem1() { object argIndex1 = str1; var ret = SRC.PList.Item(ref argIndex1); return ret; }

                                str1 = localItem1().ID;
                            }

                            if (Strings.InStr(str1, ":") > 0)
                            {
                                str1 = Strings.Left(str1, Strings.InStr(str1, ":") - 1);
                            }

                            if (Strings.InStr(str2, ":") > 0)
                            {
                                str2 = Strings.Left(str2, Strings.InStr(str2, ":") - 1);
                            }
                        }
                    }

                    // 一致したか？
                    if ((str1 ?? "") != (str2 ?? ""))
                    {
                        if (revrersible & !reversed)
                        {
                            // 対象と相手を入れ替えたイベントラベルが存在するか判定
                            string localListIndex() { string arglist = lab.Data; var ret = GeneralLib.ListIndex(ref arglist, 3); lab.Data = arglist; return ret; }

                            string localListIndex1() { string arglist = lab.Data; var ret = GeneralLib.ListIndex(ref arglist, 2); lab.Data = arglist; return ret; }

                            lname2 = litem[1] + " " + localListIndex() + " " + localListIndex1();
                            if (lab.AsterNum > 0)
                            {
                                lname2 = "*" + lname2;
                            }

                            if (FindLabel(ref lname2) == 0)
                            {
                                // 対象と相手を入れ替えて判定し直す
                                reversed = true;
                                goto CheckPara;
                            }
                        }

                        goto NextLabel;
                    }

                    NextPara:
                    ;
                }

                // ここまでたどり付けばラベルは一致している
                SearchLabelRet = lab.LineNum;

                // 対象と相手を入れ替えて一致した場合はグローバル変数も入れ替え
                if (reversed)
                {
                    tmp_u = SelectedUnitForEvent;
                    SelectedUnitForEvent = SelectedTargetForEvent;
                    SelectedTargetForEvent = tmp_u;
                }

                return SearchLabelRet;
                NextLabel:
                ;
            }

            SearchLabelRet = 0;
            return SearchLabelRet;
        }

        // 指定したイベントへのイベントラベルが定義されているか
        // 常時イベントではない通常イベントのみを探す場合は
        // normal_event_only = True を指定する
        public static bool IsEventDefined(ref string lname, bool normal_event_only = false)
        {
            bool IsEventDefinedRet = default;
            int i, ret;

            // イベントラベルを探す
            i = 0;
            while (1)
            {
                ret = SearchLabel(ref lname, i + 1);
                if (ret == 0)
                {
                    return IsEventDefinedRet;
                }

                if (normal_event_only)
                {
                    // 常時イベントではない通常イベントのみを探す場合
                    if (Strings.Asc(EventData[ret]) != 42) // *
                    {
                        IsEventDefinedRet = true;
                        return IsEventDefinedRet;
                    }
                }
                else
                {
                    IsEventDefinedRet = true;
                    return IsEventDefinedRet;
                }

                i = ret;
            }
        }

        // ラベルが定義されているか
        public static bool IsLabelDefined(ref object Index)
        {
            bool IsLabelDefinedRet = default;
            LabelData lab;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 95875


            Input:

                    On Error GoTo ErrorHandler

             */
            lab = (LabelData)colEventLabelList[Index];
            IsLabelDefinedRet = true;
            return IsLabelDefinedRet;
            ErrorHandler:
            ;
            IsLabelDefinedRet = false;
        }

        // ラベルを追加
        public static void AddLabel(ref string lname, int lnum)
        {
            var new_label = new LabelData();
            string lname2;
            short i;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 96254


            Input:

                    On Error GoTo ErrorHandler

             */
            new_label.Data = lname;
            new_label.LineNum = lnum;
            new_label.Enable = true;
            if (new_label.Name == LabelType.NormalLabel)
            {
                // 通常ラベルを追加
                if (FindNormalLabel0(ref lname) == 0)
                {
                    colNormalLabelList.Add(new_label, lname);
                }
            }
            else
            {
                // イベントラベルを追加

                // パラメータ間の文字列の違いによる不一致をなくすため、
                // 文字列を半角スペース一文字に直しておく
                lname2 = GeneralLib.ListIndex(ref lname, 1);
                var loopTo = GeneralLib.ListLength(ref lname);
                for (i = 2; i <= loopTo; i++)
                    lname2 = lname2 + " " + GeneralLib.ListIndex(ref lname, i);
                bool localIsLabelDefined() { object argIndex1 = lname2; var ret = IsLabelDefined(ref argIndex1); return ret; }

                if (!localIsLabelDefined())
                {
                    colEventLabelList.Add(new_label, lname2);
                }
                else
                {
                    colEventLabelList.Add(new_label, lname2 + "(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lnum) + ")");
                }
            }

            return;
            ErrorHandler:
            ;

            // 通常ラベルが重複定義されている場合は無視
        }

        // システム側のラベルを追加
        public static void AddSysLabel(ref string lname, int lnum)
        {
            var new_label = new LabelData();
            string lname2;
            short i;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo ErrorHandler' at character 97526


            Input:

                    On Error GoTo ErrorHandler

             */
            new_label.Data = lname;
            new_label.LineNum = lnum;
            new_label.Enable = true;
            if (new_label.Name == LabelType.NormalLabel)
            {
                // 通常ラベルを追加
                if (FindSysNormalLabel(ref lname) == 0)
                {
                    colSysNormalLabelList.Add(new_label, lname);
                }
                else
                {
                    // UPGRADE_WARNING: オブジェクト colSysNormalLabelList.Item().LineNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    colSysNormalLabelList[lname].LineNum = lnum;
                }
            }
            else
            {
                // イベントラベルを追加

                // パラメータ間の文字列の違いによる不一致をなくすため、
                // 文字列を半角スペース一文字に直しておく
                lname2 = GeneralLib.ListIndex(ref lname, 1);
                var loopTo = GeneralLib.ListLength(ref lname);
                for (i = 2; i <= loopTo; i++)
                    lname2 = lname2 + " " + GeneralLib.ListIndex(ref lname, i);
                bool localIsLabelDefined() { object argIndex1 = lname2; var ret = IsLabelDefined(ref argIndex1); return ret; }

                if (!localIsLabelDefined())
                {
                    colEventLabelList.Add(new_label, lname2);
                }
                else
                {
                    colEventLabelList.Add(new_label, lname2 + "(" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(lnum) + ")");
                }
            }

            return;
            ErrorHandler:
            ;

            // 通常ラベルが重複定義されている場合は無視
        }

        // ラベルを消去
        public static void ClearLabel(int lnum)
        {
            LabelData lab;
            short i;

            // 行番号lnumにあるラベルを探す
            foreach (LabelData currentLab in colEventLabelList)
            {
                lab = currentLab;
                if (lab.LineNum == lnum)
                {
                    lab.Enable = false;
                    return;
                }
            }

            // lnum行目になければその周りを探す
            for (i = 1; i <= 10; i++)
            {
                foreach (LabelData currentLab1 in colEventLabelList)
                {
                    lab = currentLab1;
                    if (lab.LineNum == lnum - i || lab.LineNum == lnum + i)
                    {
                        lab.Enable = false;
                        return;
                    }
                }
            }
        }

        // ラベルを復活
        public static void RestoreLabel(ref string lname)
        {
            foreach (LabelData lab in colEventLabelList)
            {
                if ((lab.Data ?? "") == (lname ?? ""))
                {
                    lab.Enable = true;
                    return;
                }
            }
        }

        // ラベルを探す
        public static int FindLabel(ref string lname)
        {
            int FindLabelRet = default;
            string lname2;
            short i;

            // 通常ラベルから検索
            FindLabelRet = FindNormalLabel(ref lname);
            if (FindLabelRet > 0)
            {
                return FindLabelRet;
            }

            // イベントラベルから検索
            FindLabelRet = FindEventLabel(ref lname);
            if (FindLabelRet > 0)
            {
                return FindLabelRet;
            }

            // パラメータ間の文字列の違いで一致しなかった可能性があるので
            // 文字列を半角スペース一文字のみにして検索してみる
            lname2 = GeneralLib.ListIndex(ref lname, 1);
            var loopTo = GeneralLib.ListLength(ref lname);
            for (i = 2; i <= loopTo; i++)
                lname2 = lname2 + " " + GeneralLib.ListIndex(ref lname, i);

            // イベントラベルから検索
            FindLabelRet = FindEventLabel(ref lname2);
            return FindLabelRet;
        }

        // イベントラベルを探す
        public static int FindEventLabel(ref string lname)
        {
            int FindEventLabelRet = default;
            LabelData lab;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo NotFound' at character 100733


            Input:

                    On Error GoTo NotFound

             */
            lab = (LabelData)colEventLabelList[lname];
            FindEventLabelRet = lab.LineNum;
            return FindEventLabelRet;
            NotFound:
            ;
            FindEventLabelRet = 0;
        }

        // 通常ラベルを探す
        public static int FindNormalLabel(ref string lname)
        {
            int FindNormalLabelRet = default;
            FindNormalLabelRet = FindNormalLabel0(ref lname);
            if (FindNormalLabelRet == 0)
            {
                FindNormalLabelRet = FindSysNormalLabel(ref lname);
            }

            return FindNormalLabelRet;
        }

        // シナリオ側の通常ラベルを探す
        private static int FindNormalLabel0(ref string lname)
        {
            int FindNormalLabel0Ret = default;
            LabelData lab;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo NotFound' at character 101357


            Input:

                    On Error GoTo NotFound

             */
            lab = (LabelData)colNormalLabelList[lname];
            FindNormalLabel0Ret = lab.LineNum;
            return FindNormalLabel0Ret;
            NotFound:
            ;
            FindNormalLabel0Ret = 0;
        }

        // システム側の通常ラベルを探す
        private static int FindSysNormalLabel(ref string lname)
        {
            int FindSysNormalLabelRet = default;
            LabelData lab;
            ;
#error Cannot convert OnErrorGoToStatementSyntax - see comment for details
            /* Cannot convert OnErrorGoToStatementSyntax, CONVERSION ERROR: Conversion for OnErrorGoToLabelStatement not implemented, please report this issue in 'On Error GoTo NotFound' at character 101696


            Input:

                    On Error GoTo NotFound

             */
            lab = (LabelData)colSysNormalLabelList[lname];
            FindSysNormalLabelRet = lab.LineNum;
            return FindSysNormalLabelRet;
            NotFound:
            ;
            FindSysNormalLabelRet = 0;
        }


        // イベントデータの消去
        // ただしグローバル変数のデータは残しておく
        public static void ClearEventData()
        {
            short i;

            // UPGRADE_NOTE: オブジェクト SelectedUnitForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SelectedUnitForEvent = null;
            // UPGRADE_NOTE: オブジェクト SelectedTargetForEvent をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
            SelectedTargetForEvent = null;
            Array.Resize(ref EventData, SysEventDataSize + 1);
            {
                var withBlock = colNormalLabelList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            i = 1;
            {
                var withBlock1 = colEventLabelList;
                while (i <= withBlock1.Count)
                {
                    // UPGRADE_WARNING: オブジェクト colEventLabelList.Item(i).LineNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    if (Conversions.ToBoolean(Operators.ConditionalCompareObjectGreater(withBlock1[(int)i].LineNum, SysEventDataSize, false)))
                    {
                        withBlock1.Remove(i);
                    }
                    else
                    {
                        i = (short)(i + 1);
                    }
                }
            }

            EventQue = new string[1];
            CallDepth = 0;
            ArgIndex = 0;
            VarIndex = 0;
            ForIndex = 0;
            UpVarLevel = 0;
            HotPointList = new HotPoint[1];
            ObjColor = ColorTranslator.ToOle(Color.White);
            ObjFillColor = ColorTranslator.ToOle(Color.White);
            // UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
            ObjFillStyle = vbFSTransparent;
            ObjDrawWidth = 1;
            ObjDrawOption = "";
            GUI.IsPictureVisible = false;
            GUI.IsCursorVisible = false;
            GUI.PaintedAreaX1 = GUI.MainPWidth;
            GUI.PaintedAreaY1 = GUI.MainPHeight;
            GUI.PaintedAreaX2 = -1;
            GUI.PaintedAreaY2 = -1;
            {
                var withBlock2 = LocalVariableList;
                var loopTo1 = (short)withBlock2.Count;
                for (i = 1; i <= loopTo1; i++)
                    withBlock2.Remove(1);
            }
        }

        // グローバル変数を含めたイベントデータの全消去
        public static void ClearAllEventData()
        {
            short i;
            ClearEventData();
            {
                var withBlock = GlobalVariableList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            Expression.DefineGlobalVariable(ref "次ステージ");
            Expression.DefineGlobalVariable(ref "セーブデータファイル名");
        }


        // 一時中断用データをファイルにセーブする
        public static void DumpEventData()
        {
            short i;

            // グローバル変数
            SaveGlobalVariables();
            // ローカル変数
            SaveLocalVariables();

            // イベント用ラベル
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)colEventLabelList.Count);
            foreach (LabelData lab in colEventLabelList)
                FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)lab.Enable);

            // Requireコマンドで追加されたイベントファイル
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)Information.UBound(AdditionalEventFileNames));
            var loopTo = (short)Information.UBound(AdditionalEventFileNames);
            for (i = 1; i <= loopTo; i++)
                FileSystem.WriteLine(SRC.SaveDataFileNumber, AdditionalEventFileNames[i]);
        }

        // 一時中断用データをファイルからロードする
        public static void RestoreEventData()
        {
            var num = default(short);
            bool lenable;
            var fname = default(string);
            int file_head;
            int i;
            short j;
            string buf;

            // グローバル変数
            LoadGlobalVariables();
            // ローカル変数
            LoadLocalVariables();

            // イベント用ラベル
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            // MOD START MARGE
            // i = 1
            // For Each lab In colEventLabelList
            // If i <= num Then
            // Input #SaveDataFileNumber, lenable
            // lab.Enable = lenable
            // Else
            // lab.Enable = True
            // End If
            // i = i + 1
            // Next
            // Do While i <= num
            // Input #SaveDataFileNumber, buf
            // i = i + 1
            // Loop
            var label_enabled = new object[(num + 1)];
            var loopTo = (int)num;
            for (i = 1; i <= loopTo; i++)
                FileSystem.Input(SRC.SaveDataFileNumber, ref label_enabled[i]);
            // MOD END MARGE

            // Requireコマンドで追加されたイベントファイル
            if (SRC.SaveDataVersion > 20003)
            {
                file_head = Information.UBound(EventData) + 1;

                // MOD START MARGE
                // 'イベントファイルをロード
                // Input #SaveDataFileNumber, num
                // If num = 0 Then
                // Exit Sub
                // End If
                // ReDim AdditionalEventFileNames(num)
                // For i = 1 To num
                // Input #SaveDataFileNumber, fname
                // AdditionalEventFileNames(i) = fname
                // If InStr(fname, ":") = 0 Then
                // fname = ScenarioPath & fname
                // End If
                // 
                // '既に読み込まれている場合はスキップ
                // For j = 1 To UBound(EventFileNames)
                // If fname = EventFileNames(j) Then
                // GoTo NextEventFile
                // End If
                // Next
                // 
                // LoadEventData2 fname, UBound(EventData)
                // NextEventFile:
                // Next
                // 
                // 'エラー表示用にサイズを大きく取っておく
                // ReDim Preserve EventData(UBound(EventData) + 1)
                // ReDim Preserve EventLineNum(UBound(EventData))
                // EventData(UBound(EventData)) = ""
                // EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
                // 
                // '複数行に分割されたコマンドを結合
                // For i = file_head To UBound(EventData) - 1
                // If Right$(EventData(i), 1) = "_" Then
                // EventData(i + 1) = _
                // '                    Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
                // EventData(i) = " "
                // End If
                // Next
                // 
                // 'ラベルを登録
                // For i = file_head To UBound(EventData)
                // buf = EventData(i)
                // If Right$(buf, 1) = ":" Then
                // AddLabel Left$(buf, Len(buf) - 1), i
                // End If
                // Next
                // 
                // 'コマンドデータ配列を設定
                // If UBound(EventData) > UBound(EventCmd) Then
                // ReDim Preserve EventCmd(UBound(EventData))
                // i = UBound(EventData)
                // Do While EventCmd(i) Is Nothing
                // Set EventCmd(i) = New CmdData
                // EventCmd(i).LineNum = i
                // i = i - 1
                // Loop
                // End If
                // For i = file_head To UBound(EventData)
                // EventCmd(i).Name = NullCmd
                // Next
                // End If
                // 追加するイベントファイル数
                FileSystem.Input(SRC.SaveDataFileNumber, ref num);
                if (num > 0)
                {
                    // イベントファイルをロード
                    AdditionalEventFileNames = new string[(num + 1)];
                    var loopTo1 = (int)num;
                    for (i = 1; i <= loopTo1; i++)
                    {
                        FileSystem.Input(SRC.SaveDataFileNumber, ref fname);
                        AdditionalEventFileNames[i] = fname;
                        if (Strings.InStr(fname, ":") == 0)
                        {
                            fname = SRC.ScenarioPath + fname;
                        }

                        // 既に読み込まれている場合はスキップ
                        var loopTo2 = (short)Information.UBound(EventFileNames);
                        for (j = 1; j <= loopTo2; j++)
                        {
                            if ((fname ?? "") == (EventFileNames[j] ?? ""))
                            {
                                goto NextEventFile;
                            }
                        }

                        LoadEventData2(ref fname, Information.UBound(EventData));
                        NextEventFile:
                        ;
                    }

                    // エラー表示用にサイズを大きく取っておく
                    Array.Resize(ref EventData, Information.UBound(EventData) + 1 + 1);
                    Array.Resize(ref EventLineNum, Information.UBound(EventData) + 1);
                    EventData[Information.UBound(EventData)] = "";
                    EventLineNum[Information.UBound(EventData)] = (short)(EventLineNum[Information.UBound(EventData) - 1] + 1);

                    // 複数行に分割されたコマンドを結合
                    var loopTo3 = Information.UBound(EventData) - 1;
                    for (i = file_head; i <= loopTo3; i++)
                    {
                        if (Strings.Right(EventData[i], 1) == "_")
                        {
                            EventData[i + 1] = Strings.Left(EventData[i], Strings.Len(EventData[i]) - 1) + EventData[i + 1];
                            EventData[i] = " ";
                        }
                    }

                    // ラベルを登録
                    var loopTo4 = Information.UBound(EventData);
                    for (i = file_head; i <= loopTo4; i++)
                    {
                        buf = EventData[i];
                        if (Strings.Right(buf, 1) == ":")
                        {
                            AddLabel(ref Strings.Left(buf, Strings.Len(buf) - 1), i);
                        }
                    }

                    // コマンドデータ配列を設定
                    if (Information.UBound(EventData) > Information.UBound(EventCmd))
                    {
                        Array.Resize(ref EventCmd, Information.UBound(EventData) + 1);
                        i = Information.UBound(EventData);
                        while (EventCmd[i] is null)
                        {
                            EventCmd[i] = new CmdData();
                            EventCmd[i].LineNum = i;
                            i = i - 1;
                        }
                    }

                    var loopTo5 = Information.UBound(EventData);
                    for (i = file_head; i <= loopTo5; i++)
                        EventCmd[i].Name = CmdType.NullCmd;
                }
            }

            // イベント用ラベルを設定
            i = 1;
            num = (short)Information.UBound(label_enabled);
            foreach (LabelData lab in colEventLabelList)
            {
                if (i <= num)
                {
                    // UPGRADE_WARNING: オブジェクト label_enabled(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    lab.Enable = Conversions.ToBoolean(label_enabled[i]);
                }
                else
                {
                    lab.Enable = true;
                }

                i = i + 1;
            }
            // MOD END MARGE
        }

        // 一時中断用データのイベントデータ部分を読み飛ばす
        public static void SkipEventData()
        {
            short i, num = default;
            string dummy;

            // グローバル変数
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo = num;
            for (i = 1; i <= loopTo; i++)
                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
            // ローカル変数
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo1 = num;
            for (i = 1; i <= loopTo1; i++)
                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // ラベル情報
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo2 = num;
            for (i = 1; i <= loopTo2; i++)
                dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);

            // Requireコマンドで読み込んだイベントデータ
            if (SRC.SaveDataVersion > 20003)
            {
                FileSystem.Input(SRC.SaveDataFileNumber, ref num);
                var loopTo3 = num;
                for (i = 1; i <= loopTo3; i++)
                    dummy = FileSystem.LineInput(SRC.SaveDataFileNumber);
            }
        }

        // グローバル変数をファイルにセーブ
        public static void SaveGlobalVariables()
        {
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)GlobalVariableList.Count);
            foreach (VarData var in GlobalVariableList)
            {
                if (var.VariableType == Expression.ValueType.StringType)
                {
                    FileSystem.WriteLine(SRC.SaveDataFileNumber, var.Name, var.StringValue);
                }
                else
                {
                    FileSystem.WriteLine(SRC.SaveDataFileNumber, var.Name, Microsoft.VisualBasic.Compatibility.VB6.Support.Format(var.NumericValue));
                }
            }
        }

        // グローバル変数をファイルからロード
        public static void LoadGlobalVariables()
        {
            short num = default, j, i, k, idx;
            string vvalue, vname = default, buf;
            string aname;
            // ADD START MARGE
            bool is_number;
            // ADD END MARGE
            // グローバル変数を全削除
            {
                var withBlock = GlobalVariableList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            // グローバル変数の総数を読み出し
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);

            // 各変数の値を読み出し
            string vname2;
            var loopTo1 = num;
            for (i = 1; i <= loopTo1; i++)
            {
                FileSystem.Input(SRC.SaveDataFileNumber, ref vname);
                buf = FileSystem.LineInput(SRC.SaveDataFileNumber);
                // MOD START MARGE
                // vvalue = Mid$(buf, 2, Len(buf) - 2)
                // ReplaceString vvalue, """""", """"
                if (Strings.Left(buf, 1) == "\"")
                {
                    is_number = false;
                    vvalue = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                    GeneralLib.ReplaceString(ref vvalue, ref "\"\"", ref "\"");
                }
                else
                {
                    is_number = true;
                    vvalue = buf;
                }
                // MOD END MARGE

                if (SRC.SaveDataVersion < 10724)
                {
                    // SetSkillコマンドのセーブデータをエリアスに対応させる
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        idx = (short)Strings.InStr(vname, ",");
                        if (idx > 0)
                        {
                            // 個々の能力定義
                            aname = Strings.Mid(vname, idx + 1, Strings.Len(vname) - idx - 1);
                            if (SRC.ALDList.IsDefined(ref aname))
                            {
                                AliasDataType localItem() { object argIndex1 = aname; var ret = SRC.ALDList.Item(ref argIndex1); return ret; }

                                vname = Strings.Left(vname, idx) + localItem().get_AliasType(1) + ")";
                                if (GeneralLib.LLength(ref vvalue) == 1)
                                {
                                    vvalue = vvalue + " " + aname;
                                }
                            }
                        }
                        else
                        {
                            // 必要技能用の能力一覧
                            buf = "";
                            var loopTo2 = GeneralLib.LLength(ref vvalue);
                            for (j = 1; j <= loopTo2; j++)
                            {
                                aname = GeneralLib.LIndex(ref vvalue, j);
                                if (SRC.ALDList.IsDefined(ref aname))
                                {
                                    AliasDataType localItem1() { object argIndex1 = aname; var ret = SRC.ALDList.Item(ref argIndex1); return ret; }

                                    aname = localItem1().get_AliasType(1);
                                }

                                buf = buf + " " + aname;
                            }

                            vvalue = Strings.Trim(buf);
                        }
                    }
                }

                if (SRC.SaveDataVersion < 10730)
                {
                    // ラーニングした特殊能力が使えないバグに対応
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        idx = (short)Strings.InStr(vname, ",");
                        if (idx > 0)
                        {
                            vname2 = Strings.Left(vname, idx - 1) + ")";
                            aname = Strings.Mid(vname, idx + 1, Strings.Len(vname) - idx - 1);
                            if (!Expression.IsGlobalVariableDefined(ref vname2))
                            {
                                Expression.DefineGlobalVariable(ref vname2);
                                // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                GlobalVariableList[vname2].StringValue = aname;
                            }
                        }
                    }
                }

                if (SRC.SaveDataVersion < 10731)
                {
                    // 不必要な非表示能力に対するSetSkillを削除
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if (Strings.Right(vname, 5) == ",非表示)")
                        {
                            goto NextVariable;
                        }
                    }
                }

                if (SRC.SaveDataVersion < 10732)
                {
                    // 不必要な非表示能力に対するSetSkillと能力名のダブりを削除
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if (Strings.InStr(vname, ",") == 0)
                        {
                            buf = "";
                            var loopTo3 = GeneralLib.LLength(ref vvalue);
                            for (j = 1; j <= loopTo3; j++)
                            {
                                aname = GeneralLib.LIndex(ref vvalue, j);
                                if (aname != "非表示")
                                {
                                    var loopTo4 = GeneralLib.LLength(ref buf);
                                    for (k = 1; k <= loopTo4; k++)
                                    {
                                        if ((GeneralLib.LIndex(ref buf, k) ?? "") == (aname ?? ""))
                                        {
                                            break;
                                        }
                                    }

                                    if (k > GeneralLib.LLength(ref buf))
                                    {
                                        buf = buf + " " + aname;
                                    }
                                }
                            }

                            vvalue = Strings.Trim(buf);
                        }
                    }
                }

                if (SRC.SaveDataVersion < 20027)
                {
                    // エリアスされた能力をSetSkillした際にエリアスに含まれる解説が無効になるバグへの対処
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if (GeneralLib.LIndex(ref vvalue, 1) == "0")
                        {
                            if (GeneralLib.LIndex(ref vvalue, 2) == "解説")
                            {
                                vvalue = Microsoft.VisualBasic.Compatibility.VB6.Support.Format(SRC.DEFAULT_LEVEL) + " 解説 " + GeneralLib.ListTail(ref vvalue, 3);
                            }
                        }
                    }
                }

                if (!Expression.IsGlobalVariableDefined(ref vname))
                {
                    Expression.DefineGlobalVariable(ref vname);
                }

                {
                    var withBlock1 = GlobalVariableList[vname];
                    // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock1.StringValue = vvalue;
                    // MOD START MARGE
                    // If IsNumber(vvalue) Then
                    if (is_number)
                    {
                        // MOD END MARGE
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.VariableType = Expression.ValueType.NumericType;
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.NumericValue = Conversions.ToDouble(vvalue);
                    }
                    else
                    {
                        // UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.VariableType = Expression.ValueType.StringType;
                    }
                }

                NextVariable:
                ;
            }
            // ADD START 240a
            // Optionを全て読み込んだら、新ＧＵＩが有効になっているか確認する
            GUI.SetNewGUIMode();
            // ADD  END  240a
        }

        // ローカル変数をファイルにセーブ
        public static void SaveLocalVariables()
        {
            FileSystem.WriteLine(SRC.SaveDataFileNumber, (object)LocalVariableList.Count);
            foreach (VarData var in LocalVariableList)
            {
                if (var.VariableType == Expression.ValueType.StringType)
                {
                    FileSystem.WriteLine(SRC.SaveDataFileNumber, var.Name, var.StringValue);
                }
                else
                {
                    FileSystem.WriteLine(SRC.SaveDataFileNumber, var.Name, Microsoft.VisualBasic.Compatibility.VB6.Support.Format(var.NumericValue));
                }

                if (Strings.InStr(var.Name, "\"") > 0)
                {
                    GUI.ErrorMessage(ref var.Name);
                }
            }
        }

        // ローカル変数をファイルからロード
        public static void LoadLocalVariables()
        {
            short i, num = default;
            // MOD START MARGE
            // Dim vname As String, vvalue As String
            string vvalue, vname = default, buf;
            bool is_number;
            // MOD END MARGE
            // ローカル変数を全削除
            {
                var withBlock = LocalVariableList;
                var loopTo = (short)withBlock.Count;
                for (i = 1; i <= loopTo; i++)
                    withBlock.Remove(1);
            }

            // ローカル変数の総数を読み出し
            FileSystem.Input(SRC.SaveDataFileNumber, ref num);
            var loopTo1 = num;
            for (i = 1; i <= loopTo1; i++)
            {
                // 変数の値を読み出し
                // MOD START MARGE
                // Input #SaveDataFileNumber, vname, vvalue
                FileSystem.Input(SRC.SaveDataFileNumber, ref vname);
                buf = FileSystem.LineInput(SRC.SaveDataFileNumber);
                if (Strings.Left(buf, 1) == "\"")
                {
                    is_number = false;
                    vvalue = Strings.Mid(buf, 2, Strings.Len(buf) - 2);
                    GeneralLib.ReplaceString(ref vvalue, ref "\"\"", ref "\"");
                }
                else
                {
                    is_number = true;
                    vvalue = buf;
                }
                // MOD END MARGE

                if (SRC.SaveDataVersion < 10731)
                {
                    // ClearSkillのバグで設定された変数を削除
                    if (Strings.Left(vname, 8) == "Ability(")
                    {
                        if ((vname ?? "") == (vvalue ?? ""))
                        {
                            goto NextVariable;
                        }
                    }
                }

                // 変数の値を設定
                if (!Expression.IsLocalVariableDefined(ref vname))
                {
                    Expression.DefineLocalVariable(ref vname);
                }

                {
                    var withBlock1 = LocalVariableList[vname];
                    // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    withBlock1.StringValue = vvalue;
                    // MOD START MARGE
                    // If IsNumber(vvalue) Then
                    if (is_number)
                    {
                        // MOD END MARGE
                        // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.VariableType = Expression.ValueType.NumericType;
                        // UPGRADE_WARNING: オブジェクト LocalVariableList.Item(vname).NumericValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.NumericValue = Conversions.ToDouble(vvalue);
                    }
                    else
                    {
                        // UPGRADE_WARNING: オブジェクト LocalVariableList.Item().VariableType の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        withBlock1.VariableType = Expression.ValueType.StringType;
                    }
                }

                NextVariable:
                ;
            }
        }


        // イベントエラー表示
        public static void DisplayEventErrorMessage(int lnum, string msg)
        {
            string buf;

            // エラーが起こったファイル、行番号、エラーメッセージを表示
            buf = EventFileNames[EventFileID[lnum]] + "：" + EventLineNum[lnum] + "行目" + Constants.vbCr + Constants.vbLf + msg + Constants.vbCr + Constants.vbLf;

            // エラーが起こった行とその前後の行の内容を表示
            if (lnum > 1)
            {
                buf = buf + EventLineNum[lnum - 1] + ": " + EventData[lnum - 1] + Constants.vbCr + Constants.vbLf;
            }

            buf = buf + EventLineNum[lnum] + ": " + EventData[lnum] + Constants.vbCr + Constants.vbLf;
            if (lnum < Information.UBound(EventData))
            {
                buf = buf + EventLineNum[lnum + 1] + ": " + EventData[lnum + 1] + Constants.vbCr + Constants.vbLf;
            }

            GUI.ErrorMessage(ref buf);
        }

        // インターミッションコマンド「ユニットリスト」におけるユニットリストを作成する
        public static void MakeUnitList([Optional, DefaultParameterValue("")] ref string smode)
        {
            Unit u;
            Pilot p;
            short xx, yy;
            var key_list = default(int[]);
            short max_item;
            int max_value;
            string max_str;
            Unit[] unit_list;
            short i, j;
            ;

            // リストのソート項目を設定
            if (!string.IsNullOrEmpty(smode))
            {
                key_type = smode;
            }

            if (string.IsNullOrEmpty(key_type))
            {
                key_type = "ＨＰ";
            }

            // マウスカーソルを砂時計に
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.WaitCursor;

            // あらかじめ撤退させておく
            foreach (Unit currentU in SRC.UList)
            {
                u = currentU;
                {
                    var withBlock = u;
                    if (withBlock.Status_Renamed == "出撃")
                    {
                        withBlock.Escape();
                    }
                }
            }

            // マップをクリア
            Map.LoadMapData(ref "");
            GUI.SetupBackground(ref "", ref "ステータス", filter_color: ref 0, filter_trans_par: ref 0d);

            // ユニット一覧を作成
            if (key_type != "名称")
            {
                // 配列作成
                unit_list = new Unit[(SRC.UList.Count() + 1)];
                key_list = new int[(SRC.UList.Count() + 1)];
                i = 0;
                foreach (Unit currentU1 in SRC.UList)
                {
                    u = currentU1;
                    {
                        var withBlock1 = u;
                        if (withBlock1.Status_Renamed == "出撃" || withBlock1.Status_Renamed == "待機")
                        {
                            i = (short)(i + 1);
                            unit_list[i] = u;

                            // ソートする項目にあわせてソートの際の優先度を決定
                            switch (key_type ?? "")
                            {
                                case "ランク":
                                    {
                                        key_list[i] = withBlock1.Rank;
                                        break;
                                    }

                                case "ＨＰ":
                                    {
                                        key_list[i] = withBlock1.HP;
                                        break;
                                    }

                                case "ＥＮ":
                                    {
                                        key_list[i] = withBlock1.EN;
                                        break;
                                    }

                                case "装甲":
                                    {
                                        key_list[i] = withBlock1.get_Armor("");
                                        break;
                                    }

                                case "運動性":
                                    {
                                        key_list[i] = withBlock1.get_Mobility("");
                                        break;
                                    }

                                case "移動力":
                                    {
                                        key_list[i] = withBlock1.Speed;
                                        break;
                                    }

                                case "最大攻撃力":
                                    {
                                        var loopTo = withBlock1.CountWeapon();
                                        for (j = 1; j <= loopTo; j++)
                                        {
                                            if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(ref withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, ref "合"))
                                            {
                                                if (withBlock1.WeaponPower(j, ref "") > key_list[i])
                                                {
                                                    key_list[i] = withBlock1.WeaponPower(j, ref "");
                                                }
                                            }
                                        }

                                        break;
                                    }

                                case "最長射程":
                                    {
                                        var loopTo1 = withBlock1.CountWeapon();
                                        for (j = 1; j <= loopTo1; j++)
                                        {
                                            if (withBlock1.IsWeaponMastered(j) & !withBlock1.IsDisabled(ref withBlock1.Weapon(j).Name) & !withBlock1.IsWeaponClassifiedAs(j, ref "合"))
                                            {
                                                if (withBlock1.WeaponMaxRange(j) > key_list[i])
                                                {
                                                    key_list[i] = withBlock1.WeaponMaxRange(j);
                                                }
                                            }
                                        }

                                        break;
                                    }

                                case "レベル":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Level;
                                        break;
                                    }

                                case "ＳＰ":
                                    {
                                        key_list[i] = withBlock1.MainPilot().MaxSP;
                                        break;
                                    }

                                case "格闘":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Infight;
                                        break;
                                    }

                                case "射撃":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Shooting;
                                        break;
                                    }

                                case "命中":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Hit;
                                        break;
                                    }

                                case "回避":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Dodge;
                                        break;
                                    }

                                case "技量":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Technique;
                                        break;
                                    }

                                case "反応":
                                    {
                                        key_list[i] = withBlock1.MainPilot().Intuition;
                                        break;
                                    }
                            }
                        }
                    }
                }

                Array.Resize(ref unit_list, i + 1);
                Array.Resize(ref key_list, i + 1);

                // ソート
                var loopTo2 = (short)(Information.UBound(key_list) - 1);
                for (i = 1; i <= loopTo2; i++)
                {
                    max_item = i;
                    max_value = key_list[i];
                    var loopTo3 = (short)Information.UBound(unit_list);
                    for (j = (short)(i + 1); j <= loopTo3; j++)
                    {
                        if (key_list[j] > max_value)
                        {
                            max_item = j;
                            max_value = key_list[j];
                        }
                    }

                    if (max_item != i)
                    {
                        u = unit_list[i];
                        unit_list[i] = unit_list[max_item];
                        unit_list[max_item] = u;
                        max_value = key_list[max_item];
                        key_list[max_item] = key_list[i];
                        key_list[i] = max_value;
                    }
                }
            }
            else
            {
                // 配列作成
                unit_list = new Unit[(SRC.UList.Count() + 1)];
                var strkey_list = new object[(SRC.UList.Count() + 1)];
                i = 0;
                foreach (Unit currentU2 in SRC.UList)
                {
                    u = currentU2;
                    {
                        var withBlock2 = u;
                        if (withBlock2.Status_Renamed == "出撃" || withBlock2.Status_Renamed == "待機")
                        {
                            i = (short)(i + 1);
                            unit_list[i] = u;
                            if (Expression.IsOptionDefined(ref "等身大基準"))
                            {
                                // UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                strkey_list[i] = withBlock2.MainPilot().KanaName;
                            }
                            else
                            {
                                // UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                                strkey_list[i] = withBlock2.KanaName;
                            }
                        }
                    }
                }

                Array.Resize(ref unit_list, i + 1);
                Array.Resize(ref strkey_list, i + 1);

                // ソート
                var loopTo4 = (short)(Information.UBound(strkey_list) - 1);
                for (i = 1; i <= loopTo4; i++)
                {
                    max_item = i;
                    // UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                    max_str = Conversions.ToString(strkey_list[i]);
                    var loopTo5 = (short)Information.UBound(strkey_list);
                    for (j = (short)(i + 1); j <= loopTo5; j++)
                    {
                        // UPGRADE_WARNING: オブジェクト strkey_list() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        if (Strings.StrComp(Conversions.ToString(strkey_list[j]), max_str, (CompareMethod)1) == -1)
                        {
                            max_item = j;
                            // UPGRADE_WARNING: オブジェクト strkey_list(j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                            max_str = Conversions.ToString(strkey_list[j]);
                        }
                    }

                    if (max_item != i)
                    {
                        u = unit_list[i];
                        unit_list[i] = unit_list[max_item];
                        unit_list[max_item] = u;

                        // UPGRADE_WARNING: オブジェクト strkey_list(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        // UPGRADE_WARNING: オブジェクト strkey_list(max_item) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
                        strkey_list[max_item] = strkey_list[i];
                    }
                }
            }

            // Font Regular 9pt 背景
            // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            {
                var withBlock3 = GUI.MainForm.picMain(0).Font;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock3.Size = 9;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock3.Bold = false;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock3.Italic = false;
            }

            GUI.PermanentStringMode = true;
            GUI.HCentering = false;
            GUI.VCentering = false;

            // ユニットのリストを作成
            xx = 1;
            yy = 1;
            var loopTo6 = (short)Information.UBound(unit_list);
            for (i = 1; i <= loopTo6; i++)
            {
                u = unit_list[i];
                {
                    var withBlock4 = u;
                    // ユニット出撃位置を折り返す
                    if (xx > 15)
                    {
                        xx = 1;
                        yy = (short)(yy + 1);
                        if (yy > 40)
                        {
                            // ユニット数が多すぎるため、一部のパイロットが表示出来ません
                            break;
                        }
                    }

                    // パイロットが乗っていない場合はダミーパイロットを乗せる
                    if (withBlock4.CountPilot() == 0)
                    {
                        p = SRC.PList.Add(ref "ステータス表示用ダミーパイロット(ザコ)", 1, ref "味方", gid: ref "");
                        p.Ride(ref u);
                    }

                    // 出撃
                    withBlock4.UsedAction = 0;
                    withBlock4.StandBy(xx, yy);

                    // プレイヤーが操作できないように
                    withBlock4.AddCondition(ref "非操作", -1, cdata: ref "");

                    // ユニットの愛称を表示
                    GUI.DrawString(ref withBlock4.Nickname, 32 * xx + 2, 32 * yy - 31);
                    withBlock4.Nickname = argmsg;

                    // ソート項目にあわせてユニットのステータスを表示
                    switch (key_type ?? "")
                    {
                        case "ランク":
                            {
                                GUI.DrawString(ref "RK" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]) + " " + Expression.Term(ref "HP", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP) + " " + Expression.Term(ref "EN", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.EN), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "ＨＰ":
                        case "ＥＮ":
                        case "名称":
                            {
                                GUI.DrawString(ref Expression.Term(ref "HP", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.HP) + " " + Expression.Term(ref "EN", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(withBlock4.EN), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "装甲":
                            {
                                GUI.DrawString(ref Expression.Term(ref "装甲", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "運動性":
                            {
                                GUI.DrawString(ref Expression.Term(ref "運動性", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "移動力":
                            {
                                GUI.DrawString(ref Expression.Term(ref "移動力", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "最大攻撃力":
                            {
                                GUI.DrawString(ref "攻撃力" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "最長射程":
                            {
                                GUI.DrawString(ref "射程" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "レベル":
                            {
                                GUI.DrawString(ref "Lv" + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "ＳＰ":
                            {
                                GUI.DrawString(ref Expression.Term(ref "SP", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "格闘":
                            {
                                GUI.DrawString(ref Expression.Term(ref "格闘", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "射撃":
                            {
                                if (withBlock4.MainPilot().HasMana())
                                {
                                    GUI.DrawString(ref Expression.Term(ref "魔力", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                }
                                else
                                {
                                    GUI.DrawString(ref Expression.Term(ref "射撃", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                }

                                break;
                            }

                        case "命中":
                            {
                                GUI.DrawString(ref Expression.Term(ref "命中", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "回避":
                            {
                                GUI.DrawString(ref Expression.Term(ref "回避", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "技量":
                            {
                                GUI.DrawString(ref Expression.Term(ref "技量", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }

                        case "反応":
                            {
                                GUI.DrawString(ref Expression.Term(ref "反応", ref u) + Microsoft.VisualBasic.Compatibility.VB6.Support.Format(key_list[i]), 32 * xx + 2, 32 * yy - 15);
                                break;
                            }
                    }

                    // 表示位置を右に5マスずらす
                    xx = (short)(xx + 5);
                }
            }

            // フォントの設定を戻しておく
            // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
            {
                var withBlock5 = GUI.MainForm.picMain(0).Font;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock5.Size = 16;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock5.Bold = true;
                // UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
                withBlock5.Italic = false;
            }

            GUI.PermanentStringMode = false;
            GUI.RedrawScreen();

            // マウスカーソルを元に戻す
            // UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
            Cursor.Current = Cursors.Default;
        }


        // 描画の基準座標位置を保存
        public static void SaveBasePoint()
        {
            BasePointIndex = BasePointIndex + 1;
            if (BasePointIndex > Information.UBound(SavedBaseX))
            {
                BasePointIndex = 0;
            }

            SavedBaseX[BasePointIndex] = BaseX;
            SavedBaseY[BasePointIndex] = BaseY;
        }

        // 描画の基準座標位置を復元
        public static void RestoreBasePoint()
        {
            if (BasePointIndex <= 0)
            {
                BasePointIndex = Information.UBound(SavedBaseX);
            }

            BaseX = SavedBaseX[BasePointIndex];
            BaseY = SavedBaseY[BasePointIndex];
            BasePointIndex = BasePointIndex - 1;
        }

        // 描画の基準座標位置をリセット
        public static void ResetBasePoint()
        {
            short i;
            BaseX = 0;
            BaseY = 0;
            BasePointIndex = 0;
            var loopTo = (short)Information.UBound(SavedBaseX);
            for (i = 1; i <= loopTo; i++)
            {
                SavedBaseX[i] = 0;
                SavedBaseY[i] = 0;
            }
        }
    }
}
