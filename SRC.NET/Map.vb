Option Strict Off
Option Explicit On
Module Map
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	Public Const MAX_TERRAIN_DATA_NUM As Short = 2000
	
	'ADD START 240a
	'Invalid_string_refer_to_original_code
	Public Const NO_LAYER_NUM As Short = 10000
	'ADD  END  240a
	
	'Invalid_string_refer_to_original_code
	Public MapFileName As String
	'Invalid_string_refer_to_original_code
	Public MapWidth As Short
	'Invalid_string_refer_to_original_code
	Public MapHeight As Short
	
	'Invalid_string_refer_to_original_code
	Public MapDrawMode As String
	'繝輔ぅ繝ｫ繧ｿ濶ｲ
	Public MapDrawFilterColor As Integer
	'繝輔ぅ繝ｫ繧ｿ縺ｮ騾城℃蠎ｦ
	Public MapDrawFilterTransPercent As Double
	'Invalid_string_refer_to_original_code
	Public MapDrawIsMapOnly As Boolean
	
	'Invalid_string_refer_to_original_code
	Public IsMapDirty As Boolean
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'ADD START 240a
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Enum MapImageFileType
		OldMapImageFileType 'Invalid_string_refer_to_original_code
		FourFiguresMapImageFileType 'Invalid_string_refer_to_original_code
		SeparateDirMapImageFileType 'Invalid_string_refer_to_original_code
	End Enum
	Public MapImageFileTypeData() As MapImageFileType
	
	'Invalid_string_refer_to_original_code
	Public MapDataForUnit() As Unit
	
	'Invalid_string_refer_to_original_code
	Public MaskData() As Boolean
	
	'Invalid_string_refer_to_original_code
	Public TotalMoveCost() As Integer
	
	'Invalid_string_refer_to_original_code
	Public PointInZOC() As Integer
	
	
	'Invalid_string_refer_to_original_code
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
	
	'(X,Y)蝨ｰ轤ｹ縺ｮ蜻ｽ荳ｭ菫ｮ豁｣
	Public Function TerrainEffectForHit(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainEffectForHit = TDList.HitMod(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				TerrainEffectForHit = TDList.HitMod(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'Invalid_string_refer_to_original_code
				TerrainEffectForHit = TDList.HitMod(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)蝨ｰ轤ｹ縺ｮ繝繝｡繝ｼ繧ｸ菫ｮ豁｣
	Public Function TerrainEffectForDamage(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'Invalid_string_refer_to_original_code
				TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TerrainEffectForHPRecover(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'Invalid_string_refer_to_original_code
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				TerrainEffectForHPRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.TerrainType), "Invalid_string_refer_to_original_code")
			Case Else
				'Invalid_string_refer_to_original_code
				TerrainEffectForHPRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.LayerType), "Invalid_string_refer_to_original_code")
		End Select
		'MOD  END  240a
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TerrainEffectForENRecover(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'Invalid_string_refer_to_original_code
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				TerrainEffectForENRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.TerrainType), "Invalid_string_refer_to_original_code")
			Case Else
				'Invalid_string_refer_to_original_code
				TerrainEffectForENRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.LayerType), "Invalid_string_refer_to_original_code")
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)蝨ｰ轤ｹ縺ｮ蝨ｰ蠖｢蜷咲ｧｰ
	Public Function TerrainName(ByVal X As Short, ByVal Y As Short) As String
		'MOD START 240a
		'    TerrainName = TDList.Name(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				TerrainName = TDList.Name(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'Invalid_string_refer_to_original_code
				TerrainName = TDList.Name(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)蝨ｰ轤ｹ縺ｮ蝨ｰ蠖｢繧ｯ繝ｩ繧ｹ
	Public Function TerrainClass(ByVal X As Short, ByVal Y As Short) As String
		'MOD START 240a
		'    TerrainClass = TDList.Class(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				TerrainClass = TDList.Class_Renamed(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'Invalid_string_refer_to_original_code
				TerrainClass = TDList.Class_Renamed(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TerrainMoveCost(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainMoveCost = TDList.MoveCost(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				TerrainMoveCost = TDList.MoveCost(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'Invalid_string_refer_to_original_code
				TerrainMoveCost = TDList.MoveCost(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TerrainHasObstacle(ByVal X As Short, ByVal Y As Short) As Boolean
		'MOD START 240a
		'Invalid_string_refer_to_original_code
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Case Else
				'Invalid_string_refer_to_original_code
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		End Select
		'MOD  END  240a
	End Function
	
	'ADD START 240a
	'Invalid_string_refer_to_original_code
	Public Function TerrainHasMoveStop(ByVal X As Short, ByVal Y As Short) As Boolean
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				TerrainHasMoveStop = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "遘ｻ蜍募●豁｢")
			Case Else
				'Invalid_string_refer_to_original_code
				TerrainHasMoveStop = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "遘ｻ蜍募●豁｢")
		End Select
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TerrainDoNotEnter(ByVal X As Short, ByVal Y As Short) As Boolean
		Dim ret As Boolean
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "騾ｲ蜈･遖∵ｭ｢")
				If Not ret Then
					'Invalid_string_refer_to_original_code
					ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "萓ｵ蜈･遖∵ｭ｢")
				End If
			Case Else
				'Invalid_string_refer_to_original_code
				ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "騾ｲ蜈･遖∵ｭ｢")
				If Not ret Then
					'Invalid_string_refer_to_original_code
					ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "萓ｵ蜈･遖∵ｭ｢")
				End If
		End Select
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function TerrainHasFeature(ByVal X As Short, ByVal Y As Short, ByRef Feature As String) As Boolean
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'Invalid_string_refer_to_original_code
				TerrainHasFeature = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), Feature)
			Case Else
				'Invalid_string_refer_to_original_code
				TerrainHasFeature = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), Feature)
		End Select
	End Function
	'ADD  END  240a
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Function SearchTerrainImageFile(ByVal tid As Short, ByVal tbitmap As Short, ByVal tx As Short, ByVal ty As Short) As String
		Dim tbmpname As String
		Dim fname2, fname1, fname3 As String
		Static init_setup_background As Boolean
		Static scenario_map_dir_exists As Boolean
		Static extdata_map_dir_exists As Boolean
		Static extdata2_map_dir_exists As Boolean
		
		'ADD START 240a
		'Invalid_string_refer_to_original_code
		If tid = NO_LAYER_NUM Then
			Exit Function
		ElseIf tbitmap = NO_LAYER_NUM Then 
			Exit Function
		End If
		'ADD  END  240a
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		tbmpname = TDList.Bitmap(tid)
		fname1 = "\Bitmap\Map\" & tbmpname & "\" & tbmpname & VB6.Format(tbitmap, "0000") & ".bmp"
		fname2 = "\Bitmap\Map\" & tbmpname & VB6.Format(tbitmap, "0000") & ".bmp"
		fname3 = "\Bitmap\Map\" & tbmpname & VB6.Format(tbitmap) & ".bmp"
		
		'Invalid_string_refer_to_original_code
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
	
	
	'Invalid_string_refer_to_original_code
	Public Sub LoadMapData(ByRef fname As String)
		Dim FileNumber As Short
		Dim i, j As Short
		Dim buf As String
		
		'Invalid_string_refer_to_original_code
		If fname = "" Or Not FileExists(fname) Then
			MapFileName = ""
			If InStr(ScenarioFileName, "Invalid_string_refer_to_original_code") > 0 Or InStr(ScenarioFileName, "繝ｩ繝ｳ繧ｭ繝ｳ繧ｰ.") > 0 Then
				SetMapSize(MainWidth, 40)
			Else
				SetMapSize(MainWidth, MainHeight)
			End If
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					'MOD START 240a
					'                MapData(i, j, 0) = MAX_TERRAIN_DATA_NUM
					'                MapData(i, j, 1) = 0
					'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input)
		
		'繝輔ぃ繧､繝ｫ蜷阪ｒ險倬鹸縺励※縺翫￥
		MapFileName = fname
		
		'Invalid_string_refer_to_original_code
		Input(FileNumber, buf)
		If buf <> "MapData" Then
			'Invalid_string_refer_to_original_code
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
		
		'繝槭ャ繝励ョ繝ｼ繧ｿ繧定ｪｭ縺ｿ霎ｼ縺ｿ
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'MOD START 240a
				'            Input #FileNumber, MapData(i, j, 0), MapData(i, j, 1)
				'            If Not TDList.IsDefined(MapData(i, j, 0)) Then
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code
				Input(FileNumber, MapData(i, j, MapDataIndex.TerrainType))
				Input(FileNumber, MapData(i, j, MapDataIndex.BitmapNo))
				If Not TDList.IsDefined(MapData(i, j, MapDataIndex.TerrainType)) Then
					MsgBox("Invalid_string_refer_to_original_code")
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					'MOD  END  240a
					FileClose(FileNumber)
					End
				End If
			Next 
		Next 
		
		'ADD START 240a
		'Invalid_string_refer_to_original_code
		If Not EOF(FileNumber) Then
			Input(FileNumber, buf)
			If buf = "Layer" Then
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Input(FileNumber, MapData(i, j, MapDataIndex.LayerType))
						Input(FileNumber, MapData(i, j, MapDataIndex.LayerBitmapNo))
						If (MapData(i, j, MapDataIndex.LayerType) <> NO_LAYER_NUM) Then
							'Invalid_string_refer_to_original_code
							If Not TDList.IsDefined(MapData(i, j, MapDataIndex.LayerType)) Then
								MsgBox("Invalid_string_refer_to_original_code")
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								FileClose(FileNumber)
								End
							End If
							'繝槭せ縺ｮ繧ｿ繧､繝励ｒ荳雁ｱ､縺ｫ
							MapData(i, j, MapDataIndex.BoxType) = BoxTypes.Upper
						Else
							'Invalid_string_refer_to_original_code
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
		ErrorMessage("Invalid_string_refer_to_original_code")
		'Invalid_string_refer_to_original_code
		'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
		FileClose(FileNumber)
		End
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Sub SetMapSize(ByVal w As Short, ByVal h As Short)
		Dim i, j As Short
		Dim ret As Integer
		
		MapWidth = w
		MapHeight = h
		MapPWidth = 32 * w
		MapPHeight = 32 * h
		MapX = (MainWidth + 1) \ 2
		MapY = (MainHeight + 1) \ 2
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
	
	'繝槭ャ繝励ョ繝ｼ繧ｿ繧偵け繝ｪ繧｢
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
		
		'繝槭ャ繝礼判髱｢繧呈ｶ亥悉
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
		
		'Invalid_string_refer_to_original_code
		If MapDrawMode <> "" And Not MapDrawIsMapOnly Then
			UList.ClearUnitBitmap()
		End If
		
		MapDrawMode = ""
		MapDrawIsMapOnly = False
		MapDrawFilterColor = 0
		MapDrawFilterTransPercent = 0
	End Sub
	
	
	'Invalid_string_refer_to_original_code
	Public Sub DumpMapData()
		Dim i, j As Short
		Dim fname As String
		
		If InStr(MapFileName, ScenarioPath) = 1 Then
			fname = Right(MapFileName, Len(MapFileName) - Len(ScenarioPath))
		Else
			fname = MapFileName
		End If
		
		
		If MapDrawIsMapOnly Then
			WriteLine(SaveDataFileNumber, fname, MapDrawMode & "Invalid_string_refer_to_original_code")
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
		'Invalid_string_refer_to_original_code
		WriteLine(SaveDataFileNumber, "Layer")
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				WriteLine(SaveDataFileNumber, MapData(i, j, MapDataIndex.LayerType), MapData(i, j, MapDataIndex.LayerBitmapNo), MapData(i, j, MapDataIndex.BoxType))
			Next 
		Next 
		'ADD  END  240a
		
	End Sub
	
	'Invalid_string_refer_to_original_code
	'MOD START 240a
	'Sub竊巽unction縺ｫ
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
		
		'Invalid_string_refer_to_original_code
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
			If sbuf2 <> MapDrawMode & "Invalid_string_refer_to_original_code" Then
				If Right(sbuf2, 7) = "Invalid_string_refer_to_original_code" Then
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
				If Right(sbuf2, 7) = "Invalid_string_refer_to_original_code" Then
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
		
		'Invalid_string_refer_to_original_code
		Input(SaveDataFileNumber, ibuf1)
		Input(SaveDataFileNumber, ibuf2)
		If ibuf1 <> MapWidth Or ibuf2 <> MapHeight Then
			SetMapSize(ibuf1, ibuf2)
		End If
		
		'Invalid_string_refer_to_original_code
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
		'Invalid_string_refer_to_original_code
		'    If is_map_changed Then
		'        IsMapDirty = True
		'    End If
		'MOV  END  240a
		
		'陦ｨ遉ｺ菴咲ｽｮ
		'Invalid_string_refer_to_original_code
		'Invalid_string_refer_to_original_code
		Input(SaveDataFileNumber, MapX)
		Input(SaveDataFileNumber, MapY)
		
		'ADD START 240a
		Input(SaveDataFileNumber, buf)
		If "Layer" = buf Then
			'Invalid_string_refer_to_original_code
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
			'Invalid_string_refer_to_original_code
			Input(SaveDataFileNumber, buf)
		End If
		RestoreMapData = buf
		'Invalid_string_refer_to_original_code
		If is_map_changed Then
			IsMapDirty = True
		End If
		'ADD  END  240a
		
		'Invalid_string_refer_to_original_code
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'UPGRADE_NOTE: オブジェクト MapDataForUnit() をガベージ コレクトするまでこのオブジェクトを破棄することはできません。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' をクリックしてください。
				MapDataForUnit(i, j) = Nothing
			Next 
		Next 
		For	Each u In UList
			With u
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				MapDataForUnit(.X, .Y) = u
				'End If
			End With
		Next u
	End Function
	
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Sub AreaInRange(ByVal X As Short, ByVal Y As Short, ByVal max_range As Short, ByVal min_range As Short, ByRef uparty As String)
		Dim x1, y1 As Short
		Dim x2, y2 As Short
		Dim i, j As Short
		Dim n As Short
		
		'驕ｸ謚樊ュ蝣ｱ繧偵け繝ｪ繧｢
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				MaskData(i, j) = True
			Next 
		Next 
		
		x1 = MaxLng(X - max_range, 1)
		x2 = MinLng(X + max_range, MapWidth)
		y1 = MaxLng(Y - max_range, 1)
		y2 = MinLng(Y + max_range, MapHeight)
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		Select Case uparty
			Case "蜻ｳ譁ｹ", "Invalid_string_refer_to_original_code"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "蜻ｳ譁ｹ" And Not MapDataForUnit(i, j).Party = "Invalid_string_refer_to_original_code" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "蜻ｳ譁ｹ縺ｮ謨ｵ", "Invalid_string_refer_to_original_code"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									'Invalid_string_refer_to_original_code_
									'And Not .IsConditionSatisfied("豺ｷ荵ｱ") _
									'Invalid_string_refer_to_original_code_
									'And Not .IsConditionSatisfied("逹｡逵") _
									'Then
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									MaskData(i, j) = True
								End With
							End If
							'End With
						End If
						'End If
					Next 
				Next 
			Case "謨ｵ"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "謨ｵ" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "謨ｵ縺ｮ謨ｵ"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If .Party = "謨ｵ" Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "Invalid_string_refer_to_original_code"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								MaskData(i, j) = True
							End If
						End If
						'End If
					Next 
				Next 
			Case "Invalid_string_refer_to_original_code"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									MaskData(i, j) = True
								End With
							End If
							'End With
						End If
						'End If
					Next 
				Next 
			Case "Invalid_string_refer_to_original_code"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								MaskData(i, j) = True
							End If
						End If
					Next 
				Next 
			Case "蜈ｨ縺ｦ", "辟｡蟾ｮ蛻･"
		End Select
		
		'Invalid_string_refer_to_original_code
		MaskData(X, Y) = False
	End Sub
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	Public Sub AreaInReachable(ByRef u As Unit, ByVal max_range As Short, ByRef uparty As String)
		Dim tmp_mask_data() As Boolean
		Dim j, i, k As Short
		
		'Invalid_string_refer_to_original_code
		AreaInSpeed(u)
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		Select Case uparty
			Case "蜻ｳ譁ｹ", "Invalid_string_refer_to_original_code"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "蜻ｳ譁ｹ" And Not MapDataForUnit(i, j).Party = "Invalid_string_refer_to_original_code" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "蜻ｳ譁ｹ縺ｮ謨ｵ", "Invalid_string_refer_to_original_code"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									'Invalid_string_refer_to_original_code_
									'Invalid_string_refer_to_original_code_
									'Then
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									MaskData(i, j) = True
								End With
							End If
							'End With
						End If
						'End If
					Next 
				Next 
			Case "謨ｵ"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "謨ｵ" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "謨ｵ縺ｮ謨ｵ"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If .Party = "謨ｵ" Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "Invalid_string_refer_to_original_code"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								'Invalid_string_refer_to_original_code
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								MaskData(i, j) = True
							End If
						End If
						'End If
					Next 
				Next 
			Case "Invalid_string_refer_to_original_code"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									MaskData(i, j) = True
								End With
							End If
							'End With
						End If
						'End If
					Next 
				Next 
			Case "Invalid_string_refer_to_original_code"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								MaskData(i, j) = True
							End If
						End If
					Next 
				Next 
			Case "蜈ｨ縺ｦ", "辟｡蟾ｮ蛻･"
		End Select
		
		'Invalid_string_refer_to_original_code
		MaskData(u.X, u.Y) = False
	End Sub
	
	'Invalid_string_refer_to_original_code
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
						Case "蜻ｳ譁ｹ"
							If .Party = "蜻ｳ譁ｹ" Or .Party = "Invalid_string_refer_to_original_code" Then
								MaskData(i, j) = False
							End If
						Case "蜻ｳ譁ｹ縺ｮ謨ｵ"
							If .Party <> "蜻ｳ譁ｹ" And .Party <> "Invalid_string_refer_to_original_code" Then
								MaskData(i, j) = False
							End If
						Case "謨ｵ"
							If .Party = "謨ｵ" Then
								MaskData(i, j) = False
							End If
						Case "謨ｵ縺ｮ謨ｵ"
							If .Party <> "謨ｵ" Then
								MaskData(i, j) = False
							End If
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							MaskData(i, j) = False
							'End If
						Case "Invalid_string_refer_to_original_code"
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							MaskData(i, j) = False
							'End If
						Case Else
							MaskData(i, j) = False
					End Select
				End With
