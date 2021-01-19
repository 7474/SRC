Option Strict Off
Option Explicit On
Module Map
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' 本プログラムはフリーソフトであり、無保証です。
	' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
	' 再頒布または改変することができます。
	
	'マップデータに関する各種処理を行うモジュール
	
	'管理可能な地形データの総数
	Public Const MAX_TERRAIN_DATA_NUM As Short = 2000
	
	'ADD START 240a
	'レイヤー無しの固定値
	Public Const NO_LAYER_NUM As Short = 10000
	'ADD  END  240a
	
	'マップファイル名
	Public MapFileName As String
	'マップの横サイズ
	Public MapWidth As Short
	'マップの縦サイズ
	Public MapHeight As Short
	
	'マップの描画モード
	Public MapDrawMode As String
	'フィルタ色
	Public MapDrawFilterColor As Integer
	'フィルタの透過度
	Public MapDrawFilterTransPercent As Double
	'フィルタやSepiaコマンドなどでユニットの色を変更するか
	Public MapDrawIsMapOnly As Boolean
	
	'マップに画像の書き込みがなされたか
	Public IsMapDirty As Boolean
	
	'マップデータを記録する配列
	' MapData(*,*,0)は地形の種類
	' MapData(*,*,1)はビットマップの番号
	'ADD START 240a
	'2～3はマップ上層レイヤーデータ
	' MapData(*,*,2)は地形の種類。未設定はNO_LAYER_NUM
	' MapData(*,*,3)はビットマップの番号。未設定はNO_LAYER_NUM
	' MapData(*,*,4)はマスのデータタイプ。1:下層 2:上層 3:上層データのみ 4:上層見た目のみ
	'ADD  END  240a
	Public MapData() As Short
	
	'ADD START 240a
	Public Enum MapDataIndex
		TerrainType = 0
		BitmapNo = 1
		LayerType = 2
		LayerBitmapNo = 3
		BoxType = 4
	End Enum
	Public Enum BoxTypes
		Under = 1
		Upper = 2
		UpperDataOnly = 3
		UpperBmpOnly = 4
	End Enum
	'ADD  END  240a
	
	'マップの画像ファイルの格納形式
	Enum MapImageFileType
		OldMapImageFileType '旧形式 (plain0.bmp)
		FourFiguresMapImageFileType '４桁の数値 (plain0000.bmp)
		SeparateDirMapImageFileType 'ディレクトリ分割 (plain\plain0000.bmp)
	End Enum
	Public MapImageFileTypeData() As MapImageFileType
	
	'マップ上に存在するユニットを記録する配列
	Public MapDataForUnit() As Unit
	
	'マップ上でターゲットを選択する際のマスク情報
	Public MaskData() As Boolean
	
	'現在地点からその地点まで移動するのに必要な移動力の配列
	Public TotalMoveCost() As Integer
	
	'各地点がＺＯＣの影響下にあるかどうか
	Public PointInZOC() As Integer
	
	
	'地形情報テーブルを初期化
	Public Sub InitMap()
		Dim i, j As Short
		
		SetMapSize(MainWidth, MainHeight)
		
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'MOD START 240a
				'            MapData(i, j, 0) = 0
				'            MapData(i, j, 1) = 0
				MapData(i, j, MapDataIndex.TerrainType) = 0
				MapData(i, j, MapDataIndex.BitmapNo) = 0
				MapData(i, j, MapDataIndex.LayerType) = 0
				MapData(i, j, MapDataIndex.LayerBitmapNo) = 0
				MapData(i, j, MapDataIndex.BoxType) = 0
				'ADD  END  240a
			Next 
		Next 
	End Sub
	
	'(X,Y)地点の命中修正
	Public Function TerrainEffectForHit(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainEffectForHit = TDList.HitMod(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				TerrainEffectForHit = TDList.HitMod(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				TerrainEffectForHit = TDList.HitMod(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)地点のダメージ修正
	Public Function TerrainEffectForDamage(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)地点のＨＰ回復率
	Public Function TerrainEffectForHPRecover(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainEffectForHPRecover = 10 * TDList.FeatureLevel(MapData(X, Y, 0), "ＨＰ回復")
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				TerrainEffectForHPRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.TerrainType), "ＨＰ回復")
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				TerrainEffectForHPRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.LayerType), "ＨＰ回復")
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)地点のＥＮ回復率
	Public Function TerrainEffectForENRecover(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainEffectForENRecover = 10 * TDList.FeatureLevel(MapData(X, Y, 0), "ＥＮ回復")
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				TerrainEffectForENRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.TerrainType), "ＥＮ回復")
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				TerrainEffectForENRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.LayerType), "ＥＮ回復")
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)地点の地形名称
	Public Function TerrainName(ByVal X As Short, ByVal Y As Short) As String
		'MOD START 240a
		'    TerrainName = TDList.Name(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				TerrainName = TDList.Name(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				TerrainName = TDList.Name(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)地点の地形クラス
	Public Function TerrainClass(ByVal X As Short, ByVal Y As Short) As String
		'MOD START 240a
		'    TerrainClass = TDList.Class(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				TerrainClass = TDList.Class_Renamed(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				TerrainClass = TDList.Class_Renamed(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)地点の移動コスト
	Public Function TerrainMoveCost(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainMoveCost = TDList.MoveCost(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				TerrainMoveCost = TDList.MoveCost(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				TerrainMoveCost = TDList.MoveCost(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)地点に障害物があるか (吹き飛ばし時に衝突するか)
	Public Function TerrainHasObstacle(ByVal X As Short, ByVal Y As Short) As Boolean
		'MOD START 240a
		'    TerrainHasObstacle = TDList.IsFeatureAvailable(MapData(X, Y, 0), "衝突")
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				TerrainHasObstacle = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "衝突")
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				TerrainHasObstacle = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "衝突")
		End Select
		'MOD  END  240a
	End Function
	
	'ADD START 240a
	'(X,Y)地点が移動停止か
	Public Function TerrainHasMoveStop(ByVal X As Short, ByVal Y As Short) As Boolean
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				TerrainHasMoveStop = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "移動停止")
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				TerrainHasMoveStop = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "移動停止")
		End Select
	End Function
	
	'(X,Y)地点が進入禁止か
	Public Function TerrainDoNotEnter(ByVal X As Short, ByVal Y As Short) As Boolean
		Dim ret As Boolean
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "進入禁止")
				If Not ret Then
					'互換性維持のため残している
					ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "侵入禁止")
				End If
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "進入禁止")
				If Not ret Then
					'互換性維持のため残している
					ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "侵入禁止")
				End If
		End Select
	End Function
	
	'(X,Y)地点が指定した能力を持っているか
	Public Function TerrainHasFeature(ByVal X As Short, ByVal Y As Short, ByRef Feature As String) As Boolean
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'上層レイヤが無い場合と上層が画像情報しか持っていない場合は下層のデータを返す
				TerrainHasFeature = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), Feature)
			Case Else
				'上層レイヤが両方持っている場合と情報のみ持っている場合は上層のデータを返す
				TerrainHasFeature = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), Feature)
		End Select
	End Function
	'ADD  END  240a
	
	'(X,Y)地点にいるユニット
	Public Function UnitAtPoint(ByVal X As Short, ByVal Y As Short) As Unit
		If X < 1 Or MapWidth < X Then
			'UPGRADE_NOTE: オブジェクト UnitAtPoint をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			UnitAtPoint = Nothing
			Exit Function
		End If
		If Y < 1 Or MapHeight < Y Then
			'UPGRADE_NOTE: オブジェクト UnitAtPoint をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
			UnitAtPoint = Nothing
			Exit Function
		End If
		UnitAtPoint = MapDataForUnit(X, Y)
	End Function
	
	'指定したマップ画像を検索する
	Public Function SearchTerrainImageFile(ByVal tid As Short, ByVal tbitmap As Short, ByVal tx As Short, ByVal ty As Short) As String
		Dim tbmpname As String
		Dim fname2, fname1, fname3 As String
		Static init_setup_background As Boolean
		Static scenario_map_dir_exists As Boolean
		Static extdata_map_dir_exists As Boolean
		Static extdata2_map_dir_exists As Boolean
		
		'ADD START 240a
		'画像無が確定してるなら処理しない
		If tid = NO_LAYER_NUM Then
			Exit Function
		ElseIf tbitmap = NO_LAYER_NUM Then 
			Exit Function
		End If
		'ADD  END  240a
		
		'初めて実行する際に、各フォルダにBitmap\Mapフォルダがあるかチェック
		If Not init_setup_background Then
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ScenarioPath & "Bitmap\Map", FileAttribute.Directory)) > 0 Then
				scenario_map_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath & "Bitmap\Map", FileAttribute.Directory)) > 0 Then
				extdata_map_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir に新しい動作が指定されています。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' をクリックしてください。
			If Len(Dir(ExtDataPath2 & "Bitmap\Map", FileAttribute.Directory)) > 0 Then
				extdata2_map_dir_exists = True
			End If
			init_setup_background = True
		End If
		
		'マップ画像のファイル名を作成
		tbmpname = TDList.Bitmap(tid)
		fname1 = "\Bitmap\Map\" & tbmpname & "\" & tbmpname & VB6.Format(tbitmap, "0000") & ".bmp"
		fname2 = "\Bitmap\Map\" & tbmpname & VB6.Format(tbitmap, "0000") & ".bmp"
		fname3 = "\Bitmap\Map\" & tbmpname & VB6.Format(tbitmap) & ".bmp"
		
		'ビットマップを探す
		If scenario_map_dir_exists Then
			If FileExists(ScenarioPath & fname1) Then
				SearchTerrainImageFile = ScenarioPath & fname1
				MapImageFileTypeData(tx, ty) = MapImageFileType.SeparateDirMapImageFileType
				Exit Function
			End If
			If FileExists(ScenarioPath & fname2) Then
				SearchTerrainImageFile = ScenarioPath & fname2
				MapImageFileTypeData(tx, ty) = MapImageFileType.FourFiguresMapImageFileType
				Exit Function
			End If
			If FileExists(ScenarioPath & fname3) Then
				SearchTerrainImageFile = ScenarioPath & fname3
				MapImageFileTypeData(tx, ty) = MapImageFileType.OldMapImageFileType
				Exit Function
			End If
		End If
		If extdata_map_dir_exists Then
			If FileExists(ExtDataPath & fname1) Then
				SearchTerrainImageFile = ExtDataPath & fname1
				MapImageFileTypeData(tx, ty) = MapImageFileType.SeparateDirMapImageFileType
				Exit Function
			End If
			If FileExists(ExtDataPath & fname2) Then
				SearchTerrainImageFile = ExtDataPath & fname2
				MapImageFileTypeData(tx, ty) = MapImageFileType.FourFiguresMapImageFileType
				Exit Function
			End If
			If FileExists(ExtDataPath & fname3) Then
				SearchTerrainImageFile = ExtDataPath & fname3
				MapImageFileTypeData(tx, ty) = MapImageFileType.OldMapImageFileType
				Exit Function
			End If
		End If
		If extdata2_map_dir_exists Then
			If FileExists(ExtDataPath2 & fname1) Then
				SearchTerrainImageFile = ExtDataPath2 & fname1
				MapImageFileTypeData(tx, ty) = MapImageFileType.SeparateDirMapImageFileType
				Exit Function
			End If
			If FileExists(ExtDataPath2 & fname2) Then
				SearchTerrainImageFile = ExtDataPath2 & fname2
				MapImageFileTypeData(tx, ty) = MapImageFileType.FourFiguresMapImageFileType
				Exit Function
			End If
			If FileExists(ExtDataPath2 & fname3) Then
				SearchTerrainImageFile = ExtDataPath2 & fname3
				MapImageFileTypeData(tx, ty) = MapImageFileType.OldMapImageFileType
				Exit Function
			End If
		End If
		If FileExists(AppPath & fname1) Then
			SearchTerrainImageFile = AppPath & fname1
			MapImageFileTypeData(tx, ty) = MapImageFileType.SeparateDirMapImageFileType
			Exit Function
		End If
		If FileExists(AppPath & fname2) Then
			SearchTerrainImageFile = AppPath & fname2
			MapImageFileTypeData(tx, ty) = MapImageFileType.FourFiguresMapImageFileType
			Exit Function
		End If
		If FileExists(AppPath & fname3) Then
			SearchTerrainImageFile = AppPath & fname3
			MapImageFileTypeData(tx, ty) = MapImageFileType.OldMapImageFileType
			Exit Function
		End If
	End Function
	
	
	'マップファイル fname のデータをロード
	Public Sub LoadMapData(ByRef fname As String)
		Dim FileNumber As Short
		Dim i, j As Short
		Dim buf As String
		
		'ファイルが存在しない場合
		If fname = "" Or Not FileExists(fname) Then
			MapFileName = ""
			If InStr(ScenarioFileName, "ステータス表示.") > 0 Or InStr(ScenarioFileName, "ランキング.") > 0 Then
				SetMapSize(MainWidth, 40)
			Else
				SetMapSize(MainWidth, MainHeight)
			End If
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					'MOD START 240a
					'                MapData(i, j, 0) = MAX_TERRAIN_DATA_NUM
					'                MapData(i, j, 1) = 0
					'ファイルが無い場合
					MapData(i, j, MapDataIndex.TerrainType) = MAX_TERRAIN_DATA_NUM
					MapData(i, j, MapDataIndex.BitmapNo) = 0
					MapData(i, j, MapDataIndex.LayerType) = NO_LAYER_NUM
					MapData(i, j, MapDataIndex.LayerBitmapNo) = NO_LAYER_NUM
					MapData(i, j, MapDataIndex.BoxType) = BoxTypes.Under
					'MOD  END  240a
				Next 
			Next 
			Exit Sub
		End If
		
		On Error GoTo ErrorHandler
		
		'ファイルを開く
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input)
		
		'ファイル名を記録しておく
		MapFileName = fname
		
		'ファイルの先頭にあるマップサイズ情報を収得
		Input(FileNumber, buf)
		If buf <> "MapData" Then
			'旧形式のマップデータ
			SetMapSize(20, 20)
			FileClose(FileNumber)
			
			FileNumber = FreeFile
			FileOpen(FileNumber, fname, OpenMode.Input)
		Else
			Input(FileNumber, buf)
			Input(FileNumber, i)
			Input(FileNumber, j)
			SetMapSize(i, j)
		End If
		
		'マップデータを読み込み
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'MOD START 240a
				'            Input #FileNumber, MapData(i, j, 0), MapData(i, j, 1)
				'            If Not TDList.IsDefined(MapData(i, j, 0)) Then
				'                MsgBox "定義されていない" & Format$(MapData(i, j, 0)) _
				''                    & "番の地形データが使われています"
				Input(FileNumber, MapData(i, j, MapDataIndex.TerrainType))
				Input(FileNumber, MapData(i, j, MapDataIndex.BitmapNo))
				If Not TDList.IsDefined(MapData(i, j, MapDataIndex.TerrainType)) Then
					MsgBox("定義されていない" & VB6.Format(MapData(i, j, MapDataIndex.TerrainType)) & "番の地形データが使われています")
					'MOD  END  240a
					FileClose(FileNumber)
					End
				End If
			Next 
		Next 
		
		'ADD START 240a
		'レイヤーデータ読み込み
		If Not EOF(FileNumber) Then
			Input(FileNumber, buf)
			If buf = "Layer" Then
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Input(FileNumber, MapData(i, j, MapDataIndex.LayerType))
						Input(FileNumber, MapData(i, j, MapDataIndex.LayerBitmapNo))
						If (MapData(i, j, MapDataIndex.LayerType) <> NO_LAYER_NUM) Then
							'定義されていたらデータの妥当性チェック
							If Not TDList.IsDefined(MapData(i, j, MapDataIndex.LayerType)) Then
								MsgBox("定義されていない" & VB6.Format(MapData(i, j, MapDataIndex.LayerType)) & "番の地形データが使われています")
								FileClose(FileNumber)
								End
							End If
							'マスのタイプを上層に
							MapData(i, j, MapDataIndex.BoxType) = BoxTypes.Upper
						Else
							'マスのタイプを下層に（初期化時下層だが、再度明示的に設定する）
							MapData(i, j, MapDataIndex.BoxType) = BoxTypes.Under
						End If
					Next 
				Next 
			End If
		End If
		'ADD  END  240a
		
		FileClose(FileNumber)
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("マップファイル「" & fname & "」のデータが不正です")
		FileClose(FileNumber)
		End
	End Sub
	
	'マップサイズを設定
	Public Sub SetMapSize(ByVal w As Short, ByVal h As Short)
		Dim i, j As Short
		Dim ret As Integer
		
		MapWidth = w
		MapHeight = h
		MapPWidth = 32 * w
		MapPHeight = 32 * h
		MapX = (MainWidth + 1) \ 2
		MapY = (MainHeight + 1) \ 2
		
		'マップ画像サイズを決定
		'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picBack
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Picture = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Move(0, 0, MapPWidth, MapPHeight)
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = PatBlt(.hDC, 0, 0, .width, .Height, BLACKNESS)
		End With
		'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMaskedBack
			'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Picture = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: Control picMaskedBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			.Move(0, 0, MapPWidth, MapPHeight)
		End With
		
		'スクロールバーの移動範囲を決定
		'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.HScroll
			'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			If .max <> MapWidth Then
				'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Visible = False
				'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.max = MapWidth
				'MOD START 240a
				'            If MainWidth = 15 Then
				If Not NewGUIMode Then
					'MOD  END  240a
					'UPGRADE_ISSUE: Control HScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Visible = True
				End If
			End If
		End With
		'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.VScroll
			'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			If .max <> MapHeight Then
				'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.Visible = False
				'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
				.max = MapHeight
				'MOD START 240a
				'            If MainWidth = 15 Then
				If Not NewGUIMode Then
					'MOD  END  240a
					'UPGRADE_ISSUE: Control VScroll は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
					.Visible = True
				End If
			End If
		End With
		
		'マップデータ用配列の領域確保
		'MOD START 240a
		'    ReDim MapData(1 To MapWidth, 1 To MapHeight, 1)
		'UPGRADE_WARNING: 配列 MapData の下限が 1,1,0 から 0,0,0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
		ReDim MapData(MapWidth, MapHeight, 4)
		'MOD  END  240a
		'UPGRADE_WARNING: 配列 MapDataForUnit の下限が 1,1 から 0,0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
		ReDim MapDataForUnit(MapWidth, MapHeight)
		'UPGRADE_WARNING: 配列 MaskData の下限が 1,1 から 0,0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
		ReDim MaskData(MapWidth, MapHeight)
		ReDim TotalMoveCost(MapWidth + 1, MapHeight + 1)
		ReDim PointInZOC(MapWidth + 1, MapHeight + 1)
		'UPGRADE_WARNING: 配列 MapImageFileTypeData の下限が 1,1 から 0,0 に変更されました。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' をクリックしてください。
		ReDim MapImageFileTypeData(MapWidth, MapHeight)
		
		'マップデータ配列の初期化
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'MOD START 240a
				'            MapData(i, j, 0) = 0
				'            MapData(i, j, 1) = 0
				MapData(i, j, MapDataIndex.TerrainType) = 0
				MapData(i, j, MapDataIndex.BitmapNo) = 0
				MapData(i, j, MapDataIndex.LayerType) = NO_LAYER_NUM
				MapData(i, j, MapDataIndex.LayerBitmapNo) = NO_LAYER_NUM
				MapData(i, j, MapDataIndex.BoxType) = BoxTypes.Under
				'MOD  END  240a
				MaskData(i, j) = True
				'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				MapDataForUnit(i, j) = Nothing
			Next 
		Next 
	End Sub
	
	'マップデータをクリア
	Public Sub ClearMap()
		Dim ret As Integer
		
		MapFileName = ""
		MapWidth = 1
		MapHeight = 1
		
		'MOD START 240a
		'    ReDim MapData(1, 1, 1)
		ReDim MapData(1, 1, 4)
		'MOD  END  240a
		ReDim MapDataForUnit(1, 1)
		ReDim MaskData(1, 1)
		
		'MOD START 240a
		'    MapData(1, 1, 0) = 0
		'    MapData(1, 1, 1) = 0
		MapData(1, 1, MapDataIndex.TerrainType) = 0
		MapData(1, 1, MapDataIndex.BitmapNo) = 0
		MapData(1, 1, MapDataIndex.LayerType) = 0
		MapData(1, 1, MapDataIndex.LayerBitmapNo) = 0
		MapData(1, 1, MapDataIndex.BoxType) = 0
		'MOD  END  240a
		'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
		MapDataForUnit(1, 1) = Nothing
		
		'マップ画面を消去
		'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picBack
			'UPGRADE_ISSUE: Control picBack は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = PatBlt(.hDC, 0, 0, .width, .Height, BLACKNESS)
		End With
		'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
		With MainForm.picMain(0)
			'UPGRADE_ISSUE: Control picMain は、汎用名前空間 Form 内にあるため、解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' をクリックしてください。
			ret = PatBlt(.hDC, 0, 0, .width, .Height, BLACKNESS)
		End With
		
		'ユニット画像をリセット
		If MapDrawMode <> "" And Not MapDrawIsMapOnly Then
			UList.ClearUnitBitmap()
		End If
		
		MapDrawMode = ""
		MapDrawIsMapOnly = False
		MapDrawFilterColor = 0
		MapDrawFilterTransPercent = 0
	End Sub
	
	
	'中断用セーブデータにマップデータをセーブ
	Public Sub DumpMapData()
		Dim i, j As Short
		Dim fname As String
		
		If InStr(MapFileName, ScenarioPath) = 1 Then
			fname = Right(MapFileName, Len(MapFileName) - Len(ScenarioPath))
		Else
			fname = MapFileName
		End If
		
		
		If MapDrawIsMapOnly Then
			WriteLine(SaveDataFileNumber, fname, MapDrawMode & "(マップ限定)")
		Else
			WriteLine(SaveDataFileNumber, fname, MapDrawMode)
		End If
		WriteLine(SaveDataFileNumber, MapWidth, MapHeight)
		
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'MOD START 240a
				'            Write #SaveDataFileNumber, MapData(i, j, 0), MapData(i, j, 1)
				WriteLine(SaveDataFileNumber, MapData(i, j, MapDataIndex.TerrainType), MapData(i, j, MapDataIndex.BitmapNo))
				'ADD  END  240a
			Next 
		Next 
		
		WriteLine(SaveDataFileNumber, MapX, MapY)
		
		'ADD START 240a
		'レイヤ情報を書き込む
		WriteLine(SaveDataFileNumber, "Layer")
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				WriteLine(SaveDataFileNumber, MapData(i, j, MapDataIndex.LayerType), MapData(i, j, MapDataIndex.LayerBitmapNo), MapData(i, j, MapDataIndex.BoxType))
			Next 
		Next 
		'ADD  END  240a
		
	End Sub
	
	'中断用セーブデータからマップデータをロード
	'MOD START 240a
	'Sub→Functionに
	'Public Sub RestoreMapData() As String
	Public Function RestoreMapData() As String
		'MOD  END  240a
		Dim sbuf1, sbuf2 As String
		Dim ibuf1, ibuf2 As Short
		'ADD START 240a
		Dim ibuf3, ibuf4 As Short
		Dim buf As String
		'ADD  END  240a
		Dim i, j As Short
		Dim is_map_changed As Boolean
		Dim u As Unit
		
		'マップファイル名, マップ描画方法
		Input(SaveDataFileNumber, sbuf1)
		Input(SaveDataFileNumber, sbuf2)
		If InStr(sbuf1, ":") = 0 Then
			sbuf1 = ScenarioPath & sbuf1
		End If
		If sbuf1 <> MapFileName Then
			MapFileName = sbuf1
			is_map_changed = True
		End If
		If MapDrawIsMapOnly Then
			If sbuf2 <> MapDrawMode & "(マップ限定)" Then
				If Right(sbuf2, 7) = "(マップ限定)" Then
					MapDrawMode = Left(sbuf2, Len(sbuf2) - 7)
					MapDrawIsMapOnly = True
				Else
					MapDrawMode = sbuf2
					MapDrawIsMapOnly = False
				End If
				UList.ClearUnitBitmap()
				is_map_changed = True
			End If
		Else
			If sbuf2 <> MapDrawMode Then
				If Right(sbuf2, 7) = "(マップ限定)" Then
					MapDrawMode = Left(sbuf2, Len(sbuf2) - 7)
					MapDrawIsMapOnly = True
				Else
					MapDrawMode = sbuf2
					MapDrawIsMapOnly = False
				End If
				UList.ClearUnitBitmap()
				is_map_changed = True
			End If
		End If
		
		'マップ幅, マップ高さ
		Input(SaveDataFileNumber, ibuf1)
		Input(SaveDataFileNumber, ibuf2)
		If ibuf1 <> MapWidth Or ibuf2 <> MapHeight Then
			SetMapSize(ibuf1, ibuf2)
		End If
		
		'各地形
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				Input(SaveDataFileNumber, ibuf1)
				Input(SaveDataFileNumber, ibuf2)
				'MOD START 240a
				'            If ibuf1 <> MapData(i, j, 0) Then
				'                MapData(i, j, 0) = ibuf1
				'                is_map_changed = True
				'            End If
				'            If ibuf2 <> MapData(i, j, 1) Then
				'                MapData(i, j, 1) = ibuf2
				'                is_map_changed = True
				'            End If
				If ibuf1 <> MapData(i, j, MapDataIndex.TerrainType) Then
					MapData(i, j, MapDataIndex.TerrainType) = ibuf1
					is_map_changed = True
				End If
				If ibuf2 <> MapData(i, j, MapDataIndex.BitmapNo) Then
					MapData(i, j, MapDataIndex.BitmapNo) = ibuf2
					is_map_changed = True
				End If
				'MOD  END  240a
			Next 
		Next 
		
		'MOV START 240a
		'    '背景書き換えの必要有り？
		'    If is_map_changed Then
		'        IsMapDirty = True
		'    End If
		'MOV  END  240a
		
		'表示位置
		'SetupBackgroundでMapX,MapYが書き換えられてしまうため、この位置で
		'値を参照する必要がある。
		Input(SaveDataFileNumber, MapX)
		Input(SaveDataFileNumber, MapY)
		
		'ADD START 240a
		Input(SaveDataFileNumber, buf)
		If "Layer" = buf Then
			'各レイヤ
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					Input(SaveDataFileNumber, ibuf1)
					Input(SaveDataFileNumber, ibuf2)
					Input(SaveDataFileNumber, ibuf3)
					If ibuf1 <> MapData(i, j, MapDataIndex.LayerType) Then
						MapData(i, j, MapDataIndex.LayerType) = ibuf1
						is_map_changed = True
					End If
					If ibuf2 <> MapData(i, j, MapDataIndex.LayerBitmapNo) Then
						MapData(i, j, MapDataIndex.LayerBitmapNo) = ibuf2
						is_map_changed = True
					End If
					If ibuf3 <> MapData(i, j, MapDataIndex.BoxType) Then
						MapData(i, j, MapDataIndex.BoxType) = ibuf3
						is_map_changed = True
					End If
				Next 
			Next 
			'ＢＧＭ関連情報の1行目を読み込む
			Input(SaveDataFileNumber, buf)
		End If
		RestoreMapData = buf
		'背景書き換えの必要有り？
		If is_map_changed Then
			IsMapDirty = True
		End If
		'ADD  END  240a
		
		'ユニット配置
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				MapDataForUnit(i, j) = Nothing
			Next 
		Next 
		For	Each u In UList
			With u
				If .Status = "出撃" Then
					MapDataForUnit(.X, .Y) = u
				End If
			End With
		Next u
	End Function
	
	
	'(X,Y)を中心とする min_range - max_range のエリアを選択
	'エリア内のユニットは uparty の指示に従い選択
	Public Sub AreaInRange(ByVal X As Short, ByVal Y As Short, ByVal max_range As Short, ByVal min_range As Short, ByRef uparty As String)
		Dim x1, y1 As Short
		Dim x2, y2 As Short
		Dim i, j As Short
		Dim n As Short
		
		'選択情報をクリア
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				MaskData(i, j) = True
			Next 
		Next 
		
		x1 = MaxLng(X - max_range, 1)
		x2 = MinLng(X + max_range, MapWidth)
		y1 = MaxLng(Y - max_range, 1)
		y2 = MinLng(Y + max_range, MapHeight)
		
		'max_range内かつmin_range外のエリアを選択
		For i = x1 To x2
			For j = y1 To y2
				n = System.Math.Abs(X - i) + System.Math.Abs(Y - j)
				If n <= max_range Then
					If n >= min_range Then
						MaskData(i, j) = False
					End If
				End If
			Next 
		Next 
		
		'エリア内のユニットを選択するかそれぞれ判定
		Select Case uparty
			Case "味方", "ＮＰＣ"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "味方" And Not MapDataForUnit(i, j).Party = "ＮＰＣ" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "味方の敵", "ＮＰＣの敵"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If (.Party = "味方" Or .Party = "ＮＰＣ") And Not .IsConditionSatisfied("暴走") And Not .IsConditionSatisfied("魅了") And Not .IsConditionSatisfied("混乱") And Not .IsConditionSatisfied("憑依") And Not .IsConditionSatisfied("睡眠") Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "敵"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "敵" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "敵の敵"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If .Party = "敵" Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "中立"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "中立" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "中立の敵"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If .Party = "中立" Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "空間"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								MaskData(i, j) = True
							End If
						End If
					Next 
				Next 
			Case "全て", "無差別"
		End Select
		
		'エリアの中心は常に選択
		MaskData(X, Y) = False
	End Sub
	
	'ユニット u から移動後使用可能な射程 max_range の武器／アビリティを使う場合の効果範囲
	'エリア内のユニットは Party の指示に従い選択
	Public Sub AreaInReachable(ByRef u As Unit, ByVal max_range As Short, ByRef uparty As String)
		Dim tmp_mask_data() As Boolean
		Dim j, i, k As Short
		
		'まずは移動範囲を選択
		AreaInSpeed(u)
		
		'選択範囲をmax_rangeぶんだけ拡大
		ReDim tmp_mask_data(MapWidth + 1, MapHeight + 1)
		For i = 0 To MapWidth + 1
			For j = 0 To MapHeight + 1
				tmp_mask_data(i, j) = True
			Next 
		Next 
		For i = 1 To max_range
			For j = 1 To MapWidth
				For k = 1 To MapHeight
					tmp_mask_data(j, k) = MaskData(j, k)
				Next 
			Next 
			For j = 1 To MapWidth
				For k = 1 To MapHeight
					MaskData(j, k) = tmp_mask_data(j, k) And tmp_mask_data(j - 1, k) And tmp_mask_data(j + 1, k) And tmp_mask_data(j, k - 1) And tmp_mask_data(j, k + 1)
				Next 
			Next 
		Next 
		
		'エリア内のユニットを選択するかそれぞれ判定
		Select Case uparty
			Case "味方", "ＮＰＣ"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "味方" And Not MapDataForUnit(i, j).Party = "ＮＰＣ" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "味方の敵", "ＮＰＣの敵"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If (.Party = "味方" Or .Party = "ＮＰＣ") And Not .IsConditionSatisfied("暴走") And Not .IsConditionSatisfied("魅了") And Not .IsConditionSatisfied("憑依") Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "敵"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "敵" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "敵の敵"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If .Party = "敵" Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "中立"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "中立" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "中立の敵"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If .Party = "中立" Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "空間"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								MaskData(i, j) = True
							End If
						End If
					Next 
				Next 
			Case "全て", "無差別"
		End Select
		
		'エリアの中心は常に選択
		MaskData(u.X, u.Y) = False
	End Sub
	
	'マップ全域に渡ってupartyに属するユニットが存在する場所を選択
	Public Sub AreaWithUnit(ByRef uparty As String)
		Dim i, j As Short
		Dim u As Unit
		
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				MaskData(i, j) = True
			Next 
		Next 
		
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				u = MapDataForUnit(i, j)
				If u Is Nothing Then
					GoTo NextLoop
				End If
				
				With u
					Select Case uparty
						Case "味方"
							If .Party = "味方" Or .Party = "ＮＰＣ" Then
								MaskData(i, j) = False
							End If
						Case "味方の敵"
							If .Party <> "味方" And .Party <> "ＮＰＣ" Then
								MaskData(i, j) = False
							End If
						Case "敵"
							If .Party = "敵" Then
								MaskData(i, j) = False
							End If
						Case "敵の敵"
							If .Party <> "敵" Then
								MaskData(i, j) = False
							End If
						Case "中立"
							If .Party = "中立" Then
								MaskData(i, j) = False
							End If
						Case "中立の敵"
							If .Party <> "中立" Then
								MaskData(i, j) = False
							End If
						Case Else
							MaskData(i, j) = False
					End Select
				End With
