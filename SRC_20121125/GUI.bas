Attribute VB_Name = "GUI"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'���[�U�[�C���^�[�t�F�[�X�Ɖ�ʕ`��̏������s�����W���[��

'Main��Form
Public MainForm As Form
Public IsFlashAvailable As Boolean

' ADD START MARGE
'GUI���V�o�[�W������
Public NewGUIMode As Boolean
' ADD END

'�}�b�v��ʂɕ\���ł���}�b�v�̃T�C�Y
Public MainWidth As Integer
Public MainHeight As Integer

'�}�b�v��ʂ̃T�C�Y�i�s�N�Z���j
Public MainPWidth As Integer
Public MainPHeight As Integer

'�}�b�v�̃T�C�Y�i�s�N�Z���j
Public MapPWidth As Integer
Public MapPHeight As Integer

'�g�o�E�d�m�̃Q�[�W�̕��i�s�N�Z���j
Public Const GauageWidth = 88

'���݃}�b�v�E�B���h�E���}�X�N�\������Ă��邩
Public ScreenIsMasked As Boolean
'���݃}�b�v�E�B���h�E���ۑ�����Ă��邩
Public ScreenIsSaved As Boolean

'���ݕ\������Ă���}�b�v�̍��W
Public MapX As Integer
Public MapY As Integer

'�h���b�O�O�̃}�b�v�̍��W
Public PrevMapX As Integer
Public PrevMapY As Integer

'�Ō�ɉ����ꂽ�}�E�X�{�^��
Public MouseButton As Integer

'���݂̃}�E�X�̍��W
Public MouseX As Single
Public MouseY As Single

'�h���b�O�O�̃}�E�X�̍��W
Public PrevMouseX As Single
Public PrevMouseY As Single

'�J�[�\���ʒu�����ύX�O�̃}�E�X�J�[�\���̍��W
Private PrevCursorX As Integer
Private PrevCursorY As Integer
'�J�[�\���ʒu�����ύX��̃}�E�X�J�[�\���̍��W
Private NewCursorX As Integer
Private NewCursorY As Integer

'�ړ��O�̃��j�b�g�̏��
Public PrevUnitX As Integer
Public PrevUnitY As Integer
Public PrevUnitArea As String
Public PrevCommand As String

'PaintPicture�ŉ摜���`�����܂ꂽ��
Public IsPictureDrawn As Boolean
'PaintPicture�ŉ摜���`����Ă��邩
Public IsPictureVisible As Boolean
'PaintPicture�ŕ`�悵���摜�̈�
Public PaintedAreaX1 As Integer
Public PaintedAreaY1 As Integer
Public PaintedAreaX2 As Integer
Public PaintedAreaY2 As Integer
'�J�[�\���摜���\������Ă��邩
Public IsCursorVisible As Boolean
'�w�i�F
Public BGColor As Long

'�摜�o�b�t�@�Ǘ��p�ϐ�
Private PicBufDateCount As Long
Private PicBufDate() As Long
Private PicBufSize() As Long
Private PicBufFname() As String
Private PicBufOption() As String
Private PicBufOption2() As String
Private PicBufDW() As Integer
Private PicBufDH() As Integer
Private PicBufSX() As Integer
Private PicBufSY() As Integer
Private PicBufSW() As Integer
Private PicBufSH() As Integer
Private PicBufIsMask() As Boolean


'GUI������͉\���ǂ���
Public IsGUILocked As Boolean

'���X�g�{�b�N�X���ŕ\���ʒu
Public TopItem As Integer

'���b�Z�[�W�E�C���h�E�ɂɊւ�����
Private DisplayedPilot As String
Private DisplayMode As String
Private RightUnit As Unit
Private LeftUnit As Unit
Private RightUnitHPRatio As Double
Private LeftUnitHPRatio As Double
Private RightUnitENRatio As Double
Private LeftUnitENRatio As Double
Public MessageWindowIsOut As Boolean

'���b�Z�[�W�E�B���h�E�̏�Ԃ�ێ����邽�߂̕ϐ�
Private IsMessageFormVisible As Boolean
Private SavedLeftUnit As Unit
Private SavedRightUnit As Unit

'�t�H�[�����N���b�N���ꂽ��
Public IsFormClicked As Boolean
'�t�H�[�������[�_����
Public IsMordal As Boolean

'���b�Z�[�W�\���̃E�F�C�g
Public MessageWait As Long

'���b�Z�[�W���������肩�ǂ���
Public AutoMessageMode As Boolean

'PaintString�̒����\���̐ݒ�
Public HCentering As Boolean
Public VCentering As Boolean
'PaintString�̏������݂��w�i�ɍs���邩�ǂ���
Public PermanentStringMode As Boolean
'PaintString�̏������݂����������ǂ���
Public KeepStringMode As Boolean


'ListBox�p�ϐ�
Public ListItemFlag() As Boolean
Public ListItemComment() As String
Public ListItemID() As String
Public MaxListItem As Integer


'API�֐��̒�`

Declare Function BitBlt Lib "gdi32" (ByVal hDestDC As Long, _
    ByVal X As Long, ByVal Y As Long, _
    ByVal nWidth As Long, ByVal nHeight As Long, ByVal hSrcDC As Long, _
    ByVal xsrc As Long, ByVal ysrc As Long, ByVal dwRop As Long) As Long
    
Declare Function StretchBlt Lib "gdi32" (ByVal hDestDC As Long, _
    ByVal X As Long, ByVal Y As Long, _
    ByVal nWidth As Long, ByVal nHeight As Long, ByVal hSrcDC As Long, _
    ByVal xsrc As Long, ByVal ysrc As Long, _
    ByVal nSrcWidth As Long, ByVal nSrcHeight As Long, _
    ByVal dwRop As Long) As Long

Declare Function PatBlt Lib "gdi32" (ByVal hDC As Long, _
    ByVal X As Long, ByVal Y As Long, _
    ByVal nWidth As Long, ByVal nHeight As Long, _
    ByVal dwRop As Long) As Long

Public Const BLACKNESS = &H42
Public Const DSTINVERT = &H550009
Public Const MERGECOPY = &HC000CA
Public Const MERGEPAINT = &HBB0226
Public Const NOTSRCCOPY = &H330008
Public Const NOTSRCERASE = &H1100A6
Public Const PATCOPY = &HF00021
Public Const PATINVERT = &H5A0049
Public Const PATPAINT = &HFB0A09
Public Const SRCAND = &H8800C6
Public Const SRCCOPY = &HCC0020
Public Const SRCERASE = &H440328
Public Const SRCINVERT = &H660046
Public Const SRCPAINT = &HEE0086
Public Const WHITENESS = &HFF0062
'ADD START 240a
Public Const STATUSBACK = &HC0C0C0
'ADD START 240a

'StretchBlt�̃��[�h�ݒ���s��
Declare Function GetStretchBltMode Lib "gdi32" (ByVal hDC As Long) As Long
Declare Function SetStretchBltMode Lib "gdi32" (ByVal hDC As Long, _
    ByVal nStretchMode As Long) As Long

Public Const STRETCH_ANDSCANS = 1
Public Const STRETCH_ORSCANS = 2
Public Const STRETCH_DELETESCANS = 3
Public Const STRETCH_HALFTONE = 4

'���ߕ`��
Declare Function TransparentBlt Lib "msimg32.dll" (ByVal hDC As Long, _
    ByVal X As Long, ByVal Y As Long, ByVal nWidth As Long, ByVal nHeight As Long, _
    ByVal hSrcDC As Long, ByVal xsrc As Long, ByVal ysrc As Long, ByVal nSrcWidth As Long, _
    ByVal nSrcHeight As Long, ByVal crTransparent As Long) As Long

'�E�B���h�E�ʒu�̐ݒ�
Declare Function SetWindowPos Lib "user32" ( _
    ByVal hwnd As Long, ByVal hWndInsertAfter As Long, _
    ByVal X As Long, ByVal Y As Long, ByVal cx As Long, _
    ByVal cy As Long, ByVal wFlags As Long) As Long

Public Const SW_SHOWNA = 8    '��A�N�e�B�u�ŕ\��

'�t�H�[�����A�N�e�B�u�ɂ��Ȃ��ŕ\��
Declare Function ShowWindow Lib "user32" ( _
    ByVal hwnd As Long, ByVal nCmdShow As Long) As Long

Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)

'�J�[�\���ʒu�擾
Declare Function GetCursorPos Lib "user32" (lpPoint As POINTAPI) As Long

'�|�C���g�\����
Type POINTAPI
    X As Long
    Y As Long
End Type

'�J�[�\���ʒu�ݒ�
Declare Function SetCursorPos Lib "user32" (ByVal X As Long, ByVal Y As Long) As Long

'�L�[�̏��𓾂�
Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Long) As Integer

Public RButtonID As Long
Public LButtonID As Long

'�V�X�e�����g���b�N�X���擾����API
Declare Function GetSystemMetrics Lib "user32" (ByVal nIndex As Long) As Long

Public Const SM_SWAPBUTTON = 23    '���E�̃{�^������������Ă��邩�ۂ�

'���݃A�N�e�B�u�ȃE�B���h�E���擾����API
Public Declare Function GetForegroundWindow Lib "user32" () As Long

'������`�悷�邽�߂�API
Declare Function MoveToEx Lib "gdi32" (ByVal hDC As Long, _
    ByVal X As Long, ByVal Y As Long, lpPoint As POINTAPI) As Long
Declare Function LineTo Lib "gdi32" (ByVal hDC As Long, _
    ByVal X As Long, ByVal Y As Long) As Long

'���p�`��`�悷��API
Public Declare Function Polygon Lib "gdi32.dll" (ByVal hDC As Long, _
    lpPoint As POINTAPI, ByVal nCount As Long) As Long


'�f�B�X�v���C�̐ݒ���Q�Ƃ���API
Public Type DEVMODE
    dmDeviceName As String * 32
    dmSpecVersion As Integer
    dmDriverVersion As Integer
    dmSize As Integer
    dmDriverExtra As Integer
    dmFields As Long
    dmOrientation As Integer
    dmPaperSize As Integer
    dmPaperLength As Integer
    dmPaperWidth As Integer
    dmScale As Integer
    dmCopies As Integer
    dmDefaultSource As Integer
    dmPrintQuality As Integer
    dmColor As Integer
    dmDuplex As Integer
    dmYResolution As Integer
    dmTTOption As Integer
    dmCollate As Integer
    dmFormName As String * 32
    dmUnusedPadding As Integer
    dmBitsPerPixel As Integer
    dmPelsWidth As Long
    dmPelsHeight As Long
    dmDisplayFlags As Long
    dmDisplayFrequency As Long
    dmICMMethod As Long
    dmICMIntent As Long
    dmMediaType As Long
    dmDitherType As Long
    dmReserved1 As Long
    dmReserved2 As Long
    dmPanningWidth As Long
    dmPanningHeight As Long
End Type

Public Declare Function EnumDisplaySettings Lib "user32.dll" Alias "EnumDisplaySettingsA" _
    (ByVal lpszDeviceName As String, ByVal iModeNum As Long, lpDevMode As DEVMODE) As Long

Public Const ENUM_CURRENT_SETTINGS = -1

'�f�B�X�v���C�̐ݒ��ύX���邽�߂�API
Public Declare Function ChangeDisplaySettings Lib "user32.dll" Alias "ChangeDisplaySettingsA" _
    (lpDevMode As Any, ByVal dwFlags As Long) As Long

Public Const CDS_UPDATEREGISTRY = &H1
Public Const CDS_TEST = &H2
Public Const CDS_FULLSCREEN = &H4
Public Const DISP_CHANGE_SUCCESSFUL = 0
Public Const DISP_CHANGE_RESTART = 1

'�f�o�C�X�̐ݒ���Q�Ƃ��邽�߂�API
Public Declare Function GetDeviceCaps Lib "gdi32" _
    (ByVal hDC As Long, ByVal nIndex As Long) As Long

'�s�N�Z��������̃J���[�r�b�g��
Private Const BITSPIXEL = 12


'�V�X�e���p�����[�^��ύX���邽�߂�API
Declare Function SetSystemParametersInfo Lib "user32.dll" Alias "SystemParametersInfoA" _
    (ByVal uiAction As Long, ByVal uiParam As Long, ByVal pvParam As Long, _
    ByVal fWinIni As Long) As Long

Declare Function GetSystemParametersInfo Lib "user32.dll" Alias "SystemParametersInfoA" _
    (ByVal uiAction As Long, ByVal uiParam As Long, pvParam As Long, _
    ByVal fWinIni As Long) As Long

'�t�H���g�̃X���[�W���O�����֘A�̒萔
Public Const SPI_GETFONTSMOOTHING = 74
Public Const SPI_SETFONTSMOOTHING = 75

'���[�U�[�v���t�@�C���̍X�V���w��
Public Const SPIF_UPDATEINIFILE = &H1
'���ׂẴg�b�v���x���E�B���h�E�ɕύX��ʒm
Public Const SPIF_SENDWININICHANGE = &H2


'���C���E�B���h�E�̃��[�h��Flash�̓o�^���s��
Public Sub LoadMainFormAndRegisterFlash()
Dim WSHShell As Object
    
    On Error GoTo ErrorHandler
    
    '�V�F������regsvr32.exe�𗘗p���āA�N�����Ƃ�SRC.exe�Ɠ����p�X�ɂ���
    'FlashControl.ocx���ēo�^����B
    Set WSHShell = CreateObject("WScript.Shell")
    WSHShell.Run "regsvr32.exe /s """ & AppPath & "FlashControl.ocx""", 0, True
    Set WSHShell = Nothing
    
    Load frmMain
    
    Set MainForm = frmMain
    IsFlashAvailable = True
    
    Exit Sub
    
ErrorHandler:
    
    'Flash���g���Ȃ��̂�Flash�����̃��C���E�B���h�E���g�p����
    Load frmSafeMain
    Set MainForm = frmSafeMain
End Sub
    
'�e�E�B���h�E�����[�h
'���������C���E�B���h�E�͂��炩����LoadMainFormAndRegisterFlash�Ń��[�h���Ă�������
Public Sub LoadForms()
Dim X As Integer, Y As Integer

    Load frmToolTip
    Load frmMessage
    Load frmListBox
    
    LockGUI
    CommandState = "���j�b�g�I��"
    
    '�}�b�v��ʂɕ\���ł���}�b�v�̃T�C�Y
    Select Case LCase$(ReadIni("Option", "NewGUI"))
        Case "on"
' MOD START MARGE
            NewGUIMode = True
' MOD END MARGE
            MainWidth = 20
        Case "off"
            MainWidth = 15
        Case Else
            MainWidth = 15
            WriteIni "Option", "NewGUI", "Off"
    End Select
' ADD START MARGE
    ' Option�Œ�`����Ă���΂������D�悷��
    If IsOptionDefined("�V�f�t�h") Then
        NewGUIMode = True
        MainWidth = 20
    End If
' ADD END MARGE
    MainHeight = 15
    
    '�}�b�v��ʂ̃T�C�Y�i�s�N�Z���j
    MainPWidth = MainWidth * 32
    MainPHeight = MainHeight * 32
    
    With MainForm
        '���C���E�B���h�E�̈ʒu���T�C�Y��ݒ�
        X = Screen.TwipsPerPixelX
        Y = Screen.TwipsPerPixelY
' MOD START MARGE
'        If MainWidth = 15 Then
        If Not NewGUIMode Then
' MOD END MARGE
            .width = .width - (.ScaleWidth * X) + (MainPWidth + 24 + 225 + 4) * X
            .Height = .Height - (.ScaleHeight * Y) + (MainPHeight + 24) * Y
        Else
            .width = .width - (.ScaleWidth * X) + MainPWidth * X
            .Height = .Height - (.ScaleHeight * Y) + MainPHeight * Y
        End If
        .Left = (Screen.width - .width) / 2
        .Top = (Screen.Height - .Height) / 2
        
        '�X�N���[���o�[�̈ʒu��ݒ�
' MOD START MARGE
'        If MainWidth = 15 Then
        If Not NewGUIMode Then
' MOD END MARGE
            .VScroll.Move MainPWidth + 4, 4, 16, MainPWidth
            .HScroll.Move 4, MainPHeight + 4, MainPWidth, 16
        Else
            .VScroll.Visible = False
            .HScroll.Visible = False
        End If
        
        '�X�e�[�^�X�E�B���h�E��ݒu
' MOD START MARGE
'        If MainWidth = 15 Then
'            .picFace.Move MainPWidth + 24, 4
'            .picPilotStatus.Move MainPWidth + 24 + 68 + 4, 4, 155, 72
'            .picUnitStatus.Move MainPWidth + 24, 4 + 68 + 4, _
'                225 + 5, MainPHeight - 64 + 16
'        Else
'            .picUnitStatus.Move MainPWidth - 230 - 10, 10, 230, MainPHeight - 20
'            .picUnitStatus.Visible = False
'            .picPilotStatus.Visible = False
'            .picFace.Visible = False
'        End If
        If NewGUIMode Then
            .picUnitStatus.Move MainPWidth - 230 - 10, 10, 230, MainPHeight - 20
            .picUnitStatus.Visible = False
            .picPilotStatus.Visible = False
            .picFace.Visible = False
            StatusWindowBackBolor = STATUSBACK
            StatusWindowFrameColor = STATUSBACK
            StatusWindowFrameWidth = 1
            .picUnitStatus.BackColor = StatusWindowBackBolor
            StatusFontColorAbilityName = rgb(0, 0, 150)
            StatusFontColorAbilityEnable = vbBlue
            StatusFontColorAbilityDisable = rgb(150, 0, 0)
            StatusFontColorNormalString = vbBlack
        Else
            .picFace.Move MainPWidth + 24, 4
            .picPilotStatus.Move MainPWidth + 24 + 68 + 4, 4, 155, 72
            .picUnitStatus.Move MainPWidth + 24, 4 + 68 + 4, _
                225 + 5, MainPHeight - 64 + 16
        End If
' MOD END MARGE
        
        '�}�b�v�E�B���h�E�̃T�C�Y��ݒ�
' MOD START MARGE
'        If MainWidth = 15 Then
        If Not NewGUIMode Then
' MOD END MARGE
            .picMain(0).Move 4, 4, MainPWidth, MainPHeight
            .picMain(1).Move 4, 4, MainPWidth, MainPHeight
        Else
            .picMain(0).Move 0, 0, MainPWidth, MainPHeight
            .picMain(1).Move 0, 0, MainPWidth, MainPHeight
        End If
    End With
End Sub

' ADD START MARGE
'Option�ɂ��V�f�t�h���L�����ǂ������Đݒ肷��
Public Sub SetNewGUIMode()
    ' Option�Œ�`����Ă���̂�NewGUIMode��false�̏ꍇ�ALoadForms���Ă�
    If IsOptionDefined("�V�f�t�h") _
        And Not NewGUIMode _
    Then
        LoadForms
    End If
End Sub
' ADD  END  MARGE

' === ���b�Z�[�W�E�B���h�E�Ɋւ��鏈�� ===

'���b�Z�[�W�E�B���h�E���J��
'�퓬���b�Z�[�W��ʂȂǁA���j�b�g�\�����s���ꍇ�� u1, u2 �Ɏw��
Public Sub OpenMessageForm(Optional u1 As Unit, Optional u2 As Unit)
Dim tppx As Integer, tppy As Integer
Dim ret As Long
    
    Set RightUnit = Nothing
    Set LeftUnit = Nothing
    
    With Screen
        tppx = .TwipsPerPixelX
        tppy = .TwipsPerPixelY
    End With
    
    Load frmMessage
    With frmMessage
        '���j�b�g�\���𔺂��ꍇ�̓L���v�V��������u(��������)�v���폜
        If Not u1 Is Nothing Then
            If .Caption = "���b�Z�[�W (��������)" Then
                .Caption = "���b�Z�[�W"
            End If
        End If
        
        '���b�Z�[�W�E�B���h�E�������I�ɍŏ�������
        If .WindowState <> vbNormal Then
            .WindowState = vbNormal
            .Show
            .SetFocus
        End If
        
        If u1 Is Nothing Then
            '���j�b�g�\���Ȃ�
            .labHP1.Visible = False
            .labHP2.Visible = False
            .labEN1.Visible = False
            .labEN2.Visible = False
            .picHP1.Visible = False
            .picHP2.Visible = False
            .picEN1.Visible = False
            .picEN2.Visible = False
            .txtHP1.Visible = False
            .txtHP2.Visible = False
            .txtEN1.Visible = False
            .txtEN2.Visible = False
            .picUnit1.Visible = False
            .picUnit2.Visible = False
            
            .width = .width - .ScaleWidth * tppx + 508 * tppx
            .Height = .Height - .ScaleHeight * tppy + 84 * tppy
            
            .picFace.Top = 8
            .picFace.Left = 8
            .picMessage.Top = 7
            .picMessage.Left = 84
        ElseIf u2 Is Nothing Then
            '���j�b�g�\���P�̂̂�
            If u1.Party = "����" Or u1.Party = "�m�o�b" Then
                .labHP1.Visible = False
                .labEN1.Visible = False
                .picHP1.Visible = False
                .picEN1.Visible = False
                .txtHP1.Visible = False
                .txtEN1.Visible = False
                .picUnit1.Visible = False
                .labHP2.Visible = True
                .labEN2.Visible = True
                .picHP2.Visible = True
                .picEN2.Visible = True
                .txtHP2.Visible = True
                .txtEN2.Visible = True
                .picUnit2.Visible = True
            Else
                .labHP1.Visible = True
                .labEN1.Visible = True
                .picHP1.Visible = True
                .picEN1.Visible = True
                .txtHP1.Visible = True
                .txtEN1.Visible = True
                .picUnit1.Visible = True
                .labHP2.Visible = False
                .labEN2.Visible = False
                .picHP2.Visible = False
                .picEN2.Visible = False
                .txtHP2.Visible = False
                .txtEN2.Visible = False
                .picUnit2.Visible = False
            End If
            
            UpdateMessageForm u1
            
            .width = .width - .ScaleWidth * tppx + 508 * tppx
            .Height = .Height - .ScaleHeight * tppy + 118 * tppy
            
            .picFace.Top = 42
            .picFace.Left = 8
            .picMessage.Top = 41
            .picMessage.Left = 84
        Else
            '���j�b�g���Q�̕\��
            .labHP1.Visible = True
            .labHP2.Visible = True
            .labEN1.Visible = True
            .labEN2.Visible = True
            .picHP1.Visible = True
            .picHP2.Visible = True
            .picEN1.Visible = True
            .picEN2.Visible = True
            .txtHP1.Visible = True
            .txtHP2.Visible = True
            .txtEN1.Visible = True
            .txtEN2.Visible = True
            .picUnit1.Visible = True
            .picUnit2.Visible = True
            
            UpdateMessageForm u1, u2
            
            .width = .width - .ScaleWidth * tppx + 508 * tppx
            .Height = .Height - .ScaleHeight * tppy + 118 * tppy
            
            .picFace.Top = 42
            .picFace.Left = 8
            .picMessage.Top = 41
            .picMessage.Left = 84
        End If
        
        '���b�Z�[�W�E�B���h�E�̈ʒu�ݒ�
        If MainForm.Visible And Not MainForm.WindowState = 1 Then
            '���C���E�B���h�E���\������Ă���΃��C���E�B���h�E�̉��[�ɍ��킹�ĕ\��
            If Not frmMessage.Visible Then
                If MainWidth = 15 Then
                    .Left = MainForm.Left
                Else
                    .Left = MainForm.Left + (MainForm.width - .width) \ 2
                End If
                If MessageWindowIsOut Then
                    .Top = MainForm.Top + MainForm.Height - 350
                Else
                    .Top = MainForm.Top + MainForm.Height - .Height
                End If
            End If
        Else
            '���C���E�B���h�E���\������Ă��Ȃ��ꍇ�͉�ʒ����ɕ\��
            .Left = (Screen.width - .width) / 2
            .Top = (Screen.Height - .Height) / 2
        End If
        
        '�E�B���h�E���N���A���Ă���
        .picFace = LoadPicture("")
        DisplayedPilot = ""
        .picMessage.Cls
        
        '�E�B���h�E��\��
        .Show
        
        '��Ɏ�O�ɕ\������
        ret = SetWindowPos(.hwnd, -1, 0, 0, 0, 0, &H3)
    End With
    
    DoEvents
End Sub

'���b�Z�[�W�E�B���h�E�����
Public Sub CloseMessageForm()
    If Not frmMessage.Visible Then
        Exit Sub
    End If
    frmMessage.Hide
    DoEvents
End Sub

'���b�Z�[�W�E�B���h�E���N���A
Public Sub ClearMessageForm()
    With frmMessage
        .picFace = LoadPicture("")
        .picMessage.Cls
    End With
    DisplayedPilot = ""
    Set LeftUnit = Nothing
    Set RightUnit = Nothing
    DoEvents
End Sub

'���b�Z�[�W�E�B���h�E�ɕ\�����Ă��郆�j�b�g�����X�V
Public Sub UpdateMessageForm(u1 As Unit, Optional u2 As Variant)
Dim lu As Unit, ru As Unit
Dim ret As Long, i As Integer, buf As String
Dim num As Integer, tmp As Long

    With frmMessage
        '�E�B���h�E�Ƀ��j�b�g��񂪕\������Ă��Ȃ��ꍇ�͂��̂܂܏I��
        If .Visible Then
            If Not .picUnit1.Visible And Not .picUnit2.Visible Then
                Exit Sub
            End If
        End If
        
        'lu�����ɕ\�����郆�j�b�g�Aru���E�ɕ\�����郆�j�b�g�ɐݒ�
        If IsMissing(u2) Then
            '�P�̂̃��j�b�g�̂ݕ\��
            If u1.Party = "����" Or u1.Party = "�m�o�b" Then
                Set lu = Nothing
                Set ru = u1
            Else
                Set lu = u1
                Set ru = Nothing
            End If
        ElseIf u2 Is Nothing Then
            '���ˍU��
            '�O��\�����ꂽ���j�b�g�����̂܂܎g�p
            Set lu = LeftUnit
            Set ru = RightUnit
        ElseIf (u2 Is LeftUnit Or u1 Is RightUnit) _
            And Not LeftUnit Is RightUnit _
        Then
            Set lu = u2
            Set ru = u1
        Else
            Set lu = u1
            Set ru = u2
        End If
        
        '���ݕ\������Ă��鏇�Ԃɉ����ă��j�b�g�̓���ւ�
        If lu Is RightUnit And ru Is LeftUnit _
            And Not LeftUnit Is RightUnit _
        Then
            Set lu = LeftUnit
            Set ru = RightUnit
        End If
        
        '�\�����郆�j�b�g�̂f�t�h���i��\��
        If Not lu Is Nothing Then
            If Not .labHP1.Visible Then
                .labHP1.Visible = True
                .labEN1.Visible = True
                .picHP1.Visible = True
                .picEN1.Visible = True
                .txtHP1.Visible = True
                .txtEN1.Visible = True
                .picUnit1.Visible = True
            End If
        End If
        If Not ru Is Nothing Then
            If Not .labHP2.Visible Then
                .labHP2.Visible = True
                .labEN2.Visible = True
                .picHP2.Visible = True
                .picEN2.Visible = True
                .txtHP2.Visible = True
                .txtEN2.Visible = True
                .picUnit2.Visible = True
            End If
        End If
        
        '���\���̃��j�b�g��\������
        If Not lu Is Nothing And Not lu Is LeftUnit Then
            '���̃��j�b�g�����\���Ȃ̂ŕ\������
            
            '���j�b�g�摜
            If lu.BitmapID > 0 Then
                If MapDrawMode = "" Then
                    ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, _
                        MainForm.picUnitBitmap.hDC, _
                        32 * (lu.BitmapID Mod 15), 96 * (lu.BitmapID \ 15), _
                        SRCCOPY)
                Else
                    LoadUnitBitmap lu, .picUnit1, 0, 0, True
                End If
            Else
                '��\���̃��j�b�g�̏ꍇ�̓��j�b�g�̂���n�`�^�C����\��
                ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, _
                    MainForm.picBack.hDC, 32 * (lu.X - 1), 32 * (lu.Y - 1), SRCCOPY)
            End If
            .picUnit1.Refresh
            
            '�g�o����
            If lu.IsConditionSatisfied("�f�[�^�s��") Then
                .labHP1.Caption = Term("HP")
            Else
                .labHP1.Caption = Term("HP", lu)
            End If
            
            '�g�o���l
            If lu.IsConditionSatisfied("�f�[�^�s��") Then
                .txtHP1.Text = "?????/?????"
            Else
                If lu.HP < 100000 Then
                    buf = LeftPaddedString(Format$(lu.HP), MinLng(Len(Format$(lu.MaxHP)), 5))
                Else
                    buf = "?????"
                End If
                If lu.MaxHP < 100000 Then
                    buf = buf & "/" & Format$(lu.MaxHP)
                Else
                    buf = buf & "/?????"
                End If
                .txtHP1.Text = buf
            End If
            
            '�g�o�Q�[�W
            .picHP1.Cls
            If lu.HP > 0 Or i < num Then
                .picHP1.Line (0, 0)-((.picHP1.width - 4) * lu.HP \ lu.MaxHP - 1, 4), , BF
            End If
            
            '�d�m����
            If lu.IsConditionSatisfied("�f�[�^�s��") Then
                .labEN1.Caption = Term("EN")
            Else
                .labEN1.Caption = Term("EN", lu)
            End If
            
            '�d�m���l
            If lu.IsConditionSatisfied("�f�[�^�s��") Then
                .txtEN1.Text = "???/???"
            Else
                If lu.EN < 1000 Then
                    buf = LeftPaddedString(Format$(lu.EN), MinLng(Len(Format$(lu.MaxEN)), 3))
                Else
                    buf = "???"
                End If
                If lu.MaxEN < 1000 Then
                    buf = buf & "/" & Format$(lu.MaxEN)
                Else
                    buf = buf & "/???"
                End If
                .txtEN1.Text = buf
            End If
            
            '�d�m�Q�[�W
            .picEN1.Cls
            If lu.EN > 0 Or i < num Then
                .picEN1.Line (0, 0)-((.picEN1.width - 4) * lu.EN \ lu.MaxEN - 1, 4), , BF
            End If
            
            '�\�����e���L�^
            Set LeftUnit = lu
            LeftUnitHPRatio = lu.HP / lu.MaxHP
            LeftUnitENRatio = lu.EN / lu.MaxEN
        End If
        
        If Not ru Is Nothing And Not RightUnit Is ru Then
            '�E�̃��j�b�g�����\���Ȃ̂ŕ\������
            
            '���j�b�g�摜
            If ru.BitmapID > 0 Then
                If MapDrawMode = "" Then
                    ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, _
                        MainForm.picUnitBitmap.hDC, _
                        32 * (ru.BitmapID Mod 15), 96 * (ru.BitmapID \ 15), _
                        SRCCOPY)
                Else
                    LoadUnitBitmap ru, .picUnit2, 0, 0, True
                End If
            Else
                '��\���̃��j�b�g�̏ꍇ�̓��j�b�g�̂���n�`�^�C����\��
                ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, _
                    MainForm.picBack.hDC, 32 * (ru.X - 1), 32 * (ru.Y - 1), SRCCOPY)
            End If
            .picUnit2.Refresh
            
            '�g�o���l
            If ru.IsConditionSatisfied("�f�[�^�s��") Then
                .labHP2.Caption = Term("HP")
            Else
                .labHP2.Caption = Term("HP", ru)
            End If
            
            '�g�o���l
            If ru.IsConditionSatisfied("�f�[�^�s��") Then
                .txtHP2.Text = "?????/?????"
            Else
                If ru.HP < 100000 Then
                    buf = LeftPaddedString(Format$(ru.HP), MinLng(Len(Format$(ru.MaxHP)), 5))
                Else
                    buf = "?????"
                End If
                If ru.MaxHP < 100000 Then
                    buf = buf & "/" & Format$(ru.MaxHP)
                Else
                    buf = buf & "/?????"
                End If
                .txtHP2.Text = buf
            End If
            
            '�g�o�Q�[�W
            .picHP2.Cls
            If ru.HP > 0 Or i < num Then
                .picHP2.Line (0, 0)-((.picHP2.width - 4) * ru.HP \ ru.MaxHP - 1, 4), , BF
            End If
            
            '�d�m����
            If ru.IsConditionSatisfied("�f�[�^�s��") Then
                .labEN2.Caption = Term("EN")
            Else
                .labEN2.Caption = Term("EN", ru)
            End If
            
            '�d�m���l
            If ru.IsConditionSatisfied("�f�[�^�s��") Then
                .txtEN2.Text = "???/???"
            Else
                If ru.EN < 1000 Then
                    buf = LeftPaddedString(Format$(ru.EN), MinLng(Len(Format$(ru.MaxEN)), 3))
                Else
                    buf = "???"
                End If
                If ru.MaxEN < 1000 Then
                    buf = buf & "/" & Format$(ru.MaxEN)
                Else
                    buf = buf & "/???"
                End If
                .txtEN2.Text = buf
            End If
            
            '�d�m�Q�[�W
            .picEN2.Cls
            If ru.EN > 0 Or i < num Then
                .picEN2.Line (0, 0)-((.picEN2.width - 4) * ru.EN \ ru.MaxEN - 1, 4), , BF
            End If
            
            '�\�����e���L�^
            Set RightUnit = ru
            RightUnitHPRatio = ru.HP / ru.MaxHP
            RightUnitENRatio = ru.EN / ru.MaxEN
        End If
        
        '�O��̕\������̂g�o�A�d�m�̕ω����A�j���\��
        
        '�ω����Ȃ��ꍇ�̓A�j���\���̕K�v���Ȃ��̂Ń`�F�b�N���Ă���
        num = 0
        If Not lu Is Nothing Then
            If lu.HP / lu.MaxHP <> LeftUnitHPRatio _
                Or lu.EN / lu.MaxEN <> LeftUnitENRatio _
            Then
                num = 8
            End If
        End If
        If Not ru Is Nothing Then
            If ru.HP <> RightUnitHPRatio Or ru.EN <> RightUnitENRatio Then
                num = 8
            End If
        End If
        
        '�E�{�^����������Ă���ꍇ�̓A�j���[�V�����\����Z�k��
        If num > 0 Then
            If IsRButtonPressed() Then
                num = 2
            End If
        End If
        
        For i = 1 To num
            '�����̃��j�b�g
            If Not lu Is Nothing Then
                '�g�o
                If lu.HP / lu.MaxHP <> LeftUnitHPRatio Then
                    tmp = (lu.MaxHP * LeftUnitHPRatio * (num - i) + lu.HP * i) \ num
                    
                    If lu.IsConditionSatisfied("�f�[�^�s��") Then
                        .txtHP1.Text = "?????/?????"
                    Else
                        If lu.HP < 100000 Then
                            buf = LeftPaddedString(Format$(tmp), _
                                MinLng(Len(Format$(lu.MaxHP)), 5))
                        Else
                            buf = "?????"
                        End If
                        If lu.MaxHP < 100000 Then
                            buf = buf & "/" & Format$(lu.MaxHP)
                        Else
                            buf = buf & "/?????"
                        End If
                        .txtHP1.Text = buf
                    End If
                        
                    .picHP1.Cls
                    If lu.HP > 0 Or i < num Then
                        .picHP1.Line (0, 0)-((.picHP1.width - 4) * tmp \ lu.MaxHP - 1, 4), , BF
                    End If
                End If
                
                '�d�m
                If lu.EN / lu.MaxEN <> LeftUnitENRatio Then
                    tmp = (lu.MaxEN * LeftUnitENRatio * (num - i) + lu.EN * i) \ num
                    
                    If lu.IsConditionSatisfied("�f�[�^�s��") Then
                        .txtEN1.Text = "???/???"
                    Else
                        If lu.EN < 1000 Then
                            buf = LeftPaddedString(Format$(tmp), _
                                MinLng(Len(Format$(lu.MaxEN)), 3))
                        Else
                            buf = "???"
                        End If
                        If lu.MaxEN < 1000 Then
                            buf = buf & "/" & Format$(lu.MaxEN)
                        Else
                            buf = buf & "/???"
                        End If
                        .txtEN1.Text = buf
                    End If
                        
                    .picEN1.Cls
                    If lu.EN > 0 Or i < num Then
                        .picEN1.Line (-1, 0)-((.picEN1.width - 4) * tmp \ lu.MaxEN - 1, 4), , BF
                    End If
                End If
            End If
            
            '�E���̃��j�b�g
            If Not ru Is Nothing Then
                '�g�o
                If ru.HP / ru.MaxHP <> RightUnitHPRatio Then
                    tmp = (ru.MaxHP * RightUnitHPRatio * (num - i) + ru.HP * i) \ num
                    
                    If ru.IsConditionSatisfied("�f�[�^�s��") Then
                        .txtHP2.Text = "?????/?????"
                    Else
                        If ru.HP < 100000 Then
                            buf = LeftPaddedString(Format$(tmp), _
                                MinLng(Len(Format$(ru.MaxHP)), 5))
                        Else
                            buf = "?????"
                        End If
                        If ru.MaxHP < 100000 Then
                            buf = buf & "/" & Format$(ru.MaxHP)
                        Else
                            buf = buf & "/?????"
                        End If
                        .txtHP2.Text = buf
                    End If
                    
                    .picHP2.Cls
                    If ru.HP > 0 Or i < num Then
                        .picHP2.Line (0, 0)-((.picHP2.width - 4) * tmp \ ru.MaxHP - 1, 4), , BF
                    End If
                End If
                
                '�d�m
                If ru.EN / ru.MaxEN <> RightUnitENRatio Then
                    tmp = (ru.MaxEN * RightUnitENRatio * (num - i) + ru.EN * i) \ num
                    If ru.IsConditionSatisfied("�f�[�^�s��") Then
                        .txtEN2.Text = "???/???"
                    Else
                        If ru.EN < 1000 Then
                            buf = LeftPaddedString(Format$(tmp), _
                                MinLng(Len(Format$(ru.MaxEN)), 3))
                        Else
                            buf = "???"
                        End If
                        If ru.MaxEN < 1000 Then
                            buf = buf & "/" & Format$(ru.MaxEN)
                        Else
                            buf = buf & "/???"
                        End If
                        .txtEN2.Text = buf
                    End If
                    
                    .picEN2.Cls
                    If ru.EN > 0 Or i < num Then
                        .picEN2.Line (0, 0)-((.picEN2.width - 4) * tmp \ ru.MaxEN - 1, 4), , BF
                    End If
                End If
            End If
            
            '���t���b�V��
            If Not lu Is Nothing Then
                If lu.HP / lu.MaxHP <> LeftUnitHPRatio Then
                    .picHP1.Refresh
                    .txtHP1.Refresh
                End If
                If lu.EN / lu.MaxEN <> LeftUnitENRatio Then
                    .picEN1.Refresh
                    .txtEN1.Refresh
                End If
            End If
            If Not ru Is Nothing Then
                If ru.HP / ru.MaxHP <> RightUnitHPRatio Then
                    .picHP2.Refresh
                    .txtHP2.Refresh
                End If
                If ru.EN / ru.MaxEN <> RightUnitENRatio Then
                    .picEN2.Refresh
                    .txtEN2.Refresh
                End If
            End If
            
            Sleep 20
        Next
        
        '�\�����e���L�^
        If Not lu Is Nothing Then
            LeftUnitHPRatio = lu.HP / lu.MaxHP
            LeftUnitENRatio = lu.EN / lu.MaxEN
        End If
        If Not ru Is Nothing Then
            RightUnitHPRatio = ru.HP / ru.MaxHP
            RightUnitENRatio = ru.EN / ru.MaxEN
        End If
        
        DoEvents
    End With
