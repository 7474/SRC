Option Strict Off
Option Explicit On
Friend Class UnitDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'繝ｦ繝九ャ繝医ョ繝ｼ繧ｿ逕ｨ繧ｳ繝ｬ繧ｯ繧ｷ繝ｧ繝ｳ
	Private colUnitDataList As New Collection
	
	'Invalid_string_refer_to_original_code
	Private IDNum As Integer
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		Dim ud As New UnitData
		
		With ud
			.Name = "Invalid_string_refer_to_original_code"
			.Nickname = "Invalid_string_refer_to_original_code"
			.PilotNum = 1
			.Transportation = "髯ｸ"
			.Adaption = "AAAA"
			.Bitmap = ".bmp"
			.AddFeature("Invalid_string_refer_to_original_code")
		End With
		colUnitDataList.Add(ud, ud.Name)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'繧ｯ繝ｩ繧ｹ縺ｮ隗｣謾ｾ
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colUnitDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colUnitDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colUnitDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Add(ByRef uname As String) As UnitData
		Dim ud As New UnitData
		
		With ud
			.Name = uname
			colUnitDataList.Add(ud, uname)
			IDNum = IDNum + 1
			.ID = IDNum
		End With
		
		Add = ud
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Count() As Short
		Count = colUnitDataList.Count()
	End Function
	
	'繝ｦ繝九ャ繝医ョ繝ｼ繧ｿ繝ｪ繧ｹ繝医°繧峨ョ繝ｼ繧ｿ繧貞炎髯､
	Public Sub Delete(ByRef Index As Object)
		colUnitDataList.Remove(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Item(ByRef Index As Object) As UnitData
		On Error GoTo ErrorHandler
		
		Item = colUnitDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item = Nothing
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim dummy As UnitData
		
		On Error GoTo ErrorHandler
		dummy = colUnitDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim j, i, k As Short
		Dim n, line_num As Integer
		Dim buf, line_buf, buf2 As String
		Dim ret As Short
		Dim ud As UnitData
		Dim wd As WeaponData
		Dim sd As AbilityData
		Dim wname, sname As String
		Dim data_name As String
		Dim err_msg As String
		Dim in_quote As Boolean
		Dim comma_num As Short
		
		On Error GoTo ErrorHandler
		
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read)
		
		line_num = 0
		
		Do While True
			data_name = ""
			
			'Invalid_string_refer_to_original_code
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
				ud = Item(data_name)
				ud.Clear()
			Else
				ud = Add(data_name)
			End If
			
			With ud
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				If ret > 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				.KanaName = buf
				
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
				ElseIf comma_num > 4 Then 
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				
				'諢帷ｧｰ
				If Len(line_buf) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				ret = InStr(line_buf, ",")
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				.Nickname = buf2
				
				'Invalid_string_refer_to_original_code
				If comma_num = 4 Then
					ret = InStr(buf, ",")
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					.KanaName = buf2
				Else
					.KanaName = StrToHiragana(.Nickname)
				End If
				
				'繝ｦ繝九ャ繝医け繝ｩ繧ｹ
				ret = InStr(buf, ",")
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If Not IsNumeric(buf2) Then
					.Class_Renamed = buf2
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
					.Class_Renamed = "豎守畑"
				End If
				
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If Left(buf2, 1) <> "(" Then
					If IsNumeric(buf2) Then
						.PilotNum = MinLng(CInt(buf2), 99)
					Else
						DataErrorMessage("Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, .Name)
						.PilotNum = 1
					End If
					If .PilotNum < 1 Then
						DataErrorMessage("Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, .Name)
						.PilotNum = 1
					End If
				Else
					If Right(buf2, 1) <> ")" Then
						Error(0)
					End If
					buf2 = Mid(buf2, 2, Len(buf2) - 2)
					If IsNumeric(buf2) Then
						.PilotNum = MinLng(CInt(buf2), 99)
					Else
						DataErrorMessage("Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, .Name)
						.PilotNum = 1
					End If
					If .PilotNum < 1 Then
						DataErrorMessage("Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, .Name)
						.PilotNum = 1
					End If
					.PilotNum = -.PilotNum
				End If
				
				'Invalid_string_refer_to_original_code
				buf = Trim(buf)
				If Len(buf) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				If IsNumeric(buf) Then
					.ItemNum = MinLng(CInt(buf), 99)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
					.ItemNum = 4
				End If
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				
				'遘ｻ蜍募庄閭ｽ蝨ｰ蠖｢
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If Not IsNumeric(buf2) Then
					.Transportation = buf2
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
					.Transportation = "髯ｸ"
				End If
				
				'遘ｻ蜍募鴨
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Speed = MinLng(CInt(buf2), 99)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
				End If
				
				'繧ｵ繧､繧ｺ
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				Select Case buf2
					Case "XL", "LL", "L", "M", "S", "SS"
						.Size = buf2
					Case Else
						DataErrorMessage("Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, .Name)
						.Size = "M"
				End Select
				
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Value = MinLng(CInt(buf2), 9999999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
				End If
				
				'邨碁ｨ灘､
				buf = Trim(buf)
				If Len(buf) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				If IsNumeric(buf) Then
					.ExpValue = MinLng(CInt(buf), 9999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
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
				Do While True
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
						
						If j = 1 Then
							If IsNumeric(buf2) Then
								Exit Do
							End If
						End If
						
						buf = Trim(Mid(buf, ret + 1))
					Else
						buf2 = buf
						buf = ""
					End If
					
					If IsNumeric(buf2) Then
						Exit Do
					ElseIf buf2 = "" Or IsNumeric(buf2) Then 
						DataErrorMessage("陦碁ｭ縺九ｉ" & VB6.Format(i) & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, .Name)
					Else
						.AddFeature(buf2)
					End If
					
					If buf = "" Then
						GetLine(FileNumber, line_buf, line_num)
						buf = line_buf
						i = 0
					End If
				Loop 
				'UPGRADE_WARNING: Load に変換されていないステートメントがあります。ソース コードを確認してください。
				'Invalid_string_refer_to_original_code
				buf = Mid(line_buf, 6)
				
				ret = 0
				in_quote = False
				For k = 1 To Len(buf)
					Select Case Mid(buf, k, 1)
						Case ","
							If Not in_quote Then
								ret = k
								Exit For
							End If
						Case """"
							in_quote = Not in_quote
					End Select
				Next 
				
				i = 0
				Do While ret > 0
					i = i + 1
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					ret = InStr(buf, ",")
					If buf2 <> "" Then
						.AddFeature(buf2)
					Else
						DataErrorMessage(VB6.Format(i) & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, .Name)
					End If
				Loop 
				
				i = i + 1
				buf2 = Trim(buf)
				If buf2 <> "" Then
					.AddFeature(buf2)
				Else
					DataErrorMessage(VB6.Format(i) & "Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
				End If
				
				GetLine(FileNumber, line_buf, line_num)
				err_msg = "Invalid_string_refer_to_original_code"
				Error(0)
				'End If
				
				'Invalid_string_refer_to_original_code
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If IsNumeric(buf2) Then
					.HP = MinLng(CInt(buf2), 9999999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
					.HP = 1000
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
					.EN = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
					.EN = 100
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
					.Armor = MinLng(CInt(buf2), 99999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
				End If
				
				'驕句虚諤ｧ
				buf2 = Trim(buf)
				If Len(buf2) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				If IsNumeric(buf2) Then
					.Mobility = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
				End If
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				
				'Invalid_string_refer_to_original_code
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If Len(buf2) = 4 Then
					.Adaption = buf2
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
					.Adaption = "AAAA"
				End If
				
				'Invalid_string_refer_to_original_code
				buf = Trim(buf)
				If Len(buf) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				If LCase(Right(buf, 4)) = ".bmp" Then
					.Bitmap = buf
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, .Name)
					.IsBitmapMissing = True
				End If
				
				If EOF(FileNumber) Then
					FileClose(FileNumber)
					Exit Sub
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
						fname( , line_num, line_buf, .Name)
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
						fname( , line_num, line_buf, .Name)
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
						fname( , line_num, line_buf, .Name)
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
						fname( , line_num, line_buf, .Name)
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
							fname( , line_num, line_buf, .Name)
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
							fname( , line_num, line_buf, .Name)
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
							fname( , line_num, line_buf, .Name)
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
						fname( , line_num, line_buf, .Name)
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
						fname( , line_num, line_buf, .Name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Critical = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'豁ｦ蝎ｨ螻樊ｧ
					buf = Trim(buf)
					If Len(buf) = 0 Then
						DataErrorMessage(wname & "Invalid_string_refer_to_original_code")
						fname( , line_num, line_buf, .Name)
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
						fname( , line_num, line_buf, .Name)
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
						fname( , line_num, line_buf, .Name)
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
							fname( , line_num, line_buf, .Name)
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
							fname( , line_num, line_buf, .Name)
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
							fname( , line_num, line_buf, .Name)
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
						fname( , line_num, line_buf, .Name)
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
						fname( , line_num, line_buf, .Name)
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
			ErrorMessage(fname & "Invalid_string_refer_to_original_code")
		Else
			FileClose(FileNumber)
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
End Class