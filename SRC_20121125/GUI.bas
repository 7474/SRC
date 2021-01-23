Attribute VB_Name = "GUI"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'ユーザーインターフェースと画面描画の処理を行うモジュール

'MainのForm
Public MainForm As Form
Public IsFlashAvailable As Boolean

' ADD START MARGE
'GUIが新バージョンか
Public NewGUIMode As Boolean
' ADD END

'マップ画面に表示できるマップのサイズ
Public MainWidth As Integer
Public MainHeight As Integer

'マップ画面のサイズ（ピクセル）
Public MainPWidth As Integer
Public MainPHeight As Integer

'マップのサイズ（ピクセル）
Public MapPWidth As Integer
Public MapPHeight As Integer

'ＨＰ・ＥＮのゲージの幅（ピクセル）
Public Const GauageWidth = 88

'現在マップウィンドウがマスク表示されているか
Public ScreenIsMasked As Boolean
'現在マップウィンドウが保存されているか
Public ScreenIsSaved As Boolean

'現在表示されているマップの座標
Public MapX As Integer
Public MapY As Integer

'ドラッグ前のマップの座標
Public PrevMapX As Integer
Public PrevMapY As Integer

'最後に押されたマウスボタン
Public MouseButton As Integer

'現在のマウスの座標
Public MouseX As Single
Public MouseY As Single

'ドラッグ前のマウスの座標
Public PrevMouseX As Single
Public PrevMouseY As Single

'カーソル位置自動変更前のマウスカーソルの座標
Private PrevCursorX As Integer
Private PrevCursorY As Integer
'カーソル位置自動変更後のマウスカーソルの座標
Private NewCursorX As Integer
Private NewCursorY As Integer

'移動前のユニットの情報
Public PrevUnitX As Integer
Public PrevUnitY As Integer
Public PrevUnitArea As String
Public PrevCommand As String

'PaintPictureで画像が描き込まれたか
Public IsPictureDrawn As Boolean
'PaintPictureで画像が描かれているか
Public IsPictureVisible As Boolean
'PaintPictureで描画した画像領域
Public PaintedAreaX1 As Integer
Public PaintedAreaY1 As Integer
Public PaintedAreaX2 As Integer
Public PaintedAreaY2 As Integer
'カーソル画像が表示されているか
Public IsCursorVisible As Boolean
'背景色
Public BGColor As Long

'画像バッファ管理用変数
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


'GUIから入力可能かどうか
Public IsGUILocked As Boolean

'リストボックス内で表示位置
Public TopItem As Integer

'メッセージウインドウにに関する情報
Private DisplayedPilot As String
Private DisplayMode As String
Private RightUnit As Unit
Private LeftUnit As Unit
Private RightUnitHPRatio As Double
Private LeftUnitHPRatio As Double
Private RightUnitENRatio As Double
Private LeftUnitENRatio As Double
Public MessageWindowIsOut As Boolean

'メッセージウィンドウの状態を保持するための変数
Private IsMessageFormVisible As Boolean
Private SavedLeftUnit As Unit
Private SavedRightUnit As Unit

'フォームがクリックされたか
Public IsFormClicked As Boolean
'フォームがモーダルか
Public IsMordal As Boolean

'メッセージ表示のウェイト
Public MessageWait As Long

'メッセージが自働送りかどうか
Public AutoMessageMode As Boolean

'PaintStringの中央表示の設定
Public HCentering As Boolean
Public VCentering As Boolean
'PaintStringの書きこみが背景に行われるかどうか
Public PermanentStringMode As Boolean
'PaintStringの書きこみが持続性かどうか
Public KeepStringMode As Boolean


'ListBox用変数
Public ListItemFlag() As Boolean
Public ListItemComment() As String
Public ListItemID() As String
Public MaxListItem As Integer


'API関数の定義

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

'StretchBltのモード設定を行う
Declare Function GetStretchBltMode Lib "gdi32" (ByVal hDC As Long) As Long
Declare Function SetStretchBltMode Lib "gdi32" (ByVal hDC As Long, _
    ByVal nStretchMode As Long) As Long

Public Const STRETCH_ANDSCANS = 1
Public Const STRETCH_ORSCANS = 2
Public Const STRETCH_DELETESCANS = 3
Public Const STRETCH_HALFTONE = 4

'透過描画
Declare Function TransparentBlt Lib "msimg32.dll" (ByVal hDC As Long, _
    ByVal X As Long, ByVal Y As Long, ByVal nWidth As Long, ByVal nHeight As Long, _
    ByVal hSrcDC As Long, ByVal xsrc As Long, ByVal ysrc As Long, ByVal nSrcWidth As Long, _
    ByVal nSrcHeight As Long, ByVal crTransparent As Long) As Long

'ウィンドウ位置の設定
Declare Function SetWindowPos Lib "user32" ( _
    ByVal hwnd As Long, ByVal hWndInsertAfter As Long, _
    ByVal X As Long, ByVal Y As Long, ByVal cx As Long, _
    ByVal cy As Long, ByVal wFlags As Long) As Long

Public Const SW_SHOWNA = 8    '非アクティブで表示

'フォームをアクティブにしないで表示
Declare Function ShowWindow Lib "user32" ( _
    ByVal hwnd As Long, ByVal nCmdShow As Long) As Long

Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Long)

'カーソル位置取得
Declare Function GetCursorPos Lib "user32" (lpPoint As POINTAPI) As Long

'ポイント構造体
Type POINTAPI
    X As Long
    Y As Long
End Type

'カーソル位置設定
Declare Function SetCursorPos Lib "user32" (ByVal X As Long, ByVal Y As Long) As Long

'キーの情報を得る
Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Long) As Integer

Public RButtonID As Long
Public LButtonID As Long

'システムメトリックスを取得するAPI
Declare Function GetSystemMetrics Lib "user32" (ByVal nIndex As Long) As Long

Public Const SM_SWAPBUTTON = 23    '左右のボタンが交換されているか否か

'現在アクティブなウィンドウを取得するAPI
Public Declare Function GetForegroundWindow Lib "user32" () As Long

'直線を描画するためのAPI
Declare Function MoveToEx Lib "gdi32" (ByVal hDC As Long, _
    ByVal X As Long, ByVal Y As Long, lpPoint As POINTAPI) As Long
Declare Function LineTo Lib "gdi32" (ByVal hDC As Long, _
    ByVal X As Long, ByVal Y As Long) As Long

'多角形を描画するAPI
Public Declare Function Polygon Lib "gdi32.dll" (ByVal hDC As Long, _
    lpPoint As POINTAPI, ByVal nCount As Long) As Long


'ディスプレイの設定を参照するAPI
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

'ディスプレイの設定を変更するためのAPI
Public Declare Function ChangeDisplaySettings Lib "user32.dll" Alias "ChangeDisplaySettingsA" _
    (lpDevMode As Any, ByVal dwFlags As Long) As Long

Public Const CDS_UPDATEREGISTRY = &H1
Public Const CDS_TEST = &H2
Public Const CDS_FULLSCREEN = &H4
Public Const DISP_CHANGE_SUCCESSFUL = 0
Public Const DISP_CHANGE_RESTART = 1

'デバイスの設定を参照するためのAPI
Public Declare Function GetDeviceCaps Lib "gdi32" _
    (ByVal hDC As Long, ByVal nIndex As Long) As Long

'ピクセル当たりのカラービット数
Private Const BITSPIXEL = 12


'システムパラメータを変更するためのAPI
Declare Function SetSystemParametersInfo Lib "user32.dll" Alias "SystemParametersInfoA" _
    (ByVal uiAction As Long, ByVal uiParam As Long, ByVal pvParam As Long, _
    ByVal fWinIni As Long) As Long

Declare Function GetSystemParametersInfo Lib "user32.dll" Alias "SystemParametersInfoA" _
    (ByVal uiAction As Long, ByVal uiParam As Long, pvParam As Long, _
    ByVal fWinIni As Long) As Long

'フォントのスムージング処理関連の定数
Public Const SPI_GETFONTSMOOTHING = 74
Public Const SPI_SETFONTSMOOTHING = 75

'ユーザープロファイルの更新を指定
Public Const SPIF_UPDATEINIFILE = &H1
'すべてのトップレベルウィンドウに変更を通知
Public Const SPIF_SENDWININICHANGE = &H2


'メインウィンドウのロードとFlashの登録を行う
Public Sub LoadMainFormAndRegisterFlash()
Dim WSHShell As Object
    
    On Error GoTo ErrorHandler
    
    'シェルからregsvr32.exeを利用して、起動ごとにSRC.exeと同じパスにある
    'FlashControl.ocxを再登録する。
    Set WSHShell = CreateObject("WScript.Shell")
    WSHShell.Run "regsvr32.exe /s """ & AppPath & "FlashControl.ocx""", 0, True
    Set WSHShell = Nothing
    
    Load frmMain
    
    Set MainForm = frmMain
    IsFlashAvailable = True
    
    Exit Sub
    
ErrorHandler:
    
    'Flashが使えないのでFlash無しのメインウィンドウを使用する
    Load frmSafeMain
    Set MainForm = frmSafeMain
End Sub
    
'各ウィンドウをロード
'ただしメインウィンドウはあらかじめLoadMainFormAndRegisterFlashでロードしておくこと
Public Sub LoadForms()
Dim X As Integer, Y As Integer

    Load frmToolTip
    Load frmMessage
    Load frmListBox
    
    LockGUI
    CommandState = "ユニット選択"
    
    'マップ画面に表示できるマップのサイズ
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
    ' Optionで定義されていればそちらを優先する
    If IsOptionDefined("新ＧＵＩ") Then
        NewGUIMode = True
        MainWidth = 20
    End If
' ADD END MARGE
    MainHeight = 15
    
    'マップ画面のサイズ（ピクセル）
    MainPWidth = MainWidth * 32
    MainPHeight = MainHeight * 32
    
    With MainForm
        'メインウィンドウの位置＆サイズを設定
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
        
        'スクロールバーの位置を設定
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
        
        'ステータスウィンドウを設置
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
        
        'マップウィンドウのサイズを設定
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
'Optionによる新ＧＵＩが有効かどうかを再設定する
Public Sub SetNewGUIMode()
    ' Optionで定義されているのにNewGUIModeがfalseの場合、LoadFormsを呼ぶ
    If IsOptionDefined("新ＧＵＩ") _
        And Not NewGUIMode _
    Then
        LoadForms
    End If
End Sub
' ADD  END  MARGE

' === メッセージウィンドウに関する処理 ===

'メッセージウィンドウを開く
'戦闘メッセージ画面など、ユニット表示を行う場合は u1, u2 に指定
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
        'ユニット表示を伴う場合はキャプションから「(自動送り)」を削除
        If Not u1 Is Nothing Then
            If .Caption = "メッセージ (自動送り)" Then
                .Caption = "メッセージ"
            End If
        End If
        
        'メッセージウィンドウを強制的に最小化解除
        If .WindowState <> vbNormal Then
            .WindowState = vbNormal
            .Show
            .SetFocus
        End If
        
        If u1 Is Nothing Then
            'ユニット表示なし
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
            'ユニット表示１体のみ
            If u1.Party = "味方" Or u1.Party = "ＮＰＣ" Then
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
            'ユニットを２体表示
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
        
        'メッセージウィンドウの位置設定
        If MainForm.Visible And Not MainForm.WindowState = 1 Then
            'メインウィンドウが表示されていればメインウィンドウの下端に合わせて表示
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
            'メインウィンドウが表示されていない場合は画面中央に表示
            .Left = (Screen.width - .width) / 2
            .Top = (Screen.Height - .Height) / 2
        End If
        
        'ウィンドウをクリアしておく
        .picFace = LoadPicture("")
        DisplayedPilot = ""
        .picMessage.Cls
        
        'ウィンドウを表示
        .Show
        
        '常に手前に表示する
        ret = SetWindowPos(.hwnd, -1, 0, 0, 0, 0, &H3)
    End With
    
    DoEvents
End Sub

'メッセージウィンドウを閉じる
Public Sub CloseMessageForm()
    If Not frmMessage.Visible Then
        Exit Sub
    End If
    frmMessage.Hide
    DoEvents
End Sub

'メッセージウィンドウをクリア
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

'メッセージウィンドウに表示しているユニット情報を更新
Public Sub UpdateMessageForm(u1 As Unit, Optional u2 As Variant)
Dim lu As Unit, ru As Unit
Dim ret As Long, i As Integer, buf As String
Dim num As Integer, tmp As Long

    With frmMessage
        'ウィンドウにユニット情報が表示されていない場合はそのまま終了
        If .Visible Then
            If Not .picUnit1.Visible And Not .picUnit2.Visible Then
                Exit Sub
            End If
        End If
        
        'luを左に表示するユニット、ruを右に表示するユニットに設定
        If IsMissing(u2) Then
            '１体のユニットのみ表示
            If u1.Party = "味方" Or u1.Party = "ＮＰＣ" Then
                Set lu = Nothing
                Set ru = u1
            Else
                Set lu = u1
                Set ru = Nothing
            End If
        ElseIf u2 Is Nothing Then
            '反射攻撃
            '前回表示されたユニットをそのまま使用
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
        
        '現在表示されている順番に応じてユニットの入れ替え
        If lu Is RightUnit And ru Is LeftUnit _
            And Not LeftUnit Is RightUnit _
        Then
            Set lu = LeftUnit
            Set ru = RightUnit
        End If
        
        '表示するユニットのＧＵＩ部品を表示
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
        
        '未表示のユニットを表示する
        If Not lu Is Nothing And Not lu Is LeftUnit Then
            '左のユニットが未表示なので表示する
            
            'ユニット画像
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
                '非表示のユニットの場合はユニットのいる地形タイルを表示
                ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, _
                    MainForm.picBack.hDC, 32 * (lu.X - 1), 32 * (lu.Y - 1), SRCCOPY)
            End If
            .picUnit1.Refresh
            
            'ＨＰ名称
            If lu.IsConditionSatisfied("データ不明") Then
                .labHP1.Caption = Term("HP")
            Else
                .labHP1.Caption = Term("HP", lu)
            End If
            
            'ＨＰ数値
            If lu.IsConditionSatisfied("データ不明") Then
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
            
            'ＨＰゲージ
            .picHP1.Cls
            If lu.HP > 0 Or i < num Then
                .picHP1.Line (0, 0)-((.picHP1.width - 4) * lu.HP \ lu.MaxHP - 1, 4), , BF
            End If
            
            'ＥＮ名称
            If lu.IsConditionSatisfied("データ不明") Then
                .labEN1.Caption = Term("EN")
            Else
                .labEN1.Caption = Term("EN", lu)
            End If
            
            'ＥＮ数値
            If lu.IsConditionSatisfied("データ不明") Then
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
            
            'ＥＮゲージ
            .picEN1.Cls
            If lu.EN > 0 Or i < num Then
                .picEN1.Line (0, 0)-((.picEN1.width - 4) * lu.EN \ lu.MaxEN - 1, 4), , BF
            End If
            
            '表示内容を記録
            Set LeftUnit = lu
            LeftUnitHPRatio = lu.HP / lu.MaxHP
            LeftUnitENRatio = lu.EN / lu.MaxEN
        End If
        
        If Not ru Is Nothing And Not RightUnit Is ru Then
            '右のユニットが未表示なので表示する
            
            'ユニット画像
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
                '非表示のユニットの場合はユニットのいる地形タイルを表示
                ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, _
                    MainForm.picBack.hDC, 32 * (ru.X - 1), 32 * (ru.Y - 1), SRCCOPY)
            End If
            .picUnit2.Refresh
            
            'ＨＰ数値
            If ru.IsConditionSatisfied("データ不明") Then
                .labHP2.Caption = Term("HP")
            Else
                .labHP2.Caption = Term("HP", ru)
            End If
            
            'ＨＰ数値
            If ru.IsConditionSatisfied("データ不明") Then
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
            
            'ＨＰゲージ
            .picHP2.Cls
            If ru.HP > 0 Or i < num Then
                .picHP2.Line (0, 0)-((.picHP2.width - 4) * ru.HP \ ru.MaxHP - 1, 4), , BF
            End If
            
            'ＥＮ名称
            If ru.IsConditionSatisfied("データ不明") Then
                .labEN2.Caption = Term("EN")
            Else
                .labEN2.Caption = Term("EN", ru)
            End If
            
            'ＥＮ数値
            If ru.IsConditionSatisfied("データ不明") Then
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
            
            'ＥＮゲージ
            .picEN2.Cls
            If ru.EN > 0 Or i < num Then
                .picEN2.Line (0, 0)-((.picEN2.width - 4) * ru.EN \ ru.MaxEN - 1, 4), , BF
            End If
            
            '表示内容を記録
            Set RightUnit = ru
            RightUnitHPRatio = ru.HP / ru.MaxHP
            RightUnitENRatio = ru.EN / ru.MaxEN
        End If
        
        '前回の表示からのＨＰ、ＥＮの変化をアニメ表示
        
        '変化がない場合はアニメ表示の必要がないのでチェックしておく
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
        
        '右ボタンが押されている場合はアニメーション表示を短縮化
        If num > 0 Then
            If IsRButtonPressed() Then
                num = 2
            End If
        End If
        
        For i = 1 To num
            '左側のユニット
            If Not lu Is Nothing Then
                'ＨＰ
                If lu.HP / lu.MaxHP <> LeftUnitHPRatio Then
                    tmp = (lu.MaxHP * LeftUnitHPRatio * (num - i) + lu.HP * i) \ num
                    
                    If lu.IsConditionSatisfied("データ不明") Then
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
                
                'ＥＮ
                If lu.EN / lu.MaxEN <> LeftUnitENRatio Then
                    tmp = (lu.MaxEN * LeftUnitENRatio * (num - i) + lu.EN * i) \ num
                    
                    If lu.IsConditionSatisfied("データ不明") Then
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
            
            '右側のユニット
            If Not ru Is Nothing Then
                'ＨＰ
                If ru.HP / ru.MaxHP <> RightUnitHPRatio Then
                    tmp = (ru.MaxHP * RightUnitHPRatio * (num - i) + ru.HP * i) \ num
                    
                    If ru.IsConditionSatisfied("データ不明") Then
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
                
                'ＥＮ
                If ru.EN / ru.MaxEN <> RightUnitENRatio Then
                    tmp = (ru.MaxEN * RightUnitENRatio * (num - i) + ru.EN * i) \ num
                    If ru.IsConditionSatisfied("データ不明") Then
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
            
            'リフレッシュ
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
        
        '表示内容を記録
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

'メッセージウィンドウの状態を記録する
Public Sub SaveMessageFormStatus()
    IsMessageFormVisible = frmMessage.Visible
    Set SavedLeftUnit = LeftUnit
    Set SavedRightUnit = RightUnit
End Sub

