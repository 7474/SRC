Option Strict Off
Option Explicit On
Module FileDialog
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'APIだけでファイルダイアログを実現するためのモジュール
	
	' OPENFILENAME構造体
	Private Structure OPENFILENAME
		Dim lStructSize As Integer
		Dim hwndOwner As Integer
		Dim hInstance As Integer
		Dim lpstrFilter As String
		Dim lpstrCustomFilter As String
		Dim nMaxCustFilter As Integer
		Dim iFilterIndex As Integer
		Dim lpstrFile As String
		Dim nMaxFile As Integer
		Dim lpstrFileTitle As String
		Dim nMaxFileTitle As Integer
		Dim lpstrInitialDir As String
		Dim lpstrTitle As String
		Dim flags As Integer
		Dim nFileOffset As Short
		Dim nFileExtension As Short
		Dim lpstrDefExt As String
		Dim lCustData As Integer
		Dim lpfnHook As Integer
		Dim lpTemplateName As String
	End Structure
	
	'「ファイルを開く」のダイアログを作成
	'UPGRADE_WARNING: 構造体 OPENFILENAME に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Private Declare Function GetOpenFileName Lib "comdlg32.dll"  Alias "GetOpenFileNameA"(ByRef f As OPENFILENAME) As Integer
	
	'「ファイルを保存」のダイアログを作成
	'UPGRADE_WARNING: 構造体 OPENFILENAME に、この Declare ステートメントの引数としてマーシャリング属性を渡す必要があります。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' をクリックしてください。
	Private Declare Function GetSaveFileName Lib "comdlg32.dll"  Alias "GetSaveFileNameA"(ByRef f As OPENFILENAME) As Integer
	
	'ファイルロード用のダイアログを表示するための関数
	Public Function LoadFileDialog(ByRef dtitle As String, ByRef fpath As String, ByRef default_file As String, ByVal default_filter As Short, ByRef ftype As String, ByRef fsuffix As String, Optional ByRef ftype2 As String = "", Optional ByRef fsuffix2 As String = "", Optional ByRef ftype3 As String = "", Optional ByRef fsuffix3 As String = "") As String
		Dim f As OPENFILENAME
		Dim fname, ftitle As String
		Dim ret As Integer
		
		fname = default_file & New String(vbNullChar, 1024 - Len(default_file))
		ftitle = Space(1024)
		
		' OPENFILENAME構造体の初期化
		With f
			.hwndOwner = MainForm.Handle.ToInt32
			If ftype3 <> "" Then
				.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar & ftype3 & " (*." & fsuffix3 & ")" & vbNullChar & "*." & fsuffix3 & vbNullChar
			ElseIf ftype2 <> "" Then 
				.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
			Else
				.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar
			End If
			.iFilterIndex = default_filter
			.lpstrFile = fname
			.nMaxFile = 1024
			.lpstrFileTitle = ftitle
			.nMaxFileTitle = 1024
			.lpstrInitialDir = fpath
			.lpstrTitle = dtitle
			.flags = &H201804
			.lpstrDefExt = ""
			.lStructSize = Len(f)
		End With
		
		ret = GetOpenFileName(f)
		
		Select Case ret
			Case 0
				'キャンセルされた
				LoadFileDialog = ""
			Case 1
				'ファイルを選択
				LoadFileDialog = f.lpstrFile
				' vbNullChar までで切り出す。
				LoadFileDialog = Left(LoadFileDialog, InStr(LoadFileDialog, vbNullChar) - 1)
		End Select
	End Function
	
	'ファイルセーブ用のダイアログを表示するための関数
	Public Function SaveFileDialog(ByRef dtitle As String, ByRef fpath As String, ByRef default_file As String, ByVal default_filter As Short, ByRef ftype As String, ByRef fsuffix As String, Optional ByRef ftype2 As String = "", Optional ByRef fsuffix2 As String = "", Optional ByRef ftype3 As String = "", Optional ByRef fsuffix3 As String = "") As String
		Dim f As OPENFILENAME
		Dim fname, ftitle As String
		Dim ret As Integer
		
		fname = default_file & New String(vbNullChar, 1024 - Len(default_file))
		ftitle = Space(1024)
		
		' OPENFILENAME構造体の初期化
		With f
			.hwndOwner = MainForm.Handle.ToInt32
			If ftype3 <> "" Then
				.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar & ftype3 & " (*." & fsuffix3 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
			ElseIf ftype2 <> "" Then 
				.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
			Else
				.lpstrFilter = "すべてのﾌｧｲﾙ (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar
			End If
			.iFilterIndex = default_filter
			.lpstrFile = fname
			.nMaxFile = 1024
			.lpstrFileTitle = ftitle
			.nMaxFileTitle = 1024
			.lpstrInitialDir = fpath
			.lpstrTitle = dtitle
			.flags = &H201804
			.lpstrDefExt = ""
			.lStructSize = Len(f)
		End With
		
		ret = GetSaveFileName(f)
		
		Select Case ret
			Case 0
				'キャンセルされた
				SaveFileDialog = ""
			Case 1
				'ファイルを選択
				SaveFileDialog = f.lpstrFile
				' vbNullChar までで切り出す。
				SaveFileDialog = Left(SaveFileDialog, InStr(SaveFileDialog, vbNullChar) - 1)
		End Select
	End Function
End Module