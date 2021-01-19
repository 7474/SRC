Attribute VB_Name = "Susie"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'Susie�v���O�C���𗘗p���ĉ摜�t�@�C����ǂݍ��ނ��߂̃��W���[��

'Susie 32-bit Plug-in API
Private Declare Function GetPNGPicture Lib "ifpng.spi" Alias "GetPicture" (buf As Any, _
   ByVal length As Long, ByVal flag As Long, pHBInfo As Long, pHBm As Long, _
   ByVal lpProgressCallback As Any, ByVal lData As Long) As Long

Private Declare Function SetDIBits Lib "gdi32" (ByVal hDC As Long, ByVal hBitmap As Long, _
    ByVal nStartScan As Long, ByVal nNumScans As Long, _
    lpBits As Any, lpBI As Any, ByVal wUsage As Long) As Long

Private Declare Function LocalFree Lib "kernel32" (ByVal hMem As Long) As Long
Private Declare Function LocalLock Lib "kernel32" (ByVal hMem As Long) As Long
Private Declare Function LocalUnlock Lib "kernel32" (ByVal hMem As Long) As Long

Private Declare Sub MoveMemory Lib "kernel32" Alias "RtlMoveMemory" (dest As Any, _
   Source As Any, ByVal length As Long)

'�摜�t�@�C����ǂݍ��ފ֐�
Public Function LoadPicture2(pic As PictureBox, fname As String) As Boolean
Dim HBInfo As Long, HBm As Long
Dim lpHBInfo As Long, lpHBm As Long
Dim bmi As BITMAPINFO
Dim ret As Long
    
    On Error GoTo ErrorHandler
    
    '�摜�̎擾
    Select Case LCase$(Right$(fname, 4))
        Case ".bmp", ".jpg", ".gif"
            'Susie�v���O�C�����g�킸�Ƀ��[�h
            pic = LoadPicture(fname)
            LoadPicture2 = True
            Exit Function
        Case ".png"
            'PNG�t�@�C���pSusie�v���O�C��API�����s
            ret = GetPNGPicture(ByVal fname, 0, 0, HBInfo, HBm, ByVal 0&, 0)
        Case Else
            '���T�|�[�g�̃t�@�C���`��
            ErrorMessage "�摜�t�@�C��" & vbCr & vbLf _
                & fname & vbCr & vbLf _
                & "�̉摜�t�H�[�}�b�g�̓T�|�[�g����Ă��܂���B"
            pic = LoadPicture("")
            Exit Function
    End Select
    
    '�ǂݍ��݂ɐ��������H
    If ret <> 0 Then
        ErrorMessage "�摜�t�@�C��" & vbCr & vbLf _
            & fname & vbCr & vbLf _
            & "�̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCr & vbLf _
            & "�摜�t�@�C�������Ă��Ȃ����m�F���ĉ������B"
        Exit Function
    End If
    
    '�������̃��b�N
    lpHBInfo = LocalLock(HBInfo)
    lpHBm = LocalLock(HBm)
    
    '�Ȃ����摜����U�������Ă����K�v����
    pic = LoadPicture("")
    
    With pic
        '�s�N�`���{�b�N�X�̃T�C�Y�ύX
        Call MoveMemory(bmi, ByVal lpHBInfo, Len(bmi))
        .Width = bmi.bmiHeader.biWidth
        .Height = bmi.bmiHeader.biHeight
        
        '�摜�̕\��
        ret = SetDIBits(.hDC, .Image, 0, .Height, ByVal lpHBm, ByVal lpHBInfo, 0)
    End With
    
    '�������̃��b�N����
    Call LocalUnlock(HBInfo)
    Call LocalUnlock(HBm)
    
    '�������n���h���̉��
    Call LocalFree(HBInfo)
    Call LocalFree(HBm)
    
    '�摜�̓ǂݏo���ɐ����������ǂ�����Ԃ�
    If ret <> 0 Then
        LoadPicture2 = True
    End If
    
    Exit Function
    
ErrorHandler:
    '�G���[����
    Select Case LCase$(Right$(fname, 4))
        Case ".bmp", ".jpg", ".gif"
            ErrorMessage "�摜�t�@�C��" & vbCr & vbLf _
                & fname & vbCr & vbLf _
                & "�̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCr & vbLf _
                & "�摜�t�@�C�������Ă��Ȃ����m�F���ĉ������B"
        Case ".png"
            ErrorMessage "�摜�t�@�C��" & vbCr & vbLf _
                & fname & vbCr & vbLf _
                & "�̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCr & vbLf _
                & "PNG�t�@�C���pSusie Plug-in���C���X�g�[������Ă��܂���B"
    End Select
End Function

