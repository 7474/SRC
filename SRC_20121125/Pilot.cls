VERSION 1.0 CLASS
BEGIN
  MultiUse = -1  'True
END
Attribute VB_Name = "Pilot"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = False
Attribute VB_Exposed = False
Option Explicit

' Copyright (C) 1997-2012 Kei Sakamoto / Inui Tetsuyuki
' 本プログラムはフリーソフトであり、無保証です。
' 本プログラムはGNU General Public License(Ver.3またはそれ以降)が定める条件の下で
' 再頒布または改変することができます。

'作成されたパイロットのクラス

'パイロットデータへのポインタ
Public Data As PilotData

'識別用ＩＤ
Public ID As String
'所属陣営
Public Party As String
'搭乗しているユニット
'未搭乗時は Nothing
Public Unit As Unit

'生きているかどうか
Public Alive As Boolean

'Leaveしているかどうか
Public Away As Boolean

'追加パイロットかどうか
Public IsAdditionalPilot As Boolean

'追加サポートかどうか
Public IsAdditionalSupport As Boolean

'サポートパイロットとして乗り込んだ時の順番
Public SupportIndex As Integer

'レベル
Private proLevel As Integer

'経験値
Private proEXP As Integer
'ＳＰ
Private proSP As Integer
'気力
Private proMorale As Integer
'霊力
Private proPlana As Integer

'能力値
Public Infight As Integer
Public Shooting As Integer
Public Hit As Integer
Public Dodge As Integer
Public Technique As Integer
Public Intuition As Integer
Public Adaption As String

'能力値の基本値
Public InfightBase As Integer
Public ShootingBase As Integer
Public HitBase As Integer
Public DodgeBase As Integer
Public TechniqueBase As Integer
Public IntuitionBase As Integer

'能力値の修正値

'特殊能力＆自ユニットによる修正
Public InfightMod As Integer
Public ShootingMod As Integer
Public HitMod As Integer
Public DodgeMod As Integer
Public TechniqueMod As Integer
Public IntuitionMod As Integer

'他ユニットによる支援修正
Public InfightMod2 As Integer
Public ShootingMod2 As Integer
Public HitMod2 As Integer
Public DodgeMod2 As Integer
Public TechniqueMod2 As Integer
Public IntuitionMod2 As Integer

'気力修正値
Public MoraleMod As Integer

'特殊能力
Private colSkill As New Collection


Private Sub Class_Terminate()
Dim i As Integer

    Set Data = Nothing
    Set Unit = Nothing
    
    With colSkill
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    Set colSkill = Nothing
End Sub

'名称
Public Property Get Name() As String
    Name = Data.Name
End Property

Public Property Let Name(new_name As String)
    Set Data = PDList.Item(new_name)
    Update
End Property

'愛称
Public Property Get Nickname0() As String
    Nickname0 = Data.Nickname
End Property

Public Property Get Nickname(Optional ByVal dont_call_unit_nickname As Boolean) As String
Dim idx As Integer
Dim u As Unit, uname As String, vname As String
    
    Nickname = Nickname0
    
    '愛称変更
    If Unit Is Nothing Then
        ReplaceSubExpression Nickname
        Exit Property
    End If
    With Unit
        If .CountPilot > 0 Then
            If Not .MainPilot Is Me Then
                ReplaceSubExpression Nickname
                Exit Property
            End If
        End If
        
        Set u = Unit
        
        'パイロットステータスコマンド中の場合はユニットを検索する必要がある
        If .Name = "ステータス表示用ダミーユニット" Then
            'メインパイロットかどうかチェック
            vname = "搭乗順番[" & ID & "]"
            If IsLocalVariableDefined(vname) Then
                If LocalVariableList.Item(vname).NumericValue <> 1 Then
                    Exit Property
                End If
            Else
                Exit Property
            End If
            
            vname = "搭乗ユニット[" & ID & "]"
            If IsLocalVariableDefined(vname) Then
                uname = LocalVariableList.Item(vname).StringValue
            End If
            If uname = "" Then
                Exit Property
            End If
            
            Set u = UList.Item(uname)
        End If
        
        With u
            If .IsFeatureAvailable("パイロット愛称") Then
                Nickname = .FeatureData("パイロット愛称")
                idx = InStr(Nickname, "$(愛称)")
                If idx > 0 Then
                    Nickname = Left$(Nickname, idx - 1) _
                        & Data.Nickname & Mid$(Nickname, idx + 5)
                End If
            End If
        End With
    End With
    
    'PilotのNickname()とUnitのNickname()の呼び出しが無限に続かないように
    'Nickname()への呼び出しは無効化
    If dont_call_unit_nickname Then
        ReplaceString Nickname, "Nickname()", ""
        ReplaceString Nickname, "nickname()", ""
    End If
    
    '愛称内の式置換のため、デフォルトユニットを一時的に変更する
    Set u = SelectedUnitForEvent
    Set SelectedUnitForEvent = Unit
    ReplaceSubExpression Nickname
    Set SelectedUnitForEvent = u
End Property

'読み仮名
Public Property Get KanaName() As String
Dim idx As Integer
Dim u As Unit, uname As String, vname As String
    
    KanaName = Data.KanaName
    
    '愛称変更
    If Unit Is Nothing Then
        ReplaceSubExpression KanaName
        Exit Property
    End If
    With Unit
        If .CountPilot > 0 Then
            If Not .MainPilot Is Me Then
                ReplaceSubExpression KanaName
                Exit Property
            End If
        End If
        
        Set u = Unit
        
        'パイロットステータスコマンド中の場合はユニットを検索する必要がある
        If .Name = "ステータス表示用ダミーユニット" Then
            'メインパイロットかどうかチェック
            vname = "搭乗順番[" & ID & "]"
            If IsLocalVariableDefined(vname) Then
                If LocalVariableList.Item(vname).NumericValue <> 1 Then
                    Exit Property
                End If
            Else
                Exit Property
            End If
            
            vname = "搭乗ユニット[" & ID & "]"
            If IsLocalVariableDefined(vname) Then
                uname = LocalVariableList.Item(vname).StringValue
            End If
            If uname = "" Then
                Exit Property
            End If
            
            Set u = UList.Item(uname)
        End If
        
        With u
            If .IsFeatureAvailable("パイロット読み仮名") Then
                KanaName = .FeatureData("パイロット読み仮名")
                idx = InStr(KanaName, "$(読み仮名)")
                If idx > 0 Then
                    KanaName = Left$(KanaName, idx - 1) _
                        & Data.KanaName & Mid$(KanaName, idx + 5)
                End If
            ElseIf .IsFeatureAvailable("パイロット愛称") Then
                KanaName = .FeatureData("パイロット愛称")
                idx = InStr(KanaName, "$(愛称)")
                If idx > 0 Then
                    KanaName = Left$(KanaName, idx - 1) _
                        & Data.Nickname & Mid$(KanaName, idx + 5)
                End If
                KanaName = StrToHiragana(KanaName)
            End If
        End With
    End With
    
    '読み仮名内の式置換のため、デフォルトユニットを一時的に変更する
    Set u = SelectedUnitForEvent
    Set SelectedUnitForEvent = Unit
    ReplaceSubExpression KanaName
    Set SelectedUnitForEvent = u
End Property

'性別
Public Property Get Sex() As String
    Sex = Data.Sex
    If Not Unit Is Nothing Then
        With Unit
            If .IsFeatureAvailable("性別") Then
                Sex = .FeatureData("性別")
            End If
        End With
    End If
End Property

'搭乗するユニットのクラス
Public Property Get Class() As String
    Class = Data.Class
End Property

'倒したときに得られる経験値
Public Property Get ExpValue() As Integer
    ExpValue = Data.ExpValue
End Property

'性格
Public Property Get Personality() As String
    Personality = Data.Personality
    
    'ユニットに乗っている？
    If Unit Is Nothing Then
        Exit Property
    End If
    
    With Unit
        'アイテム用特殊能力「性格変更」
        If .IsFeatureAvailable("性格変更") Then
            Personality = .FeatureData("性格変更")
            Exit Property
        End If
        
        '追加パイロットの性格を優先させる
        If Not IsAdditionalPilot Then
            If .CountPilot > 0 Then
                If .Pilot(1) Is Me Then
                    Personality = .MainPilot.Data.Personality
                End If
            End If
        End If
    End With
End Property

'ビットマップ
Public Property Get Bitmap(Optional ByVal use_orig As Boolean) As String
Dim u As Unit, uname As String, vname As String

    If use_orig Then
        Bitmap = Data.Bitmap0
    Else
        Bitmap = Data.Bitmap
    End If
    
    'パイロット画像変更
    If Unit Is Nothing Then
        Exit Property
    End If
    With Unit
        If .CountPilot > 0 Then
            If Not .MainPilot Is Me Then
                Exit Property
            End If
        End If
        
        Set u = Unit
        
        'パイロットステータスコマンド中の場合はユニットを検索する必要がある
        If .Name = "ステータス表示用ダミーユニット" Then
            'メインパイロットかどうかチェック
            vname = "搭乗順番[" & ID & "]"
            If IsLocalVariableDefined(vname) Then
                If LocalVariableList.Item(vname).NumericValue <> 1 Then
                    Exit Property
                End If
            Else
                Exit Property
            End If
            
            vname = "搭乗ユニット[" & ID & "]"
            If IsLocalVariableDefined(vname) Then
                uname = LocalVariableList.Item(vname).StringValue
            End If
            If uname = "" Then
                Exit Property
            End If
            
            Set u = UList.Item(uname)
        End If
        
        With u
            If .IsConditionSatisfied("パイロット画像") Then
                Bitmap = LIndex(.ConditionData("パイロット画像"), 2)
            End If
            If .IsFeatureAvailable("パイロット画像") Then
                Bitmap = .FeatureData("パイロット画像")
            End If
        End With
    End With
End Property

'ＢＧＭ
Public Property Get BGM() As String
    BGM = Data.BGM
End Property

'メッセージタイプ
Public Property Get MessageType() As String
    MessageType = Name
    
    'パイロット能力「メッセージ」
    If IsSkillAvailable("メッセージ") Then
        MessageType = SkillData("メッセージ")
    End If
    
    '能力コピーで変身した場合はメッセージもコピー元パイロットのものを使う
    If Not Unit Is Nothing Then
        With Unit
            If .IsConditionSatisfied("メッセージ") Then
                MessageType = LIndex(.ConditionData("メッセージ"), 2)
            End If
        End With
    End If
End Property


'防御力
Public Property Get Defense() As Integer
    If IsOptionDefined("防御力成長") _
        Or IsOptionDefined("防御力レベルアップ") _
    Then
        Defense = 100 + 5 * SkillLevel("耐久")
        If IsOptionDefined("防御力低成長") Then
            Defense = Defense + (Level * (1 + 2 * SkillLevel("防御成長"))) \ 2
        Else
            Defense = Defense + Int(Level * (1 + SkillLevel("防御成長")))
        End If
    Else
        Defense = 100 + 5 * SkillLevel("耐久")
    End If
End Property


