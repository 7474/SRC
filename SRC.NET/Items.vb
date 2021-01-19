Option Strict Off
Option Explicit On
Friend Class Items
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Private IDCount As Integer
	
	'Invalid_string_refer_to_original_code
	Private colItems As New Collection
	
	'繧ｯ繝ｩ繧ｹ縺ｮ隗｣謾ｾ
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colItems
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colItems をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colItems = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ForEach逕ｨ髢｢謨ｰ
	'UPGRADE_NOTE: NewEnum プロパティがコメント アウトされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' をクリックしてください。
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colItems.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: コレクション列挙子を返すには、コメントを解除して以下の行を変更してください。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' をクリックしてください。
		'GetEnumerator = colItems.GetEnumerator
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Function Add(ByRef Name As String) As Item
		Dim new_item As Item
		
		If Not IDList.IsDefined(Name) Then
			'UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			Add = Nothing
			Exit Function
		End If
		
		new_item = New Item
		Add = new_item
		With new_item
			.Name = Name
			.ID = CreateID(Name)
		End With
		colItems.Add(new_item, new_item.ID)
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Function CreateID(ByRef iname As String) As String
		Do 
			IDCount = IDCount + 1
		Loop Until Not IsDefined2(iname & "_" & VB6.Format(IDCount))
		CreateID = iname & "_" & VB6.Format(IDCount)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Count() As Short
		Count = colItems.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Delete(ByRef Index As Object)
		colItems.Remove(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Item(ByRef Index As Object) As Item
		Dim it As Item
		Dim iname As String
		
		On Error GoTo ErrorHandler
		Item = colItems.Item(Index)
		
		'Invalid_string_refer_to_original_code
		If Item.Exist Then
			Exit Function
		End If
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		iname = CStr(Index)
		For	Each it In colItems
			With it
				If .Name = iname And .Exist Then
					Item = it
					Exit Function
				End If
			End With
		Next it
		'UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item = Nothing
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim it As Item
		Dim iname As String
		
		On Error GoTo ErrorHandler
		it = colItems.Item(Index)
		
		'Invalid_string_refer_to_original_code
		If it.Exist Then
			IsDefined = True
			Exit Function
		End If
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		iname = CStr(Index)
		For	Each it In colItems
			With it
				If .Name = iname And .Exist Then
					IsDefined = True
					Exit Function
				End If
			End With
		Next it
		IsDefined = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim it As Item
		
		On Error GoTo ErrorHandler
		it = colItems.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Update()
		Dim it As Item
		
		'Invalid_string_refer_to_original_code
		For	Each it In colItems
			With it
				If Not .Exist Then
					colItems.Remove(.ID)
				End If
			End With
		Next it
		
		'Invalid_string_refer_to_original_code
		For	Each it In colItems
			With it
				If Not .Unit_Renamed Is Nothing Then
					.Unit_Renamed = UList.Item((.Unit_Renamed.ID))
				End If
			End With
		Next it
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub Save()
		Dim i As Short
		
		WriteLine(SaveDataFileNumber, IDCount)
		WriteLine(SaveDataFileNumber, Count)
		For i = 1 To Count
			With Item(i)
				WriteLine(SaveDataFileNumber, .Name)
				If .Unit_Renamed Is Nothing Then
					WriteLine(SaveDataFileNumber, .ID, "-")
				Else
					WriteLine(SaveDataFileNumber, .ID, .Unit_Renamed.ID)
				End If
			End With
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub Load()
		Dim num As Short
		Dim new_item As Item
		Dim iname As String
		Dim iid As String
		Dim i As Short
		Dim dummy As String
		
		If EOF(SaveDataFileNumber) Then
			Exit Sub
		End If
		
		Input(SaveDataFileNumber, IDCount)
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			new_item = New Item
			With new_item
				'Name
				Input(SaveDataFileNumber, iname)
				'ID, Unit
				Input(SaveDataFileNumber, iid)
				Input(SaveDataFileNumber, dummy)
				
				If Not IDList.IsDefined(iname) Then
					ErrorMessage(iname & "Invalid_string_refer_to_original_code")
					StopBGM()
					End
				End If
				
				.Name = iname
				.ID = iid
			End With
			colItems.Add(new_item, iid)
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub LoadLinkInfo()
		Dim num, i As Short
		Dim dummy As String
		
		If EOF(SaveDataFileNumber) Then
			Exit Sub
		End If
		
		'IDCount
		dummy = LineInput(SaveDataFileNumber)
		
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			'Name
			dummy = LineInput(SaveDataFileNumber)
			'ID, Unit
			dummy = LineInput(SaveDataFileNumber)
		Next 
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub Dump()
		Dim it As Item
		
		WriteLine(SaveDataFileNumber, Count)
		For	Each it In colItems
			it.Dump()
		Next it
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub Restore()
		Dim i, num As Short
		Dim it As Item
		
		With colItems
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			it = New Item
			With it
				.Restore()
				colItems.Add(it, .ID)
			End With
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreLinkInfo()
		Dim it As Item
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each it In colItems
			it.RestoreLinkInfo()
		Next it
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreParameter()
		Dim it As Item
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each it In colItems
			it.RestoreParameter()
		Next it
	End Sub
	
	
	'繝ｪ繧ｹ繝医ｒ繧ｯ繝ｪ繧｢
	Public Sub Clear()
		Dim i As Short
		
		For i = 1 To Count
			Delete(1)
		Next 
	End Sub
End Class