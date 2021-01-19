Option Strict Off
Option Explicit On
Friend Class TerrainDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Count As Short
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: TerrainDataList は TerrainDataList_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private TerrainDataList_Renamed(MAX_TERRAIN_DATA_NUM) As TerrainData
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_WARNING: 配列 OrderList の下限が 1 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
	Private OrderList(MAX_TERRAIN_DATA_NUM) As Short
	
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		Dim i As Short
		
		For i = 0 To MAX_TERRAIN_DATA_NUM
			TerrainDataList_Renamed(i) = New TerrainData
		Next 
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'繧ｯ繝ｩ繧ｹ縺ｮ隗｣謾ｾ
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		For i = 0 To MAX_TERRAIN_DATA_NUM
			'UPGRADE_NOTE: オブジェクト TerrainDataList_Renamed() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			TerrainDataList_Renamed(i) = Nothing
		Next 
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined(ByVal ID As Short) As Boolean
		If TerrainDataList_Renamed(ID).ID >= 0 Then
			IsDefined = True
		Else
			IsDefined = False
		End If
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function Item(ByVal ID As Short) As TerrainData
		Item = TerrainDataList_Renamed(ID)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Name(ByVal ID As Short) As String
		Name = TerrainDataList_Renamed(ID).Name
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Bitmap(ByVal ID As Short) As String
		Bitmap = TerrainDataList_Renamed(ID).Bitmap_Renamed
	End Function
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Function Class_Renamed(ByVal ID As Short) As String
		Class_Renamed = TerrainDataList_Renamed(ID).Class_Renamed
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function MoveCost(ByVal ID As Short) As Short
		MoveCost = TerrainDataList_Renamed(ID).MoveCost
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function HitMod(ByVal ID As Short) As Short
		HitMod = TerrainDataList_Renamed(ID).HitMod
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function DamageMod(ByVal ID As Short) As Short
		DamageMod = TerrainDataList_Renamed(ID).DamageMod
	End Function
	
	
	'Invalid_string_refer_to_original_code
	
	Public Function IsFeatureAvailable(ByVal ID As Short, ByRef ftype As String) As Boolean
		IsFeatureAvailable = TerrainDataList_Renamed(ID).IsFeatureAvailable(ftype)
	End Function
	
	Public Function FeatureLevel(ByVal ID As Short, ByRef ftype As String) As Double
		FeatureLevel = TerrainDataList_Renamed(ID).FeatureLevel(ftype)
	End Function
	
	Public Function FeatureData(ByVal ID As Short, ByRef ftype As String) As String
		FeatureData = TerrainDataList_Renamed(ID).FeatureData(ftype)
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function OrderedID(ByVal n As Short) As Short
		OrderedID = OrderList(n)
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim ret As Short
		Dim line_num As Integer
		Dim i, j As Short
		Dim buf, line_buf, buf2 As String
		Dim td As TerrainData
		Dim data_id As Short
		Dim data_name As String
		Dim err_msg As String
		Dim in_quote As Boolean
		
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
			
			'逡ｪ蜿ｷ
			If IsNumeric(line_buf) Then
				data_id = CShort(line_buf)
			Else
				err_msg = "Invalid_string_refer_to_original_code"
				Error(0)
			End If
			If data_id < 0 Or data_id >= MAX_TERRAIN_DATA_NUM Then
				err_msg = "Invalid_string_refer_to_original_code"
				Error(0)
			End If
			
			td = TerrainDataList_Renamed(data_id)
			
			With td
				'Invalid_string_refer_to_original_code
				If .ID < 0 Then
					Count = Count + 1
					OrderList(Count) = data_id
				Else
					.Clear()
				End If
				.ID = data_id
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				
				'蜷咲ｧｰ
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				data_name = Trim(Left(line_buf, ret - 1))
				.Name = data_name
				buf = Mid(line_buf, ret + 1)
				
				'Invalid_string_refer_to_original_code
				.Bitmap_Renamed = Trim(buf)
				If Len(.Bitmap_Renamed) = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
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
				.Class_Renamed = buf2
				
				'Invalid_string_refer_to_original_code
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If buf2 = "-" Then
					.MoveCost = 1000
				ElseIf IsNumeric(buf2) Then 
					'Invalid_string_refer_to_original_code
					.MoveCost = CShort(2 * CDbl(buf2))
				End If
				If .MoveCost <= 0 Then
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'蜻ｽ荳ｭ菫ｮ豁｣
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.HitMod = CShort(buf2)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'繝繝｡繝ｼ繧ｸ菫ｮ豁｣
				ret = InStr(buf, ",")
				If ret > 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
				End If
				buf2 = Trim(buf)
				If IsNumeric(buf2) Then
					.DamageMod = CShort(buf2)
				Else
					DataErrorMessage("Invalid_string_refer_to_original_code")
					fname( , line_num, line_buf, data_name)
				End If
				
				'Invalid_string_refer_to_original_code
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0
					buf = line_buf
					i = 0
					Do While Len(buf) > 0
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
						
						If buf2 <> "" Then
							.AddFeature(buf2)
						Else
							DataErrorMessage("陦碁ｭ縺九ｉ" & VB6.Format(i) & "Invalid_string_refer_to_original_code")
							fname( , line_num, line_buf, data_name)
						End If
					Loop 
					
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
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		
		End
	End Sub
	
	'繝ｪ繧ｹ繝医ｒ繧ｯ繝ｪ繧｢
	Public Sub Clear()
		Dim i As Short
		
		For i = 0 To MAX_TERRAIN_DATA_NUM - 1
			TerrainDataList_Renamed(i).Clear()
		Next 
		Count = 0
	End Sub
End Class