Option Strict Off
Option Explicit On
Module Effect
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	
	'Invalid_string_refer_to_original_code
	Private WeaponInHand As String
	
	'Invalid_string_refer_to_original_code
	Private CurrentWeaponType As String
	
	
	'戦闘アニメ再生用サブルーチン
	Public Sub ShowAnimation(ByRef aname As String)
		Dim buf As String
		Dim ret As Double
		Dim i As Short
		Dim expr As String
		
		If Not BattleAnimation Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		expr = LIndex(aname, 1)
		If InStr(expr, "戦闘アニメ_") <> 1 Then
			expr = "戦闘アニメ_" & LIndex(aname, 1)
		End If
		If FindNormalLabel(expr) = 0 Then
			ErrorMessage("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			Exit Sub
		End If
		expr = "Call(`" & expr & "`"
		For i = 2 To LLength(aname)
			expr = expr & ",`" & LIndex(aname, i) & "`"
		Next 
		expr = expr & ")"
		
		'Invalid_string_refer_to_original_code
		IsPictureDrawn = False
		
		'Invalid_string_refer_to_original_code
		SaveMessageFormStatus()
		
		'戦闘アニメ再生
		SaveBasePoint()
		CallFunction(expr, Expression.ValueType.StringType, buf, ret)
		RestoreBasePoint()
		
		'Invalid_string_refer_to_original_code
		KeepMessageFormStatus()
		
		'画像を消去しておく
		If IsPictureDrawn And LCase(buf) <> "keep" Then
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		End If
		
		Exit Sub
		
ErrorHandler: 
		
		'Invalid_string_refer_to_original_code
		If Len(EventErrorMessage) > 0 Then
			DisplayEventErrorMessage(CurrentLineNum, EventErrorMessage)
			EventErrorMessage = ""
		Else
			DisplayEventErrorMessage(CurrentLineNum, "")
		End If
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub PrepareWeaponEffect(ByRef u As Unit, ByVal w As Short)
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		If BattleAnimation Then
			PrepareWeaponAnimation(u, w)
		Else
			PrepareWeaponSound(u, w)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub PrepareWeaponAnimation(ByRef u As Unit, ByVal w As Short)
		Dim wclass, wname, wtype As String
		Dim double_weapon As Boolean
		Dim sname, aname, cname As String
		Dim with_face_up As Boolean
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		Exit Sub
		'End If
		
		With u
			'Invalid_string_refer_to_original_code
			If .CountWeapon >= 4 And w >= .CountWeapon - 1 And .Weapon(w).Power >= 1800 And ((.Weapon(w).Bullet > 0 And .Weapon(w).Bullet <= 4) Or .Weapon(w).ENConsumption >= 35) Then
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'            with_face_up = True
			End If
			
			'Invalid_string_refer_to_original_code
			If .Data.Transportation = "空" Then
				WeaponInHand = ""
				GoTo SkipWeaponAnimation
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			WeaponInHand = ""
			GoTo SkipWeaponAnimation
			'End If
			
			wname = .WeaponNickname(w)
			wclass = .Weapon(w).Class_Renamed
		End With
		
		'Invalid_string_refer_to_original_code
		' MOD START MARGE
		'    If Not WeaponAnimation Or IsOptionDefined("武器準備アニメ非表示") Then
		If (Not WeaponAnimation And Not ExtendedAnimation) Or IsOptionDefined("武器準備アニメ非表示") Then
			' MOD END MARGE
			WeaponInHand = ""
			GoTo SkipWeaponAnimation
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		double_weapon = True
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "BeamSaber.wav"
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'And InStrNotNest(wclass, "接") = 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		GoTo SkipInfightWeapon
		'End If
		
		'Invalid_string_refer_to_original_code
		wtype = CheckWeaponType(wname, wclass)
		If wtype = "手裏剣" Then
			'Invalid_string_refer_to_original_code
			Exit Sub
		End If
		If wtype <> "" Then
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "武") > 0 Then
			'Invalid_string_refer_to_original_code
			For i = 1 To u.CountItem
				With u.Item(i)
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Or .Part = "武器") _
					'Then
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					wtype = CheckWeaponType(.Nickname, "")
					If wtype <> "" Then
						GoTo FoundWeaponType
					End If
					wtype = CheckWeaponType(.Class0, "")
					If wtype <> "" Then
						GoTo FoundWeaponType
					End If
					Exit For
				End With
			Next 
		End If
		'End With
		'Next
		GoTo SkipShootingWeapon
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStrNotNest(wclass, "接") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		GoTo SkipShootingWeapon
		'End If
		
SkipInfightWeapon: 
		
		'Invalid_string_refer_to_original_code
		If Not IsBeamWeapon(wname, wclass, cname) Then
			GoTo SkipBeamWeapon
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ハイメガ") > 0 Or InStr(wname, "バスター") > 0 Or InStr(wname, "大") > 0 Or Left(wname, 2) = "ギガ" Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "メガ") > 0 Or InStr(wname, "ハイ") > 0 Or InStr(wname, "バズーカ") > 0 Then 
			If double_weapon Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "Invalid_string_refer_to_original_code"
			End If
			If InStr(wname, "ライフル") > 0 Then
				wtype = "Invalid_string_refer_to_original_code"
			End If
		ElseIf CountAttack0(u, w) >= 4 Then 
			wtype = "マシンガン"
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "レーザーガン"
		Else
			If double_weapon Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "Invalid_string_refer_to_original_code"
			End If
		End If
		GoTo FoundWeaponType
		'End If
		
SkipBeamWeapon: 
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ロングボウ") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "クロスボウ") > 0 Or InStr(wname, "ボウガン") > 0 Then
			wtype = "クロスボウ"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "バズーカ") > 0 Then
			wtype = "バズーカ"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "ヘビーマシンガン"
		wtype = "マシンガン"
		'End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ガトリング") > 0 Then
			wtype = "ガトリング"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "ピストル"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "リボルヴァー") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 2) = "ガン" _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "ライフル"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "対戦車ライフル") > 0 Then
			wtype = "対戦車ライフル"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "対物ライフル") > 0 Then
			wtype = "対物ライフル"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "消火器") > 0 Then
			wtype = "消火器"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "放水") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
SkipShootingWeapon: 
		
		'Invalid_string_refer_to_original_code
		WeaponInHand = ""
		GoTo SkipWeaponAnimation
		
FoundWeaponType: 
		
		'Invalid_string_refer_to_original_code
		WeaponInHand = wtype
		
		'Invalid_string_refer_to_original_code
		aname = wtype & "準備"
		
		'色
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			aname = aname & " ピンク"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			aname = aname & " グリーン"
		ElseIf InStr(wname, "レーザー") > 0 Then 
			aname = aname & " ブルー"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			aname = aname & " イエロー"
		End If
		'End If
		
		'効果音
		If Len(sname) > 0 Then
			aname = aname & " " & sname
		End If
		
		'Invalid_string_refer_to_original_code
		If double_weapon Then
			aname = aname & "Invalid_string_refer_to_original_code"
		End If
		
		'準備アニメ表示
		ShowAnimation(aname)
		
