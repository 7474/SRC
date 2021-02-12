VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "UnitData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'ユニットデータのクラス

'名称
Public Name As String
'識別子
Public ID As Long
'クラス
Public Class As String
'パイロット数 (マイナスの場合は括弧つきの指定)
Public PilotNum As Integer
'アイテム数
Public ItemNum As Integer
'地形適応
Public Adaption As String
'ＨＰ
Public HP As Long
'ＥＮ
Public EN As Integer
'移動タイプ
Public Transportation As String
'移動力
Public Speed As Integer
'サイズ
Public Size As String
'装甲
Public Armor As Long
'運動性
Public Mobility As Integer
'修理費
Public Value As Long
'経験値
Public ExpValue As Integer

'愛称
Private proNickname As String
'読み仮名
Private proKanaName As String

'ビットマップ名
Private proBitmap As String
'ビットマップが存在するか
Public IsBitmapMissing As Boolean

'特殊能力
Public colFeature As Collection
'武器データ
Private colWeaponData As Collection
'アビリティデータ
Private colAbilityData As Collection


'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer
    
    If Not colFeature Is Nothing Then
        With colFeature
            For i = 1 To .Count
                .Remove 1
            Next
        End With
        Set colFeature = Nothing
    End If
    
    If Not colWeaponData Is Nothing Then
        With colWeaponData
            For i = 1 To .Count
                .Remove 1
            Next
        End With
        Set colWeaponData = Nothing
    End If
    
    If Not colAbilityData Is Nothing Then
        With colAbilityData
            For i = 1 To .Count
                .Remove 1
            Next
        End With
        Set colAbilityData = Nothing
    End If
End Sub

'愛称
Public Property Get Nickname() As String
    Nickname = proNickname
    If InStr(Nickname, "主人公") = 1 Or InStr(Nickname, "ヒロイン") = 1 Then
        Nickname = GetValueAsString(Nickname & "愛称")
    End If
    ReplaceSubExpression Nickname
End Property

Public Property Let Nickname(new_name As String)
    proNickname = new_name
End Property

'読み仮名
Public Property Get KanaName() As String
    KanaName = proKanaName
    If InStr(KanaName, "主人公") = 1 _
        Or InStr(KanaName, "ヒロイン") = 1 _
        Or InStr(KanaName, "ひろいん") = 1 _
    Then
        If IsVariableDefined(KanaName & "読み仮名") Then
            KanaName = GetValueAsString(KanaName & "読み仮名")
        Else
            KanaName = StrToHiragana(GetValueAsString(KanaName & "愛称"))
        End If
    End If
    ReplaceSubExpression KanaName
End Property

Public Property Let KanaName(new_name As String)
    proKanaName = new_name
End Property

'ビットマップ
Public Property Get Bitmap0() As String
    Bitmap0 = proBitmap
End Property

Public Property Get Bitmap() As String
    If IsBitmapMissing Then
        Bitmap = "-.bmp"
    Else
        Bitmap = proBitmap
    End If
End Property

Public Property Let Bitmap(new_bitmap As String)
    proBitmap = new_bitmap
End Property


