Option Strict Off
Option Explicit On
Module GUI
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Main縺ｮForm
	Public MainForm As System.Windows.Forms.Form
	Public IsFlashAvailable As Boolean
	
	' ADD START MARGE
	'Invalid_string_refer_to_original_code
	Public NewGUIMode As Boolean
	' ADD END
	
	'Invalid_string_refer_to_original_code
	Public MainWidth As Short
	Public MainHeight As Short
	
	'Invalid_string_refer_to_original_code
	Public MainPWidth As Short
	Public MainPHeight As Short
	
	'Invalid_string_refer_to_original_code
	Public MapPWidth As Short
	Public MapPHeight As Short
	
	'Invalid_string_refer_to_original_code
	Public Const GauageWidth As Short = 88
	
	'Invalid_string_refer_to_original_code
	Public ScreenIsMasked As Boolean
	'Invalid_string_refer_to_original_code
	Public ScreenIsSaved As Boolean
	
	'Invalid_string_refer_to_original_code
	Public MapX As Short
	Public MapY As Short
	
	'Invalid_string_refer_to_original_code
	Public PrevMapX As Short
	Public PrevMapY As Short
	
	'Invalid_string_refer_to_original_code
	Public MouseButton As Short
	
	'Invalid_string_refer_to_original_code
	Public MouseX As Single
	Public MouseY As Single
	
	'Invalid_string_refer_to_original_code
	Public PrevMouseX As Single
	Public PrevMouseY As Single
	
	'Invalid_string_refer_to_original_code
	Private PrevCursorX As Short
	Private PrevCursorY As Short
	'Invalid_string_refer_to_original_code
	Private NewCursorX As Short
	Private NewCursorY As Short
	
	'Invalid_string_refer_to_original_code
	Public PrevUnitX As Short
	Public PrevUnitY As Short
	Public PrevUnitArea As String
	Public PrevCommand As String
	
	'Invalid_string_refer_to_original_code
	Public IsPictureDrawn As Boolean
	'Invalid_string_refer_to_original_code
	Public IsPictureVisible As Boolean
	'PaintPicture縺ｧ謠冗判縺励◆逕ｻ蜒城伜沺
	Public PaintedAreaX1 As Short
	Public PaintedAreaY1 As Short
	Public PaintedAreaX2 As Short
	Public PaintedAreaY2 As Short
	'Invalid_string_refer_to_original_code
	Public IsCursorVisible As Boolean
	'閭梧勹濶ｲ
	Public BGColor As Integer
	
	'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
	Public IsGUILocked As Boolean
	
	'Invalid_string_refer_to_original_code
	Public TopItem As Short
	
	'Invalid_string_refer_to_original_code
	Private DisplayedPilot As String
	Private DisplayMode As String
	Private RightUnit As Unit
	Private LeftUnit As Unit
	Private RightUnitHPRatio As Double
	Private LeftUnitHPRatio As Double
	Private RightUnitENRatio As Double
	Private LeftUnitENRatio As Double
	Public MessageWindowIsOut As Boolean
	
	'Invalid_string_refer_to_original_code
	Private IsMessageFormVisible As Boolean
	Private SavedLeftUnit As Unit
	Private SavedRightUnit As Unit
	
	'Invalid_string_refer_to_original_code
	Public IsFormClicked As Boolean
	'Invalid_string_refer_to_original_code
	Public IsMordal As Boolean
	
	'Invalid_string_refer_to_original_code
	Public MessageWait As Integer
	
	'Invalid_string_refer_to_original_code
	Public AutoMessageMode As Boolean
	
	'Invalid_string_refer_to_original_code
	Public HCentering As Boolean
	Public VCentering As Boolean
	'Invalid_string_refer_to_original_code
	Public PermanentStringMode As Boolean
	'Invalid_string_refer_to_original_code
	Public KeepStringMode As Boolean
	
	
	'ListBox逕ｨ螟画焚
	Public ListItemFlag() As Boolean
	Public ListItemComment() As String
	Public ListItemID() As String
	Public MaxListItem As Short
	
	
	'API髢｢謨ｰ縺ｮ螳夂ｾｩ
	
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
	
	'StretchBlt縺ｮ繝｢繝ｼ繝芽ｨｭ螳壹ｒ陦後≧
	Declare Function GetStretchBltMode Lib "gdi32" (ByVal hDC As Integer) As Integer
	Declare Function SetStretchBltMode Lib "gdi32" (ByVal hDC As Integer, ByVal nStretchMode As Integer) As Integer
	
	Public Const STRETCH_ANDSCANS As Short = 1
	Public Const STRETCH_ORSCANS As Short = 2
	Public Const STRETCH_DELETESCANS As Short = 3
	Public Const STRETCH_HALFTONE As Short = 4
	
	'騾城℃謠冗判
	Declare Function TransparentBlt Lib "msimg32.dll" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal nWidth As Integer, ByVal nHeight As Integer, ByVal hSrcDC As Integer, ByVal xsrc As Integer, ByVal ysrc As Integer, ByVal nSrcWidth As Integer, ByVal nSrcHeight As Integer, ByVal crTransparent As Integer) As Integer
	
	'Invalid_string_refer_to_original_code
	Declare Function SetWindowPos Lib "user32" (ByVal hwnd As Integer, ByVal hWndInsertAfter As Integer, ByVal X As Integer, ByVal Y As Integer, ByVal cx As Integer, ByVal cy As Integer, ByVal wFlags As Integer) As Integer
	
	Public Const SW_SHOWNA As Short = 8 'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Declare Function ShowWindow Lib "user32" (ByVal hwnd As Integer, ByVal nCmdShow As Integer) As Integer
	
	Declare Sub Sleep Lib "kernel32" (ByVal dwMilliseconds As Integer)
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_WARNING: 構造体 POINTAPI に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function GetCursorPos Lib "user32" (ByRef lpPoint As POINTAPI) As Integer
	
	'Invalid_string_refer_to_original_code
	Structure POINTAPI
		Dim X As Integer
		Dim Y As Integer
	End Structure
	
	'Invalid_string_refer_to_original_code
	Declare Function SetCursorPos Lib "user32" (ByVal X As Integer, ByVal Y As Integer) As Integer
	
	'Invalid_string_refer_to_original_code
	Declare Function GetAsyncKeyState Lib "user32" (ByVal vKey As Integer) As Short
	
	Public RButtonID As Integer
	Public LButtonID As Integer
	
	'Invalid_string_refer_to_original_code
	Declare Function GetSystemMetrics Lib "user32" (ByVal nIndex As Integer) As Integer
	
	Public Const SM_SWAPBUTTON As Short = 23 'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Declare Function GetForegroundWindow Lib "user32" () As Integer
	
	'逶ｴ邱壹ｒ謠冗判縺吶ｋ縺溘ａ縺ｮAPI
	'UPGRADE_WARNING: 構造体 POINTAPI に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Declare Function MoveToEx Lib "gdi32" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer, ByRef lpPoint As POINTAPI) As Integer
	Declare Function LineTo Lib "gdi32" (ByVal hDC As Integer, ByVal X As Integer, ByVal Y As Integer) As Integer
	
	'螟夊ｧ貞ｽ｢繧呈緒逕ｻ縺吶ｋAPI
	'UPGRADE_WARNING: 構造体 POINTAPI に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Public Declare Function Polygon Lib "gdi32.dll" (ByVal hDC As Integer, ByRef lpPoint As POINTAPI, ByVal nCount As Integer) As Integer
	
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_ISSUE: パラメータ 'As Any' の宣言はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FAE78A8D-8978-4FD4-8208-5B7324A8F795"' をクリックしてください。
	Public Declare Function ChangeDisplaySettings Lib "user32.dll"  Alias "ChangeDisplaySettingsA"(ByRef lpDevMode As Any, ByVal dwFlags As Integer) As Integer
	
	Public Const CDS_UPDATEREGISTRY As Integer = &H1
	Public Const CDS_TEST As Integer = &H2
	Public Const CDS_FULLSCREEN As Integer = &H4
	Public Const DISP_CHANGE_SUCCESSFUL As Short = 0
	Public Const DISP_CHANGE_RESTART As Short = 1
	
	'Invalid_string_refer_to_original_code
	Public Declare Function GetDeviceCaps Lib "gdi32" (ByVal hDC As Integer, ByVal nIndex As Integer) As Integer
	
	'Invalid_string_refer_to_original_code
	Private Const BITSPIXEL As Short = 12
	
	
	'Invalid_string_refer_to_original_code
	Declare Function SetSystemParametersInfo Lib "user32.dll"  Alias "SystemParametersInfoA"(ByVal uiAction As Integer, ByVal uiParam As Integer, ByVal pvParam As Integer, ByVal fWinIni As Integer) As Integer
	
	Declare Function GetSystemParametersInfo Lib "user32.dll"  Alias "SystemParametersInfoA"(ByVal uiAction As Integer, ByVal uiParam As Integer, ByRef pvParam As Integer, ByVal fWinIni As Integer) As Integer
	
	'Invalid_string_refer_to_original_code
	Public Const SPI_GETFONTSMOOTHING As Short = 74
	Public Const SPI_SETFONTSMOOTHING As Short = 75
	
	'Invalid_string_refer_to_original_code
	Public Const SPIF_UPDATEINIFILE As Integer = &H1
	'縺吶∋縺ｦ縺ｮ繝医ャ繝励Ξ繝吶Ν繧ｦ繧｣繝ｳ繝峨え縺ｫ螟画峩繧帝夂衍
	Public Const SPIF_SENDWININICHANGE As Integer = &H2
	
	
	'繝｡繧､繝ｳ繧ｦ繧｣繝ｳ繝峨え縺ｮ繝ｭ繝ｼ繝峨→Flash縺ｮ逋ｻ骭ｲ繧定｡後≧
	Public Sub LoadMainFormAndRegisterFlash()
		Dim WSHShell As Object
		
		On Error GoTo ErrorHandler
		
		'繧ｷ繧ｧ繝ｫ縺九ｉregsvr32.exe繧貞茜逕ｨ縺励※縲∬ｵｷ蜍輔＃縺ｨ縺ｫSRC.exe縺ｨ蜷後§繝代せ縺ｫ縺ゅｋ
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmSafeMain)
		MainForm = frmSafeMain
	End Sub
	
	'Invalid_string_refer_to_original_code
	'縺溘□縺励Γ繧､繝ｳ繧ｦ繧｣繝ｳ繝峨え縺ｯ縺ゅｉ縺九§繧´oadMainFormAndRegisterFlash縺ｧ繝ｭ繝ｼ繝峨＠縺ｦ縺翫￥縺薙→
	Public Sub LoadForms()
		Dim X, Y As Short
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmToolTip)
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmMessage)
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmListBox)
		
		LockGUI()
		CommandState = "Invalid_string_refer_to_original_code"
		
		'Invalid_string_refer_to_original_code
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
		'Invalid_string_refer_to_original_code
		If IsOptionDefined("Invalid_string_refer_to_original_code") Then
			NewGUIMode = True
			MainWidth = 20
		End If
		' ADD END MARGE
		MainHeight = 15
		
		'Invalid_string_refer_to_original_code
		MainPWidth = MainWidth * 32
		MainPHeight = MainHeight * 32
		
		With MainForm
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
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
	'Invalid_string_refer_to_original_code
	Public Sub SetNewGUIMode()
		'Invalid_string_refer_to_original_code
		If IsOptionDefined("Invalid_string_refer_to_original_code") And Not NewGUIMode Then
			LoadForms()
		End If
	End Sub
	' ADD  END  MARGE
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
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
			'Invalid_string_refer_to_original_code
			If Not u1 Is Nothing Then
				If .Text = "Invalid_string_refer_to_original_code" Then
					.Text = "Invalid_string_refer_to_original_code"
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If .WindowState <> System.Windows.Forms.FormWindowState.Normal Then
				.WindowState = System.Windows.Forms.FormWindowState.Normal
				.Show()
				.Activate()
			End If
			
			If u1 Is Nothing Then
				'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				If u1.Party = "蜻ｳ譁ｹ" Or u1.Party = "Invalid_string_refer_to_original_code" Then
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
				'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If MainForm.Visible And Not MainForm.WindowState = 1 Then
				'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
				.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			End If
			
			'繧ｦ繧｣繝ｳ繝峨え繧偵け繝ｪ繧｢縺励※縺翫￥
			.picFace.Image = System.Drawing.Image.FromFile("")
			DisplayedPilot = ""
			'UPGRADE_ISSUE: PictureBox メソッド picMessage.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.picMessage.Cls()
			
			'繧ｦ繧｣繝ｳ繝峨え繧定｡ｨ遉ｺ
			.Show()
			
			'蟶ｸ縺ｫ謇句燕縺ｫ陦ｨ遉ｺ縺吶ｋ
			ret = SetWindowPos(.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
		End With
		
		System.Windows.Forms.Application.DoEvents()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub CloseMessageForm()
		If Not frmMessage.Visible Then
			Exit Sub
		End If
		frmMessage.Hide()
		System.Windows.Forms.Application.DoEvents()
	End Sub
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Sub UpdateMessageForm(ByRef u1 As Unit, Optional ByRef u2 As Object = Nothing)
		Dim lu, ru As Unit
		Dim ret As Integer
		Dim i As Short
		Dim buf As String
		Dim num As Short
		Dim tmp As Integer
		
		With frmMessage
			'Invalid_string_refer_to_original_code
			If .Visible Then
				If Not .picUnit1.Visible And Not .picUnit2.Visible Then
					Exit Sub
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: IsMissing() は IsNothing() に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="8AE1CB93-37AB-439A-A4FF-BE3B6760BB23"' をクリックしてください。
			If IsNothing(u2) Then
				'Invalid_string_refer_to_original_code
				If u1.Party = "蜻ｳ譁ｹ" Or u1.Party = "Invalid_string_refer_to_original_code" Then
					'UPGRADE_NOTE: オブジェクト lu をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					lu = Nothing
					ru = u1
				Else
					lu = u1
					'UPGRADE_NOTE: オブジェクト ru をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					ru = Nothing
				End If
			ElseIf u2 Is Nothing Then 
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				lu = LeftUnit
				ru = RightUnit
			ElseIf (u2 Is LeftUnit Or u1 Is RightUnit) And Not LeftUnit Is RightUnit Then 
				lu = u2
				ru = u1
			Else
				lu = u1
				ru = u2
			End If
			
			'Invalid_string_refer_to_original_code
			If lu Is RightUnit And ru Is LeftUnit And Not LeftUnit Is RightUnit Then
				lu = LeftUnit
				ru = RightUnit
			End If
			
			'Invalid_string_refer_to_original_code
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
			
			'譛ｪ陦ｨ遉ｺ縺ｮ繝ｦ繝九ャ繝医ｒ陦ｨ遉ｺ縺吶ｋ
			If Not lu Is Nothing And Not lu Is LeftUnit Then
				'蟾ｦ縺ｮ繝ｦ繝九ャ繝医′譛ｪ陦ｨ遉ｺ縺ｪ縺ｮ縺ｧ陦ｨ遉ｺ縺吶ｋ
				
				'Invalid_string_refer_to_original_code
				If lu.BitmapID > 0 Then
					If MapDrawMode = "" Then
						'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: PictureBox プロパティ picUnit1.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * (lu.BitmapID Mod 15), 96 * (lu.BitmapID \ 15), SRCCOPY)
					Else
						LoadUnitBitmap(lu, .picUnit1, 0, 0, True)
					End If
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ picUnit1.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * (lu.X - 1), 32 * (lu.Y - 1), SRCCOPY)
				End If
				.picUnit1.Refresh()
				
				'Invalid_string_refer_to_original_code
				If lu.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
					.labHP1.Text = Term("HP")
				Else
					.labHP1.Text = Term("HP", lu)
				End If
				
				'Invalid_string_refer_to_original_code
				If lu.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox メソッド picHP1.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.picHP1.Cls()
				If lu.HP > 0 Or i < num Then
					'UPGRADE_ISSUE: PictureBox メソッド picHP1.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.picHP1.Line (0, 0) - ((.picHP1.Width - 4) * lu.HP \ lu.MaxHP - 1, 4), BF
				End If
				
				'Invalid_string_refer_to_original_code
				If lu.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
					.labEN1.Text = Term("EN")
				Else
					.labEN1.Text = Term("EN", lu)
				End If
				
				'Invalid_string_refer_to_original_code
				If lu.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox メソッド picEN1.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.picEN1.Cls()
				If lu.EN > 0 Or i < num Then
					'UPGRADE_ISSUE: PictureBox メソッド picEN1.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.picEN1.Line (0, 0) - ((.picEN1.Width - 4) * lu.EN \ lu.MaxEN - 1, 4), BF
				End If
				
				'Invalid_string_refer_to_original_code
				LeftUnit = lu
				LeftUnitHPRatio = lu.HP / lu.MaxHP
				LeftUnitENRatio = lu.EN / lu.MaxEN
			End If
			
			If Not ru Is Nothing And Not RightUnit Is ru Then
				'蜿ｳ縺ｮ繝ｦ繝九ャ繝医′譛ｪ陦ｨ遉ｺ縺ｪ縺ｮ縺ｧ陦ｨ遉ｺ縺吶ｋ
				
				'Invalid_string_refer_to_original_code
				If ru.BitmapID > 0 Then
					If MapDrawMode = "" Then
						'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: PictureBox プロパティ picUnit2.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, MainForm.picUnitBitmap.hDC, 32 * (ru.BitmapID Mod 15), 96 * (ru.BitmapID \ 15), SRCCOPY)
					Else
						LoadUnitBitmap(ru, .picUnit2, 0, 0, True)
					End If
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ picUnit2.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * (ru.X - 1), 32 * (ru.Y - 1), SRCCOPY)
				End If
				.picUnit2.Refresh()
				
				'Invalid_string_refer_to_original_code
				If ru.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
					.labHP2.Text = Term("HP")
				Else
					.labHP2.Text = Term("HP", ru)
				End If
				
				'Invalid_string_refer_to_original_code
				If ru.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox メソッド picHP2.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.picHP2.Cls()
				If ru.HP > 0 Or i < num Then
					'UPGRADE_ISSUE: PictureBox メソッド picHP2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.picHP2.Line (0, 0) - ((.picHP2.Width - 4) * ru.HP \ ru.MaxHP - 1, 4), BF
				End If
				
				'Invalid_string_refer_to_original_code
				If ru.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
					.labEN2.Text = Term("EN")
				Else
					.labEN2.Text = Term("EN", ru)
				End If
				
				'Invalid_string_refer_to_original_code
				If ru.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox メソッド picEN2.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.picEN2.Cls()
				If ru.EN > 0 Or i < num Then
					'UPGRADE_ISSUE: PictureBox メソッド picEN2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.picEN2.Line (0, 0) - ((.picEN2.Width - 4) * ru.EN \ ru.MaxEN - 1, 4), BF
				End If
				
				'Invalid_string_refer_to_original_code
				RightUnit = ru
				RightUnitHPRatio = ru.HP / ru.MaxHP
				RightUnitENRatio = ru.EN / ru.MaxEN
			End If
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If num > 0 Then
				If IsRButtonPressed() Then
					num = 2
				End If
			End If
			
			For i = 1 To num
				'Invalid_string_refer_to_original_code
				If Not lu Is Nothing Then
					'Invalid_string_refer_to_original_code
					If lu.HP / lu.MaxHP <> LeftUnitHPRatio Then
						tmp = (lu.MaxHP * LeftUnitHPRatio * (num - i) + lu.HP * i) \ num
						
						If lu.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
					
					'Invalid_string_refer_to_original_code
					If lu.EN / lu.MaxEN <> LeftUnitENRatio Then
						tmp = (lu.MaxEN * LeftUnitENRatio * (num - i) + lu.EN * i) \ num
						
						If lu.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
				
				'Invalid_string_refer_to_original_code
				If Not ru Is Nothing Then
					'Invalid_string_refer_to_original_code
					If ru.HP / ru.MaxHP <> RightUnitHPRatio Then
						tmp = (ru.MaxHP * RightUnitHPRatio * (num - i) + ru.HP * i) \ num
						
						If ru.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
					
					'Invalid_string_refer_to_original_code
					If ru.EN / ru.MaxEN <> RightUnitENRatio Then
						tmp = (ru.MaxEN * RightUnitENRatio * (num - i) + ru.EN * i) \ num
						If ru.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
				
				'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Sub SaveMessageFormStatus()
		IsMessageFormVisible = frmMessage.Visible
		SavedLeftUnit = LeftUnit
		SavedRightUnit = RightUnit
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub KeepMessageFormStatus()
		If Not IsMessageFormVisible Then
			'Invalid_string_refer_to_original_code
			If frmMessage.Visible Then
				'Invalid_string_refer_to_original_code
				CloseMessageForm()
			End If
		ElseIf Not frmMessage.Visible Then 
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			OpenMessageForm(SavedLeftUnit, SavedRightUnit)
		ElseIf LeftUnit Is Nothing And RightUnit Is Nothing And (Not SavedLeftUnit Is Nothing Or Not SavedRightUnit Is Nothing) Then 
			'Invalid_string_refer_to_original_code
			OpenMessageForm(SavedLeftUnit, SavedRightUnit)
		End If
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
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
		
		'繧ｭ繝｣繝ｩ陦ｨ遉ｺ縺ｮ謠上″謠帙∴
		If pname = "Invalid_string_refer_to_original_code" Then
			'Invalid_string_refer_to_original_code
			frmMessage.picFace.Image = System.Drawing.Image.FromFile("")
			frmMessage.picFace.Refresh()
			DisplayedPilot = ""
			left_margin = ""
		ElseIf pname <> "" Then 
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If fname <> "-.bmp" Then
				fname = "Pilot\" & fname
				If DisplayedPilot <> fname Or DisplayMode <> msg_mode Then
					If DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "Invalid_string_refer_to_original_code" & msg_mode) Then
						frmMessage.picFace.Refresh()
						DisplayedPilot = fname
						DisplayMode = msg_mode
					Else
						frmMessage.picFace.Image = System.Drawing.Image.FromFile("")
						frmMessage.picFace.Refresh()
						DisplayedPilot = ""
						DisplayMode = ""
						
						'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			left_margin = " "
		Else
			left_margin = "  "
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		FormatMessage(msg)
		msg = Trim(msg)
		
		'Invalid_string_refer_to_original_code
		Do While Right(msg, 1) = ";"
			msg = Left(msg, Len(msg) - 1)
		Loop 
		
		'Invalid_string_refer_to_original_code
		If msg = "" Then
			Exit Sub
		End If
		
		Select Case pname
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
			Case ""
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				i = 0
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'UPGRADE_WARNING: DisplayMessage に変換されていないステートメントがあります。ソース コードを確認してください。
				i = InStr(msg, "(")
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'End If
				If i > 1 Then
					If i < 8 Or PDList.IsDefined(Trim(Left(msg, i - 1))) Or NPDList.IsDefined(Trim(Left(msg, i - 1))) Then
						is_character_message = True
						If Not IsSpace(Mid(msg, i - 1, 1)) Then
							'Invalid_string_refer_to_original_code
							msg = Left(msg, i - 1) & " " & Mid(msg, i)
						End If
					End If
				End If
			Case Else
				is_character_message = True
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'繝｢繝弱Ο繝ｼ繧ｰ
				msg = Mid(msg, 2, Len(msg) - 2)
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				msg = Mid(msg, 2, Len(msg) - 2)
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'縺帙ｊ縺ｵ
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'End If
		End Select
		
		'Invalid_string_refer_to_original_code
		If IsOptionDefined("謾ｹ陦梧凾菴咏區遏ｭ邵ｮ") Then
			cl_margin(0) = 0.94 'Invalid_string_refer_to_original_code
			cl_margin(1) = 0.7 'Invalid_string_refer_to_original_code
			cl_margin(2) = 0.85 'Invalid_string_refer_to_original_code
		Else
			cl_margin(0) = 0.8
			cl_margin(1) = 0.6
			cl_margin(2) = 0.75
		End If
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		msg = ""
		For i = 1 To UBound(messages)
			msg = msg & messages(i)
		Next 
		
		'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				With p
					.Font = VB6.FontChangeBold(.Font, False)
					.Font = VB6.FontChangeItalic(.Font, False)
					'UPGRADE_WARNING: Windows フォームでは、TrueType および OpenType フォントのみがサポートされます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="971F4DF4-254E-44F4-861D-3AA0031FE361"' をクリックしてください。
					.Font = VB6.FontChangeName(.Font, "Invalid_string_refer_to_original_code")
					.Font = VB6.FontChangeSize(.Font, 12)
					.ForeColor = System.Drawing.Color.Black
				End With
			Else
				'Invalid_string_refer_to_original_code
				If is_character_message Then
					'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					p.Print("  ")
				End If
			End If
			
			counter = msg_head
			For j = counter To Len(buf)
				ch = Mid(buf, j, 1)
				
				'Invalid_string_refer_to_original_code
				If ch = ";" Then
					If j <> line_head Then
						PrintMessage(Mid(buf, line_head, j - line_head))
						lnum = lnum + 1
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						p.Print(left_margin)
					End If
				End If
				line_head = j + 1
				GoTo NextLoop
				'End If
				
				'Invalid_string_refer_to_original_code
				If ch = "<" Then
					in_tag = True
					GoTo NextLoop
				ElseIf ch = ">" Then 
					in_tag = False
				ElseIf in_tag Then 
					GoTo NextLoop
				End If
				
				'Invalid_string_refer_to_original_code
				If MessageLen(Mid(buf, line_head, j - line_head)) > 0.95 * VB6.PixelsToTwipsX(p.Width) Then
					PrintMessage(Mid(buf, line_head, j - line_head + 1))
					lnum = lnum + 1
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					p.Print(left_margin)
				End If
				line_head = j + 1
				GoTo NextLoop
				'End If
				
				'Invalid_string_refer_to_original_code
				Select Case Mid(buf, j + 1, 1)
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						GoTo NextLoop
				End Select
				Select Case Mid(buf, j + 2, 1)
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						GoTo NextLoop
				End Select
				If Mid(buf, j + 3, 1) = ";" Then
					GoTo NextLoop
				End If
				
				'Invalid_string_refer_to_original_code
				If MessageLen(Mid(messages(i), line_head)) < 0.95 * VB6.PixelsToTwipsX(p.Width) Then
					'Invalid_string_refer_to_original_code
					GoTo NextLoop
				End If
				Select Case ch
					Case "Invalid_string_refer_to_original_code"
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(1) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head + 1))
							lnum = lnum + 1
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print(left_margin)
						End If
						line_head = j + 1
						'End If
					Case "Invalid_string_refer_to_original_code"
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(2) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head + 1))
							lnum = lnum + 1
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print(left_margin)
						End If
						line_head = j + 1
						'End If
					Case " "
						ch = Mid(buf, j - 1, 1)
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code_
						'Or ch = "窶ｦ" Or ch = "窶･" _
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						'Invalid_string_refer_to_original_code
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(1) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head + 1))
							lnum = lnum + 1
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print(left_margin)
						End If
						line_head = j + 1
						'End If
						'蜊倥↑繧狗ｩｺ逋ｽ
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(0) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head + 1))
							lnum = lnum + 1
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print(left_margin)
						End If
						line_head = j + 1
						'End If
						'End If
					Case Else
						
						If MessageLen(Mid(buf, line_head, j - line_head)) > cl_margin(0) * VB6.PixelsToTwipsX(p.Width) Then
							PrintMessage(Mid(buf, line_head, j - line_head + 1))
							lnum = lnum + 1
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print(left_margin)
						End If
						line_head = j + 1
						'End If
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
			'Invalid_string_refer_to_original_code
			If lnum < 4 Then
				If Len(buf) >= line_head Then
					PrintMessage(Mid(buf, line_head))
				End If
			End If
			
			System.Windows.Forms.Application.DoEvents()
			
			If MessageWait > 10000 Then
				AutoMessageMode = False
			End If
			
			'Invalid_string_refer_to_original_code
			If AutoMessageMode Then
				If frmMessage.Text = "Invalid_string_refer_to_original_code" Then
					frmMessage.Text = "Invalid_string_refer_to_original_code"
				End If
			Else
				If frmMessage.Text = "Invalid_string_refer_to_original_code" Then
					frmMessage.Text = "Invalid_string_refer_to_original_code"
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			start_time = timeGetTime()
			wait_time = (lnum - prev_lnum + 2) * (MessageWait + 250)
			
			'Invalid_string_refer_to_original_code
			IsFormClicked = False
			is_automode = AutoMessageMode
			Do Until IsFormClicked
				If AutoMessageMode Then
					If start_time + wait_time < timeGetTime() Then
						Exit Do
					End If
				End If
				
				GetCursorPos(PT)
				
				'Invalid_string_refer_to_original_code
				If System.Windows.Forms.Form.ActiveForm Is frmMessage Then
					With frmMessage
						If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
							lstate = GetAsyncKeyState(LButtonID)
							rstate = GetAsyncKeyState(RButtonID)
							If (lstate And &H8000) <> 0 Then
								If start_time + wait_time < timeGetTime Then
									'Invalid_string_refer_to_original_code
									Exit Do
								End If
							ElseIf (rstate And &H8000) <> 0 Then 
								'Invalid_string_refer_to_original_code
								Exit Do
							End If
						End If
					End With
				End If
				
				'Invalid_string_refer_to_original_code
				If System.Windows.Forms.Form.ActiveForm Is MainForm Then
					With MainForm
						If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
							lstate = GetAsyncKeyState(LButtonID)
							rstate = GetAsyncKeyState(RButtonID)
							If (lstate And &H8000) <> 0 Then
								If start_time + wait_time < timeGetTime Then
									'Invalid_string_refer_to_original_code
									Exit Do
								End If
							ElseIf (rstate And &H8000) <> 0 Then 
								'Invalid_string_refer_to_original_code
								Exit Do
							End If
						End If
					End With
				End If
				
				Sleep(100)
				System.Windows.Forms.Application.DoEvents()
				
				'Invalid_string_refer_to_original_code
				If is_automode <> AutoMessageMode Then
					IsFormClicked = False
					is_automode = AutoMessageMode
					If AutoMessageMode Then
						frmMessage.Text = "Invalid_string_refer_to_original_code"
						start_time = timeGetTime()
						wait_time = (lnum - prev_lnum + 2) * (MessageWait + 250)
					Else
						frmMessage.Text = "Invalid_string_refer_to_original_code"
					End If
				End If
			Loop 
			
			'繧ｦ繧ｧ繧､繝郁ｨ育ｮ礼畑縺ｫ譌｢縺ｫ陦ｨ遉ｺ縺励◆陦梧焚繧定ｨ倬鹸
			If lnum < 4 Then
				prev_lnum = lnum
			Else
				prev_lnum = 0
			End If
		Loop 
		
		'Invalid_string_refer_to_original_code
		With p
			.Font = VB6.FontChangeBold(.Font, False)
			.Font = VB6.FontChangeItalic(.Font, False)
			'UPGRADE_WARNING: Windows フォームでは、TrueType および OpenType フォントのみがサポートされます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="971F4DF4-254E-44F4-861D-3AA0031FE361"' をクリックしてください。
			.Font = VB6.FontChangeName(.Font, "Invalid_string_refer_to_original_code")
			.Font = VB6.FontChangeSize(.Font, 12)
			.ForeColor = System.Drawing.Color.Black
		End With
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("Invalid_string_refer_to_original_code" & vbCr & vbLf & DisplayedPilot & vbCr & vbLf & "Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If is_sys_msg Then
				Select Case ch
					Case "["
						escape_depth = escape_depth + 1
						If escape_depth = 1 Then
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print(Mid(msg, head, i - head))
							head = i + 1
							GoTo NextChar
						End If
					Case "]"
						escape_depth = escape_depth - 1
						If escape_depth = 0 Then
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							p.Print(Mid(msg, head, i - head))
							head = i + 1
							GoTo NextChar
						End If
				End Select
			End If
			
			'Invalid_string_refer_to_original_code
			Select Case ch
				Case "<"
					If Not in_tag And escape_depth = 0 Then
						'Invalid_string_refer_to_original_code
						in_tag = True
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						p.Print(Mid(msg, head, i - head))
						head = i + 1
						GoTo NextChar
					End If
				Case ">"
					If in_tag Then
						'Invalid_string_refer_to_original_code
						in_tag = False
						
						'Invalid_string_refer_to_original_code
						tag = LCase(Mid(msg, head, i - head))
						
						'Invalid_string_refer_to_original_code
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
									'Invalid_string_refer_to_original_code
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
									'Invalid_string_refer_to_original_code
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
									'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If in_tag Or escape_depth > 0 Then
			head = head - 1
		End If
		
		'Invalid_string_refer_to_original_code
		If head <= Len(msg) Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
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
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		p.Print()
		'End If
		
		'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Function MessageLen(ByVal msg As String) As Short
		Dim buf As String
		Dim ret As Short
		
		'Invalid_string_refer_to_original_code
		ret = InStr(msg, "<")
		If ret = 0 Then
			'UPGRADE_ISSUE: PictureBox メソッド picMessage.TextWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			MessageLen = frmMessage.picMessage.TextWidth(msg)
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: PictureBox メソッド picMessage.TextWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		MessageLen = frmMessage.picMessage.TextWidth(buf)
	End Function
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If frmMessage.WindowState = 1 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If frmMessage.Text = "Invalid_string_refer_to_original_code" Then
			frmMessage.Text = "Invalid_string_refer_to_original_code"
		End If
		
		'繧ｭ繝｣繝ｩ陦ｨ遉ｺ縺ｮ謠上″謠帙∴
		If pname = "Invalid_string_refer_to_original_code" Then
			'Invalid_string_refer_to_original_code
			frmMessage.picFace.Image = System.Drawing.Image.FromFile("")
			frmMessage.picFace.Refresh()
			DisplayedPilot = ""
		ElseIf pname <> "" And pname <> "-" Then 
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If fname <> "-.bmp" Then
				fname = "Pilot\" & fname
				If DisplayedPilot <> fname Or DisplayMode <> msg_mode Then
					If DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "Invalid_string_refer_to_original_code" & msg_mode) Then
						frmMessage.picFace.Refresh()
						DisplayedPilot = fname
						DisplayMode = msg_mode
					Else
						frmMessage.picFace.Image = System.Drawing.Image.FromFile("")
						frmMessage.picFace.Refresh()
						DisplayedPilot = ""
						DisplayMode = ""
						
						'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If msg = "" Then
			Exit Sub
		End If
		
		p = frmMessage.picMessage
		
		'Invalid_string_refer_to_original_code
		SaveMessageFormStatus()
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If IsOptionDefined("謾ｹ陦梧凾菴咏區遏ｭ邵ｮ") Then
			cl_margin(0) = 0.94 'Invalid_string_refer_to_original_code
			cl_margin(1) = 0.7 'Invalid_string_refer_to_original_code
			cl_margin(2) = 0.85 'Invalid_string_refer_to_original_code
		Else
			cl_margin(0) = 0.8
			cl_margin(1) = 0.6
			cl_margin(2) = 0.75
		End If
		
		'Invalid_string_refer_to_original_code
		Dim fsuffix, fname0, fpath As String
		Dim first_id, last_id As Short
		Dim wait_time2 As Integer
		Dim with_footer As Boolean
		For i = 1 To UBound(messages)
			buf = messages(i)
			
			'Invalid_string_refer_to_original_code
			SaveBasePoint()
			FormatMessage(buf)
			RestoreBasePoint()
			
			'Invalid_string_refer_to_original_code
			Select Case LCase(Right(LIndex(buf, 1), 4))
				Case ".bmp", ".jpg", ".gif", ".png"
					
					'Invalid_string_refer_to_original_code
					If IsRButtonPressed() Then
						GoTo NextMessage
					End If
					
					'Invalid_string_refer_to_original_code
					fname = LIndex(buf, 1)
					
					'Invalid_string_refer_to_original_code
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
					
					'逕ｻ蜒剰｡ｨ遉ｺ縺ｮ繧ｪ繝励す繝ｧ繝ｳ
					options = ""
					n = LLength(buf)
					j = 2
					opt_n = 2
					Do While j <= n
						buf2 = LIndex(buf, j)
						Select Case buf2
							Case "騾城℃", "閭梧勹", "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								options = options & buf2 & " "
							Case "豸亥悉"
								clear_every_time = True
							Case "蜿ｳ蝗櫁ｻ｢"
								j = j + 1
								options = options & "蜿ｳ蝗櫁ｻ｢ " & LIndex(buf, j) & " "
							Case "蟾ｦ蝗櫁ｻ｢"
								j = j + 1
								options = options & "蟾ｦ蝗櫁ｻ｢ " & LIndex(buf, j) & " "
							Case "-"
								'Invalid_string_refer_to_original_code
								opt_n = j + 1
							Case Else
								If Asc(buf2) = 35 And Len(buf2) = 7 Then
									'Invalid_string_refer_to_original_code
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
									'Invalid_string_refer_to_original_code
									opt_n = j + 1
								End If
						End Select
						j = j + 1
					Loop 
					
					If Asc(fname) = 64 Then '@
						'Invalid_string_refer_to_original_code
						
						If first_id = -1 Then
							fname = Mid(fname, 2)
						Else
							fname0 = Mid(fname0, 2)
							fname = fname0 & VB6.Format(first_id, "00") & fsuffix
						End If
						
						'Invalid_string_refer_to_original_code
						If Not frmMessage.Visible Then
							OpenMessageForm()
						End If
						
						If wait_time > 0 Then
							start_time = timeGetTime()
						End If
						
						'逕ｻ蜒剰｡ｨ遉ｺ縺ｮ繧ｪ繝励す繝ｧ繝ｳ
						options = options & "Invalid_string_refer_to_original_code"
						Select Case MapDrawMode
							Case "繧ｻ繝斐い", "Invalid_string_refer_to_original_code"
								options = options & " " & MapDrawMode
						End Select
						
						If first_id = -1 Then
							'Invalid_string_refer_to_original_code
							DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, options)
							frmMessage.picFace.Refresh()
							
							If wait_time > 0 Then
								Do While (start_time + wait_time > timeGetTime())
									Sleep(20)
								Loop 
							End If
						Else
							'Invalid_string_refer_to_original_code
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
					
					'Invalid_string_refer_to_original_code
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
					
					'Invalid_string_refer_to_original_code
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
						'Invalid_string_refer_to_original_code
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
						'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					If IsRButtonPressed() Then
						GoTo NextMessage
					End If
					
					'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If Left(buf, 1) = "@" Then
				ShowAnimation(Mid(buf, 2))
				GoTo NextMessage
			End If
			
			'Invalid_string_refer_to_original_code
			Select Case LCase(LIndex(buf, 1))
				Case "clear"
					'Invalid_string_refer_to_original_code
					ClearPicture()
					need_refresh = True
					GoTo NextMessage
					
				Case "keep"
					'Invalid_string_refer_to_original_code
					IsPictureDrawn = False
					GoTo NextMessage
			End Select
			
			'Invalid_string_refer_to_original_code
			If IsNumeric(buf) Then
				wait_time = 100 * CDbl(buf)
				GoTo NextMessage
			End If
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			If buf = "" Then
				GoTo NextMessage
			End If
			
			'Invalid_string_refer_to_original_code
			KeepMessageFormStatus()
			
			With p
				'繧ｦ繧｣繝ｳ繝峨え繧偵け繝ｪ繧｢
				'UPGRADE_ISSUE: PictureBox メソッド p.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.Cls()
				'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.CurrentX = 1
				
				'Invalid_string_refer_to_original_code
				.Font = VB6.FontChangeBold(.Font, False)
				.Font = VB6.FontChangeItalic(.Font, False)
				'UPGRADE_WARNING: Windows フォームでは、TrueType および OpenType フォントのみがサポートされます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="971F4DF4-254E-44F4-861D-3AA0031FE361"' をクリックしてください。
				.Font = VB6.FontChangeName(.Font, "Invalid_string_refer_to_original_code")
				.Font = VB6.FontChangeSize(.Font, 12)
				.ForeColor = System.Drawing.Color.Black
			End With
			
			'Invalid_string_refer_to_original_code
			is_char_message = False
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			
			is_char_message = True
			
			'Invalid_string_refer_to_original_code
			If pname = "-" And Not SelectedUnit Is Nothing Then
				If SelectedUnit.CountPilot > 0 Then
					fname = SelectedUnit.MainPilot.Bitmap
					If DrawPicture(fname, 0, 0, 64, 64, 0, 0, 0, 0, "Invalid_string_refer_to_original_code" & msg_mode) Then
						frmMessage.picFace.Refresh()
						DisplayedPilot = fname
						DisplayMode = msg_mode
					End If
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If pnickname = "" And pname = "-" And Not SelectedUnit Is Nothing Then
				If SelectedUnit.CountPilot > 0 Then
					'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					p.Print(SelectedUnit.MainPilot.Nickname)
				End If
			ElseIf pnickname <> "" Then 
				'UPGRADE_ISSUE: PictureBox メソッド p.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				p.Print(pnickname)
			End If
			
			'Invalid_string_refer_to_original_code
			If Right(buf, 1) <> ":" Then
				with_footer = True
			Else
				with_footer = False
				prev_lnum = lnum
				buf = Left(buf, Len(buf) - 1)
			End If
			
			'諡ｬ蠑ｧ繧剃ｻ伜刈
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'繝｢繝弱Ο繝ｼ繧ｰ
			If with_footer Then
				buf = Mid(buf, 2, Len(buf) - 2)
				buf = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Else
				buf = Mid(buf, 2)
				buf = "Invalid_string_refer_to_original_code"
			End If
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			If with_footer Then
				buf = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Else
				buf = "Invalid_string_refer_to_original_code"
			End If
			'End If
			'Invalid_string_refer_to_original_code
			If Right(buf, 1) = ":" Then
				prev_lnum = lnum
				buf = Left(buf, Len(buf) - 1)
			End If
			'End If
			prev_lnum = MaxLng(prev_lnum, 1)
			
			lnum = 0
			line_head = 1
			For j = 1 To Len(buf)
				ch = Mid(buf, j, 1)
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				If ch = "<" Then
					in_tag = True
					GoTo NextLoop
				ElseIf ch = ">" Then 
					in_tag = False
				ElseIf in_tag Then 
					GoTo NextLoop
				End If
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				Select Case Mid(buf, j + 1, 1)
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						GoTo NextLoop
				End Select
				Select Case Mid(buf, j + 2, 1)
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						GoTo NextLoop
				End Select
				If Mid(buf, j + 3, 1) = "." Then
					GoTo NextLoop
				End If
				
				'Invalid_string_refer_to_original_code
				If MessageLen(Mid(messages(i), line_head)) < 0.95 * VB6.PixelsToTwipsX(p.Width) Then
					'Invalid_string_refer_to_original_code
					GoTo NextLoop
				End If
				Select Case ch
					Case "Invalid_string_refer_to_original_code"
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
					Case "Invalid_string_refer_to_original_code"
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
			'Invalid_string_refer_to_original_code
			If Len(buf) >= line_head Then
				PrintMessage(Mid(buf, line_head), Not is_char_message)
				lnum = lnum + 1
			End If
			
			'Invalid_string_refer_to_original_code
			With p
				.Font = VB6.FontChangeBold(.Font, False)
				.Font = VB6.FontChangeItalic(.Font, False)
				'UPGRADE_WARNING: Windows フォームでは、TrueType および OpenType フォントのみがサポートされます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="971F4DF4-254E-44F4-861D-3AA0031FE361"' をクリックしてください。
				.Font = VB6.FontChangeName(.Font, "Invalid_string_refer_to_original_code")
				.Font = VB6.FontChangeSize(.Font, 12)
				.ForeColor = System.Drawing.Color.Black
			End With
			
			'Invalid_string_refer_to_original_code
			If wait_time = DEFAULT_LEVEL Then
				wait_time = (lnum - prev_lnum + 1) * MessageWait
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				wait_time = wait_time \ 2
			End If
			'End If
			
			'逕ｻ髱｢繧呈峩譁ｰ
			If need_refresh Then
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).Refresh()
				need_refresh = False
			End If
			System.Windows.Forms.Application.DoEvents()
			
			'Invalid_string_refer_to_original_code
			start_time = timeGetTime()
			IsFormClicked = False
			Do While (start_time + wait_time > timeGetTime())
				'Invalid_string_refer_to_original_code
				If IsFormClicked Then
					Exit Do
				End If
				
				'Invalid_string_refer_to_original_code
				If IsRButtonPressed() Then
					Exit Do
				End If
				
				Sleep(20)
				System.Windows.Forms.Application.DoEvents()
			Loop 
			wait_time = DEFAULT_LEVEL