'メッセージウィンドウの状態を記録した状態に保つ
Public Sub KeepMessageFormStatus()
    If Not IsMessageFormVisible Then
        '記録した時点でメッセージウィンドウが表示されていなければ
        If frmMessage.Visible Then
            '開いているメッセージウィンドウを強制的に閉じる
            CloseMessageForm
        End If
    ElseIf Not frmMessage.Visible Then
        '記録した時点ではメッセージウィンドウが表示されていたので、
        'メッセージウィンドウが表示されていない場合は表示する
        OpenMessageForm SavedLeftUnit, SavedRightUnit
    ElseIf LeftUnit Is Nothing And RightUnit Is Nothing _
        And (Not SavedLeftUnit Is Nothing Or Not SavedRightUnit Is Nothing) _
    Then
        'メッセージウィンドウからユニット表示が消えてしまった場合は再表示
        OpenMessageForm SavedLeftUnit, SavedRightUnit
    End If
End Sub


' === メッセージ表示に関する処理 ===

'メッセージウィンドウにメッセージを表示
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
    
    'キャラ表示の描き換え
    If pname = "システム" Then
        '「システム」
        frmMessage.picFace = LoadPicture("")
        frmMessage.picFace.Refresh
        DisplayedPilot = ""
        left_margin = ""
    ElseIf pname <> "" Then
        'どのキャラ画像を使うか？
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
        
        'キャラ画像の表示
        If fname <> "-.bmp" Then
            fname = "Pilot\" & fname
            If DisplayedPilot <> fname Or DisplayMode <> msg_mode Then
                If DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "メッセージ " & msg_mode) Then
                    frmMessage.picFace.Refresh
                    DisplayedPilot = fname
                    DisplayMode = msg_mode
                Else
                    frmMessage.picFace = LoadPicture("")
                    frmMessage.picFace.Refresh
                    DisplayedPilot = ""
                    DisplayMode = ""
                    
                    'パイロット画像が存在しないことを記録しておく
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
        
        If IsOptionDefined("会話パイロット名改行") Then
            left_margin = " "
        Else
            left_margin = "  "
        End If
    End If
    
    'メッセージ中の式置換を処理
    FormatMessage msg
    msg = Trim$(msg)
    
    '末尾に強制改行が入っている場合は取り除く
    Do While Right$(msg, 1) = ";"
        msg = Left$(msg, Len(msg) - 1)
    Loop
    
    'メッセージが空の場合はキャラ表示の描き換えのみ行う
    If msg = "" Then
        Exit Sub
    End If
    
    Select Case pname
        Case "システム"
            'そのまま使用
        Case ""
            '基本的にはそのまま使用するが、せりふ表示の代用の場合は
            'せりふ表示用の処理を行う
            i = 0
            If (InStr(msg, "「") > 0 And Right$(msg, 1) = "」") Then
                i = InStr(msg, "「")
            ElseIf (InStr(msg, "『") > 0 And Right$(msg, 1) = "』") Then
                i = InStr(msg, "『")
            ElseIf (InStr(msg, "(") > 0 And Right$(msg, 1) = ")") Then
                i = InStr(msg, "(")
            ElseIf (InStr(msg, "（") > 0 And Right$(msg, 1) = "）") Then
                i = InStr(msg, "（")
            End If
            If i > 1 Then
                If i < 8 _
                    Or PDList.IsDefined(Trim$(Left$(msg, i - 1))) _
                    Or NPDList.IsDefined(Trim$(Left$(msg, i - 1))) _
                Then
                    is_character_message = True
                    If Not IsSpace(Mid$(msg, i - 1, 1)) Then
                        '"「"の前に半角スペースを挿入
                        msg = Left$(msg, i - 1) & " " & Mid$(msg, i)
                    End If
                End If
            End If
        Case Else
            is_character_message = True
            If (Left$(msg, 1) = "(" Or Left$(msg, 1) = "（") _
                And (Right$(msg, 1) = ")" Or Right$(msg, 1) = "）") _
            Then
                'モノローグ
                msg = Mid$(msg, 2, Len(msg) - 2)
                msg = pnickname _
                    & IIf(IsOptionDefined("会話パイロット名改行"), ";", " ") _
                    & "（" & msg & "）"
            ElseIf Left$(msg, 1) = "『" And Right$(msg, 1) = "』" Then
                msg = Mid$(msg, 2, Len(msg) - 2)
                msg = pnickname _
                    & IIf(IsOptionDefined("会話パイロット名改行"), ";", " ") _
                    & "『" & msg & "』"
            Else
                'せりふ
                msg = pnickname _
                    & IIf(IsOptionDefined("会話パイロット名改行"), ";", " ") _
                    & "「" & msg & "」"
            End If
    End Select

    '強制改行の位置を設定
    If IsOptionDefined("改行時余白短縮") Then
        cl_margin(0) = 0.94 'メッセージ長の超過による改行の位置
        cl_margin(1) = 0.7  '"。"," "による改行の位置
        cl_margin(2) = 0.85  '"、"による改行の位置
    Else
        cl_margin(0) = 0.8
        cl_margin(1) = 0.6
        cl_margin(2) = 0.75
    End If
    
    'メッセージを分割
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
    
    'メッセージ長判定のため、元のメッセージを再構築
    msg = ""
    For i = 1 To UBound(messages)
        msg = msg & messages(i)
    Next
        
    'メッセージの表示
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
            'フォント設定を初期化
            With p
                .FontBold = False
                .FontItalic = False
                .FontName = "ＭＳ Ｐ明朝"
                .FontSize = 12
                .ForeColor = vbBlack
            End With
        Else
            'メッセージの途中から表示
            If is_character_message Then
                p.Print "  ";
            End If
        End If
        
        For j = msg_head To Len(buf)
            ch = Mid$(buf, j, 1)
            
            '";"では必ず改行
            If ch = ";" Then
                If j <> line_head Then
                    PrintMessage Mid$(buf, line_head, j - line_head)
                    lnum = lnum + 1
                    If is_character_message _
                        And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) _
                            Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) _
                    Then
                        p.Print left_margin;
                    End If
                End If
                line_head = j + 1
                GoTo NextLoop
            End If
            
            'タグ内では改行しない
            If ch = "<" Then
                in_tag = True
                GoTo NextLoop
            ElseIf ch = ">" Then
                in_tag = False
            ElseIf in_tag Then
                GoTo NextLoop
            End If
            
            'メッセージが途切れてしまう場合は必ず改行
            If MessageLen(Mid$(buf, line_head, j - line_head)) > 0.95 * p.width Then
                PrintMessage Mid$(buf, line_head, j - line_head + 1)
                lnum = lnum + 1
                If is_character_message _
                    And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) _
                        Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) _
                Then
                    p.Print left_margin;
                End If
                line_head = j + 1
                GoTo NextLoop
            End If
            
            '禁則処理
            Select Case Mid$(buf, j + 1, 1)
                Case "。", "、", "…", "‥", "・", "･", _
                    "～", "ー", "－", "！", "？", _
                    "」", "』", "）", ")", " ", ";"
                    GoTo NextLoop
            End Select
            Select Case Mid$(buf, j + 2, 1)
                Case "。", "、", "…", "‥", "・", "･", _
                    "～", "ー", "－", "！", "？", _
                    "」", "』", "）", ")", " ", ";"
                    GoTo NextLoop
            End Select
            If Mid$(buf, j + 3, 1) = ";" Then
                GoTo NextLoop
            End If
            
            '改行の判定
            If MessageLen(Mid$(messages(i), line_head)) < 0.95 * p.width Then
                '全体が一行に収まる場合
                GoTo NextLoop
            End If
            Select Case ch
                Case "。"
                    If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(1) * p.width Then
                        PrintMessage Mid$(buf, line_head, j - line_head + 1)
                        lnum = lnum + 1
                        If is_character_message _
                            And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) _
                                Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) _
                        Then
                            p.Print left_margin;
                        End If
                        line_head = j + 1
                    End If
                Case "、"
                    If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(2) * p.width Then
                        PrintMessage Mid$(buf, line_head, j - line_head + 1)
                        lnum = lnum + 1
                        If is_character_message _
                            And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) _
                                Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) _
                        Then
                            p.Print left_margin;
                        End If
                        line_head = j + 1
                    End If
                Case " "
                    ch = Mid$(buf, j - 1, 1)
                    'スペースが文の区切りに使われているかどうか判定
                    If pname <> "システム" _
                        And (ch = "！" Or ch = "？" _
                            Or ch = "…" Or ch = "‥" _
                            Or ch = "・" Or ch = "･" _
                            Or ch = "～") _
                    Then
                        '文の区切り
                        If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(1) * p.width Then
                            PrintMessage Mid$(buf, line_head, j - line_head + 1)
                            lnum = lnum + 1
                            If is_character_message _
                                And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) _
                                    Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) _
                            Then
                                p.Print left_margin;
                            End If
                            line_head = j + 1
                        End If
                    Else
                        '単なる空白
                        If MessageLen(Mid$(buf, line_head, j - line_head)) > cl_margin(0) * p.width Then
                            PrintMessage Mid$(buf, line_head, j - line_head + 1)
                            lnum = lnum + 1
                            If is_character_message _
                                And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) _
                                    Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) _
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
                            And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) _
                                Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) _
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
        '残りの部分を表示
        If lnum < 4 Then
            If Len(buf) >= line_head Then
                PrintMessage Mid$(buf, line_head)
            End If
        End If
        
        DoEvents
        
        If MessageWait > 10000 Then
            AutoMessageMode = False
        End If
        
        'ウィンドウのキャプションを設定
        If AutoMessageMode Then
            If frmMessage.Caption = "メッセージ" Then
                frmMessage.Caption = "メッセージ (自動送り)"
            End If
        Else
            If frmMessage.Caption = "メッセージ (自動送り)" Then
                frmMessage.Caption = "メッセージ"
            End If
        End If
        
        '次のメッセージ表示までの時間を設定(自動メッセージ送り用)
        start_time = timeGetTime()
        wait_time = (lnum - prev_lnum + 2) * (MessageWait + 250)
        
        '次のメッセージ待ち
        IsFormClicked = False
        is_automode = AutoMessageMode
        Do Until IsFormClicked
            If AutoMessageMode Then
                If start_time + wait_time < timeGetTime() Then
                    Exit Do
                End If
            End If
            
            GetCursorPos PT
            
            'メッセージウインドウ上でマウスボタンを押した場合
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
                                '左ボタンでメッセージの自動送り
                                Exit Do
                            End If
                        ElseIf (rstate And &H8000) <> 0 Then
                            '右ボタンでメッセージの早送り
                            Exit Do
                        End If
                    End If
                End With
            End If
            
            'メインウインドウ上でマウスボタンを押した場合
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
                                '左ボタンでメッセージの自動送り
                                Exit Do
                            End If
                        ElseIf (rstate And &H8000) <> 0 Then
                            '右ボタンでメッセージの早送り
                            Exit Do
                        End If
                    End If
                End With
            End If
            
            Sleep 100
            DoEvents
            
            '自動送りモードが切り替えられた場合
            If is_automode <> AutoMessageMode Then
                IsFormClicked = False
                is_automode = AutoMessageMode
                If AutoMessageMode Then
                    frmMessage.Caption = "メッセージ (自動送り)"
                    start_time = timeGetTime()
                    wait_time = (lnum - prev_lnum + 2) * (MessageWait + 250)
                Else
                    frmMessage.Caption = "メッセージ"
                End If
            End If
        Loop
        
        'ウェイト計算用に既に表示した行数を記録
        If lnum < 4 Then
            prev_lnum = lnum
        Else
            prev_lnum = 0
        End If
    Loop
    
    'フォント設定を元に戻す
    With p
        .FontBold = False
        .FontItalic = False
        .FontName = "ＭＳ Ｐ明朝"
        .FontSize = 12
        .ForeColor = vbBlack
    End With
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "パイロット用画像ファイル" & vbCr & vbLf _
        & DisplayedPilot & vbCr & vbLf _
        & "の読み込み中にエラーが発生しました。" & vbCr & vbLf _
        & "画像ファイルが壊れていないか確認して下さい。"
End Sub

'メッセージウィンドウに文字列を書き込む
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
        
        'システムメッセージの時のみエスケープシーケンスの処理を行う
        If is_sys_msg Then
            Select Case ch
                Case "["
                    escape_depth = escape_depth + 1
                    If escape_depth = 1 Then
                        'エスケープシーケンス開始
                        'それまでの文字列を出力
                        p.Print Mid$(msg, head, i - head);
                        head = i + 1
                        GoTo NextChar
                    End If
                Case "]"
                    escape_depth = escape_depth - 1
                    If escape_depth = 0 Then
                        'エスケープシーケンス終了
                        'エスケープシーケンスを出力
                        p.Print Mid$(msg, head, i - head);
                        head = i + 1
                        GoTo NextChar
                    End If
            End Select
        End If
        
        'タグの処理
        Select Case ch
            Case "<"
                If Not in_tag And escape_depth = 0 Then
                    'タグ開始
                    in_tag = True
                    'それまでの文字列を出力
                    p.Print Mid$(msg, head, i - head);
                    head = i + 1
                    GoTo NextChar
                End If
            Case ">"
                If in_tag Then
                    'タグ終了
                    in_tag = False
                    
                    'タグの切り出し
                    tag = LCase$(Mid$(msg, head, i - head))
                    
                    'タグに合わせて各種処理を行う
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
                                '色設定
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
                                'サイズ設定
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
                                'タグではないのでそのまま書き出す
                                p.Print Mid$(msg, head - 1, i - head + 2);
                            End If
                    End Select
                    
                    head = i + 1
                    GoTo NextChar
                End If
        End Select
NextChar:
    Next
    
    '終了していないタグ、もしくはエスケープシーケンスはただの文字列と見なす
    If in_tag Or escape_depth > 0 Then
        head = head - 1
    End If
    
    '未出力の文字列を出力する
    If head <= Len(msg) Then
        If Right$(msg, 1) = "」" Then
            '最後の括弧の位置は一番大きなサイズの文字に合わせる
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
        '未出力の文字列がない場合は改行のみ
        p.Print
    End If
    
    '改行後の位置は一番大きなサイズの文字に合わせる
    If max_y > p.CurrentY Then
        p.CurrentY = max_y + 1
    Else
        p.CurrentY = p.CurrentY + 1
    End If
    p.CurrentX = 1
End Sub

'メッセージ幅を計算(タグを無視して)
Public Function MessageLen(ByVal msg As String) As Integer
Dim buf As String, ret As Integer
    
    'タグが存在する？
    ret = InStr(msg, "<")
    If ret = 0 Then
        MessageLen = frmMessage.picMessage.TextWidth(msg)
        Exit Function
    End If
    
    'タグを除いたメッセージを作成
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
    
    'タグ抜きメッセージのピクセル幅を計算
    MessageLen = frmMessage.picMessage.TextWidth(buf)
End Function

