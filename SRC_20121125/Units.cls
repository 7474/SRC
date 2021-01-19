VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Units"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'全ユニットのデータを管理するリストのクラス

'ユニットＩＤ作成用カウンタ
Public IDCount As Long

'ユニット一覧
Private colUnits As New Collection

'クラスの初期化
Private Sub Class_Initialize()
    IDCount = 0
End Sub

'クラスの解放
Private Sub Class_Terminate()
Dim i As Integer
    
    With colUnits
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colUnits = Nothing
End Sub

'ForEach用関数
Public Function NewEnum() As IUnknown
Attribute NewEnum.VB_UserMemId = -4
Attribute NewEnum.VB_MemberFlags = "40"
    Set NewEnum = colUnits.[_NewEnum]
End Function


'ユニットリストに新しいユニットを追加
Public Function Add(uname As String, ByVal urank As Integer, uparty As String) As Unit
Dim new_unit As Unit
Dim new_form As Unit
Dim ud As UnitData
Dim uname2 As String
Dim other_forms() As String
Dim i As Integer, j As Integer, list As String
    
    'ユニットデータが定義されている？
    If Not UDList.IsDefined(uname) Then
        Set Add = Nothing
        Exit Function
    End If
    
    Set ud = UDList.Item(uname)
    
    Set new_unit = New Unit
    Set Add = new_unit
    With new_unit
        .Name = ud.Name
        .Rank = urank
        .Party = uparty
        .ID = CreateID(ud.Name)
        .FullRecover
    End With
    colUnits.Add new_unit, new_unit.ID
    
    'これ以降は本体以外の形態の追加
    ReDim other_forms(0)
    
    '変形先の形態
    list = ud.FeatureData("変形")
    For i = 2 To LLength(list)
        uname2 = LIndex(list, i)
        If Not UDList.IsDefined(uname2) Then
            ErrorMessage "ユニットデータ「" & uname & "」の変形先形態「" _
                & uname2 & "」が見つかりません"
            Set Add = Nothing
            Exit Function
        End If
        ReDim Preserve other_forms(UBound(other_forms) + 1)
        other_forms(UBound(other_forms)) = uname2
    Next
    
    'ハイパーモード先の形態
    If ud.IsFeatureAvailable("ハイパーモード") Then
        list = ud.FeatureData("ハイパーモード")
        uname2 = LIndex(list, 2)
        If Not UDList.IsDefined(uname2) Then
            If uname = "" Then
                ErrorMessage "ユニットデータ「" & uname & _
                    "」のハイパーモード先形態が指定されていません"
            Else
                ErrorMessage "ユニットデータ「" & uname & "」のハイパーモード先形態「" _
                    & uname2 & "」が見つかりません"
            End If
            Set Add = Nothing
            Exit Function
        End If
        ReDim Preserve other_forms(UBound(other_forms) + 1)
        other_forms(UBound(other_forms)) = uname2
    End If
    
    'ノーマルモード先の形態
    If ud.IsFeatureAvailable("ノーマルモード") Then
        list = ud.FeatureData("ノーマルモード")
        uname2 = LIndex(list, 1)
        If Not UDList.IsDefined(uname2) Then
            If uname2 = "" Then
                ErrorMessage "ユニットデータ「" & uname & _
                    "」のノーマルモード先形態が指定されていません"
            Else
                ErrorMessage "ユニットデータ「" & uname & "」のノーマルモード先形態「" _
                    & uname2 & "」が見つかりません"
            End If
            Set Add = Nothing
            Exit Function
        End If
        ReDim Preserve other_forms(UBound(other_forms) + 1)
        other_forms(UBound(other_forms)) = uname2
    End If
    
    'パーツ分離先の形態
    If ud.IsFeatureAvailable("パーツ分離") Then
        uname2 = LIndex(ud.FeatureData("パーツ分離"), 2)
        If Not UDList.IsDefined(uname2) Then
            If uname2 = "" Then
                ErrorMessage "ユニットデータ「" & uname & _
                    "」のパーツ分離先形態が指定されていません"
            Else
                ErrorMessage "ユニットデータ「" & uname & "」のパーツ分離先形態「" _
                    & uname2 & "」が見つかりません"
            End If
            Set Add = Nothing
            Exit Function
        End If
        ReDim Preserve other_forms(UBound(other_forms) + 1)
        other_forms(UBound(other_forms)) = uname2
    End If
    
    'パーツ合体先の形態
    If ud.IsFeatureAvailable("パーツ合体") Then
        uname2 = ud.FeatureData("パーツ合体")
        If Not UDList.IsDefined(uname2) Then
            If uname = "" Then
                ErrorMessage "ユニットデータ「" & uname & _
                    "」のパーツ合体先形態が指定されていません"
            Else
                ErrorMessage "ユニットデータ「" & uname & "」のパーツ合体先形態「" _
                    & uname2 & "」が見つかりません"
            End If
            Set Add = Nothing
            Exit Function
        End If
        ReDim Preserve other_forms(UBound(other_forms) + 1)
        other_forms(UBound(other_forms)) = uname2
    End If
    
    '変形技の変形先の形態
    With ud
        If .IsFeatureAvailable("変形技") Then
            For i = 1 To .CountFeature
                If .Feature(i) = "変形技" Then
                    uname2 = LIndex(.FeatureData(i), 2)
                    If Not UDList.IsDefined(uname2) Then
                        If uname2 = "" Then
                            ErrorMessage "ユニットデータ「" & uname & _
                                "」の変形技使用後形態が指定されていません"
                        Else
                            ErrorMessage "ユニットデータ「" & uname & _
                                "」の変形技使用後形態「" & _
                                uname2 & "」が見つかりません"
                        End If
                        Set Add = Nothing
                        Exit Function
                    End If
                    ReDim Preserve other_forms(UBound(other_forms) + 1)
                    other_forms(UBound(other_forms)) = uname2
                End If
            Next
        End If
    End With
    
    '換装先の形態
    list = ud.FeatureData("換装")
    For i = 1 To LLength(list)
        uname2 = LIndex(list, i)
        If Not UDList.IsDefined(uname2) Then
            ErrorMessage "ユニットデータ「" & uname & "」の換装先形態「" _
                & uname2 & "」が見つかりません"
            Set Add = Nothing
            Exit Function
        End If
        ReDim Preserve other_forms(UBound(other_forms) + 1)
        other_forms(UBound(other_forms)) = uname2
    Next
    
    '他形態で指定された形態
    list = ud.FeatureData("他形態")
    For i = 1 To LLength(list)
        uname2 = LIndex(list, i)
        If Not UDList.IsDefined(uname2) Then
            ErrorMessage "ユニットデータ「" & uname & "」の他形態「" _
                & uname2 & "」が見つかりません"
            Set Add = Nothing
            Exit Function
        End If
        ReDim Preserve other_forms(UBound(other_forms) + 1)
        other_forms(UBound(other_forms)) = uname2
    Next
    
    '形態を追加
    For i = 1 To UBound(other_forms)
        If Not new_unit.IsOtherFormDefined(other_forms(i)) Then
            Set new_form = New Unit
            With new_form
                .Name = other_forms(i)
                .Rank = urank
                .Party = uparty
                .ID = CreateID(ud.Name)
                .FullRecover
                .Status = "他形態"
            End With
            colUnits.Add new_form, new_form.ID
            new_unit.AddOtherForm new_form
        End If
    Next
    
    '追加した形態に対して自分自身を追加しておく
    For i = 1 To new_unit.CountOtherForm
        new_unit.OtherForm(i).AddOtherForm new_unit
        For j = 1 To new_unit.CountOtherForm
            If Not i = j Then
                new_unit.OtherForm(i).AddOtherForm _
                    new_unit.OtherForm(j)
            End If
        Next
    Next
    
    '既に合体先 or 分離先のユニットが作成されていれば自分は他形態
    With ud
        For i = 1 To .CountFeature
            If .Feature(i) = "合体" Then
                If UList.IsDefined(LIndex(.FeatureData(i), 2)) Then
                    new_unit.Status = "他形態"
                    Exit Function
                End If
            End If
            If .Feature(i) = "分離" Then
                For j = 2 To LLength(.FeatureData(i))
                    If UList.IsDefined(LIndex(.FeatureData(i), j)) Then
                        new_unit.Status = "他形態"
                        Exit Function
                    End If
                Next
            End If
        Next
    End With
