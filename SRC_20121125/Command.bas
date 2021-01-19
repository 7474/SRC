Attribute VB_Name = "Commands"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'���j�b�g���}�b�v�R�}���h�̎��s���s�����W���[��

'���j�b�g�R�}���h�̃��j���[�ԍ�
Public Const MoveCmdID = 0
Public Const TeleportCmdID = 1
Public Const JumpCmdID = 2
Public Const TalkCmdID = 3
Public Const AttackCmdID = 4
Public Const FixCmdID = 5
Public Const SupplyCmdID = 6
Public Const AbilityCmdID = 7
Public Const ChargeCmdID = 8
Public Const SpecialPowerCmdID = 9
Public Const TransformCmdID = 10
Public Const SplitCmdID = 11
Public Const CombineCmdID = 12
Public Const HyperModeCmdID = 13
Public Const GroundCmdID = 14
Public Const SkyCmdID = 15
Public Const UndergroundCmdID = 16
Public Const WaterCmdID = 17
Public Const LaunchCmdID = 18
Public Const ItemCmdID = 19
Public Const DismissCmdID = 20
Public Const OrderCmdID = 21
Public Const FeatureListCmdID = 22
Public Const WeaponListCmdID = 23
Public Const AbilityListCmdID = 24
Public Const UnitCommand1CmdID = 25
Public Const UnitCommand2CmdID = 26
Public Const UnitCommand3CmdID = 27
Public Const UnitCommand4CmdID = 28
Public Const UnitCommand5CmdID = 29
Public Const UnitCommand6CmdID = 30
Public Const UnitCommand7CmdID = 31
Public Const UnitCommand8CmdID = 32
Public Const UnitCommand9CmdID = 33
Public Const UnitCommand10CmdID = 34
Public Const WaitCmdID = 35

'�}�b�v�R�}���h�̃��j���[�ԍ�
Public Const EndTurnCmdID = 0
Public Const DumpCmdID = 1
Public Const UnitListCmdID = 2
Public Const SearchSpecialPowerCmdID = 3
Public Const GlobalMapCmdID = 4
Public Const OperationObjectCmdID = 5
Public Const MapCommand1CmdID = 6
Public Const MapCommand2CmdID = 7
Public Const MapCommand3CmdID = 8
Public Const MapCommand4CmdID = 9
Public Const MapCommand5CmdID = 10
Public Const MapCommand6CmdID = 11
Public Const MapCommand7CmdID = 12
Public Const MapCommand8CmdID = 13
Public Const MapCommand9CmdID = 14
Public Const MapCommand10CmdID = 15
Public Const AutoDefenseCmdID = 16
Public Const ConfigurationCmdID = 17
Public Const RestartCmdID = 18
Public Const QuickLoadCmdID = 19
Public Const QuickSaveCmdID = 20

'���݂̃R�}���h�̐i�s��
Public CommandState As String

'�N���b�N�҂����[�h
Public WaitClickMode As Boolean
'�{�����[�h
Public ViewMode As Boolean

'�}�b�v�R�}���h���x���̃��X�g
Private MapCommandLabelList(10) As String
'���j�b�g�R�}���h���x���̃��X�g
Private UnitCommandLabelList(10) As String

'���ݑI������Ă������
Public SelectedUnit As Unit            '���j�b�g
Public SelectedCommand As String       '�R�}���h
Public SelectedTarget As Unit          '�^�[�Q�b�g
Public SelectedX As Integer            '�w���W
Public SelectedY As Integer            '�x���W
Public SelectedWeapon As Integer       '����
Public SelectedWeaponName As String
Public SelectedTWeapon As Integer      '��������
Public SelectedTWeaponName As String
Public SelectedDefenseOption As String '�h����@
Public SelectedAbility As Integer      '�A�r���e�B
Public SelectedAbilityName As String
Public SelectedPilot As Pilot          '�p�C���b�g
Public SelectedItem As Integer         '���X�g�{�b�N�X���̃A�C�e��
Public SelectedSpecialPower As String  '�X�y�V�����p���[
Public SelectedPartners() As Unit      '���̋Z�̃p�[�g�i�[
' ADD START MARGE
Public SelectedUnitMoveCost As Integer '�I���������j�b�g�̈ړ��͏����
' ADD END MARGE

'�I���󋵂̋L�^�p�ϐ�
Public SelectionStackIndex As Integer
Public SavedSelectedUnit() As Unit
Public SavedSelectedTarget() As Unit
Public SavedSelectedUnitForEvent() As Unit
Public SavedSelectedTargetForEvent() As Unit
Public SavedSelectedWeapon() As Integer
Public SavedSelectedWeaponName() As String
Public SavedSelectedTWeapon() As Integer
Public SavedSelectedTWeaponName() As String
Public SavedSelectedDefenseOption() As String
Public SavedSelectedAbility() As Integer
Public SavedSelectedAbilityName() As String
Public SavedSelectedX() As Integer
Public SavedSelectedY() As Integer

'������g�����ǂ���
Public UseSupportAttack As Boolean
Public UseSupportGuard As Boolean

'�u�����X�y�V�����p���[���s�v���g���ăX�y�V�����p���[���g�p���邩�ǂ���
Private WithDoubleSPConsumption As Boolean

'�U�����s�����j�b�g
Public AttackUnit As Unit
'����U�����s�����j�b�g
Public SupportAttackUnit As Unit
'����h����s�����j�b�g
Public SupportGuardUnit As Unit
'����h����s�����j�b�g�̂g�o�l
Public SupportGuardUnitHPRatio As Double
'����h����s�����j�b�g(������)
Public SupportGuardUnit2 As Unit
'����h����s�����j�b�g�̂g�o�l(������)
Public SupportGuardUnitHPRatio2 As Double

'�ړ��O�̃��j�b�g�̏��
Private PrevUnitX As Integer
Private PrevUnitY As Integer
Private PrevUnitArea As String
Private PrevUnitEN As Integer
Private PrevCommand As String

'�ړ��������j�b�g�̏��
Public MovedUnit As Unit
Public MovedUnitSpeed As Integer


'�R�}���h�̏�����i�߂�
'by_cancel = True �̏ꍇ�̓R�}���h���L�����Z�������ꍇ�̏���
Public Sub ProceedCommand(Optional ByVal by_cancel As Boolean)
Dim i As Integer, j As Integer, n As Integer
Dim u As Unit, uname As String, p As Pilot
Dim buf As String
Dim lab As LabelData
    
    '�{�����[�h�̓L�����Z���ŏI���B����ȊO�̓��͖͂���
    If ViewMode Then
        If by_cancel Then
            ViewMode = False
        End If
        Exit Sub
    End If
    
    '�������s����܂ł���ȍ~�̃R�}���h��t���֎~
    '(�X�N���[���֎~�ɂ��Ȃ���΂Ȃ�Ȃ��قǂ̎��Ԃ͂Ȃ����߁ALockGUI�͎g��Ȃ�)
    IsGUILocked = True
    
    '�R�}���h���s���s���Ƃ������Ƃ̓V�i���I�v���C���Ƃ������ƂȂ̂Ŗ��񏉊�������B
    IsScenarioFinished = False
    IsCanceled = False
    
    '�|�b�v�A�b�v���j���[��ŉ������}�E�X�{�^�������E�ǂ��炩�𔻒肷�邽�߁A
    '���炩����GetAsyncKeyState()�����s���Ă����K�v������
    Call GetAsyncKeyState(RButtonID)
    
    Select Case CommandState
        Case "���j�b�g�I��", "�}�b�v�R�}���h"
            Set SelectedUnit = Nothing
' ADD START MARGE
            SelectedUnitMoveCost = 0
' ADD END MARGE
            If 1 <= PixelToMapX(MouseX) And PixelToMapX(MouseX) <= MapWidth _
                And 1 <= PixelToMapY(MouseY) And PixelToMapY(MouseY) <= MapHeight _
            Then
                Set SelectedUnit = MapDataForUnit(PixelToMapX(MouseX), PixelToMapY(MouseY))
            End If
            If SelectedUnit Is Nothing Then
                SelectedX = PixelToMapX(MouseX)
                SelectedY = PixelToMapY(MouseY)
                
                If MapFileName <> "" Then
                    '�ʏ�̃X�e�[�W
                    
                    DisplayGlobalStatus
                    
                    '�^�[���I��
                    If ViewMode Then
                        MainForm.mnuMapCommandItem(EndTurnCmdID).Caption = "�����Ґ��ɖ߂�"
                    Else
                        MainForm.mnuMapCommandItem(EndTurnCmdID).Caption = "�^�[���I��"
                    End If
                    MainForm.mnuMapCommandItem(EndTurnCmdID).Visible = True
                    
                    '���f
                    If IsOptionDefined("�f�o�b�O") Then
                        MainForm.mnuMapCommandItem(DumpCmdID).Visible = True
                    Else
                        If Not IsOptionDefined("�N�C�b�N�Z�[�u�s��") Then
                            MainForm.mnuMapCommandItem(DumpCmdID).Visible = True
                        Else
                            MainForm.mnuMapCommandItem(DumpCmdID).Visible = False
                        End If
                    End If
                    
                    '�S�̃}�b�v
                    MainForm.mnuMapCommandItem(GlobalMapCmdID).Visible = True
                    
                    '���ړI
                    If IsEventDefined("��������") Then
                        MainForm.mnuMapCommandItem(OperationObjectCmdID).Visible = True
                    Else
                        MainForm.mnuMapCommandItem(OperationObjectCmdID).Visible = False
                    End If
                    
                    '�����������[�h
                    MainForm.mnuMapCommandItem(AutoDefenseCmdID).Visible = True
                    
                    '�ݒ�ύX
                    MainForm.mnuMapCommandItem(ConfigurationCmdID).Visible = True
                    
                    '���X�^�[�g
                    If IsRestartSaveDataAvailable And Not ViewMode Then
                        MainForm.mnuMapCommandItem(RestartCmdID).Visible = True
                    Else
                        MainForm.mnuMapCommandItem(RestartCmdID).Visible = False
                    End If
                    
                    '�N�C�b�N���[�h
                    If IsQuickSaveDataAvailable And Not ViewMode Then
                        MainForm.mnuMapCommandItem(QuickLoadCmdID).Visible = True
                    Else
                        MainForm.mnuMapCommandItem(QuickLoadCmdID).Visible = False
                    End If
                    
                    '�N�C�b�N�Z�[�u
                    If ViewMode Then
                        MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = False
                    ElseIf IsOptionDefined("�f�o�b�O") Then
                        MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = True
                    Else
                        If Not IsOptionDefined("�N�C�b�N�Z�[�u�s��") Then
                            MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = True
                        Else
                            MainForm.mnuMapCommandItem(QuickSaveCmdID).Visible = False
                        End If
                    End If
                Else
                    '�p�C���b�g�X�e�[�^�X�E���j�b�g�X�e�[�^�X�̃X�e�[�W
                    With MainForm
                        .mnuMapCommandItem(EndTurnCmdID).Visible = False
                        .mnuMapCommandItem(DumpCmdID).Visible = False
                        .mnuMapCommandItem(GlobalMapCmdID).Visible = False
                        .mnuMapCommandItem(OperationObjectCmdID).Visible = False
                        .mnuMapCommandItem(AutoDefenseCmdID).Visible = False
                        .mnuMapCommandItem(ConfigurationCmdID).Visible = False
                        .mnuMapCommandItem(RestartCmdID).Visible = False
                        .mnuMapCommandItem(QuickLoadCmdID).Visible = False
                        .mnuMapCommandItem(QuickSaveCmdID).Visible = False
                    End With
                End If
                
                '�X�y�V�����p���[����
                MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = False
                MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Caption = _
                    Term("�X�y�V�����p���[") & "����"
                For Each p In PList
                    With p
                        If .Party = "����" Then
                            If .CountSpecialPower > 0 Then
                                MainForm.mnuMapCommandItem(SearchSpecialPowerCmdID).Visible = True
                                Exit For
                            End If
                        End If
                    End With
                Next
                
                '�C�x���g�Œ�`���ꂽ�}�b�v�R�}���h
                With MainForm
                    .mnuMapCommandItem(MapCommand1CmdID).Visible = False
                    .mnuMapCommandItem(MapCommand2CmdID).Visible = False
                    .mnuMapCommandItem(MapCommand3CmdID).Visible = False
                    .mnuMapCommandItem(MapCommand4CmdID).Visible = False
                    .mnuMapCommandItem(MapCommand5CmdID).Visible = False
                    .mnuMapCommandItem(MapCommand6CmdID).Visible = False
                    .mnuMapCommandItem(MapCommand7CmdID).Visible = False
                    .mnuMapCommandItem(MapCommand8CmdID).Visible = False
                    .mnuMapCommandItem(MapCommand9CmdID).Visible = False
                    .mnuMapCommandItem(MapCommand10CmdID).Visible = False
                End With
                If Not ViewMode Then
                    i = MapCommand1CmdID
                    For Each lab In colEventLabelList
                        With lab
                            If .Name = MapCommandEventLabel Then
                                If .Enable Then
                                    If .CountPara = 2 Then
                                        MainForm.mnuMapCommandItem(i).Visible = True
                                    ElseIf StrToLng(.Para(3)) <> 0 Then
                                        MainForm.mnuMapCommandItem(i).Visible = True
                                    End If
                                End If
                                
                                If MainForm.mnuMapCommandItem(i).Visible Then
                                    MainForm.mnuMapCommandItem(i).Caption = .Para(2)
                                    MapCommandLabelList(i - MapCommand1CmdID + 1) = .LineNum
                                    i = i + 1
                                    If i > MapCommand10CmdID Then
                                        Exit For
                                    End If
                                End If
                            End If
                        End With
                    Next
                End If
                
                CommandState = "�}�b�v�R�}���h"
                
                IsGUILocked = False
' ADD START 240a
                '�����ɗ������_��cancel=True�̓��j�b�g�̂��Ȃ��Z�����E�N���b�N�����ꍇ�̂�
                If by_cancel Then
                    If NewGUIMode And (Not MapFileName = "") Then
                        If MouseX < MainPWidth \ 2 Then
                            MainForm.picUnitStatus.Move MainPWidth - 240, 10
                        Else
                            MainForm.picUnitStatus.Move 5, 10
                        End If
                        MainForm.picUnitStatus.Visible = True
                    End If
                End If
' ADD  END  240a
                MainForm.PopupMenu MainForm.mnuMapCommand, 6, MouseX, MouseY + 6
                Exit Sub
            End If
            
            Set SelectedUnitForEvent = SelectedUnit
            SelectedWeapon = 0
            SelectedTWeapon = 0
            SelectedAbility = 0
            
            If by_cancel Then
                '���j�b�g��ŃL�����Z���{�^�����������ꍇ�͕���ꗗ
                '�������̓A�r���e�B�ꗗ��\������
                With SelectedUnit
                    '��񂪉B������Ă���ꍇ�͕\�����Ȃ�
                    If (IsOptionDefined("���j�b�g���B��") _
                            And (Not .IsConditionSatisfied("���ʍς�") _
                                And (.Party0 = "�G" Or .Party0 = "����"))) _
                        Or .IsConditionSatisfied("���j�b�g���B��") _
                        Or .IsFeatureAvailable("�_�~�[���j�b�g") _
                    Then
                        IsGUILocked = False
                        Exit Sub
                    End If
                    
                    If .CountWeapon = 0 And .CountAbility > 0 Then
                        AbilityListCommand
                    Else
                        WeaponListCommand
                    End If
                End With
                IsGUILocked = False
                Exit Sub
            End If
            
            CommandState = "�R�}���h�I��"
            ProceedCommand by_cancel
            
        Case "�R�}���h�I��"
'MOD START 240aClearUnitStatus
'            If MainWidth <> 15 Then
'                DisplayUnitStatus SelectedUnit
'            End If
            If Not NewGUIMode Then
                DisplayUnitStatus SelectedUnit
            Else
                ClearUnitStatus
            End If
