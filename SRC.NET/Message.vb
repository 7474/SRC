Option Strict Off
Option Explicit On
Friend Class frmMessage
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'メッセージウィンドウのフォーム
	
	'フォーム上をクリック
	Private Sub frmMessage_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Click
		IsFormClicked = True
	End Sub
	
	'フォーム上をダブルクリック
	Private Sub frmMessage_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.DoubleClick
		IsFormClicked = True
	End Sub
	
	'フォーム上でキーを押す
	Private Sub frmMessage_KeyDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
		Dim KeyCode As Short = eventArgs.KeyCode
		Dim Shift As Short = eventArgs.KeyData \ &H10000
		IsFormClicked = True
	End Sub
	
	'フォーム上でマウスボタンを押す
	Private Sub frmMessage_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		IsFormClicked = True
	End Sub
	
	'フォームを閉じる
	Private Sub frmMessage_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		Dim ret As Short
		
		'SRCを終了するか確認
		ret = MsgBox("SRCを終了しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "終了")
		
		Select Case ret
			Case 1
				'SRCを終了
				Hide()
				TerminateSRC()
			Case 2
				'終了をキャンセル
				'UPGRADE_ISSUE: Event パラメータ Cancel はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' をクリックしてください。
				Cancel = 1
		End Select
	End Sub
	
	'パイロット画面上でクリック
	Private Sub picFace_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picFace.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		'自動メッセージ送りモードに移行
		If MessageWait < 10000 Then
			AutoMessageMode = Not AutoMessageMode
		End If
		IsFormClicked = True
	End Sub
	
	'メッセージ欄上でダブルクリック
	Private Sub picMessage_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles picMessage.DoubleClick
		IsFormClicked = True
	End Sub
	
	'メッセージ欄上でマウスボタンを押す
	Private Sub picMessage_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picMessage.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		IsFormClicked = True
	End Sub
End Class