'メッセージウィンドウに戦闘メッセージを表示
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

    '初めて実行する際に、各フォルダにBitmapフォルダがあるかチェック
    If Not init_display_battle_message Then
        If Len(Dir$(ExtDataPath & "Bitmap", vbDirectory)) > 0 Then
            extdata_bitmap_dir_exists = True
        End If
        If Len(Dir$(ExtDataPath2 & "Bitmap", vbDirectory)) > 0 Then
            extdata2_bitmap_dir_exists = True
        End If
        init_display_battle_message = True
    End If
    
    'メッセージウィンドウが閉じられていれば表示しない
    If frmMessage.WindowState = 1 Then
        Exit Sub
    End If
    
    'ウィンドウのキャプションを設定
    If frmMessage.Caption = "メッセージ (自動送り)" Then
        frmMessage.Caption = "メッセージ"
    End If
    
    'キャラ表示の描き換え
    If pname = "システム" Then
        '「システム」
        frmMessage.picFace = LoadPicture("")
        frmMessage.picFace.Refresh
        DisplayedPilot = ""
    ElseIf pname <> "" And pname <> "-" Then
        'どのキャラ画像を使うか？
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
        
        'キャラ画像の表示
        If fname <> "-.bmp" Then
            fname = "Pilot\" & fname
            If DisplayedPilot <> fname Or DisplayMode <> msg_mode Then
                If DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "メッセージ " & msg_mode) Then
                    frmMessage.picFace.Refresh
                    DisplayedPilot = fname
                    DisplayMode = msg_mode
                Else
                    frmMessage.picFace = LoadPicture("")
                    frmMessage.picFace.Refresh
                    DisplayedPilot = ""
                    DisplayMode = ""
                    
                    'パイロット画像が存在しないことを記録しておく
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
    
    'メッセージが空なら表示は止める
    If msg = "" Then
        Exit Sub
    End If
    
    Set p = frmMessage.picMessage
    
    'メッセージウィンドウの状態を記録
    SaveMessageFormStatus
    
    'メッセージを分割
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
    
    '強制改行の位置を設定
    If IsOptionDefined("改行時余白短縮") Then
        cl_margin(0) = 0.94 'メッセージ長の超過による改行の位置
        cl_margin(1) = 0.7  '"。"," "による改行の位置
        cl_margin(2) = 0.85  '"、"による改行の位置
    Else
        cl_margin(0) = 0.8
        cl_margin(1) = 0.6
        cl_margin(2) = 0.75
    End If

    '各メッセージを表示
    For i = 1 To UBound(messages)
        buf = messages(i)
        
        'メッセージ内の式置換を処理
        SaveBasePoint
        FormatMessage buf
        RestoreBasePoint
        
        '特殊効果
        Select Case LCase$(Right$(LIndex(buf, 1), 4))
            Case ".bmp", ".jpg", ".gif", ".png"
                Dim fname0 As String, fsuffix As String, fpath As String
                Dim first_id As Integer, last_id As Integer
                Dim wait_time2 As Long
                
                '右ボタンを押されていたらスキップ
                If IsRButtonPressed() Then
                    GoTo NextMessage
                End If
                
                'カットインの表示
                fname = LIndex(buf, 1)
                
                'アニメ指定かどうか判定
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
                
                '画像表示のオプション
                options = ""
                n = LLength(buf)
                j = 2
                opt_n = 2
                Do While j <= n
                    buf2 = LIndex(buf, j)
                    Select Case buf2
                        Case "透過", "背景", "白黒", "セピア", _
                            "明", "暗", "上下反転", "左右反転", _
                            "上半分", "下半分", "右半分", "左半分", _
                            "右上", "左上", "右下", "左下", _
                            "ネガポジ反転", "シルエット", _
                            "夕焼け", "水中", "保持"
                            options = options & buf2 & " "
                        Case "消去"
                            clear_every_time = True
                        Case "右回転"
                            j = j + 1
                            options = options & "右回転 " & LIndex(buf, j) & " "
                        Case "左回転"
                            j = j + 1
                            options = options & "左回転 " & LIndex(buf, j) & " "
                        Case "-"
                            'スキップ
                            opt_n = j + 1
                        Case Else
                            If Asc(buf2) = 35 And Len(buf2) = 7 Then
                                '透過色設定
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
                                'スキップ
                                opt_n = j + 1
                            End If
                    End Select
                    j = j + 1
                Loop
                
                If Asc(fname) = 64 Then '@
                    'パイロット画像切り替えの場合
                    
                    If first_id = -1 Then
                        fname = Mid$(fname, 2)
                    Else
                        fname0 = Mid$(fname0, 2)
                        fname = fname0 & Format$(first_id, "00") & fsuffix
                    End If
                    
                    'ウィンドウが表示されていなければ表示
                    If Not frmMessage.Visible Then
                        OpenMessageForm
                    End If
                    
                    If wait_time > 0 Then
                        start_time = timeGetTime()
                    End If
                    
                    '画像表示のオプション
                    options = options & " メッセージ"
                    Select Case MapDrawMode
                        Case "セピア", "白黒"
                            options = options & " " & MapDrawMode
                    End Select
                    
                    If first_id = -1 Then
                        '１枚画像の場合
                        DrawPicture fname, 0, 0, 64, 64, 0, 0, 0, 0, options
                        frmMessage.picFace.Refresh
                        
                        If wait_time > 0 Then
                            Do While (start_time + wait_time > timeGetTime())
                                Sleep 20
                            Loop
                        End If
                    Else
                        'アニメーションの場合
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
                
                '表示画像のサイズ
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
                
                '表示画像の位置
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
                    '１枚絵の場合
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
                    'アニメーションの場合
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
                '右ボタンを押されていたらスキップ
                If IsRButtonPressed() Then
                    GoTo NextMessage
                End If
                
                '効果音の演奏
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
        
        '戦闘アニメ呼び出し
        If Left$(buf, 1) = "@" Then
            ShowAnimation Mid$(buf, 2)
            GoTo NextMessage
        End If
        
        '特殊コマンド
        Select Case LCase$(LIndex(buf, 1))
            Case "clear"
                'カットインの消去
                ClearPicture
                need_refresh = True
                GoTo NextMessage
                
            Case "keep"
                'カットインの保存
                IsPictureDrawn = False
                GoTo NextMessage
        End Select
        
        'ウェイト
        If IsNumeric(buf) Then
            wait_time = 100 * CDbl(buf)
            GoTo NextMessage
        End If
        
        'これよりメッセージの表示
        
        '空メッセージの場合は表示しない
        If buf = "" Then
            GoTo NextMessage
        End If
        
        'メッセージウィンドウの状態が変化している場合は復元
        KeepMessageFormStatus
        
        With p
            'ウィンドウをクリア
            .Cls
            .CurrentX = 1
            
            'フォント設定を初期化
            .FontBold = False
            .FontItalic = False
            .FontName = "ＭＳ Ｐ明朝"
            .FontSize = 12
            .ForeColor = vbBlack
        End With
        
        '話者名と括弧の表示処理
        is_char_message = False
        If pname <> "システム" _
            And ((pname <> "" And pname <> "-") _
                Or ((Left$(buf, 1) = "「" And Right$(buf, 1) = "」")) _
                Or ((Left$(buf, 1) = "『" And Right$(buf, 1) = "』"))) _
        Then
            Dim with_footer As Boolean
            
            is_char_message = True
            
            '話者のグラフィックを表示
            If pname = "-" _
                And Not SelectedUnit Is Nothing _
            Then
                If SelectedUnit.CountPilot > 0 Then
                    fname = SelectedUnit.MainPilot.Bitmap
                    If DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "メッセージ " & msg_mode) Then
                        frmMessage.picFace.Refresh
                        DisplayedPilot = fname
                        DisplayMode = msg_mode
                    End If
                End If
            End If
            
            '話者名を表示
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
            
            'メッセージが途中で終わっているか判定
            If Right$(buf, 1) <> ":" Then
                with_footer = True
            Else
                with_footer = False
                prev_lnum = lnum
                buf = Left$(buf, Len(buf) - 1)
            End If
            
            '括弧を付加
            If (Left$(buf, 1) = "(" Or Left$(buf, 1) = "（") _
                And (Not with_footer Or (Right$(buf, 1) = ")" Or Right$(buf, 1) = "）")) _
            Then
                'モノローグ
                If with_footer Then
                    buf = Mid$(buf, 2, Len(buf) - 2)
                    buf = "（" & buf & "）"
                Else
                    buf = Mid$(buf, 2)
                    buf = "（" & buf
                End If
            ElseIf Left$(buf, 1) = "「" _
                And (Not with_footer Or Right$(buf, 1) = "」") _
            Then
                '「」の括弧が既にあるので変更しない
            ElseIf Left$(buf, 1) = "『" _
                And (Not with_footer Or Right$(buf, 1) = "』") _
            Then
                '『』の括弧が既にあるので変更しない
            Else
                If with_footer Then
                    buf = "「" & buf & "」"
                Else
                    buf = "「" & buf
                End If
            End If
        Else
            'メッセージが途中で終わっているか判定
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
            
            '「.」の場合は必ず改行
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
                
            'タグ内では改行しない
            If ch = "<" Then
                in_tag = True
                GoTo NextLoop
            ElseIf ch = ">" Then
                in_tag = False
            ElseIf in_tag Then
                GoTo NextLoop
            End If
            
            'メッセージが途切れてしまう場合は必ず改行
            If MessageLen(Mid$(buf, line_head, j - line_head)) > 0.95 * p.width Then
                PrintMessage Mid$(buf, line_head, j - line_head + 1), Not is_char_message
                If is_char_message Then
                    p.Print "  ";
                End If
                line_head = j + 1
                lnum = lnum + 1
                GoTo NextLoop
            End If
            
            '禁則処理
            Select Case Mid$(buf, j + 1, 1)
                Case "。", "、", "…", "‥", "・", "･", _
                    "～", "ー", "－", "！", "？", _
                    "」", "』", "）", ")", " ", "."
                    GoTo NextLoop
            End Select
            Select Case Mid$(buf, j + 2, 1)
                Case "。", "、", "…", "‥", "・", "･", _
                    "～", "ー", "－", "！", "？", _
                    "」", "』", "）", ")", " ", "."
                    GoTo NextLoop
            End Select
            If Mid$(buf, j + 3, 1) = "." Then
                GoTo NextLoop
            End If
            
            '改行の判定
            If MessageLen(Mid$(messages(i), line_head)) < 0.95 * p.width Then
                '全体が一行に収まる場合
                GoTo NextLoop
            End If
            Select Case ch
                Case "。"
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
                Case "、"
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
        'メッセージの残りを表示しておく
        If Len(buf) >= line_head Then
            PrintMessage Mid$(buf, line_head), Not is_char_message
            lnum = lnum + 1
        End If
        
        'フォント設定を元に戻す
        With p
            .FontBold = False
            .FontItalic = False
            .FontName = "ＭＳ Ｐ明朝"
            .FontSize = 12
            .ForeColor = vbBlack
        End With
        
        'デフォルトのウェイト
        If wait_time = DEFAULT_LEVEL Then
            wait_time = (lnum - prev_lnum + 1) * MessageWait
            If msg_mode = "高速" Then
                wait_time = wait_time \ 2
            End If
        End If
        
        '画面を更新
        If need_refresh Then
            MainForm.picMain(0).Refresh
            need_refresh = False
        End If
        DoEvents
        
        '待ち時間が切れるまで待機
        start_time = timeGetTime()
        IsFormClicked = False
        Do While (start_time + wait_time > timeGetTime())
            '左ボタンが押されたらメッセージ送り
            If IsFormClicked Then
                Exit Do
            End If
            
            '右ボタンを押されていたら早送り
            If IsRButtonPressed() Then
                Exit Do
            End If
            
            Sleep 20
            DoEvents
        Loop
        wait_time = DEFAULT_LEVEL
NextMessage:
    Next
    
    '戦闘アニメデータのカットイン表示？
    If pname = "-" Then
        Exit Sub
    End If
    
    '画面を更新
    If need_refresh Then
        MainForm.picMain(0).Refresh
        need_refresh = False
    End If
    
    'メッセージデータの最後にウェイトの指定が行われていた場合
    If wait_time > 0 Then
        start_time = timeGetTime()
        IsFormClicked = False
        Do While (start_time + wait_time > timeGetTime())
            '左ボタンが押されたらメッセージ送り
            If IsFormClicked Then
                Exit Do
            End If
            
            '右ボタンを押されていたら早送り
            If IsRButtonPressed() Then
                Exit Do
            End If
            
            Sleep 20
            DoEvents
        Loop
    End If
    
    'メッセージウィンドウの状態が変化している場合は復元
    KeepMessageFormStatus
    
    DoEvents
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "画像ファイル" & vbCr & vbLf _
        & fname & vbCr & vbLf _
        & "の読み込み中にエラーが発生しました。" & vbCr & vbLf _
        & "画像ファイルが壊れていないか確認して下さい。"
End Sub

'システムによるメッセージを表示
Public Sub DisplaySysMessage(ByVal msg As String, Optional ByVal short_wait As Boolean)
Dim i As Integer, j As Integer, line_head As Integer
Dim ch As String, buf As String
Dim p As PictureBox
Dim lnum As Integer
Dim start_time As Long, wait_time As Long
Dim in_tag As Boolean

    'メッセージウィンドウが表示されていない場合は表示をキャンセル
    If frmMessage.WindowState = 1 Then
        Exit Sub
    End If

    'メッセージ内の式を置換
    FormatMessage msg

    'ウィンドウのキャプションを設定
    If frmMessage.Caption = "メッセージ (自動送り)" Then
        frmMessage.Caption = "メッセージ"
    End If

    Set p = frmMessage.picMessage

    With p
        'メッセージウィンドウをクリア
        .Cls
        .CurrentX = 1

        'フォント設定を初期化
        .FontBold = False
        .FontItalic = False
        .FontName = "ＭＳ Ｐ明朝"
        .FontSize = 12
        .ForeColor = vbBlack
    End With

    'メッセージを表示
    lnum = 0
    line_head = 1
    For i = 1 To Len(msg)
        ch = Mid$(msg, i, 1)

        '「;」の場合は必ず改行
        If ch = ";" Then
            If line_head <> i Then
                buf = Mid$(msg, line_head, i - line_head)
                PrintMessage buf, True
                lnum = lnum + 1
            End If
            line_head = i + 1
            GoTo NextLoop
        End If

        'タグ内では改行しない
        If ch = "<" Then
            in_tag = True
            GoTo NextLoop
        ElseIf ch = ">" Then
            in_tag = False
        ElseIf in_tag Then
            GoTo NextLoop
        End If

        '禁則処理
        If ch = "。" Or ch = "、" Then
            GoTo NextLoop
        End If
        If i < Len(msg) Then
            If Mid$(msg, i + 1, 1) = "。" Or Mid$(msg, i + 1, 1) = "、" Then
                GoTo NextLoop
            End If
        End If

        If MessageLen(Mid$(msg, line_head)) < p.width Then
            '全体が一行に収まる場合
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
            '[]で囲まれた文字列内では改行しない
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

    'フォント設定を元に戻す
    With p
        .FontBold = False
        .FontItalic = False
        .FontName = "ＭＳ Ｐ明朝"
        .FontSize = 12
        .ForeColor = vbBlack
    End With

    'ウェイトを計算
    wait_time = (0.8 + 0.5 * lnum) * MessageWait
    If short_wait Then
        wait_time = wait_time \ 2
    End If

    DoEvents

    '待ち時間が切れるまで待機
    IsFormClicked = False
    start_time = timeGetTime()
    Do While (start_time + wait_time > timeGetTime())
        '左ボタンが押されたらメッセージ送り
        If IsFormClicked Then
            Exit Do
        End If

        '右ボタンを押されていたら早送り
        If IsRButtonPressed() Then
            Exit Do
        End If

        Sleep 20
        DoEvents
    Loop
End Sub


' === マップウィンドウに関する処理 ===

'マップ画面背景の設定
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
    
    'ユニット画像色を変更しないといけない場合
    If MapDrawMode <> draw_mode Then
        UList.ClearUnitBitmap
        MapDrawMode = draw_mode
        MapDrawFilterColor = filter_color
        MapDrawFilterTransPercent = filter_trans_par
    ElseIf draw_mode = "フィルタ" _
        And (MapDrawFilterColor <> filter_color _
            Or MapDrawFilterTransPercent <> filter_trans_par) _
    Then
        UList.ClearUnitBitmap
        MapDrawMode = draw_mode
        MapDrawFilterColor = filter_color
        MapDrawFilterTransPercent = filter_trans_par
    End If
    
    'マップ背景の設定
    With MainForm
        Select Case draw_option
            Case "ステータス"
                With .picBack
                    ret = PatBlt(.hDC, 0, 0, .width, .Height, BLACKNESS)
                End With
                Exit Sub
            Case Else
                MapX = MainWidth \ 2 + 1
                MapY = MainHeight \ 2 + 1
        End Select
        
        '各マスのマップ画像を表示
        For i = 1 To MapWidth
            For j = 1 To MapHeight
                xx = 32 * (i - 1)
                yy = 32 * (j - 1)
                
'DEL START 240a
'                'マップ画像が既に読み込まれているか判定
'                For k = 1 To terrain_bmp_count
'                    If terrain_bmp_type(k) = MapData(i, j, 0) _
'                        And terrain_bmp_num(k) = MapData(i, j, 1) _
'                    Then
'                        Exit For
'                    End If
'                Next
                
'                If k <= terrain_bmp_count Then
'                    '既に描画済みの画像は描画した個所から転送
'                    ret = BitBlt(.picBack.hDC, _
'                        xx, yy, 32, 32, _
'                        .picBack.hDC, terrain_bmp_x(k), terrain_bmp_y(k), SRCCOPY)
'                    MapImageFileTypeData(i, j) = _
'                        MapImageFileTypeData(terrain_bmp_x(k) \ 32 + 1, terrain_bmp_y(k) \ 32 + 1)
'                Else
'                    '新規の画像の場合
'DEL  END  240a
                '画像ファイルを探す
'MOD START 240a
'                fname = SearchTerrainImageFile(MapData(i, j, 0), MapData(i, j, 1), i, j)
                fname = SearchTerrainImageFile(MapData(i, j, MapDataIndex.TerrainType), MapData(i, j, MapDataIndex.BitmapNo), i, j)
'MOD  END  240a
                
                '画像を取り込み
                If fname <> "" Then
                    On Error GoTo ErrorHandler
                    .picTmp32(0) = LoadPicture(fname)
                    On Error GoTo 0
                Else
                    ret = PatBlt(.picTmp32(0).hDC, 0, 0, 32, 32, BLACKNESS)
                End If
                
                'マップ設定によって表示色を変更
                Select Case draw_mode
                    Case "夜"
                        GetImage .picTmp32(0)
                        Dark
                        SetImage .picTmp32(0)
                    Case "セピア"
                        GetImage .picTmp32(0)
                        Sepia
                        SetImage .picTmp32(0)
                    Case "白黒"
                        GetImage .picTmp32(0)
                        Monotone
                        SetImage .picTmp32(0)
                    Case "夕焼け"
                        GetImage .picTmp32(0)
                        Sunset
                        SetImage .picTmp32(0)
                    Case "水中"
                        GetImage .picTmp32(0)
                        Water
                        SetImage .picTmp32(0)
                    Case "フィルタ"
                        GetImage .picTmp32(0)
                        ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent
                        SetImage .picTmp32(0)
                End Select
                
                '画像を描き込み
                ret = BitBlt(.picBack.hDC, xx, yy, 32, 32, _
                    .picTmp32(0).hDC, 0, 0, SRCCOPY)
'DEL START 240a
'                    '画像を登録
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
                'レイヤー描画する必要がある場合は描画する
                If BoxTypes.Upper = MapData(i, j, MapDataIndex.BoxType) _
                    Or BoxTypes.UpperBmpOnly = MapData(i, j, MapDataIndex.BoxType) _
                Then
                    '画像ファイルを探す
                     fname = SearchTerrainImageFile(MapData(i, j, MapDataIndex.LayerType), MapData(i, j, MapDataIndex.LayerBitmapNo), i, j)
                    
                    '画像を取り込み
                    If fname <> "" Then
On Error GoTo ErrorHandler
                        .picTmp32(0) = LoadPicture(fname)
On Error GoTo 0
                        BGColor = vbWhite
                        'マップ設定によって表示色を変更
                        Select Case draw_mode
                            Case "夜"
                                GetImage .picTmp32(0)
                                Dark True
                                SetImage .picTmp32(0)
                            Case "セピア"
                                GetImage .picTmp32(0)
                                Sepia True
                                SetImage .picTmp32(0)
                            Case "白黒"
                                GetImage .picTmp32(0)
                                Monotone True
                                SetImage .picTmp32(0)
                            Case "夕焼け"
                                GetImage .picTmp32(0)
                                Sunset True
                                SetImage .picTmp32(0)
                            Case "水中"
                                GetImage .picTmp32(0)
                                Water True
                                SetImage .picTmp32(0)
                            Case "フィルタ"
                                GetImage .picTmp32(0)
                                ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent, True
                                SetImage .picTmp32(0)
                        End Select
                        
                        '画像を透過描き込み
                        ret = TransparentBlt(.picBack.hDC, xx, yy, 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, BGColor)
                    End If
                    
                End If
'ADD  END  240a
            Next
        Next
'MapDrawn:  '使用されていないラベルなので削除
        
        'マス目の表示
        If ShowSquareLine Then
            MainForm.picBack.Line (0, 0)-(MapPWidth - 1, MapPHeight - 1), rgb(100, 100, 100), B
            For i = 1 To MapWidth - 1
                .picBack.Line (32 * i, -1)-(32 * i, MapPHeight), rgb(100, 100, 100)
            Next
            For i = 1 To MapHeight - 1
                .picBack.Line (0, 32 * i - 1)-(MapPWidth, 32 * i - 1), rgb(100, 100, 100)
            Next
        End If
        
        'マスク入り背景画面を作成
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
    
    '画面を更新
    If MapFileName <> "" And draw_option = "" Then
        RefreshScreen
    End If
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "マップ用ビットマップファイル" & vbCr & vbLf _
        & fname & vbCr & vbLf _
        & "の読み込み中にエラーが発生しました。" & vbCr & vbLf _
        & "画像ファイルが壊れていないか確認して下さい。"
    TerminateSRC
End Sub

'画面の書き換え (ユニット表示からやり直し)
Public Sub RedrawScreen(Optional ByVal late_refresh As Boolean)
Dim PT As POINTAPI
Dim ret As Long

    ScreenIsMasked = False
    
    '画面を更新
    RefreshScreen False, late_refresh
    
    'カーソルを再描画
    GetCursorPos PT
    ret = SetCursorPos(PT.X, PT.Y)
End Sub

'画面をマスクがけして再表示
Public Sub MaskScreen()
    ScreenIsMasked = True
    
    '画面を更新
    RefreshScreen
End Sub

' ADD START MARGE
'画面の書き換え
Public Sub RefreshScreen(Optional ByVal without_refresh As Boolean, _
    Optional ByVal delay_refresh As Boolean)

    If NewGUIMode Then
        RefreshScreenNew without_refresh, delay_refresh
    Else
        RefreshScreenOld without_refresh, delay_refresh
    End If
End Sub
' ADD END MARGE


