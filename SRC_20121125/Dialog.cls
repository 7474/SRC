VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Dialog"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'ダイアログのクラス

'ダイアログに含まれるメッセージ数
Private intMessageNum
'メッセージの話者
Private strName() As String
'メッセージ
Private strMessage() As String

'クラスの初期化
Private Sub Class_Initialize()
    intMessageNum = 0
    ReDim strName(0)
    ReDim strMessage(0)
End Sub

'クラスの追加
Private Sub Class_Terminate()
    ReDim strName(0)
    ReDim strMessage(0)
End Sub

'ダイアログにメッセージを追加
Public Sub AddMessage(Name As String, Message As String)
    intMessageNum = intMessageNum + 1
    ReDim Preserve strName(intMessageNum)
    ReDim Preserve strMessage(intMessageNum)
    strName(intMessageNum) = Name
    strMessage(intMessageNum) = Message
End Sub

'ダイアログに含まれるメッセージ数
Public Property Get Count() As Integer
    Count = intMessageNum
End Property

'idx番目のメッセージの話者
Public Function Name(ByVal idx As Integer) As String
    Name = strName(idx)
End Function

'idx番目のメッセージ
Public Function Message(ByVal idx As Integer) As String
    Message = strMessage(idx)
End Function

'ダイアログに使われているパイロットが全て利用可能か判定
Public Property Get IsAvailable(u As Unit, ByVal ignore_condition As Boolean) As Boolean
Dim pname As String, pname2 As String
Dim i As Integer, j As Integer
Dim mpname As String, mpnickname As String, mtype As String
    
    With u.MainPilot
        mpname = .Name
        mpnickname = .Nickname
        mtype = .MessageType
    End With
    
    For i = 1 To Count
        pname = strName(i)
        
        '合体技のパートナーが指定されている場合
        If Left$(pname, 1) = "@" Then
            pname = Mid$(pname, 2)
            For j = 1 To UBound(SelectedPartners)
                With SelectedPartners(j)
                    If .CountPilot > 0 Then
                        'パートナーの名前と一致する？
                        With .MainPilot
                            If pname <> .Name _
                                And InStr(pname, .Name & "(") <> 1 _
                                And pname <> .Nickname _
                                And InStr(pname, .Nickname & "(") <> 1 _
                            Then
                                GoTo NextPartner
                            End If
                        End With
                        
                        '喋れるかどうかチェック
                        If Not ignore_condition Then
                            If .IsConditionSatisfied("睡眠") _
                                Or .IsConditionSatisfied("麻痺") _
                                Or .IsConditionSatisfied("石化") _
                                Or .IsConditionSatisfied("恐怖") _
                                Or .IsConditionSatisfied("沈黙") _
                                Or .IsConditionSatisfied("混乱") _
                            Then
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
            If Not PDList.IsDefined2(pname) _
                And NPDList.IsDefined2(pname) _
            Then
                '括弧部分を削除
                For j = 2 To Len(pname)
                    If Mid$(pname, Len(pname) - j, 1) = "(" Then
                        pname2 = Left$(pname, Len(pname) - j - 1)
                        Exit For
                    End If
                Next
                
                '表情パターンかどうか判定
                If PDList.IsDefined2(pname2) _
                    Or NPDList.IsDefined2(pname2) _
                Then
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
                        If .IsConditionSatisfied("睡眠") _
                            Or .IsConditionSatisfied("麻痺") _
                            Or .IsConditionSatisfied("石化") _
                            Or .IsConditionSatisfied("恐怖") _
                            Or .IsConditionSatisfied("沈黙") _
                            Or .IsConditionSatisfied("混乱") _
                        Then
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
End Property
