Option Strict Off
Option Explicit On
Friend Class frmNowLoading
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'データロードの進行状況を示すフォーム
	
	'データ総数
	'UPGRADE_NOTE: Size は Size_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Size_Renamed As Short
	'読み込み終えたデータの数
	Public Value As Short
	
	'ロードを１段階進行させる
	Public Sub Progress()
		Value = Value + 1
		'UPGRADE_ISSUE: PictureBox メソッド picBar.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		picBar.Cls()
		'UPGRADE_ISSUE: PictureBox メソッド picBar.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		picBar.Line (0, 0) - (picBar.ClientRectangle.Width * Value \ Size_Renamed, picBar.ClientRectangle.Height), BF
		Refresh()
	End Sub
End Class