NextMessage: 
		Next 
		
		'Invalid_string_refer_to_original_code
		If pname = "-" Then
			Exit Sub
		End If
		
		'逕ｻ髱｢繧呈峩譁ｰ
		If need_refresh Then
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Refresh()
			need_refresh = False
		End If
		
		'Invalid_string_refer_to_original_code
		If wait_time > 0 Then
			start_time = timeGetTime()
			IsFormClicked = False
			Do While (start_time + wait_time > timeGetTime())
				'Invalid_string_refer_to_original_code
				If IsFormClicked Then
					Exit Do
				End If
				
				'Invalid_string_refer_to_original_code
				If IsRButtonPressed() Then
					Exit Do
				End If
				
				Sleep(20)
				System.Windows.Forms.Application.DoEvents()
			Loop 
		End If
		
		'Invalid_string_refer_to_original_code
		KeepMessageFormStatus()
		
		System.Windows.Forms.Application.DoEvents()
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("逕ｻ蜒上ヵ繧｡繧､繝ｫ" & vbCr & vbLf & fname & vbCr & vbLf & "Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub DisplaySysMessage(ByVal msg As String, Optional ByVal short_wait As Boolean = False)
		Dim j, i, line_head As Short
		Dim ch, buf As String
		Dim p As System.Windows.Forms.PictureBox
		Dim lnum As Short
		Dim start_time, wait_time As Integer
		Dim in_tag As Boolean
		
		'Invalid_string_refer_to_original_code
		If frmMessage.WindowState = 1 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		FormatMessage(msg)
		
		'Invalid_string_refer_to_original_code
		If frmMessage.Text = "Invalid_string_refer_to_original_code" Then
			frmMessage.Text = "Invalid_string_refer_to_original_code"
		End If
		
		p = frmMessage.picMessage
		
		With p
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: PictureBox メソッド p.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.Cls()
			'UPGRADE_ISSUE: PictureBox プロパティ p.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.CurrentX = 1
			
			'Invalid_string_refer_to_original_code
			.Font = VB6.FontChangeBold(.Font, False)
			.Font = VB6.FontChangeItalic(.Font, False)
			'UPGRADE_WARNING: Windows フォームでは、TrueType および OpenType フォントのみがサポートされます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="971F4DF4-254E-44F4-861D-3AA0031FE361"' をクリックしてください。
			.Font = VB6.FontChangeName(.Font, "Invalid_string_refer_to_original_code")
			.Font = VB6.FontChangeSize(.Font, 12)
			.ForeColor = System.Drawing.Color.Black
		End With
		
		'Invalid_string_refer_to_original_code
		lnum = 0
		line_head = 1
		For i = 1 To Len(msg)
			ch = Mid(msg, i, 1)
			
			'Invalid_string_refer_to_original_code
			If ch = ";" Then
				If line_head <> i Then
					buf = Mid(msg, line_head, i - line_head)
					PrintMessage(buf, True)
					lnum = lnum + 1
				End If
				line_head = i + 1
				GoTo NextLoop
			End If
			
			'Invalid_string_refer_to_original_code
			If ch = "<" Then
				in_tag = True
				GoTo NextLoop
			ElseIf ch = ">" Then 
				in_tag = False
			ElseIf in_tag Then 
				GoTo NextLoop
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			GoTo NextLoop
			'End If
			If i < Len(msg) Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GoTo NextLoop
			End If
			'End If
			
			If MessageLen(Mid(msg, line_head)) < VB6.PixelsToTwipsX(p.Width) Then
				'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		With p
			.Font = VB6.FontChangeBold(.Font, False)
			.Font = VB6.FontChangeItalic(.Font, False)
			'UPGRADE_WARNING: Windows フォームでは、TrueType および OpenType フォントのみがサポートされます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="971F4DF4-254E-44F4-861D-3AA0031FE361"' をクリックしてください。
			.Font = VB6.FontChangeName(.Font, "Invalid_string_refer_to_original_code")
			.Font = VB6.FontChangeSize(.Font, 12)
			.ForeColor = System.Drawing.Color.Black
		End With
		
		'Invalid_string_refer_to_original_code
		wait_time = (0.8 + 0.5 * lnum) * MessageWait
		If short_wait Then
			wait_time = wait_time \ 2
		End If
		
		System.Windows.Forms.Application.DoEvents()
		
		'Invalid_string_refer_to_original_code
		IsFormClicked = False
		start_time = timeGetTime()
		Do While (start_time + wait_time > timeGetTime())
			'Invalid_string_refer_to_original_code
			If IsFormClicked Then
				Exit Do
			End If
			
			'Invalid_string_refer_to_original_code
			If IsRButtonPressed() Then
				Exit Do
			End If
			
			Sleep(20)
			System.Windows.Forms.Application.DoEvents()
		Loop 
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If MapDrawMode <> draw_mode Then
			UList.ClearUnitBitmap()
			MapDrawMode = draw_mode
			MapDrawFilterColor = filter_color
			MapDrawFilterTransPercent = filter_trans_par
		ElseIf draw_mode = "繝輔ぅ繝ｫ繧ｿ" And (MapDrawFilterColor <> filter_color Or MapDrawFilterTransPercent <> filter_trans_par) Then 
			UList.ClearUnitBitmap()
			MapDrawMode = draw_mode
			MapDrawFilterColor = filter_color
			MapDrawFilterTransPercent = filter_trans_par
		End If
		
		'Invalid_string_refer_to_original_code
		With MainForm
			Select Case draw_option
				Case "Invalid_string_refer_to_original_code"
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
			
			'Invalid_string_refer_to_original_code
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					xx = 32 * (i - 1)
					yy = 32 * (j - 1)
					
					'DEL START 240a
					'Invalid_string_refer_to_original_code
					'                For k = 1 To terrain_bmp_count
					'                    If terrain_bmp_type(k) = MapData(i, j, 0) _
					''                        And terrain_bmp_num(k) = MapData(i, j, 1) _
					''                    Then
					'                        Exit For
					'                    End If
					'                Next
					
					'                If k <= terrain_bmp_count Then
					'Invalid_string_refer_to_original_code
					'                    ret = BitBlt(.picBack.hDC, _
					''                        xx, yy, 32, 32, _
					''                        .picBack.hDC, terrain_bmp_x(k), terrain_bmp_y(k), SRCCOPY)
					'                    MapImageFileTypeData(i, j) = _
					''                        MapImageFileTypeData(terrain_bmp_x(k) \ 32 + 1, terrain_bmp_y(k) \ 32 + 1)
					'                Else
					'Invalid_string_refer_to_original_code
					'DEL  END  240a
					'Invalid_string_refer_to_original_code
					'MOD START 240a
					'                fname = SearchTerrainImageFile(MapData(i, j, 0), MapData(i, j, 1), i, j)
					fname = SearchTerrainImageFile(MapData(i, j, Map.MapDataIndex.TerrainType), MapData(i, j, Map.MapDataIndex.BitmapNo), i, j)
					'MOD  END  240a
					
					'逕ｻ蜒上ｒ蜿悶ｊ霎ｼ縺ｿ
					If fname <> "" Then
						On Error GoTo ErrorHandler
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.picTmp32(0) = System.Drawing.Image.FromFile(fname)
						On Error GoTo 0
					Else
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = PatBlt(.picTmp32(0).hDC, 0, 0, 32, 32, BLACKNESS)
					End If
					
					'繝槭ャ繝苓ｨｭ螳壹↓繧医▲縺ｦ陦ｨ遉ｺ濶ｲ繧貞､画峩
					Select Case draw_mode
						Case "Invalid_string_refer_to_original_code"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							Dark()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
						Case "繧ｻ繝斐い"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							Sepia()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
						Case "Invalid_string_refer_to_original_code"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							Monotone()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
						Case "Invalid_string_refer_to_original_code"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							Sunset()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
						Case "豌ｴ荳ｭ"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							Water()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
						Case "繝輔ぅ繝ｫ繧ｿ"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent)
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
					End Select
					
					'逕ｻ蜒上ｒ謠上″霎ｼ縺ｿ
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picBack.hDC, xx, yy, 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
					'DEL START 240a
					'                    '逕ｻ蜒上ｒ逋ｻ骭ｲ
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
					'Invalid_string_refer_to_original_code
					If Map.BoxTypes.Upper = MapData(i, j, Map.MapDataIndex.BoxType) Or Map.BoxTypes.UpperBmpOnly = MapData(i, j, Map.MapDataIndex.BoxType) Then
						'Invalid_string_refer_to_original_code
						fname = SearchTerrainImageFile(MapData(i, j, Map.MapDataIndex.LayerType), MapData(i, j, Map.MapDataIndex.LayerBitmapNo), i, j)
						
						'逕ｻ蜒上ｒ蜿悶ｊ霎ｼ縺ｿ
						If fname <> "" Then
							On Error GoTo ErrorHandler
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.picTmp32(0) = System.Drawing.Image.FromFile(fname)
							On Error GoTo 0
							BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
							'繝槭ャ繝苓ｨｭ螳壹↓繧医▲縺ｦ陦ｨ遉ｺ濶ｲ繧貞､画峩
							Select Case draw_mode
								Case "Invalid_string_refer_to_original_code"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									Dark(True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
								Case "繧ｻ繝斐い"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									Sepia(True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
								Case "Invalid_string_refer_to_original_code"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									Monotone(True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
								Case "Invalid_string_refer_to_original_code"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									Sunset(True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
								Case "豌ｴ荳ｭ"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									Water(True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
								Case "繝輔ぅ繝ｫ繧ｿ"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent, True)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
							End Select
							
							'逕ｻ蜒上ｒ騾城℃謠上″霎ｼ縺ｿ
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = TransparentBlt(.picBack.hDC, xx, yy, 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, BGColor)
						End If
						
					End If
					'ADD  END  240a
				Next 
			Next 
			'Invalid_string_refer_to_original_code
			
			'繝槭せ逶ｮ縺ｮ陦ｨ遉ｺ
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
			
			'Invalid_string_refer_to_original_code
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
		
		'逕ｻ髱｢繧呈峩譁ｰ
		If MapFileName <> "" And draw_option = "" Then
			RefreshScreen()
		End If
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("Invalid_string_refer_to_original_code" & vbCr & vbLf & fname & vbCr & vbLf & "Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		TerminateSRC()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RedrawScreen(Optional ByVal late_refresh As Boolean = False)
		Dim PT As POINTAPI
		Dim ret As Integer
		
		ScreenIsMasked = False
		
		'逕ｻ髱｢繧呈峩譁ｰ
		RefreshScreen(False, late_refresh)
		
		'Invalid_string_refer_to_original_code
		GetCursorPos(PT)
		ret = SetCursorPos(PT.X, PT.Y)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub MaskScreen()
		ScreenIsMasked = True
		
		'逕ｻ髱｢繧呈峩譁ｰ
		RefreshScreen()
	End Sub
	
	' ADD START MARGE
	'Invalid_string_refer_to_original_code
	Public Sub RefreshScreen(Optional ByVal without_refresh As Boolean = False, Optional ByVal delay_refresh As Boolean = False)
		
		If NewGUIMode Then
			RefreshScreenNew(without_refresh, delay_refresh)
		Else
			RefreshScreenOld(without_refresh, delay_refresh)
		End If
	End Sub
	' ADD END MARGE
	
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If MapWidth = 1 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = PatBlt(pic.hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
			End If
			
			mx = MapX - (MainWidth + 1) \ 2 + 1
			my_Renamed = MapY - (MainHeight + 1) \ 2 + 1
			
			'Invalid_string_refer_to_original_code
			
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
			
			'Invalid_string_refer_to_original_code
			prev_color = System.Drawing.ColorTranslator.ToOle(pic.ForeColor)
			pic.ForeColor = System.Drawing.Color.Black
			
			'Invalid_string_refer_to_original_code
			If Not ScreenIsMasked Then
				'騾壼ｸｸ陦ｨ遉ｺ
				For i = 0 To dw - 1
					xx = 32 * (dx + i - 1)
					For j = 0 To dh - 1
						If sx + i < 1 Or sx + i > MapWidth Or sy + j < 1 Or sy + j > MapHeight Then
							GoTo NextLoop
						End If
						
						yy = 32 * (dy + j - 1)
						
						u = MapDataForUnit(sx + i, sy + j)
						If u Is Nothing Then
							'蝨ｰ蠖｢
							'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
						ElseIf u.BitmapID = -1 Then 
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
						End If
						
						'Invalid_string_refer_to_original_code
						Select Case u.Area
							Case "遨ｺ荳ｭ"
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = LineTo(pic.hDC, xx + 31, yy + 28)
							Case "豌ｴ荳ｭ"
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = LineTo(pic.hDC, xx + 31, yy + 3)
							Case "蝨ｰ荳ｭ"
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = LineTo(pic.hDC, xx + 31, yy + 28)
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = LineTo(pic.hDC, xx + 31, yy + 3)
							Case "Invalid_string_refer_to_original_code"
								If TerrainClass(sx + i, sy + j) = "譛磯擇" Then
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, xx + 31, yy + 28)
								End If
						End Select
					Next 
				Next 
			End If
			
NextLoop: 
			'Next
			'Next
			'繝槭せ繧ｯ陦ｨ遉ｺ
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
							'繝槭せ繧ｯ縺輔ｌ縺溷慍蠖｢
							'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
						Else
							'蝨ｰ蠖｢
							'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
						End If
					ElseIf u.BitmapID = -1 Then 
						'Invalid_string_refer_to_original_code
						If MaskData(sx + i, sy + j) Then
							'繝槭せ繧ｯ縺輔ｌ縺溷慍蠖｢
							'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
						Else
							'蝨ｰ蠖｢
							'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
						End If
					Else
						If MaskData(sx + i, sy + j) Then
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 64, SRCCOPY)
							
							'Invalid_string_refer_to_original_code
							Select Case u.Area
								Case "遨ｺ荳ｭ"
									DottedLine(xx, yy + 28)
								Case "豌ｴ荳ｭ"
									DottedLine(xx, yy + 3)
								Case "蝨ｰ荳ｭ"
									DottedLine(xx, yy + 28)
									DottedLine(xx, yy + 3)
								Case "Invalid_string_refer_to_original_code"
									If TerrainClass(sx + i, sy + j) = "譛磯擇" Then
										DottedLine(xx, yy + 28)
									End If
							End Select
						Else
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
							
							'Invalid_string_refer_to_original_code
							Select Case u.Area
								Case "遨ｺ荳ｭ"
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, xx + 31, yy + 28)
								Case "豌ｴ荳ｭ"
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, xx + 31, yy + 3)
								Case "蝨ｰ荳ｭ"
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, xx + 31, yy + 28)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, xx + 31, yy + 3)
								Case "Invalid_string_refer_to_original_code"
									If TerrainClass(sx + i, sy + j) = "譛磯擇" Then
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
			'End If
			
			'Invalid_string_refer_to_original_code
			pic.ForeColor = System.Drawing.ColorTranslator.FromOle(prev_color)
			
			'逕ｻ髱｢縺梧嶌縺肴鋤縺医ｉ繧後◆縺薙→繧定ｨ倬鹸
			ScreenIsSaved = False
			
			If Not without_refresh And Not delay_refresh Then
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Refresh()
			End If
		End With
	End Sub
	
	' ADD START MARGE
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If MapWidth = 1 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = PatBlt(pic.hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
			End If
			
			mx = MapX - (MainWidth + 1) \ 2 + 1
			my_Renamed = MapY - (MainHeight + 1) \ 2 + 1
			
			'Invalid_string_refer_to_original_code
			
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
			
			'Invalid_string_refer_to_original_code
			prev_color = System.Drawing.ColorTranslator.ToOle(pic.ForeColor)
			pic.ForeColor = System.Drawing.Color.Black
			
			'Invalid_string_refer_to_original_code
			If Not ScreenIsMasked Then
				'騾壼ｸｸ陦ｨ遉ｺ
				For i = -1 To dw - 1
					xx = 32 * (dx + i - 0.5)
					For j = 0 To dh - 1
						yy = 32 * (dy + j - 1)
						
						If sx + i < 1 Or sx + i > MapWidth Or sy + j < 1 Or sy + j > MapHeight Then
							GoTo NextLoop
						End If
						
						u = MapDataForUnit(sx + i, sy + j)
						
						If i = -1 Then
							'Invalid_string_refer_to_original_code
							If u Is Nothing Then
								'蝨ｰ蠖｢
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
							ElseIf u.BitmapID = -1 Then 
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
							End If
							
							'Invalid_string_refer_to_original_code
							Select Case u.Area
								Case "遨ｺ荳ｭ"
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, 0 + 15, yy + 28)
								Case "豌ｴ荳ｭ"
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, 0 + 15, yy + 3)
								Case "蝨ｰ荳ｭ"
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, 0 + 15, yy + 28)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
									'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
									ret = LineTo(pic.hDC, 0 + 15, yy + 3)
								Case "Invalid_string_refer_to_original_code"
									If TerrainClass(sx + i, sy + j) = "譛磯擇" Then
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, 0 + 15, yy + 28)
									End If
							End Select
						End If
					Next 
				Next 
			Else
				'Invalid_string_refer_to_original_code
				If u Is Nothing Then
					'蝨ｰ蠖｢
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
				ElseIf u.BitmapID = -1 Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
				End If
				
				'Invalid_string_refer_to_original_code
				Select Case u.Area
					Case "遨ｺ荳ｭ"
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = LineTo(pic.hDC, xx + 31, yy + 28)
					Case "豌ｴ荳ｭ"
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = LineTo(pic.hDC, xx + 31, yy + 3)
					Case "蝨ｰ荳ｭ"
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = LineTo(pic.hDC, xx + 31, yy + 28)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = LineTo(pic.hDC, xx + 31, yy + 3)
					Case "Invalid_string_refer_to_original_code"
						If TerrainClass(sx + i, sy + j) = "譛磯擇" Then
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
							'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
							ret = LineTo(pic.hDC, xx + 31, yy + 28)
						End If
				End Select
			End If
			'End If
