Option Strict Off
Option Explicit On
Friend Class Units
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'全ユニットのデータを管理するリストのクラス
	
	'ユニットＩＤ作成用カウンタ
	Public IDCount As Integer
	
	'ユニット一覧
	Private colUnits As New Collection
	
	'クラスの初期化
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		IDCount = 0
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colUnits
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colUnits をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colUnits = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ForEach用関数
	'UPGRADE_NOTE: NewEnum プロパティがコメント アウトされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' をクリックしてください。
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colUnits.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: コレクション列挙子を返すには、コメントを解除して以下の行を変更してください。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' をクリックしてください。
		'GetEnumerator = colUnits.GetEnumerator
	End Function
	
	
	'ユニットリストに新しいユニットを追加
	Public Function Add(ByRef uname As String, ByVal urank As Short, ByRef uparty As String) As Unit
		Dim new_unit As Unit
		Dim new_form As Unit
		Dim ud As UnitData
		Dim uname2 As String
		Dim other_forms() As String
		Dim i, j As Short
		Dim list As String
		
		'ユニットデータが定義されている？
		If Not UDList.IsDefined(uname) Then
			'UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			Add = Nothing
			Exit Function
		End If
		
		ud = UDList.Item(uname)
		
		new_unit = New Unit
		Add = new_unit
		With new_unit
			.Name = ud.Name
			.Rank = urank
			.Party = uparty
			.ID = CreateID((ud.Name))
			.FullRecover()
		End With
		colUnits.Add(new_unit, new_unit.ID)
		
		'これ以降は本体以外の形態の追加
		ReDim other_forms(0)
		
		'変形先の形態
		list = ud.FeatureData("変形")
		For i = 2 To LLength(list)
			uname2 = LIndex(list, i)
			If Not UDList.IsDefined(uname2) Then
				ErrorMessage("ユニットデータ「" & uname & "」の変形先形態「" & uname2 & "」が見つかりません")
				'UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		Next 
		
		'ハイパーモード先の形態
		If ud.IsFeatureAvailable("ハイパーモード") Then
			list = ud.FeatureData("ハイパーモード")
			uname2 = LIndex(list, 2)
			If Not UDList.IsDefined(uname2) Then
				If uname = "" Then
					ErrorMessage("ユニットデータ「" & uname & "」のハイパーモード先形態が指定されていません")
				Else
					ErrorMessage("ユニットデータ「" & uname & "」のハイパーモード先形態「" & uname2 & "」が見つかりません")
				End If
				'UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		End If
		
		'ノーマルモード先の形態
		If ud.IsFeatureAvailable("ノーマルモード") Then
			list = ud.FeatureData("ノーマルモード")
			uname2 = LIndex(list, 1)
			If Not UDList.IsDefined(uname2) Then
				If uname2 = "" Then
					ErrorMessage("ユニットデータ「" & uname & "」のノーマルモード先形態が指定されていません")
				Else
					ErrorMessage("ユニットデータ「" & uname & "」のノーマルモード先形態「" & uname2 & "」が見つかりません")
				End If
				'UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		End If
		
		'パーツ分離先の形態
		If ud.IsFeatureAvailable("パーツ分離") Then
			uname2 = LIndex(ud.FeatureData("パーツ分離"), 2)
			If Not UDList.IsDefined(uname2) Then
				If uname2 = "" Then
					ErrorMessage("ユニットデータ「" & uname & "」のパーツ分離先形態が指定されていません")
				Else
					ErrorMessage("ユニットデータ「" & uname & "」のパーツ分離先形態「" & uname2 & "」が見つかりません")
				End If
				'UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		End If
		
		'パーツ合体先の形態
		If ud.IsFeatureAvailable("パーツ合体") Then
			uname2 = ud.FeatureData("パーツ合体")
			If Not UDList.IsDefined(uname2) Then
				If uname = "" Then
					ErrorMessage("ユニットデータ「" & uname & "」のパーツ合体先形態が指定されていません")
				Else
					ErrorMessage("ユニットデータ「" & uname & "」のパーツ合体先形態「" & uname2 & "」が見つかりません")
				End If
				'UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		End If
		
		'変形技の変形先の形態
		With ud
			If .IsFeatureAvailable("変形技") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "変形技" Then
						uname2 = LIndex(.FeatureData(i), 2)
						If Not UDList.IsDefined(uname2) Then
							If uname2 = "" Then
								ErrorMessage("ユニットデータ「" & uname & "」の変形技使用後形態が指定されていません")
							Else
								ErrorMessage("ユニットデータ「" & uname & "」の変形技使用後形態「" & uname2 & "」が見つかりません")
							End If
							'UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							Add = Nothing
							Exit Function
						End If
						ReDim Preserve other_forms(UBound(other_forms) + 1)
						other_forms(UBound(other_forms)) = uname2
					End If
				Next 
			End If
		End With
		
		'換装先の形態
		list = ud.FeatureData("換装")
		For i = 1 To LLength(list)
			uname2 = LIndex(list, i)
			If Not UDList.IsDefined(uname2) Then
				ErrorMessage("ユニットデータ「" & uname & "」の換装先形態「" & uname2 & "」が見つかりません")
				'UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		Next 
		
		'他形態で指定された形態
		list = ud.FeatureData("他形態")
		For i = 1 To LLength(list)
			uname2 = LIndex(list, i)
			If Not UDList.IsDefined(uname2) Then
				ErrorMessage("ユニットデータ「" & uname & "」の他形態「" & uname2 & "」が見つかりません")
				'UPGRADE_NOTE: オブジェクト Add をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				Add = Nothing
				Exit Function
			End If
			ReDim Preserve other_forms(UBound(other_forms) + 1)
			other_forms(UBound(other_forms)) = uname2
		Next 
		
		'形態を追加
		For i = 1 To UBound(other_forms)
			If Not new_unit.IsOtherFormDefined(other_forms(i)) Then
				new_form = New Unit
				With new_form
					.Name = other_forms(i)
					.Rank = urank
					.Party = uparty
					.ID = CreateID((ud.Name))
					.FullRecover()
					.Status_Renamed = "他形態"
				End With
				colUnits.Add(new_form, new_form.ID)
				new_unit.AddOtherForm(new_form)
			End If
		Next 
		
		'追加した形態に対して自分自身を追加しておく
		For i = 1 To new_unit.CountOtherForm
			new_unit.OtherForm(i).AddOtherForm(new_unit)
			For j = 1 To new_unit.CountOtherForm
				If Not i = j Then
					new_unit.OtherForm(i).AddOtherForm(new_unit.OtherForm(j))
				End If
			Next 
		Next 
		
		'既に合体先 or 分離先のユニットが作成されていれば自分は他形態
		With ud
			For i = 1 To .CountFeature
				If .Feature(i) = "合体" Then
					If UList.IsDefined(LIndex(.FeatureData(i), 2)) Then
						new_unit.Status_Renamed = "他形態"
						Exit Function
					End If
				End If
				If .Feature(i) = "分離" Then
					For j = 2 To LLength(.FeatureData(i))
						If UList.IsDefined(LIndex(.FeatureData(i), j)) Then
							new_unit.Status_Renamed = "他形態"
							Exit Function
						End If
					Next 
				End If
			Next 
		End With
	End Function
	
	'ユニットリストにユニット u を追加
	Public Sub Add2(ByRef u As Unit)
		colUnits.Add(u, u.ID)
	End Sub
	
	'新規ユニットIDを作成
	Public Function CreateID(ByRef uname As String) As String
		Do 
			IDCount = IDCount + 1
		Loop Until Not IsDefined(uname & ":" & VB6.Format(IDCount))
		CreateID = uname & ":" & VB6.Format(IDCount)
	End Function
	
	'ユニットリストに登録されているユニット数を返す
	Public Function Count() As Short
		Count = colUnits.Count()
	End Function
	
	'ユニットリストからユニットを削除
	Public Sub Delete(ByRef Index As Object)
		colUnits.Remove(Index)
	End Sub
	
	'ユニットリストから指定されたユニットを返す
	Public Function Item(ByRef Index As Object) As Unit
		Dim u As Unit
		Dim uname As String
		
		On Error GoTo ErrorHandler
		Item = colUnits.Item(Index)
		Exit Function
		
