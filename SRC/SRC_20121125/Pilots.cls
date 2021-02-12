VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Pilots"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'作成されたパイロットのデータを管理するリストクラス

'パイロット一覧
Private colPilots As New Collection

'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer
    
    With colPilots
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colPilots = Nothing
End Sub

'ForEach用関数
Public Function NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
Attribute NewEnum.VB_MemberFlags = "40"
    Set NewEnum = colPilots.[_NewEnum]
End Function


'パイロットを追加
Public Function Add(pname As String, ByVal plevel As Integer, _
    pparty As String, Optional gid As String) As Pilot
Dim new_pilot As New Pilot
Dim key As String
Dim i As Integer

    With new_pilot
        .Name = PDList.Item(pname).Name
        .Level = plevel
        .Party = pparty
        .FullRecover
        .Alive = True
        Set .Unit = Nothing
        
        If gid = "" Then
            'グループＩＤが指定されていない場合
            If InStr(.Name, "(ザコ)") = 0 _
                And InStr(.Name, "(汎用)") = 0 _
            Then
                key = .Name
               If PList.IsDefined2(key) Then
                   If PList.Item2(key).ID = key Then
                       '一度作成されたパイロットを復活させる場合
                       If Not PList.Item2(key).Alive Then
                           With PList.Item2(key)
                               .Level = plevel
                               .Party = pparty
                               .FullRecover
                               .Alive = True
                           End With
                           Set Add = PList.Item2(key)
                           Exit Function
                       End If
                       
                       ErrorMessage key & "というパイロットは既に登録されています"
                       Exit Function
                   End If
               End If
            Else
                i = colPilots.Count
                Do
                    i = i + 1
                    key = .Name & "_" & Format$(i)
                Loop While PList.IsDefined2(key)
            End If
        Else
            'グループＩＤが指定されている場合
            key = gid
            i = 1
            Do While PList.IsDefined2(key)
                i = i + 1
                key = gid & ":" & Format$(i)
            Loop
        End If
        
        .ID = key
        colPilots.Add new_pilot, key
        Set Add = new_pilot
    End With
End Function

'登録されているパイロットの総数
Public Function Count() As Integer
    Count = colPilots.Count
End Function

'リストからパイロットを削除
Public Sub Delete(Index As Variant)
    colPilots.Remove Index
End Sub

'リストからパイロットを検索
Public Function Item(Index As Variant) As Pilot
Dim p As Pilot
Dim pname As String

    On Error GoTo ErrorHandler
    Set Item = colPilots.Item(Index)
    If Item.Alive Then
        Exit Function
    End If
    
ErrorHandler:
    'ＩＤで見つからなければ名称で検索
    pname = CStr(Index)
    For Each p In colPilots
        If p.Data.Name = pname Then
            If p.Alive Then
                Set Item = p
                Exit Function
            End If
        End If
    Next
    'それでも見つからなければ愛称で検索
    For Each p In colPilots
        If p.Data.Nickname = pname Then
            If p.Alive Then
                Set Item = p
                Exit Function
            End If
        End If
    Next
End Function

'パイロットが定義されているか
Public Function IsDefined(Index As Variant) As Boolean
Dim p As Pilot
Dim pname As String

    On Error GoTo ErrorHandler
    Set p = colPilots.Item(Index)
    If p.Alive Then
        IsDefined = True
        Exit Function
    End If
    
ErrorHandler:
    'ＩＤで見つからなければ名称で検索
    pname = CStr(Index)
    For Each p In colPilots
        If p.Data.Name = pname Then
            If p.Alive Then
                IsDefined = True
                Exit Function
            End If
        End If
    Next
    'それでも見つからなければ愛称で検索
    For Each p In colPilots
        If p.Data.Nickname = pname Then
            If p.Alive Then
                IsDefined = True
                Exit Function
            End If
        End If
    Next
End Function

'パイロットを検索 (IDのみで検索)
Public Function Item2(Index As Variant) As Pilot
    On Error GoTo ErrorHandler
    Set Item2 = colPilots.Item(Index)
    Exit Function
    
ErrorHandler:
    Set Item2 = Nothing
End Function

'パイロットが定義されているか (IDのみで検索)
Public Function IsDefined2(Index As Variant) As Boolean
Dim p As Pilot

    On Error GoTo ErrorHandler
    Set p = colPilots.Item(Index)
    IsDefined2 = True
    Exit Function
    
ErrorHandler:
    IsDefined2 = False
End Function

