Attribute VB_Name = "Event"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'イベントデータの各種処理を行うモジュール

'イベントデータ
Public EventData() As String
'イベントコマンドリスト
Public EventCmd() As CmdData
'個々の行がどのイベントファイルに属しているか
Public EventFileID() As Integer
'個々の行がイベントファイルの何行目に位置するか
Public EventLineNum() As Integer
'イベントファイルのファイル名リスト
Public EventFileNames() As String
'Requireコマンドで追加されたイベントファイルのファイル名リスト
Public AdditionalEventFileNames() As String

'システム側のイベントデータのサイズ(行数)
Private SysEventDataSize As Long
'システム側のイベントファイル数
Private SysEventFileNum As Integer
'シナリオ添付のシステムファイルがチェックされたかどうか
Private ScenarioLibChecked As Boolean

'ラベルのリスト
Public colEventLabelList As New Collection
Private colSysNormalLabelList As New Collection
Private colNormalLabelList As New Collection


'変数用のコレクション
Public GlobalVariableList As New Collection
Public LocalVariableList As New Collection

'現在の行番号
Public CurrentLineNum As Long

'イベントで選択されているユニット・ターゲット
Public SelectedUnitForEvent As Unit
Public SelectedTargetForEvent As Unit

'イベント呼び出しのキュー
Public EventQue() As String
'現在実行中のイベントラベル
Public CurrentLabel As Long

'Askコマンドで選択した選択肢
Public SelectedAlternative As String

'関数呼び出し用変数

'最大呼び出し階層数
Public Const MaxCallDepth = 50
'引数の最大数
Public Const MaxArgIndex = 200
'サブルーチンローカル変数の最大数
Public Const MaxVarIndex = 2000

'呼び出し履歴
Public CallDepth As Integer
Public CallStack(MaxCallDepth) As Long
'引数スタック
Public ArgIndex As Integer
Public ArgIndexStack(MaxCallDepth) As Integer
Public ArgStack(MaxArgIndex) As String
'UpVarコマンドによって引数が何段階シフトしているか
Public UpVarLevel As Integer
Public UpVarLevelStack(MaxCallDepth) As Integer
'サブルーチンローカル変数スタック
Public VarIndex As Integer
Public VarIndexStack(MaxCallDepth) As Integer
Public VarStack(MaxVarIndex) As VarData
'Forインデックス用スタック
Public ForIndex As Integer
Public ForIndexStack(MaxCallDepth) As Integer
Public ForLimitStack(MaxCallDepth) As Long

'ForEachコマンド用変数
Public ForEachIndex As Integer
Public ForEachSet() As String

'Rideコマンド用パイロット搭乗履歴
Public LastUnitName As String
Public LastPilotID() As String

'Wait開始時刻
Public WaitStartTime As Long
Public WaitTimeCount As Long

'描画基準座標
Public BaseX As Long
Public BaseY As Long
Private SavedBaseX(10) As Long
Private SavedBaseY(10) As Long
Private BasePointIndex As Long

'オブジェクトの色
Public ObjColor As Long
'オブジェクトの線の太さ
Public ObjDrawWidth As Long
'オブジェクトの背景色
Public ObjFillColor As Long
'オブジェクトの背景描画方法
Public ObjFillStyle As Long
'オブジェクトの描画方法
Public ObjDrawOption As String

'ホットポイント
Public Type HotPoint
    Name As String
    Left As Integer
    Top As Integer
    width As Integer
    Height As Integer
    Caption As String
End Type
Public HotPointList() As HotPoint

'イベントコマンドエラーメッセージ
Public EventErrorMessage As String

'ユニットがセンタリングされたか？
Public IsUnitCenter As Boolean


'イベントコマンドの種類
Enum CmdType
    NullCmd = 0
    NopCmd
    ArcCmd
    ArrayCmd
    AskCmd
    AttackCmd
    AutoTalkCmd
    BossRankCmd
    BreakCmd
    CallCmd
    ReturnCmd
    CallInterMissionCommandCmd
    CancelCmd
    CenterCmd
    ChangeAreaCmd
    ChangeLayerCmd
    ChangeMapCmd
    ChangeModeCmd
    ChangePartyCmd
    ChangeTerrainCmd
    ChangeUnitBitmapCmd
    ChargeCmd
    CircleCmd
    ClearEventCmd
    ClearImageCmd
    ClearLayerCmd
    ClearObjCmd
    ClearPictureCmd
    ClearSkillCmd
    ClearSpecialPowerCmd
    ClearStatusCmd
    CloseCmd
    ClsCmd
    ColorCmd
    ColorFilterCmd
    CombineCmd
    ConfirmCmd
    ContinueCmd
    CopyArrayCmd
    CopyFileCmd
    CreateCmd
    CreateFolderCmd
    DebugCmd
    DestroyCmd
    DisableCmd
    DoCmd
    LoopCmd
    DrawOptionCmd
    DrawWidthCmd
    EnableCmd
    EquipCmd
    EscapeCmd
    ExchangeItemCmd
    ExecCmd
    ExitCmd
    ExplodeCmd
    ExpUpCmd
    FadeInCmd
    FadeOutCmd
    FillColorCmd
    FillStyleCmd
    FinishCmd
    FixCmd
    FontCmd
    ForCmd
    ForEachCmd
    NextCmd
    ForgetCmd
    GameClearCmd
    GameOverCmd
    FreeMemoryCmd
    GetOffCmd
    GlobalCmd
    GotoCmd
    HideCmd
    HotPointCmd
    IfCmd
    ElseCmd
    ElseIfCmd
    EndIfCmd
    IncrCmd
    IncreaseMoraleCmd
    InputCmd
    IntermissionCommandCmd
    ItemCmd
    JoinCmd
    KeepBGMCmd
    LandCmd
    LaunchCmd
    LeaveCmd
    LevelUpCmd
    LineCmd
    LineReadCmd
    LoadCmd
    LocalCmd
    MakePilotListCmd
    MakeUnitListCmd
    MapAbilityCmd
    MapAttackCmd
    MoneyCmd
    MonotoneCmd
    MoveCmd
    NightCmd
    NoonCmd
    OpenCmd
    OptionCmd
    OrganizeCmd
    OvalCmd
    PaintPictureCmd
    PaintStringCmd
    PaintStringRCmd
    PaintSysStringCmd
    PilotCmd
    PlayMIDICmd
    PlaySoundCmd
    PolygonCmd
    PrintCmd
    PSetCmd
    QuestionCmd
    QuickLoadCmd
    QuitCmd
    RankUpCmd
    ReadCmd
    RecoverENCmd
    RecoverHPCmd
    RecoverPlanaCmd
    RecoverSPCmd
    RedrawCmd
    RefreshCmd
    ReleaseCmd
    RemoveFileCmd
    RemoveFolderCmd
    RemoveItemCmd
    RemovePilotCmd
    RemoveUnitCmd
    RenameBGMCmd
    RenameFileCmd
    RenameTermCmd
    ReplacePilotCmd
    RequireCmd
    RestoreEventCmd
    RideCmd
    SaveDataCmd
    SelectCmd
    SelectTargetCmd
    SepiaCmd
    SetCmd
    SetSkillCmd
    SetBulletCmd
    SetMessageCmd
    SetRelationCmd
    SetStatusStringColorCmd
    SetStatusCmd
    SetStockCmd
    SetWindowColorCmd
    SetWindowFrameWidthCmd
    ShowCmd
    ShowImageCmd
    ShowUnitStatusCmd
    SkipCmd
    SortCmd
    SpecialPowerCmd
    SplitCmd
    StartBGMCmd
    StopBGMCmd
    StopSummoningCmd
    SupplyCmd
    SunsetCmd
    SwapCmd
    SwitchCmd
    CaseCmd
    CaseElseCmd
    EndSwCmd
    TalkCmd
    EndCmd
    SuspendCmd
    TelopCmd
    TransformCmd
    UnitCmd
    UnsetCmd
    UpgradeCmd
    UpVarCmd
    UseAbilityCmd
    WaitCmd
    WaterCmd
    WhiteInCmd
    WhiteOutCmd
    WriteCmd
    PlayFlashCmd
    ClearFlashCmd
End Enum

'イベントラベルの種類
Enum LabelType
    NormalLabel = 0
    PrologueEventLabel
    StartEventLabel
    EpilogueEventLabel
    TurnEventLabel
    DamageEventLabel
    DestructionEventLabel
    TotalDestructionEventLabel
    AttackEventLabel
    AfterAttackEventLabel
    TalkEventLabel
    ContactEventLabel
    EnterEventLabel
    EscapeEventLabel
    LandEventLabel
    UseEventLabel
    AfterUseEventLabel
    TransformEventLabel
    CombineEventLabel
    SplitEventLabel
    FinishEventLabel
    LevelUpEventLabel
    RequirementEventLabel
    ResumeEventLabel
    MapCommandEventLabel
    UnitCommandEventLabel
    EffectEventLabel
End Enum


'イベントデータを初期化
Public Sub InitEventData()
Dim i As Long

    ReDim Titles(0)
    ReDim EventData(0)
    ReDim EventCmd(50000)
    ReDim EventQue(0)
    
    'オブジェクトの生成には時間がかかるので、
    'あらかじめCmdDataオブジェクトを生成しておく。
    For i = 1 To UBound(EventCmd)
        Set EventCmd(i) = New CmdData
        EventCmd(i).LineNum = i
    Next
    
    '本体側のシナリオデータをチェックする
    LoadEventData "", "システム"
End Sub