ErrorHandler: 
		'IDで見つからなければユニット名で検索
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		uname = CStr(Index)
		For	Each u In colUnits
			With u
				If .Name = uname Then
					If .Status_Renamed <> "破棄" Then
						Item = u
						Exit Function
					End If
				End If
			End With
		Next u
		'UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item = Nothing
	End Function
	
	'ユニットリストからユニットを検索 (IDのみ)
	Public Function Item2(ByRef Index As Object) As Unit
		On Error GoTo ErrorHandler
		Item2 = colUnits.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: オブジェクト Item2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item2 = Nothing
	End Function
	
	'ユニットリストに指定されたユニットが定義されているか？
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim u As Unit
		Dim uname As String
		
		On Error GoTo ErrorHandler
		u = colUnits.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		'IDで見つからなければユニット名で検索
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		uname = CStr(Index)
		For	Each u In colUnits
			With u
				If .Name = uname Then
					If .Status_Renamed <> "破棄" Then
						IsDefined = True
						Exit Function
					End If
				End If
			End With
		Next u
		IsDefined = False
	End Function
	
	'ユニットリストに指定されたユニットが定義されているか？ (IDのみ)
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim u As Unit
		
		On Error GoTo ErrorHandler
		u = colUnits.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'ユニットリストをアップデート
	Public Sub Update()
		Dim u As Unit
		Dim k, i, j, n As Short
		Dim prev_money As Integer
		Dim flag As Boolean
		Dim pname, uname, uname2, buf As String
		
		'母艦に格納されたユニットを降ろす
		For	Each u In colUnits
			With u
				For i = 1 To .CountUnitOnBoard
					.UnloadUnit(1)
				Next 
			End With
		Next u
		
		'破壊された味方ユニットがあるか検索
		For	Each u In colUnits
			With u
				If .Party0 = "味方" Then
					If .Status_Renamed = "破壊" Then
						flag = True
						Exit For
					End If
				ElseIf .Party0 = "ＮＰＣ" Then 
					If .Status_Renamed = "破壊" Then
						If Not .Summoner Is Nothing Then
							If .Summoner.Party0 = "味方" Then
								flag = True
								Exit For
							End If
						End If
					End If
				End If
			End With
		Next u
		
		'破壊された味方ユニットがあれば修理
		If flag Then
			OpenMessageForm()
			prev_money = Money
			For	Each u In colUnits
				With u
					If .Status_Renamed <> "破壊" Then
						GoTo NextDestroyedUnit
					End If
					If .IsFeatureAvailable("召喚ユニット") Then
						GoTo NextDestroyedUnit
					End If
					Select Case .Party0
						Case "味方"
						Case "ＮＰＣ"
							If .Summoner Is Nothing Then
								GoTo NextDestroyedUnit
							ElseIf .Summoner.Party0 <> "味方" Then 
								GoTo NextDestroyedUnit
							End If
						Case Else
							GoTo NextDestroyedUnit
					End Select
					
					IncrMoney(-.Value)
					.Status_Renamed = "待機"
					If Not .IsHero Then
						DisplayMessage("システム", .Nickname & "を修理した;修理費 = " & VB6.Format(.Value))
					Else
						DisplayMessage("システム", .Nickname & "を治療した;治療費 = " & VB6.Format(.Value))
					End If
				End With