End Sub

'���b�Z�[�W�E�B���h�E�̏�Ԃ��L�^����
Public Sub SaveMessageFormStatus()
    IsMessageFormVisible = frmMessage.Visible
    Set SavedLeftUnit = LeftUnit
    Set SavedRightUnit = RightUnit
End Sub

'���b�Z�[�W�E�B���h�E�̏�Ԃ��L�^������Ԃɕۂ�
Public Sub KeepMessageFormStatus()
    If Not IsMessageFormVisible Then
        '�L�^�������_�Ń��b�Z�[�W�E�B���h�E���\������Ă��Ȃ����
        If frmMessage.Visible Then
            '�J���Ă��郁�b�Z�[�W�E�B���h�E�������I�ɕ���
            CloseMessageForm
        End If
    ElseIf Not frmMessage.Visible Then
        '�L�^�������_�ł̓��b�Z�[�W�E�B���h�E���\������Ă����̂ŁA
        '���b�Z�[�W�E�B���h�E���\������Ă��Ȃ��ꍇ�͕\������
        OpenMessageForm SavedLeftUnit, SavedRightUnit
    ElseIf LeftUnit Is Nothing And RightUnit Is Nothing _
        And (Not SavedLeftUnit Is Nothing Or Not SavedRightUnit Is Nothing) _
    Then
        '���b�Z�[�W�E�B���h�E���烆�j�b�g�\���������Ă��܂����ꍇ�͍ĕ\��
        OpenMessageForm SavedLeftUnit, SavedRightUnit
    End If
End Sub


' === ���b�Z�[�W�\���Ɋւ��鏈�� ===

'���b�Z�[�W�E�B���h�E�Ƀ��b�Z�[�W��\��
Public Sub DisplayMessage(pname As String, ByVal msg As String, _
    Optional ByVal msg_mode As String)
Dim messages() As String
Dim msg_head As Integer, line_head As Integer
Dim i As Integer, j As Integer
Dim buf As String, ch As String
Dim p As PictureBox
Dim pnickname As String, fname As String
Dim start_time As Long, wait_time As Long
Dim lnum As Integer, prev_lnum As Integer
Dim PT As POINTAPI
Dim is_automode As Boolean
Dim lstate As Integer, rstate As Integer
Dim in_tag As Boolean
Dim is_character_message As Boolean
Dim cl_margin(2) As Single
Dim left_margin As String
    
    '�L�����\���̕`������
    If pname = "�V�X�e��" Then
        '�u�V�X�e���v
        frmMessage.picFace = LoadPicture("")
        frmMessage.picFace.Refresh
        DisplayedPilot = ""
        left_margin = ""
    ElseIf pname <> "" Then
        '�ǂ̃L�����摜���g�����H
        If PList.IsDefined(pname) Then
            pnickname = PList.Item(pname).Nickname
            fname = PList.Item(pname).Bitmap
        ElseIf PDList.IsDefined(pname) Then
            pnickname = PDList.Item(pname).Nickname
            fname = PDList.Item(pname).Bitmap
        ElseIf NPDList.IsDefined(pname) Then
            pnickname = NPDList.Item(pname).Nickname
            fname = NPDList.Item(pname).Bitmap
        Else
            fname = "-.bmp"
        End If
        
        '�L�����摜�̕\��
        If fname <> "-.bmp" Then
            fname = "Pilot\" & fname
            If DisplayedPilot <> fname Or DisplayMode <> msg_mode Then
                If DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "���b�Z�[�W " & msg_mode) Then
                    frmMessage.picFace.Refresh
                    DisplayedPilot = fname
                    DisplayMode = msg_mode
                Else
                    frmMessage.picFace = LoadPicture("")
                    frmMessage.picFace.Refresh
                    DisplayedPilot = ""
                    DisplayMode = ""
                    
                    '�p�C���b�g�摜�����݂��Ȃ����Ƃ��L�^���Ă���
                    If PList.IsDefined(pname) Then
                        With PList.Item(pname)
                            If .Bitmap = .Data.Bitmap Then
                                .Data.IsBitmapMissing = True
                            End If
                        End With
                    ElseIf PDList.IsDefined(pname) Then
                        PDList.Item(pname).IsBitmapMissing = True
                    ElseIf NPDList.IsDefined(pname) Then
                        NPDList.Item(pname).IsBitmapMissing = True
                    End If
                End If
            End If
        Else
            frmMessage.picFace = LoadPicture("")
            frmMessage.picFace.Refresh
            DisplayedPilot = ""
            DisplayMode = ""
        End If
        
        If IsOptionDefined("��b�p�C���b�g�����s") Then
            left_margin = " "
        Else
            left_margin = "  "
        End If
    End If
    
    '���b�Z�[�W���̎��u��������
    FormatMessage msg
    msg = Trim$(msg)
    
    '�����ɋ������s�������Ă���ꍇ�͎�菜��
    Do While Right$(msg, 1) = ";"
        msg = Left$(msg, Len(msg) - 1)
    Loop
    
    '���b�Z�[�W����̏ꍇ�̓L�����\���̕`�������̂ݍs��
    If msg = "" Then
        Exit Sub
    End If
    
    Select Case pname
        Case "�V�X�e��"
            '���̂܂܎g�p
        Case ""
            '��{�I�ɂ͂��̂܂܎g�p���邪�A����ӕ\���̑�p�̏ꍇ��
            '����ӕ\���p�̏������s��
            i = 0
            If (InStr(msg, "�u") > 0 And Right$(msg, 1) = "�v") Then
                i = InStr(msg, "�u")
            ElseIf (InStr(msg, "�w") > 0 And Right$(msg, 1) = "�x") Then
                i = InStr(msg, "�w")
            ElseIf (InStr(msg, "(") > 0 And Right$(msg, 1) = ")") Then
                i = InStr(msg, "(")
            ElseIf (InStr(msg, "�i") > 0 And Right$(msg, 1) = "�j") Then
                i = InStr(msg, "�i")
            End If
            If i > 1 Then
                If i < 8 _
                    Or PDList.IsDefined(Trim$(Left$(msg, i - 1))) _
                    Or NPDList.IsDefined(Trim$(Left$(msg, i - 1))) _
                Then
                    is_character_message = True
                    If Not IsSpace(Mid$(msg, i - 1, 1)) Then
                        '"�u"�̑O�ɔ��p�X�y�[�X��}��
                        msg = Left$(msg, i - 1) & " " & Mid$(msg, i)
                    End If
                End If
            End If
        Case Else
            is_character_message = True
            If (Left$(msg, 1) = "(" Or Left$(msg, 1) = "�i") _
                And (Right$(msg, 1) = ")" Or Right$(msg, 1) = "�j") _
            Then
                '���m���[�O
                msg = Mid$(msg, 2, Len(msg) - 2)
                msg = pnickname _
                    & IIf(IsOptionDefined("��b�p�C���b�g�����s"), ";", " ") _
                    & "�i" & msg & "�j"
            ElseIf Left$(msg, 1) = "�w" And Right$(msg, 1) = "�x" Then
                msg = Mid$(msg, 2, Len(msg) - 2)
                msg = pnickname _
                    & IIf(IsOptionDefined("��b�p�C���b�g�����s"), ";", " ") _
                    & "�w" & msg & "�x"
            Else
                '�����
                msg = pnickname _
                    & IIf(IsOptionDefined("��b�p�C���b�g�����s"), ";", " ") _
                    & "�u" & msg & "�v"
            End If
    End Select

    '�������s�̈ʒu��ݒ�
    If IsOptionDefined("���s���]���Z�k") Then
        cl_margin(0) = 0.94 '���b�Z�[�W���̒��߂ɂ����s�̈ʒu
        cl_margin(1) = 0.7  '"�B"," "�ɂ����s�̈ʒu
        cl_margin(2) = 0.85  '"�A"�ɂ����s�̈ʒu
    Else
        cl_margin(0) = 0.8
        cl_margin(1) = 0.6
        cl_margin(2) = 0.75
    End If
    
    '���b�Z�[�W�𕪊�
    ReDim messages(1)
    msg_head = 1
    buf = ""
    For i = 1 To Len(msg)
        If Mid$(msg, i, 1) = ":" Then
            buf = buf & Mid$(msg, msg_head, i - msg_head)
            messages(UBound(messages)) = buf
            ReDim Preserve messages(UBound(messages) + 1)
            msg_head = i + 1
        End If
    Next
    messages(UBound(messages)) = buf & Mid$(msg, msg_head)
    
    '���b�Z�[�W������̂��߁A���̃��b�Z�[�W���č\�z
    msg = ""
    For i = 1 To UBound(messages)
        msg = msg & messages(i)
    Next
        
    '���b�Z�[�W�̕\��
    Set p = frmMessage.picMessage
    msg_head = 1
    prev_lnum = 0
    i = 0
    Do While i < UBound(messages)
        i = i + 1
        buf = messages(i)
        
        lnum = 0
        line_head = msg_head
        in_tag = False
        
        p.Cls
        p.CurrentX = 1
        
        If msg_head = 1 Then
            '�t�H���g�ݒ��������
            With p
                .FontBold = False
                .FontItalic = False
                .FontName = "�l�r �o����"
                .FontSize = 12
                .ForeColor = vbBlack
            End With
        Else
            '���b�Z�[�W�̓r������\��
            If is_character_message Then
                p.Print "  ";
            End If
        End If
        
        For j = msg_head To Len(buf)
            ch = Mid$(buf, j, 1)
            
            '";"�ł͕K�����s
            If ch = ";" Then
                If j <> line_head Then
                    PrintMessage Mid$(buf, line_head, j - line_head)
                    lnum = lnum + 1
                    If is_character_message _
                        And ((lnum > 1 And IsOptionDefined("��b�p�C���b�g�����s")) _
                            Or (lnum > 0 And Not IsOptionDefined("��b�p�C���b�g�����s"))) _
                    Then
                        p.Print left_margin;
                    End If
                End If
                line_head = j + 1
                GoTo NextLoop
            End If
            
            '�^�O���ł͉��s���Ȃ�
            If ch = "<" Then
                in_tag = True
                GoTo NextLoop
            ElseIf ch = ">" Then
                in_tag = False
            ElseIf in_tag Then
                GoTo NextLoop
            End If
            
            '���b�Z�[�W���r�؂�Ă��܂��ꍇ�͕K�����s
            If MessageLen(Mid$(buf, line_head, j - line_head)) > 0.95 * p.width Then
                PrintMessage Mid$(buf, line_head, j - line_head + 1)
                lnum = lnum + 1
                If is_character_message _
                    And ((lnum > 1 And IsOptionDefined("��b�p�C���b�g�����s")) _
                        Or (lnum > 0 And Not IsOptionDefined("��b�p�C���b�g�����s"))) _
                Then
                    p.Print left_margin;
                End If
                line_head = j + 1
                GoTo NextLoop
            End If
            
            '�֑�����
            Select Case Mid$(buf, j + 1, 1)
                Case "�B", "�A", "�c", "�d", "�E", "�", _
                    "�`", "�[", "�|", "�I", "�H", _
                    "�v", "�x", "�j", ")", " ", ";"
                    GoTo NextLoop
            End Select
            Select Case Mid$(buf, j + 2, 1)
                Case "�B", "�A", "�c", "�d", "�E", "�", _
                    "�`", "�[", "�|", "�I", "�H", _
                    "�v", "�x", "�j", ")", " ", ";"
                    GoTo NextLoop
            End Select
            If Mid$(buf, j + 3, 1) = ";" Then
                GoTo NextLoop
            End If
            
            '���s�̔���
            If MessageLen(Mid$(messages(i), line_head)) < 0.95 * p.width Then
                '�S�̂���s�Ɏ��܂�ꍇ
                GoTo NextLoop
            End If
            Select Case ch
                Case "�B"
                    If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(1) * p.width Then
                        PrintMessage Mid$(buf, line_head, j - line_head + 1)
                        lnum = lnum + 1
                        If is_character_message _
                            And ((lnum > 1 And IsOptionDefined("��b�p�C���b�g�����s")) _
                                Or (lnum > 0 And Not IsOptionDefined("��b�p�C���b�g�����s"))) _
                        Then
                            p.Print left_margin;
                        End If
                        line_head = j + 1
                    End If
                Case "�A"
                    If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(2) * p.width Then
                        PrintMessage Mid$(buf, line_head, j - line_head + 1)
                        lnum = lnum + 1
                        If is_character_message _
                            And ((lnum > 1 And IsOptionDefined("��b�p�C���b�g�����s")) _
                                Or (lnum > 0 And Not IsOptionDefined("��b�p�C���b�g�����s"))) _
                        Then
                            p.Print left_margin;
                        End If
                        line_head = j + 1
                    End If
                Case " "
                    ch = Mid$(buf, j - 1, 1)
                    '�X�y�[�X�����̋�؂�Ɏg���Ă��邩�ǂ�������
                    If pname <> "�V�X�e��" _
                        And (ch = "�I" Or ch = "�H" _
                            Or ch = "�c" Or ch = "�d" _
                            Or ch = "�E" Or ch = "�" _
                            Or ch = "�`") _
                    Then
                        '���̋�؂�
                        If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(1) * p.width Then
                            PrintMessage Mid$(buf, line_head, j - line_head + 1)
                            lnum = lnum + 1
                            If is_character_message _
                                And ((lnum > 1 And IsOptionDefined("��b�p�C���b�g�����s")) _
                                    Or (lnum > 0 And Not IsOptionDefined("��b�p�C���b�g�����s"))) _
                            Then
                                p.Print left_margin;
                            End If
                            line_head = j + 1
                        End If
                    Else
                        '�P�Ȃ��
                        If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(0) * p.width Then
                            PrintMessage Mid$(buf, line_head, j - line_head + 1)
                            lnum = lnum + 1
                            If is_character_message _
                                And ((lnum > 1 And IsOptionDefined("��b�p�C���b�g�����s")) _
                                    Or (lnum > 0 And Not IsOptionDefined("��b�p�C���b�g�����s"))) _
                            Then
                                p.Print left_margin;
                            End If
                            line_head = j + 1
                        End If
                    End If
                Case Else

                    If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(0) * p.width Then
                        PrintMessage Mid$(buf, line_head, j - line_head + 1)
                        lnum = lnum + 1
                        If is_character_message _
                            And ((lnum > 1 And IsOptionDefined("��b�p�C���b�g�����s")) _
                                Or (lnum > 0 And Not IsOptionDefined("��b�p�C���b�g�����s"))) _
                        Then
                            p.Print left_margin;
                        End If
                        line_head = j + 1
                    End If
            End Select
NextLoop:
            If lnum = 4 Then
                If j < Len(buf) Then
                    msg_head = line_head
                    i = i - 1
                    Exit For
                End If
            End If
        Next
        '�c��̕�����\��
        If lnum < 4 Then
            If Len(buf) >= line_head Then
                PrintMessage Mid$(buf, line_head)
            End If
        End If
        
        DoEvents
        
        If MessageWait > 10000 Then
            AutoMessageMode = False
        End If
        
        '�E�B���h�E�̃L���v�V������ݒ�
        If AutoMessageMode Then
            If frmMessage.Caption = "���b�Z�[�W" Then
                frmMessage.Caption = "���b�Z�[�W (��������)"
            End If
        Else
            If frmMessage.Caption = "���b�Z�[�W (��������)" Then
                frmMessage.Caption = "���b�Z�[�W"
            End If
        End If
        
        '���̃��b�Z�[�W�\���܂ł̎��Ԃ�ݒ�(�������b�Z�[�W����p)
        start_time = timeGetTime()
        wait_time = (lnum - prev_lnum + 2) * (MessageWait + 250)
        
        '���̃��b�Z�[�W�҂�
        IsFormClicked = False
        is_automode = AutoMessageMode
        Do Until IsFormClicked
            If AutoMessageMode Then
                If start_time + wait_time < timeGetTime() Then
                    Exit Do
                End If
            End If
            
            GetCursorPos PT
            
            '���b�Z�[�W�E�C���h�E��Ń}�E�X�{�^�����������ꍇ
            If Screen.ActiveForm Is frmMessage Then
                With frmMessage
                    If .Left \ Screen.TwipsPerPixelX <= PT.X _
                        And PT.X <= (.Left + .width) \ Screen.TwipsPerPixelX _
                        And .Top \ Screen.TwipsPerPixelY <= PT.Y _
                        And PT.Y <= (.Top + .Height) \ Screen.TwipsPerPixelY _
                    Then
                        lstate = GetAsyncKeyState(LButtonID)
                        rstate = GetAsyncKeyState(RButtonID)
                        If (lstate And &H8000) <> 0 Then
                            If start_time + wait_time < timeGetTime Then
                                '���{�^���Ń��b�Z�[�W�̎�������
                                Exit Do
                            End If
                        ElseIf (rstate And &H8000) <> 0 Then
                            '�E�{�^���Ń��b�Z�[�W�̑�����
                            Exit Do
                        End If
                    End If
                End With
            End If
            
            '���C���E�C���h�E��Ń}�E�X�{�^�����������ꍇ
            If Screen.ActiveForm Is MainForm Then
                With MainForm
                    If .Left \ Screen.TwipsPerPixelX <= PT.X _
                        And PT.X <= (.Left + .width) \ Screen.TwipsPerPixelX _
                        And .Top \ Screen.TwipsPerPixelY <= PT.Y _
                        And PT.Y <= (.Top + .Height) \ Screen.TwipsPerPixelY _
                    Then
                        lstate = GetAsyncKeyState(LButtonID)
                        rstate = GetAsyncKeyState(RButtonID)
                        If (lstate And &H8000) <> 0 Then
                            If start_time + wait_time < timeGetTime Then
                                '���{�^���Ń��b�Z�[�W�̎�������
                                Exit Do
                            End If
                        ElseIf (rstate And &H8000) <> 0 Then
                            '�E�{�^���Ń��b�Z�[�W�̑�����
                            Exit Do
                        End If
                    End If
                End With
            End If
            
            Sleep 100
            DoEvents
            
            '�������胂�[�h���؂�ւ���ꂽ�ꍇ
            If is_automode <> AutoMessageMode Then
                IsFormClicked = False
                is_automode = AutoMessageMode
                If AutoMessageMode Then
                    frmMessage.Caption = "���b�Z�[�W (��������)"
                    start_time = timeGetTime()
                    wait_time = (lnum - prev_lnum + 2) * (MessageWait + 250)
                Else
                    frmMessage.Caption = "���b�Z�[�W"
                End If
            End If
        Loop
        
        '�E�F�C�g�v�Z�p�Ɋ��ɕ\�������s�����L�^
        If lnum < 4 Then
            prev_lnum = lnum
        Else
            prev_lnum = 0
        End If
    Loop
    
    '�t�H���g�ݒ�����ɖ߂�
    With p
        .FontBold = False
        .FontItalic = False
        .FontName = "�l�r �o����"
        .FontSize = 12
        .ForeColor = vbBlack
    End With
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "�p�C���b�g�p�摜�t�@�C��" & vbCr & vbLf _
        & DisplayedPilot & vbCr & vbLf _
        & "�̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCr & vbLf _
        & "�摜�t�@�C�������Ă��Ȃ����m�F���ĉ������B"
End Sub

'���b�Z�[�W�E�B���h�E�ɕ��������������
Public Sub PrintMessage(msg As String, Optional ByVal is_sys_msg As Boolean)
Dim tag As String, buf As String, ch As String
Dim p As PictureBox
Dim last_x As Integer, last_y As Integer, max_y As Integer
Dim i As Integer, head As Integer
Dim in_tag As Boolean, escape_depth As Integer

    Set p = frmMessage.picMessage
    
    head = 1
    For i = 1 To Len(msg)
        ch = Mid$(msg, i, 1)
        
        '�V�X�e�����b�Z�[�W�̎��̂݃G�X�P�[�v�V�[�P���X�̏������s��
        If is_sys_msg Then
            Select Case ch
                Case "["
                    escape_depth = escape_depth + 1
                    If escape_depth = 1 Then
                        '�G�X�P�[�v�V�[�P���X�J�n
                        '����܂ł̕�������o��
                        p.Print Mid$(msg, head, i - head);
                        head = i + 1
                        GoTo NextChar
                    End If
                Case "]"
                    escape_depth = escape_depth - 1
                    If escape_depth = 0 Then
                        '�G�X�P�[�v�V�[�P���X�I��
                        '�G�X�P�[�v�V�[�P���X���o��
                        p.Print Mid$(msg, head, i - head);
                        head = i + 1
                        GoTo NextChar
                    End If
            End Select
        End If
        
        '�^�O�̏���
        Select Case ch
            Case "<"
                If Not in_tag And escape_depth = 0 Then
                    '�^�O�J�n
                    in_tag = True
                    '����܂ł̕�������o��
                    p.Print Mid$(msg, head, i - head);
                    head = i + 1
                    GoTo NextChar
                End If
            Case ">"
                If in_tag Then
                    '�^�O�I��
                    in_tag = False
                    
                    '�^�O�̐؂�o��
                    tag = LCase$(Mid$(msg, head, i - head))
                    
                    '�^�O�ɍ��킹�Ċe�폈�����s��
                    Select Case tag
                        Case "b"
                            p.FontBold = True
                        Case "/b"
                            p.FontBold = False
                        Case "i"
                            p.FontItalic = True
                        Case "/i"
                            p.FontItalic = False
                        Case "big"
                            p.FontSize = p.FontSize + 2
                            last_x = p.CurrentX
                            last_y = p.CurrentY
                            p.Print
                            If p.CurrentY > max_y Then
                                max_y = p.CurrentY
                            End If
                            p.CurrentX = last_x
                            p.CurrentY = last_y
                        Case "/big"
                            p.FontSize = p.FontSize - 2
                        Case "small"
                            p.FontSize = p.FontSize - 2
                            last_x = p.CurrentX
                            last_y = p.CurrentY
                            p.Print
                            If p.CurrentY > max_y Then
                                max_y = p.CurrentY
                            End If
                            p.CurrentX = last_x
                            p.CurrentY = last_y
                        Case "/small"
                            p.FontSize = p.FontSize + 2
                        Case "/color"
                            p.ForeColor = vbBlack
                        Case "/size"
                            p.FontSize = 12
                        Case "lt"
                            p.Print "<";
                        Case "gt"
                            p.Print ">";
                        Case Else
                            If InStr(tag, "color=") = 1 Then
                                '�F�ݒ�
                                Dim cname As String
                                cname = GetValueAsString(Mid$(tag, 7))
                                Select Case cname
                                    Case "black"
                                        p.ForeColor = vbBlack
                                    Case "gray"
                                        p.ForeColor = rgb(&H80, &H80, &H80)
                                    Case "silver"
                                        p.ForeColor = rgb(&HC0, &HC0, &HC0)
                                    Case "white"
                                        p.ForeColor = vbWhite
                                    Case "red"
                                        p.ForeColor = vbRed
                                    Case "yellow"
                                        p.ForeColor = vbYellow
                                    Case "lime"
                                        p.ForeColor = rgb(&H0, &HFF, &H0)
                                    Case "aqua"
                                        p.ForeColor = rgb(&H0, &HFF, &HFF)
                                    Case "blue"
                                        p.ForeColor = rgb(&H0, &H0, &HFF)
                                    Case "fuchsia"
                                        p.ForeColor = rgb(&HFF, &H0, &HFF)
                                    Case "maroon"
                                        p.ForeColor = rgb(&H80, &H0, &H0)
                                    Case "olive"
                                        p.ForeColor = rgb(&H80, &H80, &H0)
                                    Case "green"
                                        p.ForeColor = rgb(&H0, &H80, &H0)
                                    Case "teal"
                                        p.ForeColor = rgb(&H0, &H80, &H80)
                                    Case "navy"
                                        p.ForeColor = rgb(&H0, &H0, &H80)
                                    Case "purple"
                                        p.ForeColor = rgb(&H80, &H0, &H80)
                                    Case Else
                                        If Asc(cname) = 35 Then '#
                                            buf = String$(8, vbNullChar)
                                            Mid(buf, 1, 2) = "&H"
                                            Mid(buf, 3, 2) = Mid$(cname, 6, 2)
                                            Mid(buf, 5, 2) = Mid$(cname, 4, 2)
                                            Mid(buf, 7, 2) = Mid$(cname, 2, 2)
                                            If IsNumeric(buf) Then
                                                p.ForeColor = CLng(buf)
                                            End If
                                        End If
                                End Select
                            ElseIf InStr(tag, "size=") = 1 Then
                                '�T�C�Y�ݒ�
                                If IsNumeric(Mid$(tag, 6)) Then
                                    p.FontSize = CLng(Mid$(tag, 6))
                                    last_x = p.CurrentX
                                    last_y = p.CurrentY
                                    p.Print
                                    If p.CurrentY > max_y Then
                                        max_y = p.CurrentY
                                    End If
                                    p.CurrentX = last_x
                                    p.CurrentY = last_y
                                End If
                            Else
                                '�^�O�ł͂Ȃ��̂ł��̂܂܏����o��
                                p.Print Mid$(msg, head - 1, i - head + 2);
                            End If
                    End Select
                    
                    head = i + 1
                    GoTo NextChar
                End If
        End Select
NextChar:
    Next
    
    '�I�����Ă��Ȃ��^�O�A�������̓G�X�P�[�v�V�[�P���X�͂����̕�����ƌ��Ȃ�
    If in_tag Or escape_depth > 0 Then
        head = head - 1
    End If
    
    '���o�͂̕�������o�͂���
    If head <= Len(msg) Then
        If Right$(msg, 1) = "�v" Then
            '�Ō�̊��ʂ̈ʒu�͈�ԑ傫�ȃT�C�Y�̕����ɍ��킹��
            p.Print Mid$(msg, head, Len(msg) - head);
            
            last_x = p.CurrentX
            last_y = p.CurrentY
            p.Print
            p.CurrentX = last_x
            If p.CurrentY > max_y Then
                p.CurrentY = last_y
            Else
                p.CurrentY = last_y + max_y - p.CurrentY
            End If
            
            p.Print Right$(msg, 1)
        Else
            p.Print Mid$(msg, head)
        End If
    Else
        '���o�͂̕����񂪂Ȃ��ꍇ�͉��s�̂�
        p.Print
    End If
    
    '���s��̈ʒu�͈�ԑ傫�ȃT�C�Y�̕����ɍ��킹��
    If max_y > p.CurrentY Then
        p.CurrentY = max_y + 1
    Else
        p.CurrentY = p.CurrentY + 1
    End If
    p.CurrentX = 1
End Sub

'���b�Z�[�W�����v�Z(�^�O�𖳎�����)
Public Function MessageLen(ByVal msg As String) As Integer
Dim buf As String, ret As Integer
    
    '�^�O�����݂���H
    ret = InStr(msg, "<")
    If ret = 0 Then
        MessageLen = frmMessage.picMessage.TextWidth(msg)
        Exit Function
    End If
    
    '�^�O�����������b�Z�[�W���쐬
    Do While ret > 0
        buf = buf & Left$(msg, ret - 1)
        msg = Mid$(msg, ret + 1)
        
        ret = InStr(msg, ">")
        If ret > 0 Then
            msg = Mid$(msg, ret + 1)
        Else
            msg = ""
        End If
        
        ret = InStr(msg, "<")
    Loop
    buf = buf & msg
    
    '�^�O�������b�Z�[�W�̃s�N�Z�������v�Z
    MessageLen = frmMessage.picMessage.TextWidth(buf)
End Function

'���b�Z�[�W�E�B���h�E�ɐ퓬���b�Z�[�W��\��
Public Sub DisplayBattleMessage(pname As String, ByVal msg As String, _
    Optional msg_mode As String)
