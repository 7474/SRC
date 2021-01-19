VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "TerrainDataList"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = True
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'全地形データを管理するリストのクラス

'地形データの登録数
Public Count As Integer

'地形データの配列
'他のリスト管理用クラスと異なり配列を使っているのはアクセスを高速化するため
Private TerrainDataList(0 To MAX_TERRAIN_DATA_NUM) As TerrainData

'地形データの登録順を記録するための配列
Private OrderList(1 To MAX_TERRAIN_DATA_NUM) As Integer


'クラスの初期化
Private Sub Class_Initialize()
Dim i As Integer
    
    For i = 0 To MAX_TERRAIN_DATA_NUM
        Set TerrainDataList(i) = New TerrainData
    Next
End Sub

'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer
    
    For i = 0 To MAX_TERRAIN_DATA_NUM
        Set TerrainDataList(i) = Nothing
    Next
End Sub


'指定したデータは登録されているか？
Public Function IsDefined(ByVal ID As Integer) As Boolean
    If TerrainDataList(ID).ID >= 0 Then
        IsDefined = True
    Else
        IsDefined = False
    End If
End Function


'地形データリストから指定したデータを取り出す
Public Function Item(ByVal ID As Integer) As TerrainData
    Set Item = TerrainDataList(ID)
End Function

'指定したデータの名称
Public Function Name(ByVal ID As Integer) As String
    Name = TerrainDataList(ID).Name
End Function

'指定したデータの画像ファイル名
Public Function Bitmap(ByVal ID As Integer) As String
    Bitmap = TerrainDataList(ID).Bitmap
End Function

'指定したデータのクラス
Public Function Class(ByVal ID As Integer) As String
    Class = TerrainDataList(ID).Class
End Function

'指定したデータの移動コスト
Public Function MoveCost(ByVal ID As Integer) As Integer
    MoveCost = TerrainDataList(ID).MoveCost
End Function

'指定したデータの命中修正
Public Function HitMod(ByVal ID As Integer) As Integer
    HitMod = TerrainDataList(ID).HitMod
End Function

'指定したデータのダメージ修正
Public Function DamageMod(ByVal ID As Integer) As Integer
    DamageMod = TerrainDataList(ID).DamageMod
End Function


'指定したデータの特殊能力

Public Function IsFeatureAvailable(ByVal ID As Integer, ftype As String) As Boolean
    IsFeatureAvailable = TerrainDataList(ID).IsFeatureAvailable(ftype)
End Function

Public Function FeatureLevel(ByVal ID As Integer, ftype As String) As Double
    FeatureLevel = TerrainDataList(ID).FeatureLevel(ftype)
End Function

Public Function FeatureData(ByVal ID As Integer, ftype As String) As String
    FeatureData = TerrainDataList(ID).FeatureData(ftype)
End Function


'Ｎ番目に登録したデータの番号
Public Function OrderedID(ByVal n As Integer) As Integer
    OrderedID = OrderList(n)
End Function