NextDestroyedUnit: 
			Next u
			DisplayMessage("システム", "合計 = " & VB6.Format(prev_money - Money))
			CloseMessageForm()
		End If
		
		'全ユニットを待機状態に変更
		For	Each u In colUnits
			With u
				Select Case .Status_Renamed
					Case "出撃", "格納"
						.Status_Renamed = "待機"
				End Select
			End With
		Next u
		
		'３段階までの変形・合体に対応
		For i = 1 To 3
			'ノーマルモード・パーツ合体を行う
			For	Each u In colUnits
				With u
					If .Party0 = "味方" And .Status_Renamed <> "他形態" And .Status_Renamed <> "旧主形態" And .Status_Renamed <> "旧形態" Then
						If .IsFeatureAvailable("ノーマルモード") Then
							.Transform(LIndex(.FeatureData("ノーマルモード"), 1))
						ElseIf .IsFeatureAvailable("パーツ合体") Then 
							If LLength(.FeatureData("パーツ合体")) = 2 Then
								.Transform(LIndex(.FeatureData("パーツ合体"), 2))
							Else
								.Transform(LIndex(.FeatureData("パーツ合体"), 1))
							End If
						End If
					End If
				End With
			Next u
			
			'分離を行う
			For	Each u In colUnits
				With u
					If Not .IsFeatureAvailable("分離") Then
						GoTo NextLoop1
					End If
					If .Party0 <> "味方" Or .Status_Renamed = "他形態" Or .Status_Renamed = "旧主形態" Or .Status_Renamed = "旧形態" Then
						GoTo NextLoop1
					End If
					
					If .Status_Renamed = "破棄" Then
						If .CountPilot = 0 Then
							GoTo NextLoop1
						End If
					End If
					
					'合体形態が主形態なら分離を行わない
					
					If LLength(.FeatureData("分離")) > 3 And Not .IsFeatureAvailable("制限時間") Then
						GoTo NextLoop1
					End If
					
					If .IsFeatureAvailable("主形態") Then
						GoTo NextLoop1
					End If
					
					'パイロットが足らない場合は分離を行わない
					n = 0
					For j = 2 To LLength(.FeatureData("分離"))
						uname = LIndex(.FeatureData("分離"), j)
						If UDList.IsDefined(uname) Then
							With UDList.Item(uname)
								If Not .IsFeatureAvailable("召喚ユニット") Then
									n = n + .PilotNum
								End If
							End With
						End If
					Next 
					If .CountPilot < n Then
						GoTo NextLoop1
					End If
					
					'分離先の形態が利用可能？
					For j = 2 To LLength(.FeatureData("分離"))
						uname = LIndex(.FeatureData("分離"), j)
						If Not UList.IsDefined(uname) Then
							GoTo NextLoop1
						End If
						If UList.Item(uname).CurrentForm.Status_Renamed = "待機" Then
							GoTo NextLoop1
						End If
					Next 
					
					'分離を実施
					.Split_Renamed()
				End With