'能力値を更新
Public Sub Update()
Dim skill_num As Integer
Dim skill_name(64) As String
Dim skill_data(64) As SkillData
Dim i As Integer, j As Integer, lv As Double
Dim sd As SkillData

    '現在のレベルで使用可能な特殊能力の一覧を作成
    
    '以前の一覧を削除
    With colSkill
        For i = 1 To .Count
            .Remove 1
        Next
    End With
    
    'パイロットデータを参照しながら使用可能な特殊能力を検索
    skill_num = 0
    For Each sd In Data.colSkill
        With sd
            If Level >= .NecessaryLevel Then
                '既に登録済み？
                If .Name = "ＳＰ消費減少" _
                    Or .Name = "スペシャルパワー自動発動" _
                    Or .Name = "ハンター" _
                Then
                    'これらの特殊能力は同種の能力を複数持つことが出来る
                    For i = 1 To skill_num
                        If .Name = skill_name(i) Then
                           If .StrData = skill_data(i).StrData Then
                               'ただしデータ指定まで同一であれば同じ能力と見なす
                               Exit For
                           End If
                        End If
                    Next
                Else
                    For i = 1 To skill_num
                        If .Name = skill_name(i) Then
                           Exit For
                        End If
                    Next
                End If
                
                If i > skill_num Then
                    '未登録
                    skill_num = skill_num + 1
                    skill_name(skill_num) = .Name
                    Set skill_data(skill_num) = sd
                ElseIf .NecessaryLevel > skill_data(i).NecessaryLevel Then
                    '登録済みである場合は習得レベルが高いものを優先
                    Set skill_data(i) = sd
                End If
            End If
        End With
    Next
    
    'SetSkillコマンドで付加された特殊能力を検索
    If IsGlobalVariableDefined("Ability(" & ID & ")") Then
        Dim alist As String, sname As String, sdata As String
        Dim buf As String
        
        alist = GlobalVariableList.Item("Ability(" & ID & ")").StringValue
        For i = 1 To LLength(alist)
            sname = LIndex(alist, i)
            buf = GlobalVariableList.Item("Ability(" & ID & "," & sname & ")").StringValue
            sdata = ListTail(buf, 2)
            
            '既に登録済み？
            If sname = "ＳＰ消費減少" _
                Or sname = "スペシャルパワー自動発動" _
                Or sname = "ハンター" _
            Then
                'これらの特殊能力は同種の能力を複数持つことが出来る
                For j = 1 To skill_num
                    If sname = skill_name(j) Then
                        If sdata = skill_data(j).StrData Then
                            'ただしデータ指定まで同一であれば同じ能力と見なす
                            Exit For
                        End If
                    End If
                Next
            Else
                For j = 1 To skill_num
                    If sname = skill_name(j) Then
                        Exit For
                    End If
                Next
            End If
            
            If j > skill_num Then
                '未登録
                skill_num = j
                skill_name(j) = sname
            End If
            
            If StrToDbl(LIndex(buf, 1)) = 0 Then
                'レベル0の場合は能力を封印
                Set skill_data(j) = Nothing
            Else
                'PDListのデータを書き換えるわけにはいかないので
                'アビリティデータを新規に作成
                Set sd = New SkillData
                With sd
                    .Name = sname
                    .Level = StrToDbl(LIndex(buf, 1))
                    If .Level = -1 Then
                        .Level = DEFAULT_LEVEL
                    End If
                    .StrData = ListTail(buf, 2)
                End With
                Set skill_data(j) = sd
            End If
        Next
    End If
    
    '属性使用不能状態の際、対応する技能を封印する。
    If Not Unit Is Nothing Then
        For j = 1 To skill_num
            If Not skill_data(j) Is Nothing Then
                If Unit.ConditionLifetime(skill_data(j).Name & "使用不能") > 0 Then
                    Set skill_data(j) = Nothing
                End If
            End If
        Next
    End If
    
    '使用可能な特殊能力を登録
    With colSkill
        For i = 1 To skill_num
            If Not skill_data(i) Is Nothing Then
                Select Case skill_name(i)
                    Case "ＳＰ消費減少", "スペシャルパワー自動発動", "ハンター"
                        For j = 1 To i - 1
                            If skill_name(i) = skill_name(j) Then
                                Exit For
                            End If
                        Next
                        If j >= i Then
                            .Add skill_data(i), skill_name(i)
                        Else
                            .Add skill_data(i), skill_name(i) & ":" & Format$(i)
                        End If
                    Case Else
                        .Add skill_data(i), skill_name(i)
                End Select
            End If
        Next
    End With
    
    'これから下は能力値の計算
    
    '基本値
    With Data
        InfightBase = .Infight
        ShootingBase = .Shooting
        HitBase = .Hit
        DodgeBase = .Dodge
        TechniqueBase = .Technique
        IntuitionBase = .Intuition
        Adaption = .Adaption
    End With
    
    'レベルによる追加分
    lv = Level + SkillLevel("追加レベル")
    If IsOptionDefined("攻撃力低成長") Then
        InfightBase = InfightBase + (lv * (1 + 2 * SkillLevel("格闘成長"))) \ 2
        ShootingBase = ShootingBase + (lv * (1 + 2 * SkillLevel("射撃成長"))) \ 2
    Else
        InfightBase = InfightBase + Int(lv * (1 + SkillLevel("格闘成長")))
        ShootingBase = ShootingBase + Int(lv * (1 + SkillLevel("射撃成長")))
    End If
    HitBase = HitBase + Int(lv * (2 + SkillLevel("命中成長")))
    DodgeBase = DodgeBase + Int(lv * (2 + SkillLevel("回避成長")))
    TechniqueBase = TechniqueBase + Int(lv * (1 + SkillLevel("技量成長")))
    IntuitionBase = IntuitionBase + Int(lv * (1 + SkillLevel("反応成長")))
    
    '能力ＵＰ
    InfightBase = InfightBase + SkillLevel("格闘ＵＰ")
    ShootingBase = ShootingBase + SkillLevel("射撃ＵＰ")
    HitBase = HitBase + SkillLevel("命中ＵＰ")
    DodgeBase = DodgeBase + SkillLevel("回避ＵＰ")
    TechniqueBase = TechniqueBase + SkillLevel("技量ＵＰ")
    IntuitionBase = IntuitionBase + SkillLevel("反応ＵＰ")
    
    '能力ＤＯＷＮ
    InfightBase = InfightBase - SkillLevel("格闘ＤＯＷＮ")
    ShootingBase = ShootingBase - SkillLevel("射撃ＤＯＷＮ")
    HitBase = HitBase - SkillLevel("命中ＤＯＷＮ")
    DodgeBase = DodgeBase - SkillLevel("回避ＤＯＷＮ")
    TechniqueBase = TechniqueBase - SkillLevel("技量ＤＯＷＮ")
    IntuitionBase = IntuitionBase - SkillLevel("反応ＤＯＷＮ")
    
    '上限を超えないように
    InfightBase = MinLng(InfightBase, 9999)
    ShootingBase = MinLng(ShootingBase, 9999)
    HitBase = MinLng(HitBase, 9999)
    DodgeBase = MinLng(DodgeBase, 9999)
    TechniqueBase = MinLng(TechniqueBase, 9999)
    IntuitionBase = MinLng(IntuitionBase, 9999)
    
    'これから下は特殊能力による修正値の計算
    
    'まずは修正値を初期化
    InfightMod = 0
    ShootingMod = 0
    HitMod = 0
    DodgeMod = 0
    TechniqueMod = 0
    IntuitionMod = 0
    
    'パイロット用特殊能力による修正
    
    lv = SkillLevel("超感覚")
    If lv > 0 Then
        HitMod = HitMod + 2 * lv + 3
        DodgeMod = DodgeMod + 2 * lv + 3
    End If
    
    lv = SkillLevel("知覚強化")
    If lv > 0 Then
        HitMod = HitMod + 2 * lv + 3
        DodgeMod = DodgeMod + 2 * lv + 3
    End If
    
    lv = SkillLevel("念力")
    If lv > 0 Then
        HitMod = HitMod + 2 * lv + 3
        DodgeMod = DodgeMod + 2 * lv + 3
    End If
    
    lv = SkillLevel("超反応")
    HitMod = HitMod + 2 * lv
    DodgeMod = DodgeMod + 2 * lv
    
    If IsSkillAvailable("サイボーグ") Then
        HitMod = HitMod + 5
        DodgeMod = DodgeMod + 5
    End If
    If IsSkillAvailable("悟り") Then
        HitMod = HitMod + 10
        DodgeMod = DodgeMod + 10
    End If
    If IsSkillAvailable("超能力") Then
        HitMod = HitMod + 5
        DodgeMod = DodgeMod + 5
    End If
    
    'これから下はユニットによる修正値の計算
    
    'ユニットに乗っていない？
    If Unit Is Nothing Then
        GoTo SkipUnitMod
    End If
    
    With Unit
        'クイックセーブ処理などで実際には乗っていない場合
        If .CountPilot = 0 Then
            Exit Sub
        End If
        
        'サブパイロット＆サポートパイロットによるサポート
        If Me Is .MainPilot And .Status = "出撃" Then
            For i = 2 To .CountPilot
                With .Pilot(i)
                    InfightMod = InfightMod + 2 * .SkillLevel("格闘サポート")
                    If HasMana() Then
                        ShootingMod = ShootingMod + 2 * .SkillLevel("魔力サポート")
                    Else
                        ShootingMod = ShootingMod + 2 * .SkillLevel("射撃サポート")
                    End If
                    HitMod = HitMod + 3 * .SkillLevel("サポート")
                    HitMod = HitMod + 2 * .SkillLevel("命中サポート")
                    DodgeMod = DodgeMod + 3 * .SkillLevel("サポート")
                    DodgeMod = DodgeMod + 2 * .SkillLevel("回避サポート")
                    TechniqueMod = TechniqueMod + 2 * .SkillLevel("技量サポート")
                    IntuitionMod = IntuitionMod + 2 * .SkillLevel("反応サポート")
                End With
            Next
            For i = 1 To .CountSupport
                With .Support(i)
                    InfightMod = InfightMod + 2 * .SkillLevel("格闘サポート")
                    If HasMana() Then
                        ShootingMod = ShootingMod + 2 * .SkillLevel("魔力サポート")
                    Else
                        ShootingMod = ShootingMod + 2 * .SkillLevel("射撃サポート")
                    End If
                    HitMod = HitMod + 3 * .SkillLevel("サポート")
                    HitMod = HitMod + 2 * .SkillLevel("命中サポート")
                    DodgeMod = DodgeMod + 3 * .SkillLevel("サポート")
                    DodgeMod = DodgeMod + 2 * .SkillLevel("回避サポート")
                    TechniqueMod = TechniqueMod + 2 * .SkillLevel("技量サポート")
                    IntuitionMod = IntuitionMod + 2 * .SkillLevel("反応サポート")
                End With
            Next
            If .IsFeatureAvailable("追加サポート") Then
                With .AdditionalSupport
                    InfightMod = InfightMod + 2 * .SkillLevel("格闘サポート")
                    If HasMana() Then
                        ShootingMod = ShootingMod + 2 * .SkillLevel("魔力サポート")
                    Else
                        ShootingMod = ShootingMod + 2 * .SkillLevel("射撃サポート")
                    End If
                    HitMod = HitMod + 3 * .SkillLevel("サポート")
                    HitMod = HitMod + 2 * .SkillLevel("命中サポート")
                    DodgeMod = DodgeMod + 3 * .SkillLevel("サポート")
                    DodgeMod = DodgeMod + 2 * .SkillLevel("回避サポート")
                    TechniqueMod = TechniqueMod + 2 * .SkillLevel("技量サポート")
                    IntuitionMod = IntuitionMod + 2 * .SkillLevel("反応サポート")
                End With
            End If
        End If
        
        'ユニット＆アイテムによる強化
        For i = 1 To .CountFeature
            Select Case .Feature(i)
                Case "格闘強化"
                    If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
                        InfightMod = InfightMod + 5 * .FeatureLevel(i)
                    End If
                Case "射撃強化"
                    If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
                        ShootingMod = ShootingMod + 5 * .FeatureLevel(i)
                    End If
                Case "命中強化"
                    If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
                        HitMod = HitMod + 5 * .FeatureLevel(i)
                    End If
                Case "回避強化"
                    If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
                        DodgeMod = DodgeMod + 5 * .FeatureLevel(i)
                    End If
                Case "技量強化"
                    If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
                        TechniqueMod = TechniqueMod + 5 * .FeatureLevel(i)
                    End If
                Case "反応強化"
                    If Morale >= StrToLng(LIndex(.FeatureData(i), 2)) Then
                        IntuitionMod = IntuitionMod + 5 * .FeatureLevel(i)
                    End If
            End Select
        Next
        
        '地形適応変更
        If .IsFeatureAvailable("パイロット地形適応変更") Then
            Dim padaption(4) As Integer
            
            For i = 1 To 4
                Select Case Mid$(Adaption, i, 1)
                    Case "S"
                        padaption(i) = 5
                    Case "A"
                        padaption(i) = 4
                    Case "B"
                        padaption(i) = 3
                    Case "C"
                        padaption(i) = 2
                    Case "D"
                        padaption(i) = 1
                    Case "E", "-"
                        padaption(i) = 0
                End Select
            Next
            
            '地形適応変更能力による修正
            For i = 1 To .CountFeature
                If .Feature(i) = "パイロット地形適応変更" Then
                    For j = 1 To 4
                        If StrToLng(LIndex(.FeatureData(i), j)) >= 0 Then
                            '修正値がプラスのとき
                            If padaption(j) < 4 Then
                                padaption(j) = padaption(j) + StrToLng(LIndex(.FeatureData(i), j))
                                '地形適応はAより高くはならない
                                If padaption(j) > 4 Then
                                    padaption(j) = 4
                                End If
                            End If
                        Else
                            '修正値がマイナスのときは本来の地形適応が"A"以上でも処理を行なう
                            padaption(j) = padaption(j) + StrToLng(LIndex(.FeatureData(i), j))
                        End If
                    Next
                End If
            Next
            
            Adaption = ""
            For i = 1 To 4
                Select Case padaption(i)
                    Case Is >= 5
                        Adaption = Adaption & "S"
                    Case 4
                        Adaption = Adaption & "A"
                    Case 3
                        Adaption = Adaption & "B"
                    Case 2
                        Adaption = Adaption & "C"
                    Case 1
                        Adaption = Adaption & "D"
                    Case Is <= 0
                        Adaption = Adaption & "-"
                End Select
            Next
        End If
    End With
    
    '気力の値を気力上限・気力下限の範囲にする
    SetMorale Morale

