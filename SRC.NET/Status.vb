Option Strict Off
Option Explicit On
Module Status
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	'�X�e�[�^�X�E�B���h�E�ւ̃X�e�[�^�X�\�����s�����W���[��
	'ppic��upic�ɕ�����Ă��邪�Appic�ɂ̓A�C�R���Ɠ����s�̃f�[�^���������܂��B
	'�����ăp�C���b�g�X�e�[�^�X�������̂�ppic�ł͂Ȃ����Ƃ𗯈ӂ��Ă���
	
	'�X�e�[�^�X��ʂɕ\������Ă��郆�j�b�g
	Public DisplayedUnit As Unit
	Public DisplayedPilotInd As Short
	
	'�X�e�[�^�X��ʂ̍X�V���ꎞ��~���邩�ǂ���
	Public IsStatusWindowDisabled As Boolean
	'ADD START 240a
	'�X�e�[�^�X��ʂ̔w�i�F
	Public StatusWindowBackBolor As Integer
	'�X�e�[�^�X��ʂ̘g�F
	Public StatusWindowFrameColor As Integer
	'�X�e�[�^�X��ʂ̘g��
	Public StatusWindowFrameWidth As Integer
	'�X�e�[�^�X��� �\�͖��̃t�H���g�J���[
	Public StatusFontColorAbilityName As Integer
	'�X�e�[�^�X��� �L���Ȕ\�͂̃t�H���g�J���[
	Public StatusFontColorAbilityEnable As Integer
	'�X�e�[�^�X��� �����Ȕ\�͂̃t�H���g�J���[
	Public StatusFontColorAbilityDisable As Integer
	'�X�e�[�^�X��� ���̑��ʏ�`��̃t�H���g�J���[
	Public StatusFontColorNormalString As Integer
	'ADD  END
	
	'���݂̏󋵂��X�e�[�^�X�E�B���h�E�ɕ\��
	Public Sub DisplayGlobalStatus()
		Dim X, Y As Short
		Dim pic As System.Windows.Forms.PictureBox
		Dim td As TerrainData
		'ADD START 240a
		Dim fname As String
		Dim wHeight As Short
		Dim lineStart, ret, color, lineEnd As Integer
		'ADD  END  240a
		
		'�X�e�[�^�X�E�B���h�E������
		ClearUnitStatus()
		
		'UPGRADE_ISSUE: Control picUnitStatus �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
		pic = MainForm.picUnitStatus
		
		pic.Font = VB6.FontChangeSize(pic.Font, 12)
		
		'ADD START 240a
		'�}�E�X�J�[�\���̈ʒu�́H
		X = PixelToMapX(MouseX)
		Y = PixelToMapY(MouseY)
		
		If NewGUIMode Then
			'Global�ϐ����錾����Ă���΁A�X�e�[�^�X��ʗp�ϐ��̓��������
			GlobalVariableLoad()
			pic.BackColor = System.Drawing.ColorTranslator.FromOle(StatusWindowBackBolor)
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.DrawWidth �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.DrawWidth = StatusWindowFrameWidth
			color = StatusWindowFrameColor
			lineStart = (StatusWindowFrameWidth - 1) / 2
			lineEnd = (StatusWindowFrameWidth + 1) / 2
			'UPGRADE_ISSUE: �萔 vbFSTransparent �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.FillStyle �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.FillStyle = vbFSTransparent
			'��U�������ő�ɂ���
			pic.Width = VB6.TwipsToPixelsX(235)
			pic.Height = VB6.TwipsToPixelsY(MapPHeight - 20)
			wHeight = GetGlobalStatusSize(X, Y)
			'�g��������
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Line (lineStart, lineStart) - (235 - lineEnd, wHeight - lineEnd), color, B
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.FillStyle �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.FillStyle = ObjFillStyle
			'������ݒ肷��
			pic.Height = VB6.TwipsToPixelsY(wHeight)
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.CurrentX = 5
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.CurrentY = 5
			'�����F�����Z�b�g
			pic.ForeColor = System.Drawing.ColorTranslator.FromOle(StatusFontColorNormalString)
		End If
		'ADD  END  240a
		'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
		pic.Print("�^�[���� " & VB6.Format(Turn))
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
		pic.Print(Term("����", Nothing, 8) & " " & VB6.Format(Money))
		
		'MOV START 240a ���Ɉړ�
		'    '�}�E�X�J�[�\���̈ʒu�́H
		'    X = PixelToMapX(MouseX)
		'    Y = PixelToMapY(MouseY)
		'MOV  END  240a
		
		'�}�b�v�O���N���b�N�������͂����ŏI��
		If X < 1 Or MapWidth < X Or Y < 1 Or MapHeight < Y Then
			pic.Font = VB6.FontChangeSize(pic.Font, 9)
			If NewGUIMode Then
				'������ݒ肷��
				pic.Height = VB6.TwipsToPixelsY(wHeight)
			End If
			Exit Sub
		End If
		
		'�n�`���̕\��
		'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
		pic.Print()
		
		'�n�`����
		'ADD START 240a
		'�}�b�v�摜�\��
		If NewGUIMode Then
			'UPGRADE_ISSUE: Control picBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.hDC �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			ret = GUI.BitBlt(pic.hDC, 5, 48, 32, 32, MainForm.picBack.hDC, (X - 1) * 32, (Y - 1) * 32, SRCCOPY)
		Else
			'UPGRADE_ISSUE: Control picBack �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.hDC �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			ret = GUI.BitBlt(pic.hDC, 0, 48, 32, 32, MainForm.picBack.hDC, (X - 1) * 32, (Y - 1) * 32, SRCCOPY)
		End If
		'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
		pic.CurrentX = 37
		'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
		pic.CurrentY = 65
		'ADD  END  240a
		If InStr(TerrainName(X, Y), "(") > 0 Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print("(" & VB6.Format(X) & "," & VB6.Format(Y) & ") " & Left(TerrainName(X, Y), InStr(TerrainName(X, Y), "(") - 1))
		Else
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print("(" & VB6.Format(X) & "," & VB6.Format(Y) & ") " & TerrainName(X, Y))
		End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'�����C��
		If TerrainEffectForHit(X, Y) >= 0 Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print("��� +" & VB6.Format(TerrainEffectForHit(X, Y)) & "%")
		Else
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print("��� " & VB6.Format(TerrainEffectForHit(X, Y)) & "%")
		End If
		
		'�_���[�W�C��
		If TerrainEffectForDamage(X, Y) >= 0 Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print("  �h�� +" & VB6.Format(TerrainEffectForDamage(X, Y)) & "%")
		Else
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print("  �h�� " & VB6.Format(TerrainEffectForDamage(X, Y)) & "%")
		End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'�g�o�񕜗�
		If TerrainEffectForHPRecover(X, Y) > 0 Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print(Term("�g�o") & " +" & VB6.Format(TerrainEffectForHPRecover(X, Y)) & "%  ")
		End If
		
		'�d�m�񕜗�
		If TerrainEffectForENRecover(X, Y) > 0 Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print(Term("�d�m") & " +" & VB6.Format(TerrainEffectForENRecover(X, Y)) & "%")
		End If
		
		If TerrainEffectForHPRecover(X, Y) > 0 Or TerrainEffectForENRecover(X, Y) > 0 Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print()
		End If
		
		'MOD START 240a
		'    Set td = TDList.Item(MapData(X, Y, 0))
		'�}�X�̃^�C�v�ɉ����ĎQ�Ɛ��ύX
		Select Case MapData(X, Y, Map.MapDataIndex.BoxType)
			Case Map.BoxTypes.Under, Map.BoxTypes.UpperBmpOnly
				td = TDList.Item(MapData(X, Y, Map.MapDataIndex.TerrainType))
			Case Else
				td = TDList.Item(MapData(X, Y, Map.MapDataIndex.LayerType))
		End Select
		'MOD  END
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'�g�o���d�m����
		If td.IsFeatureAvailable("�g�o����") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print(Term("�g�o") & " -" & VB6.Format(10 * td.FeatureLevel("�g�o����")) & "% (" & td.FeatureData("�g�o����") & ")  ")
		End If
		If td.IsFeatureAvailable("�d�m����") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print(Term("�d�m") & " -" & VB6.Format(10 * td.FeatureLevel("�d�m����")) & "% (" & td.FeatureData("�d�m����") & ")  ")
		End If
		If td.IsFeatureAvailable("�g�o����") Or td.IsFeatureAvailable("�d�m����") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print()
		End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'�g�o���d�m����
		If td.IsFeatureAvailable("�g�o����") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print(Term("�g�o") & " +" & VB6.Format(1000 * td.FeatureLevel("�g�o����")) & "  ")
		End If
		If td.IsFeatureAvailable("�d�m����") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print(Term("�d�m") & " +" & VB6.Format(10 * td.FeatureLevel("�d�m����")) & "  ")
		End If
		If td.IsFeatureAvailable("�g�o����") Or td.IsFeatureAvailable("�d�m����") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print()
		End If
		'MOD  END
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'�g�o���d�m�ቺ
		If td.IsFeatureAvailable("�g�o�ቺ") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print(Term("�g�o") & " -" & VB6.Format(1000 * td.FeatureLevel("�g�o�ቺ")) & "  ")
		End If
		If td.IsFeatureAvailable("�d�m�ቺ") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print(Term("�d�m") & " -" & VB6.Format(10 * td.FeatureLevel("�d�m�ቺ")) & "  ")
		End If
		If td.IsFeatureAvailable("�g�o�ቺ") Or td.IsFeatureAvailable("�d�m�ቺ") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print()
		End If
		
		'ADD START 240a
		If NewGUIMode Then
			'UPGRADE_ISSUE: PictureBox �v���p�e�B pic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.CurrentX = 5
		End If
		'ADD  END  240a
		'���C
		If td.IsFeatureAvailable("���C") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print("���CLv" & VB6.Format(td.FeatureLevel("���C")))
		End If
		' ADD START MARGE
		'��Ԉُ�t��
		If td.IsFeatureAvailable("��ԕt��") Then
			'UPGRADE_ISSUE: PictureBox ���\�b�h pic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			pic.Print(td.FeatureData("��ԕt��") & "��ԕt��")
		End If
		' ADD END MARGE
		
		'�t�H���g�T�C�Y�����ɖ߂��Ă���
		pic.Font = VB6.FontChangeSize(pic.Font, 9)
	End Sub
	
	'���j�b�g�X�e�[�^�X��\��
	'pindex�̓X�e�[�^�X�\���Ɏg���p�C���b�g���w��
	Public Sub DisplayUnitStatus(ByRef u As Unit, Optional ByVal pindex As Short = 0)
		Dim p As Pilot
		Dim k, i, j, n As Short
		Dim ret As Integer
		Dim buf As String
		Dim fdata, fname, opt As String
		Dim stype, sname, slevel As String
		Dim cx, cy As Short
		Dim warray() As Short
		Dim wpower() As Integer
		Dim ppic, upic As System.Windows.Forms.PictureBox
		Dim nmorale, ecost, pmorale As Short
		Dim flist() As String
		Dim is_unknown As Boolean
		Dim prob, w, cprob As Short
		Dim dmg As Integer
		Dim def_mode As String
		Dim name_list() As String
		'ADD START 240a
		Dim lineStart, color, lineEnd As Integer
		Dim isNoSp As Boolean
		isNoSp = False
		'ADD  END  240a
		'�X�e�[�^�X��ʂ̍X�V���ꎞ��~����Ă���ꍇ�͂��̂܂܏I��
		If IsStatusWindowDisabled Then
			Exit Sub
		End If
		
		'�j��A�j�����ꂽ���j�b�g�͕\�����Ȃ�
		If u.Status_Renamed = "�j��" Or u.Status_Renamed = "�j��" Then
			Exit Sub
		End If
		
		DisplayedUnit = u
		DisplayedPilotInd = pindex
		
		'MOD START MARGE
		'    If MainWidth = 15 Then
		If Not NewGUIMode Then
			'MOD  END  MARGE
			'UPGRADE_ISSUE: Control picPilotStatus �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			ppic = MainForm.picPilotStatus
			'UPGRADE_ISSUE: Control picUnitStatus �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			upic = MainForm.picUnitStatus
			'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Cls �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			ppic.Cls()
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Cls �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Cls()
		Else
			'UPGRADE_ISSUE: Control picUnitStatus �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			ppic = MainForm.picUnitStatus
			'UPGRADE_ISSUE: Control picUnitStatus �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			upic = MainForm.picUnitStatus
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Cls �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Cls()
			'ADD START 240a
			'global�ϐ��ƃX�e�[�^�X�`��p�̕ϐ��𓯊�
			GlobalVariableLoad()
			'�V�f�t�h�ł͒n�`�\�������Ƃ��ɃT�C�Y��ς��Ă���̂Ō��ɖ߂�
			upic.SetBounds(VB6.TwipsToPixelsX(MainPWidth - 240), VB6.TwipsToPixelsY(10), VB6.TwipsToPixelsX(235), VB6.TwipsToPixelsY(MainPHeight - 20))
			upic.BackColor = System.Drawing.ColorTranslator.FromOle(StatusWindowBackBolor)
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.DrawWidth �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.DrawWidth = StatusWindowFrameWidth
			color = StatusWindowFrameColor
			lineStart = (StatusWindowFrameWidth - 1) / 2
			lineEnd = (StatusWindowFrameWidth + 1) / 2
			'UPGRADE_ISSUE: �萔 vbFSTransparent �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.FillStyle �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.FillStyle = vbFSTransparent
			'�g��������
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (lineStart, lineStart) - (235 - lineEnd, MainPHeight - 20 - lineEnd), color, B
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.FillStyle �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.FillStyle = ObjFillStyle
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.CurrentX = 5
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.CurrentY = 5
			'�����F�����Z�b�g
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(StatusFontColorNormalString)
			'ADD  END
		End If
		
		Dim td As TerrainData
		Dim wclass As String
		With u
			'�����X�V
			.Update()
			
			'���m�F���j�b�g���ǂ������肵�Ă���
			If (IsOptionDefined("���j�b�g���B��") And (Not .IsConditionSatisfied("���ʍς�") And (.Party0 = "�G" Or .Party0 = "����"))) Or .IsConditionSatisfied("���j�b�g���B��") Then
				is_unknown = True
			End If
			
			'�p�C���b�g������Ă��Ȃ��H
			If .CountPilot = 0 Then
				'�L������ʂ��N���A
				If MainWidth = 15 Then
					'UPGRADE_ISSUE: Control picFace �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
					MainForm.picFace = System.Drawing.Image.FromFile("")
				Else
					DrawPicture("white.bmp", 2, 2, 64, 64, 0, 0, 0, 0, "�X�e�[�^�X")
				End If
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 150)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				ppic.Print(Term("���x��", u))
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				ppic.Print(Term("�C��", u))
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 0)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("�i��", u, 4) & "               " & Term("�ˌ�", u))
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("����", u, 4) & "               " & Term("���", u))
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("�Z��", u, 4) & "               " & Term("����", u))
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print()
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print()
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				
				GoTo UnitStatus
			End If
			
			'�\������p�C���b�g��I��
			If pindex = 0 Then
				'���C���p�C���b�g
				p = .MainPilot
				If .MainPilot.Nickname = .Pilot(1).Nickname Or .Data.PilotNum = 1 Then
					DisplayedPilotInd = 1
				End If
			ElseIf pindex = 1 Then 
				'���C���p�C���b�g�܂��͂P�Ԗڂ̃p�C���b�g
				If .MainPilot.Nickname <> .Pilot(1).Nickname And .Data.PilotNum <> 1 Then
					p = .Pilot(1)
				Else
					p = .MainPilot
				End If
			ElseIf pindex <= .CountPilot Then 
				'�T�u�p�C���b�g
				p = .Pilot(pindex)
			ElseIf pindex <= .CountPilot + .CountSupport Then 
				'�T�|�[�g�p�C���b�g
				p = .Support(pindex - .CountPilot)
			Else
				'�ǉ��T�|�[�g
				p = .AdditionalSupport
			End If
			
			With p
				'�����X�V
				.UpdateSupportMod()
				
				'�p�C���b�g�摜��\��
				fname = "\Bitmap\Pilot\" & .Bitmap
				If frmMultiSelectListBox.Visible Then
					'�U�R���ėp�p�C���b�g����郆�j�b�g�̏o���I�����̓p�C���b�g�摜��
					'����Ƀ��j�b�g�摜��\��
					If InStr(.Name, "(�U�R)") > 0 Or InStr(.Name, "(�ėp)") > 0 Then
						fname = "\Bitmap\Unit\" & u.Bitmap
					End If
				End If
				
				'�摜�t�@�C��������
				If InStr(fname, "\-.bmp") > 0 Then
					fname = ""
				ElseIf FileExists(ScenarioPath & fname) Then 
					fname = ScenarioPath & fname
				ElseIf FileExists(ExtDataPath & fname) Then 
					fname = ExtDataPath & fname
				ElseIf FileExists(ExtDataPath2 & fname) Then 
					fname = ExtDataPath2 & fname
				ElseIf FileExists(AppPath & fname) Then 
					fname = AppPath & fname
				Else
					'�摜��������Ȃ��������Ƃ��L�^
					If InStr(fname, "\Pilot\") > 0 Then
						If .Bitmap = .Data.Bitmap Then
							.Data.IsBitmapMissing = True
						End If
					End If
					fname = ""
				End If
				
				'�摜�t�@�C����ǂݍ���ŕ\��
				If MainWidth = 15 Then
					If fname <> "" Then
						On Error GoTo ErrorHandler
						'UPGRADE_ISSUE: Control picTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picTmp = System.Drawing.Image.FromFile(fname)
						On Error GoTo 0
						'UPGRADE_ISSUE: Control picTmp �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						'UPGRADE_ISSUE: Control picFace �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picFace.PaintPicture(MainForm.picTmp.Picture, 0, 0, 64, 64)
					Else
						'�摜�t�@�C����������Ȃ������ꍇ�̓L������ʂ��N���A
						'UPGRADE_ISSUE: Control picFace �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
						MainForm.picFace = System.Drawing.Image.FromFile("")
					End If
				Else
					If fname <> "" Then
						DrawPicture(fname, 2, 2, 64, 64, 0, 0, 0, 0, "�X�e�[�^�X")
					Else
						'�摜�t�@�C����������Ȃ������ꍇ�̓L������ʂ��N���A
						DrawPicture("white.bmp", 2, 2, 64, 64, 0, 0, 0, 0, "�X�e�[�^�X")
					End If
				End If
				
				'�p�C���b�g����
				ppic.Font = VB6.FontChangeSize(ppic.Font, 10.5)
				ppic.Font = VB6.FontChangeBold(ppic.Font, False)
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END
					'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				ppic.Print(.Nickname)
				ppic.Font = VB6.FontChangeBold(ppic.Font, False)
				ppic.Font = VB6.FontChangeSize(ppic.Font, 10)
				
				'�_�~�[�p�C���b�g�H
				If .Nickname0 = "�p�C���b�g�s��" Then
					'MOD START 240a
					'                ppic.ForeColor = rgb(0, 0, 150)
					ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'MOD START 240a
					'            If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.Print(Term("���x��", u))
					'MOD START 240a
					'            If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.Print(Term("�C��", u))
					'MOD START 240a
					'                ppic.ForeColor = rgb(0, 0, 0)
					ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'MOD START 240a
					'            If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(Term("�i��", u, 4) & "               " & Term("�ˌ�", u))
					'MOD START 240a
					'            If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(Term("����", u, 4) & "               " & Term("���", u))
					'MOD START 240a
					'            If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(Term("�Z��", u, 4) & "               " & Term("����", u))
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
					'MOD START 240a
					'               upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					
					GoTo UnitStatus
				End If
				'���x���A�o���l�A�s����
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 150)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'MOD START 240a
				'            If MainWidth <> 15 Then
				If NewGUIMode Then
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				ppic.Print(Term("���x��", u) & " ")
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 0)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If .Party = "����" Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.Print(VB6.Format(.Level) & " (" & .Exp & ")")
					Select Case u.Action
						Case 2
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 200)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
							'MOD  END  240a
							'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							ppic.Print(" �v")
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 0)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
							'MOD  END  240a
						Case 3
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 200)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
							'MOD  END  240a
							'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							ppic.Print(" �s")
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 0)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
							'MOD  END  240a
					End Select
				Else
					If Not is_unknown Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.Print(VB6.Format(.Level))
						If u.Action = 2 Then
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 200)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
							'MOD  END  240a
							'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							ppic.Print(" �v")
							'MOD START 240a
							'                        ppic.ForeColor = rgb(0, 0, 0)
							ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
							'MOD  END  240a
						End If
					Else
						'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.Print("�H")
					End If
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				ppic.Print()
				
				'�C��
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 150)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				If MainWidth <> 15 Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.CurrentX = 68
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				ppic.Print(Term("�C��", u) & " ")
				'MOD START 240a
				'            ppic.ForeColor = rgb(0, 0, 0)
				ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If Not is_unknown Then
					If .MoraleMod > 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.Print(VB6.Format(.Morale) & "+" & VB6.Format(.MoraleMod) & " (" & .Personality & ")")
					Else
						'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.Print(VB6.Format(.Morale) & " (" & .Personality & ")")
					End If
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.Print("�H")
				End If
				
				'�r�o
				If .MaxSP > 0 Then
					'MOD START 240a
					'                ppic.ForeColor = rgb(0, 0, 150)
					ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					If MainWidth <> 15 Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.Print(Term("�r�o", u) & " ")
					'MOD START 240a
					'                ppic.ForeColor = rgb(0, 0, 0)
					ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					If Not is_unknown Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.Print(VB6.Format(.SP) & "/" & VB6.Format(.MaxSP))
					Else
						'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.Print("�H")
					End If
				Else
					isNoSp = True
				End If
				
				'�g�p���̃X�y�V�����p���[�ꗗ
				If Not is_unknown Then
					'MOD START 240a
					'                ppic.ForeColor = rgb(0, 0, 0)
					ppic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					'MOD START 240a
					'                If MainWidth <> 15 Then
					If NewGUIMode Then
						'MOD  END
						'UPGRADE_ISSUE: PictureBox �v���p�e�B ppic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.CurrentX = 68
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.Print(u.SpecialPowerInEffect)
					'ADD START 240a
				Else
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						ppic.Print(" ")
					End If
					'ADD  END  240a
				End If
				'ADD START 240a
				If isNoSp Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h ppic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					ppic.Print(" ")
				End If
				
				'upic�𖾎��I�ɏ�����
				upic.Font = VB6.FontChangeBold(upic.Font, False)
				upic.Font = VB6.FontChangeSize(upic.Font, 9)
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'�i��
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("�i��", u, 4) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("�H", 4) & Space(10))
				ElseIf .Data.Infight > 1 Then 
					Select Case .InfightMod + .InfightMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.InfightBase), 5) & RightPaddedString("+" & VB6.Format(.InfightMod + .InfightMod2), 9))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.InfightBase), 5) & RightPaddedString(VB6.Format(.InfightMod + .InfightMod2), 9))
						Case 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.Infight), 5) & Space(9))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("--", 5) & Space(9))
				End If
				
				'�ˌ�
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				If Not .HasMana() Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(Term("�ˌ�", u, 4) & " ")
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(Term("����", u, 4) & " ")
				End If
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("�H", 4))
				ElseIf .Data.Shooting > 1 Then 
					Select Case .ShootingMod + .ShootingMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.ShootingBase), 5) & RightPaddedString("+" & VB6.Format(.ShootingMod + .ShootingMod2), 5))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.ShootingBase), 5) & RightPaddedString(VB6.Format(.ShootingMod + .ShootingMod2), 5))
						Case 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.Shooting), 5) & Space(5))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("--", 5) & Space(5))
				End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'����
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("����", u, 4) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("�H", 4) & Space(10))
				ElseIf .Data.Hit > 1 Then 
					Select Case .HitMod + .HitMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.HitBase), 5) & RightPaddedString("+" & VB6.Format(.HitMod + .HitMod2), 9))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.HitBase), 5) & RightPaddedString(VB6.Format(.HitMod + .HitMod2), 9))
						Case 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.Hit), 5) & Space(9))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("--", 5) & Space(9))
				End If
				
				'���
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("���", u, 4) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("�H", 4))
				ElseIf .Data.Dodge > 1 Then 
					Select Case .DodgeMod + .DodgeMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.DodgeBase), 5) & RightPaddedString("+" & VB6.Format(.DodgeMod + .DodgeMod2), 9))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.DodgeBase), 5) & RightPaddedString(VB6.Format(.DodgeMod + .DodgeMod2), 9))
						Case 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.Dodge), 5) & Space(9))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("--", 5) & Space(9))
				End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'�Z��
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("�Z��", u, 4) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("�H", 4) & Space(10))
				ElseIf .Data.Technique > 1 Then 
					Select Case .TechniqueMod + .TechniqueMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.TechniqueBase), 5) & RightPaddedString("+" & VB6.Format(.TechniqueMod + .TechniqueMod2), 9))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.TechniqueBase), 5) & RightPaddedString(VB6.Format(.TechniqueMod + .TechniqueMod2), 9))
						Case 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.Technique), 5) & Space(9))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("--", 5) & Space(9))
				End If
				
				'����
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("����", u, 4) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				If is_unknown Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("�H", 4))
				ElseIf .Data.Intuition > 1 Then 
					Select Case .IntuitionMod + .IntuitionMod2
						Case Is > 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.IntuitionBase), 5) & RightPaddedString("+" & VB6.Format(.IntuitionMod + .IntuitionMod2), 9))
						Case Is < 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.IntuitionBase), 5) & RightPaddedString(VB6.Format(.IntuitionMod + .IntuitionMod2), 9))
						Case 0
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(LeftPaddedString(VB6.Format(.Intuition), 5) & Space(9))
					End Select
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString("--", 5) & Space(9))
				End If
				
				If IsOptionDefined("�h��͐���") Or IsOptionDefined("�h��̓��x���A�b�v") Then
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.CurrentX = 5
					End If
					'�h��
					'MOD START 240a
					'               upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(Term("�h��", u) & " ")
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					If is_unknown Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(LeftPaddedString("�H", 4))
					ElseIf Not .IsSupport(u) Then 
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(LeftPaddedString(VB6.Format(.Defense), 5))
					Else
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(LeftPaddedString("--", 5))
					End If
				End If
			End With
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'���L����X�y�V�����p���[�ꗗ
			With p
				If .CountSpecialPower > 0 Then
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(Term("�X�y�V�����p���[", u, 18) & " ")
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					If Not is_unknown Then
						For i = 1 To .CountSpecialPower
							If .SP < .SpecialPowerCost(.SpecialPower(i)) Then
								'MOD START 240a
								'                            upic.ForeColor = rgb(150, 0, 0)
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
								'MOD  END  240a
								
							End If
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(SPDList.Item(.SpecialPower(i)).ShortName)
							'MOD START 240a
							'                        upic.ForeColor = rgb(0, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
							'MOD  END  240a
						Next 
					Else
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("�H")
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
				End If
			End With
			
			'�����ʂ̃��j�b�g�͂���ȍ~�̏���\�����Ȃ�
			If is_unknown Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentY = upic.CurrentY + 8
				GoTo UnitStatus
			End If
			
			'�p�C���b�g�p����\�͈ꗗ
			With p
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'���
				If .MaxPlana > 0 Then
					If .IsSkillAvailable("���") Then
						sname = .SkillName("���")
					Else
						'�ǉ��p�C���b�g�͑�P�p�C���b�g�̗�͂����Ɏg���̂�
						sname = u.Pilot(1).SkillName("���")
					End If
					If InStr(sname, "��\��") = 0 Then
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 150)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
						'MOD  END  240a
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(sname & " ")
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 0)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
						'MOD  END  240a
						If u.PlanaLevel() < .Plana Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(VB6.Format(.Plana) & "/" & VB6.Format(.MaxPlana))
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 0)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
						'MOD  END  240a
					End If
				End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'������
				If .SynchroRate() > 0 Then
					If InStr(.SkillName("������"), "��\��") = 0 Then
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 150)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
						'MOD  END  240a
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(.SkillName("������") & " ")
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 0)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
						'MOD  END  240a
						If u.SyncLevel() < .SynchroRate Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(VB6.Format(.SynchroRate) & "%")
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 0)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
						'MOD  END  240a
					End If
				End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'���ӋZ���s����
				n = 0
				If .IsSkillAvailable("���ӋZ") Then
					n = n + 1
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("���ӋZ ")
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(RightPaddedString(.SkillData("���ӋZ"), 12))
				End If
				If .IsSkillAvailable("�s����") Then
					n = n + 1
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("�s���� ")
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(.SkillData("�s����"))
				End If
				If n > 0 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
				End If
				
				'�\������p�C���b�g�\�͂̃��X�g���쐬
				ReDim name_list(.CountSkill)
				For i = 1 To .CountSkill
					name_list(i) = .Skill(i)
				Next 
				'�t�����ꂽ�p�C���b�g����\��
				For i = 1 To u.CountCondition
					If u.ConditionLifetime(i) <> 0 Then
						Select Case Right(u.Condition(i), 3)
							Case "�t���Q", "�����Q"
								Select Case LIndex(u.ConditionData(i), 1)
									Case "��\��", "���"
										'��\���̔\��
									Case Else
										stype = Left(u.Condition(i), Len(u.Condition(i)) - 3)
										Select Case stype
											Case "�n���^�[", "�r�o�����", "�X�y�V�����p���[��������"
												'�d���\�Ȕ\��
												ReDim Preserve name_list(UBound(name_list) + 1)
												name_list(UBound(name_list)) = stype
											Case Else
												'���ɏ��L���Ă���\�͂ł���΃X�L�b�v
												For j = 1 To UBound(name_list)
													If stype = name_list(j) Then
														Exit For
													End If
												Next 
												If j > UBound(name_list) Then
													ReDim Preserve name_list(UBound(name_list) + 1)
													name_list(UBound(name_list)) = stype
												End If
										End Select
								End Select
						End Select
					End If
				Next 
				
				'�p�C���b�g�\�͂�\��
				n = 0
				For i = 1 To UBound(name_list)
					'ADD START 240a
					'�����F�����Z�b�g
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'ADD  END  240a
					stype = name_list(i)
					If i <= .CountSkill Then
						sname = .SkillName(i)
						slevel = CStr(.SkillLevel(i))
					Else
						sname = .SkillName(stype)
						slevel = CStr(.SkillLevel(stype))
					End If
					
					If InStr(sname, "��\��") > 0 Then
						GoTo NextSkill
					End If
					
					Select Case stype
						Case "�I�[��"
							If DisplayedPilotInd = 1 Then
								If u.AuraLevel(True) < u.AuraLevel() And MapFileName <> "" Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								If u.AuraLevel(True) > CDbl(slevel) Then
									sname = sname & "+" & VB6.Format(u.AuraLevel(True) - CDbl(slevel))
								End If
							End If
							
						Case "���\��"
							If DisplayedPilotInd = 1 Then
								If u.PsychicLevel(True) < u.PsychicLevel() And MapFileName <> "" Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								If u.PsychicLevel(True) > CDbl(slevel) Then
									sname = sname & "+" & VB6.Format(u.PsychicLevel(True) - CDbl(slevel))
								End If
							End If
							
						Case "���", "�����", "�o��"
							If u.HP <= u.MaxHP \ 4 Then
								'MOD START 240a
								'                            upic.ForeColor = vbBlue
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
								'MOD  END  240a
							End If
							
						Case "�s��"
							If u.HP <= u.MaxHP \ 2 Then
								'MOD START 240a
								'                            upic.ForeColor = vbBlue
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
								'MOD  END  240a
							End If
							
						Case "���ݗ͊J��"
							If .Morale >= 130 Then
								'MOD START 240a
								'                            upic.ForeColor = vbBlue
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
								'MOD  END  240a
							End If
							
						Case "�X�y�V�����p���[��������"
							If i <= .CountSkill Then
								If .Morale >= StrToLng(LIndex(.SkillData(i), 3)) Then
									'MOD START 240a
									'                            upic.ForeColor = vbBlue
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
									'MOD  END  240a
								End If
							Else
								If .Morale >= StrToLng(LIndex(.SkillData(stype), 3)) Then
									'MOD START 240a
									'                            upic.ForeColor = vbBlue
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
									'MOD  END  240a
								End If
							End If
							
						Case "�r�h��"
							If Not u.IsFeatureAvailable("�V�[���h") And Not u.IsFeatureAvailable("��^�V�[���h") And Not u.IsFeatureAvailable("���^�V�[���h") And Not u.IsFeatureAvailable("�G�l���M�[�V�[���h") And Not u.IsFeatureAvailable("�A�N�e�B�u�V�[���h") And Not u.IsFeatureAvailable("��") And Not u.IsFeatureAvailable("�o���A�V�[���h") And Not u.IsFeatureAvailable("�A�N�e�B�u�t�B�[���h") And Not u.IsFeatureAvailable("�A�N�e�B�u�v���e�N�V����") And InStr(u.FeatureData("�j�~"), "�r�h��") = 0 And InStr(u.FeatureData("�L��j�~"), "�r�h��") = 0 And InStr(u.FeatureData("����"), "�r�h��") = 0 And InStr(u.FeatureData("���Đg�Z"), "�r�h��") = 0 And InStr(u.FeatureData("��������"), "�r�h��") = 0 And MapFileName <> "" Then
								'MOD START 240a
								'                            upic.ForeColor = rgb(150, 0, 0)
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
								'MOD  END  240a
							End If
							
						Case "�؂蕥��"
							For j = 1 To u.CountWeapon
								If u.IsWeaponClassifiedAs(j, "��") Then
									If Not u.IsDisabled((u.Weapon(j).Name)) Then
										Exit For
									End If
								End If
							Next 
							If u.IsFeatureAvailable("�i������") Then
								j = 0
							End If
							If j > u.CountWeapon And InStr(u.FeatureData("�j�~"), "�؂蕥��") = 0 And InStr(u.FeatureData("�L��j�~"), "�؂蕥��") = 0 And InStr(u.FeatureData("����"), "�؂蕥��") = 0 And InStr(u.FeatureData("���Đg�Z"), "�؂蕥��") = 0 And InStr(u.FeatureData("��������"), "�؂蕥��") = 0 And MapFileName <> "" Then
								'MOD START 240a
								'                            upic.ForeColor = rgb(150, 0, 0)
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
								'MOD  END  240a
							End If
							
						Case "�}��"
							For j = 1 To u.CountWeapon
								If u.IsWeaponAvailable(j, "�ړ���") And u.IsWeaponClassifiedAs(j, "�ˌ��n") And (u.Weapon(j).Bullet >= 10 Or (u.Weapon(j).Bullet = 0 And u.Weapon(j).ENConsumption <= 5)) Then
									Exit For
								End If
							Next 
							If u.IsFeatureAvailable("�}������") Then
								j = 0
							End If
							If j > u.CountWeapon And InStr(u.FeatureData("�j�~"), "�}��") = 0 And InStr(u.FeatureData("�L��j�~"), "�}��") = 0 And InStr(u.FeatureData("����"), "�}��") = 0 And InStr(u.FeatureData("���Đg�Z"), "�}��") = 0 And InStr(u.FeatureData("��������"), "�}��") = 0 And MapFileName <> "" Then
								'MOD START 240a
								'                            upic.ForeColor = rgb(150, 0, 0)
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
								'MOD  END  240a
							End If
							
						Case "��"
							For j = 1 To u.CountWeapon
								If u.IsWeaponClassifiedAs(j, "��") Then
									If Not u.IsDisabled((u.Weapon(j).Name)) Then
										Exit For
									End If
								End If
							Next 
							If j > u.CountWeapon And MapFileName <> "" Then
								'MOD START 240a
								'                            upic.ForeColor = rgb(150, 0, 0)
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
								'MOD  END  240a
							End If
							
						Case "����"
							If MapFileName <> "" Then
								If u.Party = Stage Then
									ret = MaxLng(u.MaxSupportAttack - u.UsedSupportAttack, 0)
								Else
									If u.IsUnderSpecialPowerEffect("�T�|�[�g�K�[�h�s�\") Then
										'MOD START 240a
										'                                    upic.ForeColor = rgb(150, 0, 0)
										upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
										'MOD  END  240a
									End If
									ret = MaxLng(u.MaxSupportGuard - u.UsedSupportGuard, 0)
								End If
								If ret = 0 Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								sname = sname & " (�c��" & VB6.Format(ret) & "��)"
							End If
							
						Case "����U��"
							If MapFileName <> "" Then
								ret = MaxLng(u.MaxSupportAttack - u.UsedSupportAttack, 0)
								If ret = 0 Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								sname = sname & " (�c��" & VB6.Format(ret) & "��)"
							End If
							
						Case "����h��"
							If MapFileName <> "" Then
								ret = MaxLng(u.MaxSupportGuard - u.UsedSupportGuard, 0)
								If ret = 0 Or u.IsUnderSpecialPowerEffect("�T�|�[�g�K�[�h�s�\") Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								sname = sname & " (�c��" & VB6.Format(ret) & "��)"
							End If
							
						Case "����"
							If MapFileName <> "" Then
								ret = MaxLng(u.MaxSyncAttack - u.UsedSyncAttack, 0)
								If ret = 0 Then
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
								End If
								sname = sname & " (�c��" & VB6.Format(ret) & "��)"
							End If
							
						Case "�J�E���^�["
							If MapFileName <> "" Then
								ret = MaxLng(u.MaxCounterAttack - u.UsedCounterAttack, 0)
								If ret > 100 Then
									sname = sname & " (�c�聇��)"
								ElseIf ret > 0 Then 
									sname = sname & " (�c��" & VB6.Format(ret) & "��)"
								Else
									'MOD START 240a
									'                                upic.ForeColor = rgb(150, 0, 0)
									upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
									'MOD  END  240a
									sname = sname & " (�c��0��)"
								End If
							End If
							
						Case "���K��"
							If u.MaxCounterAttack > 100 Then
								'MOD START 240a
								'                            upic.ForeColor = vbBlue
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
								'MOD  END  240a
							End If
							
						Case "�ϋv"
							If IsOptionDefined("�h��͐���") Or IsOptionDefined("�h��̓��x���A�b�v") Then
								GoTo NextSkill
							End If
							
						Case "���", "������", "���ӋZ", "�s����"
							GoTo NextSkill
							
					End Select
					
					'����\�͖���\��
					'UPGRADE_ISSUE: �萔 vbFromUnicode �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
					'UPGRADE_ISSUE: LenB �֐��̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ���N���b�N���Ă��������B
					If LenB(StrConv(sname, vbFromUnicode)) > 19 Then
						If n > 0 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print()
							'ADD START 240a
							If NewGUIMode Then
								'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
								upic.CurrentX = 5
							End If
							'ADD  END  240a
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(sname)
						n = 2
					Else
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(RightPaddedString(sname, 19))
						n = n + 1
					End If
					upic.ForeColor = System.Drawing.Color.Black
					
					'�K�v�ɉ����ĉ��s
					If n > 1 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print()
						'ADD START 240a
						If NewGUIMode Then
							'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.CurrentX = 5
						End If
						'ADD  END  240a
						n = 0
					End If
NextSkill: 
				Next 
			End With
			
			If n > 0 Then
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print()
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
			End If
			
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.CurrentY = upic.CurrentY + 8
			
UnitStatus: 
			
			'�p�C���b�g�X�e�[�^�X�\���p�̃_�~�[���j�b�g�̏ꍇ�͂����ŕ\�����I��
			If .IsFeatureAvailable("�_�~�[���j�b�g") Then
				GoTo UpdateStatusWindow
			End If
			
			'��������̓��j�b�g�Ɋւ�����
			
			'���j�b�g����
			upic.Font = VB6.FontChangeSize(upic.Font, 10.5)
			upic.Font = VB6.FontChangeBold(upic.Font, False)
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
				'�����F�����Z�b�g
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(StatusFontColorNormalString)
			End If
			'ADD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(.Nickname0)
			upic.Font = VB6.FontChangeBold(upic.Font, False)
			upic.Font = VB6.FontChangeSize(upic.Font, 9)
			
			If .Status_Renamed = "�o��" And MapFileName <> "" Then
				
				'�n�`���̕\��
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'���j�b�g�̈ʒu��n�`����
				If InStr(TerrainName(.X, .Y), "(") > 0 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(.Area & " (" & Left(TerrainName(.X, .Y), InStr(TerrainName(.X, .Y), "(") - 1))
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(.Area & " (" & TerrainName(.X, .Y))
				End If
				
				'������h��C��
				If TerrainEffectForHit(.X, .Y) = TerrainEffectForDamage(.X, .Y) Then
					If TerrainEffectForHit(.X, .Y) >= 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(" �񁕖h+" & VB6.Format(TerrainEffectForHit(.X, .Y)) & "%")
					Else
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(" �񁕖h" & VB6.Format(TerrainEffectForHit(.X, .Y)) & "%")
					End If
				Else
					If TerrainEffectForHit(.X, .Y) >= 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(" ��+" & VB6.Format(TerrainEffectForHit(.X, .Y)) & "%")
					Else
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(" ��" & VB6.Format(TerrainEffectForHit(.X, .Y)) & "%")
					End If
					If TerrainEffectForDamage(.X, .Y) >= 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(" �h+" & VB6.Format(TerrainEffectForDamage(.X, .Y)) & "%")
					Else
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(" �h" & VB6.Format(TerrainEffectForDamage(.X, .Y)) & "%")
					End If
				End If
				
				'�g�o���d�m��
				If TerrainEffectForHPRecover(.X, .Y) > 0 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" " & Left(Term("�g�o"), 1) & "+" & VB6.Format(TerrainEffectForHPRecover(.X, .Y)) & "%")
				End If
				If TerrainEffectForENRecover(.X, .Y) > 0 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" " & Left(Term("�d�m"), 1) & "+" & VB6.Format(TerrainEffectForENRecover(.X, .Y)) & "%")
				End If
				
				'MOD START 240a
				'            Set td = TDList.Item(MapData(.X, .Y, 0))
				'�}�X�̃^�C�v�ɉ����ĎQ�Ɛ��ύX
				Select Case MapData(.X, .Y, Map.MapDataIndex.BoxType)
					Case Map.BoxTypes.Under, Map.BoxTypes.UpperBmpOnly
						td = TDList.Item(MapData(.X, .Y, Map.MapDataIndex.TerrainType))
					Case Else
						td = TDList.Item(MapData(.X, .Y, Map.MapDataIndex.LayerType))
				End Select
				'MOD START 240a
				'�g�o���d�m����
				If td.IsFeatureAvailable("�g�o����") Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" " & Left(Term("�g�o"), 1) & "-" & VB6.Format(10 * td.FeatureLevel("�g�o����")) & "%")
				End If
				If td.IsFeatureAvailable("�d�m����") Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" " & Left(Term("�d�m"), 1) & "-" & VB6.Format(10 * td.FeatureLevel("�d�m����")) & "%")
				End If
				
				'�g�o���d�m����
				If td.IsFeatureAvailable("�g�o����") Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" " & Left(Term("�g�o"), 1) & "+" & VB6.Format(1000 * td.FeatureLevel("�g�o����")))
				End If
				If td.IsFeatureAvailable("�d�m����") Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" " & Left(Term("�d�m"), 1) & "+" & VB6.Format(10 * td.FeatureLevel("�d�m����")))
				End If
				
				'�g�o���d�m�ቺ
				If td.IsFeatureAvailable("�g�o�ቺ") Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" " & Left(Term("�g�o"), 1) & "-" & VB6.Format(1000 * td.FeatureLevel("�g�o�ቺ")))
				End If
				If td.IsFeatureAvailable("�d�m�ቺ") Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" " & Left(Term("�d�m"), 1) & "-" & VB6.Format(10 * td.FeatureLevel("�d�m�ቺ")))
				End If
				
				'���C
				If td.IsFeatureAvailable("���C") Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" ��L" & VB6.Format(td.FeatureLevel("���C")))
				End If
				
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(")")
			Else
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("�����N ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(VB6.Format(.Rank))
			End If
			
			'���m�F���j�b�g�H
			If is_unknown Then
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'�g�o
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("�g�o", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("?????/?????")
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'�d�m
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("�d�m", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("???/???")
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'���b
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("���b", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(RightPaddedString("�H", 12))
				
				'�^����
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("�^����", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("�H")
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'�ړ��^�C�v
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("�^�C�v", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(RightPaddedString("�H", 12))
				
				'�ړ���
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("�ړ���", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("�H")
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'�n�`�K��
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("�K��   ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(RightPaddedString("�H", 12))
				
				'���j�b�g�T�C�Y
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(Term("�T�C�Y", Nothing, 6) & " ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("�H")
				
				'�T�|�[�g�A�^�b�N�𓾂��邩�ǂ����̂ݕ\��
				If (CommandState = "�^�[�Q�b�g�I��" Or CommandState = "�ړ���^�[�Q�b�g�I��") And (SelectedCommand = "�U��" Or SelectedCommand = "�}�b�v�U��") And Not SelectedUnit Is Nothing Then
					If .Party = "�G" Or .Party = "����" Or .IsConditionSatisfied("�\��") Or .IsConditionSatisfied("����") Or .IsConditionSatisfied("�߈�") Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print()
						
						'�U����i
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 150)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
						'MOD  END  240a
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("�U��     ")
						'MOD START 240a
						'                   upic.ForeColor = rgb(0, 0, 0)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
						'MOD  END  240a
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(SelectedUnit.WeaponNickname(SelectedWeapon))
						'�T�|�[�g�A�^�b�N�𓾂���H
						If Not SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "��") And Not SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "�l") Then
							If Not SelectedUnit.LookForSupportAttack(u) Is Nothing Then
								'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
								upic.Print(" [��]")
							End If
						End If
					End If
				End If
				
				GoTo UpdateStatusWindow
			End If
			
			'���s���̖���
			If .Party = "�m�o�b" And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("���|") And Not .IsConditionSatisfied("�\��") And Not .IsConditionSatisfied("����m") Then
				'�v�l���[�h������Ύ��s���Ă��閽�߂�������̂Łc�c
				buf = ""
				If .IsConditionSatisfied("����") Then
					If Not .Master Is Nothing Then
						If .Master.Party = "����" Then
							buf = .Mode
						End If
					End If
				End If
				If .IsFeatureAvailable("�������j�b�g") And Not .IsConditionSatisfied("����") Then
					If Not .Summoner Is Nothing Then
						If .Summoner.Party = "����" Then
							buf = .Mode
						End If
					End If
				End If
				
				If buf = "�ʏ�" Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("���R�s����")
				ElseIf PList.IsDefined(buf) Then 
					'�v�l���[�h�Ƀp�C���b�g�����w�肳��Ă���ꍇ
					With PList.Item(buf)
						If Not .Unit_Renamed Is Nothing Then
							With .Unit_Renamed
								If .Status_Renamed = "�o��" Then
									'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
									upic.Print(.Nickname & "(" & VB6.Format(.X) & "," & VB6.Format(.Y) & ")��")
									If .Party = "����" Then
										'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
										upic.Print("��q��")
									Else
										'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
										upic.Print("�ǐՒ�")
									End If
								End If
							End With
						End If
					End With
				ElseIf LLength(buf) = 2 Then 
					'�v�l���[�h�ɍ��W���w�肳��Ă���ꍇ
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("(" & LIndex(buf, 1) & "," & LIndex(buf, 2) & ")�Ɉړ���")
				End If
			End If
			
			'���j�b�g�ɂ������Ă������X�e�[�^�X
			ReDim name_list(0)
			For i = 1 To .CountCondition
				'���Ԑ؂�H
				If .ConditionLifetime(i) = 0 Then
					GoTo NextCondition
				End If
				
				'��\���H
				If InStr(.ConditionData(i), "��\��") > 0 Then
					GoTo NextCondition
				End If
				
				'����H
				If LIndex(.ConditionData(i), 1) = "���" Then
					GoTo NextCondition
				End If
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				Select Case .Condition(i)
					Case "�f�[�^�s��", "�`�ԌŒ�", "�@�̌Œ�", "�s���g", "���G", "���ʍς�", "�񑀍�", "�j��L�����Z��", "���_���[�W", "�\�̓R�s�[", "���b�Z�[�W�t��", "�m�[�}�����[�h�t��", "�ǉ��p�C���b�g�t��", "�ǉ��T�|�[�g�t��", "�p�C���b�g���̕t��", "�p�C���b�g�摜�t��", "���i�ύX�t��", "���ʕt��", "�a�f�l�t��", "���̕ύX�t��", "�X�y�V�����p���[������", "���_�R�}���h������", "���j�b�g�摜�t��", "���b�Z�[�W�t��"
						'��\��
					Case "�c�莞��"
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print("�c�莞��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
					Case "�������t��", "�ϐ��t��", "�z���t��", "��_�t��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(.ConditionData(i) & .Condition(i))
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" " & VB6.Format(.ConditionLifetime(i)) & "T")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "������ʖ������t��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(.ConditionData(i) & "�������t��")
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "�U�������t��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(LIndex(.ConditionData(i), 1) & "�����t��")
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "���틭���t��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("���틭��Lv" & .ConditionLevel(i) & "�t��")
						If .ConditionData(i) <> "" Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print("(" & .ConditionData(i) & ")")
						End If
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "�����������t��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("����������Lv" & .ConditionLevel(i) & "�t��")
						If .ConditionData(i) <> "" Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print("(" & .ConditionData(i) & ")")
						End If
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "�b�s�������t��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("�b�s������Lv" & .ConditionLevel(i) & "�t��")
						If .ConditionData(i) <> "" Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print("(" & .ConditionData(i) & ")")
						End If
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "������ʔ����������t��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("������ʔ���������Lv" & .ConditionLevel(i) & "�t��")
						If .ConditionData(i) <> "" Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print("(" & .ConditionData(i) & ")")
						End If
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "�n�`�K���ύX�t��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("�n�`�K���ύX�t��")
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "���t��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(LIndex(.ConditionData(i), 1) & "�t��")
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("(" & VB6.Format(.ConditionLevel(i)) & ")")
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "�_�~�[�j��"
						buf = .FeatureName("�_�~�[")
						If InStr(buf, "Lv") > 0 Then
							buf = Left(buf, InStr(buf, "Lv") - 1)
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(buf & StrConv(VB6.Format(.ConditionLevel(i)), VbStrConv.Wide) & "�̔j��")
					Case "�_�~�[�t��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(.FeatureName("�_�~�[") & "�c��" & StrConv(VB6.Format(.ConditionLevel(i)), VbStrConv.Wide) & "��")
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 100 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "�o���A����"
						If .ConditionData(i) <> "" Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(.ConditionData(i))
						Else
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print("�o���A����")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
					Case "�t�B�[���h����"
						If .ConditionData(i) <> "" Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(.ConditionData(i))
						Else
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print("�t�B�[���h����")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
					Case "���b��"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(Term("���b", u) & "��")
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "�^�����t�o"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(Term("�^����", u) & "�t�o")
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "�^�����c�n�v�m"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(Term("�^����", u) & "�c�n�v�m")
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "�ړ��͂t�o"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(Term("�ړ���", u) & "�t�o")
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case "�ړ��͂c�n�v�m"
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(Term("�ړ���", u) & "�c�n�v�m")
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
					Case Else
						'�[�U���H
						If Right(.Condition(i), 3) = "�[�U��" Then
							If .IsHero() Then
								'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
								upic.Print(Left(.Condition(i), Len(.Condition(i)) - 3))
								'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
								upic.Print("������")
							Else
								'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
								upic.Print(.Condition(i))
							End If
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
							GoTo NextCondition
						End If
						
						'�p�C���b�g����\�͕t���������ɂ���Ԃ͕\�����Ȃ�
						If Right(.Condition(i), 3) = "�t���Q" Or Right(.Condition(i), 3) = "�����Q" Then
							GoTo NextCondition
						End If
						
						If Right(.Condition(i), 2) = "�t��" And .ConditionData(i) <> "" Then
							buf = LIndex(.ConditionData(i), 1) & "�t��"
						ElseIf Right(.Condition(i), 2) = "����" And .ConditionData(i) <> "" Then 
							'�����A�r���e�B
							buf = LIndex(.ConditionData(i), 1) & "����Lv" & .ConditionLevel(i)
						ElseIf .ConditionLevel(i) > 0 Then 
							'�t���A�r���e�B(���x���w�肠��)
							buf = Left(.Condition(i), Len(.Condition(i)) - 2) & "Lv" & VB6.Format(.ConditionLevel(i)) & "�t��"
						Else
							'�t���A�r���e�B(���x���w��Ȃ�)
							buf = .Condition(i)
						End If
						
						'�G���A�X���ꂽ����\�͂̕t���\�������Ԃ�Ȃ��悤��
						For j = 1 To UBound(name_list)
							If buf = name_list(j) Then
								Exit For
							End If
						Next 
						If j <= UBound(name_list) Then
							GoTo NextCondition
						End If
						ReDim Preserve name_list(UBound(name_list) + 1)
						name_list(UBound(name_list)) = buf
						
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print(buf)
						
						If 0 < .ConditionLifetime(i) And .ConditionLifetime(i) < 20 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(" �c��" & VB6.Format(.ConditionLifetime(i)) & "�^�[��")
						End If
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("")
				End Select
NextCondition: 
			Next 
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'�g�o
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			cx = upic.CurrentX
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			cy = upic.CurrentY
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (116, cy + 2) - (118 + GauageWidth, cy + 2), RGB(100, 100, 100)
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (116, cy + 2) - (116, cy + 9), RGB(100, 100, 100)
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (117, cy + 8) - (118 + GauageWidth, cy + 8), RGB(220, 220, 220)
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (118 + GauageWidth, cy + 3) - (118 + GauageWidth, cy + 9), RGB(220, 220, 220)
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (117, cy + 3) - (117 + GauageWidth, cy + 7), RGB(200, 0, 0), BF
			If .HP > 0 Then
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Line (117, cy + 3) - (117 + GauageWidth * .HP \ .MaxHP, cy + 7), RGB(0, 210, 0), BF
			End If
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.CurrentX = cx
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.CurrentY = cy
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(Term("�g�o", u, 6) & " ")
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			If .IsConditionSatisfied("�f�[�^�s��") Then
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("?????/?????")
			Else
				If .HP < 100000 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(VB6.Format(.HP))
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("?????")
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("/")
				If .MaxHP < 100000 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(VB6.Format(.MaxHP))
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("?????")
				End If
			End If
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'�d�m
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			cx = upic.CurrentX
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			cy = upic.CurrentY
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (116, cy + 2) - (118 + GauageWidth, cy + 2), RGB(100, 100, 100)
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (116, cy + 2) - (116, cy + 9), RGB(100, 100, 100)
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (117, cy + 8) - (118 + GauageWidth, cy + 8), RGB(220, 220, 220)
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (118 + GauageWidth, cy + 3) - (118 + GauageWidth, cy + 9), RGB(220, 220, 220)
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Line (117, cy + 3) - (117 + GauageWidth, cy + 7), RGB(200, 0, 0), BF
			If .EN > 0 Then
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Line �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Line (117, cy + 3) - (117 + GauageWidth * .EN \ .MaxEN, cy + 7), RGB(0, 210, 0), BF
			End If
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.CurrentX = cx
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.CurrentY = cy
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(Term("�d�m", u, 6) & " ")
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			If .IsConditionSatisfied("�f�[�^�s��") Then
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("???/???")
			Else
				If .EN < 1000 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(VB6.Format(.EN))
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("???")
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("/")
				If .MaxEN < 1000 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(VB6.Format(.MaxEN))
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("???")
				End If
			End If
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'���b
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(Term("���b", u, 6) & " ")
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			Select Case .Armor("�C���l")
				Case Is > 0
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(RightPaddedString(VB6.Format(.Armor("��{�l")) & "+" & VB6.Format(.Armor("�C���l")), 12))
				Case Is < 0
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(RightPaddedString(VB6.Format(.Armor("��{�l")) & VB6.Format(.Armor("�C���l")), 12))
				Case 0
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(RightPaddedString(VB6.Format(.Armor), 12))
			End Select
			
			'�^����
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(Term("�^����", u, 6) & " ")
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			Select Case .Mobility("�C���l")
				Case Is > 0
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(VB6.Format(.Mobility("��{�l")) & "+" & VB6.Format(.Mobility("�C���l")))
				Case Is < 0
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(VB6.Format(.Mobility("��{�l")) & VB6.Format(.Mobility("�C���l")))
				Case 0
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(VB6.Format(.Mobility))
			End Select
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'�ړ��^�C�v
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(Term("�^�C�v", u, 6) & " ")
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(RightPaddedString(.Transportation, 12))
			
			'�ړ���
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(Term("�ړ���", u, 6) & " ")
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			If .IsFeatureAvailable("�e���|�[�g") And (.Data.Speed = 0 Or LIndex(.FeatureData("�e���|�[�g"), 2) = "0") Then
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(VB6.Format(.Speed + .FeatureLevel("�e���|�[�g")))
			Else
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(VB6.Format(.Speed))
			End If
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'�n�`�K��
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print("�K��   ")
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			For i = 1 To 4
				Select Case .Adaption(i)
					Case 5
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("S")
					Case 4
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("A")
					Case 3
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("B")
					Case 2
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("C")
					Case 1
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("D")
					Case Else
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("E")
				End Select
			Next 
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(Space(8))
			
			'���j�b�g�T�C�Y
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(Term("�T�C�Y", u, 6) & " ")
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(StrConv(.Size, VbStrConv.Wide))
			
			'�h�䑮���̕\��
			n = 0
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'�z��
			If Len(.strAbsorb) > 0 And InStr(.strAbsorb, "��\��") = 0 Then
				If Len(.strAbsorb) > 5 Then
					If n > 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print()
					End If
					n = 2
				End If
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("�z��   ")
				'MOD START 240a
				'        upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(RightPaddedString(.strAbsorb, 12))
				n = n + 1
				If n > 1 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
					'ADD START 240a
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.CurrentX = 5
					End If
					'ADD  END  240a
					n = 0
				End If
			End If
			
			'������
			If Len(.strImmune) > 0 And InStr(.strImmune, "��\��") = 0 Then
				If Len(.strImmune) > 5 Then
					If n > 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print()
						'ADD START 240a
						If NewGUIMode Then
							'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.CurrentX = 5
						End If
						'ADD  END  240a
					End If
					n = 2
				End If
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("������ ")
				'MOD START 240a
				'        upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(RightPaddedString(.strImmune, 12))
				n = n + 1
				If n > 1 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
					'ADD START 240a
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.CurrentX = 5
					End If
					'ADD  END  240a
					n = 0
				End If
			End If
			
			'�ϐ�
			If Len(.strResist) > 0 And InStr(.strResist, "��\��") = 0 Then
				If Len(.strResist) > 5 Then
					If n > 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print()
						'ADD START 240a
						If NewGUIMode Then
							'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.CurrentX = 5
						End If
						'ADD  END  240a
					End If
					n = 2
				End If
				If n = 0 And NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("�ϐ�   ")
				'MOD START 240a
				'        upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(RightPaddedString(.strResist, 12))
				n = n + 1
				If n > 1 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
					'ADD START 240a
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.CurrentX = 5
					End If
					'ADD  END  240a
					n = 0
				End If
			End If
			
			'��_
			If Len(.strWeakness) > 0 And InStr(.strWeakness, "��\��") = 0 Then
				If Len(.strWeakness) > 5 Then
					If n > 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print()
						'ADD START 240a
						If NewGUIMode Then
							'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.CurrentX = 5
						End If
						'ADD  END  240a
					End If
					n = 2
				End If
				If n = 0 And NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("��_   ")
				'MOD START 240a
				'        upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(RightPaddedString(.strWeakness, 12))
				n = n + 1
				If n > 1 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
					'ADD START 240a
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.CurrentX = 5
					End If
					'ADD  END  240a
					n = 0
				End If
			End If
			
			'�L��
			If Len(.strEffective) > 0 And InStr(.strEffective, "��\��") = 0 Then
				If Len(.strEffective) > 5 Then
					If n > 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print()
						'ADD START 240a
						If NewGUIMode Then
							'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.CurrentX = 5
						End If
						'ADD  END  240a
					End If
					n = 2
				End If
				If n = 0 And NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("�L��   ")
				'MOD START 240a
				'        upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(RightPaddedString(.strEffective, 12))
				n = n + 1
				If n > 1 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
					'ADD START 240a
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.CurrentX = 5
					End If
					'ADD  END  240a
					n = 0
				End If
			End If
			
			'������ʖ�����
			If Len(.strSpecialEffectImmune) > 0 And InStr(.strSpecialEffectImmune, "��\��") = 0 Then
				If Len(.strSpecialEffectImmune) > 5 Then
					If n > 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print()
						'ADD START 240a
						If NewGUIMode Then
							'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.CurrentX = 5
						End If
						'ADD  END  240a
					End If
					n = 2
				End If
				If n = 0 And NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("������ ")
				'MOD START 240a
				'        upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(RightPaddedString(.strSpecialEffectImmune, 12))
				n = n + 1
				If n > 1 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
					'ADD START 240a
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.CurrentX = 5
					End If
					'ADD  END  240a
					n = 0
				End If
			End If
			
			'�K�v�ɉ����ĉ��s
			If n > 0 Then
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print()
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
			End If
			n = 0
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'����E�h��N���X
			ReDim flist(0)
			If IsOptionDefined("�A�C�e������") Then
				If .IsFeatureAvailable("����N���X") Or .IsFeatureAvailable("�h��N���X") Then
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.CurrentX = 5
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(RightPaddedString("����E�h��N���X", 19))
					ReDim Preserve flist(1)
					flist(1) = "����E�h��N���X"
					n = n + 1
				End If
			End If
			
			'����\�͈ꗗ��\������O�ɕK�v�C�͔���̂��߃��C���p�C���b�g�̋C�͂��Q��
			If .CountPilot > 0 Then
				pmorale = .MainPilot.Morale
			Else
				pmorale = 150
			End If
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'����\�͈ꗗ
			For i = .AdditionalFeaturesNum + 1 To .CountAllFeature
				fname = .AllFeatureName(i)
				
				'���j�b�g�X�e�[�^�X�R�}���h���͒ʏ�͔�\���̃p�[�c���́A
				'�m�[�}�����[�h�A�������\��
				If fname = "" Then
					If MapFileName = "" Then
						Select Case .AllFeature(i)
							Case "�p�[�c����", "�m�[�}�����[�h"
								'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
								upic.Print(RightPaddedString(.AllFeature(i), 19))
								n = n + 1
								If n > 1 Then
									'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
									upic.Print()
									'ADD START 240a
									If NewGUIMode Then
										'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
										upic.CurrentX = 5
									End If
									'ADD  END  240a
									n = 0
								End If
							Case "����"
								fname = "����"
								
								'�G���A�X�Ŋ����̖��̂��ύX����Ă���H
								With ALDList
									For j = 1 To .Count
										With .Item(j)
											If .AliasType(1) = "����" Then
												fname = .Name
												Exit For
											End If
										End With
									Next 
								End With
								
								'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
								upic.Print(RightPaddedString(fname, 19))
								n = n + 1
								If n > 1 Then
									'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
									upic.Print()
									'ADD START 240a
									If NewGUIMode Then
										'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
										upic.CurrentX = 5
									End If
									'ADD  END  240a
									n = 0
								End If
						End Select
					End If
					GoTo NextFeature
				End If
				
				'���ɕ\�����Ă��邩�𔻒�
				For j = 1 To UBound(flist)
					If fname = flist(j) Then
						GoTo NextFeature
					End If
				Next 
				ReDim Preserve flist(UBound(flist) + 1)
				flist(UBound(flist)) = fname
				
				'�g�p�ۂɂ���ĕ\���F��ς���
				fdata = .AllFeatureData(i)
				Select Case .AllFeature(i)
					Case "����"
						If Not UList.IsDefined(LIndex(fdata, 2)) Then
							GoTo NextFeature
						End If
						If UList.Item(LIndex(fdata, 2)).IsConditionSatisfied("�s���s�\") Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
					Case "����"
						k = 0
						For j = 2 To LLength(fdata)
							If Not UList.IsDefined(LIndex(fdata, j)) Then
								GoTo NextFeature
							End If
							With UList.Item(LIndex(fdata, j)).Data
								If .IsFeatureAvailable("�������j�b�g") Then
									k = k + System.Math.Abs(.PilotNum)
								End If
							End With
						Next 
						If .CountPilot < k Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
					Case "�n�C�p�[���[�h"
						If pmorale < CShort(10# * .FeatureLevel(i)) + 100 And .HP > .MaxHP \ 4 Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						ElseIf .IsConditionSatisfied("�m�[�}�����[�h�t��") Then 
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
					Case "�C�����u", "�⋋���u"
						If IsNumeric(LIndex(fdata, 2)) Then
							If .EN < CShort(LIndex(fdata, 2)) Then
								'MOD START 240a
								'                            upic.ForeColor = rgb(150, 0, 0)
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
								'MOD  END  240a
							End If
						End If
					Case "�e���|�[�g"
						If IsNumeric(LIndex(fdata, 2)) Then
							If .EN < CShort(LIndex(fdata, 2)) Then
								'MOD START 240a
								'                            upic.ForeColor = rgb(150, 0, 0)
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
								'MOD  END  240a
							End If
						Else
							If .EN < 40 Then
								'MOD START 240a
								'                            upic.ForeColor = rgb(150, 0, 0)
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
								'MOD  END  240a
							End If
						End If
					Case "���g"
						If pmorale < 130 Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
					Case "�����"
						If IsNumeric(LIndex(fdata, 2)) Then
							ecost = CShort(LIndex(fdata, 2))
						Else
							ecost = 0
						End If
						If IsNumeric(LIndex(fdata, 3)) Then
							nmorale = CShort(LIndex(fdata, 3))
						Else
							nmorale = 0
						End If
						If .EN < ecost Or pmorale < nmorale Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
					Case "�ً}�e���|�[�g"
						If IsNumeric(LIndex(fdata, 3)) Then
							ecost = CShort(LIndex(fdata, 3))
						Else
							ecost = 0
						End If
						If IsNumeric(LIndex(fdata, 4)) Then
							nmorale = CShort(LIndex(fdata, 4))
						Else
							nmorale = 0
						End If
						If .EN < ecost Or pmorale < nmorale Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
					Case "�G�l���M�[�V�[���h"
						If .EN < 5 Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
					Case "�o���A", "�o���A�V�[���h", "�v���e�N�V����", "�A�N�e�B�u�v���e�N�V����"
						If IsNumeric(LIndex(fdata, 3)) Then
							ecost = CShort(LIndex(fdata, 3))
						Else
							ecost = 10
						End If
						If IsNumeric(LIndex(fdata, 4)) Then
							nmorale = CShort(LIndex(fdata, 4))
						Else
							nmorale = 0
						End If
						If .EN < ecost Or pmorale < nmorale Or (.IsConditionSatisfied("�o���A������") And InStr(fdata, "�o���A����������") = 0) Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						ElseIf InStr(fdata, "�\�͕K�v") > 0 Then 
							For j = 5 To LLength(fdata)
								opt = LIndex(fdata, j)
								If InStr(opt, "*") > 0 Then
									opt = Left(opt, InStr(opt, "*") - 1)
								End If
								Select Case opt
									Case "���E", "���a", "�ߐږ���", "�蓮", "�\�͕K�v"
										'�X�L�b�v
									Case "������"
										If .SyncLevel = 0 Then
											GoTo NextFeature
										End If
									Case "���"
										If .PlanaLevel = 0 Then
											GoTo NextFeature
										End If
									Case Else
										If .SkillLevel(opt) = 0 Then
											GoTo NextFeature
										End If
								End Select
							Next 
						End If
					Case "�t�B�[���h", "�A�N�e�B�u�t�B�[���h"
						If IsNumeric(LIndex(fdata, 3)) Then
							ecost = CShort(LIndex(fdata, 3))
						Else
							ecost = 0
						End If
						If IsNumeric(LIndex(fdata, 4)) Then
							nmorale = CShort(LIndex(fdata, 4))
						Else
							nmorale = 0
						End If
						If .EN < ecost Or pmorale < nmorale Or (.IsConditionSatisfied("�o���A������") And InStr(fdata, "�o���A����������") = 0) Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						ElseIf InStr(fdata, "�\�͕K�v") > 0 Then 
							For j = 5 To LLength(fdata)
								opt = LIndex(fdata, j)
								If InStr(opt, "*") > 0 Then
									opt = Left(opt, InStr(opt, "*") - 1)
								End If
								Select Case opt
									Case "���E", "���a", "�ߐږ���", "�蓮", "�\�͕K�v"
										'�X�L�b�v
									Case "������"
										If .SyncLevel = 0 Then
											GoTo NextFeature
										End If
									Case "���"
										If .PlanaLevel = 0 Then
											GoTo NextFeature
										End If
									Case Else
										If .SkillLevel(opt) = 0 Then
											GoTo NextFeature
										End If
								End Select
							Next 
						End If
					Case "�L��o���A", "�L��t�B�[���h", "�L��v���e�N�V����"
						If IsNumeric(LIndex(fdata, 4)) Then
							ecost = CShort(LIndex(fdata, 4))
						ElseIf IsNumeric(LIndex(fdata, 2)) Then 
							ecost = 20 * CShort(LIndex(fdata, 2))
						Else
							ecost = 0
						End If
						If IsNumeric(LIndex(fdata, 5)) Then
							nmorale = CShort(LIndex(fdata, 5))
						Else
							nmorale = 0
						End If
						If .EN < ecost Or pmorale < nmorale Or (.IsConditionSatisfied("�o���A������") And InStr(fdata, "�o���A����������") = 0) Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
						fname = fname & "(�͈�" & LIndex(fdata, 2) & "�}�X)"
					Case "�A�[�}�[", "���W�X�g"
						If IsNumeric(LIndex(fdata, 3)) Then
							nmorale = CShort(LIndex(fdata, 3))
						Else
							nmorale = 0
						End If
						If pmorale < nmorale Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						ElseIf InStr(fdata, "�\�͕K�v") > 0 Then 
							For j = 4 To LLength(fdata)
								opt = LIndex(fdata, j)
								If InStr(opt, "*") > 0 Then
									opt = Left(opt, InStr(opt, "*") - 1)
								End If
								Select Case opt
									Case "������"
										If .SyncLevel = 0 Then
											GoTo NextFeature
										End If
									Case "���"
										If .PlanaLevel = 0 Then
											GoTo NextFeature
										End If
									Case Else
										If .SkillLevel(opt) = 0 Then
											GoTo NextFeature
										End If
								End Select
							Next 
						End If
					Case "�U�����"
						If IsNumeric(LIndex(fdata, 3)) Then
							nmorale = CShort(LIndex(fdata, 3))
						Else
							nmorale = 0
						End If
						If pmorale < nmorale Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
					Case "����", "�j�~"
						If IsNumeric(LIndex(fdata, 4)) Then
							ecost = CShort(LIndex(fdata, 4))
						Else
							ecost = 0
						End If
						If IsNumeric(LIndex(fdata, 5)) Then
							nmorale = CShort(LIndex(fdata, 5))
						Else
							nmorale = 0
						End If
						If .EN < ecost Or pmorale < nmorale Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						ElseIf InStr(fdata, "�\�͕K�v") > 0 Then 
							For j = 6 To LLength(fdata)
								opt = LIndex(fdata, j)
								If InStr(opt, "*") > 0 Then
									opt = Left(opt, InStr(opt, "*") - 1)
								End If
								Select Case opt
									Case "���E", "���a", "�ߐږ���", "�蓮", "�\�͕K�v"
										'�X�L�b�v
									Case "������"
										If .SyncLevel = 0 Then
											GoTo NextFeature
										End If
									Case "���"
										If .PlanaLevel = 0 Then
											GoTo NextFeature
										End If
									Case Else
										If .SkillLevel(opt) = 0 Then
											GoTo NextFeature
										End If
								End Select
							Next 
						End If
					Case "�L��j�~"
						If IsNumeric(LIndex(fdata, 5)) Then
							ecost = CShort(LIndex(fdata, 5))
						Else
							ecost = 0
						End If
						If IsNumeric(LIndex(fdata, 6)) Then
							nmorale = CShort(LIndex(fdata, 6))
						Else
							nmorale = 0
						End If
						If .EN < ecost Or pmorale < nmorale Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
						fname = fname & "(�͈�" & LIndex(fdata, 2) & "�}�X)"
					Case "���Đg�Z", "��������"
						If IsNumeric(LIndex(fdata, 5)) Then
							ecost = CShort(LIndex(fdata, 5))
						Else
							ecost = 0
						End If
						If IsNumeric(LIndex(fdata, 6)) Then
							nmorale = CShort(LIndex(fdata, 6))
						Else
							nmorale = 0
						End If
						If .EN < ecost Or pmorale < nmorale Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						ElseIf InStr(fdata, "�\�͕K�v") > 0 Then 
							For j = 7 To LLength(fdata)
								opt = LIndex(fdata, j)
								If InStr(opt, "*") > 0 Then
									opt = Left(opt, InStr(opt, "*") - 1)
								End If
								Select Case opt
									Case "���E", "���a", "�ߐږ���", "�蓮", "�\�͕K�v"
										'�X�L�b�v
									Case "������"
										If .SyncLevel = 0 Then
											GoTo NextFeature
										End If
									Case "���"
										If .PlanaLevel = 0 Then
											GoTo NextFeature
										End If
									Case Else
										If .SkillLevel(opt) = 0 Then
											GoTo NextFeature
										End If
								End Select
							Next 
						End If
					Case "�u�[�X�g"
						If pmorale >= 130 Then
							'MOD START 240a
							'                        upic.ForeColor = vbBlue
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
							'MOD  END  240a
						End If
					Case "��"
						If .ConditionLevel("���_���[�W") >= .AllFeatureLevel("��") Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
						fname = fname & "(" & VB6.Format(MaxLng(.AllFeatureLevel("��") - .ConditionLevel("���_���[�W"), 0)) & "/" & VB6.Format(.AllFeatureLevel("��")) & ")"
					Case "�g�o��", "�d�m��"
						' MOD START MARGE
						'                    If .IsConditionSatisfied("�񕜕s�\") Then
						If .IsConditionSatisfied("�񕜕s�\") Or .IsSpecialPowerInEffect("�񕜕s�\") Then
							' MOD END MARGE
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
					Case "�i������", "�ˌ�����", "��������", "�������", "�Z�ʋ���", "��������", "�g�o����", "�d�m����", "���b����", "�^��������", "�ړ��͋���", "�g�o��������", "�d�m��������", "���b��������", "�^������������"
						If IsNumeric(LIndex(fdata, 2)) Then
							If pmorale >= StrToLng(LIndex(fdata, 2)) Then
								'MOD START 240a
								'                            upic.ForeColor = vbBlue
								upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityEnable, System.Drawing.ColorTranslator.ToOle(System.Drawing.Color.Blue)))
								'MOD  END  240a
							End If
						End If
					Case "�y�n�b"
						If LLength(fdata) < 2 Then
							j = 1
						Else
							j = CShort(LIndex(fdata, 2))
						End If
						If j >= 1 Then
							ReplaceString(fdata, vbTab, " ")
							If InStr(fdata, " ����") > 0 Or (InStr(fdata, " ����") > 0 And InStr(fdata, " ����") > 0) Then
								buf = "����"
							ElseIf InStr(fdata, " ����") > 0 Then 
								buf = "�㉺"
							ElseIf InStr(fdata, " ����") > 0 Then 
								buf = "���E"
							Else
								buf = "�͈�"
							End If
							fname = fname & "(" & buf & VB6.Format(j) & "�}�X)"
						End If
					Case "�L��y�n�b������"
						fname = fname & "(�͈�" & LIndex(fdata, 2) & "�}�X)"
					Case "�ǉ��U��"
						If IsNumeric(LIndex(fdata, 5)) Then
							ecost = CShort(LIndex(fdata, 5))
						Else
							ecost = 0
						End If
						If IsNumeric(LIndex(fdata, 6)) Then
							nmorale = CShort(LIndex(fdata, 6))
						Else
							nmorale = 0
						End If
						If .EN < ecost Or pmorale < nmorale Then
							'MOD START 240a
							'                        upic.ForeColor = rgb(150, 0, 0)
							upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
							'MOD  END  240a
						End If
				End Select
				
				'�K�v�����𖞂����Ȃ�����\�͂͐ԐF�ŕ\��
				If Not .IsFeatureActivated(i) Then
					'MOD START 240a
					'                upic.ForeColor = rgb(150, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
					'MOD  END  240a
				End If
				
				'����\�͖���\��
				'UPGRADE_ISSUE: �萔 vbFromUnicode �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
				'UPGRADE_ISSUE: LenB �֐��̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ���N���b�N���Ă��������B
				If LenB(StrConv(fname, vbFromUnicode)) > 19 Then
					If n > 0 Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print()
						'ADD START 240a
						If NewGUIMode Then
							'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.CurrentX = 5
						End If
						'ADD  END  240a
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(fname)
					n = 2
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(RightPaddedString(fname, 19))
					n = n + 1
				End If
				
				'�K�v�ɉ����ĉ��s
				If n > 1 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
					'ADD START 240a
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.CurrentX = 5
					End If
					'ADD  END  240a
					n = 0
				End If
				
				'�\���F��߂��Ă���
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
NextFeature: 
			Next 
			If n > 0 Then
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print()
			End If
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'�A�C�e���ꗗ
			If .CountItem > 0 Then
				j = 0
				For i = 1 To .CountItem
					With .Item(i)
						'�\���w������A�C�e���̂ݕ\������
						If Not .IsFeatureAvailable("�\��") Then
							GoTo NextItem
						End If
						
						'�A�C�e������\��
						If Len(.Nickname) > 9 Then
							If j = 1 Then
								'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
								upic.Print()
								'ADD START 240a
								If NewGUIMode Then
									'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
									upic.CurrentX = 5
								End If
								'ADD  END  240a
							End If
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(.Nickname)
							j = 2
						Else
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print(RightPaddedString(.Nickname, 19))
							j = j + 1
						End If
						If j = 2 Then
							'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
							upic.Print()
							'ADD START 240a
							If NewGUIMode Then
								'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
								upic.CurrentX = 5
							End If
							'ADD  END  240a
							j = 0
						End If
					End With
NextItem: 
				Next 
				If j > 0 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
					'ADD START 240a
					If NewGUIMode Then
						'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.CurrentX = 5
					End If
					'ADD  END  240a
				End If
			End If
			
			'�^�[�Q�b�g�I�����̍U�����ʗ\�z�\��
			
			'�U�����ɂ̂ݕ\��
			If (CommandState = "�^�[�Q�b�g�I��" Or CommandState = "�ړ���^�[�Q�b�g�I��") And (SelectedCommand = "�U��" Or SelectedCommand = "�}�b�v�U��") And Not SelectedUnit Is Nothing And SelectedWeapon > 0 And Stage <> "�v�����[�O" And Stage <> "�G�s���[�O" Then
				'�U�����Ɣ���
			Else
				GoTo SkipAttackExpResult
			End If
			
			'���肪�G�̏ꍇ�ɂ̂ݕ\��
			If .Party <> "�G" And .Party <> "����" And Not .IsConditionSatisfied("�\��") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("�߈�") And Not .IsConditionSatisfied("����") And Not .IsConditionSatisfied("����") Then
				GoTo SkipAttackExpResult
			End If
			
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print()
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'�U����i
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print("�U��     ")
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(SelectedUnit.WeaponNickname(SelectedWeapon))
			'�T�|�[�g�A�^�b�N�𓾂���H
			If Not SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "��") And Not SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "�l") And UseSupportAttack Then
				If Not SelectedUnit.LookForSupportAttack(u) Is Nothing Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" [��]")
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
				End If
			Else
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print()
			End If
			
			'�������󂯂�H
			If .MaxAction = 0 Or SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "�l") Or SelectedUnit.IsWeaponClassifiedAs(SelectedWeapon, "��") Then
				w = 0
			Else
				w = SelectWeapon(u, SelectedUnit, "����")
			End If
			
			'�G�̖h��s����ݒ�
			'UPGRADE_WARNING: �I�u�W�F�N�g SelectDefense() �̊���v���p�e�B�������ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6A50421D-15FE-4896-8A1B-2EC21E9037B2"' ���N���b�N���Ă��������B
			def_mode = SelectDefense(SelectedUnit, SelectedWeapon, u, w)
			If def_mode <> "" Then
				w = 0
			End If
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'�\���_���[�W
			If Not IsOptionDefined("�\���_���[�W��\��") Then
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("�_���[�W ")
				dmg = SelectedUnit.Damage(SelectedWeapon, u, True)
				If def_mode = "�h��" Then
					dmg = dmg \ 2
				End If
				If dmg >= .HP And Not .IsConditionSatisfied("�f�[�^�s��") Then
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(190, 0, 0))
				Else
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
				End If
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(VB6.Format(dmg))
			End If
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'�\��������
			If Not IsOptionDefined("�\����������\��") Then
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("������   ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				prob = SelectedUnit.HitProbability(SelectedWeapon, u, True)
				If def_mode = "���" Then
					prob = prob \ 2
				End If
				cprob = SelectedUnit.CriticalProbability(SelectedWeapon, u, def_mode)
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(MinLng(prob, 100) & "���i" & cprob & "���j")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
			End If
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			If w > 0 Then
				'������i
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print("����     ")
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(.WeaponNickname(w))
				'�T�|�[�g�K�[�h���󂯂���H
				If Not u.LookForSupportGuard(SelectedUnit, SelectedWeapon) Is Nothing Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" [��]")
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
				End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'�\���_���[�W
				If Not IsOptionDefined("�\���_���[�W��\��") Then
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("�_���[�W ")
					dmg = .Damage(w, SelectedUnit, True)
					If dmg >= SelectedUnit.HP Then
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(RGB(190, 0, 0))
					Else
						'MOD START 240a
						'                    upic.ForeColor = rgb(0, 0, 0)
						upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
						'MOD  END  240a
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(VB6.Format(dmg))
				End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'�\��������
				If Not IsOptionDefined("�\����������\��") Then
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 150)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
					'MOD  END  240a
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("������   ")
					'MOD START 240a
					'                upic.ForeColor = rgb(0, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
					'MOD  END  240a
					prob = .HitProbability(w, SelectedUnit, True)
					cprob = .CriticalProbability(w, SelectedUnit)
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(VB6.Format(MinLng(prob, 100)) & "���i" & cprob & "���j")
				End If
			Else
				'����͔����ł��Ȃ�
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 150)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
				'MOD  END  240a
				If def_mode <> "" Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(def_mode)
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("�����s�\")
				End If
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
				'�T�|�[�g�K�[�h���󂯂���H
				If Not u.LookForSupportGuard(SelectedUnit, SelectedWeapon) Is Nothing Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(" [��]")
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
				End If
			End If
			
SkipAttackExpResult: 
			
			'ADD START 240a
			If NewGUIMode Then
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.CurrentX = 5
			End If
			'ADD  END  240a
			'����ꗗ
			'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.CurrentY = upic.CurrentY + 8
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print(Space(25))
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 150)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityName, RGB(0, 0, 150)))
			'MOD  END  240a
			'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
			upic.Print("�U�� �˒�")
			'MOD START 240a
			'        upic.ForeColor = rgb(0, 0, 0)
			upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
			'MOD  END  240a
			
			ReDim warray(.CountWeapon)
			ReDim wpower(.CountWeapon)
			For i = 1 To .CountWeapon
				wpower(i) = .WeaponPower(i, "")
			Next 
			
			'�U���͂Ń\�[�g
			For i = 1 To .CountWeapon
				For j = 1 To i - 1
					If wpower(i) > wpower(warray(i - j)) Then
						Exit For
					ElseIf wpower(i) = wpower(warray(i - j)) Then 
						If .Weapon(i).ENConsumption > 0 Then
							If .Weapon(i).ENConsumption >= .Weapon(warray(i - j)).ENConsumption Then
								Exit For
							End If
						ElseIf .Weapon(i).Bullet > 0 Then 
							If .Weapon(i).Bullet <= .Weapon(warray(i - j)).Bullet Then
								Exit For
							End If
						Else
							If .Weapon(i - j).ENConsumption = 0 And .Weapon(warray(i - j)).Bullet = 0 Then
								Exit For
							End If
						End If
					End If
				Next 
				For k = 1 To j - 1
					warray(i - k + 1) = warray(i - k)
				Next 
				warray(i - j + 1) = i
			Next 
			
			'�X�̕����\��
			For i = 1 To .CountWeapon
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				If upic.CurrentY > 420 Then
					Exit For
				End If
				w = warray(i)
				If Not .IsWeaponAvailable(w, "�X�e�[�^�X") Then
					'�K�����Ă��Ȃ��Z�͕\�����Ȃ�
					If Not .IsWeaponMastered(w) Then
						GoTo NextWeapon
					End If
					'Disable�R�}���h�Ŏg�p�s�ɂȂ�����������l
					If .IsDisabled((.Weapon(w).Name)) Then
						GoTo NextWeapon
					End If
					'�t�H�[���[�V�����𖞂����Ă��Ȃ����̋Z��
					If .IsWeaponClassifiedAs(w, "��") Then
						If Not .IsCombinationAttackAvailable(w, True) Then
							GoTo NextWeapon
						End If
					End If
					'MOD START 240a
					'                    upic.ForeColor = rgb(150, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
					'MOD  END  240a
				End If
				
				'����̕\��
				If .WeaponPower(w, "") < 10000 Then
					buf = RightPaddedString(VB6.Format(.WeaponNickname(w)), 25)
					buf = buf & LeftPaddedString(VB6.Format(.WeaponPower(w, "")), 4)
				Else
					buf = RightPaddedString(VB6.Format(.WeaponNickname(w)), 24)
					buf = buf & LeftPaddedString(VB6.Format(.WeaponPower(w, "")), 5)
				End If
				
				'���킪������ʂ����ꍇ�͗��̂ŕ\�L
				If .WeaponMaxRange(w) > 1 Then
					'UPGRADE_ISSUE: �萔 vbFromUnicode �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
					'UPGRADE_ISSUE: LenB �֐��̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ���N���b�N���Ă��������B
					buf = buf & LeftPaddedString(VB6.Format(.Weapon(w).MinRange) & "-" & VB6.Format(.WeaponMaxRange(w)), 34 - LenB(StrConv(buf, vbFromUnicode)))
					'�ړ���U���\
					If .IsWeaponClassifiedAs(w, "�o") Then
						buf = buf & "P"
					End If
				Else
					'UPGRADE_ISSUE: �萔 vbFromUnicode �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="55B59875-9A95-4B71-9D6A-7C294BF7139D"' ���N���b�N���Ă��������B
					'UPGRADE_ISSUE: LenB �֐��̓T�|�[�g����܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="367764E5-F3F8-4E43-AC3E-7FE0B5E074E2"' ���N���b�N���Ă��������B
					buf = buf & LeftPaddedString("1", 34 - LenB(StrConv(buf, vbFromUnicode)))
					' ADD START MARGE
					'�ړ���U���s��
					If .IsWeaponClassifiedAs(w, "�p") Then
						buf = buf & "Q"
					End If
					' ADD END MARGE
				End If
				'�}�b�v�U��
				If .IsWeaponClassifiedAs(w, "�l") Then
					buf = buf & "M"
				End If
				'�������
				wclass = .Weapon(w).Class_Renamed
				For j = 1 To .CountWeaponEffect(w)
					buf = buf & "+"
				Next 
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(buf)
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
NextWeapon: 
			Next 
			
			'�A�r���e�B�ꗗ
			For i = 1 To .CountAbility
				'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentY �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				If upic.CurrentY > 420 Then
					Exit For
				End If
				If Not .IsAbilityAvailable(i, "�X�e�[�^�X") Then
					'�K�����Ă��Ȃ��Z�͕\�����Ȃ�
					If Not .IsAbilityMastered(i) Then
						GoTo NextAbility
					End If
					'Disable�R�}���h�Ŏg�p�s�ɂȂ�����������l
					If .IsDisabled((.Ability(i).Name)) Then
						GoTo NextAbility
					End If
					'�t�H�[���[�V�����𖞂����Ă��Ȃ����̋Z��
					If .IsAbilityClassifiedAs(i, "��") Then
						If Not .IsCombinationAbilityAvailable(i, True) Then
							GoTo NextAbility
						End If
					End If
					'MOD START 240a
					'                upic.ForeColor = rgb(150, 0, 0)
					upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorAbilityDisable, RGB(150, 0, 0)))
					'MOD  END  240a
				End If
				
				'ADD START 240a
				If NewGUIMode Then
					'UPGRADE_ISSUE: PictureBox �v���p�e�B upic.CurrentX �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.CurrentX = 5
				End If
				'ADD  END  240a
				'�A�r���e�B�̕\��
				'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
				upic.Print(RightPaddedString(VB6.Format(.AbilityNickname(i)), 29))
				If .AbilityMaxRange(i) > 1 Then
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print(LeftPaddedString(VB6.Format(.AbilityMinRange(i)) & "-" & VB6.Format(.AbilityMaxRange(i)), 5))
					If .IsAbilityClassifiedAs(i, "�o") Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("P")
					End If
					If .IsAbilityClassifiedAs(i, "�l") Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("M")
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
				ElseIf .AbilityMaxRange(i) = 1 Then 
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("    1")
					' ADD START MARGE
					If .IsAbilityClassifiedAs(i, "�p") Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("Q")
					End If
					' ADD END MARGE
					If .IsAbilityClassifiedAs(i, "�l") Then
						'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
						upic.Print("M")
					End If
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print()
				Else
					'UPGRADE_ISSUE: PictureBox ���\�b�h upic.Print �̓A�b�v�O���[�h����܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="CC4C7EC0-C903-48FC-ACCC-81861D12DA4A"' ���N���b�N���Ă��������B
					upic.Print("    -")
				End If
				'MOD START 240a
				'            upic.ForeColor = rgb(0, 0, 0)
				upic.ForeColor = System.Drawing.ColorTranslator.FromOle(IIf(NewGUIMode, StatusFontColorNormalString, RGB(0, 0, 0)))
				'MOD  END  240a