'MOD  END  240a
            
            '�����ꗗ�ȊO�͈�U�����Ă���
            With MainForm
                .mnuUnitCommandItem(WeaponListCmdID).Visible = True
                For i = 0 To WeaponListCmdID - 1
                    .mnuUnitCommandItem(i).Visible = False
                Next
                For i = WeaponListCmdID + 1 To WaitCmdID
                    .mnuUnitCommandItem(i).Visible = False
                Next
            End With
            
            Set SelectedUnitForEvent = SelectedUnit
            Set SelectedTarget = Nothing
            Set SelectedTargetForEvent = Nothing
            
            With SelectedUnit
                '����\�́��A�r���e�B�ꗗ�͂ǂ̃��j�b�g�ł������\��������̂�
                '��ɔ��肵�Ă���
                
                '����\�͈ꗗ�R�}���h
                For i = 1 To .CountAllFeature
                    If .AllFeatureName(i) <> "" Then
                        Select Case .AllFeature(i)
                            Case "����"
                                If UList.IsDefined(LIndex(.AllFeatureData(i), 2)) Then
                                    MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
                                    Exit For
                                End If
                            Case Else
                                MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
                                Exit For
                        End Select
                    ElseIf .AllFeature(i) = "�p�C���b�g�\�͕t��" _
                        Or .AllFeature(i) = "�p�C���b�g�\�͋���" _
                    Then
                        If InStr(.AllFeatureData(i), "��\��") = 0 Then
                            MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
                            Exit For
                        End If
                    ElseIf .AllFeature(i) = "����N���X" _
                        Or .AllFeature(i) = "�h��N���X" _
                    Then
                        If IsOptionDefined("�A�C�e������") Then
                            MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
                            Exit For
                        End If
                    End If
                Next
                With .MainPilot
                    For i = 1 To .CountSkill
                        If .SkillName0(i) <> "��\��" And .SkillName0(i) <> "" Then
                            Select Case .Skill(i)
                                Case "�ϋv"
                                    If Not IsOptionDefined("�h��͐���") _
                                        And Not IsOptionDefined("�h��̓��x���A�b�v") _
                                    Then
                                        MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
                                        Exit For
                                    End If
                                Case "�ǉ����x��", _
                                     "�i���t�o", "�ˌ��t�o", "�����t�o", "����t�o", _
                                     "�Z�ʂt�o", "�����t�o", "�r�o�t�o", _
                                     "�i���c�n�v�m", "�ˌ��c�n�v�m", "�����c�n�v�m", "����c�n�v�m", _
                                     "�Z�ʂc�n�v�m", "�����c�n�v�m", "�r�o�c�n�v�m", _
                                     "���b�Z�[�W", "���͏��L"
                                Case Else
                                    MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = True
                                    Exit For
                            End Select
                        End If
                    Next
                End With
                
                '�A�r���e�B�ꗗ�R�}���h
                For i = 1 To .CountAbility
                    If .IsAbilityMastered(i) _
                        And Not .IsDisabled(.Ability(i).Name) _
                        And (Not .IsAbilityClassifiedAs(i, "��") _
                            Or .IsCombinationAbilityAvailable(i, True)) _
                        And Not .Ability(i).IsItem _
                    Then
                        MainForm.mnuUnitCommandItem(AbilityListCmdID).Caption = _
                            Term("�A�r���e�B", SelectedUnit) & "�ꗗ"
                        MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = True
                        Exit For
                    End If
                Next
                
                '��������Ȃ��ꍇ
                If .Party <> "����" _
                    Or .IsConditionSatisfied("�񑀍�") _
                    Or ViewMode _
                Then
                    '�������j�b�g�͖��߃R�}���h���g�p�\
                    If .Party = "�m�o�b" _
                        And .IsFeatureAvailable("�������j�b�g") _
                        And Not .IsConditionSatisfied("����") _
                        And Not .IsConditionSatisfied("����") _
                        And Not .IsConditionSatisfied("���|") _
                        And Not .IsConditionSatisfied("�\��") _
                        And Not .IsConditionSatisfied("����m") _
                        And Not ViewMode _
                    Then
                        If Not .Summoner Is Nothing Then
                            If .Summoner.Party = "����" Then
                                MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "����"
                                MainForm.mnuUnitCommandItem(OrderCmdID).Visible = True
                            End If
                        End If
                    End If
                    
                    '�����������j�b�g�ɑ΂��Ă����߃R�}���h���g�p�\
                    If .Party = "�m�o�b" _
                        And .IsConditionSatisfied("����") _
                        And Not .IsConditionSatisfied("����") _
                        And Not .IsConditionSatisfied("���|") _
                        And Not .IsConditionSatisfied("�\��") _
                        And Not .IsConditionSatisfied("����m") _
                        And Not ViewMode _
                    Then
                        If Not .Master Is Nothing Then
                            If .Master.Party = "����" Then
                                MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "����"
                                MainForm.mnuUnitCommandItem(OrderCmdID).Visible = True
                            End If
                        End If
                    End If
                    
                    '�_�~�[���j�b�g�̏ꍇ�̓R�}���h�ꗗ��\�����Ȃ�
                    If .IsFeatureAvailable("�_�~�[���j�b�g") Then
                        '����\�͈ꗗ
                        If MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible Then
                            UnitCommand FeatureListCmdID
                        Else
                            CommandState = "���j�b�g�I��"
                        End If
                        
                        IsGUILocked = False
                        Exit Sub
                    End If
                    
                    If MapFileName <> "" Then
                        MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "�ړ��͈�"
                        MainForm.mnuUnitCommandItem(MoveCmdID).Visible = True
                        
                        For i = 1 To .CountWeapon
                            If .IsWeaponAvailable(i, "") _
                                And Not .IsWeaponClassifiedAs(i, "�l") _
                            Then
                                MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "�˒��͈�"
                                MainForm.mnuUnitCommandItem(AttackCmdID).Visible = True
                            End If
                        Next
                    End If
                    
                    '���j�b�g�X�e�[�^�X�R�}���h�p
                    If MapFileName = "" Then
                        '�ό`�R�}���h
                        If .IsFeatureAvailable("�ό`") Then
                            MainForm.mnuUnitCommandItem(TransformCmdID).Caption = _
                                .FeatureName("�ό`")
                            If MainForm.mnuUnitCommandItem(TransformCmdID).Caption = "" Then
                                MainForm.mnuUnitCommandItem(TransformCmdID).Caption = "�ό`"
                            End If
                            
                            For i = 2 To LLength(.FeatureData("�ό`"))
                                uname = LIndex(.FeatureData("�ό`"), i)
                                If .OtherForm(uname).IsAvailable Then
                                    MainForm.mnuUnitCommandItem(TransformCmdID).Visible = True
                                    Exit For
                                End If
                            Next
                        End If
                        
                        '�����R�}���h
                        If .IsFeatureAvailable("����") Then
                            MainForm.mnuUnitCommandItem(SplitCmdID).Visible = True
                            MainForm.mnuUnitCommandItem(SplitCmdID).Caption = _
                                .FeatureName("����")
                            If MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "" Then
                                MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "����"
                            End If
                            
                            buf = .FeatureData("����")
                            
                            '�����`�Ԃ����p�o���Ȃ��ꍇ�͕������s��Ȃ�
                            For i = 2 To LLength(buf)
                                If Not UList.IsDefined(LIndex(buf, i)) Then
                                    MainForm.mnuUnitCommandItem(SplitCmdID).Visible = False
                                    Exit For
                                End If
                            Next
                            
                            '�p�C���b�g������Ȃ��ꍇ���������s��Ȃ�
                            If MainForm.mnuUnitCommandItem(SplitCmdID).Visible Then
                                n = 0
                                For i = 2 To LLength(buf)
                                    With UList.Item(LIndex(buf, i)).Data
                                        If Not .IsFeatureAvailable("�������j�b�g") Then
                                            n = n + Abs(.PilotNum)
                                        End If
                                    End With
                                Next
                                If .CountPilot < n Then
                                    MainForm.mnuUnitCommandItem(SplitCmdID).Visible = False
                                End If
                            End If
                        End If
                        If .IsFeatureAvailable("�p�[�c����") Then
                            MainForm.mnuUnitCommandItem(SplitCmdID).Caption = _
                                .FeatureName("�p�[�c����")
                            If MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "" Then
                                MainForm.mnuUnitCommandItem(SplitCmdID).Caption = "�p�[�c����"
                            End If
                            MainForm.mnuUnitCommandItem(SplitCmdID).Visible = True
                        End If
                        
                        '���̃R�}���h
                        If .IsFeatureAvailable("����") Then
                            For i = 1 To .CountFeature
                                If .Feature(i) = "����" Then
                                    n = 0
                                    '�p�[�g�i�[�����݂��Ă��邩�H
                                    For j = 3 To LLength(.FeatureData(i))
                                        Set u = UList.Item(LIndex(.FeatureData(i), j))
                                        If u Is Nothing Then
                                            Exit For
                                        End If
                                        
                                        If u.Status <> "�o��" _
                                            And u.CurrentForm.IsFeatureAvailable("���̐���") _
                                        Then
                                            Exit For
                                        End If
                                        n = n + 1
                                    Next
                                    
                                    '���̐�̃��j�b�g���쐬����Ă��邩�H
                                    If Not UList.IsDefined(LIndex(.FeatureData(i), 2)) Then
                                        n = 0
                                    End If
                                    
                                    '���ׂĂ̏����𖞂����Ă���ꍇ
                                    If n = LLength(.FeatureData(i)) - 2 Then
                                        MainForm.mnuUnitCommandItem(CombineCmdID).Visible = True
                                        MainForm.mnuUnitCommandItem(CombineCmdID).Caption = _
                                            LIndex(.FeatureData(i), 1)
                                        If MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "��\��" Then
                                            MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "����"
                                        End If
                                        Exit For
                                    End If
                                End If
                            Next
                        ElseIf .IsFeatureAvailable("�p�[�c����") Then
                            MainForm.mnuUnitCommandItem(CombineCmdID).Caption = "�p�[�c����"
                            MainForm.mnuUnitCommandItem(CombineCmdID).Visible = True
                        End If
                        
                        If Not .IsConditionSatisfied("�m�[�}�����[�h�t��") Then
                            '�n�C�p�[���[�h�R�}���h
                            If .IsFeatureAvailable("�n�C�p�[���[�h") Then
                                uname = LIndex(.FeatureData("�n�C�p�[���[�h"), 2)
                                If .OtherForm(uname).IsAvailable Then
                                    MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
                                    MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = _
                                        LIndex(.FeatureData("�n�C�p�[���[�h"), 1)
                                    If MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "��\��" Then
                                        MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "�n�C�p�[���[�h"
                                    End If
                                End If
                            ElseIf .IsFeatureAvailable("�m�[�}�����[�h") Then
                                uname = LIndex(.FeatureData("�m�[�}�����[�h"), 1)
                                If .OtherForm(uname).IsAvailable Then
                                    MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
                                    MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "�m�[�}�����[�h"
                                    If uname = LIndex(.FeatureData("�ό`"), 2) Then
                                        MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = False
                                    End If
                                End If
                            End If
                        Else
                            '�ϐg����
                            If InStr(.FeatureData("�m�[�}�����[�h"), "�蓮����") > 0 Then
                                MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
                                If .IsFeatureAvailable("�ϐg�����R�}���h��") Then
                                    MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = _
                                        .FeatureData("�ϐg�����R�}���h��")
                                ElseIf .IsHero Then
                                    MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "�ϐg����"
                                Else
                                    MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "���ꃂ�[�h����"
                                End If
                            End If
                        End If
                        
                        '�����R�}���h
                        If .IsFeatureAvailable("����") Then
                            MainForm.mnuUnitCommandItem(OrderCmdID).Caption = "����"
                            
                            For i = 1 To LLength(.FeatureData("����"))
                                uname = LIndex(.FeatureData("����"), i)
                                If .OtherForm(uname).IsAvailable Then
                                    MainForm.mnuUnitCommandItem(OrderCmdID).Visible = True
                                    Exit For
                                End If
                            Next
                            
                            '�G���A�X�Ŋ����̖��̂��ύX����Ă���H
                            With ALDList
                                For i = 1 To .Count
                                    With .Item(i)
                                        If .AliasType(1) = "����" Then
                                            MainForm.mnuUnitCommandItem(OrderCmdID).Caption = .Name
                                            Exit For
                                        End If
                                    End With
                                Next
                            End With
                        End If
                    End If
                    
                    '���j�b�g�R�}���h
                    If Not ViewMode Then
                        i = UnitCommand1CmdID
                        For Each lab In colEventLabelList
                            With lab
                                If .Name = UnitCommandEventLabel And .Enable Then
                                    buf = GetValueAsString(.Para(3))
                                    If (SelectedUnit.Party = "����" _
                                        And (buf = SelectedUnit.MainPilot.Name _
                                            Or buf = SelectedUnit.MainPilot.Nickname _
                                            Or buf = SelectedUnit.Name)) _
                                        Or buf = SelectedUnit.Party _
                                        Or buf = "�S" _
                                    Then
                                        If .CountPara <= 3 Then
                                            MainForm.mnuUnitCommandItem(i).Visible = True
                                        ElseIf GetValueAsLong(.Para(4)) <> 0 Then
                                            MainForm.mnuUnitCommandItem(i).Visible = True
                                        End If
                                    End If
                                    
                                    If MainForm.mnuUnitCommandItem(i).Visible Then
                                        MainForm.mnuUnitCommandItem(i).Caption = .Para(2)
                                        UnitCommandLabelList(i - UnitCommand1CmdID + 1) = .LineNum
                                        i = i + 1
                                        If i > UnitCommand10CmdID Then
                                            Exit For
                                        End If
                                    End If
                                End If
                            End With
                        Next
                    End If
                    
                    '���m�F���j�b�g�̏ꍇ�͏����B��
                    If (IsOptionDefined("���j�b�g���B��") _
                            And (Not .IsConditionSatisfied("���ʍς�") _
                                And (.Party0 = "�G" Or .Party0 = "����"))) _
                        Or .IsConditionSatisfied("���j�b�g���B��") _
                    Then
                        MainForm.mnuUnitCommandItem(MoveCmdID).Visible = True
                        MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
                        MainForm.mnuUnitCommandItem(FeatureListCmdID).Visible = False
                        MainForm.mnuUnitCommandItem(WeaponListCmdID).Visible = False
                        MainForm.mnuUnitCommandItem(AbilityListCmdID).Visible = False
                        For i = 1 To WaitCmdID
                            If MainForm.mnuUnitCommandItem(i).Visible Then
                                Exit For
                            End If
                        Next
                        If i > WaitCmdID Then
                            '�\���\�ȃR�}���h���Ȃ�����
                            CommandState = "���j�b�g�I��"
                            IsGUILocked = False
                            Exit Sub
                        End If
                        '���j���[�R�}���h��S�ĎE���Ă��܂��ƃG���[�ɂȂ�̂ł����Ŕ�\��
                        MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
                    End If
                    
                    IsGUILocked = False
                    If by_cancel Then
                        MainForm.PopupMenu MainForm.mnuUnitCommand, 6, MouseX, MouseY + 5
                    Else
                        MainForm.PopupMenu MainForm.mnuUnitCommand, 6, MouseX, MouseY - 6
                    End If
                    Exit Sub
                    
                '�s���I�����Ă���ꍇ
                ElseIf .Action = 0 Then
                    '���i�R�}���h�͎g�p�\
                    If .IsFeatureAvailable("���") Then
                        If .Area <> "�n��" Then
                            If .CountUnitOnBoard > 0 Then
                                MainForm.mnuUnitCommandItem(LaunchCmdID).Visible = True
                            End If
                        End If
                    End If
                    
                    '���j�b�g�R�}���h
                    i = UnitCommand1CmdID
                    For Each lab In colEventLabelList
                        With lab
                            If .Name = UnitCommandEventLabel _
                                And (.AsterNum = 1 Or .AsterNum = 3) _
                            Then
                                If .Enable Then
                                    buf = .Para(3)
                                    If (SelectedUnit.Party = "����" _
                                        And (buf = SelectedUnit.MainPilot.Name _
                                            Or buf = SelectedUnit.MainPilot.Nickname _
                                            Or buf = SelectedUnit.Name)) _
                                        Or buf = SelectedUnit.Party _
                                        Or buf = "�S" _
                                    Then
                                        If .CountPara <= 3 Then
                                            MainForm.mnuUnitCommandItem(i).Visible = True
                                        ElseIf StrToLng(.Para(4)) <> 0 Then
                                            MainForm.mnuUnitCommandItem(i).Visible = True
                                        End If
                                    End If
                                End If
                                
                                If MainForm.mnuUnitCommandItem(i).Visible Then
                                    MainForm.mnuUnitCommandItem(i).Caption = .Para(2)
                                    UnitCommandLabelList(i - UnitCommand1CmdID + 1) = .LineNum
                                    i = i + 1
                                    If i > UnitCommand10CmdID Then
                                        Exit For
                                    End If
                                End If
                            End If
                        End With
                    Next
                    
                    IsGUILocked = False
                    MainForm.PopupMenu MainForm.mnuUnitCommand, 6, MouseX, MouseY - 5
                    Exit Sub
                End If
                
                '�ړ��R�}���h
                MainForm.mnuUnitCommandItem(MoveCmdID).Caption = "�ړ�"
                If .Speed <= 0 Then
                    MainForm.mnuUnitCommandItem(WaitCmdID).Visible = True '�ҋ@
                Else
                    MainForm.mnuUnitCommandItem(MoveCmdID).Visible = True '�ړ�
                End If
                
                '�e���|�[�g�R�}���h
                If .IsFeatureAvailable("�e���|�[�g") Then
                    If Len(.FeatureData("�e���|�[�g")) > 0 Then
                        MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = _
                            LIndex(.FeatureData("�e���|�[�g"), 1)
                    Else
                        MainForm.mnuUnitCommandItem(TeleportCmdID).Caption = "�e���|�[�g"
                    End If
                    
                    If LLength(.FeatureData("�e���|�[�g")) = 2 Then
                        If .EN >= CInt(LIndex(.FeatureData("�e���|�[�g"), 2)) Then
                            MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = True
                        End If
                        '�ʏ�ړ����e���|�[�g�̏ꍇ
                        If .Speed0 = 0 _
                            Or (.FeatureLevel("�e���|�[�g") >= 0 _
                                And LIndex(.FeatureData("�e���|�[�g"), 2) = "0") _
                        Then
                            MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
                        End If
                    Else
                        If .EN >= 40 Then
                            MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = True
                        End If
                    End If
                    
                    If .IsConditionSatisfied("�ړ��s�\") Then
                        MainForm.mnuUnitCommandItem(TeleportCmdID).Visible = False
                    End If
                End If
                
                '�W�����v�R�}���h
                If .IsFeatureAvailable("�W�����v") _
                    And .Area <> "��" And .Area <> "�F��" _
                Then
                    If Len(.FeatureData("�W�����v")) > 0 Then
                        MainForm.mnuUnitCommandItem(JumpCmdID).Caption = _
                            LIndex(.FeatureData("�W�����v"), 1)
                    Else
                        MainForm.mnuUnitCommandItem(JumpCmdID).Caption = "�W�����v"
                    End If
                    
                    If LLength(.FeatureData("�W�����v")) = 2 Then
                        If .EN >= CInt(LIndex(.FeatureData("�W�����v"), 2)) Then
                            MainForm.mnuUnitCommandItem(JumpCmdID).Visible = True
                        End If
                        '�ʏ�ړ����W�����v�̏ꍇ
                        If .Speed0 = 0 _
                            Or (.FeatureLevel("�W�����v") >= 0 _
                                And LIndex(.FeatureData("�W�����v"), 2) = "0") _
                        Then
                            MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
                        End If
                    Else
                        MainForm.mnuUnitCommandItem(JumpCmdID).Visible = True
                        If .FeatureLevel("�W�����v") >= 0 Then
                            MainForm.mnuUnitCommandItem(MoveCmdID).Visible = False
                        End If
                    End If
                    
                    If .IsConditionSatisfied("�ړ��s�\") Then
                        MainForm.mnuUnitCommandItem(JumpCmdID).Visible = False
                    End If
                End If
                
                '��b�R�}���h
                For i = 1 To 4
                    Set u = Nothing
                    Select Case i
                        Case 1
                            If .X > 1 Then
                                Set u = MapDataForUnit(.X - 1, .Y)
                            End If
                        Case 2
                            If .X < MapWidth Then
                                Set u = MapDataForUnit(.X + 1, .Y)
                            End If
                        Case 3
                            If .Y > 1 Then
                                Set u = MapDataForUnit(.X, .Y - 1)
                            End If
                        Case 4
                            If .Y < MapHeight Then
                                Set u = MapDataForUnit(.X, .Y + 1)
                            End If
                    End Select
                    
                    If Not u Is Nothing Then
                        If IsEventDefined("��b " & .MainPilot.ID _
                            & " " & u.MainPilot.ID) _
                        Then
                            MainForm.mnuUnitCommandItem(TalkCmdID).Visible = True
                            Exit For
                        End If
                    End If
                Next
                
                '�U���R�}���h
                MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "�U��"
                For i = 1 To .CountWeapon
                    If .IsWeaponUseful(i, "�ړ��O") Then
                        MainForm.mnuUnitCommandItem(AttackCmdID).Visible = True
                        Exit For
                    End If
                Next
                If .Area = "�n��" Then
                    MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
                End If
                If .IsConditionSatisfied("�U���s�\") Then
                    MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
                End If
                
                '�C���R�}���h
                If .IsFeatureAvailable("�C�����u") _
                    And .Area <> "�n��" _
                Then
                    For i = 1 To 4
                        Set u = Nothing
                        Select Case i
                            Case 1
                                If .X > 1 Then
                                    Set u = MapDataForUnit(.X - 1, .Y)
                                End If
                            Case 2
                                If .X < MapWidth Then
                                    Set u = MapDataForUnit(.X + 1, .Y)
                                End If
                            Case 3
                                If .Y > 1 Then
                                    Set u = MapDataForUnit(.X, .Y - 1)
                                End If
                            Case 4
                                If .Y < MapHeight Then
                                    Set u = MapDataForUnit(.X, .Y + 1)
                                End If
                        End Select
                        
                        If Not u Is Nothing Then
                            With u
                                If (.Party = "����" Or .Party = "�m�o�b") _
                                    And .HP < .MaxHP _
                                    And Not .IsConditionSatisfied("�]���r") _
                                Then
                                    MainForm.mnuUnitCommandItem(FixCmdID).Visible = True
                                    Exit For
                                End If
                            End With
                        End If
                    Next
                    
                    If Len(.FeatureData("�C�����u")) > 0 Then
                        MainForm.mnuUnitCommandItem(FixCmdID).Caption = _
                            LIndex(.FeatureData("�C�����u"), 1)
                        If IsNumeric(LIndex(.FeatureData("�C�����u"), 2)) Then
                            If .EN < CInt(LIndex(.FeatureData("�C�����u"), 2)) Then
                                MainForm.mnuUnitCommandItem(FixCmdID).Visible = False
                            End If
                        End If
                    Else
                        MainForm.mnuUnitCommandItem(FixCmdID).Caption = "�C�����u"
                    End If
                End If
                
                '�⋋�R�}���h
                If .IsFeatureAvailable("�⋋���u") _
                    And .Area <> "�n��" _
                Then
                    For i = 1 To 4
                        Set u = Nothing
                        Select Case i
                            Case 1
                                If .X > 1 Then
                                    Set u = MapDataForUnit(.X - 1, .Y)
                                End If
                            Case 2
                                If .X < MapWidth Then
                                    Set u = MapDataForUnit(.X + 1, .Y)
                                End If
                            Case 3
                                If .Y > 1 Then
                                    Set u = MapDataForUnit(.X, .Y - 1)
                                End If
                            Case 4
                                If .Y < MapHeight Then
                                    Set u = MapDataForUnit(.X, .Y + 1)
                                End If
                        End Select
                        
                        If Not u Is Nothing Then
                            With u
                                If .Party = "����" Or .Party = "�m�o�b" Then
                                    If .EN < .MaxEN _
                                        And Not .IsConditionSatisfied("�]���r") _
                                    Then
                                        MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
                                    Else
                                        For j = 1 To .CountWeapon
                                            If .Bullet(j) < .MaxBullet(j) Then
                                                MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
                                                Exit For
                                            End If
                                        Next
                                        For j = 1 To .CountAbility
                                            If .Stock(j) < .MaxStock(j) Then
                                                MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End With
                        End If
                    Next
                    
                    If Len(.FeatureData("�⋋���u")) > 0 Then
                        MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = _
                            LIndex(.FeatureData("�⋋���u"), 1)
                        If IsNumeric(LIndex(.FeatureData("�⋋���u"), 2)) Then
                            If .EN < CInt(LIndex(.FeatureData("�⋋���u"), 2)) _
                                Or .MainPilot.Morale < 100 _
                            Then
                                MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
                            End If
                        End If
                    Else
                        MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "�⋋���u"
                    End If
                End If
                
                '�A�r���e�B�R�}���h
                n = 0
                For i = 1 To .CountAbility
                    If Not .Ability(i).IsItem _
                        And .IsAbilityMastered(i) _
                    Then
                        n = n + 1
                        If .IsAbilityUseful(i, "�ړ��O") Then
                            MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = True
                        End If
                    End If
                Next
                If .Area = "�n��" Then
                    MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = False
                End If
                MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = _
                    Term("�A�r���e�B", SelectedUnit)
                If n = 1 Then
                    For i = 1 To .CountAbility
                        If Not .Ability(i).IsItem _
                            And .IsAbilityMastered(i) _
                        Then
                            MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = .AbilityNickname(i)
                            Exit For
                        End If
                    Next
                End If
                
                '�`���[�W�R�}���h
                If Not .IsConditionSatisfied("�`���[�W����") Then
                    For i = 1 To .CountWeapon
                        If .IsWeaponClassifiedAs(i, "�b") Then
                            If .IsWeaponAvailable(i, "�`���[�W") Then
                                MainForm.mnuUnitCommandItem(ChargeCmdID).Visible = True
                                Exit For
                            End If
                        End If
                    Next
                    For i = 1 To .CountAbility
                        If .IsAbilityClassifiedAs(i, "�b") Then
                            If .IsAbilityAvailable(i, "�`���[�W") Then
                                MainForm.mnuUnitCommandItem(ChargeCmdID).Visible = True
                                Exit For
                            End If
                        End If
                    Next
                End If
                
                '�X�y�V�����p���[�R�}���h
                MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Caption = _
                    Term("�X�y�V�����p���[", SelectedUnit)
                If .MainPilot.CountSpecialPower > 0 Then
                    MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = True
                Else
                    For i = 1 To .CountPilot
                        If .Pilot(i).CountSpecialPower > 0 Then
                            MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = True
                            Exit For
                        End If
                    Next
                    For i = 1 To .CountSupport
                        If .Support(i).CountSpecialPower > 0 Then
                            MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = True
                            Exit For
                        End If
                    Next
                    If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
                        If .AdditionalSupport.CountSpecialPower > 0 Then
                            MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = True
                        End If
                    End If
                End If
                If .IsConditionSatisfied("�߈�") _
                    Or .IsConditionSatisfied("�X�y�V�����p���[�g�p�s�\") _
                Then
                    MainForm.mnuUnitCommandItem(SpecialPowerCmdID).Visible = False
                End If
                
                '�ό`�R�}���h
                If .IsFeatureAvailable("�ό`") _
                    And .FeatureName("�ό`") <> "" _
                    And Not .IsConditionSatisfied("�`�ԌŒ�") _
                    And Not .IsConditionSatisfied("�@�̌Œ�") _
                Then
                    MainForm.mnuUnitCommandItem(TransformCmdID).Caption = _
                        .FeatureName("�ό`")
                    
                    For i = 2 To LLength(.FeatureData("�ό`"))
                        uname = LIndex(.FeatureData("�ό`"), i)
                        If .OtherForm(uname).IsAvailable Then
                            MainForm.mnuUnitCommandItem(TransformCmdID).Visible = True
                            Exit For
                        End If
                    Next
                End If
                
                '�����R�}���h
                If .IsFeatureAvailable("����") _
                    And .FeatureName("����") <> "" _
                    And Not .IsConditionSatisfied("�`�ԌŒ�") _
                    And Not .IsConditionSatisfied("�@�̌Œ�") _
                Then
                    MainForm.mnuUnitCommandItem(SplitCmdID).Visible = True
                    MainForm.mnuUnitCommandItem(SplitCmdID).Caption = .FeatureName("����")
                    
                    buf = .FeatureData("����")
                    
                    '�����`�Ԃ����p�o���Ȃ��ꍇ�͕������s��Ȃ�
                    For i = 2 To LLength(buf)
                        If Not UList.IsDefined(LIndex(buf, i)) Then
                            MainForm.mnuUnitCommandItem(SplitCmdID).Visible = False
                            Exit For
                        End If
                    Next
                    
                    '�p�C���b�g������Ȃ��ꍇ���������s��Ȃ�
                    If MainForm.mnuUnitCommandItem(SplitCmdID).Visible Then
                        n = 0
                        For i = 2 To LLength(buf)
                            With UList.Item(LIndex(buf, i)).Data
                                If Not .IsFeatureAvailable("�������j�b�g") Then
                                    n = n + Abs(.PilotNum)
                                End If
                            End With
                        Next
                        If .CountPilot < n Then
                            MainForm.mnuUnitCommandItem(SplitCmdID).Visible = False
                        End If
                    End If
                End If
                If .IsFeatureAvailable("�p�[�c����") _
                    And .FeatureName("�p�[�c����") <> "" _
                Then
                    MainForm.mnuUnitCommandItem(SplitCmdID).Caption = _
                        .FeatureName("�p�[�c����")
                    MainForm.mnuUnitCommandItem(SplitCmdID).Visible = True
                End If
                
                '���̃R�}���h
                If .IsFeatureAvailable("����") _
                    And Not .IsConditionSatisfied("�`�ԌŒ�") _
                    And Not .IsConditionSatisfied("�@�̌Œ�") _
                Then
                    For i = 1 To .CountFeature
                        '3�̈ȏォ��Ȃ鍇�̔\�͂������Ă��邩�H
                        If .Feature(i) = "����" _
                            And .FeatureName(i) <> "" _
                            And LLength(.FeatureData(i)) > 3 _
                        Then
                            n = 0
                            '�p�[�g�i�[�͗אڂ��Ă��邩�H
                            For j = 3 To LLength(.FeatureData(i))
                                Set u = UList.Item(LIndex(.FeatureData(i), j))
                                If u Is Nothing Then
                                    Exit For
                                End If
                                If Not u.IsOperational Then
                                    Exit For
                                End If
                                If u.Status <> "�o��" _
                                    And u.CurrentForm.IsFeatureAvailable("���̐���") _
                                Then
                                    Exit For
                                End If
                                If Abs(.X - u.CurrentForm.X) + Abs(.Y - u.CurrentForm.Y) > 2 Then
                                    Exit For
                                End If
                                n = n + 1
                            Next
                            
                            '���̐�̃��j�b�g���쐬����A�����̉\�ȏ�Ԃɂ��邩�H
                            uname = LIndex(.FeatureData(i), 2)
                            Set u = UList.Item(uname)
                            If u Is Nothing Then
                                n = 0
                            ElseIf u.IsConditionSatisfied("�s���s�\") Then
                                n = 0
                            End If
                            
                            '���ׂĂ̏����𖞂����Ă���ꍇ
                            If n = LLength(.FeatureData(i)) - 2 Then
                                MainForm.mnuUnitCommandItem(CombineCmdID).Visible = True
                                MainForm.mnuUnitCommandItem(CombineCmdID).Caption = _
                                    LIndex(.FeatureData(i), 1)
                                Exit For
                            End If
                        End If
                    Next
                End If
                
                If Not .IsConditionSatisfied("�m�[�}�����[�h�t��") Then
                    '�n�C�p�[���[�h�R�}���h
                    If .IsFeatureAvailable("�n�C�p�[���[�h") _
                        And (.MainPilot.Morale >= CInt(10# * .FeatureLevel("�n�C�p�[���[�h")) + 100 _
                            Or (.HP <= .MaxHP \ 4 _
                                And InStr(.FeatureData("�n�C�p�[���[�h"), "�C�͔���") = 0)) _
                        And InStr(.FeatureData("�n�C�p�[���[�h"), "��������") = 0 _
                        And .FeatureName("�n�C�p�[���[�h") <> "" _
                        And Not .IsConditionSatisfied("�`�ԌŒ�") _
                        And Not .IsConditionSatisfied("�@�̌Œ�") _
                    Then
                        uname = LIndex(.FeatureData("�n�C�p�[���[�h"), 2)
                        If Not .OtherForm(uname).IsConditionSatisfied("�s���s�\") _
                            And .OtherForm(uname).IsAbleToEnter(.X, .Y) _
                        Then
                            MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
                            MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = _
                                LIndex(.FeatureData("�n�C�p�[���[�h"), 1)
                        End If
                    End If
                Else
                    '�ϐg����
                    If InStr(.FeatureData("�m�[�}�����[�h"), "�蓮����") > 0 Then
                        MainForm.mnuUnitCommandItem(HyperModeCmdID).Visible = True
                        If .IsFeatureAvailable("�ϐg�����R�}���h��") Then
                            MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = _
                                .FeatureData("�ϐg�����R�}���h��")
                        ElseIf .IsHero Then
                            MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "�ϐg����"
                        Else
                            MainForm.mnuUnitCommandItem(HyperModeCmdID).Caption = "���ꃂ�[�h����"
                        End If
                    End If
                End If
                
                '�n��R�}���h
                If TerrainClass(.X, .Y) = "��" _
                    Or TerrainClass(.X, .Y) = "����" _
                    Or TerrainClass(.X, .Y) = "����" _
                Then
                    If .Area <> "�n��" _
                        And .IsTransAvailable("��") _
                    Then
                        MainForm.mnuUnitCommandItem(GroundCmdID).Caption = "�n��"
                        MainForm.mnuUnitCommandItem(GroundCmdID).Visible = True
                    End If
                ElseIf TerrainClass(.X, .Y) = "��" _
                    Or TerrainClass(.X, .Y) = "�[��" _
                Then
                    If .Area <> "����" _
                        And .IsTransAvailable("����") _
                    Then
                        MainForm.mnuUnitCommandItem(GroundCmdID).Caption = "����"
                        MainForm.mnuUnitCommandItem(GroundCmdID).Visible = True
                    End If
                End If
                
                '�󒆃R�}���h
                Select Case TerrainClass(.X, .Y)
                    Case "�F��"
                    Case "����"
                        If (.IsTransAvailable("��") Or .IsTransAvailable("�F��")) _
                            And Not .Area = "�F��" _
                        Then
                            MainForm.mnuUnitCommandItem(SkyCmdID).Caption = "�F��"
                            MainForm.mnuUnitCommandItem(SkyCmdID).Visible = True
                        End If
                    Case Else
                        If .IsTransAvailable("��") And Not .Area = "��" Then
                            MainForm.mnuUnitCommandItem(SkyCmdID).Caption = "��"
                            MainForm.mnuUnitCommandItem(SkyCmdID).Visible = True
                        End If
                End Select
                
                '�n���R�}���h
                If .IsTransAvailable("�n��") And Not .Area = "�n��" _
                    And (TerrainClass(.X, .Y) = "��" _
                    Or TerrainClass(.X, .Y) = "����") _
                Then
                    MainForm.mnuUnitCommandItem(UndergroundCmdID).Visible = True
                End If
                
                '�����R�}���h
                If .Area <> "����" Then
                    If TerrainClass(.X, .Y) = "�[��" _
                        And (.IsTransAvailable("��") Or .IsFeatureAvailable("���j")) _
                        And Mid$(.Data.Adaption, 3, 1) <> "-" _
                    Then
                        MainForm.mnuUnitCommandItem(WaterCmdID).Visible = True
                    ElseIf TerrainClass(.X, .Y) = "��" _
                        And Mid$(.Data.Adaption, 3, 1) <> "-" _
                    Then
                        MainForm.mnuUnitCommandItem(WaterCmdID).Visible = True
                    End If
                End If
                
                '���i�R�}���h
                If .IsFeatureAvailable("���") And .Area <> "�n��" Then
                    If .CountUnitOnBoard > 0 Then
                        MainForm.mnuUnitCommandItem(LaunchCmdID).Visible = True
                    End If
                End If
                
                '�A�C�e���R�}���h
                For i = 1 To .CountAbility
                    If .IsAbilityUseful(i, "�ړ��O") _
                        And .Ability(i).IsItem _
                    Then
                        MainForm.mnuUnitCommandItem(ItemCmdID).Visible = True
                        Exit For
                    End If
                Next
                If .Area = "�n��" Then
                    MainForm.mnuUnitCommandItem(ItemCmdID).Visible = False
                End If
                
                '���������R�}���h
                For i = 1 To .CountServant
                    With .Servant(i).CurrentForm
                        Select Case .Status
                            Case "�o��", "�i�["
                                MainForm.mnuUnitCommandItem(DismissCmdID).Visible = True
                            Case "����`��", "���`��"
                                '���̌�̌`�Ԃ��o�����Ȃ�g�p�s��
                                MainForm.mnuUnitCommandItem(DismissCmdID).Visible = True
                                For j = 1 To .CountFeature
                                    If .Feature(j) = "����" Then
                                        uname = LIndex(.FeatureData(j), 2)
                                        If UList.IsDefined(uname) Then
                                             With UList.Item(uname).CurrentForm
                                                 If .Status = "�o��" _
                                                     Or .Status = "�i�[" _
                                                 Then
                                                     MainForm.mnuUnitCommandItem(DismissCmdID).Visible = False
                                                 End If
                                             End With
                                        End If
                                    End If
                                Next
                        End Select
                    End With
                Next
                If .IsFeatureAvailable("���������R�}���h��") Then
                    MainForm.mnuUnitCommandItem(DismissCmdID).Caption = _
                        .FeatureData("���������R�}���h��")
                Else
                    MainForm.mnuUnitCommandItem(DismissCmdID).Caption = "��������"
                End If
                
                '���j�b�g�R�}���h
                i = UnitCommand1CmdID
                For Each lab In colEventLabelList
                    With lab
                        If .Name = UnitCommandEventLabel Then
                            If .Enable Then
                                buf = .Para(3)
                                If (SelectedUnit.Party = "����" _
                                    And (buf = SelectedUnit.MainPilot.Name _
                                        Or buf = SelectedUnit.MainPilot.Nickname _
                                        Or buf = SelectedUnit.Name)) _
                                    Or buf = SelectedUnit.Party _
                                    Or buf = "�S" _
                                Then
                                    If .CountPara <= 3 Then
                                        MainForm.mnuUnitCommandItem(i).Visible = True
                                    ElseIf StrToLng(.Para(4)) <> 0 Then
                                        MainForm.mnuUnitCommandItem(i).Visible = True
                                    End If
                                End If
                            End If
                            
                            If MainForm.mnuUnitCommandItem(i).Visible Then
                                MainForm.mnuUnitCommandItem(i).Caption = .Para(2)
                                UnitCommandLabelList(i - UnitCommand1CmdID + 1) = .LineNum
                                i = i + 1
                                If i > UnitCommand10CmdID Then
                                    Exit For
                                End If
                            End If
                        End If
                    End With
                Next
            End With
            
            If Not SelectedUnit Is DisplayedUnit Then
'MOD START 240a
'                DisplayUnitStatus SelectedUnit
                '�V�f�t�h�g�p���̓N���b�N���Ƀ��j�b�g�X�e�[�^�X��\�����Ȃ�
                If Not NewGUIMode Then
                    DisplayUnitStatus SelectedUnit
                End If
'MOD  END  240a
            End If
            
            IsGUILocked = False
            If by_cancel Then
                MainForm.PopupMenu MainForm.mnuUnitCommand, 6, MouseX, MouseY + 5
            Else
                MainForm.PopupMenu MainForm.mnuUnitCommand, 6, MouseX, MouseY - 6
            End If
        
        Case "�ړ���R�}���h�I��"
            Set SelectedUnitForEvent = SelectedUnit
            
            With SelectedUnit
                '�ړ����ɂd�m������Ă���ꍇ�̓X�e�[�^�X�E�B���h�E���X�V
' MOD START MARGE
'                If MainWidth = 15 Then
                If Not NewGUIMode Then
' MOD END MARGE
                    If PrevUnitEN <> .EN Then
                        DisplayUnitStatus SelectedUnit
                    End If
                End If
                
                With MainForm
                    .mnuUnitCommandItem(WaitCmdID).Visible = True
                    .mnuUnitCommandItem(MoveCmdID).Visible = False
                    .mnuUnitCommandItem(TeleportCmdID).Visible = False
                    .mnuUnitCommandItem(JumpCmdID).Visible = False
                End With
                
                '��b�R�}���h
                MainForm.mnuUnitCommandItem(TalkCmdID).Visible = False
                For i = 1 To 4
                    Set u = Nothing
                    Select Case i
                        Case 1
                            If .X > 1 Then
                                Set u = MapDataForUnit(.X - 1, .Y)
                            End If
                        Case 2
                            If .X < MapWidth Then
                                Set u = MapDataForUnit(.X + 1, .Y)
                            End If
                        Case 3
                            If .Y > 1 Then
                                Set u = MapDataForUnit(.X, .Y - 1)
                            End If
                        Case 4
                            If .Y < MapHeight Then
                                Set u = MapDataForUnit(.X, .Y + 1)
                            End If
                    End Select
                    
                    If Not u Is Nothing Then
                        If IsEventDefined("��b " & .MainPilot.ID _
                            & " " & u.MainPilot.ID) _
                        Then
                            MainForm.mnuUnitCommandItem(TalkCmdID).Visible = True
                            Exit For
                        End If
                    End If
                Next
                
                '�U���R�}���h
                MainForm.mnuUnitCommandItem(AttackCmdID).Caption = "�U��"
                MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
                For i = 1 To .CountWeapon
                    If .IsWeaponUseful(i, "�ړ���") Then
                        MainForm.mnuUnitCommandItem(AttackCmdID).Visible = True
                        Exit For
                    End If
                Next
                If .Area = "�n��" Then
                    MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
                End If
                If .IsConditionSatisfied("�U���s�\") Then
                    MainForm.mnuUnitCommandItem(AttackCmdID).Visible = False
                End If
                
                '�C���R�}���h
                MainForm.mnuUnitCommandItem(FixCmdID).Visible = False
                If .IsFeatureAvailable("�C�����u") _
                    And .Area <> "�n��" _
                Then
                    For i = 1 To 4
                        Set u = Nothing
                        Select Case i
                            Case 1
                                If .X > 1 Then
                                    Set u = MapDataForUnit(.X - 1, .Y)
                                End If
                            Case 2
                                If .X < MapWidth Then
                                    Set u = MapDataForUnit(.X + 1, .Y)
                                End If
                            Case 3
                                If .Y > 1 Then
                                    Set u = MapDataForUnit(.X, .Y - 1)
                                End If
                            Case 4
                                If .Y < MapHeight Then
                                    Set u = MapDataForUnit(.X, .Y + 1)
                                End If
                        End Select
                        
                        If Not u Is Nothing Then
                            With u
                                If (.Party = "����" Or .Party = "�m�o�b") _
                                    And .HP < .MaxHP _
                                Then
                                    MainForm.mnuUnitCommandItem(FixCmdID).Visible = True
                                    Exit For
                                End If
                            End With
                        End If
                    Next
                    
                    If Len(.FeatureData("�C�����u")) > 0 Then
                        MainForm.mnuUnitCommandItem(FixCmdID).Caption = _
                            LIndex(.FeatureData("�C�����u"), 1)
                        If IsNumeric(LIndex(.FeatureData("�C�����u"), 2)) Then
                            If .EN < CInt(LIndex(.FeatureData("�C�����u"), 2)) Then
                                MainForm.mnuUnitCommandItem(FixCmdID).Visible = False
                            End If
                        End If
                    Else
                        MainForm.mnuUnitCommandItem(FixCmdID).Caption = "�C�����u"
                    End If
                End If
                
                '�⋋�R�}���h
                MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
                If .IsFeatureAvailable("�⋋���u") _
                    And .Area <> "�n��" _
                Then
                    For i = 1 To 4
                        Set u = Nothing
                        Select Case i
                            Case 1
                                If .X > 1 Then
                                    Set u = MapDataForUnit(.X - 1, .Y)
                                End If
                            Case 2
                                If .X < MapWidth Then
                                    Set u = MapDataForUnit(.X + 1, .Y)
                                End If
                            Case 3
                                If .Y > 1 Then
                                    Set u = MapDataForUnit(.X, .Y - 1)
                                End If
                            Case 4
                                If .Y < MapHeight Then
                                    Set u = MapDataForUnit(.X, .Y + 1)
                                End If
                        End Select
                        
                        If Not u Is Nothing Then
                            With u
                                If .Party = "����" Or .Party = "�m�o�b" Then
                                    If .EN < .MaxEN Then
                                        MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
                                    Else
                                        For j = 1 To .CountWeapon
                                            If .Bullet(j) < .MaxBullet(j) Then
                                                MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
                                                Exit For
                                            End If
                                        Next
                                        For j = 1 To .CountAbility
                                            If .Stock(j) < .MaxStock(j) Then
                                                MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = True
                                                Exit For
                                            End If
                                        Next
                                    End If
                                End If
                            End With
                        End If
                    Next
                    
                    If Len(.FeatureData("�⋋���u")) > 0 Then
                        MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = _
                            LIndex(.FeatureData("�⋋���u"), 1)
                        If IsNumeric(LIndex(.FeatureData("�⋋���u"), 2)) Then
                            If .EN < CInt(LIndex(.FeatureData("�⋋���u"), 2)) _
                                Or .MainPilot.Morale < 100 _
                            Then
                                MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
                            End If
                        End If
                    Else
                        MainForm.mnuUnitCommandItem(SupplyCmdID).Caption = "�⋋���u"
                    End If
                    
                    If IsOptionDefined("�ړ���⋋�s��") _
                        And Not SelectedUnit.MainPilot.IsSkillAvailable("�⋋") _
                    Then
                        MainForm.mnuUnitCommandItem(SupplyCmdID).Visible = False
                    End If
                End If
                
                '�A�r���e�B�R�}���h
                MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = False
                n = 0
                For i = 1 To .CountAbility
                    If Not .Ability(i).IsItem Then
                        n = n + 1
                        If .IsAbilityUseful(i, "�ړ���") Then
                            MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = True
                        End If
                    End If
                Next
                If .Area = "�n��" Then
                    MainForm.mnuUnitCommandItem(AbilityCmdID).Visible = False
                End If
                MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = _
                    Term("�A�r���e�B", SelectedUnit)
                If n = 1 Then
                    For i = 1 To .CountAbility
                        If Not .Ability(i).IsItem Then
                            MainForm.mnuUnitCommandItem(AbilityCmdID).Caption = .AbilityNickname(i)
                            Exit For
                        End If
                    Next
                End If
                
                With MainForm
                    .mnuUnitCommandItem(ChargeCmdID).Visible = False
                    .mnuUnitCommandItem(SpecialPowerCmdID).Visible = False
                    .mnuUnitCommandItem(TransformCmdID).Visible = False
                    .mnuUnitCommandItem(SplitCmdID).Visible = False
                End With
                
                '���̃R�}���h
                MainForm.mnuUnitCommandItem(CombineCmdID).Visible = False
                If .IsFeatureAvailable("����") _
                    And Not .IsConditionSatisfied("�`�ԌŒ�") _
                    And Not .IsConditionSatisfied("�@�̌Œ�") _
                Then
                    For i = 1 To .CountFeature
                        '3�̈ȏォ��Ȃ鍇�̔\�͂������Ă��邩�H
                        If .Feature(i) = "����" _
                            And .FeatureName(i) <> "" _
                            And LLength(.FeatureData(i)) > 3 _
                        Then
                            n = 0
                            For j = 3 To LLength(.FeatureData(i))
                                Set u = UList.Item(LIndex(.FeatureData(i), j))
                                If u Is Nothing Then
                                    Exit For
                                End If
                                If Not u.IsOperational Then
                                    Exit For
                                End If
                                If u.Status <> "�o��" _
                                    And u.CurrentForm.IsFeatureAvailable("���̐���") _
                                Then
                                    Exit For
                                End If
                                If Abs(.X - u.CurrentForm.X) + Abs(.Y - u.CurrentForm.Y) > 2 Then
                                    Exit For
                                End If
                                n = n + 1
                            Next
                            
                            uname = LIndex(.FeatureData(i), 2)
                            Set u = UList.Item(uname)
                            If u Is Nothing Then
                                n = 0
                            ElseIf u.IsConditionSatisfied("�s���s�\") Then
                                n = 0
                            End If
                            
                            If n = LLength(.FeatureData(i)) - 2 Then
                                MainForm.mnuUnitCommandItem(CombineCmdID).Visible = True
                                MainForm.mnuUnitCommandItem(CombineCmdID).Caption = _
                                    LIndex(.FeatureData(i), 1)
                                Exit For
                            End If
                        End If
                    Next
                End If
                
                With MainForm
                    .mnuUnitCommandItem(HyperModeCmdID).Visible = False
                    .mnuUnitCommandItem(GroundCmdID).Visible = False
                    .mnuUnitCommandItem(SkyCmdID).Visible = False
                    .mnuUnitCommandItem(UndergroundCmdID).Visible = False
                    .mnuUnitCommandItem(WaterCmdID).Visible = False
                    .mnuUnitCommandItem(LaunchCmdID).Visible = False
                End With
                
                '�A�C�e���R�}���h
                MainForm.mnuUnitCommandItem(ItemCmdID).Visible = False
                For i = 1 To .CountAbility
                    If .IsAbilityUseful(i, "�ړ���") _
                        And .Ability(i).IsItem _
                    Then
                        MainForm.mnuUnitCommandItem(ItemCmdID).Visible = True
                        Exit For
                    End If
                Next
                If .Area = "�n��" Then
                    MainForm.mnuUnitCommandItem(ItemCmdID).Visible = False
                End If
                
                With MainForm
                    .mnuUnitCommandItem(DismissCmdID).Visible = False
                    .mnuUnitCommandItem(OrderCmdID).Visible = False
                    .mnuUnitCommandItem(FeatureListCmdID).Visible = False
                    .mnuUnitCommandItem(WeaponListCmdID).Visible = False
                    .mnuUnitCommandItem(AbilityListCmdID).Visible = False
                    .mnuUnitCommandItem(UnitCommand1CmdID).Visible = False
                    .mnuUnitCommandItem(UnitCommand2CmdID).Visible = False
                    .mnuUnitCommandItem(UnitCommand3CmdID).Visible = False
                    .mnuUnitCommandItem(UnitCommand4CmdID).Visible = False
                    .mnuUnitCommandItem(UnitCommand5CmdID).Visible = False
                    .mnuUnitCommandItem(UnitCommand6CmdID).Visible = False
                    .mnuUnitCommandItem(UnitCommand7CmdID).Visible = False
                    .mnuUnitCommandItem(UnitCommand8CmdID).Visible = False
                    .mnuUnitCommandItem(UnitCommand9CmdID).Visible = False
                    .mnuUnitCommandItem(UnitCommand10CmdID).Visible = False
                End With
                
                '���j�b�g�R�}���h
                i = UnitCommand1CmdID
                For Each lab In colEventLabelList
                    With lab
                        If .Name = UnitCommandEventLabel _
                            And .AsterNum >= 2 _
                        Then
                            If .Enable Then
                                buf = .Para(3)
                                If (SelectedUnit.Party = "����" _
                                    And (buf = SelectedUnit.MainPilot.Name _
                                        Or buf = SelectedUnit.MainPilot.Nickname _
                                        Or buf = SelectedUnit.Name)) _
                                    Or buf = SelectedUnit.Party _
                                    Or buf = "�S" _
                                Then
                                    If .CountPara <= 3 Then
                                        MainForm.mnuUnitCommandItem(i).Visible = True
                                    ElseIf StrToLng(.Para(4)) <> 0 Then
                                        MainForm.mnuUnitCommandItem(i).Visible = True
                                    End If
                                End If
                            End If
                            
                            If MainForm.mnuUnitCommandItem(i).Visible Then
                                MainForm.mnuUnitCommandItem(i).Caption = .Para(2)
                                UnitCommandLabelList(i - UnitCommand1CmdID + 1) = .LineNum
                                i = i + 1
                                If i > UnitCommand10CmdID Then
                                    Exit For
                                End If
                            End If
                        End If
                    End With
                Next
            End With
            
            IsGUILocked = False
            If by_cancel Then
                MainForm.PopupMenu MainForm.mnuUnitCommand, 6, MouseX, MouseY + 5
            Else
                MainForm.PopupMenu MainForm.mnuUnitCommand, 6, MouseX, MouseY - 6
                DoEvents
                '�o�b�ɕ��ׂ����������悤�ȏ�Ԃ��ƃ|�b�v�A�b�v���j���[�̑I����
                '���܂��s���Ȃ��ꍇ������̂ł�蒼��
                Do While (CommandState = "�ړ���R�}���h�I��" _
                        And SelectedCommand = "�ړ�")
                    MainForm.PopupMenu MainForm.mnuUnitCommand, 6, MouseX, MouseY - 6
                    DoEvents
                Loop
            End If
            
        Case "�^�[�Q�b�g�I��", "�ړ���^�[�Q�b�g�I��"
            If Not MaskData(PixelToMapX(MouseX), PixelToMapY(MouseY)) Then
                SelectedX = PixelToMapX(MouseX)
                SelectedY = PixelToMapY(MouseY)
                
                '�������g��I�����ꂽ�ꍇ
                If SelectedUnit.X = SelectedX And SelectedUnit.Y = SelectedY Then
                    If SelectedCommand = "�X�y�V�����p���[" Then
                        '���ɔ�����
                    ElseIf SelectedCommand = "�A�r���e�B" _
                        Or SelectedCommand = "�}�b�v�A�r���e�B" _
                        Or SelectedCommand = "�A�C�e��" _
                        Or SelectedCommand = "�}�b�v�A�C�e��" _
                    Then
                        If SelectedUnit.AbilityMinRange(SelectedAbility) > 0 Then
                            '�������g�͑I��s��
                            IsGUILocked = False
                            Exit Sub
                        End If
                    ElseIf SelectedCommand = "�ړ�����" Then
                        '���ɔ�����
                    Else
                        '�������g�͑I��s��
                        IsGUILocked = False
                        Exit Sub
                    End If
                End If
                
                '�ꏊ��I������R�}���h
                Select Case SelectedCommand
' MOD START MARGE
'                    Case "�ړ�"
                    Case "�ړ�", "�Ĉړ�"
' MOD END MARGE
                        FinishMoveCommand
                        IsGUILocked = False
                        Exit Sub
                    Case "�e���|�[�g"
                        FinishTeleportCommand
                        IsGUILocked = False
                        Exit Sub
                    Case "�W�����v"
                        FinishJumpCommand
                        IsGUILocked = False
                        Exit Sub
                    Case "�}�b�v�U��"
                        MapAttackCommand
                        IsGUILocked = False
                        Exit Sub
                    Case "�}�b�v�A�r���e�B", "�}�b�v�A�C�e��"
                        MapAbilityCommand
                        IsGUILocked = False
                        Exit Sub
                    Case "���i"
                        FinishLaunchCommand
                        IsGUILocked = False
                        Exit Sub
                    Case "�ړ�����"
                        FinishOrderCommand
                        IsGUILocked = False
                        Exit Sub
                End Select
                
                '����ȍ~�̓��j�b�g��I������R�}���h
                
                '�w�肵���n�_�Ƀ��j�b�g������H
                If MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
                    IsGUILocked = False
                    Exit Sub
                End If
                
                '�^�[�Q�b�g��I��
                Set SelectedTarget = MapDataForUnit(SelectedX, SelectedY)
                
                Select Case SelectedCommand
                    Case "�U��"
                        FinishAttackCommand
                    Case "�A�r���e�B", "�A�C�e��"
                        FinishAbilityCommand
                    Case "��b"
                        FinishTalkCommand
                    Case "�C��"
                        FinishFixCommand
                    Case "�⋋"
                        FinishSupplyCommand
                    Case "�X�y�V�����p���["
                        FinishSpecialPowerCommand
                    Case "�U������", "��q����"
                        FinishOrderCommand
                End Select
            End If
            
        Case "�}�b�v�U���g�p", "�ړ���}�b�v�U���g�p"
            If 1 <= PixelToMapX(MouseX) And PixelToMapX(MouseX) <= MapWidth Then
                If 1 <= PixelToMapY(MouseY) And PixelToMapY(MouseY) <= MapHeight Then
                    If Not MaskData(PixelToMapX(MouseX), PixelToMapY(MouseY)) Then
                        '���ʔ͈͓��ŃN���b�N�����΃}�b�v�U������
                        If SelectedCommand = "�}�b�v�U��" Then
                            MapAttackCommand
                        Else
                            MapAbilityCommand
                        End If
                    End If
                End If
            End If
    End Select
    
    IsGUILocked = False
End Sub

'�f�t�h�̏������L�����Z��
Public Sub CancelCommand()
Dim tmp_x As Integer, tmp_y As Integer

    With SelectedUnit
        Select Case CommandState
            Case "���j�b�g�I��"
            Case "�R�}���h�I��"
                CommandState = "���j�b�g�I��"
'ADD START
                '�I�������R�}���h��������
                SelectedCommand = ""
' MOD START MARGE
'                If MainWidth <> 15 Then
                If NewGUIMode Then
' MOD  END  MARGE
                    ClearUnitStatus
                End If
                
            Case "�^�[�Q�b�g�I��"
' ADD START MARGE
                If SelectedCommand = "�Ĉړ�" Then
                    WaitCommand
                    Exit Sub
                End If
' ADD END MARGE
                CommandState = "�R�}���h�I��"
                DisplayUnitStatus SelectedUnit
                RedrawScreen
                ProceedCommand True
                
            Case "�ړ���R�}���h�I��"
                CommandState = "�^�[�Q�b�g�I��"
                .Area = PrevUnitArea
                .Move PrevUnitX, PrevUnitY, True, True
                .EN = PrevUnitEN
                If Not SelectedUnit Is MapDataForUnit(PrevUnitX, PrevUnitY) Then
                    '���i���L�����Z�������ꍇ
                    Set SelectedTarget = SelectedUnit
                    PaintUnitBitmap SelectedTarget
                    Set SelectedUnit = MapDataForUnit(PrevUnitX, PrevUnitY)
' MOD START MARGE
'                ElseIf MainWidth = 15 Then
                ElseIf Not NewGUIMode Then
' MOD END MARGE
                    DisplayUnitStatus SelectedUnit
                End If
' MOD START MARGE
                ' �ړ���R�}���h���L�����Z�������ꍇ�AMoveCost��0�Ƀ��Z�b�g����
                SelectedUnitMoveCost = 0
' MOD END MARGE
                Select Case SelectedCommand
                    Case "�ړ�"
                        StartMoveCommand
                    Case "�e���|�[�g"
                        StartTeleportCommand
                    Case "�W�����v"
                        StartJumpCommand
                    Case "���i"
                        PaintUnitBitmap SelectedTarget
                End Select
                
            Case "�ړ���^�[�Q�b�g�I��"
                CommandState = "�ړ���R�}���h�I��"
                DisplayUnitStatus SelectedUnit
                
                tmp_x = .X
                tmp_y = .Y
                .X = PrevUnitX
                .Y = PrevUnitY
                Select Case PrevCommand
                    Case "�ړ�"
                        AreaInSpeed SelectedUnit
                    Case "�e���|�[�g"
                        AreaInTeleport SelectedUnit
                    Case "�W�����v"
                        AreaInSpeed SelectedUnit, True
                    Case "���i"
                        With SelectedTarget
                            If .IsFeatureAvailable("�e���|�[�g") _
                                And (.Data.Speed = 0 _
                                    Or LIndex(.FeatureData("�e���|�[�g"), 2) = "0") _
                            Then
                                AreaInTeleport SelectedTarget
                            ElseIf .IsFeatureAvailable("�W�����v") _
                                And (.Data.Speed = 0 _
                                    Or LLength(.FeatureData("�W�����v")) < 2 _
                                    Or LIndex(.FeatureData("�W�����v"), 2) = "0") _
                            Then
                                AreaInSpeed SelectedTarget, True
                            Else
                                AreaInSpeed SelectedTarget
                            End If
                        End With
                End Select
                .X = tmp_x
                .Y = tmp_y
                SelectedCommand = PrevCommand
                
                MaskData(tmp_x, tmp_y) = False
                MaskScreen
                ProceedCommand True
                
            Case "�}�b�v�U���g�p", "�ړ���}�b�v�U���g�p"
                If CommandState = "�}�b�v�U���g�p" Then
                    CommandState = "�^�[�Q�b�g�I��"
                Else
                    CommandState = "�ړ���^�[�Q�b�g�I��"
                End If
                If SelectedCommand = "�}�b�v�U��" Then
                    If .IsWeaponClassifiedAs(SelectedWeapon, "�l��") Then
                        AreaInCross .X, .Y, _
                            .WeaponMaxRange(SelectedWeapon), _
                            .Weapon(SelectedWeapon).MinRange
                    ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "�l��") Then
                        AreaInMoveAction SelectedUnit, .WeaponMaxRange(SelectedWeapon)
                    Else
                        AreaInRange .X, .Y, _
                            .WeaponMaxRange(SelectedWeapon), _
                            .Weapon(SelectedWeapon).MinRange, _
                            "���ׂ�"
                    End If
                Else
                    If .IsAbilityClassifiedAs(SelectedAbility, "�l��") Then
                        AreaInCross .X, .Y, _
                            .AbilityMaxRange(SelectedAbility), _
                            .AbilityMinRange(SelectedAbility)
                    ElseIf .IsAbilityClassifiedAs(SelectedAbility, "�l��") Then
                        AreaInMoveAction SelectedUnit, .AbilityMaxRange(SelectedAbility)
                    Else
                        AreaInRange .X, .Y, _
                            .AbilityMaxRange(SelectedAbility), _
                            .AbilityMinRange(SelectedAbility), _
                            "���ׂ�"
                    End If
                End If
                MaskScreen
        End Select
    End With
End Sub


' ���j�b�g�R�}���h�����s
Public Sub UnitCommand(ByVal idx As Integer)
Dim prev_used_action As Integer
    
    PrevCommand = SelectedCommand
    
    With SelectedUnit
        prev_used_action = .UsedAction
        Select Case idx
            Case MoveCmdID '�ړ�
                '�Ȃ�炩�̌����ɂ��A���j�b�g�R�}���h�̑I�������܂������Ȃ������ꍇ��
                '�ړ���̃R�}���h�I������蒼��
                If CommandState = "�ړ���R�}���h�I��" Then
                    DoEvents
                    Exit Sub
                End If
                
                If MainForm.mnuUnitCommandItem.Item(MoveCmdID).Caption = "�ړ�" Then
                    StartMoveCommand
                Else
                    ShowAreaInSpeedCommand
                End If
                
            Case TeleportCmdID '�e���|�[�g
                StartTeleportCommand
                
            Case JumpCmdID '�W�����v
                StartJumpCommand
                
            Case TalkCmdID '��b
                StartTalkCommand
                
            Case AttackCmdID '�U��
                If MainForm.mnuUnitCommandItem.Item(AttackCmdID).Caption = "�U��" Then
                    StartAttackCommand
                Else
                    ShowAreaInRangeCommand
                End If
                
            Case FixCmdID '�C��
                StartFixCommand
                
            Case SupplyCmdID '�⋋
                StartSupplyCommand
                
            Case AbilityCmdID '�A�r���e�B
                StartAbilityCommand
                
            Case ChargeCmdID '�`���[�W
                ChargeCommand
                
            Case SpecialPowerCmdID '���_
                StartSpecialPowerCommand
                
            Case TransformCmdID '�ό`
                TransformCommand
                
            Case SplitCmdID '����
                SplitCommand
                
            Case CombineCmdID '����
                CombineCommand
                
            Case HyperModeCmdID '�n�C�p�[���[�h�E�ϐg����
                If InStr(.FeatureData("�m�[�}�����[�h"), "�蓮����") > 0 Then
                    CancelTransformationCommand
                Else
                    HyperModeCommand
                End If
                
            Case GroundCmdID '�n��
                LockGUI
                If TerrainClass(.X, .Y) = "��" _
                    Or TerrainClass(.X, .Y) = "�[��" _
                Then
                    .Area = "����"
                Else
                    .Area = "�n��"
                End If
                .Update
                If .IsMessageDefined(.Area) Then
                    OpenMessageForm
                    .PilotMessage .Area
                    CloseMessageForm
                End If
                PaintUnitBitmap SelectedUnit
                CommandState = "���j�b�g�I��"
                UnlockGUI
                
            Case SkyCmdID '��
                LockGUI
                If TerrainClass(.X, .Y) = "����" Then
                    .Area = "�F��"
                Else
                    .Area = "��"
                End If
                .Update
                If .IsMessageDefined(.Area) Then
                    OpenMessageForm
                    .PilotMessage .Area
                    CloseMessageForm
                End If
                PaintUnitBitmap SelectedUnit
                CommandState = "���j�b�g�I��"
                UnlockGUI
                
            Case UndergroundCmdID '�n��
                LockGUI
                .Area = "�n��"
                .Update
                If .IsMessageDefined(.Area) Then
                    OpenMessageForm
                    .PilotMessage .Area
                    CloseMessageForm
                End If
                PaintUnitBitmap SelectedUnit
                CommandState = "���j�b�g�I��"
                UnlockGUI
                
            Case WaterCmdID '����
                LockGUI
                .Area = "����"
                .Update
                If .IsMessageDefined(.Area) Then
                    OpenMessageForm
                    .PilotMessage .Area
                    CloseMessageForm
                End If
                PaintUnitBitmap SelectedUnit
                CommandState = "���j�b�g�I��"
                UnlockGUI
                
            Case LaunchCmdID '���i
                StartLaunchCommand
                
            Case ItemCmdID '�A�C�e��
                StartAbilityCommand True
                
            Case DismissCmdID '��������
                LockGUI
                
                '���������̎g�p�C�x���g
                HandleEvent "�g�p", .MainPilot.ID, "��������"
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                    UnlockGUI
                    Exit Sub
                End If
                If IsCanceled Then
                    IsCanceled = False
                    Exit Sub
                End If
                
                '�������j�b�g�����
                .DismissServant
                
                '���������̎g�p��C�x���g
                HandleEvent "�g�p��", .CurrentForm.MainPilot.ID, "��������"
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                End If
                If IsCanceled Then
                    IsCanceled = False
                End If
                
                UnlockGUI
                
            Case OrderCmdID '����/����
                If MainForm.mnuUnitCommandItem.Item(OrderCmdID).Caption = "����" Then
                    StartOrderCommand
                Else
                    ExchangeFormCommand
                End If
                
            Case FeatureListCmdID '����\�͈ꗗ
                FeatureListCommand
                
            Case WeaponListCmdID '����ꗗ
                WeaponListCommand
                
            Case AbilityListCmdID '�A�r���e�B�ꗗ
                AbilityListCommand
                
            Case UnitCommand1CmdID To UnitCommand10CmdID '���j�b�g�R�}���h
                LockGUI
                
                '���j�b�g�R�}���h�̎g�p�C�x���g
                HandleEvent "�g�p", .MainPilot.ID, _
                    MainForm.mnuUnitCommandItem.Item(idx).Caption
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                    UnlockGUI
                    Exit Sub
                End If
                If IsCanceled Then
                    IsCanceled = False
                    WaitCommand
                    Exit Sub
                End If
                
                '���j�b�g�R�}���h�����s
                HandleEvent UnitCommandLabelList(idx - UnitCommand1CmdID + 1)
                
                If IsCanceled Then
                    IsCanceled = False
                    CancelCommand
                    UnlockGUI
                    Exit Sub
                End If
                
                '���j�b�g�R�}���h�̎g�p��C�x���g
                If .CurrentForm.CountPilot > 0 Then
                    HandleEvent "�g�p��", .CurrentForm.MainPilot.ID, _
                        MainForm.mnuUnitCommandItem.Item(idx).Caption
                    If IsScenarioFinished Then
                        IsScenarioFinished = False
                        UnlockGUI
                        Exit Sub
                    End If
                End If
                
                '�X�e�[�^�X�E�B���h�E���X�V
                If .CurrentForm.CountPilot > 0 Then
                    DisplayUnitStatus .CurrentForm
                End If
                
                '�s���I��
                If .CurrentForm.UsedAction <= prev_used_action Then
                    If CommandState = "�ړ���R�}���h�I��" Then
                        WaitCommand
                    Else
                        CommandState = "���j�b�g�I��"
                        UnlockGUI
                    End If
                ElseIf IsCanceled Then
                    IsCanceled = False
                Else
                    WaitCommand True
                End If
                
            Case WaitCmdID '�ҋ@
                WaitCommand
                
            Case Else
                '�Ȃ�炩�̌����ɂ��A���j�b�g�R�}���h�̑I�������܂������Ȃ������ꍇ��
                '�ړ���̃R�}���h�I������蒼��
                If CommandState = "�ړ���R�}���h�I��" Then
                    DoEvents
                    Exit Sub
                End If
        End Select
    End With
End Sub


'�u�ړ��v�R�}���h���J�n
' MOD START MARGE
'Public Sub StartMoveCommand()
Private Sub StartMoveCommand()
' MOD END MARGE
    SelectedCommand = "�ړ�"
    AreaInSpeed SelectedUnit
    If Not IsOptionDefined("��^�}�b�v") Then
        Center SelectedUnit.X, SelectedUnit.Y
    End If
    MaskScreen
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        DoEvents
        ClearUnitStatus
    End If
    CommandState = "�^�[�Q�b�g�I��"
End Sub

'�u�ړ��v�R�}���h���I��
' MOD START MARGE
'Public Sub FinishMoveCommand()
Private Sub FinishMoveCommand()
' MOD END MARGE
Dim ret As Integer
    
    LockGUI
    
    With SelectedUnit
        PrevUnitX = .X
        PrevUnitY = .Y
        PrevUnitArea = .Area
        PrevUnitEN = .EN
        
        '�ړ���ɒ���or���̂���ꍇ�̓v���C���[�Ɋm�F�����
        If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
            If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("���") _
                And Not .IsFeatureAvailable("���") _
            Then
                ret = MsgBox("���͂��܂����H", _
                    vbOKCancel + vbQuestion, "����")
            Else
                ret = MsgBox("���̂��܂����H", _
                    vbOKCancel + vbQuestion, "����")
            End If
            If ret = vbCancel Then
                CancelCommand
                UnlockGUI
                Exit Sub
            End If
        End If
        
        '���j�b�g���ړ�
        .Move SelectedX, SelectedY
        
        '�ړ���ɒ��͂܂��͍��̂����H
        If Not MapDataForUnit(.X, .Y) Is SelectedUnit Then
            If MapDataForUnit(.X, .Y).IsFeatureAvailable("���") _
                And Not .IsFeatureAvailable("���") _
                And .CountPilot > 0 _
            Then
                '���̓��b�Z�[�W�\��
                If .IsMessageDefined("����(" & .Name & ")") Then
                    OpenMessageForm
                    .PilotMessage "����(" & .Name & ")"
                    CloseMessageForm
                ElseIf .IsMessageDefined("����") Then
                    OpenMessageForm
                    .PilotMessage "����"
                    CloseMessageForm
                End If
                .SpecialEffect "����", .Name
                
                '���[�C�x���g
                Set SelectedTarget = MapDataForUnit(.X, .Y)
                HandleEvent "���[", .MainPilot.ID
            Else
                '���̌�̃��j�b�g��I��
                Set SelectedUnit = MapDataForUnit(.X, .Y)
                
                '���̃C�x���g
                HandleEvent "����", SelectedUnit.MainPilot.ID, SelectedUnit.Name
            End If
            
            '�ړ���̎��[�E���̃C�x���g�ŃX�e�[�W���I�����邱�Ƃ�����̂�
            If IsScenarioFinished Then
                IsScenarioFinished = False
                UnlockGUI
                Exit Sub
            End If
            If IsCanceled Then
                IsCanceled = False
                ClearUnitStatus
                RedrawScreen
                CommandState = "���j�b�g�I��"
                UnlockGUI
                Exit Sub
            End If
            
            '�c��s����������������
            SelectedUnit.UseAction
            
            '�������Ԃ��u�ړ��v�̃X�y�V�����p���[���ʂ��폜
            SelectedUnit.RemoveSpecialPowerInEffect "�ړ�"
            
            DisplayUnitStatus SelectedUnit
            RedrawScreen
            CommandState = "���j�b�g�I��"
            UnlockGUI
            Exit Sub
        End If
' ADD START MARGE
        If SelectedUnitMoveCost > 0 Then
            '�s���������炷
            WaitCommand
            Exit Sub
        End If
        
        SelectedUnitMoveCost = TotalMoveCost(.X, .Y)
' ADD END MARGE
    End With
    
    CommandState = "�ړ���R�}���h�I��"
    UnlockGUI
    ProceedCommand
End Sub


'�u�e���|�[�g�v�R�}���h���J�n
' MOD START MARGE
'Public Sub StartTeleportCommand()
Private Sub StartTeleportCommand()
' MOD END MARGE
    SelectedCommand = "�e���|�[�g"
    AreaInTeleport SelectedUnit
    If Not IsOptionDefined("��^�}�b�v") Then
        Center SelectedUnit.X, SelectedUnit.Y
    End If
    MaskScreen
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        DoEvents
        ClearUnitStatus
    End If
    CommandState = "�^�[�Q�b�g�I��"
End Sub

'�u�e���|�[�g�v�R�}���h���I��
' MOD START MARGE
'Public Sub FinishTeleportCommand()
Private Sub FinishTeleportCommand()
' MOD END MARGE
Dim ret As Integer

    LockGUI
    
    With SelectedUnit
        PrevUnitX = .X
        PrevUnitY = .Y
        PrevUnitArea = .Area
        PrevUnitEN = .EN
        
        '�e���|�[�g��ɒ���or���̂���ꍇ�̓v���C���[�Ɋm�F�����
        If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
            If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("���") Then
                ret = MsgBox("���͂��܂����H", _
                    vbOKCancel + vbQuestion, "����")
            Else
                ret = MsgBox("���̂��܂����H", _
                    vbOKCancel + vbQuestion, "����")
            End If
            If ret = vbCancel Then
                CancelCommand
                UnlockGUI
                Exit Sub
            End If
        End If
        
        '���b�Z�[�W��\��
        If .IsMessageDefined("�e���|�[�g(" & .FeatureName("�e���|�[�g") & ")") Then
            OpenMessageForm
            .PilotMessage "�e���|�[�g(" & .FeatureName("�e���|�[�g") & ")"
            CloseMessageForm
        ElseIf .IsMessageDefined("�e���|�[�g") Then
            OpenMessageForm
            .PilotMessage "�e���|�[�g"
            CloseMessageForm
        End If
        
        '�A�j���\��
        If .IsAnimationDefined("�e���|�[�g", .FeatureName("�e���|�[�g")) Then
            .PlayAnimation "�e���|�[�g", .FeatureName("�e���|�[�g")
        ElseIf .IsSpecialEffectDefined("�e���|�[�g", .FeatureName("�e���|�[�g")) Then
            .SpecialEffect "�e���|�[�g", .FeatureName("�e���|�[�g")
        ElseIf BattleAnimation Then
            ShowAnimation "�e���|�[�g���� Whiz.wav " & .FeatureName0("�e���|�[�g")
        Else
            PlayWave "Whiz.wav"
        End If
        
        '�d�m������
        If LLength(.FeatureData("�e���|�[�g")) = 2 Then
            .EN = PrevUnitEN - CInt(LIndex(.FeatureData("�e���|�[�g"), 2))
        Else
            .EN = PrevUnitEN - 40
        End If
        
        '���j�b�g���ړ�
        .Move SelectedX, SelectedY, True, False, True
        RedrawScreen
        
        '�ړ���ɒ��͂܂��͍��̂����H
        If Not MapDataForUnit(SelectedX, SelectedY) Is SelectedUnit Then
            If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("���") _
                And Not .IsFeatureAvailable("���") _
                And .CountPilot > 0 _
            Then
                '���̓��b�Z�[�W�\��
                If .IsMessageDefined("����(" & .Name & ")") Then
                    OpenMessageForm
                    .PilotMessage "����(" & .Name & ")"
                    CloseMessageForm
                ElseIf .IsMessageDefined("����") Then
                    OpenMessageForm
                    .PilotMessage "����"
                    CloseMessageForm
                End If
                .SpecialEffect "����", .Name
                
                '���[�C�x���g
                Set SelectedTarget = MapDataForUnit(SelectedX, SelectedY)
                HandleEvent "���[", .MainPilot.ID
            Else
                '���̌�̃��j�b�g��I��
                Set SelectedUnit = MapDataForUnit(SelectedX, SelectedY)
                
                '���̃C�x���g
                HandleEvent "����", SelectedUnit.MainPilot.ID, SelectedUnit.Name
            End If
            
            '�ړ���̎��[�E���̃C�x���g�ŃX�e�[�W���I�����邱�Ƃ�����̂�
            If IsScenarioFinished Then
                IsScenarioFinished = False
                UnlockGUI
                Exit Sub
            End If
            If IsCanceled Then
                IsCanceled = False
                ClearUnitStatus
                RedrawScreen
                CommandState = "���j�b�g�I��"
                UnlockGUI
                Exit Sub
            End If
            
            '�c��s����������������
            SelectedUnit.UseAction
            
            '�������Ԃ��u�ړ��v�̃X�y�V�����p���[���ʂ��폜
            SelectedUnit.RemoveSpecialPowerInEffect "�ړ�"
            
            DisplayUnitStatus MapDataForUnit(SelectedX, SelectedY)
            RedrawScreen
            CommandState = "���j�b�g�I��"
            UnlockGUI
            Exit Sub
        End If
' ADD START MARGE
        SelectedUnitMoveCost = 100
' ADD END MARGE
    End With
    
    CommandState = "�ړ���R�}���h�I��"
    UnlockGUI
    ProceedCommand
End Sub


'�u�W�����v�v�R�}���h���J�n
' MOD START MARGE
'Public Sub StartJumpCommand()
Private Sub StartJumpCommand()
' MOD END MARGE
    SelectedCommand = "�W�����v"
    AreaInSpeed SelectedUnit, True
    If Not IsOptionDefined("��^�}�b�v") Then
        Center SelectedUnit.X, SelectedUnit.Y
    End If
    MaskScreen
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        DoEvents
        ClearUnitStatus
    End If
    CommandState = "�^�[�Q�b�g�I��"
End Sub

'�u�W�����v�v�R�}���h���I��
' MOD START MARGE
'Public Sub FinishJumpCommand()
Private Sub FinishJumpCommand()
' MOD END MARGE
Dim ret As Integer

    LockGUI
    
    With SelectedUnit
        PrevUnitX = .X
        PrevUnitY = .Y
        PrevUnitArea = .Area
        PrevUnitEN = .EN
        
        '�W�����v��ɒ���or���̂���ꍇ�̓v���C���[�Ɋm�F�����
        If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
            If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("���") Then
                ret = MsgBox("���͂��܂����H", _
                    vbOKCancel + vbQuestion, "����")
            Else
                ret = MsgBox("���̂��܂����H", _
                    vbOKCancel + vbQuestion, "����")
            End If
            If ret = vbCancel Then
                CancelCommand
                UnlockGUI
                Exit Sub
            End If
        End If
        
        '���b�Z�[�W��\��
        If .IsMessageDefined("�W�����v(" & .FeatureName("�W�����v") & ")") Then
            OpenMessageForm
            .PilotMessage "�W�����v(" & .FeatureName("�W�����v") & ")"
            CloseMessageForm
        ElseIf .IsMessageDefined("�W�����v") Then
            OpenMessageForm
            .PilotMessage "�W�����v"
            CloseMessageForm
        End If
        
        '�A�j���\��
        If .IsAnimationDefined("�W�����v", .FeatureName("�W�����v")) Then
            .PlayAnimation "�W�����v", .FeatureName("�W�����v")
        ElseIf .IsSpecialEffectDefined("�W�����v", .FeatureName("�W�����v")) Then
            .SpecialEffect "�W�����v", .FeatureName("�W�����v")
        Else
            PlayWave "Swing.wav"
        End If
        
        '�d�m������
        If LLength(.FeatureData("�W�����v")) = 2 Then
            .EN = PrevUnitEN - CInt(LIndex(.FeatureData("�W�����v"), 2))
        End If
        
        '���j�b�g���ړ�
        .Move SelectedX, SelectedY, True, False, True
        RedrawScreen
        
        '�ړ���ɒ��͂܂��͍��̂����H
        If Not MapDataForUnit(SelectedX, SelectedY) Is SelectedUnit Then
            If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("���") _
                And Not .IsFeatureAvailable("���") _
                And .CountPilot > 0 _
            Then
                '���̓��b�Z�[�W�\��
                If .IsMessageDefined("����(" & .Name & ")") Then
                    OpenMessageForm
                    .PilotMessage "����(" & .Name & ")"
                    CloseMessageForm
                ElseIf .IsMessageDefined("����") Then
                    OpenMessageForm
                    .PilotMessage "����"
                    CloseMessageForm
                End If
                .SpecialEffect "����", .Name
                
                '���[�C�x���g
                Set SelectedTarget = MapDataForUnit(SelectedX, SelectedY)
                HandleEvent "���[", .MainPilot.ID
            Else
                '���̌�̃��j�b�g��I��
                Set SelectedUnit = MapDataForUnit(SelectedX, SelectedY)
                
                '���̃C�x���g
                HandleEvent "����", SelectedUnit.MainPilot.ID, SelectedUnit.Name
            End If
            
            '�ړ���̎��[�E���̃C�x���g�ŃX�e�[�W���I�����邱�Ƃ�����̂�
            If IsScenarioFinished Then
                IsScenarioFinished = False
                UnlockGUI
                Exit Sub
            End If
            If IsCanceled Then
                IsCanceled = False
                ClearUnitStatus
                RedrawScreen
                CommandState = "���j�b�g�I��"
                UnlockGUI
                Exit Sub
            End If
            
            '�c��s����������������
            SelectedUnit.UseAction
            
            '�������Ԃ��u�ړ��v�̃X�y�V�����p���[���ʂ��폜
            SelectedUnit.RemoveSpecialPowerInEffect "�ړ�"
            
            DisplayUnitStatus MapDataForUnit(SelectedX, SelectedY)
            RedrawScreen
            CommandState = "���j�b�g�I��"
            UnlockGUI
            Exit Sub
        End If
' ADD START MARGE
        SelectedUnitMoveCost = 100
' ADD END MARGE
    End With
    
    CommandState = "�ړ���R�}���h�I��"
    UnlockGUI
    ProceedCommand
End Sub


'�u�U���v�R�}���h���J�n
' MOD START MARGE
'Public Sub StartAttackCommand()
Private Sub StartAttackCommand()
' MOD END MARGE
Dim i As Integer, j As Integer
Dim t As Unit, u As Unit
Dim min_range As Integer, max_range As Integer
Dim BGM As String

    LockGUI
    
    With SelectedUnit
        '�a�f�l�̐ݒ�
        If .IsFeatureAvailable("�a�f�l") Then
            BGM = SearchMidiFile(.FeatureData("�a�f�l"))
        End If
        If Len(BGM) = 0 Then
            BGM = SearchMidiFile(.MainPilot.BGM)
        End If
        If Len(BGM) = 0 Then
            BGM = BGMName("default")
        End If
        
        '����̑I��
        UseSupportAttack = True
        If CommandState = "�R�}���h�I��" Then
            SelectedWeapon = WeaponListBox(SelectedUnit, "����I��", "�ړ��O", BGM)
        Else
            SelectedWeapon = WeaponListBox(SelectedUnit, "����I��", "�ړ���", BGM)
        End If
        
        '�L�����Z��
        If SelectedWeapon = 0 Then
            If AutoMoveCursor Then
                RestoreCursorPos
            End If
            CancelCommand
            UnlockGUI
            Exit Sub
        End If
        
        '����a�f�l�̉��t
        If .IsFeatureAvailable("����a�f�l") Then
            For i = 1 To .CountFeature
                If .Feature(i) = "����a�f�l" _
                    And LIndex(.FeatureData(i), 1) = .Weapon(SelectedWeapon).Name _
                Then
                    BGM = SearchMidiFile(Mid$(.FeatureData(i), _
                        InStr(.FeatureData(i), " ") + 1))
                    If Len(BGM) > 0 Then
                        ChangeBGM BGM
                    End If
                    Exit For
                End If
            Next
        End If
        
        '�I����������̎�ނɂ��A���̌�̃R�}���h�̐i�s�̎d�����قȂ�
        If .IsWeaponClassifiedAs(SelectedWeapon, "�l") Then
            SelectedCommand = "�}�b�v�U��"
        Else
            SelectedCommand = "�U��"
        End If
        
        '����̎˒������߂Ă���
        min_range = .Weapon(SelectedWeapon).MinRange
        max_range = .WeaponMaxRange(SelectedWeapon)
        
        '�U���͈͂̕\��
        If .IsWeaponClassifiedAs(SelectedWeapon, "�l��") Then
            AreaInCross .X, .Y, min_range, max_range
        ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "�l�g") Then
            AreaInWideCross .X, .Y, min_range, max_range
        ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "�l��") Then
            AreaInSectorCross .X, .Y, min_range, max_range, _
                .WeaponLevel(SelectedWeapon, "�l��")
        ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "�l�S") _
            Or .IsWeaponClassifiedAs(SelectedWeapon, "�l��") _
            Or .IsWeaponClassifiedAs(SelectedWeapon, "�l��") _
        Then
            AreaInRange .X, .Y, max_range, min_range, "���ׂ�"
        ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "�l��") Then
            AreaInMoveAction SelectedUnit, max_range
        Else
            AreaInRange .X, .Y, max_range, min_range, "�����̓G"
        End If
        
        '�˒��P�̍��̋Z�̓p�[�g�i�[�ő�������͂�ł��Ȃ��Ǝg�p�ł��Ȃ�
        If max_range = 1 _
            And .IsWeaponClassifiedAs(SelectedWeapon, "��") _
            And Not .IsWeaponClassifiedAs(SelectedWeapon, "�l") _
        Then
            Dim partners() As Unit
            For i = 1 To 4
                Set t = Nothing
                Select Case i
                    Case 1
                        If .X > 1 Then
                            Set t = MapDataForUnit(.X - 1, .Y)
                        End If
                    Case 2
                        If .X < MapWidth Then
                            Set t = MapDataForUnit(.X + 1, .Y)
                        End If
                    Case 3
                        If .Y > 1 Then
                            Set t = MapDataForUnit(.X, .Y - 1)
                        End If
                    Case 4
                        If .Y < MapHeight Then
                            Set t = MapDataForUnit(.X, .Y + 1)
                        End If
                End Select
                
                If Not t Is Nothing Then
                    If .IsEnemy(t) Then
                        .CombinationPartner "����", SelectedWeapon, partners, t.X, t.Y
                        If UBound(partners) = 0 Then
                            MaskData(t.X, t.Y) = True
                        End If
                    End If
                End If
            Next
        End If
        
        '���j�b�g�ɑ΂���}�X�N�̐ݒ�
        If Not .IsWeaponClassifiedAs(SelectedWeapon, "�l��") _
            And Not .IsWeaponClassifiedAs(SelectedWeapon, "�l��") _
        Then
            For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
                For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
                    If MaskData(i, j) Then
                        GoTo NextLoop
                    End If
                    
                    Set t = MapDataForUnit(i, j)
                    
                    If t Is Nothing Then
                        GoTo NextLoop
                    End If
                    
                    '����̒n�`�K�����L���H
                    If .WeaponAdaption(SelectedWeapon, t.Area) = 0 Then
                        MaskData(i, j) = True
                        GoTo NextLoop
                    End If
                    
                    '���󕐊�̑Ώۑ����O�łȂ��H
                    If .IsWeaponClassifiedAs(SelectedWeapon, "��") Then
                        If (.Weapon(SelectedWeapon).Power > 0 _
                                And .Damage(SelectedWeapon, t, True) = 0) _
                            Or .CriticalProbability(SelectedWeapon, t) = 0 _
                        Then
                            MaskData(i, j) = True
                            GoTo NextLoop
                        End If
                    End If
                    
                    '���蕐��̑Ώۑ����O�łȂ��H
                    If .IsWeaponClassifiedAs(SelectedWeapon, "��") Then
                        If (.Weapon(SelectedWeapon).Power > 0 _
                                And .Damage(SelectedWeapon, t, True) = 0) _
                            Or (.Weapon(SelectedWeapon).Power = 0 _
                                And .CriticalProbability(SelectedWeapon, t) = 0) _
                        Then
                            MaskData(i, j) = True
                            GoTo NextLoop
                        End If
                    End If
                    
                    '���ʍU���̏ꍇ�̏���
                    If .IsWeaponClassifiedAs(SelectedWeapon, "��") _
                        Or .IsUnderSpecialPowerEffect("���ʍU��") _
                    Then
                        If .IsAlly(t) Then
                            MaskData(i, j) = True
                            GoTo NextLoop
                        End If
                    End If
                    
                    '�X�e���X���B��g�`�F�b�N
                    If Not .IsWeaponClassifiedAs(SelectedWeapon, "�l") Then
                        If Not .IsTargetWithinRange(SelectedWeapon, t) Then
                            MaskData(i, j) = True
                            GoTo NextLoop
                        End If
                    End If
NextLoop:
               Next
            Next
        End If
        MaskData(.X, .Y) = False
        If Not IsOptionDefined("��^�}�b�v") Then
            Center .X, .Y
        End If
        MaskScreen
    End With
    
    '�^�[�Q�b�g�I����
    If CommandState = "�R�}���h�I��" Then
        CommandState = "�^�[�Q�b�g�I��"
    Else
        CommandState = "�ړ���^�[�Q�b�g�I��"
    End If
    
    '�J�[�\�������ړ����s���H
    If Not AutoMoveCursor Then
        UnlockGUI
        Exit Sub
    End If
    
    '�g�o�������Ƃ��Ⴂ�^�[�Q�b�g��T��
    Set t = Nothing
    For Each u In UList
        With u
            If .Status = "�o��" And (.Party = "�G" Or .Party = "����") Then
                If MaskData(.X, .Y) = False Then
                    If t Is Nothing Then
                        Set t = u
                    ElseIf .HP < t.HP Then
                        Set t = u
                    ElseIf .HP = t.HP Then
                        If Abs(SelectedUnit.X - .X) ^ 2 + Abs(SelectedUnit.Y - .Y) ^ 2 _
                            < Abs(SelectedUnit.X - t.X) ^ 2 + Abs(SelectedUnit.Y - t.Y) ^ 2 _
                        Then
                            Set t = u
                        End If
                    End If
                End If
            End If
        End With
    Next
    
    '�K���ȃ^�[�Q�b�g��������Ȃ���Ύ������g��I��
    If t Is Nothing Then
        Set t = SelectedUnit
    End If
    
    '�J�[�\�����ړ�
    MoveCursorPos "���j�b�g�I��", t
    
    '�^�[�Q�b�g�̃X�e�[�^�X��\��
    If Not SelectedUnit Is t Then
        DisplayUnitStatus t
    End If
    
    UnlockGUI
End Sub

'�u�U���v�R�}���h���I��
' MOD START MARGE
'Public Sub FinishAttackCommand()
Private Sub FinishAttackCommand()
' MOD END MARGE
Dim i As Integer
Dim earnings As Long
Dim def_mode As String
Dim u As Unit, partners() As Unit
Dim BGM As String
Dim is_suiside As Boolean
Dim wname As String, twname As String
Dim tx As Integer, ty As Integer
Dim attack_target As Unit, attack_target_hp_ratio As Double
Dim defense_target As Unit, defense_target_hp_ratio As Double
Dim defense_target2 As Unit, defense_target2_hp_ratio As Double
Dim support_attack_done As Boolean, w2 As Integer
' ADD START MARGE
Dim is_p_weapon As Boolean
' ADD END MARGE
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        ClearUnitStatus
    End If
    
    LockGUI
    
    wname = SelectedUnit.Weapon(SelectedWeapon).Name
    SelectedWeaponName = wname
    
' ADD START MARGE
    '�ړ���g�p��\�ȕ��킩�L�^���Ă���
    is_p_weapon = SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "�ړ���U����")
' ADD END MARGE

    '���̋Z�̃p�[�g�i�[��ݒ�
    With SelectedUnit
        If .IsWeaponClassifiedAs(SelectedWeapon, "��") Then
            If .WeaponMaxRange(SelectedWeapon) = 1 Then
                .CombinationPartner "����", SelectedWeapon, partners, _
                    SelectedTarget.X, SelectedTarget.Y
            Else
                .CombinationPartner "����", SelectedWeapon, partners
            End If
        Else
            ReDim SelectedPartners(0)
            ReDim partners(0)
        End If
    End With
    
    '�G�̔�����i��ݒ�
    UseSupportGuard = True
    SelectedTWeapon = SelectWeapon(SelectedTarget, SelectedUnit, "����")
    If SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "��") Then
        SelectedTWeapon = 0
    End If
    If SelectedTWeapon > 0 Then
        twname = SelectedTarget.Weapon(SelectedTWeapon).Name
        SelectedTWeaponName = twname
    Else
        SelectedTWeaponName = ""
    End If
    
    '�G�̖h��s����ݒ�
    def_mode = SelectDefense(SelectedUnit, SelectedWeapon, SelectedTarget, SelectedTWeapon)
    If def_mode <> "" Then
        If SelectedTWeapon > 0 Then
            SelectedTWeapon = -1
        End If
    End If
    SelectedDefenseOption = def_mode
    
    '�퓬�O�Ɉ�U�N���A
    Set SupportAttackUnit = Nothing
    Set SupportGuardUnit = Nothing
    Set SupportGuardUnit2 = Nothing
    
    '�U�����̕���g�p�C�x���g
    HandleEvent "�g�p", SelectedUnit.MainPilot.ID, wname
    If IsScenarioFinished Then
        UnlockGUI
        IsScenarioFinished = False
        ReDim SelectedPartners(0)
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        WaitCommand
        Exit Sub
    End If
    
    '�G�̕���g�p�C�x���g
    If SelectedTWeapon > 0 Then
        SaveSelections
        SwapSelections
        HandleEvent "�g�p", SelectedUnit.MainPilot.ID, twname
        RestoreSelections
        If IsScenarioFinished Then
            IsScenarioFinished = False
            ReDim SelectedPartners(0)
            UnlockGUI
            Exit Sub
        End If
        If IsCanceled Then
            IsCanceled = False
            ReDim SelectedPartners(0)
            WaitCommand
            Exit Sub
        End If
    End If
    
    '�U���C�x���g
    HandleEvent "�U��", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID
    If IsScenarioFinished Then
        IsScenarioFinished = False
        ReDim SelectedPartners(0)
        UnlockGUI
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        ReDim SelectedPartners(0)
        WaitCommand
        Exit Sub
    End If
    
    '�G���a�f�l�\�͂����ꍇ�͂a�f�l��ύX
    With SelectedTarget
        If .IsFeatureAvailable("�a�f�l") _
            And InStr(.MainPilot.Name, "(�U�R)") = 0 _
        Then
            BGM = SearchMidiFile(.FeatureData("�a�f�l"))
            If Len(BGM) > 0 Then
                BossBGM = False
                ChangeBGM BGM
                BossBGM = True
            End If
        End If
    End With
    
    '�����ł͂Ȃ��A�{�X�p�a�f�l������Ă���Ζ����̂a�f�l�ɐ؂�ւ�
    If Len(BGM) = 0 And BossBGM Then
        BossBGM = False
        BGM = ""
        With SelectedUnit
            If .IsFeatureAvailable("����a�f�l") Then
                For i = 1 To .CountFeature
                    If .Feature(i) = "����a�f�l" _
                        And LIndex(.FeatureData(i), 1) = .Weapon(SelectedWeapon).Name _
                    Then
                        BGM = SearchMidiFile(Mid$(.FeatureData(i), _
                            InStr(.FeatureData(i), " ") + 1))
                        Exit For
                    End If
                Next
            End If
            If Len(BGM) = 0 Then
                If .IsFeatureAvailable("�a�f�l") Then
                    BGM = SearchMidiFile(.FeatureData("�a�f�l"))
                End If
            End If
            If Len(BGM) = 0 Then
                BGM = SearchMidiFile(.MainPilot.BGM)
            End If
            If Len(BGM) = 0 Then
                BGM = BGMName("default")
            End If
            ChangeBGM BGM
        End With
    End If
    
    With SelectedUnit
        '�U���Q�����j�b�g�ȊO�̓}�X�N
        For Each u In UList
            With u
                If .Status = "�o��" Then
                    MaskData(.X, .Y) = True
                End If
            End With
        Next
        
        '���̋Z�p�[�g�i�[�̃n�C���C�g�\��
        For i = 1 To UBound(partners)
            With partners(i)
                MaskData(.X, .Y) = False
            End With
        Next
        
        MaskData(.X, .Y) = False
        MaskData(SelectedTarget.X, SelectedTarget.Y) = False
        If Not BattleAnimation Then
            MaskScreen
        End If
    End With
    
    '�C�x���g�p�ɐ퓬�ɎQ�����郆�j�b�g�̏����L�^���Ă���
    Set AttackUnit = SelectedUnit
    Set attack_target = SelectedUnit
    attack_target_hp_ratio = SelectedUnit.HP / SelectedUnit.MaxHP
    Set defense_target = SelectedTarget
    defense_target_hp_ratio = SelectedTarget.HP / SelectedTarget.MaxHP
    Set defense_target2 = Nothing
    Set SupportAttackUnit = Nothing
    Set SupportGuardUnit = Nothing
    Set SupportGuardUnit2 = Nothing
    
    '�^�[�Q�b�g�̈ʒu���L�^
    tx = SelectedTarget.X
    ty = SelectedTarget.Y
    
    OpenMessageForm SelectedTarget, SelectedUnit
    
    '����̐搧�U���H
    With SelectedTarget
' MOD START MARGE
'        If SelectedTWeapon > 0 And .MaxAction > 0 And .IsWeaponAvailable(SelectedTWeapon, "�ړ��O") Then
        'SelectedTWeapon > 0�̔���́AIsWeaponAvailable�ōs���悤�ɂ���
        If .MaxAction > 0 And .IsWeaponAvailable(SelectedTWeapon, "�ړ��O") Then
' MOD END MARGE
            If Not .IsWeaponClassifiedAs(SelectedTWeapon, "��") Then
                If SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "��") Then
                    def_mode = "�搧�U��"
                    
                    If .IsWeaponClassifiedAs(SelectedTWeapon, "��") Then
                        is_suiside = True
                    End If
                    
                    '�搧�U���U�������{
                    .Attack SelectedTWeapon, SelectedUnit, "�搧�U��", ""
                    Set SelectedTarget = .CurrentForm
                ElseIf .IsWeaponClassifiedAs(SelectedTWeapon, "��") _
                    Or .MainPilot.SkillLevel("��ǂ�") >= Dice(16) _
                    Or .IsUnderSpecialPowerEffect("�J�E���^�[") _
                Then
                    def_mode = "�搧�U��"
                    
                    If .IsWeaponClassifiedAs(SelectedTWeapon, "��") Then
                        is_suiside = True
                    End If
                    
                    '�J�E���^�[�U�������{
                    .Attack SelectedTWeapon, SelectedUnit, "�J�E���^�[", ""
                    Set SelectedTarget = .CurrentForm
                ElseIf .MaxCounterAttack > .UsedCounterAttack Then
                    def_mode = "�搧�U��"
                    
                    If .IsWeaponClassifiedAs(SelectedTWeapon, "��") Then
                        is_suiside = True
                    End If
                    
                    '�J�E���^�[�U���̎c��񐔂�����
                    .UsedCounterAttack = .UsedCounterAttack + 1
                    
                    '�J�E���^�[�U�������{
                    .Attack SelectedTWeapon, SelectedUnit, "�J�E���^�[", ""
                    Set SelectedTarget = .CurrentForm
                End If
            End If
            
            '�U�����̃��j�b�g�����΂�ꂽ�ꍇ�͍U�����̃^�[�Q�b�g���Đݒ�
            If Not SupportGuardUnit2 Is Nothing Then
                Set attack_target = SupportGuardUnit2
                attack_target_hp_ratio = SupportGuardUnitHPRatio2
            End If
        End If
    End With
    
    '�T�|�[�g�A�^�b�N�̃p�[�g�i�[��T��
    With SelectedUnit
        If .Status = "�o��" And SelectedTarget.Status = "�o��" _
            And UseSupportAttack _
        Then
            Set SupportAttackUnit = .LookForSupportAttack(SelectedTarget)
            
            '���̋Z�ł̓T�|�[�g�A�^�b�N�s�\
            If 0 < SelectedWeapon And SelectedWeapon <= .CountWeapon Then
                If .IsWeaponClassifiedAs(SelectedWeapon, "��") Then
                    Set SupportAttackUnit = Nothing
                End If
            End If
            
            '�������ꂽ�ꍇ
            If .IsConditionSatisfied("����") And .Master Is SelectedTarget Then
                Set SupportAttackUnit = Nothing
            End If
            
            '�߈˂��ꂽ�ꍇ
            If .IsConditionSatisfied("�߈�") Then
                If .Master.Party = SelectedTarget.Party Then
                    Set SupportAttackUnit = Nothing
                End If
            End If
            
            '�x�炳�ꂽ�ꍇ
            If .IsConditionSatisfied("�x��") Then
                Set SupportAttackUnit = Nothing
            End If
        End If
    End With
    
    '�U���̎��{
    With SelectedUnit
        If .Status = "�o��" _
            And .MaxAction(True) > 0 _
            And Not .IsConditionSatisfied("�U���s�\") _
            And SelectedTarget.Status = "�o��" _
        Then
            '�܂�����͎g�p�\���H
            If SelectedWeapon > .CountWeapon Then
                SelectedWeapon = -1
            ElseIf wname <> .Weapon(SelectedWeapon).Name Then
                SelectedWeapon = -1
            ElseIf CommandState = "�ړ���^�[�Q�b�g�I��" Then
                If Not .IsWeaponAvailable(SelectedWeapon, "�ړ���") Then
                    SelectedWeapon = -1
                End If
            Else
                If Not .IsWeaponAvailable(SelectedWeapon, "�ړ��O") Then
                    SelectedWeapon = -1
                End If
            End If
            If SelectedWeapon > 0 Then
                If Not .IsTargetWithinRange(SelectedWeapon, SelectedTarget) Then
                    SelectedWeapon = 0
                End If
            End If
            
            '�������ꂽ�ꍇ
            If .IsConditionSatisfied("����") And .Master Is SelectedTarget Then
                SelectedWeapon = -1
            End If
            
            '�߈˂��ꂽ�ꍇ
            If .IsConditionSatisfied("�߈�") Then
                If .Master.Party = SelectedTarget.Party0 Then
                    SelectedWeapon = -1
                End If
            End If
            
            '�x�炳�ꂽ�ꍇ
            If .IsConditionSatisfied("�x��") Then
                SelectedWeapon = -1
            End If
            
            If SelectedWeapon > 0 Then
                If Not SupportAttackUnit Is Nothing _
                    And .MaxSyncAttack > .UsedSyncAttack _
                Then
                    '��������U��
                    .Attack SelectedWeapon, SelectedTarget, "����", def_mode
                Else
                    '�ʏ�U��
                    .Attack SelectedWeapon, SelectedTarget, "", def_mode
                End If
            ElseIf SelectedWeapon = 0 Then
                If .IsAnimationDefined("�˒��O") Then
                    .PlayAnimation "�˒��O"
                Else
                    .SpecialEffect "�˒��O"
                End If
                .PilotMessage "�˒��O"
            End If
        Else
            SelectedWeapon = -1
        End If
        Set SelectedUnit = .CurrentForm
        
        '�h�䑤�̃��j�b�g�����΂�ꂽ�ꍇ��2�Ԗڂ̖h�䑤���j�b�g�Ƃ��ċL�^
        If Not SupportGuardUnit Is Nothing Then
            Set defense_target2 = SupportGuardUnit
            defense_target2_hp_ratio = SupportGuardUnitHPRatio
        End If
    End With
    
    '�����U��
    If Not SupportAttackUnit Is Nothing Then
        If SupportAttackUnit.Status <> "�o��" _
            Or SelectedUnit.Status <> "�o��" _
            Or SelectedTarget.Status <> "�o��" _
        Then
            Set SupportAttackUnit = Nothing
        End If
    End If
    If Not SupportAttackUnit Is Nothing Then
        If SelectedUnit.MaxSyncAttack > SelectedUnit.UsedSyncAttack Then
            With SupportAttackUnit
                '�T�|�[�g�A�^�b�N�Ɏg�����������
                w2 = SelectWeapon(SupportAttackUnit, SelectedTarget, "�T�|�[�g�A�^�b�N")
                
                If w2 > 0 Then
                    '�T�|�[�g�A�^�b�N�����{
                    MaskData(.X, .Y) = False
                    If Not BattleAnimation Then
                        MaskScreen
                    End If
                    If .IsAnimationDefined("�T�|�[�g�A�^�b�N�J�n") Then
                        .PlayAnimation "�T�|�[�g�A�^�b�N�J�n"
                    End If
                    UpdateMessageForm SelectedTarget, SupportAttackUnit
                    .Attack w2, SelectedTarget, "��������U��", def_mode
                End If
            End With
            
            '��n��
            With SupportAttackUnit.CurrentForm
                If w2 > 0 Then
                    If .IsAnimationDefined("�T�|�[�g�A�^�b�N�I��") Then
                        .PlayAnimation "�T�|�[�g�A�^�b�N�I��"
                    End If
                    
                    '�T�|�[�g�A�^�b�N�̎c��񐔂����炷
                    .UsedSupportAttack = .UsedSupportAttack + 1
                    
                    '��������U���̎c��񐔂����炷
                    SelectedUnit.UsedSyncAttack = SelectedUnit.UsedSyncAttack + 1
                End If
            End With
            
            support_attack_done = True
            
            '�h�䑤�̃��j�b�g�����΂�ꂽ�ꍇ�͖{���̖h�䃆�j�b�g�f�[�^��
            '����ւ��ċL�^
            If Not SupportGuardUnit Is Nothing Then
                Set defense_target = SupportGuardUnit
                defense_target_hp_ratio = SupportGuardUnitHPRatio
            End If
        End If
    End If
    
    '�����̎��{
    With SelectedTarget
        If def_mode <> "�搧�U��" Then
            If .Status = "�o��" _
                And .Party <> "����" _
                And SelectedUnit.Status = "�o��" _
            Then
                '�܂�����͎g�p�\���H
                If SelectedTWeapon > 0 Then
                    If SelectedTWeapon > .CountWeapon Then
                        SelectedTWeapon = -1
                    ElseIf twname <> .Weapon(SelectedTWeapon).Name _
                        Or Not .IsWeaponAvailable(SelectedTWeapon, "�ړ��O") _
                    Then
                        SelectedTWeapon = -1
                    End If
                End If
                If SelectedTWeapon > 0 Then
                    If Not .IsTargetWithinRange(SelectedTWeapon, SelectedUnit) Then
                        '�G���˒��O�ɓ����Ă����畐����đI��
                        SelectedTWeapon = SelectWeapon(SelectedTarget, SelectedUnit, "����")
                    End If
                End If
                
                '�s���s�\�ȏꍇ
                If .MaxAction = 0 Then
                    SelectedTWeapon = -1
                End If
                
                '�������ꂽ�ꍇ
                If .IsConditionSatisfied("����") And .Master Is SelectedUnit Then
                    SelectedTWeapon = -1
                End If
                
                '�߈˂��ꂽ�ꍇ
                If .IsConditionSatisfied("�߈�") Then
                    If .Master.Party = SelectedUnit.Party Then
                        SelectedWeapon = -1
                    End If
                End If
                
                '�x�炳�ꂽ�ꍇ
                If .IsConditionSatisfied("�x��") Then
                    SelectedTWeapon = -1
                End If
                
                If SelectedTWeapon > 0 And def_mode = "" Then
                    '���������{
                    If .IsWeaponClassifiedAs(SelectedTWeapon, "��") Then
                        is_suiside = True
                    End If
                    .Attack SelectedTWeapon, SelectedUnit, "", ""
                    
                    '�U�����̃��j�b�g�����΂�ꂽ�ꍇ�͍U�����̃^�[�Q�b�g���Đݒ�
                    If Not SupportGuardUnit2 Is Nothing Then
                        Set attack_target = SupportGuardUnit2
                        attack_target_hp_ratio = SupportGuardUnitHPRatio2
                    End If
                ElseIf SelectedTWeapon = 0 And .X = tx And .Y = ty Then
                    '�����o���镐�킪�Ȃ������ꍇ�͎˒��O���b�Z�[�W��\��
                    If .IsAnimationDefined("�˒��O") Then
                        .PlayAnimation "�˒��O"
                    Else
                        .SpecialEffect "�˒��O"
                    End If
                    .PilotMessage "�˒��O"
                Else
                    SelectedTWeapon = -1
                End If
            Else
                SelectedTWeapon = -1
            End If
        End If
    End With
    
    '�T�|�[�g�A�^�b�N
    If Not SupportAttackUnit Is Nothing Then
        If SupportAttackUnit.Status <> "�o��" _
            Or SelectedUnit.Status <> "�o��" _
            Or SelectedTarget.Status <> "�o��" _
            Or support_attack_done _
        Then
            Set SupportAttackUnit = Nothing
        End If
    End If
    If Not SupportAttackUnit Is Nothing Then
        With SupportAttackUnit
            '�T�|�[�g�A�^�b�N�Ɏg�����������
            w2 = SelectWeapon(SupportAttackUnit, SelectedTarget, "�T�|�[�g�A�^�b�N")
                        
            If w2 > 0 Then
                '�T�|�[�g�A�^�b�N�����{
                MaskData(.X, .Y) = False
                If Not BattleAnimation Then
                    MaskScreen
                End If
                If .IsAnimationDefined("�T�|�[�g�A�^�b�N�J�n") Then
                    .PlayAnimation "�T�|�[�g�A�^�b�N�J�n"
                End If
                UpdateMessageForm SelectedTarget, SupportAttackUnit
                .Attack w2, SelectedTarget, "����U��", def_mode
            End If
        End With
        
        '��n��
        With SupportAttackUnit.CurrentForm
            If .IsAnimationDefined("�T�|�[�g�A�^�b�N�I��") Then
                .PlayAnimation "�T�|�[�g�A�^�b�N�I��"
            End If
            
            '�T�|�[�g�A�^�b�N�̎c��񐔂����炷
            If w2 > 0 Then
                .UsedSupportAttack = .UsedSupportAttack + 1
            End If
        End With
        
        '�h�䑤�̃��j�b�g�����΂�ꂽ�ꍇ�͖{���̖h�䃆�j�b�g�f�[�^��
        '����ւ��ċL�^
        If Not SupportGuardUnit Is Nothing Then
            Set defense_target = SupportGuardUnit
            defense_target_hp_ratio = SupportGuardUnitHPRatio
        End If
    End If
    
    Set SelectedTarget = SelectedTarget.CurrentForm
    
    With SelectedUnit
        If .Status = "�o��" Then
            '�U�������������j�b�g���܂������c���Ă���Όo���l���������l��
            
            If SelectedTarget.Status = "�j��" And Not is_suiside Then
                '�G��j�󂵂��ꍇ
                
                '�o���l���l��
                .GetExp SelectedTarget, "�j��"
                If Not IsOptionDefined("���̋Z�p�[�g�i�[�o���l����") Then
                    For i = 1 To UBound(partners)
                        partners(i).CurrentForm.GetExp SelectedTarget, "�j��", "�p�[�g�i�["
                    Next
                End If
                
                '�l�����鎑�����Z�o
                earnings = SelectedTarget.Value \ 2
                
                '�X�y�V�����p���[�ɂ��l����������
                If .IsUnderSpecialPowerEffect("�l����������") Then
                    earnings = earnings * (1 + 0.1 * .SpecialPowerEffectLevel("�l����������"))
                End If
                
                '�p�C���b�g�\�͂ɂ��l����������
                If .IsSkillAvailable("�����l��") Then
                    If Not .IsUnderSpecialPowerEffect("�l����������") _
                        Or IsOptionDefined("�������ʏd��") _
                    Then
                        earnings = MinDbl(earnings * ((10 + .SkillLevel("�����l��", 5)) / 10), 999999999)
                    End If
                End If
                
                '�������l��
                IncrMoney earnings
                
                If earnings > 0 Then
                    DisplaySysMessage Format$(earnings) & "��" & Term("����", SelectedUnit) & "�𓾂��B"
                End If
                
                '�X�y�V�����p���[���ʁu�G�j�󎞍čs���v
                If .IsUnderSpecialPowerEffect("�G�j�󎞍čs��") Then
                    .UsedAction = .UsedAction - 1
                End If
            Else
                '�����j��ł��Ȃ������ꍇ
                
                '�o���l���l��
                .GetExp SelectedTarget, "�U��"
                If Not IsOptionDefined("���̋Z�p�[�g�i�[�o���l����") Then
                    For i = 1 To UBound(partners)
                        partners(i).CurrentForm.GetExp SelectedTarget, "�U��", "�p�[�g�i�["
                    Next
                End If
            End If
            
            '�X�y�V�����p���[�u�l�����������v�u�l���o���l�����v�̌��ʂ͂����ō폜����
            .RemoveSpecialPowerInEffect "�퓬�I��"
            If earnings > 0 Then
                .RemoveSpecialPowerInEffect "�G�j��"
            End If
        End If
    End With
    
    With SelectedTarget
        If .Status = "�o��" Then
            '�������Ԃ��u�퓬�I���v�̃X�y�V�����p���[���ʂ��폜
            .RemoveSpecialPowerInEffect "�퓬�I��"
        End If
    End With
    
    CloseMessageForm
    ClearUnitStatus
    
    '��ԁ��f�[�^�X�V
    With attack_target.CurrentForm
        .UpdateCondition
        .Update
    End With
    If Not SupportAttackUnit Is Nothing Then
        With SupportAttackUnit.CurrentForm
            .UpdateCondition
            .Update
        End With
    End If
    With defense_target.CurrentForm
        .UpdateCondition
        .Update
    End With
    If Not defense_target2 Is Nothing Then
        With defense_target2.CurrentForm
            .UpdateCondition
            .Update
        End With
    End If
    
    '�j�󁕑������C�x���g����
    
    If SelectedWeapon <= 0 Then
        SelectedWeaponName = ""
    End If
    If SelectedTWeapon <= 0 Then
        SelectedTWeaponName = ""
    End If
    
    '�U�����󂯂��U�������j�b�g
    With attack_target.CurrentForm
        If .Status = "�j��" Then
            HandleEvent "�j��", .MainPilot.ID
        ElseIf .Status = "�o��" And .HP / .MaxHP < attack_target_hp_ratio Then
            HandleEvent "������", .MainPilot.ID, 100 * (.MaxHP - .HP) \ .MaxHP
        End If
    End With
    If IsScenarioFinished Then
        UnlockGUI
        IsScenarioFinished = False
        ReDim SelectedPartners(0)
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        GoTo EndAttack
    End If
    
    Set SelectedUnit = SelectedUnit.CurrentForm
    
    '�^�[�Q�b�g���̃C�x���g�������s�����߂Ƀ��j�b�g�̓���ւ����s��
    SaveSelections
    SwapSelections
    
    '�U�����󂯂��h�䑤���j�b�g
    With defense_target.CurrentForm
        If .CountPilot > 0 Then
            If .Status = "�j��" Then
                HandleEvent "�j��", .MainPilot.ID
            ElseIf .Status = "�o��" And .HP / .MaxHP < defense_target_hp_ratio Then
                HandleEvent "������", .MainPilot.ID, 100 * (.MaxHP - .HP) \ .MaxHP
            End If
        End If
    End With
    
    If IsScenarioFinished Then
        IsScenarioFinished = False
        ReDim SelectedPartners(0)
        Exit Sub
    End If
    
    '�U�����󂯂��h�䑤���j�b�g����2
    If Not defense_target2 Is Nothing Then
        If Not defense_target2.CurrentForm Is defense_target.CurrentForm Then
            With defense_target2.CurrentForm
                If .CountPilot > 0 Then
                    If .Status = "�j��" Then
                        HandleEvent "�j��", .MainPilot.ID
                    ElseIf .Status = "�o��" And .HP / .MaxHP < defense_target2_hp_ratio Then
                        HandleEvent "������", .MainPilot.ID, 100 * (.MaxHP - .HP) \ .MaxHP
                    End If
                End If
            End With
        End If
    End If
    
    RestoreSelections
    
    If IsScenarioFinished Then
        IsScenarioFinished = False
        ReDim SelectedPartners(0)
        UnlockGUI
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        ReDim SelectedPartners(0)
        UnlockGUI
        Exit Sub
    End If
    
    '����̎g�p��C�x���g
    If SelectedUnit.Status = "�o��" And SelectedWeapon > 0 Then
        HandleEvent "�g�p��", SelectedUnit.MainPilot.ID, wname
        If IsScenarioFinished Then
            IsScenarioFinished = False
            ReDim SelectedPartners(0)
            UnlockGUI
            Exit Sub
        End If
        If IsCanceled Then
            IsCanceled = False
            ReDim SelectedPartners(0)
            UnlockGUI
            Exit Sub
        End If
    End If
    
    If SelectedTarget.Status = "�o��" And SelectedTWeapon > 0 Then
        SaveSelections
        SwapSelections
        HandleEvent "�g�p��", SelectedUnit.MainPilot.ID, twname
        RestoreSelections
        If IsScenarioFinished Then
            IsScenarioFinished = False
            ReDim SelectedPartners(0)
            UnlockGUI
            Exit Sub
        End If
        If IsCanceled Then
            IsCanceled = False
            ReDim SelectedPartners(0)
            UnlockGUI
            Exit Sub
        End If
    End If
    
    '�U����C�x���g
    If SelectedUnit.Status = "�o��" _
        And SelectedTarget.Status = "�o��" _
    Then
        HandleEvent "�U����", SelectedUnit.MainPilot.ID, _
            SelectedTarget.MainPilot.ID
        If IsScenarioFinished Then
            IsScenarioFinished = False
            ReDim SelectedPartners(0)
            UnlockGUI
            Exit Sub
        End If
        If IsCanceled Then
            IsCanceled = False
            ReDim SelectedPartners(0)
            UnlockGUI
            Exit Sub
        End If
    End If
    
    '�����G���ړ����Ă���ΐi���C�x���g
    With SelectedTarget
        Set SelectedTarget = Nothing
        If .Status = "�o��" Then
            If .X <> tx Or .Y <> ty Then
                HandleEvent "�i��", .MainPilot.ID, .X, .Y
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                    ReDim SelectedPartners(0)
                    UnlockGUI
                    Exit Sub
                End If
                If IsCanceled Then
                    IsCanceled = False
                    ReDim SelectedPartners(0)
                    UnlockGUI
                    Exit Sub
                End If
            End If
        End If
    End With
    
EndAttack:
    
    '���̋Z�̃p�[�g�i�[�̍s���������炷
    If Not IsOptionDefined("���̋Z�p�[�g�i�[�s����������") Then
        For i = 1 To UBound(partners)
            partners(i).CurrentForm.UseAction
        Next
    End If
    ReDim SelectedPartners(0)
    
    '�n�C�p�[���[�h���m�[�}�����[�h�̎����������`�F�b�N
    UList.CheckAutoHyperMode
    UList.CheckAutoNormalMode
    
' ADD START MARGE
    '�Ĉړ�
    If is_p_weapon And SelectedUnit.Status = "�o��" Then
        If SelectedUnit.MainPilot.IsSkillAvailable("�V��") _
            And SelectedUnit.Speed * 2 > SelectedUnitMoveCost _
        Then
            '�i���C�x���g
            If SelectedUnitMoveCost > 0 Then
                HandleEvent "�i��", SelectedUnit.MainPilot.ID, _
                    SelectedUnit.X, SelectedUnit.Y
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                    Exit Sub
                End If
            End If
            
            '���j�b�g�����ɏo�����Ă��Ȃ��H
            If SelectedUnit.Status <> "�o��" Then
                RedrawScreen
                ClearUnitStatus
                Exit Sub
            End If
            
            SelectedCommand = "�Ĉړ�"
            AreaInSpeed SelectedUnit
            If Not IsOptionDefined("��^�}�b�v") Then
                Center SelectedUnit.X, SelectedUnit.Y
            End If
            MaskScreen
            If NewGUIMode Then
                DoEvents
                ClearUnitStatus
            Else
                DisplayUnitStatus SelectedUnit
            End If
            CommandState = "�^�[�Q�b�g�I��"
            Exit Sub
        End If
    End If
' ADD END MARGE

    '�s���������炷
    WaitCommand
End Sub

'�}�b�v�U���ɂ��u�U���v�R�}���h���I��
' MOD START MARGE
'Public Sub MapAttackCommand()
Private Sub MapAttackCommand()
' MOD END MARGE
Dim i As Integer
Dim partners() As Unit
' ADD START MARGE
Dim is_p_weapon As Boolean
' ADD END MARGE

    With SelectedUnit
' ADD START MARGE
        '�ړ���g�p��\�ȕ��킩�L�^���Ă���
        is_p_weapon = .IsWeaponClassifiedAs(SelectedWeapon, "�ړ���U����")
' ADD END MARGE
        '�U���ڕW�n�_��I�����ď��߂čU���͈͂�������^�C�v�̃}�b�v�U��
        '�̏ꍇ�͍ēx�v���C���[�̑I���𑣂��K�v������
        If CommandState = "�^�[�Q�b�g�I��" _
            Or CommandState = "�ړ���^�[�Q�b�g�I��" _
        Then
            If .IsWeaponClassifiedAs(SelectedWeapon, "�l��") Then
                If CommandState = "�^�[�Q�b�g�I��" Then
                    CommandState = "�}�b�v�U���g�p"
                Else
                    CommandState = "�ړ���}�b�v�U���g�p"
                End If
                
                '�U���ڕW�n�_
                SelectedX = PixelToMapX(MouseX)
                SelectedY = PixelToMapY(MouseY)
                
                '�U���͈͂�ݒ�
                If .IsWeaponClassifiedAs(SelectedWeapon, "��") _
                    Or .IsUnderSpecialPowerEffect("���ʍU��") _
                Then
                    AreaInRange SelectedX, SelectedY, _
                        .WeaponLevel(SelectedWeapon, "�l��"), _
                        1, "�����̓G"
                Else
                    AreaInRange SelectedX, SelectedY, _
                        .WeaponLevel(SelectedWeapon, "�l��"), _
                        1, "���ׂ�"
                End If
                MaskScreen
                Exit Sub
            ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "�l��") Then
                '�U���ڕW�n�_
                SelectedX = PixelToMapX(MouseX)
                SelectedY = PixelToMapY(MouseY)
                
                '�U���ڕW�n�_�ɑ��̃��j�b�g�����Ă͑ʖ�
                If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
                    MaskScreen
                    Exit Sub
                End If
                
                If CommandState = "�^�[�Q�b�g�I��" Then
                    CommandState = "�}�b�v�U���g�p"
                Else
                    CommandState = "�ړ���}�b�v�U���g�p"
                End If
                
                '�U���͈͂�ݒ�
                AreaInPointToPoint .X, .Y, SelectedX, SelectedY
                MaskScreen
                Exit Sub
            ElseIf .IsWeaponClassifiedAs(SelectedWeapon, "�l��") Then
                If CommandState = "�^�[�Q�b�g�I��" Then
                    CommandState = "�}�b�v�U���g�p"
                Else
                    CommandState = "�ړ���}�b�v�U���g�p"
                End If
                
                '�U���ڕW�n�_
                SelectedX = PixelToMapX(MouseX)
                SelectedY = PixelToMapY(MouseY)
                                
                '�U���͈͂�ݒ�
                AreaInPointToPoint .X, .Y, SelectedX, SelectedY
                MaskScreen
                Exit Sub
            End If
        End If
        
        '���̋Z�p�[�g�i�[�̐ݒ�
        If .IsWeaponClassifiedAs(SelectedWeapon, "��") Then
            .CombinationPartner "����", SelectedWeapon, partners
        Else
            ReDim SelectedPartners(0)
            ReDim partners(0)
        End If
        
        If MainWidth <> 15 Then
            ClearUnitStatus
        End If
        
        LockGUI
        
        SelectedTWeapon = 0
        
        '�}�b�v�U���ɂ��U�����s��
        .MapAttack SelectedWeapon, SelectedX, SelectedY
        Set SelectedUnit = .CurrentForm
        Set SelectedTarget = Nothing
        
        If IsScenarioFinished Then
            IsScenarioFinished = False
            ReDim SelectedPartners(0)
            UnlockGUI
            Exit Sub
        End If
        If IsCanceled Then
            IsCanceled = False
            ReDim SelectedPartners(0)
            WaitCommand
            Exit Sub
        End If
    End With
    
    '���̋Z�̃p�[�g�i�[�̍s���������炷
    If Not IsOptionDefined("���̋Z�p�[�g�i�[�s����������") Then
        For i = 1 To UBound(partners)
            partners(i).CurrentForm.UseAction
        Next
    End If
    ReDim SelectedPartners(0)
    
' ADD START MARGE
    '�Ĉړ�
    If is_p_weapon And SelectedUnit.Status = "�o��" Then
        If SelectedUnit.MainPilot.IsSkillAvailable("�V��") _
            And SelectedUnit.Speed * 2 > SelectedUnitMoveCost _
        Then
            '�i���C�x���g
            If SelectedUnitMoveCost > 0 Then
                HandleEvent "�i��", SelectedUnit.MainPilot.ID, _
                    SelectedUnit.X, SelectedUnit.Y
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                    Exit Sub
                End If
            End If
            
            '���j�b�g�����ɏo�����Ă��Ȃ��H
            If SelectedUnit.Status <> "�o��" Then
                RedrawScreen
                ClearUnitStatus
                Exit Sub
            End If
            
            SelectedCommand = "�Ĉړ�"
            AreaInSpeed SelectedUnit
            If Not IsOptionDefined("��^�}�b�v") Then
                Center SelectedUnit.X, SelectedUnit.Y
            End If
            MaskScreen
            If NewGUIMode Then
                DoEvents
                ClearUnitStatus
            Else
                DisplayUnitStatus SelectedUnit
            End If
            CommandState = "�^�[�Q�b�g�I��"
            Exit Sub
        End If
    End If
' ADD END MARGE

    '�s���I��
    WaitCommand
End Sub


'�u�A�r���e�B�v�R�}���h���J�n
' is_item=True �̏ꍇ�́u�A�C�e���v�R�}���h�ɂ��g���̂ăA�C�e���̃A�r���e�B
' MOD STAR MARGE
'Public Sub StartAbilityCommand(Optional ByVal is_item As Boolean)
Private Sub StartAbilityCommand(Optional ByVal is_item As Boolean)
' MOD END MARGE
Dim i As Integer, j As Integer
Dim t As Unit, u As Unit
Dim min_range As Integer, max_range As Integer
Dim cap As String

    LockGUI
    
    '�g�p����A�r���e�B��I��
    If is_item Then
        cap = "�A�C�e���I��"
    Else
        cap = Term("�A�r���e�B", SelectedUnit) & "�I��"
    End If
    If CommandState = "�R�}���h�I��" Then
        SelectedAbility = AbilityListBox(SelectedUnit, cap, "�ړ��O", is_item)
    Else
        SelectedAbility = AbilityListBox(SelectedUnit, cap, "�ړ���", is_item)
    End If
    
    '�L�����Z��
    If SelectedAbility = 0 Then
        If AutoMoveCursor Then
            RestoreCursorPos
        End If
        CancelCommand
        UnlockGUI
        Exit Sub
    End If
    
    '�A�r���e�B��p�a�f�l������΂�������t
    With SelectedUnit
        If .IsFeatureAvailable("�A�r���e�B�a�f�l") Then
            Dim BGM As String
            For i = 1 To .CountFeature
                If .Feature(i) = "�A�r���e�B�a�f�l" _
                    And LIndex(.FeatureData(i), 1) = .Ability(SelectedAbility).Name _
                Then
                    BGM = SearchMidiFile(Mid$(.FeatureData(i), _
                        InStr(.FeatureData(i), " ") + 1))
                    If Len(BGM) > 0 Then
                        ChangeBGM BGM
                    End If
                    Exit For
                End If
            Next
        End If
    End With
    
    '�˒�0�̃A�r���e�B�͂��̏�Ŏ��s
    If SelectedUnit.Ability(SelectedAbility).MaxRange = 0 Then
        Dim is_transformation As Boolean
        
        Set SelectedTarget = SelectedUnit
        
        '�ϐg�A�r���e�B�ł��邩����
        For i = 1 To SelectedUnit.Ability(SelectedAbility).CountEffect
            If SelectedUnit.Ability(SelectedAbility).EffectType(i) = "�ϐg" Then
                is_transformation = True
                Exit For
            End If
        Next
        
        SelectedAbilityName = SelectedUnit.Ability(SelectedAbility).Name
        
        '�g�p�C�x���g
        HandleEvent "�g�p", SelectedUnit.MainPilot.ID, SelectedAbilityName
        If IsScenarioFinished Then
            IsScenarioFinished = False
            UnlockGUI
            Exit Sub
        End If
        If IsCanceled Then
            IsCanceled = False
            WaitCommand
            Exit Sub
        End If
        
        '�A�r���e�B�����s
        SelectedUnit.ExecuteAbility SelectedAbility, SelectedUnit
        Set SelectedUnit = SelectedUnit.CurrentForm
        CloseMessageForm
        
        '�j��C�x���g
        With SelectedUnit
            If .Status = "�j��" Then
                If .CountPilot > 0 Then
                    HandleEvent "�j��", .MainPilot.ID
                    If IsScenarioFinished Then
                        IsScenarioFinished = False
                        UnlockGUI
                        Exit Sub
                    End If
                    If IsCanceled Then
                        IsCanceled = False
                        UnlockGUI
                        Exit Sub
                    End If
                End If
                WaitCommand
                Exit Sub
            End If
        End With
        
        '�g�p��C�x���g
        With SelectedUnit
            If .CountPilot > 0 Then
                HandleEvent "�g�p��", .MainPilot.ID, SelectedAbilityName
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                    UnlockGUI
                    Exit Sub
                End If
                If IsCanceled Then
                    IsCanceled = False
                    UnlockGUI
                    Exit Sub
                End If
            End If
        End With
        
        '�ϐg�A�r���e�B�̏ꍇ�͍s���I�����Ȃ�
        If Not is_transformation Or CommandState = "�ړ���R�}���h�I��" Then
            WaitCommand
        Else
            If SelectedUnit.Status = "�o��" Then
                '�J�[�\�������ړ�
                If AutoMoveCursor Then
                    MoveCursorPos "���j�b�g�I��", SelectedUnit
                End If
                DisplayUnitStatus SelectedUnit
            Else
                ClearUnitStatus
            End If
            CommandState = "���j�b�g�I��"
            UnlockGUI
        End If
        Exit Sub
    End If
    
    With SelectedUnit
        '�}�b�v�^�A�r���e�B���ǂ����ō���̃R�}���h�����̐i�s�̎d�����قȂ�
        If is_item Then
            If .IsAbilityClassifiedAs(SelectedAbility, "�l") Then
                SelectedCommand = "�}�b�v�A�C�e��"
            Else
                SelectedCommand = "�A�C�e��"
            End If
        Else
            If .IsAbilityClassifiedAs(SelectedAbility, "�l") Then
                SelectedCommand = "�}�b�v�A�r���e�B"
            Else
                SelectedCommand = "�A�r���e�B"
            End If
        End If
        
        '�A�r���e�B�̎˒������߂Ă���
        min_range = .AbilityMinRange(SelectedAbility)
        max_range = .AbilityMaxRange(SelectedAbility)
        
        '�A�r���e�B�̌��ʔ͈͂�ݒ�
        If .IsAbilityClassifiedAs(SelectedAbility, "�l��") Then
            AreaInCross .X, .Y, min_range, max_range
        ElseIf .IsAbilityClassifiedAs(SelectedAbility, "�l�g") Then
            AreaInWideCross .X, .Y, min_range, max_range
        ElseIf .IsAbilityClassifiedAs(SelectedAbility, "�l��") Then
            AreaInSectorCross .X, .Y, min_range, max_range, _
                .AbilityLevel(SelectedAbility, "�l��")
        ElseIf .IsAbilityClassifiedAs(SelectedAbility, "�l��") Then
            AreaInMoveAction SelectedUnit, max_range
        Else
            AreaInRange .X, .Y, max_range, min_range, "���ׂ�"
        End If
                
        '�˒��P�̍��̋Z�̓p�[�g�i�[�ő�������͂�ł��Ȃ��Ǝg�p�ł��Ȃ�
        If .IsAbilityClassifiedAs(SelectedAbility, "��") _
            And Not .IsAbilityClassifiedAs(SelectedAbility, "�l") _
            And .Ability(SelectedAbility).MaxRange = 1 _
        Then
            Dim partners() As Unit
            For i = 1 To 4
                Set t = Nothing
                Select Case i
                    Case 1
                        If .X > 1 Then
                            Set t = MapDataForUnit(.X - 1, .Y)
                        End If
                    Case 2
                        If .X < MapWidth Then
                            Set t = MapDataForUnit(.X + 1, .Y)
                        End If
                    Case 3
                        If .Y > 1 Then
                            Set t = MapDataForUnit(.X, .Y - 1)
                        End If
                    Case 4
                        If .Y < MapHeight Then
                            Set t = MapDataForUnit(.X, .Y + 1)
                        End If
                End Select
                
                If Not t Is Nothing Then
                    If .IsAlly(t) Then
                        .CombinationPartner "�A�r���e�B", SelectedAbility, _
                            partners, t.X, t.Y
                        If UBound(partners) = 0 Then
                            MaskData(t.X, t.Y) = True
                        End If
                    End If
                End If
            Next
        End If
        
        '���j�b�g������}�X�̏���
        If Not .IsAbilityClassifiedAs(SelectedAbility, "�l��") _
            And Not .IsAbilityClassifiedAs(SelectedAbility, "�l��") _
            And Not .IsAbilityClassifiedAs(SelectedAbility, "�l��") _
        Then
            For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
                For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
                    If Not MaskData(i, j) Then
                        Set t = MapDataForUnit(i, j)
                        If Not t Is Nothing Then
                            '�L���H
                            If .IsAbilityEffective(SelectedAbility, t) Then
                                MaskData(i, j) = False
                            Else
                                MaskData(i, j) = True
                            End If
                        End If
                    End If
                Next
            Next
        End If
        
        '�x����p�A�r���e�B�͎����ɂ͎g�p�ł��Ȃ�
        If Not MaskData(.X, .Y) Then
            If .IsAbilityClassifiedAs(SelectedAbility, "��") Then
                MaskData(.X, .Y) = True
            End If
        End If
        
        If Not IsOptionDefined("��^�}�b�v") Then
            Center .X, .Y
        End If
        MaskScreen
    End With
    
    If CommandState = "�R�}���h�I��" Then
        CommandState = "�^�[�Q�b�g�I��"
    Else
        CommandState = "�ړ���^�[�Q�b�g�I��"
    End If
    
    '�J�[�\�������ړ����s���H
    If Not AutoMoveCursor Then
        UnlockGUI
        Exit Sub
    End If
    
    '��������ł��߂��������j�b�g��T��
    Set t = Nothing
    For Each u In UList
        With u
            If .Status = "�o��" And .Party = "����" Then
                If MaskData(.X, .Y) = False And Not u Is SelectedUnit Then
                    If t Is Nothing Then
                        Set t = u
                    Else
                        If Abs(SelectedUnit.X - .X) ^ 2 + Abs(SelectedUnit.Y - .Y) ^ 2 _
                            < Abs(SelectedUnit.X - t.X) ^ 2 + Abs(SelectedUnit.Y - t.Y) ^ 2 _
                        Then
                            Set t = u
                        End If
                    End If
                End If
            End If
        End With
    Next
    
    '�K�������j�b�g���Ȃ���Ύ������g��I��
    If t Is Nothing Then
        Set t = SelectedUnit
    End If
    
    '�J�[�\�����ړ�
    MoveCursorPos "���j�b�g�I��", t
    
    '�^�[�Q�b�g�̃X�e�[�^�X��\��
    If Not SelectedUnit Is t Then
        DisplayUnitStatus t
    End If
    
    UnlockGUI
End Sub

'�u�A�r���e�B�v�R�}���h���I��
' MOD START MARGE
'Public Sub FinishAbilityCommand()
Private Sub FinishAbilityCommand()
' MOD END MARGE
Dim i As Integer
Dim u As Unit, partners() As Unit
Dim aname As String
' ADD START MARGE
Dim is_p_ability As Boolean
' ADD END MARGE

' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        ClearUnitStatus
    End If
    
    LockGUI
    
    '���̋Z�̃p�[�g�i�[��ݒ�
    With SelectedUnit
        If .IsAbilityClassifiedAs(SelectedAbility, "��") Then
            If .AbilityMaxRange(SelectedAbility) = 1 Then
                .CombinationPartner "�A�r���e�B", SelectedAbility, partners, _
                    SelectedTarget.X, SelectedTarget.Y
            Else
                .CombinationPartner "�A�r���e�B", SelectedAbility, partners
            End If
        Else
            ReDim SelectedPartners(0)
            ReDim partners(0)
        End If
    End With
    
    aname = SelectedUnit.Ability(SelectedAbility).Name
    SelectedAbilityName = aname
    
' ADD START MARGE
    '�ړ���g�p��\�ȃA�r���e�B���L�^���Ă���
    is_p_ability = SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, "�o") _
            Or (SelectedUnit.AbilityMaxRange(SelectedAbility) = 1 _
                And Not SelectedUnit.IsAbilityClassifiedAs(SelectedAbility, "�p"))
