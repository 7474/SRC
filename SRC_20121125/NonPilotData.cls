VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "NonPilotData"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'ノンパイロットデータのクラス

'名称
Public Name As String

'愛称
Private strNickname As String

'ビットマップ名
Private proBitmap As String
'ビットマップが存在するか
Public IsBitmapMissing As Boolean


'愛称
Public Property Get Nickname0() As String
    Nickname0 = strNickname
End Property

Public Property Get Nickname() As String
Dim pname As String, idx As Integer

    Nickname = strNickname
    
    'イベントで愛称が変更されている？
    If InStr(Nickname, "主人公") = 1 Or InStr(Nickname, "ヒロイン") = 1 Then
        Nickname = GetValueAsString(Nickname & "愛称")
    End If
    
    ReplaceSubExpression Nickname
    
    '表情パターンの場合
    idx = InStr(Name, "(")
    If idx > 1 Then
        'パイロット本来の名称or愛称を切り出し
        pname = Left$(Name, idx - 1)
        
        'そのパイロットが作成されている？
        If Not PList.IsDefined(pname) Then
            Exit Property
        End If
        
        With PList.Item(pname)
            'パイロットがユニットに乗っている？
            If .Unit Is Nothing Then
                Exit Property
            End If
            
            With .Unit
                '念のため……
                If .CountPilot = 0 Then
                    Exit Property
                End If
                
                'パイロットはメインパイロット？
                If pname <> .MainPilot.Name _
                    And pname <> .MainPilot.Data.Nickname _
                Then
                    Exit Property
                End If
                
                'パイロット愛称変更能力を適用
                If .IsFeatureAvailable("パイロット愛称") Then
                    pname = .FeatureData("パイロット愛称")
                    idx = InStr(pname, "$(愛称)")
                    If idx > 0 Then
                        pname = Left$(pname, idx - 1) _
                            & Nickname & Mid$(pname, idx + 5)
                    End If
                    Nickname = pname
                End If
            End With
        End With
    End If
End Property

Public Property Let Nickname(new_name As String)
    strNickname = new_name
End Property

'ビットマップ
Public Property Get Bitmap0() As String
    Bitmap0 = proBitmap
End Property

Public Property Get Bitmap() As String
    If IsBitmapMissing Then
        Bitmap = "-.bmp"
    Else
        Bitmap = proBitmap
    End If
End Property

Public Property Let Bitmap(new_bitmap As String)
    proBitmap = new_bitmap
End Property

