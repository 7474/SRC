Attribute VB_Name = "SRC"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'�p�C���b�g�f�[�^�̃��X�g
Public PDList As New PilotDataList
'�m���p�C���b�g�f�[�^�̃��X�g
Public NPDList As New NonPilotDataList
'���j�b�g�f�[�^�̃��X�g
Public UDList As New UnitDataList
'�A�C�e���f�[�^�̃��X�g
Public IDList As New ItemDataList
'���b�Z�[�W�f�[�^�̃��X�g
Public MDList As New MessageDataList
'������ʃf�[�^�̃��X�g
Public EDList As New MessageDataList
'�퓬�A�j���f�[�^�̃��X�g
Public ADList As New MessageDataList
'�g���퓬�A�j���f�[�^�̃��X�g
Public EADList As New MessageDataList
'�_�C�A���O�f�[�^�̃��X�g
Public DDList As New DialogDataList
'�X�y�V�����p���[�f�[�^�̃��X�g
Public SPDList As New SpecialPowerDataList
'�G���A�X�f�[�^�̃��X�g
Public ALDList As New AliasDataList
'�n�`�f�[�^�̃��X�g
Public TDList As New TerrainDataList
'�o�g���R���t�B�O�f�[�^�̃��X�g
Public BCList As New BattleConfigDataList


'�p�C���b�g�̃��X�g
Public PList As New Pilots
'���j�b�g�̃��X�g
Public UList As New Units
'�A�C�e���̃��X�g
Public IList As New Items

'�C�x���g�t�@�C����
Public ScenarioFileName As String
'�C�x���g�t�@�C�����̂���t�H���_
Public ScenarioPath As String
'�Z�[�u�f�[�^�̃t�@�C���f�B�X�N���v�^
Public SaveDataFileNumber As Integer
'�Z�[�u�f�[�^�̃o�[�W����
Public SaveDataVersion As Long

'���̃X�e�[�W���I���������������t���O
Public IsScenarioFinished As Boolean
'�C���^�[�~�b�V�����R�}���h�ɂ��X�e�[�W���ǂ����������t���O
Public IsSubStage As Boolean
'�R�}���h���L�����Z�����ꂽ���ǂ����������t���O
Public IsCanceled As Boolean

'�t�F�C�Y��
Public Stage As String
'�^�[����
Public Turn As Integer
'���^�[����
Public TotalTurn As Long
'������
Public Money As Long
'�ǂݍ��܂�Ă���f�[�^��
Public Titles() As String
'���[�J���f�[�^���ǂݍ��܂�Ă��邩�H
Public IsLocalDataLoaded As Boolean

'�ŐV�̃Z�[�u�f�[�^�̃t�@�C����
Public LastSaveDataFileName As String
'���X�^�[�g�p�Z�[�u�f�[�^�����p�\���ǂ���
Public IsRestartSaveDataAvailable As Boolean
'�N�C�b�N���[�h�p�Z�[�u�f�[�^�����p�\���ǂ���
Public IsQuickSaveDataAvailable As Boolean

'�V�X�e���I�v�V����
'�}�X�ڂ̕\�������邩
Public ShowSquareLine As Boolean
'�G�t�F�C�Y�ɂ͂a�f�l��ύX���Ȃ���
Public KeepEnemyBGM As Boolean
'�g���f�[�^�t�H���_�ւ̃p�X
Public ExtDataPath As String
Public ExtDataPath2 As String
'MIDI�������Z�b�g�̎��
Public MidiResetType As String
'�����h�䃂�[�h���g����
Public AutoMoveCursor As Boolean
'�X�y�V�����p���[�A�j����\�����邩
Public SpecialPowerAnimation As Boolean
'�퓬�A�j����\�����邩
Public BattleAnimation As Boolean
'���폀���A�j����\�����邩
Public WeaponAnimation As Boolean
'�g��퓬�A�j����\�����邩
Public ExtendedAnimation As Boolean
'�ړ��A�j����\�����邩
Public MoveAnimation As Boolean
'�摜�o�b�t�@�̖���
Public ImageBufferSize As Integer
'�摜�o�b�t�@�̍ő�o�C�g��
Public MaxImageBufferByteSize As Long
'�g��摜���摜�o�b�t�@�ɕۑ����邩
Public KeepStretchedImage As Boolean
'���ߕ`���TransparentBlt���g����
Public UseTransparentBlt As Boolean

'SRC.exe�̂���ꏊ
Public AppPath As String

'�f�[�^���Ƀ��x���w����ȗ������ꍇ�̃f�t�H���g�̃��x���l
Public Const DEFAULT_LEVEL = -1000

Public Sub Main()
Dim fname As String
Dim i As Integer, buf As String
Dim ret As Long
    
    '�Q�d�N���֎~
    If App.PrevInstance Then
        End
    End If
    
    'SRC.exe�̂���ꏊ�𒲂ׂ�
    AppPath = App.Path
    If Right$(AppPath, 1) <> "\" Then
        AppPath = AppPath & "\"
    End If
    
    'SRC���������C���X�g�[������Ă��邩���`�F�b�N
    
    'Bitmap�֌W�̃`�F�b�N
    If Len(Dir$(AppPath & "Bitmap", vbDirectory)) = 0 Then
        ErrorMessage "Bitmap�t�H���_������܂���B" & vbCr & vbLf _
            & "SRC.exe�Ɠ����t�H���_�ɔėp�O���t�B�b�N�W���C���X�g�[�����Ă��������B"
        End
    End If
    If Len(Dir$(AppPath & "�a����������", vbDirectory)) > 0 Then
        ErrorMessage "Bitmap�t�H���_�̃t�H���_�����S�p�����ɂȂ��Ă��܂��B" & vbCr & vbLf _
            & AppPath & "�a����������" & vbCr & vbLf _
            & "�t�H���_���𔼊p�����ɒ����Ă��������B"
        End
    End If
    If Len(Dir$(AppPath & "Bitmap\Bitmap", vbDirectory)) > 0 Then
        ErrorMessage "Bitmap�t�H���_���ɂ����Bitmap�t�H���_�����݂��܂��B" & vbCr & vbLf _
            & AppPath & "Bitmap\Bitmap" & vbCr & vbLf _
            & "�t�H���_�\���𒼂��Ă��������B"
        End
    End If
    
    '�C�x���g�O���t�B�b�N
    If Len(Dir$(AppPath & "Bitmap\Event", vbDirectory)) = 0 Then
        ErrorMessage "Bitmap\Event�t�H���_��������܂���B" & vbCr & vbLf _
            & "�ėp�O���t�B�b�N�W���������C���X�g�[������Ă��Ȃ��Ǝv���܂��B"
        End
    End If
    
    '�}�b�v�O���t�B�b�N
    If Len(Dir$(AppPath & "Bitmap\Map", vbDirectory)) = 0 Then
        ErrorMessage "Bitmap\Map�t�H���_������܂���B" & vbCr & vbLf _
            & "�ėp�O���t�B�b�N�W���������C���X�g�[������Ă��Ȃ��Ǝv���܂��B"
        End
    End If
    If Len(Dir$(AppPath & "Bitmap\Map\plain\plain0000.bmp")) = 0 _
        And Len(Dir$(AppPath & "Bitmap\Map\plain0000.bmp")) = 0 _
        And Len(Dir$(AppPath & "Bitmap\Map\plain0.bmp")) = 0 _
    Then
        If Len(Dir$(AppPath & "Bitmap\Map\Map", vbDirectory)) > 0 Then
            ErrorMessage "Bitmap\Map�t�H���_���ɂ����Map�t�H���_�����݂��܂��B" & vbCr & vbLf _
                & AppPath & "Bitmap\Map\Map" & vbCr & vbLf _
                & "�t�H���_�\���𒼂��Ă��������B"
            End
        End If
        
        If Len(Dir$(AppPath & "Bitmap\Map\*", vbNormal)) = 0 Then
            ErrorMessage "Bitmap\Map�t�H���_���Ƀt�@�C��������܂���B" & vbCr & vbLf _
                & "�ėp�O���t�B�b�N�W���������C���X�g�[������Ă��Ȃ��Ǝv���܂��B"
            End
        End If
        
        ErrorMessage "Bitmap\Map�t�H���_����plain0000.bmp������܂���B" & vbCr & vbLf _
            & "�ꕔ�̃}�b�v�摜�t�@�C�������C���X�g�[������Ă��Ȃ����ꂪ����܂��B" & vbCr & vbLf _
            & "�V�K�C���X�g�[���̃t�@�C�����g���Ĕėp�O���t�B�b�N�W���C���X�g�[�����Ă��������B"
        End
    End If
    
    '���ʉ�
    If Len(Dir$(AppPath & "Sound", vbDirectory)) = 0 Then
        ErrorMessage "Sound�t�H���_������܂���B" & vbCr & vbLf _
            & "SRC.exe�Ɠ����t�H���_�Ɍ��ʉ��W���C���X�g�[�����Ă��������B"
        End
    End If
    If Len(Dir$(AppPath & "�r��������", vbDirectory)) > 0 Then
        ErrorMessage "Sound�t�H���_�̃t�H���_�����S�p�����ɂȂ��Ă��܂��B" & vbCr & vbLf _
            & AppPath & "�r��������" & vbCr & vbLf _
            & "�t�H���_���𔼊p�����ɒ����Ă��������B"
        End
    End If
    If Len(Dir$(AppPath & "Sound\Sound", vbDirectory)) > 0 Then
        ErrorMessage "Sound�t�H���_���ɂ����Sound�t�H���_�����݂��܂��B" & vbCr & vbLf _
            & AppPath & "Sound\Sound" & vbCr & vbLf _
            & "�t�H���_�\���𒼂��Ă��������B"
        End
    End If
    If Len(Dir$(AppPath & "Sound\*", vbNormal)) = 0 Then
        ErrorMessage "Sound�t�H���_���Ƀt�@�C��������܂���B" & vbCr & vbLf _
            & "Sound�t�H���_���Ɍ��ʉ��W���C���X�g�[�����Ă��������B"
        End
    End If
    
    '���C���E�B���h�E�̃��[�h��Flash�̓o�^�����{
    LoadMainFormAndRegisterFlash
    
    'Src.ini��������΍쐬
    If Not FileExists(AppPath & "Src.ini") Then
        CreateIniFile
    End If
    
    '�����̏�����
    Randomize
    
    '���ԉ𑜓x��ύX����
    Call timeBeginPeriod(1)
    
    '�t���X�N���[�����[�h���g���H
    If LCase$(ReadIni("Option", "FullScreen")) = "on" Then
        ChangeDisplaySize 800, 600
    End If
    
    '�}�E�X�J�[�\���������v��
    Screen.MousePointer = 11
    
    '�^�C�g����ʂ�\��
    OpenTitleForm
    
    'WAVE�Đ��̎�i�́H
    Select Case LCase$(ReadIni("Option", "UseDirectSound"))
        Case "on"
            'DirectSound�̏����������݂�
            InitDirectSound
        Case "off"
            UseDirectSound = False
        Case Else
            'DirectSound�̏����������݂�
            InitDirectSound
            'DirectSound���g�p�\���ǂ����Őݒ��؂�ւ�
'            If UseDirectSound Then
'                WriteIni "Option", "UseDirectSound", "On"
'            Else
'                WriteIni "Option", "UseDirectSound", "Off"
'            End If
    End Select
    
    'MIDI���t�̎�i�́H
    Select Case LCase$(ReadIni("Option", "UseDirectMusic"))
        Case "on"
            'DirectMusic�̏����������݂�
            InitDirectMusic
        Case "off"
            UseMCI = True
        Case Else
            If GetWinVersion() >= 500 Then
                'NT�n��OS�ł̓f�t�H���g��DirectMusic���g��
                'DirectMusic�̏����������݂�
                InitDirectMusic
                'DirectMusic���g�p�\���ǂ����Őݒ��؂�ւ�
                If UseDirectMusic Then
                    WriteIni "Option", "UseDirectMusic", "On"
                Else
                    WriteIni "Option", "UseDirectMusic", "Off"
                End If
            Else
                'NT�nOS�łȂ����MCI���g��
                UseMCI = True
                WriteIni "Option", "UseDirectMusic", "Off"
            End If
    End Select
    If ReadIni("Option", "MIDIPortID") = "" Then
        WriteIni "Option", "MIDIPortID", "0"
    End If
    
    'MP3�̍Đ�����
    buf = ReadIni("Option", "MP3Volume")
    If buf = "" Then
        WriteIni "Option", "MP3Volume", "50"
        MP3Volume = 50
    Else
        MP3Volume = StrToLng(buf)
        If MP3Volume < 0 Then
            WriteIni "Option", "MP3Volume", "0"
            MP3Volume = 0
        ElseIf MP3Volume > 100 Then
            WriteIni "Option", "MP3Volume", "100"
            MP3Volume = 100
        End If
    End If
    
    'MP3�̏o�̓t���[����
    buf = ReadIni("Option", "MP3OutputBlock")
    If buf = "" Then
        WriteIni "Option", "MP3OutputBlock", "20"
    End If
    
    'MP3�̓��͒���̃X���[�v����
    buf = ReadIni("Option", "MP3InputSleep")
    If buf = "" Then
        WriteIni "Option", "MP3InputSleep", "5"
    End If
    
    '�a�f�l�pMIDI�t�@�C���ݒ�
    If ReadIni("BGM", "Opening") = "" Then
        WriteIni "BGM", "Opening", "Opening.mid"
    End If
    If ReadIni("BGM", "Map1") = "" Then
        WriteIni "BGM", "Map1", "Map1.mid"
    End If
    If ReadIni("BGM", "Map2") = "" Then
        WriteIni "BGM", "Map2", "Map2.mid"
    End If
    If ReadIni("BGM", "Map3") = "" Then
        WriteIni "BGM", "Map3", "Map3.mid"
    End If
    If ReadIni("BGM", "Map4") = "" Then
        WriteIni "BGM", "Map4", "Map4.mid"
    End If
    If ReadIni("BGM", "Map5") = "" Then
        WriteIni "BGM", "Map5", "Map5.mid"
    End If
    If ReadIni("BGM", "Map6") = "" Then
        WriteIni "BGM", "Map6", "Map6.mid"
    End If
    If ReadIni("BGM", "Briefing") = "" Then
        WriteIni "BGM", "Briefing", "Briefing.mid"
    End If
    If ReadIni("BGM", "Intermission") = "" Then
        WriteIni "BGM", "Intermission", "Intermission.mid"
    End If
    If ReadIni("BGM", "Subtitle") = "" Then
        WriteIni "BGM", "Subtitle", "Subtitle.mid"
    End If
    If ReadIni("BGM", "End") = "" Then
        WriteIni "BGM", "End", "End.mid"
    End If
    If ReadIni("BGM", "default") = "" Then
        WriteIni "BGM", "default", "default.mid"
    End If
    
    
    '�N�����̈�������ǂݍ��ރt�@�C����T��
    If Left$(Command$(), 1) = """" Then
        fname = Mid$(Command$(), 2, Len(Command$()) - 2)
    Else
        fname = Command$()
    End If
    
    If LCase$(Right$(fname, 4)) <> ".src" _
        And LCase$(Right$(fname, 4)) <> ".eve" _
    Then
        '�_�C�A���O��\�����ēǂݍ��ރt�@�C�����w�肷��ꍇ
        
        '�_�C�A���O�̏����t�H���_��ݒ�
        i = 0
        ScenarioPath = ReadIni("Log", "LastFolder")
        On Error GoTo ErrorHandler
        If ScenarioPath = "" Then
            ScenarioPath = AppPath
        ElseIf Dir$(ScenarioPath, vbDirectory) = "." Then
            If Dir$(ScenarioPath & "*.src") <> "" Then
                i = 3
            End If
            If InStr(ScenarioPath, "�e�X�g�f�[�^") > 0 Then
                i = 2
            End If
            If InStr(ScenarioPath, "�퓬�A�j���e�X�g") > 0 Then
                i = 2
            End If
            If Dir$(ScenarioPath & "test.eve") <> "" Then
                i = 2
            End If
        Else
            ScenarioPath = AppPath
        End If
        On Error GoTo 0
        GoTo SkipErrorHandler
        
ErrorHandler:
        ScenarioPath = AppPath

SkipErrorHandler:
        If Right$(ScenarioPath, 1) <> "\" Then
            ScenarioPath = ScenarioPath & "\"
        End If
        
        '�g���f�[�^�̃t�H���_��ݒ�
        ExtDataPath = ReadIni("Option", "ExtDataPath")
        If Len(ExtDataPath) > 0 Then
            If Right$(ExtDataPath, 1) <> "\" Then
                ExtDataPath = ExtDataPath & "\"
            End If
        End If
        ExtDataPath2 = ReadIni("Option", "ExtDataPath2")
        If Len(ExtDataPath2) > 0 Then
            If Right$(ExtDataPath2, 1) <> "\" Then
                ExtDataPath2 = ExtDataPath2 & "\"
            End If
        End If
        
        '�I�[�v�j���O�ȉ��t
        StopBGM True
        StartBGM BGMName("Opening"), True
        
        '�C�x���g�f�[�^��������
        InitEventData
        
        '�^�C�g����ʂ����
        CloseTitleForm
        
        '�}�E�X�J�[�\�������ɖ߂�
        Screen.MousePointer = 0
        
        '�V�i���I�p�X�͕ύX�����\��������̂ŁAMIDI�t�@�C���̃T�[�`�p�X�����Z�b�g
        ResetMidiSearchPath
        
        '�v���C���[�Ƀ��[�h����t�@�C����q�˂�
        fname = LoadFileDialog("�V�i���I�^�Z�[�u�t�@�C���̎w��", _
            ScenarioPath, "", i, "������ް�", "eve", "�����ް�", "src")
        
        '�t�@�C�����w�肳��Ȃ������ꍇ�͂��̂܂܏I��
        If fname = "" Then
            TerminateSRC
            End
        End If
        
        '�V�i���I�̂���t�H���_�̃p�X������
        If InStr(fname, "\") > 0 Then
            For i = 1 To Len(fname)
                If Mid$(fname, Len(fname) - i + 1, 1) = "\" Then
                    Exit For
                End If
            Next
            ScenarioPath = Left$(fname, Len(fname) - i)
        Else
            ScenarioPath = AppPath
        End If
        If Right$(ScenarioPath, 1) <> "\" Then
            ScenarioPath = ScenarioPath & "\"
        End If
' ADD START MARGE
        '�V�i���I�p�X�����肵���i�K�Ŋg���f�[�^�t�H���_�p�X���Đݒ肷��悤�ɕύX
        '�g���f�[�^�̃t�H���_��ݒ�
        ExtDataPath = ReadIni("Option", "ExtDataPath")
        If Len(ExtDataPath) > 0 Then
            If Right$(ExtDataPath, 1) <> "\" Then
                ExtDataPath = ExtDataPath & "\"
            End If
        End If
        ExtDataPath2 = ReadIni("Option", "ExtDataPath2")
        If Len(ExtDataPath2) > 0 Then
            If Right$(ExtDataPath2, 1) <> "\" Then
                ExtDataPath2 = ExtDataPath2 & "\"
            End If
        End If
' ADD  END  MARGE
    Else
        '�h���b�O���h���b�v�œǂݍ��ރt�@�C�����w�肳�ꂽ�ꍇ
        
        '�t�@�C�����������̏ꍇ�͂��̂܂܏I��
        If fname = "" Then
            TerminateSRC
            End
        End If
        
        '�V�i���I�̂���t�H���_�̃p�X������
        If InStr(fname, "\") > 0 Then
            For i = 1 To Len(fname)
                If Mid$(fname, Len(fname) - i + 1, 1) = "\" Then
                    Exit For
                End If
            Next
            ScenarioPath = Left$(fname, Len(fname) - i)
        Else
            ScenarioPath = AppPath
        End If
        If Right$(ScenarioPath, 1) <> "\" Then
            ScenarioPath = ScenarioPath & "\"
        End If
        
        '�g���f�[�^�̃t�H���_��ݒ�
        ExtDataPath = ReadIni("Option", "ExtDataPath")
        If Len(ExtDataPath) > 0 Then
            If Right$(ExtDataPath, 1) <> "\" Then
                ExtDataPath = ExtDataPath & "\"
            End If
        End If
        ExtDataPath2 = ReadIni("Option", "ExtDataPath2")
        If Len(ExtDataPath2) > 0 Then
            If Right$(ExtDataPath2, 1) <> "\" Then
                ExtDataPath2 = ExtDataPath2 & "\"
            End If
        End If
        
        '�I�[�v�j���O�ȉ��t
        StopBGM True
        StartBGM BGMName("Opening"), True
        
        InitEventData
        
        CloseTitleForm
        
        Screen.MousePointer = 0
    End If
    
    '�����O�l�[���ɂ��Ă���
    fname = ScenarioPath & Dir$(fname)
    If Not FileExists(fname) Then
        ErrorMessage "�w�肵���t�@�C�������݂��܂���"
        TerminateSRC
    End If
    If InStr(fname, "�s�v�t�@�C���폜") = 0 _
        And InStr(fname, "�K�{�C��") = 0 _
    Then
        '�J�����t�H���_��Src.ini�ɃZ�[�u���Ă���
        WriteIni "Log", "LastFolder", ScenarioPath
    End If
    
    'Src.ini����e��p�����[�^�̓ǂݍ���
    
    '�X�y�V�����p���[�A�j��
    buf = ReadIni("Option", "SpecialPowerAnimation")
    If buf = "" Then
        buf = ReadIni("Option", "MindEffect")
        If buf <> "" Then
            WriteIni "Option", "SpecialPowerAnimation", buf
        End If
    End If
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            SpecialPowerAnimation = True
        Else
            SpecialPowerAnimation = False
        End If
    Else
        If SpecialPowerAnimation Then
            WriteIni "Option", "SpecialPowerAnimation", "On"
        Else
            WriteIni "Option", "SpecialPowerAnimation", "Off"
        End If
    End If
    
    '�퓬�A�j��
    buf = LCase$(ReadIni("Option", "BattleAnimation"))
    If buf <> "" Then
        If buf = "on" Then
            BattleAnimation = True
        Else
            BattleAnimation = False
        End If
    Else
        If BattleAnimation Then
            WriteIni "Option", "BattleAnimation", "On"
        Else
            WriteIni "Option", "BattleAnimation", "Off"
        End If
    End If
    
    '�g��퓬�A�j��
    buf = LCase$(ReadIni("Option", "ExtendedAnimation"))
    If buf <> "" Then
        If buf = "on" Then
            ExtendedAnimation = True
        Else
            ExtendedAnimation = False
        End If
    Else
        ExtendedAnimation = True
        WriteIni "Option", "ExtendedAnimation", "On"
    End If
    
    '���폀���A�j��
    buf = LCase$(ReadIni("Option", "WeaponAnimation"))
    If buf <> "" Then
        If buf = "on" Then
            WeaponAnimation = True
        Else
            WeaponAnimation = False
        End If
    Else
        WeaponAnimation = True
        WriteIni "Option", "WeaponAnimation", "On"
    End If
    
    '�ړ��A�j��
    buf = LCase$(ReadIni("Option", "MoveAnimation"))
    If buf <> "" Then
        If buf = "on" Then
            MoveAnimation = True
        Else
            MoveAnimation = False
        End If
    Else
        MoveAnimation = True
        WriteIni "Option", "MoveAnimation", "On"
    End If
    
    '���b�Z�[�W���x��ݒ�
    buf = ReadIni("Option", "MessageWait")
    If IsNumeric(buf) Then
        MessageWait = CLng(buf)
        If MessageWait > 10000000 Then
            MessageWait = 10000000
        End If
    Else
        MessageWait = 700
        WriteIni "Option", "MessageWait", "700"
    End If
    
    '�}�X�ڂ�\�����邩�ǂ���
    buf = ReadIni("Option", "Square")
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            ShowSquareLine = True
        Else
            ShowSquareLine = False
        End If
    Else
        ShowSquareLine = False
        WriteIni "Option", "Square", "Off"
    End If
    
    '�G�^�[���ɂa�f�l��ύX���邩�ǂ���
    buf = ReadIni("Option", "KeepEnemyBGM")
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            KeepEnemyBGM = True
        Else
            KeepEnemyBGM = False
        End If
    Else
        KeepEnemyBGM = False
        WriteIni "Option", "KeepEnemyBGM", "Off"
    End If
    
    '�����̃��Z�b�g�f�[�^�̎��
    MidiResetType = ReadIni("Option", "MidiReset")
    
    '�����������[�h
    buf = ReadIni("Option", "AutoDefense")
    If buf = "" Then
        buf = ReadIni("Option", "AutoDeffence")
        If buf <> "" Then
            WriteIni "Option", "AutoDefense", buf
        End If
    End If
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = True
        Else
            MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = False
        End If
    Else
        MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = False
        WriteIni "Option", "AutoDefense", "Off"
    End If
    
    '�J�[�\�������ړ�
    buf = ReadIni("Option", "AutoMoveCursor")
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            AutoMoveCursor = True
        Else
            AutoMoveCursor = False
        End If
    Else
        AutoMoveCursor = True
        WriteIni "Option", "AutoMoveCursor", "On"
    End If
    
    '�e�E�B���h�E�����[�h (���C���E�B���h�E�͐�Ƀ��[�h�ς�)
    LoadForms
    
    '�摜�o�b�t�@�̖���
    buf = ReadIni("Option", "ImageBufferNum")
    If IsNumeric(buf) Then
        ImageBufferSize = CInt(buf)
        If ImageBufferSize < 5 Then
            '�Œ�ł�5���̃o�b�t�@���g��
            ImageBufferSize = 5
        End If
    Else
        '�f�t�H���g��64��
        ImageBufferSize = 64
        WriteIni "Option", "ImageBufferNum", "64"
    End If
    
    '�摜�o�b�t�@���쐬
    MakePicBuf
    
    '�摜�o�b�t�@�̍ő�T�C�Y
    buf = ReadIni("Option", "MaxImageBufferSize")
    If IsNumeric(buf) Then
        MaxImageBufferByteSize = CDbl(buf) * 1024 * 1024
        If MaxImageBufferByteSize < CLng(1) * 1024 * 1024 Then
            '�Œ�ł�1MB�̃o�b�t�@���g��
            MaxImageBufferByteSize = CLng(1) * 1024 * 1024
        End If
    Else
        '�f�t�H���g��8MB
        MaxImageBufferByteSize = CLng(8) * 1024 * 1024
        WriteIni "Option", "MaxImageBufferSize", "8"
    End If
    
    '�g��摜���摜�o�b�t�@�ɕۑ����邩
    buf = ReadIni("Option", "KeepStretchedImage")
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            KeepStretchedImage = True
        Else
            KeepStretchedImage = False
        End If
    Else
        If IsBitBltFasterThanStretchBlt() Then
            KeepStretchedImage = True
            WriteIni "Option", "KeepStretchedImage", "On"
        Else
            KeepStretchedImage = False
            WriteIni "Option", "KeepStretchedImage", "Off"
        End If
    End If
    
    '���ߕ`���UseTransparentBlt���g�p���邩
    If GetWinVersion() >= 500 Then
        buf = ReadIni("Option", "UseTransparentBlt")
        If buf <> "" Then
            If LCase$(buf) = "on" Then
                UseTransparentBlt = True
            Else
                UseTransparentBlt = False
            End If
        Else
            UseTransparentBlt = True
            WriteIni "Option", "UseTransparentBlt", "On"
        End If
    End If
    
    
    '�}�E�X�{�^���̗����r�ݒ�
    If GetSystemMetrics(SM_SWAPBUTTON) = 0 Then
        '�E�����p
        RButtonID = &H2
        LButtonID = &H1
    Else
        '�������p
        RButtonID = &H1
        LButtonID = &H2
    End If
    
    
    ReDim ListItemComment(0)
    
    '�G���A�X�f�[�^�����[�h
    If FileExists(ScenarioPath & "Data\System\alias.txt") Then
        ALDList.Load ScenarioPath & "Data\System\alias.txt"
    ElseIf FileExists(AppPath & "Data\System\alias.txt") Then
        ALDList.Load AppPath & "Data\System\alias.txt"
    End If
    '�X�y�V�����p���[�f�[�^�����[�h
    If FileExists(ScenarioPath & "Data\System\sp.txt") Then
        SPDList.Load ScenarioPath & "Data\System\sp.txt"
    ElseIf FileExists(ScenarioPath & "Data\System\mind.txt") Then
        SPDList.Load ScenarioPath & "Data\System\mind.txt"
    ElseIf FileExists(AppPath & "Data\System\sp.txt") Then
        SPDList.Load AppPath & "Data\System\sp.txt"
    ElseIf FileExists(AppPath & "Data\System\mind.txt") Then
        SPDList.Load AppPath & "Data\System\mind.txt"
    End If
    '�ėp�A�C�e���f�[�^�����[�h
    If FileExists(ScenarioPath & "Data\System\item.txt") Then
        IDList.Load ScenarioPath & "Data\System\item.txt"
    ElseIf FileExists(AppPath & "Data\System\item.txt") Then
        IDList.Load AppPath & "Data\System\item.txt"
    End If
    '�n�`�f�[�^�����[�h
    If FileExists(AppPath & "Data\System\terrain.txt") Then
        TDList.Load AppPath & "Data\System\terrain.txt"
    Else
        ErrorMessage "�n�`�f�[�^�t�@�C���uData\System\terrain.txt�v��������܂���"
        TerminateSRC
    End If
    If FileExists(ScenarioPath & "Data\System\terrain.txt") Then
        TDList.Load ScenarioPath & "Data\System\terrain.txt"
    End If
    '�o�g���R���t�B�O�f�[�^�����[�h
    If FileExists(ScenarioPath & "Data\System\battle.txt") Then
        BCList.Load ScenarioPath & "Data\System\battle.txt"
    ElseIf FileExists(AppPath & "Data\System\battle.txt") Then
        BCList.Load AppPath & "Data\System\battle.txt"
    End If
    
    '�}�b�v��������
    InitMap
    
    '�����n���������
    RndSeed = Int(1000000 * Rnd)
    RndReset
    
    If LCase$(Right$(fname, 4)) = ".src" Then
        SaveDataFileNumber = FreeFile
        Open fname For Input As #SaveDataFileNumber
        '��P���ڂ�ǂݍ���
        Input #SaveDataFileNumber, buf
        '��P���ڂ̓Z�[�u�f�[�^�o�[�W�����H
        If IsNumeric(buf) Then
            If CLng(buf) > 10000 Then
                '�o�[�W�����f�[�^�ł���Α�Q���ڂ�ǂݍ���
                Input #SaveDataFileNumber, buf
            End If
        End If
        Close #SaveDataFileNumber
        
        '�f�[�^�̎�ނ𔻒�
        If IsNumeric(buf) Then
            '�Z�[�u�f�[�^�̓ǂݍ���
            OpenNowLoadingForm
            LoadData fname
            CloseNowLoadingForm
            
            '�C���^�[�~�b�V����
            InterMissionCommand True
            
            If Not IsSubStage Then
                If GetValueAsString("���X�e�[�W") = "" Then
                    ErrorMessage "���̃X�e�[�W�̃t�@�C�������ݒ肳��Ă��܂���"
                    TerminateSRC
                End If
                
                StartScenario GetValueAsString("���X�e�[�W")
            Else
                IsSubStage = False
            End If
        Else
            '���f�f�[�^�̓ǂݍ���
            LockGUI
            
            RestoreData fname, False
            
            '��ʂ����������ăX�e�[�^�X��\��
            RedrawScreen
            DisplayGlobalStatus
            
            UnlockGUI
        End If
    ElseIf LCase$(Right$(fname, 4)) = ".eve" Then
        '�C�x���g�t�@�C���̎��s
        StartScenario fname
    Else
        ErrorMessage "�u" & fname & "�v��SRC�p�̃t�@�C���ł͂���܂���I" & vbCr & vbLf _
            & "�g���q���u.eve�v�̃C�x���g�t�@�C���A" _
            & "�܂��͊g���q���u.src�v�̃Z�[�u�f�[�^�t�@�C�����w�肵�ĉ������B"
        TerminateSRC
    End If
End Sub

'INI�t�@�C�����쐬����
Public Sub CreateIniFile()
Dim f As Integer

    On Error GoTo ErrorHandler
    
    f = FreeFile
    Open AppPath & "Src.ini" For Output Access Write As #f
    
    Print #f, ";SRC�̐ݒ�t�@�C���ł��B"
    Print #f, ";���ڂ̓��e�Ɋւ��Ă̓w���v��"
    Print #f, "; ������@ => �}�b�v�R�}���h => �ݒ�ύX"
    Print #f, ";�̍����Q�Ƃ��ĉ������B"
    Print #f, ""
    Print #f, "[Option]"
    Print #f, ";���b�Z�[�W�̃E�F�C�g�B�W����700"
    Print #f, "MessageWait=700"
    Print #f, ""
    Print #f, ";�^�[�����̕\�� [On|Off]"
    Print #f, "Turn=Off"
    Print #f, ""
    Print #f, ";�}�X�ڂ̕\�� [On|Off]"
    Print #f, "Square=Off"
    Print #f, ""
    Print #f, ";�G�t�F�C�Y�ɂ͂a�f�l��ύX���Ȃ� [On|Off]"
    Print #f, "KeepEnemyBGM=Off"
    Print #f, ""
    Print #f, ";�����h�䃂�[�h [On|Off]"
    Print #f, "AutoDefense=Off"
    Print #f, ""
    Print #f, ";�����J�[�\���ړ� [On|Off]"
    Print #f, "AutoMoveCursor=On"
    Print #f, ""
    Print #f, ";�X�y�V�����p���[�A�j�� [On|Off]"
    Print #f, "SpecialPowerAnimation=On"
    Print #f, ""
    Print #f, ";�퓬�A�j�� [On|Off]"
    Print #f, "BattleAnimation=On"
    Print #f, ""
    Print #f, ";�퓬�A�j���̊g���@�\ [On|Off]"
    Print #f, "ExtendedAnimation=On"
    Print #f, ""
    Print #f, ";���폀���A�j���̎����I��\�� [On|Off]"
    Print #f, "WeaponAnimation=On"
    Print #f, ""
    Print #f, ";�ړ��A�j�� [On|Off]"
    Print #f, "MoveAnimation=On"
    Print #f, ""
    Print #f, ";MIDI�������Z�b�g�̎�� [None|GM|GS|XG]"
    Print #f, "MidiReset=None"
    Print #f, ""
    Print #f, ";MIDI���t��DirectMusic���g�� [On|Off]"
    If GetWinVersion() >= 500 Then
        'NT�n��OS�ł̓f�t�H���g��DirectMusic���g��
        'DirectMusic�̏����������݂�
        InitDirectMusic
        'DirectMusic���g�p�\���ǂ����Őݒ��؂�ւ�
        If UseDirectMusic Then
            Print #f, "UseDirectMusic=On"
        Else
            Print #f, "UseDirectMusic=Off"
        End If
    Else
        'NT�nOS�łȂ����MCI���g��
        UseMCI = True
        Print #f, "UseDirectMusic=Off"
    End If
    Print #f, ""
    Print #f, ";DirectMusic�Ŏg��MIDI�����̃|�[�g�ԍ� [��������=0]"
    Print #f, "MIDIPortID=0"
    Print #f, ""
    Print #f, ";MP3�Đ����̉��� (0�`100)"
    Print #f, "MP3Volume=50"
    Print #f, ""
    Print #f, ";MP3�̏o�̓t���[����"
    Print #f, "MP3OutputBlock=20"
    Print #f, ""
    Print #f, ";MP3�̓��͒���̃X���[�v����(�~���b)"
    Print #f, "MP3IutputSleep=5"
    Print #f, ""
'    Print #f, ";WAV�Đ���DirectSound���g�� [On|Off]"
'    Print #f, "UseDirectSound=On"
'    Print #f, ""
    Print #f, ";�摜�o�b�t�@�̖���"
    Print #f, "ImageBufferNum=64"
    Print #f, ""
    Print #f, ";�摜�o�b�t�@�̍ő�T�C�Y (MB)"
    Print #f, "MaxImageBufferSize=8"
    Print #f, ""
    Print #f, ";�g��摜���摜�o�b�t�@�ɕۑ����� [On|Off]"
    Print #f, "KeepStretchedImage="
    Print #f, ""
    If GetWinVersion() >= 500 Then
        Print #f, ";���ߕ`���API�֐�TransparentBlt���g�� [On|Off]"
        Print #f, "UseTransparentBlt=On"
        Print #f, ""
    End If
    Print #f, ";�g���f�[�^�̃t�H���_ (�t���p�X�Ŏw��)"
    Print #f, "ExtDataPath="
    Print #f, "ExtDataPath2="
    Print #f, ""
    Print #f, ";�f�o�b�O���[�h [On|Off]"
    Print #f, "DebugMode=Off"
    Print #f, ""
    Print #f, ";�V�f�t�h(�e�X�g��) [On|Off]"
    Print #f, "NewGUI=Off"
    Print #f, ""
    Print #f, "[Log]"
    Print #f, ";�O��g�p�����t�H���_"
    Print #f, "LastFolder="
    Print #f, ""
    Print #f, "[BGM]"
    Print #f, ";SRC�N����"
    Print #f, "Opening=Opening.mid"
    Print #f, ";�����t�F�C�Y�J�n��"
    Print #f, "Map1=Map1.mid"
    Print #f, ";�G�t�F�C�Y�J�n��"
    Print #f, "Map2=Map2.mid"
    Print #f, ";�����}�b�v�̖����t�F�C�Y�J�n��"
    Print #f, "Map3=Map3.mid"
    Print #f, ";�����}�b�v�̓G�t�F�C�Y�J�n��"
    Print #f, "Map4=Map4.mid"
    Print #f, ";�F���}�b�v�̖����t�F�C�Y�J�n��"
    Print #f, "Map5=Map5.mid"
    Print #f, ";�F���}�b�v�̓G�t�F�C�Y�J�n��"
    Print #f, "Map6=Map6.mid"
    Print #f, ";�v�����[�O�E�G�s���[�O�J�n��"
    Print #f, "Briefing=Briefing.mid"
    Print #f, ";�C���^�[�~�b�V�����J�n��"
    Print #f, "Intermission=Intermission.mid"
    Print #f, ";�e���b�v�\����"
    Print #f, "Subtitle=Subtitle.mid"
    Print #f, ";�Q�[���I�[�o�[��"
    Print #f, "End=End.mid"
    Print #f, ";�퓬���̃f�t�H���gMIDI"
    Print #f, "default=default.mid"
    Print #f, ""
    
    Close #f
    
ErrorHandler:
    '�G���[����
End Sub

'KeepStretchedImage���g�p���ׂ������肷�邽�߁ABitBlt��
'StretchBlt�̑��x���𑪒�
Private Function IsBitBltFasterThanStretchBlt()
Dim stime As Long, etime As Long
Dim bb_time As Long, sb_time As Long
Dim ret As Long, i As Integer
  
    With MainForm
        '�`��̈��ݒ�
        With .picStretchedTmp(0)
            .width = 128
            .Height = 128
        End With
        With .picStretchedTmp(1)
            .width = 128
            .Height = 128
        End With
        
        'StretchBlt�̓]�����x�𑪒�
        stime = timeGetTime()
        For i = 1 To 5
            ret = StretchBlt(.picStretchedTmp(0).hDC, 0, 0, 480, 480, _
                .picUnit.hDC, 0, 0, 32, 32, SRCCOPY)
        Next
        etime = timeGetTime()
        sb_time = etime - stime
        
        'BitBlt�̓]�����x�𑪒�
        stime = timeGetTime()
        For i = 1 To 5
            ret = BitBlt(.picStretchedTmp(1).hDC, 0, 0, 480, 480, _
                .picStretchedTmp(0).hDC, 0, 0, SRCCOPY)
        Next
        etime = timeGetTime()
        bb_time = etime - stime
        
        '�`��̈���J��
        With .picStretchedTmp(0)
            .Picture = LoadPicture()
            .width = 32
            .Height = 32
        End With
        With .picStretchedTmp(1)
            .Picture = LoadPicture()
            .width = 32
            .Height = 32
        End With
    End With
    
    'BitBlt��StretchBlt���2�{�ȏ㑬�����BitBlt��D�悵�Ďg�p����
    If 2 * bb_time < sb_time Then
        IsBitBltFasterThanStretchBlt = True
    Else
        IsBitBltFasterThanStretchBlt = False
    End If
End Function


'�C�x���g�t�@�C��fname�����s
Public Sub StartScenario(ByVal fname As String)
Dim i As Integer, ret As Long
Dim sf As StdFont
    
    '�t�@�C��������
    If Len(fname) = 0 Then
        TerminateSRC
        End
    ElseIf FileExists(ScenarioPath & fname) Then
        fname = ScenarioPath & fname
    ElseIf FileExists(AppPath & fname) Then
        fname = AppPath & fname
    End If
    
    If Dir$(fname, vbNormal) = "" Then
        MsgBox fname & "��������܂���"
        TerminateSRC
    End If
    
    '�E�B���h�E�̃^�C�g����ݒ�
    If App.Minor Mod 2 = 0 Then
        MainForm.Caption = "SRC"
    Else
        MainForm.Caption = "SRC�J����"
    End If
    
    ScenarioFileName = fname
    
    If Not IsSubStage Then
        If Len(Dir$(ScenarioPath & "Date", vbDirectory)) > 0 Then
            ErrorMessage "�V�i���I����Data�t�H���_����Date�ɂȂ��Ă��܂��B" & vbCr & vbLf _
                & ScenarioPath & "Date" & vbCr & vbLf _
                & "�t�H���_����Data�ɒ����Ă��������B"
            TerminateSRC
        End If
        If Len(Dir$(ScenarioPath & "�c������", vbDirectory)) > 0 Then
            ErrorMessage "�V�i���I����Data�t�H���_�����S�p�����ɂȂ��Ă��܂��B" & vbCr & vbLf _
                & ScenarioPath & "�c������" & vbCr & vbLf _
                & "�t�H���_���𔼊p�����ɒ����Ă��������B"
            TerminateSRC
        End If
        If Len(Dir$(ScenarioPath & "�a����������", vbDirectory)) > 0 Then
            ErrorMessage "�V�i���I����Bitmap�t�H���_�����S�p�����ɂȂ��Ă��܂��B" & vbCr & vbLf _
                & ScenarioPath & "�a����������" & vbCr & vbLf _
                & "�t�H���_���𔼊p�����ɒ����Ă��������B"
            TerminateSRC
        End If
        If Len(Dir$(ScenarioPath & "�k����", vbDirectory)) > 0 Then
            ErrorMessage "�V�i���I����Lib�t�H���_�����S�p�����ɂȂ��Ă��܂��B" & vbCr & vbLf _
                & ScenarioPath & "�k����" & vbCr & vbLf _
                & "�t�H���_���𔼊p�����ɒ����Ă��������B"
            TerminateSRC
        End If
        If Len(Dir$(ScenarioPath & "�l������", vbDirectory)) > 0 Then
            ErrorMessage "�V�i���I����Midi�t�H���_�����S�p�����ɂȂ��Ă��܂��B" & vbCr & vbLf _
                & ScenarioPath & "�l������" & vbCr & vbLf _
                & "�t�H���_���𔼊p�����ɒ����Ă��������B"
            TerminateSRC
        End If
        If Len(Dir$(ScenarioPath & "�r��������", vbDirectory)) > 0 Then
            ErrorMessage "�V�i���I����Sound�t�H���_�����S�p�����ɂȂ��Ă��܂��B" & vbCr & vbLf _
                & ScenarioPath & "�r��������" & vbCr & vbLf _
                & "�t�H���_���𔼊p�����ɒ����Ă��������B"
            TerminateSRC
        End If
        
        '�ǂݍ��ރC�x���g�t�@�C�����ɍ��킹�Ċe��V�X�e���ϐ���ݒ�
        If Not IsGlobalVariableDefined("���X�e�[�W") Then
            DefineGlobalVariable "���X�e�[�W"
        End If
        SetVariableAsString "���X�e�[�W", ""
        For i = 1 To Len(fname)
            If Mid$(fname, Len(fname) - i + 1, 1) = "\" Then
                Exit For
            End If
        Next
        SetVariableAsString "�X�e�[�W", Mid$(fname, Len(fname) - i + 2)
        
        If Not IsGlobalVariableDefined("�Z�[�u�f�[�^�t�@�C����") Then
            DefineGlobalVariable "�Z�[�u�f�[�^�t�@�C����"
        End If
        SetVariableAsString "�Z�[�u�f�[�^�t�@�C����", _
            Mid$(fname, Len(fname) - i + 2, i - 5) & "�܂ŃN���A.src"
        
        '�E�B���h�E�̃^�C�g���ɃV�i���I�t�@�C������\��
        MainForm.Caption = MainForm.Caption & " - " & Mid$(fname, Len(fname) - i + 2, i - 5)
    End If
    
    '��ʂ��N���A���Ă���
    With MainForm
        ret = PatBlt(.picMain(0).hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
        ret = PatBlt(.picMain(1).hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
    End With
    ScreenIsSaved = True
    
    '�C�x���g�f�[�^�̓ǂݍ���
    LoadEventData fname
    
    '�e��ϐ��̏�����
    Turn = 0
    IsScenarioFinished = False
    IsPictureVisible = False
    IsCursorVisible = False
    LastSaveDataFileName = ""
    IsRestartSaveDataAvailable = False
    IsQuickSaveDataAvailable = False
    CommandState = "���j�b�g�I��"
    ReDim SelectedPartners(0)
    
    '�t�H���g�ݒ���f�t�H���g�ɖ߂�
    With MainForm.picMain(0)
        .ForeColor = rgb(255, 255, 255)
        If .Font.Name <> "�l�r �o����" Then
            Set sf = New StdFont
            sf.Name = "�l�r �o����"
            Set .Font = sf
        End If
        .Font.Size = 16
        .Font.Bold = True
        .Font.Italic = False
        PermanentStringMode = False
        KeepStringMode = False
    End With
    
    '�`��̊���W�ʒu�����Z�b�g
    ResetBasePoint
    
    '������������߂��Ȃ��悤�Ƀ��j�b�g�摜���N���A
    If Not IsSubStage Then
        UList.ClearUnitBitmap
    End If
    
    LockGUI
    
    If MapWidth = 1 Then
        SetMapSize 15, 15
    End If
    
    '�v�����[�O
    Stage = "�v�����[�O"
    If Not IsSubStage And IsEventDefined("�v�����[�O", True) Then
        StopBGM
        StartBGM BGMName("Briefing")
    End If
    HandleEvent "�v�����[�O"
    
    If IsScenarioFinished Then
        IsScenarioFinished = False
        UnlockGUI
        Exit Sub
    End If
    
    If Not IsEventDefined("�X�^�[�g") Then
        ErrorMessage "�X�^�[�g�C�x���g����`����Ă��܂���"
        TerminateSRC
    End If
    
    IsPictureVisible = False
    IsCursorVisible = False
    Stage = "����"
    StopBGM
    
    '���X�^�[�g�p�Ƀf�[�^���Z�[�u
    If InStr(fname, "\Lib\���j�b�g�X�e�[�^�X�\��.eve") = 0 _
        And InStr(fname, "\Lib\�p�C���b�g�X�e�[�^�X�\��.eve") = 0 _
    Then
        DumpData ScenarioPath & "_���X�^�[�g.src"
    End If
    
    '�X�^�[�g�C�x���g���n�܂����ꍇ�͒ʏ�̃X�e�[�W�Ƃ݂Ȃ�
    IsSubStage = False
    
    ClearUnitStatus
    If Not MainForm.Visible Then
        MainForm.Show
        MainForm.Refresh
    End If
    RedrawScreen
    
    '�X�^�[�g�C�x���g
    HandleEvent "�X�^�[�g"
    If IsScenarioFinished Then
        IsScenarioFinished = False
        UnlockGUI
        Exit Sub
    End If
    
    IsPictureVisible = False
    IsCursorVisible = False
    
    '�N�C�b�N���[�h�𖳌��ɂ���
    IsQuickSaveDataAvailable = False
    
    StartTurn "����"
End Sub

'�w�cuparty�̃t�F�C�Y�����s
Public Sub StartTurn(uparty As String)
Dim i As Integer, num As Integer, phase As Integer
Dim u As Unit
    
    Stage = uparty
    BossBGM = False
    
    If uparty = "����" Then
        Do
            '�����t�F�C�Y
            Stage = "����"
            
            '�^�[������i�߂�
            If MapFileName <> "" Then
                Turn = Turn + 1
                TotalTurn = TotalTurn + 1
            End If
            
            '��ԉ�
            For Each SelectedUnit In UList
                With SelectedUnit
                    Select Case .Status
                        Case "�o��", "�i�["
                            If .Party = uparty Then
                                If MapFileName = "" Then
                                    .UsedAction = 0
                                Else
                                    .Rest
                                End If
                                If IsScenarioFinished Then
                                    UnlockGUI
                                    Exit Sub
                                End If
                            Else
                                .UsedAction = 0
                            End If
                        Case "����`��", "���`��"
                            .UsedAction = 0
                    End Select
                End With
            Next
            
            '�������G�ɂ������X�y�V�����p���[������
            For Each u In UList
                With u
                    Select Case .Status
                        Case "�o��", "�i�["
                            .RemoveSpecialPowerInEffect "�G�^�[��"
                    End Select
                End With
            Next
            RedrawScreen
            
            '�����t�F�C�Y�p�a�f�l�����t
            If MapFileName <> "" Then
                Select Case TerrainClass(1, 1)
                    Case "����"
                        StartBGM BGMName("Map3")
                    Case "�F��"
                        StartBGM BGMName("Map5")
                    Case Else
                        If TerrainName(1, 1) = "��" Then
                            StartBGM BGMName("Map3")
                        Else
                            StartBGM BGMName("Map1")
                        End If
                End Select
            End If
            
            '�^�[���C�x���g
            IsUnitCenter = False
            HandleEvent "�^�[��", Turn, "����"
            If IsScenarioFinished Then
                UnlockGUI
                Exit Sub
            End If
            
            '����\�ȃ��j�b�g�����邩�ǂ����`�F�b�N
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
            If num > 0 Or IsOptionDefined("�����t�F�C�Y��������") Then
                Exit Do
            End If
            
            'CPU�����삷�郆�j�b�g�����邩�ǂ����`�F�b�N
            num = 0
            For Each u In UList
                With u
                    If .Party <> "����" _
                        And .Status = "�o��" _
                    Then
                        num = num + 1
                    End If
                End With
            Next
            If num = 0 Then
                Exit Do
            End If
            
            '�G�t�F�C�Y
            StartTurn "�G"
            If IsScenarioFinished Then
                IsScenarioFinished = False
                Exit Sub
            End If
            
            '�����t�F�C�Y
            StartTurn "����"
            If IsScenarioFinished Then
                IsScenarioFinished = False
                Exit Sub
            End If
            
            '�m�o�b�t�F�C�Y
            StartTurn "�m�o�b"
            If IsScenarioFinished Then
                IsScenarioFinished = False
                Exit Sub
            End If
        Loop While True
    Else
        '�����t�F�C�Y�ȊO
        
        '��ԉ�
        For Each SelectedUnit In UList
            With SelectedUnit
                Select Case .Status
                    Case "�o��", "�i�["
                        If .Party = uparty Then
                            .Rest
                            If IsScenarioFinished Then
                                UnlockGUI
                                Exit Sub
                            End If
                        Else
                            .UsedAction = 0
                        End If
                    Case "����`��", "���`��"
                        .UsedAction = 0
                End Select
            End With
        Next
        
        '�G���j�b�g�������ɂ������X�y�V�����p���[������
        For Each u In UList
            With u
                Select Case .Status
                    Case "�o��", "�i�["
                        .RemoveSpecialPowerInEffect "�G�^�[��"
                End Select
            End With
        Next
        RedrawScreen
        
        '�G(�m�o�b)�t�F�C�Y�p�a�f�l�����t
        Select Case TerrainClass(1, 1)
            Case "����"
                If Stage = "�m�o�b" Then
                    StartBGM BGMName("Map3")
                Else
                    StartBGM BGMName("Map4")
                End If
            Case "�F��"
                If Stage = "�m�o�b" Then
                    StartBGM BGMName("Map5")
                Else
                    StartBGM BGMName("Map6")
                End If
            Case Else
                If Stage = "�m�o�b" Then
                    If TerrainName(1, 1) = "��" Then
                        StartBGM BGMName("Map3")
                    Else
                        StartBGM BGMName("Map1")
                    End If
                Else
                    If TerrainName(1, 1) = "��" Then
                        StartBGM BGMName("Map4")
                    Else
                        StartBGM BGMName("Map2")
                    End If
                End If
        End Select
        
        '�^�[���C�x���g
        HandleEvent "�^�[��", Turn, uparty
        If IsScenarioFinished Then
            UnlockGUI
            Exit Sub
        End If
    End If
    
    If uparty = "����" Then
        '�����t�F�C�Y�̃v���C���[�ɂ�郆�j�b�g����O�̏���
        
        '�^�[������\��
        If Turn > 1 And IsOptionDefined("�f�o�b�O") Then
            DisplayTelop "�^�[��" & Format$(Turn)
        End If
        
        '�ʏ�̃X�e�[�W�ł͕�̓��j�b�g�܂��̓��x���������Ƃ�����
        '���j�b�g�𒆉��ɔz�u
        If MapFileName <> "" And Not IsUnitCenter Then
            Dim max_lv As Integer, max_unit As Unit
            
            For Each u In UList
                With u
                    If .Party = "����" And .Status = "�o��" And .Action > 0 Then
                        If .IsFeatureAvailable("���") Then
                            Center .X, .Y
                            DisplayUnitStatus u
                            RedrawScreen
                            UnlockGUI
                            Exit Sub
                        End If
                    End If
                End With
            Next
            
            max_lv = 0
            For Each u In UList
                With u
                    If .Party = "����" And .Status = "�o��" Then
                        If .MainPilot.Level > max_lv Then
                            Set max_unit = u
                            max_lv = .MainPilot.Level
                        End If
                    End If
                End With
            Next
            If Not max_unit Is Nothing Then
                Center max_unit.X, max_unit.Y
            End If
        End If
        
        '�X�e�[�^�X��\��
        If MapFileName <> "" Then
            DisplayGlobalStatus
        End If
        
        '�v���C���[�ɂ�閡�����j�b�g����Ɉڍs
        RedrawScreen
        DoEvents
        UnlockGUI
        Exit Sub
    End If
    
    LockGUI
    
    'CPU�ɂ�郆�j�b�g����
    For phase = 1 To 5
    For i = 1 To UList.Count
        '�t�F�C�Y���ɍs�����郆�j�b�g��I��
        Set SelectedUnit = UList.Item(i)
        
        With SelectedUnit
            If .Status <> "�o��" Then
                GoTo NextLoop
            End If
            
            If .Action = 0 Then
                GoTo NextLoop
            End If
            
            If .Party <> uparty Then
                GoTo NextLoop
            End If
            
            Set u = SelectedUnit
            
            '���̃��j�b�g����q���Ă��郆�j�b�g�͌�q�ΏۂƓ������ɍs��
            If PList.IsDefined(.Mode) Then
                With PList.Item(.Mode)
                    If Not .Unit Is Nothing Then
                        If .Unit.Party = uparty Then
                            Set u = .Unit
                        End If
                    End If
                End With
            End If
            If PList.IsDefined(u.Mode) Then
                With PList.Item(u.Mode)
                    If Not .Unit Is Nothing Then
                        If .Unit.Party = uparty Then
                            Set u = .Unit
                        End If
                    End If
                End With
            End If
            
            With u
                Select Case phase
                    Case 1
                        '�ŏ��ɃT�|�[�g�\�͂������Ȃ��U�R���j�b�g���s��
                        If .BossRank >= 0 Then
                            GoTo NextLoop
                        End If
                        With .MainPilot
                            If .IsSkillAvailable("����") _
                                Or .IsSkillAvailable("����U��") _
                                Or .IsSkillAvailable("����h��") _
                                Or .IsSkillAvailable("����") _
                                Or .IsSkillAvailable("�w��") _
                                Or .IsSkillAvailable("�L��T�|�[�g") _
                            Then
                                GoTo NextLoop
                            End If
                        End With
                    Case 2
                        '���ɃT�|�[�g�\�͂������Ȃ��{�X���j�b�g���s��
                        With .MainPilot
                            If .IsSkillAvailable("����") _
                                Or .IsSkillAvailable("����U��") _
                                Or .IsSkillAvailable("����h��") _
                                Or .IsSkillAvailable("����") _
                                Or .IsSkillAvailable("�w��") _
                                Or .IsSkillAvailable("�L��T�|�[�g") _
                            Then
                                GoTo NextLoop
                            End If
                        End With
                    Case 3
                        '���ɓ����\�͂������j�b�g���s��
                        If Not .MainPilot.IsSkillAvailable("����") Then
                            GoTo NextLoop
                        End If
                    Case 4
                        '���ɃT�|�[�g�\�͂����U�R���j�b�g���s��
                        If .BossRank >= 0 Then
                            GoTo NextLoop
                        End If
                    Case 5
                        '�Ō�ɃT�|�[�g�\�͂����{�X���j�b�g���s��
                End Select
            End With
        End With
        
        Do While SelectedUnit.Action > 0
            '�r���ŏ�Ԃ��ύX���ꂽ�ꍇ
            If SelectedUnit.Status <> "�o��" Then
                Exit Do
            End If
            
            '�r���Őw�c���ύX���ꂽ�ꍇ
            If SelectedUnit.Party <> uparty Then
                Exit Do
            End If
            
            If Not IsRButtonPressed Then
                DisplayUnitStatus SelectedUnit
                Center SelectedUnit.X, SelectedUnit.Y
                RedrawScreen
                DoEvents
            End If
            
            IsCanceled = False 'Cancel�R�}���h�̃N���A
            
            '���j�b�g���s��������
            OperateUnit
            
            If IsScenarioFinished Then
                Exit Sub
            End If
            
            '�n�C�p�[���[�h�E�m�[�}�����[�h�̎��������`�F�b�N
            UList.CheckAutoHyperMode
            UList.CheckAutoNormalMode
            
            'Cancel�R�}���h�����s���ꂽ�炱���ŏI��
            If IsCanceled Then
                If SelectedUnit Is Nothing Then
                    Exit Do
                End If
                If SelectedUnit.Status <> "�o��" Then
                    Exit Do
                End If
                IsCanceled = False
            End If
            
            '�s����������
            SelectedUnit.UseAction
            
            '�ڐG�C�x���g
            With SelectedUnit
                If .Status = "�o��" And .X > 1 Then
                    If Not MapDataForUnit(.X - 1, .Y) Is Nothing Then
                        Set SelectedTarget = MapDataForUnit(.X - 1, .Y)
                        HandleEvent "�ڐG", .MainPilot.ID, _
                            MapDataForUnit(.X - 1, .Y).MainPilot.ID
                        If IsScenarioFinished Then
                            Exit Sub
                        End If
                    End If
                End If
            End With
            With SelectedUnit
                If .Status = "�o��" And .X < MapWidth Then
                    If Not MapDataForUnit(.X + 1, .Y) Is Nothing Then
                        Set SelectedTarget = MapDataForUnit(.X + 1, .Y)
                        HandleEvent "�ڐG", .MainPilot.ID, _
                            MapDataForUnit(.X + 1, .Y).MainPilot.ID
                        If IsScenarioFinished Then
                            Exit Sub
                        End If
                    End If
                End If
            End With
            With SelectedUnit
                If .Status = "�o��" And .Y > 1 Then
                    If Not MapDataForUnit(.X, .Y - 1) Is Nothing Then
                        Set SelectedTarget = MapDataForUnit(.X, .Y - 1)
                        HandleEvent "�ڐG", .MainPilot.ID, _
                            MapDataForUnit(.X, .Y - 1).MainPilot.ID
                        If IsScenarioFinished Then
                            Exit Sub
                        End If
                    End If
                End If
            End With
            With SelectedUnit
                If .Status = "�o��" And .Y < MapHeight Then
                    If Not MapDataForUnit(.X, .Y + 1) Is Nothing Then
                        Set SelectedTarget = MapDataForUnit(.X, .Y + 1)
                        HandleEvent "�ڐG", .MainPilot.ID, _
                            MapDataForUnit(.X, .Y + 1).MainPilot.ID
                        If IsScenarioFinished Then
                            Exit Sub
                        End If
                    End If
                End If
            End With
            
            '�i���C�x���g
            With SelectedUnit
                If .Status = "�o��" Then
                    HandleEvent "�i��", .MainPilot.ID, .X, .Y
                    If IsScenarioFinished Then
                        Exit Sub
                    End If
                End If
            End With
            
            '�s���I���C�x���g
            With SelectedUnit
                If .Status = "�o��" Then
                    HandleEvent "�s���I��", .MainPilot.ID
                    If IsScenarioFinished Then
                        Exit Sub
                    End If
                End If
            End With
        Loop
NextLoop:
    Next
    Next
    
    '�X�e�[�^�X�E�B���h�E�̕\��������
    ClearUnitStatus
End Sub

'�Q�[���I�[�o�[
Public Sub GameOver()
Dim fname As String

    KeepBGM = False
    BossBGM = False
    StopBGM
    MainForm.Hide
    
    'GameOver.eve��T��
    If FileExists(ScenarioPath & "Data\System\GameOver.eve") Then
        fname = ScenarioPath & "Data\System\GameOver.eve"
        If FileExists(ScenarioPath & "Data\System\non_pilot.txt") Then
            NPDList.Load ScenarioPath & "Data\System\non_pilot.txt"
        End If
    ElseIf FileExists(AppPath & "Data\System\GameOver.eve") Then
        fname = AppPath & "Data\System\GameOver.eve"
        If FileExists(AppPath & "Data\System\non_pilot.txt") Then
            NPDList.Load AppPath & "Data\System\non_pilot.txt"
        End If
    Else
        'GameOver.eve��������΂��̂܂܏I��
        TerminateSRC
    End If
    
    'GameOver.eve��ǂݍ���
    ClearEventData
    LoadEventData fname
    ScenarioFileName = fname
    
    If Not IsEventDefined("�v�����[�O") Then
        ErrorMessage fname & "���Ƀv�����[�O�C�x���g����`����Ă��܂���"
        TerminateSRC
    End If
    
    'GameOver.eve�̃v�����[�O�C�x���g�����{
    HandleEvent "�v�����[�O"
End Sub

'�Q�[���N���A
Public Sub GameClear()
    TerminateSRC
End Sub

'�Q�[����r���I��
Public Sub ExitGame()
Dim fname As String

    KeepBGM = False
    BossBGM = False
    StopBGM
    
    'Exit.eve��T��
    MainForm.Hide
    If FileExists(ScenarioPath & "Data\System\Exit.eve") Then
        fname = ScenarioPath & "Data\System\Exit.eve"
        If FileExists(ScenarioPath & "Data\System\non_pilot.txt") Then
            NPDList.Load ScenarioPath & "Data\System\non_pilot.txt"
        End If
    ElseIf FileExists(AppPath & "Data\System\Exit.eve") Then
        fname = AppPath & "Data\System\Exit.eve"
        If FileExists(AppPath & "Data\System\non_pilot.txt") Then
            NPDList.Load AppPath & "Data\System\non_pilot.txt"
        End If
    Else
        'Exit.eve��������΂��̂܂܏I��
        TerminateSRC
    End If
    
    'Exit.eve��ǂݍ���
    ClearEventData
    LoadEventData fname
    
    If Not IsEventDefined("�v�����[�O") Then
        ErrorMessage fname & "���Ƀv�����[�O�C�x���g����`����Ă��܂���"
        TerminateSRC
    End If
    
    'Exit.eve�̃v�����[�O�C�x���g�����{
    HandleEvent "�v�����[�O"
    
    'SRC���I��
    TerminateSRC
End Sub

'SRC���I��
Public Sub TerminateSRC()
Dim i As Integer, j As Integer

    '�E�B���h�E�����
    If Not MainForm Is Nothing Then
        MainForm.Hide
    End If
    Load frmMessage
    If frmMessage.Visible Then
        CloseMessageForm
    End If
    Load frmListBox
    If frmListBox.Visible Then
        frmListBox.Hide
    End If
    Load frmNowLoading
    If frmNowLoading.Visible Then
        frmNowLoading.Hide
    End If
    DoEvents
    
    '���ԉ𑜓x�����ɖ߂�
    Call timeEndPeriod(1)
    
    '�t���X�N���[�����[�h���g���Ă����ꍇ�͉𑜓x�����ɖ߂�
    If ReadIni("Option", "FullScreen") = "On" Then
        ChangeDisplaySize 0, 0
    End If
    
    '�a�f�l�E���ʉ��̍Đ����~
    FreeSoundModule
    
    '�e��f�[�^�����
    
    Set SelectedUnit = Nothing
    Set SelectedTarget = Nothing
    Set SelectedPilot = Nothing
    Set DisplayedUnit = Nothing
    
    For i = 1 To MapWidth
        For j = 1 To MapHeight
            Set MapDataForUnit(i, j) = Nothing
        Next
    Next
    
    With GlobalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set GlobalVariableList = Nothing
    
    With LocalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set LocalVariableList = Nothing
    
    Set SelectedUnitForEvent = Nothing
    Set SelectedTargetForEvent = Nothing
    
    Set UList = Nothing
    Set PList = Nothing
    Set IList = Nothing
    
    '�Ȃ������ꂪ�Ȃ��ƕs���I������c�c
    DoEvents
    
    Set PDList = Nothing
    Set NPDList = Nothing
    Set UDList = Nothing
    Set IDList = Nothing
    Set MDList = Nothing
    Set EDList = Nothing
    Set ADList = Nothing
    Set EADList = Nothing
    Set DDList = Nothing
    Set SPDList = Nothing
    Set ALDList = Nothing
    Set TDList = Nothing
    Set BCList = Nothing
    
    End
End Sub


'�f�[�^���Z�[�u
Public Sub SaveData(fname As String)
Dim i As Integer, num As Long

    On Error GoTo ErrorHandler
    
    SaveDataFileNumber = FreeFile
    Open fname For Output Access Write As #SaveDataFileNumber
    
    With App
        num = 10000 * .Major + 100 * .Minor + .Revision
    End With
    Write #SaveDataFileNumber, num
    
    Write #SaveDataFileNumber, UBound(Titles)
    For i = 1 To UBound(Titles)
        Write #SaveDataFileNumber, Titles(i)
    Next
    
    Write #SaveDataFileNumber, GetValueAsString("���X�e�[�W")
    
    Write #SaveDataFileNumber, TotalTurn
    Write #SaveDataFileNumber, Money
    Write #SaveDataFileNumber, 0 '�p�[�c�p�̃_�~�[
    
    SaveGlobalVariables
    PList.Save
    UList.Save
    IList.Save
    
    Close #SaveDataFileNumber
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "�Z�[�u���ɃG���[���������܂���"
    Close #SaveDataFileNumber
End Sub

'�f�[�^�����[�h
Public Sub LoadData(fname As String)
Dim i As Integer, num As Integer, fname2 As String
Dim dummy As String
Dim u As Unit

    On Error GoTo ErrorHandler
    
    SaveDataFileNumber = FreeFile
    Open fname For Input As #SaveDataFileNumber
    
    Input #SaveDataFileNumber, SaveDataVersion
    
    If SaveDataVersion > 10000 Then
        Input #SaveDataFileNumber, num
    Else
        num = SaveDataVersion
    End If
    
    SetLoadImageSize num * 2 + 5
    
    ReDim Titles(num)
    For i = 1 To num
        Input #SaveDataFileNumber, Titles(i)
        IncludeData Titles(i)
    Next
    
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
        
    Input #SaveDataFileNumber, fname2
    Input #SaveDataFileNumber, TotalTurn
    Input #SaveDataFileNumber, Money
    Input #SaveDataFileNumber, num '�p�[�c�p�̃_�~�[
    
    LoadGlobalVariables
    If Not IsGlobalVariableDefined("���X�e�[�W") Then
        DefineGlobalVariable "���X�e�[�W"
    End If
    SetVariableAsString "���X�e�[�W", fname2
    
    PList.Load
    UList.Load
    IList.Load
    
    Close #SaveDataFileNumber
    
    '�����N�f�[�^���������邽�߁A�Z�[�u�t�@�C������U���Ă���ēx�ǂݍ���
    
    SaveDataFileNumber = FreeFile
    Open fname For Input As #SaveDataFileNumber
    
    If SaveDataVersion > 10000 Then
        Input #SaveDataFileNumber, dummy
    End If
    Input #SaveDataFileNumber, num
    ReDim Titles(num)
    For i = 1 To num
        Input #SaveDataFileNumber, Titles(i)
    Next
    Input #SaveDataFileNumber, dummy
    Input #SaveDataFileNumber, TotalTurn
    Input #SaveDataFileNumber, Money
    Input #SaveDataFileNumber, num '�p�[�c�p�̃_�~�[
    Input #SaveDataFileNumber, num '�p�[�c�p�̃_�~�[
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    PList.LoadLinkInfo
    UList.LoadLinkInfo
    IList.LoadLinkInfo
    
    Close #SaveDataFileNumber
    
    DisplayLoadingProgress
    
    '���j�b�g�̏�Ԃ���
    For Each u In UList
        u.Reset
    Next
    
    DisplayLoadingProgress
    
    '�ǉ����ꂽ�V�X�e�����C�x���g�f�[�^�̓ǂݍ���
    LoadEventData ""
    
    DisplayLoadingProgress
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "���[�h���ɃG���[���������܂���"
    Close #SaveDataFileNumber
    TerminateSRC
End Sub


'�ꎞ���f�p�f�[�^���t�@�C���ɃZ�[�u����
Public Sub DumpData(fname As String)
Dim i As Integer, num As Long

    On Error GoTo ErrorHandler
    
    '���f�f�[�^���Z�[�u
    SaveDataFileNumber = FreeFile
    Open fname For Output Access Write As #SaveDataFileNumber
    
    With App
        num = 10000 * .Major + 100 * .Minor + .Revision
    End With
    Write #SaveDataFileNumber, num
    
    Write #SaveDataFileNumber, Mid$(ScenarioFileName, Len(ScenarioPath) + 1)
    
    Write #SaveDataFileNumber, UBound(Titles)
    For i = 1 To UBound(Titles)
        Write #SaveDataFileNumber, Titles(i)
    Next
    
    Write #SaveDataFileNumber, Turn
    Write #SaveDataFileNumber, TotalTurn
    Write #SaveDataFileNumber, Money
    
    DumpEventData
    
    PList.Dump
    IList.Dump
    UList.Dump
    
    DumpMapData
    
    ' Midi ����Ȃ��� midi ����Ȃ��ƌ������s����悤�ɂȂ��Ă�̂ŁB
    If InStr(LCase$(BGMFileName), "\midi\") > 0 Then
        Write #SaveDataFileNumber, Mid$(BGMFileName, InStr(LCase$(BGMFileName), "\midi\") + 6)
    ElseIf InStr(BGMFileName, "\") > 0 Then
        Write #SaveDataFileNumber, Mid$(BGMFileName, InStr(BGMFileName, "\") + 1)
    Else
        Write #SaveDataFileNumber, BGMFileName
    End If
    Write #SaveDataFileNumber, RepeatMode
    Write #SaveDataFileNumber, KeepBGM
    Write #SaveDataFileNumber, BossBGM
    
    Write #SaveDataFileNumber, RndSeed
    Write #SaveDataFileNumber, RndIndex
    
    Close #SaveDataFileNumber
    
    LastSaveDataFileName = fname
    If InStr(fname, "\_���X�^�[�g.src") > 0 Then
        IsRestartSaveDataAvailable = True
    ElseIf InStr(fname, "\_�N�C�b�N�Z�[�u.src") > 0 Then
        IsQuickSaveDataAvailable = True
    End If
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "�Z�[�u���ɃG���[���������܂���"
    Close #SaveDataFileNumber
End Sub

'�ꎞ���f�p�f�[�^�����[�h
Public Sub RestoreData(fname As String, quick_load As Boolean)
Dim i As Integer, num As Integer, fname2 As String
Dim dummy As String
Dim u As Unit
Dim scenario_file_is_different As Boolean

    On Error GoTo ErrorHandler
    
    '�}�E�X�J�[�\���������v��
    Screen.MousePointer = 11
    
    If quick_load Then
        If IsOptionDefined("�f�o�b�O") Then
            LoadEventData ScenarioFileName, "�N�C�b�N���[�h"
        End If
    End If
    
    If Not quick_load Then
        OpenNowLoadingForm
    End If
    
    SaveDataFileNumber = FreeFile
    Open fname For Input As #SaveDataFileNumber
    
    Input #SaveDataFileNumber, fname2
    
    If IsNumeric(fname2) Then
        SaveDataVersion = CLng(fname2)
        Input #SaveDataFileNumber, fname2
    Else
        SaveDataVersion = 1
    End If
    
    '�E�B���h�E�̃^�C�g����ݒ�
    If ScenarioFileName <> ScenarioPath & fname2 Then
        MainForm.Caption = "SRC - " & Left$(fname2, Len(fname2) - 4)
        ScenarioFileName = ScenarioPath & fname2
        scenario_file_is_different = True
    End If
    
    Input #SaveDataFileNumber, num
    
    '�g�p����f�[�^�����[�h
    If Not quick_load Then
        SetLoadImageSize num * 2 + 5
        
        ReDim Titles(num)
        For i = 1 To num
            Input #SaveDataFileNumber, Titles(i)
            IncludeData Titles(i)
        Next
        
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
        
        LoadEventData ScenarioFileName, "���X�g�A"
        
        DisplayLoadingProgress
    Else
        For i = 1 To num
            Line Input #SaveDataFileNumber, dummy
        Next
        
        If scenario_file_is_different Then
            LoadEventData ScenarioFileName, "���X�g�A"
        End If
    End If
    
    Input #SaveDataFileNumber, Turn
    Input #SaveDataFileNumber, TotalTurn
    Input #SaveDataFileNumber, Money
    
    RestoreEventData
    
    PList.Restore
    IList.Restore
    UList.Restore

'MOD START 240a
'    RestoreMapData
'    '�a�f�l�֘A�̐ݒ�𕜌�
'    Input #SaveDataFileNumber, fname2
    '�}�b�v�f�[�^�̌݊����ێ��̂��߁ARestoreMapData�ła�f�l�֘A�̂P�s�ڂ܂œǂݍ���Ŗ߂�l�ɂ���
    fname2 = RestoreMapData
'MOD  END  240a
    fname2 = SearchMidiFile("(" & fname2 & ")")
    If fname2 <> "" Then
        KeepBGM = False
        BossBGM = False
        ChangeBGM fname2
        Input #SaveDataFileNumber, RepeatMode
        Input #SaveDataFileNumber, KeepBGM
        Input #SaveDataFileNumber, BossBGM
    Else
        StopBGM
        Line Input #SaveDataFileNumber, dummy
        Line Input #SaveDataFileNumber, dummy
        Line Input #SaveDataFileNumber, dummy
    End If
    
    '�����n��𕜌�
    If Not IsOptionDefined("�f�o�b�O") _
        And Not IsOptionDefined("�����n���ۑ�") _
        And Not EOF(SaveDataFileNumber) _
    Then
        Input #SaveDataFileNumber, RndSeed
        RndReset
        Input #SaveDataFileNumber, RndIndex
    End If

    If Not quick_load Then
        DisplayLoadingProgress
    End If
    
    Close #SaveDataFileNumber
    
    '�����N�f�[�^���������邽�߁A�Z�[�u�t�@�C������U���Ă���ēx�ǂݍ���
    
    SaveDataFileNumber = FreeFile
    Open fname For Input As #SaveDataFileNumber
    
    'SaveDataVersion
    If SaveDataVersion > 10000 Then
        Line Input #SaveDataFileNumber, dummy
    End If
    
    'ScenarioFileName
    Line Input #SaveDataFileNumber, dummy
    
    '�g�p����f�[�^��
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    
    'Turn
    Line Input #SaveDataFileNumber, dummy
    'TotalTurn
    Line Input #SaveDataFileNumber, dummy
    'Money
    Line Input #SaveDataFileNumber, dummy
    
    SkipEventData
    
    PList.RestoreLinkInfo
    IList.RestoreLinkInfo
    UList.RestoreLinkInfo
    
    Close #SaveDataFileNumber
    
    '�p�����[�^�����������邽�߁A�Z�[�u�t�@�C������U���Ă���ēx�ǂݍ��݁B
    '��͂�g�o�A�d�m�Ƃ������p�����[�^�͍ő�l������\�͂ŕϓ����邽�߁A
    '����\�͂̐ݒ肪�I����Ă�����߂Đݒ肵�Ă��K�v������B
    
    SaveDataFileNumber = FreeFile
    Open fname For Input As #SaveDataFileNumber
    
    'SaveDataVersion
    If SaveDataVersion > 10000 Then
        Line Input #SaveDataFileNumber, dummy
    End If
    
    'ScenarioFileName
    Line Input #SaveDataFileNumber, dummy
    
    '�g�p����f�[�^��
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    
    'Turn
    Line Input #SaveDataFileNumber, dummy
    'TotalTurn
    Line Input #SaveDataFileNumber, dummy
    'Money
    Line Input #SaveDataFileNumber, dummy
    
    SkipEventData
    
    PList.RestoreParameter
    IList.RestoreParameter
    UList.RestoreParameter
    
    PList.UpdateSupportMod
    
    '�w�i��������
    If IsMapDirty Then
        Dim map_x As Integer, map_y As Integer
        
        map_x = MapX
        map_y = MapY
        
        SetupBackground MapDrawMode, "�񓯊�"
        
        MapX = map_x
        MapY = map_y
        
        '�ĊJ�C�x���g�ɂ��}�b�v�摜�̏��������������s��
        HandleEvent "�ĊJ"
        
        IsMapDirty = False
    End If
    
    Set SelectedUnit = Nothing
    Set SelectedTarget = Nothing
    
    '���j�b�g�摜����
    For Each u In UList
        With u
            If .BitmapID = 0 Then
                .BitmapID = MakeUnitBitmap(u)
            End If
        End With
    Next
    
    '��ʍX�V
    Center MapX, MapY
    
    Close #SaveDataFileNumber
    
    If Not quick_load Then
        DisplayLoadingProgress
    End If
    
    If Not quick_load Then
        CloseNowLoadingForm
    End If
    
    If Not quick_load Then
        MainForm.Show
    End If
    
    '�}�E�X�J�[�\�������ɖ߂�
    Screen.MousePointer = 0
    
    ClearUnitStatus
    If Not MainForm.Visible Then
        MainForm.Show
        MainForm.Refresh
    End If
    RedrawScreen
    
    If Turn = 0 Then
        HandleEvent "�X�^�[�g"
        
' MOD START MARGE
'        StartTurn "����"
        '�X�^�[�g�C�x���g���玟�̃X�e�[�W���J�n���ꂽ�ꍇ�AStartTurn�����HandleEvent��
        '���s����Ă��܂��B
        '�����^�[���̏������Q�d�N�������̂�h�����߁ATurn���`�F�b�N���Ă���N������
        If Turn = 0 Then
            StartTurn "����"
        End If
' MOD END MARGE
    Else
        CommandState = "���j�b�g�I��"
        Stage = "����"
    End If
    
    LastSaveDataFileName = fname
    If InStr(fname, "\_���X�^�[�g.src") > 0 Then
        IsRestartSaveDataAvailable = True
    ElseIf InStr(fname, "\_�N�C�b�N�Z�[�u.src") > 0 Then
        IsQuickSaveDataAvailable = True
    End If
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "���[�h���ɃG���[���������܂���"
    Close #SaveDataFileNumber
    TerminateSRC
End Sub


'���`���̃��j�b�g�h�c��V�`���ɕϊ�
'���`���j���j�b�g����+���l
'�V�`���j���j�b�g����+":"+���l
Public Sub ConvertUnitID(ID As String)
Dim i As Integer
    
    If InStr(ID, ":") > 0 Then
        Exit Sub
    End If
    
    '���l������ǂݔ�΂�
    i = Len(ID)
    Do While i > 0
        Select Case Asc(Mid$(ID, i, 1))
            Case 48 To 57
                '0-9
            Case Else
                Exit Do
        End Select
        i = i - 1
    Loop
    
    '���j�b�g���̂Ɛ��l�����̊ԂɁu:�v��}��
    ID = Left$(ID, i) & ":" & Mid$(ID, i + 1)
End Sub

'��inew_title�̃f�[�^��ǂݍ���
Public Sub IncludeData(new_title As String)
Dim fpath As String
    
    '���[�h�̃C���W�P�[�^�\�����s��
    If frmNowLoading.Visible Then
        DisplayLoadingProgress
    End If
    
    'Data�t�H���_�̏ꏊ��T��
    fpath = SearchDataFolder(new_title)
    
    If Len(fpath) = 0 Then
        ErrorMessage "�f�[�^�u" & new_title & "�v�̃t�H���_��������܂���"
        TerminateSRC
    End If
    
    On Error GoTo ErrorHandler
    
    If FileExists(fpath & "\alias.txt") Then
        ALDList.Load fpath & "\alias.txt"
    End If
    If FileExists(fpath & "\sp.txt") Then
        SPDList.Load fpath & "\sp.txt"
    ElseIf FileExists(fpath & "\mind.txt") Then
        SPDList.Load fpath & "\mind.txt"
    End If
    If FileExists(fpath & "\pilot.txt") Then
        PDList.Load fpath & "\pilot.txt"
    End If
    If FileExists(fpath & "\non_pilot.txt") Then
        NPDList.Load fpath & "\non_pilot.txt"
    End If
    If FileExists(fpath & "\robot.txt") Then
        UDList.Load fpath & "\robot.txt"
    End If
    If FileExists(fpath & "\unit.txt") Then
        UDList.Load fpath & "\unit.txt"
    End If
    
    '���[�h�̃C���W�P�[�^�\�����s��
    If frmNowLoading.Visible Then
        DisplayLoadingProgress
    End If
    
    If FileExists(fpath & "\pilot_message.txt") Then
        MDList.Load fpath & "\pilot_message.txt"
    End If
    If FileExists(fpath & "\pilot_dialog.txt") Then
        DDList.Load fpath & "\pilot_dialog.txt"
    End If
    If FileExists(fpath & "\effect.txt") Then
        EDList.Load fpath & "\effect.txt"
    End If
    If FileExists(fpath & "\animation.txt") Then
        ADList.Load fpath & "\animation.txt"
    End If
    If FileExists(fpath & "\ext_animation.txt") Then
        EADList.Load fpath & "\ext_animation.txt"
    End If
    If FileExists(fpath & "\item.txt") Then
        IDList.Load fpath & "\item.txt"
    End If
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "Src.ini����ExtDataPath�̒l���s���ł�"
    TerminateSRC
End Sub

'�f�[�^�t�H���_ fname ������
Public Function SearchDataFolder(fname As String) As String
Dim fname2 As String
Static init_search_data_folder As Boolean
Static scenario_data_dir_exists As Boolean
Static extdata_data_dir_exists As Boolean
Static extdata2_data_dir_exists As Boolean
Static src_data_dir_exists As Boolean

    '���߂Ď��s����ۂɁA�e�t�H���_��Data�t�H���_�����邩�`�F�b�N
    If Not init_search_data_folder Then
        If Len(Dir$(ScenarioPath & "Data", vbDirectory)) > 0 Then
            scenario_data_dir_exists = True
        End If
        If Len(ExtDataPath) > 0 And ScenarioPath <> ExtDataPath Then
            If Len(Dir$(ExtDataPath & "Data", vbDirectory)) > 0 Then
                extdata_data_dir_exists = True
            End If
        End If
        If Len(ExtDataPath2) > 0 And ScenarioPath <> ExtDataPath2 Then
            If Len(Dir$(ExtDataPath2 & "Data", vbDirectory)) > 0 Then
                extdata2_data_dir_exists = True
            End If
        End If
        If ScenarioPath <> AppPath Then
            If Len(Dir$(AppPath & "Data", vbDirectory)) > 0 Then
                src_data_dir_exists = True
            End If
        End If
        init_search_data_folder = True
    End If
    
    '�t�H���_������
    fname2 = "Data\" & fname
    If scenario_data_dir_exists Then
        SearchDataFolder = ScenarioPath & fname2
        If Len(Dir$(SearchDataFolder, vbDirectory)) > 0 Then
            Exit Function
        End If
    End If
    If extdata_data_dir_exists Then
        SearchDataFolder = ExtDataPath & fname2
        If Len(Dir$(SearchDataFolder, vbDirectory)) > 0 Then
            Exit Function
        End If
    End If
    If extdata2_data_dir_exists Then
        SearchDataFolder = ExtDataPath2 & fname2
        If Len(Dir$(SearchDataFolder, vbDirectory)) > 0 Then
            Exit Function
        End If
    End If
    If src_data_dir_exists Then
        SearchDataFolder = AppPath & fname2
        If Len(Dir$(SearchDataFolder, vbDirectory)) > 0 Then
            Exit Function
        End If
    End If
    
    '�t�H���_��������Ȃ�����
    SearchDataFolder = ""
End Function

'�����̗ʂ�ύX����
Public Sub IncrMoney(ByVal earnings As Long)
    Money = MinLng(Money + earnings, 999999999)
    Money = MaxLng(Money, 0)
End Sub