'特殊能力を追加
Public Sub AddFeature(fdef As String)
Dim fd As FeatureData
Dim ftype As String, flevel As Double, fdata As String
Dim nskill As String, ncondition As String
Dim i As Integer, j As Integer
Dim buf As String
    
    If colFeature Is Nothing Then
        Set colFeature = New Collection
    End If
    
    '必要技能の切り出し
    If Right$(fdef, 1) = ")" Then
        i = InStr(fdef, " (")
        If i > 0 Then
            nskill = Trim$(Mid$(fdef, i + 2, Len(fdef) - i - 2))
            buf = Trim$(Left$(fdef, i))
        ElseIf Left$(fdef, 1) = "(" Then
            nskill = Trim$(Mid$(fdef, 2, Len(fdef) - 2))
            buf = ""
        Else
            buf = fdef
        End If
    Else
        buf = fdef
    End If
    
    '必要条件の切り出し
    If Right$(buf, 1) = ">" Then
        i = InStr(buf, " <")
        If i > 0 Then
            ncondition = Trim$(Mid$(buf, i + 2, Len(buf) - i - 2))
            buf = Trim$(Left$(buf, i))
        ElseIf Left$(buf, 1) = "<" Then
            ncondition = Trim$(Mid$(buf, 2, Len(buf) - 2))
            buf = ""
        End If
    End If
    
    '特殊能力の種類、レベル、データを切り出し
    flevel = DEFAULT_LEVEL
    i = InStr(buf, "Lv")
    j = InStr(buf, "=")
    If i > 0 And j > 0 And i > j Then
        i = 0
    End If
    If i > 0 Then
        ftype = Left$(buf, i - 1)
        If j > 0 Then
            flevel = CDbl(Mid$(buf, i + 2, j - (i + 2)))
            fdata = Mid$(buf, j + 1)
        Else
            flevel = CDbl(Mid$(buf, i + 2))
        End If
    ElseIf j > 0 Then
        ftype = Left$(buf, j - 1)
        fdata = Mid$(buf, j + 1)
    Else
        ftype = buf
    End If
    
    'データが「"」で囲まれている場合、「"」を削除
    If Left$(fdata, 1) = """" Then
        If Right$(fdata, 1) = """" Then
            fdata = Mid$(fdata, 2, Len(fdata) - 2)
        End If
    End If
    
    'エリアスが定義されている？
    If ALDList.IsDefined(ftype) Then
        If LIndex(fdata, 1) <> "解説" Then
            With ALDList.Item(ftype)
                For i = 1 To .Count
                    Set fd = New FeatureData
                    
                    'エリアスの定義に従って特殊能力定義を置き換える
                    fd.Name = .AliasType(i)
                    If .AliasType(i) <> ftype Then
                        If .AliasLevelIsPlusMod(i) Then
                            If flevel = DEFAULT_LEVEL Then
                                flevel = 1
                            End If
                            If .AliasLevel(i) = DEFAULT_LEVEL Then
                                fd.Level = flevel + 1
                            Else
                                fd.Level = flevel + .AliasLevel(i)
                            End If
                        ElseIf .AliasLevelIsMultMod(i) Then
                            If flevel = DEFAULT_LEVEL Then
                                flevel = 1
                            End If
                            If .AliasLevel(i) = DEFAULT_LEVEL Then
                                fd.Level = flevel
                            Else
                                fd.Level = flevel * .AliasLevel(i)
                            End If
                        ElseIf flevel <> DEFAULT_LEVEL Then
                            fd.Level = flevel
                        Else
                            fd.Level = .AliasLevel(i)
                        End If
                        If fdata <> "" And InStr(.AliasData(i), "非表示") <> 1 Then
                            fd.StrData = fdata & " " _
                                & ListTail(.AliasData(i), LLength(fdata) + 1)
                        Else
                            fd.StrData = .AliasData(i)
                        End If
                        If .AliasLevelIsMultMod(i) Then
                            buf = fd.StrData
                            ReplaceString buf, "Lv1", "Lv" & Format$(flevel)
                            fd.StrData = buf
                        End If
                    Else
                        '特殊能力解説の定義
                        If fdata <> "" And LIndex(fdata, 1) <> "非表示" Then
                            fd.Name = LIndex(fdata, 1)
                        End If
                        fd.StrData = .AliasData(i)
                    End If
                    If nskill <> "" Then
                        fd.NecessarySkill = nskill
                    Else
                        fd.NecessarySkill = .AliasNecessarySkill(i)
                    End If
                    If ncondition <> "" Then
                        fd.NecessaryCondition = ncondition
                    Else
                        fd.NecessaryCondition = .AliasNecessaryCondition(i)
                    End If
                    
                    '特殊能力を登録
                    If IsFeatureAvailable(fd.Name) Then
                        colFeature.Add fd, fd.Name & Format$(CountFeature)
                    Else
                        colFeature.Add fd, fd.Name
                    End If
                Next
            End With
            Exit Sub
        End If
    End If
    
    '特殊能力を登録
    Set fd = New FeatureData
    With fd
        .Name = ftype
        .Level = flevel
        .StrData = fdata
        .NecessarySkill = nskill
        .NecessaryCondition = ncondition
    End With
    If IsFeatureAvailable(ftype) Then
        colFeature.Add fd, ftype & Format$(CountFeature)
    Else
        colFeature.Add fd, ftype
    End If
End Sub

'特殊能力の総数
Public Function CountFeature() As Integer
    If colFeature Is Nothing Then
        Exit Function
    End If
    CountFeature = colFeature.Count
End Function

'特殊能力
Public Function Feature(Index As Variant) As String
Dim fd As FeatureData

    Set fd = colFeature.Item(Index)
    Feature = fd.Name