SkipUnitMod:
    
    '基本値と修正値の合計から実際の能力値を算出
    Infight = MinLng(InfightBase + InfightMod + InfightMod2, 9999)
    Shooting = MinLng(ShootingBase + ShootingMod + ShootingMod2, 9999)
    Hit = MinLng(HitBase + HitMod + HitMod2, 9999)
    Dodge = MinLng(DodgeBase + DodgeMod + DodgeMod2, 9999)
    Technique = MinLng(TechniqueBase + TechniqueMod + TechniqueMod2, 9999)
    Intuition = MinLng(IntuitionBase + IntuitionMod + IntuitionMod2, 9999)
End Sub

'周りのユニットによる支援効果を更新
Public Sub UpdateSupportMod()
Dim u As Unit, my_unit As Unit, my_party As String
Dim my_cmd_rank As Integer
Dim cmd_rank As Integer, cmd_rank2 As Integer, cmd_level As Double
Dim cs_level As Double
Dim range As Integer, max_range As Integer
Dim mod_stack As Boolean, rel_lv As Integer
Dim team As String, uteam As String
Dim i As Integer, j As Integer, k As Integer

    '支援効果による修正値を初期化
    
    Infight = InfightBase + InfightMod
    Shooting = ShootingBase + ShootingMod
    Hit = HitBase + HitMod
    Dodge = DodgeBase + DodgeMod
    Technique = TechniqueBase + TechniqueMod
    Intuition = IntuitionBase + IntuitionMod
    
    InfightMod2 = 0
    ShootingMod2 = 0
    HitMod2 = 0
    DodgeMod2 = 0
    TechniqueMod2 = 0
    IntuitionMod2 = 0
    
    MoraleMod = 0
    
    'ステータス表示時には支援効果を無視
    If MapFileName = "" Then
        Exit Sub
    End If
    
    'ユニットに乗っていなければここで終了
    If Unit Is Nothing Then
        Exit Sub
    End If
    
    '一旦乗っているユニットを記録しておく
    Set my_unit = Unit
    
    With Unit
        'ユニットが出撃していなければここで終了
        If .Status <> "出撃" Then
            Exit Sub
        End If
        If Not Unit Is MapDataForUnit(.X, .Y) Then
            Exit Sub
        End If
        
        'メインパイロットでなければここで終了
        If .CountPilot = 0 Then
            Exit Sub
        End If
        If Not Me Is .MainPilot Then
            Exit Sub
        End If
        
        '正常な判断が出来ないユニットは支援を受けられない
        If .IsConditionSatisfied("暴走") Then
            Exit Sub
        End If
        If .IsConditionSatisfied("混乱") Then
            Exit Sub
        End If
        
        '支援を受けられるかどうかの判定用に陣営を参照しておく
        my_party = .Party
        
        '指揮効果判定用に自分の階級レベルを算出
        If IsSkillAvailable("階級") Then
            my_cmd_rank = SkillLevel("階級")
            cmd_rank = my_cmd_rank
        Else
            If InStr(Name, "(ザコ)") = 0 And InStr(Name, "(汎用)") = 0 Then
                my_cmd_rank = DEFAULT_LEVEL
            Else
                my_cmd_rank = 0
            End If
            cmd_rank = 0
        End If
        
        '自分が所属しているチーム名
        team = SkillData("チーム")
        
        '周りのユニットを調べる
        cs_level = DEFAULT_LEVEL
        max_range = 5
        For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
            For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
                'ユニット間の距離が範囲内？
                range = Abs(.X - i) + Abs(.Y - j)
                If range > max_range Then
                    GoTo NextUnit
                End If
                
                Set u = MapDataForUnit(i, j)
                
                If u Is Nothing Then
                    GoTo NextUnit
                End If
                If u Is Unit Then
                    GoTo NextUnit
                End If
                
                With u
                    'ユニットにパイロットが乗っていなければ無視
                    If .CountPilot = 0 Then
                        GoTo NextUnit
                    End If
                    
                    '陣営が一致していないと支援は受けられない
                    Select Case my_party
                        Case "味方", "ＮＰＣ"
                            Select Case .Party
                                Case "敵", "中立"
                                    GoTo NextUnit
                            End Select
                        Case Else
                            If my_party <> .Party Then
                                GoTo NextUnit
                            End If
                    End Select
                    
                    '相手が正常な判断能力を持っていない場合も支援は受けられない
                    If .IsConditionSatisfied("暴走") Then
                        GoTo NextUnit
                    End If
                    If .IsConditionSatisfied("混乱") Then
                        GoTo NextUnit
                    End If
                End With
                
                With u.MainPilot(True)
                    '同じチームに所属している？
                    uteam = .SkillData("チーム")
                    If team <> uteam And uteam <> "" Then
                        GoTo NextUnit
                    End If
                    
                    '広域サポート
                    If range <= 2 Then
                        cs_level = MaxDbl(cs_level, .SkillLevel("広域サポート"))
                    End If
                    
                    '指揮効果
                    If my_cmd_rank >= 0 Then
                        If range > .CommandRange Then
                            GoTo NextUnit
                        End If
                        
                        cmd_rank2 = .SkillLevel("階級")
                        If cmd_rank2 > cmd_rank Then
                            cmd_rank = cmd_rank2
                            cmd_level = .SkillLevel("指揮")
                        ElseIf cmd_rank2 = cmd_rank Then
                            cmd_level = MaxDbl(cmd_level, .SkillLevel("指揮"))
                        End If
                    End If
                End With
NextUnit:
            Next
        Next
        
        '追加パイロットの場合は乗っているユニットが変化してしまうことがあるので
        '変化してしまった場合は元に戻しておく
        If Not my_unit Is Unit Then
            my_unit.MainPilot
        End If
        
        '広域サポートによる修正
        If cs_level <> DEFAULT_LEVEL Then
            HitMod2 = HitMod2 + 5 * cs_level
            DodgeMod2 = DodgeMod2 + 5 * cs_level
        End If
        
        '指揮能力による修正
        Select Case my_cmd_rank
            Case DEFAULT_LEVEL
                '修正なし
            Case 0
                HitMod2 = HitMod2 + 5 * cmd_level
                DodgeMod2 = DodgeMod2 + 5 * cmd_level
            Case Else
                '自分が階級レベルを持っている場合はより高い階級レベルを
                '持つパイロットの指揮効果のみを受ける
                If cmd_rank > my_cmd_rank Then
                    HitMod2 = HitMod2 + 5 * cmd_level
                    DodgeMod2 = DodgeMod2 + 5 * cmd_level
                End If
        End Select
        
        '支援効果による修正を能力値に加算
        Infight = Infight + InfightMod2
        Shooting = Shooting + ShootingMod2
        Hit = Hit + HitMod2
        Dodge = Dodge + DodgeMod2
        Technique = Technique + TechniqueMod2
        Intuition = Intuition + IntuitionMod2
        
        '信頼補正
        If Not IsOptionDefined("信頼補正") Then
            Exit Sub
        End If
        If InStr(Name, "(ザコ)") > 0 Then
            Exit Sub
        End If
        
        '信頼補正が重複する？
        mod_stack = IsOptionDefined("信頼補正重複")
        
        '同じユニットに乗っているサポートパイロットからの補正
        If mod_stack Then
            For i = 1 To .CountSupport
                rel_lv = rel_lv + Relation(.Support(i))
            Next
            If .IsFeatureAvailable("追加サポート") Then
                rel_lv = rel_lv + Relation(.AdditionalSupport)
            End If
        Else
            For i = 1 To .CountSupport
                rel_lv = MaxLng(Relation(.Support(i)), rel_lv)
            Next
            If .IsFeatureAvailable("追加サポート") Then
                rel_lv = MaxLng(Relation(.AdditionalSupport), rel_lv)
            End If
        End If
        
        '周囲のユニットからの補正
        If IsOptionDefined("信頼補正範囲拡大") Then
            max_range = 2
        Else
            max_range = 1
        End If
        For i = MaxLng(.X - max_range, 1) To MinLng(.X + max_range, MapWidth)
            For j = MaxLng(.Y - max_range, 1) To MinLng(.Y + max_range, MapHeight)
                'ユニット間の距離が範囲内？
                range = Abs(.X - i) + Abs(.Y - j)
                If range > max_range Then
                    GoTo NextUnit2
                End If
                
                Set u = MapDataForUnit(i, j)
                
                If u Is Nothing Then
                    GoTo NextUnit2
                End If
                If u Is Unit Then
                    GoTo NextUnit2
                End If
                
                With u
                    'ユニットにパイロットが乗っていなければ無視
                    If .CountPilot = 0 Then
                        GoTo NextUnit2
                    End If
                    
                    '味方かどうか判定
                    Select Case my_party
                        Case "味方", "ＮＰＣ"
                            Select Case .Party
                                Case "敵", "中立"
                                    GoTo NextUnit2
                            End Select
                        Case Else
                            If my_party <> .Party Then
                                GoTo NextUnit2
                            End If
                    End Select
                    
                    If mod_stack Then
                        rel_lv = rel_lv + Relation(.MainPilot(True))
                        For k = 2 To .CountPilot
                            rel_lv = rel_lv + Relation(.Pilot(k))
                        Next
                        For k = 1 To .CountSupport
                            rel_lv = rel_lv + Relation(.Support(k))
                        Next
                        If .IsFeatureAvailable("追加サポート") Then
                            rel_lv = rel_lv + Relation(.AdditionalSupport)
                        End If
                    Else
                        rel_lv = MaxLng(Relation(.MainPilot(True)), rel_lv)
                        For k = 2 To .CountPilot
                            rel_lv = MaxLng(Relation(.Pilot(k)), rel_lv)
                        Next
                        For k = 1 To .CountSupport
                            rel_lv = MaxLng(Relation(.Support(k)), rel_lv)
                        Next
                        If .IsFeatureAvailable("追加サポート") Then
                            rel_lv = MaxLng(Relation(.AdditionalSupport), rel_lv)
                        End If
                    End If
               End With
