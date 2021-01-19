Option Strict Off
Option Explicit On
Friend Class frmMain
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'メインウィンドウのフォーム
	
	'マップウィンドウがドラッグされているか？
	Private IsDragging As Boolean
	
	Private Sub FlashObject_GetFlashEvent(ByVal FunctionParameter As String)
		GetEvent(FunctionParameter)
	End Sub
	
	'フォーム上でキーを押す
	Private Sub frmMain_KeyDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
		Dim KeyCode As Short = eventArgs.KeyCode
		Dim Shift As Short = eventArgs.KeyData \ &H10000
		'ＧＵＩをロック中？
		If IsGUILocked Then
			'リストボックス表示中はキャンセル動作とみなす
			If frmListBox.Visible Then
				SelectedItem = 0
				TopItem = frmListBox.lstItems.TopIndex + 1
				If IsFormClicked Then
					frmListBox.Hide()
				End If
				IsFormClicked = True
			End If
			
			'メッセージ表示中はメッセージ送りとみなす
			If frmMessage.Visible Then
				IsFormClicked = True
			End If
			
			'クリック待ちであれば待ちを解除
			If WaitClickMode Then
				IsFormClicked = True
			End If
			Exit Sub
		End If
		
		If Shift = 0 Then
			'方向キーを押した場合はマップを動かす
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
		'ツールチップを消す
		frmToolTip.Hide()
		'UPGRADE_ISSUE: 定数 vbCustom はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
		If picMain(0).Cursor Is vbCustom Then
			picMain(0).Cursor = System.Windows.Forms.Cursors.Default
		End If
	End Sub
	
	'フォームを閉じる
	Private Sub frmMain_FormClosed(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
		Dim ret As Short
		Dim IsErrorMessageVisible As Boolean
		
		'エラーメッセージのダイアログは一番上に重ねられるため消去する必要がある
		If Not frmErrorMessage Is Nothing Then
			IsErrorMessageVisible = frmErrorMessage.Visible
		End If
		If IsErrorMessageVisible Then
			frmErrorMessage.Hide()
		End If
		
		'SRCの終了を確認
		ret = MsgBox("SRCを終了しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "終了")
		
		Select Case ret
			Case 1
				'SRCを終了
				TerminateSRC()
			Case 2
				'終了をキャンセル
				'UPGRADE_ISSUE: Event パラメータ Cancel はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="FB723E3C-1C06-4D2B-B083-E6CD0D334DA8"' をクリックしてください。
				Cancel = 1
		End Select
		
		'エラーメッセージを表示
		If IsErrorMessageVisible Then
			frmErrorMessage.Show()
		End If
	End Sub
	
	'マップ画面の横スクロールバーを操作
	'UPGRADE_NOTE: HScroll.Change はイベントからプロシージャに変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' をクリックしてください。
	'UPGRADE_WARNING: HScrollBar イベント HScroll.Change には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
	Private Sub HScroll_Change(ByVal newScrollValue As Integer)
		MapX = HScroll_Renamed.Value
		
		'ステータス表示中はスクロールバーを中央に固定
		If MapFileName = "" Then
			MapX = 8
		End If
		
		'画面書き換え
		If Me.Visible Then
			RefreshScreen()
		End If
	End Sub
	
	'マップコマンドメニューをクリック
	Public Sub mnuMapCommandItem_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuMapCommandItem.Click
		Dim Index As Short = mnuMapCommandItem.GetIndex(eventSender)
		If GetAsyncKeyState(RButtonID) = 1 Then
			'右ボタンでキャンセル
			CancelCommand()
			Exit Sub
		End If
		
		'マップコマンドを実行
		MapCommand(Index)
	End Sub
	
	'ユニットコマンドメニューをクリック
	Public Sub mnuUnitCommandItem_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles mnuUnitCommandItem.Click
		Dim Index As Short = mnuUnitCommandItem.GetIndex(eventSender)
		If GetAsyncKeyState(RButtonID) = 1 Then
			'右ボタンでキャンセル
			CancelCommand()
			Exit Sub
		End If
		
		'ユニットコマンドを実行
		UnitCommand(Index)
	End Sub
	
	'ステータスウィンドウのパイロット画像上をクリック
	Private Sub picFace_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles picFace.Click
		Dim n As Short
		
		'ＧＵＩのロック中は無視
		If IsGUILocked Then
			Exit Sub
		End If
		
		'ステータスウィンドウで表示しているパイロットを変更
		If DisplayedUnit Is Nothing Then
			Exit Sub
		End If
		With DisplayedUnit
			If .CountPilot = 0 Then
				Exit Sub
			End If
			
			DisplayedPilotInd = DisplayedPilotInd + 1
			
			n = .CountPilot + .CountSupport
			If .IsFeatureAvailable("追加サポート") Then
				n = n + 1
			End If
			If DisplayedPilotInd > n Then
				DisplayedPilotInd = 1
			End If
			
			DisplayUnitStatus(DisplayedUnit, DisplayedPilotInd)
		End With
	End Sub
	
	'マップ画面上でダブルクリック
	Private Sub picMain_DoubleClick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles picMain.DoubleClick
		Dim Index As Short = picMain.GetIndex(eventSender)
		If IsGUILocked Then
			'ＧＵＩクロック中は単なるクリックとみなす
			If frmMessage.Visible Then
				IsFormClicked = True
			End If
			If WaitClickMode Then
				IsFormClicked = True
			End If
			Exit Sub
		Else
			'キャンセルの場合はキャンセルを連続実行
			If MouseButton = 2 Then
				Select Case CommandState
					Case "マップコマンド"
						CommandState = "ユニット選択"
					Case "ユニット選択"
						ProceedCommand(True)
					Case Else
						CancelCommand()
				End Select
			End If
		End If
	End Sub
	
	'マップ画面上でマウスをクリック
	Private Sub picMain_MouseDown(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseDown
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim Index As Short = picMain.GetIndex(eventSender)
		Dim xx, yy As Short
		
		'押されたマウスボタンの種類＆カーソルの座標を記録
		MouseButton = Button
		MouseX = X
		MouseY = Y
		
		'ＧＵＩロック中は単なるクリックとして処理
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
				'左クリック
				PrevMapX = MapX
				PrevMapY = MapY
				PrevMouseX = X
				PrevMouseY = Y
				Select Case CommandState
					Case "マップコマンド"
						CommandState = "ユニット選択"
					Case "ユニット選択"
						xx = PixelToMapX(X)
						yy = PixelToMapY(Y)
						If xx < 1 Or MapWidth < xx Or yy < 1 Or MapHeight < yy Then
							IsDragging = True
						ElseIf Not MapDataForUnit(xx, yy) Is Nothing Then 
							ProceedCommand()
						Else
							IsDragging = True
						End If
					Case "ターゲット選択", "移動後ターゲット選択"
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
						'                Case "コマンド選択", "移動後コマンド選択"
					Case "コマンド選択"
						' MOD  END  MARGE
						CancelCommand()
						' ADD START MARGE
						'もし新しいクリック地点がユニットなら、ユニット選択の処理を進める
						xx = PixelToMapX(X)
						yy = PixelToMapY(Y)
						If xx < 1 Or MapWidth < xx Or yy < 1 Or MapHeight < yy Then
							IsDragging = True
						ElseIf Not MapDataForUnit(xx, yy) Is Nothing Then 
							ProceedCommand()
						Else
							IsDragging = True
						End If
					Case "移動後コマンド選択"
						CancelCommand()
						' ADD  END  MARGE
					Case Else
						ProceedCommand()
				End Select
			Case 2
				'右クリック
				Select Case CommandState
					Case "マップコマンド"
						CommandState = "ユニット選択"
					Case "ユニット選択"
						ProceedCommand(True)
					Case Else
						CancelCommand()
				End Select
		End Select
	End Sub
	
	'マップ画面上でマウスカーソルを移動
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
		
		'ＧＵＩロック中？
		If IsGUILocked Then
			If Not WaitClickMode Then
				Exit Sub
			End If
			
			'ホットポイントが定義されている場合はツールチップを変更
			For i = 1 To UBound(HotPointList)
				With HotPointList(i)
					If .Left_Renamed <= MouseX And MouseX < .Left_Renamed + .width And .Top <= MouseY And MouseY < .Top + .Height Then
						If .Caption = "非表示" Or .Caption = "" Then
							Exit For
						End If
						
						If .Name <> LastHostSpot And LastHostSpot <> "" Then
							Exit For
						End If
						
						'ツールチップの表示
						frmToolTip.ShowToolTip(.Caption)
						
						With picMain(0)
							'UPGRADE_ISSUE:  プロパティ . はカスタム マウスポインタをサポートしません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="45116EAB-7060-405E-8ABE-9DBB40DC2E86"' をクリックしてください。
							If Not .Cursor.equals(99) Then
								.Refresh()
								'UPGRADE_ISSUE: PictureBox プロパティ picMain.MousePointer はカスタム マウスポインタをサポートしません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="45116EAB-7060-405E-8ABE-9DBB40DC2E86"' をクリックしてください。
								.Cursor = vbCustom
							End If
						End With
						
						LastHostSpot = .Name
						Exit Sub
					End If
				End With
			Next 
			
			'ホットポイント上にカーソルがなければツールチップを消す
			frmToolTip.Hide()
			LastHostSpot = ""
			picMain(0).Cursor = System.Windows.Forms.Cursors.Default
			Exit Sub
		End If
		
		'マップが設定されていない場合はこれ以降の判定は不要
		If MapWidth < 15 Or MapHeight < 15 Then
			Exit Sub
		End If
		
		'カーソル上にユニットがいればステータスウィンドウにそのユニットを表示
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
						'ユニットがいない、かつステータス表示でなければ地形情報を表示
						DisplayGlobalStatus()
					End If
				Else
					InstantUnitStatusDisplay(xx, yy)
				End If
				'MOD  END
				'ADD START 240a
			Else
				'マップ外にカーソルがある場合
				DisplayGlobalStatus()
				'ADD  END
			End If
		Else
			'ADD ユニット選択追加・移動時も表示 240a
			'        If (CommandState = "ターゲット選択" Or CommandState = "移動後ターゲット選択") _
			''            And (SelectedCommand <> "移動" _
			''                And SelectedCommand <> "テレポート" _
			''                And SelectedCommand <> "ジャンプ") _
			''        Then
			If (CommandState = "ターゲット選択" Or CommandState = "移動後ターゲット選択" Or CommandState = "ユニット選択") Then
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
		
		'マップをドラッグ中？
		If IsDragging And Button = 1 Then
			'Ｘ軸の移動量を算出
			MapX = PrevMapX - (X - PrevMouseX) \ 32
			If MapX < 1 Then
				MapX = 1
			ElseIf MapX > (HScroll_Renamed.Maximum - HScroll_Renamed.LargeChange + 1) Then 
				MapX = (HScroll_Renamed.Maximum - HScroll_Renamed.LargeChange + 1)
			End If
			
			'Ｙ軸の移動量を算出
			MapY = PrevMapY - (Y - PrevMouseY) \ 32
			If MapY < 1 Then
				MapY = 1
			ElseIf MapY > (VScroll_Renamed.Maximum - VScroll_Renamed.LargeChange + 1) Then 
				MapY = (VScroll_Renamed.Maximum - VScroll_Renamed.LargeChange + 1)
			End If
			
			If MapFileName = "" Then
				'ステータス画面の場合は移動量を限定
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
	
	'マップ画面上でマウスボタンを離す
	Private Sub picMain_MouseUp(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.MouseEventArgs) Handles picMain.MouseUp
		Dim Button As Short = eventArgs.Button \ &H100000
		Dim Shift As Short = System.Windows.Forms.Control.ModifierKeys \ &H10000
		Dim X As Single = VB6.PixelsToTwipsX(eventArgs.X)
		Dim Y As Single = VB6.PixelsToTwipsY(eventArgs.Y)
		Dim Index As Short = picMain.GetIndex(eventSender)
		
		If IsGUILocked Then
			Exit Sub
		End If
		'マップ画面のドラッグを解除
		IsDragging = False
	End Sub
	
	'ＢＧＭ連続再生用タイマー
	Private Sub Timer1_Tick(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles Timer1.Tick
		If BGMFileName <> "" Then
			If RepeatMode Then
				RestartBGM()
			End If
		End If
	End Sub
	
	'マップウィンドウの縦スクロールを操作
	'UPGRADE_NOTE: VScroll.Change はイベントからプロシージャに変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' をクリックしてください。
	'UPGRADE_WARNING: VScrollBar イベント VScroll.Change には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
	Private Sub VScroll_Change(ByVal newScrollValue As Integer)
		MapY = VScroll_Renamed.Value
		
		If MapFileName = "" Then
			'ステータス画面の場合は移動量を制限
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