VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "WeaponData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'武器データクラス

'武器名
Public Name As String
'攻撃力
Public Power As Long
'最小射程
Public MinRange As Integer
'最大射程
Public MaxRange As Integer
'命中率
Public Precision As Integer
'弾数
Public Bullet As Integer
'消費ＥＮ
Public ENConsumption As Integer
'必要気力
Public NecessaryMorale As Integer
'地形適応
Public Adaption As String
'ＣＴ率
Public Critical As Integer
'属性
Public Class As String
'必要技能
Public NecessarySkill As String
'必要条件
Public NecessaryCondition As String

'武器愛称
Public Function Nickname() As String
    Nickname = Name
    ReplaceSubExpression Nickname
    If InStr(Nickname, "(") > 0 Then
        Nickname = Left$(Nickname, InStr(Nickname, "(") - 1)
    End If
End Function

'使い捨てアイテムによる武器かどうかを返す
Public Function IsItem() As Boolean
Dim i As Integer
    
    For i = 1 To LLength(NecessarySkill)
        If LIndex(NecessarySkill, i) = "アイテム" Then
            IsItem = True
            Exit Function
        End If
    Next
End Function