NextUnit2:
            Next
        Next
        
        '追加パイロットの場合は乗っているユニットが変化してしまうことがあるので
        '変化してしまった場合は元に戻しておく
        If Not my_unit Is Unit Then
            my_unit.MainPilot
        End If
        
        '信頼補正を設定
        Select Case rel_lv
            Case 1
                MoraleMod = MoraleMod + 5
            Case 2
                MoraleMod = MoraleMod + 8
            Case Is > 2
                MoraleMod = MoraleMod + 2 * rel_lv + 4
        End Select
    End With
End Sub


' === レベル＆経験値関連処理 ===

'レベル
Public Property Get Level() As Integer
    Level = proLevel
End Property

Public Property Let Level(ByVal lv As Integer)
    If proLevel = lv Then
        '変化なし
        Exit Property
    End If
    
    proLevel = lv
    Update
End Property

'経験値
Public Property Get Exp() As Long
    Exp = proEXP
End Property

Public Property Let Exp(ByVal new_exp As Long)
Dim prev_level As Integer

    prev_level = proLevel
    
    '500ごとにレベルアップ
    proEXP = new_exp Mod 500
    proLevel = proLevel + new_exp \ 500
    
    '経験値が下がる場合はレベルを下げる
    If proEXP < 0 Then
        If proLevel > 1 Then
            proEXP = proEXP + 500
            proLevel = proLevel - 1
        Else
            'これ以上はレベルを下げられないので
            proEXP = 0
        End If
    End If
    
    'レベル上限チェック
    If new_exp \ 500 > 0 Then
        If IsOptionDefined("レベル限界突破") Then
            If proLevel > 999 Then 'レベル999で打ち止め
                proLevel = 999
                proEXP = 500
            End If
        Else
            If proLevel > 99 Then 'レベル99で打ち止め
                proLevel = 99
                proEXP = 500
            End If
        End If
    End If
    
    If prev_level <> proLevel Then
        Update
    End If
End Property


'気力
Public Property Get Morale() As Integer
    Morale = proMorale
End Property

Public Property Let Morale(ByVal new_morale As Integer)
    SetMorale new_morale
End Property

Private Sub SetMorale(ByVal new_morale As Integer)
Dim maxm As Integer
Dim minm As Integer
    
    maxm = MaxMorale
    minm = MinMorale
    
    If new_morale > maxm Then
        proMorale = maxm
    ElseIf new_morale < minm Then
        proMorale = minm
    Else
        proMorale = new_morale
    End If
End Sub

Public Property Get MaxMorale() As Integer
    MaxMorale = 150
    If IsSkillAvailable("気力上限") Then
        If IsSkillLevelSpecified("気力上限") Then
            MaxMorale = MaxLng(SkillLevel("気力上限"), 0)
        End If
    End If
End Property

Public Property Get MinMorale() As Integer
    MinMorale = 50
    If IsSkillAvailable("気力下限") Then
        If IsSkillLevelSpecified("気力下限") Then
            MinMorale = MaxLng(SkillLevel("気力下限"), 0)
        End If
    End If
End Property


' === ＳＰ値関連処理 ===

'最大ＳＰ
Public Property Get MaxSP() As Long
Dim lv As Integer

    'ＳＰなしの場合はレベルに関わらず0
    If Data.SP <= 0 Then
        MaxSP = 0
        'ただし追加パイロットの場合は第１パイロットの最大ＳＰを使用
        If Not Unit Is Nothing Then
            With Unit
                If .CountPilot > 0 Then
                    If Not .Pilot(1) Is Me Then
                        If .MainPilot Is Me Then
                            MaxSP = .Pilot(1).MaxSP
                        End If
                    End If
                End If
            End With
        End If
        Exit Property
    End If
    
    'レベルによる上昇値
    lv = Level
    If lv > 99 Then
        lv = 100
    End If
    lv = lv + SkillLevel("追加レベル")
    If lv > 40 Then
        MaxSP = lv + 40
    Else
        MaxSP = 2 * lv
    End If
    If IsSkillAvailable("ＳＰ低成長") Then
        MaxSP = MaxSP \ 2
    ElseIf IsSkillAvailable("ＳＰ高成長") Then
        MaxSP = 1.5 * MaxSP
    End If
    If IsOptionDefined("ＳＰ低成長") Then
        MaxSP = MaxSP \ 2
    End If
    
    '基本値を追加
    MaxSP = MaxSP + Data.SP
    
    '能力ＵＰ
    MaxSP = MaxSP + SkillLevel("ＳＰＵＰ")
    
    '能力ＤＯＷＮ
    MaxSP = MaxSP - SkillLevel("ＳＰＤＯＷＮ")
    
    '上限を超えないように
    If MaxSP > 9999 Then
        MaxSP = 9999
    End If
End Property

'ＳＰ値
Public Property Get SP() As Long
    SP = proSP
    
    '追加パイロットかどうか判定
    
    If Unit Is Nothing Then
        Exit Property
    End If
    
    With Unit
        If .CountPilot = 0 Then
            Exit Property
        End If
        
        If .Pilot(1) Is Me Then
            Exit Property
        End If
        If Not .MainPilot Is Me Then
            Exit Property
        End If
        
        '追加パイロットだったので第１パイロットのＳＰ値を代わりに使う
        If Data.SP > 0 Then
            'ＳＰを持つ場合は消費量を一致させる
            With .Pilot(1)
                If .MaxSP > 0 Then
                    proSP = MaxSP * .SP0 \ .MaxSP
                    SP = proSP
                End If
            End With
        Else
            'ＳＰを持たない場合はそのまま使う
            SP = .Pilot(1).SP0
        End If
    End With
End Property

Public Property Let SP(ByVal new_sp As Long)
Dim prev_sp As Long

    prev_sp = proSP
    If new_sp > MaxSP Then
        proSP = MaxSP
    ElseIf new_sp < 0 Then
        proSP = 0
    Else
        proSP = new_sp
    End If
    
    '追加パイロットかどうか判定
    
    If Unit Is Nothing Then
        Exit Property
    End If
    
    With Unit
        If .CountPilot = 0 Then
            Exit Property
        End If
        
        If .Pilot(1) Is Me Then
            Exit Property
        End If
        If Not .MainPilot Is Me Then
            Exit Property
        End If
        
        '追加パイロットだったので第１パイロットのＳＰ値を代わりに使う
        With .Pilot(1)
            If Data.SP > 0 Then
                '追加パイロットがＳＰを持つ場合は第１パイロットと消費率を一致させる
                If .MaxSP > 0 Then
                    .SP0 = .MaxSP * proSP \ MaxSP
                    proSP = MaxSP * .SP0 \ .MaxSP
                End If
            Else
                '追加パイロットがＳＰを持たない場合は第１パイロットのＳＰ値をそのまま使う
                If new_sp > .MaxSP Then
                    .SP0 = .MaxSP
                ElseIf new_sp < 0 Then
                    .SP0 = 0
                Else
                    .SP0 = new_sp
                End If
            End If
        End With
    End With
End Property

Public Property Get SP0() As Long
    SP0 = proSP
End Property

Public Property Let SP0(ByVal new_sp As Long)
    proSP = new_sp
End Property


' === 霊力関連処理 ===

'霊力最大値
Public Function MaxPlana() As Long
Dim lv As Integer

    If Not IsSkillAvailable("霊力") Then
        '霊力能力を持たない場合
        MaxPlana = 0
        
        '追加パイロットの場合は第１パイロットの霊力を代わりに使う
        If Not Unit Is Nothing Then
            With Unit
                If .CountPilot > 0 Then
                    If Not .Pilot(1) Is Me Then
                        If .MainPilot Is Me Then
                            MaxPlana = .Pilot(1).MaxPlana
                        End If
                    End If
                End If
            End With
        End If
        
        Exit Function
    End If
    
    '霊力基本値
    MaxPlana = SkillLevel("霊力")
    
    'レベルによる増加分
    lv = MinLng(Level, 100)
    If IsSkillAvailable("霊力成長") Then
        MaxPlana = MaxPlana + 1.5 * lv * (10 + SkillLevel("霊力成長")) \ 10
    Else
        MaxPlana = MaxPlana + 1.5 * lv
    End If
End Function

'霊力
Public Property Get Plana() As Long
    If IsSkillAvailable("霊力") Then
        Plana = proPlana
    End If
    
    '追加パイロットかどうか判定
    
    If Unit Is Nothing Then
        Exit Property
    End If
    
    With Unit
        If .CountPilot = 0 Then
            Exit Property
        End If
        
        If .Pilot(1) Is Me Then
            Exit Property
        End If
        If Not .MainPilot Is Me Then
            Exit Property
        End If
        
        '追加パイロットだったので第１パイロットの霊力を代わりに使う
        If IsSkillAvailable("霊力") Then
            With .Pilot(1)
                If .MaxPlana > 0 Then
                    proPlana = MaxPlana * .Plana0 \ .MaxPlana
                    Plana = proPlana
                End If
            End With
        Else
            Plana = .Pilot(1).Plana0
        End If
    End With
End Property

Public Property Let Plana(ByVal new_plana As Long)
Dim prev_plana As Long

    prev_plana = proPlana
    If new_plana > MaxPlana Then
        proPlana = MaxPlana
    ElseIf new_plana < 0 Then
        proPlana = 0
    Else
        proPlana = new_plana
    End If
    
    '追加パイロットかどうか判定
    
    If Unit Is Nothing Then
        Exit Property
    End If
    
    With Unit
        If .CountPilot = 0 Then
            Exit Property
        End If
        
        If .Pilot(1) Is Me Then
            Exit Property
        End If
        If Not .MainPilot Is Me Then
            Exit Property
        End If
        
        '追加パイロットだったので第１パイロットの霊力値を代わりに使う
        With .Pilot(1)
            If IsSkillAvailable("霊力") Then
                '追加パイロットが霊力を持つ場合は第１パイロットと消費率を一致させる
                If .MaxSP > 0 Then
                    .Plana0 = .MaxPlana * proPlana \ MaxPlana
                    proPlana = MaxPlana * .Plana0 \ .MaxPlana
                End If
            Else
                '追加パイロットが霊力を持たない場合は第１パイロットの霊力値をそのまま使う
                If new_plana > .MaxPlana Then
                    .Plana0 = .MaxPlana
                ElseIf new_plana < 0 Then
                    .Plana0 = 0
                Else
                    .Plana0 = new_plana
                End If
            End If
        End With
    End With
End Property

Public Property Get Plana0() As Long
    Plana0 = proPlana
End Property

Public Property Let Plana0(ByVal new_plana As Long)
    proPlana = new_plana
End Property


' === 特殊能力関連処理 ===

'特殊能力の総数
Public Function CountSkill() As Integer
    CountSkill = colSkill.Count
End Function

'特殊能力
Public Function Skill(Index As Variant) As String
Dim sd As SkillData

    Set sd = colSkill.Item(Index)
    Skill = sd.Name
End Function

'現在のレベルにおいて特殊能力 sname が使用可能か
Public Function IsSkillAvailable(sname As String) As Boolean
Dim sd As SkillData

    On Error GoTo ErrorHandler
    Set sd = colSkill.Item(sname)
    IsSkillAvailable = True
    Exit Function
    