Dim messages() As String
Dim i As Integer, j As Integer
Dim line_head As Integer, lnum As Integer, prev_lnum As Integer
Dim p As PictureBox
Dim buf As String, buf2 As String, ch As String
Dim pnickname As String
Dim start_time As Long, wait_time As Long, cur_time As Long
Dim need_refresh As Boolean
Dim in_tag As Boolean
Dim dw As String, dh As String, dx As String, dy As String
Dim options As String, n As Integer, opt_n As Integer
Dim fname As String
Dim cname As String
Dim clear_every_time As Boolean
Dim is_char_message As Boolean
Static init_display_battle_message As Boolean
Static extdata_bitmap_dir_exists As Boolean
Static extdata2_bitmap_dir_exists As Boolean
Static last_path As String
Dim cl_margin(2) As Single

    '���߂Ď��s����ۂɁA�e�t�H���_��Bitmap�t�H���_�����邩�`�F�b�N
    If Not init_display_battle_message Then
        If Len(Dir$(ExtDataPath & "Bitmap", vbDirectory)) > 0 Then
            extdata_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath2 & "Bitmap", vbDirectory)) > 0 Then
            extdata2_bitmap_dir_exists = True
        End If
        init_display_battle_message = True
    End If
    
    '���b�Z�[�W�E�B���h�E�������Ă���Ε\�����Ȃ�
    If frmMessage.WindowState = 1 Then
        Exit Sub
    End If
    
    '�E�B���h�E�̃L���v�V������ݒ�
    If frmMessage.Caption = "���b�Z�[�W (��������)" Then
        frmMessage.Caption = "���b�Z�[�W"
    End If
    
    '�L�����\���̕`������
    If pname = "�V�X�e��" Then
        '�u�V�X�e���v
        frmMessage.picFace = LoadPicture("")
        frmMessage.picFace.Refresh
        DisplayedPilot = ""
    ElseIf pname <> "" And pname <> "-" Then
        '�ǂ̃L�����摜���g�����H
        If PList.IsDefined(pname) Then
            pnickname = PList.Item(pname).Nickname
            fname = PList.Item(pname).Bitmap
        ElseIf PDList.IsDefined(pname) Then
            pnickname = PDList.Item(pname).Nickname
            fname = PDList.Item(pname).Bitmap
        ElseIf NPDList.IsDefined(pname) Then
            pnickname = NPDList.Item(pname).Nickname
            fname = NPDList.Item(pname).Bitmap
        Else
            fname = "-.bmp"
        End If
        
        '�L�����摜�̕\��
        If fname <> "-.bmp" Then
            fname = "Pilot\" & fname
            If DisplayedPilot <> fname Or DisplayMode <> msg_mode Then
                If DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "���b�Z�[�W " & msg_mode) Then
                    frmMessage.picFace.Refresh
                    DisplayedPilot = fname
                    DisplayMode = msg_mode
                Else
                    frmMessage.picFace = LoadPicture("")
                    frmMessage.picFace.Refresh
                    DisplayedPilot = ""
                    DisplayMode = ""
                    
                    '�p�C���b�g�摜�����݂��Ȃ����Ƃ��L�^���Ă���
                    If PList.IsDefined(pname) Then
                        With PList.Item(pname)
                            If .Bitmap = .Data.Bitmap Then
                                .Data.IsBitmapMissing = True
                            End If
                        End With
                    ElseIf PDList.IsDefined(pname) Then
                        PDList.Item(pname).IsBitmapMissing = True
                    ElseIf NPDList.IsDefined(pname) Then
                        NPDList.Item(pname).IsBitmapMissing = True
                    End If
                End If
            End If
        Else
            frmMessage.picFace = LoadPicture("")
            frmMessage.picFace.Refresh
            DisplayedPilot = ""
            DisplayMode = ""
        End If
    End If
    
    '���b�Z�[�W����Ȃ�\���͎~�߂�
    If msg = "" Then
        Exit Sub
    End If
    
    Set p = frmMessage.picMessage
    
    '���b�Z�[�W�E�B���h�E�̏�Ԃ��L�^
    SaveMessageFormStatus
    
    '���b�Z�[�W�𕪊�
    ReDim messages(1)
    line_head = 1
    buf = ""
    For i = 1 To Len(msg)
        Select Case Mid$(msg, i, 1)
            Case ":"
                buf = buf & Mid$(msg, line_head, i - line_head)
                messages(UBound(messages)) = buf & ":"
                ReDim Preserve messages(UBound(messages) + 1)
                line_head = i + 1
            Case ";"
                buf = buf & Mid$(msg, line_head, i - line_head)
                messages(UBound(messages)) = buf
                buf = ""
                ReDim Preserve messages(UBound(messages) + 1)
                line_head = i + 1
        End Select
    Next
    messages(UBound(messages)) = buf & Mid$(msg, line_head)
    
    wait_time = DEFAULT_LEVEL
    
    '�������s�̈ʒu��ݒ�
    If IsOptionDefined("���s���]���Z�k") Then
        cl_margin(0) = 0.94 '���b�Z�[�W���̒��߂ɂ����s�̈ʒu
        cl_margin(1) = 0.7  '"�B"," "�ɂ����s�̈ʒu
        cl_margin(2) = 0.85  '"�A"�ɂ����s�̈ʒu
    Else
        cl_margin(0) = 0.8
        cl_margin(1) = 0.6
        cl_margin(2) = 0.75
    End If

    '�e���b�Z�[�W��\��
    For i = 1 To UBound(messages)
        buf = messages(i)
        
        '���b�Z�[�W���̎��u��������
        SaveBasePoint
        FormatMessage buf
        RestoreBasePoint
        
        '�������
        Select Case LCase$(Right$(LIndex(buf, 1), 4))
            Case ".bmp", ".jpg", ".gif", ".png"
                Dim fname0 As String, fsuffix As String, fpath As String
                Dim first_id As Integer, last_id As Integer
                Dim wait_time2 As Long
                
                '�E�{�^����������Ă�����X�L�b�v
                If IsRButtonPressed() Then
                    GoTo NextMessage
                End If
                
                '�J�b�g�C���̕\��
                fname = LIndex(buf, 1)
                
                '�A�j���w�肩�ǂ�������
                j = InStr(fname, "[")
                If j > 0 And InStr(fname, "].") = Len(fname) - 4 Then
                    fname0 = Left$(fname, j - 1)
                    fsuffix = Right$(fname, 4)
                    buf2 = Mid$(fname, j + 1, Len(fname) - j - 5)
                    j = InStr(buf2, "-")
                    first_id = CInt(Left$(buf2, j - 1))
                    last_id = CInt(Mid$(buf2, j + 1))
                Else
                    first_id = -1
                End If
                
                '�摜�\���̃I�v�V����
                options = ""
                n = LLength(buf)
                j = 2
                opt_n = 2
                Do While j <= n
                    buf2 = LIndex(buf, j)
                    Select Case buf2
                        Case "����", "�w�i", "����", "�Z�s�A", _
                            "��", "��", "�㉺���]", "���E���]", _
                            "�㔼��", "������", "�E����", "������", _
                            "�E��", "����", "�E��", "����", _
                            "�l�K�|�W���]", "�V���G�b�g", _
                            "�[�Ă�", "����", "�ێ�"
                            options = options & buf2 & " "
                        Case "����"
                            clear_every_time = True
                        Case "�E��]"
                            j = j + 1
                            options = options & "�E��] " & LIndex(buf, j) & " "
                        Case "����]"
                            j = j + 1
                            options = options & "����] " & LIndex(buf, j) & " "
                        Case "-"
                            '�X�L�b�v
                            opt_n = j + 1
                        Case Else
                            If Asc(buf2) = 35 And Len(buf2) = 7 Then
                                '���ߐF�ݒ�
                                cname = String$(8, vbNullChar)
                                Mid(cname, 1, 2) = "&H"
                                Mid(cname, 3, 2) = Mid$(buf2, 6, 2)
                                Mid(cname, 5, 2) = Mid$(buf2, 4, 2)
                                Mid(cname, 7, 2) = Mid$(buf2, 2, 2)
                                If IsNumeric(cname) Then
                                    If CLng(cname) <> vbWhite Then
                                        options = options & Format$(CLng(cname)) & " "
                                    End If
                                End If
                            ElseIf IsNumeric(buf2) Then
                                '�X�L�b�v
                                opt_n = j + 1
                            End If
                    End Select
                    j = j + 1
                Loop
                
                If Asc(fname) = 64 Then '@
                    '�p�C���b�g�摜�؂�ւ��̏ꍇ
                    
                    If first_id = -1 Then
                        fname = Mid$(fname, 2)
                    Else
                        fname0 = Mid$(fname0, 2)
                        fname = fname0 & Format$(first_id, "00") & fsuffix
                    End If
                    
                    '�E�B���h�E���\������Ă��Ȃ���Ε\��
                    If Not frmMessage.Visible Then
                        OpenMessageForm
                    End If
                    
                    If wait_time > 0 Then
                        start_time = timeGetTime()
                    End If
                    
                    '�摜�\���̃I�v�V����
                    options = options & " ���b�Z�[�W"
                    Select Case MapDrawMode
                        Case "�Z�s�A", "����"
                            options = options & " " & MapDrawMode
                    End Select
                    
                    If first_id = -1 Then
                        '�P���摜�̏ꍇ
                        DrawPicture fname, 0, 0, 64, 64, 0, 0, 0, 0, options
                        frmMessage.picFace.Refresh
                        
                        If wait_time > 0 Then
                            Do While (start_time + wait_time > timeGetTime())
                                Sleep 20
                            Loop
                        End If
                    Else
                        '�A�j���[�V�����̏ꍇ
                        For j = first_id To last_id
                            fname = fpath & fname0 & Format$(j, "00") & fsuffix
                            
                            DrawPicture fname, 0, 0, 64, 64, 0, 0, 0, 0, options
                            
                            frmMessage.picFace.Refresh
                            
                            If wait_time > 0 Then
                                wait_time2 = wait_time * (j - first_id + 1) \ (last_id - first_id)
                                cur_time = timeGetTime()
                                If cur_time < start_time + wait_time2 Then
                                    Sleep start_time + wait_time2 - cur_time
                                End If
                            End If
                        Next
                    End If
                    wait_time = DEFAULT_LEVEL
                    
                    DisplayedPilot = fname
                    GoTo NextMessage
                End If
                
                '�\���摜�̃T�C�Y
                If opt_n > 2 Then
                    buf2 = LIndex(buf, 2)
                    If buf2 = "-" Then
                        dw = DEFAULT_LEVEL
                    Else
                        dw = StrToLng(buf2)
                    End If
                    buf2 = LIndex(buf, 3)
                    If buf2 = "-" Then
                        dh = DEFAULT_LEVEL
                    Else
                        dh = StrToLng(buf2)
                    End If
                Else
                    dw = DEFAULT_LEVEL
                    dh = DEFAULT_LEVEL
                End If
                
                '�\���摜�̈ʒu
                If opt_n > 4 Then
                    buf2 = LIndex(buf, 4)
                    If buf2 = "-" Then
                        dx = DEFAULT_LEVEL
                    Else
                        dx = StrToLng(buf2)
                    End If
                    buf2 = LIndex(buf, 5)
                    If buf2 = "-" Then
                        dy = DEFAULT_LEVEL
                    Else
                        dy = StrToLng(buf2)
                    End If
                Else
                    dx = DEFAULT_LEVEL
                    dy = DEFAULT_LEVEL
                End If
                
                If wait_time > 0 Then
                    start_time = timeGetTime()
                End If
                
                If first_id = -1 Then
                    '�P���G�̏ꍇ
                    If clear_every_time Then
                        ClearPicture
                    End If
                    
                    DrawPicture fname, dx, dy, dw, dh, _
                        0, 0, 0, 0, options
                    
                    need_refresh = True
                    
                    If wait_time > 0 Then
                        MainForm.picMain(0).Refresh
                        need_refresh = False
                        cur_time = timeGetTime()
                        If cur_time < start_time + wait_time Then
                            Sleep start_time + wait_time - cur_time
                        End If
                        wait_time = DEFAULT_LEVEL
                    End If
                Else
                    '�A�j���[�V�����̏ꍇ
                    For j = first_id To last_id
                        fname = fname0 & Format$(j, "00") & fsuffix
                        
                        If clear_every_time Then
                            ClearPicture
                        End If
                        
                        DrawPicture fname, dx, dy, dw, dh, _
                            0, 0, 0, 0, options
                        
                        MainForm.picMain(0).Refresh
                        
                        If wait_time > 0 Then
                            wait_time2 = wait_time * (j - first_id + 1) \ (last_id - first_id)
                            cur_time = timeGetTime()
                            If cur_time < start_time + wait_time2 Then
                                Sleep start_time + wait_time2 - cur_time
                            End If
                        End If
                    Next
                    wait_time = DEFAULT_LEVEL
                End If
                
                GoTo NextMessage
                
            Case ".wav", ".mp3"
                '�E�{�^����������Ă�����X�L�b�v
                If IsRButtonPressed() Then
                    GoTo NextMessage
                End If
                
                '���ʉ��̉��t
                PlayWave buf
                If wait_time > 0 Then
                    If need_refresh Then
                        MainForm.picMain(0).Refresh
                        need_refresh = False
                    End If
                    Sleep wait_time
                    wait_time = DEFAULT_LEVEL
                End If
                GoTo NextMessage
        End Select
        
        '�퓬�A�j���Ăяo��
        If Left$(buf, 1) = "@" Then
            ShowAnimation Mid$(buf, 2)
            GoTo NextMessage
        End If
        
        '����R�}���h
        Select Case LCase$(LIndex(buf, 1))
            Case "clear"
                '�J�b�g�C���̏���
                ClearPicture
                need_refresh = True
                GoTo NextMessage
                
            Case "keep"
                '�J�b�g�C���̕ۑ�
                IsPictureDrawn = False
                GoTo NextMessage
        End Select
        
        '�E�F�C�g
        If IsNumeric(buf) Then
            wait_time = 100 * CDbl(buf)
            GoTo NextMessage
        End If
        
        '�����胁�b�Z�[�W�̕\��
        
        '�󃁃b�Z�[�W�̏ꍇ�͕\�����Ȃ�
        If buf = "" Then
            GoTo NextMessage
        End If
        
        '���b�Z�[�W�E�B���h�E�̏�Ԃ��ω����Ă���ꍇ�͕���
        KeepMessageFormStatus
        
        With p
            '�E�B���h�E���N���A
            .Cls
            .CurrentX = 1
            
            '�t�H���g�ݒ��������
            .FontBold = False
            .FontItalic = False
            .FontName = "�l�r �o����"
            .FontSize = 12
            .ForeColor = vbBlack
        End With
        
        '�b�Җ��Ɗ��ʂ̕\������
        is_char_message = False
        If pname <> "�V�X�e��" _
            And ((pname <> "" And pname <> "-") _
                Or ((Left$(buf, 1) = "�u" And Right$(buf, 1) = "�v")) _
                Or ((Left$(buf, 1) = "�w" And Right$(buf, 1) = "�x"))) _
        Then
            Dim with_footer As Boolean
            
            is_char_message = True
            
            '�b�҂̃O���t�B�b�N��\��
            If pname = "-" _
                And Not SelectedUnit Is Nothing _
            Then
                If SelectedUnit.CountPilot > 0 Then
                    fname = SelectedUnit.MainPilot.Bitmap
                    If DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "���b�Z�[�W " & msg_mode) Then
                        frmMessage.picFace.Refresh
                        DisplayedPilot = fname
                        DisplayMode = msg_mode
                    End If
                End If
            End If
            
            '�b�Җ���\��
            If pnickname = "" _
                And pname = "-" _
                And Not SelectedUnit Is Nothing _
            Then
                If SelectedUnit.CountPilot > 0 Then
                    p.Print SelectedUnit.MainPilot.Nickname
                End If
            ElseIf pnickname <> "" Then
                p.Print pnickname
            End If
            
            '���b�Z�[�W���r���ŏI����Ă��邩����
            If Right$(buf, 1) <> ":" Then
                with_footer = True
            Else
                with_footer = False
                prev_lnum = lnum
                buf = Left$(buf, Len(buf) - 1)
            End If
            
            '���ʂ�t��
            If (Left$(buf, 1) = "(" Or Left$(buf, 1) = "�i") _
                And (Not with_footer Or (Right$(buf, 1) = ")" Or Right$(buf, 1) = "�j")) _
            Then
                '���m���[�O
                If with_footer Then
                    buf = Mid$(buf, 2, Len(buf) - 2)
                    buf = "�i" & buf & "�j"
                Else
                    buf = Mid$(buf, 2)
                    buf = "�i" & buf
                End If
            ElseIf Left$(buf, 1) = "�u" _
                And (Not with_footer Or Right$(buf, 1) = "�v") _
            Then
                '�u�v�̊��ʂ����ɂ���̂ŕύX���Ȃ�
            ElseIf Left$(buf, 1) = "�w" _
                And (Not with_footer Or Right$(buf, 1) = "�x") _
            Then
                '�w�x�̊��ʂ����ɂ���̂ŕύX���Ȃ�
            Else
                If with_footer Then
                    buf = "�u" & buf & "�v"
                Else
                    buf = "�u" & buf
                End If
            End If
        Else
            '���b�Z�[�W���r���ŏI����Ă��邩����
            If Right$(buf, 1) = ":" Then
                prev_lnum = lnum
                buf = Left$(buf, Len(buf) - 1)
            End If
        End If
        prev_lnum = MaxLng(prev_lnum, 1)
        
        lnum = 0
        line_head = 1
        For j = 1 To Len(buf)
            ch = Mid$(buf, j, 1)
            
            '�u.�v�̏ꍇ�͕K�����s
            If ch = "." Then
                If j <> line_head Then
                    PrintMessage Mid$(buf, line_head, j - line_head), Not is_char_message
                    If is_char_message Then
                        p.Print "  ";
                    End If
                    lnum = lnum + 1
                End If
                line_head = j + 1
                GoTo NextLoop
            End If
                
            '�^�O���ł͉��s���Ȃ�
            If ch = "<" Then
                in_tag = True
                GoTo NextLoop
            ElseIf ch = ">" Then
                in_tag = False
            ElseIf in_tag Then
                GoTo NextLoop
            End If
            
            '���b�Z�[�W���r�؂�Ă��܂��ꍇ�͕K�����s
            If MessageLen(Mid$(buf, line_head, j - line_head)) > 0.95 * p.width Then
                PrintMessage Mid$(buf, line_head, j - line_head + 1), Not is_char_message
                If is_char_message Then
                    p.Print "  ";
                End If
                line_head = j + 1
                lnum = lnum + 1
                GoTo NextLoop
            End If
            
            '�֑�����
            Select Case Mid$(buf, j + 1, 1)
                Case "�B", "�A", "�c", "�d", "�E", "�", _
                    "�`", "�[", "�|", "�I", "�H", _
                    "�v", "�x", "�j", ")", " ", "."
                    GoTo NextLoop
            End Select
            Select Case Mid$(buf, j + 2, 1)
                Case "�B", "�A", "�c", "�d", "�E", "�", _
                    "�`", "�[", "�|", "�I", "�H", _
                    "�v", "�x", "�j", ")", " ", "."
                    GoTo NextLoop
            End Select
            If Mid$(buf, j + 3, 1) = "." Then
                GoTo NextLoop
            End If
            
            '���s�̔���
            If MessageLen(Mid$(messages(i), line_head)) < 0.95 * p.width Then
                '�S�̂���s�Ɏ��܂�ꍇ
                GoTo NextLoop
            End If
            Select Case ch
                Case "�B"
                    If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(1) * p.width Then
                        PrintMessage Mid$(buf, line_head, j - line_head + 1), Not is_char_message
                        If is_char_message Then
                            p.Print "  ";
                        End If
                        line_head = j + 1
                        lnum = lnum + 1
                    End If
                Case " "
                    If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(1) * p.width Then
                        PrintMessage Mid$(buf, line_head, j - line_head), Not is_char_message
                        If is_char_message Then
                            p.Print "  ";
                        End If
                        line_head = j + 1
                        lnum = lnum + 1
                    End If
                Case "�A"
                    If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(2) * p.width Then
                        PrintMessage Mid$(buf, line_head, j - line_head + 1), Not is_char_message
                        If is_char_message Then
                            p.Print "  ";
                        End If
                        line_head = j + 1
                        lnum = lnum + 1
                    End If
                Case Else
                    If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(0) * p.width Then
                        PrintMessage Mid$(buf, line_head, j - line_head), Not is_char_message
                        If is_char_message Then
                            p.Print "  ";
                        End If
                        line_head = j
                        lnum = lnum + 1
                    End If
            End Select
NextLoop:
        Next
        '���b�Z�[�W�̎c���\�����Ă���
        If Len(buf) >= line_head Then
            PrintMessage Mid$(buf, line_head), Not is_char_message
            lnum = lnum + 1
        End If
        
        '�t�H���g�ݒ�����ɖ߂�
        With p
            .FontBold = False
            .FontItalic = False
            .FontName = "�l�r �o����"
            .FontSize = 12
            .ForeColor = vbBlack
        End With
        
        '�f�t�H���g�̃E�F�C�g
        If wait_time = DEFAULT_LEVEL Then
            wait_time = (lnum - prev_lnum + 1) * MessageWait
            If msg_mode = "����" Then
                wait_time = wait_time \ 2
            End If
        End If
        
        '��ʂ��X�V
        If need_refresh Then
            MainForm.picMain(0).Refresh
            need_refresh = False
        End If
        DoEvents
        
        '�҂����Ԃ��؂��܂őҋ@
        start_time = timeGetTime()
        IsFormClicked = False
        Do While (start_time + wait_time > timeGetTime())
            '���{�^���������ꂽ�烁�b�Z�[�W����
            If IsFormClicked Then
                Exit Do
            End If
            
            '�E�{�^����������Ă����瑁����
            If IsRButtonPressed() Then
                Exit Do
            End If
            
            Sleep 20
            DoEvents
        Loop
        wait_time = DEFAULT_LEVEL
NextMessage:
    Next
    
    '�퓬�A�j���f�[�^�̃J�b�g�C���\���H
    If pname = "-" Then
        Exit Sub
    End If
    
    '��ʂ��X�V
    If need_refresh Then
        MainForm.picMain(0).Refresh
        need_refresh = False
    End If
    
    '���b�Z�[�W�f�[�^�̍Ō�ɃE�F�C�g�̎w�肪�s���Ă����ꍇ
    If wait_time > 0 Then
        start_time = timeGetTime()
        IsFormClicked = False
        Do While (start_time + wait_time > timeGetTime())
            '���{�^���������ꂽ�烁�b�Z�[�W����
            If IsFormClicked Then
                Exit Do
            End If
            
            '�E�{�^����������Ă����瑁����
            If IsRButtonPressed() Then
                Exit Do
            End If
            
            Sleep 20
            DoEvents
        Loop
    End If
    
    '���b�Z�[�W�E�B���h�E�̏�Ԃ��ω����Ă���ꍇ�͕���
    KeepMessageFormStatus
    
    DoEvents
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "�摜�t�@�C��" & vbCr & vbLf _
        & fname & vbCr & vbLf _
        & "�̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCr & vbLf _
        & "�摜�t�@�C�������Ă��Ȃ����m�F���ĉ������B"
End Sub

'�V�X�e���ɂ�郁�b�Z�[�W��\��
Public Sub DisplaySysMessage(ByVal msg As String, Optional ByVal short_wait As Boolean)
Dim i As Integer, j As Integer, line_head As Integer
Dim ch As String, buf As String
Dim p As PictureBox
Dim lnum As Integer
Dim start_time As Long, wait_time As Long
Dim in_tag As Boolean

    '���b�Z�[�W�E�B���h�E���\������Ă��Ȃ��ꍇ�͕\�����L�����Z��
    If frmMessage.WindowState = 1 Then
        Exit Sub
    End If

    '���b�Z�[�W���̎���u��
    FormatMessage msg

    '�E�B���h�E�̃L���v�V������ݒ�
    If frmMessage.Caption = "���b�Z�[�W (��������)" Then
        frmMessage.Caption = "���b�Z�[�W"
    End If

    Set p = frmMessage.picMessage

    With p
        '���b�Z�[�W�E�B���h�E���N���A
        .Cls
        .CurrentX = 1

        '�t�H���g�ݒ��������
        .FontBold = False
        .FontItalic = False
        .FontName = "�l�r �o����"
        .FontSize = 12
        .ForeColor = vbBlack
    End With

    '���b�Z�[�W��\��
    lnum = 0
    line_head = 1
    For i = 1 To Len(msg)
        ch = Mid$(msg, i, 1)

        '�u;�v�̏ꍇ�͕K�����s
        If ch = ";" Then
            If line_head <> i Then
                buf = Mid$(msg, line_head, i - line_head)
                PrintMessage buf, True
                lnum = lnum + 1
            End If
            line_head = i + 1
            GoTo NextLoop
        End If

        '�^�O���ł͉��s���Ȃ�
        If ch = "<" Then
            in_tag = True
            GoTo NextLoop
        ElseIf ch = ">" Then
            in_tag = False
        ElseIf in_tag Then
            GoTo NextLoop
        End If

        '�֑�����
        If ch = "�B" Or ch = "�A" Then
            GoTo NextLoop
        End If
        If i < Len(msg) Then
            If Mid$(msg, i + 1, 1) = "�B" Or Mid$(msg, i + 1, 1) = "�A" Then
                GoTo NextLoop
            End If
        End If

        If MessageLen(Mid$(msg, line_head)) < p.width Then
            '�S�̂���s�Ɏ��܂�ꍇ
            GoTo NextLoop
        End If

        If IsSpace(ch) _
            And MessageLen(Mid$(msg, line_head, i - line_head)) > 0.5 * p.width _
        Then
            buf = Mid$(msg, line_head, i - line_head)
            PrintMessage buf, True
            lnum = lnum + 1
            line_head = i + 1
        ElseIf MessageLen(Mid$(msg, line_head, i - line_head + 1)) > 0.95 * p.width Then
            buf = Mid$(msg, line_head, i - line_head + 1)
            PrintMessage buf, True
            lnum = lnum + 1
            line_head = i + 1
        ElseIf ch = "[" Then
            '[]�ň͂܂ꂽ��������ł͉��s���Ȃ�
            For j = i To Len(msg)
                If Mid$(msg, j, 1) = "]" Then
                    Exit For
                End If
            Next
            If MessageLen(Mid$(msg, line_head, j - line_head)) > 0.95 * p.width Then
                buf = Mid$(msg, line_head, i - line_head)
                PrintMessage buf, True
                lnum = lnum + 1
                line_head = i
            End If
        End If
NextLoop:
    Next
    buf = Mid$(msg, line_head)
    PrintMessage buf, True
    lnum = lnum + 1

    '�t�H���g�ݒ�����ɖ߂�
    With p
        .FontBold = False
        .FontItalic = False
        .FontName = "�l�r �o����"
        .FontSize = 12
        .ForeColor = vbBlack
    End With

    '�E�F�C�g���v�Z
    wait_time = (0.8 + 0.5 * lnum) * MessageWait
    If short_wait Then
        wait_time = wait_time \ 2
    End If

    DoEvents

    '�҂����Ԃ��؂��܂őҋ@
    IsFormClicked = False
    start_time = timeGetTime()
    Do While (start_time + wait_time > timeGetTime())
        '���{�^���������ꂽ�烁�b�Z�[�W����
        If IsFormClicked Then
            Exit Do
        End If

        '�E�{�^����������Ă����瑁����
        If IsRButtonPressed() Then
            Exit Do
        End If

        Sleep 20
        DoEvents
    Loop
End Sub


' === �}�b�v�E�B���h�E�Ɋւ��鏈�� ===

'�}�b�v��ʔw�i�̐ݒ�
Public Sub SetupBackground(Optional draw_mode As String, Optional draw_option As String, _
    Optional filter_color As Long, Optional filter_trans_par As Double)
Dim i As Integer, j As Integer, k As Integer, ret As Integer
Dim xx As Integer, yy As Integer
Dim terrain_bmp_count As Integer
Dim terrain_bmp_type() As String
Dim terrain_bmp_num() As Integer
Dim terrain_bmp_x() As Integer
Dim terrain_bmp_y() As Integer
Dim fname As String
    
    IsMapDirty = False
    IsPictureVisible = False
    IsCursorVisible = False
    
    '���j�b�g�摜�F��ύX���Ȃ��Ƃ����Ȃ��ꍇ
    If MapDrawMode <> draw_mode Then
        UList.ClearUnitBitmap
        MapDrawMode = draw_mode
        MapDrawFilterColor = filter_color
        MapDrawFilterTransPercent = filter_trans_par
    ElseIf draw_mode = "�t�B���^" _
        And (MapDrawFilterColor <> filter_color _
            Or MapDrawFilterTransPercent <> filter_trans_par) _
    Then
        UList.ClearUnitBitmap
        MapDrawMode = draw_mode
        MapDrawFilterColor = filter_color
        MapDrawFilterTransPercent = filter_trans_par
    End If
    
    '�}�b�v�w�i�̐ݒ�
    With MainForm
        Select Case draw_option
            Case "�X�e�[�^�X"
                With .picBack
                    ret = PatBlt(.hDC, 0, 0, .width, .Height, BLACKNESS)
                End With
                Exit Sub
            Case Else
                MapX = MainWidth \ 2 + 1
                MapY = MainHeight \ 2 + 1
        End Select
        
        '�e�}�X�̃}�b�v�摜��\��
        For i = 1 To MapWidth
            For j = 1 To MapHeight
                xx = 32 * (i - 1)
                yy = 32 * (j - 1)
                
'DEL START 240a
'                '�}�b�v�摜�����ɓǂݍ��܂�Ă��邩����
'                For k = 1 To terrain_bmp_count
'                    If terrain_bmp_type(k) = MapData(i, j, 0) _
'                        And terrain_bmp_num(k) = MapData(i, j, 1) _
'                    Then
'                        Exit For
'                    End If
'                Next
                
'                If k <= terrain_bmp_count Then
'                    '���ɕ`��ς݂̉摜�͕`�悵��������]��
'                    ret = BitBlt(.picBack.hDC, _
'                        xx, yy, 32, 32, _
'                        .picBack.hDC, terrain_bmp_x(k), terrain_bmp_y(k), SRCCOPY)
'                    MapImageFileTypeData(i, j) = _
'                        MapImageFileTypeData(terrain_bmp_x(k) \ 32 + 1, terrain_bmp_y(k) \ 32 + 1)
'                Else
'                    '�V�K�̉摜�̏ꍇ
'DEL  END  240a
                '�摜�t�@�C����T��
'MOD START 240a
'                fname = SearchTerrainImageFile(MapData(i, j, 0), MapData(i, j, 1), i, j)
                fname = SearchTerrainImageFile(MapData(i, j, MapDataIndex.TerrainType), MapData(i, j, MapDataIndex.BitmapNo), i, j)
'MOD  END  240a
                
                '�摜����荞��
                If fname <> "" Then
                    On Error GoTo ErrorHandler
                    .picTmp32(0) = LoadPicture(fname)
                    On Error GoTo 0
                Else
                    ret = PatBlt(.picTmp32(0).hDC, 0, 0, 32, 32, BLACKNESS)
                End If
                
                '�}�b�v�ݒ�ɂ���ĕ\���F��ύX
                Select Case draw_mode
                    Case "��"
                        GetImage .picTmp32(0)
                        Dark
                        SetImage .picTmp32(0)
                    Case "�Z�s�A"
                        GetImage .picTmp32(0)
                        Sepia
                        SetImage .picTmp32(0)
                    Case "����"
                        GetImage .picTmp32(0)
                        Monotone
                        SetImage .picTmp32(0)
                    Case "�[�Ă�"
                        GetImage .picTmp32(0)
                        Sunset
                        SetImage .picTmp32(0)
                    Case "����"
                        GetImage .picTmp32(0)
                        Water
                        SetImage .picTmp32(0)
                    Case "�t�B���^"
                        GetImage .picTmp32(0)
                        ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent
                        SetImage .picTmp32(0)
                End Select
                
                '�摜��`������
                ret = BitBlt(.picBack.hDC, xx, yy, 32, 32, _
                    .picTmp32(0).hDC, 0, 0, SRCCOPY)
'DEL START 240a
'                    '�摜��o�^
'                    terrain_bmp_count = terrain_bmp_count + 1
'                    ReDim Preserve terrain_bmp_type(terrain_bmp_count)
'                    ReDim Preserve terrain_bmp_num(terrain_bmp_count)
'                    ReDim Preserve terrain_bmp_x(terrain_bmp_count)
'                    ReDim Preserve terrain_bmp_y(terrain_bmp_count)
'                    terrain_bmp_type(terrain_bmp_count) = MapData(i, j, 0)
'                    terrain_bmp_num(terrain_bmp_count) = MapData(i, j, 1)
'                    terrain_bmp_x(terrain_bmp_count) = xx
'                    terrain_bmp_y(terrain_bmp_count) = yy
'                End If
'DEL  END  240a
'ADD START 240a
                '���C���[�`�悷��K�v������ꍇ�͕`�悷��
                If BoxTypes.Upper = MapData(i, j, MapDataIndex.BoxType) _
                    Or BoxTypes.UpperBmpOnly = MapData(i, j, MapDataIndex.BoxType) _
                Then
                    '�摜�t�@�C����T��
                     fname = SearchTerrainImageFile(MapData(i, j, MapDataIndex.LayerType), MapData(i, j, MapDataIndex.LayerBitmapNo), i, j)
                    
                    '�摜����荞��
                    If fname <> "" Then
On Error GoTo ErrorHandler
                        .picTmp32(0) = LoadPicture(fname)
On Error GoTo 0
                        BGColor = vbWhite
                        '�}�b�v�ݒ�ɂ���ĕ\���F��ύX
                        Select Case draw_mode
                            Case "��"
                                GetImage .picTmp32(0)
                                Dark True
                                SetImage .picTmp32(0)
                            Case "�Z�s�A"
                                GetImage .picTmp32(0)
                                Sepia True
                                SetImage .picTmp32(0)
                            Case "����"
                                GetImage .picTmp32(0)
                                Monotone True
                                SetImage .picTmp32(0)
                            Case "�[�Ă�"
                                GetImage .picTmp32(0)
                                Sunset True
                                SetImage .picTmp32(0)
                            Case "����"
                                GetImage .picTmp32(0)
                                Water True
                                SetImage .picTmp32(0)
                            Case "�t�B���^"
                                GetImage .picTmp32(0)
                                ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent, True
                                SetImage .picTmp32(0)
                        End Select
                        
                        '�摜�𓧉ߕ`������
                        ret = TransparentBlt(.picBack.hDC, xx, yy, 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, BGColor)
                    End If
                    
                End If
'ADD  END  240a
            Next
        Next
'MapDrawn:  '�g�p����Ă��Ȃ����x���Ȃ̂ō폜
        
        '�}�X�ڂ̕\��
        If ShowSquareLine Then
            MainForm.picBack.Line (0, 0)-(MapPWidth - 1, MapPHeight - 1), rgb(100, 100, 100), B
            For i = 1 To MapWidth - 1
                .picBack.Line (32 * i, -1)-(32 * i, MapPHeight), rgb(100, 100, 100)
            Next
            For i = 1 To MapHeight - 1
                .picBack.Line (0, 32 * i - 1)-(MapPWidth, 32 * i - 1), rgb(100, 100, 100)
            Next
        End If
        
        '�}�X�N����w�i��ʂ��쐬
        ret = BitBlt(.picMaskedBack.hDC, _
            0, 0, MapPWidth, MapPHeight, _
            .picBack.hDC, 0, 0, SRCCOPY)
        For i = 1 To MapWidth
            For j = 1 To MapHeight
                xx = 32 * (i - 1)
                yy = 32 * (j - 1)
                ret = BitBlt(.picMaskedBack.hDC, _
                    xx, yy, 32, 32, _
                    .picMask.hDC, 0, 0, SRCAND)
                ret = BitBlt(.picMaskedBack.hDC, _
                    xx, yy, 32, 32, _
                    .picMask2.hDC, 0, 0, SRCINVERT)
            Next
        Next
    End With
    
    '��ʂ��X�V
    If MapFileName <> "" And draw_option = "" Then
        RefreshScreen
    End If
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "�}�b�v�p�r�b�g�}�b�v�t�@�C��" & vbCr & vbLf _
        & fname & vbCr & vbLf _
        & "�̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCr & vbLf _
        & "�摜�t�@�C�������Ă��Ȃ����m�F���ĉ������B"
    TerminateSRC
End Sub

'��ʂ̏������� (���j�b�g�\�������蒼��)
Public Sub RedrawScreen(Optional ByVal late_refresh As Boolean)
Dim PT As POINTAPI
Dim ret As Long

    ScreenIsMasked = False
    
    '��ʂ��X�V
    RefreshScreen False, late_refresh
    
    '�J�[�\�����ĕ`��
    GetCursorPos PT
    ret = SetCursorPos(PT.X, PT.Y)
End Sub

'��ʂ��}�X�N�������čĕ\��
Public Sub MaskScreen()
    ScreenIsMasked = True
    
    '��ʂ��X�V
    RefreshScreen
End Sub

' ADD START MARGE
'��ʂ̏�������
Public Sub RefreshScreen(Optional ByVal without_refresh As Boolean, _
    Optional ByVal delay_refresh As Boolean)

    If NewGUIMode Then
        RefreshScreenNew without_refresh, delay_refresh
    Else
        RefreshScreenOld without_refresh, delay_refresh
    End If
End Sub
' ADD END MARGE


'��ʂ̏������� (��GUI)
' MOD START MARGE
'Public Sub RefreshScreen(Optional ByVal without_refresh As Boolean, _
'    Optional ByVal delay_refresh As Boolean)
Private Sub RefreshScreenOld(Optional ByVal without_refresh As Boolean, _
    Optional ByVal delay_refresh As Boolean)
