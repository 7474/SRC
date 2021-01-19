Option Strict Off
Option Explicit On
Module GUI
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'ユーザーインターフェースと画面描画の処理を行うモジュール
	
	'MainのForm
	Public MainForm As System.Windows.Forms.Form
	Public IsFlashAvailable As Boolean
	
	' ADD START MARGE
	'GUIが新バージョンか
	Public NewGUIMode As Boolean
	' ADD END
	
	'マップ画面に表示できるマップのサイズ
	Public MainWidth As Short
	Public MainHeight As Short
	
	'マップ画面のサイズ（ピクセル）
	Public MainPWidth As Short
	Public MainPHeight As Short
	
	'マップのサイズ（ピクセル）
	Public MapPWidth As Short
	Public MapPHeight As Short
	
	'ＨＰ・ＥＮのゲージの幅（ピクセル）
	Public Const GauageWidth As Short = 88
	
	'現在マップウィンドウがマスク表示されているか
	Public ScreenIsMasked As Boolean
	'現在マップウィンドウが保存されているか
	Public ScreenIsSaved As Boolean
	
	'現在表示されているマップの座標
	Public MapX As Short
	Public MapY As Short
	
	'ドラッグ前のマップの座標
	Public PrevMapX As Short
	Public PrevMapY As Short
	
	'最後に押されたマウスボタン
	Public MouseButton As Short
	
	'現在のマウスの座標
	Public MouseX As Single
	Public MouseY As Single
	
	'ドラッグ前のマウスの座標
	Public PrevMouseX As Single
	Public PrevMouseY As Single
	
	'カーソル位置自動変更前のマウスカーソルの座標
	Private PrevCursorX As Short
	Private PrevCursorY As Short
	'カーソル位置自動変更後のマウスカーソルの座標
	Private NewCursorX As Short
	Private NewCursorY As Short
	
	'移動前のユニットの情報
	Public PrevUnitX As Short
	Public PrevUnitY As Short
	Public PrevUnitArea As String
	Public PrevCommand As String
	
	'PaintPictureで画像が描き込まれたか
	Public IsPictureDrawn As Boolean
	'PaintPictureで画像が描かれているか
	Public IsPictureVisible As Boolean
	'PaintPictureで描画した画像領域
	Public PaintedAreaX1 As Short
	Public PaintedAreaY1 As Short
	Public PaintedAreaX2 As Short
	Public PaintedAreaY2 As Short
	'カーソル画像が表示されているか
	Public IsCursorVisible As Boolean
	'背景色
	Public BGColor As Integer
	
	'画像バッファ管理用変数
	Private PicBufDateCount As Integer
	Private PicBufDate() As Integer
	Private PicBufSize() As Integer
	Private PicBufFname() As String
	Private PicBufOption() As String
	Private PicBufOption2() As String
	Private PicBufDW() As Short
	Private PicBufDH() As Short
	Private PicBufSX() As Short
	Private PicBufSY() As Short
	Private PicBufSW() As Short
	Private PicBufSH() As Short
	Private PicBufIsMask() As Boolean
	
	
	'GUIから入力可能かどうか
	Public IsGUILocked As Boolean
	
	'リストボックス内で表示位置
	Public TopItem As Short
	
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
	Public MessageWait As Integer
	
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
	Public MaxListItem As Short
	
	
	'API関数の定義
	
	Declare Function BitBlt Lib "gdi32" (ByVal hDestDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hSrcDC As Integer, ByVal xsrc As Integer, ByVal ysrc As Integer, ByVal dwRop As Integer) As Integer
	
	Declare Function StretchBlt Lib "gdi32" (ByVal hDestDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hSrcDC As Integer, ByVal xsrc As Integer, ByVal ysrc As Integer, ByVal nSrcWidth As Integer, ByVal nSrcHeight As Integer, ByVal dwRop As Integer) As Integer
	
	Declare Function PatBlt Lib "gdi32" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal dwRop As Integer) As Integer
	
	Public Const BLACKNESS As Integer = &H42
	Public Const DSTINVERT As Integer = &H550009
	Public Const MERGECOPY As Integer = &HC000CA
	Public Const MERGEPAINT As Integer = &HBB0226
	Public Const NOTSRCCOPY As Integer = &H330008
	Public Const NOTSRCERASE As Integer = &H1100A6
	Public Const PATCOPY As Integer = &HF00021
	Public Const PATINVERT As Integer = &H5A0049
	Public Const PATPAINT As Integer = &HFB0A09
	Public Const SRCAND As Integer = &H8800C6
	Public Const SRCCOPY As Integer = &HCC0020
	Public Const SRCERASE As Integer = &H440328
	Public Const SRCINVERT As Integer = &H660046
	Public Const SRCPAINT As Integer = &HEE0086
	Public Const WHITENESS As Integer = &HFF0062
	'ADD START 240a
	Public Const STATUSBACK As Integer = &HC0C0C0
	'ADD START 240a
	
	'StretchBltのモード設定を行う
	Declare Function GetStretchBltMode Lib "gdi32" (ByVal hDC As Integer) As Integer
	Declare Function SetStretchBltMode Lib "gdi32" (ByVal hDC As Integer, ByVal nStretchMode As Integer) As Integer
	
	Public Const STRETCH_ANDSCANS As Short = 1
	Public Const STRETCH_ORSCANS As Short = 2
	Public Const STRETCH_DELETESCANS As Short = 3
	Public Const STRETCH_HALFTONE As Short = 4
	
	'透過描画
	Declare Function TransparentBlt Lib "msimg32.dll" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hSrcDC As Integer, ByVal xsrc As Integer, ByVal ysrc As Integer, ByVal nSrcWidth As Integer, ByVal nSrcHeight As Integer, ByVal crTransparent As Integer) As Integer
	
	'ウィンドウ位置の設定
	Declare Function SetWindowPos Lib "user32" (ByVal hwnd As Integer, ByVal hWndInsertAfter As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer
	
	Public Const SW_SHOWNA As Short = 8 '非アクティブで表示
	
	'フォームをアクティブにしないで表示
	Declare Function ShowWindow Lib "user32" (ByVal hwnd As Integer, ByVal nCmdShow As Integer) As Integer
	
	Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Integer)
	
	'カーソル位置取得
	'UPGRADE_WARNING: 構造体 POINTAPI に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function GetCursorPos Lib "user32" (ByRef lpPoint As POINTAPI) As Integer
	
	'ポイント構造体
	Structure POINTAPI
		Dim X As Integer
		Dim Y As Integer
	End Structure
	
	'カーソル位置設定
	Declare Function SetCursorPos Lib "user32" (ByVal X As Integer, ByVal Y As Integer) As Integer
	
	'キーの情報を得る
	Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Integer) As Short
	
	Public RButtonID As Integer
	Public LButtonID As Integer
	
	'システムメトリックスを取得するAPI
	Declare Function GetSystemMetrics Lib "user32" (ByVal nIndex As Integer) As Integer
	
	Public Const SM_SWAPBUTTON As Short = 23 '左右のボタンが交換されているか否か
	
	'現在アクティブなウィンドウを取得するAPI
	Public Declare Function GetForegroundWindow Lib "user32" () As Integer
	
	'直線を描画するためのAPI
	'UPGRADE_WARNING: 構造体 POINTAPI に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function MoveToEx Lib "gdi32" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByRef lpPoint As POINTAPI) As Integer
	Declare Function LineTo Lib "gdi32" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer) As Integer
	
	'多角形を描画するAPI
	'UPGRADE_WARNING: 構造体 POINTAPI に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Public Declare Function Polygon Lib "gdi32.dll" (ByVal hDC As Integer, ByRef lpPoint As POINTAPI, ByVal nCount As Integer) As Integer
	
	
	'ディスプレイの設定を参照するAPI
	Public Structure DEVMODE
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(32),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=32)> Public dmDeviceName() As Char
		Dim dmSpecVersion As Short
		Dim dmDriverVersion As Short
		Dim dmSize As Short
		Dim dmDriverExtra As Short
		Dim dmFields As Integer
		Dim dmOrientation As Short
		Dim dmPaperSize As Short
		Dim dmPaperLength As Short
		Dim dmPaperWidth As Short
		Dim dmScale As Short
		Dim dmCopies As Short
		Dim dmDefaultSource As Short
		Dim dmPrintQuality As Short
		Dim dmColor As Short
		Dim dmDuplex As Short
		Dim dmYResolution As Short
		Dim dmTTOption As Short
		Dim dmCollate As Short
		'UPGRADE_WARNING: 固定長文字列のサイズはバッファに合わせる必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="3C1E4426-0B80-443E-B943-0627CD55D48B"' をクリックしてください。
		<VBFixedString(32),System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.ByValArray,SizeConst:=32)> Public dmFormName() As Char
		Dim dmUnusedPadding As Short
		Dim dmBitsPerPixel As Short
		Dim dmPelsWidth As Integer
		Dim dmPelsHeight As Integer
		Dim dmDisplayFlags As Integer
		Dim dmDisplayFrequency As Integer
		Dim dmICMMethod As Integer
		Dim dmICMIntent As Integer
		Dim dmMediaType As Integer
		Dim dmDitherType As Integer
		Dim dmReserved1 As Integer
		Dim dmReserved2 As Integer
		Dim dmPanningWidth As Integer
		Dim dmPanningHeight As Integer
	End Structure
	
	'UPGRADE_WARNING: 構造体 DEVMODE に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Public Declare Function EnumDisplaySettings Lib "user32.dll"  Alias "EnumDisplaySettingsA"(ByVal lpszDeviceName As String, ByVal iModeNum As Integer, ByRef lpDevMode As DEVMODE) As Integer
	
	Public Const ENUM_CURRENT_SETTINGS As Short = -1
	
	'ディスプレイの設定を変更するためのAPI
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	Public Declare Function ChangeDisplaySettings Lib "user32.dll"  Alias "ChangeDisplaySettingsA"(ByRef lpDevMode As Any, ByVal dwFlags As Integer) As Integer
	
	Public Const CDS_UPDATEREGISTRY As Integer = &H1
	Public Const CDS_TEST As Integer = &H2
	Public Const CDS_FULLSCREEN As Integer = &H4
	Public Const DISP_CHANGE_SUCCESSFUL As Short = 0
	Public Const DISP_CHANGE_RESTART As Short = 1
	
	'デバイスの設定を参照するためのAPI
	Public Declare Function GetDeviceCaps Lib "gdi32" (ByVal hDC As Integer, ByVal nIndex As Integer) As Integer
	
	'ピクセル当たりのカラービット数
	Private Const BITSPIXEL As Short = 12
	
	
	'システムパラメータを変更するためのAPI
	Declare Function SetSystemParametersInfo Lib "user32.dll"  Alias "SystemParametersInfoA"(ByVal uiAction As Integer, ByVal uiParam As Integer, ByVal pvParam As Integer, ByVal fWinIni As Integer) As Integer
	
	Declare Function GetSystemParametersInfo Lib "user32.dll"  Alias "SystemParametersInfoA"(ByVal uiAction As Integer, ByVal uiParam As Integer, ByRef pvParam As Integer, ByVal fWinIni As Integer) As Integer
	
	'フォントのスムージング処理関連の定数
	Public Const SPI_GETFONTSMOOTHING As Short = 74
	Public Const SPI_SETFONTSMOOTHING As Short = 75
	
	'ユーザープロファイルの更新を指定
	Public Const SPIF_UPDATEINIFILE As Integer = &H1
	'すべてのトップレベルウィンドウに変更を通知
	Public Const SPIF_SENDWININICHANGE As Integer = &H2
	
	
	'メインウィンドウのロードとFlashの登録を行う
	Public Sub LoadMainFormAndRegisterFlash()
		Dim WSHShell As Object
		
		On Error GoTo ErrorHandler
		
		'シェルからregsvr32.exeを利用して、起動ごとにSRC.exeと同じパスにある
		'FlashControl.ocxを再登録する。
		WSHShell = CreateObject("WScript.Shell")
		'UPGRADE_WARNING: オブジェクト WSHShell.Run の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		WSHShell.Run("regsvr32.exe /s """ & AppPath & "FlashControl.ocx""", 0, True)
		'UPGRADE_NOTE: オブジェクト WSHShell をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		WSHShell = Nothing
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmMain)
		
		MainForm = frmMain
		IsFlashAvailable = True
		
		Exit Sub
		
