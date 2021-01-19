Attribute VB_Name = "Event"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'�C�x���g�f�[�^�̊e�폈�����s�����W���[��

'�C�x���g�f�[�^
Public EventData() As String
'�C�x���g�R�}���h���X�g
Public EventCmd() As CmdData
'�X�̍s���ǂ̃C�x���g�t�@�C���ɑ����Ă��邩
Public EventFileID() As Integer
'�X�̍s���C�x���g�t�@�C���̉��s�ڂɈʒu���邩
Public EventLineNum() As Integer
'�C�x���g�t�@�C���̃t�@�C�������X�g
Public EventFileNames() As String
'Require�R�}���h�Œǉ����ꂽ�C�x���g�t�@�C���̃t�@�C�������X�g
Public AdditionalEventFileNames() As String

'�V�X�e�����̃C�x���g�f�[�^�̃T�C�Y(�s��)
Private SysEventDataSize As Long
'�V�X�e�����̃C�x���g�t�@�C����
Private SysEventFileNum As Integer
'�V�i���I�Y�t�̃V�X�e���t�@�C�����`�F�b�N���ꂽ���ǂ���
Private ScenarioLibChecked As Boolean

'���x���̃��X�g
Public colEventLabelList As New Collection
Private colSysNormalLabelList As New Collection
Private colNormalLabelList As New Collection


'�ϐ��p�̃R���N�V����
Public GlobalVariableList As New Collection
Public LocalVariableList As New Collection

'���݂̍s�ԍ�
Public CurrentLineNum As Long

'�C�x���g�őI������Ă��郆�j�b�g�E�^�[�Q�b�g
Public SelectedUnitForEvent As Unit
Public SelectedTargetForEvent As Unit

'�C�x���g�Ăяo���̃L���[
Public EventQue() As String
'���ݎ��s���̃C�x���g���x��
Public CurrentLabel As Long

'Ask�R�}���h�őI�������I����
Public SelectedAlternative As String

'�֐��Ăяo���p�ϐ�

'�ő�Ăяo���K�w��
Public Const MaxCallDepth = 50
'�����̍ő吔
Public Const MaxArgIndex = 200
'�T�u���[�`�����[�J���ϐ��̍ő吔
Public Const MaxVarIndex = 2000

'�Ăяo������
Public CallDepth As Integer
Public CallStack(MaxCallDepth) As Long
'�����X�^�b�N
Public ArgIndex As Integer
Public ArgIndexStack(MaxCallDepth) As Integer
Public ArgStack(MaxArgIndex) As String
'UpVar�R�}���h�ɂ���Ĉ��������i�K�V�t�g���Ă��邩
Public UpVarLevel As Integer
Public UpVarLevelStack(MaxCallDepth) As Integer
'�T�u���[�`�����[�J���ϐ��X�^�b�N
Public VarIndex As Integer
Public VarIndexStack(MaxCallDepth) As Integer
Public VarStack(MaxVarIndex) As VarData
'For�C���f�b�N�X�p�X�^�b�N
Public ForIndex As Integer
Public ForIndexStack(MaxCallDepth) As Integer
Public ForLimitStack(MaxCallDepth) As Long

'ForEach�R�}���h�p�ϐ�
Public ForEachIndex As Integer
Public ForEachSet() As String

'Ride�R�}���h�p�p�C���b�g���旚��
Public LastUnitName As String
Public LastPilotID() As String

'Wait�J�n����
Public WaitStartTime As Long
Public WaitTimeCount As Long

'�`�����W
Public BaseX As Long
Public BaseY As Long
Private SavedBaseX(10) As Long
Private SavedBaseY(10) As Long
Private BasePointIndex As Long

'�I�u�W�F�N�g�̐F
Public ObjColor As Long
'�I�u�W�F�N�g�̐��̑���
Public ObjDrawWidth As Long
'�I�u�W�F�N�g�̔w�i�F
Public ObjFillColor As Long
'�I�u�W�F�N�g�̔w�i�`����@
Public ObjFillStyle As Long
'�I�u�W�F�N�g�̕`����@
Public ObjDrawOption As String

'�z�b�g�|�C���g
Public Type HotPoint
    Name As String
    Left As Integer
    Top As Integer
    width As Integer
    Height As Integer
    Caption As String
End Type
Public HotPointList() As HotPoint

'�C�x���g�R�}���h�G���[���b�Z�[�W
Public EventErrorMessage As String

'���j�b�g���Z���^�����O���ꂽ���H
Public IsUnitCenter As Boolean


'�C�x���g�R�}���h�̎��
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

'�C�x���g���x���̎��
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


'�C�x���g�f�[�^��������
Public Sub InitEventData()
Dim i As Long

    ReDim Titles(0)
    ReDim EventData(0)
    ReDim EventCmd(50000)
    ReDim EventQue(0)
    
    '�I�u�W�F�N�g�̐����ɂ͎��Ԃ�������̂ŁA
    '���炩����CmdData�I�u�W�F�N�g�𐶐����Ă����B
    For i = 1 To UBound(EventCmd)
        Set EventCmd(i) = New CmdData
        EventCmd(i).LineNum = i
    Next
    
    '�{�̑��̃V�i���I�f�[�^���`�F�b�N����
    LoadEventData "", "�V�X�e��"
End Sub

