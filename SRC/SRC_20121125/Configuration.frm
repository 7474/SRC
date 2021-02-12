VERSION 5.00
Begin VB.Form frmConfiguration 
   BackColor       =   &H00C0C0C0&
   BorderStyle     =   3  '固定ﾀﾞｲｱﾛｸﾞ
   Caption         =   "設定変更"
   ClientHeight    =   6075
   ClientLeft      =   45
   ClientTop       =   435
   ClientWidth     =   5190
   ForeColor       =   &H00000000&
   Icon            =   "Configuration.frx":0000
   LinkTopic       =   "Form1"
   MaxButton       =   0   'False
   MinButton       =   0   'False
   ScaleHeight     =   6075
   ScaleWidth      =   5190
   ShowInTaskbar   =   0   'False
   StartUpPosition =   3  'Windows の既定値
   Begin VB.CheckBox chkExtendedAnimation 
      BackColor       =   &H00C0C0C0&
      Caption         =   "戦闘アニメの拡張機能を使用する"
      ForeColor       =   &H00000000&
      Height          =   495
      Left            =   720
      TabIndex        =   18
      Top             =   960
      Width           =   3495
   End
   Begin VB.CheckBox chkMoveAnimation 
      BackColor       =   &H00C0C0C0&
      Caption         =   "移動アニメを表示する"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   17
      Top             =   2280
      Width           =   3735
   End
   Begin VB.CheckBox chkWeaponAnimation 
      BackColor       =   &H00C0C0C0&
      Caption         =   "武器準備アニメを自動選択表示する"
      ForeColor       =   &H00000000&
      Height          =   495
      Left            =   720
      TabIndex        =   16
      Top             =   1440
      Width           =   3495
   End
   Begin VB.TextBox txtMP3Volume 
      Alignment       =   2  '中央揃え
      BackColor       =   &H00FFFFFF&
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   285
      Left            =   1305
      TabIndex        =   11
      Text            =   "100"
      Top             =   4905
      Width           =   495
   End
   Begin VB.HScrollBar hscMP3Volume 
      Height          =   255
      LargeChange     =   10
      Left            =   1920
      Max             =   100
      TabIndex        =   12
      Top             =   4920
      Value           =   50
      Width           =   2295
   End
   Begin VB.ComboBox cboMidiReset 
      BackColor       =   &H00FFFFFF&
      ForeColor       =   &H00000000&
      Height          =   300
      Left            =   2520
      TabIndex        =   10
      Top             =   4440
      Width           =   1725
   End
   Begin VB.CheckBox chkUseDirectMusic 
      BackColor       =   &H00C0C0C0&
      Caption         =   "MIDI演奏にDirectMusicを使用する (要再起動)"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   9
      Top             =   4080
      Width           =   4215
   End
   Begin VB.CommandButton cmdCancel 
      Appearance      =   0  'ﾌﾗｯﾄ
      BackColor       =   &H00C0C0C0&
      Caption         =   "キャンセル"
      Height          =   375
      Left            =   3240
      Style           =   1  'ｸﾞﾗﾌｨｯｸｽ
      TabIndex        =   14
      Top             =   5400
      Width           =   1455
   End
   Begin VB.CommandButton cmdOK 
      Appearance      =   0  'ﾌﾗｯﾄ
      BackColor       =   &H00C0C0C0&
      Caption         =   "OK"
      Height          =   375
      Left            =   1680
      Style           =   1  'ｸﾞﾗﾌｨｯｸｽ
      TabIndex        =   13
      Top             =   5400
      Width           =   1455
   End
   Begin VB.CheckBox chkKeepEnemyBGM 
      BackColor       =   &H00C0C0C0&
      Caption         =   "敵フェイズ中にＢＧＭを変更しない"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   8
      Top             =   3720
      Width           =   3735
   End
   Begin VB.ComboBox cboMessageSpeed 
      BackColor       =   &H00FFFFFF&
      ForeColor       =   &H00000000&
      Height          =   300
      Left            =   2160
      TabIndex        =   2
      Top             =   240
      Width           =   2055
   End
   Begin VB.CheckBox chkAutoMoveCursor 
      BackColor       =   &H00C0C0C0&
      Caption         =   "マウスカーソルを自動的に移動する"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   5
      Top             =   2640
      Width           =   3735
   End
   Begin VB.CheckBox chkShowSquareLine 
      BackColor       =   &H00C0C0C0&
      Caption         =   "マス目を表示する (要再起動)"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   6
      Top             =   3000
      Width           =   3975
   End
   Begin VB.CheckBox chkShowTurn 
      BackColor       =   &H00C0C0C0&
      Caption         =   "味方フェイズ開始時にターン表示を行う"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   7
      Top             =   3360
      Width           =   3735
   End
   Begin VB.CheckBox chkSpecialPowerAnimation 
      BackColor       =   &H00C0C0C0&
      Caption         =   "スペシャルパワーアニメを表示する"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   4
      Top             =   1920
      Width           =   3735
   End
   Begin VB.CheckBox chkBattleAnimation 
      BackColor       =   &H00C0C0C0&
      Caption         =   "戦闘アニメを表示する"
      ForeColor       =   &H00000000&
      Height          =   375
      Left            =   480
      TabIndex        =   3
      Top             =   600
      Width           =   3735
   End
   Begin VB.Label labMP3Volume 
      BackStyle       =   0  '透明
      Caption         =   "MP3音量"
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   495
      TabIndex        =   15
      Top             =   4950
      Width           =   735
   End
   Begin VB.Label labMidiReset 
      BackStyle       =   0  '透明
      Caption         =   "MIDI音源リセットの種類"
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   495
      TabIndex        =   1
      Top             =   4515
      Width           =   2880
   End
   Begin VB.Label labMessageSpeed 
      BackStyle       =   0  '透明
      Caption         =   "メッセージスピード"
      BeginProperty Font 
         Name            =   "ＭＳ Ｐゴシック"
         Size            =   9.75
         Charset         =   128
         Weight          =   400
         Underline       =   0   'False
         Italic          =   0   'False
         Strikethrough   =   0   'False
      EndProperty
      ForeColor       =   &H00000000&
      Height          =   255
      Left            =   490
      TabIndex        =   0
      Top             =   295
      Width           =   1935
   End