'イベントファイルのロード
Public Sub LoadEventData(fname As String, Optional load_mode As String)
Dim buf As String, buf2 As String
Dim tname As String, tfolder As String, new_titles() As String
Dim i As Long, j As Integer, num As Long
Dim CmdStack(50) As CmdType, CmdStackIdx As Integer
Dim CmdPosStack(50) As Long, CmdPosStackIdx As Integer
Dim error_found As Boolean
Dim sys_event_data_size As Long
Dim sys_event_file_num As Long
    
    'データの初期化
    ReDim Preserve EventData(SysEventDataSize)
    ReDim Preserve EventFileID(SysEventDataSize)
    ReDim Preserve EventLineNum(SysEventDataSize)
    ReDim Preserve EventFileNames(SysEventFileNum)
    ReDim AdditionalEventFileNames(0)
    CurrentLineNum = SysEventDataSize
    CallDepth = 0
    ArgIndex = 0
    UpVarLevel = 0
    VarIndex = 0
    If VarStack(1) Is Nothing Then
        For i = 1 To UBound(VarStack)
            Set VarStack(i) = New VarData
        Next
    End If
    ForIndex = 0
    ReDim new_titles(0)
    ReDim HotPointList(0)
    ObjColor = vbWhite
    ObjFillColor = vbWhite
    ObjFillStyle = vbFSTransparent
    ObjDrawWidth = 1
    ObjDrawOption = ""
    
    'ラベルの初期化
    With colNormalLabelList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    i = 1
    With colEventLabelList
        Do While i <= .Count
            If .Item(i).LineNum > SysEventDataSize Then
                .Remove i
            Else
                i = i + 1
            End If
        Loop
    End With
    
    'デバッグモードの設定
    If LCase$(ReadIni("Option", "DebugMode")) = "on" Then
        If Not IsOptionDefined("デバッグ") Then
            DefineGlobalVariable "Option(デバッグ)"
        End If
        SetVariableAsLong "Option(デバッグ)", 1
    End If
    
    'システム側のイベントデータのロード
    If load_mode = "システム" Then
        '本体側のシステムデータをチェック
        
        'スペシャルパワーアニメ用インクルードファイルをダウンロード
        If FileExists(ExtDataPath & "Lib\スペシャルパワー.eve") Then
            LoadEventData2 ExtDataPath & "Lib\スペシャルパワー.eve"
        ElseIf FileExists(ExtDataPath2 & "Lib\スペシャルパワー.eve") Then
            LoadEventData2 ExtDataPath2 & "Lib\スペシャルパワー.eve"
        ElseIf FileExists(AppPath & "Lib\スペシャルパワー.eve") Then
            LoadEventData2 AppPath & "Lib\スペシャルパワー.eve"
        ElseIf FileExists(ExtDataPath & "Lib\精神コマンド.eve") Then
            LoadEventData2 ExtDataPath & "Lib\精神コマンド.eve"
        ElseIf FileExists(ExtDataPath2 & "Lib\精神コマンド.eve") Then
            LoadEventData2 ExtDataPath2 & "Lib\精神コマンド.eve"
        ElseIf FileExists(AppPath & "Lib\精神コマンド.eve") Then
            LoadEventData2 AppPath & "Lib\精神コマンド.eve"
        End If
        
        '汎用戦闘アニメ用インクルードファイルをダウンロード
        If LCase$(ReadIni("Option", "BattleAnimation")) <> "off" Then
            BattleAnimation = True
        End If
        If FileExists(ExtDataPath & "Lib\汎用戦闘アニメ\include.eve") Then
            LoadEventData2 ExtDataPath & "Lib\汎用戦闘アニメ\include.eve"
        ElseIf FileExists(ExtDataPath2 & "Lib\汎用戦闘アニメ\include.eve") Then
            LoadEventData2 ExtDataPath2 & "Lib\汎用戦闘アニメ\include.eve"
        ElseIf FileExists(AppPath & "Lib\汎用戦闘アニメ\include.eve") Then
            LoadEventData2 AppPath & "Lib\汎用戦闘アニメ\include.eve"
        Else
            '戦闘アニメ表示切り替えコマンドを非表示に
            BattleAnimation = False
        End If
        
        'システム側のイベントデータの総行数＆ファイル数を記録しておく
        sys_event_data_size = UBound(EventData)
        sys_event_file_num = UBound(EventFileNames)
    ElseIf Not ScenarioLibChecked Then
        'シナリオ側のシステムデータをチェック
        
        ScenarioLibChecked = True
        
        If FileExists(ScenarioPath & "Lib\スペシャルパワー.eve") _
            Or FileExists(ScenarioPath & "Lib\精神コマンド.eve") _
            Or FileExists(ScenarioPath & "Lib\汎用戦闘アニメ\include.eve") _
        Then
            'システムデータのロードをやり直す
            ReDim EventData(0)
            ReDim EventFileID(0)
            ReDim EventLineNum(0)
            ReDim EventFileNames(0)
            CurrentLineNum = 0
            SysEventDataSize = 0
            SysEventFileNum = 0
            With colSysNormalLabelList
                For i = 1 To .Count
                    .Remove 1
                Next
            End With
            With colNormalLabelList
                For i = 1 To .Count
                    .Remove 1
                Next
            End With
            With colEventLabelList
                For i = 1 To .Count
                    .Remove 1
                Next
            End With
            
            'スペシャルパワーアニメ用インクルードファイルをダウンロード
            If FileExists(ScenarioPath & "Lib\スペシャルパワー.eve") Then
                LoadEventData2 ScenarioPath & "Lib\スペシャルパワー.eve"
            ElseIf FileExists(ScenarioPath & "Lib\精神コマンド.eve") Then
                LoadEventData2 ScenarioPath & "Lib\精神コマンド.eve"
            ElseIf FileExists(ExtDataPath & "Lib\スペシャルパワー.eve") Then
                LoadEventData2 ExtDataPath & "Lib\スペシャルパワー.eve"
            ElseIf FileExists(ExtDataPath2 & "Lib\スペシャルパワー.eve") Then
                LoadEventData2 ExtDataPath2 & "Lib\スペシャルパワー.eve"
            ElseIf FileExists(AppPath & "Lib\スペシャルパワー.eve") Then
                LoadEventData2 AppPath & "Lib\スペシャルパワー.eve"
            ElseIf FileExists(ExtDataPath & "Lib\精神コマンド.eve") Then
                LoadEventData2 ExtDataPath & "Lib\精神コマンド.eve"
            ElseIf FileExists(ExtDataPath2 & "Lib\精神コマンド.eve") Then
                LoadEventData2 ExtDataPath2 & "Lib\精神コマンド.eve"
            ElseIf FileExists(AppPath & "Lib\精神コマンド.eve") Then
                LoadEventData2 AppPath & "Lib\精神コマンド.eve"
            End If
            
            '汎用戦闘アニメ用インクルードファイルをダウンロード
            If LCase$(ReadIni("Option", "BattleAnimation")) <> "off" Then
                BattleAnimation = True
            End If
            If FileExists(ScenarioPath & "Lib\汎用戦闘アニメ\include.eve") Then
                LoadEventData2 ScenarioPath & "Lib\汎用戦闘アニメ\include.eve"
            ElseIf FileExists(ExtDataPath & "Lib\汎用戦闘アニメ\include.eve") Then
                LoadEventData2 ExtDataPath & "Lib\汎用戦闘アニメ\include.eve"
            ElseIf FileExists(ExtDataPath2 & "Lib\汎用戦闘アニメ\include.eve") Then
                LoadEventData2 ExtDataPath2 & "Lib\汎用戦闘アニメ\include.eve"
            ElseIf FileExists(AppPath & "Lib\汎用戦闘アニメ\include.eve") Then
                LoadEventData2 AppPath & "Lib\汎用戦闘アニメ\include.eve"
            Else
                '戦闘アニメ表示切り替えコマンドを非表示に
                BattleAnimation = False
            End If
        End If
        
        'シナリオ添付の汎用インクルードファイルをダウンロード
        If FileExists(ScenarioPath & "Lib\include.eve") Then
            LoadEventData2 ScenarioPath & "Lib\include.eve"
        End If
        
        'システム側のイベントデータの総行数＆ファイル数を記録しておく
        sys_event_data_size = UBound(EventData)
        sys_event_file_num = UBound(EventFileNames)
        
        'シナリオ側のイベントデータのロード
        LoadEventData2 fname
    Else
        'シナリオ側のイベントデータのロード
        LoadEventData2 fname
    End If
    
    'エラー表示用にサイズを大きく取っておく
    ReDim Preserve EventData(UBound(EventData) + 1)
    ReDim Preserve EventLineNum(UBound(EventData))
    EventData(UBound(EventData)) = ""
    EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
    
    'データ読みこみ指定
    For i = SysEventDataSize + 1 To UBound(EventData)
        If Left$(EventData(i), 1) = "@" Then
            tname = Mid$(EventData(i), 2)
            
            '既にそのデータが読み込まれているかチェック
            For j = 1 To UBound(Titles)
                If tname = Titles(j) Then
                    Exit For
                End If
            Next
            
            If j > UBound(Titles) Then
                'フォルダを検索
                tfolder = SearchDataFolder(tname)
                If Len(tfolder) = 0 Then
                    DisplayEventErrorMessage _
                        i, "データ「" & tname & "」のフォルダが見つかりません"
                Else
                    ReDim Preserve new_titles(UBound(new_titles) + 1)
                    ReDim Preserve Titles(UBound(Titles) + 1)
                    new_titles(UBound(new_titles)) = tname
                    Titles(UBound(Titles)) = tname
                End If
            End If
        End If
    Next
    
    '各作品データのinclude.eveを読み込む
    If load_mode <> "システム" Then
        '作品毎のインクルードファイル
        For i = 1 To UBound(Titles)
            tfolder = SearchDataFolder(Titles(i))
            If FileExists(tfolder & "\include.eve") Then
                LoadEventData2 tfolder & "\include.eve"
            End If
        Next
        
        '汎用Dataインクルードファイルをロード
        If FileExists(ScenarioPath & "Data\include.eve") Then
            LoadEventData2 ScenarioPath & "Data\include.eve"
        ElseIf FileExists(ExtDataPath & "Data\include.eve") Then
            LoadEventData2 ExtDataPath & "Data\include.eve"
        ElseIf FileExists(ExtDataPath2 & "Data\include.eve") Then
            LoadEventData2 ExtDataPath2 & "Data\include.eve"
        ElseIf FileExists(AppPath & "Data\include.eve") Then
            LoadEventData2 AppPath & "Data\include.eve"
        End If
    End If
    
    '複数行に分割されたコマンドを結合
    For i = SysEventDataSize + 1 To UBound(EventData) - 1
        If Right$(EventData(i), 1) = "_" Then
            EventData(i + 1) = _
                Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
            EventData(i) = " "
        End If
    Next
    
    'ラベルの登録
    num = CurrentLineNum
    If load_mode = "システム" Then
        For CurrentLineNum = 1 To UBound(EventData)
            buf = EventData(CurrentLineNum)
            If Right$(buf, 1) = ":" Then
                AddSysLabel Left$(buf, Len(buf) - 1), CurrentLineNum
            End If
        Next
    ElseIf sys_event_data_size > 0 Then
        'システム側へのイベントデータの追加があった場合
        For CurrentLineNum = 1 To sys_event_data_size
            buf = EventData(CurrentLineNum)
            Select Case Right$(buf, 1)
                Case ":"
                    AddSysLabel Left$(buf, Len(buf) - 1), CurrentLineNum
                Case "："
                    DisplayEventErrorMessage CurrentLineNum, "ラベルの末尾が全角文字になっています"
                    error_found = True
            End Select
        Next
        For CurrentLineNum = sys_event_data_size + 1 To UBound(EventData)
            buf = EventData(CurrentLineNum)
            Select Case Right$(buf, 1)
                Case ":"
                    AddLabel Left$(buf, Len(buf) - 1), CurrentLineNum
                Case "："
                    DisplayEventErrorMessage CurrentLineNum, "ラベルの末尾が全角文字になっています"
                    error_found = True
            End Select
        Next
    Else
        For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
            buf = EventData(CurrentLineNum)
            Select Case Right$(buf, 1)
                Case ":"
                    AddLabel Left$(buf, Len(buf) - 1), CurrentLineNum
                Case "："
                    DisplayEventErrorMessage CurrentLineNum, "ラベルの末尾が全角文字になっています"
                    error_found = True
            End Select
        Next
    End If
    CurrentLineNum = num
    
    'コマンドデータ配列を設定
    If UBound(EventData) > UBound(EventCmd) Then
        num = UBound(EventCmd)
        ReDim Preserve EventCmd(UBound(EventData))
        For i = num + 1 To UBound(EventCmd)
            Set EventCmd(i) = New CmdData
            EventCmd(i).LineNum = i
        Next
    End If
    
    '書式チェックはシナリオ側にのみ実施
    If load_mode <> "システム" Then
    
    '構文解析と書式チェックその１
    '制御構造
    CmdStackIdx = 0
    CmdPosStackIdx = 0
    For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
        If EventCmd(CurrentLineNum) Is Nothing Then
            Set EventCmd(CurrentLineNum) = New CmdData
            EventCmd(CurrentLineNum).LineNum = CurrentLineNum
        End If
        With EventCmd(CurrentLineNum)
            'コマンドの構文解析
            If Not .Parse(EventData(CurrentLineNum)) Then
                error_found = True
            End If
            
            'リスト長がマイナスのときは括弧の対応が取れていない
            If .ArgNum = -1 Then
                Select Case CmdStack(CmdStackIdx)
                    Case AskCmd, AutoTalkCmd, QuestionCmd, TalkCmd
                        'これらのコマンドの入力の場合は無視する
                    Case Else
                        DisplayEventErrorMessage CurrentLineNum, "括弧の対応が取れていません"
                        error_found = True
                End Select
            End If
            
            'コマンドに応じて制御構造をチェック
            Select Case .Name
                Case IfCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If .GetArg(4) = "then" Then
                        CmdStackIdx = CmdStackIdx + 1
                        CmdPosStackIdx = CmdPosStackIdx + 1
                        CmdStack(CmdStackIdx) = IfCmd
                        CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    End If
                    
                Case ElseIfCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) <> IfCmd Then
                        DisplayEventErrorMessage CurrentLineNum, "ElseIfに対応するIfがありません"
                        error_found = True
                        
                        CmdStackIdx = CmdStackIdx + 1
                        CmdPosStackIdx = CmdPosStackIdx + 1
                        CmdStack(CmdStackIdx) = IfCmd
                        CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    End If
                    
                Case ElseCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        DisplayEventErrorMessage CurrentLineNum, "Elseに対応するIfがありません"
                        error_found = True
                        
                        CmdStackIdx = CmdStackIdx + 1
                        CmdPosStackIdx = CmdPosStackIdx + 1
                        CmdStack(CmdStackIdx) = IfCmd
                        CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    End If
                    
                Case EndIfCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) = IfCmd Then
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                    Else
                        DisplayEventErrorMessage CurrentLineNum, "EndIfに対応するIfがありません"
                        error_found = True
                    End If
                    
                Case DoCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    CmdStackIdx = CmdStackIdx + 1
                    CmdPosStackIdx = CmdPosStackIdx + 1
                    CmdStack(CmdStackIdx) = DoCmd
                    CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    
                Case LoopCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) = DoCmd Then
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                    Else
                        DisplayEventErrorMessage CurrentLineNum, "Loopに対応するDoがありません"
                        error_found = True
                    End If
                    
                Case ForCmd, ForEachCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage _
                            num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    CmdStackIdx = CmdStackIdx + 1
                    CmdPosStackIdx = CmdPosStackIdx + 1
                    CmdStack(CmdStackIdx) = .Name
                    CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    
                Case NextCmd
                    If .ArgNum = 1 Or .ArgNum = 2 Then
                        If CmdStack(CmdStackIdx) = TalkCmd Then
                            num = CmdPosStack(CmdPosStackIdx)
                            DisplayEventErrorMessage _
                                num, "Talkに対応するEndがありません"
                            CmdStackIdx = CmdStackIdx - 1
                            CmdPosStackIdx = CmdPosStackIdx - 1
                            error_found = True
                        End If
                        
                        Select Case CmdStack(CmdStackIdx)
                            Case ForCmd, ForEachCmd
                                CmdStackIdx = CmdStackIdx - 1
                                CmdPosStackIdx = CmdPosStackIdx - 1
                            Case Else
                                DisplayEventErrorMessage CurrentLineNum, _
                                    "Nextに対応するコマンドがありません"
                                error_found = True
                        End Select
                    Else
                        If CmdStack(CmdStackIdx) = TalkCmd Then
                            Select Case CmdStack(CmdStackIdx)
                                Case ForCmd, ForEachCmd
                                    CmdStackIdx = CmdStackIdx - 1
                                    CmdPosStackIdx = CmdPosStackIdx - 1
                                Case Else
                                    DisplayEventErrorMessage CurrentLineNum, _
                                        "Nextに対応するコマンドがありません"
                                    error_found = True
                            End Select
                        End If
                    End If
                    
                Case SwitchCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage _
                            num, "Talkに対応するEndがありません"
                        error_found = True
                    End If
                    
                    CmdStackIdx = CmdStackIdx + 1
                    CmdPosStackIdx = CmdPosStackIdx + 1
                    CmdStack(CmdStackIdx) = SwitchCmd
                    CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    
                Case CaseCmd, CaseElseCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage _
                            num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) <> SwitchCmd Then
                        DisplayEventErrorMessage _
                            CurrentLineNum, "Caseに対応するSwitchがありません"
                        error_found = True
                        
                        CmdStackIdx = CmdStackIdx + 1
                        CmdPosStackIdx = CmdPosStackIdx + 1
                        CmdStack(CmdStackIdx) = SwitchCmd
                        CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    End If
                    
                Case EndSwCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage _
                            num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) = SwitchCmd Then
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                    Else
                        DisplayEventErrorMessage _
                            CurrentLineNum, "EndSwに対応するSwitchがありません"
                        error_found = True
                    End If
                    
                Case TalkCmd, AutoTalkCmd
                    If CmdStack(CmdStackIdx) <> .Name Then
                        CmdStackIdx = CmdStackIdx + 1
                        CmdPosStackIdx = CmdPosStackIdx + 1
                        CmdStack(CmdStackIdx) = .Name
                        CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    End If
                    
                Case AskCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage _
                            num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    i = .ArgNum
                    Do While i > 1
                        Select Case .GetArg(i)
                            Case "通常"
                            Case "拡大"
                            Case "連続表示"
                            Case "キャンセル可"
                            Case "終了"
                                i = 3
                                Exit Do
                            Case Else
                                Exit Do
                        End Select
                        i = i - 1
                    Loop
                    If i < 3 Then
                        CmdStackIdx = CmdStackIdx + 1
                        CmdPosStackIdx = CmdPosStackIdx + 1
                        CmdStack(CmdStackIdx) = AskCmd
                        CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    End If
                    
                Case QuestionCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage _
                            num, "Talkに対応するEndがありません"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    i = .ArgNum
                    Do While i > 1
                        Select Case .GetArg(.ArgNum)
                            Case "通常"
                            Case "拡大"
                            Case "連続表示"
                            Case "キャンセル可"
                            Case "終了"
                                i = 4
                                Exit Do
                            Case Else
                                Exit Do
                        End Select
                        i = i - 1
                    Loop
                    If i < 4 Then
                        CmdStackIdx = CmdStackIdx + 1
                        CmdPosStackIdx = CmdPosStackIdx + 1
                        CmdStack(CmdStackIdx) = QuestionCmd
                        CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    End If
                    
                Case EndCmd
                    Select Case CmdStack(CmdStackIdx)
                        Case TalkCmd, AutoTalkCmd, AskCmd, QuestionCmd
                            CmdStackIdx = CmdStackIdx - 1
                            CmdPosStackIdx = CmdPosStackIdx - 1
                        Case Else
                            DisplayEventErrorMessage _
                                CurrentLineNum, "Endに対応するTalkがありません"
                            error_found = True
                    End Select
                    
                Case SuspendCmd
                    Select Case CmdStack(CmdStackIdx)
                        Case TalkCmd, AutoTalkCmd
                            CmdStackIdx = CmdStackIdx - 1
                            CmdPosStackIdx = CmdPosStackIdx - 1
                        Case Else
                            DisplayEventErrorMessage _
                                CurrentLineNum, "Suspendに対応するTalkがありません"
                            error_found = True
                    End Select
                    
                Case ExitCmd, PlaySoundCmd, WaitCmd
                    Select Case CmdStack(CmdStackIdx)
                        Case TalkCmd, AutoTalkCmd, AskCmd, QuestionCmd
                            num = CmdPosStack(CmdPosStackIdx)
                            DisplayEventErrorMessage _
                                num, "Talkに対応するEndがありません"
                            CmdStackIdx = CmdStackIdx - 1
                            CmdPosStackIdx = CmdPosStackIdx - 1
                            error_found = True
                    End Select
                    
                Case NopCmd
                    If EventData(CurrentLineNum) = " " Then
                        '"_"で消去された行。Talk中の改行に対応するためのダミーの空白
                        EventData(CurrentLineNum) = ""
                    Else
                        Select Case CmdStack(CmdStackIdx)
                            Case TalkCmd, AutoTalkCmd, AskCmd, QuestionCmd
                                If CurrentLineNum = UBound(EventData) Then
                                    num = CmdPosStack(CmdPosStackIdx)
                                    DisplayEventErrorMessage _
                                        num, "Talkに対応するEndがありません"
                                    CmdStackIdx = CmdStackIdx - 1
                                    CmdPosStackIdx = CmdPosStackIdx - 1
                                    error_found = True
                                Else
                                    buf = LCase$(ListIndex(EventData(CurrentLineNum + 1), 1))
                                    Select Case CmdStack(CmdStackIdx)
                                        Case TalkCmd
                                            buf2 = "talk"
                                        Case AutoTalkCmd
                                            buf2 = "autotalk"
                                        Case AskCmd
                                            buf2 = "ask"
                                        Case QuestionCmd
                                            buf2 = "question"
                                        Case Else
                                            buf2 = ""
                                    End Select
                                    If buf <> buf2 And buf <> "end" And buf <> "suspend" _
                                        And Len(buf) = LenB(StrConv(buf, vbFromUnicode)) _
                                    Then
                                        num = CmdPosStack(CmdPosStackIdx)
                                        DisplayEventErrorMessage _
                                            num, "Talkに対応するEndがありません"
                                        CmdStackIdx = CmdStackIdx - 1
                                        CmdPosStackIdx = CmdPosStackIdx - 1
                                        error_found = True
                                    End If
                                End If
                        End Select
                    End If
                
            End Select
        End With
    Next
    
    'ファイルの末尾まで読んでもコマンドの終わりがなかった？
    If CmdStackIdx > 0 Then
        num = CmdPosStack(CmdPosStackIdx)
        Select Case CmdStack(CmdStackIdx)
            Case AskCmd
                DisplayEventErrorMessage num, "Askに対応するEndがありません"
            Case AutoTalkCmd
                DisplayEventErrorMessage num, "AutoTalkに対応するEndがありません"
            Case DoCmd
                DisplayEventErrorMessage num, "Doに対応するLoopがありません"
            Case ForCmd
                DisplayEventErrorMessage num, "Forに対応するNextがありません"
            Case ForEachCmd
                DisplayEventErrorMessage num, "ForEachに対応するNextがありません"
            Case IfCmd
                DisplayEventErrorMessage num, "Ifに対応するEndIfがありません"
            Case QuestionCmd
                DisplayEventErrorMessage num, "Questionに対応するEndがありません"
            Case SwitchCmd
                DisplayEventErrorMessage num, "Switchに対応するEndSwがありません"
            Case TalkCmd
                DisplayEventErrorMessage num, "Talkに対応するEndがありません"
        End Select
        error_found = True
    End If
    
    '書式エラーが見つかった場合はSRCを終了
    If error_found Then
        TerminateSRC
    End If
    
    '書式チェックその２
    '主なコマンドの引数の数をチェック
    For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
        With EventCmd(CurrentLineNum)
            Select Case .Name
                Case CreateCmd
                    If .ArgNum < 8 Then
                        DisplayEventErrorMessage _
                            CurrentLineNum, "Createコマンドのパラメータ数が違います"
                        error_found = True
                    End If
                Case PilotCmd
                    If .ArgNum < 3 Then
                        DisplayEventErrorMessage _
                            CurrentLineNum, "Pilotコマンドのパラメータ数が違います"
                        error_found = True
                    End If
                Case UnitCmd
                    If .ArgNum <> 3 Then
                        DisplayEventErrorMessage _
                            CurrentLineNum, "Unitコマンドのパラメータ数が違います"
                        error_found = True
                    End If
            End Select
        End With
    Next
    
    '書式エラーが見つかった場合はSRCを終了
    If error_found Then
        TerminateSRC
    End If
    
    'シナリオ側のイベントデータの場合はここまでスキップ
    Else
    
    'システム側のイベントデータの場合の処理
    
    'CmdDataクラスのインスタンスの生成のみ行っておく
    If CurrentLineNum > UBound(EventCmd) Then
        ReDim Preserve EventCmd(CurrentLineNum)
        i = CurrentLineNum
        Do While EventCmd(i) Is Nothing
            Set EventCmd(i) = New CmdData
            EventCmd(i).LineNum = i
            i = i - 1
        Loop
    End If
    
    End If
    
    'イベントデータの読み込みが終了したのでシステム側イベントデータのサイズを決定。
    'システム側イベントデータは読み込みを一度だけやればよい。
    If sys_event_data_size > 0 Then
        SysEventDataSize = sys_event_data_size
        SysEventFileNum = sys_event_file_num
    End If
    
    'クイックロードやリスタートの場合はシナリオデータの再ロードのみ
    Select Case load_mode
        Case "リストア"
            ADList.AddDefaultAnimation
            Exit Sub
        Case "システム", "クイックロード", "リスタート"
            Exit Sub
    End Select
    
    '追加されたシステム側イベントデータをチェックする場合はここで終了
    If fname = "" Then
        Exit Sub
    End If
    
    'ロードするデータ数をカウント
    num = 2 * UBound(new_titles)
    If IsLocalDataLoaded Then
        If num > 0 Then
            num = num + 2
        End If
    Else
        num = num + 2
    End If
    If FileExists(Left$(fname, Len(fname) - 4) & ".map") Then
        num = num + 1
    End If
    If num = 0 And IsLocalDataLoaded Then
        'デフォルトの戦闘アニメデータを設定
        ADList.AddDefaultAnimation
        Exit Sub
    End If
    
    'ロード画面を表示
    OpenNowLoadingForm
    
    'ロードサイズを設定
    SetLoadImageSize num
    
    '使用しているタイトルのデータをロード
    For i = 1 To UBound(new_titles)
        IncludeData new_titles(i)
    Next
    
    'ローカルデータの読みこみ
    If Not IsLocalDataLoaded Or UBound(new_titles) > 0 Then
        If FileExists(ScenarioPath & "Data\alias.txt") Then
            ALDList.Load ScenarioPath & "Data\alias.txt"
        End If
        If FileExists(ScenarioPath & "Data\sp.txt") Then
            SPDList.Load ScenarioPath & "Data\sp.txt"
        ElseIf FileExists(ScenarioPath & "Data\mind.txt") Then
            SPDList.Load ScenarioPath & "Data\mind.txt"
        End If
        If FileExists(ScenarioPath & "Data\pilot.txt") Then
            PDList.Load ScenarioPath & "Data\pilot.txt"
        End If
        If FileExists(ScenarioPath & "Data\non_pilot.txt") Then
            NPDList.Load ScenarioPath & "Data\non_pilot.txt"
        End If
        If FileExists(ScenarioPath & "Data\robot.txt") Then
            UDList.Load ScenarioPath & "Data\robot.txt"
        End If
        If FileExists(ScenarioPath & "Data\unit.txt") Then
            UDList.Load ScenarioPath & "Data\unit.txt"
        End If
        
        DisplayLoadingProgress
        
        If FileExists(ScenarioPath & "Data\pilot_message.txt") Then
            MDList.Load ScenarioPath & "Data\pilot_message.txt"
        End If
        If FileExists(ScenarioPath & "Data\pilot_dialog.txt") Then
            DDList.Load ScenarioPath & "Data\pilot_dialog.txt"
        End If
        If FileExists(ScenarioPath & "Data\effect.txt") Then
            EDList.Load ScenarioPath & "Data\effect.txt"
        End If
        If FileExists(ScenarioPath & "Data\animation.txt") Then
            ADList.Load ScenarioPath & "Data\animation.txt"
        End If
        If FileExists(ScenarioPath & "Data\ext_animation.txt") Then
            EADList.Load ScenarioPath & "Data\ext_animation.txt"
        End If
        If FileExists(ScenarioPath & "Data\item.txt") Then
            IDList.Load ScenarioPath & "Data\item.txt"
        End If
        
        DisplayLoadingProgress
        
        IsLocalDataLoaded = True
    End If
    
    'デフォルトの戦闘アニメデータを設定
    ADList.AddDefaultAnimation
    
    'マップデータをロード
    If FileExists(Left$(fname, Len(fname) - 4) & ".map") Then
        LoadMapData Left$(fname, Len(fname) - 4) & ".map"
        SetupBackground
        RedrawScreen
        DisplayLoadingProgress
    End If
    
    'ロード画面を閉じる
    CloseNowLoadingForm
