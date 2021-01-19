Option Strict Off
Option Explicit On
Friend Class SkillData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'パイロット用特殊能力のクラス
	
	'名称
	Public Name As String
	'レベル (レベル指定のない能力の場合はDEFAULT_LEVEL)
	Public Level As Double
	'データ
	Public StrData As String
	'習得レベル
	Public NecessaryLevel As Short
End Class