End Function

'ユニットリストにユニット u を追加
Public Sub Add2(u As Unit)
    colUnits.Add u, u.ID
End Sub

'新規ユニットIDを作成
Public Function CreateID(uname As String) As String
    Do
        IDCount = IDCount + 1
    Loop Until Not IsDefined(uname & ":" & Format$(IDCount))
    CreateID = uname & ":" & Format$(IDCount)
End Function

'ユニットリストに登録されているユニット数を返す
Public Function Count() As Integer
    Count = colUnits.Count
End Function

'ユニットリストからユニットを削除
Public Sub Delete(Index As Variant)
    colUnits.Remove Index
End Sub

'ユニットリストから指定されたユニットを返す
Public Function Item(Index As Variant) As Unit
Dim u As Unit
Dim uname As String

    On Error GoTo ErrorHandler
    Set Item = colUnits.Item(Index)
    Exit Function

ErrorHandler:
    'IDで見つからなければユニット名で検索
    uname = CStr(Index)
    For Each u In colUnits
        With u
            If .Name = uname Then
                If .Status <> "破棄" Then
                    Set Item = u
                    Exit Function
                End If
            End If
        End With
    Next
    Set Item = Nothing
End Function

'ユニットリストからユニットを検索 (IDのみ)
Public Function Item2(Index As Variant) As Unit
    On Error GoTo ErrorHandler
    Set Item2 = colUnits.Item(Index)
    Exit Function