NextLoop: 
			'Next
			'Next
			'繝槭せ繧ｯ陦ｨ遉ｺ
			For i = -1 To dw - 1
				xx = 32 * (dx + i - 0.5)
				For j = 0 To dh - 1
					yy = 32 * (dy + j - 1)
					
					If sx + i < 1 Or sx + i > MapWidth Or sy + j < 1 Or sy + j > MapHeight Then
						GoTo NextLoop2
					End If
					
					u = MapDataForUnit(sx + i, sy + j)
					
					If i = -1 Then
						'Invalid_string_refer_to_original_code
						If u Is Nothing Then
							If MaskData(sx + i, sy + j) Then
								'繝槭せ繧ｯ縺輔ｌ縺溷慍蠖｢
								'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picMaskedBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
							Else
								'蝨ｰ蠖｢
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
							End If
						ElseIf u.BitmapID = -1 Then 
							'Invalid_string_refer_to_original_code
							If MaskData(sx + i, sy + j) Then
								'繝槭せ繧ｯ縺輔ｌ縺溷慍蠖｢
								'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picMaskedBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
							Else
								'蝨ｰ蠖｢
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picBack.hDC, 32 * (sx - 1.5), 32 * (sy + j - 1), SRCCOPY)
							End If
						Else
							If MaskData(sx + i, sy + j) Then
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15) + 64, SRCCOPY)
								
								'Invalid_string_refer_to_original_code
								Select Case u.Area
									Case "遨ｺ荳ｭ"
										DottedLine(0, yy + 28, True)
									Case "豌ｴ荳ｭ"
										DottedLine(0, yy + 3, True)
									Case "蝨ｰ荳ｭ"
										DottedLine(0, yy + 28, True)
										DottedLine(0, yy + 3, True)
									Case "Invalid_string_refer_to_original_code"
										If TerrainClass(sx + i, sy + j) = "譛磯擇" Then
											DottedLine(0, yy + 28, True)
										End If
								End Select
							Else
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, 0, yy, 16, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15) + 16, 96 * (u.BitmapID \ 15), SRCCOPY)
								
								'Invalid_string_refer_to_original_code
								Select Case u.Area
									Case "遨ｺ荳ｭ"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, 0 + 15, yy + 28)
									Case "豌ｴ荳ｭ"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, 0 + 15, yy + 3)
									Case "蝨ｰ荳ｭ"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, 0 + 15, yy + 28)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, 0, yy + 3, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, 0 + 15, yy + 3)
									Case "Invalid_string_refer_to_original_code"
										If TerrainClass(sx + i, sy + j) = "譛磯擇" Then
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = MoveToEx(pic.hDC, 0, yy + 28, PT)
											'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
											ret = LineTo(pic.hDC, 0 + 15, yy + 28)
										End If
								End Select
							End If
						End If
					Else
						'Invalid_string_refer_to_original_code
						If u Is Nothing Then
							If MaskData(sx + i, sy + j) Then
								'繝槭せ繧ｯ縺輔ｌ縺溷慍蠖｢
								'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
							Else
								'蝨ｰ蠖｢
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
							End If
						ElseIf u.BitmapID = -1 Then 
							'Invalid_string_refer_to_original_code
							If MaskData(sx + i, sy + j) Then
								'繝槭せ繧ｯ縺輔ｌ縺溷慍蠖｢
								'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picMaskedBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
							Else
								'蝨ｰ蠖｢
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (sx + i - 1), 32 * (sy + j - 1), SRCCOPY)
							End If
						Else
							If MaskData(sx + i, sy + j) Then
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 64, SRCCOPY)
								
								'Invalid_string_refer_to_original_code
								Select Case u.Area
									Case "遨ｺ荳ｭ"
										DottedLine(xx, yy + 28)
									Case "豌ｴ荳ｭ"
										DottedLine(xx, yy + 3)
									Case "蝨ｰ荳ｭ"
										DottedLine(xx, yy + 28)
										DottedLine(xx, yy + 3)
									Case "Invalid_string_refer_to_original_code"
										If TerrainClass(sx + i, sy + j) = "譛磯擇" Then
											DottedLine(xx, yy + 28)
										End If
								End Select
							Else
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
								ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
								
								'Invalid_string_refer_to_original_code
								Select Case u.Area
									Case "遨ｺ荳ｭ"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 28)
									Case "豌ｴ荳ｭ"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 3)
									Case "蝨ｰ荳ｭ"
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 28)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
										'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
										ret = LineTo(pic.hDC, xx + 31, yy + 3)
									Case "Invalid_string_refer_to_original_code"
										If TerrainClass(sx + i, sy + j) = "譛磯擇" Then
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
			'End If
			
			'Invalid_string_refer_to_original_code
			pic.ForeColor = System.Drawing.ColorTranslator.FromOle(prev_color)
			
			'逕ｻ髱｢縺梧嶌縺肴鋤縺医ｉ繧後◆縺薙→繧定ｨ倬鹸
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
	
	'Invalid_string_refer_to_original_code
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
	
	
	' === 蠎ｧ讓吝､画鋤 ===
	
	'繝槭ャ繝嶺ｸ翫〒縺ｮ蠎ｧ讓吶′繝槭ャ繝礼判髱｢縺ｮ縺ｩ縺ｮ菴咲ｽｮ縺ｫ縺上ｋ縺九ｒ霑斐☆
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
	
	'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
	
	'繝ｦ繝九ャ繝育判蜒上ヵ繧｡繧､繝ｫ繧呈､懃ｴ｢
	Private Function FindUnitBitmap(ByRef u As Unit) As String
		Dim fname, uname As String
		Dim tnum, tname, tdir As String
		Dim i, j As Short
		
		With u
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .CountPilot = 0 Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'繝ｦ繝九ャ繝育判蜒上ｒ菴ｿ縺｣縺ｦ陦ｨ遉ｺ
			uname = "Invalid_string_refer_to_original_code" & .MainPilot.ID & "]"
			'UPGRADE_WARNING: オブジェクト LocalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			uname = LocalVariableList.Item(uname).StringValue
			fname = "\Bitmap\Unit\" & UList.Item(uname).Bitmap
			'Invalid_string_refer_to_original_code
			fname = "\Bitmap\Pilot\" & .MainPilot.Bitmap
			'End If
			
			'逕ｻ蜒上ｒ讀懃ｴ｢
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
			'End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			fname = .Bitmap
			If FileExists(AppPath & "Bitmap\Map\" & fname) Or FileExists(ScenarioPath & "Bitmap\Map\" & fname) Then
				fname = "Bitmap\Map\" & fname
			Else
				'Invalid_string_refer_to_original_code
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
						'Invalid_string_refer_to_original_code
						fname = "Bitmap\" & fname
					Else
						'Invalid_string_refer_to_original_code
						fname = "Bitmap\Unit\" & fname
					End If
				End If
			End If
			'騾壼ｸｸ縺ｮ繝ｦ繝九ャ繝域緒逕ｻ
			fname = .Bitmap
			If InStr(fname, ":") = 2 Then
				'Invalid_string_refer_to_original_code
			ElseIf InStr(fname, "\") > 0 Then 
				'Invalid_string_refer_to_original_code
				fname = "Bitmap\" & fname
			Else
				'Invalid_string_refer_to_original_code
				fname = "Bitmap\Unit\" & fname
			End If
			'End If
			
			'Invalid_string_refer_to_original_code
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
				
				'逕ｻ蜒上′隕九▽縺九ｉ縺ｪ縺九▲縺溘％縺ｨ繧定ｨ倬鹸
				If .Bitmap = .Data.Bitmap Then
					.Data.IsBitmapMissing = True
				End If
			End If
			
			FindUnitBitmap = fname
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function MakeUnitBitmap(ByRef u As Unit) As Short
		Dim fname, uparty As String
		Dim i As Short
		Dim ret As Integer
		Dim xx, yy As Short
		Static bitmap_num As Short
		Static fname_list() As String
		Static party_list() As String
		
		With MainForm
			If u.IsFeatureAvailable("髱櫁｡ｨ遉ｺ") Then
				MakeUnitBitmap = -1
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			If .picUnitBitmap.width = 32 Then
				bitmap_num = 0
			End If
			
			'Invalid_string_refer_to_original_code
			ReDim Preserve fname_list(bitmap_num)
			ReDim Preserve party_list(bitmap_num)
			
			'Invalid_string_refer_to_original_code
			fname = FindUnitBitmap(u)
			uparty = u.Party0
			For i = 1 To bitmap_num
				If fname = fname_list(i) And uparty = party_list(i) Then
					'Invalid_string_refer_to_original_code
					MakeUnitBitmap = i
					Exit Function
				End If
			Next 
			
			'譁ｰ縺溘↓逕ｻ蜒上ｒ逋ｻ骭ｲ
			bitmap_num = bitmap_num + 1
			ReDim Preserve fname_list(bitmap_num)
			ReDim Preserve party_list(bitmap_num)
			fname_list(bitmap_num) = fname
			party_list(bitmap_num) = uparty
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picUnitBitmap.Move(0, 0, 480, 96 * (bitmap_num \ 15 + 1))
			
			'Invalid_string_refer_to_original_code
			xx = 32 * (bitmap_num Mod 15)
			yy = 96 * (bitmap_num \ 15)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			LoadUnitBitmap(u, .picUnitBitmap, xx, yy, False, fname)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picUnitBitmap.hDC, xx, yy + 32, 32, 32, .picUnitBitmap.hDC, xx, yy, SRCCOPY)
			'UPGRADE_ISSUE: Control picMask は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picUnitBitmap.hDC, xx, yy + 32, 32, 32, .picMask.hDC, 0, 0, SRCAND)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picUnitBitmap.hDC, xx, yy + 64, 32, 32, .picUnitBitmap.hDC, xx, yy + 32, SRCCOPY)
			'UPGRADE_ISSUE: Control picMask2 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picUnitBitmap.hDC, xx, yy + 64, 32, 32, .picMask2.hDC, 0, 0, SRCINVERT)
		End With
		
		'繝ｦ繝九ャ繝育判蜒冗分蜿ｷ繧定ｿ斐☆
		MakeUnitBitmap = bitmap_num
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub LoadUnitBitmap(ByRef u As Unit, ByRef pic As System.Windows.Forms.PictureBox, ByVal dx As Short, ByVal dy As Short, Optional ByVal use_orig_color As Boolean = False, Optional ByRef fname As String = "")
		Dim ret As Integer
		Dim emit_light As Boolean
		
		With MainForm
			'逕ｻ蜒上ヵ繧｡繧､繝ｫ繧呈､懃ｴ｢
			If fname = "" Then
				fname = FindUnitBitmap(u)
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			On Error GoTo ErrorHandler
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picTmp = System.Drawing.Image.FromFile(fname)
			On Error GoTo 0
			
			'逕ｻ髱｢縺ｫ謠冗判
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(pic.hDC, dx, dy, 32, 32, .picTmp.hDC, 0, 0, .picTmp.width, .picTmp.Height, SRCCOPY)
			
			Exit Sub
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'And Not MapDrawIsMapOnly _
			'And Not use_orig_color _
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			emit_light = True
			'End If
			
			If fname <> "" Then
				'Invalid_string_refer_to_original_code
				On Error GoTo ErrorHandler
				'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picTmp32(0) = System.Drawing.Image.FromFile(fname)
				On Error GoTo 0
				
				'Invalid_string_refer_to_original_code
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
					ErrorMessage(u.Name & "Invalid_string_refer_to_original_code")
					Exit Sub
				End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
			ElseIf UseTransparentBlt Then 
				'TransparentBlt繧剃ｽｿ縺｣縺ｦ繝ｦ繝九ャ繝育判蜒上→繧ｿ繧､繝ｫ繧帝㍾縺ｭ蜷医ｏ縺帙ｋ
				
				'繧ｿ繧､繝ｫ
				Select Case u.Party0
					Case "蜻ｳ譁ｹ", "Invalid_string_refer_to_original_code"
						'UPGRADE_ISSUE: Control picUnit は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picUnit.hDC, 0, 0, SRCCOPY)
					Case "謨ｵ"
						'UPGRADE_ISSUE: Control picEnemy は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picEnemy.hDC, 0, 0, SRCCOPY)
					Case "Invalid_string_refer_to_original_code"
						'UPGRADE_ISSUE: Control picNeautral は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picNeautral.hDC, 0, 0, SRCCOPY)
				End Select
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				If Not emit_light Then
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = TransparentBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
				End If
			Else
				'BitBlt繧剃ｽｿ縺｣縺ｦ繝ｦ繝九ャ繝育判蜒上→繧ｿ繧､繝ｫ繧帝㍾縺ｭ蜷医ｏ縺帙ｋ
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MakeMask(.picTmp32(0).hDC, .picTmp32(2).hDC, 32, 32, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
				
				'繧ｿ繧､繝ｫ
				Select Case u.Party0
					Case "蜻ｳ譁ｹ", "Invalid_string_refer_to_original_code"
						'UPGRADE_ISSUE: Control picUnit は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picUnit.hDC, 0, 0, SRCCOPY)
					Case "謨ｵ"
						'UPGRADE_ISSUE: Control picEnemy は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picEnemy.hDC, 0, 0, SRCCOPY)
					Case "Invalid_string_refer_to_original_code"
						'UPGRADE_ISSUE: Control picNeautral は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picNeautral.hDC, 0, 0, SRCCOPY)
				End Select
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				If Not emit_light Then
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(2).hDC, 0, 0, SRCERASE)
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picTmp32(0).hDC, 0, 0, SRCINVERT)
				End If
			End If
			'Invalid_string_refer_to_original_code
			Select Case u.Party0
				Case "蜻ｳ譁ｹ", "Invalid_string_refer_to_original_code"
					'UPGRADE_ISSUE: Control picUnit は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picUnit.hDC, 0, 0, SRCCOPY)
				Case "謨ｵ"
					'UPGRADE_ISSUE: Control picEnemy は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picEnemy.hDC, 0, 0, SRCCOPY)
				Case "Invalid_string_refer_to_original_code"
					'UPGRADE_ISSUE: Control picNeautral は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picTmp32(1).hDC, 0, 0, 32, 32, .picNeautral.hDC, 0, 0, SRCCOPY)
			End Select
			'End If
			
			'Invalid_string_refer_to_original_code
			If Not use_orig_color And Not MapDrawIsMapOnly Then
				Select Case MapDrawMode
					Case "Invalid_string_refer_to_original_code"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						Dark()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
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
					Case "繧ｻ繝斐い"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						Sepia()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
					Case "Invalid_string_refer_to_original_code"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						Monotone()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
					Case "Invalid_string_refer_to_original_code"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						Sunset()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
					Case "豌ｴ荳ｭ"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						Water()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
					Case "繝輔ぅ繝ｫ繧ｿ"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(1))
						ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(1))
				End Select
			End If
			
			'逕ｻ髱｢縺ｫ謠冗判
			'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, dx, dy, 32, 32, .picTmp32(1).hDC, 0, 0, SRCCOPY)
		End With
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("Invalid_string_refer_to_original_code" & vbCr & vbLf & fname & vbCr & vbLf & "Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub PaintUnitBitmap(ByRef u As Unit, Optional ByVal smode As String = "")
		Dim xx, yy As Short
		Dim pic As System.Windows.Forms.PictureBox
		Dim ret As Integer
		Dim PT As POINTAPI
		
		With u
			'Invalid_string_refer_to_original_code
			If .BitmapID = -1 Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			If .X < MapX - (MainWidth + 1) \ 2 Or MapX + (MainWidth + 1) \ 2 < .X Or .Y < MapY - (MainHeight + 1) \ 2 Or MapY + (MainHeight + 1) \ 2 < .Y Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			xx = MapToPixelX(.X)
			yy = MapToPixelY(.Y)
		End With
		
		With MainForm
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = .picMain(1)
			'Invalid_string_refer_to_original_code
			PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(xx, 0))
			PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(yy, 0))
			PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(xx + 32, MainPWidth - 1))
			PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(yy + 32, MainPHeight - 1))
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = .picMain(0)
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'騾壼ｸｸ縺ｮ陦ｨ遉ｺ
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, xx, yy, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15) + 32, SRCCOPY)
			'End If
			
			'Invalid_string_refer_to_original_code
			pic.ForeColor = System.Drawing.Color.Black
			
			'Invalid_string_refer_to_original_code
			Select Case u.Area
				Case "遨ｺ荳ｭ"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, xx + 31, yy + 28)
				Case "豌ｴ荳ｭ"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, xx + 31, yy + 3)
				Case "蝨ｰ荳ｭ"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, xx + 31, yy + 28)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, xx, yy + 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, xx + 31, yy + 3)
				Case "Invalid_string_refer_to_original_code"
					If TerrainClass(u.X, u.Y) = "譛磯擇" Then
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = MoveToEx(pic.hDC, xx, yy + 28, PT)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = LineTo(pic.hDC, xx + 31, yy + 28)
					End If
			End Select
			
			'謠冗判濶ｲ繧堤區縺ｫ謌ｻ縺励※縺翫￥
			pic.ForeColor = System.Drawing.Color.White
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'逕ｻ髱｢縺梧嶌縺肴鋤縺医ｉ繧後◆縺薙→繧定ｨ倬鹸
			ScreenIsSaved = False
			
			If .Visible Then
				pic.Refresh()
			End If
			'End If
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub EraseUnitBitmap(ByVal X As Short, ByVal Y As Short, Optional ByVal do_refresh As Boolean = True)
		Dim xx, yy As Short
		Dim ret As Integer
		
		'Invalid_string_refer_to_original_code
		If X < MapX - (MainWidth + 1) \ 2 Or MapX + (MainWidth + 1) \ 2 < X Or Y < MapY - (MainHeight + 1) \ 2 Or MapY + (MainHeight + 1) \ 2 < Y Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If IsPictureVisible Then
			Exit Sub
		End If
		
		xx = MapToPixelX(X)
		yy = MapToPixelY(Y)
		
		With MainForm
			SaveScreen()
			
			'逕ｻ髱｢陦ｨ遉ｺ螟画峩
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = BitBlt(.picMain(1).hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
			
			If do_refresh Then
				'逕ｻ髱｢縺梧嶌縺肴鋤縺医ｉ繧後◆縺薙→繧定ｨ倬鹸
				ScreenIsSaved = False
				
				If .Visible Then
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.picMain(0).Refresh()
				End If
			End If
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, 0, 0, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
			
			'Invalid_string_refer_to_original_code
			Select Case u.Area
				Case "遨ｺ荳ｭ"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 28)
				Case "豌ｴ荳ｭ"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 3)
				Case "蝨ｰ荳ｭ"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 28)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 3)
				Case "Invalid_string_refer_to_original_code"
					If TerrainClass(u.X, u.Y) = "譛磯擇" Then
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = MoveToEx(pic.hDC, 0, 28, PT)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = LineTo(pic.hDC, 31, 28)
					End If
			End Select
			
			'Invalid_string_refer_to_original_code
			xx = MapToPixelX(x1)
			yy = MapToPixelY(y1)
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If u Is MapDataForUnit(x1, y1) Then
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (x1 - 1), 32 * (y1 - 1), SRCCOPY)
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(1).hDC, xx, yy, 32, 32, .picBack.hDC, 32 * (x1 - 1), 32 * (y1 - 1), SRCCOPY)
			End If
			
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			For i = 1 To division * MaxLng(System.Math.Abs(x2 - x1), System.Math.Abs(y2 - y1))
				'逕ｻ蜒上ｒ豸亥悉
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, .picMain(1).hDC, xx, yy, SRCCOPY)
				
				'Invalid_string_refer_to_original_code
				xx = xx + 32 * vx \ division
				yy = yy + 32 * vy \ division
				
				'逕ｻ蜒上ｒ謠冗判
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
			
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			For i = 1 To division * MinLng(System.Math.Abs(x2 - x1), System.Math.Abs(y2 - y1))
				'逕ｻ蜒上ｒ豸亥悉
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, .picMain(1).hDC, xx, yy, SRCCOPY)
				
				'Invalid_string_refer_to_original_code
				xx = xx + 32 * vx \ division
				yy = yy + 32 * vy \ division
				
				'逕ｻ蜒上ｒ謠冗判
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
		
		'逕ｻ髱｢縺梧嶌縺肴鋤縺医ｉ繧後◆縺薙→繧定ｨ倬鹸
		ScreenIsSaved = False
	End Sub
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, 0, 0, 32, 32, .picUnitBitmap.hDC, 32 * (u.BitmapID Mod 15), 96 * (u.BitmapID \ 15), SRCCOPY)
			
			'Invalid_string_refer_to_original_code
			Select Case u.Area
				Case "遨ｺ荳ｭ"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 28)
				Case "豌ｴ荳ｭ"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 3)
				Case "蝨ｰ荳ｭ"
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 28, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 28)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = MoveToEx(pic.hDC, 0, 3, PT)
					'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = LineTo(pic.hDC, 31, 3)
				Case "Invalid_string_refer_to_original_code"
					If TerrainClass(u.X, u.Y) = "譛磯擇" Then
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = MoveToEx(pic.hDC, 0, 28, PT)
						'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
						ret = LineTo(pic.hDC, 31, 28)
					End If
			End Select
			
			'遘ｻ蜍慕ｵ瑚ｷｯ繧呈､懃ｴ｢
			SearchMoveRoute((u.X), (u.Y), move_route_x, move_route_y)
			
			If wait_time > 0 Then
				start_time = timeGetTime()
			End If
			
			'Invalid_string_refer_to_original_code
			xx = MapToPixelX(move_route_x(UBound(move_route_x)))
			yy = MapToPixelY(move_route_y(UBound(move_route_y)))
			
			i = UBound(move_route_x) - 1
			Do While i > 0
				vx = MapToPixelX(move_route_x(i)) - xx
				vy = MapToPixelY(move_route_y(i)) - yy
				
				'Invalid_string_refer_to_original_code
				For j = 1 To division
					'逕ｻ蜒上ｒ豸亥悉
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = BitBlt(.picMain(0).hDC, xx, yy, 32, 32, .picMain(1).hDC, xx, yy, SRCCOPY)
					
					'Invalid_string_refer_to_original_code
					xx = xx + vx \ division
					yy = yy + vy \ division
					
					'逕ｻ蜒上ｒ謠冗判
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
		
		'逕ｻ髱｢縺梧嶌縺肴鋤縺医ｉ繧後◆縺薙→繧定ｨ倬鹸
		ScreenIsSaved = False
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Function ListBox(ByRef lb_caption As String, ByRef list() As String, ByRef lb_info As String, Optional ByRef lb_mode As String = "") As Short
		Dim i As Short
		Dim is_rbutton_released As Boolean
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmListBox)
		With frmListBox
			.WindowState = System.Windows.Forms.FormWindowState.Normal
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If Not .txtComment.Enabled Then
				.txtComment.Enabled = True
				.txtComment.Visible = True
				.txtComment.Width = .labCaption.Width
				.txtComment.Text = ""
				.txtComment.Top = .lstItems.Top + .lstItems.Height + 5
				.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) + 600)
			End If
			If .txtComment.Enabled Then
				.txtComment.Enabled = False
				.txtComment.Visible = False
				.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - 600)
			End If
			'End If
			
			'繧ｭ繝｣繝励す繝ｧ繝ｳ
			.Text = lb_caption
			If UBound(ListItemFlag) > 0 Then
				.labCaption.Text = "  " & lb_info
			Else
				.labCaption.Text = lb_info
			End If
			
			'Invalid_string_refer_to_original_code
			.lstItems.Visible = False
			.lstItems.Items.Clear()
			If UBound(ListItemFlag) > 0 Then
				For i = 1 To UBound(list)
					If ListItemFlag(i) Then
						.lstItems.Items.Add("Invalid_string_refer_to_original_code")
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
			
			'Invalid_string_refer_to_original_code
			If UBound(ListItemComment) <> UBound(list) Then
				ReDim Preserve ListItemComment(UBound(list))
			End If
			
			'Invalid_string_refer_to_original_code
			If .WindowState <> System.Windows.Forms.FormWindowState.Normal Then
				.WindowState = System.Windows.Forms.FormWindowState.Normal
				.Show()
			End If
			
			'Invalid_string_refer_to_original_code
			If MainForm.Visible And .HorizontalSize = "S" Then
				.Left = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(MainForm.Left))
			Else
				.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
			End If
			If MainForm.Visible And Not MainForm.WindowState = 1 And .VerticalSize = "M" And InStr(lb_mode, "荳ｭ螟ｮ陦ｨ遉ｺ") = 0 Then
				.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(MainForm.Top) + VB6.PixelsToTwipsY(MainForm.Height) - VB6.PixelsToTwipsY(.Height))
			Else
				.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			End If
			
			'Invalid_string_refer_to_original_code
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
			
			'繧ｳ繝｡繝ｳ繝医え繧｣繝ｳ繝峨え縺ｮ陦ｨ遉ｺ
			If .txtComment.Enabled Then
				.txtComment.Text = ListItemComment(.lstItems.SelectedIndex + 1)
			End If
			
			SelectedItem = 0
			
			IsFormClicked = False
			System.Windows.Forms.Application.DoEvents()
			
			'Invalid_string_refer_to_original_code
			If InStr(lb_mode, "陦ｨ遉ｺ縺ｮ縺ｿ") > 0 Then
				'陦ｨ遉ｺ縺ｮ縺ｿ繧定｡後≧
				IsMordal = False
				.Show()
				.lstItems.Focus()
				Call SetWindowPos(.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
				.Refresh()
				Exit Function
			ElseIf InStr(lb_mode, "騾｣邯夊｡ｨ遉ｺ") > 0 Then 
				'Invalid_string_refer_to_original_code
				IsMordal = False
				If Not .Visible Then
					.Show()
					Call SetWindowPos(.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
					.lstItems.Focus()
				End If
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If AutoMoveCursor Then
					MoveCursorPos("繝繧､繧｢繝ｭ繧ｰ")
				End If
			End If
			
			Do Until IsFormClicked
				System.Windows.Forms.Application.DoEvents()
				'Invalid_string_refer_to_original_code
				If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
					is_rbutton_released = True
				Else
					If is_rbutton_released Then
						IsFormClicked = True
					End If
				End If
				Sleep(50)
			Loop 
			'Invalid_string_refer_to_original_code
			IsMordal = False
			.Show()
			Call SetWindowPos(.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
			.lstItems.Focus()
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If AutoMoveCursor Then
				MoveCursorPos("繝繧､繧｢繝ｭ繧ｰ")
			End If
			'End If
			
			Do Until IsFormClicked
				System.Windows.Forms.Application.DoEvents()
				'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If AutoMoveCursor Then
				RestoreCursorPos()
			End If
			'End If
			
			If .txtComment.Enabled Then
				.txtComment.Enabled = False
				.txtComment.Visible = False
				.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - 600)
			End If
			'End If
			
			ListBox = SelectedItem
			System.Windows.Forms.Application.DoEvents()
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Sub AddPartsToListBox()
		Dim ret As Integer
		Dim fname As String
		Dim u, t As Unit
		
		u = SelectedUnit
		t = SelectedTarget
		
		With frmListBox
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ picUnit1.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(.picUnit1.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * (u.X - 1), 32 * (u.Y - 1), SRCCOPY)
				End If
			Else
				LoadUnitBitmap(u, .picUnit1, 0, 0, True)
			End If
			.picUnit1.Refresh()
			
			If u.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
			If u.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
			'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: PictureBox プロパティ picUnit2.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					ret = BitBlt(.picUnit2.hDC, 0, 0, 32, 32, MainForm.picBack.hDC, 32 * (t.X - 1), 32 * (t.Y - 1), SRCCOPY)
				End If
			Else
				LoadUnitBitmap(t, .picUnit2, 0, 0, True)
			End If
			.picUnit2.Refresh()
			
			If t.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
			If t.IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
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
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		'騾壼ｸｸ縺ｮ豁ｦ蝎ｨ驕ｸ謚樊凾縺ｮ陦ｨ遉ｺ
		For i = 1 To u.CountWeapon
			w = warray(i)
			
			With u
				If lb_mode = "荳隕ｧ" Then
					If Not .IsWeaponAvailable(w, "Invalid_string_refer_to_original_code") Then
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						If .IsDisabled((.Weapon(w).Name)) Then
							GoTo NextLoop1
						End If
						If Not .IsWeaponMastered(w) Then
							GoTo NextLoop1
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						If Not .IsCombinationAttackAvailable(w, True) Then
							GoTo NextLoop1
						End If
					End If
				End If
				ListItemFlag(UBound(list) + 1) = False
				If .IsWeaponUseful(w, lb_mode) Then
					ListItemFlag(UBound(list) + 1) = False
				Else
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					If .IsDisabled((.Weapon(w).Name)) Then
						GoTo NextLoop1
					End If
					If Not .IsWeaponMastered(w) Then
						GoTo NextLoop1
					End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If Not .IsCombinationAttackAvailable(w, True) Then
						GoTo NextLoop1
					End If
				End If
				ListItemFlag(UBound(list) + 1) = True
				'End If
				'End If
			End With
			
			ReDim Preserve list(UBound(list) + 1)
			ReDim Preserve wlist(UBound(list))
			wlist(UBound(list)) = w
			
			'Invalid_string_refer_to_original_code
			With u.Weapon(w)
				'Invalid_string_refer_to_original_code
				If wpower(w) < 10000 Then
					list(UBound(list)) = RightPaddedString(.Nickname, 27) & LeftPaddedString(VB6.Format(wpower(w)), 4)
				Else
					list(UBound(list)) = RightPaddedString(.Nickname, 26) & LeftPaddedString(VB6.Format(wpower(w)), 5)
				End If
				
				'Invalid_string_refer_to_original_code
				If u.WeaponMaxRange(w) > 1 Then
					buf = VB6.Format(.MinRange) & "-" & VB6.Format(u.WeaponMaxRange(w))
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(buf, 5)
				Else
					list(UBound(list)) = list(UBound(list)) & "    1"
				End If
				
				'Invalid_string_refer_to_original_code
				If u.WeaponPrecision(w) >= 0 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString("+" & VB6.Format(u.WeaponPrecision(w)), 4)
				Else
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponPrecision(w)), 4)
				End If
				
				'谿九ｊ蠑ｾ謨ｰ
				If .Bullet > 0 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.Bullet(w)), 3)
				Else
					list(UBound(list)) = list(UBound(list)) & "  -"
				End If
				
				'Invalid_string_refer_to_original_code
				If .ENConsumption > 0 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponENConsumption(w)), 4)
				Else
					list(UBound(list)) = list(UBound(list)) & "   -"
				End If
				
				'Invalid_string_refer_to_original_code
				If u.WeaponCritical(w) >= 0 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString("+" & VB6.Format(u.WeaponCritical(w)), 4)
				Else
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponCritical(w)), 4)
				End If
				
				'Invalid_string_refer_to_original_code
				list(UBound(list)) = list(UBound(list)) & " " & .Adaption
				
				'Invalid_string_refer_to_original_code
				If .NecessaryMorale > 0 Then
					list(UBound(list)) = list(UBound(list)) & "Invalid_string_refer_to_original_code"
				End If
				
				'螻樊ｧ
				wclass = u.WeaponClass(w)
				If InStrNotNest(wclass, "|") > 0 Then
					wclass = Left(wclass, InStrNotNest(wclass, "|") - 1)
				End If
				list(UBound(list)) = list(UBound(list)) & " " & wclass
			End With
