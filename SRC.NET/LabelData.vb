Option Strict Off
Option Explicit On
Friend Class LabelData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'イベントデータのラベルのクラス
	
	'ラベル名
	Public Name As Event.LabelType
	'行番号
	Public LineNum As Integer
	'ラベルが有効か？
	Public Enable As Boolean
	'アスタリスクの指定状況
	Public AsterNum As Short
	
	'ラベル全体
	Private StrData As String
	'ラベルの個数
	Private intParaNum As Short
	'ラベルの各パラメータ
	Private strParas() As String
	'パラメータが固定値？
	Private blnConst() As Boolean
	
	'パラメータの個数
	Public Function CountPara() As Short
		CountPara = intParaNum
	End Function
	
	'ラベルの idx 番目のパラメータ
	Public Function Para(ByVal idx As Short) As String
		If idx <= intParaNum Then
			If blnConst(idx) Then
				Para = strParas(idx)
			Else
				Para = GetValueAsString(strParas(idx), True)
			End If
		End If
	End Function
	
	'ラベル全体を取り出す
	
	'ラベル全体を設定
	Public Property Data() As String
		Get
			DataControl = StrData
		End Get
		Set(ByVal Value As String)
			Dim i As Integer
			Dim lname As String
			
			'ラベル全体
			StrData = Value
			
			'ラベル名
			lname = ListIndex(Value, 1)
			'「*」は省く
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
				Case "プロローグ"
					Name = Event_Renamed.LabelType.PrologueEventLabel
				Case "スタート"
					Name = Event_Renamed.LabelType.StartEventLabel
				Case "エピローグ"
					Name = Event_Renamed.LabelType.EpilogueEventLabel
				Case "ターン"
					Name = Event_Renamed.LabelType.TurnEventLabel
				Case "損傷率"
					Name = Event_Renamed.LabelType.DamageEventLabel
				Case "破壊"
					Name = Event_Renamed.LabelType.DestructionEventLabel
				Case "全滅"
					Name = Event_Renamed.LabelType.TotalDestructionEventLabel
				Case "攻撃"
					Name = Event_Renamed.LabelType.AttackEventLabel
				Case "攻撃後"
					Name = Event_Renamed.LabelType.AfterAttackEventLabel
				Case "会話"
					Name = Event_Renamed.LabelType.TalkEventLabel
				Case "接触"
					Name = Event_Renamed.LabelType.ContactEventLabel
				Case "進入"
					Name = Event_Renamed.LabelType.EnterEventLabel
				Case "脱出"
					Name = Event_Renamed.LabelType.EscapeEventLabel
				Case "収納"
					Name = Event_Renamed.LabelType.LandEventLabel
				Case "使用"
					Name = Event_Renamed.LabelType.UseEventLabel
				Case "使用後"
					Name = Event_Renamed.LabelType.AfterUseEventLabel
				Case "変形"
					Name = Event_Renamed.LabelType.TransformEventLabel
				Case "合体"
					Name = Event_Renamed.LabelType.CombineEventLabel
				Case "分離"
					Name = Event_Renamed.LabelType.SplitEventLabel
				Case "行動終了"
					Name = Event_Renamed.LabelType.FinishEventLabel
				Case "レベルアップ"
					Name = Event_Renamed.LabelType.LevelUpEventLabel
				Case "勝利条件"
					Name = Event_Renamed.LabelType.RequirementEventLabel
				Case "再開"
					Name = Event_Renamed.LabelType.ResumeEventLabel
				Case "マップコマンド"
					Name = Event_Renamed.LabelType.MapCommandEventLabel
				Case "ユニットコマンド"
					Name = Event_Renamed.LabelType.UnitCommandEventLabel
				Case "特殊効果"
					Name = Event_Renamed.LabelType.EffectEventLabel
				Case Else
					Name = Event_Renamed.LabelType.NormalLabel
			End Select
			
			'パラメータ
			intParaNum = ListLength(Value)
			If intParaNum = -1 Then
				DisplayEventErrorMessage(CurrentLineNum, "ラベルの引数の括弧の対応が取れていません")
				Exit Property
			End If
			ReDim strParas(intParaNum)
			ReDim blnConst(intParaNum)
			For i = 2 To intParaNum
				strParas(i) = ListIndex(Value, i)
				'パラメータが固定値かどうか判定
				If IsNumeric(strParas(i)) Then
					blnConst(i) = True
				ElseIf PDList.IsDefined(strParas(i)) Then 
					If InStr(strParas(i), "主人公") <> 1 And InStr(strParas(i), "ヒロイン") <> 1 Then
						blnConst(i) = True
					End If
				ElseIf UDList.IsDefined(strParas(i)) Then 
					blnConst(i) = True
				ElseIf IDList.IsDefined(strParas(i)) Then 
					blnConst(i) = True
				Else
					Select Case strParas(i)
						Case "味方", "ＮＰＣ", "敵", "中立", "全"
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