'画面の書き換え (旧GUI)
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

    'マップデータが設定されていなければ画面書き換えを行わない
    If MapWidth = 1 Then
        Exit Sub
    End If
    
    '表示位置がマップ外にある場合はマップ内に合わせる
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
            
            'マップウィンドウのスクロールバーの位置を変更
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
            
            '一旦マップウィンドウの内容を消去
            ret = PatBlt(pic.hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
        End If
        
        mx = MapX - (MainWidth + 1) \ 2 + 1
        my = MapY - (MainHeight + 1) \ 2 + 1
        
        'マップ画像の転送元と転送先を計算する
        
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
        
        '直線を描画する際の描画色を黒に変更
        prev_color = pic.ForeColor
        pic.ForeColor = vbBlack
        
        '表示内容を更新
        If Not ScreenIsMasked Then
            '通常表示
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
                        '地形
                        ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                            .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                            SRCCOPY)
                    ElseIf u.BitmapID = -1 Then
                        '非表示のユニット
                        ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                            .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                            SRCCOPY)
                    Else
                        If u.Action > 0 Or u.IsFeatureAvailable("地形ユニット") Then
                            'ユニット
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picUnitBitmap.hDC, _
                                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
                                SRCCOPY)
                        Else
                            '行動済のユニット
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picUnitBitmap.hDC, _
                                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, _
                                SRCCOPY)
                        End If
                        
                        'ユニットのいる場所に合わせて表示を変更
                        Select Case u.Area
                            Case "空中"
                                ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                ret = LineTo(pic.hDC, xx + 31, yy + 28)
                            Case "水中"
                                ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                ret = LineTo(pic.hDC, xx + 31, yy + 3)
                            Case "地中"
                                ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                ret = LineTo(pic.hDC, xx + 31, yy + 3)
                            Case "宇宙"
                                If TerrainClass(sx + i, sy + j) = "月面" Then
                                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                End If
                        End Select
                    End If
                    
NextLoop:
                Next
            Next
        Else
            'マスク表示
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
                            'マスクされた地形
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        Else
                            '地形
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        End If
                    ElseIf u.BitmapID = -1 Then
                        '非表示のユニット
                        If MaskData(sx + i, sy + j) Then
                            'マスクされた地形
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        Else
                            '地形
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        End If
                    Else
                        If MaskData(sx + i, sy + j) Then
                            'マスクされたユニット
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picUnitBitmap.hDC, _
                                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 64, _
                                SRCCOPY)
                            
                            'ユニットのいる場所に合わせて表示を変更
                            Select Case u.Area
                                Case "空中"
                                    DottedLine xx, yy + 28
                                Case "水中"
                                    DottedLine xx, yy + 3
                                Case "地中"
                                    DottedLine xx, yy + 28
                                    DottedLine xx, yy + 3
                                Case "宇宙"
                                    If TerrainClass(sx + i, sy + j) = "月面" Then
                                        DottedLine xx, yy + 28
                                    End If
                            End Select
                        Else
                            'ユニット
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picUnitBitmap.hDC, _
                                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
                                SRCCOPY)
                            
                            'ユニットのいる場所に合わせて表示を変更
                            Select Case u.Area
                                Case "空中"
                                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                Case "水中"
                                    ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                Case "地中"
                                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                    ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                Case "宇宙"
                                    If TerrainClass(sx + i, sy + j) = "月面" Then
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
        
        '描画色を元に戻しておく
        pic.ForeColor = prev_color
        
        '画面が書き換えられたことを記録
        ScreenIsSaved = False
        
        If Not without_refresh And Not delay_refresh Then
            .picMain(0).Refresh
        End If
    End With
End Sub

' ADD START MARGE
'画面の書き換え (新GUI)
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

    'マップデータが設定されていなければ画面書き換えを行わない
    If MapWidth = 1 Then
        Exit Sub
    End If
    
    '表示位置がマップ外にある場合はマップ内に合わせる
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
            
            'マップウィンドウのスクロールバーの位置を変更
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
            
            '一旦マップウィンドウの内容を消去
            ret = PatBlt(pic.hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
        End If
        
        mx = MapX - (MainWidth + 1) \ 2 + 1
        my = MapY - (MainHeight + 1) \ 2 + 1
        
        'マップ画像の転送元と転送先を計算する
        
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
        
        '直線を描画する際の描画色を黒に変更
        prev_color = pic.ForeColor
        pic.ForeColor = vbBlack
        
        '表示内容を更新
        If Not ScreenIsMasked Then
            '通常表示
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
                        '画面左端は16ピクセル幅分だけ表示
                        If u Is Nothing Then
                            '地形
                            ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                SRCCOPY)
                        ElseIf u.BitmapID = -1 Then
                            '非表示のユニット
                            ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                SRCCOPY)
                        Else
                            If u.Action > 0 Or u.IsFeatureAvailable("地形ユニット") Then
                                'ユニット
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15), _
                                    SRCCOPY)
                            Else
                                '行動済のユニット
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15) + 32, _
                                    SRCCOPY)
                            End If
                            
                            'ユニットのいる場所に合わせて表示を変更
                            Select Case u.Area
                                Case "空中"
                                    ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                    ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                Case "水中"
                                    ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
                                    ret = LineTo(pic.hDC, 0 + 15, yy + 3)
                                Case "地中"
                                    ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                    ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                    ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
                                    ret = LineTo(pic.hDC, 0 + 15, yy + 3)
                                Case "宇宙"
                                    If TerrainClass(sx + i, sy + j) = "月面" Then
                                        ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                        ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                    End If
                            End Select
                        End If
                    Else
                        '画面左端以外は全32ピクセル幅分だけ表示
                        If u Is Nothing Then
                            '地形
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        ElseIf u.BitmapID = -1 Then
                            '非表示のユニット
                            ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                SRCCOPY)
                        Else
                            If u.Action > 0 Or u.IsFeatureAvailable("地形ユニット") Then
                                'ユニット
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
                                    SRCCOPY)
                            Else
                                '行動済のユニット
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, _
                                    SRCCOPY)
                            End If
                            
                            'ユニットのいる場所に合わせて表示を変更
                            Select Case u.Area
                                Case "空中"
                                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                Case "水中"
                                    ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                Case "地中"
                                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                    ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                    ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                Case "宇宙"
                                    If TerrainClass(sx + i, sy + j) = "月面" Then
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
            'マスク表示
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
                        '画面左端は16ピクセル幅分だけ表示
                        If u Is Nothing Then
                            If MaskData(sx + i, sy + j) Then
                                'マスクされた地形
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picMaskedBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            Else
                                '地形
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            End If
                        ElseIf u.BitmapID = -1 Then
                            '非表示のユニット
                            If MaskData(sx + i, sy + j) Then
                                'マスクされた地形
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picMaskedBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            Else
                                '地形
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            End If
                        Else
                            If MaskData(sx + i, sy + j) Then
                                'マスクされたユニット
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15) + 64, _
                                    SRCCOPY)
                                
                                'ユニットのいる場所に合わせて表示を変更
                                Select Case u.Area
                                    Case "空中"
                                        DottedLine 0, yy + 28, True
                                    Case "水中"
                                        DottedLine 0, yy + 3, True
                                    Case "地中"
                                        DottedLine 0, yy + 28, True
                                        DottedLine 0, yy + 3, True
                                    Case "宇宙"
                                        If TerrainClass(sx + i, sy + j) = "月面" Then
                                            DottedLine 0, yy + 28, True
                                        End If
                                End Select
                            Else
                                'ユニット
                                ret = BitBlt(pic.hDC, 0, yy, 16, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15), _
                                    SRCCOPY)
                                
                                'ユニットのいる場所に合わせて表示を変更
                                Select Case u.Area
                                    Case "空中"
                                        ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                        ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                    Case "水中"
                                        ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
                                        ret = LineTo(pic.hDC, 0 + 15, yy + 3)
                                    Case "地中"
                                        ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                        ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                        ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
                                        ret = LineTo(pic.hDC, 0 + 15, yy + 3)
                                    Case "宇宙"
                                        If TerrainClass(sx + i, sy + j) = "月面" Then
                                            ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
                                            ret = LineTo(pic.hDC, 0 + 15, yy + 28)
                                        End If
                                End Select
                            End If
                        End If
                    Else
                        '画面左端以外は全32ピクセル幅分だけ表示
                        If u Is Nothing Then
                            If MaskData(sx + i, sy + j) Then
                                'マスクされた地形
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            Else
                                '地形
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            End If
                        ElseIf u.BitmapID = -1 Then
                            '非表示のユニット
                            If MaskData(sx + i, sy + j) Then
                                'マスクされた地形
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            Else
                                '地形
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), _
                                    SRCCOPY)
                            End If
                        Else
                            If MaskData(sx + i, sy + j) Then
                                'マスクされたユニット
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 64, _
                                    SRCCOPY)
                                
                                'ユニットのいる場所に合わせて表示を変更
                                Select Case u.Area
                                    Case "空中"
                                        DottedLine xx, yy + 28
                                    Case "水中"
                                        DottedLine xx, yy + 3
                                    Case "地中"
                                        DottedLine xx, yy + 28
                                        DottedLine xx, yy + 3
                                    Case "宇宙"
                                        If TerrainClass(sx + i, sy + j) = "月面" Then
                                            DottedLine xx, yy + 28
                                        End If
                                End Select
                            Else
                                'ユニット
                                ret = BitBlt(pic.hDC, xx, yy, 32, 32, _
                                    .picUnitBitmap.hDC, _
                                    32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
                                    SRCCOPY)
                                
                                'ユニットのいる場所に合わせて表示を変更
                                Select Case u.Area
                                    Case "空中"
                                        ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                        ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                    Case "水中"
                                        ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                        ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                    Case "地中"
                                        ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                                        ret = LineTo(pic.hDC, xx + 31, yy + 28)
                                        ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                                        ret = LineTo(pic.hDC, xx + 31, yy + 3)
                                    Case "宇宙"
                                        If TerrainClass(sx + i, sy + j) = "月面" Then
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
        
        '描画色を元に戻しておく
        pic.ForeColor = prev_color
        
        '画面が書き換えられたことを記録
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

'指定されたマップ座標を画面の中央に表示
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


' === 座標変換 ===

'マップ上での座標がマップ画面のどの位置にくるかを返す
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

'マップ画面でのピクセルがマップの座標のどの位置にくるかを返す
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


' === ユニット画像表示に関する処理 ===

'ユニット画像ファイルを検索
Private Function FindUnitBitmap(u As Unit) As String
Dim fname As String, uname As String
Dim tname As String, tnum As String, tdir As String
Dim i As Integer, j As Integer

    With u
        'インターミッションでのパイロットステータス表示の場合は
        '特殊な処理が必要
        If .IsFeatureAvailable("ダミーユニット") _
            And InStr(.Name, "ステータス表示用ユニット") = 0 _
        Then
            If .CountPilot = 0 Then
                Exit Function
            End If
            
            If .FeatureData("ダミーユニット") = "ユニット画像使用" Then
                'ユニット画像を使って表示
                uname = "搭乗ユニット[" & .MainPilot.ID & "]"
                uname = LocalVariableList.Item(uname).StringValue
                fname = "\Bitmap\Unit\" & UList.Item(uname).Bitmap
            Else
                'パイロット画像を使って表示
                fname = "\Bitmap\Pilot\" & .MainPilot.Bitmap
            End If
            
            '画像を検索
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
        
        If .IsFeatureAvailable("地形ユニット") Then
            '地形ユニット
            fname = .Bitmap
            If FileExists(AppPath & "Bitmap\Map\" & fname) _
                Or FileExists(ScenarioPath & "Bitmap\Map\" & fname) _
            Then
                fname = "Bitmap\Map\" & fname
            Else
                '地形画像検索用の地形画像ディレクトリ名と4桁ファイル名を作成
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
                        'フォルダ指定あり
                        fname = "Bitmap\" & fname
                    Else
                        '通常のユニット画像
                        fname = "Bitmap\Unit\" & fname
                    End If
                End If
            End If
        Else
            '通常のユニット描画
            fname = .Bitmap
            If InStr(fname, ":") = 2 Then
                'フルパス指定
            ElseIf InStr(fname, "\") > 0 Then
                'フォルダ指定あり
                fname = "Bitmap\" & fname
            Else
                '通常のユニット画像
                fname = "Bitmap\Unit\" & fname
            End If
        End If
        
        '画像の検索
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
            
            '画像が見つからなかったことを記録
            If .Bitmap = .Data.Bitmap Then
                .Data.IsBitmapMissing = True
            End If
        End If
        
        FindUnitBitmap = fname
    End With
End Function

'ユニットのビットマップを作成
Public Function MakeUnitBitmap(u As Unit) As Integer
Dim fname As String, uparty As String
Dim i As Integer, ret As Long
Dim xx As Integer, yy As Integer
Static bitmap_num As Integer
Static fname_list() As String
Static party_list() As String

    With MainForm
        If u.IsFeatureAvailable("非表示") Then
            MakeUnitBitmap = -1
            Exit Function
        End If
        
        '画像がクリアされている？
        If .picUnitBitmap.width = 32 Then
            bitmap_num = 0
        End If
        
        '今までにロードされているユニット画像数
        ReDim Preserve fname_list(bitmap_num)
        ReDim Preserve party_list(bitmap_num)
        
        '以前ロードしたユニット画像と一致している？
        fname = FindUnitBitmap(u)
        uparty = u.Party0
        For i = 1 To bitmap_num
            If fname = fname_list(i) _
                And uparty = party_list(i) _
            Then
                '一致したものが見つかった
                MakeUnitBitmap = i
                Exit Function
            End If
        Next
        
        '新たに画像を登録
        bitmap_num = bitmap_num + 1
        ReDim Preserve fname_list(bitmap_num)
        ReDim Preserve party_list(bitmap_num)
        fname_list(bitmap_num) = fname
        party_list(bitmap_num) = uparty
        
        '画像バッファの大きさを変更
        .picUnitBitmap.Move 0, 0, 480, 96 * (bitmap_num \ 15 + 1)
        
        '画像の書き込み位置
        xx = 32 * (bitmap_num Mod 15)
        yy = 96 * (bitmap_num \ 15)
        
        'ファイルをロードする
        LoadUnitBitmap u, .picUnitBitmap, xx, yy, False, fname
        
        '行動済みの際の画像を作成
        ret = BitBlt(.picUnitBitmap.hDC, _
            xx, yy + 32, 32, 32, _
            .picUnitBitmap.hDC, _
            xx, yy, SRCCOPY)
        ret = BitBlt(.picUnitBitmap.hDC, _
            xx, yy + 32, 32, 32, _
            .picMask.hDC, 0, 0, SRCAND)
        
        'マスク入りの画像を作成
        ret = BitBlt(.picUnitBitmap.hDC, _
            xx, yy + 64, 32, 32, _
            .picUnitBitmap.hDC, _
            xx, yy + 32, SRCCOPY)
        ret = BitBlt(.picUnitBitmap.hDC, _
            xx, yy + 64, 32, 32, _
            .picMask2.hDC, 0, 0, SRCINVERT)
    End With
    
    'ユニット画像番号を返す
    MakeUnitBitmap = bitmap_num
End Function

'ユニットのビットマップをロード
Public Sub LoadUnitBitmap(u As Unit, pic As PictureBox, _
    ByVal dx As Integer, ByVal dy As Integer, _
    Optional ByVal use_orig_color As Boolean, _
    Optional fname As String)
Dim ret As Long
Dim emit_light As Boolean

    With MainForm
        '画像ファイルを検索
        If fname = "" Then
            fname = FindUnitBitmap(u)
        End If
        
        '画像をそのまま使用する場合
        If InStr(fname, "\Pilot\") > 0 _
            Or u.FeatureData("ダミーユニット") = "ユニット画像使用" _
        Then
            '画像の読み込み
            On Error GoTo ErrorHandler
            .picTmp = LoadPicture(fname)
            On Error GoTo 0
            
            '画面に描画
            ret = StretchBlt(pic.hDC, dx, dy, 32, 32, _
                .picTmp.hDC, 0, 0, .picTmp.width, .picTmp.Height, SRCCOPY)
            
            Exit Sub
        End If
        
        'ユニットが自分で発光しているかをあらかじめチェック
        If MapDrawMode = "夜" _
            And Not MapDrawIsMapOnly _
            And Not use_orig_color _
            And u.IsFeatureAvailable("発光") _
        Then
            emit_light = True
        End If
        
        If fname <> "" Then
            '画像が見つかった場合は画像を読み込み
            On Error GoTo ErrorHandler
            .picTmp32(0) = LoadPicture(fname)
            On Error GoTo 0
            
            '画像のサイズが正しいかチェック
            If .picTmp32(0).width <> 32 Or .picTmp32(0).Height <> 32 Then
                With .picTmp32(0)
                    .Picture = LoadPicture("")
                    .width = 32
                    .Height = 32
                End With
                ErrorMessage u.Name & "のユニット画像が32x32の大きさになっていません"
                Exit Sub
            End If
            
            If u.IsFeatureAvailable("地形ユニット") Then
                '地形ユニットの場合は画像をそのまま使う
                ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                    .picTmp32(0).hDC, 0, 0, SRCCOPY)
            ElseIf UseTransparentBlt Then
                'TransparentBltを使ってユニット画像とタイルを重ね合わせる
                
                'タイル
                Select Case u.Party0
                    Case "味方", "ＮＰＣ"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picUnit.hDC, 0, 0, SRCCOPY)
                    Case "敵"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picEnemy.hDC, 0, 0, SRCCOPY)
                    Case "中立"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picNeautral.hDC, 0, 0, SRCCOPY)
                End Select
                
                '画像の重ね合わせ
                '(発光している場合は２度塗りを防ぐため描画しない)
                If Not emit_light Then
                    ret = TransparentBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picTmp32(0).hDC, 0, 0, 32, 32, vbWhite)
                End If
            Else
                'BitBltを使ってユニット画像とタイルを重ね合わせる
                
                'マスクを作成
                MakeMask .picTmp32(0).hDC, .picTmp32(2).hDC, 32, 32, vbWhite
                
                'タイル
                Select Case u.Party0
                    Case "味方", "ＮＰＣ"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picUnit.hDC, 0, 0, SRCCOPY)
                    Case "敵"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picEnemy.hDC, 0, 0, SRCCOPY)
                    Case "中立"
                        ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                            .picNeautral.hDC, 0, 0, SRCCOPY)
                End Select
                
                '画像の重ね合わせ
                '(発光している場合は２度塗りを防ぐため描画しない)
                If Not emit_light Then
                    ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picTmp32(2).hDC, 0, 0, SRCERASE)
                    ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picTmp32(0).hDC, 0, 0, SRCINVERT)
                End If
            End If
        Else
            '画像が見つからなかった場合はタイルのみでユニット画像を作成
            Select Case u.Party0
                Case "味方", "ＮＰＣ"
                    ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picUnit.hDC, 0, 0, SRCCOPY)
                Case "敵"
                    ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picEnemy.hDC, 0, 0, SRCCOPY)
                Case "中立"
                    ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, _
                        .picNeautral.hDC, 0, 0, SRCCOPY)
            End Select
        End If
        
        '色をステージの状況に合わせて変更
        If Not use_orig_color _
            And Not MapDrawIsMapOnly _
        Then
            Select Case MapDrawMode
                Case "夜"
                    GetImage .picTmp32(1)
                    Dark
                    SetImage .picTmp32(1)
                    'ユニットが"発光"の特殊能力を持つ場合、
                    'ユニット画像を、暗くしたタイル画像の上に描画する。
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
                Case "セピア"
                    GetImage .picTmp32(1)
                    Sepia
                    SetImage .picTmp32(1)
                Case "白黒"
                    GetImage .picTmp32(1)
                    Monotone
                    SetImage .picTmp32(1)
                Case "夕焼け"
                    GetImage .picTmp32(1)
                    Sunset
                    SetImage .picTmp32(1)
                Case "水中"
                    GetImage .picTmp32(1)
                    Water
                    SetImage .picTmp32(1)
                Case "フィルタ"
                    GetImage .picTmp32(1)
                    ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent
                    SetImage .picTmp32(1)
            End Select
        End If
        
        '画面に描画
        ret = BitBlt(pic.hDC, _
            dx, dy, 32, 32, _
            .picTmp32(1).hDC, 0, 0, SRCCOPY)
    End With
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "ユニット用ビットマップファイル" & vbCr & vbLf _
        & fname & vbCr & vbLf _
        & "の読み込み中にエラーが発生しました。" & vbCr & vbLf _
        & "画像ファイルが壊れていないか確認して下さい。"
