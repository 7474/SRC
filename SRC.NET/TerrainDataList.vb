Option Strict Off
Option Explicit On
Friend Class TerrainDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'全地形データを管理するリストのクラス
	
	'地形データの登録数
	Public Count As Short
	
	'地形データの配列
	'他のリスト管理用クラスと異なり配列を使っているのはアクセスを高速化するため
	'UPGRADE_NOTE: TerrainDataList は TerrainDataList_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private TerrainDataList_Renamed(MAX_TERRAIN_DATA_NUM) As TerrainData
	
	'地形データの登録順を記録するための配列
	'UPGRADE_WARNING: 配列 OrderList の下限が 1 から 0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
	Private OrderList(MAX_TERRAIN_DATA_NUM) As Short
	
	
	'クラスの初期化
	'UPGRADE_NOTE: Class_Initialize は Class_Initialize_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Initialize_Renamed()
		Dim i As Short
		
		For i = 0 To MAX_TERRAIN_DATA_NUM
			TerrainDataList_Renamed(i) = New TerrainData
		Next 
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate は Class_Terminate_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		For i = 0 To MAX_TERRAIN_DATA_NUM
			'UPGRADE_NOTE: オブジェクト TerrainDataList_Renamed() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			TerrainDataList_Renamed(i) = Nothing
		Next 
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	
	'指定したデータは登録されているか？
	Public Function IsDefined(ByVal ID As Short) As Boolean
		If TerrainDataList_Renamed(ID).ID >= 0 Then
			IsDefined = True
		Else
			IsDefined = False
		End If
	End Function
	
	
	'地形データリストから指定したデータを取り出す
	Public Function Item(ByVal ID As Short) As TerrainData
		Item = TerrainDataList_Renamed(ID)
	End Function
	
	'指定したデータの名称
	Public Function Name(ByVal ID As Short) As String
		Name = TerrainDataList_Renamed(ID).Name
	End Function
	
	'指定したデータの画像ファイル名
	Public Function Bitmap(ByVal ID As Short) As String
		Bitmap = TerrainDataList_Renamed(ID).Bitmap_Renamed
	End Function
	
	'指定したデータのクラス
	'UPGRADE_NOTE: Class は Class_Renamed にアップグレードされました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' をクリックしてください。
	Public Function Class_Renamed(ByVal ID As Short) As String
		Class_Renamed = TerrainDataList_Renamed(ID).Class_Renamed
	End Function
	
	'指定したデータの移動コスト
	Public Function MoveCost(ByVal ID As Short) As Short
		MoveCost = TerrainDataList_Renamed(ID).MoveCost
	End Function
	
	'指定したデータの命中修正
	Public Function HitMod(ByVal ID As Short) As Short
		HitMod = TerrainDataList_Renamed(ID).HitMod
	End Function
	
	'指定したデータのダメージ修正
	Public Function DamageMod(ByVal ID As Short) As Short
		DamageMod = TerrainDataList_Renamed(ID).DamageMod
	End Function
	
	
	'指定したデータの特殊能力
	
	Public Function IsFeatureAvailable(ByVal ID As Short, ByRef ftype As String) As Boolean
		IsFeatureAvailable = TerrainDataList_Renamed(ID).IsFeatureAvailable(ftype)
	End Function
	
	Public Function FeatureLevel(ByVal ID As Short, ByRef ftype As String) As Double
		FeatureLevel = TerrainDataList_Renamed(ID).FeatureLevel(ftype)
	End Function
	
	Public Function FeatureData(ByVal ID As Short, ByRef ftype As String) As String
		FeatureData = TerrainDataList_Renamed(ID).FeatureData(ftype)
	End Function
	
	
	'Ｎ番目に登録したデータの番号
	Public Function OrderedID(ByVal n As Short) As Short
		OrderedID = OrderList(n)
	End Function
	
	
	'データファイル fname からデータをロード
	Public Sub Load(ByRef fname As String)
		Dim FileNumber As Short
		Dim ret As Short
		Dim line_num As Integer
		Dim i, j As Short
		Dim buf, line_buf, buf2 As String
		Dim td As TerrainData
		Dim data_id As Short
		Dim data_name As String
		Dim err_msg As String
		Dim in_quote As Boolean
		
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
			
			'番号
			If IsNumeric(line_buf) Then
				data_id = CShort(line_buf)
			Else
				err_msg = "番号の設定が間違っています。"
				Error(0)
			End If
			If data_id < 0 Or data_id >= MAX_TERRAIN_DATA_NUM Then
				err_msg = "番号の設定が間違っています。"
				Error(0)
			End If
			
			td = TerrainDataList_Renamed(data_id)
			
			With td
				'新規登録？
				If .ID < 0 Then
					Count = Count + 1
					OrderList(Count) = data_id
				Else
					.Clear()
				End If
				.ID = data_id
				
				'名称, 画像ファイル名
				GetLine(FileNumber, line_buf, line_num)
				
				'名称
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "画像ファイル名が抜けています。"
					Error(0)
				End If
				data_name = Trim(Left(line_buf, ret - 1))
				.Name = data_name
				buf = Mid(line_buf, ret + 1)
				
				'画像ファイル名
				.Bitmap_Renamed = Trim(buf)
				If Len(.Bitmap_Renamed) = 0 Then
					err_msg = "画像ファイル名が指定されていません。"
					Error(0)
				End If
				
				'地形タイプ, 移動コスト, 命中修正, ダメージ修正
				GetLine(FileNumber, line_buf, line_num)
				
				'地形タイプ
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "移動コストが抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				.Class_Renamed = buf2
				
				'移動コスト
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "命中修正が抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If buf2 = "-" Then
					.MoveCost = 1000
				ElseIf IsNumeric(buf2) Then 
					'0.5刻みの移動コストを使えるようにするため、実際の２倍の値で記録する
					.MoveCost = CShort(2 * CDbl(buf2))
				End If
				If .MoveCost <= 0 Then
					DataErrorMessage("移動コストの設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'命中修正
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "ダメージ修正が抜けています。"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.HitMod = CShort(buf2)
				Else
					DataErrorMessage("命中修正の設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'ダメージ修正
				ret = InStr(buf, ",")
				If ret > 0 Then
					err_msg = "余分な「,」が指定されています。"
					Error(0)
				End If
				buf2 = Trim(buf)
				If IsNumeric(buf2) Then
					.DamageMod = CShort(buf2)
				Else
					DataErrorMessage("ダメージ修正の設定が間違っています。", fname, line_num, line_buf, data_name)
				End If
				
				'地形効果
				GetLine(FileNumber, line_buf, line_num)
				Do While Len(line_buf) > 0
					buf = line_buf
					i = 0
					Do While Len(buf) > 0
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
						
						If buf2 <> "" Then
							.AddFeature(buf2)
						Else
							DataErrorMessage("行頭から" & VB6.Format(i) & "番目の地形効果の設定が間違っています。", fname, line_num, line_buf, data_name)
						End If
					Loop 
					
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
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		
		End
	End Sub
	
	'リストをクリア
	Public Sub Clear()
		Dim i As Short
		
		For i = 0 To MAX_TERRAIN_DATA_NUM - 1
			TerrainDataList_Renamed(i).Clear()
		Next 
		Count = 0
	End Sub
End Class