' MOD END MARGE
Dim pic As PictureBox
Dim mx As Integer, my As Integer
Dim sx As Integer, sy As Integer
Dim dx As Integer, dy As Integer
Dim dw As Integer, dh As Integer
Dim xx As Integer, yy As Integer
Dim ret As Long, i As Integer, j As Integer
Dim u As Unit
Dim PT As POINTAPI
Dim prev_color As Long

    '�}�b�v�f�[�^���ݒ肳��Ă��Ȃ���Ή�ʏ����������s��Ȃ�
    If MapWidth = 1 Then
        Exit Sub
    End If
    
    '�\���ʒu���}�b�v�O�ɂ���ꍇ�̓}�b�v���ɍ��킹��
    If MapX < 1 Then
        MapX = 1
    ElseIf MapX > MapWidth Then
        MapX = MapWidth
    End If
    If MapY < 1 Then
        MapY = 1
    ElseIf MapY > MapHeight Then
        MapY = MapHeight
    End If
    
    With MainForm
        Set pic = .picMain(0)
        
        If Not without_refresh Then
            IsPictureVisible = False
            IsCursorVisible = False
            
            PaintedAreaX1 = MainPWidth
            PaintedAreaY1 = MainPHeight
            PaintedAreaX2 = -1
            PaintedAreaY2 = -1
            
            '�}�b�v�E�B���h�E�̃X�N���[���o�[�̈ʒu��ύX
            If Not IsGUILocked Then
                If .HScroll.Value <> MapX Then
                    .HScroll.Value = MapX
                    Exit Sub
                End If
                If .VScroll.Value <> MapY Then
                    .VScroll.Value = MapY
                    Exit Sub
                End If
            End If
            
            '��U�}�b�v�E�B���h�E�̓��e������
            ret = PatBlt(pic.hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
        End If
        
        mx = MapX - (MainWidth + 1) \ 2 + 1
        my = MapY - (MainHeight + 1) \ 2 + 1
        
        '�}�b�v�摜�̓]�����Ɠ]������v�Z����
        
        If mx < 1 Then
            sx = 1
            dx = 2 - mx
            dw = MainWidth - (1 - mx)
        ElseIf mx + MainWidth - 1 > MapWidth Then
            sx = mx
            dx = 1
            dw = MainWidth - (mx + MainWidth - 1 - MapWidth)
        Else
            sx = mx
            dx = 1
            dw = MainWidth
        End If
        If dw > MainWidth Then
            dw = MainWidth
        End If
        
        If my < 1 Then
            sy = 1
            dy = 2 - my
            dh = MainHeight - (1 - my)
        ElseIf my + MainHeight - 1 > MapHeight Then
            sy = my
            dy = 1
            dh = MainHeight - (my + MainHeight - 1 - MapHeight)
        Else
            sy = my
            dy = 1
            dh = MainHeight
        End If
        If dh > MainHeight Then
            dh = MainHeight
        End If
        
        '������`�悷��ۂ̕`��F�����ɕύX
        prev_color = pic.ForeColor
        pic.ForeColor = vbBlack
        
        '�\�����e���X�V
        If Not ScreenIsMasked Then
            '�ʏ�\��
            For i = 0 To dw - 1
                xx = 32 * (dx + i - 1)
                For j = 0 To dh - 1
                    If sx + i < 1 Or sx + i > MapWidth _
                        Or sy + j < 1 Or sy + j > MapHeight _
                    Then
                        GoTo NextLoop
                    End If
                    
                    yy = 32 * (dy + j - 1)
                    
                    Set u = MapDataForUnit(sx + i, sy + j)
                    If u Is Nothing Then
                        '�n�`
                        ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                            .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                            SRCCOPY)
                    ElseIf u.BitmapID = -1 Then
                        '��\���̃��j�b�g
                        ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                            .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                            SRCCOPY)
                    Else
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
                                ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                ret = LineTo(pic.hDC, xx + 31, yy + 28)
                            Case "����"
                                ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                ret = LineTo(pic.hDC, xx + 31, yy + 3)
                            Case "�n��"
                                ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                ret = LineTo(pic.hDC, xx + 31, yy + 3)
                            Case "�F��"
                                If TerrainClass(sx + i, sy + j) = "����" Then
                                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                End If
                        End Select
                    End If
                    
NextLoop:
                Next
            Next
        Else
            '�}�X�N�\��
            For i = 0 To dw - 1
                xx = 32 * (dx + i - 1)
                For j = 0 To dh - 1
                    If sx + i < 1 Or sx + i > MapWidth _
                        Or sy + j < 1 Or sy + j > MapHeight _
                    Then
                        GoTo NextLoop2
                    End If
                    
                    yy = 32 * (dy + j - 1)
                    
                    Set u = MapDataForUnit(sx + i, sy + j)
                    If u Is Nothing Then
                        If MaskData(sx + i, sy + j) Then
                            '�}�X�N���ꂽ�n�`
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        Else
                            '�n�`
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        End If
                    ElseIf u.BitmapID = -1 Then
                        '��\���̃��j�b�g
                        If MaskData(sx + i, sy + j) Then
                            '�}�X�N���ꂽ�n�`
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        Else
                            '�n�`
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        End If
                    Else
                        If MaskData(sx + i, sy + j) Then
                            '�}�X�N���ꂽ���j�b�g
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picUnitBitmap.hDC, _
                                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 64, _
                                SRCCOPY)
                            
                            '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
                            Select Case u.Area
                                Case "��"
                                    DottedLine xx, yy + 28
                                Case "����"
                                    DottedLine xx, yy + 3
                                Case "�n��"
                                    DottedLine xx, yy + 28
                                    DottedLine xx, yy + 3
                                Case "�F��"
                                    If TerrainClass(sx + i, sy + j) = "����" Then
                                        DottedLine xx, yy + 28
                                    End If
                            End Select
                        Else
                            '���j�b�g
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picUnitBitmap.hDC, _
                                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
                                SRCCOPY)
                            
                            '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
                            Select Case u.Area
                                Case "��"
                                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                Case "����"
                                    ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                Case "�n��"
                                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                    ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                Case "�F��"
                                    If TerrainClass(sx + i, sy + j) = "����" Then
                                        ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                        ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                    End If
                            End Select
                        End If
                    End If
                    
NextLoop2:
                Next
            Next
        End If
        
        '�`��F�����ɖ߂��Ă���
        pic.ForeColor = prev_color
        
        '��ʂ�����������ꂽ���Ƃ��L�^
        ScreenIsSaved = False
        
        If Not without_refresh And Not delay_refresh Then
            .picMain(0).Refresh
        End If
    End With
End Sub

' ADD START MARGE
'��ʂ̏������� (�VGUI)
Private Sub RefreshScreenNew(Optional ByVal without_refresh As Boolean, _
    Optional ByVal delay_refresh As Boolean)
Dim pic As PictureBox
Dim mx As Integer, my As Integer
Dim sx As Integer, sy As Integer
Dim dx As Integer, dy As Integer
Dim dw As Integer, dh As Integer
Dim xx As Integer, yy As Integer
Dim ret As Long, i As Integer, j As Integer
Dim u As Unit
Dim PT As POINTAPI
Dim prev_color As Long

    '�}�b�v�f�[�^���ݒ肳��Ă��Ȃ���Ή�ʏ����������s��Ȃ�
    If MapWidth = 1 Then
        Exit Sub
    End If
    
    '�\���ʒu���}�b�v�O�ɂ���ꍇ�̓}�b�v���ɍ��킹��
    If MapX < 1 Then
        MapX = 1
    ElseIf MapX > MapWidth Then
        MapX = MapWidth
    End If
    If MapY < 1 Then
        MapY = 1
    ElseIf MapY > MapHeight Then
        MapY = MapHeight
    End If
    
    With MainForm
        Set pic = .picMain(0)
        
        If Not without_refresh Then
            IsPictureVisible = False
            IsCursorVisible = False
            
            PaintedAreaX1 = MainPWidth
            PaintedAreaY1 = MainPHeight
            PaintedAreaX2 = -1
            PaintedAreaY2 = -1
            
            '�}�b�v�E�B���h�E�̃X�N���[���o�[�̈ʒu��ύX
            If Not IsGUILocked Then
                If .HScroll.Value <> MapX Then
                    .HScroll.Value = MapX
                    Exit Sub
                End If
                If .VScroll.Value <> MapY Then
                    .VScroll.Value = MapY
                    Exit Sub
                End If
            End If
            
            '��U�}�b�v�E�B���h�E�̓��e������
            ret = PatBlt(pic.hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
        End If
        
        mx = MapX - (MainWidth + 1) \ 2 + 1
        my = MapY - (MainHeight + 1) \ 2 + 1
        
        '�}�b�v�摜�̓]�����Ɠ]������v�Z����
        
        If mx < 1 Then
            sx = 1
            dx = 2 - mx
            dw = MainWidth - (1 - mx)
        ElseIf mx + MainWidth - 1 > MapWidth Then
            sx = mx
            dx = 1
            dw = MainWidth - (mx + MainWidth - 1 - MapWidth)
        Else
            sx = mx
            dx = 1
            dw = MainWidth
        End If
        If dw > MainWidth Then
            dw = MainWidth
        End If
        
        If my < 1 Then
            sy = 1
            dy = 2 - my
            dh = MainHeight - (1 - my)
        ElseIf my + MainHeight - 1 > MapHeight Then
            sy = my
            dy = 1
            dh = MainHeight - (my + MainHeight - 1 - MapHeight)
        Else
            sy = my
            dy = 1
            dh = MainHeight
        End If
        If dh > MainHeight Then
            dh = MainHeight
        End If
        
        '������`�悷��ۂ̕`��F�����ɕύX
        prev_color = pic.ForeColor
        pic.ForeColor = vbBlack
        
        '�\�����e���X�V
        If Not ScreenIsMasked Then
            '�ʏ�\��
            For i = -1 To dw - 1
                xx = 32 * (dx + i - 0.5)
                For j = 0 To dh - 1
                    yy = 32 * (dy + j - 1)
                    
                    If sx + i < 1 Or sx + i > MapWidth _
                        Or sy + j < 1 Or sy + j > MapHeight _
                    Then
                        GoTo NextLoop
                    End If
                    
                    Set u = MapDataForUnit(sx + i, sy + j)
                    
                    If i = -1 Then
                        '��ʍ��[��16�s�N�Z�����������\��
                        If u Is Nothing Then
                            '�n�`
                            ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                SRCCOPY)
                        ElseIf u.BitmapID = -1 Then
                            '��\���̃��j�b�g
                            ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                SRCCOPY)
                        Else
                            If u.Action > 0 Or u.IsFeatureAvailable("�n�`���j�b�g") Then
                                '���j�b�g
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15), _
                                    SRCCOPY)
                            Else
                                '�s���ς̃��j�b�g
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15) + 32, _
                                    SRCCOPY)
                            End If
                            
                            '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
                            Select Case u.Area
                                Case "��"
                                    ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                    ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                Case "����"
                                    ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
                                    ret = LineTo(pic.hDC, 0 + 15, yy + 3)
                                Case "�n��"
                                    ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                    ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                    ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
                                    ret = LineTo(pic.hDC, 0 + 15, yy + 3)
                                Case "�F��"
                                    If TerrainClass(sx + i, sy + j) = "����" Then
                                        ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                        ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                    End If
                            End Select
                        End If
                    Else
                        '��ʍ��[�ȊO�͑S32�s�N�Z�����������\��
                        If u Is Nothing Then
                            '�n�`
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        ElseIf u.BitmapID = -1 Then
                            '��\���̃��j�b�g
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        Else
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
                                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                Case "����"
                                    ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                Case "�n��"
                                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                    ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                Case "�F��"
                                    If TerrainClass(sx + i, sy + j) = "����" Then
                                        ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                        ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                    End If
                            End Select
                        End If
                    End If
NextLoop:
                Next
            Next
        Else
            '�}�X�N�\��
            For i = -1 To dw - 1
                xx = 32 * (dx + i - 0.5)
                For j = 0 To dh - 1
                    yy = 32 * (dy + j - 1)
                    
                    If sx + i < 1 Or sx + i > MapWidth _
                        Or sy + j < 1 Or sy + j > MapHeight _
                    Then
                        GoTo NextLoop2
                    End If
                                        
                    Set u = MapDataForUnit(sx + i, sy + j)
                    
                    If i = -1 Then
                        '��ʍ��[��16�s�N�Z�����������\��
                        If u Is Nothing Then
                            If MaskData(sx + i, sy + j) Then
                                '�}�X�N���ꂽ�n�`
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picMaskedBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            Else
                                '�n�`
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            End If
                        ElseIf u.BitmapID = -1 Then
                            '��\���̃��j�b�g
                            If MaskData(sx + i, sy + j) Then
                                '�}�X�N���ꂽ�n�`
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picMaskedBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            Else
                                '�n�`
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            End If
                        Else
                            If MaskData(sx + i, sy + j) Then
                                '�}�X�N���ꂽ���j�b�g
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15) + 64, _
                                    SRCCOPY)
                                
                                '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
                                Select Case u.Area
                                    Case "��"
                                        DottedLine 0, yy + 28, True
                                    Case "����"
                                        DottedLine 0, yy + 3, True
                                    Case "�n��"
                                        DottedLine 0, yy + 28, True
                                        DottedLine 0, yy + 3, True
                                    Case "�F��"
                                        If TerrainClass(sx + i, sy + j) = "����" Then
                                            DottedLine 0, yy + 28, True
                                        End If
                                End Select
                            Else
                                '���j�b�g
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15), _
                                    SRCCOPY)
                                
                                '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
                                Select Case u.Area
                                    Case "��"
                                        ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                        ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                    Case "����"
                                        ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
                                        ret = LineTo(pic.hDC, 0 + 15, yy + 3)
                                    Case "�n��"
                                        ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                        ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                        ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
                                        ret = LineTo(pic.hDC, 0 + 15, yy + 3)
                                    Case "�F��"
                                        If TerrainClass(sx + i, sy + j) = "����" Then
                                            ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                            ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                        End If
                                End Select
                            End If
                        End If
                    Else
                        '��ʍ��[�ȊO�͑S32�s�N�Z�����������\��
                        If u Is Nothing Then
                            If MaskData(sx + i, sy + j) Then
                                '�}�X�N���ꂽ�n�`
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            Else
                                '�n�`
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            End If
                        ElseIf u.BitmapID = -1 Then
                            '��\���̃��j�b�g
                            If MaskData(sx + i, sy + j) Then
                                '�}�X�N���ꂽ�n�`
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            Else
                                '�n�`
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            End If
                        Else
                            If MaskData(sx + i, sy + j) Then
                                '�}�X�N���ꂽ���j�b�g
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 64, _
                                    SRCCOPY)
                                
                                '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
                                Select Case u.Area
                                    Case "��"
                                        DottedLine xx, yy + 28
                                    Case "����"
                                        DottedLine xx, yy + 3
                                    Case "�n��"
                                        DottedLine xx, yy + 28
                                        DottedLine xx, yy + 3
                                    Case "�F��"
                                        If TerrainClass(sx + i, sy + j) = "����" Then
                                            DottedLine xx, yy + 28
                                        End If
                                End Select
                            Else
                                '���j�b�g
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
                                    SRCCOPY)
                                
                                '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
                                Select Case u.Area
                                    Case "��"
                                        ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                        ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                    Case "����"
                                        ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                        ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                    Case "�n��"
                                        ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                        ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                        ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                        ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                    Case "�F��"
                                        If TerrainClass(sx + i, sy + j) = "����" Then
                                            ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                            ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                        End If
                                End Select
                            End If
                        End If
                    End If
NextLoop2:
                Next
            Next
        End If
        
        '�`��F�����ɖ߂��Ă���
        pic.ForeColor = prev_color
        
        '��ʂ�����������ꂽ���Ƃ��L�^
        ScreenIsSaved = False
        
        If Not without_refresh And Not delay_refresh Then
            .picMain(0).Refresh
        End If
    End With
End Sub
' ADD END MARGE

' MOD START MARGE
'Private Sub DottedLine(ByVal X As Integer, ByVal Y As Integer)
Private Sub DottedLine(ByVal X As Integer, ByVal Y As Integer, _
    Optional ByVal half_size As Boolean)
' MOD END MARGE
Dim i As Integer
    
    With MainForm.picMain(0)
' MOD START MARGE
'        For i = 0 To 15
'            MainForm.picMain(0).PSet (X + 2 * i + 1, Y), vbBlack
'        Next
        If half_size Then
            For i = 0 To 7
                MainForm.picMain(0).PSet (X + 2 * i + 1, Y), vbBlack
            Next
        Else
            For i = 0 To 15
                MainForm.picMain(0).PSet (X + 2 * i + 1, Y), vbBlack
            Next
        End If
' MOD END MARGE
    End With
End Sub

'�w�肳�ꂽ�}�b�v���W����ʂ̒����ɕ\��
Public Sub Center(ByVal new_x As Integer, ByVal new_y As Integer)
    If MapFileName = "" Then
        new_x = (MainWidth + 1) \ 2
        If new_y < (MapHeight + 1) \ 2 Then
            new_y = (MapHeight + 1) \ 2
        ElseIf new_y > MapHeight - (MainWidth + 1) \ 2 + 1 Then
            new_y = MapHeight - (MainWidth + 1) \ 2 + 1
        End If
        Exit Sub
    End If
    
    MapX = new_x
    If MapX < 1 Then
        MapX = 1
    ElseIf MapX > MainForm.HScroll.max Then
        MapX = MainForm.HScroll.max
    End If
    MapY = new_y
    If MapY < 1 Then
        MapY = 1
    ElseIf MapY > MainForm.VScroll.max Then
        MapY = MainForm.VScroll.max
    End If
End Sub


' === ���W�ϊ� ===

'�}�b�v��ł̍��W���}�b�v��ʂ̂ǂ̈ʒu�ɂ��邩��Ԃ�
Public Function MapToPixelX(ByVal X As Integer) As Integer
' MOD START MARGE
'    MapToPixelX = 32 * ((MainWidth + 1) \ 2 - 1 - (MapX - X))
    If NewGUIMode Then
        MapToPixelX = 32 * ((MainWidth + 1) \ 2 - 0.5 - (MapX - X))
    Else
        MapToPixelX = 32 * ((MainWidth + 1) \ 2 - 1 - (MapX - X))
    End If
' MOD END MARGE
End Function

Public Function MapToPixelY(ByVal Y As Integer) As Integer
    MapToPixelY = 32 * ((MainHeight + 1) \ 2 - 1 - (MapY - Y))
End Function

'�}�b�v��ʂł̃s�N�Z�����}�b�v�̍��W�̂ǂ̈ʒu�ɂ��邩��Ԃ�
Public Function PixelToMapX(ByVal X As Integer) As Integer
    If X < 0 Then
       X = 0
    ElseIf X >= MainPWidth Then
       X = MainPWidth - 1
    End If
    
' MOD START MARGE
'    PixelToMapX = X \ 32 + 1 + MapX - (MainWidth + 1) \ 2
    If NewGUIMode Then
        PixelToMapX = (X + 16) \ 32 + MapX - (MainWidth + 1) \ 2
    Else
        PixelToMapX = X \ 32 + 1 + MapX - (MainWidth + 1) \ 2
    End If
' MOD END MARGE
End Function

Public Function PixelToMapY(ByVal Y As Integer) As Integer
    If Y < 0 Then
       Y = 0
    ElseIf Y >= MainPHeight Then
       Y = MainPHeight - 1
    End If
    
    PixelToMapY = Y \ 32 + 1 + MapY - (MainHeight + 1) \ 2
End Function


' === ���j�b�g�摜�\���Ɋւ��鏈�� ===

'���j�b�g�摜�t�@�C��������
Private Function FindUnitBitmap(u As Unit) As String
Dim fname As String, uname As String
Dim tname As String, tnum As String, tdir As String
Dim i As Integer, j As Integer

    With u
        '�C���^�[�~�b�V�����ł̃p�C���b�g�X�e�[�^�X�\���̏ꍇ��
        '����ȏ������K�v
        If .IsFeatureAvailable("�_�~�[���j�b�g") _
            And InStr(.Name, "�X�e�[�^�X�\���p���j�b�g") = 0 _
        Then
            If .CountPilot = 0 Then
                Exit Function
            End If
            
            If .FeatureData("�_�~�[���j�b�g") = "���j�b�g�摜�g�p" Then
                '���j�b�g�摜���g���ĕ\��
                uname = "���惆�j�b�g[" & .MainPilot.ID & "]"
                uname = LocalVariableList.Item(uname).StringValue
                fname = "\Bitmap\Unit\" & UList.Item(uname).Bitmap
            Else
                '�p�C���b�g�摜���g���ĕ\��
                fname = "\Bitmap\Pilot\" & .MainPilot.Bitmap
            End If
            
            '�摜������
            If InStr(fname, "\-.bmp") > 0 Then
                fname = ""
            ElseIf FileExists(ScenarioPath & fname) Then
                fname = ScenarioPath & fname
            ElseIf FileExists(ExtDataPath & fname) Then
                fname = ExtDataPath & fname
            ElseIf FileExists(ExtDataPath2 & fname) Then
                fname = ExtDataPath2 & fname
            ElseIf FileExists(AppPath & fname) Then
                fname = AppPath & fname
            Else
                fname = ""
            End If
            
            FindUnitBitmap = fname
            Exit Function
        End If
        
        If .IsFeatureAvailable("�n�`���j�b�g") Then
            '�n�`���j�b�g
            fname = .Bitmap
            If FileExists(AppPath & "Bitmap\Map\" & fname) _
                Or FileExists(ScenarioPath & "Bitmap\Map\" & fname) _
            Then
                fname = "Bitmap\Map\" & fname
            Else
                '�n�`�摜�����p�̒n�`�摜�f�B���N�g������4���t�@�C�������쐬
                i = Len(fname) - 5
                Do While i > 0
                    If Mid$(fname, i, 1) Like "[!-0-9]" Then
                        Exit Do
                    End If
                    i = i - 1
                Loop
                If i > 0 Then
                    tdir = Left$(fname, i)
                    With TDList
                        For j = 1 To .Count
                            If tdir = .Item(.OrderedID(j)).Bitmap Then
                                tnum = Mid$(fname, i + 1, Len(fname) - i - 4)
                                tname = Left$(fname, i) & Format$(StrToLng(tnum), "0000") & ".bmp"
                                Exit For
                            End If
                        Next
                        If j <= .Count Then
                            tdir = tdir & "\"
                        Else
                            tdir = ""
                        End If
                    End With
                End If
                If tdir <> "" Then
                    If FileExists(AppPath & "Bitmap\Map\" & tname) _
                        Or FileExists(ScenarioPath & "Bitmap\Map\" & tname) _
                    Then
                        fname = "Bitmap\Map\" & tname
                    Else
                        fname = "Bitmap\Map\" & tdir & "\" & tname
                    End If
                Else
                    If InStr(fname, "\") > 0 Then
                        '�t�H���_�w�肠��
                        fname = "Bitmap\" & fname
                    Else
                        '�ʏ�̃��j�b�g�摜
                        fname = "Bitmap\Unit\" & fname
                    End If
                End If
            End If
        Else
            '�ʏ�̃��j�b�g�`��
            fname = .Bitmap
            If InStr(fname, ":") = 2 Then
                '�t���p�X�w��
            ElseIf InStr(fname, "\") > 0 Then
                '�t�H���_�w�肠��
                fname = "Bitmap\" & fname
            Else
                '�ʏ�̃��j�b�g�摜
                fname = "Bitmap\Unit\" & fname
            End If
        End If
        
        '�摜�̌���
        If InStr(fname, "\-.bmp") > 0 Then
            fname = ""
        ElseIf FileExists(ScenarioPath & fname) Then
            fname = ScenarioPath & fname
        ElseIf FileExists(ExtDataPath & fname) Then
            fname = ExtDataPath & fname
        ElseIf FileExists(ExtDataPath2 & fname) Then
            fname = ExtDataPath2 & fname
        ElseIf FileExists(AppPath & fname) Then
            fname = AppPath & fname
        ElseIf Not FileExists(fname) Then
            fname = ""
            
            '�摜��������Ȃ��������Ƃ��L�^
            If .Bitmap = .Data.Bitmap Then
                .Data.IsBitmapMissing = True
            End If
        End If
        
        FindUnitBitmap = fname
    End With
End Function

'���j�b�g�̃r�b�g�}�b�v���쐬
Public Function MakeUnitBitmap(u As Unit) As Integer
Dim fname As String, uparty As String
Dim i As Integer, ret As Long
Dim xx As Integer, yy As Integer
Static bitmap_num As Integer
Static fname_list() As String
Static party_list() As String

    With MainForm
        If u.IsFeatureAvailable("��\��") Then
            MakeUnitBitmap = -1
            Exit Function
        End If
        
        '�摜���N���A����Ă���H
        If .picUnitBitmap.width = 32 Then
            bitmap_num = 0
        End If
        
        '���܂łɃ��[�h����Ă��郆�j�b�g�摜��
        ReDim Preserve fname_list(bitmap_num)
        ReDim Preserve party_list(bitmap_num)
        
        '�ȑO���[�h�������j�b�g�摜�ƈ�v���Ă���H
        fname = FindUnitBitmap(u)
        uparty = u.Party0
        For i = 1 To bitmap_num
            If fname = fname_list(i) _
                And uparty = party_list(i) _
            Then
                '��v�������̂���������
                MakeUnitBitmap = i
                Exit Function
            End If
        Next
        
        '�V���ɉ摜��o�^
        bitmap_num = bitmap_num + 1
        ReDim Preserve fname_list(bitmap_num)
        ReDim Preserve party_list(bitmap_num)
        fname_list(bitmap_num) = fname
        party_list(bitmap_num) = uparty
        
        '�摜�o�b�t�@�̑傫����ύX
        .picUnitBitmap.Move 0, 0, 480, 96 * (bitmap_num \ 15 + 1)
        
        '�摜�̏������݈ʒu
        xx = 32 * (bitmap_num Mod 15)
        yy = 96 * (bitmap_num \ 15)
        
        '�t�@�C�������[�h����
        LoadUnitBitmap u, .picUnitBitmap, xx, yy, False, fname
        
        '�s���ς݂̍ۂ̉摜���쐬
        ret = BitBlt(.picUnitBitmap.hDC, _
            xx, yy + 32, 32, 32, _
            .picUnitBitmap.hDC, _
            xx, yy, SRCCOPY)
        ret = BitBlt(.picUnitBitmap.hDC, _
            xx, yy + 32, 32, 32, _
            .picMask.hDC, 0, 0, SRCAND)
        
        '�}�X�N����̉摜���쐬
        ret = BitBlt(.picUnitBitmap.hDC, _
            xx, yy + 64, 32, 32, _
            .picUnitBitmap.hDC, _
            xx, yy + 32, SRCCOPY)
        ret = BitBlt(.picUnitBitmap.hDC, _
            xx, yy + 64, 32, 32, _
            .picMask2.hDC, 0, 0, SRCINVERT)
    End With
    
    '���j�b�g�摜�ԍ���Ԃ�
    MakeUnitBitmap = bitmap_num
End Function

'���j�b�g�̃r�b�g�}�b�v�����[�h
Public Sub LoadUnitBitmap(u As Unit, pic As PictureBox, _
    ByVal dx As Integer, ByVal dy As Integer, _
    Optional ByVal use_orig_color As Boolean, _
    Optional fname As String)
Dim ret As Long
Dim emit_light As Boolean

    With MainForm
        '�摜�t�@�C��������
        If fname = "" Then
            fname = FindUnitBitmap(u)
        End If
        
        '�摜�����̂܂܎g�p����ꍇ
        If InStr(fname, "\Pilot\") > 0 _
            Or u.FeatureData("�_�~�[���j�b�g") = "���j�b�g�摜�g�p" _
        Then
            '�摜�̓ǂݍ���
            On Error GoTo ErrorHandler
            .picTmp = LoadPicture(fname)
            On Error GoTo 0
            
            '��ʂɕ`��
            ret = StretchBlt(pic.hDC, dx, dy, 32, 32, _
                .picTmp.hDC, 0, 0, .picTmp.width, .picTmp.Height, SRCCOPY)
            
            Exit Sub
        End If
        
        '���j�b�g�������Ŕ������Ă��邩�����炩���߃`�F�b�N
        If MapDrawMode = "��" _
            And Not MapDrawIsMapOnly _
            And Not use_orig_color _
            And u.IsFeatureAvailable("����") _
        Then
            emit_light = True
        End If
        
        If fname <> "" Then
            '�摜�����������ꍇ�͉摜��ǂݍ���
            On Error GoTo ErrorHandler
            .picTmp32(0) = LoadPicture(fname)
            On Error GoTo 0
            
            '�摜�̃T�C�Y�����������`�F�b�N
            If .picTmp32(0).width <> 32 Or .picTmp32(0).Height <> 32 Then
                With .picTmp32(0)
                    .Picture = LoadPicture("")
                    .width = 32
                    .Height = 32
                End With
                ErrorMessage u.Name & "�̃��j�b�g�摜��32x32�̑傫���ɂȂ��Ă��܂���"
                Exit Sub
            End If
            
            If u.IsFeatureAvailable("�n�`���j�b�g") Then
                '�n�`���j�b�g�̏ꍇ�͉摜�����̂܂܎g��
                ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                    .picTmp32(0).hDC, 0, 0, SRCCOPY)
            ElseIf UseTransparentBlt Then
                'TransparentBlt���g���ă��j�b�g�摜�ƃ^�C�����d�ˍ��킹��
                
                '�^�C��
                Select Case u.Party0
                    Case "����", "�m�o�b"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picUnit.hDC, 0, 0, SRCCOPY)
                    Case "�G"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picEnemy.hDC, 0, 0, SRCCOPY)
                    Case "����"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picNeautral.hDC, 0, 0, SRCCOPY)
                End Select
                
                '�摜�̏d�ˍ��킹
                '(�������Ă���ꍇ�͂Q�x�h���h�����ߕ`�悵�Ȃ�)
                If Not emit_light Then
                    ret = TransparentBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picTmp32(0).hDC, 0, 0, 32, 32, vbWhite)
                End If
            Else
                'BitBlt���g���ă��j�b�g�摜�ƃ^�C�����d�ˍ��킹��
                
                '�}�X�N���쐬
                MakeMask .picTmp32(0).hDC, .picTmp32(2).hDC, 32, 32, vbWhite
                
                '�^�C��
                Select Case u.Party0
                    Case "����", "�m�o�b"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picUnit.hDC, 0, 0, SRCCOPY)
                    Case "�G"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picEnemy.hDC, 0, 0, SRCCOPY)
                    Case "����"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picNeautral.hDC, 0, 0, SRCCOPY)
                End Select
                
                '�摜�̏d�ˍ��킹
                '(�������Ă���ꍇ�͂Q�x�h���h�����ߕ`�悵�Ȃ�)
                If Not emit_light Then
                    ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picTmp32(2).hDC, 0, 0, SRCERASE)
                    ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picTmp32(0).hDC, 0, 0, SRCINVERT)
                End If
            End If
        Else
            '�摜��������Ȃ������ꍇ�̓^�C���݂̂Ń��j�b�g�摜���쐬
            Select Case u.Party0
                Case "����", "�m�o�b"
                    ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picUnit.hDC, 0, 0, SRCCOPY)
                Case "�G"
                    ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picEnemy.hDC, 0, 0, SRCCOPY)
                Case "����"
                    ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picNeautral.hDC, 0, 0, SRCCOPY)
            End Select
        End If
        
        '�F���X�e�[�W�̏󋵂ɍ��킹�ĕύX
        If Not use_orig_color _
            And Not MapDrawIsMapOnly _
        Then
            Select Case MapDrawMode
                Case "��"
                    GetImage .picTmp32(1)
                    Dark
                    SetImage .picTmp32(1)
                    '���j�b�g��"����"�̓���\�͂����ꍇ�A
                    '���j�b�g�摜���A�Â������^�C���摜�̏�ɕ`�悷��B
                    If emit_light Then
                        If UseTransparentBlt Then
                            ret = TransparentBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                                .picTmp32(0).hDC, 0, 0, 32, 32, vbWhite)
                        Else
                            ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                                .picTmp32(2).hDC, 0, 0, SRCERASE)
                            ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                                .picTmp32(0).hDC, 0, 0, SRCINVERT)
                        End If
                    End If
                Case "�Z�s�A"
                    GetImage .picTmp32(1)
                    Sepia
                    SetImage .picTmp32(1)
                Case "����"
                    GetImage .picTmp32(1)
                    Monotone
                    SetImage .picTmp32(1)
                Case "�[�Ă�"
                    GetImage .picTmp32(1)
                    Sunset
                    SetImage .picTmp32(1)
                Case "����"
                    GetImage .picTmp32(1)
                    Water
                    SetImage .picTmp32(1)
                Case "�t�B���^"
                    GetImage .picTmp32(1)
                    ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent
                    SetImage .picTmp32(1)
            End Select
        End If
        
        '��ʂɕ`��
        ret = BitBlt(pic.hDC, _
            dx, dy, 32, 32, _
            .picTmp32(1).hDC, 0, 0, SRCCOPY)
    End With
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "���j�b�g�p�r�b�g�}�b�v�t�@�C��" & vbCr & vbLf _
        & fname & vbCr & vbLf _
        & "�̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCr & vbLf _
        & "�摜�t�@�C�������Ă��Ȃ����m�F���ĉ������B"
End Sub

'���j�b�g�摜�̕`��
Public Sub PaintUnitBitmap(u As Unit, Optional ByVal smode As String)
Dim xx As Integer, yy As Integer
Dim pic As PictureBox
Dim ret As Long
Dim PT As POINTAPI
    
    With u
        '��\���H
        If .BitmapID = -1 Then
            Exit Sub
        End If
        
        '��ʊO�H
        If .X < MapX - (MainWidth + 1) \ 2 Or MapX + (MainWidth + 1) \ 2 < .X _
            Or .Y < MapY - (MainHeight + 1) \ 2 Or MapY + (MainHeight + 1) \ 2 < .Y _
        Then
            Exit Sub
        End If
        
        '�`�����ݐ�̍��W��ݒ�
        xx = MapToPixelX(.X)
        yy = MapToPixelY(.Y)
    End With
    
    With MainForm
        If smode = "���t���b�V������" And ScreenIsSaved Then
            Set pic = .picMain(1)
            '�\���摜����������ۂɎg���`��̈��ݒ�
            PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(xx, 0))
            PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(yy, 0))
            PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(xx + 32, MainPWidth - 1))
            PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(yy + 32, MainPHeight - 1))
        Else
            Set pic = .picMain(0)
        End If
        
        '���j�b�g�摜�̏�������
        If u.Action > 0 Or u.IsFeatureAvailable("�n�`���j�b�g") Then
            '�ʏ�̕\��
            ret = BitBlt(pic.hDC, _
                xx, yy, 32, 32, _
                .picUnitBitmap.hDC, _
                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
                SRCCOPY)
        Else
            '�s���ς̏ꍇ�͖Ԋ|��
            ret = BitBlt(pic.hDC, _
                xx, yy, 32, 32, _
                .picUnitBitmap.hDC, _
                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, _
                SRCCOPY)
        End If
        
        '������`�悷��ۂ̕`��F��ݒ�
        pic.ForeColor = vbBlack
        
        '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
        Select Case u.Area
            Case "��"
                ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                ret = LineTo(pic.hDC, xx + 31, yy + 28)
            Case "����"
                ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                ret = LineTo(pic.hDC, xx + 31, yy + 3)
            Case "�n��"
                ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                ret = LineTo(pic.hDC, xx + 31, yy + 28)
                ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                ret = LineTo(pic.hDC, xx + 31, yy + 3)
            Case "�F��"
                If TerrainClass(u.X, u.Y) = "����" Then
                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                End If
        End Select
        
        '�`��F�𔒂ɖ߂��Ă���
        pic.ForeColor = vbWhite
        
        If smode <> "���t���b�V������" Then
            '��ʂ�����������ꂽ���Ƃ��L�^
            ScreenIsSaved = False
            
            If .Visible Then
                pic.Refresh
            End If
        End If
    End With
End Sub

'���j�b�g�摜�̕\��������
Public Sub EraseUnitBitmap(ByVal X As Integer, ByVal Y As Integer, _
    Optional ByVal do_refresh As Boolean = True)
