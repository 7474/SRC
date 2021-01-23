VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Item"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'作成されたアイテムのクラス

'識別子
Public ID As String
'アイテムデータへのポインタ
Public Data As ItemData
'アイテムを装備しているユニット
Public Unit As Unit
'アイテムが存在しているか？ (RemoveItemされていないか？)
Public Exist As Boolean
'アイテムが効力を発揮できているか？ (必要技能や武器クラス＆防具クラスを満たしているか？)
Public Activated As Boolean

'クラスの初期化
Private Sub Class_Initialize()
    Exist = True
    Activated = True
    Set Data = Nothing
    Set Unit = Nothing
End Sub

'クラスの解放
Private Sub Class_Terminate()
    Set Data = Nothing
    Set Unit = Nothing
End Sub

'名称
Public Property Get Name() As String
    Name = Data.Name
End Property

Public Property Let Name(new_name As String)
    Set Data = IDList.Item(new_name)
End Property

'愛称
Public Function Nickname() As String
Dim u As Unit

    Nickname = Data.Nickname
    
    '愛称内の式置換のため、デフォルトユニットを一時的に変更する
    Set u = SelectedUnitForEvent
    Set SelectedUnitForEvent = Unit
    ReplaceSubExpression Nickname
    Set SelectedUnitForEvent = u
End Function

'読み仮名
Public Function KanaName() As String
Dim u As Unit

    KanaName = Data.KanaName
    
    '読み仮名内の式置換のため、デフォルトユニットを一時的に変更する
    Set u = SelectedUnitForEvent
    Set SelectedUnitForEvent = Unit
    ReplaceSubExpression KanaName
    Set SelectedUnitForEvent = u
End Function

'クラス
Public Function Class() As String
    Class = Data.Class
End Function

Public Function Class0() As String
Dim i As Integer, n As Integer
    
    Class0 = Data.Class
    
    '専用指定を削除
    If Right$(Class0, 3) = "専用)" Then
        n = 1
        i = Len(Class0) - 2
        Do
            i = i - 1
            Select Case Mid$(Class0, i, 1)
                Case "("
                    n = n - 1
                    If n = 0 Then
                        Class0 = Left$(Class0, i - 1)
                        Exit Do
                    End If
                Case ")"
                    n = n + 1
            End Select
        Loop While i > 0
    End If
End Function

'装備個所
Public Function Part() As String
    Part = Data.Part
End Function

'ＨＰ修正値
Public Function HP() As Long
    HP = Data.HP
End Function

'ＥＮ修正値
Public Function EN() As Integer
    EN = Data.EN
End Function

'装甲修正値
Public Function Armor() As Long
    Armor = Data.Armor
End Function

'運動性修正値
Public Function Mobility() As Integer
    Mobility = Data.Mobility
End Function

'移動力修正値
Public Function Speed() As Integer
    Speed = Data.Speed
End Function

'特殊能力総数
Public Function CountFeature() As Integer
    CountFeature = Data.CountFeature
End Function

'特殊能力
Public Function Feature(Index As Variant) As String
    Feature = Data.Feature(Index)
End Function

'特殊能力の名称
Public Function FeatureName(Index As Variant) As String
    FeatureName = Data.FeatureName(Index)
End Function

'特殊能力のレベル
Public Function FeatureLevel(Index As Variant) As Double
    FeatureLevel = Data.FeatureLevel(Index)
End Function

'特殊能力のデータ
Public Function FeatureData(Index As Variant) As String
    FeatureData = Data.FeatureData(Index)
End Function

'特殊能力の必要技能
Public Function FeatureNecessarySkill(Index As Variant) As String
    FeatureNecessarySkill = Data.FeatureNecessarySkill(Index)
End Function

'指定した特殊能力を持っているか？
Public Function IsFeatureAvailable(fname As String) As Boolean
    IsFeatureAvailable = Data.IsFeatureAvailable(fname)
End Function

'武器データ
Public Function Weapon(Index As Variant) As WeaponData
    Set Weapon = Data.Weapon(Index)
End Function

'武器の総数
Public Function CountWeapon() As Integer
    CountWeapon = Data.CountWeapon
End Function

'アビリティデータ
Public Function Ability(Index As Variant) As AbilityData
    Set Ability = Data.Ability(Index)
