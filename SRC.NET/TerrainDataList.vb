Option Strict Off
Option Explicit On
Friend Class TerrainDataList
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�S�n�`�f�[�^���Ǘ����郊�X�g�̃N���X
	
	'�n�`�f�[�^�̓o�^��
	Public Count As Short
	
	'�n�`�f�[�^�̔z��
	'���̃��X�g�Ǘ��p�N���X�ƈقȂ�z����g���Ă���̂̓A�N�Z�X�����������邽��
	'UPGRADE_NOTE: TerrainDataList �� TerrainDataList_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private TerrainDataList_Renamed(MAX_TERRAIN_DATA_NUM) As TerrainData
	
	'�n�`�f�[�^�̓o�^�����L�^���邽�߂̔z��
	'UPGRADE_WARNING: �z�� OrderList �̉����� 1 ���� 0 �ɕύX����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="0F1C9BE1-AF9D-476E-83B1-17D43BECFF20"' ���N���b�N���Ă��������B
	Private OrderList(MAX_TERRAIN_DATA_NUM) As Short
	
	
	'�N���X�̏�����
	'UPGRADE_NOTE: Class_Initialize �� Class_Initialize_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
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
	
	'�N���X�̉��
	'UPGRADE_NOTE: Class_Terminate �� Class_Terminate_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Private Sub Class_Terminate_Renamed()
		Dim i As Short
		
		For i = 0 To MAX_TERRAIN_DATA_NUM
			'UPGRADE_NOTE: �I�u�W�F�N�g TerrainDataList_Renamed() ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			TerrainDataList_Renamed(i) = Nothing
		Next 
	End Sub
	Protected Overrides Sub Finalize()
		Class_Terminate_Renamed()
		MyBase.Finalize()
	End Sub
	
	
	'�w�肵���f�[�^�͓o�^����Ă��邩�H
	Public Function IsDefined(ByVal ID As Short) As Boolean
		If TerrainDataList_Renamed(ID).ID >= 0 Then
			IsDefined = True
		Else
			IsDefined = False
		End If
	End Function
	
	
	'�n�`�f�[�^���X�g����w�肵���f�[�^�����o��
	Public Function Item(ByVal ID As Short) As TerrainData
		Item = TerrainDataList_Renamed(ID)
	End Function
	
	'�w�肵���f�[�^�̖���
	Public Function Name(ByVal ID As Short) As String
		Name = TerrainDataList_Renamed(ID).Name
	End Function
	
	'�w�肵���f�[�^�̉摜�t�@�C����
	Public Function Bitmap(ByVal ID As Short) As String
		Bitmap = TerrainDataList_Renamed(ID).Bitmap_Renamed
	End Function
	
	'�w�肵���f�[�^�̃N���X
	'UPGRADE_NOTE: Class �� Class_Renamed �ɃA�b�v�O���[�h����܂����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="A9E4979A-37FA-4718-9994-97DD76ED70A7"' ���N���b�N���Ă��������B
	Public Function Class_Renamed(ByVal ID As Short) As String
		Class_Renamed = TerrainDataList_Renamed(ID).Class_Renamed
	End Function
	
	'�w�肵���f�[�^�̈ړ��R�X�g
	Public Function MoveCost(ByVal ID As Short) As Short
		MoveCost = TerrainDataList_Renamed(ID).MoveCost
	End Function
	
	'�w�肵���f�[�^�̖����C��
	Public Function HitMod(ByVal ID As Short) As Short
		HitMod = TerrainDataList_Renamed(ID).HitMod
	End Function
	
	'�w�肵���f�[�^�̃_���[�W�C��
	Public Function DamageMod(ByVal ID As Short) As Short
		DamageMod = TerrainDataList_Renamed(ID).DamageMod
	End Function
	
	
	'�w�肵���f�[�^�̓���\��
	
	Public Function IsFeatureAvailable(ByVal ID As Short, ByRef ftype As String) As Boolean
		IsFeatureAvailable = TerrainDataList_Renamed(ID).IsFeatureAvailable(ftype)
	End Function
	
	Public Function FeatureLevel(ByVal ID As Short, ByRef ftype As String) As Double
		FeatureLevel = TerrainDataList_Renamed(ID).FeatureLevel(ftype)
	End Function
	
	Public Function FeatureData(ByVal ID As Short, ByRef ftype As String) As String
		FeatureData = TerrainDataList_Renamed(ID).FeatureData(ftype)
	End Function
	
	
	'�m�Ԗڂɓo�^�����f�[�^�̔ԍ�
	Public Function OrderedID(ByVal n As Short) As Short
		OrderedID = OrderList(n)
	End Function
	
	
	'�f�[�^�t�@�C�� fname ����f�[�^�����[�h
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
			
			'�ԍ�
			If IsNumeric(line_buf) Then
				data_id = CShort(line_buf)
			Else
				err_msg = "�ԍ��̐ݒ肪�Ԉ���Ă��܂��B"
				Error(0)
			End If
			If data_id < 0 Or data_id >= MAX_TERRAIN_DATA_NUM Then
				err_msg = "�ԍ��̐ݒ肪�Ԉ���Ă��܂��B"
				Error(0)
			End If
			
			td = TerrainDataList_Renamed(data_id)
			
			With td
				'�V�K�o�^�H
				If .ID < 0 Then
					Count = Count + 1
					OrderList(Count) = data_id
				Else
					.Clear()
				End If
				.ID = data_id
				
				'����, �摜�t�@�C����
				GetLine(FileNumber, line_buf, line_num)
				
				'����
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "�摜�t�@�C�����������Ă��܂��B"
					Error(0)
				End If
				data_name = Trim(Left(line_buf, ret - 1))
				.Name = data_name
				buf = Mid(line_buf, ret + 1)
				
				'�摜�t�@�C����
				.Bitmap_Renamed = Trim(buf)
				If Len(.Bitmap_Renamed) = 0 Then
					err_msg = "�摜�t�@�C�������w�肳��Ă��܂���B"
					Error(0)
				End If
				
				'�n�`�^�C�v, �ړ��R�X�g, �����C��, �_���[�W�C��
				GetLine(FileNumber, line_buf, line_num)
				
				'�n�`�^�C�v
				ret = InStr(line_buf, ",")
				If ret = 0 Then
					err_msg = "�ړ��R�X�g�������Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(line_buf, ret - 1))
				buf = Mid(line_buf, ret + 1)
				.Class_Renamed = buf2
				
				'�ړ��R�X�g
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�����C���������Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If buf2 = "-" Then
					.MoveCost = 1000
				ElseIf IsNumeric(buf2) Then 
					'0.5���݂̈ړ��R�X�g���g����悤�ɂ��邽�߁A���ۂ̂Q�{�̒l�ŋL�^����
					.MoveCost = CShort(2 * CDbl(buf2))
				End If
				If .MoveCost <= 0 Then
					DataErrorMessage("�ړ��R�X�g�̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'�����C��
				ret = InStr(buf, ",")
				If ret = 0 Then
					err_msg = "�_���[�W�C���������Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(Left(buf, ret - 1))
				buf = Mid(buf, ret + 1)
				If IsNumeric(buf2) Then
					.HitMod = CShort(buf2)
				Else
					DataErrorMessage("�����C���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'�_���[�W�C��
				ret = InStr(buf, ",")
				If ret > 0 Then
					err_msg = "�]���ȁu,�v���w�肳��Ă��܂��B"
					Error(0)
				End If
				buf2 = Trim(buf)
				If IsNumeric(buf2) Then
					.DamageMod = CShort(buf2)
				Else
					DataErrorMessage("�_���[�W�C���̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
				End If
				
				'�n�`����
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
							DataErrorMessage("�s������" & VB6.Format(i) & "�Ԗڂ̒n�`���ʂ̐ݒ肪�Ԉ���Ă��܂��B", fname, line_num, line_buf, data_name)
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
		'�G���[����
		If line_num = 0 Then
			ErrorMessage(fname & "���J���܂���B")
		Else
			FileClose(FileNumber)
			DataErrorMessage(err_msg, fname, line_num, line_buf, data_name)
		End If
		
		End
	End Sub
	
	'���X�g���N���A
	Public Sub Clear()
		Dim i As Short
		
		For i = 0 To MAX_TERRAIN_DATA_NUM - 1
			TerrainDataList_Renamed(i).Clear()
		Next 
		Count = 0
	End Sub
End Class