Dim xx As Integer, yy As Integer
Dim ret As Long
    
    '��ʊO�H
    If X < MapX - (MainWidth + 1) \ 2 Or MapX + (MainWidth + 1) \ 2 < X _
        Or Y < MapY - (MainHeight + 1) \ 2 Or MapY + (MainHeight + 1) \ 2 < Y _
    Then
        Exit Sub
    End If
    
    '��ʂ������̂ŏ��������Ȃ��H
    If IsPictureVisible Then
        Exit Sub
    End If
    
    xx = MapToPixelX(X)
    yy = MapToPixelY(Y)
    
    With MainForm
        SaveScreen
        
        '��ʕ\���ύX
        ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
            .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
        ret = BitBlt(.picMain(1).hDC, xx, yy, 32, 32, _
            .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
        
        If do_refresh Then
            '��ʂ�����������ꂽ���Ƃ��L�^
            ScreenIsSaved = False
            
            If .Visible Then
                .picMain(0).Refresh
            End If
        End If
    End With
End Sub

'���j�b�g�摜�̕\���ʒu���ړ� (�A�j���[�V����)
Public Sub MoveUnitBitmap(u As Unit, _
    ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, _
    ByVal wait_time0 As Long, Optional ByVal division As Integer = 2)
Dim xx As Integer, yy As Integer
Dim vx As Integer, vy As Integer
Dim ret As Long, i As Integer
Dim pic As PictureBox
Dim start_time As Long, cur_time As Long, wait_time As Long
Dim PT As POINTAPI
    
    wait_time = wait_time0 \ division
    
    SaveScreen
    
    With MainForm
        Set pic = .picTmp32(0)
        
        '���j�b�g�摜���쐬
        ret = BitBlt(pic.hDC, _
            0, 0, 32, 32, _
            .picUnitBitmap.hDC, _
            32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
            SRCCOPY)
        
        '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
        Select Case u.Area
            Case "��"
                ret = MoveToEx(pic.hDC, 0, 28, PT)
                ret = LineTo(pic.hDC, 31, 28)
            Case "����"
                ret = MoveToEx(pic.hDC, 0, 3, PT)
                ret = LineTo(pic.hDC, 31, 3)
            Case "�n��"
                ret = MoveToEx(pic.hDC, 0, 28, PT)
                ret = LineTo(pic.hDC, 31, 28)
                ret = MoveToEx(pic.hDC, 0, 3, PT)
                ret = LineTo(pic.hDC, 31, 3)
            Case "�F��"
                If TerrainClass(u.X, u.Y) = "����" Then
                    ret = MoveToEx(pic.hDC, 0, 28, PT)
                    ret = LineTo(pic.hDC, 31, 28)
                End If
        End Select
        
        '�ړ��̎n�_��ݒ�
        xx = MapToPixelX(x1)
        yy = MapToPixelY(y1)
        
        '�w�i��̉摜���܂�����
        '(���Ɉړ����Ă���ꍇ������)
        If u Is MapDataForUnit(x1, y1) Then
            ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
                .picBack.hDC, 32 * (x1 - 1), 32 * (y1 - 1), SRCCOPY)
            ret = BitBlt(.picMain(1).hDC, xx, yy, 32, 32, _
                .picBack.hDC, 32 * (x1 - 1), 32 * (y1 - 1), SRCCOPY)
        End If
        
        '�ŏ��̈ړ�������ݒ�
        If Abs(x2 - x1) > Abs(y2 - y1) Then
            If x2 > x1 Then
                vx = 1
            Else
                vx = -1
            End If
            vy = 0
        Else
            If y2 > y1 Then
                vy = 1
            Else
                vy = -1
            End If
            vx = 0
        End If
        
        If wait_time > 0 Then
            start_time = timeGetTime()
        End If
        
        '�ړ��̕`��
        For i = 1 To division * MaxLng(Abs(x2 - x1), Abs(y2 - y1))
            '�摜������
            ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
                .picMain(1).hDC, xx, yy, SRCCOPY)
            
            '���W���ړ�
            xx = xx + 32 * vx \ division
            yy = yy + 32 * vy \ division
            
            '�摜��`��
            ret = BitBlt(.picMain(0).hDC, _
                xx, yy, 32, 32, _
                pic.hDC, 0, 0, SRCCOPY)
            
            .picMain(0).Refresh
            
            If wait_time > 0 Then
                Do
                    DoEvents
                    cur_time = timeGetTime()
                Loop While start_time + wait_time > cur_time
                start_time = cur_time
            End If
        Next
        
        '�Q��ڂ̈ړ�������ݒ�
        If Abs(x2 - x1) > Abs(y2 - y1) Then
            If y2 > y1 Then
                vy = 1
            Else
                vy = -1
            End If
            vx = 0
        Else
            If x2 > x1 Then
                vx = 1
            Else
                vx = -1
            End If
            vy = 0
        End If
        
        '�ړ��̕`��
        For i = 1 To division * MinLng(Abs(x2 - x1), Abs(y2 - y1))
            '�摜������
            ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
                .picMain(1).hDC, xx, yy, SRCCOPY)
            
            '���W���ړ�
            xx = xx + 32 * vx \ division
            yy = yy + 32 * vy \ division
            
            '�摜��`��
            ret = BitBlt(.picMain(0).hDC, _
                xx, yy, 32, 32, _
                pic.hDC, 0, 0, SRCCOPY)
            
            .picMain(0).Refresh
            
            If wait_time > 0 Then
                Do
                    DoEvents
                    cur_time = timeGetTime()
                Loop While start_time + wait_time > cur_time
                start_time = cur_time
            End If
        Next
    End With
    
    '��ʂ�����������ꂽ���Ƃ��L�^
    ScreenIsSaved = False
End Sub

'���j�b�g�摜�̕\���ʒu���ړ� (�A�j���[�V����)
'�摜�̌o�H�����ۂ̈ړ��o�H�ɂ��킹��
Public Sub MoveUnitBitmap2(u As Unit, _
    ByVal wait_time0 As Long, Optional ByVal division As Integer = 2)
Dim xx As Integer, yy As Integer
Dim vx As Integer, vy As Integer
Dim ret As Long, i As Integer, j As Integer
Dim pic As PictureBox
Dim start_time As Long, cur_time As Long, wait_time As Long
Dim PT As POINTAPI
Dim move_route_x() As Integer, move_route_y() As Integer
    
    wait_time = wait_time0 \ division
    
    SaveScreen
    
    With MainForm
        Set pic = .picTmp32(0)
        
        '���j�b�g�摜���쐬
        ret = BitBlt(pic.hDC, 0, 0, 32, 32, .picUnitBitmap.hDC, _
            32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
            SRCCOPY)
        
        '���j�b�g�̂���ꏊ�ɍ��킹�ĕ\����ύX
        Select Case u.Area
            Case "��"
                ret = MoveToEx(pic.hDC, 0, 28, PT)
                ret = LineTo(pic.hDC, 31, 28)
            Case "����"
                ret = MoveToEx(pic.hDC, 0, 3, PT)
                ret = LineTo(pic.hDC, 31, 3)
            Case "�n��"
                ret = MoveToEx(pic.hDC, 0, 28, PT)
                ret = LineTo(pic.hDC, 31, 28)
                ret = MoveToEx(pic.hDC, 0, 3, PT)
                ret = LineTo(pic.hDC, 31, 3)
            Case "�F��"
                If TerrainClass(u.X, u.Y) = "����" Then
                    ret = MoveToEx(pic.hDC, 0, 28, PT)
                    ret = LineTo(pic.hDC, 31, 28)
                End If
        End Select
        
        '�ړ��o�H������
        SearchMoveRoute u.X, u.Y, move_route_x, move_route_y
        
        If wait_time > 0 Then
            start_time = timeGetTime()
        End If
        
        '�ړ��̎n�_
        xx = MapToPixelX(move_route_x(UBound(move_route_x)))
        yy = MapToPixelY(move_route_y(UBound(move_route_y)))
        
        i = UBound(move_route_x) - 1
        Do While i > 0
            vx = MapToPixelX(move_route_x(i)) - xx
            vy = MapToPixelY(move_route_y(i)) - yy
            
            '�ړ��̕`��
            For j = 1 To division
                '�摜������
                ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
                    .picMain(1).hDC, xx, yy, SRCCOPY)
                
                '���W���ړ�
                xx = xx + vx \ division
                yy = yy + vy \ division
                
                '�摜��`��
                ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
                    pic.hDC, 0, 0, SRCCOPY)
                
                .picMain(0).Refresh
                
                If wait_time > 0 Then
                    Do
                        DoEvents
                        cur_time = timeGetTime()
                    Loop While start_time + wait_time > cur_time
                    start_time = cur_time
                End If
            Next
            
            i = i - 1
        Loop
    End With
    
    '��ʂ�����������ꂽ���Ƃ��L�^
    ScreenIsSaved = False
End Sub


' === �e�탊�X�g�{�b�N�X�Ɋւ��鏈�� ===

'���X�g�{�b�N�X��\��
Public Function ListBox(lb_caption As String, list() As String, lb_info As String, _
    Optional lb_mode As String) As Integer
Dim i As Integer
Dim is_rbutton_released As Boolean

    Load frmListBox
    With frmListBox
        .WindowState = vbNormal
        
        '�R�����g�E�B���h�E�̏���
        If InStr(lb_mode, "�R�����g") > 0 Then
            If Not .txtComment.Enabled Then
                .txtComment.Enabled = True
                .txtComment.Visible = True
                .txtComment.width = .labCaption.width
                .txtComment.Text = ""
                .txtComment.Top = .lstItems.Top + .lstItems.Height + 5
                .Height = .Height + 600
            End If
        Else
            If .txtComment.Enabled Then
                .txtComment.Enabled = False
                .txtComment.Visible = False
                .Height = .Height - 600
            End If
        End If
        
        '�L���v�V����
        .Caption = lb_caption
        If UBound(ListItemFlag) > 0 Then
            .labCaption = "  " & lb_info
        Else
            .labCaption = lb_info
        End If
        
        '���X�g�{�b�N�X�ɃA�C�e����ǉ�
        .lstItems.Visible = False
        .lstItems.Clear
        If UBound(ListItemFlag) > 0 Then
            For i = 1 To UBound(list)
                If ListItemFlag(i) Then
                    .lstItems.AddItem "�~" & list(i)
                Else
                    .lstItems.AddItem "  " & list(i)
                End If
            Next
            i = UBound(list)
            Do While i > 0
                If Not ListItemFlag(i) Then
                    .lstItems.ListIndex = i - 1
                    Exit Do
                End If
                i = i - 1
            Loop
        Else
            For i = 1 To UBound(list)
                .lstItems.AddItem list(i)
            Next
        End If
        .lstItems.ListIndex = -1
        .lstItems.Visible = True
        
        '�R�����g�t���̃A�C�e���H
        If UBound(ListItemComment) <> UBound(list) Then
            ReDim Preserve ListItemComment(UBound(list))
        End If
        
        '�ŏ�������Ă���ꍇ�͖߂��Ă���
        If .WindowState <> vbNormal Then
            .WindowState = vbNormal
            .Show
        End If
        
        '�\���ʒu��ݒ�
        If MainForm.Visible _
            And .HorizontalSize = "S" _
        Then
            .Left = MainForm.Left
        Else
            .Left = (Screen.width - .width) / 2
        End If
        If MainForm.Visible _
            And Not MainForm.WindowState = 1 _
            And .VerticalSize = "M" _
            And InStr(lb_mode, "�����\��") = 0 _
        Then
            .Top = MainForm.Top + MainForm.Height - .Height
        Else
            .Top = (Screen.Height - .Height) / 2
        End If
        
        '�擪�̃A�C�e����ݒ�
        If TopItem > 0 Then
            If .lstItems.TopIndex <> TopItem - 1 Then
                .lstItems.TopIndex = MaxLng(MinLng(TopItem - 1, .lstItems.ListCount - 1), 0)
            End If
            If .lstItems.Columns Then
                .lstItems.ListIndex = TopItem - 1
            End If
        Else
            If .lstItems.Columns Then
                .lstItems.ListIndex = 0
            End If
        End If
        
        '�R�����g�E�B���h�E�̕\��
        If .txtComment.Enabled Then
            .txtComment.Text = ListItemComment(.lstItems.ListIndex + 1)
        End If
        
        SelectedItem = 0
        
        IsFormClicked = False
        DoEvents
        
        '���X�g�{�b�N�X��\��
        If InStr(lb_mode, "�\���̂�") > 0 Then
            '�\���݂̂��s��
            IsMordal = False
            .Show
            .lstItems.SetFocus
            Call SetWindowPos(.hwnd, -1, 0, 0, 0, 0, &H3)
            .Refresh
            Exit Function
        ElseIf InStr(lb_mode, "�A���\��") > 0 Then
            '�I�����s���Ă����X�g�{�b�N�X����Ȃ�
            IsMordal = False
            If Not .Visible Then
                .Show
                Call SetWindowPos(.hwnd, -1, 0, 0, 0, 0, &H3)
                .lstItems.SetFocus
            End If
            
            If InStr(lb_mode, "�J�[�\���ړ�") > 0 Then
                If AutoMoveCursor Then
                    MoveCursorPos "�_�C�A���O"
                End If
            End If
            
            Do Until IsFormClicked
                DoEvents
                '�E�{�^���ł̃_�u���N���b�N�̎���
                If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
                    is_rbutton_released = True
                Else
                    If is_rbutton_released Then
                        IsFormClicked = True
                    End If
                End If
                Sleep 50
            Loop
        Else
            '�I�����s��ꂽ���_�Ń��X�g�{�b�N�X�����
            IsMordal = False
            .Show
            Call SetWindowPos(.hwnd, -1, 0, 0, 0, 0, &H3)
            .lstItems.SetFocus
            
            If InStr(lb_mode, "�J�[�\���ړ�") > 0 Then
                If AutoMoveCursor Then
                    MoveCursorPos "�_�C�A���O"
                End If
            End If
            
            Do Until IsFormClicked
                DoEvents
                '�E�{�^���ł̃_�u���N���b�N�̎���
                If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
                    is_rbutton_released = True
                Else
                    If is_rbutton_released Then
                        IsFormClicked = True
                    End If
                End If
                Sleep 50
            Loop
            .Hide
            
            If InStr(lb_mode, "�J�[�\���ړ�") > 0 _
                And InStr(lb_mode, "�J�[�\���ړ�(�s���̂�)") = 0 _
            Then
                If AutoMoveCursor Then
                    RestoreCursorPos
                End If
            End If
            
            If .txtComment.Enabled Then
                .txtComment.Enabled = False
                .txtComment.Visible = False
                .Height = .Height - 600
            End If
        End If
        
        ListBox = SelectedItem
        DoEvents
    End With
End Function

'���X�g�{�b�N�X�̍�����傫������
Public Sub EnlargeListBoxHeight()
    With frmListBox
        Select Case .VerticalSize
            Case "M"
                If .WindowState <> vbNormal Then
                    .Visible = True
                    .WindowState = vbNormal
                End If
                .Visible = False
                .Height = .Height + 2400
                .lstItems.Height = 260
                .VerticalSize = "L"
        End Select
    End With
End Sub

'���X�g�{�b�N�X�̍���������������
Public Sub ReduceListBoxHeight()
    With frmListBox
        Select Case .VerticalSize
            Case "L"
                If .WindowState <> vbNormal Then
                    .Visible = True
                    .WindowState = vbNormal
                End If
                .Visible = False
                .Height = .Height - 2400
                .lstItems.Height = 100
                .VerticalSize = "M"
        End Select
    End With
End Sub

'���X�g�{�b�N�X�̕���傫������
Public Sub EnlargeListBoxWidth()
    With frmListBox
        Select Case .HorizontalSize
            Case "S"
                If .WindowState <> vbNormal Then
                    .Visible = True
                    .WindowState = vbNormal
                End If
                .Visible = False
                .width = .width + 2350
                .lstItems.width = 637
                .labCaption.width = 637
                .HorizontalSize = "M"
         End Select
    End With
End Sub

'���X�g�{�b�N�X�̕�������������
Public Sub ReduceListBoxWidth()
    With frmListBox
        Select Case .HorizontalSize
            Case "M"
                If .WindowState <> vbNormal Then
                    .Visible = True
                    .WindowState = vbNormal
                End If
                .Visible = False
                .width = .width - 2350
                .lstItems.width = 486
                .labCaption.width = 486
                .HorizontalSize = "S"
        End Select
    End With
End Sub

'����I��p�Ƀ��X�g�{�b�N�X��؂�ւ�
Public Sub AddPartsToListBox()
Dim ret As Long
Dim fname As String
Dim u As Unit, t As Unit
    
    Set u = SelectedUnit
    Set t = SelectedTarget
    
    With frmListBox
    '���X�g�{�b�N�X�Ƀ��j�b�g��g�o�̃Q�[�W��ǉ�
        .Height = .Height + 535
        .labCaption.Top = 42
        .lstItems.Top = 69
        .imgPilot1.Visible = True
        .labLevel1.Visible = True
        .txtLevel1.Visible = True
        .labMorale1.Visible = True
        .txtMorale1.Visible = True
        .picUnit1.Visible = True
        .labHP1.Visible = True
        .txtHP1.Visible = True
        .picHP1.Visible = True
        .labEN1.Visible = True
        .txtEN1.Visible = True
        .picEN1.Visible = True
        .imgPilot2.Visible = True
        .labLevel2.Visible = True
        .txtLevel2.Visible = True
        .labMorale2.Visible = True
        .txtMorale2.Visible = True
        .picUnit2.Visible = True
        .labHP2.Visible = True
        .txtHP2.Visible = True
        .picHP2.Visible = True
        .labEN2.Visible = True
        .txtEN2.Visible = True
        .picEN2.Visible = True
        
        '���j�b�g���̕\��
        fname = "Bitmap\Pilot\" & u.MainPilot.Bitmap
        If FileExists(ScenarioPath & fname) Then
            .imgPilot1 = LoadPicture(ScenarioPath & fname)
        ElseIf FileExists(ExtDataPath & fname) Then
            .imgPilot1 = LoadPicture(ExtDataPath & fname)
        ElseIf FileExists(ExtDataPath2 & fname) Then
            .imgPilot1 = LoadPicture(ExtDataPath2 & fname)
        ElseIf FileExists(AppPath & fname) Then
            .imgPilot1 = LoadPicture(AppPath & fname)
        Else
            .imgPilot1 = LoadPicture("")
        End If
        
        .txtLevel1.Text = Format$(u.MainPilot.Level)
        .txtMorale1.Text = Format$(u.MainPilot.Morale)
        
        If MapDrawMode = "" Then
            If u.BitmapID > 0 Then
                ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, _
                    MainForm.picUnitBitmap.hDC, _
                    32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
                    SRCCOPY)
            Else
                '��\���̃��j�b�g�̏ꍇ�̓��j�b�g�̂���n�`�^�C����\��
                ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, _
                    MainForm.picBack.hDC, _
                    32 * (u.X - 1), 32 * (u.Y - 1), _
                    SRCCOPY)
            End If
        Else
            LoadUnitBitmap u, .picUnit1, 0, 0, True
        End If
        .picUnit1.Refresh
            
        If u.IsConditionSatisfied("�f�[�^�s��") Then
            .labHP1.Caption = Term("HP")
            .txtHP1.Text = "?????/?????"
        Else
            .labHP1.Caption = Term("HP")
            If u.HP < 100000 Then
                .txtHP1.Text = Format$(u.HP)
            Else
                .txtHP1.Text = "?????"
            End If
            If u.MaxHP < 100000 Then
                .txtHP1.Text = .txtHP1.Text & "/" & Format$(u.MaxHP)
            Else
                .txtHP1.Text = .txtHP1.Text & "/?????"
            End If
        End If
        .picHP1.Cls
    End With
    frmListBox.picHP1.Line (0, 0)-((frmListBox.picHP1.width - 4) * u.HP \ u.MaxHP - 1, 4), , BF
    
    With frmListBox
        If u.IsConditionSatisfied("�f�[�^�s��") Then
            .labEN1.Caption = Term("EN")
            .txtEN1.Text = "???/???"
        Else
            .labEN1.Caption = Term("EN", t)
            If u.EN < 1000 Then
                .txtEN1.Text = Format$(u.EN)
            Else
                .txtEN1.Text = "???"
            End If
            If u.MaxEN < 1000 Then
                .txtEN1.Text = .txtEN1.Text & "/" & Format$(u.MaxEN)
            Else
                .txtEN1.Text = .txtEN1.Text & "/???"
            End If
        End If
        .picEN1.Cls
    End With
    frmListBox.picEN1.Line (0, 0)-((frmListBox.picEN1.width - 4) * u.EN \ u.MaxEN - 1, 4), , BF
    
    With frmListBox
        '�^�[�Q�b�g���̕\��
        fname = "Bitmap\Pilot\" & t.MainPilot.Bitmap
        If FileExists(ScenarioPath & fname) Then
            .imgPilot2 = LoadPicture(ScenarioPath & fname)
        ElseIf FileExists(ExtDataPath & fname) Then
            .imgPilot2 = LoadPicture(ExtDataPath & fname)
        ElseIf FileExists(ExtDataPath2 & fname) Then
            .imgPilot2 = LoadPicture(ExtDataPath2 & fname)
        ElseIf FileExists(AppPath & fname) Then
            .imgPilot2 = LoadPicture(AppPath & fname)
        Else
            .imgPilot2 = LoadPicture("")
        End If
        
        .txtLevel2.Text = Format$(t.MainPilot.Level)
        .txtMorale2.Text = Format$(t.MainPilot.Morale)
        
        If MapDrawMode = "" Then
            If t.BitmapID > 0 Then
                ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, _
                    MainForm.picUnitBitmap.hDC, _
                    32 * (t.BitmapID Mod 15), 96 * (t.BitmapID \ 15), _
                    SRCCOPY)
            Else
                '��\���̃��j�b�g�̏ꍇ�̓��j�b�g�̂���n�`�^�C����\��
                ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, _
                    MainForm.picBack.hDC, _
                    32 * (t.X - 1), 32 * (t.Y - 1), _
                    SRCCOPY)
            End If
        Else
            LoadUnitBitmap t, .picUnit2, 0, 0, True
        End If
        .picUnit2.Refresh
        
        If t.IsConditionSatisfied("�f�[�^�s��") Then
            .labHP2.Caption = Term("HP")
            .txtHP2.Text = "?????/?????"
        Else
            .labHP2.Caption = Term("HP", t)
            If t.HP < 100000 Then
                .txtHP2.Text = Format$(t.HP)
            Else
                .txtHP2.Text = "?????"
            End If
            If t.MaxHP < 100000 Then
                .txtHP2.Text = .txtHP2.Text & "/" & Format$(t.MaxHP)
            Else
                .txtHP2.Text = .txtHP2.Text & "/?????"
            End If
        End If
        .picHP2.Cls
    End With
    frmListBox.picHP2.Line (0, 0)-((frmListBox.picHP2.width - 4) * t.HP \ t.MaxHP - 1, 4), , BF
    
    With frmListBox
        If t.IsConditionSatisfied("�f�[�^�s��") Then
            .labEN2.Caption = Term("EN")
            .txtEN2.Text = "???/???"
        Else
            .labEN2.Caption = Term("EN", t)
            If t.EN < 1000 Then
                .txtEN2.Text = Format$(t.EN)
            Else
                .txtEN2.Text = "???"
            End If
            If t.MaxEN < 1000 Then
                .txtEN2.Text = .txtEN2.Text & "/" & Format$(t.MaxEN)
            Else
                .txtEN2.Text = .txtEN2.Text & "/???"
            End If
        End If
        .picEN2.Cls
    End With
    frmListBox.picEN2.Line (0, 0)-((frmListBox.picEN2.width - 4) * t.EN \ t.MaxEN - 1, 4), , BF
End Sub

'����I��p���X�g�{�b�N�X��ʏ�̂��̂ɐ؂�ւ�
Public Sub RemovePartsOnListBox()
    With frmListBox
        .Height = .Height - 535
        .labCaption.Top = 4
        .lstItems.Top = 32
        
        .imgPilot1.Visible = False
        .labLevel1.Visible = False
        .txtLevel1.Visible = False
        .labMorale1.Visible = False
        .txtMorale1.Visible = False
        .picUnit1.Visible = False
        .labHP1.Visible = False
        .txtHP1.Visible = False
        .picHP1.Visible = False
        .labEN1.Visible = False
        .txtEN1.Visible = False
        .picEN1.Visible = False
        .imgPilot2.Visible = False
        .labLevel2.Visible = False
        .txtLevel2.Visible = False
        .labMorale2.Visible = False
        .txtMorale2.Visible = False
        .picUnit2.Visible = False
        .labHP2.Visible = False
        .txtHP2.Visible = False
        .picHP2.Visible = False
        .labEN2.Visible = False
        .txtEN2.Visible = False
        .picEN2.Visible = False
    End With
End Sub

'����I��p���X�g�{�b�N�X
Public Function WeaponListBox(u As Unit, caption_msg As String, _
    lb_mode As String, Optional BGM As String) As Integer
Dim i As Integer, j As Integer, k As Integer, ret As Integer, w As Integer
Dim list() As String, wlist() As Integer
Dim warray() As Integer, wpower() As Long, wclass As String
Dim is_rbutton_released As Boolean
Dim buf As String

    With u
        ReDim warray(.CountWeapon)
        ReDim wpower(.CountWeapon)
        ReDim ListItemFlag(.CountWeapon)
        ReDim ToolTips(.CountWeapon)
        For i = 1 To .CountWeapon
            wpower(i) = .WeaponPower(i, "")
        Next
        
        '�U���͂Ń\�[�g
        For i = 1 To .CountWeapon
            For j = 1 To i - 1
                If wpower(i) > wpower(warray(i - j)) Then
                    Exit For
                ElseIf wpower(i) = wpower(warray(i - j)) Then
                    If .Weapon(i).ENConsumption > 0 Then
                        If .Weapon(i).ENConsumption >= .Weapon(warray(i - j)).ENConsumption Then
                            Exit For
                        End If
                    ElseIf .Weapon(i).Bullet > 0 Then
                        If .Weapon(i).Bullet <= .Weapon(warray(i - j)).Bullet Then
                            Exit For
                        End If
                    Else
                        If .Weapon(i - j).ENConsumption = 0 _
                            And .Weapon(warray(i - j)).Bullet = 0 _
                        Then
                            Exit For
                        End If
                    End If
                End If
            Next
            For k = 1 To j - 1
                warray(i - k + 1) = warray(i - k)
            Next
            warray(i - j + 1) = i
        Next
    End With
    
    ReDim list(0)
    ReDim wlist(0)
    If lb_mode = "�ړ��O" Or lb_mode = "�ړ���" Or lb_mode = "�ꗗ" Then
        '�ʏ�̕���I�����̕\��
        For i = 1 To u.CountWeapon
            w = warray(i)
            
            With u
                If lb_mode = "�ꗗ" Then
                    If Not .IsWeaponAvailable(w, "�X�e�[�^�X") Then
                        'Disable�R�}���h�Ŏg�p�s�ɂ��ꂽ����Ǝg�p�ł��Ȃ����̋Z
                        '�͕\�����Ȃ�
                        If .IsDisabled(.Weapon(w).Name) Then
                            GoTo NextLoop1
                        End If
                        If Not .IsWeaponMastered(w) Then
                            GoTo NextLoop1
                        End If
                        If .IsWeaponClassifiedAs(w, "��") Then
                            If Not .IsCombinationAttackAvailable(w, True) Then
                                GoTo NextLoop1
                            End If
                        End If
                    End If
                    ListItemFlag(UBound(list) + 1) = False
                Else
                    If .IsWeaponUseful(w, lb_mode) Then
                        ListItemFlag(UBound(list) + 1) = False
                    Else
                        'Disable�R�}���h�Ŏg�p�s�ɂ��ꂽ����Ǝg�p�ł��Ȃ����̋Z
                        '�͕\�����Ȃ�
                        If .IsDisabled(.Weapon(w).Name) Then
                            GoTo NextLoop1
                        End If
                        If Not .IsWeaponMastered(w) Then
                            GoTo NextLoop1
                        End If
                        If .IsWeaponClassifiedAs(w, "��") Then
                            If Not .IsCombinationAttackAvailable(w, True) Then
                                GoTo NextLoop1
                            End If
                        End If
                        ListItemFlag(UBound(list) + 1) = True
                    End If
                End If
            End With
            
            ReDim Preserve list(UBound(list) + 1)
            ReDim Preserve wlist(UBound(list))
            wlist(UBound(list)) = w
            
            '�e����̕\�����e�̐ݒ�
            With u.Weapon(w)
                '�U����
                If wpower(w) < 10000 Then
                    list(UBound(list)) = RightPaddedString(.Nickname, 27) _
                        & LeftPaddedString(Format$(wpower(w)), 4)
                Else
                    list(UBound(list)) = RightPaddedString(.Nickname, 26) _
                        & LeftPaddedString(Format$(wpower(w)), 5)
                End If
                
                '�ő�˒�
                If u.WeaponMaxRange(w) > 1 Then
                    buf = Format$(.MinRange) & "-" & Format$(u.WeaponMaxRange(w))
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(buf, 5)
                Else
                    list(UBound(list)) = list(UBound(list)) & "    1"
                End If
                
                '�������C��
                If u.WeaponPrecision(w) >= 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString("+" & Format$(u.WeaponPrecision(w)), 4)
                Else
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.WeaponPrecision(w)), 4)
                End If
                
                '�c��e��
                If .Bullet > 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.Bullet(w)), 3)
                Else
                    list(UBound(list)) = list(UBound(list)) & "  -"
                End If
                
                '�d�m�����
                If .ENConsumption > 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.WeaponENConsumption(w)), 4)
                Else
                    list(UBound(list)) = list(UBound(list)) & "   -"
                End If
                
                '�N���e�B�J�����C��
                If u.WeaponCritical(w) >= 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString("+" & Format$(u.WeaponCritical(w)), 4)
                Else
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.WeaponCritical(w)), 4)
                End If
                
                '�n�`�K��
                list(UBound(list)) = list(UBound(list)) & " " & .Adaption
                
                '�K�v�C��
                If .NecessaryMorale > 0 Then
                    list(UBound(list)) = list(UBound(list)) _
                        & " �C" & .NecessaryMorale
                End If
                
                '����
                wclass = u.WeaponClass(w)
                If InStrNotNest(wclass, "|") > 0 Then
                    wclass = Left$(wclass, InStrNotNest(wclass, "|") - 1)
                End If
                list(UBound(list)) = list(UBound(list)) & " " & wclass
            End With
NextLoop1:
        Next
        
        If lb_mode = "�ړ��O" Or lb_mode = "�ړ���" Then
            If Not u.LookForSupportAttack(Nothing) Is Nothing Then
                '����U�����g�����ǂ����I��
                UseSupportAttack = True
                ReDim Preserve list(UBound(list) + 1)
                ReDim Preserve ListItemFlag(UBound(list))
                list(UBound(list)) = "����U���F�g�p����"
            End If
        End If
        
        '���X�g�{�b�N�X��\��
        TopItem = -1
        ret = ListBox(caption_msg, list, _
            "����                       �U�� �˒�  �� �e  " & Term("EN", u, 2) & "  " _
                & Term("CT", u, 2) & " �K�� ����", _
            "�\���̂�")
        
        If AutoMoveCursor Then
            If lb_mode <> "�ꗗ" Then
                MoveCursorPos "����I��"
            Else
                MoveCursorPos "�_�C�A���O"
            End If
        End If
        If BGM <> "" Then
            ChangeBGM BGM
        End If
        
        Do While True
            Do Until IsFormClicked
                DoEvents
                '�E�{�^���ł̃_�u���N���b�N�̎���
                If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
                    is_rbutton_released = True
                Else
                    If is_rbutton_released Then
                        IsFormClicked = True
                    End If
                End If
            Loop
            
            If SelectedItem <= UBound(wlist) Then
                Exit Do
            Else
                '����U���̃I��/�I�t�؂�ւ�
                UseSupportAttack = Not UseSupportAttack
                If UseSupportAttack Then
                    list(UBound(list)) = "����U���F�g�p����"
                Else
                    list(UBound(list)) = "����U���F�g�p���Ȃ�"
                End If
                
                SelectedItem = ListBox(caption_msg, list, _
                    "����                       �U�� �˒�  �� �e  " & Term("EN", u, 2) _
                        & "  " & Term("CT", u, 2) & " �K�� ����", _
                    "�\���̂�")
            End If
        Loop
        
        If lb_mode <> "�ꗗ" Then
            frmListBox.Hide
        End If
        ReDim ListItemComment(0)
        WeaponListBox = wlist(SelectedItem)
        
    ElseIf lb_mode = "����" Then
        '��������I�����̕\��
        
        For i = 1 To u.CountWeapon
            w = warray(i)
            
            With u
                'Disable�R�}���h�Ŏg�p�s�ɂ��ꂽ����͕\�����Ȃ�
                If .IsDisabled(.Weapon(w).Name) Then
                    GoTo NextLoop2
                End If
                
                '�K�v�Z�\�𖞂����Ȃ�����͕\�����Ȃ�
                If Not .IsWeaponMastered(w) Then
                    GoTo NextLoop2
                End If
                
                '�g�p�ł��Ȃ����̋Z�͕\�����Ȃ�
                If .IsWeaponClassifiedAs(w, "��") Then
                    If Not .IsCombinationAttackAvailable(w, True) Then
                        GoTo NextLoop2
                    End If
                End If
                    
                If Not .IsWeaponAvailable(w, "�ړ��O") Then
                    '���̕���͎g�p�s�\
                    ListItemFlag(UBound(list) + 1) = True
                ElseIf Not .IsTargetWithinRange(w, SelectedUnit) Then
                    '�^�[�Q�b�g���˒��O
                    ListItemFlag(UBound(list) + 1) = True
                ElseIf .IsWeaponClassifiedAs(w, "�l") Then
                    '�}�b�v�U���͕���I��O
                    ListItemFlag(UBound(list) + 1) = True
                ElseIf .IsWeaponClassifiedAs(w, "��") Then
                    '���̋Z�͎�������U����������ꍇ�ɂ̂ݎg�p
                    ListItemFlag(UBound(list) + 1) = True
                ElseIf .Damage(w, SelectedUnit, True) > 0 Then
                    '�_���[�W��^������
                    ListItemFlag(UBound(list) + 1) = False
                ElseIf Not .IsNormalWeapon(w) _
                    And .CriticalProbability(w, SelectedUnit) > 0 _
                Then
                    '������ʂ�^������
                    ListItemFlag(UBound(list) + 1) = False
                Else
                    '���̕���͌��ʂ�����
                    ListItemFlag(UBound(list) + 1) = True
                End If
            End With
            
            ReDim Preserve list(UBound(list) + 1)
            ReDim Preserve wlist(UBound(list))
            wlist(UBound(list)) = w
            
            '�e����̕\�����e�̐ݒ�
            With u.Weapon(w)
                '�U����
                list(UBound(list)) = RightPaddedString(.Nickname, 29) _
                    & LeftPaddedString(Format$(wpower(w)), 4)
                
                '������
                If Not IsOptionDefined("�\����������\��") Then
                    buf = Format$(MinLng(u.HitProbability(w, SelectedUnit, True), 100)) & "%"
                    list(UBound(list)) = list(UBound(list)) _
                        & LeftPaddedString(buf, 5)
                ElseIf u.WeaponPrecision(w) >= 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString("+" & Format$(u.WeaponPrecision(w)), 5)
                Else
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.WeaponPrecision(w)), 5)
                End If
                
                
                '�N���e�B�J����
                If Not IsOptionDefined("�\����������\��") Then
                    buf = Format$(MinLng(u.CriticalProbability(w, SelectedUnit), 100)) & "%"
                    list(UBound(list)) = list(UBound(list)) _
                        & LeftPaddedString(buf, 5)
                ElseIf u.WeaponCritical(w) >= 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString("+" & Format$(u.WeaponCritical(w)), 5)
                Else
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.WeaponCritical(w)), 5)
                End If
                
                '�c��e��
                If .Bullet > 0 Then
                    list(UBound(list)) = list(UBound(list)) _
                        & LeftPaddedString(Format$(u.Bullet(w)), 3)
                Else
                    list(UBound(list)) = list(UBound(list)) & "  -"
                End If
                
                '�d�m�����
                If .ENConsumption > 0 Then
                    list(UBound(list)) = list(UBound(list)) _
                        & LeftPaddedString(Format$(u.WeaponENConsumption(w)), 4)
                Else
                    list(UBound(list)) = list(UBound(list)) & "   -"
                End If
                
                '�n�`�K��
                list(UBound(list)) = list(UBound(list)) & " " & .Adaption
                
                '�K�v�C��
                If .NecessaryMorale > 0 Then
                    list(UBound(list)) = list(UBound(list)) _
                        & " �C" & .NecessaryMorale
                End If
                
                '����
                wclass = u.WeaponClass(w)
                If InStrNotNest(wclass, "|") > 0 Then
                    wclass = Left$(wclass, InStrNotNest(wclass, "|") - 1)
                End If
                list(UBound(list)) = list(UBound(list)) & " " & wclass
            End With