NextLoop1: 
		Next 
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		If Not u.LookForSupportAttack(Nothing) Is Nothing Then
			'Invalid_string_refer_to_original_code
			UseSupportAttack = True
			ReDim Preserve list(UBound(list) + 1)
			ReDim Preserve ListItemFlag(UBound(list))
			list(UBound(list)) = "Invalid_string_refer_to_original_code"
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		TopItem = -1
		'Invalid_string_refer_to_original_code_
		'"陦ｨ遉ｺ縺ｮ縺ｿ")
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		
		If AutoMoveCursor Then
			If lb_mode <> "荳隕ｧ" Then
				MoveCursorPos("Invalid_string_refer_to_original_code")
			Else
				MoveCursorPos("繝繧､繧｢繝ｭ繧ｰ")
			End If
		End If
		If BGM <> "" Then
			ChangeBGM(BGM)
		End If
		
		Do While True
			Do Until IsFormClicked
				System.Windows.Forms.Application.DoEvents()
				'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				UseSupportAttack = Not UseSupportAttack
				If UseSupportAttack Then
					list(UBound(list)) = "Invalid_string_refer_to_original_code"
				Else
					list(UBound(list)) = "Invalid_string_refer_to_original_code"
				End If
				
				'Invalid_string_refer_to_original_code_
				'"陦ｨ遉ｺ縺ｮ縺ｿ")
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			End If
		Loop 
		
		If lb_mode <> "荳隕ｧ" Then
			frmListBox.Hide()
		End If
		ReDim ListItemComment(0)
		WeaponListBox = wlist(SelectedItem)
		
		'UPGRADE_WARNING: WeaponListBox に変換されていないステートメントがあります。ソース コードを確認してください。
		'蜿肴茶豁ｦ蝎ｨ驕ｸ謚樊凾縺ｮ陦ｨ遉ｺ
		
		For i = 1 To u.CountWeapon
			w = warray(i)
			
			With u
				'Invalid_string_refer_to_original_code
				If .IsDisabled((.Weapon(w).Name)) Then
					GoTo NextLoop2
				End If
				
				'Invalid_string_refer_to_original_code
				If Not .IsWeaponMastered(w) Then
					GoTo NextLoop2
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				If Not .IsCombinationAttackAvailable(w, True) Then
					GoTo NextLoop2
				End If
				'End If
				
				If Not .IsWeaponAvailable(w, "遘ｻ蜍募燕") Then
					'Invalid_string_refer_to_original_code
					ListItemFlag(UBound(list) + 1) = True
				ElseIf Not .IsTargetWithinRange(w, SelectedUnit) Then 
					'Invalid_string_refer_to_original_code
					ListItemFlag(UBound(list) + 1) = True
				ElseIf .IsWeaponClassifiedAs(w, "Invalid_string_refer_to_original_code") Then 
					'Invalid_string_refer_to_original_code
					ListItemFlag(UBound(list) + 1) = True
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'Invalid_string_refer_to_original_code
					ListItemFlag(UBound(list) + 1) = True
				ElseIf .Damage(w, SelectedUnit, True) > 0 Then 
					'Invalid_string_refer_to_original_code
					ListItemFlag(UBound(list) + 1) = False
				ElseIf Not .IsNormalWeapon(w) And .CriticalProbability(w, SelectedUnit) > 0 Then 
					'Invalid_string_refer_to_original_code
					ListItemFlag(UBound(list) + 1) = False
				Else
					'Invalid_string_refer_to_original_code
					ListItemFlag(UBound(list) + 1) = True
				End If
			End With
			
			ReDim Preserve list(UBound(list) + 1)
			ReDim Preserve wlist(UBound(list))
			wlist(UBound(list)) = w
			
			'Invalid_string_refer_to_original_code
			With u.Weapon(w)
				'Invalid_string_refer_to_original_code
				list(UBound(list)) = RightPaddedString(.Nickname, 29) & LeftPaddedString(VB6.Format(wpower(w)), 4)
				
				'Invalid_string_refer_to_original_code
				If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
					buf = VB6.Format(MinLng(u.HitProbability(w, SelectedUnit, True), 100)) & "%"
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(buf, 5)
				ElseIf u.WeaponPrecision(w) >= 0 Then 
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString("+" & VB6.Format(u.WeaponPrecision(w)), 5)
				Else
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponPrecision(w)), 5)
				End If
				
				
				'Invalid_string_refer_to_original_code
				If Not IsOptionDefined("Invalid_string_refer_to_original_code") Then
					buf = VB6.Format(MinLng(u.CriticalProbability(w, SelectedUnit), 100)) & "%"
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(buf, 5)
				ElseIf u.WeaponCritical(w) >= 0 Then 
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString("+" & VB6.Format(u.WeaponCritical(w)), 5)
				Else
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponCritical(w)), 5)
				End If
				
				'谿九ｊ蠑ｾ謨ｰ
				If .Bullet > 0 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.Bullet(w)), 3)
				Else
					list(UBound(list)) = list(UBound(list)) & "  -"
				End If
				
				'Invalid_string_refer_to_original_code
				If .ENConsumption > 0 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.WeaponENConsumption(w)), 4)
				Else
					list(UBound(list)) = list(UBound(list)) & "   -"
				End If
				
				'Invalid_string_refer_to_original_code
				list(UBound(list)) = list(UBound(list)) & " " & .Adaption
				
				'Invalid_string_refer_to_original_code
				If .NecessaryMorale > 0 Then
					list(UBound(list)) = list(UBound(list)) & "Invalid_string_refer_to_original_code"
				End If
				
				'螻樊ｧ
				wclass = u.WeaponClass(w)
				If InStrNotNest(wclass, "|") > 0 Then
					wclass = Left(wclass, InStrNotNest(wclass, "|") - 1)
				End If
				list(UBound(list)) = list(UBound(list)) & " " & wclass
			End With
