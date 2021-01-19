Option Strict Off
Option Explicit On
Friend Class DialogDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'全ダイアログデータを管理するリストのクラス
	
	'ダイアログデータのコレクション
	Private colDialogDataList As New Collection
	
	'クラスを解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colDialogDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colDialogDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colDialogDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ダイアログデータを追加
	Public Function Add(ByRef dname As String) As DialogData
		Dim new_dd As New DialogData
		
		new_dd.Name = dname
		colDialogDataList.Add(new_dd, dname)
		Add = new_dd
	End Function
	
	'ダイアログデータの総数
	Public Function Count() As Short
		Count = colDialogDataList.Count()
	End Function
	
	Public Sub Delete(ByRef Index As Object)
		colDialogDataList.Remove(Index)
	End Sub
	
	'ダイアログデータを返す
	Public Function Item(ByRef Index As Object) As DialogData
		On Error GoTo ErrorHandler
		
		Item = colDialogDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item = Nothing
	End Function
	
	'指定したダイアログデータが定義されているか？
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim dummy As DialogData
		
		On Error GoTo ErrorHandler
		dummy = colDialogDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'データファイル fname からデータをロード
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim i, ret As Short
		Dim line_num As Integer
		Dim line_buf As String
		Dim pilot_list As String
		Dim d As Dialog
		Dim dd As DialogData
		Dim err_msg As String
		Dim pname, msg As String
		
		On Error GoTo ErrorHandler
		
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read)
		
		line_num = 0
		
		Do While True
			Do 
				If EOF(FileNumber) Then
					FileClose(FileNumber)
					Exit Sub
				End If
				GetLine(FileNumber, line_buf, line_num)
			Loop While Len(line_buf) = 0
			
			'UPGRADE_NOTE: オブジェクト dd をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			dd = Nothing
			
			'パイロット名一覧
			If LLength(line_buf) = 0 Then
				Error(0)
			End If
			pilot_list = ""
			For i = 1 To LLength(line_buf)
				pilot_list = pilot_list & " " & LIndex(line_buf, i)
			Next 
			pilot_list = Trim(pilot_list)
			
			If IsDefined(pilot_list) Then
				Delete(pilot_list)
			End If
			dd = Add(pilot_list)
			
			GetLine(FileNumber, line_buf, line_num)
			Do While Len(line_buf) > 0
				'シチューション
				d = dd.AddDialog(line_buf)
				
				Do While True
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
					
					'話者
					ret = InStr(line_buf, ",")
					If ret = 0 Then
						Exit Do
					End If
					pname = Left(line_buf, ret - 1)
					
					'指定した話者のデータが存在するかチェック。
					'ただし合体技のパートナーは場合は他の作品のパイロットであることも
					'あるので話者チェックを行わない。
					If Left(pname, 1) <> "@" Then
						If Not PDList.IsDefined(pname) And Not NPDList.IsDefined(pname) And pname <> "システム" Then
							err_msg = "パイロット「" & pname & "」が定義されていません。"
							Error(0)
						End If
					End If
					
					'メッセージ
					If Len(line_buf) = ret Then
						err_msg = "メッセージが定義されていません。"
						Error(0)
					End If
					msg = Trim(Mid(line_buf, ret + 1))
					
					d.AddMessage(pname, msg)
				Loop 
			Loop 
		Loop 
		
ErrorHandler: 
		If line_num = 0 Then
			ErrorMessage(fname & "が開けません。")
		Else
			FileClose(FileNumber)
			If dd Is Nothing Then
				DataErrorMessage(err_msg, fname, line_num, line_buf, "")
			Else
				DataErrorMessage(err_msg, fname, line_num, line_buf, (dd.Name))
			End If
		End If
		TerminateSRC()
	End Sub
End Class