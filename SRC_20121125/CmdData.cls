VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "CmdData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'イベントコマンドのクラス

'コマンドの種類
Private CmdName As CmdType
'引数の数
Public ArgNum As Integer
'コマンドのEventDataにおける位置
Public LineNum As Long

'引数の値
Private lngArgs() As Long
Private dblArgs() As Double
Private strArgs() As String

'引数の型
Private ArgsType() As ValueType


'コマンドの種類
Public Property Get Name() As CmdType
    If CmdName = NullCmd Then
        Parse EventData(LineNum)
    End If
    Name = CmdName
End Property

Public Property Let Name(new_name As CmdType)
    CmdName = new_name
End Property

'イベントデータ行を読み込んで解析する
Public Function Parse(edata As String) As Boolean
Dim buf As String, expr As String, list() As String
Dim i As Integer
    
    '正常に解析が終了した場合はTrueを返すこと
    Parse = True
    
    On Error GoTo ErrorHandler
    
    '空行は無視
    If Len(edata) = 0 Then
        CmdName = NopCmd
        ArgNum = 0
        Exit Function
    End If
    
    'ラベルは無視
    If Right$(edata, 1) = ":" Then
        CmdName = NopCmd
        ArgNum = 0
        Exit Function
    End If
    
    'コマンドのパラメータ分割
    ArgNum = ListSplit(edata, list)
    
    '空行は無視
    If ArgNum = 0 Then
        CmdName = NopCmd
        Exit Function
    End If
    
    'パラメータの処理
    If ArgNum > 1 Then
        ReDim strArgs(2 To ArgNum)
        ReDim lngArgs(2 To ArgNum)
        ReDim dblArgs(2 To ArgNum)
        ReDim ArgsType(2 To ArgNum)
        For i = 2 To ArgNum
            buf = list(i)
            strArgs(i) = buf
            ArgsType(i) = UndefinedType
            
            '先頭の一文字からパラメータの属性を判定
            Select Case Asc(buf)
                Case 0 '空文字列
                     ArgsType(i) = StringType
                Case 34 '"
                    If Right$(buf, 1) = """" Then
                        If InStr(buf, "$(") = 0 Then
                            ArgsType(i) = StringType
                            strArgs(i) = Mid$(buf, 2, Len(buf) - 2)
                        End If
                    Else
                        ArgsType(i) = StringType
                    End If
                Case 40 '(
                    '式
                Case 45 '-
                    If IsNumeric(buf) Then
                        lngArgs(i) = StrToLng(buf)
                        dblArgs(i) = CDbl(buf)
                        ArgsType(i) = NumericType
                    Else
                        ArgsType(i) = StringType
                    End If
                Case 48 To 57 '0～9
                    If IsNumeric(buf) Then
                        lngArgs(i) = StrToLng(buf)
                        dblArgs(i) = CDbl(buf)
                        ArgsType(i) = NumericType
                    Else
                        ArgsType(i) = StringType
                    End If
                Case 96 '`
                    If Right$(buf, 1) = "`" Then
                        strArgs(i) = Mid$(buf, 2, Len(buf) - 2)
                    End If
                    ArgsType(i) = StringType
            End Select
        Next
    End If
    
    'コマンドの種類を判定
    Select Case LCase$(list(1))
        Case "arc"
            CmdName = ArcCmd
        Case "array"
            CmdName = ArrayCmd
        Case "ask"
            CmdName = AskCmd
        Case "attack"
            CmdName = AttackCmd
        Case "autotalk"
            CmdName = AutoTalkCmd
        Case "bossrank"
            CmdName = BossRankCmd
        Case "break"
            CmdName = BreakCmd
        Case "call"
            CmdName = CallCmd
        Case "return"
            CmdName = ReturnCmd
        Case "callintermissioncommand"
            CmdName = CallInterMissionCommandCmd
        Case "cancel"
            CmdName = CancelCmd
        Case "center"
            CmdName = CenterCmd
        Case "changearea"
            CmdName = ChangeAreaCmd
'ADD START 240a
        Case "changelayer"
            CmdName = ChangeLayerCmd
'ADD  END  240a
        Case "changemap"
            CmdName = ChangeMapCmd
        Case "changemode"
            CmdName = ChangeModeCmd
        Case "changeparty"
            CmdName = ChangePartyCmd
        Case "changeterrain"
            CmdName = ChangeTerrainCmd
        Case "changeunitbitmap"
            CmdName = ChangeUnitBitmapCmd
        Case "charge"
            CmdName = ChargeCmd
        Case "circle"
            CmdName = CircleCmd
        Case "clearevent"
            CmdName = ClearEventCmd
        Case "clearimage"
            CmdName = ClearImageCmd
'ADD START 240a
        Case "clearlayer"
            CmdName = ClearLayerCmd
'ADD  END  240a
        Case "clearobj"
            CmdName = ClearObjCmd
        Case "clearpicture"
            CmdName = ClearPictureCmd
        Case "clearskill", "clearability"
            CmdName = ClearSkillCmd
        Case "clearspecialpower", "clearmind"
            CmdName = ClearSpecialPowerCmd
        Case "clearstatus"
            CmdName = ClearStatusCmd
        Case "cls"
            CmdName = ClsCmd
        Case "close"
            CmdName = CloseCmd
        Case "color"
            CmdName = ColorCmd
        Case "colorfilter"
            CmdName = ColorFilterCmd
        Case "combine"
            CmdName = CombineCmd
        Case "confirm"
            CmdName = ConfirmCmd
        Case "continue"
            CmdName = ContinueCmd
        Case "copyarray"
            CmdName = CopyArrayCmd
        Case "copyfile"
            CmdName = CopyFileCmd
        Case "create"
            CmdName = CreateCmd
        Case "createfolder"
            CmdName = CreateFolderCmd
        Case "debug"
            CmdName = DebugCmd
        Case "destroy"
            CmdName = DestroyCmd
        Case "disable"
            CmdName = DisableCmd
        Case "do"
            CmdName = DoCmd
            If ArgNum = 3 Then
                strArgs(2) = LCase$(strArgs(2))
            End If
        Case "loop"
            CmdName = LoopCmd
            If ArgNum = 3 Then
                strArgs(2) = LCase$(strArgs(2))
            End If
        Case "drawoption"
            CmdName = DrawOptionCmd
        Case "drawwidth"
            CmdName = DrawWidthCmd
        Case "enable"
            CmdName = EnableCmd
        Case "equip"
            CmdName = EquipCmd
        Case "escape"
            CmdName = EscapeCmd
        Case "exchangeitem"
            CmdName = ExchangeItemCmd
        Case "exec"
            CmdName = ExecCmd
        Case "exit"
            CmdName = ExitCmd
        Case "explode"
            CmdName = ExplodeCmd
        Case "expup"
            CmdName = ExpUpCmd
        Case "fadein"
            CmdName = FadeInCmd
        Case "fadeout"
            CmdName = FadeOutCmd
        Case "fillcolor"
            CmdName = FillColorCmd
        Case "fillstyle"
            CmdName = FillStyleCmd
        Case "finish"
            CmdName = FinishCmd
        Case "fix"
            CmdName = FixCmd
        Case "for"
            CmdName = ForCmd
        Case "foreach"
            CmdName = ForEachCmd
        Case "next"
            CmdName = NextCmd
        Case "font"
            CmdName = FontCmd
        Case "forget"
            CmdName = ForgetCmd
        Case "gameclear"
            CmdName = GameClearCmd
        Case "gameover"
            CmdName = GameOverCmd
        Case "freememory"
            CmdName = FreeMemoryCmd
        Case "getoff"
            CmdName = GetOffCmd
        Case "global"
            CmdName = GlobalCmd
        Case "goto"
            CmdName = GotoCmd
        Case "hide"
            CmdName = HideCmd
        Case "hotpoint"
            CmdName = HotPointCmd
        Case "if"
            CmdName = IfCmd
        Case "else"
            CmdName = ElseCmd
        Case "elseif"
            CmdName = ElseIfCmd
        Case "endif"
            CmdName = EndIfCmd
        Case "incr"
            CmdName = IncrCmd
        Case "increasemorale"
            CmdName = IncreaseMoraleCmd
        Case "input"
            CmdName = InputCmd
        Case "intermissioncommand"
            CmdName = IntermissionCommandCmd
        Case "item"
            CmdName = ItemCmd
        Case "join"
            CmdName = JoinCmd
        Case "keepbgm"
            CmdName = KeepBGMCmd
        Case "land"
            CmdName = LandCmd
        Case "launch"
            CmdName = LaunchCmd
        Case "leave"
            CmdName = LeaveCmd
        Case "levelup"
            CmdName = LevelUpCmd
        Case "line"
            CmdName = LineCmd
        Case "lineread"
            CmdName = LineReadCmd
        Case "load"
            CmdName = LoadCmd
        Case "local"
            CmdName = LocalCmd
        Case "makepilotlist"
            CmdName = MakePilotListCmd
        Case "makeunitlist"
            CmdName = MakeUnitListCmd
        Case "mapability"
            CmdName = MapAbilityCmd
        Case "mapattack", "mapweapon"
            CmdName = MapAttackCmd
        Case "money"
            CmdName = MoneyCmd
        Case "monotone"
            CmdName = MonotoneCmd
        Case "move"
            CmdName = MoveCmd
        Case "night"
            CmdName = NightCmd
        Case "noon"
            CmdName = NoonCmd
        Case "open"
            CmdName = OpenCmd
        Case "option"
            CmdName = OptionCmd
        Case "organize"
            CmdName = OrganizeCmd
        Case "oval"
            CmdName = OvalCmd
        Case "paintpicture"
            CmdName = PaintPictureCmd
        Case "paintstring"
            CmdName = PaintStringCmd
        Case "paintsysstring"
            CmdName = PaintSysStringCmd
        Case "pilot"
            CmdName = PilotCmd
        Case "playmidi"
            CmdName = PlayMIDICmd
        Case "playsound"
            CmdName = PlaySoundCmd
        Case "polygon"
            CmdName = PolygonCmd
        Case "print"
            CmdName = PrintCmd
        Case "pset"
            CmdName = PSetCmd
        Case "question"
            CmdName = QuestionCmd
        Case "quickload"
            CmdName = QuickLoadCmd
        Case "quit"
            CmdName = QuitCmd
        Case "rankup"
            CmdName = RankUpCmd
        Case "read"
            CmdName = ReadCmd
        Case "recoveren"
            CmdName = RecoverENCmd
        Case "recoverhp"
            CmdName = RecoverHPCmd
        Case "recoverplana"
            CmdName = RecoverPlanaCmd
        Case "recoversp"
            CmdName = RecoverSPCmd
        Case "redraw"
            CmdName = RedrawCmd
        Case "refresh"
            CmdName = RefreshCmd
        Case "release"
            CmdName = ReleaseCmd
        Case "removefile"
            CmdName = RemoveFileCmd
        Case "removefolder"
            CmdName = RemoveFolderCmd
        Case "removeitem"
            CmdName = RemoveItemCmd
        Case "removepilot"
            CmdName = RemovePilotCmd
        Case "removeunit"
            CmdName = RemoveUnitCmd
        Case "renamebgm"
            CmdName = RenameBGMCmd
        Case "renamefile"
            CmdName = RenameFileCmd
        Case "renameterm"
            CmdName = RenameTermCmd
        Case "replacepilot"
            CmdName = ReplacePilotCmd
        Case "require"
            CmdName = RequireCmd
        Case "restoreevent"
            CmdName = RestoreEventCmd
        Case "ride"
            CmdName = RideCmd
        Case "select"
            CmdName = SelectCmd
        Case "savedata"
            CmdName = SaveDataCmd
        Case "selecttarget"
            CmdName = SelectTargetCmd
        Case "sepia"
            CmdName = SepiaCmd
        Case "set"
            CmdName = SetCmd
        Case "setbullet"
            CmdName = SetBulletCmd
        Case "setmessage"
            CmdName = SetMessageCmd
        Case "setrelation"
            CmdName = SetRelationCmd
        Case "setskill", "setability"
            CmdName = SetSkillCmd
        Case "setstatus"
            CmdName = SetStatusCmd
'ADD START 240a
        Case "setstatusstringcolor"
            CmdName = SetStatusStringColorCmd
'ADD  END
        Case "setstock"
            CmdName = SetStockCmd
'ADD START 240a
        Case "setwindowcolor"
            CmdName = SetWindowColorCmd
        Case "setwindowframewidth"
            CmdName = SetWindowFrameWidthCmd
'ADD  END
        Case "show"
            CmdName = ShowCmd
        Case "showimage"
            CmdName = ShowImageCmd
        Case "showunitstatus"
            CmdName = ShowUnitStatusCmd
        Case "skip"
            CmdName = SkipCmd
        Case "sort"
            CmdName = SortCmd
        Case "specialpower", "mind"
            CmdName = SpecialPowerCmd
        Case "split"
            CmdName = SplitCmd
        Case "startbgm"
            CmdName = StartBGMCmd
        Case "stopbgm"
            CmdName = StopBGMCmd
        Case "stopsummoning"
            CmdName = StopSummoningCmd
        Case "supply"
            CmdName = SupplyCmd
        Case "sunset"
            CmdName = SunsetCmd
        Case "swap"
            CmdName = SwapCmd
        Case "switch"
            CmdName = SwitchCmd
        Case "playflash"
            CmdName = PlayFlashCmd
        Case "clearflash"
            CmdName = ClearFlashCmd
        Case "case"
            CmdName = CaseCmd
            If ArgNum = 2 Then
                If LCase$(list(2)) = "else" Then
                    CmdName = CaseElseCmd
                End If
            End If
        Case "endsw"
            CmdName = EndSwCmd
        Case "talk"
            CmdName = TalkCmd
        Case "end"
            CmdName = EndCmd
        Case "suspend"
            CmdName = SuspendCmd
        Case "telop"
            CmdName = TelopCmd
        Case "transform"
            CmdName = TransformCmd
        Case "unit"
            CmdName = UnitCmd
        Case "unset"
            CmdName = UnsetCmd
        Case "upgrade"
            CmdName = UpgradeCmd
        Case "upvar"
            CmdName = UpVarCmd
        Case "useability"
            CmdName = UseAbilityCmd
        Case "wait"
            CmdName = WaitCmd
        Case "water"
            CmdName = WaterCmd
        Case "whitein"
            CmdName = WhiteInCmd
        Case "whiteout"
            CmdName = WhiteOutCmd
        Case "write"
            CmdName = WriteCmd
        Case Else
            '定義済みのイベントコマンドではない
            
            If ArgNum >= 3 Then
                If list(2) = "=" Then
                    '代入式
                    
                    CmdName = SetCmd
                    ReDim Preserve strArgs(2 To 3)
                    ReDim Preserve lngArgs(2 To 3)
                    ReDim Preserve dblArgs(2 To 3)
                    ReDim Preserve ArgsType(2 To 3)
                    
                    '代入先の変数名
                    strArgs(2) = list(1)
                    ArgsType(2) = StringType
                    
                    '代入する値
                    '(値が項の場合は既に引数の処理が済んでいるのでなにもしなくてよい)
                    If ArgNum > 3 Then
                        ArgsType(3) = UndefinedType
                        'GetValueAsStringの呼び出しの際に、Argsの内容は必ず項と仮定
                        'されているので、わざと項にしておく
                        strArgs(3) = "(" & ListTail(edata, 3) & ")"
                    End If
                    ArgNum = 3
                    Exit Function
                End If
            End If
            
            If ArgNum = -1 Then
                CmdName = NopCmd
                Exit Function
            End If
            
            'サブルーチンコール？
            CmdName = CallCmd
            ReDim Preserve strArgs(2 To ArgNum + 1)
            ReDim Preserve lngArgs(2 To ArgNum + 1)
            ReDim Preserve dblArgs(2 To ArgNum + 1)
            ReDim Preserve ArgsType(2 To ArgNum + 1)
            '引数を１個ずらす
            For i = 0 To ArgNum - 2
                strArgs(ArgNum + 1 - i) = strArgs(ArgNum - i)
                lngArgs(ArgNum + 1 - i) = lngArgs(ArgNum - i)
                dblArgs(ArgNum + 1 - i) = dblArgs(ArgNum - i)
                ArgsType(ArgNum + 1 - i) = ArgsType(ArgNum - i)
            Next
            ArgNum = ArgNum + 1
            '第２引数をサブルーチン名に設定
            strArgs(2) = list(1)
            If FindNormalLabel(list(1)) > 0 Then
                ArgsType(2) = StringType
            Else
                ArgsType(2) = UndefinedType
            End If
            Exit Function
    End Select
    
    If CmdName = IfCmd Or CmdName = ElseIfCmd Then
        'If文の処理の高速化のため、あらかじめ構文解析しておく
        If ArgNum = 1 Then
            '書式エラー
            DisplayEventErrorMessage CurrentLineNum, "Ifコマンドの書式に合っていません"
            Parse = False
            Exit Function
        End If
        
        expr = list(2)
        For i = 3 To ArgNum
            buf = list(i)
            Select Case LCase$(buf)
                Case "then", "exit"
                    ReDim strArgs(2 To 4)
                    ReDim lngArgs(2 To 4)
                    ReDim dblArgs(2 To 4)
                    ReDim ArgsType(2 To 4)
                    strArgs(2) = expr
                    lngArgs(3) = ArgNum - 2
                    ArgsType(3) = NumericType
                    strArgs(4) = LCase$(buf)
                    Exit For
                Case "goto"
                    buf = GetArg(i + 1)
                    ReDim strArgs(2 To 5)
                    ReDim lngArgs(2 To 5)
                    ReDim dblArgs(2 To 5)
                    ReDim ArgsType(2 To 5)
                    strArgs(2) = expr
                    lngArgs(3) = ArgNum - 3
                    ArgsType(3) = NumericType
                    strArgs(4) = "goto"
                    strArgs(5) = buf
                    Exit For
                Case ""
                    buf = """"""
            End Select
            expr = expr & " " & buf
        Next
        
        If i > ArgNum Then
            If CmdName = IfCmd Then
                DisplayEventErrorMessage LineNum, _
                    "Ifに対応する Then または Exit または Goto がありません"
            Else
                DisplayEventErrorMessage LineNum, _
                    "ElseIfに対応する Then または Exit または Goto がありません"
            End If
            TerminateSRC
        End If
        
        '条件式が式であることが確定していれば条件式の項数を0に
        Select Case lngArgs(3)
            Case 0
                If CmdName = IfCmd Then
                    DisplayEventErrorMessage LineNum, _
                        "Ifコマンドの条件式がありません"
                Else
                    DisplayEventErrorMessage LineNum, _
                        "ElseIfコマンドの条件式がありません"
                End If
                TerminateSRC
            Case 1
                Select Case Asc(expr)
                    Case 36 '$
                        lngArgs(3) = 0
                    Case 40 '(
                        '()を除去
                        strArgs(2) = Mid$(expr, 2, Len(expr) - 2)
                        lngArgs(3) = 0
                End Select
            Case 2
                If LCase$(LIndex(expr, 1)) = "not" Then
                    Select Case Asc(ListIndex(expr, 2))
                        Case 36, 40 '$, (
                            lngArgs(3) = 0
                    End Select
                Else
                    lngArgs(3) = 0
                End If
            Case Else
                lngArgs(3) = 0
        End Select
        
        Exit Function
    End If
    
    If CmdName = PaintStringCmd Then
        'PaintString文の処理の高速化のため、あらかじめ構文解析しておく
        
        '「;」を含む場合は改めて項に分解
        '(正しくリストの処理が行えないため)
        If Right$(buf, 1) = ";" Then
            buf = edata
            CmdName = PaintStringRCmd
            buf = Left$(buf, Len(buf) - 1)
            If Right$(buf, 1) = " " Then
                'メッセージが空文字列
                buf = buf & """"""
            End If
            ArgNum = ListSplit(buf, list)
        End If
        
        Select Case ArgNum
            Case 2
                '引数が１個の場合
                ArgNum = 2
                ReDim strArgs(2 To 2)
                ReDim lngArgs(2 To 2)
                ReDim dblArgs(2 To 2)
                ReDim ArgsType(2 To 2)
                
                buf = list(2)
                
                '表示文字列が式の場合にも対応
                If Left$(buf, 1) = """" And Right$(buf, 1) = """" Then
                    If InStr(buf, "$(") > 0 Then
                        strArgs(2) = buf
                    Else
                        strArgs(2) = Mid$(buf, 2, Len(buf) - 2)
                        ArgsType(2) = StringType
                    End If
                ElseIf Left$(buf, 1) = "`" And Right$(buf, 1) = "`" Then
                    strArgs(2) = Mid$(buf, 2, Len(buf) - 2)
                    ArgsType(2) = StringType
                ElseIf InStr(buf, "$(") > 0 Then
                    strArgs(2) = """" & buf & """"
                Else
                    strArgs(2) = buf
                End If
            Case 3
                '引数が２個の場合
                ArgNum = 2
                ReDim strArgs(2 To 2)
                ReDim lngArgs(2 To 2)
                ReDim dblArgs(2 To 2)
                ReDim ArgsType(2 To 2)
                
                '表示文字列は必ず文字列
                buf = ListTail(edata, 2)
                If InStr(buf, "$(") > 0 Then
                    strArgs(2) = """" & buf & """"
                Else
                    strArgs(2) = buf
                    ArgsType(2) = StringType
                End If
            Case 4
                '引数が３個の場合
                
                '座標指定があるかどうかが確定しているか？
                If (list(2) = "-" Or IsNumeric(list(2)) Or IsExpr(list(2))) _
                    And (list(3) = "-" Or IsNumeric(list(3)) Or IsExpr(list(3))) _
                Then
                    '座標指定があることが確定
                    ArgNum = 4
                    ReDim strArgs(2 To 4)
                    ReDim lngArgs(2 To 4)
                    ReDim dblArgs(2 To 4)
                    ReDim ArgsType(2 To 4)
                    
                    strArgs(2) = list(2)
                    strArgs(3) = list(3)
                    If Not IsExpr(list(2)) Then
                        ArgsType(2) = StringType
                    End If
                    If Not IsExpr(list(3)) Then
                        ArgsType(3) = StringType
                    End If
                Else
                    '実行時まで座標指定があるかどうか不明
                    ArgNum = 5
                    ReDim strArgs(2 To 5)
                    ReDim lngArgs(2 To 5)
                    ReDim dblArgs(2 To 5)
                    ReDim ArgsType(2 To 5)
                    
                    strArgs(2) = list(2)
                    strArgs(3) = list(3)
                    
                    '座標指定がなかった場合の表示文字列
                    buf = ListTail(edata, 2)
                    If InStr(buf, "$(") > 0 Then
                        strArgs(5) = """" & buf & """"
                    Else
                        strArgs(5) = buf
                        ArgsType(5) = StringType
                    End If
                End If
                
                '座標指定があった場合の表示文字列
                buf = list(4)
                If Left$(buf, 1) = """" And Right$(buf, 1) = """" Then
                    If InStr(buf, "$(") > 0 Then
                        strArgs(4) = buf
                    Else
                        strArgs(4) = Mid$(buf, 2, Len(buf) - 2)
                        ArgsType(4) = StringType
                    End If
                ElseIf Left$(buf, 1) = "`" And Right$(buf, 1) = "`" Then
                    strArgs(4) = Mid$(buf, 2, Len(buf) - 2)
                    ArgsType(4) = StringType
                ElseIf InStr(buf, "$(") > 0 Then
                    strArgs(4) = """" & buf & """"
                Else
                    strArgs(4) = buf
                End If
            Case Else
                '引数が４個以上の場合
                
                '座標指定があるかどうかが確定しているか？
                If (list(2) = "-" Or IsNumeric(list(2)) Or IsExpr(list(2))) _
                    And (list(3) = "-" Or IsNumeric(list(3)) Or IsExpr(list(3))) _
                Then
                    '座標指定があることが確定
                    ArgNum = 4
                    ReDim strArgs(2 To 4)
                    ReDim lngArgs(2 To 4)
                    ReDim dblArgs(2 To 4)
                    ReDim ArgsType(2 To 4)
                    
                    strArgs(2) = list(2)
                    strArgs(3) = list(3)
                    If Not IsExpr(list(2)) Then
                        ArgsType(2) = StringType
                    End If
                    If Not IsExpr(list(3)) Then
                        ArgsType(3) = StringType
                    End If
                Else
                    '実行時まで座標指定があるかどうか不明
                    ArgNum = 5
                    ReDim strArgs(2 To 5)
                    ReDim lngArgs(2 To 5)
                    ReDim dblArgs(2 To 5)
                    ReDim ArgsType(2 To 5)
                    
                    strArgs(2) = list(2)
                    strArgs(3) = list(3)
                    
                    '座標指定がなかった場合の表示文字列
                    buf = ListTail(edata, 2)
                    If InStr(buf, "$(") > 0 Then
                        strArgs(5) = """" & buf & """"
                    Else
                        strArgs(5) = buf
                        ArgsType(5) = StringType
                    End If
                End If
                
                '座標指定があった場合の表示文字列
                buf = ListTail(edata, 4)
                If InStr(buf, "$(") > 0 Then
                    strArgs(4) = """" & buf & """"
                Else
                    strArgs(4) = buf
                    ArgsType(4) = StringType
                End If
        End Select
        Exit Function
    End If
    
    If CmdName = CallCmd Then
        'Callコマンドのサブルーチン指定が式かどうか調べておく
        If FindNormalLabel(strArgs(2)) > 0 Then
            ArgsType(2) = StringType
        Else
            ArgsType(2) = UndefinedType
        End If
    End If
    
    If CmdName = LocalCmd Then
        If ArgNum > 4 Then
            If list(3) = "=" Then
                'Localコマンドが複数項から成る代入式を伴う場合
                
                ReDim Preserve strArgs(2 To 4)
                ReDim Preserve lngArgs(2 To 4)
                ReDim Preserve dblArgs(2 To 4)
                ReDim Preserve ArgsType(2 To 4)
                
                '代入する値
                ArgsType(4) = UndefinedType
                strArgs(4) = "(" & ListTail(edata, 4) & ")"
                ArgNum = 4
                Exit Function
            End If
        End If
    End If
    
    Exit Function
    
ErrorHandler:
    DisplayEventErrorMessage LineNum, "イベントコマンドの内容が不正です"
    Parse = False
End Function

Private Sub DebugMsg()
Dim idx As Integer, fname As String
    fname = EventFileNames(EventFileID(LineNum))
    idx = InStr2(fname, "\")
    If idx > 0 Then
        fname = Mid$(fname, idx + 1)
    End If
'    Debug.Print Format$(LineNum) & " : " & EventData(LineNum);
    Debug.Print "[" & fname & "：" & EventLineNum(LineNum) & "] ";
    Debug.Print EventData(LineNum)
End Sub

'コマンドを実行し、実行後の行番号を返す
Public Function Exec() As Long
    On Error GoTo ErrorHandler
    
'    DebugMsg
    
    Select Case Name
        Case NopCmd
            'スキップ
            Exec = LineNum + 1
        Case ArcCmd
            Exec = ExecArcCmd()
        Case ArrayCmd
            Exec = ExecArrayCmd()
        Case AskCmd
            Exec = ExecAskCmd()
        Case AttackCmd
            Exec = ExecAttackCmd()
        Case AutoTalkCmd
            Exec = ExecAutoTalkCmd()
        Case BossRankCmd
            Exec = ExecBossRankCmd()
        Case BreakCmd
            Exec = ExecBreakCmd()
        Case CallCmd
            Exec = ExecCallCmd()
        Case ReturnCmd
            Exec = ExecReturnCmd()
        Case CallInterMissionCommandCmd
            Exec = ExecCallInterMissionCommandCmd()
        Case CancelCmd
            Exec = ExecCancelCmd()
        Case CenterCmd
            Exec = ExecCenterCmd()
        Case ChangeAreaCmd
            Exec = ExecChangeAreaCmd()
'ADD START 240a
        Case ChangeLayerCmd
            Exec = ExecChangeLayerCmd()
'ADD  END  240a
        Case ChangeMapCmd
            Exec = ExecChangeMapCmd()
        Case ChangeModeCmd
            Exec = ExecChangeModeCmd()
        Case ChangePartyCmd
            Exec = ExecChangePartyCmd()
        Case ChangeTerrainCmd
            Exec = ExecChangeTerrainCmd()
        Case ChangeUnitBitmapCmd
            Exec = ExecChangeUnitBitmapCmd()
        Case ChargeCmd
            Exec = ExecChargeCmd()
        Case CircleCmd
            Exec = ExecCircleCmd()
        Case ClearEventCmd
            Exec = ExecClearEventCmd()
        Case ClearImageCmd
            Exec = ExecClearImageCmd()
'ADD START 240a
        Case ClearLayerCmd
            Exec = ExecClearLayerCmd()
'ADD  END  240a
        Case ClearObjCmd
            Exec = ExecClearObjCmd()
        Case ClearPictureCmd
            Exec = ExecClearPictureCmd()
        Case ClearSkillCmd
            Exec = ExecClearSkillCmd()
        Case ClearSpecialPowerCmd
            Exec = ExecClearSpecialPowerCmd()
        Case ClearStatusCmd
            Exec = ExecClearStatusCmd()
        Case CloseCmd
            Exec = ExecCloseCmd()
        Case ClsCmd
            Exec = ExecClsCmd()
        Case ColorCmd
            Exec = ExecColorCmd()
        Case ColorFilterCmd
            Exec = ExecColorFilterCmd()
        Case CombineCmd
            Exec = ExecCombineCmd()
        Case ConfirmCmd
            Exec = ExecConfirmCmd()
        Case ContinueCmd
            Exec = ExecContinueCmd()
        Case CopyArrayCmd
            Exec = ExecCopyArrayCmd()
        Case CopyFileCmd
            Exec = ExecCopyFileCmd()
        Case CreateCmd
            Exec = ExecCreateCmd()
        Case CreateFolderCmd
            Exec = ExecCreateFolderCmd()
        Case DebugCmd
            Exec = ExecDebugCmd()
        Case DestroyCmd
            Exec = ExecDestroyCmd()
        Case DisableCmd
            Exec = ExecDisableCmd()
        Case DoCmd
            Exec = ExecDoCmd()
        Case LoopCmd
            Exec = ExecLoopCmd()
        Case DrawOptionCmd
            Exec = ExecDrawOptionCmd()
        Case DrawWidthCmd
            Exec = ExecDrawWidthCmd()
        Case EnableCmd
            Exec = ExecEnableCmd()
        Case EquipCmd
            Exec = ExecEquipCmd()
        Case EscapeCmd
            Exec = ExecEscapeCmd()
        Case ExchangeItemCmd
            Exec = ExecExchangeItemCmd()
        Case ExecCmd
            Exec = ExecExecCmd()
        Case ExitCmd
            Exec = ExecExitCmd()
        Case ExplodeCmd
            Exec = ExecExplodeCmd()
        Case ExpUpCmd
            Exec = ExecExpUpCmd()
        Case FadeInCmd
            Exec = ExecFadeInCmd()
        Case FadeOutCmd
            Exec = ExecFadeOutCmd()
        Case FillColorCmd
            Exec = ExecFillColorCmd()
        Case FillStyleCmd
            Exec = ExecFillStyleCmd()
        Case FinishCmd
            Exec = ExecFinishCmd()
        Case FixCmd
            Exec = ExecFixCmd()
        Case FontCmd
            Exec = ExecFontCmd()
        Case ForCmd
            Exec = ExecForCmd()
        Case ForEachCmd
            Exec = ExecForEachCmd()
        Case NextCmd
            Exec = ExecNextCmd()
        Case ForgetCmd
            Exec = ExecForgetCmd()
        Case GameClearCmd
            Exec = ExecGameClearCmd()
        Case GameOverCmd
            Exec = ExecGameOverCmd()
        Case FreeMemoryCmd
            Exec = ExecFreeMemoryCmd()
        Case GetOffCmd
            Exec = ExecGetOffCmd()
        Case GlobalCmd
            Exec = ExecGlobalCmd()
        Case GotoCmd
            Exec = ExecGotoCmd()
        Case HideCmd
            Exec = ExecHideCmd()
        Case HotPointCmd
            Exec = ExecHotPointCmd()
        Case IfCmd
            Exec = ExecIfCmd()
        Case ElseCmd, ElseIfCmd
            Exec = ExecElseCmd()
        Case EndIfCmd
            'スキップ
            Exec = LineNum + 1
        Case IncrCmd
            Exec = ExecIncrCmd()
        Case IncreaseMoraleCmd
            Exec = ExecIncreaseMoraleCmd()
        Case InputCmd
            Exec = ExecInputCmd()
' MOD START マージ
'        Case InterMissionCommandCmd
'            Exec = ExecInterMissionCommandCmd()
        Case IntermissionCommandCmd
            Exec = ExecIntermissionCommandCmd()
' MOD ENDマージ
        Case ItemCmd
            Exec = ExecItemCmd()
        Case JoinCmd
            Exec = ExecJoinCmd()
        Case KeepBGMCmd
            Exec = ExecKeepBGMCmd()
        Case LandCmd
            Exec = ExecLandCmd()
        Case LaunchCmd
            Exec = ExecLaunchCmd()
        Case LeaveCmd
            Exec = ExecLeaveCmd()
        Case LevelUpCmd
            Exec = ExecLevelUpCmd()
        Case LineCmd
            Exec = ExecLineCmd()
        Case LineReadCmd
            Exec = ExecLineReadCmd()
        Case LoadCmd
            Exec = ExecLoadCmd()
        Case LocalCmd
            Exec = ExecLocalCmd()
        Case MakePilotListCmd
            Exec = ExecMakePilotListCmd()
        Case MakeUnitListCmd
            Exec = ExecMakeUnitListCmd()
        Case MapAbilityCmd
            Exec = ExecMapAbilityCmd()
        Case MapAttackCmd
            Exec = ExecMapAttackCmd()
        Case SpecialPowerCmd
            Exec = ExecSpecialPowerCmd()
        Case MoneyCmd
            Exec = ExecMoneyCmd()
        Case MonotoneCmd
            Exec = ExecMonotoneCmd()
        Case MoveCmd
            Exec = ExecMoveCmd()
        Case NightCmd
            Exec = ExecNightCmd()
        Case NoonCmd
            Exec = ExecNoonCmd()
        Case OpenCmd
            Exec = ExecOpenCmd()
        Case OptionCmd
            Exec = ExecOptionCmd()
        Case OrganizeCmd
            Exec = ExecOrganizeCmd()
        Case OvalCmd
            Exec = ExecOvalCmd()
        Case PaintPictureCmd
            Exec = ExecPaintPictureCmd()
        Case PaintStringCmd, PaintStringRCmd
            Exec = ExecPaintStringCmd()
        Case PaintSysStringCmd
            Exec = ExecPaintSysStringCmd()
        Case PilotCmd
            Exec = ExecPilotCmd()
        Case PlayMIDICmd
            Exec = ExecPlayMIDICmd()
        Case PlaySoundCmd
            Exec = ExecPlaySoundCmd()
        Case PolygonCmd
            Exec = ExecPolygonCmd()
        Case PrintCmd
            Exec = ExecPrintCmd()
        Case PSetCmd
            Exec = ExecPSetCmd()
        Case QuestionCmd
            Exec = ExecQuestionCmd()
        Case QuickLoadCmd
            Exec = ExecQuickLoadCmd()
        Case QuitCmd
            Exec = ExecQuitCmd()
        Case RankUpCmd
            Exec = ExecRankUpCmd()
        Case ReadCmd
            Exec = ExecReadCmd()
        Case RecoverENCmd
            Exec = ExecRecoverENCmd()
        Case RecoverHPCmd
            Exec = ExecRecoverHPCmd()
        Case RecoverPlanaCmd
            Exec = ExecRecoverPlanaCmd()
        Case RecoverSPCmd
            Exec = ExecRecoverSPCmd()
        Case RedrawCmd
            Exec = ExecRedrawCmd()
        Case RefreshCmd
            Exec = ExecRefreshCmd()
        Case ReleaseCmd
            Exec = ExecReleaseCmd()
        Case RemoveFileCmd
            Exec = ExecRemoveFileCmd()
        Case RemoveFolderCmd
            Exec = ExecRemoveFolderCmd()
        Case RemoveItemCmd
            Exec = ExecRemoveItemCmd()
        Case RemovePilotCmd
            Exec = ExecRemovePilotCmd()
        Case RemoveUnitCmd
            Exec = ExecRemoveUnitCmd()
        Case RenameBGMCmd
            Exec = ExecRenameBGMCmd()
        Case RenameFileCmd
            Exec = ExecRenameFileCmd()
        Case RenameTermCmd
            Exec = ExecRenameTermCmd()
        Case ReplacePilotCmd
            Exec = ExecReplacePilotCmd()
        Case RequireCmd
            Exec = ExecRequireCmd()
        Case RestoreEventCmd
            Exec = ExecRestoreEventCmd()
        Case RideCmd
            Exec = ExecRideCmd()
        Case SaveDataCmd
            Exec = ExecSaveDataCmd()
        Case SelectCmd
            Exec = ExecSelectCmd()
        Case SelectTargetCmd
            Exec = ExecSelectTargetCmd()
        Case SepiaCmd
            Exec = ExecSepiaCmd()
        Case SetCmd
            Exec = ExecSetCmd()
        Case SetSkillCmd
            Exec = ExecSetSkillCmd()
        Case SetBulletCmd
            Exec = ExecSetBulletCmd()
        Case SetMessageCmd
            Exec = ExecSetMessageCmd()
        Case SetRelationCmd
            Exec = ExecSetRelationCmd()
        Case SetStatusCmd
            Exec = ExecSetStatusCmd()
'ADD START 240a
        Case SetStatusStringColorCmd
            Exec = ExecSetStatusStringColor()
'ADD  END
        Case SetStockCmd
            Exec = ExecSetStockCmd()
'ADD START 240a
        Case SetWindowColorCmd
            Exec = ExecSetWindowColor()
        Case SetWindowFrameWidthCmd
            Exec = ExecSetWindowFrameWidth()
'ADD  END
        Case ShowCmd
            Exec = ExecShowCmd()
        Case ShowImageCmd
            Exec = ExecShowImageCmd()
        Case ShowUnitStatusCmd
            Exec = ExecShowUnitStatusCmd()
        Case SkipCmd
            Exec = ExecSkipCmd()
        Case SortCmd
            Exec = ExecSortCmd()
        Case SplitCmd
            Exec = ExecSplitCmd()
        Case StartBGMCmd
            Exec = ExecStartBGMCmd()
        Case StopBGMCmd
            Exec = ExecStopBGMCmd()
        Case StopSummoningCmd
            Exec = ExecStopSummoningCmd()
        Case SunsetCmd
            Exec = ExecSunsetCmd()
        Case SupplyCmd
            Exec = ExecSupplyCmd()
        Case SwapCmd
            Exec = ExecSwapCmd()
        Case SwitchCmd
            Exec = ExecSwitchCmd()
        Case CaseCmd, CaseElseCmd
            Exec = ExecCaseCmd()
        Case EndSwCmd
            'スキップ
            Exec = LineNum + 1
        Case TalkCmd
            Exec = ExecTalkCmd()
        Case TelopCmd
            Exec = ExecTelopCmd()
        Case TransformCmd
            Exec = ExecTransformCmd()
        Case UnitCmd
            Exec = ExecUnitCmd()
        Case UnsetCmd
            Exec = ExecUnsetCmd()
        Case UpgradeCmd
            Exec = ExecUpgradeCmd()
        Case UpVarCmd
            Exec = ExecUpvarCmd()
        Case UseAbilityCmd
            Exec = ExecUseAbilityCmd()
        Case WaitCmd
            Exec = ExecWaitCmd()
        Case WaterCmd
            Exec = ExecWaterCmd()
        Case WhiteInCmd
            Exec = ExecWhiteInCmd()
        Case WhiteOutCmd
            Exec = ExecWhiteOutCmd()
        Case WriteCmd
            Exec = ExecWriteCmd()
        Case PlayFlashCmd
            Exec = ExecPlayFlashCmd()
        Case ClearFlashCmd
            Exec = ExecClearFlashCmd()
        Case Else
            EventErrorMessage = _
                ListIndex(EventData(LineNum), 1) & "というコマンドは存在しません"
            Error 0
    End Select
    
    Exit Function

ErrorHandler:
    If Len(EventErrorMessage) > 0 Then
        DisplayEventErrorMessage LineNum, EventErrorMessage
        EventErrorMessage = ""
    ElseIf LCase$(ListIndex(EventData(LineNum), 1)) = "talk" Then
        DisplayEventErrorMessage LineNum, _
            "Talkコマンド実行中に不正な処理が行われました。" & _
            "MIDIがソフトウェアシンセサイザで演奏されているか、" & _
            "フォントキャッシュが壊れている可能性があります。" & _
            "詳しくはSRC公式ホームページの「よくある質問集」をご覧下さい。"
    ElseIf LCase$(ListIndex(EventData(LineNum), 1)) = "autotalk" Then
        DisplayEventErrorMessage LineNum, _
            "AutoTalkコマンド実行中に不正な処理が行われました。" & _
            "MIDIがソフトウェアシンセサイザで演奏されているか、" & _
            "フォントキャッシュが壊れている可能性があります。" & _
            "詳しくはSRC公式ホームページの「よくある質問集」をご覧下さい。"
    Else
        DisplayEventErrorMessage LineNum, "イベントデータが不正です"
    End If
    Exec = -1
End Function


'idx番目の引数を式として評価せずにそのまま返す
Public Function GetArg(ByVal idx As Integer) As String
    GetArg = strArgs(idx)
End Function

'idx番目の引数の値を文字列として返す
Public Function GetArgAsString(ByVal idx As Integer) As String
    Select Case ArgsType(idx)
        Case UndefinedType
            GetArgAsString = GetValueAsString(strArgs(idx), True)
        Case StringType
            GetArgAsString = strArgs(idx)
        Case NumericType
            GetArgAsString = Format$(dblArgs(idx))
    End Select
End Function

'idx番目の引数の値をLongとして返す
Public Function GetArgAsLong(ByVal idx As Integer) As Long
    Select Case ArgsType(idx)
        Case UndefinedType
            GetArgAsLong = GetValueAsLong(strArgs(idx), True)
        Case StringType
            GetArgAsLong = 0
        Case NumericType
            GetArgAsLong = lngArgs(idx)
    End Select
End Function

'idx番目の引数の値をDoubleとして返す
Public Function GetArgAsDouble(ByVal idx As Integer) As Double
    Select Case ArgsType(idx)
        Case UndefinedType
            GetArgAsDouble = GetValueAsDouble(strArgs(idx), True)
        Case StringType
            GetArgAsDouble = 0
        Case NumericType
            GetArgAsDouble = dblArgs(idx)
    End Select
End Function

'idx番目の引数が示すユニットを返す
Public Function GetArgAsUnit(ByVal idx As Integer, _
    Optional ByVal ignore_error As Boolean) As Unit
Dim pname As String

    pname = GetArgAsString(idx)
    Set GetArgAsUnit = UList.Item2(pname)
    If GetArgAsUnit Is Nothing Then
        If Not PList.IsDefined(pname) Then
            EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
            Error 0
        End If
        Set GetArgAsUnit = PList.Item(pname).Unit
        
        If Not ignore_error Then
            If GetArgAsUnit Is Nothing Then
                EventErrorMessage = "「" & pname & "」はユニットに乗っていません"
                Error 0
            End If
        End If
    End If
End Function

'idx番目の引数が示すパイロットを返す
Public Function GetArgAsPilot(ByVal idx As Integer) As Pilot
Dim pname As String

    pname = GetArgAsString(idx)
    If Not PList.IsDefined(pname) Then
        EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
        Error 0
    End If
    Set GetArgAsPilot = PList.Item(pname)
End Function


'ArgsTypeを参照する
Friend Function GetArgsType(ByVal idx As Integer) As ValueType
    GetArgsType = ArgsType(idx)
End Function

'ArgsTypeを設定する
Friend Sub SetArgsType(ByVal idx As Integer, ByVal new_type As ValueType)
    ArgsType(idx) = new_type
End Sub


Private Function ExecArcCmd() As Long
Dim pic As PictureBox, pic2 As PictureBox
Dim x1 As Integer, y1 As Integer, rad As Integer
Dim start_angle As Double, end_angle As Double
Dim opt As String
Dim cname As String, clr As Long
Dim i As Integer

    If ArgNum < 6 Then
        EventErrorMessage = "Arcコマンドの引数の数が違います"
        Error 0
    End If
    
    x1 = GetArgAsLong(2) + BaseX
    y1 = GetArgAsLong(3) + BaseY
    rad = GetArgAsLong(4)
    start_angle = 3.1415926535 * GetArgAsDouble(5) / 180
    end_angle = 3.1415926535 * GetArgAsDouble(6) / 180
    
    '塗りつぶしの際は角度を負の値にする必要がある
    If ObjFillStyle <> vbFSTransparent Then
        start_angle = -start_angle
        If start_angle = 0 Then
            start_angle = -0.000001
        End If
        end_angle = -end_angle
        If end_angle = 0 Then
            end_angle = -0.000001
        End If
    End If
    
    SaveScreen
    
    '描画先
    Select Case ObjDrawOption
        Case "背景"
            Set pic = MainForm.picBack
            Set pic2 = MainForm.picMaskedBack
            IsMapDirty = True
       Case "保持"
            Set pic = MainForm.picMain(0)
            Set pic2 = MainForm.picMain(1)
        Case Else
            Set pic = MainForm.picMain(0)
    End Select
    
    '描画領域
    If ObjDrawOption <> "背景" Then
        Dim tmp As Integer
        IsPictureVisible = True
        tmp = rad + ObjDrawWidth - 1
        PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(x1 - tmp, 0))
        PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(y1 - tmp, 0))
        PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(x1 + tmp, MainPWidth - 1))
        PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(y1 + tmp, MainPHeight - 1))
    End If
    
    clr = ObjColor
    For i = 7 To ArgNum
        opt = GetArgAsString(i)
        If Asc(opt) = 35 Then '#
            If Len(opt) <> 7 Then
                EventErrorMessage = "色指定が不正です"
                Error 0
            End If
            cname = String$(8, vbNullChar)
            Mid(cname, 1, 2) = "&H"
            Mid(cname, 3, 2) = Mid$(opt, 6, 2)
            Mid(cname, 5, 2) = Mid$(opt, 4, 2)
            Mid(cname, 7, 2) = Mid$(opt, 2, 2)
            If Not IsNumeric(cname) Then
                EventErrorMessage = "色指定が不正です"
                Error 0
            End If
            clr = CLng(cname)
        Else
            EventErrorMessage = _
                "Arcコマンドに不正なオプション「" & opt & "」が使われています"
            Error 0
        End If
    Next
    
    With pic
        .DrawWidth = ObjDrawWidth
        .FillColor = ObjFillColor
        .FillStyle = ObjFillStyle
    End With
    
    pic.Circle (x1, y1), rad, clr, start_angle, end_angle
    
    With pic
        .DrawWidth = 1
        .FillColor = vbWhite
        .FillStyle = vbFSTransparent
    End With
    
    If Not pic2 Is Nothing Then
        With pic2
            .DrawWidth = ObjDrawWidth
            .FillColor = ObjFillColor
            .FillStyle = ObjFillStyle
        End With
        
        pic2.Circle (x1, y1), rad, clr, start_angle, end_angle
        
        With pic2
            .DrawWidth = 1
            .FillColor = vbWhite
            .FillStyle = vbFSTransparent
        End With
    End If
    
    ExecArcCmd = LineNum + 1
End Function

Private Function ExecArrayCmd() As Long
Dim array_buf As Variant, array_buf2() As String, buf As String
Dim var_name As String, vname As String
Dim i As Long, num As Integer
Dim IsList As Boolean
Dim etype As ValueType, str_value As String, num_value As Double
Dim sep As String
    
    If ArgNum <> 4 Then
        EventErrorMessage = "Arrayコマンドの引数の数が違います"
        Error 0
    Else
        If GetArgAsString(4) = "リスト" Then
            IsList = True
        Else
            IsList = False
        End If
    End If
    
    '代入先の変数名
    var_name = GetArg(2)
    If Left$(var_name, 1) = "$" Then
        var_name = Mid$(var_name, 2)
    End If
    'Eval関数
    If LCase$(Left$(var_name, 5)) = "eval(" Then
        If Right$(var_name, 1) = ")" Then
            var_name = Mid$(var_name, 6, Len(var_name) - 6)
            var_name = GetValueAsString(var_name)
        End If
    End If
    
    '代入先の変数を初期化した上で再設定
    'サブルーチンローカル変数の場合
    If IsSubLocalVariableDefined(var_name) Then
        UndefineVariable var_name
        VarIndex = VarIndex + 1
        With VarStack(VarIndex)
            .Name = var_name
            .VariableType = NumericType
            .StringValue = ""
            .NumericValue = 0
        End With
    'ローカル変数の場合
    ElseIf IsLocalVariableDefined(var_name) Then
        UndefineVariable var_name
        DefineLocalVariable var_name
    'グローバル変数の場合
    ElseIf IsGlobalVariableDefined(var_name) Then
        UndefineVariable var_name
        DefineGlobalVariable var_name
    End If
    
    If IsList Then
        'リストを配列に変換
        num = ListSplit(GetArgAsString(3), array_buf2)
        array_buf = array_buf2
    Else
        '文字列を分割して配列に代入
        ReDim array_buf(0)
        buf = GetArgAsString(3)
        sep = GetArgAsString(4)
        i = InStr(buf, sep)
        Do While i > 0
            ReDim Preserve array_buf(UBound(array_buf) + 1)
            array_buf(UBound(array_buf)) = Left$(buf, i - 1)
            buf = Mid$(buf, i + Len(sep))
            i = InStr(buf, sep)
        Loop
        ReDim Preserve array_buf(UBound(array_buf) + 1)
        array_buf(UBound(array_buf)) = buf
    End If
    
    For i = 1 To UBound(array_buf)
        buf = CStr(array_buf(i))
        TrimString buf
        If IsNumeric(buf) Then
            etype = NumericType
            str_value = ""
            num_value = StrToDbl(buf)
        Else
            etype = StringType
            str_value = buf
            num_value = 0
        End If
        vname = var_name & "[" & CStr(i) & "]"
        SetVariable vname, etype, str_value, num_value
    Next
    
    ExecArrayCmd = LineNum + 1
End Function

Private Function ExecAskCmd() As Long
Dim use_normal_list As Boolean
Dim use_large_list As Boolean
Dim use_continuous_mode As Boolean
Dim enable_rbutton_cancel As Boolean
Dim list() As String, msg As String
Dim vname As String
Dim i As Long
Dim buf As String
Dim var As VarData
    
    ReDim list(0)
    ReDim ListItemID(0)
    ReDim ListItemFlag(0)
    
    '表示オプションの処理
    i = ArgNum
    Do While i > 1
        Select Case GetArg(i)
            Case "通常"
                use_normal_list = True
            Case "拡大"
                use_large_list = True
            Case "連続表示"
                use_continuous_mode = True
            Case "キャンセル可"
                enable_rbutton_cancel = True
            Case "終了"
                frmListBox.Hide
                If AutoMoveCursor Then
                    RestoreCursorPos
                End If
                ReduceListBoxHeight
                ExecAskCmd = LineNum + 1
                Exit Function
            Case Else
                Exit Do
        End Select
        i = i - 1
    Loop
    
    'オプションではない引数の数で書式タイプを判別
    Select Case i
        'イベントデータ中に選択肢を列挙している場合
        Case 1, 2
            If ArgNum = 1 Then
                msg = "いずれかを選んでください"
            Else
                msg = GetArgAsString(2)
            End If
            ListItemID(0) = "0"
            
            '選択肢の読みこみ
            For i = LineNum + 1 To UBound(EventData)
                buf = EventData(i)
                FormatMessage buf
                If Len(buf) > 0 Then
                    If EventCmd(i).Name = EndCmd Then
                        Exit For
                    End If
                    ReDim Preserve list(UBound(list) + 1)
                    ReDim Preserve ListItemID(UBound(list))
                    ReDim Preserve ListItemFlag(UBound(list))
                    list(UBound(list)) = buf
                    ListItemID(UBound(list)) = Format$(i - LineNum)
                    ListItemFlag(UBound(list)) = False
                End If
            Next
            
            If i > UBound(EventData) Then
                EventErrorMessage = "AskとEndが対応していません"
                Error 0
            End If
            
            ExecAskCmd = i + 1
        
        '選択肢を配列で指定する場合
        Case 3
            vname = GetArg(2)
            msg = GetArgAsString(3)
            ListItemID(0) = ""
            
            '配列の検索
            If IsSubLocalVariableDefined(vname) Then
                If Left$(vname, 1) = "$" Then
                    vname = Mid$(vname, 2) & "["
                Else
                    vname = vname & "["
                End If
                For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
                    With VarStack(i)
                        If InStr(.Name, vname) = 1 Then
                            If .VariableType = StringType Then
                                buf = .StringValue
                            Else
                                buf = Format$(.NumericValue)
                            End If
                            If Len(buf) > 0 Then
                                ReDim Preserve list(UBound(list) + 1)
                                ReDim Preserve ListItemID(UBound(list))
                                ReDim Preserve ListItemFlag(UBound(list))
                                FormatMessage buf
                                list(UBound(list)) = buf
                                ListItemID(UBound(list)) = _
                                    Mid$(.Name, Len(vname) + 1, Len(.Name) - Len(vname) - 1)
                                ListItemFlag(UBound(list)) = False
                            End If
                        End If
                    End With
                Next
            ElseIf IsLocalVariableDefined(vname) Then
                If Left$(vname, 1) = "$" Then
                    vname = Mid$(vname, 2) & "["
                Else
                    vname = vname & "["
                End If
                For Each var In LocalVariableList
                    With var
                        If InStr(.Name, vname) = 1 Then
                            If .VariableType = StringType Then
                                buf = .StringValue
                            Else
                                buf = Format$(.NumericValue)
                            End If
                            If Len(buf) > 0 Then
                                ReDim Preserve list(UBound(list) + 1)
                                ReDim Preserve ListItemID(UBound(list))
                                ReDim Preserve ListItemFlag(UBound(list))
                                FormatMessage buf
                                list(UBound(list)) = buf
                                ListItemID(UBound(list)) = _
                                    Mid$(.Name, Len(vname) + 1, Len(.Name) - Len(vname) - 1)
                                ListItemFlag(UBound(list)) = False
                            End If
                        End If
                    End With
                Next
            ElseIf IsGlobalVariableDefined(vname) Then
                If Left$(vname, 1) = "$" Then
                    vname = Mid$(vname, 2) & "["
                Else
                    vname = vname & "["
                End If
                For Each var In GlobalVariableList
                    With var
                        If InStr(.Name, vname) = 1 Then
                            If .VariableType = StringType Then
                                buf = .StringValue
                            Else
                                buf = Format$(.NumericValue)
                            End If
                            If Len(buf) > 0 Then
                                ReDim Preserve list(UBound(list) + 1)
                                ReDim Preserve ListItemID(UBound(list))
                                ReDim Preserve ListItemFlag(UBound(list))
                                FormatMessage buf
                                list(UBound(list)) = buf
                                ListItemID(UBound(list)) = _
                                    Mid$(.Name, Len(vname) + 1, Len(.Name) - Len(vname) - 1)
                                ListItemFlag(UBound(list)) = False
                            End If
                        End If
                    End With
                Next
            End If
            
            ExecAskCmd = LineNum + 1
        Case Else
            EventErrorMessage = "Askコマンドのオプションが不正です"
            Error 0
    End Select
    
    '選択肢がなければそのまま終了
    If UBound(list) = 0 Then
        SelectedAlternative = 0
        Exit Function
    End If
    
    'ダイアログを拡大するか
    If Not use_normal_list _
        And (UBound(list) > 6 Or use_large_list) _
    Then
        EnlargeListBoxHeight
    Else
        ReduceListBoxHeight
    End If
        
    If AutoMoveCursor Then
        TopItem = 1
        SelectedItem = ListBox("選択", list, msg, "表示のみ")
        MoveCursorPos "ダイアログ"
    End If
    
    '選択肢の入力
    Do
        TopItem = 1
        If use_continuous_mode Then
            SelectedItem = ListBox("選択", list, msg, "連続表示")
        Else
            SelectedItem = ListBox("選択", list, msg)
        End If
        If enable_rbutton_cancel Then
            If SelectedItem = 0 Then
                Exit Do
            End If
        End If
    Loop While SelectedItem = 0
    
    SelectedAlternative = ListItemID(SelectedItem)
    ReDim ListItemID(0)
    
    If Not use_continuous_mode Then
        If AutoMoveCursor Then
            RestoreCursorPos
        End If
    End If
    
    'ダイアログを標準の大きさに戻す
    If Not use_normal_list _
        And Not use_continuous_mode _
        And (UBound(list) > 6 Or use_large_list) _
    Then
        ReduceListBoxHeight
    End If
End Function

Private Function ExecAttackCmd() As Long
Dim u1 As Unit, u2 As Unit
Dim w1 As Integer, w2 As Integer
Dim prev_su As Unit, prev_st As Unit
Dim prev_w As Integer, prev_tw As Integer
Dim def_mode As String, cur_stage As String
Dim is_event As Boolean
Dim def_option As String
    
    is_event = True
    
    Select Case ArgNum
        Case 5
            'ＯＫ
        Case 6
            If GetArgAsString(6) = "通常戦闘" Then
                is_event = False
            Else
                EventErrorMessage = "Attackコマンドのオプションが不正です"
                Error 0
            End If
        Case Else
            EventErrorMessage = "Attackコマンドの引数の数が違います"
            Error 0
    End Select
    
    Set u1 = GetArgAsUnit(2)
    Set u2 = GetArgAsUnit(4)
    
    If u1.Status = "出撃" And u2.Status = "出撃" Then
        If GetArgAsString(3) = "自動" Then
            w1 = SelectWeapon(u1, u2, "イベント")
        Else
            For w1 = 1 To u1.CountWeapon
                If GetArgAsString(3) = u1.Weapon(w1).Name _
                    And Not u1.IsWeaponClassifiedAs(w1, "マップ攻撃") _
                Then
                    Exit For
                End If
            Next
            If w1 > u1.CountWeapon Then
                EventErrorMessage = "ユニット「" & u1.Name & "」には武装「" _
                    & GetArgAsString(3) & "」は存在しません"
                Error 0
            End If
        End If
        
        def_option = GetArgAsString(5)
        Select Case def_option
            Case "防御", "回避", "無抵抗"
                def_mode = GetArgAsString(5)
            Case "反撃不能"
                def_mode = "反撃"
                w2 = 0
            Case "自動"
                def_mode = "反撃"
                w2 = SelectWeapon(u2, u1, "反撃 イベント")
            Case Else
                def_mode = "反撃"
                For w2 = 1 To u2.CountWeapon
                    If GetArgAsString(5) = u2.Weapon(w2).Name _
                        And Not u2.IsWeaponClassifiedAs(w2, "マップ攻撃") _
                    Then
                        Exit For
                    End If
                Next
                If w2 > u2.CountWeapon Then
                    EventErrorMessage = "ユニット「" & u2.Name & "」には武装「" _
                        & GetArgAsString(5) & "」は存在しません"
                    Error 0
                End If
        End Select
            
        If w1 > 0 Then
            Set prev_su = SelectedUnit
            Set prev_st = SelectedTarget
            prev_w = SelectedWeapon
            prev_tw = SelectedTWeapon
            Set SelectedUnit = u1
            Set SelectedTarget = u2
            SelectedWeapon = w1
            SelectedTWeapon = w2
            
            If u1.Party0 = "味方" Or u1.Party0 = "ＮＰＣ" Then
                OpenMessageForm u2, u1
            Else
                OpenMessageForm u1, u2
            End If
            
            '攻撃を実行
            cur_stage = Stage
            Stage = u1.Party
            u1.Attack w1, u2, "", def_mode, is_event
            Set u1 = u1.CurrentForm
            
            '反撃用武器がまだ使用可能かチェック
' MOD START マージ
'            If def_option = "自動" Then
'                If Not u2.IsTargetWithinRange(w2, u1) Then
'                    w2 = SelectWeapon(u2, u1, "反撃 イベント")
'                    SelectedTWeapon = w2
'                End If
'            End If
            If def_option = "自動" _
                And u2.Status = "出撃" _
            Then
                If Not u2.IsTargetWithinRange(w2, u1) _
                    Or Not u2.IsWeaponAvailable(w2, "移動前") _
                Then
                    w2 = SelectWeapon(u2, u1, "反撃 イベント")
                    SelectedTWeapon = w2
                End If
            End If
' MOD END マージ
            
            '反撃を実行
' MOD START マージ
'            If def_mode = "反撃" _
'                And u2.Status = "出撃" _
'                And Not u2.IsConditionSatisfied("行動不能") _
'            Then
            If def_mode = "反撃" _
                And u2.Status = "出撃" _
                And u2.MaxAction > 0 _
                And Not u2.IsConditionSatisfied("攻撃不能") _
            Then
' MOD END マージ
                If w2 > 0 Then
                    u2.Attack w2, u1, "", "", is_event
                Else
                    u2.PilotMessage "射程外"
                End If
            End If
            Stage = cur_stage
            
            CloseMessageForm
            
            u1.CurrentForm.UpdateCondition
            u2.CurrentForm.UpdateCondition
            
            u1.CurrentForm.CheckAutoHyperMode
            u1.CurrentForm.CheckAutoNormalMode
            u2.CurrentForm.CheckAutoHyperMode
            u2.CurrentForm.CheckAutoNormalMode
            
            Set SelectedUnit = prev_su
            Set SelectedTarget = prev_st
            SelectedWeapon = prev_w
            SelectedTWeapon = prev_tw
        End If
    End If
    
    RedrawScreen
    
    ExecAttackCmd = LineNum + 1
End Function

Private Function ExecAutoTalkCmd() As Long
Dim pname As String, current_pname As String
Dim u As Unit, ux As Integer, uy As Integer
Dim i As Long, j As Integer
Dim lnum As Integer
Dim prev_msg_wait As Long
Dim without_cursor As Boolean
Dim options As String, opt As String
Dim buf As String

    'メッセージ表示速度を「普通」の値に設定
    prev_msg_wait = MessageWait
    MessageWait = 700
    
    For i = LineNum To UBound(EventData)
        With EventCmd(i)
            Select Case .Name
                Case AutoTalkCmd
                    If .ArgNum > 1 Then
                        pname = .GetArgAsString(2)
                    Else
                        pname = ""
                    End If
                    
                    If Left$(pname, 1) = "@" Then
                        'メインパイロットの強制指定
                        pname = Mid$(pname, 2)
                        If PList.IsDefined(pname) Then
                            With PList.Item(pname)
                                If Not .Unit Is Nothing Then
                                    pname = .Unit.MainPilot.Name
                                End If
                            End With
                        End If
                    End If
                    
                    '話者名チェック
                    If Not PList.IsDefined(pname) _
                        And Not PDList.IsDefined(pname) _
                        And Not NPDList.IsDefined(pname) _
                        And Not pname = "システム" _
                        And Not pname = "" _
                    Then
                        EventErrorMessage = _
                            "「" & pname & "」というパイロットが定義されていません"
                        LineNum = i
                        Error 0
                    End If
                    
                    If .ArgNum > 1 Then
                        options = ""
                        without_cursor = False
                        j = 2
                        lnum = 1
                        Do While j <= .ArgNum
                            opt = .GetArgAsString(j)
                            Select Case opt
                                Case "非表示"
                                    without_cursor = True
                                 Case "枠外"
                                    MessageWindowIsOut = True
                                Case "白黒", "セピア", _
                                    "明", "暗", "上下反転", "左右反転", _
                                    "上半分", "下半分", "右半分", "左半分", _
                                    "右上", "左上", "右下", "左下", _
                                    "ネガポジ反転", "シルエット", _
                                    "夕焼け", "水中", "通常"
                                    If j > 2 Then
                                        'これらのパイロット画像描画に関するオプションは
                                        'パイロット名が指定されている場合にのみ有効
                                        options = options & opt & " "
                                    Else
                                        lnum = j
                                    End If
                                Case "右回転"
                                    j = j + 1
                                    options = options & _
                                        "右回転 " & .GetArgAsString(j) & " "
                                Case "左回転"
                                    j = j + 1
                                    options = options & _
                                        "左回転 " & .GetArgAsString(j) & " "
                                Case "フィルタ"
                                    Dim cname As String, tcolor As Long
                                    j = j + 1
                                    buf = .GetArgAsString(j)
                                    cname = String$(8, vbNullChar)
                                    Mid(cname, 1, 2) = "&H"
                                    Mid(cname, 3, 2) = Mid$(buf, 6, 2)
                                    Mid(cname, 5, 2) = Mid$(buf, 4, 2)
                                    Mid(cname, 7, 2) = Mid$(buf, 2, 2)
                                    tcolor = CLng(cname)
                                    j = j + 1
                                    options = options & _
                                        "フィルタ " & Format$(tcolor) & " " & .GetArgAsString(j) & " "
                                Case ""
                                    '空白のオプションをスキップ
                                Case Else
                                    '通常の引数をスキップ
                                    lnum = j
                            End Select
                            j = j + 1
                        Loop
                    Else
                        lnum = 1
                    End If
                    
                    Select Case lnum
                        Case 0, 1
                            '引数なし
                            
                            If Not frmMessage.Visible Then
                                OpenMessageForm
                            End If
                            
                            'メッセージウィンドウのパイロット画像を以前指定された
                            'ものに確定させる
                            If current_pname <> "" Then
                                DisplayBattleMessage current_pname, "", options
                            End If
                            
                            current_pname = ""
                            
                        Case 2
                            'パイロット名のみ指定
                            current_pname = pname
                            
                            '話者中心に画面位置を変更
                            
                            'プロローグイベントやエピローグイベント時はキャンセル
                            If Stage = "プロローグ" _
                                Or Stage = "エピローグ" _
                            Then
                                GoTo NextLoop
                            End If
                            
                            '画面書き換え可能？
                            If Not MainForm.Visible Then
                                GoTo NextLoop
                            End If
                            If IsPictureVisible Then
                                GoTo NextLoop
                            End If
                            If MapFileName = "" Then
                                GoTo NextLoop
                            End If
                            
                            '話者を中央表示
                            CenterUnit pname, without_cursor
                            
                        Case 3
                            current_pname = pname
                            Select Case .GetArgAsString(3)
                                Case "母艦"
                                    '母艦の中央表示
                                    CenterUnit "母艦", without_cursor
                                Case "中央"
                                    '話者を中央表示
                                    CenterUnit pname, without_cursor
                                Case "固定"
                                    '表示位置固定
                            End Select
                            
                        Case 4
                            '表示の座標指定あり
                            current_pname = pname
                            CenterUnit "", without_cursor, .GetArgAsLong(3), .GetArgAsLong(4)
                            
                        Case -1
                            EventErrorMessage = _
                                "AutoTalkコマンドのパラメータの括弧の対応が取れていません"
                            LineNum = i
                            Error 0
                            
                        Case Else
                            EventErrorMessage = "AutoTalkコマンドの引数の数が違います"
                            LineNum = i
                            Error 0
                    End Select
                    
                    If Not frmMessage.Visible Then
                        OpenMessageForm
                    End If
                    
                Case EndCmd
                    CloseMessageForm
                    If .ArgNum <> 1 Then
                        EventErrorMessage = "End部分の引数の数が違います"
                        LineNum = i
                        Error 0
                    End If
                    Exit For
                    
                Case SuspendCmd
                    If .ArgNum <> 1 Then
                        EventErrorMessage = "Suspend部分の引数の数が違います"
                        LineNum = i
                        Error 0
                    End If
                    Exit For
                    
                Case Else
                    If Not frmMessage.Visible Then
                        OpenMessageForm
                    End If
                    DisplayBattleMessage current_pname, EventData(i), options
                    
            End Select
        End With
NextLoop:
    Next
    
    'メッセージ表示速度を元に戻す
    MessageWait = prev_msg_wait
    
    If i > UBound(EventData) Then
        CloseMessageForm
        EventErrorMessage = "AutoTalkとEndが対応していません"
        Error 0
    End If
    
    ExecAutoTalkCmd = i + 1
End Function

'話者の中央表示用サブルーチン
Private Sub CenterUnit(ByVal pname As String, ByVal without_cursor As Boolean, _
    Optional ByVal X As Integer, Optional ByVal Y As Integer)
Dim u As Unit, xx As Integer, yy As Integer

    '座標が指定されている場合
    If X <> 0 And Y <> 0 Then
        If X < 1 Or MapWidth < X _
            Or Y < 1 Or MapHeight < Y _
        Then
            'マップ外
            Exit Sub
        End If
        
        xx = X
        yy = Y
        GoTo FoundPoint
    End If
    
    If pname = "母艦" Then
        '母艦を中央表示
        For Each u In UList
            With u
                If .Party0 = "味方" _
                    And .Status = "出撃" _
                Then
                    If .IsFeatureAvailable("母艦") Then
                        xx = .X
                        yy = .Y
                        GoTo FoundPoint
                    End If
                End If
            End With
        Next
        Exit Sub
    End If
    
    '表情パターン名での指定はパイロット名に変換しておく
    If Not PList.IsDefined(pname) _
        And InStr(pname, "(") > 0 _
    Then
        If PList.IsDefined(Left$(pname, InStr(pname, "(") - 1)) Then
            pname = Left$(pname, InStr(pname, "(") - 1)
        End If
    End If
    If Not PList.IsDefined(pname) _
        And NPDList.IsDefined(pname) _
    Then
        pname = NPDList.Item(pname).Nickname
    End If
    
    '話者はパイロット？
    If Not PList.IsDefined(pname) Then
        Exit Sub
    End If
    
    With PList.Item(pname)
        If Not .Unit Is Nothing Then
            'パイロットが乗っているユニットを中央表示
            With .Unit
                If .Status = "出撃" Or .Status = "格納" Then
                    xx = .X
                    yy = .Y
                    GoTo FoundPoint
                End If
            End With
        End If
        
        '話者が味方でかつ出撃中でない場合は母艦を中央表示
        If .Party = "味方" Then
            CenterUnit "母艦", without_cursor
        End If
    End With
    
    Exit Sub
    
FoundPoint:
    
    If MapX <> xx Or MapY <> yy Then
        Center xx, yy
        RefreshScreen False, True
    End If
    
    If Not IsCursorVisible _
        And Not without_cursor _
    Then
        Dim tmp As Boolean
        tmp = IsPictureVisible
        DrawPicture "Event\cursor.bmp", _
            DEFAULT_LEVEL, DEFAULT_LEVEL, _
            DEFAULT_LEVEL, DEFAULT_LEVEL, _
            0, 0, 0, 0, "透過"
        IsPictureVisible = tmp
        IsCursorVisible = True
    End If
    
    MainForm.picMain(0).Refresh
End Sub

Private Function ExecBossRankCmd() As Long
Dim u As Unit
Dim buf As String

    Select Case ArgNum
        Case 3
            Set u = GetArgAsUnit(2)
            
            buf = GetArgAsString(3)
            If Not IsNumeric(buf) Then
                EventErrorMessage = "ボスランクが不正です"
                Error 0
            End If
        Case 2
            Set u = SelectedUnitForEvent
            
            buf = GetArgAsString(2)
            If Not IsNumeric(buf) Then
                EventErrorMessage = "ボスランクが不正です"
                Error 0
            End If
        Case Else
            EventErrorMessage = "BossRankコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not u Is Nothing Then
        With u
            .BossRank = CInt(buf)
            .HP = .MaxHP
            .FullSupply
        End With
    End If
    
    ExecBossRankCmd = LineNum + 1
End Function

Private Function ExecBreakCmd() As Long
Dim i As Long, depth As Integer
    
    '対応するLoopもしくはNextコマンドを探す
    depth = 1
    For i = LineNum + 1 To UBound(EventCmd)
        Select Case EventCmd(i).Name
            Case DoCmd, ForCmd, ForEachCmd
                depth = depth + 1
            Case LoopCmd
                depth = depth - 1
                If depth = 0 Then
                    Exit For
                End If
            Case NextCmd
                depth = depth - 1
                If depth = 0 Then
                    ForIndex = ForIndex - 1
                    Exit For
                End If
        End Select
    Next
    
    If i > UBound(EventCmd) Then
        EventErrorMessage = "Breakコマンドがループの外で使われています"
        Error 0
    End If
    
    ExecBreakCmd = i + 1
End Function

Private Function ExecCallCmd() As Long
Dim ret As Long, i As Integer
Dim params(MaxArgIndex) As String

    'サブルーチンを探す
    ret = FindNormalLabel(GetArgAsString(2))
    
    '見つかった？
    If ret = 0 Then
        EventErrorMessage = "サブルーチンの呼び出し先ラベルである「" _
            & GetArgAsString(2) & "」がみつかりません"
        Error 0
    End If
    
    '呼び出し階層をチェック
    If CallDepth > MaxCallDepth Then
        CallDepth = MaxCallDepth
        EventErrorMessage = Format$(MaxCallDepth) & _
            "階層を越えるサブルーチンの呼び出しは出来ません"
        Error 0
    End If
    
    '引数用スタックが溢れないかチェック
    If ArgIndex + ArgNum - 2 > MaxArgIndex Then
        EventErrorMessage = _
            "サブルーチンの引数の総数が" & Format$(MaxArgIndex) & _
            "個を超えています"
        Error 0
    End If
    
    '引数の値を先に求めておく
    '(スタックに積みながら計算すると、引数での関数呼び出しで不正になる)
    For i = 3 To ArgNum
        params(i) = GetArgAsString(i)
    Next
    
    '現在の状態を保存
    CallStack(CallDepth) = LineNum
    ArgIndexStack(CallDepth) = ArgIndex
    VarIndexStack(CallDepth) = VarIndex
    ForIndexStack(CallDepth) = ForIndex
    
    'UpVarが実行された場合、UpVar実行数は累計する
    If UpVarLevel > 0 Then
        UpVarLevelStack(CallDepth) = UpVarLevel + UpVarLevelStack(CallDepth - 1)
    Else
        UpVarLevelStack(CallDepth) = 0
    End If
    
    'UpVarの階層数を初期化
    UpVarLevel = 0
    
    '引数をスタックに積む
    For i = 3 To ArgNum
        ArgStack(ArgIndex + ArgNum - i + 1) = params(i)
    Next
    ArgIndex = ArgIndex + ArgNum - 2
    
    '呼び出し階層数をインクリメント
    CallDepth = CallDepth + 1
    
    ExecCallCmd = ret + 1
End Function

Private Function ExecReturnCmd() As Long
    If CallDepth <= 0 Then
        EventErrorMessage = "CallコマンドとReturnコマンドが対応していません"
        Error 0
    ElseIf CallDepth = 1 And CallStack(CallDepth) = 0 Then
        EventErrorMessage = "CallコマンドとReturnコマンドが対応していません"
        Error 0
    End If
    
    '呼び出し階層数をデクリメント
    CallDepth = CallDepth - 1
    
    'サブルーチン実行前の状態に復帰
    ArgIndex = ArgIndexStack(CallDepth)
    VarIndex = VarIndexStack(CallDepth)
    ForIndex = ForIndexStack(CallDepth)
    UpVarLevel = UpVarLevelStack(CallDepth)
    
    ExecReturnCmd = CallStack(CallDepth) + 1
End Function

Private Function ExecCallInterMissionCommandCmd() As Long
Dim fname As String, save_path As String
Dim ret As Integer
    
    If ArgNum <> 2 Then
        EventErrorMessage = "CallInterMissionCommandコマンドの引数の数が違います"
        Error 0
    End If
    
    '選択されたインターミッションコマンドを実行
    Select Case GetArgAsString(2)
        Case "データセーブ"
            '一旦「常に手前に表示」を解除
            If frmListBox.Visible Then
                ret = SetWindowPos(frmListBox.hwnd, -2, 0, 0, 0, 0, &H3)
            End If
            
            fname = SaveFileDialog("データセーブ", _
                ScenarioPath, GetValueAsString("セーブデータファイル名"), _
                2, "ｾｰﾌﾞﾃﾞｰﾀ", "src")
            
            '再び「常に手前に表示」
            If frmListBox.Visible Then
                ret = SetWindowPos(frmListBox.hwnd, -1, 0, 0, 0, 0, &H3)
            End If
            
            'キャンセル？
            If fname = "" Then
                ExecCallInterMissionCommandCmd = LineNum + 1
                Exit Function
            End If
            
            'セーブ先はシナリオフォルダ？
            If InStr(fname, "\") > 0 Then
                save_path = Left$(fname, InStr2(fname, "\"))
            End If
            If Dir$(save_path) <> Dir$(ScenarioPath) Then
                If MsgBox("セーブファイルはシナリオフォルダにないと読み込めません。" & vbCr & vbLf _
                        & "このままセーブしますか？", vbOKCancel + vbQuestion) <> 1 _
                Then
                    ExecCallInterMissionCommandCmd = LineNum + 1
                    Exit Function
                End If
            End If
            
            If fname <> "" Then
                UList.Update '追加パイロットを消去
                SaveData fname
            End If
            
        Case "機体改造", "ユニットの強化"
            '選択用ダイアログを拡大
            EnlargeListBoxHeight
            
            RankUpCommand
            
            '選択用リストボックスを元に戻す
            ReduceListBoxHeight
            
        Case "乗り換え"
            '選択用ダイアログを拡大
            EnlargeListBoxHeight
            
            ExchangeUnitCommand
            
            '選択用リストボックスを元に戻す
            ReduceListBoxHeight
            
        Case "アイテム交換"
            '選択用ダイアログを拡大
            EnlargeListBoxHeight
            
            ExchangeItemCommand
            
            '選択用リストボックスを元に戻す
            ReduceListBoxHeight
            
        Case "換装"
            '選択用ダイアログを拡大
            EnlargeListBoxHeight
            
            ExchangeFormCommand
            
            '選択用リストボックスを元に戻す
            ReduceListBoxHeight
            
        Case "パイロットステータス"
            frmListBox.Hide
            ReduceListBoxHeight
            IsSubStage = True
            If FileExists(ScenarioPath & "Lib\パイロットステータス表示.eve") Then
                StartScenario ScenarioPath & "Lib\パイロットステータス表示.eve"
            ElseIf FileExists(ExtDataPath & "Lib\パイロットステータス表示.eve") Then
                StartScenario ExtDataPath & "Lib\パイロットステータス表示.eve"
            ElseIf FileExists(ExtDataPath2 & "Lib\パイロットステータス表示.eve") Then
                StartScenario ExtDataPath2 & "Lib\パイロットステータス表示.eve"
            Else
                StartScenario AppPath & "Lib\パイロットステータス表示.eve"
            End If
            'サブステージを通常のステージとして実行
            IsSubStage = True
            Exit Function
            
        Case "ユニットステータス"
            frmListBox.Hide
            ReduceListBoxHeight
            IsSubStage = True
            If FileExists(ScenarioPath & "Lib\ユニットステータス表示.eve") Then
                StartScenario ScenarioPath & "Lib\ユニットステータス表示.eve"
            ElseIf FileExists(ExtDataPath & "Lib\ユニットステータス表示.eve") Then
                StartScenario ExtDataPath & "Lib\ユニットステータス表示.eve"
            ElseIf FileExists(ExtDataPath2 & "Lib\ユニットステータス表示.eve") Then
                StartScenario ExtDataPath2 & "Lib\ユニットステータス表示.eve"
            Else
                StartScenario AppPath & "Lib\ユニットステータス表示.eve"
            End If
            'サブステージを通常のステージとして実行
            IsSubStage = True
            Exit Function
    End Select
    
    ExecCallInterMissionCommandCmd = LineNum + 1
End Function

Private Function ExecCancelCmd() As Long
    IsCanceled = True
    ExecCancelCmd = LineNum + 1
End Function

Private Function ExecCenterCmd() As Long
Dim num As Integer
Dim ux As Integer, uy As Integer
Dim u As Unit
Dim late_refresh As Boolean

    num = ArgNum
    
    If num > 1 Then
        If GetArgAsString(num) = "非同期" Then
            late_refresh = True
            num = num - 1
        End If
    End If
    
    Select Case num
        Case 3
            ux = GetArgAsLong(2)
            If ux < 1 Then
                ux = 1
            ElseIf ux > MapWidth Then
                ux = MapWidth
            End If
            uy = GetArgAsLong(3)
            If uy < 1 Then
                uy = 1
            ElseIf uy > MapHeight Then
                uy = MapHeight
            End If
            Center ux, uy
        Case 2
            Set u = GetArgAsUnit(2, True)
            If Not u Is Nothing Then
                With u
                    If .Status = "出撃" Then
                        Center .X, .Y
                        IsUnitCenter = True
                    End If
                End With
            End If
        Case Else
            EventErrorMessage = "Centerコマンドの引数の数が違います"
            Error 0
    End Select
    
    RedrawScreen late_refresh
    
    ExecCenterCmd = LineNum + 1
End Function

Private Function ExecChangeAreaCmd() As Long
Dim new_area As String
Dim u As Unit

    Select Case ArgNum
        Case 2
            Set u = SelectedUnitForEvent
            new_area = GetArgAsString(2)
        Case 3
            Set u = GetArgAsUnit(2)
            new_area = GetArgAsString(3)
        Case Else
            EventErrorMessage = "ChangeAreaコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        Select Case TerrainClass(.X, .Y)
            Case "陸"
                If new_area <> "地上" And new_area <> "空中" And new_area <> "地中" Then
                    EventErrorMessage = "場所の種類が不正です"
                    Error 0
                End If
            Case "屋内"
                If new_area <> "地上" And new_area <> "空中" Then
                    EventErrorMessage = "場所の種類が不正です"
                    Error 0
                End If
            Case "月面"
                If new_area <> "地上" And new_area <> "宇宙" And new_area <> "地中" Then
                    EventErrorMessage = "場所の種類が不正です"
                    Error 0
                End If
            Case "水", "深水"
                If new_area <> "水中" And new_area <> "水上" And new_area <> "空中" Then
                    EventErrorMessage = "場所の種類が不正です"
                    Error 0
                End If
            Case "空中"
                If new_area <> "空中" Then
                    EventErrorMessage = "場所の種類が不正です"
                    Error 0
                End If
            Case "宇宙"
                If new_area <> "宇宙" Then
                    EventErrorMessage = "場所の種類が不正です"
                    Error 0
                End If
        End Select
        
        .Area = new_area
        .Update
        If .Status = "出撃" Then
            PaintUnitBitmap u
        End If
    End With
    RedrawScreen
    
    ExecChangeAreaCmd = LineNum + 1
End Function

'ADD START 240a
'ChangeLayerコマンド
'ChangeLayer X Y Name Number [Option]
Private Function ExecChangeLayerCmd() As Long
Dim X As Integer, Y As Integer
Dim lname As String, lid As Integer, lbitmap As Integer, ltypename As String, ltype As BoxTypes
Dim fname As String, fname1 As String, fname2 As String, fname3 As String
Dim basefname As String
Dim ret As Long, i As Integer
Dim isPaintBmp As Boolean

    If ArgNum <> 5 And ArgNum <> 6 Then
        EventErrorMessage = "ChangeTerrainコマンドの引数の数が違います"
        Error 0
    End If
    
    '対象座標を取得
    X = GetArgAsLong(2)
    Y = GetArgAsLong(3)
    If X < 1 Or X > MapWidth Then
        EventErrorMessage = "Ｘ座標の値は1～" & MapWidth & "で指定してください"
        Error 0
    End If
    If Y < 1 Or Y > MapHeight Then
        EventErrorMessage = "Ｙ座標の値は1～" & MapHeight & "で指定してください"
        Error 0
    End If
    
    'レイヤー情報・画像を取得
    lname = GetArgAsString(4)
    lbitmap = GetArgAsLong(5)
    If Right$(lname, 6) = "(ローカル)" Then
        lname = Left$(lname, Len(lname) - 6)
    End If
    With TDList
        For i = 1 To .Count
            lid = .OrderedID(i)
            If lname = .Name(lid) Then
                Exit For
            End If
        Next
        If i > .Count Then
            EventErrorMessage = "「" & lname & "」という地形は存在しません"
            Error 0
        End If
    End With
    MapData(X, Y, MapDataIndex.LayerType) = lid
    MapData(X, Y, MapDataIndex.LayerBitmapNo) = lbitmap
    
    'マス情報を取得
    isPaintBmp = True
    If ArgNum = 6 Then
        ltypename = GetArgAsString(6)
        If "通常" = ltypename Then
            ltype = BoxTypes.Upper
        ElseIf "情報限定" = ltypename Then
            isPaintBmp = False
            ltype = BoxTypes.UpperDataOnly
        ElseIf "画像限定" = ltypename Then
            ltype = BoxTypes.UpperBmpOnly
        Else
            EventErrorMessage = "ChangeLayerコマンドのOptionが不正です"
            Error 0
        End If
    Else
        ltype = BoxTypes.Upper
    End If
    MapData(X, Y, MapDataIndex.BoxType) = ltype
    
    If isPaintBmp Then
        'マップ画像を検索
        basefname = SearchTerrainImageFile(MapData(X, Y, MapDataIndex.TerrainType), MapData(X, Y, MapDataIndex.BitmapNo), X, Y)
        fname = SearchTerrainImageFile(lid, lbitmap, X, Y)
        
        If fname = "" Then
            EventErrorMessage = "マップビットマップ「" _
                & TDList.Bitmap(lid) & Format$(lbitmap) & ".bmp" & "」が見つかりません"
            Error 0
        End If
        
        With MainForm
            'マップ画像を背景へ書き込み
            .picTmp32(0) = LoadPicture(basefname)
            Select Case MapDrawMode
                Case "夜"
                    GetImage MainForm.picTmp32(0)
                    Dark
                    SetImage MainForm.picTmp32(0)
                Case "セピア"
                    GetImage MainForm.picTmp32(0)
                    Sepia
                    SetImage MainForm.picTmp32(0)
                Case "白黒"
                    GetImage MainForm.picTmp32(0)
                    Monotone
                    SetImage MainForm.picTmp32(0)
                Case "夕焼け"
                    GetImage MainForm.picTmp32(0)
                    Sunset
                    SetImage MainForm.picTmp32(0)
                Case "水中"
                    GetImage MainForm.picTmp32(0)
                    Water
                    SetImage MainForm.picTmp32(0)
                Case "フィルタ"
                    GetImage .picTmp32(0)
                    ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent
                    SetImage .picTmp32(0)
            End Select
            ret = BitBlt(.picBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
            
            'レイヤー画像を背景へ書き込み
            .picTmp32(0) = LoadPicture(fname)
            BGColor = vbWhite
            Select Case MapDrawMode
                Case "夜"
                    GetImage MainForm.picTmp32(0)
                    Dark True
                    SetImage MainForm.picTmp32(0)
                Case "セピア"
                    GetImage MainForm.picTmp32(0)
                    Sepia True
                    SetImage MainForm.picTmp32(0)
                Case "白黒"
                    GetImage MainForm.picTmp32(0)
                    Monotone True
                    SetImage MainForm.picTmp32(0)
                Case "夕焼け"
                    GetImage MainForm.picTmp32(0)
                    Sunset True
                    SetImage MainForm.picTmp32(0)
                Case "水中"
                    GetImage MainForm.picTmp32(0)
                    Water True
                    SetImage MainForm.picTmp32(0)
                Case "フィルタ"
                    GetImage .picTmp32(0)
                    ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent, True
                    SetImage .picTmp32(0)
            End Select
            'レイヤーは透過処理をする
            ret = TransparentBlt(.picBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, BGColor)
            
            'マス目の表示
            If ShowSquareLine Then
                .picBack.Line (32 * (X - 1), 32 * (Y - 1))-(32 * X, 32 * (Y - 1)), rgb(100, 100, 100), B
                .picBack.Line (32 * (X - 1), 32 * (Y - 1))-(32 * (X - 1), 32 * Y), rgb(100, 100, 100), B
            End If
            
            'マスク入り背景画面を作成しておく
            ret = BitBlt(.picMaskedBack.hDC, _
                32 * (X - 1), 32 * (Y - 1), 32, 32, _
                .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
            ret = BitBlt(.picMaskedBack.hDC, _
                32 * (X - 1), 32 * (Y - 1), 32, 32, _
                .picMask.hDC, 0, 0, SRCAND)
            ret = BitBlt(.picMaskedBack.hDC, _
                32 * (X - 1), 32 * (Y - 1), 32, 32, _
                .picMask2.hDC, 0, 0, SRCINVERT)
        End With
    
        '変更された地形にいたユニットを再表示（ついでにバックバッファからフロントに描画）
        If Not MapDataForUnit(X, Y) Is Nothing Then
            With MapDataForUnit(X, Y)
                Set MapDataForUnit(X, Y) = Nothing
                EraseUnitBitmap X, Y, False
                .StandBy X, Y, "非同期"
            End With
        Else
            With MainForm
                ret = TransparentBlt(.picMain(0).hDC, _
                    MapToPixelX(X), MapToPixelY(Y), 32, 32, _
                    .picTmp32(0).hDC, 0, 0, 32, 32, vbWhite)
            End With
        End If
    
    End If
    
    ExecChangeLayerCmd = LineNum + 1
End Function
'ADD  END  240a

Private Function ExecChangeMapCmd() As Long
Dim u As Unit
Dim fname As String
Dim late_refresh As Boolean
    
    Select Case ArgNum
        Case 2
            'ＯＫ
        Case 3
             If GetArgAsString(3) = "非同期" Then
                 late_refresh = True
             Else
                 EventErrorMessage = "ChangeMapコマンドのオプションが不正です"
                 Error 0
             End If
        Case Else
            EventErrorMessage = "ChangeMapコマンドの引数の数が違います"
            Error 0
    End Select
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    '出撃中のユニットを撤退させる
    For Each u In UList
        With u
            If .Status = "出撃" Or .Status = "格納" Then
                If late_refresh Then
                    .Escape "非同期"
                Else
                    .Escape
                End If
            End If
        End With
    Next
    
    fname = GetArgAsString(2)
    If Len(fname) > 0 Then
        LoadMapData ScenarioPath & fname
    Else
        LoadMapData ""
    End If
    If late_refresh Then
        SetupBackground "", "非同期"
        RedrawScreen True
    Else
        SetupBackground
        RedrawScreen
    End If
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ExecChangeMapCmd = LineNum + 1
End Function

Private Function ExecChangeModeCmd() As Long
Dim uarrary() As Unit
Dim u As Unit
Dim new_mode As String
Dim pname As String
Dim i As Integer
Dim dst_x As Integer, dst_y As Integer
    
    ReDim uarray(1)
    Select Case ArgNum
        Case 2
            Set uarray(1) = SelectedUnitForEvent
            new_mode = GetArgAsString(2)
        Case 3
            If GetArgAsLong(2) > 0 _
                And GetArgAsLong(3) > 0 _
            Then
                Set uarray(1) = SelectedUnitForEvent
                dst_x = GetArgAsLong(2)
                dst_y = GetArgAsLong(3)
                If dst_x < 1 Or MapWidth < dst_x _
                    Or dst_y < 1 Or MapHeight < dst_y _
                Then
                    EventErrorMessage = "ChangeModeコマンドの目的地の座標が不正です"
                    Error 0
                End If
                new_mode = Format$(dst_x) & " " & Format$(dst_y)
            Else
                pname = GetArgAsString(2)
                Select Case pname
                    Case "味方", "ＮＰＣ", "敵", "中立"
                        ReDim uarray(0)
                        For Each u In UList
                            If u.Party0 = pname Then
                                ReDim Preserve uarray(UBound(uarray) + 1)
                                Set uarray(UBound(uarray)) = u
                            End If
                        Next
                    Case Else
                        Set uarray(1) = UList.Item2(pname)
                        If uarray(1) Is Nothing Then
                            With PList
                                If Not .IsDefined(pname) Then
                                    EventErrorMessage = _
                                        "「" & pname & "」というパイロットが見つかりません"
                                    Error 0
                                End If
                                Set uarray(1) = .Item(pname).Unit
                                i = 2
                                Do While .IsDefined(pname & ":" & Format$(i))
                                    ReDim Preserve uarray(UBound(uarray) + 1)
                                    Set uarray(UBound(uarray)) = _
                                        .Item(pname & ":" & Format$(i)).Unit
                                    i = i + 1
                                Loop
                            End With
                        End If
                End Select
                new_mode = GetArgAsString(3)
            End If
        Case 4
            pname = GetArgAsString(2)
            Select Case pname
                Case "味方", "ＮＰＣ", "敵", "中立"
                    ReDim uarray(0)
                    For Each u In UList
                        If u.Party0 = pname Then
                            ReDim Preserve uarray(UBound(uarray) + 1)
                            Set uarray(UBound(uarray)) = u
                        End If
                    Next
                Case Else
                    Set uarray(1) = UList.Item2(pname)
                    If uarray(1) Is Nothing Then
                        With PList
                            If Not .IsDefined(pname) Then
                                EventErrorMessage = _
                                    "「" & pname & "」というパイロットが見つかりません"
                                Error 0
                            End If
                            Set uarray(1) = .Item(pname).Unit
                            i = 2
                            Do While .IsDefined(pname & ":" & Format$(i))
                                ReDim Preserve uarray(UBound(uarray) + 1)
                                Set uarray(UBound(uarray)) = _
                                    .Item(pname & ":" & Format$(i)).Unit
                                i = i + 1
                            Loop
                        End With
                    End If
            End Select
            dst_x = GetArgAsLong(3)
            dst_y = GetArgAsLong(4)
            If dst_x < 1 Or MapWidth < dst_x _
                Or dst_y < 1 Or MapHeight < dst_y _
            Then
                EventErrorMessage = "ChangeModeコマンドの目的地の座標が不正です"
                Error 0
            End If
            new_mode = Format$(dst_x) & " " & Format$(dst_y)
        Case Else
            EventErrorMessage = "ChangeModeコマンドの引数の数が違います"
            Error 0
    End Select
    
    For i = 1 To UBound(uarray)
        If Not uarray(i) Is Nothing Then
            uarray(i).Mode = new_mode
        End If
    Next
    
    ExecChangeModeCmd = LineNum + 1
End Function

Private Function ExecChangePartyCmd() As Long
Dim new_party As String
Dim pname As String
Dim u As Unit

    Select Case ArgNum
        Case 2
            new_party = GetArgAsString(2)
            If new_party <> "味方" And new_party <> "ＮＰＣ" _
                And new_party <> "敵" And new_party <> "中立" _
            Then
                EventErrorMessage = "陣営の指定が間違っています"
                Error 0
            End If
            
            SelectedUnitForEvent.ChangeParty new_party
        Case 3
            new_party = GetArgAsString(3)
            If new_party <> "味方" And new_party <> "ＮＰＣ" _
                And new_party <> "敵" And new_party <> "中立" _
            Then
                EventErrorMessage = "陣営の指定が間違っています"
                Error 0
            End If
                
            pname = GetArgAsString(2)
            Set u = UList.Item2(pname)
            If u Is Nothing Then
                If Not PList.IsDefined(pname) Then
                    EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
                    Error 0
                End If
                
                With PList.Item(pname)
                    If .Unit Is Nothing Then
                        .Party = new_party
                    Else
                        .Unit.ChangeParty new_party
                    End If
                End With
            Else
                u.ChangeParty new_party
            End If
        Case Else
            EventErrorMessage = "ChangePartyコマンドの引数の数が違います"
            Error 0
    End Select
    
    'カーソルが陣営変更されたユニット上にあるとカーソルは消去されるので
    IsCursorVisible = False
    
    ExecChangePartyCmd = LineNum + 1
End Function

Private Function ExecChangeTerrainCmd() As Long
Dim tx As Integer, ty As Integer
Dim tname As String, tid As Integer, tbitmap As Integer
Dim fname As String, fname1 As String, fname2 As String, fname3 As String
Dim ret As Long, i As Integer

    If ArgNum <> 5 Then
        EventErrorMessage = "ChangeTerrainコマンドの引数の数が違います"
        Error 0
    End If
    
    tx = GetArgAsLong(2)
    If tx < 1 Or tx > MapWidth Then
        EventErrorMessage = "Ｘ座標の値は1～" & MapWidth & "で指定してください"
        Error 0
    End If
    
    ty = GetArgAsLong(3)
    If ty < 1 Or ty > MapHeight Then
        EventErrorMessage = "Ｙ座標の値は1～" & MapHeight & "で指定してください"
        Error 0
    End If
    
    tname = GetArgAsString(4)
    If Right$(tname, 6) <> "(ローカル)" Then
        With TDList
            For i = 1 To .Count
                tid = .OrderedID(i)
                If tname = .Name(tid) Then
                    Exit For
                End If
            Next
            If i > .Count Then
                EventErrorMessage = "「" & tname & "」という地形は存在しません"
                Error 0
            End If
        End With
        
'MOD START 240a
'        MapData(tx, ty, 0) = tid
        MapData(tx, ty, MapDataIndex.TerrainType) = tid
'MOD  END  240a
        
        tbitmap = GetArgAsLong(5)
'MOD START 240a
'        MapData(tx, ty, 1) = tbitmap
        MapData(tx, ty, MapDataIndex.BitmapNo) = tbitmap
'MOD  END  240a
    Else
        tname = Left$(tname, Len(tname) - 6)
        
        With TDList
            For i = 1 To .Count
                tid = .OrderedID(i)
                If tname = .Name(tid) Then
                    Exit For
                End If
            Next
            If i > .Count Then
                EventErrorMessage = "「" & tname & "」という地形は存在しません"
                Error 0
            End If
        End With
        
'MOD START 240a
'        MapData(tx, ty, 0) = tid
        MapData(tx, ty, MapDataIndex.TerrainType) = tid
'MOD  END  240a
        
        tbitmap = -GetArgAsLong(5)
'MOD START 240a
'        MapData(tx, ty, 1) = tbitmap
        MapData(tx, ty, MapDataIndex.BitmapNo) = tbitmap
'MOD  END  240a
    End If
    
    'マップ画像を検索
    fname = SearchTerrainImageFile(tid, tbitmap, tx, ty)
    
    If fname = "" Then
        EventErrorMessage = "マップビットマップ「" _
            & TDList.Bitmap(tid) & Format$(tbitmap) & ".bmp" & "」が見つかりません"
        Error 0
    End If
    
    With MainForm
        .picTmp32(0) = LoadPicture(fname)
        
        Select Case MapDrawMode
            Case "夜"
                GetImage MainForm.picTmp32(0)
                Dark
                SetImage MainForm.picTmp32(0)
            Case "セピア"
                GetImage MainForm.picTmp32(0)
                Sepia
                SetImage MainForm.picTmp32(0)
            Case "白黒"
                GetImage MainForm.picTmp32(0)
                Monotone
                SetImage MainForm.picTmp32(0)
            Case "夕焼け"
                GetImage MainForm.picTmp32(0)
                Sunset
                SetImage MainForm.picTmp32(0)
            Case "水中"
                GetImage MainForm.picTmp32(0)
                Water
                SetImage MainForm.picTmp32(0)
            Case "フィルタ"
                GetImage .picTmp32(0)
                ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent
                SetImage .picTmp32(0)
        End Select
        
        '背景への書き込み
        ret = BitBlt(.picBack.hDC, _
            32 * (tx - 1), 32 * (ty - 1), 32, 32, _
            .picTmp32(0).hDC, 0, 0, SRCCOPY)
        
        'マス目の表示
        If ShowSquareLine Then
            .picBack.Line (32 * (tx - 1), 32 * (ty - 1))-(32 * tx, 32 * (ty - 1)), rgb(100, 100, 100), B
            .picBack.Line (32 * (tx - 1), 32 * (ty - 1))-(32 * (tx - 1), 32 * ty), rgb(100, 100, 100), B
        End If
        
        'マスク入り背景画面を作成
        ret = BitBlt(.picMaskedBack.hDC, _
            32 * (tx - 1), 32 * (ty - 1), 32, 32, _
            .picBack.hDC, 32 * (tx - 1), 32 * (ty - 1), SRCCOPY)
        ret = BitBlt(.picMaskedBack.hDC, _
            32 * (tx - 1), 32 * (ty - 1), 32, 32, _
            .picMask.hDC, 0, 0, SRCAND)
        ret = BitBlt(.picMaskedBack.hDC, _
            32 * (tx - 1), 32 * (ty - 1), 32, 32, _
            .picMask2.hDC, 0, 0, SRCINVERT)
    End With
    
    '変更された地形にいたユニットを再表示
    If Not MapDataForUnit(tx, ty) Is Nothing Then
        With MapDataForUnit(tx, ty)
            Set MapDataForUnit(tx, ty) = Nothing
            EraseUnitBitmap tx, ty, False
            .StandBy tx, ty, "非同期"
        End With
    Else
        With MainForm
            ret = BitBlt(.picMain(0).hDC, _
                MapToPixelX(tx), MapToPixelY(ty), 32, 32, _
                .picTmp32(0).hDC, 0, 0, SRCCOPY)
        End With
    End If
    
    ExecChangeTerrainCmd = LineNum + 1
End Function

Private Function ExecChangeUnitBitmapCmd() As Long
Dim new_bmp As String, prev_bmp As String
Dim u As Unit

    Select Case ArgNum
        Case 2
            Set u = SelectedUnitForEvent
            new_bmp = GetArgAsString(2)
        Case 3
            Set u = GetArgAsUnit(2)
            new_bmp = GetArgAsString(3)
        Case Else
            EventErrorMessage = "ChangeUnitBitmapコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        prev_bmp = .Bitmap
        If LCase$(Right$(new_bmp, 4)) = ".bmp" Then
            .AddCondition "ユニット画像", -1, 0, "非表示 " & new_bmp
        ElseIf new_bmp = "-" Then
            If .IsConditionSatisfied("ユニット画像") Then
                .DeleteCondition "ユニット画像"
            End If
        ElseIf new_bmp = "非表示" Then
            .AddCondition "非表示付加", -1, 0, "非表示"
            .BitmapID = -1
            EraseUnitBitmap .X, .Y, False
        ElseIf new_bmp = "非表示解除" Then
            If .IsConditionSatisfied("非表示付加") Then
                .DeleteCondition "非表示付加"
            End If
            .BitmapID = MakeUnitBitmap(u)
        Else
            EventErrorMessage = "ビットマップファイル名が不正です"
            Error 0
        End If
        
        If .Bitmap <> prev_bmp Then
            .BitmapID = MakeUnitBitmap(u)
        End If
        PaintUnitBitmap u, "リフレッシュ無し"
    End With
    
    ExecChangeUnitBitmapCmd = LineNum + 1
End Function

Private Function ExecChargeCmd() As Long
Dim u As Unit

    Select Case ArgNum
        Case 2
            Set u = GetArgAsUnit(2)
        Case 1
            Set u = SelectedUnitForEvent
        Case Else
            EventErrorMessage = "Chargeコマンドの引数の数が違います"
            Error 0
    End Select
    
    u.AddCondition "チャージ", 1
    
    ExecChargeCmd = LineNum + 1
End Function

Private Function ExecCircleCmd() As Long
Dim pic As PictureBox, pic2 As PictureBox
Dim x1 As Integer, y1 As Integer, rad As Integer
Dim opt As String
Dim cname As String, clr As Long
Dim i As Integer

    If ArgNum < 4 Then
        EventErrorMessage = "Circleコマンドの引数の数が違います"
        Error 0
    End If
    
    x1 = GetArgAsLong(2) + BaseX
    y1 = GetArgAsLong(3) + BaseY
    rad = GetArgAsLong(4)
    
    SaveScreen
    
    '描画先
    Select Case ObjDrawOption
        Case "背景"
            Set pic = MainForm.picBack
            Set pic2 = MainForm.picMaskedBack
            IsMapDirty = True
        Case "保持"
            Set pic = MainForm.picMain(0)
            Set pic2 = MainForm.picMain(1)
        Case Else
            Set pic = MainForm.picMain(0)
    End Select
    
    '描画領域
    If ObjDrawOption <> "背景" Then
        Dim tmp As Integer
        IsPictureVisible = True
        tmp = rad + ObjDrawWidth - 1
        PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(x1 - tmp, 0))
        PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(y1 - tmp, 0))
        PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(x1 + tmp, MapPWidth - 1))
        PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(y1 + tmp, MapPHeight - 1))
   End If
   
    clr = ObjColor
    For i = 5 To ArgNum
        opt = GetArgAsString(i)
        If Asc(opt) = 35 Then '#
            If Len(opt) <> 7 Then
                EventErrorMessage = "色指定が不正です"
                Error 0
            End If
            cname = String$(8, vbNullChar)
            Mid(cname, 1, 2) = "&H"
            Mid(cname, 3, 2) = Mid$(opt, 6, 2)
            Mid(cname, 5, 2) = Mid$(opt, 4, 2)
            Mid(cname, 7, 2) = Mid$(opt, 2, 2)
            If Not IsNumeric(cname) Then
                EventErrorMessage = "色指定が不正です"
                Error 0
            End If
            clr = CLng(cname)
        Else
            EventErrorMessage = _
                "Circleコマンドに不正なオプション「" & opt & "」が使われています"
            Error 0
        End If
    Next
    
    With pic
        .DrawWidth = ObjDrawWidth
        .FillColor = ObjFillColor
        .FillStyle = ObjFillStyle
    End With
    
    pic.Circle (x1, y1), rad, clr
    
    With pic
        .DrawWidth = 1
        .FillColor = vbWhite
        .FillStyle = vbFSTransparent
    End With
    
    If Not pic2 Is Nothing Then
        With pic2
            .DrawWidth = ObjDrawWidth
            .FillColor = ObjFillColor
            .FillStyle = ObjFillStyle
        End With
        
        pic2.Circle (x1, y1), rad, clr
        
        With pic2
            .DrawWidth = 1
            .FillColor = vbWhite
            .FillStyle = vbFSTransparent
        End With
    End If
    
    ExecCircleCmd = LineNum + 1
End Function

Private Function ExecClearEventCmd() As Long
Dim ret As Long

    Select Case ArgNum
        Case 2
            ret = FindLabel(GetArgAsString(2))
            If ret > 0 Then
                ClearLabel ret
            End If
        Case 1
            If CurrentLabel > 0 Then
                ClearLabel CurrentLabel
            End If
        Case Else
            EventErrorMessage = "ClearEventコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExecClearEventCmd = LineNum + 1
End Function

'互換性維持のために残している
Private Function ExecClearImageCmd() As Long
    ClearPicture
    MainForm.picMain(0).Refresh
    ExecClearImageCmd = LineNum + 1
End Function

'ADD START 240a
'ExecClearLayerCmd
'書式１ 全てのLayer情報を削除
' ClearLayer [Option]
'書式２ 指定した座標のLayer情報を削除 情報限定・画像限定を選択可能
' ClearLayer X Y [Option]
'このモジュールにおいては、 DataOnly＝データのみ消す の意
Private Function ExecClearLayerCmd() As Long
Dim X As Integer, Y As Integer, i As Integer, j As Integer
Dim isAllClear As Boolean, isDataOnly As Boolean, isBitmapOnly As Boolean
Dim fname As String, loption As String
Dim ret As Long
    '引数チェック
    If 4 < ArgNum Then
        EventErrorMessage = "ClearLayerコマンドの引数の数が違います"
        Error 0
    End If
    '全体クリアフラグ取得
    If ArgNum < 3 Then
        isAllClear = True
    Else
        isAllClear = False
    End If
    'オプション取得
    isDataOnly = False
    isBitmapOnly = False
    loption = ""
    If 2 = ArgNum Then
        loption = GetArgAsString(2)
    ElseIf 4 = ArgNum Then
        loption = GetArgAsString(4)
    End If
    If loption <> "" Then
        If "情報限定" = loption Then
            isDataOnly = True
        ElseIf "画像限定" = loption Then
            isBitmapOnly = True
        ElseIf "通常" <> loption Then
            EventErrorMessage = "ClearLayerコマンドの引数Optionが不正です"
            Error 0
        End If
    End If
    '座標取得
    If Not isAllClear Then
        X = GetArgAsLong(2)
        Y = GetArgAsLong(3)
        If X < 1 Or X > MapWidth Then
            EventErrorMessage = "Ｘ座標の値は1～" & MapWidth & "で指定してください"
            Error 0
        End If
        If Y < 1 Or Y > MapHeight Then
            EventErrorMessage = "Ｙ座標の値は1～" & MapHeight & "で指定してください"
            Error 0
        End If
    End If
    '処理開始
    If isAllClear Then
        '全削除実行
        For i = 1 To MapWidth
            For j = 1 To MapHeight
                'レイヤー情報を更新する
                If isDataOnly Then
                    MapData(i, j, MapDataIndex.BoxType) = BoxTypes.UpperBmpOnly
                ElseIf isBitmapOnly Then
                    MapData(i, j, MapDataIndex.BoxType) = BoxTypes.UpperDataOnly
                Else
                    '両方ともfalseならレイヤー丸ごと削除
                    MapData(i, j, MapDataIndex.LayerType) = NO_LAYER_NUM
                    MapData(i, j, MapDataIndex.LayerBitmapNo) = NO_LAYER_NUM
                    MapData(i, j, MapDataIndex.BoxType) = BoxTypes.Under
                End If
                'レイヤー画像だけを消すことはできないので、下層レイヤーを再描画することで処理する
                fname = SearchTerrainImageFile(MapData(i, j, MapDataIndex.TerrainType), MapData(i, j, MapDataIndex.BitmapNo), i, j)

                'データのみ削除の場合は再描画処理をスキップする
                If Not isDataOnly Then
                    With MainForm
                        'マップ画像を背景へ書き込み
                        .picTmp32(0) = LoadPicture(fname)
                        Select Case MapDrawMode
                            Case "夜"
                                GetImage MainForm.picTmp32(0)
                                Dark
                                SetImage MainForm.picTmp32(0)
                            Case "セピア"
                                GetImage MainForm.picTmp32(0)
                                Sepia
                                SetImage MainForm.picTmp32(0)
                            Case "白黒"
                                GetImage MainForm.picTmp32(0)
                                Monotone
                                SetImage MainForm.picTmp32(0)
                            Case "夕焼け"
                                GetImage MainForm.picTmp32(0)
                                Sunset
                                SetImage MainForm.picTmp32(0)
                            Case "水中"
                                GetImage MainForm.picTmp32(0)
                                Water
                                SetImage MainForm.picTmp32(0)
                            Case "フィルタ"
                                GetImage .picTmp32(0)
                                ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent
                                SetImage .picTmp32(0)
                        End Select
                        ret = BitBlt(.picBack.hDC, 32 * (i - 1), 32 * (j - 1), 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
                        'マス目の表示
                        If ShowSquareLine Then
                            .picBack.Line (32 * (i - 1), 32 * (j - 1))-(32 * i, 32 * (j - 1)), rgb(100, 100, 100), B
                            .picBack.Line (32 * (i - 1), 32 * (j - 1))-(32 * (i - 1), 32 * j), rgb(100, 100, 100), B
                        End If
                        'マスク入り背景画面を作成
                        ret = BitBlt(.picMaskedBack.hDC, 32 * (i - 1), 32 * (j - 1), 32, 32, .picBack.hDC, 32 * (i - 1), 32 * (j - 1), SRCCOPY)
                        ret = BitBlt(.picMaskedBack.hDC, 32 * (i - 1), 32 * (j - 1), 32, 32, .picMask.hDC, 0, 0, SRCAND)
                        ret = BitBlt(.picMaskedBack.hDC, 32 * (i - 1), 32 * (j - 1), 32, 32, .picMask2.hDC, 0, 0, SRCINVERT)
                    End With
                    '変更された地形にいたユニットを再表示
                    If Not MapDataForUnit(i, j) Is Nothing Then
                        '一旦ユニットをどかして再配置（変更後の地形が入れない地形の場合に対処）
                        Set MapDataForUnit(i, j) = Nothing
                        EraseUnitBitmap i, j, False
                        MapDataForUnit(i, j).StandBy i, j, "非同期"
                    Else
                        With MainForm
                            ret = TransparentBlt(.picMain(0).hDC, MapToPixelX(i), MapToPixelY(j), 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, vbWhite)
                        End With
                    End If
                End If
            Next
        Next
    Else
        '指定した座標のみレイヤー情報を更新する
        If isDataOnly Then
            MapData(X, Y, MapDataIndex.BoxType) = BoxTypes.UpperBmpOnly
        ElseIf isBitmapOnly Then
            MapData(X, Y, MapDataIndex.BoxType) = BoxTypes.UpperDataOnly
        Else
            '両方ともfalseならレイヤー丸ごと削除
            MapData(X, Y, MapDataIndex.LayerType) = NO_LAYER_NUM
            MapData(X, Y, MapDataIndex.LayerBitmapNo) = NO_LAYER_NUM
            MapData(X, Y, MapDataIndex.BoxType) = BoxTypes.Under
        End If
    
        'データのみの場合は再描画処理をスキップ
        If Not isDataOnly Then
            fname = SearchTerrainImageFile(MapData(X, Y, MapDataIndex.TerrainType), MapData(X, Y, MapDataIndex.BitmapNo), X, Y)
            With MainForm
                'マップ画像を背景へ書き込み
                .picTmp32(0) = LoadPicture(fname)
                Select Case MapDrawMode
                    Case "夜"
                        GetImage MainForm.picTmp32(0)
                        Dark
                        SetImage MainForm.picTmp32(0)
                    Case "セピア"
                        GetImage MainForm.picTmp32(0)
                        Sepia
                        SetImage MainForm.picTmp32(0)
                    Case "白黒"
                        GetImage MainForm.picTmp32(0)
                        Monotone
                        SetImage MainForm.picTmp32(0)
                    Case "夕焼け"
                        GetImage MainForm.picTmp32(0)
                        Sunset
                        SetImage MainForm.picTmp32(0)
                    Case "水中"
                        GetImage MainForm.picTmp32(0)
                        Water
                        SetImage MainForm.picTmp32(0)
                    Case "フィルタ"
                        GetImage .picTmp32(0)
                        ColorFilter MapDrawFilterColor, MapDrawFilterTransPercent
                        SetImage .picTmp32(0)
                End Select
                ret = BitBlt(.picBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
                'マス目の表示
                If ShowSquareLine Then
                    .picBack.Line (32 * (X - 1), 32 * (Y - 1))-(32 * X, 32 * (Y - 1)), rgb(100, 100, 100), B
                    .picBack.Line (32 * (X - 1), 32 * (Y - 1))-(32 * (X - 1), 32 * Y), rgb(100, 100, 100), B
                End If
                'マスク入り背景画面を作成
                ret = BitBlt(.picMaskedBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
                ret = BitBlt(.picMaskedBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picMask.hDC, 0, 0, SRCAND)
                ret = BitBlt(.picMaskedBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picMask2.hDC, 0, 0, SRCINVERT)
            End With
            '変更された地形にいたユニットを再表示
            If Not MapDataForUnit(X, Y) Is Nothing Then
                Set MapDataForUnit(X, Y) = Nothing
                EraseUnitBitmap X, Y, False
                MapDataForUnit(X, Y).StandBy X, Y, "非同期"
            Else
                With MainForm
                    ret = TransparentBlt(.picMain(0).hDC, MapToPixelX(X), MapToPixelY(Y), 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, vbWhite)
                End With
            End If
        End If
    End If
    
    ExecClearLayerCmd = LineNum + 1

End Function
'ADD  END  240a

Private Function ExecClearObjCmd() As Long
Dim i As Integer, j As Integer, n As Integer
Dim oname As String
Dim without_refresh As Boolean

    n = ArgNum
    If n > 1 Then
        If GetArgAsString(n) = "非同期" Then
            n = n - 1
            without_refresh = True
        End If
    End If
    
    Select Case n
        Case 2
            oname = GetArgAsString(2)
            For i = 1 To UBound(HotPointList)
                If HotPointList(i).Name = oname Then
                    Exit For
                End If
            Next
            If i <= UBound(HotPointList) Then
                With HotPointList(i)
                    If frmToolTip.Visible _
                        And SelectedAlternative = .Name _
                    Then
                        'ツールチップを消す
                        frmToolTip.Hide
                        'マウスカーソルを元に戻す
                        MainForm.picMain(0).MousePointer = 0
                    End If
                End With
                For j = i To UBound(HotPointList) - 1
                    HotPointList(j) = HotPointList(j + 1)
                Next
                ReDim Preserve HotPointList(UBound(HotPointList) - 1)
            End If
        Case 1
            ReDim HotPointList(0)
        Case Else
            EventErrorMessage = "ClearObjコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExecClearObjCmd = LineNum + 1
    
    'まだマウスカーソルがホットポイント上にあるか？
    For i = 1 To UBound(HotPointList)
        With HotPointList(i)
            If .Left <= MouseX And MouseX < .Left + .width _
                And .Top <= MouseY And MouseY < .Top + .Height _
            Then
                Exit Function
            End If
        End With
    Next
    
    'ツールチップを消す
    frmToolTip.Hide
    If Not without_refresh Then
        MainForm.picMain(0).Refresh
    End If
    
    'マウスカーソルを元に戻す
    MainForm.picMain(0).MousePointer = 0
End Function

Private Function ExecClearPictureCmd() As Long
    Select Case ArgNum
        Case 1
            ClearPicture
        Case 5
            ClearPicture2 _
                GetArgAsLong(2) + BaseX, GetArgAsLong(3) + BaseY, _
                GetArgAsLong(4) + BaseX, GetArgAsLong(5) + BaseY
        Case Else
            EventErrorMessage = "ClearPictureコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExecClearPictureCmd = LineNum + 1
End Function

Private Function ExecClearSkillCmd() As Long
Dim pname As String
Dim sname As String, sname2 As String, slist As String, buf As String
Dim sarray() As String
Dim vname As String, vname2 As String
Dim i As Integer, j As Integer

    pname = GetArgAsString(2)
    If PList.IsDefined(pname) Then
        pname = PList.Item(pname).ID
    ElseIf PDList.IsDefined(pname) Then
        pname = PDList.Item(pname).Name
    Else
        EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
        Error 0
    End If
    
    sname = GetArgAsString(3)
    
    'エリアスが定義されている？
    If ALDList.IsDefined(sname) Then
        With ALDList.Item(sname)
            ReDim sarray(.Count)
            For i = 1 To .Count
                sarray(i) = .AliasType(i)
            Next
        End With
    Else
        ReDim sarray(1)
        sarray(1) = sname
    End If
    
    For i = 1 To UBound(sarray)
        sname = sarray(i)
        sname2 = ""
        
        vname = "Ability(" & pname & "," & sname & ")"
        
        If LLength(GetValueAsString(vname)) >= 2 Then
            '必要技能用変数を削除
            sname2 = LIndex(GetValueAsString(vname), 2)
            vname2 = "Ability(" & pname & "," & sname2 & ")"
            UndefineVariable vname2
        End If
        
        'レベル設定用変数を削除
        UndefineVariable vname
        
        '特殊能力一覧作成用変数を削除
        vname = "Ability(" & pname & ")"
        If IsGlobalVariableDefined(vname) Then
            buf = GetValueAsString(vname)
            slist = ""
            For j = 1 To LLength(buf)
                If LIndex(buf, j) <> sname _
                    And LIndex(buf, j) <> sname2 _
                Then
                    slist = slist & " " & LIndex(buf, j)
                End If
            Next
            If LLength(slist) > 0 Then
                slist = Trim$(slist)
                SetVariableAsString vname, slist
            Else
                UndefineVariable vname
            End If
        End If
    Next
    
    'パイロットやユニットのステータスをアップデート
    If PList.IsDefined(pname) Then
        With PList.Item(pname)
            .Update
            If Not .Unit Is Nothing Then
                .Unit.Update
                If .Unit.Status = "出撃" Then
                    PList.UpdateSupportMod .Unit
                End If
            End If
        End With
    End If
    
    ExecClearSkillCmd = LineNum + 1
End Function

Private Function ExecClearSpecialPowerCmd() As Long
Dim sname As String
Dim u As Unit

    Select Case ArgNum
        Case 3
            Set u = GetArgAsUnit(2)
            sname = GetArgAsString(3)
        Case 2
            Set u = SelectedUnitForEvent
            sname = GetArgAsString(2)
        Case Else
            EventErrorMessage = "ClearSpecialPowerコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        If .IsSpecialPowerInEffect(sname) Then
            .RemoveSpecialPowerInEffect2 sname
        End If
    End With
    
    ExecClearSpecialPowerCmd = LineNum + 1
End Function

Private Function ExecClearStatusCmd() As Long
Dim sname As String
Dim u As Unit

    Select Case ArgNum
        Case 3
            Set u = GetArgAsUnit(2)
            sname = GetArgAsString(3)
        Case 2
            Set u = SelectedUnitForEvent
            sname = GetArgAsString(2)
        Case Else
            EventErrorMessage = "ClearStatusコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        If .IsConditionSatisfied(sname) Then
            .DeleteCondition sname
            .Update
            If .Status = "出撃" Then
                PaintUnitBitmap u
            End If
        End If
    End With
    
    ExecClearStatusCmd = LineNum + 1
End Function

Private Function ExecCloseCmd() As Long
    If ArgNum <> 2 Then
        EventErrorMessage = "Closeコマンドの引数の数が違います"
        Error 0
    End If
    
    Close #GetArgAsLong(2)
    
    ExecCloseCmd = LineNum + 1
End Function

Private Function ExecClsCmd() As Long
Dim cname As String, buf As String
Dim ret As Long

    Select Case ArgNum
        Case 2
            buf = GetArgAsString(2)
            If Asc(buf) <> 35 Or Len(buf) <> 7 Then
                EventErrorMessage = "色指定が不正です"
                Error 0
            End If
            cname = String$(8, vbNullChar)
            Mid(cname, 1, 2) = "&H"
            Mid(cname, 3, 2) = Mid$(buf, 6, 2)
            Mid(cname, 5, 2) = Mid$(buf, 4, 2)
            Mid(cname, 7, 2) = Mid$(buf, 2, 2)
            If Not IsNumeric(cname) Then
                EventErrorMessage = "色指定が不正です"
                Error 0
            End If
            MainForm.picMain(0).Line (0, 0)-(MainPWidth - 1, MainPHeight - 1), CLng(cname), BF
            MainForm.picMain(1).Line (0, 0)-(MainPWidth - 1, MainPHeight - 1), CLng(cname), BF
            ScreenIsSaved = True
        Case 1
            With MainForm
                ret = PatBlt(.picMain(0).hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
                ret = PatBlt(.picMain(1).hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
            End With
            ScreenIsSaved = True
        Case Else
            EventErrorMessage = "Clsコマンドの引数の数が違います"
            Error 0
    End Select
    
    IsPictureVisible = True
    IsCursorVisible = False
    
    PaintedAreaX1 = MainPWidth
    PaintedAreaY1 = MainPHeight
    PaintedAreaX2 = -1
    PaintedAreaY2 = -1
    
    ExecClsCmd = LineNum + 1
End Function

Private Function ExecColorCmd() As Long
Dim opt As String, cname As String

    If ArgNum <> 2 Then
        EventErrorMessage = "Colorコマンドの引数の数が違います"
        Error 0
    End If
    
    opt = GetArgAsString(2)
    If Asc(opt) <> 35 Or Len(opt) <> 7 Then
        EventErrorMessage = "色指定が不正です"
        Error 0
    End If
    cname = String$(8, vbNullChar)
    Mid(cname, 1, 2) = "&H"
    Mid(cname, 3, 2) = Mid$(opt, 6, 2)
    Mid(cname, 5, 2) = Mid$(opt, 4, 2)
    Mid(cname, 7, 2) = Mid$(opt, 2, 2)
    If Not IsNumeric(cname) Then
        EventErrorMessage = "色指定が不正です"
        Error 0
    End If
    
    ObjColor = CLng(cname)
    
    ExecColorCmd = LineNum + 1
End Function

Private Function ExecColorFilterCmd() As Long
Dim prev_x As Integer, prev_y As Integer
Dim u As Unit
Dim late_refresh As Boolean
Dim buf As String, fcolor As Long
Dim i As Integer, trans_par As Double

    If ArgNum < 2 Then
        EventErrorMessage = "ColorFilterコマンドの引数の数が違います"
        Error 0
    End If
    
    late_refresh = False
    MapDrawIsMapOnly = False
    trans_par = 0.5
    For i = 3 To ArgNum
        buf = GetArgAsString(i)
        Select Case buf
            Case "非同期"
                late_refresh = True
            Case "マップ限定"
                MapDrawIsMapOnly = True
            Case Else
                If Right(buf, 1) = "%" And IsNumeric(Left(buf, Len(buf) - 1)) Then
                    trans_par = MaxDbl(0, MinDbl(1, CDbl(Left(buf, Len(buf) - 1)) / 100))
                Else
                    EventErrorMessage = _
                        "ColorFilterコマンドに不正なオプション「" & buf & "」が使われています"
                    Error 0
                End If
        End Select
    Next
    
    buf = GetArgAsString(2)
    buf = "&H" & Mid$(buf, 6, 2) & Mid$(buf, 4, 2) & Mid$(buf, 2, 2)
    If IsNumeric(buf) Then
        fcolor = CLng(buf)
    Else
        EventErrorMessage = "ColorFilterコマンドのカラー指定が不正です"
        Error 0
    End If
    
    prev_x = MapX
    prev_y = MapY
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    SetupBackground "フィルタ", "非同期", fcolor, trans_par
    
    For Each u In UList
        With u
            If .Status = "出撃" Then
                If .BitmapID = 0 Then
                    With UList.Item(.Name)
                        If u.Party0 = .Party0 And .BitmapID <> 0 _
                            And u.Bitmap = .Bitmap _
                            And Not .IsFeatureAvailable("ダミーユニット") _
                        Then
                            u.BitmapID = .BitmapID
                        Else
                            u.BitmapID = MakeUnitBitmap(u)
                        End If
                    End With
                End If
            End If
        End With
    Next
    
    Center prev_x, prev_y
    RedrawScreen late_refresh
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ExecColorFilterCmd = LineNum + 1
End Function

Private Function ExecCombineCmd() As Long
Dim u As Unit
Dim uname As String
Dim anum As Integer

    Select Case ArgNum
        Case 2
            Set u = SelectedUnitForEvent
            uname = GetArgAsString(2)
        Case 3
            Set u = GetArgAsUnit(2)
            uname = GetArgAsString(3)
        Case Else
            EventErrorMessage = "Combineコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not UList.IsDefined(uname) Then
        EventErrorMessage = "「" & uname & "」というユニットが見つかりません"
        Error 0
    End If
    
    If u.CurrentForm.ID <> UList.Item(uname).CurrentForm.ID Then
        With u
            anum = .UsedAction
            .Combine uname, True
            If Not SelectedUnit Is Nothing Then
                If .ID = SelectedUnit.ID Then
                    Set SelectedUnit = UList.Item(uname)
                End If
            End If
            If Not SelectedUnitForEvent Is Nothing Then
                If .ID = SelectedUnitForEvent.ID Then
                    Set SelectedUnitForEvent = UList.Item(uname)
                End If
            End If
            If Not SelectedTarget Is Nothing Then
                If .ID = SelectedTarget.ID Then
                    Set SelectedTarget = UList.Item(uname)
                End If
            End If
            If Not SelectedTargetForEvent Is Nothing Then
                If .ID = SelectedTargetForEvent.ID Then
                    Set SelectedTargetForEvent = UList.Item(uname)
                End If
            End If
        End With
        
        With UList.Item(uname)
            .UsedAction = anum
            If .Status = "出撃" Then
                RedrawScreen
            End If
        End With
    End If
    
    ExecCombineCmd = LineNum + 1
End Function

Private Function ExecConfirmCmd() As Long
Dim ret As Integer
    
    If ArgNum <> 2 Then
        EventErrorMessage = "Confirmコマンドの引数の数が違います"
        Error 0
    End If
    
    '一度イベントを解消しておかないとMsgBoxを連続で使用したときに
    '動作がおかしくなる（ＶＢのバグ？）
    DoEvents
    
    ret = MsgBox(GetArgAsString(2), _
            vbOKCancel + vbQuestion, "選択")
    If ret = 1 Then
        SelectedAlternative = 1
    Else
        SelectedAlternative = 0
    End If
    
    ExecConfirmCmd = LineNum + 1
End Function

Private Function ExecContinueCmd() As Long
Dim msg As String
Dim n As Integer, i As Integer
Dim p As Pilot
Dim plevel As Integer
Dim u As Unit

    Select Case ArgNum
        Case 2
            If Not IsGlobalVariableDefined("次ステージ") Then
                DefineGlobalVariable "次ステージ"
            End If
            SetVariableAsString "次ステージ", GetArgAsString(2)
        Case 1
        Case Else
            EventErrorMessage = "Continueコマンドの引数の数が違います"
            Error 0
    End Select
    
    ClearUnitStatus
    
    '追加経験値を得るパイロットや破壊されたユニットがいなければ処理をスキップ
    n = 0
    For Each u In UList
        With u
            If .Party0 = "味方" Then
                If .Status = "出撃" Or .Status = "格納" Or .Status = "破壊" Then
                    n = 1
                    Exit For
                End If
            End If
        End With
    Next
    If n = 0 Then
        Turn = 0
    End If
    
    '追加経験値を収得
    If Turn > 0 And Not IsOptionDefined("追加経験値無効") Then
        OpenMessageForm
        
        n = 0
        msg = ""
        For Each p In PList
            With p
                If .Party <> "味方" Then
                    GoTo NextPilot
                End If
                
                If .MaxSP = 0 Then
                    GoTo NextPilot
                End If
                
                If .Unit Is Nothing Then
                    GoTo NextPilot
                End If
                
                If .Unit.Status <> "出撃" _
                    And .Unit.Status <> "格納" _
                Then
                    GoTo NextPilot
                End If
                
                plevel = .Level
                .Exp = .Exp + 2 * .SP
                
                '追加パイロットや暴走時パイロットに関する処理
                If .Unit.CountPilot > 0 And Not .IsSupport(.Unit) Then
                    '追加パイロットがメインパイロットの場合
                    If p Is .Unit.Pilot(1) _
                        And Not p Is .Unit.MainPilot _
                        And .Unit.MainPilot.MaxSP > 0 _
                    Then
                        GoTo NextPilot
                    End If
                    
                    '追加パイロットがメインパイロットではなくなった場合
                    If Not p Is .Unit.MainPilot Then
                        '自分がユニットのパイロット一覧に含まれているか判定
                        For i = 1 To .Unit.CountPilot
                            If p Is .Unit.Pilot(i) Then
                                Exit For
                            End If
                        Next
                        If i > .Unit.CountPilot Then
                            GoTo NextPilot
                        End If
                    End If
                End If
                
                If plevel = .Level Then
                    msg = msg & ";" & .Nickname & " 経験値 +" & Format$(2 * .SP)
                Else
                    msg = msg & ";" & .Nickname & " 経験値 +" & Format$(2 * .SP) _
                        & " レベルアップ！（Lv" & Format$(.Level) & "）"
                End If
                n = n + 1
                If n = 4 Then
                    DisplayMessage "システム", Mid$(msg, 2)
                    msg = ""
                    n = 0
                End If
            End With
NextPilot:
        Next
        If n > 0 Then
            DisplayMessage "システム", Mid$(msg, 2)
        End If
        
        CloseMessageForm
    End If
    
    MainForm.Hide
    
    'エピローグイベントを実行
    If IsEventDefined("エピローグ") Then
        'ハイパーモードや変身、能力コピーを解除
        For Each u In UList
            With u
                If .Status <> "他形態" _
                    And .Status <> "旧主形態" _
                    And .Status <> "旧形態" _
                Then
                    If .IsFeatureAvailable("ノーマルモード") Then
                        .Transform LIndex(.FeatureData("ノーマルモード"), 1)
                    End If
                End If
            End With
        Next
        
        If IsEventDefined("エピローグ", True) Then
            StopBGM
            StartBGM BGMName("Briefing")
        End If
        
        Stage = "エピローグ"
        HandleEvent "エピローグ"
    End If
    
    MainForm.Hide
    
    'インターミッションに移行
    If Not IsSubStage Then
        '
        InterMissionCommand
        
        If Not IsSubStage Then
            If GetValueAsString("次ステージ") = "" Then
                EventErrorMessage = "次のステージのファイル名が設定されていません"
                Error 0
            End If
            
            StartScenario GetValueAsString("次ステージ")
        Else
            IsSubStage = False
        End If
    End If
    
    IsScenarioFinished = True
    
    ExecContinueCmd = 0
End Function

Private Function ExecCopyArrayCmd() As Long
Dim i As Long, j As Integer
Dim buf As String
Dim var As VarData
Dim name1 As String, name2 As String

    If ArgNum <> 3 Then
        EventErrorMessage = "CopyArrayコマンドの引数の数が違います"
        Error 0
    End If
    
    'コピー元の変数名
    name1 = GetArg(2)
    If Left$(name1, 1) = "$" Then
        name1 = Mid$(name1, 2)
    End If
    'Eval関数
    If LCase$(Left$(name1, 5)) = "eval(" Then
        If Right$(name1, 1) = ")" Then
            name1 = Mid$(name1, 6, Len(name1) - 6)
            name1 = GetValueAsString(name1)
        End If
    End If
    
    'コピー先の変数名
    name2 = GetArg(3)
    If Left$(name2, 1) = "$" Then
        name1 = Mid$(name2, 2)
    End If
    'Eval関数
    If LCase$(Left$(name2, 5)) = "eval(" Then
        If Right$(name2, 1) = ")" Then
            name2 = Mid$(name2, 6, Len(name2) - 6)
            name2 = GetValueAsString(name2)
        End If
    End If
    
    'コピー先の変数を初期化
    'サブルーチンローカル変数の場合
    If IsSubLocalVariableDefined(name2) Then
        UndefineVariable name2
        VarIndex = VarIndex + 1
        With VarStack(VarIndex)
            .Name = name2
            .VariableType = StringType
            .StringValue = ""
        End With
    'ローカル変数の場合
    ElseIf IsLocalVariableDefined(name2) Then
        UndefineVariable name2
        DefineLocalVariable name2
    'グローバル変数の場合
    ElseIf IsGlobalVariableDefined(name2) Then
        UndefineVariable name2
        DefineGlobalVariable name2
    End If
    
    '配列を検索し、配列要素を見つける
    buf = ""
    If IsSubLocalVariableDefined(name1) Then
        'サブルーチンローカルな配列に対するCopyArray
        For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
            With VarStack(i)
                If InStr(.Name, name1 & "[") = 1 Then
                    buf = name2 & Mid$(.Name, InStr(.Name, "["))
                    SetVariable buf, .VariableType, .StringValue, .NumericValue
                End If
            End With
        Next
        If buf = "" Then
            Set var = GetVariableObject(name1)
            With var
                SetVariable name2, .VariableType, .StringValue, .NumericValue
            End With
        End If
    ElseIf IsLocalVariableDefined(name1) Then
        'ローカルな配列に対するCopyArray
        For Each var In LocalVariableList
            With var
                If InStr(.Name, name1 & "[") = 1 Then
                    buf = name2 & Mid$(.Name, InStr(.Name, "["))
                    SetVariable buf, .VariableType, .StringValue, .NumericValue
                End If
            End With
        Next
        If buf = "" Then
            Set var = GetVariableObject(name1)
            With var
                SetVariable name2, .VariableType, .StringValue, .NumericValue
            End With
        End If
    ElseIf IsGlobalVariableDefined(name1) Then
        'グローバルな配列に対するCopyArray
        For Each var In GlobalVariableList
            With var
                If InStr(.Name, name1 & "[") = 1 Then
                    buf = name2 & Mid$(.Name, InStr(.Name, "["))
                    SetVariable buf, .VariableType, .StringValue, .NumericValue
                End If
            End With
        Next
        If buf = "" Then
            Set var = GetVariableObject(name1)
            With var
                SetVariable name2, .VariableType, .StringValue, .NumericValue
            End With
        End If
    End If
    
    Set var = Nothing
    
    ExecCopyArrayCmd = LineNum + 1
End Function

Private Function ExecCopyFileCmd() As Long
Dim fname1 As String, fname2 As String
    
    If ArgNum <> 3 Then
        EventErrorMessage = "CopyFileコマンドの引数の数が違います"
        Error 0
    End If
    
    fname1 = GetArgAsString(2)
    If FileExists(ScenarioPath & fname1) Then
        fname1 = ScenarioPath & fname1
    ElseIf FileExists(ExtDataPath & fname1) Then
        fname1 = ExtDataPath & fname1
    ElseIf FileExists(ExtDataPath2 & fname1) Then
        fname1 = ExtDataPath2 & fname1
    ElseIf FileExists(AppPath & fname1) Then
        fname1 = AppPath & fname1
    Else
        ExecCopyFileCmd = LineNum + 1
        Exit Function
    End If
    
    If InStr(fname1, "..\") > 0 Then
        EventErrorMessage = "ファイル指定に「..\」は使えません"
        Error 0
    End If
    If InStr(fname1, "../") > 0 Then
        EventErrorMessage = "ファイル指定に「../」は使えません"
        Error 0
    End If
    
    fname2 = ScenarioPath & GetArgAsString(3)
    
    If InStr(fname2, "..\") > 0 Then
        EventErrorMessage = "ファイル指定に「..\」は使えません"
        Error 0
    End If
    If InStr(fname2, "../") > 0 Then
        EventErrorMessage = "ファイル指定に「../」は使えません"
        Error 0
    End If
    
    FileCopy fname1, fname2
    
    ExecCopyFileCmd = LineNum + 1
End Function

Private Function ExecCreateCmd() As Long
Dim uname As String, uparty As String, urank As Integer
Dim pname As String, plevel As Integer
Dim ux As Integer, uy As Integer
Dim u As Unit
Dim p As Pilot
Dim buf As String
Dim i As Integer, num As Integer
Dim opt As String

    num = ArgNum
    
    Select Case GetArgAsString(num)
        Case "非同期"
            opt = "非同期"
            num = num - 1
        Case "アニメ非表示"
            opt = ""
            num = num - 1
        Case Else
            opt = "出撃"
    End Select
    
    If num < 0 Then
        EventErrorMessage = "Createコマンドのパラメータの括弧の対応が取れていません"
        Error 0
    ElseIf num <> 8 And num <> 9 Then
        EventErrorMessage = "Createコマンドの引数の数が違います"
        Error 0
    End If
    
    uparty = GetArgAsString(2)
    If Not (uparty = "味方" Or uparty = "ＮＰＣ" _
        Or uparty = "敵" Or uparty = "中立") _
    Then
        EventErrorMessage = "所属の指定「" & uparty & "」が間違っています"
        Error 0
    End If
    
    uname = GetArgAsString(3)
    If Not UDList.IsDefined(uname) Then
        EventErrorMessage = "指定したユニット「" & uname & "」のデータが見つかりません"
        Error 0
    End If
    
    buf = GetArgAsString(4)
    If Not IsNumeric(buf) Then
        EventErrorMessage = "ユニットのランクが不正です"
        Error 0
    End If
    urank = CInt(buf)
    
    pname = GetArgAsString(5)
    If Not PDList.IsDefined(pname) Then
        EventErrorMessage = "指定したパイロット「" & pname & "」のデータが見つかりません"
        Error 0
    End If
    
    buf = GetArgAsString(6)
    If Not IsNumeric(buf) Then
        EventErrorMessage = "パイロットのレベルが不正です"
        Error 0
    End If
    plevel = CInt(buf)
    If IsOptionDefined("レベル限界突破") Then
        If plevel > 999 Then
            plevel = 999
        End If
    Else
        If plevel > 99 Then
            plevel = 99
        End If
    End If
    If plevel < 1 Then
        plevel = 1
    End If
    
    buf = GetArgAsString(7)
    If Not IsNumeric(buf) Then
        EventErrorMessage = "Ｘ座標の値が不正です"
        Error 0
    End If
    ux = CInt(buf)
    If ux < 1 Then
        ux = 1
    ElseIf ux > MapWidth Then
        ux = MapWidth
    End If
    
    buf = GetArgAsString(8)
    If Not IsNumeric(buf) Then
        EventErrorMessage = "Ｙ座標の値が不正です"
        Error 0
    End If
    uy = CInt(buf)
    If uy < 1 Then
        uy = 1
    ElseIf uy > MapHeight Then
        uy = MapHeight
    End If
    
    Set u = UList.Add(uname, urank, uparty)
    If u Is Nothing Then
        EventErrorMessage = uname & "のデータが不正です"
        Error 0
    End If
    
    If num = 9 Then
        Set p = PList.Add(pname, plevel, uparty, GetArgAsString(9))
    Else
        Set p = PList.Add(pname, plevel, uparty)
    End If
    
    p.Ride u
    
    If opt <> "非同期" And MainForm.Visible And Not IsPictureVisible Then
        Center ux, uy
        RefreshScreen
    End If
    With u
        .FullRecover
        For i = 1 To .CountOtherForm
            .OtherForm(i).FullSupply
        Next
        .UsedAction = 0
        
        .StandBy ux, uy, opt
        
        .CheckAutoHyperMode
    End With
    
    Set SelectedUnitForEvent = u.CurrentForm
    
    ExecCreateCmd = LineNum + 1
End Function

Private Function ExecCreateFolderCmd() As Long
Dim fname As String
    
    If ArgNum <> 2 Then
        EventErrorMessage = "CreateFolderコマンドの引数の数が違います"
        Error 0
    End If
    
    fname = ScenarioPath & GetArgAsString(2)
    
    If InStr(fname, "..\") > 0 Then
        EventErrorMessage = "フォルダ指定に「..\」は使えません"
        Error 0
    End If
    If InStr(fname, "../") > 0 Then
        EventErrorMessage = "フォルダ指定に「../」は使えません"
        Error 0
    End If
    
    If Right(fname, 1) = "\" Then
        fname = Left(fname, Len(fname) - 1)
    End If
    
    If Not FileExists(fname) Then
        MkDir fname
    End If
    
    ExecCreateFolderCmd = LineNum + 1
End Function

Private Function ExecDebugCmd() As Long
Dim i As Integer
    
    For i = 2 To ArgNum
        If i > 2 Then
            Debug.Print ", ";
        End If
        Debug.Print GetArgAsString(i);
    Next
    Debug.Print
    
    ExecDebugCmd = LineNum + 1
End Function

Private Function ExecDestroyCmd() As Long
Dim u As Unit
Dim uparty As String
Dim i As Integer

    Select Case ArgNum
        Case 2
            Set u = GetArgAsUnit(2)
        Case 1
            Set u = SelectedUnitForEvent
        Case Else
            EventErrorMessage = "Destroyコマンドの引数の数が違います"
            Error 0
    End Select
    
    '破壊キャンセル状態にある場合は解除しておく
    If u.IsConditionSatisfied("破壊キャンセル") Then
        u.DeleteCondition "破壊キャンセル"
    End If
    
    Select Case u.Status
        Case "出撃"
            u.Die
        Case "格納"
            u.Escape
            u.Status = "破壊"
        Case "破壊"
            If MapDataForUnit(u.X, u.Y) Is u Then
                u.Die
                '既に破壊イベントが発生しているはずなので、ここで終了
                ExecDestroyCmd = LineNum + 1
                Exit Function
            End If
        Case Else
            u.Status = "破壊"
    End Select
    
    'ステータス表示中の場合は表示を解除
    If u Is DisplayedUnit Then
        ClearUnitStatus
    End If
    
    'Destroyコマンドによって全滅したかを判定
    uparty = u.Party0
    For Each u In UList
        With u
            If .Party0 = uparty _
                And (.Status = "出撃" Or .Status = "格納") _
                And Not .IsConditionSatisfied("憑依") _
            Then
                ExecDestroyCmd = LineNum + 1
                Exit Function
            End If
        End With
    Next
    
    '戦闘時以外のイベント中の破壊は無視
    For i = 1 To UBound(EventQue)
        If EventQue(i) = "プロローグ" _
            Or EventQue(i) = "エピローグ" _
            Or EventQue(i) = "スタート" _
            Or EventQue(i) = "全滅" _
        Then
            ExecDestroyCmd = LineNum + 1
            Exit Function
        End If
    Next
    
    '後で全滅イベントを実行
    RegisterEvent "全滅", uparty
    
    ExecDestroyCmd = LineNum + 1
End Function

Private Function ExecDisableCmd() As Long
Dim aname As String, vname As String, uname As String
Dim u As Unit, i As Integer
Dim need_update As Boolean
    
    Select Case ArgNum
        Case 2
            aname = GetArgAsString(2)
        Case 3
            uname = GetArgAsString(2)
            aname = GetArgAsString(3)
        Case Else
            EventErrorMessage = "Disableコマンドの引数の数が違います"
            Error 0
    End Select
    
    If aname = "" Then
        EventErrorMessage = "Disableコマンドに指定された能力名が空文字列です"
        Error 0
    End If
    
    If uname <> "" Then
        vname = "Disable(" & uname & "," & aname & ")"
    Else
        vname = "Disable(" & aname & ")"
    End If
    
    'Disable用変数を設定
    If Not IsGlobalVariableDefined(vname) Then
        DefineGlobalVariable vname
        SetVariableAsLong vname, 1
    Else
        '既に設定済みであればそのまま終了
        ExecDisableCmd = LineNum + 1
        Exit Function
    End If
    
    'ユニットのステータスを更新
    If uname <> "" Then
        With UList
            If .IsDefined(uname) Then
                .Item(uname).CurrentForm.Update
            End If
        End With
    Else
        For Each u In UList
            With u
                If .Status = "出撃" Then
                    'ステータスを更新する必要があるかどうかチェックする
                    need_update = False
                    If .IsFeatureAvailable(aname) Then
                        need_update = True
                    Else
                        For i = 1 To .CountItem
                            If .Item(i).Name = aname Then
                                need_update = True
                                Exit For
                            End If
                        Next
                    End If
                    
                    '必要がある場合はステータスを更新
                    If need_update Then
                        .Update
                    End If
                End If
            End With
        Next
    End If
    
    ExecDisableCmd = LineNum + 1
End Function

Private Function ExecDoCmd() As Long
Dim i As Long, depth As Integer

    Select Case ArgNum
        Case 1
            ExecDoCmd = LineNum + 1
            Exit Function
        Case 3
            Select Case GetArg(2)
                Case "while"
                    If GetArgAsLong(3) <> 0 Then
                        ExecDoCmd = LineNum + 1
                        Exit Function
                    End If
                Case "until"
                    If GetArgAsLong(3) = 0 Then
                        ExecDoCmd = LineNum + 1
                        Exit Function
                    End If
                Case Else
                    EventErrorMessage = "Doコマンドの書式が間違っています"
                    Error 0
            End Select
        Case Else
            EventErrorMessage = "Doコマンドの引数の数が違います"
            Error 0
    End Select
    
    '条件式がFalseのため本体をスキップ
    depth = 1
    For i = LineNum + 1 To UBound(EventCmd)
        Select Case EventCmd(i).Name
            Case DoCmd
                depth = depth + 1
            Case LoopCmd
                depth = depth - 1
                If depth = 0 Then
                    ExecDoCmd = i + 1
                    Exit Function
                End If
        End Select
    Next
    
    EventErrorMessage = "DoとLoopが対応していません"
    Error 0
End Function

Private Function ExecLoopCmd() As Long
Dim i As Long, depth As Integer

    Select Case ArgNum
        Case 1
        Case 3
            Select Case GetArg(2)
                Case "while"
                    If GetArgAsLong(3) = 0 Then
                        ExecLoopCmd = LineNum + 1
                        Exit Function
                    End If
                Case "until"
                    If GetArgAsLong(3) <> 0 Then
                        ExecLoopCmd = LineNum + 1
                        Exit Function
                    End If
                Case Else
                    EventErrorMessage = "Loop文の書式が間違っています"
                    Error 0
            End Select
        Case Else
            EventErrorMessage = "Loop文の引数の数が違います"
            Error 0
    End Select
    
    '条件式がTrueのため先頭に戻る
    i = LineNum
    depth = 1
    Do While i > 1
        i = i - 1
        Select Case EventCmd(i).Name
            Case DoCmd
                depth = depth - 1
                If depth = 0 Then
                    ExecLoopCmd = i
                    Exit Function
                End If
            Case LoopCmd
                depth = depth + 1
        End Select
    Loop
    
    EventErrorMessage = "DoとLoopが対応していません"
    Error 0
End Function

Private Function ExecDrawOptionCmd() As Long
    If ArgNum <> 2 Then
        EventErrorMessage = "DrawOptionコマンドの引数の数が違います"
        Error 0
    End If
    
    ObjDrawOption = GetArgAsString(2)
    
    ExecDrawOptionCmd = LineNum + 1
End Function

Private Function ExecDrawWidthCmd() As Long
    If ArgNum <> 2 Then
        EventErrorMessage = "DrawWidthコマンドの引数の数が違います"
        Error 0
    End If
    
    ObjDrawWidth = GetArgAsLong(2)
    
    ExecDrawWidthCmd = LineNum + 1
End Function

Private Function ExecEnableCmd() As Long
Dim aname As String, vname As String, uname As String
Dim u As Unit

    Select Case ArgNum
        Case 2
            aname = GetArgAsString(2)
        Case 3
            uname = GetArgAsString(2)
            aname = GetArgAsString(3)
        Case Else
            EventErrorMessage = "Enableコマンドの引数の数が違います"
            Error 0
    End Select
    
    If uname <> "" Then
        vname = "Disable(" & uname & "," & aname & ")"
    Else
        vname = "Disable(" & aname & ")"
    End If
    
    'Disable用変数を削除
    If IsGlobalVariableDefined(vname) Then
        UndefineVariable vname
    Else
        '既に設定済みであればそのまま終了
        ExecEnableCmd = LineNum + 1
        Exit Function
    End If
    
    'ユニットのステータスを更新
    If uname <> "" Then
        With UList
            If .IsDefined(uname) Then
                .Item(uname).CurrentForm.Update
            End If
        End With
    Else
        For Each u In UList
            With u
                If .Status = "出撃" Then
                    .Update
                End If
            End With
        Next
    End If
    
    ExecEnableCmd = LineNum + 1
End Function

Private Function ExecEquipCmd() As Long
Dim u As Unit
Dim iname As String
Dim itm As Item
Dim i As Integer
    
    Select Case ArgNum
        Case 3
            Set u = GetArgAsUnit(2)
            iname = GetArgAsString(3)
        Case 2
            Set u = SelectedUnitForEvent
            iname = GetArgAsString(2)
        Case Else
        EventErrorMessage = "Equipコマンドの引数の数が違います"
        Error 0
    End Select
    
    '大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
    '名前をデータのそれとあわせる
    If IDList.IsDefined(iname) Then
        iname = IDList.Item(iname).Name
    End If
    
    '装備するアイテムを検索 or 作成
    If IList.IsDefined(iname) Then
        If iname = IList.Item(iname).Name Then
            'アイテム名で指定した場合
            If u.Party0 = "味方" Then
                'まずは装備されてないものを探す
                For Each itm In IList
                    With itm
                        If .Name = iname _
                            And .Unit Is Nothing _
                            And .Exist _
                        Then
                            GoTo EquipItem
                        End If
                    End With
                Next
                'なかったら装備されているものを…
                For Each itm In IList
                    With itm
                        If .Name = iname _
                            And Not .Unit Is Nothing _
                            And .Exist _
                        Then
                            If .Unit.Party0 = "味方" Then
                                GoTo EquipItem
                            End If
                        End If
                    End With
                Next
                'それでもなければ新たに作成
                Set itm = IList.Add(iname)
            Else
                Set itm = IList.Add(iname)
            End If
        Else
            'アイテムＩＤで指定した場合
            Set itm = IList.Item(iname)
        End If
    ElseIf IDList.IsDefined(iname) Then
        Set itm = IList.Add(iname)
    Else
        EventErrorMessage = "「" & iname & "」というアイテムは存在しません"
        Error 0
    End If

EquipItem:
    'アイテムを装備
    If Not itm Is Nothing Then
        With itm
            If .Exist Then
                If Not .Unit Is Nothing Then
                    .Unit.DeleteItem .ID
                End If
                
                With u
                    Dim ubitmap As String
                    Dim cmd_lv As Integer, rank_lv As Integer, support_lv As Integer
                    
                    ubitmap = .Bitmap
                    If .CountPilot > 0 Then
                        With .MainPilot
                            cmd_lv = .SkillLevel("指揮")
                            rank_lv = .SkillLevel("階級")
                            support_lv = .SkillLevel("広域サポート")
                        End With
                    End If
                    
                    .AddItem itm
                    
                    'ユニット画像が変化した？
                    If ubitmap <> .Bitmap Then
                        .BitmapID = MakeUnitBitmap(u)
                        For i = 1 To .CountOtherForm
                            .OtherForm(i).BitmapID = 0
                        Next
                        If .Status = "出撃" Then
                            If Not IsPictureVisible And MapFileName <> "" Then
                                PaintUnitBitmap u
                            End If
                        End If
                    End If
                    
                    '支援効果が変化した？
                    If .CountPilot > 0 Then
                        With .MainPilot
                            If cmd_lv <> .SkillLevel("指揮") _
                                Or rank_lv <> .SkillLevel("階級") _
                                Or support_lv <> .SkillLevel("広域サポート") _
                            Then
                                If u.Status = "出撃" Then
                                    PList.UpdateSupportMod u
                                End If
                            End If
                        End With
                    End If
                    
                    '最大弾数が変化した？
                    If itm.IsFeatureAvailable("最大弾数増加") Then
                        .FullSupply
                    End If
                End With
            End If
        End With
    End If
    
    ExecEquipCmd = LineNum + 1
End Function

Private Function ExecEscapeCmd() As Long
Dim pname As String, uparty As String
Dim u As Unit
Dim i As Integer, num As Integer
Dim opt As String
Dim ucount As Integer
    
    num = ArgNum
    
    If num > 1 Then
        If GetArgAsString(num) = "非同期" Then
            opt = "非同期"
            num = num - 1
        End If
    End If
    
    Select Case num
        Case 2
            pname = GetArgAsString(2)
            If pname = "味方" Or pname = "ＮＰＣ" _
                Or pname = "敵" Or pname = "中立" _
            Then
                uparty = pname
                For Each u In UList
                    With u
                        If .Party0 = uparty Then
                            If .Status = "出撃" Then
                                .Escape opt
                                ucount = ucount + 1
                            ElseIf .Status = "破壊" Then
                                If 1 <= .X And .X <= MapWidth _
                                    And 1 <= .Y And .Y <= MapHeight _
                                Then
                                    If u Is MapDataForUnit(.X, .Y) Then
                                        '破壊キャンセルで画面上に残っていた
                                        .Escape opt
                                    End If
                                End If
                            End If
                        End If
                    End With
                Next
            Else
                Set u = UList.Item2(pname)
                If u Is Nothing Then
                    With PList
                        If Not .IsDefined(pname) Then
                            EventErrorMessage = "「" & pname & _
                                "」というパイロットが見つかりません"
                            Error 0
                        End If
                        Set u = .Item(pname).Unit
                    End With
                End If
                If Not u Is Nothing Then
                    With u
                        If .Status = "出撃" Then
                            ucount = 1
                        End If
                        .Escape opt
                        uparty = .Party0
                    End With
                End If
            End If
        Case 1
            With SelectedUnitForEvent
                If .Status = "出撃" Then
                    ucount = 1
                End If
                .Escape opt
                uparty = .Party0
            End With
        Case Else
            EventErrorMessage = "Escapeコマンドの引数の数が違います"
            Error 0
    End Select
    
    'Escapeコマンドによって全滅したかを判定
    If uparty <> "ＮＰＣ" And uparty <> "味方" And ucount > 0 Then
        For Each u In UList
            With u
                If .Party0 = uparty _
                    And (.Status = "出撃" Or .Status = "格納") _
                    And Not .IsConditionSatisfied("憑依") _
                Then
                    ExecEscapeCmd = LineNum + 1
                    Exit Function
                End If
            End With
        Next
        
        '戦闘時以外のイベント中の撤退は無視
        For i = 1 To UBound(EventQue)
            If EventQue(i) = "プロローグ" _
                Or EventQue(i) = "エピローグ" _
                Or EventQue(i) = "スタート" _
                Or LIndex(EventQue(i), 1) = "マップ攻撃破壊" _
            Then
                ExecEscapeCmd = LineNum + 1
                Exit Function
            End If
        Next
        
        '後で全滅イベントを実行
        RegisterEvent "全滅", uparty
    End If
    
    ExecEscapeCmd = LineNum + 1
End Function

Private Function ExecExecCmd() As Long
Dim fname As String, opt As String
Dim msg As String
Dim n As Integer, i As Integer, j As Integer
Dim p As Pilot
Dim plevel As Integer
Dim u As Unit

    Select Case ArgNum
        Case 2
            fname = GetArgAsString(2)
        Case 3
            fname = GetArgAsString(2)
            opt = GetArgAsString(3)
        Case Else
            EventErrorMessage = "Execコマンドの引数の数が違います"
            Error 0
    End Select
    
    ClearUnitStatus
    
    '追加経験値を得るパイロットや破壊されたユニットがいなければ処理をスキップ
    n = 0
    For Each u In UList
        With u
            If .Party0 = "味方" Then
                If .Status = "出撃" Or .Status = "格納" Or .Status = "破壊" Then
                    n = 1
                    Exit For
                End If
            End If
        End With
    Next
    If n = 0 Then
        Turn = 0
    End If
    
    '追加経験値を収得
    If Turn > 0 And Not IsOptionDefined("追加経験値無効") Then
        OpenMessageForm
        
        n = 0
        msg = ""
        For Each p In PList
            With p
                If .Party <> "味方" Then
                    GoTo NextPilot
                End If
                
                If .MaxSP = 0 Then
                    GoTo NextPilot
                End If
                
                If .Unit Is Nothing Then
                    GoTo NextPilot
                End If
                
                If .Unit.Status <> "出撃" _
                    And .Unit.Status <> "格納" _
                Then
                    GoTo NextPilot
                End If
                
                plevel = .Level
                .Exp = .Exp + 2 * .SP
                
                '追加パイロットや暴走時パイロットに関する処理
                If .Unit.CountPilot > 0 And Not .IsSupport(.Unit) Then
                    '追加パイロットがメインパイロットの場合
                    If p Is .Unit.Pilot(1) _
                        And Not p Is .Unit.MainPilot _
                        And .Unit.MainPilot.MaxSP > 0 _
                    Then
                        GoTo NextPilot
                    End If
                    
                    '追加パイロットがメインパイロットではなくなった場合
                    If Not p Is .Unit.MainPilot Then
                        '自分がユニットのパイロット一覧に含まれているか判定
                        For i = 1 To .Unit.CountPilot
                            If p Is .Unit.Pilot(i) Then
                                Exit For
                            End If
                        Next
                        If i > .Unit.CountPilot Then
                            GoTo NextPilot
                        End If
                    End If
                End If
                
                If plevel = .Level Then
                    msg = msg & ";" & .Nickname & " 経験値 +" & Format$(2 * .SP)
                Else
                    msg = msg & ";" & .Nickname & " 経験値 +" & Format$(2 * .SP) _
                        & " レベルアップ！（Lv" & Format$(.Level) & "）"
                End If
                n = n + 1
                If n = 4 Then
                    DisplayMessage "システム", Mid$(msg, 2)
                    msg = ""
                    n = 0
                End If
            End With
NextPilot:
        Next
        If n > 0 Then
            DisplayMessage "システム", Mid$(msg, 2)
        End If
        
        CloseMessageForm
    End If
    
    MainForm.Hide
    
    'エピローグイベントを実行
    If IsEventDefined("エピローグ") Then
        'ハイパーモードや変身、能力コピーを解除
        For Each u In UList
            With u
                If .Status <> "他形態" _
                    And .Status <> "旧主形態" _
                    And .Status <> "旧形態" _
                Then
                    If .IsFeatureAvailable("ノーマルモード") Then
                        .Transform LIndex(.FeatureData("ノーマルモード"), 1)
                    End If
                End If
            End With
        Next
        
        If IsEventDefined("エピローグ", True) Then
            StopBGM
            StartBGM BGMName("Briefing")
        End If
        
        Stage = "エピローグ"
        HandleEvent "エピローグ"
    End If
    
    MainForm.Hide
    
    'マップをクリア
    For i = 1 To MapWidth
        For j = 1 To MapHeight
            Set MapDataForUnit(i, j) = Nothing
        Next
    Next
    
    '各種データをアップデート
    UList.Update
    PList.Update
    IList.Update
    ClearEventData
    ClearMap
    
    '通常ステージとして実行する？
    If opt = "通常ステージ" Then
        IsSubStage = False
    Else
        IsSubStage = True
    End If
    
    'イベントファイルを実行
    StartScenario fname
    
    IsScenarioFinished = True
    
    ExecExecCmd = 0
End Function

Private Function ExecExitCmd() As Long
    ExecExitCmd = 0
End Function

Private Function ExecExchangeItemCmd() As Long
Dim u As Unit, ipart As String

    Select Case ArgNum
        Case 1
            Set u = SelectedUnitForEvent
        Case 2
            Set u = GetArgAsUnit(2)
        Case 3
            Set u = GetArgAsUnit(2)
            ipart = GetArgAsString(3)
        Case Else
            EventErrorMessage = "ExchangeItemコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExchangeItemCommand u, ipart
    
    ExecExchangeItemCmd = LineNum + 1
End Function

Private Function ExecExplodeCmd() As Long
Dim esize As String, tx As Integer, ty As Integer

    Select Case ArgNum
        Case 2
            esize = GetArgAsString(2)
            tx = MapX
            ty = MapY
        Case 4
            esize = GetArgAsString(2)
            tx = GetArgAsLong(3)
            ty = GetArgAsLong(4)
        Case Else
            EventErrorMessage = "Explodeコマンドの引数の数が違います"
            Error 0
    End Select
    
    '爆発の表示
    ExplodeAnimation esize, tx, ty
    
    ExecExplodeCmd = LineNum + 1
End Function

Private Function ExecExpUpCmd() As Long
Dim pname As String, p As Pilot, prev_lv As Integer
Dim hp_ratio As Double, en_ratio As Double
Dim num As Integer

    Select Case ArgNum
        Case 3
            Set p = GetArgAsPilot(2)
            num = GetArgAsLong(3)
            
        Case 2
            With SelectedUnitForEvent
                If .CountPilot > 0 Then
                    Set p = .Pilot(1)
                Else
                    ExecExpUpCmd = LineNum + 1
                    Exit Function
                End If
            End With
            num = GetArgAsLong(2)
            
        Case Else
            EventErrorMessage = "ExpUpコマンドの引数の数が違います"
            Error 0
    End Select
    
    With p
        If Not .Unit Is Nothing Then
            With .Unit
                hp_ratio = 100 * .HP / .MaxHP
                en_ratio = 100 * .EN / .MaxEN
            End With
        End If
        
        prev_lv = .Level
        
        .Exp = .Exp + num
        
        If .Level = prev_lv Then
            ExecExpUpCmd = LineNum + 1
            Exit Function
        End If
        
        .Update
        
        'ＳＰ＆霊力をアップデート
        .SP = .SP
        .Plana = .Plana
        
        If Not .Unit Is Nothing Then
            With .Unit
                .Update
                .HP = .MaxHP * hp_ratio / 100
                .EN = .MaxEN * en_ratio / 100
            End With
            PList.UpdateSupportMod .Unit
        End If
    End With
    
    ExecExpUpCmd = LineNum + 1
End Function

Private Function ExecFadeInCmd() As Long
Dim start_time As Long, cur_time As Long, wait_time As Long
Dim i As Long, num As Integer, ret As Long

    If IsRButtonPressed() Then
        MainForm.picMain(0).Refresh
        ExecFadeInCmd = LineNum + 1
        Exit Function
    End If
    
    Select Case ArgNum
        Case 1
            num = 10
        Case 2
            num = GetArgAsLong(2)
        Case Else
            EventErrorMessage = "FadeInコマンドの引数の数が違います"
            Error 0
    End Select
    
    SaveScreen
    
    With MainForm
        With .picTmp
            .Picture = LoadPicture("")
            .width = MainPWidth
            .Height = MainPHeight
        End With
        
' MOD START マージ
'        ret = BitBlt(.picTmp.hDC, _
'            0, 0, MapPWidth, MapPHeight, _
'            .picMain(0).hDC, 0, 0, SRCCOPY)
        ret = BitBlt(.picTmp.hDC, _
            0, 0, MainPWidth, MainPHeight, _
            .picMain(0).hDC, 0, 0, SRCCOPY)
' MOD END マージ
        
        InitFade .picMain(0), num
        
        start_time = timeGetTime()
        wait_time = 50
        For i = 0 To num
            If i Mod 4 = 0 Then
                If IsRButtonPressed() Then
                    Exit For
                End If
            End If
            
            DoFade .picMain(0), i
            .picMain(0).Refresh
            
            cur_time = timeGetTime()
            Do While cur_time < start_time + wait_time * (i + 1)
                DoEvents
                cur_time = timeGetTime()
            Loop
        Next
        
        FinishFade
        
        ret = BitBlt(.picMain(0).hDC, _
            0, 0, MapPWidth, MapPHeight, _
            .picTmp.hDC, 0, 0, SRCCOPY)
        .picMain(0).Refresh
        
        With .picTmp
            .Picture = LoadPicture("")
            .width = 32
            .Height = 32
        End With
    End With
    
    ExecFadeInCmd = LineNum + 1
End Function

Private Function ExecFadeOutCmd() As Long
Dim start_time As Long, cur_time As Long, wait_time As Long
Dim i As Long, num As Integer, ret As Long

    Select Case ArgNum
        Case 1
            num = 10
        Case 2
            num = GetArgAsLong(2)
        Case Else
            EventErrorMessage = "FadeOutコマンドの引数の数が違います"
            Error 0
    End Select
    
    SaveScreen
    
    With MainForm
        InitFade .picMain(0), num
        
        start_time = timeGetTime()
        wait_time = 50
        For i = 0 To num
            If i Mod 4 = 0 Then
                If IsRButtonPressed() Then
                    With .picMain(0)
                        ret = PatBlt(.hDC, 0, 0, .width, .Height, BLACKNESS)
                        .Refresh
                    End With
                    Exit For
                End If
            End If
            
            DoFade .picMain(0), num - i
            .picMain(0).Refresh
            
            cur_time = timeGetTime()
            Do While cur_time < start_time + wait_time * (i + 1)
                DoEvents
                cur_time = timeGetTime()
            Loop
        Next
        
        FinishFade
    End With
    
    IsPictureVisible = True
    PaintedAreaX1 = MainPWidth
    PaintedAreaY1 = MainPHeight
    PaintedAreaX2 = -1
    PaintedAreaY2 = -1
    
    ExecFadeOutCmd = LineNum + 1
End Function

Private Function ExecFillColorCmd() As Long
Dim opt As String, cname As String

    If ArgNum <> 2 Then
        EventErrorMessage = "FillColorコマンドの引数の数が違います"
        Error 0
    End If
    
    opt = GetArgAsString(2)
    If Asc(opt) <> 35 Or Len(opt) <> 7 Then
        EventErrorMessage = "色指定が不正です"
        Error 0
    End If
    cname = String$(8, vbNullChar)
    Mid(cname, 1, 2) = "&H"
    Mid(cname, 3, 2) = Mid$(opt, 6, 2)
    Mid(cname, 5, 2) = Mid$(opt, 4, 2)
    Mid(cname, 7, 2) = Mid$(opt, 2, 2)
    If Not IsNumeric(cname) Then
        EventErrorMessage = "色指定が不正です"
        Error 0
    End If
    
    ObjFillColor = CLng(cname)
    
    ExecFillColorCmd = LineNum + 1
End Function

Private Function ExecFillStyleCmd() As Long
    If ArgNum <> 2 Then
        EventErrorMessage = "FillStyleコマンドの引数の数が違います"
        Error 0
    End If
    
    Select Case GetArgAsString(2)
        Case "塗りつぶし"
            ObjFillStyle = vbFSSolid
        Case "透明"
            ObjFillStyle = vbFSTransparent
        Case "横線"
            ObjFillStyle = vbHorizontalLine
        Case "縦線"
            ObjFillStyle = vbVerticalLine
        Case "斜線"
            ObjFillStyle = vbUpwardDiagonal
        Case "斜線２"
            ObjFillStyle = vbDownwardDiagonal
        Case "クロス"
            ObjFillStyle = vbCross
        Case "網かけ"
            ObjFillStyle = vbDiagonalCross
        Case Else
            EventErrorMessage = "背景描画方法の指定が不正です"
            Error 0
    End Select
    
    ExecFillStyleCmd = LineNum + 1
End Function

Private Function ExecFinishCmd() As Long
Dim u As Unit

    Select Case ArgNum
        Case 2
            Set u = GetArgAsUnit(2, True)
        Case 1
            Set u = SelectedUnitForEvent
        Case Else
            EventErrorMessage = "Finishコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not u Is Nothing Then
        With u
            Select Case .Action
                Case 1
                    .UseAction
                    If .Status = "出撃" Then
                        PaintUnitBitmap u
                    End If
                Case 0
                    'なにもしない
                Case Else
                    .UseAction
            End Select
        End With
    End If
    
    ExecFinishCmd = LineNum + 1
End Function

Private Function ExecFixCmd() As Long
Dim buf As String

    Select Case ArgNum
        Case 1
            buf = SelectedUnitForEvent.Pilot(1).Name
        Case 2
            buf = GetArgAsString(2)
            If Not PList.IsDefined(buf) And Not IList.IsDefined(buf) Then
                EventErrorMessage = "パイロット名またはアイテム名" & buf & "が間違っています"
                Error 0
            End If
            If PList.IsDefined(buf) Then
                buf = PList.Item(buf).Name
            Else
                buf = IList.Item(buf).Name
            End If
        Case Else
            EventErrorMessage = "Fixコマンドの引数の数が違います"
            Error 0
    End Select
    
    buf = "Fix(" & buf & ")"
    If Not IsGlobalVariableDefined(buf) Then
        DefineGlobalVariable buf
    End If
    SetVariableAsLong buf, 1
    
    ExecFixCmd = LineNum + 1
End Function

Private Function ExecFontCmd() As Long
Dim opt As String, cname As String, fname As String
Dim i As Integer
Dim sf As StdFont

    ExecFontCmd = LineNum + 1
    
    With MainForm.picMain(0)
        fname = .Font.Name
        
        'デフォルトの設定
        If ArgNum = 1 Then
            .ForeColor = vbWhite
            With .Font
                fname = "ＭＳ Ｐ明朝"
                .Size = 16
                .Bold = True
                .Italic = False
            End With
            PermanentStringMode = False
            KeepStringMode = False
        Else
            For i = 2 To ArgNum
                opt = GetArgAsString(i)
                Select Case opt
                    Case "Ｐ明朝"
                        fname = "ＭＳ Ｐ明朝"
                    Case "Ｐゴシック"
                        fname = "ＭＳ Ｐゴシック"
                    Case "明朝"
                        fname = "ＭＳ 明朝"
                    Case "ゴシック"
                        fname = "ＭＳ ゴシック"
                    Case "Bold"
                        .Font.Bold = True
                    Case "Italic"
                        .Font.Italic = True
                    Case "Regular"
                        .Font.Bold = False
                        .Font.Italic = False
                    Case "通常"
                        PermanentStringMode = False
                        KeepStringMode = False
                    Case "背景"
                        PermanentStringMode = True
                    Case "保持"
                        KeepStringMode = True
                    Case " ", ""
                        '無視
                    Case Else
                        If Right$(opt, 2) = "pt" Then
                            '文字サイズ
                            opt = Left$(opt, Len(opt) - 2)
                            .Font.Size = CInt(opt)
                        ElseIf Asc(opt) = 35 And Len(opt) = 7 Then
                            '文字色
                            cname = String$(8, vbNullChar)
                            Mid(cname, 1, 2) = "&H"
                            Mid(cname, 3, 2) = Mid$(opt, 6, 2)
                            Mid(cname, 5, 2) = Mid$(opt, 4, 2)
                            Mid(cname, 7, 2) = Mid$(opt, 2, 2)
                            If IsNumeric(cname) Then
                                .ForeColor = CLng(cname)
                            End If
                        Else
                            'その他のフォント
                            fname = opt
                        End If
                End Select
            Next
        End If
        
        'フォント名が変更されている？
        If fname <> .Font.Name Then
            Set sf = New StdFont
            With .Font
                sf.Name = fname
                sf.Size = .Size
                sf.Bold = .Bold
                sf.Italic = .Italic
            End With
            Set .Font = sf
        End If
    End With
End Function

Private Function ExecForCmd() As Long
Dim vname As String
Dim i As Long, depth As Integer, idx As Long, limit As Long
Dim isincr As Integer

    If ArgNum <> 6 And ArgNum <> 8 Then
        EventErrorMessage = "Forコマンドの引数の数が違います"
        Error 0
    End If
    
    'インデックス変数に初期値を設定
    vname = GetArg(2)
    idx = GetArgAsLong(4)
    SetVariableAsLong vname, idx
    
    'ループの終端値
    limit = GetArgAsLong(6)
    
    'ArgNumが8かつ引数8が<0の場合、インデックスが減算されるループとして
    'ループ終了の条件式の不等号を逆にします
    '(idxおよびlimitの値に-1を乗算することで、擬似的に不等号を反対にしています)
    'ExecNextCmdでも同様の処理をしています
    isincr = 1
    If ArgNum = 8 Then
        If GetArgAsLong(8) < 0 Then
            isincr = -1
        End If
    End If
    
    If idx * isincr <= limit * isincr Then
        '終端値をスタックに格納
        ForIndex = ForIndex + 1
        ForLimitStack(ForIndex) = limit
        '初回のループを実行
        ExecForCmd = LineNum + 1
    Else
        '最初から条件式を満たしていない場合
        
        '対応するNextコマンドを探す
        depth = 1
        For i = LineNum + 1 To UBound(EventCmd)
            Select Case EventCmd(i).Name
                Case ForCmd, ForEachCmd
                    depth = depth + 1
                Case NextCmd
                    depth = depth - 1
                    If depth = 0 Then
                        ExecForCmd = i + 1
                        Exit Function
                    End If
            End Select
        Next
        
        EventErrorMessage = "ForまたはForEachとNextが対応していません"
        Error 0
    End If
End Function

Private Function ExecForEachCmd() As Long
Dim uparty As String
Dim ustatus As String
Dim ugroup As String
Dim u As Unit, p As Pilot
Dim i As Long, j As Integer, depth As Integer
Dim vname As String, aname As String
Dim buf As String
Dim var As VarData
Dim key_type As String, key_list() As Long, strkey_list() As String
Dim max_value As Long, max_str As String, max_item As Integer

    ReDim ForEachSet(0)
    
    Select Case ArgNum
        'ユニットに対するForEach
        Case 2, 3
            If ArgNum = 2 Then
                ustatus = "出撃 格納"
            Else
                ustatus = GetArgAsString(3)
                If ustatus = "全て" Then
                    ustatus = "全"
                End If
            End If
            
            Select Case GetArgAsString(2)
                Case "全"
                    If ustatus = "全" Then
                        For Each u In UList
                            With u
                                If .Status <> "他形態" _
                                    And .Status <> "旧主形態" _
                                    And .Status <> "旧形態" _
                                    And .Status <> "破棄" _
                                Then
                                    ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
                                    ForEachSet(UBound(ForEachSet)) = .ID
                                End If
                            End With
                        Next
                    Else
                        For Each u In UList
                            With u
                                If InStr(ustatus, .Status) > 0 Then
                                    ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
                                    ForEachSet(UBound(ForEachSet)) = .ID
                                End If
                            End With
                        Next
                    End If
                Case "味方", "ＮＰＣ", "敵", "中立"
                    uparty = GetArgAsString(2)
                    If ustatus = "全" Then
                        For Each u In UList
                            With u
                                If .Party0 = uparty Then
                                    If .Status <> "他形態" _
                                        And .Status <> "旧主形態" _
                                        And .Status <> "旧形態" _
                                        And .Status <> "破棄" _
                                    Then
                                        ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
                                        ForEachSet(UBound(ForEachSet)) = .ID
                                    End If
                                End If
                            End With
                        Next
                    Else
                        For Each u In UList
                            With u
                                If .Party0 = uparty Then
                                    If InStr(ustatus, .Status) > 0 Then
                                        ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
                                        ForEachSet(UBound(ForEachSet)) = .ID
                                    End If
                                End If
                            End With
                        Next
                    End If
                Case Else
                    ugroup = GetArgAsString(2)
                    If ustatus = "全て" Then
                        ustatus = "全"
                    End If
                    For Each u In UList
                        With u
                            If .CountPilot > 0 Then
                                If .MainPilot.ID = ugroup _
                                    Or InStr(.MainPilot.ID, ugroup & ":") = 1 _
                                Then
                                    If ustatus = "全" Then
                                        If .Status <> "他形態" _
                                            And .Status <> "旧主形態" _
                                            And .Status <> "旧形態" _
                                            And .Status <> "破棄" _
                                        Then
                                            ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
                                            ForEachSet(UBound(ForEachSet)) = .ID
                                        End If
                                    Else
                                        If InStr(ustatus, .Status) > 0 Then
                                            ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
                                            ForEachSet(UBound(ForEachSet)) = .ID
                                        End If
                                    End If
                                End If
                            End If
                        End With
                    Next
            End Select
        
        '配列の要素に対するForEach
        Case 4
            'インデックス用変数名
            vname = GetArg(2)
            If Left$(vname, 1) = "$" Then
                vname = Mid$(vname, 2)
            End If
            
            '配列の変数名
            aname = GetArg(4)
            If Left$(aname, 1) = "$" Then
                aname = Mid$(aname, 2)
            End If
            'Eval関数
            If LCase$(Left$(aname, 5)) = "eval(" Then
                If Right$(aname, 1) = ")" Then
                    aname = Mid$(aname, 6, Len(aname) - 6)
                    aname = GetValueAsString(aname)
                End If
            End If
            
            '配列を検索し、配列要素を見つける
            If InStrNotNest(aname, "パイロット一覧(") = 1 Then
                key_type = Mid$(aname, InStrNotNest(aname, "(") + 1, _
                    Len(aname) - InStrNotNest(aname, "(") - 1)
                key_type = GetValueAsString(key_type)
                
                If key_type <> "名称" Then
                    '配列作成
                    ReDim ForEachSet(PList.Count)
                    ReDim key_list(PList.Count)
                    i = 0
                    For Each p In PList
                        With p
                            If Not .Alive Or .Away Then
                                GoTo NextPilot1
                            End If
                            
                            If Not .Unit Is Nothing Then
                                With .Unit
                                    If .CountPilot > 0 Then
                                        If p Is .MainPilot And Not p Is .Pilot(1) Then
                                            GoTo NextPilot1
                                        End If
                                    End If
                                End With
                            End If
                            
                            i = i + 1
                            ForEachSet(i) = .ID
                            Select Case key_type
                                Case "レベル"
                                    key_list(i) = 500 * CLng(.Level) + CLng(.Exp)
                                Case "ＳＰ"
                                    key_list(i) = .MaxSP
                                Case "格闘"
                                    key_list(i) = .Infight
                                Case "射撃"
                                    key_list(i) = .Shooting
                                Case "命中"
                                    key_list(i) = .Hit
                                Case "回避"
                                    key_list(i) = .Dodge
                                Case "技量"
                                    key_list(i) = .Technique
                                Case "反応"
                                    key_list(i) = .Intuition
                            End Select
                        End With
NextPilot1:
                    Next
                    ReDim Preserve ForEachSet(i)
                    ReDim Preserve key_list(i)
                    
                    'ソート
                    For i = 1 To UBound(ForEachSet) - 1
                        max_item = i
                        max_value = key_list(i)
                        For j = i + 1 To UBound(ForEachSet)
                            If key_list(j) > max_value Then
                                max_item = j
                                max_value = key_list(j)
                            End If
                        Next
                        If max_item <> i Then
                            buf = ForEachSet(i)
                            ForEachSet(i) = ForEachSet(max_item)
                            ForEachSet(max_item) = buf
                            
                            key_list(max_item) = key_list(i)
                        End If
                    Next
                Else
                    '配列作成
                    ReDim ForEachSet(PList.Count)
                    ReDim strkey_list(PList.Count)
                    i = 0
                    For Each p In PList
                        With p
                            If Not .Alive Or .Away Then
                                GoTo NextPilot2
                            End If
                            
                            If Not .Unit Is Nothing Then
                                With .Unit
                                    If .CountPilot > 0 Then
                                        If p Is .MainPilot And Not p Is .Pilot(1) Then
                                            GoTo NextPilot2
                                        End If
                                    End If
                                End With
                            End If
                            
                            i = i + 1
                            ForEachSet(i) = .ID
                            strkey_list(i) = .KanaName
                        End With
NextPilot2:
                    Next
                    ReDim Preserve ForEachSet(i)
                    ReDim Preserve strkey_list(i)
                    
                    'ソート
                    For i = 1 To UBound(ForEachSet) - 1
                        max_item = i
                        max_str = strkey_list(i)
                        For j = i + 1 To UBound(ForEachSet)
                            If StrComp(strkey_list(j), max_str, 1) = -1 Then
                                max_item = j
                                max_str = strkey_list(j)
                            End If
                        Next
                        If max_item <> i Then
                            buf = ForEachSet(i)
                            ForEachSet(i) = ForEachSet(max_item)
                            ForEachSet(max_item) = buf
                            
                            strkey_list(max_item) = strkey_list(i)
                        End If
                    Next
                End If
            ElseIf InStrNotNest(aname, "ユニット一覧(") = 1 Then
                key_type = Mid$(aname, InStrNotNest(aname, "(") + 1, _
                    Len(aname) - InStrNotNest(aname, "(") - 1)
                key_type = GetValueAsString(key_type)
                
                If key_type <> "名称" Then
                    '配列作成
                    ReDim ForEachSet(UList.Count)
                    ReDim key_list(UList.Count)
                    i = 0
                    For Each u In UList
                        With u
                            If .Status = "出撃" Or .Status = "格納" Or .Status = "待機" Then
                                i = i + 1
                                ForEachSet(i) = .ID
                                Select Case key_type
                                    Case "ランク"
                                        key_list(i) = .Rank
                                    Case "ＨＰ"
                                        key_list(i) = .HP
                                    Case "ＥＮ"
                                        key_list(i) = .EN
                                    Case "装甲"
                                        key_list(i) = .Armor
                                    Case "運動性"
                                        key_list(i) = .Mobility
                                    Case "移動力"
                                        key_list(i) = .Speed
                                    Case "最大攻撃力"
                                        For j = 1 To .CountWeapon
                                            If .IsWeaponMastered(j) _
                                                And Not .IsDisabled(.Weapon(j).Name) _
                                                And Not .IsWeaponClassifiedAs(j, "合") _
                                            Then
                                                If .WeaponPower(j, "") > key_list(i) Then
                                                    key_list(i) = .WeaponPower(j, "")
                                                End If
                                            End If
                                        Next
                                    Case "最長射程"
                                        For j = 1 To .CountWeapon
                                            If .IsWeaponMastered(j) _
                                                And Not .IsDisabled(.Weapon(j).Name) _
                                                And Not .IsWeaponClassifiedAs(j, "合") _
                                            Then
                                                If .WeaponMaxRange(j) > key_list(i) Then
                                                    key_list(i) = .WeaponMaxRange(j)
                                                End If
                                            End If
                                        Next
                                End Select
                            End If
                        End With
                    Next
                    ReDim Preserve ForEachSet(i)
                    ReDim Preserve key_list(i)
                    
                    'ソート
                    For i = 1 To UBound(ForEachSet) - 1
                        max_item = i
                        max_value = key_list(i)
                        For j = i + 1 To UBound(ForEachSet)
                            If key_list(j) > max_value Then
                                max_item = j
                                max_value = key_list(j)
                            End If
                        Next
                        If max_item <> i Then
                            buf = ForEachSet(i)
                            ForEachSet(i) = ForEachSet(max_item)
                            ForEachSet(max_item) = buf
                            
                            key_list(max_item) = key_list(i)
                        End If
                    Next
                Else
                    '配列作成
                    ReDim ForEachSet(UList.Count)
                    ReDim strkey_list(UList.Count)
                    i = 0
                    For Each u In UList
                        With u
                            If .Status = "出撃" Or .Status = "格納" Or .Status = "待機" Then
                                i = i + 1
                                ForEachSet(i) = .ID
                                strkey_list(i) = .KanaName
                            End If
                        End With
                    Next
                    ReDim Preserve ForEachSet(i)
                    ReDim Preserve strkey_list(i)
                    
                    'ソート
                    For i = 1 To UBound(ForEachSet) - 1
                        max_item = i
                        max_str = strkey_list(i)
                        For j = i + 1 To UBound(ForEachSet)
                            If StrComp(strkey_list(j), max_str, 1) = -1 Then
                                max_item = j
                                max_str = strkey_list(j)
                            End If
                        Next
                        If max_item <> i Then
                            buf = ForEachSet(i)
                            ForEachSet(i) = ForEachSet(max_item)
                            ForEachSet(max_item) = buf
                            
                            strkey_list(max_item) = strkey_list(i)
                        End If
                    Next
                End If
            ElseIf IsSubLocalVariableDefined(aname) Then
                'サブルーチンローカルな配列に対するForEach
                For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
                    With VarStack(i)
                        If InStr(.Name, aname & "[") = 1 Then
                            ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
                            buf = .Name
                            For j = 1 To Len(buf)
                                If Mid$(buf, Len(buf) - j + 1, 1) = "]" Then
                                    Exit For
                                End If
                            Next
                            buf = Mid$(buf, InStr(buf, "[") + 1)
                            buf = Left$(buf, Len(buf) - j)
                            ForEachSet(UBound(ForEachSet)) = buf
                        End If
                    End With
                Next
                If UBound(ForEachSet) = 0 Then
                    buf = GetValueAsString(aname)
                    ReDim ForEachSet(ListLength(buf))
                    For i = 1 To ListLength(buf)
                        ForEachSet(i) = ListIndex(buf, i)
                    Next
                End If
            ElseIf IsLocalVariableDefined(aname) Then
                'ローカルな配列に対するForEach
                For Each var In LocalVariableList
                    With var
                        If InStr(.Name, aname & "[") = 1 Then
                            ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
                            buf = .Name
                            For i = 1 To Len(buf)
                                If Mid$(buf, Len(buf) - i + 1, 1) = "]" Then
                                    Exit For
                                End If
                            Next
                            buf = Mid$(buf, InStr(buf, "[") + 1)
                            buf = Left$(buf, Len(buf) - i)
                            ForEachSet(UBound(ForEachSet)) = buf
                        End If
                    End With
                Next
                If UBound(ForEachSet) = 0 Then
                    buf = GetValueAsString(aname)
                    ReDim ForEachSet(ListLength(buf))
                    For i = 1 To ListLength(buf)
                        ForEachSet(i) = ListIndex(buf, i)
                    Next
                End If
            ElseIf IsGlobalVariableDefined(aname) Then
                'グローバルな配列に対するForEach
                For Each var In GlobalVariableList
                    With var
                        If InStr(.Name, aname & "[") = 1 Then
                            ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
                            buf = .Name
                            For i = 1 To Len(buf)
                                If Mid$(buf, Len(buf) - i + 1, 1) = "]" Then
                                    Exit For
                                End If
                            Next
                            buf = Mid$(buf, InStr(buf, "[") + 1)
                            buf = Left$(buf, Len(buf) - i)
                            ForEachSet(UBound(ForEachSet)) = buf
                        End If
                    End With
                Next
                If UBound(ForEachSet) = 0 Then
                    buf = GetValueAsString(aname)
                    ReDim ForEachSet(ListLength(buf))
                    For i = 1 To ListLength(buf)
                        ForEachSet(i) = ListIndex(buf, i)
                    Next
                End If
            ElseIf (Left$(aname, 1) = "(" And Right$(aname, 1) = ")") _
                Or (Left$(aname, 1) = """" And Right$(aname, 1) = """") _
                Or (Left$(aname, 1) = "`" And Right$(aname, 1) = "`") _
                Or (InStr(LCase$(aname), "list(") = 1 And Right$(aname, 1) = ")") _
            Then
                'リストに対するForEach
                buf = GetValueAsString(aname)
                ReDim ForEachSet(ListLength(buf))
                For i = 1 To ListLength(buf)
                    ForEachSet(i) = ListIndex(buf, i)
                Next
            End If
        
        Case Else
            EventErrorMessage = "ForEachコマンドの引数の数が違います"
            Error 0
    End Select
    
    If UBound(ForEachSet) > 0 Then
        'ForEachの実行要素がある場合
        
        ForEachIndex = 1
        ForIndex = ForIndex + 1
        
        If ArgNum < 4 Then
            Set SelectedUnitForEvent = UList.Item(ForEachSet(1))
        Else
            SetVariableAsString GetArg(2), ForEachSet(1)
        End If
        ExecForEachCmd = LineNum + 1
    Else
        'ForEachの実行要素がない場合
        
        '対応するNextを探す
        depth = 1
        For i = LineNum + 1 To UBound(EventCmd)
            Select Case EventCmd(i).Name
                Case ForCmd, ForEachCmd
                    depth = depth + 1
                Case NextCmd
                    depth = depth - 1
                    If depth = 0 Then
                        ExecForEachCmd = i + 1
                        Exit Function
                    End If
            End Select
        Next
        
        EventErrorMessage = "ForまたはForEachとNextが対応していません"
        Error 0
    End If
End Function

Private Function ExecNextCmd() As Long
Dim i As Long, depth As Integer, idx As Double
Dim vname As String, buf As String
Dim isincr As Integer
    
    '対応するForまたはForEachを探す
    i = LineNum
    depth = 1
    Do While i > 1
        i = i - 1
        With EventCmd(i)
            Select Case .Name
                Case ForCmd
                    depth = depth - 1
                    If depth = 0 Then
                        'インデックス変数の値を1増やす
                        vname = .GetArg(2)
                        
                        'Step句が設定されている場合、インデックス変数に引数8の値を加算
                        If .ArgNum = 6 Then
                            idx = GetValueAsDouble(vname, True) + 1
                        Else
                            idx = GetValueAsDouble(vname, True) + .GetArgAsLong(8)
                        End If
                        SetVariableAsDouble vname, idx
                        
                        'インデックス変数の値は範囲内？
                        isincr = 1
                        If .ArgNum = 8 Then
                            If .GetArgAsLong(8) < 0 Then
                                isincr = -1
                            End If
                        End If
                        If idx * isincr > ForLimitStack(ForIndex) * isincr Then
                            'ループ終了
                            i = LineNum
                            ForIndex = ForIndex - 1
                        End If
                        ExecNextCmd = i + 1
                        Exit Function
                    End If
                Case ForEachCmd
                    depth = depth - 1
                    If depth = 0 Then
                        ForEachIndex = ForEachIndex + 1
                        If ForEachIndex > UBound(ForEachSet) Then
                            'ループ終了
                            i = LineNum
                            ForIndex = ForIndex - 1
                        Else
                            If .ArgNum < 4 Then
                                'ユニット＆パイロットに対するForEach
                                Set SelectedUnitForEvent = _
                                    UList.Item(ForEachSet(ForEachIndex))
                            Else
                                '配列に対するForEach
                                SetVariableAsString .GetArg(2), ForEachSet(ForEachIndex)
                            End If
                        End If
                        ExecNextCmd = i + 1
                        Exit Function
                    End If
                Case NextCmd
                    depth = depth + 1
            End Select
        End With
    Loop
    
    EventErrorMessage = "ForまたはForEachとNextが対応していません"
    Error 0
End Function

Private Function ExecForgetCmd() As Long
Dim tname As String
Dim i As Integer, j As Integer

    If ArgNum <> 2 Then
        EventErrorMessage = "Forgetコマンドの引数の数が違います"
        Error 0
    End If
    
    tname = GetArgAsString(2)
    For i = 1 To UBound(Titles)
        If tname = Titles(i) Then
            Exit For
        End If
    Next
    If i <= UBound(Titles) Then
        For j = i + 1 To UBound(Titles)
            Titles(j - 1) = Titles(j)
        Next
        ReDim Preserve Titles(UBound(Titles) - 1)
    End If
    
    ExecForgetCmd = LineNum + 1
End Function

Private Function ExecFreeMemoryCmd() As Long
    UList.Clean
    PList.Clean
    IList.Update
    ExecFreeMemoryCmd = LineNum + 1
End Function

Private Function ExecGameClearCmd() As Long
    If ArgNum <> 1 Then
        EventErrorMessage = "GameClearコマンドの引数の数が違います"
        Error 0
    End If
    
    GameClear
End Function

Private Function ExecGameOverCmd() As Long
    If ArgNum <> 1 Then
        EventErrorMessage = "GameOverコマンドの引数の数が違います"
        Error 0
    End If
    
    GameOver
    IsScenarioFinished = True
    ExecGameOverCmd = 0
End Function

Private Function ExecGetOffCmd() As Long
Dim u As Unit

    Select Case ArgNum
        Case 1
            Set u = SelectedUnitForEvent
        Case 2
            Set u = GetArgAsUnit(2, True)
        Case Else
            EventErrorMessage = "GetOffコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not u Is Nothing Then
        With u
            If .CountPilot > 0 Then
                If .Status = "出撃" Then
                    'ユニットをマップ上から削除した状態で支援効果を更新
                    Set MapDataForUnit(.X, .Y) = Nothing
                    PList.UpdateSupportMod u
                End If
                
                'パイロットを下ろす
                .Pilot(1).GetOff True
                
                If .Status = "出撃" Then
                    'ユニットをマップ上に戻す
                    Set MapDataForUnit(.X, .Y) = u
                End If
            End If
        End With
    End If
    
    ExecGetOffCmd = LineNum + 1
End Function

Private Function ExecGlobalCmd() As Long
Dim vname As String, i As Integer

    For i = 2 To ArgNum
        vname = GetArg(i)
        If InStr(vname, """") > 0 Then
            EventErrorMessage = "変数名「" & vname & "」が不正です"
            Error 0
        End If
        If Asc(vname) = 36 Then '$
            vname = Mid$(vname, 2)
        End If
        
        If Not IsGlobalVariableDefined(vname) Then
            DefineGlobalVariable vname
        End If
    Next
    
    ExecGlobalCmd = LineNum + 1
End Function

Private Function ExecGotoCmd() As Long
    If ArgNum <> 2 Then
        EventErrorMessage = "Gotoコマンドの引数の数が違います"
        Error 0
    End If
    
    'ラベルが式でないと仮定
    ExecGotoCmd = FindLabel(GetArg(2))
    
    'ラベルが見つかった？
    If ExecGotoCmd > 0 Then
        ExecGotoCmd = ExecGotoCmd + 1
        Exit Function
    End If
    
    'ラベルは式？
    ExecGotoCmd = FindLabel(GetArgAsString(2))
    
    If ExecGotoCmd = 0 Then
        EventErrorMessage = "ラベル「" & _
            GetArg(2) & "」がみつかりません"
        Error 0
    End If
    
    ExecGotoCmd = ExecGotoCmd + 1
End Function

Private Function ExecHideCmd() As Long
    MainForm.Hide
    
    ExecHideCmd = LineNum + 1
End Function

Private Function ExecHotPointCmd() As Long
Dim hname As String, hcaption As String
Dim hx As Integer, hy As Integer
Dim hw As Integer, hh As Integer
    
    Select Case ArgNum
        Case 6
            hname = GetArgAsString(2)
            hx = GetArgAsLong(3) + BaseX
            hy = GetArgAsLong(4) + BaseY
            hw = GetArgAsLong(5)
            hh = GetArgAsLong(6)
            hcaption = hname
        Case 7
            hname = GetArgAsString(2)
            hx = GetArgAsLong(3) + BaseX
            hy = GetArgAsLong(4) + BaseY
            hw = GetArgAsLong(5)
            hh = GetArgAsLong(6)
            hcaption = GetArgAsString(7)
        Case Else
            EventErrorMessage = "HotPointコマンドの引数の数が違います"
            Error 0
    End Select
    
    ReDim Preserve HotPointList(UBound(HotPointList) + 1)
    With HotPointList(UBound(HotPointList))
        .Name = hname
        .Left = hx
        .Top = hy
        .width = hw
        .Height = hh
        .Caption = hcaption
    End With
    
    ExecHotPointCmd = LineNum + 1
End Function

Private Function ExecIfCmd() As Long
Dim expr As String
Dim i As Long, depth As Integer
Dim pname As String
Dim flag As Boolean
Dim ret As Long
    
    expr = GetArg(2)
    
    'Ifコマンドはあらかじめ構文解析されていて、第3引数に条件式の項数
    'が入っている
    Select Case GetArgAsLong(3)
        Case 1
            If PList.IsDefined(expr) Then
                With PList.Item(expr)
                    If .Unit Is Nothing Then
                        flag = False
                    Else
                        With .Unit
                            If .Status = "出撃" _
                                Or .Status = "格納" _
                            Then
                                flag = True
                            Else
                                flag = False
                            End If
                        End With
                    End If
                End With
            Else
                If GetValueAsLong(expr, True) <> 0 Then
                    flag = True
                Else
                    flag = False
                End If
            End If
        Case 2
            pname = ListIndex(expr, 2)
            If PList.IsDefined(pname) Then
                With PList.Item(pname)
                    If .Unit Is Nothing Then
                        flag = True
                    Else
                        With .Unit
                            If .Status = "出撃" _
                                Or .Status = "格納" _
                            Then
                                flag = False
                            Else
                                flag = True
                            End If
                        End With
                    End If
                End With
            Else
                If GetValueAsLong(pname, True) = 0 Then
                    flag = True
                Else
                    flag = False
                End If
            End If
        Case Else
            If GetValueAsLong(expr) <> 0 Then
                flag = True
            Else
                flag = False
            End If
    End Select
    
    Select Case GetArg(4)
        Case "exit"
            If flag Then
                ExecIfCmd = 0
            Else
                ExecIfCmd = LineNum + 1
            End If
            
        Case "goto"
            If flag Then
                ret = FindLabel(GetArg(5))
                If ret = 0 Then
                    ret = FindLabel(GetArgAsString(5))
                    If ret = 0 Then
                        EventErrorMessage = "ラベル「" & GetArg(5) _
                            & "」がみつかりません"
                        Error 0
                    End If
                End If
                ExecIfCmd = ret + 1
            Else
                ExecIfCmd = LineNum + 1
            End If
            
        Case "then"
            If flag Then
                'Then節をそのまま実行
                ExecIfCmd = LineNum + 1
                Exit Function
            End If
            
            '条件式が成り立たない場合はElse節もしくはEndIfを探す
            depth = 1
            For i = LineNum + 1 To UBound(EventCmd)
                With EventCmd(i)
                    Select Case .Name
                        Case IfCmd
                            If .GetArg(4) = "then" Then
                                depth = depth + 1
                            End If
                        Case ElseCmd
                            If depth = 1 Then
                                Exit For
                            End If
                        Case ElseIfCmd
                            If depth <> 1 Then
                                GoTo NextLoop
                            End If
                            '条件式が成り立つか判定
                            expr = .GetArg(2)
                            Select Case .GetArgAsLong(3)
                                Case 1
                                    If PList.IsDefined(expr) Then
                                        With PList.Item(expr)
                                            If .Unit Is Nothing Then
                                                flag = False
                                            Else
                                                With .Unit
                                                    If .Status = "出撃" _
                                                        Or .Status = "格納" _
                                                    Then
                                                        flag = True
                                                    Else
                                                        flag = False
                                                    End If
                                                End With
                                            End If
                                        End With
                                    Else
                                        If GetValueAsLong(expr, True) <> 0 Then
                                            flag = True
                                        Else
                                            flag = False
                                        End If
                                    End If
                                Case 2
                                    pname = ListIndex(expr, 2)
                                    If PList.IsDefined(pname) Then
                                        With PList.Item(pname)
                                            If .Unit Is Nothing Then
                                                flag = True
                                            Else
                                                With .Unit
                                                    If .Status = "出撃" _
                                                        Or .Status = "格納" _
                                                    Then
                                                        flag = False
                                                    Else
                                                        flag = True
                                                    End If
                                                End With
                                            End If
                                        End With
                                    Else
                                        If GetValueAsLong(pname, True) = 0 Then
                                            flag = True
                                        Else
                                            flag = False
                                        End If
                                    End If
                                Case Else
                                    If GetValueAsLong(expr) <> 0 Then
                                        flag = True
                                    Else
                                        flag = False
                                    End If
                            End Select
                            If flag Then
                                Exit For
                            End If
                        Case EndIfCmd
                            depth = depth - 1
                            If depth = 0 Then
                                Exit For
                            End If
                    End Select
                End With
NextLoop:
            Next
            
            If i > UBound(EventData) Then
                EventErrorMessage = "IfとEndIfが対応していません"
                Error 0
            End If
            
            ExecIfCmd = i + 1
            
        Case Else
            EventErrorMessage = "If行には Goto, Exit, Then のいずれかを指定して下さい"
            Error 0
    End Select
End Function

Private Function ExecElseCmd() As Long
Dim i As Long, depth As Integer
    
    'EndIfを探す
    depth = 1
    For i = LineNum + 1 To UBound(EventCmd)
        With EventCmd(i)
            Select Case .Name
                Case IfCmd
                    If .GetArg(4) = "then" Then
                        depth = depth + 1
                    End If
                Case EndIfCmd
                    depth = depth - 1
                    If depth = 0 Then
                        ExecElseCmd = i + 1
                        Exit Function
                    End If
            End Select
        End With
    Next
    
    EventErrorMessage = "IfとEndIfが対応していません"
    Error 0
End Function

Private Function ExecIncrCmd() As Long
Dim vname As String, buf As String, num As Double

    vname = GetArg(2)
    GetVariable vname, NumericType, buf, num
    Select Case ArgNum
        Case 3
            SetVariableAsDouble vname, num + GetArgAsDouble(3)
        Case 2
            SetVariableAsDouble vname, num + 1
        Case Else
            EventErrorMessage = "Incrコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExecIncrCmd = LineNum + 1
End Function

Private Function ExecIncreaseMoraleCmd() As Long
Dim u As Unit, num As String

    Select Case ArgNum
        Case 3
            Set u = GetArgAsUnit(2, True)
            num = GetArgAsLong(3)
        Case 2
            Set u = SelectedUnitForEvent
            num = GetArgAsLong(2)
        Case Else
            EventErrorMessage = "IncreaseMoraleコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not u Is Nothing Then
        With u
            .IncreaseMorale num, True
            .CurrentForm.CheckAutoHyperMode
            .CurrentForm.CheckAutoNormalMode
        End With
    End If
    
    ExecIncreaseMoraleCmd = LineNum + 1
End Function

Private Function ExecInputCmd() As Long
Dim str As String

    Select Case ArgNum
        Case 3
            str = InputBox(GetArgAsString(3), "SRC")
        Case 4
            str = InputBox(GetArgAsString(3), "SRC", _
                GetArgAsString(4))
        Case Else
            EventErrorMessage = "Inputコマンドの引数の数が違います"
            Error 0
    End Select
    
    SetVariableAsString GetArg(2), str
    
    ExecInputCmd = LineNum + 1
End Function

' MOD START マージ
'Private Function ExecInterMissionCommandCmd() As Long
Private Function ExecIntermissionCommandCmd() As Long
' MOD END マージ
Dim vname As String
    
    If ArgNum <> 3 Then
' MOD START マージ
'        EventErrorMessage = "InterMissionCommandコマンドの引数の数が違います"
        EventErrorMessage = "IntermissionCommandコマンドの引数の数が違います"
' MOD END マージ
        Error 0
    End If
    
' MOD START マージ
'    vname = "InterMissionCommand(" & GetArgAsString(2) & ")"
    vname = "IntermissionCommand(" & GetArgAsString(2) & ")"
' MOD END マージ
    
    If GetArg(3) = "削除" Then
        UndefineVariable vname
    Else
        If Not IsGlobalVariableDefined(vname) Then
            DefineGlobalVariable vname
        End If
        SetVariableAsString vname, GetArgAsString(3)
    End If
    
' MOD START マージ
'    ExecInterMissionCommandCmd = LineNum + 1
    ExecIntermissionCommandCmd = LineNum + 1
' MOD END マージ
End Function

Private Function ExecItemCmd() As Long
Dim iname As String
    
    Select Case ArgNum
        Case 2
            iname = GetArgAsString(2)
        Case Else
            EventErrorMessage = "Itemコマンドの引数の数が違います"
            Error 0
    End Select
    If Not IDList.IsDefined(iname) Then
        EventErrorMessage = "「" & iname & "」というアイテムは存在しません"
        Error 0
    End If
    IList.Add iname
    
    ExecItemCmd = LineNum + 1
End Function

Private Function ExecJoinCmd() As Long
Dim pname As String
Dim u As Unit
Dim i As Integer

    Select Case ArgNum
        Case 2
            pname = GetArgAsString(2)
            If PList.IsDefined(pname) Then
                Set u = PList.Item(pname).Unit
            ElseIf NPDList.IsDefined(pname) Then
                pname = "IsAway(" & NPDList.Item(pname).Name & ")"
                If IsGlobalVariableDefined(pname) Then
                    UndefineVariable pname
                End If
                ExecJoinCmd = LineNum + 1
                Exit Function
            ElseIf UList.IsDefined(pname) Then
                If pname = UList.Item(pname).ID Then
                    u = UList.Item(pname)
                Else
                    For Each u In UList
                        With u
                            If .Name = pname _
                                And .Party0 = "味方" _
                                And .CurrentForm.Status = "離脱" _
                            Then
                                Set u = .CurrentForm
                                Exit For
                            End If
                        End With
                    Next
                    If u.Name <> pname Then
                        Set u = Nothing
                    End If
                End If
            Else
                EventErrorMessage = "「" & pname & _
                    "」というパイロットまたはユニットが見つかりません"
                Error 0
            End If
        Case 1
            Set u = SelectedUnitForEvent
        Case Else
            EventErrorMessage = "Joinコマンドの引数の数が違います"
            Error 0
    End Select
    
    If u Is Nothing Then
        If PList.IsDefined(pname) Then
            PList.Item(pname).Away = False
        End If
    Else
        With u
            .Status = "待機"
            For i = 1 To .CountPilot
                .Pilot(i).Away = False
            Next
            For i = 1 To .CountSupport
                .Support(i).Away = False
            Next
        End With
    End If
    
    ExecJoinCmd = LineNum + 1
End Function

Private Function ExecKeepBGMCmd() As Long
    If ArgNum <> 1 Then
        EventErrorMessage = "KeepBGMコマンドの引数の数が違います"
        Error 0
    End If
    
    KeepBGM = True
    
    ExecKeepBGMCmd = LineNum + 1
End Function

Private Function ExecLandCmd() As Long
Dim u1 As Unit, u2 As Unit

    Select Case ArgNum
        Case 2
            Set u1 = SelectedUnitForEvent
            Set u2 = GetArgAsUnit(2)
        Case 3
            Set u1 = GetArgAsUnit(2)
            Set u2 = GetArgAsUnit(3)
        Case Else
            EventErrorMessage = "Landコマンドの引数の数が違います"
            Error 0
    End Select
    
    If u1.IsFeatureAvailable("母艦") Then
        EventErrorMessage = u1.Name & "は母艦なので格納出来ません"
        Error 0
    End If
    If Not u2.IsFeatureAvailable("母艦") Then
        EventErrorMessage = u2.Name & "は母艦能力を持っていません"
        Error 0
    End If
    
    u1.Land u2, True, True
    
    ExecLandCmd = LineNum + 1
End Function

Private Function ExecLaunchCmd() As Long
Dim u As Unit, ux As Integer, uy As Integer, num As Integer
Dim opt As String

    num = ArgNum
    
    Select Case GetArgAsString(num)
        Case "非同期"
            opt = "非同期"
            num = num - 1
        Case "アニメ非表示"
            opt = ""
            num = num - 1
        Case Else
            opt = "出撃"
    End Select
    
    Select Case num
        Case 3
            Set u = SelectedUnitForEvent
            
            ux = GetArgAsLong(2)
            If ux < 1 Then
                ux = 1
            ElseIf ux > MapWidth Then
                ux = MapWidth
            End If
            
            uy = GetArgAsLong(3)
            If uy < 1 Then
                uy = 1
            ElseIf uy > MapHeight Then
                uy = MapHeight
            End If
        Case 4
            Set u = GetArgAsUnit(2)
            
            ux = GetArgAsLong(3)
            If ux < 1 Then
                ux = 1
            ElseIf ux > MapWidth Then
                ux = MapWidth
            End If
            
            uy = GetArgAsLong(4)
            If uy < 1 Then
                uy = 1
            ElseIf uy > MapHeight Then
                uy = MapHeight
            End If
        Case Else
            EventErrorMessage = "Launchコマンドの引数の数が違います"
            Error 0
    End Select
    
    If opt <> "非同期" And MainForm.Visible And Not IsPictureVisible Then
        Center ux, uy
        RefreshScreen
    End If
    
    With u
        Select Case .Status
            Case "出撃"
                EventErrorMessage = .MainPilot.Nickname & "はすでに出撃しています"
                Error 0
            Case "離脱"
                EventErrorMessage = .MainPilot.Nickname & "はまだ離脱しています"
                Error 0
        End Select
        
        .UsedAction = 0
        .UsedSupportAttack = 0
        .UsedSupportGuard = 0
        .UsedSyncAttack = 0
        .UsedCounterAttack = 0
        
        If .HP <= 0 Then
            .HP = 1
        End If
        
        .StandBy ux, uy, opt
        
        .CheckAutoHyperMode
    End With
    
    Set SelectedUnitForEvent = u.CurrentForm
    
    ExecLaunchCmd = LineNum + 1
End Function

Private Function ExecLeaveCmd() As Long
Dim pname As String, vname As String
Dim u As Unit
Dim i As Integer, num As Integer
Dim opt As String
    
    num = ArgNum
    
    If num > 1 Then
        If GetArgAsString(num) = "非同期" Then
            opt = "非同期"
            num = num - 1
        End If
    End If
    
    Select Case num
        Case 2
            pname = GetArgAsString(2)
            If PList.IsDefined(pname) Then
                Set u = PList.Item(pname).Unit
            ElseIf NPDList.IsDefined(pname) Then
                vname = "IsAway(" & NPDList.Item(pname).Name & ")"
                If Not IsGlobalVariableDefined(vname) Then
                    DefineGlobalVariable vname
                End If
                SetVariableAsLong vname, 1
                ExecLeaveCmd = LineNum + 1
                Exit Function
            ElseIf UList.IsDefined(pname) Then
                If pname = UList.Item(pname).ID Then
                    Set u = UList.Item(pname)
                Else
                    For Each u In UList
                        If u.Name = pname _
                            And u.Party0 = "味方" _
                            And u.CurrentForm.Status <> "離脱" _
                        Then
                            Set u = u.CurrentForm
                            Exit For
                        End If
                    Next
                    If u.Name <> pname Then
                        Set u = Nothing
                    End If
                End If
            Else
                EventErrorMessage = _
                    "「" & pname & "」というパイロットまたはユニットが見つかりません"
                Error 0
            End If
        Case 1
            Set u = SelectedUnitForEvent
        Case Else
            EventErrorMessage = "Leaveコマンドの引数の数が違います"
            Error 0
    End Select
    
    If u Is Nothing Then
        PList.Item(pname).Away = True
    Else
        With u
            If .Status = "出撃" Or .Status = "格納" Then
                .Escape opt
            End If
            If .Party0 <> "味方" Then
                .ChangeParty "味方"
            End If
            If .Status <> "他形態" And .Status <> "旧主形態" And .Status <> "旧形態" Then
                .Status = "離脱"
            End If
            For i = 1 To .CountPilot
                .Pilot(i).Away = True
            Next
            For i = 1 To .CountSupport
                .Support(i).Away = True
            Next
        End With
    End If
    
    ExecLeaveCmd = LineNum + 1
End Function

Private Function ExecLevelUpCmd() As Long
Dim p As Pilot, num As Integer
Dim hp_ratio As Double, en_ratio As Double
    
    Select Case ArgNum
        Case 3
            Set p = GetArgAsPilot(2)
            num = GetArgAsLong(3)
        Case 2
            With SelectedUnitForEvent
                If .CountPilot > 0 Then
                    Set p = .Pilot(1)
                End If
            End With
            num = GetArgAsLong(2)
        Case Else
            EventErrorMessage = "LevelUpコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not p Is Nothing Then
        With p
            If Not .Unit Is Nothing Then
                With .Unit
                    hp_ratio = 100 * .HP / .MaxHP
                    en_ratio = 100 * .EN / .MaxEN
                End With
            End If
            
            If IsOptionDefined("レベル限界突破") Then
                .Level = MinLng(MaxLng(.Level + num, 1), 999)
            Else
                .Level = MinLng(MaxLng(.Level + num, 1), 99)
            End If
            
            '闘争本能入手？
            If .IsSkillAvailable("闘争本能") Then
                If .MinMorale > 100 Then
                    If .Morale = .MinMorale Then
                        .Morale = .MinMorale + 5 * .SkillLevel("闘争本能")
                    End If
                Else
                    If .Morale = 100 Then
                        .Morale = 100 + 5 * .SkillLevel("闘争本能")
                    End If
                End If
            End If
            
            'ＳＰ＆霊力をアップデート
            .SP = .SP
            .Plana = .Plana
            
            If Not .Unit Is Nothing Then
                With .Unit
                    .Update
                    .HP = .MaxHP * hp_ratio / 100
                    .EN = .MaxEN * en_ratio / 100
                End With
                PList.UpdateSupportMod .Unit
            End If
        End With
    End If
    
    ExecLevelUpCmd = LineNum + 1
End Function

Private Function ExecLineCmd() As Long
Dim pic As PictureBox, pic2 As PictureBox
Dim x1 As Integer, y1 As Integer
Dim x2 As Integer, y2 As Integer
Dim opt As String, dtype As String
Dim cname As String, clr As Long
Dim i As Integer

    If ArgNum < 5 Then
        EventErrorMessage = "Lineコマンドの引数の数が違います"
        Error 0
    End If
    
    x1 = GetArgAsLong(2) + BaseX
    y1 = GetArgAsLong(3) + BaseY
    x2 = GetArgAsLong(4) + BaseX
    y2 = GetArgAsLong(5) + BaseY
    
    SaveScreen
    
    '描画先
    Select Case ObjDrawOption
        Case "背景"
            Set pic = MainForm.picBack
            Set pic2 = MainForm.picMaskedBack
            IsMapDirty = True
        Case "保持"
            Set pic = MainForm.picMain(0)
            Set pic2 = MainForm.picMain(1)
            IsPictureVisible = True
        Case Else
            Set pic = MainForm.picMain(0)
    End Select
    
    '描画領域
    If ObjDrawOption <> "背景" Then
        Dim tmp As Integer
        IsPictureVisible = True
        tmp = ObjDrawWidth - 1
        PaintedAreaX1 = MaxLng(MinLng(PaintedAreaX1, MinLng(x1 - tmp, x2 - tmp)), 0)
        PaintedAreaY1 = MaxLng(MinLng(PaintedAreaY1, MinLng(y1 - tmp, y2 - tmp)), 0)
        PaintedAreaX2 = MaxLng(MinLng(PaintedAreaX2, MinLng(x1 + tmp, x2 + tmp)), MapPWidth - 1)
        PaintedAreaY2 = MaxLng(MinLng(PaintedAreaY2, MinLng(y1 + tmp, y2 + tmp)), MapPHeight - 1)
    End If
    
    clr = ObjColor
    For i = 6 To ArgNum
        opt = GetArgAsString(i)
        If Asc(opt) = 35 Then '#
            If Len(opt) <> 7 Then
                EventErrorMessage = "色指定が不正です"
                Error 0
            End If
            cname = String$(8, vbNullChar)
            Mid(cname, 1, 2) = "&H"
            Mid(cname, 3, 2) = Mid$(opt, 6, 2)
            Mid(cname, 5, 2) = Mid$(opt, 4, 2)
            Mid(cname, 7, 2) = Mid$(opt, 2, 2)
            If Not IsNumeric(cname) Then
                EventErrorMessage = "色指定が不正です"
                Error 0
            End If
            clr = CLng(cname)
        Else
            If opt <> "B" And opt <> "BF" Then
                EventErrorMessage = _
                    "Lineコマンドに不正なオプション「" & opt & "」が使われています"
                Error 0
            End If
            dtype = opt
        End If
    Next
    
    With pic
        .DrawWidth = ObjDrawWidth
        .FillColor = ObjFillColor
        .FillStyle = ObjFillStyle
    End With
    
    Select Case dtype
        Case "B"
            pic.Line (x1, y1)-(x2, y2), clr, B
        Case "BF"
            pic.Line (x1, y1)-(x2, y2), clr, BF
        Case Else
            pic.Line (x1, y1)-(x2, y2), clr
    End Select
    
    With pic
        .DrawWidth = 1
        .FillColor = vbWhite
        .FillStyle = vbFSTransparent
    End With
    
    If Not pic2 Is Nothing Then
        With pic2
            .DrawWidth = ObjDrawWidth
            .FillColor = ObjFillColor
            .FillStyle = ObjFillStyle
        End With
        
        Select Case dtype
            Case "B"
                pic2.Line (x1, y1)-(x2, y2), clr, B
            Case "BF"
                pic2.Line (x1, y1)-(x2, y2), clr, BF
            Case Else
                pic2.Line (x1, y1)-(x2, y2), clr
        End Select
        
        With pic2
            .DrawWidth = 1
            .FillColor = vbWhite
            .FillStyle = vbFSTransparent
        End With
    End If
    
    ExecLineCmd = LineNum + 1
End Function

Private Function ExecLineReadCmd() As Long
Dim buf As String

    If ArgNum <> 3 Then
        EventErrorMessage = "LineReadコマンドの引数の数が違います"
        Error 0
    End If
    
    Line Input #GetArgAsLong(2), buf
    SetVariableAsString GetArg(3), buf
    
    ExecLineReadCmd = LineNum + 1
End Function

Private Function ExecLoadCmd() As Long
Dim new_titles() As String, tname As String, tfolder As String
Dim i As Long, j As Integer
Dim cur_data_size As Long
Dim flag As Boolean

    ReDim new_titles(0)
    For i = 2 To ArgNum
        tname = GetArgAsString(i)
        flag = False
        For j = 1 To UBound(Titles)
            If tname = Titles(j) Then
                flag = True
                Exit For
            End If
        Next
        If Not flag Then
            ReDim Preserve new_titles(UBound(new_titles) + 1)
            ReDim Preserve Titles(UBound(Titles) + 1)
            new_titles(UBound(new_titles)) = tname
            Titles(UBound(Titles)) = tname
        End If
    Next
    
    '新規のデータがなかった？
    If UBound(new_titles) = 0 Then
        ExecLoadCmd = LineNum + 1
        Exit Function
    End If
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    cur_data_size = UBound(EventData)
    
    '使用しているタイトルのデータをロード
    For i = 1 To UBound(new_titles)
        IncludeData new_titles(i)
        tfolder = SearchDataFolder(new_titles(i))
        If FileExists(tfolder & "\include.eve") Then
            LoadEventData2 tfolder & "\include.eve", UBound(EventData)
        End If
    Next
    
    'ローカルデータの読みこみ
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
    
    If FileExists(ScenarioPath & "Data\pilot_message.txt") Then
        MDList.Load ScenarioPath & "Data\pilot_message.txt"
    End If
    If FileExists(ScenarioPath & "Data\pilot_dialog.txt") Then
        DDList.Load ScenarioPath & "Data\pilot_dialog.txt"
    End If
    If FileExists(ScenarioPath & "Data\item.txt") Then
        IDList.Load ScenarioPath & "Data\item.txt"
    End If
    
    For i = cur_data_size + 1 To UBound(EventData)
        '複数行に分割されたコマンドを結合
        If Right$(EventData(i), 1) = "_" Then
            If UBound(EventData) > i Then
                EventData(i + 1) = Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
                EventData(i) = " "
            End If
        End If
    Next
    
    'ラベルの登録
    For i = cur_data_size + 1 To UBound(EventData)
        If Right$(EventData(i), 1) = ":" Then
            AddLabel Left$(EventData(i), Len(EventData(i)) - 1), i
        End If
    Next
    
    'コマンドデータ配列を増やす
    If UBound(EventData) > UBound(EventCmd) Then
        ReDim Preserve EventCmd(UBound(EventData))
    End If
    
    'イベントデータの構文解析
    For i = cur_data_size + 1 To UBound(EventData)
        If EventCmd(i) Is Nothing Then
            Set EventCmd(i) = New CmdData
        End If
        With EventCmd(i)
            .LineNum = i
            .Parse EventData(i)
        End With
    Next
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ExecLoadCmd = LineNum + 1
End Function

Private Function ExecLocalCmd() As Long
Dim vname As String, i As Integer
Dim etype As ValueType, str_result As String, num_result As Double
    
    '代入式付きの変数定義？
    If ArgNum >= 4 Then
        If GetArg(3) = "=" Then
            If VarIndex >= MaxVarIndex Then
                VarIndex = MaxVarIndex
                EventErrorMessage = Format$(MaxVarIndex) & _
                    "個を超えるサブルーチンローカル変数は作成できません"
                Error 0
            End If
            
            vname = GetArg(2)
            If InStr(vname, """") > 0 Then
                EventErrorMessage = "変数名「" & vname & "」が不正です"
                Error 0
            End If
            If Asc(vname) = 36 Then '$
                vname = Mid$(vname, 2)
            End If
            
            If ArgNum = 4 Then
                Select Case ArgsType(4)
                    Case UndefinedType
                        etype = EvalTerm(strArgs(4), UndefinedType, str_result, num_result)
                        VarIndex = VarIndex + 1
                        With VarStack(VarIndex)
                            .Name = vname
                            .VariableType = etype
                            .StringValue = str_result
                            .NumericValue = num_result
                        End With
                    Case StringType
                        VarIndex = VarIndex + 1
                        With VarStack(VarIndex)
                            .Name = vname
                            .VariableType = StringType
                            .StringValue = strArgs(4)
                            .NumericValue = num_result
                        End With
                    Case NumericType
                        VarIndex = VarIndex + 1
                        With VarStack(VarIndex)
                            .Name = vname
                            .VariableType = NumericType
                            .StringValue = str_result
                            .NumericValue = dblArgs(4)
                        End With
                End Select
            Else
                etype = EvalTerm(strArgs(4), UndefinedType, str_result, num_result)
                
                VarIndex = VarIndex + 1
                With VarStack(VarIndex)
                    .Name = vname
                    .VariableType = NumericType
                    .StringValue = str_result
                    .NumericValue = dblArgs(4)
                End With
            End If
            
            ExecLocalCmd = LineNum + 1
            Exit Function
        End If
    End If
    
    VarIndex = VarIndex + ArgNum - 1
    
    If VarIndex > MaxVarIndex Then
        VarIndex = MaxVarIndex
        EventErrorMessage = Format$(MaxVarIndex) & _
            "個を超えるサブルーチンローカル変数は作成できません"
        Error 0
    End If
    
    For i = 2 To ArgNum
        With VarStack(VarIndex - i + 2)
            vname = GetArg(i)
            If InStr(vname, """") > 0 Then
                EventErrorMessage = "変数名「" & vname & "」が不正です"
                Error 0
            End If
            If Asc(vname) = 36 Then '$
                vname = Mid$(vname, 2)
            End If
            
            .Name = vname
            .VariableType = StringType
            .StringValue = ""
        End With
    Next
    
    ExecLocalCmd = LineNum + 1
End Function

Private Function ExecMakePilotListCmd() As Long
Dim u As Unit, p As Pilot
Dim xx As Integer, yy As Integer
Dim key_type As String, key_list() As Long, strkey_list() As String
Dim max_item As Integer, max_value As Long, max_str As String
Dim pilot_list() As Pilot
Dim i As Integer, j As Integer
Dim buf As String
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    'パイロットがどのユニットに乗っていたか記録しておく
    For Each u In UList
        With u
            If .Status = "出撃" Then
                'あらかじめ撤退させておく
                .Escape "非同期"
            End If
            If .Status = "待機" Then
                If InStr(.Name, "ステータス表示用") = 0 Then
                    For i = 1 To .CountPilot
                        SetVariableAsString "搭乗ユニット[" & .Pilot(i).ID & "]", .ID
                    Next
                    For i = 1 To .CountSupport
                        SetVariableAsString "搭乗ユニット[" & .Support(i).ID & "]", .ID
                    Next
                End If
            End If
        End With
    Next
    
    'マップをクリア
    LoadMapData ""
    SetupBackground "", "ステータス"
    
    'ユニット一覧を作成
    key_type = GetArgAsString(2)
    If key_type <> "名称" Then
        '配列作成
        ReDim pilot_list(PList.Count)
        ReDim key_list(PList.Count)
        i = 0
        For Each p In PList
            With p
                If Not .Alive Or .Away Then
                    GoTo NextPilot1
                End If
                
                If Not .Unit Is Nothing Then
                    If .IsAdditionalPilot Then
                        '追加パイロットは勘定に入れない
                        GoTo NextPilot1
                    End If
                    If .IsAdditionalSupport Then
                        '追加サポートは勘定に入れない
                        GoTo NextPilot1
                    End If
                End If
                
                i = i + 1
                Set pilot_list(i) = p
                Select Case key_type
                    Case "レベル"
                        key_list(i) = .Level
                    Case "ＳＰ"
                        key_list(i) = .MaxSP
                    Case "格闘"
                        key_list(i) = .Infight
                    Case "射撃"
                        key_list(i) = .Shooting
                    Case "命中"
                        key_list(i) = .Hit
                    Case "回避"
                        key_list(i) = .Dodge
                    Case "技量"
                        key_list(i) = .Technique
                    Case "反応"
                        key_list(i) = .Intuition
                End Select
            End With
NextPilot1:
        Next
        ReDim Preserve pilot_list(i)
        ReDim Preserve key_list(i)
        
        'ソート
        For i = 1 To UBound(pilot_list) - 1
            max_item = i
            max_value = key_list(i)
            For j = i + 1 To UBound(pilot_list)
                If key_list(j) > max_value Then
                    max_item = j
                    max_value = key_list(j)
                End If
            Next
            If max_item <> i Then
                Set p = pilot_list(i)
                Set pilot_list(i) = pilot_list(max_item)
                Set pilot_list(max_item) = p
                
                max_value = key_list(max_item)
                key_list(max_item) = key_list(i)
                key_list(i) = max_value
            End If
        Next
    Else
        '配列作成
        ReDim pilot_list(PList.Count)
        ReDim strkey_list(PList.Count)
        i = 0
        For Each p In PList
            With p
                If Not .Alive Or .Away Then
                    GoTo NextPilot2
                End If
                
                If Not .Unit Is Nothing Then
                    If .Name = .Unit.FeatureData("追加パイロット") Then
                        '追加パイロットは勘定に入れない
                        GoTo NextPilot2
                    End If
                End If
                
                i = i + 1
                Set pilot_list(i) = p
                strkey_list(i) = p.KanaName
            End With
NextPilot2:
        Next
        ReDim Preserve pilot_list(i)
        ReDim Preserve strkey_list(i)
        
        'ソート
        For i = 1 To UBound(pilot_list) - 1
            max_item = i
            max_str = strkey_list(max_item)
            For j = i + 1 To UBound(pilot_list)
                If StrComp(strkey_list(j), max_str, 1) = -1 Then
                    max_item = j
                    max_str = strkey_list(j)
                End If
            Next
            If max_item <> i Then
                Set p = pilot_list(i)
                Set pilot_list(i) = pilot_list(max_item)
                Set pilot_list(max_item) = p
                
                strkey_list(max_item) = strkey_list(i)
            End If
        Next
    End If
    
    'Font Regular 9pt 背景
    With MainForm.picMain(0).Font
        .Size = 9
        .Bold = False
        .Italic = False
    End With
    PermanentStringMode = True
    HCentering = False
    VCentering = False
    
    xx = 1
    yy = 1
    For i = 1 To UBound(pilot_list)
        Set p = pilot_list(i)
        With p
            'ユニット出撃位置を折り返す
            If xx > 15 Then
                xx = 1
                yy = yy + 1
                If yy > 40 Then
                    'パイロット数が多すぎるため、一部のパイロットが表示出来ません
                    Exit For
                End If
            End If
            
            'ダミーユニットに載せる
            If .Unit Is Nothing Then
                If UDList.IsDefined(.Name & "ステータス表示用ユニット") Then
                    Set u = UList.Add(.Name & "ステータス表示用ユニット", 0, "味方")
                Else
                    Set u = UList.Add("ステータス表示用ダミーユニット", 0, "味方")
                End If
                .Ride u
            ElseIf Not .Unit.IsFeatureAvailable("ダミーユニット") Then
                .GetOff
                If UDList.IsDefined(.Name & "ステータス表示用ユニット") Then
                    Set u = UList.Add(.Name & "ステータス表示用ユニット", 0, "味方")
                Else
                    Set u = UList.Add("ステータス表示用ダミーユニット", 0, "味方")
                End If
                .Ride u
            Else
                Set u = .Unit
            End If
            
            '出撃
            u.UsedAction = 0
            u.StandBy xx, yy, "非同期"
            
            'プレイヤーが操作できないように
            u.AddCondition "非操作", -1
            
            'パイロットの愛称を表示
            DrawString .Nickname, 32 * xx + 2, 32 * yy - 31
            
            Select Case key_type
                Case "レベル", "名称"
                    DrawString "Lv" & Format$(.Level), _
                        32 * xx + 2, 32 * yy - 15
                Case "ＳＰ"
                    DrawString Term("SP", u) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "格闘"
                    DrawString Left$(Term("格闘", u), 1) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "射撃"
                    If .HasMana() Then
                        DrawString Left$(Term("魔力", u), 1) & Format$(key_list(i)), _
                            32 * xx + 2, 32 * yy - 15
                    Else
                        DrawString Left$(Term("射撃", u), 1) & Format$(key_list(i)), _
                            32 * xx + 2, 32 * yy - 15
                    End If
                Case "命中"
                    DrawString Left$(Term("命中", u), 1) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "回避"
                    DrawString Left$(Term("回避", u), 1) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "技量"
                    DrawString Left$(Term("技量", u), 1) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
                Case "反応"
                    DrawString Left$(Term("反応", u), 1) & Format$(key_list(i)), _
                        32 * xx + 2, 32 * yy - 15
            End Select
            
            '表示位置を右に3マスずらす
            xx = xx + 3
        End With
    Next
    
    'フォントの設定を戻しておく
    With MainForm.picMain(0).Font
        .Size = 16
        .Bold = True
        .Italic = False
    End With
    PermanentStringMode = False
    
    RedrawScreen
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ExecMakePilotListCmd = LineNum + 1
End Function

Private Function ExecMakeUnitListCmd() As Long
    'ユニット一覧を作成
    MakeUnitList GetArgAsString(2)
    ExecMakeUnitListCmd = LineNum + 1
End Function

Private Function ExecMapAbilityCmd() As Long
Dim u As Unit
Dim tx As Integer, ty As Integer
Dim a As Integer

    Select Case ArgNum
        Case 5
            Set u = GetArgAsUnit(2)
            
            With u
                For a = 1 To .CountAbility
                    If GetArgAsString(3) = .Ability(a).Name _
                        And .IsAbilityClassifiedAs(a, "Ｍ") _
                    Then
                        Exit For
                    End If
                Next
                If a > .CountAbility Then
                    EventErrorMessage = "アビリティ名が間違っています"
                    Error 0
                End If
            End With
            
            tx = GetArgAsLong(4)
            If tx < 1 Then
                tx = 1
            ElseIf tx > MapWidth Then
                tx = MapWidth
            End If
            
            ty = GetArgAsLong(5)
            If ty < 1 Then
                ty = 1
            ElseIf ty > MapHeight Then
                ty = MapHeight
            End If
        Case 4
            Set u = SelectedUnitForEvent
            
            With u
                For a = 1 To .CountAbility
                    If GetArgAsString(2) = .Ability(a).Name _
                        And .IsAbilityClassifiedAs(a, "Ｍ") Then
                        Exit For
                    End If
                Next
                If a > .CountAbility Then
                    EventErrorMessage = "アビリティ名が間違っています"
                    Error 0
                End If
            End With
            
            tx = GetArgAsLong(3)
            If tx < 1 Then
                tx = 1
            ElseIf tx > MapWidth Then
                tx = MapWidth
            End If
            
            ty = GetArgAsLong(4)
            If ty < 1 Then
                ty = 1
            ElseIf ty > MapHeight Then
                ty = MapHeight
            End If
        Case Else
            EventErrorMessage = "MapAbilityコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        If .Status <> "出撃" Then
            EventErrorMessage = .Nickname & "は出撃していません"
            Error 0
        End If
        OpenMessageForm
        .ExecuteMapAbility a, tx, ty, True
        CloseMessageForm
    End With
    
    RedrawScreen
    
    ExecMapAbilityCmd = LineNum + 1
End Function

Private Function ExecMapAttackCmd() As Long
Dim u As Unit
Dim tx As Integer, ty As Integer
Dim w As Integer
Dim prev_w As Integer, prev_tw As Integer
Dim cur_stage As String
Dim is_event As Boolean
Dim num As Integer

    num = ArgNum
    is_event = True
    
    If num <= 6 Then
        If GetArgAsString(num) = "通常戦闘" Then
            is_event = False
            num = num - 1
        End If
    End If
    
    Select Case num
        Case 5
            Set u = GetArgAsUnit(2)
            
            With u
                For w = 1 To .CountWeapon
                    If GetArgAsString(3) = .Weapon(w).Name _
                        And .IsWeaponClassifiedAs(w, "Ｍ") _
                    Then
                        Exit For
                    End If
                Next
                If w > .CountWeapon Then
                    EventErrorMessage = "マップ攻撃名が間違っています"
                    Error 0
                End If
            End With
            
            tx = GetArgAsLong(4)
            If tx < 1 Then
                tx = 1
            ElseIf tx > MapWidth Then
                tx = MapWidth
            End If
            
            ty = GetArgAsLong(5)
            If ty < 1 Then
                ty = 1
            ElseIf ty > MapHeight Then
                ty = MapHeight
            End If
        Case 4
            Set u = SelectedUnitForEvent
            
            With u
                For w = 1 To u.CountWeapon
                    If GetArgAsString(2) = .Weapon(w).Name _
                        And .IsWeaponClassifiedAs(w, "Ｍ") _
                    Then
                        Exit For
                    End If
                Next
                If w > .CountWeapon Then
                    EventErrorMessage = "マップ攻撃名が間違っています"
                    Error 0
                End If
            End With
            
            tx = GetArgAsLong(3)
            If tx < 1 Then
                tx = 1
            ElseIf tx > MapWidth Then
                tx = MapWidth
            End If
            
            ty = GetArgAsLong(4)
            If ty < 1 Then
                ty = 1
            ElseIf ty > MapHeight Then
                ty = MapHeight
            End If
        Case Else
            EventErrorMessage = "MapAttackコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        If .Status <> "出撃" Then
            EventErrorMessage = .Nickname & "は出撃していません"
            Error 0
        End If
        
        'ステージを仮想的に変更しておく
        cur_stage = Stage
        Stage = .Party
        
        prev_w = SelectedWeapon
        prev_tw = SelectedTWeapon
        SelectedWeapon = w
        SelectedTWeapon = 0
        SelectedX = tx
        SelectedY = ty
        
        .MapAttack w, tx, ty, is_event
        
        SelectedWeapon = prev_w
        SelectedTWeapon = prev_tw
        
        Stage = cur_stage
    End With
    
    RedrawScreen
    
    ExecMapAttackCmd = LineNum + 1
End Function

Private Function ExecMoneyCmd() As Long
    If ArgNum <> 2 Then
        EventErrorMessage = "Moneyコマンドの引数の数が間違っています"
        Error 0
    End If
    
    IncrMoney GetArgAsLong(2)
    
    ExecMoneyCmd = LineNum + 1
End Function

Private Function ExecMonotoneCmd() As Long
Dim prev_x As Integer, prev_y As Integer
Dim u As Unit
Dim late_refresh As Boolean
Dim i As Integer, buf As String
    
    late_refresh = False
    MapDrawIsMapOnly = False
    For i = 2 To ArgNum
        buf = GetArgAsString(i)
        Select Case buf
            Case "非同期"
                late_refresh = True
            Case "マップ限定"
                MapDrawIsMapOnly = True
            Case Else
                EventErrorMessage = _
                    "Monotoneコマンドに不正なオプション「" & buf & "」が使われています"
                Error 0
        End Select
    Next
    
    prev_x = MapX
    prev_y = MapY
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    SetupBackground "白黒", "非同期"
    
    For Each u In UList
        With u
            If .Status = "出撃" Then
                If .BitmapID = 0 Then
                    With UList.Item(.Name)
                        If u.Party0 = .Party0 And .BitmapID <> 0 _
                            And u.Bitmap = .Bitmap _
                            And Not .IsFeatureAvailable("ダミーユニット") _
                        Then
                            u.BitmapID = .BitmapID
                        Else
                            u.BitmapID = MakeUnitBitmap(u)
                        End If
                    End With
                End If
            End If
        End With
    Next
    
    Center prev_x, prev_y
    RedrawScreen late_refresh
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ExecMonotoneCmd = LineNum + 1
End Function

Private Function ExecMoveCmd() As Long
Dim u As Unit
Dim ux As Integer, uy As Integer
Dim tx As Integer, ty As Integer
Dim opt As String
Dim idx As Integer

    If Not IsNumeric(GetArgAsString(2)) Then
        idx = 3
        Set u = GetArgAsUnit(2)
    Else
        idx = 2
        Set u = SelectedUnitForEvent
    End If
    
    tx = GetArgAsLong(idx)
    If tx < 1 Then
        tx = 1
    ElseIf tx > MapWidth Then
        tx = MapWidth
    End If
    
    idx = idx + 1
    ty = GetArgAsLong(idx)
    If ty < 1 Then
        ty = 1
    ElseIf ty > MapHeight Then
        ty = MapHeight
    End If
    
    idx = idx + 1
    If idx <= ArgNum Then
        opt = GetArgAsString(idx)
    End If
    
    With u
        Select Case u.Status
            Case "出撃"
                If InStr(opt, "アニメ表示") = 1 Then
                     '現在位置を記録
                     ux = .X
                     uy = .Y
                     
                     '目的地にユニットがいて入れない場合があるので
                     '実際に移動させて到着地点を確かめる
                     .Jump tx, ty, False
                     tx = .X
                     ty = .Y
                     
                     '一旦元の位置に戻す
                     .Jump ux, uy, False
                     
                     '移動アニメ表示
                     MoveUnitBitmap u, ux, uy, tx, ty, 20
                End If
                
                .Jump tx, ty, False
            Case "格納"
                .StandBy tx, ty, opt
            Case Else
                EventErrorMessage = .MainPilot.Nickname & "は出撃していません"
                Error 0
        End Select
    End With
    
    If opt = "" Or InStr(opt, "アニメ表示") = 1 Then
        If MainForm.Visible And Not IsPictureVisible Then
            RedrawScreen
        End If
    ElseIf opt = "非同期" Then
        '画面更新しない
    Else
        EventErrorMessage = "Moveコマンドの引数の数が違います"
        Error 0
    End If
    
    ExecMoveCmd = LineNum + 1
End Function

Private Function ExecNightCmd() As Long
Dim prev_x As Integer, prev_y As Integer
Dim u As Unit
Dim late_refresh As Boolean
Dim i As Integer, buf As String
    
    late_refresh = False
    MapDrawIsMapOnly = False
    For i = 2 To ArgNum
        buf = GetArgAsString(i)
        Select Case buf
            Case "非同期"
                late_refresh = True
            Case "マップ限定"
                MapDrawIsMapOnly = True
            Case Else
                EventErrorMessage = _
                    "Nightコマンドに不正なオプション「" & buf & "」が使われています"
                Error 0
        End Select
    Next
    
    prev_x = MapX
    prev_y = MapY
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    SetupBackground "夜", "非同期"
    
    For Each u In UList
        With u
            If .Status = "出撃" Then
                If .BitmapID = 0 Then
                    With UList.Item(.Name)
                        If u.Party0 = .Party0 And .BitmapID <> 0 _
                            And u.Bitmap = .Bitmap _
                            And Not .IsFeatureAvailable("ダミーユニット") _
                        Then
                            u.BitmapID = .BitmapID
                        Else
                            u.BitmapID = MakeUnitBitmap(u)
                        End If
                    End With
                End If
            End If
        End With
    Next
    
    Center prev_x, prev_y
    RedrawScreen late_refresh
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ExecNightCmd = LineNum + 1
End Function

Private Function ExecNoonCmd() As Long
Dim prev_x As Integer, prev_y As Integer
Dim u As Unit
Dim late_refresh As Boolean
    
    Select Case ArgNum
        Case 1
            'ＯＫ
        Case 2
             If GetArgAsString(2) = "非同期" Then
                 late_refresh = True
             Else
                 EventErrorMessage = "Noonコマンドのオプションが不正です"
                 Error 0
             End If
        Case Else
            EventErrorMessage = "Noonコマンドの引数の数が違います"
            Error 0
    End Select
    
    MapDrawIsMapOnly = False
    
    prev_x = MapX
    prev_y = MapY
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    SetupBackground "", "非同期"
    
    For Each u In UList
        With u
            If .Status = "出撃" Then
                If .BitmapID = 0 Then
                    With UList.Item(.Name)
                        If u.Party0 = .Party0 And .BitmapID <> 0 _
                            And u.Bitmap = .Bitmap _
                            And Not .IsFeatureAvailable("ダミーユニット") _
                        Then
                            u.BitmapID = .BitmapID
                        Else
                            u.BitmapID = MakeUnitBitmap(u)
                        End If
                    End With
                End If
            End If
        End With
    Next
    
    Center prev_x, prev_y
    RedrawScreen late_refresh
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ExecNoonCmd = LineNum + 1
End Function

Private Function ExecOpenCmd() As Long
Dim fname As String
Dim vname As String
Dim opt As String
Dim f As Integer

    If ArgNum <> 6 Then
        EventErrorMessage = "Openコマンドの引数の数が違います"
        Error 0
    End If
    
    fname = ScenarioPath & GetArgAsString(2)
    
    If InStr(fname, "..\") > 0 Then
        EventErrorMessage = "ファイル指定に「..\」は使えません"
        Error 0
    End If
    If InStr(fname, "../") > 0 Then
        EventErrorMessage = "ファイル指定に「../」は使えません"
        Error 0
    End If
        
    opt = GetArgAsString(4)
    vname = GetArg(6)
    
    f = FreeFile
    SetVariableAsLong vname, f
    Select Case opt
        Case "出力"
            Open fname For Output Access Write As #f
        Case "追加出力"
            Open fname For Append Access Write As #f
        Case "入力"
            If Not FileExists(fname) Then
                EventErrorMessage = fname & "というファイルは存在しません"
                Error 0
            End If
            Open fname For Input Access Read As #f
        Case Else
            EventErrorMessage = "ファイルの入出力モードが不正です"
            Error 0
    End Select
    
    ExecOpenCmd = LineNum + 1
End Function

Private Function ExecOptionCmd() As Long
Dim vname As String

    Select Case ArgNum
        Case 2
            vname = GetArgAsString(2)
            vname = "Option(" & vname & ")"
            If Not IsGlobalVariableDefined(vname) Then
                DefineGlobalVariable vname
            End If
            SetVariableAsLong vname, 1
' ADD START MARGE
            If vname = "Option(新ＧＵＩ)" Then
                ' 新ＧＵＩが指定されたら即反映するためにメイン画面をロードしなおす
                LoadForms
            End If
' ADD END MARGE
        Case 3
            vname = GetArgAsString(2)
            vname = "Option(" & vname & ")"
            If IsGlobalVariableDefined(vname) Then
                UndefineVariable vname
            End If
        Case Else
            EventErrorMessage = "Optionコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExecOptionCmd = LineNum + 1
End Function

Private Function ExecOrganizeCmd() As Long
Dim unum As Integer
Dim u As Unit
Dim ux As Integer, uy As Integer
Dim uclass As String
Dim buf As String, opt As String
Dim i As Integer, j As Integer, num As Integer, tmp As Long
Dim min_value As Long
Dim max_item As Integer, max_value As Long
Dim lv_list() As Long
Dim list() As String
Dim ret As Integer
Dim without_refresh As Boolean
Dim without_animation As Boolean
    
    num = ArgNum
    
    For i = 5 To num
' MOD START MARGE
'        Select Case GetArgAsString(num)
        Select Case GetArgAsString(i)
' MOD END MARGE
            Case "密集"
                opt = opt & " 出撃"
'                num = num - 1
            Case "非同期"
                opt = opt & " 非同期"
'                num = num - 1
            Case "アニメ非表示"
                opt = opt & " アニメ非表示"
'                num = num - 1
        End Select
    Next
' MOD START MARGE
'    If InStr(opt, "出撃") = 0 Then
    If InStr(opt, "出撃") <= 0 Then
' MOD END MARGE
        opt = opt & " 部隊配置"
    End If
    
    If num < 4 Then
        EventErrorMessage = "Organizeコマンドの引数の数が違います"
        Error 0
    End If
    
    unum = GetArgAsLong(2)
    If unum < 1 Then
        EventErrorMessage = "ユニット数が不正です"
        Error 0
    End If
    
    ux = GetArgAsLong(3)
    If ux < 1 Then
        ux = 1
    ElseIf ux > MapWidth Then
        ux = MapWidth
    End If
    
    uy = GetArgAsLong(4)
    If uy < 1 Then
        uy = 1
    ElseIf uy > MapHeight Then
        uy = MapHeight
    End If
    
    If num < 5 Then
        uclass = "全て"
    Else
        For i = 5 To num
            uclass = uclass & " " & GetArgAsString(i)
        Next
        uclass = Trim$(uclass)
    End If
    
Beginning:
    
    ReDim list(0)
    ReDim ListItemID(0)
    For Each u In UList
        With u
            If .Party0 <> "味方" Or .Status <> "待機" _
                Or .CountPilot = 0 _
            Then
                GoTo NextOrganizeLoop
            End If
            
            'パイロット数のチェック
            If (.Data.PilotNum = 1 Or Abs(.Data.PilotNum) = 2) _
                And .CountPilot < Abs(.Data.PilotNum) _
                And Not .IsFeatureAvailable("１人乗り可能") _
            Then
                GoTo NextOrganizeLoop
            End If
            
            Select Case TerrainClass(1, 1)
                Case "宇宙", "月面"
                    If .Adaption(4) = 0 Then
                        GoTo NextOrganizeLoop
                    End If
                Case Else
                    '宇宙専用ユニットは宇宙でしか活動できない
                    If .Transportation = "宇宙" Then
                        GoTo NextOrganizeLoop
                    End If
                    
                    '空中マップか？
                    If TerrainName(1, 1) = "空" _
                        And TerrainName(MapWidth \ 2, MapHeight \ 2) = "空" _
                        And TerrainName(MapWidth, MapHeight) = "空" _
                    Then
                        If Not .IsTransAvailable("空") Then
                            GoTo NextOrganizeLoop
                        End If
                    End If
            End Select
            
            Select Case uclass
                Case "全て", ""
                    '全てのユニット
                Case "通常ユニット"
                    If .IsFeatureAvailable("母艦") Then
                        GoTo NextOrganizeLoop
                    End If
                Case "母艦ユニット"
                    If Not .IsFeatureAvailable("母艦") Then
                        GoTo NextOrganizeLoop
                    End If
                Case "LL"
                    If .Size = "XL" Then
                        GoTo NextOrganizeLoop
                    End If
                Case "L"
                    If .Size = "XL" _
                        Or .Size = "LL" _
                    Then
                        GoTo NextOrganizeLoop
                    End If
                Case "M"
                    If .Size = "XL" _
                        Or .Size = "LL" _
                        Or .Size = "L" _
                    Then
                        GoTo NextOrganizeLoop
                    End If
                Case "S"
                    If .Size = "XL" _
                        Or .Size = "LL" _
                        Or .Size = "L" _
                        Or .Size = "M" _
                    Then
                        GoTo NextOrganizeLoop
                    End If
                Case "SS"
                    If .Size = "XL" _
                        Or .Size = "LL" _
                        Or .Size = "L" _
                        Or .Size = "M" _
                        Or .Size = "S" _
                    Then
                        GoTo NextOrganizeLoop
                    End If
                Case Else
                    'ユニットクラス指定した場合
                    
                    '指定されたクラスに該当するか
                    For i = 1 To ListLength(uclass)
                        If ListIndex(uclass, i) = .Class0 Then
                            Exit For
                        End If
                    Next
                    If i > ListLength(uclass) Then
                        GoTo NextOrganizeLoop
                    End If
            End Select
            
            ReDim Preserve list(UBound(list) + 1)
            ReDim Preserve ListItemID(UBound(list))
            If IsOptionDefined("等身大基準") Then
                list(UBound(list)) = _
                    .Nickname0 & _
                    Space$(MaxLng(52 - LenB(StrConv(.Nickname0, vbFromUnicode)), 1)) & _
                    LeftPaddedString(.MainPilot.Level, 2)
            Else
                list(UBound(list)) = _
                    .Nickname0 & _
                    Space$(MaxLng(36 - LenB(StrConv(.Nickname0, vbFromUnicode)), 1)) & _
                    .MainPilot.Nickname & _
                    Space$(MaxLng(17 - LenB(StrConv(.MainPilot.Nickname, vbFromUnicode)), 1)) & _
                    LeftPaddedString(.MainPilot.Level, 2)
            End If
            ListItemID(UBound(list)) = .ID
        End With
NextOrganizeLoop:
    Next
    ReDim ListItemFlag(UBound(list))
    
    'レベルの一覧と最大値・最小値を求める
    ReDim lv_list(UBound(list))
    min_value = 100000
    max_value = 0
    For i = 1 To UBound(list)
        With UList.Item(ListItemID(i)).MainPilot
            lv_list(i) = 500 * CLng(.Level) + CLng(.Exp)
        End With
        If lv_list(i) > max_value Then
            max_value = lv_list(i)
        End If
        If lv_list(i) < min_value Then
            min_value = lv_list(i)
        End If
    Next
    
    'レベルにばらつきがある時にのみレベルでソート
    If min_value <> max_value Then
        For i = 1 To UBound(list) - 1
            max_item = i
            max_value = lv_list(i)
            For j = i + 1 To UBound(list)
                If lv_list(j) > max_value Then
                    max_item = j
                    max_value = lv_list(j)
                End If
            Next
            If max_item <> i Then
                buf = list(i)
                list(i) = list(max_item)
                list(max_item) = buf
                
                buf = ListItemID(i)
                ListItemID(i) = ListItemID(max_item)
                ListItemID(max_item) = buf
                
                lv_list(max_item) = lv_list(i)
            End If
        Next
    End If
    
    If UBound(list) > 0 Then
        Do
            If IsOptionDefined("等身大基準") Then
                ret = MultiSelectListBox("出撃ユニット選択", list, _
                    "ユニット                                            Lv", unum)
            Else
                ret = MultiSelectListBox("出撃ユニット選択", list, _
                    "ユニット                            パイロット       Lv", unum)
            End If
            If ret = 0 Then
                CommandState = "ユニット選択"
                UnlockGUI
                ViewMode = True
                Do While ViewMode
                    Sleep 50
                    DoEvents
                Loop
                LockGUI
                GoTo Beginning
            End If
        Loop While ret = 0
        
        If InStr(opt, "非同期") > 0 Then
            Center ux, uy
            RefreshScreen
        End If
        
        For i = 1 To UBound(list)
            If ListItemFlag(i) Then
                With UList.Item(ListItemID(i))
                    .UsedAction = 0
                    .UsedSupportAttack = 0
                    .UsedSupportGuard = 0
                    .UsedSyncAttack = 0
                    .UsedCounterAttack = 0
                    .StandBy ux, uy, opt
                End With
            End If
        Next
    End If
    
    UList.CheckAutoHyperMode
    
    ReDim ListItemID(0)
    
    ExecOrganizeCmd = LineNum + 1
End Function

Private Function ExecOvalCmd() As Long
Dim pic As PictureBox, pic2 As PictureBox
Dim x1 As Integer, y1 As Integer, rad As Integer
Dim oval_ratio As Double
Dim opt As String
Dim cname As String, clr As Long
Dim i As Integer

    If ArgNum < 5 Then
        EventErrorMessage = "Ovalコマンドの引数の数が違います"
        Error 0
    End If
    
    x1 = GetArgAsLong(2) + BaseX
    y1 = GetArgAsLong(3) + BaseY
    rad = GetArgAsLong(4)
    oval_ratio = GetArgAsDouble(5)
    
    SaveScreen
    
    '描画先
    Select Case ObjDrawOption
        Case "背景"
            Set pic = MainForm.picBack
            Set pic2 = MainForm.picMaskedBack
            IsMapDirty = True
        Case "保持"
            Set pic = MainForm.picMain(0)
            Set pic2 = MainForm.picMain(1)
            IsPictureVisible = True
        Case Else
            Set pic = MainForm.picMain(0)
    End Select
    
    '描画領域
    If ObjDrawOption <> "背景" Then
        Dim tmp As Integer
        IsPictureVisible = True
        tmp = rad + ObjDrawWidth - 1
        PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(x1 - tmp, 0))
        PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(y1 - tmp, 0))
        PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(x1 + tmp, MapPWidth - 1))
        PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(y1 + tmp, MapPHeight - 1))
    End If
    
    clr = ObjColor
    For i = 6 To ArgNum
        opt = GetArgAsString(i)
        If Asc(opt) = 35 Then '#
            If Len(opt) <> 7 Then
                EventErrorMessage = "色指定が不正です"
                Error 0
            End If
            cname = String$(8, vbNullChar)
            Mid(cname, 1, 2) = "&H"
            Mid(cname, 3, 2) = Mid$(opt, 6, 2)
            Mid(cname, 5, 2) = Mid$(opt, 4, 2)
            Mid(cname, 7, 2) = Mid$(opt, 2, 2)
            If Not IsNumeric(cname) Then
                EventErrorMessage = "色指定が不正です"
                Error 0
            End If
            clr = CLng(cname)
        Else
            EventErrorMessage = _
                "Ovalコマンドに不正なオプション「" & opt & "」が使われています"
            Error 0
        End If
    Next
    
    With pic
        .DrawWidth = ObjDrawWidth
        .FillColor = ObjFillColor
        .FillStyle = ObjFillStyle
    End With
    
    pic.Circle (x1, y1), rad, clr, , , oval_ratio
    
    With pic
        .DrawWidth = 1
        .FillColor = vbWhite
        .FillStyle = vbFSTransparent
    End With
    
    If Not pic2 Is Nothing Then
        With pic
            .DrawWidth = ObjDrawWidth
            .FillColor = ObjFillColor
            .FillStyle = ObjFillStyle
        End With
        
        pic.Circle (x1, y1), rad, clr, , , oval_ratio
        
        With pic
            .DrawWidth = 1
            .FillColor = vbWhite
            .FillStyle = vbFSTransparent
        End With
    End If
    
    ExecOvalCmd = LineNum + 1
End Function

Private Function ExecPaintPictureCmd() As Long
Dim fname As String
Dim dx As Long, dy As Long
Dim dw As Long, dh As Long
Dim sx As Long, sy As Long
Dim sw As Long, sh As Long
Dim i As Integer, opt_n As Integer
Dim ret As Integer
Dim buf As String, options As String
Dim cname As String, tcolor As Long

    If ArgNum < 4 Then
        EventErrorMessage = "PaintPictureコマンドの引数の数が違います"
        Error 0
    End If
    
    tcolor = vbWhite
    
    i = 5
    opt_n = 4
    Do While i <= ArgNum
        buf = GetArgAsString(i)
        Select Case buf
            Case "透過", "背景", "白黒", "セピア", _
                "明", "暗", "上下反転", "左右反転", _
                "上半分", "下半分", "右半分", "左半分", _
                "右上", "左上", "右下", "左下", _
                "ネガポジ反転", "シルエット", _
                "夕焼け", "水中", "保持", "フィルタ"
                options = options & buf & " "
            Case "右回転"
                i = i + 1
                options = options & "右回転 " & GetArgAsString(i) & " "
            Case "左回転"
                i = i + 1
                options = options & "左回転 " & GetArgAsString(i) & " "
            Case "-"
                'スキップ
                opt_n = i
            Case ""
                'スキップ
            Case Else
                If Asc(buf) = 35 And Len(buf) = 7 Then
                    cname = String$(8, vbNullChar)
                    Mid(cname, 1, 2) = "&H"
                    Mid(cname, 3, 2) = Mid$(buf, 6, 2)
                    Mid(cname, 5, 2) = Mid$(buf, 4, 2)
                    Mid(cname, 7, 2) = Mid$(buf, 2, 2)
                    If IsNumeric(cname) Then
                        tcolor = CLng(cname)
                        If tcolor <> vbWhite Or GetArgAsString(i - 1) = "フィルタ" Then
                            options = options & Format$(tcolor) & " "
                        End If
                    End If
                ElseIf IsNumeric(buf) Then
                    'スキップ
                    opt_n = i
                ElseIf InStr(buf, " ") > 0 Then
                    options = options & buf & " "
                ElseIf Right(buf, 1) = "%" And IsNumeric(Left(buf, Len(buf) - 1)) Then
                    options = options & buf & " "
                Else
                    EventErrorMessage = _
                       "PaintPictureコマンドの" & Format$(i) & _
                       "番目のパラメータ「" & buf & "」が不正です"
                    Error 0
                End If
        End Select
        i = i + 1
    Loop
    
    fname = GetArgAsString(2)
    Select Case Right$(LCase$(fname), 4)
        Case ".bmp", ".jpg", ".gif", ".png"
            '正しい画像ファイル名
        Case Else
            If PDList.IsDefined(fname) Then
                fname = "Pilot\" & PDList.Item(fname).Bitmap
            ElseIf NPDList.IsDefined(fname) Then
                fname = "Pilot\" & NPDList.Item(fname).Bitmap
            ElseIf UDList.IsDefined(fname) Then
                fname = "Unit\" & UDList.Item(fname).Bitmap
            Else
                EventErrorMessage = _
                    "不正な画像ファイル名「" & fname & "」が指定されています"
                Error 0
            End If
    End Select
    
    '描画先の画像
    buf = GetArgAsString(3)
    If buf = "-" Then
        dx = DEFAULT_LEVEL
    Else
        dx = StrToLng(buf) + BaseX
    End If
    buf = GetArgAsString(4)
    If buf = "-" Then
        dy = DEFAULT_LEVEL
    Else
        dy = StrToLng(buf) + BaseY
    End If
    
    '描画サイズ
    If opt_n >= 6 Then
        buf = GetArgAsString(5)
        If buf = "-" Then
            dw = DEFAULT_LEVEL
        Else
            dw = StrToLng(buf)
            If dw <= 0 Then
                ExecPaintPictureCmd = LineNum + 1
                Exit Function
            End If
        End If
        buf = GetArgAsString(6)
        If buf = "-" Then
            dh = DEFAULT_LEVEL
        Else
            dh = StrToLng(buf)
            If dh <= 0 Then
                ExecPaintPictureCmd = LineNum + 1
                Exit Function
            End If
        End If
    Else
        dw = DEFAULT_LEVEL
        dh = DEFAULT_LEVEL
    End If
    
    '原画像における転送元座標＆サイズ
    If opt_n = 10 Then
        buf = GetArgAsString(7)
        If buf = "-" Then
            sx = DEFAULT_LEVEL
        Else
            sx = StrToLng(buf)
        End If
        buf = GetArgAsString(8)
        If buf = "-" Then
            sy = DEFAULT_LEVEL
        Else
            sy = StrToLng(buf)
        End If
        sw = GetArgAsLong(9)
        sh = GetArgAsLong(10)
    Else
        sx = 0
        sy = 0
        sw = 0
        sh = 0
    End If
    
    ret = DrawPicture(fname, dx, dy, dw, dh, sx, sy, sw, sh, options)
    
    ExecPaintPictureCmd = LineNum + 1
End Function

Private Function ExecPaintStringCmd() As Long
Dim sx As String, sy As String
Dim xx As Integer, yy As Integer
Dim without_cr As Boolean
    
    If CmdName <> PaintStringCmd Then
        without_cr = True
    End If
    
    'PaintStringはあらかじめ構文解析済み
    Select Case ArgNum
        Case 2
            '座標指定がないことが確定
' MOD START マージ
'            DrawString GetArgAsString(2), -1, -1, without_cr
            DrawString GetArgAsString(2), DEFAULT_LEVEL, DEFAULT_LEVEL, without_cr
' MOD END マージ
        Case 4
            '座標指定付きであることが確定
            sx = GetArgAsString(2)
            sy = GetArgAsString(3)
            
            If sx = "-" Then
                HCentering = True
                xx = -1
            Else
                HCentering = False
                xx = CInt(sx) + BaseX
            End If
            If sy = "-" Then
                VCentering = True
                yy = -1
            Else
                VCentering = False
                yy = CInt(sy) + BaseY
            End If
            
            DrawString GetArgAsString(4), xx, yy, without_cr
        Case 5
            '座標指定付きかどうか実行時まで不明
            sx = GetArgAsString(2)
            sy = GetArgAsString(3)
            
            '最初の2引数が有効な座標指定かどうかで判断する
            If (IsNumeric(sx) Or sx = "-") _
                And (IsNumeric(sy) Or sy = "-") _
            Then
                If sx = "-" Then
                    HCentering = True
                    xx = -1
                Else
                    HCentering = False
                    xx = CInt(sx) + BaseX
                End If
                If sy = "-" Then
                    VCentering = True
                    yy = -1
                Else
                    VCentering = False
                    yy = CInt(sy) + BaseY
                End If
                
                DrawString GetArgAsString(4), xx, yy, without_cr
            Else
                DrawString GetArgAsString(5), -1, -1, without_cr
            End If
    End Select
    
    ExecPaintStringCmd = LineNum + 1
End Function

Private Function ExecPaintSysStringCmd() As Long
Dim without_refresh As Boolean

    If ArgNum <> 4 And ArgNum <> 5 Then
        EventErrorMessage = "PaintSysStringコマンドの引数の数が違います"
        Error 0
    End If
    
    If ArgNum = 5 Then
        If GetArgAsString(5) = "非同期" Then
            without_refresh = True
        End If
    End If
    
    DrawSysString GetArgAsLong(2), GetArgAsLong(3), GetArgAsString(4), without_refresh
    
    ExecPaintSysStringCmd = LineNum + 1
End Function

Private Function ExecPilotCmd() As Long
Dim pname As String
Dim plevel As Integer
    
    If ArgNum < 0 Then
        EventErrorMessage = "Pilotコマンドのパラメータの括弧の対応が取れていません"
        Error 0
    ElseIf ArgNum <> 3 And ArgNum <> 4 Then
        EventErrorMessage = "Pilotコマンドの引数の数が違います"
        Error 0
    End If
    
    pname = GetArgAsString(2)
    If Not PDList.IsDefined(pname) Then
        EventErrorMessage = "指定したパイロット「" & pname & "」のデータが見つかりません"
        Error 0
    End If
    
    plevel = GetArgAsLong(3)
    If IsOptionDefined("レベル限界突破") Then
        If plevel > 999 Then
            plevel = 999
        End If
    Else
        If plevel > 99 Then
            plevel = 99
        End If
    End If
    If plevel < 1 Then
        plevel = 1
    End If
    
    If ArgNum = 3 Then
        With PList.Add(pname, plevel, "味方")
            .FullRecover
        End With
    Else
        With PList.Add(pname, plevel, "味方", GetArgAsString(4))
            .FullRecover
        End With
    End If
    
    ExecPilotCmd = LineNum + 1
End Function

Private Function ExecPlayMIDICmd() As Long
Dim fname As String
Dim play_bgm_end As Long
Dim i As Long

    'PlayMIDIコマンドが連続してる場合、最後のPlayMIDIコマンドの位置を検索
    For i = LineNum + 1 To UBound(EventCmd)
        If EventCmd(i).Name <> PlayMIDICmd Then
            Exit For
        End If
    Next
    play_bgm_end = i - 1
    
    '最後のSPlayMIDIから順にMIDIファイルを検索
    For i = play_bgm_end To LineNum Step -1
        fname = ListTail(EventData(i), 2)
        If ListLength(fname) = 1 Then
            If Left(fname, 2) = "$(" Then
                fname = """" & fname & """"
            End If
            fname = GetValueAsString(fname, True)
        Else
            fname = "(" & fname & ")"
        End If
        fname = SearchMidiFile(fname)
        If fname <> "" Then
            'MIDIファイルが存在したので選択
            Exit For
        End If
    Next
    
    'MIDIファイルを再生
    KeepBGM = False
    BossBGM = False
    StartBGM fname, False
    
    '次のコマンド実行位置は最後のPlayMIDIコマンドの後
    ExecPlayMIDICmd = play_bgm_end + 1
End Function

Private Function ExecPlaySoundCmd() As Long
Dim fname As String
    
    fname = ListTail(EventData(LineNum), 2)
    If ListLength(fname) = 1 Then
        fname = GetValueAsString(fname)
    End If
    
    PlayWave fname
    
    ExecPlaySoundCmd = LineNum + 1
End Function

Private Function ExecPolygonCmd() As Long
Dim pic As PictureBox, pic2 As PictureBox
Dim points() As POINTAPI
Dim pnum As Integer
Dim xx As Integer, yy As Integer
Dim x1 As Integer, y1 As Integer
Dim x2 As Integer, y2 As Integer
Dim prev_clr As Long
    
    x1 = MainPWidth
    y1 = MainPHeight
    x2 = 0
    y2 = 0
    pnum = 1
    
    Do While 2 * pnum < ArgNum
        ReDim Preserve points(pnum - 1)
        
        xx = GetArgAsLong(2 * pnum) + BaseX
        yy = GetArgAsLong(2 * pnum + 1) + BaseY
        
        points(pnum - 1).X = xx
        points(pnum - 1).Y = yy
        
        If xx < x1 Then
            x1 = xx
        End If
        If xx > x2 Then
            x2 = xx
        End If
        If yy < y1 Then
            y1 = yy
        End If
        If yy > y2 Then
            y2 = yy
        End If
        
        pnum = pnum + 1
    Loop
    
    If pnum = 1 Then
        EventErrorMessage = "頂点数が少なすぎます"
        Error 0
    End If
    
    SaveScreen
    
    '描画先
    Select Case ObjDrawOption
        Case "背景"
            Set pic = MainForm.picBack
            Set pic2 = MainForm.picMaskedBack
            IsMapDirty = True
        Case "保持"
            Set pic = MainForm.picMain(0)
            Set pic2 = MainForm.picMain(1)
        Case Else
            Set pic = MainForm.picMain(0)
    End Select
    
    '描画領域
    If ObjDrawOption <> "背景" Then
        Dim tmp As Integer
        IsPictureVisible = True
        tmp = ObjDrawWidth - 1
        PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(x1 - tmp, 0))
        PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(y1 - tmp, 0))
        PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(x2 + tmp, MapPWidth - 1))
        PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(y2 + tmp, MapPHeight - 1))
    End If
    
    With pic
        prev_clr = .ForeColor
        .ForeColor = ObjColor
        .DrawWidth = ObjDrawWidth
        .FillColor = ObjFillColor
        .FillStyle = ObjFillStyle
    End With
    
    Call Polygon(pic.hDC, points(0), pnum - 1)
    
    With pic
        .ForeColor = prev_clr
        .DrawWidth = 1
        .FillColor = vbWhite
        .FillStyle = vbFSTransparent
    End With
    
    If Not pic2 Is Nothing Then
        With pic2
            prev_clr = .ForeColor
            .ForeColor = ObjColor
            .DrawWidth = ObjDrawWidth
            .FillColor = ObjFillColor
            .FillStyle = ObjFillStyle
        End With
        
        Call Polygon(pic2.hDC, points(0), pnum - 1)
        
        With pic2
            .ForeColor = prev_clr
            .DrawWidth = 1
            .FillColor = vbWhite
            .FillStyle = vbFSTransparent
        End With
    End If
    
    ExecPolygonCmd = LineNum + 1
End Function

Private Function ExecPrintCmd() As Long
Dim f As Integer
Dim msg As String

    If ArgNum = 1 Then
        EventErrorMessage = "Printコマンドの引数の数が違います"
        Error 0
    End If
    
    f = GetArgAsLong(2)
    
    msg = ListTail(EventData(LineNum), 3)
    
    If Right$(msg, 1) <> ";" Then
        If Left$(msg, 1) <> "`" Or Right$(msg, 1) <> "`" Then
            If Left$(msg, 2) = "$(" Then
                If Right$(msg, 1) = ")" Then
                    msg = GetValueAsString(Mid$(msg, 3, Len(msg) - 3))
                End If
            Else
                If ListLength(msg) = 1 Then
                    msg = GetValueAsString(msg)
                End If
            End If
            ReplaceSubExpression msg
        Else
            msg = Mid$(msg, 2, Len(msg) - 2)
        End If
        Print #f, msg
    Else
        msg = Left$(msg, Len(msg) - 1)
        If Left$(msg, 1) <> "`" Or Right$(msg, 1) <> "`" Then
            If Left$(msg, 2) = "$(" Then
                If Right$(msg, 1) = ")" Then
                    msg = GetValueAsString(Mid$(msg, 3, Len(msg) - 3))
                End If
            Else
                If ListLength(msg) = 1 Then
                    msg = GetValueAsString(msg)
                End If
            End If
            ReplaceSubExpression msg
        Else
            msg = Mid$(msg, 2, Len(msg) - 2)
        End If
        Print #f, msg;
    End If
    
    ExecPrintCmd = LineNum + 1
End Function

Private Function ExecPSetCmd() As Long
Dim pic As PictureBox, pic2 As PictureBox
Dim xx As Integer, yy As Integer
Dim opt As String
Dim cname As String, clr As Long

    If ArgNum < 3 Then
        EventErrorMessage = "PSetコマンドの引数の数が違います"
        Error 0
    End If
    
    '座標
    xx = GetArgAsLong(2) + BaseX
    yy = GetArgAsLong(3) + BaseY
    
    '座標は画面上にある？
    If xx < 0 Or MapPWidth <= xx _
        Or yy < 0 Or MapPHeight <= yy _
    Then
        ExecPSetCmd = LineNum + 1
        Exit Function
    End If
    
    SaveScreen
    
    '描画先
    Select Case ObjDrawOption
        Case "背景"
            Set pic = MainForm.picBack
            Set pic2 = MainForm.picMaskedBack
            IsMapDirty = True
        Case "保持"
            Set pic = MainForm.picMain(0)
            Set pic2 = MainForm.picMain(1)
        Case Else
            Set pic = MainForm.picMain(0)
    End Select
    
    '描画領域
    If ObjDrawOption <> "背景" Then
        Dim tmp As Integer
        IsPictureVisible = True
        tmp = ObjDrawWidth - 1
        If xx - tmp < PaintedAreaX1 Then
            PaintedAreaX1 = xx - tmp
        ElseIf xx + tmp > PaintedAreaX2 Then
            PaintedAreaX2 = xx + tmp
        End If
        If yy - tmp < PaintedAreaY1 Then
            PaintedAreaY1 = yy - tmp
        ElseIf yy + tmp > PaintedAreaY2 Then
            PaintedAreaY2 = yy + tmp
        End If
    End If
    
    '描画色
    If ArgNum = 4 Then
        opt = GetArgAsString(4)
        If Asc(opt) <> 35 Or Len(opt) <> 7 Then
            EventErrorMessage = "色指定が不正です"
            Error 0
        End If
        cname = String$(8, vbNullChar)
        Mid(cname, 1, 2) = "&H"
        Mid(cname, 3, 2) = Mid$(opt, 6, 2)
        Mid(cname, 5, 2) = Mid$(opt, 4, 2)
        Mid(cname, 7, 2) = Mid$(opt, 2, 2)
        If Not IsNumeric(cname) Then
            EventErrorMessage = "色指定が不正です"
            Error 0
        End If
        clr = CLng(cname)
    Else
        clr = ObjColor
    End If
    
    pic.DrawWidth = ObjDrawWidth
    
    '点を描画
    pic.PSet (xx, yy), clr
    
    pic.DrawWidth = 1
    
    If Not pic2 Is Nothing Then
        pic2.DrawWidth = ObjDrawWidth
        
        '点を描画
        pic2.PSet (xx, yy), clr
        
        pic2.DrawWidth = 1
    End If
    
    ExecPSetCmd = LineNum + 1
End Function

Private Function ExecQuestionCmd() As Long
Dim list() As String
Dim i As Long
Dim buf As String

    ReDim list(0)
    ReDim ListItemID(0)
    ReDim ListItemFlag(0)
    ListItemID(0) = "0"
    
    For i = LineNum + 1 To UBound(EventData)
        buf = EventData(i)
        FormatMessage buf
        If Len(buf) > 0 Then
            If LCase$(buf) = "end" Then
                Exit For
            End If
            ReDim Preserve list(UBound(list) + 1)
            ReDim Preserve ListItemID(UBound(list))
            ReDim Preserve ListItemFlag(UBound(list))
            list(UBound(list)) = buf
            ListItemID(UBound(list)) = Format$(i - LineNum)
            ListItemFlag(UBound(list)) = False
        End If
    Next
    If i = UBound(EventData) Then
        EventErrorMessage = "QuestionとEndが対応していません"
        Error 0
    End If
    
    If UBound(list) > 0 Then
        Select Case ArgNum
            Case 3
                SelectedItem = LIPS("選択", list, _
                    GetArgAsString(3), _
                    GetArgAsLong(2))
            Case 2
                SelectedItem = LIPS("選択", list, "さあ、どうする？", _
                    GetArgAsLong(2))
            Case Else
                EventErrorMessage = "Questionコマンドの引数の数が違います"
                Error 0
        End Select
    Else
        SelectedItem = 0
    End If
    
    SelectedAlternative = ListItemID(SelectedItem)
    
    ReDim ListItemID(0)
    
    ExecQuestionCmd = i + 1
End Function

Private Function ExecQuickLoadCmd() As Long
    LockGUI
    
    ClearUnitStatus
    StopBGM
    
    If FileExists(LastSaveDataFileName) Then
        'セーブしたファイルが存在すればそれをロード
        RestoreData LastSaveDataFileName, True
    Else
        'セーブファイルが見つからなければ強制終了
        ErrorMessage "セーブデータが見つかりません"
        TerminateSRC
    End If
    
    '詰まないように乱数系列をリセット
    RndSeed = RndSeed + 1
    RndReset
    
    '再開イベントによるマップ画像の書き換え処理を行う
    HandleEvent "再開"
    IsMapDirty = False
    
    '画面を書き直してステータスを表示
    RedrawScreen
    DisplayGlobalStatus
    MainForm.Show
    
    '操作可能にする
    CommandState = "ユニット選択"
    
    UnlockGUI
    
    IsScenarioFinished = True
    ExecQuickLoadCmd = 0
End Function

Private Function ExecQuitCmd() As Long
    TerminateSRC
End Function

Private Function ExecRankUpCmd() As Long
Dim uname As String
Dim u As Unit
Dim rk As Integer
Dim hp_ratio As Double, en_ratio As Double
Dim i As Integer, j As Integer
Dim buf As String

    Select Case ArgNum
        Case 3
            uname = GetArgAsString(2)
            Set u = UList.Item(uname)
            If u Is Nothing Then
                EventErrorMessage = uname & "というユニットは存在しません"
                Error 0
            End If
            rk = GetArgAsLong(3)
        Case 2
            Set u = SelectedUnitForEvent
            rk = GetArgAsLong(2)
        Case Else
            EventErrorMessage = "RankUpコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        hp_ratio = 100 * .HP / .MaxHP
        en_ratio = 100 * .EN / .MaxEN
        .Rank = .Rank + rk
        .HP = .MaxHP * hp_ratio / 100
        .EN = .MaxEN * en_ratio / 100
        
        For i = 1 To .CountOtherForm
            With .OtherForm(i)
                hp_ratio = 100 * .HP / .MaxHP
                en_ratio = 100 * .EN / .MaxEN
                .Rank = .Rank + rk
                .HP = .MaxHP * hp_ratio / 100
                .EN = .MaxEN * en_ratio / 100
            End With
        Next
        
        '合体できる場合は他の分離ユニットのユニットランクを上げる
        If .IsFeatureAvailable("合体") Then
            '合体後の形態を検索
            For i = 1 To .CountFeature
                If .Feature(i) = "合体" Then
                    buf = LIndex(.FeatureData(i), 2)
                    If LLength(.FeatureData(i)) = 3 Then
                        If UDList.IsDefined(buf) Then
                            If UDList.Item(buf).IsFeatureAvailable("主形態") Then
                                Exit For
                            End If
                        End If
                    Else
                        If UDList.IsDefined(buf) Then
                            If Not UDList.Item(buf).IsFeatureAvailable("制限時間") Then
                                Exit For
                            End If
                        End If
                    End If
                End If
            Next
            If i <= .CountFeature Then
                buf = UDList.Item(LIndex(.FeatureData(i), 2)).FeatureData("分離")
                For i = 2 To LLength(buf)
                    If UList.IsDefined(LIndex(buf, i)) Then
                        With UList.Item(LIndex(buf, i))
                            If Not u.IsEqual(.Name) Then
                                '他の分離形態のユニットランクを上げる
                                hp_ratio = 100 * .HP / .MaxHP
                                en_ratio = 100 * .EN / .MaxEN
                                .Rank = .Rank + rk
                                .HP = .MaxHP * hp_ratio / 100
                                .EN = .MaxEN * en_ratio / 100
                                
                                For j = 1 To .CountOtherForm
                                    With .OtherForm(j)
                                        hp_ratio = 100 * .HP / .MaxHP
                                        en_ratio = 100 * .EN / .MaxEN
                                        .Rank = .Rank + rk
                                        .HP = .MaxHP * hp_ratio / 100
                                        .EN = .MaxEN * en_ratio / 100
                                    End With
                                Next
                            End If
                        End With
                    End If
                Next
            End If
        End If
        
        '分離できる場合は分離ユニットのユニットランクを上げる
        If .IsFeatureAvailable("分離") Then
            buf = .FeatureData("分離")
            For i = 2 To LLength(buf)
                If UList.IsDefined(LIndex(buf, i)) Then
                    With UList.Item(LIndex(buf, i))
                        .Rank = MaxLng(.Rank, u.Rank)
                        .HP = .MaxHP
                        .EN = .MaxEN
                        For j = 1 To .CountOtherForm
                            With .OtherForm(j)
                                hp_ratio = 100 * .HP / .MaxHP
                                en_ratio = 100 * .EN / .MaxEN
                                .Rank = .Rank + rk
                                .HP = .MaxHP * hp_ratio / 100
                                .EN = .MaxEN * en_ratio / 100
                            End With
                        Next
                    End With
                End If
            Next
        End If
    End With
    
    ExecRankUpCmd = LineNum + 1
End Function

Private Function ExecReadCmd() As Long
Dim f As Integer
Dim i As Integer
Dim buf As String

    If ArgNum < 3 Then
        EventErrorMessage = "Readコマンドの引数の数が違います"
        Error 0
    End If
    
    f = GetArgAsLong(2)
    For i = 3 To ArgNum
        Input #f, buf
        SetVariableAsString GetArg(i), buf
    Next
    
    ExecReadCmd = LineNum + 1
End Function

Private Function ExecRecoverENCmd() As Long
Dim u As Unit
Dim per As Double

    Select Case ArgNum
        Case 3
            Set u = GetArgAsUnit(2, True)
            per = GetArgAsDouble(3)
        Case 2
            Set u = SelectedUnitForEvent
            per = GetArgAsDouble(2)
        Case Else
        EventErrorMessage = "RecoverENコマンドの引数の数が違います"
        Error 0
    End Select
    
    If Not u Is Nothing Then
        With u
            .RecoverEN per
            .Update
            If .EN = 0 And .Status = "出撃" Then
                PaintUnitBitmap u
            End If
            .CheckAutoHyperMode
            .CheckAutoNormalMode
        End With
    End If
    
    ExecRecoverENCmd = LineNum + 1
End Function

Private Function ExecRecoverHPCmd() As Long
Dim u As Unit
Dim per As Double

    Select Case ArgNum
        Case 3
            Set u = GetArgAsUnit(2, True)
            per = GetArgAsDouble(3)
        Case 2
            Set u = SelectedUnitForEvent
            per = GetArgAsDouble(2)
        Case Else
            EventErrorMessage = "RecoverHPコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not u Is Nothing Then
        With u
            .RecoverHP per
            .Update
            .CheckAutoHyperMode
            .CheckAutoNormalMode
        End With
    End If
    
    ExecRecoverHPCmd = LineNum + 1
End Function

Private Function ExecRecoverPlanaCmd() As Long
Dim p As Pilot
Dim per As Double
Dim hp_ratio As Double, en_ratio As Double

    Set p = Nothing
    Select Case ArgNum
        Case 3
            Set p = GetArgAsPilot(2)
            per = GetArgAsDouble(3)
        Case 2
            With SelectedUnitForEvent
                If .CountPilot > 0 Then
                    Set p = .MainPilot
                End If
            End With
            per = GetArgAsDouble(2)
        Case Else
            EventErrorMessage = "RecoverPlanaコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExecRecoverPlanaCmd = LineNum + 1
    
    If p Is Nothing Then
        Exit Function
    End If
    
    With p
        If .MaxPlana = 0 Then
            Exit Function
        End If
        
        If Not .Unit Is Nothing Then
            With .Unit
                hp_ratio = 100 * .HP / .MaxHP
                en_ratio = 100 * .EN / .MaxEN
            End With
        End If
        
        .Plana = .Plana + per * .MaxPlana / 100
        
        If Not .Unit Is Nothing Then
            With .Unit
                .HP = .MaxHP * hp_ratio / 100
                .EN = .MaxEN * en_ratio / 100
            End With
        End If
    End With
End Function

Private Function ExecRecoverSPCmd() As Long
Dim p As Pilot
Dim per As Double

    Set p = Nothing
    Select Case ArgNum
        Case 3
            Set p = GetArgAsPilot(2)
            per = GetArgAsDouble(3)
        Case 2
            With SelectedUnitForEvent
                If .CountPilot > 0 Then
                    Set p = .MainPilot
                End If
            End With
            per = GetArgAsDouble(2)
        Case Else
            EventErrorMessage = "RecoverSPコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not p Is Nothing Then
        With p
            If .MaxSP > 0 Then
                .SP = .SP + per * .MaxSP / 100
            End If
        End With
    End If
    
    ExecRecoverSPCmd = LineNum + 1
End Function

Private Function ExecRedrawCmd() As Long
Dim late_refresh As Boolean

    If ArgNum = 2 Then
        If GetArgAsString(2) = "非同期" Then
            late_refresh = True
        End If
    End If
    
    RedrawScreen late_refresh
    ExecRedrawCmd = LineNum + 1
End Function

Private Function ExecRefreshCmd() As Long
    MainForm.picMain(0).Refresh
    ExecRefreshCmd = LineNum + 1
End Function

Private Function ExecReleaseCmd() As Long
Dim buf As String
    
    Select Case ArgNum
        Case 1
            buf = SelectedUnitForEvent.MainPilot.Name
        Case 2
            buf = GetArgAsString(2)
            If Not PList.IsDefined(buf) And Not IList.IsDefined(buf) Then
                EventErrorMessage = _
                    "パイロット名またはアイテム名" & buf & "が間違っています"
                Error 0
            End If
            If PList.IsDefined(buf) Then
                buf = PList.Item(buf).Name
            Else
                buf = IList.Item(buf).Name
            End If
        Case Else
            EventErrorMessage = "Releaseコマンドの引数の数が違います"
            Error 0
    End Select
    
    buf = "Fix(" & buf & ")"
    If IsGlobalVariableDefined(buf) Then
        GlobalVariableList.Remove buf
    End If
    
    ExecReleaseCmd = LineNum + 1
End Function

Private Function ExecRemoveFileCmd() As Long
Dim fname As String

    If ArgNum <> 2 Then
        EventErrorMessage = "RemoveFileコマンドの引数の数が違います"
        Error 0
    End If
    
    fname = ScenarioPath & GetArgAsString(2)
    
    If InStr(fname, "..\") > 0 Then
        EventErrorMessage = "ファイル指定に「..\」は使えません"
        Error 0
    End If
    If InStr(fname, "../") > 0 Then
        EventErrorMessage = "ファイル指定に「../」は使えません"
        Error 0
    End If
    
    If FileExists(fname) Then
        Kill fname
    End If
    
    ExecRemoveFileCmd = LineNum + 1
End Function

Private Function ExecRemoveFolderCmd() As Long
Dim fname As String
Dim fso As Object
   
    If ArgNum <> 2 Then
        EventErrorMessage = "RemoveFolderコマンドの引数の数が違います"
        Error 0
    End If
    
    fname = ScenarioPath & GetArgAsString(2)
    
    If InStr(fname, "..\") > 0 Then
        EventErrorMessage = "ファイル指定に「..\」は使えません"
        Error 0
    End If
    If InStr(fname, "../") > 0 Then
        EventErrorMessage = "ファイル指定に「../」は使えません"
        Error 0
    End If
    
    If Right(fname, 1) = "\" Then
        fname = Left(fname, Len(fname) - 1)
    End If
    
    If FileExists(fname) Then
        Set fso = CreateObject("Scripting.FileSystemObject")
        
        fso.DeleteFolder fname
        
        Set fso = Nothing
    End If
    
    ExecRemoveFolderCmd = LineNum + 1
End Function

Private Function ExecRemoveItemCmd() As Long
Dim pname As String
Dim u As Unit
Dim iname As String, inumber As Integer
Dim itm As Item
Dim i As Integer, j As Integer
Dim item_with_image As Boolean
    
    Select Case ArgNum
        Case 1
            '指定したユニットが装備しているアイテムすべてを外す
            With SelectedUnitForEvent
                Do While .CountItem > 0
                    If .Item(1).IsFeatureAvailable("ユニット画像") Then
                        item_with_image = True
                    End If
                    
                    If .Party0 <> "味方" Then
                        .Item(1).Exist = False
                    End If
                    .DeleteItem 1
                Loop
                
                If item_with_image Then
                    .BitmapID = MakeUnitBitmap(SelectedUnitForEvent)
                    For i = 1 To .CountOtherForm
                        .OtherForm(i).BitmapID = 0
                    Next
                    If .Status = "出撃" Then
                        If Not IsPictureVisible And MapFileName <> "" Then
                            PaintUnitBitmap SelectedUnitForEvent
                        End If
                    End If
                End If
            End With
            
        Case 2
            pname = GetArgAsString(2)
            If UList.IsDefined(pname) Then
                '指定したユニットが装備しているアイテムすべてを外す
                Set u = UList.Item(pname).CurrentForm
                With u
                    Do While .CountItem > 0
                        If .Item(1).IsFeatureAvailable("ユニット画像") Then
                            item_with_image = True
                        End If
                        
                        If .Party0 <> "味方" Then
                            .Item(1).Exist = False
                        End If
                        .DeleteItem 1
                    Loop
                    
                    If item_with_image Then
                        .BitmapID = MakeUnitBitmap(SelectedUnitForEvent)
                        For i = 1 To .CountOtherForm
                            .OtherForm(i).BitmapID = 0
                        Next
                        If .Status = "出撃" Then
                            If Not IsPictureVisible And MapFileName <> "" Then
                                PaintUnitBitmap SelectedUnitForEvent
                            End If
                        End If
                    End If
                End With
            ElseIf PList.IsDefined(pname) Then
                '指定したパイロットが乗るユニットが装備しているアイテムすべてを外す
                Set u = PList.Item(pname).Unit
                If Not u Is Nothing Then
                    With u
                        Do While .CountItem > 0
                            If .Item(1).IsFeatureAvailable("ユニット画像") Then
                                item_with_image = True
                            End If
                            
                            If .Party0 <> "味方" Then
                                .Item(1).Exist = False
                            End If
                            .DeleteItem 1
                        Loop
                        
                        If item_with_image Then
                            .BitmapID = MakeUnitBitmap(u)
                            For i = 1 To .CountOtherForm
                                .OtherForm(i).BitmapID = 0
                            Next
                            If .Status = "出撃" Then
                                If Not IsPictureVisible And MapFileName <> "" Then
                                    PaintUnitBitmap u
                                End If
                            End If
                        End If
                    End With
                End If
            Else
                '指定されたアイテムを削除
                iname = pname
                
                If IsNumeric(iname) Then
                    With SelectedUnitForEvent
                        inumber = CInt(iname)
                        If inumber < 1 Then
                            EventErrorMessage = "指定されたアイテム番号「" & iname & _
                                "」が不正です"
                            Error 0
                        End If
                        If inumber > .CountItem Then
                            EventErrorMessage = "指定されたユニットは" & Format$(.CountItem) & _
                                "個のアイテムしか持っていません"
                            Error 0
                        End If
                        
                        With .Item(inumber)
                            If .IsFeatureAvailable("ユニット画像") Then
                                item_with_image = True
                            End If
                            
                            SelectedUnitForEvent.DeleteItem .ID
                            
                            If item_with_image Then
                                With SelectedUnitForEvent
                                    .BitmapID = MakeUnitBitmap(SelectedUnitForEvent)
                                    For i = 1 To .CountOtherForm
                                        .OtherForm(i).BitmapID = 0
                                    Next
                                    If .Status = "出撃" Then
                                        If Not IsPictureVisible And MapFileName <> "" Then
                                            PaintUnitBitmap SelectedUnitForEvent
                                        End If
                                    End If
                                End With
                            End If
                            
                            Set .Unit = Nothing
                            .Exist = False
                            
                            ExecRemoveItemCmd = LineNum + 1
                            Exit Function
                        End With
                    End With
                End If
                
                'アイテムＩＤが指定された場合はそのまま削除
                If IList.IsDefined(iname) Then
                    If IList.Item(iname).ID = iname Then
                        With IList.Item(iname)
                            If Not .Unit Is Nothing Then
                                If .IsFeatureAvailable("ユニット画像") Then
                                    item_with_image = True
                                End If
                                
                                .Unit.DeleteItem .ID
                                
                                If item_with_image Then
                                    .Unit.BitmapID = MakeUnitBitmap(.Unit)
                                    With .Unit
                                        For i = 1 To .CountOtherForm
                                            .OtherForm(i).BitmapID = 0
                                        Next
                                        If .Status = "出撃" Then
                                            If Not IsPictureVisible And MapFileName <> "" Then
                                                PaintUnitBitmap SelectedUnitForEvent
                                            End If
                                        End If
                                    End With
                                End If
                            End If
                            
                            Set .Unit = Nothing
                            .Exist = False
                            ExecRemoveItemCmd = LineNum + 1
                            Exit Function
                        End With
                    End If
                End If
                
                '大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
                '名前をデータのそれとあわせる
                If IDList.IsDefined(iname) Then
                    iname = IDList.Item(iname).Name
                End If
                
                'まずは装備されていないアイテムを探す
                For Each itm In IList
                    With itm
                        If .Name = iname And .Exist _
                            And .Unit Is Nothing _
                        Then
                            '見つかった
                            .Exist = False
                            Exit For
                        End If
                    End With
                Next
                '見つからなかったら装備されたアイテムから
                If itm Is Nothing Then
                    For Each itm In IList
                        With itm
                            If .Name = iname And .Exist Then
                                If .IsFeatureAvailable("ユニット画像") Then
                                    item_with_image = True
                                End If
                                
                                Set u = .Unit
                                u.DeleteItem .ID
                                
                                If item_with_image Then
                                    u.BitmapID = MakeUnitBitmap(u)
                                    With u
                                        For i = 1 To .CountOtherForm
                                            .OtherForm(i).BitmapID = 0
                                        Next
                                        If .Status = "出撃" Then
                                            If Not IsPictureVisible _
                                                And MapFileName <> "" _
                                            Then
                                                PaintUnitBitmap SelectedUnitForEvent
                                            End If
                                        End If
                                    End With
                                End If
                                
                                Set .Unit = Nothing
                                .Exist = False
                                Exit For
                            End If
                        End With
                    Next
                End If
            End If
            
        Case 3
            '指定されたアイテムを削除
            pname = GetArgAsString(2)
            If UList.IsDefined(pname) Then
                Set u = UList.Item(pname).CurrentForm
            ElseIf PList.IsDefined(pname) Then
                Set u = PList.Item(pname).Unit
                If u Is Nothing Then
                    EventErrorMessage = "「" & pname & "」はユニットに乗っていません"
                    Error 0
                End If
            Else
                EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
                Error 0
            End If
            
            iname = GetArgAsString(3)
            
            With u
                If IsNumeric(iname) Then
                    inumber = CInt(iname)
                    If inumber < 1 Then
                        EventErrorMessage = "指定されたアイテム番号「" & iname & _
                            "」が不正です"
                        Error 0
                    End If
                    If inumber > .CountItem Then
                        EventErrorMessage = "指定されたユニットは" & Format$(.CountItem) & _
                            "個のアイテムしか持っていません"
                        Error 0
                    End If
                    
                    With .Item(inumber)
                        If .IsFeatureAvailable("ユニット画像") Then
                            item_with_image = True
                        End If
                        
                        u.DeleteItem .ID
                        
                        If item_with_image Then
                            With u
                                .BitmapID = MakeUnitBitmap(u)
                                For j = 1 To .CountOtherForm
                                    .OtherForm(j).BitmapID = 0
                                Next
                                If .Status = "出撃" Then
                                    If Not IsPictureVisible And MapFileName <> "" Then
                                        PaintUnitBitmap u
                                    End If
                                End If
                            End With
                        End If
                        
                        Set .Unit = Nothing
                        .Exist = False
                        ExecRemoveItemCmd = LineNum + 1
                        Exit Function
                    End With
                End If
                
                '大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
                '名前をデータのそれとあわせる
                If IDList.IsDefined(iname) Then
                    iname = IDList.Item(iname).Name
                End If
                
                For i = 1 To .CountItem
                    With .Item(i)
                        If (.Name = iname Or .ID = iname) _
                            And .Exist _
                        Then
                            If .IsFeatureAvailable("ユニット画像") Then
                                item_with_image = True
                            End If
                            
                            u.DeleteItem .ID
                            
                            If item_with_image Then
                                With u
                                    .BitmapID = MakeUnitBitmap(u)
                                    For j = 1 To .CountOtherForm
                                        .OtherForm(j).BitmapID = 0
                                    Next
                                    If .Status = "出撃" Then
                                        If Not IsPictureVisible And MapFileName <> "" Then
                                            PaintUnitBitmap u
                                        End If
                                    End If
                                End With
                            End If
                            
                            Set .Unit = Nothing
                            .Exist = False
                            Exit For
                        End If
                    End With
                Next
            End With
            
        Case Else
            EventErrorMessage = "RemoveItemコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExecRemoveItemCmd = LineNum + 1
End Function

Private Function ExecRemovePilotCmd() As Long
Dim pname As String
Dim i As Integer, num As Integer
Dim p As Pilot
Dim opt As String
    
    num = ArgNum
    
    If num > 1 Then
        If GetArgAsString(num) = "非同期" Then
            opt = "非同期"
            num = num - 1
        End If
    End If
    
    ExecRemovePilotCmd = LineNum + 1
    
    Select Case num
        Case 1
            With SelectedUnitForEvent
                If .CountPilot = 0 Then
                    EventErrorMessage = "指定されたユニットにパイロットが乗っていません"
                    Error 0
                End If
                If .Status = "出撃" Then
                    .Escape opt
                End If
                For i = 1 To .CountPilot
                    .Pilot(i).Alive = False
                Next
                For i = 1 To .CountSupport
                    .Support(i).Alive = False
                Next
                .Status = "破棄"
                For i = 1 To .CountOtherForm
                    If .OtherForm(i).Status = "他形態" Then
                        .OtherForm(i).Status = "破棄"
                    End If
                Next
            End With
            
        Case 2
            pname = GetArgAsString(2)
            If Not PList.IsDefined(pname) Then
                EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
                Error 0
            End If
            
            Set p = PList.Item(pname)
            p.Alive = False
            
            If Not p.Unit Is Nothing Then
                With p.Unit
                    If p.ID = .MainPilot.ID Or p.ID = .Pilot(1).ID Then
                        'メインパイロットの場合はパイロット＆サポートを全員削除
                        'ユニットも削除する
                        If .Status = "出撃" Or .Status = "格納" Then
                            .Escape opt
                        End If
                        For i = 1 To .CountPilot
                            .Pilot(i).Alive = False
                        Next
                        For i = 1 To .CountSupport
                            .Support(i).Alive = False
                        Next
                        .Status = "破棄"
                        For i = 1 To .CountOtherForm
                            If .OtherForm(i).Status = "他形態" Then
                                .OtherForm(i).Status = "破棄"
                            End If
                        Next
                    Else
                        'メインパイロットが対象でなければ指定されたパイロットのみを削除
                        For i = 1 To .CountPilot
                            If p.ID = .Pilot(i).ID Then
                                .DeletePilot i
                                Set p.Unit = Nothing
                                Exit Function
                            End If
                        Next
                        For i = 1 To .CountSupport
                            If p.ID = .Support(i).ID Then
                                .DeleteSupport i
                                Set p.Unit = Nothing
                                Exit Function
                            End If
                        Next
                    End If
                End With
            End If
            
        Case Else
            EventErrorMessage = "RemovePilotの引数の数が違います"
            Error 0
    End Select
End Function

Private Function ExecRemoveUnitCmd() As Long
Dim uname As String
Dim u As Unit
Dim i As Integer, num As Integer
Dim opt As String
    
    num = ArgNum
    
    If num > 1 Then
        If GetArgAsString(num) = "非同期" Then
            opt = "非同期"
            num = num - 1
        End If
    End If
    
    Select Case num
        Case 1
            With SelectedUnitForEvent.CurrentForm
                .Escape opt
                If .CountPilot > 0 Then
                    .Pilot(1).GetOff
                End If
                .Status = "破棄"
                For i = 1 To .CountOtherForm
                    If .OtherForm(i).Status = "他形態" Then
                        .OtherForm(i).Status = "破棄"
                    End If
                Next
            End With
        Case 2
            uname = GetArgAsString(2)
            Set u = UList.Item(uname)
            
            'ユニットが存在しなければそのまま終了
            If u Is Nothing Then
                ExecRemoveUnitCmd = LineNum + 1
                Exit Function
            End If
            
            'ユニットＩＤで指定された場合
            If u.ID = uname Then
                With u
                    .Escape opt
                    If .CountPilot > 0 Then
                        .Pilot(1).GetOff
                    End If
                    .Status = "破棄"
                    For i = 1 To .CountOtherForm
                        If .OtherForm(i).Status = "他形態" Then
                            .OtherForm(i).Status = "破棄"
                        End If
                    Next
                End With
                ExecRemoveUnitCmd = LineNum + 1
                Exit Function
            End If
            
            '大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
            '名前をデータのそれとあわせる
            If UDList.IsDefined(uname) Then
                uname = UDList.Item(uname).Name
            End If
            
            'パイロットが乗ってないユニットを優先
            For Each u In UList
                With u.CurrentForm
                    If .Name = uname And .Status <> "破棄" Then
                        If .CountPilot = 0 Then
                            .Escape opt
                            .Status = "破棄"
                            For i = 1 To .CountOtherForm
                                If .OtherForm(i).Status = "他形態" Then
                                    .OtherForm(i).Status = "破棄"
                                End If
                            Next
                            ExecRemoveUnitCmd = LineNum + 1
                            Exit Function
                        End If
                    End If
                End With
            Next
            
            '見つからなければパイロットが乗っているユニットを削除
            For Each u In UList
                With u.CurrentForm
                    If .Name = uname And .Status <> "破棄" Then
                        .Escape opt
                        .Pilot(1).GetOff
                        .Status = "破棄"
                        For i = 1 To .CountOtherForm
                            If .OtherForm(i).Status = "他形態" Then
                                .OtherForm(i).Status = "破棄"
                            End If
                        Next
                        ExecRemoveUnitCmd = LineNum + 1
                        Exit Function
                    End If
                End With
            Next
        Case Else
            EventErrorMessage = "RemoveUnitの引数の数が違います"
            Error 0
    End Select
    
    ExecRemoveUnitCmd = LineNum + 1
End Function

Private Function ExecRenameBGMCmd() As Long
Dim bname As String, vname As String

    If ArgNum <> 3 Then
        EventErrorMessage = "RenameBGMの引数の数が違います"
        Error 0
    End If
    
    bname = GetArgAsString(2)
    Select Case bname
        Case "Map1", "Map2", "Map3", "Map4", "Map5", "Map6", _
            "Briefing", "Intermission", "Subtitle", "End", "default"
            vname = "BGM(" & bname & ")"
        Case Else
            EventErrorMessage = "BGM名が不正です"
            Error 0
    End Select
    
    If Not IsGlobalVariableDefined(vname) Then
        DefineGlobalVariable vname
    End If
    SetVariableAsString vname, GetArgAsString(3)
    
    ExecRenameBGMCmd = LineNum + 1
End Function

Private Function ExecRenameFileCmd() As Long
Dim fname1 As String, fname2 As String

    If ArgNum <> 3 Then
        EventErrorMessage = "RenameFileコマンドの引数の数が違います"
        Error 0
    End If
    
    fname1 = ScenarioPath & GetArgAsString(2)
    fname2 = ScenarioPath & GetArgAsString(3)
    
    If InStr(fname1, "..\") > 0 Then
        EventErrorMessage = "ファイル指定に「..\」は使えません"
        Error 0
    End If
    If InStr(fname1, "../") > 0 Then
        EventErrorMessage = "ファイル指定に「../」は使えません"
        Error 0
    End If
    If InStr(fname2, "..\") > 0 Then
        EventErrorMessage = "ファイル指定に「..\」は使えません"
        Error 0
    End If
    If InStr(fname2, "../") > 0 Then
        EventErrorMessage = "ファイル指定に「../」は使えません"
        Error 0
    End If
    
    If Not FileExists(fname1) Then
        EventErrorMessage = "元のファイル" _
            & "「" & fname1 & "」が見つかりません"
        Error 0
    End If
    If FileExists(fname2) Then
        EventErrorMessage = "既に" _
            & "「" & fname2 & "」が存在しています"
        Error 0
    End If
    
    Name fname1 As fname2
    
    ExecRenameFileCmd = LineNum + 1
End Function

Private Function ExecRenameTermCmd() As Long
Dim tname As String, vname As String

    If ArgNum <> 3 Then
        EventErrorMessage = "RenameTermの引数の数が違います"
        Error 0
    End If
    
    tname = GetArgAsString(2)
    Select Case tname
        Case "HP", "EN", "SP", "CT"
            vname = "ShortTerm(" & tname & ")"
        Case Else
            vname = "Term(" & tname & ")"
    End Select
    
    If Not IsGlobalVariableDefined(vname) Then
        DefineGlobalVariable vname
    End If
    SetVariableAsString vname, GetArgAsString(3)
    
    ExecRenameTermCmd = LineNum + 1
End Function

Private Function ExecReplacePilotCmd() As Long
Dim pname As String
Dim p1 As Pilot, p2 As Pilot
Dim i As Integer, is_support As Boolean
    
    If ArgNum <> 3 Then
        EventErrorMessage = "ReplacePilotの引数の数が違います"
        Error 0
    End If
    
    Set p1 = GetArgAsPilot(2)
    
    pname = GetArgAsString(3)
    If Not PDList.IsDefined(pname) Then
        EventErrorMessage = "パイロット名が間違っています"
        Error 0
    End If
    Set p2 = PList.Add(pname, p1.Level, p1.Party)
    
    With p2
        .FullRecover
        .Morale = p1.Morale
        .Exp = p1.Exp
        If .Data.SP > 0 And p1.MaxSP > 0 Then
            .SP = .MaxSP * p1.SP \ p1.MaxSP
        End If
        If .IsSkillAvailable("霊力") Then
            If p1.IsSkillAvailable("霊力") Then
                .Plana = .MaxPlana * p1.Plana \ p1.MaxPlana
            End If
        End If
    End With
    
    '乗せ換え
    If Not p1.Unit Is Nothing Then
        With p1.Unit
            'パイロットがサポートがどうか判定
            is_support = False
            For i = 1 To .CountSupport
                If .Support(i) Is p1 Then
                    is_support = True
                    Exit For
                End If
            Next
            If .IsFeatureAvailable("追加サポート") Then
                If .AdditionalSupport Is p1 Then
                    is_support = True
                End If
            End If
            
            If is_support Then
                .ReplaceSupport p2, p1.ID
            Else
                .ReplacePilot p2, p1.ID
            End If
        End With
        PList.UpdateSupportMod p1.Unit
    End If
    
    p1.Alive = False
    
    ExecReplacePilotCmd = LineNum + 1
End Function

Private Function ExecRequireCmd() As Long
Dim fname As String, file_head As Long
Dim i As Long, buf As String

    If ArgNum <> 2 Then
        EventErrorMessage = "Requireコマンドの引数の数が違います"
        Error 0
    End If
    
    'LoadEventData2内でLineNumは書き換えられるのでここで設定
    ExecRequireCmd = LineNum + 1

' ADD START マージ
    'Requireコマンドで読み込まれたことを記録済み？
    For i = 1 To UBound(AdditionalEventFileNames)
       If GetArgAsString(2) = AdditionalEventFileNames(i) Then
           Exit Function
       End If
    Next
    
    '読み込んだイベントファイルを記録
    ReDim Preserve AdditionalEventFileNames(UBound(AdditionalEventFileNames) + 1)
    AdditionalEventFileNames(UBound(AdditionalEventFileNames)) = GetArgAsString(2)
' ADD END マージ
    
    '読み込むファイル名
    fname = ScenarioPath & GetArgAsString(2)
    
    '既に読み込まれている場合はスキップ
    For i = 1 To UBound(EventFileNames)
       If fname = EventFileNames(i) Then
           Exit Function
       End If
    Next
    
    'ファイルが存在する？
    If Not FileExists(fname) Then
        EventErrorMessage = "指定されたファイル「" & fname & "」が見つかりません。"
        Error 0
    End If
    
    'ファイルをロード
    file_head = UBound(EventData) + 1
    LoadEventData2 fname, UBound(EventData)
    
    'エラー表示用にサイズを大きく取っておく
    ReDim Preserve EventData(UBound(EventData) + 1)
    ReDim Preserve EventLineNum(UBound(EventData))
    EventData(UBound(EventData)) = ""
    EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
    
    '複数行に分割されたコマンドを結合
    For i = file_head To UBound(EventData) - 1
        If Right$(EventData(i), 1) = "_" Then
            EventData(i + 1) = _
                Left$(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
            EventData(i) = " "
        End If
    Next
    
    'ラベルを登録
    For i = file_head To UBound(EventData)
        buf = EventData(i)
        If Right$(buf, 1) = ":" Then
            AddLabel Left$(buf, Len(buf) - 1), i
        End If
    Next
    
    'コマンドデータ配列を設定
    If UBound(EventData) > UBound(EventCmd) Then
        ReDim Preserve EventCmd(UBound(EventData))
        i = UBound(EventData)
        Do While EventCmd(i) Is Nothing
            Set EventCmd(i) = New CmdData
            EventCmd(i).LineNum = i
            i = i - 1
        Loop
    End If
    For i = file_head To UBound(EventData)
        EventCmd(i).Name = NullCmd
    Next
    
    '読み込んだイベントファイルを記録
    ReDim Preserve AdditionalEventFileNames(UBound(AdditionalEventFileNames) + 1)
    AdditionalEventFileNames(UBound(AdditionalEventFileNames)) = GetArgAsString(2)
End Function

Private Function ExecRestoreEventCmd() As Long
    If ArgNum <> 2 Then
        EventErrorMessage = "RestoreEventコマンドの引数の数が違います"
        Error 0
    End If
    
    RestoreLabel GetArgAsString(2)
    
    ExecRestoreEventCmd = LineNum + 1
End Function

Private Function ExecRideCmd() As Long
Dim p As Pilot
Dim u As Unit
Dim uname As String
Dim i As Integer

    Set p = GetArgAsPilot(2)
    
    Select Case ArgNum
        Case 3
            uname = GetArgAsString(3)
            
            '指定したユニットに既に乗っている場合は何もしない
            If Not p.Unit Is Nothing Then
                With p.Unit
                    If .Name = uname Or .ID = uname Then
                        ExecRideCmd = LineNum + 1
                        Exit Function
                    End If
                End With
            End If
            
            p.GetOff
            
            Set u = UList.Item(uname)
            If u Is Nothing Then
                EventErrorMessage = "ユニット名が間違っています"
                Error 0
            End If
            
            'ユニットＩＤで指定された場合
            If u.ID = uname Then
                p.Ride u.CurrentForm
                ExecRideCmd = LineNum + 1
                Exit Function
            End If
            
            '大文字・小文字、ひらがな・かたかなの違いを正しく判定できるように、
            '名前をデータのそれとあわせる
            If UDList.IsDefined(uname) Then
                uname = UDList.Item(uname).Name
            End If
            
            'Rideコマンドで乗せ換えられたユニット＆パイロットの履歴を更新
            If uname = LastUnitName Then
                ReDim Preserve LastPilotID(UBound(LastPilotID) + 1)
            Else
                LastUnitName = uname
                ReDim LastPilotID(1)
            End If
            LastPilotID(UBound(LastPilotID)) = p.ID
            
            'パイロットが足りていないものを優先
            For Each u In UList
                With u
                    If .Name = uname And .Party0 = p.Party And .Status <> "破棄" Then
                        If p.IsSupport(u) _
                            And Not .IsFeatureAvailable("ダミーユニット") _
                        Then
                            p.Ride .CurrentForm
                            ExecRideCmd = LineNum + 1
                            Exit Function
                        End If
                        If .CurrentForm.CountPilot < Abs(.Data.PilotNum) Then
                            p.Ride .CurrentForm
                            ExecRideCmd = LineNum + 1
                            Exit Function
                        End If
                    End If
                End With
            Next
            
            '空きがなければ今までRideコマンドで指定されてないユニットに乗り込む
            For Each u In UList
                With u
                    If .Name = uname And .Party0 = p.Party And .Status <> "破棄" Then
                        If .CurrentForm.CountPilot > 0 Then
                            '今までにRideコマンドで指定されているか判定
                            For i = 1 To UBound(LastPilotID) - 1
                                If .CurrentForm.MainPilot.ID = LastPilotID(i) Then
                                    GoTo NextUnit
                                End If
                            Next
                            .CurrentForm.Pilot(1).GetOff True
                        End If
                        p.Ride .CurrentForm
                        ExecRideCmd = LineNum + 1
                        Exit Function
                    End If
                End With
NextUnit:
            Next
            
            'それでも見つからなければ無差別で……
            For Each u In UList
                With u
                    If .Name = uname And .Party0 = p.Party And .Status <> "破棄" Then
                        If .CurrentForm.CountPilot > 0 Then
                            .CurrentForm.Pilot(1).GetOff True
                        End If
                        p.Ride .CurrentForm
                        '乗り込み履歴を初期化
                        ReDim LastPilotID(1)
                        LastPilotID(1) = p.ID
                        ExecRideCmd = LineNum + 1
                        Exit Function
                    End If
                End With
            Next
            
            EventErrorMessage = p.Name & "が乗り込むための" & uname & "が存在しません"
            Error 0
        Case 2
            '指定したユニットに既に乗っている場合は何もしない
            If p.Unit Is SelectedUnitForEvent Then
                ExecRideCmd = LineNum + 1
                Exit Function
            End If
            
            With SelectedUnitForEvent
                If .CountPilot = Abs(.Data.PilotNum) _
                    And Not p.IsSupport(SelectedUnitForEvent) _
                Then
' MOD START マージ
'                    .Pilot(1).GetOff
                    .Pilot(1).GetOff True
' MOD END マージ
                End If
            End With
            p.GetOff
            p.Ride SelectedUnitForEvent
        Case Else
            EventErrorMessage = "Rideコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExecRideCmd = LineNum + 1
End Function

Private Function ExecSaveDataCmd() As Long
Dim fname As String, save_path As String
Dim ret As Integer
    
    If ArgNum = 2 Then
        fname = GetArgAsString(2)
    ElseIf ArgNum = 1 Then
        '一旦「常に手前に表示」を解除
        If frmListBox.Visible Then
            ret = SetWindowPos(frmListBox.hwnd, -2, 0, 0, 0, 0, &H3)
        End If
        
        fname = SaveFileDialog("データセーブ", _
            ScenarioPath, GetValueAsString("セーブデータファイル名"), _
            2, "ｾｰﾌﾞﾃﾞｰﾀ", "src")
        
        '再び「常に手前に表示」
        If frmListBox.Visible Then
            ret = SetWindowPos(frmListBox.hwnd, -1, 0, 0, 0, 0, &H3)
        End If
        
        'キャンセル？
        If fname = "" Then
            ExecSaveDataCmd = LineNum + 1
            Exit Function
        End If
        
        'セーブ先はシナリオフォルダ？
        If InStr(fname, "\") > 0 Then
            save_path = Left$(fname, InStr2(fname, "\"))
        End If
        If Dir$(save_path) <> Dir$(ScenarioPath) Then
            If MsgBox("セーブファイルはシナリオフォルダにないと読み込めません。" & vbCr & vbLf _
                    & "このままセーブしますか？", vbOKCancel + vbQuestion) <> 1 _
            Then
                ExecSaveDataCmd = LineNum + 1
                Exit Function
            End If
        End If
    Else
        EventErrorMessage = "SaveDataコマンドの引数の数が違います"
        Error 0
    End If
    
    If fname <> "" Then
        UList.Update '追加パイロットを消去
        SaveData fname
    End If
    
    ExecSaveDataCmd = LineNum + 1
End Function

Private Function ExecSelectCmd() As Long
    If ArgNum <> 2 Then
        EventErrorMessage = "Selectコマンドの引数の数が違います"
        Error 0
    End If
    
    Set SelectedUnitForEvent = GetArgAsUnit(2)
    
    ExecSelectCmd = LineNum + 1
End Function

Private Function ExecSelectTargetCmd() As Long
Dim pname As String

    If ArgNum <> 2 Then
        EventErrorMessage = "SelectTargetコマンドの引数の数が違います"
        Error 0
    End If
    
    Set SelectedTargetForEvent = GetArgAsUnit(2)
    
    ExecSelectTargetCmd = LineNum + 1
End Function

Private Function ExecSepiaCmd() As Long
Dim prev_x As Integer, prev_y As Integer
Dim u As Unit
Dim late_refresh As Boolean
Dim i As Integer, buf As String
    
    late_refresh = False
    MapDrawIsMapOnly = False
    For i = 2 To ArgNum
        buf = GetArgAsString(i)
        Select Case buf
            Case "非同期"
                late_refresh = True
            Case "マップ限定"
                MapDrawIsMapOnly = True
            Case Else
                EventErrorMessage = _
                    "Sepiaコマンドに不正なオプション「" & buf & "」が使われています"
                Error 0
        End Select
    Next
    
    prev_x = MapX
    prev_y = MapY
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    SetupBackground "セピア", "非同期"
    
    For Each u In UList
        With u
            If .Status = "出撃" Then
                If .BitmapID = 0 Then
                    With UList.Item(.Name)
                        If u.Party0 = .Party0 And .BitmapID <> 0 _
                            And u.Bitmap = .Bitmap _
                            And Not .IsFeatureAvailable("ダミーユニット") _
                        Then
                            u.BitmapID = .BitmapID
                        Else
                            u.BitmapID = MakeUnitBitmap(u)
                        End If
                    End With
                End If
            End If
        End With
    Next
    
    Center prev_x, prev_y
    RedrawScreen late_refresh
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ExecSepiaCmd = LineNum + 1
End Function

Private Function ExecSetCmd() As Long
Dim etype As ValueType, str_result As String, num_result As Double
Dim num As Integer

    num = ArgNum
    If num > 3 Then
        '過去のバージョンのシナリオを読み込めるようにするため、
        'Setコマンドの後ろの「#」形式のコメントは無視する
        If Left$(GetArg(4), 1) = "#" Then
            num = 3
        Else
            EventErrorMessage = "Setコマンドの引数の数が違います"
            Error 0
        End If
    End If
    
    Select Case num
        Case 2
            SetVariableAsLong GetArg(2), 1
        Case 3
            Select Case ArgsType(3)
                Case UndefinedType
                    etype = EvalTerm(strArgs(3), UndefinedType, str_result, num_result)
                    If etype = NumericType Then
                        SetVariableAsDouble GetArg(2), num_result
                    Else
                        SetVariableAsString GetArg(2), str_result
                    End If
                Case StringType
                    SetVariableAsString GetArg(2), strArgs(3)
                Case NumericType
                    SetVariableAsDouble GetArg(2), dblArgs(3)
            End Select
    End Select
    
    ExecSetCmd = LineNum + 1
End Function

Private Function ExecSetBulletCmd() As Long
Dim wname As String, wid As Integer, num As Integer
Dim u As Unit
    
    Select Case ArgNum
        Case 4
            Set u = GetArgAsUnit(2)
            wname = GetArgAsString(3)
            num = GetArgAsLong(4)
            
        Case 3
            Set u = SelectedUnitForEvent
            wname = GetArgAsString(2)
            num = GetArgAsLong(3)
            
        Case Else
            EventErrorMessage = "SetBulletコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        If IsNumeric(wname) Then
            wid = StrToLng(wname)
            If wid < 1 Or .CountWeapon < wid Then
                EventErrorMessage = "武器の番号「" & wname & "」が間違っています"
                Error 0
            End If
        Else
            For wid = 1 To .CountWeapon
                If .Weapon(wid).Name = wname Then
                    Exit For
                End If
            Next
            If wid < 1 Or .CountWeapon < wid Then
                EventErrorMessage = .Name & "は武器「" & wname & "」を持っていません"
                Error 0
            End If
        End If
        .SetBullet wid, MinLng(num, .MaxBullet(wid))
    End With
    
    ExecSetBulletCmd = LineNum + 1
End Function

Private Function ExecSetMessageCmd() As Long
Dim u As Unit
Dim pname As String, pname0 As String
Dim sit As String
Dim selected_msg As String
    
    Select Case ArgNum
        Case 4
            pname = GetArgAsString(2)
            Set u = UList.Item2(pname)
            If u Is Nothing Then
                With PList
                    If Not .IsDefined(pname) Then
                        pname0 = pname
                        If InStr(pname0, "(") > 0 Then
                            pname0 = Left$(pname0, InStr2(pname0, "(") - 1)
                        End If
                        If Not .IsDefined(pname0) Then
                            EventErrorMessage = _
                                "「" & pname & "」というパイロットが見つかりません"
                            Error 0
                        End If
                        Set u = .Item(pname0).Unit
                    Else
                        Set u = .Item(pname).Unit
                    End If
                End With
                If u Is Nothing Then
                    EventErrorMessage = "「" & pname & "」はユニットに乗っていません"
                    Error 0
                End If
            ElseIf u.CountPilot = 0 Then
                EventErrorMessage = _
                    "指定されたユニットにはパイロットが乗っていません"
                Error 0
            End If
            sit = GetArgAsString(3)
            selected_msg = GetArgAsString(4)
        Case 3
            Set u = SelectedUnitForEvent
            If u.CountPilot = 0 Then
                EventErrorMessage = _
                    "指定されたユニットにはパイロットが乗っていません"
                Error 0
            End If
            sit = GetArgAsString(2)
            selected_msg = GetArgAsString(3)
        Case Else
            EventErrorMessage = "SetMessageコマンドの引数の数が違います"
            Error 0
    End Select
    
    If selected_msg = "解除" Then
        'メッセージ用変数を削除
        UndefineVariable "Message(" & u.MainPilot.ID & "," & sit & ")"
    ElseIf pname0 <> "" Then
        '表情指定付きメッセージをローカル変数として登録する
        SetVariableAsString "Message(" & u.MainPilot.ID & "," & sit & ")", _
            pname & "::" & selected_msg
    Else
        'メッセージをローカル変数として登録する
        SetVariableAsString "Message(" & u.MainPilot.ID & "," & sit & ")", _
            selected_msg
    End If
    
    ExecSetMessageCmd = LineNum + 1
End Function

Private Function ExecSetRelationCmd() As Long
Dim pname1 As String, pname2 As String
Dim vname As String, rel As Integer

    Select Case ArgNum
        Case 3
            pname1 = SelectedUnitForEvent.MainPilot.Name
            pname2 = GetArgAsString(2)
            If Not PDList.IsDefined(pname2) Then
                EventErrorMessage = "キャラクター名が間違っています"
                Error 0
            End If
            pname2 = PDList.Item(pname2).Name
            
            rel = GetArgAsLong(3)
        Case 4
            pname1 = GetArgAsString(2)
            If Not PDList.IsDefined(pname1) Then
                EventErrorMessage = "キャラクター名が間違っています"
                Error 0
            End If
            pname1 = PDList.Item(pname1).Name
            
            pname2 = GetArgAsString(3)
            If Not PDList.IsDefined(pname2) Then
                EventErrorMessage = "キャラクター名が間違っています"
                Error 0
            End If
            pname2 = PDList.Item(pname2).Name
            
            rel = GetArgAsLong(4)
        Case Else
            EventErrorMessage = "SetRelationコマンドの引数の数が違います"
            Error 0
    End Select
    
    vname = "関係:" & pname1 & ":" & pname2
    
    If rel <> 0 Then
        If Not IsGlobalVariableDefined(vname) Then
            DefineGlobalVariable vname
        End If
        SetVariableAsLong vname, rel
    Else
        If IsGlobalVariableDefined(vname) Then
            UndefineVariable vname
        End If
    End If
    
    '信頼度補正による気力修正を更新
    If IsOptionDefined("信頼度補正") Then
        If PList.IsDefined(pname1) Then
            PList.Item(pname1).UpdateSupportMod
        End If
        If PList.IsDefined(pname2) Then
            PList.Item(pname2).UpdateSupportMod
        End If
    End If
    
    ExecSetRelationCmd = LineNum + 1
End Function

Private Function ExecSetSkillCmd() As Long
Dim pname As String
Dim vname As String
Dim slist As String
Dim sname As String, sname_array() As String
Dim slevel As Double, slevel_array() As Double
Dim sdata As String, sdata_array() As String
Dim i As Integer, j As Integer

    If ArgNum <> 4 And ArgNum <> 5 Then
        EventErrorMessage = "SetSkillコマンドの引数の数が違います"
        Error 0
    End If
    
    pname = GetArgAsString(2)
    If PList.IsDefined(pname) Then
        pname = PList.Item(pname).ID
    ElseIf PDList.IsDefined(pname) Then
        pname = PDList.Item(pname).Name
    Else
        EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
        Error 0
    End If
    
    sname = GetArgAsString(3)
    slevel = GetArgAsDouble(4)
    If ArgNum = 5 Then
        sdata = GetArgAsString(5)
    End If
    
    'エリアスが定義されている？
    If ALDList.IsDefined(sname) Then
        With ALDList.Item(sname)
            ReDim sname_array(.Count)
            ReDim slevel_array(.Count)
            ReDim sdata_array(.Count)
            For i = 1 To .Count
                If LIndex(.AliasData(i), 1) = "解説" Then
                    If sdata = "" Then
                        sname_array(i) = .AliasType(i)
                    Else
                        sname_array(i) = LIndex(sdata, 1)
                    End If
                    If slevel = 0 Then
                        slevel_array(i) = 0
                    Else
                        slevel_array(i) = DEFAULT_LEVEL
                    End If
                    sdata_array(i) = .AliasData(i)
                Else
                    sname_array(i) = .AliasType(i)
                    
                    If slevel = -1 Then
                        slevel_array(i) = .AliasLevel(i)
                    ElseIf .AliasLevelIsPlusMod(i) Then
                        slevel_array(i) = slevel + .AliasLevel(i)
                    ElseIf .AliasLevelIsMultMod(i) Then
                        slevel_array(i) = slevel * .AliasLevel(i)
                    Else
                        slevel_array(i) = slevel
                    End If
                    
                    If sdata = "" Then
                        sdata_array(i) = .AliasData(i)
                    Else
                        sdata_array(i) = Trim$(sdata & " " & ListTail(.AliasData(i), 2))
                    End If
                    
                    If .AliasLevelIsPlusMod(i) Or .AliasLevelIsMultMod(i) Then
                        sdata_array(i) = LIndex(sdata_array(i), 1) & "Lv" & Format$(slevel) & _
                            " " & ListTail(sdata_array(i), 2)
                        sdata_array(i) = Trim$(sdata_array(i))
                    End If
                End If
            Next
        End With
    Else
        ReDim sname_array(1)
        ReDim slevel_array(1)
        ReDim sdata_array(1)
        sname_array(1) = sname
        slevel_array(1) = slevel
        sdata_array(1) = sdata
    End If
    
    For i = 1 To UBound(sname_array)
        sname = sname_array(i)
        slevel = slevel_array(i)
        sdata = sdata_array(i)
        
        If sname = "" Then
            GoTo NextSkill
        End If
        
        'アビリティ一覧表示用にSetSkillが適用された能力の一覧用変数を作成
        If Not IsGlobalVariableDefined("Ability(" & pname & ")") Then
            DefineGlobalVariable "Ability(" & pname & ")"
            slist = sname
        Else
            slist = GlobalVariableList.Item("Ability(" & pname & ")").StringValue
            For j = 1 To LLength(slist)
                If sname = LIndex(slist, j) Then
                    Exit For
                End If
            Next
            If j > LLength(slist) Then
                slist = slist & " " & sname
            End If
        End If
        SetVariableAsString "Ability(" & pname & ")", slist
        
        '今回SetSkillが適用された能力sname用変数を作成
        vname = "Ability(" & pname & "," & sname & ")"
        If Not IsGlobalVariableDefined(vname) Then
            DefineGlobalVariable vname
        End If
        
        If sdata <> "" Then
            '別名指定があった場合
            SetVariableAsString vname, Format$(slevel) & " " & sdata
            
            '必要技能用
            If sdata <> "非表示" And LIndex(sdata, 1) <> "解説" Then
                vname = "Ability(" & pname & "," & LIndex(sdata, 1) & ")"
                If Not IsGlobalVariableDefined(vname) Then
                    DefineGlobalVariable vname
                End If
                SetVariableAsString vname, Format$(slevel)
            End If
        Else
            SetVariableAsString vname, Format$(slevel)
        End If
NextSkill:
    Next
    
    'パイロットやユニットのステータスをアップデート
    If PList.IsDefined(pname) Then
        With PList.Item(pname)
            .Update
            If Not .Unit Is Nothing Then
                .Unit.Update
                If .Unit.Status = "出撃" Then
                    PList.UpdateSupportMod .Unit
                End If
            End If
        End With
    End If
    
    ExecSetSkillCmd = LineNum + 1
End Function

Private Function ExecSetStatusCmd() As Long
Dim u As Unit
Dim cname As String
    
    Select Case ArgNum
        Case 4
            Set u = GetArgAsUnit(2)
            With u
                cname = GetArgAsString(3)
                .AddCondition cname, GetArgAsLong(4)
                If .Status = "出撃" Then
                    PaintUnitBitmap u
                End If
                If cname <> "非操作" Then
                    .Update
                End If
            End With
        Case 3
            If Not SelectedUnitForEvent Is Nothing Then
                With SelectedUnitForEvent
                    cname = GetArgAsString(2)
                    .AddCondition cname, GetArgAsLong(3)
                    If .Status = "出撃" Then
                        PaintUnitBitmap SelectedUnitForEvent
                    End If
                    If cname <> "非操作" Then
                        .Update
                    End If
                End With
            End If
        Case Else
            EventErrorMessage = "SetStatusコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExecSetStatusCmd = LineNum + 1
End Function

'ADD START 240a
Private Function ExecSetStatusStringColor() As Long
Dim opt As String, cname As String, target As String
Dim color As Long
    '引数チェック
    If ArgNum <> 3 Then
        EventErrorMessage = "StatusStringColorコマンドの引数の数が違います"
        Error 0
    End If
    
    '変更色を取得
    opt = GetArgAsString(2)
    If Asc(opt) <> 35 Or Len(opt) <> 7 Then
        EventErrorMessage = "色指定が不正です"
        Error 0
    End If
    cname = String$(8, vbNullChar)
    Mid(cname, 1, 2) = "&H"
    Mid(cname, 3, 2) = Mid$(opt, 6, 2)
    Mid(cname, 5, 2) = Mid$(opt, 4, 2)
    Mid(cname, 7, 2) = Mid$(opt, 2, 2)
    If Not IsNumeric(cname) Then
        EventErrorMessage = "色指定が不正です"
        Error 0
    End If
    color = CLng(cname)
    
    '変更対象を取得
    target = GetArgAsString(3)
    If target <> "通常" And target <> "能力名" And target <> "有効" And target <> "無効" Then
        EventErrorMessage = "設定対象の指定が不正です"
        Error 0
    End If

    '処理実行
    Select Case target
    Case "通常"
        StatusFontColorNormalString = color
        'Global変数に保存
        If Not IsGlobalVariableDefined("StatusWindow(StringColor)") Then
            DefineGlobalVariable "StatusWindow(StringColor)"
        End If
        SetVariableAsLong "StatusWindow(StringColor)", color
    Case "能力名"
        StatusFontColorAbilityName = color
        'Global変数に保存
        If Not IsGlobalVariableDefined("StatusWindow(ANameColor)") Then
            DefineGlobalVariable "StatusWindow(ANameColor)"
        End If
        SetVariableAsLong "StatusWindow(ANameColor)", color
    Case "有効"
        StatusFontColorAbilityEnable = color
        'Global変数に保存
        If Not IsGlobalVariableDefined("StatusWindow(EnableColor)") Then
            DefineGlobalVariable "StatusWindow(EnableColor)"
        End If
        SetVariableAsLong "StatusWindow(EnableColor)", color
    Case "無効"
        StatusFontColorAbilityDisable = color
        'Global変数に保存
        If Not IsGlobalVariableDefined("StatusWindow(DisableColor)") Then
            DefineGlobalVariable "StatusWindow(DisableColor)"
        End If
        SetVariableAsLong "StatusWindow(DisableColor)", color
    End Select

    ExecSetStatusStringColor = LineNum + 1

End Function
'ADD  END  240a

Private Function ExecSetStockCmd() As Long
Dim aname As String, aid As Integer, num As Integer
Dim u As Unit
    
    Select Case ArgNum
        Case 4
            Set u = GetArgAsUnit(2)
            aname = GetArgAsString(3)
            num = GetArgAsLong(4)
            
        Case 3
            Set u = SelectedUnitForEvent
            aname = GetArgAsString(2)
            num = GetArgAsLong(3)
            
        Case Else
            EventErrorMessage = "SetStockコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        If IsNumeric(aname) Then
            aid = StrToLng(aname)
            If aid < 1 Or .CountAbility < aid Then
                EventErrorMessage = "アビリティの番号「" & aname & "」が間違っています"
                Error 0
            End If
        Else
            For aid = 1 To .CountAbility
                If .Ability(aid).Name = aname Then
                    Exit For
                End If
            Next
            If aid < 1 Or .CountAbility < aid Then
                EventErrorMessage = .Name & "はアビリティ「" & aname & "」を持っていません"
                Error 0
            End If
        End If
        .SetStock aid, MinLng(num, .MaxStock(aid))
    End With
    
    ExecSetStockCmd = LineNum + 1
End Function
'ADD START 240a
Private Function ExecSetWindowColor() As Long
Dim cname As String, opt As String, target As String
Dim color As Long
Dim isTargetLine As Boolean, isTargetBG As Boolean

    '引数チェック
    If ArgNum <> 2 And ArgNum <> 3 Then
        EventErrorMessage = "SetWindowColorコマンドの引数の数が違います"
        Error 0
    End If
    
    '色取得
    opt = GetArgAsString(2)
    If Asc(opt) <> 35 Or Len(opt) <> 7 Then
        EventErrorMessage = "色指定が不正です"
        Error 0
    End If
    cname = String$(8, vbNullChar)
    Mid(cname, 1, 2) = "&H"
    Mid(cname, 3, 2) = Mid$(opt, 6, 2)
    Mid(cname, 5, 2) = Mid$(opt, 4, 2)
    Mid(cname, 7, 2) = Mid$(opt, 2, 2)
    If Not IsNumeric(cname) Then
        EventErrorMessage = "色指定が不正です"
        Error 0
    End If
    color = CLng(cname)
    
    '変更対象取得
    isTargetLine = False
    isTargetBG = False
    If ArgNum = 3 Then
        target = GetArgAsString(3)
        If target = "枠" Then
            isTargetLine = True
        ElseIf target = "背景" Then
            isTargetBG = True
        Else
            EventErrorMessage = "色設定対象の指定が不正です"
            Error 0
        End If
    End If
    
    '処理開始
    If isTargetLine Then
        StatusWindowFrameColor = color
        'Global変数に保存
        If Not IsGlobalVariableDefined("StatusWindow(FrameColor)") Then
            DefineGlobalVariable "StatusWindow(FrameColor)"
        End If
        SetVariableAsLong "StatusWindow(FrameColor)", color
    ElseIf isTargetBG Then
        StatusWindowBackBolor = color
        'Global変数に保存
        If Not IsGlobalVariableDefined("StatusWindow(BackBolor)") Then
            DefineGlobalVariable "StatusWindow(BackBolor)"
        End If
        SetVariableAsLong "StatusWindow(BackBolor)", color
    ElseIf Not isTargetLine And Not isTargetBG Then
        StatusWindowFrameColor = color
        'Global変数に保存
        If Not IsGlobalVariableDefined("StatusWindow(FrameColor)") Then
            DefineGlobalVariable "StatusWindow(FrameColor)"
        End If
        SetVariableAsLong "StatusWindow(FrameColor)", color
        StatusWindowBackBolor = color
        'Global変数に保存
        If Not IsGlobalVariableDefined("StatusWindow(BackBolor)") Then
            DefineGlobalVariable "StatusWindow(BackBolor)"
        End If
        SetVariableAsLong "StatusWindow(BackBolor)", color
    End If
    
    ExecSetWindowColor = LineNum + 1

End Function

Private Function ExecSetWindowFrameWidth() As Long
Dim width As Long
    '引数チェック
    If ArgNum <> 2 Then
        EventErrorMessage = "SetWindowColorコマンドの引数の数が違います"
        Error 0
    End If
    '幅取得
    width = GetArgAsLong(2)
    '処理開始
    StatusWindowFrameWidth = width
    'Global変数に保存
    If Not IsGlobalVariableDefined("StatusWindow(FrameWidth)") Then
        DefineGlobalVariable "StatusWindow(FrameWidth)"
    End If
    SetVariableAsLong "StatusWindow(FrameWidth)", width
    
    ExecSetWindowFrameWidth = LineNum + 1
End Function
'ADD  END

Private Function ExecShowCmd() As Long
    With MainForm
        If Not .Visible Then
            .Show
            .Refresh
            DoEvents
        End If
    End With
    
    If Not IsPictureVisible Then
        RedrawScreen
    End If
    
    ExecShowCmd = LineNum + 1
End Function

'互換性維持のために残している
Private Function ExecShowImageCmd() As Long
Dim fname As String
Dim dw As Long, dh As Long
Dim ret As Integer

    fname = GetArgAsString(2)
    Select Case Right$(LCase$(fname), 4)
        Case ".bmp", ".jpg", ".gif", ".png"
            '正しい画像ファイル名
        Case Else
            EventErrorMessage = _
                "不正な画像ファイル名「" & fname & "」が指定されています"
            Error 0
    End Select
    
    If ArgNum > 2 Then
        dw = GetArgAsLong(3)
        dh = GetArgAsLong(4)
    Else
        dw = DEFAULT_LEVEL
        dh = DEFAULT_LEVEL
    End If
    
    If Not MainForm.Visible Then
        MainForm.Show
    End If
    
    ret = DrawPicture(fname, DEFAULT_LEVEL, DEFAULT_LEVEL, dw, dh, 0, 0, 0, 0, "")
    MainForm.picMain(0).Refresh
    
    ExecShowImageCmd = LineNum + 1
End Function

Private Function ExecShowUnitStatusCmd() As Long
Dim u As Unit

    Select Case ArgNum
        Case 1
            Set u = SelectedUnitForEvent
        Case 2
            If GetArgAsString(2) = "終了" Then
                ClearUnitStatus
                ExecShowUnitStatusCmd = LineNum + 1
                Exit Function
            End If
            
            Set u = GetArgAsUnit(2)
        Case Else
            EventErrorMessage = "ShowUnitStatusコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not u Is Nothing Then
        DisplayUnitStatus u
    End If
    
    ExecShowUnitStatusCmd = LineNum + 1
End Function

Private Function ExecSkipCmd() As Long
Dim i As Long, depth As Integer

    '対応するループの末尾を探す
    depth = 1
    For i = LineNum + 1 To UBound(EventCmd)
        Select Case EventCmd(i).Name
            Case DoCmd, ForCmd, ForEachCmd
                depth = depth + 1
            Case LoopCmd, NextCmd
                depth = depth - 1
                If depth = 0 Then
                    ExecSkipCmd = i
                    Exit Function
                End If
        End Select
    Next
    
    EventErrorMessage = "Skipコマンドがループの外で使われています"
    Error 0
End Function

Private Function ExecSortCmd()
Dim i As Integer, j As Integer, k As Integer
Dim isStringkey As Boolean, isStringValue As Boolean
Dim isAscOrder As Boolean, isSwap As Boolean, isKeySort As Boolean
Dim vname As String, buf As String, value_buf As Variant, num As Integer
Dim var As VarData, array_buf() As Variant, var_buf(2) As Variant

    'array_buf(opt, value)
    ' opt=0…配列の添字
    '    =1…変数のValueTyep
    '    =2…変数の値

    If ArgNum < 2 Then
        EventErrorMessage = "Sortコマンドの引数の数が違います"
        Error 0
    End If
    
    '初期値
    isAscOrder = True       'ソート順を昇順似設定
    isStringkey = False     '配列のインデックスを数値として扱う
    isStringValue = False   '配列の要素を数値として扱う
    isKeySort = False       'インデックスのみのソートではない
    
    For i = 3 To ArgNum
        buf = GetArgAsString(i)
        Select Case buf
            Case "昇順"
                isAscOrder = True
            Case "降順"
                isAscOrder = False
            Case "数値"
                isStringValue = False
            Case "文字"
                isStringValue = True
            Case "インデックスのみ"
                isKeySort = True
            Case "文字インデックス"
                isStringkey = True
            Case Else
                EventErrorMessage = _
                    "Sortコマンドに不正なオプション「" & buf & "」が使われています"
                Error 0
        End Select
    Next

    'ソートする配列変数名
    vname = GetArg(2)
    If Left$(vname, 1) = "$" Then
        vname = Mid$(vname, 2)
    End If
    'Eval関数
    If LCase$(Left$(vname, 5)) = "eval(" Then
        If Right$(vname, 1) = ")" Then
            vname = Mid$(vname, 6, Len(vname) - 6)
            vname = GetValueAsString(vname)
        End If
    End If

    '配列を検索し、配列要素を見つける
    num = 0
    If IsSubLocalVariableDefined(vname) Then
        'サブルーチンローカルな配列
        For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
            With VarStack(i)
                If InStr(.Name, vname & "[") = 1 Then
                    ReDim Preserve array_buf(2, num)
                    
                    buf = Mid$(.Name, InStr(.Name, "[") + 1, InStr2(.Name, "]") - InStr(.Name, "[") - 1)
                    
                    If Not IsNumeric(buf) Then
                        isStringkey = True
                    End If
                    If .VariableType = StringType Then
                        value_buf = .StringValue
                    Else
                        value_buf = .NumericValue
                    End If
                    If Not IsNumeric(value_buf) Then
                        isStringValue = True
                    End If
                    
                    array_buf(0, num) = buf
                    array_buf(1, num) = .VariableType
                    array_buf(2, num) = value_buf
                    
                    num = num + 1
                End If
            End With
        Next
        If num = 0 Then
            ExecSortCmd = LineNum + 1
            Exit Function
        End If
    ElseIf IsLocalVariableDefined(vname) Then
        'ローカルな配列
        For Each var In LocalVariableList
            With var
                If InStr(.Name, vname & "[") = 1 Then
                    ReDim Preserve array_buf(2, num)
                    
                    buf = Mid$(.Name, InStr(.Name, "[") + 1, InStr2(.Name, "]") - InStr(.Name, "[") - 1)
                    
                    If Not IsNumeric(buf) Then
                        isStringkey = True
                    End If
                    If .VariableType = StringType Then
                        value_buf = .StringValue
                    Else
                        value_buf = .NumericValue
                    End If
                    If Not IsNumeric(value_buf) Then
                        isStringValue = True
                    End If
                    
                    array_buf(0, num) = buf
                    array_buf(1, num) = .VariableType
                    array_buf(2, num) = value_buf
                    
                    num = num + 1
                End If
            End With
        Next
        If num = 0 Then
            ExecSortCmd = LineNum + 1
            Exit Function
        End If
    ElseIf IsGlobalVariableDefined(vname) Then
        'グローバルな配列
        For Each var In GlobalVariableList
            With var
                If InStr(.Name, vname & "[") = 1 Then
                    ReDim Preserve array_buf(2, num)
                    
                    buf = Mid$(.Name, InStr(.Name, "[") + 1, InStr2(.Name, "]") - InStr(.Name, "[") - 1)
                    
                    If Not IsNumeric(buf) Then
                        isStringkey = True
                    End If
                    If .VariableType = StringType Then
                        value_buf = .StringValue
                    Else
                        value_buf = .NumericValue
                    End If
                    If Not IsNumeric(value_buf) Then
                        isStringValue = True
                    End If
                    
                    array_buf(0, num) = buf
                    array_buf(1, num) = .VariableType
                    array_buf(2, num) = value_buf
                    
                    num = num + 1
                End If
            End With
        Next
        If num = 0 Then
            ExecSortCmd = LineNum + 1
            Exit Function
        End If
    End If
    
    num = num - 1
    
    If Not isStringkey Or isKeySort Then
        '添字が数値の場合、またはインデックスのみのソートの場合、
        '先に添字の昇順に並び替える
        For i = 0 To num - 1
            For j = num To i + 1 Step -1
                isSwap = False
                
                If isStringkey Then
                    If isAscOrder Then
                        isSwap = IIf(StrComp(array_buf(0, i), array_buf(0, j), vbTextCompare) = 1, True, False)
                    Else
                        isSwap = IIf(StrComp(array_buf(0, i), array_buf(0, j), vbTextCompare) = -1, True, False)
                    End If
                Else
                    If isAscOrder Then
                        isSwap = IIf(CDbl(array_buf(0, i)) > CDbl(array_buf(0, j)), True, False)
                    Else
                        isSwap = IIf(CDbl(array_buf(0, i)) < CDbl(array_buf(0, j)), True, False)
                    End If
                End If
                    
                If isSwap Then
                    For k = 0 To 2
                        var_buf(k) = array_buf(k, i)
                        array_buf(k, i) = array_buf(k, j)
                        array_buf(k, j) = var_buf(k)
                    Next
                End If
            Next
        Next
    End If
    
    If Not isKeySort Then
        '改めて要素をソート
        For i = 0 To num - 1
            For j = num To i + 1 Step -1
                isSwap = False
                If isStringValue Then
                    If isAscOrder Then
                        isSwap = IIf(StrComp(array_buf(2, i), array_buf(2, j), vbTextCompare) = 1, True, False)
                    Else
                        isSwap = IIf(StrComp(array_buf(2, i), array_buf(2, j), vbTextCompare) = -1, True, False)
                    End If
                Else
                    If isAscOrder Then
                        isSwap = IIf(CDbl(array_buf(2, i)) > CDbl(array_buf(2, j)), True, False)
                    Else
                        isSwap = IIf(CDbl(array_buf(2, i)) < CDbl(array_buf(2, j)), True, False)
                    End If
                End If
                
                If isSwap Then
                    For k = IIf(isStringkey, 0, 1) To 2
                        var_buf(k) = array_buf(k, i)
                        array_buf(k, i) = array_buf(k, j)
                        array_buf(k, j) = var_buf(k)
                    Next
                End If
        
            Next
        Next
    End If
    
    'SRC変数に再配置
    For i = 0 To num
        buf = vname & "[" & CStr(array_buf(0, i)) & "]"
        UndefineVariable buf
        If array_buf(1, i) = StringType Then
            SetVariable buf, StringType, CStr(array_buf(2, i)), 0
        Else
            SetVariable buf, NumericType, "", CDbl(array_buf(2, i))
        End If
    Next
    
    ExecSortCmd = LineNum + 1
End Function

Private Function ExecSpecialPowerCmd() As Long
Dim u As Unit, t As Unit
Dim sname As String, sd As SpecialPowerData
Dim need_target As Boolean, msg_window_visible As Boolean
Dim prev_action As Integer

    Select Case ArgNum
        Case 4
            Set u = GetArgAsUnit(2)
            sname = GetArgAsString(3)
            Set t = GetArgAsUnit(4)
        Case 3
            If SPDList.IsDefined(GetArgAsString(2)) Then
                With SPDList.Item(GetArgAsString(2))
                    If .IsEffectAvailable("みがわり") _
                        Or .IsEffectAvailable("挑発") _
                    Then
                        need_target = True
                    End If
                End With
            End If
            
            If need_target Then
                Set u = SelectedUnitForEvent
                sname = GetArgAsString(2)
                Set t = GetArgAsUnit(3)
            Else
                Set u = GetArgAsUnit(2)
                sname = GetArgAsString(3)
            End If
        Case 2
            Set u = SelectedUnitForEvent
            sname = GetArgAsString(2)
        Case Else
            EventErrorMessage = "SpecialPowerコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not SPDList.IsDefined(sname) Then
        EventErrorMessage = "SpecialPowerコマンドで指定されたスペシャルパワー「" & _
            sname & "」が見つかりません"
        Error 0
    End If
    
    Set sd = SPDList.Item(sname)
    
    msg_window_visible = frmMessage.Visible
    
    If sd.Duration = "即効" Then
        Dim prev_target As Unit
        Set prev_target = SelectedTarget
        If Not t Is Nothing Then
            Set SelectedTarget = t
        Else
            Set SelectedTarget = SelectedTargetForEvent
        End If
        prev_action = u.Action
        sd.Execute u.MainPilot, True
        If Not prev_target Is Nothing Then
            Set SelectedTarget = prev_target.CurrentForm
        End If
        If (prev_action = 0 And u.Action > 0) _
            Or (prev_action > 0 And u.Action = 0) _
        Then
            RedrawScreen
        End If
    ElseIf Not t Is Nothing Then
        prev_action = t.Action
        t.MakeSpecialPowerInEffect sname, u.MainPilot.ID
        If (prev_action = 0 And t.Action > 0) _
            Or (prev_action > 0 And t.Action = 0) _
        Then
            RedrawScreen
        End If
    Else
        prev_action = u.Action
        u.MakeSpecialPowerInEffect sname
        If (prev_action = 0 And u.Action > 0) _
            Or (prev_action > 0 And u.Action = 0) _
        Then
            RedrawScreen
        End If
    End If
    
    If Not msg_window_visible Then
        CloseMessageForm
    End If
    
    ExecSpecialPowerCmd = LineNum + 1
End Function

Private Function ExecSplitCmd() As Long
Dim u As Unit

    Select Case ArgNum
        Case 1
            Set u = SelectedUnitForEvent
        Case 2
            Set u = GetArgAsUnit(2)
        Case Else
            EventErrorMessage = "Splitコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        If Not .IsFeatureAvailable("分離") Then
            EventErrorMessage = .Name & "は分離できません"
            Error 0
        End If
        
        .Split
        
        '分離形態の１番目のユニットをメインユニットに設定
        Set u = UList.Item(LIndex(.FeatureData("分離"), 2))
        
        '変数のアップデート
        If Not SelectedUnit Is Nothing Then
            If .ID = SelectedUnit.ID Then
                Set SelectedUnit = u
            End If
        End If
        If Not SelectedUnitForEvent Is Nothing Then
            If .ID = SelectedUnitForEvent.ID Then
                Set SelectedUnitForEvent = u
            End If
        End If
        If Not SelectedTarget Is Nothing Then
            If .ID = SelectedTarget.ID Then
                Set SelectedTarget = u
            End If
        End If
        If Not SelectedTargetForEvent Is Nothing Then
            If .ID = SelectedTargetForEvent.ID Then
                Set SelectedTargetForEvent = u
            End If
        End If
    End With
    
    ExecSplitCmd = LineNum + 1
End Function

Private Function ExecStartBGMCmd() As Long
Dim fname As String
Dim start_bgm_end As Long
Dim i As Long

    'StartBGMコマンドが連続してる場合、最後のStartBGMコマンドの位置を検索
    For i = LineNum + 1 To UBound(EventCmd)
        If EventCmd(i).Name <> StartBGMCmd Then
            Exit For
        End If
    Next
    start_bgm_end = i - 1
    
    '最後のStartBGMから順にMIDIファイルを検索
    For i = start_bgm_end To LineNum Step -1
        fname = ListTail(EventData(i), 2)
        If ListLength(fname) = 1 Then
            If Left(fname, 2) = "$(" Then
                fname = """" & fname & """"
            End If
            fname = GetValueAsString(fname, True)
        Else
            fname = "(" & fname & ")"
        End If
        fname = SearchMidiFile(fname)
        If fname <> "" Then
            'MIDIファイルが存在したので選択
            Exit For
        End If
    Next
    
    'MIDIファイルを再生
    KeepBGM = False
    BossBGM = False
    StartBGM fname
    
    '次のコマンド実行位置は最後のStartBGMコマンドの後
    ExecStartBGMCmd = start_bgm_end + 1
End Function

Private Function ExecStopBGMCmd() As Long
    KeepBGM = False
    BossBGM = False
' MOD START マージ
'    StopBGM
    StopBGM True
' MOD END マージ
    ExecStopBGMCmd = LineNum + 1
End Function

Private Function ExecStopSummoningCmd() As Long
Dim u As Unit

    Select Case ArgNum
        Case 1
            Set u = SelectedUnitForEvent
        Case 2
            Set u = GetArgAsUnit(2)
        Case Else
            EventErrorMessage = "StopSummoningコマンドの引数の数が違います"
            Error 0
    End Select
    
    '召喚したユニットを解放
    u.DismissServant
    
    ExecStopSummoningCmd = LineNum + 1
End Function

Private Function ExecSunsetCmd() As Long
Dim prev_x As Integer, prev_y As Integer
Dim u As Unit
Dim late_refresh As Boolean
Dim i As Integer, buf As String
    
    late_refresh = False
    MapDrawIsMapOnly = False
    For i = 2 To ArgNum
        buf = GetArgAsString(i)
        Select Case buf
            Case "非同期"
                late_refresh = True
            Case "マップ限定"
                MapDrawIsMapOnly = True
            Case Else
                EventErrorMessage = _
                    "Sunsetコマンドに不正なオプション「" & buf & "」が使われています"
                Error 0
        End Select
    Next
    
    prev_x = MapX
    prev_y = MapY
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    SetupBackground "夕焼け", "非同期"
    
    For Each u In UList
        With u
            If .Status = "出撃" Then
                If .BitmapID = 0 Then
                    With UList.Item(.Name)
                        If u.Party0 = .Party0 And .BitmapID <> 0 _
                            And u.Bitmap = .Bitmap _
                            And Not .IsFeatureAvailable("ダミーユニット") _
                        Then
                            u.BitmapID = .BitmapID
                        Else
                            u.BitmapID = MakeUnitBitmap(u)
                        End If
                    End With
                End If
            End If
        End With
    Next
    
    Center prev_x, prev_y
    RedrawScreen late_refresh
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ExecSunsetCmd = LineNum + 1
End Function

Private Function ExecSupplyCmd() As Long
Dim u As Unit

    Select Case ArgNum
        Case 2
            Set u = GetArgAsUnit(2)
        Case 1
            Set u = SelectedUnitForEvent
        Case Else
        EventErrorMessage = "Supplyコマンドの引数の数が違います"
        Error 0
    End Select
    
    If Not u Is Nothing Then
        u.FullSupply
    End If
    
    ExecSupplyCmd = LineNum + 1
End Function

Private Function ExecSwapCmd() As Long
Dim new_var1 As New VarData
Dim new_var2 As New VarData
Dim old_var1    As VarData
Dim old_var2    As VarData
Dim i As Integer
    
    If ArgNum <> 3 Then
        EventErrorMessage = "Swapコマンドの引数の数が違います"
        Error 0
    Else
        '入れ替える前の変数の値を保存
        '引数1の変数
        Set old_var1 = GetVariableObject(GetArg(2))
        If Not old_var1 Is Nothing Then
            With new_var2
                .Name = old_var1.Name
                .VariableType = old_var1.VariableType
                .StringValue = old_var1.StringValue
                .NumericValue = old_var1.NumericValue
            End With
        End If
        '引数2の変数
        Set old_var2 = GetVariableObject(GetArg(3))
        If Not old_var2 Is Nothing Then
            With new_var1
                .Name = old_var2.Name
                .VariableType = old_var2.VariableType
                .StringValue = old_var2.StringValue
                .NumericValue = old_var2.NumericValue
            End With
        End If
        
        '引数2の変数を引数1の変数に代入
        With old_var1
            '引数1がサブルーチンローカル変数の場合
            If CallDepth > 0 Then
                For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
                    If .Name = VarStack(i).Name Then
                        With VarStack(i)
                            .VariableType = new_var1.VariableType
                            .StringValue = new_var1.StringValue
                            .NumericValue = new_var1.NumericValue
                        End With
                        GoTo Swap_Var2toVar1_End
                    End If
                Next
            End If
            
            'ローカル・またはグローバル変数の場合
            .VariableType = new_var1.VariableType
            .StringValue = new_var1.StringValue
            .NumericValue = new_var1.NumericValue
        End With
Swap_Var2toVar1_End:
        '引数1の変数を引数2の変数に代入
        With old_var2
            '引数2がサブルーチンローカル変数の場合
            If CallDepth > 0 Then
                For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
                    If .Name = VarStack(i).Name Then
                        With VarStack(i)
                            .VariableType = new_var2.VariableType
                            .StringValue = new_var2.StringValue
                            .NumericValue = new_var2.NumericValue
                        End With
                        GoTo Swap_Var1toVar2_End
                    End If
                Next
            End If
            
            'ローカル・またはグローバル変数の場合
            .VariableType = new_var2.VariableType
            .StringValue = new_var2.StringValue
            .NumericValue = new_var2.NumericValue
        End With
Swap_Var1toVar2_End:
    End If
        
    'オブジェクトの解放
    Set old_var1 = Nothing
    Set old_var2 = Nothing
    Set new_var1 = Nothing
    Set new_var2 = Nothing
    
    ExecSwapCmd = LineNum + 1
End Function

Private Function ExecSwitchCmd() As Long
Dim i As Long, j As Integer, depth As Integer
Dim a As String, b As String

    If ArgNum <> 2 Then
        EventErrorMessage = "Switchコマンドの引数の数が違います"
        Error 0
    End If
    
    a = GetArgAsString(2)
    
    depth = 1
    For i = LineNum + 1 To UBound(EventCmd)
        With EventCmd(i)
            Select Case .Name
                Case CaseCmd
                    If depth = 1 Then
                        For j = 2 To .ArgNum
                            If .GetArgsType(j) = UndefinedType Then
                                '未識別のパラメータは式として処理する
                                b = .GetArgAsString(j)
                                If b = .GetArg(j) Then
                                    '文字列として識別済みにする
                                    .SetArgsType j, StringType
                                End If
                            Else
                                '識別済みのパラメータは文字列としてそのまま参照する
                                b = .GetArg(j)
                            End If
                            
                            If a = b Then
                                ExecSwitchCmd = i + 1
                                Exit Function
                            End If
                        Next
                    End If
                Case CaseElseCmd
                    If depth = 1 Then
                        ExecSwitchCmd = i + 1
                        Exit Function
                    End If
                Case EndSwCmd
                    If depth = 1 Then
                        ExecSwitchCmd = i + 1
                        Exit Function
                    Else
                        depth = depth - 1
                    End If
                Case SwitchCmd
                    depth = depth + 1
            End Select
        End With
    Next
    
    EventErrorMessage = "SwitchとEndSwが対応していません"
    Error 0
End Function

Private Function ExecCaseCmd() As Long
Dim i As Long, depth As Integer

    '対応するEndSwを探す
    depth = 1
    For i = LineNum + 1 To UBound(EventCmd)
        Select Case EventCmd(i).Name
            Case SwitchCmd
                depth = depth + 1
            Case EndSwCmd
                depth = depth - 1
                If depth = 0 Then
                    ExecCaseCmd = i + 1
                    Exit Function
                End If
        End Select
    Next
    
    EventErrorMessage = "SwitchとEndSwが対応していません"
    Error 0
End Function

Private Function ExecTalkCmd() As Long
Dim pname As String, current_pname As String
Dim u As Unit, ux As Integer, uy As Integer
Dim i As Long, j As Integer
Dim lnum As Integer
Dim without_cursor As Boolean
Dim options As String, opt As String
Dim buf As String

    For i = LineNum To UBound(EventData)
        With EventCmd(i)
            Select Case .Name
                Case TalkCmd
                    If .ArgNum > 1 Then
                        pname = .GetArgAsString(2)
                    Else
                        pname = ""
                    End If
                    
                    If Left$(pname, 1) = "@" Then
                        'メインパイロットの強制指定
                        pname = Mid$(pname, 2)
                        If PList.IsDefined(pname) Then
                            With PList.Item(pname)
                                If Not .Unit Is Nothing Then
                                    pname = .Unit.MainPilot.Name
                                End If
                            End With
                        End If
                    End If
                    
                    '話者名チェック
                    If Not PList.IsDefined(pname) _
                        And Not PDList.IsDefined(pname) _
                        And Not NPDList.IsDefined(pname) _
                        And Not pname = "システム" _
                        And Not pname = "" _
                    Then
                        EventErrorMessage = _
                            "「" & pname & "」というパイロットが定義されていません"
                        LineNum = i
                        Error 0
                    End If
                    
                    If .ArgNum > 1 Then
                        options = ""
                        without_cursor = False
                        j = 2
                        lnum = 1
                        Do While j <= .ArgNum
                            opt = .GetArgAsString(j)
                            Select Case opt
                                Case "非表示"
                                    without_cursor = True
                                 Case "枠外"
                                    MessageWindowIsOut = True
                                Case "白黒", "セピア", _
                                    "明", "暗", "上下反転", "左右反転", _
                                    "上半分", "下半分", "右半分", "左半分", _
                                    "右上", "左上", "右下", "左下", _
                                    "ネガポジ反転", "シルエット", _
                                    "夕焼け", "水中", "通常"
                                    If j > 2 Then
                                        'これらのパイロット画像描画に関するオプションは
                                        'パイロット名が指定されている場合にのみ有効
                                        options = options & opt & " "
                                    Else
                                        lnum = j
                                    End If
                                Case "右回転"
                                    j = j + 1
                                    options = options & _
                                        "右回転 " & .GetArgAsString(j) & " "
                                Case "左回転"
                                    j = j + 1
                                    options = options & _
                                        "左回転 " & .GetArgAsString(j) & " "
                                Case "フィルタ"
                                    Dim cname As String, tcolor As Long
                                    j = j + 1
                                    buf = .GetArgAsString(j)
                                    cname = String$(8, vbNullChar)
                                    Mid(cname, 1, 2) = "&H"
                                    Mid(cname, 3, 2) = Mid$(buf, 6, 2)
                                    Mid(cname, 5, 2) = Mid$(buf, 4, 2)
                                    Mid(cname, 7, 2) = Mid$(buf, 2, 2)
                                    tcolor = CLng(cname)
                                    j = j + 1
                                    options = options & _
                                        "フィルタ " & Format$(tcolor) & " " & .GetArgAsString(j) & " "
                                Case ""
                                    '空白のオプションをスキップ
                                Case Else
                                    '通常の引数をスキップ
                                    lnum = j
                            End Select
                            j = j + 1
                        Loop
                    Else
                        lnum = 1
                    End If
                    
                    Select Case lnum
                        Case 0, 1
                            '引数なし
                            
                            If Not frmMessage.Visible Then
                                OpenMessageForm
                            End If
                            
                            'メッセージウィンドウのパイロット画像を以前指定された
                            'ものに確定させる
                            If current_pname <> "" Then
                                DisplayMessage current_pname, "", options
                            End If
                            
                            current_pname = ""
                            
                        Case 2
                            'パイロット名のみ指定
                            current_pname = pname
                            
                            '話者中心に画面位置を変更
                            
                            'プロローグイベントやエピローグイベント時はキャンセル
                            If Stage = "プロローグ" _
                                Or Stage = "エピローグ" _
                            Then
                                GoTo NextLoop
                            End If
                            
                            '画面書き換え可能？
                            If Not MainForm.Visible Then
                                GoTo NextLoop
                            End If
                            If IsPictureVisible Then
                                GoTo NextLoop
                            End If
                            If MapFileName = "" Then
                                GoTo NextLoop
                            End If
                            
                            '話者を中央表示
                            CenterUnit pname, without_cursor
                            
                        Case 3
                            current_pname = pname
                            Select Case .GetArgAsString(3)
                                Case "母艦"
                                    '母艦の中央表示
                                     CenterUnit "母艦", without_cursor
                                Case "中央"
                                    '話者の中央表示
                                     CenterUnit pname, without_cursor
                                Case "固定"
                                    '表示位置固定
                            End Select
                            
                        Case 4
                            '表示の座標指定あり
                            current_pname = pname
                            CenterUnit pname, without_cursor, .GetArgAsLong(3), .GetArgAsLong(4)
                            
                        Case -1
                            EventErrorMessage = _
                                "Talkコマンドのパラメータの括弧の対応が取れていません"
                            LineNum = i
                            Error 0
                            
                        Case Else
                            EventErrorMessage = "Talkコマンドの引数の数が違います"
                            LineNum = i
                            Error 0
                    End Select
                    
                    If Not frmMessage.Visible Then
                        OpenMessageForm
                    End If
                    
                Case EndCmd
                    CloseMessageForm
                    MessageWindowIsOut = False
                    If .ArgNum <> 1 Then
                        EventErrorMessage = "End部分の引数の数が違います"
                        LineNum = i
                        Error 0
                    End If
                    Exit For
                    
                Case SuspendCmd
                    If .ArgNum <> 1 Then
                        EventErrorMessage = "Suspend部分の引数の数が違います"
                        LineNum = i
                        Error 0
                    End If
                    Exit For
                    
                Case Else
                    If Not frmMessage.Visible Then
                        OpenMessageForm
                    End If
                    DisplayMessage current_pname, EventData(i), options
                    
            End Select
        End With
NextLoop:
    Next
    
    If i > UBound(EventData) Then
        CloseMessageForm
        EventErrorMessage = "TalkとEndが対応していません"
        Error 0
    End If
    
    ExecTalkCmd = i + 1
End Function

Private Function ExecTelopCmd() As Long
Dim msg As String, BGM As String

    msg = ListTail(EventData(LineNum), 2)
    If ListLength(msg) = 1 Then
        msg = GetValueAsString(msg)
    End If
    FormatMessage msg
    
    BGM = SearchMidiFile(BGMName("Subtitle"))
    If Len(BGM) > 0 Then
        StartBGM BGM, False
        If Not IsRButtonPressed() Then
            Sleep 1000
        End If
        DisplayTelop msg
        If Not IsRButtonPressed() Then
            Sleep 2000
        End If
    Else
        DisplayTelop msg
    End If
    
    ExecTelopCmd = LineNum + 1
End Function

Private Function ExecTransformCmd() As Long
Dim u As Unit
Dim tname As String

    Select Case ArgNum
        Case 3
            Set u = GetArgAsUnit(2)
            tname = GetArgAsString(3)
        Case 2
            Set u = SelectedUnitForEvent
            tname = GetArgAsString(2)
        Case Else
            EventErrorMessage = "Transformコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u
        If .Name = tname Then
            '元々指定された形態になっていたので変形の必要なし
            ExecTransformCmd = LineNum + 1
            Exit Function
        End If
        
        '変形
        .Transform tname
        
        'グローバル変数の更新
        If u Is SelectedUnit Then
            Set SelectedUnit = .CurrentForm
        End If
        If u Is SelectedUnitForEvent Then
            Set SelectedUnitForEvent = .CurrentForm
        End If
        If u Is SelectedTarget Then
            Set SelectedTarget = .CurrentForm
        End If
        If u Is SelectedTargetForEvent Then
            Set SelectedTargetForEvent = .CurrentForm
        End If
    End With
    
    ExecTransformCmd = LineNum + 1
End Function

Private Function ExecUnitCmd() As Long
Dim uname As String
Dim u As Unit
Dim urank As Integer

    If ArgNum < 0 Then
        EventErrorMessage = "Unitコマンドのパラメータの括弧の対応が取れていません"
        Error 0
    ElseIf ArgNum <> 3 Then
        EventErrorMessage = "Unitコマンドの引数の数が違います"
        Error 0
    End If
    
    uname = GetArgAsString(2)
    If Not UDList.IsDefined(uname) Then
        EventErrorMessage = "指定したユニット「" & uname & "」のデータが見つかりません"
        Error 0
    End If
    
    urank = GetArgAsLong(3)
    
    Set u = UList.Add(uname, urank, "味方")
    If u Is Nothing Then
        EventErrorMessage = uname & "のユニットデータが不正です"
        Error 0
    End If
    Set SelectedUnitForEvent = u
    
    ExecUnitCmd = LineNum + 1
End Function

Private Function ExecUnsetCmd() As Long
    UndefineVariable GetArg(2)
    
    ExecUnsetCmd = LineNum + 1
End Function

Private Function ExecUpgradeCmd() As Long
Dim uname As String
Dim u As Unit, u1 As Unit, u2 As Unit
Dim i As Integer
Dim prev_status As String

    Select Case ArgNum
        Case 2
            Set u1 = SelectedUnitForEvent.CurrentForm
            uname = GetArgAsString(2)
        Case 3
            uname = GetArgAsString(2)
            If Not UList.IsDefined(uname) Then
                EventErrorMessage = uname & "というユニットはありません"
                Error 0
            End If
            Set u1 = UList.Item(uname).CurrentForm
            
            uname = GetArgAsString(3)
        Case Else
            EventErrorMessage = "Upgradeコマンドの引数の数が違います"
            Error 0
    End Select
    
    If Not UDList.IsDefined(uname) Then
        EventErrorMessage = "ユニット「" & uname & "」のデータが見つかりません"
        Error 0
    End If
    
    prev_status = u1.Status
    
    Set u2 = UList.Add(uname, u1.Rank, u1.Party0)
    If u2 Is Nothing Then
        EventErrorMessage = uname & "のユニットデータが不正です"
        Error 0
    End If
    
    If u1.BossRank > 0 Then
        u2.BossRank = u1.BossRank
        u2.FullRecover
    End If
    
    'パイロットの乗せ換え
    If u1.CountPilot > 0 Then
        Dim pilot_list() As Pilot
        Dim support_list() As Pilot
        
        ReDim pilot_list(u1.CountPilot)
        ReDim support_list(u1.CountSupport)
        
        For i = 1 To UBound(pilot_list)
            Set pilot_list(i) = u1.Pilot(i)
        Next
        For i = 1 To UBound(support_list)
            Set support_list(i) = u1.Support(i)
        Next
        
        u1.Pilot(1).GetOff
        
        For i = 1 To UBound(pilot_list)
            pilot_list(i).Ride u2
        Next
        For i = 1 To UBound(support_list)
            support_list(i).Ride u2
        Next
    End If
    
    'アイテムの交換
    For i = 1 To u1.CountItem
        u2.AddItem u1.Item(i)
    Next
    For i = 1 To u1.CountItem
        u1.DeleteItem 1
    Next
    
    'リンクの付け替え
    Set u2.Master = u1.Master
    Set u1.Master = Nothing
    Set u2.Summoner = u1.Summoner
    Set u1.Summoner = Nothing
    
    '召喚ユニットの交換
    For i = 1 To u1.CountServant
        u2.AddServant u1.Servant(i)
    Next
    For i = 1 To u1.CountServant
        u1.DeleteServant 1
    Next
    
    '収納ユニットの交換
    If u1.IsFeatureAvailable("母艦") Then
        For i = 1 To u1.CountOtherForm
            If u1.OtherForm(i).Status = "格納" Then
                u2.AddOtherForm u1.OtherForm(i)
            End If
        Next
        For i = 1 To u2.CountOtherForm
            If u2.OtherForm(i).Status = "格納" Then
                u1.DeleteOtherForm u2.OtherForm(i).ID
            End If
        Next
    End If
    
    u2.Area = u1.Area
    
    '元のユニットを削除
    u1.Status = "破棄"
    For i = 1 To u1.CountOtherForm
        If u1.OtherForm(i).Status = "他形態" Then
            u1.OtherForm(i).Status = "破棄"
        End If
    Next
    
    u2.UsedAction = u1.UsedAction
    u2.UsedSupportAttack = u1.UsedSupportAttack
    u2.UsedSupportGuard = u1.UsedSupportGuard
    u2.UsedSyncAttack = u1.UsedSyncAttack
    u2.UsedCounterAttack = u1.UsedCounterAttack
    
    Select Case prev_status
        Case "出撃"
            Set MapDataForUnit(u1.X, u1.Y) = Nothing
            u2.StandBy u1.X, u1.Y
            If Not IsPictureVisible Then
                RedrawScreen
            End If
        Case "破壊", "破棄"
            If MapDataForUnit(u1.X, u1.Y) Is u1 Then
                Set MapDataForUnit(u1.X, u1.Y) = Nothing
            End If
            u2.StandBy u1.X, u1.Y
            If Not IsPictureVisible Then
                RedrawScreen
            End If
        Case "格納"
            For Each u In UList
                With u
                    For i = 1 To .CountUnitOnBoard
                        If u1.ID = .UnitOnBoard(i).ID Then
                            .UnloadUnit u1.ID
                            u2.Land u, True
                            GoTo ExitLoop
                        End If
                    Next
                End With
            Next
ExitLoop:
        Case Else
            u2.Status = prev_status
    End Select
    
    'グローバル変数の更新
    If u1 Is SelectedUnit Then
        Set SelectedUnit = u2
    End If
    If u1 Is SelectedUnitForEvent Then
        Set SelectedUnitForEvent = u2
    End If
    If u1 Is SelectedTarget Then
        Set SelectedTarget = u2
    End If
    If u1 Is SelectedTargetForEvent Then
        Set SelectedTargetForEvent = u2
    End If
    For i = 1 To SelectionStackIndex
        If u1 Is SavedSelectedUnit(i) Then
            Set SavedSelectedUnit(i) = u2
        End If
        If u1 Is SavedSelectedUnitForEvent(i) Then
            Set SavedSelectedUnitForEvent(i) = u2
        End If
        If u1 Is SavedSelectedTarget(i) Then
            Set SavedSelectedTarget(i) = u2
        End If
        If u1 Is SavedSelectedTargetForEvent(i) Then
            Set SavedSelectedTargetForEvent(i) = u2
        End If
    Next
    
    ExecUpgradeCmd = LineNum + 1
End Function

Private Function ExecUpvarCmd() As Long
    UpVarLevel = UpVarLevel + 1
    ExecUpvarCmd = LineNum + 1
End Function

Private Function ExecUseAbilityCmd() As Long
Dim u1 As Unit, u2 As Unit
Dim aname As String
Dim a As Integer

    Select Case ArgNum
        Case 4
            Set u1 = GetArgAsUnit(2)
            
            aname = GetArgAsString(3)
            For a = 1 To u1.CountAbility
                If aname = u1.Ability(a).Name Then
                    Exit For
                End If
            Next
            If a > u1.CountAbility Then
                EventErrorMessage = "アビリティ名が間違っています"
                Error 0
            End If
            
            Set u2 = GetArgAsUnit(4)
        Case 3
            Set u1 = SelectedUnitForEvent
            If Not u1 Is Nothing Then
                aname = GetArgAsString(2)
                For a = 1 To u1.CountAbility
                    If aname = u1.Ability(a).Name Then
                        Exit For
                    End If
                Next
                
                If a <= u1.CountAbility Then
                    Set u2 = GetArgAsUnit(3)
                Else
                    Set u1 = GetArgAsUnit(2)
                    
                    aname = GetArgAsString(3)
                    For a = 1 To u1.CountAbility
                        If aname = u1.Ability(a).Name Then
                            Exit For
                        End If
                    Next
                    If a > u1.CountAbility Then
                        EventErrorMessage = "アビリティ名が間違っています"
                        Error 0
                    End If
                    
                    Set u2 = u1
                End If
            Else
                Set u1 = GetArgAsUnit(2)
                
                aname = GetArgAsString(3)
                For a = 1 To u1.CountAbility
                    If aname = u1.Ability(a).Name Then
                        Exit For
                    End If
                Next
                If a > u1.CountAbility Then
                    EventErrorMessage = "アビリティ名が間違っています"
                    Error 0
                End If
                
                Set u2 = u1
            End If
        Case 2
            Set u1 = SelectedUnitForEvent
            
            aname = GetArgAsString(2)
            For a = 1 To u1.CountAbility
                If aname = u1.Ability(a).Name Then
                    Exit For
                End If
            Next
            If a > u1.CountAbility Then
                EventErrorMessage = "アビリティ名が間違っています"
                Error 0
            End If
            
            Set u2 = SelectedUnitForEvent
        Case Else
            EventErrorMessage = "UseAbilityコマンドの引数の数が違います"
            Error 0
    End Select
    
    With u1
        If .Status <> "出撃" Then
            EventErrorMessage = .Nickname & "は出撃していません"
            Error 0
        End If
        .ExecuteAbility a, u2, False, True
        CloseMessageForm
    End With
    
    RedrawScreen
    
    ExecUseAbilityCmd = LineNum + 1
End Function

Private Function ExecWaitCmd() As Long
Dim i As Integer
Dim start_time As Long, wait_time As Long, cur_time As Long

    Select Case ArgNum
        Case 2
            Select Case LCase$(GetArg(2))
                Case "start"
                    WaitStartTime = timeGetTime()
                    WaitTimeCount = 0
                    
                Case "reset"
                    WaitStartTime = -1
                    WaitTimeCount = 0
                    
                Case "click"
                    '先行入力されていたクリックイベントを解消
                    DoEvents
                    WaitClickMode = True
                    IsFormClicked = False
                    SelectedAlternative = ""
                    
                    'ウィンドウが表示されていない場合は表示
                    With MainForm
                        If Not .Visible Then
                            .Show
                            .Refresh
                        End If
                    End With
                    
                    'クリックされるまで待つ
                    Do Until IsFormClicked
                        If IsRButtonPressed(True) Then
                            MouseButton = 0
                            Exit Do
                        End If
                        DoEvents
                        Sleep 25
                    Loop
                    
                    'マウスの左ボタンが押された場合はホットポイントの判定を行う
                    If SelectedAlternative = "" And MouseButton = 1 Then
                        For i = 1 To UBound(HotPointList)
                            With HotPointList(i)
                                If .Left <= MouseX And MouseX < .Left + .width _
                                    And .Top <= MouseY And MouseY < .Top + .Height _
                                Then
                                    SelectedAlternative = .Name
                                    Exit For
                                End If
                            End With
                        Next
                    End If
                    
                    WaitClickMode = False
                    IsFormClicked = False
                    
                Case Else
                    wait_time = 100 * GetArgAsDouble(2)
                    
                    '待ち時間が切れるまで待機
                    If wait_time < 1000 Then
                        If Not IsRButtonPressed(True) Then
                            DoEvents
                            Sleep wait_time
                        End If
                    Else
                        start_time = timeGetTime()
                        Do While (start_time + wait_time > timeGetTime())
                            '右ボタンを押されていたら早送り
                            If IsRButtonPressed(True) Then
                                Exit Do
                            End If
                            
                            DoEvents
                            Sleep 25
                        Loop
                    End If
            End Select
            
        Case 3
            'Wait Until ～
            
            wait_time = 100 * GetArgAsDouble(3)
            WaitTimeCount = WaitTimeCount + 1
            
            If WaitStartTime = -1 Then
                'Wait Reset が実行されていた場合
                WaitStartTime = timeGetTime()
            ElseIf wait_time < 100 Then
                'アニメの１回目の表示は例外的に時間がかかってしまうことがある
                'ので、超過時間を無視する
                If WaitTimeCount = 1 Then
                    cur_time = timeGetTime()
                    If WaitStartTime + wait_time > cur_time Then
                        WaitStartTime = cur_time
                    End If
                End If
            End If
            
            Do While (WaitStartTime + wait_time > timeGetTime())
                If IsRButtonPressed(True) Then
                    Exit Do
                End If
                
                DoEvents
                Sleep 25
            Loop
            
        Case Else
            EventErrorMessage = "Waitコマンドの引数の数が違います"
            Error 0
    End Select
    
    ExecWaitCmd = LineNum + 1
End Function

Private Function ExecWaterCmd() As Long
Dim prev_x As Integer, prev_y As Integer
Dim u As Unit
Dim late_refresh As Boolean
Dim i As Integer, buf As String
    
    late_refresh = False
    MapDrawIsMapOnly = False
    For i = 2 To ArgNum
        buf = GetArgAsString(i)
        Select Case buf
            Case "非同期"
                late_refresh = True
            Case "マップ限定"
                MapDrawIsMapOnly = True
            Case Else
                EventErrorMessage = _
                    "Waterコマンドに不正なオプション「" & buf & "」が使われています"
                Error 0
        End Select
    Next
    
    prev_x = MapX
    prev_y = MapY
    
    'マウスカーソルを砂時計に
    Screen.MousePointer = vbHourglass
    
    SetupBackground "水中", "非同期"
    
    For Each u In UList
        With u
            If .Status = "出撃" Then
                If .BitmapID = 0 Then
                    With UList.Item(.Name)
                        If u.Party0 = .Party0 And .BitmapID <> 0 _
                            And u.Bitmap = .Bitmap _
                            And Not .IsFeatureAvailable("ダミーユニット") _
                        Then
                            u.BitmapID = .BitmapID
                        Else
                            u.BitmapID = MakeUnitBitmap(u)
                        End If
                    End With
                End If
            End If
        End With
    Next
    
    Center prev_x, prev_y
    RedrawScreen late_refresh
    
    'マウスカーソルを元に戻す
    Screen.MousePointer = 0
    
    ExecWaterCmd = LineNum + 1
End Function

Private Function ExecWhiteInCmd() As Long
Dim start_time As Long, cur_time As Long, wait_time As Long
Dim i As Long, num As Integer, ret As Long

    Select Case ArgNum
        Case 1
            num = 10
        Case 2
            num = GetArgAsLong(2)
        Case Else
            EventErrorMessage = "WhiteInコマンドの引数の数が違います"
            Error 0
    End Select
    
    SaveScreen
    
    With MainForm
        With .picTmp
            .Picture = LoadPicture("")
            .width = MainPWidth
            .Height = MainPHeight
        End With
        
' MOD START マージ
'        ret = BitBlt(.picTmp.hDC, _
'            0, 0, MapPWidth, MapPHeight, _
'            .picMain(0).hDC, 0, 0, SRCCOPY)
        ret = BitBlt(.picTmp.hDC, _
            0, 0, MainPWidth, MainPHeight, _
            .picMain(0).hDC, 0, 0, SRCCOPY)
' MOD END マージ
        
        InitFade .picMain(0), num, True
        
        start_time = timeGetTime()
        wait_time = 50
        For i = 0 To num
            If i Mod 4 = 0 Then
                If IsRButtonPressed() Then
                    Exit For
                End If
            End If
            
            DoFade .picMain(0), i
            .picMain(0).Refresh
            
            cur_time = timeGetTime()
            Do While cur_time < start_time + wait_time * (i + 1)
                DoEvents
                cur_time = timeGetTime()
            Loop
        Next
        
        FinishFade
        
        ret = BitBlt(.picMain(0).hDC, _
            0, 0, MapPWidth, MapPHeight, _
            .picTmp.hDC, 0, 0, SRCCOPY)
        .picMain(0).Refresh
        
        With .picTmp
            .Picture = LoadPicture("")
            .width = 32
            .Height = 32
        End With
    End With
    
    ExecWhiteInCmd = LineNum + 1
End Function

Private Function ExecWhiteOutCmd() As Long
Dim start_time As Long, cur_time As Long, wait_time As Long
Dim i As Long, num As Integer, ret As Long

    Select Case ArgNum
        Case 1
            num = 10
        Case 2
            num = GetArgAsLong(2)
        Case Else
            EventErrorMessage = "WhiteOutコマンドの引数の数が違います"
            Error 0
    End Select
    
    SaveScreen
    
    With MainForm
        InitFade .picMain(0), num, True
        
        start_time = timeGetTime()
        wait_time = 50
        For i = 0 To num
            If i Mod 4 = 0 Then
                If IsRButtonPressed() Then
                    With .picMain(0)
                        ret = PatBlt(.hDC, 0, 0, .width, .Height, WHITENESS)
                        .Refresh
                    End With
                    Exit For
                End If
            End If
            
            DoFade .picMain(0), num - i
            .picMain(0).Refresh
            
            cur_time = timeGetTime()
            Do While cur_time < start_time + wait_time * (i + 1)
                DoEvents
                cur_time = timeGetTime()
            Loop
        Next
        
        FinishFade
    End With
    
    IsPictureVisible = True
    PaintedAreaX1 = MainPWidth
    PaintedAreaY1 = MainPHeight
    PaintedAreaX2 = -1
    PaintedAreaY2 = -1
    
    ExecWhiteOutCmd = LineNum + 1
End Function

Private Function ExecWriteCmd() As Long
Dim f As Integer
Dim i As Integer
    
    If ArgNum < 3 Then
        EventErrorMessage = "Writeコマンドの引数の数が違います"
        Error 0
    End If
    
    f = GetArgAsLong(2)
    For i = 3 To ArgNum
        Write #f, GetArgAsString(i)
    Next
    
    ExecWriteCmd = LineNum + 1
End Function

'Flashファイルの再生
Private Function ExecPlayFlashCmd() As Long
Dim fname As String
Dim fx As Integer, fy As Integer, fw As Integer, fh As Integer
Dim i As Integer, opt As String, buf As String

    If ArgNum < 6 Then
        EventErrorMessage = "PlayFlashコマンドの引数の数が違います"
        Error 0
    End If
    
    fname = GetArgAsString(2)
    fw = GetArgAsLong(5)
    fh = GetArgAsLong(6)
    buf = GetArgAsString(3)
    If buf = "-" Then
        fx = (480 - fw) / 2
    Else
        fx = CInt(buf)
    End If
    buf = GetArgAsString(4)
    If buf = "-" Then
        fy = (480 - fh) / 2
    Else
        fy = CInt(buf)
    End If
    
    opt = ""
    For i = 7 To ArgNum
        opt = opt & " " & GetArgAsString(i)
    Next
    opt = Trim$(opt)
    
    PlayFlash fname, fx, fy, fw, fh, opt
    
    ExecPlayFlashCmd = LineNum + 1
End Function

'Flashファイルの消去
Private Function ExecClearFlashCmd() As Long
    ClearFlash
    
    ExecClearFlashCmd = LineNum + 1
End Function