NextAbility: 
			Next 
		End With
		
UpdateStatusWindow: 
		
		'MOD START 240a
		'    If MainWidth = 15 Then
		If Not NewGUIMode Then
			'MOD  END
			'�X�e�[�^�X�E�B���h�E�����t���b�V��
			'UPGRADE_ISSUE: Control picFace �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picFace.Refresh()
			ppic.Refresh()
			upic.Refresh()
		Else
			If MouseX < MainPWidth \ 2 Then
				'MOD START 240a
				'            upic.Move MainPWidth - 230 - 5, 10
				'��ʍ����ɃJ�[�\��������ꍇ
				upic.SetBounds(VB6.TwipsToPixelsX(MainPWidth - 240), VB6.TwipsToPixelsY(10), 0, 0, Windows.Forms.BoundsSpecified.X Or Windows.Forms.BoundsSpecified.Y)
				'MOD  END
			Else
				upic.SetBounds(VB6.TwipsToPixelsX(5), VB6.TwipsToPixelsY(10), 0, 0, Windows.Forms.BoundsSpecified.X Or Windows.Forms.BoundsSpecified.Y)
			End If
			If upic.Visible Then
				upic.Refresh()
			Else
				upic.Visible = True
			End If
		End If
		
		Exit Sub
		
ErrorHandler: 
		ErrorMessage("�p�C���b�g�p�摜�t�@�C��" & vbCr & vbLf & fname & vbCr & vbLf & "�̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCr & vbLf & "�摜�t�@�C�������Ă��Ȃ����m�F���ĉ������B")
	End Sub
	
	'�w�肳�ꂽ�p�C���b�g�̃X�e�[�^�X���X�e�[�^�X�E�B���h�E�ɕ\��
	Public Sub DisplayPilotStatus(ByVal p As Pilot)
		Dim i As Short
		
		DisplayedUnit = p.Unit_Renamed
		
		With DisplayedUnit
			If p Is .MainPilot Then
				'���C���p�C���b�g
				DisplayUnitStatus(DisplayedUnit, 0)
			Else
				'�T�u�p�C���b�g
				For i = 1 To .CountPilot
					If p Is .Pilot(i) Then
						DisplayUnitStatus(DisplayedUnit, i)
						Exit Sub
					End If
				Next 
				
				'�T�|�[�g�p�C���b�g
				For i = 1 To .CountSupport
					If p Is .Support(i) Then
						DisplayUnitStatus(DisplayedUnit, i + .CountPilot)
						Exit Sub
					End If
				Next 
				
				'�ǉ��T�|�[�g
				If .IsFeatureAvailable("�ǉ��T�|�[�g") Then
					DisplayUnitStatus(DisplayedUnit, .CountPilot + .CountSupport + 1)
				End If
			End If
		End With
	End Sub
	
	'�w�肵���}�b�v���W�ɂ��郆�j�b�g�̃X�e�[�^�X���X�e�[�^�X�E�B���h�E�ɕ\��
	Public Sub InstantUnitStatusDisplay(ByVal X As Short, ByVal Y As Short)
		Dim u As Unit
		
		'�w�肳�ꂽ���W�ɂ��郆�j�b�g������
		u = MapDataForUnit(X, Y)
		
		'���i�R�}���h�̏ꍇ�͕�͂ł͂Ȃ����i���郆�j�b�g���g��
		If CommandState = "�^�[�Q�b�g�I��" And SelectedCommand = "���i" Then
			If u Is SelectedUnit Then
				u = SelectedTarget
				If u Is Nothing Then
					Exit Sub
				End If
			End If
		End If
		
		If DisplayedUnit Is Nothing Then
			'�X�e�[�^�X�E�B���h�E�ɉ����\������Ă��Ȃ���Ζ������ŕ\��
		Else
			'�������j�b�g���\������Ă���΃X�L�b�v
			If u Is DisplayedUnit Then
				Exit Sub
			End If
		End If
		
		DisplayUnitStatus(u)
	End Sub
	
	'�X�e�[�^�X�E�B���h�E���N���A
	Public Sub ClearUnitStatus()
		If MainWidth = 15 Then
			'UPGRADE_ISSUE: Control picFace �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picFace = System.Drawing.Image.FromFile("")
			'UPGRADE_ISSUE: Control picPilotStatus �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picPilotStatus.Cls()
			'UPGRADE_ISSUE: Control picUnitStatus �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picUnitStatus.Cls()
			'UPGRADE_NOTE: �I�u�W�F�N�g DisplayedUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DisplayedUnit = Nothing
		Else
			'UPGRADE_ISSUE: Control picUnitStatus �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picUnitStatus.Visible = False
			'UPGRADE_ISSUE: Control picUnitStatus �́A�ėp���O��� Form ���ɂ��邽�߁A�����ł��܂���ł����B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="084D22AD-ECB1-400F-B4C7-418ECEC5E36E"' ���N���b�N���Ă��������B
			MainForm.picUnitStatus.Cls()
			IsStatusWindowDisabled = True
			System.Windows.Forms.Application.DoEvents()
			IsStatusWindowDisabled = False
			'ADD
			'UPGRADE_NOTE: �I�u�W�F�N�g DisplayedUnit ���K�x�[�W �R���N�g����܂ł��̃I�u�W�F�N�g��j�����邱�Ƃ͂ł��܂���B �ڍׂɂ��ẮA'ms-help://MS.VSCC.v90/dv_commoner/local/redirect.htm?keyword="6E35BFF6-CD74-4B09-9689-3E1A43DF8969"' ���N���b�N���Ă��������B
			DisplayedUnit = Nothing
		End If
	End Sub
	
	'ADD START 240a
	'�V�f�t�h���̃O���[�o���X�e�[�^�X�E�C���h�E�̃T�C�Y���擾����
	Private Function GetGlobalStatusSize(ByRef X As Short, ByRef Y As Short) As Integer
		Dim ret As Integer
		ret = 42
		If Not (X < 1 Or MapWidth < X Or Y < 1 Or MapHeight < Y) Then
			'�n�`���̕\�����m��
			ret = 106
			'�g�o�E�d�m�񕜂��L�q�����ꍇ
			If TerrainEffectForHPRecover(X, Y) > 0 Or TerrainEffectForENRecover(X, Y) > 0 Then
				ret = ret + 16
			End If
			'�g�o�E�d�m�������L�q�����ꍇ
			If TerrainHasFeature(X, Y, "�g�o����") Or TerrainHasFeature(X, Y, "�d�m����") Then
				ret = ret + 16
			End If
			'�g�o�E�d�m�������L�q�����ꍇ
			If TerrainHasFeature(X, Y, "�g�o����") Or TerrainHasFeature(X, Y, "�d�m����") Then
				ret = ret + 16
			End If
			'�g�o�E�d�m�ቺ���L�q�����ꍇ
			If TerrainHasFeature(X, Y, "�g�o�ቺ") Or TerrainHasFeature(X, Y, "�d�m�ቺ") Then
				ret = ret + 16
			End If
			'���C�E��ԕt�����L�q�����ꍇ
			If TerrainHasFeature(X, Y, "���C") Or TerrainHasFeature(X, Y, "��ԕt��") Then
				ret = ret + 16
			End If
		End If
		GetGlobalStatusSize = ret
	End Function
	
	'Global�ϐ��ƃX�e�[�^�X�`��n�ϐ��̓����B
	Private Sub GlobalVariableLoad()
		'�w�i�F
		If IsGlobalVariableDefined("StatusWindow(BackBolor)") Then
			If Not StatusWindowBackBolor = GetValueAsLong("StatusWindow(BackBolor)") Then
				StatusWindowBackBolor = GetValueAsLong("StatusWindow(BackBolor)")
			End If
		End If
		'�g�̐F
		If IsGlobalVariableDefined("StatusWindow(FrameColor)") Then
			If Not StatusWindowFrameColor = GetValueAsLong("StatusWindow(FrameColor)") Then
				StatusWindowFrameColor = GetValueAsLong("StatusWindow(FrameColor)")
			End If
		End If
		'�g�̑���
		If IsGlobalVariableDefined("StatusWindow(FrameWidth)") Then
			If Not StatusWindowFrameWidth = GetValueAsLong("StatusWindow(FrameWidth)") Then
				StatusWindowFrameWidth = GetValueAsLong("StatusWindow(FrameWidth)")
			End If
		End If
		'�\�͖��̐F
		If IsGlobalVariableDefined("StatusWindow(ANameColor)") Then
			If Not StatusFontColorAbilityName = GetValueAsLong("StatusWindow(ANameColor)") Then
				StatusFontColorAbilityName = GetValueAsLong("StatusWindow(ANameColor)")
			End If
		End If
		'�L���Ȕ\�͂̐F
		If IsGlobalVariableDefined("StatusWindow(EnableColor)") Then
			If Not StatusFontColorAbilityEnable = GetValueAsLong("StatusWindow(EnableColor)") Then
				StatusFontColorAbilityEnable = GetValueAsLong("StatusWindow(EnableColor)")
			End If
		End If
		'�����Ȕ\�͂̐F
		If IsGlobalVariableDefined("StatusWindow(DisableColor)") Then
			If Not StatusFontColorAbilityDisable = GetValueAsLong("StatusWindow(DisableColor)") Then
				StatusFontColorAbilityDisable = GetValueAsLong("StatusWindow(DisableColor)")
			End If
		End If
		'�ʏ핶���̐F
		If IsGlobalVariableDefined("StatusWindow(StringColor)") Then
			If Not StatusFontColorNormalString = GetValueAsLong("StatusWindow(StringColor)") Then
				StatusFontColorNormalString = GetValueAsLong("StatusWindow(StringColor)")
			End If
		End If
	End Sub
	'ADD  END  240a
End Module