' ADD END MARGE

    '�g�p�C�x���g
    HandleEvent "�g�p", SelectedUnit.MainPilot.ID, aname
    If IsScenarioFinished Then
        IsScenarioFinished = False
        ReDim SelectedPartners(0)
        UnlockGUI
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        ReDim SelectedPartners(0)
        WaitCommand
        Exit Sub
    End If
    
    With SelectedUnit
        For Each u In UList
            With u
                If .Status = "�o��" Then
                    MaskData(.X, .Y) = True
                End If
            End With
        Next
        
        '���̋Z�p�[�g�i�[�̃n�C���C�g�\��
        If .IsAbilityClassifiedAs(SelectedAbility, "��") Then
            For i = 1 To UBound(partners)
                With partners(i)
                    MaskData(.X, .Y) = False
                End With
            Next
        End If
        
        MaskData(.X, .Y) = False
        MaskData(SelectedTarget.X, SelectedTarget.Y) = False
        If Not BattleAnimation Then
            MaskScreen
        End If
        
        '�A�r���e�B�����s
        .ExecuteAbility SelectedAbility, SelectedTarget
        Set SelectedUnit = .CurrentForm
        
        CloseMessageForm
        ClearUnitStatus
    End With
    
    '�j��C�x���g
    With SelectedUnit
        If .Status = "�j��" Then
            If .CountPilot > 0 Then
                HandleEvent "�j��", .MainPilot.ID
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                    ReDim SelectedPartners(0)
                    UnlockGUI
                    Exit Sub
                End If
                If IsCanceled Then
                    IsCanceled = False
                    ReDim SelectedPartners(0)
                    UnlockGUI
                    Exit Sub
                End If
            End If
            WaitCommand
            Exit Sub
        End If
    End With
    
    '�g�p��C�x���g
    With SelectedUnit
        If .CountPilot > 0 Then
            HandleEvent "�g�p��", .MainPilot.ID, aname
            If IsScenarioFinished Then
                IsScenarioFinished = False
                ReDim SelectedPartners(0)
                UnlockGUI
                Exit Sub
            End If
            If IsCanceled Then
                IsCanceled = False
                ReDim SelectedPartners(0)
                UnlockGUI
                Exit Sub
            End If
        End If
    End With
    
    '���̋Z�̃p�[�g�i�[�̍s���������炷
    If Not IsOptionDefined("���̋Z�p�[�g�i�[�s����������") Then
        For i = 1 To UBound(partners)
            partners(i).CurrentForm.UseAction
        Next
    End If
    ReDim SelectedPartners(0)
    
