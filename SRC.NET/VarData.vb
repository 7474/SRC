Option Strict Off
Option Explicit On
Friend Class VarData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'変数のクラス
	
	'名称
	Public Name As String
	'型
	Public VariableType As Expression.ValueType
	'文字列値
	Public StringValue As String
	'数値
	Public NumericValue As Double
End Class