SkipWeaponAnimation: 
		
		'Invalid_string_refer_to_original_code
		
		If with_face_up Then
			'Invalid_string_refer_to_original_code
			aname = "Invalid_string_refer_to_original_code"
			
			'Invalid_string_refer_to_original_code
			If InStrNotNest(wclass, "サ") > 0 Then
				aname = aname & " 衝撃"
			End If
			
			'Invalid_string_refer_to_original_code
			ShowAnimation(aname)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Function CheckWeaponType(ByRef wname As String, ByRef wclass As String) As String
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "レーザー") > 0 _
		'Or InStr(wname, "ブラスター") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "剣") > 0 _
		'Or InStr(wname, "刀") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		'UPGRADE_WARNING: CheckWeaponType �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		'End If
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		Else
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		End If
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ダガー") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 3) = "スロー" Or Right$(wname, 3) = "スロウ" _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		'End If
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "大剣"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "手裏剣") > 0 Then
			CheckWeaponType = "手裏剣"
			Exit Function
		End If
		
		If Right(wname, 1) = "剣" And (Len(wname) <= 3 Or Right(wname, 2) = "の剣") Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			CheckWeaponType = "黒剣"
		Else
			CheckWeaponType = "剣"
		End If
		Exit Function
		'End If
		
		If InStr(wname, "ソードブレイカー") > 0 Then
			CheckWeaponType = "ソードブレイカー"
			Exit Function
		End If
		
		If InStr(wname, "レイピア") > 0 Then
			CheckWeaponType = "レイピア"
			Exit Function
		End If
		
		If InStr(wname, "シミター") > 0 Or InStr(wname, "サーベル") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "シミター"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "ナギナタ"
		Exit Function
		'End If
		
		If InStr(wname, "竹刀") > 0 Then
			CheckWeaponType = "竹刀"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "小太刀") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If wname = "刀" Or wname = "日本刀" Or InStr(wname, "太刀") > 0 Then
			CheckWeaponType = "日本刀"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "十手") > 0 Then
			CheckWeaponType = "十手"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "斧") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "バトル") > 0 _
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		Else
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		End If
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "スパナ") > 0 Then
			CheckWeaponType = "スパナ"
			Exit Function
		End If
		
		If InStr(wname, "メイス") > 0 Then
			CheckWeaponType = "メイス"
			Exit Function
		End If
		
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		'End If
		Exit Function
		'End If
		
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		Else
			CheckWeaponType = "Invalid_string_refer_to_original_code"
		End If
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If Right(wname, 3) = "モール" Then
			CheckWeaponType = "モール"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "鞭"
		Exit Function
		'End If
		
		If wname = "サイ" Then
			CheckWeaponType = "サイ"
			Exit Function
		End If
		
		If InStr(wname, "トンファー") > 0 Then
			CheckWeaponType = "トンファー"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "クロー"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		
		If InStr(wname, "モーニングスター") > 0 Then
			CheckWeaponType = "モーニングスター"
			Exit Function
		End If
		
		If InStr(wname, "フレイル") > 0 Then
			CheckWeaponType = "フレイル"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "ヌンチャク") > 0 Then
			CheckWeaponType = "ヌンチャク"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "チェーン") > 0 Then
			CheckWeaponType = "チェーン"
			Exit Function
		End If
		
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "チャクラム") > 0 Then
			CheckWeaponType = "チャクラム"
			Exit Function
		End If
		
		If InStr(wname, "ソーサー") > 0 Then
			CheckWeaponType = "ソーサー"
			Exit Function
		End If
		
		If InStr(wname, "クナイ") > 0 Then
			CheckWeaponType = "クナイ"
			Exit Function
		End If
		
		If InStr(wname, "石") > 0 Or InStr(wname, "礫") > 0 Then
			CheckWeaponType = "石"
			Exit Function
		End If
		
		If InStr(wname, "岩") > 0 Then
			CheckWeaponType = "岩"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "手榴弾") > 0 Then
			CheckWeaponType = "手榴弾"
			Exit Function
		End If
		
		If InStr(wname, "ポテトスマッシャー") > 0 Then
			CheckWeaponType = "ポテトスマッシャー"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			If InStr(wname, "投げ") > 0 Then
				CheckWeaponType = "Invalid_string_refer_to_original_code"
				Exit Function
			End If
		End If
		
		If InStr(wname, "火炎瓶") > 0 Then
			CheckWeaponType = "火炎瓶"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "手錠") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "札") > 0 Then
			CheckWeaponType = "お札"
			Exit Function
		End If
		
		
		If InStr(wname, "リボン") > 0 Then
			CheckWeaponType = "リボン"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		
		If InStr(wname, "カタログ") > 0 Then
			CheckWeaponType = "カタログ"
			Exit Function
		End If
		
		If InStr(wname, "フライパン") > 0 Then
			CheckWeaponType = "フライパン"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "唐傘") > 0 Then
			CheckWeaponType = "唐傘"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If InStr(wname, "Invalid_string_refer_to_original_code") = 0 Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "ギター") > 0 Then
			CheckWeaponType = "ギター"
			Exit Function
		End If
		
		If InStr(wname, "ハリセン") > 0 Then
			CheckWeaponType = "ハリセン"
			Exit Function
		End If
		
		If wname = "Invalid_string_refer_to_original_code" Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ジャベリン") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "スピア") > 0 Then
			CheckWeaponType = "スピア"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CheckWeaponType = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		If InStr(wname, "ランス") > 0 Or InStr(wname, "ランサー") > 0 Then
			CheckWeaponType = "ランス"
			Exit Function
		End If
		
		If InStr(wname, "パイク") > 0 Then
			CheckWeaponType = "ランス"
			Exit Function
		End If
		
		If InStr(wname, "エストック") > 0 Then
			CheckWeaponType = "エストック"
			Exit Function
		End If
		
		If wname = "Invalid_string_refer_to_original_code" Then
			CheckWeaponType = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		If InStr(wname, "ドリル") > 0 Then
			CheckWeaponType = "ドリル"
			Exit Function
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub PrepareWeaponSound(ByRef u As Unit, ByVal w As Short)
		Dim wname, wclass As String
		
		'フラグをクリア
		IsWavePlayed = False
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "レーザー") > 0 _
		'Or InStr(wname, "ブラスター") > 0 _
		'Or InStr(wname, "高周波") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or wname = "ランサー" _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("BeamSaber.wav")
		'End If
		'End If
		
		'フラグをクリア
		IsWavePlayed = False
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub AttackEffect(ByRef u As Unit, ByVal w As Short)
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		If BattleAnimation Then
			AttackAnimation(u, w)
		Else
			AttackSound(u, w)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub AttackAnimation(ByRef u As Unit, ByVal w As Short)
		Dim wtype, wname, wclass, wtype0 As String
		Dim cname, aname, bmpname, cname0 As String
		Dim sname, sname0 As String
		Dim attack_times As Short
		Dim double_weapon As Boolean
		Dim double_attack As Boolean
		Dim combo_attack As Boolean
		Dim is_handy_weapon As Boolean
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ShowAnimation("Invalid_string_refer_to_original_code")
		Exit Sub
		'End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		double_weapon = True
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "連") > 0 Or InStrNotNest(wclass, "連") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		double_attack = True
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		combo_attack = True
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'And InStrNotNest(wclass, "接") = 0 _
		'And InStrNotNest(wclass, "格") = 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		GoTo SkipInfightWeapon
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 3) = "スロー" Or Right$(wname, 3) = "スロウ" _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		GoTo SkipInfightWeapon
		'End If
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "突撃") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "チャージ") > 0 Then
			Select Case WeaponInHand
				Case ""
					'Invalid_string_refer_to_original_code
				Case Else
					wtype = WeaponInHand & "突撃"
					GoTo FoundWeaponType
			End Select
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 5) = "ストライク" _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ナックル") > 0 Or InStr(wname, "ブロー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "殴") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "シミター") > 0 Or InStr(wname, "サーベル") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "クロー") > 0 Or InStr(wname, "爪") > 0 _
		'Or InStr(wname, "ひっかき") > 0 _
		'Or InStr(wname, "アーム") > 0 _
		'Or Right$(wname, 1) = "尾" _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "回転") > 0 Then 
			wtype = "白兵回転"
		ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
			wtype = "振り上げ"
		Else
			wtype = "白兵武器"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "回転") > 0 Then 
			wtype = "白兵回転"
		ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
			wtype = "振り上げ"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'大きく振りまわす武器
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "モーニングスター") > 0 Then
			wtype = "モーニングスター"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "フレイル") > 0 Then
			wtype = "フレイル"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "チェーン") > 0 And InStr(wname, "チェーンソー") = 0 Then
			wtype = "チェーン"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ヌンチャク") > 0 Then
			wtype = "ヌンチャク"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'突き刺す武器
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ランス") > 0 Or InStr(wname, "ランサー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ジャベリン") > 0 _
		'Or InStr(wname, "レイピア") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'特殊な格闘武器
		
		If InStr(wname, "ドリル") > 0 Then
			wtype = "ドリル"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "チェーンソー") > 0 Then
			wtype = "チェーンソー"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "武") > 0 Then
			'Invalid_string_refer_to_original_code
			For i = 1 To u.CountItem
				With u.Item(i)
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Or .Part = "武器") _
					'Then
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					wtype = CheckWeaponType(.Nickname, "")
					If wtype = "" Then
						wtype = CheckWeaponType(.Class0, "")
					End If
					Exit For
				End With
			Next 
		End If
		'End With
		'Next
		Select Case wtype
			Case "スピア", "ランス", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'"エストック"
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If combo_attack Then
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf double_attack Then 
					wtype = "Invalid_string_refer_to_original_code"
				Else
					wtype = "Invalid_string_refer_to_original_code"
				End If
			Case Else
				If combo_attack Then
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf double_attack Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStr(wname, "回転") > 0 Then 
					wtype = "白兵回転"
				ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
					wtype = "振り上げ"
				Else
					wtype = "白兵武器"
				End If
		End Select
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'And InStrNotNest(wclass, "接") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
SkipInfightWeapon: 
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		GoTo SkipThrowingWeapon
		'End If
		
		'投擲武器
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "クナイ") > 0 Or InStr(wname, "苦無") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "石") > 0 Or InStr(wname, "礫") > 0 Then
			wtype = "石"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "岩") > 0 Then
			wtype = "岩"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			If InStr(wname, "投げ") > 0 Then
				wtype = "Invalid_string_refer_to_original_code"
				GoTo FoundWeaponType
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "斧") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "バトル") > 0 _
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "大鎌投擲"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "鎌投擲"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "チャクラム") > 0 Then
			wtype = "チャクラム"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "手裏剣") > 0 Then
			wtype = "手裏剣"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "手榴弾") > 0 Then
			wtype = "手榴弾"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "火炎瓶") > 0 Then
			wtype = "火炎瓶"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "手錠") > 0 Then
			wtype = "手錠"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "札") > 0 Then
			wtype = "お札"
			GoTo FoundWeaponType
		End If
		
		'弓矢
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ロングボウ") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "弓矢"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "矢") > 0 Or InStr(wname, "アロー") > 0 Then
			If CountAttack0(u, w) > 1 Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "矢"
			End If
			GoTo FoundWeaponType
		End If
		
		'遠距離系の格闘武器
		
		'振る武器
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "白兵武器"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'大きく振りまわす武器
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "チェーン") > 0 Then
			wtype = "チェーン"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
SkipThrowingWeapon: 
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		is_handy_weapon = True
		
		'Invalid_string_refer_to_original_code
		
		If IsBeamWeapon(wname, wclass, cname) Then
			wtype = "Invalid_string_refer_to_original_code"
			
			'Invalid_string_refer_to_original_code
			GoTo SkipNormalHandWeapon
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "クロスボウ") > 0 Or InStr(wname, "ボウガン") > 0 Then
			wtype = "クロスボウ"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "バズーカ") > 0 Then
			wtype = "バズーカ"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "対戦車ライフル") > 0 Then
			wtype = "対戦車ライフル"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "対物ライフル") > 0 Then
			wtype = "対物ライフル"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "ピストル"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "リボルヴァー") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "ライフル"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "ヘビーマシンガン"
		wtype = "マシンガン"
		'End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ガトリング") > 0 Then
			wtype = "ガトリング"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "レールガン") > 0 Or InStr(wname, "リニアガン") > 0 Then
			PlayWave("Thunder.wav")
			Sleep(300)
			wtype = "キャノン砲"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		If Right(wname, 2) = "ガン" Then
			wtype = "ライフル"
			GoTo FoundWeaponType
		End If
		
		GoTo SkipHandWeapon
		
