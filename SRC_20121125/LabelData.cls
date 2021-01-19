VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "LabelData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'イベントデータのラベルのクラス

'ラベル名
Public Name As LabelType
'行番号
Public LineNum As Long
'ラベルが有効か？
Public Enable As Boolean
'アスタリスクの指定状況
Public AsterNum As Integer

'ラベル全体
Private StrData As String
'ラベルの個数
Private intParaNum As Integer
'ラベルの各パラメータ
Private strParas() As String
'パラメータが固定値？
Private blnConst() As Boolean

'パラメータの個数
Public Function CountPara() As Integer
    CountPara = intParaNum
End Function

'ラベルの idx 番目のパラメータ
Public Function Para(ByVal idx As Integer) As String
    If idx <= intParaNum Then
        If blnConst(idx) Then
            Para = strParas(idx)
        Else
            Para = GetValueAsString(strParas(idx), True)
        End If
    End If
End Function

'ラベル全体を取り出す
Public Property Get Data() As String
    Data = StrData
End Property

'ラベル全体を設定
Public Property Let Data(new_data As String)
Dim i As Long, lname As String
    
    'ラベル全体
    StrData = new_data
    
    'ラベル名
    lname = ListIndex(new_data, 1)
    '「*」は省く
    Select Case Asc(lname)
        Case 42 '*
            lname = Mid$(lname, 2)
            Select Case Asc(lname)
                Case 42 '*
                    lname = Mid$(lname, 2)
                    AsterNum = 3
                Case 45 '-
                    lname = Mid$(lname, 2)
                    AsterNum = 2
                Case Else
                    AsterNum = 2
            End Select
        Case 45 '-
            lname = Mid$(lname, 2)
            Select Case Asc(lname)
                Case 42 '*
                    lname = Mid$(lname, 2)
                    AsterNum = 1
                Case 45 '-
                    lname = Mid$(lname, 2)
            End Select
    End Select
    Select Case lname
        Case "プロローグ"
            Name = PrologueEventLabel
        Case "スタート"
            Name = StartEventLabel
        Case "エピローグ"
            Name = EpilogueEventLabel
        Case "ターン"
            Name = TurnEventLabel
        Case "損傷率"
            Name = DamageEventLabel
        Case "破壊"
            Name = DestructionEventLabel
        Case "全滅"
            Name = TotalDestructionEventLabel
        Case "攻撃"
            Name = AttackEventLabel
        Case "攻撃後"
            Name = AfterAttackEventLabel
        Case "会話"
            Name = TalkEventLabel
        Case "接触"
            Name = ContactEventLabel
        Case "進入"
            Name = EnterEventLabel
        Case "脱出"
            Name = EscapeEventLabel
        Case "収納"
            Name = LandEventLabel
        Case "使用"
            Name = UseEventLabel
        Case "使用後"
            Name = AfterUseEventLabel
        Case "変形"
            Name = TransformEventLabel
        Case "合体"
            Name = CombineEventLabel
        Case "分離"
            Name = SplitEventLabel
        Case "行動終了"
            Name = FinishEventLabel
        Case "レベルアップ"
            Name = LevelUpEventLabel
        Case "勝利条件"
            Name = RequirementEventLabel
        Case "再開"
            Name = ResumeEventLabel
        Case "マップコマンド"
            Name = MapCommandEventLabel
        Case "ユニットコマンド"
            Name = UnitCommandEventLabel
        Case "特殊効果"
            Name = EffectEventLabel
        Case Else
            Name = NormalLabel
    End Select
    
    'パラメータ
    intParaNum = ListLength(new_data)
    If intParaNum = -1 Then
        DisplayEventErrorMessage CurrentLineNum, "ラベルの引数の括弧の対応が取れていません"
        Exit Property
    End If
    ReDim strParas(intParaNum)
    ReDim blnConst(intParaNum)
    For i = 2 To intParaNum
        strParas(i) = ListIndex(new_data, i)
        'パラメータが固定値かどうか判定
        If IsNumeric(strParas(i)) Then
            blnConst(i) = True
        ElseIf PDList.IsDefined(strParas(i)) Then
            If InStr(strParas(i), "主人公") <> 1 _
                And InStr(strParas(i), "ヒロイン") <> 1 _
            Then
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
                    If Name = EscapeEventLabel Then
                        blnConst(i) = True
                    End If
                Case Else
                    If Left$(strParas(i), 1) = """" _
                        And Right$(strParas(i), 1) = """" _
                    Then
                        If InStr(strParas(i), "$(") = 0 Then
                            strParas(i) = Mid$(strParas(i), 2, Len(strParas(i)) - 2)
                            blnConst(i) = True
                        End If
                    End If
            End Select
        End If
    Next
End Property

