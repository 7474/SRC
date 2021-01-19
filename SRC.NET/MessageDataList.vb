Option Strict Off
Option Explicit On
Friend Class MessageDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'全メッセージデータ(または特殊効果データ)を管理するコレクションクラス
	
	'メッセージデータ(または特殊効果データ)一覧
	Private colMessageDataList As New Collection
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colMessageDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colMessageDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colMessageDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'メッセージデータの追加
	Public Function Add(ByRef mname As String) As MessageData
		Dim new_md As New MessageData
		
		new_md.Name = mname
		colMessageDataList.Add(new_md, mname)
		Add = new_md
	End Function
	
	'メッセージデータの総数
	Public Function Count() As Short
		Count = colMessageDataList.Count()
	End Function
	
	Public Sub Delete(ByRef Index As Object)
		colMessageDataList.Remove(Index)
	End Sub
	
	'メッセージデータの検索
	Public Function Item(ByRef Index As Object) As MessageData
		On Error GoTo ErrorHandler
		
		Item = colMessageDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_NOTE: オブジェクト Item をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		Item = Nothing
	End Function
	
	'メッセージデータが登録されているか？
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim md As MessageData
		
		On Error GoTo ErrorHandler
		md = colMessageDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'メッセージデータをファイルからロード
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim ret As Short
		Dim line_num As Integer
		Dim line_buf As String
		Dim md As MessageData
		Dim is_effect As Boolean
		Dim sname, msg As String
		Dim data_name As String
		Dim err_msg As String
		
		'特殊効果データor戦闘アニメデータか？
		If InStr(LCase(fname), "effect.txt") > 0 Or InStr(LCase(fname), "animation.txt") > 0 Then
			is_effect = True
		End If
		
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
			
			data_name = line_buf
			
			If InStr(data_name, " ") > 0 Then
				err_msg = "名称に半角スペースは使用出来ません。"
				Error(0)
			End If
			If InStr(data_name, "（") > 0 Or InStr(data_name, "）") > 0 Then
				err_msg = "名称に全角括弧は使用出来ません。"
				Error(0)
			End If
			If InStr(data_name, """") > 0 Then
				err_msg = "名称に""は使用出来ません。"
				Error(0)
			End If
			
			'重複して定義されたデータの場合
			If IsDefined(data_name) Then
				If Not is_effect Then
					'パイロットメッセージの場合は後から定義されたものを優先
					Delete(data_name)
					md = Add(data_name)
				Else
					'特殊効果データの場合は既存のものに追加
					md = Item(data_name)
				End If
			Else
				md = Add(data_name)
			End If
			
			With md
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0
					ret = InStr(line_buf, ",")
					
					If ret < 2 Then
						Error(0)
					End If
					
					sname = Left(line_buf, ret - 1)
					msg = Trim(Mid(line_buf, ret + 1))
					
					If Len(sname) = 0 Then
						err_msg = "シチュエーションの指定が抜けています。"
						Error(0)
					End If
					
					.AddMessage(sname, msg)
					
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
				Loop 
			End With
		Loop 
		
ErrorHandler: 
		'エラー処理
		If line_num = 0 Then
			ErrorMessage(fname & "が開けません。")
		Else
			FileClose(FileNumber)
			DataErrorMessage("", fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
	
	'デフォルトの戦闘アニメデータを定義
	Public Sub AddDefaultAnimation()
		Dim md As MessageData
		
		'アニメデータが用意されていない場合は Data\System\animation.txt を読み込む
		If Count() = 0 Then
			If FileExists(AppPath & "Data\System\animation.txt") Then
				Load(AppPath & "Data\System\animation.txt")
			End If
		End If
		
		If IsDefined("汎用") Then
			md = Item("汎用")
		Else
			md = Add("汎用")
		End If
		
		With md
			If FindNormalLabel("戦闘アニメ_回避発動") > 0 Then
				If .SelectMessage("回避") = "" Then
					.AddMessage("回避", "回避")
				End If
			End If
			If FindNormalLabel("戦闘アニメ_切り払い発動") > 0 Then
				If .SelectMessage("切り払い") = "" Then
					.AddMessage("切り払い", "切り払い")
				End If
			End If
			If FindNormalLabel("戦闘アニメ_迎撃発動") > 0 Then
				If .SelectMessage("迎撃") = "" Then
					.AddMessage("迎撃", "迎撃")
				End If
			End If
			If FindNormalLabel("戦闘アニメ_ダミー発動") > 0 Then
				If .SelectMessage("ダミー") = "" Then
					.AddMessage("ダミー", "ダミー")
				End If
			End If
			If FindNormalLabel("戦闘アニメ_修理装置発動") > 0 Then
				If .SelectMessage("修理") = "" Then
					.AddMessage("修理", "修理装置")
				End If
			End If
			If FindNormalLabel("戦闘アニメ_補給装置発動") > 0 Then
				If .SelectMessage("補給") = "" Then
					.AddMessage("補給", "補給装置")
				End If
			End If
			If FindNormalLabel("戦闘アニメ_終了発動") > 0 Then
				If .SelectMessage("終了") = "" Then
					.AddMessage("終了", "終了")
				End If
			End If
		End With
	End Sub
End Class