SkipNormalHandWeapon: 
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ハイメガ") > 0 Or InStr(wname, "バスター") > 0 Or InStr(wname, "大") > 0 Or Left(wname, 2) = "ギガ" Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "メガ") > 0 Or InStr(wname, "ハイ") > 0 Or InStr(wname, "バズーカ") > 0 Then 
			If double_weapon Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "Invalid_string_refer_to_original_code"
			End If
			If InStr(wname, "ライフル") > 0 Then
				bmpname = "Weapon\EFFECT_BusterRifle01.bmp"
			End If
		ElseIf CountAttack0(u, w) >= 4 Then 
			wtype = "レーザーマシンガン"
			bmpname = "Weapon\EFFECT_Rifle01.bmp"
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "レーザーガン"
		Else
			If double_weapon Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "Invalid_string_refer_to_original_code"
			End If
		End If
		
		If wtype = "バスター" Then
			wtype0 = "粒子集中"
		End If
		
		GoTo FoundWeaponType
		'End If
		
SkipHandWeapon: 
		
		'Invalid_string_refer_to_original_code
		is_handy_weapon = False
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "ミサイル") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "ミサイル"
			
			If InStr(wname, "ドリル") > 0 Then
				wtype = "ドリルミサイル"
				GoTo FoundWeaponType
			End If
			
			attack_times = CountAttack0(u, w)
			
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "対艦") > 0 _
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
			attack_times = 1
		ElseIf InStr(wname, "小型") > 0 Then 
			wtype = "小型ミサイル"
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "マイクロ") > 0 Or InStr(wname, "スプレー") > 0 _
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "小型ミサイル"
			attack_times = 6
		End If
		
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		
		attack_times = CountAttack0(u, w)
		
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		
		bmpname = "Bullet\EFFECT_BazookaBullet01.bmp"
		attack_times = CountAttack0(u, w)
		
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			If u.Weapon(w).MaxRange = 1 Then
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "Invalid_string_refer_to_original_code"
				attack_times = CountAttack0(u, w)
			End If
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "光子魚雷") > 0 Then
			wtype = "光子魚雷"
			GoTo FoundWeaponType
		End If
		
		'(怪光線系)
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "消火") > 0 Then
			wtype = "消火器"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or Right(wname, 1) = "液" Then
			wtype = "Invalid_string_refer_to_original_code"
			sname = "Bow.wav"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			bmpname = "Bullet\EFFECT_Venom01.bmp"
		Else
			bmpname = "Bullet\EFFECT_WaterShot01.bmp"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "縮退") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "重力弾"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "落雷") > 0 Or Right(wname, 2) = "稲妻" Then
			wtype = "落雷"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "雷") > 0 Or InStr(wname, "ライトニング") > 0 Or InStr(wname, "サンダー") > 0 Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			If u.Weapon(w).MaxRange = 1 Then
				wtype = "Invalid_string_refer_to_original_code"
				sname = "Thunder.wav"
			Else
				wtype = "落雷"
			End If
			GoTo FoundWeaponType
		End If
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		sname = "Thunder.wav"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "エネルギー弾") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			sname = "Beam.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "泡") > 0 Or InStr(wname, "バブル") > 0 Then
			wtype = "泡"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ソニック") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "叫び") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "音波"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "音符"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		If CountAttack0(u, w) > 1 Then
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "津波") > 0 Or InStr(wname, "ダイダル") > 0 Then
			wtype = "津波"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "流星"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "隕石") > 0 Then
			wtype = "隕石"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "竜巻"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "つらら") > 0 Then
			wtype = "氷弾"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "つぶて") > 0 Then
			wtype = "岩弾"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "吹雪"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "強風"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "風") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "風"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 2) = "ガス" Or Right$(wname, 1) = "霧" _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		cname = "Invalid_string_refer_to_original_code"
		'End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "火炎弾") > 0 Then
			wtype = "火炎弾"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		sname = "AntiShipMissile.wav"
		GoTo FoundWeaponType
		'End If
		
		If Right(wname, 5) = "ファイアー" Or Right(wname, 5) = "ファイヤー" Or Right(wname, 4) = "ファイア" Or Right(wname, 4) = "ファイヤ" Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
			sname = "AntiShipMissile.wav"
		End If
		GoTo FoundWeaponType
		'End If
		'End If
		
		If InStr(wname, "息") > 0 Or Right(wname, 3) = "ブレス" Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
			sname = "Breath.wav"
			
			Select Case SpellColor(wname, wclass)
				Case "赤", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					cname = SpellColor(wname, wclass)
			End Select
			
			GoTo FoundWeaponType
		End If
		'End If
		
		If InStr(wname, "エネルギー波") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			sname = "Beam.wav"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "衝撃") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			cname = "白"
			sname = "Bazooka.wav"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "気弾") > 0 Then
			wtype = "気弾"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "気斬"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'        cname = SpellColor(wname, wclass)
		wtype = "Invalid_string_refer_to_original_code"
		sname = "Whiz.wav"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If wtype = "Invalid_string_refer_to_original_code" Then
			If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
				wtype = "Invalid_string_refer_to_original_code"
				GoTo FoundWeaponType
			End If
			
			If InStr(wname, "ハイメガ") > 0 Or InStr(wname, "バスター") > 0 Or InStr(wname, "大") > 0 Or Left(wname, 2) = "ギガ" Then
				wtype = "Invalid_string_refer_to_original_code"
			ElseIf InStr(wname, "メガ") > 0 Or InStr(wname, "ハイ") > 0 Then 
				wtype = "Invalid_string_refer_to_original_code"
			ElseIf CountAttack0(u, w) >= 4 Or InStr(wname, "対空") > 0 Then 
				wtype = "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				wtype = "Invalid_string_refer_to_original_code"
			ElseIf InStr(wname, "ランチャー") > 0 Or InStr(wname, "キャノン") > 0 Or InStr(wname, "カノン") > 0 Or InStr(wname, "砲") > 0 Then 
				wtype = "Invalid_string_refer_to_original_code"
			Else
				wtype = "小ビーム"
			End If
			
			If wtype = "Invalid_string_refer_to_original_code" Then
				wtype0 = "粒子集中"
			End If
			
			Select Case wtype
				Case "小ビーム", "Invalid_string_refer_to_original_code"
					If double_weapon Then
						wtype = "Invalid_string_refer_to_original_code" & wtype
					End If
			End Select
			
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
		End If
		
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "バルカン") > 0 Then
			wtype = "バルカン"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "機銃") > 0 Or InStr(wname, "機関砲") > 0 Then
			wtype = "機関砲"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "チェーンガン") > 0 Or InStr(wname, "ガンランチャー") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "マシンキャノン") > 0 Or InStr(wname, "オートキャノン") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "重機関砲"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ベアリング") > 0 Or InStr(wname, "クレイモア") > 0 Then
			wtype = "ベアリング"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		If InStr(wname, "砲") > 0 Or InStr(wname, "キャノン") > 0 Or InStr(wname, "カノン") > 0 Or InStr(wname, "弾") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			PlayWave("Thunder.wav")
			Sleep(300)
		End If
		
		wtype = "キャノン砲"
		
		attack_times = CountAttack0(u, w)
		
		GoTo FoundWeaponType
		'End If
		
SkipShootingWeapon: 
		
		'Invalid_string_refer_to_original_code
		wtype = "Invalid_string_refer_to_original_code"
		
