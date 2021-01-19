Option Strict Off
Option Explicit On
Friend Class frmToolTip
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'ツールチップ用フォーム
	
	'フォームをロード
	Private Sub frmToolTip_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		Dim ret As Integer
		
		'常に手前に表示
		ret = SetWindowPos(Me.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
	End Sub
	
	'ツールチップを表示
	Public Sub ShowToolTip(ByRef msg As String)
		Dim ret As Integer
		Dim PT As POINTAPI
		Dim tw As Short
		Static cur_msg As String
		
		tw = VB6.TwipsPerPixelX
		
		If msg <> cur_msg Then
			cur_msg = msg
			With Me.picMessage
				'メッセージ長にサイズを合わせる
				'UPGRADE_ISSUE: PictureBox メソッド picMessage.TextWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.Width = VB6.TwipsToPixelsX((.TextWidth(msg) + 6) * tw)
				'UPGRADE_ISSUE: PictureBox メソッド picMessage.TextHeight はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.Height = VB6.TwipsToPixelsY((.TextHeight(msg) + 4) * tw)
				Me.Width = .Width
				Me.Height = .Height
				
				'UPGRADE_ISSUE: PictureBox メソッド picMessage.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.Cls()
				
				.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(200, 200, 200))
				'UPGRADE_ISSUE: PictureBox メソッド picMessage.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				frmToolTip.picMessage.Line (0, 0) - (VB6.PixelsToTwipsX(.Width) \ tw, 0)
				'UPGRADE_ISSUE: PictureBox メソッド picMessage.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				frmToolTip.picMessage.Line (0, 0) - (0, VB6.PixelsToTwipsX(.Width) \ tw)
				.ForeColor = System.Drawing.Color.Black
				'UPGRADE_ISSUE: PictureBox メソッド picMessage.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				frmToolTip.picMessage.Line (0, (VB6.PixelsToTwipsY(.Height) - 1) \ tw) - (VB6.PixelsToTwipsX(.Width) \ tw, (VB6.PixelsToTwipsY(.Height) - 1) \ tw)
				'UPGRADE_ISSUE: PictureBox メソッド picMessage.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				frmToolTip.picMessage.Line ((VB6.PixelsToTwipsX(.Width) - 1) \ tw, 0) - ((VB6.PixelsToTwipsX(.Width) - 1) \ tw, VB6.PixelsToTwipsY(.Height) \ tw)
				
				'メッセージを書き込み
				'UPGRADE_ISSUE: PictureBox プロパティ picMessage.CurrentX はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.CurrentX = 3
				'UPGRADE_ISSUE: PictureBox プロパティ picMessage.CurrentY はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.CurrentY = 2
				'UPGRADE_ISSUE: PictureBox メソッド picMessage.Print はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				frmToolTip.picMessage.Print(msg)
				
				.ForeColor = System.Drawing.Color.White
				.Refresh()
			End With
		End If
		
		'フォームの位置を設定
		ret = GetCursorPos(PT)
		Me.Left = VB6.TwipsToPixelsX(PT.X * tw + 0)
		Me.Top = VB6.TwipsToPixelsY((PT.Y + 24) * tw)
		
		'フォームを非アクティブで表示
		ret = ShowWindow(Me.Handle.ToInt32, SW_SHOWNA)
	End Sub
End Class