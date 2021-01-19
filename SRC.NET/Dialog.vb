Option Strict Off
Option Explicit On
Friend Class Dialog
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'ダイアログのクラス
	
	'ダイアログに含まれるメッセージ数
	Private intMessageNum As Object
	'メッセージの話者
	Private strName() As String
	'メッセージ
	Private strMessage() As String
	
	'クラスの初期化
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
	
	'クラスの追加
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		ReDim strName(0)
		ReDim strMessage(0)
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ダイアログにメッセージを追加
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
	
	'ダイアログに含まれるメッセージ数
	Public ReadOnly Property Count() As Short
		Get
			'UPGRADE_WARNING: オブジェクト intMessageNum の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			Count = intMessageNum
		End Get
	End Property
	
	'ダイアログに使われているパイロットが全て利用可能か判定
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
				
				'合体技のパートナーが指定されている場合
				If Left(pname, 1) = "@" Then
					pname = Mid(pname, 2)
					For j = 1 To UBound(SelectedPartners)
						With SelectedPartners(j)
							If .CountPilot > 0 Then
								'パートナーの名前と一致する？
								With .MainPilot
									If pname <> .Name And InStr(pname, .Name & "(") <> 1 And pname <> .Nickname And InStr(pname, .Nickname & "(") <> 1 Then
										GoTo NextPartner
									End If
								End With
								
								'喋れるかどうかチェック
								If Not ignore_condition Then
									If .IsConditionSatisfied("睡眠") Or .IsConditionSatisfied("麻痺") Or .IsConditionSatisfied("石化") Or .IsConditionSatisfied("恐怖") Or .IsConditionSatisfied("沈黙") Or .IsConditionSatisfied("混乱") Then
										IsAvailable = False
										Exit Property
									End If
								End If
								
								'メッセージは使用可能
								GoTo NextMessage
							End If
						End With
NextPartner: 
					Next 
					
					IsAvailable = False
					Exit Property
				End If
				
				'表情パターンが指定されている？
				If InStr(pname, "(") > 0 Then
					If Not PDList.IsDefined2(pname) And NPDList.IsDefined2(pname) Then
						'括弧部分を削除
						For j = 2 To Len(pname)
							If Mid(pname, Len(pname) - j, 1) = "(" Then
								pname2 = Left(pname, Len(pname) - j - 1)
								Exit For
							End If
						Next 
						
						'表情パターンかどうか判定
						If PDList.IsDefined2(pname2) Or NPDList.IsDefined2(pname2) Then
							'表情パターンとみなす
							pname = pname2
						End If
					End If
				End If
				
				'メインパイロットは常に存在
				If pname = mpname Then
					GoTo NextMessage
				End If
				If pname = mpnickname Then
					GoTo NextMessage
				End If
				If pname = mtype Then
					GoTo NextMessage
				End If
				
				'ノンパイロットはLeaveしていない限り常に存在
				If NPDList.IsDefined(pname) Then
					If IsGlobalVariableDefined("IsAway(" & pname & ")") Then
						IsAvailable = False
						Exit Property
					End If
					GoTo NextMessage
				End If
				
				If PDList.IsDefined(pname) Then
					'パイロットの場合
					
					'パイロットが作成されていない？
					If Not PList.IsDefined(pname) Then
						IsAvailable = False
						Exit Property
					End If
					
					With PList.Item(pname)
						'Leave中？
						If .Away Then
							IsAvailable = False
							Exit Property
						End If
						
						'喋れるかどうかチェック
						If Not ignore_condition And Not .Unit Is Nothing Then
							With .Unit
								If .IsConditionSatisfied("睡眠") Or .IsConditionSatisfied("麻痺") Or .IsConditionSatisfied("石化") Or .IsConditionSatisfied("恐怖") Or .IsConditionSatisfied("沈黙") Or .IsConditionSatisfied("混乱") Then
									IsAvailable = False
									Exit Property
								End If
							End With
						End If
					End With
				End If
				
NextMessage: 
			Next 
			
			IsAvailable = True
		End Get
	End Property
	
	'idx番目のメッセージの話者
	Public Function Name(ByVal idx As Short) As String
		Name = strName(idx)
	End Function
	
	'idx番目のメッセージ
	Public Function Message(ByVal idx As Short) As String
		Message = strMessage(idx)
	End Function
End Class