ErrorHandler:
    Set Item2 = Nothing
End Function

'ユニットリストに指定されたユニットが定義されているか？
Public Function IsDefined(Index As Variant) As Boolean
Dim u As Unit
Dim uname As String

    On Error GoTo ErrorHandler
    Set u = colUnits.Item(Index)
    IsDefined = True
    Exit Function
    
ErrorHandler:
    'IDで見つからなければユニット名で検索
    uname = CStr(Index)
    For Each u In colUnits
        With u
            If .Name = uname Then
                If .Status <> "破棄" Then
                    IsDefined = True
                    Exit Function
                End If
            End If
        End With
    Next
    IsDefined = False
End Function

'ユニットリストに指定されたユニットが定義されているか？ (IDのみ)
Public Function IsDefined2(Index As Variant) As Boolean
Dim u As Unit

    On Error GoTo ErrorHandler
    Set u = colUnits.Item(Index)
    IsDefined2 = True
    Exit Function
    
ErrorHandler:
    IsDefined2 = False
End Function

'ユニットリストをアップデート
Public Sub Update()
Dim u As Unit
Dim i As Integer, j As Integer, k As Integer, n As Integer
Dim prev_money As Long, flag As Boolean
Dim uname As String, uname2 As String, pname As String, buf As String

    '母艦に格納されたユニットを降ろす
    For Each u In colUnits
        With u
            For i = 1 To .CountUnitOnBoard
                .UnloadUnit 1
            Next
        End With
    Next
    
    '破壊された味方ユニットがあるか検索
    For Each u In colUnits
        With u
            If .Party0 = "味方" Then
                If .Status = "破壊" Then
                    flag = True
                    Exit For
                End If
            ElseIf .Party0 = "ＮＰＣ" Then
                If .Status = "破壊" Then
                    If Not .Summoner Is Nothing Then
                        If .Summoner.Party0 = "味方" Then
                            flag = True
                            Exit For
                        End If
                    End If
                End If
            End If
        End With
    Next
    
    '破壊された味方ユニットがあれば修理
    If flag Then
        OpenMessageForm
        prev_money = Money
        For Each u In colUnits
            With u
                If .Status <> "破壊" Then
                    GoTo NextDestroyedUnit
                End If
                If .IsFeatureAvailable("召喚ユニット") Then
                    GoTo NextDestroyedUnit
                End If
                Select Case .Party0
                    Case "味方"
                    Case "ＮＰＣ"
                        If .Summoner Is Nothing Then
                            GoTo NextDestroyedUnit
                        ElseIf .Summoner.Party0 <> "味方" Then
                            GoTo NextDestroyedUnit
                        End If
                    Case Else
                        GoTo NextDestroyedUnit
                End Select
                
                IncrMoney -.Value
                .Status = "待機"
                If Not .IsHero Then
                    DisplayMessage "システム", .Nickname & "を修理した;修理費 = " _
                        & Format$(.Value)
                Else
                    DisplayMessage "システム", .Nickname & "を治療した;治療費 = " _
                        & Format$(.Value)
                End If
            End With
