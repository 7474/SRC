Option Strict Off
Option Explicit On
Friend Class frmMain
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'���C���E�B���h�E�̃t�H�[��
	
	'�}�b�v�E�B���h�E���h���b�O����Ă��邩�H
	Private IsDragging As Boolean
	
	Private Sub FlashObject_GetFlashEvent(ByVal FunctionParameter As String)
		GetEvent(FunctionParameter)
	End Sub
	
	'�t�H�[����ŃL�[������
	Private Sub frmMain_KeyDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
		Dim KeyCode As Short = eventArgs.KeyCode
		Dim Shift As Short = eventArgs.KeyData \ &H10000
		'�f�t�h�����b�N���H
		If IsGUILocked Then
			'���X�g�{�b�N�X�\�����̓L�����Z������Ƃ݂Ȃ�
			If frmListBox.Visible Then
				SelectedItem = 0
				TopItem = frmListBox.lstItems.TopIndex + 1
				If IsFormClicked Then
					frmListBox.Hide()
				End If
				IsFormClicked = True
			End If
			
			'���b�Z�[�W�\�����̓��b�Z�[�W����Ƃ݂Ȃ�
			If frmMessage.Visible Then
				IsFormClicked = True
			End If
			
			'�N���b�N�҂��ł���Α҂�������
			If WaitClickMode Then
				IsFormClicked = True
			End If
			Exit Sub
		End If
		
		If Shift = 0 Then
			'�����L�[���������ꍇ�̓}�b�v�𓮂���
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
	
	'�t�H�[����Ń}�E�X�𓮂���
	Private Sub frmMain_MouseMove(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles MyBase.MouseMove
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = eventArgs.X
		Dim Y As Single = eventArgs.Y
		'�c�[���`�b�v������
		frmToolTip.Hide()
		'UPGRADE_ISSUE: �萔 vbCustom �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
		If picMain(0).Cursor Is vbCustom Then
			picMain(0).Cursor = System.Windows.Forms.Cursors.Default
		End If
	End Sub
	
	'�t�H�[�������
	Private Sub frmMain_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		Dim ret As Short
		Dim IsErrorMessageVisible As Boolean
		
		'�G���[���b�Z�[�W�̃_�C�A���O�͈�ԏ�ɏd�˂��邽�ߏ�������K�v������
		If Not frmErrorMessage Is Nothing Then
			IsErrorMessageVisible = frmErrorMessage.Visible
		End If
		If IsErrorMessageVisible Then
			frmErrorMessage.Hide()
		End If
		
		'SRC�̏I�����m�F
		ret = MsgBox("SRC���I�����܂����H", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "�I��")
		
		Select Case ret
			Case 1
				'SRC���I��
				TerminateSRC()
			Case 2
				'�I�����L�����Z��
				'UPGRADE_ISSUE: Event �p�����[�^ Cancel �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' ���N���b�N���Ă��������B
				Cancel = 1
		End Select
		
		'�G���[���b�Z�[�W��\��
		If IsErrorMessageVisible Then
			frmErrorMessage.Show()
		End If
	End Sub
	
	'�}�b�v��ʂ̉��X�N���[���o�[�𑀍�
	'UPGRADE_NOTE: HScroll.Change �̓C�x���g����v���V�[�W���ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' ���N���b�N���Ă��������B
	'UPGRADE_WARNING: HScrollBar �C�x���g HScroll.Change �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
	Private Sub HScroll_Change(ByVal newScrollValue As Integer)
		MapX = HScroll_Renamed.Value
		
		'�X�e�[�^�X�\�����̓X�N���[���o�[�𒆉��ɌŒ�
		If MapFileName = "" Then
			MapX = 8
		End If
		
		'��ʏ�������
		If Me.Visible Then
			RefreshScreen()
		End If
	End Sub
	
	'�}�b�v�R�}���h���j���[���N���b�N
	Public Sub mnuMapCommandItem_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuMapCommandItem.Click
		Dim Index As Short = mnuMapCommandItem.GetIndex(eventSender)
		If GetAsyncKeyState(RButtonID) = 1 Then
			'�E�{�^���ŃL�����Z��
			CancelCommand()
			Exit Sub
		End If
		
		'�}�b�v�R�}���h�����s
		MapCommand(Index)
	End Sub
	
	'���j�b�g�R�}���h���j���[���N���b�N
	Public Sub mnuUnitCommandItem_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuUnitCommandItem.Click
		Dim Index As Short = mnuUnitCommandItem.GetIndex(eventSender)
		If GetAsyncKeyState(RButtonID) = 1 Then
			'�E�{�^���ŃL�����Z��
			CancelCommand()
			Exit Sub
		End If
		
		'���j�b�g�R�}���h�����s
		UnitCommand(Index)
	End Sub
	
	'�X�e�[�^�X�E�B���h�E�̃p�C���b�g�摜����N���b�N
	Private Sub picFace_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles picFace.Click
		Dim n As Short
		
		'�f�t�h�̃��b�N���͖���
		If IsGUILocked Then
			Exit Sub
		End If
		
		'�X�e�[�^�X�E�B���h�E�ŕ\�����Ă���p�C���b�g��ύX
		If DisplayedUnit Is Nothing Then
			Exit Sub
		End If
		With DisplayedUnit
			If .CountPilot = 0 Then
				Exit Sub
			End If
			
			DisplayedPilotInd = DisplayedPilotInd + 1
			
			n = .CountPilot + .CountSupport
			If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
				n = n + 1
			End If
			If DisplayedPilotInd > n Then
				DisplayedPilotInd = 1
			End If
			
			DisplayUnitStatus(DisplayedUnit, DisplayedPilotInd)
		End With
	End Sub
	
	'�}�b�v��ʏ�Ń_�u���N���b�N
	Private Sub picMain_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles picMain.DoubleClick
		Dim Index As Short = picMain.GetIndex(eventSender)
		If IsGUILocked Then
			'�f�t�h�N���b�N���͒P�Ȃ�N���b�N�Ƃ݂Ȃ�
			If frmMessage.Visible Then
				IsFormClicked = True
			End If
			If WaitClickMode Then
				IsFormClicked = True
			End If
			Exit Sub
		Else
			'�L�����Z���̏ꍇ�̓L�����Z����A�����s
			If MouseButton = 2 Then
				Select Case CommandState
					Case "�}�b�v�R�}���h"
						CommandState = "���j�b�g�I��"
					Case "���j�b�g�I��"
						ProceedCommand(True)
					Case Else
						CancelCommand()
				End Select
			End If
		End If
	End Sub
	
	'�}�b�v��ʏ�Ń}�E�X���N���b�N
	Private Sub picMain_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim Index As Short = picMain.GetIndex(eventSender)
		Dim xx, yy As Short
		
		'�����ꂽ�}�E�X�{�^���̎�ށ��J�[�\���̍��W���L�^
		MouseButton = Button
		MouseX = X
		MouseY = Y
		
		'�f�t�h���b�N���͒P�Ȃ�N���b�N�Ƃ��ď���
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
				'���N���b�N
				PrevMapX = MapX
				PrevMapY = MapY
				PrevMouseX = X
				PrevMouseY = Y
				Select Case CommandState
					Case "�}�b�v�R�}���h"
						CommandState = "���j�b�g�I��"
					Case "���j�b�g�I��"
						xx = PixelToMapX(X)
						yy = PixelToMapY(Y)
						If xx < 1 Or MapWidth < xx Or yy < 1 Or MapHeight < yy Then
							IsDragging = True
						ElseIf Not MapDataForUnit(xx, yy) Is Nothing Then 
							ProceedCommand()
						Else
							IsDragging = True
						End If
					Case "�^�[�Q�b�g�I��", "�ړ���^�[�Q�b�g�I��"
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
						'                Case "�R�}���h�I��", "�ړ���R�}���h�I��"
					Case "�R�}���h�I��"
						' MOD  END  MARGE
						CancelCommand()
						' ADD START MARGE
						'�����V�����N���b�N�n�_�����j�b�g�Ȃ�A���j�b�g�I���̏�����i�߂�
						xx = PixelToMapX(X)
						yy = PixelToMapY(Y)
						If xx < 1 Or MapWidth < xx Or yy < 1 Or MapHeight < yy Then
							IsDragging = True
						ElseIf Not MapDataForUnit(xx, yy) Is Nothing Then 
							ProceedCommand()
						Else
							IsDragging = True
						End If
					Case "�ړ���R�}���h�I��"
						CancelCommand()
						' ADD  END  MARGE
					Case Else
						ProceedCommand()
				End Select
			Case 2
				'�E�N���b�N
				Select Case CommandState
					Case "�}�b�v�R�}���h"
						CommandState = "���j�b�g�I��"
					Case "���j�b�g�I��"
						ProceedCommand(True)
					Case Else
						CancelCommand()
				End Select
		End Select
	End Sub
	
	'�}�b�v��ʏ�Ń}�E�X�J�[�\�����ړ�
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
		
		'�O��̃}�E�X�ʒu���L�^
		LastMouseX = MouseX
		LastMouseY = MouseY
		
		'���݂̃}�E�X�ʒu���L�^
		MouseX = X
		MouseY = Y
		
		'�f�t�h���b�N���H
		If IsGUILocked Then
			If Not WaitClickMode Then
				Exit Sub
			End If
			
			'�z�b�g�|�C���g����`����Ă���ꍇ�̓c�[���`�b�v��ύX
			For i = 1 To UBound(HotPointList)
				With HotPointList(i)
					If .Left_Renamed <= MouseX And MouseX < .Left_Renamed + .width And .Top <= MouseY And MouseY < .Top + .Height Then
						If .Caption = "��\��" Or .Caption = "" Then
							Exit For
						End If
						
						If .Name <> LastHostSpot And LastHostSpot <> "" Then
							Exit For
						End If
						
						'�c�[���`�b�v�̕\��
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
			
			'�z�b�g�|�C���g��ɃJ�[�\�����Ȃ���΃c�[���`�b�v������
			frmToolTip.Hide()
			LastHostSpot = ""
			picMain(0).Cursor = System.Windows.Forms.Cursors.Default
			Exit Sub
		End If
		
		'�}�b�v���ݒ肳��Ă��Ȃ��ꍇ�͂���ȍ~�̔���͕s�v
		If MapWidth < 15 Or MapHeight < 15 Then
			Exit Sub
		End If
		
		'�J�[�\����Ƀ��j�b�g������΃X�e�[�^�X�E�B���h�E�ɂ��̃��j�b�g��\��
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
						'���j�b�g�����Ȃ��A���X�e�[�^�X�\���łȂ���Βn�`����\��
						DisplayGlobalStatus()
					End If
				Else
					InstantUnitStatusDisplay(xx, yy)
				End If
				'MOD  END
				'ADD START 240a
			Else
				'�}�b�v�O�ɃJ�[�\��������ꍇ
				DisplayGlobalStatus()
				'ADD  END
			End If
		Else
			'ADD ���j�b�g�I��ǉ��E�ړ������\�� 240a
			'        If (CommandState = "�^�[�Q�b�g�I��" Or CommandState = "�ړ���^�[�Q�b�g�I��") _
			''            And (SelectedCommand <> "�ړ�" _
			''                And SelectedCommand <> "�e���|�[�g" _
			''                And SelectedCommand <> "�W�����v") _
			''        Then
			If (CommandState = "�^�[�Q�b�g�I��" Or CommandState = "�ړ���^�[�Q�b�g�I��" Or CommandState = "���j�b�g�I��") Then
				If 1 <= xx And xx <= MapWidth And 1 <= yy And yy <= MapHeight Then
					If Not MapDataForUnit(xx, yy) Is Nothing Then
						Me.picMain(0).Refresh()
						'                    RedrawScreen
						InstantUnitStatusDisplay(xx, yy)
						'ADD Else
					Else
						ClearUnitStatus()
					End If
				End If
			ElseIf MouseX <> LastMouseX Or MouseY <> LastMouseY Then 
				ClearUnitStatus()
			End If
		End If
		
		'�}�b�v���h���b�O���H
		If IsDragging And Button = 1 Then
			'�w���̈ړ��ʂ��Z�o
			MapX = PrevMapX - (X - PrevMouseX) \ 32
			If MapX < 1 Then
				MapX = 1
			ElseIf MapX > (HScroll_Renamed.Maximum - HScroll_Renamed.LargeChange + 1) Then 
				MapX = (HScroll_Renamed.Maximum - HScroll_Renamed.LargeChange + 1)
			End If
			
			'�x���̈ړ��ʂ��Z�o
			MapY = PrevMapY - (Y - PrevMouseY) \ 32
			If MapY < 1 Then
				MapY = 1
			ElseIf MapY > (VScroll_Renamed.Maximum - VScroll_Renamed.LargeChange + 1) Then 
				MapY = (VScroll_Renamed.Maximum - VScroll_Renamed.LargeChange + 1)
			End If
			
			If MapFileName = "" Then
				'�X�e�[�^�X��ʂ̏ꍇ�͈ړ��ʂ�����
				MapX = 8
				If MapY < 8 Then
					MapY = 8
				ElseIf MapY > MapHeight - 7 Then 
					MapY = MapHeight - 7
				End If
			End If
			
			'�}�b�v��ʂ�V�������W�ōX�V
			If Not MapX = LastMapX Or Not MapY = LastMapY Then
				RefreshScreen()
			End If
		End If
	End Sub
	
	'�}�b�v��ʏ�Ń}�E�X�{�^���𗣂�
	Private Sub picMain_MouseUp(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseUp
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim Index As Short = picMain.GetIndex(eventSender)
		
		If IsGUILocked Then
			Exit Sub
		End If
		'�}�b�v��ʂ̃h���b�O������
		IsDragging = False
	End Sub
	
	'�a�f�l�A���Đ��p�^�C�}�[
	Private Sub Timer1_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer1.Tick
		If BGMFileName <> "" Then
			If RepeatMode Then
				RestartBGM()
			End If
		End If
	End Sub
	
	'�}�b�v�E�B���h�E�̏c�X�N���[���𑀍�
	'UPGRADE_NOTE: VScroll.Change �̓C�x���g����v���V�[�W���ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' ���N���b�N���Ă��������B
	'UPGRADE_WARNING: VScrollBar �C�x���g VScroll.Change �ɂ͐V�������삪�܂܂�܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' ���N���b�N���Ă��������B
	Private Sub VScroll_Change(ByVal newScrollValue As Integer)
		MapY = VScroll_Renamed.Value
		
		If MapFileName = "" Then
			'�X�e�[�^�X��ʂ̏ꍇ�͈ړ��ʂ𐧌�
			If MapY < 8 Then
				MapY = 8
			ElseIf MapY > MapHeight - 7 Then 
				MapY = MapHeight - 7
			End If
		End If
		
		'�}�b�v��ʂ��X�V
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