' ADD START MARGE
    '�Ĉړ�
    If is_p_ability And SelectedUnit.Status = "�o��" Then
        If SelectedUnit.MainPilot.IsSkillAvailable("�V��") _
            And SelectedUnit.Speed * 2 > SelectedUnitMoveCost _
        Then
            '�i���C�x���g
            If SelectedUnitMoveCost > 0 Then
                HandleEvent "�i��", SelectedUnit.MainPilot.ID, _
                    SelectedUnit.X, SelectedUnit.Y
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                    Exit Sub
                End If
            End If
            
            '���j�b�g�����ɏo�����Ă��Ȃ��H
            If SelectedUnit.Status <> "�o��" Then
                RedrawScreen
                ClearUnitStatus
                Exit Sub
            End If
            
            SelectedCommand = "�Ĉړ�"
            AreaInSpeed SelectedUnit
            If Not IsOptionDefined("��^�}�b�v") Then
                Center SelectedUnit.X, SelectedUnit.Y
            End If
            MaskScreen
            If NewGUIMode Then
                DoEvents
                ClearUnitStatus
            Else
                DisplayUnitStatus SelectedUnit
            End If
            CommandState = "�^�[�Q�b�g�I��"
            Exit Sub
        End If
    End If
' ADD END MARGE

    '�s���I��
    WaitCommand