End Sub

'イベントファイルの読み込み
Public Sub LoadEventData2(fname As String, Optional ByVal lnum As Long)
Dim FileNumber As Integer, CurrentLineNum2 As Integer
Dim i As Integer
Dim buf As String, fname2 As String
Dim fid As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean
    
    If fname = "" Then
        Exit Sub
    End If
    
    'イベントファイル名を記録しておく (エラー表示用)
    ReDim Preserve EventFileNames(UBound(EventFileNames) + 1)
    EventFileNames(UBound(EventFileNames)) = fname
    fid = UBound(EventFileNames)
    
    On Error GoTo ErrorHandler
    
    'ファイルを開く
    FileNumber = FreeFile
    Open fname For Input Access Read As #FileNumber
    
    '行番号の設定
    If lnum > 0 Then
        CurrentLineNum = lnum
    End If
    CurrentLineNum2 = 0
    
    '各行の読み込み
    Do Until EOF(FileNumber)
        CurrentLineNum = CurrentLineNum + 1
        CurrentLineNum2 = CurrentLineNum2 + 1
        
        'データ領域確保
        ReDim Preserve EventData(CurrentLineNum)
        ReDim Preserve EventFileID(CurrentLineNum)
        ReDim Preserve EventLineNum(CurrentLineNum)
        
        '行の読み込み
        Line Input #FileNumber, buf
        TrimString buf
        
        'コメントを削除
        If Left$(buf, 1) = "#" Then
            buf = " "
        ElseIf InStr(buf, "//") > 0 Then
            in_single_quote = False
            in_double_quote = False
            For i = 1 To Len(buf)
                Select Case Mid$(buf, i, 1)
                    Case "`"
                        'シングルクオート
                        If Not in_double_quote Then
                            in_single_quote = Not in_single_quote
                        End If
                    Case """"
                        'ダブルクオート
                        If Not in_single_quote Then
                            in_double_quote = Not in_double_quote
                        End If
                    Case "/"
                        'コメント？
                        If Not in_double_quote And Not in_single_quote Then
                            If i > 1 Then
                                If Mid$(buf, i - 1, 1) = "/" Then
                                    buf = Left$(buf, i - 2)
                                    If buf = "" Then
                                        buf = " "
                                    End If
                                    Exit For
                                End If
                            End If
                        End If
                End Select
            Next
        End If
        
        '行を保存
        EventData(CurrentLineNum) = buf
        EventFileID(CurrentLineNum) = fid
        EventLineNum(CurrentLineNum) = CurrentLineNum2
        
        '他のイベントファイルの読み込み
        If Left$(buf, 1) = "<" Then
            If InStr(buf, ">") = Len(buf) And buf <> "<>" Then
                CurrentLineNum = CurrentLineNum - 1
                fname2 = Mid$(buf, 2, Len(buf) - 2)
                If fname2 <> "Lib\スペシャルパワー.eve" _
                    And fname2 <> "Lib\汎用戦闘アニメ\include.eve" _
                    And fname2 <> "Lib\include.eve" _
                Then
                    If Len(Dir$(ScenarioPath & fname2)) > 0 Then
                        LoadEventData2 ScenarioPath & fname2
                    ElseIf Len(Dir$(ExtDataPath & fname2)) > 0 Then
                        LoadEventData2 ExtDataPath & fname2
                    ElseIf Len(Dir$(ExtDataPath2 & fname2)) > 0 Then
                        LoadEventData2 ExtDataPath2 & fname2
                    ElseIf Len(Dir$(AppPath & fname2)) > 0 Then
                        LoadEventData2 AppPath & fname2
                    End If
                End If
            End If
        End If
    Loop
    
    'ファイルを閉じる
    Close #FileNumber
    
    Exit Sub
    
ErrorHandler:
    If Len(buf) = 0 Then
        ErrorMessage fname & "が開けません"
    Else
        ErrorMessage fname & "のロード中にエラーが発生しました" & vbCr _
            & Format$(CurrentLineNum2) & "行目のイベントデータが不正です"
    End If
    TerminateSRC
End Sub


'イベントの実行
Public Sub HandleEvent(ParamArray Args() As Variant)
Dim event_que_idx As Integer
Dim ret As Long, i As Integer
Dim flag As Boolean
Dim prev_is_gui_locked As Boolean
Dim prev_call_depth As Integer
Dim uparty As String
Dim u As Unit
Dim main_event_done As Boolean
    
    '画面入力をロック
    prev_is_gui_locked = IsGUILocked
    If Not IsGUILocked Then
        LockGUI
    End If
    
    '現在選択されているユニット＆ターゲットをイベント用に設定
    '(SearchLabel()実行時の式計算用にあらかじめ設定しておく)
    Set SelectedUnitForEvent = SelectedUnit
    '引数に指定されたユニットを優先
    If UBound(Args) > 0 Then
        If PList.IsDefined(Args(1)) Then
            With PList.Item(Args(1))
                If Not .Unit Is Nothing Then
                    Set SelectedUnitForEvent = .Unit
                End If
            End With
        End If
    End If
    Set SelectedTargetForEvent = SelectedTarget
    
    'イベントキューを作成
    ReDim Preserve EventQue(UBound(EventQue) + 1)
    event_que_idx = UBound(EventQue)
    Select Case Args(0)
        Case "プロローグ"
            EventQue(UBound(EventQue)) = "プロローグ"
            Stage = "プロローグ"
        Case "エピローグ"
            EventQue(UBound(EventQue)) = "エピローグ"
            Stage = "エピローグ"
        Case "破壊"
            EventQue(UBound(EventQue)) = "破壊 " & Args(1)
            With PList.Item(Args(1))
                uparty = .Party
                If Not .Unit Is Nothing Then
                    With .Unit
                        '格納されていたユニットも破壊しておく
' MOD START MARGE
'                        For i = 1 To .CountUnitOnBoard
'                            Set u = .UnitOnBoard(1)
'                            .UnloadUnit u.ID
'                            u.Status = "破壊"
'                            u.HP = 0
'                            ReDim Preserve EventQue(UBound(EventQue) + 1)
'                            EventQue(UBound(EventQue)) = _
'                                "破壊 " & u.MainPilot.ID
'                        Next
                        Do While .CountUnitOnBoard > 0
                            Set u = .UnitOnBoard(1)
                            .UnloadUnit u.ID
                            u.Status = "破壊"
                            u.HP = 0
                            ReDim Preserve EventQue(UBound(EventQue) + 1)
                            EventQue(UBound(EventQue)) = _
                                "マップ攻撃破壊 " & u.MainPilot.ID
                        Loop
' MOD END MARGE
                        uparty = .Party0
                    End With
                End If
            End With
            
            '全滅の判定
            flag = False
            For Each u In UList
                With u
                    If .Party0 = uparty _
                        And .Status = "出撃" _
                        And Not .IsConditionSatisfied("憑依") _
                    Then
                        flag = True
                        Exit For
                    End If
                End With
            Next
            If Not flag Then
                ReDim Preserve EventQue(UBound(EventQue) + 1)
                EventQue(UBound(EventQue)) = "全滅 " & uparty
            End If
        Case "マップ攻撃破壊"
            EventQue(UBound(EventQue)) = "マップ攻撃破壊 " & Args(1)
            With PList.Item(Args(1))
                uparty = .Party
                If Not .Unit Is Nothing Then
                    With .Unit
                        '格納されていたユニットも破壊しておく
                        For i = 1 To .CountUnitOnBoard
                            Set u = .UnitOnBoard(i)
                            .UnloadUnit u.ID
                            u.Status = "破壊"
                            u.HP = 0
                            ReDim Preserve EventQue(UBound(EventQue) + 1)
                            EventQue(UBound(EventQue)) = _
                                "マップ攻撃破壊 " & u.MainPilot.ID
                        Next
                        uparty = .Party0
                    End With
                End If
            End With
        Case "ターン"
            EventQue(UBound(EventQue)) = "ターン 全 " & Args(2)
            ReDim Preserve EventQue(UBound(EventQue) + 1)
            EventQue(UBound(EventQue)) = "ターン " & Format$(Args(1)) & " " & Args(2)
        Case "損傷率"
            EventQue(UBound(EventQue)) = "損傷率 " & Args(1) & " " & Format$(Args(2))
        Case "攻撃"
            EventQue(UBound(EventQue)) = "攻撃 " & Args(1) & " " & Args(2)
        Case "攻撃後"
            EventQue(UBound(EventQue)) = "攻撃後 " & Args(1) & " " & Args(2)
        Case "会話"
            EventQue(UBound(EventQue)) = "会話 " & Args(1) & " " & Args(2)
        Case "接触"
            EventQue(UBound(EventQue)) = "接触 " & Args(1) & " " & Args(2)
        Case "進入"
            EventQue(UBound(EventQue)) = "進入 " & Args(1) & " " _
                & Format$(Args(2)) & " " & Format$(Args(3))
            ReDim Preserve EventQue(UBound(EventQue) + 1)
            EventQue(UBound(EventQue)) = "進入 " & Args(1) & " " _
                & TerrainName(CInt(Args(2)), CInt(Args(3)))
            If Args(2) = 1 Then
                ReDim Preserve EventQue(UBound(EventQue) + 1)
                EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " W"
            ElseIf Args(2) = MapWidth Then
                ReDim Preserve EventQue(UBound(EventQue) + 1)
                EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " E"
            End If
            If Args(3) = 1 Then
                ReDim Preserve EventQue(UBound(EventQue) + 1)
                EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " N"
            ElseIf Args(3) = MapHeight Then
                ReDim Preserve EventQue(UBound(EventQue) + 1)
                EventQue(UBound(EventQue)) = "脱出 " & Args(1) & " S"
            End If
        Case "収納"
            EventQue(UBound(EventQue)) = "収納 " & Args(1)
        Case "使用"
            EventQue(UBound(EventQue)) = "使用 " & Args(1) & " " & Args(2)
        Case "使用後"
            EventQue(UBound(EventQue)) = "使用後 " & Args(1) & " " & Args(2)
        Case "行動終了"
            EventQue(UBound(EventQue)) = "行動終了 " & Args(1)
        Case "ユニットコマンド"
            EventQue(UBound(EventQue)) = "ユニットコマンド " & Args(1) & " " & Args(2)
            If Not IsEventDefined(EventQue(UBound(EventQue))) Then
                EventQue(UBound(EventQue)) = "ユニットコマンド " & Args(1) & " " _
                    & PList.Item(Args(2)).Unit.Name
            End If
        Case Else
            EventQue(UBound(EventQue)) = Args(0)
            For i = 1 To UBound(Args)
                EventQue(UBound(EventQue)) = EventQue(UBound(EventQue)) & " " & Args(i)
            Next
    End Select
    
    If CallDepth > MaxCallDepth Then
        ErrorMessage "サブルーチンの呼び出し階層が" & Format$(MaxCallDepth) & _
            "を超えているため、イベントの処理が出来ません"
        CallDepth = MaxCallDepth
        Exit Sub
    End If
    
    '現在の状態を保存
    ArgIndexStack(CallDepth) = ArgIndex
    VarIndexStack(CallDepth) = VarIndex
    ForIndexStack(CallDepth) = ForIndex
    SaveBasePoint
    
    '呼び出し階層数をインクリメント
    prev_call_depth = CallDepth
    CallDepth = CallDepth + 1
    
    '各イベントを発生させる
    i = event_que_idx
    IsCanceled = False
    Do
        'Debug.Print "HandleEvent (" & EventQue(i) & ")"
        
        '前のイベントで他のユニットが出現している可能性があるので
        '本当に全滅したのか判定
        If LIndex(EventQue(i), 1) = "全滅" Then
            uparty = LIndex(EventQue(i), 2)
            For Each u In UList
                With u
                    If .Party0 = uparty _
                        And .Status = "出撃" _
                        And Not .IsConditionSatisfied("憑依") _
                    Then
                        GoTo NextLoop
                    End If
                End With
            Next
        End If
        
        CurrentLabel = 0
        main_event_done = False
        Do While True
            '現在選択されているユニット＆ターゲットをイベント用に設定
            'SearchLabel()で入れ替えられる可能性があるので、毎回設定し直す必要あり
            Set SelectedUnitForEvent = SelectedUnit
            '引数に指定されたユニットを優先
            If UBound(Args) > 0 Then
                If PList.IsDefined(Args(1)) Then
                    With PList.Item(Args(1))
                        If Not .Unit Is Nothing Then
                            Set SelectedUnitForEvent = .Unit
                        End If
                    End With
                End If
            End If
            Set SelectedTargetForEvent = SelectedTarget
            
            '実行するイベントラベルを探す
            Do
                If IsNumeric(EventQue(i)) Then
                    If CurrentLabel = 0 Then
                        ret = CLng(EventQue(i))
                    Else
                        ret = 0
                    End If
                Else
                    ret = SearchLabel(EventQue(i), CurrentLabel + 1)
                End If
                If ret = 0 Then
                    GoTo NextLoop
                End If
                
                CurrentLabel = ret
                
                If Asc(EventData(ret)) <> 42 Then '*
                    '常時イベントではないイベントは１度しか実行しない
                    If main_event_done Then
                        ret = 0
                    Else
                        main_event_done = True
                    End If
                End If
            Loop While ret = 0
            
            '戦闘後のイベント実行前にはいくつかの後始末が必要
            If Left$(EventData(ret), 1) <> "*" Then
                If Args(0) = "破壊" _
                    Or Args(0) = "損傷率" _
                    Or Args(0) = "攻撃後" _
                    Or Args(0) = "全滅" _
                Then
                    '画面をクリア
                    If MainForm.Visible = True Then
                        ClearUnitStatus
                        RedrawScreen
                    End If
                    
                    'メッセージウィンドウを閉じる
                    If frmMessage.Visible = True Then
                        CloseMessageForm
                    End If
                End If
            End If
            
            'ラベルの行は実行しても無駄なので
            ret = ret + 1
            
            DoEvents
            
            'イベントの各コマンドを実行
            Do
                CurrentLineNum = ret
                If CurrentLineNum > UBound(EventCmd) Then
                    GoTo ExitLoop
                End If
                ret = EventCmd(CurrentLineNum).Exec
            Loop While ret > 0
            
            'ステージが終了 or キャンセル？
            If IsScenarioFinished Or IsCanceled Then
                GoTo ExitLoop
            End If
        Loop
NextLoop:
        i = i + 1
    Loop While i <= UBound(EventQue)
ExitLoop:
    
    If CallDepth >= 0 Then
        '呼び出し階層数を元に戻す
        '（サブルーチン内でExitが呼ばれることがあるので単純に-1出来ない）
        CallDepth = prev_call_depth
        
        'イベント実行前の状態に復帰
        ArgIndex = ArgIndexStack(CallDepth)
        VarIndex = VarIndexStack(CallDepth)
        ForIndex = ForIndexStack(CallDepth)
    Else
        ArgIndex = 0
        VarIndex = 0
        ForIndex = 0
    End If
    
    'イベントキューを元に戻す
    ReDim Preserve EventQue(MinLng(event_que_idx - 1, UBound(EventQue)))
    
    'フォント設定をデフォルトに戻す
    With MainForm.picMain(0)
        .ForeColor = rgb(255, 255, 255)
        With .Font
            .Size = 16
            .Name = "ＭＳ Ｐ明朝"
            .Bold = True
            .Italic = False
        End With
        PermanentStringMode = False
        KeepStringMode = False
    End With
    
    'オブジェクト色をデフォルトに戻す
    ObjColor = vbWhite
    ObjFillColor = vbWhite
    ObjFillStyle = vbFSTransparent
    ObjDrawWidth = 1
    ObjDrawOption = ""
    
    '描画の基準座標位置を元に戻す
    RestoreBasePoint
    
    '画面入力のロックを解除
    If Not prev_is_gui_locked Then
        UnlockGUI
    End If
End Sub

'イベントを登録しておき、後で実行
Public Sub RegisterEvent(ParamArray Args() As Variant)
Dim i As Integer

    ReDim Preserve EventQue(UBound(EventQue) + 1)
    EventQue(UBound(EventQue)) = Args(0)
    For i = 1 To UBound(Args)
        EventQue(UBound(EventQue)) = EventQue(UBound(EventQue)) & " " & Args(i)
    Next
End Sub


'ラベルの検索
Public Function SearchLabel(lname As String, Optional ByVal start As Long) As Long
Dim ltype As LabelType, llen As Integer, litem() As String, lnum(4) As String
Dim is_unit(4) As Boolean, is_num(4) As Boolean, is_condition(4) As Boolean
Dim str1 As String, str2 As String, lname2 As String
Dim i As Long, lab As LabelData, tmp_u As Unit
Dim revrersible As Boolean, reversed As Boolean
    
    'ラベルの各要素をあらかじめ解析
    llen = ListSplit(lname, litem)
    
    'ラベルの種類を判定
    Select Case litem(1)
        Case "プロローグ"
            ltype = PrologueEventLabel
        Case "スタート"
            ltype = StartEventLabel
        Case "エピローグ"
            ltype = EpilogueEventLabel
        Case "ターン"
            ltype = TurnEventLabel
            If IsNumeric(litem(2)) Then
                is_num(2) = True
            End If
            lnum(2) = StrToLng(litem(2))
        Case "損傷率"
            ltype = DamageEventLabel
            is_unit(2) = True
            is_num(3) = True
            lnum(3) = StrToLng(litem(3))
        Case "破壊", "マップ攻撃破壊"
            ltype = DestructionEventLabel
            is_unit(2) = True
        Case "全滅"
            ltype = TotalDestructionEventLabel
        Case "攻撃"
            ltype = AttackEventLabel
            revrersible = True
            is_unit(2) = True
            is_unit(3) = True
        Case "攻撃後"
            ltype = AfterAttackEventLabel
            revrersible = True
            is_unit(2) = True
            is_unit(3) = True
        Case "会話"
            ltype = TalkEventLabel
            is_unit(2) = True
            is_unit(3) = True
        Case "接触"
            ltype = ContactEventLabel
            revrersible = True
            is_unit(2) = True
            is_unit(3) = True
        Case "進入"
            ltype = EnterEventLabel
            is_unit(2) = True
            If llen = 4 Then
                is_num(3) = True
                is_num(4) = True
                lnum(3) = StrToLng(litem(3))
                lnum(4) = StrToLng(litem(4))
            End If
        Case "脱出"
            ltype = EscapeEventLabel
            is_unit(2) = True
        Case "収納"
            ltype = LandEventLabel
            is_unit(2) = True
        Case "使用"
            ltype = UseEventLabel
            is_unit(2) = True
        Case "使用後"
            ltype = AfterUseEventLabel
            is_unit(2) = True
        Case "変形"
            ltype = TransformEventLabel
            is_unit(2) = True
        Case "合体"
            ltype = CombineEventLabel
            is_unit(2) = True
        Case "分離"
            ltype = SplitEventLabel
            is_unit(2) = True
        Case "行動終了"
            ltype = FinishEventLabel
            is_unit(2) = True
        Case "レベルアップ"
            ltype = LevelUpEventLabel
            is_unit(2) = True
        Case "勝利条件"
            ltype = RequirementEventLabel
        Case "再開"
            ltype = ResumeEventLabel
        Case "マップコマンド"
            ltype = MapCommandEventLabel
            is_condition(3) = True
        Case "ユニットコマンド"
            ltype = UnitCommandEventLabel
            is_condition(4) = True
        Case "特殊効果"
            ltype = EffectEventLabel
        Case Else
            ltype = NormalLabel
    End Select
    
    '各ラベルについて一致しているかチェック
    For Each lab In colEventLabelList
        With lab
            'ラベルの種類が一致している？
            If ltype <> .Name Then
                GoTo NextLabel
            End If
            
            'ClearEventされていない？
            If Not .Enable Then
                GoTo NextLabel
            End If
            
            '検索開始行より後ろ？
            If .LineNum < start Then
                GoTo NextLabel
            End If
            
            'パラメータ数が一致している？
            If llen <> .CountPara Then
                If ltype <> MapCommandEventLabel _
                    And ltype <> UnitCommandEventLabel _
                Then
                    GoTo NextLabel
                End If
            End If
            
            '各パラメータが一致している？
            reversed = False
CheckPara:
            For i = 2 To llen
                'コマンド関連ラベルの最後のパラメータは条件式なのでチェックを省く
                If is_condition(i) Then
                    Exit For
                End If
                
                '比較するパラメータ
                str1 = litem(i)
                If reversed Then
                    str2 = .Para(5 - i)
                Else
                    str2 = .Para(i)
                End If
                
                '「全」は全てに一致
                If str2 = "全" Then
                    'だだし、「ターン 全」が２回実行されるのは防ぐ
                    If ltype <> TurnEventLabel Or i <> 2 Then
                        GoTo NextPara
                    End If
                End If
                
                '数値として比較？
                If is_num(i) Then
                    If IsNumeric(str2) Then
                        If lnum(i) = CLng(str2) Then
                            GoTo NextPara
                        ElseIf ltype = DamageEventLabel Then
                            '損傷率ラベルの処理
                            If lnum(i) > CLng(str2) Then
                                Exit For
                            End If
                        End If
                    End If
                    GoTo NextLabel
                End If
                
                'ユニット指定として比較？
                If is_unit(i) Then
                    If str2 = "味方" Or str2 = "ＮＰＣ" _
                        Or str2 = "敵" Or str2 = "中立" _
                    Then
                        '陣営名で比較
                        If str1 <> "味方" And str1 <> "ＮＰＣ" _
                            And str1 <> "敵" And str1 <> "中立" _
                        Then
                            If PList.IsDefined(str1) Then
                                str1 = PList.Item(str1).Party
                            End If
                        End If
                    ElseIf PList.IsDefined(str2) Then
                        'パイロットで比較
                        With PList.Item(str2)
                            If str2 = .Data.Name Or str2 = .Data.Nickname Then
                                'グループＩＤが付けられていない場合は
                                'パイロット名で比較
                                str2 = .Name
                                If PList.IsDefined(str1) Then
                                    str1 = PList.Item(str1).Name
                                End If
                            Else
                                'グループＩＤが付けられている場合は
                                'グループＩＤで比較
                                If PList.IsDefined(str1) Then
                                    str1 = PList.Item(str1).ID
                                End If
                                If InStr(str1, ":") > 0 Then
                                    str1 = Left$(str1, InStr(str1, ":") - 1)
                                End If
                            End If
                        End With
                    ElseIf PDList.IsDefined(str2) Then
                        'パイロット名で比較
                        str2 = PDList.Item(str2).Name
                        If PList.IsDefined(str1) Then
                            str1 = PList.Item(str1).Name
                        End If
                    ElseIf UDList.IsDefined(str2) Then
                        'ユニット名で比較
                        If PList.IsDefined(str1) Then
                            With PList.Item(str1)
                                If Not .Unit Is Nothing Then
                                    str1 = .Unit.Name
                                End If
                            End With
                        End If
                    Else
                        'グループＩＤが付けられているおり、なおかつ同じＩＤの
                        '２番目以降のユニットの場合はグループＩＤで比較
                        If PList.IsDefined(str1) Then
                            str1 = PList.Item(str1).ID
                        End If
                        If InStr(str1, ":") > 0 Then
                            str1 = Left$(str1, InStr(str1, ":") - 1)
                        End If
                        If InStr(str2, ":") > 0 Then
                            str2 = Left$(str2, InStr(str2, ":") - 1)
                        End If
                    End If
                End If
                
                '一致したか？
                If str1 <> str2 Then
                    If revrersible And Not reversed Then
                        '対象と相手を入れ替えたイベントラベルが存在するか判定
                        lname2 = litem(1) & _
                            " " & ListIndex(.Data, 3) & _
                            " " & ListIndex(.Data, 2)
                        If .AsterNum > 0 Then
                            lname2 = "*" & lname2
                        End If
                        If FindLabel(lname2) = 0 Then
                            '対象と相手を入れ替えて判定し直す
                            reversed = True
                            GoTo CheckPara
                        End If
                    End If
                    GoTo NextLabel
                End If
NextPara:
            Next
            
            'ここまでたどり付けばラベルは一致している
            SearchLabel = .LineNum
            
            '対象と相手を入れ替えて一致した場合はグローバル変数も入れ替え
            If reversed Then
                Set tmp_u = SelectedUnitForEvent
                Set SelectedUnitForEvent = SelectedTargetForEvent
                Set SelectedTargetForEvent = tmp_u
            End If
            Exit Function
        End With
NextLabel:
    Next
    
    SearchLabel = 0
End Function

'指定したイベントへのイベントラベルが定義されているか
'常時イベントではない通常イベントのみを探す場合は
' normal_event_only = True を指定する
Public Function IsEventDefined(lname As String, _
    Optional ByVal normal_event_only As Boolean) As Boolean
Dim i As Long, ret As Long

    'イベントラベルを探す
    i = 0
    Do While 1
        ret = SearchLabel(lname, i + 1)
        If ret = 0 Then
            Exit Function
        End If
        
        If normal_event_only Then
            '常時イベントではない通常イベントのみを探す場合
            If Asc(EventData(ret)) <> 42 Then '*
                IsEventDefined = True
                Exit Function
            End If
        Else
            IsEventDefined = True
            Exit Function
        End If
        i = ret
    Loop
End Function

'ラベルが定義されているか
Public Function IsLabelDefined(Index As Variant) As Boolean
Dim lab As LabelData

    On Error GoTo ErrorHandler
    Set lab = colEventLabelList.Item(Index)
    IsLabelDefined = True
    Exit Function
    
ErrorHandler:
    IsLabelDefined = False
End Function

'ラベルを追加
Public Sub AddLabel(lname As String, ByVal lnum As Long)
Dim new_label As New LabelData
Dim lname2 As String
Dim i As Integer

    On Error GoTo ErrorHandler
    
    With new_label
        .Data = lname
        .LineNum = lnum
        .Enable = True
        
        If .Name = NormalLabel Then
            '通常ラベルを追加
            If FindNormalLabel0(lname) = 0 Then
                colNormalLabelList.Add new_label, lname
            End If
        Else
            'イベントラベルを追加
            
            'パラメータ間の文字列の違いによる不一致をなくすため、
            '文字列を半角スペース一文字に直しておく
            lname2 = ListIndex(lname, 1)
            For i = 2 To ListLength(lname)
                lname2 = lname2 & " " & ListIndex(lname, i)
            Next
            
            If Not IsLabelDefined(lname2) Then
                colEventLabelList.Add new_label, lname2
            Else
                colEventLabelList.Add new_label, lname2 _
                    & "(" & Format$(lnum) & ")"
            End If
        End If
    End With
    
    Exit Sub
    
ErrorHandler:
    '通常ラベルが重複定義されている場合は無視
End Sub

'システム側のラベルを追加
Public Sub AddSysLabel(lname As String, ByVal lnum As Long)
Dim new_label As New LabelData
Dim lname2 As String
Dim i As Integer

    On Error GoTo ErrorHandler
    
    With new_label
        .Data = lname
        .LineNum = lnum
        .Enable = True
        
        If .Name = NormalLabel Then
            '通常ラベルを追加
            If FindSysNormalLabel(lname) = 0 Then
                colSysNormalLabelList.Add new_label, lname
            Else
                colSysNormalLabelList.Item(lname).LineNum = lnum
            End If
        Else
            'イベントラベルを追加
            
            'パラメータ間の文字列の違いによる不一致をなくすため、
            '文字列を半角スペース一文字に直しておく
            lname2 = ListIndex(lname, 1)
            For i = 2 To ListLength(lname)
                lname2 = lname2 & " " & ListIndex(lname, i)
            Next
            
            If Not IsLabelDefined(lname2) Then
                colEventLabelList.Add new_label, lname2
            Else
                colEventLabelList.Add new_label, lname2 _
                    & "(" & Format$(lnum) & ")"
            End If
        End If
    End With
    
    Exit Sub
    
ErrorHandler:
    '通常ラベルが重複定義されている場合は無視
End Sub

'ラベルを消去
Public Sub ClearLabel(ByVal lnum As Long)
Dim lab As LabelData, i As Integer
    
    '行番号lnumにあるラベルを探す
    For Each lab In colEventLabelList
        With lab
            If .LineNum = lnum Then
                .Enable = False
                Exit Sub
            End If
        End With
    Next
    
    'lnum行目になければその周りを探す
    For i = 1 To 10
        For Each lab In colEventLabelList
            With lab
                If .LineNum = lnum - i Or .LineNum = lnum + i Then
                    .Enable = False
                    Exit Sub
                End If
            End With
        Next
    Next
End Sub

'ラベルを復活
Public Sub RestoreLabel(lname As String)
Dim lab As LabelData
    
    For Each lab In colEventLabelList
        With lab
            If .Data = lname Then
                .Enable = True
                Exit Sub
            End If
        End With
    Next
End Sub

'ラベルを探す
Public Function FindLabel(lname As String) As Long
Dim lname2 As String
Dim i As Integer

    '通常ラベルから検索
    FindLabel = FindNormalLabel(lname)
    If FindLabel > 0 Then
        Exit Function
    End If
    
    'イベントラベルから検索
    FindLabel = FindEventLabel(lname)
    If FindLabel > 0 Then
        Exit Function
    End If
    
    'パラメータ間の文字列の違いで一致しなかった可能性があるので
    '文字列を半角スペース一文字のみにして検索してみる
    lname2 = ListIndex(lname, 1)
    For i = 2 To ListLength(lname)
        lname2 = lname2 & " " & ListIndex(lname, i)
    Next
    
    'イベントラベルから検索
    FindLabel = FindEventLabel(lname2)
End Function

'イベントラベルを探す
Public Function FindEventLabel(lname As String) As Long
Dim lab As LabelData

    On Error GoTo NotFound
    Set lab = colEventLabelList.Item(lname)
    FindEventLabel = lab.LineNum
    Exit Function

NotFound:
    FindEventLabel = 0
End Function

'通常ラベルを探す
Public Function FindNormalLabel(lname As String) As Long
    FindNormalLabel = FindNormalLabel0(lname)
    If FindNormalLabel = 0 Then
        FindNormalLabel = FindSysNormalLabel(lname)
    End If
End Function

'シナリオ側の通常ラベルを探す
Private Function FindNormalLabel0(lname As String) As Long
Dim lab As LabelData

    On Error GoTo NotFound
    Set lab = colNormalLabelList.Item(lname)
    FindNormalLabel0 = lab.LineNum
    Exit Function

NotFound:
    FindNormalLabel0 = 0
End Function

'システム側の通常ラベルを探す
Private Function FindSysNormalLabel(lname As String) As Long
Dim lab As LabelData

    On Error GoTo NotFound
    Set lab = colSysNormalLabelList.Item(lname)
    FindSysNormalLabel = lab.LineNum
    Exit Function

NotFound:
    FindSysNormalLabel = 0
End Function


'イベントデータの消去
'ただしグローバル変数のデータは残しておく
Public Sub ClearEventData()
Dim i As Integer

    Set SelectedUnitForEvent = Nothing
    Set SelectedTargetForEvent = Nothing
    
    ReDim Preserve EventData(SysEventDataSize)
    
    With colNormalLabelList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    i = 1
    With colEventLabelList
        Do While i <= .Count
            If .Item(i).LineNum > SysEventDataSize Then
                .Remove i
            Else
                i = i + 1
            End If
        Loop
    End With
    
    ReDim EventQue(0)
    
    CallDepth = 0
    ArgIndex = 0
    VarIndex = 0
    ForIndex = 0
    UpVarLevel = 0
    ReDim HotPointList(0)
    ObjColor = vbWhite
    ObjFillColor = vbWhite
    ObjFillStyle = vbFSTransparent
    ObjDrawWidth = 1
    ObjDrawOption = ""
    
    IsPictureVisible = False
    IsCursorVisible = False
    
    PaintedAreaX1 = MainPWidth
    PaintedAreaY1 = MainPHeight
    PaintedAreaX2 = -1
    PaintedAreaY2 = -1
    
    With LocalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
End Sub

'グローバル変数を含めたイベントデータの全消去
Public Sub ClearAllEventData()
Dim i As Integer

    ClearEventData
    
    With GlobalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    
    DefineGlobalVariable "次ステージ"
    DefineGlobalVariable "セーブデータファイル名"
End Sub


'一時中断用データをファイルにセーブする
Public Sub DumpEventData()
Dim lab As LabelData, i As Integer

    'グローバル変数
    SaveGlobalVariables
    'ローカル変数
    SaveLocalVariables
    
    'イベント用ラベル
    Write #SaveDataFileNumber, colEventLabelList.Count
    For Each lab In colEventLabelList
        Write #SaveDataFileNumber, lab.Enable
    Next
    
    'Requireコマンドで追加されたイベントファイル
    Write #SaveDataFileNumber, UBound(AdditionalEventFileNames)
    For i = 1 To UBound(AdditionalEventFileNames)
        Write #SaveDataFileNumber, AdditionalEventFileNames(i)
    Next
End Sub

'一時中断用データをファイルからロードする
Public Sub RestoreEventData()
Dim lab As LabelData
Dim num As Integer, lenable As Boolean
Dim fname As String, file_head As Long
Dim i As Long, j As Integer, buf As String

    'グローバル変数
    LoadGlobalVariables
    'ローカル変数
    LoadLocalVariables
    
    'イベント用ラベル
    Input #SaveDataFileNumber, num
' MOD START MARGE
'    i = 1
'    For Each lab In colEventLabelList
'        If i <= num Then
'            Input #SaveDataFileNumber, lenable
'            lab.Enable = lenable
'        Else
'            lab.Enable = True
'        End If
'        i = i + 1
'    Next
'    Do While i <= num
'        Input #SaveDataFileNumber, buf
'        i = i + 1
'    Loop
    ReDim label_enabled(num)
    For i = 1 To num
        Input #SaveDataFileNumber, label_enabled(i)
    Next
' MOD END MARGE
    
    'Requireコマンドで追加されたイベントファイル
    If SaveDataVersion > 20003 Then
        file_head = UBound(EventData) + 1
        
' MOD START MARGE
'        'イベントファイルをロード
'        Input #SaveDataFileNumber, num
'        If num = 0 Then
'            Exit Sub
'        End If
'        ReDim AdditionalEventFileNames(num)
'        For i = 1 To num
'            Input #SaveDataFileNumber, fname
'            AdditionalEventFileNames(i) = fname
'            If InStr(fname, ":") = 0 Then
'                fname = ScenarioPath & fname
'            End If
'
'            '既に読み込まれている場合はスキップ
'            For j = 1 To UBound(EventFileNames)
'               If fname = EventFileNames(j) Then
'                   GoTo NextEventFile
'               End If
'            Next
'
'            LoadEventData2 fname, UBound(EventData)
'NextEventFile:
'        Next
'
'        'エラー表示用にサイズを大きく取っておく
'        ReDim Preserve EventData(UBound(EventData) + 1)
'        ReDim Preserve EventLineNum(UBound(EventData))
'        EventData(UBound(EventData)) = ""
'        EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
'
'        '複数行に分割されたコマンドを結合
'        For i = file_head To UBound(EventData) - 1
'            If Right$(EventData(i), 1) = "_" Then
'                EventData(i + 1) = _
'                    Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
'                EventData(i) = " "
'            End If
'        Next
'
'        'ラベルを登録
'        For i = file_head To UBound(EventData)
'            buf = EventData(i)
'            If Right$(buf, 1) = ":" Then
'                AddLabel Left$(buf, Len(buf) - 1), i
'            End If
'        Next
'
'        'コマンドデータ配列を設定
'        If UBound(EventData) > UBound(EventCmd) Then
'            ReDim Preserve EventCmd(UBound(EventData))
'            i = UBound(EventData)
'            Do While EventCmd(i) Is Nothing
'                Set EventCmd(i) = New CmdData
'                EventCmd(i).LineNum = i
'                i = i - 1
'            Loop
'        End If
'        For i = file_head To UBound(EventData)
'            EventCmd(i).Name = NullCmd
'        Next
'    End If
        '追加するイベントファイル数
        Input #SaveDataFileNumber, num
        
        If num > 0 Then
            'イベントファイルをロード
            ReDim AdditionalEventFileNames(num)
            For i = 1 To num
                Input #SaveDataFileNumber, fname
                AdditionalEventFileNames(i) = fname
                If InStr(fname, ":") = 0 Then
                    fname = ScenarioPath & fname
                End If
                
                '既に読み込まれている場合はスキップ
                For j = 1 To UBound(EventFileNames)
                   If fname = EventFileNames(j) Then
                       GoTo NextEventFile
                   End If
                Next
                
                LoadEventData2 fname, UBound(EventData)
NextEventFile:
            Next
            
            'エラー表示用にサイズを大きく取っておく
            ReDim Preserve EventData(UBound(EventData) + 1)
            ReDim Preserve EventLineNum(UBound(EventData))
            EventData(UBound(EventData)) = ""
            EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
            
            '複数行に分割されたコマンドを結合
            For i = file_head To UBound(EventData) - 1
                If Right$(EventData(i), 1) = "_" Then
                    EventData(i + 1) = _
                        Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
                    EventData(i) = " "
                End If
            Next
            
            'ラベルを登録
            For i = file_head To UBound(EventData)
                buf = EventData(i)
                If Right$(buf, 1) = ":" Then
                    AddLabel Left$(buf, Len(buf) - 1), i
                End If
            Next
            
            'コマンドデータ配列を設定
            If UBound(EventData) > UBound(EventCmd) Then
                ReDim Preserve EventCmd(UBound(EventData))
                i = UBound(EventData)
                Do While EventCmd(i) Is Nothing
                    Set EventCmd(i) = New CmdData
                    EventCmd(i).LineNum = i
                    i = i - 1
                Loop
            End If
            For i = file_head To UBound(EventData)
                EventCmd(i).Name = NullCmd
            Next
        End If
    End If
    
    'イベント用ラベルを設定
    i = 1
    num = UBound(label_enabled)
    For Each lab In colEventLabelList
        If i <= num Then
            lab.Enable = label_enabled(i)
        Else
            lab.Enable = True
        End If
        i = i + 1
    Next
' MOD END MARGE
End Sub

'一時中断用データのイベントデータ部分を読み飛ばす
Public Sub SkipEventData()
Dim i As Integer, num As Integer
Dim dummy As String

    'グローバル変数
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    'ローカル変数
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    
    'ラベル情報
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    
    'Requireコマンドで読み込んだイベントデータ
    If SaveDataVersion > 20003 Then
        Input #SaveDataFileNumber, num
        For i = 1 To num
            Line Input #SaveDataFileNumber, dummy
        Next
    End If
End Sub

'グローバル変数をファイルにセーブ
Public Sub SaveGlobalVariables()
Dim var As VarData
    
    Write #SaveDataFileNumber, GlobalVariableList.Count
    For Each var In GlobalVariableList
        With var
            If .VariableType = StringType Then
                Write #SaveDataFileNumber, .Name, .StringValue
            Else
                Write #SaveDataFileNumber, .Name, Format$(.NumericValue)
            End If
        End With
    Next
End Sub

'グローバル変数をファイルからロード
Public Sub LoadGlobalVariables()
Dim i As Integer, j As Integer, k As Integer, num As Integer, idx As Integer
Dim vname As String, vvalue As String, buf As String
Dim aname As String
' ADD START MARGE
Dim is_number As Boolean
' ADD END MARGE
    'グローバル変数を全削除
    With GlobalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    
    'グローバル変数の総数を読み出し
    Input #SaveDataFileNumber, num
    
    '各変数の値を読み出し
    For i = 1 To num
        Input #SaveDataFileNumber, vname
        Line Input #SaveDataFileNumber, buf
' MOD START MARGE
'        vvalue = Mid$(buf, 2, Len(buf) - 2)
'        ReplaceString vvalue, """""", """"
        If Left$(buf, 1) = """" Then
            is_number = False
            vvalue = Mid$(buf, 2, Len(buf) - 2)
            ReplaceString vvalue, """""", """"
        Else
            is_number = True
            vvalue = buf
        End If