NextDestroyedUnit:
        Next
        DisplayMessage "システム", "合計 = " & Format$(prev_money - Money)
        CloseMessageForm
    End If
    
    '全ユニットを待機状態に変更
    For Each u In colUnits
        With u
            Select Case .Status
                Case "出撃", "格納"
                    .Status = "待機"
            End Select
        End With
    Next
    
    '３段階までの変形・合体に対応
    For i = 1 To 3
        'ノーマルモード・パーツ合体を行う
        For Each u In colUnits
            With u
                If .Party0 = "味方" _
                    And .Status <> "他形態" _
                    And .Status <> "旧主形態" _
                    And .Status <> "旧形態" _
                Then
                    If .IsFeatureAvailable("ノーマルモード") Then
                        .Transform LIndex(.FeatureData("ノーマルモード"), 1)
                    ElseIf .IsFeatureAvailable("パーツ合体") Then
                        If LLength(.FeatureData("パーツ合体")) = 2 Then
                            .Transform LIndex(.FeatureData("パーツ合体"), 2)
                        Else
                            .Transform LIndex(.FeatureData("パーツ合体"), 1)
                        End If
                    End If
                End If
            End With
        Next
        
        '分離を行う
        For Each u In colUnits
            With u
                If Not .IsFeatureAvailable("分離") Then
                    GoTo NextLoop1
                End If
                If .Party0 <> "味方" _
                    Or .Status = "他形態" _
                    Or .Status = "旧主形態" _
                    Or .Status = "旧形態" _
                Then
                    GoTo NextLoop1
                End If
                
                If .Status = "破棄" Then
                    If .CountPilot = 0 Then
                        GoTo NextLoop1
                    End If
                End If
                
                '合体形態が主形態なら分離を行わない
                
                If LLength(.FeatureData("分離")) > 3 _
                    And Not .IsFeatureAvailable("制限時間") _
                Then
                    GoTo NextLoop1
                End If
                
                If .IsFeatureAvailable("主形態") Then
                    GoTo NextLoop1
                End If
                
                'パイロットが足らない場合は分離を行わない
                n = 0
                For j = 2 To LLength(.FeatureData("分離"))
                    uname = LIndex(.FeatureData("分離"), j)
                    If UDList.IsDefined(uname) Then
                        With UDList.Item(uname)
                            If Not .IsFeatureAvailable("召喚ユニット") Then
                                n = n + .PilotNum
                            End If
                        End With
                    End If
                Next
                If .CountPilot < n Then
                    GoTo NextLoop1
                End If
                
                '分離先の形態が利用可能？
                For j = 2 To LLength(.FeatureData("分離"))
                    uname = LIndex(.FeatureData("分離"), j)
                    If Not UList.IsDefined(uname) Then
                        GoTo NextLoop1
                    End If
                    If UList.Item(uname).CurrentForm.Status = "待機" Then
                        GoTo NextLoop1
                    End If
                Next
                
                '分離を実施
                .Split
            End With
