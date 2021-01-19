Option Strict Off
Option Explicit On
Friend Class frmMain
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'メインウィンドウのフォーム
	
	'Invalid_string_refer_to_original_code
	Private IsDragging As Boolean
	
	Private Sub FlashObject_GetFlashEvent(ByVal FunctionParameter As String)
		GetEvent(FunctionParameter)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub frmMain_KeyDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
		Dim KeyCode As Short = eventArgs.KeyCode
		Dim Shift As Short = eventArgs.KeyData \ &H10000
		'Invalid_string_refer_to_original_code
		If IsGUILocked Then
			'Invalid_string_refer_to_original_code
			If frmListBox.Visible Then
				SelectedItem = 0
				TopItem = frmListBox.lstItems.TopIndex + 1
				If IsFormClicked Then
					frmListBox.Hide()
				End If
				IsFormClicked = True
			End If
			
			'Invalid_string_refer_to_original_code
			If frmMessage.Visible Then
				IsFormClicked = True
			End If
			
			'Invalid_string_refer_to_original_code
			If WaitClickMode Then
				IsFormClicked = True
			End If
			Exit Sub
		End If
		
		If Shift = 0 Then
			'Invalid_string_refer_to_original_code
			Select Case KeyCode
				Case System.Windows.Forms.Keys.Left
					If MapX > 1 Then
						MapX = MapX - 1
						RefreshScreen()
					End If
				Case System.Windows.Forms.Keys.Up
					If MapY > 1 Then
						MapY = MapY - 1
						RefreshScreen()
					End If
				Case System.Windows.Forms.Keys.Right
					If MapX < (HScroll_Renamed.Maximum - HScroll_Renamed.LargeChange + 1) Then
						MapX = MapX + 1
						RefreshScreen()
					End If
				Case System.Windows.Forms.Keys.Down
					If MapY < (VScroll_Renamed.Maximum - VScroll_Renamed.LargeChange + 1) Then
						MapY = MapY + 1
						RefreshScreen()
					End If
				Case System.Windows.Forms.Keys.Escape, System.Windows.Forms.Keys.Delete, System.Windows.Forms.Keys.Back
					picMain_MouseDown(picMain.Item(0), New System.Windows.Forms.MouseEventArgs(&H100000, 0, 0, 0, 0))
				Case Else
					picMain_MouseDown(picMain.Item(0), New System.Windows.Forms.MouseEventArgs(&H100000, 0, 0, 0, 0))
			End Select
		End If
	End Sub
	
	'フォーム上でマウスを動かす
	Private Sub frmMain_MouseMove(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		'Invalid_string_refer_to_original_code
		frmToolTip.Hide()
		'UPGRADE_ISSUE: �萔 vbCustom �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
		If picMain(0).Cursor Is vbCustom Then
			picMain(0).Cursor = System.Windows.Forms.Cursors.Default
		End If
	End Sub
	
	'フォームを閉じる
	Private Sub frmMain_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		Dim ret As Short
		Dim IsErrorMessageVisible As Boolean
		
		'Invalid_string_refer_to_original_code
		If Not frmErrorMessage Is Nothing Then
			IsErrorMessageVisible = frmErrorMessage.Visible
		End If
		If IsErrorMessageVisible Then
			frmErrorMessage.Hide()
		End If
		
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code_
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
		
		Select Case ret
			Case 1
				'Invalid_string_refer_to_original_code
				TerminateSRC()
			Case 2
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: Event �p�����[�^ Cancel �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' ���N���b�N���Ă��������B
				Cancel = 1
		End Select
		
		'Invalid_string_refer_to_original_code
		If IsErrorMessageVisible Then
			frmErrorMessage.Show()
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: HScroll.Change �̓C�x���g����v���V�[�W���ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' ���N���b�N���Ă��������B
	'UPGRADE_WARNING: HScrollBar �C�x���g HScroll.Change �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
	Private Sub HScroll_Change(ByVal newScrollValue As Integer)
		MapX = HScroll_Renamed.Value
		
		'Invalid_string_refer_to_original_code
		If MapFileName = "" Then
			MapX = 8
		End If
		
		'Invalid_string_refer_to_original_code
		If Me.Visible Then
			RefreshScreen()
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub mnuMapCommandItem_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuMapCommandItem.Click
		Dim Index As Short = mnuMapCommandItem.GetIndex(eventSender)
		If GetAsyncKeyState(RButtonID) = 1 Then
			'右ボタンでキャンセル
			CancelCommand()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		MapCommand(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub mnuUnitCommandItem_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuUnitCommandItem.Click
		Dim Index As Short = mnuUnitCommandItem.GetIndex(eventSender)
		If GetAsyncKeyState(RButtonID) = 1 Then
			'右ボタンでキャンセル
			CancelCommand()
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		UnitCommand(Index)
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub picFace_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles picFace.Click
		Dim n As Short
		
		'Invalid_string_refer_to_original_code
		If IsGUILocked Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If DisplayedUnit Is Nothing Then
			Exit Sub
		End If
		With DisplayedUnit
			If .CountPilot = 0 Then
				Exit Sub
			End If
			
			DisplayedPilotInd = DisplayedPilotInd + 1
			
			n = .CountPilot + .CountSupport
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
			n = n + 1
			'End If
			If DisplayedPilotInd > n Then
				DisplayedPilotInd = 1
			End If
			
			DisplayUnitStatus(DisplayedUnit, DisplayedPilotInd)
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub picMain_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles picMain.DoubleClick
		Dim Index As Short = picMain.GetIndex(eventSender)
		If IsGUILocked Then
			'Invalid_string_refer_to_original_code
			If frmMessage.Visible Then
				IsFormClicked = True
			End If
			If WaitClickMode Then
				IsFormClicked = True
			End If
			Exit Sub
		Else
			'Invalid_string_refer_to_original_code
			If MouseButton = 2 Then
				Select Case CommandState
					Case "Invalid_string_refer_to_original_code"
						CommandState = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						ProceedCommand(True)
					Case Else
						CancelCommand()
				End Select
			End If
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub picMain_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim Index As Short = picMain.GetIndex(eventSender)
		Dim xx, yy As Short
		
		'Invalid_string_refer_to_original_code
		MouseButton = Button
		MouseX = X
		MouseY = Y
		
		'Invalid_string_refer_to_original_code
		If IsGUILocked Then
			If frmMessage.Visible Then
				IsFormClicked = True
			End If
			If WaitClickMode Then
				IsFormClicked = True
			End If
			Exit Sub
		End If
		
		Select Case Button
			Case 1
				'Invalid_string_refer_to_original_code
				PrevMapX = MapX
				PrevMapY = MapY
				PrevMouseX = X
				PrevMouseY = Y
				Select Case CommandState
					Case "Invalid_string_refer_to_original_code"
						CommandState = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						xx = PixelToMapX(X)
						yy = PixelToMapY(Y)
						If xx < 1 Or MapWidth < xx Or yy < 1 Or MapHeight < yy Then
							IsDragging = True
						ElseIf Not MapDataForUnit(xx, yy) Is Nothing Then 
							ProceedCommand()
						Else
							IsDragging = True
						End If
					Case "Invalid_string_refer_to_original_code"
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: �O�̍s����͂ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' ���N���b�N���Ă��������B
						xx = PixelToMapX(X)
						yy = PixelToMapY(Y)
						If xx < 1 Or MapWidth < xx Or yy < 1 Or MapHeight < yy Then
							IsDragging = True
						ElseIf Not MaskData(xx, yy) Then 
							ProceedCommand()
						Else
							IsDragging = True
						End If
						' MOD START MARGE
						'Invalid_string_refer_to_original_code
					Case "Invalid_string_refer_to_original_code"
						' MOD  END  MARGE
						CancelCommand()
						' ADD START MARGE
						'Invalid_string_refer_to_original_code
						xx = PixelToMapX(X)
						yy = PixelToMapY(Y)
						If xx < 1 Or MapWidth < xx Or yy < 1 Or MapHeight < yy Then
							IsDragging = True
						ElseIf Not MapDataForUnit(xx, yy) Is Nothing Then 
							ProceedCommand()
						Else
							IsDragging = True
						End If
					Case "Invalid_string_refer_to_original_code"
						CancelCommand()
						' ADD  END  MARGE
					Case Else
						ProceedCommand()
				End Select
			Case 2
				'Invalid_string_refer_to_original_code
				Select Case CommandState
					Case "Invalid_string_refer_to_original_code"
						CommandState = "Invalid_string_refer_to_original_code"
					Case "Invalid_string_refer_to_original_code"
						ProceedCommand(True)
					Case Else
						CancelCommand()
				End Select
		End Select
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub picMain_MouseMove(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseMove
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim Index As Short = picMain.GetIndex(eventSender)
		Static LastMouseX, LastMouseY As Short
		Static LastMapX, LastMapY As Short
		Static LastHostSpot As String
		Dim xx, yy As Short
		Dim i As Short
		
		'前回のマウス位置を記録
		LastMouseX = MouseX
		LastMouseY = MouseY
		
		'現在のマウス位置を記録
		MouseX = X
		MouseY = Y
		
		'Invalid_string_refer_to_original_code
		If IsGUILocked Then
			If Not WaitClickMode Then
				Exit Sub
			End If
			
			'Invalid_string_refer_to_original_code
			For i = 1 To UBound(HotPointList)
				With HotPointList(i)
					If .Left_Renamed <= MouseX And MouseX < .Left_Renamed + .width And .Top <= MouseY And MouseY < .Top + .Height Then
						If .Caption = "非表示" Or .Caption = "" Then
							Exit For
						End If
						
						If .Name <> LastHostSpot And LastHostSpot <> "" Then
							Exit For
						End If
						
						'Invalid_string_refer_to_original_code
						frmToolTip.ShowToolTip(.Caption)
						
						With picMain(0)
							'UPGRADE_ISSUE:  �v���p�e�B . �̓J�X�^�� �}�E�X�|�C���^���T�|�[�g���܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="45116EAB-7060-405E-8ABE-9DBB40DC2E86"' ���N���b�N���Ă��������B
							If Not .Cursor.equals(99) Then
								.Refresh()
								'UPGRADE_ISSUE: PictureBox �v���p�e�B picMain.MousePointer �̓J�X�^�� �}�E�X�|�C���^���T�|�[�g���܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="45116EAB-7060-405E-8ABE-9DBB40DC2E86"' ���N���b�N���Ă��������B
								.Cursor = vbCustom
							End If
						End With
						
						LastHostSpot = .Name
						Exit Sub
					End If
				End With
			Next 
			
			'Invalid_string_refer_to_original_code
			frmToolTip.Hide()
			LastHostSpot = ""
			picMain(0).Cursor = System.Windows.Forms.Cursors.Default
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		If MapWidth < 15 Or MapHeight < 15 Then
			Exit Sub
		End If
		
		'Invalid_string_refer_to_original_code
		xx = PixelToMapX(X)
		yy = PixelToMapY(Y)
		'MOD START 240a
		'    If MainWidth = 15 Then
		If Not NewGUIMode Then
			'MOD  END
			If 1 <= xx And xx <= MapWidth And 1 <= yy And yy <= MapHeight Then
				'MOD START 240a
				'            If Not MapDataForUnit(xx, yy) Is Nothing Then
				'                InstantUnitStatusDisplay xx, yy
				'            End If
				If MapDataForUnit(xx, yy) Is Nothing Then
					If Not MapFileName = "" Then
						'Invalid_string_refer_to_original_code
						DisplayGlobalStatus()
					End If
				Else
					InstantUnitStatusDisplay(xx, yy)
				End If
				'MOD  END
				'ADD START 240a
			Else
				'Invalid_string_refer_to_original_code
				DisplayGlobalStatus()
				'ADD  END
			End If
		Else
			ClearUnitStatus()
		End If
		'End If
		
		'Invalid_string_refer_to_original_code
		If IsDragging And Button = 1 Then
			'Invalid_string_refer_to_original_code
			MapX = PrevMapX - (X - PrevMouseX) \ 32
			If MapX < 1 Then
				MapX = 1
			ElseIf MapX > (HScroll_Renamed.Maximum - HScroll_Renamed.LargeChange + 1) Then 
				MapX = (HScroll_Renamed.Maximum - HScroll_Renamed.LargeChange + 1)
			End If
			
			'Invalid_string_refer_to_original_code
			MapY = PrevMapY - (Y - PrevMouseY) \ 32
			If MapY < 1 Then
				MapY = 1
			ElseIf MapY > (VScroll_Renamed.Maximum - VScroll_Renamed.LargeChange + 1) Then 
				MapY = (VScroll_Renamed.Maximum - VScroll_Renamed.LargeChange + 1)
			End If
			
			If MapFileName = "" Then
				'Invalid_string_refer_to_original_code
				MapX = 8
				If MapY < 8 Then
					MapY = 8
				ElseIf MapY > MapHeight - 7 Then 
					MapY = MapHeight - 7
				End If
			End If
			
			'マップ画面を新しい座標で更新
			If Not MapX = LastMapX Or Not MapY = LastMapY Then
				RefreshScreen()
			End If
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub picMain_MouseUp(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseUp
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim Index As Short = picMain.GetIndex(eventSender)
		
		If IsGUILocked Then
			Exit Sub
		End If
		'Invalid_string_refer_to_original_code
		IsDragging = False
	End Sub
	
	'Invalid_string_refer_to_original_code
	Private Sub Timer1_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer1.Tick
		If BGMFileName <> "" Then
			If RepeatMode Then
				RestartBGM()
			End If
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: VScroll.Change �̓C�x���g����v���V�[�W���ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' ���N���b�N���Ă��������B
	'UPGRADE_WARNING: VScrollBar �C�x���g VScroll.Change �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
	Private Sub VScroll_Change(ByVal newScrollValue As Integer)
		MapY = VScroll_Renamed.Value
		
		If MapFileName = "" Then
			'Invalid_string_refer_to_original_code
			If MapY < 8 Then
				MapY = 8
			ElseIf MapY > MapHeight - 7 Then 
				MapY = MapHeight - 7
			End If
		End If
		
		'マップ画面を更新
		If Me.Visible Then
			RefreshScreen()
		End If
	End Sub
	Private Sub HScroll_Renamed_Scroll(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.ScrollEventArgs) Handles HScroll_Renamed.Scroll
		Select Case eventArgs.type
			Case System.Windows.Forms.ScrollEventType.EndScroll
				HScroll_Change(eventArgs.newValue)
		End Select
	End Sub
	Private Sub VScroll_Renamed_Scroll(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.ScrollEventArgs) Handles VScroll_Renamed.Scroll
		Select Case eventArgs.type
			Case System.Windows.Forms.ScrollEventType.EndScroll
				VScroll_Change(eventArgs.newValue)
		End Select
	End Sub
End Class