Option Strict Off
Option Explicit On
Friend Class SpecialPowerDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Private colSpecialPowerDataList As New Collection
	
	
	'繧ｯ繝ｩ繧ｹ縺ｮ隗｣謾ｾ
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colSpecialPowerDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colSpecialPowerDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colSpecialPowerDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Add(ByRef sname As String) As SpecialPowerData
		Dim new_data As New SpecialPowerData
		
		new_data.Name = sname
		colSpecialPowerDataList.Add(new_data, sname)
		Add = new_data
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Count() As Short
		Count = colSpecialPowerDataList.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Delete(ByRef Index As Object)
		colSpecialPowerDataList.Remove(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Item(ByRef Index As Object) As SpecialPowerData
		On Error GoTo ErrorHandler
		
		Item = colSpecialPowerDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item = Nothing
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim dummy As SpecialPowerData
		
		On Error GoTo ErrorHandler
		dummy = colSpecialPowerDataList.Item(Index)
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
		Dim buf, line_buf, buf2 As String
		Dim sd As SpecialPowerData
		Dim data_name As String
		Dim err_msg As String
		
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
			
			'蜷咲ｧｰ
			ret = InStr(line_buf, ",")
			If ret > 0 Then
				data_name = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
			Else
				data_name = line_buf
				buf = ""
			End If
			
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
			
			If IsDefined(data_name) Then
				Delete(data_name)
			End If
			sd = Add(data_name)
			
			With sd
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				If ret > 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				.KanaName = Trim(buf)
				If .KanaName = "" Then
					.KanaName = StrToHiragana(data_name)
				End If
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				
				'遏ｭ邵ｮ蠖｢
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				.ShortName = buf2
				
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.SPConsumption = CShort(buf2)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'蟇ｾ雎｡
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				.TargetType = buf2
				
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				.Duration = buf2
				
				'驕ｩ逕ｨ譚｡莉ｶ
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				.NecessaryCondition = buf2
				
				'菴ｿ逕ｨ譚｡莉ｶ, 繧｢繝九Γ
				ret = InStr(buf, ",")
				If ret > 0 Then
					.Animation = Trim(Mid(buf, InStr(buf, ",") + 1))
				Else
					.Animation = .Name
				End If
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				If Len(line_buf) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				.SetEffect(line_buf)
				
				'隗｣隱ｬ
				GetLine(FileNumber, line_buf, line_num)
				If Len(line_buf) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				.Comment = line_buf
			End With
		Loop 
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
		If line_num = 0 Then
			ErrorMessage(fname & "Invalid_string_refer_to_original_code")
		Else
			FileClose(FileNumber)
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
End Class