NextLoop1:
        Next
        
        '合体を行う
        For Each u In colUnits
            With u
                If .Party0 = "味方" _
                    And .Status <> "他形態" _
                    And .Status <> "旧主形態" _
                    And .Status <> "旧形態" _
                Then
                    If .IsFeatureAvailable("合体") Then
                        For j = 1 To .CountFeature
                            If .Feature(j) <> "合体" Then
                                GoTo NextLoop2
                            End If
                            
                            '合体後の形態が利用可能？
                            uname = LIndex(.FeatureData(j), 2)
                            If Not UList.IsDefined(uname) Then
                                GoTo NextLoop2
                            End If
                            With UList.Item(uname)
                                If u.Status = "待機" And .CurrentForm.Status = "離脱" Then
                                    GoTo NextLoop2
                                End If
                                If .IsFeatureAvailable("制限時間") Then
                                    GoTo NextLoop2
                                End If
                                If Not .IsFeatureAvailable("主形態") _
                                    And LLength(u.FeatureData(j)) = 3 _
                                Then
                                    GoTo NextLoop2
                                End If
                            End With
                            
                            '合体のパートナーが利用可能？
                            For k = 3 To LLength(.FeatureData(j))
                                uname = LIndex(.FeatureData(j), k)
                                If Not UList.IsDefined(uname) Then
                                    GoTo NextLoop2
                                End If
                                With UList.Item(uname)
                                    If u.Status = "待機" Then
                                        If .CurrentForm.Status <> "待機" Then
                                            GoTo NextLoop2
                                        End If
                                    Else
                                        If .CurrentForm.Status <> "離脱" Then
                                            GoTo NextLoop2
                                        End If
                                    End If
                                End With
                            Next
                            
                            '合体を実施
                            .Combine LIndex(.FeatureData(j), 2)
                            Exit For
NextLoop2:
                        Next
                    End If
                End If
            End With
        Next
        
        '標準形態に変形
        For Each u In colUnits
            With u
                If .Party0 = "味方" _
                    And .Status <> "他形態" _
                    And .Status <> "旧主形態" _
                    And .Status <> "旧形態" _
                Then
                    If .IsFeatureAvailable("変形") Then
                        uname = .Name
                        buf = .FeatureData("変形")
                        For j = 2 To LLength(buf)
                            uname2 = LIndex(buf, j)
                            If UDList.IsDefined(uname2) Then
                                If UDList.Item(uname2).ID < UDList.Item(uname).ID Then
                                    uname = uname2
                                End If
                            Else
                                ErrorMessage uname & "の変形先ユニット「" _
                                    & uname2 & "」のデータが定義されていません。"
                            End If
                        Next
                        
                        If uname <> .Name Then
                            .Transform uname
                        End If
                    End If
                End If
            End With
        Next
    Next
    
    '暴走時パイロットを削除
    For Each u In colUnits
        With u
            If .IsFeatureAvailable("暴走時パイロット") Then
                If PList.IsDefined(.FeatureData("暴走時パイロット")) Then
                    PList.Delete .FeatureData("暴走時パイロット")
                End If
            End If
        End With
    Next
    
    'ダミーパイロットを削除
    For Each u In colUnits
        With u
            If .CountPilot > 0 Then
                If .Pilot(1).Nickname0 = "パイロット不在" Then
                    .DeletePilot 1
                End If
            End If
        End With
    Next
    
    '変身先の形態等、一時的な形態を削除
    For Each u In colUnits
        With u
            If .Status = "待機" Then
                .DeleteTemporaryOtherForm
            End If
        End With
    Next
    
    '破棄されたユニットを削除
    For Each u In colUnits
        With u
            '召喚ユニットは必ず破棄
            If .IsFeatureAvailable("召喚ユニット") Then
                .Status = "破棄"
            End If
            'ダミーユニットを破棄
            If .IsFeatureAvailable("ダミーユニット") Then
                .Status = "破棄"
            End If
            
            '味方ユニット以外のユニットと破棄されたユニットを削除
            If .Party0 <> "味方" Or .Status = "破棄" Then
                'ユニットが装備しているアイテムも破棄
                For i = 1 To .CountItem
                    .Item(i).Exist = False
                Next
                Delete .ID
            End If
        End With
    Next
    
    'ユニットの状態を回復
    For Each u In colUnits
        u.Reset
    Next
    
    'ステータスをアップデート
    For Each u In colUnits
        u.Update True
    Next
End Sub


