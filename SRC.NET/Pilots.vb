Option Strict Off
Option Explicit On
Friend Class Pilots
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'作成されたパイロットのデータを管理するリストクラス
	
	'パイロット一覧
	Private colPilots As New Collection
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colPilots
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colPilots をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colPilots = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ForEach用関数
	'UPGRADE_NOTE: NewEnum プロパティがコメント アウトされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' をクリックしてください。
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colPilots.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: コレクション列挙子を返すには、コメントを解除して以下の行を変更してください。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' をクリックしてください。
		'GetEnumerator = colPilots.GetEnumerator
	End Function
	
	
	'パイロットを追加
	Public Function Add(ByRef pname As String, ByVal plevel As Short, ByRef pparty As String, Optional ByRef gid As String = "") As Pilot
		Dim new_pilot As New Pilot
		Dim key As String
		Dim i As Short
		
		With new_pilot
			.Name = PDList.Item(pname).Name
			.Level = plevel
			.Party = pparty
			.FullRecover()
			.Alive = True
			'UPGRADE_NOTE: オブジェクト new_pilot.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			.Unit_Renamed = Nothing
			
			If gid = "" Then
				'グループＩＤが指定されていない場合
				If InStr(.Name, "(ザコ)") = 0 And InStr(.Name, "(汎用)") = 0 Then
					key = .Name
					If PList.IsDefined2(key) Then
						If PList.Item2(key).ID = key Then
							'一度作成されたパイロットを復活させる場合
							If Not PList.Item2(key).Alive Then
								With PList.Item2(key)
									.Level = plevel
									.Party = pparty
									.FullRecover()
									.Alive = True
								End With
								Add = PList.Item2(key)
								Exit Function
							End If
							
							ErrorMessage(key & "というパイロットは既に登録されています")
							Exit Function
						End If
					End If
				Else
					i = colPilots.Count()
					Do 
						i = i + 1
						key = .Name & "_" & VB6.Format(i)
					Loop While PList.IsDefined2(key)
				End If
			Else
				'グループＩＤが指定されている場合
				key = gid
				i = 1
				Do While PList.IsDefined2(key)
					i = i + 1
					key = gid & ":" & VB6.Format(i)
				Loop 
			End If
			
			.ID = key
			colPilots.Add(new_pilot, key)
			Add = new_pilot
		End With
	End Function
	
	'登録されているパイロットの総数
	Public Function Count() As Short
		Count = colPilots.Count()
	End Function
	
	'リストからパイロットを削除
	Public Sub Delete(ByRef Index As Object)
		colPilots.Remove(Index)
	End Sub
	
	'リストからパイロットを検索
	Public Function Item(ByRef Index As Object) As Pilot
		Dim p As Pilot
		Dim pname As String
		
		On Error GoTo ErrorHandler
		Item = colPilots.Item(Index)
		If Item.Alive Then
			Exit Function
		End If
		
ErrorHandler: 
		'ＩＤで見つからなければ名称で検索
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		pname = CStr(Index)
		For	Each p In colPilots
			If p.Data.Name = pname Then
				If p.Alive Then
					Item = p
					Exit Function
				End If
			End If
		Next p
		'それでも見つからなければ愛称で検索
		For	Each p In colPilots
			If p.Data.Nickname = pname Then
				If p.Alive Then
					Item = p
					Exit Function
				End If
			End If
		Next p
	End Function
	
	'パイロットが定義されているか
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim p As Pilot
		Dim pname As String
		
		On Error GoTo ErrorHandler
		p = colPilots.Item(Index)
		If p.Alive Then
			IsDefined = True
			Exit Function
		End If
		
