Attribute VB_Name = "SRC"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'パイロットデータのリスト
Public PDList As New PilotDataList
'ノンパイロットデータのリスト
Public NPDList As New NonPilotDataList
'ユニットデータのリスト
Public UDList As New UnitDataList
'アイテムデータのリスト
Public IDList As New ItemDataList
'メッセージデータのリスト
Public MDList As New MessageDataList
'特殊効果データのリスト
Public EDList As New MessageDataList
'戦闘アニメデータのリスト
Public ADList As New MessageDataList
'拡張戦闘アニメデータのリスト
Public EADList As New MessageDataList
'ダイアログデータのリスト
Public DDList As New DialogDataList
'スペシャルパワーデータのリスト
Public SPDList As New SpecialPowerDataList
'エリアスデータのリスト
Public ALDList As New AliasDataList
'地形データのリスト
Public TDList As New TerrainDataList
'バトルコンフィグデータのリスト
Public BCList As New BattleConfigDataList


'パイロットのリスト
Public PList As New Pilots
'ユニットのリスト
Public UList As New Units
'アイテムのリスト
Public IList As New Items

'イベントファイル名
Public ScenarioFileName As String
'イベントファイル名のあるフォルダ
Public ScenarioPath As String
'セーブデータのファイルディスクリプタ
Public SaveDataFileNumber As Integer
'セーブデータのバージョン
Public SaveDataVersion As Long

'そのステージが終了したかを示すフラグ
Public IsScenarioFinished As Boolean
'インターミッションコマンドによるステージかどうかを示すフラグ
Public IsSubStage As Boolean
'コマンドがキャンセルされたかどうかを示すフラグ
Public IsCanceled As Boolean

'フェイズ名
Public Stage As String
'ターン数
Public Turn As Integer
'総ターン数
Public TotalTurn As Long
'総資金
Public Money As Long
'読み込まれているデータ数
Public Titles() As String
'ローカルデータが読み込まれているか？
Public IsLocalDataLoaded As Boolean

'最新のセーブデータのファイル名
Public LastSaveDataFileName As String
'リスタート用セーブデータが利用可能かどうか
Public IsRestartSaveDataAvailable As Boolean
'クイックロード用セーブデータが利用可能かどうか
Public IsQuickSaveDataAvailable As Boolean

'システムオプション
'マス目の表示をするか
Public ShowSquareLine As Boolean
'敵フェイズにはＢＧＭを変更しないか
Public KeepEnemyBGM As Boolean
'拡張データフォルダへのパス
Public ExtDataPath As String
Public ExtDataPath2 As String
'MIDI音源リセットの種類
Public MidiResetType As String
'自動防御モードを使うか
Public AutoMoveCursor As Boolean
'スペシャルパワーアニメを表示するか
Public SpecialPowerAnimation As Boolean
'戦闘アニメを表示するか
Public BattleAnimation As Boolean
'武器準備アニメを表示するか
Public WeaponAnimation As Boolean
'拡大戦闘アニメを表示するか
Public ExtendedAnimation As Boolean
'移動アニメを表示するか
Public MoveAnimation As Boolean
'画像バッファの枚数
Public ImageBufferSize As Integer
'画像バッファの最大バイト数
Public MaxImageBufferByteSize As Long
'拡大画像を画像バッファに保存するか
Public KeepStretchedImage As Boolean
'透過描画にTransparentBltを使うか
Public UseTransparentBlt As Boolean

'SRC.exeのある場所
Public AppPath As String

'データ中にレベル指定を省略した場合のデフォルトのレベル値
Public Const DEFAULT_LEVEL = -1000

Public Sub Main()
Dim fname As String
Dim i As Integer, buf As String
Dim ret As Long
    
    '２重起動禁止
    If App.PrevInstance Then
        End
    End If
    
    'SRC.exeのある場所を調べる
    AppPath = App.Path
    If Right$(AppPath, 1) <> "\" Then
        AppPath = AppPath & "\"
    End If
    
    'SRCが正しくインストールされているかをチェック
    
    'Bitmap関係のチェック
    If Len(Dir$(AppPath & "Bitmap", vbDirectory)) = 0 Then
        ErrorMessage "Bitmapフォルダがありません。" & vbCr & vbLf _
            & "SRC.exeと同じフォルダに汎用グラフィック集をインストールしてください。"
        End
    End If
    If Len(Dir$(AppPath & "Ｂｉｔｍａｐ", vbDirectory)) > 0 Then
        ErrorMessage "Bitmapフォルダのフォルダ名が全角文字になっています。" & vbCr & vbLf _
            & AppPath & "Ｂｉｔｍａｐ" & vbCr & vbLf _
            & "フォルダ名を半角文字に直してください。"
        End
    End If
    If Len(Dir$(AppPath & "Bitmap\Bitmap", vbDirectory)) > 0 Then
        ErrorMessage "Bitmapフォルダ内にさらにBitmapフォルダが存在します。" & vbCr & vbLf _
            & AppPath & "Bitmap\Bitmap" & vbCr & vbLf _
            & "フォルダ構造を直してください。"
        End
    End If
    
    'イベントグラフィック
    If Len(Dir$(AppPath & "Bitmap\Event", vbDirectory)) = 0 Then
        ErrorMessage "Bitmap\Eventフォルダが見つかりません。" & vbCr & vbLf _
            & "汎用グラフィック集が正しくインストールされていないと思われます。"
        End
    End If
    
    'マップグラフィック
    If Len(Dir$(AppPath & "Bitmap\Map", vbDirectory)) = 0 Then
        ErrorMessage "Bitmap\Mapフォルダがありません。" & vbCr & vbLf _
            & "汎用グラフィック集が正しくインストールされていないと思われます。"
        End
    End If
    If Len(Dir$(AppPath & "Bitmap\Map\plain\plain0000.bmp")) = 0 _
        And Len(Dir$(AppPath & "Bitmap\Map\plain0000.bmp")) = 0 _
        And Len(Dir$(AppPath & "Bitmap\Map\plain0.bmp")) = 0 _
    Then
        If Len(Dir$(AppPath & "Bitmap\Map\Map", vbDirectory)) > 0 Then
            ErrorMessage "Bitmap\Mapフォルダ内にさらにMapフォルダが存在します。" & vbCr & vbLf _
                & AppPath & "Bitmap\Map\Map" & vbCr & vbLf _
                & "フォルダ構造を直してください。"
            End
        End If
        
        If Len(Dir$(AppPath & "Bitmap\Map\*", vbNormal)) = 0 Then
            ErrorMessage "Bitmap\Mapフォルダ内にファイルがありません。" & vbCr & vbLf _
                & "汎用グラフィック集が正しくインストールされていないと思われます。"
            End
        End If
        
        ErrorMessage "Bitmap\Mapフォルダ内にplain0000.bmpがありません。" & vbCr & vbLf _
            & "一部のマップ画像ファイルしかインストールされていない恐れがあります。" & vbCr & vbLf _
            & "新規インストールのファイルを使って汎用グラフィック集をインストールしてください。"
        End
    End If
    
    '効果音
    If Len(Dir$(AppPath & "Sound", vbDirectory)) = 0 Then
        ErrorMessage "Soundフォルダがありません。" & vbCr & vbLf _
            & "SRC.exeと同じフォルダに効果音集をインストールしてください。"
        End
    End If
    If Len(Dir$(AppPath & "Ｓｏｕｎｄ", vbDirectory)) > 0 Then
        ErrorMessage "Soundフォルダのフォルダ名が全角文字になっています。" & vbCr & vbLf _
            & AppPath & "Ｓｏｕｎｄ" & vbCr & vbLf _
            & "フォルダ名を半角文字に直してください。"
        End
    End If
    If Len(Dir$(AppPath & "Sound\Sound", vbDirectory)) > 0 Then
        ErrorMessage "Soundフォルダ内にさらにSoundフォルダが存在します。" & vbCr & vbLf _
            & AppPath & "Sound\Sound" & vbCr & vbLf _
            & "フォルダ構造を直してください。"
        End
    End If
    If Len(Dir$(AppPath & "Sound\*", vbNormal)) = 0 Then
        ErrorMessage "Soundフォルダ内にファイルがありません。" & vbCr & vbLf _
            & "Soundフォルダ内に効果音集をインストールしてください。"
        End
    End If
    
    'メインウィンドウのロードとFlashの登録を実施
    LoadMainFormAndRegisterFlash
    
    'Src.iniが無ければ作成
    If Not FileExists(AppPath & "Src.ini") Then
        CreateIniFile
    End If
    
    '乱数の初期化
    Randomize
    
    '時間解像度を変更する
    Call timeBeginPeriod(1)
    
    'フルスクリーンモードを使う？
    If LCase$(ReadIni("Option", "FullScreen")) = "on" Then
        ChangeDisplaySize 800, 600
    End If
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = 11
    
    'タイトル画面を表示
    OpenTitleForm
    
    'WAVE再生の手段は？
    Select Case LCase$(ReadIni("Option", "UseDirectSound"))
        Case "on"
            'DirectSoundの初期化を試みる
            InitDirectSound
        Case "off"
            UseDirectSound = False
        Case Else
            'DirectSoundの初期化を試みる
            InitDirectSound
            'DirectSoundが使用可能かどうかで設定を切り替え
