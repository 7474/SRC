Option Strict Off
Option Explicit On
Friend Class MessageDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Private colMessageDataList As New Collection
	
	'繧ｯ繝ｩ繧ｹ縺ｮ隗｣謾ｾ
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colMessageDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colMessageDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colMessageDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Add(ByRef mname As String) As MessageData
		Dim new_md As New MessageData
		
		new_md.Name = mname
		colMessageDataList.Add(new_md, mname)
		Add = new_md
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Count() As Short
		Count = colMessageDataList.Count()
	End Function
	
	Public Sub Delete(ByRef Index As Object)
		colMessageDataList.Remove(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Item(ByRef Index As Object) As MessageData
		On Error GoTo ErrorHandler
		
		Item = colMessageDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item = Nothing
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim md As MessageData
		
		On Error GoTo ErrorHandler
		md = colMessageDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim ret As Short
		Dim line_num As Integer
		Dim line_buf As String
		Dim md As MessageData
		Dim is_effect As Boolean
		Dim sname, msg As String
		Dim data_name As String
		Dim err_msg As String
		
		'Invalid_string_refer_to_original_code
		If InStr(LCase(fname), "effect.txt") > 0 Or InStr(LCase(fname), "animation.txt") > 0 Then
			is_effect = True
		End If
		
		On Error GoTo ErrorHandler
		
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read)
		
		line_num = 0
		
		Do While True
			data_name = ""
			
			Do 
				If EOF(FileNumber) Then
					FileClose(FileNumber)
					Exit Sub
				End If
				GetLine(FileNumber, line_buf, line_num)
			Loop While Len(line_buf) = 0
			
			If InStr(line_buf, ",") > 0 Then
				err_msg = "Invalid_string_refer_to_original_code"
				Error(0)
			End If
			
			data_name = line_buf
			
			If InStr(data_name, " ") > 0 Then
				err_msg = "Invalid_string_refer_to_original_code"
				Error(0)
			End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			err_msg = "Invalid_string_refer_to_original_code"
			Error(0)
			'End If
			If InStr(data_name, """") > 0 Then
				err_msg = "Invalid_string_refer_to_original_code"
				Error(0)
			End If
			
			'Invalid_string_refer_to_original_code
			If IsDefined(data_name) Then
				If Not is_effect Then
					'Invalid_string_refer_to_original_code
					Delete(data_name)
					md = Add(data_name)
				Else
					'Invalid_string_refer_to_original_code
					md = Item(data_name)
				End If
			Else
				md = Add(data_name)
			End If
			
			With md
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0
					ret = InStr(line_buf, ",")
					
					If ret < 2 Then
						Error(0)
					End If
					
					sname = Left(line_buf, ret - 1)
					msg = Trim(Mid(line_buf, ret + 1))
					
					If Len(sname) = 0 Then
						err_msg = "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					
					.AddMessage(sname, msg)
					
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Loop 
			End With
		Loop 
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
		If line_num = 0 Then
			ErrorMessage(fname & "Invalid_string_refer_to_original_code")
		Else
			FileClose(FileNumber)
			DataErrorMessage("", fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub AddDefaultAnimation()
		Dim md As MessageData
		
		'Invalid_string_refer_to_original_code
		If Count() = 0 Then
			If FileExists(AppPath & "Data\System\animation.txt") Then
				Load(AppPath & "Data\System\animation.txt")
			End If
		End If
		
		If IsDefined("豎守畑") Then
			md = Item("豎守畑")
		Else
			md = Add("豎守畑")
		End If
		
		With md
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .SelectMessage("蝗樣∩") = "" Then
				.AddMessage("蝗樣∩", "蝗樣∩")
			End If
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .SelectMessage("Invalid_string_refer_to_original_code") = "" Then
				.AddMessage("Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
			End If
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .SelectMessage("霑取茶") = "" Then
				.AddMessage("霑取茶", "霑取茶")
			End If
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .SelectMessage("Invalid_string_refer_to_original_code") = "" Then
				.AddMessage("Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
			End If
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.AddMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .SelectMessage("陬懃ｵｦ") = "" Then
				.AddMessage("陬懃ｵｦ", "Invalid_string_refer_to_original_code")
			End If
			'End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			.AddMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'End If
			'End If
		End With
	End Sub
End Class