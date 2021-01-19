Option Strict Off
Option Explicit On
Friend Class DialogDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Private colDialogDataList As New Collection
	
	'繧ｯ繝ｩ繧ｹ繧定ｧ｣謾ｾ
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
	
	'Invalid_string_refer_to_original_code
	Public Function Add(ByRef dname As String) As DialogData
		Dim new_dd As New DialogData
		
		new_dd.Name = dname
		colDialogDataList.Add(new_dd, dname)
		Add = new_dd
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Count() As Short
		Count = colDialogDataList.Count()
	End Function
	
	Public Sub Delete(ByRef Index As Object)
		colDialogDataList.Remove(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Item(ByRef Index As Object) As DialogData
		On Error GoTo ErrorHandler
		
		Item = colDialogDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item = Nothing
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim dummy As DialogData
		
		On Error GoTo ErrorHandler
		dummy = colDialogDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
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
				'繧ｷ繝√Η繝ｼ繧ｷ繝ｧ繝ｳ
				d = dd.AddDialog(line_buf)
				
				Do While True
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
					
					'Invalid_string_refer_to_original_code
					ret = InStr(line_buf, ",")
					If ret = 0 Then
						Exit Do
					End If
					pname = Left(line_buf, ret - 1)
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					If Left(pname, 1) <> "@" Then
						If Not PDList.IsDefined(pname) And Not NPDList.IsDefined(pname) And pname <> "Invalid_string_refer_to_original_code" Then
							err_msg = "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							Error(0)
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					If Len(line_buf) = ret Then
						err_msg = "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					msg = Trim(Mid(line_buf, ret + 1))
					
					d.AddMessage(pname, msg)
				Loop 
			Loop 
		Loop 
		
ErrorHandler: 
		If line_num = 0 Then
			ErrorMessage(fname & "Invalid_string_refer_to_original_code")
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