'            If UseDirectSound Then
'                WriteIni "Option", "UseDirectSound", "On"
'            Else
'                WriteIni "Option", "UseDirectSound", "Off"
'            End If
    End Select
    
    'MIDI演奏の手段は？
    Select Case LCase$(ReadIni("Option", "UseDirectMusic"))
        Case "on"
            'DirectMusicの初期化を試みる
            InitDirectMusic
        Case "off"
            UseMCI = True
        Case Else
            If GetWinVersion() >= 500 Then
                'NT系のOSではデフォルトでDirectMusicを使う
                'DirectMusicの初期化を試みる
                InitDirectMusic
                'DirectMusicが使用可能かどうかで設定を切り替え
                If UseDirectMusic Then
                    WriteIni "Option", "UseDirectMusic", "On"
                Else
                    WriteIni "Option", "UseDirectMusic", "Off"
                End If
            Else
                'NT系OSでなければMCIを使う
                UseMCI = True
                WriteIni "Option", "UseDirectMusic", "Off"
            End If
    End Select
    If ReadIni("Option", "MIDIPortID") = "" Then
        WriteIni "Option", "MIDIPortID", "0"
    End If
    
    'MP3の再生音量
    buf = ReadIni("Option", "MP3Volume")
    If buf = "" Then
        WriteIni "Option", "MP3Volume", "50"
        MP3Volume = 50
    Else
        MP3Volume = StrToLng(buf)
        If MP3Volume < 0 Then
            WriteIni "Option", "MP3Volume", "0"
            MP3Volume = 0
        ElseIf MP3Volume > 100 Then
            WriteIni "Option", "MP3Volume", "100"
            MP3Volume = 100
        End If
    End If
    
    'MP3の出力フレーム数
    buf = ReadIni("Option", "MP3OutputBlock")
    If buf = "" Then
        WriteIni "Option", "MP3OutputBlock", "20"
    End If
    
    'MP3の入力直後のスリープ時間
    buf = ReadIni("Option", "MP3InputSleep")
    If buf = "" Then
        WriteIni "Option", "MP3InputSleep", "5"
    End If
    
    'ＢＧＭ用MIDIファイル設定
    If ReadIni("BGM", "Opening") = "" Then
        WriteIni "BGM", "Opening", "Opening.mid"
    End If
    If ReadIni("BGM", "Map1") = "" Then
        WriteIni "BGM", "Map1", "Map1.mid"
    End If
    If ReadIni("BGM", "Map2") = "" Then
        WriteIni "BGM", "Map2", "Map2.mid"
    End If
    If ReadIni("BGM", "Map3") = "" Then
        WriteIni "BGM", "Map3", "Map3.mid"
    End If
    If ReadIni("BGM", "Map4") = "" Then
        WriteIni "BGM", "Map4", "Map4.mid"
    End If
    If ReadIni("BGM", "Map5") = "" Then
        WriteIni "BGM", "Map5", "Map5.mid"
    End If
    If ReadIni("BGM", "Map6") = "" Then
        WriteIni "BGM", "Map6", "Map6.mid"
    End If
    If ReadIni("BGM", "Briefing") = "" Then
        WriteIni "BGM", "Briefing", "Briefing.mid"
    End If
    If ReadIni("BGM", "Intermission") = "" Then
        WriteIni "BGM", "Intermission", "Intermission.mid"
    End If
    If ReadIni("BGM", "Subtitle") = "" Then
        WriteIni "BGM", "Subtitle", "Subtitle.mid"
    End If
    If ReadIni("BGM", "End") = "" Then
        WriteIni "BGM", "End", "End.mid"
    End If
    If ReadIni("BGM", "default") = "" Then
        WriteIni "BGM", "default", "default.mid"
    End If
    
    
    '起動時の引数から読み込むファイルを探す
    If Left$(Command$(), 1) = """" Then
        fname = Mid$(Command$(), 2, Len(Command$()) - 2)
    Else
        fname = Command$()
    End If
    
    If LCase$(Right$(fname, 4)) <> ".src" _
        And LCase$(Right$(fname, 4)) <> ".eve" _
    Then
        'ダイアログを表示して読み込むファイルを指定する場合
        
        'ダイアログの初期フォルダを設定
        i = 0
        ScenarioPath = ReadIni("Log", "LastFolder")
        On Error GoTo ErrorHandler
        If ScenarioPath = "" Then
            ScenarioPath = AppPath
        ElseIf Dir$(ScenarioPath, vbDirectory) = "." Then
            If Dir$(ScenarioPath & "*.src") <> "" Then
                i = 3
            End If
            If InStr(ScenarioPath, "テストデータ") > 0 Then
                i = 2
            End If
            If InStr(ScenarioPath, "戦闘アニメテスト") > 0 Then
                i = 2
            End If
            If Dir$(ScenarioPath & "test.eve") <> "" Then
                i = 2
            End If
        Else
            ScenarioPath = AppPath
        End If
        On Error GoTo 0
        GoTo SkipErrorHandler
        
ErrorHandler:
        ScenarioPath = AppPath

SkipErrorHandler:
        If Right$(ScenarioPath, 1) <> "\" Then
            ScenarioPath = ScenarioPath & "\"
        End If
        
        '拡張データのフォルダを設定
        ExtDataPath = ReadIni("Option", "ExtDataPath")
        If Len(ExtDataPath) > 0 Then
            If Right$(ExtDataPath, 1) <> "\" Then
                ExtDataPath = ExtDataPath & "\"
            End If
        End If
        ExtDataPath2 = ReadIni("Option", "ExtDataPath2")
        If Len(ExtDataPath2) > 0 Then
            If Right$(ExtDataPath2, 1) <> "\" Then
                ExtDataPath2 = ExtDataPath2 & "\"
            End If
        End If
        
        'オープニング曲演奏
        StopBGM True
        StartBGM BGMName("Opening"), True
        
        'イベントデータを初期化
        InitEventData
        
        'タイトル画面を閉じる
        CloseTitleForm
        
        'マウスカーソルを元に戻す
        Screen.MousePointer = 0
        
        'シナリオパスは変更される可能性があるので、MIDIファイルのサーチパスをリセット
        ResetMidiSearchPath
        
        'プレイヤーにロードするファイルを尋ねる
        fname = LoadFileDialog("シナリオ／セーブファイルの指定", _
            ScenarioPath, "", i, "ｲﾍﾞﾝﾄﾃﾞｰﾀ", "eve", "ｾｰﾌﾞﾃﾞｰﾀ", "src")
        
        'ファイルが指定されなかった場合はそのまま終了
        If fname = "" Then
            TerminateSRC
            End
        End If
        
        'シナリオのあるフォルダのパスを収得
        If InStr(fname, "\") > 0 Then
            For i = 1 To Len(fname)
                If Mid$(fname, Len(fname) - i + 1, 1) = "\" Then
                    Exit For
                End If
            Next
            ScenarioPath = Left$(fname, Len(fname) - i)
        Else
            ScenarioPath = AppPath
        End If
        If Right$(ScenarioPath, 1) <> "\" Then
            ScenarioPath = ScenarioPath & "\"
        End If
' ADD START MARGE
        'シナリオパスが決定した段階で拡張データフォルダパスを再設定するように変更
        '拡張データのフォルダを設定
        ExtDataPath = ReadIni("Option", "ExtDataPath")
        If Len(ExtDataPath) > 0 Then
            If Right$(ExtDataPath, 1) <> "\" Then
                ExtDataPath = ExtDataPath & "\"
            End If
        End If
        ExtDataPath2 = ReadIni("Option", "ExtDataPath2")
        If Len(ExtDataPath2) > 0 Then
            If Right$(ExtDataPath2, 1) <> "\" Then
                ExtDataPath2 = ExtDataPath2 & "\"
            End If
        End If
' ADD  END  MARGE
    Else
        'ドラッグ＆ドロップで読み込むファイルが指定された場合
        
        'ファイル名が無効の場合はそのまま終了
        If fname = "" Then
            TerminateSRC
            End
        End If
        
        'シナリオのあるフォルダのパスを収得
        If InStr(fname, "\") > 0 Then
            For i = 1 To Len(fname)
                If Mid$(fname, Len(fname) - i + 1, 1) = "\" Then
                    Exit For
                End If
            Next
            ScenarioPath = Left$(fname, Len(fname) - i)
        Else
            ScenarioPath = AppPath
        End If
        If Right$(ScenarioPath, 1) <> "\" Then
            ScenarioPath = ScenarioPath & "\"
        End If
        
        '拡張データのフォルダを設定
        ExtDataPath = ReadIni("Option", "ExtDataPath")
        If Len(ExtDataPath) > 0 Then
            If Right$(ExtDataPath, 1) <> "\" Then
                ExtDataPath = ExtDataPath & "\"
            End If
        End If
        ExtDataPath2 = ReadIni("Option", "ExtDataPath2")
        If Len(ExtDataPath2) > 0 Then
            If Right$(ExtDataPath2, 1) <> "\" Then
                ExtDataPath2 = ExtDataPath2 & "\"
            End If
        End If
        
        'オープニング曲演奏
        StopBGM True
        StartBGM BGMName("Opening"), True
        
        InitEventData
        
        CloseTitleForm
        
        Screen.MousePointer = 0
    End If
    
    'ロングネームにしておく
    fname = ScenarioPath & Dir$(fname)
    If Not FileExists(fname) Then
        ErrorMessage "指定したファイルが存在しません"
        TerminateSRC
    End If
    If InStr(fname, "不要ファイル削除") = 0 _
        And InStr(fname, "必須修正") = 0 _
    Then
        '開いたフォルダをSrc.iniにセーブしておく
        WriteIni "Log", "LastFolder", ScenarioPath
    End If
    
    'Src.iniから各種パラメータの読み込み
    
    'スペシャルパワーアニメ
    buf = ReadIni("Option", "SpecialPowerAnimation")
    If buf = "" Then
        buf = ReadIni("Option", "MindEffect")
        If buf <> "" Then
            WriteIni "Option", "SpecialPowerAnimation", buf
        End If
    End If
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            SpecialPowerAnimation = True
        Else
            SpecialPowerAnimation = False
        End If
    Else
        If SpecialPowerAnimation Then
            WriteIni "Option", "SpecialPowerAnimation", "On"
        Else
            WriteIni "Option", "SpecialPowerAnimation", "Off"
        End If
    End If
    
    '戦闘アニメ
    buf = LCase$(ReadIni("Option", "BattleAnimation"))
    If buf <> "" Then
        If buf = "on" Then
            BattleAnimation = True
        Else
            BattleAnimation = False
        End If
    Else
        If BattleAnimation Then
            WriteIni "Option", "BattleAnimation", "On"
        Else
            WriteIni "Option", "BattleAnimation", "Off"
        End If
    End If
    
    '拡大戦闘アニメ
    buf = LCase$(ReadIni("Option", "ExtendedAnimation"))
    If buf <> "" Then
        If buf = "on" Then
            ExtendedAnimation = True
        Else
            ExtendedAnimation = False
        End If
    Else
        ExtendedAnimation = True
        WriteIni "Option", "ExtendedAnimation", "On"
    End If
    
    '武器準備アニメ
    buf = LCase$(ReadIni("Option", "WeaponAnimation"))
    If buf <> "" Then
        If buf = "on" Then
            WeaponAnimation = True
        Else
            WeaponAnimation = False
        End If
    Else
        WeaponAnimation = True
        WriteIni "Option", "WeaponAnimation", "On"
    End If
    
    '移動アニメ
    buf = LCase$(ReadIni("Option", "MoveAnimation"))
    If buf <> "" Then
        If buf = "on" Then
            MoveAnimation = True
        Else
            MoveAnimation = False
        End If
    Else
        MoveAnimation = True
        WriteIni "Option", "MoveAnimation", "On"
    End If
    
    'メッセージ速度を設定
    buf = ReadIni("Option", "MessageWait")
    If IsNumeric(buf) Then
        MessageWait = CLng(buf)
        If MessageWait > 10000000 Then
            MessageWait = 10000000
        End If
    Else
        MessageWait = 700
        WriteIni "Option", "MessageWait", "700"
    End If
    
    'マス目を表示するかどうか
    buf = ReadIni("Option", "Square")
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            ShowSquareLine = True
        Else
            ShowSquareLine = False
        End If
    Else
        ShowSquareLine = False
        WriteIni "Option", "Square", "Off"
    End If
    
    '敵ターンにＢＧＭを変更するかどうか
    buf = ReadIni("Option", "KeepEnemyBGM")
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            KeepEnemyBGM = True
        Else
            KeepEnemyBGM = False
        End If
    Else
        KeepEnemyBGM = False
        WriteIni "Option", "KeepEnemyBGM", "Off"
    End If
    
    '音源のリセットデータの種類
    MidiResetType = ReadIni("Option", "MidiReset")
    
    '自動反撃モード
    buf = ReadIni("Option", "AutoDefense")
    If buf = "" Then
        buf = ReadIni("Option", "AutoDeffence")
        If buf <> "" Then
            WriteIni "Option", "AutoDefense", buf
        End If
    End If
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = True
        Else
            MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = False
        End If
    Else
        MainForm.mnuMapCommandItem(AutoDefenseCmdID).Checked = False
        WriteIni "Option", "AutoDefense", "Off"
    End If
    
    'カーソル自動移動
    buf = ReadIni("Option", "AutoMoveCursor")
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            AutoMoveCursor = True
        Else
            AutoMoveCursor = False
        End If
    Else
        AutoMoveCursor = True
        WriteIni "Option", "AutoMoveCursor", "On"
    End If
    
    '各ウィンドウをロード (メインウィンドウは先にロード済み)
    LoadForms
    
    '画像バッファの枚数
    buf = ReadIni("Option", "ImageBufferNum")
    If IsNumeric(buf) Then
        ImageBufferSize = CInt(buf)
        If ImageBufferSize < 5 Then
            '最低でも5枚のバッファを使う
            ImageBufferSize = 5
        End If
    Else
        'デフォルトは64枚
        ImageBufferSize = 64
        WriteIni "Option", "ImageBufferNum", "64"
    End If
    
    '画像バッファを作成
    MakePicBuf
    
    '画像バッファの最大サイズ
    buf = ReadIni("Option", "MaxImageBufferSize")
    If IsNumeric(buf) Then
        MaxImageBufferByteSize = CDbl(buf) * 1024 * 1024
        If MaxImageBufferByteSize < CLng(1) * 1024 * 1024 Then
            '最低でも1MBのバッファを使う
            MaxImageBufferByteSize = CLng(1) * 1024 * 1024
        End If
    Else
        'デフォルトは8MB
        MaxImageBufferByteSize = CLng(8) * 1024 * 1024
        WriteIni "Option", "MaxImageBufferSize", "8"
    End If
    
    '拡大画像を画像バッファに保存するか
    buf = ReadIni("Option", "KeepStretchedImage")
    If buf <> "" Then
        If LCase$(buf) = "on" Then
            KeepStretchedImage = True
        Else
            KeepStretchedImage = False
        End If
    Else
        If IsBitBltFasterThanStretchBlt() Then
            KeepStretchedImage = True
            WriteIni "Option", "KeepStretchedImage", "On"
        Else
            KeepStretchedImage = False
            WriteIni "Option", "KeepStretchedImage", "Off"
        End If
    End If
    
    '透過描画にUseTransparentBltを使用するか
    If GetWinVersion() >= 500 Then
        buf = ReadIni("Option", "UseTransparentBlt")
        If buf <> "" Then
            If LCase$(buf) = "on" Then
                UseTransparentBlt = True
            Else
                UseTransparentBlt = False
            End If
        Else
            UseTransparentBlt = True
            WriteIni "Option", "UseTransparentBlt", "On"
        End If
    End If
    
    
    'マウスボタンの利き腕設定
    If GetSystemMetrics(SM_SWAPBUTTON) = 0 Then
        '右利き用
        RButtonID = &H2
        LButtonID = &H1
    Else
        '左利き用
        RButtonID = &H1
        LButtonID = &H2
    End If
    
    
    ReDim ListItemComment(0)
    
    'エリアスデータをロード
    If FileExists(ScenarioPath & "Data\System\alias.txt") Then
        ALDList.Load ScenarioPath & "Data\System\alias.txt"
    ElseIf FileExists(AppPath & "Data\System\alias.txt") Then
        ALDList.Load AppPath & "Data\System\alias.txt"
    End If
    'スペシャルパワーデータをロード
    If FileExists(ScenarioPath & "Data\System\sp.txt") Then
        SPDList.Load ScenarioPath & "Data\System\sp.txt"
    ElseIf FileExists(ScenarioPath & "Data\System\mind.txt") Then
        SPDList.Load ScenarioPath & "Data\System\mind.txt"
    ElseIf FileExists(AppPath & "Data\System\sp.txt") Then
        SPDList.Load AppPath & "Data\System\sp.txt"
    ElseIf FileExists(AppPath & "Data\System\mind.txt") Then
        SPDList.Load AppPath & "Data\System\mind.txt"
    End If
    '汎用アイテムデータをロード
    If FileExists(ScenarioPath & "Data\System\item.txt") Then
        IDList.Load ScenarioPath & "Data\System\item.txt"
    ElseIf FileExists(AppPath & "Data\System\item.txt") Then
        IDList.Load AppPath & "Data\System\item.txt"
    End If
    '地形データをロード
    If FileExists(AppPath & "Data\System\terrain.txt") Then
        TDList.Load AppPath & "Data\System\terrain.txt"
    Else
        ErrorMessage "地形データファイル「Data\System\terrain.txt」が見つかりません"
        TerminateSRC
    End If
    If FileExists(ScenarioPath & "Data\System\terrain.txt") Then
        TDList.Load ScenarioPath & "Data\System\terrain.txt"
    End If
    'バトルコンフィグデータをロード
    If FileExists(ScenarioPath & "Data\System\battle.txt") Then
        BCList.Load ScenarioPath & "Data\System\battle.txt"
    ElseIf FileExists(AppPath & "Data\System\battle.txt") Then
        BCList.Load AppPath & "Data\System\battle.txt"
    End If
    
    'マップを初期化
    InitMap
    
    '乱数系列を初期化
    RndSeed = Int(1000000 * Rnd)
    RndReset
    
    If LCase$(Right$(fname, 4)) = ".src" Then
        SaveDataFileNumber = FreeFile
        Open fname For Input As #SaveDataFileNumber
        '第１項目を読み込み
        Input #SaveDataFileNumber, buf
        '第１項目はセーブデータバージョン？
        If IsNumeric(buf) Then
            If CLng(buf) > 10000 Then
                'バージョンデータであれば第２項目を読み込み
                Input #SaveDataFileNumber, buf
            End If
        End If
        Close #SaveDataFileNumber
        
        'データの種類を判定
        If IsNumeric(buf) Then
            'セーブデータの読み込み
            OpenNowLoadingForm
            LoadData fname
            CloseNowLoadingForm
            
            'インターミッション
            InterMissionCommand True
            
            If Not IsSubStage Then
                If GetValueAsString("次ステージ") = "" Then
                    ErrorMessage "次のステージのファイル名が設定されていません"
                    TerminateSRC
                End If
                
                StartScenario GetValueAsString("次ステージ")
            Else
                IsSubStage = False
            End If
        Else
            '中断データの読み込み
            LockGUI
            
            RestoreData fname, False
            
            '画面を書き直してステータスを表示
            RedrawScreen
            DisplayGlobalStatus
            
            UnlockGUI
        End If
    ElseIf LCase$(Right$(fname, 4)) = ".eve" Then
        'イベントファイルの実行
        StartScenario fname
    Else
        ErrorMessage "「" & fname & "」はSRC用のファイルではありません！" & vbCr & vbLf _
            & "拡張子が「.eve」のイベントファイル、" _
            & "または拡張子が「.src」のセーブデータファイルを指定して下さい。"
        TerminateSRC
    End If
End Sub

'INIファイルを作成する
Public Sub CreateIniFile()
Dim f As Integer

    On Error GoTo ErrorHandler
    
    f = FreeFile
    Open AppPath & "Src.ini" For Output Access Write As #f
    
    Print #f, ";SRCの設定ファイルです。"
    Print #f, ";項目の内容に関してはヘルプの"
    Print #f, "; 操作方法 => マップコマンド => 設定変更"
    Print #f, ";の項を参照して下さい。"
    Print #f, ""
    Print #f, "[Option]"
    Print #f, ";メッセージのウェイト。標準は700"
    Print #f, "MessageWait=700"
    Print #f, ""
    Print #f, ";ターン数の表示 [On|Off]"
    Print #f, "Turn=Off"
    Print #f, ""
    Print #f, ";マス目の表示 [On|Off]"
    Print #f, "Square=Off"
    Print #f, ""
    Print #f, ";敵フェイズにはＢＧＭを変更しない [On|Off]"
    Print #f, "KeepEnemyBGM=Off"
    Print #f, ""
    Print #f, ";自動防御モード [On|Off]"
    Print #f, "AutoDefense=Off"
    Print #f, ""
    Print #f, ";自動カーソル移動 [On|Off]"
    Print #f, "AutoMoveCursor=On"
    Print #f, ""
    Print #f, ";スペシャルパワーアニメ [On|Off]"
    Print #f, "SpecialPowerAnimation=On"
    Print #f, ""
    Print #f, ";戦闘アニメ [On|Off]"
    Print #f, "BattleAnimation=On"
    Print #f, ""
    Print #f, ";戦闘アニメの拡張機能 [On|Off]"
    Print #f, "ExtendedAnimation=On"
    Print #f, ""
    Print #f, ";武器準備アニメの自動選択表示 [On|Off]"
    Print #f, "WeaponAnimation=On"
    Print #f, ""
    Print #f, ";移動アニメ [On|Off]"
    Print #f, "MoveAnimation=On"
    Print #f, ""
    Print #f, ";MIDI音源リセットの種類 [None|GM|GS|XG]"
    Print #f, "MidiReset=None"
    Print #f, ""
    Print #f, ";MIDI演奏にDirectMusicを使う [On|Off]"
    If GetWinVersion() >= 500 Then
        'NT系のOSではデフォルトでDirectMusicを使う
        'DirectMusicの初期化を試みる
        InitDirectMusic
        'DirectMusicが使用可能かどうかで設定を切り替え
        If UseDirectMusic Then
            Print #f, "UseDirectMusic=On"
        Else
            Print #f, "UseDirectMusic=Off"
        End If
    Else
        'NT系OSでなければMCIを使う
        UseMCI = True
        Print #f, "UseDirectMusic=Off"
    End If
    Print #f, ""
    Print #f, ";DirectMusicで使うMIDI音源のポート番号 [自動検索=0]"
    Print #f, "MIDIPortID=0"
    Print #f, ""
    Print #f, ";MP3再生時の音量 (0～100)"
    Print #f, "MP3Volume=50"
    Print #f, ""
    Print #f, ";MP3の出力フレーム数"
    Print #f, "MP3OutputBlock=20"
    Print #f, ""
    Print #f, ";MP3の入力直後のスリープ時間(ミリ秒)"
    Print #f, "MP3IutputSleep=5"
    Print #f, ""
'    Print #f, ";WAV再生にDirectSoundを使う [On|Off]"
'    Print #f, "UseDirectSound=On"
'    Print #f, ""
    Print #f, ";画像バッファの枚数"
    Print #f, "ImageBufferNum=64"
    Print #f, ""
    Print #f, ";画像バッファの最大サイズ (MB)"
    Print #f, "MaxImageBufferSize=8"
    Print #f, ""
    Print #f, ";拡大画像を画像バッファに保存する [On|Off]"
    Print #f, "KeepStretchedImage="
    Print #f, ""
    If GetWinVersion() >= 500 Then
        Print #f, ";透過描画にAPI関数TransparentBltを使う [On|Off]"
        Print #f, "UseTransparentBlt=On"
        Print #f, ""
    End If
    Print #f, ";拡張データのフォルダ (フルパスで指定)"
    Print #f, "ExtDataPath="
    Print #f, "ExtDataPath2="
    Print #f, ""
    Print #f, ";デバッグモード [On|Off]"
    Print #f, "DebugMode=Off"
    Print #f, ""
    Print #f, ";新ＧＵＩ(テスト中) [On|Off]"
    Print #f, "NewGUI=Off"
    Print #f, ""
    Print #f, "[Log]"
    Print #f, ";前回使用したフォルダ"
    Print #f, "LastFolder="
    Print #f, ""
    Print #f, "[BGM]"
    Print #f, ";SRC起動時"
    Print #f, "Opening=Opening.mid"
    Print #f, ";味方フェイズ開始時"
    Print #f, "Map1=Map1.mid"
    Print #f, ";敵フェイズ開始時"
    Print #f, "Map2=Map2.mid"
    Print #f, ";屋内マップの味方フェイズ開始時"
    Print #f, "Map3=Map3.mid"
    Print #f, ";屋内マップの敵フェイズ開始時"
    Print #f, "Map4=Map4.mid"
    Print #f, ";宇宙マップの味方フェイズ開始時"
    Print #f, "Map5=Map5.mid"
    Print #f, ";宇宙マップの敵フェイズ開始時"
    Print #f, "Map6=Map6.mid"
    Print #f, ";プロローグ・エピローグ開始時"
    Print #f, "Briefing=Briefing.mid"
    Print #f, ";インターミッション開始時"
    Print #f, "Intermission=Intermission.mid"
    Print #f, ";テロップ表示時"
    Print #f, "Subtitle=Subtitle.mid"
    Print #f, ";ゲームオーバー時"
    Print #f, "End=End.mid"
    Print #f, ";戦闘時のデフォルトMIDI"
    Print #f, "default=default.mid"
    Print #f, ""
    
    Close #f
    
ErrorHandler:
    'エラー発生
End Sub

'KeepStretchedImageを使用すべきか決定するため、BitBltと
'StretchBltの速度差を測定
Private Function IsBitBltFasterThanStretchBlt()
Dim stime As Long, etime As Long
Dim bb_time As Long, sb_time As Long
Dim ret As Long, i As Integer
  
    With MainForm
        '描画領域を設定
        With .picStretchedTmp(0)
            .width = 128
            .Height = 128
        End With
        With .picStretchedTmp(1)
            .width = 128
            .Height = 128
        End With
        
        'StretchBltの転送速度を測定
        stime = timeGetTime()
        For i = 1 To 5
            ret = StretchBlt(.picStretchedTmp(0).hDC, 0, 0, 480, 480, _
                .picUnit.hDC, 0, 0, 32, 32, SRCCOPY)
        Next
        etime = timeGetTime()
        sb_time = etime - stime
        
        'BitBltの転送速度を測定
        stime = timeGetTime()
        For i = 1 To 5
            ret = BitBlt(.picStretchedTmp(1).hDC, 0, 0, 480, 480, _
                .picStretchedTmp(0).hDC, 0, 0, SRCCOPY)
        Next
        etime = timeGetTime()
        bb_time = etime - stime
        
        '描画領域を開放
        With .picStretchedTmp(0)
            .Picture = LoadPicture()
            .width = 32
            .Height = 32
        End With
        With .picStretchedTmp(1)
            .Picture = LoadPicture()
            .width = 32
            .Height = 32
        End With
    End With
    
    'BitBltがStretchBltより2倍以上速ければBitBltを優先して使用する
    If 2 * bb_time < sb_time Then
        IsBitBltFasterThanStretchBlt = True
    Else
        IsBitBltFasterThanStretchBlt = False
    End If
End Function


'イベントファイルfnameを実行
Public Sub StartScenario(ByVal fname As String)
Dim i As Integer, ret As Long
Dim sf As StdFont
    
    'ファイルを検索
    If Len(fname) = 0 Then
        TerminateSRC
        End
    ElseIf FileExists(ScenarioPath & fname) Then
        fname = ScenarioPath & fname
    ElseIf FileExists(AppPath & fname) Then
        fname = AppPath & fname
    End If
    
    If Dir$(fname, vbNormal) = "" Then
        MsgBox fname & "が見つかりません"
        TerminateSRC
    End If
    
    'ウィンドウのタイトルを設定
    If App.Minor Mod 2 = 0 Then
        MainForm.Caption = "SRC"
    Else
        MainForm.Caption = "SRC開発版"
    End If
    
    ScenarioFileName = fname
    
    If Not IsSubStage Then
        If Len(Dir$(ScenarioPath & "Date", vbDirectory)) > 0 Then
            ErrorMessage "シナリオ側のDataフォルダ名がDateになっています。" & vbCr & vbLf _
                & ScenarioPath & "Date" & vbCr & vbLf _
                & "フォルダ名をDataに直してください。"
            TerminateSRC
        End If
        If Len(Dir$(ScenarioPath & "Ｄａｔａ", vbDirectory)) > 0 Then
            ErrorMessage "シナリオ側のDataフォルダ名が全角文字になっています。" & vbCr & vbLf _
                & ScenarioPath & "Ｄａｔａ" & vbCr & vbLf _
                & "フォルダ名を半角文字に直してください。"
            TerminateSRC
        End If
        If Len(Dir$(ScenarioPath & "Ｂｉｔｍａｐ", vbDirectory)) > 0 Then
            ErrorMessage "シナリオ側のBitmapフォルダ名が全角文字になっています。" & vbCr & vbLf _
                & ScenarioPath & "Ｂｉｔｍａｐ" & vbCr & vbLf _
                & "フォルダ名を半角文字に直してください。"
            TerminateSRC
        End If
        If Len(Dir$(ScenarioPath & "Ｌｉｂ", vbDirectory)) > 0 Then
            ErrorMessage "シナリオ側のLibフォルダ名が全角文字になっています。" & vbCr & vbLf _
                & ScenarioPath & "Ｌｉｂ" & vbCr & vbLf _
                & "フォルダ名を半角文字に直してください。"
            TerminateSRC
        End If
        If Len(Dir$(ScenarioPath & "Ｍｉｄｉ", vbDirectory)) > 0 Then
            ErrorMessage "シナリオ側のMidiフォルダ名が全角文字になっています。" & vbCr & vbLf _
                & ScenarioPath & "Ｍｉｄｉ" & vbCr & vbLf _
                & "フォルダ名を半角文字に直してください。"
            TerminateSRC
        End If
        If Len(Dir$(ScenarioPath & "Ｓｏｕｎｄ", vbDirectory)) > 0 Then
            ErrorMessage "シナリオ側のSoundフォルダ名が全角文字になっています。" & vbCr & vbLf _
                & ScenarioPath & "Ｓｏｕｎｄ" & vbCr & vbLf _
                & "フォルダ名を半角文字に直してください。"
            TerminateSRC
        End If
        
        '読み込むイベントファイル名に合わせて各種システム変数を設定
        If Not IsGlobalVariableDefined("次ステージ") Then
            DefineGlobalVariable "次ステージ"
        End If
        SetVariableAsString "次ステージ", ""
        For i = 1 To Len(fname)
            If Mid$(fname, Len(fname) - i + 1, 1) = "\" Then
                Exit For
            End If
        Next
        SetVariableAsString "ステージ", Mid$(fname, Len(fname) - i + 2)
        
        If Not IsGlobalVariableDefined("セーブデータファイル名") Then
            DefineGlobalVariable "セーブデータファイル名"
        End If
        SetVariableAsString "セーブデータファイル名", _
            Mid$(fname, Len(fname) - i + 2, i - 5) & "までクリア.src"
        
        'ウィンドウのタイトルにシナリオファイル名を表示
        MainForm.Caption = MainForm.Caption & " - " & Mid$(fname, Len(fname) - i + 2, i - 5)
    End If
    
    '画面をクリアしておく
    With MainForm
        ret = PatBlt(.picMain(0).hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
        ret = PatBlt(.picMain(1).hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
    End With
    ScreenIsSaved = True
    
    'イベントデータの読み込み
    LoadEventData fname
    
    '各種変数の初期化
    Turn = 0
    IsScenarioFinished = False
    IsPictureVisible = False
    IsCursorVisible = False
    LastSaveDataFileName = ""
    IsRestartSaveDataAvailable = False
    IsQuickSaveDataAvailable = False
    CommandState = "ユニット選択"
    ReDim SelectedPartners(0)
    
    'フォント設定をデフォルトに戻す
    With MainForm.picMain(0)
        .ForeColor = rgb(255, 255, 255)
        If .Font.Name <> "ＭＳ Ｐ明朝" Then
            Set sf = New StdFont
            sf.Name = "ＭＳ Ｐ明朝"
            Set .Font = sf
        End If
        .Font.Size = 16
        .Font.Bold = True
        .Font.Italic = False
        PermanentStringMode = False
        KeepStringMode = False
    End With
    
    '描画の基準座標位置をリセット
    ResetBasePoint
    
    'メモリを消費し過ぎないようにユニット画像をクリア
    If Not IsSubStage Then
        UList.ClearUnitBitmap
    End If
    
    LockGUI
    
    If MapWidth = 1 Then
        SetMapSize 15, 15
    End If
    
    'プロローグ
    Stage = "プロローグ"
    If Not IsSubStage And IsEventDefined("プロローグ", True) Then
        StopBGM
        StartBGM BGMName("Briefing")
    End If
    HandleEvent "プロローグ"
    
    If IsScenarioFinished Then
        IsScenarioFinished = False
        UnlockGUI
        Exit Sub
    End If
    
    If Not IsEventDefined("スタート") Then
        ErrorMessage "スタートイベントが定義されていません"
        TerminateSRC
    End If
    
    IsPictureVisible = False
    IsCursorVisible = False
    Stage = "味方"
    StopBGM
    
    'リスタート用にデータをセーブ
    If InStr(fname, "\Lib\ユニットステータス表示.eve") = 0 _
        And InStr(fname, "\Lib\パイロットステータス表示.eve") = 0 _
    Then
        DumpData ScenarioPath & "_リスタート.src"
    End If
    
    'スタートイベントが始まった場合は通常のステージとみなす
    IsSubStage = False
    
    ClearUnitStatus
    If Not MainForm.Visible Then
        MainForm.Show
        MainForm.Refresh
    End If
    RedrawScreen
    
    'スタートイベント
    HandleEvent "スタート"
    If IsScenarioFinished Then
        IsScenarioFinished = False
        UnlockGUI
        Exit Sub
    End If
    
    IsPictureVisible = False
    IsCursorVisible = False
    
    'クイックロードを無効にする
    IsQuickSaveDataAvailable = False
    
    StartTurn "味方"
End Sub

'陣営upartyのフェイズを実行
Public Sub StartTurn(uparty As String)
Dim i As Integer, num As Integer, phase As Integer
Dim u As Unit
    
    Stage = uparty
    BossBGM = False
    
    If uparty = "味方" Then
        Do
            '味方フェイズ
            Stage = "味方"
            
            'ターン数を進める
            If MapFileName <> "" Then
                Turn = Turn + 1
                TotalTurn = TotalTurn + 1
            End If
            
            '状態回復
            For Each SelectedUnit In UList
                With SelectedUnit
                    Select Case .Status
                        Case "出撃", "格納"
                            If .Party = uparty Then
                                If MapFileName = "" Then
                                    .UsedAction = 0
                                Else
                                    .Rest
                                End If
                                If IsScenarioFinished Then
                                    UnlockGUI
                                    Exit Sub
                                End If
                            Else
                                .UsedAction = 0
                            End If
                        Case "旧主形態", "旧形態"
                            .UsedAction = 0
                    End Select
                End With
            Next
            
            '味方が敵にかけたスペシャルパワーを解除
            For Each u In UList
                With u
                    Select Case .Status
                        Case "出撃", "格納"
                            .RemoveSpecialPowerInEffect "敵ターン"
                    End Select
                End With
            Next
            RedrawScreen
            
            '味方フェイズ用ＢＧＭを演奏
            If MapFileName <> "" Then
                Select Case TerrainClass(1, 1)
                    Case "屋内"
                        StartBGM BGMName("Map3")
                    Case "宇宙"
                        StartBGM BGMName("Map5")
                    Case Else
                        If TerrainName(1, 1) = "壁" Then
                            StartBGM BGMName("Map3")
                        Else
                            StartBGM BGMName("Map1")
                        End If
                End Select
            End If
            
            'ターンイベント
            IsUnitCenter = False
            HandleEvent "ターン", Turn, "味方"
            If IsScenarioFinished Then
                UnlockGUI
                Exit Sub
            End If
            
            '操作可能なユニットがいるかどうかチェック
            num = 0
            For Each u In UList
                With u
                    If .Party = "味方" _
                        And (.Status = "出撃" Or .Status = "格納") _
                        And .Action > 0 _
                    Then
                        num = num + 1
                    End If
                End With
            Next
            If num > 0 Or IsOptionDefined("味方フェイズ強制発動") Then
                Exit Do
            End If
            
            'CPUが操作するユニットがいるかどうかチェック
            num = 0
            For Each u In UList
                With u
                    If .Party <> "味方" _
                        And .Status = "出撃" _
                    Then
                        num = num + 1
                    End If
                End With
            Next
            If num = 0 Then
                Exit Do
            End If
            
            '敵フェイズ
            StartTurn "敵"
            If IsScenarioFinished Then
                IsScenarioFinished = False
                Exit Sub
            End If
            
            '中立フェイズ
            StartTurn "中立"
            If IsScenarioFinished Then
                IsScenarioFinished = False
                Exit Sub
            End If
            
            'ＮＰＣフェイズ
            StartTurn "ＮＰＣ"
            If IsScenarioFinished Then
                IsScenarioFinished = False
                Exit Sub
            End If
        Loop While True
    Else
        '味方フェイズ以外
        
        '状態回復
        For Each SelectedUnit In UList
            With SelectedUnit
                Select Case .Status
                    Case "出撃", "格納"
                        If .Party = uparty Then
                            .Rest
                            If IsScenarioFinished Then
                                UnlockGUI
                                Exit Sub
                            End If
                        Else
                            .UsedAction = 0
                        End If
                    Case "旧主形態", "旧形態"
                        .UsedAction = 0
                End Select
            End With
        Next
        
        '敵ユニットが味方にかけたスペシャルパワーを解除
        For Each u In UList
            With u
                Select Case .Status
                    Case "出撃", "格納"
                        .RemoveSpecialPowerInEffect "敵ターン"
                End Select
            End With
        Next
        RedrawScreen
        
        '敵(ＮＰＣ)フェイズ用ＢＧＭを演奏
        Select Case TerrainClass(1, 1)
            Case "屋内"
                If Stage = "ＮＰＣ" Then
                    StartBGM BGMName("Map3")
                Else
                    StartBGM BGMName("Map4")
                End If
            Case "宇宙"
                If Stage = "ＮＰＣ" Then
                    StartBGM BGMName("Map5")
                Else
                    StartBGM BGMName("Map6")
                End If
            Case Else
                If Stage = "ＮＰＣ" Then
                    If TerrainName(1, 1) = "壁" Then
                        StartBGM BGMName("Map3")
                    Else
                        StartBGM BGMName("Map1")
                    End If
                Else
                    If TerrainName(1, 1) = "壁" Then
                        StartBGM BGMName("Map4")
                    Else
                        StartBGM BGMName("Map2")
                    End If
                End If
        End Select
        
        'ターンイベント
        HandleEvent "ターン", Turn, uparty
        If IsScenarioFinished Then
            UnlockGUI
            Exit Sub
        End If
    End If
    
    If uparty = "味方" Then
        '味方フェイズのプレイヤーによるユニット操作前の処理
        
        'ターン数を表示
        If Turn > 1 And IsOptionDefined("デバッグ") Then
            DisplayTelop "ターン" & Format$(Turn)
        End If
        
        '通常のステージでは母艦ユニットまたはレベルがもっとも高い
        'ユニットを中央に配置
        If MapFileName <> "" And Not IsUnitCenter Then
            Dim max_lv As Integer, max_unit As Unit
            
            For Each u In UList
                With u
                    If .Party = "味方" And .Status = "出撃" And .Action > 0 Then
                        If .IsFeatureAvailable("母艦") Then
                            Center .X, .Y
                            DisplayUnitStatus u
                            RedrawScreen
                            UnlockGUI
                            Exit Sub
                        End If
                    End If
                End With
            Next
            
            max_lv = 0
            For Each u In UList
                With u
                    If .Party = "味方" And .Status = "出撃" Then
                        If .MainPilot.Level > max_lv Then
                            Set max_unit = u
                            max_lv = .MainPilot.Level
                        End If
                    End If
                End With
            Next
            If Not max_unit Is Nothing Then
                Center max_unit.X, max_unit.Y
            End If
        End If
        
        'ステータスを表示
        If MapFileName <> "" Then
            DisplayGlobalStatus
        End If
        
        'プレイヤーによる味方ユニット操作に移行
        RedrawScreen
        DoEvents
        UnlockGUI
        Exit Sub
    End If
    
    LockGUI
    
    'CPUによるユニット操作
    For phase = 1 To 5
    For i = 1 To UList.Count
        'フェイズ中に行動するユニットを選択
        Set SelectedUnit = UList.Item(i)
        
        With SelectedUnit
            If .Status <> "出撃" Then
                GoTo NextLoop
            End If
            
            If .Action = 0 Then
                GoTo NextLoop
            End If
            
            If .Party <> uparty Then
                GoTo NextLoop
            End If
            
            Set u = SelectedUnit
            
            '他のユニットを護衛しているユニットは護衛対象と同じ順に行動
            If PList.IsDefined(.Mode) Then
                With PList.Item(.Mode)
                    If Not .Unit Is Nothing Then
                        If .Unit.Party = uparty Then
                            Set u = .Unit
                        End If
                    End If
                End With
            End If
            If PList.IsDefined(u.Mode) Then
                With PList.Item(u.Mode)
                    If Not .Unit Is Nothing Then
                        If .Unit.Party = uparty Then
                            Set u = .Unit
                        End If
                    End If
                End With
            End If
            
            With u
                Select Case phase
                    Case 1
                        '最初にサポート能力を持たないザコユニットが行動
                        If .BossRank >= 0 Then
                            GoTo NextLoop
                        End If
                        With .MainPilot
                            If .IsSkillAvailable("援護") _
                                Or .IsSkillAvailable("援護攻撃") _
                                Or .IsSkillAvailable("援護防御") _
                                Or .IsSkillAvailable("統率") _
                                Or .IsSkillAvailable("指揮") _
                                Or .IsSkillAvailable("広域サポート") _
                            Then
                                GoTo NextLoop
                            End If
                        End With
                    Case 2
                        '次にサポート能力を持たないボスユニットが行動
                        With .MainPilot
                            If .IsSkillAvailable("援護") _
                                Or .IsSkillAvailable("援護攻撃") _
                                Or .IsSkillAvailable("援護防御") _
                                Or .IsSkillAvailable("統率") _
                                Or .IsSkillAvailable("指揮") _
                                Or .IsSkillAvailable("広域サポート") _
                            Then
                                GoTo NextLoop
                            End If
                        End With
                    Case 3
                        '次に統率能力を持つユニットが行動
                        If Not .MainPilot.IsSkillAvailable("統率") Then
                            GoTo NextLoop
                        End If
                    Case 4
                        '次にサポート能力を持つザコユニットが行動
                        If .BossRank >= 0 Then
                            GoTo NextLoop
                        End If
                    Case 5
                        '最後にサポート能力を持つボスユニットが行動
                End Select
            End With
        End With
        
        Do While SelectedUnit.Action > 0
            '途中で状態が変更された場合
            If SelectedUnit.Status <> "出撃" Then
                Exit Do
            End If
            
            '途中で陣営が変更された場合
            If SelectedUnit.Party <> uparty Then
                Exit Do
            End If
            
            If Not IsRButtonPressed Then
                DisplayUnitStatus SelectedUnit
                Center SelectedUnit.X, SelectedUnit.Y
                RedrawScreen
                DoEvents
            End If
            
            IsCanceled = False 'Cancelコマンドのクリア
            
            'ユニットを行動させる
            OperateUnit
            
            If IsScenarioFinished Then
                Exit Sub
            End If
            
            'ハイパーモード・ノーマルモードの自動発動チェック
            UList.CheckAutoHyperMode
            UList.CheckAutoNormalMode
            
            'Cancelコマンドが実行されたらここで終了
            If IsCanceled Then
                If SelectedUnit Is Nothing Then
                    Exit Do
                End If
                If SelectedUnit.Status <> "出撃" Then
                    Exit Do
                End If
                IsCanceled = False
            End If
            
            '行動数を減少
            SelectedUnit.UseAction
            
            '接触イベント
            With SelectedUnit
                If .Status = "出撃" And .X > 1 Then
                    If Not MapDataForUnit(.X - 1, .Y) Is Nothing Then
                        Set SelectedTarget = MapDataForUnit(.X - 1, .Y)
                        HandleEvent "接触", .MainPilot.ID, _
                            MapDataForUnit(.X - 1, .Y).MainPilot.ID
                        If IsScenarioFinished Then
                            Exit Sub
                        End If
                    End If
                End If
            End With
            With SelectedUnit
                If .Status = "出撃" And .X < MapWidth Then
                    If Not MapDataForUnit(.X + 1, .Y) Is Nothing Then
                        Set SelectedTarget = MapDataForUnit(.X + 1, .Y)
                        HandleEvent "接触", .MainPilot.ID, _
                            MapDataForUnit(.X + 1, .Y).MainPilot.ID
                        If IsScenarioFinished Then
                            Exit Sub
                        End If
                    End If
                End If
            End With
            With SelectedUnit
                If .Status = "出撃" And .Y > 1 Then
                    If Not MapDataForUnit(.X, .Y - 1) Is Nothing Then
                        Set SelectedTarget = MapDataForUnit(.X, .Y - 1)
                        HandleEvent "接触", .MainPilot.ID, _
                            MapDataForUnit(.X, .Y - 1).MainPilot.ID
                        If IsScenarioFinished Then
                            Exit Sub
                        End If
                    End If
                End If
            End With
            With SelectedUnit
                If .Status = "出撃" And .Y < MapHeight Then
                    If Not MapDataForUnit(.X, .Y + 1) Is Nothing Then
                        Set SelectedTarget = MapDataForUnit(.X, .Y + 1)
                        HandleEvent "接触", .MainPilot.ID, _
                            MapDataForUnit(.X, .Y + 1).MainPilot.ID
                        If IsScenarioFinished Then
                            Exit Sub
                        End If
                    End If
                End If
            End With
            
            '進入イベント
            With SelectedUnit
                If .Status = "出撃" Then
                    HandleEvent "進入", .MainPilot.ID, .X, .Y
                    If IsScenarioFinished Then
                        Exit Sub
                    End If
                End If
            End With
            
            '行動終了イベント
            With SelectedUnit
                If .Status = "出撃" Then
                    HandleEvent "行動終了", .MainPilot.ID
                    If IsScenarioFinished Then
                        Exit Sub
                    End If
                End If
            End With
        Loop
NextLoop:
    Next
    Next
    
    'ステータスウィンドウの表示を消去
    ClearUnitStatus
End Sub

'ゲームオーバー
Public Sub GameOver()
Dim fname As String

    KeepBGM = False
    BossBGM = False
    StopBGM
    MainForm.Hide
    
    'GameOver.eveを探す
    If FileExists(ScenarioPath & "Data\System\GameOver.eve") Then
        fname = ScenarioPath & "Data\System\GameOver.eve"
        If FileExists(ScenarioPath & "Data\System\non_pilot.txt") Then
            NPDList.Load ScenarioPath & "Data\System\non_pilot.txt"
        End If
    ElseIf FileExists(AppPath & "Data\System\GameOver.eve") Then
        fname = AppPath & "Data\System\GameOver.eve"
        If FileExists(AppPath & "Data\System\non_pilot.txt") Then
            NPDList.Load AppPath & "Data\System\non_pilot.txt"
        End If
    Else
        'GameOver.eveが無ければそのまま終了
        TerminateSRC
    End If
    
    'GameOver.eveを読み込み
    ClearEventData
    LoadEventData fname
    ScenarioFileName = fname
    
    If Not IsEventDefined("プロローグ") Then
        ErrorMessage fname & "中にプロローグイベントが定義されていません"
        TerminateSRC
    End If
    
    'GameOver.eveのプロローグイベントを実施
    HandleEvent "プロローグ"
End Sub

'ゲームクリア
Public Sub GameClear()
    TerminateSRC
End Sub

'ゲームを途中終了
Public Sub ExitGame()
Dim fname As String

    KeepBGM = False
    BossBGM = False
    StopBGM
    
    'Exit.eveを探す
    MainForm.Hide
    If FileExists(ScenarioPath & "Data\System\Exit.eve") Then
        fname = ScenarioPath & "Data\System\Exit.eve"
        If FileExists(ScenarioPath & "Data\System\non_pilot.txt") Then
            NPDList.Load ScenarioPath & "Data\System\non_pilot.txt"
        End If
    ElseIf FileExists(AppPath & "Data\System\Exit.eve") Then
        fname = AppPath & "Data\System\Exit.eve"
        If FileExists(AppPath & "Data\System\non_pilot.txt") Then
            NPDList.Load AppPath & "Data\System\non_pilot.txt"
        End If
    Else
        'Exit.eveが無ければそのまま終了
        TerminateSRC
    End If
    
    'Exit.eveを読み込み
    ClearEventData
    LoadEventData fname
    
    If Not IsEventDefined("プロローグ") Then
        ErrorMessage fname & "中にプロローグイベントが定義されていません"
        TerminateSRC
    End If
    
    'Exit.eveのプロローグイベントを実施
    HandleEvent "プロローグ"
    
    'SRCを終了
    TerminateSRC
End Sub

'SRCを終了
Public Sub TerminateSRC()
Dim i As Integer, j As Integer

    'ウィンドウを閉じる
    If Not MainForm Is Nothing Then
        MainForm.Hide
    End If
    Load frmMessage
    If frmMessage.Visible Then
        CloseMessageForm
    End If
    Load frmListBox
    If frmListBox.Visible Then
        frmListBox.Hide
    End If
    Load frmNowLoading
    If frmNowLoading.Visible Then
        frmNowLoading.Hide
    End If
    DoEvents
    
    '時間解像度を元に戻す
    Call timeEndPeriod(1)
    
    'フルスクリーンモードを使っていた場合は解像度を元に戻す
    If ReadIni("Option", "FullScreen") = "On" Then
        ChangeDisplaySize 0, 0
    End If
    
    'ＢＧＭ・効果音の再生を停止
    FreeSoundModule
    
    '各種データを解放
    
    Set SelectedUnit = Nothing
    Set SelectedTarget = Nothing
    Set SelectedPilot = Nothing
    Set DisplayedUnit = Nothing
    
    For i = 1 To MapWidth
        For j = 1 To MapHeight
            Set MapDataForUnit(i, j) = Nothing
        Next
    Next
    
    With GlobalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set GlobalVariableList = Nothing
    
    With LocalVariableList
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set LocalVariableList = Nothing
    
    Set SelectedUnitForEvent = Nothing
    Set SelectedTargetForEvent = Nothing
    
    Set UList = Nothing
    Set PList = Nothing
    Set IList = Nothing
    
    'なぜかこれがないと不正終了する……
    DoEvents
    
    Set PDList = Nothing
    Set NPDList = Nothing
    Set UDList = Nothing
    Set IDList = Nothing
    Set MDList = Nothing
    Set EDList = Nothing
    Set ADList = Nothing
    Set EADList = Nothing
    Set DDList = Nothing
    Set SPDList = Nothing
    Set ALDList = Nothing
    Set TDList = Nothing
    Set BCList = Nothing
    
    End
End Sub


'データをセーブ
Public Sub SaveData(fname As String)
Dim i As Integer, num As Long

    On Error GoTo ErrorHandler
    
    SaveDataFileNumber = FreeFile
    Open fname For Output Access Write As #SaveDataFileNumber
    
    With App
        num = 10000 * .Major + 100 * .Minor + .Revision
    End With
    Write #SaveDataFileNumber, num
    
    Write #SaveDataFileNumber, UBound(Titles)
    For i = 1 To UBound(Titles)
        Write #SaveDataFileNumber, Titles(i)
    Next
    
    Write #SaveDataFileNumber, GetValueAsString("次ステージ")
    
    Write #SaveDataFileNumber, TotalTurn
    Write #SaveDataFileNumber, Money
    Write #SaveDataFileNumber, 0 'パーツ用のダミー
    
    SaveGlobalVariables
    PList.Save
    UList.Save
    IList.Save
    
    Close #SaveDataFileNumber
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "セーブ中にエラーが発生しました"
    Close #SaveDataFileNumber
End Sub

'データをロード
Public Sub LoadData(fname As String)
Dim i As Integer, num As Integer, fname2 As String
Dim dummy As String
Dim u As Unit

    On Error GoTo ErrorHandler
    
    SaveDataFileNumber = FreeFile
    Open fname For Input As #SaveDataFileNumber
    
    Input #SaveDataFileNumber, SaveDataVersion
    
    If SaveDataVersion > 10000 Then
        Input #SaveDataFileNumber, num
    Else
        num = SaveDataVersion
    End If
    
    SetLoadImageSize num * 2 + 5
    
    ReDim Titles(num)
    For i = 1 To num
        Input #SaveDataFileNumber, Titles(i)
        IncludeData Titles(i)
    Next
    
    If FileExists(ScenarioPath & "Data\alias.txt") Then
        ALDList.Load ScenarioPath & "Data\alias.txt"
    End If
    If FileExists(ScenarioPath & "Data\sp.txt") Then
        SPDList.Load ScenarioPath & "Data\sp.txt"
    ElseIf FileExists(ScenarioPath & "Data\mind.txt") Then
        SPDList.Load ScenarioPath & "Data\mind.txt"
    End If
    If FileExists(ScenarioPath & "Data\pilot.txt") Then
        PDList.Load ScenarioPath & "Data\pilot.txt"
    End If
    If FileExists(ScenarioPath & "Data\non_pilot.txt") Then
        NPDList.Load ScenarioPath & "Data\non_pilot.txt"
    End If
    If FileExists(ScenarioPath & "Data\robot.txt") Then
        UDList.Load ScenarioPath & "Data\robot.txt"
    End If
    If FileExists(ScenarioPath & "Data\unit.txt") Then
        UDList.Load ScenarioPath & "Data\unit.txt"
    End If
    
    DisplayLoadingProgress
    
    If FileExists(ScenarioPath & "Data\pilot_message.txt") Then
        MDList.Load ScenarioPath & "Data\pilot_message.txt"
    End If
    If FileExists(ScenarioPath & "Data\pilot_dialog.txt") Then
        DDList.Load ScenarioPath & "Data\pilot_dialog.txt"
    End If
    If FileExists(ScenarioPath & "Data\effect.txt") Then
        EDList.Load ScenarioPath & "Data\effect.txt"
    End If
    If FileExists(ScenarioPath & "Data\animation.txt") Then
        ADList.Load ScenarioPath & "Data\animation.txt"
    End If
    If FileExists(ScenarioPath & "Data\ext_animation.txt") Then
        EADList.Load ScenarioPath & "Data\ext_animation.txt"
    End If
    If FileExists(ScenarioPath & "Data\item.txt") Then
        IDList.Load ScenarioPath & "Data\item.txt"
    End If
    
    DisplayLoadingProgress
    
    IsLocalDataLoaded = True
        
    Input #SaveDataFileNumber, fname2
    Input #SaveDataFileNumber, TotalTurn
    Input #SaveDataFileNumber, Money
    Input #SaveDataFileNumber, num 'パーツ用のダミー
    
    LoadGlobalVariables
    If Not IsGlobalVariableDefined("次ステージ") Then
        DefineGlobalVariable "次ステージ"
    End If
    SetVariableAsString "次ステージ", fname2
    
    PList.Load
    UList.Load
    IList.Load
    
    Close #SaveDataFileNumber
    
    'リンクデータを処理するため、セーブファイルを一旦閉じてから再度読み込み
    
    SaveDataFileNumber = FreeFile
    Open fname For Input As #SaveDataFileNumber
    
    If SaveDataVersion > 10000 Then
        Input #SaveDataFileNumber, dummy
    End If
    Input #SaveDataFileNumber, num
    ReDim Titles(num)
    For i = 1 To num
        Input #SaveDataFileNumber, Titles(i)
    Next
    Input #SaveDataFileNumber, dummy
    Input #SaveDataFileNumber, TotalTurn
    Input #SaveDataFileNumber, Money
    Input #SaveDataFileNumber, num 'パーツ用のダミー
    Input #SaveDataFileNumber, num 'パーツ用のダミー
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    PList.LoadLinkInfo
    UList.LoadLinkInfo
    IList.LoadLinkInfo
    
    Close #SaveDataFileNumber
    
    DisplayLoadingProgress
    
    'ユニットの状態を回復
    For Each u In UList
        u.Reset
    Next
    
    DisplayLoadingProgress
    
    '追加されたシステム側イベントデータの読み込み
    LoadEventData ""
    
    DisplayLoadingProgress
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "ロード中にエラーが発生しました"
    Close #SaveDataFileNumber
    TerminateSRC
End Sub


'一時中断用データをファイルにセーブする
Public Sub DumpData(fname As String)
Dim i As Integer, num As Long

    On Error GoTo ErrorHandler
    
    '中断データをセーブ
    SaveDataFileNumber = FreeFile
    Open fname For Output Access Write As #SaveDataFileNumber
    
    With App
        num = 10000 * .Major + 100 * .Minor + .Revision
    End With
    Write #SaveDataFileNumber, num
    
    Write #SaveDataFileNumber, Mid$(ScenarioFileName, Len(ScenarioPath) + 1)
    
    Write #SaveDataFileNumber, UBound(Titles)
    For i = 1 To UBound(Titles)
        Write #SaveDataFileNumber, Titles(i)
    Next
    
    Write #SaveDataFileNumber, Turn
    Write #SaveDataFileNumber, TotalTurn
    Write #SaveDataFileNumber, Money
    
    DumpEventData
    
    PList.Dump
    IList.Dump
    UList.Dump
    
    DumpMapData
    
    ' Midi じゃなくて midi じゃないと検索失敗するようになってるので。
    If InStr(LCase$(BGMFileName), "\midi\") > 0 Then
        Write #SaveDataFileNumber, Mid$(BGMFileName, InStr(LCase$(BGMFileName), "\midi\") + 6)
    ElseIf InStr(BGMFileName, "\") > 0 Then
        Write #SaveDataFileNumber, Mid$(BGMFileName, InStr(BGMFileName, "\") + 1)
    Else
        Write #SaveDataFileNumber, BGMFileName
    End If
    Write #SaveDataFileNumber, RepeatMode
    Write #SaveDataFileNumber, KeepBGM
    Write #SaveDataFileNumber, BossBGM
    
    Write #SaveDataFileNumber, RndSeed
    Write #SaveDataFileNumber, RndIndex
    
    Close #SaveDataFileNumber
    
    LastSaveDataFileName = fname
    If InStr(fname, "\_リスタート.src") > 0 Then
        IsRestartSaveDataAvailable = True
    ElseIf InStr(fname, "\_クイックセーブ.src") > 0 Then
        IsQuickSaveDataAvailable = True
    End If
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "セーブ中にエラーが発生しました"
    Close #SaveDataFileNumber
End Sub

'一時中断用データをロード
Public Sub RestoreData(fname As String, quick_load As Boolean)
Dim i As Integer, num As Integer, fname2 As String
Dim dummy As String
Dim u As Unit
Dim scenario_file_is_different As Boolean

    On Error GoTo ErrorHandler
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = 11
    
    If quick_load Then
        If IsOptionDefined("デバッグ") Then
            LoadEventData ScenarioFileName, "クイックロード"
        End If
    End If
    
    If Not quick_load Then
        OpenNowLoadingForm
    End If
    
    SaveDataFileNumber = FreeFile
    Open fname For Input As #SaveDataFileNumber
    
    Input #SaveDataFileNumber, fname2
    
    If IsNumeric(fname2) Then
        SaveDataVersion = CLng(fname2)
        Input #SaveDataFileNumber, fname2
    Else
        SaveDataVersion = 1
    End If
    
    'ウィンドウのタイトルを設定
    If ScenarioFileName <> ScenarioPath & fname2 Then
        MainForm.Caption = "SRC - " & Left$(fname2, Len(fname2) - 4)
        ScenarioFileName = ScenarioPath & fname2
        scenario_file_is_different = True
    End If
    
    Input #SaveDataFileNumber, num
    
    '使用するデータをロード
    If Not quick_load Then
        SetLoadImageSize num * 2 + 5
        
        ReDim Titles(num)
        For i = 1 To num
            Input #SaveDataFileNumber, Titles(i)
            IncludeData Titles(i)
        Next
        
        If FileExists(ScenarioPath & "Data\alias.txt") Then
            ALDList.Load ScenarioPath & "Data\alias.txt"
        End If
        If FileExists(ScenarioPath & "Data\sp.txt") Then
            SPDList.Load ScenarioPath & "Data\sp.txt"
        ElseIf FileExists(ScenarioPath & "Data\mind.txt") Then
            SPDList.Load ScenarioPath & "Data\mind.txt"
        End If
        If FileExists(ScenarioPath & "Data\pilot.txt") Then
            PDList.Load ScenarioPath & "Data\pilot.txt"
        End If
        If FileExists(ScenarioPath & "Data\non_pilot.txt") Then
            NPDList.Load ScenarioPath & "Data\non_pilot.txt"
        End If
        If FileExists(ScenarioPath & "Data\robot.txt") Then
            UDList.Load ScenarioPath & "Data\robot.txt"
        End If
        If FileExists(ScenarioPath & "Data\unit.txt") Then
            UDList.Load ScenarioPath & "Data\unit.txt"
        End If
        
        DisplayLoadingProgress
        
        If FileExists(ScenarioPath & "Data\pilot_message.txt") Then
            MDList.Load ScenarioPath & "Data\pilot_message.txt"
        End If
        If FileExists(ScenarioPath & "Data\pilot_dialog.txt") Then
            DDList.Load ScenarioPath & "Data\pilot_dialog.txt"
        End If
        If FileExists(ScenarioPath & "Data\effect.txt") Then
            EDList.Load ScenarioPath & "Data\effect.txt"
        End If
        If FileExists(ScenarioPath & "Data\animation.txt") Then
            ADList.Load ScenarioPath & "Data\animation.txt"
        End If
        If FileExists(ScenarioPath & "Data\ext_animation.txt") Then
            EADList.Load ScenarioPath & "Data\ext_animation.txt"
        End If
        If FileExists(ScenarioPath & "Data\item.txt") Then
            IDList.Load ScenarioPath & "Data\item.txt"
        End If
        
        DisplayLoadingProgress
        IsLocalDataLoaded = True
        
        LoadEventData ScenarioFileName, "リストア"
        
        DisplayLoadingProgress
    Else
        For i = 1 To num
            Line Input #SaveDataFileNumber, dummy
        Next
        
        If scenario_file_is_different Then
            LoadEventData ScenarioFileName, "リストア"
        End If
    End If
    
    Input #SaveDataFileNumber, Turn
    Input #SaveDataFileNumber, TotalTurn
    Input #SaveDataFileNumber, Money
    
    RestoreEventData
    
    PList.Restore
    IList.Restore
    UList.Restore

'MOD START 240a
'    RestoreMapData
'    'ＢＧＭ関連の設定を復元
'    Input #SaveDataFileNumber, fname2
    'マップデータの互換性維持のため、RestoreMapDataでＢＧＭ関連の１行目まで読み込んで戻り値にした
    fname2 = RestoreMapData
'MOD  END  240a
    fname2 = SearchMidiFile("(" & fname2 & ")")
    If fname2 <> "" Then
        KeepBGM = False
        BossBGM = False
        ChangeBGM fname2
        Input #SaveDataFileNumber, RepeatMode
        Input #SaveDataFileNumber, KeepBGM
        Input #SaveDataFileNumber, BossBGM
    Else
        StopBGM
        Line Input #SaveDataFileNumber, dummy
        Line Input #SaveDataFileNumber, dummy
        Line Input #SaveDataFileNumber, dummy
    End If
    
    '乱数系列を復元
    If Not IsOptionDefined("デバッグ") _
        And Not IsOptionDefined("乱数系列非保存") _
        And Not EOF(SaveDataFileNumber) _
    Then
        Input #SaveDataFileNumber, RndSeed
        RndReset
        Input #SaveDataFileNumber, RndIndex
    End If

    If Not quick_load Then
        DisplayLoadingProgress
    End If
    
    Close #SaveDataFileNumber
    
    'リンクデータを処理するため、セーブファイルを一旦閉じてから再度読み込み
    
    SaveDataFileNumber = FreeFile
    Open fname For Input As #SaveDataFileNumber
    
    'SaveDataVersion
    If SaveDataVersion > 10000 Then
        Line Input #SaveDataFileNumber, dummy
    End If
    
    'ScenarioFileName
    Line Input #SaveDataFileNumber, dummy
    
    '使用するデータ名
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    
    'Turn
    Line Input #SaveDataFileNumber, dummy
    'TotalTurn
    Line Input #SaveDataFileNumber, dummy
    'Money
    Line Input #SaveDataFileNumber, dummy
    
    SkipEventData
    
    PList.RestoreLinkInfo
    IList.RestoreLinkInfo
    UList.RestoreLinkInfo
    
    Close #SaveDataFileNumber
    
    'パラメータ情報を処理するため、セーブファイルを一旦閉じてから再度読み込み。
    '霊力やＨＰ、ＥＮといったパラメータは最大値が特殊能力で変動するため、
    '特殊能力の設定が終わってから改めて設定してやる必要がある。
    
    SaveDataFileNumber = FreeFile
    Open fname For Input As #SaveDataFileNumber
    
    'SaveDataVersion
    If SaveDataVersion > 10000 Then
        Line Input #SaveDataFileNumber, dummy
    End If
    
    'ScenarioFileName
    Line Input #SaveDataFileNumber, dummy
    
    '使用するデータ名
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Line Input #SaveDataFileNumber, dummy
    Next
    
    'Turn
    Line Input #SaveDataFileNumber, dummy
    'TotalTurn
    Line Input #SaveDataFileNumber, dummy
    'Money
    Line Input #SaveDataFileNumber, dummy
    
    SkipEventData
    
    PList.RestoreParameter
    IList.RestoreParameter
    UList.RestoreParameter
    
    PList.UpdateSupportMod
    
    '背景書き換え
    If IsMapDirty Then
        Dim map_x As Integer, map_y As Integer
        
        map_x = MapX
        map_y = MapY
        
        SetupBackground MapDrawMode, "非同期"
        
        MapX = map_x
        MapY = map_y
        
        '再開イベントによるマップ画像の書き換え処理を行う
        HandleEvent "再開"
        
        IsMapDirty = False
    End If
    
    Set SelectedUnit = Nothing
    Set SelectedTarget = Nothing
    
    'ユニット画像生成
    For Each u In UList
        With u
            If .BitmapID = 0 Then
                .BitmapID = MakeUnitBitmap(u)
            End If
        End With
    Next
    
    '画面更新
    Center MapX, MapY
    
    Close #SaveDataFileNumber
    
    If Not quick_load Then
        DisplayLoadingProgress
    End If
    
    If Not quick_load Then
        CloseNowLoadingForm
    End If
    
    If Not quick_load Then
        MainForm.Show
    End If
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ClearUnitStatus
    If Not MainForm.Visible Then
        MainForm.Show
        MainForm.Refresh
    End If
    RedrawScreen
    
    If Turn = 0 Then
        HandleEvent "スタート"
        
' MOD START MARGE
'        StartTurn "味方"
        'スタートイベントから次のステージが開始された場合、StartTurnが上のHandleEventで
        '実行されてしまう。
        '味方ターンの処理が２重起動されるのを防ぐため、Turnをチェックしてから起動する
        If Turn = 0 Then
            StartTurn "味方"
        End If
' MOD END MARGE
    Else
        CommandState = "ユニット選択"
        Stage = "味方"
    End If
    
    LastSaveDataFileName = fname
    If InStr(fname, "\_リスタート.src") > 0 Then
        IsRestartSaveDataAvailable = True
    ElseIf InStr(fname, "\_クイックセーブ.src") > 0 Then
        IsQuickSaveDataAvailable = True
    End If
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "ロード中にエラーが発生しました"
    Close #SaveDataFileNumber
    TerminateSRC
End Sub


'旧形式のユニットＩＤを新形式に変換
'旧形式）ユニット名称+数値
'新形式）ユニット名称+":"+数値
Public Sub ConvertUnitID(ID As String)
Dim i As Integer
    
    If InStr(ID, ":") > 0 Then
        Exit Sub
    End If
    
    '数値部分を読み飛ばす
    i = Len(ID)
    Do While i > 0
        Select Case Asc(Mid$(ID, i, 1))
            Case 48 To 57
                '0-9
            Case Else
                Exit Do
        End Select
        i = i - 1
    Loop
    
    'ユニット名称と数値部分の間に「:」を挿入
    ID = Left$(ID, i) & ":" & Mid$(ID, i + 1)
End Sub

'作品new_titleのデータを読み込み
Public Sub IncludeData(new_title As String)
Dim fpath As String
    
    'ロードのインジケータ表示を行う
    If frmNowLoading.Visible Then
        DisplayLoadingProgress
    End If
    
    'Dataフォルダの場所を探す
    fpath = SearchDataFolder(new_title)
    
    If Len(fpath) = 0 Then
        ErrorMessage "データ「" & new_title & "」のフォルダが見つかりません"
        TerminateSRC
    End If
    
    On Error GoTo ErrorHandler
    
    If FileExists(fpath & "\alias.txt") Then
        ALDList.Load fpath & "\alias.txt"
    End If
    If FileExists(fpath & "\sp.txt") Then
        SPDList.Load fpath & "\sp.txt"
    ElseIf FileExists(fpath & "\mind.txt") Then
        SPDList.Load fpath & "\mind.txt"
    End If
    If FileExists(fpath & "\pilot.txt") Then
        PDList.Load fpath & "\pilot.txt"
    End If
    If FileExists(fpath & "\non_pilot.txt") Then
        NPDList.Load fpath & "\non_pilot.txt"
    End If
    If FileExists(fpath & "\robot.txt") Then
        UDList.Load fpath & "\robot.txt"
    End If
    If FileExists(fpath & "\unit.txt") Then
        UDList.Load fpath & "\unit.txt"
    End If
    
    'ロードのインジケータ表示を行う
    If frmNowLoading.Visible Then
        DisplayLoadingProgress
    End If
    
    If FileExists(fpath & "\pilot_message.txt") Then
        MDList.Load fpath & "\pilot_message.txt"
    End If
    If FileExists(fpath & "\pilot_dialog.txt") Then
        DDList.Load fpath & "\pilot_dialog.txt"
    End If
    If FileExists(fpath & "\effect.txt") Then
        EDList.Load fpath & "\effect.txt"
    End If
    If FileExists(fpath & "\animation.txt") Then
        ADList.Load fpath & "\animation.txt"
    End If
    If FileExists(fpath & "\ext_animation.txt") Then
        EADList.Load fpath & "\ext_animation.txt"
    End If
    If FileExists(fpath & "\item.txt") Then
        IDList.Load fpath & "\item.txt"
    End If
    
    Exit Sub
    
ErrorHandler:
    ErrorMessage "Src.ini内のExtDataPathの値が不正です"
    TerminateSRC
End Sub

'データフォルダ fname を検索
Public Function SearchDataFolder(fname As String) As String
Dim fname2 As String
Static init_search_data_folder As Boolean
Static scenario_data_dir_exists As Boolean
Static extdata_data_dir_exists As Boolean
Static extdata2_data_dir_exists As Boolean
Static src_data_dir_exists As Boolean

    '初めて実行する際に、各フォルダにDataフォルダがあるかチェック
    If Not init_search_data_folder Then
        If Len(Dir$(ScenarioPath & "Data", vbDirectory)) > 0 Then
            scenario_data_dir_exists = True
        End If
        If Len(ExtDataPath) > 0 And ScenarioPath <> ExtDataPath Then
            If Len(Dir$(ExtDataPath & "Data", vbDirectory)) > 0 Then
                extdata_data_dir_exists = True
            End If
        End If
        If Len(ExtDataPath2) > 0 And ScenarioPath <> ExtDataPath2 Then
            If Len(Dir$(ExtDataPath2 & "Data", vbDirectory)) > 0 Then
                extdata2_data_dir_exists = True
            End If
        End If
        If ScenarioPath <> AppPath Then
            If Len(Dir$(AppPath & "Data", vbDirectory)) > 0 Then
                src_data_dir_exists = True
            End If
        End If
        init_search_data_folder = True
    End If
    
    'フォルダを検索
    fname2 = "Data\" & fname
    If scenario_data_dir_exists Then
        SearchDataFolder = ScenarioPath & fname2
        If Len(Dir$(SearchDataFolder, vbDirectory)) > 0 Then
            Exit Function
        End If
    End If
    If extdata_data_dir_exists Then
        SearchDataFolder = ExtDataPath & fname2
        If Len(Dir$(SearchDataFolder, vbDirectory)) > 0 Then
            Exit Function
        End If
    End If
    If extdata2_data_dir_exists Then
        SearchDataFolder = ExtDataPath2 & fname2
        If Len(Dir$(SearchDataFolder, vbDirectory)) > 0 Then
            Exit Function
        End If
    End If
    If src_data_dir_exists Then
        SearchDataFolder = AppPath & fname2
        If Len(Dir$(SearchDataFolder, vbDirectory)) > 0 Then
            Exit Function
        End If
    End If
    
    'フォルダが見つからなかった
    SearchDataFolder = ""
End Function

'資金の量を変更する
Public Sub IncrMoney(ByVal earnings As Long)
    Money = MinLng(Money + earnings, 999999999)
    Money = MaxLng(Money, 0)
End Sub
