Option Strict Off
Option Explicit On
Friend Class BattleConfigDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'バトルコンフィグデータを管理するクラス
	' --- ダメージ計算、命中率算出など、バトルに関連するエリアスの定義を設定します。
	
	'バトルコンフィグデータのコレクション
	Private colBattleConfigData As New Collection
	
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colBattleConfigData
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colBattleConfigData をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colBattleConfigData = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'バトルコンフィグデータリストにデータを追加
	Public Function Add(ByRef cname As String) As BattleConfigData
		Dim cd As New BattleConfigData
		
		cd.Name = cname
		colBattleConfigData.Add(cd, cname)
		Add = cd
	End Function
	
	'バトルコンフィグデータリストに登録されているデータの総数
	Public Function Count() As Short
		Count = colBattleConfigData.Count()
	End Function
	
	'バトルコンフィグデータリストからデータを削除
	Public Sub Delete(ByRef Index As Object)
		colBattleConfigData.Remove(Index)
	End Sub
	
	'バトルコンフィグデータリストからデータを取り出す
	Public Function Item(ByRef Index As String) As BattleConfigData
		Item = colBattleConfigData.Item(Index)
	End Function
	
	'バトルコンフィグデータリストに指定したデータが定義されているか？
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim cd As BattleConfigData
		
		On Error GoTo ErrorHandler
		cd = colBattleConfigData.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		IsDefined = False
	End Function
	
	'データファイル fname からデータをロード
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim line_num As Integer
		Dim line_buf As String
		Dim cd As BattleConfigData
		Dim data_name As String
		Dim err_msg As String
		
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
			
			data_name = line_buf
			
			If IsDefined(data_name) Then
				'すでに定義されているエリアスのデータであれば置き換える
				Delete(data_name)
			End If
			cd = Add(data_name)
			
			With cd
				Do While True
					If EOF(FileNumber) Then
						FileClose(FileNumber)
						Exit Sub
					End If
					
					GetLine(FileNumber, line_buf, line_num)
					
					If Len(line_buf) = 0 Then
						Exit Do
					End If
					
					.ConfigCalc = line_buf
				Loop 
			End With
		Loop 
		
ErrorHandler: 
		'エラー処理
		If line_num = 0 Then
			ErrorMessage(fname & "が開けません。")
		Else
			FileClose(FileNumber)
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		TerminateSRC()
	End Sub
End Class