'リストのアップデート
Public Sub Update()
Dim p As Pilot
Dim i As Integer

    For Each p In colPilots
        With p
            If .Party <> "味方" Or Not .Alive Then
                '味方でないパイロットや破棄されたパイロットは削除
                Delete .ID
            ElseIf .IsAdditionalPilot Then
                '追加パイロットは削除
                If Not .Unit Is Nothing Then
                    With .Unit
                        Set .pltAdditionalPilot = Nothing
                        For i = 1 To .CountOtherForm
                            Set .OtherForm(i).pltAdditionalPilot = Nothing
                        Next
                    End With
                End If
                Delete .ID
            ElseIf .IsAdditionalSupport Then
                '追加サポートは削除
                If Not .Unit Is Nothing Then
                    With .Unit
                        Set .pltAdditionalSupport = Nothing
                        For i = 1 To .CountOtherForm
                            Set .OtherForm(i).pltAdditionalSupport = Nothing
                        Next
                    End With
                End If
                Delete .ID
            ElseIf .Nickname0 = "パイロット不在" Then
                'ダミーパイロットは削除
                Delete .ID
            ElseIf Not .Unit Is Nothing Then
                If .Unit.IsFeatureAvailable("召喚ユニット") Then
                    '召喚ユニットの追加パイロットも削除
                    Delete .ID
                End If
            End If
        End With
    Next
    
    '残ったパイロットを全回復
    For Each p In colPilots
        p.FullRecover
    Next
End Sub


'ファイルにデータをセーブ
Public Sub Save()
Dim num As Integer
Dim p As Pilot

    '追加パイロットや追加サポートを除いたパイロット数を算出
    For Each p In colPilots
        With p
            If Not .IsAdditionalPilot And Not .IsAdditionalSupport Then
                num = num + 1
            End If
        End With
    Next
    Write #SaveDataFileNumber, num
    
    For Each p In colPilots
        With p
            '追加パイロットや追加サポートはセーブしない
            If Not .IsAdditionalPilot And Not .IsAdditionalSupport Then
                If .Name = .ID Then
                    Write #SaveDataFileNumber, .Name
                Else
                    Write #SaveDataFileNumber, .Name & " " & .ID
                End If
                Write #SaveDataFileNumber, .Level, .Exp
                If .Unit Is Nothing Then
                    If .Away Then
                        Write #SaveDataFileNumber, "離脱"
                    Else
                        Write #SaveDataFileNumber, "-"
                    End If
                Else
                    Write #SaveDataFileNumber, .Unit.ID
                End If
            End If
        End With
    Next
End Sub

'ファイルからデータをロード (リンクは行わない)
Public Sub Load()
Dim i As Integer, num As Integer
Dim pname As String
Dim plevel As Integer, pexp As Integer
Dim dummy As String
    
    Input #SaveDataFileNumber, num
    For i = 1 To num
        'Name
        Input #SaveDataFileNumber, pname
        'Level, Exp
        Input #SaveDataFileNumber, plevel, pexp
        'Unit
        Input #SaveDataFileNumber, dummy
        
        If LLength(pname) = 1 Then
            If Not PDList.IsDefined(pname) Then
                If InStr(pname, "(") > 0 Then
                    pname = Left$(pname, InStr(pname, "(") - 1)
                End If
                If Not PDList.IsDefined(pname) Then
                    ErrorMessage pname & "のデータが定義されていません"
                    TerminateSRC
                    End
                End If
            End If
            With Add(pname, plevel, "味方")
                .Exp = pexp
            End With
        Else
            If Not PDList.IsDefined(LIndex(pname, 1)) Then
                ErrorMessage LIndex(pname, 1) & "のデータが定義されていません"
                TerminateSRC
                End
            End If
            With Add(LIndex(pname, 1), plevel, "味方", LIndex(pname, 2))
                .Exp = pexp
            End With
        End If
    Next
End Sub

'ファイルからデータを読み込みリンク
Public Sub LoadLinkInfo()
Dim i As Integer, ret As Integer, num As Integer
Dim pname As String, uid As String
Dim dummy As String
Dim u As Unit

    Input #SaveDataFileNumber, num
    For i = 1 To num
        'Name
        Input #SaveDataFileNumber, pname
        'Level, Exp
        Line Input #SaveDataFileNumber, dummy
        'Unit
        Input #SaveDataFileNumber, uid
        
        If LLength(pname) = 1 Then
            If Not IsDefined(pname) Then
                pname = Left$(pname, InStr(pname, "(") - 1)
            End If
        End If
        
        Select Case uid
            Case "離脱"
                'Leaveされたパイロット
                If LLength(pname) = 1 Then
                    Item(pname).Away = True
                Else
                    Item(LIndex(pname, 2)).Away = True
                End If
                GoTo NextPilot
            Case "-", "Dummy"
                'ユニットに乗っていないパイロット
                GoTo NextPilot
        End Select
        
        '旧形式のユニットＩＤを新形式に変換
        If SaveDataVersion < 10700 Then
            ConvertUnitID uid
        End If
        
        If UList.IsDefined(uid) Then
            'パイロットをユニットに乗せる
            Set u = UList.Item(uid)
            If LLength(pname) = 1 Then
                Set Item(pname).Unit = u
            Else
                Set Item(LIndex(pname, 2)).Unit = u
            End If
        Else
            '乗せるべきユニットが見つからなかった場合は強制的にユニットを
            '作って乗せる (バグ対策だったけど……不要？)
            ret = InStr(uid, ":")
            uid = Left$(uid, ret - 1)
            If UDList.IsDefined(uid) Then
                Set u = UList.Add(uid, 0, "味方")
                If LLength(pname) = 1 Then
                    Item(pname).Ride u
                Else
                    Item(LIndex(pname, 2)).Ride u
                End If
            End If
        End If
