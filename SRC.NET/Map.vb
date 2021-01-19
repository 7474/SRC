Option Strict Off
Option Explicit On
Module Map
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�}�b�v�f�[�^�Ɋւ���e�폈�����s�����W���[��
	
	'�Ǘ��\�Ȓn�`�f�[�^�̑���
	Public Const MAX_TERRAIN_DATA_NUM As Short = 2000
	
	'ADD START 240a
	'���C���[�����̌Œ�l
	Public Const NO_LAYER_NUM As Short = 10000
	'ADD  END  240a
	
	'�}�b�v�t�@�C����
	Public MapFileName As String
	'�}�b�v�̉��T�C�Y
	Public MapWidth As Short
	'�}�b�v�̏c�T�C�Y
	Public MapHeight As Short
	
	'�}�b�v�̕`�惂�[�h
	Public MapDrawMode As String
	'�t�B���^�F
	Public MapDrawFilterColor As Integer
	'�t�B���^�̓��ߓx
	Public MapDrawFilterTransPercent As Double
	'�t�B���^��Sepia�R�}���h�ȂǂŃ��j�b�g�̐F��ύX���邩
	Public MapDrawIsMapOnly As Boolean
	
	'�}�b�v�ɉ摜�̏������݂��Ȃ��ꂽ��
	Public IsMapDirty As Boolean
	
	'�}�b�v�f�[�^���L�^����z��
	' MapData(*,*,0)�͒n�`�̎��
	' MapData(*,*,1)�̓r�b�g�}�b�v�̔ԍ�
	'ADD START 240a
	'2�`3�̓}�b�v��w���C���[�f�[�^
	' MapData(*,*,2)�͒n�`�̎�ށB���ݒ��NO_LAYER_NUM
	' MapData(*,*,3)�̓r�b�g�}�b�v�̔ԍ��B���ݒ��NO_LAYER_NUM
	' MapData(*,*,4)�̓}�X�̃f�[�^�^�C�v�B1:���w 2:��w 3:��w�f�[�^�̂� 4:��w�����ڂ̂�
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
	
	'�}�b�v�̉摜�t�@�C���̊i�[�`��
	Enum MapImageFileType
		OldMapImageFileType '���`�� (plain0.bmp)
		FourFiguresMapImageFileType '�S���̐��l (plain0000.bmp)
		SeparateDirMapImageFileType '�f�B���N�g������ (plain\plain0000.bmp)
	End Enum
	Public MapImageFileTypeData() As MapImageFileType
	
	'�}�b�v��ɑ��݂��郆�j�b�g���L�^����z��
	Public MapDataForUnit() As Unit
	
	'�}�b�v��Ń^�[�Q�b�g��I������ۂ̃}�X�N���
	Public MaskData() As Boolean
	
	'���ݒn�_���炻�̒n�_�܂ňړ�����̂ɕK�v�Ȉړ��͂̔z��
	Public TotalMoveCost() As Integer
	
	'�e�n�_���y�n�b�̉e�����ɂ��邩�ǂ���
	Public PointInZOC() As Integer
	
	
	'�n�`���e�[�u����������
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
	
	'(X,Y)�n�_�̖����C��
	Public Function TerrainEffectForHit(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainEffectForHit = TDList.HitMod(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				TerrainEffectForHit = TDList.HitMod(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				TerrainEffectForHit = TDList.HitMod(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)�n�_�̃_���[�W�C��
	Public Function TerrainEffectForDamage(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				TerrainEffectForDamage = TDList.DamageMod(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)�n�_�̂g�o�񕜗�
	Public Function TerrainEffectForHPRecover(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainEffectForHPRecover = 10 * TDList.FeatureLevel(MapData(X, Y, 0), "�g�o��")
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				TerrainEffectForHPRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.TerrainType), "�g�o��")
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				TerrainEffectForHPRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.LayerType), "�g�o��")
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)�n�_�̂d�m�񕜗�
	Public Function TerrainEffectForENRecover(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainEffectForENRecover = 10 * TDList.FeatureLevel(MapData(X, Y, 0), "�d�m��")
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				TerrainEffectForENRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.TerrainType), "�d�m��")
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				TerrainEffectForENRecover = 10 * TDList.FeatureLevel(MapData(X, Y, MapDataIndex.LayerType), "�d�m��")
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)�n�_�̒n�`����
	Public Function TerrainName(ByVal X As Short, ByVal Y As Short) As String
		'MOD START 240a
		'    TerrainName = TDList.Name(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				TerrainName = TDList.Name(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				TerrainName = TDList.Name(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)�n�_�̒n�`�N���X
	Public Function TerrainClass(ByVal X As Short, ByVal Y As Short) As String
		'MOD START 240a
		'    TerrainClass = TDList.Class(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				TerrainClass = TDList.Class_Renamed(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				TerrainClass = TDList.Class_Renamed(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)�n�_�̈ړ��R�X�g
	Public Function TerrainMoveCost(ByVal X As Short, ByVal Y As Short) As Short
		'MOD START 240a
		'    TerrainMoveCost = TDList.MoveCost(MapData(X, Y, 0))
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				TerrainMoveCost = TDList.MoveCost(MapData(X, Y, MapDataIndex.TerrainType))
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				TerrainMoveCost = TDList.MoveCost(MapData(X, Y, MapDataIndex.LayerType))
		End Select
		'MOD  END  240a
	End Function
	
	'(X,Y)�n�_�ɏ�Q�������邩 (������΂����ɏՓ˂��邩)
	Public Function TerrainHasObstacle(ByVal X As Short, ByVal Y As Short) As Boolean
		'MOD START 240a
		'    TerrainHasObstacle = TDList.IsFeatureAvailable(MapData(X, Y, 0), "�Փ�")
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				TerrainHasObstacle = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "�Փ�")
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				TerrainHasObstacle = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "�Փ�")
		End Select
		'MOD  END  240a
	End Function
	
	'ADD START 240a
	'(X,Y)�n�_���ړ���~��
	Public Function TerrainHasMoveStop(ByVal X As Short, ByVal Y As Short) As Boolean
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				TerrainHasMoveStop = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "�ړ���~")
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				TerrainHasMoveStop = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "�ړ���~")
		End Select
	End Function
	
	'(X,Y)�n�_���i���֎~��
	Public Function TerrainDoNotEnter(ByVal X As Short, ByVal Y As Short) As Boolean
		Dim ret As Boolean
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "�i���֎~")
				If Not ret Then
					'�݊����ێ��̂��ߎc���Ă���
					ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), "�N���֎~")
				End If
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "�i���֎~")
				If Not ret Then
					'�݊����ێ��̂��ߎc���Ă���
					ret = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), "�N���֎~")
				End If
		End Select
	End Function
	
	'(X,Y)�n�_���w�肵���\�͂������Ă��邩
	Public Function TerrainHasFeature(ByVal X As Short, ByVal Y As Short, ByRef Feature As String) As Boolean
		Select Case MapData(X, Y, MapDataIndex.BoxType)
			Case BoxTypes.Under, BoxTypes.UpperBmpOnly
				'��w���C���������ꍇ�Ə�w���摜��񂵂������Ă��Ȃ��ꍇ�͉��w�̃f�[�^��Ԃ�
				TerrainHasFeature = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.TerrainType), Feature)
			Case Else
				'��w���C�������������Ă���ꍇ�Ə��̂ݎ����Ă���ꍇ�͏�w�̃f�[�^��Ԃ�
				TerrainHasFeature = TDList.IsFeatureAvailable(MapData(X, Y, MapDataIndex.LayerType), Feature)
		End Select
	End Function
	'ADD  END  240a
	
	'(X,Y)�n�_�ɂ��郆�j�b�g
	Public Function UnitAtPoint(ByVal X As Short, ByVal Y As Short) As Unit
		If X < 1 Or MapWidth < X Then
			'UPGRADE_NOTE: �I�u�W�F�N�g UnitAtPoint ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			UnitAtPoint = Nothing
			Exit Function
		End If
		If Y < 1 Or MapHeight < Y Then
			'UPGRADE_NOTE: �I�u�W�F�N�g UnitAtPoint ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			UnitAtPoint = Nothing
			Exit Function
		End If
		UnitAtPoint = MapDataForUnit(X, Y)
	End Function
	
	'�w�肵���}�b�v�摜����������
	Public Function SearchTerrainImageFile(ByVal tid As Short, ByVal tbitmap As Short, ByVal tx As Short, ByVal ty As Short) As String
		Dim tbmpname As String
		Dim fname2, fname1, fname3 As String
		Static init_setup_background As Boolean
		Static scenario_map_dir_exists As Boolean
		Static extdata_map_dir_exists As Boolean
		Static extdata2_map_dir_exists As Boolean
		
		'ADD START 240a
		'�摜�����m�肵�Ă�Ȃ珈�����Ȃ�
		If tid = NO_LAYER_NUM Then
			Exit Function
		ElseIf tbitmap = NO_LAYER_NUM Then 
			Exit Function
		End If
		'ADD  END  240a
		
		'���߂Ď��s����ۂɁA�e�t�H���_��Bitmap\Map�t�H���_�����邩�`�F�b�N
		If Not init_setup_background Then
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(ScenarioPath & "Bitmap\Map", FileAttribute.Directory)) > 0 Then
				scenario_map_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(ExtDataPath & "Bitmap\Map", FileAttribute.Directory)) > 0 Then
				extdata_map_dir_exists = True
			End If
			'UPGRADE_WARNING: Dir �ɐV�������삪�w�肳��Ă��܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="9B7D5ADD-D8FE-4819-A36C-6DEDAF088CC7"' ���N���b�N���Ă��������B
			If Len(Dir(ExtDataPath2 & "Bitmap\Map", FileAttribute.Directory)) > 0 Then
				extdata2_map_dir_exists = True
			End If
			init_setup_background = True
		End If
		
		'�}�b�v�摜�̃t�@�C�������쐬
		tbmpname = TDList.Bitmap(tid)
		fname1 = "\Bitmap\Map\" & tbmpname & "\" & tbmpname & VB6.Format(tbitmap, "0000") & ".bmp"
		fname2 = "\Bitmap\Map\" & tbmpname & VB6.Format(tbitmap, "0000") & ".bmp"
		fname3 = "\Bitmap\Map\" & tbmpname & VB6.Format(tbitmap) & ".bmp"
		
		'�r�b�g�}�b�v��T��
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
	
	
	'�}�b�v�t�@�C�� fname �̃f�[�^�����[�h
	Public Sub LoadMapData(ByRef fname As String)
		Dim FileNumber As Short
		Dim i, j As Short
		Dim buf As String
		
		'�t�@�C�������݂��Ȃ��ꍇ
		If fname = "" Or Not FileExists(fname) Then
			MapFileName = ""
			If InStr(ScenarioFileName, "�X�e�[�^�X�\��.") > 0 Or InStr(ScenarioFileName, "�����L���O.") > 0 Then
				SetMapSize(MainWidth, 40)
			Else
				SetMapSize(MainWidth, MainHeight)
			End If
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					'MOD START 240a
					'                MapData(i, j, 0) = MAX_TERRAIN_DATA_NUM
					'                MapData(i, j, 1) = 0
					'�t�@�C���������ꍇ
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
		
		'�t�@�C�����J��
		FileNumber = FreeFile
		FileOpen(FileNumber, fname, OpenMode.Input)
		
		'�t�@�C�������L�^���Ă���
		MapFileName = fname
		
		'�t�@�C���̐擪�ɂ���}�b�v�T�C�Y��������
		Input(FileNumber, buf)
		If buf <> "MapData" Then
			'���`���̃}�b�v�f�[�^
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
		
		'�}�b�v�f�[�^��ǂݍ���
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'MOD START 240a
				'            Input #FileNumber, MapData(i, j, 0), MapData(i, j, 1)
				'            If Not TDList.IsDefined(MapData(i, j, 0)) Then
				'                MsgBox "��`����Ă��Ȃ�" & Format$(MapData(i, j, 0)) _
				''                    & "�Ԃ̒n�`�f�[�^���g���Ă��܂�"
				Input(FileNumber, MapData(i, j, MapDataIndex.TerrainType))
				Input(FileNumber, MapData(i, j, MapDataIndex.BitmapNo))
				If Not TDList.IsDefined(MapData(i, j, MapDataIndex.TerrainType)) Then
					MsgBox("��`����Ă��Ȃ�" & VB6.Format(MapData(i, j, MapDataIndex.TerrainType)) & "�Ԃ̒n�`�f�[�^���g���Ă��܂�")
					'MOD  END  240a
					FileClose(FileNumber)
					End
				End If
			Next 
		Next 
		
		'ADD START 240a
		'���C���[�f�[�^�ǂݍ���
		If Not EOF(FileNumber) Then
			Input(FileNumber, buf)
			If buf = "Layer" Then
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Input(FileNumber, MapData(i, j, MapDataIndex.LayerType))
						Input(FileNumber, MapData(i, j, MapDataIndex.LayerBitmapNo))
						If (MapData(i, j, MapDataIndex.LayerType) <> NO_LAYER_NUM) Then
							'��`����Ă�����f�[�^�̑Ó����`�F�b�N
							If Not TDList.IsDefined(MapData(i, j, MapDataIndex.LayerType)) Then
								MsgBox("��`����Ă��Ȃ�" & VB6.Format(MapData(i, j, MapDataIndex.LayerType)) & "�Ԃ̒n�`�f�[�^���g���Ă��܂�")
								FileClose(FileNumber)
								End
							End If
							'�}�X�̃^�C�v����w��
							MapData(i, j, MapDataIndex.BoxType) = BoxTypes.Upper
						Else
							'�}�X�̃^�C�v�����w�Ɂi�����������w�����A�ēx�����I�ɐݒ肷��j
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
		ErrorMessage("�}�b�v�t�@�C���u" & fname & "�v�̃f�[�^���s���ł�")
		FileClose(FileNumber)
		End
	End Sub
	
	'�}�b�v�T�C�Y��ݒ�
	Public Sub SetMapSize(ByVal w As Short, ByVal h As Short)
		Dim i, j As Short
		Dim ret As Integer
		
		MapWidth = w
		MapHeight = h
		MapPWidth = 32 * w
		MapPHeight = 32 * h
		MapX = (MainWidth + 1) \ 2
		MapY = (MainHeight + 1) \ 2
		
		'�}�b�v�摜�T�C�Y������
		'UPGRADE_ISSUE: Control picBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		With MainForm.picBack
			'UPGRADE_ISSUE: Control picBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.Picture = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: Control picBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.Move(0, 0, MapPWidth, MapPHeight)
			'UPGRADE_ISSUE: Control picBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			ret = PatBlt(.hDC, 0, 0, .width, .Height, BLACKNESS)
		End With
		'UPGRADE_ISSUE: Control picMaskedBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		With MainForm.picMaskedBack
			'UPGRADE_ISSUE: Control picMaskedBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.Picture = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: Control picMaskedBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			.Move(0, 0, MapPWidth, MapPHeight)
		End With
		
		'�X�N���[���o�[�̈ړ��͈͂�����
		'UPGRADE_ISSUE: Control HScroll �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		With MainForm.HScroll
			'UPGRADE_ISSUE: Control HScroll �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			If .max <> MapWidth Then
				'UPGRADE_ISSUE: Control HScroll �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Visible = False
				'UPGRADE_ISSUE: Control HScroll �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.max = MapWidth
				'MOD START 240a
				'            If MainWidth = 15 Then
				If Not NewGUIMode Then
					'MOD  END  240a
					'UPGRADE_ISSUE: Control HScroll �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
					.Visible = True
				End If
			End If
		End With
		'UPGRADE_ISSUE: Control VScroll �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		With MainForm.VScroll
			'UPGRADE_ISSUE: Control VScroll �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			If .max <> MapHeight Then
				'UPGRADE_ISSUE: Control VScroll �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.Visible = False
				'UPGRADE_ISSUE: Control VScroll �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
				.max = MapHeight
				'MOD START 240a
				'            If MainWidth = 15 Then
				If Not NewGUIMode Then
					'MOD  END  240a
					'UPGRADE_ISSUE: Control VScroll �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
					.Visible = True
				End If
			End If
		End With
		
		'�}�b�v�f�[�^�p�z��̗̈�m��
		'MOD START 240a
		'    ReDim MapData(1 To MapWidth, 1 To MapHeight, 1)
		'UPGRADE_WARNING: �z�� MapData �̉����� 1,1,0 ���� 0,0,0 �ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' ���N���b�N���Ă��������B
		ReDim MapData(MapWidth, MapHeight, 4)
		'MOD  END  240a
		'UPGRADE_WARNING: �z�� MapDataForUnit �̉����� 1,1 ���� 0,0 �ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' ���N���b�N���Ă��������B
		ReDim MapDataForUnit(MapWidth, MapHeight)
		'UPGRADE_WARNING: �z�� MaskData �̉����� 1,1 ���� 0,0 �ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' ���N���b�N���Ă��������B
		ReDim MaskData(MapWidth, MapHeight)
		ReDim TotalMoveCost(MapWidth + 1, MapHeight + 1)
		ReDim PointInZOC(MapWidth + 1, MapHeight + 1)
		'UPGRADE_WARNING: �z�� MapImageFileTypeData �̉����� 1,1 ���� 0,0 �ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' ���N���b�N���Ă��������B
		ReDim MapImageFileTypeData(MapWidth, MapHeight)
		
		'�}�b�v�f�[�^�z��̏�����
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
				'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				MapDataForUnit(i, j) = Nothing
			Next 
		Next 
	End Sub
	
	'�}�b�v�f�[�^���N���A
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
		'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
		MapDataForUnit(1, 1) = Nothing
		
		'�}�b�v��ʂ�����
		'UPGRADE_ISSUE: Control picBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		With MainForm.picBack
			'UPGRADE_ISSUE: Control picBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			ret = PatBlt(.hDC, 0, 0, .width, .Height, BLACKNESS)
		End With
		'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		With MainForm.picMain(0)
			'UPGRADE_ISSUE: Control picMain �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			ret = PatBlt(.hDC, 0, 0, .width, .Height, BLACKNESS)
		End With
		
		'���j�b�g�摜�����Z�b�g
		If MapDrawMode <> "" And Not MapDrawIsMapOnly Then
			UList.ClearUnitBitmap()
		End If
		
		MapDrawMode = ""
		MapDrawIsMapOnly = False
		MapDrawFilterColor = 0
		MapDrawFilterTransPercent = 0
	End Sub
	
	
	'���f�p�Z�[�u�f�[�^�Ƀ}�b�v�f�[�^���Z�[�u
	Public Sub DumpMapData()
		Dim i, j As Short
		Dim fname As String
		
		If InStr(MapFileName, ScenarioPath) = 1 Then
			fname = Right(MapFileName, Len(MapFileName) - Len(ScenarioPath))
		Else
			fname = MapFileName
		End If
		
		
		If MapDrawIsMapOnly Then
			WriteLine(SaveDataFileNumber, fname, MapDrawMode & "(�}�b�v����)")
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
		'���C��������������
		WriteLine(SaveDataFileNumber, "Layer")
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				WriteLine(SaveDataFileNumber, MapData(i, j, MapDataIndex.LayerType), MapData(i, j, MapDataIndex.LayerBitmapNo), MapData(i, j, MapDataIndex.BoxType))
			Next 
		Next 
		'ADD  END  240a
		
	End Sub
	
	'���f�p�Z�[�u�f�[�^����}�b�v�f�[�^�����[�h
	'MOD START 240a
	'Sub��Function��
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
		
		'�}�b�v�t�@�C����, �}�b�v�`����@
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
			If sbuf2 <> MapDrawMode & "(�}�b�v����)" Then
				If Right(sbuf2, 7) = "(�}�b�v����)" Then
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
				If Right(sbuf2, 7) = "(�}�b�v����)" Then
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
		
		'�}�b�v��, �}�b�v����
		Input(SaveDataFileNumber, ibuf1)
		Input(SaveDataFileNumber, ibuf2)
		If ibuf1 <> MapWidth Or ibuf2 <> MapHeight Then
			SetMapSize(ibuf1, ibuf2)
		End If
		
		'�e�n�`
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
		'    '�w�i���������̕K�v�L��H
		'    If is_map_changed Then
		'        IsMapDirty = True
		'    End If
		'MOV  END  240a
		
		'�\���ʒu
		'SetupBackground��MapX,MapY�������������Ă��܂����߁A���̈ʒu��
		'�l���Q�Ƃ���K�v������B
		Input(SaveDataFileNumber, MapX)
		Input(SaveDataFileNumber, MapY)
		
		'ADD START 240a
		Input(SaveDataFileNumber, buf)
		If "Layer" = buf Then
			'�e���C��
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
			'�a�f�l�֘A����1�s�ڂ�ǂݍ���
			Input(SaveDataFileNumber, buf)
		End If
		RestoreMapData = buf
		'�w�i���������̕K�v�L��H
		If is_map_changed Then
			IsMapDirty = True
		End If
		'ADD  END  240a
		
		'���j�b�g�z�u
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				'UPGRADE_NOTE: �I�u�W�F�N�g MapDataForUnit() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
				MapDataForUnit(i, j) = Nothing
			Next 
		Next 
		For	Each u In UList
			With u
				If .Status = "�o��" Then
					MapDataForUnit(.X, .Y) = u
				End If
			End With
		Next u
	End Function
	
	
	'(X,Y)�𒆐S�Ƃ��� min_range - max_range �̃G���A��I��
	'�G���A���̃��j�b�g�� uparty �̎w���ɏ]���I��
	Public Sub AreaInRange(ByVal X As Short, ByVal Y As Short, ByVal max_range As Short, ByVal min_range As Short, ByRef uparty As String)
		Dim x1, y1 As Short
		Dim x2, y2 As Short
		Dim i, j As Short
		Dim n As Short
		
		'�I�������N���A
		For i = 1 To MapWidth
			For j = 1 To MapHeight
				MaskData(i, j) = True
			Next 
		Next 
		
		x1 = MaxLng(X - max_range, 1)
		x2 = MinLng(X + max_range, MapWidth)
		y1 = MaxLng(Y - max_range, 1)
		y2 = MinLng(Y + max_range, MapHeight)
		
		'max_range������min_range�O�̃G���A��I��
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
		
		'�G���A���̃��j�b�g��I�����邩���ꂼ�ꔻ��
		Select Case uparty
			Case "����", "�m�o�b"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "����" And Not MapDataForUnit(i, j).Party = "�m�o�b" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "�����̓G", "�m�o�b�̓G"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If (.Party = "����" Or .Party = "�m�o�b") And Not .IsConditionSatisfied("�\��") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("�߈�") And Not .IsConditionSatisfied("����") Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "�G"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "�G" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "�G�̓G"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If .Party = "�G" Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "����"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "����" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "�����̓G"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If .Party = "����" Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "���"
				For i = x1 To x2
					For j = y1 To y2
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								MaskData(i, j) = True
							End If
						End If
					Next 
				Next 
			Case "�S��", "������"
		End Select
		
		'�G���A�̒��S�͏�ɑI��
		MaskData(X, Y) = False
	End Sub
	
	'���j�b�g u ����ړ���g�p�\�Ȏ˒� max_range �̕���^�A�r���e�B���g���ꍇ�̌��ʔ͈�
	'�G���A���̃��j�b�g�� Party �̎w���ɏ]���I��
	Public Sub AreaInReachable(ByRef u As Unit, ByVal max_range As Short, ByRef uparty As String)
		Dim tmp_mask_data() As Boolean
		Dim j, i, k As Short
		
		'�܂��͈ړ��͈͂�I��
		AreaInSpeed(u)
		
		'�I��͈͂�max_range�Ԃ񂾂��g��
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
		
		'�G���A���̃��j�b�g��I�����邩���ꂼ�ꔻ��
		Select Case uparty
			Case "����", "�m�o�b"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "����" And Not MapDataForUnit(i, j).Party = "�m�o�b" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "�����̓G", "�m�o�b�̓G"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If (.Party = "����" Or .Party = "�m�o�b") And Not .IsConditionSatisfied("�\��") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("�߈�") Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "�G"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "�G" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "�G�̓G"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If .Party = "�G" Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "����"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								If Not MapDataForUnit(i, j).Party = "����" Then
									MaskData(i, j) = True
								End If
							End If
						End If
					Next 
				Next 
			Case "�����̓G"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								With MapDataForUnit(i, j)
									If .Party = "����" Then
										MaskData(i, j) = True
									End If
								End With
							End If
						End If
					Next 
				Next 
			Case "���"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If Not MaskData(i, j) Then
							If Not MapDataForUnit(i, j) Is Nothing Then
								MaskData(i, j) = True
							End If
						End If
					Next 
				Next 
			Case "�S��", "������"
		End Select
		
		'�G���A�̒��S�͏�ɑI��
		MaskData(u.X, u.Y) = False
	End Sub
	
	'�}�b�v�S��ɓn����uparty�ɑ����郆�j�b�g�����݂���ꏊ��I��
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
						Case "����"
							If .Party = "����" Or .Party = "�m�o�b" Then
								MaskData(i, j) = False
							End If
						Case "�����̓G"
							If .Party <> "����" And .Party <> "�m�o�b" Then
								MaskData(i, j) = False
							End If
						Case "�G"
							If .Party = "�G" Then
								MaskData(i, j) = False
							End If
						Case "�G�̓G"
							If .Party <> "�G" Then
								MaskData(i, j) = False
							End If
						Case "����"
							If .Party = "����" Then
								MaskData(i, j) = False
							End If
						Case "�����̓G"
							If .Party <> "����" Then
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
	
	'�\����̃G���A��I�� (�l���̍U�������I��p)
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
	
	'������̃G���A��I�� (�l���̍U���͈͐ݒ�p)
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
	
	'���R�}�X�̏\����̃G���A��I�� (�l�g�̍U�������I��p)
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
	
	'���R�}�X�̒�����̃G���A��I�� (�l�g�̍U���͈͐ݒ�p)
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
	
	'���̃G���A��I�� (�l��̍U���͈͐ݒ�p)
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
	
	'�\����̐��̃G���A��I�� (�l��̍U�������I��p)
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
	
	'�Q�_�Ԃ����Ԓ�����̃G���A��I�� (�l���͈̔͐ݒ�p)
	Public Sub AreaInPointToPoint(ByVal x1 As Short, ByVal y1 As Short, ByVal x2 As Short, ByVal y2 As Short)
		Dim xx, yy As Short
		
		'�܂��S�̈���}�X�N
		For xx = 1 To MapWidth
			For yy = 1 To MapHeight
				MaskData(xx, yy) = True
			Next 
		Next 
		
		'�N�_�̃}�X�N������
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
	
	'���j�b�g u �̈ړ��͈͂�I��
	'�W�����v����ꍇ�� ByJump = True
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
			'�ړ����Ɏg�p����G���A
			If ByJump Then
				move_area = "��"
			Else
				move_area = .Area
			End If
			
			'�ړ��\�͂̉ۂ𒲂ׂĂ���
			is_trans_available_on_ground = .IsTransAvailable("��") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("��") And .Adaption(3) <> 0
			is_trans_available_in_sky = .IsTransAvailable("��") And .Adaption(1) <> 0
			is_trans_available_in_moon_sky = (.IsTransAvailable("��") And .Adaption(1) <> 0) Or (.IsTransAvailable("�F��") And .Adaption(4) <> 0)
			If Mid(.Data.Adaption, 3, 1) <> "-" Or .IsFeatureAvailable("�����ړ�") Then
				is_adaptable_in_water = True
			End If
			If Mid(.Data.Adaption, 4, 1) <> "-" Or .IsFeatureAvailable("�F���ړ�") Then
				is_adaptable_in_space = True
			End If
			If .IsFeatureAvailable("����ړ�") Or .IsFeatureAvailable("�z�o�[�ړ�") Then
				is_trans_available_on_water = True
			End If
			If .IsFeatureAvailable("���j") Then
				is_swimable = True
			End If
			
			'�n�`�K���̂���n�`�̃��X�g���쐬
			ReDim adopted_terrain(0)
			If .IsFeatureAvailable("�n�`�K��") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "�n�`�K��" Then
						buf = .FeatureData(i)
						If LLength(buf) = 0 Then
							ErrorMessage("���j�b�g�u" & .Name & "�v�̒n�`�K���\�͂ɑΉ��n�`���w�肳��Ă��܂���")
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
			
			'�ړ���
			If ByJump Then
				uspeed = .Speed + .FeatureLevel("�W�����v")
			Else
				uspeed = .Speed
			End If
			If .IsConditionSatisfied("�ړ��s�\") Then
				uspeed = 0
			End If
			
			'�ړ��R�X�g�͎��ۂ̂Q�{�̒l�ŋL�^����Ă��邽�߁A�ړ��͂�����ɍ��킹��
			'�Q�{�ɂ��Ĉړ��͈͂��v�Z����
			uspeed = 2 * uspeed
			
			' ADD START MARGE
			'�Ĉړ����͍ŏ��̈ړ��̕������ړ��͂�����������
			If SelectedCommand = "�Ĉړ�" Then
				uspeed = uspeed - SelectedUnitMoveCost
			End If
			
			If .IsConditionSatisfied("�ړ��s�\") Then
				uspeed = 0
			End If
			' ADD END MARGE
			
			'�ړ��͈͂��`�F�b�N���ׂ��̈�
			x1 = MaxLng(1, .X - uspeed)
			y1 = MaxLng(1, .Y - uspeed)
			x2 = MinLng(.X + uspeed, MapWidth)
			y2 = MinLng(.Y + uspeed, MapHeight)
			
			'�ړ��R�X�g�Ƃy�n�b�����Z�b�g
			For i = 0 To MapWidth + 1
				For j = 0 To MapHeight + 1
					move_cost(i, j) = 1000000
					PointInZOC(i, j) = 0
				Next 
			Next 
			
			'�e�n�`�̈ړ��R�X�g���Z�o���Ă���
			Select Case move_area
				Case "��"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "��"
									move_cost(i, j) = TerrainMoveCost(i, j)
								Case "�F��"
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
				Case "�n��"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "��", "����", "����"
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
								Case "��"
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
								Case "�[��"
									If is_trans_available_in_water Or is_trans_available_on_water Then
										move_cost(i, j) = 2
									ElseIf is_swimable Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
									Else
										move_cost(i, j) = 1000000
									End If
								Case "��"
									move_cost(i, j) = 1000000
								Case "�F��"
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
				Case "����"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "��", "����", "����"
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
								Case "��", "�[��"
									move_cost(i, j) = 2
								Case "��"
									move_cost(i, j) = 1000000
								Case "�F��"
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
				Case "����"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "��", "����", "����"
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
								Case "��"
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
								Case "�[��"
									If is_trans_available_in_water Then
										move_cost(i, j) = 2
									ElseIf is_swimable Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
									Else
										move_cost(i, j) = 1000000
									End If
								Case "��"
									move_cost(i, j) = 1000000
								Case "�F��"
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
				Case "�F��"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "�F��"
									move_cost(i, j) = TerrainMoveCost(i, j)
									For k = 1 To UBound(adopted_terrain)
										If TerrainName(i, j) = adopted_terrain(k) Then
											move_cost(i, j) = MinLng(move_cost(i, j), 2)
											Exit For
										End If
									Next 
								Case "��", "����"
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
								Case "����"
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
								Case "��"
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
								Case "�[��"
									If is_trans_available_in_water Or is_trans_available_on_water Then
										move_cost(i, j) = 2
									ElseIf is_swimable Then 
										move_cost(i, j) = TerrainMoveCost(i, j)
									Else
										move_cost(i, j) = 1000000
									End If
								Case "��"
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
				Case "�n��"
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainClass(i, j)
								Case "��", "����"
									move_cost(i, j) = 2
								Case Else
									move_cost(i, j) = 1000000
							End Select
						Next 
					Next 
			End Select
			
			'���H�ړ�
			If .IsFeatureAvailable("���H�ړ�") Then
				If .Area = "�n��" And Not ByJump Then
					For i = x1 To x2
						For j = y1 To y2
							If TerrainName(i, j) = "���H" Then
								move_cost(i, j) = 2
							Else
								move_cost(i, j) = 1000000
							End If
						Next 
					Next 
				End If
			End If
			
			'�ړ�����
			ReDim allowed_terrains(0)
			If .IsFeatureAvailable("�ړ�����") Then
				If .Area <> "��" And .Area <> "�n��" Then
					n = LLength(.FeatureData("�ړ�����"))
					ReDim allowed_terrains(n)
					For i = 2 To n
						allowed_terrains(i) = LIndex(.FeatureData("�ړ�����"), i)
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
			
			'�i���s��
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("�i���s��") Then
				If .Area <> "��" And .Area <> "�n��" Then
					n = LLength(.FeatureData("�i���s��"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("�i���s��"), i)
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
			
			'�z�o�[�ړ�
			If .IsFeatureAvailable("�z�o�[�ړ�") Then
				If move_area = "�n��" Or move_area = "����" Then
					For i = x1 To x2
						For j = y1 To y2
							Select Case TerrainName(i, j)
								Case "����", "�ጴ"
									move_cost(i, j) = 2
							End Select
						Next 
					Next 
				End If
			End If
			
			'���߈ړ�
			If .IsFeatureAvailable("���߈ړ�") Or .IsUnderSpecialPowerEffect("���߈ړ�") Then
				For i = x1 To x2
					For j = y1 To y2
						move_cost(i, j) = 2
					Next 
				Next 
			End If
			
			'���j�b�g�����邽�ߒʂ蔲���o���Ȃ��ꏊ���`�F�b�N
			If Not .IsFeatureAvailable("���蔲���ړ�") And Not .IsUnderSpecialPowerEffect("���蔲���ړ�") Then
				For	Each u2 In UList
					With u2
						If .Status = "�o��" Then
							
							blocked = False
							
							'�G�΂���ꍇ�͒ʂ蔲���s��
							If .IsEnemy(u, True) Then
								blocked = True
							End If
							
							'�w�c������Ȃ��ꍇ���ʂ蔲���s��
							Select Case .Party0
								Case "����", "�m�o�b"
									If u.Party0 <> "����" And u.Party0 <> "�m�o�b" Then
										blocked = True
									End If
								Case Else
									If .Party0 <> u.Party0 Then
										blocked = True
									End If
							End Select
							
							'�ʂ蔲�����Ȃ��ꍇ
							If blocked Then
								move_cost(.X, .Y) = 1000000
							End If
							
							'�y�n�b
							If blocked And Not ByJump Then
								is_zoc = False
								zarea = 0
								If .IsFeatureAvailable("�y�n�b") Or IsOptionDefined("�y�n�b") Then
									is_zoc = True
									zarea = 1
									
									'�y�n�b���̂y�n�b���x��
									n = .FeatureLevel("�y�n�b")
									If n = 1 Then n = 10000
									
									'Option�u�y�n�b�v���w�肳��Ă���
									n = MaxLng(1, n)
									
									If u.IsFeatureAvailable("�y�n�b������") Then
										'�ړ����̂y�n�b���������x��
										'���x���w��Ȃ��A�܂���Lv1��Lv10000�Ƃ��Ĉ���
										l = u.FeatureLevel("�y�n�b������")
										If l = 1 Then l = 10000
										
										'�ړ����̂y�n�b���������x���̕��������ꍇ�A
										'�y�n�b�s�\
										If l >= n Then
											is_zoc = False
										End If
									End If
									
									'�אڂ��郆�j�b�g���u�אڃ��j�b�g�y�n�b�������v�������Ă���ꍇ
									If is_zoc Then
										For i = -1 To 1
											For j = System.Math.Abs(i) - 1 To System.Math.Abs(System.Math.Abs(i) - 1)
												If (i <> 0 Or j <> 0) And ((.X + i) >= 1 And (.X + i) <= MapWidth And (.Y + j) >= 1 And (.Y + j) <= MapHeight) Then
													'�אڃ��j�b�g�����݂���H
													If Not MapDataForUnit(.X + i, .Y + j) Is Nothing Then
														buf = .Party0
														With MapDataForUnit(.X + i, .Y + j)
															'�G�ΐw�c�H
															Select Case .Party0
																Case "����", "�m�o�b"
																	If buf = "����" Or buf = "�m�o�b" Then
																		Exit For
																	End If
																Case Else
																	If .Party0 = buf Then
																		Exit For
																	End If
															End Select
															
															l = .FeatureLevel("�אڃ��j�b�g�y�n�b������")
															If l = 1 Then l = 10000
															
															'�ړ����̂y�n�b���������x���̕��������ꍇ�A
															'�y�n�b�s�\
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
									'����\�́u�y�n�b�v���w�肳��Ă���Ȃ�A���̃f�[�^��2�ڂ̒l���y�n�b�͈̔͂ɐݒ�
									'2�ڂ̒l���ȗ�����Ă���ꍇ��1��ݒ�
									'�y�n�bLv��0�ȉ��̏ꍇ�A�I�v�V�����u�y�n�b�v���w�肳��Ă��Ă��͈͂�0�ɐݒ�
									If LLength(.FeatureData("�y�n�b")) >= 2 Then
										zarea = MaxLng(CInt(LIndex(.FeatureData("�y�n�b"), 2)), 0)
									End If
									
									'���΋����{�y�n�b�͈̔͂��ړ��͈ȓ��̂Ƃ��A�y�n�b��ݒ�
									If System.Math.Abs(u.X - .X) + System.Math.Abs(u.Y - .Y) - zarea <= uspeed Then
										'�����E���������݂̂̂y�n�b���ǂ����𔻒f
										is_hzoc = False
										is_vzoc = False
										If InStr(.FeatureData("�y�n�b"), "����") Then
											is_hzoc = True
											is_vzoc = True
										Else
											If InStr(.FeatureData("�y�n�b"), "����") Then
												is_hzoc = True
											End If
											If InStr(.FeatureData("�y�n�b"), "����") Then
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
													'�����y�n�b
													If is_hzoc And (.X + i) >= 1 And (.X + i) <= MapWidth Then
														If PointInZOC(.X + i, .Y) < 0 Then
															If n > System.Math.Abs(PointInZOC(.X + i, .Y)) Then
																PointInZOC(.X + i, .Y) = n
															End If
														Else
															PointInZOC(.X + i, .Y) = MaxLng(n, PointInZOC(.X + i, .Y))
														End If
													End If
													'�����y�n�b
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
											'�S���ʂy�n�b
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
								'�u�L��y�n�b�������v���������Ă���ꍇ�̏���
								If System.Math.Abs(u.X - .X) + System.Math.Abs(u.Y - .Y) - zarea <= uspeed Then
									'���x���w��Ȃ��A�܂���Lv1��Lv10000�Ƃ��Ĉ���
									l = .FeatureLevel("�L��y�n�b������")
									If l = 1 Then l = 10000
									
									If l > 0 Then
										n = MaxLng(StrToLng(LIndex(.FeatureData("�L��y�n�b������"), 2)), 1)
										
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
			
			'�ړ���~�n�`�͂y�n�b���Ĉ���
			If Not ByJump Then
				With TDList
					For i = x1 To x2
						For j = y1 To y2
							'MOD START 240a
							'                        If .IsFeatureAvailable(MapData(i, j, 0), "�ړ���~") Then
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
			
			'�}�b�v��̊e�n�_�ɓ��B����̂ɕK�v�Ȉړ��͂��v�Z����
			
			'�܂��ړ��R�X�g�v�Z�p�̔z���������
			For i = 0 To MapWidth + 1
				For j = 0 To MapHeight + 1
					TotalMoveCost(i, j) = 1000000
				Next 
			Next 
			
			'���݂���ꏊ�͈ړ�����K�v���Ȃ����߁A�K�v�ړ��͂�0
			TotalMoveCost(.X, .Y) = 0
			
			'�K�v�ړ��͂̌v�Z
			For i = 1 To uspeed
				'���݂̕K�v�ړ��͂�ۑ�
				For j = MaxLng(0, .X - i - 1) To MinLng(.X + i + 1, MapWidth + 1)
					For k = MaxLng(0, .Y - i - 1) To MinLng(.Y + i + 1, MapHeight + 1)
						cur_cost(j, k) = TotalMoveCost(j, k)
					Next 
				Next 
				For j = MaxLng(1, .X - i) To MinLng(.X + i, MapWidth)
					For k = MaxLng(1, .Y - i) To MinLng(.Y + i, MapHeight)
						'�אڂ���n�_�Ɣ�r���čł��Ⴂ�K�v�ړ��͂����߂�
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
						'�n�`�ɐi������̂ɕK�v�Ȉړ��͂����Z
						tmp = tmp + move_cost(j, k)
						'�O��̒l�Ƃǂ��炪�Ⴂ�H
						TotalMoveCost(j, k) = MinLng(tmp, cur_cost(j, k))
					Next 
				Next 
			Next 
			
			'�Z�o���ꂽ�K�v�ړ��͂����ɐi���\������
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = True
					
					'�K�v�ړ��͂��ړ��͈ȓ��H
					If TotalMoveCost(i, j) > uspeed Then
						GoTo NextLoop
					End If
					
					u2 = MapDataForUnit(i, j)
					
					'���j�b�g�����݁H
					If u2 Is Nothing Then
						MaskData(i, j) = False
						GoTo NextLoop
					End If
					
					'���́����͂���͖̂����̂�
					If .Party0 <> "����" Then
						GoTo NextLoop
					End If
					
					Select Case u2.Party0
						Case "����"
							If u2.IsFeatureAvailable("���") Then
								'��͂ɒ��́H
								If Not .IsFeatureAvailable("���") And u2.Area <> "�n��" Then
									If Not .IsFeatureAvailable("�i�[�s��") Then
										MaskData(i, j) = False
									End If
								End If
							ElseIf .IsFeatureAvailable("����") And u2.IsFeatureAvailable("����") Then 
								'�Q�̍��́H
								MaskData(i, j) = True
								For k = 1 To .CountFeature
									If .Feature(k) = "����" And .FeatureName(k) <> "" Then
										buf = .FeatureData(k)
										If LLength(buf) = 3 And UList.IsDefined(LIndex(buf, 2)) And UList.IsDefined(LIndex(buf, 3)) Then
											With UList.Item(LIndex(buf, 2))
												If .IsConditionSatisfied("�s���s�\") Then
													Exit For
												End If
												If .Status = "�j��" Then
													Exit For
												End If
											End With
											If u2.Name = LIndex(buf, 3) Then
												MaskData(i, j) = False
												Exit For
											ElseIf u2.Name = UList.Item(LIndex(buf, 3)).CurrentForm.Name And Not u2.IsFeatureAvailable("���̐���") Then 
												MaskData(i, j) = False
												Exit For
											End If
										End If
									End If
								Next 
							End If
						Case "�m�o�b"
							If .IsFeatureAvailable("����") And u2.IsFeatureAvailable("����") Then
								'�Q�̍��́H
								MaskData(i, j) = True
								For k = 1 To .CountFeature
									If .Feature(k) = "����" Then
										buf = .FeatureData(k)
										If LLength(buf) = 3 And UList.IsDefined(LIndex(buf, 2)) And UList.IsDefined(LIndex(buf, 3)) Then
											With UList.Item(LIndex(buf, 2))
												If .IsConditionSatisfied("�s���s�\") Then
													Exit For
												End If
												If .Status = "�j��" Then
													Exit For
												End If
											End With
											If u2.Name = LIndex(buf, 3) Then
												MaskData(i, j) = False
												Exit For
											ElseIf u2.Name = UList.Item(LIndex(buf, 3)).CurrentForm.Name And Not u2.IsFeatureAvailable("���̐���") Then 
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
			
			'�W�����v�����߈ړ���͐i���\�H
			If ByJump Or .IsFeatureAvailable("���߈ړ�") Or .IsUnderSpecialPowerEffect("���߈ړ�") Then
				For i = x1 To x2
					For j = y1 To y2
						If MaskData(i, j) Then
							GoTo NextLoop2
						End If
						
						'���j�b�g������n�`�ɐi���o����Ƃ������Ƃ�
						'����or���͉\�Ƃ������ƂȂ̂Œn�`�͖���
						If Not MapDataForUnit(i, j) Is Nothing Then
							GoTo NextLoop2
						End If
						
						Select Case .Area
							Case "�n��"
								Select Case TerrainClass(i, j)
									Case "��"
										MaskData(i, j) = True
									Case "��"
										If Not is_adaptable_in_water And Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "�[��"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "�F��"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "����"
								Select Case TerrainClass(i, j)
									Case "��"
										MaskData(i, j) = True
									Case "�F��"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "����"
								Select Case TerrainClass(i, j)
									Case "��"
										MaskData(i, j) = True
									Case "�[��"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "�F��"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "��"
								Select Case TerrainClass(i, j)
									Case "��"
										If TerrainMoveCost(i, j) > 100 Then
											MaskData(i, j) = True
										End If
									Case "�F��"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "�n��"
								If TerrainClass(i, j) <> "��" Then
									MaskData(i, j) = True
								End If
							Case "�F��"
								Select Case TerrainClass(i, j)
									Case "��", "����"
										If Not is_trans_available_in_sky And Not is_trans_available_on_ground Then
											MaskData(i, j) = True
										End If
									Case "��"
										If Not is_trans_available_in_sky Or TerrainMoveCost(i, j) > 10 Then
											MaskData(i, j) = True
										End If
									Case "��"
										If Not is_trans_available_in_water And Not is_trans_available_on_water And Not is_adaptable_in_water Then
											MaskData(i, j) = True
										End If
									Case "�[��"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
								End Select
						End Select
						
						'�ړ�����
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
						
						'�i���s��
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
			
			'���݂���ꏊ�͏�ɐi���\
			MaskData(.X, .Y) = False
		End With
	End Sub
	
	'���j�b�g u ���e���|�[�g���Ĉړ��ł���͈͂�I��
	'�ő勗�� lv ���w��\�B(�ȗ����͈ړ��́{�e���|�[�g���x��)
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
			'�ړ��\�͂̉ۂ𒲂ׂĂ���
			is_trans_available_on_ground = .IsTransAvailable("��") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("��") And .Adaption(3) <> 0
			is_trans_available_in_sky = .IsTransAvailable("��") And .Adaption(1) <> 0
			If Mid(.Data.Adaption, 3, 1) <> "-" Or .IsFeatureAvailable("�����ړ�") Then
				is_adaptable_in_water = True
			End If
			If Mid(.Data.Adaption, 4, 1) <> "-" Or .IsFeatureAvailable("�F���ړ�") Then
				is_adaptable_in_space = True
			End If
			If .IsFeatureAvailable("����ړ�") Or .IsFeatureAvailable("�z�o�[�ړ�") Then
				is_trans_available_on_water = True
			End If
			
			'�ړ�����
			ReDim allowed_terrains(0)
			If .IsFeatureAvailable("�ړ�����") Then
				If .Area <> "��" And .Area <> "�n��" Then
					n = LLength(.FeatureData("�ړ�����"))
					ReDim allowed_terrains(n)
					For i = 2 To n
						allowed_terrains(i) = LIndex(.FeatureData("�ړ�����"), i)
					Next 
				End If
			End If
			
			'�i���s��
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("�i���s��") Then
				If .Area <> "��" And .Area <> "�n��" Then
					n = LLength(.FeatureData("�i���s��"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("�i���s��"), i)
					Next 
				End If
			End If
			
			'�e���|�[�g�ɂ��ړ��������Z�o
			If lv > 0 Then
				r = lv
			Else
				r = .Speed + .FeatureLevel("�e���|�[�g")
			End If
			If .IsConditionSatisfied("�ړ��s�\") Then
				r = 0
			End If
			
			'�I������
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = True
				Next 
			Next 
			
			'�ړ��\�Ȓn�_�𒲂ׂ�
			For i = MaxLng(1, .X - r) To MinLng(MapWidth, .X + r)
				For j = MaxLng(1, .Y - r) To MinLng(MapHeight, .Y + r)
					'�ړ��͈͓��H
					If System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) > r Then
						GoTo NextLoop
					End If
					
					u2 = MapDataForUnit(i, j)
					
					If u2 Is Nothing Then
						'���j�b�g�����Ȃ��n�_�͒n�`����i���\���`�F�b�N
						MaskData(i, j) = False
						Select Case .Area
							Case "�n��"
								Select Case TerrainClass(i, j)
									Case "��"
										MaskData(i, j) = True
									Case "��"
										If Not is_adaptable_in_water And Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "�[��"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "�F��"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "����"
								Select Case TerrainClass(i, j)
									Case "��"
										MaskData(i, j) = True
									Case "�[��"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
									Case "�F��"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "��"
								Select Case TerrainClass(i, j)
									Case "��"
										If TerrainMoveCost(i, j) > 100 Then
											MaskData(i, j) = True
										End If
									Case "�F��"
										If Not is_adaptable_in_space Then
											MaskData(i, j) = True
										End If
								End Select
							Case "�n��"
								If TerrainClass(i, j) <> "��" Then
									MaskData(i, j) = True
								End If
							Case "�F��"
								Select Case TerrainClass(i, j)
									Case "��", "����"
										If Not is_trans_available_in_sky And Not is_trans_available_on_ground Then
											MaskData(i, j) = True
										End If
									Case "��"
										If Not is_trans_available_in_sky Or TerrainMoveCost(i, j) > 100 Then
											MaskData(i, j) = True
										End If
									Case "��"
										If Not is_trans_available_in_water And Not is_trans_available_on_water And Not is_adaptable_in_water Then
											MaskData(i, j) = True
										End If
									Case "�[��"
										If Not is_trans_available_on_water And Not is_trans_available_in_water Then
											MaskData(i, j) = True
										End If
								End Select
						End Select
						
						'�ړ�����
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
						
						'�i���s��
						For k = 2 To UBound(prohibited_terrains)
							If TerrainName(i, j) = prohibited_terrains(k) Then
								MaskData(i, j) = True
								Exit For
							End If
						Next 
						
						GoTo NextLoop
					End If
					
					'���́����͂���͖̂����̂�
					If .Party0 <> "����" Then
						GoTo NextLoop
					End If
					
					Select Case u2.Party0
						Case "����"
							If u2.IsFeatureAvailable("���") Then
								'��͂ɒ��́H
								If Not .IsFeatureAvailable("���") And Not .IsFeatureAvailable("�i�[�s��") And u2.Area <> "�n��" And Not u2.IsDisabled("���") Then
									MaskData(i, j) = False
								End If
							ElseIf .IsFeatureAvailable("����") And u2.IsFeatureAvailable("����") Then 
								'�Q�̍��́H
								MaskData(i, j) = True
								For k = 1 To .CountFeature
									If .Feature(k) = "����" And .FeatureName(k) <> "" Then
										buf = .FeatureData(k)
										If LLength(buf) = 3 And UList.IsDefined(LIndex(buf, 2)) And UList.IsDefined(LIndex(buf, 3)) Then
											With UList.Item(LIndex(buf, 2))
												If .IsConditionSatisfied("�s���s�\") Then
													Exit For
												End If
												If .Status = "�j��" Then
													Exit For
												End If
											End With
											If u2.Name = LIndex(buf, 3) Then
												MaskData(i, j) = False
												Exit For
											ElseIf u2.Name = UList.Item(LIndex(buf, 3)).CurrentForm.Name And Not u2.IsFeatureAvailable("���̐���") Then 
												MaskData(i, j) = False
												Exit For
											End If
										End If
									End If
								Next 
							End If
						Case "�m�o�b"
							If .IsFeatureAvailable("����") And u2.IsFeatureAvailable("����") Then
								'�Q�̍��́H
								MaskData(i, j) = True
								For k = 1 To .CountFeature
									If .Feature(k) = "����" Then
										buf = .FeatureData(k)
										If LLength(buf) = 3 And UList.IsDefined(LIndex(buf, 2)) And UList.IsDefined(LIndex(buf, 3)) Then
											With UList.Item(LIndex(buf, 2))
												If .IsConditionSatisfied("�s���s�\") Then
													Exit For
												End If
												If .Status = "�j��" Then
													Exit For
												End If
											End With
											If u2.Name = LIndex(buf, 3) Then
												MaskData(i, j) = False
												Exit For
											ElseIf u2.Name = UList.Item(LIndex(buf, 3)).CurrentForm.Name And Not u2.IsFeatureAvailable("���̐���") Then 
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
			
			'���݂���ꏊ�͏�ɐi���\
			MaskData(.X, .Y) = False
		End With
	End Sub
	
	'���j�b�g u �̂l�ڕ���A�A�r���e�B�̃^�[�Q�b�g���W�I��p
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
			'�S�̈�}�X�N
			For i = 1 To MapWidth
				For j = 1 To MapHeight
					MaskData(i, j) = True
				Next 
			Next 
			
			'�ړ��\�͂̉ۂ𒲂ׂĂ���
			is_trans_available_on_ground = .IsTransAvailable("��") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("��") And .Adaption(3) <> 0
			is_trans_available_in_sky = .IsTransAvailable("��") And .Adaption(1) <> 0
			If Mid(.Data.Adaption, 3, 1) <> "-" Or .IsFeatureAvailable("�����ړ�") Then
				is_adaptable_in_water = True
			End If
			If Mid(.Data.Adaption, 4, 1) <> "-" Or .IsFeatureAvailable("�F���ړ�") Then
				is_adaptable_in_space = True
			End If
			If .IsFeatureAvailable("����ړ�") Or .IsFeatureAvailable("�z�o�[�ړ�") Then
				is_trans_available_on_water = True
			End If
			If .IsFeatureAvailable("���߈ړ�") Or .IsUnderSpecialPowerEffect("���߈ړ�") Then
				is_able_to_penetrate = True
			End If
			
			' ADD START MARGE
			'�n�`�K���̂���n�`�̃��X�g���쐬
			ReDim adopted_terrain(0)
			If .IsFeatureAvailable("�n�`�K��") Then
				For i = 1 To .CountFeature
					If .Feature(i) = "�n�`�K��" Then
						buf = .FeatureData(i)
						If LLength(buf) = 0 Then
							ErrorMessage("���j�b�g�u" & .Name & "�v�̒n�`�K���\�͂ɑΉ��n�`���w�肳��Ă��܂���")
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
			
			'�ړ�����
			ReDim allowed_terrains(0)
			If .IsFeatureAvailable("�ړ�����") Then
				If .Area <> "��" And .Area <> "�n��" Then
					n = LLength(.FeatureData("�ړ�����"))
					ReDim allowed_terrains(n)
					For i = 2 To n
						allowed_terrains(i) = LIndex(.FeatureData("�ړ�����"), i)
					Next 
				End If
			End If
			
			'�i���s��
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("�i���s��") Then
				If .Area <> "��" And .Area <> "�n��" Then
					n = LLength(.FeatureData("�i���s��"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("�i���s��"), i)
					Next 
				End If
			End If
			
			'�ړ��͈͂��`�F�b�N���ׂ��̈�
			x1 = MaxLng(1, .X - max_range)
			y1 = MaxLng(1, .Y - max_range)
			x2 = MinLng(.X + max_range, MapWidth)
			y2 = MinLng(.Y + max_range, MapHeight)
			
			'�i���\������
			For i = x1 To x2
				For j = y1 To y2
					'�ړ��͈͓͂̔��H
					If System.Math.Abs(.X - i) + System.Math.Abs(.Y - j) > max_range Then
						GoTo NextLoop
					End If
					
					'���j�b�g�����݁H
					If Not MapDataForUnit(i, j) Is Nothing Then
						GoTo NextLoop
					End If
					
					'�K������H
					Select Case .Area
						Case "�n��"
							Select Case TerrainClass(i, j)
								Case "��"
									GoTo NextLoop
								Case "��"
									If Not is_adaptable_in_water And Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
								Case "�[��"
									If Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
								Case "�F��"
									If Not is_adaptable_in_space Then
										GoTo NextLoop
									End If
							End Select
						Case "����"
							Select Case TerrainClass(i, j)
								Case "��"
									GoTo NextLoop
								Case "�[��"
									If Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
								Case "�F��"
									If Not is_adaptable_in_space Then
										GoTo NextLoop
									End If
							End Select
						Case "��"
							Select Case TerrainClass(i, j)
								Case "��"
									If TerrainMoveCost(i, j) > 100 Then
										GoTo NextLoop
									End If
								Case "�F��"
									If Not is_adaptable_in_space Then
										GoTo NextLoop
									End If
							End Select
						Case "�n��"
							If TerrainClass(i, j) <> "��" Then
								GoTo NextLoop
							End If
						Case "�F��"
							Select Case TerrainClass(i, j)
								Case "��", "����"
									If Not is_trans_available_in_sky And Not is_trans_available_on_ground Then
										GoTo NextLoop
									End If
								Case "��"
									If Not is_trans_available_in_sky Or TerrainMoveCost(i, j) > 100 Then
										GoTo NextLoop
									End If
								Case "��"
									If Not is_trans_available_in_water And Not is_trans_available_on_water And Not is_adaptable_in_water Then
										GoTo NextLoop
									End If
								Case "�[��"
									If Not is_trans_available_on_water And Not is_trans_available_in_water Then
										GoTo NextLoop
									End If
							End Select
							
							'�ړ�����
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
							
							'�i���s��
							For k = 2 To UBound(prohibited_terrains)
								If TerrainName(i, j) = prohibited_terrains(k) Then
									GoTo NextLoop
								End If
							Next 
					End Select
					
					'�N���i�i���j�֎~�n�`�H
					'MOD START 240a
					'                Set td = TDList.Item(MapData(i, j, 0))
					'                With td
					'                    If .IsFeatureAvailable("�N���֎~") Then
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
					
					'�i�H��ɕǂ�����H
					If Not is_able_to_penetrate Then
						If IsLineBlocked(.X, .Y, i, j, .Area = "��") Then
							GoTo NextLoop
						End If
					End If
					
					'�}�X�N����
					MaskData(i, j) = False
NextLoop: 
				Next 
			Next 
			
			'���݂���ꏊ�͏�ɐi���\
			MaskData(.X, .Y) = False
		End With
	End Sub
	
	'�Q�_�Ԃ����Ԓ������ǂŃu���b�N����Ă��邩����
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
				
				'�ǁH
				If is_flying Then
					If TerrainName(xx, yy) = "��" Or TerrainName(xx, yy2) = "��" Then
						IsLineBlocked = True
						Exit Function
					End If
				Else
					Select Case TerrainName(xx, yy)
						Case "��", "�h��"
							IsLineBlocked = True
							Exit Function
					End Select
					Select Case TerrainName(xx, yy2)
						Case "��", "�h��"
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
				
				'�ǁH
				If is_flying Then
					If TerrainName(xx, yy) = "��" Or TerrainName(xx2, yy) = "��" Then
						IsLineBlocked = True
						Exit Function
					End If
				Else
					Select Case TerrainName(xx, yy)
						Case "��", "�h��"
							IsLineBlocked = True
							Exit Function
					End Select
					Select Case TerrainName(xx2, yy)
						Case "��", "�h��"
							IsLineBlocked = True
							Exit Function
					End Select
				End If
			Loop Until yy = y2
		End If
		
		IsLineBlocked = False
	End Function
	
	'���j�b�g u �� (dst_x,dst_y) �ɍs���̂ɍł��߂��ړ��͈͓��̏ꏊ (X,Y) �͂ǂ�������
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
		
		'�ړI�n���}�b�v�O�ɂȂ�Ȃ��悤��
		dst_x = MaxLng(MinLng(dst_x, MapWidth), 1)
		dst_y = MaxLng(MinLng(dst_y, MapHeight), 1)
		
		'�ړ��\�͂̉ۂ𒲂ׂĂ���
		With u
			X = .X
			Y = .Y
			
			is_trans_available_on_ground = .IsTransAvailable("��") And .Adaption(2) <> 0
			is_trans_available_in_water = .IsTransAvailable("��") And .Adaption(3) <> 0
			If Mid(.Data.Adaption, 3, 1) <> "-" Or .IsFeatureAvailable("�����ړ�") Then
				is_adaptable_in_water = True
			End If
			If .IsFeatureAvailable("����ړ�") Or .IsFeatureAvailable("�z�o�[�ړ�") Then
				is_trans_available_on_water = True
			End If
			
			ReDim adopted_terrain(LLength(.FeatureData("�n�`�K��")))
			For i = 2 To UBound(adopted_terrain)
				adopted_terrain(i) = LIndex(.FeatureData("�n�`�K��"), i)
			Next 
		End With
		
		'�e�n�`�̈ړ��R�X�g���Z�o���Ă���
		Select Case u.Area
			Case "��"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If TerrainClass(i, j) = "��" Then
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
				
			Case "�n��"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Select Case TerrainClass(i, j)
							Case "��", "����", "����"
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
							Case "��"
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
							Case "�[��"
								If is_trans_available_in_water Then
									move_cost(i, j) = 1
								Else
									move_cost(i, j) = 1000000
								End If
							Case "��"
								move_cost(i, j) = 1000000
						End Select
					Next 
				Next 
				
			Case "����"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Select Case TerrainClass(i, j)
							Case "��", "����", "����"
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
							Case "��", "�[��"
								move_cost(i, j) = 2
							Case "��"
								move_cost(i, j) = 1000000
						End Select
					Next 
				Next 
				
			Case "����"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						Select Case TerrainClass(i, j)
							Case "��", "����", "����"
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
							Case "��"
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
							Case "�[��"
								If is_trans_available_in_water Then
									move_cost(i, j) = 1
								Else
									move_cost(i, j) = 1000000
								End If
							Case "��"
								move_cost(i, j) = 1000000
						End Select
					Next 
				Next 
				
			Case "�F��"
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
				
			Case "�n��"
				For i = 1 To MapWidth
					For j = 1 To MapHeight
						If TerrainClass(i, j) = "��" Then
							move_cost(i, j) = 2
						Else
							move_cost(i, j) = 1000000
						End If
					Next 
				Next 
		End Select
		
		With u
			'���H�ړ�
			If .IsFeatureAvailable("���H�ړ�") Then
				If .Area = "�n��" Then
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							If TerrainName(i, j) = "���H" Then
								move_cost(i, j) = 1
							Else
								move_cost(i, j) = 1000000
							End If
						Next 
					Next 
				End If
			End If
			
			'�ړ�����
			ReDim allowed_terrains(0)
			If .IsFeatureAvailable("�ړ�����") Then
				If .Area <> "��" And .Area <> "�n��" Then
					n = LLength(.FeatureData("�ړ�����"))
					ReDim allowed_terrains(n)
					For i = 2 To n
						allowed_terrains(i) = LIndex(.FeatureData("�ړ�����"), i)
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
			
			'�i���s��
			ReDim prohibited_terrains(0)
			If .IsFeatureAvailable("�i���s��") Then
				If .Area <> "��" And .Area <> "�n��" Then
					n = LLength(.FeatureData("�i���s��"))
					ReDim prohibited_terrains(n)
					For i = 2 To n
						prohibited_terrains(i) = LIndex(.FeatureData("�i���s��"), i)
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
			
			'�z�o�[�ړ�
			If .IsFeatureAvailable("�z�o�[�ړ�") Then
				If .Area = "�n��" Or .Area = "����" Then
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							Select Case TerrainName(i, j)
								Case "����", "����"
									move_cost(i, j) = 1
							End Select
						Next 
					Next 
				End If
			End If
			
			'�W�����v�ړ�
			If .IsFeatureAvailable("�W�����v�ړ�") Then
				If .Area = "�n��" Or .Area = "����" Or .Area = "����" Then
					For i = 1 To MapWidth
						For j = 1 To MapHeight
							Select Case TerrainClass(i, j)
								Case "��", "����"
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
		
		'�ړI�n����e�n�_�ɓ��B����̂ɂ�����ړ��͂��v�Z
		i = 0
		Do 
			i = i + 1
			
			'�^�C���A�E�g
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
			
			'�ŒZ�o�H�𔭌�����
			If total_cost(X, Y) <= System.Math.Abs(dst_x - X) + System.Math.Abs(dst_y - Y) + 2 Then
				Exit Do
			End If
		Loop While is_changed
		
		'�ړ��\�͈͓��ŖړI�n�ɍł��߂��ꏊ�����t����
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
	
	'���j�b�g u ���G����ł������Ȃ�ꏊ(X,Y)������
	Public Sub SafetyPoint(ByRef u As Unit, ByRef X As Short, ByRef Y As Short)
		Dim i, j As Short
		Dim total_cost(51, 51) As Integer
		Dim cur_cost(51, 51) As Integer
		Dim tmp As Integer
		Dim t As Unit
		Dim is_changed As Boolean
		
		'��Ɨp�z���������
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
		
		'�e�n�_�̓G����̋������v�Z
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
		
		'�ړ��\�͈͓��œG����ł������ꏊ�����t����
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
	
	'���݈ʒu����w�肵���ꏊ�܂ł̈ړ��o�H�𒲂ׂ�
	'���O��AreaInSpeed�����s���Ă��������K�v
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
		
		'���݈ʒu�𒲂ׂ�
		For xx = 1 To MapWidth
			For yy = 1 To MapHeight
				If TotalMoveCost(xx, yy) = 0 Then
					ox = xx
					oy = yy
				End If
			Next 
		Next 
		
		'���݈ʒu�̂y�n�b�͖���������
		PointInZOC(ox, oy) = 0
		
		xx = tx
		yy = ty
		nx = tx
		ny = ty
		
		Do While TotalMoveCost(xx, yy) > 0
			tmp = TotalMoveCost(xx, yy)
			
			'����̏ꏊ����ł��K�v�ړ��͂��Ⴂ�ꏊ��T��
			
			'�Ȃ�ׂ����������Ɉړ������邽�߁A�O��Ɠ����ړ�������D�悳����
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
			
			'�Ȃ�ׂ��ڕW�ʒu�t�߂Œ��i�����邽�߁A�ڕW�ʒu�Ƃ̋������̏�����
			'���W�������ɗD�悵�Ĉړ�������
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
				'����ȏ�K�v�ړ��͂��Ⴂ�ꏊ��������Ȃ������̂ŏI��
				Exit Do
			End If
			
			'���������ꏊ���L�^
			ReDim Preserve move_route_x(UBound(move_route_x) + 1)
			ReDim Preserve move_route_y(UBound(move_route_y) + 1)
			move_route_x(UBound(move_route_x)) = nx
			move_route_y(UBound(move_route_y)) = ny
			
			'�ړ��������L�^
			ReDim Preserve move_direction(UBound(move_direction) + 1)
			'UPGRADE_WARNING: �I�u�W�F�N�g move_direction(UBound()) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			move_direction(UBound(move_direction)) = direction
			prev_direction = direction
			
			'����͍��񌩂������ꏊ���N�_�Ɍ�������
			xx = nx
			yy = ny
		Loop 
		
		'�����𑖂����������v�Z
		MovedUnitSpeed = 1
		For i = 2 To UBound(move_direction) - 1
			'UPGRADE_WARNING: �I�u�W�F�N�g move_direction(i + 1) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			'UPGRADE_WARNING: �I�u�W�F�N�g move_direction(i) �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			If move_direction(i) <> move_direction(i + 1) Then
				Exit For
			End If
			MovedUnitSpeed = MovedUnitSpeed + 1
		Next 
	End Sub
End Module