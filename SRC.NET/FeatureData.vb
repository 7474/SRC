Option Strict Off
Option Explicit On
Friend Class FeatureData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'特殊能力のクラス
	
	'名称
	Public Name As String
	'レベル
	Public Level As Double
	'データ
	Public StrData As String
	'必要技能
	Public NecessarySkill As String
	'必要条件
	Public NecessaryCondition As String
End Class