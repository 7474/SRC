Option Strict Off
Option Explicit On
Friend Class CmdData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'イベントコマンドのクラス
	
	'コマンドの種類
	Private CmdName As Event.CmdType
	'引数の数
	Public ArgNum As Short
	'コマンドのEventDataにおける位置
	Public LineNum As Integer
	
	'引数の値
	Private lngArgs() As Integer
	Private dblArgs() As Double
	Private strArgs() As String
	
	'引数の型
	Private ArgsType() As Expression.ValueType
	
	
	'コマンドの種類
	
	Public Property Name() As Event.CmdType
		Get
			If CmdName = Event_Renamed.CmdType.NullCmd Then
				Parse(EventData(LineNum))
			End If
			Name = CmdName
		End Get
		Set(ByVal Value As Event.CmdType)
			CmdName = Value
		End Set
	End Property
	
	'イベントデータ行を読み込んで解析する
	Public Function Parse(ByRef edata As String) As Boolean
		Dim buf, expr As String
		Dim list() As String
		Dim i As Short
		
		'正常に解析が終了した場合はTrueを返すこと
		Parse = True
		
		On Error GoTo ErrorHandler
		
		'空行は無視
		If Len(edata) = 0 Then
			CmdName = Event_Renamed.CmdType.NopCmd
			ArgNum = 0
			Exit Function
		End If
		
		'ラベルは無視
		If Right(edata, 1) = ":" Then
			CmdName = Event_Renamed.CmdType.NopCmd
			ArgNum = 0
			Exit Function
		End If
		
		'コマンドのパラメータ分割
		ArgNum = ListSplit(edata, list)
		
		'空行は無視
		If ArgNum = 0 Then
			CmdName = Event_Renamed.CmdType.NopCmd
			Exit Function
		End If
		
		'パラメータの処理
		If ArgNum > 1 Then
			'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
			ReDim strArgs(ArgNum)
			'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
			ReDim lngArgs(ArgNum)
			'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
			ReDim dblArgs(ArgNum)
			'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
			ReDim ArgsType(ArgNum)
			For i = 2 To ArgNum
				buf = list(i)
				strArgs(i) = buf
				ArgsType(i) = Expression.ValueType.UndefinedType
				
				'先頭の一文字からパラメータの属性を判定
				Select Case Asc(buf)
					Case 0 '空文字列
						ArgsType(i) = Expression.ValueType.StringType
					Case 34 '"
						If Right(buf, 1) = """" Then
							If InStr(buf, "$(") = 0 Then
								ArgsType(i) = Expression.ValueType.StringType
								strArgs(i) = Mid(buf, 2, Len(buf) - 2)
							End If
						Else
							ArgsType(i) = Expression.ValueType.StringType
						End If
					Case 40 '(
						'式
					Case 45 '-
						If IsNumeric(buf) Then
							lngArgs(i) = StrToLng(buf)
							dblArgs(i) = CDbl(buf)
							ArgsType(i) = Expression.ValueType.NumericType
						Else
							ArgsType(i) = Expression.ValueType.StringType
						End If
					Case 48 To 57 '0～9
						If IsNumeric(buf) Then
							lngArgs(i) = StrToLng(buf)
							dblArgs(i) = CDbl(buf)
							ArgsType(i) = Expression.ValueType.NumericType
						Else
							ArgsType(i) = Expression.ValueType.StringType
						End If
					Case 96 '`
						If Right(buf, 1) = "`" Then
							strArgs(i) = Mid(buf, 2, Len(buf) - 2)
						End If
						ArgsType(i) = Expression.ValueType.StringType
				End Select
			Next 
		End If
		
		'コマンドの種類を判定
		Select Case LCase(list(1))
			Case "arc"
				CmdName = Event_Renamed.CmdType.ArcCmd
			Case "array"
				CmdName = Event_Renamed.CmdType.ArrayCmd
			Case "ask"
				CmdName = Event_Renamed.CmdType.AskCmd
			Case "attack"
				CmdName = Event_Renamed.CmdType.AttackCmd
			Case "autotalk"
				CmdName = Event_Renamed.CmdType.AutoTalkCmd
			Case "bossrank"
				CmdName = Event_Renamed.CmdType.BossRankCmd
			Case "break"
				CmdName = Event_Renamed.CmdType.BreakCmd
			Case "call"
				CmdName = Event_Renamed.CmdType.CallCmd
			Case "return"
				CmdName = Event_Renamed.CmdType.ReturnCmd
			Case "callintermissioncommand"
				CmdName = Event_Renamed.CmdType.CallInterMissionCommandCmd
			Case "cancel"
				CmdName = Event_Renamed.CmdType.CancelCmd
			Case "center"
				CmdName = Event_Renamed.CmdType.CenterCmd
			Case "changearea"
				CmdName = Event_Renamed.CmdType.ChangeAreaCmd
				'ADD START 240a
			Case "changelayer"
				CmdName = Event_Renamed.CmdType.ChangeLayerCmd
				'ADD  END  240a
			Case "changemap"
				CmdName = Event_Renamed.CmdType.ChangeMapCmd
			Case "changemode"
				CmdName = Event_Renamed.CmdType.ChangeModeCmd
			Case "changeparty"
				CmdName = Event_Renamed.CmdType.ChangePartyCmd
			Case "changeterrain"
				CmdName = Event_Renamed.CmdType.ChangeTerrainCmd
			Case "changeunitbitmap"
				CmdName = Event_Renamed.CmdType.ChangeUnitBitmapCmd
			Case "charge"
				CmdName = Event_Renamed.CmdType.ChargeCmd
			Case "circle"
				CmdName = Event_Renamed.CmdType.CircleCmd
			Case "clearevent"
				CmdName = Event_Renamed.CmdType.ClearEventCmd
			Case "clearimage"
				CmdName = Event_Renamed.CmdType.ClearImageCmd
				'ADD START 240a
			Case "clearlayer"
				CmdName = Event_Renamed.CmdType.ClearLayerCmd
				'ADD  END  240a
			Case "clearobj"
				CmdName = Event_Renamed.CmdType.ClearObjCmd
			Case "clearpicture"
				CmdName = Event_Renamed.CmdType.ClearPictureCmd
			Case "clearskill", "clearability"
				CmdName = Event_Renamed.CmdType.ClearSkillCmd
			Case "clearspecialpower", "clearmind"
				CmdName = Event_Renamed.CmdType.ClearSpecialPowerCmd
			Case "clearstatus"
				CmdName = Event_Renamed.CmdType.ClearStatusCmd
			Case "cls"
				CmdName = Event_Renamed.CmdType.ClsCmd
			Case "close"
				CmdName = Event_Renamed.CmdType.CloseCmd
			Case "color"
				CmdName = Event_Renamed.CmdType.ColorCmd
			Case "colorfilter"
				CmdName = Event_Renamed.CmdType.ColorFilterCmd
			Case "combine"
				CmdName = Event_Renamed.CmdType.CombineCmd
			Case "confirm"
				CmdName = Event_Renamed.CmdType.ConfirmCmd
			Case "continue"
				CmdName = Event_Renamed.CmdType.ContinueCmd
			Case "copyarray"
				CmdName = Event_Renamed.CmdType.CopyArrayCmd
			Case "copyfile"
				CmdName = Event_Renamed.CmdType.CopyFileCmd
			Case "create"
				CmdName = Event_Renamed.CmdType.CreateCmd
			Case "createfolder"
				CmdName = Event_Renamed.CmdType.CreateFolderCmd
			Case "debug"
				CmdName = Event_Renamed.CmdType.DebugCmd
			Case "destroy"
				CmdName = Event_Renamed.CmdType.DestroyCmd
			Case "disable"
				CmdName = Event_Renamed.CmdType.DisableCmd
			Case "do"
				CmdName = Event_Renamed.CmdType.DoCmd
				If ArgNum = 3 Then
					strArgs(2) = LCase(strArgs(2))
				End If
			Case "loop"
				CmdName = Event_Renamed.CmdType.LoopCmd
				If ArgNum = 3 Then
					strArgs(2) = LCase(strArgs(2))
				End If
			Case "drawoption"
				CmdName = Event_Renamed.CmdType.DrawOptionCmd
			Case "drawwidth"
				CmdName = Event_Renamed.CmdType.DrawWidthCmd
			Case "enable"
				CmdName = Event_Renamed.CmdType.EnableCmd
			Case "equip"
				CmdName = Event_Renamed.CmdType.EquipCmd
			Case "escape"
				CmdName = Event_Renamed.CmdType.EscapeCmd
			Case "exchangeitem"
				CmdName = Event_Renamed.CmdType.ExchangeItemCmd
			Case "exec"
				CmdName = Event_Renamed.CmdType.ExecCmd
			Case "exit"
				CmdName = Event_Renamed.CmdType.ExitCmd
			Case "explode"
				CmdName = Event_Renamed.CmdType.ExplodeCmd
			Case "expup"
				CmdName = Event_Renamed.CmdType.ExpUpCmd
			Case "fadein"
				CmdName = Event_Renamed.CmdType.FadeInCmd
			Case "fadeout"
				CmdName = Event_Renamed.CmdType.FadeOutCmd
			Case "fillcolor"
				CmdName = Event_Renamed.CmdType.FillColorCmd
			Case "fillstyle"
				CmdName = Event_Renamed.CmdType.FillStyleCmd
			Case "finish"
				CmdName = Event_Renamed.CmdType.FinishCmd
			Case "fix"
				CmdName = Event_Renamed.CmdType.FixCmd
			Case "for"
				CmdName = Event_Renamed.CmdType.ForCmd
			Case "foreach"
				CmdName = Event_Renamed.CmdType.ForEachCmd
			Case "next"
				CmdName = Event_Renamed.CmdType.NextCmd
			Case "font"
				CmdName = Event_Renamed.CmdType.FontCmd
			Case "forget"
				CmdName = Event_Renamed.CmdType.ForgetCmd
			Case "gameclear"
				CmdName = Event_Renamed.CmdType.GameClearCmd
			Case "gameover"
				CmdName = Event_Renamed.CmdType.GameOverCmd
			Case "freememory"
				CmdName = Event_Renamed.CmdType.FreeMemoryCmd
			Case "getoff"
				CmdName = Event_Renamed.CmdType.GetOffCmd
			Case "global"
				CmdName = Event_Renamed.CmdType.GlobalCmd
			Case "goto"
				CmdName = Event_Renamed.CmdType.GotoCmd
			Case "hide"
				CmdName = Event_Renamed.CmdType.HideCmd
			Case "hotpoint"
				CmdName = Event_Renamed.CmdType.HotPointCmd
			Case "if"
				CmdName = Event_Renamed.CmdType.IfCmd
			Case "else"
				CmdName = Event_Renamed.CmdType.ElseCmd
			Case "elseif"
				CmdName = Event_Renamed.CmdType.ElseIfCmd
			Case "endif"
				CmdName = Event_Renamed.CmdType.EndIfCmd
			Case "incr"
				CmdName = Event_Renamed.CmdType.IncrCmd
			Case "increasemorale"
				CmdName = Event_Renamed.CmdType.IncreaseMoraleCmd
			Case "input"
				CmdName = Event_Renamed.CmdType.InputCmd
			Case "intermissioncommand"
				CmdName = Event_Renamed.CmdType.IntermissionCommandCmd
			Case "item"
				CmdName = Event_Renamed.CmdType.ItemCmd
			Case "join"
				CmdName = Event_Renamed.CmdType.JoinCmd
			Case "keepbgm"
				CmdName = Event_Renamed.CmdType.KeepBGMCmd
			Case "land"
				CmdName = Event_Renamed.CmdType.LandCmd
			Case "launch"
				CmdName = Event_Renamed.CmdType.LaunchCmd
			Case "leave"
				CmdName = Event_Renamed.CmdType.LeaveCmd
			Case "levelup"
				CmdName = Event_Renamed.CmdType.LevelUpCmd
			Case "line"
				CmdName = Event_Renamed.CmdType.LineCmd
			Case "lineread"
				CmdName = Event_Renamed.CmdType.LineReadCmd
			Case "load"
				CmdName = Event_Renamed.CmdType.LoadCmd
			Case "local"
				CmdName = Event_Renamed.CmdType.LocalCmd
			Case "makepilotlist"
				CmdName = Event_Renamed.CmdType.MakePilotListCmd
			Case "makeunitlist"
				CmdName = Event_Renamed.CmdType.MakeUnitListCmd
			Case "mapability"
				CmdName = Event_Renamed.CmdType.MapAbilityCmd
			Case "mapattack", "mapweapon"
				CmdName = Event_Renamed.CmdType.MapAttackCmd
			Case "money"
				CmdName = Event_Renamed.CmdType.MoneyCmd
			Case "monotone"
				CmdName = Event_Renamed.CmdType.MonotoneCmd
			Case "move"
				CmdName = Event_Renamed.CmdType.MoveCmd
			Case "night"
				CmdName = Event_Renamed.CmdType.NightCmd
			Case "noon"
				CmdName = Event_Renamed.CmdType.NoonCmd
			Case "open"
				CmdName = Event_Renamed.CmdType.OpenCmd
			Case "option"
				CmdName = Event_Renamed.CmdType.OptionCmd
			Case "organize"
				CmdName = Event_Renamed.CmdType.OrganizeCmd
			Case "oval"
				CmdName = Event_Renamed.CmdType.OvalCmd
			Case "paintpicture"
				CmdName = Event_Renamed.CmdType.PaintPictureCmd
			Case "paintstring"
				CmdName = Event_Renamed.CmdType.PaintStringCmd
			Case "paintsysstring"
				CmdName = Event_Renamed.CmdType.PaintSysStringCmd
			Case "pilot"
				CmdName = Event_Renamed.CmdType.PilotCmd
			Case "playmidi"
				CmdName = Event_Renamed.CmdType.PlayMIDICmd
			Case "playsound"
				CmdName = Event_Renamed.CmdType.PlaySoundCmd
			Case "polygon"
				CmdName = Event_Renamed.CmdType.PolygonCmd
			Case "print"
				CmdName = Event_Renamed.CmdType.PrintCmd
			Case "pset"
				CmdName = Event_Renamed.CmdType.PSetCmd
			Case "question"
				CmdName = Event_Renamed.CmdType.QuestionCmd
			Case "quickload"
				CmdName = Event_Renamed.CmdType.QuickLoadCmd
			Case "quit"
				CmdName = Event_Renamed.CmdType.QuitCmd
			Case "rankup"
				CmdName = Event_Renamed.CmdType.RankUpCmd
			Case "read"
				CmdName = Event_Renamed.CmdType.ReadCmd
			Case "recoveren"
				CmdName = Event_Renamed.CmdType.RecoverENCmd
			Case "recoverhp"
				CmdName = Event_Renamed.CmdType.RecoverHPCmd
			Case "recoverplana"
				CmdName = Event_Renamed.CmdType.RecoverPlanaCmd
			Case "recoversp"
				CmdName = Event_Renamed.CmdType.RecoverSPCmd
			Case "redraw"
				CmdName = Event_Renamed.CmdType.RedrawCmd
			Case "refresh"
				CmdName = Event_Renamed.CmdType.RefreshCmd
			Case "release"
				CmdName = Event_Renamed.CmdType.ReleaseCmd
			Case "removefile"
				CmdName = Event_Renamed.CmdType.RemoveFileCmd
			Case "removefolder"
				CmdName = Event_Renamed.CmdType.RemoveFolderCmd
			Case "removeitem"
				CmdName = Event_Renamed.CmdType.RemoveItemCmd
			Case "removepilot"
				CmdName = Event_Renamed.CmdType.RemovePilotCmd
			Case "removeunit"
				CmdName = Event_Renamed.CmdType.RemoveUnitCmd
			Case "renamebgm"
				CmdName = Event_Renamed.CmdType.RenameBGMCmd
			Case "renamefile"
				CmdName = Event_Renamed.CmdType.RenameFileCmd
			Case "renameterm"
				CmdName = Event_Renamed.CmdType.RenameTermCmd
			Case "replacepilot"
				CmdName = Event_Renamed.CmdType.ReplacePilotCmd
			Case "require"
				CmdName = Event_Renamed.CmdType.RequireCmd
			Case "restoreevent"
				CmdName = Event_Renamed.CmdType.RestoreEventCmd
			Case "ride"
				CmdName = Event_Renamed.CmdType.RideCmd
			Case "select"
				CmdName = Event_Renamed.CmdType.SelectCmd
			Case "savedata"
				CmdName = Event_Renamed.CmdType.SaveDataCmd
			Case "selecttarget"
				CmdName = Event_Renamed.CmdType.SelectTargetCmd
			Case "sepia"
				CmdName = Event_Renamed.CmdType.SepiaCmd
			Case "set"
				CmdName = Event_Renamed.CmdType.SetCmd
			Case "setbullet"
				CmdName = Event_Renamed.CmdType.SetBulletCmd
			Case "setmessage"
				CmdName = Event_Renamed.CmdType.SetMessageCmd
			Case "setrelation"
				CmdName = Event_Renamed.CmdType.SetRelationCmd
			Case "setskill", "setability"
				CmdName = Event_Renamed.CmdType.SetSkillCmd
			Case "setstatus"
				CmdName = Event_Renamed.CmdType.SetStatusCmd
				'ADD START 240a
			Case "setstatusstringcolor"
				CmdName = Event_Renamed.CmdType.SetStatusStringColorCmd
				'ADD  END
			Case "setstock"
				CmdName = Event_Renamed.CmdType.SetStockCmd
				'ADD START 240a
			Case "setwindowcolor"
				CmdName = Event_Renamed.CmdType.SetWindowColorCmd
			Case "setwindowframewidth"
				CmdName = Event_Renamed.CmdType.SetWindowFrameWidthCmd
				'ADD  END
			Case "show"
				CmdName = Event_Renamed.CmdType.ShowCmd
			Case "showimage"
				CmdName = Event_Renamed.CmdType.ShowImageCmd
			Case "showunitstatus"
				CmdName = Event_Renamed.CmdType.ShowUnitStatusCmd
			Case "skip"
				CmdName = Event_Renamed.CmdType.SkipCmd
			Case "sort"
				CmdName = Event_Renamed.CmdType.SortCmd
			Case "specialpower", "mind"
				CmdName = Event_Renamed.CmdType.SpecialPowerCmd
			Case "split"
				CmdName = Event_Renamed.CmdType.SplitCmd
			Case "startbgm"
				CmdName = Event_Renamed.CmdType.StartBGMCmd
			Case "stopbgm"
				CmdName = Event_Renamed.CmdType.StopBGMCmd
			Case "stopsummoning"
				CmdName = Event_Renamed.CmdType.StopSummoningCmd
			Case "supply"
				CmdName = Event_Renamed.CmdType.SupplyCmd
			Case "sunset"
				CmdName = Event_Renamed.CmdType.SunsetCmd
			Case "swap"
				CmdName = Event_Renamed.CmdType.SwapCmd
			Case "switch"
				CmdName = Event_Renamed.CmdType.SwitchCmd
			Case "playflash"
				CmdName = Event_Renamed.CmdType.PlayFlashCmd
			Case "clearflash"
				CmdName = Event_Renamed.CmdType.ClearFlashCmd
			Case "case"
				CmdName = Event_Renamed.CmdType.CaseCmd
				If ArgNum = 2 Then
					If LCase(list(2)) = "else" Then
						CmdName = Event_Renamed.CmdType.CaseElseCmd
					End If
				End If
			Case "endsw"
				CmdName = Event_Renamed.CmdType.EndSwCmd
			Case "talk"
				CmdName = Event_Renamed.CmdType.TalkCmd
			Case "end"
				CmdName = Event_Renamed.CmdType.EndCmd
			Case "suspend"
				CmdName = Event_Renamed.CmdType.SuspendCmd
			Case "telop"
				CmdName = Event_Renamed.CmdType.TelopCmd
			Case "transform"
				CmdName = Event_Renamed.CmdType.TransformCmd
			Case "unit"
				CmdName = Event_Renamed.CmdType.UnitCmd
			Case "unset"
				CmdName = Event_Renamed.CmdType.UnsetCmd
			Case "upgrade"
				CmdName = Event_Renamed.CmdType.UpgradeCmd
			Case "upvar"
				CmdName = Event_Renamed.CmdType.UpVarCmd
			Case "useability"
				CmdName = Event_Renamed.CmdType.UseAbilityCmd
			Case "wait"
				CmdName = Event_Renamed.CmdType.WaitCmd
			Case "water"
				CmdName = Event_Renamed.CmdType.WaterCmd
			Case "whitein"
				CmdName = Event_Renamed.CmdType.WhiteInCmd
			Case "whiteout"
				CmdName = Event_Renamed.CmdType.WhiteOutCmd
			Case "write"
				CmdName = Event_Renamed.CmdType.WriteCmd
			Case Else
				'定義済みのイベントコマンドではない
				
				If ArgNum >= 3 Then
					If list(2) = "=" Then
						'代入式
						
						CmdName = Event_Renamed.CmdType.SetCmd
						'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim Preserve strArgs(3)
						'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim Preserve lngArgs(3)
						'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim Preserve dblArgs(3)
						'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim Preserve ArgsType(3)
						
						'代入先の変数名
						strArgs(2) = list(1)
						ArgsType(2) = Expression.ValueType.StringType
						
						'代入する値
						'(値が項の場合は既に引数の処理が済んでいるのでなにもしなくてよい)
						If ArgNum > 3 Then
							ArgsType(3) = Expression.ValueType.UndefinedType
							'GetValueAsStringの呼び出しの際に、Argsの内容は必ず項と仮定
							'されているので、わざと項にしておく
							strArgs(3) = "(" & ListTail(edata, 3) & ")"
						End If
						ArgNum = 3
						Exit Function
					End If
				End If
				
				If ArgNum = -1 Then
					CmdName = Event_Renamed.CmdType.NopCmd
					Exit Function
				End If
				
				'サブルーチンコール？
				CmdName = Event_Renamed.CmdType.CallCmd
				'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
				ReDim Preserve strArgs(ArgNum + 1)
				'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
				ReDim Preserve lngArgs(ArgNum + 1)
				'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
				ReDim Preserve dblArgs(ArgNum + 1)
				'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
				ReDim Preserve ArgsType(ArgNum + 1)
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
					ArgsType(2) = Expression.ValueType.StringType
				Else
					ArgsType(2) = Expression.ValueType.UndefinedType
				End If
				Exit Function
		End Select
		
		If CmdName = Event_Renamed.CmdType.IfCmd Or CmdName = Event_Renamed.CmdType.ElseIfCmd Then
			'If文の処理の高速化のため、あらかじめ構文解析しておく
			If ArgNum = 1 Then
				'書式エラー
				DisplayEventErrorMessage(CurrentLineNum, "Ifコマンドの書式に合っていません")
				Parse = False
				Exit Function
			End If
			
			expr = list(2)
			For i = 3 To ArgNum
				buf = list(i)
				Select Case LCase(buf)
					Case "then", "exit"
						'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim strArgs(4)
						'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim lngArgs(4)
						'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim dblArgs(4)
						'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim ArgsType(4)
						strArgs(2) = expr
						lngArgs(3) = ArgNum - 2
						ArgsType(3) = Expression.ValueType.NumericType
						strArgs(4) = LCase(buf)
						Exit For
					Case "goto"
						buf = GetArg(i + 1)
						'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim strArgs(5)
						'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim lngArgs(5)
						'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim dblArgs(5)
						'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim ArgsType(5)
						strArgs(2) = expr
						lngArgs(3) = ArgNum - 3
						ArgsType(3) = Expression.ValueType.NumericType
						strArgs(4) = "goto"
						strArgs(5) = buf
						Exit For
					Case ""
						buf = """"""
				End Select
				expr = expr & " " & buf
			Next 
			
			If i > ArgNum Then
				If CmdName = Event_Renamed.CmdType.IfCmd Then
					DisplayEventErrorMessage(LineNum, "Ifに対応する Then または Exit または Goto がありません")
				Else
					DisplayEventErrorMessage(LineNum, "ElseIfに対応する Then または Exit または Goto がありません")
				End If
				TerminateSRC()
			End If
			
			'条件式が式であることが確定していれば条件式の項数を0に
			Select Case lngArgs(3)
				Case 0
					If CmdName = Event_Renamed.CmdType.IfCmd Then
						DisplayEventErrorMessage(LineNum, "Ifコマンドの条件式がありません")
					Else
						DisplayEventErrorMessage(LineNum, "ElseIfコマンドの条件式がありません")
					End If
					TerminateSRC()
				Case 1
					Select Case Asc(expr)
						Case 36 '$
							lngArgs(3) = 0
						Case 40 '(
							'()を除去
							strArgs(2) = Mid(expr, 2, Len(expr) - 2)
							lngArgs(3) = 0
					End Select
				Case 2
					If LCase(LIndex(expr, 1)) = "not" Then
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
		
		If CmdName = Event_Renamed.CmdType.PaintStringCmd Then
			'PaintString文の処理の高速化のため、あらかじめ構文解析しておく
			
			'「;」を含む場合は改めて項に分解
			'(正しくリストの処理が行えないため)
			If Right(buf, 1) = ";" Then
				buf = edata
				CmdName = Event_Renamed.CmdType.PaintStringRCmd
				buf = Left(buf, Len(buf) - 1)
				If Right(buf, 1) = " " Then
					'メッセージが空文字列
					buf = buf & """"""
				End If
				ArgNum = ListSplit(buf, list)
			End If
			
			Select Case ArgNum
				Case 2
					'引数が１個の場合
					ArgNum = 2
					'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim strArgs(2)
					'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim lngArgs(2)
					'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim dblArgs(2)
					'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim ArgsType(2)
					
					buf = list(2)
					
					'表示文字列が式の場合にも対応
					If Left(buf, 1) = """" And Right(buf, 1) = """" Then
						If InStr(buf, "$(") > 0 Then
							strArgs(2) = buf
						Else
							strArgs(2) = Mid(buf, 2, Len(buf) - 2)
							ArgsType(2) = Expression.ValueType.StringType
						End If
					ElseIf Left(buf, 1) = "`" And Right(buf, 1) = "`" Then 
						strArgs(2) = Mid(buf, 2, Len(buf) - 2)
						ArgsType(2) = Expression.ValueType.StringType
					ElseIf InStr(buf, "$(") > 0 Then 
						strArgs(2) = """" & buf & """"
					Else
						strArgs(2) = buf
					End If
				Case 3
					'引数が２個の場合
					ArgNum = 2
					'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim strArgs(2)
					'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim lngArgs(2)
					'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim dblArgs(2)
					'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim ArgsType(2)
					
					'表示文字列は必ず文字列
					buf = ListTail(edata, 2)
					If InStr(buf, "$(") > 0 Then
						strArgs(2) = """" & buf & """"
					Else
						strArgs(2) = buf
						ArgsType(2) = Expression.ValueType.StringType
					End If
				Case 4
					'引数が３個の場合
					
					'座標指定があるかどうかが確定しているか？
					If (list(2) = "-" Or IsNumeric(list(2)) Or IsExpr(list(2))) And (list(3) = "-" Or IsNumeric(list(3)) Or IsExpr(list(3))) Then
						'座標指定があることが確定
						ArgNum = 4
						'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim strArgs(4)
						'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim lngArgs(4)
						'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim dblArgs(4)
						'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim ArgsType(4)
						
						strArgs(2) = list(2)
						strArgs(3) = list(3)
						If Not IsExpr(list(2)) Then
							ArgsType(2) = Expression.ValueType.StringType
						End If
						If Not IsExpr(list(3)) Then
							ArgsType(3) = Expression.ValueType.StringType
						End If
					Else
						'実行時まで座標指定があるかどうか不明
						ArgNum = 5
						'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim strArgs(5)
						'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim lngArgs(5)
						'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim dblArgs(5)
						'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim ArgsType(5)
						
						strArgs(2) = list(2)
						strArgs(3) = list(3)
						
						'座標指定がなかった場合の表示文字列
						buf = ListTail(edata, 2)
						If InStr(buf, "$(") > 0 Then
							strArgs(5) = """" & buf & """"
						Else
							strArgs(5) = buf
							ArgsType(5) = Expression.ValueType.StringType
						End If
					End If
					
					'座標指定があった場合の表示文字列
					buf = list(4)
					If Left(buf, 1) = """" And Right(buf, 1) = """" Then
						If InStr(buf, "$(") > 0 Then
							strArgs(4) = buf
						Else
							strArgs(4) = Mid(buf, 2, Len(buf) - 2)
							ArgsType(4) = Expression.ValueType.StringType
						End If
					ElseIf Left(buf, 1) = "`" And Right(buf, 1) = "`" Then 
						strArgs(4) = Mid(buf, 2, Len(buf) - 2)
						ArgsType(4) = Expression.ValueType.StringType
					ElseIf InStr(buf, "$(") > 0 Then 
						strArgs(4) = """" & buf & """"
					Else
						strArgs(4) = buf
					End If
				Case Else
					'引数が４個以上の場合
					
					'座標指定があるかどうかが確定しているか？
					If (list(2) = "-" Or IsNumeric(list(2)) Or IsExpr(list(2))) And (list(3) = "-" Or IsNumeric(list(3)) Or IsExpr(list(3))) Then
						'座標指定があることが確定
						ArgNum = 4
						'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim strArgs(4)
						'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim lngArgs(4)
						'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim dblArgs(4)
						'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim ArgsType(4)
						
						strArgs(2) = list(2)
						strArgs(3) = list(3)
						If Not IsExpr(list(2)) Then
							ArgsType(2) = Expression.ValueType.StringType
						End If
						If Not IsExpr(list(3)) Then
							ArgsType(3) = Expression.ValueType.StringType
						End If
					Else
						'実行時まで座標指定があるかどうか不明
						ArgNum = 5
						'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim strArgs(5)
						'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim lngArgs(5)
						'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim dblArgs(5)
						'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
						ReDim ArgsType(5)
						
						strArgs(2) = list(2)
						strArgs(3) = list(3)
						
						'座標指定がなかった場合の表示文字列
						buf = ListTail(edata, 2)
						If InStr(buf, "$(") > 0 Then
							strArgs(5) = """" & buf & """"
						Else
							strArgs(5) = buf
							ArgsType(5) = Expression.ValueType.StringType
						End If
					End If
					
					'座標指定があった場合の表示文字列
					buf = ListTail(edata, 4)
					If InStr(buf, "$(") > 0 Then
						strArgs(4) = """" & buf & """"
					Else
						strArgs(4) = buf
						ArgsType(4) = Expression.ValueType.StringType
					End If
			End Select
			Exit Function
		End If
		
		If CmdName = Event_Renamed.CmdType.CallCmd Then
			'Callコマンドのサブルーチン指定が式かどうか調べておく
			If FindNormalLabel(strArgs(2)) > 0 Then
				ArgsType(2) = Expression.ValueType.StringType
			Else
				ArgsType(2) = Expression.ValueType.UndefinedType
			End If
		End If
		
		If CmdName = Event_Renamed.CmdType.LocalCmd Then
			If ArgNum > 4 Then
				If list(3) = "=" Then
					'Localコマンドが複数項から成る代入式を伴う場合
					
					'UPGRADE_WARNING: 配列 strArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim Preserve strArgs(4)
					'UPGRADE_WARNING: 配列 lngArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim Preserve lngArgs(4)
					'UPGRADE_WARNING: 配列 dblArgs の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim Preserve dblArgs(4)
					'UPGRADE_WARNING: 配列 ArgsType の下限が 2 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
					ReDim Preserve ArgsType(4)
					
					'代入する値
					ArgsType(4) = Expression.ValueType.UndefinedType
					strArgs(4) = "(" & ListTail(edata, 4) & ")"
					ArgNum = 4
					Exit Function
				End If
			End If
		End If
		
		Exit Function
		
ErrorHandler: 
		DisplayEventErrorMessage(LineNum, "イベントコマンドの内容が不正です")
		Parse = False
	End Function
	
	Private Sub DebugMsg()
		Dim idx As Short
		Dim fname As String
		fname = EventFileNames(EventFileID(LineNum))
		idx = InStr2(fname, "\")
		If idx > 0 Then
			fname = Mid(fname, idx + 1)
		End If
		'    Debug.Print Format$(LineNum) & " : " & EventData(LineNum);
		System.Diagnostics.Debug.Write("[" & fname & "：" & EventLineNum(LineNum) & "] ")
		Debug.Print(EventData(LineNum))
	End Sub
	
	'コマンドを実行し、実行後の行番号を返す
	Public Function Exec() As Integer
		On Error GoTo ErrorHandler
		
		'    DebugMsg
		
		Select Case Name
			Case Event_Renamed.CmdType.NopCmd
				'スキップ
				Exec = LineNum + 1
			Case Event_Renamed.CmdType.ArcCmd
				Exec = ExecArcCmd()
			Case Event_Renamed.CmdType.ArrayCmd
				Exec = ExecArrayCmd()
			Case Event_Renamed.CmdType.AskCmd
				Exec = ExecAskCmd()
			Case Event_Renamed.CmdType.AttackCmd
				Exec = ExecAttackCmd()
			Case Event_Renamed.CmdType.AutoTalkCmd
				Exec = ExecAutoTalkCmd()
			Case Event_Renamed.CmdType.BossRankCmd
				Exec = ExecBossRankCmd()
			Case Event_Renamed.CmdType.BreakCmd
				Exec = ExecBreakCmd()
			Case Event_Renamed.CmdType.CallCmd
				Exec = ExecCallCmd()
			Case Event_Renamed.CmdType.ReturnCmd
				Exec = ExecReturnCmd()
			Case Event_Renamed.CmdType.CallInterMissionCommandCmd
				Exec = ExecCallInterMissionCommandCmd()
			Case Event_Renamed.CmdType.CancelCmd
				Exec = ExecCancelCmd()
			Case Event_Renamed.CmdType.CenterCmd
				Exec = ExecCenterCmd()
			Case Event_Renamed.CmdType.ChangeAreaCmd
				Exec = ExecChangeAreaCmd()
				'ADD START 240a
			Case Event_Renamed.CmdType.ChangeLayerCmd
				Exec = ExecChangeLayerCmd()
				'ADD  END  240a
			Case Event_Renamed.CmdType.ChangeMapCmd
				Exec = ExecChangeMapCmd()
			Case Event_Renamed.CmdType.ChangeModeCmd
				Exec = ExecChangeModeCmd()
			Case Event_Renamed.CmdType.ChangePartyCmd
				Exec = ExecChangePartyCmd()
			Case Event_Renamed.CmdType.ChangeTerrainCmd
				Exec = ExecChangeTerrainCmd()
			Case Event_Renamed.CmdType.ChangeUnitBitmapCmd
				Exec = ExecChangeUnitBitmapCmd()
			Case Event_Renamed.CmdType.ChargeCmd
				Exec = ExecChargeCmd()
			Case Event_Renamed.CmdType.CircleCmd
				Exec = ExecCircleCmd()
			Case Event_Renamed.CmdType.ClearEventCmd
				Exec = ExecClearEventCmd()
			Case Event_Renamed.CmdType.ClearImageCmd
				Exec = ExecClearImageCmd()
				'ADD START 240a
			Case Event_Renamed.CmdType.ClearLayerCmd
				Exec = ExecClearLayerCmd()
				'ADD  END  240a
			Case Event_Renamed.CmdType.ClearObjCmd
				Exec = ExecClearObjCmd()
			Case Event_Renamed.CmdType.ClearPictureCmd
				Exec = ExecClearPictureCmd()
			Case Event_Renamed.CmdType.ClearSkillCmd
				Exec = ExecClearSkillCmd()
			Case Event_Renamed.CmdType.ClearSpecialPowerCmd
				Exec = ExecClearSpecialPowerCmd()
			Case Event_Renamed.CmdType.ClearStatusCmd
				Exec = ExecClearStatusCmd()
			Case Event_Renamed.CmdType.CloseCmd
				Exec = ExecCloseCmd()
			Case Event_Renamed.CmdType.ClsCmd
				Exec = ExecClsCmd()
			Case Event_Renamed.CmdType.ColorCmd
				Exec = ExecColorCmd()
			Case Event_Renamed.CmdType.ColorFilterCmd
				Exec = ExecColorFilterCmd()
			Case Event_Renamed.CmdType.CombineCmd
				Exec = ExecCombineCmd()
			Case Event_Renamed.CmdType.ConfirmCmd
				Exec = ExecConfirmCmd()
			Case Event_Renamed.CmdType.ContinueCmd
				Exec = ExecContinueCmd()
			Case Event_Renamed.CmdType.CopyArrayCmd
				Exec = ExecCopyArrayCmd()
			Case Event_Renamed.CmdType.CopyFileCmd
				Exec = ExecCopyFileCmd()
			Case Event_Renamed.CmdType.CreateCmd
				Exec = ExecCreateCmd()
			Case Event_Renamed.CmdType.CreateFolderCmd
				Exec = ExecCreateFolderCmd()
			Case Event_Renamed.CmdType.DebugCmd
				Exec = ExecDebugCmd()
			Case Event_Renamed.CmdType.DestroyCmd
				Exec = ExecDestroyCmd()
			Case Event_Renamed.CmdType.DisableCmd
				Exec = ExecDisableCmd()
			Case Event_Renamed.CmdType.DoCmd
				Exec = ExecDoCmd()
			Case Event_Renamed.CmdType.LoopCmd
				Exec = ExecLoopCmd()
			Case Event_Renamed.CmdType.DrawOptionCmd
				Exec = ExecDrawOptionCmd()
			Case Event_Renamed.CmdType.DrawWidthCmd
				Exec = ExecDrawWidthCmd()
			Case Event_Renamed.CmdType.EnableCmd
				Exec = ExecEnableCmd()
			Case Event_Renamed.CmdType.EquipCmd
				Exec = ExecEquipCmd()
			Case Event_Renamed.CmdType.EscapeCmd
				Exec = ExecEscapeCmd()
			Case Event_Renamed.CmdType.ExchangeItemCmd
				Exec = ExecExchangeItemCmd()
			Case Event_Renamed.CmdType.ExecCmd
				Exec = ExecExecCmd()
			Case Event_Renamed.CmdType.ExitCmd
				Exec = ExecExitCmd()
			Case Event_Renamed.CmdType.ExplodeCmd
				Exec = ExecExplodeCmd()
			Case Event_Renamed.CmdType.ExpUpCmd
				Exec = ExecExpUpCmd()
			Case Event_Renamed.CmdType.FadeInCmd
				Exec = ExecFadeInCmd()
			Case Event_Renamed.CmdType.FadeOutCmd
				Exec = ExecFadeOutCmd()
			Case Event_Renamed.CmdType.FillColorCmd
				Exec = ExecFillColorCmd()
			Case Event_Renamed.CmdType.FillStyleCmd
				Exec = ExecFillStyleCmd()
			Case Event_Renamed.CmdType.FinishCmd
				Exec = ExecFinishCmd()
			Case Event_Renamed.CmdType.FixCmd
				Exec = ExecFixCmd()
			Case Event_Renamed.CmdType.FontCmd
				Exec = ExecFontCmd()
			Case Event_Renamed.CmdType.ForCmd
				Exec = ExecForCmd()
			Case Event_Renamed.CmdType.ForEachCmd
				Exec = ExecForEachCmd()
			Case Event_Renamed.CmdType.NextCmd
				Exec = ExecNextCmd()
			Case Event_Renamed.CmdType.ForgetCmd
				Exec = ExecForgetCmd()
			Case Event_Renamed.CmdType.GameClearCmd
				Exec = ExecGameClearCmd()
			Case Event_Renamed.CmdType.GameOverCmd
				Exec = ExecGameOverCmd()
			Case Event_Renamed.CmdType.FreeMemoryCmd
				Exec = ExecFreeMemoryCmd()
			Case Event_Renamed.CmdType.GetOffCmd
				Exec = ExecGetOffCmd()
			Case Event_Renamed.CmdType.GlobalCmd
				Exec = ExecGlobalCmd()
			Case Event_Renamed.CmdType.GotoCmd
				Exec = ExecGotoCmd()
			Case Event_Renamed.CmdType.HideCmd
				Exec = ExecHideCmd()
			Case Event_Renamed.CmdType.HotPointCmd
				Exec = ExecHotPointCmd()
			Case Event_Renamed.CmdType.IfCmd
				Exec = ExecIfCmd()
			Case Event_Renamed.CmdType.ElseCmd, Event_Renamed.CmdType.ElseIfCmd
				Exec = ExecElseCmd()
			Case Event_Renamed.CmdType.EndIfCmd
				'スキップ
				Exec = LineNum + 1
			Case Event_Renamed.CmdType.IncrCmd
				Exec = ExecIncrCmd()
			Case Event_Renamed.CmdType.IncreaseMoraleCmd
				Exec = ExecIncreaseMoraleCmd()
			Case Event_Renamed.CmdType.InputCmd
				Exec = ExecInputCmd()
				' MOD START マージ
				'        Case InterMissionCommandCmd
				'            Exec = ExecInterMissionCommandCmd()
			Case Event_Renamed.CmdType.IntermissionCommandCmd
				Exec = ExecIntermissionCommandCmd()
				' MOD ENDマージ
			Case Event_Renamed.CmdType.ItemCmd
				Exec = ExecItemCmd()
			Case Event_Renamed.CmdType.JoinCmd
				Exec = ExecJoinCmd()
			Case Event_Renamed.CmdType.KeepBGMCmd
				Exec = ExecKeepBGMCmd()
			Case Event_Renamed.CmdType.LandCmd
				Exec = ExecLandCmd()
			Case Event_Renamed.CmdType.LaunchCmd
				Exec = ExecLaunchCmd()
			Case Event_Renamed.CmdType.LeaveCmd
				Exec = ExecLeaveCmd()
			Case Event_Renamed.CmdType.LevelUpCmd
				Exec = ExecLevelUpCmd()
			Case Event_Renamed.CmdType.LineCmd
				Exec = ExecLineCmd()
			Case Event_Renamed.CmdType.LineReadCmd
				Exec = ExecLineReadCmd()
			Case Event_Renamed.CmdType.LoadCmd
				Exec = ExecLoadCmd()
			Case Event_Renamed.CmdType.LocalCmd
				Exec = ExecLocalCmd()
			Case Event_Renamed.CmdType.MakePilotListCmd
				Exec = ExecMakePilotListCmd()
			Case Event_Renamed.CmdType.MakeUnitListCmd
				Exec = ExecMakeUnitListCmd()
			Case Event_Renamed.CmdType.MapAbilityCmd
				Exec = ExecMapAbilityCmd()
			Case Event_Renamed.CmdType.MapAttackCmd
				Exec = ExecMapAttackCmd()
			Case Event_Renamed.CmdType.SpecialPowerCmd
				Exec = ExecSpecialPowerCmd()
			Case Event_Renamed.CmdType.MoneyCmd
				Exec = ExecMoneyCmd()
			Case Event_Renamed.CmdType.MonotoneCmd
				Exec = ExecMonotoneCmd()
			Case Event_Renamed.CmdType.MoveCmd
				Exec = ExecMoveCmd()
			Case Event_Renamed.CmdType.NightCmd
				Exec = ExecNightCmd()
			Case Event_Renamed.CmdType.NoonCmd
				Exec = ExecNoonCmd()
			Case Event_Renamed.CmdType.OpenCmd
				Exec = ExecOpenCmd()
			Case Event_Renamed.CmdType.OptionCmd
				Exec = ExecOptionCmd()
			Case Event_Renamed.CmdType.OrganizeCmd
				Exec = ExecOrganizeCmd()
			Case Event_Renamed.CmdType.OvalCmd
				Exec = ExecOvalCmd()
			Case Event_Renamed.CmdType.PaintPictureCmd
				Exec = ExecPaintPictureCmd()
			Case Event_Renamed.CmdType.PaintStringCmd, Event_Renamed.CmdType.PaintStringRCmd
				Exec = ExecPaintStringCmd()
			Case Event_Renamed.CmdType.PaintSysStringCmd
				Exec = ExecPaintSysStringCmd()
			Case Event_Renamed.CmdType.PilotCmd
				Exec = ExecPilotCmd()
			Case Event_Renamed.CmdType.PlayMIDICmd
				Exec = ExecPlayMIDICmd()
			Case Event_Renamed.CmdType.PlaySoundCmd
				Exec = ExecPlaySoundCmd()
			Case Event_Renamed.CmdType.PolygonCmd
				Exec = ExecPolygonCmd()
			Case Event_Renamed.CmdType.PrintCmd
				Exec = ExecPrintCmd()
			Case Event_Renamed.CmdType.PSetCmd
				Exec = ExecPSetCmd()
			Case Event_Renamed.CmdType.QuestionCmd
				Exec = ExecQuestionCmd()
			Case Event_Renamed.CmdType.QuickLoadCmd
				Exec = ExecQuickLoadCmd()
			Case Event_Renamed.CmdType.QuitCmd
				Exec = ExecQuitCmd()
			Case Event_Renamed.CmdType.RankUpCmd
				Exec = ExecRankUpCmd()
			Case Event_Renamed.CmdType.ReadCmd
				Exec = ExecReadCmd()
			Case Event_Renamed.CmdType.RecoverENCmd
				Exec = ExecRecoverENCmd()
			Case Event_Renamed.CmdType.RecoverHPCmd
				Exec = ExecRecoverHPCmd()
			Case Event_Renamed.CmdType.RecoverPlanaCmd
				Exec = ExecRecoverPlanaCmd()
			Case Event_Renamed.CmdType.RecoverSPCmd
				Exec = ExecRecoverSPCmd()
			Case Event_Renamed.CmdType.RedrawCmd
				Exec = ExecRedrawCmd()
			Case Event_Renamed.CmdType.RefreshCmd
				Exec = ExecRefreshCmd()
			Case Event_Renamed.CmdType.ReleaseCmd
				Exec = ExecReleaseCmd()
			Case Event_Renamed.CmdType.RemoveFileCmd
				Exec = ExecRemoveFileCmd()
			Case Event_Renamed.CmdType.RemoveFolderCmd
				Exec = ExecRemoveFolderCmd()
			Case Event_Renamed.CmdType.RemoveItemCmd
				Exec = ExecRemoveItemCmd()
			Case Event_Renamed.CmdType.RemovePilotCmd
				Exec = ExecRemovePilotCmd()
			Case Event_Renamed.CmdType.RemoveUnitCmd
				Exec = ExecRemoveUnitCmd()
			Case Event_Renamed.CmdType.RenameBGMCmd
				Exec = ExecRenameBGMCmd()
			Case Event_Renamed.CmdType.RenameFileCmd
				Exec = ExecRenameFileCmd()
			Case Event_Renamed.CmdType.RenameTermCmd
				Exec = ExecRenameTermCmd()
			Case Event_Renamed.CmdType.ReplacePilotCmd
				Exec = ExecReplacePilotCmd()
			Case Event_Renamed.CmdType.RequireCmd
				Exec = ExecRequireCmd()
			Case Event_Renamed.CmdType.RestoreEventCmd
				Exec = ExecRestoreEventCmd()
			Case Event_Renamed.CmdType.RideCmd
				Exec = ExecRideCmd()
			Case Event_Renamed.CmdType.SaveDataCmd
				Exec = ExecSaveDataCmd()
			Case Event_Renamed.CmdType.SelectCmd
				Exec = ExecSelectCmd()
			Case Event_Renamed.CmdType.SelectTargetCmd
				Exec = ExecSelectTargetCmd()
			Case Event_Renamed.CmdType.SepiaCmd
				Exec = ExecSepiaCmd()
			Case Event_Renamed.CmdType.SetCmd
				Exec = ExecSetCmd()
			Case Event_Renamed.CmdType.SetSkillCmd
				Exec = ExecSetSkillCmd()
			Case Event_Renamed.CmdType.SetBulletCmd
				Exec = ExecSetBulletCmd()
			Case Event_Renamed.CmdType.SetMessageCmd
				Exec = ExecSetMessageCmd()
			Case Event_Renamed.CmdType.SetRelationCmd
				Exec = ExecSetRelationCmd()
			Case Event_Renamed.CmdType.SetStatusCmd
				Exec = ExecSetStatusCmd()
				'ADD START 240a
			Case Event_Renamed.CmdType.SetStatusStringColorCmd
				Exec = ExecSetStatusStringColor()
				'ADD  END
			Case Event_Renamed.CmdType.SetStockCmd
				Exec = ExecSetStockCmd()
				'ADD START 240a
			Case Event_Renamed.CmdType.SetWindowColorCmd
				Exec = ExecSetWindowColor()
			Case Event_Renamed.CmdType.SetWindowFrameWidthCmd
				Exec = ExecSetWindowFrameWidth()
				'ADD  END
			Case Event_Renamed.CmdType.ShowCmd
				Exec = ExecShowCmd()
			Case Event_Renamed.CmdType.ShowImageCmd
				Exec = ExecShowImageCmd()
			Case Event_Renamed.CmdType.ShowUnitStatusCmd
				Exec = ExecShowUnitStatusCmd()
			Case Event_Renamed.CmdType.SkipCmd
				Exec = ExecSkipCmd()
			Case Event_Renamed.CmdType.SortCmd
				'UPGRADE_WARNING: オブジェクト ExecSortCmd() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				Exec = ExecSortCmd()
			Case Event_Renamed.CmdType.SplitCmd
				Exec = ExecSplitCmd()
			Case Event_Renamed.CmdType.StartBGMCmd
				Exec = ExecStartBGMCmd()
			Case Event_Renamed.CmdType.StopBGMCmd
				Exec = ExecStopBGMCmd()
			Case Event_Renamed.CmdType.StopSummoningCmd
				Exec = ExecStopSummoningCmd()
			Case Event_Renamed.CmdType.SunsetCmd
				Exec = ExecSunsetCmd()
			Case Event_Renamed.CmdType.SupplyCmd
				Exec = ExecSupplyCmd()
			Case Event_Renamed.CmdType.SwapCmd
				Exec = ExecSwapCmd()
			Case Event_Renamed.CmdType.SwitchCmd
				Exec = ExecSwitchCmd()
			Case Event_Renamed.CmdType.CaseCmd, Event_Renamed.CmdType.CaseElseCmd
				Exec = ExecCaseCmd()
			Case Event_Renamed.CmdType.EndSwCmd
				'スキップ
				Exec = LineNum + 1
			Case Event_Renamed.CmdType.TalkCmd
				Exec = ExecTalkCmd()
			Case Event_Renamed.CmdType.TelopCmd
				Exec = ExecTelopCmd()
			Case Event_Renamed.CmdType.TransformCmd
				Exec = ExecTransformCmd()
			Case Event_Renamed.CmdType.UnitCmd
				Exec = ExecUnitCmd()
			Case Event_Renamed.CmdType.UnsetCmd
				Exec = ExecUnsetCmd()
			Case Event_Renamed.CmdType.UpgradeCmd
				Exec = ExecUpgradeCmd()
			Case Event_Renamed.CmdType.UpVarCmd
				Exec = ExecUpvarCmd()
			Case Event_Renamed.CmdType.UseAbilityCmd
				Exec = ExecUseAbilityCmd()
			Case Event_Renamed.CmdType.WaitCmd
				Exec = ExecWaitCmd()
			Case Event_Renamed.CmdType.WaterCmd
				Exec = ExecWaterCmd()
			Case Event_Renamed.CmdType.WhiteInCmd
				Exec = ExecWhiteInCmd()
			Case Event_Renamed.CmdType.WhiteOutCmd
				Exec = ExecWhiteOutCmd()
			Case Event_Renamed.CmdType.WriteCmd
				Exec = ExecWriteCmd()
			Case Event_Renamed.CmdType.PlayFlashCmd
				Exec = ExecPlayFlashCmd()
			Case Event_Renamed.CmdType.ClearFlashCmd
				Exec = ExecClearFlashCmd()
			Case Else
				EventErrorMessage = ListIndex(EventData(LineNum), 1) & "というコマンドは存在しません"
				Error(0)
		End Select
		
		Exit Function
		
ErrorHandler: 
		If Len(EventErrorMessage) > 0 Then
			DisplayEventErrorMessage(LineNum, EventErrorMessage)
			EventErrorMessage = ""
		ElseIf LCase(ListIndex(EventData(LineNum), 1)) = "talk" Then 
			DisplayEventErrorMessage(LineNum, "Talkコマンド実行中に不正な処理が行われました。" & "MIDIがソフトウェアシンセサイザで演奏されているか、" & "フォントキャッシュが壊れている可能性があります。" & "詳しくはSRC公式ホームページの「よくある質問集」をご覧下さい。")
		ElseIf LCase(ListIndex(EventData(LineNum), 1)) = "autotalk" Then 
			DisplayEventErrorMessage(LineNum, "AutoTalkコマンド実行中に不正な処理が行われました。" & "MIDIがソフトウェアシンセサイザで演奏されているか、" & "フォントキャッシュが壊れている可能性があります。" & "詳しくはSRC公式ホームページの「よくある質問集」をご覧下さい。")
		Else
			DisplayEventErrorMessage(LineNum, "イベントデータが不正です")
		End If
		Exec = -1
	End Function
	
	
	'idx番目の引数を式として評価せずにそのまま返す
	Public Function GetArg(ByVal idx As Short) As String
		GetArg = strArgs(idx)
	End Function
	
	'idx番目の引数の値を文字列として返す
	Public Function GetArgAsString(ByVal idx As Short) As String
		Select Case ArgsType(idx)
			Case Expression.ValueType.UndefinedType
				GetArgAsString = GetValueAsString(strArgs(idx), True)
			Case Expression.ValueType.StringType
				GetArgAsString = strArgs(idx)
			Case Expression.ValueType.NumericType
				GetArgAsString = VB6.Format(dblArgs(idx))
		End Select
	End Function
	
	'idx番目の引数の値をLongとして返す
	Public Function GetArgAsLong(ByVal idx As Short) As Integer
		Select Case ArgsType(idx)
			Case Expression.ValueType.UndefinedType
				GetArgAsLong = GetValueAsLong(strArgs(idx), True)
			Case Expression.ValueType.StringType
				GetArgAsLong = 0
			Case Expression.ValueType.NumericType
				GetArgAsLong = lngArgs(idx)
		End Select
	End Function
	
	'idx番目の引数の値をDoubleとして返す
	Public Function GetArgAsDouble(ByVal idx As Short) As Double
		Select Case ArgsType(idx)
			Case Expression.ValueType.UndefinedType
				GetArgAsDouble = GetValueAsDouble(strArgs(idx), True)
			Case Expression.ValueType.StringType
				GetArgAsDouble = 0
			Case Expression.ValueType.NumericType
				GetArgAsDouble = dblArgs(idx)
		End Select
	End Function
	
	'idx番目の引数が示すユニットを返す
	Public Function GetArgAsUnit(ByVal idx As Short, Optional ByVal ignore_error As Boolean = False) As Unit
		Dim pname As String
		
		pname = GetArgAsString(idx)
		GetArgAsUnit = UList.Item2(pname)
		If GetArgAsUnit Is Nothing Then
			If Not PList.IsDefined(pname) Then
				EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
				Error(0)
			End If
			GetArgAsUnit = PList.Item(pname).Unit_Renamed
			
			If Not ignore_error Then
				If GetArgAsUnit Is Nothing Then
					EventErrorMessage = "「" & pname & "」はユニットに乗っていません"
					Error(0)
				End If
			End If
		End If
	End Function
	
	'idx番目の引数が示すパイロットを返す
	Public Function GetArgAsPilot(ByVal idx As Short) As Pilot
		Dim pname As String
		
		pname = GetArgAsString(idx)
		If Not PList.IsDefined(pname) Then
			EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
			Error(0)
		End If
		GetArgAsPilot = PList.Item(pname)
	End Function
	
	
	'ArgsTypeを参照する
	Friend Function GetArgsType(ByVal idx As Short) As Expression.ValueType
		GetArgsType = ArgsType(idx)
	End Function
	
	'ArgsTypeを設定する
	Friend Sub SetArgsType(ByVal idx As Short, ByVal new_type As Expression.ValueType)
		ArgsType(idx) = new_type
	End Sub
	
	
	Private Function ExecArcCmd() As Integer
		Dim pic, pic2 As System.Windows.Forms.PictureBox
		Dim y1, x1, rad As Short
		Dim start_angle, end_angle As Double
		Dim opt As String
		Dim cname As String
		Dim clr As Integer
		Dim i As Short
		
		If ArgNum < 6 Then
			EventErrorMessage = "Arcコマンドの引数の数が違います"
			Error(0)
		End If
		
		x1 = GetArgAsLong(2) + BaseX
		y1 = GetArgAsLong(3) + BaseY
		rad = GetArgAsLong(4)
		start_angle = 3.1415926535 * GetArgAsDouble(5) / 180
		end_angle = 3.1415926535 * GetArgAsDouble(6) / 180
		
		'塗りつぶしの際は角度を負の値にする必要がある
		'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
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
		
		SaveScreen()
		
		'描画先
		Select Case ObjDrawOption
			Case "背景"
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picBack
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMaskedBack
				IsMapDirty = True
			Case "保持"
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMain(1)
			Case Else
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
		End Select
		
		'描画領域
		Dim tmp As Short
		If ObjDrawOption <> "背景" Then
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
					Error(0)
				End If
				cname = New String(vbNullChar, 8)
				Mid(cname, 1, 2) = "&H"
				Mid(cname, 3, 2) = Mid(opt, 6, 2)
				Mid(cname, 5, 2) = Mid(opt, 4, 2)
				Mid(cname, 7, 2) = Mid(opt, 2, 2)
				If Not IsNumeric(cname) Then
					EventErrorMessage = "色指定が不正です"
					Error(0)
				End If
				clr = CInt(cname)
			Else
				EventErrorMessage = "Arcコマンドに不正なオプション「" & opt & "」が使われています"
				Error(0)
			End If
		Next 
		
		With pic
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.DrawWidth = ObjDrawWidth
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillColor = ObjFillColor
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillStyle = ObjFillStyle
		End With
		
		'UPGRADE_ISSUE: PictureBox メソッド pic.Circle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.Circle (x1, y1), rad, clr, start_angle, end_angle
		
		With pic
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.DrawWidth = 1
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
			'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillStyle = vbFSTransparent
		End With
		
		If Not pic2 Is Nothing Then
			With pic2
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.DrawWidth = ObjDrawWidth
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillColor = ObjFillColor
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillStyle = ObjFillStyle
			End With
			
			'UPGRADE_ISSUE: PictureBox メソッド pic2.Circle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic2.Circle (x1, y1), rad, clr, start_angle, end_angle
			
			With pic2
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.DrawWidth = 1
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
				'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillStyle = vbFSTransparent
			End With
		End If
		
		ExecArcCmd = LineNum + 1
	End Function
	
	Private Function ExecArrayCmd() As Integer
		Dim array_buf As Object
		Dim array_buf2() As String
		Dim buf As String
		Dim var_name, vname As String
		Dim i As Integer
		Dim num As Short
		Dim IsList As Boolean
		Dim etype As Expression.ValueType
		Dim str_value As String
		Dim num_value As Double
		Dim sep As String
		
		If ArgNum <> 4 Then
			EventErrorMessage = "Arrayコマンドの引数の数が違います"
			Error(0)
		Else
			If GetArgAsString(4) = "リスト" Then
				IsList = True
			Else
				IsList = False
			End If
		End If
		
		'代入先の変数名
		var_name = GetArg(2)
		If Left(var_name, 1) = "$" Then
			var_name = Mid(var_name, 2)
		End If
		'Eval関数
		If LCase(Left(var_name, 5)) = "eval(" Then
			If Right(var_name, 1) = ")" Then
				var_name = Mid(var_name, 6, Len(var_name) - 6)
				var_name = GetValueAsString(var_name)
			End If
		End If
		
		'代入先の変数を初期化した上で再設定
		'サブルーチンローカル変数の場合
		If IsSubLocalVariableDefined(var_name) Then
			UndefineVariable(var_name)
			VarIndex = VarIndex + 1
			With VarStack(VarIndex)
				.Name = var_name
				.VariableType = Expression.ValueType.NumericType
				.StringValue = ""
				.NumericValue = 0
			End With
			'ローカル変数の場合
		ElseIf IsLocalVariableDefined(var_name) Then 
			UndefineVariable(var_name)
			DefineLocalVariable(var_name)
			'グローバル変数の場合
		ElseIf IsGlobalVariableDefined(var_name) Then 
			UndefineVariable(var_name)
			DefineGlobalVariable(var_name)
		End If
		
		If IsList Then
			'リストを配列に変換
			num = ListSplit(GetArgAsString(3), array_buf2)
			'UPGRADE_WARNING: オブジェクト array_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			array_buf = VB6.CopyArray(array_buf2)
		Else
			'文字列を分割して配列に代入
			ReDim array_buf(0)
			buf = GetArgAsString(3)
			sep = GetArgAsString(4)
			i = InStr(buf, sep)
			Do While i > 0
				ReDim Preserve array_buf(UBound(array_buf) + 1)
				'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				array_buf(UBound(array_buf)) = Left(buf, i - 1)
				buf = Mid(buf, i + Len(sep))
				i = InStr(buf, sep)
			Loop 
			ReDim Preserve array_buf(UBound(array_buf) + 1)
			'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			array_buf(UBound(array_buf)) = buf
		End If
		
		For i = 1 To UBound(array_buf)
			'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			buf = CStr(array_buf(i))
			TrimString(buf)
			If IsNumeric(buf) Then
				etype = Expression.ValueType.NumericType
				str_value = ""
				num_value = StrToDbl(buf)
			Else
				etype = Expression.ValueType.StringType
				str_value = buf
				num_value = 0
			End If
			vname = var_name & "[" & CStr(i) & "]"
			SetVariable(vname, etype, str_value, num_value)
		Next 
		
		ExecArrayCmd = LineNum + 1
	End Function
	
	Private Function ExecAskCmd() As Integer
		Dim use_normal_list As Boolean
		Dim use_large_list As Boolean
		Dim use_continuous_mode As Boolean
		Dim enable_rbutton_cancel As Boolean
		Dim list() As String
		Dim msg As String
		Dim vname As String
		Dim i As Integer
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
					frmListBox.Hide()
					If AutoMoveCursor Then
						RestoreCursorPos()
					End If
					ReduceListBoxHeight()
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
					FormatMessage(buf)
					If Len(buf) > 0 Then
						If EventCmd(i).Name = Event_Renamed.CmdType.EndCmd Then
							Exit For
						End If
						ReDim Preserve list(UBound(list) + 1)
						ReDim Preserve ListItemID(UBound(list))
						ReDim Preserve ListItemFlag(UBound(list))
						list(UBound(list)) = buf
						ListItemID(UBound(list)) = VB6.Format(i - LineNum)
						ListItemFlag(UBound(list)) = False
					End If
				Next 
				
				If i > UBound(EventData) Then
					EventErrorMessage = "AskとEndが対応していません"
					Error(0)
				End If
				
				ExecAskCmd = i + 1
				
				'選択肢を配列で指定する場合
			Case 3
				vname = GetArg(2)
				msg = GetArgAsString(3)
				ListItemID(0) = ""
				
				'配列の検索
				If IsSubLocalVariableDefined(vname) Then
					If Left(vname, 1) = "$" Then
						vname = Mid(vname, 2) & "["
					Else
						vname = vname & "["
					End If
					For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
						With VarStack(i)
							If InStr(.Name, vname) = 1 Then
								If .VariableType = Expression.ValueType.StringType Then
									buf = .StringValue
								Else
									buf = VB6.Format(.NumericValue)
								End If
								If Len(buf) > 0 Then
									ReDim Preserve list(UBound(list) + 1)
									ReDim Preserve ListItemID(UBound(list))
									ReDim Preserve ListItemFlag(UBound(list))
									FormatMessage(buf)
									list(UBound(list)) = buf
									ListItemID(UBound(list)) = Mid(.Name, Len(vname) + 1, Len(.Name) - Len(vname) - 1)
									ListItemFlag(UBound(list)) = False
								End If
							End If
						End With
					Next 
				ElseIf IsLocalVariableDefined(vname) Then 
					If Left(vname, 1) = "$" Then
						vname = Mid(vname, 2) & "["
					Else
						vname = vname & "["
					End If
					For	Each var In LocalVariableList
						With var
							If InStr(.Name, vname) = 1 Then
								If .VariableType = Expression.ValueType.StringType Then
									buf = .StringValue
								Else
									buf = VB6.Format(.NumericValue)
								End If
								If Len(buf) > 0 Then
									ReDim Preserve list(UBound(list) + 1)
									ReDim Preserve ListItemID(UBound(list))
									ReDim Preserve ListItemFlag(UBound(list))
									FormatMessage(buf)
									list(UBound(list)) = buf
									ListItemID(UBound(list)) = Mid(.Name, Len(vname) + 1, Len(.Name) - Len(vname) - 1)
									ListItemFlag(UBound(list)) = False
								End If
							End If
						End With
					Next var
				ElseIf IsGlobalVariableDefined(vname) Then 
					If Left(vname, 1) = "$" Then
						vname = Mid(vname, 2) & "["
					Else
						vname = vname & "["
					End If
					For	Each var In GlobalVariableList
						With var
							If InStr(.Name, vname) = 1 Then
								If .VariableType = Expression.ValueType.StringType Then
									buf = .StringValue
								Else
									buf = VB6.Format(.NumericValue)
								End If
								If Len(buf) > 0 Then
									ReDim Preserve list(UBound(list) + 1)
									ReDim Preserve ListItemID(UBound(list))
									ReDim Preserve ListItemFlag(UBound(list))
									FormatMessage(buf)
									list(UBound(list)) = buf
									ListItemID(UBound(list)) = Mid(.Name, Len(vname) + 1, Len(.Name) - Len(vname) - 1)
									ListItemFlag(UBound(list)) = False
								End If
							End If
						End With
					Next var
				End If
				
				ExecAskCmd = LineNum + 1
			Case Else
				EventErrorMessage = "Askコマンドのオプションが不正です"
				Error(0)
		End Select
		
		'選択肢がなければそのまま終了
		If UBound(list) = 0 Then
			SelectedAlternative = CStr(0)
			Exit Function
		End If
		
		'ダイアログを拡大するか
		If Not use_normal_list And (UBound(list) > 6 Or use_large_list) Then
			EnlargeListBoxHeight()
		Else
			ReduceListBoxHeight()
		End If
		
		If AutoMoveCursor Then
			TopItem = 1
			SelectedItem = ListBox("選択", list, msg, "表示のみ")
			MoveCursorPos("ダイアログ")
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
				RestoreCursorPos()
			End If
		End If
		
		'ダイアログを標準の大きさに戻す
		If Not use_normal_list And Not use_continuous_mode And (UBound(list) > 6 Or use_large_list) Then
			ReduceListBoxHeight()
		End If
	End Function
	
	Private Function ExecAttackCmd() As Integer
		Dim u1, u2 As Unit
		Dim w1, w2 As Short
		Dim prev_su, prev_st As Unit
		Dim prev_w, prev_tw As Short
		Dim def_mode, cur_stage As String
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
					Error(0)
				End If
			Case Else
				EventErrorMessage = "Attackコマンドの引数の数が違います"
				Error(0)
		End Select
		
		u1 = GetArgAsUnit(2)
		u2 = GetArgAsUnit(4)
		
		If u1.Status_Renamed = "出撃" And u2.Status_Renamed = "出撃" Then
			If GetArgAsString(3) = "自動" Then
				w1 = SelectWeapon(u1, u2, "イベント")
			Else
				For w1 = 1 To u1.CountWeapon
					If GetArgAsString(3) = u1.Weapon(w1).Name And Not u1.IsWeaponClassifiedAs(w1, "マップ攻撃") Then
						Exit For
					End If
				Next 
				If w1 > u1.CountWeapon Then
					EventErrorMessage = "ユニット「" & u1.Name & "」には武装「" & GetArgAsString(3) & "」は存在しません"
					Error(0)
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
						If GetArgAsString(5) = u2.Weapon(w2).Name And Not u2.IsWeaponClassifiedAs(w2, "マップ攻撃") Then
							Exit For
						End If
					Next 
					If w2 > u2.CountWeapon Then
						EventErrorMessage = "ユニット「" & u2.Name & "」には武装「" & GetArgAsString(5) & "」は存在しません"
						Error(0)
					End If
			End Select
			
			If w1 > 0 Then
				prev_su = SelectedUnit
				prev_st = SelectedTarget
				prev_w = SelectedWeapon
				prev_tw = SelectedTWeapon
				SelectedUnit = u1
				SelectedTarget = u2
				SelectedWeapon = w1
				SelectedTWeapon = w2
				
				If u1.Party0 = "味方" Or u1.Party0 = "ＮＰＣ" Then
					OpenMessageForm(u2, u1)
				Else
					OpenMessageForm(u1, u2)
				End If
				
				'攻撃を実行
				cur_stage = Stage
				Stage = u1.Party
				u1.Attack(w1, u2, "", def_mode, is_event)
				u1 = u1.CurrentForm
				
				'反撃用武器がまだ使用可能かチェック
				' MOD START マージ
				'            If def_option = "自動" Then
				'                If Not u2.IsTargetWithinRange(w2, u1) Then
				'                    w2 = SelectWeapon(u2, u1, "反撃 イベント")
				'                    SelectedTWeapon = w2
				'                End If
				'            End If
				If def_option = "自動" And u2.Status_Renamed = "出撃" Then
					If Not u2.IsTargetWithinRange(w2, u1) Or Not u2.IsWeaponAvailable(w2, "移動前") Then
						w2 = SelectWeapon(u2, u1, "反撃 イベント")
						SelectedTWeapon = w2
					End If
				End If
				' MOD END マージ
				
				'反撃を実行
				' MOD START マージ
				'            If def_mode = "反撃" _
				''                And u2.Status = "出撃" _
				''                And Not u2.IsConditionSatisfied("行動不能") _
				''            Then
				If def_mode = "反撃" And u2.Status_Renamed = "出撃" And u2.MaxAction > 0 And Not u2.IsConditionSatisfied("攻撃不能") Then
					' MOD END マージ
					If w2 > 0 Then
						u2.Attack(w2, u1, "", "", is_event)
					Else
						u2.PilotMessage("射程外")
					End If
				End If
				Stage = cur_stage
				
				CloseMessageForm()
				
				u1.CurrentForm.UpdateCondition()
				u2.CurrentForm.UpdateCondition()
				
				u1.CurrentForm.CheckAutoHyperMode()
				u1.CurrentForm.CheckAutoNormalMode()
				u2.CurrentForm.CheckAutoHyperMode()
				u2.CurrentForm.CheckAutoNormalMode()
				
				SelectedUnit = prev_su
				SelectedTarget = prev_st
				SelectedWeapon = prev_w
				SelectedTWeapon = prev_tw
			End If
		End If
		
		RedrawScreen()
		
		ExecAttackCmd = LineNum + 1
	End Function
	
	Private Function ExecAutoTalkCmd() As Integer
		Dim pname, current_pname As String
		Dim u As Unit
		Dim ux, uy As Short
		Dim i As Integer
		Dim j As Short
		Dim lnum As Short
		Dim prev_msg_wait As Integer
		Dim without_cursor As Boolean
		Dim options, opt As String
		Dim buf As String
		
		'メッセージ表示速度を「普通」の値に設定
		prev_msg_wait = MessageWait
		MessageWait = 700
		
		Dim counter As Short
		counter = LineNum
		Dim cname As String
		Dim tcolor As Integer
		For i = counter To UBound(EventData)
			With EventCmd(i)
				Select Case .Name
					Case Event_Renamed.CmdType.AutoTalkCmd
						If .ArgNum > 1 Then
							pname = .GetArgAsString(2)
						Else
							pname = ""
						End If
						
						If Left(pname, 1) = "@" Then
							'メインパイロットの強制指定
							pname = Mid(pname, 2)
							If PList.IsDefined(pname) Then
								With PList.Item(pname)
									If Not .Unit_Renamed Is Nothing Then
										pname = .Unit_Renamed.MainPilot.Name
									End If
								End With
							End If
						End If
						
						'話者名チェック
						If Not PList.IsDefined(pname) And Not PDList.IsDefined(pname) And Not NPDList.IsDefined(pname) And Not pname = "システム" And Not pname = "" Then
							EventErrorMessage = "「" & pname & "」というパイロットが定義されていません"
							LineNum = i
							Error(0)
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
									Case "白黒", "セピア", "明", "暗", "上下反転", "左右反転", "上半分", "下半分", "右半分", "左半分", "右上", "左上", "右下", "左下", "ネガポジ反転", "シルエット", "夕焼け", "水中", "通常"
										If j > 2 Then
											'これらのパイロット画像描画に関するオプションは
											'パイロット名が指定されている場合にのみ有効
											options = options & opt & " "
										Else
											lnum = j
										End If
									Case "右回転"
										j = j + 1
										options = options & "右回転 " & .GetArgAsString(j) & " "
									Case "左回転"
										j = j + 1
										options = options & "左回転 " & .GetArgAsString(j) & " "
									Case "フィルタ"
										j = j + 1
										buf = .GetArgAsString(j)
										cname = New String(vbNullChar, 8)
										Mid(cname, 1, 2) = "&H"
										Mid(cname, 3, 2) = Mid(buf, 6, 2)
										Mid(cname, 5, 2) = Mid(buf, 4, 2)
										Mid(cname, 7, 2) = Mid(buf, 2, 2)
										tcolor = CInt(cname)
										j = j + 1
										options = options & "フィルタ " & VB6.Format(tcolor) & " " & .GetArgAsString(j) & " "
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
									OpenMessageForm()
								End If
								
								'メッセージウィンドウのパイロット画像を以前指定された
								'ものに確定させる
								If current_pname <> "" Then
									DisplayBattleMessage(current_pname, "", options)
								End If
								
								current_pname = ""
								
							Case 2
								'パイロット名のみ指定
								current_pname = pname
								
								'話者中心に画面位置を変更
								
								'プロローグイベントやエピローグイベント時はキャンセル
								If Stage = "プロローグ" Or Stage = "エピローグ" Then
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
								CenterUnit(pname, without_cursor)
								
							Case 3
								current_pname = pname
								Select Case .GetArgAsString(3)
									Case "母艦"
										'母艦の中央表示
										CenterUnit("母艦", without_cursor)
									Case "中央"
										'話者を中央表示
										CenterUnit(pname, without_cursor)
									Case "固定"
										'表示位置固定
								End Select
								
							Case 4
								'表示の座標指定あり
								current_pname = pname
								CenterUnit("", without_cursor, .GetArgAsLong(3), .GetArgAsLong(4))
								
							Case -1
								EventErrorMessage = "AutoTalkコマンドのパラメータの括弧の対応が取れていません"
								LineNum = i
								Error(0)
								
							Case Else
								EventErrorMessage = "AutoTalkコマンドの引数の数が違います"
								LineNum = i
								Error(0)
						End Select
						
						If Not frmMessage.Visible Then
							OpenMessageForm()
						End If
						
					Case Event_Renamed.CmdType.EndCmd
						CloseMessageForm()
						If .ArgNum <> 1 Then
							EventErrorMessage = "End部分の引数の数が違います"
							LineNum = i
							Error(0)
						End If
						Exit For
						
					Case Event_Renamed.CmdType.SuspendCmd
						If .ArgNum <> 1 Then
							EventErrorMessage = "Suspend部分の引数の数が違います"
							LineNum = i
							Error(0)
						End If
						Exit For
						
					Case Else
						If Not frmMessage.Visible Then
							OpenMessageForm()
						End If
						DisplayBattleMessage(current_pname, EventData(i), options)
						
				End Select
			End With
NextLoop: 
		Next 
		
		'メッセージ表示速度を元に戻す
		MessageWait = prev_msg_wait
		
		If i > UBound(EventData) Then
			CloseMessageForm()
			EventErrorMessage = "AutoTalkとEndが対応していません"
			Error(0)
		End If
		
		ExecAutoTalkCmd = i + 1
	End Function
	
	'話者の中央表示用サブルーチン
	Private Sub CenterUnit(ByVal pname As String, ByVal without_cursor As Boolean, Optional ByVal X As Short = 0, Optional ByVal Y As Short = 0)
		Dim u As Unit
		Dim xx, yy As Short
		
		'座標が指定されている場合
		If X <> 0 And Y <> 0 Then
			If X < 1 Or MapWidth < X Or Y < 1 Or MapHeight < Y Then
				'マップ外
				Exit Sub
			End If
			
			xx = X
			yy = Y
			GoTo FoundPoint
		End If
		
		If pname = "母艦" Then
			'母艦を中央表示
			For	Each u In UList
				With u
					If .Party0 = "味方" And .Status_Renamed = "出撃" Then
						If .IsFeatureAvailable("母艦") Then
							xx = .X
							yy = .Y
							GoTo FoundPoint
						End If
					End If
				End With
			Next u
			Exit Sub
		End If
		
		'表情パターン名での指定はパイロット名に変換しておく
		If Not PList.IsDefined(pname) And InStr(pname, "(") > 0 Then
			If PList.IsDefined(Left(pname, InStr(pname, "(") - 1)) Then
				pname = Left(pname, InStr(pname, "(") - 1)
			End If
		End If
		If Not PList.IsDefined(pname) And NPDList.IsDefined(pname) Then
			pname = NPDList.Item(pname).Nickname
		End If
		
		'話者はパイロット？
		If Not PList.IsDefined(pname) Then
			Exit Sub
		End If
		
		With PList.Item(pname)
			If Not .Unit_Renamed Is Nothing Then
				'パイロットが乗っているユニットを中央表示
				With .Unit_Renamed
					If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
						xx = .X
						yy = .Y
						GoTo FoundPoint
					End If
				End With
			End If
			
			'話者が味方でかつ出撃中でない場合は母艦を中央表示
			If .Party = "味方" Then
				CenterUnit("母艦", without_cursor)
			End If
		End With
		
		Exit Sub
		
FoundPoint: 
		
		If MapX <> xx Or MapY <> yy Then
			Center(xx, yy)
			RefreshScreen(False, True)
		End If
		
		Dim tmp As Boolean
		If Not IsCursorVisible And Not without_cursor Then
			tmp = IsPictureVisible
			DrawPicture("Event\cursor.bmp", DEFAULT_LEVEL, DEFAULT_LEVEL, DEFAULT_LEVEL, DEFAULT_LEVEL, 0, 0, 0, 0, "透過")
			IsPictureVisible = tmp
			IsCursorVisible = True
		End If
		
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		MainForm.picMain(0).Refresh()
	End Sub
	
	Private Function ExecBossRankCmd() As Integer
		Dim u As Unit
		Dim buf As String
		
		Select Case ArgNum
			Case 3
				u = GetArgAsUnit(2)
				
				buf = GetArgAsString(3)
				If Not IsNumeric(buf) Then
					EventErrorMessage = "ボスランクが不正です"
					Error(0)
				End If
			Case 2
				u = SelectedUnitForEvent
				
				buf = GetArgAsString(2)
				If Not IsNumeric(buf) Then
					EventErrorMessage = "ボスランクが不正です"
					Error(0)
				End If
			Case Else
				EventErrorMessage = "BossRankコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not u Is Nothing Then
			With u
				.BossRank = CShort(buf)
				.HP = .MaxHP
				.FullSupply()
			End With
		End If
		
		ExecBossRankCmd = LineNum + 1
	End Function
	
	Private Function ExecBreakCmd() As Integer
		Dim i As Integer
		Dim depth As Short
		
		'対応するLoopもしくはNextコマンドを探す
		depth = 1
		For i = LineNum + 1 To UBound(EventCmd)
			Select Case EventCmd(i).Name
				Case Event_Renamed.CmdType.DoCmd, Event_Renamed.CmdType.ForCmd, Event_Renamed.CmdType.ForEachCmd
					depth = depth + 1
				Case Event_Renamed.CmdType.LoopCmd
					depth = depth - 1
					If depth = 0 Then
						Exit For
					End If
				Case Event_Renamed.CmdType.NextCmd
					depth = depth - 1
					If depth = 0 Then
						ForIndex = ForIndex - 1
						Exit For
					End If
			End Select
		Next 
		
		If i > UBound(EventCmd) Then
			EventErrorMessage = "Breakコマンドがループの外で使われています"
			Error(0)
		End If
		
		ExecBreakCmd = i + 1
	End Function
	
	Private Function ExecCallCmd() As Integer
		Dim ret As Integer
		Dim i As Short
		Dim params(MaxArgIndex) As String
		
		'サブルーチンを探す
		ret = FindNormalLabel(GetArgAsString(2))
		
		'見つかった？
		If ret = 0 Then
			EventErrorMessage = "サブルーチンの呼び出し先ラベルである「" & GetArgAsString(2) & "」がみつかりません"
			Error(0)
		End If
		
		'呼び出し階層をチェック
		If CallDepth > MaxCallDepth Then
			CallDepth = MaxCallDepth
			EventErrorMessage = VB6.Format(MaxCallDepth) & "階層を越えるサブルーチンの呼び出しは出来ません"
			Error(0)
		End If
		
		'引数用スタックが溢れないかチェック
		If ArgIndex + ArgNum - 2 > MaxArgIndex Then
			EventErrorMessage = "サブルーチンの引数の総数が" & VB6.Format(MaxArgIndex) & "個を超えています"
			Error(0)
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
	
	Private Function ExecReturnCmd() As Integer
		If CallDepth <= 0 Then
			EventErrorMessage = "CallコマンドとReturnコマンドが対応していません"
			Error(0)
		ElseIf CallDepth = 1 And CallStack(CallDepth) = 0 Then 
			EventErrorMessage = "CallコマンドとReturnコマンドが対応していません"
			Error(0)
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
	
	Private Function ExecCallInterMissionCommandCmd() As Integer
		Dim fname, save_path As String
		Dim ret As Short
		
		If ArgNum <> 2 Then
			EventErrorMessage = "CallInterMissionCommandコマンドの引数の数が違います"
			Error(0)
		End If
		
		'選択されたインターミッションコマンドを実行
		Select Case GetArgAsString(2)
			Case "データセーブ"
				'一旦「常に手前に表示」を解除
				If frmListBox.Visible Then
					ret = SetWindowPos(frmListBox.Handle.ToInt32, -2, 0, 0, 0, 0, &H3)
				End If
				
				fname = SaveFileDialog("データセーブ", ScenarioPath, GetValueAsString("セーブデータファイル名"), 2, "ｾｰﾌﾞﾃﾞｰﾀ", "src")
				
				'再び「常に手前に表示」
				If frmListBox.Visible Then
					ret = SetWindowPos(frmListBox.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
				End If
				
				'キャンセル？
				If fname = "" Then
					ExecCallInterMissionCommandCmd = LineNum + 1
					Exit Function
				End If
				
				'セーブ先はシナリオフォルダ？
				If InStr(fname, "\") > 0 Then
					save_path = Left(fname, InStr2(fname, "\"))
				End If
				'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
				If Dir(save_path) <> Dir(ScenarioPath) Then
					If MsgBox("セーブファイルはシナリオフォルダにないと読み込めません。" & vbCr & vbLf & "このままセーブしますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question) <> 1 Then
						ExecCallInterMissionCommandCmd = LineNum + 1
						Exit Function
					End If
				End If
				
				If fname <> "" Then
					UList.Update() '追加パイロットを消去
					SaveData(fname)
				End If
				
			Case "機体改造", "ユニットの強化"
				'選択用ダイアログを拡大
				EnlargeListBoxHeight()
				
				RankUpCommand()
				
				'選択用リストボックスを元に戻す
				ReduceListBoxHeight()
				
			Case "乗り換え"
				'選択用ダイアログを拡大
				EnlargeListBoxHeight()
				
				ExchangeUnitCommand()
				
				'選択用リストボックスを元に戻す
				ReduceListBoxHeight()
				
			Case "アイテム交換"
				'選択用ダイアログを拡大
				EnlargeListBoxHeight()
				
				ExchangeItemCommand()
				
				'選択用リストボックスを元に戻す
				ReduceListBoxHeight()
				
			Case "換装"
				'選択用ダイアログを拡大
				EnlargeListBoxHeight()
				
				ExchangeFormCommand()
				
				'選択用リストボックスを元に戻す
				ReduceListBoxHeight()
				
			Case "パイロットステータス"
				frmListBox.Hide()
				ReduceListBoxHeight()
				IsSubStage = True
				If FileExists(ScenarioPath & "Lib\パイロットステータス表示.eve") Then
					StartScenario(ScenarioPath & "Lib\パイロットステータス表示.eve")
				ElseIf FileExists(ExtDataPath & "Lib\パイロットステータス表示.eve") Then 
					StartScenario(ExtDataPath & "Lib\パイロットステータス表示.eve")
				ElseIf FileExists(ExtDataPath2 & "Lib\パイロットステータス表示.eve") Then 
					StartScenario(ExtDataPath2 & "Lib\パイロットステータス表示.eve")
				Else
					StartScenario(AppPath & "Lib\パイロットステータス表示.eve")
				End If
				'サブステージを通常のステージとして実行
				IsSubStage = True
				Exit Function
				
			Case "ユニットステータス"
				frmListBox.Hide()
				ReduceListBoxHeight()
				IsSubStage = True
				If FileExists(ScenarioPath & "Lib\ユニットステータス表示.eve") Then
					StartScenario(ScenarioPath & "Lib\ユニットステータス表示.eve")
				ElseIf FileExists(ExtDataPath & "Lib\ユニットステータス表示.eve") Then 
					StartScenario(ExtDataPath & "Lib\ユニットステータス表示.eve")
				ElseIf FileExists(ExtDataPath2 & "Lib\ユニットステータス表示.eve") Then 
					StartScenario(ExtDataPath2 & "Lib\ユニットステータス表示.eve")
				Else
					StartScenario(AppPath & "Lib\ユニットステータス表示.eve")
				End If
				'サブステージを通常のステージとして実行
				IsSubStage = True
				Exit Function
		End Select
		
		ExecCallInterMissionCommandCmd = LineNum + 1
	End Function
	
	Private Function ExecCancelCmd() As Integer
		IsCanceled = True
		ExecCancelCmd = LineNum + 1
	End Function
	
	Private Function ExecCenterCmd() As Integer
		Dim num As Short
		Dim ux, uy As Short
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
				Center(ux, uy)
			Case 2
				u = GetArgAsUnit(2, True)
				If Not u Is Nothing Then
					With u
						If .Status_Renamed = "出撃" Then
							Center(.X, .Y)
							IsUnitCenter = True
						End If
					End With
				End If
			Case Else
				EventErrorMessage = "Centerコマンドの引数の数が違います"
				Error(0)
		End Select
		
		RedrawScreen(late_refresh)
		
		ExecCenterCmd = LineNum + 1
	End Function
	
	Private Function ExecChangeAreaCmd() As Integer
		Dim new_area As String
		Dim u As Unit
		
		Select Case ArgNum
			Case 2
				u = SelectedUnitForEvent
				new_area = GetArgAsString(2)
			Case 3
				u = GetArgAsUnit(2)
				new_area = GetArgAsString(3)
			Case Else
				EventErrorMessage = "ChangeAreaコマンドの引数の数が違います"
				Error(0)
		End Select
		
		With u
			Select Case TerrainClass(.X, .Y)
				Case "陸"
					If new_area <> "地上" And new_area <> "空中" And new_area <> "地中" Then
						EventErrorMessage = "場所の種類が不正です"
						Error(0)
					End If
				Case "屋内"
					If new_area <> "地上" And new_area <> "空中" Then
						EventErrorMessage = "場所の種類が不正です"
						Error(0)
					End If
				Case "月面"
					If new_area <> "地上" And new_area <> "宇宙" And new_area <> "地中" Then
						EventErrorMessage = "場所の種類が不正です"
						Error(0)
					End If
				Case "水", "深水"
					If new_area <> "水中" And new_area <> "水上" And new_area <> "空中" Then
						EventErrorMessage = "場所の種類が不正です"
						Error(0)
					End If
				Case "空中"
					If new_area <> "空中" Then
						EventErrorMessage = "場所の種類が不正です"
						Error(0)
					End If
				Case "宇宙"
					If new_area <> "宇宙" Then
						EventErrorMessage = "場所の種類が不正です"
						Error(0)
					End If
			End Select
			
			.Area = new_area
			.Update()
			If .Status_Renamed = "出撃" Then
				PaintUnitBitmap(u)
			End If
		End With
		RedrawScreen()
		
		ExecChangeAreaCmd = LineNum + 1
	End Function
	
	'ADD START 240a
	'ChangeLayerコマンド
	'ChangeLayer X Y Name Number [Option]
	Private Function ExecChangeLayerCmd() As Integer
		Dim B As Object
		Dim X, Y As Short
		Dim lname, ltypename As String
		Dim lid, lbitmap As Short
		Dim ltype As Map.BoxTypes
		Dim fname2, fname, fname1, fname3 As String
		Dim basefname As String
		Dim ret As Integer
		Dim i As Short
		Dim isPaintBmp As Boolean
		
		If ArgNum <> 5 And ArgNum <> 6 Then
			EventErrorMessage = "ChangeTerrainコマンドの引数の数が違います"
			Error(0)
		End If
		
		'対象座標を取得
		X = GetArgAsLong(2)
		Y = GetArgAsLong(3)
		If X < 1 Or X > MapWidth Then
			EventErrorMessage = "Ｘ座標の値は1～" & MapWidth & "で指定してください"
			Error(0)
		End If
		If Y < 1 Or Y > MapHeight Then
			EventErrorMessage = "Ｙ座標の値は1～" & MapHeight & "で指定してください"
			Error(0)
		End If
		
		'レイヤー情報・画像を取得
		lname = GetArgAsString(4)
		lbitmap = GetArgAsLong(5)
		If Right(lname, 6) = "(ローカル)" Then
			lname = Left(lname, Len(lname) - 6)
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
				Error(0)
			End If
		End With
		MapData(X, Y, Map.MapDataIndex.LayerType) = lid
		MapData(X, Y, Map.MapDataIndex.LayerBitmapNo) = lbitmap
		
		'マス情報を取得
		isPaintBmp = True
		If ArgNum = 6 Then
			ltypename = GetArgAsString(6)
			If "通常" = ltypename Then
				ltype = Map.BoxTypes.Upper
			ElseIf "情報限定" = ltypename Then 
				isPaintBmp = False
				ltype = Map.BoxTypes.UpperDataOnly
			ElseIf "画像限定" = ltypename Then 
				ltype = Map.BoxTypes.UpperBmpOnly
			Else
				EventErrorMessage = "ChangeLayerコマンドのOptionが不正です"
				Error(0)
			End If
		Else
			ltype = Map.BoxTypes.Upper
		End If
		MapData(X, Y, Map.MapDataIndex.BoxType) = ltype
		
		If isPaintBmp Then
			'マップ画像を検索
			basefname = SearchTerrainImageFile(MapData(X, Y, Map.MapDataIndex.TerrainType), MapData(X, Y, Map.MapDataIndex.BitmapNo), X, Y)
			fname = SearchTerrainImageFile(lid, lbitmap, X, Y)
			
			If fname = "" Then
				EventErrorMessage = "マップビットマップ「" & TDList.Bitmap(lid) & VB6.Format(lbitmap) & ".bmp" & "」が見つかりません"
				Error(0)
			End If
			
			With MainForm
				'マップ画像を背景へ書き込み
				'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picTmp32(0) = System.Drawing.Image.FromFile(basefname)
				Select Case MapDrawMode
					Case "夜"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(MainForm.picTmp32(0))
						Dark()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(MainForm.picTmp32(0))
					Case "セピア"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(MainForm.picTmp32(0))
						Sepia()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(MainForm.picTmp32(0))
					Case "白黒"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(MainForm.picTmp32(0))
						Monotone()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(MainForm.picTmp32(0))
					Case "夕焼け"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(MainForm.picTmp32(0))
						Sunset()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(MainForm.picTmp32(0))
					Case "水中"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(MainForm.picTmp32(0))
						Water()
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(MainForm.picTmp32(0))
					Case "フィルタ"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(0))
						ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(0))
				End Select
				'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = GUI.BitBlt(.picBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
				
				'レイヤー画像を背景へ書き込み
				'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picTmp32(0) = System.Drawing.Image.FromFile(fname)
				BGColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
				Select Case MapDrawMode
					Case "夜"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(MainForm.picTmp32(0))
						Dark(True)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(MainForm.picTmp32(0))
					Case "セピア"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(MainForm.picTmp32(0))
						Sepia(True)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(MainForm.picTmp32(0))
					Case "白黒"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(MainForm.picTmp32(0))
						Monotone(True)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(MainForm.picTmp32(0))
					Case "夕焼け"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(MainForm.picTmp32(0))
						Sunset(True)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(MainForm.picTmp32(0))
					Case "水中"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(MainForm.picTmp32(0))
						Water(True)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(MainForm.picTmp32(0))
					Case "フィルタ"
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						GetImage(.picTmp32(0))
						ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent, True)
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						SetImage(.picTmp32(0))
				End Select
				'レイヤーは透過処理をする
				'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = TransparentBlt(.picBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, BGColor)
				
				'マス目の表示
				If ShowSquareLine Then
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.picBack.Line((32 * (X - 1), 32 * (Y - 1)) - (32 * X, 32 * (Y - 1)), RGB(100, 100, 100), B)
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.picBack.Line((32 * (X - 1), 32 * (Y - 1)) - (32 * (X - 1), 32 * Y), RGB(100, 100, 100), B)
				End If
				
				'マスク入り背景画面を作成しておく
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
				'UPGRADE_ISSUE: Control picMask は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picMask.hDC, 0, 0, SRCAND)
				'UPGRADE_ISSUE: Control picMask2 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picMask2.hDC, 0, 0, SRCINVERT)
			End With
			
			'変更された地形にいたユニットを再表示（ついでにバックバッファからフロントに描画）
			If Not MapDataForUnit(X, Y) Is Nothing Then
				With MapDataForUnit(X, Y)
					'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					MapDataForUnit(X, Y) = Nothing
					EraseUnitBitmap(X, Y, False)
					.StandBy(X, Y, "非同期")
				End With
			Else
				With MainForm
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = TransparentBlt(.picMain(0).hDC, MapToPixelX(X), MapToPixelY(Y), 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
				End With
			End If
			
		End If
		
		ExecChangeLayerCmd = LineNum + 1
	End Function
	'ADD  END  240a
	
	Private Function ExecChangeMapCmd() As Integer
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
					Error(0)
				End If
			Case Else
				EventErrorMessage = "ChangeMapコマンドの引数の数が違います"
				Error(0)
		End Select
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		'出撃中のユニットを撤退させる
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
					If late_refresh Then
						.Escape("非同期")
					Else
						.Escape()
					End If
				End If
			End With
		Next u
		
		fname = GetArgAsString(2)
		If Len(fname) > 0 Then
			LoadMapData(ScenarioPath & fname)
		Else
			LoadMapData("")
		End If
		If late_refresh Then
			SetupBackground("", "非同期")
			RedrawScreen(True)
		Else
			SetupBackground()
			RedrawScreen()
		End If
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ExecChangeMapCmd = LineNum + 1
	End Function
	
	Private Function ExecChangeModeCmd() As Integer
		Dim uarrary() As Unit
		Dim u As Unit
		Dim new_mode As String
		Dim pname As String
		Dim i As Short
		Dim dst_x, dst_y As Short
		
		Dim uarray(1) As Object
		Select Case ArgNum
			Case 2
				uarray(1) = SelectedUnitForEvent
				new_mode = GetArgAsString(2)
			Case 3
				If GetArgAsLong(2) > 0 And GetArgAsLong(3) > 0 Then
					uarray(1) = SelectedUnitForEvent
					dst_x = GetArgAsLong(2)
					dst_y = GetArgAsLong(3)
					If dst_x < 1 Or MapWidth < dst_x Or dst_y < 1 Or MapHeight < dst_y Then
						EventErrorMessage = "ChangeModeコマンドの目的地の座標が不正です"
						Error(0)
					End If
					new_mode = VB6.Format(dst_x) & " " & VB6.Format(dst_y)
				Else
					pname = GetArgAsString(2)
					Select Case pname
						Case "味方", "ＮＰＣ", "敵", "中立"
							ReDim uarray(0)
							For	Each u In UList
								If u.Party0 = pname Then
									ReDim Preserve uarray(UBound(uarray) + 1)
									uarray(UBound(uarray)) = u
								End If
							Next u
						Case Else
							uarray(1) = UList.Item2(pname)
							If uarray(1) Is Nothing Then
								With PList
									If Not .IsDefined(pname) Then
										EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
										Error(0)
									End If
									uarray(1) = .Item(pname).Unit_Renamed
									i = 2
									Do While .IsDefined(pname & ":" & VB6.Format(i))
										ReDim Preserve uarray(UBound(uarray) + 1)
										uarray(UBound(uarray)) = .Item(pname & ":" & VB6.Format(i)).Unit_Renamed
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
						For	Each u In UList
							If u.Party0 = pname Then
								ReDim Preserve uarray(UBound(uarray) + 1)
								uarray(UBound(uarray)) = u
							End If
						Next u
					Case Else
						uarray(1) = UList.Item2(pname)
						If uarray(1) Is Nothing Then
							With PList
								If Not .IsDefined(pname) Then
									EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
									Error(0)
								End If
								uarray(1) = .Item(pname).Unit_Renamed
								i = 2
								Do While .IsDefined(pname & ":" & VB6.Format(i))
									ReDim Preserve uarray(UBound(uarray) + 1)
									uarray(UBound(uarray)) = .Item(pname & ":" & VB6.Format(i)).Unit_Renamed
									i = i + 1
								Loop 
							End With
						End If
				End Select
				dst_x = GetArgAsLong(3)
				dst_y = GetArgAsLong(4)
				If dst_x < 1 Or MapWidth < dst_x Or dst_y < 1 Or MapHeight < dst_y Then
					EventErrorMessage = "ChangeModeコマンドの目的地の座標が不正です"
					Error(0)
				End If
				new_mode = VB6.Format(dst_x) & " " & VB6.Format(dst_y)
			Case Else
				EventErrorMessage = "ChangeModeコマンドの引数の数が違います"
				Error(0)
		End Select
		
		For i = 1 To UBound(uarray)
			If Not uarray(i) Is Nothing Then
				'UPGRADE_WARNING: オブジェクト uarray().Mode の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				uarray(i).Mode = new_mode
			End If
		Next 
		
		ExecChangeModeCmd = LineNum + 1
	End Function
	
	Private Function ExecChangePartyCmd() As Integer
		Dim new_party As String
		Dim pname As String
		Dim u As Unit
		
		Select Case ArgNum
			Case 2
				new_party = GetArgAsString(2)
				If new_party <> "味方" And new_party <> "ＮＰＣ" And new_party <> "敵" And new_party <> "中立" Then
					EventErrorMessage = "陣営の指定が間違っています"
					Error(0)
				End If
				
				SelectedUnitForEvent.ChangeParty(new_party)
			Case 3
				new_party = GetArgAsString(3)
				If new_party <> "味方" And new_party <> "ＮＰＣ" And new_party <> "敵" And new_party <> "中立" Then
					EventErrorMessage = "陣営の指定が間違っています"
					Error(0)
				End If
				
				pname = GetArgAsString(2)
				u = UList.Item2(pname)
				If u Is Nothing Then
					If Not PList.IsDefined(pname) Then
						EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
						Error(0)
					End If
					
					With PList.Item(pname)
						If .Unit_Renamed Is Nothing Then
							.Party = new_party
						Else
							.Unit_Renamed.ChangeParty(new_party)
						End If
					End With
				Else
					u.ChangeParty(new_party)
				End If
			Case Else
				EventErrorMessage = "ChangePartyコマンドの引数の数が違います"
				Error(0)
		End Select
		
		'カーソルが陣営変更されたユニット上にあるとカーソルは消去されるので
		IsCursorVisible = False
		
		ExecChangePartyCmd = LineNum + 1
	End Function
	
	Private Function ExecChangeTerrainCmd() As Integer
		Dim B As Object
		Dim tx, ty As Short
		Dim tname As String
		Dim tid, tbitmap As Short
		Dim fname2, fname, fname1, fname3 As String
		Dim ret As Integer
		Dim i As Short
		
		If ArgNum <> 5 Then
			EventErrorMessage = "ChangeTerrainコマンドの引数の数が違います"
			Error(0)
		End If
		
		tx = GetArgAsLong(2)
		If tx < 1 Or tx > MapWidth Then
			EventErrorMessage = "Ｘ座標の値は1～" & MapWidth & "で指定してください"
			Error(0)
		End If
		
		ty = GetArgAsLong(3)
		If ty < 1 Or ty > MapHeight Then
			EventErrorMessage = "Ｙ座標の値は1～" & MapHeight & "で指定してください"
			Error(0)
		End If
		
		tname = GetArgAsString(4)
		If Right(tname, 6) <> "(ローカル)" Then
			With TDList
				For i = 1 To .Count
					tid = .OrderedID(i)
					If tname = .Name(tid) Then
						Exit For
					End If
				Next 
				If i > .Count Then
					EventErrorMessage = "「" & tname & "」という地形は存在しません"
					Error(0)
				End If
			End With
			
			'MOD START 240a
			'        MapData(tx, ty, 0) = tid
			MapData(tx, ty, Map.MapDataIndex.TerrainType) = tid
			'MOD  END  240a
			
			tbitmap = GetArgAsLong(5)
			'MOD START 240a
			'        MapData(tx, ty, 1) = tbitmap
			MapData(tx, ty, Map.MapDataIndex.BitmapNo) = tbitmap
			'MOD  END  240a
		Else
			tname = Left(tname, Len(tname) - 6)
			
			With TDList
				For i = 1 To .Count
					tid = .OrderedID(i)
					If tname = .Name(tid) Then
						Exit For
					End If
				Next 
				If i > .Count Then
					EventErrorMessage = "「" & tname & "」という地形は存在しません"
					Error(0)
				End If
			End With
			
			'MOD START 240a
			'        MapData(tx, ty, 0) = tid
			MapData(tx, ty, Map.MapDataIndex.TerrainType) = tid
			'MOD  END  240a
			
			tbitmap = -GetArgAsLong(5)
			'MOD START 240a
			'        MapData(tx, ty, 1) = tbitmap
			MapData(tx, ty, Map.MapDataIndex.BitmapNo) = tbitmap
			'MOD  END  240a
		End If
		
		'マップ画像を検索
		fname = SearchTerrainImageFile(tid, tbitmap, tx, ty)
		
		If fname = "" Then
			EventErrorMessage = "マップビットマップ「" & TDList.Bitmap(tid) & VB6.Format(tbitmap) & ".bmp" & "」が見つかりません"
			Error(0)
		End If
		
		With MainForm
			'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picTmp32(0) = System.Drawing.Image.FromFile(fname)
			
			Select Case MapDrawMode
				Case "夜"
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					GetImage(MainForm.picTmp32(0))
					Dark()
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					SetImage(MainForm.picTmp32(0))
				Case "セピア"
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					GetImage(MainForm.picTmp32(0))
					Sepia()
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					SetImage(MainForm.picTmp32(0))
				Case "白黒"
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					GetImage(MainForm.picTmp32(0))
					Monotone()
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					SetImage(MainForm.picTmp32(0))
				Case "夕焼け"
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					GetImage(MainForm.picTmp32(0))
					Sunset()
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					SetImage(MainForm.picTmp32(0))
				Case "水中"
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					GetImage(MainForm.picTmp32(0))
					Water()
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					SetImage(MainForm.picTmp32(0))
				Case "フィルタ"
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					GetImage(.picTmp32(0))
					ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent)
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					SetImage(.picTmp32(0))
			End Select
			
			'背景への書き込み
			'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = GUI.BitBlt(.picBack.hDC, 32 * (tx - 1), 32 * (ty - 1), 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
			
			'マス目の表示
			If ShowSquareLine Then
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picBack.Line((32 * (tx - 1), 32 * (ty - 1)) - (32 * tx, 32 * (ty - 1)), RGB(100, 100, 100), B)
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picBack.Line((32 * (tx - 1), 32 * (ty - 1)) - (32 * (tx - 1), 32 * ty), RGB(100, 100, 100), B)
			End If
			
			'マスク入り背景画面を作成
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (tx - 1), 32 * (ty - 1), 32, 32, .picBack.hDC, 32 * (tx - 1), 32 * (ty - 1), SRCCOPY)
			'UPGRADE_ISSUE: Control picMask は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (tx - 1), 32 * (ty - 1), 32, 32, .picMask.hDC, 0, 0, SRCAND)
			'UPGRADE_ISSUE: Control picMask2 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (tx - 1), 32 * (ty - 1), 32, 32, .picMask2.hDC, 0, 0, SRCINVERT)
		End With
		
		'変更された地形にいたユニットを再表示
		If Not MapDataForUnit(tx, ty) Is Nothing Then
			With MapDataForUnit(tx, ty)
				'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				MapDataForUnit(tx, ty) = Nothing
				EraseUnitBitmap(tx, ty, False)
				.StandBy(tx, ty, "非同期")
			End With
		Else
			With MainForm
				'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				ret = GUI.BitBlt(.picMain(0).hDC, MapToPixelX(tx), MapToPixelY(ty), 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
			End With
		End If
		
		ExecChangeTerrainCmd = LineNum + 1
	End Function
	
	Private Function ExecChangeUnitBitmapCmd() As Integer
		Dim new_bmp, prev_bmp As String
		Dim u As Unit
		
		Select Case ArgNum
			Case 2
				u = SelectedUnitForEvent
				new_bmp = GetArgAsString(2)
			Case 3
				u = GetArgAsUnit(2)
				new_bmp = GetArgAsString(3)
			Case Else
				EventErrorMessage = "ChangeUnitBitmapコマンドの引数の数が違います"
				Error(0)
		End Select
		
		With u
			prev_bmp = .Bitmap
			If LCase(Right(new_bmp, 4)) = ".bmp" Then
				.AddCondition("ユニット画像", -1, 0, "非表示 " & new_bmp)
			ElseIf new_bmp = "-" Then 
				If .IsConditionSatisfied("ユニット画像") Then
					.DeleteCondition("ユニット画像")
				End If
			ElseIf new_bmp = "非表示" Then 
				.AddCondition("非表示付加", -1, 0, "非表示")
				.BitmapID = -1
				EraseUnitBitmap(.X, .Y, False)
			ElseIf new_bmp = "非表示解除" Then 
				If .IsConditionSatisfied("非表示付加") Then
					.DeleteCondition("非表示付加")
				End If
				.BitmapID = MakeUnitBitmap(u)
			Else
				EventErrorMessage = "ビットマップファイル名が不正です"
				Error(0)
			End If
			
			If .Bitmap <> prev_bmp Then
				.BitmapID = MakeUnitBitmap(u)
			End If
			PaintUnitBitmap(u, "リフレッシュ無し")
		End With
		
		ExecChangeUnitBitmapCmd = LineNum + 1
	End Function
	
	Private Function ExecChargeCmd() As Integer
		Dim u As Unit
		
		Select Case ArgNum
			Case 2
				u = GetArgAsUnit(2)
			Case 1
				u = SelectedUnitForEvent
			Case Else
				EventErrorMessage = "Chargeコマンドの引数の数が違います"
				Error(0)
		End Select
		
		u.AddCondition("チャージ", 1)
		
		ExecChargeCmd = LineNum + 1
	End Function
	
	Private Function ExecCircleCmd() As Integer
		Dim pic, pic2 As System.Windows.Forms.PictureBox
		Dim y1, x1, rad As Short
		Dim opt As String
		Dim cname As String
		Dim clr As Integer
		Dim i As Short
		
		If ArgNum < 4 Then
			EventErrorMessage = "Circleコマンドの引数の数が違います"
			Error(0)
		End If
		
		x1 = GetArgAsLong(2) + BaseX
		y1 = GetArgAsLong(3) + BaseY
		rad = GetArgAsLong(4)
		
		SaveScreen()
		
		'描画先
		Select Case ObjDrawOption
			Case "背景"
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picBack
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMaskedBack
				IsMapDirty = True
			Case "保持"
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMain(1)
			Case Else
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
		End Select
		
		'描画領域
		Dim tmp As Short
		If ObjDrawOption <> "背景" Then
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
					Error(0)
				End If
				cname = New String(vbNullChar, 8)
				Mid(cname, 1, 2) = "&H"
				Mid(cname, 3, 2) = Mid(opt, 6, 2)
				Mid(cname, 5, 2) = Mid(opt, 4, 2)
				Mid(cname, 7, 2) = Mid(opt, 2, 2)
				If Not IsNumeric(cname) Then
					EventErrorMessage = "色指定が不正です"
					Error(0)
				End If
				clr = CInt(cname)
			Else
				EventErrorMessage = "Circleコマンドに不正なオプション「" & opt & "」が使われています"
				Error(0)
			End If
		Next 
		
		With pic
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.DrawWidth = ObjDrawWidth
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillColor = ObjFillColor
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillStyle = ObjFillStyle
		End With
		
		'UPGRADE_ISSUE: PictureBox メソッド pic.Circle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.Circle (x1, y1), rad, clr
		
		With pic
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.DrawWidth = 1
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
			'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillStyle = vbFSTransparent
		End With
		
		If Not pic2 Is Nothing Then
			With pic2
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.DrawWidth = ObjDrawWidth
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillColor = ObjFillColor
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillStyle = ObjFillStyle
			End With
			
			'UPGRADE_ISSUE: PictureBox メソッド pic2.Circle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic2.Circle (x1, y1), rad, clr
			
			With pic2
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.DrawWidth = 1
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
				'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillStyle = vbFSTransparent
			End With
		End If
		
		ExecCircleCmd = LineNum + 1
	End Function
	
	Private Function ExecClearEventCmd() As Integer
		Dim ret As Integer
		
		Select Case ArgNum
			Case 2
				ret = FindLabel(GetArgAsString(2))
				If ret > 0 Then
					ClearLabel(ret)
				End If
			Case 1
				If CurrentLabel > 0 Then
					ClearLabel(CurrentLabel)
				End If
			Case Else
				EventErrorMessage = "ClearEventコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExecClearEventCmd = LineNum + 1
	End Function
	
	'互換性維持のために残している
	Private Function ExecClearImageCmd() As Integer
		ClearPicture()
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		MainForm.picMain(0).Refresh()
		ExecClearImageCmd = LineNum + 1
	End Function
	
	'ADD START 240a
	'ExecClearLayerCmd
	'書式１ 全てのLayer情報を削除
	' ClearLayer [Option]
	'書式２ 指定した座標のLayer情報を削除 情報限定・画像限定を選択可能
	' ClearLayer X Y [Option]
	'このモジュールにおいては、 DataOnly＝データのみ消す の意
	Private Function ExecClearLayerCmd() As Integer
		Dim B As Object
		Dim i, X, Y, j As Short
		Dim isDataOnly, isAllClear, isBitmapOnly As Boolean
		Dim fname, loption As String
		Dim ret As Integer
		'引数チェック
		If 4 < ArgNum Then
			EventErrorMessage = "ClearLayerコマンドの引数の数が違います"
			Error(0)
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
				Error(0)
			End If
		End If
		'座標取得
		If Not isAllClear Then
			X = GetArgAsLong(2)
			Y = GetArgAsLong(3)
			If X < 1 Or X > MapWidth Then
				EventErrorMessage = "Ｘ座標の値は1～" & MapWidth & "で指定してください"
				Error(0)
			End If
			If Y < 1 Or Y > MapHeight Then
				EventErrorMessage = "Ｙ座標の値は1～" & MapHeight & "で指定してください"
				Error(0)
			End If
		End If
		'処理開始
		If isAllClear Then
			'全削除実行
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					'レイヤー情報を更新する
					If isDataOnly Then
						MapData(i, j, Map.MapDataIndex.BoxType) = Map.BoxTypes.UpperBmpOnly
					ElseIf isBitmapOnly Then 
						MapData(i, j, Map.MapDataIndex.BoxType) = Map.BoxTypes.UpperDataOnly
					Else
						'両方ともfalseならレイヤー丸ごと削除
						MapData(i, j, Map.MapDataIndex.LayerType) = NO_LAYER_NUM
						MapData(i, j, Map.MapDataIndex.LayerBitmapNo) = NO_LAYER_NUM
						MapData(i, j, Map.MapDataIndex.BoxType) = Map.BoxTypes.Under
					End If
					'レイヤー画像だけを消すことはできないので、下層レイヤーを再描画することで処理する
					fname = SearchTerrainImageFile(MapData(i, j, Map.MapDataIndex.TerrainType), MapData(i, j, Map.MapDataIndex.BitmapNo), i, j)
					
					'データのみ削除の場合は再描画処理をスキップする
					If Not isDataOnly Then
						With MainForm
							'マップ画像を背景へ書き込み
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.picTmp32(0) = System.Drawing.Image.FromFile(fname)
							Select Case MapDrawMode
								Case "夜"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(MainForm.picTmp32(0))
									Dark()
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(MainForm.picTmp32(0))
								Case "セピア"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(MainForm.picTmp32(0))
									Sepia()
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(MainForm.picTmp32(0))
								Case "白黒"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(MainForm.picTmp32(0))
									Monotone()
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(MainForm.picTmp32(0))
								Case "夕焼け"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(MainForm.picTmp32(0))
									Sunset()
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(MainForm.picTmp32(0))
								Case "水中"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(MainForm.picTmp32(0))
									Water()
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(MainForm.picTmp32(0))
								Case "フィルタ"
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									GetImage(.picTmp32(0))
									ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent)
									'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									SetImage(.picTmp32(0))
							End Select
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = GUI.BitBlt(.picBack.hDC, 32 * (i - 1), 32 * (j - 1), 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
							'マス目の表示
							If ShowSquareLine Then
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								.picBack.Line((32 * (i - 1), 32 * (j - 1)) - (32 * i, 32 * (j - 1)), RGB(100, 100, 100), B)
								'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								.picBack.Line((32 * (i - 1), 32 * (j - 1)) - (32 * (i - 1), 32 * j), RGB(100, 100, 100), B)
							End If
							'マスク入り背景画面を作成
							'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (i - 1), 32 * (j - 1), 32, 32, .picBack.hDC, 32 * (i - 1), 32 * (j - 1), SRCCOPY)
							'UPGRADE_ISSUE: Control picMask は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (i - 1), 32 * (j - 1), 32, 32, .picMask.hDC, 0, 0, SRCAND)
							'UPGRADE_ISSUE: Control picMask2 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (i - 1), 32 * (j - 1), 32, 32, .picMask2.hDC, 0, 0, SRCINVERT)
						End With
						'変更された地形にいたユニットを再表示
						If Not MapDataForUnit(i, j) Is Nothing Then
							'一旦ユニットをどかして再配置（変更後の地形が入れない地形の場合に対処）
							'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							MapDataForUnit(i, j) = Nothing
							EraseUnitBitmap(i, j, False)
							MapDataForUnit(i, j).StandBy(i, j, "非同期")
						Else
							With MainForm
								'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								ret = TransparentBlt(.picMain(0).hDC, MapToPixelX(i), MapToPixelY(j), 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
							End With
						End If
					End If
				Next 
			Next 
		Else
			'指定した座標のみレイヤー情報を更新する
			If isDataOnly Then
				MapData(X, Y, Map.MapDataIndex.BoxType) = Map.BoxTypes.UpperBmpOnly
			ElseIf isBitmapOnly Then 
				MapData(X, Y, Map.MapDataIndex.BoxType) = Map.BoxTypes.UpperDataOnly
			Else
				'両方ともfalseならレイヤー丸ごと削除
				MapData(X, Y, Map.MapDataIndex.LayerType) = NO_LAYER_NUM
				MapData(X, Y, Map.MapDataIndex.LayerBitmapNo) = NO_LAYER_NUM
				MapData(X, Y, Map.MapDataIndex.BoxType) = Map.BoxTypes.Under
			End If
			
			'データのみの場合は再描画処理をスキップ
			If Not isDataOnly Then
				fname = SearchTerrainImageFile(MapData(X, Y, Map.MapDataIndex.TerrainType), MapData(X, Y, Map.MapDataIndex.BitmapNo), X, Y)
				With MainForm
					'マップ画像を背景へ書き込み
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.picTmp32(0) = System.Drawing.Image.FromFile(fname)
					Select Case MapDrawMode
						Case "夜"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(MainForm.picTmp32(0))
							Dark()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(MainForm.picTmp32(0))
						Case "セピア"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(MainForm.picTmp32(0))
							Sepia()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(MainForm.picTmp32(0))
						Case "白黒"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(MainForm.picTmp32(0))
							Monotone()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(MainForm.picTmp32(0))
						Case "夕焼け"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(MainForm.picTmp32(0))
							Sunset()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(MainForm.picTmp32(0))
						Case "水中"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(MainForm.picTmp32(0))
							Water()
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(MainForm.picTmp32(0))
						Case "フィルタ"
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							GetImage(.picTmp32(0))
							ColorFilter(MapDrawFilterColor, MapDrawFilterTransPercent)
							'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							SetImage(.picTmp32(0))
					End Select
					'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = GUI.BitBlt(.picBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picTmp32(0).hDC, 0, 0, SRCCOPY)
					'マス目の表示
					If ShowSquareLine Then
						'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.picBack.Line((32 * (X - 1), 32 * (Y - 1)) - (32 * X, 32 * (Y - 1)), RGB(100, 100, 100), B)
						'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						.picBack.Line((32 * (X - 1), 32 * (Y - 1)) - (32 * (X - 1), 32 * Y), RGB(100, 100, 100), B)
					End If
					'マスク入り背景画面を作成
					'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picBack.hDC, 32 * (X - 1), 32 * (Y - 1), SRCCOPY)
					'UPGRADE_ISSUE: Control picMask は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picMask.hDC, 0, 0, SRCAND)
					'UPGRADE_ISSUE: Control picMask2 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = GUI.BitBlt(.picMaskedBack.hDC, 32 * (X - 1), 32 * (Y - 1), 32, 32, .picMask2.hDC, 0, 0, SRCINVERT)
				End With
				'変更された地形にいたユニットを再表示
				If Not MapDataForUnit(X, Y) Is Nothing Then
					'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					MapDataForUnit(X, Y) = Nothing
					EraseUnitBitmap(X, Y, False)
					MapDataForUnit(X, Y).StandBy(X, Y, "非同期")
				Else
					With MainForm
						'UPGRADE_ISSUE: Control picTmp32 は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						ret = TransparentBlt(.picMain(0).hDC, MapToPixelX(X), MapToPixelY(Y), 32, 32, .picTmp32(0).hDC, 0, 0, 32, 32, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White))
					End With
				End If
			End If
		End If
		
		ExecClearLayerCmd = LineNum + 1
		
	End Function
	'ADD  END  240a
	
	Private Function ExecClearObjCmd() As Integer
		Dim j, i, n As Short
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
						If frmToolTip.Visible And SelectedAlternative = .Name Then
							'ツールチップを消す
							frmToolTip.Hide()
							'マウスカーソルを元に戻す
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							MainForm.picMain(0).MousePointer = 0
						End If
					End With
					For j = i To UBound(HotPointList) - 1
						'UPGRADE_WARNING: オブジェクト HotPointList(j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						HotPointList(j) = HotPointList(j + 1)
					Next 
					ReDim Preserve HotPointList(UBound(HotPointList) - 1)
				End If
			Case 1
				ReDim HotPointList(0)
			Case Else
				EventErrorMessage = "ClearObjコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExecClearObjCmd = LineNum + 1
		
		'まだマウスカーソルがホットポイント上にあるか？
		For i = 1 To UBound(HotPointList)
			With HotPointList(i)
				If .Left_Renamed <= MouseX And MouseX < .Left_Renamed + .width And .Top <= MouseY And MouseY < .Top + .Height Then
					Exit Function
				End If
			End With
		Next 
		
		'ツールチップを消す
		frmToolTip.Hide()
		If Not without_refresh Then
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Refresh()
		End If
		
		'マウスカーソルを元に戻す
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		MainForm.picMain(0).MousePointer = 0
	End Function
	
	Private Function ExecClearPictureCmd() As Integer
		Select Case ArgNum
			Case 1
				ClearPicture()
			Case 5
				ClearPicture2(GetArgAsLong(2) + BaseX, GetArgAsLong(3) + BaseY, GetArgAsLong(4) + BaseX, GetArgAsLong(5) + BaseY)
			Case Else
				EventErrorMessage = "ClearPictureコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExecClearPictureCmd = LineNum + 1
	End Function
	
	Private Function ExecClearSkillCmd() As Integer
		Dim pname As String
		Dim slist, sname, sname2, buf As String
		Dim sarray() As String
		Dim vname, vname2 As String
		Dim i, j As Short
		
		pname = GetArgAsString(2)
		If PList.IsDefined(pname) Then
			pname = PList.Item(pname).ID
		ElseIf PDList.IsDefined(pname) Then 
			pname = PDList.Item(pname).Name
		Else
			EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
			Error(0)
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
				UndefineVariable(vname2)
			End If
			
			'レベル設定用変数を削除
			UndefineVariable(vname)
			
			'特殊能力一覧作成用変数を削除
			vname = "Ability(" & pname & ")"
			If IsGlobalVariableDefined(vname) Then
				buf = GetValueAsString(vname)
				slist = ""
				For j = 1 To LLength(buf)
					If LIndex(buf, j) <> sname And LIndex(buf, j) <> sname2 Then
						slist = slist & " " & LIndex(buf, j)
					End If
				Next 
				If LLength(slist) > 0 Then
					slist = Trim(slist)
					SetVariableAsString(vname, slist)
				Else
					UndefineVariable(vname)
				End If
			End If
		Next 
		
		'パイロットやユニットのステータスをアップデート
		If PList.IsDefined(pname) Then
			With PList.Item(pname)
				.Update()
				If Not .Unit_Renamed Is Nothing Then
					.Unit_Renamed.Update()
					If .Unit_Renamed.Status_Renamed = "出撃" Then
						PList.UpdateSupportMod(.Unit_Renamed)
					End If
				End If
			End With
		End If
		
		ExecClearSkillCmd = LineNum + 1
	End Function
	
	Private Function ExecClearSpecialPowerCmd() As Integer
		Dim sname As String
		Dim u As Unit
		
		Select Case ArgNum
			Case 3
				u = GetArgAsUnit(2)
				sname = GetArgAsString(3)
			Case 2
				u = SelectedUnitForEvent
				sname = GetArgAsString(2)
			Case Else
				EventErrorMessage = "ClearSpecialPowerコマンドの引数の数が違います"
				Error(0)
		End Select
		
		With u
			If .IsSpecialPowerInEffect(sname) Then
				.RemoveSpecialPowerInEffect2(sname)
			End If
		End With
		
		ExecClearSpecialPowerCmd = LineNum + 1
	End Function
	
	Private Function ExecClearStatusCmd() As Integer
		Dim sname As String
		Dim u As Unit
		
		Select Case ArgNum
			Case 3
				u = GetArgAsUnit(2)
				sname = GetArgAsString(3)
			Case 2
				u = SelectedUnitForEvent
				sname = GetArgAsString(2)
			Case Else
				EventErrorMessage = "ClearStatusコマンドの引数の数が違います"
				Error(0)
		End Select
		
		With u
			If .IsConditionSatisfied(sname) Then
				.DeleteCondition(sname)
				.Update()
				If .Status_Renamed = "出撃" Then
					PaintUnitBitmap(u)
				End If
			End If
		End With
		
		ExecClearStatusCmd = LineNum + 1
	End Function
	
	Private Function ExecCloseCmd() As Integer
		If ArgNum <> 2 Then
			EventErrorMessage = "Closeコマンドの引数の数が違います"
			Error(0)
		End If
		
		FileClose(GetArgAsLong(2))
		
		ExecCloseCmd = LineNum + 1
	End Function
	
	Private Function ExecClsCmd() As Integer
		Dim BF As Object
		Dim cname, buf As String
		Dim ret As Integer
		
		Select Case ArgNum
			Case 2
				buf = GetArgAsString(2)
				If Asc(buf) <> 35 Or Len(buf) <> 7 Then
					EventErrorMessage = "色指定が不正です"
					Error(0)
				End If
				cname = New String(vbNullChar, 8)
				Mid(cname, 1, 2) = "&H"
				Mid(cname, 3, 2) = Mid(buf, 6, 2)
				Mid(cname, 5, 2) = Mid(buf, 4, 2)
				Mid(cname, 7, 2) = Mid(buf, 2, 2)
				If Not IsNumeric(cname) Then
					EventErrorMessage = "色指定が不正です"
					Error(0)
				End If
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(0).Line((0, 0) - (MainPWidth - 1, MainPHeight - 1), CInt(cname), BF)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				MainForm.picMain(1).Line((0, 0) - (MainPWidth - 1, MainPHeight - 1), CInt(cname), BF)
				ScreenIsSaved = True
			Case 1
				With MainForm
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = PatBlt(.picMain(0).hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					ret = PatBlt(.picMain(1).hDC, 0, 0, MainPWidth, MainPHeight, BLACKNESS)
				End With
				ScreenIsSaved = True
			Case Else
				EventErrorMessage = "Clsコマンドの引数の数が違います"
				Error(0)
		End Select
		
		IsPictureVisible = True
		IsCursorVisible = False
		
		PaintedAreaX1 = MainPWidth
		PaintedAreaY1 = MainPHeight
		PaintedAreaX2 = -1
		PaintedAreaY2 = -1
		
		ExecClsCmd = LineNum + 1
	End Function
	
	Private Function ExecColorCmd() As Integer
		Dim opt, cname As String
		
		If ArgNum <> 2 Then
			EventErrorMessage = "Colorコマンドの引数の数が違います"
			Error(0)
		End If
		
		opt = GetArgAsString(2)
		If Asc(opt) <> 35 Or Len(opt) <> 7 Then
			EventErrorMessage = "色指定が不正です"
			Error(0)
		End If
		cname = New String(vbNullChar, 8)
		Mid(cname, 1, 2) = "&H"
		Mid(cname, 3, 2) = Mid(opt, 6, 2)
		Mid(cname, 5, 2) = Mid(opt, 4, 2)
		Mid(cname, 7, 2) = Mid(opt, 2, 2)
		If Not IsNumeric(cname) Then
			EventErrorMessage = "色指定が不正です"
			Error(0)
		End If
		
		ObjColor = CInt(cname)
		
		ExecColorCmd = LineNum + 1
	End Function
	
	Private Function ExecColorFilterCmd() As Integer
		Dim prev_x, prev_y As Short
		Dim u As Unit
		Dim late_refresh As Boolean
		Dim buf As String
		Dim fcolor As Integer
		Dim i As Short
		Dim trans_par As Double
		
		If ArgNum < 2 Then
			EventErrorMessage = "ColorFilterコマンドの引数の数が違います"
			Error(0)
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
						EventErrorMessage = "ColorFilterコマンドに不正なオプション「" & buf & "」が使われています"
						Error(0)
					End If
			End Select
		Next 
		
		buf = GetArgAsString(2)
		buf = "&H" & Mid(buf, 6, 2) & Mid(buf, 4, 2) & Mid(buf, 2, 2)
		If IsNumeric(buf) Then
			fcolor = CInt(buf)
		Else
			EventErrorMessage = "ColorFilterコマンドのカラー指定が不正です"
			Error(0)
		End If
		
		prev_x = MapX
		prev_y = MapY
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		SetupBackground("フィルタ", "非同期", fcolor, trans_par)
		
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Then
					If .BitmapID = 0 Then
						With UList.Item(.Name)
							If u.Party0 = .Party0 And .BitmapID <> 0 And u.Bitmap = .Bitmap And Not .IsFeatureAvailable("ダミーユニット") Then
								u.BitmapID = .BitmapID
							Else
								u.BitmapID = MakeUnitBitmap(u)
							End If
						End With
					End If
				End If
			End With
		Next u
		
		Center(prev_x, prev_y)
		RedrawScreen(late_refresh)
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ExecColorFilterCmd = LineNum + 1
	End Function
	
	Private Function ExecCombineCmd() As Integer
		Dim u As Unit
		Dim uname As String
		Dim anum As Short
		
		Select Case ArgNum
			Case 2
				u = SelectedUnitForEvent
				uname = GetArgAsString(2)
			Case 3
				u = GetArgAsUnit(2)
				uname = GetArgAsString(3)
			Case Else
				EventErrorMessage = "Combineコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not UList.IsDefined(uname) Then
			EventErrorMessage = "「" & uname & "」というユニットが見つかりません"
			Error(0)
		End If
		
		If u.CurrentForm.ID <> UList.Item(uname).CurrentForm.ID Then
			With u
				anum = .UsedAction
				.Combine(uname, True)
				If Not SelectedUnit Is Nothing Then
					If .ID = SelectedUnit.ID Then
						SelectedUnit = UList.Item(uname)
					End If
				End If
				If Not SelectedUnitForEvent Is Nothing Then
					If .ID = SelectedUnitForEvent.ID Then
						SelectedUnitForEvent = UList.Item(uname)
					End If
				End If
				If Not SelectedTarget Is Nothing Then
					If .ID = SelectedTarget.ID Then
						SelectedTarget = UList.Item(uname)
					End If
				End If
				If Not SelectedTargetForEvent Is Nothing Then
					If .ID = SelectedTargetForEvent.ID Then
						SelectedTargetForEvent = UList.Item(uname)
					End If
				End If
			End With
			
			With UList.Item(uname)
				.UsedAction = anum
				If .Status_Renamed = "出撃" Then
					RedrawScreen()
				End If
			End With
		End If
		
		ExecCombineCmd = LineNum + 1
	End Function
	
	Private Function ExecConfirmCmd() As Integer
		Dim ret As Short
		
		If ArgNum <> 2 Then
			EventErrorMessage = "Confirmコマンドの引数の数が違います"
			Error(0)
		End If
		
		'一度イベントを解消しておかないとMsgBoxを連続で使用したときに
		'動作がおかしくなる（ＶＢのバグ？）
		System.Windows.Forms.Application.DoEvents()
		
		ret = MsgBox(GetArgAsString(2), MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "選択")
		If ret = 1 Then
			SelectedAlternative = CStr(1)
		Else
			SelectedAlternative = CStr(0)
		End If
		
		ExecConfirmCmd = LineNum + 1
	End Function
	
	Private Function ExecContinueCmd() As Integer
		Dim msg As String
		Dim n, i As Short
		Dim p As Pilot
		Dim plevel As Short
		Dim u As Unit
		
		Select Case ArgNum
			Case 2
				If Not IsGlobalVariableDefined("次ステージ") Then
					DefineGlobalVariable("次ステージ")
				End If
				SetVariableAsString("次ステージ", GetArgAsString(2))
			Case 1
			Case Else
				EventErrorMessage = "Continueコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ClearUnitStatus()
		
		'追加経験値を得るパイロットや破壊されたユニットがいなければ処理をスキップ
		n = 0
		For	Each u In UList
			With u
				If .Party0 = "味方" Then
					If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Or .Status_Renamed = "破壊" Then
						n = 1
						Exit For
					End If
				End If
			End With
		Next u
		If n = 0 Then
			Turn = 0
		End If
		
		'追加経験値を収得
		If Turn > 0 And Not IsOptionDefined("追加経験値無効") Then
			OpenMessageForm()
			
			n = 0
			msg = ""
			For	Each p In PList
				With p
					If .Party <> "味方" Then
						GoTo NextPilot
					End If
					
					If .MaxSP = 0 Then
						GoTo NextPilot
					End If
					
					If .Unit_Renamed Is Nothing Then
						GoTo NextPilot
					End If
					
					If .Unit_Renamed.Status_Renamed <> "出撃" And .Unit_Renamed.Status_Renamed <> "格納" Then
						GoTo NextPilot
					End If
					
					plevel = .Level
					.Exp = .Exp + 2 * .SP
					
					'追加パイロットや暴走時パイロットに関する処理
					If .Unit_Renamed.CountPilot > 0 And Not .IsSupport(.Unit_Renamed) Then
						'追加パイロットがメインパイロットの場合
						If p Is .Unit_Renamed.Pilot(1) And Not p Is .Unit_Renamed.MainPilot And .Unit_Renamed.MainPilot.MaxSP > 0 Then
							GoTo NextPilot
						End If
						
						'追加パイロットがメインパイロットではなくなった場合
						If Not p Is .Unit_Renamed.MainPilot Then
							'自分がユニットのパイロット一覧に含まれているか判定
							For i = 1 To .Unit_Renamed.CountPilot
								If p Is .Unit_Renamed.Pilot(i) Then
									Exit For
								End If
							Next 
							If i > .Unit_Renamed.CountPilot Then
								GoTo NextPilot
							End If
						End If
					End If
					
					If plevel = .Level Then
						msg = msg & ";" & .Nickname & " 経験値 +" & VB6.Format(2 * .SP)
					Else
						msg = msg & ";" & .Nickname & " 経験値 +" & VB6.Format(2 * .SP) & " レベルアップ！（Lv" & VB6.Format(.Level) & "）"
					End If
					n = n + 1
					If n = 4 Then
						DisplayMessage("システム", Mid(msg, 2))
						msg = ""
						n = 0
					End If
				End With
NextPilot: 
			Next p
			If n > 0 Then
				DisplayMessage("システム", Mid(msg, 2))
			End If
			
			CloseMessageForm()
		End If
		
		MainForm.Hide()
		
		'エピローグイベントを実行
		If IsEventDefined("エピローグ") Then
			'ハイパーモードや変身、能力コピーを解除
			For	Each u In UList
				With u
					If .Status_Renamed <> "他形態" And .Status_Renamed <> "旧主形態" And .Status_Renamed <> "旧形態" Then
						If .IsFeatureAvailable("ノーマルモード") Then
							.Transform(LIndex(.FeatureData("ノーマルモード"), 1))
						End If
					End If
				End With
			Next u
			
			If IsEventDefined("エピローグ", True) Then
				StopBGM()
				StartBGM(BGMName("Briefing"))
			End If
			
			Stage = "エピローグ"
			HandleEvent("エピローグ")
		End If
		
		MainForm.Hide()
		
		'インターミッションに移行
		If Not IsSubStage Then
			'
			InterMissionCommand()
			
			If Not IsSubStage Then
				If GetValueAsString("次ステージ") = "" Then
					EventErrorMessage = "次のステージのファイル名が設定されていません"
					Error(0)
				End If
				
				StartScenario(GetValueAsString("次ステージ"))
			Else
				IsSubStage = False
			End If
		End If
		
		IsScenarioFinished = True
		
		ExecContinueCmd = 0
	End Function
	
	Private Function ExecCopyArrayCmd() As Integer
		Dim i As Integer
		Dim j As Short
		Dim buf As String
		Dim var As VarData
		Dim name1, name2 As String
		
		If ArgNum <> 3 Then
			EventErrorMessage = "CopyArrayコマンドの引数の数が違います"
			Error(0)
		End If
		
		'コピー元の変数名
		name1 = GetArg(2)
		If Left(name1, 1) = "$" Then
			name1 = Mid(name1, 2)
		End If
		'Eval関数
		If LCase(Left(name1, 5)) = "eval(" Then
			If Right(name1, 1) = ")" Then
				name1 = Mid(name1, 6, Len(name1) - 6)
				name1 = GetValueAsString(name1)
			End If
		End If
		
		'コピー先の変数名
		name2 = GetArg(3)
		If Left(name2, 1) = "$" Then
			name1 = Mid(name2, 2)
		End If
		'Eval関数
		If LCase(Left(name2, 5)) = "eval(" Then
			If Right(name2, 1) = ")" Then
				name2 = Mid(name2, 6, Len(name2) - 6)
				name2 = GetValueAsString(name2)
			End If
		End If
		
		'コピー先の変数を初期化
		'サブルーチンローカル変数の場合
		If IsSubLocalVariableDefined(name2) Then
			UndefineVariable(name2)
			VarIndex = VarIndex + 1
			With VarStack(VarIndex)
				.Name = name2
				.VariableType = Expression.ValueType.StringType
				.StringValue = ""
			End With
			'ローカル変数の場合
		ElseIf IsLocalVariableDefined(name2) Then 
			UndefineVariable(name2)
			DefineLocalVariable(name2)
			'グローバル変数の場合
		ElseIf IsGlobalVariableDefined(name2) Then 
			UndefineVariable(name2)
			DefineGlobalVariable(name2)
		End If
		
		'配列を検索し、配列要素を見つける
		buf = ""
		If IsSubLocalVariableDefined(name1) Then
			'サブルーチンローカルな配列に対するCopyArray
			For i = VarIndexStack(CallDepth - 1) + 1 To VarIndex
				With VarStack(i)
					If InStr(.Name, name1 & "[") = 1 Then
						buf = name2 & Mid(.Name, InStr(.Name, "["))
						SetVariable(buf, .VariableType, .StringValue, .NumericValue)
					End If
				End With
			Next 
			If buf = "" Then
				var = GetVariableObject(name1)
				With var
					SetVariable(name2, .VariableType, .StringValue, .NumericValue)
				End With
			End If
		ElseIf IsLocalVariableDefined(name1) Then 
			'ローカルな配列に対するCopyArray
			For	Each var In LocalVariableList
				With var
					If InStr(.Name, name1 & "[") = 1 Then
						buf = name2 & Mid(.Name, InStr(.Name, "["))
						SetVariable(buf, .VariableType, .StringValue, .NumericValue)
					End If
				End With
			Next var
			If buf = "" Then
				var = GetVariableObject(name1)
				With var
					SetVariable(name2, .VariableType, .StringValue, .NumericValue)
				End With
			End If
		ElseIf IsGlobalVariableDefined(name1) Then 
			'グローバルな配列に対するCopyArray
			For	Each var In GlobalVariableList
				With var
					If InStr(.Name, name1 & "[") = 1 Then
						buf = name2 & Mid(.Name, InStr(.Name, "["))
						SetVariable(buf, .VariableType, .StringValue, .NumericValue)
					End If
				End With
			Next var
			If buf = "" Then
				var = GetVariableObject(name1)
				With var
					SetVariable(name2, .VariableType, .StringValue, .NumericValue)
				End With
			End If
		End If
		
		'UPGRADE_NOTE: オブジェクト var をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		var = Nothing
		
		ExecCopyArrayCmd = LineNum + 1
	End Function
	
	Private Function ExecCopyFileCmd() As Integer
		Dim fname1, fname2 As String
		
		If ArgNum <> 3 Then
			EventErrorMessage = "CopyFileコマンドの引数の数が違います"
			Error(0)
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
			Error(0)
		End If
		If InStr(fname1, "../") > 0 Then
			EventErrorMessage = "ファイル指定に「../」は使えません"
			Error(0)
		End If
		
		fname2 = ScenarioPath & GetArgAsString(3)
		
		If InStr(fname2, "..\") > 0 Then
			EventErrorMessage = "ファイル指定に「..\」は使えません"
			Error(0)
		End If
		If InStr(fname2, "../") > 0 Then
			EventErrorMessage = "ファイル指定に「../」は使えません"
			Error(0)
		End If
		
		FileCopy(fname1, fname2)
		
		ExecCopyFileCmd = LineNum + 1
	End Function
	
	Private Function ExecCreateCmd() As Integer
		Dim uname, uparty As String
		Dim urank As Short
		Dim pname As String
		Dim plevel As Short
		Dim ux, uy As Short
		Dim u As Unit
		Dim p As Pilot
		Dim buf As String
		Dim i, num As Short
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
			Error(0)
		ElseIf num <> 8 And num <> 9 Then 
			EventErrorMessage = "Createコマンドの引数の数が違います"
			Error(0)
		End If
		
		uparty = GetArgAsString(2)
		If Not (uparty = "味方" Or uparty = "ＮＰＣ" Or uparty = "敵" Or uparty = "中立") Then
			EventErrorMessage = "所属の指定「" & uparty & "」が間違っています"
			Error(0)
		End If
		
		uname = GetArgAsString(3)
		If Not UDList.IsDefined(uname) Then
			EventErrorMessage = "指定したユニット「" & uname & "」のデータが見つかりません"
			Error(0)
		End If
		
		buf = GetArgAsString(4)
		If Not IsNumeric(buf) Then
			EventErrorMessage = "ユニットのランクが不正です"
			Error(0)
		End If
		urank = CShort(buf)
		
		pname = GetArgAsString(5)
		If Not PDList.IsDefined(pname) Then
			EventErrorMessage = "指定したパイロット「" & pname & "」のデータが見つかりません"
			Error(0)
		End If
		
		buf = GetArgAsString(6)
		If Not IsNumeric(buf) Then
			EventErrorMessage = "パイロットのレベルが不正です"
			Error(0)
		End If
		plevel = CShort(buf)
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
			Error(0)
		End If
		ux = CShort(buf)
		If ux < 1 Then
			ux = 1
		ElseIf ux > MapWidth Then 
			ux = MapWidth
		End If
		
		buf = GetArgAsString(8)
		If Not IsNumeric(buf) Then
			EventErrorMessage = "Ｙ座標の値が不正です"
			Error(0)
		End If
		uy = CShort(buf)
		If uy < 1 Then
			uy = 1
		ElseIf uy > MapHeight Then 
			uy = MapHeight
		End If
		
		u = UList.Add(uname, urank, uparty)
		If u Is Nothing Then
			EventErrorMessage = uname & "のデータが不正です"
			Error(0)
		End If
		
		If num = 9 Then
			p = PList.Add(pname, plevel, uparty, GetArgAsString(9))
		Else
			p = PList.Add(pname, plevel, uparty)
		End If
		
		p.Ride(u)
		
		If opt <> "非同期" And MainForm.Visible And Not IsPictureVisible Then
			Center(ux, uy)
			RefreshScreen()
		End If
		With u
			.FullRecover()
			For i = 1 To .CountOtherForm
				.OtherForm(i).FullSupply()
			Next 
			.UsedAction = 0
			
			.StandBy(ux, uy, opt)
			
			.CheckAutoHyperMode()
		End With
		
		SelectedUnitForEvent = u.CurrentForm
		
		ExecCreateCmd = LineNum + 1
	End Function
	
	Private Function ExecCreateFolderCmd() As Integer
		Dim fname As String
		
		If ArgNum <> 2 Then
			EventErrorMessage = "CreateFolderコマンドの引数の数が違います"
			Error(0)
		End If
		
		fname = ScenarioPath & GetArgAsString(2)
		
		If InStr(fname, "..\") > 0 Then
			EventErrorMessage = "フォルダ指定に「..\」は使えません"
			Error(0)
		End If
		If InStr(fname, "../") > 0 Then
			EventErrorMessage = "フォルダ指定に「../」は使えません"
			Error(0)
		End If
		
		If Right(fname, 1) = "\" Then
			fname = Left(fname, Len(fname) - 1)
		End If
		
		If Not FileExists(fname) Then
			MkDir(fname)
		End If
		
		ExecCreateFolderCmd = LineNum + 1
	End Function
	
	Private Function ExecDebugCmd() As Integer
		Dim i As Short
		
		For i = 2 To ArgNum
			If i > 2 Then
				System.Diagnostics.Debug.Write(", ")
			End If
			System.Diagnostics.Debug.Write(GetArgAsString(i))
		Next 
		Debug.Print("")
		
		ExecDebugCmd = LineNum + 1
	End Function
	
	Private Function ExecDestroyCmd() As Integer
		Dim u As Unit
		Dim uparty As String
		Dim i As Short
		
		Select Case ArgNum
			Case 2
				u = GetArgAsUnit(2)
			Case 1
				u = SelectedUnitForEvent
			Case Else
				EventErrorMessage = "Destroyコマンドの引数の数が違います"
				Error(0)
		End Select
		
		'破壊キャンセル状態にある場合は解除しておく
		If u.IsConditionSatisfied("破壊キャンセル") Then
			u.DeleteCondition("破壊キャンセル")
		End If
		
		Select Case u.Status_Renamed
			Case "出撃"
				u.Die()
			Case "格納"
				u.Escape()
				u.Status_Renamed = "破壊"
			Case "破壊"
				If MapDataForUnit(u.X, u.Y) Is u Then
					u.Die()
					'既に破壊イベントが発生しているはずなので、ここで終了
					ExecDestroyCmd = LineNum + 1
					Exit Function
				End If
			Case Else
				u.Status_Renamed = "破壊"
		End Select
		
		'ステータス表示中の場合は表示を解除
		If u Is DisplayedUnit Then
			ClearUnitStatus()
		End If
		
		'Destroyコマンドによって全滅したかを判定
		uparty = u.Party0
		For	Each u In UList
			With u
				If .Party0 = uparty And (.Status_Renamed = "出撃" Or .Status_Renamed = "格納") And Not .IsConditionSatisfied("憑依") Then
					ExecDestroyCmd = LineNum + 1
					Exit Function
				End If
			End With
		Next u
		
		'戦闘時以外のイベント中の破壊は無視
		For i = 1 To UBound(EventQue)
			If EventQue(i) = "プロローグ" Or EventQue(i) = "エピローグ" Or EventQue(i) = "スタート" Or EventQue(i) = "全滅" Then
				ExecDestroyCmd = LineNum + 1
				Exit Function
			End If
		Next 
		
		'後で全滅イベントを実行
		RegisterEvent("全滅", uparty)
		
		ExecDestroyCmd = LineNum + 1
	End Function
	
	Private Function ExecDisableCmd() As Integer
		Dim vname, aname, uname As String
		Dim u As Unit
		Dim i As Short
		Dim need_update As Boolean
		
		Select Case ArgNum
			Case 2
				aname = GetArgAsString(2)
			Case 3
				uname = GetArgAsString(2)
				aname = GetArgAsString(3)
			Case Else
				EventErrorMessage = "Disableコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If aname = "" Then
			EventErrorMessage = "Disableコマンドに指定された能力名が空文字列です"
			Error(0)
		End If
		
		If uname <> "" Then
			vname = "Disable(" & uname & "," & aname & ")"
		Else
			vname = "Disable(" & aname & ")"
		End If
		
		'Disable用変数を設定
		If Not IsGlobalVariableDefined(vname) Then
			DefineGlobalVariable(vname)
			SetVariableAsLong(vname, 1)
		Else
			'既に設定済みであればそのまま終了
			ExecDisableCmd = LineNum + 1
			Exit Function
		End If
		
		'ユニットのステータスを更新
		If uname <> "" Then
			With UList
				If .IsDefined(uname) Then
					.Item(uname).CurrentForm.Update()
				End If
			End With
		Else
			For	Each u In UList
				With u
					If .Status_Renamed = "出撃" Then
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
							.Update()
						End If
					End If
				End With
			Next u
		End If
		
		ExecDisableCmd = LineNum + 1
	End Function
	
	Private Function ExecDoCmd() As Integer
		Dim i As Integer
		Dim depth As Short
		
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
						Error(0)
				End Select
			Case Else
				EventErrorMessage = "Doコマンドの引数の数が違います"
				Error(0)
		End Select
		
		'条件式がFalseのため本体をスキップ
		depth = 1
		For i = LineNum + 1 To UBound(EventCmd)
			Select Case EventCmd(i).Name
				Case Event_Renamed.CmdType.DoCmd
					depth = depth + 1
				Case Event_Renamed.CmdType.LoopCmd
					depth = depth - 1
					If depth = 0 Then
						ExecDoCmd = i + 1
						Exit Function
					End If
			End Select
		Next 
		
		EventErrorMessage = "DoとLoopが対応していません"
		Error(0)
	End Function
	
	Private Function ExecLoopCmd() As Integer
		Dim i As Integer
		Dim depth As Short
		
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
						Error(0)
				End Select
			Case Else
				EventErrorMessage = "Loop文の引数の数が違います"
				Error(0)
		End Select
		
		'条件式がTrueのため先頭に戻る
		i = LineNum
		depth = 1
		Do While i > 1
			i = i - 1
			Select Case EventCmd(i).Name
				Case Event_Renamed.CmdType.DoCmd
					depth = depth - 1
					If depth = 0 Then
						ExecLoopCmd = i
						Exit Function
					End If
				Case Event_Renamed.CmdType.LoopCmd
					depth = depth + 1
			End Select
		Loop 
		
		EventErrorMessage = "DoとLoopが対応していません"
		Error(0)
	End Function
	
	Private Function ExecDrawOptionCmd() As Integer
		If ArgNum <> 2 Then
			EventErrorMessage = "DrawOptionコマンドの引数の数が違います"
			Error(0)
		End If
		
		ObjDrawOption = GetArgAsString(2)
		
		ExecDrawOptionCmd = LineNum + 1
	End Function
	
	Private Function ExecDrawWidthCmd() As Integer
		If ArgNum <> 2 Then
			EventErrorMessage = "DrawWidthコマンドの引数の数が違います"
			Error(0)
		End If
		
		ObjDrawWidth = GetArgAsLong(2)
		
		ExecDrawWidthCmd = LineNum + 1
	End Function
	
	Private Function ExecEnableCmd() As Integer
		Dim vname, aname, uname As String
		Dim u As Unit
		
		Select Case ArgNum
			Case 2
				aname = GetArgAsString(2)
			Case 3
				uname = GetArgAsString(2)
				aname = GetArgAsString(3)
			Case Else
				EventErrorMessage = "Enableコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If uname <> "" Then
			vname = "Disable(" & uname & "," & aname & ")"
		Else
			vname = "Disable(" & aname & ")"
		End If
		
		'Disable用変数を削除
		If IsGlobalVariableDefined(vname) Then
			UndefineVariable(vname)
		Else
			'既に設定済みであればそのまま終了
			ExecEnableCmd = LineNum + 1
			Exit Function
		End If
		
		'ユニットのステータスを更新
		If uname <> "" Then
			With UList
				If .IsDefined(uname) Then
					.Item(uname).CurrentForm.Update()
				End If
			End With
		Else
			For	Each u In UList
				With u
					If .Status_Renamed = "出撃" Then
						.Update()
					End If
				End With
			Next u
		End If
		
		ExecEnableCmd = LineNum + 1
	End Function
	
	Private Function ExecEquipCmd() As Integer
		Dim u As Unit
		Dim iname As String
		Dim itm As Item
		Dim i As Short
		
		Select Case ArgNum
			Case 3
				u = GetArgAsUnit(2)
				iname = GetArgAsString(3)
			Case 2
				u = SelectedUnitForEvent
				iname = GetArgAsString(2)
			Case Else
				EventErrorMessage = "Equipコマンドの引数の数が違います"
				Error(0)
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
					For	Each itm In IList
						With itm
							If .Name = iname And .Unit_Renamed Is Nothing And .Exist Then
								GoTo EquipItem
							End If
						End With
					Next itm
					'なかったら装備されているものを…
					For	Each itm In IList
						With itm
							If .Name = iname And Not .Unit_Renamed Is Nothing And .Exist Then
								If .Unit_Renamed.Party0 = "味方" Then
									GoTo EquipItem
								End If
							End If
						End With
					Next itm
					'それでもなければ新たに作成
					itm = IList.Add(iname)
				Else
					itm = IList.Add(iname)
				End If
			Else
				'アイテムＩＤで指定した場合
				itm = IList.Item(iname)
			End If
		ElseIf IDList.IsDefined(iname) Then 
			itm = IList.Add(iname)
		Else
			EventErrorMessage = "「" & iname & "」というアイテムは存在しません"
			Error(0)
		End If
		
EquipItem: 
		'アイテムを装備
		Dim ubitmap As String
		Dim rank_lv, cmd_lv, support_lv As Short
		If Not itm Is Nothing Then
			With itm
				If .Exist Then
					If Not .Unit_Renamed Is Nothing Then
						.Unit_Renamed.DeleteItem(.ID)
					End If
					
					With u
						
						ubitmap = .Bitmap
						If .CountPilot > 0 Then
							With .MainPilot
								cmd_lv = .SkillLevel("指揮")
								rank_lv = .SkillLevel("階級")
								support_lv = .SkillLevel("広域サポート")
							End With
						End If
						
						.AddItem(itm)
						
						'ユニット画像が変化した？
						If ubitmap <> .Bitmap Then
							.BitmapID = MakeUnitBitmap(u)
							For i = 1 To .CountOtherForm
								.OtherForm(i).BitmapID = 0
							Next 
							If .Status_Renamed = "出撃" Then
								If Not IsPictureVisible And MapFileName <> "" Then
									PaintUnitBitmap(u)
								End If
							End If
						End If
						
						'支援効果が変化した？
						If .CountPilot > 0 Then
							With .MainPilot
								If cmd_lv <> .SkillLevel("指揮") Or rank_lv <> .SkillLevel("階級") Or support_lv <> .SkillLevel("広域サポート") Then
									If u.Status_Renamed = "出撃" Then
										PList.UpdateSupportMod(u)
									End If
								End If
							End With
						End If
						
						'最大弾数が変化した？
						If itm.IsFeatureAvailable("最大弾数増加") Then
							.FullSupply()
						End If
					End With
				End If
			End With
		End If
		
		ExecEquipCmd = LineNum + 1
	End Function
	
	Private Function ExecEscapeCmd() As Integer
		Dim pname, uparty As String
		Dim u As Unit
		Dim i, num As Short
		Dim opt As String
		Dim ucount As Short
		
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
				If pname = "味方" Or pname = "ＮＰＣ" Or pname = "敵" Or pname = "中立" Then
					uparty = pname
					For	Each u In UList
						With u
							If .Party0 = uparty Then
								If .Status_Renamed = "出撃" Then
									.Escape(opt)
									ucount = ucount + 1
								ElseIf .Status_Renamed = "破壊" Then 
									If 1 <= .X And .X <= MapWidth And 1 <= .Y And .Y <= MapHeight Then
										If u Is MapDataForUnit(.X, .Y) Then
											'破壊キャンセルで画面上に残っていた
											.Escape(opt)
										End If
									End If
								End If
							End If
						End With
					Next u
				Else
					u = UList.Item2(pname)
					If u Is Nothing Then
						With PList
							If Not .IsDefined(pname) Then
								EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
								Error(0)
							End If
							u = .Item(pname).Unit_Renamed
						End With
					End If
					If Not u Is Nothing Then
						With u
							If .Status_Renamed = "出撃" Then
								ucount = 1
							End If
							.Escape(opt)
							uparty = .Party0
						End With
					End If
				End If
			Case 1
				With SelectedUnitForEvent
					If .Status_Renamed = "出撃" Then
						ucount = 1
					End If
					.Escape(opt)
					uparty = .Party0
				End With
			Case Else
				EventErrorMessage = "Escapeコマンドの引数の数が違います"
				Error(0)
		End Select
		
		'Escapeコマンドによって全滅したかを判定
		If uparty <> "ＮＰＣ" And uparty <> "味方" And ucount > 0 Then
			For	Each u In UList
				With u
					If .Party0 = uparty And (.Status_Renamed = "出撃" Or .Status_Renamed = "格納") And Not .IsConditionSatisfied("憑依") Then
						ExecEscapeCmd = LineNum + 1
						Exit Function
					End If
				End With
			Next u
			
			'戦闘時以外のイベント中の撤退は無視
			For i = 1 To UBound(EventQue)
				If EventQue(i) = "プロローグ" Or EventQue(i) = "エピローグ" Or EventQue(i) = "スタート" Or LIndex(EventQue(i), 1) = "マップ攻撃破壊" Then
					ExecEscapeCmd = LineNum + 1
					Exit Function
				End If
			Next 
			
			'後で全滅イベントを実行
			RegisterEvent("全滅", uparty)
		End If
		
		ExecEscapeCmd = LineNum + 1
	End Function
	
	Private Function ExecExecCmd() As Integer
		Dim fname, opt As String
		Dim msg As String
		Dim i, n, j As Short
		Dim p As Pilot
		Dim plevel As Short
		Dim u As Unit
		
		Select Case ArgNum
			Case 2
				fname = GetArgAsString(2)
			Case 3
				fname = GetArgAsString(2)
				opt = GetArgAsString(3)
			Case Else
				EventErrorMessage = "Execコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ClearUnitStatus()
		
		'追加経験値を得るパイロットや破壊されたユニットがいなければ処理をスキップ
		n = 0
		For	Each u In UList
			With u
				If .Party0 = "味方" Then
					If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Or .Status_Renamed = "破壊" Then
						n = 1
						Exit For
					End If
				End If
			End With
		Next u
		If n = 0 Then
			Turn = 0
		End If
		
		'追加経験値を収得
		If Turn > 0 And Not IsOptionDefined("追加経験値無効") Then
			OpenMessageForm()
			
			n = 0
			msg = ""
			For	Each p In PList
				With p
					If .Party <> "味方" Then
						GoTo NextPilot
					End If
					
					If .MaxSP = 0 Then
						GoTo NextPilot
					End If
					
					If .Unit_Renamed Is Nothing Then
						GoTo NextPilot
					End If
					
					If .Unit_Renamed.Status_Renamed <> "出撃" And .Unit_Renamed.Status_Renamed <> "格納" Then
						GoTo NextPilot
					End If
					
					plevel = .Level
					.Exp = .Exp + 2 * .SP
					
					'追加パイロットや暴走時パイロットに関する処理
					If .Unit_Renamed.CountPilot > 0 And Not .IsSupport(.Unit_Renamed) Then
						'追加パイロットがメインパイロットの場合
						If p Is .Unit_Renamed.Pilot(1) And Not p Is .Unit_Renamed.MainPilot And .Unit_Renamed.MainPilot.MaxSP > 0 Then
							GoTo NextPilot
						End If
						
						'追加パイロットがメインパイロットではなくなった場合
						If Not p Is .Unit_Renamed.MainPilot Then
							'自分がユニットのパイロット一覧に含まれているか判定
							For i = 1 To .Unit_Renamed.CountPilot
								If p Is .Unit_Renamed.Pilot(i) Then
									Exit For
								End If
							Next 
							If i > .Unit_Renamed.CountPilot Then
								GoTo NextPilot
							End If
						End If
					End If
					
					If plevel = .Level Then
						msg = msg & ";" & .Nickname & " 経験値 +" & VB6.Format(2 * .SP)
					Else
						msg = msg & ";" & .Nickname & " 経験値 +" & VB6.Format(2 * .SP) & " レベルアップ！（Lv" & VB6.Format(.Level) & "）"
					End If
					n = n + 1
					If n = 4 Then
						DisplayMessage("システム", Mid(msg, 2))
						msg = ""
						n = 0
					End If
				End With
NextPilot: 
			Next p
			If n > 0 Then
				DisplayMessage("システム", Mid(msg, 2))
			End If
			
			CloseMessageForm()
		End If
		
		MainForm.Hide()
		
		'エピローグイベントを実行
		If IsEventDefined("エピローグ") Then
			'ハイパーモードや変身、能力コピーを解除
			For	Each u In UList
				With u
					If .Status_Renamed <> "他形態" And .Status_Renamed <> "旧主形態" And .Status_Renamed <> "旧形態" Then
						If .IsFeatureAvailable("ノーマルモード") Then
							.Transform(LIndex(.FeatureData("ノーマルモード"), 1))
						End If
					End If
				End With
			Next u
			
			If IsEventDefined("エピローグ", True) Then
				StopBGM()
				StartBGM(BGMName("Briefing"))
			End If
			
			Stage = "エピローグ"
			HandleEvent("エピローグ")
		End If
		
		MainForm.Hide()
		
		'マップをクリア
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				MapDataForUnit(i, j) = Nothing
			Next 
		Next 
		
		'各種データをアップデート
		UList.Update()
		PList.Update()
		IList.Update()
		ClearEventData()
		ClearMap()
		
		'通常ステージとして実行する？
		If opt = "通常ステージ" Then
			IsSubStage = False
		Else
			IsSubStage = True
		End If
		
		'イベントファイルを実行
		StartScenario(fname)
		
		IsScenarioFinished = True
		
		ExecExecCmd = 0
	End Function
	
	Private Function ExecExitCmd() As Integer
		ExecExitCmd = 0
	End Function
	
	Private Function ExecExchangeItemCmd() As Integer
		Dim u As Unit
		Dim ipart As String
		
		Select Case ArgNum
			Case 1
				u = SelectedUnitForEvent
			Case 2
				u = GetArgAsUnit(2)
			Case 3
				u = GetArgAsUnit(2)
				ipart = GetArgAsString(3)
			Case Else
				EventErrorMessage = "ExchangeItemコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExchangeItemCommand(u, ipart)
		
		ExecExchangeItemCmd = LineNum + 1
	End Function
	
	Private Function ExecExplodeCmd() As Integer
		Dim esize As String
		Dim tx, ty As Short
		
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
				Error(0)
		End Select
		
		'爆発の表示
		ExplodeAnimation(esize, tx, ty)
		
		ExecExplodeCmd = LineNum + 1
	End Function
	
	Private Function ExecExpUpCmd() As Integer
		Dim pname As String
		Dim p As Pilot
		Dim prev_lv As Short
		Dim hp_ratio, en_ratio As Double
		Dim num As Short
		
		Select Case ArgNum
			Case 3
				p = GetArgAsPilot(2)
				num = GetArgAsLong(3)
				
			Case 2
				With SelectedUnitForEvent
					If .CountPilot > 0 Then
						p = .Pilot(1)
					Else
						ExecExpUpCmd = LineNum + 1
						Exit Function
					End If
				End With
				num = GetArgAsLong(2)
				
			Case Else
				EventErrorMessage = "ExpUpコマンドの引数の数が違います"
				Error(0)
		End Select
		
		With p
			If Not .Unit_Renamed Is Nothing Then
				With .Unit_Renamed
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
			
			.Update()
			
			'ＳＰ＆霊力をアップデート
			.SP = .SP
			.Plana = .Plana
			
			If Not .Unit_Renamed Is Nothing Then
				With .Unit_Renamed
					.Update()
					.HP = .MaxHP * hp_ratio / 100
					.EN = .MaxEN * en_ratio / 100
				End With
				PList.UpdateSupportMod(.Unit_Renamed)
			End If
		End With
		
		ExecExpUpCmd = LineNum + 1
	End Function
	
	Private Function ExecFadeInCmd() As Integer
		Dim cur_time, start_time, wait_time As Integer
		Dim i, ret As Integer
		Dim num As Short
		
		If IsRButtonPressed() Then
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			MainForm.picMain(0).Refresh()
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
				Error(0)
		End Select
		
		SaveScreen()
		
		With MainForm
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With .picTmp
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Picture = System.Drawing.Image.FromFile("")
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.width = MainPWidth
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Height = MainPHeight
			End With
			
			' MOD START マージ
			'        ret = BitBlt(.picTmp.hDC, _
			''            0, 0, MapPWidth, MapPHeight, _
			''            .picMain(0).hDC, 0, 0, SRCCOPY)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = GUI.BitBlt(.picTmp.hDC, 0, 0, MainPWidth, MainPHeight, .picMain(0).hDC, 0, 0, SRCCOPY)
			' MOD END マージ
			
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			InitFade(.picMain(0), num)
			
			start_time = timeGetTime()
			wait_time = 50
			For i = 0 To num
				If i Mod 4 = 0 Then
					If IsRButtonPressed() Then
						Exit For
					End If
				End If
				
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				DoFade(.picMain(0), i)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Refresh()
				
				cur_time = timeGetTime()
				Do While cur_time < start_time + wait_time * (i + 1)
					System.Windows.Forms.Application.DoEvents()
					cur_time = timeGetTime()
				Loop 
			Next 
			
			FinishFade()
			
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = GUI.BitBlt(.picMain(0).hDC, 0, 0, MapPWidth, MapPHeight, .picTmp.hDC, 0, 0, SRCCOPY)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picMain(0).Refresh()
			
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With .picTmp
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Picture = System.Drawing.Image.FromFile("")
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.width = 32
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Height = 32
			End With
		End With
		
		ExecFadeInCmd = LineNum + 1
	End Function
	
	Private Function ExecFadeOutCmd() As Integer
		Dim cur_time, start_time, wait_time As Integer
		Dim i, ret As Integer
		Dim num As Short
		
		Select Case ArgNum
			Case 1
				num = 10
			Case 2
				num = GetArgAsLong(2)
			Case Else
				EventErrorMessage = "FadeOutコマンドの引数の数が違います"
				Error(0)
		End Select
		
		SaveScreen()
		
		With MainForm
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			InitFade(.picMain(0), num)
			
			start_time = timeGetTime()
			wait_time = 50
			For i = 0 To num
				If i Mod 4 = 0 Then
					If IsRButtonPressed() Then
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						With .picMain(0)
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = PatBlt(.hDC, 0, 0, .width, .Height, BLACKNESS)
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.Refresh()
						End With
						Exit For
					End If
				End If
				
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				DoFade(.picMain(0), num - i)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Refresh()
				
				cur_time = timeGetTime()
				Do While cur_time < start_time + wait_time * (i + 1)
					System.Windows.Forms.Application.DoEvents()
					cur_time = timeGetTime()
				Loop 
			Next 
			
			FinishFade()
		End With
		
		IsPictureVisible = True
		PaintedAreaX1 = MainPWidth
		PaintedAreaY1 = MainPHeight
		PaintedAreaX2 = -1
		PaintedAreaY2 = -1
		
		ExecFadeOutCmd = LineNum + 1
	End Function
	
	Private Function ExecFillColorCmd() As Integer
		Dim opt, cname As String
		
		If ArgNum <> 2 Then
			EventErrorMessage = "FillColorコマンドの引数の数が違います"
			Error(0)
		End If
		
		opt = GetArgAsString(2)
		If Asc(opt) <> 35 Or Len(opt) <> 7 Then
			EventErrorMessage = "色指定が不正です"
			Error(0)
		End If
		cname = New String(vbNullChar, 8)
		Mid(cname, 1, 2) = "&H"
		Mid(cname, 3, 2) = Mid(opt, 6, 2)
		Mid(cname, 5, 2) = Mid(opt, 4, 2)
		Mid(cname, 7, 2) = Mid(opt, 2, 2)
		If Not IsNumeric(cname) Then
			EventErrorMessage = "色指定が不正です"
			Error(0)
		End If
		
		ObjFillColor = CInt(cname)
		
		ExecFillColorCmd = LineNum + 1
	End Function
	
	Private Function ExecFillStyleCmd() As Integer
		If ArgNum <> 2 Then
			EventErrorMessage = "FillStyleコマンドの引数の数が違います"
			Error(0)
		End If
		
		Select Case GetArgAsString(2)
			Case "塗りつぶし"
				'UPGRADE_ISSUE: 定数 vbFSSolid はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				ObjFillStyle = vbFSSolid
			Case "透明"
				'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				ObjFillStyle = vbFSTransparent
			Case "横線"
				'UPGRADE_ISSUE: 定数 vbHorizontalLine はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				ObjFillStyle = vbHorizontalLine
			Case "縦線"
				'UPGRADE_ISSUE: 定数 vbVerticalLine はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				ObjFillStyle = vbVerticalLine
			Case "斜線"
				'UPGRADE_ISSUE: 定数 vbUpwardDiagonal はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				ObjFillStyle = vbUpwardDiagonal
			Case "斜線２"
				'UPGRADE_ISSUE: 定数 vbDownwardDiagonal はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				ObjFillStyle = vbDownwardDiagonal
			Case "クロス"
				'UPGRADE_ISSUE: 定数 vbCross はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				ObjFillStyle = vbCross
			Case "網かけ"
				'UPGRADE_ISSUE: 定数 vbDiagonalCross はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				ObjFillStyle = vbDiagonalCross
			Case Else
				EventErrorMessage = "背景描画方法の指定が不正です"
				Error(0)
		End Select
		
		ExecFillStyleCmd = LineNum + 1
	End Function
	
	Private Function ExecFinishCmd() As Integer
		Dim u As Unit
		
		Select Case ArgNum
			Case 2
				u = GetArgAsUnit(2, True)
			Case 1
				u = SelectedUnitForEvent
			Case Else
				EventErrorMessage = "Finishコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not u Is Nothing Then
			With u
				Select Case .Action
					Case 1
						.UseAction()
						If .Status_Renamed = "出撃" Then
							PaintUnitBitmap(u)
						End If
					Case 0
						'なにもしない
					Case Else
						.UseAction()
				End Select
			End With
		End If
		
		ExecFinishCmd = LineNum + 1
	End Function
	
	Private Function ExecFixCmd() As Integer
		Dim buf As String
		
		Select Case ArgNum
			Case 1
				buf = SelectedUnitForEvent.Pilot(1).Name
			Case 2
				buf = GetArgAsString(2)
				If Not PList.IsDefined(buf) And Not IList.IsDefined(buf) Then
					EventErrorMessage = "パイロット名またはアイテム名" & buf & "が間違っています"
					Error(0)
				End If
				If PList.IsDefined(buf) Then
					buf = PList.Item(buf).Name
				Else
					buf = IList.Item(buf).Name
				End If
			Case Else
				EventErrorMessage = "Fixコマンドの引数の数が違います"
				Error(0)
		End Select
		
		buf = "Fix(" & buf & ")"
		If Not IsGlobalVariableDefined(buf) Then
			DefineGlobalVariable(buf)
		End If
		SetVariableAsLong(buf, 1)
		
		ExecFixCmd = LineNum + 1
	End Function
	
	Private Function ExecFontCmd() As Integer
		Dim cname, opt, fname As String
		Dim i As Short
		Dim sf As System.Drawing.Font
		
		ExecFontCmd = LineNum + 1
		
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMain(0)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			fname = .Font.Name
			
			'デフォルトの設定
			If ArgNum = 1 Then
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.ForeColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				With .Font
					fname = "ＭＳ Ｐ明朝"
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Size = 16
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Bold = True
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.Font.Bold = True
						Case "Italic"
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.Font.Italic = True
						Case "Regular"
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.Font.Bold = False
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
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
							If Right(opt, 2) = "pt" Then
								'文字サイズ
								opt = Left(opt, Len(opt) - 2)
								'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
								.Font.Size = CShort(opt)
							ElseIf Asc(opt) = 35 And Len(opt) = 7 Then 
								'文字色
								cname = New String(vbNullChar, 8)
								Mid(cname, 1, 2) = "&H"
								Mid(cname, 3, 2) = Mid(opt, 6, 2)
								Mid(cname, 5, 2) = Mid(opt, 4, 2)
								Mid(cname, 7, 2) = Mid(opt, 2, 2)
								If IsNumeric(cname) Then
									'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
									.ForeColor = CInt(cname)
								End If
							Else
								'その他のフォント
								fname = opt
							End If
					End Select
				Next 
			End If
			
			'フォント名が変更されている？
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			If fname <> .Font.Name Then
				sf = System.Windows.Forms.Control.DefaultFont.Clone()
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				With .Font
					sf = VB6.FontChangeName(sf, fname)
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					sf = VB6.FontChangeSize(sf, .Size)
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					sf = VB6.FontChangeBold(sf, .Bold)
					'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					sf = VB6.FontChangeItalic(sf, .Italic)
				End With
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Font = sf
			End If
		End With
	End Function
	
	Private Function ExecForCmd() As Integer
		Dim vname As String
		Dim idx, i, limit As Integer
		Dim depth As Short
		Dim isincr As Short
		
		If ArgNum <> 6 And ArgNum <> 8 Then
			EventErrorMessage = "Forコマンドの引数の数が違います"
			Error(0)
		End If
		
		'インデックス変数に初期値を設定
		vname = GetArg(2)
		idx = GetArgAsLong(4)
		SetVariableAsLong(vname, idx)
		
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
					Case Event_Renamed.CmdType.ForCmd, Event_Renamed.CmdType.ForEachCmd
						depth = depth + 1
					Case Event_Renamed.CmdType.NextCmd
						depth = depth - 1
						If depth = 0 Then
							ExecForCmd = i + 1
							Exit Function
						End If
				End Select
			Next 
			
			EventErrorMessage = "ForまたはForEachとNextが対応していません"
			Error(0)
		End If
	End Function
	
	Private Function ExecForEachCmd() As Integer
		Dim uparty As String
		Dim ustatus As String
		Dim ugroup As String
		Dim u As Unit
		Dim p As Pilot
		Dim i As Integer
		Dim j, depth As Short
		Dim vname, aname As String
		Dim buf As String
		Dim var As VarData
		Dim key_type As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_value As Integer
		Dim max_str As String
		Dim max_item As Short
		
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
							For	Each u In UList
								With u
									If .Status_Renamed <> "他形態" And .Status_Renamed <> "旧主形態" And .Status_Renamed <> "旧形態" And .Status_Renamed <> "破棄" Then
										ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
										ForEachSet(UBound(ForEachSet)) = .ID
									End If
								End With
							Next u
						Else
							For	Each u In UList
								With u
									If InStr(ustatus, .Status_Renamed) > 0 Then
										ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
										ForEachSet(UBound(ForEachSet)) = .ID
									End If
								End With
							Next u
						End If
					Case "味方", "ＮＰＣ", "敵", "中立"
						uparty = GetArgAsString(2)
						If ustatus = "全" Then
							For	Each u In UList
								With u
									If .Party0 = uparty Then
										If .Status_Renamed <> "他形態" And .Status_Renamed <> "旧主形態" And .Status_Renamed <> "旧形態" And .Status_Renamed <> "破棄" Then
											ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
											ForEachSet(UBound(ForEachSet)) = .ID
										End If
									End If
								End With
							Next u
						Else
							For	Each u In UList
								With u
									If .Party0 = uparty Then
										If InStr(ustatus, .Status_Renamed) > 0 Then
											ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
											ForEachSet(UBound(ForEachSet)) = .ID
										End If
									End If
								End With
							Next u
						End If
					Case Else
						ugroup = GetArgAsString(2)
						If ustatus = "全て" Then
							ustatus = "全"
						End If
						For	Each u In UList
							With u
								If .CountPilot > 0 Then
									If .MainPilot.ID = ugroup Or InStr(.MainPilot.ID, ugroup & ":") = 1 Then
										If ustatus = "全" Then
											If .Status_Renamed <> "他形態" And .Status_Renamed <> "旧主形態" And .Status_Renamed <> "旧形態" And .Status_Renamed <> "破棄" Then
												ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
												ForEachSet(UBound(ForEachSet)) = .ID
											End If
										Else
											If InStr(ustatus, .Status_Renamed) > 0 Then
												ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
												ForEachSet(UBound(ForEachSet)) = .ID
											End If
										End If
									End If
								End If
							End With
						Next u
				End Select
				
				'配列の要素に対するForEach
			Case 4
				'インデックス用変数名
				vname = GetArg(2)
				If Left(vname, 1) = "$" Then
					vname = Mid(vname, 2)
				End If
				
				'配列の変数名
				aname = GetArg(4)
				If Left(aname, 1) = "$" Then
					aname = Mid(aname, 2)
				End If
				'Eval関数
				If LCase(Left(aname, 5)) = "eval(" Then
					If Right(aname, 1) = ")" Then
						aname = Mid(aname, 6, Len(aname) - 6)
						aname = GetValueAsString(aname)
					End If
				End If
				
				'配列を検索し、配列要素を見つける
				If InStrNotNest(aname, "パイロット一覧(") = 1 Then
					key_type = Mid(aname, InStrNotNest(aname, "(") + 1, Len(aname) - InStrNotNest(aname, "(") - 1)
					key_type = GetValueAsString(key_type)
					
					If key_type <> "名称" Then
						'配列作成
						ReDim ForEachSet(PList.Count)
						ReDim key_list(PList.Count)
						i = 0
						For	Each p In PList
							With p
								If Not .Alive Or .Away Then
									GoTo NextPilot1
								End If
								
								If Not .Unit_Renamed Is Nothing Then
									With .Unit_Renamed
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
										key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
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
						Next p
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
						For	Each p In PList
							With p
								If Not .Alive Or .Away Then
									GoTo NextPilot2
								End If
								
								If Not .Unit_Renamed Is Nothing Then
									With .Unit_Renamed
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
						Next p
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
					key_type = Mid(aname, InStrNotNest(aname, "(") + 1, Len(aname) - InStrNotNest(aname, "(") - 1)
					key_type = GetValueAsString(key_type)
					
					If key_type <> "名称" Then
						'配列作成
						ReDim ForEachSet(UList.Count)
						ReDim key_list(UList.Count)
						i = 0
						For	Each u In UList
							With u
								If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Or .Status_Renamed = "待機" Then
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
												If .IsWeaponMastered(j) And Not .IsDisabled((.Weapon(j).Name)) And Not .IsWeaponClassifiedAs(j, "合") Then
													If .WeaponPower(j, "") > key_list(i) Then
														key_list(i) = .WeaponPower(j, "")
													End If
												End If
											Next 
										Case "最長射程"
											For j = 1 To .CountWeapon
												If .IsWeaponMastered(j) And Not .IsDisabled((.Weapon(j).Name)) And Not .IsWeaponClassifiedAs(j, "合") Then
													If .WeaponMaxRange(j) > key_list(i) Then
														key_list(i) = .WeaponMaxRange(j)
													End If
												End If
											Next 
									End Select
								End If
							End With
						Next u
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
						For	Each u In UList
							With u
								If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Or .Status_Renamed = "待機" Then
									i = i + 1
									ForEachSet(i) = .ID
									strkey_list(i) = .KanaName
								End If
							End With
						Next u
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
									If Mid(buf, Len(buf) - j + 1, 1) = "]" Then
										Exit For
									End If
								Next 
								buf = Mid(buf, InStr(buf, "[") + 1)
								buf = Left(buf, Len(buf) - j)
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
					For	Each var In LocalVariableList
						With var
							If InStr(.Name, aname & "[") = 1 Then
								ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
								buf = .Name
								For i = 1 To Len(buf)
									If Mid(buf, Len(buf) - i + 1, 1) = "]" Then
										Exit For
									End If
								Next 
								buf = Mid(buf, InStr(buf, "[") + 1)
								buf = Left(buf, Len(buf) - i)
								ForEachSet(UBound(ForEachSet)) = buf
							End If
						End With
					Next var
					If UBound(ForEachSet) = 0 Then
						buf = GetValueAsString(aname)
						ReDim ForEachSet(ListLength(buf))
						For i = 1 To ListLength(buf)
							ForEachSet(i) = ListIndex(buf, i)
						Next 
					End If
				ElseIf IsGlobalVariableDefined(aname) Then 
					'グローバルな配列に対するForEach
					For	Each var In GlobalVariableList
						With var
							If InStr(.Name, aname & "[") = 1 Then
								ReDim Preserve ForEachSet(UBound(ForEachSet) + 1)
								buf = .Name
								For i = 1 To Len(buf)
									If Mid(buf, Len(buf) - i + 1, 1) = "]" Then
										Exit For
									End If
								Next 
								buf = Mid(buf, InStr(buf, "[") + 1)
								buf = Left(buf, Len(buf) - i)
								ForEachSet(UBound(ForEachSet)) = buf
							End If
						End With
					Next var
					If UBound(ForEachSet) = 0 Then
						buf = GetValueAsString(aname)
						ReDim ForEachSet(ListLength(buf))
						For i = 1 To ListLength(buf)
							ForEachSet(i) = ListIndex(buf, i)
						Next 
					End If
				ElseIf (Left(aname, 1) = "(" And Right(aname, 1) = ")") Or (Left(aname, 1) = """" And Right(aname, 1) = """") Or (Left(aname, 1) = "`" And Right(aname, 1) = "`") Or (InStr(LCase(aname), "list(") = 1 And Right(aname, 1) = ")") Then 
					'リストに対するForEach
					buf = GetValueAsString(aname)
					ReDim ForEachSet(ListLength(buf))
					For i = 1 To ListLength(buf)
						ForEachSet(i) = ListIndex(buf, i)
					Next 
				End If
				
			Case Else
				EventErrorMessage = "ForEachコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If UBound(ForEachSet) > 0 Then
			'ForEachの実行要素がある場合
			
			ForEachIndex = 1
			ForIndex = ForIndex + 1
			
			If ArgNum < 4 Then
				SelectedUnitForEvent = UList.Item(ForEachSet(1))
			Else
				SetVariableAsString(GetArg(2), ForEachSet(1))
			End If
			ExecForEachCmd = LineNum + 1
		Else
			'ForEachの実行要素がない場合
			
			'対応するNextを探す
			depth = 1
			For i = LineNum + 1 To UBound(EventCmd)
				Select Case EventCmd(i).Name
					Case Event_Renamed.CmdType.ForCmd, Event_Renamed.CmdType.ForEachCmd
						depth = depth + 1
					Case Event_Renamed.CmdType.NextCmd
						depth = depth - 1
						If depth = 0 Then
							ExecForEachCmd = i + 1
							Exit Function
						End If
				End Select
			Next 
			
			EventErrorMessage = "ForまたはForEachとNextが対応していません"
			Error(0)
		End If
	End Function
	
	Private Function ExecNextCmd() As Integer
		Dim i As Integer
		Dim depth As Short
		Dim idx As Double
		Dim vname, buf As String
		Dim isincr As Short
		
		'対応するForまたはForEachを探す
		i = LineNum
		depth = 1
		Do While i > 1
			i = i - 1
			With EventCmd(i)
				Select Case .Name
					Case Event_Renamed.CmdType.ForCmd
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
							SetVariableAsDouble(vname, idx)
							
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
					Case Event_Renamed.CmdType.ForEachCmd
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
									SelectedUnitForEvent = UList.Item(ForEachSet(ForEachIndex))
								Else
									'配列に対するForEach
									SetVariableAsString(.GetArg(2), ForEachSet(ForEachIndex))
								End If
							End If
							ExecNextCmd = i + 1
							Exit Function
						End If
					Case Event_Renamed.CmdType.NextCmd
						depth = depth + 1
				End Select
			End With
		Loop 
		
		EventErrorMessage = "ForまたはForEachとNextが対応していません"
		Error(0)
	End Function
	
	Private Function ExecForgetCmd() As Integer
		Dim tname As String
		Dim i, j As Short
		
		If ArgNum <> 2 Then
			EventErrorMessage = "Forgetコマンドの引数の数が違います"
			Error(0)
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
	
	Private Function ExecFreeMemoryCmd() As Integer
		UList.Clean()
		PList.Clean()
		IList.Update()
		ExecFreeMemoryCmd = LineNum + 1
	End Function
	
	Private Function ExecGameClearCmd() As Integer
		If ArgNum <> 1 Then
			EventErrorMessage = "GameClearコマンドの引数の数が違います"
			Error(0)
		End If
		
		GameClear()
	End Function
	
	Private Function ExecGameOverCmd() As Integer
		If ArgNum <> 1 Then
			EventErrorMessage = "GameOverコマンドの引数の数が違います"
			Error(0)
		End If
		
		GameOver()
		IsScenarioFinished = True
		ExecGameOverCmd = 0
	End Function
	
	Private Function ExecGetOffCmd() As Integer
		Dim u As Unit
		
		Select Case ArgNum
			Case 1
				u = SelectedUnitForEvent
			Case 2
				u = GetArgAsUnit(2, True)
			Case Else
				EventErrorMessage = "GetOffコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not u Is Nothing Then
			With u
				If .CountPilot > 0 Then
					If .Status_Renamed = "出撃" Then
						'ユニットをマップ上から削除した状態で支援効果を更新
						'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
						MapDataForUnit(.X, .Y) = Nothing
						PList.UpdateSupportMod(u)
					End If
					
					'パイロットを下ろす
					.Pilot(1).GetOff(True)
					
					If .Status_Renamed = "出撃" Then
						'ユニットをマップ上に戻す
						MapDataForUnit(.X, .Y) = u
					End If
				End If
			End With
		End If
		
		ExecGetOffCmd = LineNum + 1
	End Function
	
	Private Function ExecGlobalCmd() As Integer
		Dim vname As String
		Dim i As Short
		
		For i = 2 To ArgNum
			vname = GetArg(i)
			If InStr(vname, """") > 0 Then
				EventErrorMessage = "変数名「" & vname & "」が不正です"
				Error(0)
			End If
			If Asc(vname) = 36 Then '$
				vname = Mid(vname, 2)
			End If
			
			If Not IsGlobalVariableDefined(vname) Then
				DefineGlobalVariable(vname)
			End If
		Next 
		
		ExecGlobalCmd = LineNum + 1
	End Function
	
	Private Function ExecGotoCmd() As Integer
		If ArgNum <> 2 Then
			EventErrorMessage = "Gotoコマンドの引数の数が違います"
			Error(0)
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
			EventErrorMessage = "ラベル「" & GetArg(2) & "」がみつかりません"
			Error(0)
		End If
		
		ExecGotoCmd = ExecGotoCmd + 1
	End Function
	
	Private Function ExecHideCmd() As Integer
		MainForm.Hide()
		
		ExecHideCmd = LineNum + 1
	End Function
	
	Private Function ExecHotPointCmd() As Integer
		Dim hname, hcaption As String
		Dim hx, hy As Short
		Dim hw, hh As Short
		
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
				Error(0)
		End Select
		
		ReDim Preserve HotPointList(UBound(HotPointList) + 1)
		With HotPointList(UBound(HotPointList))
			.Name = hname
			.Left_Renamed = hx
			.Top = hy
			.width = hw
			.Height = hh
			.Caption = hcaption
		End With
		
		ExecHotPointCmd = LineNum + 1
	End Function
	
	Private Function ExecIfCmd() As Integer
		Dim expr As String
		Dim i As Integer
		Dim depth As Short
		Dim pname As String
		Dim flag As Boolean
		Dim ret As Integer
		
		expr = GetArg(2)
		
		'Ifコマンドはあらかじめ構文解析されていて、第3引数に条件式の項数
		'が入っている
		Select Case GetArgAsLong(3)
			Case 1
				If PList.IsDefined(expr) Then
					With PList.Item(expr)
						If .Unit_Renamed Is Nothing Then
							flag = False
						Else
							With .Unit_Renamed
								If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
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
						If .Unit_Renamed Is Nothing Then
							flag = True
						Else
							With .Unit_Renamed
								If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
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
							EventErrorMessage = "ラベル「" & GetArg(5) & "」がみつかりません"
							Error(0)
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
							Case Event_Renamed.CmdType.IfCmd
								If .GetArg(4) = "then" Then
									depth = depth + 1
								End If
							Case Event_Renamed.CmdType.ElseCmd
								If depth = 1 Then
									Exit For
								End If
							Case Event_Renamed.CmdType.ElseIfCmd
								If depth <> 1 Then
									GoTo NextLoop
								End If
								'条件式が成り立つか判定
								expr = .GetArg(2)
								Select Case .GetArgAsLong(3)
									Case 1
										If PList.IsDefined(expr) Then
											With PList.Item(expr)
												If .Unit_Renamed Is Nothing Then
													flag = False
												Else
													With .Unit_Renamed
														If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
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
												If .Unit_Renamed Is Nothing Then
													flag = True
												Else
													With .Unit_Renamed
														If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
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
							Case Event_Renamed.CmdType.EndIfCmd
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
					Error(0)
				End If
				
				ExecIfCmd = i + 1
				
			Case Else
				EventErrorMessage = "If行には Goto, Exit, Then のいずれかを指定して下さい"
				Error(0)
		End Select
	End Function
	
	Private Function ExecElseCmd() As Integer
		Dim i As Integer
		Dim depth As Short
		
		'EndIfを探す
		depth = 1
		For i = LineNum + 1 To UBound(EventCmd)
			With EventCmd(i)
				Select Case .Name
					Case Event_Renamed.CmdType.IfCmd
						If .GetArg(4) = "then" Then
							depth = depth + 1
						End If
					Case Event_Renamed.CmdType.EndIfCmd
						depth = depth - 1
						If depth = 0 Then
							ExecElseCmd = i + 1
							Exit Function
						End If
				End Select
			End With
		Next 
		
		EventErrorMessage = "IfとEndIfが対応していません"
		Error(0)
	End Function
	
	Private Function ExecIncrCmd() As Integer
		Dim vname, buf As String
		Dim num As Double
		
		vname = GetArg(2)
		GetVariable(vname, Expression.ValueType.NumericType, buf, num)
		Select Case ArgNum
			Case 3
				SetVariableAsDouble(vname, num + GetArgAsDouble(3))
			Case 2
				SetVariableAsDouble(vname, num + 1)
			Case Else
				EventErrorMessage = "Incrコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExecIncrCmd = LineNum + 1
	End Function
	
	Private Function ExecIncreaseMoraleCmd() As Integer
		Dim u As Unit
		Dim num As String
		
		Select Case ArgNum
			Case 3
				u = GetArgAsUnit(2, True)
				num = CStr(GetArgAsLong(3))
			Case 2
				u = SelectedUnitForEvent
				num = CStr(GetArgAsLong(2))
			Case Else
				EventErrorMessage = "IncreaseMoraleコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not u Is Nothing Then
			With u
				.IncreaseMorale(CShort(num), True)
				.CurrentForm.CheckAutoHyperMode()
				.CurrentForm.CheckAutoNormalMode()
			End With
		End If
		
		ExecIncreaseMoraleCmd = LineNum + 1
	End Function
	
	Private Function ExecInputCmd() As Integer
		'UPGRADE_NOTE: str は str_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
		Dim str_Renamed As String
		
		Select Case ArgNum
			Case 3
				str_Renamed = InputBox(GetArgAsString(3), "SRC")
			Case 4
				str_Renamed = InputBox(GetArgAsString(3), "SRC", GetArgAsString(4))
			Case Else
				EventErrorMessage = "Inputコマンドの引数の数が違います"
				Error(0)
		End Select
		
		SetVariableAsString(GetArg(2), str_Renamed)
		
		ExecInputCmd = LineNum + 1
	End Function
	
	' MOD START マージ
	'Private Function ExecInterMissionCommandCmd() As Long
	Private Function ExecIntermissionCommandCmd() As Integer
		' MOD END マージ
		Dim vname As String
		
		If ArgNum <> 3 Then
			' MOD START マージ
			'        EventErrorMessage = "InterMissionCommandコマンドの引数の数が違います"
			EventErrorMessage = "IntermissionCommandコマンドの引数の数が違います"
			' MOD END マージ
			Error(0)
		End If
		
		' MOD START マージ
		'    vname = "InterMissionCommand(" & GetArgAsString(2) & ")"
		vname = "IntermissionCommand(" & GetArgAsString(2) & ")"
		' MOD END マージ
		
		If GetArg(3) = "削除" Then
			UndefineVariable(vname)
		Else
			If Not IsGlobalVariableDefined(vname) Then
				DefineGlobalVariable(vname)
			End If
			SetVariableAsString(vname, GetArgAsString(3))
		End If
		
		' MOD START マージ
		'    ExecInterMissionCommandCmd = LineNum + 1
		ExecIntermissionCommandCmd = LineNum + 1
		' MOD END マージ
	End Function
	
	Private Function ExecItemCmd() As Integer
		Dim iname As String
		
		Select Case ArgNum
			Case 2
				iname = GetArgAsString(2)
			Case Else
				EventErrorMessage = "Itemコマンドの引数の数が違います"
				Error(0)
		End Select
		If Not IDList.IsDefined(iname) Then
			EventErrorMessage = "「" & iname & "」というアイテムは存在しません"
			Error(0)
		End If
		IList.Add(iname)
		
		ExecItemCmd = LineNum + 1
	End Function
	
	Private Function ExecJoinCmd() As Integer
		Dim pname As String
		Dim u As Unit
		Dim i As Short
		
		Select Case ArgNum
			Case 2
				pname = GetArgAsString(2)
				If PList.IsDefined(pname) Then
					u = PList.Item(pname).Unit_Renamed
				ElseIf NPDList.IsDefined(pname) Then 
					pname = "IsAway(" & NPDList.Item(pname).Name & ")"
					If IsGlobalVariableDefined(pname) Then
						UndefineVariable(pname)
					End If
					ExecJoinCmd = LineNum + 1
					Exit Function
				ElseIf UList.IsDefined(pname) Then 
					If pname = UList.Item(pname).ID Then
						'UPGRADE_WARNING: オブジェクト u の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						u = UList.Item(pname)
					Else
						For	Each u In UList
							With u
								If .Name = pname And .Party0 = "味方" And .CurrentForm.Status_Renamed = "離脱" Then
									u = .CurrentForm
									Exit For
								End If
							End With
						Next u
						If u.Name <> pname Then
							'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							u = Nothing
						End If
					End If
				Else
					EventErrorMessage = "「" & pname & "」というパイロットまたはユニットが見つかりません"
					Error(0)
				End If
			Case 1
				u = SelectedUnitForEvent
			Case Else
				EventErrorMessage = "Joinコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If u Is Nothing Then
			If PList.IsDefined(pname) Then
				PList.Item(pname).Away = False
			End If
		Else
			With u
				.Status_Renamed = "待機"
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
	
	Private Function ExecKeepBGMCmd() As Integer
		If ArgNum <> 1 Then
			EventErrorMessage = "KeepBGMコマンドの引数の数が違います"
			Error(0)
		End If
		
		KeepBGM = True
		
		ExecKeepBGMCmd = LineNum + 1
	End Function
	
	Private Function ExecLandCmd() As Integer
		Dim u1, u2 As Unit
		
		Select Case ArgNum
			Case 2
				u1 = SelectedUnitForEvent
				u2 = GetArgAsUnit(2)
			Case 3
				u1 = GetArgAsUnit(2)
				u2 = GetArgAsUnit(3)
			Case Else
				EventErrorMessage = "Landコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If u1.IsFeatureAvailable("母艦") Then
			EventErrorMessage = u1.Name & "は母艦なので格納出来ません"
			Error(0)
		End If
		If Not u2.IsFeatureAvailable("母艦") Then
			EventErrorMessage = u2.Name & "は母艦能力を持っていません"
			Error(0)
		End If
		
		u1.Land(u2, True, True)
		
		ExecLandCmd = LineNum + 1
	End Function
	
	Private Function ExecLaunchCmd() As Integer
		Dim u As Unit
		Dim uy, ux, num As Short
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
				u = SelectedUnitForEvent
				
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
				u = GetArgAsUnit(2)
				
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
				Error(0)
		End Select
		
		If opt <> "非同期" And MainForm.Visible And Not IsPictureVisible Then
			Center(ux, uy)
			RefreshScreen()
		End If
		
		With u
			Select Case .Status_Renamed
				Case "出撃"
					EventErrorMessage = .MainPilot.Nickname & "はすでに出撃しています"
					Error(0)
				Case "離脱"
					EventErrorMessage = .MainPilot.Nickname & "はまだ離脱しています"
					Error(0)
			End Select
			
			.UsedAction = 0
			.UsedSupportAttack = 0
			.UsedSupportGuard = 0
			.UsedSyncAttack = 0
			.UsedCounterAttack = 0
			
			If .HP <= 0 Then
				.HP = 1
			End If
			
			.StandBy(ux, uy, opt)
			
			.CheckAutoHyperMode()
		End With
		
		SelectedUnitForEvent = u.CurrentForm
		
		ExecLaunchCmd = LineNum + 1
	End Function
	
	Private Function ExecLeaveCmd() As Integer
		Dim pname, vname As String
		Dim u As Unit
		Dim i, num As Short
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
					u = PList.Item(pname).Unit_Renamed
				ElseIf NPDList.IsDefined(pname) Then 
					vname = "IsAway(" & NPDList.Item(pname).Name & ")"
					If Not IsGlobalVariableDefined(vname) Then
						DefineGlobalVariable(vname)
					End If
					SetVariableAsLong(vname, 1)
					ExecLeaveCmd = LineNum + 1
					Exit Function
				ElseIf UList.IsDefined(pname) Then 
					If pname = UList.Item(pname).ID Then
						u = UList.Item(pname)
					Else
						For	Each u In UList
							If u.Name = pname And u.Party0 = "味方" And u.CurrentForm.Status_Renamed <> "離脱" Then
								u = u.CurrentForm
								Exit For
							End If
						Next u
						If u.Name <> pname Then
							'UPGRADE_NOTE: オブジェクト u をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							u = Nothing
						End If
					End If
				Else
					EventErrorMessage = "「" & pname & "」というパイロットまたはユニットが見つかりません"
					Error(0)
				End If
			Case 1
				u = SelectedUnitForEvent
			Case Else
				EventErrorMessage = "Leaveコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If u Is Nothing Then
			PList.Item(pname).Away = True
		Else
			With u
				If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
					.Escape(opt)
				End If
				If .Party0 <> "味方" Then
					.ChangeParty("味方")
				End If
				If .Status_Renamed <> "他形態" And .Status_Renamed <> "旧主形態" And .Status_Renamed <> "旧形態" Then
					.Status_Renamed = "離脱"
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
	
	Private Function ExecLevelUpCmd() As Integer
		Dim p As Pilot
		Dim num As Short
		Dim hp_ratio, en_ratio As Double
		
		Select Case ArgNum
			Case 3
				p = GetArgAsPilot(2)
				num = GetArgAsLong(3)
			Case 2
				With SelectedUnitForEvent
					If .CountPilot > 0 Then
						p = .Pilot(1)
					End If
				End With
				num = GetArgAsLong(2)
			Case Else
				EventErrorMessage = "LevelUpコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not p Is Nothing Then
			With p
				If Not .Unit_Renamed Is Nothing Then
					With .Unit_Renamed
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
				
				If Not .Unit_Renamed Is Nothing Then
					With .Unit_Renamed
						.Update()
						.HP = .MaxHP * hp_ratio / 100
						.EN = .MaxEN * en_ratio / 100
					End With
					PList.UpdateSupportMod(.Unit_Renamed)
				End If
			End With
		End If
		
		ExecLevelUpCmd = LineNum + 1
	End Function
	
	Private Function ExecLineCmd() As Integer
		Dim pic, pic2 As System.Windows.Forms.PictureBox
		Dim x1, y1 As Short
		Dim x2, y2 As Short
		Dim opt, dtype As String
		Dim cname As String
		Dim clr As Integer
		Dim i As Short
		
		If ArgNum < 5 Then
			EventErrorMessage = "Lineコマンドの引数の数が違います"
			Error(0)
		End If
		
		x1 = GetArgAsLong(2) + BaseX
		y1 = GetArgAsLong(3) + BaseY
		x2 = GetArgAsLong(4) + BaseX
		y2 = GetArgAsLong(5) + BaseY
		
		SaveScreen()
		
		'描画先
		Select Case ObjDrawOption
			Case "背景"
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picBack
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMaskedBack
				IsMapDirty = True
			Case "保持"
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMain(1)
				IsPictureVisible = True
			Case Else
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
		End Select
		
		'描画領域
		Dim tmp As Short
		If ObjDrawOption <> "背景" Then
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
					Error(0)
				End If
				cname = New String(vbNullChar, 8)
				Mid(cname, 1, 2) = "&H"
				Mid(cname, 3, 2) = Mid(opt, 6, 2)
				Mid(cname, 5, 2) = Mid(opt, 4, 2)
				Mid(cname, 7, 2) = Mid(opt, 2, 2)
				If Not IsNumeric(cname) Then
					EventErrorMessage = "色指定が不正です"
					Error(0)
				End If
				clr = CInt(cname)
			Else
				If opt <> "B" And opt <> "BF" Then
					EventErrorMessage = "Lineコマンドに不正なオプション「" & opt & "」が使われています"
					Error(0)
				End If
				dtype = opt
			End If
		Next 
		
		With pic
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.DrawWidth = ObjDrawWidth
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillColor = ObjFillColor
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillStyle = ObjFillStyle
		End With
		
		Select Case dtype
			Case "B"
				'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				pic.Line (x1, y1) - (x2, y2), clr, B
			Case "BF"
				'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				pic.Line (x1, y1) - (x2, y2), clr, BF
			Case Else
				'UPGRADE_ISSUE: PictureBox メソッド pic.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				pic.Line (x1, y1) - (x2, y2), clr
		End Select
		
		With pic
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.DrawWidth = 1
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
			'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillStyle = vbFSTransparent
		End With
		
		If Not pic2 Is Nothing Then
			With pic2
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.DrawWidth = ObjDrawWidth
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillColor = ObjFillColor
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillStyle = ObjFillStyle
			End With
			
			Select Case dtype
				Case "B"
					'UPGRADE_ISSUE: PictureBox メソッド pic2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					pic2.Line (x1, y1) - (x2, y2), clr, B
				Case "BF"
					'UPGRADE_ISSUE: PictureBox メソッド pic2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					pic2.Line (x1, y1) - (x2, y2), clr, BF
				Case Else
					'UPGRADE_ISSUE: PictureBox メソッド pic2.Line はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
					pic2.Line (x1, y1) - (x2, y2), clr
			End Select
			
			With pic2
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.DrawWidth = 1
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
				'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillStyle = vbFSTransparent
			End With
		End If
		
		ExecLineCmd = LineNum + 1
	End Function
	
	Private Function ExecLineReadCmd() As Integer
		Dim buf As String
		
		If ArgNum <> 3 Then
			EventErrorMessage = "LineReadコマンドの引数の数が違います"
			Error(0)
		End If
		
		buf = LineInput(GetArgAsLong(2))
		SetVariableAsString(GetArg(3), buf)
		
		ExecLineReadCmd = LineNum + 1
	End Function
	
	Private Function ExecLoadCmd() As Integer
		Dim new_titles() As String
		Dim tname, tfolder As String
		Dim i As Integer
		Dim j As Short
		Dim cur_data_size As Integer
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
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		cur_data_size = UBound(EventData)
		
		'使用しているタイトルのデータをロード
		For i = 1 To UBound(new_titles)
			IncludeData(new_titles(i))
			tfolder = SearchDataFolder(new_titles(i))
			If FileExists(tfolder & "\include.eve") Then
				LoadEventData2(tfolder & "\include.eve", UBound(EventData))
			End If
		Next 
		
		'ローカルデータの読みこみ
		If FileExists(ScenarioPath & "Data\alias.txt") Then
			ALDList.Load(ScenarioPath & "Data\alias.txt")
		End If
		If FileExists(ScenarioPath & "Data\sp.txt") Then
			SPDList.Load(ScenarioPath & "Data\sp.txt")
		ElseIf FileExists(ScenarioPath & "Data\mind.txt") Then 
			SPDList.Load(ScenarioPath & "Data\mind.txt")
		End If
		If FileExists(ScenarioPath & "Data\pilot.txt") Then
			PDList.Load(ScenarioPath & "Data\pilot.txt")
		End If
		If FileExists(ScenarioPath & "Data\non_pilot.txt") Then
			NPDList.Load(ScenarioPath & "Data\non_pilot.txt")
		End If
		If FileExists(ScenarioPath & "Data\robot.txt") Then
			UDList.Load(ScenarioPath & "Data\robot.txt")
		End If
		If FileExists(ScenarioPath & "Data\unit.txt") Then
			UDList.Load(ScenarioPath & "Data\unit.txt")
		End If
		
		If FileExists(ScenarioPath & "Data\pilot_message.txt") Then
			MDList.Load(ScenarioPath & "Data\pilot_message.txt")
		End If
		If FileExists(ScenarioPath & "Data\pilot_dialog.txt") Then
			DDList.Load(ScenarioPath & "Data\pilot_dialog.txt")
		End If
		If FileExists(ScenarioPath & "Data\item.txt") Then
			IDList.Load(ScenarioPath & "Data\item.txt")
		End If
		
		For i = cur_data_size + 1 To UBound(EventData)
			'複数行に分割されたコマンドを結合
			If Right(EventData(i), 1) = "_" Then
				If UBound(EventData) > i Then
					EventData(i + 1) = Left(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
					EventData(i) = " "
				End If
			End If
		Next 
		
		'ラベルの登録
		For i = cur_data_size + 1 To UBound(EventData)
			If Right(EventData(i), 1) = ":" Then
				AddLabel(Left(EventData(i), Len(EventData(i)) - 1), i)
			End If
		Next 
		
		'コマンドデータ配列を増やす
		If UBound(EventData) > UBound(EventCmd) Then
			ReDim Preserve EventCmd(UBound(EventData))
		End If
		
		'イベントデータの構文解析
		For i = cur_data_size + 1 To UBound(EventData)
			If EventCmd(i) Is Nothing Then
				EventCmd(i) = New CmdData
			End If
			With EventCmd(i)
				.LineNum = i
				.Parse(EventData(i))
			End With
		Next 
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ExecLoadCmd = LineNum + 1
	End Function
	
	Private Function ExecLocalCmd() As Integer
		Dim vname As String
		Dim i As Short
		Dim etype As Expression.ValueType
		Dim str_result As String
		Dim num_result As Double
		
		'代入式付きの変数定義？
		If ArgNum >= 4 Then
			If GetArg(3) = "=" Then
				If VarIndex >= MaxVarIndex Then
					VarIndex = MaxVarIndex
					EventErrorMessage = VB6.Format(MaxVarIndex) & "個を超えるサブルーチンローカル変数は作成できません"
					Error(0)
				End If
				
				vname = GetArg(2)
				If InStr(vname, """") > 0 Then
					EventErrorMessage = "変数名「" & vname & "」が不正です"
					Error(0)
				End If
				If Asc(vname) = 36 Then '$
					vname = Mid(vname, 2)
				End If
				
				If ArgNum = 4 Then
					Select Case ArgsType(4)
						Case Expression.ValueType.UndefinedType
							etype = EvalTerm(strArgs(4), Expression.ValueType.UndefinedType, str_result, num_result)
							VarIndex = VarIndex + 1
							With VarStack(VarIndex)
								.Name = vname
								.VariableType = etype
								.StringValue = str_result
								.NumericValue = num_result
							End With
						Case Expression.ValueType.StringType
							VarIndex = VarIndex + 1
							With VarStack(VarIndex)
								.Name = vname
								.VariableType = Expression.ValueType.StringType
								.StringValue = strArgs(4)
								.NumericValue = num_result
							End With
						Case Expression.ValueType.NumericType
							VarIndex = VarIndex + 1
							With VarStack(VarIndex)
								.Name = vname
								.VariableType = Expression.ValueType.NumericType
								.StringValue = str_result
								.NumericValue = dblArgs(4)
							End With
					End Select
				Else
					etype = EvalTerm(strArgs(4), Expression.ValueType.UndefinedType, str_result, num_result)
					
					VarIndex = VarIndex + 1
					With VarStack(VarIndex)
						.Name = vname
						.VariableType = Expression.ValueType.NumericType
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
			EventErrorMessage = VB6.Format(MaxVarIndex) & "個を超えるサブルーチンローカル変数は作成できません"
			Error(0)
		End If
		
		For i = 2 To ArgNum
			With VarStack(VarIndex - i + 2)
				vname = GetArg(i)
				If InStr(vname, """") > 0 Then
					EventErrorMessage = "変数名「" & vname & "」が不正です"
					Error(0)
				End If
				If Asc(vname) = 36 Then '$
					vname = Mid(vname, 2)
				End If
				
				.Name = vname
				.VariableType = Expression.ValueType.StringType
				.StringValue = ""
			End With
		Next 
		
		ExecLocalCmd = LineNum + 1
	End Function
	
	Private Function ExecMakePilotListCmd() As Integer
		Dim u As Unit
		Dim p As Pilot
		Dim xx, yy As Short
		Dim key_type As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim pilot_list() As Pilot
		Dim i, j As Short
		Dim buf As String
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		'パイロットがどのユニットに乗っていたか記録しておく
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Then
					'あらかじめ撤退させておく
					.Escape("非同期")
				End If
				If .Status_Renamed = "待機" Then
					If InStr(.Name, "ステータス表示用") = 0 Then
						For i = 1 To .CountPilot
							SetVariableAsString("搭乗ユニット[" & .Pilot(i).ID & "]", .ID)
						Next 
						For i = 1 To .CountSupport
							SetVariableAsString("搭乗ユニット[" & .Support(i).ID & "]", .ID)
						Next 
					End If
				End If
			End With
		Next u
		
		'マップをクリア
		LoadMapData("")
		SetupBackground("", "ステータス")
		
		'ユニット一覧を作成
		key_type = GetArgAsString(2)
		If key_type <> "名称" Then
			'配列作成
			ReDim pilot_list(PList.Count)
			ReDim key_list(PList.Count)
			i = 0
			For	Each p In PList
				With p
					If Not .Alive Or .Away Then
						GoTo NextPilot1
					End If
					
					If Not .Unit_Renamed Is Nothing Then
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
					pilot_list(i) = p
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
			Next p
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
					p = pilot_list(i)
					pilot_list(i) = pilot_list(max_item)
					pilot_list(max_item) = p
					
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
			For	Each p In PList
				With p
					If Not .Alive Or .Away Then
						GoTo NextPilot2
					End If
					
					If Not .Unit_Renamed Is Nothing Then
						If .Name = .Unit_Renamed.FeatureData("追加パイロット") Then
							'追加パイロットは勘定に入れない
							GoTo NextPilot2
						End If
					End If
					
					i = i + 1
					pilot_list(i) = p
					strkey_list(i) = p.KanaName
				End With
NextPilot2: 
			Next p
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
					p = pilot_list(i)
					pilot_list(i) = pilot_list(max_item)
					pilot_list(max_item) = p
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'Font Regular 9pt 背景
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMain(0).Font
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Size = 9
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Bold = False
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Italic = False
		End With
		PermanentStringMode = True
		HCentering = False
		VCentering = False
		
		xx = 1
		yy = 1
		For i = 1 To UBound(pilot_list)
			p = pilot_list(i)
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
				If .Unit_Renamed Is Nothing Then
					If UDList.IsDefined(.Name & "ステータス表示用ユニット") Then
						u = UList.Add(.Name & "ステータス表示用ユニット", 0, "味方")
					Else
						u = UList.Add("ステータス表示用ダミーユニット", 0, "味方")
					End If
					.Ride(u)
				ElseIf Not .Unit_Renamed.IsFeatureAvailable("ダミーユニット") Then 
					.GetOff()
					If UDList.IsDefined(.Name & "ステータス表示用ユニット") Then
						u = UList.Add(.Name & "ステータス表示用ユニット", 0, "味方")
					Else
						u = UList.Add("ステータス表示用ダミーユニット", 0, "味方")
					End If
					.Ride(u)
				Else
					u = .Unit_Renamed
				End If
				
				'出撃
				u.UsedAction = 0
				u.StandBy(xx, yy, "非同期")
				
				'プレイヤーが操作できないように
				u.AddCondition("非操作", -1)
				
				'パイロットの愛称を表示
				DrawString(.Nickname, 32 * xx + 2, 32 * yy - 31)
				
				Select Case key_type
					Case "レベル", "名称"
						DrawString("Lv" & VB6.Format(.Level), 32 * xx + 2, 32 * yy - 15)
					Case "ＳＰ"
						DrawString(Term("SP", u) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "格闘"
						DrawString(Left(Term("格闘", u), 1) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "射撃"
						If .HasMana() Then
							DrawString(Left(Term("魔力", u), 1) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
						Else
							DrawString(Left(Term("射撃", u), 1) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
						End If
					Case "命中"
						DrawString(Left(Term("命中", u), 1) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "回避"
						DrawString(Left(Term("回避", u), 1) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "技量"
						DrawString(Left(Term("技量", u), 1) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
					Case "反応"
						DrawString(Left(Term("反応", u), 1) & VB6.Format(key_list(i)), 32 * xx + 2, 32 * yy - 15)
				End Select
				
				'表示位置を右に3マスずらす
				xx = xx + 3
			End With
		Next 
		
		'フォントの設定を戻しておく
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMain(0).Font
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Size = 16
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Bold = True
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Italic = False
		End With
		PermanentStringMode = False
		
		RedrawScreen()
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ExecMakePilotListCmd = LineNum + 1
	End Function
	
	Private Function ExecMakeUnitListCmd() As Integer
		'ユニット一覧を作成
		MakeUnitList(GetArgAsString(2))
		ExecMakeUnitListCmd = LineNum + 1
	End Function
	
	Private Function ExecMapAbilityCmd() As Integer
		Dim u As Unit
		Dim tx, ty As Short
		Dim a As Short
		
		Select Case ArgNum
			Case 5
				u = GetArgAsUnit(2)
				
				With u
					For a = 1 To .CountAbility
						If GetArgAsString(3) = .Ability(a).Name And .IsAbilityClassifiedAs(a, "Ｍ") Then
							Exit For
						End If
					Next 
					If a > .CountAbility Then
						EventErrorMessage = "アビリティ名が間違っています"
						Error(0)
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
				u = SelectedUnitForEvent
				
				With u
					For a = 1 To .CountAbility
						If GetArgAsString(2) = .Ability(a).Name And .IsAbilityClassifiedAs(a, "Ｍ") Then
							Exit For
						End If
					Next 
					If a > .CountAbility Then
						EventErrorMessage = "アビリティ名が間違っています"
						Error(0)
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
				Error(0)
		End Select
		
		With u
			If .Status_Renamed <> "出撃" Then
				EventErrorMessage = .Nickname & "は出撃していません"
				Error(0)
			End If
			OpenMessageForm()
			.ExecuteMapAbility(a, tx, ty, True)
			CloseMessageForm()
		End With
		
		RedrawScreen()
		
		ExecMapAbilityCmd = LineNum + 1
	End Function
	
	Private Function ExecMapAttackCmd() As Integer
		Dim u As Unit
		Dim tx, ty As Short
		Dim w As Short
		Dim prev_w, prev_tw As Short
		Dim cur_stage As String
		Dim is_event As Boolean
		Dim num As Short
		
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
				u = GetArgAsUnit(2)
				
				With u
					For w = 1 To .CountWeapon
						If GetArgAsString(3) = .Weapon(w).Name And .IsWeaponClassifiedAs(w, "Ｍ") Then
							Exit For
						End If
					Next 
					If w > .CountWeapon Then
						EventErrorMessage = "マップ攻撃名が間違っています"
						Error(0)
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
				u = SelectedUnitForEvent
				
				With u
					For w = 1 To u.CountWeapon
						If GetArgAsString(2) = .Weapon(w).Name And .IsWeaponClassifiedAs(w, "Ｍ") Then
							Exit For
						End If
					Next 
					If w > .CountWeapon Then
						EventErrorMessage = "マップ攻撃名が間違っています"
						Error(0)
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
				Error(0)
		End Select
		
		With u
			If .Status_Renamed <> "出撃" Then
				EventErrorMessage = .Nickname & "は出撃していません"
				Error(0)
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
			
			.MapAttack(w, tx, ty, is_event)
			
			SelectedWeapon = prev_w
			SelectedTWeapon = prev_tw
			
			Stage = cur_stage
		End With
		
		RedrawScreen()
		
		ExecMapAttackCmd = LineNum + 1
	End Function
	
	Private Function ExecMoneyCmd() As Integer
		If ArgNum <> 2 Then
			EventErrorMessage = "Moneyコマンドの引数の数が間違っています"
			Error(0)
		End If
		
		IncrMoney(GetArgAsLong(2))
		
		ExecMoneyCmd = LineNum + 1
	End Function
	
	Private Function ExecMonotoneCmd() As Integer
		Dim prev_x, prev_y As Short
		Dim u As Unit
		Dim late_refresh As Boolean
		Dim i As Short
		Dim buf As String
		
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
					EventErrorMessage = "Monotoneコマンドに不正なオプション「" & buf & "」が使われています"
					Error(0)
			End Select
		Next 
		
		prev_x = MapX
		prev_y = MapY
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		SetupBackground("白黒", "非同期")
		
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Then
					If .BitmapID = 0 Then
						With UList.Item(.Name)
							If u.Party0 = .Party0 And .BitmapID <> 0 And u.Bitmap = .Bitmap And Not .IsFeatureAvailable("ダミーユニット") Then
								u.BitmapID = .BitmapID
							Else
								u.BitmapID = MakeUnitBitmap(u)
							End If
						End With
					End If
				End If
			End With
		Next u
		
		Center(prev_x, prev_y)
		RedrawScreen(late_refresh)
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ExecMonotoneCmd = LineNum + 1
	End Function
	
	Private Function ExecMoveCmd() As Integer
		Dim u As Unit
		Dim ux, uy As Short
		Dim tx, ty As Short
		Dim opt As String
		Dim idx As Short
		
		If Not IsNumeric(GetArgAsString(2)) Then
			idx = 3
			u = GetArgAsUnit(2)
		Else
			idx = 2
			u = SelectedUnitForEvent
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
			Select Case u.Status_Renamed
				Case "出撃"
					If InStr(opt, "アニメ表示") = 1 Then
						'現在位置を記録
						ux = .X
						uy = .Y
						
						'目的地にユニットがいて入れない場合があるので
						'実際に移動させて到着地点を確かめる
						.Jump(tx, ty, False)
						tx = .X
						ty = .Y
						
						'一旦元の位置に戻す
						.Jump(ux, uy, False)
						
						'移動アニメ表示
						MoveUnitBitmap(u, ux, uy, tx, ty, 20)
					End If
					
					.Jump(tx, ty, False)
				Case "格納"
					.StandBy(tx, ty, opt)
				Case Else
					EventErrorMessage = .MainPilot.Nickname & "は出撃していません"
					Error(0)
			End Select
		End With
		
		If opt = "" Or InStr(opt, "アニメ表示") = 1 Then
			If MainForm.Visible And Not IsPictureVisible Then
				RedrawScreen()
			End If
		ElseIf opt = "非同期" Then 
			'画面更新しない
		Else
			EventErrorMessage = "Moveコマンドの引数の数が違います"
			Error(0)
		End If
		
		ExecMoveCmd = LineNum + 1
	End Function
	
	Private Function ExecNightCmd() As Integer
		Dim prev_x, prev_y As Short
		Dim u As Unit
		Dim late_refresh As Boolean
		Dim i As Short
		Dim buf As String
		
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
					EventErrorMessage = "Nightコマンドに不正なオプション「" & buf & "」が使われています"
					Error(0)
			End Select
		Next 
		
		prev_x = MapX
		prev_y = MapY
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		SetupBackground("夜", "非同期")
		
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Then
					If .BitmapID = 0 Then
						With UList.Item(.Name)
							If u.Party0 = .Party0 And .BitmapID <> 0 And u.Bitmap = .Bitmap And Not .IsFeatureAvailable("ダミーユニット") Then
								u.BitmapID = .BitmapID
							Else
								u.BitmapID = MakeUnitBitmap(u)
							End If
						End With
					End If
				End If
			End With
		Next u
		
		Center(prev_x, prev_y)
		RedrawScreen(late_refresh)
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ExecNightCmd = LineNum + 1
	End Function
	
	Private Function ExecNoonCmd() As Integer
		Dim prev_x, prev_y As Short
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
					Error(0)
				End If
			Case Else
				EventErrorMessage = "Noonコマンドの引数の数が違います"
				Error(0)
		End Select
		
		MapDrawIsMapOnly = False
		
		prev_x = MapX
		prev_y = MapY
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		SetupBackground("", "非同期")
		
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Then
					If .BitmapID = 0 Then
						With UList.Item(.Name)
							If u.Party0 = .Party0 And .BitmapID <> 0 And u.Bitmap = .Bitmap And Not .IsFeatureAvailable("ダミーユニット") Then
								u.BitmapID = .BitmapID
							Else
								u.BitmapID = MakeUnitBitmap(u)
							End If
						End With
					End If
				End If
			End With
		Next u
		
		Center(prev_x, prev_y)
		RedrawScreen(late_refresh)
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ExecNoonCmd = LineNum + 1
	End Function
	
	Private Function ExecOpenCmd() As Integer
		Dim fname As String
		Dim vname As String
		Dim opt As String
		Dim f As Short
		
		If ArgNum <> 6 Then
			EventErrorMessage = "Openコマンドの引数の数が違います"
			Error(0)
		End If
		
		fname = ScenarioPath & GetArgAsString(2)
		
		If InStr(fname, "..\") > 0 Then
			EventErrorMessage = "ファイル指定に「..\」は使えません"
			Error(0)
		End If
		If InStr(fname, "../") > 0 Then
			EventErrorMessage = "ファイル指定に「../」は使えません"
			Error(0)
		End If
		
		opt = GetArgAsString(4)
		vname = GetArg(6)
		
		f = FreeFile
		SetVariableAsLong(vname, f)
		Select Case opt
			Case "出力"
				FileOpen(f, fname, OpenMode.Output, OpenAccess.Write)
			Case "追加出力"
				FileOpen(f, fname, OpenMode.Append, OpenAccess.Write)
			Case "入力"
				If Not FileExists(fname) Then
					EventErrorMessage = fname & "というファイルは存在しません"
					Error(0)
				End If
				FileOpen(f, fname, OpenMode.Input, OpenAccess.Read)
			Case Else
				EventErrorMessage = "ファイルの入出力モードが不正です"
				Error(0)
		End Select
		
		ExecOpenCmd = LineNum + 1
	End Function
	
	Private Function ExecOptionCmd() As Integer
		Dim vname As String
		
		Select Case ArgNum
			Case 2
				vname = GetArgAsString(2)
				vname = "Option(" & vname & ")"
				If Not IsGlobalVariableDefined(vname) Then
					DefineGlobalVariable(vname)
				End If
				SetVariableAsLong(vname, 1)
				' ADD START MARGE
				If vname = "Option(新ＧＵＩ)" Then
					' 新ＧＵＩが指定されたら即反映するためにメイン画面をロードしなおす
					LoadForms()
				End If
				' ADD END MARGE
			Case 3
				vname = GetArgAsString(2)
				vname = "Option(" & vname & ")"
				If IsGlobalVariableDefined(vname) Then
					UndefineVariable(vname)
				End If
			Case Else
				EventErrorMessage = "Optionコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExecOptionCmd = LineNum + 1
	End Function
	
	Private Function ExecOrganizeCmd() As Integer
		Dim unum As Short
		Dim u As Unit
		Dim ux, uy As Short
		Dim uclass As String
		Dim buf, opt As String
		Dim j, i, num As Short
		Dim tmp As Integer
		Dim min_value As Integer
		Dim max_item As Short
		Dim max_value As Integer
		Dim lv_list() As Integer
		Dim list() As String
		Dim ret As Short
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
			Error(0)
		End If
		
		unum = GetArgAsLong(2)
		If unum < 1 Then
			EventErrorMessage = "ユニット数が不正です"
			Error(0)
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
			uclass = Trim(uclass)
		End If
		
Beginning: 
		
		ReDim list(0)
		ReDim ListItemID(0)
		For	Each u In UList
			With u
				If .Party0 <> "味方" Or .Status_Renamed <> "待機" Or .CountPilot = 0 Then
					GoTo NextOrganizeLoop
				End If
				
				'パイロット数のチェック
				If (.Data.PilotNum = 1 Or System.Math.Abs(.Data.PilotNum) = 2) And .CountPilot < System.Math.Abs(.Data.PilotNum) And Not .IsFeatureAvailable("１人乗り可能") Then
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
						If TerrainName(1, 1) = "空" And TerrainName(MapWidth \ 2, MapHeight \ 2) = "空" And TerrainName(MapWidth, MapHeight) = "空" Then
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
						If .Size = "XL" Or .Size = "LL" Then
							GoTo NextOrganizeLoop
						End If
					Case "M"
						If .Size = "XL" Or .Size = "LL" Or .Size = "L" Then
							GoTo NextOrganizeLoop
						End If
					Case "S"
						If .Size = "XL" Or .Size = "LL" Or .Size = "L" Or .Size = "M" Then
							GoTo NextOrganizeLoop
						End If
					Case "SS"
						If .Size = "XL" Or .Size = "LL" Or .Size = "L" Or .Size = "M" Or .Size = "S" Then
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
					'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
					'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
					list(UBound(list)) = .Nickname0 & Space(MaxLng(52 - LenB(StrConv(.Nickname0, vbFromUnicode)), 1)) & LeftPaddedString(CStr(.MainPilot.Level), 2)
				Else
					'UPGRADE_ISSUE: 定数 vbFromUnicode はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
					'UPGRADE_ISSUE: LenB 関数はサポートされません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' をクリックしてください。
					list(UBound(list)) = .Nickname0 & Space(MaxLng(36 - LenB(StrConv(.Nickname0, vbFromUnicode)), 1)) & .MainPilot.Nickname & Space(MaxLng(17 - LenB(StrConv(.MainPilot.Nickname, vbFromUnicode)), 1)) & LeftPaddedString(CStr(.MainPilot.Level), 2)
				End If
				ListItemID(UBound(list)) = .ID
			End With
NextOrganizeLoop: 
		Next u
		ReDim ListItemFlag(UBound(list))
		
		'レベルの一覧と最大値・最小値を求める
		ReDim lv_list(UBound(list))
		min_value = 100000
		max_value = 0
		For i = 1 To UBound(list)
			With UList.Item(ListItemID(i)).MainPilot
				lv_list(i) = 500 * CInt(.Level) + CInt(.Exp)
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
					ret = MultiSelectListBox("出撃ユニット選択", list, "ユニット                                            Lv", unum)
				Else
					ret = MultiSelectListBox("出撃ユニット選択", list, "ユニット                            パイロット       Lv", unum)
				End If
				If ret = 0 Then
					CommandState = "ユニット選択"
					UnlockGUI()
					ViewMode = True
					Do While ViewMode
						Sleep(50)
						System.Windows.Forms.Application.DoEvents()
					Loop 
					LockGUI()
					GoTo Beginning
				End If
			Loop While ret = 0
			
			If InStr(opt, "非同期") > 0 Then
				Center(ux, uy)
				RefreshScreen()
			End If
			
			For i = 1 To UBound(list)
				If ListItemFlag(i) Then
					With UList.Item(ListItemID(i))
						.UsedAction = 0
						.UsedSupportAttack = 0
						.UsedSupportGuard = 0
						.UsedSyncAttack = 0
						.UsedCounterAttack = 0
						.StandBy(ux, uy, opt)
					End With
				End If
			Next 
		End If
		
		UList.CheckAutoHyperMode()
		
		ReDim ListItemID(0)
		
		ExecOrganizeCmd = LineNum + 1
	End Function
	
	Private Function ExecOvalCmd() As Integer
		Dim pic, pic2 As System.Windows.Forms.PictureBox
		Dim y1, x1, rad As Short
		Dim oval_ratio As Double
		Dim opt As String
		Dim cname As String
		Dim clr As Integer
		Dim i As Short
		
		If ArgNum < 5 Then
			EventErrorMessage = "Ovalコマンドの引数の数が違います"
			Error(0)
		End If
		
		x1 = GetArgAsLong(2) + BaseX
		y1 = GetArgAsLong(3) + BaseY
		rad = GetArgAsLong(4)
		oval_ratio = GetArgAsDouble(5)
		
		SaveScreen()
		
		'描画先
		Select Case ObjDrawOption
			Case "背景"
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picBack
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMaskedBack
				IsMapDirty = True
			Case "保持"
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMain(1)
				IsPictureVisible = True
			Case Else
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
		End Select
		
		'描画領域
		Dim tmp As Short
		If ObjDrawOption <> "背景" Then
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
					Error(0)
				End If
				cname = New String(vbNullChar, 8)
				Mid(cname, 1, 2) = "&H"
				Mid(cname, 3, 2) = Mid(opt, 6, 2)
				Mid(cname, 5, 2) = Mid(opt, 4, 2)
				Mid(cname, 7, 2) = Mid(opt, 2, 2)
				If Not IsNumeric(cname) Then
					EventErrorMessage = "色指定が不正です"
					Error(0)
				End If
				clr = CInt(cname)
			Else
				EventErrorMessage = "Ovalコマンドに不正なオプション「" & opt & "」が使われています"
				Error(0)
			End If
		Next 
		
		With pic
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.DrawWidth = ObjDrawWidth
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillColor = ObjFillColor
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillStyle = ObjFillStyle
		End With
		
		'UPGRADE_ISSUE: PictureBox メソッド pic.Circle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.Circle (x1, y1), rad, clr, 0, 0, oval_ratio
		
		With pic
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.DrawWidth = 1
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
			'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillStyle = vbFSTransparent
		End With
		
		If Not pic2 Is Nothing Then
			With pic
				'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.DrawWidth = ObjDrawWidth
				'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillColor = ObjFillColor
				'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillStyle = ObjFillStyle
			End With
			
			'UPGRADE_ISSUE: PictureBox メソッド pic.Circle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic.Circle (x1, y1), rad, clr, 0, 0, oval_ratio
			
			With pic
				'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.DrawWidth = 1
				'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
				'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillStyle = vbFSTransparent
			End With
		End If
		
		ExecOvalCmd = LineNum + 1
	End Function
	
	Private Function ExecPaintPictureCmd() As Integer
		Dim fname As String
		Dim dx, dy As Integer
		Dim dw, dh As Integer
		Dim sx, sy As Integer
		Dim sw, sh As Integer
		Dim i, opt_n As Short
		Dim ret As Short
		Dim buf, options As String
		Dim cname As String
		Dim tcolor As Integer
		
		If ArgNum < 4 Then
			EventErrorMessage = "PaintPictureコマンドの引数の数が違います"
			Error(0)
		End If
		
		tcolor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
		
		i = 5
		opt_n = 4
		Do While i <= ArgNum
			buf = GetArgAsString(i)
			Select Case buf
				Case "透過", "背景", "白黒", "セピア", "明", "暗", "上下反転", "左右反転", "上半分", "下半分", "右半分", "左半分", "右上", "左上", "右下", "左下", "ネガポジ反転", "シルエット", "夕焼け", "水中", "保持", "フィルタ"
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
						cname = New String(vbNullChar, 8)
						Mid(cname, 1, 2) = "&H"
						Mid(cname, 3, 2) = Mid(buf, 6, 2)
						Mid(cname, 5, 2) = Mid(buf, 4, 2)
						Mid(cname, 7, 2) = Mid(buf, 2, 2)
						If IsNumeric(cname) Then
							tcolor = CInt(cname)
							If tcolor <> System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White) Or GetArgAsString(i - 1) = "フィルタ" Then
								options = options & VB6.Format(tcolor) & " "
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
						EventErrorMessage = "PaintPictureコマンドの" & VB6.Format(i) & "番目のパラメータ「" & buf & "」が不正です"
						Error(0)
					End If
			End Select
			i = i + 1
		Loop 
		
		fname = GetArgAsString(2)
		Select Case Right(LCase(fname), 4)
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
					EventErrorMessage = "不正な画像ファイル名「" & fname & "」が指定されています"
					Error(0)
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
	
	Private Function ExecPaintStringCmd() As Integer
		Dim sx, sy As String
		Dim xx, yy As Short
		Dim without_cr As Boolean
		
		If CmdName <> Event_Renamed.CmdType.PaintStringCmd Then
			without_cr = True
		End If
		
		'PaintStringはあらかじめ構文解析済み
		Select Case ArgNum
			Case 2
				'座標指定がないことが確定
				' MOD START マージ
				'            DrawString GetArgAsString(2), -1, -1, without_cr
				DrawString(GetArgAsString(2), DEFAULT_LEVEL, DEFAULT_LEVEL, without_cr)
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
					xx = CShort(sx) + BaseX
				End If
				If sy = "-" Then
					VCentering = True
					yy = -1
				Else
					VCentering = False
					yy = CShort(sy) + BaseY
				End If
				
				DrawString(GetArgAsString(4), xx, yy, without_cr)
			Case 5
				'座標指定付きかどうか実行時まで不明
				sx = GetArgAsString(2)
				sy = GetArgAsString(3)
				
				'最初の2引数が有効な座標指定かどうかで判断する
				If (IsNumeric(sx) Or sx = "-") And (IsNumeric(sy) Or sy = "-") Then
					If sx = "-" Then
						HCentering = True
						xx = -1
					Else
						HCentering = False
						xx = CShort(sx) + BaseX
					End If
					If sy = "-" Then
						VCentering = True
						yy = -1
					Else
						VCentering = False
						yy = CShort(sy) + BaseY
					End If
					
					DrawString(GetArgAsString(4), xx, yy, without_cr)
				Else
					DrawString(GetArgAsString(5), -1, -1, without_cr)
				End If
		End Select
		
		ExecPaintStringCmd = LineNum + 1
	End Function
	
	Private Function ExecPaintSysStringCmd() As Integer
		Dim without_refresh As Boolean
		
		If ArgNum <> 4 And ArgNum <> 5 Then
			EventErrorMessage = "PaintSysStringコマンドの引数の数が違います"
			Error(0)
		End If
		
		If ArgNum = 5 Then
			If GetArgAsString(5) = "非同期" Then
				without_refresh = True
			End If
		End If
		
		DrawSysString(GetArgAsLong(2), GetArgAsLong(3), GetArgAsString(4), without_refresh)
		
		ExecPaintSysStringCmd = LineNum + 1
	End Function
	
	Private Function ExecPilotCmd() As Integer
		Dim pname As String
		Dim plevel As Short
		
		If ArgNum < 0 Then
			EventErrorMessage = "Pilotコマンドのパラメータの括弧の対応が取れていません"
			Error(0)
		ElseIf ArgNum <> 3 And ArgNum <> 4 Then 
			EventErrorMessage = "Pilotコマンドの引数の数が違います"
			Error(0)
		End If
		
		pname = GetArgAsString(2)
		If Not PDList.IsDefined(pname) Then
			EventErrorMessage = "指定したパイロット「" & pname & "」のデータが見つかりません"
			Error(0)
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
				.FullRecover()
			End With
		Else
			With PList.Add(pname, plevel, "味方", GetArgAsString(4))
				.FullRecover()
			End With
		End If
		
		ExecPilotCmd = LineNum + 1
	End Function
	
	Private Function ExecPlayMIDICmd() As Integer
		Dim fname As String
		Dim play_bgm_end As Integer
		Dim i As Integer
		
		'PlayMIDIコマンドが連続してる場合、最後のPlayMIDIコマンドの位置を検索
		For i = LineNum + 1 To UBound(EventCmd)
			If EventCmd(i).Name <> Event_Renamed.CmdType.PlayMIDICmd Then
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
		StartBGM(fname, False)
		
		'次のコマンド実行位置は最後のPlayMIDIコマンドの後
		ExecPlayMIDICmd = play_bgm_end + 1
	End Function
	
	Private Function ExecPlaySoundCmd() As Integer
		Dim fname As String
		
		fname = ListTail(EventData(LineNum), 2)
		If ListLength(fname) = 1 Then
			fname = GetValueAsString(fname)
		End If
		
		PlayWave(fname)
		
		ExecPlaySoundCmd = LineNum + 1
	End Function
	
	Private Function ExecPolygonCmd() As Integer
		Dim pic, pic2 As System.Windows.Forms.PictureBox
		Dim points() As POINTAPI
		Dim pnum As Short
		Dim xx, yy As Short
		Dim x1, y1 As Short
		Dim x2, y2 As Short
		Dim prev_clr As Integer
		
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
			Error(0)
		End If
		
		SaveScreen()
		
		'描画先
		Select Case ObjDrawOption
			Case "背景"
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picBack
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMaskedBack
				IsMapDirty = True
			Case "保持"
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMain(1)
			Case Else
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
		End Select
		
		'描画領域
		Dim tmp As Short
		If ObjDrawOption <> "背景" Then
			IsPictureVisible = True
			tmp = ObjDrawWidth - 1
			PaintedAreaX1 = MinLng(PaintedAreaX1, MaxLng(x1 - tmp, 0))
			PaintedAreaY1 = MinLng(PaintedAreaY1, MaxLng(y1 - tmp, 0))
			PaintedAreaX2 = MaxLng(PaintedAreaX2, MinLng(x2 + tmp, MapPWidth - 1))
			PaintedAreaY2 = MaxLng(PaintedAreaY2, MinLng(y2 + tmp, MapPHeight - 1))
		End If
		
		With pic
			prev_clr = System.Drawing.ColorTranslator.ToOle(.ForeColor)
			.ForeColor = System.Drawing.ColorTranslator.FromOle(ObjColor)
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.DrawWidth = ObjDrawWidth
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillColor = ObjFillColor
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillStyle = ObjFillStyle
		End With
		
		'UPGRADE_ISSUE: PictureBox プロパティ pic.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		Call Polygon(pic.hDC, points(0), pnum - 1)
		
		With pic
			.ForeColor = System.Drawing.ColorTranslator.FromOle(prev_clr)
			'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.DrawWidth = 1
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
			'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
			'UPGRADE_ISSUE: PictureBox プロパティ pic.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			.FillStyle = vbFSTransparent
		End With
		
		If Not pic2 Is Nothing Then
			With pic2
				prev_clr = System.Drawing.ColorTranslator.ToOle(.ForeColor)
				.ForeColor = System.Drawing.ColorTranslator.FromOle(ObjColor)
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.DrawWidth = ObjDrawWidth
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillColor = ObjFillColor
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillStyle = ObjFillStyle
			End With
			
			'UPGRADE_ISSUE: PictureBox プロパティ pic2.hDC はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			Call Polygon(pic2.hDC, points(0), pnum - 1)
			
			With pic2
				.ForeColor = System.Drawing.ColorTranslator.FromOle(prev_clr)
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.DrawWidth = 1
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillColor はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillColor = System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.White)
				'UPGRADE_ISSUE: 定数 vbFSTransparent はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' をクリックしてください。
				'UPGRADE_ISSUE: PictureBox プロパティ pic2.FillStyle はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
				.FillStyle = vbFSTransparent
			End With
		End If
		
		ExecPolygonCmd = LineNum + 1
	End Function
	
	Private Function ExecPrintCmd() As Integer
		Dim f As Short
		Dim msg As String
		
		If ArgNum = 1 Then
			EventErrorMessage = "Printコマンドの引数の数が違います"
			Error(0)
		End If
		
		f = GetArgAsLong(2)
		
		msg = ListTail(EventData(LineNum), 3)
		
		If Right(msg, 1) <> ";" Then
			If Left(msg, 1) <> "`" Or Right(msg, 1) <> "`" Then
				If Left(msg, 2) = "$(" Then
					If Right(msg, 1) = ")" Then
						msg = GetValueAsString(Mid(msg, 3, Len(msg) - 3))
					End If
				Else
					If ListLength(msg) = 1 Then
						msg = GetValueAsString(msg)
					End If
				End If
				ReplaceSubExpression(msg)
			Else
				msg = Mid(msg, 2, Len(msg) - 2)
			End If
			PrintLine(f, msg)
		Else
			msg = Left(msg, Len(msg) - 1)
			If Left(msg, 1) <> "`" Or Right(msg, 1) <> "`" Then
				If Left(msg, 2) = "$(" Then
					If Right(msg, 1) = ")" Then
						msg = GetValueAsString(Mid(msg, 3, Len(msg) - 3))
					End If
				Else
					If ListLength(msg) = 1 Then
						msg = GetValueAsString(msg)
					End If
				End If
				ReplaceSubExpression(msg)
			Else
				msg = Mid(msg, 2, Len(msg) - 2)
			End If
			Print(f, msg)
		End If
		
		ExecPrintCmd = LineNum + 1
	End Function
	
	Private Function ExecPSetCmd() As Integer
		Dim pic, pic2 As System.Windows.Forms.PictureBox
		Dim xx, yy As Short
		Dim opt As String
		Dim cname As String
		Dim clr As Integer
		
		If ArgNum < 3 Then
			EventErrorMessage = "PSetコマンドの引数の数が違います"
			Error(0)
		End If
		
		'座標
		xx = GetArgAsLong(2) + BaseX
		yy = GetArgAsLong(3) + BaseY
		
		'座標は画面上にある？
		If xx < 0 Or MapPWidth <= xx Or yy < 0 Or MapPHeight <= yy Then
			ExecPSetCmd = LineNum + 1
			Exit Function
		End If
		
		SaveScreen()
		
		'描画先
		Select Case ObjDrawOption
			Case "背景"
				'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picBack
				'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMaskedBack
				IsMapDirty = True
			Case "保持"
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic2 = MainForm.picMain(1)
			Case Else
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				pic = MainForm.picMain(0)
		End Select
		
		'描画領域
		Dim tmp As Short
		If ObjDrawOption <> "背景" Then
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
				Error(0)
			End If
			cname = New String(vbNullChar, 8)
			Mid(cname, 1, 2) = "&H"
			Mid(cname, 3, 2) = Mid(opt, 6, 2)
			Mid(cname, 5, 2) = Mid(opt, 4, 2)
			Mid(cname, 7, 2) = Mid(opt, 2, 2)
			If Not IsNumeric(cname) Then
				EventErrorMessage = "色指定が不正です"
				Error(0)
			End If
			clr = CInt(cname)
		Else
			clr = ObjColor
		End If
		
		'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.DrawWidth = ObjDrawWidth
		
		'点を描画
		'UPGRADE_ISSUE: PictureBox メソッド pic.PSet はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.PSet (xx, yy), clr
		
		'UPGRADE_ISSUE: PictureBox プロパティ pic.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
		pic.DrawWidth = 1
		
		If Not pic2 Is Nothing Then
			'UPGRADE_ISSUE: PictureBox プロパティ pic2.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic2.DrawWidth = ObjDrawWidth
			
			'点を描画
			'UPGRADE_ISSUE: PictureBox メソッド pic2.PSet はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic2.PSet (xx, yy), clr
			
			'UPGRADE_ISSUE: PictureBox プロパティ pic2.DrawWidth はアップグレードされませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' をクリックしてください。
			pic2.DrawWidth = 1
		End If
		
		ExecPSetCmd = LineNum + 1
	End Function
	
	Private Function ExecQuestionCmd() As Integer
		Dim list() As String
		Dim i As Integer
		Dim buf As String
		
		ReDim list(0)
		ReDim ListItemID(0)
		ReDim ListItemFlag(0)
		ListItemID(0) = "0"
		
		For i = LineNum + 1 To UBound(EventData)
			buf = EventData(i)
			FormatMessage(buf)
			If Len(buf) > 0 Then
				If LCase(buf) = "end" Then
					Exit For
				End If
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve ListItemID(UBound(list))
				ReDim Preserve ListItemFlag(UBound(list))
				list(UBound(list)) = buf
				ListItemID(UBound(list)) = VB6.Format(i - LineNum)
				ListItemFlag(UBound(list)) = False
			End If
		Next 
		If i = UBound(EventData) Then
			EventErrorMessage = "QuestionとEndが対応していません"
			Error(0)
		End If
		
		If UBound(list) > 0 Then
			Select Case ArgNum
				Case 3
					SelectedItem = LIPS("選択", list, GetArgAsString(3), GetArgAsLong(2))
				Case 2
					SelectedItem = LIPS("選択", list, "さあ、どうする？", GetArgAsLong(2))
				Case Else
					EventErrorMessage = "Questionコマンドの引数の数が違います"
					Error(0)
			End Select
		Else
			SelectedItem = 0
		End If
		
		SelectedAlternative = ListItemID(SelectedItem)
		
		ReDim ListItemID(0)
		
		ExecQuestionCmd = i + 1
	End Function
	
	Private Function ExecQuickLoadCmd() As Integer
		LockGUI()
		
		ClearUnitStatus()
		StopBGM()
		
		If FileExists(LastSaveDataFileName) Then
			'セーブしたファイルが存在すればそれをロード
			RestoreData(LastSaveDataFileName, True)
		Else
			'セーブファイルが見つからなければ強制終了
			ErrorMessage("セーブデータが見つかりません")
			TerminateSRC()
		End If
		
		'詰まないように乱数系列をリセット
		RndSeed = RndSeed + 1
		RndReset()
		
		'再開イベントによるマップ画像の書き換え処理を行う
		HandleEvent("再開")
		IsMapDirty = False
		
		'画面を書き直してステータスを表示
		RedrawScreen()
		DisplayGlobalStatus()
		MainForm.Show()
		
		'操作可能にする
		CommandState = "ユニット選択"
		
		UnlockGUI()
		
		IsScenarioFinished = True
		ExecQuickLoadCmd = 0
	End Function
	
	Private Function ExecQuitCmd() As Integer
		TerminateSRC()
	End Function
	
	Private Function ExecRankUpCmd() As Integer
		Dim uname As String
		Dim u As Unit
		Dim rk As Short
		Dim hp_ratio, en_ratio As Double
		Dim i, j As Short
		Dim buf As String
		
		Select Case ArgNum
			Case 3
				uname = GetArgAsString(2)
				u = UList.Item(uname)
				If u Is Nothing Then
					EventErrorMessage = uname & "というユニットは存在しません"
					Error(0)
				End If
				rk = GetArgAsLong(3)
			Case 2
				u = SelectedUnitForEvent
				rk = GetArgAsLong(2)
			Case Else
				EventErrorMessage = "RankUpコマンドの引数の数が違います"
				Error(0)
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
	
	Private Function ExecReadCmd() As Integer
		Dim f As Short
		Dim i As Short
		Dim buf As String
		
		If ArgNum < 3 Then
			EventErrorMessage = "Readコマンドの引数の数が違います"
			Error(0)
		End If
		
		f = GetArgAsLong(2)
		For i = 3 To ArgNum
			Input(f, buf)
			SetVariableAsString(GetArg(i), buf)
		Next 
		
		ExecReadCmd = LineNum + 1
	End Function
	
	Private Function ExecRecoverENCmd() As Integer
		Dim u As Unit
		Dim per As Double
		
		Select Case ArgNum
			Case 3
				u = GetArgAsUnit(2, True)
				per = GetArgAsDouble(3)
			Case 2
				u = SelectedUnitForEvent
				per = GetArgAsDouble(2)
			Case Else
				EventErrorMessage = "RecoverENコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not u Is Nothing Then
			With u
				.RecoverEN(per)
				.Update()
				If .EN = 0 And .Status_Renamed = "出撃" Then
					PaintUnitBitmap(u)
				End If
				.CheckAutoHyperMode()
				.CheckAutoNormalMode()
			End With
		End If
		
		ExecRecoverENCmd = LineNum + 1
	End Function
	
	Private Function ExecRecoverHPCmd() As Integer
		Dim u As Unit
		Dim per As Double
		
		Select Case ArgNum
			Case 3
				u = GetArgAsUnit(2, True)
				per = GetArgAsDouble(3)
			Case 2
				u = SelectedUnitForEvent
				per = GetArgAsDouble(2)
			Case Else
				EventErrorMessage = "RecoverHPコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not u Is Nothing Then
			With u
				.RecoverHP(per)
				.Update()
				.CheckAutoHyperMode()
				.CheckAutoNormalMode()
			End With
		End If
		
		ExecRecoverHPCmd = LineNum + 1
	End Function
	
	Private Function ExecRecoverPlanaCmd() As Integer
		Dim p As Pilot
		Dim per As Double
		Dim hp_ratio, en_ratio As Double
		
		'UPGRADE_NOTE: オブジェクト p をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		p = Nothing
		Select Case ArgNum
			Case 3
				p = GetArgAsPilot(2)
				per = GetArgAsDouble(3)
			Case 2
				With SelectedUnitForEvent
					If .CountPilot > 0 Then
						p = .MainPilot
					End If
				End With
				per = GetArgAsDouble(2)
			Case Else
				EventErrorMessage = "RecoverPlanaコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExecRecoverPlanaCmd = LineNum + 1
		
		If p Is Nothing Then
			Exit Function
		End If
		
		With p
			If .MaxPlana = 0 Then
				Exit Function
			End If
			
			If Not .Unit_Renamed Is Nothing Then
				With .Unit_Renamed
					hp_ratio = 100 * .HP / .MaxHP
					en_ratio = 100 * .EN / .MaxEN
				End With
			End If
			
			.Plana = .Plana + per * .MaxPlana / 100
			
			If Not .Unit_Renamed Is Nothing Then
				With .Unit_Renamed
					.HP = .MaxHP * hp_ratio / 100
					.EN = .MaxEN * en_ratio / 100
				End With
			End If
		End With
	End Function
	
	Private Function ExecRecoverSPCmd() As Integer
		Dim p As Pilot
		Dim per As Double
		
		'UPGRADE_NOTE: オブジェクト p をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		p = Nothing
		Select Case ArgNum
			Case 3
				p = GetArgAsPilot(2)
				per = GetArgAsDouble(3)
			Case 2
				With SelectedUnitForEvent
					If .CountPilot > 0 Then
						p = .MainPilot
					End If
				End With
				per = GetArgAsDouble(2)
			Case Else
				EventErrorMessage = "RecoverSPコマンドの引数の数が違います"
				Error(0)
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
	
	Private Function ExecRedrawCmd() As Integer
		Dim late_refresh As Boolean
		
		If ArgNum = 2 Then
			If GetArgAsString(2) = "非同期" Then
				late_refresh = True
			End If
		End If
		
		RedrawScreen(late_refresh)
		ExecRedrawCmd = LineNum + 1
	End Function
	
	Private Function ExecRefreshCmd() As Integer
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		MainForm.picMain(0).Refresh()
		ExecRefreshCmd = LineNum + 1
	End Function
	
	Private Function ExecReleaseCmd() As Integer
		Dim buf As String
		
		Select Case ArgNum
			Case 1
				buf = SelectedUnitForEvent.MainPilot.Name
			Case 2
				buf = GetArgAsString(2)
				If Not PList.IsDefined(buf) And Not IList.IsDefined(buf) Then
					EventErrorMessage = "パイロット名またはアイテム名" & buf & "が間違っています"
					Error(0)
				End If
				If PList.IsDefined(buf) Then
					buf = PList.Item(buf).Name
				Else
					buf = IList.Item(buf).Name
				End If
			Case Else
				EventErrorMessage = "Releaseコマンドの引数の数が違います"
				Error(0)
		End Select
		
		buf = "Fix(" & buf & ")"
		If IsGlobalVariableDefined(buf) Then
			GlobalVariableList.Remove(buf)
		End If
		
		ExecReleaseCmd = LineNum + 1
	End Function
	
	Private Function ExecRemoveFileCmd() As Integer
		Dim fname As String
		
		If ArgNum <> 2 Then
			EventErrorMessage = "RemoveFileコマンドの引数の数が違います"
			Error(0)
		End If
		
		fname = ScenarioPath & GetArgAsString(2)
		
		If InStr(fname, "..\") > 0 Then
			EventErrorMessage = "ファイル指定に「..\」は使えません"
			Error(0)
		End If
		If InStr(fname, "../") > 0 Then
			EventErrorMessage = "ファイル指定に「../」は使えません"
			Error(0)
		End If
		
		If FileExists(fname) Then
			Kill(fname)
		End If
		
		ExecRemoveFileCmd = LineNum + 1
	End Function
	
	Private Function ExecRemoveFolderCmd() As Integer
		Dim fname As String
		Dim fso As Object
		
		If ArgNum <> 2 Then
			EventErrorMessage = "RemoveFolderコマンドの引数の数が違います"
			Error(0)
		End If
		
		fname = ScenarioPath & GetArgAsString(2)
		
		If InStr(fname, "..\") > 0 Then
			EventErrorMessage = "ファイル指定に「..\」は使えません"
			Error(0)
		End If
		If InStr(fname, "../") > 0 Then
			EventErrorMessage = "ファイル指定に「../」は使えません"
			Error(0)
		End If
		
		If Right(fname, 1) = "\" Then
			fname = Left(fname, Len(fname) - 1)
		End If
		
		If FileExists(fname) Then
			fso = CreateObject("Scripting.FileSystemObject")
			
			'UPGRADE_WARNING: オブジェクト fso.DeleteFolder の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			fso.DeleteFolder(fname)
			
			'UPGRADE_NOTE: オブジェクト fso をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			fso = Nothing
		End If
		
		ExecRemoveFolderCmd = LineNum + 1
	End Function
	
	Private Function ExecRemoveItemCmd() As Integer
		Dim pname As String
		Dim u As Unit
		Dim iname As String
		Dim inumber As Short
		Dim itm As Item
		Dim i, j As Short
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
						.DeleteItem(1)
					Loop 
					
					If item_with_image Then
						.BitmapID = MakeUnitBitmap(SelectedUnitForEvent)
						For i = 1 To .CountOtherForm
							.OtherForm(i).BitmapID = 0
						Next 
						If .Status_Renamed = "出撃" Then
							If Not IsPictureVisible And MapFileName <> "" Then
								PaintUnitBitmap(SelectedUnitForEvent)
							End If
						End If
					End If
				End With
				
			Case 2
				pname = GetArgAsString(2)
				If UList.IsDefined(pname) Then
					'指定したユニットが装備しているアイテムすべてを外す
					u = UList.Item(pname).CurrentForm
					With u
						Do While .CountItem > 0
							If .Item(1).IsFeatureAvailable("ユニット画像") Then
								item_with_image = True
							End If
							
							If .Party0 <> "味方" Then
								.Item(1).Exist = False
							End If
							.DeleteItem(1)
						Loop 
						
						If item_with_image Then
							.BitmapID = MakeUnitBitmap(SelectedUnitForEvent)
							For i = 1 To .CountOtherForm
								.OtherForm(i).BitmapID = 0
							Next 
							If .Status_Renamed = "出撃" Then
								If Not IsPictureVisible And MapFileName <> "" Then
									PaintUnitBitmap(SelectedUnitForEvent)
								End If
							End If
						End If
					End With
				ElseIf PList.IsDefined(pname) Then 
					'指定したパイロットが乗るユニットが装備しているアイテムすべてを外す
					u = PList.Item(pname).Unit_Renamed
					If Not u Is Nothing Then
						With u
							Do While .CountItem > 0
								If .Item(1).IsFeatureAvailable("ユニット画像") Then
									item_with_image = True
								End If
								
								If .Party0 <> "味方" Then
									.Item(1).Exist = False
								End If
								.DeleteItem(1)
							Loop 
							
							If item_with_image Then
								.BitmapID = MakeUnitBitmap(u)
								For i = 1 To .CountOtherForm
									.OtherForm(i).BitmapID = 0
								Next 
								If .Status_Renamed = "出撃" Then
									If Not IsPictureVisible And MapFileName <> "" Then
										PaintUnitBitmap(u)
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
							inumber = CShort(iname)
							If inumber < 1 Then
								EventErrorMessage = "指定されたアイテム番号「" & iname & "」が不正です"
								Error(0)
							End If
							If inumber > .CountItem Then
								EventErrorMessage = "指定されたユニットは" & VB6.Format(.CountItem) & "個のアイテムしか持っていません"
								Error(0)
							End If
							
							With .Item(inumber)
								If .IsFeatureAvailable("ユニット画像") Then
									item_with_image = True
								End If
								
								SelectedUnitForEvent.DeleteItem(.ID)
								
								If item_with_image Then
									With SelectedUnitForEvent
										.BitmapID = MakeUnitBitmap(SelectedUnitForEvent)
										For i = 1 To .CountOtherForm
											.OtherForm(i).BitmapID = 0
										Next 
										If .Status_Renamed = "出撃" Then
											If Not IsPictureVisible And MapFileName <> "" Then
												PaintUnitBitmap(SelectedUnitForEvent)
											End If
										End If
									End With
								End If
								
								'UPGRADE_NOTE: オブジェクト SelectedUnitForEvent.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
								.Unit_Renamed = Nothing
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
								If Not .Unit_Renamed Is Nothing Then
									If .IsFeatureAvailable("ユニット画像") Then
										item_with_image = True
									End If
									
									.Unit_Renamed.DeleteItem(.ID)
									
									If item_with_image Then
										.Unit_Renamed.BitmapID = MakeUnitBitmap(.Unit_Renamed)
										With .Unit_Renamed
											For i = 1 To .CountOtherForm
												.OtherForm(i).BitmapID = 0
											Next 
											If .Status_Renamed = "出撃" Then
												If Not IsPictureVisible And MapFileName <> "" Then
													PaintUnitBitmap(SelectedUnitForEvent)
												End If
											End If
										End With
									End If
								End If
								
								'UPGRADE_NOTE: オブジェクト IList.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
								.Unit_Renamed = Nothing
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
					For	Each itm In IList
						With itm
							If .Name = iname And .Exist And .Unit_Renamed Is Nothing Then
								'見つかった
								.Exist = False
								Exit For
							End If
						End With
					Next itm
					'見つからなかったら装備されたアイテムから
					If itm Is Nothing Then
						For	Each itm In IList
							With itm
								If .Name = iname And .Exist Then
									If .IsFeatureAvailable("ユニット画像") Then
										item_with_image = True
									End If
									
									u = .Unit_Renamed
									u.DeleteItem(.ID)
									
									If item_with_image Then
										u.BitmapID = MakeUnitBitmap(u)
										With u
											For i = 1 To .CountOtherForm
												.OtherForm(i).BitmapID = 0
											Next 
											If .Status_Renamed = "出撃" Then
												If Not IsPictureVisible And MapFileName <> "" Then
													PaintUnitBitmap(SelectedUnitForEvent)
												End If
											End If
										End With
									End If
									
									'UPGRADE_NOTE: オブジェクト itm.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
									.Unit_Renamed = Nothing
									.Exist = False
									Exit For
								End If
							End With
						Next itm
					End If
				End If
				
			Case 3
				'指定されたアイテムを削除
				pname = GetArgAsString(2)
				If UList.IsDefined(pname) Then
					u = UList.Item(pname).CurrentForm
				ElseIf PList.IsDefined(pname) Then 
					u = PList.Item(pname).Unit_Renamed
					If u Is Nothing Then
						EventErrorMessage = "「" & pname & "」はユニットに乗っていません"
						Error(0)
					End If
				Else
					EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
					Error(0)
				End If
				
				iname = GetArgAsString(3)
				
				With u
					If IsNumeric(iname) Then
						inumber = CShort(iname)
						If inumber < 1 Then
							EventErrorMessage = "指定されたアイテム番号「" & iname & "」が不正です"
							Error(0)
						End If
						If inumber > .CountItem Then
							EventErrorMessage = "指定されたユニットは" & VB6.Format(.CountItem) & "個のアイテムしか持っていません"
							Error(0)
						End If
						
						With .Item(inumber)
							If .IsFeatureAvailable("ユニット画像") Then
								item_with_image = True
							End If
							
							u.DeleteItem(.ID)
							
							If item_with_image Then
								With u
									.BitmapID = MakeUnitBitmap(u)
									For j = 1 To .CountOtherForm
										.OtherForm(j).BitmapID = 0
									Next 
									If .Status_Renamed = "出撃" Then
										If Not IsPictureVisible And MapFileName <> "" Then
											PaintUnitBitmap(u)
										End If
									End If
								End With
							End If
							
							'UPGRADE_NOTE: オブジェクト u.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
							.Unit_Renamed = Nothing
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
							If (.Name = iname Or .ID = iname) And .Exist Then
								If .IsFeatureAvailable("ユニット画像") Then
									item_with_image = True
								End If
								
								u.DeleteItem(.ID)
								
								If item_with_image Then
									With u
										.BitmapID = MakeUnitBitmap(u)
										For j = 1 To .CountOtherForm
											.OtherForm(j).BitmapID = 0
										Next 
										If .Status_Renamed = "出撃" Then
											If Not IsPictureVisible And MapFileName <> "" Then
												PaintUnitBitmap(u)
											End If
										End If
									End With
								End If
								
								'UPGRADE_NOTE: オブジェクト u.Item().Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
								.Unit_Renamed = Nothing
								.Exist = False
								Exit For
							End If
						End With
					Next 
				End With
				
			Case Else
				EventErrorMessage = "RemoveItemコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExecRemoveItemCmd = LineNum + 1
	End Function
	
	Private Function ExecRemovePilotCmd() As Integer
		Dim pname As String
		Dim i, num As Short
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
						Error(0)
					End If
					If .Status_Renamed = "出撃" Then
						.Escape(opt)
					End If
					For i = 1 To .CountPilot
						.Pilot(i).Alive = False
					Next 
					For i = 1 To .CountSupport
						.Support(i).Alive = False
					Next 
					.Status_Renamed = "破棄"
					For i = 1 To .CountOtherForm
						If .OtherForm(i).Status_Renamed = "他形態" Then
							.OtherForm(i).Status_Renamed = "破棄"
						End If
					Next 
				End With
				
			Case 2
				pname = GetArgAsString(2)
				If Not PList.IsDefined(pname) Then
					EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
					Error(0)
				End If
				
				p = PList.Item(pname)
				p.Alive = False
				
				If Not p.Unit_Renamed Is Nothing Then
					With p.Unit_Renamed
						If p.ID = .MainPilot.ID Or p.ID = .Pilot(1).ID Then
							'メインパイロットの場合はパイロット＆サポートを全員削除
							'ユニットも削除する
							If .Status_Renamed = "出撃" Or .Status_Renamed = "格納" Then
								.Escape(opt)
							End If
							For i = 1 To .CountPilot
								.Pilot(i).Alive = False
							Next 
							For i = 1 To .CountSupport
								.Support(i).Alive = False
							Next 
							.Status_Renamed = "破棄"
							For i = 1 To .CountOtherForm
								If .OtherForm(i).Status_Renamed = "他形態" Then
									.OtherForm(i).Status_Renamed = "破棄"
								End If
							Next 
						Else
							'メインパイロットが対象でなければ指定されたパイロットのみを削除
							For i = 1 To .CountPilot
								If p.ID = .Pilot(i).ID Then
									.DeletePilot(i)
									'UPGRADE_NOTE: オブジェクト p.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
									p.Unit_Renamed = Nothing
									Exit Function
								End If
							Next 
							For i = 1 To .CountSupport
								If p.ID = .Support(i).ID Then
									.DeleteSupport(i)
									'UPGRADE_NOTE: オブジェクト p.Unit をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
									p.Unit_Renamed = Nothing
									Exit Function
								End If
							Next 
						End If
					End With
				End If
				
			Case Else
				EventErrorMessage = "RemovePilotの引数の数が違います"
				Error(0)
		End Select
	End Function
	
	Private Function ExecRemoveUnitCmd() As Integer
		Dim uname As String
		Dim u As Unit
		Dim i, num As Short
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
					.Escape(opt)
					If .CountPilot > 0 Then
						.Pilot(1).GetOff()
					End If
					.Status_Renamed = "破棄"
					For i = 1 To .CountOtherForm
						If .OtherForm(i).Status_Renamed = "他形態" Then
							.OtherForm(i).Status_Renamed = "破棄"
						End If
					Next 
				End With
			Case 2
				uname = GetArgAsString(2)
				u = UList.Item(uname)
				
				'ユニットが存在しなければそのまま終了
				If u Is Nothing Then
					ExecRemoveUnitCmd = LineNum + 1
					Exit Function
				End If
				
				'ユニットＩＤで指定された場合
				If u.ID = uname Then
					With u
						.Escape(opt)
						If .CountPilot > 0 Then
							.Pilot(1).GetOff()
						End If
						.Status_Renamed = "破棄"
						For i = 1 To .CountOtherForm
							If .OtherForm(i).Status_Renamed = "他形態" Then
								.OtherForm(i).Status_Renamed = "破棄"
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
				For	Each u In UList
					With u.CurrentForm
						If .Name = uname And .Status_Renamed <> "破棄" Then
							If .CountPilot = 0 Then
								.Escape(opt)
								.Status_Renamed = "破棄"
								For i = 1 To .CountOtherForm
									If .OtherForm(i).Status_Renamed = "他形態" Then
										.OtherForm(i).Status_Renamed = "破棄"
									End If
								Next 
								ExecRemoveUnitCmd = LineNum + 1
								Exit Function
							End If
						End If
					End With
				Next u
				
				'見つからなければパイロットが乗っているユニットを削除
				For	Each u In UList
					With u.CurrentForm
						If .Name = uname And .Status_Renamed <> "破棄" Then
							.Escape(opt)
							.Pilot(1).GetOff()
							.Status_Renamed = "破棄"
							For i = 1 To .CountOtherForm
								If .OtherForm(i).Status_Renamed = "他形態" Then
									.OtherForm(i).Status_Renamed = "破棄"
								End If
							Next 
							ExecRemoveUnitCmd = LineNum + 1
							Exit Function
						End If
					End With
				Next u
			Case Else
				EventErrorMessage = "RemoveUnitの引数の数が違います"
				Error(0)
		End Select
		
		ExecRemoveUnitCmd = LineNum + 1
	End Function
	
	Private Function ExecRenameBGMCmd() As Integer
		Dim bname, vname As String
		
		If ArgNum <> 3 Then
			EventErrorMessage = "RenameBGMの引数の数が違います"
			Error(0)
		End If
		
		bname = GetArgAsString(2)
		Select Case bname
			Case "Map1", "Map2", "Map3", "Map4", "Map5", "Map6", "Briefing", "Intermission", "Subtitle", "End", "default"
				vname = "BGM(" & bname & ")"
			Case Else
				EventErrorMessage = "BGM名が不正です"
				Error(0)
		End Select
		
		If Not IsGlobalVariableDefined(vname) Then
			DefineGlobalVariable(vname)
		End If
		SetVariableAsString(vname, GetArgAsString(3))
		
		ExecRenameBGMCmd = LineNum + 1
	End Function
	
	Private Function ExecRenameFileCmd() As Integer
		Dim fname1, fname2 As String
		
		If ArgNum <> 3 Then
			EventErrorMessage = "RenameFileコマンドの引数の数が違います"
			Error(0)
		End If
		
		fname1 = ScenarioPath & GetArgAsString(2)
		fname2 = ScenarioPath & GetArgAsString(3)
		
		If InStr(fname1, "..\") > 0 Then
			EventErrorMessage = "ファイル指定に「..\」は使えません"
			Error(0)
		End If
		If InStr(fname1, "../") > 0 Then
			EventErrorMessage = "ファイル指定に「../」は使えません"
			Error(0)
		End If
		If InStr(fname2, "..\") > 0 Then
			EventErrorMessage = "ファイル指定に「..\」は使えません"
			Error(0)
		End If
		If InStr(fname2, "../") > 0 Then
			EventErrorMessage = "ファイル指定に「../」は使えません"
			Error(0)
		End If
		
		If Not FileExists(fname1) Then
			EventErrorMessage = "元のファイル" & "「" & fname1 & "」が見つかりません"
			Error(0)
		End If
		If FileExists(fname2) Then
			EventErrorMessage = "既に" & "「" & fname2 & "」が存在しています"
			Error(0)
		End If
		
		Rename(fname1, fname2)
		
		ExecRenameFileCmd = LineNum + 1
	End Function
	
	Private Function ExecRenameTermCmd() As Integer
		Dim tname, vname As String
		
		If ArgNum <> 3 Then
			EventErrorMessage = "RenameTermの引数の数が違います"
			Error(0)
		End If
		
		tname = GetArgAsString(2)
		Select Case tname
			Case "HP", "EN", "SP", "CT"
				vname = "ShortTerm(" & tname & ")"
			Case Else
				vname = "Term(" & tname & ")"
		End Select
		
		If Not IsGlobalVariableDefined(vname) Then
			DefineGlobalVariable(vname)
		End If
		SetVariableAsString(vname, GetArgAsString(3))
		
		ExecRenameTermCmd = LineNum + 1
	End Function
	
	Private Function ExecReplacePilotCmd() As Integer
		Dim pname As String
		Dim p1, p2 As Pilot
		Dim i As Short
		Dim is_support As Boolean
		
		If ArgNum <> 3 Then
			EventErrorMessage = "ReplacePilotの引数の数が違います"
			Error(0)
		End If
		
		p1 = GetArgAsPilot(2)
		
		pname = GetArgAsString(3)
		If Not PDList.IsDefined(pname) Then
			EventErrorMessage = "パイロット名が間違っています"
			Error(0)
		End If
		p2 = PList.Add(pname, p1.Level, (p1.Party))
		
		With p2
			.FullRecover()
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
		If Not p1.Unit_Renamed Is Nothing Then
			With p1.Unit_Renamed
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
					.ReplaceSupport(p2, (p1.ID))
				Else
					.ReplacePilot(p2, (p1.ID))
				End If
			End With
			PList.UpdateSupportMod(p1.Unit_Renamed)
		End If
		
		p1.Alive = False
		
		ExecReplacePilotCmd = LineNum + 1
	End Function
	
	Private Function ExecRequireCmd() As Integer
		Dim fname As String
		Dim file_head As Integer
		Dim i As Integer
		Dim buf As String
		
		If ArgNum <> 2 Then
			EventErrorMessage = "Requireコマンドの引数の数が違います"
			Error(0)
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
			Error(0)
		End If
		
		'ファイルをロード
		file_head = UBound(EventData) + 1
		LoadEventData2(fname, UBound(EventData))
		
		'エラー表示用にサイズを大きく取っておく
		ReDim Preserve EventData(UBound(EventData) + 1)
		ReDim Preserve EventLineNum(UBound(EventData))
		EventData(UBound(EventData)) = ""
		EventLineNum(UBound(EventData)) = EventLineNum(UBound(EventData) - 1) + 1
		
		'複数行に分割されたコマンドを結合
		For i = file_head To UBound(EventData) - 1
			If Right(EventData(i), 1) = "_" Then
				EventData(i + 1) = Left(EventData(i), Len(EventData(i)) - 1) & EventData(i + 1)
				EventData(i) = " "
			End If
		Next 
		
		'ラベルを登録
		For i = file_head To UBound(EventData)
			buf = EventData(i)
			If Right(buf, 1) = ":" Then
				AddLabel(Left(buf, Len(buf) - 1), i)
			End If
		Next 
		
		'コマンドデータ配列を設定
		If UBound(EventData) > UBound(EventCmd) Then
			ReDim Preserve EventCmd(UBound(EventData))
			i = UBound(EventData)
			Do While EventCmd(i) Is Nothing
				EventCmd(i) = New CmdData
				EventCmd(i).LineNum = i
				i = i - 1
			Loop 
		End If
		For i = file_head To UBound(EventData)
			EventCmd(i).Name = Event_Renamed.CmdType.NullCmd
		Next 
		
		'読み込んだイベントファイルを記録
		ReDim Preserve AdditionalEventFileNames(UBound(AdditionalEventFileNames) + 1)
		AdditionalEventFileNames(UBound(AdditionalEventFileNames)) = GetArgAsString(2)
	End Function
	
	Private Function ExecRestoreEventCmd() As Integer
		If ArgNum <> 2 Then
			EventErrorMessage = "RestoreEventコマンドの引数の数が違います"
			Error(0)
		End If
		
		RestoreLabel(GetArgAsString(2))
		
		ExecRestoreEventCmd = LineNum + 1
	End Function
	
	Private Function ExecRideCmd() As Integer
		Dim p As Pilot
		Dim u As Unit
		Dim uname As String
		Dim i As Short
		
		p = GetArgAsPilot(2)
		
		Select Case ArgNum
			Case 3
				uname = GetArgAsString(3)
				
				'指定したユニットに既に乗っている場合は何もしない
				If Not p.Unit_Renamed Is Nothing Then
					With p.Unit_Renamed
						If .Name = uname Or .ID = uname Then
							ExecRideCmd = LineNum + 1
							Exit Function
						End If
					End With
				End If
				
				p.GetOff()
				
				u = UList.Item(uname)
				If u Is Nothing Then
					EventErrorMessage = "ユニット名が間違っています"
					Error(0)
				End If
				
				'ユニットＩＤで指定された場合
				If u.ID = uname Then
					p.Ride(u.CurrentForm)
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
				For	Each u In UList
					With u
						If .Name = uname And .Party0 = p.Party And .Status_Renamed <> "破棄" Then
							If p.IsSupport(u) And Not .IsFeatureAvailable("ダミーユニット") Then
								p.Ride(.CurrentForm)
								ExecRideCmd = LineNum + 1
								Exit Function
							End If
							If .CurrentForm.CountPilot < System.Math.Abs(.Data.PilotNum) Then
								p.Ride(.CurrentForm)
								ExecRideCmd = LineNum + 1
								Exit Function
							End If
						End If
					End With
				Next u
				
				'空きがなければ今までRideコマンドで指定されてないユニットに乗り込む
				For	Each u In UList
					With u
						If .Name = uname And .Party0 = p.Party And .Status_Renamed <> "破棄" Then
							If .CurrentForm.CountPilot > 0 Then
								'今までにRideコマンドで指定されているか判定
								For i = 1 To UBound(LastPilotID) - 1
									If .CurrentForm.MainPilot.ID = LastPilotID(i) Then
										GoTo NextUnit
									End If
								Next 
								.CurrentForm.Pilot(1).GetOff(True)
							End If
							p.Ride(.CurrentForm)
							ExecRideCmd = LineNum + 1
							Exit Function
						End If
					End With
NextUnit: 
				Next u
				
				'それでも見つからなければ無差別で……
				For	Each u In UList
					With u
						If .Name = uname And .Party0 = p.Party And .Status_Renamed <> "破棄" Then
							If .CurrentForm.CountPilot > 0 Then
								.CurrentForm.Pilot(1).GetOff(True)
							End If
							p.Ride(.CurrentForm)
							'乗り込み履歴を初期化
							ReDim LastPilotID(1)
							LastPilotID(1) = p.ID
							ExecRideCmd = LineNum + 1
							Exit Function
						End If
					End With
				Next u
				
				EventErrorMessage = p.Name & "が乗り込むための" & uname & "が存在しません"
				Error(0)
			Case 2
				'指定したユニットに既に乗っている場合は何もしない
				If p.Unit_Renamed Is SelectedUnitForEvent Then
					ExecRideCmd = LineNum + 1
					Exit Function
				End If
				
				With SelectedUnitForEvent
					If .CountPilot = System.Math.Abs(.Data.PilotNum) And Not p.IsSupport(SelectedUnitForEvent) Then
						' MOD START マージ
						'                    .Pilot(1).GetOff
						.Pilot(1).GetOff(True)
						' MOD END マージ
					End If
				End With
				p.GetOff()
				p.Ride(SelectedUnitForEvent)
			Case Else
				EventErrorMessage = "Rideコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExecRideCmd = LineNum + 1
	End Function
	
	Private Function ExecSaveDataCmd() As Integer
		Dim fname, save_path As String
		Dim ret As Short
		
		If ArgNum = 2 Then
			fname = GetArgAsString(2)
		ElseIf ArgNum = 1 Then 
			'一旦「常に手前に表示」を解除
			If frmListBox.Visible Then
				ret = SetWindowPos(frmListBox.Handle.ToInt32, -2, 0, 0, 0, 0, &H3)
			End If
			
			fname = SaveFileDialog("データセーブ", ScenarioPath, GetValueAsString("セーブデータファイル名"), 2, "ｾｰﾌﾞﾃﾞｰﾀ", "src")
			
			'再び「常に手前に表示」
			If frmListBox.Visible Then
				ret = SetWindowPos(frmListBox.Handle.ToInt32, -1, 0, 0, 0, 0, &H3)
			End If
			
			'キャンセル？
			If fname = "" Then
				ExecSaveDataCmd = LineNum + 1
				Exit Function
			End If
			
			'セーブ先はシナリオフォルダ？
			If InStr(fname, "\") > 0 Then
				save_path = Left(fname, InStr2(fname, "\"))
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Dir(save_path) <> Dir(ScenarioPath) Then
				If MsgBox("セーブファイルはシナリオフォルダにないと読み込めません。" & vbCr & vbLf & "このままセーブしますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question) <> 1 Then
					ExecSaveDataCmd = LineNum + 1
					Exit Function
				End If
			End If
		Else
			EventErrorMessage = "SaveDataコマンドの引数の数が違います"
			Error(0)
		End If
		
		If fname <> "" Then
			UList.Update() '追加パイロットを消去
			SaveData(fname)
		End If
		
		ExecSaveDataCmd = LineNum + 1
	End Function
	
	Private Function ExecSelectCmd() As Integer
		If ArgNum <> 2 Then
			EventErrorMessage = "Selectコマンドの引数の数が違います"
			Error(0)
		End If
		
		SelectedUnitForEvent = GetArgAsUnit(2)
		
		ExecSelectCmd = LineNum + 1
	End Function
	
	Private Function ExecSelectTargetCmd() As Integer
		Dim pname As String
		
		If ArgNum <> 2 Then
			EventErrorMessage = "SelectTargetコマンドの引数の数が違います"
			Error(0)
		End If
		
		SelectedTargetForEvent = GetArgAsUnit(2)
		
		ExecSelectTargetCmd = LineNum + 1
	End Function
	
	Private Function ExecSepiaCmd() As Integer
		Dim prev_x, prev_y As Short
		Dim u As Unit
		Dim late_refresh As Boolean
		Dim i As Short
		Dim buf As String
		
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
					EventErrorMessage = "Sepiaコマンドに不正なオプション「" & buf & "」が使われています"
					Error(0)
			End Select
		Next 
		
		prev_x = MapX
		prev_y = MapY
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		SetupBackground("セピア", "非同期")
		
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Then
					If .BitmapID = 0 Then
						With UList.Item(.Name)
							If u.Party0 = .Party0 And .BitmapID <> 0 And u.Bitmap = .Bitmap And Not .IsFeatureAvailable("ダミーユニット") Then
								u.BitmapID = .BitmapID
							Else
								u.BitmapID = MakeUnitBitmap(u)
							End If
						End With
					End If
				End If
			End With
		Next u
		
		Center(prev_x, prev_y)
		RedrawScreen(late_refresh)
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ExecSepiaCmd = LineNum + 1
	End Function
	
	Private Function ExecSetCmd() As Integer
		Dim etype As Expression.ValueType
		Dim str_result As String
		Dim num_result As Double
		Dim num As Short
		
		num = ArgNum
		If num > 3 Then
			'過去のバージョンのシナリオを読み込めるようにするため、
			'Setコマンドの後ろの「#」形式のコメントは無視する
			If Left(GetArg(4), 1) = "#" Then
				num = 3
			Else
				EventErrorMessage = "Setコマンドの引数の数が違います"
				Error(0)
			End If
		End If
		
		Select Case num
			Case 2
				SetVariableAsLong(GetArg(2), 1)
			Case 3
				Select Case ArgsType(3)
					Case Expression.ValueType.UndefinedType
						etype = EvalTerm(strArgs(3), Expression.ValueType.UndefinedType, str_result, num_result)
						If etype = Expression.ValueType.NumericType Then
							SetVariableAsDouble(GetArg(2), num_result)
						Else
							SetVariableAsString(GetArg(2), str_result)
						End If
					Case Expression.ValueType.StringType
						SetVariableAsString(GetArg(2), strArgs(3))
					Case Expression.ValueType.NumericType
						SetVariableAsDouble(GetArg(2), dblArgs(3))
				End Select
		End Select
		
		ExecSetCmd = LineNum + 1
	End Function
	
	Private Function ExecSetBulletCmd() As Integer
		Dim wname As String
		Dim wid, num As Short
		Dim u As Unit
		
		Select Case ArgNum
			Case 4
				u = GetArgAsUnit(2)
				wname = GetArgAsString(3)
				num = GetArgAsLong(4)
				
			Case 3
				u = SelectedUnitForEvent
				wname = GetArgAsString(2)
				num = GetArgAsLong(3)
				
			Case Else
				EventErrorMessage = "SetBulletコマンドの引数の数が違います"
				Error(0)
		End Select
		
		With u
			If IsNumeric(wname) Then
				wid = StrToLng(wname)
				If wid < 1 Or .CountWeapon < wid Then
					EventErrorMessage = "武器の番号「" & wname & "」が間違っています"
					Error(0)
				End If
			Else
				For wid = 1 To .CountWeapon
					If .Weapon(wid).Name = wname Then
						Exit For
					End If
				Next 
				If wid < 1 Or .CountWeapon < wid Then
					EventErrorMessage = .Name & "は武器「" & wname & "」を持っていません"
					Error(0)
				End If
			End If
			.SetBullet(wid, MinLng(num, .MaxBullet(wid)))
		End With
		
		ExecSetBulletCmd = LineNum + 1
	End Function
	
	Private Function ExecSetMessageCmd() As Integer
		Dim u As Unit
		Dim pname, pname0 As String
		Dim sit As String
		Dim selected_msg As String
		
		Select Case ArgNum
			Case 4
				pname = GetArgAsString(2)
				u = UList.Item2(pname)
				If u Is Nothing Then
					With PList
						If Not .IsDefined(pname) Then
							pname0 = pname
							If InStr(pname0, "(") > 0 Then
								pname0 = Left(pname0, InStr2(pname0, "(") - 1)
							End If
							If Not .IsDefined(pname0) Then
								EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
								Error(0)
							End If
							u = .Item(pname0).Unit_Renamed
						Else
							u = .Item(pname).Unit_Renamed
						End If
					End With
					If u Is Nothing Then
						EventErrorMessage = "「" & pname & "」はユニットに乗っていません"
						Error(0)
					End If
				ElseIf u.CountPilot = 0 Then 
					EventErrorMessage = "指定されたユニットにはパイロットが乗っていません"
					Error(0)
				End If
				sit = GetArgAsString(3)
				selected_msg = GetArgAsString(4)
			Case 3
				u = SelectedUnitForEvent
				If u.CountPilot = 0 Then
					EventErrorMessage = "指定されたユニットにはパイロットが乗っていません"
					Error(0)
				End If
				sit = GetArgAsString(2)
				selected_msg = GetArgAsString(3)
			Case Else
				EventErrorMessage = "SetMessageコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If selected_msg = "解除" Then
			'メッセージ用変数を削除
			UndefineVariable("Message(" & u.MainPilot.ID & "," & sit & ")")
		ElseIf pname0 <> "" Then 
			'表情指定付きメッセージをローカル変数として登録する
			SetVariableAsString("Message(" & u.MainPilot.ID & "," & sit & ")", pname & "::" & selected_msg)
		Else
			'メッセージをローカル変数として登録する
			SetVariableAsString("Message(" & u.MainPilot.ID & "," & sit & ")", selected_msg)
		End If
		
		ExecSetMessageCmd = LineNum + 1
	End Function
	
	Private Function ExecSetRelationCmd() As Integer
		Dim pname1, pname2 As String
		Dim vname As String
		Dim rel As Short
		
		Select Case ArgNum
			Case 3
				pname1 = SelectedUnitForEvent.MainPilot.Name
				pname2 = GetArgAsString(2)
				If Not PDList.IsDefined(pname2) Then
					EventErrorMessage = "キャラクター名が間違っています"
					Error(0)
				End If
				pname2 = PDList.Item(pname2).Name
				
				rel = GetArgAsLong(3)
			Case 4
				pname1 = GetArgAsString(2)
				If Not PDList.IsDefined(pname1) Then
					EventErrorMessage = "キャラクター名が間違っています"
					Error(0)
				End If
				pname1 = PDList.Item(pname1).Name
				
				pname2 = GetArgAsString(3)
				If Not PDList.IsDefined(pname2) Then
					EventErrorMessage = "キャラクター名が間違っています"
					Error(0)
				End If
				pname2 = PDList.Item(pname2).Name
				
				rel = GetArgAsLong(4)
			Case Else
				EventErrorMessage = "SetRelationコマンドの引数の数が違います"
				Error(0)
		End Select
		
		vname = "関係:" & pname1 & ":" & pname2
		
		If rel <> 0 Then
			If Not IsGlobalVariableDefined(vname) Then
				DefineGlobalVariable(vname)
			End If
			SetVariableAsLong(vname, rel)
		Else
			If IsGlobalVariableDefined(vname) Then
				UndefineVariable(vname)
			End If
		End If
		
		'信頼度補正による気力修正を更新
		If IsOptionDefined("信頼度補正") Then
			If PList.IsDefined(pname1) Then
				PList.Item(pname1).UpdateSupportMod()
			End If
			If PList.IsDefined(pname2) Then
				PList.Item(pname2).UpdateSupportMod()
			End If
		End If
		
		ExecSetRelationCmd = LineNum + 1
	End Function
	
	Private Function ExecSetSkillCmd() As Integer
		Dim pname As String
		Dim vname As String
		Dim slist As String
		Dim sname As String
		Dim sname_array() As String
		Dim slevel As Double
		Dim slevel_array() As Double
		Dim sdata As String
		Dim sdata_array() As String
		Dim i, j As Short
		
		If ArgNum <> 4 And ArgNum <> 5 Then
			EventErrorMessage = "SetSkillコマンドの引数の数が違います"
			Error(0)
		End If
		
		pname = GetArgAsString(2)
		If PList.IsDefined(pname) Then
			pname = PList.Item(pname).ID
		ElseIf PDList.IsDefined(pname) Then 
			pname = PDList.Item(pname).Name
		Else
			EventErrorMessage = "「" & pname & "」というパイロットが見つかりません"
			Error(0)
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
							sdata_array(i) = Trim(sdata & " " & ListTail(.AliasData(i), 2))
						End If
						
						If .AliasLevelIsPlusMod(i) Or .AliasLevelIsMultMod(i) Then
							sdata_array(i) = LIndex(sdata_array(i), 1) & "Lv" & VB6.Format(slevel) & " " & ListTail(sdata_array(i), 2)
							sdata_array(i) = Trim(sdata_array(i))
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
				DefineGlobalVariable("Ability(" & pname & ")")
				slist = sname
			Else
				'UPGRADE_WARNING: オブジェクト GlobalVariableList.Item().StringValue の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
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
			SetVariableAsString("Ability(" & pname & ")", slist)
			
			'今回SetSkillが適用された能力sname用変数を作成
			vname = "Ability(" & pname & "," & sname & ")"
			If Not IsGlobalVariableDefined(vname) Then
				DefineGlobalVariable(vname)
			End If
			
			If sdata <> "" Then
				'別名指定があった場合
				SetVariableAsString(vname, VB6.Format(slevel) & " " & sdata)
				
				'必要技能用
				If sdata <> "非表示" And LIndex(sdata, 1) <> "解説" Then
					vname = "Ability(" & pname & "," & LIndex(sdata, 1) & ")"
					If Not IsGlobalVariableDefined(vname) Then
						DefineGlobalVariable(vname)
					End If
					SetVariableAsString(vname, VB6.Format(slevel))
				End If
			Else
				SetVariableAsString(vname, VB6.Format(slevel))
			End If
NextSkill: 
		Next 
		
		'パイロットやユニットのステータスをアップデート
		If PList.IsDefined(pname) Then
			With PList.Item(pname)
				.Update()
				If Not .Unit_Renamed Is Nothing Then
					.Unit_Renamed.Update()
					If .Unit_Renamed.Status_Renamed = "出撃" Then
						PList.UpdateSupportMod(.Unit_Renamed)
					End If
				End If
			End With
		End If
		
		ExecSetSkillCmd = LineNum + 1
	End Function
	
	Private Function ExecSetStatusCmd() As Integer
		Dim u As Unit
		Dim cname As String
		
		Select Case ArgNum
			Case 4
				u = GetArgAsUnit(2)
				With u
					cname = GetArgAsString(3)
					.AddCondition(cname, GetArgAsLong(4))
					If .Status_Renamed = "出撃" Then
						PaintUnitBitmap(u)
					End If
					If cname <> "非操作" Then
						.Update()
					End If
				End With
			Case 3
				If Not SelectedUnitForEvent Is Nothing Then
					With SelectedUnitForEvent
						cname = GetArgAsString(2)
						.AddCondition(cname, GetArgAsLong(3))
						If .Status_Renamed = "出撃" Then
							PaintUnitBitmap(SelectedUnitForEvent)
						End If
						If cname <> "非操作" Then
							.Update()
						End If
					End With
				End If
			Case Else
				EventErrorMessage = "SetStatusコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExecSetStatusCmd = LineNum + 1
	End Function
	
	'ADD START 240a
	Private Function ExecSetStatusStringColor() As Integer
		Dim cname, opt, target As String
		Dim color As Integer
		'引数チェック
		If ArgNum <> 3 Then
			EventErrorMessage = "StatusStringColorコマンドの引数の数が違います"
			Error(0)
		End If
		
		'変更色を取得
		opt = GetArgAsString(2)
		If Asc(opt) <> 35 Or Len(opt) <> 7 Then
			EventErrorMessage = "色指定が不正です"
			Error(0)
		End If
		cname = New String(vbNullChar, 8)
		Mid(cname, 1, 2) = "&H"
		Mid(cname, 3, 2) = Mid(opt, 6, 2)
		Mid(cname, 5, 2) = Mid(opt, 4, 2)
		Mid(cname, 7, 2) = Mid(opt, 2, 2)
		If Not IsNumeric(cname) Then
			EventErrorMessage = "色指定が不正です"
			Error(0)
		End If
		color = CInt(cname)
		
		'変更対象を取得
		target = GetArgAsString(3)
		If target <> "通常" And target <> "能力名" And target <> "有効" And target <> "無効" Then
			EventErrorMessage = "設定対象の指定が不正です"
			Error(0)
		End If
		
		'処理実行
		Select Case target
			Case "通常"
				StatusFontColorNormalString = color
				'Global変数に保存
				If Not IsGlobalVariableDefined("StatusWindow(StringColor)") Then
					DefineGlobalVariable("StatusWindow(StringColor)")
				End If
				SetVariableAsLong("StatusWindow(StringColor)", color)
			Case "能力名"
				StatusFontColorAbilityName = color
				'Global変数に保存
				If Not IsGlobalVariableDefined("StatusWindow(ANameColor)") Then
					DefineGlobalVariable("StatusWindow(ANameColor)")
				End If
				SetVariableAsLong("StatusWindow(ANameColor)", color)
			Case "有効"
				StatusFontColorAbilityEnable = color
				'Global変数に保存
				If Not IsGlobalVariableDefined("StatusWindow(EnableColor)") Then
					DefineGlobalVariable("StatusWindow(EnableColor)")
				End If
				SetVariableAsLong("StatusWindow(EnableColor)", color)
			Case "無効"
				StatusFontColorAbilityDisable = color
				'Global変数に保存
				If Not IsGlobalVariableDefined("StatusWindow(DisableColor)") Then
					DefineGlobalVariable("StatusWindow(DisableColor)")
				End If
				SetVariableAsLong("StatusWindow(DisableColor)", color)
		End Select
		
		ExecSetStatusStringColor = LineNum + 1
		
	End Function
	'ADD  END  240a
	
	Private Function ExecSetStockCmd() As Integer
		Dim aname As String
		Dim aid, num As Short
		Dim u As Unit
		
		Select Case ArgNum
			Case 4
				u = GetArgAsUnit(2)
				aname = GetArgAsString(3)
				num = GetArgAsLong(4)
				
			Case 3
				u = SelectedUnitForEvent
				aname = GetArgAsString(2)
				num = GetArgAsLong(3)
				
			Case Else
				EventErrorMessage = "SetStockコマンドの引数の数が違います"
				Error(0)
		End Select
		
		With u
			If IsNumeric(aname) Then
				aid = StrToLng(aname)
				If aid < 1 Or .CountAbility < aid Then
					EventErrorMessage = "アビリティの番号「" & aname & "」が間違っています"
					Error(0)
				End If
			Else
				For aid = 1 To .CountAbility
					If .Ability(aid).Name = aname Then
						Exit For
					End If
				Next 
				If aid < 1 Or .CountAbility < aid Then
					EventErrorMessage = .Name & "はアビリティ「" & aname & "」を持っていません"
					Error(0)
				End If
			End If
			.SetStock(aid, MinLng(num, .MaxStock(aid)))
		End With
		
		ExecSetStockCmd = LineNum + 1
	End Function
	'ADD START 240a
	Private Function ExecSetWindowColor() As Integer
		Dim opt, cname, target As String
		Dim color As Integer
		Dim isTargetLine, isTargetBG As Boolean
		
		'引数チェック
		If ArgNum <> 2 And ArgNum <> 3 Then
			EventErrorMessage = "SetWindowColorコマンドの引数の数が違います"
			Error(0)
		End If
		
		'色取得
		opt = GetArgAsString(2)
		If Asc(opt) <> 35 Or Len(opt) <> 7 Then
			EventErrorMessage = "色指定が不正です"
			Error(0)
		End If
		cname = New String(vbNullChar, 8)
		Mid(cname, 1, 2) = "&H"
		Mid(cname, 3, 2) = Mid(opt, 6, 2)
		Mid(cname, 5, 2) = Mid(opt, 4, 2)
		Mid(cname, 7, 2) = Mid(opt, 2, 2)
		If Not IsNumeric(cname) Then
			EventErrorMessage = "色指定が不正です"
			Error(0)
		End If
		color = CInt(cname)
		
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
				Error(0)
			End If
		End If
		
		'処理開始
		If isTargetLine Then
			StatusWindowFrameColor = color
			'Global変数に保存
			If Not IsGlobalVariableDefined("StatusWindow(FrameColor)") Then
				DefineGlobalVariable("StatusWindow(FrameColor)")
			End If
			SetVariableAsLong("StatusWindow(FrameColor)", color)
		ElseIf isTargetBG Then 
			StatusWindowBackBolor = color
			'Global変数に保存
			If Not IsGlobalVariableDefined("StatusWindow(BackBolor)") Then
				DefineGlobalVariable("StatusWindow(BackBolor)")
			End If
			SetVariableAsLong("StatusWindow(BackBolor)", color)
		ElseIf Not isTargetLine And Not isTargetBG Then 
			StatusWindowFrameColor = color
			'Global変数に保存
			If Not IsGlobalVariableDefined("StatusWindow(FrameColor)") Then
				DefineGlobalVariable("StatusWindow(FrameColor)")
			End If
			SetVariableAsLong("StatusWindow(FrameColor)", color)
			StatusWindowBackBolor = color
			'Global変数に保存
			If Not IsGlobalVariableDefined("StatusWindow(BackBolor)") Then
				DefineGlobalVariable("StatusWindow(BackBolor)")
			End If
			SetVariableAsLong("StatusWindow(BackBolor)", color)
		End If
		
		ExecSetWindowColor = LineNum + 1
		
	End Function
	
	Private Function ExecSetWindowFrameWidth() As Integer
		Dim width As Integer
		'引数チェック
		If ArgNum <> 2 Then
			EventErrorMessage = "SetWindowColorコマンドの引数の数が違います"
			Error(0)
		End If
		'幅取得
		width = GetArgAsLong(2)
		'処理開始
		StatusWindowFrameWidth = width
		'Global変数に保存
		If Not IsGlobalVariableDefined("StatusWindow(FrameWidth)") Then
			DefineGlobalVariable("StatusWindow(FrameWidth)")
		End If
		SetVariableAsLong("StatusWindow(FrameWidth)", width)
		
		ExecSetWindowFrameWidth = LineNum + 1
	End Function
	'ADD  END
	
	Private Function ExecShowCmd() As Integer
		With MainForm
			If Not .Visible Then
				.Show()
				.Refresh()
				System.Windows.Forms.Application.DoEvents()
			End If
		End With
		
		If Not IsPictureVisible Then
			RedrawScreen()
		End If
		
		ExecShowCmd = LineNum + 1
	End Function
	
	'互換性維持のために残している
	Private Function ExecShowImageCmd() As Integer
		Dim fname As String
		Dim dw, dh As Integer
		Dim ret As Short
		
		fname = GetArgAsString(2)
		Select Case Right(LCase(fname), 4)
			Case ".bmp", ".jpg", ".gif", ".png"
				'正しい画像ファイル名
			Case Else
				EventErrorMessage = "不正な画像ファイル名「" & fname & "」が指定されています"
				Error(0)
		End Select
		
		If ArgNum > 2 Then
			dw = GetArgAsLong(3)
			dh = GetArgAsLong(4)
		Else
			dw = DEFAULT_LEVEL
			dh = DEFAULT_LEVEL
		End If
		
		If Not MainForm.Visible Then
			MainForm.Show()
		End If
		
		ret = DrawPicture(fname, DEFAULT_LEVEL, DEFAULT_LEVEL, dw, dh, 0, 0, 0, 0, "")
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		MainForm.picMain(0).Refresh()
		
		ExecShowImageCmd = LineNum + 1
	End Function
	
	Private Function ExecShowUnitStatusCmd() As Integer
		Dim u As Unit
		
		Select Case ArgNum
			Case 1
				u = SelectedUnitForEvent
			Case 2
				If GetArgAsString(2) = "終了" Then
					ClearUnitStatus()
					ExecShowUnitStatusCmd = LineNum + 1
					Exit Function
				End If
				
				u = GetArgAsUnit(2)
			Case Else
				EventErrorMessage = "ShowUnitStatusコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not u Is Nothing Then
			DisplayUnitStatus(u)
		End If
		
		ExecShowUnitStatusCmd = LineNum + 1
	End Function
	
	Private Function ExecSkipCmd() As Integer
		Dim i As Integer
		Dim depth As Short
		
		'対応するループの末尾を探す
		depth = 1
		For i = LineNum + 1 To UBound(EventCmd)
			Select Case EventCmd(i).Name
				Case Event_Renamed.CmdType.DoCmd, Event_Renamed.CmdType.ForCmd, Event_Renamed.CmdType.ForEachCmd
					depth = depth + 1
				Case Event_Renamed.CmdType.LoopCmd, Event_Renamed.CmdType.NextCmd
					depth = depth - 1
					If depth = 0 Then
						ExecSkipCmd = i
						Exit Function
					End If
			End Select
		Next 
		
		EventErrorMessage = "Skipコマンドがループの外で使われています"
		Error(0)
	End Function
	
	Private Function ExecSortCmd() As Object
		Dim j, i, k As Short
		Dim isStringkey, isStringValue As Boolean
		Dim isSwap, isAscOrder, isKeySort As Boolean
		Dim vname, buf As String
		Dim value_buf As Object
		Dim num As Short
		Dim var As VarData
		Dim array_buf() As Object
		Dim var_buf(2) As Object
		
		'array_buf(opt, value)
		' opt=0…配列の添字
		'    =1…変数のValueTyep
		'    =2…変数の値
		
		If ArgNum < 2 Then
			EventErrorMessage = "Sortコマンドの引数の数が違います"
			Error(0)
		End If
		
		'初期値
		isAscOrder = True 'ソート順を昇順似設定
		isStringkey = False '配列のインデックスを数値として扱う
		isStringValue = False '配列の要素を数値として扱う
		isKeySort = False 'インデックスのみのソートではない
		
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
					EventErrorMessage = "Sortコマンドに不正なオプション「" & buf & "」が使われています"
					Error(0)
			End Select
		Next 
		
		'ソートする配列変数名
		vname = GetArg(2)
		If Left(vname, 1) = "$" Then
			vname = Mid(vname, 2)
		End If
		'Eval関数
		If LCase(Left(vname, 5)) = "eval(" Then
			If Right(vname, 1) = ")" Then
				vname = Mid(vname, 6, Len(vname) - 6)
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
						
						buf = Mid(.Name, InStr(.Name, "[") + 1, InStr2(.Name, "]") - InStr(.Name, "[") - 1)
						
						If Not IsNumeric(buf) Then
							isStringkey = True
						End If
						If .VariableType = Expression.ValueType.StringType Then
							'UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							value_buf = .StringValue
						Else
							'UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							value_buf = .NumericValue
						End If
						If Not IsNumeric(value_buf) Then
							isStringValue = True
						End If
						
						'UPGRADE_WARNING: オブジェクト array_buf(0, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						array_buf(0, num) = buf
						'UPGRADE_WARNING: オブジェクト array_buf(1, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						array_buf(1, num) = .VariableType
						'UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						'UPGRADE_WARNING: オブジェクト array_buf(2, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						array_buf(2, num) = value_buf
						
						num = num + 1
					End If
				End With
			Next 
			If num = 0 Then
				'UPGRADE_WARNING: オブジェクト ExecSortCmd の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				ExecSortCmd = LineNum + 1
				Exit Function
			End If
		ElseIf IsLocalVariableDefined(vname) Then 
			'ローカルな配列
			For	Each var In LocalVariableList
				With var
					If InStr(.Name, vname & "[") = 1 Then
						ReDim Preserve array_buf(2, num)
						
						buf = Mid(.Name, InStr(.Name, "[") + 1, InStr2(.Name, "]") - InStr(.Name, "[") - 1)
						
						If Not IsNumeric(buf) Then
							isStringkey = True
						End If
						If .VariableType = Expression.ValueType.StringType Then
							'UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							value_buf = .StringValue
						Else
							'UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							value_buf = .NumericValue
						End If
						If Not IsNumeric(value_buf) Then
							isStringValue = True
						End If
						
						'UPGRADE_WARNING: オブジェクト array_buf(0, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						array_buf(0, num) = buf
						'UPGRADE_WARNING: オブジェクト array_buf(1, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						array_buf(1, num) = .VariableType
						'UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						'UPGRADE_WARNING: オブジェクト array_buf(2, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						array_buf(2, num) = value_buf
						
						num = num + 1
					End If
				End With
			Next var
			If num = 0 Then
				'UPGRADE_WARNING: オブジェクト ExecSortCmd の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				ExecSortCmd = LineNum + 1
				Exit Function
			End If
		ElseIf IsGlobalVariableDefined(vname) Then 
			'グローバルな配列
			For	Each var In GlobalVariableList
				With var
					If InStr(.Name, vname & "[") = 1 Then
						ReDim Preserve array_buf(2, num)
						
						buf = Mid(.Name, InStr(.Name, "[") + 1, InStr2(.Name, "]") - InStr(.Name, "[") - 1)
						
						If Not IsNumeric(buf) Then
							isStringkey = True
						End If
						If .VariableType = Expression.ValueType.StringType Then
							'UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							value_buf = .StringValue
						Else
							'UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							value_buf = .NumericValue
						End If
						If Not IsNumeric(value_buf) Then
							isStringValue = True
						End If
						
						'UPGRADE_WARNING: オブジェクト array_buf(0, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						array_buf(0, num) = buf
						'UPGRADE_WARNING: オブジェクト array_buf(1, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						array_buf(1, num) = .VariableType
						'UPGRADE_WARNING: オブジェクト value_buf の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						'UPGRADE_WARNING: オブジェクト array_buf(2, num) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
						array_buf(2, num) = value_buf
						
						num = num + 1
					End If
				End With
			Next var
			If num = 0 Then
				'UPGRADE_WARNING: オブジェクト ExecSortCmd の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
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
							'UPGRADE_WARNING: オブジェクト array_buf(0, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							isSwap = IIf(StrComp(array_buf(0, i), array_buf(0, j), CompareMethod.Text) = 1, True, False)
						Else
							'UPGRADE_WARNING: オブジェクト array_buf(0, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							isSwap = IIf(StrComp(array_buf(0, i), array_buf(0, j), CompareMethod.Text) = -1, True, False)
						End If
					Else
						If isAscOrder Then
							'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							isSwap = IIf(CDbl(array_buf(0, i)) > CDbl(array_buf(0, j)), True, False)
						Else
							'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							isSwap = IIf(CDbl(array_buf(0, i)) < CDbl(array_buf(0, j)), True, False)
						End If
					End If
					
					If isSwap Then
						For k = 0 To 2
							'UPGRADE_WARNING: オブジェクト array_buf(k, i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト var_buf(k) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							var_buf(k) = array_buf(k, i)
							'UPGRADE_WARNING: オブジェクト array_buf(k, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト array_buf(k, i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							array_buf(k, i) = array_buf(k, j)
							'UPGRADE_WARNING: オブジェクト var_buf(k) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト array_buf(k, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
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
							'UPGRADE_WARNING: オブジェクト array_buf(2, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							isSwap = IIf(StrComp(array_buf(2, i), array_buf(2, j), CompareMethod.Text) = 1, True, False)
						Else
							'UPGRADE_WARNING: オブジェクト array_buf(2, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							isSwap = IIf(StrComp(array_buf(2, i), array_buf(2, j), CompareMethod.Text) = -1, True, False)
						End If
					Else
						If isAscOrder Then
							'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							isSwap = IIf(CDbl(array_buf(2, i)) > CDbl(array_buf(2, j)), True, False)
						Else
							'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							isSwap = IIf(CDbl(array_buf(2, i)) < CDbl(array_buf(2, j)), True, False)
						End If
					End If
					
					If isSwap Then
						For k = IIf(isStringkey, 0, 1) To 2
							'UPGRADE_WARNING: オブジェクト array_buf(k, i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト var_buf(k) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							var_buf(k) = array_buf(k, i)
							'UPGRADE_WARNING: オブジェクト array_buf(k, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト array_buf(k, i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							array_buf(k, i) = array_buf(k, j)
							'UPGRADE_WARNING: オブジェクト var_buf(k) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							'UPGRADE_WARNING: オブジェクト array_buf(k, j) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
							array_buf(k, j) = var_buf(k)
						Next 
					End If
					
				Next 
			Next 
		End If
		
		'SRC変数に再配置
		For i = 0 To num
			'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			buf = vname & "[" & CStr(array_buf(0, i)) & "]"
			UndefineVariable(buf)
			If array_buf(1, i) = Expression.ValueType.StringType Then
				'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				SetVariable(buf, Expression.ValueType.StringType, CStr(array_buf(2, i)), 0)
			Else
				'UPGRADE_WARNING: オブジェクト array_buf() の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
				SetVariable(buf, Expression.ValueType.NumericType, "", CDbl(array_buf(2, i)))
			End If
		Next 
		
		'UPGRADE_WARNING: オブジェクト ExecSortCmd の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		ExecSortCmd = LineNum + 1
	End Function
	
	Private Function ExecSpecialPowerCmd() As Integer
		Dim u, t As Unit
		Dim sname As String
		Dim sd As SpecialPowerData
		Dim need_target, msg_window_visible As Boolean
		Dim prev_action As Short
		
		Select Case ArgNum
			Case 4
				u = GetArgAsUnit(2)
				sname = GetArgAsString(3)
				t = GetArgAsUnit(4)
			Case 3
				If SPDList.IsDefined(GetArgAsString(2)) Then
					With SPDList.Item(GetArgAsString(2))
						If .IsEffectAvailable("みがわり") Or .IsEffectAvailable("挑発") Then
							need_target = True
						End If
					End With
				End If
				
				If need_target Then
					u = SelectedUnitForEvent
					sname = GetArgAsString(2)
					t = GetArgAsUnit(3)
				Else
					u = GetArgAsUnit(2)
					sname = GetArgAsString(3)
				End If
			Case 2
				u = SelectedUnitForEvent
				sname = GetArgAsString(2)
			Case Else
				EventErrorMessage = "SpecialPowerコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not SPDList.IsDefined(sname) Then
			EventErrorMessage = "SpecialPowerコマンドで指定されたスペシャルパワー「" & sname & "」が見つかりません"
			Error(0)
		End If
		
		sd = SPDList.Item(sname)
		
		msg_window_visible = frmMessage.Visible
		
		Dim prev_target As Unit
		If sd.Duration = "即効" Then
			prev_target = SelectedTarget
			If Not t Is Nothing Then
				SelectedTarget = t
			Else
				SelectedTarget = SelectedTargetForEvent
			End If
			prev_action = u.Action
			sd.Execute(u.MainPilot, True)
			If Not prev_target Is Nothing Then
				SelectedTarget = prev_target.CurrentForm
			End If
			If (prev_action = 0 And u.Action > 0) Or (prev_action > 0 And u.Action = 0) Then
				RedrawScreen()
			End If
		ElseIf Not t Is Nothing Then 
			prev_action = t.Action
			t.MakeSpecialPowerInEffect(sname, u.MainPilot.ID)
			If (prev_action = 0 And t.Action > 0) Or (prev_action > 0 And t.Action = 0) Then
				RedrawScreen()
			End If
		Else
			prev_action = u.Action
			u.MakeSpecialPowerInEffect(sname)
			If (prev_action = 0 And u.Action > 0) Or (prev_action > 0 And u.Action = 0) Then
				RedrawScreen()
			End If
		End If
		
		If Not msg_window_visible Then
			CloseMessageForm()
		End If
		
		ExecSpecialPowerCmd = LineNum + 1
	End Function
	
	Private Function ExecSplitCmd() As Integer
		Dim u As Unit
		
		Select Case ArgNum
			Case 1
				u = SelectedUnitForEvent
			Case 2
				u = GetArgAsUnit(2)
			Case Else
				EventErrorMessage = "Splitコマンドの引数の数が違います"
				Error(0)
		End Select
		
		With u
			If Not .IsFeatureAvailable("分離") Then
				EventErrorMessage = .Name & "は分離できません"
				Error(0)
			End If
			
			.Split_Renamed()
			
			'分離形態の１番目のユニットをメインユニットに設定
			u = UList.Item(LIndex(.FeatureData("分離"), 2))
			
			'変数のアップデート
			If Not SelectedUnit Is Nothing Then
				If .ID = SelectedUnit.ID Then
					SelectedUnit = u
				End If
			End If
			If Not SelectedUnitForEvent Is Nothing Then
				If .ID = SelectedUnitForEvent.ID Then
					SelectedUnitForEvent = u
				End If
			End If
			If Not SelectedTarget Is Nothing Then
				If .ID = SelectedTarget.ID Then
					SelectedTarget = u
				End If
			End If
			If Not SelectedTargetForEvent Is Nothing Then
				If .ID = SelectedTargetForEvent.ID Then
					SelectedTargetForEvent = u
				End If
			End If
		End With
		
		ExecSplitCmd = LineNum + 1
	End Function
	
	Private Function ExecStartBGMCmd() As Integer
		Dim fname As String
		Dim start_bgm_end As Integer
		Dim i As Integer
		
		'StartBGMコマンドが連続してる場合、最後のStartBGMコマンドの位置を検索
		For i = LineNum + 1 To UBound(EventCmd)
			If EventCmd(i).Name <> Event_Renamed.CmdType.StartBGMCmd Then
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
		StartBGM(fname)
		
		'次のコマンド実行位置は最後のStartBGMコマンドの後
		ExecStartBGMCmd = start_bgm_end + 1
	End Function
	
	Private Function ExecStopBGMCmd() As Integer
		KeepBGM = False
		BossBGM = False
		' MOD START マージ
		'    StopBGM
		StopBGM(True)
		' MOD END マージ
		ExecStopBGMCmd = LineNum + 1
	End Function
	
	Private Function ExecStopSummoningCmd() As Integer
		Dim u As Unit
		
		Select Case ArgNum
			Case 1
				u = SelectedUnitForEvent
			Case 2
				u = GetArgAsUnit(2)
			Case Else
				EventErrorMessage = "StopSummoningコマンドの引数の数が違います"
				Error(0)
		End Select
		
		'召喚したユニットを解放
		u.DismissServant()
		
		ExecStopSummoningCmd = LineNum + 1
	End Function
	
	Private Function ExecSunsetCmd() As Integer
		Dim prev_x, prev_y As Short
		Dim u As Unit
		Dim late_refresh As Boolean
		Dim i As Short
		Dim buf As String
		
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
					EventErrorMessage = "Sunsetコマンドに不正なオプション「" & buf & "」が使われています"
					Error(0)
			End Select
		Next 
		
		prev_x = MapX
		prev_y = MapY
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		SetupBackground("夕焼け", "非同期")
		
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Then
					If .BitmapID = 0 Then
						With UList.Item(.Name)
							If u.Party0 = .Party0 And .BitmapID <> 0 And u.Bitmap = .Bitmap And Not .IsFeatureAvailable("ダミーユニット") Then
								u.BitmapID = .BitmapID
							Else
								u.BitmapID = MakeUnitBitmap(u)
							End If
						End With
					End If
				End If
			End With
		Next u
		
		Center(prev_x, prev_y)
		RedrawScreen(late_refresh)
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ExecSunsetCmd = LineNum + 1
	End Function
	
	Private Function ExecSupplyCmd() As Integer
		Dim u As Unit
		
		Select Case ArgNum
			Case 2
				u = GetArgAsUnit(2)
			Case 1
				u = SelectedUnitForEvent
			Case Else
				EventErrorMessage = "Supplyコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not u Is Nothing Then
			u.FullSupply()
		End If
		
		ExecSupplyCmd = LineNum + 1
	End Function
	
	Private Function ExecSwapCmd() As Integer
		Dim new_var1 As New VarData
		Dim new_var2 As New VarData
		Dim old_var1 As VarData
		Dim old_var2 As VarData
		Dim i As Short
		
		If ArgNum <> 3 Then
			EventErrorMessage = "Swapコマンドの引数の数が違います"
			Error(0)
		Else
			'入れ替える前の変数の値を保存
			'引数1の変数
			old_var1 = GetVariableObject(GetArg(2))
			If Not old_var1 Is Nothing Then
				With new_var2
					.Name = old_var1.Name
					.VariableType = old_var1.VariableType
					.StringValue = old_var1.StringValue
					.NumericValue = old_var1.NumericValue
				End With
			End If
			'引数2の変数
			old_var2 = GetVariableObject(GetArg(3))
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
		'UPGRADE_NOTE: オブジェクト old_var1 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		old_var1 = Nothing
		'UPGRADE_NOTE: オブジェクト old_var2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		old_var2 = Nothing
		'UPGRADE_NOTE: オブジェクト new_var1 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		new_var1 = Nothing
		'UPGRADE_NOTE: オブジェクト new_var2 をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		new_var2 = Nothing
		
		ExecSwapCmd = LineNum + 1
	End Function
	
	Private Function ExecSwitchCmd() As Integer
		Dim i As Integer
		Dim j, depth As Short
		Dim a, b As String
		
		If ArgNum <> 2 Then
			EventErrorMessage = "Switchコマンドの引数の数が違います"
			Error(0)
		End If
		
		a = GetArgAsString(2)
		
		depth = 1
		For i = LineNum + 1 To UBound(EventCmd)
			With EventCmd(i)
				Select Case .Name
					Case Event_Renamed.CmdType.CaseCmd
						If depth = 1 Then
							For j = 2 To .ArgNum
								If .GetArgsType(j) = Expression.ValueType.UndefinedType Then
									'未識別のパラメータは式として処理する
									b = .GetArgAsString(j)
									If b = .GetArg(j) Then
										'文字列として識別済みにする
										.SetArgsType(j, Expression.ValueType.StringType)
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
					Case Event_Renamed.CmdType.CaseElseCmd
						If depth = 1 Then
							ExecSwitchCmd = i + 1
							Exit Function
						End If
					Case Event_Renamed.CmdType.EndSwCmd
						If depth = 1 Then
							ExecSwitchCmd = i + 1
							Exit Function
						Else
							depth = depth - 1
						End If
					Case Event_Renamed.CmdType.SwitchCmd
						depth = depth + 1
				End Select
			End With
		Next 
		
		EventErrorMessage = "SwitchとEndSwが対応していません"
		Error(0)
	End Function
	
	Private Function ExecCaseCmd() As Integer
		Dim i As Integer
		Dim depth As Short
		
		'対応するEndSwを探す
		depth = 1
		For i = LineNum + 1 To UBound(EventCmd)
			Select Case EventCmd(i).Name
				Case Event_Renamed.CmdType.SwitchCmd
					depth = depth + 1
				Case Event_Renamed.CmdType.EndSwCmd
					depth = depth - 1
					If depth = 0 Then
						ExecCaseCmd = i + 1
						Exit Function
					End If
			End Select
		Next 
		
		EventErrorMessage = "SwitchとEndSwが対応していません"
		Error(0)
	End Function
	
	Private Function ExecTalkCmd() As Integer
		Dim pname, current_pname As String
		Dim u As Unit
		Dim ux, uy As Short
		Dim i As Integer
		Dim j As Short
		Dim lnum As Short
		Dim without_cursor As Boolean
		Dim options, opt As String
		Dim buf As String
		
		Dim counter As Short
		counter = LineNum
		Dim cname As String
		Dim tcolor As Integer
		For i = counter To UBound(EventData)
			With EventCmd(i)
				Select Case .Name
					Case Event_Renamed.CmdType.TalkCmd
						If .ArgNum > 1 Then
							pname = .GetArgAsString(2)
						Else
							pname = ""
						End If
						
						If Left(pname, 1) = "@" Then
							'メインパイロットの強制指定
							pname = Mid(pname, 2)
							If PList.IsDefined(pname) Then
								With PList.Item(pname)
									If Not .Unit_Renamed Is Nothing Then
										pname = .Unit_Renamed.MainPilot.Name
									End If
								End With
							End If
						End If
						
						'話者名チェック
						If Not PList.IsDefined(pname) And Not PDList.IsDefined(pname) And Not NPDList.IsDefined(pname) And Not pname = "システム" And Not pname = "" Then
							EventErrorMessage = "「" & pname & "」というパイロットが定義されていません"
							LineNum = i
							Error(0)
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
									Case "白黒", "セピア", "明", "暗", "上下反転", "左右反転", "上半分", "下半分", "右半分", "左半分", "右上", "左上", "右下", "左下", "ネガポジ反転", "シルエット", "夕焼け", "水中", "通常"
										If j > 2 Then
											'これらのパイロット画像描画に関するオプションは
											'パイロット名が指定されている場合にのみ有効
											options = options & opt & " "
										Else
											lnum = j
										End If
									Case "右回転"
										j = j + 1
										options = options & "右回転 " & .GetArgAsString(j) & " "
									Case "左回転"
										j = j + 1
										options = options & "左回転 " & .GetArgAsString(j) & " "
									Case "フィルタ"
										j = j + 1
										buf = .GetArgAsString(j)
										cname = New String(vbNullChar, 8)
										Mid(cname, 1, 2) = "&H"
										Mid(cname, 3, 2) = Mid(buf, 6, 2)
										Mid(cname, 5, 2) = Mid(buf, 4, 2)
										Mid(cname, 7, 2) = Mid(buf, 2, 2)
										tcolor = CInt(cname)
										j = j + 1
										options = options & "フィルタ " & VB6.Format(tcolor) & " " & .GetArgAsString(j) & " "
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
									OpenMessageForm()
								End If
								
								'メッセージウィンドウのパイロット画像を以前指定された
								'ものに確定させる
								If current_pname <> "" Then
									DisplayMessage(current_pname, "", options)
								End If
								
								current_pname = ""
								
							Case 2
								'パイロット名のみ指定
								current_pname = pname
								
								'話者中心に画面位置を変更
								
								'プロローグイベントやエピローグイベント時はキャンセル
								If Stage = "プロローグ" Or Stage = "エピローグ" Then
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
								CenterUnit(pname, without_cursor)
								
							Case 3
								current_pname = pname
								Select Case .GetArgAsString(3)
									Case "母艦"
										'母艦の中央表示
										CenterUnit("母艦", without_cursor)
									Case "中央"
										'話者の中央表示
										CenterUnit(pname, without_cursor)
									Case "固定"
										'表示位置固定
								End Select
								
							Case 4
								'表示の座標指定あり
								current_pname = pname
								CenterUnit(pname, without_cursor, .GetArgAsLong(3), .GetArgAsLong(4))
								
							Case -1
								EventErrorMessage = "Talkコマンドのパラメータの括弧の対応が取れていません"
								LineNum = i
								Error(0)
								
							Case Else
								EventErrorMessage = "Talkコマンドの引数の数が違います"
								LineNum = i
								Error(0)
						End Select
						
						If Not frmMessage.Visible Then
							OpenMessageForm()
						End If
						
					Case Event_Renamed.CmdType.EndCmd
						CloseMessageForm()
						MessageWindowIsOut = False
						If .ArgNum <> 1 Then
							EventErrorMessage = "End部分の引数の数が違います"
							LineNum = i
							Error(0)
						End If
						Exit For
						
					Case Event_Renamed.CmdType.SuspendCmd
						If .ArgNum <> 1 Then
							EventErrorMessage = "Suspend部分の引数の数が違います"
							LineNum = i
							Error(0)
						End If
						Exit For
						
					Case Else
						If Not frmMessage.Visible Then
							OpenMessageForm()
						End If
						DisplayMessage(current_pname, EventData(i), options)
						
				End Select
			End With
NextLoop: 
		Next 
		
		If i > UBound(EventData) Then
			CloseMessageForm()
			EventErrorMessage = "TalkとEndが対応していません"
			Error(0)
		End If
		
		ExecTalkCmd = i + 1
	End Function
	
	Private Function ExecTelopCmd() As Integer
		Dim msg, BGM As String
		
		msg = ListTail(EventData(LineNum), 2)
		If ListLength(msg) = 1 Then
			msg = GetValueAsString(msg)
		End If
		FormatMessage(msg)
		
		BGM = SearchMidiFile(BGMName("Subtitle"))
		If Len(BGM) > 0 Then
			StartBGM(BGM, False)
			If Not IsRButtonPressed() Then
				Sleep(1000)
			End If
			DisplayTelop(msg)
			If Not IsRButtonPressed() Then
				Sleep(2000)
			End If
		Else
			DisplayTelop(msg)
		End If
		
		ExecTelopCmd = LineNum + 1
	End Function
	
	Private Function ExecTransformCmd() As Integer
		Dim u As Unit
		Dim tname As String
		
		Select Case ArgNum
			Case 3
				u = GetArgAsUnit(2)
				tname = GetArgAsString(3)
			Case 2
				u = SelectedUnitForEvent
				tname = GetArgAsString(2)
			Case Else
				EventErrorMessage = "Transformコマンドの引数の数が違います"
				Error(0)
		End Select
		
		With u
			If .Name = tname Then
				'元々指定された形態になっていたので変形の必要なし
				ExecTransformCmd = LineNum + 1
				Exit Function
			End If
			
			'変形
			.Transform(tname)
			
			'グローバル変数の更新
			If u Is SelectedUnit Then
				SelectedUnit = .CurrentForm
			End If
			If u Is SelectedUnitForEvent Then
				SelectedUnitForEvent = .CurrentForm
			End If
			If u Is SelectedTarget Then
				SelectedTarget = .CurrentForm
			End If
			If u Is SelectedTargetForEvent Then
				SelectedTargetForEvent = .CurrentForm
			End If
		End With
		
		ExecTransformCmd = LineNum + 1
	End Function
	
	Private Function ExecUnitCmd() As Integer
		Dim uname As String
		Dim u As Unit
		Dim urank As Short
		
		If ArgNum < 0 Then
			EventErrorMessage = "Unitコマンドのパラメータの括弧の対応が取れていません"
			Error(0)
		ElseIf ArgNum <> 3 Then 
			EventErrorMessage = "Unitコマンドの引数の数が違います"
			Error(0)
		End If
		
		uname = GetArgAsString(2)
		If Not UDList.IsDefined(uname) Then
			EventErrorMessage = "指定したユニット「" & uname & "」のデータが見つかりません"
			Error(0)
		End If
		
		urank = GetArgAsLong(3)
		
		u = UList.Add(uname, urank, "味方")
		If u Is Nothing Then
			EventErrorMessage = uname & "のユニットデータが不正です"
			Error(0)
		End If
		SelectedUnitForEvent = u
		
		ExecUnitCmd = LineNum + 1
	End Function
	
	Private Function ExecUnsetCmd() As Integer
		UndefineVariable(GetArg(2))
		
		ExecUnsetCmd = LineNum + 1
	End Function
	
	Private Function ExecUpgradeCmd() As Integer
		Dim uname As String
		Dim u1, u, u2 As Unit
		Dim i As Short
		Dim prev_status As String
		
		Select Case ArgNum
			Case 2
				u1 = SelectedUnitForEvent.CurrentForm
				uname = GetArgAsString(2)
			Case 3
				uname = GetArgAsString(2)
				If Not UList.IsDefined(uname) Then
					EventErrorMessage = uname & "というユニットはありません"
					Error(0)
				End If
				u1 = UList.Item(uname).CurrentForm
				
				uname = GetArgAsString(3)
			Case Else
				EventErrorMessage = "Upgradeコマンドの引数の数が違います"
				Error(0)
		End Select
		
		If Not UDList.IsDefined(uname) Then
			EventErrorMessage = "ユニット「" & uname & "」のデータが見つかりません"
			Error(0)
		End If
		
		prev_status = u1.Status_Renamed
		
		u2 = UList.Add(uname, u1.Rank, (u1.Party0))
		If u2 Is Nothing Then
			EventErrorMessage = uname & "のユニットデータが不正です"
			Error(0)
		End If
		
		If u1.BossRank > 0 Then
			u2.BossRank = u1.BossRank
			u2.FullRecover()
		End If
		
		'パイロットの乗せ換え
		Dim pilot_list() As Pilot
		Dim support_list() As Pilot
		If u1.CountPilot > 0 Then
			
			ReDim pilot_list(u1.CountPilot)
			ReDim support_list(u1.CountSupport)
			
			For i = 1 To UBound(pilot_list)
				pilot_list(i) = u1.Pilot(i)
			Next 
			For i = 1 To UBound(support_list)
				support_list(i) = u1.Support(i)
			Next 
			
			u1.Pilot(1).GetOff()
			
			For i = 1 To UBound(pilot_list)
				pilot_list(i).Ride(u2)
			Next 
			For i = 1 To UBound(support_list)
				support_list(i).Ride(u2)
			Next 
		End If
		
		'アイテムの交換
		For i = 1 To u1.CountItem
			u2.AddItem(u1.Item(i))
		Next 
		For i = 1 To u1.CountItem
			u1.DeleteItem(1)
		Next 
		
		'リンクの付け替え
		u2.Master = u1.Master
		'UPGRADE_NOTE: オブジェクト u1.Master をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		u1.Master = Nothing
		u2.Summoner = u1.Summoner
		'UPGRADE_NOTE: オブジェクト u1.Summoner をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		u1.Summoner = Nothing
		
		'召喚ユニットの交換
		For i = 1 To u1.CountServant
			u2.AddServant(u1.Servant(i))
		Next 
		For i = 1 To u1.CountServant
			u1.DeleteServant(1)
		Next 
		
		'収納ユニットの交換
		If u1.IsFeatureAvailable("母艦") Then
			For i = 1 To u1.CountOtherForm
				If u1.OtherForm(i).Status_Renamed = "格納" Then
					u2.AddOtherForm(u1.OtherForm(i))
				End If
			Next 
			For i = 1 To u2.CountOtherForm
				If u2.OtherForm(i).Status_Renamed = "格納" Then
					u1.DeleteOtherForm((u2.OtherForm(i).ID))
				End If
			Next 
		End If
		
		u2.Area = u1.Area
		
		'元のユニットを削除
		u1.Status_Renamed = "破棄"
		For i = 1 To u1.CountOtherForm
			If u1.OtherForm(i).Status_Renamed = "他形態" Then
				u1.OtherForm(i).Status_Renamed = "破棄"
			End If
		Next 
		
		u2.UsedAction = u1.UsedAction
		u2.UsedSupportAttack = u1.UsedSupportAttack
		u2.UsedSupportGuard = u1.UsedSupportGuard
		u2.UsedSyncAttack = u1.UsedSyncAttack
		u2.UsedCounterAttack = u1.UsedCounterAttack
		
		Select Case prev_status
			Case "出撃"
				'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				MapDataForUnit(u1.X, u1.Y) = Nothing
				u2.StandBy(u1.X, u1.Y)
				If Not IsPictureVisible Then
					RedrawScreen()
				End If
			Case "破壊", "破棄"
				If MapDataForUnit(u1.X, u1.Y) Is u1 Then
					'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
					MapDataForUnit(u1.X, u1.Y) = Nothing
				End If
				u2.StandBy(u1.X, u1.Y)
				If Not IsPictureVisible Then
					RedrawScreen()
				End If
			Case "格納"
				For	Each u In UList
					With u
						For i = 1 To .CountUnitOnBoard
							If u1.ID = .UnitOnBoard(i).ID Then
								.UnloadUnit((u1.ID))
								u2.Land(u, True)
								GoTo ExitLoop
							End If
						Next 
					End With
				Next u
ExitLoop: 
			Case Else
				u2.Status_Renamed = prev_status
		End Select
		
		'グローバル変数の更新
		If u1 Is SelectedUnit Then
			SelectedUnit = u2
		End If
		If u1 Is SelectedUnitForEvent Then
			SelectedUnitForEvent = u2
		End If
		If u1 Is SelectedTarget Then
			SelectedTarget = u2
		End If
		If u1 Is SelectedTargetForEvent Then
			SelectedTargetForEvent = u2
		End If
		For i = 1 To SelectionStackIndex
			If u1 Is SavedSelectedUnit(i) Then
				SavedSelectedUnit(i) = u2
			End If
			If u1 Is SavedSelectedUnitForEvent(i) Then
				SavedSelectedUnitForEvent(i) = u2
			End If
			If u1 Is SavedSelectedTarget(i) Then
				SavedSelectedTarget(i) = u2
			End If
			If u1 Is SavedSelectedTargetForEvent(i) Then
				SavedSelectedTargetForEvent(i) = u2
			End If
		Next 
		
		ExecUpgradeCmd = LineNum + 1
	End Function
	
	Private Function ExecUpvarCmd() As Integer
		UpVarLevel = UpVarLevel + 1
		ExecUpvarCmd = LineNum + 1
	End Function
	
	Private Function ExecUseAbilityCmd() As Integer
		Dim u1, u2 As Unit
		Dim aname As String
		Dim a As Short
		
		Select Case ArgNum
			Case 4
				u1 = GetArgAsUnit(2)
				
				aname = GetArgAsString(3)
				For a = 1 To u1.CountAbility
					If aname = u1.Ability(a).Name Then
						Exit For
					End If
				Next 
				If a > u1.CountAbility Then
					EventErrorMessage = "アビリティ名が間違っています"
					Error(0)
				End If
				
				u2 = GetArgAsUnit(4)
			Case 3
				u1 = SelectedUnitForEvent
				If Not u1 Is Nothing Then
					aname = GetArgAsString(2)
					For a = 1 To u1.CountAbility
						If aname = u1.Ability(a).Name Then
							Exit For
						End If
					Next 
					
					If a <= u1.CountAbility Then
						u2 = GetArgAsUnit(3)
					Else
						u1 = GetArgAsUnit(2)
						
						aname = GetArgAsString(3)
						For a = 1 To u1.CountAbility
							If aname = u1.Ability(a).Name Then
								Exit For
							End If
						Next 
						If a > u1.CountAbility Then
							EventErrorMessage = "アビリティ名が間違っています"
							Error(0)
						End If
						
						u2 = u1
					End If
				Else
					u1 = GetArgAsUnit(2)
					
					aname = GetArgAsString(3)
					For a = 1 To u1.CountAbility
						If aname = u1.Ability(a).Name Then
							Exit For
						End If
					Next 
					If a > u1.CountAbility Then
						EventErrorMessage = "アビリティ名が間違っています"
						Error(0)
					End If
					
					u2 = u1
				End If
			Case 2
				u1 = SelectedUnitForEvent
				
				aname = GetArgAsString(2)
				For a = 1 To u1.CountAbility
					If aname = u1.Ability(a).Name Then
						Exit For
					End If
				Next 
				If a > u1.CountAbility Then
					EventErrorMessage = "アビリティ名が間違っています"
					Error(0)
				End If
				
				u2 = SelectedUnitForEvent
			Case Else
				EventErrorMessage = "UseAbilityコマンドの引数の数が違います"
				Error(0)
		End Select
		
		With u1
			If .Status_Renamed <> "出撃" Then
				EventErrorMessage = .Nickname & "は出撃していません"
				Error(0)
			End If
			.ExecuteAbility(a, u2, False, True)
			CloseMessageForm()
		End With
		
		RedrawScreen()
		
		ExecUseAbilityCmd = LineNum + 1
	End Function
	
	Private Function ExecWaitCmd() As Integer
		Dim i As Short
		Dim wait_time, start_time, cur_time As Integer
		
		Select Case ArgNum
			Case 2
				Select Case LCase(GetArg(2))
					Case "start"
						WaitStartTime = timeGetTime()
						WaitTimeCount = 0
						
					Case "reset"
						WaitStartTime = -1
						WaitTimeCount = 0
						
					Case "click"
						'先行入力されていたクリックイベントを解消
						System.Windows.Forms.Application.DoEvents()
						WaitClickMode = True
						IsFormClicked = False
						SelectedAlternative = ""
						
						'ウィンドウが表示されていない場合は表示
						With MainForm
							If Not .Visible Then
								.Show()
								.Refresh()
							End If
						End With
						
						'クリックされるまで待つ
						Do Until IsFormClicked
							If IsRButtonPressed(True) Then
								MouseButton = 0
								Exit Do
							End If
							System.Windows.Forms.Application.DoEvents()
							Sleep(25)
						Loop 
						
						'マウスの左ボタンが押された場合はホットポイントの判定を行う
						If SelectedAlternative = "" And MouseButton = 1 Then
							For i = 1 To UBound(HotPointList)
								With HotPointList(i)
									If .Left_Renamed <= MouseX And MouseX < .Left_Renamed + .width And .Top <= MouseY And MouseY < .Top + .Height Then
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
								System.Windows.Forms.Application.DoEvents()
								Sleep(wait_time)
							End If
						Else
							start_time = timeGetTime()
							Do While (start_time + wait_time > timeGetTime())
								'右ボタンを押されていたら早送り
								If IsRButtonPressed(True) Then
									Exit Do
								End If
								
								System.Windows.Forms.Application.DoEvents()
								Sleep(25)
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
					
					System.Windows.Forms.Application.DoEvents()
					Sleep(25)
				Loop 
				
			Case Else
				EventErrorMessage = "Waitコマンドの引数の数が違います"
				Error(0)
		End Select
		
		ExecWaitCmd = LineNum + 1
	End Function
	
	Private Function ExecWaterCmd() As Integer
		Dim prev_x, prev_y As Short
		Dim u As Unit
		Dim late_refresh As Boolean
		Dim i As Short
		Dim buf As String
		
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
					EventErrorMessage = "Waterコマンドに不正なオプション「" & buf & "」が使われています"
					Error(0)
			End Select
		Next 
		
		prev_x = MapX
		prev_y = MapY
		
		'マウスカーソルを砂時計に
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.WaitCursor
		
		SetupBackground("水中", "非同期")
		
		For	Each u In UList
			With u
				If .Status_Renamed = "出撃" Then
					If .BitmapID = 0 Then
						With UList.Item(.Name)
							If u.Party0 = .Party0 And .BitmapID <> 0 And u.Bitmap = .Bitmap And Not .IsFeatureAvailable("ダミーユニット") Then
								u.BitmapID = .BitmapID
							Else
								u.BitmapID = MakeUnitBitmap(u)
							End If
						End With
					End If
				End If
			End With
		Next u
		
		Center(prev_x, prev_y)
		RedrawScreen(late_refresh)
		
		'マウスカーソルを元に戻す
		'UPGRADE_WARNING: Screen プロパティ Screen.MousePointer には新しい動作が含まれます。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6BA9B8D2-2A32-4B6E-8D36-44949974A5B4"' をクリックしてください。
		System.Windows.Forms.Cursor.Current = System.Windows.Forms.Cursors.Default
		
		ExecWaterCmd = LineNum + 1
	End Function
	
	Private Function ExecWhiteInCmd() As Integer
		Dim cur_time, start_time, wait_time As Integer
		Dim i, ret As Integer
		Dim num As Short
		
		Select Case ArgNum
			Case 1
				num = 10
			Case 2
				num = GetArgAsLong(2)
			Case Else
				EventErrorMessage = "WhiteInコマンドの引数の数が違います"
				Error(0)
		End Select
		
		SaveScreen()
		
		With MainForm
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With .picTmp
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Picture = System.Drawing.Image.FromFile("")
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.width = MainPWidth
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Height = MainPHeight
			End With
			
			' MOD START マージ
			'        ret = BitBlt(.picTmp.hDC, _
			''            0, 0, MapPWidth, MapPHeight, _
			''            .picMain(0).hDC, 0, 0, SRCCOPY)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = GUI.BitBlt(.picTmp.hDC, 0, 0, MainPWidth, MainPHeight, .picMain(0).hDC, 0, 0, SRCCOPY)
			' MOD END マージ
			
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			InitFade(.picMain(0), num, True)
			
			start_time = timeGetTime()
			wait_time = 50
			For i = 0 To num
				If i Mod 4 = 0 Then
					If IsRButtonPressed() Then
						Exit For
					End If
				End If
				
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				DoFade(.picMain(0), i)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Refresh()
				
				cur_time = timeGetTime()
				Do While cur_time < start_time + wait_time * (i + 1)
					System.Windows.Forms.Application.DoEvents()
					cur_time = timeGetTime()
				Loop 
			Next 
			
			FinishFade()
			
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = GUI.BitBlt(.picMain(0).hDC, 0, 0, MapPWidth, MapPHeight, .picTmp.hDC, 0, 0, SRCCOPY)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.picMain(0).Refresh()
			
			'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			With .picTmp
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Picture = System.Drawing.Image.FromFile("")
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.width = 32
				'UPGRADE_ISSUE: Control picTmp は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Height = 32
			End With
		End With
		
		ExecWhiteInCmd = LineNum + 1
	End Function
	
	Private Function ExecWhiteOutCmd() As Integer
		Dim cur_time, start_time, wait_time As Integer
		Dim i, ret As Integer
		Dim num As Short
		
		Select Case ArgNum
			Case 1
				num = 10
			Case 2
				num = GetArgAsLong(2)
			Case Else
				EventErrorMessage = "WhiteOutコマンドの引数の数が違います"
				Error(0)
		End Select
		
		SaveScreen()
		
		With MainForm
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			InitFade(.picMain(0), num, True)
			
			start_time = timeGetTime()
			wait_time = 50
			For i = 0 To num
				If i Mod 4 = 0 Then
					If IsRButtonPressed() Then
						'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
						With .picMain(0)
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							ret = PatBlt(.hDC, 0, 0, .width, .Height, WHITENESS)
							'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
							.Refresh()
						End With
						Exit For
					End If
				End If
				
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				DoFade(.picMain(0), num - i)
				'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.picMain(0).Refresh()
				
				cur_time = timeGetTime()
				Do While cur_time < start_time + wait_time * (i + 1)
					System.Windows.Forms.Application.DoEvents()
					cur_time = timeGetTime()
				Loop 
			Next 
			
			FinishFade()
		End With
		
		IsPictureVisible = True
		PaintedAreaX1 = MainPWidth
		PaintedAreaY1 = MainPHeight
		PaintedAreaX2 = -1
		PaintedAreaY2 = -1
		
		ExecWhiteOutCmd = LineNum + 1
	End Function
	
	Private Function ExecWriteCmd() As Integer
		Dim f As Short
		Dim i As Short
		
		If ArgNum < 3 Then
			EventErrorMessage = "Writeコマンドの引数の数が違います"
			Error(0)
		End If
		
		f = GetArgAsLong(2)
		For i = 3 To ArgNum
			WriteLine(f, GetArgAsString(i))
		Next 
		
		ExecWriteCmd = LineNum + 1
	End Function
	
	'Flashファイルの再生
	Private Function ExecPlayFlashCmd() As Integer
		Dim fname As String
		Dim fw, fx, fy, fh As Short
		Dim i As Short
		Dim opt, buf As String
		
		If ArgNum < 6 Then
			EventErrorMessage = "PlayFlashコマンドの引数の数が違います"
			Error(0)
		End If
		
		fname = GetArgAsString(2)
		fw = GetArgAsLong(5)
		fh = GetArgAsLong(6)
		buf = GetArgAsString(3)
		If buf = "-" Then
			fx = (480 - fw) / 2
		Else
			fx = CShort(buf)
		End If
		buf = GetArgAsString(4)
		If buf = "-" Then
			fy = (480 - fh) / 2
		Else
			fy = CShort(buf)
		End If
		
		opt = ""
		For i = 7 To ArgNum
			opt = opt & " " & GetArgAsString(i)
		Next 
		opt = Trim(opt)
		
		PlayFlash(fname, fx, fy, fw, fh, opt)
		
		ExecPlayFlashCmd = LineNum + 1
	End Function
	
	'Flashファイルの消去
	Private Function ExecClearFlashCmd() As Integer
		ClearFlash()
		
		ExecClearFlashCmd = LineNum + 1
	End Function
End Class