FoundWeaponType: 
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		Select Case wtype
			Case "Invalid_string_refer_to_original_code"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "小ビーム"
			Case "レーザーマシンガン"
				wtype = "Invalid_string_refer_to_original_code"
			Case "レーザーガン"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code", "マシンガン"
				wtype = "機関砲"
			Case "ヘビーマシンガン"
				wtype = "重機関砲"
			Case "ガトリング"
				wtype = "Invalid_string_refer_to_original_code"
			Case "Invalid_string_refer_to_original_code"
				wtype = "ベアリング"
			Case Else
				'手持ち武器の画像を空にする
				bmpname = "-.bmp"
		End Select
		'End If
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			'Invalid_string_refer_to_original_code
			Select Case wtype
				Case "矢", "小型ミサイル", "ミサイル", "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'"強風", "竜巻", "津波", "泡", _
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					wtype = "Invalid_string_refer_to_original_code" & wtype
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					wtype = "Invalid_string_refer_to_original_code"
				Case "Invalid_string_refer_to_original_code", "Invalid_string_refer_to_original_code"
					wtype = "Invalid_string_refer_to_original_code"
				Case "Invalid_string_refer_to_original_code"
					wtype = "Invalid_string_refer_to_original_code"
				Case "重力弾"
					wtype = "Invalid_string_refer_to_original_code"
				Case Else
					If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
						wtype = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						wtype = "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code_
						'Or InStr(wname, "クエイク") > 0 _
						'Then
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						wtype = "Invalid_string_refer_to_original_code"
						sname = " Explode(Far).wav"
					ElseIf InStr(wname, "核") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then 
						wtype = "Invalid_string_refer_to_original_code"
					End If
			End Select
		End If
		
		
		'Invalid_string_refer_to_original_code
		CurrentWeaponType = wtype
		
		'Invalid_string_refer_to_original_code
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "赤") > 0 Then
			cname = "赤"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "白"
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(wtype0) > 0 Then
			'Invalid_string_refer_to_original_code
			aname = wtype0 & "準備"
			
			'色
			If Len(cname0) > 0 Then
				aname = aname & " " & cname0
			ElseIf Len(cname) > 0 Then 
				aname = aname & " " & cname
			End If
			
			'効果音
			If Len(sname0) > 0 Then
				aname = aname & " " & sname0
			End If
			
			'戦闘アニメ表示
			ShowAnimation(aname)
		End If
		
		'Invalid_string_refer_to_original_code
		aname = wtype & "Invalid_string_refer_to_original_code"
		
		'Invalid_string_refer_to_original_code
		If attack_times > 0 Then
			aname = aname & " " & VB6.Format(attack_times)
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(bmpname) > 0 Then
			aname = aname & " " & bmpname
		End If
		
		'色
		If Len(cname) > 0 Then
			aname = aname & " " & cname
		End If
		
		'効果音
		If Len(sname) > 0 Then
			aname = aname & " " & sname
		End If
		
		'Invalid_string_refer_to_original_code
		ShowAnimation(aname)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub AttackSound(ByRef u As Unit, ByVal w As Short)
		Dim wname, wclass As String
		Dim sname As String
		Dim num As Short
		Dim i As Short
		
		'フラグをクリア
		IsWavePlayed = False
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Or u.IsWeaponClassifiedAs(w, "接") _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		Exit Sub
		'End If
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			Exit Sub
		End If
		If InStrNotNest(wclass, "武") > 0 Then
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				Exit Sub
			End If
		End If
		
		'効果音の再生回数
		num = CountAttack(u, w)
		
		'Invalid_string_refer_to_original_code
		If InStr(wname, "主砲") > 0 Or InStr(wname, "副砲") > 0 Then
			If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
				sname = "Beam.wav"
			Else
				sname = "Cannon.wav"
			End If
		ElseIf InStr(wname, "対空砲") > 0 Then 
			If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
				sname = "Beam.wav"
				num = 4
			Else
				sname = "MachineCannon.wav"
			End If
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "衝撃波") > 0 Or InStr(wname, "電磁波") > 0 _
			'Or InStr(wname, "電波") > 0 Or InStr(wname, "音波") > 0 _
			'Or InStr(wname, "磁力") > 0 _
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "エネルギー") > 0 _
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			sname = "LaserGun.wav"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "イオン") > 0 Or InStr(wname, "プロミネンス") > 0 _
			'Or InStr(wname, "ハイドロ") > 0 Or InStr(wname, "インパルス") > 0 _
			'Or InStr(wname, "フレイム") > 0 Or InStr(wname, "サンシャイン") > 0 _
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			sname = "Beam.wav"
		ElseIf InStr(wname, "シューター") > 0 Then 
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			sname = "Missile.wav"
		Else
			sname = "Beam.wav"
		End If
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "LaserGun.wav"
		End If
		If InStr(wname, "バルカン") > 0 Or InStr(wname, "マシンガン") > 0 Then
			num = 4
		End If
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "機銃") > 0 _
		'Or InStr(wname, "マシンガン") > 0 _
		'Or InStr(wname, "アサルトライフル") > 0 _
		'Or InStr(wname, "チェーンライフル") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "マウラー砲") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "LaserGun.wav"
		Else
			sname = "MachineGun.wav"
		End If
		num = 1
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "LaserGun.wav"
		Else
			sname = "MachineCannon.wav"
		End If
		num = 1
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "バルカン") > 0 _
		'Or InStr(wname, "ガトリング") > 0 _
		'Or InStr(wname, "ハンドレールガン") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "LaserGun.wav"
		Else
			sname = "GunPod.wav"
		End If
		num = 1
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Thunder.wav")
		Sleep(300)
		PlayWave("Cannon.wav")
		For i = 2 To num
			Sleep(130)
			PlayWave("Cannon.wav")
		Next 
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "Rifle.wav"
		End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ナパーム") > 0 _
		'Or InStr(wname, "クレイモア") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "無反動砲") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "Bazooka.wav"
		End If
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		sname = "FastGun.wav"
		num = 1
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "アロー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ボウガン") > 0 _
		'Or InStr(wname, "ロングボウ") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "髪") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Bow.wav"
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		sname = "Swing.wav"
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		sname = "Bomb.wav"
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		sname = "Explode.wav"
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		sname = "MicroMissile.wav"
		num = 1
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		sname = "MicroMissile.wav"
		num = 1
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ランチャー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "Missile.wav"
		End If
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "Cannon.wav"
		End If
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			sname = "Beam.wav"
		Else
			sname = "Gun.wav"
		End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "スライサー") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Saber.wav"
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Shock(Low).wav"
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ハリケーン") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "サイクロン") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "竜巻") > 0 _
		'Or InStr(wname, "渦巻") > 0 _
		'Or InStr(wname, "台風") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "吹雪") > 0 _
		'Or InStr(wname, "フリーザー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Storm.wav"
		num = 1
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		sname = "Swing.wav"
		num = 5
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "稲妻") > 0 _
		'Or InStr(wname, "放電") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "雷") > 0 _
		'Or InStrNotNest(wclass, "雷") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Thunder.wav"
		num = 1
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "AntiShipMissile.wav"
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Fire.wav"
		num = 1
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "サイコキネシス") > 0 _
		'Or InStr(wname, "糸") > 0 _
		'Or InStr(wname, "アンカー") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Whiz.wav"
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		sname = "Bubble.wav"
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		sname = "Shower.wav"
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		If InStrNotNest(wclass, "火") > 0 Then
			sname = "AntiShipMissile.wav"
		ElseIf InStrNotNest(wclass, "冷") > 0 Then 
			sname = "Storm.wav"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			sname = "GunPod.wav"
		ElseIf InStrNotNest(wclass, "水") > 0 Then 
			sname = "Hide.wav"
		Else
			sname = "AntiShipMissile.wav"
		End If
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		sname = "MultipleRocketLauncher(Light).wav"
		num = 1
		'UPGRADE_WARNING: AttackSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		'Invalid_string_refer_to_original_code
		sname = "Beam.wav"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		sname = "Gun.wav"
		'End If
		
		'Invalid_string_refer_to_original_code
		If sname = "" Then
			'フラグをクリア
			IsWavePlayed = False
			Exit Sub
		End If
		
		For i = 1 To num
			PlayWave(sname)
			
			'ウェイトを入れる
			Sleep(130)
			If sname = "Swing.wav" Then
				Sleep(150)
			End If
		Next 
		
		'フラグをクリア
		IsWavePlayed = False
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub HitEffect(ByRef u As Unit, ByRef w As Short, ByRef t As Unit, Optional ByVal hit_count As Short = 0)
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		If BattleAnimation Then
			HitAnimation(u, w, t, hit_count)
		Else
			HitSound(u, w, t, hit_count)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub HitAnimation(ByRef u As Unit, ByVal w As Short, ByRef t As Unit, ByVal hit_count As Short)
		Dim wtype, wname, wclass, wtype0 As String
		Dim cname, aname, sname As String
		Dim attack_times As Short
		Dim double_weapon As Boolean
		Dim double_attack As Boolean
		Dim combo_attack As Boolean
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ShowAnimation("ダメージ命中")
		Exit Sub
		'End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			'Invalid_string_refer_to_original_code
			If u.WeaponPower(w, "") = 0 Then
				Exit Sub
			End If
			
			wtype = "ダメージ"
			
			If IsBeamWeapon(wname, wclass, cname) Or InStr(wname, "ミサイル") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				sname = "Explode.wav"
			End If
			
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		double_weapon = True
		'End If
		
		'Invalid_string_refer_to_original_code
		If InStr(wname, "ダブル") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "コンビネーション") > 0 Or InStr(wname, "連") > 0 Or InStrNotNest(wclass, "連") > 0 Then
			double_attack = True
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		combo_attack = True
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'And InStrNotNest(wclass, "接") = 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		GoTo SkipInfightWeapon
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "突撃") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "チャージ") > 0 Then
			Select Case WeaponInHand
				Case ""
					'Invalid_string_refer_to_original_code
				Case Else
					wtype = WeaponInHand & "突撃"
					GoTo FoundWeaponType
			End Select
		End If
		
		'打撃系
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 5) = "ストライク" _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'通常打撃
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ビンタ") > 0 _
		'Or InStr(wname, "殴") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "トンファー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "モーニングスター") > 0 Or InStr(wname, "フレイル") > 0 _
		'Or InStr(wname, "ヌンチャク") > 0 Or InStr(wname, "三節根") > 0 _
		'Or (InStr(wname, "チェーン") > 0 And InStr(wname, "チェーンソー") = 0) _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "竹刀") > 0 _
		'Or InStr(wname, "ハリセン") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "打撃"
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "竹刀") > 0 _
		'Or InStr(wname, "ハリセン") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Whip.wav"
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ビンタ") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Slap.wav"
		'End If
		
		GoTo FoundWeaponType
		'End If
		
		'強打撃
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "モール") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Bazooka.wav")
		'End If
		
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "バンカー") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		sname = "Crash.wav"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "バンカー") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			sname = "Bazooka.wav"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		sname = "Crash.wav"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "打撃"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "レーザー") > 0 _
		'Or InStr(wname, "ブラスター") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		cname = "グリーン"
		'UPGRADE_WARNING: HitAnimation �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		cname = "ブルー"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		cname = "イエロー"
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "剣") > 0 _
		'Or InStr(wname, "刀") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		wtype = "Invalid_string_refer_to_original_code"
		'End If
		
		If double_weapon Then
			wtype = "ダブル" & wtype
		ElseIf InStr(wname, "回転") > 0 Then 
			wtype = "回転" & wtype
		End If
		
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ダガー") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ナギナタ") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "シミター") > 0 Or InStr(wname, "サーベル") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "刀") > 0 Or InStr(wname, "斬") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_weapon Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStrNotNest(wclass, "火") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStrNotNest(wclass, "雷") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStrNotNest(wclass, "冷") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "唐竹割") > 0 Or InStr(wname, "縦") > 0 Then 
			wtype = "唐竹割"
		ElseIf InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "横") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStr(wname, "斬") > 0 Then 
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
			wtype = "斬り上げ"
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "死") > 0 _
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'刺突系
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ランス") > 0 Or InStr(wname, "ランサー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ジャベリン") > 0 _
		'Or InStr(wname, "レイピア") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "爪") > 0 Or InStr(wname, "クロー") > 0 Or InStr(wname, "ひっかき") > 0 Then
			If InStr(wname, "アーム") > 0 Then
				wtype = "打撃"
				sname = "Crash.wav"
			Else
				wtype = "Invalid_string_refer_to_original_code"
			End If
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "噛み付き"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ドリル") > 0 Then
			wtype = "ドリル"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "リボン") > 0 Then
			wtype = "リボン"
			GoTo FoundWeaponType
		End If
		
		'掴み系
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "投げ") > 0 Or wname = "返し" Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ブリーカー") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "折り") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ジャイアントスイング") > 0 Then
			wtype = "ジャイアントスイング"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "ブレーンバスター") > 0 Then
			wtype = "ブレーンバスター"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "武") > 0 Then
			'Invalid_string_refer_to_original_code
			For i = 1 To u.CountItem
				With u.Item(i)
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Or .Part = "武器") _
					'Then
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					wtype = CheckWeaponType(.Nickname, "")
					If wtype = "" Then
						wtype = CheckWeaponType(.Class0, "")
					End If
					Exit For
				End With
			Next 
		End If
		'End With
		'Next
		Select Case wtype
			Case "スピア", "ランス", "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code_
				'"エストック"
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If combo_attack Then
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf double_attack Then 
					wtype = "Invalid_string_refer_to_original_code"
				Else
					wtype = "Invalid_string_refer_to_original_code"
				End If
			Case Else
				If combo_attack Then
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf double_weapon Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf double_attack Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStrNotNest(wclass, "火") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStrNotNest(wclass, "雷") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStrNotNest(wclass, "冷") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then 
					wtype = "斬り上げ"
				Else
					wtype = "Invalid_string_refer_to_original_code"
				End If
		End Select
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'And InStrNotNest(wclass, "接") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If combo_attack Then
			wtype = "Invalid_string_refer_to_original_code"
		ElseIf double_attack Then 
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "打撃"
		End If
		GoTo FoundWeaponType
		'End If
		
