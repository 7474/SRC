Option Strict Off
Option Explicit On
Friend Class AliasDataList
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	
	'Invalid_string_refer_to_original_code
	Private colAliasDataList As New Collection
	
	
	'繧ｯ繝ｩ繧ｹ縺ｮ隗｣謾ｾ
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colAliasDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colAliasDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colAliasDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Add(ByRef aname As String) As AliasData
		Dim ad As New AliasData
		
		ad.Name = aname
		colAliasDataList.Add(ad, aname)
		Add = ad
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Count() As Short
		Count = colAliasDataList.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Delete(ByRef Index As Object)
		colAliasDataList.Remove(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Item(ByRef Index As Object) As AliasData
		Item = colAliasDataList.Item(Index)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim ad As AliasData
		
		On Error GoTo ErrorHandler
		ad = colAliasDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim line_num As Integer
		Dim line_buf As String
		Dim ad As AliasData
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
			
			If IsDefined(data_name) Then
				'Invalid_string_refer_to_original_code
				Delete(data_name)
			End If
			ad = Add(data_name)
			
			With ad
				Do While True
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
					
					If Len(line_buf) = 0 Then
						Exit Do
					End If
					
					.AddAlias(line_buf)
				Loop 
				If .Count = 0 Then
					err_msg = "Invalid_string_refer_to_original_code"
					Error(0)
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
	
	'ForEach逕ｨ髢｢謨ｰ
	'UPGRADE_NOTE: NewEnum プロパティがコメント アウトされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' をクリックしてください。
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colAliasDataList.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: コレクション列挙子を返すには、コメントを解除して以下の行を変更してください。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' をクリックしてください。
		'GetEnumerator = colAliasDataList.GetEnumerator
	End Function
End Class