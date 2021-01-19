Option Strict Off
Option Explicit On
Friend Class Condition
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'特殊状態のクラス
	
	'名称
	Public Name As String
	'有効期間
	Public Lifetime As Short
	'レベル
	Public Level As Double
	'データ
	Public StrData As String
End Class