'ユニットリストに登録されたユニットの情報をセーブ
Public Sub Save()
Dim i As Integer
Dim u As Unit
    
    Write #SaveDataFileNumber, IDCount
    Write #SaveDataFileNumber, Count
    For Each u In colUnits
        With u
            Write #SaveDataFileNumber, .Name
            Write #SaveDataFileNumber, .ID, .Rank, .Status
            
            Write #SaveDataFileNumber, .CountOtherForm
            For i = 1 To .CountOtherForm
                Write #SaveDataFileNumber, .OtherForm(i).ID
            Next
            
            Write #SaveDataFileNumber, .CountPilot
            For i = 1 To .CountPilot
                Write #SaveDataFileNumber, .Pilot(i).ID
            Next
            
            Write #SaveDataFileNumber, .CountSupport
            For i = 1 To .CountSupport
                Write #SaveDataFileNumber, .Support(i).ID
            Next
            
            Write #SaveDataFileNumber, .CountItem
            For i = 1 To .CountItem
                Write #SaveDataFileNumber, .Item(i).ID
            Next
        End With
    Next
End Sub

'ユニットリストにユニットの情報をロード
'(リンクは後で行う)
Public Sub Load()
Dim num As Integer, num2 As Integer
Dim new_unit As Unit
Dim Name As String
Dim ID As String, Rank As Integer, Status As String
Dim i As Integer, j As Integer
Dim dummy As String
    
    Input #SaveDataFileNumber, IDCount
    Input #SaveDataFileNumber, num
    For i = 1 To num
        Set new_unit = New Unit
        With new_unit
            'Name
            Input #SaveDataFileNumber, Name
            
            If Not UDList.IsDefined(Name) Then
                ErrorMessage Name & "のデータが定義されていません"
                TerminateSRC
                End
            End If
            
            'ID, Rank, Status
            Input #SaveDataFileNumber, ID, Rank, Status
            
            '旧形式のユニットＩＤを新形式に変換
            If SaveDataVersion < 10700 Then
                ConvertUnitID ID
            End If
            
            .Name = Name
            .ID = ID
            .Rank = Rank
            .Party = "味方"
            .Status = Status
            .FullRecover
        End With
        colUnits.Add new_unit, new_unit.ID
        
        'OtherForm
        Input #SaveDataFileNumber, num2
        For j = 1 To num2
            Line Input #SaveDataFileNumber, dummy
        Next
        
        'Pilot
        Input #SaveDataFileNumber, num2
        For j = 1 To num2
            Line Input #SaveDataFileNumber, dummy
        Next
        
        'Support
        Input #SaveDataFileNumber, num2
        For j = 1 To num2
            Line Input #SaveDataFileNumber, dummy
        Next
        
        'Item
        Input #SaveDataFileNumber, num2
        For j = 1 To num2
            Line Input #SaveDataFileNumber, dummy
        Next
    Next
End Sub

'ユニットリストにユニットの情報をロードし、リンクを行う
Public Sub LoadLinkInfo()
Dim num As Integer, num2 As Integer
Dim ID As String, ID2 As String
Dim i As Integer, j As Integer
Dim int_dummy As Integer
Dim str_dummy As String
Dim u As Unit

    Input #SaveDataFileNumber, IDCount
    Input #SaveDataFileNumber, num
    
    For i = 1 To num
        'Name
        Line Input #SaveDataFileNumber, str_dummy
        'ID, Rank, Status
        Input #SaveDataFileNumber, ID, int_dummy, str_dummy
        
        '旧形式のユニットＩＤを新形式に変換
        If SaveDataVersion < 10700 Then
            ConvertUnitID ID
        End If
        
        With Item(ID)
            'OtherForm
            Input #SaveDataFileNumber, num2
            For j = 1 To num2
                Input #SaveDataFileNumber, ID2
                ConvertUnitID ID2
                If IsDefined(ID2) Then
                    .AddOtherForm Item(ID2)
                End If
            Next
            
            'Pilot
            Input #SaveDataFileNumber, num2
            For j = 1 To num2
                Input #SaveDataFileNumber, ID2
                If PList.IsDefined(ID2) Then
                    .AddPilot PList.Item(ID2)
                    If .Status = "離脱" Then
                        PList.Item(ID2).Away = True
                    End If
                Else
                    ID2 = Left$(ID2, InStr(ID2, "(") - 1)
                    If PList.IsDefined(ID2) Then
                        .AddPilot PList.Item(ID2)
                        If .Status = "離脱" Then
                            PList.Item(ID2).Away = True
                        End If
                    End If
                End If
            Next
            
            'Support
            Input #SaveDataFileNumber, num2
            For j = 1 To num2
                Input #SaveDataFileNumber, ID2
                If PList.IsDefined(ID2) Then
                    .AddSupport PList.Item(ID2)
                    If .Status = "離脱" Then
                        PList.Item(ID2).Away = True
                    End If
                End If
            Next
            
            'Unit
            Input #SaveDataFileNumber, num2
            For j = 1 To num2
                Input #SaveDataFileNumber, ID2
                If IList.IsDefined(ID2) Then
                    If IList.Item(ID2).Unit Is Nothing Then
                        .CurrentForm.AddItem0 IList.Item(ID2)
                    End If
                ElseIf IDList.IsDefined(ID2) Then
                    .CurrentForm.AddItem0 IList.Add(ID2)
                End If
            Next
        End With
    Next
    
    For Each u In colUnits
        u.Update True
    Next