ErrorHandler:
    
    '特殊能力付加＆強化による修正
    If Not Unit Is Nothing Then
        With Unit
            If .CountCondition = 0 Then
                Exit Function
            End If
            
            If .CountPilot = 0 Then
                Exit Function
            End If
            If Not Me Is .MainPilot And Not Me Is .Pilot(1) Then
                Exit Function
            End If

            If .IsConditionSatisfied(sname & "付加") Then
                IsSkillAvailable = True
                Exit Function
            ElseIf .IsConditionSatisfied(sname & "付加２") Then
                IsSkillAvailable = True
                Exit Function
            End If
            
            If .IsConditionSatisfied(sname & "強化") Then
                If .ConditionLevel(sname & "強化") > 0 Then
                    IsSkillAvailable = True
                    Exit Function
                End If
            End If
            If .IsConditionSatisfied(sname & "強化２") Then
                If .ConditionLevel(sname & "強化２") > 0 Then
                    IsSkillAvailable = True
                    Exit Function
                End If
            End If
        End With
    End If
    
    IsSkillAvailable = False
End Function

'現在のレベルにおいて特殊能力 sname が使用可能か
'(付加による影響を無視した場合)
Public Function IsSkillAvailable2(sname As String) As Boolean
Dim sd As SkillData

    On Error GoTo ErrorHandler
    Set sd = colSkill.Item(sname)
    IsSkillAvailable2 = True
    Exit Function
    
ErrorHandler:
    IsSkillAvailable2 = False
End Function

'現在のレベルにおける特殊能力 Index のレベル
'データでレベル指定がない場合はレベル 1
'特殊能力が使用不能の場合はレベル 0
Public Function SkillLevel(Index As Variant, Optional ref_mode As String) As Double
Dim sname As String, sd As SkillData

    On Error GoTo ErrorHandler
    Set sd = colSkill.Item(Index)
    With sd
        sname = .Name
        SkillLevel = .Level
    End With
    If SkillLevel = DEFAULT_LEVEL Then
        SkillLevel = 1
    End If
    
ErrorHandler:
    
    If sname = "" Then
        If IsNumeric(Index) Then
            Exit Function
        Else
            sname = CStr(Index)
        End If
    End If
    
    If ref_mode = "修正値" Then
        SkillLevel = 0
    ElseIf ref_mode = "基本値" Then
        Exit Function
    End If
    
    '重複可能な能力は特殊能力付加で置き換えられことはない
    Select Case sname
        Case "ハンター", "ＳＰ消費減少", "スペシャルパワー自動発動"
            If IsNumeric(Index) Then
                Exit Function
            End If
    End Select
    
    '特殊能力付加＆強化による修正
    If Unit Is Nothing Then
        Exit Function
    End If
    With Unit
        If .CountCondition = 0 Then
            Exit Function
        End If
        
        If .CountPilot = 0 Then
            Exit Function
        End If
        If Not Me Is .MainPilot And Not Me Is .Pilot(1) Then
            Exit Function
        End If
        
        If .IsConditionSatisfied(sname & "付加") Then
            SkillLevel = .ConditionLevel(sname & "付加")
            If SkillLevel = DEFAULT_LEVEL Then
                SkillLevel = 1
            End If
        ElseIf .IsConditionSatisfied(sname & "付加２") Then
            SkillLevel = .ConditionLevel(sname & "付加２")
            If SkillLevel = DEFAULT_LEVEL Then
                SkillLevel = 1
            End If
        End If
        
        If .IsConditionSatisfied(sname & "強化") Then
            SkillLevel = SkillLevel + .ConditionLevel(sname & "強化")
        End If
        If .IsConditionSatisfied(sname & "強化２") Then
            SkillLevel = SkillLevel + .ConditionLevel(sname & "強化２")
        End If
    End With
End Function

'特殊能力 Index にレベル指定がなされているか判定
Public Function IsSkillLevelSpecified(Index As Variant) As Boolean
Dim sname As String, sd As SkillData

    On Error GoTo ErrorHandler
    Set sd = colSkill.Item(Index)
    With sd
        If .Level <> DEFAULT_LEVEL Then
            IsSkillLevelSpecified = True
            sname = .Name
        End If
    End With
    
    Exit Function
    
ErrorHandler:
    
    If sname = "" Then
        If IsNumeric(Index) Then
            Exit Function
        Else
            sname = CStr(Index)
        End If
    End If
    
    '特殊能力付加＆強化による修正
    If Unit Is Nothing Then
        Exit Function
    End If
    With Unit
        If .CountCondition = 0 Then
            Exit Function
        End If
        
        If .CountPilot = 0 Then
            Exit Function
        End If
        If Not Me Is .MainPilot And Not Me Is .Pilot(1) Then
            Exit Function
        End If
        
        If .IsConditionSatisfied(sname & "付加") Then
            If .ConditionLevel(sname & "付加") <> DEFAULT_LEVEL Then
                IsSkillLevelSpecified = True
            End If
        ElseIf .IsConditionSatisfied(sname & "付加２") Then
            If .ConditionLevel(sname & "付加２") <> DEFAULT_LEVEL Then
                IsSkillLevelSpecified = True
            End If
        End If
        If .IsConditionSatisfied(sname & "強化") Then
            IsSkillLevelSpecified = True
        ElseIf .IsConditionSatisfied(sname & "強化２") Then
            IsSkillLevelSpecified = True
        End If
    End With
End Function

'特殊能力のデータ
Public Function SkillData(Index As Variant) As String
Dim sname As String, sd As SkillData

    On Error GoTo ErrorHandler
    Set sd = colSkill.Item(Index)
    With sd
        sname = .Name
        SkillData = .StrData
    End With
    
ErrorHandler:
    
    If sname = "" Then
        If IsNumeric(Index) Then
            Exit Function
        Else
            sname = CStr(Index)
        End If
    End If
    
    '重複可能な能力は特殊能力付加で置き換えられことはない
    Select Case sname
        Case "ハンター", "ＳＰ消費減少", "スペシャルパワー自動発動"
            If IsNumeric(Index) Then
                Exit Function
            End If
    End Select
    
    '特殊能力付加＆強化による修正
    If Unit Is Nothing Then
        Exit Function
    End If
    With Unit
        If .CountCondition = 0 Then
            Exit Function
        End If
        
        If .CountPilot = 0 Then
            Exit Function
        End If
        If Not Me Is .MainPilot And Not Me Is .Pilot(1) Then
            Exit Function
        End If
        
        If .IsConditionSatisfied(sname & "付加") Then
            SkillData = .ConditionData(sname & "付加")
        ElseIf .IsConditionSatisfied(sname & "付加２") Then
            SkillData = .ConditionData(sname & "付加２")
        End If
        
        If .IsConditionSatisfied(sname & "強化") Then
            If Len(.ConditionData(sname & "強化")) > 0 Then
                SkillData = .ConditionData(sname & "強化")
            End If
        End If
        If .IsConditionSatisfied(sname & "強化２") Then
            If Len(.ConditionData(sname & "強化２")) > 0 Then
                SkillData = .ConditionData(sname & "強化２")
            End If
        End If
    End With
End Function

'特殊能力の名称
Public Function SkillName(Index As Variant) As String
Dim sd As SkillData, sname As String
Dim buf As String, i As Integer

    'パイロットが所有している特殊能力の中から検索
    On Error GoTo ErrorHandler
    Set sd = colSkill.Item(Index)
    With sd
        sname = .Name
        
        '能力強化系は非表示
        If Right$(sname, 2) = "ＵＰ" Or Right$(sname, 4) = "ＤＯＷＮ" Then
            SkillName = "非表示"
            Exit Function
        End If
        
        Select Case sname
            Case "追加レベル", "メッセージ", "魔力所有"
                '非表示の能力
                SkillName = "非表示"
                Exit Function
            Case "得意技", "不得手"
                '別名指定が存在しない能力
                SkillName = sname
                Exit Function
        End Select
        
        If Len(.StrData) > 0 Then
            SkillName = LIndex(.StrData, 1)
            Select Case SkillName
                Case "非表示"
                    Exit Function
                Case "解説"
                    SkillName = "非表示"
                    Exit Function
            End Select
        Else
            SkillName = sname
        End If
        
        'レベル指定
        If .Level <> DEFAULT_LEVEL _
            And InStr(SkillName, "Lv") = 0 _
            And Left$(SkillName, 1) <> "(" _
        Then
            SkillName = SkillName & "Lv" & Format$(.Level)
        End If
    End With
    
ErrorHandler:
    
    If sname = "" Then
        If IsNumeric(Index) Then
            Exit Function
        Else
            sname = CStr(Index)
        End If
    End If
    
    If sname = "耐久" Then
        If IsOptionDefined("防御力成長") _
            Or IsOptionDefined("防御力レベルアップ") _
        Then
            '防御力成長オプション使用時には耐久能力を非表示
            SkillName = "非表示"
            Exit Function
        End If
    End If
    
    '得意技＆不得手は名称変更されない
    Select Case sname
        Case "得意技", "不得手"
            SkillName = sname
            Exit Function
    End Select
    
    'SetSkillコマンドで封印されている場合
    If SkillName = "" Then
        If IsGlobalVariableDefined("Ability(" & ID & "," & sname & ")") Then
            'オリジナルの名称を使用
            SkillName = Data.SkillName(Level, sname)
            
            If InStr(SkillName, "非表示") > 0 Then
                SkillName = "非表示"
                Exit Function
            End If
        End If
    End If
    
    '重複可能な能力は特殊能力付加で名称が置き換えられことはない
    Select Case sname
        Case "ハンター", "スペシャルパワー自動発動"
            If IsNumeric(Index) Then
                If Left$(SkillName, 1) = "(" Then
                    SkillName = Mid$(SkillName, 2)
                    SkillName = Left$(SkillName, InStr2(SkillName, ")") - 1)
                End If
                Exit Function
            End If
        Case "ＳＰ消費減少"
            If IsNumeric(Index) Then
                If Left$(SkillName, 1) = "(" Then
                    SkillName = Mid$(SkillName, 2)
                    SkillName = Left$(SkillName, InStr2(SkillName, ")") - 1)
                End If
                i = InStr(SkillName, "Lv")
                If i > 0 Then
                    SkillName = Left$(SkillName, i - 1)
                End If
                Exit Function
            End If
    End Select
    
    '特殊能力付加＆強化による修正
    If Not Unit Is Nothing Then
        With Unit
            If .CountCondition > 0 And .CountPilot > 0 Then
                If .MainPilot Is Me Or .Pilot(1) Is Me Then
                    'ユニット用特殊能力による付加
                    If .IsConditionSatisfied(sname & "付加２") Then
                        buf = LIndex(.ConditionData(sname & "付加２"), 1)
                        
                        If buf <> "" Then
                            SkillName = buf
                        ElseIf SkillName = "" Then
                            SkillName = sname
                        End If
                        
                        If InStr(SkillName, "非表示") > 0 Then
                            SkillName = "非表示"
                            Exit Function
                        End If
                        
                        'レベル指定
                        If .ConditionLevel(sname & "付加２") <> DEFAULT_LEVEL Then
                            If InStr(SkillName, "Lv") > 0 Then
                                SkillName = Left$(SkillName, InStr(SkillName, "Lv") - 1)
                            End If
                            SkillName = SkillName & "Lv" & Format$(.ConditionLevel(sname & "付加２"))
                        End If
                    End If
                    
                    'アビリティによる付加
                    If .IsConditionSatisfied(sname & "付加") Then
                        buf = LIndex(.ConditionData(sname & "付加"), 1)
                        
                        If buf <> "" Then
                            SkillName = buf
                        ElseIf SkillName = "" Then
                            SkillName = sname
                        End If
                        
                        If InStr(SkillName, "非表示") > 0 Then
                            SkillName = "非表示"
                            Exit Function
                        End If
                        
                        'レベル指定
                        If .ConditionLevel(sname & "付加") <> DEFAULT_LEVEL Then
                            If InStr(SkillName, "Lv") > 0 Then
                                SkillName = Left$(SkillName, InStr(SkillName, "Lv") - 1)
                            End If
                            SkillName = SkillName & "Lv" & Format$(.ConditionLevel(sname & "付加"))
                        End If
                    End If
                    
                    'ユニット用特殊能力による強化
                    If .IsConditionSatisfied(sname & "強化２") Then
                        If SkillName = "" Then
                            '強化される能力をパイロットが持っていなかった場合
                            SkillName = LIndex(.ConditionData(sname & "強化２"), 1)
                            
                            If SkillName = "" Then
                                SkillName = sname
                            End If
                            
                            If InStr(SkillName, "非表示") > 0 Then
                                SkillName = "非表示"
                                Exit Function
                            End If
                            
                            SkillName = SkillName & "Lv0"
                        End If
                        
                        If sname <> "同調率" _
                            And sname <> "霊力" _
                        Then
                            If .ConditionLevel(sname & "強化２") >= 0 Then
                                SkillName = SkillName & "+" & _
                                    Format$(.ConditionLevel(sname & "強化２"))
                            Else
                                SkillName = SkillName & _
                                    Format$(.ConditionLevel(sname & "強化２"))
                            End If
                        End If
                    End If
                    
                    'アビリティによる強化
                    If .IsConditionSatisfied(sname & "強化") Then
                        If SkillName = "" Then
                            '強化される能力をパイロットが持っていなかった場合
                            SkillName = LIndex(.ConditionData(sname & "強化"), 1)
                            
                            If SkillName = "" Then
                                SkillName = sname
                            End If
                            
                            If InStr(SkillName, "非表示") > 0 Then
                                SkillName = "非表示"
                                Exit Function
                            End If
                            
                            SkillName = SkillName & "Lv0"
                        End If
                        
                        If sname <> "同調率" _
                            And sname <> "霊力" _
                        Then
                            If .ConditionLevel(sname & "強化") >= 0 Then
                                SkillName = SkillName & "+" & _
                                    Format$(.ConditionLevel(sname & "強化"))
                            Else
                                SkillName = SkillName & _
                                    Format$(.ConditionLevel(sname & "強化"))
                            End If
                        End If
                    End If
                End If
            End If
        End With
    End If
    
    '能力強化系は非表示
    If Right$(sname, 2) = "ＵＰ" Or Right$(sname, 4) = "ＤＯＷＮ" Then
        SkillName = "非表示"
        Exit Function
    End If
    
    Select Case sname
        Case "追加レベル", "メッセージ", "魔力所有"
            '非表示の能力
            SkillName = "非表示"
            Exit Function
        Case "耐久"
            If IsOptionDefined("防御力成長") _
                Or IsOptionDefined("防御力レベルアップ") _
            Then
                '防御力成長オプション使用時には耐久能力を非表示
                SkillName = "非表示"
                Exit Function
            End If
    End Select
    
    'これらの能力からはレベル指定を除く
    Select Case sname
        Case "階級", "同調率", "霊力", "ＳＰ消費減少"
            i = InStr(SkillName, "Lv")
            If i > 0 Then
                SkillName = Left$(SkillName, i - 1)
            End If
    End Select
    
    'レベル非表示用の括弧を削除
    If Left$(SkillName, 1) = "(" Then
        SkillName = Mid$(SkillName, 2)
        SkillName = Left$(SkillName, InStr2(SkillName, ")") - 1)
    End If
    
    If SkillName = "" Then
        SkillName = sname
    End If