End Sub

'ユニット画像の描画
Public Sub PaintUnitBitmap(u As Unit, Optional ByVal smode As String)
Dim xx As Integer, yy As Integer
Dim pic As PictureBox
Dim ret As Long
Dim PT As POINTAPI
    
    With u
        '非表示？
        If .BitmapID = -1 Then
            Exit Sub
        End If
        
        '画面外？
        If .X < MapX - (MainWidth + 1) \ 2 Or MapX + (MainWidth + 1) \ 2 < .X _
            Or .Y < MapY - (MainHeight + 1) \ 2 Or MapY + (MainHeight + 1) \ 2 < .Y _
        Then
            Exit Sub
        End If
        
        '描き込み先の座標を設定
        xx = MapToPixelX(.X)
        yy = MapToPixelY(.Y)
    End With
    
    With MainForm
        If smode = "リフレッシュ無し" And ScreenIsSaved Then
            Set pic = .picMain(1)
            '表示画像を消去する際に使う描画領域を設定
            PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(xx, 0))
            PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(yy, 0))
            PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(xx + 32, MainPWidth - 1))
            PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(yy + 32, MainPHeight - 1))
        Else
            Set pic = .picMain(0)
        End If
        
        'ユニット画像の書き込み
        If u.Action > 0 Or u.IsFeatureAvailable("地形ユニット") Then
            '通常の表示
            ret = BitBlt(pic.hDC, _
                xx, yy, 32, 32, _
                .picUnitBitmap.hDC, _
                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
                SRCCOPY)
        Else
            '行動済の場合は網掛け
            ret = BitBlt(pic.hDC, _
                xx, yy, 32, 32, _
                .picUnitBitmap.hDC, _
                32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, _
                SRCCOPY)
        End If
        
        '直線を描画する際の描画色を設定
        pic.ForeColor = vbBlack
        
        'ユニットのいる場所に合わせて表示を変更
        Select Case u.Area
            Case "空中"
                ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                ret = LineTo(pic.hDC, xx + 31, yy + 28)
            Case "水中"
                ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                ret = LineTo(pic.hDC, xx + 31, yy + 3)
            Case "地中"
                ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                ret = LineTo(pic.hDC, xx + 31, yy + 28)
                ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
                ret = LineTo(pic.hDC, xx + 31, yy + 3)
            Case "宇宙"
                If TerrainClass(u.X, u.Y) = "月面" Then
                    ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
                    ret = LineTo(pic.hDC, xx + 31, yy + 28)
                End If
        End Select
        
        '描画色を白に戻しておく
        pic.ForeColor = vbWhite
        
        If smode <> "リフレッシュ無し" Then
            '画面が書き換えられたことを記録
            ScreenIsSaved = False
            
            If .Visible Then
                pic.Refresh
            End If
        End If
    End With
End Sub

'ユニット画像の表示を消す
Public Sub EraseUnitBitmap(ByVal X As Integer, ByVal Y As Integer, _
    Optional ByVal do_refresh As Boolean = True)
Dim xx As Integer, yy As Integer
Dim ret As Long
    
    '画面外？
    If X < MapX - (MainWidth + 1) \ 2 Or MapX + (MainWidth + 1) \ 2 < X _
        Or Y < MapY - (MainHeight + 1) \ 2 Or MapY + (MainHeight + 1) \ 2 < Y _
    Then
        Exit Sub
    End If
    
    '画面が乱れるので書き換えない？
    If IsPictureVisible Then
        Exit Sub
    End If
    
    xx = MapToPixelX(X)
    yy = MapToPixelY(Y)
    
    With MainForm
        SaveScreen
        
        '画面表示変更
        ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
            .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
        ret = BitBlt(.picMain(1).hDC, xx, yy, 32, 32, _
            .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
        
        If do_refresh Then
            '画面が書き換えられたことを記録
            ScreenIsSaved = False
            
            If .Visible Then
                .picMain(0).Refresh
            End If
        End If
    End With
End Sub

'ユニット画像の表示位置を移動 (アニメーション)
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
        
        'ユニット画像を作成
        ret = BitBlt(pic.hDC, _
            0, 0, 32, 32, _
            .picUnitBitmap.hDC, _
            32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
            SRCCOPY)
        
        'ユニットのいる場所に合わせて表示を変更
        Select Case u.Area
            Case "空中"
                ret = MoveToEx(pic.hDC, 0, 28, PT)
                ret = LineTo(pic.hDC, 31, 28)
            Case "水中"
                ret = MoveToEx(pic.hDC, 0, 3, PT)
                ret = LineTo(pic.hDC, 31, 3)
            Case "地中"
                ret = MoveToEx(pic.hDC, 0, 28, PT)
                ret = LineTo(pic.hDC, 31, 28)
                ret = MoveToEx(pic.hDC, 0, 3, PT)
                ret = LineTo(pic.hDC, 31, 3)
            Case "宇宙"
                If TerrainClass(u.X, u.Y) = "月面" Then
                    ret = MoveToEx(pic.hDC, 0, 28, PT)
                    ret = LineTo(pic.hDC, 31, 28)
                End If
        End Select
        
        '移動の始点を設定
        xx = MapToPixelX(x1)
        yy = MapToPixelY(y1)
        
        '背景上の画像をまず消去
        '(既に移動している場合を除く)
        If u Is MapDataForUnit(x1, y1) Then
            ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
                .picBack.hDC, 32 * (x1 - 1), 32 * (y1 - 1), SRCCOPY)
            ret = BitBlt(.picMain(1).hDC, xx, yy, 32, 32, _
                .picBack.hDC, 32 * (x1 - 1), 32 * (y1 - 1), SRCCOPY)
        End If
        
        '最初の移動方向を設定
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
        
        '移動の描画
        For i = 1 To division * MaxLng(Abs(x2 - x1), Abs(y2 - y1))
            '画像を消去
            ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
                .picMain(1).hDC, xx, yy, SRCCOPY)
            
            '座標を移動
            xx = xx + 32 * vx \ division
            yy = yy + 32 * vy \ division
            
            '画像を描画
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
        
        '２回目の移動方向を設定
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
        
        '移動の描画
        For i = 1 To division * MinLng(Abs(x2 - x1), Abs(y2 - y1))
            '画像を消去
            ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
                .picMain(1).hDC, xx, yy, SRCCOPY)
            
            '座標を移動
            xx = xx + 32 * vx \ division
            yy = yy + 32 * vy \ division
            
            '画像を描画
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
    
    '画面が書き換えられたことを記録
    ScreenIsSaved = False
End Sub

'ユニット画像の表示位置を移動 (アニメーション)
'画像の経路を実際の移動経路にあわせる
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
        
        'ユニット画像を作成
        ret = BitBlt(pic.hDC, 0, 0, 32, 32, .picUnitBitmap.hDC, _
            32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), _
            SRCCOPY)
        
        'ユニットのいる場所に合わせて表示を変更
        Select Case u.Area
            Case "空中"
                ret = MoveToEx(pic.hDC, 0, 28, PT)
                ret = LineTo(pic.hDC, 31, 28)
            Case "水中"
                ret = MoveToEx(pic.hDC, 0, 3, PT)
                ret = LineTo(pic.hDC, 31, 3)
            Case "地中"
                ret = MoveToEx(pic.hDC, 0, 28, PT)
                ret = LineTo(pic.hDC, 31, 28)
                ret = MoveToEx(pic.hDC, 0, 3, PT)
                ret = LineTo(pic.hDC, 31, 3)
            Case "宇宙"
                If TerrainClass(u.X, u.Y) = "月面" Then
                    ret = MoveToEx(pic.hDC, 0, 28, PT)
                    ret = LineTo(pic.hDC, 31, 28)
                End If
        End Select
        
        '移動経路を検索
        SearchMoveRoute u.X, u.Y, move_route_x, move_route_y
        
        If wait_time > 0 Then
            start_time = timeGetTime()
        End If
        
        '移動の始点
        xx = MapToPixelX(move_route_x(UBound(move_route_x)))
        yy = MapToPixelY(move_route_y(UBound(move_route_y)))
        
        i = UBound(move_route_x) - 1
        Do While i > 0
            vx = MapToPixelX(move_route_x(i)) - xx
            vy = MapToPixelY(move_route_y(i)) - yy
            
            '移動の描画
            For j = 1 To division
                '画像を消去
                ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, _
                    .picMain(1).hDC, xx, yy, SRCCOPY)
                
                '座標を移動
                xx = xx + vx \ division
                yy = yy + vy \ division
                
                '画像を描画
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
    
    '画面が書き換えられたことを記録
    ScreenIsSaved = False
End Sub


' === 各種リストボックスに関する処理 ===

'リストボックスを表示
Public Function ListBox(lb_caption As String, list() As String, lb_info As String, _
    Optional lb_mode As String) As Integer
Dim i As Integer
Dim is_rbutton_released As Boolean

    Load frmListBox
    With frmListBox
        .WindowState = vbNormal
        
        'コメントウィンドウの処理
        If InStr(lb_mode, "コメント") > 0 Then
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
        
        'キャプション
        .Caption = lb_caption
        If UBound(ListItemFlag) > 0 Then
            .labCaption = "  " & lb_info
        Else
            .labCaption = lb_info
        End If
        
        'リストボックスにアイテムを追加
        .lstItems.Visible = False
        .lstItems.Clear
        If UBound(ListItemFlag) > 0 Then
            For i = 1 To UBound(list)
                If ListItemFlag(i) Then
                    .lstItems.AddItem "×" & list(i)
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
        
        'コメント付きのアイテム？
        If UBound(ListItemComment) <> UBound(list) Then
            ReDim Preserve ListItemComment(UBound(list))
        End If
        
        '最小化されている場合は戻しておく
        If .WindowState <> vbNormal Then
            .WindowState = vbNormal
            .Show
        End If
        
        '表示位置を設定
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
            And InStr(lb_mode, "中央表示") = 0 _
        Then
            .Top = MainForm.Top + MainForm.Height - .Height
        Else
            .Top = (Screen.Height - .Height) / 2
        End If
        
        '先頭のアイテムを設定
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
        
        'コメントウィンドウの表示
        If .txtComment.Enabled Then
            .txtComment.Text = ListItemComment(.lstItems.ListIndex + 1)
        End If
        
        SelectedItem = 0
        
        IsFormClicked = False
        DoEvents
        
        'リストボックスを表示
        If InStr(lb_mode, "表示のみ") > 0 Then
            '表示のみを行う
            IsMordal = False
            .Show
            .lstItems.SetFocus
            Call SetWindowPos(.hwnd, -1, 0, 0, 0, 0, &H3)
            .Refresh
            Exit Function
        ElseIf InStr(lb_mode, "連続表示") > 0 Then
            '選択が行われてもリストボックスを閉じない
            IsMordal = False
            If Not .Visible Then
                .Show
                Call SetWindowPos(.hwnd, -1, 0, 0, 0, 0, &H3)
                .lstItems.SetFocus
            End If
            
            If InStr(lb_mode, "カーソル移動") > 0 Then
                If AutoMoveCursor Then
                    MoveCursorPos "ダイアログ"
                End If
            End If
            
            Do Until IsFormClicked
                DoEvents
                '右ボタンでのダブルクリックの実現
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
            '選択が行われた時点でリストボックスを閉じる
            IsMordal = False
            .Show
            Call SetWindowPos(.hwnd, -1, 0, 0, 0, 0, &H3)
            .lstItems.SetFocus
            
            If InStr(lb_mode, "カーソル移動") > 0 Then
                If AutoMoveCursor Then
                    MoveCursorPos "ダイアログ"
                End If
            End If
            
            Do Until IsFormClicked
                DoEvents
                '右ボタンでのダブルクリックの実現
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
            
            If InStr(lb_mode, "カーソル移動") > 0 _
                And InStr(lb_mode, "カーソル移動(行きのみ)") = 0 _
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

'リストボックスの高さを大きくする
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

'リストボックスの高さを小さくする
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

'リストボックスの幅を大きくする
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

'リストボックスの幅を小さくする
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

'武器選択用にリストボックスを切り替え
Public Sub AddPartsToListBox()
Dim ret As Long
Dim fname As String
Dim u As Unit, t As Unit
    
    Set u = SelectedUnit
    Set t = SelectedTarget
    
    With frmListBox
    'リストボックスにユニットやＨＰのゲージを追加
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
        
        'ユニット側の表示
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
                '非表示のユニットの場合はユニットのいる地形タイルを表示
                ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, _
                    MainForm.picBack.hDC, _
                    32 * (u.X - 1), 32 * (u.Y - 1), _
                    SRCCOPY)
            End If
        Else
            LoadUnitBitmap u, .picUnit1, 0, 0, True
        End If
        .picUnit1.Refresh
            
        If u.IsConditionSatisfied("データ不明") Then
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
        If u.IsConditionSatisfied("データ不明") Then
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
        'ターゲット側の表示
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
                '非表示のユニットの場合はユニットのいる地形タイルを表示
                ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, _
                    MainForm.picBack.hDC, _
                    32 * (t.X - 1), 32 * (t.Y - 1), _
                    SRCCOPY)
            End If
        Else
            LoadUnitBitmap t, .picUnit2, 0, 0, True
        End If
        .picUnit2.Refresh
        
        If t.IsConditionSatisfied("データ不明") Then
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
        If t.IsConditionSatisfied("データ不明") Then
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

'武器選択用リストボックスを通常のものに切り替え
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

'武器選択用リストボックス
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
        
        '攻撃力でソート
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
    If lb_mode = "移動前" Or lb_mode = "移動後" Or lb_mode = "一覧" Then
        '通常の武器選択時の表示
        For i = 1 To u.CountWeapon
            w = warray(i)
            
            With u
                If lb_mode = "一覧" Then
                    If Not .IsWeaponAvailable(w, "ステータス") Then
                        'Disableコマンドで使用不可にされた武器と使用できない合体技
                        'は表示しない
                        If .IsDisabled(.Weapon(w).Name) Then
                            GoTo NextLoop1
                        End If
                        If Not .IsWeaponMastered(w) Then
                            GoTo NextLoop1
                        End If
                        If .IsWeaponClassifiedAs(w, "合") Then
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
                        'Disableコマンドで使用不可にされた武器と使用できない合体技
                        'は表示しない
                        If .IsDisabled(.Weapon(w).Name) Then
                            GoTo NextLoop1
                        End If
                        If Not .IsWeaponMastered(w) Then
                            GoTo NextLoop1
                        End If
                        If .IsWeaponClassifiedAs(w, "合") Then
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
            
            '各武器の表示内容の設定
            With u.Weapon(w)
                '攻撃力
                If wpower(w) < 10000 Then
                    list(UBound(list)) = RightPaddedString(.Nickname, 27) _
                        & LeftPaddedString(Format$(wpower(w)), 4)
                Else
                    list(UBound(list)) = RightPaddedString(.Nickname, 26) _
                        & LeftPaddedString(Format$(wpower(w)), 5)
                End If
                
                '最大射程
                If u.WeaponMaxRange(w) > 1 Then
                    buf = Format$(.MinRange) & "-" & Format$(u.WeaponMaxRange(w))
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(buf, 5)
                Else
                    list(UBound(list)) = list(UBound(list)) & "    1"
                End If
                
                '命中率修正
                If u.WeaponPrecision(w) >= 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString("+" & Format$(u.WeaponPrecision(w)), 4)
                Else
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.WeaponPrecision(w)), 4)
                End If
                
                '残り弾数
                If .Bullet > 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.Bullet(w)), 3)
                Else
                    list(UBound(list)) = list(UBound(list)) & "  -"
                End If
                
                'ＥＮ消費量
                If .ENConsumption > 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.WeaponENConsumption(w)), 4)
                Else
                    list(UBound(list)) = list(UBound(list)) & "   -"
                End If
                
                'クリティカル率修正
                If u.WeaponCritical(w) >= 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString("+" & Format$(u.WeaponCritical(w)), 4)
                Else
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.WeaponCritical(w)), 4)
                End If
                
                '地形適応
                list(UBound(list)) = list(UBound(list)) & " " & .Adaption
                
                '必要気力
                If .NecessaryMorale > 0 Then
                    list(UBound(list)) = list(UBound(list)) _
                        & " 気" & .NecessaryMorale
                End If
                
                '属性
                wclass = u.WeaponClass(w)
                If InStrNotNest(wclass, "|") > 0 Then
                    wclass = Left$(wclass, InStrNotNest(wclass, "|") - 1)
                End If
                list(UBound(list)) = list(UBound(list)) & " " & wclass
            End With