'�C�x���g�t�@�C���̃��[�h
Public Sub LoadEventData(fname As String, Optional load_mode As String)
Dim buf As String, buf2 As String
Dim tname As String, tfolder As String, new_titles() As String
Dim i As Long, j As Integer, num As Long
Dim CmdStack(50) As CmdType, CmdStackIdx As Integer
Dim CmdPosStack(50) As Long, CmdPosStackIdx As Integer
Dim error_found As Boolean
Dim sys_event_data_size As Long
Dim sys_event_file_num As Long
    
    '�f�[�^�̏�����
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
    
    '���x���̏�����
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
    
    '�f�o�b�O���[�h�̐ݒ�
    If LCase$(ReadIni("Option", "DebugMode")) = "on" Then
        If Not IsOptionDefined("�f�o�b�O") Then
            DefineGlobalVariable "Option(�f�o�b�O)"
        End If
        SetVariableAsLong "Option(�f�o�b�O)", 1
    End If
    
    '�V�X�e�����̃C�x���g�f�[�^�̃��[�h
    If load_mode = "�V�X�e��" Then
        '�{�̑��̃V�X�e���f�[�^���`�F�b�N
        
        '�X�y�V�����p���[�A�j���p�C���N���[�h�t�@�C�����_�E�����[�h
        If FileExists(ExtDataPath & "Lib\�X�y�V�����p���[.eve") Then
            LoadEventData2 ExtDataPath & "Lib\�X�y�V�����p���[.eve"
        ElseIf FileExists(ExtDataPath2 & "Lib\�X�y�V�����p���[.eve") Then
            LoadEventData2 ExtDataPath2 & "Lib\�X�y�V�����p���[.eve"
        ElseIf FileExists(AppPath & "Lib\�X�y�V�����p���[.eve") Then
            LoadEventData2 AppPath & "Lib\�X�y�V�����p���[.eve"
        ElseIf FileExists(ExtDataPath & "Lib\���_�R�}���h.eve") Then
            LoadEventData2 ExtDataPath & "Lib\���_�R�}���h.eve"
        ElseIf FileExists(ExtDataPath2 & "Lib\���_�R�}���h.eve") Then
            LoadEventData2 ExtDataPath2 & "Lib\���_�R�}���h.eve"
        ElseIf FileExists(AppPath & "Lib\���_�R�}���h.eve") Then
            LoadEventData2 AppPath & "Lib\���_�R�}���h.eve"
        End If
        
        '�ėp�퓬�A�j���p�C���N���[�h�t�@�C�����_�E�����[�h
        If LCase$(ReadIni("Option", "BattleAnimation")) <> "off" Then
            BattleAnimation = True
        End If
        If FileExists(ExtDataPath & "Lib\�ėp�퓬�A�j��\include.eve") Then
            LoadEventData2 ExtDataPath & "Lib\�ėp�퓬�A�j��\include.eve"
        ElseIf FileExists(ExtDataPath2 & "Lib\�ėp�퓬�A�j��\include.eve") Then
            LoadEventData2 ExtDataPath2 & "Lib\�ėp�퓬�A�j��\include.eve"
        ElseIf FileExists(AppPath & "Lib\�ėp�퓬�A�j��\include.eve") Then
            LoadEventData2 AppPath & "Lib\�ėp�퓬�A�j��\include.eve"
        Else
            '�퓬�A�j���\���؂�ւ��R�}���h���\����
            BattleAnimation = False
        End If
        
        '�V�X�e�����̃C�x���g�f�[�^�̑��s�����t�@�C�������L�^���Ă���
        sys_event_data_size = UBound(EventData)
        sys_event_file_num = UBound(EventFileNames)
    ElseIf Not ScenarioLibChecked Then
        '�V�i���I���̃V�X�e���f�[�^���`�F�b�N
        
        ScenarioLibChecked = True
        
        If FileExists(ScenarioPath & "Lib\�X�y�V�����p���[.eve") _
            Or FileExists(ScenarioPath & "Lib\���_�R�}���h.eve") _
            Or FileExists(ScenarioPath & "Lib\�ėp�퓬�A�j��\include.eve") _
        Then
            '�V�X�e���f�[�^�̃��[�h����蒼��
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
            
            '�X�y�V�����p���[�A�j���p�C���N���[�h�t�@�C�����_�E�����[�h
            If FileExists(ScenarioPath & "Lib\�X�y�V�����p���[.eve") Then
                LoadEventData2 ScenarioPath & "Lib\�X�y�V�����p���[.eve"
            ElseIf FileExists(ScenarioPath & "Lib\���_�R�}���h.eve") Then
                LoadEventData2 ScenarioPath & "Lib\���_�R�}���h.eve"
            ElseIf FileExists(ExtDataPath & "Lib\�X�y�V�����p���[.eve") Then
                LoadEventData2 ExtDataPath & "Lib\�X�y�V�����p���[.eve"
            ElseIf FileExists(ExtDataPath2 & "Lib\�X�y�V�����p���[.eve") Then
                LoadEventData2 ExtDataPath2 & "Lib\�X�y�V�����p���[.eve"
            ElseIf FileExists(AppPath & "Lib\�X�y�V�����p���[.eve") Then
                LoadEventData2 AppPath & "Lib\�X�y�V�����p���[.eve"
            ElseIf FileExists(ExtDataPath & "Lib\���_�R�}���h.eve") Then
                LoadEventData2 ExtDataPath & "Lib\���_�R�}���h.eve"
            ElseIf FileExists(ExtDataPath2 & "Lib\���_�R�}���h.eve") Then
                LoadEventData2 ExtDataPath2 & "Lib\���_�R�}���h.eve"
            ElseIf FileExists(AppPath & "Lib\���_�R�}���h.eve") Then
                LoadEventData2 AppPath & "Lib\���_�R�}���h.eve"
            End If
            
            '�ėp�퓬�A�j���p�C���N���[�h�t�@�C�����_�E�����[�h
            If LCase$(ReadIni("Option", "BattleAnimation")) <> "off" Then
                BattleAnimation = True
            End If
            If FileExists(ScenarioPath & "Lib\�ėp�퓬�A�j��\include.eve") Then
                LoadEventData2 ScenarioPath & "Lib\�ėp�퓬�A�j��\include.eve"
            ElseIf FileExists(ExtDataPath & "Lib\�ėp�퓬�A�j��\include.eve") Then
                LoadEventData2 ExtDataPath & "Lib\�ėp�퓬�A�j��\include.eve"
            ElseIf FileExists(ExtDataPath2 & "Lib\�ėp�퓬�A�j��\include.eve") Then
                LoadEventData2 ExtDataPath2 & "Lib\�ėp�퓬�A�j��\include.eve"
            ElseIf FileExists(AppPath & "Lib\�ėp�퓬�A�j��\include.eve") Then
                LoadEventData2 AppPath & "Lib\�ėp�퓬�A�j��\include.eve"
            Else
                '�퓬�A�j���\���؂�ւ��R�}���h���\����
                BattleAnimation = False
            End If
        End If
        
        '�V�i���I�Y�t�̔ėp�C���N���[�h�t�@�C�����_�E�����[�h
        If FileExists(ScenarioPath & "Lib\include.eve") Then
            LoadEventData2 ScenarioPath & "Lib\include.eve"
        End If
        
        '�V�X�e�����̃C�x���g�f�[�^�̑��s�����t�@�C�������L�^���Ă���
        sys_event_data_size = UBound(EventData)
        sys_event_file_num = UBound(EventFileNames)
        
        '�V�i���I���̃C�x���g�f�[�^�̃��[�h
        LoadEventData2 fname
    Else
        '�V�i���I���̃C�x���g�f�[�^�̃��[�h
        LoadEventData2 fname
    End If
    
    '�G���[�\���p�ɃT�C�Y��傫������Ă���
    ReDim Preserve EventData(UBound(EventData) + 1)
    ReDim Preserve EventLineNum(UBound(EventData))
    EventData(UBound(EventData)) = ""
    EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
    
    '�f�[�^�ǂ݂��ݎw��
    For i = SysEventDataSize + 1 To UBound(EventData)
        If Left$(EventData(i), 1) = "@" Then
            tname = Mid$(EventData(i), 2)
            
            '���ɂ��̃f�[�^���ǂݍ��܂�Ă��邩�`�F�b�N
            For j = 1 To UBound(Titles)
                If tname = Titles(j) Then
                    Exit For
                End If
            Next
            
            If j > UBound(Titles) Then
                '�t�H���_������
                tfolder = SearchDataFolder(tname)
                If Len(tfolder) = 0 Then
                    DisplayEventErrorMessage _
                        i, "�f�[�^�u" & tname & "�v�̃t�H���_��������܂���"
                Else
                    ReDim Preserve new_titles(UBound(new_titles) + 1)
                    ReDim Preserve Titles(UBound(Titles) + 1)
                    new_titles(UBound(new_titles)) = tname
                    Titles(UBound(Titles)) = tname
                End If
            End If
        End If
    Next
    
    '�e��i�f�[�^��include.eve��ǂݍ���
    If load_mode <> "�V�X�e��" Then
        '��i���̃C���N���[�h�t�@�C��
        For i = 1 To UBound(Titles)
            tfolder = SearchDataFolder(Titles(i))
            If FileExists(tfolder & "\include.eve") Then
                LoadEventData2 tfolder & "\include.eve"
            End If
        Next
        
        '�ėpData�C���N���[�h�t�@�C�������[�h
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
    
    '�����s�ɕ������ꂽ�R�}���h������
    For i = SysEventDataSize + 1 To UBound(EventData) - 1
        If Right$(EventData(i), 1) = "_" Then
            EventData(i + 1) = _
                Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
            EventData(i) = " "
        End If
    Next
    
    '���x���̓o�^
    num = CurrentLineNum
    If load_mode = "�V�X�e��" Then
        For CurrentLineNum = 1 To UBound(EventData)
            buf = EventData(CurrentLineNum)
            If Right$(buf, 1) = ":" Then
                AddSysLabel Left$(buf, Len(buf) - 1), CurrentLineNum
            End If
        Next
    ElseIf sys_event_data_size > 0 Then
        '�V�X�e�����ւ̃C�x���g�f�[�^�̒ǉ����������ꍇ
        For CurrentLineNum = 1 To sys_event_data_size
            buf = EventData(CurrentLineNum)
            Select Case Right$(buf, 1)
                Case ":"
                    AddSysLabel Left$(buf, Len(buf) - 1), CurrentLineNum
                Case "�F"
                    DisplayEventErrorMessage CurrentLineNum, "���x���̖������S�p�����ɂȂ��Ă��܂�"
                    error_found = True
            End Select
        Next
        For CurrentLineNum = sys_event_data_size + 1 To UBound(EventData)
            buf = EventData(CurrentLineNum)
            Select Case Right$(buf, 1)
                Case ":"
                    AddLabel Left$(buf, Len(buf) - 1), CurrentLineNum
                Case "�F"
                    DisplayEventErrorMessage CurrentLineNum, "���x���̖������S�p�����ɂȂ��Ă��܂�"
                    error_found = True
            End Select
        Next
    Else
        For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
            buf = EventData(CurrentLineNum)
            Select Case Right$(buf, 1)
                Case ":"
                    AddLabel Left$(buf, Len(buf) - 1), CurrentLineNum
                Case "�F"
                    DisplayEventErrorMessage CurrentLineNum, "���x���̖������S�p�����ɂȂ��Ă��܂�"
                    error_found = True
            End Select
        Next
    End If
    CurrentLineNum = num
    
    '�R�}���h�f�[�^�z���ݒ�
    If UBound(EventData) > UBound(EventCmd) Then
        num = UBound(EventCmd)
        ReDim Preserve EventCmd(UBound(EventData))
        For i = num + 1 To UBound(EventCmd)
            Set EventCmd(i) = New CmdData
            EventCmd(i).LineNum = i
        Next
    End If
    
    '�����`�F�b�N�̓V�i���I���ɂ̂ݎ��{
    If load_mode <> "�V�X�e��" Then
    
    '�\����͂Ə����`�F�b�N���̂P
    '����\��
    CmdStackIdx = 0
    CmdPosStackIdx = 0
    For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
        If EventCmd(CurrentLineNum) Is Nothing Then
            Set EventCmd(CurrentLineNum) = New CmdData
            EventCmd(CurrentLineNum).LineNum = CurrentLineNum
        End If
        With EventCmd(CurrentLineNum)
            '�R�}���h�̍\�����
            If Not .Parse(EventData(CurrentLineNum)) Then
                error_found = True
            End If
            
            '���X�g�����}�C�i�X�̂Ƃ��͊��ʂ̑Ή������Ă��Ȃ�
            If .ArgNum = -1 Then
                Select Case CmdStack(CmdStackIdx)
                    Case AskCmd, AutoTalkCmd, QuestionCmd, TalkCmd
                        '�����̃R�}���h�̓��͂̏ꍇ�͖�������
                    Case Else
                        DisplayEventErrorMessage CurrentLineNum, "���ʂ̑Ή������Ă��܂���"
                        error_found = True
                End Select
            End If
            
            '�R�}���h�ɉ����Đ���\�����`�F�b�N
            Select Case .Name
                Case IfCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage num, "Talk�ɑΉ�����End������܂���"
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
                        DisplayEventErrorMessage num, "Talk�ɑΉ�����End������܂���"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) <> IfCmd Then
                        DisplayEventErrorMessage CurrentLineNum, "ElseIf�ɑΉ�����If������܂���"
                        error_found = True
                        
                        CmdStackIdx = CmdStackIdx + 1
                        CmdPosStackIdx = CmdPosStackIdx + 1
                        CmdStack(CmdStackIdx) = IfCmd
                        CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    End If
                    
                Case ElseCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage num, "Talk�ɑΉ�����End������܂���"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        DisplayEventErrorMessage CurrentLineNum, "Else�ɑΉ�����If������܂���"
                        error_found = True
                        
                        CmdStackIdx = CmdStackIdx + 1
                        CmdPosStackIdx = CmdPosStackIdx + 1
                        CmdStack(CmdStackIdx) = IfCmd
                        CmdPosStack(CmdPosStackIdx) = CurrentLineNum
                    End If
                    
                Case EndIfCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage num, "Talk�ɑΉ�����End������܂���"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) = IfCmd Then
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                    Else
                        DisplayEventErrorMessage CurrentLineNum, "EndIf�ɑΉ�����If������܂���"
                        error_found = True
                    End If
                    
                Case DoCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage num, "Talk�ɑΉ�����End������܂���"
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
                        DisplayEventErrorMessage num, "Talk�ɑΉ�����End������܂���"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) = DoCmd Then
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                    Else
                        DisplayEventErrorMessage CurrentLineNum, "Loop�ɑΉ�����Do������܂���"
                        error_found = True
                    End If
                    
                Case ForCmd, ForEachCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage _
                            num, "Talk�ɑΉ�����End������܂���"
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
                                num, "Talk�ɑΉ�����End������܂���"
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
                                    "Next�ɑΉ�����R�}���h������܂���"
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
                                        "Next�ɑΉ�����R�}���h������܂���"
                                    error_found = True
                            End Select
                        End If
                    End If
                    
                Case SwitchCmd
                    If CmdStack(CmdStackIdx) = TalkCmd Then
                        num = CmdPosStack(CmdPosStackIdx)
                        DisplayEventErrorMessage _
                            num, "Talk�ɑΉ�����End������܂���"
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
                            num, "Talk�ɑΉ�����End������܂���"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) <> SwitchCmd Then
                        DisplayEventErrorMessage _
                            CurrentLineNum, "Case�ɑΉ�����Switch������܂���"
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
                            num, "Talk�ɑΉ�����End������܂���"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    If CmdStack(CmdStackIdx) = SwitchCmd Then
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                    Else
                        DisplayEventErrorMessage _
                            CurrentLineNum, "EndSw�ɑΉ�����Switch������܂���"
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
                            num, "Talk�ɑΉ�����End������܂���"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    i = .ArgNum
                    Do While i > 1
                        Select Case .GetArg(i)
                            Case "�ʏ�"
                            Case "�g��"
                            Case "�A���\��"
                            Case "�L�����Z����"
                            Case "�I��"
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
                            num, "Talk�ɑΉ�����End������܂���"
                        CmdStackIdx = CmdStackIdx - 1
                        CmdPosStackIdx = CmdPosStackIdx - 1
                        error_found = True
                    End If
                    
                    i = .ArgNum
                    Do While i > 1
                        Select Case .GetArg(.ArgNum)
                            Case "�ʏ�"
                            Case "�g��"
                            Case "�A���\��"
                            Case "�L�����Z����"
                            Case "�I��"
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
                                CurrentLineNum, "End�ɑΉ�����Talk������܂���"
                            error_found = True
                    End Select
                    
                Case SuspendCmd
                    Select Case CmdStack(CmdStackIdx)
                        Case TalkCmd, AutoTalkCmd
                            CmdStackIdx = CmdStackIdx - 1
                            CmdPosStackIdx = CmdPosStackIdx - 1
                        Case Else
                            DisplayEventErrorMessage _
                                CurrentLineNum, "Suspend�ɑΉ�����Talk������܂���"
                            error_found = True
                    End Select
                    
                Case ExitCmd, PlaySoundCmd, WaitCmd
                    Select Case CmdStack(CmdStackIdx)
                        Case TalkCmd, AutoTalkCmd, AskCmd, QuestionCmd
                            num = CmdPosStack(CmdPosStackIdx)
                            DisplayEventErrorMessage _
                                num, "Talk�ɑΉ�����End������܂���"
                            CmdStackIdx = CmdStackIdx - 1
                            CmdPosStackIdx = CmdPosStackIdx - 1
                            error_found = True
                    End Select
                    
                Case NopCmd
                    If EventData(CurrentLineNum) = " " Then
                        '"_"�ŏ������ꂽ�s�BTalk���̉��s�ɑΉ����邽�߂̃_�~�[�̋�
                        EventData(CurrentLineNum) = ""
                    Else
                        Select Case CmdStack(CmdStackIdx)
                            Case TalkCmd, AutoTalkCmd, AskCmd, QuestionCmd
                                If CurrentLineNum = UBound(EventData) Then
                                    num = CmdPosStack(CmdPosStackIdx)
                                    DisplayEventErrorMessage _
                                        num, "Talk�ɑΉ�����End������܂���"
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
                                            num, "Talk�ɑΉ�����End������܂���"
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
    
    '�t�@�C���̖����܂œǂ�ł��R�}���h�̏I��肪�Ȃ������H
    If CmdStackIdx > 0 Then
        num = CmdPosStack(CmdPosStackIdx)
        Select Case CmdStack(CmdStackIdx)
            Case AskCmd
                DisplayEventErrorMessage num, "Ask�ɑΉ�����End������܂���"
            Case AutoTalkCmd
                DisplayEventErrorMessage num, "AutoTalk�ɑΉ�����End������܂���"
            Case DoCmd
                DisplayEventErrorMessage num, "Do�ɑΉ�����Loop������܂���"
            Case ForCmd
                DisplayEventErrorMessage num, "For�ɑΉ�����Next������܂���"
            Case ForEachCmd
                DisplayEventErrorMessage num, "ForEach�ɑΉ�����Next������܂���"
            Case IfCmd
                DisplayEventErrorMessage num, "If�ɑΉ�����EndIf������܂���"
            Case QuestionCmd
                DisplayEventErrorMessage num, "Question�ɑΉ�����End������܂���"
            Case SwitchCmd
                DisplayEventErrorMessage num, "Switch�ɑΉ�����EndSw������܂���"
            Case TalkCmd
                DisplayEventErrorMessage num, "Talk�ɑΉ�����End������܂���"
        End Select
        error_found = True
    End If
    
    '�����G���[�����������ꍇ��SRC���I��
    If error_found Then
        TerminateSRC
    End If
    
    '�����`�F�b�N���̂Q
    '��ȃR�}���h�̈����̐����`�F�b�N
    For CurrentLineNum = SysEventDataSize + 1 To UBound(EventData)
        With EventCmd(CurrentLineNum)
            Select Case .Name
                Case CreateCmd
                    If .ArgNum < 8 Then
                        DisplayEventErrorMessage _
                            CurrentLineNum, "Create�R�}���h�̃p�����[�^�����Ⴂ�܂�"
                        error_found = True
                    End If
                Case PilotCmd
                    If .ArgNum < 3 Then
                        DisplayEventErrorMessage _
                            CurrentLineNum, "Pilot�R�}���h�̃p�����[�^�����Ⴂ�܂�"
                        error_found = True
                    End If
                Case UnitCmd
                    If .ArgNum <> 3 Then
                        DisplayEventErrorMessage _
                            CurrentLineNum, "Unit�R�}���h�̃p�����[�^�����Ⴂ�܂�"
                        error_found = True
                    End If
            End Select
        End With
    Next
    
    '�����G���[�����������ꍇ��SRC���I��
    If error_found Then
        TerminateSRC
    End If
    
    '�V�i���I���̃C�x���g�f�[�^�̏ꍇ�͂����܂ŃX�L�b�v
    Else
    
    '�V�X�e�����̃C�x���g�f�[�^�̏ꍇ�̏���
    
    'CmdData�N���X�̃C���X�^���X�̐����̂ݍs���Ă���
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
    
    '�C�x���g�f�[�^�̓ǂݍ��݂��I�������̂ŃV�X�e�����C�x���g�f�[�^�̃T�C�Y������B
    '�V�X�e�����C�x���g�f�[�^�͓ǂݍ��݂���x�������΂悢�B
    If sys_event_data_size > 0 Then
        SysEventDataSize = sys_event_data_size
        SysEventFileNum = sys_event_file_num
    End If
    
    '�N�C�b�N���[�h�⃊�X�^�[�g�̏ꍇ�̓V�i���I�f�[�^�̍ă��[�h�̂�
    Select Case load_mode
        Case "���X�g�A"
            ADList.AddDefaultAnimation
            Exit Sub
        Case "�V�X�e��", "�N�C�b�N���[�h", "���X�^�[�g"
            Exit Sub
    End Select
    
    '�ǉ����ꂽ�V�X�e�����C�x���g�f�[�^���`�F�b�N����ꍇ�͂����ŏI��
    If fname = "" Then
        Exit Sub
    End If
    
    '���[�h����f�[�^�����J�E���g
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
        '�f�t�H���g�̐퓬�A�j���f�[�^��ݒ�
        ADList.AddDefaultAnimation
        Exit Sub
    End If
    
    '���[�h��ʂ�\��
    OpenNowLoadingForm
    
    '���[�h�T�C�Y��ݒ�
    SetLoadImageSize num
    
    '�g�p���Ă���^�C�g���̃f�[�^�����[�h
    For i = 1 To UBound(new_titles)
        IncludeData new_titles(i)
    Next
    
    '���[�J���f�[�^�̓ǂ݂���
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
    
    '�f�t�H���g�̐퓬�A�j���f�[�^��ݒ�
    ADList.AddDefaultAnimation
    
    '�}�b�v�f�[�^�����[�h
    If FileExists(Left$(fname, Len(fname) - 4) & ".map") Then
        LoadMapData Left$(fname, Len(fname) - 4) & ".map"
        SetupBackground
        RedrawScreen
        DisplayLoadingProgress
    End If
    
    '���[�h��ʂ����
    CloseNowLoadingForm
