Option Strict Off
Option Explicit On
Friend Class SpecialPowerData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Name As String
	'Invalid_string_refer_to_original_code
	Public KanaName As String
	'Invalid_string_refer_to_original_code
	Public ShortName As String
	'Invalid_string_refer_to_original_code
	Public SPConsumption As Short
	'対象
	Public TargetType As String
	'Invalid_string_refer_to_original_code
	Public Duration As String
	'適用条件
	Public NecessaryCondition As String
	'アニメ
	Public Animation As String
	
	'効果名
	Private strEffectType() As String
	'効果レベル
	Private dblEffectLevel() As Double
	'効果データ
	Private strEffectData() As String
	
	'解説
	Public Comment As String
	
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		ReDim strEffectType(0)
		ReDim dblEffectLevel(0)
		ReDim strEffectData(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	
	'スペシャルパワーに効果を追加
	Public Sub SetEffect(ByRef elist As String)
		Dim j, i, k As Short
		Dim buf As String
		Dim elevel, etype, edata As String
		
		ReDim strEffectType(ListLength(elist))
		ReDim dblEffectLevel(ListLength(elist))
		ReDim strEffectData(ListLength(elist))
		
		TrimString(elist)
		For i = 1 To ListLength(elist)
			buf = ListIndex(elist, i)
			j = InStr(buf, "Lv")
			k = InStr(buf, "=")
			If j > 0 And (k = 0 Or j < k) Then
				'Invalid_string_refer_to_original_code
				strEffectType(i) = Left(buf, j - 1)
				
				If k > 0 Then
					'Invalid_string_refer_to_original_code
					dblEffectLevel(i) = CDbl(Mid(buf, j + 2, k - (j + 2)))
					
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
					
					If Name = "付加" And elevel = "" Then
						elevel = VB6.Format(DEFAULT_LEVEL)
					End If
					
					strEffectData(i) = Trim(etype & " " & elevel & " " & edata)
				Else
					'Invalid_string_refer_to_original_code
					dblEffectLevel(i) = CDbl(Mid(buf, j + 2))
				End If
			ElseIf k > 0 Then 
				'Invalid_string_refer_to_original_code
				strEffectType(i) = Left(buf, k - 1)
				
				buf = Mid(buf, k + 1)
				If Asc(buf) = 34 Then '"
					buf = Mid(buf, 2, Len(buf) - 2)
				End If
				
				j = InStr(buf, "Lv")
				k = InStr(buf, "=")
				
				If j > 0 Then
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
				
				If Name = "付加" And elevel = "" Then
					elevel = VB6.Format(DEFAULT_LEVEL)
				End If
				
				strEffectData(i) = Trim(etype & " " & elevel & " " & edata)
			Else
				'効果名のみ
				strEffectType(i) = buf
			End If
		Next 
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Function CountEffect() As Short
		CountEffect = UBound(strEffectType)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function EffectType(ByVal idx As Short) As String
		EffectType = strEffectType(idx)
	End Function
	
	'idx番目の特殊効果レベル
	Public Function EffectLevel(ByVal idx As Short) As Double
		EffectLevel = dblEffectLevel(idx)
	End Function
	
	'idx番目の特殊効果データ
	Public Function EffectData(ByVal idx As Short) As String
		EffectData = strEffectData(idx)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsEffectAvailable(ByRef ename As String) As Object
		Dim i As Short
		
		For i = 1 To CountEffect
			If ename = EffectType(i) Then
				'UPGRADE_WARNING: �I�u�W�F�N�g IsEffectAvailable �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				IsEffectAvailable = True
				Exit Function
			End If
			
			If EffectType(i) = "スペシャルパワー" Then
				'UPGRADE_WARNING: �I�u�W�F�N�g SPDList.Item(EffectData(i)).IsEffectAvailable(ename) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
				If SPDList.Item(EffectData(i)).IsEffectAvailable(ename) Then
					'UPGRADE_WARNING: �I�u�W�F�N�g IsEffectAvailable �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
					IsEffectAvailable = True
					Exit Function
				End If
			End If
		Next 
	End Function
	
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function Useful(ByRef p As Pilot) As Boolean
		Dim u As Unit
		Dim i As Short
		
		Select Case TargetType
			Case "Invalid_string_refer_to_original_code"
				Useful = Effective(p, (p.Unit_Renamed))
				Exit Function
				
			Case "味方", "全味方"
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						GoTo NextUnit1
						'End If
						
						'Invalid_string_refer_to_original_code
						Select Case p.Party
							Case "味方", "Invalid_string_refer_to_original_code"
								If .Party <> "味方" And .Party0 <> "味方" And .Party <> "Invalid_string_refer_to_original_code" And .Party0 <> "Invalid_string_refer_to_original_code" Then
									GoTo NextUnit1
								End If
							Case Else
								If p.Party <> .Party Then
									GoTo NextUnit1
								End If
						End Select
						
						'Invalid_string_refer_to_original_code
						If Effective(p, u) Then
							Useful = True
							Exit Function
						End If
					End With
NextUnit1: 
				Next u
				
			Case "破壊味方"
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						GoTo NextUnit2
						'End If
						
						'Invalid_string_refer_to_original_code
						If p.Party <> .Party0 Then
							GoTo NextUnit2
						End If
						
						'Invalid_string_refer_to_original_code
						If Effective(p, u) Then
							Useful = True
							Exit Function
						End If
					End With
NextUnit2: 
				Next u
				
			Case "敵", "全敵"
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						GoTo NextUnit3
						'End If
						
						'Invalid_string_refer_to_original_code
						Select Case p.Party
							Case "味方", "Invalid_string_refer_to_original_code"
								If (.Party = "味方" And .Party0 = "味方") Or (.Party = "Invalid_string_refer_to_original_code" And .Party0 = "Invalid_string_refer_to_original_code") Then
									GoTo NextUnit3
								End If
							Case Else
								If p.Party = .Party Then
									GoTo NextUnit3
								End If
						End Select
						
						'Invalid_string_refer_to_original_code
						If Effective(p, u) Then
							Useful = True
							Exit Function
						End If
					End With
NextUnit3: 
				Next u
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				For	Each u In UList
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					If Effective(p, u) Then
						Useful = True
						Exit Function
					End If
					'End If
				Next u
		End Select
	End Function
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function Effective(ByRef p As Pilot, ByRef t As Unit) As Boolean
		Dim i, j As Short
		Dim ncond As String
		Dim my_unit As Unit
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		my_unit = p.Unit_Renamed
		
		With t
			'Invalid_string_refer_to_original_code
			For i = 1 To LLength(NecessaryCondition)
				ncond = LIndex(NecessaryCondition, i)
				Select Case ncond
					Case "Invalid_string_refer_to_original_code"
						If p.Technique < .MainPilot.Technique Then
							GoTo ExitFunc
						End If
					Case "Invalid_string_refer_to_original_code"
						If .BossRank >= 0 Then
							GoTo ExitFunc
						End If
					Case "支援"
						If my_unit Is t Then
							GoTo ExitFunc
						End If
					Case "隣接"
						With my_unit
							If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) <> 1 Then
								GoTo ExitFunc
							End If
						End With
					Case Else
						If InStr(ncond, "Invalid_string_refer_to_original_code") = 1 Then
							With my_unit
								If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > StrToLng(Mid(ncond, 5)) Then
									GoTo ExitFunc
								End If
							End With
						End If
				End Select
				
				'Invalid_string_refer_to_original_code
				If Not my_unit Is p.Unit_Renamed Then
					my_unit.MainPilot()
				End If
			Next 
			
			'Invalid_string_refer_to_original_code
			Select Case TargetType
				Case "敵", "全敵", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					GoTo ExitFunc
					'End If
			End Select
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			If Duration <> "即効" Then
				If Not .IsSpecialPowerInEffect(Name) Then
					Effective = True
				End If
				
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If my_unit Is t Then
					Effective = False
					GoTo ExitFunc
				End If
			End If
			
			GoTo ExitFunc
			'End If
			
			'Invalid_string_refer_to_original_code
			For i = 1 To CountEffect
				Select Case EffectType(i)
					Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
						If EffectLevel(i) < 0 Then
							Effective = True
							GoTo ExitFunc
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						GoTo NextEffect
						'End If
						If .HP < .MaxHP Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
						If EffectLevel(i) < 0 Then
							Effective = True
							GoTo ExitFunc
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						GoTo NextEffect
						'End If
						If .EN < .MaxEN Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "霊力回復", "霊力増加"
						If EffectLevel(i) < 0 Then
							Effective = True
							GoTo ExitFunc
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						GoTo NextEffect
						'End If
						If .MainPilot.Plana < .MainPilot.MaxPlana Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "Invalid_string_refer_to_original_code"
						If EffectLevel(i) < 0 Then
							Effective = True
							GoTo ExitFunc
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						GoTo NextEffect
						'End If
						If .MainPilot.SP < .MainPilot.MaxSP Then
							Effective = True
							GoTo ExitFunc
						End If
						For j = 2 To .CountPilot
							If .Pilot(j).SP < .Pilot(j).MaxSP Then
								Effective = True
								GoTo ExitFunc
							End If
						Next 
						For j = 1 To .CountSupport
							If .Support(j).SP < .Support(j).MaxSP Then
								Effective = True
								GoTo ExitFunc
							End If
						Next 
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If .AdditionalSupport.SP < .AdditionalSupport.MaxSP Then
							Effective = True
							GoTo ExitFunc
						End If
						'End If
					Case "状態回復"
						'Invalid_string_refer_to_original_code_
						'Or .ConditionLifetime("混乱") > 0 _
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Or .ConditionLifetime("麻痺") > 0 _
						'Or .ConditionLifetime("睡眠") > 0 _
						'Invalid_string_refer_to_original_code_
						'Or .ConditionLifetime("盲目") > 0 _
						'Or .ConditionLifetime("撹乱") > 0 _
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Effective = True
						GoTo ExitFunc
						For j = 1 To .CountCondition
							If Len(.Condition(j)) > 6 Then
								If Right(.Condition(j), 6) = "Invalid_string_refer_to_original_code" Then
									If .ConditionLifetime(.Condition(j)) > 0 Then
										Effective = True
										GoTo ExitFunc
									End If
								End If
							End If
						Next 
						'End If
					Case "Invalid_string_refer_to_original_code"
						For j = 1 To .CountWeapon
							If .Bullet(j) < .MaxBullet(j) Then
								Effective = True
								GoTo ExitFunc
							End If
						Next 
					Case "行動数回復"
						If .Action = 0 And .MaxAction > 0 Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "行動数増加"
						If EffectLevel(i) < 0 Then
							Effective = True
							GoTo ExitFunc
						End If
						If .Action < 3 And .MaxAction > 0 Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "スペシャルパワー", "Invalid_string_refer_to_original_code"
						If Not .IsSpecialPowerInEffect(EffectData(i)) Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "気力増加"
						If .MainPilot.Personality <> "機械" And .MainPilot.Morale < .MainPilot.MaxMorale Then
							Effective = True
							GoTo ExitFunc
						End If
						For j = 2 To .CountPilot
							If .Pilot(j).Personality <> "機械" And .Pilot(j).Morale < .MainPilot.MaxMorale Then
								Effective = True
								GoTo ExitFunc
							End If
						Next 
					Case "Invalid_string_refer_to_original_code"
						If .MainPilot.Personality = "機械" Then
							GoTo NextEffect
						End If
						If .MainPilot.Morale > .MainPilot.MinMorale Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "ランダムダメージ", "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If Not .IsConditionSatisfied("無敵") Then
							Effective = True
							GoTo ExitFunc
						End If
					Case "気力増加", "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Effective = True
						GoTo ExitFunc
				End Select