ErrorHandler: 
		'ＩＤで見つからなければ名称で検索
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		pname = CStr(Index)
		For	Each p In colPilots
			If p.Data.Name = pname Then
				If p.Alive Then
					IsDefined = True
					Exit Function
				End If
			End If
		Next p
		'それでも見つからなければ愛称で検索
		For	Each p In colPilots
			If p.Data.Nickname = pname Then
				If p.Alive Then
					IsDefined = True
					Exit Function
				End If
			End If
		Next p
	End Function
	
	'パイロットを検索 (IDのみで検索)
	Public Function Item2(ByRef Index As Object) As Pilot
		On Error GoTo ErrorHandler
		Item2 = colPilots.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: オブジェクト Item2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item2 = Nothing
	End Function
	
	'パイロットが定義されているか (IDのみで検索)
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim p As Pilot
		
		On Error GoTo ErrorHandler
		p = colPilots.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'リストのアップデート
	Public Sub Update()
		Dim p As Pilot
		Dim i As Short
		
		For	Each p In colPilots
			With p
				If .Party <> "味方" Or Not .Alive Then
					'味方でないパイロットや破棄されたパイロットは削除
					Delete(.ID)
				ElseIf .IsAdditionalPilot Then 
					'追加パイロットは削除
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'UPGRADE_NOTE: オブジェクト p.Unit.pltAdditionalPilot をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							.pltAdditionalPilot = Nothing
							For i = 1 To .CountOtherForm
								'UPGRADE_NOTE: オブジェクト p.Unit.OtherForm().pltAdditionalPilot をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
								.OtherForm(i).pltAdditionalPilot = Nothing
							Next 
						End With
					End If
					Delete(.ID)
				ElseIf .IsAdditionalSupport Then 
					'追加サポートは削除
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'UPGRADE_NOTE: オブジェクト p.Unit.pltAdditionalSupport をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							.pltAdditionalSupport = Nothing
							For i = 1 To .CountOtherForm
								'UPGRADE_NOTE: オブジェクト p.Unit.OtherForm().pltAdditionalSupport をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
								.OtherForm(i).pltAdditionalSupport = Nothing
							Next 
						End With
					End If
					Delete(.ID)
				ElseIf .Nickname0 = "パイロット不在" Then 
					'ダミーパイロットは削除
					Delete(.ID)
				ElseIf Not .Unit_Renamed Is Nothing Then 
					If .Unit_Renamed.IsFeatureAvailable("召喚ユニット") Then
						'召喚ユニットの追加パイロットも削除
						Delete(.ID)
					End If
				End If
			End With
		Next p
		
		'残ったパイロットを全回復
		For	Each p In colPilots
			p.FullRecover()
		Next p
	End Sub
	
	
	'ファイルにデータをセーブ
	Public Sub Save()
		Dim num As Short
		Dim p As Pilot
		
		'追加パイロットや追加サポートを除いたパイロット数を算出
		For	Each p In colPilots
			With p
				If Not .IsAdditionalPilot And Not .IsAdditionalSupport Then
					num = num + 1
				End If
			End With
		Next p
		WriteLine(SaveDataFileNumber, num)
		
		For	Each p In colPilots
			With p
				'追加パイロットや追加サポートはセーブしない
				If Not .IsAdditionalPilot And Not .IsAdditionalSupport Then
					If .Name = .ID Then
						WriteLine(SaveDataFileNumber, .Name)
					Else
						WriteLine(SaveDataFileNumber, .Name & " " & .ID)
					End If
					WriteLine(SaveDataFileNumber, .Level, .Exp)
					If .Unit_Renamed Is Nothing Then
						If .Away Then
							WriteLine(SaveDataFileNumber, "離脱")
						Else
							WriteLine(SaveDataFileNumber, "-")
						End If
					Else
						WriteLine(SaveDataFileNumber, .Unit_Renamed.ID)
					End If
				End If
			End With
		Next p
	End Sub
	
	'ファイルからデータをロード (リンクは行わない)
	Public Sub Load()
		Dim i, num As Short
		Dim pname As String
		Dim plevel, pexp As Short
		Dim dummy As String
		
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			'Name
			Input(SaveDataFileNumber, pname)
			'Level, Exp
			Input(SaveDataFileNumber, plevel)
			Input(SaveDataFileNumber, pexp)
			'Unit
			Input(SaveDataFileNumber, dummy)
			
			If LLength(pname) = 1 Then
				If Not PDList.IsDefined(pname) Then
					If InStr(pname, "(") > 0 Then
						pname = Left(pname, InStr(pname, "(") - 1)
					End If
					If Not PDList.IsDefined(pname) Then
						ErrorMessage(pname & "のデータが定義されていません")
						TerminateSRC()
						End
					End If
				End If
				With Add(pname, plevel, "味方")
					.Exp = pexp
				End With
			Else
				If Not PDList.IsDefined(LIndex(pname, 1)) Then
					ErrorMessage(LIndex(pname, 1) & "のデータが定義されていません")
					TerminateSRC()
					End
				End If
				With Add(LIndex(pname, 1), plevel, "味方", LIndex(pname, 2))
					.Exp = pexp
				End With
			End If
		Next 
	End Sub
	
	'ファイルからデータを読み込みリンク
	Public Sub LoadLinkInfo()
		Dim ret, i, num As Short
		Dim pname, uid As String
		Dim dummy As String
		Dim u As Unit
		
		Input(SaveDataFileNumber, num)
		For i = 1 To num
			'Name
			Input(SaveDataFileNumber, pname)
			'Level, Exp
			dummy = LineInput(SaveDataFileNumber)
			'Unit
			Input(SaveDataFileNumber, uid)
			
			If LLength(pname) = 1 Then
				If Not IsDefined(pname) Then
					pname = Left(pname, InStr(pname, "(") - 1)
				End If
			End If
			
			Select Case uid
				Case "離脱"
					'Leaveされたパイロット
					If LLength(pname) = 1 Then
						Item(pname).Away = True
					Else
						Item(LIndex(pname, 2)).Away = True
					End If
					GoTo NextPilot
				Case "-", "Dummy"
					'ユニットに乗っていないパイロット
					GoTo NextPilot
			End Select
			
			'旧形式のユニットＩＤを新形式に変換
			If SaveDataVersion < 10700 Then
				ConvertUnitID(uid)
			End If
			
			If UList.IsDefined(uid) Then
				'パイロットをユニットに乗せる
				u = UList.Item(uid)
				If LLength(pname) = 1 Then
					Item(pname).Unit_Renamed = u
				Else
					Item(LIndex(pname, 2)).Unit_Renamed = u
				End If
			Else
				'乗せるべきユニットが見つからなかった場合は強制的にユニットを
				'作って乗せる (バグ対策だったけど……不要？)
				ret = InStr(uid, ":")
				uid = Left(uid, ret - 1)
				If UDList.IsDefined(uid) Then
					u = UList.Add(uid, 0, "味方")
					If LLength(pname) = 1 Then
						Item(pname).Ride(u)
					Else
						Item(LIndex(pname, 2)).Ride(u)
					End If
				End If
			End If