End
Attribute VB_Name = "frmConfiguration"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'マップコマンド「設定変更」用ダイアログ


'MP3Volumeを記録
Private SavedMP3Volume As Integer

'戦闘アニメOn・Off切り替え
Private Sub chkBattleAnimation_Click()
    '戦闘アニメを表示しない場合は拡張戦闘アニメ、武器アニメ選択の項目を選択不能にする
    If chkBattleAnimation.Value = 1 Then
        chkExtendedAnimation.Enabled = True
        chkWeaponAnimation.Enabled = True
    Else
        chkExtendedAnimation.Enabled = False
        chkWeaponAnimation.Enabled = False
    End If
End Sub

'キャンセルボタンが押された
Private Sub cmdCancel_Click()
    'ダイアログを閉じる
    Hide
    
    'MP3音量のみその場で変更しているので元に戻す必要がある
    MP3Volume = SavedMP3Volume
    If IsMP3Supported Then
        Call vbmp3_setVolume(MP3Volume, MP3Volume)
    End If
End Sub

'OKボタンが押された
Private Sub cmdOK_Click()
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
    WriteIni "Option", "MessageWait", Format$(MessageWait)
    
    '戦闘アニメ表示
    If chkBattleAnimation.Value = 1 Then
        BattleAnimation = True
        WriteIni "Option", "BattleAnimation", "On"
    Else
        BattleAnimation = False
        WriteIni "Option", "BattleAnimation", "Off"
    End If
    
    '拡大戦闘アニメ表示
    If chkExtendedAnimation.Value = 1 Then
        ExtendedAnimation = True
        WriteIni "Option", "ExtendedAnimation", "On"
    Else
        ExtendedAnimation = False
        WriteIni "Option", "Extendednimation", "Off"
    End If
    
    '武器準備アニメ表示
    If chkWeaponAnimation.Value = 1 Then
        WeaponAnimation = True
        WriteIni "Option", "WeaponAnimation", "On"
    Else
        WeaponAnimation = False
        WriteIni "Option", "WeaponAnimation", "Off"
    End If
    
    '移動アニメ表示
    If chkMoveAnimation.Value = 1 Then
        MoveAnimation = True
        WriteIni "Option", "MoveAnimation", "On"
    Else
        MoveAnimation = False
        WriteIni "Option", "MoveAnimation", "Off"
    End If
    
    'スペシャルパワーアニメ表示
    If chkSpecialPowerAnimation.Value = 1 Then
        SpecialPowerAnimation = True
        WriteIni "Option", "SpecialPowerAnimation", "On"
    Else
        SpecialPowerAnimation = False
        WriteIni "Option", "SpecialPowerAnimation", "Off"
    End If
    
    'マウスカーソルの自動移動
    If chkAutoMoveCursor.Value Then
        AutoMoveCursor = True
        WriteIni "Option", "AutoMoveCursor", "On"
    Else
        AutoMoveCursor = False
        WriteIni "Option", "AutoMoveCursor", "Off"
    End If
    
    'マス目の表示
    If chkShowSquareLine.Value Then
        ShowSquareLine = True
        WriteIni "Option", "Square", "On"
    Else
        ShowSquareLine = False
        WriteIni "Option", "Square", "Off"
    End If
    
    '味方フェイズ開始時のターン表示
    If chkShowTurn.Value Then
        WriteIni "Option", "Turn", "On"
    Else
        WriteIni "Option", "Turn", "Off"
    End If
    
    '敵フェイズ中にＢＧＭを変更しない
    If chkKeepEnemyBGM.Value Then
        KeepEnemyBGM = True
        WriteIni "Option", "KeepEnemyBGM", "On"
    Else
        KeepEnemyBGM = False
        WriteIni "Option", "KeepEnemyBGM", "Off"
    End If
    
    'MIDI演奏にDirectMusicを使用する
    If chkUseDirectMusic.Value Then
        WriteIni "Option", "UseDirectMusic", "On"
    Else
        WriteIni "Option", "UseDirectMusic", "Off"
    End If
    
    'MIDI音源リセットの種類
    MidiResetType = cboMidiReset.Text
    WriteIni "Option", "MidiReset", cboMidiReset.Text
    
    'MP3再生音量
    WriteIni "Option", "MP3Volume", Format$(MP3Volume)
    
    'ダイアログを閉じる
    Hide
