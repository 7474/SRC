Option Strict Off
Option Explicit On
Friend Class LabelData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'繧､繝吶Φ繝医ョ繝ｼ繧ｿ縺ｮ繝ｩ繝吶Ν縺ｮ繧ｯ繝ｩ繧ｹ
	
	'Invalid_string_refer_to_original_code
	Public Name As Event.LabelType
	'陦檎分蜿ｷ
	Public LineNum As Integer
	'Invalid_string_refer_to_original_code
	Public Enable As Boolean
	'Invalid_string_refer_to_original_code
	Public AsterNum As Short
	
	'Invalid_string_refer_to_original_code
	Private StrData As String
	'繝ｩ繝吶Ν縺ｮ蛟区焚
	Private intParaNum As Short
	'Invalid_string_refer_to_original_code
	Private strParas() As String
	'Invalid_string_refer_to_original_code
	Private blnConst() As Boolean
	
	'繝代Λ繝｡繝ｼ繧ｿ縺ｮ蛟区焚
	Public Function CountPara() As Short
		CountPara = intParaNum
	End Function
	
	'繝ｩ繝吶Ν縺ｮ idx 逡ｪ逶ｮ縺ｮ繝代Λ繝｡繝ｼ繧ｿ
	Public Function Para(ByVal idx As Short) As String
		If idx <= intParaNum Then
			If blnConst(idx) Then
				Para = strParas(idx)
			Else
				Para = GetValueAsString(strParas(idx), True)
			End If
		End If
	End Function
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Property Data() As String
		Get
			DataControl = StrData
		End Get
		Set(ByVal Value As String)
			Dim i As Integer
			Dim lname As String
			
			'Invalid_string_refer_to_original_code
			StrData = Value
			
			'Invalid_string_refer_to_original_code
			lname = ListIndex(Value, 1)
			'Invalid_string_refer_to_original_code
			Select Case Asc(lname)
				Case 42 '*
					lname = Mid(lname, 2)
					Select Case Asc(lname)
						Case 42 '*
							lname = Mid(lname, 2)
							AsterNum = 3
						Case 45 '-
							lname = Mid(lname, 2)
							AsterNum = 2
						Case Else
							AsterNum = 2
					End Select
				Case 45 '-
					lname = Mid(lname, 2)
					Select Case Asc(lname)
						Case 42 '*
							lname = Mid(lname, 2)
							AsterNum = 1
						Case 45 '-
							lname = Mid(lname, 2)
					End Select
			End Select
			Select Case lname
				Case "繝励Ο繝ｭ繝ｼ繧ｰ"
					Name = Event_Renamed.LabelType.PrologueEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.StartEventLabel
				Case "繧ｨ繝斐Ο繝ｼ繧ｰ"
					Name = Event_Renamed.LabelType.EpilogueEventLabel
				Case "繧ｿ繝ｼ繝ｳ"
					Name = Event_Renamed.LabelType.TurnEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.DamageEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.DestructionEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.TotalDestructionEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.AttackEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.AfterAttackEventLabel
				Case "莨夊ｩｱ"
					Name = Event_Renamed.LabelType.TalkEventLabel
				Case "謗･隗ｦ"
					Name = Event_Renamed.LabelType.ContactEventLabel
				Case "騾ｲ蜈･"
					Name = Event_Renamed.LabelType.EnterEventLabel
				Case "閼ｱ蜃ｺ"
					Name = Event_Renamed.LabelType.EscapeEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.LandEventLabel
				Case "菴ｿ逕ｨ"
					Name = Event_Renamed.LabelType.UseEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.AfterUseEventLabel
				Case "螟牙ｽ｢"
					Name = Event_Renamed.LabelType.TransformEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.CombineEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.SplitEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.FinishEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.LevelUpEventLabel
				Case "蜍晏茜譚｡莉ｶ"
					Name = Event_Renamed.LabelType.RequirementEventLabel
				Case "蜀埼幕"
					Name = Event_Renamed.LabelType.ResumeEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.MapCommandEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.UnitCommandEventLabel
				Case "Invalid_string_refer_to_original_code"
					Name = Event_Renamed.LabelType.EffectEventLabel
				Case Else
					Name = Event_Renamed.LabelType.NormalLabel
			End Select
			
			'繝代Λ繝｡繝ｼ繧ｿ
			intParaNum = ListLength(Value)
			If intParaNum = -1 Then
				DisplayEventErrorMessage(CurrentLineNum, "Invalid_string_refer_to_original_code")
				Exit Property
			End If
			ReDim strParas(intParaNum)
			ReDim blnConst(intParaNum)
			For i = 2 To intParaNum
				strParas(i) = ListIndex(Value, i)
				'Invalid_string_refer_to_original_code
				If IsNumeric(strParas(i)) Then
					blnConst(i) = True
				ElseIf PDList.IsDefined(strParas(i)) Then 
					If InStr(strParas(i), "荳ｻ莠ｺ蜈ｬ") <> 1 And InStr(strParas(i), "繝偵Ο繧､繝ｳ") <> 1 Then
						blnConst(i) = True
					End If
				ElseIf UDList.IsDefined(strParas(i)) Then 
					blnConst(i) = True
				ElseIf IDList.IsDefined(strParas(i)) Then 
					blnConst(i) = True
				Else
					Select Case strParas(i)
						Case "蜻ｳ譁ｹ", "Invalid_string_refer_to_original_code", "謨ｵ", "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							blnConst(i) = True
						Case "N", "W", "S", "E"
							If Name = Event_Renamed.LabelType.EscapeEventLabel Then
								blnConst(i) = True
							End If
						Case Else
							If Left(strParas(i), 1) = """" And Right(strParas(i), 1) = """" Then
								If InStr(strParas(i), "$(") = 0 Then
									strParas(i) = Mid(strParas(i), 2, Len(strParas(i)) - 2)
									blnConst(i) = True
								End If
							End If
					End Select
				End If
			Next 
		End Set
	End Property
End Class