NextLoop1:
        Next
        
        If lb_mode = "移動前" Or lb_mode = "移動後" Then
            If Not u.LookForSupportAttack(Nothing) Is Nothing Then
                '援護攻撃を使うかどうか選択
                UseSupportAttack = True
                ReDim Preserve list(UBound(list) + 1)
                ReDim Preserve ListItemFlag(UBound(list))
                list(UBound(list)) = "援護攻撃：使用する"
            End If
        End If
        
        'リストボックスを表示
        TopItem = -1
        ret = ListBox(caption_msg, list, _
            "名称                       攻撃 射程  命 弾  " & Term("EN", u, 2) & "  " _
                & Term("CT", u, 2) & " 適応 分類", _
            "表示のみ")
        
        If AutoMoveCursor Then
            If lb_mode <> "一覧" Then
                MoveCursorPos "武器選択"
            Else
                MoveCursorPos "ダイアログ"
            End If
        End If
        If BGM <> "" Then
            ChangeBGM BGM
        End If
        
        Do While True
            Do Until IsFormClicked
                DoEvents
                '右ボタンでのダブルクリックの実現
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
                '援護攻撃のオン/オフ切り替え
                UseSupportAttack = Not UseSupportAttack
                If UseSupportAttack Then
                    list(UBound(list)) = "援護攻撃：使用する"
                Else
                    list(UBound(list)) = "援護攻撃：使用しない"
                End If
                
                SelectedItem = ListBox(caption_msg, list, _
                    "名称                       攻撃 射程  命 弾  " & Term("EN", u, 2) _
                        & "  " & Term("CT", u, 2) & " 適応 分類", _
                    "表示のみ")
            End If
        Loop
        
        If lb_mode <> "一覧" Then
            frmListBox.Hide
        End If
        ReDim ListItemComment(0)
        WeaponListBox = wlist(SelectedItem)
        
    ElseIf lb_mode = "反撃" Then
        '反撃武器選択時の表示
        
        For i = 1 To u.CountWeapon
            w = warray(i)
            
            With u
                'Disableコマンドで使用不可にされた武器は表示しない
                If .IsDisabled(.Weapon(w).Name) Then
                    GoTo NextLoop2
                End If
                
                '必要技能を満たさない武器は表示しない
                If Not .IsWeaponMastered(w) Then
                    GoTo NextLoop2
                End If
                
                '使用できない合体技は表示しない
                If .IsWeaponClassifiedAs(w, "合") Then
                    If Not .IsCombinationAttackAvailable(w, True) Then
                        GoTo NextLoop2
                    End If
                End If
                    
                If Not .IsWeaponAvailable(w, "移動前") Then
                    'この武器は使用不能
                    ListItemFlag(UBound(list) + 1) = True
                ElseIf Not .IsTargetWithinRange(w, SelectedUnit) Then
                    'ターゲットが射程外
                    ListItemFlag(UBound(list) + 1) = True
                ElseIf .IsWeaponClassifiedAs(w, "Ｍ") Then
                    'マップ攻撃は武器選定外
                    ListItemFlag(UBound(list) + 1) = True
                ElseIf .IsWeaponClassifiedAs(w, "合") Then
                    '合体技は自分から攻撃をかける場合にのみ使用
                    ListItemFlag(UBound(list) + 1) = True
                ElseIf .Damage(w, SelectedUnit, True) > 0 Then
                    'ダメージを与えられる
                    ListItemFlag(UBound(list) + 1) = False
                ElseIf Not .IsNormalWeapon(w) _
                    And .CriticalProbability(w, SelectedUnit) > 0 _
                Then
                    '特殊効果を与えられる
                    ListItemFlag(UBound(list) + 1) = False
                Else
                    'この武器は効果が無い
                    ListItemFlag(UBound(list) + 1) = True
                End If
            End With
            
            ReDim Preserve list(UBound(list) + 1)
            ReDim Preserve wlist(UBound(list))
            wlist(UBound(list)) = w
            
            '各武器の表示内容の設定
            With u.Weapon(w)
                '攻撃力
                list(UBound(list)) = RightPaddedString(.Nickname, 29) _
                    & LeftPaddedString(Format$(wpower(w)), 4)
                
                '命中率
                If Not IsOptionDefined("予測命中率非表示") Then
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
                
                
                'クリティカル率
                If Not IsOptionDefined("予測命中率非表示") Then
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
                
                '残り弾数
                If .Bullet > 0 Then
                    list(UBound(list)) = list(UBound(list)) _
                        & LeftPaddedString(Format$(u.Bullet(w)), 3)
                Else
                    list(UBound(list)) = list(UBound(list)) & "  -"
                End If
                
                'ＥＮ消費量
                If .ENConsumption > 0 Then
                    list(UBound(list)) = list(UBound(list)) _
                        & LeftPaddedString(Format$(u.WeaponENConsumption(w)), 4)
                Else
                    list(UBound(list)) = list(UBound(list)) & "   -"
                End If
                
                '地形適応
                list(UBound(list)) = list(UBound(list)) & " " & .Adaption
                
                '必要気力
                If .NecessaryMorale > 0 Then
                    list(UBound(list)) = list(UBound(list)) _
                        & " 気" & .NecessaryMorale
                End If
                
                '属性
                wclass = u.WeaponClass(w)
                If InStrNotNest(wclass, "|") > 0 Then
                    wclass = Left$(wclass, InStrNotNest(wclass, "|") - 1)
                End If
                list(UBound(list)) = list(UBound(list)) & " " & wclass
            End With
NextLoop2:
        Next
        
        'リストボックスを表示
        TopItem = -1
        ret = ListBox(caption_msg, list, _
            "名称                         攻撃 命中 " & Term("CT", u, 2) & "   弾  " _
                & Term("EN", u, 2) & " 適応 分類", _
            "連続表示,カーソル移動")
        WeaponListBox = wlist(ret)
    End If
    
    DoEvents
End Function

'アビリティ選択用リストボックス
Public Function AbilityListBox(u As Unit, caption_msg As String, _
    lb_mode As String, Optional ByVal is_item As Boolean) As Integer
Dim i As Integer, j As Integer, k As Integer
Dim ret As Integer
Dim buf As String, msg As String, rest_msg As String
Dim list() As String, alist() As Integer
Dim is_available As Boolean
Dim is_rbutton_released As Boolean
    
    With u
        'アビリティが一つしかない場合は自動的にそのアビリティを選択する。
        'リストボックスの表示は行わない。
        If lb_mode <> "一覧" And Not is_item _
            And MainForm.mnuUnitCommandItem(AbilityCmdID).Caption <> Term("アビリティ", u) _
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
            If lb_mode = "一覧" Then
                If .IsAbilityAvailable(i, "ステータス") Then
                    'アイテムの使用効果かどうか
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
                    'Disableコマンドで使用不可にされたアビリティと使用できない合体技
                    'は表示しない
                    If .IsDisabled(.Ability(i).Name) Then
                        GoTo NextLoop
                    End If
                    If Not .IsAbilityMastered(i) Then
                        GoTo NextLoop
                    End If
                    If .IsAbilityClassifiedAs(i, "合") Then
                        If Not .IsCombinationAbilityAvailable(i, True) Then
                            GoTo NextLoop
                        End If
                    End If
                End If
            Else
                'アイテムの使用効果かどうか
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
                    'Disableコマンドで使用不可にされた武器と使用できない合体技
                    'は表示しない
                    If .IsDisabled(.Ability(i).Name) Then
                        GoTo NextLoop
                    End If
                    If Not .IsAbilityMastered(i) Then
                        GoTo NextLoop
                    End If
                    If .IsAbilityClassifiedAs(i, "合") Then
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
                    If .EffectType(j) = "解説" Then
                        msg = .EffectName(j)
                        Exit For
                    ElseIf InStr(.EffectName(j), "ターン)") > 0 Then
                        '持続時間が同じ能力はターン数をまとめて表示
                        k = InStr(msg, Mid$(.EffectName(j), InStr(.EffectName(j), "(")))
                        If k > 0 Then
                            msg = Left$(msg, k - 1) & "、" _
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
                
                '効果解説が長すぎる場合は改行
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
                
                '最大射程
                If u.AbilityMaxRange(i) > 1 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.AbilityMinRange(i)) _
                        & "-" & Format$(u.AbilityMaxRange(i)), 4)
                ElseIf u.AbilityMaxRange(i) = 1 Then
                    list(UBound(list)) = list(UBound(list)) & "   1"
                Else
                    list(UBound(list)) = list(UBound(list)) & "   -"
                End If
                
                '残り使用回数
                If .Stock > 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.Stock(i)), 3)
                Else
                    list(UBound(list)) = list(UBound(list)) & "  -"
                End If
                
                'ＥＮ消費量
                If .ENConsumption > 0 Then
                    list(UBound(list)) = list(UBound(list)) & _
                        LeftPaddedString(Format$(u.AbilityENConsumption(i)), 4)
                Else
                    list(UBound(list)) = list(UBound(list)) & "   -"
                End If
                
                '必要気力
                If .NecessaryMorale > 0 Then
                    list(UBound(list)) = list(UBound(list)) _
                        & " 気" & .NecessaryMorale
                End If
                
                '属性
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
    
    'リストボックスを表示
    TopItem = -1
    ret = ListBox(caption_msg, list, _
        "名称                 効果                            射程 数  " _
            & Term("EN", u, 2) & " 分類", _
        "表示のみ")
    
    If AutoMoveCursor Then
        MoveCursorPos "ダイアログ"
    End If
    
    Do Until IsFormClicked
        DoEvents
        '右ボタンでのダブルクリックの実現
        If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
            is_rbutton_released = True
        Else
            If is_rbutton_released Then
                IsFormClicked = True
            End If
        End If
    Loop
    If lb_mode <> "一覧" Then
        frmListBox.Hide
    End If
    ReDim ListItemComment(0)
    AbilityListBox = alist(SelectedItem)
    
    DoEvents
End Function

'入力時間制限付きのリストボックスを表示
Public Function LIPS(lb_caption As String, list() As String, _
    lb_info As String, ByVal time_limit As Integer) As Integer
Dim i As Integer
    
    Load frmListBox
    With frmListBox
        '表示内容を設定
        .Caption = lb_caption
        .labCaption = "  " & lb_info
        .lstItems.Clear
        For i = 1 To UBound(list)
            .lstItems.AddItem "  " & list(i)
        Next
        .lstItems.ListIndex = 0
        .lstItems.Height = 86
        
        '表示位置を設定
        .Left = (Screen.width - .width) / 2
        If MainForm.Visible = True And Not MainForm.WindowState = 1 Then
            .Top = MainForm.Top + MainForm.Height - .Height
        Else
            .Top = (Screen.Height - .Height) / 2
        End If
        
        '入力制限時間に関する設定を行う
        .CurrentTime = 0
        .TimeLimit = time_limit
        .picBar.Visible = True
        .Timer1.Enabled = True
        
        'リストボックスを表示し、プレイヤーからの入力を待つ
        SelectedItem = 0
        IsFormClicked = False
        .Show 1
        .CurrentTime = 0
        LIPS = SelectedItem
        
        'リストボックスを消去
        .lstItems.Height = 100
        .picBar.Visible = False
        .Timer1.Enabled = False
    End With
End Function

'複数段のリストボックスを表示
Public Function MultiColumnListBox(lb_caption As String, _
    list() As String, ByVal is_center As Boolean) As Integer
Dim i As Integer

    Load frmMultiColumnListBox
    With frmMultiColumnListBox
        .Caption = lb_caption
        .lstItems.Visible = False
        .lstItems.Clear
        
        'アイテムを追加
        For i = 1 To UBound(list)
            If ListItemFlag(i) Then
                .lstItems.AddItem "×" & list(i)
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
        
        '表示位置を設定
        .Left = (Screen.width - .width) / 2
        If MainForm.Visible = True _
            And Not MainForm.WindowState = 1 _
            And Not is_center _
        Then
            .Top = MainForm.Top + MainForm.Height - .Height
        Else
            .Top = (Screen.Height - .Height) / 2
        End If
        
        '先頭に表示するアイテムを設定
        If TopItem > 0 Then
            If .lstItems.TopIndex <> TopItem - 1 Then
                .lstItems.TopIndex = MinLng(TopItem, .lstItems.ListCount) - 1
            End If
        End If
        
        SelectedItem = 0
        
        DoEvents
        IsFormClicked = False
        
        'リストボックスを表示
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

'複数のアイテム選択可能なリストボックスを表示
Public Function MultiSelectListBox(ByVal lb_caption As String, list() As String, _
    ByVal lb_info As String, ByVal max_num As Integer) As Integer
Dim i As Integer, j As Integer
    
    'ステータスウィンドウに攻撃の命中率などを表示させないようにする
    CommandState = "ユニット選択"
    
    'リストボックスを作成して表示
    Load frmMultiSelectListBox
    With frmMultiSelectListBox
        .Caption = lb_caption
        .lblCaption = "　" & lb_info
        MaxListItem = max_num
        For i = 1 To UBound(list)
            .lstItems.AddItem "　" & list(i)
        Next
        .cmdSort.Caption = "名称順に並べ替え"
        .Left = MainForm.Left
        .Top = (Screen.Height - .Height) / 2
        .Show 1
    End With
    
    '選択された項目数を返す
    j = 0
    For i = 1 To UBound(list)
        If ListItemFlag(i) Then
            j = j + 1
        End If
    Next
    MultiSelectListBox = j
    
    'リストボックスを消去
    Unload frmMultiSelectListBox
    Set frmMultiSelectListBox = Nothing
End Function


' === 画像描画に関する処理 ===

'画像をウィンドウに描画
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
    
    '初回実行時に各種情報の初期化を行う
    If Not init_draw_pitcure Then
        '各フォルダにBitmapフォルダがあるかチェック
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
        
        '画面の色数を参照
        display_byte_pixel = GetDeviceCaps(MainForm.picMain(0).hDC, BITSPIXEL) \ 8
        
        init_draw_pitcure = True
    End If
    
    'ダミーのファイル名？
    Select Case fname
        Case "", "-.bmp", "EFFECT_Void.bmp"
            Exit Function
    End Select
    
    'Debug.Print fname, draw_option
    
    'オプションの解析
    BGColor = vbWhite
    'マスク画像に影響しないオプション
    pic_option = ""
    'マスク画像に影響するオプション
    pic_option2 = ""
    'フィルタ時の透過度を初期化
    trans_par = -1
    i = 1
    Do While i <= LLength(draw_option)
        opt = LIndex(draw_option, i)
        Select Case opt
            Case "背景"
                permanent = True
                '背景書き込みで夜やセピア色のマップの場合は指定がなくても特殊効果を付ける
                Select Case MapDrawMode
                    Case "夜"
                        dark_count = dark_count + 1
                        pic_option = pic_option & " 暗"
                    Case "白黒"
                        is_monotone = True
                        pic_option = pic_option & " 白黒"
                    Case "セピア"
                        is_sepia = True
                        pic_option = pic_option & " セピア"
                    Case "夕焼け"
                        is_sunset = True
                        pic_option = pic_option & " 夕焼け"
                    Case "水中"
                        is_water = True
                        pic_option = pic_option & " 水中"
                    Case "フィルタ"
                        is_colorfilter = True
                        fcolor = MapDrawFilterColor
                        pic_option2 = pic_option2 & " フィルタ=" & CStr(MapDrawFilterColor)
                End Select
            Case "透過"
                transparent = True
                pic_option = pic_option & " " & opt
            Case "白黒"
                is_monotone = True
                pic_option = pic_option & " " & opt
            Case "セピア"
                is_sepia = True
                pic_option = pic_option & " " & opt
            Case "夕焼け"
                is_sunset = True
                pic_option = pic_option & " " & opt
            Case "水中"
                is_water = True
                pic_option = pic_option & " " & opt
            Case "明"
                bright_count = bright_count + 1
                pic_option = pic_option & " " & opt
            Case "暗"
                dark_count = dark_count + 1
                pic_option = pic_option & " " & opt
            Case "左右反転"
                hrev = True
                pic_option2 = pic_option2 & " " & opt
            Case "上下反転"
                vrev = True
                pic_option2 = pic_option2 & " " & opt
            Case "ネガポジ反転"
                negpos = True
                pic_option = pic_option & " " & opt
            Case "シルエット"
                is_sil = True
                pic_option = pic_option & " " & opt
            Case "上半分"
                top_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "下半分"
                bottom_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "右半分"
                right_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "左半分"
                left_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "右上"
                tright_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "左上"
                tleft_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "右下"
                bright_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "左下"
                bleft_part = True
                pic_option2 = pic_option2 & " " & opt
            Case "メッセージ"
                on_msg_window = True
            Case "ステータス"
                on_status_window = True
            Case "保持"
                keep_picture = True
            Case "右回転"
                i = i + 1
                angle = StrToLng(LIndex(draw_option, i))
                pic_option2 = pic_option2 & " 右回転=" & Format$(angle Mod 360)
            Case "左回転"
                i = i + 1
                angle = -StrToLng(LIndex(draw_option, i))
                pic_option2 = pic_option2 & " 右回転=" & Format$(angle Mod 360)
            Case "フィルタ"
                is_colorfilter = True
            Case Else
                If Right(opt, 1) = "%" And IsNumeric(Left(opt, Len(opt) - 1)) Then
                    trans_par = MaxDbl(0, MinDbl(1, CDbl(Left(opt, Len(opt) - 1)) / 100))
                    pic_option2 = pic_option2 & " フィルタ透過度=" & opt
                Else
                    If is_colorfilter Then
                        fcolor = CLng(opt)
                        pic_option2 = pic_option2 & " フィルタ=" & opt
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
    
    '描画先を設定
    If on_msg_window Then
        'メッセージウィンドウへのパイロット画像の描画
        Set pic = frmMessage.picFace
        permanent = False
    ElseIf on_status_window Then
        'ステータスウィンドウへのパイロット画像の描画
        Set pic = MainForm.picUnitStatus
    ElseIf permanent Then
        '背景への描画
        Set pic = MainForm.picBack
    Else
        'マップウィンドウへの通常の描画
        Set pic = MainForm.picMain(0)
        SaveScreen
    End If
    
    '読み込むファイルの探索
    
    '前回の画像ファイルと同じ？
    If fname = last_fname Then
        '前回ファイルは見つかっていたのか？
        If Not last_exists Then
            DrawPicture = False
            Exit Function
        End If
    End If
    
    '以前表示した拡大画像が利用可能？
    For i = 0 To ImageBufferSize - 1
        '同じファイル？
        If PicBufFname(i) = fname Then
            'オプションも同じ？
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
                '同じファイル、オプションによる画像が見つかった
                
                '以前表示した画像をそのまま利用
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
    
    '以前表示した画像が利用可能？
    For i = 0 To ImageBufferSize - 1
        '同じファイル？
        If PicBufFname(i) = fname Then
            'オプションも同じ？
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
                '同じファイル、オプションによる画像が見つかった
                
                '以前表示した画像をそのまま利用
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
    
    '以前使用した部分画像が利用可能？
    If sw <> 0 Then
        For i = 0 To ImageBufferSize - 1
            '同じファイル？
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
                    '以前使用した部分画像をそのまま利用
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
    
    '以前使用した原画像が利用可能？
    For i = 0 To ImageBufferSize - 1
        '同じファイル？
        If PicBufFname(i) = fname Then
            If PicBufOption(i) = "" _
                And PicBufOption2(i) = "" _
                And Not PicBufIsMask(i) _
                And PicBufDW(i) = DEFAULT_LEVEL _
                And PicBufDH(i) = DEFAULT_LEVEL _
                And PicBufSW(i) = 0 _
            Then
                '以前使用した原画像をそのまま利用
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
    
    '特殊なファイル名
    Select Case LCase$(fname)
        Case "black.bmp", "event\black.bmp"
            '黒で塗りつぶし
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
            '白で塗りつぶし
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
            '味方ユニットタイル
            Set orig_pic = MainForm.picUnit
            orig_width = 32
            orig_height = 32
            GoTo LoadedOrigPicture
        Case "common\effect_tile(enemy).bmp", "anime\common\effect_tile(enemy).bmp"
            '敵ユニットタイル
            Set orig_pic = MainForm.picEnemy
            orig_width = 32
            orig_height = 32
            GoTo LoadedOrigPicture
        Case "common\effect_tile(neutral).bmp", "anime\common\effect_tile(neutral).bmp"
            '中立ユニットタイル
            Set orig_pic = MainForm.picNeautral
            orig_width = 32
            orig_height = 32
            GoTo LoadedOrigPicture
    End Select
    
    'フルパスで指定されている？
    If InStr(fname, ":") = 2 Then
        fpath = ""
        last_path = ""
        '登録を避けるため
        in_history = True
        GoTo FoundPicture
    End If
    
    '履歴を検索してみる
    On Error GoTo NotFound
    fpath = fpath_history.Item(fname)
    last_path = ""
    
    '履歴上にファイルを発見
    On Error GoTo 0
    If fpath = "" Then
        'ファイルは存在しない
        last_fname = fname
        last_exists = False
        DrawPicture = False
        Exit Function
    End If
    in_history = True
    GoTo FoundPicture
    
