Attribute VB_Name = "BCVariable"
Option Explicit

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
Public WeaponNumber As Integer

' ---------��`����Ȃ��ꍇ������(�v�Z��Ƀ��Z�b�g�����)�f�[�^
' �U���l
Public AttackExp As Long

' �U������`�ϐ�
Public AttackVariable As Long

' �h�䑤��`�ϐ�
Public DffenceVariable As Long

' �n�`�␳
Public TerrainAdaption As Double

' �T�C�Y�␳
Public SizeMod As Double

' �ŏI�l
Public LastVariable As Long

' ����U����
Public WeaponPower As Long

' ���b�l
Public Armor As Long

' �U�R�␳
Public CommonEnemy As Long

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