End Function

'アビリティの総数
Public Function CountAbility() As Integer
    CountAbility = Data.CountAbility
End Function

'サイズ(アイテムが消費するアイテムスロット数)
Public Function Size() As Integer
    Size = Data.Size
End Function


'アイテムが使用可能か？
Public Function IsAvailable(u As Unit) As Boolean
Dim i As Integer, j As Integer, k As Integer, iclass As String
Dim sname As String, fdata As String
    
    IsAvailable = False
    
    'イベントコマンド「Disable」
    If IsGlobalVariableDefined("Disable(" & Name & ")") Then
        Exit Function
    End If
    
    '装備個所に適合しているか
    Select Case Part
        Case "片手", "両手", "盾"
            If InStr(u.FeatureData("装備個所"), "腕") = 0 Then
                Exit Function
            End If
        Case "肩", "両肩"
            If InStr(u.FeatureData("装備個所"), "肩") = 0 Then
                Exit Function
            End If
        Case "体"
            If InStr(u.FeatureData("装備個所"), "体") = 0 Then
                Exit Function
            End If
        Case "頭"
            If InStr(u.FeatureData("装備個所"), "頭") = 0 Then
                Exit Function
            End If
    End Select
    
    '武器クラス or 防具クラスに属しているか？
    Select Case Part
        Case "武器", "片手", "両手"
            iclass = u.WeaponProficiency() & " 固定 汎用"
            For i = 1 To LLength(iclass)
                If Class0 = LIndex(iclass, i) Then
                    IsAvailable = True
                    Exit For
                End If
            Next
        Case "盾", "体", "頭"
            iclass = u.ArmorProficiency() & " 固定 汎用"
            For i = 1 To LLength(iclass)
                If Class0 = LIndex(iclass, i) Then
                    IsAvailable = True
                    Exit For
                End If
            Next
        Case Else
            'その他のアイテムは常に利用可能
            IsAvailable = True
    End Select
    
    If Not IsAvailable Then
        Exit Function
    End If
    
    '技能チェックが必要？
    If Not IsFeatureAvailable("必要技能") _
        And Not IsFeatureAvailable("不必要技能") _
    Then
        Exit Function
    End If
    
    With u
        '必要技能をチェック
        For i = 1 To CountFeature
            Select Case Feature(i)
                Case "必要技能"
                    If Not .IsNecessarySkillSatisfied(FeatureData(i)) Then
                        'アイテム自身により必要技能に指定された能力が封印されていた場合は
                        '必要技能を満たしていると判定させるため、チェックする必要がある。
                        
                        For j = 1 To .CountItem
                            If Me Is .Item(j) Then
                                Exit For
                            End If
                        Next
                        If j > .CountItem Then
                            '既に装備しているのでなければ装備しない
                            IsAvailable = False
                            Exit Function
                        End If
                        
                        If .CountPilot > 0 Then
                            sname = .MainPilot.SkillType(FeatureData(i))
                        Else
                            sname = FeatureData(i)
                        End If
                        
                        '必要技能が「～装備」？
                        If Right$(sname, 2) = "装備" Then
                            If Left$(sname, Len(sname) - 2) = Name _
                                Or Left$(sname, Len(sname) - 2) = Class0 _
                            Then
                                GoTo NextLoop
                            End If
                        End If
                        
                        '封印する能力が必要技能になっている？
                        For j = 1 To CountFeature
                            Select Case Feature(j)
                                Case "パイロット能力付加", "パイロット能力強化"
                                Case Else
                                    GoTo NextLoop1
                            End Select
                            
                            '封印する能力名
                            fdata = FeatureData(j)
                            If Left$(fdata, 1) = """" Then
                                fdata = Mid$(fdata, 2, Len(fdata) - 2)
                            End If
                            If InStr(fdata, "=") > 0 Then
                                fdata = Left$(fdata, InStr(fdata, "=") - 1)
                            End If
                            
                            '必要技能と封印する能力が一致している？
                            If fdata = sname Then
                                GoTo NextLoop
                            End If
                            If .CountPilot > 0 Then
                                If ALDList.IsDefined(fdata) Then
                                    With ALDList.Item(fdata)
                                        For k = 1 To .Count
                                            If .AliasType(k) = sname Then
                                                GoTo NextLoop
                                            End If
                                        Next
                                    End With
                                ElseIf .MainPilot.SkillType(fdata) = sname Then
                                    GoTo NextLoop
                                End If
                            End If
NextLoop1:
                        Next
                        
                        '必要技能が満たされていなかった
                        IsAvailable = False
                        Exit Function
                    End If
                Case "不必要技能"
                    If .IsNecessarySkillSatisfied(FeatureData(i)) Then
                        'アイテム自身により不必要技能が満たされている場合は不必要技能を
                        '無視させるため、チェックする必要がある。
                        
                        For j = 1 To .CountItem
                            If Me Is .Item(j) Then
                                Exit For
                            End If
                        Next
                        If j > .CountItem Then
                            '既に装備しているのでなければ装備しない
                            IsAvailable = False
                            Exit Function
                        End If
                        
                        If .CountPilot > 0 Then
                            sname = .MainPilot.SkillType(FeatureData(i))
                        Else
                            sname = FeatureData(i)
                        End If
                        
                        '不必要技能が「～装備」？
                        If Right$(sname, 2) = "装備" Then
                            If Left$(sname, Len(sname) - 2) = Name _
                                Or Left$(sname, Len(sname) - 2) = Class0 _
                            Then
                                GoTo NextLoop
                            End If
                        End If
                        
                        '付加する能力が不必要技能になっている？
                        For j = 1 To CountFeature
                            Select Case Feature(j)
                                Case "パイロット能力付加", "パイロット能力強化"
                                Case Else
                                    GoTo NextLoop2
                            End Select
                            
                            '付加する能力名
                            fdata = FeatureData(j)
                            If Left$(fdata, 1) = """" Then
                                fdata = Mid$(fdata, 2, Len(fdata) - 2)
                            End If
                            If InStr(fdata, "=") > 0 Then
                                fdata = Left$(fdata, InStr(fdata, "=") - 1)
                            End If
                            
                            '必要技能と付加する能力が一致している？
                            If fdata = sname Then
                                GoTo NextLoop
                            End If
                            If .CountPilot > 0 Then
                                If ALDList.IsDefined(fdata) Then
                                    With ALDList.Item(fdata)
                                        For k = 1 To .Count
                                            If .AliasType(k) = sname Then
                                                GoTo NextLoop
                                            End If
                                        Next
                                    End With
                                ElseIf .MainPilot.SkillType(fdata) = sname Then
                                    GoTo NextLoop
                                End If
                            End If