SkipInfightWeapon: 
		
		'Invalid_string_refer_to_original_code
		
		If InStr(wname, "斧") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "ソーサー") > 0 Or InStr(wname, "チャクラム") > 0 Then
			wtype = "ダメージ"
			sname = "Saber.wav"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "石") > 0 Or InStr(wname, "礫") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "打撃"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "手裏剣") > 0 Or InStr(wname, "クナイ") > 0 _
		'Or InStr(wname, "苦無") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		
		If IsBeamWeapon(wname, wclass, cname) Then
			wtype = "Invalid_string_refer_to_original_code"
		End If
		
		If wtype = "Invalid_string_refer_to_original_code" Then
			'Invalid_string_refer_to_original_code
			GoTo SkipNormalWeapon
		End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ロングボウ") > 0 _
		'Or InStr(wname, "ボウガン") > 0 _
		'Or InStr(wname, "矢") > 0 Or InStr(wname, "アロー") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "矢"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "バルカン") > 0 Then
			wtype = "バルカン"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ガトリング") > 0 Or InStr(wname, "チェーンガン") Or InStr(wname, "ガンランチャー") Then
			wtype = "ガトリング"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "ヘビーマシンガン"
		wtype = "マシンガン"
		'End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "機銃") > 0 Or InStr(wname, "機関砲") > 0 Then
			wtype = "マシンガン"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "マシンキャノン") > 0 Or InStr(wname, "オートキャノン") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "ヘビーマシンガン"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "散弾") > 0 Or InStr(wname, "拡散バズーカ") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "ベアリング") > 0 Or InStr(wname, "クレイモア") > 0 Then
			wtype = "ベアリング"
			GoTo FoundWeaponType
		End If
		
SkipNormalWeapon: 
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If wtype = "Invalid_string_refer_to_original_code" Then
			If InStr(CurrentWeaponType, "Invalid_string_refer_to_original_code") > 0 Or InStr(CurrentWeaponType, "レーザー") > 0 Then
				'Invalid_string_refer_to_original_code
				Select Case CurrentWeaponType
					Case "Invalid_string_refer_to_original_code"
						wtype = "小ビーム"
					Case "Invalid_string_refer_to_original_code"
						wtype = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						wtype = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						wtype = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						wtype = "Invalid_string_refer_to_original_code"
					Case "レーザーガン"
						wtype = "Invalid_string_refer_to_original_code"
					Case "レーザーマシンガン"
						wtype = "Invalid_string_refer_to_original_code"
					Case Else
						wtype = CurrentWeaponType
				End Select
			Else
				If InStr(wname, "ハイメガ") > 0 Or InStr(wname, "バスター") > 0 Or InStr(wname, "大") > 0 Or Left(wname, 2) = "ギガ" Then
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStr(wname, "メガ") > 0 Or InStr(wname, "ハイ") > 0 Or InStr(wname, "バズーカ") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf CountAttack0(u, w) >= 4 Or InStr(wname, "対空") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					wtype = "Invalid_string_refer_to_original_code"
				ElseIf InStr(wname, "ランチャー") > 0 Or InStr(wname, "キャノン") > 0 Or InStr(wname, "カノン") > 0 Or InStr(wname, "砲") > 0 Then 
					wtype = "Invalid_string_refer_to_original_code"
				Else
					wtype = "小ビーム"
				End If
				
				Select Case wtype
					Case "小ビーム", "Invalid_string_refer_to_original_code"
						If double_weapon Then
							wtype = "Invalid_string_refer_to_original_code" & wtype
						End If
				End Select
				
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				wtype = "Invalid_string_refer_to_original_code"
			End If
			
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
		End If
		'End If
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ライフル") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or CurrentWeaponType = "Invalid_string_refer_to_original_code" Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "反応弾") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "マイン") > 0 Or InStr(wname, "ボム") > 0 _
		'Or InStr(wname, "魚雷") > 0 Or InStr(wname, "機雷") > 0 _
		'Or InStr(wname, "バズーカ") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "マイクロ") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		wtype = "Invalid_string_refer_to_original_code"
		'End If
		
		'Invalid_string_refer_to_original_code
		
		If wtype = "Invalid_string_refer_to_original_code" Then
			GoTo FoundWeaponType
		End If
		
		attack_times = CountAttack0(u, w)
		If InStrNotNest(wclass, "連") > 0 Then
			attack_times = hit_count
		End If
		
		If attack_times = 1 Then
			attack_times = 0
			GoTo FoundWeaponType
		End If
		
		If wtype = "Invalid_string_refer_to_original_code" Then
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "雷") > 0 Or InStr(wname, "ライトニング") > 0 Or InStr(wname, "サンダー") > 0 Or Right(wname, 2) = "稲妻" Or InStrNotNest(wclass, "電") > 0 Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "放電"
			GoTo FoundWeaponType
		End If
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "吹雪"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "強風"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "風") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "風"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "竜巻") > 0 Or InStr(wname, "渦巻") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "竜巻"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "泡") > 0 Or InStr(wname, "バブル") > 0 Or InStr(wname, "消火") > 0 Then
			wtype = "泡"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "縮退") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "重力圧縮"
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "スロウ") > 0 Then
			wtype = "Invalid_string_refer_to_original_code"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 2) = "ガス" Or Right$(wname, 1) = "霧" _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		cname = "Invalid_string_refer_to_original_code"
		'End If
		GoTo FoundWeaponType
		'End If
		
		If InStr(wname, "火炎弾") > 0 Then
			wtype = "火炎弾"
			GoTo FoundWeaponType
		End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		If Right(wname, 5) = "ファイアー" Or Right(wname, 5) = "ファイヤー" Or Right(wname, 4) = "ファイア" Or Right(wname, 4) = "ファイヤ" Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
		Else
			wtype = "Invalid_string_refer_to_original_code"
		End If
		GoTo FoundWeaponType
		'End If
		'End If
		
		If InStr(wname, "息") > 0 Or Right(wname, 3) = "ブレス" Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			wtype = "Invalid_string_refer_to_original_code"
			
			Select Case SpellColor(wname, wclass)
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					cname = SpellColor(wname, wclass)
					sname = "Breath.wav"
			End Select
			
			GoTo FoundWeaponType
		End If
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or Right$(wname, 1) = "液" Or Right$(wname, 1) = "酸" _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "飛沫"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		cname = "Invalid_string_refer_to_original_code"
		'UPGRADE_WARNING: HitAnimation �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		cname = "白"
		cname = "Invalid_string_refer_to_original_code"
		'End If
		sname = "Splash.wav"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		wtype = "Invalid_string_refer_to_original_code"
		GoTo FoundWeaponType
		'End If
		
		'Invalid_string_refer_to_original_code
		If u.WeaponPower(w, "") = 0 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		wtype = "ダメージ"
		
