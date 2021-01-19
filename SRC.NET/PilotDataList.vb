Option Strict Off
Option Explicit On
Friend Class PilotDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'全パイロットデータを管理するリストのクラス
	
	'パイロットデータのコレクション
	Private colPilotDataList As New Collection
	
	'クラスの初期化
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		Dim pd As New PilotData
		
		'ユニットステータスコマンドの無人ユニット用
		With pd
			.Name = "ステータス表示用ダミーパイロット(ザコ)"
			.Nickname = "パイロット不在"
			.Adaption = "AAAA"
			.Bitmap = ".bmp"
		End With
		colPilotDataList.Add(pd, pd.Name)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colPilotDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colPilotDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colPilotDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'パイロットデータリストにデータを追加
	Public Function Add(ByRef pname As String) As PilotData
		Dim new_pilot_data As New PilotData
		
		new_pilot_data.Name = pname
		colPilotDataList.Add(new_pilot_data, pname)
		Add = new_pilot_data
	End Function
	
	'パイロットデータリストに登録されているデータの総数
	Public Function Count() As Short
		Count = colPilotDataList.Count()
	End Function
	
	'パイロットデータリストから指定したデータを消去
	Public Sub Delete(ByRef Index As Object)
		colPilotDataList.Remove(Index)
	End Sub
	
	'パイロットデータリストから指定したデータを取り出す
	Public Function Item(ByRef Index As Object) As PilotData
		Dim pd As PilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		Item = colPilotDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		pname = CStr(Index)
		For	Each pd In colPilotDataList
			If pd.Nickname = pname Then
				Item = pd
				Exit Function
			End If
		Next pd
	End Function
	
	'パイロットデータリストに指定したデータが登録されているか？
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim pd As PilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		pd = colPilotDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		pname = CStr(Index)
		For	Each pd In colPilotDataList
			If pd.Nickname = pname Then
				IsDefined = True
				Exit Function
			End If
		Next pd
		IsDefined = False
	End Function
	
	'パイロットデータリストに指定したデータが登録されているか？ (愛称は見ない)
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim pd As PilotData
		
		On Error GoTo ErrorHandler
		pd = colPilotDataList.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'データファイル fname からデータをロード
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim line_num As Integer
		Dim i, j As Short
		Dim ret, n, ret2 As Integer
		Dim buf, line_buf, buf2 As String
		Dim pd As PilotData
		Dim data_name As String
		Dim err_msg As String
		Dim aname, adata As String
		Dim alevel As Double
		Dim wd As WeaponData
		Dim sd As AbilityData
		Dim wname, sname As String
		Dim sp_cost As Short
		Dim in_quote As Boolean
		Dim comma_num As Short
		
		On Error GoTo ErrorHandler
		
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read)
		
		line_num = 0
		
		Do While True
			data_name = ""
			
			Do 
				If EOF(FileNumber) Then
					FileClose(FileNumber)
					Exit Sub
				End If
				GetLine(FileNumber, line_buf, line_num)
			Loop While Len(line_buf) = 0
			
			If InStr(line_buf, ",") > 0 Then
				err_msg = "名称の設定が抜けています。"
				Error(0)
			End If
			
			'名称
			data_name = line_buf
			
			If InStr(data_name, " ") > 0 Then
				err_msg = "名称に半角スペースは使用出来ません。"
				Error(0)
			End If
			If InStr(data_name, "（") > 0 Or InStr(data_name, "）") > 0 Then
				err_msg = "名称に全角括弧は使用出来ません"
				Error(0)
			End If
			If InStr(data_name, """") > 0 Then
				err_msg = "名称に""は使用出来ません。"
				Error(0)
			End If
			
			If IsDefined(data_name) Then
				'すでに定義済みのパイロットの場合はデータを置き換え
				If Item(data_name).Name = data_name Then
					pd = Item(data_name)
					pd.Clear()
				Else
					pd = Add(data_name)
				End If
			Else
				pd = Add(data_name)
			End If
			
			With pd
				'愛称, 読み仮名, 性別, クラス, 地形適応, 経験値
				GetLine(FileNumber, line_buf, line_num)
				
				'書式チェックのため、コンマの数を数えておく
				comma_num = 0
				For i = 1 To Len(line_buf)
					If Mid(line_buf, i, 1) = "," Then
						comma_num = comma_num + 1
					End If
				Next 
				
				If comma_num < 3 Then
					err_msg = "設定に抜けがあります。"
					Error(0)
				ElseIf comma_num > 5 Then 
					err_msg = "余分な「,」があります。"
					Error(0)
				End If
				
				'愛称
				ret = InStr(line_buf, ",")
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If Len(buf2) = 0 Then
					err_msg = "愛称の設定が抜けています。"
					Error(0)
				End If
				.Nickname = buf2
				
				Select Case comma_num
					Case 4
						'読み仮名 or 性別
						ret = InStr(buf, ",")
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						Select Case buf2
							Case "男性", "女性", "-"
								.KanaName = StrToHiragana(.Nickname)
								.Sex = buf2
							Case Else
								.KanaName = buf2
						End Select
					Case 5
						'読み仮名
						ret = InStr(buf, ",")
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						Select Case buf2
							Case "男性", "女性", "-"
								DataErrorMessage("読み仮名の設定が抜けています。", fname, line_num, line_buf, data_name)
								.KanaName = StrToHiragana(.Nickname)
							Case Else
								.KanaName = buf2
						End Select
						
						'性別
						ret = InStr(buf, ",")
						buf2 = Trim(Left(buf, ret - 1))
						buf = Mid(buf, ret + 1)
						Select Case buf2
							Case "男性", "女性", "-"
								.Sex = buf2
							Case Else
								DataErrorMessage("性別の設定が間違っています。", fname, line_num, line_buf, data_name)
						End Select
					Case Else
						.KanaName = StrToHiragana(.Nickname)
				End Select
				
				'クラス
				ret = InStr(buf, ",")
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If Not IsNumeric(buf2) Then
					.Class_Renamed = buf2
				Else
					DataErrorMessage("クラスの設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'地形適応
				ret = InStr(buf, ",")
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If Len(buf2) = 4 Then
					.Adaption = buf2
				Else
					DataErrorMessage("地形適応の設定が間違っています。", fname, line_num, line_buf, data_name)
					.Adaption = "AAAA"
				End If
				
				'経験値
				buf2 = Trim(buf)
				If IsNumeric(buf2) Then
					.ExpValue = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("経験値の設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'特殊能力データ
				GetLine(FileNumber, line_buf, line_num)
				If line_buf = "特殊能力なし" Then
					GetLine(FileNumber, line_buf, line_num)
				ElseIf line_buf = "特殊能力" Then 
					'新形式による特殊能力表記
					GetLine(FileNumber, line_buf, line_num)
					buf = line_buf
					
					i = 0
					aname = ""
					Do While True
						i = i + 1
						
						'コンマの位置を検索
						ret = InStr(buf, ",")
						'「"」が使われているか検索
						ret2 = InStr(buf, """")
						
						If ret2 < ret And ret2 > 0 Then
							'「"」が見つかった場合、次の「"」後のコンマを検索
							in_quote = True
							j = ret2 + 1
							Do While j <= Len(buf)
								Select Case Mid(buf, j, 1)
									Case """"
										in_quote = Not in_quote
									Case ","
										If Not in_quote Then
											buf2 = Left(buf, j - 1)
											buf = Mid(buf, j + 1)
										End If
								End Select
								j = j + 1
							Loop 
							If j = Len(buf) Then
								buf2 = buf
								buf = ""
							End If
							in_quote = False
						ElseIf ret > 0 Then 
							'コンマが見つかったらコンマまでの文字列を切り出す
							buf2 = Trim(Left(buf, ret - 1))
							buf = Trim(Mid(buf, ret + 1))
							
							'コンマの後ろの文字列が空白の場合
							If buf = "" Then
								If i Mod 2 = 1 Then
									err_msg = "行末の「,」の後に特殊能力指定が抜けています。"
								Else
									err_msg = "行末の「,」の後に特殊能力レベル指定が抜けています。"
								End If
								Error(0)
							End If
						Else
							'行末の文字列
							buf2 = buf
							buf = ""
						End If
						
						If i Mod 2 = 1 Then
							'特殊能力名＆レベル
							
							If IsNumeric(buf2) Then
								If i = 1 Then
									'特殊能力の指定は終り。能力値の指定へ
									buf = buf2 & ", " & buf
									Exit Do
								Else
									DataErrorMessage("行頭から" & VB6.Format((i + 1) \ 2) & "番目の特殊能力名の設定が間違っています。", fname, line_num, line_buf, data_name)
								End If
							End If
							
							If InStr(buf2, " ") > 0 Then
								If Left(buf2, 4) <> "先手必勝" And Left(buf2, 6) <> "ＳＰ消費減少" And Left(buf2, 12) <> "スペシャルパワー自動発動" And Left(buf2, 4) <> "ハンター" And InStr(buf2, "=解説 ") = 0 Then
									If aname = "" Then
										err_msg = "行頭から" & VB6.Format((i + 1) \ 2) & "番目の特殊能力「" & Trim(Left(buf2, InStr(buf2, " "))) & "」の指定の後に「,」が抜けています。"
									Else
										err_msg = "特殊能力「" & aname & "」のレベル指定の後に「,」が抜けています。"
									End If
									Error(0)
								End If
							End If
							
							'特殊能力の別名指定がある？
							j = InStr(buf2, "=")
							If j > 0 Then
								adata = Mid(buf2, j + 1)
								buf2 = Left(buf2, j - 1)
							Else
								adata = ""
							End If
							
							'特殊能力のレベル指定を切り出す
							j = InStr(buf2, "Lv")
							Select Case j
								Case 0
									'指定なし
									aname = buf2
									alevel = DEFAULT_LEVEL
								Case 1
									'レベル指定のみあり
									If Not IsNumeric(Mid(buf2, j + 2)) Then
										DataErrorMessage("特殊能力「" & aname & "」のレベル指定が不正です。", fname, line_num, line_buf, data_name)
									End If
									
									alevel = CShort(Mid(buf2, j + 2))
									If aname = "" Then
										DataErrorMessage("行頭から" & VB6.Format((i + 1) \ 2) & "番目の特殊能力名の設定が間違っています。", fname, line_num, line_buf, data_name)
									End If
								Case Else
									'特殊能力名とレベルの両方が指定されている
									aname = Left(buf2, j - 1)
									alevel = CDbl(Mid(buf2, j + 2))
							End Select
						Else
							'特殊能力修得レベル
							If IsNumeric(buf2) Then
								.AddSkill(aname, alevel, adata, CShort(buf2))
							Else
								If alevel > 0 Then
									DataErrorMessage("特殊能力「" & aname & "Lv" & VB6.Format(alevel) & "」の修得レベルが間違っています。", fname, line_num, line_buf, data_name)
								Else
									DataErrorMessage("特殊能力「" & aname & "」の修得レベルが間違っています。", fname, line_num, line_buf, data_name)
								End If
								.AddSkill(aname, alevel, adata, 1)
							End If
						End If
						
						If buf = "" Then
							'ここでこの行は終り
							
							'iが奇数の場合は修得レベルが抜けている
							If i Mod 2 = 1 Then
								If alevel > 0 Then
									DataErrorMessage("特殊能力「" & aname & "Lv" & VB6.Format(alevel) & "」の修得レベルが間違っています。", fname, line_num, line_buf, data_name)
								Else
									DataErrorMessage("特殊能力「" & aname & "」の修得レベルが間違っています。", fname, line_num, line_buf, data_name)
								End If
							End If
							
							GetLine(FileNumber, line_buf, line_num)
							buf = line_buf
							
							i = 0
							aname = ""
						End If
					Loop 
				ElseIf InStr(line_buf, "特殊能力,") = 1 Then 
					'旧形式による特殊能力表記
					buf = Mid(line_buf, 6)
					
					i = 0
					aname = ""
					Do 
						i = i + 1
						
						'コンマの位置を検索
						ret = InStr(buf, ",")
						'「"」が使われているか検索
						ret2 = InStr(buf, """")
						
						If ret2 < ret And ret2 > 0 Then
							'「"」が見つかった場合、次の「"」後のコンマを検索
							in_quote = True
							j = ret2 + 1
							Do While j <= Len(buf)
								Select Case Mid(buf, j, 1)
									Case """"
										in_quote = Not in_quote
									Case ","
										If Not in_quote Then
											buf2 = Left(buf, j - 1)
											buf = Mid(buf, j + 1)
										End If
								End Select
								j = j + 1
							Loop 
							If j = Len(buf) Then
								buf2 = buf
								buf = ""
							End If
							in_quote = False
						ElseIf ret > 0 Then 
							'コンマが見つかったらコンマまでの文字列を切り出す
							buf2 = Trim(Left(buf, ret - 1))
							buf = Mid(buf, ret + 1)
							
							'コンマの後ろの文字列が空白の場合
							If buf = "" Then
								If i Mod 2 = 1 Then
									err_msg = "行末の「,」の後に特殊能力指定が抜けています。"
								Else
									err_msg = "行末の「,」の後に特殊能力レベル指定が抜けています。"
								End If
								Error(0)
							End If
						Else
							'行末の文字列
							buf2 = buf
							buf = ""
						End If
						
						If i Mod 2 = 1 Then
							'特殊能力名＆レベル
							
							If InStr(buf2, " ") > 0 Then
								If aname = "" Then
									err_msg = "行頭から" & VB6.Format((i + 1) \ 2) & "番目の特殊能力の指定の後に「,」が抜けています。"
								Else
									err_msg = "特殊能力「" & aname & "」のレベル指定の後に「,」が抜けています。"
								End If
								Error(0)
							End If
							
							'特殊能力の別名指定がある？
							j = InStr(buf2, "=")
							If j > 0 Then
								adata = Mid(buf2, j + 1)
								buf2 = Left(buf2, j - 1)
							Else
								adata = ""
							End If
							
							'特殊能力のレベル指定を切り出す
							j = InStr(buf2, "Lv")
							Select Case j
								Case 0
									'指定なし
									aname = buf2
									alevel = DEFAULT_LEVEL
								Case 1
									'レベル指定のみあり
									If Not IsNumeric(Mid(buf2, j + 2)) Then
										err_msg = "特殊能力「" & aname & "」のレベル指定が不正です"
										Error(0)
									End If
									
									alevel = CDbl(Mid(buf2, j + 2))
									If aname = "" Then
										err_msg = "行頭から" & VB6.Format((i + 1) \ 2) & "番目の特殊能力の名前「" & buf2 & "」が不正です"
										Error(0)
									End If
								Case Else
									'特殊能力名とレベルの両方が指定されている
									aname = Left(buf2, j - 1)
									alevel = CDbl(Mid(buf2, j + 2))
							End Select
						Else
							'特殊能力修得レベル
							If IsNumeric(buf2) Then
								.AddSkill(aname, alevel, adata, CShort(buf2))
							Else
								If alevel > 0 Then
									DataErrorMessage("特殊能力「" & aname & "Lv" & VB6.Format(alevel) & "」の修得レベルが間違っています。", fname, line_num, line_buf, data_name)
								Else
									DataErrorMessage("特殊能力「" & aname & "」の修得レベルが間違っています。", fname, line_num, line_buf, data_name)
								End If
								.AddSkill(aname, alevel, adata, 1)
							End If
						End If
					Loop While ret > 0
					
					'iが奇数の場合は修得レベルが抜けている
					If i Mod 2 = 1 Then
						If alevel > 0 Then
							DataErrorMessage("特殊能力「" & aname & "Lv" & VB6.Format(alevel) & "」の修得レベルが間違っています。", fname, line_num, line_buf, data_name)
						Else
							DataErrorMessage("特殊能力「" & aname & "」の修得レベルが間違っています。", fname, line_num, line_buf, data_name)
						End If
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Else
					err_msg = "特殊能力の設定が抜けています。"
					Error(0)
				End If
				
				'格闘
				If Len(line_buf) = 0 Then
					err_msg = "格闘攻撃力の設定が抜けています。"
					Error(0)
				End If
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "射撃攻撃力の設定が抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If IsNumeric(buf2) Then
					.Infight = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("格闘攻撃力の設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'射撃
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "命中の設定が抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Shooting = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("射撃攻撃力の設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'命中
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "回避の設定が抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Hit = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("命中の設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'回避
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "技量の設定が抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Dodge = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("回避の設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'技量
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "反応の設定が抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Technique = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("技量の設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'反応
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "性格の設定が抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.Intuition = MinLng(CInt(buf2), 9999)
				Else
					DataErrorMessage("反応の設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'性格
				buf2 = Trim(buf)
				If Len(buf2) = 0 Then
					err_msg = "性格の設定が抜けています。"
					Error(0)
				End If
				If InStr(buf2, ",") > 0 Then
					DataErrorMessage("行末に余分なコンマが付けられています。", fname, line_num, line_buf, data_name)
					buf2 = Trim(Left(buf2, InStr(buf2, ",") - 1))
				End If
				If Not IsNumeric(buf2) Then
					.Personality = buf2
				Else
					DataErrorMessage("性格の設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'スペシャルパワー
				GetLine(FileNumber, line_buf, line_num)
				Select Case line_buf
					Case "ＳＰなし", "精神なし"
						'スペシャルパワーを持っていない
					Case ""
						err_msg = "スペシャルパワーの設定が抜けています。"
						Error(0)
					Case Else
						ret = InStr(line_buf, ",")
						If ret = 0 Then
							err_msg = "ＳＰ値の設定が抜けています。"
							Error(0)
						End If
						buf2 = Trim(Left(line_buf, ret - 1))
						buf = Mid(line_buf, ret + 1)
						If buf2 <> "ＳＰ" And buf2 <> "精神" Then
							err_msg = "スペシャルパワーの設定が抜けています。"
							Error(0)
						End If
						
						'ＳＰ値
						ret = InStr(buf, ",")
						If ret > 0 Then
							buf2 = Trim(Left(buf, ret - 1))
							buf = Mid(buf, ret + 1)
						Else
							buf2 = Trim(buf)
							buf = ""
						End If
						If IsNumeric(buf2) Then
							.SP = MinLng(CInt(buf2), 9999)
						Else
							DataErrorMessage("ＳＰの設定が間違っています。", fname, line_num, line_buf, data_name)
							.SP = 1
						End If
						
						'スペシャルパワーと獲得レベル
						ret = InStr(buf, ",")
						Do While ret > 0
							sname = Trim(Left(buf, ret - 1))
							buf = Mid(buf, ret + 1)
							
							'ＳＰ消費量
							If InStr(sname, "=") > 0 Then
								sp_cost = StrToLng(Mid(sname, InStr(sname, "=") + 1))
								sname = Left(sname, InStr(sname, "=") - 1)
							Else
								sp_cost = 0
							End If
							
							ret = InStr(buf, ",")
							If ret = 0 Then
								buf2 = Trim(buf)
								buf = ""
							Else
								buf2 = Trim(Left(buf, ret - 1))
								buf = Mid(buf, ret + 1)
							End If
							
							If sname = "" Then
								DataErrorMessage("スペシャルパワーの指定が抜けています。", fname, line_num, line_buf, data_name)
							ElseIf Not SPDList.IsDefined(sname) Then 
								DataErrorMessage(sname & "というスペシャルパワーは存在しません。", fname, line_num, line_buf, data_name)
							ElseIf Not IsNumeric(buf2) Then 
								DataErrorMessage("スペシャルパワー「" & sname & "」の獲得レベルが間違っています。", fname, line_num, line_buf, data_name)
								.AddSpecialPower(sname, 1, sp_cost)
							Else
								.AddSpecialPower(sname, CShort(buf2), sp_cost)
							End If
							
							ret = InStr(buf, ",")
						Loop 
						
						If buf <> "" Then
							DataErrorMessage("スペシャルパワー「" & Trim(sname) & "」の獲得レベル指定が抜けています。", fname, line_num, line_buf, data_name)
						End If
				End Select
				
				'ビットマップ, ＭＩＤＩ
				GetLine(FileNumber, line_buf, line_num)
				
				'ビットマップ
				If Len(line_buf) = 0 Then
					err_msg = "ビットマップの設定が抜けています。"
					Error(0)
				End If
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "ＭＩＤＩの設定が抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If LCase(Right(buf2, 4)) = ".bmp" Then
					.Bitmap = buf2
				Else
					DataErrorMessage("ビットマップの設定が間違っています。", fname, line_num, line_buf, data_name)
					.IsBitmapMissing = True
				End If
				
				'ＭＩＤＩ
				buf = Trim(buf)
				buf2 = buf
				Do While Right(buf2, 1) = ")"
					buf2 = Left(buf2, Len(buf2) - 1)
				Loop 
				Select Case LCase(Right(buf2, 4))
					Case ".mid", ".mp3", ".wav", "-"
						.BGM = buf
					Case ""
						DataErrorMessage("ＭＩＤＩの設定が抜けています。", fname, line_num, line_buf, data_name)
						.Bitmap = "-.mid"
					Case Else
						DataErrorMessage("ＭＩＤＩの設定が間違っています。", fname, line_num, line_buf, data_name)
						.Bitmap = "-.mid"
				End Select
				
				If EOF(FileNumber) Then
					FileClose(FileNumber)
					Exit Sub
				End If
				
				GetLine(FileNumber, line_buf, line_num)
				
				If line_buf <> "===" Then
					GoTo SkipRest
				End If
				
				'特殊能力データ
				GetLine(FileNumber, line_buf, line_num)
				
				buf = line_buf
				i = 0
				Do While line_buf <> "==="
					i = i + 1
					
					ret = 0
					in_quote = False
					For j = 1 To Len(buf)
						Select Case Mid(buf, j, 1)
							Case ","
								If Not in_quote Then
									ret = j
									Exit For
								End If
							Case """"
								in_quote = Not in_quote
						End Select
					Next 
					
					If ret > 0 Then
						buf2 = Trim(Left(buf, ret - 1))
						buf = Trim(Mid(buf, ret + 1))
					Else
						buf2 = buf
						buf = ""
					End If
					
					If buf2 = "" Or IsNumeric(buf2) Then
						DataErrorMessage("行頭から" & VB6.Format(i) & "番目の特殊能力の設定が間違っています。", fname, line_num, line_buf, data_name)
					Else
						.AddFeature(buf2)
					End If
					
					If buf = "" Then
						If EOF(FileNumber) Then
							FileClose(FileNumber)
							Exit Sub
						End If
						
						GetLine(FileNumber, line_buf, line_num)
						buf = line_buf
						i = 0
						
						If line_buf = "" Or line_buf = "===" Then
							Exit Do
						End If
					End If
				Loop 
				
				If line_buf <> "===" Then
					GoTo SkipRest
				End If
				
				'武器データ
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0 And line_buf <> "==="
					'武器名
					ret = InStr(line_buf, ",")
					If ret = 0 Then
						err_msg = "武器データの終りには空行を置いてください。"
						Error(0)
					End If
					wname = Trim(Left(line_buf, ret - 1))
					buf = Mid(line_buf, ret + 1)
					
					If wname = "" Then
						err_msg = "武器名の設定が間違っています。"
						Error(0)
					End If
					
					'武器を登録
					wd = .AddWeapon(wname)
					
					'攻撃力
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "の最小射程が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						wd.Power = MinLng(CInt(buf2), 99999)
					ElseIf buf = "-" Then 
						wd.Power = 0
					Else
						DataErrorMessage(wname & "の攻撃力の設定が間違っています。", fname, line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Power = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'最小射程
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "の最大射程の設定が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						wd.MinRange = CShort(buf2)
					Else
						DataErrorMessage(wname & "の最小射程の設定が間違っています。", fname, line_num, line_buf, data_name)
						wd.MinRange = 1
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.MinRange = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'最大射程
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "の命中率の設定が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						wd.MaxRange = MinLng(CInt(buf2), 99)
					Else
						DataErrorMessage(wname & "の最大射程の設定が間違っています。", fname, line_num, line_buf, data_name)
						wd.MaxRange = 1
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.MaxRange = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'命中率
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "の弾数の設定が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						n = CInt(buf2)
						If n > 999 Then
							n = 999
						ElseIf n < -999 Then 
							n = -999
						End If
						wd.Precision = n
					Else
						DataErrorMessage(wname & "の命中率の設定が間違っています。", fname, line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Precision = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'弾数
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "の消費ＥＮの設定が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							wd.Bullet = MinLng(CInt(buf2), 99)
						Else
							DataErrorMessage(wname & "の弾数の設定が間違っています。", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								wd.Bullet = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'消費ＥＮ
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "の必要気力が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							wd.ENConsumption = MinLng(CInt(buf2), 999)
						Else
							DataErrorMessage(wname & "の消費ＥＮの設定が間違っています。", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								wd.ENConsumption = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'必要気力
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "の地形適応が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							n = CInt(buf2)
							If n > 1000 Then
								n = 1000
							ElseIf n < 0 Then 
								n = 0
							End If
							wd.NecessaryMorale = n
						Else
							DataErrorMessage(wname & "の必要気力の設定が間違っています。", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								wd.NecessaryMorale = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'地形適応
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "のクリティカル率が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If Len(buf2) = 4 Then
						wd.Adaption = buf2
					Else
						DataErrorMessage(wname & "の地形適応の設定が間違っています。", fname, line_num, line_buf, data_name)
						wd.Adaption = "----"
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Adaption = LIndex(buf2, 1)
						End If
					End If
					
					'クリティカル率
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = wname & "の武器属性が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						n = CInt(buf2)
						If n > 999 Then
							n = 999
						ElseIf n < -999 Then 
							n = -999
						End If
						wd.Critical = n
					Else
						DataErrorMessage(wname & "のクリティカル率の設定が間違っています。", fname, line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							wd.Critical = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'武器属性
					buf = Trim(buf)
					If Len(buf) = 0 Then
						DataErrorMessage(wname & "の武器属性の設定が間違っています。", fname, line_num, line_buf, data_name)
					End If
					If Right(buf, 1) = ")" Then
						'必要技能
						ret = InStr(buf, "> ")
						If ret > 0 Then
							If ret > 0 Then
								wd.NecessarySkill = Mid(buf, ret + 2)
								buf = Trim(Left(buf, ret + 1))
								ret = InStr(wd.NecessarySkill, "(")
								wd.NecessarySkill = Mid(wd.NecessarySkill, ret + 1, Len(wd.NecessarySkill) - ret - 1)
							End If
						Else
							ret = InStr(buf, "(")
							If ret > 0 Then
								wd.NecessarySkill = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
								buf = Trim(Left(buf, ret - 1))
							End If
						End If
					End If
					If Right(buf, 1) = ">" Then
						'必要条件
						ret = InStr(buf, "<")
						If ret > 0 Then
							wd.NecessaryCondition = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
							buf = Trim(Left(buf, ret - 1))
						End If
					End If
					wd.Class_Renamed = buf
					If wd.Class_Renamed = "-" Then
						wd.Class_Renamed = ""
					End If
					If InStr(wd.Class_Renamed, "Lv") > 0 Then
						DataErrorMessage(wname & "の属性のレベル指定が間違っています。", fname, line_num, line_buf, data_name)
					End If
					
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Loop 
				
				If line_buf <> "===" Then
					GoTo SkipRest
				End If
				
				'アビリティデータ
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0
					'アビリティ名
					ret = InStr(line_buf, ",")
					If ret = 0 Then
						err_msg = "アビリティデータの終りには空行を置いてください。"
						Error(0)
					End If
					sname = Trim(Left(line_buf, ret - 1))
					buf = Mid(line_buf, ret + 1)
					
					If sname = "" Then
						err_msg = "アビリティ名の設定が間違っています。"
						Error(0)
					End If
					
					'アビリティを登録
					sd = .AddAbility(sname)
					
					'効果
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "の射程の設定が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					sd.SetEffect(buf2)
					
					'射程
					sd.MinRange = 0
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "の回数の設定が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If IsNumeric(buf2) Then
						sd.MaxRange = MinLng(CInt(buf2), 99)
					ElseIf buf2 = "-" Then 
						sd.MaxRange = 0
					Else
						DataErrorMessage(sname & "の射程の設定が間違っています。", fname, line_num, line_buf, data_name)
						If LLength(buf2) > 1 Then
							buf = LIndex(buf2, 2) & "," & buf
							sd.MaxRange = StrToLng(LIndex(buf2, 1))
						End If
					End If
					
					'回数
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "の消費ＥＮの設定が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							sd.Stock = MinLng(CInt(buf2), 99)
						Else
							DataErrorMessage(sname & "の回数の設定が間違っています。", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								sd.Stock = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'消費ＥＮ
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "の必要気力の設定が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							sd.ENConsumption = MinLng(CInt(buf2), 999)
						Else
							DataErrorMessage(sname & "の消費ＥＮの設定が間違っています。", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								sd.ENConsumption = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'必要気力
					ret = InStr(buf, ",")
					If ret = 0 Then
						err_msg = sname & "のアビリティ属性の設定が抜けています。"
						Error(0)
					End If
					buf2 = Trim(Left(buf, ret - 1))
					buf = Mid(buf, ret + 1)
					If buf2 <> "-" Then
						If IsNumeric(buf2) Then
							n = CInt(buf2)
							If n > 1000 Then
								n = 1000
							ElseIf n < 0 Then 
								n = 0
							End If
							sd.NecessaryMorale = n
						Else
							DataErrorMessage(sname & "の必要気力の設定が間違っています。", fname, line_num, line_buf, data_name)
							If LLength(buf2) > 1 Then
								buf = LIndex(buf2, 2) & "," & buf
								sd.NecessaryMorale = StrToLng(LIndex(buf2, 1))
							End If
						End If
					End If
					
					'アビリティ属性
					buf = Trim(buf)
					If Len(buf) = 0 Then
						DataErrorMessage(sname & "のアビリティ属性の設定が間違っています。", fname, line_num, line_buf, data_name)
					End If
					If Right(buf, 1) = ")" Then
						'必要技能
						ret = InStr(buf, "> ")
						If ret > 0 Then
							If ret > 0 Then
								sd.NecessarySkill = Mid(buf, ret + 2)
								buf = Trim(Left(buf, ret + 1))
								ret = InStr(sd.NecessarySkill, "(")
								sd.NecessarySkill = Mid(sd.NecessarySkill, ret + 1, Len(sd.NecessarySkill) - ret - 1)
							End If
						Else
							ret = InStr(buf, "(")
							If ret > 0 Then
								sd.NecessarySkill = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
								buf = Trim(Left(buf, ret - 1))
							End If
						End If
					End If
					If Right(buf, 1) = ">" Then
						'必要条件
						ret = InStr(buf, "<")
						If ret > 0 Then
							sd.NecessaryCondition = Trim(Mid(buf, ret + 1, Len(buf) - ret - 1))
							buf = Trim(Left(buf, ret - 1))
						End If
					End If
					sd.Class_Renamed = buf
					If sd.Class_Renamed = "-" Then
						sd.Class_Renamed = ""
					End If
					If InStr(sd.Class_Renamed, "Lv") > 0 Then
						DataErrorMessage(sname & "の属性のレベル指定が間違っています。", fname, line_num, line_buf, data_name)
					End If
					
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Loop 
			End With
SkipRest: 
		Loop 
		
ErrorHandler: 
		'エラー処理
		If line_num = 0 Then
			ErrorMessage(fname & "が開けません")
		Else
			FileClose(FileNumber)
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
End Class