End Sub

'�}�b�v�^�u�A�r���e�B�v�R�}���h���I��
' MOD START MARGE
'Public Sub MapAbilityCommand()
Private Sub MapAbilityCommand()
' MOD END MARGE
Dim i As Integer
Dim partners() As Unit
' ADD START MARGE
Dim is_p_ability As Boolean
' ADD END MARGE

    With SelectedUnit
' ADD START MARGE
        '�ړ���g�p��\�ȃA�r���e�B���L�^���Ă���
        is_p_ability = .IsAbilityClassifiedAs(SelectedAbility, "�o") _
                Or (.AbilityMaxRange(SelectedAbility) = 1 _
                    And Not .IsAbilityClassifiedAs(SelectedAbility, "�p"))
' ADD END MARGE

        '�ڕW�n�_��I�����ď��߂Č��ʔ͈͂�������^�C�v�̃}�b�v�A�r���e�B
        '�̏ꍇ�͍ēx�v���C���[�̑I���𑣂��K�v������
        If CommandState = "�^�[�Q�b�g�I��" _
            Or CommandState = "�ړ���^�[�Q�b�g�I��" _
        Then
            If .IsAbilityClassifiedAs(SelectedAbility, "�l��") Then
                If CommandState = "�^�[�Q�b�g�I��" Then
                    CommandState = "�}�b�v�U���g�p"
                Else
                    CommandState = "�ړ���}�b�v�U���g�p"
                End If
                
                '�ڕW�n�_
                SelectedX = PixelToMapX(MouseX)
                SelectedY = PixelToMapY(MouseY)
                
                '���ʔ͈͂�ݒ�
                AreaInRange SelectedX, SelectedY, _
                    .AbilityLevel(SelectedAbility, "�l��"), _
                    1, "����"
                MaskScreen
                Exit Sub
            ElseIf .IsAbilityClassifiedAs(SelectedAbility, "�l��") Then
                SelectedX = PixelToMapX(MouseX)
                SelectedY = PixelToMapY(MouseY)
                
                If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
                    MaskScreen
                    Exit Sub
                End If
                
                '�ڕW�n�_
                If CommandState = "�^�[�Q�b�g�I��" Then
                    CommandState = "�}�b�v�U���g�p"
                Else
                    CommandState = "�ړ���}�b�v�U���g�p"
                End If
                
                '���ʔ͈͂�ݒ�
                AreaInPointToPoint .X, .Y, SelectedX, SelectedY
                MaskScreen
                Exit Sub
            ElseIf .IsAbilityClassifiedAs(SelectedAbility, "�l��") Then
                If CommandState = "�^�[�Q�b�g�I��" Then
                    CommandState = "�}�b�v�U���g�p"
                Else
                    CommandState = "�ړ���}�b�v�U���g�p"
                End If
                
                '�ڕW�n�_
                SelectedX = PixelToMapX(MouseX)
                SelectedY = PixelToMapY(MouseY)
                                
                '���ʔ͈͂�ݒ�
                AreaInPointToPoint .X, .Y, SelectedX, SelectedY
                MaskScreen
                Exit Sub
            End If
        End If
        
        '���̋Z�p�[�g�i�[�̐ݒ�
        If .IsAbilityClassifiedAs(SelectedAbility, "��") Then
            .CombinationPartner "�A�r���e�B", SelectedAbility, partners
        Else
            ReDim SelectedPartners(0)
            ReDim partners(0)
        End If
    End With
    
    If MainWidth <> 15 Then
        ClearUnitStatus
    End If
    
    LockGUI
    
    '�A�r���e�B�����s
    SelectedUnit.ExecuteMapAbility _
        SelectedAbility, SelectedX, SelectedY
    Set SelectedUnit = SelectedUnit.CurrentForm
    Set SelectedTarget = Nothing
    
    If IsScenarioFinished Then
        IsScenarioFinished = False
        ReDim SelectedPartners(0)
        UnlockGUI
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        ReDim SelectedPartners(0)
        WaitCommand
        Exit Sub
    End If
    
    '���̋Z�̃p�[�g�i�[�̍s���������炷
    If Not IsOptionDefined("���̋Z�p�[�g�i�[�s����������") Then
        For i = 1 To UBound(partners)
            partners(i).CurrentForm.UseAction
        Next
    End If
    ReDim SelectedPartners(0)
    
' ADD START MARGE
    '�Ĉړ�
    If is_p_ability And SelectedUnit.Status = "�o��" Then
        If SelectedUnit.MainPilot.IsSkillAvailable("�V��") _
            And SelectedUnit.Speed * 2 > SelectedUnitMoveCost _
        Then
            '�i���C�x���g
            If SelectedUnitMoveCost > 0 Then
                HandleEvent "�i��", SelectedUnit.MainPilot.ID, _
                    SelectedUnit.X, SelectedUnit.Y
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                    Exit Sub
                End If
            End If
            
            '���j�b�g�����ɏo�����Ă��Ȃ��H
            If SelectedUnit.Status <> "�o��" Then
                RedrawScreen
                ClearUnitStatus
                Exit Sub
            End If
            
            SelectedCommand = "�Ĉړ�"
            AreaInSpeed SelectedUnit
            If Not IsOptionDefined("��^�}�b�v") Then
                Center SelectedUnit.X, SelectedUnit.Y
            End If
            MaskScreen
            If NewGUIMode Then
                DoEvents
                ClearUnitStatus
            Else
                DisplayUnitStatus SelectedUnit
            End If
            CommandState = "�^�[�Q�b�g�I��"
            Exit Sub
        End If
    End If
' ADD END MARGE

    '�s���I��
    WaitCommand
End Sub


'�X�y�V�����p���[�R�}���h���J�n
' MOD START MARGE
'Public Sub StartSpecialPowerCommand()
Private Sub StartSpecialPowerCommand()
' MOD END MARGE
Dim i As Integer, j As Integer, n As Integer, ret As Integer
Dim pname_list() As String, pid_list() As String
Dim sp_list() As String
Dim list() As String, id_list() As String
Dim sname As String, sd As SpecialPowerData
Dim u As Unit, p As Pilot
Dim strkey_list() As String, max_item As Integer, max_str As String
Dim buf As String
Dim found As Boolean

    LockGUI
    
    SelectedCommand = "�X�y�V�����p���["
    
    With SelectedUnit
        ReDim pname_list(0)
        ReDim pid_list(0)
        ReDim ListItemFlag(0)
        
        '�X�y�V�����p���[���g�p�\�ȃp�C���b�g�̈ꗗ���쐬
        n = .CountPilot + .CountSupport
        If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
            n = n + 1
        End If
        For i = 1 To n
            If i <= .CountPilot Then
                '���C���p�C���b�g���T�u�p�C���b�g
                Set p = .Pilot(i)
                
                If i = 1 Then
                    '�P�Ԗڂ̃p�C���b�g�̏ꍇ�̓��C���p�C���b�g���g�p
                    Set p = .MainPilot
                    
                    '�������Q�l���ȏ�̃��j�b�g�ŁA���C���p�C���b�g��
                    '�X�y�V�����p���[�������Ȃ��ꍇ�͂��̂܂܂P�Ԗڂ̃p�C���b�g���g�p
                    If .CountPilot > 1 Then
                        If p.Data.SP <= 0 And .Pilot(1).Data.SP > 0 Then
                            Set p = .Pilot(1)
                        End If
                    End If
                    
                    '�T�u�p�C���b�g�����C���p�C���b�g���΂߂Ă���ꍇ��
                    '�P�Ԗڂ̃p�C���b�g���g�p
                    For j = 2 To .CountPilot
                        If p Is .Pilot(j) Then
                            Set p = .Pilot(1)
                            Exit For
                        End If
                    Next
                Else
                    
                End If
                
                With p
                    If .CountSpecialPower = 0 Then
                        GoTo NextPilot
                    End If
                    
                    ReDim Preserve pname_list(UBound(pname_list) + 1)
                    ReDim Preserve pid_list(UBound(pname_list) + 1)
                    ReDim Preserve ListItemFlag(UBound(pname_list))
                    ListItemFlag(UBound(pname_list)) = False
                    
                    pid_list(UBound(pname_list)) = .ID
                    pname_list(UBound(pname_list)) = _
                        RightPaddedString(.Nickname, 17) & _
                        RightPaddedString(Format$(.SP) & "/" & Format$(.MaxSP), 8)
                    For j = 1 To .CountSpecialPower
                        sname = .SpecialPower(j)
                        If .SP >= .SpecialPowerCost(sname) Then
                            pname_list(UBound(pname_list)) = _
                                pname_list(UBound(pname_list)) & _
                                SPDList.Item(sname).ShortName
                        End If
                    Next
                End With
            ElseIf i <= .CountPilot + .CountSupport Then
                '�T�|�[�g�p�C���b�g
                With .Support(i - .CountPilot)
                    If .CountSpecialPower = 0 Then
                        GoTo NextPilot
                    End If
                    
                    ReDim Preserve pname_list(UBound(pname_list) + 1)
                    ReDim Preserve pid_list(UBound(pname_list) + 1)
                    ReDim Preserve ListItemFlag(UBound(pname_list))
                    ListItemFlag(UBound(pname_list)) = False
                    
                    pid_list(UBound(pname_list)) = .ID
                    pname_list(UBound(pname_list)) = _
                        RightPaddedString(.Nickname, 17) & _
                        RightPaddedString(Format$(.SP) & "/" & Format$(.MaxSP), 8)
                    For j = 1 To .CountSpecialPower
                        sname = .SpecialPower(j)
                        If .SP >= .SpecialPowerCost(sname) Then
                            pname_list(UBound(pname_list)) = _
                                pname_list(UBound(pname_list)) & _
                                SPDList.Item(sname).ShortName
                        End If
                    Next
                End With
            Else
                '�ǉ��T�|�[�g�p�C���b�g
                With .AdditionalSupport
                    If .CountSpecialPower = 0 Then
                        GoTo NextPilot
                    End If
                    
                    ReDim Preserve pname_list(UBound(pname_list) + 1)
                    ReDim Preserve pid_list(UBound(pname_list) + 1)
                    ReDim Preserve ListItemFlag(UBound(pname_list))
                    ListItemFlag(UBound(pname_list)) = False
                    
                    pid_list(UBound(pname_list)) = .ID
                    pname_list(UBound(pname_list)) = _
                        RightPaddedString(.Nickname, 17) & _
                        RightPaddedString(Format$(.SP) & "/" & Format$(.MaxSP), 8)
                    For j = 1 To .CountSpecialPower
                        sname = .SpecialPower(j)
                        If .SP >= .SpecialPowerCost(sname) Then
                            pname_list(UBound(pname_list)) = _
                                pname_list(UBound(pname_list)) & _
                                SPDList.Item(sname).ShortName
                        End If
                    Next
                End With
            End If
NextPilot:
        Next
        TopItem = 1
        If UBound(pname_list) > 1 Then
            '�ǂ̃p�C���b�g���g�����I��
            If IsOptionDefined("���g��") Then
                i = ListBox("�L�����N�^�[�I��", pname_list, _
                    "�L�����N�^�[     " & Term("SP", SelectedUnit, 2) _
                         & "/Max" & Term("SP", SelectedUnit, 2), _
                    "�A���\��,�J�[�\���ړ�")
            Else
                i = ListBox("�p�C���b�g�I��", pname_list, _
                    "�p�C���b�g       " & Term("SP", SelectedUnit, 2) _
                         & "/Max" & Term("SP", SelectedUnit, 2), _
                    "�A���\��,�J�[�\���ړ�")
            End If
        Else
            '��l�������Ȃ��̂őI���̕K�v�Ȃ�
            i = 1
        End If
        
        '�N���X�y�V�����p���[���g���Ȃ���΃L�����Z��
        If i = 0 Then
            frmListBox.Hide
            If AutoMoveCursor Then
                RestoreCursorPos
            End If
            UnlockGUI
            CancelCommand
            Exit Sub
        End If
        
        '�X�y�V�����p���[���g���p�C���b�g��ݒ�
        Set SelectedPilot = PList.Item(pid_list(i))
        '���̃p�C���b�g�̃X�e�[�^�X��\��
        If UBound(pname_list) > 1 Then
            DisplayPilotStatus SelectedPilot
        End If
    End With
    
    With SelectedPilot
        '�g�p�\�ȃX�y�V�����p���[�̈ꗗ���쐬
        ReDim sp_list(.CountSpecialPower)
        ReDim ListItemFlag(.CountSpecialPower)
        For i = 1 To .CountSpecialPower
            sname = .SpecialPower(i)
            sp_list(i) = RightPaddedString(sname, 13) _
                & LeftPaddedString(Format$(.SpecialPowerCost(sname)), 3) & "  " _
                & SPDList.Item(sname).Comment
            
            If .SP >= .SpecialPowerCost(sname) Then
                If Not .IsSpecialPowerUseful(sname) Then
                    ListItemFlag(i) = True
                End If
            Else
                ListItemFlag(i) = True
            End If
        Next
    End With
    
    '�ǂ̃R�}���h���g�p���邩��I��
    With SelectedPilot
        TopItem = 1
        i = ListBox(Term("�X�y�V�����p���[", SelectedUnit) & "�I��", sp_list, _
            "����         ����" & Term("SP", SelectedUnit) & "�i" _
                & .Nickname & " " & Term("SP", SelectedUnit) & "=" _
                & Format$(.SP) & "/" & Format$(.MaxSP) & "�j", _
            "�J�[�\���ړ�(�s���̂�)")
    End With
    
    '�L�����Z��
    If i = 0 Then
        DisplayUnitStatus SelectedUnit
        '�J�[�\�������ړ�
        If AutoMoveCursor Then
            MoveCursorPos "���j�b�g�I��", SelectedUnit
        End If
        UnlockGUI
        CancelCommand
        Exit Sub
    End If
    
    '�g�p����X�y�V�����p���[��ݒ�
    SelectedSpecialPower = SelectedPilot.SpecialPower(i)
    
    '�����X�y�V�����p���[���s�̌��ʂɂ�葼�̃p�C���b�g�������Ă���X�y�V�����p���[��
    '�g���ꍇ�͋L�^���Ă����A��ŏ���r�o��{�ɂ���K�v������
    If SPDList.Item(SelectedSpecialPower).EffectType(1) = "�����X�y�V�����p���[���s" Then
        '�X�y�V�����p���[�ꗗ
        ReDim list(0)
        For i = 1 To SPDList.Count
            With SPDList.Item(i)
                If .EffectType(1) <> "�����X�y�V�����p���[���s" _
                    And .ShortName <> "��\��" _
                Then
                    ReDim Preserve list(UBound(list) + 1)
                    ReDim Preserve strkey_list(UBound(list))
                    list(UBound(list)) = .Name
                    strkey_list(UBound(list)) = .KanaName
                End If
            End With
        Next
        ReDim ListItemFlag(UBound(list))
        
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
                buf = list(i)
                list(i) = list(max_item)
                list(max_item) = buf
                
                buf = strkey_list(i)
                strkey_list(i) = max_str
                strkey_list(max_item) = buf
            End If
        Next
        
        '�X�y�V�����p���[���g�p�\�ȃp�C���b�g�����邩�ǂ����𔻒�
        For i = 1 To UBound(list)
            ListItemFlag(i) = True
            For Each p In PList
                With p
                    If .Party = "����" Then
                        If Not .Unit Is Nothing Then
                            If .Unit.Status = "�o��" _
                                And Not .Unit.IsConditionSatisfied("�߈�") _
                            Then
                                '�{���ɏ���Ă���H
                                found = False
                                With .Unit
                                    If p Is .MainPilot Then
                                        found = True
                                    Else
                                        For j = 2 To .CountPilot
                                            If p Is .Pilot(j) Then
                                                found = True
                                                Exit For
                                            End If
                                        Next
                                        For j = 1 To .CountSupport
                                            If p Is .Support(j) Then
                                                found = True
                                                Exit For
                                            End If
                                        Next
                                        If p Is .AdditionalSupport Then
                                            found = True
                                        End If
                                    End If
                                End With
                                
                                If found Then
                                    If .IsSpecialPowerAvailable(list(i)) Then
                                        ListItemFlag(i) = False
                                        Exit For
                                    End If
                                End If
                            End If
                        End If
                    End If
                End With
            Next
        Next
        
        '�e�X�y�V�����p���[���g�p�\������
        With SelectedPilot
            For i = 1 To UBound(list)
                If Not ListItemFlag(i) _
                    And .SP >= 2 * .SpecialPowerCost(list(i)) _
                Then
                    If Not .IsSpecialPowerUseful(list(i)) Then
                        ListItemFlag(i) = True
                    End If
                Else
                    ListItemFlag(i) = True
                End If
            Next
        End With
        
        '�X�y�V�����p���[�̉����ݒ�
        ReDim ListItemComment(UBound(list))
        For i = 1 To UBound(list)
            ListItemComment(i) = SPDList.Item(list(i)).Comment
        Next
        
        '��������X�y�V�����p���[��I��
        TopItem = 1
        ret = MultiColumnListBox(Term("�X�y�V�����p���[") & "����", list, True)
        If ret = 0 Then
            SelectedSpecialPower = ""
            CancelCommand
            UnlockGUI
            Exit Sub
        End If
        
        '�X�y�V�����p���[�g�p���b�Z�[�W
        If SelectedUnit.IsMessageDefined(SelectedSpecialPower) Then
            OpenMessageForm
            SelectedUnit.PilotMessage SelectedSpecialPower
            CloseMessageForm
        End If
        
        SelectedSpecialPower = list(ret)
        
        WithDoubleSPConsumption = True
    Else
        WithDoubleSPConsumption = False
    End If
    
    Set sd = SPDList.Item(SelectedSpecialPower)
    
    '�^�[�Q�b�g��I������K�v������X�y�V�����p���[�̏ꍇ
    Select Case sd.TargetType
        Case "����", "�G", "�C��"
            '�}�b�v��̃��j�b�g����^�[�Q�b�g��I������
            
            OpenMessageForm
            DisplaySysMessage SelectedPilot.Nickname & "��" _
                & SelectedSpecialPower & "���g�����B;" _
                & "�^�[�Q�b�g��I��ł��������B"
            CloseMessageForm
            
            '�^�[�Q�b�g�̃G���A��ݒ�
            For i = 1 To MapWidth
                For j = 1 To MapHeight
                    MaskData(i, j) = True
                    
                    Set u = MapDataForUnit(i, j)
                    
                    If u Is Nothing Then
                        GoTo NextLoop
                    End If
                    
                    '�w�c�������Ă���H
                    Select Case sd.TargetType
                        Case "����"
                            With u
                                If .Party <> "����" _
                                   And .Party0 <> "����" _
                                   And .Party <> "�m�o�b" _
                                   And .Party0 <> "�m�o�b" _
                                Then
                                    GoTo NextLoop
                                End If
                            End With
                        Case "�G"
                            With u
                                If (.Party = "����" _
                                       And .Party0 = "����") _
                                   Or (.Party = "�m�o�b" _
                                       And .Party0 = "�m�o�b") _
                                Then
                                    GoTo NextLoop
                                End If
                            End With
                    End Select
                    
                    '�X�y�V�����p���[��K�p�\�H
                    If Not sd.Effective(SelectedPilot, u) Then
                        GoTo NextLoop
                    End If
                    
                    MaskData(i, j) = False
NextLoop:
                Next
            Next
            MaskScreen
            CommandState = "�^�[�Q�b�g�I��"
            UnlockGUI
            Exit Sub
            
        Case "�j�󖡕�"
            '�j�󂳂ꂽ�������j�b�g�̒�����^�[�Q�b�g��I������
            
            OpenMessageForm
            DisplaySysMessage _
                SelectedPilot.Nickname & "��" & SelectedSpecialPower & "���g�����B;" & _
                "���������郆�j�b�g��I��ł��������B"
            CloseMessageForm
            
            '�j�󂳂ꂽ�������j�b�g�̃��X�g���쐬
            ReDim list(0)
            ReDim id_list(0)
            ReDim ListItemFlag(0)
            For Each u In UList
                With u
                    If .Party0 = "����" _
                        And .Status = "�j��" _
                        And (.CountPilot > 0 Or .Data.PilotNum = 0) _
                    Then
                        ReDim Preserve list(UBound(list) + 1)
                        ReDim Preserve id_list(UBound(list))
                        ReDim Preserve ListItemFlag(UBound(list))
                        list(UBound(list)) = _
                            RightPaddedString(.Nickname, 28) _
                            & RightPaddedString(.MainPilot.Nickname, 18) _
                            & LeftPaddedString(Format$(.MainPilot.Level), 3)
                        id_list(UBound(list)) = .ID
                        ListItemFlag(UBound(list)) = False
                    End If
                End With
            Next
            
            TopItem = 1
            i = ListBox("���j�b�g�I��", list, "���j�b�g��                  �p�C���b�g     ���x��")
            If i = 0 Then
                UnlockGUI
                CancelCommand
                Exit Sub
            End If
            
            Set SelectedTarget = UList.Item(id_list(i))
    End Select
    
    '������I�������ꍇ�͊m�F�����
    If sd.IsEffectAvailable("����") Then
        ret = MsgBox("���������܂����H", _
            vbOKCancel + vbQuestion, "����")
        If ret = vbCancel Then
            UnlockGUI
            Exit Sub
        End If
    End If
    
    '�g�p�C�x���g
    HandleEvent "�g�p", SelectedUnit.MainPilot.ID, SelectedSpecialPower
    If IsScenarioFinished Then
        IsScenarioFinished = False
        UnlockGUI
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        UnlockGUI
        Exit Sub
    End If
    
    '�X�y�V�����p���[���g�p
    If WithDoubleSPConsumption Then
        SelectedPilot.UseSpecialPower SelectedSpecialPower, 2
    Else
        SelectedPilot.UseSpecialPower SelectedSpecialPower
    End If
    Set SelectedUnit = SelectedUnit.CurrentForm
    
    '�J�[�\�������ړ�
    If AutoMoveCursor Then
        MoveCursorPos "���j�b�g�I��", SelectedUnit
    End If
    
    '�X�e�[�^�X�E�B���h�E���X�V
    DisplayUnitStatus SelectedUnit
    
    '�g�p��C�x���g
    HandleEvent "�g�p��", SelectedUnit.MainPilot.ID, SelectedSpecialPower
    If IsScenarioFinished Then
        IsScenarioFinished = False
    End If
    If IsCanceled Then
        IsCanceled = False
    End If
    
    SelectedSpecialPower = ""
    
    UnlockGUI
    
    CommandState = "���j�b�g�I��"
End Sub

'�X�y�V�����p���[�R�}���h���I��
' MOD START MARGE
'Public Sub FinishSpecialPowerCommand()
Private Sub FinishSpecialPowerCommand()
' MOD END MARGE
Dim i As Integer, ret As Integer

    LockGUI
    
    '������I�������ꍇ�͊m�F�����
    With SPDList.Item(SelectedSpecialPower)
        For i = 1 To .CountEffect
            If .EffectType(i) = "����" Then
                ret = MsgBox("���������܂����H", _
                    vbOKCancel + vbQuestion, "����")
                If ret = vbCancel Then
                    CommandState = "���j�b�g�I��"
                    UnlockGUI
                    Exit Sub
                End If
                Exit For
            End If
        Next
    End With
    
    '�g�p�C�x���g
    HandleEvent "�g�p", SelectedUnit.MainPilot.ID, SelectedSpecialPower
    If IsScenarioFinished Then
        IsScenarioFinished = False
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    
    '�X�y�V�����p���[���g�p
    If WithDoubleSPConsumption Then
        SelectedPilot.UseSpecialPower SelectedSpecialPower, 2
    Else
        SelectedPilot.UseSpecialPower SelectedSpecialPower
    End If
    Set SelectedUnit = SelectedUnit.CurrentForm
    
    '�X�e�[�^�X�E�B���h�E���X�V
    If Not SelectedTarget Is Nothing Then
        If SelectedTarget.CurrentForm.Status = "�o��" Then
            DisplayUnitStatus SelectedTarget
        End If
    End If
    
    '�g�p��C�x���g
    HandleEvent "�g�p��", SelectedUnit.MainPilot.ID, SelectedSpecialPower
    If IsScenarioFinished Then
        IsScenarioFinished = False
    End If
    If IsCanceled Then
        IsCanceled = False
    End If
    
    SelectedSpecialPower = ""
    
    UnlockGUI
    
    CommandState = "���j�b�g�I��"
End Sub


'�u�C���v�R�}���h���J�n
' MOD START MARGE
'Public Sub StartFixCommand()
Private Sub StartFixCommand()
' MOD END MARGE
Dim i As Integer, j As Integer, k As Integer
Dim t As Unit, u As Unit
Dim fname As String

    SelectedCommand = "�C��"
    
    '�˒��͈́H��\��
    With SelectedUnit
        AreaInRange .X, .Y, 1, 1, "����"
        For i = 1 To MapWidth
            For j = 1 To MapHeight
                If Not MaskData(i, j) _
                    And Not MapDataForUnit(i, j) Is Nothing _
                Then
                    With MapDataForUnit(i, j)
                        If .HP = .MaxHP _
                            Or .IsConditionSatisfied("�]���r") _
                        Then
                            MaskData(i, j) = True
                        End If
                        If .IsFeatureAvailable("�C���s��") Then
                            For k = 2 To .FeatureData("�C���s��")
                                fname = LIndex(.FeatureData("�C���s��"), k)
                                If Left$(fname, 1) = "!" Then
                                    fname = Mid$(fname, 2)
                                    If fname <> SelectedUnit.FeatureName0("�C�����u") Then
                                        MaskData(i, j) = True
                                        Exit For
                                    End If
                                Else
                                    If fname = SelectedUnit.FeatureName0("�C�����u") Then
                                        MaskData(i, j) = True
                                        Exit For
                                    End If
                                End If
                            Next
                        End If
                    End With
                End If
            Next
        Next
        MaskData(.X, .Y) = False
    End With
    MaskScreen
    
    '�J�[�\�������ړ�
    If AutoMoveCursor Then
        Set t = Nothing
        For Each u In UList
            With u
                If .Status = "�o��" And .Party = "����" Then
                    If MaskData(.X, .Y) = False And Not u Is SelectedUnit Then
                        If t Is Nothing Then
                            Set t = u
                        ElseIf u.MaxHP - u.HP > t.MaxHP - t.HP Then
                            Set t = u
                        End If
                    End If
                End If
            End With
        Next
        If t Is Nothing Then
            Set t = SelectedUnit
        End If
        MoveCursorPos "���j�b�g�I��", t
        If Not SelectedUnit Is t Then
            DisplayUnitStatus t
        End If
    End If
    
    If CommandState = "�R�}���h�I��" Then
        CommandState = "�^�[�Q�b�g�I��"
    Else
        CommandState = "�ړ���^�[�Q�b�g�I��"
    End If
End Sub

'�u�C���v�R�}���h���I��
' MOD START MARGE
'Public Sub FinishFixCommand()
Private Sub FinishFixCommand()
' MOD END MARGE
Dim tmp As Long

    LockGUI
    
    OpenMessageForm SelectedTarget, SelectedUnit
    
    With SelectedUnit
        '�I����e��ύX
        Set SelectedUnitForEvent = SelectedUnit
        Set SelectedTargetForEvent = SelectedTarget
        
        '�C�����b�Z�[�W���������
        If .IsMessageDefined("�C��") Then
            .PilotMessage "�C��"
        End If
        If .IsAnimationDefined("�C��", .FeatureName("�C�����u")) Then
            .PlayAnimation "�C��", .FeatureName("�C�����u")
        Else
            .SpecialEffect "�C��", .FeatureName("�C�����u")
        End If
        
        DisplaySysMessage .Nickname & "��" & SelectedTarget.Nickname & "��" _
            & .FeatureName("�C�����u") & "���g�����B"
        
        '�C�������s
        tmp = SelectedTarget.HP
        Select Case .FeatureLevel("�C�����u")
            Case 1, -1
                SelectedTarget.RecoverHP _
                    30 + 3 * SelectedUnit.MainPilot.SkillLevel("�C��")
            Case 2
                SelectedTarget.RecoverHP _
                    50 + 5 * SelectedUnit.MainPilot.SkillLevel("�C��")
            Case 3
                SelectedTarget.RecoverHP 100
        End Select
        If IsNumeric(LIndex(.FeatureData("�C�����u"), 2)) Then
            .EN = .EN - CInt(LIndex(.FeatureData("�C�����u"), 2))
        End If
        
        DrawSysString SelectedTarget.X, SelectedTarget.Y, _
            "+" & Format$(SelectedTarget.HP - tmp)
        UpdateMessageForm SelectedTarget, SelectedUnit
        DisplaySysMessage SelectedTarget.Nickname & "��" _
            & Term("�g�o", SelectedTarget) & "��" _
            & Format$(SelectedTarget.HP - tmp) & "�񕜂����B"
        
        '�o���l�l��
        .GetExp SelectedTarget, "�C��"
        
        If MessageWait < 10000 Then
            Sleep MessageWait
        End If
    End With
    
    CloseMessageForm
    
    '�`�ԕω��̃`�F�b�N
    SelectedTarget.Update
    SelectedTarget.CurrentForm.CheckAutoHyperMode
    SelectedTarget.CurrentForm.CheckAutoNormalMode
    
    '�s���I��
    WaitCommand