End Sub

'�C�x���g�t�@�C���̓ǂݍ���
Public Sub LoadEventData2(fname As String, Optional ByVal lnum As Long)
Dim FileNumber As Integer, CurrentLineNum2 As Integer
Dim i As Integer
Dim buf As String, fname2 As String
Dim fid As Integer
Dim in_single_quote As Boolean, in_double_quote As Boolean
    
    If fname = "" Then
        Exit Sub
    End If
    
    '�C�x���g�t�@�C�������L�^���Ă��� (�G���[�\���p)
    ReDim Preserve EventFileNames(UBound(EventFileNames) + 1)
    EventFileNames(UBound(EventFileNames)) = fname
    fid = UBound(EventFileNames)
    
    On Error GoTo ErrorHandler
    
    '�t�@�C�����J��
    FileNumber = FreeFile
    Open fname For Input Access Read As #FileNumber
    
    '�s�ԍ��̐ݒ�
    If lnum > 0 Then
        CurrentLineNum = lnum
    End If
    CurrentLineNum2 = 0
    
    '�e�s�̓ǂݍ���
    Do Until EOF(FileNumber)
        CurrentLineNum = CurrentLineNum + 1
        CurrentLineNum2 = CurrentLineNum2 + 1
        
        '�f�[�^�̈�m��
        ReDim Preserve EventData(CurrentLineNum)
        ReDim Preserve EventFileID(CurrentLineNum)
        ReDim Preserve EventLineNum(CurrentLineNum)
        
        '�s�̓ǂݍ���
        Line Input #FileNumber, buf
        TrimString buf
        
        '�R�����g���폜
        If Left$(buf, 1) = "#" Then
            buf = " "
        ElseIf InStr(buf, "//") > 0 Then
            in_single_quote = False
            in_double_quote = False
            For i = 1 To Len(buf)
                Select Case Mid$(buf, i, 1)
                    Case "`"
                        '�V���O���N�I�[�g
                        If Not in_double_quote Then
                            in_single_quote = Not in_single_quote
                        End If
                    Case """"
                        '�_�u���N�I�[�g
                        If Not in_single_quote Then
                            in_double_quote = Not in_double_quote
                        End If
                    Case "/"
                        '�R�����g�H
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
        
        '�s��ۑ�
        EventData(CurrentLineNum) = buf
        EventFileID(CurrentLineNum) = fid
        EventLineNum(CurrentLineNum) = CurrentLineNum2
        
        '���̃C�x���g�t�@�C���̓ǂݍ���
        If Left$(buf, 1) = "<" Then
            If InStr(buf, ">") = Len(buf) And buf <> "<>" Then
                CurrentLineNum = CurrentLineNum - 1
                fname2 = Mid$(buf, 2, Len(buf) - 2)
                If fname2 <> "Lib\�X�y�V�����p���[.eve" _
                    And fname2 <> "Lib\�ėp�퓬�A�j��\include.eve" _
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
    
    '�t�@�C�������
    Close #FileNumber
    
    Exit Sub
    
ErrorHandler:
    If Len(buf) = 0 Then
        ErrorMessage fname & "���J���܂���"
    Else
        ErrorMessage fname & "�̃��[�h���ɃG���[���������܂���" & vbCr _
            & Format$(CurrentLineNum2) & "�s�ڂ̃C�x���g�f�[�^���s���ł�"
    End If
    TerminateSRC
End Sub


'�C�x���g�̎��s
Public Sub HandleEvent(ParamArray Args() As Variant)
Dim event_que_idx As Integer
Dim ret As Long, i As Integer
Dim flag As Boolean
Dim prev_is_gui_locked As Boolean
Dim prev_call_depth As Integer
Dim uparty As String
Dim u As Unit
Dim main_event_done As Boolean
    
    '��ʓ��͂����b�N
    prev_is_gui_locked = IsGUILocked
    If Not IsGUILocked Then
        LockGUI
    End If
    
    '���ݑI������Ă��郆�j�b�g���^�[�Q�b�g���C�x���g�p�ɐݒ�
    '(SearchLabel()���s���̎��v�Z�p�ɂ��炩���ߐݒ肵�Ă���)
    Set SelectedUnitForEvent = SelectedUnit
    '�����Ɏw�肳�ꂽ���j�b�g��D��
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
    
    '�C�x���g�L���[���쐬
    ReDim Preserve EventQue(UBound(EventQue) + 1)
    event_que_idx = UBound(EventQue)
    Select Case Args(0)
        Case "�v�����[�O"
            EventQue(UBound(EventQue)) = "�v�����[�O"
            Stage = "�v�����[�O"
        Case "�G�s���[�O"
            EventQue(UBound(EventQue)) = "�G�s���[�O"
            Stage = "�G�s���[�O"
        Case "�j��"
            EventQue(UBound(EventQue)) = "�j�� " & Args(1)
            With PList.Item(Args(1))
                uparty = .Party
                If Not .Unit Is Nothing Then
                    With .Unit
                        '�i�[����Ă������j�b�g���j�󂵂Ă���
' MOD START MARGE
'                        For i = 1 To .CountUnitOnBoard
'                            Set u = .UnitOnBoard(1)
'                            .UnloadUnit u.ID
'                            u.Status = "�j��"
'                            u.HP = 0
'                            ReDim Preserve EventQue(UBound(EventQue) + 1)
'                            EventQue(UBound(EventQue)) = _
'                                "�j�� " & u.MainPilot.ID
'                        Next
                        Do While .CountUnitOnBoard > 0
                            Set u = .UnitOnBoard(1)
                            .UnloadUnit u.ID
                            u.Status = "�j��"
                            u.HP = 0
                            ReDim Preserve EventQue(UBound(EventQue) + 1)
                            EventQue(UBound(EventQue)) = _
                                "�}�b�v�U���j�� " & u.MainPilot.ID
                        Loop
' MOD END MARGE
                        uparty = .Party0
                    End With
                End If
            End With
            
            '�S�ł̔���
            flag = False
            For Each u In UList
                With u
                    If .Party0 = uparty _
                        And .Status = "�o��" _
                        And Not .IsConditionSatisfied("�߈�") _
                    Then
                        flag = True
                        Exit For
                    End If
                End With
            Next
            If Not flag Then
                ReDim Preserve EventQue(UBound(EventQue) + 1)
                EventQue(UBound(EventQue)) = "�S�� " & uparty
            End If
        Case "�}�b�v�U���j��"
            EventQue(UBound(EventQue)) = "�}�b�v�U���j�� " & Args(1)
            With PList.Item(Args(1))
                uparty = .Party
                If Not .Unit Is Nothing Then
                    With .Unit
                        '�i�[����Ă������j�b�g���j�󂵂Ă���
                        For i = 1 To .CountUnitOnBoard
                            Set u = .UnitOnBoard(i)
                            .UnloadUnit u.ID
                            u.Status = "�j��"
                            u.HP = 0
                            ReDim Preserve EventQue(UBound(EventQue) + 1)
                            EventQue(UBound(EventQue)) = _
                                "�}�b�v�U���j�� " & u.MainPilot.ID
                        Next
                        uparty = .Party0
                    End With
                End If
            End With
        Case "�^�[��"
            EventQue(UBound(EventQue)) = "�^�[�� �S " & Args(2)
            ReDim Preserve EventQue(UBound(EventQue) + 1)
            EventQue(UBound(EventQue)) = "�^�[�� " & Format$(Args(1)) & " " & Args(2)
        Case "������"
            EventQue(UBound(EventQue)) = "������ " & Args(1) & " " & Format$(Args(2))
        Case "�U��"
            EventQue(UBound(EventQue)) = "�U�� " & Args(1) & " " & Args(2)
        Case "�U����"
            EventQue(UBound(EventQue)) = "�U���� " & Args(1) & " " & Args(2)
        Case "��b"
            EventQue(UBound(EventQue)) = "��b " & Args(1) & " " & Args(2)
        Case "�ڐG"
            EventQue(UBound(EventQue)) = "�ڐG " & Args(1) & " " & Args(2)
        Case "�i��"
            EventQue(UBound(EventQue)) = "�i�� " & Args(1) & " " _
                & Format$(Args(2)) & " " & Format$(Args(3))
            ReDim Preserve EventQue(UBound(EventQue) + 1)
            EventQue(UBound(EventQue)) = "�i�� " & Args(1) & " " _
                & TerrainName(CInt(Args(2)), CInt(Args(3)))
            If Args(2) = 1 Then
                ReDim Preserve EventQue(UBound(EventQue) + 1)
                EventQue(UBound(EventQue)) = "�E�o " & Args(1) & " W"
            ElseIf Args(2) = MapWidth Then
                ReDim Preserve EventQue(UBound(EventQue) + 1)
                EventQue(UBound(EventQue)) = "�E�o " & Args(1) & " E"
            End If
            If Args(3) = 1 Then
                ReDim Preserve EventQue(UBound(EventQue) + 1)
                EventQue(UBound(EventQue)) = "�E�o " & Args(1) & " N"
            ElseIf Args(3) = MapHeight Then
                ReDim Preserve EventQue(UBound(EventQue) + 1)
                EventQue(UBound(EventQue)) = "�E�o " & Args(1) & " S"
            End If
        Case "���["
            EventQue(UBound(EventQue)) = "���[ " & Args(1)
        Case "�g�p"
            EventQue(UBound(EventQue)) = "�g�p " & Args(1) & " " & Args(2)
        Case "�g�p��"
            EventQue(UBound(EventQue)) = "�g�p�� " & Args(1) & " " & Args(2)
        Case "�s���I��"
            EventQue(UBound(EventQue)) = "�s���I�� " & Args(1)
        Case "���j�b�g�R�}���h"
            EventQue(UBound(EventQue)) = "���j�b�g�R�}���h " & Args(1) & " " & Args(2)
            If Not IsEventDefined(EventQue(UBound(EventQue))) Then
                EventQue(UBound(EventQue)) = "���j�b�g�R�}���h " & Args(1) & " " _
                    & PList.Item(Args(2)).Unit.Name
            End If
        Case Else
            EventQue(UBound(EventQue)) = Args(0)
            For i = 1 To UBound(Args)
                EventQue(UBound(EventQue)) = EventQue(UBound(EventQue)) & " " & Args(i)
            Next
    End Select
    
    If CallDepth > MaxCallDepth Then
        ErrorMessage "�T�u���[�`���̌Ăяo���K�w��" & Format$(MaxCallDepth) & _
            "�𒴂��Ă��邽�߁A�C�x���g�̏������o���܂���"
        CallDepth = MaxCallDepth
        Exit Sub
    End If
    
    '���݂̏�Ԃ�ۑ�
    ArgIndexStack(CallDepth) = ArgIndex
    VarIndexStack(CallDepth) = VarIndex
    ForIndexStack(CallDepth) = ForIndex
    SaveBasePoint
    
    '�Ăяo���K�w�����C���N�������g
    prev_call_depth = CallDepth
    CallDepth = CallDepth + 1
    
    '�e�C�x���g�𔭐�������
    i = event_que_idx
    IsCanceled = False
    Do
        'Debug.Print "HandleEvent (" & EventQue(i) & ")"
        
        '�O�̃C�x���g�ő��̃��j�b�g���o�����Ă���\��������̂�
        '�{���ɑS�ł����̂�����
        If LIndex(EventQue(i), 1) = "�S��" Then
            uparty = LIndex(EventQue(i), 2)
            For Each u In UList
                With u
                    If .Party0 = uparty _
                        And .Status = "�o��" _
                        And Not .IsConditionSatisfied("�߈�") _
                    Then
                        GoTo NextLoop
                    End If
                End With
            Next
        End If
        
        CurrentLabel = 0
        main_event_done = False
        Do While True
            '���ݑI������Ă��郆�j�b�g���^�[�Q�b�g���C�x���g�p�ɐݒ�
            'SearchLabel()�œ���ւ�����\��������̂ŁA����ݒ肵�����K�v����
            Set SelectedUnitForEvent = SelectedUnit
            '�����Ɏw�肳�ꂽ���j�b�g��D��
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
            
            '���s����C�x���g���x����T��
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
                    '�펞�C�x���g�ł͂Ȃ��C�x���g�͂P�x�������s���Ȃ�
                    If main_event_done Then
                        ret = 0
                    Else
                        main_event_done = True
                    End If
                End If
            Loop While ret = 0
            
            '�퓬��̃C�x���g���s�O�ɂ͂������̌�n�����K�v
            If Left$(EventData(ret), 1) <> "*" Then
                If Args(0) = "�j��" _
                    Or Args(0) = "������" _
                    Or Args(0) = "�U����" _
                    Or Args(0) = "�S��" _
                Then
                    '��ʂ��N���A
                    If MainForm.Visible = True Then
                        ClearUnitStatus
                        RedrawScreen
                    End If
                    
                    '���b�Z�[�W�E�B���h�E�����
                    If frmMessage.Visible = True Then
                        CloseMessageForm
                    End If
                End If
            End If
            
            '���x���̍s�͎��s���Ă����ʂȂ̂�
            ret = ret + 1
            
            DoEvents
            
            '�C�x���g�̊e�R�}���h�����s
            Do
                CurrentLineNum = ret
                If CurrentLineNum > UBound(EventCmd) Then
                    GoTo ExitLoop
                End If
                ret = EventCmd(CurrentLineNum).Exec
            Loop While ret > 0
            
            '�X�e�[�W���I�� or �L�����Z���H
            If IsScenarioFinished Or IsCanceled Then
                GoTo ExitLoop
            End If
        Loop
NextLoop:
        i = i + 1
    Loop While i <= UBound(EventQue)
ExitLoop:
    
    If CallDepth >= 0 Then
        '�Ăяo���K�w�������ɖ߂�
        '�i�T�u���[�`������Exit���Ă΂�邱�Ƃ�����̂ŒP����-1�o���Ȃ��j
        CallDepth = prev_call_depth
        
        '�C�x���g���s�O�̏�Ԃɕ��A
        ArgIndex = ArgIndexStack(CallDepth)
        VarIndex = VarIndexStack(CallDepth)
        ForIndex = ForIndexStack(CallDepth)
    Else
        ArgIndex = 0
        VarIndex = 0
        ForIndex = 0
    End If
    
    '�C�x���g�L���[�����ɖ߂�
    ReDim Preserve EventQue(MinLng(event_que_idx - 1, UBound(EventQue)))
    
    '�t�H���g�ݒ���f�t�H���g�ɖ߂�
    With MainForm.picMain(0)
        .ForeColor = rgb(255, 255, 255)
        With .Font
            .Size = 16
            .Name = "�l�r �o����"
            .Bold = True
            .Italic = False
        End With
        PermanentStringMode = False
        KeepStringMode = False
    End With
    
    '�I�u�W�F�N�g�F���f�t�H���g�ɖ߂�
    ObjColor = vbWhite
    ObjFillColor = vbWhite
    ObjFillStyle = vbFSTransparent
    ObjDrawWidth = 1
    ObjDrawOption = ""
    
    '�`��̊���W�ʒu�����ɖ߂�
    RestoreBasePoint
    
    '��ʓ��͂̃��b�N������
    If Not prev_is_gui_locked Then
        UnlockGUI
    End If
End Sub

'�C�x���g��o�^���Ă����A��Ŏ��s
Public Sub RegisterEvent(ParamArray Args() As Variant)
Dim i As Integer

    ReDim Preserve EventQue(UBound(EventQue) + 1)
    EventQue(UBound(EventQue)) = Args(0)
    For i = 1 To UBound(Args)
        EventQue(UBound(EventQue)) = EventQue(UBound(EventQue)) & " " & Args(i)
    Next
End Sub


'���x���̌���
Public Function SearchLabel(lname As String, Optional ByVal start As Long) As Long
Dim ltype As LabelType, llen As Integer, litem() As String, lnum(4) As String
Dim is_unit(4) As Boolean, is_num(4) As Boolean, is_condition(4) As Boolean
Dim str1 As String, str2 As String, lname2 As String
Dim i As Long, lab As LabelData, tmp_u As Unit
Dim revrersible As Boolean, reversed As Boolean
    
    '���x���̊e�v�f�����炩���߉��
    llen = ListSplit(lname, litem)
    
    '���x���̎�ނ𔻒�
    Select Case litem(1)
        Case "�v�����[�O"
            ltype = PrologueEventLabel
        Case "�X�^�[�g"
            ltype = StartEventLabel
        Case "�G�s���[�O"
            ltype = EpilogueEventLabel
        Case "�^�[��"
            ltype = TurnEventLabel
            If IsNumeric(litem(2)) Then
                is_num(2) = True
            End If
            lnum(2) = StrToLng(litem(2))
        Case "������"
            ltype = DamageEventLabel
            is_unit(2) = True
            is_num(3) = True
            lnum(3) = StrToLng(litem(3))
        Case "�j��", "�}�b�v�U���j��"
            ltype = DestructionEventLabel
            is_unit(2) = True
        Case "�S��"
            ltype = TotalDestructionEventLabel
        Case "�U��"
            ltype = AttackEventLabel
            revrersible = True
            is_unit(2) = True
            is_unit(3) = True
        Case "�U����"
            ltype = AfterAttackEventLabel
            revrersible = True
            is_unit(2) = True
            is_unit(3) = True
        Case "��b"
            ltype = TalkEventLabel
            is_unit(2) = True
            is_unit(3) = True
        Case "�ڐG"
            ltype = ContactEventLabel
            revrersible = True
            is_unit(2) = True
            is_unit(3) = True
        Case "�i��"
            ltype = EnterEventLabel
            is_unit(2) = True
            If llen = 4 Then
                is_num(3) = True
                is_num(4) = True
                lnum(3) = StrToLng(litem(3))
                lnum(4) = StrToLng(litem(4))
            End If
        Case "�E�o"
            ltype = EscapeEventLabel
            is_unit(2) = True
        Case "���["
            ltype = LandEventLabel
            is_unit(2) = True
        Case "�g�p"
            ltype = UseEventLabel
            is_unit(2) = True
        Case "�g�p��"
            ltype = AfterUseEventLabel
            is_unit(2) = True
        Case "�ό`"
            ltype = TransformEventLabel
            is_unit(2) = True
        Case "����"
            ltype = CombineEventLabel
            is_unit(2) = True
        Case "����"
            ltype = SplitEventLabel
            is_unit(2) = True
        Case "�s���I��"
            ltype = FinishEventLabel
            is_unit(2) = True
        Case "���x���A�b�v"
            ltype = LevelUpEventLabel
            is_unit(2) = True
        Case "��������"
            ltype = RequirementEventLabel
        Case "�ĊJ"
            ltype = ResumeEventLabel
        Case "�}�b�v�R�}���h"
            ltype = MapCommandEventLabel
            is_condition(3) = True
        Case "���j�b�g�R�}���h"
            ltype = UnitCommandEventLabel
            is_condition(4) = True
        Case "�������"
            ltype = EffectEventLabel
        Case Else
            ltype = NormalLabel
    End Select
    
    '�e���x���ɂ��Ĉ�v���Ă��邩�`�F�b�N
    For Each lab In colEventLabelList
        With lab
            '���x���̎�ނ���v���Ă���H
            If ltype <> .Name Then
                GoTo NextLabel
            End If
            
            'ClearEvent����Ă��Ȃ��H
            If Not .Enable Then
                GoTo NextLabel
            End If
            
            '�����J�n�s�����H
            If .LineNum < start Then
                GoTo NextLabel
            End If
            
            '�p�����[�^������v���Ă���H
            If llen <> .CountPara Then
                If ltype <> MapCommandEventLabel _
                    And ltype <> UnitCommandEventLabel _
                Then
                    GoTo NextLabel
                End If
            End If
            
            '�e�p�����[�^����v���Ă���H
            reversed = False
CheckPara:
            For i = 2 To llen
                '�R�}���h�֘A���x���̍Ō�̃p�����[�^�͏������Ȃ̂Ń`�F�b�N���Ȃ�
                If is_condition(i) Then
                    Exit For
                End If
                
                '��r����p�����[�^
                str1 = litem(i)
                If reversed Then
                    str2 = .Para(5 - i)
                Else
                    str2 = .Para(i)
                End If
                
                '�u�S�v�͑S�ĂɈ�v
                If str2 = "�S" Then
                    '�������A�u�^�[�� �S�v���Q����s�����͖̂h��
                    If ltype <> TurnEventLabel Or i <> 2 Then
                        GoTo NextPara
                    End If
                End If
                
                '���l�Ƃ��Ĕ�r�H
                If is_num(i) Then
                    If IsNumeric(str2) Then
                        If lnum(i) = CLng(str2) Then
                            GoTo NextPara
                        ElseIf ltype = DamageEventLabel Then
                            '���������x���̏���
                            If lnum(i) > CLng(str2) Then
                                Exit For
                            End If
                        End If
                    End If
                    GoTo NextLabel
                End If
                
                '���j�b�g�w��Ƃ��Ĕ�r�H
                If is_unit(i) Then
                    If str2 = "����" Or str2 = "�m�o�b" _
                        Or str2 = "�G" Or str2 = "����" _
                    Then
                        '�w�c���Ŕ�r
                        If str1 <> "����" And str1 <> "�m�o�b" _
                            And str1 <> "�G" And str1 <> "����" _
                        Then
                            If PList.IsDefined(str1) Then
                                str1 = PList.Item(str1).Party
                            End If
                        End If
                    ElseIf PList.IsDefined(str2) Then
                        '�p�C���b�g�Ŕ�r
                        With PList.Item(str2)
                            If str2 = .Data.Name Or str2 = .Data.Nickname Then
                                '�O���[�v�h�c���t�����Ă��Ȃ��ꍇ��
                                '�p�C���b�g���Ŕ�r
                                str2 = .Name
                                If PList.IsDefined(str1) Then
                                    str1 = PList.Item(str1).Name
                                End If
                            Else
                                '�O���[�v�h�c���t�����Ă���ꍇ��
                                '�O���[�v�h�c�Ŕ�r
                                If PList.IsDefined(str1) Then
                                    str1 = PList.Item(str1).ID
                                End If
                                If InStr(str1, ":") > 0 Then
                                    str1 = Left$(str1, InStr(str1, ":") - 1)
                                End If
                            End If
                        End With
                    ElseIf PDList.IsDefined(str2) Then
                        '�p�C���b�g���Ŕ�r
                        str2 = PDList.Item(str2).Name
                        If PList.IsDefined(str1) Then
                            str1 = PList.Item(str1).Name
                        End If
                    ElseIf UDList.IsDefined(str2) Then
                        '���j�b�g���Ŕ�r
                        If PList.IsDefined(str1) Then
                            With PList.Item(str1)
                                If Not .Unit Is Nothing Then
                                    str1 = .Unit.Name
                                End If
                            End With
                        End If
                    Else
                        '�O���[�v�h�c���t�����Ă��邨��A�Ȃ��������h�c��
                        '�Q�Ԗڈȍ~�̃��j�b�g�̏ꍇ�̓O���[�v�h�c�Ŕ�r
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
                
                '��v�������H
                If str1 <> str2 Then
                    If revrersible And Not reversed Then
                        '�ΏۂƑ�������ւ����C�x���g���x�������݂��邩����
                        lname2 = litem(1) & _
                            " " & ListIndex(.Data, 3) & _
                            " " & ListIndex(.Data, 2)
                        If .AsterNum > 0 Then
                            lname2 = "*" & lname2
                        End If
                        If FindLabel(lname2) = 0 Then
                            '�ΏۂƑ�������ւ��Ĕ��肵����
                            reversed = True
                            GoTo CheckPara
                        End If
                    End If
                    GoTo NextLabel
                End If
NextPara:
            Next
            
            '�����܂ł��ǂ�t���΃��x���͈�v���Ă���
            SearchLabel = .LineNum
            
            '�ΏۂƑ�������ւ��Ĉ�v�����ꍇ�̓O���[�o���ϐ�������ւ�
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

'�w�肵���C�x���g�ւ̃C�x���g���x������`����Ă��邩
'�펞�C�x���g�ł͂Ȃ��ʏ�C�x���g�݂̂�T���ꍇ��
' normal_event_only = True ���w�肷��
Public Function IsEventDefined(lname As String, _
    Optional ByVal normal_event_only As Boolean) As Boolean
Dim i As Long, ret As Long

    '�C�x���g���x����T��
    i = 0
    Do While 1
        ret = SearchLabel(lname, i + 1)
        If ret = 0 Then
            Exit Function
        End If
        
        If normal_event_only Then
            '�펞�C�x���g�ł͂Ȃ��ʏ�C�x���g�݂̂�T���ꍇ
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

'���x������`����Ă��邩
Public Function IsLabelDefined(Index As Variant) As Boolean
Dim lab As LabelData

    On Error GoTo ErrorHandler
    Set lab = colEventLabelList.Item(Index)
    IsLabelDefined = True
    Exit Function
    
ErrorHandler:
    IsLabelDefined = False
End Function

'���x����ǉ�
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
            '�ʏ탉�x����ǉ�
            If FindNormalLabel0(lname) = 0 Then
                colNormalLabelList.Add new_label, lname
            End If
        Else
            '�C�x���g���x����ǉ�
            
            '�p�����[�^�Ԃ̕�����̈Ⴂ�ɂ��s��v���Ȃ������߁A
            '������𔼊p�X�y�[�X�ꕶ���ɒ����Ă���
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
    '�ʏ탉�x�����d����`����Ă���ꍇ�͖���
End Sub

'�V�X�e�����̃��x����ǉ�
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
            '�ʏ탉�x����ǉ�
            If FindSysNormalLabel(lname) = 0 Then
                colSysNormalLabelList.Add new_label, lname
            Else
                colSysNormalLabelList.Item(lname).LineNum = lnum
            End If
        Else
            '�C�x���g���x����ǉ�
            
            '�p�����[�^�Ԃ̕�����̈Ⴂ�ɂ��s��v���Ȃ������߁A
            '������𔼊p�X�y�[�X�ꕶ���ɒ����Ă���
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
    '�ʏ탉�x�����d����`����Ă���ꍇ�͖���
End Sub

'���x��������
Public Sub ClearLabel(ByVal lnum As Long)
Dim lab As LabelData, i As Integer
    
    '�s�ԍ�lnum�ɂ��郉�x����T��
    For Each lab In colEventLabelList
        With lab
            If .LineNum = lnum Then
                .Enable = False
                Exit Sub
            End If
        End With
    Next
    
    'lnum�s�ڂɂȂ���΂��̎����T��
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

'���x���𕜊�
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

'���x����T��
Public Function FindLabel(lname As String) As Long
Dim lname2 As String
Dim i As Integer

    '�ʏ탉�x�����猟��
    FindLabel = FindNormalLabel(lname)
    If FindLabel > 0 Then
        Exit Function
    End If
    
    '�C�x���g���x�����猟��
    FindLabel = FindEventLabel(lname)
    If FindLabel > 0 Then
        Exit Function
    End If
    
    '�p�����[�^�Ԃ̕�����̈Ⴂ�ň�v���Ȃ������\��������̂�
    '������𔼊p�X�y�[�X�ꕶ���݂̂ɂ��Č������Ă݂�
    lname2 = ListIndex(lname, 1)
    For i = 2 To ListLength(lname)
        lname2 = lname2 & " " & ListIndex(lname, i)
    Next
    
    '�C�x���g���x�����猟��
    FindLabel = FindEventLabel(lname2)
End Function

'�C�x���g���x����T��
Public Function FindEventLabel(lname As String) As Long
Dim lab As LabelData

    On Error GoTo NotFound
    Set lab = colEventLabelList.Item(lname)
    FindEventLabel = lab.LineNum
    Exit Function

NotFound:
    FindEventLabel = 0
End Function

'�ʏ탉�x����T��
Public Function FindNormalLabel(lname As String) As Long
    FindNormalLabel = FindNormalLabel0(lname)
    If FindNormalLabel = 0 Then
        FindNormalLabel = FindSysNormalLabel(lname)
    End If
End Function

'�V�i���I���̒ʏ탉�x����T��
Private Function FindNormalLabel0(lname As String) As Long
Dim lab As LabelData

    On Error GoTo NotFound
    Set lab = colNormalLabelList.Item(lname)
    FindNormalLabel0 = lab.LineNum
    Exit Function

NotFound:
    FindNormalLabel0 = 0
End Function

'�V�X�e�����̒ʏ탉�x����T��
Private Function FindSysNormalLabel(lname As String) As Long
Dim lab As LabelData

    On Error GoTo NotFound
    Set lab = colSysNormalLabelList.Item(lname)
    FindSysNormalLabel = lab.LineNum
    Exit Function

NotFound:
    FindSysNormalLabel = 0
End Function


'�C�x���g�f�[�^�̏���
'�������O���[�o���ϐ��̃f�[�^�͎c���Ă���
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

'�O���[�o���ϐ����܂߂��C�x���g�f�[�^�̑S����
Public Sub ClearAllEventData()
Dim i As Integer

    ClearEventData
    
    With GlobalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    
    DefineGlobalVariable "���X�e�[�W"
    DefineGlobalVariable "�Z�[�u�f�[�^�t�@�C����"
End Sub


'�ꎞ���f�p�f�[�^���t�@�C���ɃZ�[�u����
Public Sub DumpEventData()
Dim lab As LabelData, i As Integer

    '�O���[�o���ϐ�
    SaveGlobalVariables
    '���[�J���ϐ�
    SaveLocalVariables
    
    '�C�x���g�p���x��
    Write #SaveDataFileNumber, colEventLabelList.Count
    For Each lab In colEventLabelList
        Write #SaveDataFileNumber, lab.Enable
    Next
    
    'Require�R�}���h�Œǉ����ꂽ�C�x���g�t�@�C��
    Write #SaveDataFileNumber, UBound(AdditionalEventFileNames)
    For i = 1 To UBound(AdditionalEventFileNames)
        Write #SaveDataFileNumber, AdditionalEventFileNames(i)
    Next
End Sub

'�ꎞ���f�p�f�[�^���t�@�C�����烍�[�h����
Public Sub RestoreEventData()
Dim lab As LabelData
Dim num As Integer, lenable As Boolean
Dim fname As String, file_head As Long
Dim i As Long, j As Integer, buf As String

    '�O���[�o���ϐ�
    LoadGlobalVariables
    '���[�J���ϐ�
    LoadLocalVariables
    
    '�C�x���g�p���x��
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
    
    'Require�R�}���h�Œǉ����ꂽ�C�x���g�t�@�C��
    If SaveDataVersion > 20003 Then
        file_head = UBound(EventData) + 1
        
' MOD START MARGE
'        '�C�x���g�t�@�C�������[�h
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
'            '���ɓǂݍ��܂�Ă���ꍇ�̓X�L�b�v
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
'        '�G���[�\���p�ɃT�C�Y��傫������Ă���
'        ReDim Preserve EventData(UBound(EventData) + 1)
'        ReDim Preserve EventLineNum(UBound(EventData))
'        EventData(UBound(EventData)) = ""
'        EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
'
'        '�����s�ɕ������ꂽ�R�}���h������
'        For i = file_head To UBound(EventData) - 1
'            If Right$(EventData(i), 1) = "_" Then
'                EventData(i + 1) = _
'                    Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
'                EventData(i) = " "
'            End If
'        Next
'
'        '���x����o�^
'        For i = file_head To UBound(EventData)
'            buf = EventData(i)
'            If Right$(buf, 1) = ":" Then
'                AddLabel Left$(buf, Len(buf) - 1), i
'            End If
'        Next
'
'        '�R�}���h�f�[�^�z���ݒ�
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
        '�ǉ�����C�x���g�t�@�C����
        Input #SaveDataFileNumber, num
        
        If num > 0 Then
            '�C�x���g�t�@�C�������[�h
            ReDim AdditionalEventFileNames(num)
            For i = 1 To num
                Input #SaveDataFileNumber, fname
                AdditionalEventFileNames(i) = fname
                If InStr(fname, ":") = 0 Then
                    fname = ScenarioPath & fname
                End If
                
                '���ɓǂݍ��܂�Ă���ꍇ�̓X�L�b�v
                For j = 1 To UBound(EventFileNames)
                   If fname = EventFileNames(j) Then
                       GoTo NextEventFile
                   End If
                Next
                
                LoadEventData2 fname, UBound(EventData)
NextEventFile:
            Next
            
            '�G���[�\���p�ɃT�C�Y��傫������Ă���
            ReDim Preserve EventData(UBound(EventData) + 1)
            ReDim Preserve EventLineNum(UBound(EventData))
            EventData(UBound(EventData)) = ""
            EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
            
            '�����s�ɕ������ꂽ�R�}���h������
            For i = file_head To UBound(EventData) - 1
                If Right$(EventData(i), 1) = "_" Then
                    EventData(i + 1) = _
                        Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
                    EventData(i) = " "
                End If
            Next
            
            '���x����o�^
            For i = file_head To UBound(EventData)
                buf = EventData(i)
                If Right$(buf, 1) = ":" Then
                    AddLabel Left$(buf, Len(buf) - 1), i
                End If
            Next
            
            '�R�}���h�f�[�^�z���ݒ�
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
    
    '�C�x���g�p���x����ݒ�
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

'�ꎞ���f�p�f�[�^�̃C�x���g�f�[�^������ǂݔ�΂�
Public Sub SkipEventData()
Dim i As Integer, num As Integer
Dim dummy As String

    '�O���[�o���ϐ�
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    '���[�J���ϐ�
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    
    '���x�����
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    
    'Require�R�}���h�œǂݍ��񂾃C�x���g�f�[�^
    If SaveDataVersion > 20003 Then
        Input #SaveDataFileNumber, num
        For i = 1 To num
            Line Input #SaveDataFileNumber, dummy
        Next
    End If
End Sub

'�O���[�o���ϐ����t�@�C���ɃZ�[�u
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

'�O���[�o���ϐ����t�@�C�����烍�[�h
Public Sub LoadGlobalVariables()
Dim i As Integer, j As Integer, k As Integer, num As Integer, idx As Integer
Dim vname As String, vvalue As String, buf As String
Dim aname As String
' ADD START MARGE
Dim is_number As Boolean
' ADD END MARGE
    '�O���[�o���ϐ���S�폜
    With GlobalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    
    '�O���[�o���ϐ��̑�����ǂݏo��
    Input #SaveDataFileNumber, num
    
    '�e�ϐ��̒l��ǂݏo��
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
            'SetSkill�R�}���h�̃Z�[�u�f�[�^���G���A�X�ɑΉ�������
            If Left$(vname, 8) = "Ability(" Then
                idx = InStr(vname, ",")
                If idx > 0 Then
                    '�X�̔\�͒�`
                    aname = Mid$(vname, idx + 1, Len(vname) - idx - 1)
                    If ALDList.IsDefined(aname) Then
                        vname = Left$(vname, idx) & ALDList.Item(aname).AliasType(1) & ")"
                        If LLength(vvalue) = 1 Then
                            vvalue = vvalue & " " & aname
                        End If
                    End If
                Else
                    '�K�v�Z�\�p�̔\�͈ꗗ
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
            '���[�j���O��������\�͂��g���Ȃ��o�O�ɑΉ�
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
            '�s�K�v�Ȕ�\���\�͂ɑ΂���SetSkill���폜
            If Left$(vname, 8) = "Ability(" Then
                If Right$(vname, 5) = ",��\��)" Then
                    GoTo NextVariable
                End If
            End If
        End If
        
        If SaveDataVersion < 10732 Then
            '�s�K�v�Ȕ�\���\�͂ɑ΂���SetSkill�Ɣ\�͖��̃_�u����폜
            If Left$(vname, 8) = "Ability(" Then
                If InStr(vname, ",") = 0 Then
                    buf = ""
                    For j = 1 To LLength(vvalue)
                        aname = LIndex(vvalue, j)
                        If aname <> "��\��" Then
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
            '�G���A�X���ꂽ�\�͂�SetSkill�����ۂɃG���A�X�Ɋ܂܂�����������ɂȂ�o�O�ւ̑Ώ�
            If Left$(vname, 8) = "Ability(" Then
                If LIndex(vvalue, 1) = "0" Then
                    If LIndex(vvalue, 2) = "���" Then
                        vvalue = Format$(DEFAULT_LEVEL) & " ��� " & ListTail(vvalue, 3)
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
    'Option��S�ēǂݍ��񂾂�A�V�f�t�h���L���ɂȂ��Ă��邩�m�F����
    SetNewGUIMode
'ADD  END  240a
End Sub

'���[�J���ϐ����t�@�C���ɃZ�[�u
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

'���[�J���ϐ����t�@�C�����烍�[�h
Public Sub LoadLocalVariables()
Dim i As Integer, num As Integer
' MOD START MARGE
'Dim vname As String, vvalue As String
Dim vname As String, vvalue As String, buf As String
Dim is_number As Boolean
' MOD END MARGE
    '���[�J���ϐ���S�폜
    With LocalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    
    '���[�J���ϐ��̑�����ǂݏo��
    Input #SaveDataFileNumber, num
    
    For i = 1 To num
        '�ϐ��̒l��ǂݏo��
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
            'ClearSkill�̃o�O�Őݒ肳�ꂽ�ϐ����폜
            If Left$(vname, 8) = "Ability(" Then
                If vname = vvalue Then
                    GoTo NextVariable
                End If
            End If
        End If
        
        '�ϐ��̒l��ݒ�
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


'�C�x���g�G���[�\��
Public Sub DisplayEventErrorMessage(ByVal lnum As Long, ByVal msg As String)
Dim buf As String
    
    '�G���[���N�������t�@�C���A�s�ԍ��A�G���[���b�Z�[�W��\��
    buf = EventFileNames(EventFileID(lnum)) & "�F" _
        & EventLineNum(lnum) & "�s��" & vbCr & vbLf _
        & msg & vbCr & vbLf
    
    '�G���[���N�������s�Ƃ��̑O��̍s�̓��e��\��
    If lnum > 1 Then
        buf = buf & EventLineNum(lnum - 1) & ": " & EventData(lnum - 1) & vbCr & vbLf
    End If
    buf = buf & EventLineNum(lnum) & ": " & EventData(lnum) & vbCr & vbLf
    If lnum < UBound(EventData) Then
        buf = buf & EventLineNum(lnum + 1) & ": " & EventData(lnum + 1) & vbCr & vbLf
    End If
    
    ErrorMessage buf
End Sub

'�C���^�[�~�b�V�����R�}���h�u���j�b�g���X�g�v�ɂ����郆�j�b�g���X�g���쐬����
Public Sub MakeUnitList(Optional smode As String)
Dim u As Unit, p As Pilot
Dim xx As Integer, yy As Integer
Dim key_list() As Long
Dim max_item As Integer, max_value As Long
Dim max_str As String
Dim unit_list() As Unit
Dim i As Integer, j As Integer
Static key_type As String
    
    '���X�g�̃\�[�g���ڂ�ݒ�
    If smode <> "" Then
        key_type = smode
    End If
    If key_type = "" Then
        key_type = "�g�o"
    End If
    
    '�}�E�X�J�[�\���������v��
    Screen.MousePointer = vbHourglass
    
    '���炩���ߓP�ނ����Ă���
    For Each u In UList
        With u
            If .Status = "�o��" Then
                .Escape
            End If
        End With
    Next
    
    '�}�b�v���N���A
    LoadMapData ""
    SetupBackground "", "�X�e�[�^�X"
    
    '���j�b�g�ꗗ���쐬
    If key_type <> "����" Then
        '�z��쐬
        ReDim unit_list(UList.Count)
        ReDim key_list(UList.Count)
        i = 0
        For Each u In UList
            With u
                If .Status = "�o��" Or .Status = "�ҋ@" Then
                    i = i + 1
                    Set unit_list(i) = u
                    
                    '�\�[�g���鍀�ڂɂ��킹�ă\�[�g�̍ۂ̗D��x������
                    Select Case key_type
                        Case "�����N"
                            key_list(i) = .Rank
                        Case "�g�o"
                            key_list(i) = .HP
                        Case "�d�m"
                            key_list(i) = .EN
                        Case "���b"
                            key_list(i) = .Armor
                        Case "�^����"
                            key_list(i) = .Mobility
                        Case "�ړ���"
                            key_list(i) = .Speed
                        Case "�ő�U����"
                            For j = 1 To .CountWeapon
                                If .IsWeaponMastered(j) _
                                    And Not .IsDisabled(.Weapon(j).Name) _
                                    And Not .IsWeaponClassifiedAs(j, "��") _
                                Then
                                    If .WeaponPower(j, "") > key_list(i) Then
                                        key_list(i) = .WeaponPower(j, "")
                                    End If
                                End If
                            Next
                        Case "�Œ��˒�"
                            For j = 1 To .CountWeapon
                                If .IsWeaponMastered(j) _
                                    And Not .IsDisabled(.Weapon(j).Name) _
                                    And Not .IsWeaponClassifiedAs(j, "��") _
                                Then
                                    If .WeaponMaxRange(j) > key_list(i) Then
                                        key_list(i) = .WeaponMaxRange(j)
                                    End If
                                End If
                            Next
                        Case "���x��"
                            key_list(i) = .MainPilot.Level
                        Case "�r�o"
                            key_list(i) = .MainPilot.MaxSP
                        Case "�i��"
                            key_list(i) = .MainPilot.Infight
                        Case "�ˌ�"
                            key_list(i) = .MainPilot.Shooting
                        Case "����"
                            key_list(i) = .MainPilot.Hit
                        Case "���"
                            key_list(i) = .MainPilot.Dodge
                        Case "�Z��"
                            key_list(i) = .MainPilot.Technique
                        Case "����"
                            key_list(i) = .MainPilot.Intuition
                    End Select
                End If
            End With
        Next
        ReDim Preserve unit_list(i)
        ReDim Preserve key_list(i)
        
        '�\�[�g
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
        '�z��쐬
        ReDim unit_list(UList.Count)
        ReDim strkey_list(UList.Count)
        i = 0
        For Each u In UList
            With u
                If .Status = "�o��" Or .Status = "�ҋ@" Then
                    i = i + 1
                    Set unit_list(i) = u
                    If IsOptionDefined("���g��") Then
                        strkey_list(i) = .MainPilot.KanaName
                    Else
                        strkey_list(i) = .KanaName
                    End If
                End If
            End With
        Next
        ReDim Preserve unit_list(i)
        ReDim Preserve strkey_list(i)
        
        '�\�[�g
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
    
    'Font Regular 9pt �w�i
    With MainForm.picMain(0).Font
        .Size = 9
        .Bold = False
        .Italic = False
    End With
    PermanentStringMode = True
    HCentering = False
    VCentering = False
    
    '���j�b�g�̃��X�g���쐬
    xx = 1
    yy = 1
    For i = 1 To UBound(unit_list)
        Set u = unit_list(i)
        With u
            '���j�b�g�o���ʒu��܂�Ԃ�
            If xx > 15 Then
                xx = 1
                yy = yy + 1
                If yy > 40 Then
                    '���j�b�g�����������邽�߁A�ꕔ�̃p�C���b�g���\���o���܂���
                    Exit For
                End If
            End If
            
            '�p�C���b�g������Ă��Ȃ��ꍇ�̓_�~�[�p�C���b�g���悹��
            If .CountPilot = 0 Then
                Set p = PList.Add("�X�e�[�^�X�\���p�_�~�[�p�C���b�g(�U�R)", 1, "����")
                p.Ride u
            End If
            
            '�o��
            .UsedAction = 0
            .StandBy xx, yy
            
            '�v���C���[������ł��Ȃ��悤��
            .AddCondition "�񑀍�", -1
            
            '���j�b�g�̈��̂�\��
            DrawString .Nickname, 32 * xx + 2, 32 * yy - 31
            
            '�\�[�g���ڂɂ��킹�ă��j�b�g�̃X�e�[�^�X��\��
            Select Case key_type
                Case "�����N"
                    DrawString _
                        "RK" & Format$(key_list(i)) & " " & Term("HP", u) & Format$(.HP) _
                            & " " & Term("EN", u) & Format$(.EN), _
                        32 * xx + 2, 32 * yy - 15
                Case "�g�o", "�d�m", "����"
                    DrawString _
                        Term("HP", u) & Format$(.HP) & " " & Term("EN", u) & Format$(.EN), _
                        32 * xx + 2, 32 * yy - 15
                Case "���b"
                    DrawString Term("���b", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "�^����"
                    DrawString Term("�^����", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "�ړ���"
                    DrawString Term("�ړ���", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "�ő�U����"
                    DrawString "�U����" & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "�Œ��˒�"
                    DrawString "�˒�" & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "���x��"
                    DrawString "Lv" & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "�r�o"
                    DrawString Term("SP", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "�i��"
                    DrawString Term("�i��", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "�ˌ�"
                    If .MainPilot.HasMana() Then
                        DrawString Term("����", u) & Format$(key_list(i)), _
                            32 * xx + 2, 32 * yy - 15
                    Else
                        DrawString Term("�ˌ�", u) & Format$(key_list(i)), _
                            32 * xx + 2, 32 * yy - 15
                    End If
                Case "����"
                    DrawString Term("����", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "���"
                    DrawString Term("���", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "�Z��"
                    DrawString Term("�Z��", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "����"
                    DrawString Term("����", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
            End Select
            
            '�\���ʒu���E��5�}�X���炷
            xx = xx + 5
        End With
    Next
    
    '�t�H���g�̐ݒ��߂��Ă���
    With MainForm.picMain(0).Font
        .Size = 16
        .Bold = True
        .Italic = False
    End With
    PermanentStringMode = False
    
    RedrawScreen
    
    '�}�E�X�J�[�\�������ɖ߂�
    Screen.MousePointer = 0
End Sub


'�`��̊���W�ʒu��ۑ�
Public Sub SaveBasePoint()
    BasePointIndex = BasePointIndex + 1
    If BasePointIndex > UBound(SavedBaseX) Then
        BasePointIndex = 0
    End If
    SavedBaseX(BasePointIndex) = BaseX
    SavedBaseY(BasePointIndex) = BaseY
End Sub

'�`��̊���W�ʒu�𕜌�
Public Sub RestoreBasePoint()
    If BasePointIndex <= 0 Then
        BasePointIndex = UBound(SavedBaseX)
    End If
    BaseX = SavedBaseX(BasePointIndex)
    BaseY = SavedBaseY(BasePointIndex)
    BasePointIndex = BasePointIndex - 1
End Sub

'�`��̊���W�ʒu�����Z�b�g
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

