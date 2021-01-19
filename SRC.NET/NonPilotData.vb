Option Strict Off
Option Explicit On
Friend Class NonPilotData
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�m���p�C���b�g�f�[�^�̃N���X
	
	'����
	Public Name As String
	
	'����
	Private strNickname As String
	
	'�r�b�g�}�b�v��
	Private proBitmap As String
	'�r�b�g�}�b�v�����݂��邩
	Public IsBitmapMissing As Boolean
	
	
	'����
	Public ReadOnly Property Nickname0() As String
		Get
			Nickname0 = strNickname
		End Get
	End Property
	
	
	Public Property Nickname() As String
		Get
			Dim pname As String
			Dim idx As Short
			
			Nickname = strNickname
			
			'�C�x���g�ň��̂��ύX����Ă���H
			If InStr(Nickname, "��l��") = 1 Or InStr(Nickname, "�q���C��") = 1 Then
				Nickname = GetValueAsString(Nickname & "����")
			End If
			
			ReplaceSubExpression(Nickname)
			
			'�\��p�^�[���̏ꍇ
			idx = InStr(Name, "(")
			If idx > 1 Then
				'�p�C���b�g�{���̖���or���̂�؂�o��
				pname = Left(Name, idx - 1)
				
				'���̃p�C���b�g���쐬����Ă���H
				If Not PList.IsDefined(pname) Then
					Exit Property
				End If
				
				With PList.Item(pname)
					'�p�C���b�g�����j�b�g�ɏ���Ă���H
					If .Unit_Renamed Is Nothing Then
						Exit Property
					End If
					
					With .Unit_Renamed
						'�O�̂��߁c�c
						If .CountPilot = 0 Then
							Exit Property
						End If
						
						'�p�C���b�g�̓��C���p�C���b�g�H
						If pname <> .MainPilot.Name And pname <> .MainPilot.Data.Nickname Then
							Exit Property
						End If
						
						'�p�C���b�g���̕ύX�\�͂�K�p
						If .IsFeatureAvailable("�p�C���b�g����") Then
							pname = .FeatureData("�p�C���b�g����")
							idx = InStr(pname, "$(����)")
							If idx > 0 Then
								pname = Left(pname, idx - 1) & Nickname & Mid(pname, idx + 5)
							End If
							Nickname = pname
						End If
					End With
				End With
			End If
		End Get
		Set(ByVal Value As String)
			strNickname = Value
		End Set
	End Property
	
	'�r�b�g�}�b�v
	Public ReadOnly Property Bitmap0() As String
		Get
			Bitmap0 = proBitmap
		End Get
	End Property
	
	
	Public Property Bitmap() As String
		Get
			If IsBitmapMissing Then
				Bitmap = "-.bmp"
			Else
				Bitmap = proBitmap
			End If
		End Get
		Set(ByVal Value As String)
			proBitmap = Value
		End Set
	End Property
End Class