NextLoop2:
        Next
        
        '���X�g�{�b�N�X��\��
        TopItem = -1
        ret = ListBox(caption_msg, list, _
            "����                         �U�� ���� " & Term("CT", u, 2) & "   �e  " _
                & Term("EN", u, 2) & " �K�� ����", _
            "�A���\��,�J�[�\���ړ�")
        WeaponListBox = wlist(ret)
    End If
    
    DoEvents
End Function

'�A�r���e�B�I��p���X�g�{�b�N�X
Public Function AbilityListBox(u As Unit, caption_msg As String, _
    lb_mode As String, Optional ByVal is_item As Boolean) As Integer
Dim i As Integer, j As Integer, k As Integer
Dim ret As Integer
Dim buf As String, msg As String, rest_msg As String
Dim list() As String, alist() As Integer
Dim is_available As Boolean
Dim is_rbutton_released As Boolean
    
    With u
        '�A�r���e�B��������Ȃ��ꍇ�͎����I�ɂ��̃A�r���e�B��I������B
        '���X�g�{�b�N�X�̕\���͍s��Ȃ��B
        If lb_mode <> "�ꗗ" And Not is_item _
            And MainForm.mnuUnitCommandItem(AbilityCmdID).Caption <> Term("�A�r���e�B", u) _
        Then
            For i = 1 To .CountAbility
                If Not .Ability(i).IsItem _
                    And .IsAbilityMastered(i) _
                Then
                    AbilityListBox = i
                    Exit Function
                End If
            Next
        End If
        
        ReDim list(0)
        ReDim aist(0)
        ReDim ListItemFlag(0)
        
        For i = 1 To .CountAbility
            is_available = True
            If lb_mode = "�ꗗ" Then
                If .IsAbilityAvailable(i, "�X�e�[�^�X") Then
                    '�A�C�e���̎g�p���ʂ��ǂ���
                    With .Ability(i)
                        If is_item Then
                            If Not .IsItem Then
                                GoTo NextLoop
                            End If
                        Else
                            If .IsItem Then
                                GoTo NextLoop
                            End If
                        End If
                    End With
                Else
                    'Disable�R�}���h�Ŏg�p�s�ɂ��ꂽ�A�r���e�B�Ǝg�p�ł��Ȃ����̋Z
                    '�͕\�����Ȃ�
                    If .IsDisabled(.Ability(i).Name) Then
                        GoTo NextLoop
                    End If
                    If Not .IsAbilityMastered(i) Then
                        GoTo NextLoop
                    End If
                    If .IsAbilityClassifiedAs(i, "��") Then
                        If Not .IsCombinationAbilityAvailable(i, True) Then
                            GoTo NextLoop
                        End If
                    End If
                End If
            Else
                '�A�C�e���̎g�p���ʂ��ǂ���
                With .Ability(i)
                    If is_item Then
                        If Not .IsItem Then
                            GoTo NextLoop
                        End If
                    Else
                        If .IsItem Then
                            GoTo NextLoop
                        End If
                    End If
                End With
                If Not .IsAbilityUseful(i, lb_mode) Then
                    'Disable�R�}���h�Ŏg�p�s�ɂ��ꂽ����Ǝg�p�ł��Ȃ����̋Z
                    '�͕\�����Ȃ�
                    If .IsDisabled(.Ability(i).Name) Then
                        GoTo NextLoop
                    End If
                    If Not .IsAbilityMastered(i) Then
                        GoTo NextLoop
                    End If
                    If .IsAbilityClassifiedAs(i, "��") Then
                        If Not .IsCombinationAbilityAvailable(i, True) Then
                            GoTo NextLoop
                        End If
                    End If
                    is_available = False
                End If
            End If
            
            ReDim Preserve list(UBound(list) + 1)
            ReDim Preserve alist(UBound(list))
            ReDim Preserve ListItemFlag(UBound(list))
            alist(UBound(list)) = i
            ListItemFlag(UBound(list)) = Not is_available
            
            With .Ability(i)
                list(UBound(list)) = RightPaddedString(.Nickname, 20)
                
                msg = ""
                rest_msg = ""
                For j = 1 To .CountEffect
                    If .EffectType(j) = "���" Then
                        msg = .EffectName(j)
                        Exit For
                    ElseIf InStr(.EffectName(j), "�^�[��)") > 0 Then
                        '�������Ԃ������\�͂̓^�[�������܂Ƃ߂ĕ\��
                        k = InStr(msg, Mid$(.EffectName(j), InStr(.EffectName(j), "(")))
                        If k > 0 Then
                            msg = Left$(msg, k - 1) & "�A" _
                                & Left$(.EffectName(j), InStr(.EffectName(j), "(") - 1) _
                                & Mid$(msg, k)
                        Else
                            msg = msg & " " & .EffectName(j)
                        End If
                    ElseIf .EffectName(j) <> "" Then
                        msg = msg & " " & .EffectName(j)
                    End If
                Next
                msg = Trim$(msg)
                
                '���ʉ������������ꍇ�͉��s
                buf = StrConv(msg, vbFromUnicode)
                If LenB(buf) > 32 Then
                    Do
                        buf = StrConv(buf, vbUnicode)
                        buf = Left$(buf, Len(buf) - 1)
                        buf = StrConv(buf, vbFromUnicode)
                    Loop While LenB(buf) >= 32
                    buf = StrConv(buf, vbUnicode)
                    rest_msg = Mid$(msg, Len(buf) + 1)
                    If LenB(StrConv(buf, vbFromUnicode)) < 32 Then
                        buf = buf & Space(32 - LenB(StrConv(buf, vbFromUnicode)))
                    End If
                    msg = buf
                End If
                
                list(UBound(list)) = RightPaddedString(list(UBound(list)) & " " & msg, 53)
                
                '�ő�˒�
                If u.AbilityMaxRange(i) > 1 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.AbilityMinRange(i)) _
                        & "-" & Format$(u.AbilityMaxRange(i)), 4)
                ElseIf u.AbilityMaxRange(i) = 1 Then
                    list(UBound(list)) = list(UBound(list)) & "   1"
                Else
                    list(UBound(list)) = list(UBound(list)) & "   -"
                End If
                
                '�c��g�p��
                If .Stock > 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.Stock(i)), 3)
                Else
                    list(UBound(list)) = list(UBound(list)) & "  -"
                End If
                
                '�d�m�����
                If .ENConsumption > 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.AbilityENConsumption(i)), 4)
                Else
                    list(UBound(list)) = list(UBound(list)) & "   -"
                End If
                
                '�K�v�C��
                If .NecessaryMorale > 0 Then
                    list(UBound(list)) = list(UBound(list)) _
                        & " �C" & .NecessaryMorale
                End If
                
                '����
                If InStrNotNest(.Class, "|") > 0 Then
                   list(UBound(list)) = list(UBound(list)) & " " & _
                       Left$(.Class, InStrNotNest(.Class, "|") - 1)
                Else
                   list(UBound(list)) = list(UBound(list)) & " " & .Class
                End If
                
                If rest_msg <> "" Then
                    ReDim Preserve list(UBound(list) + 1)
                    ReDim Preserve alist(UBound(list))
                    ReDim Preserve ListItemFlag(UBound(list))
                    list(UBound(list)) = Space(21) & rest_msg
                    alist(UBound(list)) = i
                    ListItemFlag(UBound(list)) = Not is_available
                End If
            End With
NextLoop:
        Next
    End With
    
    If UBound(list) = 0 Then
        AbilityListBox = 0
        Exit Function
    End If
    
    '���X�g�{�b�N�X��\��
    TopItem = -1
    ret = ListBox(caption_msg, list, _
        "����                 ����                            �˒� ��  " _
            & Term("EN", u, 2) & " ����", _
        "�\���̂�")
    
    If AutoMoveCursor Then
        MoveCursorPos "�_�C�A���O"
    End If
    
    Do Until IsFormClicked
        DoEvents
        '�E�{�^���ł̃_�u���N���b�N�̎���
        If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
            is_rbutton_released = True
        Else
            If is_rbutton_released Then
                IsFormClicked = True
            End If
        End If
    Loop
    If lb_mode <> "�ꗗ" Then
        frmListBox.Hide
    End If
    ReDim ListItemComment(0)
    AbilityListBox = alist(SelectedItem)
    
    DoEvents
End Function

'���͎��Ԑ����t���̃��X�g�{�b�N�X��\��
Public Function LIPS(lb_caption As String, list() As String, _
    lb_info As String, ByVal time_limit As Integer) As Integer
Dim i As Integer
    
    Load frmListBox
    With frmListBox
        '�\�����e��ݒ�
        .Caption = lb_caption
        .labCaption = "  " & lb_info
        .lstItems.Clear
        For i = 1 To UBound(list)
            .lstItems.AddItem "  " & list(i)
        Next
        .lstItems.ListIndex = 0
        .lstItems.Height = 86
        
        '�\���ʒu��ݒ�
        .Left = (Screen.width - .width) / 2
        If MainForm.Visible = True And Not MainForm.WindowState = 1 Then
            .Top = MainForm.Top + MainForm.Height - .Height
        Else
            .Top = (Screen.Height - .Height) / 2
        End If
        
        '���͐������ԂɊւ���ݒ���s��
        .CurrentTime = 0
        .TimeLimit = time_limit
        .picBar.Visible = True
        .Timer1.Enabled = True
        
        '���X�g�{�b�N�X��\�����A�v���C���[����̓��͂�҂�
        SelectedItem = 0
        IsFormClicked = False
        .Show 1
        .CurrentTime = 0
        LIPS = SelectedItem
        
        '���X�g�{�b�N�X������
        .lstItems.Height = 100
        .picBar.Visible = False
        .Timer1.Enabled = False
    End With
End Function

'�����i�̃��X�g�{�b�N�X��\��
Public Function MultiColumnListBox(lb_caption As String, _
    list() As String, ByVal is_center As Boolean) As Integer
Dim i As Integer

    Load frmMultiColumnListBox
    With frmMultiColumnListBox
        .Caption = lb_caption
        .lstItems.Visible = False
        .lstItems.Clear
        
        '�A�C�e����ǉ�
        For i = 1 To UBound(list)
            If ListItemFlag(i) Then
                .lstItems.AddItem "�~" & list(i)
            Else
                .lstItems.AddItem "  " & list(i)
            End If
        Next
        
        For i = 1 To UBound(list)
            If Not ListItemFlag(UBound(list) - i + 1) Then
                .lstItems.ListIndex = UBound(list) - i
                Exit For
            End If
        Next
        
        .lstItems.ListIndex = -1
        .lstItems.Visible = True
        
        If UBound(ListItemComment) <> UBound(list) Then
            ReDim Preserve ListItemComment(UBound(list))
        End If
        
        '�\���ʒu��ݒ�
        .Left = (Screen.width - .width) / 2
        If MainForm.Visible = True _
            And Not MainForm.WindowState = 1 _
            And Not is_center _
        Then
            .Top = MainForm.Top + MainForm.Height - .Height
        Else
            .Top = (Screen.Height - .Height) / 2
        End If
        
        '�擪�ɕ\������A�C�e����ݒ�
        If TopItem > 0 Then
            If .lstItems.TopIndex <> TopItem - 1 Then
                .lstItems.TopIndex = MinLng(TopItem, .lstItems.ListCount) - 1
            End If
        End If
        
        SelectedItem = 0
        
        DoEvents
        IsFormClicked = False
        
        '���X�g�{�b�N�X��\��
        IsMordal = False
        .Show
        Do Until IsFormClicked
            DoEvents
        Loop
        Unload frmMultiColumnListBox
        Set frmMultiColumnListBox = Nothing
        
        MultiColumnListBox = SelectedItem
    End With
End Function

'�����̃A�C�e���I���\�ȃ��X�g�{�b�N�X��\��
Public Function MultiSelectListBox(ByVal lb_caption As String, list() As String, _
    ByVal lb_info As String, ByVal max_num As Integer) As Integer
Dim i As Integer, j As Integer
    
    '�X�e�[�^�X�E�B���h�E�ɍU���̖������Ȃǂ�\�������Ȃ��悤�ɂ���
    CommandState = "���j�b�g�I��"
    
    '���X�g�{�b�N�X���쐬���ĕ\��
    Load frmMultiSelectListBox
    With frmMultiSelectListBox
        .Caption = lb_caption
        .lblCaption = "�@" & lb_info
        MaxListItem = max_num
        For i = 1 To UBound(list)
            .lstItems.AddItem "�@" & list(i)
        Next
        .cmdSort.Caption = "���̏��ɕ��בւ�"
        .Left = MainForm.Left
        .Top = (Screen.Height - .Height) / 2
        .Show 1
    End With
    
    '�I�����ꂽ���ڐ���Ԃ�
    j = 0
    For i = 1 To UBound(list)
        If ListItemFlag(i) Then
            j = j + 1
        End If
    Next
    MultiSelectListBox = j
    
    '���X�g�{�b�N�X������
    Unload frmMultiSelectListBox
    Set frmMultiSelectListBox = Nothing
End Function


' === �摜�`��Ɋւ��鏈�� ===

'�摜���E�B���h�E�ɕ`��
Public Function DrawPicture(fname As String, _
    ByVal dx As Long, ByVal dy As Long, _
    ByVal dw As Long, ByVal dh As Long, _
    ByVal sx As Long, ByVal sy As Long, _
    ByVal sw As Long, ByVal sh As Long, _
    draw_option As String) As Boolean
Dim opt As String, pic_option As String, pic_option2 As String
Dim permanent As Boolean, transparent As Boolean
Dim is_monotone As Boolean, is_sepia As Boolean
Dim is_sunset As Boolean, is_water As Boolean
Dim bright_count As Integer, dark_count As Integer
Dim is_sil As Boolean, negpos As Boolean
Dim vrev As Boolean, hrev As Boolean
Dim top_part As Boolean, bottom_part As Boolean
Dim left_part As Boolean, right_part As Boolean
Dim tleft_part As Boolean, tright_part As Boolean
Dim bleft_part As Boolean, bright_part As Boolean
Dim angle As Long
Dim on_msg_window As Boolean, on_status_window As Boolean
Dim keep_picture As Boolean
Dim ret As Long, i As Integer, j As Integer
Dim pfname As String, fpath As String
Dim pic As PictureBox, mask_pic As PictureBox
Dim stretched_pic As PictureBox, stretched_mask_pic As PictureBox
Dim orig_pic As PictureBox, orig_width As Long, orig_height As Long
Dim in_history As Boolean, found_orig As Boolean, load_only As Boolean
Dim is_colorfilter As Boolean
Dim fcolor As Long, trans_par As Double
Dim tdir As String, tnum As String, tname As String
Static init_draw_pitcure As Boolean
Static scenario_bitmap_dir_exists As Boolean
Static extdata_bitmap_dir_exists As Boolean
Static extdata2_bitmap_dir_exists As Boolean
Static scenario_anime_bitmap_dir_exists As Boolean
Static extdata_anime_bitmap_dir_exists As Boolean
Static extdata2_anime_bitmap_dir_exists As Boolean
Static scenario_event_bitmap_dir_exists As Boolean
Static extdata_event_bitmap_dir_exists As Boolean
Static extdata2_event_bitmap_dir_exists As Boolean
Static scenario_cutin_bitmap_dir_exists As Boolean
Static extdata_cutin_bitmap_dir_exists As Boolean
Static extdata2_cutin_bitmap_dir_exists As Boolean
Static app_cutin_bitmap_dir_exists As Boolean
Static scenario_pilot_bitmap_dir_exists As Boolean
Static extdata_pilot_bitmap_dir_exists As Boolean
Static extdata2_pilot_bitmap_dir_exists As Boolean
Static app_pilot_bitmap_dir_exists As Boolean
Static scenario_unit_bitmap_dir_exists As Boolean
Static extdata_unit_bitmap_dir_exists As Boolean
Static extdata2_unit_bitmap_dir_exists As Boolean
Static app_unit_bitmap_dir_exists As Boolean
Static scenario_map_bitmap_dir_exists As Boolean
Static extdata_map_bitmap_dir_exists As Boolean
Static extdata2_map_bitmap_dir_exists As Boolean
Static display_byte_pixel As Long
Static last_fname As String
Static last_exists As Boolean
Static last_path As String
Static last_angle As Long
Static fpath_history As New Collection
    
    '������s���Ɋe����̏��������s��
    If Not init_draw_pitcure Then
        '�e�t�H���_��Bitmap�t�H���_�����邩�`�F�b�N
        If Len(Dir$(ScenarioPath & "Bitmap", vbDirectory)) > 0 Then
            scenario_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath & "Bitmap", vbDirectory)) > 0 Then
            extdata_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath2 & "Bitmap", vbDirectory)) > 0 Then
            extdata2_bitmap_dir_exists = True
        End If
        
        If Len(Dir$(ScenarioPath & "Bitmap\Anime", vbDirectory)) > 0 Then
            scenario_anime_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath & "Bitmap\Anime", vbDirectory)) > 0 Then
            extdata_anime_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath2 & "Bitmap\Anime", vbDirectory)) > 0 Then
            extdata2_anime_bitmap_dir_exists = True
        End If
        
        If Len(Dir$(ScenarioPath & "Bitmap\Event", vbDirectory)) > 0 Then
            scenario_event_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath & "Bitmap\Event", vbDirectory)) > 0 Then
            extdata_event_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath2 & "Bitmap\Event", vbDirectory)) > 0 Then
            extdata2_event_bitmap_dir_exists = True
        End If
        
        If Len(Dir$(ScenarioPath & "Bitmap\Cutin", vbDirectory)) > 0 Then
            scenario_cutin_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath & "Bitmap\Cutin", vbDirectory)) > 0 Then
            extdata_cutin_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath2 & "Bitmap\Cutin", vbDirectory)) > 0 Then
            extdata2_cutin_bitmap_dir_exists = True
        End If
        If Len(Dir$(AppPath & "Bitmap\Cutin", vbDirectory)) > 0 Then
            app_cutin_bitmap_dir_exists = True
        End If
        
        If Len(Dir$(ScenarioPath & "Bitmap\Pilot", vbDirectory)) > 0 Then
            scenario_pilot_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath & "Bitmap\Pilot", vbDirectory)) > 0 Then
            extdata_pilot_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath2 & "Bitmap\Pilot", vbDirectory)) > 0 Then
            extdata2_pilot_bitmap_dir_exists = True
        End If
        If Len(Dir$(AppPath & "Bitmap\Pilot", vbDirectory)) > 0 Then
            app_pilot_bitmap_dir_exists = True
        End If
        
        If Len(Dir$(ScenarioPath & "Bitmap\Unit", vbDirectory)) > 0 Then
            scenario_unit_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath & "Bitmap\Unit", vbDirectory)) > 0 Then
            extdata_unit_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath2 & "Bitmap\Unit", vbDirectory)) > 0 Then
            extdata2_unit_bitmap_dir_exists = True
        End If
        If Len(Dir$(AppPath & "Bitmap\Unit", vbDirectory)) > 0 Then
            app_unit_bitmap_dir_exists = True
        End If
        
        If Len(Dir$(ScenarioPath & "Bitmap\Map", vbDirectory)) > 0 Then
            scenario_map_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath & "Bitmap\Map", vbDirectory)) > 0 Then
            extdata_map_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath2 & "Bitmap\Map", vbDirectory)) > 0 Then
            extdata2_map_bitmap_dir_exists = True
        End If
        
        '��ʂ̐F�����Q��
        display_byte_pixel = GetDeviceCaps(MainForm.picMain(0).hDC, BITSPIXEL) \ 8
        
        init_draw_pitcure = True
    End If
    
    '�_�~�[�̃t�@�C�����H
    Select Case fname
        Case "", "-.bmp", "EFFECT_Void.bmp"
            Exit Function
    End Select
    
    'Debug.Print fname, draw_option
    
    '�I�v�V�����̉��
    BGColor = vbWhite
    '�}�X�N�摜�ɉe�����Ȃ��I�v�V����
    pic_option = ""
    '�}�X�N�摜�ɉe������I�v�V����
    pic_option2 = ""
    '�t�B���^���̓��ߓx��������
    trans_par = -1
    i = 1
    Do While i <= LLength(draw_option)
        opt = LIndex(draw_option, i)
        Select Case opt
            Case "�w�i"
                permanent = True
                '�w�i�������݂Ŗ��Z�s�A�F�̃}�b�v�̏ꍇ�͎w�肪�Ȃ��Ă�������ʂ�t����
                Select Case MapDrawMode
                    Case "��"
                        dark_count = dark_count + 1
                        pic_option = pic_option & " ��"
                    Case "����"
                        is_monotone = True
                        pic_option = pic_option & " ����"
                    Case "�Z�s�A"
                        is_sepia = True
                        pic_option = pic_option & " �Z�s�A"
                    Case "�[�Ă�"
                        is_sunset = True
                        pic_option = pic_option & " �[�Ă�"
                    Case "����"
                        is_water = True
                        pic_option = pic_option & " ����"
                    Case "�t�B���^"
                        is_colorfilter = True
                        fcolor = MapDrawFilterColor
                        pic_option2 = pic_option2 & " �t�B���^=" & CStr(MapDrawFilterColor)
                End Select
            Case "����"
                transparent = True
                pic_option = pic_option & " " & opt
            Case "����"
                is_monotone = True
                pic_option = pic_option & " " & opt
            Case "�Z�s�A"
                is_sepia = True
                pic_option = pic_option & " " & opt
            Case "�[�Ă�"
                is_sunset = True
                pic_option = pic_option & " " & opt
            Case "����"
                is_water = True
                pic_option = pic_option & " " & opt
            Case "��"
                bright_count = bright_count + 1
                pic_option = pic_option & " " & opt
            Case "��"
                dark_count = dark_count + 1
                pic_option = pic_option & " " & opt
            Case "���E���]"
                hrev = True
                pic_option2 = pic_option2 & " " & opt
            Case "�㉺���]"
                vrev = True
                pic_option2 = pic_option2 & " " & opt
            Case "�l�K�|�W���]"
                negpos = True
                pic_option = pic_option & " " & opt
            Case "�V���G�b�g"
                is_sil = True
                pic_option = pic_option & " " & opt
            Case "�㔼��"
                top_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "������"
                bottom_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "�E����"
                right_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "������"
                left_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "�E��"
                tright_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "����"
                tleft_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "�E��"
                bright_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "����"
                bleft_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "���b�Z�[�W"
                on_msg_window = True
            Case "�X�e�[�^�X"
                on_status_window = True
            Case "�ێ�"
                keep_picture = True
            Case "�E��]"
                i = i + 1
                angle = StrToLng(LIndex(draw_option, i))
                pic_option2 = pic_option2 & " �E��]=" & Format$(angle Mod 360)
            Case "����]"
                i = i + 1
                angle = -StrToLng(LIndex(draw_option, i))
                pic_option2 = pic_option2 & " �E��]=" & Format$(angle Mod 360)
            Case "�t�B���^"
                is_colorfilter = True
            Case Else
                If Right(opt, 1) = "%" And IsNumeric(Left(opt, Len(opt) - 1)) Then
                    trans_par = MaxDbl(0, MinDbl(1, CDbl(Left(opt, Len(opt) - 1)) / 100))
                    pic_option2 = pic_option2 & " �t�B���^���ߓx=" & opt
                Else
                    If is_colorfilter Then
                        fcolor = CLng(opt)
                        pic_option2 = pic_option2 & " �t�B���^=" & opt
                    Else
                        BGColor = CLng(opt)
                        pic_option2 = pic_option2 & " " & opt
                    End If
                End If
        End Select
        i = i + 1
    Loop
    pic_option = Trim$(pic_option)
    pic_option2 = Trim$(pic_option2)
    
    '�`����ݒ�
    If on_msg_window Then
        '���b�Z�[�W�E�B���h�E�ւ̃p�C���b�g�摜�̕`��
        Set pic = frmMessage.picFace
        permanent = False
    ElseIf on_status_window Then
        '�X�e�[�^�X�E�B���h�E�ւ̃p�C���b�g�摜�̕`��
        Set pic = MainForm.picUnitStatus
    ElseIf permanent Then
        '�w�i�ւ̕`��
        Set pic = MainForm.picBack
    Else
        '�}�b�v�E�B���h�E�ւ̒ʏ�̕`��
        Set pic = MainForm.picMain(0)
        SaveScreen
    End If
    
    '�ǂݍ��ރt�@�C���̒T��
    
    '�O��̉摜�t�@�C���Ɠ����H
    If fname = last_fname Then
        '�O��t�@�C���͌������Ă����̂��H
        If Not last_exists Then
            DrawPicture = False
            Exit Function
        End If
    End If
    
    '�ȑO�\�������g��摜�����p�\�H
    For i = 0 To ImageBufferSize - 1
        '�����t�@�C���H
        If PicBufFname(i) = fname Then
            '�I�v�V�����������H
            If PicBufOption(i) = pic_option _
                And PicBufOption2(i) = pic_option2 _
                And Not PicBufIsMask(i) _
                And PicBufDW(i) = dw _
                And PicBufDH(i) = dh _
                And PicBufSX(i) = sx _
                And PicBufSY(i) = sy _
                And PicBufSW(i) = sw _
                And PicBufSH(i) = sh _
            Then
                '�����t�@�C���A�I�v�V�����ɂ��摜����������
                
                '�ȑO�\�������摜�����̂܂ܗ��p
                UsePicBuf i
                Set orig_pic = MainForm.picBuf(i)
                With orig_pic
                    orig_width = .width
                    orig_height = .Height
                End With
                'Debug.Print "Reuse " & Format$(i) & " As Stretched"
                GoTo EditedPicture
            End If
        End If
    Next
    
    '�ȑO�\�������摜�����p�\�H
    For i = 0 To ImageBufferSize - 1
        '�����t�@�C���H
        If PicBufFname(i) = fname Then
            '�I�v�V�����������H
            If PicBufOption(i) = pic_option _
                And PicBufOption2(i) = pic_option2 _
                And Not PicBufIsMask(i) _
                And PicBufDW(i) = DEFAULT_LEVEL _
                And PicBufDH(i) = DEFAULT_LEVEL _
                And PicBufSX(i) = sx _
                And PicBufSY(i) = sy _
                And PicBufSW(i) = sw _
                And PicBufSH(i) = sh _
            Then
                '�����t�@�C���A�I�v�V�����ɂ��摜����������
                
                '�ȑO�\�������摜�����̂܂ܗ��p
                UsePicBuf i
                Set orig_pic = MainForm.picBuf(i)
                With orig_pic
                    orig_width = .width
                    orig_height = .Height
                End With
                'Debug.Print "Reuse " & Format$(i) & " As Edited"
                found_orig = True
                GoTo EditedPicture
            End If
        End If
    Next
    
    '�ȑO�g�p���������摜�����p�\�H
    If sw <> 0 Then
        For i = 0 To ImageBufferSize - 1
            '�����t�@�C���H
            If PicBufFname(i) = fname Then
                If PicBufOption(i) = "" _
                    And PicBufOption2(i) = "" _
                    And Not PicBufIsMask(i) _
                    And PicBufDW(i) = DEFAULT_LEVEL _
                    And PicBufDH(i) = DEFAULT_LEVEL _
                    And PicBufSX(i) = sx _
                    And PicBufSY(i) = sy _
                    And PicBufSW(i) = sw _
                    And PicBufSH(i) = sh _
                Then
                    '�ȑO�g�p���������摜�����̂܂ܗ��p
                    UsePicBuf i
                    Set orig_pic = MainForm.picBuf(i)
                    With orig_pic
                        orig_width = .width
                        orig_height = .Height
                    End With
                    'Debug.Print "Reuse " & Format$(i) & " As Partial"
                    GoTo LoadedOrigPicture
                End If
            End If
        Next
    End If
    
    '�ȑO�g�p�������摜�����p�\�H
    For i = 0 To ImageBufferSize - 1
        '�����t�@�C���H
        If PicBufFname(i) = fname Then
            If PicBufOption(i) = "" _
                And PicBufOption2(i) = "" _
                And Not PicBufIsMask(i) _
                And PicBufDW(i) = DEFAULT_LEVEL _
                And PicBufDH(i) = DEFAULT_LEVEL _
                And PicBufSW(i) = 0 _
            Then
                '�ȑO�g�p�������摜�����̂܂ܗ��p
                UsePicBuf i
                Set orig_pic = MainForm.picBuf(i)
                With orig_pic
                    orig_width = .width
                    orig_height = .Height
                End With
                'Debug.Print "Reuse " & Format$(i) & " As Orig"
                GoTo LoadedOrigPicture
            End If
        End If
    Next
    
    '����ȃt�@�C����
    Select Case LCase$(fname)
        Case "black.bmp", "event\black.bmp"
            '���œh��Ԃ�
            With pic
                If dx = DEFAULT_LEVEL Then
                    dx = (.width - dw) \ 2
                End If
                If dy = DEFAULT_LEVEL Then
                    dy = (.Height - dh) \ 2
                End If
                ret = PatBlt(.hDC, dx, dy, dw, dh, BLACKNESS)
            End With
            GoTo DrewPicture
        Case "white.bmp", "event\white.bmp"
            '���œh��Ԃ�
            With pic
                If dx = DEFAULT_LEVEL Then
                    dx = (.width - dw) \ 2
                End If
                If dy = DEFAULT_LEVEL Then
                    dy = (.Height - dh) \ 2
                End If
                ret = PatBlt(.hDC, dx, dy, dw, dh, WHITENESS)
            End With
            GoTo DrewPicture
        Case "common\effect_tile(ally).bmp", "anime\common\effect_tile(ally).bmp"
            '�������j�b�g�^�C��
            Set orig_pic = MainForm.picUnit
            orig_width = 32
            orig_height = 32
            GoTo LoadedOrigPicture
        Case "common\effect_tile(enemy).bmp", "anime\common\effect_tile(enemy).bmp"
            '�G���j�b�g�^�C��
            Set orig_pic = MainForm.picEnemy
            orig_width = 32
            orig_height = 32
            GoTo LoadedOrigPicture
        Case "common\effect_tile(neutral).bmp", "anime\common\effect_tile(neutral).bmp"
            '�������j�b�g�^�C��
            Set orig_pic = MainForm.picNeautral
            orig_width = 32
            orig_height = 32
            GoTo LoadedOrigPicture
    End Select
    
    '�t���p�X�Ŏw�肳��Ă���H
    If InStr(fname, ":") = 2 Then
        fpath = ""
        last_path = ""
        '�o�^������邽��
        in_history = True
        GoTo FoundPicture
    End If
    
    '�������������Ă݂�
    On Error GoTo NotFound
    fpath = fpath_history.Item(fname)
    last_path = ""
    
    '������Ƀt�@�C���𔭌�
    On Error GoTo 0
    If fpath = "" Then
        '�t�@�C���͑��݂��Ȃ�
        last_fname = fname
        last_exists = False
        DrawPicture = False
        Exit Function
    End If
    in_history = True
    GoTo FoundPicture
    