NotFound:
    
    '履歴になかった
    On Error GoTo 0
    
    '戦闘アニメ用？
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
    
    '前回と同じパス？
    If Len(last_path) > 0 Then
        If FileExists(last_path & fname) Then
            fpath = last_path
            GoTo FoundPicture
        End If
    End If
    
    'パス名入り？
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
    
    'フォルダ指定あり？
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
        '地形画像検索用の地形画像ディレクトリ名と4桁ファイル名を作成
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
    
    '各フォルダを検索する
    
    'Bitmapフォルダに直置き
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
    
    'シナリオフォルダ
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
                    '登録を避けるため
                    in_history = True
                    GoTo FoundPicture
                End If
                If FileExists(ScenarioPath & "Bitmap\Map\" & tname) Then
                    fname = tname
                    fpath = ScenarioPath & "Bitmap\Map\"
                    last_path = fpath
                    '登録を避けるため
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
                    '登録を避けるため
                    in_history = True
                    GoTo FoundPicture
                End If
                If FileExists(ExtDataPath & "Bitmap\Map\" & tname) Then
                    fname = tname
                    fpath = ExtDataPath & "Bitmap\Map\"
                    last_path = ""
                    '登録を避けるため
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
                    '登録を避けるため
                    in_history = True
                    GoTo FoundPicture
                End If
                If FileExists(ExtDataPath2 & "Bitmap\Map\" & tname) Then
                    fname = tname
                    fpath = ExtDataPath2 & "Bitmap\Map\"
                    last_path = ""
                    '登録を避けるため
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
    
    '本体側フォルダ
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
            '登録を避けるため
            in_history = True
            GoTo FoundPicture
        End If
        If FileExists(AppPath & "Bitmap\Map\" & tname) Then
            fname = tname
            fpath = AppPath & "Bitmap\Map\"
            last_path = ""
            '登録を避けるため
            in_history = True
            GoTo FoundPicture
        End If
    End If
    If FileExists(AppPath & "Bitmap\Map\" & fname) Then
        fpath = AppPath & "Bitmap\Map\"
        last_path = ""
        GoTo FoundPicture
    End If
    
    '見つからなかった……
    
    '履歴に記録しておく
    fpath_history.Add "", fname
    
    '表示を中止
    last_fname = fname
    last_exists = False
    DrawPicture = False
    Exit Function
    
FoundPicture:
    
    'ファイル名を記録しておく
    last_fname = fname
    
    '履歴に記録しておく
    If Not in_history Then
        fpath_history.Add fpath, fname
    End If
    
    last_exists = True
    pfname = fpath & fname
    
    '使用するバッファを選択
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
    
    '読み込んだ画像のサイズ(バイト数)をバッファ情報に記録しておく
    With orig_pic
        PicBufSize(i) = display_byte_pixel * .width * .Height
    End With
    
LoadedOrigPicture:
    
    With orig_pic
        orig_width = .width
        orig_height = .Height
    End With
    
    '原画像の一部のみを描画？
    If sw <> 0 Then
        If sw <> orig_width Or sh <> orig_height Then
            '使用するpicBufを選択
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
            
            '原画像から描画部分をコピー
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
    
    '原画像を修正して使う場合は原画像を別のpicBufにコピーして修正する
    If top_part Or bottom_part Or left_part Or right_part _
        Or tleft_part Or tright_part Or bleft_part Or bright_part _
        Or is_monotone Or is_sepia Or is_sunset Or is_water _
        Or negpos Or is_sil Or vrev Or hrev _
        Or bright_count > 0 Or dark_count > 0 Or angle Mod 360 <> 0 _
        Or is_colorfilter _
    Then
        '使用するpicBufを選択
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
        
        '画像をコピー
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
    
    '画像の一部を塗りつぶして描画する場合
    If top_part Then
        '上半分
        orig_pic.Line (0, orig_height \ 2)-(orig_width - 1, orig_height - 1), BGColor, BF
    End If
    If bottom_part Then
        '下半分
        orig_pic.Line (0, 0)-(orig_width - 1, orig_height \ 2 - 1), BGColor, BF
    End If
    If left_part Then
        '左半分
        orig_pic.Line (orig_width \ 2, 0)-(orig_width - 1, orig_height - 1), BGColor, BF
    End If
    If right_part Then
        '右半分
        orig_pic.Line (0, 0)-(orig_width \ 2 - 1, orig_height - 1), BGColor, BF
    End If
    If tleft_part Then
        '左上
        For i = 0 To orig_width - 1
            orig_pic.Line (i, orig_height - 1 - i)-(i, orig_height - 1), BGColor, B
        Next
    End If
    If tright_part Then
        '右上
        For i = 0 To orig_width - 1
            orig_pic.Line (i, i)-(i, orig_height - 1), BGColor, B
        Next
    End If
    If bleft_part Then
        '左下
        For i = 0 To orig_width - 1
            orig_pic.Line (i, 0)-(i, i), BGColor, B
        Next
    End If
    If bright_part Then
        '右下
        For i = 0 To orig_width - 1
            orig_pic.Line (i, 0)-(i, orig_height - 1 - i), BGColor, B
        Next
    End If
    
    '特殊効果
    If is_monotone Or is_sepia _
        Or is_sunset Or is_water _
        Or is_colorfilter _
        Or bright_count > 0 Or dark_count > 0 _
        Or negpos Or is_sil _
        Or vrev Or hrev _
        Or angle <> 0 _
    Then
        '画像のサイズをチェック
        If orig_width * orig_height Mod 4 <> 0 Then
            ErrorMessage fname & "の画像サイズが4の倍数になっていません"
            Exit Function
        End If
        
        'イメージをバッファに取り込み
        GetImage orig_pic
        
        '白黒
        If is_monotone Then
            Monotone transparent
        End If
        
        'セピア
        If is_sepia Then
            Sepia transparent
        End If
        
        '夕焼け
        If is_sunset Then
            Sunset transparent
        End If
        
        '水中
        If is_water Then
            Water transparent
        End If
        
        'シルエット
        If is_sil Then
            Silhouette
        End If
        
        'ネガポジ反転
        If negpos Then
            NegPosReverse transparent
        End If
        
        'フィルタ
        If is_colorfilter Then
            If trans_par < 0 Then
                trans_par = 0.5
            End If
            ColorFilter fcolor, trans_par, transparent
        End If
        
        '明 (多段指定可能)
        For i = 1 To bright_count
            Bright transparent
        Next
        
        '暗 (多段指定可能)
        For i = 1 To dark_count
            Dark transparent
        Next
        
        '左右反転
        If vrev Then
            VReverse
        End If
        
        '上下反転
        If hrev Then
            HReverse
        End If
        
        '回転
        If angle <> 0 Then
            '前回の回転角が90度の倍数かどうかで描画の際の最適化使用可否を決める
            '(連続で回転させる場合に描画速度を一定にするため)
            Rotate angle, last_angle Mod 90 <> 0
        End If
        
        '変更した内容をイメージに変換
        SetImage orig_pic
        
        'バッファを破棄
        ClearImage
    End If
    last_angle = angle
    
EditedPicture:
    
    'クリッピング処理
    If dw = DEFAULT_LEVEL Then
        dw = orig_width
    End If
    If dh = DEFAULT_LEVEL Then
        dh = orig_height
    End If
    If permanent Then
        '背景描画の場合、センタリングはマップ中央に
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
        'ユニット上で画像のセンタリングを行うことを意図している
        '場合は修正が必要
        If InStr(fname, "EFFECT_") > 0 _
            Or InStr(fname, "スペシャルパワー\") > 0 _
            Or InStr(fname, "精神コマンド\") > 0 _
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
            '通常描画の場合、センタリングは画面中央に
            If dx = DEFAULT_LEVEL Then
                dx = (MainPWidth - dw) \ 2
            End If
            If dy = DEFAULT_LEVEL Then
                dy = (MainPHeight - dh) \ 2
            End If
        End If
    End If
    
    '描画先が画面外の場合や描画サイズが0の場合は画像のロードのみを行う
    With pic
        If dx >= .width Or dy >= .Height _
            Or dx + dw <= 0 Or dy + dh <= 0 _
            Or dw <= 0 Or dh <= 0 _
        Then
            load_only = True
        End If
    End With
    
    '描画を最適化するため、描画方法を細かく分けている。
    '描画方法は以下の通り。
    '(1) BitBltでそのまま描画 (拡大処理なし、透過処理なし)
    '(2) 拡大画像を作ってからバッファリングして描画 (拡大処理あり、透過処理なし)
    '(3) 拡大画像を作らずにStretchBltで直接拡大描画 (拡大処理あり、透過処理なし)
    '(4) TransparentBltで拡大透過描画 (拡大処理あり、透過処理あり)
    '(5) 原画像をそのまま透過描画 (拡大処理なし、透過処理あり)
    '(6) 拡大画像を作ってからバッファリングして透過描画 (拡大処理あり、透過処理あり)
    '(7) 拡大画像を作ってからバッファリングせずに透過描画 (拡大処理あり、透過処理あり)
    '(8) 拡大画像を作らずにStretchBltで直接拡大透過描画 (拡大処理あり、透過処理あり)
    
    '画面に描画する
    If Not transparent _
        And dw = orig_width And dh = orig_height _
    Then
        '原画像をそのまま描画
        
        '描画をキャンセル？
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '画像を描画先に描画
        ret = BitBlt(pic.hDC, _
            dx, dy, dw, dh, _
            orig_pic.hDC, 0, 0, _
            SRCCOPY)
    ElseIf KeepStretchedImage _
        And Not transparent _
        And (Not found_orig Or load_only) _
        And dw <= 480 And dh <= 480 _
    Then
        '拡大画像を作成し、バッファリングして描画
        
        '拡大画像に使用するpicBufを選択
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
        
        'バッファの初期化
        Set stretched_pic = MainForm.picBuf(i)
        With stretched_pic
            .Picture = LoadPicture("")
            .width = dw
            .Height = dh
        End With
        
        'バッファに拡大した画像を保存
        ret = StretchBlt(stretched_pic.hDC, _
            0, 0, dw, dh, _
            orig_pic.hDC, _
            0, 0, orig_width, orig_height, _
            SRCCOPY)
        
        '描画をキャンセル？
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '拡大した画像を描画先に描画
        ret = BitBlt(pic.hDC, _
            dx, dy, dw, dh, _
            stretched_pic.hDC, 0, 0, _
            SRCCOPY)
    ElseIf Not transparent Then
        '拡大画像を作らずにStretchBltで直接拡大描画
        
        '描画をキャンセル？
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '拡大した画像を描画先に描画
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
        'TransparentBltの方が高速に描画できる場合に限り
        'TransparentBltを使って拡大透過描画
        
        '描画をキャンセル？
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '画像を描画先に透過描画
        ret = TransparentBlt(pic.hDC, _
            dx, dy, dw, dh, _
            orig_pic.hDC, _
            0, 0, orig_width, orig_height, _
            BGColor)
    ElseIf dw = orig_width And dh = orig_height Then
        '原画像をそのまま透過描画
        
        '以前使用したマスク画像が利用可能？
        Set mask_pic = Nothing
        For i = 0 To ImageBufferSize - 1
            '同じファイル？
            If PicBufFname(i) = fname Then
                'オプションも同じ？
                If PicBufIsMask(i) _
                    And PicBufOption2(i) = pic_option2 _
                    And PicBufDW(i) = orig_width _
                    And PicBufDH(i) = orig_height _
                    And PicBufSX(i) = sx _
                    And PicBufSX(i) = sy _
                    And PicBufSW(i) = sw _
                    And PicBufSH(i) = sh _
                Then
                    '以前使用したマスク画像をそのまま利用
                    UsePicBuf i
                    Set mask_pic = MainForm.picBuf(i)
                    'Debug.Print "Reuse " & Format$(i) & " As Mask"
                    Exit For
                End If
            End If
        Next
        
        If mask_pic Is Nothing Then
            'マスク画像を新規に作成
            
            'マスク画像に使用するpicBufを選択
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
            
            'バッファの初期化
            Set mask_pic = MainForm.picBuf(i)
            With mask_pic
                .Picture = LoadPicture("")
                .width = orig_width
                .Height = orig_height
            End With
            
            'マスク画像を作成
            MakeMask orig_pic.hDC, mask_pic.hDC, orig_width, orig_height, BGColor
        End If
        
        '描画をキャンセル？
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '画像を透過描画
        If BGColor = vbWhite Then
            '背景色が白
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                mask_pic.hDC, 0, 0, _
                SRCERASE)
            
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                orig_pic.hDC, 0, 0, _
                SRCINVERT)
        Else
            '背景色が白以外
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
            
            'マスク画像が再利用できないのでバッファを開放
            ReleasePicBuf i
        End If
    ElseIf KeepStretchedImage _
        And (Not found_orig Or load_only) _
        And dw <= 480 And dh <= 480 _
    Then
        '拡大画像を作成し、バッファリングして透過描画
        
        '拡大画像用に使用するpicBufを選択
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
        
        'バッファの初期化
        Set stretched_pic = MainForm.picBuf(i)
        With stretched_pic
            .Picture = LoadPicture("")
            .width = dw
            .Height = dh
        End With
        
        'バッファに拡大した画像を保存
        ret = StretchBlt(stretched_pic.hDC, _
            0, 0, dw, dh, _
            orig_pic.hDC, _
            0, 0, orig_width, orig_height, _
            SRCCOPY)
        
        '以前使用した拡大マスク画像が利用可能？
        Set stretched_mask_pic = Nothing
        For i = 0 To ImageBufferSize - 1
            '同じファイル？
            If PicBufFname(i) = fname Then
                'オプションも同じ？
                If PicBufIsMask(i) _
                    And PicBufOption2(i) = pic_option2 _
                    And PicBufDW(i) = dw _
                    And PicBufDH(i) = dh _
                    And PicBufSX(i) = sx _
                    And PicBufSY(i) = sy _
                    And PicBufSW(i) = sw _
                    And PicBufSH(i) = sh _
                Then
                    '以前使用した拡大マスク画像をそのまま利用
                    UsePicBuf i
                    Set stretched_mask_pic = MainForm.picBuf(i)
                    'Debug.Print "Reuse " & Format$(i) & " As StretchedMask"
                    Exit For
                End If
            End If
        Next
        
        If stretched_mask_pic Is Nothing Then
            '拡大マスク画像を新規に作成
            
            'マスク画像用の領域を初期化
            Set mask_pic = MainForm.picTmp
            With mask_pic
                .Picture = LoadPicture("")
                .width = orig_width
                .Height = orig_height
            End With
            
            'マスク画像を作成
            MakeMask orig_pic.hDC, mask_pic.hDC, orig_width, orig_height, BGColor
            
            '拡大マスク画像に使用するpicBufを選択
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
            
            'バッファを初期化
            Set stretched_mask_pic = MainForm.picBuf(i)
            With stretched_mask_pic
                .Picture = LoadPicture("")
                .width = dw
                .Height = dh
            End With
            
            'バッファに拡大したマスク画像を保存
            ret = StretchBlt(stretched_mask_pic.hDC, _
                0, 0, dw, dh, _
                mask_pic.hDC, _
                0, 0, orig_width, orig_height, _
                SRCCOPY)
        End If
        
        '描画をキャンセル？
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '画像を透過描画
        If BGColor = vbWhite Then
            '背景色が白
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_mask_pic.hDC, 0, 0, _
                SRCERASE)
            
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_pic.hDC, 0, 0, _
                SRCINVERT)
        Else
            '背景色が白以外
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
            
            '拡大マスク画像が再利用できないのでバッファを開放
            ReleasePicBuf i
        End If
    ElseIf dw <= 480 And dh <= 480 Then
        '拡大画像を作成した後、バッファリングせずに透過描画
        
        '拡大画像用の領域を作成
        Set stretched_pic = MainForm.picStretchedTmp(0)
        With stretched_pic
            .width = dw
            .Height = dh
        End With
        
        'バッファに拡大した画像を保存
        ret = StretchBlt(stretched_pic.hDC, _
            0, 0, dw, dh, _
            orig_pic.hDC, _
            0, 0, orig_width, orig_height, _
            SRCCOPY)
        
        '以前使用したマスク画像が利用可能？
        Set mask_pic = Nothing
        For i = 0 To ImageBufferSize - 1
            '同じファイル？
            If PicBufFname(i) = fname Then
                'オプションも同じ？
                If PicBufIsMask(i) _
                    And PicBufOption2(i) = pic_option2 _
                    And PicBufDW(i) = orig_width _
                    And PicBufDH(i) = orig_height _
                    And PicBufSX(i) = sx _
                    And PicBufSX(i) = sy _
                    And PicBufSW(i) = sw _
                    And PicBufSH(i) = sh _
                Then
                    '以前使用したマスク画像をそのまま利用
                    UsePicBuf i
                    Set mask_pic = MainForm.picBuf(i)
                    'Debug.Print "Reuse " & Format$(i) & " As Mask"
                    Exit For
                End If
            End If
        Next
        
        If mask_pic Is Nothing Then
            '新規にマスク画像作成
            
            'マスク画像に使用するpicBufを選択
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
            
            'バッファを初期化
            Set mask_pic = MainForm.picBuf(i)
            With mask_pic
                .width = orig_width
                .Height = orig_height
            End With
            
            'マスク画像を作成
            MakeMask orig_pic.hDC, mask_pic.hDC, orig_width, orig_height, BGColor
        End If
        
        '拡大マスク画像用の領域を作成
        Set stretched_mask_pic = MainForm.picStretchedTmp(1)
        With stretched_mask_pic
            .Picture = LoadPicture("")
            .width = dw
            .Height = dh
        End With
        
        'マスク画像を拡大して拡大マスク画像を作成
        ret = StretchBlt(stretched_mask_pic.hDC, _
            0, 0, dw, dh, _
            mask_pic.hDC, _
            0, 0, orig_width, orig_height, _
            SRCCOPY)
        
        '描画をキャンセル？
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '画像を透過描画
        If BGColor = vbWhite Then
            '背景色が白
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_mask_pic.hDC, 0, 0, _
                SRCERASE)
            
            ret = BitBlt(pic.hDC, _
                dx, dy, dw, dh, _
                stretched_pic.hDC, 0, 0, _
                SRCINVERT)
        Else
            '背景色が白以外
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
        
        '使用した一時画像領域を開放
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
        '拡大画像を作成せず、StretchBltで直接拡大透過描画
        
        '以前使用したマスク画像が利用可能？
        Set mask_pic = Nothing
        For i = 0 To ImageBufferSize - 1
            '同じファイル？
            If PicBufFname(i) = fname Then
                'オプションも同じ？
                If PicBufIsMask(i) _
                    And PicBufOption2(i) = pic_option2 _
                    And PicBufDW(i) = orig_width _
                    And PicBufDH(i) = orig_height _
                    And PicBufSX(i) = sx _
                    And PicBufSX(i) = sy _
                    And PicBufSW(i) = sw _
                    And PicBufSH(i) = sh _
                Then
                    '以前使用したマスク画像をそのまま利用
                    UsePicBuf i
                    Set mask_pic = MainForm.picBuf(i)
                    'Debug.Print "Reuse " & Format$(i) & " As Mask"
                    Exit For
                End If
            End If
        Next
        
        If mask_pic Is Nothing Then
            '新規にマスク画像作成
            
            'マスク画像に使用するpicBufを選択
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
            
            'バッファを初期化
            Set mask_pic = MainForm.picBuf(i)
            With mask_pic
                .width = orig_width
                .Height = orig_height
            End With
            
            'マスク画像を作成
            MakeMask orig_pic.hDC, mask_pic.hDC, orig_width, orig_height, BGColor
        End If
        
        '描画をキャンセル？
        If load_only Then
            DrawPicture = True
            Exit Function
        End If
        
        '画像を透過描画
        If BGColor = vbWhite Then
            '背景色が白
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
            '背景色が白以外
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
            
            'マスク画像が再利用できないのでバッファを開放
            ReleasePicBuf i
        End If
    End If
    
DrewPicture:
    
    If permanent Then
        '背景への描き込み
        IsMapDirty = True
        With MainForm
            'マスク入り背景画像画面にも画像を描き込む
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
        '表示画像を消去する際に使う描画領域を設定
        PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(dx, 0))
        PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(dy, 0))
        PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(dx + dw, MainPWidth - 1))
        PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(dy + dh, MainPHeight - 1))
        
        IsPictureDrawn = True
        IsPictureVisible = True
        IsCursorVisible = False
        
        If keep_picture Then
            'picMain(1)にも描画
            ret = BitBlt(MainForm.picMain(1).hDC, _
                dx, dy, dw, dh, _
                pic.hDC, dx, dy, _
                SRCCOPY)
        End If
    End If
    
    DrawPicture = True
