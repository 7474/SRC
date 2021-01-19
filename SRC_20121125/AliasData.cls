VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "AliasData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'エリアスデータのクラス

'名称
Public Name As String

Private strAliasType() As String
Private dblAliasLevel() As Double
Private blnAliasLevelIsPlusMod() As Boolean
Private blnAliasLevelIsMultMod() As Boolean
Private strAliasData() As String
Private strAliasNecessarySkill() As String
Private strAliasNecessaryCondition() As String



'クラスの初期化
Private Sub Class_Initialize()
    ReDim strAliasType(0)
    ReDim dblAliasLevel(0)
    ReDim blnAliasLevelIsPlusMod(0)
    ReDim blnAliasLevelIsMultMod(0)
    ReDim strAliasData(0)
    ReDim strAliasNecessarySkill(0)
    ReDim strAliasNecessaryCondition(0)
End Sub


'エリアスの個数
Public Property Get Count() As Integer
    Count = UBound(strAliasType)
End Property

'エリアスの種類
Public Property Get AliasType(ByVal idx As Integer) As String
    AliasType = strAliasType(idx)
End Property

'エリアスのレベル
Public Property Get AliasLevel(ByVal idx As Integer) As Double
    AliasLevel = dblAliasLevel(idx)
End Property

'エリアスのレベルが＋修正値かどうか
Public Property Get AliasLevelIsPlusMod(ByVal idx As Integer) As Boolean
    AliasLevelIsPlusMod = blnAliasLevelIsPlusMod(idx)
End Property

'エリアスのレベルが×修正値かどうか
Public Property Get AliasLevelIsMultMod(ByVal idx As Integer) As Boolean
    AliasLevelIsMultMod = blnAliasLevelIsMultMod(idx)
End Property

'エリアスのデータ
Public Property Get AliasData(ByVal idx As Integer) As String
    AliasData = strAliasData(idx)
End Property

'エリアスの必要技能
Public Property Get AliasNecessarySkill(ByVal idx As Integer) As String
    AliasNecessarySkill = strAliasNecessarySkill(idx)
End Property

'エリアスの必要条件
Public Property Get AliasNecessaryCondition(ByVal idx As Integer) As String
    AliasNecessaryCondition = strAliasNecessaryCondition(idx)
End Property

'エリアスを登録
Public Sub AddAlias(ByVal adef As String)
Dim atype As String, alevel As Double, adata As String
Dim i As Integer, j As Integer, n As Integer
Dim buf As String
    
    'エリアスの領域を確保
    n = UBound(strAliasType) + 1
    ReDim Preserve strAliasType(n)
    ReDim Preserve dblAliasLevel(n)
    ReDim Preserve blnAliasLevelIsPlusMod(n)
    ReDim Preserve blnAliasLevelIsMultMod(n)
    ReDim Preserve strAliasData(n)
    ReDim Preserve strAliasNecessarySkill(n)
    ReDim Preserve strAliasNecessaryCondition(n)
    
    '必要技能の切り出し
    If Right$(adef, 1) = ")" Then
        i = InStr(adef, " (")
        If i > 0 Then
            strAliasNecessarySkill(n) = Trim$(Mid$(adef, i + 2, Len(adef) - i - 2))
            buf = Trim$(Left$(adef, i))
        ElseIf Left$(adef, 1) = "(" Then
            strAliasNecessarySkill(n) = Trim$(Mid$(adef, 2, Len(adef) - 2))
            buf = ""
        Else
            buf = adef
        End If
    Else
        buf = adef
    End If
    
    '必要条件の切り出し
    If Right$(buf, 1) = ">" Then
        i = InStr(adef, " <")
        If i > 0 Then
            strAliasNecessaryCondition(n) = Trim$(Mid$(buf, i + 2, Len(buf) - i - 2))
            buf = Trim$(Left$(buf, i))
        ElseIf Left$(buf, 1) = "<" Then
            strAliasNecessaryCondition(n) = Trim$(Mid$(buf, 2, Len(buf) - 2))
            buf = ""
        End If
    End If
    
    'レベル指定が修正値か？
    If ReplaceString(buf, "Lv+", "Lv") Then
        blnAliasLevelIsPlusMod(n) = True
    ElseIf ReplaceString(buf, "Lv*", "Lv") Then
        blnAliasLevelIsMultMod(n) = True
    End If
    
    'エリアスの種類、レベル、データを切り出し
    alevel = DEFAULT_LEVEL
    i = InStr(buf, "Lv")
    j = InStr(buf, "=")
    If i > 0 And j > 0 And i > j Then
        i = 0
    End If
    If i > 0 Then
        atype = Left$(buf, i - 1)
        If j > 0 Then
            alevel = CDbl(Mid$(buf, i + 2, j - (i + 2)))
            adata = Mid$(buf, j + 1)
        Else
            alevel = CDbl(Mid$(buf, i + 2))
        End If
    ElseIf j > 0 Then
        atype = Left$(buf, j - 1)
        adata = Mid$(buf, j + 1)
    Else
        atype = buf
    End If
    
    'エリアスを登録
    strAliasType(n) = atype
    dblAliasLevel(n) = alevel
    strAliasData(n) = adata
End Sub