NextLoop1: 
			Next u
			
			'合体を行う
			For	Each u In colUnits
				With u
					If .Party0 = "味方" And .Status_Renamed <> "他形態" And .Status_Renamed <> "旧主形態" And .Status_Renamed <> "旧形態" Then
						If .IsFeatureAvailable("合体") Then
							For j = 1 To .CountFeature
								If .Feature(j) <> "合体" Then
									GoTo NextLoop2
								End If
								
								'合体後の形態が利用可能？
								uname = LIndex(.FeatureData(j), 2)
								If Not UList.IsDefined(uname) Then
									GoTo NextLoop2
								End If
								With UList.Item(uname)
									If u.Status_Renamed = "待機" And .CurrentForm.Status_Renamed = "離脱" Then
										GoTo NextLoop2
									End If
									If .IsFeatureAvailable("制限時間") Then
										GoTo NextLoop2
									End If
									If Not .IsFeatureAvailable("主形態") And LLength(u.FeatureData(j)) = 3 Then
										GoTo NextLoop2
									End If
								End With
								
								'合体のパートナーが利用可能？
								For k = 3 To LLength(.FeatureData(j))
									uname = LIndex(.FeatureData(j), k)
									If Not UList.IsDefined(uname) Then
										GoTo NextLoop2
									End If
									With UList.Item(uname)
										If u.Status_Renamed = "待機" Then
											If .CurrentForm.Status_Renamed <> "待機" Then
												GoTo NextLoop2
											End If
										Else
											If .CurrentForm.Status_Renamed <> "離脱" Then
												GoTo NextLoop2
											End If
										End If
									End With
								Next 
								
								'合体を実施
								.Combine(LIndex(.FeatureData(j), 2))
								Exit For