'データファイル fname からデータをロード
Public Sub Load(fname As String)
Dim FileNumber As Integer
Dim ret As Integer, line_num As Long
Dim i As Integer, j As Integer
Dim line_buf As String, buf As String, buf2 As String
Dim td As TerrainData
Dim data_id As Integer, data_name As String
Dim err_msg As String
Dim in_quote As Boolean

    On Error GoTo ErrorHandler
    
    FileNumber = FreeFile
    Open fname For Input Access Read As #FileNumber
    
    line_num = 0
    
    Do While True
        data_name = ""
        
        Do
            If EOF(FileNumber) Then
                Close #FileNumber
                Exit Sub
            End If
            GetLine FileNumber, line_buf, line_num
        Loop While Len(line_buf) = 0
        
        '番号
        If IsNumeric(line_buf) Then
            data_id = CInt(line_buf)
        Else
            err_msg = "番号の設定が間違っています。"
            Error 0
        End If
        If data_id < 0 Or data_id >= MAX_TERRAIN_DATA_NUM Then
            err_msg = "番号の設定が間違っています。"
            Error 0
        End If
        
        Set td = TerrainDataList(data_id)
        
        With td
            '新規登録？
            If .ID < 0 Then
                Count = Count + 1
                OrderList(Count) = data_id
            Else
                .Clear
            End If
            .ID = data_id
            
            '名称, 画像ファイル名
            GetLine FileNumber, line_buf, line_num
            
            '名称
            ret = InStr(line_buf, ",")
            If ret = 0 Then
                err_msg = "画像ファイル名が抜けています。"
                Error 0
            End If
            data_name = Trim$(Left$(line_buf, ret - 1))
            .Name = data_name
            buf = Mid$(line_buf, ret + 1)
            
            '画像ファイル名
            .Bitmap = Trim$(buf)
            If Len(.Bitmap) = 0 Then
                err_msg = "画像ファイル名が指定されていません。"
                Error 0
            End If
            
            '地形タイプ, 移動コスト, 命中修正, ダメージ修正
            GetLine FileNumber, line_buf, line_num
            
            '地形タイプ
            ret = InStr(line_buf, ",")
            If ret = 0 Then
                err_msg = "移動コストが抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(line_buf, ret - 1))
            buf = Mid$(line_buf, ret + 1)
            .Class = buf2
            
            '移動コスト
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "命中修正が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If buf2 = "-" Then
                .MoveCost = 1000
            ElseIf IsNumeric(buf2) Then
                '0.5刻みの移動コストを使えるようにするため、実際の２倍の値で記録する
                .MoveCost = CInt(2 * CDbl(buf2))
            End If
            If .MoveCost <= 0 Then
                DataErrorMessage "移動コストの設定が間違っています。", _
                    fname, line_num, line_buf, data_name
            End If
            
            '命中修正
            ret = InStr(buf, ",")
            If ret = 0 Then
                err_msg = "ダメージ修正が抜けています。"
                Error 0
            End If
            buf2 = Trim$(Left$(buf, ret - 1))
            buf = Mid$(buf, ret + 1)
            If IsNumeric(buf2) Then
                .HitMod = CInt(buf2)
            Else
                DataErrorMessage "命中修正の設定が間違っています。", _
                    fname, line_num, line_buf, data_name
            End If
            
            'ダメージ修正
            ret = InStr(buf, ",")
            If ret > 0 Then
                err_msg = "余分な「,」が指定されています。"
                Error 0
            End If
            buf2 = Trim$(buf)
            If IsNumeric(buf2) Then
                .DamageMod = CInt(buf2)
            Else
                DataErrorMessage "ダメージ修正の設定が間違っています。", _
                    fname, line_num, line_buf, data_name
            End If
            
            '地形効果
            GetLine FileNumber, line_buf, line_num
            Do While Len(line_buf) > 0
                buf = line_buf
                i = 0
                Do While Len(buf) > 0
                    i = i + 1
                    
                    ret = 0
                    in_quote = False
                    For j = 1 To Len(buf)
                        Select Case Mid$(buf, j, 1)
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
                        buf2 = Trim$(Left$(buf, ret - 1))
                        buf = Trim$(Mid$(buf, ret + 1))
                    Else
                        buf2 = buf
                        buf = ""
                    End If
                    
                    If buf2 <> "" Then
                        .AddFeature buf2
                    Else
                        DataErrorMessage "行頭から" & Format$(i) & _
                            "番目の地形効果の設定が間違っています。", _
                            fname, line_num, line_buf, data_name
                    End If
                Loop
                
                If EOF(FileNumber) Then
                    Close #FileNumber
                    Exit Sub
                End If
                
                GetLine FileNumber, line_buf, line_num
            Loop
        End With
    Loop
    
ErrorHandler:
    'エラー処理
    If line_num = 0 Then
        ErrorMessage fname & "が開けません。"
    Else
        Close #FileNumber
        DataErrorMessage err_msg, fname, line_num, line_buf, data_name
    End If
    
    End
End Sub

'リストをクリア
Public Sub Clear()
Dim i As Integer
    
    For i = 0 To MAX_TERRAIN_DATA_NUM - 1
        TerrainDataList(i).Clear
    Next
    Count = 0
End Sub

