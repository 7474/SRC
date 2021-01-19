Option Strict Off
Option Explicit On
Friend Class AbilityData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'名称
	Public Name As String
	'使用可能回数
	Public Stock As Short
	'Invalid_string_refer_to_original_code
	Public ENConsumption As Short
	'Invalid_string_refer_to_original_code
	Public NecessaryMorale As Short
	'Invalid_string_refer_to_original_code
	Public MinRange As Short
	'Invalid_string_refer_to_original_code
	Public MaxRange As Short
	'属性
	'UPGRADE_NOTE: Class �� Class_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Class_Renamed As String
	'Invalid_string_refer_to_original_code
	Public NecessarySkill As String
	'Invalid_string_refer_to_original_code
	Public NecessaryCondition As String
	
	'Invalid_string_refer_to_original_code
	Private colEffects As New Collection
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colEffects
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: �I�u�W�F�N�g colEffects ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		colEffects = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function Nickname() As String
		Nickname = Name
		ReplaceSubExpression(Nickname)
		If InStr(Nickname, "(") > 0 Then
			Nickname = Left(Nickname, InStr(Nickname, "(") - 1)
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub SetEffect(ByRef elist As String)
		Dim j, i, k As Short
		Dim buf As String
		Dim dat, dat2 As AbilityEffect
		Dim elevel, etype, edata As String
		
		TrimString(elist)
		For i = 1 To ListLength(elist)
			dat = NewAbilityEffect
			With dat
				buf = ListIndex(elist, i)
				j = InStr(buf, "Lv")
				k = InStr(buf, "=")
				If j > 0 And (k = 0 Or j < k) Then
					'Invalid_string_refer_to_original_code
					.Name = Left(buf, j - 1)
					If k > 0 Then
						'Invalid_string_refer_to_original_code
						.Level = CDbl(Mid(buf, j + 2, k - (j + 2)))
						buf = Mid(buf, k + 1)
						If Left(buf, 1) = """" Then
							buf = Mid(buf, 2, Len(buf) - 2)
						End If
						
						j = InStr(buf, "Lv")
						k = InStr(buf, "=")
						
						If j > 0 And (k = 0 Or j < k) Then
							'Invalid_string_refer_to_original_code
							etype = Left(buf, j - 1)
							If k > 0 Then
								elevel = Mid(buf, j + 2, k - (j + 2))
								edata = Mid(buf, k + 1)
							Else
								elevel = Mid(buf, j + 2)
								edata = ""
							End If
						ElseIf k > 0 Then 
							'Invalid_string_refer_to_original_code
							etype = Left(buf, k - 1)
							elevel = ""
							edata = Mid(buf, k + 1)
						Else
							'Invalid_string_refer_to_original_code
							etype = buf
							elevel = ""
							edata = ""
						End If
						
						If .Name = "付加" And elevel = "" Then
							elevel = VB6.Format(DEFAULT_LEVEL)
						End If
						
						.Data = Trim(etype & " " & elevel & " " & edata)
					Else
						'Invalid_string_refer_to_original_code
						.Level = CDbl(Mid(buf, j + 2))
					End If
				ElseIf k > 0 Then 
					'Invalid_string_refer_to_original_code
					.Name = Left(buf, k - 1)
					buf = Mid(buf, k + 1)
					If Asc(buf) = 34 Then '"
						buf = Mid(buf, 2, Len(buf) - 2)
					End If
					
					j = InStr(buf, "Lv")
					k = InStr(buf, "=")
					
					If .Name = "解説" Then
						'Invalid_string_refer_to_original_code
						etype = buf
						elevel = ""
						edata = ""
					ElseIf j > 0 Then 
						'Invalid_string_refer_to_original_code
						etype = Left(buf, j - 1)
						If k > 0 Then
							elevel = Mid(buf, j + 2, k - (j + 2))
							edata = Mid(buf, k + 1)
						Else
							elevel = Mid(buf, j + 2)
							edata = ""
						End If
					ElseIf k > 0 Then 
						'Invalid_string_refer_to_original_code
						etype = Left(buf, k - 1)
						elevel = ""
						edata = Mid(buf, k + 1)
					Else
						'Invalid_string_refer_to_original_code
						etype = buf
						elevel = ""
						edata = ""
					End If
					
					If .Name = "付加" And elevel = "" Then
						elevel = VB6.Format(DEFAULT_LEVEL)
					End If
					
					.Data = Trim(etype & " " & elevel & " " & edata)
				Else
					'効果名のみ
					.Name = buf
				End If
				
				j = 1
				For	Each dat2 In colEffects
					If .Name = dat2.Name Then
						j = j + 1
					End If
				Next dat2
				If j = 1 Then
					colEffects.Add(dat, .Name)
				Else
					colEffects.Add(dat, .Name & VB6.Format(j))
				End If
			End With
		Next 
	End Sub
	
	Private Function NewAbilityEffect() As AbilityEffect
		Dim dat As New AbilityEffect
		NewAbilityEffect = dat
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function CountEffect() As Short
		CountEffect = colEffects.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function EffectType(ByRef Index As Object) As String
		'UPGRADE_WARNING: �I�u�W�F�N�g colEffects.Item().Name �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		EffectType = colEffects.Item(Index).Name
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function EffectLevel(ByRef Index As Object) As Double
		'UPGRADE_WARNING: �I�u�W�F�N�g colEffects.Item().Level �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		EffectLevel = colEffects.Item(Index).Level
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function EffectData(ByRef Index As Object) As String
		'UPGRADE_WARNING: �I�u�W�F�N�g colEffects.Item().Data �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
		EffectData = colEffects.Item(Index).Data
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function EffectName(ByRef Index As Object) As String
		Dim ae As AbilityEffect
		Dim elevel, elevel2 As Double
		Dim uname, wclass As String
		Dim flevel As Double
		Dim heal_lv, supply_lv As Double
		Dim i As Short
		Dim buf As String
		Dim cname, aname As String
		
		ae = colEffects.Item(Index)
		With ae
			'Invalid_string_refer_to_original_code
			elevel = .Level
			'Invalid_string_refer_to_original_code
			elevel2 = elevel
			
			If SelectedUnit.CountPilot > 0 Then
				With SelectedUnit.MainPilot
					'得意技
					If .IsSkillAvailable("得意技") Then
						buf = .SkillData("得意技")
						For i = 1 To Len(buf)
							If InStrNotNest(Class_Renamed, GetClassBundle(buf, i)) > 0 Then
								elevel = 1.2 * elevel
								elevel2 = 1.4 * elevel2
								Exit For
							End If
						Next 
					End If
					
					'不得手
					If .IsSkillAvailable("不得手") Then
						buf = .SkillData("不得手")
						For i = 1 To Len(buf)
							If InStrNotNest(Class_Renamed, GetClassBundle(buf, i)) > 0 Then
								elevel = 0.8 * elevel
								elevel2 = 0.6 * elevel2
								Exit For
							End If
						Next 
					End If
					
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					elevel = elevel * .Shooting / 100
				End With
			Else
				For i = 1 To LLength(NecessarySkill)
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'UPGRADE_WARNING: �I�u�W�F�N�g ae.Shooting �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					elevel = elevel * .Shooting / 100
					Exit For
				Next 
			End If
			'Next
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'UPGRADE_WARNING: �I�u�W�F�N�g ae.SkillLevel �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			supply_lv = .SkillLevel("補給")
		End With
		'End If
		
		'Invalid_string_refer_to_original_code
		If elevel2 <> 0 Then
			elevel2 = MaxLng(elevel2, 1)
		End If
		
		'UPGRADE_WARNING: EffectName �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		'End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Function WeaponType(ByRef wclass As String) As String
		If wclass = "全" Or wclass = "" Then
			WeaponType = "武器"
		ElseIf Len(wclass) = 1 Then 
			WeaponType = AttributeName(Nothing, wclass)
		ElseIf Right(wclass, 2) = "Invalid_string_refer_to_original_code" Then 
			WeaponType = Left(wclass, Len(wclass) - 2)
		Else
			WeaponType = wclass & "Invalid_string_refer_to_original_code"
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsItem() As Boolean
		Dim i As Short
		
		For i = 1 To LLength(NecessarySkill)
			If LIndex(NecessarySkill, i) = "Invalid_string_refer_to_original_code" Then
				IsItem = True
				Exit Function
			End If
		Next 
	End Function
End Class