NextLoop2: 
		Next 
		
		'Invalid_string_refer_to_original_code
		TopItem = -1
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		WeaponListBox = wlist(ret)
		'End If
		
		System.Windows.Forms.Application.DoEvents()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function AbilityListBox(ByRef u As Unit, ByRef caption_msg As String, ByRef lb_mode As String, Optional ByVal is_item As Boolean = False) As Short
		Dim j, i, k As Short
		Dim ret As Short
		Dim msg, buf, rest_msg As String
		Dim list() As String
		Dim alist() As Short
		Dim is_available As Boolean
		Dim is_rbutton_released As Boolean
		
		With u
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control mnuUnitCommandItem は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			If lb_mode <> "荳隕ｧ" And Not is_item And MainForm.mnuUnitCommandItem(AbilityCmdID).Caption <> Term("Invalid_string_refer_to_original_code", u) Then
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
				If lb_mode = "荳隕ｧ" Then
					If .IsAbilityAvailable(i, "Invalid_string_refer_to_original_code") Then
						'Invalid_string_refer_to_original_code
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
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						If .IsDisabled((.Ability(i).Name)) Then
							GoTo NextLoop
						End If
						If Not .IsAbilityMastered(i) Then
							GoTo NextLoop
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						If Not .IsCombinationAbilityAvailable(i, True) Then
							GoTo NextLoop
						End If
					End If
				End If
				'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					If .IsDisabled((.Ability(i).Name)) Then
						GoTo NextLoop
					End If
					If Not .IsAbilityMastered(i) Then
						GoTo NextLoop
					End If
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					If Not .IsCombinationAbilityAvailable(i, True) Then
						GoTo NextLoop
					End If
				End If
				is_available = False
				'End If
				'End If
				
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
						If .EffectType(j) = "隗｣隱ｬ" Then
							msg = .EffectName(j)
							Exit For
						ElseIf InStr(.EffectName(j), "繧ｿ繝ｼ繝ｳ)") > 0 Then 
							'Invalid_string_refer_to_original_code
							k = InStr(msg, Mid(.EffectName(j), InStr(.EffectName(j), "(")))
							If k > 0 Then
								msg = Left(msg, k - 1) & "Invalid_string_refer_to_original_code"
								& Left$(.EffectName(j), InStr(.EffectName(j), "(") - 1) _
								& Mid$(msg, k)
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							Else
								msg = msg & " " & .EffectName(j)
							End If
						ElseIf .EffectName(j) <> "" Then 
							msg = msg & " " & .EffectName(j)
						End If
					Next 
					msg = Trim(msg)
					
					'Invalid_string_refer_to_original_code
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
					
					'Invalid_string_refer_to_original_code
					If u.AbilityMaxRange(i) > 1 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.AbilityMinRange(i)) & "-" & VB6.Format(u.AbilityMaxRange(i)), 4)
					ElseIf u.AbilityMaxRange(i) = 1 Then 
						list(UBound(list)) = list(UBound(list)) & "   1"
					Else
						list(UBound(list)) = list(UBound(list)) & "   -"
					End If
					
					'谿九ｊ菴ｿ逕ｨ蝗樊焚
					If .Stock > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.Stock(i)), 3)
					Else
						list(UBound(list)) = list(UBound(list)) & "  -"
					End If
					
					'Invalid_string_refer_to_original_code
					If .ENConsumption > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(u.AbilityENConsumption(i)), 4)
					Else
						list(UBound(list)) = list(UBound(list)) & "   -"
					End If
					
					'Invalid_string_refer_to_original_code
					If .NecessaryMorale > 0 Then
						list(UBound(list)) = list(UBound(list)) & "Invalid_string_refer_to_original_code"
					End If
					
					'螻樊ｧ
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
		
		'Invalid_string_refer_to_original_code
		TopItem = -1
		'Invalid_string_refer_to_original_code_
		'"陦ｨ遉ｺ縺ｮ縺ｿ")
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		
		If AutoMoveCursor Then
			MoveCursorPos("繝繧､繧｢繝ｭ繧ｰ")
		End If
		
		Do Until IsFormClicked
			System.Windows.Forms.Application.DoEvents()
			'Invalid_string_refer_to_original_code
			If (GetAsyncKeyState(RButtonID) And &H8000) = 0 Then
				is_rbutton_released = True
			Else
				If is_rbutton_released Then
					IsFormClicked = True
				End If
			End If
		Loop 
		If lb_mode <> "荳隕ｧ" Then
			frmListBox.Hide()
		End If
		ReDim ListItemComment(0)
		AbilityListBox = alist(SelectedItem)
		
		System.Windows.Forms.Application.DoEvents()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function LIPS(ByRef lb_caption As String, ByRef list() As String, ByRef lb_info As String, ByVal time_limit As Short) As Short
		Dim i As Short
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmListBox)
		With frmListBox
			'Invalid_string_refer_to_original_code
			.Text = lb_caption
			.labCaption.Text = "  " & lb_info
			.lstItems.Items.Clear()
			For i = 1 To UBound(list)
				.lstItems.Items.Add("  " & list(i))
			Next 
			.lstItems.SelectedIndex = 0
			.lstItems.Height = 86
			
			'Invalid_string_refer_to_original_code
			.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
			If MainForm.Visible = True And Not MainForm.WindowState = 1 Then
				.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(MainForm.Top) + VB6.PixelsToTwipsY(MainForm.Height) - VB6.PixelsToTwipsY(.Height))
			Else
				.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			End If
			
			'蜈･蜉帛宛髯先凾髢薙↓髢｢縺吶ｋ險ｭ螳壹ｒ陦後≧
			.CurrentTime = 0
			.TimeLimit = time_limit
			.picBar.Visible = True
			.Timer1.Enabled = True
			
			'Invalid_string_refer_to_original_code
			SelectedItem = 0
			IsFormClicked = False
			.ShowDialog()
			.CurrentTime = 0
			LIPS = SelectedItem
			
			'Invalid_string_refer_to_original_code
			.lstItems.Height = 100
			.picBar.Visible = False
			.Timer1.Enabled = False
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function MultiColumnListBox(ByRef lb_caption As String, ByRef list() As String, ByVal is_center As Boolean) As Short
		Dim i As Short
		
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmMultiColumnListBox)
		With frmMultiColumnListBox
			.Text = lb_caption
			.lstItems.Visible = False
			.lstItems.Items.Clear()
			
			'Invalid_string_refer_to_original_code
			For i = 1 To UBound(list)
				If ListItemFlag(i) Then
					.lstItems.Items.Add("Invalid_string_refer_to_original_code")
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
			
			'Invalid_string_refer_to_original_code
			.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(.Width)) / 2)
			If MainForm.Visible = True And Not MainForm.WindowState = 1 And Not is_center Then
				.Top = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(MainForm.Top) + VB6.PixelsToTwipsY(MainForm.Height) - VB6.PixelsToTwipsY(.Height))
			Else
				.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			End If
			
			'Invalid_string_refer_to_original_code
			If TopItem > 0 Then
				If .lstItems.TopIndex <> TopItem - 1 Then
					.lstItems.TopIndex = MinLng(TopItem, .lstItems.Items.Count) - 1
				End If
			End If
			
			SelectedItem = 0
			
			System.Windows.Forms.Application.DoEvents()
			IsFormClicked = False
			
			'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Function MultiSelectListBox(ByVal lb_caption As String, ByRef list() As String, ByVal lb_info As String, ByVal max_num As Short) As Short
		Dim i, j As Short
		
		'Invalid_string_refer_to_original_code
		CommandState = "Invalid_string_refer_to_original_code"
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmMultiSelectListBox)
		With frmMultiSelectListBox
			.Text = lb_caption
			.lblCaption.Text = "縲" & lb_info
			MaxListItem = max_num
			For i = 1 To UBound(list)
				.lstItems.Items.Add("縲" & list(i))
			Next 
			.cmdSort.Text = "Invalid_string_refer_to_original_code"
			.Left = VB6.TwipsToPixelsX(VB6.PixelsToTwipsX(MainForm.Left))
			.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(.Height)) / 2)
			.ShowDialog()
		End With
		
		'Invalid_string_refer_to_original_code
		j = 0
		For i = 1 To UBound(list)
			If ListItemFlag(i) Then
				j = j + 1
			End If
		Next 
		MultiSelectListBox = j
		
		'Invalid_string_refer_to_original_code
		frmMultiSelectListBox.Close()
		'UPGRADE_NOTE: オブジェクト frmMultiSelectListBox をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		frmMultiSelectListBox = Nothing
	End Function
	
	
	'Invalid_string_refer_to_original_code
	
	'逕ｻ蜒上ｒ繧ｦ繧｣繝ｳ繝峨え縺ｫ謠冗判
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
		
		'Invalid_string_refer_to_original_code
		If Not init_draw_pitcure Then
			'Invalid_string_refer_to_original_code
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
			
			'逕ｻ髱｢縺ｮ濶ｲ謨ｰ繧貞盾辣ｧ
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			display_byte_pixel = GetDeviceCaps(MainForm.picMain(0).hDC, BITSPIXEL) \ 8
			
			init_draw_pitcure = True
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case fname
			Case "", "-.bmp", "EFFECT_Void.bmp"
				Exit Function
		End Select
		
		'Debug.Print fname, draw_option
		
		'Invalid_string_refer_to_original_code
		BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'Invalid_string_refer_to_original_code
		pic_option = ""
		'繝槭せ繧ｯ逕ｻ蜒上↓蠖ｱ髻ｿ縺吶ｋ繧ｪ繝励す繝ｧ繝ｳ
		pic_option2 = ""
		'Invalid_string_refer_to_original_code
		trans_par = -1
		i = 1
		Do While i <= LLength(draw_option)
			opt = LIndex(draw_option, i)
			Select Case opt
				Case "閭梧勹"
					permanent = True
					'Invalid_string_refer_to_original_code
					Select Case MapDrawMode
						Case "Invalid_string_refer_to_original_code"
							dark_count = dark_count + 1
							pic_option = pic_option & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							is_monotone = True
							pic_option = pic_option & "Invalid_string_refer_to_original_code"
						Case "繧ｻ繝斐い"
							is_sepia = True
							pic_option = pic_option & " 繧ｻ繝斐い"
						Case "Invalid_string_refer_to_original_code"
							is_sunset = True
							pic_option = pic_option & "Invalid_string_refer_to_original_code"
						Case "豌ｴ荳ｭ"
							is_water = True
							pic_option = pic_option & " 豌ｴ荳ｭ"
						Case "繝輔ぅ繝ｫ繧ｿ"
							is_colorfilter = True
							fcolor = MapDrawFilterColor
							pic_option2 = pic_option2 & " 繝輔ぅ繝ｫ繧ｿ=" & CStr(MapDrawFilterColor)
					End Select
				Case "騾城℃"
					transparent = True
					pic_option = pic_option & " " & opt
				Case "Invalid_string_refer_to_original_code"
					is_monotone = True
					pic_option = pic_option & " " & opt
				Case "繧ｻ繝斐い"
					is_sepia = True
					pic_option = pic_option & " " & opt
				Case "Invalid_string_refer_to_original_code"
					is_sunset = True
					pic_option = pic_option & " " & opt
				Case "豌ｴ荳ｭ"
					is_water = True
					pic_option = pic_option & " " & opt
				Case "Invalid_string_refer_to_original_code"
					bright_count = bright_count + 1
					pic_option = pic_option & " " & opt
				Case "Invalid_string_refer_to_original_code"
					dark_count = dark_count + 1
					pic_option = pic_option & " " & opt
				Case "蟾ｦ蜿ｳ蜿崎ｻ｢"
					hrev = True
					pic_option2 = pic_option2 & " " & opt
				Case "荳贋ｸ句渚霆｢"
					vrev = True
					pic_option2 = pic_option2 & " " & opt
				Case "繝阪ぎ繝昴ず蜿崎ｻ｢"
					negpos = True
					pic_option = pic_option & " " & opt
				Case "Invalid_string_refer_to_original_code"
					is_sil = True
					pic_option = pic_option & " " & opt
				Case "Invalid_string_refer_to_original_code"
					top_part = True
					pic_option2 = pic_option2 & " " & opt
				Case "Invalid_string_refer_to_original_code"
					bottom_part = True
					pic_option2 = pic_option2 & " " & opt
				Case "Invalid_string_refer_to_original_code"
					right_part = True
					pic_option2 = pic_option2 & " " & opt
				Case "Invalid_string_refer_to_original_code"
					left_part = True
					pic_option2 = pic_option2 & " " & opt
				Case "Invalid_string_refer_to_original_code"
					tright_part = True
					pic_option2 = pic_option2 & " " & opt
				Case "Invalid_string_refer_to_original_code"
					tleft_part = True
					pic_option2 = pic_option2 & " " & opt
				Case "Invalid_string_refer_to_original_code"
					bright_part = True
					pic_option2 = pic_option2 & " " & opt
				Case "Invalid_string_refer_to_original_code"
					bleft_part = True
					pic_option2 = pic_option2 & " " & opt
				Case "Invalid_string_refer_to_original_code"
					on_msg_window = True
				Case "Invalid_string_refer_to_original_code"
					on_status_window = True
				Case "菫晄戟"
					keep_picture = True
				Case "蜿ｳ蝗櫁ｻ｢"
					i = i + 1
					angle = StrToLng(LIndex(draw_option, i))
					pic_option2 = pic_option2 & " 蜿ｳ蝗櫁ｻ｢=" & VB6.Format(angle Mod 360)
				Case "蟾ｦ蝗櫁ｻ｢"
					i = i + 1
					angle = -StrToLng(LIndex(draw_option, i))
					pic_option2 = pic_option2 & " 蜿ｳ蝗櫁ｻ｢=" & VB6.Format(angle Mod 360)
				Case "繝輔ぅ繝ｫ繧ｿ"
					is_colorfilter = True
				Case Else
					If Right(opt, 1) = "%" And IsNumeric(Left(opt, Len(opt) - 1)) Then
						trans_par = MaxDbl(0, MinDbl(1, CDbl(Left(opt, Len(opt) - 1)) / 100))
						pic_option2 = pic_option2 & " 繝輔ぅ繝ｫ繧ｿ騾城℃蠎ｦ=" & opt
					Else
						If is_colorfilter Then
							fcolor = CInt(opt)
							pic_option2 = pic_option2 & " 繝輔ぅ繝ｫ繧ｿ=" & opt
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
		
		'Invalid_string_refer_to_original_code
		If on_msg_window Then
			'Invalid_string_refer_to_original_code
			pic = frmMessage.picFace
			permanent = False
		ElseIf on_status_window Then 
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picUnitStatus は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = MainForm.picUnitStatus
		ElseIf permanent Then 
			'閭梧勹縺ｸ縺ｮ謠冗判
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = MainForm.picBack
		Else
			'繝槭ャ繝励え繧｣繝ｳ繝峨え縺ｸ縺ｮ騾壼ｸｸ縺ｮ謠冗判
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = MainForm.picMain(0)
			SaveScreen()
		End If
		
		'隱ｭ縺ｿ霎ｼ繧繝輔ぃ繧､繝ｫ縺ｮ謗｢邏｢
		
		'Invalid_string_refer_to_original_code
		If fname = last_fname Then
			'Invalid_string_refer_to_original_code
			If Not last_exists Then
				DrawPicture = False
				Exit Function
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		For i = 0 To ImageBufferSize - 1
			'Invalid_string_refer_to_original_code
			If PicBufFname(i) = fname Then
				'Invalid_string_refer_to_original_code
				If PicBufOption(i) = pic_option And PicBufOption2(i) = pic_option2 And Not PicBufIsMask(i) And PicBufDW(i) = dw And PicBufDH(i) = dh And PicBufSX(i) = sx And PicBufSY(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
					'Invalid_string_refer_to_original_code
					
					'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		For i = 0 To ImageBufferSize - 1
			'Invalid_string_refer_to_original_code
			If PicBufFname(i) = fname Then
				'Invalid_string_refer_to_original_code
				If PicBufOption(i) = pic_option And PicBufOption2(i) = pic_option2 And Not PicBufIsMask(i) And PicBufDW(i) = DEFAULT_LEVEL And PicBufDH(i) = DEFAULT_LEVEL And PicBufSX(i) = sx And PicBufSY(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
					'Invalid_string_refer_to_original_code
					
					'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If sw <> 0 Then
			For i = 0 To ImageBufferSize - 1
				'Invalid_string_refer_to_original_code
				If PicBufFname(i) = fname Then
					If PicBufOption(i) = "" And PicBufOption2(i) = "" And Not PicBufIsMask(i) And PicBufDW(i) = DEFAULT_LEVEL And PicBufDH(i) = DEFAULT_LEVEL And PicBufSX(i) = sx And PicBufSY(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
						'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		For i = 0 To ImageBufferSize - 1
			'Invalid_string_refer_to_original_code
			If PicBufFname(i) = fname Then
				If PicBufOption(i) = "" And PicBufOption2(i) = "" And Not PicBufIsMask(i) And PicBufDW(i) = DEFAULT_LEVEL And PicBufDH(i) = DEFAULT_LEVEL And PicBufSW(i) = 0 Then
					'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		Select Case LCase(fname)
			Case "black.bmp", "event\black.bmp"
				'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
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
				'蜻ｳ譁ｹ繝ｦ繝九ャ繝医ち繧､繝ｫ
				'UPGRADE_ISSUE: Control picUnit は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				orig_pic = MainForm.picUnit
				orig_width = 32
				orig_height = 32
				GoTo LoadedOrigPicture
			Case "common\effect_tile(enemy).bmp", "anime\common\effect_tile(enemy).bmp"
				'謨ｵ繝ｦ繝九ャ繝医ち繧､繝ｫ
				'UPGRADE_ISSUE: Control picEnemy は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				orig_pic = MainForm.picEnemy
				orig_width = 32
				orig_height = 32
				GoTo LoadedOrigPicture
			Case "common\effect_tile(neutral).bmp", "anime\common\effect_tile(neutral).bmp"
				'荳ｭ遶九Θ繝九ャ繝医ち繧､繝ｫ
				'UPGRADE_ISSUE: Control picNeautral は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				orig_pic = MainForm.picNeautral
				orig_width = 32
				orig_height = 32
				GoTo LoadedOrigPicture
		End Select
		
		'Invalid_string_refer_to_original_code
		If InStr(fname, ":") = 2 Then
			fpath = ""
			last_path = ""
			'逋ｻ骭ｲ繧帝∩縺代ｋ縺溘ａ
			in_history = True
			GoTo FoundPicture
		End If
		
		'Invalid_string_refer_to_original_code
		On Error GoTo NotFound
		'UPGRADE_WARNING: オブジェクト fpath_history.Item() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		fpath = fpath_history.Item(fname)
		last_path = ""
		
		'Invalid_string_refer_to_original_code
		On Error GoTo 0
		If fpath = "" Then
			'Invalid_string_refer_to_original_code
			last_fname = fname
			last_exists = False
			DrawPicture = False
			Exit Function
		End If
		in_history = True
		GoTo FoundPicture
		
NotFound: 
		
		'Invalid_string_refer_to_original_code
		On Error GoTo 0
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If Len(last_path) > 0 Then
			If FileExists(last_path & fname) Then
				fpath = last_path
				GoTo FoundPicture
			End If
		End If
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
			'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
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
		
		'繧ｷ繝翫Μ繧ｪ繝輔か繝ｫ繝
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
						'逋ｻ骭ｲ繧帝∩縺代ｋ縺溘ａ
						in_history = True
						GoTo FoundPicture
					End If
					If FileExists(ScenarioPath & "Bitmap\Map\" & tname) Then
						fname = tname
						fpath = ScenarioPath & "Bitmap\Map\"
						last_path = fpath
						'逋ｻ骭ｲ繧帝∩縺代ｋ縺溘ａ
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
						'逋ｻ骭ｲ繧帝∩縺代ｋ縺溘ａ
						in_history = True
						GoTo FoundPicture
					End If
					If FileExists(ExtDataPath & "Bitmap\Map\" & tname) Then
						fname = tname
						fpath = ExtDataPath & "Bitmap\Map\"
						last_path = ""
						'逋ｻ骭ｲ繧帝∩縺代ｋ縺溘ａ
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
						'逋ｻ骭ｲ繧帝∩縺代ｋ縺溘ａ
						in_history = True
						GoTo FoundPicture
					End If
					If FileExists(ExtDataPath2 & "Bitmap\Map\" & tname) Then
						fname = tname
						fpath = ExtDataPath2 & "Bitmap\Map\"
						last_path = ""
						'逋ｻ骭ｲ繧帝∩縺代ｋ縺溘ａ
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
		
		'Invalid_string_refer_to_original_code
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
				'逋ｻ骭ｲ繧帝∩縺代ｋ縺溘ａ
				in_history = True
				GoTo FoundPicture
			End If
			If FileExists(AppPath & "Bitmap\Map\" & tname) Then
				fname = tname
				fpath = AppPath & "Bitmap\Map\"
				last_path = ""
				'逋ｻ骭ｲ繧帝∩縺代ｋ縺溘ａ
				in_history = True
				GoTo FoundPicture
			End If
		End If
		If FileExists(AppPath & "Bitmap\Map\" & fname) Then
			fpath = AppPath & "Bitmap\Map\"
			last_path = ""
			GoTo FoundPicture
		End If
		
		'隕九▽縺九ｉ縺ｪ縺九▲縺溪ｦ窶ｦ
		
		'螻･豁ｴ縺ｫ險倬鹸縺励※縺翫￥
		fpath_history.Add("", fname)
		
		'陦ｨ遉ｺ繧剃ｸｭ豁｢
		last_fname = fname
		last_exists = False
		DrawPicture = False
		Exit Function
		
FoundPicture: 
		
		'繝輔ぃ繧､繝ｫ蜷阪ｒ險倬鹸縺励※縺翫￥
		last_fname = fname
		
		'螻･豁ｴ縺ｫ險倬鹸縺励※縺翫￥
		If Not in_history Then
			fpath_history.Add(fpath, fname)
		End If
		
		last_exists = True
		pfname = fpath & fname
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		With orig_pic
			PicBufSize(i) = display_byte_pixel * VB6.PixelsToTwipsX(.Width) * VB6.PixelsToTwipsY(.Height)
		End With
		
LoadedOrigPicture: 
		
		With orig_pic
			orig_width = VB6.PixelsToTwipsX(.Width)
			orig_height = VB6.PixelsToTwipsY(.Height)
		End With
		
		'Invalid_string_refer_to_original_code
		If sw <> 0 Then
			If sw <> orig_width Or sh <> orig_height Then
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If top_part Or bottom_part Or left_part Or right_part Or tleft_part Or tright_part Or bleft_part Or bright_part Or is_monotone Or is_sepia Or is_sunset Or is_water Or negpos Or is_sil Or vrev Or hrev Or bright_count > 0 Or dark_count > 0 Or angle Mod 360 <> 0 Or is_colorfilter Then
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		If top_part Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			orig_pic.Line (0, orig_height \ 2) - (orig_width - 1, orig_height - 1), BGColor, BF
		End If
		If bottom_part Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			orig_pic.Line (0, 0) - (orig_width - 1, orig_height \ 2 - 1), BGColor, BF
		End If
		If left_part Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			orig_pic.Line (orig_width \ 2, 0) - (orig_width - 1, orig_height - 1), BGColor, BF
		End If
		If right_part Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			orig_pic.Line (0, 0) - (orig_width \ 2 - 1, orig_height - 1), BGColor, BF
		End If
		If tleft_part Then
			'Invalid_string_refer_to_original_code
			For i = 0 To orig_width - 1
				'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				orig_pic.Line (i, orig_height - 1 - i) - (i, orig_height - 1), BGColor, B
			Next 
		End If
		If tright_part Then
			'Invalid_string_refer_to_original_code
			For i = 0 To orig_width - 1
				'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				orig_pic.Line (i, i) - (i, orig_height - 1), BGColor, B
			Next 
		End If
		If bleft_part Then
			'Invalid_string_refer_to_original_code
			For i = 0 To orig_width - 1
				'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				orig_pic.Line (i, 0) - (i, i), BGColor, B
			Next 
		End If
		If bright_part Then
			'Invalid_string_refer_to_original_code
			For i = 0 To orig_width - 1
				'UPGRADE_ISSUE: PictureBox メソッド orig_pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				orig_pic.Line (i, 0) - (i, orig_height - 1 - i), BGColor, B
			Next 
		End If
		
		'Invalid_string_refer_to_original_code
		If is_monotone Or is_sepia Or is_sunset Or is_water Or is_colorfilter Or bright_count > 0 Or dark_count > 0 Or negpos Or is_sil Or vrev Or hrev Or angle <> 0 Then
			'Invalid_string_refer_to_original_code
			If orig_width * orig_height Mod 4 <> 0 Then
				ErrorMessage(fname & "Invalid_string_refer_to_original_code")
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			GetImage(orig_pic)
			
			'Invalid_string_refer_to_original_code
			If is_monotone Then
				Monotone(transparent)
			End If
			
			'繧ｻ繝斐い
			If is_sepia Then
				Sepia(transparent)
			End If
			
			'Invalid_string_refer_to_original_code
			If is_sunset Then
				Sunset(transparent)
			End If
			
			'豌ｴ荳ｭ
			If is_water Then
				Water(transparent)
			End If
			
			'Invalid_string_refer_to_original_code
			If is_sil Then
				Silhouette()
			End If
			
			'繝阪ぎ繝昴ず蜿崎ｻ｢
			If negpos Then
				NegPosReverse(transparent)
			End If
			
			'繝輔ぅ繝ｫ繧ｿ
			If is_colorfilter Then
				If trans_par < 0 Then
					trans_par = 0.5
				End If
				ColorFilter(fcolor, trans_par, transparent)
			End If
			
			'Invalid_string_refer_to_original_code
			For i = 1 To bright_count
				Bright(transparent)
			Next 
			
			'Invalid_string_refer_to_original_code
			For i = 1 To dark_count
				Dark(transparent)
			Next 
			
			'蟾ｦ蜿ｳ蜿崎ｻ｢
			If vrev Then
				VReverse()
			End If
			
			'荳贋ｸ句渚霆｢
			If hrev Then
				HReverse()
			End If
			
			'蝗櫁ｻ｢
			If angle <> 0 Then
				'Invalid_string_refer_to_original_code
				'(騾｣邯壹〒蝗櫁ｻ｢縺輔○繧句ｴ蜷医↓謠冗判騾溷ｺｦ繧剃ｸ螳壹↓縺吶ｋ縺溘ａ)
				Rotate(angle, last_angle Mod 90 <> 0)
			End If
			
			'Invalid_string_refer_to_original_code
			SetImage(orig_pic)
			
			'Invalid_string_refer_to_original_code
			ClearImage()
		End If
		last_angle = angle
		
EditedPicture: 
		
		'Invalid_string_refer_to_original_code
		If dw = DEFAULT_LEVEL Then
			dw = orig_width
		End If
		If dh = DEFAULT_LEVEL Then
			dh = orig_height
		End If
		If permanent Then
			'閭梧勹謠冗判縺ｮ蝣ｴ蜷医√そ繝ｳ繧ｿ繝ｪ繝ｳ繧ｰ縺ｯ繝槭ャ繝嶺ｸｭ螟ｮ縺ｫ
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
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If InStr(fname, "EFFECT_") > 0 Or InStr(fname, "繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ\") > 0 Or InStr(fname, "邊ｾ逾槭さ繝槭Φ繝噂") > 0 Then
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
				'騾壼ｸｸ謠冗判縺ｮ蝣ｴ蜷医√そ繝ｳ繧ｿ繝ｪ繝ｳ繧ｰ縺ｯ逕ｻ髱｢荳ｭ螟ｮ縺ｫ
				If dx = DEFAULT_LEVEL Then
					dx = (MainPWidth - dw) \ 2
				End If
				If dy = DEFAULT_LEVEL Then
					dy = (MainPHeight - dh) \ 2
				End If
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		With pic
			If dx >= VB6.PixelsToTwipsX(.Width) Or dy >= VB6.PixelsToTwipsY(.Height) Or dx + dw <= 0 Or dy + dh <= 0 Or dw <= 0 Or dh <= 0 Then
				load_only = True
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		
		'逕ｻ髱｢縺ｫ謠冗判縺吶ｋ
		If Not transparent And dw = orig_width And dh = orig_height Then
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'逕ｻ蜒上ｒ謠冗判蜈医↓謠冗判
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, SRCCOPY)
		ElseIf KeepStretchedImage And Not transparent And (Not found_orig Or load_only) And dw <= 480 And dh <= 480 Then 
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			stretched_pic = MainForm.picBuf(i)
			With stretched_pic
				.Image = System.Drawing.Image.FromFile("")
				.Width = VB6.TwipsToPixelsX(dw)
				.Height = VB6.TwipsToPixelsY(dh)
			End With
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
			
			'Invalid_string_refer_to_original_code
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'諡｡螟ｧ縺励◆逕ｻ蜒上ｒ謠冗判蜈医↓謠冗判
			'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCCOPY)
		ElseIf Not transparent Then 
			'諡｡螟ｧ逕ｻ蜒上ｒ菴懊ｉ縺壹↓StretchBlt縺ｧ逶ｴ謗･諡｡螟ｧ謠冗判
			
			'Invalid_string_refer_to_original_code
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'諡｡螟ｧ縺励◆逕ｻ蜒上ｒ謠冗判蜈医↓謠冗判
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
		ElseIf UseTransparentBlt And (dw <> orig_width Or dh <> orig_height) And found_orig And Not load_only And (dw * dh < 40000 Or orig_width * orig_height > 40000) Then 
			'TransparentBlt縺ｮ譁ｹ縺碁ｫ倬溘↓謠冗判縺ｧ縺阪ｋ蝣ｴ蜷医↓髯舌ｊ
			'TransparentBlt繧剃ｽｿ縺｣縺ｦ諡｡螟ｧ騾城℃謠冗判
			
			'Invalid_string_refer_to_original_code
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'逕ｻ蜒上ｒ謠冗判蜈医↓騾城℃謠冗判
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = TransparentBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, BGColor)
		ElseIf dw = orig_width And dh = orig_height Then 
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			mask_pic.Image = Nothing
			For i = 0 To ImageBufferSize - 1
				'Invalid_string_refer_to_original_code
				If PicBufFname(i) = fname Then
					'Invalid_string_refer_to_original_code
					If PicBufIsMask(i) And PicBufOption2(i) = pic_option2 And PicBufDW(i) = orig_width And PicBufDH(i) = orig_height And PicBufSX(i) = sx And PicBufSX(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
						'Invalid_string_refer_to_original_code
						UsePicBuf(i)
						'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						mask_pic = MainForm.picBuf(i)
						'Debug.Print "Reuse " & Format$(i) & " As Mask"
						Exit For
					End If
				End If
			Next 
			
			If mask_pic Is Nothing Then
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				mask_pic = MainForm.picBuf(i)
				With mask_pic
					.Image = System.Drawing.Image.FromFile("")
					.Width = VB6.TwipsToPixelsX(orig_width)
					.Height = VB6.TwipsToPixelsY(orig_height)
				End With
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				MakeMask((orig_pic.hDC), (mask_pic.hDC), orig_width, orig_height, BGColor)
			End If
			
			'Invalid_string_refer_to_original_code
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'逕ｻ蜒上ｒ騾城℃謠冗判
			If BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
				'閭梧勹濶ｲ縺檎區
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, SRCINVERT)
			Else
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCAND)
				
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(mask_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, SRCINVERT)
				
				'Invalid_string_refer_to_original_code
				ReleasePicBuf(i)
			End If
		ElseIf KeepStretchedImage And (Not found_orig Or load_only) And dw <= 480 And dh <= 480 Then 
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			stretched_pic = MainForm.picBuf(i)
			With stretched_pic
				.Image = System.Drawing.Image.FromFile("")
				.Width = VB6.TwipsToPixelsX(dw)
				.Height = VB6.TwipsToPixelsY(dh)
			End With
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: オブジェクト stretched_mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			stretched_mask_pic.Image = Nothing
			For i = 0 To ImageBufferSize - 1
				'Invalid_string_refer_to_original_code
				If PicBufFname(i) = fname Then
					'Invalid_string_refer_to_original_code
					If PicBufIsMask(i) And PicBufOption2(i) = pic_option2 And PicBufDW(i) = dw And PicBufDH(i) = dh And PicBufSX(i) = sx And PicBufSY(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
						'Invalid_string_refer_to_original_code
						UsePicBuf(i)
						'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						stretched_mask_pic = MainForm.picBuf(i)
						'Debug.Print "Reuse " & Format$(i) & " As StretchedMask"
						Exit For
					End If
				End If
			Next 
			
			If stretched_mask_pic Is Nothing Then
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				mask_pic = MainForm.picTmp
				With mask_pic
					.Image = System.Drawing.Image.FromFile("")
					.Width = VB6.TwipsToPixelsX(orig_width)
					.Height = VB6.TwipsToPixelsY(orig_height)
				End With
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				MakeMask((orig_pic.hDC), (mask_pic.hDC), orig_width, orig_height, BGColor)
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				stretched_mask_pic = MainForm.picBuf(i)
				With stretched_mask_pic
					.Image = System.Drawing.Image.FromFile("")
					.Width = VB6.TwipsToPixelsX(dw)
					.Height = VB6.TwipsToPixelsY(dh)
				End With
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
			End If
			
			'Invalid_string_refer_to_original_code
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'逕ｻ蜒上ｒ騾城℃謠冗判
			If BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
				'閭梧勹濶ｲ縺檎區
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCINVERT)
			Else
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCAND)
				
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, stretched_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCINVERT)
				
				'Invalid_string_refer_to_original_code
				ReleasePicBuf(i)
			End If
		ElseIf dw <= 480 And dh <= 480 Then 
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			stretched_pic = MainForm.picStretchedTmp(0)
			With stretched_pic
				.Width = VB6.TwipsToPixelsX(dw)
				.Height = VB6.TwipsToPixelsY(dh)
			End With
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(stretched_pic.hDC, 0, 0, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			mask_pic.Image = Nothing
			For i = 0 To ImageBufferSize - 1
				'Invalid_string_refer_to_original_code
				If PicBufFname(i) = fname Then
					'Invalid_string_refer_to_original_code
					If PicBufIsMask(i) And PicBufOption2(i) = pic_option2 And PicBufDW(i) = orig_width And PicBufDH(i) = orig_height And PicBufSX(i) = sx And PicBufSX(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
						'Invalid_string_refer_to_original_code
						UsePicBuf(i)
						'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						mask_pic = MainForm.picBuf(i)
						'Debug.Print "Reuse " & Format$(i) & " As Mask"
						Exit For
					End If
				End If
			Next 
			
			If mask_pic Is Nothing Then
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				mask_pic = MainForm.picBuf(i)
				With mask_pic
					.Width = VB6.TwipsToPixelsX(orig_width)
					.Height = VB6.TwipsToPixelsY(orig_height)
				End With
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				MakeMask((orig_pic.hDC), (mask_pic.hDC), orig_width, orig_height, BGColor)
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picStretchedTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			stretched_mask_pic = MainForm.picStretchedTmp(1)
			With stretched_mask_pic
				.Image = System.Drawing.Image.FromFile("")
				.Width = VB6.TwipsToPixelsX(dw)
				.Height = VB6.TwipsToPixelsY(dh)
			End With
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			ret = StretchBlt(stretched_mask_pic.hDC, 0, 0, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCCOPY)
			
			'Invalid_string_refer_to_original_code
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'逕ｻ蜒上ｒ騾城℃謠冗判
			If BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
				'閭梧勹濶ｲ縺檎區
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_mask_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ stretched_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(pic.hDC, dx, dy, dw, dh, stretched_pic.hDC, 0, 0, SRCINVERT)
			Else
				'Invalid_string_refer_to_original_code
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
			
			'菴ｿ逕ｨ縺励◆荳譎ら判蜒城伜沺繧帝幕謾ｾ
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
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_NOTE: オブジェクト mask_pic をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			mask_pic.Image = Nothing
			For i = 0 To ImageBufferSize - 1
				'Invalid_string_refer_to_original_code
				If PicBufFname(i) = fname Then
					'Invalid_string_refer_to_original_code
					If PicBufIsMask(i) And PicBufOption2(i) = pic_option2 And PicBufDW(i) = orig_width And PicBufDH(i) = orig_height And PicBufSX(i) = sx And PicBufSX(i) = sy And PicBufSW(i) = sw And PicBufSH(i) = sh Then
						'Invalid_string_refer_to_original_code
						UsePicBuf(i)
						'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						mask_pic = MainForm.picBuf(i)
						'Debug.Print "Reuse " & Format$(i) & " As Mask"
						Exit For
					End If
				End If
			Next 
			
			If mask_pic Is Nothing Then
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				mask_pic = MainForm.picBuf(i)
				With mask_pic
					.Width = VB6.TwipsToPixelsX(orig_width)
					.Height = VB6.TwipsToPixelsY(orig_height)
				End With
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				MakeMask((orig_pic.hDC), (mask_pic.hDC), orig_width, orig_height, BGColor)
			End If
			
			'Invalid_string_refer_to_original_code
			If load_only Then
				DrawPicture = True
				Exit Function
			End If
			
			'逕ｻ蜒上ｒ騾城℃謠冗判
			If BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Then
				'閭梧勹濶ｲ縺檎區
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(pic.hDC, dx, dy, dw, dh, orig_pic.hDC, 0, 0, orig_width, orig_height, SRCINVERT)
			Else
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCAND)
				
				'UPGRADE_ISSUE: PictureBox プロパティ orig_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = BitBlt(mask_pic.hDC, 0, 0, orig_width, orig_width, orig_pic.hDC, 0, 0, SRCERASE)
				
				'UPGRADE_ISSUE: PictureBox プロパティ mask_pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				ret = StretchBlt(pic.hDC, dx, dy, dw, dh, mask_pic.hDC, 0, 0, orig_width, orig_height, SRCINVERT)
				
				'Invalid_string_refer_to_original_code
				ReleasePicBuf(i)
			End If
		End If
		
DrewPicture: 
		
		If permanent Then
			'閭梧勹縺ｸ縺ｮ謠上″霎ｼ縺ｿ
			IsMapDirty = True
			With MainForm
				'繝槭せ繧ｯ蜈･繧願レ譎ｯ逕ｻ蜒冗判髱｢縺ｫ繧ら判蜒上ｒ謠上″霎ｼ繧
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
			'Invalid_string_refer_to_original_code
			PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(dx, 0))
			PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(dy, 0))
			PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(dx + dw, MainPWidth - 1))
			PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(dy + dh, MainPHeight - 1))
			
			IsPictureDrawn = True
			IsPictureVisible = True
			IsCursorVisible = False
			
			If keep_picture Then
				'picMain(1)縺ｫ繧よ緒逕ｻ
				'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(MainForm.picMain(1).hDC, dx, dy, dw, dh, pic.hDC, dx, dy, SRCCOPY)
			End If
		End If
		
		DrawPicture = True
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub MakePicBuf()
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		With MainForm
			For i = 1 To ImageBufferSize - 1
				'UPGRADE_ISSUE: Control picBuf は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
				Load(.picBuf(i))
			Next 
		End With
		
		'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Private Function GetPicBuf(Optional ByVal buf_size As Integer = 0) As Short
		Dim total_size As Integer
		Dim oldest_buf, used_buf_num As Short
		Dim i As Short
		Dim tmp As Integer
		
		'Invalid_string_refer_to_original_code
		total_size = buf_size
		For i = 0 To ImageBufferSize - 1
			total_size = total_size + PicBufSize(i)
			If PicBufFname(i) <> "" Then
				used_buf_num = used_buf_num + 1
			End If
		Next 
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		Do While total_size > MaxImageBufferByteSize And used_buf_num > 4
			'Invalid_string_refer_to_original_code
			tmp = 100000000
			For i = 0 To ImageBufferSize - 1
				If PicBufFname(i) <> "" Then
					If PicBufDate(i) < tmp Then
						oldest_buf = i
						tmp = PicBufDate(i)
					End If
				End If
			Next 
			
			'繝舌ャ繝輔ぃ繧帝幕謾ｾ
			ReleasePicBuf(oldest_buf)
			used_buf_num = used_buf_num - 1
			
			'邱上し繧､繧ｺ謨ｰ繧呈ｸ帛ｰ代＆縺帙ｋ
			total_size = total_size - PicBufSize(oldest_buf)
			PicBufSize(oldest_buf) = 0
		Loop 
		
		'Invalid_string_refer_to_original_code
		GetPicBuf = 0
		For i = 1 To ImageBufferSize - 1
			If PicBufDate(i) < PicBufDate(GetPicBuf) Then
				GetPicBuf = i
			End If
		Next 
		
		'Invalid_string_refer_to_original_code
		PicBufSize(GetPicBuf) = buf_size
		
		'菴ｿ逕ｨ縺吶ｋ縺薙→繧定ｨ倬鹸縺吶ｋ
		UsePicBuf(GetPicBuf)
	End Function
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Private Sub UsePicBuf(ByVal idx As Short)
		PicBufDateCount = PicBufDateCount + 1
		PicBufDate(idx) = PicBufDateCount
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Sub DrawString(ByRef msg As String, ByVal X As Integer, ByVal Y As Integer, Optional ByVal without_cr As Boolean = False)
		Dim tx, ty As Short
		Dim prev_cx As Short
		Dim pic As System.Windows.Forms.PictureBox
		Dim sf As System.Drawing.Font
		Static font_smoothing As Integer
		Static init_draw_string As Boolean
		
		If PermanentStringMode Then
			'閭梧勹譖ｸ縺崎ｾｼ縺ｿ
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = MainForm.picBack
			'繝輔か繝ｳ繝郁ｨｭ螳壹ｒ螟画峩
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
			'騾壼ｸｸ縺ｮ譖ｸ縺崎ｾｼ縺ｿ
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			pic = MainForm.picMain(0)
			SaveScreen()
		End If
		
		'Invalid_string_refer_to_original_code
		If Not init_draw_string Then
			Call GetSystemParametersInfo(SPI_GETFONTSMOOTHING, 0, font_smoothing, 0)
			init_draw_string = True
		End If
		
		'Invalid_string_refer_to_original_code
		If font_smoothing = 0 Then
			Call SetSystemParametersInfo(SPI_SETFONTSMOOTHING, 1, 0, 0)
		End If
		
		With pic
			'迴ｾ蝨ｨ縺ｮX菴咲ｽｮ繧定ｨ倬鹸縺励※縺翫￥
			'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			prev_cx = .CurrentX
			
			'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				pic.Print(msg)
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				If X <> DEFAULT_LEVEL Then
					'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.CurrentX = X
				Else
					'UPGRADE_ISSUE: PictureBox プロパティ pic.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					.CurrentX = prev_cx
				End If
			Else
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: PictureBox メソッド pic.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				pic.Print(msg)
				
				'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Sub DrawSysString(ByVal X As Short, ByVal Y As Short, ByRef msg As String, Optional ByVal without_refresh As Boolean = False)
		Dim prev_color As Integer
		Dim prev_name As String
		Dim prev_size As Short
		Dim is_bold As Boolean
		Dim is_italic As Boolean
		Dim sf As System.Drawing.Font
		
		'Invalid_string_refer_to_original_code
		If X < MapX - MainWidth \ 2 Or MapX + MainWidth \ 2 < X Or Y < MapY - MainHeight \ 2 Or MapY + MainHeight \ 2 < Y Then
			Exit Sub
		End If
		
		SaveScreen()
		
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMain(0)
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.ForeColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Black)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.FontTransparent = False
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			If .Font.Name <> "Invalid_string_refer_to_original_code" Then
				sf = System.Windows.Forms.Control.DefaultFont.Clone()
				'UPGRADE_WARNING: Windows フォームでは、TrueType および OpenType フォントのみがサポートされます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="971F4DF4-254E-44F4-861D-3AA0031FE361"' をクリックしてください。
				sf = VB6.FontChangeName(sf, "Invalid_string_refer_to_original_code")
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
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.CurrentX = MapToPixelX(X) + (32 - .TextWidth(msg)) \ 2 - 1
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.CurrentY = MapToPixelY(Y + 1) - .TextHeight(msg)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Print(msg)
			
			'Invalid_string_refer_to_original_code
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
			
			'陦ｨ遉ｺ繧呈峩譁ｰ
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
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Sub SaveScreen()
		Dim ret As Integer
		
		If Not ScreenIsSaved Then
			'Invalid_string_refer_to_original_code
			With MainForm
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = BitBlt(.picMain(1).hDC, 0, 0, MainPWidth, MainPHeight, .picMain(0).hDC, 0, 0, SRCCOPY)
			End With
			ScreenIsSaved = True
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Sub LockGUI()
		IsGUILocked = True
		With MainForm
			'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.VScroll.Enabled = False
			'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.HScroll.Enabled = False
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub UnlockGUI()
		IsGUILocked = False
		With MainForm
			'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.VScroll.Enabled = True
			'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.HScroll.Enabled = True
		End With
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	
	'迴ｾ蝨ｨ縺ｮ繝槭え繧ｹ繧ｫ繝ｼ繧ｽ繝ｫ縺ｮ菴咲ｽｮ繧定ｨ倬鹸
	Public Sub SaveCursorPos()
		Dim PT As POINTAPI
		
		GetCursorPos(PT)
		PrevCursorX = PT.X
		PrevCursorY = PT.Y
		NewCursorX = 0
		NewCursorY = 0
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub MoveCursorPos(ByRef cursor_mode As String, Optional ByVal t As Unit = Nothing)
		Dim i, tx, ty, num As Integer
		Dim ret As Integer
		Dim prev_lock As Boolean
		Dim PT As POINTAPI
		
		'Invalid_string_refer_to_original_code
		GetCursorPos(PT)
		
		'迴ｾ蝨ｨ縺ｮ菴咲ｽｮ繧定ｨ倬鹸縺励※縺翫￥
		If PrevCursorX = 0 And cursor_mode <> "Invalid_string_refer_to_original_code" Then
			SaveCursorPos()
		End If
		
		'Invalid_string_refer_to_original_code
		If t Is Nothing Then
			If cursor_mode = "Invalid_string_refer_to_original_code" Then
				'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				With frmListBox
					If PT.X < (VB6.PixelsToTwipsX(.Left) + 0.1 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX Then
						tx = (VB6.PixelsToTwipsX(.Left) + 0.1 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX
					ElseIf PT.X > (VB6.PixelsToTwipsX(.Left) + 0.9 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX Then 
						tx = (VB6.PixelsToTwipsX(.Left) + 0.9 * VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX
					Else
						tx = PT.X
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'Invalid_string_refer_to_original_code
					i = .lstItems.Items.Count
					Do 
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						Exit Do
					Loop 
				End With
			End If
			i = i - 1
			'UPGRADE_WARNING: MoveCursorPos に変換されていないステートメントがあります。ソース コードを確認してください。
			'Loop
		Else
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: MoveCursorPos に変換されていないステートメントがあります。ソース コードを確認してください。
		End If
		
		'UPGRADE_WARNING: MoveCursorPos に変換されていないステートメントがあります。ソース コードを確認してください。
		'End With
		'End If
		'Invalid_string_refer_to_original_code
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
		'End If
		
		'Invalid_string_refer_to_original_code
		num = System.Math.Sqrt((tx - PT.X) ^ 2 + (ty - PT.Y) ^ 2) \ 25 + 1
		
		'Invalid_string_refer_to_original_code
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
		
		'譁ｰ縺励＞繧ｫ繝ｼ繧ｽ繝ｫ菴咲ｽｮ繧定ｨ倬鹸
		If NewCursorX = 0 Then
			NewCursorX = tx
			NewCursorY = ty
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreCursorPos()
		Dim i, tx, ty, num As Short
		Dim ret As Integer
		Dim PT As POINTAPI
		
		'Invalid_string_refer_to_original_code
		If Not SelectedUnit Is Nothing Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			MoveCursorPos("Invalid_string_refer_to_original_code")
			Exit Sub
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		If PrevCursorX = 0 And PrevCursorY = 0 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		GetCursorPos(PT)
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		PrevCursorX = 0
		PrevCursorY = 0
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	
	'繧ｿ繧､繝医Ν逕ｻ髱｢繧定｡ｨ遉ｺ
	Public Sub OpenTitleForm()
		'UPGRADE_ISSUE: Load ステートメント はサポートされていません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B530EFF2-3132-48F8-B8BC-D88AF543D321"' をクリックしてください。
		Load(frmTitle)
		
		frmTitle.Left = VB6.TwipsToPixelsX((VB6.PixelsToTwipsX(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width) - VB6.PixelsToTwipsX(frmTitle.Width)) / 2)
		frmTitle.Top = VB6.TwipsToPixelsY((VB6.PixelsToTwipsY(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height) - VB6.PixelsToTwipsY(frmTitle.Height)) / 2)
		
		frmTitle.Show()
		frmTitle.Refresh()
	End Sub
	
	'繧ｿ繧､繝医Ν逕ｻ髱｢繧帝哩縺倥ｋ
	Public Sub CloseTitleForm()
		frmTitle.Close()
		'UPGRADE_NOTE: オブジェクト frmTitle をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		frmTitle = Nothing
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Sub CloseNowLoadingForm()
		frmNowLoading.Close()
		'UPGRADE_NOTE: オブジェクト frmNowLoading をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		frmNowLoading = Nothing
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub DisplayLoadingProgress()
		frmNowLoading.Progress()
		System.Windows.Forms.Application.DoEvents()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub SetLoadImageSize(ByVal new_size As Short)
		With frmNowLoading
			.Value = 0
			.Size_Renamed = new_size
		End With
	End Sub
	
	
	' === 逕ｻ髱｢縺ｮ隗｣蜒丞ｺｦ螟画峩 ===
	
	Public Sub ChangeDisplaySize(ByVal w As Short, ByVal h As Short)
		Dim dm As DEVMODE
		Dim ret As Integer
		Static orig_width, orig_height As Short
		
		'Invalid_string_refer_to_original_code
		dm.dmSize = Len(dm)
		
		'Invalid_string_refer_to_original_code
		ret = EnumDisplaySettings(vbNullString, ENUM_CURRENT_SETTINGS, dm)
		
		If w <> 0 And h <> 0 Then
			'Invalid_string_refer_to_original_code
			
			'迴ｾ蝨ｨ縺ｮ隗｣蜒丞ｺｦ繧定ｨ倬鹸縺励※縺翫￥
			orig_width = dm.dmPelsWidth
			orig_height = dm.dmPelsHeight
			
			If dm.dmPelsWidth = w And dm.dmPelsHeight = h Then
				'Invalid_string_refer_to_original_code
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			dm.dmPelsWidth = w
			dm.dmPelsHeight = h
		Else
			'Invalid_string_refer_to_original_code
			
			If orig_width = 0 And orig_height Then
				'Invalid_string_refer_to_original_code
				Exit Sub
			End If
			
			If dm.dmPelsWidth = orig_width And dm.dmPelsHeight = orig_width Then
				'Invalid_string_refer_to_original_code
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			ret = ChangeDisplaySettings(VariantType.Null, 0)
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: オブジェクト dm の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		ret = ChangeDisplaySettings(dm, CDS_TEST)
		If ret <> DISP_CHANGE_SUCCESSFUL Then
			Exit Sub
		End If
		
		'隗｣蜒丞ｺｦ繧貞ｮ滄圀縺ｫ螟画峩縺吶ｋ
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
				'Invalid_string_refer_to_original_code
				Exit Sub
			Case DISP_CHANGE_RESTART
				'Invalid_string_refer_to_original_code
				ret = ChangeDisplaySettings(VariantType.Null, 0)
		End Select
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		Do While frmErrorMessage.Visible
			System.Windows.Forms.Application.DoEvents()
			Sleep(200)
		Loop 
		
		frmErrorMessage.Close()
		'UPGRADE_NOTE: オブジェクト frmErrorMessage をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		frmErrorMessage = Nothing
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub DataErrorMessage(ByRef msg As String, ByRef fname As String, ByVal line_num As Short, ByRef line_buf As String, ByRef dname As String)
		Dim err_msg As String
		
		'繧ｨ繝ｩ繝ｼ縺檎匱逕溘＠縺溘ヵ繧｡繧､繝ｫ蜷阪→陦檎分蜿ｷ
		err_msg = fname & "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		
		'Invalid_string_refer_to_original_code
		If Len(dname) > 0 Then
			err_msg = err_msg & dname & "Invalid_string_refer_to_original_code"
		End If
		
		'繧ｨ繝ｩ繝ｼ縺ｮ蜴溷屏
		If Len(msg) > 0 Then
			err_msg = err_msg & msg & vbCr & vbLf
		End If
		
		'Invalid_string_refer_to_original_code
		If dname = "" And msg = "" Then
			err_msg = err_msg & "Invalid_string_refer_to_original_code"
		End If
		
		'Invalid_string_refer_to_original_code
		err_msg = err_msg & line_buf
		
		'Invalid_string_refer_to_original_code
		ErrorMessage(err_msg)
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Function IsRButtonPressed(Optional ByVal ignore_message_wait As Boolean = False) As Boolean
		Dim PT As POINTAPI
		
		'Invalid_string_refer_to_original_code
		If Not ignore_message_wait And MessageWait = 0 Then
			IsRButtonPressed = True
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		If MainForm.Handle.ToInt32 = GetForegroundWindow Then
			GetCursorPos(PT)
			With MainForm
				If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
					If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
						'Invalid_string_refer_to_original_code
						IsRButtonPressed = True
						Exit Function
					End If
				End If
			End With
			'Invalid_string_refer_to_original_code
		ElseIf frmMessage.Handle.ToInt32 = GetForegroundWindow Then 
			GetCursorPos(PT)
			With frmMessage
				If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
					If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
						'Invalid_string_refer_to_original_code
						IsRButtonPressed = True
						Exit Function
					End If
				End If
			End With
		End If
	End Function
	
	
	'Telop繧ｳ繝槭Φ繝臥畑謠冗判繝ｫ繝ｼ繝√Φ
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