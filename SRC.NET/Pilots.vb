Option Strict Off
Option Explicit On
Friend Class Pilots
	Implements System.Collections.IEnumerable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Private colPilots As New Collection
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colPilots
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colPilots ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colPilots = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ForEach用関数
	'UPGRADE_NOTE: NewEnum �v���p�e�B���R�����g �A�E�g����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="B3FC1610-34F3-43F5-86B7-16C984F0E88E"' ���N���b�N���Ă��������B
	'Public Function NewEnum() As stdole.IUnknown
		'NewEnum = colPilots.GetEnumerator
	'End Function
	
	Function GetEnumerator() As System.Collections.IEnumerator Implements System.Collections.IEnumerable.GetEnumerator
		'UPGRADE_TODO: �R���N�V�����񋓎q��Ԃ��ɂ́A�R�����g���������Ĉȉ��̍s��ύX���Ă��������B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="95F9AAD0-1319-4921-95F0-B9D3C4FF7F1C"' ���N���b�N���Ă��������B
		'GetEnumerator = colPilots.GetEnumerator
	End Function
	
	
	'Invalid_string_refer_to_original_code
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
			'UPGRADE_NOTE: �I�u�W�F�N�g new_pilot.Unit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			.Unit_Renamed = Nothing
			
			If gid = "" Then
				'Invalid_string_refer_to_original_code
				If InStr(.Name, "(ザコ)") = 0 And InStr(.Name, "(汎用)") = 0 Then
					key = .Name
					If PList.IsDefined2(key) Then
						If PList.Item2(key).ID = key Then
							'Invalid_string_refer_to_original_code
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
							
							ErrorMessage(key & "Invalid_string_refer_to_original_code")
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
				'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Function Count() As Short
		Count = colPilots.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Delete(ByRef Index As Object)
		colPilots.Remove(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Item(ByRef Index As Object) As Pilot
		Dim p As Pilot
		Dim pname As String
		
		On Error GoTo ErrorHandler
		Item = colPilots.Item(Index)
		If Item.Alive Then
			Exit Function
		End If
		
ErrorHandler: 
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
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
	
	'Invalid_string_refer_to_original_code
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
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: �I�u�W�F�N�g Index �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
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
	
	'Invalid_string_refer_to_original_code
	Public Function Item2(ByRef Index As Object) As Pilot
		On Error GoTo ErrorHandler
		Item2 = colPilots.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: �I�u�W�F�N�g Item2 ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		Item2 = Nothing
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim p As Pilot
		
		On Error GoTo ErrorHandler
		p = colPilots.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Update()
		Dim p As Pilot
		Dim i As Short
		
		For	Each p In colPilots
			With p
				If .Party <> "味方" Or Not .Alive Then
					'Invalid_string_refer_to_original_code
					Delete(.ID)
				ElseIf .IsAdditionalPilot Then 
					'Invalid_string_refer_to_original_code
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'UPGRADE_NOTE: �I�u�W�F�N�g p.Unit.pltAdditionalPilot ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
							.pltAdditionalPilot = Nothing
							For i = 1 To .CountOtherForm
								'UPGRADE_NOTE: �I�u�W�F�N�g p.Unit.OtherForm().pltAdditionalPilot ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
								.OtherForm(i).pltAdditionalPilot = Nothing
							Next 
						End With
					End If
					Delete(.ID)
				ElseIf .IsAdditionalSupport Then 
					'Invalid_string_refer_to_original_code
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'UPGRADE_NOTE: �I�u�W�F�N�g p.Unit.pltAdditionalSupport ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
							.pltAdditionalSupport = Nothing
							For i = 1 To .CountOtherForm
								'UPGRADE_NOTE: �I�u�W�F�N�g p.Unit.OtherForm().pltAdditionalSupport ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
								.OtherForm(i).pltAdditionalSupport = Nothing
							Next 
						End With
					End If
					Delete(.ID)
				ElseIf .Nickname0 = "Invalid_string_refer_to_original_code" Then 
					'Invalid_string_refer_to_original_code
					Delete(.ID)
				ElseIf Not .Unit_Renamed Is Nothing Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					Delete(.ID)
				End If
				'End If
			End With
		Next p
		
		'Invalid_string_refer_to_original_code
		For	Each p In colPilots
			p.FullRecover()
		Next p
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub Save()
		Dim num As Short
		Dim p As Pilot
		
		'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
						ErrorMessage(pname & "Invalid_string_refer_to_original_code")
						TerminateSRC()
						End
					End If
				End If
				With Add(pname, plevel, "味方")
					.Exp = pexp
				End With
			Else
				If Not PDList.IsDefined(LIndex(pname, 1)) Then
					ErrorMessage(LIndex(pname, 1) & "Invalid_string_refer_to_original_code")
					TerminateSRC()
					End
				End If
				With Add(LIndex(pname, 1), plevel, "味方", LIndex(pname, 2))
					.Exp = pexp
				End With
			End If
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
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
					'Invalid_string_refer_to_original_code
					If LLength(pname) = 1 Then
						Item(pname).Away = True
					Else
						Item(LIndex(pname, 2)).Away = True
					End If
					GoTo NextPilot
				Case "-", "Dummy"
					'Invalid_string_refer_to_original_code
					GoTo NextPilot
			End Select
			
			'Invalid_string_refer_to_original_code
			If SaveDataVersion < 10700 Then
				ConvertUnitID(uid)
			End If
			
			If UList.IsDefined(uid) Then
				'Invalid_string_refer_to_original_code
				u = UList.Item(uid)
				If LLength(pname) = 1 Then
					Item(pname).Unit_Renamed = u
				Else
					Item(LIndex(pname, 2)).Unit_Renamed = u
				End If
			Else
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
	Public Sub Dump()
		Dim p As Pilot
		Dim num As Short
		
		'Invalid_string_refer_to_original_code
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
				'Invalid_string_refer_to_original_code
				If Not .IsAdditionalPilot Then
					.Dump()
				End If
			End With
		Next p
	End Sub
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreLinkInfo()
		Dim i, num As Short
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'UPGRADE_WARNING: �I�u�W�F�N�g colPilots().RestoreLinkInfo �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			colPilots.Item(i).RestoreLinkInfo()
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub RestoreParameter()
		Dim i, num As Short
		
		Input(SaveDataFileNumber, num)
		
		For i = 1 To num
			'UPGRADE_WARNING: �I�u�W�F�N�g colPilots().RestoreParameter �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
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
	
	
	'Invalid_string_refer_to_original_code
	Public Sub UpdateSupportMod(Optional ByVal u As Unit = Nothing)
		Dim p As Pilot
		Dim xx, i, yy As Short
		Dim max_range, range As Short
		
		If MapFileName = "" Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If u Is Nothing Then
			For	Each p In colPilots
				p.UpdateSupportMod()
			Next p
			Exit Sub
		End If
		
		With u
			'Invalid_string_refer_to_original_code
			If .CountPilot = 0 Then
				Exit Sub
			End If
			
			With .MainPilot
				'Invalid_string_refer_to_original_code
				.UpdateSupportMod()
				
				'Invalid_string_refer_to_original_code
				max_range = .CommandRange
				
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				max_range = MaxLng(max_range, 2)
				'End If
				
				If IsOptionDefined("信頼補正") And InStr(.Name, "(ザコ)") = 0 Then
					If IsOptionDefined("Invalid_string_refer_to_original_code") Then
						max_range = MaxLng(max_range, 2)
					Else
						max_range = MaxLng(max_range, 1)
					End If
				End If
			End With
			
			'Invalid_string_refer_to_original_code
			For i = 2 To .CountPilot
				.Pilot(i).UpdateSupportMod()
			Next 
			For i = 1 To .CountSupport
				.Support(i).UpdateSupportMod()
			Next 
			
			'Invalid_string_refer_to_original_code
			If max_range = 0 Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			For xx = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
				For yy = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
					If MapDataForUnit(xx, yy) Is Nothing Then
						GoTo NextPoint
					End If
					
					'Invalid_string_refer_to_original_code
					range = System.Math.Abs(.X - xx) + System.Math.Abs(.Y - yy)
					If range > max_range Then
						GoTo NextPoint
					End If
					If range = 0 Then
						GoTo NextPoint
					End If
					
					'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Sub Clean()
		Dim p As Pilot
		
		For	Each p In colPilots
			With p
				If Not .Alive Then
					'Invalid_string_refer_to_original_code
					Delete(.ID)
				End If
			End With
		Next p
	End Sub
End Class