FoundWeaponType: 
		
		'Invalid_string_refer_to_original_code
		Select Case wtype
			Case "Invalid_string_refer_to_original_code"
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
				If InStrNotNest(wclass, "吹") > 0 Or InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
					wtype = "打撃"
				End If
		End Select
		
		'Invalid_string_refer_to_original_code
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "赤") > 0 Then
			cname = "赤"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "白"
		End If
		
		'Invalid_string_refer_to_original_code
		If Len(wtype0) > 0 Then
			'Invalid_string_refer_to_original_code
			aname = wtype0 & "命中"
			
			'色
			If Len(cname) > 0 Then
				aname = aname & " " & cname
			End If
			
			'命中アニメ表示
			ShowAnimation(aname)
		End If
		
		'Invalid_string_refer_to_original_code
		aname = wtype & "命中"
		
		'色
		If Len(cname) > 0 Then
			aname = aname & " " & cname
		End If
		
		'効果音
		If Len(sname) > 0 Then
			aname = aname & " " & sname
		End If
		
		'命中数
		If attack_times > 0 Then
			aname = aname & " " & VB6.Format(attack_times)
		End If
		
		'命中アニメ表示
		ShowAnimation(aname)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub HitSound(ByRef u As Unit, ByRef w As Short, ByRef t As Unit, ByVal hit_count As Short)
		Dim wname, wclass As String
		Dim num, i As Short
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'効果音の再生回数
		num = CountAttack(u, w)
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Or InStrNotNest(wclass, "接") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Swing.wav")
		Sleep(200)
		PlayWave("Sword.wav")
		For i = 2 To num
			Sleep(200)
			PlayWave("Sword.wav")
		Next 
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ドリル") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Drill.wav")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "バスター") > 0 Or InStr(wname, "ブラスター") > 0 _
		'Or InStr(wname, "クロー") > 0 Or InStr(wname, "ジザース") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or (InStr(wname, "剣") > 0 And InStr(wname, "手裏剣") = 0) _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "羽") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "レーザー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Saber.wav")
		For i = 2 To num
			Sleep(350)
			PlayWave("Saber.wav")
		Next 
		PlayWave("Swing.wav")
		Sleep(190)
		PlayWave("Slash.wav")
		For i = 2 To num
			Sleep(350)
			PlayWave("Slash.wav")
		Next 
		'End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "手裏剣") > 0 _
		'Or InStr(wname, "苦無") > 0 Or InStr(wname, "クナイ") > 0 _
		'Or (InStr(wname, "突き") > 0 _
		'And InStr(wname, "拳") = 0 And InStr(wname, "頭") = 0) _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "レーザー") > 0 _
		'Or InStr(wname, "ランサー") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Saber.wav")
		For i = 2 To num
			Sleep(350)
			PlayWave("Saber.wav")
		Next 
		PlayWave("Swing.wav")
		Sleep(190)
		PlayWave("Stab.wav")
		For i = 2 To num
			Sleep(350)
			PlayWave("Stab.wav")
		Next 
		'End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		If Not t.IsHero Then
			PlayWave("Saber.wav")
			For i = 2 To num
				Sleep(350)
				PlayWave("Saber.wav")
			Next 
		Else
			PlayWave("Stab.wav")
			For i = 2 To num
				Sleep(350)
				PlayWave("Stab.wav")
			Next 
		End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "振動拳") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Combo.wav")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ナックル") > 0 Or InStr(wname, "ブロー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "頭突き") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "蹴") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "石") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "尻尾") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Punch.wav")
		For i = 2 To num
			Sleep(120)
			PlayWave("Punch.wav")
		Next 
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "突撃") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "車輪") > 0 _
		'Or InStr(wname, "キャタピラ") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Crash.wav")
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Bazooka.wav")
		For i = 2 To num
			Sleep(120)
			PlayWave("Bazooka.wav")
		Next 
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Shock(Low).wav")
		'UPGRADE_WARNING: HitSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		PlayWave("Slap.wav")
		For i = 2 To num
			Sleep(120)
			PlayWave("Slap.wav")
		Next 
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "矢") > 0 _
		'Or InStr(wname, "アロー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ボウガン") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ロングボウ") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Stab.wav")
		For i = 2 To num
			Sleep(120)
			PlayWave("Stab.wav")
		Next 
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "チェーン") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "尾") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "糸") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Whip.wav")
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Swing.wav")
		Sleep(500)
		PlayWave("Shock(Low).wav")
		For i = 2 To num
			Sleep(700)
			PlayWave("Swing.wav")
			Sleep(500)
			PlayWave("Shock(Low).wav")
		Next 
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Swing.wav")
		Sleep(700)
		PlayWave("Swing.wav")
		Sleep(500)
		PlayWave("Swing.wav")
		Sleep(300)
		PlayWave("Shock(Low).wav")
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "折り") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "絞め") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Swing.wav")
		Sleep(190)
		PlayWave("BreakOff.wav")
		'UPGRADE_WARNING: HitSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		PlayWave("Explode(Nuclear).wav")
		'UPGRADE_WARNING: HitSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		PlayWave("Explode(Small).wav")
		For i = 2 To num
			Sleep(130)
			PlayWave("Explode(Small).wav")
		Next 
		'UPGRADE_WARNING: HitSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		'無音
		'UPGRADE_WARNING: HitSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		'Invalid_string_refer_to_original_code
		PlayWave("Saber.wav")
		For i = 2 To num
			Sleep(350)
			PlayWave("Saber.wav")
		Next 
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		PlayWave("Punch.wav")
		For i = 2 To num
			Sleep(120)
			PlayWave("Punch.wav")
		Next 
		If Not t.IsHero Then
			PlayWave("Explode(Small).wav")
			For i = 2 To num
				Sleep(130)
				PlayWave("Explode(Small).wav")
			Next 
		End If
		'End If
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ハリケーン") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "サイクロン") > 0 _
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "竜巻") > 0 _
		'Or InStr(wname, "渦巻") > 0 _
		'Or InStr(wname, "台風") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		'UPGRADE_WARNING: HitSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		PlayWave("Inori.wav")
		'UPGRADE_WARNING: HitSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		PlayWave("Fire.wav")
		'UPGRADE_WARNING: HitSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		PlayWave("Crash.wav")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Charge.wav")
		'UPGRADE_WARNING: HitSound �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		PlayWave("Explode(Nuclear).wav")
		If Not t.IsHero Then
			PlayWave("Explode(Small).wav")
			For i = 2 To num
				Sleep(130)
				PlayWave("Explode(Small).wav")
			Next 
		End If
		'End If
		'End If
		
		'フラグをクリア
		IsWavePlayed = False
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub DodgeEffect(ByRef u As Unit, ByRef w As Short)
		Dim wname, wclass As String
		Dim sname As String
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		If u.IsSpecialEffectDefined(wname & "(回避)") Then
			u.SpecialEffect(wname & "(回避)")
			Exit Sub
		End If
		
		If BattleAnimation Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		sname = u.SpecialEffectData(wname)
		If InStr(sname, ";") > 0 Then
			sname = Mid(sname, InStr(sname, ";"))
		End If
		If sname = "Swing.wav" Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Or InStrNotNest(wclass, "接") _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Swing.wav")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "チェーン") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "尾") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "糸") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Swing.wav")
		'End If
		'End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ParryEffect(ByRef u As Unit, ByRef w As Short, ByRef t As Unit)
		Dim wname, wclass As String
		Dim sname As String
		Dim num As Short
		Dim i As Short
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		num = CountAttack(u, w)
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "アサルトライフル") > 0 _
		'Or InStr(wname, "バルカン") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		num = 4
		'End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ランサー") > 0 Or InStr(wname, "ダガー") > 0 _
		'Or InStr(wname, "剣") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Sword.wav"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		sname = "Explode(Small).wav"
		InStrNotNest(wclass, "Invalid_string_refer_to_original_code")
		sname = "BeamCoat.wav"
		sname = "Explode(Small).wav"
		'End If
		
		'Invalid_string_refer_to_original_code
		PlayWave("Saber.wav")
		Sleep(100)
		PlayWave(sname)
		For i = 2 To num
			Sleep(130)
			PlayWave("Saber.wav")
			Sleep(100)
			PlayWave(sname)
		Next 
		
		'フラグをクリア
		IsWavePlayed = False
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ShieldEffect(ByRef u As Unit)
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		ShowAnimation("Invalid_string_refer_to_original_code")
		Exit Sub
		'End If
		
		'Invalid_string_refer_to_original_code
		With u
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			ShowAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			ShowAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			ShowAnimation("Invalid_string_refer_to_original_code")
			ShowAnimation("Invalid_string_refer_to_original_code")
			'End If
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub AbsorbEffect(ByRef u As Unit, ByRef w As Short, ByRef t As Unit)
		Dim wclass, wname, cname As String
		
		'Invalid_string_refer_to_original_code
		If IsRButtonPressed() Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		PlayWave("Charge.wav")
		Exit Sub
		'End If
		
		With u.Weapon(w)
			wname = .Nickname
			wclass = .Class_Renamed
		End With
		
		'Invalid_string_refer_to_original_code
		cname = SpellColor(wname, wclass)
		If cname = "" Then
			IsBeamWeapon(wname, wclass, cname)
		End If
		
		'アニメを表示
		ShowAnimation("Invalid_string_refer_to_original_code" & cname)
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: ctype �� ctype_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Sub CriticalEffect(ByRef ctype_Renamed As String, ByVal w As Short, ByVal ignore_death As Boolean)
		Dim aname, sname As String
		Dim i As Short
		
		If Len(ctype_Renamed) = 0 Then
			ShowAnimation("Invalid_string_refer_to_original_code")
		Else
			For i = 1 To LLength(ctype_Renamed)
				aname = LIndex(ctype_Renamed, i) & "Invalid_string_refer_to_original_code"
				
				If aname = "Invalid_string_refer_to_original_code" And ignore_death Then
					GoTo NextLoop
				End If
				
				If FindNormalLabel("戦闘アニメ_" & aname) = 0 Then
					GoTo NextLoop
				End If
				
				sname = ""
				
				If aname = "Invalid_string_refer_to_original_code" Then
					If SelectedUnit.IsWeaponClassifiedAs(w, "冷") Then
						'Invalid_string_refer_to_original_code
						sname = "-.wav"
					End If
				End If
				
				If sname <> "" Then
					ShowAnimation(aname & " " & sname)
				Else
					ShowAnimation(aname)
				End If