End Sub


'一時中断用データをファイルにセーブする
Public Sub Dump()
Dim u As Unit

    Write #SaveDataFileNumber, Count
    
    For Each u In colUnits
        u.Dump
    Next
End Sub

'一時中断用データをファイルからロードする
Public Sub Restore()
Dim i As Integer, num As Integer
Dim u As Unit
    
    With colUnits
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    
    Input #SaveDataFileNumber, num
    
    For i = 1 To num
        Set u = New Unit
        With u
            .Restore
            colUnits.Add u, .ID
        End With
    Next
End Sub

'一時中断用データのリンク情報をファイルからロードする
Public Sub RestoreLinkInfo()
Dim u As Unit
Dim num As Integer

    Input #SaveDataFileNumber, num
    
    For Each u In colUnits
        u.RestoreLinkInfo
    Next
End Sub

'一時中断用データのパラメータ情報をファイルからロードする
Public Sub RestoreParameter()
Dim u As Unit
Dim num As Integer

    Input #SaveDataFileNumber, num
    
    For Each u In colUnits
        u.RestoreParameter
    Next
End Sub


'ユニットリストをクリア
Public Sub Clear()
Dim i As Integer
    
    For i = 1 To Count
        Delete 1
    Next
End Sub

'ユニットリストに登録されたユニットのビットマップIDをクリア
Public Sub ClearUnitBitmap()
Dim u As Unit
    
    With MainForm.picUnitBitmap
      If .Width = 32 Then
          '既にクリアされていればそのまま終了
          Exit Sub
      End If
      
      '画像をクリア
      .Picture = LoadPicture("")
      .Move 0, 0, 32, 96
    End With
    
    'BitmapIDをクリア
    For Each u In colUnits
        u.BitmapID = 0
    Next
End Sub


'ハイパーモードの自動発動チェック
Public Sub CheckAutoHyperMode()
Dim u As Unit

    For Each u In colUnits
        u.CheckAutoHyperMode
    Next
End Sub

'ノーマルモードの自動発動チェック
Public Sub CheckAutoNormalMode()
Dim u As Unit
Dim is_redraw_necessary As Boolean

    For Each u In colUnits
        If u.CheckAutoNormalMode(True) Then
            is_redraw_necessary = True
        End If
    Next
    
    '画面の再描画が必要？
    If is_redraw_necessary Then
        RedrawScreen
    End If
End Sub

'破棄されたユニットを削除
Public Sub Clean()
Dim u As Unit
Dim i As Integer

    For Each u In colUnits
        With u
            '出撃していないユニットは味方ユニット以外全て削除
            If .Party0 <> "味方" Then
                If .Status = "待機" Or .Status = "破壊" Then
                    .Status = "破棄"
                    For i = 1 To .CountOtherForm
                        .OtherForm(i).Status = "破棄"
                    Next
                End If
            End If
        End With
    Next
    
    For Each u In colUnits
        With u
            '破棄されたユニットを削除
            If .Status = "破棄" Then
                'ユニットに乗っているパイロットも破棄
                For i = 1 To .CountPilot
                    .Pilot(i).Alive = False
                Next
                For i = 1 To .CountSupport
                    .Support(i).Alive = False
                Next
                
                'ユニットが装備しているアイテムも破棄
                For i = 1 To .CountItem
                    .Item(i).Exist = False
                Next
                
                Delete .ID
            End If
        End With
    Next
End Sub