NextEffect: 
			Next 
		End With
		
ExitFunc: 
		
		'Invalid_string_refer_to_original_code
		If Not my_unit Is p.Unit_Renamed Then
			my_unit.MainPilot()
		End If
	End Function
	
	
	'スペシャルパワーを使用する
	'Invalid_string_refer_to_original_code
	Public Sub Execute(ByRef p As Pilot, Optional ByVal is_event As Boolean = False)
		Dim u As Unit
		Dim i, j As Short
		
		Select Case TargetType
			Case "Invalid_string_refer_to_original_code"
				If Apply(p, p.Unit_Renamed, is_event) And Not is_event Then
					Sleep(300)
				End If
				
			Case "全味方"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						u = MapDataForUnit(i, j)
						If u Is Nothing Then
							GoTo NextUnit1
						End If
						With u
							'Invalid_string_refer_to_original_code
							Select Case p.Party
								Case "味方", "Invalid_string_refer_to_original_code"
									If .Party <> "味方" And .Party0 <> "味方" And .Party <> "Invalid_string_refer_to_original_code" And .Party0 <> "Invalid_string_refer_to_original_code" Then
										GoTo NextUnit1
									End If
								Case Else
									If p.Party <> .Party Then
										GoTo NextUnit1
									End If
							End Select
							
							Apply(p, u, is_event)
						End With