NotFound:
    
    '�����ɂȂ�����
    On Error GoTo 0
    
    '�퓬�A�j���p�H
    If InStr(fname, "\EFFECT_") > 0 Then
        If scenario_anime_bitmap_dir_exists Then
            If FileExists(ScenarioPath & "Bitmap\Anime\" & fname) Then
                fpath = ScenarioPath & "Bitmap\Anime\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata_anime_bitmap_dir_exists Then
            If FileExists(ExtDataPath & "Bitmap\Anime\" & fname) Then
                fpath = ExtDataPath & "Bitmap\Anime\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata2_anime_bitmap_dir_exists Then
            If FileExists(ExtDataPath2 & "Bitmap\Anime\" & fname) Then
                fpath = ExtDataPath2 & "Bitmap\Anime\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If FileExists(AppPath & "Bitmap\Anime\" & fname) Then
            fpath = AppPath & "Bitmap\Anime\"
            last_path = ""
            GoTo FoundPicture
        End If
    End If
    
    '�O��Ɠ����p�X�H
    If Len(last_path) > 0 Then
        If FileExists(last_path & fname) Then
            fpath = last_path
            GoTo FoundPicture
        End If
    End If
    
    '�p�X������H
    If InStr(fname, "Bitmap\") > 0 Then
        If scenario_bitmap_dir_exists Then
            If FileExists(ScenarioPath & fname) Then
                fpath = ScenarioPath
                last_path = fpath
                GoTo FoundPicture
            End If
        End If
        If FileExists(AppPath & fname) Then
            fpath = AppPath
            last_path = ""
            GoTo FoundPicture
        End If
        If Mid$(fname, 2, 1) = ":" Then
            fpath = ""
            last_path = ""
            GoTo FoundPicture
        End If
    End If
    
    '�t�H���_�w�肠��H
    If InStr(fname, "\") > 0 Then
        If scenario_bitmap_dir_exists Then
            If FileExists(ScenarioPath & "Bitmap\" & fname) Then
                fpath = ScenarioPath & "Bitmap\"
                last_path = fpath
                GoTo FoundPicture
            End If
        End If
        If extdata_bitmap_dir_exists Then
            If FileExists(ExtDataPath & "Bitmap\" & fname) Then
                fpath = ExtDataPath & "Bitmap\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata2_bitmap_dir_exists Then
            If FileExists(ExtDataPath2 & "Bitmap\" & fname) Then
                fpath = ExtDataPath2 & "Bitmap\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If FileExists(AppPath & "Bitmap\" & fname) Then
            fpath = AppPath & "Bitmap\"
            last_path = ""
            GoTo FoundPicture
        End If
        
        If LCase$(Left$(fname, 4)) = "map\" Then
            tname = Mid$(fname, 5)
            If InStr(tname, "\") = 0 Then
                i = Len(tname) - 5
                Do While i > 0
                    If Mid$(tname, i, 1) Like "[!-0-9]" Then
                        Exit Do
                    End If
                    i = i - 1
                Loop
                If i > 0 Then
                    tdir = Left$(tname, i) & "\"
                    tnum = Mid$(tname, i + 1, Len(tname) - i - 4)
                    tname = Left$(tname, i) & Format$(StrToLng(tnum), "0000") & ".bmp"
                End If
            End If
        End If
    Else
        '�n�`�摜�����p�̒n�`�摜�f�B���N�g������4���t�@�C�������쐬
        If fname Like "*#.bmp" And Left$(fname, 1) Like "[a-z]" Then
            i = Len(fname) - 5
            Do While i > 0
                If Mid$(fname, i, 1) Like "[!-0-9]" Then
                    Exit Do
                End If
                i = i - 1
            Loop
            If i > 0 Then
                tdir = Left$(fname, i)
                With TDList
                    For j = 1 To .Count
                        If tdir = .Item(.OrderedID(j)).Bitmap Then
                            tnum = Mid$(fname, i + 1, Len(fname) - i - 4)
                            tname = Left$(fname, i) & Format$(StrToLng(tnum), "0000") & ".bmp"
                            Exit For
                        End If
                    Next
                    If j <= .Count Then
                        tdir = tdir & "\"
                    Else
                        tdir = ""
                    End If
                End With
            End If
        End If
    End If
    
    '�e�t�H���_����������
    
    'Bitmap�t�H���_�ɒ��u��
    If scenario_map_bitmap_dir_exists Then
        If FileExists(ScenarioPath & "Bitmap\" & fname) Then
            fpath = ScenarioPath & "Bitmap\"
            last_path = fpath
            GoTo FoundPicture
        End If
    End If
    If FileExists(ScenarioPath & "Bitmap\" & fname) Then
        fpath = ScenarioPath & "Bitmap\"
        last_path = fpath
        GoTo FoundPicture
    End If
    
    '�V�i���I�t�H���_
    If scenario_bitmap_dir_exists Then
        If scenario_anime_bitmap_dir_exists Then
            If FileExists(ScenarioPath & "Bitmap\Anime\" & fname) Then
                fpath = ScenarioPath & "Bitmap\Anime\"
                last_path = fpath
                GoTo FoundPicture
            End If
        End If
        If scenario_event_bitmap_dir_exists Then
            If FileExists(ScenarioPath & "Bitmap\Event\" & fname) Then
                fpath = ScenarioPath & "Bitmap\Event\"
                last_path = fpath
                GoTo FoundPicture
            End If
        End If
        If scenario_cutin_bitmap_dir_exists Then
            If FileExists(ScenarioPath & "Bitmap\Cutin\" & fname) Then
                fpath = ScenarioPath & "Bitmap\Cutin\"
                last_path = fpath
                GoTo FoundPicture
            End If
        End If
        If scenario_pilot_bitmap_dir_exists Then
            If FileExists(ScenarioPath & "Bitmap\Pilot\" & fname) Then
                fpath = ScenarioPath & "Bitmap\Pilot\"
                last_path = fpath
                GoTo FoundPicture
            End If
        End If
        If scenario_unit_bitmap_dir_exists Then
            If FileExists(ScenarioPath & "Bitmap\Unit\" & fname) Then
                fpath = ScenarioPath & "Bitmap\Unit\"
                last_path = fpath
                GoTo FoundPicture
            End If
        End If
        If scenario_map_bitmap_dir_exists Then
            If tdir <> "" Then
                If FileExists(ScenarioPath & "Bitmap\Map\" & tdir & fname) Then
                    fpath = ScenarioPath & "Bitmap\Map\" & tdir
                    last_path = fpath
                    GoTo FoundPicture
                End If
                If FileExists(ScenarioPath & "Bitmap\Map\" & tdir & tname) Then
                    fname = tname
                    fpath = ScenarioPath & "Bitmap\Map\" & tdir
                    last_path = fpath
                    '�o�^������邽��
                    in_history = True
                    GoTo FoundPicture
                End If
                If FileExists(ScenarioPath & "Bitmap\Map\" & tname) Then
                    fname = tname
                    fpath = ScenarioPath & "Bitmap\Map\"
                    last_path = fpath
                    '�o�^������邽��
                    in_history = True
                    GoTo FoundPicture
                End If
            End If
            If FileExists(ScenarioPath & "Bitmap\Map\" & fname) Then
                fpath = ScenarioPath & "Bitmap\Map\"
                last_path = fpath
                GoTo FoundPicture
            End If
        End If
    End If
    
    'ExtDataPath
    If extdata_bitmap_dir_exists Then
        If extdata_anime_bitmap_dir_exists Then
            If FileExists(ExtDataPath & "Bitmap\Anime\" & fname) Then
                fpath = ExtDataPath & "Bitmap\Anime\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata_event_bitmap_dir_exists Then
            If FileExists(ExtDataPath & "Bitmap\Event\" & fname) Then
                fpath = ExtDataPath & "Bitmap\Event\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata_cutin_bitmap_dir_exists Then
            If FileExists(ExtDataPath & "Bitmap\Cutin\" & fname) Then
                fpath = ExtDataPath & "Bitmap\Cutin\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata_pilot_bitmap_dir_exists Then
            If FileExists(ExtDataPath & "Bitmap\Pilot\" & fname) Then
                fpath = ExtDataPath & "Bitmap\Pilot\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata_unit_bitmap_dir_exists Then
            If FileExists(ExtDataPath & "Bitmap\Unit\" & fname) Then
                fpath = ExtDataPath & "Bitmap\Unit\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata_map_bitmap_dir_exists Then
            If tdir <> "" Then
                If FileExists(ExtDataPath & "Bitmap\Map\" & tdir & fname) Then
                    fpath = ExtDataPath & "Bitmap\Map\" & tdir
                    last_path = ""
                    GoTo FoundPicture
                End If
                If FileExists(ExtDataPath & "Bitmap\Map\" & tdir & tname) Then
                    fname = tname
                    fpath = ExtDataPath & "Bitmap\Map\" & tdir
                    last_path = ""
                    '�o�^������邽��
                    in_history = True
                    GoTo FoundPicture
                End If
                If FileExists(ExtDataPath & "Bitmap\Map\" & tname) Then
                    fname = tname
                    fpath = ExtDataPath & "Bitmap\Map\"
                    last_path = ""
                    '�o�^������邽��
                    in_history = True
                    GoTo FoundPicture
                End If
            End If
            If FileExists(ExtDataPath & "Bitmap\Map\" & fname) Then
                fpath = ExtDataPath & "Bitmap\Map\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
    End If
    
    'ExtDataPath2
    If extdata2_bitmap_dir_exists Then
        If extdata2_anime_bitmap_dir_exists Then
            If FileExists(ExtDataPath2 & "Bitmap\Anime\" & fname) Then
                fpath = ExtDataPath2 & "Bitmap\Anime\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata2_event_bitmap_dir_exists Then
            If FileExists(ExtDataPath2 & "Bitmap\Event\" & fname) Then
                fpath = ExtDataPath2 & "Bitmap\Event\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata2_cutin_bitmap_dir_exists Then
            If FileExists(ExtDataPath2 & "Bitmap\Cutin\" & fname) Then
                fpath = ExtDataPath2 & "Bitmap\Cutin\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata2_pilot_bitmap_dir_exists Then
            If FileExists(ExtDataPath2 & "Bitmap\Pilot\" & fname) Then
                fpath = ExtDataPath2 & "Bitmap\Pilot\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata2_unit_bitmap_dir_exists Then
            If FileExists(ExtDataPath2 & "Bitmap\Unit\" & fname) Then
                fpath = ExtDataPath2 & "Bitmap\Unit\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
        If extdata2_map_bitmap_dir_exists Then
            If tdir <> "" Then
                If FileExists(ExtDataPath2 & "Bitmap\Map\" & tdir & fname) Then
                    fpath = ExtDataPath2 & "Bitmap\Map\" & tdir
                    last_path = ""
                    GoTo FoundPicture
                End If
                If FileExists(ExtDataPath2 & "Bitmap\Map\" & tdir & tname) Then
                    fname = tname
                    fpath = ExtDataPath2 & "Bitmap\Map\" & tdir
                    last_path = ""
                    '�o�^������邽��
                    in_history = True
                    GoTo FoundPicture
                End If
                If FileExists(ExtDataPath2 & "Bitmap\Map\" & tname) Then
                    fname = tname
                    fpath = ExtDataPath2 & "Bitmap\Map\"
                    last_path = ""
                    '�o�^������邽��
                    in_history = True
                    GoTo FoundPicture
                End If
            End If
            If FileExists(ExtDataPath2 & "Bitmap\Map\" & fname) Then
                fpath = ExtDataPath2 & "Bitmap\Map\"
                last_path = ""
                GoTo FoundPicture
            End If
        End If
    End If
    
    '�{�̑��t�H���_
    If FileExists(AppPath & "Bitmap\Anime\" & fname) Then
        fpath = AppPath & "Bitmap\Anime\"
        last_path = ""
        GoTo FoundPicture
    End If
    If FileExists(AppPath & "Bitmap\Event\" & fname) Then
        fpath = AppPath & "Bitmap\Event\"
        last_path = ""
        GoTo FoundPicture
    End If
    If app_cutin_bitmap_dir_exists Then
        If FileExists(AppPath & "Bitmap\Cutin\" & fname) Then
            fpath = AppPath & "Bitmap\Cutin\"
            last_path = ""
            GoTo FoundPicture
        End If
    End If
    If app_pilot_bitmap_dir_exists Then
        If FileExists(AppPath & "Bitmap\Pilot\" & fname) Then
            fpath = AppPath & "Bitmap\Pilot\"
            last_path = ""
            GoTo FoundPicture
        End If
    End If
    If app_unit_bitmap_dir_exists Then
        If FileExists(AppPath & "Bitmap\Unit\" & fname) Then
            fpath = AppPath & "Bitmap\Unit\"
            last_path = ""
            GoTo FoundPicture
        End If
    End If
    If tdir <> "" Then
        If FileExists(AppPath & "Bitmap\Map\" & tdir & fname) Then
            fpath = AppPath & "Bitmap\Map\" & tdir
            last_path = ""
            GoTo FoundPicture
        End If
        If FileExists(AppPath & "Bitmap\Map\" & tdir & tname) Then
            fname = tname
            fpath = AppPath & "Bitmap\Map\" & tdir
            last_path = ""
            '�o�^������邽��
            in_history = True
            GoTo FoundPicture
        End If
        If FileExists(AppPath & "Bitmap\Map\" & tname) Then
            fname = tname
            fpath = AppPath & "Bitmap\Map\"
            last_path = ""
            '�o�^������邽��
            in_history = True
            GoTo FoundPicture
        End If
    End If
    If FileExists(AppPath & "Bitmap\Map\" & fname) Then
        fpath = AppPath & "Bitmap\Map\"
        last_path = ""
        GoTo FoundPicture
    End If
    
    '������Ȃ������c�c
    
    '�����ɋL�^���Ă���
    fpath_history.Add "", fname
    
    '�\���𒆎~
    last_fname = fname
    last_exists = False
    DrawPicture = False
    Exit Function
    
FoundPicture:
    
    '�t�@�C�������L�^���Ă���
    last_fname = fname
    
    '�����ɋL�^���Ă���
    If Not in_history Then
        fpath_history.Add fpath, fname
    End If
    
    last_exists = True
    pfname = fpath & fname
    
    '�g�p����o�b�t�@��I��
    i = GetPicBuf()
    Set orig_pic = MainForm.picBuf(i)
    PicBufFname(i) = fname
    PicBufOption(i) = ""
    PicBufOption2(i) = ""
    PicBufDW(i) = DEFAULT_LEVEL
    PicBufDH(i) = DEFAULT_LEVEL
    PicBufSX(i) = 0
    PicBufSY(i) = 0
    PicBufSW(i) = 0
    PicBufSH(i) = 0
    PicBufIsMask(i) = False
    'Debug.Print "Use " & Format$(i) & " As Orig"
    
    LoadPicture2 orig_pic, pfname
    
    '�ǂݍ��񂾉摜�̃T�C�Y(�o�C�g��)���o�b�t�@���ɋL�^���Ă���
    With orig_pic
        PicBufSize(i) = display_byte_pixel * .width * .Height
    End With
    
LoadedOrigPicture:
    
    With orig_pic
        orig_width = .width
        orig_height = .Height
    End With
    
    '���摜�̈ꕔ�݂̂�`��H
    If sw <> 0 Then
        If sw <> orig_width Or sh <> orig_height Then
            '�g�p����picBuf��I��
            i = GetPicBuf(display_byte_pixel * sw * sh)
            PicBufFname(i) = fname
            PicBufOption(i) = ""
            PicBufOption2(i) = ""
            PicBufDW(i) = DEFAULT_LEVEL
            PicBufDH(i) = DEFAULT_LEVEL
            PicBufSX(i) = sx
            PicBufSY(i) = sy
            PicBufSW(i) = sw
            PicBufSH(i) = sh
            PicBufIsMask(i) = False
            'Debug.Print "Use " & Format$(i) & " As Partial"
            
            '���摜����`�敔�����R�s�[
            With MainForm.picBuf(i)
                .Picture = LoadPicture("")
                .width = sw
                .Height = sh
                If sx = DEFAULT_LEVEL Then
                    sx = (orig_width - sw) \ 2
                End If
                If sy = DEFAULT_LEVEL Then
                    sy = (orig_height - sh) \ 2
                End If
                ret = BitBlt(.hDC, _
                    0, 0, sw, sh, _
                    orig_pic.hDC, sx, sy, _
                    SRCCOPY)
            End With
            
            Set orig_pic = MainForm.picBuf(i)
            orig_width = sw
            orig_height = sh
        End If
    End If
    
LoadedPicture:
    
    '���摜���C�����Ďg���ꍇ�͌��摜��ʂ�picBuf�ɃR�s�[���ďC������
    If top_part Or bottom_part Or left_part Or right_part _
        Or tleft_part Or tright_part Or bleft_part Or bright_part _
        Or is_monotone Or is_sepia Or is_sunset Or is_water _
        Or negpos Or is_sil Or vrev Or hrev _
        Or bright_count > 0 Or dark_count > 0 Or angle Mod 360 <> 0 _
        Or is_colorfilter _
    Then
        '�g�p����picBuf��I��
        i = GetPicBuf(display_byte_pixel * orig_width * orig_height)
        PicBufFname(i) = fname
        PicBufOption(i) = pic_option
        PicBufOption2(i) = pic_option2
        PicBufDW(i) = DEFAULT_LEVEL
        PicBufDH(i) = DEFAULT_LEVEL
        PicBufSX(i) = sx
        PicBufSY(i) = sy
        PicBufSW(i) = sw
        PicBufSH(i) = sh
        PicBufIsMask(i) = False
        'Debug.Print "Use " & Format$(i) & " As Edited"
        
        '�摜���R�s�[
        With MainForm.picBuf(i)
            .Picture = LoadPicture("")
            .width = orig_width
            .Height = orig_height
            ret = BitBlt(.hDC, _
                0, 0, orig_width, orig_height, _
                orig_pic.hDC, 0, 0, _
                SRCCOPY)
        End With
        Set orig_pic = MainForm.picBuf(i)
    End If
    
    '�摜�̈ꕔ��h��Ԃ��ĕ`�悷��ꍇ
    If top_part Then
        '�㔼��
        orig_pic.Line (0, orig_height \ 2)-(orig_width - 1, orig_height - 1), BGColor, BF
    End If
    If bottom_part Then
        '������
        orig_pic.Line (0, 0)-(orig_width - 1, orig_height \ 2 - 1), BGColor, BF
    End If
    If left_part Then
        '������
        orig_pic.Line (orig_width \ 2, 0)-(orig_width - 1, orig_height - 1), BGColor, BF
    End If
    If right_part Then
        '�E����
        orig_pic.Line (0, 0)-(orig_width \ 2 - 1, orig_height - 1), BGColor, BF
    End If
    If tleft_part Then
        '����
        For i = 0 To orig_width - 1
            orig_pic.Line (i, orig_height - 1 - i)-(i, orig_height - 1), BGColor, B
        Next
    End If
    If tright_part Then
        '�E��
        For i = 0 To orig_width - 1
            orig_pic.Line (i, i)-(i, orig_height - 1), BGColor, B
        Next
    End If
    If bleft_part Then
        '����
        For i = 0 To orig_width - 1
            orig_pic.Line (i, 0)-(i, i), BGColor, B
        Next
    End If
    If bright_part Then
        '�E��
        For i = 0 To orig_width - 1
            orig_pic.Line (i, 0)-(i, orig_height - 1 - i), BGColor, B
        Next
    End If
    
    '�������
    If is_monotone Or is_sepia _
        Or is_sunset Or is_water _
        Or is_colorfilter _
        Or bright_count > 0 Or dark_count > 0 _
        Or negpos Or is_sil _
        Or vrev Or hrev _
        Or angle <> 0 _
    Then
        '�摜�̃T�C�Y���`�F�b�N
        If orig_width * orig_height Mod 4 <> 0 Then
            ErrorMessage fname & "�̉摜�T�C�Y��4�̔{���ɂȂ��Ă��܂���"
            Exit Function
        End If
        
        '�C���[�W���o�b�t�@�Ɏ�荞��
        GetImage orig_pic
        
        '����
        If is_monotone Then
            Monotone transparent
        End If
        
        '�Z�s�A
        If is_sepia Then
            Sepia transparent
        End If
        
        '�[�Ă�
        If is_sunset Then
            Sunset transparent
        End If
        
        '����
        If is_water Then
            Water transparent
        End If
        
        '�V���G�b�g
        If is_sil Then
            Silhouette
        End If
        
        '�l�K�|�W���]
        If negpos Then
            NegPosReverse transparent
        End If
        
        '�t�B���^
        If is_colorfilter Then
            If trans_par < 0 Then
                trans_par = 0.5
            End If
            ColorFilter fcolor, trans_par, transparent
        End If
        
        '�� (���i�w��\)
        For i = 1 To bright_count
            Bright transparent
        Next
        
        '�� (���i�w��\)
        For i = 1 To dark_count
            Dark transparent
        Next
        
        '���E���]
        If vrev Then
            VReverse
        End If
        
        '�㉺���]
        If hrev Then
            HReverse
        End If
        
        '��]
        If angle <> 0 Then
            '�O��̉�]�p��90�x�̔{�����ǂ����ŕ`��̍ۂ̍œK���g�p�ۂ����߂�
            '(�A���ŉ�]������ꍇ�ɕ`�摬�x�����ɂ��邽��)
            Rotate angle, last_angle Mod 90 <> 0
        End If
        
        '�ύX�������e���C���[�W�ɕϊ�
        SetImage orig_pic
        
        '�o�b�t�@��j��
        ClearImage
    End If
    last_angle = angle
    
EditedPicture:
    
    '�N���b�s���O����
    If dw = DEFAULT_LEVEL Then
        dw = orig_width
    End If
    If dh = DEFAULT_LEVEL Then
        dh = orig_height
    End If
    If permanent Then
        '�w�i�`��̏ꍇ�A�Z���^�����O�̓}�b�v������
        If dx = DEFAULT_LEVEL Then
            dx = (MapPWidth - dw) \ 2
        End If
        If dy = DEFAULT_LEVEL Then
            If MapFileName = "" Then
                dy = (32 * 15 - dh) \ 2
            Else
                dy = (MapPHeight - dh) \ 2
            End If
        End If
    Else
        '���j�b�g��ŉ摜�̃Z���^�����O���s�����Ƃ��Ӑ}���Ă���
        '�ꍇ�͏C�����K�v
        If InStr(fname, "EFFECT_") > 0 _
            Or InStr(fname, "�X�y�V�����p���[\") > 0 _
            Or InStr(fname, "���_�R�}���h\") > 0 _
        Then
            If dx = DEFAULT_LEVEL Then
                dx = (MainPWidth - dw) \ 2
                If MainWidth Mod 2 = 0 Then
                    dx = dx - 16
                End If
            End If
            If dy = DEFAULT_LEVEL Then
                dy = (MainPHeight - dh) \ 2
                If MainHeight Mod 2 = 0 Then
                    dy = dy - 16
                End If
            End If
        Else
            '�ʏ�`��̏ꍇ�A�Z���^�����O�͉�ʒ�����
            If dx = DEFAULT_LEVEL Then
                dx = (MainPWidth - dw) \ 2
            End If
            If dy = DEFAULT_LEVEL Then
                dy = (MainPHeight - dh) \ 2
            End If
        End If
    End If
    
    '�`��悪��ʊO�̏ꍇ��`��T�C�Y��0�̏ꍇ�͉摜�̃��[�h�݂̂��s��
    With pic
        If dx >= .width Or dy >= .Height _
            Or dx + dw <= 0 Or dy + dh <= 0 _
            Or dw <= 0 Or dh <= 0 _
        Then
            load_only = True
        End If
    End With
    
    '�`����œK�����邽�߁A�`����@���ׂ��������Ă���B
    '�`����@�͈ȉ��̒ʂ�B
    '(1) BitBlt�ł��̂܂ܕ`�� (�g�又���Ȃ��A���ߏ����Ȃ�)
    '(2) �g��摜������Ă���o�b�t�@�����O���ĕ`�� (�g�又������A���ߏ����Ȃ�)
    '(3) �g��摜����炸��StretchBlt�Œ��ڊg��`�� (�g�又������A���ߏ����Ȃ�)
    '(4) TransparentBlt�Ŋg�哧�ߕ`�� (�g�又������A���ߏ�������)
    '(5) ���摜�����̂܂ܓ��ߕ`�� (�g�又���Ȃ��A���ߏ�������)
    '(6) �g��摜������Ă���o�b�t�@�����O���ē��ߕ`�� (�g�又������A���ߏ�������)
    '(7) �g��摜������Ă���o�b�t�@�����O�����ɓ��ߕ`�� (�g�又������A���ߏ�������)
    '(8) �g��摜����炸��StretchBlt�Œ��ڊg�哧�ߕ`�� (�g�又������A���ߏ�������)
    
    '��ʂɕ`�悷��
    If Not transparent _
        And dw = orig_width And dh = orig_height _
    Then
        '���摜�����̂܂ܕ`��
        
        '�`����L�����Z���H
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '�摜��`���ɕ`��
        ret = BitBlt(pic.hDC, _
            dx, dy, dw, dh, _
            orig_pic.hDC, 0, 0, _
            SRCCOPY)
    ElseIf KeepStretchedImage _
        And Not transparent _
        And (Not found_orig Or load_only) _
        And dw <= 480 And dh <= 480 _
    Then
        '�g��摜���쐬���A�o�b�t�@�����O���ĕ`��
        
        '�g��摜�Ɏg�p����picBuf��I��
        i = GetPicBuf(display_byte_pixel * dw * dh)
        PicBufFname(i) = fname
        PicBufIsMask(i) = False
        PicBufOption(i) = pic_option
        PicBufOption2(i) = pic_option2
        PicBufDW(i) = dw
        PicBufDH(i) = dh
        PicBufSX(i) = sx
        PicBufSY(i) = sy
        PicBufSW(i) = sw
        PicBufSH(i) = sh
        'Debug.Print "Use " & Format$(i) & " As Stretched"
        
        '�o�b�t�@�̏�����
        Set stretched_pic = MainForm.picBuf(i)
        With stretched_pic
            .Picture = LoadPicture("")
            .width = dw
            .Height = dh
        End With
        
        '�o�b�t�@�Ɋg�債���摜��ۑ�
        ret = StretchBlt(stretched_pic.hDC, _
            0, 0, dw, dh, _
            orig_pic.hDC, _
            0, 0, orig_width, orig_height, _
            SRCCOPY)
        
        '�`����L�����Z���H
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '�g�債���摜��`���ɕ`��
        ret = BitBlt(pic.hDC, _
            dx, dy, dw, dh, _
            stretched_pic.hDC, 0, 0, _
            SRCCOPY)
    ElseIf Not transparent Then
        '�g��摜����炸��StretchBlt�Œ��ڊg��`��
        
        '�`����L�����Z���H
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '�g�債���摜��`���ɕ`��
        ret = StretchBlt(pic.hDC, _
            dx, dy, dw, dh, _
            orig_pic.hDC, _
            0, 0, orig_width, orig_height, _
            SRCCOPY)
    ElseIf UseTransparentBlt _
        And (dw <> orig_width Or dh <> orig_height) _
        And found_orig _
        And Not load_only _
        And (dw * dh < 40000 _
            Or orig_width * orig_height > 40000) _
    Then
        'TransparentBlt�̕��������ɕ`��ł���ꍇ�Ɍ���
        'TransparentBlt���g���Ċg�哧�ߕ`��
        
        '�`����L�����Z���H
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '�摜��`���ɓ��ߕ`��
        ret = TransparentBlt(pic.hDC, _
            dx, dy, dw, dh, _
            orig_pic.hDC, _
            0, 0, orig_width, orig_height, _
            BGColor)
    ElseIf dw = orig_width And dh = orig_height Then
        '���摜�����̂܂ܓ��ߕ`��
        
        '�ȑO�g�p�����}�X�N�摜�����p�\�H
        Set mask_pic = Nothing
        For i = 0 To ImageBufferSize - 1
            '�����t�@�C���H
            If PicBufFname(i) = fname Then
                '�I�v�V�����������H
                If PicBufIsMask(i) _
                    And PicBufOption2(i) = pic_option2 _
                    And PicBufDW(i) = orig_width _
                    And PicBufDH(i) = orig_height _
                    And PicBufSX(i) = sx _
                    And PicBufSX(i) = sy _
                    And PicBufSW(i) = sw _
                    And PicBufSH(i) = sh _
                Then
                    '�ȑO�g�p�����}�X�N�摜�����̂܂ܗ��p
                    UsePicBuf i
                    Set mask_pic = MainForm.picBuf(i)
                    'Debug.Print "Reuse " & Format$(i) & " As Mask"
                    Exit For
                End If
            End If
        Next
        
        If mask_pic Is Nothing Then
            '�}�X�N�摜��V�K�ɍ쐬
            
            '�}�X�N�摜�Ɏg�p����picBuf��I��
            i = GetPicBuf(display_byte_pixel * dw * dh)
            PicBufFname(i) = fname
            PicBufIsMask(i) = True
            PicBufOption(i) = ""
            PicBufOption2(i) = pic_option2
            PicBufDW(i) = orig_width
            PicBufDH(i) = orig_height
            PicBufSX(i) = sx
            PicBufSY(i) = sy
            PicBufSW(i) = sw
            PicBufSH(i) = sh
            'Debug.Print "Use " & Format$(i) & " As Mask"
            
            '�o�b�t�@�̏�����
            Set mask_pic = MainForm.picBuf(i)
            With mask_pic
                .Picture = LoadPicture("")
                .width = orig_width
                .Height = orig_height
            End With
            
            '�}�X�N�摜���쐬
            MakeMask orig_pic.hDC, mask_pic.hDC, orig_width, orig_height, BGColor
        End If
        
        '�`����L�����Z���H
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '�摜�𓧉ߕ`��
        If BGColor = vbWhite Then
            '�w�i�F����
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                mask_pic.hDC, 0, 0, _
                SRCERASE)
            
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                orig_pic.hDC, 0, 0, _
                SRCINVERT)
        Else
            '�w�i�F�����ȊO
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                mask_pic.hDC, 0, 0, _
                SRCAND)
            
            ret = BitBlt(mask_pic.hDC, _
                0, 0, dw, dh, _
                orig_pic.hDC, 0, 0, _
                SRCERASE)
            
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                mask_pic.hDC, 0, 0, _
                SRCINVERT)
            
            '�}�X�N�摜���ė��p�ł��Ȃ��̂Ńo�b�t�@���J��
            ReleasePicBuf i
        End If
    ElseIf KeepStretchedImage _
        And (Not found_orig Or load_only) _
        And dw <= 480 And dh <= 480 _
    Then
        '�g��摜���쐬���A�o�b�t�@�����O���ē��ߕ`��
        
        '�g��摜�p�Ɏg�p����picBuf��I��
        i = GetPicBuf(display_byte_pixel * dw * dh)
        PicBufFname(i) = fname
        PicBufIsMask(i) = False
        PicBufOption(i) = pic_option
        PicBufOption2(i) = pic_option2
        PicBufDW(i) = dw
        PicBufDH(i) = dh
        PicBufSX(i) = sx
        PicBufSY(i) = sy
        PicBufSW(i) = sw
        PicBufSH(i) = sh
        'Debug.Print "Use " & Format$(i) & " As Stretched"
        
        '�o�b�t�@�̏�����
        Set stretched_pic = MainForm.picBuf(i)
        With stretched_pic
            .Picture = LoadPicture("")
            .width = dw
            .Height = dh
        End With
        
        '�o�b�t�@�Ɋg�債���摜��ۑ�
        ret = StretchBlt(stretched_pic.hDC, _
            0, 0, dw, dh, _
            orig_pic.hDC, _
            0, 0, orig_width, orig_height, _
            SRCCOPY)
        
        '�ȑO�g�p�����g��}�X�N�摜�����p�\�H
        Set stretched_mask_pic = Nothing
        For i = 0 To ImageBufferSize - 1
            '�����t�@�C���H
            If PicBufFname(i) = fname Then
                '�I�v�V�����������H
                If PicBufIsMask(i) _
                    And PicBufOption2(i) = pic_option2 _
                    And PicBufDW(i) = dw _
                    And PicBufDH(i) = dh _
                    And PicBufSX(i) = sx _
                    And PicBufSY(i) = sy _
                    And PicBufSW(i) = sw _
                    And PicBufSH(i) = sh _
                Then
                    '�ȑO�g�p�����g��}�X�N�摜�����̂܂ܗ��p
                    UsePicBuf i
                    Set stretched_mask_pic = MainForm.picBuf(i)
                    'Debug.Print "Reuse " & Format$(i) & " As StretchedMask"
                    Exit For
                End If
            End If
        Next
        
        If stretched_mask_pic Is Nothing Then
            '�g��}�X�N�摜��V�K�ɍ쐬
            
            '�}�X�N�摜�p�̗̈��������
            Set mask_pic = MainForm.picTmp
            With mask_pic
                .Picture = LoadPicture("")
                .width = orig_width
                .Height = orig_height
            End With
            
            '�}�X�N�摜���쐬
            MakeMask orig_pic.hDC, mask_pic.hDC, orig_width, orig_height, BGColor
            
            '�g��}�X�N�摜�Ɏg�p����picBuf��I��
            i = GetPicBuf(display_byte_pixel * orig_width * orig_height)
            PicBufFname(i) = fname
            PicBufIsMask(i) = True
            PicBufOption(i) = ""
            PicBufOption2(i) = pic_option2
            PicBufDW(i) = dw
            PicBufDH(i) = dh
            PicBufSX(i) = sx
            PicBufSY(i) = sy
            PicBufSW(i) = sw
            PicBufSH(i) = sh
            'Debug.Print "Use " & Format$(i) & " As StretchedMask"
            
            '�o�b�t�@��������
            Set stretched_mask_pic = MainForm.picBuf(i)
            With stretched_mask_pic
                .Picture = LoadPicture("")
                .width = dw
                .Height = dh
            End With
            
            '�o�b�t�@�Ɋg�債���}�X�N�摜��ۑ�
            ret = StretchBlt(stretched_mask_pic.hDC, _
                0, 0, dw, dh, _
                mask_pic.hDC, _
                0, 0, orig_width, orig_height, _
                SRCCOPY)
        End If
        
        '�`����L�����Z���H
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '�摜�𓧉ߕ`��
        If BGColor = vbWhite Then
            '�w�i�F����
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_mask_pic.hDC, 0, 0, _
                SRCERASE)
            
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_pic.hDC, 0, 0, _
                SRCINVERT)
        Else
            '�w�i�F�����ȊO
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_mask_pic.hDC, 0, 0, _
                SRCAND)
            
            ret = BitBlt(stretched_mask_pic.hDC, _
                0, 0, dw, dh, _
                stretched_pic.hDC, 0, 0, _
                SRCERASE)
            
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_mask_pic.hDC, 0, 0, _
                SRCINVERT)
            
            '�g��}�X�N�摜���ė��p�ł��Ȃ��̂Ńo�b�t�@���J��
            ReleasePicBuf i
        End If
    ElseIf dw <= 480 And dh <= 480 Then
        '�g��摜���쐬������A�o�b�t�@�����O�����ɓ��ߕ`��
        
        '�g��摜�p�̗̈���쐬
        Set stretched_pic = MainForm.picStretchedTmp(0)
        With stretched_pic
            .width = dw
            .Height = dh
        End With
        
        '�o�b�t�@�Ɋg�債���摜��ۑ�
        ret = StretchBlt(stretched_pic.hDC, _
            0, 0, dw, dh, _
            orig_pic.hDC, _
            0, 0, orig_width, orig_height, _
            SRCCOPY)
        
        '�ȑO�g�p�����}�X�N�摜�����p�\�H
        Set mask_pic = Nothing
        For i = 0 To ImageBufferSize - 1
            '�����t�@�C���H
            If PicBufFname(i) = fname Then
                '�I�v�V�����������H
                If PicBufIsMask(i) _
                    And PicBufOption2(i) = pic_option2 _
                    And PicBufDW(i) = orig_width _
                    And PicBufDH(i) = orig_height _
                    And PicBufSX(i) = sx _
                    And PicBufSX(i) = sy _
                    And PicBufSW(i) = sw _
                    And PicBufSH(i) = sh _
                Then
                    '�ȑO�g�p�����}�X�N�摜�����̂܂ܗ��p
                    UsePicBuf i
                    Set mask_pic = MainForm.picBuf(i)
                    'Debug.Print "Reuse " & Format$(i) & " As Mask"
                    Exit For
                End If
            End If
        Next
        
        If mask_pic Is Nothing Then
            '�V�K�Ƀ}�X�N�摜�쐬
            
            '�}�X�N�摜�Ɏg�p����picBuf��I��
            i = GetPicBuf(display_byte_pixel * orig_width * orig_height)
            PicBufFname(i) = fname
            PicBufIsMask(i) = True
            PicBufOption(i) = ""
            PicBufOption2(i) = pic_option2
            PicBufDW(i) = orig_width
            PicBufDH(i) = orig_height
            PicBufSX(i) = sx
            PicBufSY(i) = sy
            PicBufSW(i) = sw
            PicBufSH(i) = sh
            'Debug.Print "Use " & Format$(i) & " As Mask"
            
            '�o�b�t�@��������
            Set mask_pic = MainForm.picBuf(i)
            With mask_pic
                .width = orig_width
                .Height = orig_height
            End With
            
            '�}�X�N�摜���쐬
            MakeMask orig_pic.hDC, mask_pic.hDC, orig_width, orig_height, BGColor
        End If
        
        '�g��}�X�N�摜�p�̗̈���쐬
        Set stretched_mask_pic = MainForm.picStretchedTmp(1)
        With stretched_mask_pic
            .Picture = LoadPicture("")
            .width = dw
            .Height = dh
        End With
        
        '�}�X�N�摜���g�債�Ċg��}�X�N�摜���쐬
        ret = StretchBlt(stretched_mask_pic.hDC, _
            0, 0, dw, dh, _
            mask_pic.hDC, _
            0, 0, orig_width, orig_height, _
            SRCCOPY)
        
        '�`����L�����Z���H
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '�摜�𓧉ߕ`��
        If BGColor = vbWhite Then
            '�w�i�F����
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_mask_pic.hDC, 0, 0, _
                SRCERASE)
            
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_pic.hDC, 0, 0, _
                SRCINVERT)
        Else
            '�w�i�F�����ȊO
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_mask_pic.hDC, 0, 0, _
                SRCAND)
            
            ret = BitBlt(stretched_mask_pic.hDC, _
                0, 0, dw, dh, _
                stretched_pic.hDC, 0, 0, _
                SRCERASE)
            
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_mask_pic.hDC, 0, 0, _
                SRCINVERT)
        End If
        
        '�g�p�����ꎞ�摜�̈���J��
        With MainForm.picStretchedTmp(0)
            .Picture = LoadPicture("")
            .width = 32
            .Height = 32
        End With
        With MainForm.picStretchedTmp(1)
            .Picture = LoadPicture("")
            .width = 32
            .Height = 32
        End With
    Else
        '�g��摜���쐬�����AStretchBlt�Œ��ڊg�哧�ߕ`��
        
        '�ȑO�g�p�����}�X�N�摜�����p�\�H
        Set mask_pic = Nothing
        For i = 0 To ImageBufferSize - 1
            '�����t�@�C���H
            If PicBufFname(i) = fname Then
                '�I�v�V�����������H
                If PicBufIsMask(i) _
                    And PicBufOption2(i) = pic_option2 _
                    And PicBufDW(i) = orig_width _
                    And PicBufDH(i) = orig_height _
                    And PicBufSX(i) = sx _
                    And PicBufSX(i) = sy _
                    And PicBufSW(i) = sw _
                    And PicBufSH(i) = sh _
                Then
                    '�ȑO�g�p�����}�X�N�摜�����̂܂ܗ��p
                    UsePicBuf i
                    Set mask_pic = MainForm.picBuf(i)
                    'Debug.Print "Reuse " & Format$(i) & " As Mask"
                    Exit For
                End If
            End If
        Next
        
        If mask_pic Is Nothing Then
            '�V�K�Ƀ}�X�N�摜�쐬
            
            '�}�X�N�摜�Ɏg�p����picBuf��I��
            i = GetPicBuf(display_byte_pixel * orig_width * orig_height)
            PicBufFname(i) = fname
            PicBufIsMask(i) = True
            PicBufOption(i) = ""
            PicBufOption2(i) = pic_option2
            PicBufDW(i) = orig_width
            PicBufDH(i) = orig_height
            PicBufSX(i) = sx
            PicBufSY(i) = sy
            PicBufSW(i) = sw
            PicBufSH(i) = sh
            'Debug.Print "Use " & Format$(i) & " As Mask"
            
            '�o�b�t�@��������
            Set mask_pic = MainForm.picBuf(i)
            With mask_pic
                .width = orig_width
                .Height = orig_height
            End With
            
            '�}�X�N�摜���쐬
            MakeMask orig_pic.hDC, mask_pic.hDC, orig_width, orig_height, BGColor
        End If
        
        '�`����L�����Z���H
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '�摜�𓧉ߕ`��
        If BGColor = vbWhite Then
            '�w�i�F����
            ret = StretchBlt(pic.hDC, _
                dx, dy, dw, dh, _
                mask_pic.hDC, _
                0, 0, orig_width, orig_height, _
                SRCERASE)
            
            ret = StretchBlt(pic.hDC, _
                dx, dy, dw, dh, _
                orig_pic.hDC, _
                0, 0, orig_width, orig_height, _
                SRCINVERT)
        Else
            '�w�i�F�����ȊO
            ret = StretchBlt(pic.hDC, _
                dx, dy, dw, dh, _
                mask_pic.hDC, _
                0, 0, orig_width, orig_height, _
                SRCAND)
            
            ret = BitBlt(mask_pic.hDC, _
                0, 0, orig_width, orig_width, _
                orig_pic.hDC, 0, 0, _
                SRCERASE)
            
            ret = StretchBlt(pic.hDC, _
                dx, dy, dw, dh, _
                mask_pic.hDC, _
                0, 0, orig_width, orig_height, _
                SRCINVERT)
            
            '�}�X�N�摜���ė��p�ł��Ȃ��̂Ńo�b�t�@���J��
            ReleasePicBuf i
        End If
    End If
    
DrewPicture:
    
    If permanent Then
        '�w�i�ւ̕`������
        IsMapDirty = True
        With MainForm
            '�}�X�N����w�i�摜��ʂɂ��摜��`������
            ret = BitBlt(.picMaskedBack.hDC, _
                dx, dy, dw, dh, _
                pic.hDC, dx, dy, _
                SRCCOPY)
            For i = dx \ 32 To (dx + dw - 1) \ 32
                For j = dy \ 32 To (dy + dh - 1) \ 32
                    ret = BitBlt(.picMaskedBack.hDC, _
                        32 * i, 32 * j, 32, 32, _
                        .picMask.hDC, 0, 0, SRCAND)
                    ret = BitBlt(.picMaskedBack.hDC, _
                        32 * i, 32 * j, 32, 32, _
                        .picMask2.hDC, 0, 0, SRCINVERT)
                Next
            Next
        End With
    ElseIf Not on_msg_window And Not on_status_window Then
        '�\���摜����������ۂɎg���`��̈��ݒ�
        PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(dx, 0))
        PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(dy, 0))
        PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(dx + dw, MainPWidth - 1))
        PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(dy + dh, MainPHeight - 1))
        
        IsPictureDrawn = True
        IsPictureVisible = True
        IsCursorVisible = False
        
        If keep_picture Then
            'picMain(1)�ɂ��`��
            ret = BitBlt(MainForm.picMain(1).hDC, _
                dx, dy, dw, dh, _
                pic.hDC, dx, dy, _
                SRCCOPY)
        End If
    End If
    
    DrawPicture = True
End Function

'�摜�o�b�t�@���쐬
Public Sub MakePicBuf()
Dim i As Integer

    '�摜�o�b�t�@�p��PictureBox�𓮓I�ɐ�������
    With MainForm
        For i = 1 To ImageBufferSize - 1
            Load .picBuf(i)
        Next
    End With
    
    '�摜�o�b�t�@�Ǘ��p�z���������
    ReDim PicBufDate(ImageBufferSize)
    ReDim PicBufSize(ImageBufferSize)
    ReDim PicBufFname(ImageBufferSize - 1)
    ReDim PicBufOption(ImageBufferSize - 1)
    ReDim PicBufOption2(ImageBufferSize - 1)
    ReDim PicBufDW(ImageBufferSize - 1)
    ReDim PicBufDH(ImageBufferSize - 1)
    ReDim PicBufSX(ImageBufferSize - 1)
    ReDim PicBufSY(ImageBufferSize - 1)
    ReDim PicBufSW(ImageBufferSize - 1)
    ReDim PicBufSH(ImageBufferSize - 1)
    ReDim PicBufIsMask(ImageBufferSize - 1)
End Sub

'�g�p�\�ȉ摜�o�b�t�@������
Private Function GetPicBuf(Optional ByVal buf_size As Long) As Integer
Dim total_size As Long, oldest_buf As Integer, used_buf_num As Integer
Dim i As Integer, tmp As Long
    
    '�摜�o�b�t�@�̑��T�C�Y�y�юg�p����Ă���o�b�t�@���𒲂ׂ�
    total_size = buf_size
    For i = 0 To ImageBufferSize - 1
        total_size = total_size + PicBufSize(i)
        If PicBufFname(i) <> "" Then
            used_buf_num = used_buf_num + 1
        End If
    Next
    
    '���T�C�Y��MaxImageBufferByteSize�𒴂��Ă��܂��ꍇ�͑��T�C�Y��
    'MaxImageBufferByteSize�ȉ��ɂȂ�܂Ńo�b�t�@���J������B
    '��������x�̕`��ōő��5���̃o�b�t�@���g���邽�߁A�ŐV��4��
    '�o�b�t�@�̓L�[�v���Ă����B
    Do While total_size > MaxImageBufferByteSize And used_buf_num > 4
        '�ł������Ԏg���Ă��Ȃ��o�b�t�@��T��
        tmp = 100000000
        For i = 0 To ImageBufferSize - 1
            If PicBufFname(i) <> "" Then
                If PicBufDate(i) < tmp Then
                    oldest_buf = i
                    tmp = PicBufDate(i)
                End If
            End If
        Next
        
        '�o�b�t�@���J��
        ReleasePicBuf oldest_buf
        used_buf_num = used_buf_num - 1
        
        '���T�C�Y��������������
        total_size = total_size - PicBufSize(oldest_buf)
        PicBufSize(oldest_buf) = 0
    Loop
    
    '�ł������Ԏg���Ă��Ȃ��o�b�t�@��T��
    GetPicBuf = 0
    For i = 1 To ImageBufferSize - 1
        If PicBufDate(i) < PicBufDate(GetPicBuf) Then
            GetPicBuf = i
        End If
    Next
    
    '�摜�̃T�C�Y���L�^���Ă���
    PicBufSize(GetPicBuf) = buf_size
    
    '�g�p���邱�Ƃ��L�^����
    UsePicBuf GetPicBuf
End Function

'�摜�o�b�t�@���J������
Private Sub ReleasePicBuf(ByVal idx As Integer)
    PicBufFname(idx) = ""
    With MainForm.picBuf(idx)
        .Picture = LoadPicture("")
        .width = 32
        .Height = 32
    End With
End Sub

'�摜�o�b�t�@�̎g�p�L�^������
Private Sub UsePicBuf(ByVal idx As Integer)
    PicBufDateCount = PicBufDateCount + 1
    PicBufDate(idx) = PicBufDateCount
End Sub


' === ������`��Ɋւ��鏈�� ===

'���C���E�B���h�E�ɕ������\������
Public Sub DrawString(msg As String, ByVal X As Long, ByVal Y As Long, _
    Optional ByVal without_cr As Boolean)
Dim tx As Integer, ty As Integer
Dim prev_cx As Integer
Dim pic As PictureBox
Dim sf As StdFont
Static font_smoothing As Long
Static init_draw_string As Boolean

    If PermanentStringMode Then
        '�w�i��������
        Set pic = MainForm.picBack
        '�t�H���g�ݒ��ύX
        With MainForm.picBack
            .ForeColor = MainForm.picMain(0).ForeColor
            If .Font.Name <> MainForm.picMain(0).Font.Name Then
                Set sf = New StdFont
                sf.Name = MainForm.picMain(0).Font.Name
                Set .Font = sf
            End If
            .Font.Size = MainForm.picMain(0).Font.Size
            .Font.Bold = MainForm.picMain(0).Font.Bold
            .Font.Italic = MainForm.picMain(0).Font.Italic
        End With
        With MainForm.picMaskedBack
            .ForeColor = MainForm.picMain(0).ForeColor
            If .Font.Name <> MainForm.picMain(0).Font.Name Then
                Set sf = New StdFont
                sf.Name = MainForm.picMain(0).Font.Name
                Set .Font = sf
            End If
            .Font.Size = MainForm.picMain(0).Font.Size
            .Font.Bold = MainForm.picMain(0).Font.Bold
            .Font.Italic = MainForm.picMain(0).Font.Italic
        End With
    Else
        '�ʏ�̏�������
        Set pic = MainForm.picMain(0)
        SaveScreen
    End If
    
    '�t�H���g���X���[�W���O�\������Ă��邩�Q��
    If Not init_draw_string Then
        Call GetSystemParametersInfo(SPI_GETFONTSMOOTHING, 0, font_smoothing, 0)
        init_draw_string = True
    End If
    
    '�t�H���g���X���[�W���O����悤�ɐݒ�
    If font_smoothing = 0 Then
        Call SetSystemParametersInfo(SPI_SETFONTSMOOTHING, 1, 0, 0)
    End If
    
    With pic
        '���݂�X�ʒu���L�^���Ă���
        prev_cx = .CurrentX
        
        '�������ݐ�̍��W�����߂�
        If HCentering Then
            .CurrentX = (.width - .TextWidth(msg)) \ 2
        Else
            If X <> DEFAULT_LEVEL Then
                .CurrentX = X
            End If
        End If
        If VCentering Then
            .CurrentY = (.Height - .TextHeight(msg)) \ 2
        Else
            If Y <> DEFAULT_LEVEL Then
                .CurrentY = Y
            End If
        End If
        tx = .CurrentX
        ty = .CurrentY
        
        If Not without_cr Then
            '���s����
            pic.Print msg
            
            '�w�i�������݂̏ꍇ
            If PermanentStringMode Then
                With MainForm.picMaskedBack
                    .CurrentX = tx
                    .CurrentY = ty
                End With
                MainForm.picMaskedBack.Print msg
                IsMapDirty = True
            End If
            
            '�ێ��I�v�V�����g�p��
            If KeepStringMode Then
                With MainForm.picMain(1)
                    .CurrentX = tx
                    .CurrentY = ty
                    .ForeColor = pic.ForeColor
                    If .Font.Name <> pic.Font.Name Then
                        Set sf = New StdFont
                        sf.Name = pic.Font.Name
                        Set .Font = sf
                    End If
                    .Font.Size = pic.Font.Size
                    .Font.Bold = pic.Font.Bold
                    .Font.Italic = pic.Font.Italic
                End With
                MainForm.picMain(1).Print msg
            End If
            
            '����̏������݂̂��߁AX���W�ʒu��ݒ肵����
            If X <> DEFAULT_LEVEL Then
                .CurrentX = X
            Else
                .CurrentX = prev_cx
            End If
        Else
            '���s�Ȃ�
            pic.Print msg;
            
            '�w�i�������݂̏ꍇ
            If PermanentStringMode Then
                With MainForm.picMaskedBack
                    .CurrentX = tx
                    .CurrentY = ty
                End With
                MainForm.picMaskedBack.Print msg;
                IsMapDirty = True
            End If
            
            '�ێ��I�v�V�����g�p��
            If KeepStringMode Then
                With MainForm.picMain(1)
                    .CurrentX = tx
                    .CurrentY = ty
                End With
                MainForm.picMain(1).Print msg;
            End If
        End If
    End With
    
    '�t�H���g�̃X���[�W���O�Ɋւ���ݒ�����ɖ߂�
    If font_smoothing = 0 Then
        Call SetSystemParametersInfo(SPI_SETFONTSMOOTHING, 0, 0, 0)
    End If
    
    If Not PermanentStringMode Then
        IsPictureVisible = True
        
        PaintedAreaX1 = 0
        PaintedAreaY1 = 0
        PaintedAreaX2 = MainPWidth - 1
        PaintedAreaY2 = MainPHeight - 1
    End If
End Sub

'���C���E�B���h�E�ɕ������\�� (�V�X�e�����b�Z�[�W)
Public Sub DrawSysString(ByVal X As Integer, ByVal Y As Integer, msg As String, _
    Optional ByVal without_refresh As Boolean)
Dim prev_color As Long
Dim prev_name As String
Dim prev_size As Integer
Dim is_bold As Boolean
Dim is_italic As Boolean
Dim sf As StdFont
    
    '�\���ʒu����ʊO�H
    If X < MapX - MainWidth \ 2 Or MapX + MainWidth \ 2 < X _
        Or Y < MapY - MainHeight \ 2 Or MapY + MainHeight \ 2 < Y _
    Then
        Exit Sub
    End If
    
    SaveScreen
    
    With MainForm.picMain(0)
        '���݂̃t�H���g�ݒ��ۑ�
        prev_color = .ForeColor
        prev_size = .Font.Size
        prev_name = .Font.Name
        is_bold = .Font.Bold
        is_italic = .Font.Italic
        
        '�t�H���g�ݒ���V�X�e���p�ɐ؂�ւ�
        .ForeColor = vbBlack
        .FontTransparent = False
        If .Font.Name <> "�l�r �o����" Then
            Set sf = New StdFont
            sf.Name = "�l�r �o����"
            Set .Font = sf
        End If
        With .Font
            If BattleAnimation Then
                .Size = 9
                .Bold = True
            Else
                .Size = 8
                .Bold = False
            End If
            .Italic = False
        End With
        
        '���b�Z�[�W�̏�������
        .CurrentX = MapToPixelX(X) + (32 - .TextWidth(msg)) \ 2 - 1
        .CurrentY = MapToPixelY(Y + 1) - .TextHeight(msg)
        MainForm.picMain(0).Print msg
        
        '�t�H���g�ݒ�����ɖ߂�
        .ForeColor = prev_color
        .FontTransparent = True
        If .Font.Name <> prev_name Then
            Set sf = New StdFont
            sf.Name = prev_name
            Set .Font = sf
        End If
        With .Font
            .Size = prev_size
            .Bold = is_bold
            .Italic = is_italic
        End With
        
        '�\�����X�V
        If Not without_refresh Then
            .Refresh
        End If
        
        PaintedAreaX1 = MinLng(PaintedAreaX1, MapToPixelX(X) - 4)
        PaintedAreaY1 = MaxLng(PaintedAreaY1, MapToPixelY(Y) + 16)
        PaintedAreaX2 = MinLng(PaintedAreaX2, MapToPixelX(X) + 36)
        PaintedAreaY2 = MaxLng(PaintedAreaY2, MapToPixelY(Y) + 32)
    End With
End Sub


' === �摜�����Ɋւ��鏈�� ===

'�`�悵���摜�������ł���悤�Ɍ��摜��ۑ�����
Public Sub SaveScreen()
Dim ret As Long

    If Not ScreenIsSaved Then
        '�摜��picMain(1)�ɕۑ�
        With MainForm
             ret = BitBlt(.picMain(1).hDC, 0, 0, MainPWidth, MainPHeight, _
                 .picMain(0).hDC, 0, 0, SRCCOPY)
        End With
        ScreenIsSaved = True
    End If
End Sub

'�`�悵���O���t�B�b�N������
Public Sub ClearPicture()
Dim pawidth As Integer, paheight As Integer
Dim ret As Long
    
    If Not ScreenIsSaved Then
        Exit Sub
    End If
    
    IsPictureVisible = False
    IsCursorVisible = False
    
    pawidth = PaintedAreaX2 - PaintedAreaX1 + 1
    paheight = PaintedAreaY2 - PaintedAreaY1 + 1
    
    If pawidth < 1 Or paheight < 1 Then
        Exit Sub
    End If
    
    With MainForm
        ret = BitBlt(.picMain(0).hDC, _
            PaintedAreaX1, PaintedAreaY1, pawidth, paheight, _
            .picMain(1).hDC, _
            PaintedAreaX1, PaintedAreaY1, _
            SRCCOPY)
    End With
End Sub

'�`�悵���O���t�B�b�N�̈ꕔ������
Public Sub ClearPicture2(ByVal x1 As Long, ByVal y1 As Long, _
    ByVal x2 As Long, ByVal y2 As Long)
Dim ret As Long
    
    If Not ScreenIsSaved Then
        Exit Sub
    End If
    
    With MainForm
        ret = BitBlt(.picMain(0).hDC, _
            x1, y1, x2 - x1 + 1, y2 - y1 + 1, _
            .picMain(1).hDC, _
            x1, y1, _
            SRCCOPY)
    End With
End Sub


' === ��ʃ��b�N�Ɋւ��鏈�� ===

'�f�t�h�����b�N���A�v���C���[����̓��͂𖳌��ɂ���
Public Sub LockGUI()
    IsGUILocked = True
    With MainForm
        .VScroll.Enabled = False
        .HScroll.Enabled = False
    End With
End Sub

'�f�t�h�̃��b�N���������A�v���C���[����̓��͂�L���ɂ���
Public Sub UnlockGUI()
    IsGUILocked = False
    With MainForm
        .VScroll.Enabled = True
        .HScroll.Enabled = True
    End With
End Sub


' === �}�E�X�J�[�\���̎����ړ��Ɋւ��鏈�� ===

'���݂̃}�E�X�J�[�\���̈ʒu���L�^
Public Sub SaveCursorPos()
Dim PT As POINTAPI

    GetCursorPos PT
    PrevCursorX = PT.X
    PrevCursorY = PT.Y
    NewCursorX = 0
    NewCursorY = 0
End Sub

'�}�E�X�J�[�\�����ړ�����
Public Sub MoveCursorPos(cursor_mode As String, Optional ByVal t As Unit)
Dim tx As Long, ty As Long, i As Long, num As Long
Dim ret As Long, prev_lock As Boolean
Dim PT As POINTAPI
    
    '�}�E�X�J�[�\���̈ʒu������
    GetCursorPos PT
    
    '���݂̈ʒu���L�^���Ă���
    If PrevCursorX = 0 _
        And cursor_mode <> "���b�Z�[�W�E�B���h�E" _
    Then
        SaveCursorPos
    End If
    
    '�J�[�\�������ړ�
    If t Is Nothing Then
        If cursor_mode = "���b�Z�[�W�E�B���h�E" Then
            '���b�Z�[�W�E�B���h�E�܂ňړ�
            With frmMessage
                If PT.X < (.Left + 0.05 * .width) \ Screen.TwipsPerPixelX Then
                    tx = (.Left + 0.05 * .width) \ Screen.TwipsPerPixelX
                ElseIf PT.X > (.Left + 0.95 * .width) \ Screen.TwipsPerPixelX Then
                    tx = (.Left + 0.95 * .width) \ Screen.TwipsPerPixelX
                Else
                    tx = PT.X
                End If
                
                If PT.Y < (.Top + .Height) \ Screen.TwipsPerPixelY _
                        - .ScaleHeight + .picMessage.Top _
                Then
                    ty = (.Top + .Height) \ Screen.TwipsPerPixelY _
                        - .ScaleHeight + .picMessage.Top
                ElseIf PT.Y > (.Top + 0.9 * .Height) \ Screen.TwipsPerPixelY Then
                    ty = (.Top + 0.9 * .Height) \ Screen.TwipsPerPixelY
                Else
                    ty = PT.Y
                End If
            End With
        Else
            '���X�g�{�b�N�X�܂ňړ�
            With frmListBox
                If PT.X < (.Left + 0.1 * .width) \ Screen.TwipsPerPixelX Then
                    tx = (.Left + 0.1 * .width) \ Screen.TwipsPerPixelX
                ElseIf PT.X > (.Left + 0.9 * .width) \ Screen.TwipsPerPixelX Then
                    tx = (.Left + 0.9 * .width) \ Screen.TwipsPerPixelX
                Else
                    tx = PT.X
                End If
                
                '�I������A�C�e��
                If cursor_mode = "����I��" Then
                    '����I���̏ꍇ�͑I���\�ȍŌ�̃A�C�e����
                    i = .lstItems.ListCount
                    Do
                        If Not ListItemFlag(i) And InStr(.lstItems.list(i), "����U���F") = 0 Then
                            Exit Do
                        End If
                        i = i - 1
                    Loop While i > 1
                Else
                    '�����łȂ���΍ŏ��̃A�C�e����
                    i = .lstItems.TopIndex + 1
                End If
                
                ty = .Top \ Screen.TwipsPerPixelY _
                    + .Height \ Screen.TwipsPerPixelY - .ScaleHeight _
                    + .lstItems.Top _
                    + 16 * (i - .lstItems.TopIndex) - 8
            End With
        End If
    Else
        '���j�b�g��܂ňړ�
        With MainForm
'MOD START 240a
'            If MainWidth = 15 Then
'                tx = .Left \ Screen.TwipsPerPixelX _
'                    + 32 * (t.X - (MapX - MainWidth \ 2)) + 24
'                ty = .Top \ Screen.TwipsPerPixelY _
'                    + .Height \ Screen.TwipsPerPixelY - .ScaleHeight _
'                    + 32 * (t.Y - (MapY - MainHeight \ 2)) + 20
'            Else
'                tx = .Left \ Screen.TwipsPerPixelX _
'                    + 32 * (t.X - (MapX - MainWidth \ 2)) - 14
'                ty = .Top \ Screen.TwipsPerPixelY _
'                    + .Height \ Screen.TwipsPerPixelY - .ScaleHeight _
'                    + 32 * (t.Y - (MapY - MainHeight \ 2)) + 16
'            End If
            If NewGUIMode Then
                tx = .Left \ Screen.TwipsPerPixelX _
                    + 32 * (t.X - (MapX - MainWidth \ 2)) + 4
                ty = .Top \ Screen.TwipsPerPixelY _
                    + .Height \ Screen.TwipsPerPixelY - .ScaleHeight _
                    + 32 * (t.Y - (MapY - MainHeight \ 2)) + 16
            Else
                tx = .Left \ Screen.TwipsPerPixelX _
                    + 32 * (t.X - (MapX - MainWidth \ 2)) + 24
                ty = .Top \ Screen.TwipsPerPixelY _
                    + .Height \ Screen.TwipsPerPixelY - .ScaleHeight _
                    + 32 * (t.Y - (MapY - MainHeight \ 2)) + 20
            End If
'MOD  END  240a
        End With
    End If
    
    '����ɕ����Ĉړ����邩�v�Z
    num = Sqr((tx - PT.X) ^ 2 + (ty - PT.Y) ^ 2) \ 25 + 1
    
    '�J�[�\�����ړ�
    prev_lock = IsGUILocked
    IsGUILocked = True
    IsStatusWindowDisabled = True
    For i = 1 To num
        ret = SetCursorPos((tx * i + PT.X * (num - i)) \ num, _
            (ty * i + PT.Y * (num - i)) \ num)
        Sleep 10
    Next
    DoEvents
    IsStatusWindowDisabled = False
    IsGUILocked = prev_lock
    
    '�V�����J�[�\���ʒu���L�^
    If NewCursorX = 0 Then
        NewCursorX = tx
        NewCursorY = ty
    End If
End Sub

'�}�E�X�J�[�\�������̈ʒu�ɖ߂�
Public Sub RestoreCursorPos()
Dim tx As Integer, ty As Integer, i As Integer, num As Integer
Dim ret As Long
Dim PT As POINTAPI
    
    '���j�b�g���I������Ă���΂��̏ꏊ�܂Ŗ߂�
    If Not SelectedUnit Is Nothing Then
        If SelectedUnit.Status = "�o��" Then
            MoveCursorPos "���j�b�g�I��", SelectedUnit
            Exit Sub
        End If
    End If
    
    '�߂�ׂ��ʒu���ݒ肳��Ă��Ȃ��H
    If PrevCursorX = 0 And PrevCursorY = 0 Then
        Exit Sub
    End If
    
    '���݂̃J�[�\���ʒu����
    GetCursorPos PT
    
    '�ȑO�̈ʒu�܂ŃJ�[�\�������ړ�
    With frmListBox
        tx = PrevCursorX
        ty = PrevCursorY
        
        num = Sqr((tx - PT.X) ^ 2 + (ty - PT.Y) ^ 2) \ 50 + 1
        
        For i = 1 To num
            ret = SetCursorPos((tx * i + PT.X * (num - i)) \ num, _
                (ty * i + PT.Y * (num - i)) \ num)
            DoEvents
            Sleep 10
        Next
    End With
    
    '�߂�ʒu��������
    PrevCursorX = 0
    PrevCursorY = 0
End Sub


' === �^�C�g����ʕ\���Ɋւ��鏈�� ===

'�^�C�g����ʂ�\��
Public Sub OpenTitleForm()
    Load frmTitle
    
    frmTitle.Left = (Screen.width - frmTitle.width) / 2
    frmTitle.Top = (Screen.Height - frmTitle.Height) / 2
    
    frmTitle.Show
    frmTitle.Refresh
End Sub

'�^�C�g����ʂ����
Public Sub CloseTitleForm()
    Unload frmTitle
    Set frmTitle = Nothing
End Sub


' === �uNow Loading...�v�\���Ɋւ��鏈�� ===

'�uNow Loading...�v�̉�ʂ�\��
Public Sub OpenNowLoadingForm()
    Screen.MousePointer = 11
    Load frmNowLoading
    With frmNowLoading
        .Left = (Screen.width - .width) / 2
        .Top = (Screen.Height - .Height) / 2
        .Show
        .Label1.Refresh
    End With
End Sub

'�uNow Loading...�v�̉�ʂ�����
Public Sub CloseNowLoadingForm()
    Unload frmNowLoading
    Set frmNowLoading = Nothing
    Screen.MousePointer = 0
End Sub

'�uNow Loading...�v�̃o�[���P�i�K�i�s������
Public Sub DisplayLoadingProgress()
    frmNowLoading.Progress
    DoEvents
End Sub

'�uNow Loading...�v�̃o�[�̒�����ݒ�
Public Sub SetLoadImageSize(ByVal new_size As Integer)
    With frmNowLoading
        .Value = 0
        .Size = new_size
    End With
End Sub


' === ��ʂ̉𑜓x�ύX ===

Public Sub ChangeDisplaySize(ByVal w As Integer, ByVal h As Integer)
Dim dm As DEVMODE
Dim ret As Long
Static orig_width As Integer, orig_height As Integer
    
    'DEVMODE�\���̂�������
    dm.dmSize = Len(dm)
    
    '���݂̃f�B�X�v���C�ݒ���Q��
    ret = EnumDisplaySettings(vbNullString, ENUM_CURRENT_SETTINGS, dm)
    
    If w <> 0 And h <> 0 Then
        '��ʂ̉𑜓x�� w x h �ɕύX����ꍇ
        
        '���݂̉𑜓x���L�^���Ă���
        orig_width = dm.dmPelsWidth
        orig_height = dm.dmPelsHeight
        
        If dm.dmPelsWidth = w And dm.dmPelsHeight = h Then
            '���Ɏg�p�������𑜓x�ɂȂ��Ă���΂��̂܂܏I��
            Exit Sub
        End If
        
        '��ʂ̉𑜓x�� w x h �ɕύX
        dm.dmPelsWidth = w
        dm.dmPelsHeight = h
    Else
        '��ʂ̉𑜓x�����̉𑜓x�ɖ߂��ꍇ
        
        If orig_width = 0 And orig_height Then
            '�𑜓x��ύX���Ă��Ȃ���ΏI��
            Exit Sub
        End If
        
        If dm.dmPelsWidth = orig_width And dm.dmPelsHeight = orig_width Then
            '�𑜓x���ω����Ă��Ȃ���΂��̂܂܏I��
            Exit Sub
        End If
        
        '��ʂ̉𑜓x�����ɖ߂�
        ret = ChangeDisplaySettings(vbNull, 0)
        Exit Sub
    End If
    
    '�𑜓x��ύX�\���ǂ������ׂ�
    ret = ChangeDisplaySettings(dm, CDS_TEST)
    If ret <> DISP_CHANGE_SUCCESSFUL Then
        Exit Sub
    End If
    
    '�𑜓x�����ۂɕύX����
' MOD START MARGE
'    If GetWinVersion() >= 5 Then
    If GetWinVersion() >= 501 Then
' MOD END MARGE
        ret = ChangeDisplaySettings(dm, CDS_FULLSCREEN)
    Else
        ret = ChangeDisplaySettings(dm, 0)
    End If
    Select Case ret
        Case DISP_CHANGE_SUCCESSFUL
            '�����I
            Exit Sub
        Case DISP_CHANGE_RESTART
            '�ċN�����K�v�ȏꍇ�͂�����߂Ă��Ƃ̉𑜓x�ɖ߂�
            ret = ChangeDisplaySettings(vbNull, 0)
    End Select
End Sub


' === ���̑� ===

'�G���[���b�Z�[�W��\��
Public Sub ErrorMessage(msg As String)
Dim ret As Long

    Load frmErrorMessage
    
    With frmErrorMessage
        ret = SetWindowPos(.hwnd, -1, 0, 0, 0, 0, &H3)
        .txtMessage = msg
        .Left = (Screen.width - .width) / 2
        .Top = (Screen.Height - .Height) / 2
        .Show
    End With
    
    '���C���E�B���h�E�̃N���[�Y���s����悤�Ƀ��[�_�����[�h�͎g�p���Ȃ�
    Do While frmErrorMessage.Visible
        DoEvents
        Sleep 200
    Loop
    
    Unload frmErrorMessage
    Set frmErrorMessage = Nothing
End Sub

'�f�[�^�ǂݍ��ݎ��̃G���[���b�Z�[�W��\������
Public Sub DataErrorMessage(msg As String, fname As String, ByVal line_num As Integer, _
    line_buf As String, dname As String)
Dim err_msg As String

    '�G���[�����������t�@�C�����ƍs�ԍ�
    err_msg = fname & "�F" & line_num & "�s��" & vbCr & vbLf
    
    '�G���[�����������f�[�^��
    If Len(dname) > 0 Then
        err_msg = err_msg & dname & "�̃f�[�^���s���ł��B" & vbCr & vbLf
    End If
    
    '�G���[�̌���
    If Len(msg) > 0 Then
        err_msg = err_msg & msg & vbCr & vbLf
    End If
    
    '�Ȃɂ��w�肳��Ă��Ȃ��H
    If dname = "" And msg = "" Then
        err_msg = err_msg & "�f�[�^���s���ł��B" & vbCr & vbLf
    End If
    
    '�G���[�����������f�[�^�s
    err_msg = err_msg & line_buf
    
    '�G���[���b�Z�[�W��\��
    ErrorMessage err_msg
End Sub


'�}�E�X�̉E�{�^����������Ă��邩(�L�����Z��)����
Public Function IsRButtonPressed(Optional ByVal ignore_message_wait As Boolean) As Boolean
Dim PT As POINTAPI

    '���b�Z�[�W���E�G�C�g�����Ȃ�X�L�b�v
    If Not ignore_message_wait And MessageWait = 0 Then
        IsRButtonPressed = True
        Exit Function
    End If
    
    '���C���E�C���h�E��Ń}�E�X�{�^�����������ꍇ
    If MainForm.hwnd = GetForegroundWindow Then
        GetCursorPos PT
        With MainForm
            If .Left \ Screen.TwipsPerPixelX <= PT.X _
                And PT.X <= (.Left + .width) \ Screen.TwipsPerPixelX _
                And .Top \ Screen.TwipsPerPixelY <= PT.Y _
                And PT.Y <= (.Top + .Height) \ Screen.TwipsPerPixelY _
            Then
                If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
                    '�E�{�^���ŃX�L�b�v
                    IsRButtonPressed = True
                    Exit Function
                End If
            End If
        End With
    '���b�Z�[�W�E�C���h�E��Ń}�E�X�{�^�����������ꍇ
    ElseIf frmMessage.hwnd = GetForegroundWindow Then
        GetCursorPos PT
        With frmMessage
            If .Left \ Screen.TwipsPerPixelX <= PT.X _
                And PT.X <= (.Left + .width) \ Screen.TwipsPerPixelX _
                And .Top \ Screen.TwipsPerPixelY <= PT.Y _
                And PT.Y <= (.Top + .Height) \ Screen.TwipsPerPixelY _
            Then
                If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
                    '�E�{�^���ŃX�L�b�v
                    IsRButtonPressed = True
                    Exit Function
                End If
            End If
        End With
    End If
End Function


'Telop�R�}���h�p�`�惋�[�`��
Public Sub DisplayTelop(msg As String)
    Load frmTelop
    
    With frmTelop
        FormatMessage msg
        If InStr(msg, ".") > 0 Then
            Mid$(msg, InStr(msg, ".")) = vbCr
            .Height = 1170
        Else
            .Height = 800
        End If
        
        If MainForm.Visible = True And Not MainForm.WindowState = 1 Then
            .Left = MainForm.Left + _
                (MainForm.picMain(0).width * MainForm.width \ MainForm.ScaleWidth - .width) \ 2
            .Top = MainForm.Top + (MainForm.Height - .Height) \ 2
        Else
            .Left = (Screen.width - .width) / 2
            .Top = (Screen.Height - .Height) / 2
        End If
        
        If InStr(msg, ".") > 0 Then
            Mid$(msg, InStr(msg, ".")) = vbCr
        End If
        .Label1 = msg
        .Show
        .Refresh
    End With
    
    If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
        Sleep 1000
    End If
    Unload frmTelop
End Sub

