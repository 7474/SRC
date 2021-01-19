Option Strict Off
Option Explicit On
Friend Class frmConfiguration
	Inherits System.Windows.Forms.Form
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'マップコマンド「設定変更」用ダイアログ
	
	
	'MP3Volumeを記録
	Private SavedMP3Volume As Short
	
	'戦闘アニメOn・Off切り替え
	'UPGRADE_WARNING: イベント chkBattleAnimation.CheckStateChanged は、フォームが初期化されたときに発生します。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"' をクリックしてください。
	Private Sub chkBattleAnimation_CheckStateChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles chkBattleAnimation.CheckStateChanged
		'戦闘アニメを表示しない場合は拡張戦闘アニメ、武器アニメ選択の項目を選択不能にする
		If chkBattleAnimation.CheckState = 1 Then
			chkExtendedAnimation.Enabled = True
			chkWeaponAnimation.Enabled = True
		Else
			chkExtendedAnimation.Enabled = False
			chkWeaponAnimation.Enabled = False
		End If
	End Sub
	
	'キャンセルボタンが押された
	Private Sub cmdCancel_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdCancel.Click
		Dim IsMP3Supported As Object
		'ダイアログを閉じる
		Hide()
		
		'MP3音量のみその場で変更しているので元に戻す必要がある
		MP3Volume = SavedMP3Volume
		'UPGRADE_WARNING: オブジェクト IsMP3Supported の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		If IsMP3Supported Then
			Call vbmp3_setVolume(MP3Volume, MP3Volume)
		End If
	End Sub
	
	'OKボタンが押された
	Private Sub cmdOK_Click(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles cmdOK.Click
		'各種設定を変更
		
		'メッセージスピード
		Select Case cboMessageSpeed.Text
			Case "神の領域"
				MessageWait = 0
			Case "超高速"
				MessageWait = 200
			Case "高速"
				MessageWait = 400
			Case "普通"
				MessageWait = 700
			Case "低速"
				MessageWait = 1000
			Case "手動送り"
				MessageWait = 10000000
		End Select
		WriteIni("Option", "MessageWait", VB6.Format(MessageWait))
		
		'戦闘アニメ表示
		If chkBattleAnimation.CheckState = 1 Then
			BattleAnimation = True
			WriteIni("Option", "BattleAnimation", "On")
		Else
			BattleAnimation = False
			WriteIni("Option", "BattleAnimation", "Off")
		End If
		
		'拡大戦闘アニメ表示
		If chkExtendedAnimation.CheckState = 1 Then
			ExtendedAnimation = True
			WriteIni("Option", "ExtendedAnimation", "On")
		Else
			ExtendedAnimation = False
			WriteIni("Option", "Extendednimation", "Off")
		End If
		
		'武器準備アニメ表示
		If chkWeaponAnimation.CheckState = 1 Then
			WeaponAnimation = True
			WriteIni("Option", "WeaponAnimation", "On")
		Else
			WeaponAnimation = False
			WriteIni("Option", "WeaponAnimation", "Off")
		End If
		
		'移動アニメ表示
		If chkMoveAnimation.CheckState = 1 Then
			MoveAnimation = True
			WriteIni("Option", "MoveAnimation", "On")
		Else
			MoveAnimation = False
			WriteIni("Option", "MoveAnimation", "Off")
		End If
		
		'スペシャルパワーアニメ表示
		If chkSpecialPowerAnimation.CheckState = 1 Then
			SpecialPowerAnimation = True
			WriteIni("Option", "SpecialPowerAnimation", "On")
		Else
			SpecialPowerAnimation = False
			WriteIni("Option", "SpecialPowerAnimation", "Off")
		End If
		
		'マウスカーソルの自動移動
		If chkAutoMoveCursor.CheckState Then
			AutoMoveCursor = True
			WriteIni("Option", "AutoMoveCursor", "On")
		Else
			AutoMoveCursor = False
			WriteIni("Option", "AutoMoveCursor", "Off")
		End If
		
		'マス目の表示
		If chkShowSquareLine.CheckState Then
			ShowSquareLine = True
			WriteIni("Option", "Square", "On")
		Else
			ShowSquareLine = False
			WriteIni("Option", "Square", "Off")
		End If
		
		'味方フェイズ開始時のターン表示
		If chkShowTurn.CheckState Then
			WriteIni("Option", "Turn", "On")
		Else
			WriteIni("Option", "Turn", "Off")
		End If
		
		'敵フェイズ中にＢＧＭを変更しない
		If chkKeepEnemyBGM.CheckState Then
			KeepEnemyBGM = True
			WriteIni("Option", "KeepEnemyBGM", "On")
		Else
			KeepEnemyBGM = False
			WriteIni("Option", "KeepEnemyBGM", "Off")
		End If
		
		'MIDI演奏にDirectMusicを使用する
		If chkUseDirectMusic.CheckState Then
			WriteIni("Option", "UseDirectMusic", "On")
		Else
			WriteIni("Option", "UseDirectMusic", "Off")
		End If
		
		'MIDI音源リセットの種類
		MidiResetType = cboMidiReset.Text
		WriteIni("Option", "MidiReset", (cboMidiReset.Text))
		
		'MP3再生音量
		WriteIni("Option", "MP3Volume", VB6.Format(MP3Volume))
		
		'ダイアログを閉じる
		Hide()
	End Sub
	
	Private Sub frmConfiguration_Load(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles MyBase.Load
		'ダイアログを初期化
		
		'メッセージスピード
		cboMessageSpeed.Items.Add("手動送り")
		cboMessageSpeed.Items.Add("低速")
		cboMessageSpeed.Items.Add("普通")
		cboMessageSpeed.Items.Add("高速")
		cboMessageSpeed.Items.Add("超高速")
		cboMessageSpeed.Items.Add("神の領域")
		Select Case MessageWait
			Case 0
				cboMessageSpeed.Text = "神の領域"
			Case 200
				cboMessageSpeed.Text = "超高速"
			Case 400
				cboMessageSpeed.Text = "高速"
			Case 700
				cboMessageSpeed.Text = "普通"
			Case 1000
				cboMessageSpeed.Text = "低速"
			Case 10000000
				cboMessageSpeed.Text = "手動送り"
		End Select
		
		'戦闘アニメ表示
		If BattleAnimation Then
			chkBattleAnimation.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkBattleAnimation.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		If Not FileExists(AppPath & "Lib\汎用戦闘アニメ\include.eve") Then
			chkBattleAnimation.CheckState = System.Windows.Forms.CheckState.Indeterminate '無効
		End If
		
		'拡張戦闘アニメ表示
		If ExtendedAnimation Then
			chkExtendedAnimation.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkExtendedAnimation.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		If chkBattleAnimation.CheckState = 1 Then
			chkExtendedAnimation.Enabled = True
		Else
			chkExtendedAnimation.Enabled = False
		End If
		
		'武器準備アニメ表示
		If WeaponAnimation Then
			chkWeaponAnimation.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkWeaponAnimation.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		If chkBattleAnimation.CheckState = 1 Then
			chkWeaponAnimation.Enabled = True
		Else
			chkWeaponAnimation.Enabled = False
		End If
		
		'移動アニメ表示
		If MoveAnimation Then
			chkMoveAnimation.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkMoveAnimation.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'スペシャルパワーアニメ表示
		If SpecialPowerAnimation Then
			chkSpecialPowerAnimation.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkSpecialPowerAnimation.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'マウスカーソルの自動移動
		If AutoMoveCursor Then
			chkAutoMoveCursor.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkAutoMoveCursor.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'マス目の表示
		If ShowSquareLine Then
			chkShowSquareLine.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkShowSquareLine.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'味方フェイズ開始時のターン表示
		If LCase(ReadIni("Option", "Turn")) = "on" Then
			chkShowTurn.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkShowTurn.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'敵フェイズ中にＢＧＭを変更しない
		If KeepEnemyBGM Then
			chkKeepEnemyBGM.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkKeepEnemyBGM.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'MIDI演奏にDirectMusicを使用する
		If LCase(ReadIni("Option", "UseDirectMusic")) = "on" Then
			chkUseDirectMusic.CheckState = System.Windows.Forms.CheckState.Checked
		Else
			chkUseDirectMusic.CheckState = System.Windows.Forms.CheckState.Unchecked
		End If
		
		'MIDI音源リセットの種類
		cboMidiReset.Items.Add("None")
		cboMidiReset.Items.Add("GM")
		cboMidiReset.Items.Add("GS")
		cboMidiReset.Items.Add("XG")
		cboMidiReset.Text = MidiResetType
		
		'MP3音量
		SavedMP3Volume = MP3Volume
		txtMP3Volume.Text = VB6.Format(MP3Volume)
	End Sub
	
	'MP3音量変更
	'UPGRADE_NOTE: hscMP3Volume.Change はイベントからプロシージャに変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' をクリックしてください。
	'UPGRADE_WARNING: HScrollBar イベント hscMP3Volume.Change には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
	Private Sub hscMP3Volume_Change(ByVal newScrollValue As Integer)
		Dim IsMP3Supported As Object
		MP3Volume = newScrollValue
		txtMP3Volume.Text = VB6.Format(MP3Volume)
		'UPGRADE_WARNING: オブジェクト IsMP3Supported の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		If IsMP3Supported Then
			Call vbmp3_setVolume(MP3Volume, MP3Volume)
		End If
	End Sub
	
	'UPGRADE_NOTE: hscMP3Volume.Scroll はイベントからプロシージャに変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="4E2DC008-5EDA-4547-8317-C9316952674F"' をクリックしてください。
	Private Sub hscMP3Volume_Scroll_Renamed(ByVal newScrollValue As Integer)
		Dim IsMP3Supported As Object
		MP3Volume = newScrollValue
		txtMP3Volume.Text = VB6.Format(MP3Volume)
		'UPGRADE_WARNING: オブジェクト IsMP3Supported の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		If IsMP3Supported Then
			Call vbmp3_setVolume(MP3Volume, MP3Volume)
		End If
	End Sub
	
	'UPGRADE_WARNING: イベント txtMP3Volume.TextChanged は、フォームが初期化されたときに発生します。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="88B12AE1-6DE0-48A0-86F1-60C0686C026A"' をクリックしてください。
	Private Sub txtMP3Volume_TextChanged(ByVal eventSender As System.Object, ByVal eventArgs As System.EventArgs) Handles txtMP3Volume.TextChanged
		Dim IsMP3Supported As Object
		MP3Volume = StrToLng((txtMP3Volume.Text))
		
		If MP3Volume < 0 Then
			MP3Volume = 0
			txtMP3Volume.Text = "0"
		ElseIf MP3Volume > 100 Then 
			MP3Volume = 100
			txtMP3Volume.Text = "100"
		End If
		
		hscMP3Volume.Value = MP3Volume
		
		'UPGRADE_WARNING: オブジェクト IsMP3Supported の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		If IsMP3Supported Then
			Call vbmp3_setVolume(MP3Volume, MP3Volume)
		End If
	End Sub
	Private Sub hscMP3Volume_Scroll(ByVal eventSender As System.Object, ByVal eventArgs As System.Windows.Forms.ScrollEventArgs) Handles hscMP3Volume.Scroll
		Select Case eventArgs.type
			Case System.Windows.Forms.ScrollEventType.ThumbTrack
				hscMP3Volume_Scroll_Renamed(eventArgs.newValue)
			Case System.Windows.Forms.ScrollEventType.EndScroll
				hscMP3Volume_Change(eventArgs.newValue)
		End Select
	End Sub
End Class