ErrorHandler: 
		
		'Flashが使えないのでFlash無しのメインウィンドウを使用する
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmSafeMain)
		MainForm = frmSafeMain
	End Sub
	
	'各ウィンドウをロード
	'ただしメインウィンドウはあらかじめLoadMainFormAndRegisterFlashでロードしておくこと
	Public Sub LoadForms()
		Dim X, Y As Short
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmToolTip)
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmMessage)
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmListBox)
		
		LockGUI()
		CommandState = "ユニット選択"
		
		'マップ画面に表示できるマップのサイズ
		Select Case LCase(ReadIni("Option", "NewGUI"))
			Case "on"
				' MOD START MARGE
				NewGUIMode = True
				' MOD END MARGE
				MainWidth = 20
			Case "off"
				MainWidth = 15
			Case Else
				MainWidth = 15
				WriteIni("Option", "NewGUI", "Off")
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
			X = VB6.TwipsPerPixelX
			Y = VB6.TwipsPerPixelY
			' MOD START MARGE
			'        If MainWidth = 15 Then
			If Not NewGUIMode Then
				' MOD END MARGE
				.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(.Width) - (VB6.PixelsToTwipsX(.ClientRectangle.Width) * X) + (MainPWidth + 24 + 225 + 4) * X)
				.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - (VB6.PixelsToTwipsY(.ClientRectangle.Height) * Y) + (MainPHeight + 24) * Y)
			Else
				.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(.Width) - (VB6.PixelsToTwipsX(.ClientRectangle.Width) * X) + MainPWidth * X)
				.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - (VB6.PixelsToTwipsY(.ClientRectangle.Height) * Y) + MainPHeight * Y)
			End If
			.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
			.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			
			'スクロールバーの位置を設定
			' MOD START MARGE
			'        If MainWidth = 15 Then
			If Not NewGUIMode Then
				' MOD END MARGE
				'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.VScroll.Move(MainPWidth + 4, 4, 16, MainPWidth)
				'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.HScroll.Move(4, MainPHeight + 4, MainPWidth, 16)
			Else
				'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.VScroll.Visible = False
				'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.HScroll.Visible = False
			End If
			
			'ステータスウィンドウを設置
			' MOD START MARGE
			'        If MainWidth = 15 Then
			'            .picFace.Move MainPWidth + 24, 4
			'            .picPilotStatus.Move MainPWidth + 24 + 68 + 4, 4, 155, 72
			'            .picUnitStatus.Move MainPWidth + 24, 4 + 68 + 4, _
			''                225 + 5, MainPHeight - 64 + 16
			'        Else
			'            .picUnitStatus.Move MainPWidth - 230 - 10, 10, 230, MainPHeight - 20
			'            .picUnitStatus.Visible = False
			'            .picPilotStatus.Visible = False
			'            .picFace.Visible = False
			'        End If
			If NewGUIMode Then
				'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picUnitStatus.Move(MainPWidth - 230 - 10, 10, 230, MainPHeight - 20)
				'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picUnitStatus.Visible = False
				'UPGRADE_ISSUE: Control picPilotStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picPilotStatus.Visible = False
				'UPGRADE_ISSUE: Control picFace は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picFace.Visible = False
				StatusWindowBackBolor = STATUSBACK
				StatusWindowFrameColor = STATUSBACK
				StatusWindowFrameWidth = 1
				'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picUnitStatus.BackColor = StatusWindowBackBolor
				StatusFontColorAbilityName = RGB(0, 0, 150)
				StatusFontColorAbilityEnable = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)
				StatusFontColorAbilityDisable = RGB(150, 0, 0)
				StatusFontColorNormalString = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
			Else
				'UPGRADE_ISSUE: Control picFace は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picFace.Move(MainPWidth + 24, 4)
				'UPGRADE_ISSUE: Control picPilotStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picPilotStatus.Move(MainPWidth + 24 + 68 + 4, 4, 155, 72)
				'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picUnitStatus.Move(MainPWidth + 24, 4 + 68 + 4, 225 + 5, MainPHeight - 64 + 16)
			End If
			' MOD END MARGE
			
			'マップウィンドウのサイズを設定
			' MOD START MARGE
			'        If MainWidth = 15 Then
			If Not NewGUIMode Then
				' MOD END MARGE
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Move(4, 4, MainPWidth, MainPHeight)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(1).Move(4, 4, MainPWidth, MainPHeight)
			Else
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Move(0, 0, MainPWidth, MainPHeight)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(1).Move(0, 0, MainPWidth, MainPHeight)
			End If
		End With
	End Sub
	
	' ADD START MARGE
	'Optionによる新ＧＵＩが有効かどうかを再設定する
	Public Sub SetNewGUIMode()
		' Optionで定義されているのにNewGUIModeがfalseの場合、LoadFormsを呼ぶ
		If IsOptionDefined("新ＧＵＩ") And Not NewGUIMode Then
			LoadForms()
		End If
	End Sub
	' ADD  END  MARGE
	
	' === メッセージウィンドウに関する処理 ===
	
	'メッセージウィンドウを開く
	'戦闘メッセージ画面など、ユニット表示を行う場合は u1, u2 に指定
	Public Sub OpenMessageForm(Optional ByRef u1 As Unit = Nothing, Optional ByRef u2 As Unit = Nothing)
		Dim tppx, tppy As Short
		Dim ret As Integer
		
		'UPGRADE_NOTE: オブジェクト RightUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		RightUnit = Nothing
		'UPGRADE_NOTE: オブジェクト LeftUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		LeftUnit = Nothing
		
		'UPGRADE_ISSUE: Screen オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
		tppx = VB6.TwipsPerPixelX
		tppy = VB6.TwipsPerPixelY
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmMessage)
		With frmMessage
			'ユニット表示を伴う場合はキャプションから「(自動送り)」を削除
			If Not u1 Is Nothing Then
				If .Text = "メッセージ (自動送り)" Then
					.Text = "メッセージ"
				End If
			End If
			
			'メッセージウィンドウを強制的に最小化解除
			If .WindowState <> System.Windows.Forms.FormWindowState.Normal Then
				.WindowState = System.Windows.Forms.FormWindowState.Normal
				.Show()
				.Activate()
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
				
				.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(.Width) - .ClientRectangle.Width * tppx + 508 * tppx)
				.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - .ClientRectangle.Height * tppy + 84 * tppy)
				
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
				
				UpdateMessageForm(u1)
				
				.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(.Width) - .ClientRectangle.Width * tppx + 508 * tppx)
				.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - .ClientRectangle.Height * tppy + 118 * tppy)
				
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
				
				UpdateMessageForm(u1, u2)
				
				.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(.Width) - .ClientRectangle.Width * tppx + 508 * tppx)
				.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - .ClientRectangle.Height * tppy + 118 * tppy)
				
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
						.Left = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(MainForm.Left))
					Else
						.Left = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(MainForm.Left) + (VB6.PixelsToTwipsX(MainForm.Width) - VB6.PixelsToTwipsX(.Width)) \ 2)
					End If
					If MessageWindowIsOut Then
						.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(MainForm.Top) + VB6.PixelsToTwipsY(MainForm.Height) - 350)
					Else
						.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(MainForm.Top) + VB6.PixelsToTwipsY(MainForm.Height) - VB6.PixelsToTwipsY(.Height))
					End If
				End If
			Else
				'メインウィンドウが表示されていない場合は画面中央に表示
				.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
				.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			End If
			
			'ウィンドウをクリアしておく
			.picFace.Image = System.Drawing.Image.FromFile("")
			DisplayedPilot = ""
			'UPGRADE_ISSUE: PictureBox メソッド picMessage.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.picMessage.Cls()
			
			'ウィンドウを表示
			.Show()
			
			'常に手前に表示する
			ret = SetWindowPos(.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
		End With
		
		System.Windows.Forms.Application.DoEvents()
	End Sub
	
	'メッセージウィンドウを閉じる
	Public Sub CloseMessageForm()
		If Not frmMessage.Visible Then
			Exit Sub
		End If
		frmMessage.Hide()
		System.Windows.Forms.Application.DoEvents()
	End Sub
	
	'メッセージウィンドウをクリア
	Public Sub ClearMessageForm()
		With frmMessage
			.picFace.Image = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: PictureBox メソッド picMessage.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.picMessage.Cls()
		End With
		DisplayedPilot = ""
		'UPGRADE_NOTE: オブジェクト LeftUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		LeftUnit = Nothing
		'UPGRADE_NOTE: オブジェクト RightUnit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		RightUnit = Nothing
		System.Windows.Forms.Application.DoEvents()
	End Sub
	
	'メッセージウィンドウに表示しているユニット情報を更新
	Public Sub UpdateMessageForm(ByRef u1 As Unit, Optional ByRef u2 As Object = Nothing)
		Dim lu, ru As Unit
		Dim ret As Integer
		Dim i As Short
		Dim buf As String
		Dim num As Short
		Dim tmp As Integer
		
		With frmMessage
			'ウィンドウにユニット情報が表示されていない場合はそのまま終了
			If .Visible Then
				If Not .picUnit1.Visible And Not .picUnit2.Visible Then
					Exit Sub
				End If
			End If
			
			'luを左に表示するユニット、ruを右に表示するユニットに設定
			'UPGRADE_NOTE: IsMissing() は IsNothing() に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="8AE1CB93-37AB-439A-A4FF-BE3B6760BB23"' をクリックしてください。
			If IsNothing(u2) Then
				'１体のユニットのみ表示
				If u1.Party = "味方" Or u1.Party = "ＮＰＣ" Then
					'UPGRADE_NOTE: オブジェクト lu をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					lu = Nothing
					ru = u1
				Else
					lu = u1
					'UPGRADE_NOTE: オブジェクト ru をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					ru = Nothing
				End If
			ElseIf u2 Is Nothing Then 
				'反射攻撃
				'前回表示されたユニットをそのまま使用
				lu = LeftUnit
				ru = RightUnit
			ElseIf (u2 Is LeftUnit Or u1 Is RightUnit) And Not LeftUnit Is RightUnit Then 
				lu = u2
				ru = u1
			Else
				lu = u1
				ru = u2
			End If
			
			'現在表示されている順番に応じてユニットの入れ替え
			If lu Is RightUnit And ru Is LeftUnit And Not LeftUnit Is RightUnit Then
				lu = LeftUnit
				ru = RightUnit
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
						'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: PictureBox プロパティ picUnit1.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * (lu.BitmapID Mod 15), 96 * (lu.BitmapID \ 15), SRCCOPY)
					Else
						LoadUnitBitmap(lu, .picUnit1, 0, 0, True)
					End If
				Else
					'非表示のユニットの場合はユニットのいる地形タイルを表示
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ picUnit1.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * (lu.X - 1), 32 * (lu.Y - 1), SRCCOPY)
				End If
				.picUnit1.Refresh()
				
				'ＨＰ名称
				If lu.IsConditionSatisfied("データ不明") Then
					.labHP1.Text = Term("HP")
				Else
					.labHP1.Text = Term("HP", lu)
				End If
				
				'ＨＰ数値
				If lu.IsConditionSatisfied("データ不明") Then
					.txtHP1.Text = "?????/?????"
				Else
					If lu.HP < 100000 Then
						buf = LeftPaddedString(VB6.Format(lu.HP), MinLng(Len(VB6.Format(lu.MaxHP)), 5))
					Else
						buf = "?????"
					End If
					If lu.MaxHP < 100000 Then
						buf = buf & "/" & VB6.Format(lu.MaxHP)
					Else
						buf = buf & "/?????"
					End If
					.txtHP1.Text = buf
				End If
				
				'ＨＰゲージ
				'UPGRADE_ISSUE: PictureBox メソッド picHP1.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.picHP1.Cls()
				If lu.HP > 0 Or i < num Then
					'UPGRADE_ISSUE: PictureBox メソッド picHP1.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.picHP1.Line (0, 0) - ((.picHP1.Width - 4) * lu.HP \ lu.MaxHP - 1, 4), BF
				End If
				
				'ＥＮ名称
				If lu.IsConditionSatisfied("データ不明") Then
					.labEN1.Text = Term("EN")
				Else
					.labEN1.Text = Term("EN", lu)
				End If
				
				'ＥＮ数値
				If lu.IsConditionSatisfied("データ不明") Then
					.txtEN1.Text = "???/???"
				Else
					If lu.EN < 1000 Then
						buf = LeftPaddedString(VB6.Format(lu.EN), MinLng(Len(VB6.Format(lu.MaxEN)), 3))
					Else
						buf = "???"
					End If
					If lu.MaxEN < 1000 Then
						buf = buf & "/" & VB6.Format(lu.MaxEN)
					Else
						buf = buf & "/???"
					End If
					.txtEN1.Text = buf
				End If
				
				'ＥＮゲージ
				'UPGRADE_ISSUE: PictureBox メソッド picEN1.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.picEN1.Cls()
				If lu.EN > 0 Or i < num Then
					'UPGRADE_ISSUE: PictureBox メソッド picEN1.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.picEN1.Line (0, 0) - ((.picEN1.Width - 4) * lu.EN \ lu.MaxEN - 1, 4), BF
				End If
				
				'表示内容を記録
				LeftUnit = lu
				LeftUnitHPRatio = lu.HP / lu.MaxHP
				LeftUnitENRatio = lu.EN / lu.MaxEN
			End If
			
			If Not ru Is Nothing And Not RightUnit Is ru Then
				'右のユニットが未表示なので表示する
				
				'ユニット画像
				If ru.BitmapID > 0 Then
					If MapDrawMode = "" Then
						'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: PictureBox プロパティ picUnit2.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * (ru.BitmapID Mod 15), 96 * (ru.BitmapID \ 15), SRCCOPY)
					Else
						LoadUnitBitmap(ru, .picUnit2, 0, 0, True)
					End If
				Else
					'非表示のユニットの場合はユニットのいる地形タイルを表示
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ picUnit2.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * (ru.X - 1), 32 * (ru.Y - 1), SRCCOPY)
				End If
				.picUnit2.Refresh()
				
				'ＨＰ数値
				If ru.IsConditionSatisfied("データ不明") Then
					.labHP2.Text = Term("HP")
				Else
					.labHP2.Text = Term("HP", ru)
				End If
				
				'ＨＰ数値
				If ru.IsConditionSatisfied("データ不明") Then
					.txtHP2.Text = "?????/?????"
				Else
					If ru.HP < 100000 Then
						buf = LeftPaddedString(VB6.Format(ru.HP), MinLng(Len(VB6.Format(ru.MaxHP)), 5))
					Else
						buf = "?????"
					End If
					If ru.MaxHP < 100000 Then
						buf = buf & "/" & VB6.Format(ru.MaxHP)
					Else
						buf = buf & "/?????"
					End If
					.txtHP2.Text = buf
				End If
				
				'ＨＰゲージ
				'UPGRADE_ISSUE: PictureBox メソッド picHP2.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.picHP2.Cls()
				If ru.HP > 0 Or i < num Then
					'UPGRADE_ISSUE: PictureBox メソッド picHP2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.picHP2.Line (0, 0) - ((.picHP2.Width - 4) * ru.HP \ ru.MaxHP - 1, 4), BF
				End If
				
				'ＥＮ名称
				If ru.IsConditionSatisfied("データ不明") Then
					.labEN2.Text = Term("EN")
				Else
					.labEN2.Text = Term("EN", ru)
				End If
				
				'ＥＮ数値
				If ru.IsConditionSatisfied("データ不明") Then
					.txtEN2.Text = "???/???"
				Else
					If ru.EN < 1000 Then
						buf = LeftPaddedString(VB6.Format(ru.EN), MinLng(Len(VB6.Format(ru.MaxEN)), 3))
					Else
						buf = "???"
					End If
					If ru.MaxEN < 1000 Then
						buf = buf & "/" & VB6.Format(ru.MaxEN)
					Else
						buf = buf & "/???"
					End If
					.txtEN2.Text = buf
				End If
				
				'ＥＮゲージ
				'UPGRADE_ISSUE: PictureBox メソッド picEN2.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.picEN2.Cls()
				If ru.EN > 0 Or i < num Then
					'UPGRADE_ISSUE: PictureBox メソッド picEN2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.picEN2.Line (0, 0) - ((.picEN2.Width - 4) * ru.EN \ ru.MaxEN - 1, 4), BF
				End If
				
				'表示内容を記録
				RightUnit = ru
				RightUnitHPRatio = ru.HP / ru.MaxHP
				RightUnitENRatio = ru.EN / ru.MaxEN
			End If
			
			'前回の表示からのＨＰ、ＥＮの変化をアニメ表示
			
			'変化がない場合はアニメ表示の必要がないのでチェックしておく
			num = 0
			If Not lu Is Nothing Then
				If lu.HP / lu.MaxHP <> LeftUnitHPRatio Or lu.EN / lu.MaxEN <> LeftUnitENRatio Then
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
								buf = LeftPaddedString(VB6.Format(tmp), MinLng(Len(VB6.Format(lu.MaxHP)), 5))
							Else
								buf = "?????"
							End If
							If lu.MaxHP < 100000 Then
								buf = buf & "/" & VB6.Format(lu.MaxHP)
							Else
								buf = buf & "/?????"
							End If
							.txtHP1.Text = buf
						End If
						
						'UPGRADE_ISSUE: PictureBox メソッド picHP1.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						.picHP1.Cls()
						If lu.HP > 0 Or i < num Then
							'UPGRADE_ISSUE: PictureBox メソッド picHP1.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							.picHP1.Line (0, 0) - ((.picHP1.Width - 4) * tmp \ lu.MaxHP - 1, 4), BF
						End If
					End If
					
					'ＥＮ
					If lu.EN / lu.MaxEN <> LeftUnitENRatio Then
						tmp = (lu.MaxEN * LeftUnitENRatio * (num - i) + lu.EN * i) \ num
						
						If lu.IsConditionSatisfied("データ不明") Then
							.txtEN1.Text = "???/???"
						Else
							If lu.EN < 1000 Then
								buf = LeftPaddedString(VB6.Format(tmp), MinLng(Len(VB6.Format(lu.MaxEN)), 3))
							Else
								buf = "???"
							End If
							If lu.MaxEN < 1000 Then
								buf = buf & "/" & VB6.Format(lu.MaxEN)
							Else
								buf = buf & "/???"
							End If
							.txtEN1.Text = buf
						End If
						
						'UPGRADE_ISSUE: PictureBox メソッド picEN1.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						.picEN1.Cls()
						If lu.EN > 0 Or i < num Then
							'UPGRADE_ISSUE: PictureBox メソッド picEN1.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							.picEN1.Line (-1, 0) - ((.picEN1.Width - 4) * tmp \ lu.MaxEN - 1, 4), BF
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
								buf = LeftPaddedString(VB6.Format(tmp), MinLng(Len(VB6.Format(ru.MaxHP)), 5))
							Else
								buf = "?????"
							End If
							If ru.MaxHP < 100000 Then
								buf = buf & "/" & VB6.Format(ru.MaxHP)
							Else
								buf = buf & "/?????"
							End If
							.txtHP2.Text = buf
						End If
						
						'UPGRADE_ISSUE: PictureBox メソッド picHP2.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						.picHP2.Cls()
						If ru.HP > 0 Or i < num Then
							'UPGRADE_ISSUE: PictureBox メソッド picHP2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							.picHP2.Line (0, 0) - ((.picHP2.Width - 4) * tmp \ ru.MaxHP - 1, 4), BF
						End If
					End If
					
					'ＥＮ
					If ru.EN / ru.MaxEN <> RightUnitENRatio Then
						tmp = (ru.MaxEN * RightUnitENRatio * (num - i) + ru.EN * i) \ num
						If ru.IsConditionSatisfied("データ不明") Then
							.txtEN2.Text = "???/???"
						Else
							If ru.EN < 1000 Then
								buf = LeftPaddedString(VB6.Format(tmp), MinLng(Len(VB6.Format(ru.MaxEN)), 3))
							Else
								buf = "???"
							End If
							If ru.MaxEN < 1000 Then
								buf = buf & "/" & VB6.Format(ru.MaxEN)
							Else
								buf = buf & "/???"
							End If
							.txtEN2.Text = buf
						End If
						
						'UPGRADE_ISSUE: PictureBox メソッド picEN2.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						.picEN2.Cls()
						If ru.EN > 0 Or i < num Then
							'UPGRADE_ISSUE: PictureBox メソッド picEN2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							.picEN2.Line (0, 0) - ((.picEN2.Width - 4) * tmp \ ru.MaxEN - 1, 4), BF
						End If
					End If
				End If
				
				'リフレッシュ
				If Not lu Is Nothing Then
					If lu.HP / lu.MaxHP <> LeftUnitHPRatio Then
						.picHP1.Refresh()
						.txtHP1.Refresh()
					End If
					If lu.EN / lu.MaxEN <> LeftUnitENRatio Then
						.picEN1.Refresh()
						.txtEN1.Refresh()
					End If
				End If
				If Not ru Is Nothing Then
					If ru.HP / ru.MaxHP <> RightUnitHPRatio Then
						.picHP2.Refresh()
						.txtHP2.Refresh()
					End If
					If ru.EN / ru.MaxEN <> RightUnitENRatio Then
						.picEN2.Refresh()
						.txtEN2.Refresh()
					End If
				End If
				
				Sleep(20)
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
			
			System.Windows.Forms.Application.DoEvents()
		End With
	End Sub
	
	'メッセージウィンドウの状態を記録する
	Public Sub SaveMessageFormStatus()
		IsMessageFormVisible = frmMessage.Visible
		SavedLeftUnit = LeftUnit
		SavedRightUnit = RightUnit
	End Sub
	
	'メッセージウィンドウの状態を記録した状態に保つ
	Public Sub KeepMessageFormStatus()
		If Not IsMessageFormVisible Then
			'記録した時点でメッセージウィンドウが表示されていなければ
			If frmMessage.Visible Then
				'開いているメッセージウィンドウを強制的に閉じる
				CloseMessageForm()
			End If
		ElseIf Not frmMessage.Visible Then 
			'記録した時点ではメッセージウィンドウが表示されていたので、
			'メッセージウィンドウが表示されていない場合は表示する
			OpenMessageForm(SavedLeftUnit, SavedRightUnit)
		ElseIf LeftUnit Is Nothing And RightUnit Is Nothing And (Not SavedLeftUnit Is Nothing Or Not SavedRightUnit Is Nothing) Then 
			'メッセージウィンドウからユニット表示が消えてしまった場合は再表示
			OpenMessageForm(SavedLeftUnit, SavedRightUnit)
		End If
	End Sub
	
	
	' === メッセージ表示に関する処理 ===
	
	'メッセージウィンドウにメッセージを表示
	Public Sub DisplayMessage(ByRef pname As String, ByVal msg As String, Optional ByVal msg_mode As String = "")
		Dim messages() As String
		Dim msg_head, line_head As Short
		Dim i, j As Short
		Dim buf, ch As String
		Dim p As System.Windows.Forms.PictureBox
		Dim pnickname, fname As String
		Dim start_time, wait_time As Integer
		Dim lnum, prev_lnum As Short
		Dim PT As POINTAPI
		Dim is_automode As Boolean
		Dim lstate, rstate As Short
		Dim in_tag As Boolean
		Dim is_character_message As Boolean
		Dim cl_margin(2) As Single
		Dim left_margin As String
		
		'キャラ表示の描き換え
		If pname = "システム" Then
			'「システム」
			frmMessage.picFace.Image = System.Drawing.Image.FromFile("")
			frmMessage.picFace.Refresh()
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
						frmMessage.picFace.Refresh()
						DisplayedPilot = fname
						DisplayMode = msg_mode
					Else
						frmMessage.picFace.Image = System.Drawing.Image.FromFile("")
						frmMessage.picFace.Refresh()
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
				frmMessage.picFace.Image = System.Drawing.Image.FromFile("")
				frmMessage.picFace.Refresh()
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
		FormatMessage(msg)
		msg = Trim(msg)
		
		'末尾に強制改行が入っている場合は取り除く
		Do While Right(msg, 1) = ";"
			msg = Left(msg, Len(msg) - 1)
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
				If (InStr(msg, "「") > 0 And Right(msg, 1) = "」") Then
					i = InStr(msg, "「")
				ElseIf (InStr(msg, "『") > 0 And Right(msg, 1) = "』") Then 
					i = InStr(msg, "『")
				ElseIf (InStr(msg, "(") > 0 And Right(msg, 1) = ")") Then 
					i = InStr(msg, "(")
				ElseIf (InStr(msg, "（") > 0 And Right(msg, 1) = "）") Then 
					i = InStr(msg, "（")
				End If
				If i > 1 Then
					If i < 8 Or PDList.IsDefined(Trim(Left(msg, i - 1))) Or NPDList.IsDefined(Trim(Left(msg, i - 1))) Then
						is_character_message = True
						If Not IsSpace(Mid(msg, i - 1, 1)) Then
							'"「"の前に半角スペースを挿入
							msg = Left(msg, i - 1) & " " & Mid(msg, i)
						End If
					End If
				End If
			Case Else
				is_character_message = True
				If (Left(msg, 1) = "(" Or Left(msg, 1) = "（") And (Right(msg, 1) = ")" Or Right(msg, 1) = "）") Then
					'モノローグ
					msg = Mid(msg, 2, Len(msg) - 2)
					msg = pnickname & IIf(IsOptionDefined("会話パイロット名改行"), ";", " ") & "（" & msg & "）"
				ElseIf Left(msg, 1) = "『" And Right(msg, 1) = "』" Then 
					msg = Mid(msg, 2, Len(msg) - 2)
					msg = pnickname & IIf(IsOptionDefined("会話パイロット名改行"), ";", " ") & "『" & msg & "』"
				Else
					'せりふ
					msg = pnickname & IIf(IsOptionDefined("会話パイロット名改行"), ";", " ") & "「" & msg & "」"
				End If
		End Select
		
		'強制改行の位置を設定
		If IsOptionDefined("改行時余白短縮") Then
			cl_margin(0) = 0.94 'メッセージ長の超過による改行の位置
			cl_margin(1) = 0.7 '"。"," "による改行の位置
			cl_margin(2) = 0.85 '"、"による改行の位置
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
			If Mid(msg, i, 1) = ":" Then
				buf = buf & Mid(msg, msg_head, i - msg_head)
				messages(UBound(messages)) = buf
				ReDim Preserve messages(UBound(messages) + 1)
				msg_head = i + 1
			End If
		Next 
		messages(UBound(messages)) = buf & Mid(msg, msg_head)
		
		'メッセージ長判定のため、元のメッセージを再構築
		msg = ""
		For i = 1 To UBound(messages)
			msg = msg & messages(i)
		Next 
		
		'メッセージの表示
		p = frmMessage.picMessage
		msg_head = 1
		prev_lnum = 0
		i = 0
		Dim counter As Short
		Do While i < UBound(messages)
			i = i + 1
			buf = messages(i)
			
			lnum = 0
			line_head = msg_head
			in_tag = False
			
			'UPGRADE_ISSUE: PictureBox メソッド p.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			p.Cls()
			'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			p.CurrentX = 1
			
			If msg_head = 1 Then
				'フォント設定を初期化
				With p
					.Font = VB6.FontChangeBold(.Font, False)
					.Font = VB6.FontChangeItalic(.Font, False)
					.Font = VB6.FontChangeName(.Font, "ＭＳ Ｐ明朝")
					.Font = VB6.FontChangeSize(.Font, 12)
					.ForeColor = System.Drawing.Color.Black
				End With
			Else
				'メッセージの途中から表示
				If is_character_message Then
					'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					p.Print("  ")
				End If
			End If
			
			counter = msg_head
			For j = counter To Len(buf)
				ch = Mid(buf, j, 1)
				
				'";"では必ず改行
				If ch = ";" Then
					If j <> line_head Then
						PrintMessage(Mid(buf, line_head, j - line_head))
						lnum = lnum + 1
						If is_character_message And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) Then
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print(left_margin)
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
				If MessageLen(Mid(buf, line_head, j - line_head)) > 0.95 * VB6.PixelsToTwipsX(p.Width) Then
					PrintMessage(Mid(buf, line_head, j - line_head + 1))
					lnum = lnum + 1
					If is_character_message And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) Then
						'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						p.Print(left_margin)
					End If
					line_head = j + 1
					GoTo NextLoop
				End If
				
				'禁則処理
				Select Case Mid(buf, j + 1, 1)
					Case "。", "、", "…", "‥", "・", "･", "～", "ー", "－", "！", "？", "」", "』", "）", ")", " ", ";"
						GoTo NextLoop
				End Select
				Select Case Mid(buf, j + 2, 1)
					Case "。", "、", "…", "‥", "・", "･", "～", "ー", "－", "！", "？", "」", "』", "）", ")", " ", ";"
						GoTo NextLoop
				End Select
				If Mid(buf, j + 3, 1) = ";" Then
					GoTo NextLoop
				End If
				
				'改行の判定
				If MessageLen(Mid(messages(i), line_head)) < 0.95 * VB6.PixelsToTwipsX(p.Width) Then
					'全体が一行に収まる場合
					GoTo NextLoop
				End If
				Select Case ch
					Case "。"
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(1) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head + 1))
							lnum = lnum + 1
							If is_character_message And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) Then
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print(left_margin)
							End If
							line_head = j + 1
						End If
					Case "、"
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(2) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head + 1))
							lnum = lnum + 1
							If is_character_message And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) Then
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print(left_margin)
							End If
							line_head = j + 1
						End If
					Case " "
						ch = Mid(buf, j - 1, 1)
						'スペースが文の区切りに使われているかどうか判定
						If pname <> "システム" And (ch = "！" Or ch = "？" Or ch = "…" Or ch = "‥" Or ch = "・" Or ch = "･" Or ch = "～") Then
							'文の区切り
							If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(1) * VB6.PixelsToTwipsX(p.Width) Then
								PrintMessage(Mid(buf, line_head, j - line_head + 1))
								lnum = lnum + 1
								If is_character_message And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) Then
									'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									p.Print(left_margin)
								End If
								line_head = j + 1
							End If
						Else
							'単なる空白
							If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(0) * VB6.PixelsToTwipsX(p.Width) Then
								PrintMessage(Mid(buf, line_head, j - line_head + 1))
								lnum = lnum + 1
								If is_character_message And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) Then
									'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									p.Print(left_margin)
								End If
								line_head = j + 1
							End If
						End If
					Case Else
						
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(0) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head + 1))
							lnum = lnum + 1
							If is_character_message And ((lnum > 1 And IsOptionDefined("会話パイロット名改行")) Or (lnum > 0 And Not IsOptionDefined("会話パイロット名改行"))) Then
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print(left_margin)
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
					PrintMessage(Mid(buf, line_head))
				End If
			End If
			
			System.Windows.Forms.Application.DoEvents()
			
			If MessageWait > 10000 Then
				AutoMessageMode = False
			End If
			
			'ウィンドウのキャプションを設定
			If AutoMessageMode Then
				If frmMessage.Text = "メッセージ" Then
					frmMessage.Text = "メッセージ (自動送り)"
				End If
			Else
				If frmMessage.Text = "メッセージ (自動送り)" Then
					frmMessage.Text = "メッセージ"
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
				
				GetCursorPos(PT)
				
				'メッセージウインドウ上でマウスボタンを押した場合
				If System.Windows.Forms.Form.ActiveForm Is frmMessage Then
					With frmMessage
						If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
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
				If System.Windows.Forms.Form.ActiveForm Is MainForm Then
					With MainForm
						If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
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
				
				Sleep(100)
				System.Windows.Forms.Application.DoEvents()
				
				'自動送りモードが切り替えられた場合
				If is_automode <> AutoMessageMode Then
					IsFormClicked = False
					is_automode = AutoMessageMode
					If AutoMessageMode Then
						frmMessage.Text = "メッセージ (自動送り)"
						start_time = timeGetTime()
						wait_time = (lnum - prev_lnum + 2) * (MessageWait + 250)
					Else
						frmMessage.Text = "メッセージ"
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
			.Font = VB6.FontChangeBold(.Font, False)
			.Font = VB6.FontChangeItalic(.Font, False)
			.Font = VB6.FontChangeName(.Font, "ＭＳ Ｐ明朝")
			.Font = VB6.FontChangeSize(.Font, 12)
			.ForeColor = System.Drawing.Color.Black
		End With
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("パイロット用画像ファイル" & vbCr & vbLf & DisplayedPilot & vbCr & vbLf & "の読み込み中にエラーが発生しました。" & vbCr & vbLf & "画像ファイルが壊れていないか確認して下さい。")
	End Sub
	
	'メッセージウィンドウに文字列を書き込む
	Public Sub PrintMessage(ByRef msg As String, Optional ByVal is_sys_msg As Boolean = False)
		Dim buf, tag, ch As String
		Dim p As System.Windows.Forms.PictureBox
		Dim last_y, last_x, max_y As Short
		Dim i, head As Short
		Dim in_tag As Boolean
		Dim escape_depth As Short
		
		p = frmMessage.picMessage
		
		head = 1
		Dim cname As String
		For i = 1 To Len(msg)
			ch = Mid(msg, i, 1)
			
			'システムメッセージの時のみエスケープシーケンスの処理を行う
			If is_sys_msg Then
				Select Case ch
					Case "["
						escape_depth = escape_depth + 1
						If escape_depth = 1 Then
							'エスケープシーケンス開始
							'それまでの文字列を出力
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print(Mid(msg, head, i - head))
							head = i + 1
							GoTo NextChar
						End If
					Case "]"
						escape_depth = escape_depth - 1
						If escape_depth = 0 Then
							'エスケープシーケンス終了
							'エスケープシーケンスを出力
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print(Mid(msg, head, i - head))
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
						'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						p.Print(Mid(msg, head, i - head))
						head = i + 1
						GoTo NextChar
					End If
				Case ">"
					If in_tag Then
						'タグ終了
						in_tag = False
						
						'タグの切り出し
						tag = LCase(Mid(msg, head, i - head))
						
						'タグに合わせて各種処理を行う
						Select Case tag
							Case "b"
								p.Font = VB6.FontChangeBold(p.Font, True)
							Case "/b"
								p.Font = VB6.FontChangeBold(p.Font, False)
							Case "i"
								p.Font = VB6.FontChangeItalic(p.Font, True)
							Case "/i"
								p.Font = VB6.FontChangeItalic(p.Font, False)
							Case "big"
								p.Font = VB6.FontChangeSize(p.Font, p.Font.SizeInPoints + 2)
								'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								last_x = p.CurrentX
								'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								last_y = p.CurrentY
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print()
								'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								If p.CurrentY > max_y Then
									'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									max_y = p.CurrentY
								End If
								'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.CurrentX = last_x
								'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.CurrentY = last_y
							Case "/big"
								p.Font = VB6.FontChangeSize(p.Font, p.Font.SizeInPoints - 2)
							Case "small"
								p.Font = VB6.FontChangeSize(p.Font, p.Font.SizeInPoints - 2)
								'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								last_x = p.CurrentX
								'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								last_y = p.CurrentY
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print()
								'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								If p.CurrentY > max_y Then
									'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									max_y = p.CurrentY
								End If
								'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.CurrentX = last_x
								'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.CurrentY = last_y
							Case "/small"
								p.Font = VB6.FontChangeSize(p.Font, p.Font.SizeInPoints + 2)
							Case "/color"
								p.ForeColor = System.Drawing.Color.Black
							Case "/size"
								p.Font = VB6.FontChangeSize(p.Font, 12)
							Case "lt"
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print("<")
							Case "gt"
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print(">")
							Case Else
								If InStr(tag, "color=") = 1 Then
									'色設定
									cname = GetValueAsString(Mid(tag, 7))
									Select Case cname
										Case "black"
											p.ForeColor = System.Drawing.Color.Black
										Case "gray"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&H80, &H80, &H80))
										Case "silver"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&HC0, &HC0, &HC0))
										Case "white"
											p.ForeColor = System.Drawing.Color.White
										Case "red"
											p.ForeColor = System.Drawing.Color.Red
										Case "yellow"
											p.ForeColor = System.Drawing.Color.Yellow
										Case "lime"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&H0, &HFF, &H0))
										Case "aqua"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&H0, &HFF, &HFF))
										Case "blue"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&H0, &H0, &HFF))
										Case "fuchsia"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&HFF, &H0, &HFF))
										Case "maroon"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&H80, &H0, &H0))
										Case "olive"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&H80, &H80, &H0))
										Case "green"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&H0, &H80, &H0))
										Case "teal"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&H0, &H80, &H80))
										Case "navy"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&H0, &H0, &H80))
										Case "purple"
											p.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(&H80, &H0, &H80))
										Case Else
											If Asc(cname) = 35 Then '#
												buf = New String(vbNullChar, 8)
												Mid(buf, 1, 2) = "&H"
												Mid(buf, 3, 2) = Mid(cname, 6, 2)
												Mid(buf, 5, 2) = Mid(cname, 4, 2)
												Mid(buf, 7, 2) = Mid(cname, 2, 2)
												If IsNumeric(buf) Then
													p.ForeColor = System.Drawing.ColorTranslator.FromOle(CInt(buf))
												End If
											End If
									End Select
								ElseIf InStr(tag, "size=") = 1 Then 
									'サイズ設定
									If IsNumeric(Mid(tag, 6)) Then
										p.Font = VB6.FontChangeSize(p.Font, CInt(Mid(tag, 6)))
										'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										last_x = p.CurrentX
										'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										last_y = p.CurrentY
										'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										p.Print()
										'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										If p.CurrentY > max_y Then
											'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											max_y = p.CurrentY
										End If
										'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										p.CurrentX = last_x
										'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										p.CurrentY = last_y
									End If
								Else
									'タグではないのでそのまま書き出す
									'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									p.Print(Mid(msg, head - 1, i - head + 2))
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
			If Right(msg, 1) = "」" Then
				'最後の括弧の位置は一番大きなサイズの文字に合わせる
				'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				p.Print(Mid(msg, head, Len(msg) - head))
				
				'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				last_x = p.CurrentX
				'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				last_y = p.CurrentY
				'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				p.Print()
				'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				p.CurrentX = last_x
				'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				If p.CurrentY > max_y Then
					'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					p.CurrentY = last_y
				Else
					'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					p.CurrentY = last_y + max_y - p.CurrentY
				End If
				
				'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				p.Print(Right(msg, 1))
			Else
				'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				p.Print(Mid(msg, head))
			End If
		Else
			'未出力の文字列がない場合は改行のみ
			'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			p.Print()
		End If
		
		'改行後の位置は一番大きなサイズの文字に合わせる
		'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		If max_y > p.CurrentY Then
			'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			p.CurrentY = max_y + 1
		Else
			'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			p.CurrentY = p.CurrentY + 1
		End If
		'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		p.CurrentX = 1
	End Sub
	
	'メッセージ幅を計算(タグを無視して)
	Public Function MessageLen(ByVal msg As String) As Short
		Dim buf As String
		Dim ret As Short
		
		'タグが存在する？
		ret = InStr(msg, "<")
		If ret = 0 Then
			'UPGRADE_ISSUE: PictureBox メソッド picMessage.TextWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			MessageLen = frmMessage.picMessage.TextWidth(msg)
			Exit Function
		End If
		
		'タグを除いたメッセージを作成
		Do While ret > 0
			buf = buf & Left(msg, ret - 1)
			msg = Mid(msg, ret + 1)
			
			ret = InStr(msg, ">")
			If ret > 0 Then
				msg = Mid(msg, ret + 1)
			Else
				msg = ""
			End If
			
			ret = InStr(msg, "<")
		Loop 
		buf = buf & msg
		
		'タグ抜きメッセージのピクセル幅を計算
		'UPGRADE_ISSUE: PictureBox メソッド picMessage.TextWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		MessageLen = frmMessage.picMessage.TextWidth(buf)
	End Function
	
	'メッセージウィンドウに戦闘メッセージを表示
	Public Sub DisplayBattleMessage(ByRef pname As String, ByVal msg As String, Optional ByRef msg_mode As String = "")
		Dim messages() As String
		Dim i, j As Short
		Dim lnum, line_head, prev_lnum As Short
		Dim p As System.Windows.Forms.PictureBox
		Dim buf2, buf, ch As String
		Dim pnickname As String
		Dim wait_time, start_time, cur_time As Integer
		Dim need_refresh As Boolean
		Dim in_tag As Boolean
		Dim dx, dw, dh, dy As String
		Dim options As String
		Dim n, opt_n As Short
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
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath & "Bitmap", FileAttribute.Directory)) > 0 Then
				extdata_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath2 & "Bitmap", FileAttribute.Directory)) > 0 Then
				extdata2_bitmap_dir_exists = True
			End If
			init_display_battle_message = True
		End If
		
		'メッセージウィンドウが閉じられていれば表示しない
		If frmMessage.WindowState = 1 Then
			Exit Sub
		End If
		
		'ウィンドウのキャプションを設定
		If frmMessage.Text = "メッセージ (自動送り)" Then
			frmMessage.Text = "メッセージ"
		End If
		
		'キャラ表示の描き換え
		If pname = "システム" Then
			'「システム」
			frmMessage.picFace.Image = System.Drawing.Image.FromFile("")
			frmMessage.picFace.Refresh()
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
						frmMessage.picFace.Refresh()
						DisplayedPilot = fname
						DisplayMode = msg_mode
					Else
						frmMessage.picFace.Image = System.Drawing.Image.FromFile("")
						frmMessage.picFace.Refresh()
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
				frmMessage.picFace.Image = System.Drawing.Image.FromFile("")
				frmMessage.picFace.Refresh()
				DisplayedPilot = ""
				DisplayMode = ""
			End If
		End If
		
		'メッセージが空なら表示は止める
		If msg = "" Then
			Exit Sub
		End If
		
		p = frmMessage.picMessage
		
		'メッセージウィンドウの状態を記録
		SaveMessageFormStatus()
		
		'メッセージを分割
		ReDim messages(1)
		line_head = 1
		buf = ""
		For i = 1 To Len(msg)
			Select Case Mid(msg, i, 1)
				Case ":"
					buf = buf & Mid(msg, line_head, i - line_head)
					messages(UBound(messages)) = buf & ":"
					ReDim Preserve messages(UBound(messages) + 1)
					line_head = i + 1
				Case ";"
					buf = buf & Mid(msg, line_head, i - line_head)
					messages(UBound(messages)) = buf
					buf = ""
					ReDim Preserve messages(UBound(messages) + 1)
					line_head = i + 1
			End Select
		Next 
		messages(UBound(messages)) = buf & Mid(msg, line_head)
		
		wait_time = DEFAULT_LEVEL
		
		'強制改行の位置を設定
		If IsOptionDefined("改行時余白短縮") Then
			cl_margin(0) = 0.94 'メッセージ長の超過による改行の位置
			cl_margin(1) = 0.7 '"。"," "による改行の位置
			cl_margin(2) = 0.85 '"、"による改行の位置
		Else
			cl_margin(0) = 0.8
			cl_margin(1) = 0.6
			cl_margin(2) = 0.75
		End If
		
		'各メッセージを表示
		Dim fsuffix, fname0, fpath As String
		Dim first_id, last_id As Short
		Dim wait_time2 As Integer
		Dim with_footer As Boolean
		For i = 1 To UBound(messages)
			buf = messages(i)
			
			'メッセージ内の式置換を処理
			SaveBasePoint()
			FormatMessage(buf)
			RestoreBasePoint()
			
			'特殊効果
			Select Case LCase(Right(LIndex(buf, 1), 4))
				Case ".bmp", ".jpg", ".gif", ".png"
					
					'右ボタンを押されていたらスキップ
					If IsRButtonPressed() Then
						GoTo NextMessage
					End If
					
					'カットインの表示
					fname = LIndex(buf, 1)
					
					'アニメ指定かどうか判定
					j = InStr(fname, "[")
					If j > 0 And InStr(fname, "].") = Len(fname) - 4 Then
						fname0 = Left(fname, j - 1)
						fsuffix = Right(fname, 4)
						buf2 = Mid(fname, j + 1, Len(fname) - j - 5)
						j = InStr(buf2, "-")
						first_id = CShort(Left(buf2, j - 1))
						last_id = CShort(Mid(buf2, j + 1))
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
							Case "透過", "背景", "白黒", "セピア", "明", "暗", "上下反転", "左右反転", "上半分", "下半分", "右半分", "左半分", "右上", "左上", "右下", "左下", "ネガポジ反転", "シルエット", "夕焼け", "水中", "保持"
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
									cname = New String(vbNullChar, 8)
									Mid(cname, 1, 2) = "&H"
									Mid(cname, 3, 2) = Mid(buf2, 6, 2)
									Mid(cname, 5, 2) = Mid(buf2, 4, 2)
									Mid(cname, 7, 2) = Mid(buf2, 2, 2)
									If IsNumeric(cname) Then
										If CInt(cname) <> System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
											options = options & VB6.Format(CInt(cname)) & " "
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
							fname = Mid(fname, 2)
						Else
							fname0 = Mid(fname0, 2)
							fname = fname0 & VB6.Format(first_id, "00") & fsuffix
						End If
						
						'ウィンドウが表示されていなければ表示
						If Not frmMessage.Visible Then
							OpenMessageForm()
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
							DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, options)
							frmMessage.picFace.Refresh()
							
							If wait_time > 0 Then
								Do While (start_time + wait_time > timeGetTime())
									Sleep(20)
								Loop 
							End If
						Else
							'アニメーションの場合
							For j = first_id To last_id
								fname = fpath & fname0 & VB6.Format(j, "00") & fsuffix
								
								DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, options)
								
								frmMessage.picFace.Refresh()
								
								If wait_time > 0 Then
									wait_time2 = wait_time * (j - first_id + 1) \ (last_id - first_id)
									cur_time = timeGetTime()
									If cur_time < start_time + wait_time2 Then
										Sleep(start_time + wait_time2 - cur_time)
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
							dw = CStr(DEFAULT_LEVEL)
						Else
							dw = CStr(StrToLng(buf2))
						End If
						buf2 = LIndex(buf, 3)
						If buf2 = "-" Then
							dh = CStr(DEFAULT_LEVEL)
						Else
							dh = CStr(StrToLng(buf2))
						End If
					Else
						dw = CStr(DEFAULT_LEVEL)
						dh = CStr(DEFAULT_LEVEL)
					End If
					
					'表示画像の位置
					If opt_n > 4 Then
						buf2 = LIndex(buf, 4)
						If buf2 = "-" Then
							dx = CStr(DEFAULT_LEVEL)
						Else
							dx = CStr(StrToLng(buf2))
						End If
						buf2 = LIndex(buf, 5)
						If buf2 = "-" Then
							dy = CStr(DEFAULT_LEVEL)
						Else
							dy = CStr(StrToLng(buf2))
						End If
					Else
						dx = CStr(DEFAULT_LEVEL)
						dy = CStr(DEFAULT_LEVEL)
					End If
					
					If wait_time > 0 Then
						start_time = timeGetTime()
					End If
					
					If first_id = -1 Then
						'１枚絵の場合
						If clear_every_time Then
							ClearPicture()
						End If
						
						DrawPicture(fname, CInt(dx), CInt(dy), CInt(dw), CInt(dh), 0, 0, 0, 0, options)
						
						need_refresh = True
						
						If wait_time > 0 Then
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.picMain(0).Refresh()
							need_refresh = False
							cur_time = timeGetTime()
							If cur_time < start_time + wait_time Then
								Sleep(start_time + wait_time - cur_time)
							End If
							wait_time = DEFAULT_LEVEL
						End If
					Else
						'アニメーションの場合
						For j = first_id To last_id
							fname = fname0 & VB6.Format(j, "00") & fsuffix
							
							If clear_every_time Then
								ClearPicture()
							End If
							
							DrawPicture(fname, CInt(dx), CInt(dy), CInt(dw), CInt(dh), 0, 0, 0, 0, options)
							
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.picMain(0).Refresh()
							
							If wait_time > 0 Then
								wait_time2 = wait_time * (j - first_id + 1) \ (last_id - first_id)
								cur_time = timeGetTime()
								If cur_time < start_time + wait_time2 Then
									Sleep(start_time + wait_time2 - cur_time)
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
					PlayWave(buf)
					If wait_time > 0 Then
						If need_refresh Then
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.picMain(0).Refresh()
							need_refresh = False
						End If
						Sleep(wait_time)
						wait_time = DEFAULT_LEVEL
					End If
					GoTo NextMessage
			End Select
			
			'戦闘アニメ呼び出し
			If Left(buf, 1) = "@" Then
				ShowAnimation(Mid(buf, 2))
				GoTo NextMessage
			End If
			
			'特殊コマンド
			Select Case LCase(LIndex(buf, 1))
				Case "clear"
					'カットインの消去
					ClearPicture()
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
			KeepMessageFormStatus()
			
			With p
				'ウィンドウをクリア
				'UPGRADE_ISSUE: PictureBox メソッド p.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.Cls()
				'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.CurrentX = 1
				
				'フォント設定を初期化
				.Font = VB6.FontChangeBold(.Font, False)
				.Font = VB6.FontChangeItalic(.Font, False)
				.Font = VB6.FontChangeName(.Font, "ＭＳ Ｐ明朝")
				.Font = VB6.FontChangeSize(.Font, 12)
				.ForeColor = System.Drawing.Color.Black
			End With
			
			'話者名と括弧の表示処理
			is_char_message = False
			If pname <> "システム" And ((pname <> "" And pname <> "-") Or ((Left(buf, 1) = "「" And Right(buf, 1) = "」")) Or ((Left(buf, 1) = "『" And Right(buf, 1) = "』"))) Then
				
				is_char_message = True
				
				'話者のグラフィックを表示
				If pname = "-" And Not SelectedUnit Is Nothing Then
					If SelectedUnit.CountPilot > 0 Then
						fname = SelectedUnit.MainPilot.Bitmap
						If DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "メッセージ " & msg_mode) Then
							frmMessage.picFace.Refresh()
							DisplayedPilot = fname
							DisplayMode = msg_mode
						End If
					End If
				End If
				
				'話者名を表示
				If pnickname = "" And pname = "-" And Not SelectedUnit Is Nothing Then
					If SelectedUnit.CountPilot > 0 Then
						'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						p.Print(SelectedUnit.MainPilot.Nickname)
					End If
				ElseIf pnickname <> "" Then 
					'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					p.Print(pnickname)
				End If
				
				'メッセージが途中で終わっているか判定
				If Right(buf, 1) <> ":" Then
					with_footer = True
				Else
					with_footer = False
					prev_lnum = lnum
					buf = Left(buf, Len(buf) - 1)
				End If
				
				'括弧を付加
				If (Left(buf, 1) = "(" Or Left(buf, 1) = "（") And (Not with_footer Or (Right(buf, 1) = ")" Or Right(buf, 1) = "）")) Then
					'モノローグ
					If with_footer Then
						buf = Mid(buf, 2, Len(buf) - 2)
						buf = "（" & buf & "）"
					Else
						buf = Mid(buf, 2)
						buf = "（" & buf
					End If
				ElseIf Left(buf, 1) = "「" And (Not with_footer Or Right(buf, 1) = "」") Then 
					'「」の括弧が既にあるので変更しない
				ElseIf Left(buf, 1) = "『" And (Not with_footer Or Right(buf, 1) = "』") Then 
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
				If Right(buf, 1) = ":" Then
					prev_lnum = lnum
					buf = Left(buf, Len(buf) - 1)
				End If
			End If
			prev_lnum = MaxLng(prev_lnum, 1)
			
			lnum = 0
			line_head = 1
			For j = 1 To Len(buf)
				ch = Mid(buf, j, 1)
				
				'「.」の場合は必ず改行
				If ch = "." Then
					If j <> line_head Then
						PrintMessage(Mid(buf, line_head, j - line_head), Not is_char_message)
						If is_char_message Then
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print("  ")
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
				If MessageLen(Mid(buf, line_head, j - line_head)) > 0.95 * VB6.PixelsToTwipsX(p.Width) Then
					PrintMessage(Mid(buf, line_head, j - line_head + 1), Not is_char_message)
					If is_char_message Then
						'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						p.Print("  ")
					End If
					line_head = j + 1
					lnum = lnum + 1
					GoTo NextLoop
				End If
				
				'禁則処理
				Select Case Mid(buf, j + 1, 1)
					Case "。", "、", "…", "‥", "・", "･", "～", "ー", "－", "！", "？", "」", "』", "）", ")", " ", "."
						GoTo NextLoop
				End Select
				Select Case Mid(buf, j + 2, 1)
					Case "。", "、", "…", "‥", "・", "･", "～", "ー", "－", "！", "？", "」", "』", "）", ")", " ", "."
						GoTo NextLoop
				End Select
				If Mid(buf, j + 3, 1) = "." Then
					GoTo NextLoop
				End If
				
				'改行の判定
				If MessageLen(Mid(messages(i), line_head)) < 0.95 * VB6.PixelsToTwipsX(p.Width) Then
					'全体が一行に収まる場合
					GoTo NextLoop
				End If
				Select Case ch
					Case "。"
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(1) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head + 1), Not is_char_message)
							If is_char_message Then
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print("  ")
							End If
							line_head = j + 1
							lnum = lnum + 1
						End If
					Case " "
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(1) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head), Not is_char_message)
							If is_char_message Then
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print("  ")
							End If
							line_head = j + 1
							lnum = lnum + 1
						End If
					Case "、"
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(2) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head + 1), Not is_char_message)
							If is_char_message Then
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print("  ")
							End If
							line_head = j + 1
							lnum = lnum + 1
						End If
					Case Else
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(0) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head), Not is_char_message)
							If is_char_message Then
								'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								p.Print("  ")
							End If
							line_head = j
							lnum = lnum + 1
						End If
				End Select
