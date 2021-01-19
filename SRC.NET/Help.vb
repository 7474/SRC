Option Strict Off
Option Explicit On
Module Help
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	
	'Invalid_string_refer_to_original_code
	Public Sub SkillHelp(ByRef p As Pilot, ByRef sindex As String)
		Dim stype, sname As String
		Dim msg As String
		Dim prev_mode As Boolean
		
		'Invalid_string_refer_to_original_code
		If IsNumeric(sindex) Then
			sname = p.SkillName(CShort(sindex))
		Else
			'Invalid_string_refer_to_original_code
			If InStr(sindex, "Lv") > 0 Then
				stype = Left(sindex, InStr(sindex, "Lv") - 1)
			Else
				stype = sindex
			End If
			sname = p.SkillName(stype)
		End If
		
		msg = SkillHelpMessage(p, sindex)
		
		'解説の表示
		If Len(msg) > 0 Then
			prev_mode = AutoMessageMode
			AutoMessageMode = False
			
			OpenMessageForm()
			If AutoMoveCursor Then
				MoveCursorPos("Invalid_string_refer_to_original_code")
			End If
			DisplayMessage("Invalid_string_refer_to_original_code", "<b>" & sname & "</b>;" & msg)
			CloseMessageForm()
			
			AutoMessageMode = prev_mode
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function SkillHelpMessage(ByRef p As Pilot, ByRef sindex As String) As String
		Dim sname, stype, sname0 As String
		Dim slevel As Double
		Dim sdata As String
		Dim is_level_specified As Boolean
		Dim msg As String
		Dim u, u2 As Unit
		Dim uname, fdata As String
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		With p
			If IsNumeric(sindex) Then
				stype = .Skill(CShort(sindex))
				slevel = .SkillLevel(CShort(sindex))
				sdata = .SkillData(CShort(sindex))
				sname = .SkillName(CShort(sindex))
				sname0 = .SkillName0(CShort(sindex))
				is_level_specified = .IsSkillLevelSpecified(CShort(sindex))
			Else
				'Invalid_string_refer_to_original_code
				If InStr(sindex, "Lv") > 0 Then
					stype = Left(sindex, InStr(sindex, "Lv") - 1)
				Else
					stype = sindex
				End If
				stype = .SkillType(stype)
				slevel = .SkillLevel(stype)
				sdata = .SkillData(stype)
				sname = .SkillName(stype)
				sname0 = .SkillName0(stype)
				is_level_specified = .IsSkillLevelSpecified(stype)
			End If
			
			'Invalid_string_refer_to_original_code
			u = .Unit_Renamed
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			If IsLocalVariableDefined("Invalid_string_refer_to_original_code" & .ID & "]") Then
				'UPGRADE_WARNING: �I�u�W�F�N�g LocalVariableList.Item().StringValue �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				uname = LocalVariableList.Item("Invalid_string_refer_to_original_code" & .ID & "]").StringValue
				If uname <> "" Then
					u2 = u
					u = UList.Item(uname)
				End If
			End If
			'End If
		End With
		
		Select Case stype
			Case "オーラ"
				If u.FeatureName0("バリア") = "オーラバリア" Then
					msg = "Invalid_string_refer_to_original_code" & u.FeatureName0("オーラバリア") & "の強度に" & VB6.Format(CInt(100 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				If u.IsFeatureAvailable("オーラ変換器") Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("命中", u) & "・" & Term("回避", u)
				If slevel > 0 Then
					msg = msg & "に +" & VB6.Format(CInt(2 * slevel + 3)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				If slevel > 3 Then
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel \ 4)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("命中", u) & "・" & Term("回避", u)
				If slevel > 0 Then
					msg = msg & "に +" & VB6.Format(CInt(2 * slevel + 3)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				If slevel > 3 Then
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel \ 4)) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "精神不安定により" & Term("Invalid_string_refer_to_original_code", u) & "消費量が20%増加する"
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("命中", u) & "・" & Term("回避", u)
				If slevel > 0 Then
					msg = msg & "に +" & VB6.Format(CInt(2 * slevel + 3)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				
			Case "迎撃"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("命中", u) & "・" & Term("回避", u)
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If u.IsFeatureAvailable("盾") Then
					msg = "Invalid_string_refer_to_original_code"
					VB6.Format((CInt(100 * slevel + 400)) & "Invalid_string_refer_to_original_code")
				Else
					msg = VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "敵を倒した時に得られる" & Term("Invalid_string_refer_to_original_code")
				If Not is_level_specified Then
					msg = msg & "Invalid_string_refer_to_original_code"
				ElseIf slevel >= 0 Then 
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & p.SkillName0("再生") & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If u.IsHero Then
					msg = "同調により"
				Else
					msg = "Invalid_string_refer_to_original_code"
				End If
				msg = msg & Term("運動性", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If slevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				
			Case "霊力"
				msg = "現在の" & sname0 & "値にあわせて" & Term("Invalid_string_refer_to_original_code", u) & "・" & Term("Invalid_string_refer_to_original_code", u) & "・" & Term("Invalid_string_refer_to_original_code", u) & "・" & Term("移動力", u) & "Invalid_string_refer_to_original_code"
				
			Case "霊力成長"
				If slevel >= 0 Then
					msg = p.SkillName0("霊力") & "Invalid_string_refer_to_original_code" & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = p.SkillName0("霊力") & "Invalid_string_refer_to_original_code" & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "底力"
				msg = Term("Invalid_string_refer_to_original_code", u) & "が最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("Invalid_string_refer_to_original_code", u) & "が最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				
			Case "覚悟"
				msg = Term("Invalid_string_refer_to_original_code", u) & "が最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
				'End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("Invalid_string_refer_to_original_code", u) & "が最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				
			Case "素質"
				If Not is_level_specified Then
					msg = "Invalid_string_refer_to_original_code"
				ElseIf slevel >= 0 Then 
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "再生", "Invalid_string_refer_to_original_code"
				msg = Term("Invalid_string_refer_to_original_code", u) & "が０になった時に" & VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "悟り"
				msg = Term("命中", u) & "・" & Term("回避", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If slevel >= 0 Then
					msg = msg & "にそれぞれ +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "にそれぞれ " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				Select Case slevel
					Case 1
						i = 0
					Case 2
						i = 10
					Case 3
						i = 20
					Case 4
						i = 30
					Case 5
						i = 40
					Case 6
						i = 50
					Case 7
						i = 55
					Case 8
						i = 60
					Case 9
						i = 65
					Case Is >= 10
						i = 70
					Case Else
						i = 0
				End Select
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "技"
				Select Case slevel
					Case 1
						i = 0
					Case 2
						i = 10
					Case 3
						i = 20
					Case 4
						i = 30
					Case 5
						i = 40
					Case 6
						i = 50
					Case 7
						i = 55
					Case 8
						i = 60
					Case 9
						i = 65
					Case Is >= 10
						i = 70
					Case Else
						i = 0
				End Select
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("スペシャルパワー", u) & "の" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "闘争本能"
				If p.MinMorale > 100 Then
					If Not p.IsSkillLevelSpecified("闘争本能") Then
						msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
						VB6.Format((p.MinMorale + 5 * slevel) & "Invalid_string_refer_to_original_code")
					ElseIf slevel >= 0 Then 
						msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
						VB6.Format((p.MinMorale + 5 * slevel) & "Invalid_string_refer_to_original_code")
					Else
						msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
						VB6.Format((p.MinMorale + 5 * slevel) & "Invalid_string_refer_to_original_code")
					End If
				Else
					If Not p.IsSkillLevelSpecified("闘争本能") Then
						msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
					ElseIf slevel >= 0 Then 
						msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
						VB6.Format((100 + 5 * slevel) & "Invalid_string_refer_to_original_code")
					Else
						msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
						VB6.Format((100 + 5 * slevel) & "Invalid_string_refer_to_original_code")
					End If
				End If
				
			Case "潜在力開放"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = Term("気力", u) & "Invalid_string_refer_to_original_code"
				msg = Term("気力", u) & "Invalid_string_refer_to_original_code"
				'End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Term("命中", u) & "・" & Term("回避", u)
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If slevel >= 0 Then
					msg = msg & "に +" & VB6.Format(CInt(5 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "に " & VB6.Format(CInt(5 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If InStr(sname, "階級Lv") = 0 Then
					msg = "階級レベル" & StrConv(VB6.Format(CInt(slevel)), VbStrConv.Wide) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If slevel >= 0 Then
					msg = msg & "に +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "に " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u)
				If slevel >= 0 Then
					msg = msg & "に +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "に " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("魔力", u)
				If slevel >= 0 Then
					msg = msg & "に +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "に " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("命中", u)
				If slevel >= 0 Then
					msg = msg & "に +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "に " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("回避", u)
				If slevel >= 0 Then
					msg = msg & "に +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "に " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If slevel >= 0 Then
					msg = msg & "に +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "に " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If slevel >= 0 Then
					msg = msg & "に +" & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "に " & VB6.Format(CInt(2 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("命中", u) & "・" & Term("回避", u)
				If slevel >= 0 Then
					msg = msg & "に +" & VB6.Format(CInt(3 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "に " & VB6.Format(CInt(3 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("命中", u) & "・" & Term("回避", u)
				If slevel >= 0 Then
					msg = msg & "に +" & VB6.Format(CInt(5 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "に " & VB6.Format(CInt(5 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "援護"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				
			Case "援護防御"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				msg = sdata & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "カウンター"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				If LLength(sdata) = 2 Then
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "先読み"
				msg = VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 * slevel \ 16)) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If slevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					VB6.Format((CInt(5 * slevel)) & "Invalid_string_refer_to_original_code")
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					VB6.Format((CInt(5 * System.Math.Abs(slevel))) & "Invalid_string_refer_to_original_code")
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "毎ターン" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & VB6.Format(p.Level \ 8 + 5) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If IsOptionDefined("Invalid_string_refer_to_original_code") Then
					msg = msg & VB6.Format(slevel + 0.5) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(slevel + 1) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				If p.HasMana() Then
					msg = "Invalid_string_refer_to_original_code" & Term("魔力", u) & "の増加量が"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "の増加量が"
				End If
				If IsOptionDefined("Invalid_string_refer_to_original_code") Then
					msg = msg & VB6.Format(slevel + 0.5) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(slevel + 1) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "命中成長"
				msg = "Invalid_string_refer_to_original_code" & Term("命中", u) & "の増加量が" & VB6.Format(slevel + 2) & "Invalid_string_refer_to_original_code"
				
			Case "回避成長"
				msg = "Invalid_string_refer_to_original_code" & Term("回避", u) & "の増加量が" & VB6.Format(slevel + 2) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "防御成長"
				'Invalid_string_refer_to_original_code
				msg = "Invalid_string_refer_to_original_code" & Term("防御", u) & "の増加量が"
				If IsOptionDefined("Invalid_string_refer_to_original_code") Then
					msg = msg & VB6.Format(slevel + 0.5) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(slevel + 1) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "精神統一"
				msg = Term("Invalid_string_refer_to_original_code", u) & "が最大" & Term("Invalid_string_refer_to_original_code", u) & "の20%未満(" & VB6.Format(p.MaxSP \ 5) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "損傷時気力増加"
				If slevel >= -1 Then
					msg = "ダメージを受けた際に" & Term("気力", u) & "+" & VB6.Format(CInt(slevel + 1)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "ダメージを受けた際に" & Term("気力", u) & VB6.Format(CInt(slevel + 1)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "命中時気力増加"
				If slevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "+" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "失敗時気力増加"
				If slevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "+" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "回避時気力増加"
				If slevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "+" & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & VB6.Format(CInt(slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "起死回生"
				msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If slevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(10 * slevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(10 * slevel)) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "得意技"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "不得手"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "ハンター"
				msg = "Invalid_string_refer_to_original_code"
				For i = 2 To LLength(sdata)
					If i = 3 Then
						msg = msg & "Invalid_string_refer_to_original_code"
					ElseIf 3 > 2 Then 
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & LIndex(sdata, i)
				Next 
				msg = msg & "Invalid_string_refer_to_original_code"
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("スペシャルパワー", u)
				For i = 2 To LLength(sdata)
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Next 
				msg = msg & "の" & Term("Invalid_string_refer_to_original_code", u) & "消費量が"
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("気力", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "を使った際の" & Term("Invalid_string_refer_to_original_code", u) & "回復量が "
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "補給"
				If IsOptionDefined("移動後補給不可") Then
					msg = "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "補給" & Term("Invalid_string_refer_to_original_code", u) & "を使った際の" & Term("Invalid_string_refer_to_original_code", u) & "回復量が "
				If slevel >= 0 Then
					msg = msg & VB6.Format(10 * slevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(-10 * slevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "気力上限"
				i = 150
				If slevel <> 0 Then
					i = MaxLng(slevel, 0)
				End If
				msg = Term("気力", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "気力下限"
				i = 50
				If slevel <> 0 Then
					i = MaxLng(slevel, 0)
				End If
				msg = Term("気力", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
				' ADD START MARGE
			Case "遊撃"
				msg = "移動後使用可能な武器・" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				' ADD END MARGE
				
			Case Else
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
				With p
					sdata = .SkillData(sname0)
					If ListIndex(sdata, 1) = "解説" Then
						msg = ListIndex(sdata, ListLength(sdata))
						If Left(msg, 1) = """" Then
							msg = Mid(msg, 2, Len(msg) - 2)
						End If
						SkillHelpMessage = msg
						Exit Function
					End If
				End With
				
				'Invalid_string_refer_to_original_code
				With u
					For i = 1 To .CountFeature
						If .Feature(i) = stype Then
							fdata = .FeatureData(i)
							If ListIndex(fdata, 1) = "解説" Then
								msg = ListIndex(fdata, ListLength(fdata))
							End If
						End If
					Next 
				End With
				If Not u2 Is Nothing Then
					With u2
						For i = 1 To .CountFeature
							If .Feature(i) = stype Then
								fdata = .FeatureData(i)
								If ListIndex(fdata, 1) = "解説" Then
									msg = ListIndex(fdata, ListLength(fdata))
								End If
							End If
						Next 
					End With
				End If
				
				If msg = "" Then
					Exit Function
				End If
				
				'Invalid_string_refer_to_original_code
				If Left(msg, 1) = """" Then
					msg = Mid(msg, 2, Len(msg) - 2)
				End If
		End Select
		
		'Invalid_string_refer_to_original_code
		With p
			sdata = .SkillData(sname0)
			If ListIndex(sdata, 1) = "解説" Then
				msg = ListIndex(sdata, ListLength(sdata))
				If Left(msg, 1) = """" Then
					msg = Mid(msg, 2, Len(msg) - 2)
				End If
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		With u
			For i = 1 To .CountFeature
				If .Feature(i) = sname0 Then
					fdata = .FeatureData(i)
					If ListIndex(fdata, 1) = "解説" Then
						msg = ListIndex(fdata, ListLength(fdata))
						If Left(msg, 1) = """" Then
							msg = Mid(msg, 2, Len(msg) - 2)
						End If
					End If
				End If
			Next 
		End With
		If Not u2 Is Nothing Then
			With u2
				For i = 1 To .CountFeature
					If .Feature(i) = sname0 Then
						fdata = .FeatureData(i)
						If ListIndex(fdata, 1) = "解説" Then
							msg = ListIndex(fdata, ListLength(fdata))
							If Left(msg, 1) = """" Then
								msg = Mid(msg, 2, Len(msg) - 2)
							End If
						End If
					End If
				Next 
			End With
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "レベル")
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		'End If
		
		SkillHelpMessage = msg
	End Function
	
	
	'Invalid_string_refer_to_original_code
	Public Sub FeatureHelp(ByRef u As Unit, ByVal findex As Object, ByVal is_additional As Boolean)
		Dim fname As String
		Dim msg As String
		Dim prev_mode As Boolean
		
		With u
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If findex = "武器・防具クラス" Then
				'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				fname = findex
			ElseIf IsNumeric(findex) Then 
				'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				fname = .AllFeatureName(CShort(findex))
			Else
				fname = .AllFeatureName(findex)
			End If
		End With
		
		msg = FeatureHelpMessage(u, findex, is_additional)
		
		'解説の表示
		If Len(msg) > 0 Then
			prev_mode = AutoMessageMode
			AutoMessageMode = False
			
			OpenMessageForm()
			If AutoMoveCursor Then
				MoveCursorPos("Invalid_string_refer_to_original_code")
			End If
			DisplayMessage("Invalid_string_refer_to_original_code", "<b>" & fname & "</b>;" & msg)
			CloseMessageForm()
			
			AutoMessageMode = prev_mode
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureHelpMessage(ByRef u As Unit, ByVal findex As Object, ByVal is_additional As Boolean) As String
		Dim fid As Short
		Dim fname, ftype, fname0 As String
		Dim fdata, opt As String
		Dim flevel, lv_mod As Double
		Dim flevel_specified As Boolean
		Dim msg As String
		Dim i, idx As Short
		Dim buf As String
		Dim prob As Short
		Dim p As Pilot
		Dim sname As String
		Dim slevel As Double
		Dim uname As String
		
		With u
			'Invalid_string_refer_to_original_code
			p = .MainPilot
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If findex = "武器・防具クラス" Then
				'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				ftype = findex
				'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				fname = findex
			ElseIf IsNumeric(findex) Then 
				'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				fid = CShort(findex)
				ftype = .AllFeature(fid)
				fname = .AllFeatureName(fid)
				fdata = .AllFeatureData(fid)
				flevel = .AllFeatureLevel(fid)
				flevel_specified = .AllFeatureLevelSpecified(fid)
			Else
				ftype = .AllFeature(findex)
				fname = .AllFeatureName(findex)
				fdata = .AllFeatureData(findex)
				flevel = .AllFeatureLevel(findex)
				flevel_specified = .AllFeatureLevelSpecified(findex)
				For fid = 1 To .CountFeature
					'UPGRADE_WARNING: �I�u�W�F�N�g findex �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					If .AllFeature(fid) = findex Then
						Exit For
					End If
				Next 
			End If
			If InStr(fname, "Lv") > 0 Then
				fname0 = Left(fname, InStr(fname, "Lv") - 1)
			Else
				fname0 = fname
			End If
			
			'Invalid_string_refer_to_original_code
			Select Case ftype
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					For i = 1 To u.CountAllFeature
						If i <> fid And .AllFeature(i) = ftype And .AllFeatureData(i) = fdata Then
							flevel = flevel + .AllFeatureLevel(i)
						End If
					Next 
			End Select
		End With
		
		Select Case ftype
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				If p.IsSkillAvailable("Invalid_string_refer_to_original_code") Then
					prob = (p.SkillLevel("Invalid_string_refer_to_original_code") + 1) * 100 \ 16
				End If
				msg = "(" & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				If flevel > 0 Then
					msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				If p.IsSkillAvailable("Invalid_string_refer_to_original_code") Then
					prob = (p.SkillLevel("Invalid_string_refer_to_original_code") + 2) * 100 \ 16
				End If
				msg = "(" & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "盾"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				slevel = p.SkillLevel("Invalid_string_refer_to_original_code")
				If slevel > 0 Then
					slevel = 100 * slevel + 400
				End If
				msg = VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "バリア"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				msg = msg & "ダメージ" & VB6.Format(CInt(1000 * flevel)) & "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 3)) Then
					If StrToLng(LIndex(fdata, 3)) > 0 Then
						msg = msg & ";発動時に" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
						msg = msg & ";発動時に" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("気力", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "相殺"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Case "近接無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "バリア無効化無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "手動"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				msg = msg & "ダメージ" & VB6.Format(CInt(1000 * flevel)) & "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 3)) Then
					If StrToLng(LIndex(fdata, 3)) > 0 Then
						msg = msg & "発動時に" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
						msg = msg & ";発動時に" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & "発動時に10" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("気力", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "相殺"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Case "近接無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "バリア無効化無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "手動"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "隣接する味方ユニットに対する"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				msg = msg & "ダメージ" & VB6.Format(CInt(1000 * flevel)) & "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 4)) Then
					If StrToLng(LIndex(fdata, 4)) > 0 Then
						msg = msg & ";発動時に" & LIndex(fdata, 4) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
						msg = msg & ";発動時に" & Mid(LIndex(fdata, 4), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & ";発動時に" & VB6.Format(20 * i) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If StrToLng(LIndex(fdata, 3)) > 0 Then
					msg = msg & ";発動時に" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";発動時に" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("気力", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "相殺"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Case "近接無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "バリア無効化無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "手動"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If StrToLng(LIndex(fdata, 3)) > 0 Then
					msg = msg & ";発動時に" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";発動時に" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("気力", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "相殺"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Case "近接無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "バリア無効化無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "手動"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "隣接する味方ユニットに対する"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					If StrToLng(LIndex(fdata, 4)) > 0 Then
						msg = msg & ";発動時に" & LIndex(fdata, 4) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
						msg = msg & ";発動時に" & Mid(LIndex(fdata, 4), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & ";発動時に" & VB6.Format(20 * i) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel > 10 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				ElseIf flevel >= 0 Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If Not IsNumeric(LIndex(fdata, 3)) Then
					msg = msg & ";発動時に10" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) > 0 Then 
					msg = msg & ";発動時に" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";発動時に" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("気力", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "相殺"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Case "近接無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "バリア無効化無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "手動"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.2
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				sname = p.SkillName0("Invalid_string_refer_to_original_code")
				prob = p.SkillLevel("Invalid_string_refer_to_original_code") * 100 \ 16
				msg = sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel > 10 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				ElseIf flevel >= 0 Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If Not IsNumeric(LIndex(fdata, 3)) Then
					msg = msg & ";発動時に10" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) > 0 Then 
					msg = msg & ";発動時に" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
					msg = msg & ";発動時に" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & Term("気力", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				For i = 5 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "相殺"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Case "近接無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "バリア無効化無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "手動"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 0.2
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "隣接する味方ユニットに対する"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel > 10 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				ElseIf flevel >= 0 Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 4)) Then
					If StrToLng(LIndex(fdata, 4)) > 0 Then
						msg = msg & ";発動時に" & LIndex(fdata, 4) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
						msg = msg & ";発動時に" & Mid(LIndex(fdata, 4), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & ";発動時に" & VB6.Format(20 * i) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & Term("気力", u) & LIndex(fdata, 3) & "Invalid_string_refer_to_original_code"
				End If
				For i = 4 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 2
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel > 10 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				ElseIf flevel >= 0 Then 
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & Term("気力", u) & LIndex(fdata, 3) & "Invalid_string_refer_to_original_code"
				End If
				For i = 4 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 5
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 2
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 50
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "当て身技"
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				
				If flevel <> 1 Then
					msg = msg & "ダメージ" & VB6.Format(CInt(500 * flevel)) & "までの"
				End If
				
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "Invalid_string_refer_to_original_code"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				End If
				
				buf = LIndex(fdata, 2)
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				msg = msg & buf & "Invalid_string_refer_to_original_code"
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";発動時に" & LIndex(fdata, 5) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";発動時に" & Mid(LIndex(fdata, 5), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 6) & "Invalid_string_refer_to_original_code"
				End If
				For i = 7 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "相殺"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Case "近接無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "手動"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				
				If flevel <> 1 Then
					msg = msg & "ダメージ" & VB6.Format(CInt(500 * flevel)) & "までの"
				End If
				
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 3)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "Invalid_string_refer_to_original_code"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				End If
				
				If StrToLng(LIndex(fdata, 4)) > 0 Then
					msg = msg & ";発動時に" & LIndex(fdata, 4) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
					msg = msg & ";発動時に" & Mid(LIndex(fdata, 4), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				End If
				For i = 6 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "相殺"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Case "近接無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "手動"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "阻止"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel <> 1 Then
					msg = msg & "ダメージ" & VB6.Format(CInt(500 * flevel)) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 3)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "Invalid_string_refer_to_original_code"
					Else
						' MOD START MARGE
						'Invalid_string_refer_to_original_code
						msg = msg & "Invalid_string_refer_to_original_code"
						' MOD END MARGE
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				End If
				
				If StrToLng(LIndex(fdata, 4)) > 0 Then
					msg = msg & ";発動時に" & LIndex(fdata, 4) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 4)) < 0 Then 
					msg = msg & ";発動時に" & Mid(LIndex(fdata, 4), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 5)) > 50 Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				End If
				For i = 6 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "相殺"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Case "近接無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "手動"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 2)) And LIndex(fdata, 2) <> "1" Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					i = CShort(LIndex(fdata, 2))
				Else
					msg = "隣接する味方ユニットに対する"
					i = 1
				End If
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel <> 1 Then
					msg = msg & "ダメージ" & VB6.Format(CInt(500 * flevel)) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						' MOD START MARGE
						'Invalid_string_refer_to_original_code
						msg = msg & buf & "Invalid_string_refer_to_original_code"
						' MOD END MARGE
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				End If
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";発動時に" & LIndex(fdata, 5) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";発動時に" & Mid(LIndex(fdata, 5), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & Term("気力", u) & LIndex(fdata, 6) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "融合"
				prob = flevel * 100 \ 16
				msg = VB6.Format(flevel) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "変換"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = msg & VB6.Format(0.01 * flevel)
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 3) <> "" And LIndex(fdata, 3) <> "全" Then
					If Left(LIndex(fdata, 3), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				
				If flevel <> 1 Then
					msg = msg & "ダメージ" & VB6.Format(CInt(500 * flevel)) & "までの"
				End If
				
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "Invalid_string_refer_to_original_code"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "Invalid_string_refer_to_original_code"
				End If
				
				buf = LIndex(fdata, 2)
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				msg = msg & buf & "Invalid_string_refer_to_original_code"
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";発動時に" & LIndex(fdata, 5) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";発動時に" & Mid(LIndex(fdata, 5), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 6) & "Invalid_string_refer_to_original_code"
				End If
				For i = 7 To LLength(fdata)
					opt = LIndex(fdata, i)
					idx = InStr(opt, "*")
					If idx > 0 Then
						lv_mod = StrToDbl(Mid(opt, idx + 1))
						opt = Left(opt, idx - 1)
					Else
						lv_mod = -1
					End If
					Select Case p.SkillType(opt)
						Case "相殺"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							msg = msg & ";" & fname0 & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Case "近接無効"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "手動"
							msg = msg & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 20
							End If
							If u.SyncLevel >= 30 Then
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(+" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							ElseIf u.SyncLevel > 0 Then 
								msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が変化(" & VB6.Format(lv_mod * (u.SyncLevel - 30)) & "Invalid_string_refer_to_original_code"
							End If
						Case "霊力"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 10
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PlanaLevel) & "Invalid_string_refer_to_original_code"
						Case "オーラ"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.AuraLevel) & "Invalid_string_refer_to_original_code"
						Case "Invalid_string_refer_to_original_code"
							sname = p.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "により強度が増加(+" & VB6.Format(lv_mod * u.PsychicLevel) & "Invalid_string_refer_to_original_code"
						Case Else
							sname = u.SkillName0(opt)
							If lv_mod = -1 Then
								lv_mod = 200
							End If
							msg = msg & "Invalid_string_refer_to_original_code" & sname & "レベルにより強度が増加(+" & VB6.Format(lv_mod * u.SkillLevel(opt)) & "Invalid_string_refer_to_original_code"
					End Select
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				msg = "毎ターン最大" & Term("Invalid_string_refer_to_original_code", u) & "の" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "毎ターン最大" & Term("Invalid_string_refer_to_original_code", u) & "の" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "霊力回復"
				sname = p.SkillName0("霊力")
				msg = "毎ターン最大" & sname & "の" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & sname & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "毎ターン最大" & Term("Invalid_string_refer_to_original_code", u) & "の" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "毎ターン最大" & Term("Invalid_string_refer_to_original_code", u) & "の" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "霊力消費"
				sname = p.SkillName0("霊力")
				msg = "毎ターン最大" & sname & "の" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & sname & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code"
				If IsNumeric(LIndex(fdata, 2)) Then
					If StrToLng(LIndex(fdata, 2)) > 0 Then
						msg = msg & ";発動時に" & LIndex(fdata, 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 2)) < 0 Then 
						msg = msg & ";発動時に" & Mid(LIndex(fdata, 2), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 3) & "Invalid_string_refer_to_original_code"
				End If
				If LIndex(fdata, 4) = "手動" Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code" & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If IsNumeric(LIndex(fdata, 3)) Then
					If StrToLng(LIndex(fdata, 3)) > 0 Then
						msg = msg & ";発動時に" & LIndex(fdata, 3) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					ElseIf StrToLng(LIndex(fdata, 3)) < 0 Then 
						msg = msg & ";発動時に" & Mid(LIndex(fdata, 3), 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				End If
				If StrToLng(LIndex(fdata, 4)) > 50 Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 4) & "Invalid_string_refer_to_original_code"
				End If
				If LIndex(fdata, 5) = "手動" Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				buf = fname
				If InStr(buf, "Lv") Then
					buf = Left(buf, InStr(buf, "Lv") - 1)
				End If
				msg = buf & "Invalid_string_refer_to_original_code" & VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 2) <> "" And LIndex(fdata, 2) <> "全" Then
					If Left(LIndex(fdata, 2), 1) = "!" Then
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If flevel >= 0 Then
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 - 10 * flevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code" & VB6.Format(CInt(100 - 10 * flevel)) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 3)) > 50 Then
					msg = msg & Term("気力", u) & LIndex(fdata, 3) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "抵抗力"
				If flevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(-10 * flevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u)
				Select Case flevel
					Case 1
						msg = msg & "を最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					Case 2
						msg = msg & "を最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					Case 3
						msg = msg & "Invalid_string_refer_to_original_code"
				End Select
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
				If IsOptionDefined("移動後補給不可") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				For i = 2 To CInt(fdata)
					buf = LIndex(fdata, i)
					If Left(buf, 1) = "!" Then
						buf = Mid(buf, 2)
						msg = msg & buf & "以外では" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					Else
						msg = msg & buf & "では" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Next 
				msg = msg & buf & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "霊力変換器"
				sname = p.SkillName0("霊力")
				msg = sname & "Invalid_string_refer_to_original_code"
				If flevel_specified Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				
			Case "オーラ変換器"
				sname = p.SkillName0("オーラ")
				msg = sname & "Invalid_string_refer_to_original_code"
				If flevel_specified Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = sname & "レベルごとに" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If flevel_specified Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = sname & "Invalid_string_refer_to_original_code"
				If flevel_specified Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel_specified Then
					msg = "敵から" & StrConv(VB6.Format(flevel), VbStrConv.Wide) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If LLength(fdata) > 1 Then
					If CShort(LIndex(fdata, 2)) > 0 Then
						msg = msg & LIndex(fdata, 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = msg & "40" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("移動力", u) & VB6.Format(u.Speed + flevel) & "Invalid_string_refer_to_original_code"
				If LLength(fdata) > 1 Then
					If StrToLng(LIndex(fdata, 2)) > 0 Then
						msg = msg & ";" & LIndex(fdata, 2) & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				End If
				
			Case "水泳"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = msg & LIndex(fdata, 2)
				For i = 3 To LLength(fdata)
					msg = msg & "Invalid_string_refer_to_original_code"
				Next 
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "進入不可"
				msg = msg & LIndex(fdata, 2)
				For i = 3 To LLength(fdata)
					msg = msg & "Invalid_string_refer_to_original_code"
				Next 
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = msg & LIndex(fdata, 2)
				For i = 3 To LLength(fdata)
					msg = msg & "Invalid_string_refer_to_original_code"
				Next 
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "追加移動力"
				msg = LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If flevel >= 0 Then
					msg = msg & VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(-flevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "母艦"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "格納不可"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "変形"
				If u.IsHero Then
					buf = "変化"
				Else
					buf = "変形"
				End If
				If LLength(fdata) > 2 Then
					msg = "Invalid_string_refer_to_original_code" & buf & "; "
					For i = 2 To LLength(fdata)
						If u.OtherForm(LIndex(fdata, i)).IsAvailable() Then
							If u.Nickname = UDList.Item(LIndex(fdata, i)).Nickname Then
								uname = UDList.Item(LIndex(fdata, i)).Name
								If Right(uname, 5) = "Invalid_string_refer_to_original_code" Then
									uname = Left(uname, Len(uname) - 5)
								ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
									uname = Left(uname, Len(uname) - 5) & ")"
								ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
									uname = Left(uname, Len(uname) - 5)
								End If
							Else
								uname = UDList.Item(LIndex(fdata, i)).Nickname
							End If
							msg = msg & uname & "  "
						End If
					Next 
				Else
					If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
						uname = UDList.Item(LIndex(fdata, 2)).Name
					Else
						uname = UDList.Item(LIndex(fdata, 2)).Nickname
					End If
					If Right(uname, 5) = "Invalid_string_refer_to_original_code" Then
						uname = Left(uname, Len(uname) - 5)
					ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
						uname = Left(uname, Len(uname) - 5) & ")"
					ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
						uname = Left(uname, Len(uname) - 5)
					End If
					msg = "<B>" & uname & "</B>に" & buf & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
					uname = UDList.Item(LIndex(fdata, 2)).Name
				Else
					uname = UDList.Item(LIndex(fdata, 2)).Nickname
				End If
				If Right(uname, 5) = "Invalid_string_refer_to_original_code" Then
					uname = Left(uname, Len(uname) - 5)
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5) & ")"
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5)
				End If
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If flevel_specified Then
					msg = msg & ";ユニット破壊時に" & VB6.Format(10 * flevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If u.Nickname = UDList.Item(fdata).Nickname Then
					uname = UDList.Item(fdata).Name
				Else
					uname = UDList.Item(fdata).Nickname
				End If
				If Right(uname, 5) = "Invalid_string_refer_to_original_code" Then
					uname = Left(uname, Len(uname) - 5)
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5) & ")"
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5)
				End If
				msg = "Invalid_string_refer_to_original_code" & uname & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If u.Nickname = UDList.Item(LIndex(fdata, 2)).Nickname Then
					uname = UDList.Item(LIndex(fdata, 2)).Name
				Else
					uname = UDList.Item(LIndex(fdata, 2)).Nickname
				End If
				If Right(uname, 5) = "Invalid_string_refer_to_original_code" Then
					uname = Left(uname, Len(uname) - 5)
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5) & ")"
				ElseIf Right(uname, 5) = "Invalid_string_refer_to_original_code" Then 
					uname = Left(uname, Len(uname) - 5)
				End If
				If u.Nickname <> uname Then
					uname = "<B>" & uname & "</B>"
				Else
					uname = ""
				End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = Term("気力", u) & VB6.Format(100 + 10 * flevel) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'UPGRADE_WARNING: FeatureHelpMessage �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
				msg = Term("気力", u) & VB6.Format(100 + 10 * flevel) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = Term("Invalid_string_refer_to_original_code", u) & "が最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				'End If
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = msg & "Invalid_string_refer_to_original_code"
				'End If
				If u.IsHero Then
					msg = msg & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If u.IsHero Then
					buf = "Invalid_string_refer_to_original_code"
				Else
					buf = "Invalid_string_refer_to_original_code"
				End If
				If LLength(fdata) > 3 Then
					If UDList.IsDefined(LIndex(fdata, 2)) Then
						msg = "Invalid_string_refer_to_original_code" & UDList.Item(LIndex(fdata, 2)).Nickname & "</B>に" & buf & "; "
					Else
						msg = "Invalid_string_refer_to_original_code" & LIndex(fdata, 2) & "</B>に" & buf & "; "
					End If
					
					For i = 3 To LLength(fdata)
						If UDList.IsDefined(LIndex(fdata, i)) Then
							msg = msg & UDList.Item(LIndex(fdata, i)).Nickname & "  "
						Else
							msg = msg & LIndex(fdata, i) & "  "
						End If
					Next 
				Else
					If UDList.IsDefined(LIndex(fdata, 3)) Then
						msg = UDList.Item(LIndex(fdata, 3)).Nickname & "と合体し"
					Else
						msg = LIndex(fdata, 3) & "と合体し"
					End If
					If UDList.IsDefined(LIndex(fdata, 2)) Then
						msg = msg & UDList.Item(LIndex(fdata, 2)).Nickname & "に" & buf
					Else
						msg = msg & LIndex(fdata, 2) & "に" & buf
					End If
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				For i = 2 To LLength(fdata)
					If UDList.IsDefined(LIndex(fdata, i)) Then
						msg = msg & UDList.Item(LIndex(fdata, i)).Nickname & "  "
					Else
						msg = msg & LIndex(fdata, i) & "  "
					End If
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If LLength(fdata) = 2 Then
					If Not PDList.IsDefined(LIndex(fdata, 2)) Then
						ErrorMessage("Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Exit Function
					End If
					msg = PDList.Item(LIndex(fdata, 2)).Nickname & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					For i = 2 To LLength(fdata)
						If Not PDList.IsDefined(LIndex(fdata, 2)) Then
							ErrorMessage("Invalid_string_refer_to_original_code")
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							Exit Function
						End If
						msg = msg & PDList.Item(LIndex(fdata, i)).Nickname & "  "
					Next 
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = msg & VB6.Format(100 - 5 * flevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & VB6.Format(100 - 5 * flevel) & "Invalid_string_refer_to_original_code"
				End If
				buf = fname
				If InStr(buf, "Lv") Then
					buf = Left(buf, InStr(buf, "Lv") - 1)
				End If
				msg = msg & "Invalid_string_refer_to_original_code" & buf & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = Term("気力", u) & "Invalid_string_refer_to_original_code"
				msg = Term("気力", u) & "Invalid_string_refer_to_original_code"
				'End If
				
			Case "防御不可"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "回避不可"
				msg = "Invalid_string_refer_to_original_code"
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If p.HasMana() Then
					If flevel >= 0 Then
						msg = "Invalid_string_refer_to_original_code" & Term("魔力", u) & "Invalid_string_refer_to_original_code" & VB6.Format(CShort(5 * flevel)) & "Invalid_string_refer_to_original_code"
					Else
						msg = "Invalid_string_refer_to_original_code" & Term("魔力", u) & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				Else
					If flevel >= 0 Then
						msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & VB6.Format(CShort(5 * flevel)) & "Invalid_string_refer_to_original_code"
					Else
						msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("命中", u) & "Invalid_string_refer_to_original_code" & VB6.Format(CShort(5 * flevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("命中", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & "気力" & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("回避", u) & "Invalid_string_refer_to_original_code" & VB6.Format(CShort(5 * flevel)) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("回避", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = "最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = "最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = "最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = "最大" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = Term("運動性", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = Term("運動性", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					msg = Term("移動力", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = Term("移動力", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel >= 0 Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If IsNumeric(LIndex(fdata, 2)) Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 2) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "武器・防具クラス"
				fdata = Trim(u.WeaponProficiency)
				If fdata <> "" Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = "Invalid_string_refer_to_original_code"
				End If
				fdata = Trim(u.ArmorProficiency)
				If fdata <> "" Then
					msg = msg & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If LIndex(fdata, 3) <> "全" Then
					buf = LIndex(fdata, 3)
					If Left(buf, 1) = "@" Then
						msg = Mid(buf, 2) & "による"
					Else
						msg = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
				
				msg = msg & "Invalid_string_refer_to_original_code"
				
				buf = LIndex(fdata, 4)
				If IsNumeric(buf) Then
					If buf <> "100" Then
						msg = msg & buf & "Invalid_string_refer_to_original_code"
					End If
				ElseIf InStr(buf, "+") > 0 Or InStr(buf, "-") > 0 Then 
					i = MaxLng(InStr(buf, "+"), InStr(buf, "-"))
					sname = u.SkillName0(Left(buf, i - 1))
					prob = (u.SkillLevel(Left(buf, i - 1)) + CShort(Mid(buf, i))) * 100 \ 16
					msg = msg & "(" & sname & "Lv" & Mid(buf, i) & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "%)で"
				Else
					sname = u.SkillName0(buf)
					prob = u.SkillLevel(buf) * 100 \ 16
					msg = msg & sname & "Invalid_string_refer_to_original_code" & VB6.Format(prob) & "%)で"
				End If
				
				buf = LIndex(fdata, 2)
				If InStr(buf, "(") > 0 Then
					buf = Left(buf, InStr(buf, "(") - 1)
				End If
				msg = msg & buf & "Invalid_string_refer_to_original_code"
				
				If StrToLng(LIndex(fdata, 5)) > 0 Then
					msg = msg & ";発動時に" & LIndex(fdata, 5) & "Invalid_string_refer_to_original_code"
				ElseIf StrToLng(LIndex(fdata, 5)) < 0 Then 
					msg = msg & ";発動時に" & Mid(LIndex(fdata, 5), 2) & "Invalid_string_refer_to_original_code"
				End If
				If StrToLng(LIndex(fdata, 6)) > 50 Then
					msg = msg & ";" & Term("気力", u) & LIndex(fdata, 6) & "Invalid_string_refer_to_original_code"
				End If
				If InStr(fdata, "連鎖不可") > 0 Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If u.FeatureLevel("Invalid_string_refer_to_original_code") < 0 Then
					msg = "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				'End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel = 1 Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				If flevel = 1 Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				End If
				
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If LLength(fdata) < 2 Then
					buf = "1"
				Else
					buf = LIndex(fdata, 2)
				End If
				
				If flevel = 1 Then
					msg = msg & buf & "Invalid_string_refer_to_original_code"
				Else
					msg = msg & buf & "Invalid_string_refer_to_original_code" & VB6.Format(flevel) & "Invalid_string_refer_to_original_code"
				End If
				
				' ADD START MARGE
			Case "Invalid_string_refer_to_original_code"
				If LLength(fdata) > 1 Then
					For i = 2 To LLength(fdata)
						If i > 2 Then
							msg = msg & "Invalid_string_refer_to_original_code"
						End If
						msg = msg & LIndex(fdata, i)
					Next 
					msg = msg & "の"
				Else
					msg = msg & "全地形の"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				' ADD END MARGE
				
			Case Else
				If is_additional Then
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					msg = SkillHelpMessage(u.MainPilot, ftype)
					If Len(msg) > 0 Then
						Exit Function
					End If
					
					'Invalid_string_refer_to_original_code
					If Len(fdata) > 0 Then
						msg = ListIndex(fdata, ListLength(fdata))
						If Left(msg, 1) = """" Then
							msg = Mid(msg, 2, Len(msg) - 2)
						End If
					End If
					
					'Invalid_string_refer_to_original_code
					If Len(msg) = 0 Then
						Exit Function
					End If
				ElseIf Len(fdata) > 0 Then 
					'Invalid_string_refer_to_original_code
					msg = ListIndex(fdata, ListLength(fdata))
					If Left(msg, 1) = """" Then
						msg = Mid(msg, 2, Len(msg) - 2)
					End If
				ElseIf ListIndex(u.AllFeatureData(fname), 1) <> "解説" Then 
					'Invalid_string_refer_to_original_code
					Exit Function
				End If
				
		End Select
		
		fdata = u.AllFeatureData(fname0)
		If ListIndex(fdata, 1) = "解説" Then
			'Invalid_string_refer_to_original_code
			msg = ListTail(fdata, 2)
			If Left(msg, 1) = """" Then
				msg = Mid(msg, 2, Len(msg) - 2)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		'End If
		
		FeatureHelpMessage = msg
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function AttributeName(ByRef u As Unit, ByRef atr As String, Optional ByVal is_ability As Boolean = False) As String
		Dim fdata As String
		
		Select Case atr
			Case "全"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "格"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "低改造武器"
			Case "改"
				AttributeName = "低改造武器"
			Case "攻"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "反撃専用"
			Case "武"
				AttributeName = "格闘武器"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "接"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "オ"
				AttributeName = "オーラ技"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "シ"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "サ"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "吸"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "奪"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "貫"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "無"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "封印技"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "限定技"
			Case "殺"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "浸"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "破"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "♀"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "合体技"
			Case "共"
				If Not is_ability Then
					AttributeName = "弾薬共有武器"
				Else
					AttributeName = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "永"
				AttributeName = "永続武器"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "技"
				AttributeName = "技"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "音"
				If Not is_ability Then
					AttributeName = "Invalid_string_refer_to_original_code"
				Else
					AttributeName = "音波" & Term("Invalid_string_refer_to_original_code", u)
				End If
			Case "Invalid_string_refer_to_original_code"
				AttributeName = Term("気力", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "失"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "銭"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "消耗技"
			Case "自"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "変形技"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "劣"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "中"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "石"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "痺"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "眠"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "乱"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "盲"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "撹"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "止"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "除"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "即"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "脱"
				AttributeName = Term("気力", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = Term("気力", u) & "Invalid_string_refer_to_original_code"
			Case "低攻"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "低防"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "低運"
				AttributeName = Term("運動性", u) & "Invalid_string_refer_to_original_code"
			Case "低移"
				AttributeName = Term("移動力", u) & "Invalid_string_refer_to_original_code"
			Case "精"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "連"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "吹"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "転"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "暗殺技"
			Case "尽"
				AttributeName = "全" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "盗み"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "追"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "空"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "固"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "衰"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ゾ"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "害"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "ラーニング"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "変化"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "援"
				AttributeName = "支援専用" & Term("Invalid_string_refer_to_original_code", u)
			Case "難"
				AttributeName = "高難度" & Term("Invalid_string_refer_to_original_code", u)
			Case "地", "水", "火", "風", "冷", "雷", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				AttributeName = atr & "属性"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				AttributeName = atr & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "対"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "ラ"
				AttributeName = "ラーニング可能技"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "使用禁止"
			Case "Invalid_string_refer_to_original_code"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case "散"
				AttributeName = "Invalid_string_refer_to_original_code"
			Case Else
				If Left(atr, 1) = "弱" Then
					AttributeName = Mid(atr, 2) & "Invalid_string_refer_to_original_code"
				ElseIf Left(atr, 1) = "効" Then 
					AttributeName = Mid(atr, 2) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					AttributeName = Mid(atr, 2) & "Invalid_string_refer_to_original_code"
				End If
		End Select
		
		If Not u Is Nothing Then
			fdata = u.FeatureData(atr)
			If ListIndex(fdata, 1) = "解説" Then
				'Invalid_string_refer_to_original_code
				AttributeName = ListIndex(fdata, 2)
				Exit Function
			End If
		End If
		
		If is_ability Then
			'Invalid_string_refer_to_original_code_
			'Or Right$(AttributeName, 2) = "武器" _
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			AttributeName = Left(AttributeName, Len(AttributeName) - 2) & Term("Invalid_string_refer_to_original_code", u)
		End If
		'End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub AttributeHelp(ByRef u As Unit, ByRef atr As String, ByVal idx As Short, Optional ByVal is_ability As Boolean = False)
		Dim msg, aname As String
		Dim prev_mode As Boolean
		
		msg = AttributeHelpMessage(u, atr, idx, is_ability)
		
		'解説の表示
		If Len(msg) > 0 Then
			prev_mode = AutoMessageMode
			AutoMessageMode = False
			
			OpenMessageForm()
			If AutoMoveCursor Then
				MoveCursorPos("Invalid_string_refer_to_original_code")
			End If
			If InStr(atr, "L") > 0 Then
				aname = AttributeName(u, Left(atr, InStr(atr, "L") - 1), is_ability) & "レベル" & StrConv(VB6.Format(Mid(atr, InStr(atr, "L") + 1)), VbStrConv.Wide)
			Else
				aname = AttributeName(u, atr, is_ability)
			End If
			DisplayMessage("Invalid_string_refer_to_original_code", "<b>" & aname & "</b>;" & msg)
			CloseMessageForm()
			
			AutoMessageMode = prev_mode
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function AttributeHelpMessage(ByRef u As Unit, ByRef atr As String, ByVal idx As Short, ByVal is_ability As Boolean) As String
		Dim atype As String
		Dim alevel As Double
		Dim msg, whatsthis As String
		Dim wanickname, waname, uname As String
		Dim p As Pilot
		Dim i, j As Short
		Dim buf As String
		Dim fdata As String
		
		'Invalid_string_refer_to_original_code
		If InStr(atr, "L") > 0 Then
			atype = Left(atr, InStr(atr, "L") - 1)
			alevel = CDbl(Mid(atr, InStr(atr, "L") + 1))
		Else
			atype = atr
			alevel = DEFAULT_LEVEL
		End If
		
		With u
			'Invalid_string_refer_to_original_code
			If Not is_ability Then
				waname = .Weapon(idx).Name
				wanickname = .WeaponNickname(idx)
				whatsthis = "Invalid_string_refer_to_original_code"
			Else
				waname = .Ability(idx).Name
				wanickname = .AbilityNickname(idx)
				whatsthis = Term("Invalid_string_refer_to_original_code", u)
			End If
			
			'Invalid_string_refer_to_original_code
			p = .MainPilot
		End With
		
		Select Case atype
			Case "格"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "Invalid_string_refer_to_original_code"
				If p.HasMana() Then
					msg = "Invalid_string_refer_to_original_code" & Term("魔力", u) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "Invalid_string_refer_to_original_code"
				If p.HasMana() Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "攻"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				msg = "Invalid_string_refer_to_original_code"
			Case "改"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
			Case "武"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "接"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("距離修正") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "オ"
				msg = "Invalid_string_refer_to_original_code" & p.SkillName0("オーラ") & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "シ"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "サ"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If IsOptionDefined("距離修正") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "吸"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "奪"
				msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "貫"
				If alevel > 0 Then
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "を本来の" & VB6.Format(100 - 10 * alevel) & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "無"
				msg = "Invalid_string_refer_to_original_code"
			Case "浸"
				msg = "Invalid_string_refer_to_original_code"
			Case "破"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "敵の" & p.SkillName0("再生") & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "殺"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "♀"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "使用すると" & VB6.Format(alevel) & "Invalid_string_refer_to_original_code"
				If Not is_ability Then
					For i = 1 To u.CountWeapon
						If i <> idx And wanickname = u.WeaponNickname(i) Then
							msg = msg & "Invalid_string_refer_to_original_code"
							Exit For
						End If
					Next 
					If u.IsWeaponClassifiedAs(idx, "共") And u.Weapon(idx).Bullet = 0 Then
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
				Else
					For i = 1 To u.CountAbility
						If i <> idx And wanickname = u.AbilityNickname(i) Then
							msg = msg & "同名の" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
							Exit For
						End If
					Next 
					If u.IsAbilityClassifiedAs(idx, "共") And u.Ability(idx).Stock = 0 Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				For i = 1 To u.CountFeature
					If u.Feature(i) = "合体技" And LIndex(u.FeatureData(i), 1) = waname Then
						Exit For
					End If
				Next 
				If i > u.CountFeature Then
					ErrorMessage(u.Name & "Invalid_string_refer_to_original_code")
					& "」に対応した合体技能力がありません"
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Exit Function
				End If
				If LLength(u.FeatureData(i)) = 2 Then
					uname = LIndex(u.FeatureData(i), 2)
					If UDList.IsDefined(uname) Then
						uname = UDList.Item(uname).Nickname
					End If
					If uname = u.Nickname Then
						msg = "Invalid_string_refer_to_original_code" & uname & "Invalid_string_refer_to_original_code"
					Else
						msg = uname & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = "Invalid_string_refer_to_original_code"
					For j = 2 To LLength(u.FeatureData(i))
						uname = LIndex(u.FeatureData(i), j)
						If UDList.IsDefined(uname) Then
							uname = UDList.Item(uname).Nickname
						End If
						msg = msg & uname & "  "
					Next 
				End If
			Case "共"
				If Not is_ability Then
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					End If
				End If
			Case "Invalid_string_refer_to_original_code"
				If Not is_ability Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "回数制の" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "永"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If buf = "非表示" Then
					buf = "Invalid_string_refer_to_original_code"
				End If
				msg = buf & "技能によって" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				If is_ability Then
					msg = msg & "Invalid_string_refer_to_original_code" & Term("魔力", u) & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "技"
				buf = p.SkillName0("技")
				If buf = "非表示" Then
					buf = "技"
				End If
				msg = buf & "技能によって" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "音"
				If Not is_ability Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "使用時に気力" & VB6.Format(5 * alevel) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = "使用時に" & VB6.Format(5 * alevel) & p.SkillName0("霊力") & "Invalid_string_refer_to_original_code"
			Case "失"
				msg = "使用時に" & VB6.Format(alevel * u.MaxHP \ 10) & "の" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "銭"
				msg = "使用時に" & VB6.Format(MaxLng(alevel, 1) * u.Value \ 10) & "の" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "尽"
				If Not is_ability Then
					If alevel > 0 Then
						msg = "全" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & "(残り" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					Else
						msg = "全" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
					End If
				Else
					msg = "使用後に" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "自"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If u.IsFeatureAvailable("変形技") Then
					For i = 1 To u.CountFeature
						If u.Feature(i) = "変形技" And LIndex(u.FeatureData(i), 1) = waname Then
							uname = LIndex(u.FeatureData(i), 2)
							Exit For
						End If
					Next 
				End If
				If uname = "" Then
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
				If UDList.IsDefined(uname) Then
					With UDList.Item(uname)
						If u.Nickname <> .Nickname Then
							uname = .Nickname
						Else
							uname = .Name
						End If
					End With
				End If
				msg = "使用後に" & uname & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 2
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "劣"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "中"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "石"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "痺"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "眠"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "乱"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "盲"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "撹"
				If alevel = DEFAULT_LEVEL Then
					alevel = 2
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "止"
				If alevel = DEFAULT_LEVEL Then
					alevel = 1
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "除"
				If Not is_ability Then
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "即"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = "Invalid_string_refer_to_original_code"
					StrConv(Format$(CInt(alevel)), vbWide) & "ターン後に" & _
					Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "脱"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
				ElseIf alevel >= 0 Then 
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
				ElseIf alevel >= 0 Then 
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
			Case "低攻"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "低防"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "低運"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code" & Term("運動性", u) & "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "低移"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code" & Term("移動力", u) & "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "吹"
				If alevel > 0 Then
					msg = "相手ユニットを" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
				End If
			Case "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = "相手ユニットを" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Else
					msg = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				End If
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "転"
				msg = "Invalid_string_refer_to_original_code" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "Invalid_string_refer_to_original_code"
			Case "連"
				msg = VB6.Format(alevel) & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = VB6.Format(100 * alevel \ 16) & "Invalid_string_refer_to_original_code"
			Case "精"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "援"
				msg = "Invalid_string_refer_to_original_code"
			Case "難"
				msg = VB6.Format(10 * alevel) & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "盗めるものは通常は" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("距離修正") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "追"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("距離修正") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("距離修正") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
				msg = msg & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("距離修正") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("距離修正") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "空"
				msg = "Invalid_string_refer_to_original_code"
				If IsOptionDefined("高度修正") Then
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "固"
				msg = "Invalid_string_refer_to_original_code" & Term("気力", u) & "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "衰"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "を現在値の "
				Select Case CShort(alevel)
					Case 1
						msg = msg & "3/4"
					Case 2
						msg = msg & "1/2"
					Case 3
						msg = msg & "1/4"
				End Select
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & Term("Invalid_string_refer_to_original_code", u) & "を現在値の "
				Select Case CShort(alevel)
					Case 1
						msg = msg & "3/4"
					Case 2
						msg = msg & "1/2"
					Case 3
						msg = msg & "1/4"
				End Select
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If alevel = DEFAULT_LEVEL Then
					alevel = 3
				End If
				msg = "Invalid_string_refer_to_original_code"
				If alevel > 0 Then
					msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
				Else
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "ゾ"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "害"
				If alevel = DEFAULT_LEVEL Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "Invalid_string_refer_to_original_code"
					If alevel > 0 Then
						msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
					Else
						msg = msg & "Invalid_string_refer_to_original_code"
					End If
					msg = msg & "Invalid_string_refer_to_original_code"
				End If
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = msg & VB6.Format(100 + 10 * (alevel + 2))
				msg = msg & VB6.Format(100 + 25 * (alevel + 2))
				'End If
				msg = msg & "Invalid_string_refer_to_original_code"
			Case "地", "水", "火", "風", "冷", "雷", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				Select Case atype
					Case "水", "火", "風"
						msg = atype & "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						msg = atype & "Invalid_string_refer_to_original_code"
					Case "地"
						msg = "Invalid_string_refer_to_original_code"
					Case "冷"
						msg = "冷気による"
					Case "雷"
						msg = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						msg = "Invalid_string_refer_to_original_code"
					Case "木"
						msg = "Invalid_string_refer_to_original_code"
				End Select
				msg = msg & whatsthis & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				If Not is_ability Then
					msg = "Invalid_string_refer_to_original_code"
				Else
					msg = "魔法による" & Term("Invalid_string_refer_to_original_code", u) & "Invalid_string_refer_to_original_code"
				End If
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				msg = atype & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "瀕死時にのみ使用可能な" & whatsthis & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code"
			Case "対"
				If Not is_ability Then
					whatsthis = "Invalid_string_refer_to_original_code"
				End If
				msg = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Case "ラ"
				If Not is_ability Then
					whatsthis = "Invalid_string_refer_to_original_code"
				End If
				msg = "ラーニングが可能な" & whatsthis & "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				msg = "Invalid_string_refer_to_original_code" & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "Invalid_string_refer_to_original_code"
			Case "散"
				msg = "Invalid_string_refer_to_original_code"
			Case Else
				'弱、効、剋属性
				Select Case Left(atype, 1)
					Case "弱"
						If alevel = DEFAULT_LEVEL Then
							alevel = 3
						End If
						msg = "Invalid_string_refer_to_original_code" & Mid(atype, 2) & "Invalid_string_refer_to_original_code"
						If alevel > 0 Then
							msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
						Else
							msg = msg & "Invalid_string_refer_to_original_code"
						End If
						msg = msg & "Invalid_string_refer_to_original_code"
					Case "効"
						If alevel = DEFAULT_LEVEL Then
							alevel = 3
						End If
						msg = "Invalid_string_refer_to_original_code" & Mid(atype, 2) & "Invalid_string_refer_to_original_code"
						If alevel > 0 Then
							msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
						Else
							msg = msg & "Invalid_string_refer_to_original_code"
						End If
						msg = msg & "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						If alevel = DEFAULT_LEVEL Then
							alevel = 3
						End If
						msg = "Invalid_string_refer_to_original_code"
						Select Case Mid(atype, 2)
							Case "オ"
								msg = msg & "オーラ"
							Case "Invalid_string_refer_to_original_code"
								msg = msg & "Invalid_string_refer_to_original_code"
							Case "シ"
								msg = msg & "Invalid_string_refer_to_original_code"
							Case "サ"
								msg = msg & "Invalid_string_refer_to_original_code"
							Case "Invalid_string_refer_to_original_code"
								msg = msg & "霊力"
							Case "Invalid_string_refer_to_original_code"
								msg = msg & "Invalid_string_refer_to_original_code"
							Case "技"
								msg = msg & "技"
							Case Else
								msg = msg & Mid(atype, 2) & "Invalid_string_refer_to_original_code"
						End Select
						msg = msg & "Invalid_string_refer_to_original_code"
						If alevel > 0 Then
							msg = msg & StrConv(VB6.Format(CShort(alevel)), VbStrConv.Wide) & "ターン"
						Else
							msg = msg & "Invalid_string_refer_to_original_code"
						End If
						msg = msg & "Invalid_string_refer_to_original_code"
				End Select
		End Select
		
		fdata = u.FeatureData(atype)
		If ListIndex(fdata, 1) = "解説" Then
			'Invalid_string_refer_to_original_code
			msg = ListTail(fdata, 3)
			If Left(msg, 1) = """" Then
				msg = Mid(msg, 2, Len(msg) - 2)
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		ReplaceString(msg, "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code")
		ReplaceString(msg, "Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'End If
		
		AttributeHelpMessage = msg
	End Function
End Module