Option Strict Off
Option Explicit On
Module BCVariable
	
	' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
	' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
	' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
	' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B
	
	' �o�g���R���t�B�O�f�[�^���L�����H
	Public IsConfig As Boolean
	
	' �o�g���R���t�B�O�f�[�^�̊e��ϐ����`����
	
	' �o�g���R���t�B�O�f�[�^�Ώے��S���j�b�g��`
	' ---------���Ȃ炸��`�����f�[�^
	Public MeUnit As Unit
	
	' �U�������j�b�g��`
	Public AtkUnit As Unit
	
	' �h�䑤���j�b�g��`
	Public DefUnit As Unit
	
	' ����ԍ�
	Public WeaponNumber As Short
	
	' ---------��`����Ȃ��ꍇ������(�v�Z��Ƀ��Z�b�g�����)�f�[�^
	' �U���l
	Public AttackExp As Integer
	
	' �U������`�ϐ�
	Public AttackVariable As Integer
	
	' �h�䑤��`�ϐ�
	Public DffenceVariable As Integer
	
	' �n�`�␳
	Public TerrainAdaption As Double
	
	' �T�C�Y�␳
	Public SizeMod As Double
	
	' �ŏI�l
	Public LastVariable As Integer
	
	' ����U����
	Public WeaponPower As Integer
	
	' ���b�l
	Public Armor As Integer
	
	' �U�R�␳
	Public CommonEnemy As Integer
	
	'��`����Ȃ����Ƃ�����f�[�^�������Ń��Z�b�g����
	Public Sub DataReset()
		AttackExp = 0
		AttackVariable = 0
		DffenceVariable = 0
		TerrainAdaption = 1
		SizeMod = 1
		LastVariable = 0
		WeaponPower = 0
		CommonEnemy = 0
	End Sub
End Module