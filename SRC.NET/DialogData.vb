Option Strict Off
Option Explicit On
Friend Class DialogData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'特定のパイロットに指定された全ダイアログのクラス
	
	'パイロット一覧
	Public Name As String
	
	'ダイアログ総数
	Private intDialogNum As Short
	'シチュエーション一覧
	Private strSituation() As String
	'ダイアログ一覧
	Private Dialoges() As Dialog
	
	'クラスの初期化
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		intDialogNum = 0
		ReDim strSituation(0)
		ReDim Dialoges(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		ReDim strSituation(0)
		
		For i = 1 To UBound(Dialoges)
			'UPGRADE_NOTE: オブジェクト Dialoges() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			Dialoges(i) = Nothing
		Next 
		'    ReDim Dialoges(0)
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ダイアログを追加
	Public Function AddDialog(ByRef msg_situation As String) As Dialog
		Dim new_dialog As New Dialog
		
		intDialogNum = intDialogNum + 1
		ReDim Preserve strSituation(intDialogNum)
		ReDim Preserve Dialoges(intDialogNum)
		strSituation(intDialogNum) = msg_situation
		Dialoges(intDialogNum) = new_dialog
		AddDialog = new_dialog
	End Function
	
	'ダイアログの総数
	Public Function CountDialog() As Integer
		CountDialog = intDialogNum
	End Function
	
	'idx番目のシチュエーション
	Public Function Situation(ByVal idx As Integer) As String
		Situation = strSituation(idx)
	End Function
	
	'idx番目のダイアログ
	Public Function Dialog(ByVal idx As Integer) As Dialog
		Dialog = Dialoges(idx)
	End Function
	
	'ユニット u のシチュエーション msg_situation におけるダイアログを選択
	Public Function SelectDialog(ByRef msg_situation As String, ByRef u As Unit, Optional ByVal ignore_condition As Boolean = False) As Dialog
		Dim situations() As String
		Dim sub_situations() As String
		Dim list0() As Short
		Dim list0_num As Short
		Dim tlist() As Short
		Dim tlist_num As Short
		Dim list() As Short
		Dim list_num As Short
		Dim j, i, k As Short
		Dim found As Boolean
		Dim t As Unit
		Dim w, tw As Short
		
		'配列領域確保
		ReDim list0(300)
		ReDim tlist(100)
		ReDim list(200)
		
		'シチュエーションを設定
		Select Case msg_situation
			Case "格闘", "射撃"
				ReDim situations(2)
				situations(2) = "攻撃"
			Case "格闘(命中)", "射撃(命中)"
				ReDim situations(2)
				situations(2) = "攻撃(命中)"
			Case "格闘(回避)", "射撃(回避)"
				ReDim situations(2)
				situations(2) = "攻撃(回避)"
			Case "格闘(とどめ)", "射撃(とどめ)"
				ReDim situations(2)
				situations(2) = "攻撃(とどめ)"
			Case "格闘(クリティカル)", "射撃(クリティカル)"
				ReDim situations(2)
				situations(2) = "攻撃(クリティカル)"
			Case "格闘(反撃)", "射撃(反撃)"
				ReDim situations(2)
				situations(2) = "攻撃(反撃)"
			Case "格闘(命中)(反撃)", "射撃(命中)(反撃)"
				ReDim situations(2)
				situations(2) = "攻撃(命中)(反撃)"
			Case "格闘(回避)(反撃)", "射撃(回避)(反撃)"
				ReDim situations(2)
				situations(2) = "攻撃(回避)(反撃)"
			Case "格闘(とどめ)(反撃)", "射撃(とどめ)(反撃)"
				ReDim situations(2)
				situations(2) = "攻撃(とどめ)(反撃)"
			Case "格闘(クリティカル)(反撃)", "射撃(クリティカル)(反撃)"
				ReDim situations(2)
				situations(2) = "攻撃(クリティカル)(反撃)"
			Case Else
				ReDim situations(1)
		End Select
		situations(1) = msg_situation
		
		'メッセージの候補リスト第一次審査
		list0_num = 0
		For i = 1 To intDialogNum
			For j = 1 To UBound(situations)
				If Left(strSituation(i), Len(situations(j))) = situations(j) Then
					If Dialoges(i).IsAvailable(u, ignore_condition) Then
						list0_num = list0_num + 1
						If list0_num > UBound(list0) Then
							ReDim Preserve list0(list0_num)
						End If
						list0(list0_num) = i
					End If
					Exit For
				End If
			Next 
		Next 
		If list0_num = 0 Then
			Exit Function
		End If
		
		'最初に相手限定のシチュエーションのみで検索
		If u Is SelectedUnit Then
			t = SelectedTarget
		ElseIf u Is SelectedTarget Then 
			t = SelectedUnit
		End If
		If t Is Nothing Then
			GoTo SkipMessagesWithTarget
		End If
		
		'相手限定メッセージのリストを作成
		tlist_num = 0
		For i = 1 To list0_num
			If InStr(strSituation(list0(i)), "(対") > 0 Then
				tlist_num = tlist_num + 1
				If tlist_num > UBound(tlist) Then
					ReDim Preserve tlist(tlist_num)
				End If
				tlist(tlist_num) = list0(i)
			End If
		Next 
		If tlist_num = 0 Then
			'相手限定メッセージがない
			GoTo SkipMessagesWithTarget
		End If
		
		'自分自身にアビリティを使う場合は必ず「(対自分)」を優先
		If t Is u Then
			list_num = 0
			For i = 1 To tlist_num
				For j = 1 To UBound(situations)
					If strSituation(tlist(i)) = situations(j) & "(対自分)" Then
						list_num = list_num + 1
						If list_num > UBound(list) Then
							ReDim Preserve list(list_num)
						End If
						list(list_num) = tlist(i)
						Exit For
					End If
				Next 
			Next 
			If list_num > 0 Then
				SelectDialog = Dialoges(list(Dice(list_num)))
				Exit Function
			End If
		End If
		
		Dim wclass, ch As String
		With t
			If .Status <> "出撃" Then
				GoTo SkipMessagesWithTarget
			End If
			
			ReDim sub_situations(8)
			'対パイロット名称
			sub_situations(1) = "(対" & .MainPilot.Name & ")"
			'対パイロット愛称
			sub_situations(2) = "(対" & .MainPilot.Nickname & ")"
			'対ユニット名称
			sub_situations(3) = "(対" & .Name & ")"
			'対ユニット愛称
			sub_situations(4) = "(対" & .Nickname & ")"
			'対ユニットクラス
			sub_situations(5) = "(対" & .Class0 & ")"
			'対ユニットサイズ
			sub_situations(6) = "(対" & .Size & ")"
			'対地形名
			sub_situations(7) = "(対" & TerrainName(.X, .Y) & ")"
			'対エリア
			sub_situations(8) = "(対" & .Area & ")"
			
			'対メッセージクラス
			If .IsFeatureAvailable("メッセージクラス") Then
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(対" & .FeatureData("メッセージクラス") & ")"
			End If
			
			'対性別
			Select Case .MainPilot.Sex
				Case "男性"
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(対男性)"
				Case "女性"
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(対女性)"
			End Select
			
			'対特殊能力
			With .MainPilot
				For i = 1 To .CountSkill
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(対" & .SkillName0(i) & ")"
					If sub_situations(UBound(sub_situations)) = "(対非表示)" Then
						sub_situations(UBound(sub_situations)) = "(対" & .Skill(i) & ")"
					End If
				Next 
			End With
			For i = 1 To .CountFeature
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(対" & .FeatureName0(i) & ")"
				If sub_situations(UBound(sub_situations)) = "(対)" Then
					sub_situations(UBound(sub_situations)) = "(対" & .Feature(i) & ")"
				End If
			Next 
			
			'対弱点
			If Len(.strWeakness) > 0 Then
				For i = 1 To Len(.strWeakness)
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(対弱点=" & Mid(.strWeakness, i, 1) & ")"
				Next 
			End If
			
			'対有効
			If Len(.strEffective) > 0 Then
				For i = 1 To Len(.strEffective)
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(対弱点=" & Mid(.strEffective, i, 1) & ")"
				Next 
			End If
			
			'対ザコ
			If InStr(.MainPilot.Name, "(ザコ)") > 0 And (u.MainPilot.Technique > .MainPilot.Technique Or u.HP > .HP \ 2) Then
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(対ザコ)"
			End If
			
			'対強敵
			If .BossRank >= 0 Or (InStr(.MainPilot.Name, "(ザコ)") = 0 And u.MainPilot.Technique <= .MainPilot.Technique) Then
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(対強敵)"
			End If
			
			'自分が使用する武器をチェック
			w = 0
			If SelectedUnit Is u Then
				If 0 < SelectedWeapon And SelectedWeapon <= u.CountWeapon Then
					w = SelectedWeapon
				End If
			ElseIf SelectedTarget Is u Then 
				If 0 < SelectedTWeapon And SelectedTWeapon <= u.CountWeapon Then
					w = SelectedTWeapon
				End If
			End If
			
			If w > 0 Then
				'対瀕死
				If .HP <= u.Damage(w, t, u.Party = "味方") Then
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(対瀕死)"
				End If
				
				Select Case u.HitProbability(w, t, u.Party = "味方")
					Case Is < 50
						'対高回避率
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "(対高回避率)"
					Case Is >= 100
						'対低回避率
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "(対低回避率)"
				End Select
			End If
			
			'相手が使用する武器をチェック
			tw = 0
			If SelectedUnit Is t Then
				If 0 < SelectedWeapon And SelectedWeapon <= .CountWeapon Then
					tw = SelectedWeapon
				End If
			ElseIf SelectedTarget Is t Then 
				If 0 < SelectedTWeapon And SelectedTWeapon <= .CountWeapon Then
					tw = SelectedTWeapon
				End If
			End If
			
			If tw > 0 Then
				'対武器名
				ReDim Preserve sub_situations(UBound(sub_situations) + 1)
				sub_situations(UBound(sub_situations)) = "(対" & .Weapon(tw).Name & ")"
				
				'対武器属性
				wclass = .WeaponClass(tw)
				For i = 1 To Len(wclass)
					ch = Mid(wclass, i, 1)
					Select Case ch
						Case CStr(0) To CStr(127)
						Case Else
							ReDim Preserve sub_situations(UBound(sub_situations) + 1)
							sub_situations(UBound(sub_situations)) = "(対" & ch & "属性)"
					End Select
				Next 
				
				Select Case .HitProbability(tw, u, .Party = "味方")
					Case Is > 75
						'対高命中率
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "(対高命中率)"
					Case Is < 25
						'対低命中率
						ReDim Preserve sub_situations(UBound(sub_situations) + 1)
						sub_situations(UBound(sub_situations)) = "(対低命中率)"
				End Select
			End If
		End With
		
		'定義されている相手限定メッセージのうち、状況に合ったメッセージを抜き出す
		list_num = 0
		For i = 1 To tlist_num
			found = False
			For j = 1 To UBound(situations)
				For k = 1 To UBound(sub_situations)
					If strSituation(tlist(i)) = situations(j) & sub_situations(k) Then
						found = True
						Exit For
					End If
				Next 
				If found Then
					Exit For
				End If
			Next 
			If found Then
				list_num = list_num + 1
				If list_num > UBound(list) Then
					ReDim Preserve list(list_num)
				End If
				list(list_num) = tlist(i)
			End If
		Next 
		
		'状況に合った相手限定メッセージが一つでもあれば、その中からメッセージを選択
		If list_num > 0 Then
			SelectDialog = Dialoges(list(Dice(list_num)))
			If Dice(2) = 1 Or InStr(msg_situation, "(とどめ)") > 0 Or msg_situation = "挑発" Or msg_situation = "脱力" Or msg_situation = "魅惑" Or msg_situation = "威圧" Or u.Party = t.Party Then
				Exit Function
			End If
		End If
		
SkipMessagesWithTarget: 
		
		'次にサブシチュエーションなしとユニット限定のサブシチュエーションで検索
		If Not u Is Nothing Then
			ReDim sub_situations(3)
			With u
				sub_situations(1) = "(" & .Name & ")"
				sub_situations(2) = "(" & .Nickname0 & ")"
				sub_situations(3) = "(" & .Class0 & ")"
				Select Case msg_situation
					Case "格闘", "射撃", "格闘(反撃)", "射撃(反撃)"
						If SelectedUnit Is u Then
							'自分が使用する武器をチェック
							If 0 < SelectedWeapon And SelectedWeapon <= u.CountWeapon Then
								ReDim Preserve sub_situations(4)
								sub_situations(4) = "(" & .WeaponNickname(SelectedWeapon) & ")"
							End If
						End If
				End Select
				If .IsFeatureAvailable("メッセージクラス") Then
					ReDim Preserve sub_situations(UBound(sub_situations) + 1)
					sub_situations(UBound(sub_situations)) = "(" & .FeatureData("メッセージクラス") & ")"
				End If
			End With
		Else
			ReDim sub_situations(0)
		End If
		
		'上で見つかったメッセージリストの中からシチュエーションに合ったメッセージを抜き出す
		list_num = 0
		For i = 1 To list0_num
			found = False
			For j = 1 To UBound(situations)
				If strSituation(list0(i)) = situations(j) Then
					found = True
					Exit For
				End If
				For k = 1 To UBound(sub_situations)
					If strSituation(list0(i)) = situations(j) & sub_situations(k) Then
						found = True
						Exit For
					End If
				Next 
				If found Then
					Exit For
				End If
			Next 
			If found Then
				list_num = list_num + 1
				If list_num > UBound(list) Then
					ReDim Preserve list(list_num)
				End If
				list(list_num) = list0(i)
			End If
		Next 
		
		'シチュエーションに合ったメッセージが見つかれば、その中からメッセージを選択
		If list_num > 0 Then
			SelectDialog = Dialoges(list(Dice(list_num)))
		End If
	End Function
End Class