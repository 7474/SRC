VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "TerrainData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'地形データのクラス

'識別番号
Public ID As Integer
'名称
Public Name As String
'ビットマップ名
Public Bitmap As String
'地形タイプ
Public Class As String
'移動コスト
Public MoveCost As Integer
'命中修正
Public HitMod As Integer
'ダメージ修正
Public DamageMod As Integer

'地形効果
Public colFeature As Collection


'クラスの初期化
Private Sub Class_Initialize()
    ID = -1
End Sub

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
End Sub


'地形効果を追加
Public Sub AddFeature(fdef As String)
Dim fd As FeatureData
Dim ftype As String, flevel As Double, fdata As String
Dim i As Integer, j As Integer
Dim buf As String
    
    If colFeature Is Nothing Then
        Set colFeature = New Collection
    End If
    
    buf = fdef
    
    '地形効果の種類、レベル、データを切り出し
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
    
    '地形効果を登録
    Set fd = New FeatureData
    With fd
        .Name = ftype
        .Level = flevel
        .StrData = fdata
    End With
    If IsFeatureAvailable(ftype) Then
        colFeature.Add fd, ftype & Format$(CountFeature)
    Else
        colFeature.Add fd, ftype
    End If
End Sub

'地形効果の総数
Public Function CountFeature() As Integer
    If colFeature Is Nothing Then
        Exit Function
    End If
    CountFeature = colFeature.Count
End Function

'地形効果
Public Function Feature(Index As Variant) As String
Dim fd As FeatureData

    Set fd = colFeature.Item(Index)
    Feature = fd.Name
End Function

'地形効果の名称
Public Function FeatureName(Index As Variant) As String
Dim fd As FeatureData

    Set fd = colFeature.Item(Index)
    With fd
        If Len(.StrData) > 0 Then
            FeatureName = ListIndex(.StrData, 1)
        ElseIf .Level <> DEFAULT_LEVEL Then
            FeatureName = .Name & "Lv" & Format$(.Level)
        Else
            FeatureName = .Name
        End If
    End With
End Function

'地形効果のレベル
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

'地形効果のデータ
Public Function FeatureData(Index As Variant) As String
Dim fd As FeatureData

    On Error GoTo ErrorHandler
    Set fd = colFeature.Item(Index)
    FeatureData = fd.StrData
    Exit Function

ErrorHandler:
    FeatureData = ""
End Function

'指定した地形効果を持っているか？
Public Function IsFeatureAvailable(fname As String) As Boolean
Dim fd As FeatureData

    If colFeature Is Nothing Then
        Exit Function
    End If
    For Each fd In colFeature
        If fd.Name = fname Then
            IsFeatureAvailable = True
            Exit Function
        End If
    Next
    IsFeatureAvailable = False
End Function

'指定した地形効果はレベル指定がされているか？
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

'データをクリア
Public Sub Clear()
Dim i As Integer
    
    ID = -1
    If Not colFeature Is Nothing Then
        With colFeature
            For i = 1 To .Count
                .Remove 1
            Next
        End With
        Set colFeature = Nothing
    End If
End Sub