NextLoop2:
                        Next
                        
                        '不必要技能が満たされていた
                        IsAvailable = False
                        Exit Function
                    End If
            End Select
NextLoop:
        Next
    End With
End Function


'一時中断用データをファイルにセーブする
Public Sub Dump()
    Write #SaveDataFileNumber, Name, ID, Exist
    If Unit Is Nothing Then
        Write #SaveDataFileNumber, "-"
    Else
        Write #SaveDataFileNumber, Unit.ID
    End If
End Sub

'一時中断用データをファイルからロードする
Public Sub Restore()
Dim sbuf As String, bbuf As Boolean

    'Name, ID, Exist
    Input #SaveDataFileNumber, sbuf
    Name = sbuf
    Input #SaveDataFileNumber, sbuf
    ID = sbuf
    Input #SaveDataFileNumber, bbuf
    Exist = bbuf
    
    'Unit
    Line Input #SaveDataFileNumber, sbuf
End Sub

'一時中断用データのリンク情報をファイルからロードする
Public Sub RestoreLinkInfo()
Dim sbuf As String

    'Name, ID, Exist
    Line Input #SaveDataFileNumber, sbuf
    
    'Unit
    Input #SaveDataFileNumber, sbuf
    If UList.IsDefined(sbuf) Then
        Set Unit = UList.Item(sbuf)
    End If
End Sub

''一時中断用データのパラメータ情報をファイルからロードする
Public Sub RestoreParameter()
Dim sbuf As String

    'Name, ID, Exist
    Line Input #SaveDataFileNumber, sbuf
    
    'Unit
    Line Input #SaveDataFileNumber, sbuf
End Sub