NextUnit1: 
					Next 
				Next 
				If Not is_event Then
					Sleep(300)
				End If
				
			Case "全敵"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						u = MapDataForUnit(i, j)
						If u Is Nothing Then
							GoTo NextUnit2
						End If
						With u
							'Invalid_string_refer_to_original_code
							Select Case p.Party
								Case "味方", "Invalid_string_refer_to_original_code"
									If .Party = "味方" Or .Party = "Invalid_string_refer_to_original_code" Then
										GoTo NextUnit2
									End If
								Case Else
									If p.Party = .Party Then
										GoTo NextUnit2
									End If
							End Select
							
							Apply(p, u, is_event)
						End With
NextUnit2: 
					Next 
				Next 
				If Not is_event Then
					Sleep(300)
				End If
				
			Case "全"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						u = MapDataForUnit(i, j)
						If Not u Is Nothing Then
							Apply(p, u, is_event)
						End If
					Next 
				Next 
				If Not is_event Then
					Sleep(300)
				End If
				
			Case Else
				If Apply(p, SelectedTarget, is_event) And Not is_event Then
					Sleep(300)
				End If
		End Select
		
		If Not is_event Then
			CloseMessageForm()
			RedrawScreen()
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function Apply(ByRef p As Pilot, ByVal t As Unit, Optional ByVal is_event As Boolean = False, Optional ByVal as_instant As Boolean = False) As Boolean
		Dim j, i, n As Short
		Dim tmp As Integer
		Dim need_update, is_invalid, displayed_string As Boolean
		Dim msg, ncond As String
		Dim my_unit As Unit
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		my_unit = p.Unit_Renamed
		
		With t
			'Invalid_string_refer_to_original_code
			For i = 1 To LLength(NecessaryCondition)
				ncond = LIndex(NecessaryCondition, i)
				Select Case ncond
					Case "Invalid_string_refer_to_original_code"
						If p.Technique < .MainPilot.Technique Then
							is_invalid = True
						End If
					Case "Invalid_string_refer_to_original_code"
						If .BossRank >= 0 Then
							is_invalid = True
						End If
					Case "支援"
						If my_unit Is t Then
							is_invalid = True
						End If
					Case "隣接"
						With my_unit
							If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) <> 1 Then
								is_invalid = True
							End If
						End With
					Case Else
						If InStr(ncond, "Invalid_string_refer_to_original_code") = 1 Then
							With my_unit
								If System.Math.Abs(.X - t.X) + System.Math.Abs(.Y - t.Y) > StrToLng(Mid(ncond, 5)) Then
									is_invalid = True
								End If
							End With
						End If
				End Select
				
				'Invalid_string_refer_to_original_code
				If Not my_unit Is p.Unit_Renamed Then
					my_unit.CurrentForm.MainPilot()
				End If
			Next 
			
			'Invalid_string_refer_to_original_code
			Select Case TargetType
				Case "敵", "全敵"
					If .IsConditionSatisfied("スペシャルパワー無効") Then
						is_invalid = True
					End If
			End Select
			
			'Invalid_string_refer_to_original_code
			If is_invalid Then
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code
			If Duration <> "即効" And Not as_instant Then
				.MakeSpecialPowerInEffect(Name, my_unit.MainPilot.ID)
				Exit Function
			End If
		End With
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		For i = 1 To CountEffect
			With t
				Select Case EffectType(i)
					Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						If EffectLevel(i) > 0 Then
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							GoTo NextEffect
						End If
						If .HP = .MaxHP Then
							GoTo NextEffect
						End If
						'End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
							Sleep(150)
						End If
						
						'Invalid_string_refer_to_original_code
						tmp = .HP
						If EffectType(i) = "Invalid_string_refer_to_original_code" Then
							.HP = .HP + 1000 * EffectLevel(i)
						Else
							.RecoverHP(10 * EffectLevel(i))
						End If
						
						If Not is_event Then
							If Not displayed_string Then
								If EffectLevel(i) >= 0 Then
									DrawSysString(.X, .Y, "+" & VB6.Format(.HP - tmp))
								Else
									DrawSysString(.X, .Y, VB6.Format(.HP - tmp))
								End If
							End If
							displayed_string = True
							
							If t Is SelectedUnit Then
								UpdateMessageForm(SelectedUnit)
							Else
								UpdateMessageForm(t, SelectedUnit)
							End If
							
							If EffectLevel(i) >= 0 Then
								DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							Else
								DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							End If
						End If
						
						need_update = True
						
					Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						If EffectLevel(i) > 0 Then
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							GoTo NextEffect
						End If
						If .EN = .MaxEN Then
							GoTo NextEffect
						End If
						'End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
							Sleep(150)
						End If
						
						'Invalid_string_refer_to_original_code
						tmp = .EN
						If EffectType(i) = "Invalid_string_refer_to_original_code" Then
							.EN = .EN + 10 * EffectLevel(i)
						Else
							.RecoverEN(10 * EffectLevel(i))
						End If
						
						If Not is_event Then
							If Not displayed_string Then
								If EffectLevel(i) >= 0 Then
									DrawSysString(.X, .Y, "+" & VB6.Format(.EN - tmp))
								Else
									DrawSysString(.X, .Y, VB6.Format(.EN - tmp))
								End If
							End If
							displayed_string = True
							
							If t Is SelectedUnit Then
								UpdateMessageForm(SelectedUnit)
							Else
								UpdateMessageForm(t, SelectedUnit)
							End If
							
							If EffectLevel(i) >= 0 Then
								DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							Else
								DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							End If
						End If
						
						need_update = True
						
					Case "霊力回復", "霊力増加"
						'Invalid_string_refer_to_original_code
						If EffectLevel(i) > 0 Then
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							GoTo NextEffect
						End If
						If .MainPilot.Plana = .MainPilot.MaxPlana Then
							GoTo NextEffect
						End If
						'End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
							Sleep(150)
						End If
						
						'Invalid_string_refer_to_original_code
						With .MainPilot
							tmp = .Plana
							If EffectType(i) = "霊力増加" Then
								.Plana = .Plana + 10 * EffectLevel(i)
							Else
								.Plana = .Plana + .MaxPlana * EffectLevel(i) \ 10
							End If
						End With
						
						If Not is_event Then
							If Not displayed_string Then
								If EffectLevel(i) >= 0 Then
									DrawSysString(.X, .Y, "+" & VB6.Format(.MainPilot.Plana - tmp))
								Else
									DrawSysString(.X, .Y, VB6.Format(.MainPilot.Plana - tmp))
								End If
							End If
							displayed_string = True
							
							If t Is SelectedUnit Then
								UpdateMessageForm(SelectedUnit)
							Else
								UpdateMessageForm(t, SelectedUnit)
							End If
							
							If EffectLevel(i) >= 0 Then
								DisplaySysMessage(.Nickname & "の" & .MainPilot.SkillName0("霊力") & "Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							Else
								DisplaySysMessage(.Nickname & "の" & .MainPilot.SkillName0("霊力") & "Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							End If
						End If
						
						need_update = True
						
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						If EffectLevel(i) > 0 Then
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							GoTo NextEffect
						End If
						'End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
							Sleep(150)
						End If
						
						'Invalid_string_refer_to_original_code
						n = .CountPilot + .CountSupport
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						n = n + 1
						'End If
						
						'Invalid_string_refer_to_original_code
						If n = 1 Then
							'Invalid_string_refer_to_original_code
							tmp = .MainPilot.SP
							.MainPilot.SP = .MainPilot.SP + 10 * EffectLevel(i)
							
							If Not is_event Then
								If Not displayed_string Then
									If EffectLevel(i) >= 0 Then
										DrawSysString(.X, .Y, "+" & VB6.Format(.MainPilot.SP - tmp))
									Else
										DrawSysString(.X, .Y, VB6.Format(.MainPilot.SP - tmp))
									End If
								End If
								displayed_string = True
								
								If EffectLevel(i) >= 0 Then
									DisplaySysMessage(.MainPilot.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
								Else
									DisplaySysMessage(.MainPilot.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
								End If
							End If
						Else
							'Invalid_string_refer_to_original_code
							tmp = .MainPilot.SP
							.MainPilot.SP = .MainPilot.SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
							
							If Not is_event Then
								If Not displayed_string Then
									If EffectLevel(i) >= 0 Then
										DrawSysString(.X, .Y, "+" & VB6.Format(.MainPilot.SP - tmp))
									Else
										DrawSysString(.X, .Y, VB6.Format(.MainPilot.SP - tmp))
									End If
								End If
								displayed_string = True
								
								If EffectLevel(i) >= 0 Then
									DisplaySysMessage(.MainPilot.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
								Else
									DisplaySysMessage(.MainPilot.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
								End If
							End If
							
							'Invalid_string_refer_to_original_code
							For j = 2 To .CountPilot
								With .Pilot(j)
									tmp = .SP
									.SP = .SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
									If Not is_event Then
										If .SP <> tmp Then
											If EffectLevel(i) >= 0 Then
												DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
												'Invalid_string_refer_to_original_code
												'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
											Else
												DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
												'Invalid_string_refer_to_original_code
												'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
											End If
										End If
									End If
								End With
							Next 
							
							'Invalid_string_refer_to_original_code
							For j = 1 To .CountSupport
								With .Support(j)
									tmp = .SP
									.SP = .SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
									If Not is_event Then
										If .SP <> tmp Then
											If EffectLevel(i) >= 0 Then
												DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
												'Invalid_string_refer_to_original_code
												'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
											Else
												DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
												'Invalid_string_refer_to_original_code
												'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
											End If
										End If
									End If
								End With
							Next 
							
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							With .AdditionalSupport
								tmp = .SP
								.SP = .SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
								If Not is_event Then
									If .SP <> tmp Then
										If EffectLevel(i) >= 0 Then
											DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
											'Invalid_string_refer_to_original_code
											'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
										Else
											DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
											'Invalid_string_refer_to_original_code
											'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
										End If
									End If
								End If
							End With
						End If
						'End If
						
						If Not is_event Then
							If TargetType = "全味方" Then
								Sleep(150)
							End If
						End If
						
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						For j = 1 To .CountWeapon
							If .Bullet(j) < .MaxBullet(j) Then
								Exit For
							End If
						Next 
						If j > .CountWeapon Then
							GoTo NextEffect
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
						End If
						
						'弾薬を補給
						.BulletSupply()
						
						If Not is_event Then
							DisplaySysMessage(.Nickname & "Invalid_string_refer_to_original_code")
						End If
						
					Case "状態回復"
						'Invalid_string_refer_to_original_code_
						'And .ConditionLifetime("混乱") <= 0 _
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'And .ConditionLifetime("麻痺") <= 0 _
						'And .ConditionLifetime("睡眠") <= 0 _
						'Invalid_string_refer_to_original_code_
						'And .ConditionLifetime("盲目") <= 0 _
						'And .ConditionLifetime("撹乱") <= 0 _
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code_
						'Then
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						For j = 1 To .CountCondition
							If Len(.Condition(j)) > 6 Then
								'Invalid_string_refer_to_original_code
								If Right(.Condition(j), 6) = "Invalid_string_refer_to_original_code" Then
									If .ConditionLifetime(.Condition(j)) > 0 Then
										Exit For
									End If
								End If
							End If
						Next 
						If (j > .CountCondition) Then
							GoTo NextEffect
						End If
						'End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
						End If
						
						'Invalid_string_refer_to_original_code
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.DeleteCondition("Invalid_string_refer_to_original_code")
						'End If
						If .ConditionLifetime("混乱") > 0 Then
							.DeleteCondition("混乱")
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.DeleteCondition("Invalid_string_refer_to_original_code")
						'End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.DeleteCondition("Invalid_string_refer_to_original_code")
						'End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.DeleteCondition("Invalid_string_refer_to_original_code")
						'End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.DeleteCondition("Invalid_string_refer_to_original_code")
						'End If
						If .ConditionLifetime("麻痺") > 0 Then
							.DeleteCondition("麻痺")
						End If
						If .ConditionLifetime("睡眠") > 0 Then
							.DeleteCondition("睡眠")
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.DeleteCondition("Invalid_string_refer_to_original_code")
						'End If
						If .ConditionLifetime("盲目") > 0 Then
							.DeleteCondition("盲目")
						End If
						If .ConditionLifetime("撹乱") > 0 Then
							.DeleteCondition("撹乱")
						End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.DeleteCondition("Invalid_string_refer_to_original_code")
						'End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.DeleteCondition("Invalid_string_refer_to_original_code")
						'End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						.DeleteCondition("Invalid_string_refer_to_original_code")
						'End If
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						If .ConditionLifetime("Invalid_string_refer_to_original_code") > 0 Then
							.DeleteCondition("Invalid_string_refer_to_original_code")
						End If
						For j = 1 To .CountCondition
							If Len(.Condition(j)) > 6 Then
								'Invalid_string_refer_to_original_code
								If Right(.Condition(j), 6) = "Invalid_string_refer_to_original_code" Then
									If .ConditionLifetime(.Condition(j)) > 0 Then
										.DeleteCondition(.Condition(j))
									End If
								End If
							End If
						Next 
						
						If Not is_event Then
							DisplaySysMessage(.Nickname & "Invalid_string_refer_to_original_code")
						End If
						
					Case "行動数回復"
						'Invalid_string_refer_to_original_code
						If .Action > 0 Or .MaxAction = 0 Then
							GoTo NextEffect
						End If
						
						'Invalid_string_refer_to_original_code
						.UsedAction = .UsedAction - 1
						
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						If Not is_event Then
							If frmMessage.Visible Then
								If t Is SelectedUnit Then
									UpdateMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
								
								DisplaySysMessage(.Nickname & "Invalid_string_refer_to_original_code")
							End If
						End If
						
					Case "行動数増加"
						'Invalid_string_refer_to_original_code
						If .Action > 3 Or .MaxAction = 0 Then
							GoTo NextEffect
						End If
						
						'Invalid_string_refer_to_original_code
						.UsedAction = .UsedAction - 1
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
							
							DisplaySysMessage(.Nickname & "は" & StrConv(VB6.Format(.Action), VbStrConv.Wide) & "Invalid_string_refer_to_original_code")
						End If
						
					Case "スペシャルパワー", "Invalid_string_refer_to_original_code"
						If SPDList.IsDefined(EffectData(i)) Then
							.MakeSpecialPowerInEffect(EffectData(i), my_unit.MainPilot.ID)
						Else
							ErrorMessage("Invalid_string_refer_to_original_code")
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						End If
						
					Case "気力増加"
						If .MainPilot.Personality = "機械" Then
							GoTo NextEffect
						End If
						If .MainPilot.Morale = .MainPilot.MaxMorale Then
							GoTo NextEffect
						End If
						
						'Invalid_string_refer_to_original_code
						tmp = .MainPilot.Morale
						.IncreaseMorale(10 * EffectLevel(i))
						
						If Not is_event Then
							If Not displayed_string Then
								DrawSysString(.X, .Y, "+" & VB6.Format(.MainPilot.Morale - tmp))
							End If
							displayed_string = True
						End If
						
						need_update = True
						
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						If .MainPilot.Personality = "機械" Then
							GoTo NextEffect
						End If
						If .MainPilot.Morale = .MainPilot.MinMorale Then
							GoTo NextEffect
						End If
						
						'気力を低下させる
						tmp = .MainPilot.Morale
						.IncreaseMorale(-10 * EffectLevel(i))
						
						If Not is_event Then
							If TargetType = "敵" Or TargetType = "全敵" Then
								If Not displayed_string Then
									DrawSysString(.X, .Y, VB6.Format(.MainPilot.Morale - tmp))
									displayed_string = True
								End If
							End If
						End If
						
						need_update = True
						
					Case "ランダムダメージ"
						'Invalid_string_refer_to_original_code
						If .IsConditionSatisfied("無敵") Then
							GoTo NextEffect
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
						End If
						
						'Invalid_string_refer_to_original_code
						tmp = .HP
						.HP = MaxLng(.HP - 10 * Dice(10 * EffectLevel(i)), 10)
						If TargetType = "全敵" Then
							Sleep(150)
						End If
						
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If .HP <= .MaxHP \ 4 And Not .IsConditionSatisfied("暴走") Then
							.AddCondition("暴走", -1)
						End If
						'End If
						
						If Not is_event Then
							If Not displayed_string Then
								DrawSysString(.X, .Y, VB6.Format(tmp - .HP))
							End If
							displayed_string = True
							
							If t Is SelectedUnit Then
								UpdateMessageForm(SelectedUnit)
							Else
								UpdateMessageForm(t, SelectedUnit)
							End If
							
							DisplaySysMessage(.Nickname & "に" & VB6.Format(tmp - .HP) & "Invalid_string_refer_to_original_code")
						End If
						
						need_update = True
						
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						If .IsConditionSatisfied("無敵") Then
							GoTo NextEffect
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
						End If
						
						'Invalid_string_refer_to_original_code
						tmp = .HP
						.HP = .HP - .HP * EffectLevel(i) \ 10
						If TargetType = "全敵" Then
							Sleep(150)
						End If
						
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If .HP <= .MaxHP \ 4 And Not .IsConditionSatisfied("暴走") Then
							.AddCondition("暴走", -1)
						End If
						'End If
						
						If Not is_event Then
							If Not displayed_string Then
								DrawSysString(.X, .Y, VB6.Format(tmp - .HP))
							End If
							displayed_string = True
							
							If t Is SelectedUnit Then
								UpdateMessageForm(SelectedUnit)
							Else
								UpdateMessageForm(t, SelectedUnit)
							End If
							
							If SelectedUnit Is t Then
								DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							Else
								DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
							End If
						End If
						
						need_update = True
						
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						If .IsConditionSatisfied("無敵") Then
							GoTo NextEffect
						End If
						
						If Not is_event Then
							If t Is SelectedUnit Then
								If Not frmMessage.Visible Then
									OpenMessageForm(SelectedUnit)
								Else
									UpdateMessageForm(SelectedUnit)
								End If
							Else
								If Not frmMessage.Visible Then
									OpenMessageForm(t, SelectedUnit)
								Else
									UpdateMessageForm(t, SelectedUnit)
								End If
							End If
						End If
						
						'Invalid_string_refer_to_original_code
						tmp = .EN
						.EN = .EN - .EN * EffectLevel(i) \ 10
						If TargetType = "全敵" Then
							Sleep(150)
						End If
						
						If Not displayed_string Then
							DrawSysString(.X, .Y, VB6.Format(tmp - .EN))
						End If
						displayed_string = True
						
						If t Is SelectedUnit Then
							UpdateMessageForm(SelectedUnit)
						Else
							UpdateMessageForm(t, SelectedUnit)
						End If
						
						If SelectedUnit Is t Then
							DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						Else
							DisplaySysMessage(.Nickname & "の" & Term("Invalid_string_refer_to_original_code", t) & "Invalid_string_refer_to_original_code")
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						End If
						
						need_update = True
						
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						If IsOptionDefined("ユニット情報隠蔽") Then
							If Not .IsConditionSatisfied("識別済み") Then
								.AddCondition("識別済み", -1, 0, "非表示")
								DisplayUnitStatus(t)
							End If
						End If
						If .IsConditionSatisfied("ユニット情報隠蔽") Then
							.DeleteCondition("ユニット情報隠蔽")
							DisplayUnitStatus(t)
						End If
						
						If Not frmMessage.Visible Then
							OpenMessageForm()
						End If
						DisplayMessage("Invalid_string_refer_to_original_code", Term("Invalid_string_refer_to_original_code", t, 6) & "Invalid_string_refer_to_original_code")
						& Format$(.HP) & "/" & Format$(.MaxHP) & ";" _
						Invalid_string_refer_to_original_code_
						& Format$(.EN) & "/" & Format$(.MaxEN) & ";" _
						Invalid_string_refer_to_original_code_
						& Format$(.Value \ 2) & ";" _
						Invalid_string_refer_to_original_code_
						& Format$(.ExpValue + .MainPilot.ExpValue)
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						ErrorMessage(.Name & "Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'End If
						'End If
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						If Len(msg) > 0 Then
							msg = msg & "Invalid_string_refer_to_original_code"
						End If
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						ErrorMessage(.Name & "Invalid_string_refer_to_original_code")
						'Invalid_string_refer_to_original_code_
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'End If
						'End If
						If .IsFeatureAvailable("ラーニング可能技") Then
							msg = msg & "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						End If
						If Len(msg) > 0 Then
							DisplayMessage("Invalid_string_refer_to_original_code", msg)
						End If
						
					Case "Invalid_string_refer_to_original_code"
						OpenMessageForm(t)
						.SuicidalExplosion()
						Exit Function
						
					Case "復活"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						.HP = .MaxHP
						'Invalid_string_refer_to_original_code
						
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						t = .CurrentForm
						n = 0
						n = .ConditionLifetime("残り時間")
						
						'Invalid_string_refer_to_original_code
						If n > 0 Then
							.AddCondition("残り時間", 10)
						End If
						'End If
						
						'Invalid_string_refer_to_original_code
						With t
							.FullRecover()
							.UsedAction = 0
							.StandBy(my_unit.X, my_unit.Y)
							.Rest()
							
							'Invalid_string_refer_to_original_code
							If n > 0 Then
								.DeleteCondition("残り時間")
								.AddCondition("残り時間", n)
							End If
							
							RedrawScreen()
						End With
						'End If
						
						With t
							If Not frmMessage.Visible Then
								OpenMessageForm()
							End If
							If .IsMessageDefined("復活") Then
								.PilotMessage("復活")
							End If
							If .IsAnimationDefined("復活") Then
								.PlayAnimation("復活")
							Else
								.SpecialEffect("復活")
							End If
							DisplaySysMessage(.Nickname & "Invalid_string_refer_to_original_code")
						End With
						
					Case "Invalid_string_refer_to_original_code"
						'イベントコマンドで定義されたスペシャルパワー
						'Invalid_string_refer_to_original_code
						SelectedUnitForEvent = my_unit.CurrentForm
						SelectedTargetForEvent = .CurrentForm
						'Invalid_string_refer_to_original_code
						GetValueAsString("Call(" & EffectData(i) & ")")
				End Select
			End With
NextEffect: 
		Next 
		
		'Invalid_string_refer_to_original_code
		If Not my_unit Is p.Unit_Renamed Then
			my_unit.CurrentForm.MainPilot()
		End If
		
		'Invalid_string_refer_to_original_code
		If need_update Then
			With t
				.CheckAutoHyperMode()
				.CurrentForm.CheckAutoNormalMode()
				.CurrentForm.Update()
				PList.UpdateSupportMod(.CurrentForm)
			End With
		End If
		
		Apply = displayed_string
	End Function
	
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Function CountTarget(ByRef p As Pilot) As Short
		Dim u As Unit
		Dim i As Short
		
		Select Case TargetType
			Case "Invalid_string_refer_to_original_code"
				If Effective(p, (p.Unit_Renamed)) Then
					CountTarget = 1
				End If
				
			Case "味方", "全味方"
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						GoTo NextUnit1
						'End If
						
						'Invalid_string_refer_to_original_code
						Select Case p.Party
							Case "味方", "Invalid_string_refer_to_original_code"
								If .Party <> "味方" And .Party0 <> "味方" And .Party <> "Invalid_string_refer_to_original_code" And .Party0 <> "Invalid_string_refer_to_original_code" Then
									GoTo NextUnit1
								End If
							Case Else
								If p.Party <> .Party Then
									GoTo NextUnit1
								End If
						End Select
						
						'Invalid_string_refer_to_original_code
						If Effective(p, u) Then
							CountTarget = CountTarget + 1
						End If
					End With
NextUnit1: 
				Next u
				
			Case "破壊味方"
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						GoTo NextUnit2
						'End If
						
						'Invalid_string_refer_to_original_code
						If p.Party <> .Party0 Then
							GoTo NextUnit2
						End If
						
						'Invalid_string_refer_to_original_code
						If Effective(p, u) Then
							CountTarget = CountTarget + 1
						End If
					End With
NextUnit2: 
				Next u
				
			Case "敵", "全敵"
				For	Each u In UList
					With u
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						GoTo NextUnit3
						'End If
						
						'Invalid_string_refer_to_original_code
						Select Case p.Party
							Case "味方", "Invalid_string_refer_to_original_code"
								If (.Party = "味方" And .Party0 = "味方") Or (.Party = "Invalid_string_refer_to_original_code" And .Party0 = "Invalid_string_refer_to_original_code") Then
									GoTo NextUnit3
								End If
							Case Else
								If p.Party = .Party Then
									GoTo NextUnit3
								End If
						End Select
						
						'Invalid_string_refer_to_original_code
						If Effective(p, u) Then
							CountTarget = CountTarget + 1
						End If
					End With
NextUnit3: 
				Next u
				
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				For	Each u In UList
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					'Invalid_string_refer_to_original_code
					If Effective(p, u) Then
						CountTarget = CountTarget + 1
					End If
					'End If
				Next u
		End Select
	End Function
	
	'スペシャルパワーのアニメーションを表示
	Public Function PlayAnimation() As Boolean
		Dim anime As String
		Dim animes() As String
		Dim anime_head As Short
		Dim buf As String
		Dim ret As Double
		Dim i, j As Short
		Dim expr As String
		Dim wait_time As Integer
		Dim prev_obj_color As Integer
		Dim prev_obj_fill_color As Integer
		Dim prev_obj_fill_style As Integer
		Dim prev_obj_draw_width As Integer
		Dim prev_obj_draw_option As String
		
		If Not SpecialPowerAnimation Then
			Exit Function
		End If
		
		If Animation = "-" Then
			PlayAnimation = True
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		If Animation = Name Then
			If FindNormalLabel("Invalid_string_refer_to_original_code" & Animation) = 0 Then
				'Invalid_string_refer_to_original_code_
				'And Name <> "祈り" _
				'Then
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If IsLabelDefined("Invalid_string_refer_to_original_code" & Name) Then
					HandleEvent("Invalid_string_refer_to_original_code")
					PlayAnimation = True
				End If
			End If
			Exit Function
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			PlayAnimation = True
			Exit Function
		End If
		
		'オブジェクト色等を記録しておく
		prev_obj_color = ObjColor
		prev_obj_fill_color = ObjFillColor
		prev_obj_fill_style = ObjFillStyle
		prev_obj_draw_width = ObjDrawWidth
		prev_obj_draw_option = ObjDrawOption
		
		'Invalid_string_refer_to_original_code
		ObjColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		ObjFillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		'UPGRADE_ISSUE: �萔 vbFSTransparent �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
		ObjFillStyle = vbFSTransparent
		ObjDrawWidth = 1
		ObjDrawOption = ""
		
		'Invalid_string_refer_to_original_code
		ReDim animes(1)
		anime_head = 1
		For i = 1 To Len(Animation)
			If Mid(Animation, i, 1) = ";" Then
				animes(UBound(animes)) = Mid(Animation, anime_head, i - anime_head)
				ReDim Preserve animes(UBound(animes) + 1)
				anime_head = i + 1
			End If
		Next 
		animes(UBound(animes)) = Mid(Animation, anime_head)
		
		On Error GoTo ErrorHandler
		
		For i = 1 To UBound(animes)
			anime = animes(i)
			
			'式評価
			FormatMessage(anime)
			
			'Invalid_string_refer_to_original_code
			If LCase(anime) = "clear" Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
				GoTo NextAnime
			End If
			
			'Invalid_string_refer_to_original_code
			Select Case LCase(Right(LIndex(anime, 1), 4))
				Case ".wav", ".mp3"
					'効果音
					PlayWave(anime)
					If wait_time > 0 Then
						Sleep(wait_time)
						wait_time = 0
					End If
					GoTo NextAnime
					
				Case ".bmp", ".jpg", ".gif", ".png"
					'Invalid_string_refer_to_original_code
					If wait_time > 0 Then
						anime = VB6.Format(wait_time / 100) & ";" & anime
						wait_time = 0
					End If
					DisplayBattleMessage("", anime)
					GoTo NextAnime
			End Select
			
			Select Case LCase(LIndex(anime, 1))
				Case "line", "circle", "arc", "oval", "color", "fillcolor", "fillstyle", "drawwidth"
					'Invalid_string_refer_to_original_code
					If wait_time > 0 Then
						anime = VB6.Format(wait_time / 100) & ";" & anime
						wait_time = 0
					End If
					DisplayBattleMessage("", anime)
					GoTo NextAnime
				Case "center"
					'Invalid_string_refer_to_original_code
					buf = GetValueAsString(ListIndex(anime, 2))
					If UList.IsDefined(buf) Then
						With UList.Item(buf)
							Center(.X, .Y)
							RedrawScreen()
						End With
					End If
					GoTo NextAnime
			End Select
			
			'Invalid_string_refer_to_original_code
			If IsNumeric(anime) Then
				wait_time = 100 * CDbl(anime)
				GoTo NextAnime
			End If
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			If wait_time > 0 Then
				Sleep(wait_time)
				wait_time = 0
			End If
			
			'Invalid_string_refer_to_original_code
			If Left(anime, 1) = "@" Then
				expr = Mid(ListIndex(anime, 1), 2) & "("
				'Invalid_string_refer_to_original_code
				For j = 2 To ListLength(anime)
					If j > 2 Then
						expr = expr & ","
					End If
					expr = expr & ListIndex(anime, j)
				Next 
				expr = expr & ")"
			ElseIf Not SelectedTarget Is Nothing Then 
				expr = "Invalid_string_refer_to_original_code" & anime & "(" & SelectedUnit.ID & "," & SelectedTarget.ID & ")"
			Else
				expr = "Invalid_string_refer_to_original_code" & anime & "(" & SelectedUnit.ID & ",-)"
			End If
			
			'Invalid_string_refer_to_original_code
			IsPictureDrawn = False
			
			'アニメ再生
			SaveBasePoint()
			CallFunction(expr, Expression.ValueType.StringType, buf, ret)
			RestoreBasePoint()
			
			'画像を消去しておく
			If IsPictureDrawn And LCase(buf) <> "keep" Then
				ClearPicture()
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
			End If
			
NextAnime: 
		Next 
		
		'Invalid_string_refer_to_original_code
		If wait_time > 0 Then
			Sleep(wait_time)
			wait_time = 0
		End If
		
		'Invalid_string_refer_to_original_code
		CloseMessageForm()
		
		'Invalid_string_refer_to_original_code
		ObjColor = prev_obj_color
		ObjFillColor = prev_obj_fill_color
		ObjFillStyle = prev_obj_fill_style
		ObjDrawWidth = prev_obj_draw_width
		ObjDrawOption = prev_obj_draw_option
		
		PlayAnimation = True
		Exit Function
		
ErrorHandler: 
		
		'Invalid_string_refer_to_original_code
		If Len(EventErrorMessage) > 0 Then
			DisplayEventErrorMessage(CurrentLineNum, EventErrorMessage)
			EventErrorMessage = ""
		Else
			DisplayEventErrorMessage(CurrentLineNum, "")
		End If
	End Function
End Class