Option Strict Off
Option Explicit On
Friend Class frmTitle
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'タイトル画面用フォーム
	
	Private Sub frmTitle_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		'UPGRADE_ISSUE: App オブジェクト はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6B85A2A7-FE9F-4FBE-AA0C-CF11AC86A305"' をクリックしてください。
		With App
			labVersion.Text = "Ver " & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Revision & "a"
		End With
	End Sub
End Class