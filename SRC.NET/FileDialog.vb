Option Strict Off
Option Explicit On
Module FileDialog
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'API�����Ńt�@�C���_�C�A���O���������邽�߂̃��W���[��
	
	' OPENFILENAME�\����
	Private Structure OPENFILENAME
		Dim lStructSize As Integer
		Dim hwndOwner As Integer
		Dim hInstance As Integer
		Dim lpstrFilter As String
		Dim lpstrCustomFilter As String
		Dim nMaxCustFilter As Integer
		Dim iFilterIndex As Integer
		Dim lpstrFile As String
		Dim nMaxFile As Integer
		Dim lpstrFileTitle As String
		Dim nMaxFileTitle As Integer
		Dim lpstrInitialDir As String
		Dim lpstrTitle As String
		Dim flags As Integer
		Dim nFileOffset As Short
		Dim nFileExtension As Short
		Dim lpstrDefExt As String
		Dim lCustData As Integer
		Dim lpfnHook As Integer
		Dim lpTemplateName As String
	End Structure
	
	'�u�t�@�C�����J���v�̃_�C�A���O���쐬
	'UPGRADE_WARNING: �\���� OPENFILENAME �ɁA���� Declare �X�e�[�g�����g�̈����Ƃ��ă}�[�V�������O������n���K�v������܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' ���N���b�N���Ă��������B
	Private Declare Function GetOpenFileName Lib "comdlg32.dll"  Alias "GetOpenFileNameA"(ByRef f As OPENFILENAME) As Integer
	
	'�u�t�@�C����ۑ��v�̃_�C�A���O���쐬
	'UPGRADE_WARNING: �\���� OPENFILENAME �ɁA���� Declare �X�e�[�g�����g�̈����Ƃ��ă}�[�V�������O������n���K�v������܂��B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="C429C3A5-5D47-4CD9-8F51-74A1616405DC"' ���N���b�N���Ă��������B
	Private Declare Function GetSaveFileName Lib "comdlg32.dll"  Alias "GetSaveFileNameA"(ByRef f As OPENFILENAME) As Integer
	
	'�t�@�C�����[�h�p�̃_�C�A���O��\�����邽�߂̊֐�
	Public Function LoadFileDialog(ByRef dtitle As String, ByRef fpath As String, ByRef default_file As String, ByVal default_filter As Short, ByRef ftype As String, ByRef fsuffix As String, Optional ByRef ftype2 As String = "", Optional ByRef fsuffix2 As String = "", Optional ByRef ftype3 As String = "", Optional ByRef fsuffix3 As String = "") As String
		Dim f As OPENFILENAME
		Dim fname, ftitle As String
		Dim ret As Integer
		
		fname = default_file & New String(vbNullChar, 1024 - Len(default_file))
		ftitle = Space(1024)
		
		' OPENFILENAME�\���̂̏�����
		With f
			.hwndOwner = MainForm.Handle.ToInt32
			If ftype3 <> "" Then
				.lpstrFilter = "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar & ftype3 & " (*." & fsuffix3 & ")" & vbNullChar & "*." & fsuffix3 & vbNullChar
			ElseIf ftype2 <> "" Then 
				.lpstrFilter = "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
			Else
				.lpstrFilter = "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar
			End If
			.iFilterIndex = default_filter
			.lpstrFile = fname
			.nMaxFile = 1024
			.lpstrFileTitle = ftitle
			.nMaxFileTitle = 1024
			.lpstrInitialDir = fpath
			.lpstrTitle = dtitle
			.flags = &H201804
			.lpstrDefExt = ""
			.lStructSize = Len(f)
		End With
		
		ret = GetOpenFileName(f)
		
		Select Case ret
			Case 0
				'�L�����Z�����ꂽ
				LoadFileDialog = ""
			Case 1
				'�t�@�C����I��
				LoadFileDialog = f.lpstrFile
				' vbNullChar �܂łŐ؂�o���B
				LoadFileDialog = Left(LoadFileDialog, InStr(LoadFileDialog, vbNullChar) - 1)
		End Select
	End Function
	
	'�t�@�C���Z�[�u�p�̃_�C�A���O��\�����邽�߂̊֐�
	Public Function SaveFileDialog(ByRef dtitle As String, ByRef fpath As String, ByRef default_file As String, ByVal default_filter As Short, ByRef ftype As String, ByRef fsuffix As String, Optional ByRef ftype2 As String = "", Optional ByRef fsuffix2 As String = "", Optional ByRef ftype3 As String = "", Optional ByRef fsuffix3 As String = "") As String
		Dim f As OPENFILENAME
		Dim fname, ftitle As String
		Dim ret As Integer
		
		fname = default_file & New String(vbNullChar, 1024 - Len(default_file))
		ftitle = Space(1024)
		
		' OPENFILENAME�\���̂̏�����
		With f
			.hwndOwner = MainForm.Handle.ToInt32
			If ftype3 <> "" Then
				.lpstrFilter = "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar & ftype3 & " (*." & fsuffix3 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
			ElseIf ftype2 <> "" Then 
				.lpstrFilter = "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar & ftype2 & " (*." & fsuffix2 & ")" & vbNullChar & "*." & fsuffix2 & vbNullChar
			Else
				.lpstrFilter = "���ׂĂ�̧�� (*.*)" & vbNullChar & "*.*" & vbNullChar & ftype & " (*." & fsuffix & ")" & vbNullChar & "*." & fsuffix & vbNullChar
			End If
			.iFilterIndex = default_filter
			.lpstrFile = fname
			.nMaxFile = 1024
			.lpstrFileTitle = ftitle
			.nMaxFileTitle = 1024
			.lpstrInitialDir = fpath
			.lpstrTitle = dtitle
			.flags = &H201804
			.lpstrDefExt = ""
			.lStructSize = Len(f)
		End With
		
		ret = GetSaveFileName(f)
		
		Select Case ret
			Case 0
				'�L�����Z�����ꂽ
				SaveFileDialog = ""
			Case 1
				'�t�@�C����I��
				SaveFileDialog = f.lpstrFile
				' vbNullChar �܂łŐ؂�o���B
				SaveFileDialog = Left(SaveFileDialog, InStr(SaveFileDialog, vbNullChar) - 1)
		End Select
	End Function
End Module