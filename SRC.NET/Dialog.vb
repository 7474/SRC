Option Strict Off
Option Explicit On
Friend Class Dialog
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'繝繧､繧｢繝ｭ繧ｰ縺ｮ繧ｯ繝ｩ繧ｹ
	
	'Invalid_string_refer_to_original_code
	Private intMessageNum As Object
	'Invalid_string_refer_to_original_code
	Private strName() As String
	'Invalid_string_refer_to_original_code
	Private strMessage() As String
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		'UPGRADE_WARNING: オブジェクト intMessageNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		intMessageNum = 0
		ReDim strName(0)
		ReDim strMessage(0)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'繧ｯ繝ｩ繧ｹ縺ｮ霑ｽ蜉
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		ReDim strName(0)
		ReDim strMessage(0)
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub AddMessage(ByRef Name As String, ByRef Message As String)
		'UPGRADE_WARNING: オブジェクト intMessageNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		intMessageNum = intMessageNum + 1
		ReDim Preserve strName(intMessageNum)
		ReDim Preserve strMessage(intMessageNum)
		'UPGRADE_WARNING: オブジェクト intMessageNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		strName(intMessageNum) = Name
		'UPGRADE_WARNING: オブジェクト intMessageNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		strMessage(intMessageNum) = Message
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property Count() As Short
		Get
			'UPGRADE_WARNING: オブジェクト intMessageNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Count = intMessageNum
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	Public ReadOnly Property IsAvailable(ByVal u As Unit, ByVal ignore_condition As Boolean) As Boolean
		Get
			Dim pname, pname2 As String
			Dim i, j As Short
			Dim mpnickname, mpname, mtype As String
			
			With u.MainPilot
				mpname = .Name
				mpnickname = .Nickname
				mtype = .MessageType
			End With
			
			For i = 1 To Count
				pname = strName(i)
				
				'Invalid_string_refer_to_original_code
				If Left(pname, 1) = "@" Then
					pname = Mid(pname, 2)
					For j = 1 To UBound(SelectedPartners)
						With SelectedPartners(j)
							If .CountPilot > 0 Then
								'Invalid_string_refer_to_original_code
								With .MainPilot
									If pname <> .Name And InStr(pname, .Name & "(") <> 1 And pname <> .Nickname And InStr(pname, .Nickname & "(") <> 1 Then
										GoTo NextPartner
									End If
								End With
								
								'Invalid_string_refer_to_original_code
								If Not ignore_condition Then
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Or .IsConditionSatisfied("豺ｷ荵ｱ") _
									'Then
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									IsAvailable = False
									Exit Property
								End If
							End If
							
							'Invalid_string_refer_to_original_code
							GoTo NextMessage
						End With
					Next 
				End If
				'End With
NextPartner: 
			Next 
			
			IsAvailable = False
			Exit Property
			'End If
			
			'Invalid_string_refer_to_original_code
			If InStr(pname, "(") > 0 Then
				If Not PDList.IsDefined2(pname) And NPDList.IsDefined2(pname) Then
					'Invalid_string_refer_to_original_code
					For j = 2 To Len(pname)
						If Mid(pname, Len(pname) - j, 1) = "(" Then
							pname2 = Left(pname, Len(pname) - j - 1)
							Exit For
						End If
					Next 
					
					'Invalid_string_refer_to_original_code
					If PDList.IsDefined2(pname2) Or NPDList.IsDefined2(pname2) Then
						'Invalid_string_refer_to_original_code
						pname = pname2
					End If
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If pname = mpname Then
				GoTo NextMessage
			End If
			If pname = mpnickname Then
				GoTo NextMessage
			End If
			If pname = mtype Then
				GoTo NextMessage
			End If
			
			'Invalid_string_refer_to_original_code
			If NPDList.IsDefined(pname) Then
				If IsGlobalVariableDefined("IsAway(" & pname & ")") Then
					IsAvailable = False
					Exit Property
				End If
				GoTo NextMessage
			End If
			
			If PDList.IsDefined(pname) Then
				'Invalid_string_refer_to_original_code
				
				'Invalid_string_refer_to_original_code
				If Not PList.IsDefined(pname) Then
					IsAvailable = False
					Exit Property
				End If
				
				With PList.Item(pname)
					'Invalid_string_refer_to_original_code
					If .Away Then
						IsAvailable = False
						Exit Property
					End If
					
					'Invalid_string_refer_to_original_code
					If Not ignore_condition And Not .Unit Is Nothing Then
						With .Unit
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Invalid_string_refer_to_original_code_
							'Or .IsConditionSatisfied("豺ｷ荵ｱ") _
							'Then
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							IsAvailable = False
							Exit Property
						End With
					End If
				End With
			End If
			'End With
			'End If
			
NextMessage: 
			'Next
			
			IsAvailable = True
		End Get
	End Property
	
	'Invalid_string_refer_to_original_code
	Public Function Name(ByVal idx As Short) As String
		Name = strName(idx)
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Message(ByVal idx As Short) As String
		Message = strMessage(idx)
	End Function
End Class