End Sub


'�u�⋋�v�R�}���h���J�n
' MOD START MARGE
'Public Sub StartSupplyCommand()
Private Sub StartSupplyCommand()
' MOD END MARGE
Dim i As Integer, j As Integer, k As Integer
Dim t As Unit, u As Unit

    SelectedCommand = "�⋋"
    
    '�˒��͈́H��\��
    With SelectedUnit
        AreaInRange .X, .Y, 1, 1, "����"
        For i = 1 To MapWidth
            For j = 1 To MapHeight
                If Not MaskData(i, j) _
                    And Not MapDataForUnit(i, j) Is Nothing _
                Then
                    MaskData(i, j) = True
                    With MapDataForUnit(i, j)
                        If .EN < .MaxEN _
                            And Not .IsConditionSatisfied("�]���r") _
                        Then
                            MaskData(i, j) = False
                        Else
                            For k = 1 To .CountWeapon
                                If .Bullet(k) < .MaxBullet(k) Then
                                    MaskData(i, j) = False
                                    Exit For
                                End If
                            Next
                            For k = 1 To .CountAbility
                                If .Stock(k) < .MaxStock(k) Then
                                    MaskData(i, j) = False
                                    Exit For
                                End If
                            Next
                        End If
                    End With
                End If
            Next
        Next
        MaskData(.X, .Y) = False
    End With
    MaskScreen
    
    '�J�[�\�������ړ�
    If AutoMoveCursor Then
        Set t = Nothing
        For Each u In UList
            With u
                If .Status = "�o��" And .Party = "����" Then
                    If MaskData(.X, .Y) = False And Not u Is SelectedUnit Then
                        Set t = u
                        Exit For
                    End If
                End If
            End With
        Next
        If t Is Nothing Then
            Set t = SelectedUnit
        End If
        MoveCursorPos "���j�b�g�I��", t
        If Not SelectedUnit Is t Then
            DisplayUnitStatus t
        End If
    End If
    
    If CommandState = "�R�}���h�I��" Then
        CommandState = "�^�[�Q�b�g�I��"
    Else
        CommandState = "�ړ���^�[�Q�b�g�I��"
    End If
End Sub

'�u�⋋�v�R�}���h���I��
' MOD START MARGE
'Public Sub FinishSupplyCommand()
Private Sub FinishSupplyCommand()
' MOD END MARGE

    LockGUI
    
    OpenMessageForm SelectedTarget, SelectedUnit
    
    With SelectedUnit
        '�I����e��ύX
        Set SelectedUnitForEvent = SelectedUnit
        Set SelectedTargetForEvent = SelectedTarget
        
        '�⋋���b�Z�[�W���������
        If .IsMessageDefined("�⋋") Then
            .PilotMessage "�⋋"
        End If
        If .IsAnimationDefined("�⋋", .FeatureName("�⋋���u")) Then
            .PlayAnimation "�⋋", .FeatureName("�⋋���u")
        Else
            .SpecialEffect "�⋋", .FeatureName("�⋋���u")
        End If
        
        DisplaySysMessage .Nickname & "��" & SelectedTarget.Nickname & "��" _
            & .FeatureName("�⋋���u") & "���g�����B"
        
        '�⋋�����{
        SelectedTarget.FullSupply
        SelectedTarget.IncreaseMorale -10
        If IsNumeric(LIndex(.FeatureData("�⋋���u"), 2)) Then
            .EN = .EN - CInt(LIndex(.FeatureData("�⋋���u"), 2))
        End If
        
        UpdateMessageForm SelectedTarget, SelectedUnit
        DisplaySysMessage SelectedTarget.Nickname & "�̒e����" & _
            Term("�d�m", SelectedTarget) & "���S�������B"
        
        '�o���l���l��
        .GetExp SelectedTarget, "�⋋"
        
        If MessageWait < 10000 Then
            Sleep MessageWait
        End If
    End With
    
    '�`�ԕω��̃`�F�b�N
    SelectedTarget.Update
    SelectedTarget.CurrentForm.CheckAutoHyperMode
    SelectedTarget.CurrentForm.CheckAutoNormalMode
    
    CloseMessageForm
    
    '�s���I��
    WaitCommand
End Sub


'�u�`���[�W�v�R�}���h
' MOD START MARGE
'Public Sub ChargeCommand()
Private Sub ChargeCommand()
' MOD END MARGE
Dim ret As Integer
Dim partners() As Unit
Dim i As Integer

    LockGUI
    
    ret = MsgBox("�`���[�W���J�n���܂����H", _
        vbOKCancel + vbQuestion, "�`���[�W�J�n")
    If ret = vbCancel Then
        CancelCommand
        UnlockGUI
        Exit Sub
    End If
    
    '�g�p�C�x���g
    HandleEvent "�g�p", SelectedUnit.MainPilot.ID, "�`���[�W"
    If IsScenarioFinished Then
        IsScenarioFinished = False
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    
    With SelectedUnit
        '�`���[�W�̃��b�Z�[�W��\��
        If .IsMessageDefined("�`���[�W") Then
            OpenMessageForm
            .PilotMessage "�`���[�W"
            CloseMessageForm
        End If
        
        '�A�j���\�����s��
        If .IsAnimationDefined("�`���[�W") Then
            .PlayAnimation "�`���[�W"
        ElseIf .IsSpecialEffectDefined("�`���[�W") Then
            .SpecialEffect "�`���[�W"
        Else
            PlayWave "Charge.wav"
        End If
        
        '�`���[�W�U���̃p�[�g�i�[��T��
        ReDim partners(0)
        For i = 1 To .CountWeapon
            If .IsWeaponClassifiedAs(i, "�b") _
                And .IsWeaponClassifiedAs(i, "��") _
            Then
                If .IsWeaponAvailable(i, "�`���[�W") Then
                    .CombinationPartner "����", i, partners
                    Exit For
                End If
            End If
        Next
        If UBound(partners) = 0 Then
            For i = 1 To .CountAbility
                If .IsAbilityClassifiedAs(i, "�b") _
                    And .IsAbilityClassifiedAs(i, "��") _
                Then
                    If .IsAbilityAvailable(i, "�`���[�W") Then
                        .CombinationPartner "�A�r���e�B", i, partners
                        Exit For
                    End If
                End If
            Next
        End If
        
        '���j�b�g�̏�Ԃ��`���[�W����
        .AddCondition "�`���[�W", 1
        
        '�`���[�W�U���̃p�[�g�i�[���`���[�W���ɂ���
        For i = 1 To UBound(partners)
            With partners(i)
                .AddCondition "�`���[�W", 1
            End With
        Next
    End With
    
    '�g�p��C�x���g
    HandleEvent "�g�p��", SelectedUnit.MainPilot.ID, "�`���[�W"
    If IsScenarioFinished Then
        IsScenarioFinished = False
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    
    UnlockGUI
    
    '�s���I��
    WaitCommand
End Sub

'�u��b�v�R�}���h���J�n
' MOD START MARGE
'Public Sub StartTalkCommand()
Private Sub StartTalkCommand()
' MOD END MARGE
Dim i As Integer, j As Integer
Dim t As Unit

    SelectedCommand = "��b"
    
    Set t = Nothing
    
    '��b�\�ȃ��j�b�g��\��
    With SelectedUnit
        AreaInRange .X, .Y, 1, 1, ""
        For i = 1 To MapWidth
            For j = 1 To MapHeight
                If Not MaskData(i, j) Then
                    If Not MapDataForUnit(i, j) Is Nothing Then
                        If Not IsEventDefined("��b " & .MainPilot.ID _
                            & " " & MapDataForUnit(i, j).MainPilot.ID) _
                        Then
                            MaskData(i, j) = True
                            Set t = MapDataForUnit(i, j)
                        End If
                    End If
                End If
            Next
        Next
        MaskData(.X, .Y) = False
    End With
    MaskScreen
    
    '�J�[�\�������ړ�
    If AutoMoveCursor Then
        If Not t Is Nothing Then
            MoveCursorPos "���j�b�g�I��", t
            DisplayUnitStatus t
        End If
    End If
    
    If CommandState = "�R�}���h�I��" Then
        CommandState = "�^�[�Q�b�g�I��"
    Else
        CommandState = "�ړ���^�[�Q�b�g�I��"
    End If
End Sub

'�u��b�v�R�}���h���I��
' MOD START MARGE
'Public Sub FinishTalkCommand()
Private Sub FinishTalkCommand()
' MOD END MARGE
Dim p As Pilot

    LockGUI
    
    If SelectedUnit.CountPilot > 0 Then
        Set p = SelectedUnit.Pilot(1)
    Else
        Set p = Nothing
    End If
    
    '��b�C�x���g�����{
    HandleEvent "��b", SelectedUnit.MainPilot.ID, _
        SelectedTarget.MainPilot.ID
    
    If IsScenarioFinished Then
        IsScenarioFinished = False
        Exit Sub
    End If
    
    If Not p Is Nothing Then
        If Not p.Unit Is Nothing Then
            Set SelectedUnit = p.Unit
        End If
    End If
    
    UnlockGUI
    
    '�s���I��
    WaitCommand
End Sub

'�u�ό`�v�R�}���h
' MOD START MARGE
'Public Sub TransformCommand()
Private Sub TransformCommand()
' MOD END MARGE
Dim list() As String, list_id() As String
Dim i As Integer
Dim ret As Integer, uname As String, fdata As String
Dim prev_uname As String
    
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        ClearUnitStatus
    End If
    
    LockGUI
    
    fdata = SelectedUnit.FeatureData("�ό`")
    
    If MapFileName = "" Then
        '���j�b�g�X�e�[�^�X�R�}���h�̏ꍇ
        With SelectedUnit
            ReDim list(0)
            ReDim list_id(0)
            '�ό`�\�Ȍ`�Ԉꗗ���쐬
            For i = 2 To LLength(fdata)
                With .OtherForm(LIndex(fdata, i))
                    If .IsAvailable Then
                        ReDim Preserve list(UBound(list) + 1)
                        ReDim Preserve list_id(UBound(list))
                        list(UBound(list)) = .Nickname
                        list_id(UBound(list)) = .Name
                    End If
                End With
            Next
            ReDim ListItemFlag(UBound(list))
            
            '�ό`����`�Ԃ�I��
            If UBound(list) > 1 Then
                TopItem = 1
                ret = ListBox("�ό`", list, "���O", "�J�[�\���ړ�")
                If ret = 0 Then
                    CancelCommand
                    UnlockGUI
                    Exit Sub
                End If
            Else
                ret = 1
            End If
            
            '�ό`�����{
            .Transform .OtherForm(list_id(ret)).Name
            
            '���j�b�g���X�g�̕\�����X�V
            MakeUnitList
            
            '�X�e�[�^�X�E�B���h�E�̕\�����X�V
            DisplayUnitStatus .CurrentForm
            
            '�R�}���h���I��
            UnlockGUI
            CommandState = "���j�b�g�I��"
            Exit Sub
        End With
    End If
    
    '�ό`�\�Ȍ`�Ԃ̈ꗗ���쐬
    ReDim list(0)
    ReDim list_id(0)
    ReDim ListItemFlag(0)
    For i = 2 To LLength(fdata)
        With SelectedUnit.OtherForm(LIndex(fdata, i))
            If .IsAvailable Then
                ReDim Preserve list(UBound(list) + 1)
                ReDim Preserve list_id(UBound(list))
                ReDim Preserve ListItemFlag(UBound(list))
                list(UBound(list)) = .Nickname0
                list_id(UBound(list)) = .Name
                If .IsAbleToEnter(SelectedUnit.X, SelectedUnit.Y) _
                    Or MapFileName = "" _
                Then
                    ListItemFlag(UBound(list)) = False
                Else
                    ListItemFlag(UBound(list)) = True
                End If
            End If
        End With
    Next
    
    '�ό`��̌`�Ԃ�I��
    If UBound(list) = 1 Then
        If ListItemFlag(1) Then
            MsgBox "���̒n�`�ł�" & LIndex(fdata, 1) & "�ł��܂���"
            CancelCommand
            UnlockGUI
            Exit Sub
        End If
        ret = 1
    Else
        TopItem = 1
        If Not SelectedUnit.IsHero Then
            ret = ListBox("�ό`��", list, "���O", "�J�[�\���ړ�")
        Else
            ret = ListBox("�ϐg��", list, "���O", "�J�[�\���ړ�")
        End If
        If ret = 0 Then
            CancelCommand
            UnlockGUI
            Exit Sub
        End If
    End If
    
    uname = list_id(ret)
    
    With SelectedUnit
        '�_�C�A���O�Ń��b�Z�[�W��\�������邽�ߒǉ��p�C���b�g�����炩���ߍ쐬
        With UDList.Item(uname)
            If .IsFeatureAvailable("�ǉ��p�C���b�g") Then
                If Not PList.IsDefined(.FeatureData("�ǉ��p�C���b�g")) Then
                    If Not PDList.IsDefined(.FeatureData("�ǉ��p�C���b�g")) Then
                        ErrorMessage _
                            uname & "�̒ǉ��p�C���b�g�u" & _
                            .FeatureData("�ǉ��p�C���b�g") & _
                            "�v�̃f�[�^��������܂���"
                        TerminateSRC
                    End If
                    PList.Add .FeatureData("�ǉ��p�C���b�g"), _
                        SelectedUnit.MainPilot.Level, SelectedUnit.Party0
                End If
            End If
        End With
        
        '�a�f�l�̕ύX
        If .IsFeatureAvailable("�ό`�a�f�l") Then
            Dim BGM As String
            For i = 1 To .CountFeature
                If .Feature(i) = "�ό`�a�f�l" _
                    And LIndex(.FeatureData(i), 1) = uname _
                Then
                    BGM = SearchMidiFile(Mid$(.FeatureData(i), _
                        InStr(.FeatureData(i), " ") + 1))
                    If Len(BGM) > 0 Then
                        ChangeBGM BGM
                        Sleep 500
                    End If
                    Exit For
                End If
            Next
        End If
        
        '���b�Z�[�W��\��
        If .IsMessageDefined("�ό`(" & .Name & "=>" & uname & ")") _
            Or .IsMessageDefined("�ό`(" & uname & ")") _
            Or .IsMessageDefined("�ό`(" & .FeatureName("�ό`") & ")") _
        Then
            Center .X, .Y
            RefreshScreen
            
            OpenMessageForm
            If .IsMessageDefined("�ό`(" & .Name & "=>" & uname & ")") Then
                .PilotMessage "�ό`(" & .Name & "=>" & uname & ")"
            ElseIf .IsMessageDefined("�ό`(" & uname & ")") Then
                .PilotMessage "�ό`(" & uname & ")"
            ElseIf .IsMessageDefined("�ό`(" & .FeatureName("�ό`") & ")") Then
                .PilotMessage "�ό`(" & .FeatureName("�ό`") & ")"
            End If
            CloseMessageForm
        End If
        
        '�A�j���\��
        If .IsAnimationDefined("�ό`(" & .Name & "=>" & uname & ")") Then
            .PlayAnimation "�ό`(" & .Name & "=>" & uname & ")"
        ElseIf .IsAnimationDefined("�ό`(" & uname & ")") Then
            .PlayAnimation "�ό`(" & uname & ")"
        ElseIf .IsAnimationDefined("�ό`(" & .FeatureName("�ό`") & ")") Then
            .PlayAnimation "�ό`(" & .FeatureName("�ό`") & ")"
        ElseIf .IsSpecialEffectDefined("�ό`(" & .Name & "=>" & uname & ")") Then
            .SpecialEffect "�ό`(" & .Name & "=>" & uname & ")"
        ElseIf .IsSpecialEffectDefined("�ό`(" & uname & ")") Then
            .SpecialEffect "�ό`(" & uname & ")"
        ElseIf .IsSpecialEffectDefined("�ό`(" & .FeatureName("�ό`") & ")") Then
            .SpecialEffect "�ό`(" & .FeatureName("�ό`") & ")"
        End If
    End With
    
    '�ό`
    prev_uname = SelectedUnit.Name
    SelectedUnit.Transform uname
    Set SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
    
    '�ό`���L�����Z������H
    If SelectedUnit.Action = 0 Then
        ret = MsgBox("���̌`�Ԃł͂���ȏ�̍s�����o���܂���B" & vbCr & vbLf _
            & "����ł��ό`���܂����H", _
            vbOKCancel + vbQuestion, "�ό`")
        If ret = vbCancel Then
            SelectedUnit.Transform prev_uname
            Set SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
            If SelectedUnit.IsConditionSatisfied("����") Then
                SelectedUnit.DeleteCondition "����"
            End If
        End If
        RedrawScreen
    End If
    
    '�ό`�C�x���g
    With SelectedUnit.CurrentForm
        HandleEvent "�ό`", .MainPilot.ID, .Name
    End With
    If IsScenarioFinished Then
        IsScenarioFinished = False
        ClearUnitStatus
        RedrawScreen
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    IsCanceled = False
    
    '�n�C�p�[���[�h�E�m�[�}�����[�h�̎����������`�F�b�N
    SelectedUnit.CurrentForm.CheckAutoHyperMode
    SelectedUnit.CurrentForm.CheckAutoNormalMode
    
    '�J�[�\�������ړ�
    If SelectedUnit.Status = "�o��" Then
        If AutoMoveCursor Then
            MoveCursorPos "���j�b�g�I��", SelectedUnit
        End If
        DisplayUnitStatus SelectedUnit
    End If
    
    CommandState = "���j�b�g�I��"
    
    UnlockGUI
End Sub

'�u�n�C�p�[���[�h�v�R�}���h
' MOD START MARGE
'Public Sub HyperModeCommand()
Private Sub HyperModeCommand()
' MOD END MARGE
Dim uname As String, fname As String
Dim i As Integer
    
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        ClearUnitStatus
    End If
    
    LockGUI
    
    uname = LIndex(SelectedUnit.FeatureData("�n�C�p�[���[�h"), 2)
    fname = SelectedUnit.FeatureName("�n�C�p�[���[�h")
    
    If MapFileName = "" Then
        '���j�b�g�X�e�[�^�X�R�}���h�̏ꍇ
        With SelectedUnit
            If Not .IsFeatureAvailable("�n�C�p�[���[�h") Then
                uname = LIndex(SelectedUnit.FeatureData("�m�[�}�����[�h"), 1)
            End If
            
            '�n�C�p�[���[�h�𔭓�
            .Transform uname
            
            '���j�b�g���X�g�̕\�����X�V
            MakeUnitList
            
            '�X�e�[�^�X�E�B���h�E�̕\�����X�V
            DisplayUnitStatus .CurrentForm
            
            '�R�}���h���I��
            UnlockGUI
            CommandState = "���j�b�g�I��"
            Exit Sub
        End With
    End If
    
    '�n�C�p�[���[�h�𔭓��\���ǂ����`�F�b�N
    With SelectedUnit.OtherForm(uname)
        If Not .IsAbleToEnter(SelectedUnit.X, SelectedUnit.Y) _
            And MapFileName <> "" _
        Then
            MsgBox "���̒n�`�ł͕ό`�ł��܂���"
            UnlockGUI
            CancelCommand
            Exit Sub
        End If
    End With
    
    '�_�C�A���O�Ń��b�Z�[�W��\�������邽�ߒǉ��p�C���b�g�����炩���ߍ쐬
    With UDList.Item(uname)
        If .IsFeatureAvailable("�ǉ��p�C���b�g") Then
            If Not PList.IsDefined(.FeatureData("�ǉ��p�C���b�g")) Then
                If Not PDList.IsDefined(.FeatureData("�ǉ��p�C���b�g")) Then
                    ErrorMessage _
                        uname & "�̒ǉ��p�C���b�g�u" & _
                        .FeatureData("�ǉ��p�C���b�g") & _
                        "�v�̃f�[�^��������܂���"
                    TerminateSRC
                End If
                PList.Add .FeatureData("�ǉ��p�C���b�g"), _
                    SelectedUnit.MainPilot.Level, SelectedUnit.Party0
            End If
        End If
    End With
    
    With SelectedUnit
        '�a�f�l��ύX
        If .IsFeatureAvailable("�n�C�p�[���[�h�a�f�l") Then
            Dim BGM As String
            For i = 1 To .CountFeature
                If .Feature(i) = "�n�C�p�[���[�h�a�f�l" _
                    And LIndex(.FeatureData(i), 1) = uname _
                Then
                    BGM = SearchMidiFile(Mid$(.FeatureData(i), _
                        InStr(.FeatureData(i), " ") + 1))
                    If Len(BGM) > 0 Then
                        ChangeBGM BGM
                        Sleep 500
                    End If
                    Exit For
                End If
            Next
        End If
        
        '���b�Z�[�W��\��
        If .IsMessageDefined("�n�C�p�[���[�h(" & .Name & "=>" & uname & ")") _
            Or .IsMessageDefined("�n�C�p�[���[�h(" & uname & ")") _
            Or .IsMessageDefined("�n�C�p�[���[�h(" & fname & ")") _
            Or .IsMessageDefined("�n�C�p�[���[�h") _
        Then
            Center .X, .Y
            RefreshScreen
            
            OpenMessageForm
            If .IsMessageDefined("�n�C�p�[���[�h(" & .Name & "=>" & uname & ")") Then
                .PilotMessage "�n�C�p�[���[�h(" & .Name & "=>" & uname & ")"
            ElseIf .IsMessageDefined("�n�C�p�[���[�h(" & uname & ")") Then
                .PilotMessage "�n�C�p�[���[�h(" & uname & ")"
            ElseIf .IsMessageDefined("�n�C�p�[���[�h(" & fname & ")") Then
                .PilotMessage "�n�C�p�[���[�h(" & fname & ")"
            Else
                .PilotMessage "�n�C�p�[���[�h"
            End If
            CloseMessageForm
        End If
        
        '�A�j���\��
        If .IsAnimationDefined("�n�C�p�[���[�h(" & .Name & "=>" & uname & ")") Then
            .PlayAnimation "�n�C�p�[���[�h(" & .Name & "=>" & uname & ")"
        ElseIf .IsAnimationDefined("�n�C�p�[���[�h(" & uname & ")") Then
            .PlayAnimation "�n�C�p�[���[�h(" & uname & ")"
        ElseIf .IsAnimationDefined("�n�C�p�[���[�h(" & fname & ")") Then
            .PlayAnimation "�n�C�p�[���[�h(" & fname & ")"
        ElseIf .IsAnimationDefined("�n�C�p�[���[�h") Then
            .PlayAnimation "�n�C�p�[���[�h"
        ElseIf .IsSpecialEffectDefined("�n�C�p�[���[�h(" & .Name & "=>" & uname & ")") Then
            .SpecialEffect "�n�C�p�[���[�h(" & .Name & "=>" & uname & ")"
        ElseIf .IsSpecialEffectDefined("�n�C�p�[���[�h(" & uname & ")") Then
            .SpecialEffect "�n�C�p�[���[�h(" & uname & ")"
        ElseIf .IsSpecialEffectDefined("�n�C�p�[���[�h(" & fname & ")") Then
            .SpecialEffect "�n�C�p�[���[�h(" & fname & ")"
        Else
            .SpecialEffect "�n�C�p�[���[�h"
        End If
    End With
    
    '�n�C�p�[���[�h����
    SelectedUnit.Transform uname
    
    '�n�C�p�[���[�h�E�m�[�}�����[�h�̎����������`�F�b�N
    SelectedUnit.CurrentForm.CheckAutoHyperMode
    SelectedUnit.CurrentForm.CheckAutoNormalMode
    Set SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
    
    '�ό`�C�x���g
    With SelectedUnit.CurrentForm
        HandleEvent "�ό`", .MainPilot.ID, .Name
    End With
    If IsScenarioFinished Then
        IsScenarioFinished = False
        ClearUnitStatus
        RedrawScreen
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    IsCanceled = False
    
    '�J�[�\�������ړ�
    If SelectedUnit.Status = "�o��" Then
        If AutoMoveCursor Then
            MoveCursorPos "���j�b�g�I��", SelectedUnit
        End If
        DisplayUnitStatus SelectedUnit
    End If
    
    CommandState = "���j�b�g�I��"
    
    UnlockGUI
End Sub

'�u�ϐg�����v�R�}���h
' MOD START MARGE
'Public Sub CancelTransformationCommand()
Private Sub CancelTransformationCommand()
' MOD END MARGE
Dim ret As Integer
    
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        ClearUnitStatus
    End If
    
    LockGUI
    
    With SelectedUnit
        If MapFileName = "" Then
            '���j�b�g�X�e�[�^�X�R�}���h�̏ꍇ
            .Transform LIndex(.FeatureData("�m�[�}�����[�h"), 1)
            MakeUnitList
            DisplayUnitStatus .CurrentForm
            UnlockGUI
            CommandState = "���j�b�g�I��"
            Exit Sub
        End If
        
        If .IsHero Then
            ret = MsgBox("�ϐg���������܂����H", _
                vbOKCancel + vbQuestion, "�ϐg����")
        Else
            ret = MsgBox("���ꃂ�[�h���������܂����H", _
                vbOKCancel + vbQuestion, "���ꃂ�[�h����")
        End If
        If ret = vbCancel Then
            UnlockGUI
            CancelCommand
            Exit Sub
        End If
            
        .Transform LIndex(.FeatureData("�m�[�}�����[�h"), 1)
        Set SelectedUnit = MapDataForUnit(.X, .Y)
    End With
    
    '�J�[�\�������ړ�
    If AutoMoveCursor Then
        MoveCursorPos "���j�b�g�I��", SelectedUnit
    End If
    DisplayUnitStatus SelectedUnit
    
    RedrawScreen
    
    '�ό`�C�x���g
    HandleEvent "�ό`", SelectedUnit.MainPilot.ID, SelectedUnit.Name
    IsScenarioFinished = False
    IsCanceled = False
    
    CommandState = "���j�b�g�I��"
    
    UnlockGUI
End Sub

'�u�����v�R�}���h
' MOD START MARGE
'Public Sub SplitCommand()
Private Sub SplitCommand()
' MOD END MARGE
Dim tname As String, uname As String, fname As String
Dim ret As Integer
Dim BGM As String
    
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        ClearUnitStatus
    End If
    
    LockGUI
    
    If MapFileName = "" Then
        '���j�b�g�X�e�[�^�X�R�}���h�̏ꍇ
        
        '���������{
        With SelectedUnit
            If .IsFeatureAvailable("�p�[�c����") Then
                tname = LIndex(.FeatureData("�p�[�c����"), 2)
                .Transform tname
            Else
                .Split
            End If
            UList.CheckAutoHyperMode
            UList.CheckAutoNormalMode
            DisplayUnitStatus MapDataForUnit(.X, .Y)
        End With
        
        '���j�b�g���X�g�̕\�����X�V
        MakeUnitList
        
        '�R�}���h���I��
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    
    With SelectedUnit
        If .IsFeatureAvailable("�p�[�c����") Then
            '�p�[�c�������s���ꍇ
            
            ret = MsgBox("�p�[�c�𕪗����܂����H", _
                vbOKCancel + vbQuestion, "�p�[�c����")
            If ret = vbCancel Then
                UnlockGUI
                CancelCommand
                Exit Sub
            End If
            
            tname = LIndex(.FeatureData("�p�[�c����"), 2)
            
            If Not .OtherForm(tname).IsAbleToEnter(.X, .Y) Then
                MsgBox "���̒n�`�ł͕����ł��܂���"
                UnlockGUI
                CancelCommand
                Exit Sub
            End If
            
            '�a�f�l�ύX
            If .IsFeatureAvailable("�����a�f�l") Then
                BGM = SearchMidiFile(.FeatureData("�����a�f�l"))
                If Len(BGM) > 0 Then
                    StartBGM .FeatureData("�����a�f�l")
                    Sleep 500
                End If
            End If
            
            fname = .FeatureName("�p�[�c����")
            
            '���b�Z�[�W��\��
            If .IsMessageDefined("����(" & .Name & ")") _
                Or .IsMessageDefined("����(" & fname & ")") _
                Or .IsMessageDefined("����") _
            Then
                Center .X, .Y
                RefreshScreen
                
                OpenMessageForm
                If .IsMessageDefined("����(" & .Name & ")") Then
                    .PilotMessage "����(" & .Name & ")"
                ElseIf .IsMessageDefined("����(" & fname & ")") Then
                    .PilotMessage "����(" & fname & ")"
                Else
                    .PilotMessage "����"
                End If
                CloseMessageForm
            End If
            
            '�A�j���\��
            If .IsAnimationDefined("����(" & .Name & ")") Then
                .PlayAnimation "����(" & .Name & ")"
            ElseIf .IsAnimationDefined("����(" & fname & ")") Then
                .PlayAnimation "����(" & fname & ")"
            ElseIf .IsAnimationDefined("����") Then
                .PlayAnimation "����"
            ElseIf .IsSpecialEffectDefined("����(" & .Name & ")") Then
                .SpecialEffect "����(" & .Name & ")"
            ElseIf .IsSpecialEffectDefined("����(" & fname & ")") Then
                .SpecialEffect "����(" & fname & ")"
            Else
                .SpecialEffect "����"
            End If
            
            '�p�[�c����
            uname = .Name
            .Transform tname
            Set SelectedUnit = MapDataForUnit(.X, .Y)
            DisplayUnitStatus SelectedUnit
        Else
            '�ʏ�̕������s���ꍇ
            
            ret = MsgBox("�������܂����H", _
                vbOKCancel + vbQuestion, "����")
            If ret = vbCancel Then
                UnlockGUI
                CancelCommand
                Exit Sub
            End If
            
            '�a�f�l��ύX
            If .IsFeatureAvailable("�����a�f�l") Then
                BGM = SearchMidiFile(.FeatureData("�����a�f�l"))
                If Len(BGM) > 0 Then
                    StartBGM .FeatureData("�����a�f�l")
                    Sleep 500
                End If
            End If
            
            fname = .FeatureName("����")
            
            '���b�Z�[�W��\��
            If .IsMessageDefined("����(" & .Name & ")") _
                Or .IsMessageDefined("����(" & fname & ")") _
                Or .IsMessageDefined("����") _
            Then
                Center .X, .Y
                RefreshScreen
                
                OpenMessageForm
                If .IsMessageDefined("����(" & .Name & ")") Then
                    .PilotMessage "����(" & .Name & ")"
                ElseIf .IsMessageDefined("����(" & fname & ")") Then
                    .PilotMessage "����(" & fname & ")"
                Else
                    .PilotMessage "����"
                End If
                CloseMessageForm
            End If
            
            '�A�j���\��
            If .IsAnimationDefined("����(" & .Name & ")") Then
                .PlayAnimation "����(" & .Name & ")"
            ElseIf .IsAnimationDefined("����(" & fname & ")") Then
                .PlayAnimation "����(" & fname & ")"
            ElseIf .IsAnimationDefined("����") Then
                .PlayAnimation "����"
            ElseIf .IsSpecialEffectDefined("����(" & .Name & ")") Then
                .SpecialEffect "����(" & .Name & ")"
            ElseIf .IsSpecialEffectDefined("����(" & fname & ")") Then
                .SpecialEffect "����(" & fname & ")"
            Else
                .SpecialEffect "����"
            End If
            
            '����
            uname = .Name
            .Split
            
            '�I�����j�b�g���Đݒ�
            Set SelectedUnit = UList.Item(LIndex(.FeatureData("����"), 2))
            
            DisplayUnitStatus SelectedUnit
            
        End If
    End With
    
    '�����C�x���g
    HandleEvent "����", SelectedUnit.MainPilot.ID, uname
    If IsScenarioFinished Then
        IsScenarioFinished = False
        ClearUnitStatus
        RedrawScreen
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    IsCanceled = False
    
    '�J�[�\�������ړ�
    If AutoMoveCursor Then
        MoveCursorPos "���j�b�g�I��", SelectedUnit
    End If
    
    '�n�C�p�[���[�h���m�[�}�����[�h�̎��������`�F�b�N
    UList.CheckAutoHyperMode
    UList.CheckAutoNormalMode
    
    CommandState = "���j�b�g�I��"
    
    UnlockGUI
End Sub

'�u���́v�R�}���h
' MOD START MARGE
'Public Sub CombineCommand()
Private Sub CombineCommand()
' MOD END MARGE
Dim i As Integer, j As Integer
Dim list() As String
Dim u As Unit
    
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        ClearUnitStatus
    End If
    
    LockGUI
    
    ReDim list(0)
    ReDim ListItemFlag(0)
    With SelectedUnit
        If MapFileName = "" Then
            '���j�b�g�X�e�[�^�X�R�}���h�̎�
            '�p�[�c���̂Ȃ�΁c�c
            If MainForm.mnuUnitCommandItem.Item(CombineCmdID).Caption = "�p�[�c����" _
                And .IsFeatureAvailable("�p�[�c����") _
            Then
                '�p�[�c���̂����{
                .Transform .FeatureData("�p�[�c����")
                DisplayUnitStatus MapDataForUnit(.X, .Y)
                MapDataForUnit(.X, .Y).CheckAutoHyperMode
                MapDataForUnit(.X, .Y).CheckAutoNormalMode
                
                '���j�b�g���X�g�̕\�����X�V
                MakeUnitList
                
                '�R�}���h���I��
                CommandState = "���j�b�g�I��"
                UnlockGUI
                Exit Sub
            End If
        End If
        
        '�I���\�ȍ��̃p�^�[���̃��X�g���쐬
        For i = 1 To .CountFeature
            If .Feature(i) = "����" _
                And (LLength(.FeatureData(i)) > 3 Or MapFileName = "") _
            Then
                If Not UList.IsDefined(LIndex(.FeatureData(i), 2)) Then
                    GoTo NextLoop
                End If
                
                For j = 3 To LLength(.FeatureData(i))
                    Set u = UList.Item(LIndex(.FeatureData(i), j))
                    If u Is Nothing Then
                        GoTo NextLoop
                    End If
                    If Not u.IsOperational Then
                        GoTo NextLoop
                    End If
                    If u.Status <> "�o��" _
                        And u.CurrentForm.IsFeatureAvailable("���̐���") _
                    Then
                        GoTo NextLoop
                    End If
                    If MapFileName <> "" Then
                        If Abs(.X - u.CurrentForm.X) > 2 _
                            Or Abs(.Y - u.CurrentForm.Y) > 2 _
                        Then
                            GoTo NextLoop
                        End If
                    End If
                Next
                
                ReDim Preserve list(UBound(list) + 1)
                ReDim ListItemFlag(UBound(list))
                list(UBound(list)) = LIndex(.FeatureData(i), 2)
                ListItemFlag(UBound(list)) = False
            End If
NextLoop:
        Next
        
        '�ǂ̍��̂��s������I��
        If UBound(list) = 1 Then
            i = 1
        Else
            TopItem = 1
            i = ListBox("���̌�̌`��", list, "���O")
            If i = 0 Then
                CancelCommand
                UnlockGUI
                Exit Sub
            End If
        End If
    End With
    
    If MapFileName = "" Then
        '���j�b�g�X�e�[�^�X�R�}���h�̎�
        SelectedUnit.Combine list(i), True
        
        '�n�C�p�[���[�h�E�m�[�}�����[�h�̎����������`�F�b�N
        UList.CheckAutoHyperMode
        UList.CheckAutoNormalMode
        
        '���j�b�g���X�g�̕\�����X�V
        MakeUnitList
        
        '�R�}���h���I��
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    
    '���́I
    SelectedUnit.Combine list(i)
    
    '�n�C�p�[���[�h���m�[�}�����[�h�̎�������
    UList.CheckAutoHyperMode
    UList.CheckAutoNormalMode
    
    '���̌�̃��j�b�g��I�����Ă���
    Set SelectedUnit = MapDataForUnit(SelectedUnit.X, SelectedUnit.Y)
    
    '�s��������
    SelectedUnit.UseAction
    
    '�J�[�\�������ړ�
    If AutoMoveCursor Then
        MoveCursorPos "���j�b�g�I��", SelectedUnit
    End If
    
    DisplayUnitStatus SelectedUnit
    
    '���̃C�x���g
    HandleEvent "����", SelectedUnit.MainPilot.ID, SelectedUnit.Name
    If IsScenarioFinished Then
        IsScenarioFinished = False
        UnlockGUI
        Exit Sub
    End If
    If IsCanceled Then
        IsCanceled = False
        ClearUnitStatus
        RedrawScreen
        CommandState = "���j�b�g�I��"
        UnlockGUI
        Exit Sub
    End If
    
    '�s���I��
    WaitCommand True
End Sub

