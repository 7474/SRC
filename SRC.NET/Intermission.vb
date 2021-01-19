Option Strict Off
Option Explicit On
Module InterMission
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'インターミッションに関する処理を行うモジュール
	
	'インターミッション
	Public Sub InterMissionCommand(Optional ByVal skip_update As Boolean = False)
		Dim cmd_list() As String
		Dim name_list() As String
		Dim j, i, ret As Short
		Dim buf As String
		Dim u As Unit
		Dim var As VarData
		Dim fname, save_path As String
		
		Stage = "インターミッション"
		IsSubStage = False
		
		'ＢＧＭを変更
		KeepBGM = False
		BossBGM = False
		If InStr(BGMFileName, "\" & BGMName("Intermission")) = 0 Then
			StopBGM()
			StartBGM(BGMName("Intermission"))
		End If
		
		'マップをクリア
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				MapDataForUnit(i, j) = Nothing
			Next 
		Next 
		
		'各種データをアップデート
		If Not skip_update Then
			UList.Update()
			PList.Update()
			IList.Update()
		End If
		ClearEventData()
		ClearMap()
		
		'選択用ダイアログを拡大
		EnlargeListBoxHeight()
		
		Do While True
			'利用可能なインターミッションコマンドを選択
			
			ReDim cmd_list(0)
			ReDim ListItemFlag(0)
			ReDim ListItemID(0)
			cmd_list(0) = "キャンセル"
			
			'「次のステージへ」コマンド
			If GetValueAsString("次ステージ") <> "" Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "次のステージへ"
			End If
			
			'データセーブコマンド
			If Not IsOptionDefined("データセーブ不可") Or IsOptionDefined("デバッグ") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "データセーブ"
			End If
			
			'機体改造コマンド
			If Not IsOptionDefined("改造不可") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				If IsOptionDefined("等身大基準") Then
					cmd_list(UBound(cmd_list)) = "ユニットの強化"
				Else
					cmd_list(UBound(cmd_list)) = "機体改造"
					For	Each u In UList
						With u
							If .Party0 = "味方" And .Status_Renamed = "待機" Then
								If Left(.Class_Renamed, 1) = "(" Then
									cmd_list(UBound(cmd_list)) = "ユニットの強化"
									Exit For
								End If
							End If
						End With
					Next u
				End If
			End If
			
			'乗り換えコマンド
			If IsOptionDefined("乗り換え") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "乗り換え"
			End If
			
			'アイテム交換コマンド
			If IsOptionDefined("アイテム交換") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "アイテム交換"
			End If
			
			'換装コマンド
			For	Each u In UList
				With u
					If .Party0 = "味方" And .Status_Renamed = "待機" Then
						If .IsFeatureAvailable("換装") Then
							For i = 1 To LLength(.FeatureData("換装"))
								If .OtherForm(LIndex(.FeatureData("換装"), i)).IsAvailable Then
									Exit For
								End If
							Next 
							If i <= LLength(.FeatureData("換装")) Then
								ReDim Preserve cmd_list(UBound(cmd_list) + 1)
								ReDim Preserve ListItemFlag(UBound(cmd_list))
								cmd_list(UBound(cmd_list)) = "換装"
								Exit For
							End If
						End If
					End If
				End With
			Next u
			
			'パイロットステータスコマンド
			If Not IsOptionDefined("等身大基準") Then
				ReDim Preserve cmd_list(UBound(cmd_list) + 1)
				ReDim Preserve ListItemFlag(UBound(cmd_list))
				cmd_list(UBound(cmd_list)) = "パイロットステータス"
			End If
			
			'ユニットステータスコマンド
			ReDim Preserve cmd_list(UBound(cmd_list) + 1)
			ReDim Preserve ListItemFlag(UBound(cmd_list))
			cmd_list(UBound(cmd_list)) = "ユニットステータス"
			
			'ユーザー定義のインターミッションコマンド
			For	Each var In GlobalVariableList
				If InStr(var.Name, "IntermissionCommand(") = 1 Then
					ret = Len("IntermissionCommand(")
					buf = Mid(var.Name, ret + 1, Len(var.Name) - ret - 1)
					buf = GetValueAsString(buf)
					FormatMessage(buf)
					ReDim Preserve cmd_list(UBound(cmd_list) + 1)
					ReDim Preserve ListItemFlag(UBound(cmd_list))
					ReDim Preserve ListItemID(UBound(cmd_list))
					cmd_list(UBound(cmd_list)) = buf
					ListItemID(UBound(cmd_list)) = var.Name
				End If
			Next var
			
			'終了コマンド
			ReDim Preserve cmd_list(UBound(cmd_list) + 1)
			ReDim Preserve ListItemFlag(UBound(cmd_list))
			cmd_list(UBound(cmd_list)) = "SRCを終了"
			
			'インターミッションのコマンド名称にエリアスを適用
			ReDim name_list(UBound(cmd_list))
			For i = 1 To UBound(name_list)
				name_list(i) = cmd_list(i)
				With ALDList
					For j = 1 To .Count
						With .Item(j)
							If .AliasType(1) = cmd_list(i) Then
								name_list(i) = .Name
								Exit For
							End If
						End With
					Next 
				End With
			Next 
			
			'プレイヤーによるコマンド選択
			TopItem = 1
			ret = ListBox("インターミッション： 総ターン数" & VB6.Format(TotalTurn) & " " & Term("資金") & VB6.Format(Money), name_list, "コマンド", "連続表示")
			
			'選択されたインターミッションコマンドを実行
			Select Case cmd_list(ret)
				Case "次のステージへ"
					If MsgBox("次のステージへ進みますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "次ステージ") = 1 Then
						
						UList.Update() '追加パイロットを消去
						
						frmListBox.Hide()
						ReduceListBoxHeight()
						StopBGM()
						Exit Sub
					End If
					
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
						GoTo NextLoop
					End If
					
					'セーブ先はシナリオフォルダ？
					If InStr(fname, "\") > 0 Then
						save_path = Left(fname, InStr2(fname, "\"))
					End If
					'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
					If Dir(save_path) <> Dir(ScenarioPath) Then
						If MsgBox("セーブファイルはシナリオフォルダにないと読み込めません。" & vbCr & vbLf & "このままセーブしますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question) <> 1 Then
							GoTo NextLoop
						End If
					End If
					
					If fname <> "" Then
						UList.Update() '追加パイロットを消去
						SaveData(fname)
					End If
					
				Case "機体改造", "ユニットの強化"
					RankUpCommand()
					
				Case "乗り換え"
					ExchangeUnitCommand()
					
				Case "アイテム交換"
					ExchangeItemCommand()
					
				Case "換装"
					ExchangeFormCommand()
					
				Case "SRCを終了"
					If MsgBox("SRCを終了しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "終了") = 1 Then
						frmListBox.Hide()
						ReduceListBoxHeight()
						ExitGame()
					End If
					
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
					Exit Sub
					
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
					Exit Sub
					
				Case "キャンセル"
					'キャンセル
					
					'ユーザー定義のインターミッションコマンド
				Case Else
					frmListBox.Hide()
					ReduceListBoxHeight()
					IsSubStage = True
					StartScenario(GetValueAsString(ListItemID(ret)))
					If IsSubStage Then
						'インターミッションを再開
						KeepBGM = False
						BossBGM = False
						ChangeBGM(BGMName("Intermission"))
						UList.Update()
						PList.Update()
						IList.Update()
						ClearEventData()
						If MapWidth > 1 Then
							ClearMap()
						End If
						IsSubStage = False
						EnlargeListBoxHeight()
					Else
						'サブステージを通常のステージとして実行
						IsSubStage = True
						Exit Sub
					End If
			End Select
NextLoop: 
		Loop 
	End Sub
	
	'機体改造コマンド
	Public Sub RankUpCommand()
		Dim k, i, j, urank As Short
		Dim list() As String
		Dim id_list() As String
		Dim sort_mode As String
		Dim sort_mode_type(7) As String
		Dim sort_mode_list(7) As String
		Dim item_flag_backup() As Boolean
		Dim item_comment_backup() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim u As Unit
		Dim cost As Integer
		Dim buf As String
		Dim ret As Short
		Dim b As Boolean
		Dim use_max_rank As Boolean
		Dim name_width As Short
		
		TopItem = 1
		
		'デフォルトのソート方法
		If IsOptionDefined("等身大基準") Then
			sort_mode = "レベル"
		Else
			sort_mode = "ＨＰ"
		End If
		
		'最大改造数がユニット毎に変更されているかをあらかじめチェック
		For	Each u In UList
			If u.IsFeatureAvailable("最大改造数") Then
				use_max_rank = True
				Exit For
			End If
		Next u
		
		'ユニット名の項の文字数を設定
		name_width = 33
		If use_max_rank Then
			name_width = name_width - 2
		End If
		If IsOptionDefined("等身大基準") Then
			name_width = name_width + 8
		End If
		
		'ユニットのリストを作成
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemFlag(1)
		ReDim ListItemComment(1)
		list(1) = "▽並べ替え▽"
		For	Each u In UList
			With u
				If .Party0 <> "味方" Or .Status_Renamed <> "待機" Then
					GoTo NextLoop
				End If
				
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve ListItemFlag(UBound(list))
				ReDim Preserve ListItemComment(UBound(list))
				
				'改造が可能？
				cost = RankUpCost(u)
				If cost > Money Or cost > 10000000 Then
					ListItemFlag(UBound(list)) = True
				End If
				
				'ユニットランク
				If use_max_rank Then
					list(UBound(list)) = RightPaddedString(.Nickname0, name_width) & LeftPaddedString(VB6.Format(.Rank), 2) & "/"
					If MaxRank(u) > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(MaxRank(u)), 2)
					Else
						list(UBound(list)) = list(UBound(list)) & "--"
					End If
				Else
					If .Rank < 10 Then
						list(UBound(list)) = RightPaddedString(.Nickname0, name_width) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = RightPaddedString(.Nickname0, name_width) & VB6.Format(.Rank)
					End If
				End If
				
				'改造に必要な資金
				If cost < 10000000 Then
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(cost), 7)
				Else
					list(UBound(list)) = list(UBound(list)) & LeftPaddedString("----", 7)
				End If
				
				'等身大基準の場合はパイロットレベルも表示
				If IsOptionDefined("等身大基準") Then
					If .CountPilot > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
					End If
				End If
				
				'ユニットに関する情報
				list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 6) & LeftPaddedString(VB6.Format(.Mobility), 4)
				
				'等身大基準でない場合はパイロット名を表示
				If Not IsOptionDefined("等身大基準") Then
					If .CountPilot > 0 Then
						list(UBound(list)) = list(UBound(list)) & "  " & .MainPilot.Nickname
					End If
				End If
				
				'装備しているアイテムをコメント欄に列記
				For k = 1 To .CountItem
					With .Item(k)
						If (.Class_Renamed() <> "固定" Or Not .IsFeatureAvailable("非表示")) And .Part <> "非表示" Then
							ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
						End If
					End With
				Next 
				
				'ユニットＩＤを記録しておく
				id_list(UBound(list)) = .ID
			End With
NextLoop: 
		Next u
		
Beginning: 
		
		'ソート
		If InStr(sort_mode, "名称") = 0 Then
			'数値を使ったソート
			
			'まず並べ替えに使うキーのリストを作成
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "ＨＰ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxHP
						Next 
					Case "ＥＮ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxEN
						Next 
					Case "装甲"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Armor
						Next 
					Case "運動性"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Mobility
						Next 
					Case "ユニットランク"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Rank
						Next 
					Case "レベル"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									With .MainPilot
										key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
									End With
								End If
							End With
						Next 
				End Select
			End With
			
			'キーを使って並べ換え
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'数値を使ったソート
			
			'まず並べ替えに使うキーのリストを作成
			ReDim strkey_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "名称", "ユニット名称"
						For i = 2 To UBound(list)
							strkey_list(i) = .Item(id_list(i)).KanaName
						Next 
					Case "パイロット名称"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									strkey_list(i) = .MainPilot.KanaName
								End If
							End With
						Next 
				End Select
			End With
			
			'キーを使って並べ換え
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'改造するユニットを選択
		If IsOptionDefined("等身大基準") Then
			If use_max_rank Then
				ret = ListBox("ユニット選択： " & Term("資金") & VB6.Format(Money), list, "ユニット                               " & Term("ランク", Nothing, 6) & "  費用 Lv  " & Term("ＨＰ", Nothing, 4) & " " & Term("ＥＮ", Nothing, 4) & " " & Term("装甲", Nothing, 4) & " " & Term("運動"), "連続表示,コメント")
			Else
				ret = ListBox("ユニット選択： " & Term("資金") & VB6.Format(Money), list, "ユニット                             " & Term("ランク", Nothing, 6) & "   費用 Lv  " & Term("ＨＰ", Nothing, 4) & " " & Term("ＥＮ", Nothing, 4) & " " & Term("装甲", Nothing, 4) & " " & Term("運動"), "連続表示,コメント")
			End If
		Else
			If use_max_rank Then
				ret = ListBox("ユニット選択： " & Term("資金") & VB6.Format(Money), list, "ユニット                       " & Term("ランク", Nothing, 6) & "  費用  " & Term("ＨＰ", Nothing, 4) & " " & Term("ＥＮ", Nothing, 4) & " " & Term("装甲", Nothing, 4) & " " & Term("運動", Nothing, 4) & " パイロット", "連続表示,コメント")
			Else
				ret = ListBox("ユニット選択： " & Term("資金") & VB6.Format(Money), list, "ユニット                     " & Term("ランク", Nothing, 6) & "   費用  " & Term("ＨＰ", Nothing, 4) & " " & Term("ＥＮ", Nothing, 4) & " " & Term("装甲", Nothing, 4) & " " & Term("運動", Nothing, 4) & " パイロット", "連続表示,コメント")
			End If
		End If
		
		Select Case ret
			Case 0
				'キャンセル
				Exit Sub
			Case 1
				'ソート方法を選択
				If IsOptionDefined("等身大基準") Then
					sort_mode_type(1) = "名称"
					sort_mode_list(1) = "名称"
					sort_mode_type(2) = "レベル"
					sort_mode_list(2) = "レベル"
					sort_mode_type(3) = "ＨＰ"
					sort_mode_list(3) = Term("ＨＰ")
					sort_mode_type(4) = "ＥＮ"
					sort_mode_list(4) = Term("ＥＮ")
					sort_mode_type(5) = "装甲"
					sort_mode_list(5) = Term("装甲")
					sort_mode_type(6) = "運動性"
					sort_mode_list(6) = Term("運動性")
					sort_mode_type(7) = "ユニットランク"
					sort_mode_list(7) = Term("ランク")
				Else
					sort_mode_type(1) = "ＨＰ"
					sort_mode_list(1) = Term("ＨＰ")
					sort_mode_type(2) = "ＥＮ"
					sort_mode_list(2) = Term("ＥＮ")
					sort_mode_type(3) = "装甲"
					sort_mode_list(3) = Term("装甲")
					sort_mode_type(4) = "運動性"
					sort_mode_list(4) = Term("運動性")
					sort_mode_type(5) = "ユニットランク"
					sort_mode_list(5) = Term("ランク")
					sort_mode_type(6) = "ユニット名称"
					sort_mode_list(6) = "ユニット名称"
					sort_mode_type(7) = "パイロット名称"
					sort_mode_list(7) = "パイロット名称"
				End If
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				ret = ListBox("どれで並べ替えますか？", sort_mode_list, "並べ替えの方法", "連続表示,コメント")
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'ソート方法を変更して再表示
				If ret > 0 Then
					sort_mode = sort_mode_type(ret)
				End If
				GoTo Beginning
		End Select
		
		'改造するユニットを検索
		u = UList.Item(id_list(ret))
		
		'改造するか確認
		If u.IsHero Then
			If MsgBox(u.Nickname0 & "をパワーアップさせますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "パワーアップ") <> 1 Then
				GoTo Beginning
			End If
		Else
			If MsgBox(u.Nickname0 & "を改造しますか？", MsgBoxStyle.OKCancel + MsgBoxStyle.Question, "改造") <> 1 Then
				GoTo Beginning
			End If
		End If
		
		'資金を減らす
		IncrMoney(-RankUpCost(u))
		
		'ユニットランクを一段階上げる
		With u
			.Rank = .Rank + 1
			.HP = .MaxHP
			.EN = .MaxEN
			
			'他形態のランクも上げておく
			For i = 1 To .CountOtherForm
				.OtherForm(i).Rank = .Rank
				.OtherForm(i).HP = .OtherForm(i).MaxHP
				.OtherForm(i).EN = .OtherForm(i).MaxEN
			Next 
			
			'合体形態が主形態の分離形態が改造された場合は他の分離形態のユニットの
			'ランクも上げる
			If .IsFeatureAvailable("合体") Then
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
					urank = .Rank
					buf = UDList.Item(LIndex(.FeatureData(i), 2)).FeatureData("分離")
					For i = 2 To LLength(buf)
						If Not UList.IsDefined(LIndex(buf, i)) Then
							GoTo NextForm
						End If
						
						With UList.Item(LIndex(buf, i))
							.Rank = MaxLng(urank, .Rank)
							.HP = .MaxHP
							.EN = .MaxEN
							For j = 1 To .CountOtherForm
								.OtherForm(j).Rank = .Rank
								.OtherForm(j).HP = .OtherForm(j).MaxHP
								.OtherForm(j).EN = .OtherForm(j).MaxEN
							Next 
							
							For j = 1 To UBound(id_list)
								If .CurrentForm.ID = id_list(j) Then
									Exit For
								End If
							Next 
							
							If j > UBound(id_list) Then
								GoTo NextForm
							End If
							
							If use_max_rank Then
								list(j) = RightPaddedString(.Nickname0, name_width) & LeftPaddedString(VB6.Format(.Rank), 2) & "/"
								If MaxRank(u) > 0 Then
									list(j) = list(j) & LeftPaddedString(VB6.Format(MaxRank(u)), 2)
								Else
									list(j) = list(j) & "--"
								End If
							Else
								If .Rank < 10 Then
									list(j) = RightPaddedString(.Nickname0, name_width) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
								Else
									list(j) = RightPaddedString(.Nickname0, name_width) & VB6.Format(.Rank)
								End If
							End If
							
							If RankUpCost(u) < 1000000 Then
								list(j) = list(j) & LeftPaddedString(VB6.Format(RankUpCost(u)), 7)
							Else
								list(j) = list(j) & LeftPaddedString("----", 7)
							End If
							
							If IsOptionDefined("等身大基準") Then
								If .CountPilot > 0 Then
									list(j) = list(j) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
								End If
							End If
							list(j) = list(j) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 6) & LeftPaddedString(VB6.Format(.Mobility), 4)
							If Not IsOptionDefined("等身大基準") Then
								If .CountPilot > 0 Then
									list(j) = list(j) & "  " & .MainPilot.Nickname
								End If
							End If
						End With
NextForm: 
					Next 
				End If
			End If
			
			'合体ユニットの場合は分離形態のユニットのランクも上げる
			If .IsFeatureAvailable("分離") Then
				urank = .Rank
				buf = .FeatureData("分離")
				For i = 2 To LLength(buf)
					If UList.IsDefined(LIndex(buf, i)) Then
						With UList.Item(LIndex(buf, i))
							.Rank = MaxLng(urank, .Rank)
							.HP = .MaxHP
							.EN = .MaxEN
							For j = 1 To .CountOtherForm
								.OtherForm(j).Rank = .Rank
								.OtherForm(j).HP = .OtherForm(j).MaxHP
								.OtherForm(j).EN = .OtherForm(j).MaxEN
							Next 
						End With
					End If
				Next 
			End If
			
			'ユニットリストの表示内容を更新
			
			If use_max_rank Then
				list(ret) = RightPaddedString(.Nickname0, name_width) & LeftPaddedString(VB6.Format(.Rank), 2) & "/"
				If MaxRank(u) > 0 Then
					list(ret) = list(ret) & LeftPaddedString(VB6.Format(MaxRank(u)), 2)
				Else
					list(ret) = list(ret) & "--"
				End If
			Else
				If .Rank < 10 Then
					list(ret) = RightPaddedString(.Nickname0, name_width) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
				Else
					list(ret) = RightPaddedString(.Nickname0, name_width) & VB6.Format(.Rank)
				End If
			End If
			
			If RankUpCost(u) < 10000000 Then
				list(ret) = list(ret) & LeftPaddedString(VB6.Format(RankUpCost(u)), 7)
			Else
				list(ret) = list(ret) & LeftPaddedString("----", 7)
			End If
			
			If IsOptionDefined("等身大基準") Then
				If .CountPilot > 0 Then
					list(ret) = list(ret) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
				End If
			End If
			list(ret) = list(ret) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 6) & LeftPaddedString(VB6.Format(.Mobility), 4)
			If Not IsOptionDefined("等身大基準") Then
				If .CountPilot > 0 Then
					list(ret) = list(ret) & "  " & .MainPilot.Nickname
				End If
			End If
		End With
		
		'改めて資金と改造費を調べ、各ユニットが改造可能かチェックする
		For i = 2 To UBound(list)
			cost = RankUpCost(UList.Item(id_list(i)))
			If cost > Money Or cost > 10000000 Then
				ListItemFlag(i) = True
			Else
				ListItemFlag(i) = False
			End If
		Next 
		
		GoTo Beginning
	End Sub
	
	'ユニットランクを上げるためのコストを算出
	Public Function RankUpCost(ByRef u As Unit) As Integer
		With u
			'これ以上改造できない？
			If .Rank >= MaxRank(u) Then
				RankUpCost = 999999999
				Exit Function
			End If
			
			'合体状態にある場合はそれが主形態でない限り改造不可
			If .IsFeatureAvailable("分離") Then
				If (LLength(.FeatureData("分離")) = 3 And Not .IsFeatureAvailable("主形態")) Or .IsFeatureAvailable("制限時間") Then
					RankUpCost = 999999999
					Exit Function
				End If
			End If
			
			If IsOptionDefined("低改造費") Then
				'低改造費の場合
				Select Case .Rank
					Case 0
						RankUpCost = 10000
					Case 1
						RankUpCost = 15000
					Case 2
						RankUpCost = 20000
					Case 3
						RankUpCost = 30000
					Case 4
						RankUpCost = 40000
					Case 5
						RankUpCost = 50000
					Case 6
						RankUpCost = 60000
					Case 7
						RankUpCost = 70000
					Case 8
						RankUpCost = 80000
					Case 9
						RankUpCost = 100000
					Case 10
						RankUpCost = 120000
					Case 11
						RankUpCost = 140000
					Case 12
						RankUpCost = 160000
					Case 13
						RankUpCost = 180000
					Case 14
						RankUpCost = 200000
					Case Else
						RankUpCost = 999999999
						Exit Function
				End Select
			ElseIf IsOptionDefined("１５段階改造") Then 
				'通常の１５段改造
				'(１０段改造時よりお求め安い価格になっております……)
				Select Case .Rank
					Case 0
						RankUpCost = 10000
					Case 1
						RankUpCost = 15000
					Case 2
						RankUpCost = 20000
					Case 3
						RankUpCost = 40000
					Case 4
						RankUpCost = 80000
					Case 5
						RankUpCost = 120000
					Case 6
						RankUpCost = 160000
					Case 7
						RankUpCost = 200000
					Case 8
						RankUpCost = 250000
					Case 9
						RankUpCost = 300000
					Case 10
						RankUpCost = 350000
					Case 11
						RankUpCost = 400000
					Case 12
						RankUpCost = 450000
					Case 13
						RankUpCost = 500000
					Case 14
						RankUpCost = 550000
					Case Else
						RankUpCost = 999999999
						Exit Function
				End Select
			Else
				'通常の１０段改造
				Select Case .Rank
					Case 0
						RankUpCost = 10000
					Case 1
						RankUpCost = 15000
					Case 2
						RankUpCost = 20000
					Case 3
						RankUpCost = 40000
					Case 4
						RankUpCost = 80000
					Case 5
						RankUpCost = 150000
					Case 6
						RankUpCost = 200000
					Case 7
						RankUpCost = 300000
					Case 8
						RankUpCost = 400000
					Case 9
						RankUpCost = 500000
					Case Else
						RankUpCost = 999999999
						Exit Function
				End Select
			End If
			
			'ユニット用特殊能力「改造費修正」による修正
			If .IsFeatureAvailable("改造費修正") Then
				RankUpCost = RankUpCost * (1# + .FeatureLevel("改造費修正") / 10)
			End If
		End With
	End Function
	
	'ユニットの最大改造数を算出
	Public Function MaxRank(ByRef u As Unit) As Integer
		If IsOptionDefined("５段階改造") Then
			'５段階改造までしか出来ない
			MaxRank = 5
		ElseIf IsOptionDefined("１５段階改造") Then 
			'１５段階改造まで可能
			MaxRank = 15
		Else
			'デフォルトは１０段階まで
			MaxRank = 10
		End If
		
		With u
			'Disableコマンドで改造不可にされている？
			If IsGlobalVariableDefined("Disable(" & .Name & ",改造)") Then
				MaxRank = 0
				Exit Function
			End If
			
			'最大改造数が設定されている？
			If .IsFeatureAvailable("最大改造数") Then
				MaxRank = MinLng(MaxRank, .FeatureLevel("最大改造数"))
			End If
		End With
	End Function
	
	'乗り換えコマンド
	Public Sub ExchangeUnitCommand()
		Dim j, i, k As Short
		Dim list() As String
		Dim id_list() As String
		Dim sort_mode, sort_mode2 As String
		Dim sort_mode_type() As String
		Dim sort_mode_list() As String
		Dim item_flag_backup() As Boolean
		Dim item_comment_backup() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim u As Unit
		Dim p As Pilot
		Dim pname As String
		Dim buf As String
		Dim ret As Short
		Dim b As Boolean
		Dim is_support As Boolean
		Dim caption_str As String
		Dim top_item As Short
		
		top_item = 1
		
		'デフォルトのソート方法
		sort_mode = "レベル"
		sort_mode2 = "名称"
		
Beginning: 
		
		'乗り換えるパイロットの一覧を作成
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemComment(1)
		list(1) = "▽並べ替え▽"
		For	Each p In PList
			With p
				If .Party <> "味方" Or .Away Or IsGlobalVariableDefined("Fix(" & .Name & ")") Then
					GoTo NextLoop
				End If
				
				'追加パイロット＆サポートは乗り換え不可
				If .IsAdditionalPilot Or .IsAdditionalSupport Then
					GoTo NextLoop
				End If
				
				is_support = False
				If Not .Unit_Renamed Is Nothing Then
					'サポートが複数乗っている場合は乗り降り不可
					If .Unit_Renamed.CountSupport > 1 Then
						GoTo NextLoop
					End If
					
					'サポートパイロットとして乗り込んでいるかを判定
					If .Unit_Renamed.CountSupport = 1 Then
						If .ID = .Unit_Renamed.Support(1).ID Then
							is_support = True
						End If
					End If
					
					'通常のパイロットの場合
					If Not is_support Then
						'３人乗り以上は乗り降り不可
						If .Unit_Renamed.Data.PilotNum <> 1 And System.Math.Abs(.Unit_Renamed.Data.PilotNum) <> 2 Then
							GoTo NextLoop
						End If
					End If
				End If
				
				If is_support Then
					'サポートパイロットの場合
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'パイロットのステータス
					list(UBound(list)) = RightPaddedString("*" & .Nickname, 25) & LeftPaddedString(StrConv(VB6.Format(.Level), VbStrConv.Wide), 4)
					
					If Not .Unit_Renamed Is Nothing Then
						With .Unit_Renamed
							'ユニットのステータス
							list(UBound(list)) = list(UBound(list)) & "  " & RightPaddedString(.Nickname0, 29) & "(" & .MainPilot.Nickname & ")"
							
							'ユニットが装備しているアイテム一覧
							For k = 1 To .CountItem
								With .Item(k)
									If (.Class_Renamed() <> "固定" Or Not .IsFeatureAvailable("非表示")) And .Part <> "非表示" Then
										ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
									End If
								End With
							Next 
						End With
					End If
					
					'パイロットＩＤを記録しておく
					id_list(UBound(list)) = .ID
				ElseIf .Unit_Renamed Is Nothing Then 
					'ユニットに乗っていないパイロットの場合
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'パイロットのステータス
					list(UBound(list)) = RightPaddedString(" " & .Nickname, 25) & LeftPaddedString(StrConv(VB6.Format(.Level), VbStrConv.Wide), 4)
					
					'パイロットＩＤを記録しておく
					id_list(UBound(list)) = .ID
				ElseIf .Unit_Renamed.CountPilot <= 2 Then 
					'複数乗りのユニットに乗っているパイロットの場合
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'パイロットが足りない？
					If .Unit_Renamed.CountPilot < System.Math.Abs(.Unit_Renamed.Data.PilotNum) Then
						list(UBound(list)) = "-"
					Else
						list(UBound(list)) = " "
					End If
					
					If .Unit_Renamed.IsFeatureAvailable("追加パイロット") Then
						pname = .Unit_Renamed.MainPilot.Nickname
					Else
						pname = .Nickname
					End If
					
					'複数乗りの場合は何番目のパイロットか表示
					If System.Math.Abs(.Unit_Renamed.Data.PilotNum) > 1 Then
						For k = 1 To .Unit_Renamed.CountPilot
							If .Unit_Renamed.Pilot(k) Is p Then
								pname = pname & "(" & VB6.Format(k) & ")"
							End If
						Next 
					End If
					
					'パイロット＆ユニットのステータス
					list(UBound(list)) = list(UBound(list)) & RightPaddedString(pname, 24) & LeftPaddedString(StrConv(VB6.Format(.Level), VbStrConv.Wide), 4) & "  " & RightPaddedString((.Unit_Renamed.Nickname0), 29)
					If .Unit_Renamed.CountSupport > 0 Then
						list(UBound(list)) = list(UBound(list)) & "(" & .Unit_Renamed.Support(1).Nickname & ")"
					End If
					
					'ユニットが装備しているアイテム一覧
					With .Unit_Renamed
						For k = 1 To .CountItem
							With .Item(k)
								If (.Class_Renamed() <> "固定" Or Not .IsFeatureAvailable("非表示")) And .Part <> "非表示" Then
									ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
								End If
							End With
						Next 
					End With
					
					'パイロットＩＤを記録しておく
					id_list(UBound(list)) = .ID
				End If
			End With
NextLoop: 
		Next p
		ReDim ListItemFlag(UBound(list))
		
SortAgain: 
		
		'ソート
		If sort_mode = "レベル" Then
			'レベルによるソート
			
			'まずレベルのリストを作成
			ReDim key_list(UBound(list))
			With PList
				For i = 2 To UBound(list)
					With .Item(id_list(i))
						key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
					End With
				Next 
			End With
			
			'レベルを使って並べ換え
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'読み仮名によるソート
			
			'まず読み仮名のリストを作成
			ReDim strkey_list(UBound(list))
			With PList
				For i = 2 To UBound(list)
					strkey_list(i) = .Item(id_list(i)).KanaName
				Next 
			End With
			
			'読み仮名を使って並べ替え
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'パイロットを選択
		TopItem = top_item
		If IsOptionDefined("等身大基準") Then
			caption_str = " キャラクター          レベル  ユニット"
			ret = ListBox("キャラクター選択", list, caption_str, "連続表示,コメント")
		Else
			caption_str = " パイロット            レベル  ユニット"
			ret = ListBox("パイロット選択", list, caption_str, "連続表示,コメント")
		End If
		top_item = TopItem
		
		Select Case ret
			Case 0
				'キャンセル
				Exit Sub
			Case 1
				'ソート方法を選択
				ReDim sort_mode_list(2)
				sort_mode_list(1) = "レベル"
				sort_mode_list(2) = "名称"
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				ret = ListBox("どれで並べ替えますか？", sort_mode_list, "並べ替えの方法", "連続表示,コメント")
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'ソート方法を変更して再表示
				If ret > 0 Then
					sort_mode = sort_mode_list(ret)
				End If
				GoTo SortAgain
		End Select
		
		'乗り換えさせるパイロット
		p = PList.Item(id_list(ret))
		
		'乗り換え先ユニット一覧作成
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemComment(1)
		list(1) = "▽並べ替え▽"
		For	Each u In UList
			With u
				If .Party0 <> "味方" Or .Status_Renamed <> "待機" Then
					GoTo NextUnit
				End If
				
				If .CountSupport > 1 Then
					If InStr(p.Class_Renamed, "専属サポート") = 0 Then
						GoTo NextUnit
					End If
				End If
				
				If u Is p.Unit_Renamed Then
					GoTo NextUnit
				End If
				
				If Not p.IsAbleToRide(u) Then
					GoTo NextUnit
				End If
				
				'サポートキャラでなければ乗り換えられるパイロット数に制限がある
				If Not p.IsSupport(u) Then
					If .Data.PilotNum <> 1 And System.Math.Abs(.Data.PilotNum) <> 2 Then
						GoTo NextUnit
					End If
				End If
				
				If .CountPilot > 0 Then
					If IsGlobalVariableDefined("Fix(" & .Pilot(1).Name & ")") And Not p.IsSupport(u) Then
						'Fixコマンドでパイロットが固定されたユニットはサポートでない
						'限り乗り換え不可
						GoTo NextUnit
					End If
					
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'パイロットが足りている？
					If .CountPilot < System.Math.Abs(.Data.PilotNum) Then
						list(UBound(list)) = "-"
					Else
						list(UBound(list)) = " "
					End If
					
					list(UBound(list)) = list(UBound(list)) & RightPaddedString(.Nickname0, 35) & RightPaddedString(.MainPilot.Nickname, 21)
					If .Rank < 10 Then
						list(UBound(list)) = list(UBound(list)) & " " & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = list(UBound(list)) & " " & VB6.Format(.Rank)
					End If
					If .CountSupport > 0 Then
						list(UBound(list)) = list(UBound(list)) & " (" & .Support(1).Nickname & ")"
					End If
					
					'ユニットに装備されているアイテムをコメント欄に列記
					For j = 1 To .CountItem
						With .Item(j)
							If (.Class_Renamed() <> "固定" Or Not .IsFeatureAvailable("非表示")) And .Part <> "非表示" Then
								ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
							End If
						End With
					Next 
					
					'ユニットＩＤを記録しておく
					id_list(UBound(list)) = .ID
				ElseIf Not p.IsSupport(u) Then 
					'誰も乗ってないユニットに乗れるのは通常パイロットのみ
					
					ReDim Preserve list(UBound(list) + 1)
					ReDim Preserve id_list(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					list(UBound(list)) = " " & RightPaddedString(.Nickname0, 35) & Space(21)
					If .Rank < 10 Then
						list(UBound(list)) = list(UBound(list)) & " " & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = list(UBound(list)) & " " & VB6.Format(.Rank)
					End If
					
					'ユニットに装備されているアイテムをコメント欄に列記
					For j = 1 To .CountItem
						With .Item(j)
							If (.Class_Renamed() <> "固定" Or Not .IsFeatureAvailable("非表示")) And .Part <> "非表示" Then
								ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
							End If
						End With
					Next 
					
					'ユニットＩＤを記録しておく
					id_list(UBound(list)) = .ID
				End If
			End With
NextUnit: 
		Next u
		ReDim ListItemFlag(UBound(list))
		
SortAgain2: 
		
		'ソート
		If InStr(sort_mode2, "名称") = 0 Then
			'数値によるソート
			
			'まずキーのリストを作成
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode2
					Case "ＨＰ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxHP
						Next 
					Case "ＥＮ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxEN
						Next 
					Case "装甲"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Armor
						Next 
					Case "運動性"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Mobility
						Next 
					Case "ユニットランク"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Rank
						Next 
				End Select
			End With
			
			'キーを使って並べ替え
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'読み仮名によるソート
			
			'まず読み仮名のリストを作成
			ReDim strkey_list(UBound(list))
			With UList
				For i = 2 To UBound(list)
					strkey_list(i) = .Item(id_list(i)).KanaName
				Next 
			End With
			
			'読み仮名を使って並べ替え
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					b = ListItemFlag(i)
					ListItemFlag(i) = ListItemFlag(max_item)
					ListItemFlag(max_item) = b
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'乗り換え先を選択
		TopItem = 1
		u = p.Unit_Renamed
		If IsOptionDefined("等身大基準") Then
			caption_str = " ユニット                           キャラクター       " & Term("ランク")
		Else
			caption_str = " ユニット                           パイロット         " & Term("ランク")
		End If
		If Not u Is Nothing Then
			If u.IsFeatureAvailable("追加パイロット") Then
				ret = ListBox("乗り換え先選択 ： " & u.MainPilot.Nickname & " (" & u.Nickname & ")", list, caption_str, "連続表示,コメント")
			Else
				ret = ListBox("乗り換え先選択 ： " & p.Nickname & " (" & u.Nickname & ")", list, caption_str, "連続表示,コメント")
			End If
		Else
			ret = ListBox("乗り換え先選択 ： " & p.Nickname, list, caption_str, "連続表示,コメント")
		End If
		
		Select Case ret
			Case 0
				'キャンセル
				Exit Sub
			Case 1
				'ソート方法を選択
				ReDim sort_mode_type(6)
				ReDim sort_mode_list(6)
				If IsOptionDefined("等身大基準") Then
					sort_mode_type(1) = "名称"
					sort_mode_list(1) = "名称"
					sort_mode_type(2) = "ＨＰ"
					sort_mode_list(2) = Term("ＨＰ")
					sort_mode_type(3) = "ＥＮ"
					sort_mode_list(3) = Term("ＥＮ")
					sort_mode_type(4) = "装甲"
					sort_mode_list(4) = Term("装甲")
					sort_mode_type(5) = "運動性"
					sort_mode_list(5) = Term("運動性")
					sort_mode_type(6) = "ユニットランク"
					sort_mode_list(6) = Term("ランク")
				Else
					sort_mode_type(1) = "ＨＰ"
					sort_mode_list(1) = Term("ＨＰ")
					sort_mode_type(2) = "ＥＮ"
					sort_mode_list(2) = Term("ＥＮ")
					sort_mode_type(3) = "装甲"
					sort_mode_list(3) = Term("装甲")
					sort_mode_type(4) = "運動性"
					sort_mode_list(4) = Term("運動性")
					sort_mode_type(5) = "ユニットランク"
					sort_mode_list(5) = Term("ランク")
					sort_mode_type(6) = "ユニット名称"
					sort_mode_list(6) = "ユニット名称"
				End If
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				TopItem = 1
				ret = ListBox("どれで並べ替えますか？", sort_mode_list, "並べ替えの方法", "連続表示,コメント")
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'ソート方法を変更して再表示
				If ret > 0 Then
					sort_mode2 = sort_mode_type(ret)
				End If
				GoTo SortAgain2
		End Select
		
		'キャンセル？
		If ret = 0 Then
			GoTo Beginning
		End If
		
		u = UList.Item(id_list(ret))
		
		'元のユニットから降ろす
		p.GetOff()
		
		'乗り換え
		With u
			If Not p.IsSupport(u) Then
				'通常のパイロット
				If .CountPilot = .Data.PilotNum Then
					.Pilot(1).GetOff()
				End If
			Else
				'サポートパイロット
				For i = 1 To .CountSupport
					.Support(1).GetOff()
				Next 
			End If
		End With
		p.Ride(UList.Item(id_list(ret)))
		
		GoTo Beginning
	End Sub
	
	'アイテム交換コマンド
	Public Sub ExchangeItemCommand(Optional ByRef selected_unit As Unit = Nothing, Optional ByRef selected_part As String = "")
		Dim j, i, k As Short
		Dim inum, inum2 As Short
		Dim list() As String
		Dim id_list() As String
		Dim iid As String
		Dim sort_mode As String
		Dim sort_mode_type(7) As String
		Dim sort_mode_list(7) As String
		Dim item_flag_backup() As Boolean
		Dim item_comment_backup() As String
		Dim key_list() As Integer
		Dim strkey_list() As String
		Dim max_item As Short
		Dim max_value As Integer
		Dim max_str As String
		Dim caption_str As String
		Dim u As Unit
		Dim it As Item
		Dim iname As String
		Dim buf As String
		Dim ret As Short
		Dim part_list() As String
		Dim part_item() As String
		Dim arm_point, shoulder_point As Short
		Dim ipart As String
		Dim empty_slot As Short
		Dim is_right_hand_available As Boolean
		Dim is_left_hand_available As Boolean
		Dim item_list() As String
		Dim top_item1, top_item2 As Short
		
		top_item1 = 1
		top_item2 = 1
		
		'デフォルトのソート方法
		If IsOptionDefined("等身大基準") Then
			sort_mode = "レベル"
		Else
			sort_mode = "ＨＰ"
		End If
		
		'ユニットがあらかじめ選択されている場合
		'(ユニットステータスからのアイテム交換時)
		If Not selected_unit Is Nothing Then
			EnlargeListBoxHeight()
			ReduceListBoxWidth()
			
			u = selected_unit
			If MainForm.Visible Then
				If Not u Is DisplayedUnit Then
					DisplayUnitStatus(u)
				End If
			End If
			
			GoTo MakeEquipedItemList
		End If
		
Beginning: 
		
		'ユニット一覧の作成
		ReDim list(1)
		ReDim id_list(1)
		ReDim ListItemComment(1)
		list(1) = "▽並べ替え▽"
		For	Each u In UList
			With u
				If .Party0 <> "味方" Or .Status_Renamed <> "待機" Then
					GoTo NextUnit
				End If
				
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve ListItemComment(UBound(list))
				
				'装備しているアイテムの数を数える
				inum = 0
				inum2 = 0
				For i = 1 To .CountItem
					With .Item(i)
						If (.Class_Renamed() <> "固定" Or Not .IsFeatureAvailable("非表示")) And .Part <> "非表示" Then
							ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
							If .Part = "強化パーツ" Or .Part = "アイテム" Then
								inum = inum + .Size
							Else
								inum2 = inum2 + .Size
							End If
						End If
					End With
				Next 
				
				'リストを作成
				If IsOptionDefined("等身大基準") Then
					list(UBound(list)) = RightPaddedString(.Nickname0, 39)
				Else
					list(UBound(list)) = RightPaddedString(.Nickname0, 31)
				End If
				list(UBound(list)) = list(UBound(list)) & VB6.Format(inum) & "/" & VB6.Format(.MaxItemNum)
				If inum2 > 0 Then
					list(UBound(list)) = list(UBound(list)) & "(" & VB6.Format(inum2) & ")   "
				Else
					list(UBound(list)) = list(UBound(list)) & "      "
				End If
				If .Rank < 10 Then
					list(UBound(list)) = list(UBound(list)) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
				Else
					list(UBound(list)) = list(UBound(list)) & VB6.Format(.Rank)
				End If
				If IsOptionDefined("等身大基準") Then
					If .CountPilot > 0 Then
						list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MainPilot.Level), 3)
					End If
				End If
				list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 4) & LeftPaddedString(VB6.Format(.Armor), 5) & LeftPaddedString(VB6.Format(.Mobility), 5)
				If Not IsOptionDefined("等身大基準") Then
					If .CountPilot > 0 Then
						list(UBound(list)) = list(UBound(list)) & " " & .MainPilot.Nickname
					End If
				End If
				
				'ユニットＩＤを記録しておく
				id_list(UBound(list)) = .ID
			End With
NextUnit: 
		Next u
		ReDim ListItemFlag(UBound(list))
		
SortAgain: 
		
		'ソート
		If InStr(sort_mode, "名称") = 0 Then
			'数値によるソート
			
			'まずキーのリストを作成
			ReDim key_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "ＨＰ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxHP
						Next 
					Case "ＥＮ"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).MaxEN
						Next 
					Case "装甲"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Armor
						Next 
					Case "運動性"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Mobility
						Next 
					Case "ユニットランク"
						For i = 2 To UBound(list)
							key_list(i) = .Item(id_list(i)).Rank
						Next 
					Case "レベル"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									With .MainPilot
										key_list(i) = 500 * CInt(.Level) + CInt(.Exp)
									End With
								End If
							End With
						Next 
				End Select
			End With
			
			'キーを使って並べ替え
			For i = 2 To UBound(list) - 1
				max_item = i
				max_value = key_list(i)
				For j = i + 1 To UBound(list)
					If key_list(j) > max_value Then
						max_item = j
						max_value = key_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					key_list(max_item) = key_list(i)
				End If
			Next 
		Else
			'文字列によるソート
			
			'まずはキーのリストを作成
			ReDim strkey_list(UBound(list))
			With UList
				Select Case sort_mode
					Case "名称", "ユニット名称"
						For i = 2 To UBound(list)
							strkey_list(i) = .Item(id_list(i)).KanaName
						Next 
					Case "パイロット名称"
						For i = 2 To UBound(list)
							With .Item(id_list(i))
								If .CountPilot() > 0 Then
									strkey_list(i) = .MainPilot.KanaName
								End If
							End With
						Next 
				End Select
			End With
			
			'キーを使って並べ替え
			For i = 2 To UBound(strkey_list) - 1
				max_item = i
				max_str = strkey_list(i)
				For j = i + 1 To UBound(strkey_list)
					If StrComp(strkey_list(j), max_str, 1) = -1 Then
						max_item = j
						max_str = strkey_list(j)
					End If
				Next 
				If max_item <> i Then
					buf = list(i)
					list(i) = list(max_item)
					list(max_item) = buf
					
					buf = id_list(i)
					id_list(i) = id_list(max_item)
					id_list(max_item) = buf
					
					buf = ListItemComment(i)
					ListItemComment(i) = ListItemComment(max_item)
					ListItemComment(max_item) = buf
					
					strkey_list(max_item) = strkey_list(i)
				End If
			Next 
		End If
		
		'アイテムを交換するユニットを選択
		TopItem = top_item1
		If IsOptionDefined("等身大基準") Then
			ret = ListBox("アイテムを交換するユニットを選択", list, "ユニット                               アイテム " & Term("RK", Nothing, 2) & " Lv  " & Term("ＨＰ", Nothing, 4) & " " & Term("ＥＮ", Nothing, 4) & " " & Term("装甲", Nothing, 4) & " " & Term("運動"), "連続表示,コメント")
		Else
			ret = ListBox("アイテムを交換するユニットを選択", list, "ユニット                       アイテム " & Term("RK", Nothing, 2) & "  " & Term("ＨＰ", Nothing, 4) & " " & Term("ＥＮ", Nothing, 4) & " " & Term("装甲", Nothing, 4) & " " & Term("運動", Nothing, 4) & " パイロット", "連続表示,コメント")
		End If
		top_item1 = TopItem
		
		Select Case ret
			Case 0
				'キャンセル
				Exit Sub
			Case 1
				'ソート方法を選択
				If IsOptionDefined("等身大基準") Then
					sort_mode_type(1) = "名称"
					sort_mode_list(1) = "名称"
					sort_mode_type(2) = "レベル"
					sort_mode_list(2) = "レベル"
					sort_mode_type(3) = "ＨＰ"
					sort_mode_list(3) = Term("ＨＰ")
					sort_mode_type(4) = "ＥＮ"
					sort_mode_list(4) = Term("ＥＮ")
					sort_mode_type(5) = "装甲"
					sort_mode_list(5) = Term("装甲")
					sort_mode_type(6) = "運動性"
					sort_mode_list(6) = Term("運動性")
					sort_mode_type(7) = "ユニットランク"
					sort_mode_list(7) = Term("ランク")
				Else
					sort_mode_type(1) = "ＨＰ"
					sort_mode_list(1) = Term("ＨＰ")
					sort_mode_type(2) = "ＥＮ"
					sort_mode_list(2) = Term("ＥＮ")
					sort_mode_type(3) = "装甲"
					sort_mode_list(3) = Term("装甲")
					sort_mode_type(4) = "運動性"
					sort_mode_list(4) = Term("運動性")
					sort_mode_type(5) = "ユニットランク"
					sort_mode_list(5) = Term("ランク")
					sort_mode_type(6) = "ユニット名称"
					sort_mode_list(6) = "ユニット名称"
					sort_mode_type(7) = "パイロット名称"
					sort_mode_list(7) = "パイロット名称"
				End If
				ReDim item_flag_backup(UBound(list))
				ReDim item_comment_backup(UBound(list))
				For i = 2 To UBound(list)
					item_flag_backup(i) = ListItemFlag(i)
					item_comment_backup(i) = ListItemComment(i)
				Next 
				ReDim ListItemComment(UBound(sort_mode_list))
				ReDim ListItemFlag(UBound(sort_mode_list))
				
				TopItem = 1
				ret = ListBox("どれで並べ替えますか？", sort_mode_list, "並べ替えの方法", "連続表示,コメント")
				
				ReDim ListItemFlag(UBound(list))
				ReDim ListItemComment(UBound(list))
				For i = 2 To UBound(list)
					ListItemFlag(i) = item_flag_backup(i)
					ListItemComment(i) = item_comment_backup(i)
				Next 
				
				'ソート方法を変更して再表示
				If ret > 0 Then
					sort_mode = sort_mode_type(ret)
				End If
				GoTo SortAgain
		End Select
		
		'ユニットを選択
		u = UList.Item(id_list(ret))
		
MakeEquipedItemList: 
		
		'選択されたユニットが装備しているアイテム一覧の作成
		Dim tmp_part_list() As String
		With u
			Do While True
				'アイテムの装備個所一覧を作成
				ReDim part_list(0)
				If .IsFeatureAvailable("装備個所") Then
					buf = .FeatureData("装備個所")
					If InStr(buf, "腕") > 0 Then
						arm_point = UBound(part_list) + 1
						ReDim Preserve part_list(UBound(part_list) + 2)
						part_list(1) = "右手"
						part_list(2) = "左手"
					End If
					If InStr(buf, "肩") > 0 Then
						shoulder_point = UBound(part_list) + 1
						ReDim Preserve part_list(UBound(part_list) + 2)
						part_list(UBound(part_list) - 1) = "右肩"
						part_list(UBound(part_list)) = "左肩"
					End If
					If InStr(buf, "体") > 0 Then
						ReDim Preserve part_list(UBound(part_list) + 1)
						part_list(UBound(part_list)) = "体"
					End If
					If InStr(buf, "頭") > 0 Then
						ReDim Preserve part_list(UBound(part_list) + 1)
						part_list(UBound(part_list)) = "頭"
					End If
				End If
				For i = 1 To .CountFeature
					If .Feature(i) = "ハードポイント" Then
						ipart = .FeatureData(i)
						Select Case ipart
							Case "強化パーツ", "アイテム", "非表示"
								'表示しない
							Case Else
								For j = 1 To UBound(part_list)
									If part_list(j) = ipart Then
										Exit For
									End If
								Next 
								If j > UBound(part_list) Then
									ReDim Preserve part_list(UBound(part_list) + .ItemSlotSize(ipart))
									For j = UBound(part_list) - .ItemSlotSize(ipart) + 1 To UBound(part_list)
										part_list(j) = ipart
									Next 
								End If
						End Select
					End If
				Next 
				
				ReDim Preserve part_list(UBound(part_list) + .MaxItemNum)
				If .IsHero Then
					For i = UBound(part_list) - .MaxItemNum + 1 To UBound(part_list)
						part_list(i) = "アイテム"
					Next 
				Else
					For i = UBound(part_list) - .MaxItemNum + 1 To UBound(part_list)
						part_list(i) = "強化パーツ"
					Next 
				End If
				
				'特定の装備個所のアイテムのみを交換する？
				If selected_part <> "" Then
					
					ReDim tmp_part_list(UBound(part_list))
					For i = 1 To UBound(part_list)
						tmp_part_list(i) = part_list(i)
					Next 
					
					ReDim part_list(0)
					arm_point = 0
					shoulder_point = 0
					For i = 1 To UBound(tmp_part_list)
						If tmp_part_list(i) = selected_part Or ((selected_part = "片手" Or selected_part = "両手" Or selected_part = "盾") And (tmp_part_list(i) = "右手" Or tmp_part_list(i) = "左手")) Or ((selected_part = "肩" Or selected_part = "両肩") And (tmp_part_list(i) = "右肩" Or tmp_part_list(i) = "左肩")) Or ((selected_part = "アイテム" Or selected_part = "強化パーツ") And (tmp_part_list(i) = "アイテム" Or tmp_part_list(i) = "強化パーツ")) Then
							ReDim Preserve part_list(UBound(part_list) + 1)
							part_list(UBound(part_list)) = tmp_part_list(i)
							Select Case part_list(UBound(part_list))
								Case "右手"
									arm_point = UBound(part_list)
								Case "右肩"
									shoulder_point = UBound(part_list)
							End Select
						End If
					Next 
				End If
				
				ReDim part_item(UBound(part_list))
				
				'装備個所に現在装備しているアイテムを割り当て
				For i = 1 To .CountItem
					With .Item(i)
						If .Class_Renamed() = "固定" And .IsFeatureAvailable("非表示") Then
							GoTo NextEquipedItem
						End If
						
						Select Case .Part
							Case "両手"
								If arm_point = 0 Then
									GoTo NextEquipedItem
								End If
								part_item(arm_point) = .ID
								part_item(arm_point + 1) = ":"
							Case "片手"
								If arm_point = 0 Then
									GoTo NextEquipedItem
								End If
								If part_item(arm_point) = "" Then
									part_item(arm_point) = .ID
								Else
									part_item(arm_point + 1) = .ID
								End If
							Case "盾"
								If arm_point = 0 Then
									GoTo NextEquipedItem
								End If
								part_item(arm_point + 1) = .ID
							Case "両肩"
								If shoulder_point = 0 Then
									GoTo NextEquipedItem
								End If
								part_item(shoulder_point) = .ID
							Case "肩"
								If shoulder_point = 0 Then
									GoTo NextEquipedItem
								End If
								If part_item(shoulder_point) = "" Then
									part_item(shoulder_point) = .ID
								Else
									part_item(shoulder_point + 1) = .ID
								End If
							Case "非表示"
								'無視
							Case Else
								If .Part = "強化パーツ" Or .Part = "アイテム" Then
									For j = 1 To UBound(part_list)
										If (part_list(j) = "強化パーツ" Or part_list(j) = "アイテム") And part_item(j) = "" Then
											part_item(j) = .ID
											For k = j + 1 To j + .Size - 1
												If k > UBound(part_item) Then
													Exit For
												End If
												part_item(k) = ":"
											Next 
											Exit For
										End If
									Next 
								Else
									For j = 1 To UBound(part_list)
										If part_list(j) = .Part And part_item(j) = "" Then
											part_item(j) = .ID
											For k = j + 1 To j + .Size - 1
												If k > UBound(part_item) Then
													Exit For
												End If
												part_item(k) = ":"
											Next 
											Exit For
										End If
									Next 
								End If
								If j > UBound(part_list) And selected_part = "" Then
									ReDim Preserve part_list(UBound(part_list) + 1)
									ReDim Preserve part_item(UBound(part_list))
									part_list(UBound(part_list)) = .Part
									part_item(UBound(part_list)) = .ID
								End If
						End Select
					End With
NextEquipedItem: 
				Next 
				
				ReDim list(UBound(part_list) + 1)
				ReDim id_list(UBound(list))
				ReDim ListItemComment(UBound(list))
				ReDim ListItemFlag(UBound(list))
				
				'リストを構築
				For i = 1 To UBound(part_item)
					Select Case part_item(i)
						Case ""
							list(i) = RightPaddedString("----", 23) & part_list(i)
						Case ":"
							list(i) = RightPaddedString(" :  ", 23) & part_list(i)
							ListItemComment(i) = ListItemComment(i - 1)
							ListItemFlag(i) = ListItemFlag(i - 1)
						Case Else
							With IList.Item(part_item(i))
								list(i) = RightPaddedString(.Nickname, 22) & " " & part_list(i)
								ListItemComment(i) = .Data.Comment
								id_list(i) = .ID
								For j = i + 1 To i + .Size - 1
									If j > UBound(part_item) Then
										Exit For
									End If
									id_list(j) = .ID
								Next 
								If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "固定" Or .IsFeatureAvailable("呪い") Then
									ListItemFlag(i) = True
									For j = i + 1 To i + .Size - 1
										If j > UBound(part_item) Then
											Exit For
										End If
										ListItemFlag(j) = True
									Next 
								End If
							End With
					End Select
				Next 
				list(UBound(list)) = "▽装備解除▽"
				
				'交換するアイテムを選択
				caption_str = "装備個所を選択 ： " & .Nickname
				If .CountPilot > 0 And Not IsOptionDefined("等身大基準") Then
					caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
				End If
				caption_str = caption_str & "  " & Term("ＨＰ", u) & "=" & VB6.Format(.MaxHP) & " " & Term("ＥＮ", u) & "=" & VB6.Format(.MaxEN) & " " & Term("装甲", u) & "=" & VB6.Format(.Armor) & " " & Term("運動性", u) & "=" & VB6.Format(.Mobility) & " " & Term("移動力", u) & "=" & VB6.Format(.Speed)
				TopItem = top_item2
				ret = ListBox(caption_str, list, "アイテム               分類", "連続表示,コメント")
				top_item2 = TopItem
				If ret = 0 Then
					Exit Do
				End If
				
				'装備を解除する場合
				If ret = UBound(list) Then
					list(UBound(list)) = "▽全て外す▽"
					caption_str = "外すアイテムを選択 ： " & .Nickname
					If .CountPilot > 0 And Not IsOptionDefined("等身大基準") Then
						caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
					End If
					caption_str = caption_str & "  " & Term("ＨＰ", u) & "=" & VB6.Format(.MaxHP) & " " & Term("ＥＮ", u) & "=" & VB6.Format(.MaxEN) & " " & Term("装甲", u) & "=" & VB6.Format(.Armor) & " " & Term("運動性", u) & "=" & VB6.Format(.Mobility) & " " & Term("移動力", u) & "=" & VB6.Format(.Speed)
					ret = ListBox(caption_str, list, "アイテム               分類", "連続表示,コメント")
					If ret <> 0 Then
						If ret < UBound(list) Then
							'指定されたアイテムを外す
							If id_list(ret) <> "" Then
								.DeleteItem(id_list(ret), False)
							ElseIf LIndex(list(ret), 1) = ":" Then 
								.DeleteItem(id_list(ret - 1), False)
							End If
						Else
							'全てのアイテムを外す
							For i = 1 To UBound(list) - 1
								If Not ListItemFlag(i) And id_list(i) <> "" Then
									.DeleteItem(id_list(i), False)
								End If
							Next 
						End If
						If MapFileName = "" Then
							.FullRecover()
						End If
						If MainForm.Visible Then
							DisplayUnitStatus(u)
						End If
					End If
					GoTo NextLoop2
				End If
				
				'交換するアイテムの装備個所
				iid = id_list(ret)
				If iid <> "" Then
					ipart = IList.Item(iid).Part
				Else
					ipart = LIndex(list(ret), 2)
				End If
				
				'空きスロットを調べておく
				Select Case ipart
					Case "右手", "左手", "片手", "両手", "盾"
						is_right_hand_available = True
						is_left_hand_available = True
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = "片手" Then
									If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "固定" Or .IsFeatureAvailable("呪い") Then
										If is_right_hand_available Then
											is_right_hand_available = False
										Else
											is_left_hand_available = False
										End If
									End If
								ElseIf .Part = "盾" Then 
									If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "固定" Or .IsFeatureAvailable("呪い") Then
										is_left_hand_available = False
									End If
								End If
							End With
						Next 
					Case "右肩", "左肩", "肩"
						empty_slot = 2
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = "肩" Then
									If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "固定" Or .IsFeatureAvailable("呪い") Then
										empty_slot = empty_slot - 1
									End If
								End If
							End With
						Next 
					Case "強化パーツ", "アイテム"
						empty_slot = .MaxItemNum
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = "強化パーツ" Or .Part = "アイテム" Then
									If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "固定" Or .IsFeatureAvailable("呪い") Then
										empty_slot = empty_slot - .Size
									End If
								End If
							End With
						Next 
					Case Else
						empty_slot = 0
						For i = 1 To .CountFeature
							If .Feature(i) = "ハードポイント" And .FeatureData(i) = ipart Then
								empty_slot = empty_slot + .FeatureLevel(i)
							End If
						Next 
						If empty_slot = 0 Then
							empty_slot = 1
						End If
						For i = 1 To .CountItem
							With .Item(i)
								If .Part = ipart Then
									If IsGlobalVariableDefined("Fix(" & .Name & ")") Or .Class_Renamed() = "固定" Or .IsFeatureAvailable("呪い") Then
										empty_slot = empty_slot - .Size
									End If
								End If
							End With
						Next 
				End Select
				
				Do While True
					'装備可能なアイテムを調べる
					ReDim item_list(0)
					For	Each it In IList
						With it
							If Not .Exist Then
								GoTo NextItem
							End If
							
							'装備スロットが空いている？
							Select Case ipart
								Case "右手", "左手", "片手", "両手"
									Select Case .Part
										Case "両手"
											If Not is_right_hand_available Or Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case "片手"
											If u.IsFeatureAvailable("両手持ち") Then
												If Not is_right_hand_available And Not is_left_hand_available Then
													GoTo NextItem
												End If
											Else
												If Not is_right_hand_available Then
													GoTo NextItem
												End If
											End If
										Case "盾"
											If Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case Else
											GoTo NextItem
									End Select
								Case "盾"
									Select Case .Part
										Case "両手"
											If Not is_right_hand_available Or Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case "片手"
											If u.IsFeatureAvailable("両手持ち") Then
												If Not is_right_hand_available And Not is_left_hand_available Then
													GoTo NextItem
												End If
											Else
												GoTo NextItem
											End If
										Case "盾"
											If Not is_left_hand_available Then
												GoTo NextItem
											End If
										Case Else
											GoTo NextItem
									End Select
								Case "右肩", "左肩", "肩"
									If .Part <> "両肩" And .Part <> "肩" Then
										GoTo NextItem
									End If
									If .Part = "両肩" Then
										If empty_slot < 2 Then
											GoTo NextItem
										End If
									End If
								Case "強化パーツ", "アイテム"
									If .Part <> "強化パーツ" And .Part <> "アイテム" Then
										GoTo NextItem
									End If
									If empty_slot < .Size Then
										GoTo NextItem
									End If
								Case Else
									If .Part <> ipart Then
										GoTo NextItem
									End If
									If empty_slot < .Size Then
										GoTo NextItem
									End If
							End Select
							
							If Not .Unit_Renamed Is Nothing Then
								With .Unit_Renamed.CurrentForm
									'離脱したユニットが装備している
									If .Status_Renamed = "離脱" Then
										GoTo NextItem
									End If
									
									'敵ユニットが装備している
									If .Party <> "味方" Then
										GoTo NextItem
									End If
								End With
								
								'呪われているので外せない……
								If .IsFeatureAvailable("呪い") Then
									GoTo NextItem
								End If
							End If
							
							'既に登録済み？
							For i = 1 To UBound(item_list)
								If item_list(i) = .Name Then
									GoTo NextItem
								End If
							Next 
						End With
						
						'装備可能？
						If Not .IsAbleToEquip(it) Then
							GoTo NextItem
						End If
						
						ReDim Preserve item_list(UBound(item_list) + 1)
						item_list(UBound(item_list)) = it.Name
NextItem: 
					Next it
					
					'装備可能なアイテムの一覧を表示
					ReDim list(UBound(item_list))
					ReDim strkey_list(UBound(item_list))
					ReDim id_list(UBound(item_list))
					ReDim ListItemFlag(UBound(item_list))
					ReDim ListItemComment(UBound(item_list))
					For i = 1 To UBound(item_list)
						iname = item_list(i)
						With IDList.Item(iname)
							list(i) = RightPaddedString(.Nickname, 22) & " "
							
							If .IsFeatureAvailable("大型アイテム") Then
								list(i) = list(i) & RightPaddedString(.Part & "[" & VB6.Format(.Size) & "]", 15)
							Else
								list(i) = list(i) & RightPaddedString(.Part, 15)
							End If
							
							'アイテムの数をカウント
							inum = 0
							inum2 = 0
							For	Each it In IList
								With it
									If .Name = iname Then
										If .Exist Then
											If .Unit_Renamed Is Nothing Then
												inum = inum + 1
												inum2 = inum2 + 1
											Else
												If .Unit_Renamed.CurrentForm.Status_Renamed <> "離脱" Then
													If Not .IsFeatureAvailable("呪い") Then
														inum = inum + 1
													End If
												End If
											End If
										End If
									End If
								End With
							Next it
							
							list(i) = list(i) & LeftPaddedString(VB6.Format(inum2), 2) & "/" & LeftPaddedString(VB6.Format(inum), 2)
							
							id_list(i) = .Name
							strkey_list(i) = .KanaName
							ListItemComment(i) = .Comment
						End With
					Next 
					
					'アイテムを名前順にソート
					For i = 1 To UBound(strkey_list) - 1
						max_item = i
						max_str = strkey_list(i)
						For j = i + 1 To UBound(strkey_list)
							If StrComp(strkey_list(j), max_str, 1) = -1 Then
								max_item = j
								max_str = strkey_list(j)
							End If
						Next 
						If max_item <> i Then
							buf = list(i)
							list(i) = list(max_item)
							list(max_item) = buf
							
							buf = id_list(i)
							id_list(i) = id_list(max_item)
							id_list(max_item) = buf
							
							buf = ListItemComment(i)
							ListItemComment(i) = ListItemComment(max_item)
							ListItemComment(max_item) = buf
							
							strkey_list(max_item) = strkey_list(i)
						End If
					Next 
					
					'装備するアイテムの種類を選択
					caption_str = "装備するアイテムを選択 ： " & .Nickname
					If .CountPilot > 0 And Not IsOptionDefined("等身大基準") Then
						caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
					End If
					caption_str = caption_str & "  " & Term("ＨＰ", u) & "=" & VB6.Format(.MaxHP) & " " & Term("ＥＮ", u) & "=" & VB6.Format(.MaxEN) & " " & Term("装甲", u) & "=" & VB6.Format(.Armor) & " " & Term("運動性", u) & "=" & VB6.Format(.Mobility) & " " & Term("移動力", u) & "=" & VB6.Format(.Speed)
					ret = ListBox(caption_str, list, "アイテム               分類            数量", "連続表示,コメント")
					
					'キャンセルされた？
					If ret = 0 Then
						Exit Do
					End If
					
					iname = id_list(ret)
					
					'未装備のアイテムがあるかどうか探す
					For	Each it In IList
						With it
							If .Name = iname And .Exist Then
								If .Unit_Renamed Is Nothing Then
									'未装備の装備が見つかったのでそれを装備
									If iid <> "" Then
										u.DeleteItem(iid)
									End If
									'呪いのアイテムを装備……
									If .IsFeatureAvailable("呪い") Then
										MsgBox(.Nickname & "は呪われていた！")
									End If
									u.AddItem(it)
									If MapFileName = "" Then
										u.FullRecover()
									End If
									If MainForm.Visible Then
										DisplayUnitStatus(u)
									End If
									Exit Do
								End If
							End If
						End With
					Next it
					
					'選択されたアイテムを列挙
					ReDim list(0)
					ReDim id_list(0)
					ReDim ListItemComment(0)
					inum = 0
					If Not IDList.Item(iname).IsFeatureAvailable("呪い") Then
						For	Each it In IList
							With it
								If .Name <> iname Or Not .Exist Then
									GoTo NextItem2
								End If
								If .Unit_Renamed Is Nothing Then
									GoTo NextItem2
								End If
								With .Unit_Renamed.CurrentForm
									If .Status_Renamed = "離脱" Then
										GoTo NextItem2
									End If
									If .Party <> "味方" Then
										GoTo NextItem2
									End If
									
									ReDim Preserve list(UBound(list) + 1)
									ReDim Preserve id_list(UBound(list))
									ReDim Preserve ListItemComment(UBound(list))
									
									If Not IsOptionDefined("等身大基準") And .CountPilot > 0 Then
										list(UBound(list)) = RightPaddedString(.Nickname0, 36) & " " & .MainPilot.Nickname
									Else
										list(UBound(list)) = .Nickname0
									End If
									id_list(UBound(list)) = it.ID
									
									For i = 1 To .CountItem
										With .Item(i)
											If (.Class_Renamed() <> "固定" Or Not .IsFeatureAvailable("非表示")) And .Part <> "非表示" Then
												ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
											End If
										End With
									Next 
									
									inum = inum + 1
								End With
							End With
NextItem2: 
						Next it
					End If
					ReDim ListItemFlag(UBound(list))
					ReDim Preserve ListItemComment(UBound(list))
					
					'どのアイテムを装備するか選択
					caption_str = IList.Item(id_list(1)).Nickname & "の入手先を選択 ： " & .Nickname
					If .CountPilot > 0 And Not IsOptionDefined("等身大基準") Then
						caption_str = caption_str & " (" & .MainPilot.Nickname & ")"
					End If
					caption_str = caption_str & "  " & Term("ＨＰ", u) & "=" & VB6.Format(.MaxHP) & " " & Term("ＥＮ", u) & "=" & VB6.Format(.MaxEN) & " " & Term("装甲", u) & "=" & VB6.Format(.Armor) & " " & Term("運動性", u) & "=" & VB6.Format(.Mobility) & " " & Term("移動力", u) & "=" & VB6.Format(.Speed)
					TopItem = 1
					If IsOptionDefined("等身大基準") Then
						ret = ListBox(caption_str, list, "ユニット", "連続表示,コメント")
					Else
						ret = ListBox(caption_str, list, "ユニット                             パイロット", "連続表示,コメント")
					End If
					
					'アイテムを交換
					If ret > 0 Then
						If iid <> "" Then
							.DeleteItem(iid)
						End If
						With IList.Item(id_list(ret))
							If Not .Unit_Renamed Is Nothing Then
								.Unit_Renamed.DeleteItem(.ID)
							End If
							
							'呪いのアイテムを装備……
							If .IsFeatureAvailable("呪い") Then
								MsgBox(.Nickname & "は呪われていた！")
							End If
						End With
						.AddItem(IList.Item(id_list(ret)))
						If MapFileName = "" Then
							.FullRecover()
						End If
						If MainForm.Visible Then
							DisplayUnitStatus(u)
						End If
						Exit Do
					End If
NextLoop: 
				Loop 
NextLoop2: 
			Loop 
		End With
		
		'ユニットがあらかじめ選択されている場合
		'(ユニットステータスからのアイテム交換時)
		If Not selected_unit Is Nothing Then
			With frmListBox
				.Hide()
				If .txtComment.Enabled Then
					.txtComment.Enabled = False
					.txtComment.Visible = False
					.Height = VB6.TwipsToPixelsY(VB6.PixelsToTwipsY(.Height) - 600)
				End If
			End With
			ReduceListBoxHeight()
			EnlargeListBoxWidth()
			Exit Sub
		End If
		
		GoTo Beginning
	End Sub
	
	'換装コマンド
	' MOD START MARGE
	'Public Sub ExchangeFormCommand()
	Private Sub ExchangeFormCommand()
		' MOD END MARGE
		Dim j, i, k As Short
		Dim list() As String
		Dim id_list() As String
		Dim key_list() As Integer
		Dim list2() As String
		Dim id_list2() As String
		Dim max_item, max_value As Short
		Dim u As Unit
		Dim buf As String
		Dim ret As Short
		Dim top_item As Short
		Dim farray() As String
		
Beginning: 
		
		top_item = 1
		
		'換装可能なユニットのリストを作成
		ReDim list(0)
		ReDim id_list(0)
		ReDim ListItemComment(0)
		For	Each u In UList
			With u
				'待機中の味方ユニット？
				If .Party0 <> "味方" Or .Status_Renamed <> "待機" Then
					GoTo NextLoop
				End If
				
				'換装能力を持っている？
				If Not .IsFeatureAvailable("換装") Then
					GoTo NextLoop
				End If
				
				'いずれかの形態に換装可能？
				For i = 1 To LLength(.FeatureData("換装"))
					If .OtherForm(LIndex(.FeatureData("換装"), i)).IsAvailable Then
						Exit For
					End If
				Next 
				If i > LLength(.FeatureData("換装")) Then
					GoTo NextLoop
				End If
				
				ReDim Preserve list(UBound(list) + 1)
				ReDim Preserve id_list(UBound(list))
				ReDim Preserve ListItemComment(UBound(list))
				
				'ユニットのステータスを表示
				If IsOptionDefined("等身大基準") Then
					If .Rank < 10 Then
						list(UBound(list)) = RightPaddedString(.Nickname0, 37) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = RightPaddedString(.Nickname0, 37) & VB6.Format(.Rank)
					End If
				Else
					If .Rank < 10 Then
						list(UBound(list)) = RightPaddedString(.Nickname0, 33) & StrConv(VB6.Format(.Rank), VbStrConv.Wide)
					Else
						list(UBound(list)) = RightPaddedString(.Nickname0, 33) & VB6.Format(.Rank)
					End If
				End If
				list(UBound(list)) = list(UBound(list)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 5) & LeftPaddedString(VB6.Format(.Armor), 5) & LeftPaddedString(VB6.Format(.Mobility), 5)
				If .CountPilot > 0 Then
					If IsOptionDefined("等身大基準") Then
						list(UBound(list)) = list(UBound(list)) & "  " & LeftPaddedString(VB6.Format(.MainPilot.Level), 6)
					Else
						list(UBound(list)) = list(UBound(list)) & "  " & .MainPilot.Nickname
					End If
				End If
				
				'ユニットに装備されているアイテムをコメント欄に列記
				For k = 1 To .CountItem
					With .Item(k)
						If (.Class_Renamed() <> "固定" Or Not .IsFeatureAvailable("非表示")) And .Part <> "非表示" Then
							ListItemComment(UBound(list)) = ListItemComment(UBound(list)) & .Nickname & " "
						End If
					End With
				Next 
				
				'ユニットＩＤを記録しておく
				id_list(UBound(list)) = .ID
			End With
NextLoop: 
		Next u
		ReDim ListItemFlag(UBound(list))
		
		'リストをユニットのＨＰでソート
		ReDim key_list(UBound(list))
		With UList
			For i = 1 To UBound(list)
				key_list(i) = .Item(id_list(i)).MaxHP
			Next 
		End With
		For i = 1 To UBound(list) - 1
			max_item = i
			max_value = key_list(i)
			For j = i + 1 To UBound(list)
				If key_list(j) > max_value Then
					max_item = j
					max_value = key_list(j)
				End If
			Next 
			If max_item <> i Then
				buf = list(i)
				list(i) = list(max_item)
				list(max_item) = buf
				
				buf = id_list(i)
				id_list(i) = id_list(max_item)
				id_list(max_item) = buf
				
				buf = ListItemComment(i)
				ListItemComment(i) = ListItemComment(max_item)
				ListItemComment(max_item) = buf
				
				key_list(max_item) = key_list(i)
			End If
		Next 
		
		'換装するユニットを選択
		TopItem = top_item
		If IsOptionDefined("等身大基準") Then
			ret = ListBox("ユニット選択", list, "ユニット                         " & Term("ランク", Nothing, 6) & "   " & Term("ＨＰ", Nothing, 4) & " " & Term("ＥＮ", Nothing, 4) & " " & Term("装甲", Nothing, 4) & " " & Term("運動", Nothing, 4) & " レベル", "連続表示,コメント")
		Else
			ret = ListBox("ユニット選択", list, "ユニット                     " & Term("ランク", Nothing, 6) & "   " & Term("ＨＰ", Nothing, 4) & " " & Term("ＥＮ", Nothing, 4) & " " & Term("装甲", Nothing, 4) & " " & Term("運動", Nothing, 4) & " パイロット", "連続表示,コメント")
		End If
		top_item = TopItem
		
		'キャンセル？
		If ret = 0 Then
			Exit Sub
		End If
		
		'選択されたユニットを検索
		u = UList.Item(id_list(ret))
		
		'換装可能な形態のリストを作成
		With u
			buf = .FeatureData("換装")
			ReDim list2(0)
			ReDim id_list2(0)
			ReDim ListItemComment(0)
			For i = 1 To LLength(buf)
				With .OtherForm(LIndex(buf, i))
					If .IsAvailable Then
						ReDim Preserve list2(UBound(list2) + 1)
						ReDim Preserve id_list2(UBound(list2))
						ReDim Preserve ListItemComment(UBound(list2))
						
						'ユニットランクを合わせる
						.Rank = u.Rank
						.BossRank = u.BossRank
						.Update()
						
						'換装先のリストを作成
						id_list2(UBound(list2)) = .Name
						If u.Nickname0 = .Nickname Then
							list2(UBound(list2)) = RightPaddedString(.Name, 27)
						Else
							list2(UBound(list2)) = RightPaddedString(.Nickname0, 27)
						End If
						list2(UBound(list2)) = list2(UBound(list2)) & LeftPaddedString(VB6.Format(.MaxHP), 6) & LeftPaddedString(VB6.Format(.MaxEN), 5) & LeftPaddedString(VB6.Format(.Armor), 5) & LeftPaddedString(VB6.Format(.Mobility), 5) & " " & .Data.Adaption
						
						'最大攻撃力
						max_value = 0
						For j = 1 To .CountWeapon
							If .IsWeaponMastered(j) And Not .IsDisabled((.Weapon(j).Name)) And Not .IsWeaponClassifiedAs(j, "合") Then
								If .WeaponPower(j, "") > max_value Then
									max_value = .WeaponPower(j, "")
								End If
							End If
						Next 
						list2(UBound(list2)) = list2(UBound(list2)) & LeftPaddedString(VB6.Format(max_value), 7)
						
						'最大射程
						max_value = 0
						For j = 1 To .CountWeapon
							If .IsWeaponMastered(j) And Not .IsDisabled((.Weapon(j).Name)) And Not .IsWeaponClassifiedAs(j, "合") Then
								If .WeaponMaxRange(j) > max_value Then
									max_value = .WeaponMaxRange(j)
								End If
							End If
						Next 
						list2(UBound(list2)) = list2(UBound(list2)) & LeftPaddedString(VB6.Format(max_value), 5)
						
						'換装先が持つ特殊能力一覧
						ReDim farray(0)
						For j = 1 To .CountFeature
							If .FeatureName(j) <> "" Then
								'重複する特殊能力は表示しないようチェック
								For k = 1 To UBound(farray)
									If .FeatureName(j) = farray(k) Then
										Exit For
									End If
								Next 
								If k > UBound(farray) Then
									ListItemComment(UBound(list2)) = ListItemComment(UBound(list2)) & .FeatureName(j) & " "
									ReDim Preserve farray(UBound(farray) + 1)
									farray(UBound(farray)) = .FeatureName(j)
								End If
							End If
						Next 
					End If
				End With
			Next 
			ReDim ListItemFlag(UBound(list2))
			
			'換装先の形態を選択
			TopItem = 1
			ret = ListBox("変更先選択", list2, "ユニット                     " & Term("ＨＰ", u, 4) & " " & Term("ＥＮ", u, 4) & " " & Term("装甲", u, 4) & " " & Term("運動", u, 4) & " 適応 攻撃力 射程", "連続表示,コメント")
			
			'キャンセル？
			If ret = 0 Then
				GoTo Beginning
			End If
			
			'換装を実施
			.Transform(id_list2(ret))
		End With
		
		GoTo Beginning
	End Sub
	
	'ステータスコマンド中かどうかを返す
	Public Function InStatusCommand() As Boolean
		If MapFileName = "" Then
			If InStr(ScenarioFileName, "\ユニットステータス表示.eve") > 0 Or InStr(ScenarioFileName, "\パイロットステータス表示.eve") > 0 Or IsSubStage Then
				InStatusCommand = True
			End If
		End If
	End Function
End Module