End Function

'特殊能力名称（レベル表示抜き）
Public Function SkillName0(Index As Variant) As String
Dim sd As SkillData, sname As String
Dim buf As String, i As Integer

    'パイロットが所有している特殊能力の中から検索
    On Error GoTo ErrorHandler
    Set sd = colSkill.Item(Index)
    With sd
        sname = .Name
        
        '能力強化系は非表示
        If Right$(sname, 2) = "ＵＰ" Or Right$(sname, 4) = "ＤＯＷＮ" Then
            SkillName0 = "非表示"
            Exit Function
        End If
        
        Select Case sname
            Case "追加レベル", "メッセージ", "魔力所有"
                '非表示の能力
                SkillName0 = "非表示"
                Exit Function
            Case "得意技", "不得手"
                '別名指定が存在しない能力
                SkillName0 = sname
                Exit Function
        End Select
        
        If Len(.StrData) > 0 Then
            SkillName0 = LIndex(.StrData, 1)
            
            If SkillName0 = "非表示" Then
                Exit Function
            End If
        Else
            SkillName0 = sname
        End If
    End With
    
ErrorHandler:
    
    If sname = "" Then
        If IsNumeric(Index) Then
            Exit Function
        Else
            sname = CStr(Index)
        End If
    End If
    
    If sname = "耐久" Then
        If IsOptionDefined("防御力成長") _
            Or IsOptionDefined("防御力レベルアップ") _
        Then
            '防御力成長オプション使用時には耐久能力を非表示
            SkillName0 = "非表示"
            Exit Function
        End If
    End If
    
    '得意技＆不得手は名称変更されない
    Select Case sname
        Case "得意技", "不得手"
            SkillName0 = sname
            Exit Function
    End Select
    
    'SetSkillコマンドで封印されている場合
    If SkillName0 = "" Then
        If IsGlobalVariableDefined("Ability(" & ID & "," & sname & ")") Then
            'オリジナルの名称を使用
            SkillName0 = Data.SkillName(Level, sname)
            
            If InStr(SkillName0, "非表示") > 0 Then
                SkillName0 = "非表示"
                Exit Function
            End If
        End If
    End If
    
    '重複可能な能力は特殊能力付加で名称が置き換えられことはない
    Select Case sname
        Case "ハンター", "ＳＰ消費減少", "スペシャルパワー自動発動"
            If IsNumeric(Index) Then
                Exit Function
            End If
    End Select
    
    '特殊能力付加＆強化による修正
    If Not Unit Is Nothing Then
        With Unit
            If .CountCondition > 0 And .CountPilot > 0 Then
                If .MainPilot Is Me Or .Pilot(1) Is Me Then
                    'ユニット用特殊能力による付加
                    If .IsConditionSatisfied(sname & "付加２") Then
                        buf = LIndex(.ConditionData(sname & "付加２"), 1)
                        
                        If buf <> "" Then
                            SkillName0 = buf
                        ElseIf SkillName0 = "" Then
                            SkillName0 = sname
                        End If
                        
                        If InStr(SkillName0, "非表示") > 0 Then
                            SkillName0 = "非表示"
                            Exit Function
                        End If
                    End If
                    
                    'アビリティによる付加
                    If .IsConditionSatisfied(sname & "付加") Then
                        buf = LIndex(.ConditionData(sname & "付加"), 1)
                        
                        If buf <> "" Then
                            SkillName0 = buf
                        ElseIf SkillName0 = "" Then
                            SkillName0 = sname
                        End If
                        
                        If InStr(SkillName0, "非表示") > 0 Then
                            SkillName0 = "非表示"
                            Exit Function
                        End If
                    End If
                    
                    'ユニット用特殊能力による強化
                    If SkillName0 = "" Then
                        If .IsConditionSatisfied(sname & "強化２") Then
                            SkillName0 = LIndex(.ConditionData(sname & "強化２"), 1)
                            
                            If SkillName0 = "" Then
                                SkillName0 = sname
                            End If
                            
                            If InStr(SkillName0, "非表示") > 0 Then
                                SkillName0 = "非表示"
                                Exit Function
                            End If
                        End If
                    End If
                    
                    'アビリティによる強化
                    If SkillName0 = "" Then
                        If .IsConditionSatisfied(sname & "強化") Then
                            SkillName0 = LIndex(.ConditionData(sname & "強化"), 1)
                            
                            If SkillName0 = "" Then
                                SkillName0 = sname
                            End If
                            
                            If InStr(SkillName0, "非表示") > 0 Then
                                SkillName0 = "非表示"
                                Exit Function
                            End If
                        End If
                    End If
                End If
            End If
        End With
    End If
    
    '該当するものが無ければエリアスから検索
    If SkillName0 = "" Then
        With ALDList
            For i = 1 To .Count
                With .Item(i)
                    If .AliasType(1) = sname Then
                        SkillName0 = .Name
                        Exit Function
                    End If
                End With
            Next
        End With
        SkillName0 = sname
    End If
    
    '能力強化系は非表示
    If Right$(sname, 2) = "ＵＰ" Or Right$(sname, 4) = "ＤＯＷＮ" Then
        SkillName0 = "非表示"
        Exit Function
    End If
    
    Select Case sname
        Case "追加レベル", "メッセージ", "魔力所有"
            '非表示の能力
            SkillName0 = "非表示"
            Exit Function
        Case "耐久"
            If IsOptionDefined("防御力成長") _
                Or IsOptionDefined("防御力レベルアップ") _
            Then
                '防御力成長オプション使用時には耐久能力を非表示
                SkillName0 = "非表示"
                Exit Function
            End If
    End Select
    
    'レベル非表示用の括弧を削除
    If Left$(SkillName0, 1) = "(" Then
        SkillName0 = Mid$(SkillName0, 2)
        SkillName0 = Left$(SkillName0, InStr2(SkillName0, ")") - 1)
    End If
    
    'レベル指定を削除
    i = InStr(SkillName0, "Lv")
    If i > 0 Then
        SkillName0 = Left$(SkillName0, i - 1)
    End If
End Function

'特殊能力名称（必要技能判定用）
'名称からレベル指定を削除し、名称が非表示にされている場合は元の特殊能力名
'もしくはエリアス名を使用する。
Public Function SkillNameForNS(stype As String) As String
Dim sd As SkillData
Dim buf As String, i As Integer

    '非表示の特殊能力
    If Right$(stype, 2) = "ＵＰ" _
        Or Right$(stype, 4) = "ＤＯＷＮ" _
    Then
        SkillNameForNS = stype
        Exit Function
    End If
    If stype = "メッセージ" Then
        SkillNameForNS = stype
        Exit Function
    End If
    
    'パイロットが所有している特殊能力の中から検索
    On Error GoTo ErrorHandler
    Set sd = colSkill.Item(stype)
    With sd
        If Len(.StrData) > 0 Then
            SkillNameForNS = LIndex(.StrData, 1)
        Else
            SkillNameForNS = stype
        End If
    End With
    