NextPilot: 
		Next 
	End Sub
	
	
	'一時中断用データをセーブする
	Public Sub Dump()
		Dim p As Pilot
		Dim num As Short
		
		'追加パイロットを除いたパイロット数を算出
		For	Each p In colPilots
			With p
				If Not .IsAdditionalPilot Then
					num = num + 1
				End If
			End With
		Next p
		WriteLine(SaveDataFileNumber, num)
		
		For	Each p In colPilots
			With p
				'追加パイロットはセーブしない
				If Not .IsAdditionalPilot Then
					.Dump()
				End If
			End With
		Next p
	End Sub
	
	'一時中断用データをファイルからロードする
	Public Sub Restore()
		Dim i, num As Short
		Dim buf As String
		Dim p As Pilot
		
		With colPilots
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			p = New Pilot
			With p
				.Restore()
				colPilots.Add(p, .ID)
			End With
		Next 
	End Sub
	
	'一時中断用データのリンク情報をファイルからロードする
	Public Sub RestoreLinkInfo()
		Dim i, num As Short
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'UPGRADE_WARNING: オブジェクト colPilots().RestoreLinkInfo の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			colPilots.Item(i).RestoreLinkInfo()
		Next 
	End Sub
	
	'一時中断用データのパラメータ情報をファイルからロードする
	Public Sub RestoreParameter()
		Dim i, num As Short
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'UPGRADE_WARNING: オブジェクト colPilots().RestoreParameter の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			colPilots.Item(i).RestoreParameter()
		Next 
	End Sub
	
	
	'リストをクリア
	Public Sub Clear()
		Dim i As Short
		
		For i = 1 To Count
			Delete(1)
		Next 
	End Sub
	
	
	'パイロットの支援修正を更新
	Public Sub UpdateSupportMod(Optional ByVal u As Unit = Nothing)
		Dim p As Pilot
		Dim xx, i, yy As Short
		Dim max_range, range As Short
		
		If MapFileName = "" Then
			Exit Sub
		End If
		
		'ユニット指定がなければ全パイロットを更新
		If u Is Nothing Then
			For	Each p In colPilots
				p.UpdateSupportMod()
			Next p
			Exit Sub
		End If
		
		With u
			'ユニットにパイロットが乗っていなければそのまま終了
			If .CountPilot = 0 Then
				Exit Sub
			End If
			
			With .MainPilot
				'メインパイロットを更新
				.UpdateSupportMod()
				
				'支援範囲を算出
				max_range = .CommandRange
				
				If .IsSkillAvailable("広域サポート") Then
					max_range = MaxLng(max_range, 2)
				End If
				
				If IsOptionDefined("信頼補正") And InStr(.Name, "(ザコ)") = 0 Then
					If IsOptionDefined("信頼補正範囲拡大") Then
						max_range = MaxLng(max_range, 2)
					Else
						max_range = MaxLng(max_range, 1)
					End If
				End If
			End With
			
			'他のパイロットを更新
			For i = 2 To .CountPilot
				.Pilot(i).UpdateSupportMod()
			Next 
			For i = 1 To .CountSupport
				.Support(i).UpdateSupportMod()
			Next 
			
			'支援範囲が無いなら他のユニットに乗っているパイロットには影響無し
			If max_range = 0 Then
				Exit Sub
			End If
			
			'周りのユニットに乗っているパイロットの支援修正を更新
			For xx = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For yy = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					If MapDataForUnit(xx, yy) Is Nothing Then
						GoTo NextPoint
					End If
					
					'支援範囲内にいるかチェック
					range = System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy)
					If range > max_range Then
						GoTo NextPoint
					End If
					If range = 0 Then
						GoTo NextPoint
					End If
					
					'乗っているパイロット全員の支援修正を更新
					With MapDataForUnit(xx, yy)
						If .CountPilot = 0 Then
							GoTo NextPoint
						End If
						
						.MainPilot.UpdateSupportMod()
						For i = 2 To .CountPilot
							.Pilot(i).UpdateSupportMod()
						Next 
						For i = 1 To .CountSupport
							.Support(i).UpdateSupportMod()
						Next 
					End With
NextPoint: 
				Next 
			Next 
		End With
	End Sub
	
	'破棄されたパイロットを削除する
	Public Sub Clean()
		Dim p As Pilot
		
		For	Each p In colPilots
			With p
				If Not .Alive Then
					'破棄されたパイロットを削除
					Delete(.ID)
				End If
			End With
		Next p
	End Sub
End Class