NextLoop: 
			Next 
			'メッセージの残りを表示しておく
			If Len(buf) >= line_head Then
				PrintMessage(Mid(buf, line_head), Not is_char_message)
				lnum = lnum + 1
			End If
			
			'フォント設定を元に戻す
			With p
				.Font = VB6.FontChangeBold(.Font, False)
				.Font = VB6.FontChangeItalic(.Font, False)
				.Font = VB6.FontChangeName(.Font, "ＭＳ Ｐ明朝")
				.Font = VB6.FontChangeSize(.Font, 12)
				.ForeColor = System.Drawing.Color.Black
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
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).Refresh()
				need_refresh = False
			End If
			System.Windows.Forms.Application.DoEvents()
			
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
				
				Sleep(20)
				System.Windows.Forms.Application.DoEvents()
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
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Refresh()
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
				
				Sleep(20)
				System.Windows.Forms.Application.DoEvents()
			Loop 
		End If
		
		'メッセージウィンドウの状態が変化している場合は復元
		KeepMessageFormStatus()
		
		System.Windows.Forms.Application.DoEvents()
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("画像ファイル" & vbCr & vbLf & fname & vbCr & vbLf & "の読み込み中にエラーが発生しました。" & vbCr & vbLf & "画像ファイルが壊れていないか確認して下さい。")
	End Sub
	
	'システムによるメッセージを表示
	Public Sub DisplaySysMessage(ByVal msg As String, Optional ByVal short_wait As Boolean = False)
		Dim j, i, line_head As Short
		Dim ch, buf As String
		Dim p As System.Windows.Forms.PictureBox
		Dim lnum As Short
		Dim start_time, wait_time As Integer
		Dim in_tag As Boolean
		
		'メッセージウィンドウが表示されていない場合は表示をキャンセル
		If frmMessage.WindowState = 1 Then
			Exit Sub
		End If
		
		'メッセージ内の式を置換
		FormatMessage(msg)
		
		'ウィンドウのキャプションを設定
		If frmMessage.Text = "メッセージ (自動送り)" Then
			frmMessage.Text = "メッセージ"
		End If
		
		p = frmMessage.picMessage
		
		With p
			'メッセージウィンドウをクリア
			'UPGRADE_ISSUE: PictureBox メソッド p.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.Cls()
			'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.CurrentX = 1
			
			'フォント設定を初期化
			.Font = VB6.FontChangeBold(.Font, False)
			.Font = VB6.FontChangeItalic(.Font, False)
			.Font = VB6.FontChangeName(.Font, "ＭＳ Ｐ明朝")
			.Font = VB6.FontChangeSize(.Font, 12)
			.ForeColor = System.Drawing.Color.Black
		End With
		
		'メッセージを表示
		lnum = 0
		line_head = 1
		For i = 1 To Len(msg)
			ch = Mid(msg, i, 1)
			
			'「;」の場合は必ず改行
			If ch = ";" Then
				If line_head <> i Then
					buf = Mid(msg, line_head, i - line_head)
					PrintMessage(buf, True)
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
				If Mid(msg, i + 1, 1) = "。" Or Mid(msg, i + 1, 1) = "、" Then
					GoTo NextLoop
				End If
			End If
			
			If MessageLen(Mid(msg, line_head)) < VB6.PixelsToTwipsX(p.Width) Then
				'全体が一行に収まる場合
				GoTo NextLoop
			End If
			
			If IsSpace(ch) And MessageLen(Mid(msg, line_head, i - line_head)) > 0.5 * VB6.PixelsToTwipsX(p.Width) Then
				buf = Mid(msg, line_head, i - line_head)
				PrintMessage(buf, True)
				lnum = lnum + 1
				line_head = i + 1
			ElseIf MessageLen(Mid(msg, line_head, i - line_head + 1)) > 0.95 * VB6.PixelsToTwipsX(p.Width) Then 
				buf = Mid(msg, line_head, i - line_head + 1)
				PrintMessage(buf, True)
				lnum = lnum + 1
				line_head = i + 1
			ElseIf ch = "[" Then 
				'[]で囲まれた文字列内では改行しない
				For j = i To Len(msg)
					If Mid(msg, j, 1) = "]" Then
						Exit For
					End If
				Next 
				If MessageLen(Mid(msg, line_head, j - line_head)) > 0.95 * VB6.PixelsToTwipsX(p.Width) Then
					buf = Mid(msg, line_head, i - line_head)
					PrintMessage(buf, True)
					lnum = lnum + 1
					line_head = i
				End If
			End If