NextLoop: 
			Next 
		Next 
	End Sub
	
	'十字状のエリアを選択 (Ｍ直の攻撃方向選択用)
	Public Sub AreaInCross(ByVal X As Short, ByVal Y As Short, ByVal min_range As Short, ByRef max_range As Short)
		Dim i, j As Short
		
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				MaskData(i, j) = True
			Next 
		Next 
		
		For i = Y - max_range To Y - min_range
			If i >= 1 Then
				MaskData(X, i) = False
			End If
		Next 
		For i = Y + min_range To Y + max_range
			If i <= MapHeight Then
				MaskData(X, i) = False
			End If
		Next 
		For i = X - max_range To X - min_range
			If i >= 1 Then
				MaskData(i, Y) = False
			End If
		Next 
		For i = X + min_range To X + max_range
			If i <= MapWidth Then
				MaskData(i, Y) = False
			End If
		Next 
		MaskData(X, Y) = False
	End Sub
	
	'直線状のエリアを選択 (Ｍ直の攻撃範囲設定用)
	Public Sub AreaInLine(ByVal X As Short, ByVal Y As Short, ByVal min_range As Short, ByRef max_range As Short, ByRef direction As String)
		Dim i, j As Short
		
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				MaskData(i, j) = True
			Next 
		Next 
		
		Select Case direction
			Case "N"
				For i = Y - max_range To Y - min_range
					If i >= 1 Then
						MaskData(X, i) = False
					End If
				Next 
			Case "S"
				For i = Y + min_range To Y + max_range
					If i <= MapHeight Then
						MaskData(X, i) = False
					End If
				Next 
			Case "W"
				For i = X - max_range To X - min_range
					If i >= 1 Then
						MaskData(i, Y) = False
					End If
				Next 
			Case "E"
				For i = X + min_range To X + max_range
					If i <= MapWidth Then
						MaskData(i, Y) = False
					End If
				Next 
		End Select
		MaskData(X, Y) = False
	End Sub
	
	'幅３マスの十字状のエリアを選択 (Ｍ拡の攻撃方向選択用)
	Public Sub AreaInWideCross(ByVal X As Short, ByVal Y As Short, ByVal min_range As Short, ByRef max_range As Short)
		Dim i, j As Short
		
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				MaskData(i, j) = True
			Next 
		Next 
		
		For i = Y - max_range To Y - min_range
			If i >= 1 Then
				MaskData(X, i) = False
			End If
		Next 
		For i = Y - max_range + 1 To Y - (min_range + 1)
			If i >= 1 Then
				If X > 1 Then
					MaskData(X - 1, i) = False
				End If
				If X < MapWidth Then
					MaskData(X + 1, i) = False
				End If
			End If
		Next 
		
		For i = Y + min_range To Y + max_range
			If i <= MapHeight Then
				MaskData(X, i) = False
			End If
		Next 
		For i = Y + (min_range + 1) To Y + max_range - 1
			If i <= MapHeight Then
				If X > 1 Then
					MaskData(X - 1, i) = False
				End If
				If X < MapWidth Then
					MaskData(X + 1, i) = False
				End If
			End If
		Next 
		
		For i = X - max_range To X - min_range
			If i >= 1 Then
				MaskData(i, Y) = False
			End If
		Next 
		For i = X - max_range + 1 To X - (min_range + 1)
			If i >= 1 Then
				If Y > 1 Then
					MaskData(i, Y - 1) = False
				End If
				If Y < MapHeight Then
					MaskData(i, Y + 1) = False
				End If
			End If
		Next 
		
		For i = X + min_range To X + max_range
			If i <= MapWidth Then
				MaskData(i, Y) = False
			End If
		Next 
		For i = X + (min_range + 1) To X + max_range - 1
			If i <= MapWidth Then
				If Y > 1 Then
					MaskData(i, Y - 1) = False
				End If
				If Y < MapHeight Then
					MaskData(i, Y + 1) = False
				End If
			End If
		Next 
		
		MaskData(X, Y) = False
	End Sub
	
	'幅３マスの直線状のエリアを選択 (Ｍ拡の攻撃範囲設定用)
	Public Sub AreaInCone(ByVal X As Short, ByVal Y As Short, ByVal min_range As Short, ByRef max_range As Short, ByRef direction As String)
		Dim i, j As Short
		
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				MaskData(i, j) = True
			Next 
		Next 
		
		Select Case direction
			Case "N"
				For i = Y - max_range To Y - min_range
					If i >= 1 Then
						MaskData(X, i) = False
					End If
				Next 
				For i = Y - max_range + 1 To Y - (min_range + 1)
					If i >= 1 Then
						If X > 1 Then
							MaskData(X - 1, i) = False
						End If
						If X < MapWidth Then
							MaskData(X + 1, i) = False
						End If
					End If
				Next 
				
			Case "S"
				For i = Y + min_range To Y + max_range
					If i <= MapHeight Then
						MaskData(X, i) = False
					End If
				Next 
				For i = Y + (min_range + 1) To Y + max_range - 1
					If i <= MapHeight Then
						If X > 1 Then
							MaskData(X - 1, i) = False
						End If
						If X < MapWidth Then
							MaskData(X + 1, i) = False
						End If
					End If
				Next 
				
			Case "W"
				For i = X - max_range To X - min_range
					If i >= 1 Then
						MaskData(i, Y) = False
					End If
				Next 
				For i = X - max_range + 1 To X - (min_range + 1)
					If i >= 1 Then
						If Y > 1 Then
							MaskData(i, Y - 1) = False
						End If
						If Y < MapHeight Then
							MaskData(i, Y + 1) = False
						End If
					End If
				Next 
				
			Case "E"
				For i = X + min_range To X + max_range
					If i <= MapWidth Then
						MaskData(i, Y) = False
					End If
				Next 
				For i = X + (min_range + 1) To X + max_range - 1
					If i <= MapWidth Then
						If Y > 1 Then
							MaskData(i, Y - 1) = False
						End If
						If Y < MapHeight Then
							MaskData(i, Y + 1) = False
						End If
					End If
				Next 
		End Select
		
		MaskData(X, Y) = False
	End Sub
	
	'扇状のエリアを選択 (Ｍ扇の攻撃範囲設定用)
	Public Sub AreaInSector(ByVal X As Short, ByVal Y As Short, ByVal min_range As Short, ByRef max_range As Short, ByRef direction As String, ByVal lv As Short, Optional ByVal without_refresh As Boolean = False)
		Dim xx, i, yy As Short
		
		If Not without_refresh Then
			For xx = 1 To MapWidth
				For yy = 1 To MapHeight
					MaskData(xx, yy) = True
				Next 
			Next 
		End If
		
		Select Case direction
			Case "N"
				For i = min_range To max_range
					yy = Y - i
					If yy < 1 Then
						Exit For
					End If
					Select Case lv
						Case 1
							For xx = MaxLng(X - i \ 3, 1) To MinLng(X + i \ 3, MapWidth)
								MaskData(xx, yy) = False
							Next 
						Case 2
							For xx = MaxLng(X - i \ 2, 1) To MinLng(X + i \ 2, MapWidth)
								MaskData(xx, yy) = False
							Next 
						Case 3
							For xx = MaxLng(X - (i - 1), 1) To MinLng(X + (i - 1), MapWidth)
								MaskData(xx, yy) = False
							Next 
						Case 4
							For xx = MaxLng(X - i, 1) To MinLng(X + i, MapWidth)
								MaskData(xx, yy) = False
							Next 
					End Select
				Next 
				
			Case "S"
				For i = min_range To max_range
					yy = Y + i
					If yy > MapHeight Then
						Exit For
					End If
					Select Case lv
						Case 1
							For xx = MaxLng(X - i \ 3, 1) To MinLng(X + i \ 3, MapWidth)
								MaskData(xx, yy) = False
							Next 
						Case 2
							For xx = MaxLng(X - i \ 2, 1) To MinLng(X + i \ 2, MapWidth)
								MaskData(xx, yy) = False
							Next 
						Case 3
							For xx = MaxLng(X - (i - 1), 1) To MinLng(X + (i - 1), MapWidth)
								MaskData(xx, yy) = False
							Next 
						Case 4
							For xx = MaxLng(X - i, 1) To MinLng(X + i, MapWidth)
								MaskData(xx, yy) = False
							Next 
					End Select
				Next 
				
			Case "W"
				For i = min_range To max_range
					xx = X - i
					If xx < 1 Then
						Exit For
					End If
					Select Case lv
						Case 1
							For yy = MaxLng(Y - i \ 3, 1) To MinLng(Y + i \ 3, MapHeight)
								MaskData(xx, yy) = False
							Next 
						Case 2
							For yy = MaxLng(Y - i \ 2, 1) To MinLng(Y + i \ 2, MapHeight)
								MaskData(xx, yy) = False
							Next 
						Case 3
							For yy = MaxLng(Y - (i - 1), 1) To MinLng(Y + (i - 1), MapHeight)
								MaskData(xx, yy) = False
							Next 
						Case 4
							For yy = MaxLng(Y - i, 1) To MinLng(Y + i, MapHeight)
								MaskData(xx, yy) = False
							Next 
					End Select
				Next 
				
			Case "E"
				For i = min_range To max_range
					xx = X + i
					If xx > MapWidth Then
						Exit For
					End If
					Select Case lv
						Case 1
							For yy = MaxLng(Y - i \ 3, 1) To MinLng(Y + i \ 3, MapHeight)
								MaskData(xx, yy) = False
							Next 
						Case 2
							For yy = MaxLng(Y - i \ 2, 1) To MinLng(Y + i \ 2, MapHeight)
								MaskData(xx, yy) = False
							Next 
						Case 3
							For yy = MaxLng(Y - (i - 1), 1) To MinLng(Y + (i - 1), MapHeight)
								MaskData(xx, yy) = False
							Next 
						Case 4
							For yy = MaxLng(Y - i, 1) To MinLng(Y + i, MapHeight)
								MaskData(xx, yy) = False
							Next 
					End Select
				Next 
		End Select
		
		MaskData(X, Y) = False
	End Sub
	
	'十字状の扇状のエリアを選択 (Ｍ扇の攻撃方向選択用)
	Public Sub AreaInSectorCross(ByVal X As Short, ByVal Y As Short, ByVal min_range As Short, ByRef max_range As Short, ByVal lv As Short)
		Dim xx, yy As Short
		
		For xx = 1 To MapWidth
			For yy = 1 To MapHeight
				MaskData(xx, yy) = True
			Next 
		Next 
		
		AreaInSector(X, Y, min_range, max_range, "N", lv, True)
		AreaInSector(X, Y, min_range, max_range, "S", lv, True)
		AreaInSector(X, Y, min_range, max_range, "W", lv, True)
		AreaInSector(X, Y, min_range, max_range, "E", lv, True)
	End Sub
	
	'２点間を結ぶ直線状のエリアを選択 (Ｍ線の範囲設定用)
	Public Sub AreaInPointToPoint(ByVal x1 As Short, ByVal y1 As Short, ByVal x2 As Short, ByVal y2 As Short)
		Dim xx, yy As Short
		
		'まず全領域をマスク
		For xx = 1 To MapWidth
			For yy = 1 To MapHeight
				MaskData(xx, yy) = True
			Next 
		Next 
		
		'起点のマスクを解除
		MaskData(x1, y1) = False
		
		xx = x1
		yy = y1
		If System.Math.Abs(x1 - x2) > System.Math.Abs(y1 - y2) Then
			Do 
				If x1 > x2 Then
					xx = xx - 1
				Else
					xx = xx + 1
				End If
				MaskData(xx, yy) = False
				yy = y1 + (y2 - y1) * (x1 - xx + 0#) / (x1 - x2)
				MaskData(xx, yy) = False
			Loop Until xx = x2
		Else
			Do 
				If y1 > y2 Then
					yy = yy - 1
				Else
					yy = yy + 1
				End If
				MaskData(xx, yy) = False
				xx = x1 + (x2 - x1) * (y1 - yy + 0#) / (y1 - y2)
				MaskData(xx, yy) = False
			Loop Until yy = y2
		End If
	End Sub
	
	'ユニット u の移動範囲を選択
	'ジャンプする場合は ByJump = True
	Public Sub AreaInSpeed(ByRef u As Unit, Optional ByVal ByJump As Boolean = False)
		Dim l, j, i, k, n As Short
		Dim cur_cost(51, 51) As Integer
		Dim move_cost(51, 51) As Integer
		Dim move_area As String
		Dim tmp As Integer
		Dim buf As String
		Dim is_trans_available_on_ground As Boolean
		Dim is_trans_available_in_water As Boolean
		Dim is_trans_available_on_water As Boolean
		Dim is_trans_available_in_sky As Boolean
		Dim is_trans_available_in_moon_sky As Boolean
		Dim is_adaptable_in_water As Boolean
		Dim is_adaptable_in_space As Boolean
		Dim is_swimable As Boolean
		Dim adopted_terrain() As String
		Dim allowed_terrains() As String
		Dim prohibited_terrains() As String
		Dim uspeed As Short
		Dim u2 As Unit
		Dim x1, y1 As Short
		Dim x2, y2 As Short
		Dim zarea As Short
		Dim is_zoc As Boolean
		Dim is_vzoc, is_hzoc As Boolean
		' ADD START MARGE
		Dim td As TerrainData
		Dim is_terrain_effective As Boolean
		' ADD END MARGE
		
		Dim blocked As Boolean
		With u
			'移動時に使用するエリア
			If ByJump Then
				move_area = "空中"
			Else
				move_area = .Area
			End If
			
			'移動能力の可否を調べておく
			is_trans_available_on_ground = .IsTransAvailable("陸") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("水") And .Adaption(3) <> 0
			is_trans_available_in_sky = .IsTransAvailable("空") And .Adaption(1) <> 0
			is_trans_available_in_moon_sky = (.IsTransAvailable("空") And .Adaption(1) <> 0) Or (.IsTransAvailable("宇宙") And .Adaption(4) <> 0)
			If Mid(.Data.Adaption, 3, 1) <> "-" Or .IsFeatureAvailable("水中移動") Then
				is_adaptable_in_water = True
			End If
			If Mid(.Data.Adaption, 4, 1) <> "-" Or .IsFeatureAvailable("宇宙移動") Then
				is_adaptable_in_space = True
			End If
			If .IsFeatureAvailable("水上移動") Or .IsFeatureAvailable("ホバー移動") Then
				is_trans_available_on_water = True
			End If
			If .IsFeatureAvailable("水泳") Then
				is_swimable = True
			End If
			
			'地形適応のある地形のリストを作成
			ReDim adopted_terrain(0)
			If .IsFeatureAvailable("地形適応") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "地形適応" Then
						buf = .FeatureData(i)
						If LLength(buf) = 0 Then
							ErrorMessage("ユニット「" & .Name & "」の地形適応能力に対応地形が指定されていません")
							TerminateSRC()
						End If
						n = LLength(buf)
						ReDim Preserve adopted_terrain(UBound(adopted_terrain) + n - 1)
						For j = 2 To n
							adopted_terrain(UBound(adopted_terrain) - j + 2) = LIndex(buf, j)
						Next 
					End If
				Next 
			End If
			
			'移動力
			If ByJump Then
				uspeed = .Speed + .FeatureLevel("ジャンプ")
			Else
				uspeed = .Speed
			End If
			If .IsConditionSatisfied("移動不能") Then
				uspeed = 0
			End If
			
			'移動コストは実際の２倍の値で記録されているため、移動力もそれに合わせて
			'２倍にして移動範囲を計算する
			uspeed = 2 * uspeed
			
			' ADD START MARGE
			'再移動時は最初の移動の分だけ移動力を減少させる
			If SelectedCommand = "再移動" Then
				uspeed = uspeed - SelectedUnitMoveCost
			End If
			
			If .IsConditionSatisfied("移動不能") Then
				uspeed = 0
			End If
			' ADD END MARGE
			
			'移動範囲をチェックすべき領域
			x1 = MaxLng(1, .X - uspeed)
			y1 = MaxLng(1, .Y - uspeed)
			x2 = MinLng(.X + uspeed, MapWidth)
			y2 = MinLng(.Y + uspeed, MapHeight)
			
			'移動コストとＺＯＣをリセット
			For i = 0 To MapWidth + 1
				For j = 0 To MapHeight + 1
					move_cost(i, j) = 1000000
					PointInZOC(i, j) = 0
				Next 
			Next 
			
			'各地形の移動コストを算出しておく
			Select Case move_area
				Case "空中"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "空"
									move_cost(i, j) = TerrainMoveCost(i, j)
								Case "宇宙"
									If is_adaptable_in_space Then
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
								Case Else
									move_cost(i, j) = MinLng(move_cost(i, j), 2)
							End Select
						Next 
					Next 
				Case "地上"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "陸", "屋内", "月面"
									If is_trans_available_on_ground Then
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
								Case "水"
									If is_trans_available_in_water Or is_trans_available_on_water Then
										move_cost(i, j) = 2
									ElseIf is_adaptable_in_water Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
								Case "深水"
									If is_trans_available_in_water Or is_trans_available_on_water Then
										move_cost(i, j) = 2
									ElseIf is_swimable Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
									Else
										move_cost(i, j) = 1000000
									End If
								Case "空"
									move_cost(i, j) = 1000000
								Case "宇宙"
									If is_adaptable_in_space Then
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
							End Select
						Next 
					Next 
				Case "水上"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "陸", "屋内", "月面"
									If is_trans_available_on_ground Then
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
								Case "水", "深水"
									move_cost(i, j) = 2
								Case "空"
									move_cost(i, j) = 1000000
								Case "宇宙"
									If is_adaptable_in_space Then
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
							End Select
						Next 
					Next 
				Case "水中"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "陸", "屋内", "月面"
									If is_trans_available_on_ground Then
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
								Case "水"
									If is_trans_available_in_water Then
										move_cost(i, j) = 2
									Else
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									End If
								Case "深水"
									If is_trans_available_in_water Then
										move_cost(i, j) = 2
									ElseIf is_swimable Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
									Else
										move_cost(i, j) = 1000000
									End If
								Case "空"
									move_cost(i, j) = 1000000
								Case "宇宙"
									If is_adaptable_in_space Then
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
							End Select
						Next 
					Next 
				Case "宇宙"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "宇宙"
									move_cost(i, j) = TerrainMoveCost(i, j)
									For k = 1 To UBound(adopted_terrain)
										If TerrainName(i, j) = adopted_terrain(k) Then
											move_cost(i, j) = MinLng(move_cost(i, j), 2)
											Exit For
										End If
									Next 
								Case "陸", "屋内"
									If is_trans_available_in_sky Then
										move_cost(i, j) = 2
									ElseIf is_trans_available_on_ground Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
								Case "月面"
									If is_trans_available_in_moon_sky Then
										move_cost(i, j) = 2
									ElseIf is_trans_available_on_ground Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
								Case "水"
									If is_trans_available_in_water Or is_trans_available_on_water Then
										move_cost(i, j) = 2
									ElseIf is_adaptable_in_water Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
								Case "深水"
									If is_trans_available_in_water Or is_trans_available_on_water Then
										move_cost(i, j) = 2
									ElseIf is_swimable Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
									Else
										move_cost(i, j) = 1000000
									End If
								Case "空"
									If is_trans_available_in_sky Then
										move_cost(i, j) = TerrainMoveCost(i, j)
										For k = 1 To UBound(adopted_terrain)
											If TerrainName(i, j) = adopted_terrain(k) Then
												move_cost(i, j) = MinLng(move_cost(i, j), 2)
												Exit For
											End If
										Next 
									Else
										move_cost(i, j) = 1000000
									End If
							End Select
						Next 
					Next 
				Case "地中"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "陸", "月面"
									move_cost(i, j) = 2
								Case Else
									move_cost(i, j) = 1000000
							End Select
						Next 
					Next 
			End Select
			
			'線路移動
			If .IsFeatureAvailable("線路移動") Then
				If .Area = "地上" And Not ByJump Then
					For i = x1 To x2
						For j = y1 To y2
							If TerrainName(i, j) = "線路" Then
								move_cost(i, j) = 2
							Else
								move_cost(i, j) = 1000000
							End If
						Next 
					Next 
				End If
			End If
			
			'移動制限
			ReDim allowed_terrains(0)
			If .IsFeatureAvailable("移動制限") Then
				If .Area <> "空中" And .Area <> "地中" Then
					n = LLength(.FeatureData("移動制限"))
					ReDim allowed_terrains(n)
					For i = 2 To n
						allowed_terrains(i) = LIndex(.FeatureData("移動制限"), i)
					Next 
					If Not ByJump Then
						For i = x1 To x2
							For j = y1 To y2
								For k = 2 To n
									If TerrainName(i, j) = allowed_terrains(k) Then
										Exit For
									End If
								Next 
								If k > n Then
									move_cost(i, j) = 1000000
								End If
							Next 
						Next 
					End If
				End If
			End If
			
			'進入不可
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("進入不可") Then
				If .Area <> "空中" And .Area <> "地中" Then
					n = LLength(.FeatureData("進入不可"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("進入不可"), i)
					Next 
					If Not ByJump Then
						For i = x1 To x2
							For j = y1 To y2
								For k = 2 To n
									If TerrainName(i, j) = prohibited_terrains(k) Then
										move_cost(i, j) = 1000000
										Exit For
									End If
								Next 
							Next 
						Next 
					End If
				End If
			End If
			
			'ホバー移動
			If .IsFeatureAvailable("ホバー移動") Then
				If move_area = "地上" Or move_area = "水上" Then
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainName(i, j)
								Case "砂漠", "雪原"
									move_cost(i, j) = 2
							End Select
						Next 
					Next 
				End If
			End If
			
			'透過移動
			If .IsFeatureAvailable("透過移動") Or .IsUnderSpecialPowerEffect("透過移動") Then
				For i = x1 To x2
					For j = y1 To y2
						move_cost(i, j) = 2
					Next 
				Next 
			End If
			
			'ユニットがいるため通り抜け出来ない場所をチェック
			If Not .IsFeatureAvailable("すり抜け移動") And Not .IsUnderSpecialPowerEffect("すり抜け移動") Then
				For	Each u2 In UList
					With u2
						If .Status = "出撃" Then
							
							blocked = False
							
							'敵対する場合は通り抜け不可
							If .IsEnemy(u, True) Then
								blocked = True
							End If
							
							'陣営が合わない場合も通り抜け不可
							Select Case .Party0
								Case "味方", "ＮＰＣ"
									If u.Party0 <> "味方" And u.Party0 <> "ＮＰＣ" Then
										blocked = True
									End If
								Case Else
									If .Party0 <> u.Party0 Then
										blocked = True
									End If
							End Select
							
							'通り抜けられない場合
							If blocked Then
								move_cost(.X, .Y) = 1000000
							End If
							
							'ＺＯＣ
							If blocked And Not ByJump Then
								is_zoc = False
								zarea = 0
								If .IsFeatureAvailable("ＺＯＣ") Or IsOptionDefined("ＺＯＣ") Then
									is_zoc = True
									zarea = 1
									
									'ＺＯＣ側のＺＯＣレベル
									n = .FeatureLevel("ＺＯＣ")
									If n = 1 Then n = 10000
									
									'Option「ＺＯＣ」が指定されている
									n = MaxLng(1, n)
									
									If u.IsFeatureAvailable("ＺＯＣ無効化") Then
										'移動側のＺＯＣ無効化レベル
										'レベル指定なし、またはLv1はLv10000として扱う
										l = u.FeatureLevel("ＺＯＣ無効化")
										If l = 1 Then l = 10000
										
										'移動側のＺＯＣ無効化レベルの方が高い場合、
										'ＺＯＣ不可能
										If l >= n Then
											is_zoc = False
										End If
									End If
									
									'隣接するユニットが「隣接ユニットＺＯＣ無効化」を持っている場合
									If is_zoc Then
										For i = -1 To 1
											For j = System.Math.Abs(i) - 1 To System.Math.Abs(System.Math.Abs(i) - 1)
												If (i <> 0 Or j <> 0) And ((.X + i) >= 1 And (.X + i) <= MapWidth And (.Y + j) >= 1 And (.Y + j) <= MapHeight) Then
													'隣接ユニットが存在する？
													If Not MapDataForUnit(.X + i, .Y + j) Is Nothing Then
														buf = .Party0
														With MapDataForUnit(.X + i, .Y + j)
															'敵対陣営？
															Select Case .Party0
																Case "味方", "ＮＰＣ"
																	If buf = "味方" Or buf = "ＮＰＣ" Then
																		Exit For
																	End If
																Case Else
																	If .Party0 = buf Then
																		Exit For
																	End If
															End Select
															
															l = .FeatureLevel("隣接ユニットＺＯＣ無効化")
															If l = 1 Then l = 10000
															
															'移動側のＺＯＣ無効化レベルの方が高い場合、
															'ＺＯＣ不可能
															If l >= n Then
																is_zoc = False
																Exit For
															End If
														End With
													End If
												End If
											Next 
										Next 
									End If
								End If
								
								If is_zoc Then
									'特殊能力「ＺＯＣ」が指定されているなら、そのデータの2つ目の値をＺＯＣの範囲に設定
									'2つ目の値が省略されている場合は1を設定
									'ＺＯＣLvが0以下の場合、オプション「ＺＯＣ」が指定されていても範囲を0に設定
									If LLength(.FeatureData("ＺＯＣ")) >= 2 Then
										zarea = MaxLng(CInt(LIndex(.FeatureData("ＺＯＣ"), 2)), 0)
									End If
									
									'相対距離＋ＺＯＣの範囲が移動力以内のとき、ＺＯＣを設定
									If System.Math.Abs(u.X - .X) + System.Math.Abs(u.Y - .Y) - zarea <= uspeed Then
										'水平・垂直方向のみのＺＯＣかどうかを判断
										is_hzoc = False
										is_vzoc = False
										If InStr(.FeatureData("ＺＯＣ"), "直線") Then
											is_hzoc = True
											is_vzoc = True
										Else
											If InStr(.FeatureData("ＺＯＣ"), "水平") Then
												is_hzoc = True
											End If
											If InStr(.FeatureData("ＺＯＣ"), "垂直") Then
												is_vzoc = True
											End If
										End If
										
										If is_hzoc Or is_vzoc Then
											For i = (zarea * -1) To zarea
												If i = 0 Then
													If PointInZOC(.X, .Y) < 0 Then
														If n > System.Math.Abs(PointInZOC(.X, .Y)) Then
															PointInZOC(.X, .Y) = n
														End If
													Else
														PointInZOC(.X, .Y) = MaxLng(n, PointInZOC(.X, .Y))
													End If
												Else
													'水平ＺＯＣ
													If is_hzoc And (.X + i) >= 1 And (.X + i) <= MapWidth Then
														If PointInZOC(.X + i, .Y) < 0 Then
															If n > System.Math.Abs(PointInZOC(.X + i, .Y)) Then
																PointInZOC(.X + i, .Y) = n
															End If
														Else
															PointInZOC(.X + i, .Y) = MaxLng(n, PointInZOC(.X + i, .Y))
														End If
													End If
													'垂直ＺＯＣ
													If is_vzoc And (.Y + i) >= 1 And (.Y + i) <= MapHeight Then
														If PointInZOC(.X, .Y + i) < 0 Then
															If n > System.Math.Abs(PointInZOC(.X, .Y + i)) Then
																PointInZOC(.X, .Y + i) = n
															End If
														Else
															PointInZOC(.X, .Y + i) = MaxLng(n, PointInZOC(.X, .Y + i))
														End If
													End If
												End If
											Next 
										Else
											'全方位ＺＯＣ
											For i = (zarea * -1) To zarea
												For j = (System.Math.Abs(i) - zarea) To System.Math.Abs(System.Math.Abs(i) - zarea)
													If (.X + i) >= 1 And (.X + i) <= MapWidth And (.Y + j) >= 1 And (.Y + j) <= MapHeight Then
														If PointInZOC(.X + i, .Y + j) < 0 Then
															If n > System.Math.Abs(PointInZOC(.X + i, .Y + j)) Then
																PointInZOC(.X + i, .Y + j) = n
															End If
														Else
															PointInZOC(.X + i, .Y + j) = MaxLng(n, PointInZOC(.X + i, .Y + j))
														End If
													End If
												Next 
											Next 
										End If
									End If
								End If
							Else
								'「広域ＺＯＣ無効化」を所持している場合の処理
								If System.Math.Abs(u.X - .X) + System.Math.Abs(u.Y - .Y) - zarea <= uspeed Then
									'レベル指定なし、またはLv1はLv10000として扱う
									l = .FeatureLevel("広域ＺＯＣ無効化")
									If l = 1 Then l = 10000
									
									If l > 0 Then
										n = MaxLng(StrToLng(LIndex(.FeatureData("広域ＺＯＣ無効化"), 2)), 1)
										
										For i = (n * -1) To n
											For j = (System.Math.Abs(i) - n) To System.Math.Abs(System.Math.Abs(i) - n)
												If (.X + i) >= 1 And (.X + i) <= MapWidth And (.Y + j) >= 1 And (.Y + j) <= MapHeight Then
													PointInZOC(.X + i, .Y + j) = PointInZOC(.X + i, .Y + j) - l
												End If
											Next 
										Next 
									End If
								End If
							End If
						End If
					End With
				Next u2
			End If
			
			'移動停止地形はＺＯＣして扱う
			If Not ByJump Then
				With TDList
					For i = x1 To x2
						For j = y1 To y2
							'MOD START 240a
							'                        If .IsFeatureAvailable(MapData(i, j, 0), "移動停止") Then
							'                            PointInZOC(i, j) = 20000
							'                        End If
							If TerrainHasMoveStop(i, j) Then
								PointInZOC(i, j) = 20000
							End If
							'MOD  END  240a
						Next 
					Next 
				End With
			End If
			
			'マップ上の各地点に到達するのに必要な移動力を計算する
			
			'まず移動コスト計算用の配列を初期化
			For i = 0 To MapWidth + 1
				For j = 0 To MapHeight + 1
					TotalMoveCost(i, j) = 1000000
				Next 
			Next 
			
			'現在いる場所は移動する必要がないため、必要移動力が0
			TotalMoveCost(.X, .Y) = 0
			
			'必要移動力の計算
			For i = 1 To uspeed
				'現在の必要移動力を保存
				For j = MaxLng(0, .X - i - 1) To MinLng(.X + i + 1, MapWidth + 1)
					For k = MaxLng(0, .Y - i - 1) To MinLng(.Y + i + 1, MapHeight + 1)
						cur_cost(j, k) = TotalMoveCost(j, k)
					Next 
				Next 
				For j = MaxLng(1, .X - i) To MinLng(.X + i, MapWidth)
					For k = MaxLng(1, .Y - i) To MinLng(.Y + i, MapHeight)
						'隣接する地点と比較して最も低い必要移動力を求める
						tmp = cur_cost(j, k)
						If i > 1 Then
							With TDList
								tmp = MinLng(tmp, cur_cost(j - 1, k) + IIf(PointInZOC(j - 1, k) > 0, 10000, 0))
								tmp = MinLng(tmp, cur_cost(j + 1, k) + IIf(PointInZOC(j + 1, k) > 0, 10000, 0))
								tmp = MinLng(tmp, cur_cost(j, k - 1) + IIf(PointInZOC(j, k - 1) > 0, 10000, 0))
								tmp = MinLng(tmp, cur_cost(j, k + 1) + IIf(PointInZOC(j, k + 1) > 0, 10000, 0))
							End With
						Else
							tmp = MinLng(tmp, cur_cost(j - 1, k))
							tmp = MinLng(tmp, cur_cost(j + 1, k))
							tmp = MinLng(tmp, cur_cost(j, k - 1))
							tmp = MinLng(tmp, cur_cost(j, k + 1))
						End If
						'地形に進入するのに必要な移動力を加算
						tmp = tmp + move_cost(j, k)
						'前回の値とどちらが低い？
						TotalMoveCost(j, k) = MinLng(tmp, cur_cost(j, k))
					Next 
				Next 
			Next 
			
			'算出された必要移動力を元に進入可能か判定
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = True
					
					'必要移動力が移動力以内？
					If TotalMoveCost(i, j) > uspeed Then
						GoTo NextLoop
					End If
					
					u2 = MapDataForUnit(i, j)
					
					'ユニットが存在？
					If u2 Is Nothing Then
						MaskData(i, j) = False
						GoTo NextLoop
					End If
					
					'合体＆着艦するのは味方のみ
					If .Party0 <> "味方" Then
						GoTo NextLoop
					End If
					
					Select Case u2.Party0
						Case "味方"
							If u2.IsFeatureAvailable("母艦") Then
								'母艦に着艦？
								If Not .IsFeatureAvailable("母艦") And u2.Area <> "地中" Then
									If Not .IsFeatureAvailable("格納不可") Then
										MaskData(i, j) = False
									End If
								End If
							ElseIf .IsFeatureAvailable("合体") And u2.IsFeatureAvailable("合体") Then 
								'２体合体？
								MaskData(i, j) = True
								For k = 1 To .CountFeature
									If .Feature(k) = "合体" And .FeatureName(k) <> "" Then
										buf = .FeatureData(k)
										If LLength(buf) = 3 And UList.IsDefined(LIndex(buf, 2)) And UList.IsDefined(LIndex(buf, 3)) Then
											With UList.Item(LIndex(buf, 2))
												If .IsConditionSatisfied("行動不能") Then
													Exit For
												End If
												If .Status = "破棄" Then
													Exit For
												End If
											End With
											If u2.Name = LIndex(buf, 3) Then
												MaskData(i, j) = False
												Exit For
											ElseIf u2.Name = UList.Item(LIndex(buf, 3)).CurrentForm.Name And Not u2.IsFeatureAvailable("合体制限") Then 
												MaskData(i, j) = False
												Exit For
											End If
										End If
									End If
								Next 
							End If
						Case "ＮＰＣ"
							If .IsFeatureAvailable("合体") And u2.IsFeatureAvailable("合体") Then
								'２体合体？
								MaskData(i, j) = True
								For k = 1 To .CountFeature
									If .Feature(k) = "合体" Then
										buf = .FeatureData(k)
										If LLength(buf) = 3 And UList.IsDefined(LIndex(buf, 2)) And UList.IsDefined(LIndex(buf, 3)) Then
											With UList.Item(LIndex(buf, 2))
												If .IsConditionSatisfied("行動不能") Then
													Exit For
												End If
												If .Status = "破棄" Then
													Exit For
												End If
											End With
											If u2.Name = LIndex(buf, 3) Then
												MaskData(i, j) = False
												Exit For
											ElseIf u2.Name = UList.Item(LIndex(buf, 3)).CurrentForm.Name And Not u2.IsFeatureAvailable("合体制限") Then 
												MaskData(i, j) = False
												Exit For
											End If
										End If
									End If
								Next 
							End If
					End Select
NextLoop: 
				Next 
			Next 
			
			'ジャンプ＆透過移動先は進入可能？
			If ByJump Or .IsFeatureAvailable("透過移動") Or .IsUnderSpecialPowerEffect("透過移動") Then
				For i = x1 To x2
					For j = y1 To y2
						If MaskData(i, j) Then
							GoTo NextLoop2
						End If
						
						'ユニットがいる地形に進入出来るということは
						'合体or着艦可能ということなので地形は無視
						If Not MapDataForUnit(i, j) Is Nothing Then
							GoTo NextLoop2
						End If
						
						Select Case .Area
							Case "地上"
								Select Case TerrainClass(i, j)
									Case "空"
										MaskData(i, j) = True
									Case "水"
										If Not is_adaptable_in_water And Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "深水"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "宇宙"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "水上"
								Select Case TerrainClass(i, j)
									Case "空"
										MaskData(i, j) = True
									Case "宇宙"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "水中"
								Select Case TerrainClass(i, j)
									Case "空"
										MaskData(i, j) = True
									Case "深水"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "宇宙"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "空中"
								Select Case TerrainClass(i, j)
									Case "空"
										If TerrainMoveCost(i, j) > 100 Then
											MaskData(i, j) = True
										End If
									Case "宇宙"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "地中"
								If TerrainClass(i, j) <> "陸" Then
									MaskData(i, j) = True
								End If
							Case "宇宙"
								Select Case TerrainClass(i, j)
									Case "陸", "屋内"
										If Not is_trans_available_in_sky And Not is_trans_available_on_ground Then
											MaskData(i, j) = True
										End If
									Case "空"
										If Not is_trans_available_in_sky Or TerrainMoveCost(i, j) > 10 Then
											MaskData(i, j) = True
										End If
									Case "水"
										If Not is_trans_available_in_water And Not is_trans_available_on_water And Not is_adaptable_in_water Then
											MaskData(i, j) = True
										End If
									Case "深水"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
								End Select
						End Select
						
						'移動制限
						If UBound(allowed_terrains) > 0 Then
							For k = 2 To UBound(allowed_terrains)
								If TerrainName(i, j) = allowed_terrains(k) Then
									Exit For
								End If
							Next 
							If k > UBound(allowed_terrains) Then
								MaskData(i, j) = True
							End If
						End If
						
						'進入不可
						For k = 2 To UBound(prohibited_terrains)
							If TerrainName(i, j) = prohibited_terrains(k) Then
								MaskData(i, j) = True
								Exit For
							End If
						Next 
NextLoop2: 
					Next 
				Next 
			End If
			
			'現在いる場所は常に進入可能
			MaskData(.X, .Y) = False
		End With
	End Sub
	
	'ユニット u がテレポートして移動できる範囲を選択
	'最大距離 lv を指定可能。(省略時は移動力＋テレポートレベル)
	Public Sub AreaInTeleport(ByRef u As Unit, Optional ByVal lv As Short = 0)
		Dim is_trans_available_on_ground As Boolean
		Dim is_trans_available_in_water As Boolean
		Dim is_trans_available_on_water As Boolean
		Dim is_trans_available_in_sky As Boolean
		Dim is_adaptable_in_water As Boolean
		Dim is_adaptable_in_space As Boolean
		Dim allowed_terrains() As String
		Dim prohibited_terrains() As String
		Dim n, j, i, k, r As Short
		Dim buf As String
		Dim u2 As Unit
		
		With u
			'移動能力の可否を調べておく
			is_trans_available_on_ground = .IsTransAvailable("陸") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("水") And .Adaption(3) <> 0
			is_trans_available_in_sky = .IsTransAvailable("空") And .Adaption(1) <> 0
			If Mid(.Data.Adaption, 3, 1) <> "-" Or .IsFeatureAvailable("水中移動") Then
				is_adaptable_in_water = True
			End If
			If Mid(.Data.Adaption, 4, 1) <> "-" Or .IsFeatureAvailable("宇宙移動") Then
				is_adaptable_in_space = True
			End If
			If .IsFeatureAvailable("水上移動") Or .IsFeatureAvailable("ホバー移動") Then
				is_trans_available_on_water = True
			End If
			
			'移動制限
			ReDim allowed_terrains(0)
			If .IsFeatureAvailable("移動制限") Then
				If .Area <> "空中" And .Area <> "地中" Then
					n = LLength(.FeatureData("移動制限"))
					ReDim allowed_terrains(n)
					For i = 2 To n
						allowed_terrains(i) = LIndex(.FeatureData("移動制限"), i)
					Next 
				End If
			End If
			
			'進入不可
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("進入不可") Then
				If .Area <> "空中" And .Area <> "地中" Then
					n = LLength(.FeatureData("進入不可"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("進入不可"), i)
					Next 
				End If
			End If
			
			'テレポートによる移動距離を算出
			If lv > 0 Then
				r = lv
			Else
				r = .Speed + .FeatureLevel("テレポート")
			End If
			If .IsConditionSatisfied("移動不能") Then
				r = 0
			End If
			
			'選択解除
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = True
				Next 
			Next 
			
			'移動可能な地点を調べる
			For i = MaxLng(1, .X - r) To MinLng(MapWidth, .X + r)
				For j = MaxLng(1, .Y - r) To MinLng(MapHeight, .Y + r)
					'移動範囲内？
					If System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) > r Then
						GoTo NextLoop
					End If
					
					u2 = MapDataForUnit(i, j)
					
					If u2 Is Nothing Then
						'ユニットがいない地点は地形から進入可能かチェック
						MaskData(i, j) = False
						Select Case .Area
							Case "地上"
								Select Case TerrainClass(i, j)
									Case "空"
										MaskData(i, j) = True
									Case "水"
										If Not is_adaptable_in_water And Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "深水"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "宇宙"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "水中"
								Select Case TerrainClass(i, j)
									Case "空"
										MaskData(i, j) = True
									Case "深水"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "宇宙"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "空中"
								Select Case TerrainClass(i, j)
									Case "空"
										If TerrainMoveCost(i, j) > 100 Then
											MaskData(i, j) = True
										End If
									Case "宇宙"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "地中"
								If TerrainClass(i, j) <> "陸" Then
									MaskData(i, j) = True
								End If
							Case "宇宙"
								Select Case TerrainClass(i, j)
									Case "陸", "屋内"
										If Not is_trans_available_in_sky And Not is_trans_available_on_ground Then
											MaskData(i, j) = True
										End If
									Case "空"
										If Not is_trans_available_in_sky Or TerrainMoveCost(i, j) > 100 Then
											MaskData(i, j) = True
										End If
									Case "水"
										If Not is_trans_available_in_water And Not is_trans_available_on_water And Not is_adaptable_in_water Then
											MaskData(i, j) = True
										End If
									Case "深水"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
								End Select
						End Select
						
						'移動制限
						If UBound(allowed_terrains) > 0 Then
							For k = 2 To UBound(allowed_terrains)
								If TerrainName(i, j) = allowed_terrains(k) Then
									Exit For
								End If
							Next 
							If k > UBound(allowed_terrains) Then
								MaskData(i, j) = True
							End If
						End If
						
						'進入不可
						For k = 2 To UBound(prohibited_terrains)
							If TerrainName(i, j) = prohibited_terrains(k) Then
								MaskData(i, j) = True
								Exit For
							End If
						Next 
						
						GoTo NextLoop
					End If
					
					'合体＆着艦するのは味方のみ
					If .Party0 <> "味方" Then
						GoTo NextLoop
					End If
					
					Select Case u2.Party0
						Case "味方"
							If u2.IsFeatureAvailable("母艦") Then
								'母艦に着艦？
								If Not .IsFeatureAvailable("母艦") And Not .IsFeatureAvailable("格納不可") And u2.Area <> "地中" And Not u2.IsDisabled("母艦") Then
									MaskData(i, j) = False
								End If
							ElseIf .IsFeatureAvailable("合体") And u2.IsFeatureAvailable("合体") Then 
								'２体合体？
								MaskData(i, j) = True
								For k = 1 To .CountFeature
									If .Feature(k) = "合体" And .FeatureName(k) <> "" Then
										buf = .FeatureData(k)
										If LLength(buf) = 3 And UList.IsDefined(LIndex(buf, 2)) And UList.IsDefined(LIndex(buf, 3)) Then
											With UList.Item(LIndex(buf, 2))
												If .IsConditionSatisfied("行動不能") Then
													Exit For
												End If
												If .Status = "破棄" Then
													Exit For
												End If
											End With
											If u2.Name = LIndex(buf, 3) Then
												MaskData(i, j) = False
												Exit For
											ElseIf u2.Name = UList.Item(LIndex(buf, 3)).CurrentForm.Name And Not u2.IsFeatureAvailable("合体制限") Then 
												MaskData(i, j) = False
												Exit For
											End If
										End If
									End If
								Next 
							End If
						Case "ＮＰＣ"
							If .IsFeatureAvailable("合体") And u2.IsFeatureAvailable("合体") Then
								'２体合体？
								MaskData(i, j) = True
								For k = 1 To .CountFeature
									If .Feature(k) = "合体" Then
										buf = .FeatureData(k)
										If LLength(buf) = 3 And UList.IsDefined(LIndex(buf, 2)) And UList.IsDefined(LIndex(buf, 3)) Then
											With UList.Item(LIndex(buf, 2))
												If .IsConditionSatisfied("行動不能") Then
													Exit For
												End If
												If .Status = "破棄" Then
													Exit For
												End If
											End With
											If u2.Name = LIndex(buf, 3) Then
												MaskData(i, j) = False
												Exit For
											ElseIf u2.Name = UList.Item(LIndex(buf, 3)).CurrentForm.Name And Not u2.IsFeatureAvailable("合体制限") Then 
												MaskData(i, j) = False
												Exit For
											End If
										End If
									End If
								Next 
							End If
					End Select
NextLoop: 
				Next 
			Next 
			
			'現在いる場所は常に進入可能
			MaskData(.X, .Y) = False
		End With
	End Sub
	
	'ユニット u のＭ移武器、アビリティのターゲット座標選択用
	Public Sub AreaInMoveAction(ByRef u As Unit, ByVal max_range As Short)
		Dim k, i, j, n As Short
		' ADD START MARGE
		Dim buf As String
		' ADD END MARGE
		Dim is_trans_available_on_ground As Boolean
		Dim is_trans_available_in_water As Boolean
		Dim is_trans_available_on_water As Boolean
		Dim is_trans_available_in_sky As Boolean
		Dim is_adaptable_in_water As Boolean
		Dim is_adaptable_in_space As Boolean
		Dim is_able_to_penetrate As Boolean
		' ADD START MARGE
		Dim adopted_terrain() As String
		' ADD END MARGE
		Dim allowed_terrains() As String
		Dim prohibited_terrains() As String
		Dim x1, y1 As Short
		Dim x2, y2 As Short
		' ADD START MARGE
		Dim td As TerrainData
		' ADD END MARGE
		
		With u
			'全領域マスク
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = True
				Next 
			Next 
			
			'移動能力の可否を調べておく
			is_trans_available_on_ground = .IsTransAvailable("陸") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("水") And .Adaption(3) <> 0
			is_trans_available_in_sky = .IsTransAvailable("空") And .Adaption(1) <> 0
			If Mid(.Data.Adaption, 3, 1) <> "-" Or .IsFeatureAvailable("水中移動") Then
				is_adaptable_in_water = True
			End If
			If Mid(.Data.Adaption, 4, 1) <> "-" Or .IsFeatureAvailable("宇宙移動") Then
				is_adaptable_in_space = True
			End If
			If .IsFeatureAvailable("水上移動") Or .IsFeatureAvailable("ホバー移動") Then
				is_trans_available_on_water = True
			End If
			If .IsFeatureAvailable("透過移動") Or .IsUnderSpecialPowerEffect("透過移動") Then
				is_able_to_penetrate = True
			End If
			
			' ADD START MARGE
			'地形適応のある地形のリストを作成
			ReDim adopted_terrain(0)
			If .IsFeatureAvailable("地形適応") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "地形適応" Then
						buf = .FeatureData(i)
						If LLength(buf) = 0 Then
							ErrorMessage("ユニット「" & .Name & "」の地形適応能力に対応地形が指定されていません")
							TerminateSRC()
						End If
						n = LLength(buf)
						ReDim Preserve adopted_terrain(UBound(adopted_terrain) + n - 1)
						For j = 2 To n
							adopted_terrain(UBound(adopted_terrain) - j + 2) = LIndex(buf, j)
						Next 
					End If
				Next 
			End If
			' ADD END MARGE
			
			'移動制限
			ReDim allowed_terrains(0)
			If .IsFeatureAvailable("移動制限") Then
				If .Area <> "空中" And .Area <> "地中" Then
					n = LLength(.FeatureData("移動制限"))
					ReDim allowed_terrains(n)
					For i = 2 To n
						allowed_terrains(i) = LIndex(.FeatureData("移動制限"), i)
					Next 
				End If
			End If
			
			'進入不可
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("進入不可") Then
				If .Area <> "空中" And .Area <> "地中" Then
					n = LLength(.FeatureData("進入不可"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("進入不可"), i)
					Next 
				End If
			End If
			
			'移動範囲をチェックすべき領域
			x1 = MaxLng(1, .X - max_range)
			y1 = MaxLng(1, .Y - max_range)
			x2 = MinLng(.X + max_range, MapWidth)
			y2 = MinLng(.Y + max_range, MapHeight)
			
			'進入可能か判定
			For i = x1 To x2
				For j = y1 To y2
					'移動力の範囲内？
					If System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) > max_range Then
						GoTo NextLoop
					End If
					
					'ユニットが存在？
					If Not MapDataForUnit(i, j) Is Nothing Then
						GoTo NextLoop
					End If
					
					'適応あり？
					Select Case .Area
						Case "地上"
							Select Case TerrainClass(i, j)
								Case "空"
									GoTo NextLoop
								Case "水"
									If Not is_adaptable_in_water And Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
								Case "深水"
									If Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
								Case "宇宙"
									If Not is_adaptable_in_space Then
										GoTo NextLoop
									End If
							End Select
						Case "水中"
							Select Case TerrainClass(i, j)
								Case "空"
									GoTo NextLoop
								Case "深水"
									If Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
								Case "宇宙"
									If Not is_adaptable_in_space Then
										GoTo NextLoop
									End If
							End Select
						Case "空中"
							Select Case TerrainClass(i, j)
								Case "空"
									If TerrainMoveCost(i, j) > 100 Then
										GoTo NextLoop
									End If
								Case "宇宙"
									If Not is_adaptable_in_space Then
										GoTo NextLoop
									End If
							End Select
						Case "地中"
							If TerrainClass(i, j) <> "陸" Then
								GoTo NextLoop
							End If
						Case "宇宙"
							Select Case TerrainClass(i, j)
								Case "陸", "屋内"
									If Not is_trans_available_in_sky And Not is_trans_available_on_ground Then
										GoTo NextLoop
									End If
								Case "空"
									If Not is_trans_available_in_sky Or TerrainMoveCost(i, j) > 100 Then
										GoTo NextLoop
									End If
								Case "水"
									If Not is_trans_available_in_water And Not is_trans_available_on_water And Not is_adaptable_in_water Then
										GoTo NextLoop
									End If
								Case "深水"
									If Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
							End Select
							
							'移動制限
							If UBound(allowed_terrains) > 0 Then
								For k = 2 To UBound(allowed_terrains)
									If TerrainName(i, j) = allowed_terrains(k) Then
										Exit For
									End If
								Next 
								If k > UBound(allowed_terrains) Then
									GoTo NextLoop
								End If
							End If
							
							'進入不可
							For k = 2 To UBound(prohibited_terrains)
								If TerrainName(i, j) = prohibited_terrains(k) Then
									GoTo NextLoop
								End If
							Next 
					End Select
					
					'侵入（進入）禁止地形？
					'MOD START 240a
					'                Set td = TDList.Item(MapData(i, j, 0))
					'                With td
					'                    If .IsFeatureAvailable("侵入禁止") Then
					'                        For k = 1 To UBound(adopted_terrain)
					'                            If .Name = adopted_terrain(k) Then
					'                                Exit For
					'                            End If
					'                        Next
					'                        If k > UBound(adopted_terrain) Then
					'                            GoTo NextLoop
					'                        End If
					'                    End If
					'                End With
					If TerrainDoNotEnter(i, j) Then
						For k = 1 To UBound(adopted_terrain)
							If TerrainName(i, j) = adopted_terrain(k) Then
								Exit For
							End If
						Next 
						If k > UBound(adopted_terrain) Then
							GoTo NextLoop
						End If
					End If
					'MOD START 240a
					
					'進路上に壁がある？
					If Not is_able_to_penetrate Then
						If IsLineBlocked(.X, .Y, i, j, .Area = "空中") Then
							GoTo NextLoop
						End If
					End If
					
					'マスク解除
					MaskData(i, j) = False
NextLoop: 
				Next 
			Next 
			
			'現在いる場所は常に進入可能
			MaskData(.X, .Y) = False
		End With
	End Sub
	
	'２点間を結ぶ直線が壁でブロックされているか判定
	Public Function IsLineBlocked(ByVal x1 As Short, ByVal y1 As Short, ByVal x2 As Short, ByVal y2 As Short, Optional ByVal is_flying As Boolean = False) As Boolean
		Dim xx, yy As Short
		Dim xx2, yy2 As Short
		
		xx = x1
		yy = y1
		If System.Math.Abs(x1 - x2) > System.Math.Abs(y1 - y2) Then
			Do 
				If x1 > x2 Then
					xx = xx - 1
				Else
					xx = xx + 1
				End If
				yy2 = yy
				yy = y1 + (y2 - y1) * (x1 - xx + 0#) / (x1 - x2)
				
				'壁？
				If is_flying Then
					If TerrainName(xx, yy) = "壁" Or TerrainName(xx, yy2) = "壁" Then
						IsLineBlocked = True
						Exit Function
					End If
				Else
					Select Case TerrainName(xx, yy)
						Case "壁", "防壁"
							IsLineBlocked = True
							Exit Function
					End Select
					Select Case TerrainName(xx, yy2)
						Case "壁", "防壁"
							IsLineBlocked = True
							Exit Function
					End Select
				End If
			Loop Until xx = x2
		Else
			Do 
				If y1 > y2 Then
					yy = yy - 1
				Else
					yy = yy + 1
				End If
				xx2 = xx
				xx = x1 + (x2 - x1) * (y1 - yy + 0#) / (y1 - y2)
				
				'壁？
				If is_flying Then
					If TerrainName(xx, yy) = "壁" Or TerrainName(xx2, yy) = "壁" Then
						IsLineBlocked = True
						Exit Function
					End If
				Else
					Select Case TerrainName(xx, yy)
						Case "壁", "防壁"
							IsLineBlocked = True
							Exit Function
					End Select
					Select Case TerrainName(xx2, yy)
						Case "壁", "防壁"
							IsLineBlocked = True
							Exit Function
					End Select
				End If
			Loop Until yy = y2
		End If
		
		IsLineBlocked = False
	End Function
	
	'ユニット u が (dst_x,dst_y) に行くのに最も近い移動範囲内の場所 (X,Y) はどこか検索
	Public Sub NearestPoint(ByRef u As Unit, ByVal dst_x As Short, ByVal dst_y As Short, ByRef X As Short, ByRef Y As Short)
		Dim k, i, j, n As Short
		Dim total_cost(51, 51) As Integer
		Dim cur_speed(51, 51) As Integer
		Dim move_cost(51, 51) As Integer
		Dim tmp As Integer
		Dim is_trans_available_on_ground As Boolean
		Dim is_trans_available_in_water As Boolean
		Dim is_trans_available_on_water As Boolean
		Dim is_adaptable_in_water As Boolean
		Dim adopted_terrain() As String
		Dim allowed_terrains() As String
		Dim prohibited_terrains() As String
		Dim is_changed As Boolean
		Dim min_x, max_x As Short
		Dim min_y, max_y As Short
		
		'目的地がマップ外にならないように
		dst_x = MaxLng(MinLng(dst_x, MapWidth), 1)
		dst_y = MaxLng(MinLng(dst_y, MapHeight), 1)
		
		'移動能力の可否を調べておく
		With u
			X = .X
			Y = .Y
			
			is_trans_available_on_ground = .IsTransAvailable("陸") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("水") And .Adaption(3) <> 0
			If Mid(.Data.Adaption, 3, 1) <> "-" Or .IsFeatureAvailable("水中移動") Then
				is_adaptable_in_water = True
			End If
			If .IsFeatureAvailable("水上移動") Or .IsFeatureAvailable("ホバー移動") Then
				is_trans_available_on_water = True
			End If
			
			ReDim adopted_terrain(LLength(.FeatureData("地形適応")))
			For i = 2 To UBound(adopted_terrain)
				adopted_terrain(i) = LIndex(.FeatureData("地形適応"), i)
			Next 
		End With
		
		'各地形の移動コストを算出しておく
		Select Case u.Area
			Case "空中"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If TerrainClass(i, j) = "空" Then
							move_cost(i, j) = TerrainMoveCost(i, j)
							For k = 2 To UBound(adopted_terrain)
								If TerrainName(i, j) = adopted_terrain(k) Then
									move_cost(i, j) = MinLng(move_cost(i, j), 2)
									Exit For
								End If
							Next 
						Else
							move_cost(i, j) = 2
						End If
					Next 
				Next 
				
			Case "地上"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Select Case TerrainClass(i, j)
							Case "陸", "屋内", "月面"
								If is_trans_available_on_ground Then
									move_cost(i, j) = TerrainMoveCost(i, j)
									For k = 2 To UBound(adopted_terrain)
										If TerrainName(i, j) = adopted_terrain(k) Then
											move_cost(i, j) = MinLng(move_cost(i, j), 2)
											Exit For
										End If
									Next 
								Else
									move_cost(i, j) = 1000000
								End If
							Case "水"
								If is_trans_available_in_water Then
									move_cost(i, j) = 2
								ElseIf is_adaptable_in_water Then 
									move_cost(i, j) = TerrainMoveCost(i, j)
									For k = 2 To UBound(adopted_terrain)
										If TerrainName(i, j) = adopted_terrain(k) Then
											move_cost(i, j) = MinLng(move_cost(i, j), 2)
											Exit For
										End If
									Next 
								Else
									move_cost(i, j) = 1000000
								End If
							Case "深水"
								If is_trans_available_in_water Then
									move_cost(i, j) = 1
								Else
									move_cost(i, j) = 1000000
								End If
							Case "空"
								move_cost(i, j) = 1000000
						End Select
					Next 
				Next 
				
			Case "水上"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Select Case TerrainClass(i, j)
							Case "陸", "屋内", "月面"
								If is_trans_available_on_ground Then
									move_cost(i, j) = TerrainMoveCost(i, j)
									For k = 2 To UBound(adopted_terrain)
										If TerrainName(i, j) = adopted_terrain(k) Then
											move_cost(i, j) = MinLng(move_cost(i, j), 2)
											Exit For
										End If
									Next 
								Else
									move_cost(i, j) = 1000000
								End If
							Case "水", "深水"
								move_cost(i, j) = 2
							Case "空"
								move_cost(i, j) = 1000000
						End Select
					Next 
				Next 
				
			Case "水中"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Select Case TerrainClass(i, j)
							Case "陸", "屋内", "月面"
								If is_trans_available_on_ground Then
									move_cost(i, j) = TerrainMoveCost(i, j)
									For k = 2 To UBound(adopted_terrain)
										If TerrainName(i, j) = adopted_terrain(k) Then
											move_cost(i, j) = MinLng(move_cost(i, j), 2)
											Exit For
										End If
									Next 
								Else
									move_cost(i, j) = 1000000
								End If
							Case "水"
								If is_trans_available_in_water Then
									move_cost(i, j) = 2
								Else
									move_cost(i, j) = TerrainMoveCost(i, j)
									For k = 2 To UBound(adopted_terrain)
										If TerrainName(i, j) = adopted_terrain(k) Then
											move_cost(i, j) = MinLng(move_cost(i, j), 2)
											Exit For
										End If
									Next 
								End If
							Case "深水"
								If is_trans_available_in_water Then
									move_cost(i, j) = 1
								Else
									move_cost(i, j) = 1000000
								End If
							Case "空"
								move_cost(i, j) = 1000000
						End Select
					Next 
				Next 
				
			Case "宇宙"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						move_cost(i, j) = TerrainMoveCost(i, j)
						For k = 2 To UBound(adopted_terrain)
							If TerrainName(i, j) = adopted_terrain(k) Then
								move_cost(i, j) = MinLng(move_cost(i, j), 2)
								Exit For
							End If
						Next 
					Next 
				Next 
				
			Case "地中"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If TerrainClass(i, j) = "陸" Then
							move_cost(i, j) = 2
						Else
							move_cost(i, j) = 1000000
						End If
					Next 
				Next 
		End Select
		
		With u
			'線路移動
			If .IsFeatureAvailable("線路移動") Then
				If .Area = "地上" Then
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							If TerrainName(i, j) = "線路" Then
								move_cost(i, j) = 1
							Else
								move_cost(i, j) = 1000000
							End If
						Next 
					Next 
				End If
			End If
			
			'移動制限
			ReDim allowed_terrains(0)
			If .IsFeatureAvailable("移動制限") Then
				If .Area <> "空中" And .Area <> "地中" Then
					n = LLength(.FeatureData("移動制限"))
					ReDim allowed_terrains(n)
					For i = 2 To n
						allowed_terrains(i) = LIndex(.FeatureData("移動制限"), i)
					Next 
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							For k = 2 To n
								If TerrainName(i, j) = allowed_terrains(k) Then
									Exit For
								End If
							Next 
							If k > n Then
								move_cost(i, j) = 1000000
							End If
						Next 
					Next 
				End If
			End If
			
			'進入不可
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("進入不可") Then
				If .Area <> "空中" And .Area <> "地中" Then
					n = LLength(.FeatureData("進入不可"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("進入不可"), i)
					Next 
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							For k = 2 To n
								If TerrainName(i, j) = prohibited_terrains(k) Then
									move_cost(i, j) = 1000000
									Exit For
								End If
							Next 
						Next 
					Next 
				End If
			End If
			
			'ホバー移動
			If .IsFeatureAvailable("ホバー移動") Then
				If .Area = "地上" Or .Area = "水上" Then
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							Select Case TerrainName(i, j)
								Case "砂漠", "月面"
									move_cost(i, j) = 1
							End Select
						Next 
					Next 
				End If
			End If
			
			'ジャンプ移動
			If .IsFeatureAvailable("ジャンプ移動") Then
				If .Area = "地上" Or .Area = "水上" Or .Area = "水中" Then
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							Select Case TerrainClass(i, j)
								Case "陸", "月面"
									move_cost(i, j) = 1
							End Select
						Next 
					Next 
				End If
			End If
		End With
		
		For i = 0 To MapWidth + 1
			For j = 0 To MapHeight + 1
				total_cost(i, j) = 1000000
			Next 
		Next 
		total_cost(dst_x, dst_y) = 0
		
		'目的地から各地点に到達するのにかかる移動力を計算
		i = 0
		Do 
			i = i + 1
			
			'タイムアウト
			If i > 3 * (MapWidth + MapHeight) Then
				Exit Do
			End If
			
			is_changed = False
			For j = 0 To MapWidth + 1
				For k = 0 To MapHeight + 1
					cur_speed(j, k) = total_cost(j, k)
				Next 
			Next 
			
			min_x = MaxLng(1, dst_x - i)
			max_x = MinLng(dst_x + i, MapWidth)
			For j = min_x To max_x
				min_y = MaxLng(1, dst_y - (i - System.Math.Abs(dst_x - j)))
				max_y = MinLng(dst_y + (i - System.Math.Abs(dst_x - j)), MapHeight)
				For k = min_y To max_y
					tmp = cur_speed(j, k)
					tmp = MinLng(tmp, cur_speed(j - 1, k))
					tmp = MinLng(tmp, cur_speed(j + 1, k))
					tmp = MinLng(tmp, cur_speed(j, k - 1))
					tmp = MinLng(tmp, cur_speed(j, k + 1))
					tmp = tmp + move_cost(j, k)
					If tmp < cur_speed(j, k) Then
						is_changed = True
						total_cost(j, k) = tmp
					End If
				Next 
			Next 
			
			'最短経路を発見した
			If total_cost(X, Y) <= System.Math.Abs(dst_x - X) + System.Math.Abs(dst_y - Y) + 2 Then
				Exit Do
			End If
		Loop While is_changed
		
		'移動可能範囲内で目的地に最も近い場所を見付ける
		tmp = total_cost(X, Y)
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				If Not MaskData(i, j) Then
					If MapDataForUnit(i, j) Is Nothing Then
						If total_cost(i, j) < tmp Then
							X = i
							Y = j
							tmp = total_cost(i, j)
						ElseIf total_cost(i, j) = tmp Then 
							If System.Math.Abs(dst_x - i) ^ 2 + System.Math.Abs(dst_y - j) ^ 2 < System.Math.Abs(dst_x - X) ^ 2 + System.Math.Abs(dst_y - Y) ^ 2 Then
								X = i
								Y = j
							End If
						End If
					End If
				End If
			Next 
		Next 
	End Sub
	
	'ユニット u が敵から最も遠くなる場所(X,Y)を検索
	Public Sub SafetyPoint(ByRef u As Unit, ByRef X As Short, ByRef Y As Short)
		Dim i, j As Short
		Dim total_cost(51, 51) As Integer
		Dim cur_cost(51, 51) As Integer
		Dim tmp As Integer
		Dim t As Unit
		Dim is_changed As Boolean
		
		'作業用配列を初期化
		For i = 0 To MapWidth + 1
			For j = 0 To MapHeight + 1
				total_cost(i, j) = 1000000
			Next 
		Next 
		For	Each t In UList
			If u.IsEnemy(t) Then
				total_cost(t.X, t.Y) = 0
			End If
		Next t
		
		'各地点の敵からの距離を計算
		Do 
			is_changed = False
			
			For i = 0 To MapWidth + 1
				For j = 0 To MapHeight + 1
					cur_cost(i, j) = total_cost(i, j)
				Next 
			Next 
			
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					tmp = cur_cost(i, j)
					tmp = MinLng(cur_cost(i - 1, j) + 1, tmp)
					tmp = MinLng(cur_cost(i + 1, j) + 1, tmp)
					tmp = MinLng(cur_cost(i, j - 1) + 1, tmp)
					tmp = MinLng(cur_cost(i, j + 1) + 1, tmp)
					If tmp < cur_cost(i, j) Then
						is_changed = True
						total_cost(i, j) = tmp
					End If
				Next 
			Next 
		Loop While is_changed
		
		'移動可能範囲内で敵から最も遠い場所を見付ける
		tmp = 0
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				If Not MaskData(i, j) Then
					If MapDataForUnit(i, j) Is Nothing Then
						If total_cost(i, j) > tmp Then
							X = i
							Y = j
							tmp = total_cost(i, j)
						ElseIf total_cost(i, j) = tmp Then 
							If System.Math.Abs(u.X - i) ^ 2 + System.Math.Abs(u.Y - j) ^ 2 < System.Math.Abs(u.X - X) ^ 2 + System.Math.Abs(u.Y - Y) ^ 2 Then
								X = i
								Y = j
							End If
						End If
					End If
				End If
			Next 
		Next 
	End Sub
	
	'現在位置から指定した場所までの移動経路を調べる
	'事前にAreaInSpeedを実行しておく事が必要
	Public Sub SearchMoveRoute(ByRef tx As Short, ByRef ty As Short, ByRef move_route_x() As Short, ByRef move_route_y() As Short)
		Dim xx, yy As Short
		Dim nx, ny As Short
		Dim ox, oy As Short
		Dim tmp As Integer
		Dim i As Short
		Dim direction, prev_direction As String
		Dim move_direction() As Object
		
		ReDim move_route_x(1)
		ReDim move_route_y(1)
		ReDim move_direction(1)
		move_route_x(1) = tx
		move_route_y(1) = ty
		
		'現在位置を調べる
		For xx = 1 To MapWidth
			For yy = 1 To MapHeight
				If TotalMoveCost(xx, yy) = 0 Then
					ox = xx
					oy = yy
				End If
			Next 
		Next 
		
		'現在位置のＺＯＣは無効化する
		PointInZOC(ox, oy) = 0
		
		xx = tx
		yy = ty
		nx = tx
		ny = ty
		
		Do While TotalMoveCost(xx, yy) > 0
			tmp = TotalMoveCost(xx, yy)
			
			'周りの場所から最も必要移動力が低い場所を探す
			
			'なるべく直線方向に移動させるため、前回と同じ移動方向を優先させる
			Select Case prev_direction
				Case "N"
					If TotalMoveCost(xx, yy - 1) < tmp And PointInZOC(xx, yy - 1) <= 0 Then
						tmp = TotalMoveCost(xx, yy - 1)
						nx = xx
						ny = yy - 1
						direction = "N"
					End If
				Case "S"
					If TotalMoveCost(xx, yy + 1) < tmp And PointInZOC(xx, yy + 1) <= 0 Then
						tmp = TotalMoveCost(xx, yy + 1)
						nx = xx
						ny = yy + 1
						direction = "S"
					End If
				Case "W"
					If TotalMoveCost(xx - 1, yy) < tmp And PointInZOC(xx - 1, yy) <= 0 Then
						tmp = TotalMoveCost(xx - 1, yy)
						nx = xx - 1
						ny = yy
						direction = "W"
					End If
				Case "E"
					If TotalMoveCost(xx + 1, yy) < tmp And PointInZOC(xx + 1, yy) <= 0 Then
						tmp = TotalMoveCost(xx + 1, yy)
						nx = xx + 1
						ny = yy
						direction = "E"
					End If
			End Select
			
			'なるべく目標位置付近で直進させるため、目標位置との距離差の小さい
			'座標軸方向に優先して移動させる
			If System.Math.Abs(xx - ox) <= System.Math.Abs(yy - oy) Then
				If TotalMoveCost(xx, yy - 1) < tmp And PointInZOC(xx, yy - 1) <= 0 Then
					tmp = TotalMoveCost(xx, yy - 1)
					nx = xx
					ny = yy - 1
					direction = "N"
				End If
				If TotalMoveCost(xx, yy + 1) < tmp And PointInZOC(xx, yy + 1) <= 0 Then
					tmp = TotalMoveCost(xx, yy + 1)
					nx = xx
					ny = yy + 1
					direction = "S"
				End If
				If TotalMoveCost(xx - 1, yy) < tmp And PointInZOC(xx - 1, yy) <= 0 Then
					tmp = TotalMoveCost(xx - 1, yy)
					nx = xx - 1
					ny = yy
					direction = "W"
				End If
				If TotalMoveCost(xx + 1, yy) < tmp And PointInZOC(xx + 1, yy) <= 0 Then
					tmp = TotalMoveCost(xx + 1, yy)
					nx = xx + 1
					ny = yy
					direction = "E"
				End If
			Else
				If TotalMoveCost(xx - 1, yy) < tmp And PointInZOC(xx - 1, yy) <= 0 Then
					tmp = TotalMoveCost(xx - 1, yy)
					nx = xx - 1
					ny = yy
					direction = "W"
				End If
				If TotalMoveCost(xx + 1, yy) < tmp And PointInZOC(xx + 1, yy) <= 0 Then
					tmp = TotalMoveCost(xx + 1, yy)
					nx = xx + 1
					ny = yy
					direction = "E"
				End If
				If TotalMoveCost(xx, yy - 1) < tmp And PointInZOC(xx, yy - 1) <= 0 Then
					tmp = TotalMoveCost(xx, yy - 1)
					nx = xx
					ny = yy - 1
					direction = "N"
				End If
				If TotalMoveCost(xx, yy + 1) < tmp And PointInZOC(xx, yy + 1) <= 0 Then
					tmp = TotalMoveCost(xx, yy + 1)
					nx = xx
					ny = yy + 1
					direction = "S"
				End If
			End If
			
			If nx = xx And ny = yy Then
				'これ以上必要移動力が低い場所が見つからなかったので終了
				Exit Do
			End If
			
			'見つかった場所を記録
			ReDim Preserve move_route_x(UBound(move_route_x) + 1)
			ReDim Preserve move_route_y(UBound(move_route_y) + 1)
			move_route_x(UBound(move_route_x)) = nx
			move_route_y(UBound(move_route_y)) = ny
			
			'移動方向を記録
			ReDim Preserve move_direction(UBound(move_direction) + 1)
			'UPGRADE_WARNING: オブジェクト move_direction(UBound()) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			move_direction(UBound(move_direction)) = direction
			prev_direction = direction
			
			'次回は今回見つかった場所を起点に検索する
			xx = nx
			yy = ny
		Loop 
		
		'直線を走った距離を計算
		MovedUnitSpeed = 1
		For i = 2 To UBound(move_direction) - 1
			'UPGRADE_WARNING: オブジェクト move_direction(i + 1) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			'UPGRADE_WARNING: オブジェクト move_direction(i) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			If move_direction(i) <> move_direction(i + 1) Then
				Exit For
			End If
			MovedUnitSpeed = MovedUnitSpeed + 1
		Next 
	End Sub
End Module