NextLoop: 
			Next 
		End If
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Private Function CountAttack(ByRef u As Unit, ByVal w As Short, Optional ByVal hit_count As Short = 0) As Short
		'Invalid_string_refer_to_original_code
		If MessageWait <= 200 Then
			CountAttack = 1
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code
		If hit_count > 0 And InStr(u.Weapon(w).Class_Renamed, "連") > 0 Then
			CountAttack = hit_count
			Exit Function
		End If
		
		CountAttack = MinLng(CountAttack0(u, w), 4)
	End Function
	
	Private Function CountAttack0(ByRef u As Unit, ByVal w As Short) As Short
		Dim wname, wclass As String
		
		wname = u.WeaponNickname(w)
		wclass = u.Weapon(w).Class_Renamed
		
		'Invalid_string_refer_to_original_code
		If InStrNotNest(wclass, "連") > 0 Then
			CountAttack0 = u.WeaponLevel(w, "連")
			Exit Function
		End If
		
		If InStr(wname, "連") > 0 Then
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 8
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 8
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "二十連") > 0 Then
				CountAttack0 = 8
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 7
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 7
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "十四連") > 0 Then
				CountAttack0 = 7
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "十二連") > 0 Then
				CountAttack0 = 6
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "一連") > 0 Then
				CountAttack0 = 6
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 6
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "九連") > 0 Then
				CountAttack0 = 5
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "八連") > 0 Then
				CountAttack0 = 5
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "Invalid_string_refer_to_original_code") > 0 Then
				CountAttack0 = 5
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "六連") > 0 Then
				CountAttack0 = 4
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "五連") > 0 Then
				CountAttack0 = 4
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "四連") > 0 Then
				CountAttack0 = 4
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "三連") > 0 Then
				CountAttack0 = 3
				Exit Function
			End If
			If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStr(wname, "二連") > 0 Then
				CountAttack0 = 2
				Exit Function
			End If
			
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Or InStr(wname, "多連") > 0 _
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			CountAttack0 = 3
			Exit Function
		End If
		
		CountAttack0 = 2
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "フルファイア") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "パラレル") > 0 _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ファンネル") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CountAttack0 = 4
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "バルカン") > 0 _
		'Or InStr(wname, "ガトリング") > 0 _
		'Or (InStr(wname, "パルス") > 0 And InStr(wname, "インパルス") = 0) _
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ミサイルランチャー") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CountAttack0 = 4
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CountAttack0 = 3
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		CountAttack0 = 2
		Exit Function
		'End If
		
		CountAttack0 = 1
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Function IsBeamWeapon(ByRef wname As String, ByVal wclass As String, ByRef cname As String) As Boolean
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		'Invalid_string_refer_to_original_code
		Exit Function
		'End If
		
		If InStr(wname, "Invalid_string_refer_to_original_code") > 0 Or InStrNotNest(wclass, "Invalid_string_refer_to_original_code") > 0 Then
			IsBeamWeapon = True
		Else
			If Right(wname, 2) = "ガス" Then
				Exit Function
			End If
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ブラスター") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		IsBeamWeapon = True
		cname = "Invalid_string_refer_to_original_code"
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		IsBeamWeapon = True
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		cname = "イエロー"
		cname = "ピンク"
		'End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "フリーザー") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		IsBeamWeapon = True
		cname = "ブルー"
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ニュートロン") > 0 _
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		IsBeamWeapon = True
		cname = "グリーン"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		IsBeamWeapon = True
		cname = "オレンジ"
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		IsBeamWeapon = True
		cname = "イエロー"
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		IsBeamWeapon = True
		cname = "Invalid_string_refer_to_original_code"
		'End If
		
		If cname = "" Then
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			cname = "イエロー"
		Else
			cname = "ピンク"
		End If
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		cname = "ブルー"
		'End If
		'End If
		
		If Not IsBeamWeapon And cname <> "" Then
			'Invalid_string_refer_to_original_code_
			'Or Right$(wname, 1) = "砲" _
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			IsBeamWeapon = True
		End If
		'End If
	End Function
	
	'Invalid_string_refer_to_original_code
	Private Function SpellColor(ByRef wname As String, ByVal wclass As String) As String
		Dim sclass As String
		Dim i As Short
		
		sclass = wname & wclass
		
		'Invalid_string_refer_to_original_code
		For i = 1 To Len(sclass)
			Select Case Mid(sclass, i, 1)
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					SpellColor = "赤"
					Exit Function
				Case "水", "海", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
				Case "風", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
				Case "邪", "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
					SpellColor = "白"
					Exit Function
				Case "日", "陽"
					SpellColor = "Invalid_string_refer_to_original_code"
					Exit Function
			End Select
		Next 
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		SpellColor = "赤"
		Exit Function
		'End If
		
		If InStr(wname, "ウォーター") > 0 Or InStr(wname, "アクア") > 0 Then
			SpellColor = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "ポイズン") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		SpellColor = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "シャドウ") > 0 _
		'Or InStr(wname, "カース") > 0 _
		'Or InStr(wname, "カーズ") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		SpellColor = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code_
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		SpellColor = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		SpellColor = "Invalid_string_refer_to_original_code"
		Exit Function
		'End If
		
		'Invalid_string_refer_to_original_code_
		'Or InStr(wname, "アイス") > 0 _
		'Or InStr(wname, "フリーズ") > 0 _
		'Then
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		SpellColor = "白"
		Exit Function
		'End If
		
		If InStr(wname, "サン") Then
			SpellColor = "Invalid_string_refer_to_original_code"
			Exit Function
		End If
	End Function
	
	
	'破壊アニメーションを表示する
	Public Sub DieAnimation(ByRef u As Unit)
		Dim i As Short
		Dim PT As POINTAPI
		Dim fname, draw_mode As String
		
		With u
			EraseUnitBitmap(.X, .Y)
			
			'Invalid_string_refer_to_original_code
			If Not .IsHero Then
				ExplodeAnimation(.Size, .X, .Y)
				Exit Sub
			End If
			
			GetCursorPos(PT)
			
			'Invalid_string_refer_to_original_code
			If System.Windows.Forms.Form.ActiveForm Is frmMessage Then
				With frmMessage
					If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
						If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
							'Invalid_string_refer_to_original_code
							Exit Sub
						End If
					End If
				End With
			End If
			
			'Invalid_string_refer_to_original_code
			If System.Windows.Forms.Form.ActiveForm Is MainForm Then
				With MainForm
					If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
						If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
							'Invalid_string_refer_to_original_code
							Exit Sub
						End If
					End If
				End With
			End If
			
			'倒れる音
			Select Case .Area
				Case "Invalid_string_refer_to_original_code"
					PlayWave("FallDown.wav")
				Case "空中"
					If MessageWait > 0 Then
						PlayWave("Bomb.wav")
						Sleep(500)
					End If
					If TerrainClass(.X, .Y) = "水" Or TerrainClass(.X, .Y) = "深海" Then
						PlayWave("Splash.wav")
					Else
						PlayWave("FallDown.wav")
					End If
			End Select
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			If MessageWait = 0 Then
				Exit Sub
			End If
			
			Select Case .Party0
				Case "味方", "Invalid_string_refer_to_original_code"
					fname = "Bitmap\Anime\Common\EFFECT_Tile(Ally)"
				Case "敵"
					fname = "Bitmap\Anime\Common\EFFECT_Tile(Enemy)"
				Case "Invalid_string_refer_to_original_code"
					fname = "Bitmap\Anime\Common\EFFECT_Tile(Neutral)"
			End Select
			If FileExists(ScenarioPath & fname & ".bmp") Then
				fname = ScenarioPath & fname
			Else
				fname = AppPath & fname
			End If
			If Not FileExists(fname & "01.bmp") Then
				Exit Sub
			End If
			
			Select Case MapDrawMode
				Case "Invalid_string_refer_to_original_code"
					draw_mode = "Invalid_string_refer_to_original_code"
				Case Else
					draw_mode = MapDrawMode
			End Select
			
			For i = 1 To 6
				DrawPicture(fname & ".bmp", MapToPixelX(.X), MapToPixelY(.Y), 32, 32, 0, 0, 0, 0, draw_mode)
				DrawPicture("Unit\" & .Bitmap, MapToPixelX(.X), MapToPixelY(.Y), 32, 32, 0, 0, 0, 0, "透過 " & draw_mode)
				DrawPicture(fname & "0" & VB6.Format(i) & ".bmp", MapToPixelX(.X), MapToPixelY(.Y), 32, 32, 0, 0, 0, 0, "透過 " & draw_mode)
				'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				MainForm.picMain(0).Refresh()
				Sleep(50)
			Next 
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub ExplodeAnimation(ByRef tsize As String, ByVal tx As Short, ByVal ty As Short)
		Dim i As Short
		Dim PT As POINTAPI
		Static init_explode_animation As Boolean
		Static explode_image_path As String
		Static explode_image_num As Short
		
		'Invalid_string_refer_to_original_code
		If Not init_explode_animation Then
			'Invalid_string_refer_to_original_code
			If FileExists(ScenarioPath & "Bitmap\Anime\Explode\EFFECT_Explode01.bmp") Then
				explode_image_path = ScenarioPath & "Bitmap\Anime\Explode\EFFECT_Explode"
			ElseIf FileExists(ScenarioPath & "Bitmap\Event\Explode01.bmp") Then 
				explode_image_path = ScenarioPath & "Bitmap\Event\Explode"
			ElseIf FileExists(AppPath & "Bitmap\Anime\Explode\EFFECT_Explode01.bmp") Then 
				explode_image_path = AppPath & "Bitmap\Anime\Explode\EFFECT_Explode"
			Else
				explode_image_path = AppPath & "Bitmap\Event\Explode"
			End If
			
			'Invalid_string_refer_to_original_code
			i = 2
			Do While FileExists(explode_image_path & VB6.Format(i, "00") & ".bmp")
				i = i + 1
			Loop 
			explode_image_num = i - 1
		End If
		
		GetCursorPos(PT)
		
		'Invalid_string_refer_to_original_code
		If System.Windows.Forms.Form.ActiveForm Is frmMessage Then
			With frmMessage
				If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
					If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
						'Invalid_string_refer_to_original_code
						Exit Sub
					End If
				End If
			End With
		End If
		
		'Invalid_string_refer_to_original_code
		If System.Windows.Forms.Form.ActiveForm Is MainForm Then
			With MainForm
				If VB6.PixelsToTwipsX(.Left) \ VB6.TwipsPerPixelX <= PT.X And PT.X <= (VB6.PixelsToTwipsX(.Left) + VB6.PixelsToTwipsX(.Width)) \ VB6.TwipsPerPixelX And VB6.PixelsToTwipsY(.Top) \ VB6.TwipsPerPixelY <= PT.Y And PT.Y <= (VB6.PixelsToTwipsY(.Top) + VB6.PixelsToTwipsY(.Height)) \ VB6.TwipsPerPixelY Then
					If (GetAsyncKeyState(RButtonID) And &H8000) <> 0 Then
						'Invalid_string_refer_to_original_code
						Exit Sub
					End If
				End If
			End With
		End If
		
		'Invalid_string_refer_to_original_code
		Select Case tsize
			Case "XL", "LL"
				PlayWave("Explode(Far).wav")
			Case "L", "M", "S", "SS"
				PlayWave("Explode.wav")
		End Select
		
		'Invalid_string_refer_to_original_code
		If MessageWait = 0 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If InStr(explode_image_path, "\Anime\") > 0 Then
			'Invalid_string_refer_to_original_code
			Select Case tsize
				Case "XL"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 64, MapToPixelY(ty) - 64, 160, 160, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(130)
					Next 
				Case "LL"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 56, MapToPixelY(ty) - 56, 144, 144, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(100)
					Next 
				Case "L"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 48, MapToPixelY(ty) - 48, 128, 128, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(70)
					Next 
				Case "M"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 40, MapToPixelY(ty) - 40, 112, 112, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(50)
					Next 
				Case "S"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 24, MapToPixelY(ty) - 24, 80, 80, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
				Case "SS"
					For i = 1 To explode_image_num
						ClearPicture()
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 8, MapToPixelY(ty) - 8, 48, 48, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
			End Select
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		Else
			'汎用イベント画像版の画像を使用
			Select Case tsize
				Case "XL"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 64, MapToPixelY(ty) - 64, 160, 160, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(130)
					Next 
				Case "LL"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 48, MapToPixelY(ty) - 48, 128, 128, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(100)
					Next 
				Case "L"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 32, MapToPixelY(ty) - 32, 96, 96, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(70)
					Next 
				Case "M"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 16, MapToPixelY(ty) - 16, 64, 64, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(50)
					Next 
				Case "S"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx) - 8, MapToPixelY(ty) - 8, 48, 48, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
				Case "SS"
					For i = 1 To explode_image_num
						DrawPicture(explode_image_path & VB6.Format(i, "00") & ".bmp", MapToPixelX(tx), MapToPixelY(ty), 32, 32, 0, 0, 0, 0, "透過")
						'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picMain(0).Refresh()
						Sleep(40)
					Next 
			End Select
			ClearPicture()
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picMain(0).Refresh()
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub NegateEffect(ByRef u As Unit, ByRef t As Unit, ByVal w As Short, ByRef wname As String, ByVal dmg As Integer, ByRef fname As String, ByRef fdata As String, ByVal ecost As Short, ByRef msg As String, ByVal be_quiet As Boolean)
		Dim defined As Boolean
		
		If LIndex(fdata, 1) = "Invalid_string_refer_to_original_code" Or LIndex(fdata, 2) = "Invalid_string_refer_to_original_code" Or LIndex(fdata, 3) = "Invalid_string_refer_to_original_code" Then
			If Not be_quiet Then
				If t.IsMessageDefined("Invalid_string_refer_to_original_code" & fname & ")") Then
					t.PilotMessage("Invalid_string_refer_to_original_code" & fname & ")")
				Else
					t.PilotMessage("Invalid_string_refer_to_original_code")
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			t.PlayAnimation("Invalid_string_refer_to_original_code")
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			t.SpecialEffect("Invalid_string_refer_to_original_code")
		ElseIf dmg < 0 Then 
			AbsorbEffect(u, w, t)
		ElseIf BattleAnimation Then 
			ShowAnimation("Invalid_string_refer_to_original_code" & fname)
		ElseIf Not IsWavePlayed Then 
			PlayWave("BeamCoat.wav")
		End If
		
		If u.IsAnimationDefined(wname & "Invalid_string_refer_to_original_code") Then
			u.PlayAnimation(wname & "Invalid_string_refer_to_original_code")
		ElseIf u.IsSpecialEffectDefined(wname & "Invalid_string_refer_to_original_code") Then 
			u.SpecialEffect(wname & "Invalid_string_refer_to_original_code")
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		t.SysMessage("Invalid_string_refer_to_original_code")
		'UPGRADE_WARNING: NegateEffect �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		If dmg < 0 Then
			DisplaySysMessage(msg & t.Nickname & "Invalid_string_refer_to_original_code")
		Else
			DisplaySysMessage(msg & t.Nickname & "Invalid_string_refer_to_original_code")
		End If
		If dmg < 0 Then
			DisplaySysMessage(msg & t.Nickname & "の[" & fname & "Invalid_string_refer_to_original_code")
		Else
			DisplaySysMessage(msg & t.Nickname & "の[" & fname & "Invalid_string_refer_to_original_code")
		End If
		'End If
		If Not be_quiet Then
			If t.IsMessageDefined("Invalid_string_refer_to_original_code" & fname & ")") Then
				t.PilotMessage("Invalid_string_refer_to_original_code" & fname & ")")
			Else
				t.PilotMessage("Invalid_string_refer_to_original_code")
			End If
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		t.PlayAnimation("Invalid_string_refer_to_original_code")
		defined = True
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		t.SpecialEffect("Invalid_string_refer_to_original_code")
		defined = True
		'UPGRADE_WARNING: NegateEffect �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		AbsorbEffect(u, w, t)
		defined = True
		'UPGRADE_WARNING: NegateEffect �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		If InStr(fdata, "バリア無効化無効") = 0 Or ecost > 0 Then
			If fname = "バリア" Then
				ShowAnimation("Invalid_string_refer_to_original_code")
			ElseIf fname = "" Then 
				ShowAnimation("Invalid_string_refer_to_original_code")
			Else
				ShowAnimation("Invalid_string_refer_to_original_code" & fname)
			End If
			defined = True
		End If
		'End If
		
		If u.IsAnimationDefined(wname & "Invalid_string_refer_to_original_code") Then
			u.PlayAnimation(wname & "Invalid_string_refer_to_original_code")
			defined = True
		ElseIf u.IsSpecialEffectDefined(wname & "Invalid_string_refer_to_original_code") Then 
			u.SpecialEffect(wname & "Invalid_string_refer_to_original_code")
			defined = True
		End If
		
		If Not defined Then
			HitEffect(u, w, t)
		End If
		
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		t.SysMessage("Invalid_string_refer_to_original_code")
		'UPGRADE_WARNING: NegateEffect �ɕϊ�����Ă��Ȃ��X�e�[�g�����g������܂��B�\�[�X �R�[�h���m�F���Ă��������B
		If dmg < 0 Then
			DisplaySysMessage(msg & t.Nickname & "Invalid_string_refer_to_original_code")
		Else
			DisplaySysMessage(msg & t.Nickname & "Invalid_string_refer_to_original_code")
		End If
		If dmg < 0 Then
			DisplaySysMessage(msg & t.Nickname & "の[" & fname & "Invalid_string_refer_to_original_code")
		Else
			DisplaySysMessage(msg & t.Nickname & "の[" & fname & "Invalid_string_refer_to_original_code")
		End If
		'End If
		'End If
	End Sub
End Module