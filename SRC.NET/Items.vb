Option Strict Off
Option Explicit On
Friend Class Items
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'アイテムＩＤ作成用カウンタ
	Private IDCount As Integer
	
	'アイテム一覧
	Private colItems As New Collection
	
	'クラスの解放
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
	
	'ForEach用関数
	'UPGRADE_NOTE: NewEnum プロパティがコメント アウトされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' をクリックしてください。
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colItems.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: コレクション列挙子を返すには、コメントを解除して以下の行を変更してください。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' をクリックしてください。
		'GetEnumerator = colItems.GetEnumerator
	End Function
	
	
	'リストにアイテムを追加
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
	
	'新しいアイテムＩＤを作成
	Private Function CreateID(ByRef iname As String) As String
		Do 
			IDCount = IDCount + 1
		Loop Until Not IsDefined2(iname & "_" & VB6.Format(IDCount))
		CreateID = iname & "_" & VB6.Format(IDCount)
	End Function
	
	'リストに登録されているアイテムの総数
	Public Function Count() As Short
		Count = colItems.Count()
	End Function
	
	'リストからアイテムを削除
	Public Sub Delete(ByRef Index As Object)
		colItems.Remove(Index)
	End Sub
	
	'指定されたアイテムを検索
	Public Function Item(ByRef Index As Object) As Item
		Dim it As Item
		Dim iname As String
		
		On Error GoTo ErrorHandler
		Item = colItems.Item(Index)
		
		'破棄されていない？
		If Item.Exist Then
			Exit Function
		End If
		
ErrorHandler: 
		'見つからなければアイテム名で検索
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
	
	'指定されたアイテムが登録されているか？
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim it As Item
		Dim iname As String
		
		On Error GoTo ErrorHandler
		it = colItems.Item(Index)
		
		'破棄されたアイテムは登録されていないとみなす
		If it.Exist Then
			IsDefined = True
			Exit Function
		End If
		
ErrorHandler: 
		'見つからなければアイテム名で検索
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
	
	'アイテム名とExitフラグを無視してアイテムを検索
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim it As Item
		
		On Error GoTo ErrorHandler
		it = colItems.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'リストに登録されたアイテムをアップデート
	Public Sub Update()
		Dim it As Item
		
		'破棄されたアイテムを削除
		For	Each it In colItems
			With it
				If Not .Exist Then
					colItems.Remove(.ID)
				End If
			End With
		Next it
		
		'リンクデータの整合性を取る
		For	Each it In colItems
			With it
				If Not .Unit_Renamed Is Nothing Then
					.Unit_Renamed = UList.Item((.Unit_Renamed.ID))
				End If
			End With
		Next it
	End Sub
	
	
	'データをファイルにセーブ
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
	
	'データをファイルからロード
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
					ErrorMessage(iname & "のデータが定義されていません")
					StopBGM()
					End
				End If
				
				.Name = iname
				.ID = iid
			End With
			colItems.Add(new_item, iid)
		Next 
	End Sub
	
	'リンク情報をファイルからロード
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
	
	
	'一時中断用データをファイルにセーブする
	Public Sub Dump()
		Dim it As Item
		
		WriteLine(SaveDataFileNumber, Count)
		For	Each it In colItems
			it.Dump()
		Next it
	End Sub
	
	'一時中断用データをファイルからロードする
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
	
	'一時中断用データのリンク情報をファイルからロードする
	Public Sub RestoreLinkInfo()
		Dim it As Item
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each it In colItems
			it.RestoreLinkInfo()
		Next it
	End Sub
	
	'一時中断用データのパラメータ情報をファイルからロードする
	Public Sub RestoreParameter()
		Dim it As Item
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each it In colItems
			it.RestoreParameter()
		Next it
	End Sub
	
	
	'リストをクリア
	Public Sub Clear()
		Dim i As Short
		
		For i = 1 To Count
			Delete(1)
		Next 
	End Sub
End Class