' MOD END MARGE
        
        If SaveDataVersion < 10724 Then
            'SetSkillコマンドのセーブデータをエリアスに対応させる
            If Left$(vname, 8) = "Ability(" Then
                idx = InStr(vname, ",")
                If idx > 0 Then
                    '個々の能力定義
                    aname = Mid$(vname, idx + 1, Len(vname) - idx - 1)
                    If ALDList.IsDefined(aname) Then
                        vname = Left$(vname, idx) & ALDList.Item(aname).AliasType(1) & ")"
                        If LLength(vvalue) = 1 Then
                            vvalue = vvalue & " " & aname
                        End If
                    End If
                Else
                    '必要技能用の能力一覧
                    buf = ""
                    For j = 1 To LLength(vvalue)
                        aname = LIndex(vvalue, j)
                        If ALDList.IsDefined(aname) Then
                            aname = ALDList.Item(aname).AliasType(1)
                        End If
                        buf = buf & " " & aname
                    Next
                    vvalue = Trim$(buf)
                End If
            End If
        End If
        
        If SaveDataVersion < 10730 Then
            'ラーニングした特殊能力が使えないバグに対応
            If Left$(vname, 8) = "Ability(" Then
                idx = InStr(vname, ",")
                If idx > 0 Then
                    Dim vname2 As String
                    vname2 = Left$(vname, idx - 1) & ")"
                    aname = Mid$(vname, idx + 1, Len(vname) - idx - 1)
                    If Not IsGlobalVariableDefined(vname2) Then
                        DefineGlobalVariable vname2
                        GlobalVariableList.Item(vname2).StringValue = aname
                    End If
                End If
            End If
        End If
        
        If SaveDataVersion < 10731 Then
            '不必要な非表示能力に対するSetSkillを削除
            If Left$(vname, 8) = "Ability(" Then
                If Right$(vname, 5) = ",非表示)" Then
                    GoTo NextVariable
                End If
            End If
        End If
        
        If SaveDataVersion < 10732 Then
            '不必要な非表示能力に対するSetSkillと能力名のダブりを削除
            If Left$(vname, 8) = "Ability(" Then
                If InStr(vname, ",") = 0 Then
                    buf = ""
                    For j = 1 To LLength(vvalue)
                        aname = LIndex(vvalue, j)
                        If aname <> "非表示" Then
                            For k = 1 To LLength(buf)
                                If LIndex(buf, k) = aname Then
                                    Exit For
                                End If
                            Next
                            If k > LLength(buf) Then
                                buf = buf & " " & aname
                            End If
                        End If
                    Next
                    vvalue = Trim$(buf)
                End If
            End If
        End If
        
        If SaveDataVersion < 20027 Then
            'エリアスされた能力をSetSkillした際にエリアスに含まれる解説が無効になるバグへの対処
            If Left$(vname, 8) = "Ability(" Then
                If LIndex(vvalue, 1) = "0" Then
                    If LIndex(vvalue, 2) = "解説" Then
                        vvalue = Format$(DEFAULT_LEVEL) & " 解説 " & ListTail(vvalue, 3)
                    End If
                End If
            End If
        End If
        
        If Not IsGlobalVariableDefined(vname) Then
            DefineGlobalVariable vname
        End If
        With GlobalVariableList.Item(vname)
            .StringValue = vvalue
