Option Strict Off
Option Explicit On
Friend Class TerrainData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	'Invalid_string_refer_to_original_code
	
	'Invalid_string_refer_to_original_code
	
	'識別番号
	Public ID As Short
	'名称
	Public Name As String
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Bitmap �� Bitmap_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Bitmap_Renamed As String
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class �� Class_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Class_Renamed As String
	'Invalid_string_refer_to_original_code
	Public MoveCost As Short
	'命中修正
	Public HitMod As Short
	'ダメージ修正
	Public DamageMod As Short
	
	'Invalid_string_refer_to_original_code
	Public colFeature As Collection
	
	
	'Invalid_string_refer_to_original_code
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Initialize_Renamed()
		ID = -1
	End Sub
	Public Sub New()
		MyBase.New()
		Class_Initialize_Renamed()
	End Sub
	
	'クラスの解放
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		If Not colFeature Is Nothing Then
			With colFeature
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
			'UPGRADE_NOTE: �I�u�W�F�N�g colFeature ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			colFeature = Nothing
		End If
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	
	'地形効果を追加
	Public Sub AddFeature(ByRef fdef As String)
		Dim fd As FeatureData
		Dim ftype, fdata As String
		Dim flevel As Double
		Dim i, j As Short
		Dim buf As String
		
		If colFeature Is Nothing Then
			colFeature = New Collection
		End If
		
		buf = fdef
		
		'Invalid_string_refer_to_original_code
		flevel = DEFAULT_LEVEL
		i = InStr(buf, "Lv")
		j = InStr(buf, "=")
		If i > 0 And j > 0 And i > j Then
			i = 0
		End If
		If i > 0 Then
			ftype = Left(buf, i - 1)
			If j > 0 Then
				flevel = CDbl(Mid(buf, i + 2, j - (i + 2)))
				fdata = Mid(buf, j + 1)
			Else
				flevel = CDbl(Mid(buf, i + 2))
			End If
		ElseIf j > 0 Then 
			ftype = Left(buf, j - 1)
			fdata = Mid(buf, j + 1)
		Else
			ftype = buf
		End If
		
		'地形効果を登録
		fd = New FeatureData
		With fd
			.Name = ftype
			.Level = flevel
			.StrData = fdata
		End With
		If IsFeatureAvailable(ftype) Then
			colFeature.Add(fd, ftype & VB6.Format(CountFeature))
		Else
			colFeature.Add(fd, ftype)
		End If
	End Sub
	
	'Invalid_string_refer_to_original_code
	Public Function CountFeature() As Short
		If colFeature Is Nothing Then
			Exit Function
		End If
		CountFeature = colFeature.Count()
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function Feature(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		fd = colFeature.Item(Index)
		Feature = fd.Name
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureName(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		fd = colFeature.Item(Index)
		With fd
			If Len(.StrData) > 0 Then
				FeatureName = ListIndex(.StrData, 1)
			ElseIf .Level <> DEFAULT_LEVEL Then 
				FeatureName = .Name & "Lv" & VB6.Format(.Level)
			Else
				FeatureName = .Name
			End If
		End With
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureLevel(ByRef Index As Object) As Double
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		FeatureLevel = fd.Level
		If FeatureLevel = DEFAULT_LEVEL Then
			FeatureLevel = 1
		End If
		Exit Function
		
ErrorHandler: 
		FeatureLevel = 0
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function FeatureData(ByRef Index As Object) As String
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		FeatureData = fd.StrData
		Exit Function
		
ErrorHandler: 
		FeatureData = ""
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsFeatureAvailable(ByRef fname As String) As Boolean
		Dim fd As FeatureData
		
		If colFeature Is Nothing Then
			Exit Function
		End If
		For	Each fd In colFeature
			If fd.Name = fname Then
				IsFeatureAvailable = True
				Exit Function
			End If
		Next fd
		IsFeatureAvailable = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Function IsFeatureLevelSpecified(ByRef Index As Object) As Boolean
		Dim fd As FeatureData
		
		On Error GoTo ErrorHandler
		fd = colFeature.Item(Index)
		
		If fd.Level = DEFAULT_LEVEL Then
			IsFeatureLevelSpecified = False
		Else
			IsFeatureLevelSpecified = True
		End If
		Exit Function
		
ErrorHandler: 
		IsFeatureLevelSpecified = False
	End Function
	
	'Invalid_string_refer_to_original_code
	Public Sub Clear()
		Dim i As Short
		
		ID = -1
		If Not colFeature Is Nothing Then
			With colFeature
				For i = 1 To .Count()
					.Remove(1)
				Next 
			End With
			'UPGRADE_NOTE: �I�u�W�F�N�g colFeature ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			colFeature = Nothing
		End If
	End Sub
End Class