'�u�����v�R�}���h
' MOD START MARGE
'Public Sub ExchangeFormCommand()
Public Sub ExchangeFormCommand()
' MOD END MARGE
Dim list() As String, id_list() As String
Dim i As Integer, j As Integer, k As Integer, max_value As Long
Dim ret As Integer, fdata As String
Dim farray() As String
    
    LockGUI
    
    With SelectedUnit
        fdata = .FeatureData("����")
        
        '�I���\�Ȋ�����̃��X�g���쐬
        ReDim list(0)
        ReDim id_list(0)
        ReDim ListItemComment(0)
        For i = 1 To LLength(fdata)
            With .OtherForm(LIndex(fdata, i))
                If .IsAvailable Then
                    ReDim Preserve list(UBound(list) + 1)
                    ReDim Preserve id_list(UBound(list))
                    ReDim Preserve ListItemComment(UBound(list))
                    id_list(UBound(list)) = .Name
                    
                    '�e�`�Ԃ̕\�����e���쐬
                    If SelectedUnit.Nickname0 = .Nickname Then
                        list(UBound(list)) = RightPaddedString(.Name, 27)
                    Else
                        list(UBound(list)) = RightPaddedString(.Nickname0, 27)
                    End If
                    list(UBound(list)) = list(UBound(list)) _
                        & LeftPaddedString(Format$(.MaxHP), 6) _
                        & LeftPaddedString(Format$(.MaxEN), 5) _
                        & LeftPaddedString(Format$(.Armor), 5) _
                        & LeftPaddedString(Format$(.Mobility), 5) _
                        & " " & .Data.Adaption
                    
                    '�ő�U����
                    max_value = 0
                    For j = 1 To .CountWeapon
                        If .IsWeaponMastered(j) _
                            And Not .IsDisabled(.Weapon(j).Name) _
                            And Not .IsWeaponClassifiedAs(j, "��") _
                        Then
                            If .WeaponPower(j, "") > max_value Then
                                max_value = .WeaponPower(j, "")
                            End If
                        End If
                    Next
                    list(UBound(list)) = list(UBound(list)) _
                        & LeftPaddedString(Format$(max_value), 7)
                    
                    '�ő�˒�
                    max_value = 0
                    For j = 1 To .CountWeapon
                        If .IsWeaponMastered(j) _
                            And Not .IsDisabled(.Weapon(j).Name) _
                            And Not .IsWeaponClassifiedAs(j, "��") _
                        Then
                            If .WeaponMaxRange(j) > max_value Then
                                max_value = .WeaponMaxRange(j)
                            End If
                        End If
                    Next
                    list(UBound(list)) = list(UBound(list)) _
                        & LeftPaddedString(Format$(max_value), 5)
                    
                    '�����悪������\�͈ꗗ
                    ReDim farray(0)
                    For j = 1 To .CountFeature
                        If .FeatureName(j) <> "" Then
                            '�d���������\�͕͂\�����Ȃ��悤�`�F�b�N
                            For k = 1 To UBound(farray)
                                If .FeatureName(j) = farray(k) Then
                                    Exit For
                                End If
                            Next
                            If k > UBound(farray) Then
                                ListItemComment(UBound(list)) = _
                                    ListItemComment(UBound(list)) & .FeatureName(j) & " "
                                ReDim Preserve farray(UBound(farray) + 1)
                                farray(UBound(farray)) = .FeatureName(j)
                            End If
                        End If
                    Next
                End If
            End With
        Next
        ReDim ListItemFlag(UBound(list))
        
        '�ǂ̌`�ԂɊ������邩��I��
        TopItem = 1
        ret = ListBox("�ύX��I��", list, _
            "���j�b�g                     " _
                & Term("�g�o", Nothing, 4) & " " & Term("�d�m", Nothing, 4) & " " _
                & Term("���b", Nothing, 4) & " " & Term("�^��", Nothing, 4) & " " _
                & "�K�� �U���� �˒�", _
            "�J�[�\���ړ�,�R�����g")
        If ret = 0 Then
            CancelCommand
            UnlockGUI
            Exit Sub
        End If
        
        '���������{
        .Transform .OtherForm(id_list(ret)).Name
        
        '���j�b�g���X�g�̍č\�z
        MakeUnitList
        
        '�J�[�\�������ړ�
        If AutoMoveCursor Then
            MoveCursorPos "���j�b�g�I��", .CurrentForm
        End If
        DisplayUnitStatus .CurrentForm
    End With
    
    CommandState = "���j�b�g�I��"
    
    UnlockGUI
End Sub


'�u���i�v�R�}���h���J�n
' MOD START MARGE
'Public Sub StartLaunchCommand()
Private Sub StartLaunchCommand()
' MOD END MARGE
Dim i As Integer, ret As Integer
Dim list() As String
    
    With SelectedUnit
        ReDim list(.CountUnitOnBoard)
        ReDim ListItemID(.CountUnitOnBoard)
        ReDim ListItemFlag(.CountUnitOnBoard)
    End With
    
    '��͂ɓ��ڂ��Ă��郆�j�b�g�̈ꗗ���쐬
    For i = 1 To SelectedUnit.CountUnitOnBoard
        With SelectedUnit.UnitOnBoard(i)
            list(i) = _
                RightPaddedString(.Nickname0, 25) _
                & RightPaddedString(.MainPilot.Nickname, 17) _
                & LeftPaddedString(Format$(.MainPilot.Level), 2) & " " _
                & RightPaddedString(Format$(.HP) & "/" & Format$(.MaxHP), 12) _
                & RightPaddedString(Format$(.EN) & "/" & Format$(.MaxEN), 8)
            ListItemID(i) = .ID
            If .Action > 0 Then
                ListItemFlag(i) = False
            Else
                ListItemFlag(i) = True
            End If
        End With
    Next
    
    '�ǂ̃��j�b�g�𔭐i�����邩�I��
    TopItem = 1
    ret = ListBox("���j�b�g�I��", list, _
        "���j�b�g��               �p�C���b�g       Lv " & Term("�g�o", Nothing, 8) & Term("�d�m"), _
        "�J�[�\���ړ�")
    
    '�L�����Z�����ꂽ�H
    If ret = 0 Then
        ReDim ListItemID(0)
        CancelCommand
        Exit Sub
    End If
    
    SelectedCommand = "���i"
    
    '���j�b�g�̔��i����
    Set SelectedTarget = UList.Item(ListItemID(ret))
    With SelectedTarget
        .X = SelectedUnit.X
        .Y = SelectedUnit.Y
        
        If .IsFeatureAvailable("�e���|�[�g") _
            And (.Data.Speed = 0 _
                Or LIndex(.FeatureData("�e���|�[�g"), 2) = "0") _
        Then
            '�e���|�[�g�ɂ�锭�i
            AreaInTeleport SelectedTarget
        ElseIf .IsFeatureAvailable("�W�����v") _
            And (.Data.Speed = 0 _
                Or LLength(.FeatureData("�W�����v")) < 2 _
                Or LIndex(.FeatureData("�W�����v"), 2) = "0") _
        Then
            '�W�����v�ɂ�锭�i
            AreaInSpeed SelectedTarget, True
        Else
            '�ʏ�ړ��ɂ�锭�i
            AreaInSpeed SelectedTarget
        End If
        
        '��͂𒆉��\��
        Center .X, .Y
        
        '���i�����郆�j�b�g���͂̑���ɕ\��
        If .BitmapID = 0 Then
            With UList.Item(.Name)
                If SelectedTarget.Party0 = .Party0 And .BitmapID <> 0 _
                    And SelectedTarget.Bitmap = .Bitmap _
                Then
                    SelectedTarget.BitmapID = .BitmapID
                Else
                    SelectedTarget.BitmapID = MakeUnitBitmap(SelectedTarget)
                End If
            End With
        End If
        
        MaskScreen
    End With
    
    ReDim ListItemID(0)
    
    If CommandState = "�R�}���h�I��" Then
        CommandState = "�^�[�Q�b�g�I��"
    End If
End Sub

'�u���i�v�R�}���h���I��
' MOD START MARGE
'Public Sub FinishLaunchCommand()
Private Sub FinishLaunchCommand()
' MOD END MARGE
Dim ret As Integer

    LockGUI
    
    With SelectedTarget
        '���i�R�}���h�̖ړI�n�Ƀ��j�b�g�������ꍇ
        If Not MapDataForUnit(SelectedX, SelectedY) Is Nothing Then
            If MapDataForUnit(SelectedX, SelectedY).IsFeatureAvailable("���") Then
                ret = MsgBox("���͂��܂����H", _
                    vbOKCancel + vbQuestion, "����")
            Else
                ret = MsgBox("���̂��܂����H", _
                    vbOKCancel + vbQuestion, "����")
            End If
            If ret = vbCancel Then
                CancelCommand
                UnlockGUI
                Exit Sub
            End If
        End If
        
        '���b�Z�[�W�̕\��
        If .IsMessageDefined("���i(" & .Name & ")") Then
            OpenMessageForm
            .PilotMessage "���i(" & .Name & ")"
            CloseMessageForm
        ElseIf .IsMessageDefined("���i") Then
            OpenMessageForm
            .PilotMessage "���i"
            CloseMessageForm
        End If
        
        .SpecialEffect "���i", .Name
        
        PrevUnitArea = .Area
        PrevUnitEN = .EN
        .Status = "�o��"
        
        '�w�肵���ʒu�ɔ��i�������j�b�g���ړ�
        .Move SelectedX, SelectedY
    End With
    
    '���i�������j�b�g���͂���~�낷
    With SelectedUnit
        PrevUnitX = .X
        PrevUnitY = .Y
        .UnloadUnit SelectedTarget.ID
        
        '��͂̈ʒu�ɂ͔��i�������j�b�g���\������Ă���̂Ō��ɖ߂��Ă���
        Set MapDataForUnit(.X, .Y) = SelectedUnit
        PaintUnitBitmap SelectedUnit
    End With
    
    Set SelectedUnit = SelectedTarget
    With SelectedUnit
        If MapDataForUnit(.X, .Y).ID <> .ID Then
            RedrawScreen
            CommandState = "���j�b�g�I��"
            UnlockGUI
            Exit Sub
        End If
    End With
    
    CommandState = "�ړ���R�}���h�I��"
    
    UnlockGUI
    ProceedCommand
End Sub


'�u���߁v�R�}���h���J�n
' MOD START MARGE
'Public Sub StartOrderCommand()
Private Sub StartOrderCommand()
' MOD END MARGE
Dim list() As String
Dim ret As Integer, i As Integer, j As Integer

    LockGUI
    
    ReDim list(4)
    ReDim ListItemFlag(4)
    
    '�\�Ȗ��ߓ��e�ꗗ���쐬
    list(1) = "���R�F���R�ɍs��������"
    list(2) = "�ړ��F�w�肵���ʒu�Ɉړ�"
    list(3) = "�U���F�w�肵���G���U��"
    list(4) = "��q�F�w�肵�����j�b�g����q"
    If Not SelectedUnit.Summoner Is Nothing _
        Or Not SelectedUnit.Master Is Nothing _
    Then
        ReDim Preserve list(5)
        ReDim Preserve ListItemFlag(5)
        If Not SelectedUnit.Master Is Nothing Then
            list(5) = "�A�ҁF��l�̏��ɖ߂�"
        Else
            list(5) = "�A�ҁF������̏��ɖ߂�"
        End If
    End If
    
    '���߂���s���p�^�[����I��
    ret = ListBox("����", list, "�s���p�^�[��")
    
    '�I�����ꂽ�s���p�^�[���ɉ����ă^�[�Q�b�g�̈��\��
    Select Case ret
        Case 0
            CancelCommand
        Case 1 '���R
            SelectedUnit.Mode = "�ʏ�"
            CommandState = "���j�b�g�I��"
            DisplayUnitStatus SelectedUnit
        Case 2 '�ړ�
            SelectedCommand = "�ړ�����"
            For i = 1 To MapWidth
                For j = 1 To MapHeight
                    MaskData(i, j) = False
                Next
            Next
            MaskScreen
            CommandState = "�^�[�Q�b�g�I��"
        Case 3 '�U��
            SelectedCommand = "�U������"
            AreaWithUnit "�����̓G"
            MaskData(SelectedUnit.X, SelectedUnit.Y) = True
            MaskScreen
            CommandState = "�^�[�Q�b�g�I��"
        Case 4 '��q
            SelectedCommand = "��q����"
            AreaWithUnit "����"
            MaskData(SelectedUnit.X, SelectedUnit.Y) = True
            MaskScreen
            CommandState = "�^�[�Q�b�g�I��"
        Case 5 '�A��
            If Not SelectedUnit.Master Is Nothing Then
                SelectedUnit.Mode = SelectedUnit.Master.MainPilot.ID
            Else
                SelectedUnit.Mode = SelectedUnit.Summoner.MainPilot.ID
            End If
            CommandState = "���j�b�g�I��"
            DisplayUnitStatus SelectedUnit
    End Select
    
    UnlockGUI
End Sub

'�u���߁v�R�}���h���I��
' MOD START MARGE
'Public Sub FinishOrderCommand()
Private Sub FinishOrderCommand()
' MOD END MARGE
    Select Case SelectedCommand
        Case "�ړ�����"
            SelectedUnit.Mode = Format$(SelectedX) & " " & Format$(SelectedY)
        Case "�U������", "��q����"
            SelectedUnit.Mode = SelectedTarget.MainPilot.ID
    End Select
    If DisplayedUnit Is SelectedUnit Then
        DisplayUnitStatus SelectedUnit
    End If
    
    RedrawScreen
    
    CommandState = "���j�b�g�I��"
End Sub


'�u����\�͈ꗗ�v�R�}���h
' MOD START MARGE
'Public Sub FeatureListCommand()
Private Sub FeatureListCommand()
' MOD END MARGE
Dim list() As String, id_list() As String
Dim is_unit_feature() As Boolean
Dim i As Integer, j As Integer
Dim ret As Integer
Dim fname As String, fname0 As String, ftype As String
    
    LockGUI
    
    '�\���������\�͖��ꗗ�̍쐬
    ReDim list(0)
    ReDim id_ist(0)
    ReDim is_unit_feature(0)
    
    '����E�h��N���X
    If IsOptionDefined("�A�C�e������") Then
        With SelectedUnit
            If .IsFeatureAvailable("����N���X") _
                Or .IsFeatureAvailable("�h��N���X") _
            Then
                ReDim Preserve list(UBound(list) + 1)
                ReDim Preserve id_list(UBound(list))
                ReDim Preserve is_unit_feature(UBound(list))
                list(UBound(list)) = "����E�h��N���X"
                id_list(UBound(list)) = "����E�h��N���X"
                is_unit_feature(UBound(list)) = True
            End If
        End With
    End If
    
    With SelectedUnit.MainPilot
        '�p�C���b�g����\��
        For i = 1 To .CountSkill
            Select Case .Skill(i)
                Case "���ӋZ", "�s����"
                    fname = .Skill(i)
                Case Else
                    fname = .SkillName(i)
            End Select
            
            '��\���̔\�͂͏���
            If InStr(fname, "��\��") > 0 Then
                GoTo NextSkill
            End If
            
            '���ɕ\������Ă���΃X�L�b�v
            For j = 1 To UBound(list)
                If list(j) = fname Then
                    GoTo NextSkill
                End If
            Next
            
            '���X�g�ɒǉ�
            ReDim Preserve list(UBound(list) + 1)
            ReDim Preserve id_list(UBound(list))
            list(UBound(list)) = fname
            id_list(UBound(list)) = Format$(i)
NextSkill:
        Next
    End With
    With SelectedUnit
        '�t���E�������ꂽ�p�C���b�g�p����\��
        For i = 1 To .CountCondition
            '�p�C���b�g�\�͕t���܂��͋����H
            If Right$(.Condition(i), 3) <> "�t���Q" _
                And Right$(.Condition(i), 3) <> "�����Q" _
            Then
                GoTo NextSkill2
            End If
            
            ftype = Left$(.Condition(i), Len(.Condition(i)) - 3)
            
            '��\���̔\�́H
            Select Case LIndex(.ConditionData(i), 1)
                Case "��\��", "���"
                    GoTo NextSkill2
            End Select
            
            '�L�����Ԃ��c���Ă���H
            If .ConditionLifetime(i) = 0 Then
                GoTo NextSkill2
            End If
            
            '�\������
            fname = .MainPilot.SkillName(ftype)
            If InStr(fname, "��\��") > 0 Then
                GoTo NextSkill2
            End If
            
            '���ɕ\�����Ă���΃X�L�b�v
            For j = 1 To UBound(list)
                If list(j) = fname Then
                    GoTo NextSkill2
                End If
            Next
            
            '���X�g�ɒǉ�
            ReDim Preserve list(UBound(list) + 1)
            ReDim Preserve id_list(UBound(list))
            list(UBound(list)) = fname
            id_list(UBound(list)) = ftype
NextSkill2:
        Next
        ReDim Preserve is_unit_feature(UBound(list))
        
        '���j�b�g�p����\��
        '�t�����ꂽ����\�͂���ɌŗL�̓���\�͂�\��
        If .CountAllFeature > .AdditionalFeaturesNum Then
            i = .AdditionalFeaturesNum + 1
        Else
            i = 1
        End If
        Do While i <= .CountAllFeature
            '��\���̓���\�͂�r��
            If .AllFeatureName(i) = "" Then
                GoTo NextFeature
            End If
            
            '���̂̏ꍇ�͍��̌�̌`�Ԃ��쐬����Ă��Ȃ���΂Ȃ�Ȃ�
            If .AllFeature(i) = "����" _
                And Not UList.IsDefined(LIndex(.AllFeatureData(i), 2)) _
            Then
                GoTo NextFeature
            End If
            
            '���ɕ\�����Ă���΃X�L�b�v
            For j = 1 To UBound(list)
                If list(j) = .AllFeatureName(i) Then
                    GoTo NextFeature
                End If
            Next
            
            '���X�g�ɒǉ�
            ReDim Preserve list(UBound(list) + 1)
            ReDim Preserve id_list(UBound(list))
            ReDim Preserve is_unit_feature(UBound(list))
            list(UBound(list)) = .AllFeatureName(i)
            id_list(UBound(list)) = Format$(i)
            is_unit_feature(UBound(list)) = True
NextFeature:
            If i = .AdditionalFeaturesNum Then
                Exit Do
            ElseIf i = .CountFeature Then
                '�t�����ꂽ����\�͂͌ォ��\��
                If .AdditionalFeaturesNum > 0 Then
                    i = 0
                End If
            End If
            i = i + 1
        Loop
        
        '�A�r���e�B�ŕt���E�������ꂽ�p�C���b�g�p����\��
        For i = 1 To .CountCondition
            '�p�C���b�g�\�͕t���܂��͋����H
            If Right$(.Condition(i), 2) <> "�t��" _
                And Right$(.Condition(i), 2) <> "����" _
            Then
                GoTo NextSkill3
            End If
            
            ftype = Left$(.Condition(i), Len(.Condition(i)) - 2)
            
            '��\���̔\�́H
            If ftype = "���b�Z�[�W" Then
                GoTo NextSkill3
            End If
            Select Case LIndex(.ConditionData(i), 1)
                Case "��\��", "���"
                    GoTo NextSkill3
            End Select
            
            '�L�����Ԃ��c���Ă���H
            If .ConditionLifetime(i) = 0 Then
                GoTo NextSkill3
            End If
            
            '�\������
            If .FeatureName0(ftype) = "" Then
                GoTo NextSkill3
            End If
            fname = .MainPilot.SkillName0(ftype)
            If InStr(fname, "��\��") > 0 Then
                GoTo NextSkill3
            End If
            
            '�t�����ꂽ���j�b�g�p����\�͂Ƃ��Ċ��ɕ\�����Ă���΃X�L�b�v
            For j = 1 To UBound(list)
                If list(j) = fname Then
                    GoTo NextSkill3
                End If
            Next
            
            fname = .MainPilot.SkillName(ftype)
            If InStr(fname, "Lv") > 0 Then
                fname0 = Left$(fname, InStr(fname, "Lv") - 1)
            Else
                fname0 = fname
            End If
            
            '�p�C���b�g�p����\�͂Ƃ��Ċ��ɕ\�����Ă���΃X�L�b�v
            For j = 1 To UBound(list)
                If list(j) = fname Or list(j) = fname0 Then
                    GoTo NextSkill3
                End If
            Next
            
            '���X�g�ɒǉ�
            ReDim Preserve list(UBound(list) + 1)
            ReDim Preserve id_list(UBound(list))
            ReDim Preserve is_unit_feature(UBound(list))
            list(UBound(list)) = fname
            id_list(UBound(list)) = ftype
            is_unit_feature(UBound(list)) = False
NextSkill3:
        Next
    End With
    ReDim ListItemFlag(UBound(list))
    
    Select Case UBound(list)
        Case 0
        Case 1
            If AutoMoveCursor Then
                SaveCursorPos
            End If
            If id_list(ret) = "����E�h��N���X" Then
                FeatureHelp SelectedUnit, id_list(1), False
            ElseIf is_unit_feature(1) Then
                FeatureHelp SelectedUnit, id_list(1), _
                    StrToLng(id_list(1)) <= SelectedUnit.AdditionalFeaturesNum
            Else
                SkillHelp SelectedUnit.MainPilot, id_list(1)
            End If
            If AutoMoveCursor Then
                RestoreCursorPos
            End If
        Case Else
            TopItem = 1
            ret = ListBox("����\�͈ꗗ", list, "�\�͖�", "�\���̂�")
            If AutoMoveCursor Then
                MoveCursorPos "�_�C�A���O"
            End If
            Do While True
                ret = ListBox("����\�͈ꗗ", list, "�\�͖�", "�A���\��")
                'list�����Ȃ̂ŘA���\���𗬗p
                frmListBox.Hide
                If ret = 0 Then
                    Exit Do
                End If
                If id_list(ret) = "����E�h��N���X" Then
                    FeatureHelp SelectedUnit, id_list(ret), False
                ElseIf is_unit_feature(ret) Then
                    FeatureHelp SelectedUnit, id_list(ret), _
                        id_list(ret) <= SelectedUnit.AdditionalFeaturesNum
                Else
                    SkillHelp SelectedUnit.MainPilot, id_list(ret)
                End If
            Loop
            If AutoMoveCursor Then
                RestoreCursorPos
            End If
    End Select
    
    CommandState = "���j�b�g�I��"
    
    UnlockGUI
End Sub

'�u����ꗗ�v�R�}���h
' MOD START MARGE
'Public Sub WeaponListCommand()
Private Sub WeaponListCommand()
' MOD END MARGE
Dim list() As String
Dim i As Integer, buf As String
Dim w As Integer, wclass As String
Dim atype As String, alevel As String
Dim c As String

    LockGUI
    
    Do While True
        w = WeaponListBox(SelectedUnit, "�����ꗗ", "�ꗗ")
        SelectedWeapon = w
        
        If SelectedWeapon <= 0 Then
            '�L�����Z��
            If AutoMoveCursor Then
                RestoreCursorPos
            End If
            frmListBox.Hide
            UnlockGUI
            CommandState = "���j�b�g�I��"
            Exit Sub
        End If
        
        '�w�肳�ꂽ����̑����ꗗ���쐬
        ReDim list(0)
        i = 0
        With SelectedUnit
            wclass = .WeaponClass(w)
            
            Do While i <= Len(wclass)
                i = i + 1
                buf = GetClassBundle(wclass, i)
                atype = ""
                alevel = ""
                
                '��\���H
                If buf = "|" Then
                    Exit Do
                End If
                
                '�l����
                If Mid$(wclass, i, 1) = "�l" Then
                    i = i + 1
                    buf = buf & Mid$(wclass, i, 1)
                End If
                
                '���x���w��
                If Mid$(wclass, i + 1, 1) = "L" Then
                    i = i + 2
                    c = Mid$(wclass, i, 1)
                    Do While IsNumeric(c) Or c = "." Or c = "-"
                        alevel = alevel & c
                        i = i + 1
                        c = Mid$(wclass, i, 1)
                    Loop
                    i = i - 1
                End If
                
                '�����̖���
                atype = AttributeName(SelectedUnit, buf)
                If Len(atype) > 0 Then
                    ReDim Preserve list(UBound(list) + 1)
                    
                    If Len(alevel) > 0 Then
                        list(UBound(list)) = RightPaddedString(buf & "L" & alevel, 8) _
                            & atype & "���x��" & StrConv(alevel, vbWide)
                    Else
                        list(UBound(list)) = RightPaddedString(buf, 8) & atype
                    End If
                End If
            Loop
            
            If MapFileName <> "" Then
                ReDim Preserve list(UBound(list) + 1)
                list(UBound(list)) = "�˒��͈�"
            End If
            
            If UBound(list) > 0 Then
                TopItem = 1
                Do While True
                    If UBound(list) = 1 And list(1) = "�˒��͈�" Then
                        i = 1
                    Else
                        ReDim ListItemFlag(UBound(list))
                        i = ListBox("���푮���ꗗ", list, "����    ����", "�A���\��")
                    End If
                    
                    If i = 0 Then
                        '�L�����Z��
                        Exit Do
                    ElseIf list(i) = "�˒��͈�" Then
                        Dim min_range As Integer, max_range As Integer
                        
                        frmListBox.Hide
                        
                        '����̎˒������߂Ă���
                        min_range = .Weapon(w).MinRange
                        max_range = .WeaponMaxRange(w)
                        
                        '�˒��͈͕\��
                        If (max_range = 1 Or .IsWeaponClassifiedAs(w, "�o")) _
                            And Not .IsWeaponClassifiedAs(w, "�p") _
                        Then
                            AreaInReachable SelectedUnit, max_range, .Party & "�̓G"
                        ElseIf .IsWeaponClassifiedAs(w, "�l��") Then
                            AreaInCross .X, .Y, min_range, max_range
                        ElseIf .IsWeaponClassifiedAs(w, "�l�g") Then
                            AreaInWideCross .X, .Y, min_range, max_range
                        ElseIf .IsWeaponClassifiedAs(w, "�l��") Then
                            AreaInSectorCross .X, .Y, min_range, max_range, _
                                .WeaponLevel(w, "�l��")
                        ElseIf .IsWeaponClassifiedAs(w, "�l�S") _
                            Or .IsWeaponClassifiedAs(w, "�l��") _
                        Then
                            AreaInRange .X, .Y, max_range, min_range, "���ׂ�"
                        ElseIf .IsWeaponClassifiedAs(w, "�l��") Then
                            max_range = max_range + .WeaponLevel(w, "�l��")
                            min_range = min_range - .WeaponLevel(w, "�l��")
                            min_range = MaxLng(min_range, 1)
                            AreaInRange .X, .Y, max_range, min_range, "���ׂ�"
                        ElseIf .IsWeaponClassifiedAs(w, "�l��") Then
                            AreaInMoveAction SelectedUnit, max_range
                        Else
                            AreaInRange .X, .Y, max_range, min_range, .Party & "�̓G"
                        End If
                        Center .X, .Y
                        MaskScreen
                        
                        '��s���͂���Ă����N���b�N�C�x���g������
                        DoEvents
                        WaitClickMode = True
                        IsFormClicked = False
                        
                        '�N���b�N�����܂ő҂�
                        Do Until IsFormClicked
                            Sleep 25
                            DoEvents
                            
                            If IsRButtonPressed(True) Then
                                Exit Do
                            End If
                        Loop
                        
                        RedrawScreen
                        
                        If UBound(list) = 1 And list(i) = "�˒��͈�" Then
                            Exit Do
                        End If
                    Else
                        '�w�肳�ꂽ�����̉����\��
                        frmListBox.Hide
                        AttributeHelp SelectedUnit, LIndex(list(i), 1), w
                    End If
                Loop
            End If
        End With
    Loop
End Sub

'�u�A�r���e�B�ꗗ�v�R�}���h
' MOD START MARGE
'Public Sub AbilityListCommand()
Private Sub AbilityListCommand()
' MOD END MARGE
Dim list() As String
Dim i As Integer, buf As String
Dim a As Integer, atype As String, alevel As String, aclass As String
Dim c As String

    LockGUI
    
    Do While True
        a = AbilityListBox(SelectedUnit, Term("�A�r���e�B", SelectedUnit) & "�ꗗ", "�ꗗ")
        SelectedAbility = a
        
        If SelectedAbility <= 0 Then
            '�L�����Z��
            If AutoMoveCursor Then
                RestoreCursorPos
            End If
            frmListBox.Hide
            UnlockGUI
            CommandState = "���j�b�g�I��"
            Exit Sub
        End If
        
        '�w�肳�ꂽ�A�r���e�B�̑����ꗗ���쐬
        ReDim list(0)
        i = 0
        With SelectedUnit
            aclass = .Ability(a).Class
            
            Do While i <= Len(aclass)
                i = i + 1
                buf = GetClassBundle(aclass, i)
                atype = ""
                alevel = ""
                
                '��\���H
                If buf = "|" Then
                    Exit Do
                End If
                
                '�l����
                If Mid$(aclass, i, 1) = "�l" Then
                    i = i + 1
                    buf = buf & Mid$(aclass, i, 1)
                End If
                
                '���x���w��
                If Mid$(aclass, i + 1, 1) = "L" Then
                    i = i + 2
                    c = Mid$(aclass, i, 1)
                    Do While IsNumeric(c) Or c = "." Or c = "-"
                        alevel = alevel & c
                        i = i + 1
                        c = Mid$(aclass, i, 1)
                    Loop
                    i = i - 1
                End If
                
                '�����̖���
                atype = AttributeName(SelectedUnit, buf, True)
                If Len(atype) > 0 Then
                    ReDim Preserve list(UBound(list) + 1)
                    
                    If Len(alevel) > 0 Then
                        list(UBound(list)) = RightPaddedString(buf & "L" & alevel, 8) _
                            & atype & "���x��" & StrConv(alevel, vbWide)
                    Else
                        list(UBound(list)) = RightPaddedString(buf, 8) & atype
                    End If
                End If
            Loop
            
            If MapFileName <> "" Then
                ReDim Preserve list(UBound(list) + 1)
                list(UBound(list)) = "�˒��͈�"
            End If
            
            If UBound(list) > 0 Then
                TopItem = 1
                Do While True
                    If UBound(list) = 1 And list(1) = "�˒��͈�" Then
                        i = 1
                    Else
                        ReDim ListItemFlag(UBound(list))
                        i = ListBox("�A�r���e�B�����ꗗ", list, "����    ����", "�A���\��")
                    End If
                    
                    If i = 0 Then
                        '�L�����Z��
                        Exit Do
                    ElseIf list(i) = "�˒��͈�" Then
                        Dim min_range As Integer, max_range As Integer
                        
                        frmListBox.Hide
                        
                        '�A�r���e�B�̎˒������߂Ă���
                        min_range = .AbilityMinRange(a)
                        max_range = .AbilityMaxRange(a)
                        
                        '�˒��͈͕\��
                        If (max_range = 1 Or .IsAbilityClassifiedAs(a, "�o")) _
                            And Not .IsAbilityClassifiedAs(a, "�p") _
                        Then
                            AreaInReachable SelectedUnit, max_range, "���ׂ�"
                        ElseIf .IsAbilityClassifiedAs(a, "�l��") Then
                            AreaInCross .X, .Y, min_range, max_range
                        ElseIf .IsAbilityClassifiedAs(a, "�l�g") Then
                            AreaInWideCross .X, .Y, min_range, max_range
                        ElseIf .IsAbilityClassifiedAs(a, "�l��") Then
                            AreaInSectorCross .X, .Y, min_range, max_range, _
                                .AbilityLevel(a, "�l��")
                        ElseIf .IsAbilityClassifiedAs(a, "�l��") Then
                            max_range = max_range + .AbilityLevel(a, "�l��")
                            min_range = min_range - .AbilityLevel(a, "�l��")
                            min_range = MaxLng(min_range, 1)
                            AreaInRange .X, .Y, max_range, min_range, "���ׂ�"
                        ElseIf .IsAbilityClassifiedAs(a, "�l��") Then
                            AreaInMoveAction SelectedUnit, max_range
                        Else
                            AreaInRange .X, .Y, max_range, min_range, "���ׂ�"
                        End If
                        Center .X, .Y
                        MaskScreen
                        
                        '��s���͂���Ă����N���b�N�C�x���g������
                        DoEvents
                        WaitClickMode = True
                        IsFormClicked = False
                        
                        '�N���b�N�����܂ő҂�
                        Do Until IsFormClicked
                            Sleep 25
                            DoEvents
                            
                            If IsRButtonPressed(True) Then
                                Exit Do
                            End If
                        Loop
                        
                        RedrawScreen
                        
                        If UBound(list) = 1 And list(i) = "�˒��͈�" Then
                            Exit Do
                        End If
                    Else
                        '�w�肳�ꂽ�����̉����\��
                        frmListBox.Hide
                        AttributeHelp SelectedUnit, LIndex(list(i), 1), a, True
                    End If
                Loop
            End If
        End With
    Loop
End Sub

'�u�ړ��͈́v�R�}���h
' MOD START MARGE
'Public Sub ShowAreaInSpeedCommand()
Private Sub ShowAreaInSpeedCommand()
' MOD END MARGE
    SelectedCommand = "�ړ��͈�"
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        ClearUnitStatus
    End If
    AreaInSpeed SelectedUnit
    Center SelectedUnit.X, SelectedUnit.Y
    MaskScreen
    CommandState = "�^�[�Q�b�g�I��"
End Sub

'�u�˒��͈́v�R�}���h
' MOD START MARGE
'Public Sub ShowAreaInRangeCommand()
Private Sub ShowAreaInRangeCommand()
' MOD END MARGE
Dim i As Integer, w As Integer, max_range As Integer

    SelectedCommand = "�˒��͈�"
    
' MOD START MARGE
'    If MainWidth <> 15 Then
    If NewGUIMode Then
' MOD END MARGE
        ClearUnitStatus
    End If
    
    With SelectedUnit
        '�ő�̎˒����������T��
        w = 0
        max_range = 0
        For i = 1 To .CountWeapon
            If .IsWeaponAvailable(i, "�X�e�[�^�X") _
                And Not .IsWeaponClassifiedAs(i, "�l") _
            Then
                If .WeaponMaxRange(i) > max_range Then
                    w = i
                    max_range = .WeaponMaxRange(i)
                End If
            End If
        Next
        
        '���������ő�̎˒���������̎˒��͈͂�I��
        AreaInRange .X, .Y, max_range, 1, .Party & "�̓G"
        
        '�˒��͈͂�\��
        Center .X, .Y
        MaskScreen
    End With
    
    CommandState = "�^�[�Q�b�g�I��"
End Sub

'�u�ҋ@�v�R�}���h
'���̃R�}���h�̏I�������ɂ��g����
' MOD START MARGE
'Public Sub WaitCommand(Optional ByVal WithoutAction As Boolean)
' ����ǂ����Ă�Private����_���ȏ������o����߂��Ă�������
Private Sub WaitCommand(Optional ByVal WithoutAction As Boolean)
' MOD END MARGE
Dim p As Pilot, i As Integer
    
    '�R�}���h�I�����̓^�[�Q�b�g������
    Set SelectedTarget = Nothing
    
    '���j�b�g�Ƀp�C���b�g������Ă��Ȃ��H
    If SelectedUnit.CountPilot = 0 Then
        CommandState = "���j�b�g�I��"
        RedrawScreen
        ClearUnitStatus
        Exit Sub
    End If
    
    If Not WithoutAction Then
        '�c��s����������������
        SelectedUnit.UseAction
        
        '�������Ԃ��u�ړ��v�̃X�y�V�����p���[���ʂ��폜
        If InStr(CommandState, "�ړ���") > 0 Then
            SelectedUnit.RemoveSpecialPowerInEffect "�ړ�"
        End If
    End If
    
    CommandState = "���j�b�g�I��"
    
    '�A�b�v�f�[�g
    SelectedUnit.Update
    PList.UpdateSupportMod SelectedUnit
    
    '���j�b�g�����ɏo�����Ă��Ȃ��H
    If SelectedUnit.Status <> "�o��" Then
        RedrawScreen
        ClearUnitStatus
        Exit Sub
    End If
    
    LockGUI
    
    RedrawScreen
    
    Set p = SelectedUnit.Pilot(1)
    
    '�ڐG�C�x���g
    For i = 1 To 4
        Set SelectedTarget = Nothing
        With SelectedUnit
            Select Case i
                Case 1
                    If .X > 1 Then
                        Set SelectedTarget = MapDataForUnit(.X - 1, .Y)
                    End If
                Case 2
                    If .X < MapWidth Then
                        Set SelectedTarget = MapDataForUnit(.X + 1, .Y)
                    End If
                Case 3
                    If .Y > 1 Then
                        Set SelectedTarget = MapDataForUnit(.X, .Y - 1)
                    End If
                Case 4
                    If .Y < MapHeight Then
                        Set SelectedTarget = MapDataForUnit(.X, .Y + 1)
                    End If
            End Select
        End With
        
        If Not SelectedTarget Is Nothing Then
            HandleEvent "�ڐG", SelectedUnit.MainPilot.ID, SelectedTarget.MainPilot.ID
            Set SelectedTarget = Nothing
            If IsScenarioFinished Then
                IsScenarioFinished = False
                Exit Sub
            End If
            If SelectedUnit.Status <> "�o��" Then
                RedrawScreen
                ClearUnitStatus
                UnlockGUI
                Exit Sub
            End If
        End If
    Next
    
    '�i���C�x���g
    HandleEvent "�i��", SelectedUnit.MainPilot.ID, _
        SelectedUnit.X, SelectedUnit.Y
    If IsScenarioFinished Then
        IsScenarioFinished = False
        Exit Sub
    End If
    If SelectedUnit.CountPilot = 0 Then
        RedrawScreen
        ClearUnitStatus
        UnlockGUI
        Exit Sub
    End If
    
    '�s���I���C�x���g
    HandleEvent "�s���I��", SelectedUnit.MainPilot.ID
    If IsScenarioFinished Then
        IsScenarioFinished = False
        Exit Sub
    End If
    If SelectedUnit.CountPilot = 0 Then
        RedrawScreen
        ClearUnitStatus
        UnlockGUI
        Exit Sub
    End If
    
    If Not p.Unit Is Nothing Then
        Set SelectedUnit = p.Unit
    End If
    
    If SelectedUnit.Action > 0 And SelectedUnit.CountPilot > 0 Then
        '�J�[�\�������ړ�
        If AutoMoveCursor Then
            MoveCursorPos "���j�b�g�I��", SelectedUnit
        End If
    End If
    
    '�n�C�p�[���[�h�E�m�[�}�����[�h�̎����������`�F�b�N
    SelectedUnit.CurrentForm.CheckAutoHyperMode
    SelectedUnit.CurrentForm.CheckAutoNormalMode
    
    If IsPictureVisible Or IsCursorVisible Then
        RedrawScreen
    End If
    
    UnlockGUI
    
    '�X�e�[�^�X�E�B���h�E�̕\�����e���X�V
    If SelectedUnit.Status = "�o��" And MainWidth = 15 Then
        DisplayUnitStatus SelectedUnit
    Else
        ClearUnitStatus
    End If