NextLoop: 
		Next 
		buf = Mid(msg, line_head)
		PrintMessage(buf, True)
		lnum = lnum + 1
		
		'フォント設定を元に戻す
		With p
			.Font = VB6.FontChangeBold(.Font, False)
			.Font = VB6.FontChangeItalic(.Font, False)
			.Font = VB6.FontChangeName(.Font, "ＭＳ Ｐ明朝")
			.Font = VB6.FontChangeSize(.Font, 12)
			.ForeColor = System.Drawing.Color.Black
		End With
		
		'ウェイトを計算
		wait_time = (0.8 + 0.5 * lnum) * MessageWait
		If short_wait Then
			wait_time = wait_time \ 2
		End If
		
		System.Windows.Forms.Application.DoEvents()
		
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
			
			Sleep(20)
			System.Windows.Forms.Application.DoEvents()
		Loop 
	End Sub
	
	
	' === マップウィンドウに関する処理 ===
	
	'マップ画面背景の設定
	Public Sub SetupBackground(Optional ByRef draw_mode As String = "", Optional ByRef draw_option As String = "", Optional ByRef filter_color As Integer = 0, Optional ByRef filter_trans_par As Double = 0)
		Dim B As Object
		Dim k, i, j, ret As Short
		Dim xx, yy As Short
		Dim terrain_bmp_count As Short
		Dim terrain_bmp_type() As String
		Dim terrain_bmp_num() As Short
		Dim terrain_bmp_x() As Short
		Dim terrain_bmp_y() As Short
		Dim fname As String
		
		IsMapDirty = False
		IsPictureVisible = False
		IsCursorVisible = False
		
		'ユニット画像色を変更しないといけない場合
		If MapDrawMode <> draw_mode Then
			UList.ClearUnitBitmap()
			MapDrawMode = draw_mode
			MapDrawFilterColor = filter_color
			MapDrawFilterTransPercent = filter_trans_par
		ElseIf draw_mode = "フィルタ" And (MapDrawFilterColor <> filter_color Or MapDrawFilterTransPercent <> filter_trans_par) Then 
			UList.ClearUnitBitmap()
			MapDrawMode = draw_mode
			MapDrawFilterColor = filter_color
			MapDrawFilterTransPercent = filter_trans_par
		End If
		
		'マップ背景の設定
		With MainForm
			Select Case draw_option
				Case "ステータス"
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					With .picBack
						'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
					''                        And terrain_bmp_num(k) = MapData(i, j, 1) _
					''                    Then
					'                        Exit For
					'                    End If
					'                Next
					
					'                If k <= terrain_bmp_count Then
					'                    '既に描画済みの画像は描画した個所から転送
					'                    ret = BitBlt(.picBack.hDC, _
					''                        xx, yy, 32, 32, _
					''                        .picBack.hDC, terrain_bmp_x(k), terrain_bmp_y(k), SRCCOPY)
					'                    MapImageFileTypeData(i, j) = _
					''                        MapImageFileTypeData(terrain_bmp_x(k) \ 32 + 1, terrain_bmp_y(k) \ 32 + 1)
					'                Else
					'                    '新規の画像の場合
					'DEL  END  240a
					'画像ファイルを探す
					'MOD START 240a
					'                fname = SearchTerrainImageFile(MapData(i, j, 0), MapData(i, j, 1), i, j)
					fname = SearchTerrainImageFile(MapData(i, j, Map.MapDataIndex.TerrainType), MapData(i, j, Map.MapDataIndex.BitmapNo), i, j)
					'MOD  END  240a
					
					'画像を取り込み
					If fname <> "" Then
						On Error GoTo ErrorHandler
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.picTmp32(0) = System.Drawing.Image.FromFile(fname)
						On Error GoTo 0
					Else
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = PatBlt(.picTmp32(0).hDC, 0, 0, 32, 32, BLACKNESS)
					End If
					
					'マップ設定によって表示色を変更
					Select Case draw_mode
						Case "夜"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							Dark()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
						Case "セピア"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							Sepia()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
						Case "白黒"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							Monotone()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
						Case "夕焼け"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							Sunset()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
						Case "水中"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							Water()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
						Case "フィルタ"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent)
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
					End Select
					
					'画像を描き込み
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picBack.hDC, xx, yy, 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
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
					If Map.BoxTypes.Upper = MapData(i, j, Map.MapDataIndex.BoxType) Or Map.BoxTypes.UpperBmpOnly = MapData(i, j, Map.MapDataIndex.BoxType) Then
						'画像ファイルを探す
						fname = SearchTerrainImageFile(MapData(i, j, Map.MapDataIndex.LayerType), MapData(i, j, Map.MapDataIndex.LayerBitmapNo), i, j)
						
						'画像を取り込み
						If fname <> "" Then
							On Error GoTo ErrorHandler
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.picTmp32(0) = System.Drawing.Image.FromFile(fname)
							On Error GoTo 0
							BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
							'マップ設定によって表示色を変更
							Select Case draw_mode
								Case "夜"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									Dark(True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
								Case "セピア"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									Sepia(True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
								Case "白黒"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									Monotone(True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
								Case "夕焼け"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									Sunset(True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
								Case "水中"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									Water(True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
								Case "フィルタ"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent, True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
							End Select
							
							'画像を透過描き込み
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = TransparentBlt(.picBack.hDC, xx, yy, 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, BGColor)
						End If
						
					End If
					'ADD  END  240a
				Next 
			Next 
			'MapDrawn:  '使用されていないラベルなので削除
			
			'マス目の表示
			If ShowSquareLine Then
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picBack.Line((0, 0) - (MapPWidth - 1, MapPHeight - 1), RGB(100, 100, 100), B)
				For i = 1 To MapWidth - 1
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.picBack.Line((32 * i, -1) - (32 * i, MapPHeight), RGB(100, 100, 100))
				Next 
				For i = 1 To MapHeight - 1
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.picBack.Line((0, 32 * i - 1) - (MapPWidth, 32 * i - 1), RGB(100, 100, 100))
				Next 
			End If
			
			'マスク入り背景画面を作成
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picMaskedBack.hDC, 0, 0, MapPWidth, MapPHeight, .picBack.hDC, 0, 0, SRCCOPY)
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					xx = 32 * (i - 1)
					yy = 32 * (j - 1)
					'UPGRADE_ISSUE: Control picMask は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picMaskedBack.hDC, xx, yy, 32, 32, .picMask.hDC, 0, 0, SRCAND)
					'UPGRADE_ISSUE: Control picMask2 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picMaskedBack.hDC, xx, yy, 32, 32, .picMask2.hDC, 0, 0, SRCINVERT)
				Next 
			Next 
		End With
		
		'画面を更新
		If MapFileName <> "" And draw_option = "" Then
			RefreshScreen()
		End If
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("マップ用ビットマップファイル" & vbCr & vbLf & fname & vbCr & vbLf & "の読み込み中にエラーが発生しました。" & vbCr & vbLf & "画像ファイルが壊れていないか確認して下さい。")
		TerminateSRC()
	End Sub
	
	'画面の書き換え (ユニット表示からやり直し)
	Public Sub RedrawScreen(Optional ByVal late_refresh As Boolean = False)
		Dim PT As POINTAPI
		Dim ret As Integer
		
		ScreenIsMasked = False
		
		'画面を更新
		RefreshScreen(False, late_refresh)
		
		'カーソルを再描画
		GetCursorPos(PT)
		ret = SetCursorPos(PT.X, PT.Y)
	End Sub
	
	'画面をマスクがけして再表示
	Public Sub MaskScreen()
		ScreenIsMasked = True
		
		'画面を更新
		RefreshScreen()
	End Sub
	
	' ADD START MARGE
	'画面の書き換え
	Public Sub RefreshScreen(Optional ByVal without_refresh As Boolean = False, Optional ByVal delay_refresh As Boolean = False)
		
		If NewGUIMode Then
			RefreshScreenNew(without_refresh, delay_refresh)
		Else
			RefreshScreenOld(without_refresh, delay_refresh)
		End If
	End Sub
	' ADD END MARGE
	
	
	'画面の書き換え (旧GUI)
	' MOD START MARGE
	'Public Sub RefreshScreen(Optional ByVal without_refresh As Boolean, _
	''    Optional ByVal delay_refresh As Boolean)
	Private Sub RefreshScreenOld(Optional ByVal without_refresh As Boolean = False, Optional ByVal delay_refresh As Boolean = False)
		' MOD END MARGE
		Dim pic As System.Windows.Forms.PictureBox
		'UPGRADE_NOTE: my は my_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim mx, my_Renamed As Short
		Dim sx, sy As Short
		Dim dx, dy As Short
		Dim dw, dh As Short
		Dim xx, yy As Short
		Dim ret As Integer
		Dim i, j As Short
		Dim u As Unit
		Dim PT As POINTAPI
		Dim prev_color As Integer
		
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
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = .picMain(0)
			
			If Not without_refresh Then
				IsPictureVisible = False
				IsCursorVisible = False
				
				PaintedAreaX1 = MainPWidth
				PaintedAreaY1 = MainPHeight
				PaintedAreaX2 = -1
				PaintedAreaY2 = -1
				
				'マップウィンドウのスクロールバーの位置を変更
				If Not IsGUILocked Then
					'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					If .HScroll.Value <> MapX Then
						'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.HScroll.Value = MapX
						Exit Sub
					End If
					'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					If .VScroll.Value <> MapY Then
						'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.VScroll.Value = MapY
						Exit Sub
					End If
				End If
				
				'一旦マップウィンドウの内容を消去
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = PatBlt(pic.hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
			End If
			
			mx = MapX - (MainWidth + 1) \ 2 + 1
			my_Renamed = MapY - (MainHeight + 1) \ 2 + 1
			
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
			
			If my_Renamed < 1 Then
				sy = 1
				dy = 2 - my_Renamed
				dh = MainHeight - (1 - my_Renamed)
			ElseIf my_Renamed + MainHeight - 1 > MapHeight Then 
				sy = my_Renamed
				dy = 1
				dh = MainHeight - (my_Renamed + MainHeight - 1 - MapHeight)
			Else
				sy = my_Renamed
				dy = 1
				dh = MainHeight
			End If
			If dh > MainHeight Then
				dh = MainHeight
			End If
			
			'直線を描画する際の描画色を黒に変更
			prev_color = System.Drawing.ColorTranslator.ToOle(pic.ForeColor)
			pic.ForeColor = System.Drawing.Color.Black
			
			'表示内容を更新
			If Not ScreenIsMasked Then
				'通常表示
				For i = 0 To dw - 1
					xx = 32 * (dx + i - 1)
					For j = 0 To dh - 1
						If sx + i < 1 Or sx + i > MapWidth Or sy + j < 1 Or sy + j > MapHeight Then
							GoTo NextLoop
						End If
						
						yy = 32 * (dy + j - 1)
						
						u = MapDataForUnit(sx + i, sy + j)
						If u Is Nothing Then
							'地形
							'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
						ElseIf u.BitmapID = -1 Then 
							'非表示のユニット
							'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
						Else
							If u.Action > 0 Or u.IsFeatureAvailable("地形ユニット") Then
								'ユニット
								'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
							Else
								'行動済のユニット
								'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, SRCCOPY)
							End If
							
							'ユニットのいる場所に合わせて表示を変更
							Select Case u.Area
								Case "空中"
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, xx + 31, yy + 28)
								Case "水中"
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, xx + 31, yy + 3)
								Case "地中"
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, xx + 31, yy + 28)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, xx + 31, yy + 3)
								Case "宇宙"
									If TerrainClass(sx + i, sy + j) = "月面" Then
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
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
						If sx + i < 1 Or sx + i > MapWidth Or sy + j < 1 Or sy + j > MapHeight Then
							GoTo NextLoop2
						End If
						
						yy = 32 * (dy + j - 1)
						
						u = MapDataForUnit(sx + i, sy + j)
						If u Is Nothing Then
							If MaskData(sx + i, sy + j) Then
								'マスクされた地形
								'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
							Else
								'地形
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
							End If
						ElseIf u.BitmapID = -1 Then 
							'非表示のユニット
							If MaskData(sx + i, sy + j) Then
								'マスクされた地形
								'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
							Else
								'地形
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
							End If
						Else
							If MaskData(sx + i, sy + j) Then
								'マスクされたユニット
								'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 64, SRCCOPY)
								
								'ユニットのいる場所に合わせて表示を変更
								Select Case u.Area
									Case "空中"
										DottedLine(xx, yy + 28)
									Case "水中"
										DottedLine(xx, yy + 3)
									Case "地中"
										DottedLine(xx, yy + 28)
										DottedLine(xx, yy + 3)
									Case "宇宙"
										If TerrainClass(sx + i, sy + j) = "月面" Then
											DottedLine(xx, yy + 28)
										End If
								End Select
							Else
								'ユニット
								'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
								
								'ユニットのいる場所に合わせて表示を変更
								Select Case u.Area
									Case "空中"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 28)
									Case "水中"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 3)
									Case "地中"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 28)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 3)
									Case "宇宙"
										If TerrainClass(sx + i, sy + j) = "月面" Then
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
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
			pic.ForeColor = System.Drawing.ColorTranslator.FromOle(prev_color)
			
			'画面が書き換えられたことを記録
			ScreenIsSaved = False
			
			If Not without_refresh And Not delay_refresh Then
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Refresh()
			End If
		End With
	End Sub
	
	' ADD START MARGE
	'画面の書き換え (新GUI)
	Private Sub RefreshScreenNew(Optional ByVal without_refresh As Boolean = False, Optional ByVal delay_refresh As Boolean = False)
		Dim pic As System.Windows.Forms.PictureBox
		'UPGRADE_NOTE: my は my_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim mx, my_Renamed As Short
		Dim sx, sy As Short
		Dim dx, dy As Short
		Dim dw, dh As Short
		Dim xx, yy As Short
		Dim ret As Integer
		Dim i, j As Short
		Dim u As Unit
		Dim PT As POINTAPI
		Dim prev_color As Integer
		
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
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = .picMain(0)
			
			If Not without_refresh Then
				IsPictureVisible = False
				IsCursorVisible = False
				
				PaintedAreaX1 = MainPWidth
				PaintedAreaY1 = MainPHeight
				PaintedAreaX2 = -1
				PaintedAreaY2 = -1
				
				'マップウィンドウのスクロールバーの位置を変更
				If Not IsGUILocked Then
					'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					If .HScroll.Value <> MapX Then
						'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.HScroll.Value = MapX
						Exit Sub
					End If
					'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					If .VScroll.Value <> MapY Then
						'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.VScroll.Value = MapY
						Exit Sub
					End If
				End If
				
				'一旦マップウィンドウの内容を消去
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = PatBlt(pic.hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
			End If
			
			mx = MapX - (MainWidth + 1) \ 2 + 1
			my_Renamed = MapY - (MainHeight + 1) \ 2 + 1
			
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
			
			If my_Renamed < 1 Then
				sy = 1
				dy = 2 - my_Renamed
				dh = MainHeight - (1 - my_Renamed)
			ElseIf my_Renamed + MainHeight - 1 > MapHeight Then 
				sy = my_Renamed
				dy = 1
				dh = MainHeight - (my_Renamed + MainHeight - 1 - MapHeight)
			Else
				sy = my_Renamed
				dy = 1
				dh = MainHeight
			End If
			If dh > MainHeight Then
				dh = MainHeight
			End If
			
			'直線を描画する際の描画色を黒に変更
			prev_color = System.Drawing.ColorTranslator.ToOle(pic.ForeColor)
			pic.ForeColor = System.Drawing.Color.Black
			
			'表示内容を更新
			If Not ScreenIsMasked Then
				'通常表示
				For i = -1 To dw - 1
					xx = 32 * (dx + i - 0.5)
					For j = 0 To dh - 1
						yy = 32 * (dy + j - 1)
						
						If sx + i < 1 Or sx + i > MapWidth Or sy + j < 1 Or sy + j > MapHeight Then
							GoTo NextLoop
						End If
						
						u = MapDataForUnit(sx + i, sy + j)
						
						If i = -1 Then
							'画面左端は16ピクセル幅分だけ表示
							If u Is Nothing Then
								'地形
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
							ElseIf u.BitmapID = -1 Then 
								'非表示のユニット
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
							Else
								If u.Action > 0 Or u.IsFeatureAvailable("地形ユニット") Then
									'ユニット
									'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15), SRCCOPY)
								Else
									'行動済のユニット
									'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15) + 32, SRCCOPY)
								End If
								
								'ユニットのいる場所に合わせて表示を変更
								Select Case u.Area
									Case "空中"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, 0 + 15, yy + 28)
									Case "水中"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, 0 + 15, yy + 3)
									Case "地中"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, 0 + 15, yy + 28)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, 0 + 15, yy + 3)
									Case "宇宙"
										If TerrainClass(sx + i, sy + j) = "月面" Then
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = LineTo(pic.hDC, 0 + 15, yy + 28)
										End If
								End Select
							End If
						Else
							'画面左端以外は全32ピクセル幅分だけ表示
							If u Is Nothing Then
								'地形
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
							ElseIf u.BitmapID = -1 Then 
								'非表示のユニット
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
							Else
								If u.Action > 0 Or u.IsFeatureAvailable("地形ユニット") Then
									'ユニット
									'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
								Else
									'行動済のユニット
									'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, SRCCOPY)
								End If
								
								'ユニットのいる場所に合わせて表示を変更
								Select Case u.Area
									Case "空中"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 28)
									Case "水中"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 3)
									Case "地中"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 28)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 3)
									Case "宇宙"
										If TerrainClass(sx + i, sy + j) = "月面" Then
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
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
						
						If sx + i < 1 Or sx + i > MapWidth Or sy + j < 1 Or sy + j > MapHeight Then
							GoTo NextLoop2
						End If
						
						u = MapDataForUnit(sx + i, sy + j)
						
						If i = -1 Then
							'画面左端は16ピクセル幅分だけ表示
							If u Is Nothing Then
								If MaskData(sx + i, sy + j) Then
									'マスクされた地形
									'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picMaskedBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
								Else
									'地形
									'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
								End If
							ElseIf u.BitmapID = -1 Then 
								'非表示のユニット
								If MaskData(sx + i, sy + j) Then
									'マスクされた地形
									'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picMaskedBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
								Else
									'地形
									'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
								End If
							Else
								If MaskData(sx + i, sy + j) Then
									'マスクされたユニット
									'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15) + 64, SRCCOPY)
									
									'ユニットのいる場所に合わせて表示を変更
									Select Case u.Area
										Case "空中"
											DottedLine(0, yy + 28, True)
										Case "水中"
											DottedLine(0, yy + 3, True)
										Case "地中"
											DottedLine(0, yy + 28, True)
											DottedLine(0, yy + 3, True)
										Case "宇宙"
											If TerrainClass(sx + i, sy + j) = "月面" Then
												DottedLine(0, yy + 28, True)
											End If
									End Select
								Else
									'ユニット
									'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15), SRCCOPY)
									
									'ユニットのいる場所に合わせて表示を変更
									Select Case u.Area
										Case "空中"
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = LineTo(pic.hDC, 0 + 15, yy + 28)
										Case "水中"
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = LineTo(pic.hDC, 0 + 15, yy + 3)
										Case "地中"
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = LineTo(pic.hDC, 0 + 15, yy + 28)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = LineTo(pic.hDC, 0 + 15, yy + 3)
										Case "宇宙"
											If TerrainClass(sx + i, sy + j) = "月面" Then
												'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
												ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
												'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
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
									'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
								Else
									'地形
									'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
								End If
							ElseIf u.BitmapID = -1 Then 
								'非表示のユニット
								If MaskData(sx + i, sy + j) Then
									'マスクされた地形
									'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
								Else
									'地形
									'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
								End If
							Else
								If MaskData(sx + i, sy + j) Then
									'マスクされたユニット
									'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 64, SRCCOPY)
									
									'ユニットのいる場所に合わせて表示を変更
									Select Case u.Area
										Case "空中"
											DottedLine(xx, yy + 28)
										Case "水中"
											DottedLine(xx, yy + 3)
										Case "地中"
											DottedLine(xx, yy + 28)
											DottedLine(xx, yy + 3)
										Case "宇宙"
											If TerrainClass(sx + i, sy + j) = "月面" Then
												DottedLine(xx, yy + 28)
											End If
									End Select
								Else
									'ユニット
									'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
									
									'ユニットのいる場所に合わせて表示を変更
									Select Case u.Area
										Case "空中"
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = LineTo(pic.hDC, xx + 31, yy + 28)
										Case "水中"
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = LineTo(pic.hDC, xx + 31, yy + 3)
										Case "地中"
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = LineTo(pic.hDC, xx + 31, yy + 28)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = LineTo(pic.hDC, xx + 31, yy + 3)
										Case "宇宙"
											If TerrainClass(sx + i, sy + j) = "月面" Then
												'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
												ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
												'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
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
			pic.ForeColor = System.Drawing.ColorTranslator.FromOle(prev_color)
			
			'画面が書き換えられたことを記録
			ScreenIsSaved = False
			
			If Not without_refresh And Not delay_refresh Then
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Refresh()
			End If
		End With
	End Sub
	' ADD END MARGE
	
	' MOD START MARGE
	'Private Sub DottedLine(ByVal X As Integer, ByVal Y As Integer)
	Private Sub DottedLine(ByVal X As Short, ByVal Y As Short, Optional ByVal half_size As Boolean = False)
		' MOD END MARGE
		Dim i As Short
		
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMain(0)
			' MOD START MARGE
			'        For i = 0 To 15
			'            MainForm.picMain(0).PSet (X + 2 * i + 1, Y), vbBlack
			'        Next
			If half_size Then
				For i = 0 To 7
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.picMain(0).PSet(New Point[](X + 2 * i + 1, Y), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black))
				Next 
			Else
				For i = 0 To 15
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.picMain(0).PSet(New Point[](X + 2 * i + 1, Y), System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black))
				Next 
			End If
			' MOD END MARGE
		End With
	End Sub
	
	'指定されたマップ座標を画面の中央に表示
	Public Sub Center(ByVal new_x As Short, ByVal new_y As Short)
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
			'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		ElseIf MapX > MainForm.HScroll.max Then 
			'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MapX = MainForm.HScroll.max
		End If
		MapY = new_y
		If MapY < 1 Then
			MapY = 1
			'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		ElseIf MapY > MainForm.VScroll.max Then 
			'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MapY = MainForm.VScroll.max
		End If
	End Sub
	
	
	' === 座標変換 ===
	
	'マップ上での座標がマップ画面のどの位置にくるかを返す
	Public Function MapToPixelX(ByVal X As Short) As Short
		' MOD START MARGE
		'    MapToPixelX = 32 * ((MainWidth + 1) \ 2 - 1 - (MapX - X))
		If NewGUIMode Then
			MapToPixelX = 32 * ((MainWidth + 1) \ 2 - 0.5 - (MapX - X))
		Else
			MapToPixelX = 32 * ((MainWidth + 1) \ 2 - 1 - (MapX - X))
		End If
		' MOD END MARGE
	End Function
	
	Public Function MapToPixelY(ByVal Y As Short) As Short
		MapToPixelY = 32 * ((MainHeight + 1) \ 2 - 1 - (MapY - Y))
	End Function
	
	'マップ画面でのピクセルがマップの座標のどの位置にくるかを返す
	Public Function PixelToMapX(ByVal X As Short) As Short
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
	
	Public Function PixelToMapY(ByVal Y As Short) As Short
		If Y < 0 Then
			Y = 0
		ElseIf Y >= MainPHeight Then 
			Y = MainPHeight - 1
		End If
		
		PixelToMapY = Y \ 32 + 1 + MapY - (MainHeight + 1) \ 2
	End Function
	
	
	' === ユニット画像表示に関する処理 ===
	
	'ユニット画像ファイルを検索
	Private Function FindUnitBitmap(ByRef u As Unit) As String
		Dim fname, uname As String
		Dim tnum, tname, tdir As String
		Dim i, j As Short
		
		With u
			'インターミッションでのパイロットステータス表示の場合は
			'特殊な処理が必要
			If .IsFeatureAvailable("ダミーユニット") And InStr(.Name, "ステータス表示用ユニット") = 0 Then
				If .CountPilot = 0 Then
					Exit Function
				End If
				
				If .FeatureData("ダミーユニット") = "ユニット画像使用" Then
					'ユニット画像を使って表示
					uname = "搭乗ユニット[" & .MainPilot.ID & "]"
					'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
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
				If FileExists(AppPath & "Bitmap\Map\" & fname) Or FileExists(ScenarioPath & "Bitmap\Map\" & fname) Then
					fname = "Bitmap\Map\" & fname
				Else
					'地形画像検索用の地形画像ディレクトリ名と4桁ファイル名を作成
					i = Len(fname) - 5
					Do While i > 0
						If Mid(fname, i, 1) Like "[!-0-9]" Then
							Exit Do
						End If
						i = i - 1
					Loop 
					If i > 0 Then
						tdir = Left(fname, i)
						With TDList
							For j = 1 To .Count
								If tdir = .Item(.OrderedID(j)).Bitmap Then
									tnum = Mid(fname, i + 1, Len(fname) - i - 4)
									tname = Left(fname, i) & VB6.Format(StrToLng(tnum), "0000") & ".bmp"
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
						If FileExists(AppPath & "Bitmap\Map\" & tname) Or FileExists(ScenarioPath & "Bitmap\Map\" & tname) Then
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
	Public Function MakeUnitBitmap(ByRef u As Unit) As Short
		Dim fname, uparty As String
		Dim i As Short
		Dim ret As Integer
		Dim xx, yy As Short
		Static bitmap_num As Short
		Static fname_list() As String
		Static party_list() As String
		
		With MainForm
			If u.IsFeatureAvailable("非表示") Then
				MakeUnitBitmap = -1
				Exit Function
			End If
			
			'画像がクリアされている？
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
				If fname = fname_list(i) And uparty = party_list(i) Then
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
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picUnitBitmap.Move(0, 0, 480, 96 * (bitmap_num \ 15 + 1))
			
			'画像の書き込み位置
			xx = 32 * (bitmap_num Mod 15)
			yy = 96 * (bitmap_num \ 15)
			
			'ファイルをロードする
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			LoadUnitBitmap(u, .picUnitBitmap, xx, yy, False, fname)
			
			'行動済みの際の画像を作成
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picUnitBitmap.hDC, xx, yy + 32, 32, 32, .picUnitBitmap.hDC, xx, yy, SRCCOPY)
			'UPGRADE_ISSUE: Control picMask は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picUnitBitmap.hDC, xx, yy + 32, 32, 32, .picMask.hDC, 0, 0, SRCAND)
			
			'マスク入りの画像を作成
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picUnitBitmap.hDC, xx, yy + 64, 32, 32, .picUnitBitmap.hDC, xx, yy + 32, SRCCOPY)
			'UPGRADE_ISSUE: Control picMask2 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picUnitBitmap.hDC, xx, yy + 64, 32, 32, .picMask2.hDC, 0, 0, SRCINVERT)
		End With
		
		'ユニット画像番号を返す
		MakeUnitBitmap = bitmap_num
	End Function
	
	'ユニットのビットマップをロード
	Public Sub LoadUnitBitmap(ByRef u As Unit, ByRef pic As System.Windows.Forms.PictureBox, ByVal dx As Short, ByVal dy As Short, Optional ByVal use_orig_color As Boolean = False, Optional ByRef fname As String = "")
		Dim ret As Integer
		Dim emit_light As Boolean
		
		With MainForm
			'画像ファイルを検索
			If fname = "" Then
				fname = FindUnitBitmap(u)
			End If
			
			'画像をそのまま使用する場合
			If InStr(fname, "\Pilot\") > 0 Or u.FeatureData("ダミーユニット") = "ユニット画像使用" Then
				'画像の読み込み
				On Error GoTo ErrorHandler
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picTmp = System.Drawing.Image.FromFile(fname)
				On Error GoTo 0
				
				'画面に描画
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(pic.hDC, dx, dy, 32, 32, .picTmp.hDC, 0, 0, .picTmp.width, .picTmp.Height, SRCCOPY)
				
				Exit Sub
			End If
			
			'ユニットが自分で発光しているかをあらかじめチェック
			If MapDrawMode = "夜" And Not MapDrawIsMapOnly And Not use_orig_color And u.IsFeatureAvailable("発光") Then
				emit_light = True
			End If
			
			If fname <> "" Then
				'画像が見つかった場合は画像を読み込み
				On Error GoTo ErrorHandler
				'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picTmp32(0) = System.Drawing.Image.FromFile(fname)
				On Error GoTo 0
				
				'画像のサイズが正しいかチェック
				'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				If .picTmp32(0).width <> 32 Or .picTmp32(0).Height <> 32 Then
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					With .picTmp32(0)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.Picture = System.Drawing.Image.FromFile("")
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.width = 32
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.Height = 32
					End With
					ErrorMessage(u.Name & "のユニット画像が32x32の大きさになっていません")
					Exit Sub
				End If
				
				If u.IsFeatureAvailable("地形ユニット") Then
					'地形ユニットの場合は画像をそのまま使う
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
				ElseIf UseTransparentBlt Then 
					'TransparentBltを使ってユニット画像とタイルを重ね合わせる
					
					'タイル
					Select Case u.Party0
						Case "味方", "ＮＰＣ"
							'UPGRADE_ISSUE: Control picUnit は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picUnit.hDC, 0, 0, SRCCOPY)
						Case "敵"
							'UPGRADE_ISSUE: Control picEnemy は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picEnemy.hDC, 0, 0, SRCCOPY)
						Case "中立"
							'UPGRADE_ISSUE: Control picNeautral は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picNeautral.hDC, 0, 0, SRCCOPY)
					End Select
					
					'画像の重ね合わせ
					'(発光している場合は２度塗りを防ぐため描画しない)
					If Not emit_light Then
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = TransparentBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					End If
				Else
					'BitBltを使ってユニット画像とタイルを重ね合わせる
					
					'マスクを作成
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MakeMask(.picTmp32(0).hDC, .picTmp32(2).hDC, 32, 32, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					
					'タイル
					Select Case u.Party0
						Case "味方", "ＮＰＣ"
							'UPGRADE_ISSUE: Control picUnit は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picUnit.hDC, 0, 0, SRCCOPY)
						Case "敵"
							'UPGRADE_ISSUE: Control picEnemy は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picEnemy.hDC, 0, 0, SRCCOPY)
						Case "中立"
							'UPGRADE_ISSUE: Control picNeautral は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picNeautral.hDC, 0, 0, SRCCOPY)
					End Select
					
					'画像の重ね合わせ
					'(発光している場合は２度塗りを防ぐため描画しない)
					If Not emit_light Then
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(2).hDC, 0, 0, SRCERASE)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(0).hDC, 0, 0, SRCINVERT)
					End If
				End If
			Else
				'画像が見つからなかった場合はタイルのみでユニット画像を作成
				Select Case u.Party0
					Case "味方", "ＮＰＣ"
						'UPGRADE_ISSUE: Control picUnit は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picUnit.hDC, 0, 0, SRCCOPY)
					Case "敵"
						'UPGRADE_ISSUE: Control picEnemy は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picEnemy.hDC, 0, 0, SRCCOPY)
					Case "中立"
						'UPGRADE_ISSUE: Control picNeautral は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picNeautral.hDC, 0, 0, SRCCOPY)
				End Select
			End If
			
			'色をステージの状況に合わせて変更
			If Not use_orig_color And Not MapDrawIsMapOnly Then
				Select Case MapDrawMode
					Case "夜"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						Dark()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
						'ユニットが"発光"の特殊能力を持つ場合、
						'ユニット画像を、暗くしたタイル画像の上に描画する。
						If emit_light Then
							If UseTransparentBlt Then
								'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								ret = TransparentBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
							Else
								'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(2).hDC, 0, 0, SRCERASE)
								'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(0).hDC, 0, 0, SRCINVERT)
							End If
						End If
					Case "セピア"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						Sepia()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
					Case "白黒"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						Monotone()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
					Case "夕焼け"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						Sunset()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
					Case "水中"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						Water()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
					Case "フィルタ"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
				End Select
			End If
			
			'画面に描画
			'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, dx, dy, 32, 32, .picTmp32(1).hDC, 0, 0, SRCCOPY)
		End With
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("ユニット用ビットマップファイル" & vbCr & vbLf & fname & vbCr & vbLf & "の読み込み中にエラーが発生しました。" & vbCr & vbLf & "画像ファイルが壊れていないか確認して下さい。")
	End Sub
	
	'ユニット画像の描画
	Public Sub PaintUnitBitmap(ByRef u As Unit, Optional ByVal smode As String = "")
		Dim xx, yy As Short
		Dim pic As System.Windows.Forms.PictureBox
		Dim ret As Integer
		Dim PT As POINTAPI
		
		With u
			'非表示？
			If .BitmapID = -1 Then
				Exit Sub
			End If
			
			'画面外？
			If .X < MapX - (MainWidth + 1) \ 2 Or MapX + (MainWidth + 1) \ 2 < .X Or .Y < MapY - (MainHeight + 1) \ 2 Or MapY + (MainHeight + 1) \ 2 < .Y Then
				Exit Sub
			End If
			
			'描き込み先の座標を設定
			xx = MapToPixelX(.X)
			yy = MapToPixelY(.Y)
		End With
		
		With MainForm
			If smode = "リフレッシュ無し" And ScreenIsSaved Then
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = .picMain(1)
				'表示画像を消去する際に使う描画領域を設定
				PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(xx, 0))
				PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(yy, 0))
				PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(xx + 32, MainPWidth - 1))
				PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(yy + 32, MainPHeight - 1))
			Else
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = .picMain(0)
			End If
			
			'ユニット画像の書き込み
			If u.Action > 0 Or u.IsFeatureAvailable("地形ユニット") Then
				'通常の表示
				'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
			Else
				'行動済の場合は網掛け
				'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, SRCCOPY)
			End If
			
			'直線を描画する際の描画色を設定
			pic.ForeColor = System.Drawing.Color.Black
			
			'ユニットのいる場所に合わせて表示を変更
			Select Case u.Area
				Case "空中"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, xx + 31, yy + 28)
				Case "水中"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, xx + 31, yy + 3)
				Case "地中"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, xx + 31, yy + 28)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, xx + 31, yy + 3)
				Case "宇宙"
					If TerrainClass(u.X, u.Y) = "月面" Then
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = LineTo(pic.hDC, xx + 31, yy + 28)
					End If
			End Select
			
			'描画色を白に戻しておく
			pic.ForeColor = System.Drawing.Color.White
			
			If smode <> "リフレッシュ無し" Then
				'画面が書き換えられたことを記録
				ScreenIsSaved = False
				
				If .Visible Then
					pic.Refresh()
				End If
			End If
		End With
	End Sub
	
	'ユニット画像の表示を消す
	Public Sub EraseUnitBitmap(ByVal X As Short, ByVal Y As Short, Optional ByVal do_refresh As Boolean = True)
		Dim xx, yy As Short
		Dim ret As Integer
		
		'画面外？
		If X < MapX - (MainWidth + 1) \ 2 Or MapX + (MainWidth + 1) \ 2 < X Or Y < MapY - (MainHeight + 1) \ 2 Or MapY + (MainHeight + 1) \ 2 < Y Then
			Exit Sub
		End If
		
		'画面が乱れるので書き換えない？
		If IsPictureVisible Then
			Exit Sub
		End If
		
		xx = MapToPixelX(X)
		yy = MapToPixelY(Y)
		
		With MainForm
			SaveScreen()
			
			'画面表示変更
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picMain(1).hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
			
			If do_refresh Then
				'画面が書き換えられたことを記録
				ScreenIsSaved = False
				
				If .Visible Then
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.picMain(0).Refresh()
				End If
			End If
		End With
	End Sub
	
	'ユニット画像の表示位置を移動 (アニメーション)
	Public Sub MoveUnitBitmap(ByRef u As Unit, ByVal x1 As Short, ByVal y1 As Short, ByVal x2 As Short, ByVal y2 As Short, ByVal wait_time0 As Integer, Optional ByVal division As Short = 2)
		Dim xx, yy As Short
		Dim vx, vy As Short
		Dim ret As Integer
		Dim i As Short
		Dim pic As System.Windows.Forms.PictureBox
		Dim cur_time, start_time, wait_time As Integer
		Dim PT As POINTAPI
		
		wait_time = wait_time0 \ division
		
		SaveScreen()
		
		With MainForm
			'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = .picTmp32(0)
			
			'ユニット画像を作成
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, 0, 0, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
			
			'ユニットのいる場所に合わせて表示を変更
			Select Case u.Area
				Case "空中"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 28)
				Case "水中"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 3)
				Case "地中"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 28)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 3)
				Case "宇宙"
					If TerrainClass(u.X, u.Y) = "月面" Then
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = MoveToEx(pic.hDC, 0, 28, PT)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = LineTo(pic.hDC, 31, 28)
					End If
			End Select
			
			'移動の始点を設定
			xx = MapToPixelX(x1)
			yy = MapToPixelY(y1)
			
			'背景上の画像をまず消去
			'(既に移動している場合を除く)
			If u Is MapDataForUnit(x1, y1) Then
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (x1 - 1), 32 * (y1 - 1), SRCCOPY)
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(1).hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (x1 - 1), 32 * (y1 - 1), SRCCOPY)
			End If
			
			'最初の移動方向を設定
			If System.Math.Abs(x2 - x1) > System.Math.Abs(y2 - y1) Then
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
			For i = 1 To division * MaxLng(System.Math.Abs(x2 - x1), System.Math.Abs(y2 - y1))
				'画像を消去
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, .picMain(1).hDC, xx, yy, SRCCOPY)
				
				'座標を移動
				xx = xx + 32 * vx \ division
				yy = yy + 32 * vy \ division
				
				'画像を描画
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, pic.hDC, 0, 0, SRCCOPY)
				
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Refresh()
				
				If wait_time > 0 Then
					Do 
						System.Windows.Forms.Application.DoEvents()
						cur_time = timeGetTime()
					Loop While start_time + wait_time > cur_time
					start_time = cur_time
				End If
			Next 
			
			'２回目の移動方向を設定
			If System.Math.Abs(x2 - x1) > System.Math.Abs(y2 - y1) Then
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
			For i = 1 To division * MinLng(System.Math.Abs(x2 - x1), System.Math.Abs(y2 - y1))
				'画像を消去
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, .picMain(1).hDC, xx, yy, SRCCOPY)
				
				'座標を移動
				xx = xx + 32 * vx \ division
				yy = yy + 32 * vy \ division
				
				'画像を描画
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, pic.hDC, 0, 0, SRCCOPY)
				
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Refresh()
				
				If wait_time > 0 Then
					Do 
						System.Windows.Forms.Application.DoEvents()
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
	Public Sub MoveUnitBitmap2(ByRef u As Unit, ByVal wait_time0 As Integer, Optional ByVal division As Short = 2)
		Dim xx, yy As Short
		Dim vx, vy As Short
		Dim ret As Integer
		Dim i, j As Short
		Dim pic As System.Windows.Forms.PictureBox
		Dim cur_time, start_time, wait_time As Integer
		Dim PT As POINTAPI
		Dim move_route_x() As Short
		Dim move_route_y() As Short
		
		wait_time = wait_time0 \ division
		
		SaveScreen()
		
		With MainForm
			'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = .picTmp32(0)
			
			'ユニット画像を作成
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, 0, 0, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
			
			'ユニットのいる場所に合わせて表示を変更
			Select Case u.Area
				Case "空中"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 28)
				Case "水中"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 3)
				Case "地中"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 28)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 3)
				Case "宇宙"
					If TerrainClass(u.X, u.Y) = "月面" Then
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = MoveToEx(pic.hDC, 0, 28, PT)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = LineTo(pic.hDC, 31, 28)
					End If
			End Select
			
			'移動経路を検索
			SearchMoveRoute((u.X), (u.Y), move_route_x, move_route_y)
			
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
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, .picMain(1).hDC, xx, yy, SRCCOPY)
					
					'座標を移動
					xx = xx + vx \ division
					yy = yy + vy \ division
					
					'画像を描画
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, pic.hDC, 0, 0, SRCCOPY)
					
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.picMain(0).Refresh()
					
					If wait_time > 0 Then
						Do 
							System.Windows.Forms.Application.DoEvents()
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
	Public Function ListBox(ByRef lb_caption As String, ByRef list() As String, ByRef lb_info As String, Optional ByRef lb_mode As String = "") As Short
		Dim i As Short
		Dim is_rbutton_released As Boolean
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmListBox)
		With frmListBox
			.WindowState = System.Windows.Forms.FormWindowState.Normal
			
			'コメントウィンドウの処理
			If InStr(lb_mode, "コメント") > 0 Then
				If Not .txtComment.Enabled Then
					.txtComment.Enabled = True
					.txtComment.Visible = True
					.txtComment.Width = .labCaption.Width
					.txtComment.Text = ""
					.txtComment.Top = .lstItems.Top + .lstItems.Height + 5
					.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) + 600)
				End If
			Else
				If .txtComment.Enabled Then
					.txtComment.Enabled = False
					.txtComment.Visible = False
					.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - 600)
				End If
			End If
			
			'キャプション
			.Text = lb_caption
			If UBound(ListItemFlag) > 0 Then
				.labCaption.Text = "  " & lb_info
			Else
				.labCaption.Text = lb_info
			End If
			
			'リストボックスにアイテムを追加
			.lstItems.Visible = False
			.lstItems.Items.Clear()
			If UBound(ListItemFlag) > 0 Then
				For i = 1 To UBound(list)
					If ListItemFlag(i) Then
						.lstItems.Items.Add("×" & list(i))
					Else
						.lstItems.Items.Add("  " & list(i))
					End If
				Next 
				i = UBound(list)
				Do While i > 0
					If Not ListItemFlag(i) Then
						.lstItems.SelectedIndex = i - 1
						Exit Do
					End If
					i = i - 1
				Loop 
			Else
				For i = 1 To UBound(list)
					.lstItems.Items.Add(list(i))
				Next 
			End If
			.lstItems.SelectedIndex = -1
			.lstItems.Visible = True
			
			'コメント付きのアイテム？
			If UBound(ListItemComment) <> UBound(list) Then
				ReDim Preserve ListItemComment(UBound(list))
			End If
			
			'最小化されている場合は戻しておく
			If .WindowState <> System.Windows.Forms.FormWindowState.Normal Then
				.WindowState = System.Windows.Forms.FormWindowState.Normal
				.Show()
			End If
			
			'表示位置を設定
			If MainForm.Visible And .HorizontalSize = "S" Then
				.Left = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(MainForm.Left))
			Else
				.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
			End If
			If MainForm.Visible And Not MainForm.WindowState = 1 And .VerticalSize = "M" And InStr(lb_mode, "中央表示") = 0 Then
				.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(MainForm.Top) + VB6.PixelsToTwipsY(MainForm.Height) - VB6.PixelsToTwipsY(.Height))
			Else
				.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			End If
			
			'先頭のアイテムを設定
			If TopItem > 0 Then
				If .lstItems.TopIndex <> TopItem - 1 Then
					.lstItems.TopIndex = MaxLng(MinLng(TopItem - 1, .lstItems.Items.Count - 1), 0)
				End If
				'UPGRADE_ISSUE: ListBox プロパティ lstItems.Columns はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				If .lstItems.Columns Then
					.lstItems.SelectedIndex = TopItem - 1
				End If
			Else
				'UPGRADE_ISSUE: ListBox プロパティ lstItems.Columns はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				If .lstItems.Columns Then
					.lstItems.SelectedIndex = 0
				End If
			End If
			
			'コメントウィンドウの表示
			If .txtComment.Enabled Then
				.txtComment.Text = ListItemComment(.lstItems.SelectedIndex + 1)
			End If
			
			SelectedItem = 0
			
			IsFormClicked = False
			System.Windows.Forms.Application.DoEvents()
			
			'リストボックスを表示
			If InStr(lb_mode, "表示のみ") > 0 Then
				'表示のみを行う
				IsMordal = False
				.Show()
				.lstItems.Focus()
				Call SetWindowPos(.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
				.Refresh()
				Exit Function
			ElseIf InStr(lb_mode, "連続表示") > 0 Then 
				'選択が行われてもリストボックスを閉じない
				IsMordal = False
				If Not .Visible Then
					.Show()
					Call SetWindowPos(.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
					.lstItems.Focus()
				End If
				
				If InStr(lb_mode, "カーソル移動") > 0 Then
					If AutoMoveCursor Then
						MoveCursorPos("ダイアログ")
					End If
				End If
				
				Do Until IsFormClicked
					System.Windows.Forms.Application.DoEvents()
					'右ボタンでのダブルクリックの実現
					If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
						is_rbutton_released = True
					Else
						If is_rbutton_released Then
							IsFormClicked = True
						End If
					End If
					Sleep(50)
				Loop 
			Else
				'選択が行われた時点でリストボックスを閉じる
				IsMordal = False
				.Show()
				Call SetWindowPos(.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
				.lstItems.Focus()
				
				If InStr(lb_mode, "カーソル移動") > 0 Then
					If AutoMoveCursor Then
						MoveCursorPos("ダイアログ")
					End If
				End If
				
				Do Until IsFormClicked
					System.Windows.Forms.Application.DoEvents()
					'右ボタンでのダブルクリックの実現
					If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
						is_rbutton_released = True
					Else
						If is_rbutton_released Then
							IsFormClicked = True
						End If
					End If
					Sleep(50)
				Loop 
				.Hide()
				
				If InStr(lb_mode, "カーソル移動") > 0 And InStr(lb_mode, "カーソル移動(行きのみ)") = 0 Then
					If AutoMoveCursor Then
						RestoreCursorPos()
					End If
				End If
				
				If .txtComment.Enabled Then
					.txtComment.Enabled = False
					.txtComment.Visible = False
					.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - 600)
				End If
			End If
			
			ListBox = SelectedItem
			System.Windows.Forms.Application.DoEvents()
		End With
	End Function
	
	'リストボックスの高さを大きくする
	Public Sub EnlargeListBoxHeight()
		With frmListBox
			Select Case .VerticalSize
				Case "M"
					If .WindowState <> System.Windows.Forms.FormWindowState.Normal Then
						.Visible = True
						.WindowState = System.Windows.Forms.FormWindowState.Normal
					End If
					.Visible = False
					.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) + 2400)
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
					If .WindowState <> System.Windows.Forms.FormWindowState.Normal Then
						.Visible = True
						.WindowState = System.Windows.Forms.FormWindowState.Normal
					End If
					.Visible = False
					.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - 2400)
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
					If .WindowState <> System.Windows.Forms.FormWindowState.Normal Then
						.Visible = True
						.WindowState = System.Windows.Forms.FormWindowState.Normal
					End If
					.Visible = False
					.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(.Width) + 2350)
					.lstItems.Width = 637
					.labCaption.Width = 637
					.HorizontalSize = "M"
			End Select
		End With
	End Sub
	
	'リストボックスの幅を小さくする
	Public Sub ReduceListBoxWidth()
		With frmListBox
			Select Case .HorizontalSize
				Case "M"
					If .WindowState <> System.Windows.Forms.FormWindowState.Normal Then
						.Visible = True
						.WindowState = System.Windows.Forms.FormWindowState.Normal
					End If
					.Visible = False
					.Width = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(.Width) - 2350)
					.lstItems.Width = 486
					.labCaption.Width = 486
					.HorizontalSize = "S"
			End Select
		End With
	End Sub
	
	'武器選択用にリストボックスを切り替え
	Public Sub AddPartsToListBox()
		Dim ret As Integer
		Dim fname As String
		Dim u, t As Unit
		
		u = SelectedUnit
		t = SelectedTarget
		
		With frmListBox
			'リストボックスにユニットやＨＰのゲージを追加
			.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) + 535)
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
				.imgPilot1.Image = System.Drawing.Image.FromFile(ScenarioPath & fname)
			ElseIf FileExists(ExtDataPath & fname) Then 
				.imgPilot1.Image = System.Drawing.Image.FromFile(ExtDataPath & fname)
			ElseIf FileExists(ExtDataPath2 & fname) Then 
				.imgPilot1.Image = System.Drawing.Image.FromFile(ExtDataPath2 & fname)
			ElseIf FileExists(AppPath & fname) Then 
				.imgPilot1.Image = System.Drawing.Image.FromFile(AppPath & fname)
			Else
				.imgPilot1.Image = System.Drawing.Image.FromFile("")
			End If
			
			.txtLevel1.Text = VB6.Format(u.MainPilot.Level)
			.txtMorale1.Text = VB6.Format(u.MainPilot.Morale)
			
			If MapDrawMode = "" Then
				If u.BitmapID > 0 Then
					'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ picUnit1.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
				Else
					'非表示のユニットの場合はユニットのいる地形タイルを表示
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ picUnit1.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * (u.X - 1), 32 * (u.Y - 1), SRCCOPY)
				End If
			Else
				LoadUnitBitmap(u, .picUnit1, 0, 0, True)
			End If
			.picUnit1.Refresh()
			
			If u.IsConditionSatisfied("データ不明") Then
				.labHP1.Text = Term("HP")
				.txtHP1.Text = "?????/?????"
			Else
				.labHP1.Text = Term("HP")
				If u.HP < 100000 Then
					.txtHP1.Text = VB6.Format(u.HP)
				Else
					.txtHP1.Text = "?????"
				End If
				If u.MaxHP < 100000 Then
					.txtHP1.Text = .txtHP1.Text & "/" & VB6.Format(u.MaxHP)
				Else
					.txtHP1.Text = .txtHP1.Text & "/?????"
				End If
			End If
			'UPGRADE_ISSUE: PictureBox メソッド picHP1.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.picHP1.Cls()
		End With
		'UPGRADE_ISSUE: PictureBox メソッド picHP1.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		frmListBox.picHP1.Line (0, 0) - ((frmListBox.picHP1.Width - 4) * u.HP \ u.MaxHP - 1, 4), BF
		
		With frmListBox
			If u.IsConditionSatisfied("データ不明") Then
				.labEN1.Text = Term("EN")
				.txtEN1.Text = "???/???"
			Else
				.labEN1.Text = Term("EN", t)
				If u.EN < 1000 Then
					.txtEN1.Text = VB6.Format(u.EN)
				Else
					.txtEN1.Text = "???"
				End If
				If u.MaxEN < 1000 Then
					.txtEN1.Text = .txtEN1.Text & "/" & VB6.Format(u.MaxEN)
				Else
					.txtEN1.Text = .txtEN1.Text & "/???"
				End If
			End If
			'UPGRADE_ISSUE: PictureBox メソッド picEN1.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.picEN1.Cls()
		End With
		'UPGRADE_ISSUE: PictureBox メソッド picEN1.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		frmListBox.picEN1.Line (0, 0) - ((frmListBox.picEN1.Width - 4) * u.EN \ u.MaxEN - 1, 4), BF
		
		With frmListBox
			'ターゲット側の表示
			fname = "Bitmap\Pilot\" & t.MainPilot.Bitmap
			If FileExists(ScenarioPath & fname) Then
				.imgPilot2.Image = System.Drawing.Image.FromFile(ScenarioPath & fname)
			ElseIf FileExists(ExtDataPath & fname) Then 
				.imgPilot2.Image = System.Drawing.Image.FromFile(ExtDataPath & fname)
			ElseIf FileExists(ExtDataPath2 & fname) Then 
				.imgPilot2.Image = System.Drawing.Image.FromFile(ExtDataPath2 & fname)
			ElseIf FileExists(AppPath & fname) Then 
				.imgPilot2.Image = System.Drawing.Image.FromFile(AppPath & fname)
			Else
				.imgPilot2.Image = System.Drawing.Image.FromFile("")
			End If
			
			.txtLevel2.Text = VB6.Format(t.MainPilot.Level)
			.txtMorale2.Text = VB6.Format(t.MainPilot.Morale)
			
			If MapDrawMode = "" Then
				If t.BitmapID > 0 Then
					'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ picUnit2.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * (t.BitmapID Mod 15), 96 * (t.BitmapID \ 15), SRCCOPY)
				Else
					'非表示のユニットの場合はユニットのいる地形タイルを表示
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ picUnit2.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * (t.X - 1), 32 * (t.Y - 1), SRCCOPY)
				End If
			Else
				LoadUnitBitmap(t, .picUnit2, 0, 0, True)
			End If
			.picUnit2.Refresh()
			
			If t.IsConditionSatisfied("データ不明") Then
				.labHP2.Text = Term("HP")
				.txtHP2.Text = "?????/?????"
			Else
				.labHP2.Text = Term("HP", t)
				If t.HP < 100000 Then
					.txtHP2.Text = VB6.Format(t.HP)
				Else
					.txtHP2.Text = "?????"
				End If
				If t.MaxHP < 100000 Then
					.txtHP2.Text = .txtHP2.Text & "/" & VB6.Format(t.MaxHP)
				Else
					.txtHP2.Text = .txtHP2.Text & "/?????"
				End If
			End If
			'UPGRADE_ISSUE: PictureBox メソッド picHP2.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.picHP2.Cls()
		End With
		'UPGRADE_ISSUE: PictureBox メソッド picHP2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		frmListBox.picHP2.Line (0, 0) - ((frmListBox.picHP2.Width - 4) * t.HP \ t.MaxHP - 1, 4), BF
		
		With frmListBox
			If t.IsConditionSatisfied("データ不明") Then
				.labEN2.Text = Term("EN")
				.txtEN2.Text = "???/???"
			Else
				.labEN2.Text = Term("EN", t)
				If t.EN < 1000 Then
					.txtEN2.Text = VB6.Format(t.EN)
				Else
					.txtEN2.Text = "???"
				End If
				If t.MaxEN < 1000 Then
					.txtEN2.Text = .txtEN2.Text & "/" & VB6.Format(t.MaxEN)
				Else
					.txtEN2.Text = .txtEN2.Text & "/???"
				End If
			End If
			'UPGRADE_ISSUE: PictureBox メソッド picEN2.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.picEN2.Cls()
		End With
		'UPGRADE_ISSUE: PictureBox メソッド picEN2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		frmListBox.picEN2.Line (0, 0) - ((frmListBox.picEN2.Width - 4) * t.EN \ t.MaxEN - 1, 4), BF
	End Sub
	
	'武器選択用リストボックスを通常のものに切り替え
	Public Sub RemovePartsOnListBox()
		With frmListBox
			.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - 535)
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
	Public Function WeaponListBox(ByRef u As Unit, ByRef caption_msg As String, ByRef lb_mode As String, Optional ByRef BGM As String = "") As Short
		Dim ret, j, i, k, w As Short
		Dim list() As String
		Dim wlist() As Short
		Dim warray() As Short
		Dim wpower() As Integer
		Dim wclass As String
		Dim is_rbutton_released As Boolean
		Dim buf As String
		
		With u
			ReDim warray(.CountWeapon)
			ReDim wpower(.CountWeapon)
			ReDim ListItemFlag(.CountWeapon)
			Dim ToolTips(.CountWeapon) As Object
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
							If .Weapon(i - j).ENConsumption = 0 And .Weapon(warray(i - j)).Bullet = 0 Then
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
							If .IsDisabled((.Weapon(w).Name)) Then
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
							If .IsDisabled((.Weapon(w).Name)) Then
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
						list(UBound(list)) = RightPaddedString(.Nickname, 27) & LeftPaddedString(VB6.Format(wpower(w)), 4)
					Else
						list(UBound(list)) = RightPaddedString(.Nickname, 26) & LeftPaddedString(VB6.Format(wpower(w)), 5)
					End If
					
					'最大射程
					If u.WeaponMaxRange(w) > 1 Then
						buf = VB6.Format(.MinRange) & "-" & VB6.Format(u.WeaponMaxRange(w))
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(buf, 5)
					Else
						list(UBound(list)) = list(UBound(list)) & "    1"
					End If
					
					'命中率修正
					If u.WeaponPrecision(w) >= 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString("+" & VB6.Format(u.WeaponPrecision(w)), 4)
					Else
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponPrecision(w)), 4)
					End If
					
					'残り弾数
					If .Bullet > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.Bullet(w)), 3)
					Else
						list(UBound(list)) = list(UBound(list)) & "  -"
					End If
					
					'ＥＮ消費量
					If .ENConsumption > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponENConsumption(w)), 4)
					Else
						list(UBound(list)) = list(UBound(list)) & "   -"
					End If
					
					'クリティカル率修正
					If u.WeaponCritical(w) >= 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString("+" & VB6.Format(u.WeaponCritical(w)), 4)
					Else
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponCritical(w)), 4)
					End If
					
					'地形適応
					list(UBound(list)) = list(UBound(list)) & " " & .Adaption
					
					'必要気力
					If .NecessaryMorale > 0 Then
						list(UBound(list)) = list(UBound(list)) & " 気" & .NecessaryMorale
					End If
					
					'属性
					wclass = u.WeaponClass(w)
					If InStrNotNest(wclass, "|") > 0 Then
						wclass = Left(wclass, InStrNotNest(wclass, "|") - 1)
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
			ret = ListBox(caption_msg, list, "名称                       攻撃 射程  命 弾  " & Term("EN", u, 2) & "  " & Term("CT", u, 2) & " 適応 分類", "表示のみ")
			
			If AutoMoveCursor Then
				If lb_mode <> "一覧" Then
					MoveCursorPos("武器選択")
				Else
					MoveCursorPos("ダイアログ")
				End If
			End If
			If BGM <> "" Then
				ChangeBGM(BGM)
			End If
			
			Do While True
				Do Until IsFormClicked
					System.Windows.Forms.Application.DoEvents()
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
					
					SelectedItem = ListBox(caption_msg, list, "名称                       攻撃 射程  命 弾  " & Term("EN", u, 2) & "  " & Term("CT", u, 2) & " 適応 分類", "表示のみ")
				End If
			Loop 
			
			If lb_mode <> "一覧" Then
				frmListBox.Hide()
			End If
			ReDim ListItemComment(0)
			WeaponListBox = wlist(SelectedItem)
			
		ElseIf lb_mode = "反撃" Then 
			'反撃武器選択時の表示
			
			For i = 1 To u.CountWeapon
				w = warray(i)
				
				With u
					'Disableコマンドで使用不可にされた武器は表示しない
					If .IsDisabled((.Weapon(w).Name)) Then
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
					ElseIf Not .IsNormalWeapon(w) And .CriticalProbability(w, SelectedUnit) > 0 Then 
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
					list(UBound(list)) = RightPaddedString(.Nickname, 29) & LeftPaddedString(VB6.Format(wpower(w)), 4)
					
					'命中率
					If Not IsOptionDefined("予測命中率非表示") Then
						buf = VB6.Format(MinLng(u.HitProbability(w, SelectedUnit, True), 100)) & "%"
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(buf, 5)
					ElseIf u.WeaponPrecision(w) >= 0 Then 
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString("+" & VB6.Format(u.WeaponPrecision(w)), 5)
					Else
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponPrecision(w)), 5)
					End If
					
					
					'クリティカル率
					If Not IsOptionDefined("予測命中率非表示") Then
						buf = VB6.Format(MinLng(u.CriticalProbability(w, SelectedUnit), 100)) & "%"
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(buf, 5)
					ElseIf u.WeaponCritical(w) >= 0 Then 
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString("+" & VB6.Format(u.WeaponCritical(w)), 5)
					Else
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponCritical(w)), 5)
					End If
					
					'残り弾数
					If .Bullet > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.Bullet(w)), 3)
					Else
						list(UBound(list)) = list(UBound(list)) & "  -"
					End If
					
					'ＥＮ消費量
					If .ENConsumption > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponENConsumption(w)), 4)
					Else
						list(UBound(list)) = list(UBound(list)) & "   -"
					End If
					
					'地形適応
					list(UBound(list)) = list(UBound(list)) & " " & .Adaption
					
					'必要気力
					If .NecessaryMorale > 0 Then
						list(UBound(list)) = list(UBound(list)) & " 気" & .NecessaryMorale
					End If
					
					'属性
					wclass = u.WeaponClass(w)
					If InStrNotNest(wclass, "|") > 0 Then
						wclass = Left(wclass, InStrNotNest(wclass, "|") - 1)
					End If
					list(UBound(list)) = list(UBound(list)) & " " & wclass
				End With
