Option Strict Off
Option Explicit On
Friend Class NonPilotDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'全ノンパイロットデータを管理するリストのクラス
	
	'ノンパイロットデータのコレクション
	Private colNonPilotDataList As New Collection
	
	'クラスの初期化
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		Dim npd As New NonPilotData
		
		'Talkコマンド用
		With npd
			.Name = "ナレーター"
			.Nickname = "ナレーター"
			.Bitmap = ".bmp"
		End With
		colNonPilotDataList.Add(npd, npd.Name)
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		With colNonPilotDataList
			For i = 1 To .Count()
				.Remove(1)
			Next 
		End With
		'UPGRADE_NOTE: オブジェクト colNonPilotDataList をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		colNonPilotDataList = Nothing
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	'ノンパイロットデータリストにデータを追加
	Public Function Add(ByRef pname As String) As NonPilotData
		Dim new_pilot_data As New NonPilotData
		
		new_pilot_data.Name = pname
		colNonPilotDataList.Add(new_pilot_data, pname)
		Add = new_pilot_data
	End Function
	
	'ノンパイロットデータリストに登録されているデータの総数
	Public Function Count() As Short
		Count = colNonPilotDataList.Count()
	End Function
	
	'ノンパイロットデータリストからデータを削除
	Public Sub Delete(ByRef Index As Object)
		colNonPilotDataList.Remove(Index)
	End Sub
	
	'ノンパイロットデータリストからデータを取り出す
	Public Function Item(ByRef Index As Object) As NonPilotData
		Dim pd As NonPilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		Item = colNonPilotDataList.Item(Index)
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		pname = CStr(Index)
		For	Each pd In colNonPilotDataList
			If pd.Nickname0 = pname Then
				Item = pd
				Exit Function
			End If
		Next pd
	End Function
	
	'ノンパイロットデータリストに指定したデータが定義されているか？
	Public Function IsDefined(ByRef Index As Object) As Boolean
		Dim pd As NonPilotData
		Dim pname As String
		
		On Error GoTo ErrorHandler
		pd = colNonPilotDataList.Item(Index)
		IsDefined = True
		Exit Function
		
ErrorHandler: 
		'UPGRADE_WARNING: オブジェクト Index の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
		pname = CStr(Index)
		For	Each pd In colNonPilotDataList
			If pd.Nickname0 = pname Then
				IsDefined = True
				Exit Function
			End If
		Next pd
		IsDefined = False
	End Function
	
	'ノンパイロットデータリストに指定したデータが定義されているか？ (愛称は見ない)
	Public Function IsDefined2(ByRef Index As Object) As Boolean
		Dim pd As NonPilotData
		
		On Error GoTo ErrorHandler
		pd = colNonPilotDataList.Item(Index)
		IsDefined2 = True
		Exit Function
		
ErrorHandler: 
		IsDefined2 = False
	End Function
	
	'データファイル fname からデータをロード
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim line_num As Integer
		Dim buf, line_buf, buf2 As String
		Dim ret As Short
		Dim pd As New NonPilotData
		Dim data_name As String
		Dim err_msg As String
		
		On Error GoTo ErrorHandler
		
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input, OpenAccess.Read)
		
		line_num = 0
		
		Do While True
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
				err_msg = "名称に全角括弧は使用出来ません。"
				Error(0)
			End If
			If InStr(data_name, """") > 0 Then
				err_msg = "名称に""は使用出来ません。"
				Error(0)
			End If
			
			If IsDefined(data_name) Then
				'すでに定義されているノンパイロットのデータであれば置き換える
				If Item(data_name).Name = data_name Then
					pd = Item(data_name)
				Else
					pd = Add(data_name)
				End If
			Else
				pd = Add(data_name)
			End If
			
			With pd
				'愛称、ビットマップ
				GetLine(FileNumber, line_buf, line_num)
				
				'愛称
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "ビットマップの設定が抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				If buf2 <> "" Then
					.Nickname = buf2
				Else
					err_msg = "愛称の設定が間違っています。"
					Error(0)
				End If
				
				'ビットマップ
				buf2 = Trim(buf)
				If LCase(Right(buf2, 4)) = ".bmp" Then
					.Bitmap = buf2
				Else
					DataErrorMessage("ビットマップの設定が間違っています。", fname, line_num, line_buf, .Name)
				End If
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