' MOD START MARGE
'            If IsNumber(vvalue) Then
            If is_number Then
' MOD END MARGE
                .VariableType = NumericType
                .NumericValue = CDbl(vvalue)
            Else
                .VariableType = StringType
            End If
        End With
NextVariable:
    Next
'ADD START 240a
    'Optionを全て読み込んだら、新ＧＵＩが有効になっているか確認する
    SetNewGUIMode
'ADD  END  240a
End Sub

'ローカル変数をファイルにセーブ
Public Sub SaveLocalVariables()
Dim var As VarData
    
    Write #SaveDataFileNumber, LocalVariableList.Count
    For Each var In LocalVariableList
        With var
            If .VariableType = StringType Then
                Write #SaveDataFileNumber, .Name, .StringValue
            Else
                Write #SaveDataFileNumber, .Name, Format$(.NumericValue)
            End If
            If InStr(.Name, """") > 0 Then
                ErrorMessage .Name
            End If
        End With
    Next
End Sub

'ローカル変数をファイルからロード
Public Sub LoadLocalVariables()
Dim i As Integer, num As Integer
' MOD START MARGE
'Dim vname As String, vvalue As String
Dim vname As String, vvalue As String, buf As String
Dim is_number As Boolean
' MOD END MARGE
    'ローカル変数を全削除
    With LocalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    
    'ローカル変数の総数を読み出し
    Input #SaveDataFileNumber, num
    
    For i = 1 To num
        '変数の値を読み出し
' MOD START MARGE
'        Input #SaveDataFileNumber, vname, vvalue
        Input #SaveDataFileNumber, vname
        Line Input #SaveDataFileNumber, buf
        If Left$(buf, 1) = """" Then
            is_number = False
            vvalue = Mid$(buf, 2, Len(buf) - 2)
            ReplaceString vvalue, """""", """"
        Else
            is_number = True
            vvalue = buf
        End If
' MOD END MARGE
        
        If SaveDataVersion < 10731 Then
            'ClearSkillのバグで設定された変数を削除
            If Left$(vname, 8) = "Ability(" Then
                If vname = vvalue Then
                    GoTo NextVariable
                End If
            End If
        End If
        
        '変数の値を設定
        If Not IsLocalVariableDefined(vname) Then
            DefineLocalVariable vname
        End If
        With LocalVariableList.Item(vname)
            .StringValue = vvalue
' MOD START MARGE
'            If IsNumber(vvalue) Then
            If is_number Then
' MOD END MARGE
                .VariableType = NumericType
                .NumericValue = CDbl(vvalue)
            Else
                .VariableType = StringType
            End If
        End With
NextVariable:
    Next
End Sub


'イベントエラー表示
Public Sub DisplayEventErrorMessage(ByVal lnum As Long, ByVal msg As String)
Dim buf As String
    
    'エラーが起こったファイル、行番号、エラーメッセージを表示
    buf = EventFileNames(EventFileID(lnum)) & "：" _
        & EventLineNum(lnum) & "行目" & vbCr & vbLf _
        & msg & vbCr & vbLf
    
    'エラーが起こった行とその前後の行の内容を表示
    If lnum > 1 Then
        buf = buf & EventLineNum(lnum - 1) & ": " & EventData(lnum - 1) & vbCr & vbLf
    End If
    buf = buf & EventLineNum(lnum) & ": " & EventData(lnum) & vbCr & vbLf
    If lnum < UBound(EventData) Then
        buf = buf & EventLineNum(lnum + 1) & ": " & EventData(lnum + 1) & vbCr & vbLf
    End If
    
    ErrorMessage buf
End Sub

'インターミッションコマンド「ユニットリスト」におけるユニットリストを作成する
Public Sub MakeUnitList(Optional smode As String)
Dim u As Unit, p As Pilot
Dim xx As Integer, yy As Integer
Dim key_list() As Long
Dim max_item As Integer, max_value As Long
Dim max_str As String
Dim unit_list() As Unit
Dim i As Integer, j As Integer
Static key_type As String
    
    'リストのソート項目を設定
    If smode <> "" Then
        key_type = smode
    End If
    If key_type = "" Then
        key_type = "ＨＰ"
    End If
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    'あらかじめ撤退させておく
    For Each u In UList
        With u
            If .Status = "出撃" Then
                .Escape
            End If
        End With
    Next
    
    'マップをクリア
    LoadMapData ""
    SetupBackground "", "ステータス"
    
    'ユニット一覧を作成
    If key_type <> "名称" Then
        '配列作成
        ReDim unit_list(UList.Count)
        ReDim key_list(UList.Count)
        i = 0
        For Each u In UList
            With u
                If .Status = "出撃" Or .Status = "待機" Then
                    i = i + 1
                    Set unit_list(i) = u
                    
                    'ソートする項目にあわせてソートの際の優先度を決定
                    Select Case key_type
                        Case "ランク"
                            key_list(i) = .Rank
                        Case "ＨＰ"
                            key_list(i) = .HP
                        Case "ＥＮ"
                            key_list(i) = .EN
                        Case "装甲"
                            key_list(i) = .Armor
                        Case "運動性"
                            key_list(i) = .Mobility
                        Case "移動力"
                            key_list(i) = .Speed
                        Case "最大攻撃力"
                            For j = 1 To .CountWeapon
                                If .IsWeaponMastered(j) _
                                    And Not .IsDisabled(.Weapon(j).Name) _
                                    And Not .IsWeaponClassifiedAs(j, "合") _
                                Then
                                    If .WeaponPower(j, "") > key_list(i) Then
                                        key_list(i) = .WeaponPower(j, "")
                                    End If
                                End If
                            Next
                        Case "最長射程"
                            For j = 1 To .CountWeapon
                                If .IsWeaponMastered(j) _
                                    And Not .IsDisabled(.Weapon(j).Name) _
                                    And Not .IsWeaponClassifiedAs(j, "合") _
                                Then
                                    If .WeaponMaxRange(j) > key_list(i) Then
                                        key_list(i) = .WeaponMaxRange(j)
                                    End If
                                End If
                            Next
                        Case "レベル"
                            key_list(i) = .MainPilot.Level
                        Case "ＳＰ"
                            key_list(i) = .MainPilot.MaxSP
                        Case "格闘"
                            key_list(i) = .MainPilot.Infight
                        Case "射撃"
                            key_list(i) = .MainPilot.Shooting
                        Case "命中"
                            key_list(i) = .MainPilot.Hit
                        Case "回避"
                            key_list(i) = .MainPilot.Dodge
                        Case "技量"
                            key_list(i) = .MainPilot.Technique
                        Case "反応"
                            key_list(i) = .MainPilot.Intuition
                    End Select
                End If
            End With
        Next
        ReDim Preserve unit_list(i)
        ReDim Preserve key_list(i)
        
        'ソート
        For i = 1 To UBound(key_list) - 1
            max_item = i
            max_value = key_list(i)
            For j = i + 1 To UBound(unit_list)
                If key_list(j) > max_value Then
                    max_item = j
                    max_value = key_list(j)
                End If
            Next
            If max_item <> i Then
                Set u = unit_list(i)
                Set unit_list(i) = unit_list(max_item)
                Set unit_list(max_item) = u
                
                max_value = key_list(max_item)
                key_list(max_item) = key_list(i)
                key_list(i) = max_value
            End If
        Next
    Else
        '配列作成
        ReDim unit_list(UList.Count)
        ReDim strkey_list(UList.Count)
        i = 0
        For Each u In UList
            With u
                If .Status = "出撃" Or .Status = "待機" Then
                    i = i + 1
                    Set unit_list(i) = u
                    If IsOptionDefined("等身大基準") Then
                        strkey_list(i) = .MainPilot.KanaName
                    Else
                        strkey_list(i) = .KanaName
                    End If
                End If
            End With
        Next
        ReDim Preserve unit_list(i)
        ReDim Preserve strkey_list(i)
        
        'ソート
        For i = 1 To UBound(strkey_list) - 1
            max_item = i
            max_str = strkey_list(i)
            For j = i + 1 To UBound(strkey_list)
                If StrComp(strkey_list(j), max_str, 1) = -1 Then
                    max_item = j
                    max_str = strkey_list(j)
                End If
            Next
            If max_item <> i Then
                Set u = unit_list(i)
                Set unit_list(i) = unit_list(max_item)
                Set unit_list(max_item) = u
                
                strkey_list(max_item) = strkey_list(i)
            End If
        Next
    End If
    
    'Font Regular 9pt 背景
    With MainForm.picMain(0).Font
        .Size = 9
        .Bold = False
        .Italic = False
    End With
    PermanentStringMode = True
    HCentering = False
    VCentering = False
    
    'ユニットのリストを作成
    xx = 1
    yy = 1
    For i = 1 To UBound(unit_list)
        Set u = unit_list(i)
        With u
            'ユニット出撃位置を折り返す
            If xx > 15 Then
                xx = 1
                yy = yy + 1
                If yy > 40 Then
                    'ユニット数が多すぎるため、一部のパイロットが表示出来ません
                    Exit For
                End If
            End If
            
            'パイロットが乗っていない場合はダミーパイロットを乗せる
            If .CountPilot = 0 Then
                Set p = PList.Add("ステータス表示用ダミーパイロット(ザコ)", 1, "味方")
                p.Ride u
            End If
            
            '出撃
            .UsedAction = 0
            .StandBy xx, yy
            
            'プレイヤーが操作できないように
            .AddCondition "非操作", -1
            
            'ユニットの愛称を表示
            DrawString .Nickname, 32 * xx + 2, 32 * yy - 31
            
            'ソート項目にあわせてユニットのステータスを表示
            Select Case key_type
                Case "ランク"
                    DrawString _
                        "RK" & Format$(key_list(i)) & " " & Term("HP", u) & Format$(.HP) _
                            & " " & Term("EN", u) & Format$(.EN), _
                        32 * xx + 2, 32 * yy - 15
                Case "ＨＰ", "ＥＮ", "名称"
                    DrawString _
                        Term("HP", u) & Format$(.HP) & " " & Term("EN", u) & Format$(.EN), _
                        32 * xx + 2, 32 * yy - 15
                Case "装甲"
                    DrawString Term("装甲", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "運動性"
                    DrawString Term("運動性", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "移動力"
                    DrawString Term("移動力", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "最大攻撃力"
                    DrawString "攻撃力" & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "最長射程"
                    DrawString "射程" & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "レベル"
                    DrawString "Lv" & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "ＳＰ"
                    DrawString Term("SP", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "格闘"
                    DrawString Term("格闘", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "射撃"
                    If .MainPilot.HasMana() Then
                        DrawString Term("魔力", u) & Format$(key_list(i)), _
                            32 * xx + 2, 32 * yy - 15
                    Else
                        DrawString Term("射撃", u) & Format$(key_list(i)), _
                            32 * xx + 2, 32 * yy - 15
                    End If
                Case "命中"
                    DrawString Term("命中", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "回避"
                    DrawString Term("回避", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "技量"
                    DrawString Term("技量", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "反応"
                    DrawString Term("反応", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
            End Select
            
            '表示位置を右に5マスずらす
            xx = xx + 5
        End With
    Next
    
    'フォントの設定を戻しておく
    With MainForm.picMain(0).Font
        .Size = 16
        .Bold = True
        .Italic = False
    End With
    PermanentStringMode = False
    
    RedrawScreen
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
End Sub


'描画の基準座標位置を保存
Public Sub SaveBasePoint()
    BasePointIndex = BasePointIndex + 1
    If BasePointIndex > UBound(SavedBaseX) Then
        BasePointIndex = 0
    End If
    SavedBaseX(BasePointIndex) = BaseX
    SavedBaseY(BasePointIndex) = BaseY
End Sub

'描画の基準座標位置を復元
Public Sub RestoreBasePoint()
    If BasePointIndex <= 0 Then
        BasePointIndex = UBound(SavedBaseX)
    End If
    BaseX = SavedBaseX(BasePointIndex)
    BaseY = SavedBaseY(BasePointIndex)
    BasePointIndex = BasePointIndex - 1
End Sub

'描画の基準座標位置をリセット
Public Sub ResetBasePoint()
Dim i As Integer

    BaseX = 0
    BaseY = 0
    BasePointIndex = 0
    For i = 1 To UBound(SavedBaseX)
        SavedBaseX(i) = 0
        SavedBaseY(i) = 0
    Next
End Sub