NextLoop2: 
			Next 
			
			'リストボックスを表示
			TopItem = -1
			ret = ListBox(caption_msg, list, "名称                         攻撃 命中 " & Term("CT", u, 2) & "   弾  " & Term("EN", u, 2) & " 適応 分類", "連続表示,カーソル移動")
			WeaponListBox = wlist(ret)
		End If
		
		System.Windows.Forms.Application.DoEvents()
	End Function
	
	'アビリティ選択用リストボックス
	Public Function AbilityListBox(ByRef u As Unit, ByRef caption_msg As String, ByRef lb_mode As String, Optional ByVal is_item As Boolean = False) As Short
		Dim j, i, k As Short
		Dim ret As Short
		Dim msg, buf, rest_msg As String
		Dim list() As String
		Dim alist() As Short
		Dim is_available As Boolean
		Dim is_rbutton_released As Boolean
		
		With u
			'アビリティが一つしかない場合は自動的にそのアビリティを選択する。
			'リストボックスの表示は行わない。
			'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			If lb_mode <> "一覧" And Not is_item And MainForm.mnuUnitCommandItem(AbilityCmdID).Caption <> Term("アビリティ", u) Then
				For i = 1 To .CountAbility
					If Not .Ability(i).IsItem And .IsAbilityMastered(i) Then
						AbilityListBox = i
						Exit Function
					End If
				Next 
			End If
			
			ReDim list(0)
			Dim aist(0) As Object
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
						If .IsDisabled((.Ability(i).Name)) Then
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
						If .IsDisabled((.Ability(i).Name)) Then
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
							k = InStr(msg, Mid(.EffectName(j), InStr(.EffectName(j), "(")))
							If k > 0 Then
								msg = Left(msg, k - 1) & "、" & Left(.EffectName(j), InStr(.EffectName(j), "(") - 1) & Mid(msg, k)
							Else
								msg = msg & " " & .EffectName(j)
							End If
						ElseIf .EffectName(j) <> "" Then 
							msg = msg & " " & .EffectName(j)
						End If
					Next 
					msg = Trim(msg)
					
					'効果解説が長すぎる場合は改行
					'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
					buf = StrConv(msg, vbFromUnicode)
					'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
					If LenB(buf) > 32 Then
						Do 
							'UPGRADE_ISSUE: 定数 vbUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
							buf = StrConv(buf, vbUnicode)
							buf = Left(buf, Len(buf) - 1)
							'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
							buf = StrConv(buf, vbFromUnicode)
							'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
						Loop While LenB(buf) >= 32
						'UPGRADE_ISSUE: 定数 vbUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
						buf = StrConv(buf, vbUnicode)
						rest_msg = Mid(msg, Len(buf) + 1)
						'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
						'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
						If LenB(StrConv(buf, vbFromUnicode)) < 32 Then
							'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
							'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
							buf = buf & Space(32 - LenB(StrConv(buf, vbFromUnicode)))
						End If
						msg = buf
					End If
					
					list(UBound(list)) = RightPaddedString(list(UBound(list)) & " " & msg, 53)
					
					'最大射程
					If u.AbilityMaxRange(i) > 1 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.AbilityMinRange(i)) & "-" & VB6.Format(u.AbilityMaxRange(i)), 4)
					ElseIf u.AbilityMaxRange(i) = 1 Then 
						list(UBound(list)) = list(UBound(list)) & "   1"
					Else
						list(UBound(list)) = list(UBound(list)) & "   -"
					End If
					
					'残り使用回数
					If .Stock > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.Stock(i)), 3)
					Else
						list(UBound(list)) = list(UBound(list)) & "  -"
					End If
					
					'ＥＮ消費量
					If .ENConsumption > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.AbilityENConsumption(i)), 4)
					Else
						list(UBound(list)) = list(UBound(list)) & "   -"
					End If
					
					'必要気力
					If .NecessaryMorale > 0 Then
						list(UBound(list)) = list(UBound(list)) & " 気" & .NecessaryMorale
					End If
					
					'属性
					If InStrNotNest(.Class_Renamed, "|") > 0 Then
						list(UBound(list)) = list(UBound(list)) & " " & Left(.Class_Renamed, InStrNotNest(.Class_Renamed, "|") - 1)
					Else
						list(UBound(list)) = list(UBound(list)) & " " & .Class_Renamed
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
		ret = ListBox(caption_msg, list, "名称                 効果                            射程 数  " & Term("EN", u, 2) & " 分類", "表示のみ")
		
		If AutoMoveCursor Then
			MoveCursorPos("ダイアログ")
		End If
		
		Do Until IsFormClicked
			System.Windows.Forms.Application.DoEvents()
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
			frmListBox.Hide()
		End If
		ReDim ListItemComment(0)
		AbilityListBox = alist(SelectedItem)
		
		System.Windows.Forms.Application.DoEvents()
	End Function
	
	'入力時間制限付きのリストボックスを表示
	Public Function LIPS(ByRef lb_caption As String, ByRef list() As String, ByRef lb_info As String, ByVal time_limit As Short) As Short
		Dim i As Short
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmListBox)
		With frmListBox
			'表示内容を設定
			.Text = lb_caption
			.labCaption.Text = "  " & lb_info
			.lstItems.Items.Clear()
			For i = 1 To UBound(list)
				.lstItems.Items.Add("  " & list(i))
			Next 
			.lstItems.SelectedIndex = 0
			.lstItems.Height = 86
			
			'表示位置を設定
			.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
			If MainForm.Visible = True And Not MainForm.WindowState = 1 Then
				.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(MainForm.Top) + VB6.PixelsToTwipsY(MainForm.Height) - VB6.PixelsToTwipsY(.Height))
			Else
				.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			End If
			
			'入力制限時間に関する設定を行う
			.CurrentTime = 0
			.TimeLimit = time_limit
			.picBar.Visible = True
			.Timer1.Enabled = True
			
			'リストボックスを表示し、プレイヤーからの入力を待つ
			SelectedItem = 0
			IsFormClicked = False
			.ShowDialog()
			.CurrentTime = 0
			LIPS = SelectedItem
			
			'リストボックスを消去
			.lstItems.Height = 100
			.picBar.Visible = False
			.Timer1.Enabled = False
		End With
	End Function
	
	'複数段のリストボックスを表示
	Public Function MultiColumnListBox(ByRef lb_caption As String, ByRef list() As String, ByVal is_center As Boolean) As Short
		Dim i As Short
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmMultiColumnListBox)
		With frmMultiColumnListBox
			.Text = lb_caption
			.lstItems.Visible = False
			.lstItems.Items.Clear()
			
			'アイテムを追加
			For i = 1 To UBound(list)
				If ListItemFlag(i) Then
					.lstItems.Items.Add("×" & list(i))
				Else
					.lstItems.Items.Add("  " & list(i))
				End If
			Next 
			
			For i = 1 To UBound(list)
				If Not ListItemFlag(UBound(list) - i + 1) Then
					.lstItems.SelectedIndex = UBound(list) - i
					Exit For
				End If
			Next 
			
			.lstItems.SelectedIndex = -1
			.lstItems.Visible = True
			
			If UBound(ListItemComment) <> UBound(list) Then
				ReDim Preserve ListItemComment(UBound(list))
			End If
			
			'表示位置を設定
			.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
			If MainForm.Visible = True And Not MainForm.WindowState = 1 And Not is_center Then
				.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(MainForm.Top) + VB6.PixelsToTwipsY(MainForm.Height) - VB6.PixelsToTwipsY(.Height))
			Else
				.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			End If
			
			'先頭に表示するアイテムを設定
			If TopItem > 0 Then
				If .lstItems.TopIndex <> TopItem - 1 Then
					.lstItems.TopIndex = MinLng(TopItem, .lstItems.Items.Count) - 1
				End If
			End If
			
			SelectedItem = 0
			
			System.Windows.Forms.Application.DoEvents()
			IsFormClicked = False
			
			'リストボックスを表示
			IsMordal = False
			.Show()
			Do Until IsFormClicked
				System.Windows.Forms.Application.DoEvents()
			Loop 
			frmMultiColumnListBox.Close()
			'UPGRADE_NOTE: オブジェクト frmMultiColumnListBox をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			frmMultiColumnListBox = Nothing
			
			MultiColumnListBox = SelectedItem
		End With
	End Function
	
	'複数のアイテム選択可能なリストボックスを表示
	Public Function MultiSelectListBox(ByVal lb_caption As String, ByRef list() As String, ByVal lb_info As String, ByVal max_num As Short) As Short
		Dim i, j As Short
		
		'ステータスウィンドウに攻撃の命中率などを表示させないようにする
		CommandState = "ユニット選択"
		
		'リストボックスを作成して表示
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmMultiSelectListBox)
		With frmMultiSelectListBox
			.Text = lb_caption
			.lblCaption.Text = "　" & lb_info
			MaxListItem = max_num
			For i = 1 To UBound(list)
				.lstItems.Items.Add("　" & list(i))
			Next 
			.cmdSort.Text = "名称順に並べ替え"
			.Left = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(MainForm.Left))
			.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			.ShowDialog()
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
		frmMultiSelectListBox.Close()
		'UPGRADE_NOTE: オブジェクト frmMultiSelectListBox をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		frmMultiSelectListBox = Nothing
	End Function
	
	
	' === 画像描画に関する処理 ===
	
	'画像をウィンドウに描画
	Public Function DrawPicture(ByRef fname As String, ByVal dx As Integer, ByVal dy As Integer, ByVal dw As Integer, ByVal dh As Integer, ByVal sx As Integer, ByVal sy As Integer, ByVal sw As Integer, ByVal sh As Integer, ByRef draw_option As String) As Boolean
		Dim pic_option, opt, pic_option2 As String
		Dim permanent, transparent As Boolean
		Dim is_monotone, is_sepia As Boolean
		Dim is_sunset, is_water As Boolean
		Dim bright_count, dark_count As Short
		Dim is_sil, negpos As Boolean
		Dim vrev, hrev As Boolean
		Dim top_part, bottom_part As Boolean
		Dim left_part, right_part As Boolean
		Dim tleft_part, tright_part As Boolean
		Dim bleft_part, bright_part As Boolean
		Dim angle As Integer
		Dim on_msg_window, on_status_window As Boolean
		Dim keep_picture As Boolean
		Dim ret As Integer
		Dim i, j As Short
		Dim pfname, fpath As String
		Dim pic, mask_pic As System.Windows.Forms.PictureBox
		Dim stretched_pic, stretched_mask_pic As System.Windows.Forms.PictureBox
		Dim orig_pic As System.Windows.Forms.PictureBox
		Dim orig_width, orig_height As Integer
		Dim found_orig, in_history, load_only As Boolean
		Dim is_colorfilter As Boolean
		Dim fcolor As Integer
		Dim trans_par As Double
		Dim tnum, tdir, tname As String
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
		Static display_byte_pixel As Integer
		Static last_fname As String
		Static last_exists As Boolean
		Static last_path As String
		Static last_angle As Integer
		Static fpath_history As New Collection
		
		'初回実行時に各種情報の初期化を行う
		If Not init_draw_pitcure Then
			'各フォルダにBitmapフォルダがあるかチェック
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ScenarioPath & "Bitmap", FileAttribute.Directory)) > 0 Then
				scenario_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath & "Bitmap", FileAttribute.Directory)) > 0 Then
				extdata_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath2 & "Bitmap", FileAttribute.Directory)) > 0 Then
				extdata2_bitmap_dir_exists = True
			End If
			
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ScenarioPath & "Bitmap\Anime", FileAttribute.Directory)) > 0 Then
				scenario_anime_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath & "Bitmap\Anime", FileAttribute.Directory)) > 0 Then
				extdata_anime_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath2 & "Bitmap\Anime", FileAttribute.Directory)) > 0 Then
				extdata2_anime_bitmap_dir_exists = True
			End If
			
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ScenarioPath & "Bitmap\Event", FileAttribute.Directory)) > 0 Then
				scenario_event_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath & "Bitmap\Event", FileAttribute.Directory)) > 0 Then
				extdata_event_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath2 & "Bitmap\Event", FileAttribute.Directory)) > 0 Then
				extdata2_event_bitmap_dir_exists = True
			End If
			
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ScenarioPath & "Bitmap\Cutin", FileAttribute.Directory)) > 0 Then
				scenario_cutin_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath & "Bitmap\Cutin", FileAttribute.Directory)) > 0 Then
				extdata_cutin_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath2 & "Bitmap\Cutin", FileAttribute.Directory)) > 0 Then
				extdata2_cutin_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(AppPath & "Bitmap\Cutin", FileAttribute.Directory)) > 0 Then
				app_cutin_bitmap_dir_exists = True
			End If
			
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ScenarioPath & "Bitmap\Pilot", FileAttribute.Directory)) > 0 Then
				scenario_pilot_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath & "Bitmap\Pilot", FileAttribute.Directory)) > 0 Then
				extdata_pilot_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath2 & "Bitmap\Pilot", FileAttribute.Directory)) > 0 Then
				extdata2_pilot_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(AppPath & "Bitmap\Pilot", FileAttribute.Directory)) > 0 Then
				app_pilot_bitmap_dir_exists = True
			End If
			
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ScenarioPath & "Bitmap\Unit", FileAttribute.Directory)) > 0 Then
				scenario_unit_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath & "Bitmap\Unit", FileAttribute.Directory)) > 0 Then
				extdata_unit_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath2 & "Bitmap\Unit", FileAttribute.Directory)) > 0 Then
				extdata2_unit_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(AppPath & "Bitmap\Unit", FileAttribute.Directory)) > 0 Then
				app_unit_bitmap_dir_exists = True
			End If
			
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ScenarioPath & "Bitmap\Map", FileAttribute.Directory)) > 0 Then
				scenario_map_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath & "Bitmap\Map", FileAttribute.Directory)) > 0 Then
				extdata_map_bitmap_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath2 & "Bitmap\Map", FileAttribute.Directory)) > 0 Then
				extdata2_map_bitmap_dir_exists = True
			End If
			
			'画面の色数を参照
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
		BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
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
					pic_option2 = pic_option2 & " 右回転=" & VB6.Format(angle Mod 360)
				Case "左回転"
					i = i + 1
					angle = -StrToLng(LIndex(draw_option, i))
					pic_option2 = pic_option2 & " 右回転=" & VB6.Format(angle Mod 360)
				Case "フィルタ"
					is_colorfilter = True
				Case Else
					If Right(opt, 1) = "%" And IsNumeric(Left(opt, Len(opt) - 1)) Then
						trans_par = MaxDbl(0, MinDbl(1, CDbl(Left(opt, Len(opt) - 1)) / 100))
						pic_option2 = pic_option2 & " フィルタ透過度=" & opt
					Else
						If is_colorfilter Then
							fcolor = CInt(opt)
							pic_option2 = pic_option2 & " フィルタ=" & opt
						Else
							BGColor = CInt(opt)
							pic_option2 = pic_option2 & " " & opt
						End If
					End If
			End Select
			i = i + 1
		Loop 
		pic_option = Trim(pic_option)
		pic_option2 = Trim(pic_option2)
		
		'描画先を設定
		If on_msg_window Then
			'メッセージウィンドウへのパイロット画像の描画
			pic = frmMessage.picFace
			permanent = False
		ElseIf on_status_window Then 
			'ステータスウィンドウへのパイロット画像の描画
			'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = MainForm.picUnitStatus
		ElseIf permanent Then 
			'背景への描画
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = MainForm.picBack
		Else
			'マップウィンドウへの通常の描画
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = MainForm.picMain(0)
			SaveScreen()
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
				If PicBufOption(i) = pic_option And PicBufOption2(i) = pic_option2 And Not PicBufIsMask(i) And PicBufDW(i) = dw And PicBufDH(i) = dh And PicBufSX(i) = sx And PicBufSY(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
					'同じファイル、オプションによる画像が見つかった
					
					'以前表示した画像をそのまま利用
					UsePicBuf(i)
					'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					orig_pic = MainForm.picBuf(i)
					With orig_pic
						orig_width = VB6.PixelsToTwipsX(.Width)
						orig_height = VB6.PixelsToTwipsY(.Height)
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
				If PicBufOption(i) = pic_option And PicBufOption2(i) = pic_option2 And Not PicBufIsMask(i) And PicBufDW(i) = DEFAULT_LEVEL And PicBufDH(i) = DEFAULT_LEVEL And PicBufSX(i) = sx And PicBufSY(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
					'同じファイル、オプションによる画像が見つかった
					
					'以前表示した画像をそのまま利用
					UsePicBuf(i)
					'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					orig_pic = MainForm.picBuf(i)
					With orig_pic
						orig_width = VB6.PixelsToTwipsX(.Width)
						orig_height = VB6.PixelsToTwipsY(.Height)
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
					If PicBufOption(i) = "" And PicBufOption2(i) = "" And Not PicBufIsMask(i) And PicBufDW(i) = DEFAULT_LEVEL And PicBufDH(i) = DEFAULT_LEVEL And PicBufSX(i) = sx And PicBufSY(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
						'以前使用した部分画像をそのまま利用
						UsePicBuf(i)
						'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						orig_pic = MainForm.picBuf(i)
						With orig_pic
							orig_width = VB6.PixelsToTwipsX(.Width)
							orig_height = VB6.PixelsToTwipsY(.Height)
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
				If PicBufOption(i) = "" And PicBufOption2(i) = "" And Not PicBufIsMask(i) And PicBufDW(i) = DEFAULT_LEVEL And PicBufDH(i) = DEFAULT_LEVEL And PicBufSW(i) = 0 Then
					'以前使用した原画像をそのまま利用
					UsePicBuf(i)
					'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					orig_pic = MainForm.picBuf(i)
					With orig_pic
						orig_width = VB6.PixelsToTwipsX(.Width)
						orig_height = VB6.PixelsToTwipsY(.Height)
					End With
					'Debug.Print "Reuse " & Format$(i) & " As Orig"
					GoTo LoadedOrigPicture
				End If
			End If
		Next 
		
		'特殊なファイル名
		Select Case LCase(fname)
			Case "black.bmp", "event\black.bmp"
				'黒で塗りつぶし
				With pic
					If dx = DEFAULT_LEVEL Then
						dx = (VB6.PixelsToTwipsX(.Width) - dw) \ 2
					End If
					If dy = DEFAULT_LEVEL Then
						dy = (VB6.PixelsToTwipsY(.Height) - dh) \ 2
					End If
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = PatBlt(.hDC, dx, dy, dw, dh, BLACKNESS)
				End With
				GoTo DrewPicture
			Case "white.bmp", "event\white.bmp"
				'白で塗りつぶし
				With pic
					If dx = DEFAULT_LEVEL Then
						dx = (VB6.PixelsToTwipsX(.Width) - dw) \ 2
					End If
					If dy = DEFAULT_LEVEL Then
						dy = (VB6.PixelsToTwipsY(.Height) - dh) \ 2
					End If
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = PatBlt(.hDC, dx, dy, dw, dh, WHITENESS)
				End With
				GoTo DrewPicture
			Case "common\effect_tile(ally).bmp", "anime\common\effect_tile(ally).bmp"
				'味方ユニットタイル
				'UPGRADE_ISSUE: Control picUnit は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				orig_pic = MainForm.picUnit
				orig_width = 32
				orig_height = 32
				GoTo LoadedOrigPicture
			Case "common\effect_tile(enemy).bmp", "anime\common\effect_tile(enemy).bmp"
				'敵ユニットタイル
				'UPGRADE_ISSUE: Control picEnemy は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				orig_pic = MainForm.picEnemy
				orig_width = 32
				orig_height = 32
				GoTo LoadedOrigPicture
			Case "common\effect_tile(neutral).bmp", "anime\common\effect_tile(neutral).bmp"
				'中立ユニットタイル
				'UPGRADE_ISSUE: Control picNeautral は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				orig_pic = MainForm.picNeautral
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
		'UPGRADE_WARNING: オブジェクト fpath_history.Item() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
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
			If Mid(fname, 2, 1) = ":" Then
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
			
			If LCase(Left(fname, 4)) = "map\" Then
				tname = Mid(fname, 5)
				If InStr(tname, "\") = 0 Then
					i = Len(tname) - 5
					Do While i > 0
						If Mid(tname, i, 1) Like "[!-0-9]" Then
							Exit Do
						End If
						i = i - 1
					Loop 
					If i > 0 Then
						tdir = Left(tname, i) & "\"
						tnum = Mid(tname, i + 1, Len(tname) - i - 4)
						tname = Left(tname, i) & VB6.Format(StrToLng(tnum), "0000") & ".bmp"
					End If
				End If
			End If
		Else
			'地形画像検索用の地形画像ディレクトリ名と4桁ファイル名を作成
			If fname Like "*#.bmp" And Left(fname, 1) Like "[a-z]" Then
				i = Len(fname) - 5
				Do While i > 0
					If Mid(fname, i, 1) Like "[!-0-9]" Then
						Exit Do
					End If
					i = i - 1
				Loop 
				If i > 0 Then
					tdir = Left(fname, i)
					With TDList
						For j = 1 To .Count
							If tdir = .Item(.OrderedID(j)).Bitmap Then
								tnum = Mid(fname, i + 1, Len(fname) - i - 4)
								tname = Left(fname, i) & VB6.Format(StrToLng(tnum), "0000") & ".bmp"
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
		fpath_history.Add("", fname)
		
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
			fpath_history.Add(fpath, fname)
		End If
		
		last_exists = True
		pfname = fpath & fname
		
		'使用するバッファを選択
		i = GetPicBuf()
		'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		orig_pic = MainForm.picBuf(i)
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
		
		LoadPicture2(orig_pic, pfname)
		
		'読み込んだ画像のサイズ(バイト数)をバッファ情報に記録しておく
		With orig_pic
			PicBufSize(i) = display_byte_pixel * VB6.PixelsToTwipsX(.Width) * VB6.PixelsToTwipsY(.Height)
		End With
		
LoadedOrigPicture: 
		
		With orig_pic
			orig_width = VB6.PixelsToTwipsX(.Width)
			orig_height = VB6.PixelsToTwipsY(.Height)
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
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				With MainForm.picBuf(i)
					'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Picture = System.Drawing.Image.FromFile("")
					'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.width = sw
					'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Height = sh
					If sx = DEFAULT_LEVEL Then
						sx = (orig_width - sw) \ 2
					End If
					If sy = DEFAULT_LEVEL Then
						sy = (orig_height - sh) \ 2
					End If
					'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.hDC, 0, 0, sw, sh, orig_pic.hDC, sx, sy, SRCCOPY)
				End With
				
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				orig_pic = MainForm.picBuf(i)
				orig_width = sw
				orig_height = sh
			End If
		End If
		
LoadedPicture: 
		
		'原画像を修正して使う場合は原画像を別のpicBufにコピーして修正する
		If top_part Or bottom_part Or left_part Or right_part Or tleft_part Or tright_part Or bleft_part Or bright_part Or is_monotone Or is_sepia Or is_sunset Or is_water Or negpos Or is_sil Or vrev Or hrev Or bright_count > 0 Or dark_count > 0 Or angle Mod 360 <> 0 Or is_colorfilter Then
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
			'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With MainForm.picBuf(i)
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Picture = System.Drawing.Image.FromFile("")
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.width = orig_width
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Height = orig_height
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.hDC, 0, 0, orig_width, orig_height, orig_pic.hDC, 0, 0, SRCCOPY)
			End With
			'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			orig_pic = MainForm.picBuf(i)
		End If
		
		'画像の一部を塗りつぶして描画する場合
		If top_part Then
			'上半分
			'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			orig_pic.Line (0, orig_height \ 2) - (orig_width - 1, orig_height - 1), BGColor, BF
		End If
		If bottom_part Then
			'下半分
			'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			orig_pic.Line (0, 0) - (orig_width - 1, orig_height \ 2 - 1), BGColor, BF
		End If
		If left_part Then
			'左半分
			'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			orig_pic.Line (orig_width \ 2, 0) - (orig_width - 1, orig_height - 1), BGColor, BF
		End If
		If right_part Then
			'右半分
			'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			orig_pic.Line (0, 0) - (orig_width \ 2 - 1, orig_height - 1), BGColor, BF
		End If
		If tleft_part Then
			'左上
			For i = 0 To orig_width - 1
				'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				orig_pic.Line (i, orig_height - 1 - i) - (i, orig_height - 1), BGColor, B
			Next 
		End If
		If tright_part Then
			'右上
			For i = 0 To orig_width - 1
				'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				orig_pic.Line (i, i) - (i, orig_height - 1), BGColor, B
			Next 
		End If
		If bleft_part Then
			'左下
			For i = 0 To orig_width - 1
				'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				orig_pic.Line (i, 0) - (i, i), BGColor, B
			Next 
		End If
		If bright_part Then
			'右下
			For i = 0 To orig_width - 1
				'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				orig_pic.Line (i, 0) - (i, orig_height - 1 - i), BGColor, B
			Next 
		End If
		
		'特殊効果
		If is_monotone Or is_sepia Or is_sunset Or is_water Or is_colorfilter Or bright_count > 0 Or dark_count > 0 Or negpos Or is_sil Or vrev Or hrev Or angle <> 0 Then
			'画像のサイズをチェック
			If orig_width * orig_height Mod 4 <> 0 Then
				ErrorMessage(fname & "の画像サイズが4の倍数になっていません")
				Exit Function
			End If
			
			'イメージをバッファに取り込み
			GetImage(orig_pic)
			
			'白黒
			If is_monotone Then
				Monotone(transparent)
			End If
			
			'セピア
			If is_sepia Then
				Sepia(transparent)
			End If
			
			'夕焼け
			If is_sunset Then
				Sunset(transparent)
			End If
			
			'水中
			If is_water Then
				Water(transparent)
			End If
			
			'シルエット
			If is_sil Then
				Silhouette()
			End If
			
			'ネガポジ反転
			If negpos Then
				NegPosReverse(transparent)
			End If
			
			'フィルタ
			If is_colorfilter Then
				If trans_par < 0 Then
					trans_par = 0.5
				End If
				ColorFilter(fcolor, trans_par, transparent)
			End If
			
			'明 (多段指定可能)
			For i = 1 To bright_count
				Bright(transparent)
			Next 
			
			'暗 (多段指定可能)
			For i = 1 To dark_count
				Dark(transparent)
			Next 
			
			'左右反転
			If vrev Then
				VReverse()
			End If
			
			'上下反転
			If hrev Then
				HReverse()
			End If
			
			'回転
			If angle <> 0 Then
				'前回の回転角が90度の倍数かどうかで描画の際の最適化使用可否を決める
				'(連続で回転させる場合に描画速度を一定にするため)
				Rotate(angle, last_angle Mod 90 <> 0)
			End If
			
			'変更した内容をイメージに変換
			SetImage(orig_pic)
			
			'バッファを破棄
			ClearImage()
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
			If InStr(fname, "EFFECT_") > 0 Or InStr(fname, "スペシャルパワー\") > 0 Or InStr(fname, "精神コマンド\") > 0 Then
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
			If dx >= VB6.PixelsToTwipsX(.Width) Or dy >= VB6.PixelsToTwipsY(.Height) Or dx + dw <= 0 Or dy + dh <= 0 Or dw <= 0 Or dh <= 0 Then
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
		If Not transparent And dw = orig_width And dh = orig_height Then
			'原画像をそのまま描画
			
			'描画をキャンセル？
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'画像を描画先に描画
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, SRCCOPY)
		ElseIf KeepStretchedImage And Not transparent And (Not found_orig Or load_only) And dw <= 480 And dh <= 480 Then 
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
			'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			stretched_pic = MainForm.picBuf(i)
			With stretched_pic
				.Image = System.Drawing.Image.FromFile("")
				.Width = VB6.TwipsToPixelsX(dw)
				.Height = VB6.TwipsToPixelsY(dh)
			End With
			
			'バッファに拡大した画像を保存
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
			
			'描画をキャンセル？
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'拡大した画像を描画先に描画
			'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCCOPY)
		ElseIf Not transparent Then 
			'拡大画像を作らずにStretchBltで直接拡大描画
			
			'描画をキャンセル？
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'拡大した画像を描画先に描画
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
		ElseIf UseTransparentBlt And (dw <> orig_width Or dh <> orig_height) And found_orig And Not load_only And (dw * dh < 40000 Or orig_width * orig_height > 40000) Then 
			'TransparentBltの方が高速に描画できる場合に限り
			'TransparentBltを使って拡大透過描画
			
			'描画をキャンセル？
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'画像を描画先に透過描画
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = TransparentBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, BGColor)
		ElseIf dw = orig_width And dh = orig_height Then 
			'原画像をそのまま透過描画
			
			'以前使用したマスク画像が利用可能？
			'UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			mask_pic.Image = Nothing
			For i = 0 To ImageBufferSize - 1
				'同じファイル？
				If PicBufFname(i) = fname Then
					'オプションも同じ？
					If PicBufIsMask(i) And PicBufOption2(i) = pic_option2 And PicBufDW(i) = orig_width And PicBufDH(i) = orig_height And PicBufSX(i) = sx And PicBufSX(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
						'以前使用したマスク画像をそのまま利用
						UsePicBuf(i)
						'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						mask_pic = MainForm.picBuf(i)
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
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				mask_pic = MainForm.picBuf(i)
				With mask_pic
					.Image = System.Drawing.Image.FromFile("")
					.Width = VB6.TwipsToPixelsX(orig_width)
					.Height = VB6.TwipsToPixelsY(orig_height)
				End With
				
				'マスク画像を作成
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				MakeMask((orig_pic.hDC), (mask_pic.hDC), orig_width, orig_height, BGColor)
			End If
			
			'描画をキャンセル？
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'画像を透過描画
			If BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
				'背景色が白
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, SRCINVERT)
			Else
				'背景色が白以外
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCAND)
				
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(mask_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCINVERT)
				
				'マスク画像が再利用できないのでバッファを開放
				ReleasePicBuf(i)
			End If
		ElseIf KeepStretchedImage And (Not found_orig Or load_only) And dw <= 480 And dh <= 480 Then 
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
			'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			stretched_pic = MainForm.picBuf(i)
			With stretched_pic
				.Image = System.Drawing.Image.FromFile("")
				.Width = VB6.TwipsToPixelsX(dw)
				.Height = VB6.TwipsToPixelsY(dh)
			End With
			
			'バッファに拡大した画像を保存
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
			
			'以前使用した拡大マスク画像が利用可能？
			'UPGRADE_NOTE: オブジェクト stretched_mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			stretched_mask_pic.Image = Nothing
			For i = 0 To ImageBufferSize - 1
				'同じファイル？
				If PicBufFname(i) = fname Then
					'オプションも同じ？
					If PicBufIsMask(i) And PicBufOption2(i) = pic_option2 And PicBufDW(i) = dw And PicBufDH(i) = dh And PicBufSX(i) = sx And PicBufSY(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
						'以前使用した拡大マスク画像をそのまま利用
						UsePicBuf(i)
						'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						stretched_mask_pic = MainForm.picBuf(i)
						'Debug.Print "Reuse " & Format$(i) & " As StretchedMask"
						Exit For
					End If
				End If
			Next 
			
			If stretched_mask_pic Is Nothing Then
				'拡大マスク画像を新規に作成
				
				'マスク画像用の領域を初期化
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				mask_pic = MainForm.picTmp
				With mask_pic
					.Image = System.Drawing.Image.FromFile("")
					.Width = VB6.TwipsToPixelsX(orig_width)
					.Height = VB6.TwipsToPixelsY(orig_height)
				End With
				
				'マスク画像を作成
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				MakeMask((orig_pic.hDC), (mask_pic.hDC), orig_width, orig_height, BGColor)
				
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
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				stretched_mask_pic = MainForm.picBuf(i)
				With stretched_mask_pic
					.Image = System.Drawing.Image.FromFile("")
					.Width = VB6.TwipsToPixelsX(dw)
					.Height = VB6.TwipsToPixelsY(dh)
				End With
				
				'バッファに拡大したマスク画像を保存
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
			End If
			
			'描画をキャンセル？
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'画像を透過描画
			If BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
				'背景色が白
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCINVERT)
			Else
				'背景色が白以外
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCAND)
				
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, stretched_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCINVERT)
				
				'拡大マスク画像が再利用できないのでバッファを開放
				ReleasePicBuf(i)
			End If
		ElseIf dw <= 480 And dh <= 480 Then 
			'拡大画像を作成した後、バッファリングせずに透過描画
			
			'拡大画像用の領域を作成
			'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			stretched_pic = MainForm.picStretchedTmp(0)
			With stretched_pic
				.Width = VB6.TwipsToPixelsX(dw)
				.Height = VB6.TwipsToPixelsY(dh)
			End With
			
			'バッファに拡大した画像を保存
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
			
			'以前使用したマスク画像が利用可能？
			'UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			mask_pic.Image = Nothing
			For i = 0 To ImageBufferSize - 1
				'同じファイル？
				If PicBufFname(i) = fname Then
					'オプションも同じ？
					If PicBufIsMask(i) And PicBufOption2(i) = pic_option2 And PicBufDW(i) = orig_width And PicBufDH(i) = orig_height And PicBufSX(i) = sx And PicBufSX(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
						'以前使用したマスク画像をそのまま利用
						UsePicBuf(i)
						'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						mask_pic = MainForm.picBuf(i)
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
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				mask_pic = MainForm.picBuf(i)
				With mask_pic
					.Width = VB6.TwipsToPixelsX(orig_width)
					.Height = VB6.TwipsToPixelsY(orig_height)
				End With
				
				'マスク画像を作成
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				MakeMask((orig_pic.hDC), (mask_pic.hDC), orig_width, orig_height, BGColor)
			End If
			
			'拡大マスク画像用の領域を作成
			'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			stretched_mask_pic = MainForm.picStretchedTmp(1)
			With stretched_mask_pic
				.Image = System.Drawing.Image.FromFile("")
				.Width = VB6.TwipsToPixelsX(dw)
				.Height = VB6.TwipsToPixelsY(dh)
			End With
			
			'マスク画像を拡大して拡大マスク画像を作成
			'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
			
			'描画をキャンセル？
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'画像を透過描画
			If BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
				'背景色が白
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCINVERT)
			Else
				'背景色が白以外
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCAND)
				
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, stretched_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCINVERT)
			End If
			
			'使用した一時画像領域を開放
			'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With MainForm.picStretchedTmp(0)
				'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Picture = System.Drawing.Image.FromFile("")
				'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.width = 32
				'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Height = 32
			End With
			'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With MainForm.picStretchedTmp(1)
				'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Picture = System.Drawing.Image.FromFile("")
				'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.width = 32
				'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Height = 32
			End With
		Else
			'拡大画像を作成せず、StretchBltで直接拡大透過描画
			
			'以前使用したマスク画像が利用可能？
			'UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			mask_pic.Image = Nothing
			For i = 0 To ImageBufferSize - 1
				'同じファイル？
				If PicBufFname(i) = fname Then
					'オプションも同じ？
					If PicBufIsMask(i) And PicBufOption2(i) = pic_option2 And PicBufDW(i) = orig_width And PicBufDH(i) = orig_height And PicBufSX(i) = sx And PicBufSX(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
						'以前使用したマスク画像をそのまま利用
						UsePicBuf(i)
						'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						mask_pic = MainForm.picBuf(i)
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
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				mask_pic = MainForm.picBuf(i)
				With mask_pic
					.Width = VB6.TwipsToPixelsX(orig_width)
					.Height = VB6.TwipsToPixelsY(orig_height)
				End With
				
				'マスク画像を作成
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				MakeMask((orig_pic.hDC), (mask_pic.hDC), orig_width, orig_height, BGColor)
			End If
			
			'描画をキャンセル？
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'画像を透過描画
			If BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
				'背景色が白
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCINVERT)
			Else
				'背景色が白以外
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCAND)
				
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(mask_pic.hDC, 0, 0, orig_width, orig_width, orig_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCINVERT)
				
				'マスク画像が再利用できないのでバッファを開放
				ReleasePicBuf(i)
			End If
		End If
		