End Sub

Private Sub Form_Load()
    'ダイアログを初期化
    
    'メッセージスピード
    cboMessageSpeed.AddItem "手動送り"
    cboMessageSpeed.AddItem "低速"
    cboMessageSpeed.AddItem "普通"
    cboMessageSpeed.AddItem "高速"
    cboMessageSpeed.AddItem "超高速"
    cboMessageSpeed.AddItem "神の領域"
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
        chkBattleAnimation.Value = 1
    Else
        chkBattleAnimation.Value = 0
    End If
    If Not FileExists(AppPath & "Lib\汎用戦闘アニメ\include.eve") Then
        chkBattleAnimation.Value = 2 '無効
    End If
    
    '拡張戦闘アニメ表示
    If ExtendedAnimation Then
        chkExtendedAnimation.Value = 1
    Else
        chkExtendedAnimation.Value = 0
    End If
    If chkBattleAnimation.Value = 1 Then
        chkExtendedAnimation.Enabled = True
    Else
        chkExtendedAnimation.Enabled = False
    End If
    
    '武器準備アニメ表示
    If WeaponAnimation Then
        chkWeaponAnimation.Value = 1
    Else
        chkWeaponAnimation.Value = 0
    End If
    If chkBattleAnimation.Value = 1 Then
        chkWeaponAnimation.Enabled = True
    Else
        chkWeaponAnimation.Enabled = False
    End If
    
    '移動アニメ表示
    If MoveAnimation Then
        chkMoveAnimation.Value = 1
    Else
        chkMoveAnimation.Value = 0
    End If
    
    'スペシャルパワーアニメ表示
    If SpecialPowerAnimation Then
        chkSpecialPowerAnimation.Value = 1
    Else
        chkSpecialPowerAnimation.Value = 0
    End If
    
    'マウスカーソルの自動移動
    If AutoMoveCursor Then
        chkAutoMoveCursor.Value = 1
    Else
        chkAutoMoveCursor.Value = 0
    End If
    
    'マス目の表示
    If ShowSquareLine Then
        chkShowSquareLine.Value = 1
    Else
        chkShowSquareLine.Value = 0
    End If
    
    '味方フェイズ開始時のターン表示
    If LCase$(ReadIni("Option", "Turn")) = "on" Then
        chkShowTurn.Value = 1
    Else
        chkShowTurn.Value = 0
    End If
    
    '敵フェイズ中にＢＧＭを変更しない
    If KeepEnemyBGM Then
        chkKeepEnemyBGM.Value = 1
    Else
        chkKeepEnemyBGM.Value = 0
    End If
    
    'MIDI演奏にDirectMusicを使用する
    If LCase$(ReadIni("Option", "UseDirectMusic")) = "on" Then
        chkUseDirectMusic.Value = 1
    Else
        chkUseDirectMusic.Value = 0
    End If
    
    'MIDI音源リセットの種類
    cboMidiReset.AddItem "None"
    cboMidiReset.AddItem "GM"
    cboMidiReset.AddItem "GS"
    cboMidiReset.AddItem "XG"
    cboMidiReset.Text = MidiResetType
    
    'MP3音量
    SavedMP3Volume = MP3Volume
    txtMP3Volume = Format$(MP3Volume)
End Sub

'MP3音量変更
Private Sub hscMP3Volume_Change()
    MP3Volume = hscMP3Volume.Value
    txtMP3Volume.Text = Format$(MP3Volume)
    If IsMP3Supported Then
        Call vbmp3_setVolume(MP3Volume, MP3Volume)
    End If
End Sub

Private Sub hscMP3Volume_Scroll()
    MP3Volume = hscMP3Volume.Value
    txtMP3Volume.Text = Format$(MP3Volume)
    If IsMP3Supported Then
        Call vbmp3_setVolume(MP3Volume, MP3Volume)
    End If
End Sub

Private Sub txtMP3Volume_Change()
    MP3Volume = StrToLng(txtMP3Volume.Text)
    
    If MP3Volume < 0 Then
        MP3Volume = 0
        txtMP3Volume.Text = "0"
    ElseIf MP3Volume > 100 Then
        MP3Volume = 100
        txtMP3Volume.Text = "100"
    End If
    
    hscMP3Volume.Value = MP3Volume
    
    If IsMP3Supported Then
        Call vbmp3_setVolume(MP3Volume, MP3Volume)
    End If
End Sub