NextLoop2: 
							Next 
						End If
					End If
				End With
			Next u
			
			'標準形態に変形
			For	Each u In colUnits
				With u
					If .Party0 = "味方" And .Status_Renamed <> "他形態" And .Status_Renamed <> "旧主形態" And .Status_Renamed <> "旧形態" Then
						If .IsFeatureAvailable("変形") Then
							uname = .Name
							buf = .FeatureData("変形")
							For j = 2 To LLength(buf)
								uname2 = LIndex(buf, j)
								If UDList.IsDefined(uname2) Then
									If UDList.Item(uname2).ID < UDList.Item(uname).ID Then
										uname = uname2
									End If
								Else
									ErrorMessage(uname & "の変形先ユニット「" & uname2 & "」のデータが定義されていません。")
								End If
							Next 
							
							If uname <> .Name Then
								.Transform(uname)
							End If
						End If
					End If
				End With
			Next u
		Next 
		
		'暴走時パイロットを削除
		For	Each u In colUnits
			With u
				If .IsFeatureAvailable("暴走時パイロット") Then
					If PList.IsDefined(.FeatureData("暴走時パイロット")) Then
						PList.Delete(.FeatureData("暴走時パイロット"))
					End If
				End If
			End With
		Next u
		
		'ダミーパイロットを削除
		For	Each u In colUnits
			With u
				If .CountPilot > 0 Then
					If .Pilot(1).Nickname0 = "パイロット不在" Then
						.DeletePilot(1)
					End If
				End If
			End With
		Next u
		
		'変身先の形態等、一時的な形態を削除
		For	Each u In colUnits
			With u
				If .Status_Renamed = "待機" Then
					.DeleteTemporaryOtherForm()
				End If
			End With
		Next u
		
		'破棄されたユニットを削除
		For	Each u In colUnits
			With u
				'召喚ユニットは必ず破棄
				If .IsFeatureAvailable("召喚ユニット") Then
					.Status_Renamed = "破棄"
				End If
				'ダミーユニットを破棄
				If .IsFeatureAvailable("ダミーユニット") Then
					.Status_Renamed = "破棄"
				End If
				
				'味方ユニット以外のユニットと破棄されたユニットを削除
				If .Party0 <> "味方" Or .Status_Renamed = "破棄" Then
					'ユニットが装備しているアイテムも破棄
					For i = 1 To .CountItem
						.Item(i).Exist = False
					Next 
					Delete(.ID)
				End If
			End With
		Next u
		
		'ユニットの状態を回復
		For	Each u In colUnits
			u.Reset_Renamed()
		Next u
		
		'ステータスをアップデート
		For	Each u In colUnits
			u.Update(True)
		Next u
	End Sub
	
	
	'ユニットリストに登録されたユニットの情報をセーブ
	Public Sub Save()
		Dim i As Short
		Dim u As Unit
		
		WriteLine(SaveDataFileNumber, IDCount)
		WriteLine(SaveDataFileNumber, Count)
		For	Each u In colUnits
			With u
				WriteLine(SaveDataFileNumber, .Name)
				WriteLine(SaveDataFileNumber, .ID, .Rank, .Status_Renamed)
				
				WriteLine(SaveDataFileNumber, .CountOtherForm)
				For i = 1 To .CountOtherForm
					WriteLine(SaveDataFileNumber, .OtherForm(i).ID)
				Next 
				
				WriteLine(SaveDataFileNumber, .CountPilot)
				For i = 1 To .CountPilot
					WriteLine(SaveDataFileNumber, .Pilot(i).ID)
				Next 
				
				WriteLine(SaveDataFileNumber, .CountSupport)
				For i = 1 To .CountSupport
					WriteLine(SaveDataFileNumber, .Support(i).ID)
				Next 
				
				WriteLine(SaveDataFileNumber, .CountItem)
				For i = 1 To .CountItem
					WriteLine(SaveDataFileNumber, .Item(i).ID)
				Next 
			End With
		Next u
	End Sub
	
	'ユニットリストにユニットの情報をロード
	'(リンクは後で行う)
	Public Sub Load()
		Dim num, num2 As Short
		Dim new_unit As Unit
		Dim Name As String
		'UPGRADE_NOTE: Status は Status_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim ID, Status_Renamed As String
		Dim Rank As Short
		Dim i, j As Short
		Dim dummy As String
		
		Input(SaveDataFileNumber, IDCount)
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			new_unit = New Unit
			With new_unit
				'Name
				Input(SaveDataFileNumber, Name)
				
				If Not UDList.IsDefined(Name) Then
					ErrorMessage(Name & "のデータが定義されていません")
					TerminateSRC()
					End
				End If
				
				'ID, Rank, Status
				Input(SaveDataFileNumber, ID)
				Input(SaveDataFileNumber, Rank)
				Input(SaveDataFileNumber, Status_Renamed)
				
				'旧形式のユニットＩＤを新形式に変換
				If SaveDataVersion < 10700 Then
					ConvertUnitID(ID)
				End If
				
				.Name = Name
				.ID = ID
				.Rank = Rank
				.Party = "味方"
				.Status_Renamed = Status_Renamed
				.FullRecover()
			End With
			colUnits.Add(new_unit, new_unit.ID)
			
			'OtherForm
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
			
			'Pilot
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
			
			'Support
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
			
			'Item
			Input(SaveDataFileNumber, num2)
			For j = 1 To num2
				dummy = LineInput(SaveDataFileNumber)
			Next 
		Next 
	End Sub
	
	'ユニットリストにユニットの情報をロードし、リンクを行う
	Public Sub LoadLinkInfo()
		Dim num, num2 As Short
		Dim ID, ID2 As String
		Dim i, j As Short
		Dim int_dummy As Short
		Dim str_dummy As String
		Dim u As Unit
		
		Input(SaveDataFileNumber, IDCount)
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'Name
			str_dummy = LineInput(SaveDataFileNumber)
			'ID, Rank, Status
			Input(SaveDataFileNumber, ID)
			Input(SaveDataFileNumber, int_dummy)
			Input(SaveDataFileNumber, str_dummy)
			
			'旧形式のユニットＩＤを新形式に変換
			If SaveDataVersion < 10700 Then
				ConvertUnitID(ID)
			End If
			
			With Item(ID)
				'OtherForm
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					ConvertUnitID(ID2)
					If IsDefined(ID2) Then
						.AddOtherForm(Item(ID2))
					End If
				Next 
				
				'Pilot
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					If PList.IsDefined(ID2) Then
						.AddPilot(PList.Item(ID2))
						If .Status_Renamed = "離脱" Then
							PList.Item(ID2).Away = True
						End If
					Else
						ID2 = Left(ID2, InStr(ID2, "(") - 1)
						If PList.IsDefined(ID2) Then
							.AddPilot(PList.Item(ID2))
							If .Status_Renamed = "離脱" Then
								PList.Item(ID2).Away = True
							End If
						End If
					End If
				Next 
				
				'Support
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					If PList.IsDefined(ID2) Then
						.AddSupport(PList.Item(ID2))
						If .Status_Renamed = "離脱" Then
							PList.Item(ID2).Away = True
						End If
					End If
				Next 
				
				'Unit
				Input(SaveDataFileNumber, num2)
				For j = 1 To num2
					Input(SaveDataFileNumber, ID2)
					If IList.IsDefined(ID2) Then
						If IList.Item(ID2).Unit Is Nothing Then
							.CurrentForm.AddItem0(IList.Item(ID2))
						End If
					ElseIf IDList.IsDefined(ID2) Then 
						.CurrentForm.AddItem0(IList.Add(ID2))
					End If
				Next 
			End With
		Next 
		
		For	Each u In colUnits
			u.Update(True)
		Next u
	End Sub
	
	
	'一時中断用データをファイルにセーブする
	Public Sub Dump()
		Dim u As Unit
		
		WriteLine(SaveDataFileNumber, Count)
		
		For	Each u In colUnits
			u.Dump()
		Next u
	End Sub
	
	'一時中断用データをファイルからロードする
	Public Sub Restore()
		Dim i, num As Short
		Dim u As Unit
		
		With colUnits
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			u = New Unit
			With u
				.Restore()
				colUnits.Add(u, .ID)
			End With
		Next 
	End Sub
	
	'一時中断用データのリンク情報をファイルからロードする
	Public Sub RestoreLinkInfo()
		Dim u As Unit
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each u In colUnits
			u.RestoreLinkInfo()
		Next u
	End Sub
	
	'一時中断用データのパラメータ情報をファイルからロードする
	Public Sub RestoreParameter()
		Dim u As Unit
		Dim num As Short
		
		Input(SaveDataFileNumber, num)
		
		For	Each u In colUnits
			u.RestoreParameter()
		Next u
	End Sub
	
	
	'ユニットリストをクリア
	Public Sub Clear()
		Dim i As Short
		
		For i = 1 To Count
			Delete(1)
		Next 
	End Sub
	
	'ユニットリストに登録されたユニットのビットマップIDをクリア
	Public Sub ClearUnitBitmap()
		Dim u As Unit
		
		'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picUnitBitmap
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			If .Width = 32 Then
				'既にクリアされていればそのまま終了
				Exit Sub
			End If
			
			'画像をクリア
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Picture = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: Control picUnitBitmap は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Move(0, 0, 32, 96)
		End With
		
		'BitmapIDをクリア
		For	Each u In colUnits
			u.BitmapID = 0
		Next u
	End Sub
	
	
	'ハイパーモードの自動発動チェック
	Public Sub CheckAutoHyperMode()
		Dim u As Unit
		
		For	Each u In colUnits
			u.CheckAutoHyperMode()
		Next u
	End Sub
	
	'ノーマルモードの自動発動チェック
	Public Sub CheckAutoNormalMode()
		Dim u As Unit
		Dim is_redraw_necessary As Boolean
		
		For	Each u In colUnits
			If u.CheckAutoNormalMode(True) Then
				is_redraw_necessary = True
			End If
		Next u
		
		'画面の再描画が必要？
		If is_redraw_necessary Then
			RedrawScreen()
		End If
	End Sub
	
	'破棄されたユニットを削除
	Public Sub Clean()
		Dim u As Unit
		Dim i As Short
		
		For	Each u In colUnits
			With u
				'出撃していないユニットは味方ユニット以外全て削除
				If .Party0 <> "味方" Then
					If .Status_Renamed = "待機" Or .Status_Renamed = "破壊" Then
						.Status_Renamed = "破棄"
						For i = 1 To .CountOtherForm
							.OtherForm(i).Status_Renamed = "破棄"
						Next 
					End If
				End If
			End With
		Next u
		
		For	Each u In colUnits
			With u
				'破棄されたユニットを削除
				If .Status_Renamed = "破棄" Then
					'ユニットに乗っているパイロットも破棄
					For i = 1 To .CountPilot
						.Pilot(i).Alive = False
					Next 
					For i = 1 To .CountSupport
						.Support(i).Alive = False
					Next 
					
					'ユニットが装備しているアイテムも破棄
					For i = 1 To .CountItem
						.Item(i).Exist = False
					Next 
					
					Delete(.ID)
				End If
			End With
		Next u
	End Sub
End Class