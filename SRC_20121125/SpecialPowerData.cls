VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "SpecialPowerData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'スペシャルパワーデータのクラス

'スペシャルパワー名
Public Name As String
'スペシャルパワー名の読み仮名
Public KanaName As String
'スペシャルパワー名の短縮形
Public ShortName As String
'消費ＳＰ
Public SPConsumption As Integer
'対象
Public TargetType As String
'効果時間
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


'クラスの初期化
Private Sub Class_Initialize()
    ReDim strEffectType(0)
    ReDim dblEffectLevel(0)
    ReDim strEffectData(0)
End Sub


'スペシャルパワーに効果を追加
Public Sub SetEffect(elist As String)
Dim i As Integer, j As Integer, k As Integer
Dim buf As String
Dim etype As String, elevel As String, edata As String

    ReDim strEffectType(ListLength(elist))
    ReDim dblEffectLevel(ListLength(elist))
    ReDim strEffectData(ListLength(elist))
    
    TrimString elist
    For i = 1 To ListLength(elist)
        buf = ListIndex(elist, i)
        j = InStr(buf, "Lv")
        k = InStr(buf, "=")
        If j > 0 And (k = 0 Or j < k) Then
            'レベル指定のある効果(データ指定を伴うものを含む)
            strEffectType(i) = Left$(buf, j - 1)
            
            If k > 0 Then
                'データ指定を伴うもの
                dblEffectLevel(i) = CDbl(Mid$(buf, j + 2, k - (j + 2)))
                
                buf = Mid$(buf, k + 1)
                If Left$(buf, 1) = """" Then
                    buf = Mid$(buf, 2, Len(buf) - 2)
                End If
                
                j = InStr(buf, "Lv")
                k = InStr(buf, "=")
                
                If j > 0 And (k = 0 Or j < k) Then
                    'データ指定中にレベル指定があるもの
                    etype = Left$(buf, j - 1)
                    If k > 0 Then
                        elevel = Mid$(buf, j + 2, k - (j + 2))
                        edata = Mid$(buf, k + 1)
                    Else
                        elevel = Mid$(buf, j + 2)
                        edata = ""
                    End If
                ElseIf k > 0 Then
                    'データ指定中にデータ指定があるもの
                    etype = Left$(buf, k - 1)
                    elevel = ""
                    edata = Mid$(buf, k + 1)
                Else
                    'データ指定のみ
                    etype = buf
                    elevel = ""
                    edata = ""
                End If
                
                If Name = "付加" And elevel = "" Then
                    elevel = Format$(DEFAULT_LEVEL)
                End If
                
                strEffectData(i) = Trim$(etype & " " & elevel & " " & edata)
            Else
                'データ指定を伴わないもの
                dblEffectLevel(i) = CDbl(Mid$(buf, j + 2))
            End If
        ElseIf k > 0 Then
            'データ指定を伴う効果
            strEffectType(i) = Left$(buf, k - 1)
            
            buf = Mid$(buf, k + 1)
            If Asc(buf) = 34 Then '"
                buf = Mid$(buf, 2, Len(buf) - 2)
            End If
            
            j = InStr(buf, "Lv")
            k = InStr(buf, "=")
            
            If j > 0 Then
                'データ指定中にレベル指定があるもの
                etype = Left$(buf, j - 1)
                If k > 0 Then
                    elevel = Mid$(buf, j + 2, k - (j + 2))
                    edata = Mid$(buf, k + 1)
                Else
                    elevel = Mid$(buf, j + 2)
                    edata = ""
                End If
            ElseIf k > 0 Then
                'データ指定中にデータ指定があるもの
                etype = Left$(buf, k - 1)
                elevel = ""
                edata = Mid$(buf, k + 1)
            Else
                'データ指定のみ
                etype = buf
                elevel = ""
                edata = ""
            End If
            
            If Name = "付加" And elevel = "" Then
                elevel = Format$(DEFAULT_LEVEL)
            End If
            
            strEffectData(i) = Trim$(etype & " " & elevel & " " & edata)
        Else
            '効果名のみ
            strEffectType(i) = buf
        End If
    Next
End Sub


'特殊効果の個数
Public Function CountEffect() As Integer
    CountEffect = UBound(strEffectType)
End Function

'idx番目の特殊効果タイプ
Public Function EffectType(ByVal idx As Integer) As String
    EffectType = strEffectType(idx)
End Function

'idx番目の特殊効果レベル
Public Function EffectLevel(ByVal idx As Integer) As Double
    EffectLevel = dblEffectLevel(idx)
End Function

'idx番目の特殊効果データ
Public Function EffectData(ByVal idx As Integer) As String
    EffectData = strEffectData(idx)
End Function

'特殊効果 ename を持っているか
Public Function IsEffectAvailable(ename As String)
Dim i As Integer

    For i = 1 To CountEffect
        If ename = EffectType(i) Then
            IsEffectAvailable = True
            Exit Function
        End If
        
        If EffectType(i) = "スペシャルパワー" Then
            If SPDList.Item(EffectData(i)).IsEffectAvailable(ename) Then
                IsEffectAvailable = True
                Exit Function
            End If
        End If
    Next
End Function


'スペシャルパワーがその時点で役に立つかどうか
'(パイロット p が使用した場合)
Public Function Useful(p As Pilot) As Boolean
Dim u As Unit, i As Integer

    Select Case TargetType
        Case "自分"
            Useful = Effective(p, p.Unit)
            Exit Function
            
        Case "味方", "全味方"
            For Each u In UList
                With u
                    '出撃している？
                    If .Status <> "出撃" Then
                        GoTo NextUnit1
                    End If
                    
                    '味方ユニット？
                    Select Case p.Party
                        Case "味方", "ＮＰＣ"
                            If .Party <> "味方" _
                                And .Party0 <> "味方" _
                                And .Party <> "ＮＰＣ" _
                                And .Party0 <> "ＮＰＣ" _
                            Then
                                GoTo NextUnit1
                            End If
                        Case Else
                            If p.Party <> .Party Then
                                GoTo NextUnit1
                            End If
                    End Select
                    
                    '効果がある？
                    If Effective(p, u) Then
                        Useful = True
                        Exit Function
                    End If
                End With
NextUnit1:
            Next
            
        Case "破壊味方"
            For Each u In UList
                With u
                    '破壊されている？
                    If .Status <> "破壊" Then
                        GoTo NextUnit2
                    End If
                    
                    '味方ユニット？
                    If p.Party <> .Party0 Then
                        GoTo NextUnit2
                    End If
                    
                    '効果がある？
                    If Effective(p, u) Then
                        Useful = True
                        Exit Function
                    End If
                End With
NextUnit2:
            Next
            
        Case "敵", "全敵"
            For Each u In UList
                With u
                    '出撃している？
                    If .Status <> "出撃" Then
                        GoTo NextUnit3
                    End If
                    
                    '敵ユニット？
                    Select Case p.Party
                        Case "味方", "ＮＰＣ"
                            If (.Party = "味方" _
                                   And .Party0 = "味方") _
                               Or (.Party = "ＮＰＣ" _
                                   And .Party0 = "ＮＰＣ") _
                            Then
                                GoTo NextUnit3
                            End If
                        Case Else
                            If p.Party = .Party Then
                                GoTo NextUnit3
                            End If
                    End Select
                    
                    '効果がある？
                    If Effective(p, u) Then
                        Useful = True
                        Exit Function
                    End If
                End With
NextUnit3:
            Next
            
        Case "任意", "全"
            For Each u In UList
                '出撃している？
                If u.Status = "出撃" Then
                    '効果がある？
                    If Effective(p, u) Then
                        Useful = True
                        Exit Function
                    End If
                End If
            Next
    End Select
End Function

'スペシャルパワーがユニット t に対して効果があるかどうか
'(パイロット p が使用した場合)
Public Function Effective(p As Pilot, t As Unit) As Boolean
Dim i As Integer, j As Integer
Dim ncond As String
Dim my_unit As Unit

    '同じ追加パイロットを持つユニットが複数いる場合、パイロットのUnitが
    '変化してしまうことがあるため、元のUnitを記録しておく
    Set my_unit = p.Unit
    
    With t
        '適用条件を満たしている？
        For i = 1 To LLength(NecessaryCondition)
            ncond = LIndex(NecessaryCondition, i)
            Select Case ncond
                Case "技量"
                    If p.Technique < .MainPilot.Technique Then
                        GoTo ExitFunc
                    End If
                Case "非ボス"
                    If .BossRank >= 0 Then
                        GoTo ExitFunc
                    End If
                Case "支援"
                    If my_unit Is t Then
                        GoTo ExitFunc
                    End If
                Case "隣接"
                    With my_unit
                        If Abs(.X - t.X) + Abs(.Y - t.Y) <> 1 Then
                            GoTo ExitFunc
                        End If
                    End With
                Case Else
                    If InStr(ncond, "射程Lv") = 1 Then
                        With my_unit
                            If Abs(.X - t.X) + Abs(.Y - t.Y) > StrToLng(Mid$(ncond, 5)) Then
                                GoTo ExitFunc
                            End If
                        End With
                    End If
            End Select
            
            'Unitが変化してしまった場合は元に戻しておく
            If Not my_unit Is p.Unit Then
                my_unit.MainPilot
            End If
        Next
        
        '無効化されている？
        Select Case TargetType
            Case "敵", "全敵", "任意", "全"
                If .IsConditionSatisfied("スペシャルパワー無効化") _
                    Or .IsConditionSatisfied("精神コマンド無効化") _
                Then
                    GoTo ExitFunc
                End If
        End Select
        
        '持続効果があるものは同じスペシャルパワーが既に適用されていなければ
        '効果があるとみなす
        If Duration <> "即効" Then
            If Not .IsSpecialPowerInEffect(Name) Then
                Effective = True
            End If
            
            'ただしみがわりは自分自身には使えないのでチェックしておく
            If EffectType(1) = "みがわり" Then
                If my_unit Is t Then
                    Effective = False
                    GoTo ExitFunc
                End If
            End If
            
            GoTo ExitFunc
        End If
        
        '個々の効果に関して有効かどうか判定
        For i = 1 To CountEffect
            Select Case EffectType(i)
                Case "ＨＰ回復", "ＨＰ増加"
                    If EffectLevel(i) < 0 Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                    If .IsConditionSatisfied("ゾンビ") Then
                        GoTo NextEffect
                    End If
                    If .HP < .MaxHP Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                Case "ＥＮ回復", "ＥＮ増加"
                    If EffectLevel(i) < 0 Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                    If .IsConditionSatisfied("ゾンビ") Then
                        GoTo NextEffect
                    End If
                    If .EN < .MaxEN Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                Case "霊力回復", "霊力増加"
                    If EffectLevel(i) < 0 Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                    If .IsConditionSatisfied("ゾンビ") Then
                        GoTo NextEffect
                    End If
                    If .MainPilot.Plana < .MainPilot.MaxPlana Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                Case "ＳＰ回復"
                    If EffectLevel(i) < 0 Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                    If .IsConditionSatisfied("ゾンビ") Then
                        GoTo NextEffect
                    End If
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
                    If .IsFeatureAvailable("追加サポート") Then
                        If .AdditionalSupport.SP < .AdditionalSupport.MaxSP Then
                            Effective = True
                            GoTo ExitFunc
                        End If
                    End If
                Case "状態回復"
                    If .ConditionLifetime("攻撃不能") > 0 _
                        Or .ConditionLifetime("移動不能") > 0 _
                        Or .ConditionLifetime("装甲劣化") > 0 _
                        Or .ConditionLifetime("混乱") > 0 _
                        Or .ConditionLifetime("魅了") > 0 _
                        Or .ConditionLifetime("憑依") > 0 _
                        Or .ConditionLifetime("石化") > 0 _
                        Or .ConditionLifetime("凍結") > 0 _
                        Or .ConditionLifetime("麻痺") > 0 _
                        Or .ConditionLifetime("睡眠") > 0 _
                        Or .ConditionLifetime("毒") > 0 _
                        Or .ConditionLifetime("盲目") > 0 _
                        Or .ConditionLifetime("撹乱") > 0 _
                        Or .ConditionLifetime("恐怖") > 0 _
                        Or .ConditionLifetime("沈黙") > 0 _
                        Or .ConditionLifetime("ゾンビ") > 0 _
                        Or .ConditionLifetime("回復不能") > 0 _
                        Or .ConditionLifetime("オーラ使用不能") > 0 Or .ConditionLifetime("超能力使用不能") > 0 _
                        Or .ConditionLifetime("同調率使用不能") > 0 Or .ConditionLifetime("超感覚使用不能") > 0 _
                        Or .ConditionLifetime("知覚強化使用不能") > 0 Or .ConditionLifetime("霊力使用不能") > 0 _
                        Or .ConditionLifetime("術使用不能") > 0 Or .ConditionLifetime("技使用不能") > 0 _
                    Then
                        Effective = True
                        GoTo ExitFunc
                    Else
                        For j = 1 To .CountCondition
                            If Len(.Condition(j)) > 6 Then
                                If Right$(.Condition(j), 6) = "属性使用不能" Then
                                    If .ConditionLifetime(.Condition(j)) > 0 Then
                                        Effective = True
                                        GoTo ExitFunc
                                    End If
                                End If
                            End If
                        Next
                    End If
                Case "装填"
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
                Case "スペシャルパワー", "精神コマンド"
                    If Not .IsSpecialPowerInEffect(EffectData(i)) Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                Case "気力増加"
                    If .MainPilot.Personality <> "機械" _
                        And .MainPilot.Morale < .MainPilot.MaxMorale _
                    Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                    For j = 2 To .CountPilot
                        If .Pilot(j).Personality <> "機械" _
                            And .Pilot(j).Morale < .MainPilot.MaxMorale _
                        Then
                            Effective = True
                            GoTo ExitFunc
                        End If
                    Next
                Case "気力低下"
                    If .MainPilot.Personality = "機械" Then
                        GoTo NextEffect
                    End If
                    If .MainPilot.Morale > .MainPilot.MinMorale Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                Case "ランダムダメージ", "ＨＰ減少", "ＥＮ減少"
                    If Not .IsConditionSatisfied("無敵") Then
                        Effective = True
                        GoTo ExitFunc
                    End If
                Case "気力増加", "自爆", "復活", "偵察", _
                    "味方スペシャルパワー実行", "イベント"
                    Effective = True
                    GoTo ExitFunc
            End Select
NextEffect:
        Next
    End With
    
ExitFunc:
    
    'Unitが変化してしまった場合は元に戻しておく
    If Not my_unit Is p.Unit Then
        my_unit.MainPilot
    End If
End Function


'スペシャルパワーを使用する
'(パイロット p が使用した場合)
Public Sub Execute(p As Pilot, Optional ByVal is_event As Boolean)
Dim u As Unit, i As Integer, j As Integer

    Select Case TargetType
        Case "自分"
            If Apply(p, p.Unit, is_event) And Not is_event Then
                Sleep 300
            End If
            
        Case "全味方"
            For i = 1 To MapWidth
                For j = 1 To MapHeight
                    Set u = MapDataForUnit(i, j)
                    If u Is Nothing Then
                        GoTo NextUnit1
                    End If
                    With u
                        '味方ユニット？
                        Select Case p.Party
                            Case "味方", "ＮＰＣ"
                                If .Party <> "味方" _
                                    And .Party0 <> "味方" _
                                    And .Party <> "ＮＰＣ" _
                                    And .Party0 <> "ＮＰＣ" _
                                Then
                                    GoTo NextUnit1
                                End If
                            Case Else
                                If p.Party <> .Party Then
                                    GoTo NextUnit1
                                End If
                        End Select
                        
                        Apply p, u, is_event
                    End With
NextUnit1:
                Next
            Next
            If Not is_event Then
                Sleep 300
            End If
            
        Case "全敵"
            For i = 1 To MapWidth
                For j = 1 To MapHeight
                    Set u = MapDataForUnit(i, j)
                    If u Is Nothing Then
                        GoTo NextUnit2
                    End If
                    With u
                        '敵ユニット？
                        Select Case p.Party
                            Case "味方", "ＮＰＣ"
                                If .Party = "味方" _
                                    Or .Party = "ＮＰＣ" _
                                Then
                                    GoTo NextUnit2
                                End If
                            Case Else
                                If p.Party = .Party Then
                                    GoTo NextUnit2
                                End If
                        End Select
                        
                        Apply p, u, is_event
                    End With
NextUnit2:
                Next
            Next
            If Not is_event Then
                Sleep 300
            End If
            
        Case "全"
            For i = 1 To MapWidth
                For j = 1 To MapHeight
                    Set u = MapDataForUnit(i, j)
                    If Not u Is Nothing Then
                        Apply p, u, is_event
                    End If
                Next
            Next
            If Not is_event Then
                Sleep 300
            End If
            
        Case Else
            If Apply(p, SelectedTarget, is_event) And Not is_event Then
                Sleep 300
            End If
    End Select
    
    If Not is_event Then
        CloseMessageForm
        RedrawScreen
    End If
End Sub

'スペシャルパワーをユニット t に対して適用
'(パイロット p が使用)
'実行後にウェイトが必要かどうかを返す
Public Function Apply(p As Pilot, ByVal t As Unit, Optional ByVal is_event As Boolean, _
    Optional ByVal as_instant As Boolean) As Boolean
Dim i As Integer, j As Integer, n As Integer, tmp As Long
Dim is_invalid As Boolean, need_update As Boolean, displayed_string As Boolean
Dim msg As String, ncond As String
Dim my_unit As Unit

    '同じ追加パイロットを持つユニットが複数いる場合、パイロットのUnitが
    '変化してしまうことがあるため、元のUnitを記録しておく
    Set my_unit = p.Unit
    
    With t
        '適用条件を満たしている？
        For i = 1 To LLength(NecessaryCondition)
            ncond = LIndex(NecessaryCondition, i)
            Select Case ncond
                Case "技量"
                    If p.Technique < .MainPilot.Technique Then
                        is_invalid = True
                    End If
                Case "非ボス"
                    If .BossRank >= 0 Then
                        is_invalid = True
                    End If
                Case "支援"
                    If my_unit Is t Then
                        is_invalid = True
                    End If
                Case "隣接"
                    With my_unit
                        If Abs(.X - t.X) + Abs(.Y - t.Y) <> 1 Then
                            is_invalid = True
                        End If
                    End With
                Case Else
                    If InStr(ncond, "射程Lv") = 1 Then
                        With my_unit
                            If Abs(.X - t.X) + Abs(.Y - t.Y) > StrToLng(Mid$(ncond, 5)) Then
                                is_invalid = True
                            End If
                        End With
                    End If
            End Select
            
            'Unitが変化してしまった場合は元に戻しておく
            If Not my_unit Is p.Unit Then
                my_unit.CurrentForm.MainPilot
            End If
        Next
        
        '無効化されている？
        Select Case TargetType
            Case "敵", "全敵"
                If .IsConditionSatisfied("スペシャルパワー無効") Then
                    is_invalid = True
                End If
        End Select
        
        'スペシャルパワーが適用可能？
        If is_invalid Then
            Exit Function
        End If
        
        '持続効果がある場合は単にスペシャルパワーの効果を付加するだけでよい
        If Duration <> "即効" And Not as_instant Then
            .MakeSpecialPowerInEffect Name, my_unit.MainPilot.ID
            Exit Function
        End If
    End With
    
    'これ以降は持続効果が即効であるスペシャルパワーの処理
    
    '個々の効果を適用
    For i = 1 To CountEffect
        With t
            Select Case EffectType(i)
                Case "ＨＰ回復", "ＨＰ増加"
                    '効果が適用可能かどうか判定
                    If EffectLevel(i) > 0 Then
                        If .IsConditionSatisfied("ゾンビ") Then
                            GoTo NextEffect
                        End If
                        If .HP = .MaxHP Then
                            GoTo NextEffect
                        End If
                    End If
                    
                    If Not is_event Then
                        If t Is SelectedUnit Then
                            If Not frmMessage.Visible Then
                                OpenMessageForm SelectedUnit
                            Else
                                UpdateMessageForm SelectedUnit
                            End If
                        Else
                            If Not frmMessage.Visible Then
                                OpenMessageForm t, SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                        End If
                        Sleep 150
                    End If
                    
                    'ＨＰを回復させる
                    tmp = .HP
                    If EffectType(i) = "ＨＰ増加" Then
                        .HP = .HP + 1000 * EffectLevel(i)
                    Else
                        .RecoverHP 10 * EffectLevel(i)
                    End If
                    
                    If Not is_event Then
                        If Not displayed_string Then
                            If EffectLevel(i) >= 0 Then
                                DrawSysString .X, .Y, "+" & Format$(.HP - tmp)
                            Else
                                DrawSysString .X, .Y, Format$(.HP - tmp)
                            End If
                        End If
                        displayed_string = True
                        
                        If t Is SelectedUnit Then
                            UpdateMessageForm SelectedUnit
                        Else
                            UpdateMessageForm t, SelectedUnit
                        End If
                        
                        If EffectLevel(i) >= 0 Then
                            DisplaySysMessage .Nickname & "の" & Term("ＨＰ", t) & "が" _
                                & Format$(.HP - tmp) & "回復した。"
                        Else
                            DisplaySysMessage .Nickname & "の" & Term("ＨＰ", t) & "が" _
                                & Format$(tmp - .HP) & "減少した。"
                        End If
                    End If
                    
                    need_update = True
                    
                Case "ＥＮ回復", "ＥＮ増加"
                    '効果が適用可能かどうか判定
                    If EffectLevel(i) > 0 Then
                        If .IsConditionSatisfied("ゾンビ") Then
                            GoTo NextEffect
                        End If
                        If .EN = .MaxEN Then
                            GoTo NextEffect
                        End If
                    End If
                    
                    If Not is_event Then
                        If t Is SelectedUnit Then
                            If Not frmMessage.Visible Then
                                OpenMessageForm SelectedUnit
                            Else
                                UpdateMessageForm SelectedUnit
                            End If
                        Else
                            If Not frmMessage.Visible Then
                                OpenMessageForm t, SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                        End If
                        Sleep 150
                    End If
                    
                    'ＥＮを回復させる
                    tmp = .EN
                    If EffectType(i) = "ＥＮ増加" Then
                        .EN = .EN + 10 * EffectLevel(i)
                    Else
                        .RecoverEN 10 * EffectLevel(i)
                    End If
                    
                    If Not is_event Then
                        If Not displayed_string Then
                            If EffectLevel(i) >= 0 Then
                                DrawSysString .X, .Y, "+" & Format$(.EN - tmp)
                            Else
                                DrawSysString .X, .Y, Format$(.EN - tmp)
                            End If
                        End If
                        displayed_string = True
                        
                        If t Is SelectedUnit Then
                            UpdateMessageForm SelectedUnit
                        Else
                            UpdateMessageForm t, SelectedUnit
                        End If
                        
                        If EffectLevel(i) >= 0 Then
                            DisplaySysMessage .Nickname & "の" & Term("ＥＮ", t) & "が" _
                                & Format$(.EN - tmp) & "回復した。"
                        Else
                            DisplaySysMessage .Nickname & "の" & Term("ＥＮ", t) & "が" _
                                & Format$(tmp - .EN) & "減少した。"
                        End If
                    End If
                    
                    need_update = True
                    
                Case "霊力回復", "霊力増加"
                    '効果が適用可能かどうか判定
                    If EffectLevel(i) > 0 Then
                        If .IsConditionSatisfied("ゾンビ") Then
                            GoTo NextEffect
                        End If
                        If .MainPilot.Plana = .MainPilot.MaxPlana Then
                            GoTo NextEffect
                        End If
                    End If
                    
                    If Not is_event Then
                        If t Is SelectedUnit Then
                            If Not frmMessage.Visible Then
                                OpenMessageForm SelectedUnit
                            Else
                                UpdateMessageForm SelectedUnit
                            End If
                        Else
                            If Not frmMessage.Visible Then
                                OpenMessageForm t, SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                        End If
                        Sleep 150
                    End If
                    
                    '霊力を回復させる
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
                                DrawSysString .X, .Y, "+" & Format$(.MainPilot.Plana - tmp)
                            Else
                                DrawSysString .X, .Y, Format$(.MainPilot.Plana - tmp)
                            End If
                        End If
                        displayed_string = True
                        
                        If t Is SelectedUnit Then
                            UpdateMessageForm SelectedUnit
                        Else
                            UpdateMessageForm t, SelectedUnit
                        End If
                        
                        If EffectLevel(i) >= 0 Then
                            DisplaySysMessage .Nickname & "の" & .MainPilot.SkillName0("霊力") _
                                & "が" & Format$(.MainPilot.Plana - tmp) & "回復した。"
                        Else
                            DisplaySysMessage .Nickname & "の" & .MainPilot.SkillName0("霊力") _
                                & "が" & Format$(tmp - .MainPilot.Plana) & "減少した。"
                        End If
                    End If
                    
                    need_update = True
                    
                Case "ＳＰ回復"
                    '効果が適用可能かどうか判定
                    If EffectLevel(i) > 0 Then
                        If .IsConditionSatisfied("ゾンビ") Then
                            GoTo NextEffect
                        End If
                    End If
                    
                    If Not is_event Then
                        If t Is SelectedUnit Then
                            If Not frmMessage.Visible Then
                                OpenMessageForm SelectedUnit
                            Else
                                UpdateMessageForm SelectedUnit
                            End If
                        Else
                            If Not frmMessage.Visible Then
                                OpenMessageForm t, SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                        End If
                        Sleep 150
                    End If
                    
                    '回復対象となるパイロット数を算出
                    n = .CountPilot + .CountSupport
                    If .IsFeatureAvailable("追加サポート") Then
                        n = n + 1
                    End If
                    
                    'ＳＰを回復
                    If n = 1 Then
                        'メインパイロットのみのＳＰを回復
                        tmp = .MainPilot.SP
                        .MainPilot.SP = .MainPilot.SP + 10 * EffectLevel(i)
                        
                        If Not is_event Then
                            If Not displayed_string Then
                                If EffectLevel(i) >= 0 Then
                                    DrawSysString .X, .Y, "+" & Format$(.MainPilot.SP - tmp)
                                Else
                                    DrawSysString .X, .Y, Format$(.MainPilot.SP - tmp)
                                End If
                            End If
                            displayed_string = True
                            
                            If EffectLevel(i) >= 0 Then
                                DisplaySysMessage .MainPilot.Nickname & "の" _
                                    & Term("ＳＰ", t) & "が" _
                                    & Format$(.MainPilot.SP - tmp) & "回復した。"
                            Else
                                DisplaySysMessage .MainPilot.Nickname & "の" _
                                    & Term("ＳＰ", t) & "が" _
                                    & Format$(tmp - .MainPilot.SP) & "減少した。"
                            End If
                        End If
                    Else
                        'メインパイロットのＳＰを回復
                        tmp = .MainPilot.SP
                        .MainPilot.SP = .MainPilot.SP _
                            + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
                        
                        If Not is_event Then
                            If Not displayed_string Then
                                If EffectLevel(i) >= 0 Then
                                    DrawSysString .X, .Y, "+" & Format$(.MainPilot.SP - tmp)
                                Else
                                    DrawSysString .X, .Y, Format$(.MainPilot.SP - tmp)
                                End If
                            End If
                            displayed_string = True
                            
                            If EffectLevel(i) >= 0 Then
                                DisplaySysMessage .MainPilot.Nickname & "の" _
                                    & Term("ＳＰ", t) & "が" _
                                    & Format$(.MainPilot.SP - tmp) & "回復した。"
                            Else
                                DisplaySysMessage .MainPilot.Nickname & "の" _
                                    & Term("ＳＰ", t) & "が" _
                                    & Format$(tmp - .MainPilot.SP) & "減少した。"
                            End If
                        End If
                        
                        'サブパイロットのＳＰを回復
                        For j = 2 To .CountPilot
                            With .Pilot(j)
                                tmp = .SP
                                .SP = .SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
                                If Not is_event Then
                                    If .SP <> tmp Then
                                        If EffectLevel(i) >= 0 Then
                                            DisplaySysMessage .Nickname & "の" _
                                                & Term("ＳＰ", t) & "が" _
                                                & Format$(.SP - tmp) & "回復した。"
                                        Else
                                            DisplaySysMessage .Nickname & "の" _
                                                & Term("ＳＰ", t) & "が" _
                                                & Format$(tmp - .SP) & "減少した。"
                                        End If
                                    End If
                                End If
                            End With
                        Next
                        
                        'サポートパイロットのＳＰを回復
                        For j = 1 To .CountSupport
                            With .Support(j)
                                tmp = .SP
                                .SP = .SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
                                If Not is_event Then
                                    If .SP <> tmp Then
                                        If EffectLevel(i) >= 0 Then
                                            DisplaySysMessage .Nickname & "の" _
                                                & Term("ＳＰ", t) & "が" _
                                                & Format$(.SP - tmp) & "回復した。"
                                        Else
                                            DisplaySysMessage .Nickname & "の" _
                                                & Term("ＳＰ", t) & "が" _
                                                & Format$(tmp - .SP) & "減少した。"
                                        End If
                                    End If
                                End If
                            End With
                        Next
                        
                        '追加サポートパイロットのＳＰを回復
                        If .IsFeatureAvailable("追加サポート") Then
                            With .AdditionalSupport
                                tmp = .SP
                                .SP = .SP + 2 * EffectLevel(i) + 10 * EffectLevel(i) \ n
                                If Not is_event Then
                                    If .SP <> tmp Then
                                        If EffectLevel(i) >= 0 Then
                                            DisplaySysMessage .Nickname & "の" _
                                                & Term("ＳＰ", t) & "が" _
                                                & Format$(.SP - tmp) & "回復した。"
                                        Else
                                            DisplaySysMessage .Nickname & "の" _
                                                & Term("ＳＰ", t) & "が" _
                                                & Format$(tmp - .SP) & "減少した。"
                                        End If
                                    End If
                                End If
                            End With
                        End If
                    End If
                    
                    If Not is_event Then
                        If TargetType = "全味方" Then
                            Sleep 150
                        End If
                    End If
                    
                Case "装填"
                    '効果が適用可能かどうか判定
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
                                OpenMessageForm SelectedUnit
                            Else
                                UpdateMessageForm SelectedUnit
                            End If
                        Else
                            If Not frmMessage.Visible Then
                                OpenMessageForm t, SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                        End If
                    End If
                    
                    '弾薬を補給
                    .BulletSupply
                    
                    If Not is_event Then
                        DisplaySysMessage .Nickname & "の弾数が全快した。"
                    End If
                    
                Case "状態回復"
                    If .ConditionLifetime("攻撃不能") <= 0 _
                        And .ConditionLifetime("移動不能") <= 0 _
                        And .ConditionLifetime("装甲劣化") <= 0 _
                        And .ConditionLifetime("混乱") <= 0 _
                        And .ConditionLifetime("魅了") <= 0 _
                        And .ConditionLifetime("憑依") <= 0 _
                        And .ConditionLifetime("石化") <= 0 _
                        And .ConditionLifetime("凍結") <= 0 _
                        And .ConditionLifetime("麻痺") <= 0 _
                        And .ConditionLifetime("睡眠") <= 0 _
                        And .ConditionLifetime("毒") <= 0 _
                        And .ConditionLifetime("盲目") <= 0 _
                        And .ConditionLifetime("撹乱") <= 0 _
                        And .ConditionLifetime("恐怖") <= 0 _
                        And .ConditionLifetime("沈黙") <= 0 _
                        And .ConditionLifetime("ゾンビ") <= 0 _
                        And .ConditionLifetime("回復不能") <= 0 _
                        And .ConditionLifetime("オーラ使用不能") <= 0 And .ConditionLifetime("超能力使用不能") <= 0 _
                        And .ConditionLifetime("同調率使用不能") <= 0 And .ConditionLifetime("超感覚使用不能") <= 0 _
                        And .ConditionLifetime("知覚強化使用不能") <= 0 And .ConditionLifetime("霊力使用不能") <= 0 _
                        And .ConditionLifetime("術使用不能") <= 0 And .ConditionLifetime("技使用不能") <= 0 _
                    Then
                        For j = 1 To .CountCondition
                            If Len(.Condition(j)) > 6 Then
                                '弱、効属性は状態回復から除外。
                                If Right$(.Condition(j), 6) = "属性使用不能" Then
                                    If .ConditionLifetime(.Condition(j)) > 0 Then
                                        Exit For
                                    End If
                                End If
                            End If
                        Next
                        If (j > .CountCondition) Then
                            GoTo NextEffect
                        End If
                    End If
                    
                    If Not is_event Then
                        If t Is SelectedUnit Then
                            If Not frmMessage.Visible Then
                                OpenMessageForm SelectedUnit
                            Else
                                UpdateMessageForm SelectedUnit
                            End If
                        Else
                            If Not frmMessage.Visible Then
                                OpenMessageForm t, SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                        End If
                    End If
                    
                    '全てのステータス異常を回復
                    If .ConditionLifetime("攻撃不能") > 0 Then
                        .DeleteCondition "攻撃不能"
                    End If
                    If .ConditionLifetime("移動不能") > 0 Then
                        .DeleteCondition "移動不能"
                    End If
                    If .ConditionLifetime("装甲劣化") > 0 Then
                        .DeleteCondition "装甲劣化"
                    End If
                    If .ConditionLifetime("混乱") > 0 Then
                        .DeleteCondition "混乱"
                    End If
                    If .ConditionLifetime("魅了") > 0 Then
                        .DeleteCondition "魅了"
                    End If
                    If .ConditionLifetime("憑依") > 0 Then
                        .DeleteCondition "憑依"
                    End If
                    If .ConditionLifetime("石化") > 0 Then
                        .DeleteCondition "石化"
                    End If
                    If .ConditionLifetime("凍結") > 0 Then
                        .DeleteCondition "凍結"
                    End If
                    If .ConditionLifetime("麻痺") > 0 Then
                        .DeleteCondition "麻痺"
                    End If
                    If .ConditionLifetime("睡眠") > 0 Then
                        .DeleteCondition "睡眠"
                    End If
                    If .ConditionLifetime("毒") > 0 Then
                        .DeleteCondition "毒"
                    End If
                    If .ConditionLifetime("盲目") > 0 Then
                        .DeleteCondition "盲目"
                    End If
                    If .ConditionLifetime("撹乱") > 0 Then
                        .DeleteCondition "撹乱"
                    End If
                    If .ConditionLifetime("恐怖") > 0 Then
                        .DeleteCondition "恐怖"
                    End If
                    If .ConditionLifetime("沈黙") > 0 Then
                        .DeleteCondition "沈黙"
                    End If
                    If .ConditionLifetime("ゾンビ") > 0 Then
                        .DeleteCondition "ゾンビ"
                    End If
                    If .ConditionLifetime("回復不能") > 0 Then
                        .DeleteCondition "回復不能"
                    End If
                    
                    If .ConditionLifetime("オーラ使用不能") > 0 Then
                        .DeleteCondition "オーラ使用不能"
                    End If
                    If .ConditionLifetime("超能力使用不能") > 0 Then
                        .DeleteCondition "超能力使用不能"
                    End If
                    If .ConditionLifetime("同調率使用不能") > 0 Then
                        .DeleteCondition "同調率使用不能"
                    End If
                    If .ConditionLifetime("超感覚使用不能") > 0 Then
                        .DeleteCondition "超感覚使用不能"
                    End If
                    If .ConditionLifetime("知覚強化使用不能") > 0 Then
                        .DeleteCondition "知覚強化使用不能"
                    End If
                    If .ConditionLifetime("霊力使用不能") > 0 Then
                        .DeleteCondition "霊力使用不能"
                    End If
                    If .ConditionLifetime("術使用不能") > 0 Then
                        .DeleteCondition "術使用不能"
                    End If
                    If .ConditionLifetime("技使用不能") > 0 Then
                        .DeleteCondition "技使用不能"
                    End If
                    For j = 1 To .CountCondition
                        If Len(.Condition(j)) > 6 Then
                            '弱、効属性は状態回復から除外。
                            If Right$(.Condition(j), 6) = "属性使用不能" Then
                                If .ConditionLifetime(.Condition(j)) > 0 Then
                                    .DeleteCondition .Condition(j)
                                End If
                            End If
                        End If
                    Next
                    
                    If Not is_event Then
                        DisplaySysMessage .Nickname & "の状態が回復した。"
                    End If
                    
                Case "行動数回復"
                    '効果が適用可能かどうか判定
                    If .Action > 0 Or .MaxAction = 0 Then
                        GoTo NextEffect
                    End If
                    
                    '行動数を回復させる
                    .UsedAction = .UsedAction - 1
                    
                    '他の効果の表示のためにメッセージウィンドウが表示されているので
                    'なければ特にメッセージは表示しない (効果は見れば分かるので)
                    If Not is_event Then
                        If frmMessage.Visible Then
                            If t Is SelectedUnit Then
                                UpdateMessageForm SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                            
                            DisplaySysMessage .Nickname & "は行動可能になった。"
                        End If
                    End If
                    
                Case "行動数増加"
                    '効果が適用可能かどうか判定
                    If .Action > 3 Or .MaxAction = 0 Then
                        GoTo NextEffect
                    End If
                    
                    '行動数を増やす
                    .UsedAction = .UsedAction - 1
                    
                    If Not is_event Then
                        If t Is SelectedUnit Then
                            If Not frmMessage.Visible Then
                                OpenMessageForm SelectedUnit
                            Else
                                UpdateMessageForm SelectedUnit
                            End If
                        Else
                            If Not frmMessage.Visible Then
                                OpenMessageForm t, SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                        End If
                        
                        DisplaySysMessage .Nickname & "は" & _
                            StrConv(Format$(.Action), vbWide) & "回行動可能になった。"
                    End If
                    
                Case "スペシャルパワー", "精神コマンド"
                    If SPDList.IsDefined(EffectData(i)) Then
                        .MakeSpecialPowerInEffect EffectData(i), my_unit.MainPilot.ID
                    Else
                        ErrorMessage "スペシャルパワー「" & Name _
                            & "」で使われているスペシャルパワー「" _
                            & EffectData(i) & "」は定義されていません。"
                    End If
                    
                Case "気力増加"
                    If .MainPilot.Personality = "機械" Then
                        GoTo NextEffect
                    End If
                    If .MainPilot.Morale = .MainPilot.MaxMorale Then
                        GoTo NextEffect
                    End If
                    
                    '気力を増加させる
                    tmp = .MainPilot.Morale
                    .IncreaseMorale 10 * EffectLevel(i)
                    
                    If Not is_event Then
                        If Not displayed_string Then
                            DrawSysString .X, .Y, "+" & Format$(.MainPilot.Morale - tmp)
                        End If
                        displayed_string = True
                    End If
                    
                    need_update = True
                    
                Case "気力低下"
                    '効果が適用可能かどうか判定
                    If .MainPilot.Personality = "機械" Then
                        GoTo NextEffect
                    End If
                    If .MainPilot.Morale = .MainPilot.MinMorale Then
                        GoTo NextEffect
                    End If
                    
                    '気力を低下させる
                    tmp = .MainPilot.Morale
                    .IncreaseMorale -10 * EffectLevel(i)
                    
                    If Not is_event Then
                        If TargetType = "敵" Or TargetType = "全敵" Then
                            If Not displayed_string Then
                                DrawSysString .X, .Y, Format$(.MainPilot.Morale - tmp)
                                displayed_string = True
                            End If
                        End If
                    End If
                    
                    need_update = True
                    
                Case "ランダムダメージ"
                    '効果が適用可能かどうか判定
                    If .IsConditionSatisfied("無敵") Then
                        GoTo NextEffect
                    End If
                    
                    If Not is_event Then
                        If t Is SelectedUnit Then
                            If Not frmMessage.Visible Then
                                OpenMessageForm SelectedUnit
                            Else
                                UpdateMessageForm SelectedUnit
                            End If
                        Else
                            If Not frmMessage.Visible Then
                                OpenMessageForm t, SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                        End If
                    End If
                    
                    'ダメージを与える
                    tmp = .HP
                    .HP = MaxLng(.HP - 10 * Dice(10 * EffectLevel(i)), 10)
                    If TargetType = "全敵" Then
                        Sleep 150
                    End If
                    
                    '特殊能力「不安定」による暴走チェック
                    If .IsFeatureAvailable("不安定") Then
                        If .HP <= .MaxHP \ 4 _
                            And Not .IsConditionSatisfied("暴走") _
                        Then
                            .AddCondition "暴走", -1
                        End If
                    End If
                    
                    If Not is_event Then
                        If Not displayed_string Then
                            DrawSysString .X, .Y, Format$(tmp - .HP)
                        End If
                        displayed_string = True
                        
                        If t Is SelectedUnit Then
                            UpdateMessageForm SelectedUnit
                        Else
                            UpdateMessageForm t, SelectedUnit
                        End If
                        
                        DisplaySysMessage .Nickname & "に" & Format$(tmp - .HP) _
                            & "のダメージを与えた。"
                    End If
                    
                    need_update = True
                    
                Case "ＨＰ減少"
                    '効果が適用可能かどうか判定
                    If .IsConditionSatisfied("無敵") Then
                        GoTo NextEffect
                    End If
                    
                    If Not is_event Then
                        If t Is SelectedUnit Then
                            If Not frmMessage.Visible Then
                                OpenMessageForm SelectedUnit
                            Else
                                UpdateMessageForm SelectedUnit
                            End If
                        Else
                            If Not frmMessage.Visible Then
                                OpenMessageForm t, SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                        End If
                    End If
                    
                    'ＨＰを減少させる
                    tmp = .HP
                    .HP = .HP - .HP * EffectLevel(i) \ 10
                    If TargetType = "全敵" Then
                        Sleep 150
                    End If
                    
                    '特殊能力「不安定」による暴走チェック
                    If .IsFeatureAvailable("不安定") Then
                        If .HP <= .MaxHP \ 4 _
                            And Not .IsConditionSatisfied("暴走") _
                        Then
                            .AddCondition "暴走", -1
                        End If
                    End If
                    
                    If Not is_event Then
                        If Not displayed_string Then
                            DrawSysString .X, .Y, Format$(tmp - .HP)
                        End If
                        displayed_string = True
                        
                        If t Is SelectedUnit Then
                            UpdateMessageForm SelectedUnit
                        Else
                            UpdateMessageForm t, SelectedUnit
                        End If
                        
                        If SelectedUnit Is t Then
                            DisplaySysMessage .Nickname & "の" & Term("ＨＰ", t) & "が" _
                                & Format$(tmp - .HP) & "減少した。"
                        Else
                            DisplaySysMessage .Nickname & "の" & Term("ＨＰ", t) & "を" _
                                & Format$(tmp - .HP) & "減少させた。"
                        End If
                    End If
                    
                    need_update = True
                    
                Case "ＥＮ減少"
                    '効果が適用可能かどうか判定
                    If .IsConditionSatisfied("無敵") Then
                        GoTo NextEffect
                    End If
                    
                    If Not is_event Then
                        If t Is SelectedUnit Then
                            If Not frmMessage.Visible Then
                                OpenMessageForm SelectedUnit
                            Else
                                UpdateMessageForm SelectedUnit
                            End If
                        Else
                            If Not frmMessage.Visible Then
                                OpenMessageForm t, SelectedUnit
                            Else
                                UpdateMessageForm t, SelectedUnit
                            End If
                        End If
                    End If
                    
                    'ＥＮを減少させる
                    tmp = .EN
                    .EN = .EN - .EN * EffectLevel(i) \ 10
                    If TargetType = "全敵" Then
                        Sleep 150
                    End If
                    
                    If Not displayed_string Then
                        DrawSysString .X, .Y, Format$(tmp - .EN)
                    End If
                    displayed_string = True
                    
                    If t Is SelectedUnit Then
                        UpdateMessageForm SelectedUnit
                    Else
                        UpdateMessageForm t, SelectedUnit
                    End If
                    
                    If SelectedUnit Is t Then
                        DisplaySysMessage .Nickname & "の" & Term("ＥＮ", t) & "が" _
                            & Format$(tmp - .EN) & "減少した。"
                    Else
                        DisplaySysMessage .Nickname & "の" & Term("ＥＮ", t) & "を" _
                            & Format$(tmp - .EN) & "減少させた。"
                    End If
                    
                    need_update = True
                    
                Case "偵察"
                    '未識別のユニットは識別しておく
                    If IsOptionDefined("ユニット情報隠蔽") Then
                        If Not .IsConditionSatisfied("識別済み") Then
                            .AddCondition "識別済み", -1, 0, "非表示"
                            DisplayUnitStatus t
                        End If
                    End If
                    If .IsConditionSatisfied("ユニット情報隠蔽") Then
                        .DeleteCondition "ユニット情報隠蔽"
                        DisplayUnitStatus t
                    End If
                    
                    If Not frmMessage.Visible Then
                        OpenMessageForm
                    End If
                    DisplayMessage "システム", _
                          Term("ＨＰ", t, 6) & "：" _
                            & Format$(.HP) & "/" & Format$(.MaxHP) & ";" _
                        & Term("ＥＮ", t, 6) & "：" _
                            & Format$(.EN) & "/" & Format$(.MaxEN) & ";" _
                        & Term("資金", t, 6) & "：" _
                            & Format$(.Value \ 2) & ";" _
                        & "経験値：" _
                            & Format$(.ExpValue + .MainPilot.ExpValue)
                    If .IsFeatureAvailable("アイテム所有") Then
                        If IDList.IsDefined(.FeatureData("アイテム所有")) Then
                            msg = IDList.Item(.FeatureData("アイテム所有")).Nickname _
                                & "を盗むことが出来る。;"
                        Else
                            ErrorMessage .Name & "の所有アイテム「" _
                                & .FeatureData("アイテム所有") _
                                & "」のデータが見つかりません"
                        End If
                    End If
                    If .IsFeatureAvailable("レアアイテム所有") Then
                        If IDList.IsDefined(.FeatureData("レアアイテム所有")) Then
                            If Len(msg) > 0 Then
                                msg = msg & "また、"
                            End If
                            msg = msg & "まれに" _
                                & IDList.Item(.FeatureData("レアアイテム所有")).Nickname _
                                & "を盗むことが出来る。;"
                         Else
                            ErrorMessage .Name & "の所有レアアイテム「" _
                                & .FeatureData("レアアイテム所有") _
                                & "」のデータが見つかりません"
                         End If
                    End If
                    If .IsFeatureAvailable("ラーニング可能技") Then
                        msg = msg & "「" & .FeatureData("ラーニング可能技") _
                            & "」をラーニング可能。"
                    End If
                    If Len(msg) > 0 Then
                        DisplayMessage "システム", msg
                    End If
                    
                Case "自爆"
                    OpenMessageForm t
                    .SuicidalExplosion
                    Exit Function
                    
                Case "復活"
                    If Duration = "破壊" Then
                        '破壊直後に復活する場合
                        .HP = .MaxHP
                    Else
                        '破壊後に他のパイロットの力で復活する場合
                        
                        '復活時は通常形態に戻る
                        If .IsFeatureAvailable("ノーマルモード") Then
                            .Transform LIndex(.FeatureData("ノーマルモード"), 1)
                            Set t = .CurrentForm
                            n = 0
                        Else
                            n = .ConditionLifetime("残り時間")
                            
                            '後のRestで残り時間が0にならないように一旦時間を巻き戻す
                            If n > 0 Then
                                .AddCondition "残り時間", 10
                            End If
                        End If
                        
                        'ユニットを復活させる
                        With t
                            .FullRecover
                            .UsedAction = 0
                            .StandBy my_unit.X, my_unit.Y
                            .Rest
                            
                            '残り時間を元に戻す
                            If n > 0 Then
                                .DeleteCondition "残り時間"
                                .AddCondition "残り時間", n
                            End If
                            
                            RedrawScreen
                        End With
                    End If
                    
                    With t
                        If Not frmMessage.Visible Then
                            OpenMessageForm
                        End If
                        If .IsMessageDefined("復活") Then
                            .PilotMessage "復活"
                        End If
                        If .IsAnimationDefined("復活") Then
                            .PlayAnimation "復活"
                        Else
                            .SpecialEffect "復活"
                        End If
                        DisplaySysMessage .Nickname & "は復活した。"
                    End With
                    
                Case "イベント"
                    'イベントコマンドで定義されたスペシャルパワー
                    '対象ユニットＩＤ及び相手ユニットＩＤを設定
                    Set SelectedUnitForEvent = my_unit.CurrentForm
                    Set SelectedTargetForEvent = .CurrentForm
                    '指定されたサブルーチンを実行
                    GetValueAsString "Call(" & EffectData(i) & ")"
            End Select
        End With
NextEffect:
    Next
    
    'Unitが変化してしまった場合は元に戻しておく
    If Not my_unit Is p.Unit Then
        my_unit.CurrentForm.MainPilot
    End If
    
    'ステータスの更新が必要？
    If need_update Then
        With t
            .CheckAutoHyperMode
            .CurrentForm.CheckAutoNormalMode
            .CurrentForm.Update
            PList.UpdateSupportMod .CurrentForm
        End With
    End If
    
    Apply = displayed_string
End Function


'スペシャルパワーが有効なターゲットの総数を返す
'(パイロット p が使用した場合)
Public Function CountTarget(p As Pilot) As Integer
Dim u As Unit, i As Integer

    Select Case TargetType
        Case "自分"
            If Effective(p, p.Unit) Then
                CountTarget = 1
            End If
            
        Case "味方", "全味方"
            For Each u In UList
                With u
                    '出撃している？
                    If .Status <> "出撃" Then
                        GoTo NextUnit1
                    End If
                    
                    '味方ユニット？
                    Select Case p.Party
                        Case "味方", "ＮＰＣ"
                            If .Party <> "味方" _
                                And .Party0 <> "味方" _
                                And .Party <> "ＮＰＣ" _
                                And .Party0 <> "ＮＰＣ" _
                            Then
                                GoTo NextUnit1
                            End If
                        Case Else
                            If p.Party <> .Party Then
                                GoTo NextUnit1
                            End If
                    End Select
                    
                    '効果がある？
                    If Effective(p, u) Then
                        CountTarget = CountTarget + 1
                    End If
                End With
NextUnit1:
            Next
            
        Case "破壊味方"
            For Each u In UList
                With u
                    '破壊されている？
                    If .Status <> "破壊" Then
                        GoTo NextUnit2
                    End If
                    
                    '味方ユニット？
                    If p.Party <> .Party0 Then
                        GoTo NextUnit2
                    End If
                    
                    '効果がある？
                    If Effective(p, u) Then
                        CountTarget = CountTarget + 1
                    End If
                End With
NextUnit2:
            Next
            
        Case "敵", "全敵"
            For Each u In UList
                With u
                    '出撃している？
                    If .Status <> "出撃" Then
                        GoTo NextUnit3
                    End If
                    
                    '敵ユニット？
                    Select Case p.Party
                        Case "味方", "ＮＰＣ"
                            If (.Party = "味方" _
                                   And .Party0 = "味方") _
                               Or (.Party = "ＮＰＣ" _
                                   And .Party0 = "ＮＰＣ") _
                            Then
                                GoTo NextUnit3
                            End If
                        Case Else
                            If p.Party = .Party Then
                                GoTo NextUnit3
                            End If
                    End Select
                    
                    '効果がある？
                    If Effective(p, u) Then
                        CountTarget = CountTarget + 1
                    End If
                End With
NextUnit3:
            Next
            
        Case "任意", "全"
            For Each u In UList
                '出撃している？
                If u.Status = "出撃" Then
                    '効果がある？
                    If Effective(p, u) Then
                        CountTarget = CountTarget + 1
                    End If
                End If
            Next
    End Select
End Function

'スペシャルパワーのアニメーションを表示
Public Function PlayAnimation() As Boolean
Dim anime As String, animes() As String, anime_head As Integer
Dim buf As String, ret As Double, i As Integer, j As Integer
Dim expr As String
Dim wait_time As Long
Dim prev_obj_color As Long
Dim prev_obj_fill_color As Long
Dim prev_obj_fill_style As Long
Dim prev_obj_draw_width As Long
Dim prev_obj_draw_option As String

    If Not SpecialPowerAnimation Then
        Exit Function
    End If
    
    If Animation = "-" Then
        PlayAnimation = True
        Exit Function
    End If
    
    'アニメ指定がなされていない場合はアニメ表示用サブルーチンが見つらなければそのまま終了
    If Animation = Name Then
        If FindNormalLabel("ＳＰアニメ_" & Animation) = 0 Then
            If Name <> "自爆" _
                And Name <> "祈り" _
            Then
                If IsLabelDefined("特殊効果 " & Name) Then
                    HandleEvent "特殊効果", Name
                    PlayAnimation = True
                End If
            End If
            Exit Function
        End If
    End If
    
    '右クリック中はアニメ表示をスキップ
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
    
    'オブジェクト色等をデフォルトに戻す
    ObjColor = vbWhite
    ObjFillColor = vbWhite
    ObjFillStyle = vbFSTransparent
    ObjDrawWidth = 1
    ObjDrawOption = ""
    
    'アニメ指定を分割
    ReDim animes(1)
    anime_head = 1
    For i = 1 To Len(Animation)
        If Mid$(Animation, i, 1) = ";" Then
            animes(UBound(animes)) = Mid$(Animation, anime_head, i - anime_head)
            ReDim Preserve animes(UBound(animes) + 1)
            anime_head = i + 1
        End If
    Next
    animes(UBound(animes)) = Mid$(Animation, anime_head)
    
    On Error GoTo ErrorHandler
    
    For i = 1 To UBound(animes)
        anime = animes(i)
        
        '式評価
        FormatMessage anime
        
        '画面クリア？
        If LCase$(anime) = "clear" Then
            ClearPicture
            MainForm.picMain(0).Refresh
            GoTo NextAnime
        End If
        
        '戦闘アニメ以外の特殊効果
        Select Case LCase$(Right$(LIndex(anime, 1), 4))
            Case ".wav", ".mp3"
                '効果音
                PlayWave anime
                If wait_time > 0 Then
                    Sleep wait_time
                    wait_time = 0
                End If
                GoTo NextAnime
                
            Case ".bmp", ".jpg", ".gif", ".png"
                'カットインの表示
                If wait_time > 0 Then
                    anime = Format$(wait_time / 100) & ";" & anime
                    wait_time = 0
                End If
                DisplayBattleMessage "", anime
                GoTo NextAnime
        End Select
        
        Select Case LCase$(LIndex(anime, 1))
            Case "line", "circle", "arc", "oval", _
                "color", "fillcolor", "fillstyle", "drawwidth"
                '画面処理コマンド
                If wait_time > 0 Then
                    anime = Format$(wait_time / 100) & ";" & anime
                    wait_time = 0
                End If
                DisplayBattleMessage "", anime
                GoTo NextAnime
            Case "center"
                '指定したユニットを中央表示
                buf = GetValueAsString(ListIndex(anime, 2))
                If UList.IsDefined(buf) Then
                    With UList.Item(buf)
                        Center .X, .Y
                        RedrawScreen
                    End With
                End If
                GoTo NextAnime
        End Select
        
        'ウェイト？
        If IsNumeric(anime) Then
            wait_time = 100 * CDbl(anime)
            GoTo NextAnime
        End If
        
        'サブルーチンの呼び出しが確定
        
        '戦闘アニメ再生前にウェイトを入れる？
        If wait_time > 0 Then
            Sleep wait_time
            wait_time = 0
        End If
        
        'サブルーチン呼び出しのための式を作成
        If Left$(anime, 1) = "@" Then
            expr = Mid$(ListIndex(anime, 1), 2) & "("
            '引数の構築
            For j = 2 To ListLength(anime)
                If j > 2 Then
                    expr = expr & ","
                End If
                expr = expr & ListIndex(anime, j)
            Next
            expr = expr & ")"
        ElseIf Not SelectedTarget Is Nothing Then
            expr = "ＳＰアニメ_" & anime & "(" & SelectedUnit.ID & "," & SelectedTarget.ID & ")"
        Else
            expr = "ＳＰアニメ_" & anime & "(" & SelectedUnit.ID & ",-)"
        End If
        
        '画像描画が行われたかどうかの判定のためにフラグを初期化
        IsPictureDrawn = False
        
        'アニメ再生
        SaveBasePoint
        CallFunction expr, StringType, buf, ret
        RestoreBasePoint
        
        '画像を消去しておく
        If IsPictureDrawn And LCase$(buf) <> "keep" Then
            ClearPicture
            MainForm.picMain(0).Refresh
        End If
        
NextAnime:
    Next
    
    '戦闘アニメ再生後にウェイトを入れる？
    If wait_time > 0 Then
        Sleep wait_time
        wait_time = 0
    End If
    
    'メッセージウィンドウを閉じる
    CloseMessageForm
    
    'オブジェクト色等を元に戻す
    ObjColor = prev_obj_color
    ObjFillColor = prev_obj_fill_color
    ObjFillStyle = prev_obj_fill_style
    ObjDrawWidth = prev_obj_draw_width
    ObjDrawOption = prev_obj_draw_option
    
    PlayAnimation = True
    Exit Function
    
ErrorHandler:
    
    'アニメ再生中に発生したエラーの処理
    If Len(EventErrorMessage) > 0 Then
        DisplayEventErrorMessage CurrentLineNum, EventErrorMessage
        EventErrorMessage = ""
    Else
        DisplayEventErrorMessage CurrentLineNum, ""
    End If
End Function