ErrorHandler:
    
    'SetSkillコマンドで封印されている場合
    If SkillNameForNS = "" Then
        If IsGlobalVariableDefined("Ability(" & ID & "," & stype & ")") Then
            'オリジナルの名称を使用
            SkillNameForNS = Data.SkillName(Level, stype)
            
            If InStr(SkillNameForNS, "非表示") > 0 Then
                SkillNameForNS = "非表示"
            End If
        End If
    End If
    
    '特殊能力付加＆強化による修正
    If Not Unit Is Nothing Then
        With Unit
            If .CountCondition > 0 And .CountPilot > 0 Then
                If Me Is .MainPilot Or Me Is .Pilot(1) Then
                    'ユニット用特殊能力による付加
                    If .IsConditionSatisfied(stype & "付加２") Then
                        buf = LIndex(.ConditionData(stype & "付加２"), 1)
                        
                        If buf <> "" Then
                            SkillNameForNS = buf
                        ElseIf SkillNameForNS = "" Then
                            SkillNameForNS = stype
                        End If
                        
                        If InStr(SkillNameForNS, "非表示") > 0 Then
                            SkillNameForNS = "非表示"
                        End If
                    End If
                    
                    'アビリティによる付加
                    If .IsConditionSatisfied(stype & "付加") Then
                        buf = LIndex(.ConditionData(stype & "付加"), 1)
                        
                        If buf <> "" Then
                            SkillNameForNS = buf
                        ElseIf SkillNameForNS = "" Then
                            SkillNameForNS = stype
                        End If
                        
                        If InStr(SkillNameForNS, "非表示") > 0 Then
                            SkillNameForNS = "非表示"
                        End If
                    End If
                    
                    'ユニット用特殊能力による強化
                    If SkillNameForNS = "" Then
                        If .IsConditionSatisfied(stype & "強化２") Then
                            SkillNameForNS = LIndex(.ConditionData(stype & "強化２"), 1)
                            
                            If SkillNameForNS = "" Then
                                SkillNameForNS = stype
                            End If
                            
                            If InStr(SkillNameForNS, "非表示") > 0 Then
                                SkillNameForNS = "非表示"
                            End If
                        End If
                    End If
                    
                    'アビリティによる強化
                    If SkillNameForNS = "" Then
                        If .IsConditionSatisfied(stype & "強化") Then
                            SkillNameForNS = LIndex(.ConditionData(stype & "強化"), 1)
                            
                            If SkillNameForNS = "" Then
                                SkillNameForNS = stype
                            End If
                            
                            If InStr(SkillNameForNS, "非表示") > 0 Then
                                SkillNameForNS = "非表示"
                            End If
                        End If
                    End If
                End If
            End If
        End With
    End If
    
    '該当するものが無ければエリアスから検索
    If SkillNameForNS = "" Or SkillNameForNS = "非表示" Then
        With ALDList
            For i = 1 To .Count
                With .Item(i)
                    If .AliasType(1) = stype Then
                        SkillNameForNS = .Name
                        Exit Function
                    End If
                End With
            Next
        End With
        SkillNameForNS = stype
    End If
    
    'レベル非表示用の括弧を削除
    If Left$(SkillNameForNS, 1) = "(" Then
        SkillNameForNS = Mid$(SkillNameForNS, 2)
        SkillNameForNS = Left$(SkillNameForNS, InStr2(SkillNameForNS, ")") - 1)
    End If
    
    'レベル表示を削除
    i = InStr(SkillNameForNS, "Lv")
    If i > 0 Then
        SkillNameForNS = Left$(SkillNameForNS, i - 1)
    End If
End Function

'特殊能力の種類
Public Function SkillType(sname As String) As String
Dim i As Integer, sd As SkillData
Dim sname0 As String, sname2 As String
    
    If sname = "" Then
        Exit Function
    End If
    
    i = InStr(sname, "Lv")
    If i > 0 Then
        sname0 = Left$(sname, i - 1)
    Else
        sname0 = sname
    End If
    
    'エリアスデータが定義されている？
    If ALDList.IsDefined(sname0) Then
        With ALDList.Item(sname0)
            SkillType = .AliasType(1)
            Exit Function
        End With
    End If
        
    '特殊能力一覧から検索
    For Each sd In colSkill
        With sd
            If sname0 = .Name Then
                SkillType = .Name
                Exit Function
            End If
            
            sname2 = LIndex(.StrData, 1)
            If sname0 = sname2 Then
                SkillType = .Name
                Exit Function
            End If
            
            If Left$(sname2, 1) = "(" Then
                If Right$(sname2, 1) = ")" Then
                    sname2 = Mid$(sname2, 2, Len(sname2) - 2)
                    If sname = sname2 Then
                        SkillType = .Name
                        Exit Function
                    End If
                End If
            End If
        End With
    Next
    
    'その能力を修得していない
    SkillType = sname0
    
    '特殊能力付加による修正
    If Not Unit Is Nothing Then
        With Unit
            If .CountCondition And .CountPilot > 0 Then
                If Me Is .MainPilot Or Me Is .Pilot(1) Then
                    For i = 1 To .CountCondition
                        If Right$(.Condition(i), 2) = "付加" Then
                            If LIndex(.ConditionData(i), 1) = sname0 Then
                                SkillType = .Condition(i)
                                SkillType = Left$(SkillType, Len(SkillType) - 2)
                                Exit For
                            End If
                        ElseIf Right$(.Condition(i), 3) = "付加２" Then
                            If LIndex(.ConditionData(i), 1) = sname0 Then
                                SkillType = .Condition(i)
                                SkillType = Left$(SkillType, Len(SkillType) - 3)
                                Exit For
                            End If
                        End If
                    Next
                End If
            End If
        End With
    End If
End Function


' === スペシャルパワー関連処理 ===

'スペシャルパワーの個数
Public Property Get CountSpecialPower() As Integer
    If Data.SP <= 0 Then
        'ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
        If Not Unit Is Nothing Then
            With Unit
                If .CountPilot > 0 Then
                    If Not .Pilot(1) Is Me Then
                        If .MainPilot Is Me Then
                            CountSpecialPower = .Pilot(1).Data.CountSpecialPower(Level)
                            Exit Property
                        End If
                    End If
                End If
            End With
        End If
    End If
    
    CountSpecialPower = Data.CountSpecialPower(Level)
End Property

'idx番目のスペシャルパワー
Public Property Get SpecialPower(ByVal idx As Integer) As String
    If Data.SP <= 0 Then
        'ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
        If Not Unit Is Nothing Then
            With Unit
                If .CountPilot > 0 Then
                    If Not .Pilot(1) Is Me Then
                        If .MainPilot Is Me Then
                            SpecialPower = .Pilot(1).Data.SpecialPower(Level, idx)
                            Exit Property
                        End If
                    End If
                End If
            End With
        End If
    End If
    
    SpecialPower = Data.SpecialPower(Level, idx)
End Property

'スペシャルパワー sname を修得しているか？
Public Function IsSpecialPowerAvailable(sname As String) As Boolean
    If Data.SP <= 0 Then
        'ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
        If Not Unit Is Nothing Then
            With Unit
                If .CountPilot > 0 Then
                    If Not .Pilot(1) Is Me Then
                        If .MainPilot Is Me Then
                            IsSpecialPowerAvailable = _
                                Unit.Pilot(1).Data.IsSpecialPowerAvailable(Level, sname)
                            Exit Function
                        End If
                    End If
                End If
            End With
        End If
    End If
    
    IsSpecialPowerAvailable = Data.IsSpecialPowerAvailable(Level, sname)
End Function

'スペシャルパワー sname が有用か？
Public Function IsSpecialPowerUseful(sname As String) As Boolean
    IsSpecialPowerUseful = SPDList.Item(sname).Useful(Me)
End Function

'スペシャルパワー sname に必要なＳＰ値
Public Function SpecialPowerCost(sname As String) As Integer
Dim i As Integer, j As Integer
Dim adata As String

    If Data.SP <= 0 Then
        'ＳＰを持たない追加パイロットの場合は１番目のパイロットのデータを使う
        If Not Unit Is Nothing Then
            With Unit
                If .CountPilot > 0 Then
                    If Not .Pilot(1) Is Me Then
                        If .MainPilot Is Me Then
                            SpecialPowerCost = .Pilot(1).SpecialPowerCost(sname)
                            Exit Function
                        End If
                    End If
                End If
            End With
        End If
    End If
    
    '基本消費ＳＰ値
    SpecialPowerCost = Data.SpecialPowerCost(sname)
    
    '特殊能力による消費ＳＰ値修正
    If IsSkillAvailable("超能力") _
        Or IsSkillAvailable("集中力") _
    Then
        SpecialPowerCost = 0.8 * SpecialPowerCost
    End If
    If IsSkillAvailable("知覚強化") Then
        SpecialPowerCost = 1.2 * SpecialPowerCost
    End If
    
    'ＳＰ消費減少能力
    If Not Unit Is Nothing Then
        With Unit
            If .CountPilot > 0 Then
                If .MainPilot Is Me Then
                    If .IsConditionSatisfied("ＳＰ消費減少付加") _
                        Or .IsConditionSatisfied("ＳＰ消費減少付加２") _
                    Then
                        adata = SkillData("ＳＰ消費減少")
                        For i = 2 To LLength(adata)
                            If sname = LIndex(adata, i) Then
                                SpecialPowerCost = _
                                    (10 - SkillLevel("ＳＰ消費減少")) * SpecialPowerCost \ 10
                                Exit Function
                            End If
                        Next
                    End If
                End If
            End If
        End With
    End If
    For i = 1 To CountSkill
        If Skill(i) = "ＳＰ消費減少" Then
            adata = SkillData(i)
            For j = 2 To LLength(adata)
                If sname = LIndex(adata, j) Then
                    SpecialPowerCost = (10 - SkillLevel(i)) * SpecialPowerCost \ 10
                    Exit Function
                End If
            Next
        End If
    Next
End Function

'スペシャルパワー sname を実行する
Public Sub UseSpecialPower(sname As String, Optional ByVal sp_mod As Double = 1)
Dim my_unit As Unit

    If Not SPDList.IsDefined(sname) Then
        Exit Sub
    End If
    
    ClearUnitStatus
    
    Set SelectedPilot = Me
    
    'スペシャルパワー使用メッセージ
    If sp_mod <> 2 _
        And Not SPDList.Item(sname).IsEffectAvailable("復活") _
        And Not SPDList.Item(sname).IsEffectAvailable("自爆") _
    Then
        If Unit.IsMessageDefined(sname) Then
            OpenMessageForm
            Unit.PilotMessage sname
            CloseMessageForm
        End If
    End If
    
    '同じ追加パイロットを持つユニットが複数いる場合、パイロットのUnitが
    '変化してしまうことがあるため、元のUnitを記録しておく
    Set my_unit = Unit
    
    'スペシャルパワーアニメを表示
    If Not SPDList.Item(sname).PlayAnimation Then
        'メッセージ表示のみ
        OpenMessageForm Unit
        DisplaySysMessage Nickname & "は" & sname & "を使った。"
    End If
    
    'Unitが変化した場合に元に戻す
    If Not my_unit Is Unit Then
        my_unit.MainPilot
    End If
    
    'スペシャルパワーを実行
    SPDList.Item(sname).Execute Me
    
    'Unitが変化した場合に元に戻す
    If Not my_unit Is Unit Then
        my_unit.CurrentForm.MainPilot
    End If
    
    SP = SP - sp_mod * SpecialPowerCost(sname)
    
    CloseMessageForm
End Sub


' === ユニット搭乗＆下乗関連処理 ===

'ユニット u に搭乗
Public Sub Ride(u As Unit, Optional ByVal is_support As Boolean)
Dim hp_ratio As Double, en_ratio As Double
Dim plana_ratio As Double

    '既に乗っていればなにもしない
    If Unit Is u Then
        Exit Sub
    End If
    
    With u
        hp_ratio = 100 * .HP / .MaxHP
        en_ratio = 100 * .EN / .MaxEN
        
        '現在の霊力値を記録
        If MaxPlana > 0 Then
            plana_ratio = 100 * Plana / MaxPlana
        Else
            plana_ratio = -1
        End If
        
        Set Unit = u
        
        If InStrNotNest(Class, "サポート)") > 0 _
            And LLength(Class) = 1 _
            And Not .IsFeatureAvailable("ダミーユニット") _
        Then
            'サポートにしかなれないパイロットの場合
            .AddSupport Me
        ElseIf IsSupport(u) Then
            '同じユニットクラスに対して通常パイロットとサポートの両方のパターン
            'がいける場合は通常パイロットを優先
            If .CountPilot < Abs(.Data.PilotNum) _
                And InStrNotNest(Class, u.Class0 & " ") > 0 _
                And Not is_support _
            Then
                .AddPilot Me
            Else
                .AddSupport Me
            End If
        Else
            'パイロットが既に規定数の場合は全パイロットを降ろす
            If .CountPilot = Abs(.Data.PilotNum) Then
                .Pilot(1).GetOff
            End If
            .AddPilot Me
        End If
        
        'Pilotコマンドで作成されたパイロットは全て味方なので搭乗時に変更が必要
        Party = .Party0
        
        'ユニットのステータスをアップデート
        .Update
        
        '霊力値のアップデート
        If plana_ratio >= 0 Then
            Plana = MaxPlana * plana_ratio \ 100
        Else
            Plana = MaxPlana
        End If
        
        'パイロットが乗り込むことによるＨＰ＆ＥＮの増減に対応
        .HP = .MaxHP * hp_ratio \ 100
        .EN = .MaxEN * en_ratio \ 100
    End With
