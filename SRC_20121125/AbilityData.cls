VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AbilityData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'アビリティデータのクラス

'名称
Public Name As String
'使用可能回数
Public Stock As Integer
'ＥＮ消費量
Public ENConsumption As Integer
'必要気力
Public NecessaryMorale As Integer
'最小射程
Public MinRange As Integer
'最大射程
Public MaxRange As Integer
'属性
Public Class As String
'必要技能
Public NecessarySkill As String
'必要条件
Public NecessaryCondition As String

'効果
Private colEffects As New Collection

'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer
    
    With colEffects
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colEffects = Nothing
End Sub

'アビリティ愛称
Public Function Nickname() As String
    Nickname = Name
    ReplaceSubExpression Nickname
    If InStr(Nickname, "(") > 0 Then
        Nickname = Left$(Nickname, InStr(Nickname, "(") - 1)
    End If
End Function

'アビリティに効果を追加
Public Sub SetEffect(elist As String)
Dim i As Integer, j As Integer, k As Integer
Dim buf As String
Dim dat As AbilityEffect, dat2 As AbilityEffect
Dim etype As String, elevel As String, edata As String

    TrimString elist
    For i = 1 To ListLength(elist)
        Set dat = NewAbilityEffect
        With dat
            buf = ListIndex(elist, i)
            j = InStr(buf, "Lv")
            k = InStr(buf, "=")
            If j > 0 And (k = 0 Or j < k) Then
                'レベル指定のある効果(データ指定があるものを含む)
                .Name = Left$(buf, j - 1)
                If k > 0 Then
                    'データ指定があるもの
                    .Level = CDbl(Mid$(buf, j + 2, k - (j + 2)))
                    buf = Mid$(buf, k + 1)
                    If Left$(buf, 1) = """" Then
                        buf = Mid$(buf, 2, Len(buf) - 2)
                    End If
                    
                    j = InStr(buf, "Lv")
                    k = InStr(buf, "=")
                    
                    If j > 0 And (k = 0 Or j < k) Then
                        'データ指定内にレベル指定がある
                        etype = Left$(buf, j - 1)
                        If k > 0 Then
                            elevel = Mid$(buf, j + 2, k - (j + 2))
                            edata = Mid$(buf, k + 1)
                        Else
                            elevel = Mid$(buf, j + 2)
                            edata = ""
                        End If
                    ElseIf k > 0 Then
                        'データ指定内にデータ指定がある
                        etype = Left$(buf, k - 1)
                        elevel = ""
                        edata = Mid$(buf, k + 1)
                    Else
                        '単純なデータ指定
                        etype = buf
                        elevel = ""
                        edata = ""
                    End If
                    
                    If .Name = "付加" And elevel = "" Then
                        elevel = Format$(DEFAULT_LEVEL)
                    End If
                    
                    .Data = Trim$(etype & " " & elevel & " " & edata)
                Else
                    'データ指定がないもの
                    .Level = CDbl(Mid$(buf, j + 2))
                End If
            ElseIf k > 0 Then
                'データ指定を含む効果
                .Name = Left$(buf, k - 1)
                buf = Mid$(buf, k + 1)
                If Asc(buf) = 34 Then '"
                    buf = Mid$(buf, 2, Len(buf) - 2)
                End If
                
                j = InStr(buf, "Lv")
                k = InStr(buf, "=")
                
                If .Name = "解説" Then
                    '解説の指定
                    etype = buf
                    elevel = ""
                    edata = ""
                ElseIf j > 0 Then
                    'データ指定内にレベル指定がある
                    etype = Left$(buf, j - 1)
                    If k > 0 Then
                        elevel = Mid$(buf, j + 2, k - (j + 2))
                        edata = Mid$(buf, k + 1)
                    Else
                        elevel = Mid$(buf, j + 2)
                        edata = ""
                    End If
                ElseIf k > 0 Then
                    'データ指定内にデータ指定がある
                    etype = Left$(buf, k - 1)
                    elevel = ""
                    edata = Mid$(buf, k + 1)
                Else
                    '単純なデータ指定
                    etype = buf
                    elevel = ""
                    edata = ""
                End If
                
                If .Name = "付加" And elevel = "" Then
                    elevel = Format$(DEFAULT_LEVEL)
                End If
                
                .Data = Trim$(etype & " " & elevel & " " & edata)
            Else
                '効果名のみ
                .Name = buf
            End If
            
            j = 1
            For Each dat2 In colEffects
                If .Name = dat2.Name Then
                    j = j + 1
                End If
            Next
            If j = 1 Then
                colEffects.Add dat, .Name
            Else
                colEffects.Add dat, .Name & Format$(j)
            End If
        End With
    Next
End Sub

Private Function NewAbilityEffect() As AbilityEffect
Dim dat As New AbilityEffect
    Set NewAbilityEffect = dat
End Function

'効果の総数
Public Function CountEffect() As Integer
    CountEffect = colEffects.Count
End Function

'効果の種類
Public Function EffectType(Index As Variant) As String
    EffectType = colEffects.Item(Index).Name
End Function

'効果のレベル
Public Function EffectLevel(Index As Variant) As Double
    EffectLevel = colEffects.Item(Index).Level
End Function

'効果のデータ
Public Function EffectData(Index As Variant) As String
    EffectData = colEffects.Item(Index).Data
End Function

'効果内容の解説
Public Function EffectName(Index As Variant) As String
Dim ae As AbilityEffect, elevel As Double, elevel2 As Double
Dim uname As String, wclass As String, flevel As Double
Dim heal_lv As Double, supply_lv As Double
Dim i As Integer, buf As String
Dim cname As String, aname As String
    
    Set ae = colEffects.Item(Index)
    With ae
        '効果レベルが回復・増加量を意味するアビリティ用
        elevel = .Level
        '効果レベルがターン数を意味するアビリティ及び召喚アビリティ用
        elevel2 = elevel
        
        If SelectedUnit.CountPilot > 0 Then
            With SelectedUnit.MainPilot
                '得意技
                If .IsSkillAvailable("得意技") Then
                    buf = .SkillData("得意技")
                    For i = 1 To Len(buf)
                        If InStrNotNest(Class, GetClassBundle(buf, i)) > 0 Then
                            elevel = 1.2 * elevel
                            elevel2 = 1.4 * elevel2
                            Exit For
                        End If
                    Next
                End If
                
                '不得手
                If .IsSkillAvailable("不得手") Then
                    buf = .SkillData("不得手")
                    For i = 1 To Len(buf)
                        If InStrNotNest(Class, GetClassBundle(buf, i)) > 0 Then
                            elevel = 0.8 * elevel
                            elevel2 = 0.6 * elevel2
                            Exit For
                        End If
                    Next
                End If
                
                '術アビリティの場合は魔力によって効果レベルが修正を受ける
                If InStrNotNest(Class, "術") > 0 Then
                    elevel = elevel * .Shooting / 100
                Else
                    For i = 1 To LLength(NecessarySkill)
                        If .SkillType(LIndex(NecessarySkill, i)) = "術" Then
                            elevel = elevel * .Shooting / 100
                            Exit For
                        End If
                    Next
                End If
                
                '修理＆補給技能
                heal_lv = .SkillLevel("修理")
                supply_lv = .SkillLevel("補給")
            End With
        End If
        
        'アビリティの効果は最低でも１ターン持続
        If elevel2 <> 0 Then
            elevel2 = MaxLng(elevel2, 1)
        End If
        
        Select Case .Name
            Case "回復"
                EffectName = Term("ＨＰ") & "を"
                If elevel > 0 Then
                    EffectName = EffectName & Format$(CLng(500 * elevel * (10 + heal_lv) \ 10)) & "回復"
                ElseIf elevel < 0 Then
                    EffectName = EffectName & Format$(CLng(-500 * elevel)) & "減少"
                End If
                
            Case "補給"
                EffectName = Term("ＥＮ") & "を"
                If elevel > 0 Then
                    EffectName = EffectName & Format$(CLng(50 * elevel * (10 + supply_lv)) \ 10) & "回復"
                ElseIf elevel < 0 Then
                    EffectName = EffectName & Format$(CLng(-50 * elevel)) & "減少"
                End If
                
            Case "霊力回復", "プラーナ回復"
                If SelectedUnit.CountPilot > 0 Then
                    EffectName = SelectedUnit.MainPilot.SkillName0("霊力")
                Else
                    EffectName = "霊力"
                End If
                If elevel > 0 Then
                    EffectName = EffectName & "を" & Format$(CLng(10 * elevel)) & "回復"
                ElseIf elevel < 0 Then
                    EffectName = EffectName & "を" & Format$(CLng(-10 * elevel)) & "減少"
                End If
                
            Case "ＳＰ回復"
                EffectName = Term("ＳＰ") & "を"
                If elevel > 0 Then
                    EffectName = EffectName & Format$(CLng(10 * elevel)) & "回復"
                ElseIf elevel < 0 Then
                    EffectName = EffectName & Format$(CLng(-10 * elevel)) & "減少"
                End If
                
            Case "気力増加"
                EffectName = Term("気力") & "を"
                If elevel > 0 Then
                    EffectName = EffectName & Format$(CLng(10 * elevel)) & "増加"
                ElseIf elevel < 0 Then
                    EffectName = EffectName & Format$(CLng(-10 * elevel)) & "減少"
                End If
                
            Case "装填"
                If Len(.Data) = 0 Then
                    EffectName = "武器の弾数を回復"
                Else
                    For i = 1 To SelectedUnit.CountWeapon
                        If SelectedUnit.WeaponNickname(i) = .Data Then
                            EffectName = .Data & "の弾数を回復"
                            Exit Function
                        End If
                    Next
                    EffectName = .Data & "属性を持つ武器の弾数を回復"
                End If
                
            Case "治癒"
                For i = 1 To LLength(.Data)
                    cname = LIndex(.Data, i)
                    Select Case cname
                        Case "装甲劣化"
                            cname = Term("装甲") & "劣化"
                        Case "運動性ＵＰ"
                            cname = Term("運動性") & "ＵＰ"
                        Case "運動性ＤＯＷＮ"
                            cname = Term("運動性") & "ＤＯＷＮ"
                        Case "移動力ＵＰ"
                            cname = Term("移動力") & "ＵＰ"
                        Case "移動力ＤＯＷＮ"
                            cname = Term("移動力") & "ＤＯＷＮ"
                    End Select
                    EffectName = EffectName & " " & cname
                Next
                EffectName = Trim$(EffectName)
                If Len(EffectName) > 0 Then
                    EffectName = EffectName & "を回復"
                Else
                    EffectName = "状態回復"
                End If
                
            Case "状態"
                cname = LIndex(.Data, 1)
                Select Case cname
                    Case "装甲劣化"
                        cname = Term("装甲") & "劣化"
                    Case "運動性ＵＰ"
                        cname = Term("運動性") & "ＵＰ"
                    Case "運動性ＤＯＷＮ"
                        cname = Term("運動性") & "ＤＯＷＮ"
                    Case "移動力ＵＰ"
                        cname = Term("移動力") & "ＵＰ"
                    Case "移動力ＤＯＷＮ"
                        cname = Term("移動力") & "ＤＯＷＮ"
                End Select
                If 0 < elevel2 And elevel2 <= 10 Then
                    EffectName = cname _
                        & "(" & Format$(CLng(elevel2)) & "ターン)"
                Else
                    EffectName = cname
                End If
                
            Case "付加"
                Select Case LIndex(.Data, 1)
                    Case "耐性"
                        aname = AttributeName(SelectedUnit, LIndex(.Data, 3))
                        If aname = "" Then
                            aname = LIndex(.Data, 3) & "攻撃"
                        End If
                        EffectName = aname & "のダメージを半減"
                    Case "無効化"
                        aname = AttributeName(SelectedUnit, LIndex(.Data, 3))
                        If aname = "" Then
                            aname = LIndex(.Data, 3) & "攻撃"
                        End If
                        EffectName = aname & "を無効化"
                    Case "特殊効果無効化"
                        aname = AttributeName(SelectedUnit, LIndex(.Data, 3))
                        If aname = "" Then
                            aname = LIndex(.Data, 3) & "攻撃"
                        End If
                        EffectName = aname & "の特殊効果を無効化"
                    Case "吸収"
                        aname = AttributeName(SelectedUnit, LIndex(.Data, 3))
                        If aname = "" Then
                            aname = LIndex(.Data, 3) & "攻撃"
                        End If
                        EffectName = aname & "を吸収"
                    Case "追加パイロット"
                        EffectName = "パイロット変化"
                    Case "追加サポート"
                        EffectName = "サポート追加"
                    Case "性格変更"
                        EffectName = "パイロットの性格を" & LIndex(.Data, 3) & "に変更"
                    Case "ＢＧＭ"
                        EffectName = "ＢＧＭ変更"
                    Case "攻撃属性"
                        For i = 4 To LLength(.Data)
                            If InStr(LIndex(.Data, i), "!") = 0 Then
                                wclass = wclass & LIndex(.Data, i)
                            End If
                        Next
                        EffectName = WeaponType(wclass) & "の属性に" _
                            & LIndex(.Data, 3) & "を追加"
                    Case "武器強化"
                        For i = 3 To LLength(.Data)
                            If InStr(LIndex(.Data, i), "!") = 0 Then
                                wclass = wclass & LIndex(.Data, i)
                            End If
                        Next
                        flevel = StrToDbl(LIndex(.Data, 2))
                        If flevel >= 0 Then
                            EffectName = WeaponType(wclass) & "の攻撃力を+" _
                                & Format$(100 * flevel)
                        Else
                            EffectName = WeaponType(wclass) & "の攻撃力を" _
                                & Format$(100 * flevel)
                        End If
                    Case "命中率強化"
                        For i = 3 To LLength(.Data)
                            If InStr(LIndex(.Data, i), "!") = 0 Then
                                wclass = wclass & LIndex(.Data, i)
                            End If
                        Next
                        flevel = StrToDbl(LIndex(.Data, 2))
                        If flevel >= 0 Then
                            EffectName = WeaponType(wclass) & "の命中率を+" _
                                & Format$(5 * flevel)
                        Else
                            EffectName = WeaponType(wclass) & "の命中率を" _
                                & Format$(5 * flevel)
                        End If
                    Case "ＣＴ率強化", "特殊効果発動率強化"
                        For i = 3 To LLength(.Data)
                            If InStr(LIndex(.Data, i), "!") = 0 Then
                                wclass = wclass & LIndex(.Data, i)
                            End If
                        Next
                        flevel = StrToDbl(LIndex(.Data, 2))
                        If flevel >= 0 Then
                            EffectName = WeaponType(wclass) & "のＣＴ率を+" _
                                & Format$(5 * flevel)
                        Else
                            EffectName = WeaponType(wclass) & "のＣＴ率を" _
                                & Format$(5 * flevel)
                        End If
                    Case "射程延長"
                        For i = 3 To LLength(.Data)
                            If InStr(LIndex(.Data, i), "!") = 0 Then
                                wclass = wclass & LIndex(.Data, i)
                            End If
                        Next
                        flevel = StrToLng(LIndex(.Data, 2))
                        If flevel >= 0 Then
                            EffectName = WeaponType(wclass) & "の射程を+" _
                                & Format$(flevel)
                        Else
                            EffectName = WeaponType(wclass) & "の射程を" _
                                & Format$(flevel)
                        End If
                    Case "サイズ変更"
                        EffectName = "サイズが" & LIndex(.Data, 3) & "に変化"
                    Case "地形適応変更"
                        If StrToLng(LIndex(.Data, 3)) > 0 Then
                            EffectName = "空への適応を強化"
                        ElseIf StrToLng(LIndex(.Data, 4)) > 0 Then
                            EffectName = "陸への適応を強化"
                        ElseIf StrToLng(LIndex(.Data, 5)) > 0 Then
                            EffectName = "水中への適応を強化"
                        ElseIf StrToLng(LIndex(.Data, 6)) > 0 Then
                            EffectName = "宇宙への適応を強化"
                        End If
                    Case "地形適応固定変更"
                        If StrToLng(LIndex(.Data, 3)) <= 5 And StrToLng(LIndex(.Data, 3)) >= 0 Then
                            If LIndex(.Data, 6) = "強制" Then
                                EffectName = "空への適応を強制的に変化"
                            Else
                                EffectName = "空への適応を変化"
                            End If
                        ElseIf StrToLng(LIndex(.Data, 4)) <= 5 And StrToLng(LIndex(.Data, 4)) >= 0 Then
                            If LIndex(.Data, 6) = "強制" Then
                                EffectName = "陸への適応を強制的に変化"
                            Else
                                EffectName = "陸への適応を変化"
                            End If
                        ElseIf StrToLng(LIndex(.Data, 5)) <= 5 And StrToLng(LIndex(.Data, 5)) >= 0 Then
                            If LIndex(.Data, 6) = "強制" Then
                                EffectName = "水中への適応を強制的に変化"
                            Else
                                EffectName = "水中への適応を変化"
                            End If
                        ElseIf StrToLng(LIndex(.Data, 6)) <= 5 And StrToLng(LIndex(.Data, 6)) >= 0 Then
                            If LIndex(.Data, 6) = "強制" Then
                                EffectName = "宇宙への適応を強制的に変化"
                            Else
                                EffectName = "宇宙への適応を変化"
                            End If
                        End If
                    Case "Ｖ－ＵＰ"
                        Select Case LIndex(.Data, 3)
                            Case "武器"
                                EffectName = "武器攻撃力を強化"
                            Case "ユニット"
                                EffectName = "各パラメータを強化"
                            Case Else
                                EffectName = "ユニットを強化"
                        End Select
                    Case "格闘武器", "迎撃武器", "制限時間"
                        EffectName = EffectName & "付加"
                    Case "パイロット愛称", "パイロット画像", "愛称変更", "ユニット画像"
                        EffectName = ""
                    Case Else
                        EffectName = ListIndex(.Data, 3)
                        If Left$(EffectName, 1) = """" Then
                            EffectName = ListIndex(Mid$(EffectName, 2, Len(EffectName) - 2), 1)
                        End If
                        If EffectName = "" Or EffectName = "非表示" Then
                            If LIndex(.Data, 2) <> Format$(DEFAULT_LEVEL) _
                                And LLength(.Data) <= 3 _
                            Then
                                EffectName = LIndex(.Data, 1) & "Lv" _
                                    & LIndex(.Data, 2) & "付加"
                            Else
                                EffectName = LIndex(.Data, 1) & "付加"
                            End If
                        Else
                            If LIndex(.Data, 2) <> Format$(DEFAULT_LEVEL) _
                                And LLength(.Data) <= 3 _
                            Then
                                EffectName = EffectName & "Lv" & LIndex(.Data, 2)
                            End If
                            EffectName = EffectName & "付加"
                        End If
                End Select
                If EffectName <> "" Then
                    If 0 < elevel2 And elevel2 <= 10 Then
                        EffectName = EffectName & "(" & Format$(CLng(elevel2)) & "ターン)"
                    End If
                End If
                
            Case "強化"
                EffectName = ListIndex(.Data, 3)
                If EffectName = "" Or EffectName = "非表示" Then
                    If StrToLng(LIndex(.Data, 2)) > 0 Then
                        EffectName = LIndex(.Data, 1) & "Lv" & LIndex(.Data, 2)
                    Else
                        EffectName = LIndex(.Data, 1)
                    End If
                End If
                If 0 < elevel2 And elevel2 <= 10 Then
                    EffectName = EffectName & "強化(" & Format$(CLng(elevel2)) & "ターン)"
                End If
                
            Case "召喚"
                If Not UDList.IsDefined(.Data) Then
                    ErrorMessage "召喚ユニット「" & .Data & "」が定義されていません"
                    Exit Function
                End If
                If elevel2 > 1 Then
                    EffectName = UDList.Item(.Data).Nickname & "を" _
                        & StrConv(Format$(CLng(elevel2)), vbWide) & "体召喚"
                Else
                    EffectName = UDList.Item(.Data).Nickname & "を召喚"
                End If
                
            Case "変身"
                uname = LIndex(.Data, 1)
                If Not UDList.IsDefined(uname) Then
                    ErrorMessage "変身先のデータ「" & uname & "」が定義されていません"
                    Exit Function
                End If
                If 0 < elevel2 And elevel2 <= 10 Then
                    EffectName = UDList.Item(uname).Nickname & "に変身" _
                        & "(" & Format$(CLng(elevel2)) & "ターン)"
                Else
                    EffectName = UDList.Item(uname).Nickname & "に変身"
                End If
                
            Case "能力コピー"
                If 0 < elevel2 And elevel2 <= 10 Then
                    EffectName = "任意の味方ユニットに変身" _
                        & "(" & Format$(CLng(elevel2)) & "ターン)"
                Else
                    EffectName = "任意の味方ユニットに変身"
                End If
                
            Case "再行動"
                If MaxRange <> 0 Then
                    EffectName = "行動済みユニットを再行動"
                Else
                    EffectName = "行動非消費"
                End If
                
            Case "解説"
                EffectName = .Data
                
        End Select
    End With
End Function

'付加する武器強化系能力の対象表示用に武器の種類を判定
Private Function WeaponType(wclass As String) As String
    If wclass = "全" Or wclass = "" Then
        WeaponType = "武器"
    ElseIf Len(wclass) = 1 Then
        WeaponType = AttributeName(Nothing, wclass)
    ElseIf Right$(wclass, 2) = "装備" Then
        WeaponType = Left$(wclass, Len(wclass) - 2)
    Else
        WeaponType = wclass & "属性攻撃"
    End If
End Function

'使い捨てアイテムによるアビリティかどうかを返す
Public Function IsItem() As Boolean
Dim i As Integer
    
    For i = 1 To LLength(NecessarySkill)
        If LIndex(NecessarySkill, i) = "アイテム" Then
            IsItem = True
            Exit Function
        End If
    Next
End Function