End Function

'画像バッファを作成
Public Sub MakePicBuf()
Dim i As Integer

    '画像バッファ用のPictureBoxを動的に生成する
    With MainForm
        For i = 1 To ImageBufferSize - 1
            Load .picBuf(i)
        Next
    End With
    
    '画像バッファ管理用配列を初期化
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

'使用可能な画像バッファを検索
Private Function GetPicBuf(Optional ByVal buf_size As Long) As Integer
Dim total_size As Long, oldest_buf As Integer, used_buf_num As Integer
Dim i As Integer, tmp As Long
    
    '画像バッファの総サイズ及び使用されているバッファ数を調べる
    total_size = buf_size
    For i = 0 To ImageBufferSize - 1
        total_size = total_size + PicBufSize(i)
        If PicBufFname(i) <> "" Then
            used_buf_num = used_buf_num + 1
        End If
    Next
    
    '総サイズがMaxImageBufferByteSizeを超えてしまう場合は総サイズが
    'MaxImageBufferByteSize以下になるまでバッファを開放する。
    'ただし一度の描画で最大で5枚のバッファが使われるため、最新の4つの
    'バッファはキープしておく。
    Do While total_size > MaxImageBufferByteSize And used_buf_num > 4
        '最も長い間使われていないバッファを探す
        tmp = 100000000
        For i = 0 To ImageBufferSize - 1
            If PicBufFname(i) <> "" Then
                If PicBufDate(i) < tmp Then
                    oldest_buf = i
                    tmp = PicBufDate(i)
                End If
            End If
        Next
        
        'バッファを開放
        ReleasePicBuf oldest_buf
        used_buf_num = used_buf_num - 1
        
        '総サイズ数を減少させる
        total_size = total_size - PicBufSize(oldest_buf)
        PicBufSize(oldest_buf) = 0
    Loop
    
    '最も長い間使われていないバッファを探す
    GetPicBuf = 0
    For i = 1 To ImageBufferSize - 1
        If PicBufDate(i) < PicBufDate(GetPicBuf) Then
            GetPicBuf = i
        End If
    Next
    
    '画像のサイズを記録しておく
    PicBufSize(GetPicBuf) = buf_size
    
    '使用することを記録する
    UsePicBuf GetPicBuf
End Function

'画像バッファを開放する
Private Sub ReleasePicBuf(ByVal idx As Integer)
    PicBufFname(idx) = ""
    With MainForm.picBuf(idx)
        .Picture = LoadPicture("")
        .width = 32
        .Height = 32
    End With
End Sub

'画像バッファの使用記録をつける
Private Sub UsePicBuf(ByVal idx As Integer)
    PicBufDateCount = PicBufDateCount + 1
    PicBufDate(idx) = PicBufDateCount
End Sub


' === 文字列描画に関する処理 ===

'メインウィンドウに文字列を表示する
Public Sub DrawString(msg As String, ByVal X As Long, ByVal Y As Long, _
    Optional ByVal without_cr As Boolean)
Dim tx As Integer, ty As Integer
Dim prev_cx As Integer
Dim pic As PictureBox
Dim sf As StdFont
Static font_smoothing As Long
Static init_draw_string As Boolean

    If PermanentStringMode Then
        '背景書き込み
        Set pic = MainForm.picBack
        'フォント設定を変更
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
        '通常の書き込み
        Set pic = MainForm.picMain(0)
        SaveScreen
    End If
    
    'フォントがスムージング表示されているか参照
    If Not init_draw_string Then
        Call GetSystemParametersInfo(SPI_GETFONTSMOOTHING, 0, font_smoothing, 0)
        init_draw_string = True
    End If
    
    'フォントをスムージングするように設定
    If font_smoothing = 0 Then
        Call SetSystemParametersInfo(SPI_SETFONTSMOOTHING, 1, 0, 0)
    End If
    
    With pic
        '現在のX位置を記録しておく
        prev_cx = .CurrentX
        
        '書き込み先の座標を求める
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
            '改行あり
            pic.Print msg
            
            '背景書き込みの場合
            If PermanentStringMode Then
                With MainForm.picMaskedBack
                    .CurrentX = tx
                    .CurrentY = ty
                End With
                MainForm.picMaskedBack.Print msg
                IsMapDirty = True
            End If
            
            '保持オプション使用時
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
            
            '次回の書き込みのため、X座標位置を設定し直す
            If X <> DEFAULT_LEVEL Then
                .CurrentX = X
            Else
                .CurrentX = prev_cx
            End If
        Else
            '改行なし
            pic.Print msg;
            
            '背景書き込みの場合
            If PermanentStringMode Then
                With MainForm.picMaskedBack
                    .CurrentX = tx
                    .CurrentY = ty
                End With
                MainForm.picMaskedBack.Print msg;
                IsMapDirty = True
            End If
            
            '保持オプション使用時
            If KeepStringMode Then
                With MainForm.picMain(1)
                    .CurrentX = tx
                    .CurrentY = ty
                End With
                MainForm.picMain(1).Print msg;
            End If
        End If
    End With
    
    'フォントのスムージングに関する設定を元に戻す
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

'メインウィンドウに文字列を表示 (システムメッセージ)
Public Sub DrawSysString(ByVal X As Integer, ByVal Y As Integer, msg As String, _
    Optional ByVal without_refresh As Boolean)
Dim prev_color As Long
Dim prev_name As String
Dim prev_size As Integer
Dim is_bold As Boolean
Dim is_italic As Boolean
Dim sf As StdFont
    
    '表示位置が画面外？
    If X < MapX - MainWidth \ 2 Or MapX + MainWidth \ 2 < X _
        Or Y < MapY - MainHeight \ 2 Or MapY + MainHeight \ 2 < Y _
    Then
        Exit Sub
    End If
    
    SaveScreen
    
    With MainForm.picMain(0)
        '現在のフォント設定を保存
        prev_color = .ForeColor
        prev_size = .Font.Size
        prev_name = .Font.Name
        is_bold = .Font.Bold
        is_italic = .Font.Italic
        
        'フォント設定をシステム用に切り替え
        .ForeColor = vbBlack
        .FontTransparent = False
        If .Font.Name <> "ＭＳ Ｐ明朝" Then
            Set sf = New StdFont
            sf.Name = "ＭＳ Ｐ明朝"
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
        
        'メッセージの書き込み
        .CurrentX = MapToPixelX(X) + (32 - .TextWidth(msg)) \ 2 - 1
        .CurrentY = MapToPixelY(Y + 1) - .TextHeight(msg)
        MainForm.picMain(0).Print msg
        
        'フォント設定を元に戻す
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
        
        '表示を更新
        If Not without_refresh Then
            .Refresh
        End If
        
        PaintedAreaX1 = MinLng(PaintedAreaX1, MapToPixelX(X) - 4)
        PaintedAreaY1 = MaxLng(PaintedAreaY1, MapToPixelY(Y) + 16)
        PaintedAreaX2 = MinLng(PaintedAreaX2, MapToPixelX(X) + 36)
        PaintedAreaY2 = MaxLng(PaintedAreaY2, MapToPixelY(Y) + 32)
    End With
End Sub


' === 画像消去に関する処理 ===

'描画した画像を消去できるように元画像を保存する
Public Sub SaveScreen()
Dim ret As Long

    If Not ScreenIsSaved Then
        '画像をpicMain(1)に保存
        With MainForm
             ret = BitBlt(.picMain(1).hDC, 0, 0, MainPWidth, MainPHeight, _
                 .picMain(0).hDC, 0, 0, SRCCOPY)
        End With
        ScreenIsSaved = True
    End If
End Sub

'描画したグラフィックを消去
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

'描画したグラフィックの一部を消去
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


' === 画面ロックに関する処理 ===

'ＧＵＩをロックし、プレイヤーからの入力を無効にする
Public Sub LockGUI()
    IsGUILocked = True
    With MainForm
        .VScroll.Enabled = False
        .HScroll.Enabled = False
    End With
End Sub

'ＧＵＩのロックを解除し、プレイヤーからの入力を有効にする
Public Sub UnlockGUI()
    IsGUILocked = False
    With MainForm
        .VScroll.Enabled = True
        .HScroll.Enabled = True
    End With
End Sub


' === マウスカーソルの自動移動に関する処理 ===

'現在のマウスカーソルの位置を記録
Public Sub SaveCursorPos()
Dim PT As POINTAPI

    GetCursorPos PT
    PrevCursorX = PT.X
    PrevCursorY = PT.Y
    NewCursorX = 0
    NewCursorY = 0
End Sub

'マウスカーソルを移動する
Public Sub MoveCursorPos(cursor_mode As String, Optional ByVal t As Unit)
Dim tx As Long, ty As Long, i As Long, num As Long
Dim ret As Long, prev_lock As Boolean
Dim PT As POINTAPI
    
    'マウスカーソルの位置を収得
    GetCursorPos PT
    
    '現在の位置を記録しておく
    If PrevCursorX = 0 _
        And cursor_mode <> "メッセージウィンドウ" _
    Then
        SaveCursorPos
    End If
    
    'カーソル自動移動
    If t Is Nothing Then
        If cursor_mode = "メッセージウィンドウ" Then
            'メッセージウィンドウまで移動
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
            'リストボックスまで移動
            With frmListBox
                If PT.X < (.Left + 0.1 * .width) \ Screen.TwipsPerPixelX Then
                    tx = (.Left + 0.1 * .width) \ Screen.TwipsPerPixelX
                ElseIf PT.X > (.Left + 0.9 * .width) \ Screen.TwipsPerPixelX Then
                    tx = (.Left + 0.9 * .width) \ Screen.TwipsPerPixelX
                Else
                    tx = PT.X
                End If
                
                '選択するアイテム
                If cursor_mode = "武器選択" Then
                    '武器選択の場合は選択可能な最後のアイテムに
                    i = .lstItems.ListCount
                    Do
                        If Not ListItemFlag(i) And InStr(.lstItems.list(i), "援護攻撃：") = 0 Then
                            Exit Do
                        End If
                        i = i - 1
                    Loop While i > 1
                Else
                    'そうでなければ最初のアイテムに
                    i = .lstItems.TopIndex + 1
                End If
                
                ty = .Top \ Screen.TwipsPerPixelY _
                    + .Height \ Screen.TwipsPerPixelY - .ScaleHeight _
                    + .lstItems.Top _
                    + 16 * (i - .lstItems.TopIndex) - 8
            End With
        End If
    Else
        'ユニット上まで移動
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
    
    '何回に分けて移動するか計算
    num = Sqr((tx - PT.X) ^ 2 + (ty - PT.Y) ^ 2) \ 25 + 1
    
    'カーソルを移動
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
    
    '新しいカーソル位置を記録
    If NewCursorX = 0 Then
        NewCursorX = tx
        NewCursorY = ty
    End If
End Sub

'マウスカーソルを元の位置に戻す
Public Sub RestoreCursorPos()
Dim tx As Integer, ty As Integer, i As Integer, num As Integer
Dim ret As Long
Dim PT As POINTAPI
    
    'ユニットが選択されていればその場所まで戻す
    If Not SelectedUnit Is Nothing Then
        If SelectedUnit.Status = "出撃" Then
            MoveCursorPos "ユニット選択", SelectedUnit
            Exit Sub
        End If
    End If
    
    '戻るべき位置が設定されていない？
    If PrevCursorX = 0 And PrevCursorY = 0 Then
        Exit Sub
    End If
    
    '現在のカーソル位置収得
    GetCursorPos PT
    
    '以前の位置までカーソル自動移動
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
    
    '戻り位置を初期化
    PrevCursorX = 0
    PrevCursorY = 0
End Sub


' === タイトル画面表示に関する処理 ===

'タイトル画面を表示
Public Sub OpenTitleForm()
    Load frmTitle
    
    frmTitle.Left = (Screen.width - frmTitle.width) / 2
    frmTitle.Top = (Screen.Height - frmTitle.Height) / 2
    
    frmTitle.Show
    frmTitle.Refresh
End Sub

'タイトル画面を閉じる
Public Sub CloseTitleForm()
    Unload frmTitle
    Set frmTitle = Nothing
End Sub


' === 「Now Loading...」表示に関する処理 ===

'「Now Loading...」の画面を表示
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

'「Now Loading...」の画面を消去
Public Sub CloseNowLoadingForm()
    Unload frmNowLoading
    Set frmNowLoading = Nothing
    Screen.MousePointer = 0
End Sub

'「Now Loading...」のバーを１段階進行させる
Public Sub DisplayLoadingProgress()
    frmNowLoading.Progress
    DoEvents
End Sub

'「Now Loading...」のバーの長さを設定
Public Sub SetLoadImageSize(ByVal new_size As Integer)
    With frmNowLoading
        .Value = 0
        .Size = new_size
    End With
End Sub


' === 画面の解像度変更 ===

Public Sub ChangeDisplaySize(ByVal w As Integer, ByVal h As Integer)
Dim dm As DEVMODE
Dim ret As Long
Static orig_width As Integer, orig_height As Integer
    
    'DEVMODE構造体を初期化
    dm.dmSize = Len(dm)
    
    '現在のディスプレイ設定を参照
    ret = EnumDisplaySettings(vbNullString, ENUM_CURRENT_SETTINGS, dm)
    
    If w <> 0 And h <> 0 Then
        '画面の解像度を w x h に変更する場合
        
        '現在の解像度を記録しておく
        orig_width = dm.dmPelsWidth
        orig_height = dm.dmPelsHeight
        
        If dm.dmPelsWidth = w And dm.dmPelsHeight = h Then
            '既に使用したい解像度になっていればそのまま終了
            Exit Sub
        End If
        
        '画面の解像度を w x h に変更
        dm.dmPelsWidth = w
        dm.dmPelsHeight = h
    Else
        '画面の解像度を元の解像度に戻す場合
        
        If orig_width = 0 And orig_height Then
            '解像度を変更していなければ終了
            Exit Sub
        End If
        
        If dm.dmPelsWidth = orig_width And dm.dmPelsHeight = orig_width Then
            '解像度が変化していなければそのまま終了
            Exit Sub
        End If
        
        '画面の解像度を元に戻す
        ret = ChangeDisplaySettings(vbNull, 0)
        Exit Sub
    End If
    
    '解像度を変更可能かどうか調べる
    ret = ChangeDisplaySettings(dm, CDS_TEST)
    If ret <> DISP_CHANGE_SUCCESSFUL Then
        Exit Sub
    End If
    
    '解像度を実際に変更する
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
            '成功！
            Exit Sub
        Case DISP_CHANGE_RESTART
            '再起動が必要な場合はあきらめてもとの解像度に戻す
            ret = ChangeDisplaySettings(vbNull, 0)
    End Select
End Sub


' === その他 ===

'エラーメッセージを表示
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
    
    'メインウィンドウのクローズが行えるようにモーダルモードは使用しない
    Do While frmErrorMessage.Visible
        DoEvents
        Sleep 200
    Loop
    
    Unload frmErrorMessage
    Set frmErrorMessage = Nothing
End Sub

'データ読み込み時のエラーメッセージを表示する
Public Sub DataErrorMessage(msg As String, fname As String, ByVal line_num As Integer, _
    line_buf As String, dname As String)
Dim err_msg As String

    'エラーが発生したファイル名と行番号
    err_msg = fname & "：" & line_num & "行目" & vbCr & vbLf
    
    'エラーが発生したデータ名
    If Len(dname) > 0 Then
        err_msg = err_msg & dname & "のデータが不正です。" & vbCr & vbLf
    End If
    
    'エラーの原因
    If Len(msg) > 0 Then
        err_msg = err_msg & msg & vbCr & vbLf
    End If
    
    'なにも指定されていない？
    If dname = "" And msg = "" Then
        err_msg = err_msg & "データが不正です。" & vbCr & vbLf
    End If
    
    'エラーが発生したデータ行
    err_msg = err_msg & line_buf
    
    'エラーメッセージを表示
    ErrorMessage err_msg
End Sub


'マウスの右ボタンが押されているか(キャンセル)判定
Public Function IsRButtonPressed(Optional ByVal ignore_message_wait As Boolean) As Boolean
Dim PT As POINTAPI

    'メッセージがウエイト無しならスキップ
    If Not ignore_message_wait And MessageWait = 0 Then
        IsRButtonPressed = True
        Exit Function
    End If
    
    'メインウインドウ上でマウスボタンを押した場合
    If MainForm.hwnd = GetForegroundWindow Then
        GetCursorPos PT
        With MainForm
            If .Left \ Screen.TwipsPerPixelX <= PT.X _
                And PT.X <= (.Left + .width) \ Screen.TwipsPerPixelX _
                And .Top \ Screen.TwipsPerPixelY <= PT.Y _
                And PT.Y <= (.Top + .Height) \ Screen.TwipsPerPixelY _
            Then
                If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
                    '右ボタンでスキップ
                    IsRButtonPressed = True
                    Exit Function
                End If
            End If
        End With
    'メッセージウインドウ上でマウスボタンを押した場合
    ElseIf frmMessage.hwnd = GetForegroundWindow Then
        GetCursorPos PT
        With frmMessage
            If .Left \ Screen.TwipsPerPixelX <= PT.X _
                And PT.X <= (.Left + .width) \ Screen.TwipsPerPixelX _
                And .Top \ Screen.TwipsPerPixelY <= PT.Y _
                And PT.Y <= (.Top + .Height) \ Screen.TwipsPerPixelY _
            Then
                If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
                    '右ボタンでスキップ
                    IsRButtonPressed = True
                    Exit Function
                End If
            End If
        End With
    End If
End Function


'Telopコマンド用描画ルーチン
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