NextLoop: 
			Next 
		Next 
	End Sub
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Sub AreaInPointToPoint(ByVal x1 As Short, ByVal y1 As Short, ByVal x2 As Short, ByVal y2 As Short)
		Dim xx, yy As Short
		
		'Invalid_string_refer_to_original_code
		For xx = 1 To MapWidth
			For yy = 1 To MapHeight
				MaskData(xx, yy) = True
			Next 
		Next 
		
		'襍ｷ轤ｹ縺ｮ繝槭せ繧ｯ繧定ｧ｣髯､
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
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
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
			'遘ｻ蜍墓凾縺ｫ菴ｿ逕ｨ縺吶ｋ繧ｨ繝ｪ繧｢
			If ByJump Then
				move_area = "遨ｺ荳ｭ"
			Else
				move_area = .Area
			End If
			
			'Invalid_string_refer_to_original_code
			is_trans_available_on_ground = .IsTransAvailable("髯ｸ") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("豌ｴ") And .Adaption(3) <> 0
			is_trans_available_in_sky = .IsTransAvailable("遨ｺ") And .Adaption(1) <> 0
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_adaptable_in_water = True
			'End If
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_adaptable_in_space = True
			'End If
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_trans_available_on_water = True
			'End If
			If .IsFeatureAvailable("豌ｴ豕ｳ") Then
				is_swimable = True
			End If
			
			'Invalid_string_refer_to_original_code
			ReDim adopted_terrain(0)
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = 1 To .CountFeature
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				buf = .FeatureData(i)
				If LLength(buf) = 0 Then
					ErrorMessage("Invalid_string_refer_to_original_code")
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					TerminateSRC()
				End If
				n = LLength(buf)
				ReDim Preserve adopted_terrain(UBound(adopted_terrain) + n - 1)
				For j = 2 To n
					adopted_terrain(UBound(adopted_terrain) - j + 2) = LIndex(buf, j)
				Next 
				'End If
			Next 
			'End If
			
			'遘ｻ蜍募鴨
			If ByJump Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Else
				uspeed = .Speed
			End If
			If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
				uspeed = 0
			End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			uspeed = 2 * uspeed
			
			' ADD START MARGE
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			uspeed = uspeed - SelectedUnitMoveCost
			'End If
			
			If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
				uspeed = 0
			End If
			' ADD END MARGE
			
			'Invalid_string_refer_to_original_code
			x1 = MaxLng(1, .X - uspeed)
			y1 = MaxLng(1, .Y - uspeed)
			x2 = MinLng(.X + uspeed, MapWidth)
			y2 = MinLng(.Y + uspeed, MapHeight)
			
			'Invalid_string_refer_to_original_code
			For i = 0 To MapWidth + 1
				For j = 0 To MapHeight + 1
					move_cost(i, j) = 1000000
					PointInZOC(i, j) = 0
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			Select Case move_area
				Case "遨ｺ荳ｭ"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "遨ｺ"
									move_cost(i, j) = TerrainMoveCost(i, j)
								Case "Invalid_string_refer_to_original_code"
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
				Case "Invalid_string_refer_to_original_code"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "髯ｸ", "Invalid_string_refer_to_original_code", "譛磯擇"
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
								Case "豌ｴ"
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
								Case "豺ｱ豌ｴ"
									If is_trans_available_in_water Or is_trans_available_on_water Then
										move_cost(i, j) = 2
									ElseIf is_swimable Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
									Else
										move_cost(i, j) = 1000000
									End If
								Case "遨ｺ"
									move_cost(i, j) = 1000000
								Case "Invalid_string_refer_to_original_code"
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
				Case "Invalid_string_refer_to_original_code"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "髯ｸ", "Invalid_string_refer_to_original_code", "譛磯擇"
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
								Case "豌ｴ", "豺ｱ豌ｴ"
									move_cost(i, j) = 2
								Case "遨ｺ"
									move_cost(i, j) = 1000000
								Case "Invalid_string_refer_to_original_code"
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
				Case "豌ｴ荳ｭ"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "髯ｸ", "Invalid_string_refer_to_original_code", "譛磯擇"
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
								Case "豌ｴ"
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
								Case "豺ｱ豌ｴ"
									If is_trans_available_in_water Then
										move_cost(i, j) = 2
									ElseIf is_swimable Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
									Else
										move_cost(i, j) = 1000000
									End If
								Case "遨ｺ"
									move_cost(i, j) = 1000000
								Case "Invalid_string_refer_to_original_code"
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
				Case "Invalid_string_refer_to_original_code"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "Invalid_string_refer_to_original_code"
									move_cost(i, j) = TerrainMoveCost(i, j)
									For k = 1 To UBound(adopted_terrain)
										If TerrainName(i, j) = adopted_terrain(k) Then
											move_cost(i, j) = MinLng(move_cost(i, j), 2)
											Exit For
										End If
									Next 
								Case "髯ｸ", "Invalid_string_refer_to_original_code"
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
								Case "譛磯擇"
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
								Case "豌ｴ"
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
								Case "豺ｱ豌ｴ"
									If is_trans_available_in_water Or is_trans_available_on_water Then
										move_cost(i, j) = 2
									ElseIf is_swimable Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
									Else
										move_cost(i, j) = 1000000
									End If
								Case "遨ｺ"
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
				Case "蝨ｰ荳ｭ"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "髯ｸ", "譛磯擇"
									move_cost(i, j) = 2
								Case Else
									move_cost(i, j) = 1000000
							End Select
						Next 
					Next 
			End Select
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = x1 To x2
				For j = y1 To y2
					If TerrainName(i, j) = "邱夊ｷｯ" Then
						move_cost(i, j) = 2
					Else
						move_cost(i, j) = 1000000
					End If
				Next 
			Next 
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			ReDim allowed_terrains(0)
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .Area <> "遨ｺ荳ｭ" And .Area <> "蝨ｰ荳ｭ" Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ReDim allowed_terrains(n)
				For i = 2 To n
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
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
			'End If
			
			'騾ｲ蜈･荳榊庄
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("騾ｲ蜈･荳榊庄") Then
				If .Area <> "遨ｺ荳ｭ" And .Area <> "蝨ｰ荳ｭ" Then
					n = LLength(.FeatureData("騾ｲ蜈･荳榊庄"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("騾ｲ蜈･荳榊庄"), i)
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
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = x1 To x2
				For j = y1 To y2
					Select Case TerrainName(i, j)
						Case "遐よｼ", "Invalid_string_refer_to_original_code"
							move_cost(i, j) = 2
					End Select
				Next 
			Next 
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = x1 To x2
				For j = y1 To y2
					move_cost(i, j) = 2
				Next 
			Next 
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For	Each u2 In UList
				With u2
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					
					blocked = False
					
					'Invalid_string_refer_to_original_code
					If .IsEnemy(u, True) Then
						blocked = True
					End If
					
					'Invalid_string_refer_to_original_code
					Select Case .Party0
						Case "蜻ｳ譁ｹ", "Invalid_string_refer_to_original_code"
							If u.Party0 <> "蜻ｳ譁ｹ" And u.Party0 <> "Invalid_string_refer_to_original_code" Then
								blocked = True
							End If
						Case Else
							If .Party0 <> u.Party0 Then
								blocked = True
							End If
					End Select
					
					'Invalid_string_refer_to_original_code
					If blocked Then
						move_cost(.X, .Y) = 1000000
					End If
					
					'Invalid_string_refer_to_original_code
					If blocked And Not ByJump Then
						is_zoc = False
						zarea = 0
						If .IsFeatureAvailable("Invalid_string_refer_to_original_code") Or IsOptionDefined("Invalid_string_refer_to_original_code") Then
							is_zoc = True
							zarea = 1
							
							'Invalid_string_refer_to_original_code
							n = .FeatureLevel("Invalid_string_refer_to_original_code")
							If n = 1 Then n = 10000
							
							'Invalid_string_refer_to_original_code
							n = MaxLng(1, n)
							
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							If l = 1 Then l = 10000
							
							'Invalid_string_refer_to_original_code
							'Invalid_string_refer_to_original_code
							If l >= n Then
								is_zoc = False
							End If
						End If
						
						'Invalid_string_refer_to_original_code
						If is_zoc Then
							For i = -1 To 1
								For j = System.Math.Abs(i) - 1 To System.Math.Abs(System.Math.Abs(i) - 1)
									If (i <> 0 Or j <> 0) And ((.X + i) >= 1 And (.X + i) <= MapWidth And (.Y + j) >= 1 And (.Y + j) <= MapHeight) Then
										'Invalid_string_refer_to_original_code
										If Not MapDataForUnit(.X + i, .Y + j) Is Nothing Then
											buf = .Party0
											With MapDataForUnit(.X + i, .Y + j)
												'Invalid_string_refer_to_original_code
												Select Case .Party0
													Case "蜻ｳ譁ｹ", "Invalid_string_refer_to_original_code"
														If buf = "蜻ｳ譁ｹ" Or buf = "Invalid_string_refer_to_original_code" Then
															Exit For
														End If
													Case Else
														If .Party0 = buf Then
															Exit For
														End If
												End Select
												
												'Invalid_string_refer_to_original_code
												'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
												If l = 1 Then l = 10000
												
												'Invalid_string_refer_to_original_code
												'Invalid_string_refer_to_original_code
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
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						If LLength(.FeatureData("Invalid_string_refer_to_original_code")) >= 2 Then
							zarea = MaxLng(CInt(LIndex(.FeatureData("Invalid_string_refer_to_original_code"), 2)), 0)
						End If
						
						'Invalid_string_refer_to_original_code
						If System.Math.Abs(u.X - .X) + System.Math.Abs(u.Y - .Y) - zarea <= uspeed Then
							'Invalid_string_refer_to_original_code
							is_hzoc = False
							is_vzoc = False
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							is_hzoc = True
							is_vzoc = True
						Else
							If InStr(.FeatureData("Invalid_string_refer_to_original_code"), "豌ｴ蟷ｳ") Then
								is_hzoc = True
							End If
							If InStr(.FeatureData("Invalid_string_refer_to_original_code"), "蝙ら峩") Then
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
									'Invalid_string_refer_to_original_code
									If is_hzoc And (.X + i) >= 1 And (.X + i) <= MapWidth Then
										If PointInZOC(.X + i, .Y) < 0 Then
											If n > System.Math.Abs(PointInZOC(.X + i, .Y)) Then
												PointInZOC(.X + i, .Y) = n
											End If
										Else
											PointInZOC(.X + i, .Y) = MaxLng(n, PointInZOC(.X + i, .Y))
										End If
									End If
									'Invalid_string_refer_to_original_code
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
							'Invalid_string_refer_to_original_code
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
					'End If
					'Invalid_string_refer_to_original_code
					If System.Math.Abs(u.X - .X) + System.Math.Abs(u.Y - .Y) - zarea <= uspeed Then
						'Invalid_string_refer_to_original_code
						'Invalid_string_refer_to_original_code
						'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
						If l = 1 Then l = 10000
						
						If l > 0 Then
							'Invalid_string_refer_to_original_code
							'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
							
							For i = (n * -1) To n
								For j = (System.Math.Abs(i) - n) To System.Math.Abs(System.Math.Abs(i) - n)
									If (.X + i) >= 1 And (.X + i) <= MapWidth And (.Y + j) >= 1 And (.Y + j) <= MapHeight Then
										PointInZOC(.X + i, .Y + j) = PointInZOC(.X + i, .Y + j) - l
									End If
								Next 
							Next 
						End If
					End If
					'End If
					'End If
				End With
			Next u2
			'End If
			
			'Invalid_string_refer_to_original_code
			If Not ByJump Then
				With TDList
					For i = x1 To x2
						For j = y1 To y2
							'MOD START 240a
							'                        If .IsFeatureAvailable(MapData(i, j, 0), "遘ｻ蜍募●豁｢") Then
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
			
			'Invalid_string_refer_to_original_code
			
			'Invalid_string_refer_to_original_code
			For i = 0 To MapWidth + 1
				For j = 0 To MapHeight + 1
					TotalMoveCost(i, j) = 1000000
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			TotalMoveCost(.X, .Y) = 0
			
			'Invalid_string_refer_to_original_code
			For i = 1 To uspeed
				'Invalid_string_refer_to_original_code
				For j = MaxLng(0, .X - i - 1) To MinLng(.X + i + 1, MapWidth + 1)
					For k = MaxLng(0, .Y - i - 1) To MinLng(.Y + i + 1, MapHeight + 1)
						cur_cost(j, k) = TotalMoveCost(j, k)
					Next 
				Next 
				For j = MaxLng(1, .X - i) To MinLng(.X + i, MapWidth)
					For k = MaxLng(1, .Y - i) To MinLng(.Y + i, MapHeight)
						'Invalid_string_refer_to_original_code
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
						'Invalid_string_refer_to_original_code
						tmp = tmp + move_cost(j, k)
						'Invalid_string_refer_to_original_code
						TotalMoveCost(j, k) = MinLng(tmp, cur_cost(j, k))
					Next 
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = True
					
					'Invalid_string_refer_to_original_code
					If TotalMoveCost(i, j) > uspeed Then
						GoTo NextLoop
					End If
					
					u2 = MapDataForUnit(i, j)
					
					'Invalid_string_refer_to_original_code
					If u2 Is Nothing Then
						MaskData(i, j) = False
						GoTo NextLoop
					End If
					
					'Invalid_string_refer_to_original_code
					If .Party0 <> "蜻ｳ譁ｹ" Then
						GoTo NextLoop
					End If
					
					Select Case u2.Party0
						Case "蜻ｳ譁ｹ"
							If u2.IsFeatureAvailable("豈崎襖") Then
								'Invalid_string_refer_to_original_code
								If Not .IsFeatureAvailable("豈崎襖") And u2.Area <> "蝨ｰ荳ｭ" Then
									If Not .IsFeatureAvailable("譬ｼ邏堺ｸ榊庄") Then
										MaskData(i, j) = False
									End If
								End If
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'Then
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code
								MaskData(i, j) = True
								For k = 1 To .CountFeature
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									buf = .FeatureData(k)
									If LLength(buf) = 3 And UList.IsDefined(LIndex(buf, 2)) And UList.IsDefined(LIndex(buf, 3)) Then
										With UList.Item(LIndex(buf, 2))
											If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
												Exit For
											End If
											'Invalid_string_refer_to_original_code
											'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
											Exit For
										End With
									End If
								Next 
							End If
					End Select
				Next 
			Next 
		End With
		If u2.Name = LIndex(buf, 3) Then
			MaskData(i, j) = False
			Exit For
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			MaskData(i, j) = False
			Exit For
		End If
		'End If
		'End If
		'Next
		'End If
		'UPGRADE_WARNING: AreaInSpeed に変換されていないステートメントがあります。ソース コードを確認してください。
		'Case "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'MaskData(i, j) = True
			''UPGRADE_WARNING: AreaInSpeed に変換されていないステートメントがあります。ソース コードを確認してください。
			'End If
			'End Select
'NextLoop: '
			'Next
			'Next
			'
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'For 'i = x1 To x2
				'For 'j = y1 To y2
					'If MaskData(i, j) Then
						'GoTo NextLoop2
					'End If
					'
					'Invalid_string_refer_to_original_code
					'Invalid_string_refer_to_original_code
					'If Not MapDataForUnit(i, j) Is Nothing Then
						'GoTo NextLoop2
					'End If
					'
					''UPGRADE_WARNING: AreaInSpeed に変換されていないステートメントがあります。ソース コードを確認してください。
					'
					'Invalid_string_refer_to_original_code
					'If UBound(allowed_terrains) > 0 Then
						'For 'k = 2 To UBound(allowed_terrains)
							'If TerrainName(i, j) = allowed_terrains(k) Then
								'Exit For
							'End If
						'Next 
						'If k > UBound(allowed_terrains) Then
							'MaskData(i, j) = True
						'End If
					'End If
					'
					'騾ｲ蜈･荳榊庄
					'For 'k = 2 To UBound(prohibited_terrains)
						'If TerrainName(i, j) = prohibited_terrains(k) Then
							'MaskData(i, j) = True
							'Exit For
						'End If
					'Next 
'NextLoop2: '
				'Next 
			'Next 
			'End If
			'
			'Invalid_string_refer_to_original_code
			''UPGRADE_WARNING: AreaInSpeed に変換されていないステートメントがあります。ソース コードを確認してください。
			'End With
	End Sub
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
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
			'Invalid_string_refer_to_original_code
			is_trans_available_on_ground = .IsTransAvailable("髯ｸ") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("豌ｴ") And .Adaption(3) <> 0
			is_trans_available_in_sky = .IsTransAvailable("遨ｺ") And .Adaption(1) <> 0
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_adaptable_in_water = True
			'End If
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_adaptable_in_space = True
			'End If
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_trans_available_on_water = True
			'End If
			
			'Invalid_string_refer_to_original_code
			ReDim allowed_terrains(0)
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .Area <> "遨ｺ荳ｭ" And .Area <> "蝨ｰ荳ｭ" Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ReDim allowed_terrains(n)
				For i = 2 To n
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Next 
			End If
			'End If
			
			'騾ｲ蜈･荳榊庄
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("騾ｲ蜈･荳榊庄") Then
				If .Area <> "遨ｺ荳ｭ" And .Area <> "蝨ｰ荳ｭ" Then
					n = LLength(.FeatureData("騾ｲ蜈･荳榊庄"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("騾ｲ蜈･荳榊庄"), i)
					Next 
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			If lv > 0 Then
				r = lv
			Else
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			End If
			If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
				r = 0
			End If
			
			'驕ｸ謚櫁ｧ｣髯､
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = True
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			For i = MaxLng(1, .X - r) To MinLng(MapWidth, .X + r)
				For j = MaxLng(1, .Y - r) To MinLng(MapHeight, .Y + r)
					'Invalid_string_refer_to_original_code
					If System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) > r Then
						GoTo NextLoop
					End If
					
					u2 = MapDataForUnit(i, j)
					
					If u2 Is Nothing Then
						'Invalid_string_refer_to_original_code
						MaskData(i, j) = False
						Select Case .Area
							Case "Invalid_string_refer_to_original_code"
								Select Case TerrainClass(i, j)
									Case "遨ｺ"
										MaskData(i, j) = True
									Case "豌ｴ"
										If Not is_adaptable_in_water And Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "豺ｱ豌ｴ"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "Invalid_string_refer_to_original_code"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "豌ｴ荳ｭ"
								Select Case TerrainClass(i, j)
									Case "遨ｺ"
										MaskData(i, j) = True
									Case "豺ｱ豌ｴ"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "Invalid_string_refer_to_original_code"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "遨ｺ荳ｭ"
								Select Case TerrainClass(i, j)
									Case "遨ｺ"
										If TerrainMoveCost(i, j) > 100 Then
											MaskData(i, j) = True
										End If
									Case "Invalid_string_refer_to_original_code"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "蝨ｰ荳ｭ"
								If TerrainClass(i, j) <> "髯ｸ" Then
									MaskData(i, j) = True
								End If
							Case "Invalid_string_refer_to_original_code"
								Select Case TerrainClass(i, j)
									Case "髯ｸ", "Invalid_string_refer_to_original_code"
										If Not is_trans_available_in_sky And Not is_trans_available_on_ground Then
											MaskData(i, j) = True
										End If
									Case "遨ｺ"
										If Not is_trans_available_in_sky Or TerrainMoveCost(i, j) > 100 Then
											MaskData(i, j) = True
										End If
									Case "豌ｴ"
										If Not is_trans_available_in_water And Not is_trans_available_on_water And Not is_adaptable_in_water Then
											MaskData(i, j) = True
										End If
									Case "豺ｱ豌ｴ"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
								End Select
						End Select
						
						'Invalid_string_refer_to_original_code
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
						
						'騾ｲ蜈･荳榊庄
						For k = 2 To UBound(prohibited_terrains)
							If TerrainName(i, j) = prohibited_terrains(k) Then
								MaskData(i, j) = True
								Exit For
							End If
						Next 
						
						GoTo NextLoop
					End If
					
					'Invalid_string_refer_to_original_code
					If .Party0 <> "蜻ｳ譁ｹ" Then
						GoTo NextLoop
					End If
					
					Select Case u2.Party0
						Case "蜻ｳ譁ｹ"
							If u2.IsFeatureAvailable("豈崎襖") Then
								'Invalid_string_refer_to_original_code
								If Not .IsFeatureAvailable("豈崎襖") And Not .IsFeatureAvailable("譬ｼ邏堺ｸ榊庄") And u2.Area <> "蝨ｰ荳ｭ" And Not u2.IsDisabled("豈崎襖") Then
									MaskData(i, j) = False
								End If
								'Invalid_string_refer_to_original_code_
								'Invalid_string_refer_to_original_code_
								'Then
								'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
								'Invalid_string_refer_to_original_code
								MaskData(i, j) = True
								For k = 1 To .CountFeature
									'Invalid_string_refer_to_original_code
									'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
									buf = .FeatureData(k)
									If LLength(buf) = 3 And UList.IsDefined(LIndex(buf, 2)) And UList.IsDefined(LIndex(buf, 3)) Then
										With UList.Item(LIndex(buf, 2))
											If .IsConditionSatisfied("Invalid_string_refer_to_original_code") Then
												Exit For
											End If
											'Invalid_string_refer_to_original_code
											'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
											Exit For
										End With
									End If
								Next 
							End If
					End Select
				Next 
			Next 
		End With
		If u2.Name = LIndex(buf, 3) Then
			MaskData(i, j) = False
			Exit For
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			MaskData(i, j) = False
			Exit For
		End If
		'End If
		'End If
		'Next
		'End If
		'UPGRADE_WARNING: AreaInTeleport に変換されていないステートメントがあります。ソース コードを確認してください。
		'Case "Invalid_string_refer_to_original_code"
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'MaskData(i, j) = True
			''UPGRADE_WARNING: AreaInTeleport に変換されていないステートメントがあります。ソース コードを確認してください。
			'End If
			'End Select
'NextLoop: '
			'Next
			'Next
			'
			'Invalid_string_refer_to_original_code
			''UPGRADE_WARNING: AreaInTeleport に変換されていないステートメントがあります。ソース コードを確認してください。
			'End With
	End Sub
	
	'Invalid_string_refer_to_original_code
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
			'蜈ｨ鬆伜沺繝槭せ繧ｯ
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = True
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			is_trans_available_on_ground = .IsTransAvailable("髯ｸ") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("豌ｴ") And .Adaption(3) <> 0
			is_trans_available_in_sky = .IsTransAvailable("遨ｺ") And .Adaption(1) <> 0
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_adaptable_in_water = True
			'End If
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_adaptable_in_space = True
			'End If
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_trans_available_on_water = True
			'End If
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_able_to_penetrate = True
			'End If
			
			' ADD START MARGE
			'Invalid_string_refer_to_original_code
			ReDim adopted_terrain(0)
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = 1 To .CountFeature
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				buf = .FeatureData(i)
				If LLength(buf) = 0 Then
					ErrorMessage("Invalid_string_refer_to_original_code")
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					TerminateSRC()
				End If
				n = LLength(buf)
				ReDim Preserve adopted_terrain(UBound(adopted_terrain) + n - 1)
				For j = 2 To n
					adopted_terrain(UBound(adopted_terrain) - j + 2) = LIndex(buf, j)
				Next 
				'End If
			Next 
			'End If
			' ADD END MARGE
			
			'Invalid_string_refer_to_original_code
			ReDim allowed_terrains(0)
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .Area <> "遨ｺ荳ｭ" And .Area <> "蝨ｰ荳ｭ" Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ReDim allowed_terrains(n)
				For i = 2 To n
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				Next 
			End If
			'End If
			
			'騾ｲ蜈･荳榊庄
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("騾ｲ蜈･荳榊庄") Then
				If .Area <> "遨ｺ荳ｭ" And .Area <> "蝨ｰ荳ｭ" Then
					n = LLength(.FeatureData("騾ｲ蜈･荳榊庄"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("騾ｲ蜈･荳榊庄"), i)
					Next 
				End If
			End If
			
			'Invalid_string_refer_to_original_code
			x1 = MaxLng(1, .X - max_range)
			y1 = MaxLng(1, .Y - max_range)
			x2 = MinLng(.X + max_range, MapWidth)
			y2 = MinLng(.Y + max_range, MapHeight)
			
			'Invalid_string_refer_to_original_code
			For i = x1 To x2
				For j = y1 To y2
					'Invalid_string_refer_to_original_code
					If System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) > max_range Then
						GoTo NextLoop
					End If
					
					'Invalid_string_refer_to_original_code
					If Not MapDataForUnit(i, j) Is Nothing Then
						GoTo NextLoop
					End If
					
					'Invalid_string_refer_to_original_code
					Select Case .Area
						Case "Invalid_string_refer_to_original_code"
							Select Case TerrainClass(i, j)
								Case "遨ｺ"
									GoTo NextLoop
								Case "豌ｴ"
									If Not is_adaptable_in_water And Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
								Case "豺ｱ豌ｴ"
									If Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
								Case "Invalid_string_refer_to_original_code"
									If Not is_adaptable_in_space Then
										GoTo NextLoop
									End If
							End Select
						Case "豌ｴ荳ｭ"
							Select Case TerrainClass(i, j)
								Case "遨ｺ"
									GoTo NextLoop
								Case "豺ｱ豌ｴ"
									If Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
								Case "Invalid_string_refer_to_original_code"
									If Not is_adaptable_in_space Then
										GoTo NextLoop
									End If
							End Select
						Case "遨ｺ荳ｭ"
							Select Case TerrainClass(i, j)
								Case "遨ｺ"
									If TerrainMoveCost(i, j) > 100 Then
										GoTo NextLoop
									End If
								Case "Invalid_string_refer_to_original_code"
									If Not is_adaptable_in_space Then
										GoTo NextLoop
									End If
							End Select
						Case "蝨ｰ荳ｭ"
							If TerrainClass(i, j) <> "髯ｸ" Then
								GoTo NextLoop
							End If
						Case "Invalid_string_refer_to_original_code"
							Select Case TerrainClass(i, j)
								Case "髯ｸ", "Invalid_string_refer_to_original_code"
									If Not is_trans_available_in_sky And Not is_trans_available_on_ground Then
										GoTo NextLoop
									End If
								Case "遨ｺ"
									If Not is_trans_available_in_sky Or TerrainMoveCost(i, j) > 100 Then
										GoTo NextLoop
									End If
								Case "豌ｴ"
									If Not is_trans_available_in_water And Not is_trans_available_on_water And Not is_adaptable_in_water Then
										GoTo NextLoop
									End If
								Case "豺ｱ豌ｴ"
									If Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
							End Select
							
							'Invalid_string_refer_to_original_code
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
							
							'騾ｲ蜈･荳榊庄
							For k = 2 To UBound(prohibited_terrains)
								If TerrainName(i, j) = prohibited_terrains(k) Then
									GoTo NextLoop
								End If
							Next 
					End Select
					
					'Invalid_string_refer_to_original_code
					'MOD START 240a
					'                Set td = TDList.Item(MapData(i, j, 0))
					'                With td
					'                    If .IsFeatureAvailable("萓ｵ蜈･遖∵ｭ｢") Then
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
					
					'Invalid_string_refer_to_original_code
					If Not is_able_to_penetrate Then
						If IsLineBlocked(.X, .Y, i, j, .Area = "遨ｺ荳ｭ") Then
							GoTo NextLoop
						End If
					End If
					
					'繝槭せ繧ｯ隗｣髯､
					MaskData(i, j) = False
NextLoop: 
				Next 
			Next 
			
			'Invalid_string_refer_to_original_code
			MaskData(.X, .Y) = False
		End With
	End Sub
	
	'Invalid_string_refer_to_original_code
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
				
				'Invalid_string_refer_to_original_code
				If is_flying Then
					'Invalid_string_refer_to_original_code_
					'Invalid_string_refer_to_original_code_
					'Then
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					IsLineBlocked = True
					Exit Function
				End If
			Loop 
		Else
			Select Case TerrainName(xx, yy)
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					IsLineBlocked = True
					Exit Function
			End Select
			Select Case TerrainName(xx, yy2)
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					IsLineBlocked = True
					Exit Function
			End Select
		End If
		'UPGRADE_WARNING: IsLineBlocked に変換されていないステートメントがあります。ソース コードを確認してください。
		'Loop
		Do 
			If y1 > y2 Then
				yy = yy - 1
			Else
				yy = yy + 1
			End If
			xx2 = xx
			xx = x1 + (x2 - x1) * (y1 - yy + 0#) / (y1 - y2)
			
			'Invalid_string_refer_to_original_code
			If is_flying Then
				'Invalid_string_refer_to_original_code_
				'Invalid_string_refer_to_original_code_
				'Then
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				IsLineBlocked = True
				Exit Function
			End If
			Select Case TerrainName(xx, yy)
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					IsLineBlocked = True
					Exit Function
			End Select
			Select Case TerrainName(xx2, yy)
				Case "Invalid_string_refer_to_original_code"
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
					IsLineBlocked = True
					Exit Function
			End Select
			'End If
		Loop Until yy = y2
		'End If
		
		IsLineBlocked = False
	End Function
	
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		dst_x = MaxLng(MinLng(dst_x, MapWidth), 1)
		dst_y = MaxLng(MinLng(dst_y, MapHeight), 1)
		
		'Invalid_string_refer_to_original_code
		With u
			X = .X
			Y = .Y
			
			is_trans_available_on_ground = .IsTransAvailable("髯ｸ") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("豌ｴ") And .Adaption(3) <> 0
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_adaptable_in_water = True
			'End If
			'Invalid_string_refer_to_original_code_
			'Invalid_string_refer_to_original_code_
			'Then
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			is_trans_available_on_water = True
			'End If
			
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = 2 To UBound(adopted_terrain)
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			Next 
		End With
		
		'Invalid_string_refer_to_original_code
		Select Case u.Area
			Case "遨ｺ荳ｭ"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If TerrainClass(i, j) = "遨ｺ" Then
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
				
			Case "Invalid_string_refer_to_original_code"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Select Case TerrainClass(i, j)
							Case "髯ｸ", "Invalid_string_refer_to_original_code", "譛磯擇"
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
							Case "豌ｴ"
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
							Case "豺ｱ豌ｴ"
								If is_trans_available_in_water Then
									move_cost(i, j) = 1
								Else
									move_cost(i, j) = 1000000
								End If
							Case "遨ｺ"
								move_cost(i, j) = 1000000
						End Select
					Next 
				Next 
				
			Case "Invalid_string_refer_to_original_code"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Select Case TerrainClass(i, j)
							Case "髯ｸ", "Invalid_string_refer_to_original_code", "譛磯擇"
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
							Case "豌ｴ", "豺ｱ豌ｴ"
								move_cost(i, j) = 2
							Case "遨ｺ"
								move_cost(i, j) = 1000000
						End Select
					Next 
				Next 
				
			Case "豌ｴ荳ｭ"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Select Case TerrainClass(i, j)
							Case "髯ｸ", "Invalid_string_refer_to_original_code", "譛磯擇"
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
							Case "豌ｴ"
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
							Case "豺ｱ豌ｴ"
								If is_trans_available_in_water Then
									move_cost(i, j) = 1
								Else
									move_cost(i, j) = 1000000
								End If
							Case "遨ｺ"
								move_cost(i, j) = 1000000
						End Select
					Next 
				Next 
				
			Case "Invalid_string_refer_to_original_code"
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
				
			Case "蝨ｰ荳ｭ"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If TerrainClass(i, j) = "髯ｸ" Then
							move_cost(i, j) = 2
						Else
							move_cost(i, j) = 1000000
						End If
					Next 
				Next 
		End Select
		
		With u
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					If TerrainName(i, j) = "邱夊ｷｯ" Then
						move_cost(i, j) = 1
					Else
						move_cost(i, j) = 1000000
					End If
				Next 
			Next 
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			ReDim allowed_terrains(0)
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			If .Area <> "遨ｺ荳ｭ" And .Area <> "蝨ｰ荳ｭ" Then
				'Invalid_string_refer_to_original_code
				'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
				ReDim allowed_terrains(n)
				For i = 2 To n
					'Invalid_string_refer_to_original_code
					'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
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
			'End If
			
			'騾ｲ蜈･荳榊庄
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("騾ｲ蜈･荳榊庄") Then
				If .Area <> "遨ｺ荳ｭ" And .Area <> "蝨ｰ荳ｭ" Then
					n = LLength(.FeatureData("騾ｲ蜈･荳榊庄"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("騾ｲ蜈･荳榊庄"), i)
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
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					Select Case TerrainName(i, j)
						Case "遐よｼ", "譛磯擇"
							move_cost(i, j) = 1
					End Select
				Next 
			Next 
			'End If
			'End If
			
			'Invalid_string_refer_to_original_code
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			'Invalid_string_refer_to_original_code
			'UPGRADE_ISSUE: 前の行を解析できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="82EBB1AE-1FCB-4FEF-9E6C-8736A316F8A7"' をクリックしてください。
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					Select Case TerrainClass(i, j)
						Case "髯ｸ", "譛磯擇"
							move_cost(i, j) = 1
					End Select
				Next 
			Next 
			'End If
			'End If
		End With
		
		For i = 0 To MapWidth + 1
			For j = 0 To MapHeight + 1
				total_cost(i, j) = 1000000
			Next 
		Next 
		total_cost(dst_x, dst_y) = 0
		
		'Invalid_string_refer_to_original_code
		i = 0
		Do 
			i = i + 1
			
			'Invalid_string_refer_to_original_code
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
			
			'Invalid_string_refer_to_original_code
			If total_cost(X, Y) <= System.Math.Abs(dst_x - X) + System.Math.Abs(dst_y - Y) + 2 Then
				Exit Do
			End If
		Loop While is_changed
		
		'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	Public Sub SafetyPoint(ByRef u As Unit, ByRef X As Short, ByRef Y As Short)
		Dim i, j As Short
		Dim total_cost(51, 51) As Integer
		Dim cur_cost(51, 51) As Integer
		Dim tmp As Integer
		Dim t As Unit
		Dim is_changed As Boolean
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
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
	
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
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
		
		'Invalid_string_refer_to_original_code
		For xx = 1 To MapWidth
			For yy = 1 To MapHeight
				If TotalMoveCost(xx, yy) = 0 Then
					ox = xx
					oy = yy
				End If
			Next 
		Next 
		
		'Invalid_string_refer_to_original_code
		PointInZOC(ox, oy) = 0
		
		xx = tx
		yy = ty
		nx = tx
		ny = ty
		
		Do While TotalMoveCost(xx, yy) > 0
			tmp = TotalMoveCost(xx, yy)
			
			'Invalid_string_refer_to_original_code
			
			'縺ｪ繧九∋縺冗峩邱壽婿蜷代↓遘ｻ蜍輔＆縺帙ｋ縺溘ａ縲∝燕蝗槭→蜷後§遘ｻ蜍墓婿蜷代ｒ蜆ｪ蜈医＆縺帙ｋ
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
			
			'Invalid_string_refer_to_original_code
			'蠎ｧ讓呵ｻｸ譁ｹ蜷代↓蜆ｪ蜈医＠縺ｦ遘ｻ蜍輔＆縺帙ｋ
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
				'Invalid_string_refer_to_original_code
				Exit Do
			End If
			
			'隕九▽縺九▲縺溷ｴ謇繧定ｨ倬鹸
			ReDim Preserve move_route_x(UBound(move_route_x) + 1)
			ReDim Preserve move_route_y(UBound(move_route_y) + 1)
			move_route_x(UBound(move_route_x)) = nx
			move_route_y(UBound(move_route_y)) = ny
			
			'遘ｻ蜍墓婿蜷代ｒ險倬鹸
			ReDim Preserve move_direction(UBound(move_direction) + 1)
			'UPGRADE_WARNING: オブジェクト move_direction(UBound()) の既定プロパティを解決できませんでした。 詳細については、'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' をクリックしてください。
			move_direction(UBound(move_direction)) = direction
			prev_direction = direction
			
			'Invalid_string_refer_to_original_code
			xx = nx
			yy = ny
		Loop 
		
		'Invalid_string_refer_to_original_code
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