Option Strict Off
Option Explicit On
Friend Class PilotDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Private colPilotDataList As New Collection
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		Dim pd As New PilotData
		
		'Invalid_string_refer_to_original_code
		With pd
			.Name = "Invalid_string_refer_to_original_code"
			.Nickname = "Invalid_string_refer_to_original_code"
			.Adaption = "AAAA"
			.Bitmap = ".bmp"
		End With
		colPilotDataList.Add(pd, pd.Name)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'繧ｯ繝ｩ繧ｹ縺ｮ隗｣謾ｾ
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colPilotDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colPilotDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colPilotDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Add(ByRef pname As String) As PilotData
		Dim new_pilot_data As New PilotData
		
		new_pilot_data.Name = pname
		colPilotDataList.Add(new_pilot_data, pname)
		Add = new_pilot_data
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Count() As Short
		Count = colPilotDataList.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Delete(ByRef Index As Object)
		colPilotDataList.Remove(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Item(ByRef Index As Object) As PilotData
		Dim pd As PilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		Item = colPilotDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		pname = CStr(Index)
		For	Each pd In colPilotDataList
			If pd.Nickname = pname Then
				Item = pd
				Exit Function
			End If
		Next pd
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim pd As PilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		pd = colPilotDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		pname = CStr(Index)
		For	Each pd In colPilotDataList
			If pd.Nickname = pname Then
				IsDefined = True
				Exit Function
			End If
		Next pd
		IsDefined = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim pd As PilotData
		
		On Error GoTo ErrorHandler
		pd = colPilotDataList.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim line_num As Integer
		Dim i, j As Short
		Dim ret, n, ret2 As Integer
		Dim buf, line_buf, buf2 As String
		Dim pd As PilotData
		Dim data_name As String
		Dim err_msg As String
		Dim aname, adata As String
		Dim alevel As Double
		Dim wd As WeaponData
		Dim sd As AbilityData
		Dim wname, sname As String
		Dim sp_cost As Short
		Dim in_quote As Boolean
		Dim comma_num As Short
		
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
			
			'蜷咲ｧｰ
			data_name = line_buf
			
			If InStr(data_name, " ") > 0 Then
				err_msg = "Invalid_string_refer_to_original_code"
				Error(0)
			End If
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			err_msg = "蜷咲ｧｰ縺ｫ蜈ｨ隗呈峡蠑ｧ縺ｯ菴ｿ逕ｨ蜃ｺ譚･縺ｾ縺帙ｓ"
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
					pd.Clear()
				Else
					pd = Add(data_name)
				End If
			Else
				pd = Add(data_name)
			End If
			
			With pd
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				
				'Invalid_string_refer_to_original_code
				comma_num = 0
				For i = 1 To Len(line_buf)
					If Mid(line_buf, i, 1) = "," Then
						comma_num = comma_num + 1
					End If
				Next 
				
				If comma_num < 3 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				ElseIf comma_num > 5 Then 
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				
				'諢帷ｧｰ
				ret = InStr(line_buf, ",")
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If Len(buf2) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				.Nickname = buf2
				
				Select Case comma_num
					Case 4
						'Invalid_string_refer_to_original_code
						ret = InStr(buf, ",")
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						Select Case buf2
							Case "逕ｷ諤ｧ", "螂ｳ諤ｧ", "-"
								.KanaName = StrToHiragana(.Nickname)
								.Sex = buf2
							Case Else
								.KanaName = buf2
						End Select
					Case 5
						'Invalid_string_refer_to_original_code
						ret = InStr(buf, ",")
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						Select Case buf2
							Case "逕ｷ諤ｧ", "螂ｳ諤ｧ", "-"
								DataErrorMessage("Invalid_string_refer_to_original_code")
								fname( , line_num, line_buf, data_name)
								.KanaName = StrToHiragana(.Nickname)
							Case Else
								.KanaName = buf2
						End Select
						
						'諤ｧ蛻･
						ret = InStr(buf, ",")
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						Select Case buf2
							Case "逕ｷ諤ｧ", "螂ｳ諤ｧ", "-"
								.Sex = buf2
							Case Else
								DataErrorMessage("Invalid_string_refer_to_original_code")
								fname( , line_num, line_buf, data_name)
						End Select
					Case Else
						.KanaName = StrToHiragana(.Nickname)
				End Select
				
				'繧ｯ繝ｩ繧ｹ
				ret = InStr(buf, ",")
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If Not IsNumeric(buf2) Then
					.Class_Renamed = buf2
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If Len(buf2) = 4 Then
					.Adaption = buf2
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
					.Adaption = "AAAA"
				End If
				
				'邨碁ｨ灘､
				buf2 = Trim(buf)
				If IsNumeric(buf2) Then
					.ExpValue = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				GetLine(FileNumber, line_buf, line_num)
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				buf = line_buf
				
				i = 0
				aname = ""
				Do While True
					i = i + 1
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					'Invalid_string_refer_to_original_code
					ret2 = InStr(buf, """")
					
					If ret2 < ret And ret2 > 0 Then
						'Invalid_string_refer_to_original_code
						in_quote = True
						j = ret2 + 1
						Do While j <= Len(buf)
							Select Case Mid(buf, j, 1)
								Case """"
									in_quote = Not in_quote
								Case ","
									If Not in_quote Then
										buf2 = Left(buf, j - 1)
										buf = Mid(buf, j + 1)
									End If
							End Select
							j = j + 1
						Loop 
						If j = Len(buf) Then
							buf2 = buf
							buf = ""
						End If
						in_quote = False
					ElseIf ret > 0 Then 
						'Invalid_string_refer_to_original_code
						buf2 = Trim(Left(buf, ret - 1))
						buf = Trim(Mid(buf, ret + 1))
						
						'Invalid_string_refer_to_original_code
						If buf = "" Then
							If i Mod 2 = 1 Then
								err_msg = "Invalid_string_refer_to_original_code"
							Else
								err_msg = "Invalid_string_refer_to_original_code"
							End If
							Error(0)
						End If
					Else
						'Invalid_string_refer_to_original_code
						buf2 = buf
						buf = ""
					End If
					
					If i Mod 2 = 1 Then
						'Invalid_string_refer_to_original_code
						
						If IsNumeric(buf2) Then
							If i = 1 Then
								'Invalid_string_refer_to_original_code
								buf = buf2 & ", " & buf
								Exit Do
							Else
								DataErrorMessage("陦碁ｭ縺九ｉ" & VB6.Format((i + 1) \ 2) & "Invalid_string_refer_to_original_code")
								fname( , line_num, line_buf, data_name)
							End If
						End If
						
						If InStr(buf2, " ") > 0 Then
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'And Left$(buf2, 4) <> "繝上Φ繧ｿ繝ｼ" _
							'And InStr(buf2, "=隗｣隱ｬ ") = 0 _
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							If aname = "" Then
								err_msg = "陦碁ｭ縺九ｉ" & VB6.Format((i + 1) \ 2) & "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							Else
								err_msg = "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							End If
							Error(0)
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					j = InStr(buf2, "=")
					If j > 0 Then
						adata = Mid(buf2, j + 1)
						buf2 = Left(buf2, j - 1)
					Else
						adata = ""
					End If
					
					'Invalid_string_refer_to_original_code
					j = InStr(buf2, "Lv")
					Select Case j
						Case 0
							'Invalid_string_refer_to_original_code
							aname = buf2
							alevel = DEFAULT_LEVEL
						Case 1
							'Invalid_string_refer_to_original_code
							If Not IsNumeric(Mid(buf2, j + 2)) Then
								DataErrorMessage("Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code_
								'fname, line_num, line_buf, data_name
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							End If
							
							alevel = CShort(Mid(buf2, j + 2))
							If aname = "" Then
								DataErrorMessage("陦碁ｭ縺九ｉ" & VB6.Format((i + 1) \ 2) & "Invalid_string_refer_to_original_code")
								fname( , line_num, line_buf, data_name)
							End If
						Case Else
							'Invalid_string_refer_to_original_code
							aname = Left(buf2, j - 1)
							alevel = CDbl(Mid(buf2, j + 2))
					End Select
					'Invalid_string_refer_to_original_code
					If IsNumeric(buf2) Then
						.AddSkill(aname, alevel, adata, CShort(buf2))
					Else
						If alevel > 0 Then
							DataErrorMessage("Invalid_string_refer_to_original_code")
							aname & "Lv" & Format$(alevel) _
							Invalid_string_refer_to_original_code_
							fname, line_num, line_buf, data_name
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						Else
							DataErrorMessage("Invalid_string_refer_to_original_code")
							'Invalid_string_refer_to_original_code_
							'fname, line_num, line_buf, data_name
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						End If
						.AddSkill(aname, alevel, adata, 1)
					End If
					'End If
					
					If buf = "" Then
						'Invalid_string_refer_to_original_code
						
						'Invalid_string_refer_to_original_code
						If i Mod 2 = 1 Then
							If alevel > 0 Then
								DataErrorMessage("Invalid_string_refer_to_original_code")
								aname & "Lv" & Format$(alevel) _
								Invalid_string_refer_to_original_code_
								fname, line_num, line_buf, data_name
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							Else
								DataErrorMessage("Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code_
								'fname, line_num, line_buf, data_name
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							End If
						End If
						
						GetLine(FileNumber, line_buf, line_num)
						buf = line_buf
						
						i = 0
						aname = ""
					End If
				Loop 
				'UPGRADE_WARNING: Load に変換されていないステートメントがあります。ソース コードを確認してください。
				'Invalid_string_refer_to_original_code
				buf = Mid(line_buf, 6)
				
				i = 0
				aname = ""
				Do 
					i = i + 1
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					'Invalid_string_refer_to_original_code
					ret2 = InStr(buf, """")
					
					If ret2 < ret And ret2 > 0 Then
						'Invalid_string_refer_to_original_code
						in_quote = True
						j = ret2 + 1
						Do While j <= Len(buf)
							Select Case Mid(buf, j, 1)
								Case """"
									in_quote = Not in_quote
								Case ","
									If Not in_quote Then
										buf2 = Left(buf, j - 1)
										buf = Mid(buf, j + 1)
									End If
							End Select
							j = j + 1
						Loop 
						If j = Len(buf) Then
							buf2 = buf
							buf = ""
						End If
						in_quote = False
					ElseIf ret > 0 Then 
						'Invalid_string_refer_to_original_code
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						
						'Invalid_string_refer_to_original_code
						If buf = "" Then
							If i Mod 2 = 1 Then
								err_msg = "Invalid_string_refer_to_original_code"
							Else
								err_msg = "Invalid_string_refer_to_original_code"
							End If
							Error(0)
						End If
					Else
						'Invalid_string_refer_to_original_code
						buf2 = buf
						buf = ""
					End If
					
					If i Mod 2 = 1 Then
						'Invalid_string_refer_to_original_code
						
						If InStr(buf2, " ") > 0 Then
							If aname = "" Then
								err_msg = "陦碁ｭ縺九ｉ" & VB6.Format((i + 1) \ 2) & "Invalid_string_refer_to_original_code"
							Else
								err_msg = "Invalid_string_refer_to_original_code"
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							End If
							Error(0)
						End If
						
						'Invalid_string_refer_to_original_code
						j = InStr(buf2, "=")
						If j > 0 Then
							adata = Mid(buf2, j + 1)
							buf2 = Left(buf2, j - 1)
						Else
							adata = ""
						End If
						
						'Invalid_string_refer_to_original_code
						j = InStr(buf2, "Lv")
						Select Case j
							Case 0
								'Invalid_string_refer_to_original_code
								aname = buf2
								alevel = DEFAULT_LEVEL
							Case 1
								'Invalid_string_refer_to_original_code
								If Not IsNumeric(Mid(buf2, j + 2)) Then
									err_msg = "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									Error(0)
								End If
								
								alevel = CDbl(Mid(buf2, j + 2))
								If aname = "" Then
									err_msg = "陦碁ｭ縺九ｉ" & VB6.Format((i + 1) \ 2) & "Invalid_string_refer_to_original_code"
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									Error(0)
								End If
							Case Else
								'Invalid_string_refer_to_original_code
								aname = Left(buf2, j - 1)
								alevel = CDbl(Mid(buf2, j + 2))
						End Select
					Else
						'Invalid_string_refer_to_original_code
						If IsNumeric(buf2) Then
							.AddSkill(aname, alevel, adata, CShort(buf2))
						Else
							If alevel > 0 Then
								DataErrorMessage("Invalid_string_refer_to_original_code")
								aname & "Lv" & Format$(alevel) _
								Invalid_string_refer_to_original_code_
								fname, line_num, line_buf, data_name
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							Else
								DataErrorMessage("Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code_
								'fname, line_num, line_buf, data_name
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							End If
							.AddSkill(aname, alevel, adata, 1)
						End If
					End If
				Loop While ret > 0
				
				'Invalid_string_refer_to_original_code
				If i Mod 2 = 1 Then
					If alevel > 0 Then
						DataErrorMessage("Invalid_string_refer_to_original_code")
						aname & "Lv" & Format$(alevel) _
						Invalid_string_refer_to_original_code_
						fname, line_num, line_buf, data_name
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					Else
						DataErrorMessage("Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code_
						'fname, line_num, line_buf, data_name
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					End If
				End If
				
				GetLine(FileNumber, line_buf, line_num)
				err_msg = "Invalid_string_refer_to_original_code"
				Error(0)
				'End If
				
				'Invalid_string_refer_to_original_code
				If Len(line_buf) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If IsNumeric(buf2) Then
					.Infight = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Shooting = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'蜻ｽ荳ｭ
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Hit = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'蝗樣∩
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Dodge = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Technique = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Intuition = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'諤ｧ譬ｼ
				buf2 = Trim(buf)
				If Len(buf2) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				If InStr(buf2, ",") > 0 Then
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
					buf2 = Trim(Left(buf2, InStr(buf2, ",") - 1))
				End If
				If Not IsNumeric(buf2) Then
					.Personality = buf2
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ
				GetLine(FileNumber, line_buf, line_num)
				Select Case line_buf
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						'Invalid_string_refer_to_original_code
					Case ""
						err_msg = "Invalid_string_refer_to_original_code"
						Error(0)
					Case Else
						ret = InStr(line_buf, ",")
						If ret = 0 Then
							err_msg = "Invalid_string_refer_to_original_code"
							Error(0)
						End If
						buf2 = Trim(Left(line_buf, ret - 1))
						buf = Mid(line_buf, ret + 1)
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						err_msg = "Invalid_string_refer_to_original_code"
						Error(0)
						'End If
						
						'Invalid_string_refer_to_original_code
						ret = InStr(buf, ",")
						If ret > 0 Then
							buf2 = Trim(Left(buf, ret - 1))
							buf = Mid(buf, ret + 1)
						Else
							buf2 = Trim(buf)
							buf = ""
						End If
						If IsNumeric(buf2) Then
							.SP = MinLng(CInt(buf2), 9999)
						Else
							DataErrorMessage("Invalid_string_refer_to_original_code")
							fname( , line_num, line_buf, data_name)
							.SP = 1
						End If
						
						'繧ｹ繝壹す繝｣繝ｫ繝代Ρ繝ｼ縺ｨ迯ｲ蠕励Ξ繝吶Ν
						ret = InStr(buf, ",")
						Do While ret > 0
							sname = Trim(Left(buf, ret - 1))
							buf = Mid(buf, ret + 1)
							
							'Invalid_string_refer_to_original_code
							If InStr(sname, "=") > 0 Then
								sp_cost = StrToLng(Mid(sname, InStr(sname, "=") + 1))
								sname = Left(sname, InStr(sname, "=") - 1)
							Else
								sp_cost = 0
							End If
							
							ret = InStr(buf, ",")
							If ret = 0 Then
								buf2 = Trim(buf)
								buf = ""
							Else
								buf2 = Trim(Left(buf, ret - 1))
								buf = Mid(buf, ret + 1)
							End If
							
							If sname = "" Then
								DataErrorMessage("Invalid_string_refer_to_original_code")
								fname( , line_num, line_buf, data_name)
							ElseIf Not SPDList.IsDefined(sname) Then 
								DataErrorMessage(sname & "Invalid_string_refer_to_original_code")
								fname( , line_num, line_buf, data_name)
							ElseIf Not IsNumeric(buf2) Then 
								DataErrorMessage("Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code_
								'fname, line_num, line_buf, data_name
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								.AddSpecialPower(sname, 1, sp_cost)
							Else
								.AddSpecialPower(sname, CShort(buf2), sp_cost)
							End If
							
							ret = InStr(buf, ",")
						Loop 
						
						If buf <> "" Then
							DataErrorMessage("Invalid_string_refer_to_original_code")
							'Invalid_string_refer_to_original_code_
							'fname, line_num, line_buf, data_name
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						End If
				End Select
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				
				'Invalid_string_refer_to_original_code
				If Len(line_buf) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If LCase(Right(buf2, 4)) = ".bmp" Then
					.Bitmap = buf2
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
					.IsBitmapMissing = True
				End If
				
				'Invalid_string_refer_to_original_code
				buf = Trim(buf)
				buf2 = buf
				Do While Right(buf2, 1) = ")"
					buf2 = Left(buf2, Len(buf2) - 1)
				Loop 
				Select Case LCase(Right(buf2, 4))
					Case ".mid", ".mp3", ".wav", "-"
						.BGM = buf
					Case ""
						DataErrorMessage("Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
						.Bitmap = "-.mid"
					Case Else
						DataErrorMessage("Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
						.Bitmap = "-.mid"
				End Select
				
				If EOF(FileNumber) Then
					FileClose(FileNumber)
					Exit Sub
				End If
				
				GetLine(FileNumber, line_buf, line_num)
				
				If line_buf <> "===" Then
					GoTo SkipRest
				End If
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				
				buf = line_buf
				i = 0
				Do While line_buf <> "==="
					i = i + 1
					
					ret = 0
					in_quote = False
					For j = 1 To Len(buf)
						Select Case Mid(buf, j, 1)
							Case ","
								If Not in_quote Then
									ret = j
									Exit For
								End If
							Case """"
								in_quote = Not in_quote
						End Select
					Next 
					
					If ret > 0 Then
						buf2 = Trim(Left(buf, ret - 1))
						buf = Trim(Mid(buf, ret + 1))
					Else
						buf2 = buf
						buf = ""
					End If
					
					If buf2 = "" Or IsNumeric(buf2) Then
						DataErrorMessage("陦碁ｭ縺九ｉ" & VB6.Format(i) & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
					Else
						.AddFeature(buf2)
					End If
					
					If buf = "" Then
						If EOF(FileNumber) Then
							FileClose(FileNumber)
							Exit Sub
						End If
						
						GetLine(FileNumber, line_buf, line_num)
						buf = line_buf
						i = 0
						
						If line_buf = "" Or line_buf = "===" Then
							Exit Do
						End If
					End If
				Loop 
				
				If line_buf <> "===" Then
					GoTo SkipRest
				End If
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0 And line_buf <> "==="
					'Invalid_string_refer_to_original_code
					ret = InStr(line_buf, ",")
					If ret = 0 Then
						err_msg = "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					wname = Trim(Left(line_buf, ret - 1))
					buf = Mid(line_buf, ret + 1)
					
					If wname = "" Then
						err_msg = "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					
					'豁ｦ蝎ｨ繧堤匳骭ｲ
					wd = .AddWeapon(wname)
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						wd.Power = MinLng(CInt(buf2), 99999)
					ElseIf buf = "-" Then 
						wd.Power = 0
					Else
						DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Power = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						wd.MinRange = CShort(buf2)
					Else
						DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
						wd.MinRange = 1
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.MinRange = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						wd.MaxRange = MinLng(CInt(buf2), 99)
					Else
						DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
						wd.MaxRange = 1
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.MaxRange = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						n = CInt(buf2)
						If n > 999 Then
							n = 999
						ElseIf n < -999 Then 
							n = -999
						End If
						wd.Precision = n
					Else
						DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Precision = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'蠑ｾ謨ｰ
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							wd.Bullet = MinLng(CInt(buf2), 99)
						Else
							DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
							fname( , line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								wd.Bullet = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							wd.ENConsumption = MinLng(CInt(buf2), 999)
						Else
							DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
							fname( , line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								wd.ENConsumption = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							n = CInt(buf2)
							If n > 1000 Then
								n = 1000
							ElseIf n < 0 Then 
								n = 0
							End If
							wd.NecessaryMorale = n
						Else
							DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
							fname( , line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								wd.NecessaryMorale = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If Len(buf2) = 4 Then
						wd.Adaption = buf2
					Else
						DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
						wd.Adaption = "----"
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Adaption = LIndex(buf2, 1)
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						n = CInt(buf2)
						If n > 999 Then
							n = 999
						ElseIf n < -999 Then 
							n = -999
						End If
						wd.Critical = n
					Else
						DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Critical = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'豁ｦ蝎ｨ螻樊ｧ
					buf = Trim(buf)
					If Len(buf) = 0 Then
						DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
					End If
					If Right(buf, 1) = ")" Then
						'Invalid_string_refer_to_original_code
						ret = InStr(buf, "> ")
						If ret > 0 Then
							If ret > 0 Then
								wd.NecessarySkill = Mid(buf, ret + 2)
								buf = Trim(Left(buf, ret + 1))
								ret = InStr(wd.NecessarySkill, "(")
								wd.NecessarySkill = Mid(wd.NecessarySkill, ret + 1, Len(wd.NecessarySkill) - ret - 1)
							End If
						Else
							ret = InStr(buf, "(")
							If ret > 0 Then
								wd.NecessarySkill = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
								buf = Trim(Left(buf, ret - 1))
							End If
						End If
					End If
					If Right(buf, 1) = ">" Then
						'Invalid_string_refer_to_original_code
						ret = InStr(buf, "<")
						If ret > 0 Then
							wd.NecessaryCondition = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
							buf = Trim(Left(buf, ret - 1))
						End If
					End If
					wd.Class_Renamed = buf
					If wd.Class_Renamed = "-" Then
						wd.Class_Renamed = ""
					End If
					If InStr(wd.Class_Renamed, "Lv") > 0 Then
						DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
					End If
					
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Loop 
				
				If line_buf <> "===" Then
					GoTo SkipRest
				End If
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0
					'Invalid_string_refer_to_original_code
					ret = InStr(line_buf, ",")
					If ret = 0 Then
						err_msg = "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					sname = Trim(Left(line_buf, ret - 1))
					buf = Mid(line_buf, ret + 1)
					
					If sname = "" Then
						err_msg = "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					
					'Invalid_string_refer_to_original_code
					sd = .AddAbility(sname)
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					sd.SetEffect(buf2)
					
					'Invalid_string_refer_to_original_code
					sd.MinRange = 0
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						sd.MaxRange = MinLng(CInt(buf2), 99)
					ElseIf buf2 = "-" Then 
						sd.MaxRange = 0
					Else
						DataErrorMessage(sname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							sd.MaxRange = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'蝗樊焚
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							sd.Stock = MinLng(CInt(buf2), 99)
						Else
							DataErrorMessage(sname & "Invalid_string_refer_to_original_code")
							fname( , line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								sd.Stock = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							sd.ENConsumption = MinLng(CInt(buf2), 999)
						Else
							DataErrorMessage(sname & "Invalid_string_refer_to_original_code")
							fname( , line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								sd.ENConsumption = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "Invalid_string_refer_to_original_code"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							n = CInt(buf2)
							If n > 1000 Then
								n = 1000
							ElseIf n < 0 Then 
								n = 0
							End If
							sd.NecessaryMorale = n
						Else
							DataErrorMessage(sname & "Invalid_string_refer_to_original_code")
							fname( , line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								sd.NecessaryMorale = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					buf = Trim(buf)
					If Len(buf) = 0 Then
						DataErrorMessage(sname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
					End If
					If Right(buf, 1) = ")" Then
						'Invalid_string_refer_to_original_code
						ret = InStr(buf, "> ")
						If ret > 0 Then
							If ret > 0 Then
								sd.NecessarySkill = Mid(buf, ret + 2)
								buf = Trim(Left(buf, ret + 1))
								ret = InStr(sd.NecessarySkill, "(")
								sd.NecessarySkill = Mid(sd.NecessarySkill, ret + 1, Len(sd.NecessarySkill) - ret - 1)
							End If
						Else
							ret = InStr(buf, "(")
							If ret > 0 Then
								sd.NecessarySkill = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
								buf = Trim(Left(buf, ret - 1))
							End If
						End If
					End If
					If Right(buf, 1) = ">" Then
						'Invalid_string_refer_to_original_code
						ret = InStr(buf, "<")
						If ret > 0 Then
							sd.NecessaryCondition = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
							buf = Trim(Left(buf, ret - 1))
						End If
					End If
					sd.Class_Renamed = buf
					If sd.Class_Renamed = "-" Then
						sd.Class_Renamed = ""
					End If
					If InStr(sd.Class_Renamed, "Lv") > 0 Then
						DataErrorMessage(sname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, data_name)
					End If
					
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Loop 
			End With
SkipRest: 
		Loop 
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
		If line_num = 0 Then
			ErrorMessage(fname & "縺碁幕縺代∪縺帙ｓ")
		Else
			FileClose(FileNumber)
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
End Class