DrewPicture: 
		
		If permanent Then
			'背景への描き込み
			IsMapDirty = True
			With MainForm
				'マスク入り背景画像画面にも画像を描き込む
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMaskedBack.hDC, dx, dy, dw, dh, pic.hDC, dx, dy, SRCCOPY)
				For i = dx \ 32 To (dx + dw - 1) \ 32
					For j = dy \ 32 To (dy + dh - 1) \ 32
						'UPGRADE_ISSUE: Control picMask は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picMaskedBack.hDC, 32 * i, 32 * j, 32, 32, .picMask.hDC, 0, 0, SRCAND)
						'UPGRADE_ISSUE: Control picMask2 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picMaskedBack.hDC, 32 * i, 32 * j, 32, 32, .picMask2.hDC, 0, 0, SRCINVERT)
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
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(MainForm.picMain(1).hDC, dx, dy, dw, dh, pic.hDC, dx, dy, SRCCOPY)
			End If
		End If
		
		DrawPicture = True
	End Function
	
	'画像バッファを作成
	Public Sub MakePicBuf()
		Dim i As Short
		
		'画像バッファ用のPictureBoxを動的に生成する
		With MainForm
			For i = 1 To ImageBufferSize - 1
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
				Load(.picBuf(i))
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
	Private Function GetPicBuf(Optional ByVal buf_size As Integer = 0) As Short
		Dim total_size As Integer
		Dim oldest_buf, used_buf_num As Short
		Dim i As Short
		Dim tmp As Integer
		
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
			ReleasePicBuf(oldest_buf)
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
		UsePicBuf(GetPicBuf)
	End Function
	
	'画像バッファを開放する
	Private Sub ReleasePicBuf(ByVal idx As Short)
		PicBufFname(idx) = ""
		'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picBuf(idx)
			'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Picture = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.width = 32
			'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Height = 32
		End With
	End Sub
	
	'画像バッファの使用記録をつける
	Private Sub UsePicBuf(ByVal idx As Short)
		PicBufDateCount = PicBufDateCount + 1
		PicBufDate(idx) = PicBufDateCount
	End Sub
	
	
	' === 文字列描画に関する処理 ===
	
	'メインウィンドウに文字列を表示する
	Public Sub DrawString(ByRef msg As String, ByVal X As Integer, ByVal Y As Integer, Optional ByVal without_cr As Boolean = False)
		Dim tx, ty As Short
		Dim prev_cx As Short
		Dim pic As System.Windows.Forms.PictureBox
		Dim sf As System.Drawing.Font
		Static font_smoothing As Integer
		Static init_draw_string As Boolean
		
		If PermanentStringMode Then
			'背景書き込み
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = MainForm.picBack
			'フォント設定を変更
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With MainForm.picBack
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.ForeColor = MainForm.picMain(0).ForeColor
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				If .Font.Name <> MainForm.picMain(0).Font.Name Then
					sf = System.Windows.Forms.Control.DefaultFont.Clone()
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					sf = VB6.FontChangeName(sf, MainForm.picMain(0).Font.Name)
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Font = sf
				End If
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Font.Size = MainForm.picMain(0).Font.Size
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Font.Bold = MainForm.picMain(0).Font.Bold
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Font.Italic = MainForm.picMain(0).Font.Italic
			End With
			'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With MainForm.picMaskedBack
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.ForeColor = MainForm.picMain(0).ForeColor
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				If .Font.Name <> MainForm.picMain(0).Font.Name Then
					sf = System.Windows.Forms.Control.DefaultFont.Clone()
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					sf = VB6.FontChangeName(sf, MainForm.picMain(0).Font.Name)
					'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Font = sf
				End If
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Font.Size = MainForm.picMain(0).Font.Size
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Font.Bold = MainForm.picMain(0).Font.Bold
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Font.Italic = MainForm.picMain(0).Font.Italic
			End With
		Else
			'通常の書き込み
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = MainForm.picMain(0)
			SaveScreen()
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
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			prev_cx = .CurrentX
			
			'書き込み先の座標を求める
			If HCentering Then
				'UPGRADE_ISSUE: PictureBox メソッド pic.TextWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.CurrentX = (VB6.PixelsToTwipsX(.Width) - .TextWidth(msg)) \ 2
			Else
				If X <> DEFAULT_LEVEL Then
					'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.CurrentX = X
				End If
			End If
			If VCentering Then
				'UPGRADE_ISSUE: PictureBox メソッド pic.TextHeight はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.CurrentY = (VB6.PixelsToTwipsY(.Height) - .TextHeight(msg)) \ 2
			Else
				If Y <> DEFAULT_LEVEL Then
					'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.CurrentY = Y
				End If
			End If
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			tx = .CurrentX
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ty = .CurrentY
			
			If Not without_cr Then
				'改行あり
				'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				pic.Print(msg)
				
				'背景書き込みの場合
				If PermanentStringMode Then
					'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					With MainForm.picMaskedBack
						'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.CurrentX = tx
						'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.CurrentY = ty
					End With
					'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.picMaskedBack.Print(msg)
					IsMapDirty = True
				End If
				
				'保持オプション使用時
				If KeepStringMode Then
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					With MainForm.picMain(1)
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.CurrentX = tx
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.CurrentY = ty
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.ForeColor = System.Drawing.ColorTranslator.ToOle(pic.ForeColor)
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						If .Font.Name <> pic.Font.Name Then
							sf = System.Windows.Forms.Control.DefaultFont.Clone()
							sf = VB6.FontChangeName(sf, pic.Font.Name)
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.Font = sf
						End If
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.Font.Size = pic.Font.SizeInPoints
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.Font.Bold = pic.Font.Bold
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.Font.Italic = pic.Font.Italic
					End With
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.picMain(1).Print(msg)
				End If
				
				'次回の書き込みのため、X座標位置を設定し直す
				If X <> DEFAULT_LEVEL Then
					'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.CurrentX = X
				Else
					'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.CurrentX = prev_cx
				End If
			Else
				'改行なし
				'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				pic.Print(msg)
				
				'背景書き込みの場合
				If PermanentStringMode Then
					'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					With MainForm.picMaskedBack
						'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.CurrentX = tx
						'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.CurrentY = ty
					End With
					'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.picMaskedBack.Print(msg)
					IsMapDirty = True
				End If
				
				'保持オプション使用時
				If KeepStringMode Then
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					With MainForm.picMain(1)
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.CurrentX = tx
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.CurrentY = ty
					End With
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					MainForm.picMain(1).Print(msg)
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
	Public Sub DrawSysString(ByVal X As Short, ByVal Y As Short, ByRef msg As String, Optional ByVal without_refresh As Boolean = False)
		Dim prev_color As Integer
		Dim prev_name As String
		Dim prev_size As Short
		Dim is_bold As Boolean
		Dim is_italic As Boolean
		Dim sf As System.Drawing.Font
		
		'表示位置が画面外？
		If X < MapX - MainWidth \ 2 Or MapX + MainWidth \ 2 < X Or Y < MapY - MainHeight \ 2 Or MapY + MainHeight \ 2 < Y Then
			Exit Sub
		End If
		
		SaveScreen()
		
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMain(0)
			'現在のフォント設定を保存
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			prev_color = .ForeColor
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			prev_size = .Font.Size
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			prev_name = .Font.Name
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			is_bold = .Font.Bold
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			is_italic = .Font.Italic
			
			'フォント設定をシステム用に切り替え
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.ForeColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.FontTransparent = False
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			If .Font.Name <> "ＭＳ Ｐ明朝" Then
				sf = System.Windows.Forms.Control.DefaultFont.Clone()
				sf = VB6.FontChangeName(sf, "ＭＳ Ｐ明朝")
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Font = sf
			End If
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With .Font
				If BattleAnimation Then
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Size = 9
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Bold = True
				Else
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Size = 8
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Bold = False
				End If
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Italic = False
			End With
			
			'メッセージの書き込み
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.CurrentX = MapToPixelX(X) + (32 - .TextWidth(msg)) \ 2 - 1
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.CurrentY = MapToPixelY(Y + 1) - .TextHeight(msg)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Print(msg)
			
			'フォント設定を元に戻す
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.ForeColor = prev_color
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.FontTransparent = True
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			If .Font.Name <> prev_name Then
				sf = System.Windows.Forms.Control.DefaultFont.Clone()
				sf = VB6.FontChangeName(sf, prev_name)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Font = sf
			End If
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With .Font
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Size = prev_size
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Bold = is_bold
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Italic = is_italic
			End With
			
			'表示を更新
			If Not without_refresh Then
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Refresh()
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
		Dim ret As Integer
		
		If Not ScreenIsSaved Then
			'画像をpicMain(1)に保存
			With MainForm
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(1).hDC, 0, 0, MainPWidth, MainPHeight, .picMain(0).hDC, 0, 0, SRCCOPY)
			End With
			ScreenIsSaved = True
		End If
	End Sub
	
	'描画したグラフィックを消去
	Public Sub ClearPicture()
		Dim pawidth, paheight As Short
		Dim ret As Integer
		
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
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picMain(0).hDC, PaintedAreaX1, PaintedAreaY1, pawidth, paheight, .picMain(1).hDC, PaintedAreaX1, PaintedAreaY1, SRCCOPY)
		End With
	End Sub
	
	'描画したグラフィックの一部を消去
	Public Sub ClearPicture2(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer)
		Dim ret As Integer
		
		If Not ScreenIsSaved Then
			Exit Sub
		End If
		
		With MainForm
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picMain(0).hDC, x1, y1, x2 - x1 + 1, y2 - y1 + 1, .picMain(1).hDC, x1, y1, SRCCOPY)
		End With
	End Sub
	
	
	' === 画面ロックに関する処理 ===
	
	'ＧＵＩをロックし、プレイヤーからの入力を無効にする
	Public Sub LockGUI()
		IsGUILocked = True
		With MainForm
			'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.VScroll.Enabled = False
			'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.HScroll.Enabled = False
		End With
	End Sub
	
	'ＧＵＩのロックを解除し、プレイヤーからの入力を有効にする
	Public Sub UnlockGUI()
		IsGUILocked = False
		With MainForm
			'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.VScroll.Enabled = True
			'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.HScroll.Enabled = True
		End With
	End Sub
	
	
	' === マウスカーソルの自動移動に関する処理 ===
	
	'現在のマウスカーソルの位置を記録
	Public Sub SaveCursorPos()
		Dim PT As POINTAPI
		
		GetCursorPos(PT)
		PrevCursorX = PT.X
		PrevCursorY = PT.Y
		NewCursorX = 0
		NewCursorY = 0
	End Sub
	
	'マウスカーソルを移動する
	Public Sub MoveCursorPos(ByRef cursor_mode As String, Optional ByVal t As Unit = Nothing)
		Dim i, tx, ty, num As Integer
		Dim ret As Integer
		Dim prev_lock As Boolean
		Dim PT As POINTAPI
		
		'マウスカーソルの位置を収得
		GetCursorPos(PT)
		
		'現在の位置を記録しておく
		If PrevCursorX = 0 And cursor_mode <> "メッセージウィンドウ" Then
			SaveCursorPos()
		End If
		
		'カーソル自動移動
		If t Is Nothing Then
			If cursor_mode = "メッセージウィンドウ" Then
				'メッセージウィンドウまで移動
				With frmMessage
					If PT.X < (VB6.PixelsToTwipsX(.Left) + 0.05 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX Then
						tx = (VB6.PixelsToTwipsX(.Left) + 0.05 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX
					ElseIf PT.X > (VB6.PixelsToTwipsX(.Left) + 0.95 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX Then 
						tx = (VB6.PixelsToTwipsX(.Left) + 0.95 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX
					Else
						tx = PT.X
					End If
					
					If PT.Y < (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY - .ClientRectangle.Height + .picMessage.Top Then
						ty = (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY - .ClientRectangle.Height + .picMessage.Top
					ElseIf PT.Y > (VB6.PixelsToTwipsY(.Top) + 0.9 * VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then 
						ty = (VB6.PixelsToTwipsY(.Top) + 0.9 * VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY
					Else
						ty = PT.Y
					End If
				End With
			Else
				'リストボックスまで移動
				With frmListBox
					If PT.X < (VB6.PixelsToTwipsX(.Left) + 0.1 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX Then
						tx = (VB6.PixelsToTwipsX(.Left) + 0.1 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX
					ElseIf PT.X > (VB6.PixelsToTwipsX(.Left) + 0.9 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX Then 
						tx = (VB6.PixelsToTwipsX(.Left) + 0.9 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX
					Else
						tx = PT.X
					End If
					
					'選択するアイテム
					If cursor_mode = "武器選択" Then
						'武器選択の場合は選択可能な最後のアイテムに
						i = .lstItems.Items.Count
						Do 
							If Not ListItemFlag(i) And InStr(VB6.GetItemString(.lstItems, i), "援護攻撃：") = 0 Then
								Exit Do
							End If
							i = i - 1
						Loop While i > 1
					Else
						'そうでなければ最初のアイテムに
						i = .lstItems.TopIndex + 1
					End If
					
					ty = VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY + VB6.PixelsToTwipsY(.Height) \ VB6.TwipsPerPixelY - .ClientRectangle.Height + .lstItems.Top + 16 * (i - .lstItems.TopIndex) - 8
				End With
			End If
		Else
			'ユニット上まで移動
			With MainForm
				'MOD START 240a
				'            If MainWidth = 15 Then
				'                tx = .Left \ Screen.TwipsPerPixelX _
				''                    + 32 * (t.X - (MapX - MainWidth \ 2)) + 24
				'                ty = .Top \ Screen.TwipsPerPixelY _
				''                    + .Height \ Screen.TwipsPerPixelY - .ScaleHeight _
				''                    + 32 * (t.Y - (MapY - MainHeight \ 2)) + 20
				'            Else
				'                tx = .Left \ Screen.TwipsPerPixelX _
				''                    + 32 * (t.X - (MapX - MainWidth \ 2)) - 14
				'                ty = .Top \ Screen.TwipsPerPixelY _
				''                    + .Height \ Screen.TwipsPerPixelY - .ScaleHeight _
				''                    + 32 * (t.Y - (MapY - MainHeight \ 2)) + 16
				'            End If
				If NewGUIMode Then
					tx = VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX + 32 * (t.X - (MapX - MainWidth \ 2)) + 4
					ty = VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY + VB6.PixelsToTwipsY(.Height) \ VB6.TwipsPerPixelY - VB6.PixelsToTwipsY(.ClientRectangle.Height) + 32 * (t.Y - (MapY - MainHeight \ 2)) + 16
				Else
					tx = VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX + 32 * (t.X - (MapX - MainWidth \ 2)) + 24
					ty = VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY + VB6.PixelsToTwipsY(.Height) \ VB6.TwipsPerPixelY - VB6.PixelsToTwipsY(.ClientRectangle.Height) + 32 * (t.Y - (MapY - MainHeight \ 2)) + 20
				End If
				'MOD  END  240a
			End With
		End If
		
		'何回に分けて移動するか計算
		num = System.Math.Sqrt((tx - PT.X) ^ 2 + (ty - PT.Y) ^ 2) \ 25 + 1
		
		'カーソルを移動
		prev_lock = IsGUILocked
		IsGUILocked = True
		IsStatusWindowDisabled = True
		For i = 1 To num
			ret = SetCursorPos((tx * i + PT.X * (num - i)) \ num, (ty * i + PT.Y * (num - i)) \ num)
			Sleep(10)
		Next 
		System.Windows.Forms.Application.DoEvents()
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
		Dim i, tx, ty, num As Short
		Dim ret As Integer
		Dim PT As POINTAPI
		
		'ユニットが選択されていればその場所まで戻す
		If Not SelectedUnit Is Nothing Then
			If SelectedUnit.Status = "出撃" Then
				MoveCursorPos("ユニット選択", SelectedUnit)
				Exit Sub
			End If
		End If
		
		'戻るべき位置が設定されていない？
		If PrevCursorX = 0 And PrevCursorY = 0 Then
			Exit Sub
		End If
		
		'現在のカーソル位置収得
		GetCursorPos(PT)
		
		'以前の位置までカーソル自動移動
		With frmListBox
			tx = PrevCursorX
			ty = PrevCursorY
			
			num = System.Math.Sqrt((tx - PT.X) ^ 2 + (ty - PT.Y) ^ 2) \ 50 + 1
			
			For i = 1 To num
				ret = SetCursorPos((tx * i + PT.X * (num - i)) \ num, (ty * i + PT.Y * (num - i)) \ num)
				System.Windows.Forms.Application.DoEvents()
				Sleep(10)
			Next 
		End With
		
		'戻り位置を初期化
		PrevCursorX = 0
		PrevCursorY = 0
	End Sub
	
	
	' === タイトル画面表示に関する処理 ===
	
	'タイトル画面を表示
	Public Sub OpenTitleForm()
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmTitle)
		
		frmTitle.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(frmTitle.Width)) / 2)
		frmTitle.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(frmTitle.Height)) / 2)
		
		frmTitle.Show()
		frmTitle.Refresh()
	End Sub
	
	'タイトル画面を閉じる
	Public Sub CloseTitleForm()
		frmTitle.Close()
		'UPGRADE_NOTE: オブジェクト frmTitle をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		frmTitle = Nothing
	End Sub
	
	
	' === 「Now Loading...」表示に関する処理 ===
	
	'「Now Loading...」の画面を表示
	Public Sub OpenNowLoadingForm()
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmNowLoading)
		With frmNowLoading
			.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
			.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			.Show()
			.Label1.Refresh()
		End With
	End Sub
	
	'「Now Loading...」の画面を消去
	Public Sub CloseNowLoadingForm()
		frmNowLoading.Close()
		'UPGRADE_NOTE: オブジェクト frmNowLoading をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		frmNowLoading = Nothing
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
	End Sub
	
	'「Now Loading...」のバーを１段階進行させる
	Public Sub DisplayLoadingProgress()
		frmNowLoading.Progress()
		System.Windows.Forms.Application.DoEvents()
	End Sub
	
	'「Now Loading...」のバーの長さを設定
	Public Sub SetLoadImageSize(ByVal new_size As Short)
		With frmNowLoading
			.Value = 0
			.Size_Renamed = new_size
		End With
	End Sub
	
	
	' === 画面の解像度変更 ===
	
	Public Sub ChangeDisplaySize(ByVal w As Short, ByVal h As Short)
		Dim dm As DEVMODE
		Dim ret As Integer
		Static orig_width, orig_height As Short
		
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
			ret = ChangeDisplaySettings(VariantType.Null, 0)
			Exit Sub
		End If
		
		'解像度を変更可能かどうか調べる
		'UPGRADE_WARNING: オブジェクト dm の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		ret = ChangeDisplaySettings(dm, CDS_TEST)
		If ret <> DISP_CHANGE_SUCCESSFUL Then
			Exit Sub
		End If
		
		'解像度を実際に変更する
		' MOD START MARGE
		'    If GetWinVersion() >= 5 Then
		If GetWinVersion() >= 501 Then
			' MOD END MARGE
			'UPGRADE_WARNING: オブジェクト dm の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			ret = ChangeDisplaySettings(dm, CDS_FULLSCREEN)
		Else
			'UPGRADE_WARNING: オブジェクト dm の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			ret = ChangeDisplaySettings(dm, 0)
		End If
		Select Case ret
			Case DISP_CHANGE_SUCCESSFUL
				'成功！
				Exit Sub
			Case DISP_CHANGE_RESTART
				'再起動が必要な場合はあきらめてもとの解像度に戻す
				ret = ChangeDisplaySettings(VariantType.Null, 0)
		End Select
	End Sub
	
	
	' === その他 ===
	
	'エラーメッセージを表示
	Public Sub ErrorMessage(ByRef msg As String)
		Dim ret As Integer
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmErrorMessage)
		
		With frmErrorMessage
			ret = SetWindowPos(.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
			.txtMessage.Text = msg
			.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
			.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			.Show()
		End With
		
		'メインウィンドウのクローズが行えるようにモーダルモードは使用しない
		Do While frmErrorMessage.Visible
			System.Windows.Forms.Application.DoEvents()
			Sleep(200)
		Loop 
		
		frmErrorMessage.Close()
		'UPGRADE_NOTE: オブジェクト frmErrorMessage をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		frmErrorMessage = Nothing
	End Sub
	
	'データ読み込み時のエラーメッセージを表示する
	Public Sub DataErrorMessage(ByRef msg As String, ByRef fname As String, ByVal line_num As Short, ByRef line_buf As String, ByRef dname As String)
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
		ErrorMessage(err_msg)
	End Sub
	
	
	'マウスの右ボタンが押されているか(キャンセル)判定
	Public Function IsRButtonPressed(Optional ByVal ignore_message_wait As Boolean = False) As Boolean
		Dim PT As POINTAPI
		
		'メッセージがウエイト無しならスキップ
		If Not ignore_message_wait And MessageWait = 0 Then
			IsRButtonPressed = True
			Exit Function
		End If
		
		'メインウインドウ上でマウスボタンを押した場合
		If MainForm.Handle.ToInt32 = GetForegroundWindow Then
			GetCursorPos(PT)
			With MainForm
				If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
					If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
						'右ボタンでスキップ
						IsRButtonPressed = True
						Exit Function
					End If
				End If
			End With
			'メッセージウインドウ上でマウスボタンを押した場合
		ElseIf frmMessage.Handle.ToInt32 = GetForegroundWindow Then 
			GetCursorPos(PT)
			With frmMessage
				If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
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
	Public Sub DisplayTelop(ByRef msg As String)
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmTelop)
		
		With frmTelop
			FormatMessage(msg)
			If InStr(msg, ".") > 0 Then
				Mid(msg, InStr(msg, ".")) = vbCr
				.Height = VB6.TwipsToPixelsY(1170)
			Else
				.Height = VB6.TwipsToPixelsY(800)
			End If
			
			If MainForm.Visible = True And Not MainForm.WindowState = 1 Then
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Left = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(MainForm.Left) + (MainForm.picMain(0).width * VB6.PixelsToTwipsX(MainForm.Width) \ VB6.PixelsToTwipsX(MainForm.ClientRectangle.Width) - VB6.PixelsToTwipsX(.Width)) \ 2)
				.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(MainForm.Top) + (VB6.PixelsToTwipsY(MainForm.Height) - VB6.PixelsToTwipsY(.Height)) \ 2)
			Else
				.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
				.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			End If
			
			If InStr(msg, ".") > 0 Then
				Mid(msg, InStr(msg, ".")) = vbCr
			End If
			.Label1.Text = msg
			.Show()
			.Refresh()
		End With
		
		If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
			Sleep(1000)
		End If
		frmTelop.Close()
	End Sub
End Module