NextPilot:
    Next
End Sub


'一時中断用データをセーブする
Public Sub Dump()
Dim p As Pilot
Dim num As Integer

    '追加パイロットを除いたパイロット数を算出
    For Each p In colPilots
        With p
            If Not .IsAdditionalPilot Then
                num = num + 1
            End If
        End With
    Next
    Write #SaveDataFileNumber, num
    
    For Each p In colPilots
        With p
            '追加パイロットはセーブしない
            If Not .IsAdditionalPilot Then
                .Dump
            End If
        End With
    Next
End Sub

'一時中断用データをファイルからロードする
Public Sub Restore()
Dim i As Integer, num As Integer, buf As String
Dim p As Pilot
    
    With colPilots
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    
    Input #SaveDataFileNumber, num
    
    For i = 1 To num
        Set p = New Pilot
        With p
            .Restore
            colPilots.Add p, .ID
        End With
    Next
End Sub

'一時中断用データのリンク情報をファイルからロードする
Public Sub RestoreLinkInfo()
Dim i As Integer, num As Integer

    Input #SaveDataFileNumber, num
    
    For i = 1 To num
        colPilots(i).RestoreLinkInfo
    Next
End Sub

'一時中断用データのパラメータ情報をファイルからロードする
Public Sub RestoreParameter()
Dim i As Integer, num As Integer

    Input #SaveDataFileNumber, num
    
    For i = 1 To num
        colPilots(i).RestoreParameter
    Next
End Sub


'リストをクリア
Public Sub Clear()
Dim i As Integer

    For i = 1 To Count
        Delete 1
    Next
End Sub


'パイロットの支援修正を更新
Public Sub UpdateSupportMod(Optional ByVal u As Unit)
Dim p As Pilot
Dim i As Integer, xx As Integer, yy As Integer
Dim max_range As Integer, range As Integer
    
    If MapFileName = "" Then
        Exit Sub
    End If
    
    'ユニット指定がなければ全パイロットを更新
    If u Is Nothing Then
        For Each p In colPilots
            p.UpdateSupportMod
        Next
        Exit Sub
    End If
    
    With u
        'ユニットにパイロットが乗っていなければそのまま終了
        If .CountPilot = 0 Then
            Exit Sub
        End If
        
        With .MainPilot
            'メインパイロットを更新
            .UpdateSupportMod
            
            '支援範囲を算出
            max_range = .CommandRange
            
            If .IsSkillAvailable("広域サポート") Then
                max_range = MaxLng(max_range, 2)
            End If
            
            If IsOptionDefined("信頼補正") _
                And InStr(.Name, "(ザコ)") = 0 _
            Then
                If IsOptionDefined("信頼補正範囲拡大") Then
                    max_range = MaxLng(max_range, 2)
                Else
                    max_range = MaxLng(max_range, 1)
                End If
            End If
        End With
        
        '他のパイロットを更新
        For i = 2 To .CountPilot
            .Pilot(i).UpdateSupportMod
        Next
        For i = 1 To .CountSupport
            .Support(i).UpdateSupportMod
        Next
        
        '支援範囲が無いなら他のユニットに乗っているパイロットには影響無し
        If max_range = 0 Then
            Exit Sub
        End If
        
        '周りのユニットに乗っているパイロットの支援修正を更新
        For xx = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
            For yy = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
                If MapDataForUnit(xx, yy) Is Nothing Then
                    GoTo NextPoint
                End If
                
                '支援範囲内にいるかチェック
                range = Abs(.X - xx) + Abs(.Y - yy)
                If range > max_range Then
                    GoTo NextPoint
                End If
                If range = 0 Then
                    GoTo NextPoint
                End If
                
                '乗っているパイロット全員の支援修正を更新
                With MapDataForUnit(xx, yy)
                    If .CountPilot = 0 Then
                        GoTo NextPoint
                    End If
                    
                    .MainPilot.UpdateSupportMod
                    For i = 2 To .CountPilot
                        .Pilot(i).UpdateSupportMod
                    Next
                    For i = 1 To .CountSupport
                        .Support(i).UpdateSupportMod
                    Next
                End With
NextPoint:
            Next
        Next
    End With
End Sub

'破棄されたパイロットを削除する
Public Sub Clean()
Dim p As Pilot

    For Each p In colPilots
        With p
            If Not .Alive Then
                '破棄されたパイロットを削除
                Delete .ID
            End If
        End With
    Next
End Sub
