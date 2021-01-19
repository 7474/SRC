Option Strict Off
Option Explicit On
Friend Class frmListBox
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'リストボックスのフォーム
	
	'リストボックスのサイズ (通常:M 大型:L)
	'幅
	Public HorizontalSize As String
	'高さ
	Public VerticalSize As String
	
	'Questionコマンド用変数
	Public CurrentTime As Short
	Public TimeLimit As Short
	
	'最後に選択されたアイテム
	Private LastSelectedItem As Short
	
	'リストボックスへのキー入力
	Private Sub frmListBox_KeyDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
		Dim KeyCode As Short = eventArgs.KeyCode
		Dim Shift As Short = eventArgs.KeyData \ &H10000
		'既にウィンドウが隠れている場合は無視
		If Not Visible Then
			Exit Sub
		End If
		
		Select Case KeyCode
			Case System.Windows.Forms.Keys.Up
			Case System.Windows.Forms.Keys.Down
			Case System.Windows.Forms.Keys.Left, System.Windows.Forms.Keys.Right
			Case System.Windows.Forms.Keys.Escape, System.Windows.Forms.Keys.Delete, System.Windows.Forms.Keys.Back
				'キャンセル
				SelectedItem = 0
				LastSelectedItem = SelectedItem
				TopItem = lstItems.TopIndex
				If IsFormClicked Then
					Hide()
				End If
				IsFormClicked = True
			Case Else
				'選択
				If lstItems.SelectedIndex < 0 Then
					Exit Sub
				End If
				If UBound(ListItemFlag) >= lstItems.SelectedIndex + 1 Then
					If ListItemFlag(lstItems.SelectedIndex + 1) Then
						Exit Sub
					End If
				End If
				SelectedItem = lstItems.SelectedIndex + 1
				LastSelectedItem = SelectedItem
				TopItem = lstItems.TopIndex + 1
				If IsFormClicked Then
					Hide()
				End If
				IsFormClicked = True
		End Select
	End Sub
	
	'リストボックスを開く
	Private Sub frmListBox_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		Dim ret As Integer
		
		'リストボックスを常に手前に表示
		ret = SetWindowPos(Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
		HorizontalSize = "M"
		VerticalSize = "M"
	End Sub
	
	'リストボックスを閉じる
	Private Sub frmListBox_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		TopItem = lstItems.TopIndex + 1
		IsFormClicked = True
		If Not IsMordal And Visible Then
			'UPGRADE_ISSUE: Event パラメータ Cancel はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' をクリックしてください。
			Cancel = 1
		End If
		Hide()
	End Sub
	
	'項目をダブルクリック
	Private Sub lstItems_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles lstItems.DoubleClick
		'無効なアイテムが選択されている？
		If lstItems.SelectedIndex < 0 Then
			Exit Sub
		End If
		If UBound(ListItemFlag) >= lstItems.SelectedIndex + 1 Then
			If ListItemFlag(lstItems.SelectedIndex + 1) Then
				Exit Sub
			End If
		End If
		
		If LastSelectedItem <> 0 Then
			'連続で選択
			If Not Visible Then
				Exit Sub
			End If
			SelectedItem = lstItems.SelectedIndex + 1
			LastSelectedItem = SelectedItem
			TopItem = lstItems.TopIndex + 1
			If IsFormClicked Then
				Hide()
			End If
			IsFormClicked = True
		Else
			'連続でキャンセル
			SelectedItem = 0
			LastSelectedItem = SelectedItem
			TopItem = lstItems.TopIndex + 1
			If IsFormClicked Then
				Hide()
			End If
			IsFormClicked = True
		End If
	End Sub
	
	'マウスでクリック
	Private Sub lstItems_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles lstItems.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Select Case Button
			Case 1
				'選択
				If Not Visible Then
					Exit Sub
				End If
				
				'無効なアイテムが選択されている？
				If lstItems.SelectedIndex < 0 Then
					Exit Sub
				End If
				If UBound(ListItemFlag) >= lstItems.SelectedIndex + 1 Then
					If ListItemFlag(lstItems.SelectedIndex + 1) Then
						Exit Sub
					End If
				End If
				
				SelectedItem = lstItems.SelectedIndex + 1
				LastSelectedItem = SelectedItem
				TopItem = lstItems.TopIndex + 1
				IsFormClicked = True
				
				If CurrentTime > 0 Then
					Timer1.Enabled = False
					Hide()
				End If
				
			Case 2
				'キャンセル
				SelectedItem = 0
				LastSelectedItem = SelectedItem
				TopItem = lstItems.TopIndex + 1
				IsFormClicked = True
		End Select
	End Sub
	
	'キャプション部分をクリック
	Private Sub labCaption_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles labCaption.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Select Case Button
			Case 1
				'ユニットステータスを表示しているユニットを入れ替え
				If MainForm.Visible Then
					If Not DisplayedUnit Is Nothing And Not SelectedUnit Is Nothing And Not SelectedTarget Is Nothing Then
						If DisplayedUnit.ID = SelectedUnit.ID Then
							DisplayUnitStatus(SelectedTarget)
						Else
							DisplayUnitStatus(SelectedUnit)
						End If
					End If
				End If
			Case 2
				'キャンセル
				SelectedItem = 0
				LastSelectedItem = SelectedItem
				TopItem = lstItems.TopIndex
				If IsFormClicked Then
					Hide()
				End If
				IsFormClicked = True
		End Select
	End Sub
	
	'リストボックスの端をクリック
	Private Sub frmListBox_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		If Button = 2 Then
			'キャンセル
			SelectedItem = 0
			LastSelectedItem = SelectedItem
			TopItem = lstItems.TopIndex
			If IsFormClicked Then
				Hide()
			End If
			IsFormClicked = True
		End If
	End Sub
	
	'リストボックス上でマウスカーソルを動かす
	Private Sub lstItems_MouseMove(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles lstItems.MouseMove
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim itm As Short
		
		'カーソルがあるアイテムを算出
		itm = ((Y * ClientRectangle.Width) \ VB6.PixelsToTwipsX(Width) + 1) \ 16
		itm = itm + lstItems.TopIndex
		
		'カーソル上のアイテムをハイライト表示
		If itm < 0 Or itm >= lstItems.Items.Count Then
			lstItems.SelectedIndex = -1
			Exit Sub
		End If
		If lstItems.SelectedIndex = itm Then
			Exit Sub
		End If
		lstItems.SelectedIndex = itm
		
		'コメント欄を更新
		If txtComment.Enabled Then
			txtComment.Text = ListItemComment(itm + 1)
		End If
	End Sub
	
	'Questionコマンド対応
	Private Sub Timer1_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer1.Tick
		CurrentTime = CurrentTime + 1
		'UPGRADE_ISSUE: PictureBox メソッド picBar.Cls はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		picBar.Cls()
		'UPGRADE_ISSUE: PictureBox メソッド picBar.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		picBar.Line (0, 0) - (picBar.ClientRectangle.Width * CurrentTime \ TimeLimit, picBar.ClientRectangle.Height), BF
		picBar.Refresh()
		
		If CurrentTime >= TimeLimit Then
			SelectedItem = 0
			LastSelectedItem = SelectedItem
			TopItem = lstItems.TopIndex
			Hide()
			Timer1.Enabled = False
		End If
	End Sub
	
	'選択されているアイテムに対応するユニットのステータス表示
	Private Sub Timer2_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer2.Tick
		Dim u As Unit
		
		If Not Visible Or Not MainForm.Visible Then
			Exit Sub
		End If
		
		If lstItems.SelectedIndex = -1 Then
			Exit Sub
		End If
		
		On Error GoTo ErrorHandler
		
		If lstItems.SelectedIndex >= UBound(ListItemID) Then
			Exit Sub
		End If
		
		'選択されたユニットが存在する？
		If Not UList.IsDefined2(ListItemID(lstItems.SelectedIndex + 1)) Then
			Exit Sub
		End If
		
		u = UList.Item2(ListItemID(lstItems.SelectedIndex + 1))
		
		'選択されたユニットにパイロットが乗っている？
		If u.CountPilot = 0 Then
			Exit Sub
		End If
		
		'既に表示している？
		If DisplayedUnit Is u Then
			Exit Sub
		End If
		
		'選択されたユニットをステータスウィンドウに表示
		DisplayUnitStatus(u)
		
ErrorHandler: 
	End Sub
End Class