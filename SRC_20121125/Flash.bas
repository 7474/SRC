Attribute VB_Name = "Flash"
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' �{�v���O�����̓t���[�\�t�g�ł���A���ۏ؂ł��B
' �{�v���O������GNU General Public License(Ver.3�܂��͂���ȍ~)����߂�����̉���
' �ĔЕz�܂��͉��ς��邱�Ƃ��ł��܂��B

'Flash�t�@�C���̍Đ�
Public Sub PlayFlash(fname As String, _
    fx As Integer, fy As Integer, fw As Integer, fh As Integer, _
    opt As String)
Dim i As Integer
Dim is_VisibleEnd As Boolean
    
    'FLASH���g�p�ł��Ȃ��ꍇ�̓G���[
    If Not IsFlashAvailable Then
        ErrorMessage "Flash�t�@�C���̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCrLf _
            & "�uMacromedia Flash Player�v���C���X�g�[������Ă��܂���B" & vbCrLf _
            & "����URL����A�ŐV�ł�Flash Player���C���X�g�[�����Ă��������B" & vbCrLf _
            & "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese"
        Exit Sub
    End If
    If Not frmMain.FlashObject.Enable Then
        ErrorMessage "Flash�t�@�C���̓ǂݍ��ݒ��ɃG���[���������܂����B" & vbCrLf _
            & "�uMacromedia Flash Player�v���C���X�g�[������Ă��܂���B" & vbCrLf _
            & "����URL����A�ŐV�ł�Flash Player���C���X�g�[�����Ă��������B" & vbCrLf _
            & "http://www.macromedia.com/shockwave/download/download.cgi?P5_Language=Japanese&Lang=Japanese&P1_Prod_Version=ShockwaveFlash&Lang=Japanese"
        Exit Sub
    End If
    
    is_VisibleEnd = False
    
    For i = 1 To LLength(opt)
        Select Case LIndex(opt, i)
            Case "�ێ�"
                is_VisibleEnd = True
        End Select
    Next
    
    With frmMain.FlashObject
        '��U��\��
        .Visible = False
    
        'Flash�I�u�W�F�N�g�̈ʒu�E�T�C�Y�ݒ�
        .Left = fx
        .Top = fy
        .Width = fw
        .Height = fh
        .Visible = True
        .ZOrder
        
        .LoadMovie ScenarioPath & fname
            
        Do While .Playing And Not IsRButtonPressed(True)
            DoEvents
        Loop
        
        frmMain.FlashObject.StopMovie
            
        If Not is_VisibleEnd Then
            .ClearMovie
            .Visible = False
        End If
    End With
End Sub

'�\�������܂܂�Flash����������
Public Sub ClearFlash()
    If Not IsFlashAvailable Then Exit Sub
    If Not frmMain.FlashObject.Enable Then Exit Sub
    
    With frmMain.FlashObject
        .ClearMovie
        .Visible = False
    End With
End Sub

'Flash�t�@�C������C�x���g���擾
' Flash�̃A�N�V�����́uGetURL�v��
'�@1.�uURL�v��"FSCommand:"
'�@2.�u�^�[�Q�b�g�v�Ɂu�T�u���[�`���� [����1 [����2 [�c]]�v
'���w�肷��ƁA���̃A�N�V���������s���ꂽ�Ƃ���
'�^�[�Q�b�g�̃T�u���[�`�������s�����B
'�T�u���[�`�������s���Ă���ԁAFlash�̍Đ��͒�~����B
Public Sub GetEvent(ByVal fpara As String)
Dim buf As String, i As Integer, j As Integer
Dim funcname As String, funcpara As String
Dim etype As ValueType, str_result As String, num_result As Double

    '�Đ����ꎞ��~
    frmMain.FlashObject.StopMovie
        
    funcname = ""
    funcpara = ""
    
    '�O�̂��߂�Flash����n���ꂽ�p�����[�^�S�Ă����
    '��ԍŏ��Ɍ���������������A�Ăяo���T�u���[�`�����Ƃ���
    If funcname = "" Then
        funcname = ListIndex(fpara, 1)
        buf = ListTail(fpara, 2)
    End If
    '�T�u���[�`���̈������L�^
    For j = 1 To ListLength(buf)
        funcpara = funcpara & ", " & ListIndex(buf, j)
    Next
    
    '�T�u���[�`�����ƈ�������ACall�֐��̌Ăяo���̕�����𐶐�
    buf = "Call(" & funcname & funcpara & ")"
    '���Ƃ��Đ�����������������s
    CallFunction buf, etype, str_result, num_result
    
    '�Đ����ĊJ
    frmMain.FlashObject.PlayMovie
End Sub