End Function

'特殊能力の名称
Public Function FeatureName(Index As Variant) As String
Dim fd As FeatureData

    Set fd = colFeature.Item(Index)
    With fd
        If Len(.StrData) > 0 Then
            FeatureName = ListIndex(.StrData, 1)
        ElseIf .Level > 0 Then
            FeatureName = .Name & "Lv" & Format$(.Level)
        Else
            FeatureName = .Name
        End If
    End With
End Function

'特殊能力のレベル
Public Function FeatureLevel(Index As Variant) As Double
Dim fd As FeatureData

    On Error GoTo ErrorHandler
    Set fd = colFeature.Item(Index)
    
    FeatureLevel = fd.Level
    If FeatureLevel = DEFAULT_LEVEL Then
        FeatureLevel = 1
    End If
    Exit Function

ErrorHandler:
    FeatureLevel = 0
End Function

'特殊能力のデータ
Public Function FeatureData(Index As Variant) As String
Dim fd As FeatureData

    On Error GoTo ErrorHandler
    Set fd = colFeature.Item(Index)
    FeatureData = fd.StrData
    Exit Function

ErrorHandler:
    FeatureData = ""
End Function

'特殊能力の必要技能
Public Function FeatureNecessarySkill(Index As Variant) As String
Dim fd As FeatureData

    On Error GoTo ErrorHandler
    Set fd = colFeature.Item(Index)
    FeatureNecessarySkill = fd.NecessarySkill
    Exit Function

ErrorHandler:
    FeatureNecessarySkill = ""
End Function

'指定した特殊能力を持っているか？
Public Function IsFeatureAvailable(fname As String) As Boolean
Dim fd As FeatureData

    On Error GoTo ErrorHandler
    Set fd = colFeature.Item(fname)
    IsFeatureAvailable = True
    Exit Function

ErrorHandler:
    IsFeatureAvailable = False
End Function

'指定した特殊能力がレベル指定されているか？
Public Function IsFeatureLevelSpecified(Index As Variant) As Boolean
Dim fd As FeatureData
    
    On Error GoTo ErrorHandler
    Set fd = colFeature.Item(Index)
    
    If fd.Level = DEFAULT_LEVEL Then
        IsFeatureLevelSpecified = False
    Else
        IsFeatureLevelSpecified = True
    End If
    Exit Function

ErrorHandler:
    IsFeatureLevelSpecified = False
End Function

'武器を追加
Public Function AddWeapon(wname As String) As WeaponData
Dim new_wdata As New WeaponData

    If colWeaponData Is Nothing Then
        Set colWeaponData = New Collection
    End If
    new_wdata.Name = wname
    colWeaponData.Add new_wdata, wname & Format$(CountWeapon)
    Set AddWeapon = new_wdata
End Function

'武器の総数
Public Function CountWeapon() As Integer
    If colWeaponData Is Nothing Then
        Exit Function
    End If
    CountWeapon = colWeaponData.Count
End Function

'武器データ
Public Function Weapon(Index As Variant) As WeaponData
    Set Weapon = colWeaponData.Item(Index)
End Function

'アビリティを追加
Public Function AddAbility(aname As String) As AbilityData
Dim new_sadata As New AbilityData
    
    If colAbilityData Is Nothing Then
        Set colAbilityData = New Collection
    End If
    new_sadata.Name = aname
    colAbilityData.Add new_sadata, aname & Format$(CountAbility)
    Set AddAbility = new_sadata
End Function

'アビリティの総数
Public Function CountAbility() As Integer
    If colAbilityData Is Nothing Then
        Exit Function
    End If
    CountAbility = colAbilityData.Count
End Function

'アビリティデータ
Public Function Ability(Index As Variant) As AbilityData
    Set Ability = colAbilityData.Item(Index)
End Function

'特殊能力、武器データ、アビリティデータを削除する
Public Sub Clear()
Dim i As Integer

    If Not colFeature Is Nothing Then
        For i = 1 To colFeature.Count
            colFeature.Remove 1
        Next
    End If
    If Not colWeaponData Is Nothing Then
        For i = 1 To colWeaponData.Count
            colWeaponData.Remove 1
        Next
    End If
    If Not colAbilityData Is Nothing Then
        For i = 1 To colAbilityData.Count
            colAbilityData.Remove 1
        Next
    End If
End Sub