End Sub

'パイロットをユニットから降ろす
Public Sub GetOff(Optional ByVal without_leave As Boolean)
Dim i As Integer
    
    '既に降りている？
    If Unit Is Nothing Then
        Exit Sub
    End If
    
    With Unit
        For i = 1 To .CountSupport
            If .Support(i) Is Me Then
                'サポートパイロットとして乗り込んでいる場合
                .DeleteSupport i
                .Update
                Set Unit = Nothing
                Update
                Exit Sub
            End If
        Next
        
        '出撃していた場合は退却
        If Not without_leave Then
            If .Status = "出撃" Then
                .Status = "待機"
                Set MapDataForUnit(.X, .Y) = Nothing
                EraseUnitBitmap .X, .Y, False
            End If
        End If
        
        '通常のパイロットの場合は、そのユニットに乗っていた他のパイロットも降ろされる
        For i = 1 To .CountPilot
            Set .Pilot(1).Unit = Nothing
            .DeletePilot 1
        Next
        For i = 1 To .CountSupport
            Set .Support(1).Unit = Nothing
            .DeleteSupport 1
        Next
        
        .Update
    End With
    
    Set Unit = Nothing
    
    Update
End Sub

'パイロットがユニット u のサポートかどうか
Public Function IsSupport(u As Unit) As Boolean
Dim uclass As String, pclass As String
Dim i As Integer, j As Integer

    With u
        If .IsFeatureAvailable("ダミーユニット") Then
            'ダミーユニットの場合はサポートパイロットも通常のパイロットとして扱う
            IsSupport = False
            Exit Function
        End If
        
        'サポート指定が存在する？
        If InStrNotNest(Class, "サポート)") = 0 Then
            IsSupport = False
            Exit Function
        End If
        
        If .CountPilot = 0 Then
            'パイロットが乗っていないユニットの場合は通常パイロットを優先
            For i = 1 To LLength(Class)
                pclass = LIndex(Class, i)
                If .Class = pclass _
                    Or .Class = pclass & "(" & Name & "専用)" _
                    Or .Class = pclass & "(" & Nickname & "専用)" _
                    Or .Class = pclass & "(" & Sex & "専用)" _
                Then
                    '通常のパイロットとして搭乗可能であればサポートでないとみなす
                    IsSupport = False
                    Exit Function
                End If
            Next
        Else
            '通常のパイロットとして搭乗している？
            For i = 1 To .CountPilot
                If .Pilot(i) Is Me Then
                    IsSupport = False
                    Exit Function
                End If
            Next
        End If
        
        uclass = .Class0
        
        '通常のサポート？
        For i = 1 To LLength(Class)
            If uclass & "(サポート)" = LIndex(Class, i) Then
                IsSupport = True
                Exit Function
            End If
        Next
        
        'パイロットが乗っていないユニットの場合はここで終了
        If .CountPilot = 0 Then
            IsSupport = False
            Exit Function
        End If
        
        '専属サポート？
        With .MainPilot
            For i = 1 To LLength(Class)
                pclass = LIndex(Class, i)
                If pclass = uclass & "(" & .Name & "専属サポート)" _
                    Or pclass = uclass & "(" & .Nickname & "専属サポート)" _
                    Or pclass = uclass & "(" & .Sex & "専属サポート)" _
                Then
                    IsSupport = True
                    Exit Function
                End If
                
                For j = 1 To .CountSkill
                    If pclass = uclass & "(" & .Skill(j) & "専属サポート)" Then
                        IsSupport = True
                        Exit Function
                    End If
                Next
            Next
        End With
    End With
    
    IsSupport = False
End Function

'ユニット u に乗ることができるかどうか
Public Function IsAbleToRide(u As Unit) As Boolean
Dim uclass As String, pclass As String
Dim i As Integer

    With u
        '汎用ユニットは必要技能を満たせばＯＫ
        If .Class = "汎用" Then
            IsAbleToRide = True
            GoTo CheckNecessarySkill
        End If
        
        '人間ユニット指定を除いて判定
        If Left$(.Class, 1) = "(" And Right$(.Class, 1) = ")" Then
            uclass = Mid$(.Class, 2, Len(.Class) - 2)
        Else
            uclass = .Class
        End If
        
        'サポートかどうかをまず判定しておく
        If IsSupport(u) Then
            IsAbleToRide = True
            '必要技能をチェックする
            GoTo CheckNecessarySkill
        End If
        
        For i = 1 To LLength(Class) 'ユニットクラスは複数設定可能
            pclass = LIndex(Class, i)
            If uclass = pclass _
                Or uclass = pclass & "(" & Nickname & "専用)" _
                Or uclass = pclass & "(" & Name & "専用)" _
                Or uclass = pclass & "(" & Sex & "専用)" _
            Then
                IsAbleToRide = True
                '必要技能をチェックする
                GoTo CheckNecessarySkill
            End If
        Next
        
        'クラスが合わない
        IsAbleToRide = False
        Exit Function
        
CheckNecessarySkill:
        
        '必要技能＆不必要技能をチェック
        
        '両能力を持っていない場合はチェック不要
        If Not .IsFeatureAvailable("必要技能") _
            And Not .IsFeatureAvailable("不必要技能") _
        Then
            Exit Function
        End If
        
        For i = 1 To .CountFeature
            If .Feature(i) = "必要技能" Then
                If Not .IsNecessarySkillSatisfied(.FeatureData(i), Me) Then
                    IsAbleToRide = False
                    Exit Function
                End If
            ElseIf .Feature(i) = "不必要技能" Then
                If .IsNecessarySkillSatisfied(.FeatureData(i), Me) Then
                    IsAbleToRide = False
                    Exit Function
                End If
            End If
        Next
    End With
End Function


' === 一時中断関連処理 ===

'一時中断用データをファイルにセーブする
Public Sub Dump()
    Write #SaveDataFileNumber, Name, ID, Party
    Write #SaveDataFileNumber, Level, Exp
    Write #SaveDataFileNumber, SP, Morale, Plana
    Write #SaveDataFileNumber, Alive, Away, SupportIndex
    If Unit Is Nothing Then
        Write #SaveDataFileNumber, "-"
    Else
        Write #SaveDataFileNumber, Unit.ID
    End If
End Sub

'一時中断用データをファイルからロードする
Public Sub Restore()
Dim sbuf As String, ibuf As Integer, bbuf As Boolean

    'Name, ID, Party
    Input #SaveDataFileNumber, sbuf
    Name = sbuf
    Input #SaveDataFileNumber, sbuf
    ID = sbuf
    Input #SaveDataFileNumber, sbuf
    Party = sbuf
    
    'Leve, Exp
    Input #SaveDataFileNumber, ibuf
    Level = ibuf
    Input #SaveDataFileNumber, ibuf
    Exp = ibuf
    
    'SP, Morale, Plana
    Input #SaveDataFileNumber, ibuf
    SP = ibuf
    Input #SaveDataFileNumber, ibuf
    Morale = ibuf
    Input #SaveDataFileNumber, ibuf
    Plana = ibuf
    
    'Alive, Away, SupportIndex
    Input #SaveDataFileNumber, bbuf
    Alive = bbuf
    Input #SaveDataFileNumber, bbuf
    Away = bbuf
    Input #SaveDataFileNumber, ibuf
    SupportIndex = ibuf
    
    'Unit
    Line Input #SaveDataFileNumber, sbuf
End Sub

'一時中断用データのリンク情報をファイルからロードする
Public Sub RestoreLinkInfo()
Dim sbuf As String, ibuf As Integer

    'Name, ID, Party
    Line Input #SaveDataFileNumber, sbuf
    
    'Leve, Exp
    Line Input #SaveDataFileNumber, sbuf
    
    'SP, Morale, Plana
    Line Input #SaveDataFileNumber, sbuf
    
    'Alive, Away, SupportIndex
    Line Input #SaveDataFileNumber, sbuf
    
    'Unit
    Input #SaveDataFileNumber, sbuf
    Set Unit = UList.Item(sbuf)
End Sub

'一時中断用データのパラメータ情報をファイルからロードする
Public Sub RestoreParameter()
Dim sbuf As String, ibuf As Integer

    'Name, ID, Party
    Line Input #SaveDataFileNumber, sbuf
    
    'Leve, Exp
    Line Input #SaveDataFileNumber, sbuf
    
    'SP, Morale, Plana
    Input #SaveDataFileNumber, ibuf
    SP = ibuf
    Input #SaveDataFileNumber, ibuf
    Morale = ibuf
    Input #SaveDataFileNumber, ibuf
    Plana = ibuf
    
    'Alive, Away, SupportIndex
    Line Input #SaveDataFileNumber, sbuf
    
    'Unit
    Line Input #SaveDataFileNumber, sbuf
End Sub


' === その他 ===

'全回復
Public Sub FullRecover()
    '闘争本能によって初期気力は変化する
    If IsSkillAvailable("闘争本能") Then
        If MinMorale > 100 Then
            SetMorale MinMorale + 5 * SkillLevel("闘争本能")
        Else
            SetMorale 100 + 5 * SkillLevel("闘争本能")
        End If
    Else
        SetMorale 100
    End If
    
    SP = MaxSP
    Plana = MaxPlana
End Sub

'同調率
Public Function SynchroRate() As Integer
Dim lv As Integer

    If Not IsSkillAvailable("同調率") Then
        Exit Function
    End If
    
    '同調率基本値
    SynchroRate = SkillLevel("同調率")
    
    'レベルによる増加分
    lv = MinLng(Level, 100)
    If IsSkillAvailable("同調率成長") Then
        SynchroRate = SynchroRate + lv * (10 + SkillLevel("同調率成長")) \ 10
    Else
        SynchroRate = SynchroRate + lv
    End If
End Function

'指揮範囲
Public Function CommandRange() As Integer
    '指揮能力を持っていなければ範囲は0
    If Not IsSkillAvailable("指揮") Then
        CommandRange = 0
        Exit Function
    End If
    
    '指揮能力を持っている場合は階級レベルに依存
    Select Case SkillLevel("階級")
        Case 0 To 6
            CommandRange = 2
        Case 7 To 9
            CommandRange = 3
        Case 10 To 12
            CommandRange = 4
        Case Else
            CommandRange = 5
    End Select
End Function

'行動決定に用いられる戦闘判断力
Public Function TacticalTechnique0() As Integer
    TacticalTechnique0 = TechniqueBase - Level + 10 * SkillLevel("戦術")
End Function

Public Function TacticalTechnique() As Integer
    '正常な判断能力がある？
    If Not Unit Is Nothing Then
        With Unit
            If .IsConditionSatisfied("混乱") _
                Or .IsConditionSatisfied("暴走") _
                Or .IsConditionSatisfied("狂戦士") _
            Then
                Exit Function
            End If
        End With
    End If
    
    TacticalTechnique = TacticalTechnique0
End Function

'イベントコマンド SetRelation で設定した値を返す
Public Function Relation(t As Pilot) As Integer
    Relation = GetValueAsLong("関係:" & Name & ":" & t.Name)
End Function

'射撃能力が「魔力」と表示されるかどうか
Public Function HasMana() As Boolean
    If IsSkillAvailable("術") Or IsSkillAvailable("魔力所有") Then
        HasMana = True
        Exit Function
    End If
    
    If IsOptionDefined("魔力使用") Then
        HasMana = True
        Exit Function
    End If
    
    HasMana = False
End Function
