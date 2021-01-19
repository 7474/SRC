Option Strict Off
Option Explicit On
Module Flash
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'Flashファイルの再生
	Public Sub PlayFlash(ByRef fname As String, ByRef fx As Short, ByRef fy As Short, ByRef fw As Short, ByRef fh As Short, ByRef opt As String)
		Dim i As Short
		Dim is_VisibleEnd As Boolean
		
		'FLASHが使用できない場合はエラー
		If Not IsFlashAvailable Then
			ErrorMessage("Flashファイルの読み込み中にエラーが発生しました。" & vbCrLf & "「Macromedia Flash Player」がインストールされていません。" & vbCrLf & "次のURLから、最新版のFlash Playerをインストールしてください。" & vbCrLf & "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese")
			Exit Sub
		End If
		'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Enable の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		If Not frmMain.FlashObject.Enable Then
			ErrorMessage("Flashファイルの読み込み中にエラーが発生しました。" & vbCrLf & "「Macromedia Flash Player」がインストールされていません。" & vbCrLf & "次のURLから、最新版のFlash Playerをインストールしてください。" & vbCrLf & "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese")
			Exit Sub
		End If
		
		is_VisibleEnd = False
		
		For i = 1 To LLength(opt)
			Select Case LIndex(opt, i)
				Case "保持"
					is_VisibleEnd = True
			End Select
		Next 
		
		With frmMain.FlashObject
			'一旦非表示
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Visible の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			.Visible = False
			
			'Flashオブジェクトの位置・サイズ設定
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Left の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			.Left = fx
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Top の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			.Top = fy
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Width の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			.Width = fw
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Height の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			.Height = fh
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Visible の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			.Visible = True
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.ZOrder の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			.ZOrder()
			
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.LoadMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			.LoadMovie(ScenarioPath & fname)
			
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Playing の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Do While .Playing And Not IsRButtonPressed(True)
				System.Windows.Forms.Application.DoEvents()
			Loop 
			
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.StopMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			frmMain.FlashObject.StopMovie()
			
			If Not is_VisibleEnd Then
				'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.ClearMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.ClearMovie()
				'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Visible の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				.Visible = False
			End If
		End With
	End Sub
	
	'表示したままのFlashを消去する
	Public Sub ClearFlash()
		If Not IsFlashAvailable Then Exit Sub
		'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Enable の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		If Not frmMain.FlashObject.Enable Then Exit Sub
		
		With frmMain.FlashObject
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.ClearMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			.ClearMovie()
			'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.Visible の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			.Visible = False
		End With
	End Sub
	
	'Flashファイルからイベントを取得
	' Flashのアクションの「GetURL」で
	'　1.「URL」に"FSCommand:"
	'　2.「ターゲット」に「サブルーチン名 [引数1 [引数2 […]]」
	'を指定すると、そのアクションが実行されたときに
	'ターゲットのサブルーチンが実行される。
	'サブルーチンを実行している間、Flashの再生は停止する。
	Public Sub GetEvent(ByVal fpara As String)
		Dim buf As String
		Dim i, j As Short
		Dim funcname, funcpara As String
		Dim etype As Expression.ValueType
		Dim str_result As String
		Dim num_result As Double
		
		'再生を一時停止
		'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.StopMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		frmMain.FlashObject.StopMovie()
		
		funcname = ""
		funcpara = ""
		
		'念のためにFlashから渡されたパラメータ全てを解析
		'一番最初に見つかった文字列を、呼び出すサブルーチン名とする
		If funcname = "" Then
			funcname = ListIndex(fpara, 1)
			buf = ListTail(fpara, 2)
		End If
		'サブルーチンの引数を記録
		For j = 1 To ListLength(buf)
			funcpara = funcpara & ", " & ListIndex(buf, j)
		Next 
		
		'サブルーチン名と引数から、Call関数の呼び出しの文字列を生成
		buf = "Call(" & funcname & funcpara & ")"
		'式として生成した文字列を実行
		CallFunction(buf, etype, str_result, num_result)
		
		'再生を再開
		'UPGRADE_WARNING: オブジェクト frmMain.FlashObject.PlayMovie の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		frmMain.FlashObject.PlayMovie()
	End Sub
End Module