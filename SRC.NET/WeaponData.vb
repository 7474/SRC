Option Strict Off
Option Explicit On
Friend Class WeaponData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'武器データクラス
	
	'武器名
	Public Name As String
	'攻撃力
	Public Power As Integer
	'最小射程
	Public MinRange As Short
	'最大射程
	Public MaxRange As Short
	'命中率
	Public Precision As Short
	'弾数
	Public Bullet As Short
	'消費ＥＮ
	Public ENConsumption As Short
	'必要気力
	Public NecessaryMorale As Short
	'地形適応
	Public Adaption As String
	'ＣＴ率
	Public Critical As Short
	'属性
	'UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Class_Renamed As String
	'必要技能
	Public NecessarySkill As String
	'必要条件
	Public NecessaryCondition As String
	
	'武器愛称
	Public Function Nickname() As String
		Nickname = Name
		ReplaceSubExpression(Nickname)
		If InStr(Nickname, "(") > 0 Then
			Nickname = Left(Nickname, InStr(Nickname, "(") - 1)
		End If
	End Function
	
	'使い捨てアイテムによる武器かどうかを返す
	Public Function IsItem() As Boolean
		Dim i As Short
		
		For i = 1 To LLength(NecessarySkill)
			If LIndex(NecessarySkill, i) = "アイテム" Then
				IsItem = True
				Exit Function
			End If
		Next 
	End Function
End Class