End Sub


'�}�b�v�R�}���h���s
Public Sub MapCommand(ByVal idx As Integer)
    CommandState = "���j�b�g�I��"
    
    Select Case idx
        Case EndTurnCmdID '�^�[���I��
            If ViewMode Then
                ViewMode = False
                Exit Sub
            End If
            EndTurnCommand
        Case DumpCmdID '���f
            DumpCommand
        Case UnitListCmdID '�����\
            UnitListCommand
        Case SearchSpecialPowerCmdID '�X�y�V�����p���[����
            SearchSpecialPowerCommand
        Case GlobalMapCmdID '�S�̃}�b�v
            GlobalMapCommand
        Case OperationObjectCmdID '���ړI
            LockGUI
            HandleEvent "��������"
            RedrawScreen
            UnlockGUI
        Case MapCommand1CmdID To MapCommand10CmdID '�}�b�v�R�}���h
            LockGUI
            HandleEvent MapCommandLabelList(idx - MapCommand1CmdID + 1)
            UnlockGUI
        Case AutoDefenseCmdID '�����������[�h
            MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked _
                = Not MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked
            If MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked Then
                WriteIni "Option", "AutoDefense", "On"
            Else
                WriteIni "Option", "AutoDefense", "Off"
            End If
        Case ConfigurationCmdID '�ݒ�ύX
            Load frmConfiguration
            frmConfiguration.Left = (Screen.width - frmConfiguration.width) / 2
            frmConfiguration.Top = (Screen.Height - frmConfiguration.Height) / 3
            frmConfiguration.Show 1
            Unload frmConfiguration
            Set frmConfiguration = Nothing
        Case RestartCmdID '���X�^�[�g
            RestartCommand
        Case QuickLoadCmdID '�N�C�b�N���[�h
            QuickLoadCommand
        Case QuickSaveCmdID '�N�C�b�N�Z�[�u
            QuickSaveCommand
    End Select
    IsScenarioFinished = False
End Sub

'�u�^�[���I���v�R�}���h
' MOD START MARGE
'Public Sub EndTurnCommand()
Private Sub EndTurnCommand()
' MOD END MARGE
Dim num As Integer
Dim ret As Integer
Dim u As Unit

    '�s�����Ă��Ȃ��������j�b�g�̐��𐔂���
    num = 0
    For Each u In UList
        With u
            If .Party = "����" _
                And (.Status = "�o��" Or .Status = "�i�[") _
                And .Action > 0 _
            Then
                num = num + 1
            End If
        End With
    Next
    
    '�s�����Ă��Ȃ����j�b�g������Όx��
    If num > 0 Then
        ret = MsgBox("�s�����Ă��Ȃ����j�b�g��" & Format$(num) & "�̂���܂�" & vbCr _
            & "���̃^�[�����I�����܂����H", _
            vbOKCancel + vbQuestion, "�I��")
    Else
        ret = 0
    End If
    
    Select Case ret
        Case 1
        Case 2
            Exit Sub
    End Select
    
    '�s���I�����Ă��Ȃ����j�b�g�ɑ΂��čs���I���C�x���g�����{
    For Each SelectedUnit In UList
        With SelectedUnit
            If .Party = "����" _
                And (.Status = "�o��" Or .Status = "�i�[") _
                And .Action > 0 _
            Then
                HandleEvent "�s���I��", .MainPilot.ID
                If IsScenarioFinished Then
                    IsScenarioFinished = False
                    Exit Sub
                End If
            End If
        End With
    Next
    
    '�e�w�c�̃t�F�C�Y�Ɉڍs
    
    StartTurn "�G"
    If IsScenarioFinished Then
        IsScenarioFinished = False
        Exit Sub
    End If
    
    StartTurn "����"
    If IsScenarioFinished Then
        IsScenarioFinished = False
        Exit Sub
    End If
    
    StartTurn "�m�o�b"
    If IsScenarioFinished Then
        IsScenarioFinished = False
        Exit Sub
    End If
    
    '�����t�F�C�Y�ɖ߂�
    StartTurn "����"
    IsScenarioFinished = False
End Sub

'���j�b�g�ꗗ�̕\��
' MOD START MARGE
'Public Sub UnitListCommand()
Private Sub UnitListCommand()
' MOD END MARGE
Dim list() As String, id_list() As String
Dim i As Integer, j As Integer, ret As Integer
Dim uparty As String, sort_mode As String, mode_list() As String
Dim key_list() As Long, strkey_list() As String
Dim max_item As Integer, max_value As Long, max_str As String
Dim buf As String
Dim u As Unit
Dim pilot_status_mode As Boolean
    
    LockGUI
    
    TopItem = 1
    EnlargeListBoxHeight
    ReduceListBoxWidth
    
    '�f�t�H���g�̃\�[�g���@
    uparty = "����"
    sort_mode = "���x��"
    
Beginning:
    
    '���j�b�g�ꗗ�̃��X�g���쐬
    ReDim list(1)
    ReDim id_list(1)
    list(1) = "���w�c�ύX�E���בւ���"
    For Each u In UList
        With u
            If .Party0 = uparty _
                And (.Status = "�o��" Or .Status = "�i�[") _
            Then
                '���m�F�̃��j�b�g�͕\�����Ȃ�
                If (IsOptionDefined("���j�b�g���B��") _
                        And (Not .IsConditionSatisfied("���ʍς�") _
                            And (.Party0 = "�G" Or .Party0 = "����"))) _
                    Or .IsConditionSatisfied("���j�b�g���B��") _
                Then
                    GoTo NextUnit
                End If
                
                ReDim Preserve list(UBound(list) + 1)
                ReDim Preserve id_list(UBound(list))
                ReDim Preserve ListItemFlag(UBound(list))
                
                If Not .IsFeatureAvailable("�_�~�[���j�b�g") Then
                    '�ʏ�̃��j�b�g�\��
                    If IsOptionDefined("���g��") Then
                        '���g�����g�����ꍇ�̃��j�b�g�\��
                        list(UBound(list)) = _
                            RightPaddedString(.Nickname0, 33) & _
                            LeftPaddedString(Format$(.MainPilot.Level), 3) & " "
                    Else
                        list(UBound(list)) = RightPaddedString(.Nickname0, 23)
                        If .MainPilot.Nickname0 = "�p�C���b�g�s��" Then
                            '�p�C���b�g������Ă��Ȃ��ꍇ
                            list(UBound(list)) = _
                                RightPaddedString(list(UBound(list)) & "", 34) & _
                                LeftPaddedString("", 2)
                        Else
                            list(UBound(list)) = _
                                RightPaddedString(list(UBound(list)) & .MainPilot.Nickname, 34) & _
                                LeftPaddedString(Format$(.MainPilot.Level), 2)
                        End If
                        list(UBound(list)) = RightPaddedString(list(UBound(list)), 37)
                    End If
                    If .IsConditionSatisfied("�f�[�^�s��") Then
                        list(UBound(list)) = list(UBound(list)) & _
                            "?????/????? ???/???"
                    Else
                        list(UBound(list)) = list(UBound(list)) & _
                            LeftPaddedString(Format$(.HP), 5) & "/" & _
                            LeftPaddedString(Format$(.MaxHP), 5) & " " & _
                            LeftPaddedString(Format$(.EN), 3) & "/" & _
                            LeftPaddedString(Format$(.MaxEN), 3)
                    End If
                Else
                    '�p�C���b�g�X�e�[�^�X�\����
                    pilot_status_mode = True
                    With .MainPilot
                        list(UBound(list)) = _
                            RightPaddedString(.Nickname, 21) & _
                            LeftPaddedString(Format$(.Level), 3) & _
                            LeftPaddedString(Format$(.SP) & "/" & Format$(.MaxSP), 9) & "  "
                            '�g�p�\�ȃX�y�V�����p���[�ꗗ
                            For i = 1 To .CountSpecialPower
                                If .SP >= .SpecialPowerCost(.SpecialPower(i)) Then
                                    list(UBound(list)) = list(UBound(list)) & _
                                        SPDList.Item(.SpecialPower(i)).ShortName
                                End If
                            Next
                    End With
                End If
                
                If .Action = 0 Then
                    list(UBound(list)) = list(UBound(list)) & "��"
                End If
                If .Status = "�i�[" Then
                    list(UBound(list)) = list(UBound(list)) & "�i"
                End If
                
                id_list(UBound(list)) = .ID
                ListItemFlag(UBound(list)) = False
            End If
        End With
NextUnit:
    Next
    
SortList:
    
    '�\�[�g
    If InStr(sort_mode, "����") = 0 Then
        '���l���g�����\�[�g
        
        '�܂����בւ��Ɏg���L�[�̃��X�g���쐬
        ReDim key_list(UBound(list))
        With UList
            Select Case sort_mode
                Case "�g�o"
                    For i = 2 To UBound(list)
                        key_list(i) = .Item(id_list(i)).HP
                    Next
                Case "�d�m"
                    For i = 2 To UBound(list)
                        key_list(i) = .Item(id_list(i)).EN
                    Next
                Case "���x��", "�p�C���b�g���x��"
                    For i = 2 To UBound(list)
                        With .Item(id_list(i))
                            If .CountPilot() > 0 Then
                                With .MainPilot
                                    key_list(i) = 500 * CLng(.Level) + CLng(.Exp)
                                End With
                            End If
                        End With
                    Next
            End Select
        End With
        
        '�L�[���g���ĕ��׊���
        For i = 2 To UBound(list) - 1
            max_item = i
            max_value = key_list(i)
            For j = i + 1 To UBound(list)
                If key_list(j) > max_value Then
                    max_item = j
                    max_value = key_list(j)
                End If
            Next
            If max_item <> i Then
                buf = list(i)
                list(i) = list(max_item)
                list(max_item) = buf
                
                buf = id_list(i)
                id_list(i) = id_list(max_item)
                id_list(max_item) = buf
                
                key_list(max_item) = key_list(i)
            End If
        Next
    Else
        '��������g�����\�[�g
        
        '�܂����בւ��Ɏg���L�[�̃��X�g���쐬
        ReDim strkey_list(UBound(list))
        With UList
            Select Case sort_mode
                Case "����", "���j�b�g����"
                    For i = 2 To UBound(list)
                        strkey_list(i) = .Item(id_list(i)).KanaName
                    Next
                Case "�p�C���b�g����"
                    For i = 2 To UBound(list)
                        With .Item(id_list(i))
                            If .CountPilot() > 0 Then
                                strkey_list(i) = .MainPilot.KanaName
                            End If
                        End With
                    Next
            End Select
        End With
        
        '�L�[���g���ĕ��׊���
        For i = 2 To UBound(strkey_list) - 1
            max_item = i
            max_str = strkey_list(i)
            For j = i + 1 To UBound(strkey_list)
                If StrComp(strkey_list(j), max_str, 1) = -1 Then
                    max_item = j
                    max_str = strkey_list(j)
                End If
            Next
            If max_item <> i Then
                buf = list(i)
                list(i) = list(max_item)
                list(max_item) = buf
                
                buf = id_list(i)
                id_list(i) = id_list(max_item)
                id_list(max_item) = buf
                
                strkey_list(max_item) = strkey_list(i)
            End If
        Next
    End If
    ReDim ListItemFlag(0)
    
    ReDim ListItemID(UBound(list))
    For i = 1 To UBound(list)
        ListItemID(i) = id_list(i)
    Next
    
    '���X�g��\��
    If pilot_status_mode Then
        ret = ListBox(uparty & "�p�C���b�g�ꗗ", list, _
            "�p�C���b�g��       ���x��    " & Term("�r�o", Nothing, 4) & "  " & Term("�X�y�V�����p���["), _
            "�A���\��")
    ElseIf IsOptionDefined("���g��") Then
        ret = ListBox(uparty & "���j�b�g�ꗗ", list, _
            "���j�b�g��                        Lv     " & Term("�g�o", Nothing, 8) & Term("�d�m"), _
            "�A���\��")
    Else
        ret = ListBox(uparty & "���j�b�g�ꗗ", list, _
            "���j�b�g               �p�C���b�g Lv     " & Term("�g�o", Nothing, 8) & Term("�d�m"), _
            "�A���\��")
    End If
    
    Select Case ret
        Case 0
            '�L�����Z��
            frmListBox.Hide
            ReduceListBoxHeight
            EnlargeListBoxWidth
            ReDim ListItemID(0)
            UnlockGUI
            Exit Sub
        Case 1
            '�\������w�c
            ReDim mode_list(4)
            mode_list(1) = "�����ꗗ"
            mode_list(2) = "�m�o�b�ꗗ"
            mode_list(3) = "�G�ꗗ"
            mode_list(4) = "�����ꗗ"
            
            '�\�[�g���@��I��
            If pilot_status_mode Then
                ReDim Preserve mode_list(7)
                mode_list(5) = "�p�C���b�g���̂ŕ��בւ�"
                mode_list(6) = "���x���ŕ��בւ�"
                mode_list(7) = Term("�r�o") & "�ŕ��בւ�"
            ElseIf IsOptionDefined("���g��") Then
                ReDim Preserve mode_list(8)
                mode_list(5) = "���̂ŕ��בւ�"
                mode_list(6) = "���x���ŕ��בւ�"
                mode_list(7) = Term("�g�o") & "�ŕ��בւ�"
                mode_list(8) = Term("�d�m") & "�ŕ��בւ�"
            Else
                ReDim Preserve mode_list(9)
                mode_list(5) = Term("�g�o") & "�ŕ��בւ�"
                mode_list(6) = Term("�d�m") & "�ŕ��בւ�"
                mode_list(7) = "�p�C���b�g���x���ŕ��בւ�"
                mode_list(8) = "���j�b�g���̂ŕ��בւ�"
                mode_list(9) = "�p�C���b�g���̂ŕ��בւ�"
            End If
            ReDim ListItemID(UBound(mode_list))
            ReDim ListItemFlag(UBound(mode_list))
            
            ret = ListBox("�I��", mode_list, _
                "�ꗗ�\�����@", "�A���\��")
            
            '�w�c��ύX���čĕ\��
            If ret > 0 Then
                If Right$(mode_list(ret), 2) = "�ꗗ" Then
                    uparty = Left$(mode_list(ret), Len(mode_list(ret)) - 2)
                    GoTo Beginning
                ElseIf Right$(mode_list(ret), 5) = "�ŕ��בւ�" Then
                    sort_mode = Left$(mode_list(ret), Len(mode_list(ret)) - 5)
                    GoTo SortList
                End If
            End If
            
            GoTo SortList
    End Select
    
    frmListBox.Hide
    ReduceListBoxHeight
    EnlargeListBoxWidth
    
    '�I�����ꂽ���j�b�g����ʒ����ɕ\��
    Set u = UList.Item(ListItemID(ret))
    Center u.X, u.Y
    RefreshScreen
    DisplayUnitStatus u
    
    '�J�[�\�������ړ�
    If AutoMoveCursor Then
        MoveCursorPos "���j�b�g�I��", u
    End If
    
    ReDim ListItemID(0)
    
    UnlockGUI
End Sub

'�X�y�V�����p���[�����R�}���h
' MOD START MARGE
'Public Sub SearchSpecialPowerCommand()
Private Sub SearchSpecialPowerCommand()
' MOD END MARGE
Dim i As Integer, j As Integer, ret As Integer
Dim list() As String, list2() As String
Dim flist() As Boolean
Dim pid() As String
Dim buf As String
Dim p As Pilot
Dim id_list() As String
Dim strkey_list() As String, max_item As Integer, max_str As String
Dim found As Boolean

    LockGUI
    
    '�C�x���g��p�̃R�}���h���������S�X�y�V�����p���[�̃��X�g���쐬
    ReDim list(0)
    For i = 1 To SPDList.Count
        With SPDList.Item(i)
            If .ShortName <> "��\��" Then
                ReDim Preserve list(UBound(list) + 1)
                ReDim Preserve strkey_list(UBound(list))
                list(UBound(list)) = .Name
                strkey_list(UBound(list)) = .KanaName
            End If
        End With
    Next
    
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
            buf = list(i)
            list(i) = list(max_item)
            list(max_item) = buf
            
            buf = strkey_list(i)
            strkey_list(i) = max_str
            strkey_list(max_item) = buf
        End If
    Next
    
    '�X�̃X�y�V�����p���[�ɑ΂��āA���̃X�y�V�����p���[���g�p�\�ȃp�C���b�g��
    '���邩�ǂ�������
    ReDim flist(UBound(list))
    For i = 1 To UBound(list)
        flist(i) = True
        For Each p In PList
            With p
                If .Party = "����" Then
                    If Not .Unit Is Nothing Then
                        If .Unit.Status = "�o��" _
                            And Not .Unit.IsConditionSatisfied("�߈�") _
                        Then
                            '�{���ɏ���Ă���H
                            found = False
                            With .Unit
                                If p Is .MainPilot Then
                                    found = True
                                Else
                                    For j = 2 To .CountPilot
                                        If p Is .Pilot(j) Then
                                            found = True
                                            Exit For
                                        End If
                                    Next
                                    For j = 1 To .CountSupport
                                        If p Is .Support(j) Then
                                            found = True
                                            Exit For
                                        End If
                                    Next
                                    If p Is .AdditionalSupport Then
                                        found = True
                                    End If
                                End If
                            End With
                            
                            If found Then
                                If .IsSpecialPowerAvailable(list(i)) Then
                                    flist(i) = False
                                    Exit For
                                End If
                            End If
                        End If
                    End If
                End If
            End With
        Next
    Next
    
    Do While True
        ReDim ListItemFlag(UBound(list))
        ReDim ListItemComment(UBound(list))
        ReDim id_list(UBound(list))
        ReDim strkey_list(UBound(list))
        
        '�I���o���Ȃ��X�y�V�����p���[���}�X�N
        For i = 1 To UBound(ListItemFlag)
            ListItemFlag(i) = flist(i)
        Next
        
        '�X�y�V�����p���[�̉����ݒ�
        For i = 1 To UBound(ListItemComment)
            ListItemComment(i) = SPDList.Item(list(i)).Comment
        Next
        
        '��������X�y�V�����p���[��I��
        TopItem = 1
        ret = MultiColumnListBox(Term("�X�y�V�����p���[") & "����", list, True)
        If ret = 0 Then
            CancelCommand
            UnlockGUI
            Exit Sub
        End If
        SelectedSpecialPower = list(ret)
        
        '�I�����ꂽ�X�y�V�����p���[���g�p�ł���p�C���b�g�̈ꗗ���쐬
        ReDim list2(0)
        ReDim ListItemFlag(0)
        ReDim id_list(0)
        ReDim pid(0)
        For Each p In PList
            With p
                '�I�������X�y�V�����p���[���g�p�ł���p�C���b�g���ǂ�������
                If .Party <> "����" Then
                    GoTo NextLoop
                End If
                If .Unit Is Nothing Then
                    GoTo NextLoop
                End If
                If .Unit.Status <> "�o��" Then
                    GoTo NextLoop
                End If
                If .Unit.CountPilot > 0 Then
                    If .ID = .Unit.Pilot(1).ID _
                        And .ID <> .Unit.MainPilot.ID _
                    Then
                        '�ǉ��p�C���b�g�̂��߁A�g�p����Ă��Ȃ�
                        GoTo NextLoop
                    End If
                End If
                If Not .IsSpecialPowerAvailable(SelectedSpecialPower) Then
                    GoTo NextLoop
                End If
                
                '�p�C���b�g�����X�g�ɒǉ�
                ReDim Preserve list2(UBound(list2) + 1)
                ReDim Preserve ListItemFlag(UBound(list2))
                ReDim Preserve id_list(UBound(list2))
                ReDim Preserve pid(UBound(list2))
                ListItemFlag(UBound(list2)) = False
                id_list(UBound(list2)) = .Unit.ID
                pid(UBound(list2)) = .ID
                list2(UBound(list2)) = RightPaddedString(.Nickname, 19) _
                    & RightPaddedString(Format$(.SP) & "/" & Format$(.MaxSP), 10)
                
                buf = ""
                For j = 1 To .CountSpecialPower
                    buf = buf & SPDList.Item(.SpecialPower(j)).ShortName
                Next
                list2(UBound(list2)) = list2(UBound(list2)) & RightPaddedString(buf, 12)
                
                If .SP < .SpecialPowerCost(SelectedSpecialPower) Then
                    list2(UBound(list2)) = list2(UBound(list2)) & " " _
                        & Term("�r�o", p.Unit) & "�s��"
                End If
                If .Unit.Action = 0 Then
                    list2(UBound(list2)) = list2(UBound(list2)) & " �s����"
                End If
            End With
NextLoop:
        Next
        
        SelectedSpecialPower = ""
        
        '������������p�C���b�g�̑I��
        TopItem = 1
        EnlargeListBoxHeight
        If IsOptionDefined("���g��") Then
            ret = ListBox("���j�b�g�I��", list2, _
                "���j�b�g           " & Term("SP", Nothing, 2) & "/Max" & Term("SP", Nothing, 2) _
                     & "  " & Term("�X�y�V�����p���["))
        Else
            ret = ListBox("�p�C���b�g�I��", list2, _
                "�p�C���b�g         " & Term("SP", Nothing, 2) & "/Max" & Term("SP", Nothing, 2) _
                     & "  " & Term("�X�y�V�����p���["))
        End If
        ReduceListBoxHeight
        
        '�p�C���b�g�̏�郆�j�b�g����ʒ����ɕ\��
        If ret > 0 Then
            With PList.Item(pid(ret))
                Center .Unit.X, .Unit.Y
                RefreshScreen
                DisplayUnitStatus .Unit
                
                '�J�[�\�������ړ�
                If AutoMoveCursor Then
                    MoveCursorPos "���j�b�g�I��", .Unit
                End If
            End With
            
            ReDim id_list(0)
            
            UnlockGUI
            Exit Sub
        End If
    Loop
End Sub

'���X�^�[�g�R�}���h
' MOD START MARGE
'Public Sub RestartCommand()
Private Sub RestartCommand()
' MOD END MARGE
Dim ret As Integer

    '���X�^�[�g���s�����m�F
    ret = MsgBox("���X�^�[�g���܂����H", _
        vbOKCancel + vbQuestion, "���X�^�[�g")
    If ret = vbCancel Then
        Exit Sub
    End If
    
    LockGUI
    
    RestoreData ScenarioPath & "_���X�^�[�g.src", True
    
    UnlockGUI
End Sub

'�N�C�b�N���[�h�R�}���h
' MOD START MARGE
'Public Sub QuickLoadCommand()
Private Sub QuickLoadCommand()
' MOD END MARGE
Dim ret As Integer

    '���[�h���s�����m�F
    ret = MsgBox("�f�[�^�����[�h���܂����H", _
        vbOKCancel + vbQuestion, "�N�C�b�N���[�h")
    If ret = vbCancel Then
        Exit Sub
    End If
    
    LockGUI
    
    RestoreData ScenarioPath & "_�N�C�b�N�Z�[�u.src", True
    
    '��ʂ����������ăX�e�[�^�X��\��
    RedrawScreen
    DisplayGlobalStatus
    
    UnlockGUI
End Sub

'�N�C�b�N�Z�[�u�R�}���h
' MOD START MARGE
'Public Sub QuickSaveCommand()
Private Sub QuickSaveCommand()
' MOD END MARGE

    LockGUI
    
    '�}�E�X�J�[�\���������v��
    Screen.MousePointer = vbHourglass
    
    '���f�f�[�^���Z�[�u
    DumpData ScenarioPath & "_�N�C�b�N�Z�[�u.src"
    
    UnlockGUI
    
    '�}�E�X�J�[�\�������ɖ߂�
    Screen.MousePointer = 0
End Sub


'�v���C�𒆒f���A���f�p�f�[�^���Z�[�u����
' MOD START MARGE
'Public Sub DumpCommand()
Private Sub DumpCommand()
' MOD END MARGE
Dim fname As String, save_path As String
Dim ret As Integer, i As Integer

    '�v���C�𒆒f���邩�m�F
    ret = MsgBox("�v���C�𒆒f���܂����H", _
        vbOKCancel + vbQuestion, "���f")
    If ret = vbCancel Then
        Exit Sub
    End If
    
    '���f�f�[�^���Z�[�u����t�@�C����������
    For i = 1 To Len(ScenarioFileName)
        If Mid$(ScenarioFileName, Len(ScenarioFileName) - i + 1, 1) = "\" Then
            Exit For
        End If
    Next
    fname = Mid$(ScenarioFileName, Len(ScenarioFileName) - i + 2, i - 5)
    fname = fname & "�𒆒f.src"
    
    fname = SaveFileDialog("�f�[�^�Z�[�u", _
        ScenarioPath, fname, _
        2, "�����ް�", "src")
    
    If fname = "" Then
        '�L�����Z��
        Exit Sub
    End If
    
    '�Z�[�u��̓V�i���I�t�H���_�H
    If InStr(fname, "\") > 0 Then
        save_path = Left$(fname, InStr2(fname, "\"))
    End If
    If Dir$(save_path) <> Dir$(ScenarioPath) Then
        If MsgBox("�Z�[�u�t�@�C���̓V�i���I�t�H���_�ɂȂ��Ɠǂݍ��߂܂���B" & vbCr & vbLf _
                & "���̂܂܃Z�[�u���܂����H", vbOKCancel + vbQuestion) <> 1 _
        Then
            Exit Sub
        End If
    End If
    
    Screen.MousePointer = vbHourglass '�}�E�X�J�[�\���������v��
    
    LockGUI
    
    '���f�f�[�^���Z�[�u
    DumpData fname
    
    '�}�E�X�J�[�\�������ɖ߂�
    Screen.MousePointer = 0
    
    MainForm.Hide
    
    '�Q�[�����I��
    ExitGame
End Sub


'�S�̃}�b�v�̕\��
' MOD START MARGE
'Public Sub GlobalMapCommand()
Private Sub GlobalMapCommand()
' MOD END MARGE
Dim pic As PictureBox, xx As Integer, yy As Integer
Dim num As Integer, num2 As Integer
Dim mwidth As Long, mheight As Long
Dim ret As Long, smode As Long
Dim u As Unit, i As Integer, j As Integer
Dim prev_mode As Boolean

    LockGUI
    
    With MainForm
        '���₷���悤�ɔw�i��ݒ�
        .picMain(0).BackColor = &HC0C0C0
        .picMain(0) = LoadPicture("")
        
        '�}�b�v�̏c���̔䗦�����ɏk���}�b�v�̑傫�������߂�
        If MapWidth > MapHeight Then
            mwidth = 300
            mheight = 300 * MapHeight \ MapWidth
        Else
            mheight = 300
            mwidth = 300 * MapWidth \ MapHeight
        End If
        
        '�}�b�v�̑S�̉摜���쐬
        Set pic = .picTmp
        With pic
            .Picture = LoadPicture("")
            .width = MapPWidth
            .Height = MapPHeight
        End With
        ret = BitBlt(pic.hDC, 0, 0, MapPWidth, MapPHeight, .picBack.hDC, 0, 0, SRCCOPY)
        For i = 1 To MapWidth
            xx = 32 * (i - 1)
            For j = 1 To MapHeight
                yy = 32 * (j - 1)
                Set u = MapDataForUnit(i, j)
                If Not u Is Nothing Then
                    If u.BitmapID > 0 Then
                        If u.Action > 0 Or u.IsFeatureAvailable("�n�`���j�b�g") Then
                            '���j�b�g
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picUnitBitmap.hDC, _
                                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
                                SRCCOPY)
                        Else
                            '�s���ς̃��j�b�g
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picUnitBitmap.hDC, _
                                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, _
                                SRCCOPY)
                        End If
                        
                        '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
                        Select Case u.Area
                            Case "��"
                                pic.Line (xx, yy + 28)-(xx + 31, yy + 28), vbBlack
                            Case "����"
                                pic.Line (xx, yy + 3)-(xx + 31, yy + 3), vbBlack
                            Case "�n��"
                                pic.Line (xx, yy + 28)-(xx + 31, yy + 28), vbBlack
                                pic.Line (xx, yy + 3)-(xx + 31, yy + 3), vbBlack
                            Case "�F��"
                                If TerrainClass(i, j) = "����" Then
                                    pic.Line (xx, yy + 28)-(xx + 31, yy + 28), vbBlack
                                End If
                        End Select
                    End If
                End If
            Next
        Next
        
        '�}�b�v�S�̂��k�����ĕ`��
        smode = GetStretchBltMode(.picMain(0).hDC)
        ret = SetStretchBltMode(.picMain(0).hDC, STRETCH_DELETESCANS)
        ret = StretchBlt(.picMain(0).hDC, _
            (MainPWidth - mwidth) \ 2, _
            (MainPHeight - mheight) \ 2, _
            mwidth, mheight, _
            pic.hDC, 0, 0, _
            MapPWidth, MapPHeight, SRCCOPY)
        ret = SetStretchBltMode(.picMain(0).hDC, smode)
        
        '�}�b�v�S�̉摜��j��
        With pic
            .Picture = LoadPicture("")
            .width = 32
            .Height = 32
        End With
        
        '��ʂ��X�V
        .picMain(0).Refresh
    End With
    
    '�������j�b�g���A�G���j�b�g���̃J�E���g
    For Each u In UList
        With u
            If .Status = "�o��" Or .Status = "�i�[" Then
                If .Party0 = "����" Or .Party0 = "�m�o�b" Then
                    num = num + 1
                Else
                    num2 = num2 + 1
                End If
            End If
        End With
    Next
    
    '�e���j�b�g���̕\��
    prev_mode = AutoMessageMode
    AutoMessageMode = False
    
    OpenMessageForm
    DisplayMessage "�V�X�e��", _
        "�������j�b�g�F " & Format$(num) & ";" & _
        "�G���j�b�g  �F " & Format$(num2)
    CloseMessageForm
    
    AutoMessageMode = prev_mode
    
    '��ʂ����ɖ߂�
    MainForm.picMain(0).BackColor = &HFFFFFF
    RefreshScreen
    
    UnlockGUI
End Sub


'���݂̑I���󋵂��L�^
Public Sub SaveSelections()
    '�X�^�b�N�̃C���f�b�N�X�𑝂₷
    SelectionStackIndex = SelectionStackIndex + 1
    
    '�X�^�b�N�̈�m��
    ReDim Preserve SavedSelectedUnit(SelectionStackIndex)
    ReDim Preserve SavedSelectedTarget(SelectionStackIndex)
    ReDim Preserve SavedSelectedUnitForEvent(SelectionStackIndex)
    ReDim Preserve SavedSelectedTargetForEvent(SelectionStackIndex)
    ReDim Preserve SavedSelectedWeapon(SelectionStackIndex)
    ReDim Preserve SavedSelectedWeaponName(SelectionStackIndex)
    ReDim Preserve SavedSelectedTWeapon(SelectionStackIndex)
    ReDim Preserve SavedSelectedTWeaponName(SelectionStackIndex)
    ReDim Preserve SavedSelectedDefenseOption(SelectionStackIndex)
    ReDim Preserve SavedSelectedAbility(SelectionStackIndex)
    ReDim Preserve SavedSelectedAbilityName(SelectionStackIndex)
    ReDim Preserve SavedSelectedX(SelectionStackIndex)
    ReDim Preserve SavedSelectedY(SelectionStackIndex)
    
    '�m�ۂ����̈�ɑI���󋵂��L�^
    Set SavedSelectedUnit(SelectionStackIndex) = SelectedUnit
    Set SavedSelectedTarget(SelectionStackIndex) = SelectedTarget
    Set SavedSelectedUnitForEvent(SelectionStackIndex) = SelectedUnitForEvent
    Set SavedSelectedTargetForEvent(SelectionStackIndex) = SelectedTargetForEvent
    SavedSelectedWeapon(SelectionStackIndex) = SelectedWeapon
    SavedSelectedWeaponName(SelectionStackIndex) = SelectedWeaponName
    SavedSelectedTWeapon(SelectionStackIndex) = SelectedTWeapon
    SavedSelectedTWeaponName(SelectionStackIndex) = SelectedTWeaponName
    SavedSelectedDefenseOption(SelectionStackIndex) = SelectedDefenseOption
    SavedSelectedAbility(SelectionStackIndex) = SelectedAbility
    SavedSelectedAbilityName(SelectionStackIndex) = SelectedAbilityName
    SavedSelectedX(SelectionStackIndex) = SelectedX
    SavedSelectedY(SelectionStackIndex) = SelectedY
End Sub

'�I���󋵂𕜌�
Public Sub RestoreSelections()
    '�X�^�b�N�ɐς܂�Ă��Ȃ��H
    If SelectionStackIndex = 0 Then
        Exit Sub
    End If
    
    '�X�^�b�N�g�b�v����L�^���ꂽ�I���󋵂����o��
    If Not SavedSelectedUnit(SelectionStackIndex) Is Nothing Then
        Set SelectedUnit = SavedSelectedUnit(SelectionStackIndex).CurrentForm
    Else
        Set SelectedUnit = Nothing
    End If
    If Not SavedSelectedTarget(SelectionStackIndex) Is Nothing Then
        Set SelectedTarget = SavedSelectedTarget(SelectionStackIndex).CurrentForm
    Else
        Set SelectedTarget = Nothing
    End If
    If Not SavedSelectedUnitForEvent(SelectionStackIndex) Is Nothing Then
        Set SelectedUnitForEvent = _
            SavedSelectedUnitForEvent(SelectionStackIndex).CurrentForm
    Else
        Set SelectedUnitForEvent = Nothing
    End If
    If Not SavedSelectedTargetForEvent(SelectionStackIndex) Is Nothing Then
        Set SelectedTargetForEvent = _
            SavedSelectedTargetForEvent(SelectionStackIndex).CurrentForm
    Else
        Set SelectedTargetForEvent = Nothing
    End If
    SelectedWeapon = SavedSelectedWeapon(SelectionStackIndex)
    SelectedWeaponName = SavedSelectedWeaponName(SelectionStackIndex)
    SelectedTWeapon = SavedSelectedTWeapon(SelectionStackIndex)
    SelectedTWeaponName = SavedSelectedTWeaponName(SelectionStackIndex)
    SelectedDefenseOption = SavedSelectedDefenseOption(SelectionStackIndex)
    SelectedAbility = SavedSelectedAbility(SelectionStackIndex)
    SelectedAbilityName = SavedSelectedAbilityName(SelectionStackIndex)
    SelectedX = SavedSelectedX(SelectionStackIndex)
    SelectedY = SavedSelectedY(SelectionStackIndex)
    
    '�X�^�b�N�̃C���f�b�N�X���P���炷
    SelectionStackIndex = SelectionStackIndex - 1
    
    '�X�^�b�N�̗̈���J��
    ReDim Preserve SavedSelectedUnit(SelectionStackIndex)
    ReDim Preserve SavedSelectedTarget(SelectionStackIndex)
    ReDim Preserve SavedSelectedUnitForEvent(SelectionStackIndex)
    ReDim Preserve SavedSelectedTargetForEvent(SelectionStackIndex)
    ReDim Preserve SavedSelectedWeapon(SelectionStackIndex)
    ReDim Preserve SavedSelectedWeaponName(SelectionStackIndex)
    ReDim Preserve SavedSelectedTWeapon(SelectionStackIndex)
    ReDim Preserve SavedSelectedTWeaponName(SelectionStackIndex)
    ReDim Preserve SavedSelectedDefenseOption(SelectionStackIndex)
    ReDim Preserve SavedSelectedAbility(SelectionStackIndex)
    ReDim Preserve SavedSelectedAbilityName(SelectionStackIndex)
    ReDim Preserve SavedSelectedX(SelectionStackIndex)
    ReDim Preserve SavedSelectedY(SelectionStackIndex)
End Sub

'�I�������ւ���
Public Sub SwapSelections()
Dim u As Unit, t As Unit
Dim w As Integer, tw As Integer
Dim wname As String, twname As String

    Set u = SelectedUnit
    Set t = SelectedTarget
    Set SelectedUnit = t
    Set SelectedTarget = u
    
    w = SelectedWeapon
    tw = SelectedTWeapon
    SelectedWeapon = tw
    SelectedTWeapon = w
    
    wname = SelectedWeaponName
    twname = SelectedTWeaponName
    SelectedWeaponName = twname
    SelectedTWeaponName = wname
End Sub

