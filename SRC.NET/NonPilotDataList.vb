Option Strict Off
Option Explicit On
Friend Class NonPilotDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Private colNonPilotDataList As New Collection
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		Dim npd As New NonPilotData
		
		'Talk繧ｳ繝槭Φ繝臥畑
		With npd
			.Name = "繝翫Ξ繝ｼ繧ｿ繝ｼ"
			.Nickname = "繝翫Ξ繝ｼ繧ｿ繝ｼ"
			.Bitmap = ".bmp"
		End With
		colNonPilotDataList.Add(npd, npd.Name)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'繧ｯ繝ｩ繧ｹ縺ｮ隗｣謾ｾ
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colNonPilotDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colNonPilotDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colNonPilotDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Add(ByRef pname As String) As NonPilotData
		Dim new_pilot_data As New NonPilotData
		
		new_pilot_data.Name = pname
		colNonPilotDataList.Add(new_pilot_data, pname)
		Add = new_pilot_data
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Count() As Short
		Count = colNonPilotDataList.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Delete(ByRef Index As Object)
		colNonPilotDataList.Remove(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Item(ByRef Index As Object) As NonPilotData
		Dim pd As NonPilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		Item = colNonPilotDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		pname = CStr(Index)
		For	Each pd In colNonPilotDataList
			If pd.Nickname0 = pname Then
				Item = pd
				Exit Function
			End If
		Next pd
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim pd As NonPilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		pd = colNonPilotDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		pname = CStr(Index)
		For	Each pd In colNonPilotDataList
			If pd.Nickname0 = pname Then
				IsDefined = True
				Exit Function
			End If
		Next pd
		IsDefined = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim pd As NonPilotData
		
		On Error GoTo ErrorHandler
		pd = colNonPilotDataList.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim line_num As Integer
		Dim buf, line_buf, buf2 As String
		Dim ret As Short
		Dim pd As New NonPilotData
		Dim data_name As String
		Dim err_msg As String
		
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
			
			If InStr(line_buf, ",") > 0 Then
				err_msg = "Invalid_string_refer_to_original_code"
				Error(0)
			End If
			
			'蜷咲ｧｰ
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
			
			If IsDefined(data_name) Then
				'Invalid_string_refer_to_original_code
				If Item(data_name).Name = data_name Then
					pd = Item(data_name)
				Else
					pd = Add(data_name)
				End If
			Else
				pd = Add(data_name)
			End If
			
			With pd
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				
				'諢帷ｧｰ
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If buf2 <> "" Then
					.Nickname = buf2
				Else
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				
				'Invalid_string_refer_to_original_code
				buf2 = Trim(buf)
				If LCase(Right(buf2, 4)) = ".bmp" Then
					.Bitmap = buf2
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
				End If
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